using Cybele.Extensions;
using Kvasir.Annotations;
using Kvasir.Extraction;
using Kvasir.Relations;
using Kvasir.Schema;
using Optional;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace Kvasir.Translation {
    /// <summary>
    ///   A <see cref="MultiFieldGroup"/> backed by an Relation property (i.e. one that is an implementation of the
    ///   <see cref="IRelation"/> interface).
    /// </summary>
    internal sealed class RelationFieldGroup : MultiFieldGroup {
        /// <inheritdoc/>
        protected sealed override MultiKind Kind => MultiKind.Relation;

        /// <summary>
        ///   A <c>SOME</c> instance containing the name of the Relation for this group based on any applied
        ///   <see cref="RelationTableAttribute">[RelationTable]</see> annotation, if one exists; otherwise, a
        ///   <c>NONE</c> instance.
        /// </summary>
        public Option<TableName> TableName {
            get {
                return relationTableName_.Map(n => new TableName(n));
            }
        }

        /// <summary>
        ///   Constructs a new <see cref="RelationFieldGroup"/>.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which <paramref name="source"/> was accessed via reflection. This context
        ///   should include that property.
        /// </param>
        /// <param name="source">
        ///   The CLR property backing the new <see cref="RelationFieldGroup"/>.
        /// </param>
        /// <param name="fields">
        ///   The collection of constituent <see cref="FieldGroup">FieldGroups</see> that comprise the new
        ///   <see cref="RelationFieldGroup"/>. The order is irrelevant. The Fields should be in a position to be
        ///   directly modified (i.e. already <see cref="FieldGroup.Clone">cloned</see>).
        /// </param>
        public RelationFieldGroup(Context context, PropertyInfo source, IEnumerable<FieldGroup> fields)
            : base(context, source, fields) {

            Debug.Assert(!fields.IsEmpty());

            // We are relying on the `relationTable_` member being a struct whose default/zero initialization is to put
            // in in the <NONE> state. Annotation processing is done when we construct the base class, at which point
            // `SetRelationTableName` may be called. Therefore, we _don't_ want to re-initialize the value here, since
            // doing so may _overwrite_ the name previously set. We can get away with this only because of the fact that
            // Optionals are structs that get implicitly initialized before the base class constructor runs.
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public sealed override RelationFieldGroup Clone() {
            throw new NotSupportedException($"Logically, a {nameof(RelationFieldGroup)} should never be cloned!");
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public sealed override Option<FieldGroup> Filter(IEnumerable<FieldDescriptor> constituents) {
            throw new NotSupportedException($"Logically, a {nameof(RelationFieldGroup)} should never be filtered!");
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public sealed override RelationFieldGroup Reset() {
            throw new NotSupportedException($"Logically, a {nameof(RelationFieldGroup)} should never be reset!");
        }

        /// <inheritdoc/>
        public sealed override void SetColumn(Context context, int index) {
            throw new InapplicableAnnotationException(context, typeof(ColumnAttribute), CLRType, Kind);
        }

        /// <inheritdoc/>
        public sealed override void SetInCandidateKey(Context context, Nested<UniqueAttribute> annotation) {
            if (annotation.AppliesHere) {
                throw new InapplicableAnnotationException(context, annotation.Annotation, CLRType, Kind);
            }
            else {
                base.SetInCandidateKey(context, annotation);
            }
        }

        /// <inheritdoc/>
        public sealed override void SetInPrimaryKey(Context context, Nested<PrimaryKeyAttribute> annotation, string cascadePath) {
            Debug.Assert(cascadePath == "");

            if (annotation.AppliesHere) {
                throw new InapplicableAnnotationException(context, annotation.Annotation, CLRType, Kind);
            }
            else {
                base.SetInPrimaryKey(context, annotation, cascadePath);
            }
        }

        /// <inheritdoc/>
        public sealed override void SetNamePrefix(Context context, IEnumerable<string> prefix) {
            // When the base `MultiFieldGroup` is translated, it will attempt to prepend the property's name to all the
            // nested Fields. This is desirable for Aggregates and References, but for Relations that name of the source
            // property affects the RelationTable and does *not* appear in the name of the constituent Fields. So, we
            // have to do nothing.
        }

        /// <inheritdoc/>
        public sealed override void SetNullability(Context context, bool nullable) {
            Debug.Assert(context is not null);
            
            // While we ignore *native* nullability of a Relation property (see `SetNativeNullability`), we consider it
            // an error to explicitly request that a Relation property be nullable. Such a request has no logical
            // meaning behind it and would have no effect anyway, but it's not redundant the way that a [NonNullable]
            // annotation is.

            if (nullable) {
                throw new InapplicableAnnotationException(context, typeof(NullableAttribute), CLRType, Kind);
            }
        }

        /// <inheritdoc/>
        protected sealed override void SetRelationTableName(Context context, RelationTableAttribute annotation) {
            Debug.Assert(context is not null);
            Debug.Assert(!relationTableName_.HasValue);

            if (annotation.Name is null || annotation.Name == "") {
                throw new InvalidNameException(context, annotation);
            }
            relationTableName_ = Option.Some(annotation.Name);
        }

        /// <inheritdoc/>
        protected sealed override void ProcessNativeNullability(Context context) {
            // Native nullability is fully IGNORED by Relations. If a Relation is natively non-nullable, there's nothing
            // to do anyway. If a Relation is natively nullable, we treat a Relation with the value `null` as being
            // identical to the last state it was in (which is "empty" if we've never looked at it before).
        }

        /// <inheritdoc/>
        protected sealed override IMultiExtractor CreateExtractor(IEnumerable<FieldGroup> fields) {
            Debug.Assert(fields is not null);
            Debug.Assert(fields.Count() >= 2 && fields.Count() <= 3);

            // The Extractor for a RelationFieldGroup should only extract the _element_ values, not the owning Entity
            // values; this is an optimization to avoid running the reflection over the owning Entity multiple times for
            // a single Relation or across Relations. The owning Entity will always be represented by the first
            // FieldGroup, which is why we skip it.
            return new DecomposingExtractor(fields.OrderBy(g => g.Column.Unwrap()).Skip(1).Select(f => f.Extractor));
        }


        // This is an optional because the "default" name of a RelationTable depends on the full access path of the
        // source property (which we kind of have from the Context constructor argument), and the namespace of the
        // owning Entity (which we don't have) if there is no [ExcludeNamespaceFromName] annotation present (which we
        // don't know).
        private Option<string> relationTableName_;
    }
}
