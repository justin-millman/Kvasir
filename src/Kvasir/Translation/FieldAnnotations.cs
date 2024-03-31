using Cybele.Extensions;
using Kvasir.Annotations;
using Kvasir.Core;
using Kvasir.Schema;
using Optional;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

// This code takes a "base translation" - which is intentionally agnostic of the source property's category - and
// applies annotations. In some cases, it is necessary to reverse engineer the initial category; for example, a
// [DataConverter] annotation can only be applied to a Scalar or an Enumeration property. But for the most part, the
// annotations result in the descriptor being updated in some manner. If there are no annotations, then the "base
// translation" is 100% accurate.
//
// The code for producing the original "base translations" can be found in the BaseTranslations.cs file. The code for
// applying constraint annotations can be found in the FieldConstraints.cs file.

namespace Kvasir.Translation {
    internal sealed partial class Translator {
        private TranslationState ApplyAnnotations(PropertyInfo property, TranslationState baseTranslation) {
            Debug.Assert(property is not null);
            Debug.Assert(!baseTranslation.Fields.IsEmpty() || !baseTranslation.Relations.IsEmpty());

            var resultFields = new Dictionary<string, FieldDescriptor>(baseTranslation.Fields);
            var resultRelations = new Dictionary<string, IRelationDescriptor>(baseTranslation.Relations);
            var result = new MutableTranslationState(Fields: resultFields, Relations: resultRelations);

            ProcessNames(property, result);
            ProcessRelationTables(property, result);
            ProcessNullability(property, result);
            ProcessColumn(property, result);
            ProcessConverters(property, result);
            ProcessDefaults(property, result);              // must come after Nullability and Data Converters
            ProcessPrimaryKeyOptIns(property, result);      // must come after Nullability
            ProcessCandidateKeys(property, result);
            ProcessConstraints(property, result);           // must come after Data Converters

            return new TranslationState(resultFields, resultRelations);
        }

        private static void ProcessNames(PropertyInfo property, MutableTranslationState state) {
            Debug.Assert(property is not null);
            HashSet<string> processed = new HashSet<string>();

            foreach (var annotation in property.GetCustomAttributes<NameAttribute>()) {
                var context = new PropertyTranslationContext(property, annotation.Path);

                // It is an error for a [Name] attribute to have a null Path
                if (annotation.Path is null) {
                    throw Error.InvalidPath(context, annotation);
                }

                // It is an error for the value specified by a [Name] attribute to be invalid; currently, the only
                // concrete restriction is that the value is neither null nor the empty string
                if (annotation.Name is null || annotation.Name == "") {
                    throw Error.InvalidName(context, annotation, annotation.Name);
                }

                // It is an error for multiple [Name] attributes on a single property to apply to the same Path, unless
                // the name being applied is the same (making the second annotation redundant). It is, however, legal
                // for a [Name] attribute at an outer scope to override that of one applied at an inner scope
                if (!processed.Add(annotation.Path)) {
                    if (!state.Fields[annotation.Path].Name.SequenceEqual(Enumerable.Repeat(annotation.Name, 1))) {
                        throw Error.DuplicateAnnotation(context, annotation);
                    }
                    continue;
                }

                // If the Path on a [Name] attribute refers to a concrete Field (it would be a scalar or an enumeration)
                // then the Field's name in total is replaced by the annotation value
                if (state.Fields.TryGetValue(annotation.Path, out FieldDescriptor target)) {
                    state.Fields[annotation.Path] = target with { Name = new List<string>() { annotation.Name } };
                    continue;
                }

                // If the Path on a [Name] attribute refers to a concrete Relation Field, then the Relation's name in
                // total is replaced by the annotation value
                if (state.Relations.TryGetValue(annotation.Path, out IRelationDescriptor? relation)) {
                    state.Relations[annotation.Path] = relation.WithName(Enumerable.Repeat(annotation.Name, 1));
                    continue;
                }

                // If the Path on a [Name] attribute refers to a Field potentially nested within a Relation, then the
                // attribute is attached to the top-level parent Field in that Relation
                if (TryAttachAnnotation(state, annotation)) {
                    continue;
                }

                // It is an error for a [Name] attribute to have a non-existent Path
                var matches = GetMatches(state.Fields, annotation.Path).ToList();
                if (matches.IsEmpty()) {
                    throw Error.InvalidPath(context, annotation);
                }

                // If there is no scalar/enumeration Field at the given Path, we assume it will resolve to a compound
                // property (such as an Aggregate). We identify all Fields sourced from that compound by looking for
                // those whose path is prefixed with the provided Path. Any Field whose name has fewer "parts" than the
                // Path had a blanket renaming (as above) applied up-scope, so nothing is done. Otherwise, only the
                // "part" matched by the prefix gets replaced.
                foreach ((var path, var descriptor) in matches) {
                    if (annotation.Path == "") {
                        // This check guards against overwriting the renaming of a Field that has already been renamed
                        // through a previous full-path-supplied annotation
                        if (descriptor.Name[0] == property.Name) {
                            state.Fields[path] = state.Fields[path] with {
                                Name = new List<string>(descriptor.Name) { [0] = annotation.Name }
                            };
                        }
                    }
                    else {
                        // Note: The -1 here is because the Name already has the property appended (as the default
                        //   naming convention for an Aggregate), but we want to discount that when considering if a
                        //   name change is applicable
                        var pathParts = annotation.Path.Split(PATH_SEPARATOR);
                        if (descriptor.Name.Count - 1 > pathParts.Length) {
                            state.Fields[path] = state.Fields[path] with {
                                Name = new List<string>(descriptor.Name) { [pathParts.Length] = annotation.Name }
                            };
                        }
                    }
                }
            }
        }

        private static void ProcessRelationTables(PropertyInfo property, MutableTranslationState state) {
            Debug.Assert(property is not null);

            var annotation = property.GetCustomAttribute<RelationTableAttribute>();
            var context = new PropertyTranslationContext(property, "");

            if (annotation is not null) {
                // It is an error for [RelationTable] to be applied to anything other than Relation
                if (!state.Fields.IsEmpty()) {
                    throw Error.UserError(context, annotation, "property is not a Relation");
                }

                // It is an error for the value specified by a [RelationTable] attribute to be invalid; currently, th
                // only concrete restriction is that the value is neither null nor the empty string
                if (annotation.Name is null || annotation.Name == "") {
                    throw Error.InvalidName(context, annotation, annotation.Name);
                }

                // No errors encountered
                Debug.Assert(state.Relations.Count == 1);
                var relationPath = state.Relations.Keys.First();
                state.Relations[relationPath] = state.Relations[relationPath].WithTableName(annotation.Name);
            }
        }

        private static void ProcessNullability(PropertyInfo property, MutableTranslationState state) {
            Debug.Assert(property is not null);

            var nativeNullability = new NullabilityInfoContext().Create(property).ReadState;
            var nullable = property.HasAttribute<NullableAttribute>();
            var nonNullable = property.HasAttribute<NonNullableAttribute>();

            // It is an error for a property to be annotated as both [Nullable] and [NonNullable]
            if (nullable && nonNullable) {
                var context = new PropertyTranslationContext(property, "");
                throw Error.MutuallyExclusive(context, new NullableAttribute(), new NonNullableAttribute());
            }

            // Native nullability of Relation-type Fields is ignored; annotated [Nullable] is an error
            if (state.Fields.IsEmpty()) {
                if (nullable) {
                    var context = new PropertyTranslationContext(property, "");
                    throw Error.InapplicableConstraint(context, new NullableAttribute(), "Relations have no nullability");
                }
                return;
            }

            // If the property is annotated as being nullable, or if there are no annotations and the property's type
            // is natively nullable, then nullability is imparted. This is done by making all of the Fields nullable;
            // for scalars and aggregates, there will be only one. Because the default for scalars and and aggregates is
            // non-nullable, they will never fail the ambiguity check.
            if (nullable || (!nonNullable && nativeNullability == NullabilityState.Nullable)) {
                var noAmbiguity = false;
                foreach ((var path, var descriptor) in state.Fields) {
                    noAmbiguity |= descriptor.Nullability == IsNullable.No;
                    state.Fields[path] = descriptor with { Nullability = IsNullable.Yes };
                }

                // It is an error for a compound property, such as an Aggregate, to be nullable if all its constituent
                // nested Fields are already nullable; this creates an ambiguity as to the meaning of all-null
                if (!noAmbiguity) {
                    var context = new PropertyTranslationContext(property, "");
                    throw Error.AmbiguousNullability(context);
                }
            }
        }

        private static void ProcessColumn(PropertyInfo property, MutableTranslationState state) {
            Debug.Assert(property is not null);
            var annotation = property.GetCustomAttribute<ColumnAttribute>();

            if (annotation is not null) {
                // It is an error for a [Column] annotation to be applied to a Relation-type Field
                if (state.Fields.IsEmpty()) {
                    var context = new PropertyTranslationContext(property, "");
                    throw Error.InapplicableConstraint(context, annotation, "Relations cannot be ordered");
                }

                // It is an error for the index of a [Column] annotation to be negative
                if (annotation.Column < 0) {
                    var context = new PropertyTranslationContext(property, "");
                    var msg = $"column index {annotation.Column} is negative";
                    throw Error.UserError(context, annotation, msg);
                }

                foreach ((var path, var descriptor) in state.Fields) {
                    state.Fields[path] = descriptor with {
                        AbsoluteColumn = Option.Some(descriptor.RelativeColumn + annotation.Column)
                    };
                }
            }
        }

        private static void ProcessConverters(PropertyInfo property, MutableTranslationState state) {
            Debug.Assert(property is not null);

            var dpAnnotation = property.GetCustomAttribute<DataConverterAttribute>();
            var numeric = property.HasAttribute<NumericAttribute>();
            var asString = property.HasAttribute<AsStringAttribute>();

            var context = new PropertyTranslationContext(property, "");
            var expectedType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
            var isScalarOrEnumeration = state.Fields.ContainsKey("");
            Debug.Assert(!isScalarOrEnumeration || state.Fields.Count == 1);

            if (dpAnnotation is not null) {
                // It is an error for a non-Scalar/non-Enumeration property to be annotated with [DataProvider]
                if (!isScalarOrEnumeration) {
                    var msg = $"{expectedType.Name} is neither a scalar nor an enumeration";
                    throw Error.UserError(context, dpAnnotation, msg);
                }

                // It is an error for a property to be annotated with both [DataProvider] and [Numeric]
                if (numeric) {
                    throw Error.MutuallyExclusive(context, dpAnnotation, new NumericAttribute());
                }

                // It is an error for a property to be annotated with both [DataProvider] and [AsString]
                if (asString) {
                    throw Error.MutuallyExclusive(context, dpAnnotation, new AsStringAttribute());
                }

                // It is an error for a [DataConverter] annotation to have a populated <UserError>
                if (dpAnnotation.UserError is not null) {
                    throw Error.UserError(context, dpAnnotation, dpAnnotation.UserError);
                }

                var converter = dpAnnotation.DataConverter;
                Debug.Assert(converter.IsBidirectional);

                // It is an error for the <SourceType> of a [DataConverter] annotation to be different than the
                // annotated property's CLR type, modulo nullability on primitives and structs
                if (!expectedType.IsInstanceOf(converter.SourceType)) {
                    var expected = $"converter with source type {property.PropertyType.Name}";
                    var actual = $"converter with source type {converter.SourceType.Name}";
                    throw Error.UserError(context, dpAnnotation, $"expected {expected}, but got {actual}");
                }

                // It is an error for the <ResultType> of a [DataConverter] annotation to be unsupported
                if (!DBType.IsSupported(converter.ResultType)) {
                    var msg = $"converter result type {converter.ResultType.Name} is not supported";
                    throw Error.UserError(context, dpAnnotation, msg);
                }

                // No errors encountered
                state.Fields[""] = state.Fields[""] with { Converter = converter };

                // If the original type is not an enumeration but the converted type is, we need to set the restricted
                // image that would ordinarily be set by the Enumeration BaseTranslation. We assume that all the result
                // enumerators are possible. However, if we are converting between two different enumeration types (or
                // between the same enumeration types), we update the range just like we would if the source weren't an
                // enumeration at all.
                if (dpAnnotation.DataConverter.ResultType.IsEnum && !dpAnnotation.DataConverter.SourceType.IsEnum) {
                    state.Fields[""] = state.Fields[""] with {
                        Constraints = state.Fields[""].Constraints with {
                            RestrictedImage = dpAnnotation.DataConverter.ResultType.ValidValues().Cast<object>().ToHashSet()
                        }
                    };
                }
            }
            else if (numeric) {
                // It is an error for a property whose type is not an enumeration to be annotated with [Numeric]
                if (!expectedType.IsEnum) {
                    var msg = $"expected Field of enumeration type, but got {expectedType.Name}";
                    throw Error.UserError(context, new NumericAttribute(), msg);
                }

                // It is an error for a property to be annotated with both [Numeric] and [AsString]
                if (asString) {
                    throw Error.MutuallyExclusive(context, new NumericAttribute(), new AsStringAttribute());
                }

                state.Fields[""] = state.Fields[""] with { Converter = new EnumToNumericConverter(expectedType).ConverterImpl };
            }
            else if (asString) {
                // It is an error for a property whose type is not an enumeration to be annotated with [AsString]
                if (!expectedType.IsEnum) {
                    var msg = $"expected Field of enumeration type, but got {expectedType.Name}";
                    throw Error.UserError(context, new AsStringAttribute(), msg);
                }

                state.Fields[""] = state.Fields[""] with { Converter = new EnumToStringConverter(expectedType).ConverterImpl };
            }

            // Fields whose pre-conversion CLR type is an enumeration have an implicitly restricted domain, which
            // therefore means that they have an implicitly restricted image. The valid enumerators of that type are fed
            // through the Field's converter (which may be the identity converter) to produce the final set of viable
            // values. These values may be augmented or filtered by [Check.IsOneOf] and [Check.IsNotOneOf] constraints
            // later. If the converter's result type is not itself an enumeration, or if the back-end database provider
            // does not actually support enumerations, the restricted image will be realized as a CHECK constraint.
            if (isScalarOrEnumeration && state.Fields[""].Converter.SourceType.IsEnum) {
                var updated = new HashSet<object>();
                foreach (var enumerator in state.Fields[""].Constraints.RestrictedImage) {
                    try {
                        updated.Add(state.Fields[""].Converter.Convert(enumerator)!);
                    }
                    catch (Exception ex) {
                        Debug.Assert(dpAnnotation is not null);
                        var msg = $"error converting {enumerator.ForDisplay()}: {ex.Message}";
                        throw Error.UserError(context, dpAnnotation, msg);
                    }
                }
                state.Fields[""] = state.Fields[""] with {
                    Constraints = state.Fields[""].Constraints with { RestrictedImage = updated }
                };
            }
        }

        private static void ProcessDefaults(PropertyInfo property, MutableTranslationState state) {
            Debug.Assert(property is not null);
            HashSet<string> processed = new HashSet<string>();

            foreach (var annotation in property.GetCustomAttributes<DefaultAttribute>()) {
                var context = new PropertyTranslationContext(property, annotation.Path);

                // It is an error for a [Default] attribute to have a null Path
                if (annotation.Path is null) {
                    throw Error.InvalidPath(context, annotation);
                }

                // If the Path on a [Default] attribute refers to a Field potentially nested within a Relation, then the
                // attribute is attached to the top-level parent Field in that Relation
                if (TryAttachAnnotation(state, annotation)) {
                    continue;
                }

                // It is an error for a [Default] attribute to have a non-existent Path
                if (!state.Fields.TryGetValue(annotation.Path, out FieldDescriptor target)) {
                    throw Error.InvalidPath(context, annotation);
                }

                // It is an error for the value of a [Default] attribute to be incompatible with the expected data type,
                // including nullability
                var value = annotation.Value.ParseFor(target.Converter.ResultType, target.Nullability == IsNullable.Yes);
                value.MatchNone(reason => throw Error.BadValue(context, annotation, annotation.Value, reason));

                // It is an error for multiple [Default] attributes on a single property to apply to the same Path,
                // though it is legal if a [Default] attribute on an outer aggregate to override that of an inner
                // aggregate
                if (!processed.Add(annotation.Path)) {
                    throw Error.DuplicateAnnotation(context, annotation);
                }

                // No errors encountered
                state.Fields[annotation.Path] = target with { Default = value.WithoutException() };
            }
        }

        private static void ProcessPrimaryKeyOptIns(PropertyInfo property, MutableTranslationState state) {
            Debug.Assert(property is not null);

            void MarkInPrimaryKey(string path) {
                var target = state.Fields[path];
                var annotation = new PrimaryKeyAttribute();
                var context = new PropertyTranslationContext(property, path);

                // It is an error for a [PrimaryKey] attribute to be applied to a nullable Field
                if (target.Nullability == IsNullable.Yes) {
                    var msg = "a nullable Field cannot be part of an Entity's primary key";
                    throw Error.UserError(context, annotation, msg);
                }

                // It is an error for a [PrimaryKey] attribute to be placed directly on a nested Field
                if (!property.ReflectedType!.IsClass) {
                    var msg = "a nested Field cannot be directly annotated";
                    throw Error.UserError(context, annotation, msg);
                }

                // No errors encountered
                state.Fields[path] = state.Fields[path] with { InPrimaryKey = true };
            }

            foreach (var annotation in property.GetCustomAttributes<PrimaryKeyAttribute>()) {
                var context = new PropertyTranslationContext(property, annotation.Path);

                // It is an error for a [PrimaryKey] attribute to have a null Path
                if (annotation.Path is null) {
                    throw Error.InvalidPath(context, annotation);
                }

                // If the Path on a [PrimaryKey] attribute refers to a Field potentially nested within a Relation, then
                // the attribute is attached to the top-level parent Field in that Relation
                if (TryAttachAnnotation(state, annotation)) {
                    continue;
                }

                // If the Path on a [PrimaryKey] attribute refers to a concrete Field (it would be a scalar or an
                // enumeration) then the Field is simply placed into the Primary Key
                if (state.Fields.TryGetValue(annotation.Path, out FieldDescriptor target)) {
                    MarkInPrimaryKey(annotation.Path);
                }
                else {
                    // It is an error for a [PrimaryKey] attribute to have a non-existent Path
                    var matches = GetMatches(state.Fields, annotation.Path).ToList();
                    if (matches.IsEmpty()) {
                        throw Error.InvalidPath(context, annotation);
                    }

                    // If the Path on a [PrimaryKey] attribute refers to a grouping of Fields (e.g. an aggregate), then
                    // all of the nested Fields are placed into the Primary Key
                    foreach ((var path, var _) in matches) {
                        MarkInPrimaryKey(path);
                    }
                }
            }
        }

        private static void ProcessCandidateKeys(PropertyInfo property, MutableTranslationState state) {
            Debug.Assert(property is not null);

            void PlaceInCandidateKey(string path, string keyName) {
                state.Fields[path] = state.Fields[path] with {
                    CandidateKeyMemberships = new HashSet<string>(state.Fields[path].CandidateKeyMemberships) { keyName }
                };
            }

            foreach (var annotation in property.GetCustomAttributes<UniqueAttribute>()) {
                var context = new PropertyTranslationContext(property, annotation.Path);

                // It is an error for a [Unique] attribute to have a null Path
                if (annotation.Path is null) {
                    throw Error.InvalidPath(context, annotation);
                }

                // It is an error for the value specified by a [Unique] attribute to be invalid; currently, the only
                // concrete restriction is that the value is neither null nor the empty string and that a non-anonymous
                // annotation's name does not begin with the reserved prefix
                if (annotation.Name is null || annotation.Name == "") {
                    throw Error.InvalidName(context, annotation, annotation.Name);
                }
                else if (!annotation.IsAnonymous && annotation.Name.StartsWith(UniqueAttribute.ANONYMOUS_PREFIX)) {
                    var msg = $"begins with reserved character sequence \"{UniqueAttribute.ANONYMOUS_PREFIX}\"";
                    throw Error.InvalidName(context, annotation, annotation.Name, msg);
                }

                // If the Path on a [Unique] attribute refers to a Field potentially nested within a Relation, then the
                // attribute is attached to the top-level parent Field in that Relation
                if (TryAttachAnnotation(state, annotation)) {
                    continue;
                }

                // If the Path on a [Unique] attribute refers to a concrete Field (it would be a scalar or an
                // enumeration) then the Field is simply placed into the appropriate Candidate Key
                if (state.Fields.TryGetValue(annotation.Path, out FieldDescriptor target)) {
                    PlaceInCandidateKey(annotation.Path, annotation.Name);
                }
                else {
                    // It is an error for a [Unique] attribute to have a non-existent Path
                    var matches = GetMatches(state.Fields, annotation.Path).ToList();
                    if (matches.IsEmpty()) {
                        throw Error.InvalidPath(context, annotation);
                    }

                    // If the Path on a [Unique] attribute refers to a grouping of Fields (e.g. an aggregate), then all
                    // of the nested Fields are placed into the appropriate Candidate Key
                    foreach ((var path, var _) in matches) {
                        PlaceInCandidateKey(path, annotation.Name);
                    }
                }
            }
        }

        private static IEnumerable<(string, FieldDescriptor)> GetMatches(FieldsListing state, string targetPath) {
            foreach ((var path, var descriptor) in state) {
                if (targetPath == "" || path.StartsWith(targetPath + NAME_SEPARATOR)) {
                    yield return (path, descriptor);
                }
            }
        }

        private static bool TryAttachAnnotation(MutableTranslationState state, INestableAnnotation annotation) {
            // There are fundamentally two cases we have to consider: no initial access path (for when an attribute is
            // applied directly onto a Relation-type property referring to a nested Field) and yes initial access path
            // (for when an attribute is applied to an Aggregate referring to a Relation-nested Field). In the former
            // situation, we will have only a single possible Relation that should automatically be a candidate. In the
            // latter, we will have one or more possible Relations that are candidates only if their access path _plus_
            // an additional separator character is a prefix of the actual target path. In either case, the annotation's
            // Path may itself refer to further-nested Fields, so the actual Relation-nested Field is the first segment
            // after the appropriate prefix has been removed.
            foreach ((var accessPath, var relationDescriptor) in state.Relations) {
                if (accessPath == "") {
                    var nestedField = annotation.Path.Split(PATH_SEPARATOR)[0];
                    if (relationDescriptor.FieldTypes.ContainsKey(nestedField)) {
                        state.Relations[accessPath] = relationDescriptor.WithAnnotation(nestedField, annotation);
                        return true;
                    }
                    return false;
                }
                else if (annotation.Path.StartsWith(accessPath + ".")) {
                    var nestedField = annotation.Path[(accessPath.Length + 1)..].Split(PATH_SEPARATOR)[0];
                    if (relationDescriptor.FieldTypes.ContainsKey(nestedField)) {
                        state.Relations[accessPath] = relationDescriptor.WithAnnotation(nestedField, annotation);
                        return true;
                    }
                    return false;
                }
            }
            return false;
        }
    }
}
