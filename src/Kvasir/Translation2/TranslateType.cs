using Cybele.Extensions;
using Kvasir.Annotations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Kvasir.Translation2 {
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
        /// <param name="allowRelations">
        ///   <see langword="true"/> if Relations should be allowed as valid (e.g. when translating a real Entity Type);
        ///   <see langword="false"/> if Relations should be disallowed (e.g. when translating a Relation).
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
        /// </exception>
        /// <exception cref="NotEnoughFieldsException">
        ///   if <paramref name="source"/> corresponds to an Aggregate that contributes fewer than 1 Field to the data
        ///   model.
        /// </exception>
        /// <exception cref="NestedRelationException">
        ///   if <paramref name="allowRelations"/> is <see langword="false"/> and <paramref name="source"/> has a
        ///   Relation-type Field that would be included in the data model.
        /// </exception>
        private IEnumerable<FieldGroup> TranslateType(Context context, Type source, bool allowRelations) {
            Debug.Assert(context is not null);
            Debug.Assert(source is not null);
            Debug.Assert(Nullable.GetUnderlyingType(source) is null);

            // Memoization
            if (typeCache_.TryGetValue(source, out IReadOnlyList<FieldGroup>? memoization)) {
                return memoization.Select(g => g.Clone());
            }
            var translation = new List<FieldGroup>();
            var relationTrackers = new List<RelationTracker>();

            void performAssemblyCheck(Type type) {
                if (type.Assembly != callingAssembly_) {
                    throw new InvalidPropertyInDataModelException(context, type, callingAssembly_);
                }
            }

            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
            foreach (var property in source.GetProperties(flags).OrderBy(f => f.Name)) {
                using var propGuard = context.Push(property);
                var propType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                var propName = property.Name.Split('.')[^1];

                var propCategory = property.TranslationCategory();
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
                    var typeCategory = property.PropertyType.TranslationCategory();
                    if (typeCategory.Equals(TypeCategory.Enumeration) || typeCategory.Equals(TypeCategory.Supported)) {
                        translation.Add(new SingleFieldGroup(context, property));
                    }
                    else if (typeCategory.Equals(TypeCategory.Relation)) {
                        if (!allowRelations) {
                            throw new NestedRelationException(context);
                        }
                        relationTrackers.Add(new RelationTracker(property));
                    }
                    else if (typeCategory.Equals(TypeCategory.Class)) {
                        // The `nestedGuard` variable is manually disposed because we want it to be active only for the
                        // duration of the type translation; once we go to create the ReferenceFieldGroup, we want the
                        // Context to be back at the property's scope
                        performAssemblyCheck(propType);
                        var nestedGuard = context.Push(propType);
                        TranslatePrincipalTable(context, propType);
                        nestedGuard.Dispose();
                        var pk = pkCache_[propType].Select(g => g.Reset()).ToList();
                        translation.Add(new ReferenceFieldGroup(context, property, pk));
                    }
                    else if (typeCategory.Equals(TypeCategory.Struct)) {
                        // The `nestedGuard` variable is manually disposed because we want it to be active only for the
                        // duration of the type translation and emptiness check; once we go to create the
                        // AggregateFieldGroup, we want the Context to be back at the property's scope
                        performAssemblyCheck(propType);
                        var nestedGuard = context.Push(propType);
                        var fields = TranslateType(context, propType, allowRelations).Select(g => g.Clone()).ToList();
                        var trackers = relationTrackersCache_[propType].Select(t => t.ExtendPath(propName)).ToList();
                        if (fields.IsEmpty() && trackers.IsEmpty()) {
                            throw new NotEnoughFieldsException(context, 1, 0);
                        }
                        nestedGuard.Dispose();
                        translation.Add(new AggregateFieldGroup(context, property, fields, trackers));
                        relationTrackers.AddRange(trackers);
                    }
                    else {
                        throw new InvalidPropertyInDataModelException(context, propType, typeCategory);
                    }
                }
            }

            AssignColumns(context, translation);
            typeCache_[source] = translation.ToList();
            relationTrackersCache_[source] = relationTrackers;

            // It's important to return a concrete collection here rather than the result of a LINQ query because we do
            // not want to re-evaluate the `Clone()` operation multiple times. Doing so will cause issues with Primary
            // Key extraction due to the use of identity equality.
            return translation.Select(g => g.Clone()).ToList();
        }
    }
}
