using Cybele.Extensions;
using Kvasir.Annotations;
using Kvasir.Extraction;
using Kvasir.Reconstitution;
using Optional;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace Kvasir.Translation {
    /// <summary>
    ///   The base class for the data state that manages the translation of one or more Fields in a back-end database,
    ///   each derived from the same CLR property and those thereunder nested.
    /// </summary>
    internal abstract class FieldGroup : IEnumerable<FieldDescriptor> {
        /// <summary>
        ///   The number of Fields in the <see cref="FieldGroup"/>.
        /// </summary>
        public abstract int Size { get; }

        /// <summary>
        ///   <see langword="true"/> if the property that backs the <see cref="FieldGroup"/> is nullable (accounting for
        ///   any annotations); otherwise, <see langword="false"/>.
        /// </summary>
        public abstract bool IsNativelyNullable { get; }

        /// <summary>
        ///   <see langword="true"/> if all the Fields in the <see cref="FieldGroup"/> are nullable, otherwise
        ///   <see langword="false"/>.
        /// </summary>
        public abstract bool AllNullable { get; }

        /// <summary>
        ///   The starting column index of the <see cref="FieldGroup"/>.
        /// </summary>
        public Option<int> Column { get; private set; }

        /// <summary>
        ///   The property that backs the <see cref="FieldGroup"/>
        /// </summary>
        public PropertyInfo Source { get; private set; }

        /// <summary>
        ///   The (unnormalized) name of the constructor argument that would be a match for the <see cref="FieldGroup"/>
        ///   when determining viability for Reconstitution.
        /// </summary>
        public abstract string ReconstitutionArgumentName { get; }

        /// <summary>
        ///   The dot-separated access path of the single Field within the group at a given column index.
        /// </summary>
        /// <param name="column">
        ///   The column index, which must be non-negative and strictly less than the <see cref="Size"/> of the group.
        /// </param>
        public abstract string this[int column] { get; }

        /// <summary>
        ///   The <see cref="IMultiExtractor"/> for this group.
        /// </summary>
        public abstract IMultiExtractor Extractor { get; }

        /// <summary>
        ///   The <see cref="ICreator"/> for this group. If the group represents a <c>[Calculated]</c> property, this
        ///   value will be a <c>NONE</c> instance.
        /// </summary>
        public abstract Option<ICreator> Creator { get; protected init; }

        /// <summary>
        ///   Clones this <see cref="FieldGroup"/>.
        /// </summary>
        /// <returns>
        ///   A deep copy of this <see cref="FieldGroup"/> as a brand new instance. Modifications to the returned value
        ///   will not affect the source instance, and vice-versa.
        /// </returns>
        public abstract FieldGroup Clone();

        /// <summary>
        ///   Filters this <see cref="FieldGroup"/> to include only a subset of Fields.
        /// </summary>
        /// <param name="constituents">
        ///   The Fields to keep during the filter operation.
        /// </param>
        /// <returns>
        ///   A <c>SOME</c> instance containing a deep copy of this <see cref="FieldGroup"/> where Fields not in
        ///   <paramref name="constituents"/> have been discarded, if any such Fields exist; otherwise, a <c>NONE</c>
        ///   instance.
        /// </returns>
        public abstract Option<FieldGroup> Filter(IEnumerable<FieldDescriptor> constituents);

        /// <summary>
        ///   Ranged-based for loop enumerator accessor.
        /// </summary>
        /// <returns>
        ///   An enumerator that iterates over the individual <see cref="FieldDescriptor">Fields</see> in this
        ///   <see cref="FieldGroup"/> in column order.
        /// </returns>
        public abstract IEnumerator<FieldDescriptor> GetEnumerator();

        /// <see cref="GetEnumerator"/>
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        /// <summary>
        ///   Access the <see cref="ReferenceFieldGroup"/> that are constituent to this <see cref="FieldGroup"/>, which
        ///   are the groups that comprise all the Foreign Keys.
        /// </summary>
        /// <returns>
        ///   A flattened list of the <see cref="ReferenceFieldGroup"/> that make up this <see cref="FieldGroup"/>, in
        ///   no particular order. If a <see cref="ReferenceFieldGroup"/> contains another such group (i.e. the Primary
        ///   Key of one Entity is itself a (full or partial) Reference, only the outermost group will be returned.
        /// </returns>
        public abstract IEnumerable<ReferenceFieldGroup> References();

        /// <summary>
        ///   Resets this <see cref="FieldGroup"/>.
        /// </summary>
        /// <returns>
        ///   A deep copy of this <see cref="FieldGroup"/> as a brand new instance, with all constituent Fields reset
        ///   themselves. Modifications to the returned value will not affect the source instance, and vice-versa.
        /// </returns>
        public abstract FieldGroup Reset();

        /// <summary>
        ///   Constructs a new <see cref="FieldGroup"/>.
        /// </summary>
        /// <param name="source">
        ///   The CLR property from which the new <see cref="FieldGroup"/> is derived.
        /// </param>
        protected FieldGroup(PropertyInfo source) {
            Debug.Assert(source is not null);

            Source = source;
            Column = Option.None<int>();
        }

        /// <summary>
        ///   Constructs a new <see cref="FieldGroup"/> that is largely identical to another.
        /// </summary>
        /// <param name="source">
        ///   The source <see cref="FieldGroup"/>.
        /// </param>
        /// <seealso cref="Clone"/>
        public FieldGroup(FieldGroup source) {
            Debug.Assert(source is not null);

            Source = source.Source;
            Column = source.Column;
        }

        /// <seealso cref="FieldDescriptor.ApplyConstraint(Context, Check.ComparisonAttribute)"/>
        public abstract void ApplyConstraint(Context context, Nested<CheckAttribute> annotation);

        /// <seealso cref="FieldDescriptor.ApplyConstraint(Context, Check.ComparisonAttribute)"/>
        public abstract void ApplyConstraint(Context context, Nested<Check.ComparisonAttribute> annotation);

        /// <seealso cref="FieldDescriptor.ApplyConstraint(Context, Check.InclusionAttribute)"/>
        public abstract void ApplyConstraint(Context context, Nested<Check.InclusionAttribute> annotation);

        /// <seealso cref="FieldDescriptor.ApplyConstraint(Context, Check.SignednessAttribute)"/>
        public abstract void ApplyConstraint(Context context, Nested<Check.SignednessAttribute> annotation);

        /// <seealso cref="FieldDescriptor.ApplyConstraint(Context, Check.StringLengthAttribute)"/>
        public abstract void ApplyConstraint(Context context, Nested<Check.StringLengthAttribute> annotation);

        /// <summary>
        ///   Sets the column index for the group.
        /// </summary>
        /// <remarks>
        ///   The Fields that make up a FieldGroup are guaranteed to be contiguous. The column index of a FieldGroup is
        ///   therefore the index of the first of its Fields.
        /// </remarks>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the column for the group was determined, either by a
        ///   <see cref="ColumnAttribute">[Column]</see> annotation or an assignment process.
        /// </param>
        /// <param name="index">
        ///   The column index.
        /// </param>
        /// <exception cref="InvalidColumnIndexException">
        ///   if <paramref name="index"/> is negative.
        /// </exception>
        public virtual void SetColumn(Context context, int index) {
            Debug.Assert(context is not null);
            Debug.Assert(!Column.HasValue);

            if (index < 0) {
                throw new InvalidColumnIndexException(context, index);
            }
            Column = Option.Some(index);
        }

        /// <seealso cref="FieldDescriptor.SetDefault(Context, DefaultAttribute)"/>
        public abstract void SetDefault(Context context, Nested<DefaultAttribute> annotation);

        /// <seealso cref="FieldDescriptor.SetInCandidateKey(Context, UniqueAttribute)"/>
        public abstract void SetInCandidateKey(Context context, Nested<UniqueAttribute> annotation);

        /// <seealso cref="FieldDescriptor.SetInPrimaryKey(Context, PrimaryKeyAttribute, string)"/>
        public abstract void SetInPrimaryKey(Context context, Nested<PrimaryKeyAttribute> annotation, string cascadePath);

        /// <seealso cref="FieldDescriptor.SetName(Context, NameAttribute)"/>
        public abstract void SetName(Context context, Nested<NameAttribute> annotation);

        /// <seealso cref="FieldDescriptor.SetNamePrefix(Context, IReadOnlyList{string})"/>
        public abstract void SetNamePrefix(Context context, IEnumerable<string> prefix);

        /// <seealso cref="FieldDescriptor.SetNullability(Context, bool)"/>
        public abstract void SetNullability(Context context, bool nullable);

        /// <summary>
        ///   Sets the name of the Relation Table defined by the Fields in this <see cref="FieldGroup"/>.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which <paramref name="annotation"/> was encountered via reflection.
        /// </param>
        /// <param name="annotation">
        ///   The <see cref="RelationTableAttribute">[RelationTable]</see> annotation.
        /// </param>
        /// <exception cref="NotRelationException">
        ///   if this <see cref="FieldGroup"/> does not correspond to a Relation.
        /// </exception>
        /// <exception cref="InvalidNameException">
        ///   if the name carried by <paramref name="annotation"/> is <see langword="null"/> or is the empty string.
        /// </exception>
        protected virtual void SetRelationTableName(Context context, RelationTableAttribute annotation) {
            throw new NotRelationException(context, Source.PropertyType, typeof(RelationTableAttribute));
        }

        /// <summary>
        ///   Processes the native nullability of a property.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the property backing the <see cref="FieldGroup"/> was accessed via
        ///   reflection. This context should include that property.
        /// </param>
        protected abstract void ProcessNativeNullability(Context context);

        /// <summary>
        ///   Processes all of the annotations applied to the property backing the <see cref="FieldGroup"/>.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the property backing the <see cref="FieldGroup"/> was accessed via
        ///   reflection. This context should include that property.
        /// </param>
        /// <exception cref="InvalidPathException">
        ///   if the path of a "nestable" annotation applied to the property backing the <see cref="FieldGroup"/> is
        ///   <see langword="null"/>.
        /// </exception>
        protected virtual void ProcessAnnotations(Context context) {
            Debug.Assert(!Source.HasAttribute<CodeOnlyAttribute>());

            // [RelationTable]
            var relationTableAnnotation = Source.GetCustomAttribute<RelationTableAttribute>();
            if (relationTableAnnotation is not null) {
                SetRelationTableName(context, relationTableAnnotation);
            }

            // [Name]
            foreach (var nameAnnotation in Source.GetCustomAttributes<NameAttribute>()) {
                var _ = nameAnnotation.Path ?? throw new InvalidPathException(context, nameAnnotation);
                SetName(context, nameAnnotation);
            }

            // [Nullable]
            if (Source.HasAttribute<NullableAttribute>()) {
                SetNullability(context, true);
            }

            // [NonNullable]
            if (Source.HasAttribute<NonNullableAttribute>()) {
                if (Source.HasAttribute<NullableAttribute>()) {
                    throw new ConflictingAnnotationsException(context, typeof(NullableAttribute), typeof(NonNullableAttribute));
                }
                SetNullability(context, false);
            }

            // <Native Nullability>
            ProcessNativeNullability(context);

            // [Column]
            var columnAnnotation = Source.GetCustomAttribute<ColumnAttribute>();
            if (columnAnnotation is not null) {
                SetColumn(context, columnAnnotation.Column);
            }

            // [Default]
            foreach (var defaultAnnotation in Source.GetCustomAttributes<DefaultAttribute>()) {
                var _ = defaultAnnotation.Path ?? throw new InvalidPathException(context, defaultAnnotation);
                SetDefault(context, defaultAnnotation);
            }

            // [PrimaryKey]
            foreach (var pkAnnotation in Source.GetCustomAttributes<PrimaryKeyAttribute>()) {
                var _ = pkAnnotation.Path ?? throw new InvalidPathException(context, pkAnnotation);
                SetInPrimaryKey(context, pkAnnotation, "");
            }

            // [Unique]
            foreach (var ckAnnotation in Source.GetCustomAttributes<UniqueAttribute>()) {
                var _ = ckAnnotation.Path ?? throw new InvalidPathException(context, ckAnnotation);
                SetInCandidateKey(context, ckAnnotation);
            }

            // [Check]
            foreach (var checkAnnotation in Source.GetCustomAttributes<CheckAttribute>()) {
                var _ = checkAnnotation.Path ?? throw new InvalidPathException(context, checkAnnotation);
                ApplyConstraint(context, checkAnnotation);
            }

            // [Check] Signedness
            foreach (var signednessAnnotation in Source.GetCustomAttributes<Check.SignednessAttribute>()) {
                var _ = signednessAnnotation.Path ?? throw new InvalidPathException(context, signednessAnnotation);
                ApplyConstraint(context, signednessAnnotation);
            }

            // [Check] Comparison
            foreach (var comparisonAnnotation in Source.GetCustomAttributes<Check.ComparisonAttribute>()) {
                var _ = comparisonAnnotation.Path ?? throw new InvalidPathException(context, comparisonAnnotation);
                ApplyConstraint(context, comparisonAnnotation);
            }

            // [Check] String Length
            foreach (var stringLengthAnnotation in Source.GetCustomAttributes<Check.StringLengthAttribute>()) {
                var _ = stringLengthAnnotation.Path ?? throw new InvalidPathException(context, stringLengthAnnotation);
                ApplyConstraint(context, stringLengthAnnotation);
            }

            // [Check] Discreteness
            foreach (var discretenessAnnotation in Source.GetCustomAttributes<Check.InclusionAttribute>()) {
                var _ = discretenessAnnotation.Path ?? throw new InvalidPathException(context, discretenessAnnotation);
                ApplyConstraint(context, discretenessAnnotation);
            }
        }


        // A discriminator for the constructor to indicate that metadata should be reset
        protected struct ResetTag {}
        protected static readonly ResetTag RESET = new ResetTag();
    }
}
