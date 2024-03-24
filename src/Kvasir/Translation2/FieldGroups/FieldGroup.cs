using Cybele.Extensions;
using Kvasir.Annotations;
using Optional;
using System;
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
        ///   Clones this <see cref="FieldGroup"/>.
        /// </summary>
        /// <returns>
        ///   A deep copy of this <see cref="FieldGroup"/> as a brand new instance. Modifications to the returned value
        ///   will not affect the source instance, and vice-versa.
        /// </returns>
        public abstract FieldGroup Clone();

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
        public abstract void SetDefault(Context context, Nested<DefaultAttribute> annotation);

        /// <seealso cref="FieldDescriptor.SetInCandidateKey(Context, UniqueAttribute)"/>
        public abstract void SetInCandidateKey(Context context, Nested<UniqueAttribute> annotation);

        /// <seealso cref="FieldDescriptor.SetInPrimaryKey(Context, PrimaryKeyAttribute, string)"/>
        public abstract void SetInPrimaryKey(Context context, Nested<PrimaryKeyAttribute> annotation);

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
            throw new NotRelationException(context, Source.PropertyType, typeof(RelationTableAttribute));
        }

        /// <summary>
        ///   Processes all of the annotations applied to the property backing the <see cref="FieldGroup"/>.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the property backing the <see cref="FieldGroup"/> was accessed via
        ///   reflection. This context should not include that property.
        /// </param>
        /// <exception cref="InvalidPathException">
        ///   if the path of a "nestable" annotation applied to the property backing the <see cref="FieldGroup"/> is
        ///   <see langword="null"/>.
        /// </exception>
        protected virtual void ProcessAnnotations(Context context) {
            Debug.Assert(!Source.HasAttribute<CodeOnlyAttribute>());
            using var guard = context.Push(Source);

            // [RelationTable]
            var relationTableAnnotation = Source.GetCustomAttribute<RelationTableAttribute>();
            if (relationTableAnnotation is not null) {
                SetRelationTableName(context, relationTableAnnotation.Name);
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
                SetNullability(context, false);
            }

            // [Column]
            var columnAnnotation = Source.GetCustomAttribute<ColumnAttribute>();
            if (columnAnnotation is not null) {
                SetColumn(context, columnAnnotation.Column);
            }

            // [Default]
            foreach (var defaultAnnotation in Source.GetCustomAttributes<DefaultAttribute>()) {
                SetDefault(context, defaultAnnotation);
            }

            // [PrimaryKey]
            foreach (var pkAnnotation in Source.GetCustomAttributes<PrimaryKeyAttribute>()) {
                var _ = pkAnnotation.Path ?? throw new InvalidPathException(context, pkAnnotation);
                SetInPrimaryKey(context, pkAnnotation);
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
    }
}
