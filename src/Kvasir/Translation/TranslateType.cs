using Cybele.Extensions;
using Kvasir.Annotations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Kvasir.Translation {
    internal sealed partial class Translator {
        /// <summary>
        ///   Translates a single CLR type.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which <paramref name="source"/> is being translated. This should include
        ///   <paramref name="source"/> already.
        /// </param>
        /// <param name="source">
        ///   The CLR type to translate.
        /// </param>
        /// <param name="traits">
        ///   The <see cref="TranslationTraits"/> describing how to treat Relations, Localizations, and Pre-Defined
        ///   Entities.
        /// </param>
        /// <returns>
        ///   The unordered (but column-assigned) Fields that make up the data model for <paramref name="source"/>, with
        ///   only the annotations available form <paramref name="source"/> applied. The returned groups can be mutated
        ///   without affecting other translations.
        /// </returns>
        /// <exception cref="ConflictingAnnotationsException">
        ///   if any property of <paramref name="source"/> is annotated with both
        ///   <see cref="IncludeInModelAttribute">[IncludeInModel]</see> and
        ///   <see cref="CodeOnlyAttribute">[CodeOnly]</see>.
        /// </exception>
        /// <exception cref="InvalidPropertyInDataModelException">
        ///   if a write-only property of <paramref name="source"/> is annotated with
        ///   <see cref="IncludeInModelAttribute">[IncludeInModel]</see>
        ///     --or--
        ///   if an indexer of <paramref name="source"/> is annotated with
        ///   <see cref="IncludeInModelAttribute">[IncludeInModel]</see>
        ///     --or--
        ///   if a property of <paramref name="source"/> whose type is not supported (e.g. is a delegate, comes from a
        ///   different assembly, etc.) would be included in the data model
        ///     --or--
        ///   if <paramref name="source"/> is a Pre-Defined Entity type and a writeable property of
        ///   <paramref name="source"/> is included in the data model.
        /// </exception>
        /// <exception cref="NotEnoughFieldsException">
        ///   if <paramref name="source"/> corresponds to an Aggregate that contributes fewer than 1 Field to the data
        ///   model.
        /// </exception>
        /// <exception cref="NestedRelationException">
        ///   if the <see cref="TranslationTraits.AllowRelations"/> flag of <paramref name="traits"/> is not set and
        ///   <paramref name="source"/> has a Relation-type Field that would be included in the data model.
        /// </exception>
        /// <exception cref="NestedLocalizationException">
        ///   if the <see cref="TranslationTraits.AllowLocalizations"/> flag of <paramref name="traits"/> is not set and
        ///   <paramref name="source"/> has a Localization-type Field that would be included in the data model.
        /// </exception>
        /// <exception cref="PreDefinedReferenceException">
        ///   if the <see cref="TranslationTraits.RequirePreDefined"/> flag of <paramref name="traits"/> is set and
        ///   <paramref name="source"/> is not a Pre-Defined Entity type.
        /// </exception>
        private IEnumerable<FieldGroup> TranslateType(Context context, Type source, TranslationTraits traits) {
            Debug.Assert(context is not null);
            Debug.Assert(source is not null);
            Debug.Assert(Nullable.GetUnderlyingType(source) is null);

            bool allowRelations = traits.HasFlag(TranslationTraits.AllowRelations);
            bool allowLocalizations = traits.HasFlag(TranslationTraits.AllowLocalizations);
            bool requirePreDefined = traits.HasFlag(TranslationTraits.RequirePreDefined);

            // Memoization
            if (typeCache_.TryGetValue(source, out IReadOnlyList<FieldGroup>? memoization)) {
                var existsRelations = relationTrackersCache_[source].Count > 0;
                var existsLocalizations = localizationTrackersCache_[source].Count > 0;
                var existRegularRefs = memoization.OfType<ReferenceFieldGroup>().Any(g => !IsPreDefined(g.Extractor.SourceType));

                // This may not be the most elegant solution, but it works. If we've seen an Aggregate type before in a
                // context that allows Relations or regular References, and then we see it again in a context that
                // doesn't (namely, when translating a Relation or something involving a Pre-Defined Entity), we have to
                // flag it as an error. We don't have enough information from the initial translation to fully report
                // the error, so we simply let the regular translation happen again. Since we know there's an error,
                // it's guaranteed to fail; and, since we know the type got translated successfully once already, we
                // know there won't be any other errors.
                var illegalNestedRelations = !allowRelations && existsRelations;
                var illegalNestedLocalizations = !allowLocalizations && existsLocalizations;
                var illegalRegularReference = requirePreDefined && existRegularRefs;

                if (!illegalNestedRelations && !illegalNestedLocalizations && !illegalRegularReference) {
                    return memoization.Select(g => g.Clone());
                }
            }
            var isPreDefined = IsPreDefined(source);
            var translation = new List<FieldGroup>();
            var relationTrackers = new List<RelationTracker>();
            var localizationTrackers = new List<LocalizationTracker>();

            void performAssemblyCheck(Type type) {
                if (type.Assembly != callingAssembly_) {
                    throw new InvalidPropertyInDataModelException(context, type, callingAssembly_);
                }
            }

            // We need an isolated Context (i.e. one rooted at the type being translated) for contextualizing
            // Localizations. Because Localizations can be valid in one Context and then invalid in another due to
            // Pre-Defined Entity references and reference cycles, we cannot just reuse the Context of the first
            // encounter.
            var isolatedContext = new Context(source);

            foreach (var property in source.GetProperties(PROPERTY_FLAGS).OrderBy(f => f.Name)) {
                using var propGuard = context.Push(property);
                using var isolatedPropGuard = isolatedContext.Push(property);

                var propType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                var propCategory = property.TranslationCategory();
                var newTraits = traits;

                // For pre-defined Entities, we want to ignore any static properties of the Entity's own type; these
                // are the pre-defined instances, and if we actually process them here we'll get a reference cycle
                // error. Error checking for these properties' annotations is performed elsewhere.
                if (isPreDefined && propType == source) {
                    if (propCategory.Equals(PropertyCategory.WriteOnly) || property.GetMethod!.IsStatic) {
                        continue;
                    }
                    newTraits |= TranslationTraits.RequirePreDefined;
                }

                if (propCategory.Equals(PropertyCategory.Ambiguous)) {
                    throw new ConflictingAnnotationsException(context, typeof(IncludeInModelAttribute), typeof(CodeOnlyAttribute));
                }
                else if (propCategory.Equals(PropertyCategory.WriteOnly)) {
                    throw new InvalidPropertyInDataModelException(context, propCategory);
                }
                else if (propCategory.Equals(PropertyCategory.Indexer)) {
                    throw new InvalidPropertyInDataModelException(context, propCategory);
                }
                else if (propCategory.Equals(PropertyCategory.InDataModel)) {
                    // Pre-Defined Entities cannot have writeable properties, since their data is supposed to be
                    // hard-coded into the source. Data isn't loaded from the database, so any dynamic changes to such
                    // Fields, even if properly saved into the database, wouldn't be loaded later.
                    if (isPreDefined && property.CanWrite && !property.IsInitOnly()) {
                        throw new InvalidPropertyInDataModelException(context, new PreDefinedTag());
                    }

                    var typeCategory = propType.TranslationCategory();
                    if (typeCategory.Equals(TypeCategory.Enumeration) || typeCategory.Equals(TypeCategory.Supported)) {
                        translation.Add(new SingleFieldGroup(context, property));
                    }
                    else if (typeCategory.Equals(TypeCategory.Relation)) {
                        if (!allowRelations) {
                            throw new NestedRelationException(context);
                        }
                        else if (property.CanWrite && !property.IsInitOnly()) {
                            throw new WriteableRelationException(context);
                        }
                        relationTrackers.Add(new RelationTracker(property));
                    }
                    else if (typeCategory.Equals(TypeCategory.Localization)) {
                        if (!allowLocalizations) {
                            throw new NestedLocalizationException(context);
                        }
                        else if (property.CanWrite && !property.IsInitOnly()) {
                            throw new WriteableLocalizationException(context);
                        }
                        translation.Add(new LocalizationKeyFieldGroup(context, property));              // use FULL context for error reporting
                        using var nestedGuard = isolatedContext.Push(property.PropertyType);
                        localizationTrackers.Add(new LocalizationTracker(property, isolatedContext));   // use ISOLATED context for concatenation
                    }
                    else if (typeCategory.Equals(TypeCategory.Class)) {
                        performAssemblyCheck(propType);
                        if (requirePreDefined && !IsPreDefined(propType)) {
                            // If Relations are allowed, that means we're translating a Reference-type Field. If they're
                            // disallowed, that means we're translating a Relation. When disallowed, the non-allowance
                            // permeates the entire translation stack.
                            throw new PreDefinedReferenceException(context, propType, allowRelations);
                        }

                        // The `nestedGuard` variable is manually disposed because we want it to be active only for the
                        // duration of the type translation; once we go to create the ReferenceFieldGroup, we want the
                        // Context to be back at the property's scope
                        var nestedGuard = context.Push(propType);
                        TranslatePrincipalTable(context, propType);
                        nestedGuard.Dispose();
                        var pk = pkCache_[propType].Select(g => g.Reset()).ToList();
                        translation.Add(new ReferenceFieldGroup(context, property, pk, keyMatchers_[propType]));
                    }
                    else if (typeCategory.Equals(TypeCategory.Struct)) {
                        // The `nestedGuard` variable is manually disposed because we want it to be active only for the
                        // duration of the type translation and emptiness check; once we go to create the
                        // AggregateFieldGroup, we want the Context to be back at the property's scope
                        performAssemblyCheck(propType);
                        var nestedGuard = context.Push(propType);

                        var fields = TranslateType(context, propType, newTraits).Select(g => g.Clone()).ToList();
                        var trackersForRelations = relationTrackersCache_[propType].Select(t => t.Extend(property)).ToList();
                        var trackersForLocalizations = localizationTrackersCache_[propType].Select(t => new LocalizationTracker(t.Source, isolatedContext.Concat(t.Context))).ToList();
                        if (fields.IsEmpty() && trackersForRelations.IsEmpty()) {
                            throw new NotEnoughFieldsException(context, 1, 0);
                        }
                        nestedGuard.Dispose();
                        var aggregate = new AggregateFieldGroup(context, property, fields, trackersForRelations);
                        relationTrackers.AddRange(trackersForRelations);
                        localizationTrackers.AddRange(trackersForLocalizations);

                        // If an Aggregate contains only Relation-type Fields, we still need to do a Translation of it
                        // so that we can process any annotations on the original Aggregate property. However, we don't
                        // want to actually add such a FieldGroup to the Translation, since it doesn't contribute
                        // anything to the data model.
                        if (aggregate.Size > 0) {
                            translation.Add(aggregate);
                        }
                    }
                    else {
                        throw new InvalidPropertyInDataModelException(context, propType, typeCategory);
                    }
                }
            }

            AssignColumns(context, translation);
            typeCache_[source] = translation.ToList();
            relationTrackersCache_[source] = relationTrackers;
            localizationTrackersCache_[source] = localizationTrackers;

            // It's important to return a concrete collection here rather than the result of a LINQ query because we do
            // not want to re-evaluate the `Clone()` operation multiple times. Doing so will cause issues with Primary
            // Key extraction due to the use of identity equality.
            return translation.Select(g => g.Clone()).ToList();
        }


        /// <summary>
        ///   Traits controlling the validity of different categories of properties when translating a Type. Essentially
        ///   a collection of Boolean flags.
        /// </summary>
        [Flags] private enum TranslationTraits {
            None = 0,
            AllowRelations = 1 << 0,
            AllowLocalizations = 1 << 1,
            RequirePreDefined = 1 << 2,
        }


        private static readonly BindingFlags PROPERTY_FLAGS =
            BindingFlags.Public |
            BindingFlags.NonPublic |
            BindingFlags.Instance |
            BindingFlags.Static;
    }
}
