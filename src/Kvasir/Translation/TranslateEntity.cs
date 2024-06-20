using Cybele.Extensions;
using Kvasir.Annotations;
using Kvasir.Core;
using Kvasir.Extraction;
using Kvasir.Schema;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Kvasir.Translation {
    internal sealed partial class Translator {
        /// <summary>
        ///   Translates only the Principal Table for an Entity Type, ignoring all Relations.
        /// </summary>
        /// <remarks>
        ///   This method is invoked from two different evaluation streams. The first is when translating an Entity in
        ///   full, such as when the <see cref="this[Type]">index operator</see> is used. In this case, translation of
        ///   the Principal Table will be immediately followed by translation of the Entity's Relations. The second is
        ///   <i>during</i> the translation of a Principal Table or a Relation when a Reference property is encountered.
        ///   In this case, we need to delay the translation of Relations until a later time, since Relations are
        ///   allowed to contain reference cycles.
        /// </remarks>
        /// <param name="context">
        ///   The <see cref="Context"/> in which <paramref name="source"/> is being translated. This <b>should</b>
        ///   include that type.
        /// </param>
        /// <param name="source">
        ///   The Entity Type.
        /// </param>
        /// <returns>
        ///   The <see cref="PrincipalTableDef"/> for <paramref name="source"/>.
        /// </returns>
        /// <exception cref="InvalidEntityTypeException">
        ///   if <paramref name="source"/> is not a valid Entity Type (e.g. it is an interface, or is a generic, or is
        ///   <see langword="abstract"/>, etc.).
        /// </exception>
        /// <exception cref="NotEnoughFieldsException">
        ///   if <paramref name="source"/> does not contribute at least 2 Fields to the data model for its Principal
        ///   Table.
        /// </exception>
        /// <exception cref="DuplicateNameException">
        ///   if 2 or more Fields in the data model of the Principal Table for <paramref name="source"/> have the same
        ///   name
        ///     --or--
        ///   if the name of the Principal Table of <paramref name="source"/> is already taken by another Entity's
        ///   Principal Table or by some Relation Table.
        /// </exception>
        private PrincipalTableDef TranslatePrincipalTable(Context context, Type source) {
            Debug.Assert(context is not null);
            Debug.Assert(source is not null);

            // Memoization
            if (principalTableCache_.TryGetValue(source, out PrincipalTableDef? principal)) {
                Debug.Assert(pkCache_.ContainsKey(source));
                return principal;
            }
            Debug.Assert(!pkCache_.ContainsKey(source));

            // The error checking for invalid Entity types has to be here rather than in operator[], because the latter
            // is invoked only from the top-level API whereas the former is invoked from both the top-level API (via the
            // latter) and when a Reference-type property is encountered. Many of these checks are only relevant when
            // translating the top-level Entity, as the logic for translating individual properties performs many of the
            // same checks.
            //
            // Note that we HAVE NOT adjusted the context before doing this check and potentially throwing a
            // contextualized exception. That is okay: this function is either called from the operator[] API or from
            // `translateType` on a Reference property, and in the latter case we have already performed the necessary
            // checking to ensure that this check will not fail.
            var category = source.TranslationCategory();
            if (!category.Equals(TypeCategory.Class)) {
                throw new InvalidEntityTypeException(context, category);
            }

            var fieldGroups = TranslateType(context, source, true).ToList();
            var schemas = fieldGroups.OrderBy(g => g.Column.Unwrap()).SelectMany(g => g).Select(d => d.MakeSchema(settings_)).ToList();
            var fields = schemas.Select(s => s.Field).ToList();

            if (fields.Count < 2) {
                throw new NotEnoughFieldsException(context, 2, fields.Count);
            }

            var duplicates = fields.GroupBy(f => f.Name).Where(g => g.Count() != 1);
            if (!duplicates.IsEmpty()) {
                throw new DuplicateNameException(context, duplicates.First().Key);
            }

            (var primaryKey, var candidateKeys) = KeyTranslator.ComputeKeys(context, source, schemas);
            var foreignKeys = CreateForeignKeys(fieldGroups);
            var constraints = schemas.SelectMany(s => s.CHECKs).Concat(GetTableConstraints(context, source, schemas, settings_)).ToList();

            // We have to reverse-engineer the Primary Key back into FieldGroup/FieldDescriptor form, because when we
            // interact with it later for Reference-type Fields we need the structure intact. Specifically, we need to
            // be able to apply Path-based annotations to the Fields in the Primary Key using their original paths. To
            // do this, we extract out the Descriptors from Schemas whose Field is in the Primary Key, then filter each
            // of the groups using those Descriptors (keeping only the non-empty groups). The filter operation performs
            // an additional clone
            var pkDescriptors = schemas.Where(s => primaryKey.Fields.Contains(s.Field)).Select(s => s.Descriptor);
            var pkGroups = fieldGroups.Select(g => g.Filter(pkDescriptors)).Where(o => o.HasValue).Select(o => o.Unwrap());
            pkCache_[source] = pkGroups.ToList();

            var tableName = GetTableName(context, source);
            if (tableNameCache_.TryGetValue(tableName, out Type? match)) {
                throw new DuplicateNameException(context, tableName, match);
            }

            var table = new Table(tableName, fields, primaryKey, candidateKeys, foreignKeys, constraints);
            var extractor = new DataExtractionPlan(fieldGroups.OrderBy(g => g.Column.Unwrap()).Select(g => g.Extractor));
            principal = new PrincipalTableDef(table, extractor, null!);

            principalTableCache_.Add(source, principal);
            tableNameCache_.Add(tableName, source);
            return principal;
        }

        /// <summary>
        ///   Translates the Relation Tables "owned" by an Entity Type.
        /// </summary>
        /// <param name="source">
        ///   The "owning" Entity Type.
        /// </param>
        /// <returns>
        ///   A collection of <see cref="RelationTableDef">Table definitions</see>, in no particular order, for the
        ///   Relation Tables "owned" by <paramref name="source"/>.
        /// </returns>
        /// <exception cref="DuplicateNameException">
        ///   if 2 or more Fields in the data model of any Relation Table "owned" by <paramref name="source"/> have the
        ///   same name
        ///     --or--
        ///   if the name of any of the Relation Tables "owned" by <paramref name="source"/> is already taken by another
        ///   Entity's Principal Table or by some other Relation Table (including another Relation Table "owned" by
        ///   <paramref name="source"/>).
        /// </exception>
        private List<RelationTableDef> TranslateRelationTables(Type source) {
            Debug.Assert(source is not null && source.IsClass);
            Debug.Assert(principalTableCache_.ContainsKey(source));
            Debug.Assert(pkCache_.ContainsKey(source));
            Debug.Assert(relationTrackersCache_.ContainsKey(source));

            // Because a Relation Table is translated from the combination of a particular Entity Type and a particular
            // nested property, there's no need for memoization: each combination will only ever be translated once.
            // However, the owning Entity will have been memoized when initially translated, and the element type will
            // be memoized if it is an Aggregate or a Reference.

            var relationTables = new List<RelationTableDef>();
            foreach (var tracker in relationTrackersCache_[source]) {
                var property = tracker.Property;
                var syntheticType = SyntheticType.MakeSyntheticType(source, tracker);
                var context = new Context(syntheticType);

                var relationGroup = new RelationFieldGroup(context, property, TranslateType(context, syntheticType, false));
                var schemas = relationGroup.Select(d => d.MakeSchema(settings_)).ToList();
                var fields = schemas.Select(s => s.Field).ToList();
                Debug.Assert(fields.Count >= 2);

                var duplicates = fields.GroupBy(f => f.Name).Where(g => g.Count() != 1);
                if (!duplicates.IsEmpty()) {
                    throw new DuplicateNameException(context, duplicates.First().Key);
                }

                // A RelationTable can still have CHECK constraints from its constituent members (specifically, the
                // element type if it is an Aggregate), but it cannot have any Type-scope custom CHECK annotations
                (var primaryKey, var candidateKeys) = KeyTranslator.ComputeKeys(context, source, schemas);
                var foreignKeys = CreateForeignKeys(Enumerable.Repeat(relationGroup, 1));
                var constraints = schemas.SelectMany(s => s.CHECKs).ToList();

                var tableName = relationGroup.TableName.ValueOr(GetTableName(tracker, source));
                if (tableNameCache_.TryGetValue(tableName, out Type? match)) {
                    throw new DuplicateNameException(context, tableName, match);
                }

                var extractRelationProperty = new ReadPropertyExtractor(property);
                var elementExtractor = new DataExtractionPlan(Enumerable.Repeat(relationGroup.Extractor, 1));

                var table = new Table(tableName, fields, primaryKey, candidateKeys, foreignKeys, constraints);
                var extractor = new RelationExtractionPlan(extractRelationProperty, elementExtractor);
                var def = new RelationTableDef(table, extractor, null!);

                tableNameCache_.Add(tableName, syntheticType);
                relationTables.Add(def);
            }

            return relationTables;
        }

        /// <summary>
        ///   Determines the name of the Principal Table for an Entity Type.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which <paramref name="source"/> is being translated.
        /// </param>
        /// <param name="source">
        ///   The Entity Type.
        /// </param>
        /// <returns>
        ///   The name of the Principal Table of <paramref name="source"/>.
        /// </returns>
        /// <exception cref="InvalidNameException">
        ///   if <paramref name="source"/> is annotated with a <see cref="TableAttribute"><c>[Table]</c></see>
        ///   annotation whose value is either <see langword="null"/> or the empty string.
        /// </exception>
        private static TableName GetTableName(Context context, Type source) {
            Debug.Assert(context is not null);
            Debug.Assert(source is not null);

            var annotation = source.GetCustomAttribute<TableAttribute>();
            var excludeNS = source.HasAttribute<ExcludeNamespaceFromNameAttribute>();

            if (annotation is not null && (annotation.Name is null || annotation.Name == "")) {
                throw new InvalidNameException(context, annotation);
            }

            if (annotation is null) {
                return new TableName((excludeNS ? source.Name : source.FullName!) + "Table");
            }
            else if (!excludeNS) {
                return new TableName(annotation.Name);
            }
            else {
                // [ExcludeNamespaceFromName] also strips any outer class identifiers
                var annotatedName = annotation.Name;
                var ns = source.FullName![..^source.Name.Length];
                var name = annotatedName.StartsWith(ns) ? annotatedName[ns.Length..] : annotatedName;

                if (name == "") {
                    throw new InvalidNameException(context, new ExcludeNamespaceFromNameAttribute());
                }
                return new TableName(name);
            }
        }

        /// <summary>
        ///   Determines the name of a Relation Table defined by a property nested within an Entity Type.
        /// </summary>
        /// <param name="tracker">
        ///   The <see cref="RelationTracker"/> contextualizing the translation of the Relation-type property.
        /// </param>
        /// <param name="source">
        ///   The "owning" Entity Type.
        /// </param>
        /// <returns>
        ///   The default name of the Relation Table for the property encapsulated by <paramref name="tracker"/>. Note
        ///   that <see cref="RelationTableAttribute">[RelationTable]</see> annotations are not considered.
        /// </returns>
        private static TableName GetTableName(RelationTracker tracker, Type source) {
            Debug.Assert(tracker is not null);
            Debug.Assert(source is not null);

            // When a [Name] annotation is applied to a property in an Entity, it does not get seen by the
            // RelationTracker. Instead, it gets processed like any other [Name] annotation; however, the effect is
            // encapsulated entirely by the `MultiFieldGroup` base class. So, we have to do a little re-implementation
            // here to find the [Name] annotation applied directly to the property, if any. We only want to do this when
            // dealing with top-level properties, otherwise the annotation will have already been handled. Similarly, we
            // know that the name is valid (i.e. not null and not empty) and that there are no duplicates.
            var ownPart = tracker.Name;
            if (tracker.Name == tracker.Path) {
                foreach (var annotation in tracker.Property.GetCustomAttributes<NameAttribute>()) {
                    if (annotation.Path == "") {
                        ownPart = annotation.Name;
                        break;
                    }
                }
            }

            if (source.HasAttribute<ExcludeNamespaceFromNameAttribute>()) {
                return new TableName($"{source.Name}.{ownPart}Table");
            }
            else {
                return new TableName($"{source.FullName!}.{ownPart}Table");
            }
        }

        /// <summary>
        ///   Identifies all of the custom <c>CHECK</c> constraints applied to a Entity's Principal Table.
        /// </summary>
        /// <exception cref="InvalidCustomConstraintException">
        ///   if <paramref name="source"/> is annotated with at least one <c>[Check.Complex]</c> annotation that has a
        ///   populated <see cref="Check.ComplexAttribute.UserError">user error</see>
        ///     --or--
        ///   if <paramref name="source"/> is annotated with at least one <c>[Check.Complex]</c> annotation for which no
        ///   Fields were provided.
        /// </exception>
        /// <exception cref="UnrecognizedFieldException">
        ///   if <paramref name="source"/> is annotated with at least one <c>[Check.Complex]</c> annotation for which
        ///   at least one of the provided Fields does not exist in <paramref name="schemas"/>.
        /// </exception>
        private static IEnumerable<CheckConstraint>
        GetTableConstraints(Context context, Type source, IEnumerable<FieldSchema> schemas, Settings settings) {
            Debug.Assert(context is not null);
            Debug.Assert(source is not null);
            Debug.Assert(schemas is not null && schemas.Count() >= 2);

            foreach (var annotation in source.GetCustomAttributes<Check.ComplexAttribute>()) {
                if (annotation.UserError is not null) {
                    throw new InvalidCustomConstraintException(context, annotation);
                }
                else if (annotation.FieldNames.IsEmpty()) {
                    throw new InvalidCustomConstraintException(context, new NoFields());
                }

                var members = new List<FieldSchema>();
                foreach (var name in annotation.FieldNames) {
                    var match = schemas.Where(f => f.Field.Name == name);
                    if (match.IsEmpty()) {
                        throw new UnrecognizedFieldException(context, name.ToString());
                    }
                    members.Add(match.First());
                }

                var fields = members.Select(f => f.Field);
                var converters = members.Select(f => f.Descriptor.Converter);
                var clause = annotation.TryMakeClause(fields, converters, settings, context);
                yield return new CheckConstraint(clause);
            }
        }
    }
}
