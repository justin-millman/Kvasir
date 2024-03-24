using Cybele.Extensions;
using Kvasir.Annotations;
using Optional;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace Kvasir.Translation2 {
    /// <summary>
    ///   The base class for the data state that manages the translation of one or more Fields in a back-end database,
    ///   each derived from the same CLR property and those thereunder nested.
    /// </summary>
    internal abstract class FieldGroup {
        /// <summary>
        ///   The number of Fields in the <see cref="FieldGroup"/>.
        /// </summary>
        public abstract int Size { get; }

        /// <summary>
        ///   The starting column index of the <see cref="FieldGroup"/>.
        /// </summary>
        public Option<int> Column { get; private set; }

        /// <summary>
        ///   Constructs a new <see cref="FieldGroup"/>.
        /// </summary>
        /// <param name="source">
        ///   The CLR property from which the new <see cref="FieldGroup"/> is derived.
        /// </param>
        protected FieldGroup(PropertyInfo source) {
            Debug.Assert(source is not null);

            source_ = source;
            processed_ = false;
            Column = Option.None<int>();
        }

        /// <summary>
        ///   Constructs a new <see cref="FieldGroup"/> that is largely identical to another.
        /// </summary>
        /// <param name="source">
        ///   The source <see cref="FieldGroup"/>.
        /// </param>
        /// <seealso cref="Clone"/>
        protected FieldGroup(FieldGroup source) {
            Debug.Assert(source is not null);
            Debug.Assert(source.processed_);

            source_ = source.source_;
            processed_ = true;
            Column = source.Column;
        }

        /// <seealso cref="FieldDescriptor.ApplyConstraint(Context, Check.ComparisonAttribute)"/>
        protected abstract void ApplyConstraint(Context context, Nested<CheckAttribute> annotation);

        /// <seealso cref="FieldDescriptor.ApplyConstraint(Context, Check.ComparisonAttribute)"/>
        protected abstract void ApplyConstraint(Context context, Nested<Check.ComparisonAttribute> annotation);

        /// <seealso cref="FieldDescriptor.ApplyConstraint(Context, Check.InclusionAttribute)"/>
        protected abstract void ApplyConstraint(Context context, Nested<Check.InclusionAttribute> annotation);

        /// <seealso cref="FieldDescriptor.ApplyConstraint(Context, Check.SignednessAttribute)"/>
        protected abstract void ApplyConstraint(Context context, Nested<Check.SignednessAttribute> annotation);

        /// <seealso cref="FieldDescriptor.ApplyConstraint(Context, Check.StringLengthAttribute)"/>
        protected abstract void ApplyConstraint(Context context, Nested<Check.StringLengthAttribute> annotation);

        /// <summary>
        ///   Clones this <see cref="FieldGroup"/>.
        /// </summary>
        /// <returns>
        ///   A deep copy of this <see cref="FieldGroup"/> as a brand new instance. Modifications to the returned value
        ///   will not affect the source instance, and vice-versa.
        /// </returns>
        protected abstract FieldGroup Clone();

        /// <summary>
        ///   Sets the column index for the group.
        /// </summary>
        /// <remarks>
        ///   The Fields that make up a FieldGroup are guaranteed to be contiguous. The column index of a FieldGroup is
        ///   therefore the index of the first of its Fields.
        /// </remarks>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the <see cref="ColumnAttribute">[Column]</see> annotation was
        ///   translated via reflection.
        /// </param>
        /// <param name="index">
        ///   The column index.
        /// </param>
        /// <exception cref="InvalidColumnIndexException">
        ///   if <paramref name="index"/> is negative.
        /// </exception>
        private void SetColumn(Context context, int index) {
            Debug.Assert(context is not null);
            Debug.Assert(!Column.HasValue);

            if (index < 0) {
                throw new InvalidColumnIndexException(context, index);
            }
            Column = Option.Some(index);
        }

        /// <seealso cref="FieldDescriptor.SetDefault(Context, DefaultAttribute)"/>
        protected abstract void SetDefault(Context context, Nested<DefaultAttribute> annotation);

        /// <seealso cref="FieldDescriptor.SetInCandidateKey(Context, UniqueAttribute)"/>
        protected abstract void SetInCandidateKey(Context context, Nested<UniqueAttribute> annotation);

        /// <seealso cref="FieldDescriptor.SetInPrimaryKey(Context, PrimaryKeyAttribute, string)"/>
        protected abstract void SetInPrimaryKey(Context context, Nested<PrimaryKeyAttribute> annotation);

        /// <seealso cref="FieldDescriptor.SetName(Context, NameAttribute)"/>
        protected abstract void SetName(Context context, Nested<NameAttribute> annotation);

        /// <seealso cref="FieldDescriptor.SetNamePrefix(Context, IReadOnlyList{string})"/>
        protected abstract void SetNamePrefix(Context context, IReadOnlyList<string> prefix);

        /// <seealso cref="FieldDescriptor.SetNullability(Context, bool)"/>
        protected abstract void SetNullability(Context context, bool nullable);

        /// <summary>
        ///   Sets the name of the Relation Table defined by the Fields in this <see cref="FieldGroup"/>.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the <see cref="RelationTableAttribute">[RelationTable]</see> annotation
        ///   was encountered via reflection.
        /// </param>
        /// <param name="name">
        ///   The name of the Relation Table imparted by the annotation.
        /// </param>
        /// <exception cref="NotRelationException">
        ///   if this <see cref="FieldGroup"/> does not correspond to a Relation.
        /// </exception>
        /// <exception cref="InvalidNameException">
        ///   if <paramref name="name"/> is <see langword="null"/> or is the empty string.
        /// </exception>
        protected virtual void SetRelationTableName(Context context, string name) {
            throw new NotRelationException(context, source_.PropertyType, typeof(RelationTableAttribute));
        }

        /// <summary>
        ///   Processes all of the annotations applied to the property backing a particular <see cref="FieldGroup"/>.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the property backing <paramref name="group"/> was accessed via
        ///   reflection. This context should not include that property.
        /// </param>
        /// <param name="group">
        ///   The <see cref="FieldGroup"/> to act upon.
        /// </param>
        /// <exception cref="InvalidPathException">
        ///   if the path of a "nestable" annotation applied to the property backing <paramref name="group"/> is
        ///   <see langword="null"/>.
        /// </exception>
        protected static void ProcessAnnotations(Context context, FieldGroup group) {
            Debug.Assert(!group.processed_);
            Debug.Assert(!group.source_.HasAttribute<CodeOnlyAttribute>());
            using var guard = context.Push(group.source_);

            // [RelationTable]
            var relationTableAnnotation = group.source_.GetCustomAttribute<RelationTableAttribute>();
            if (relationTableAnnotation is not null) {
                group.SetRelationTableName(context, relationTableAnnotation.Name);
            }

            // [Name]
            foreach (var nameAnnotation in group.source_.GetCustomAttributes<NameAttribute>()) {
                var _ = nameAnnotation.Path ?? throw new InvalidPathException(context, nameAnnotation);
                group.SetName(context, nameAnnotation);
            }

            // [Nullable]
            if (group.source_.HasAttribute<NullableAttribute>()) {
                group.SetNullability(context, true);
            }

            // [NonNullable]
            if (group.source_.HasAttribute<NonNullableAttribute>()) {
                group.SetNullability(context, false);
            }

            // [Column]
            var columnAnnotation = group.source_.GetCustomAttribute<ColumnAttribute>();
            if (columnAnnotation is not null) {
                group.SetColumn(context, columnAnnotation.Column);
            }

            // [Default]
            foreach (var defaultAnnotation in group.source_.GetCustomAttributes<DefaultAttribute>()) {
                group.SetDefault(context, defaultAnnotation);
            }

            // [PrimaryKey]
            foreach (var pkAnnotation in group.source_.GetCustomAttributes<PrimaryKeyAttribute>()) {
                var _ = pkAnnotation.Path ?? throw new InvalidPathException(context, pkAnnotation);
                group.SetInPrimaryKey(context, pkAnnotation);
            }

            // [Unique]
            foreach (var ckAnnotation in group.source_.GetCustomAttributes<UniqueAttribute>()) {
                var _ = ckAnnotation.Path ?? throw new InvalidPathException(context, ckAnnotation);
                group.SetInCandidateKey(context, ckAnnotation);
            }

            // [Check]
            foreach (var checkAnnotation in group.source_.GetCustomAttributes<CheckAttribute>()) {
                var _ = checkAnnotation.Path ?? throw new InvalidPathException(context, checkAnnotation);
                group.ApplyConstraint(context, checkAnnotation);
            }

            // [Check] Signedness
            foreach (var signednessAnnotation in group.source_.GetCustomAttributes<Check.SignednessAttribute>()) {
                var _ = signednessAnnotation.Path ?? throw new InvalidPathException(context, signednessAnnotation);
                group.ApplyConstraint(context, signednessAnnotation);
            }

            // [Check] Comparison
            foreach (var comparisonAnnotation in group.source_.GetCustomAttributes<Check.ComparisonAttribute>()) {
                var _ = comparisonAnnotation.Path ?? throw new InvalidPathException(context, comparisonAnnotation);
                group.ApplyConstraint(context, comparisonAnnotation);
            }

            // [Check] String Length
            foreach (var stringLengthAnnotation in group.source_.GetCustomAttributes<Check.StringLengthAttribute>()) {
                var _ = stringLengthAnnotation.Path ?? throw new InvalidPathException(context, stringLengthAnnotation);
                group.ApplyConstraint(context, stringLengthAnnotation);
            }

            // [Check] Discreteness
            foreach (var discretenessAnnotation in group.source_.GetCustomAttributes<Check.InclusionAttribute>()) {
                var _ = discretenessAnnotation.Path ?? throw new InvalidPathException(context, discretenessAnnotation);
                group.ApplyConstraint(context, discretenessAnnotation);
            }

            group.processed_ = true;
        }


        private readonly PropertyInfo source_;
        private bool processed_;
    }
}
