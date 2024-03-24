using Cybele.Extensions;
using Kvasir.Annotations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Kvasir.Translation2 {
    ///
    internal abstract class MultiFieldGroup : FieldGroup {
        /// <inheritdoc/>
        public sealed override int Size => fields_.Count;

        /// <inheritdoc/>
        public sealed override bool AllNullable => fields_.Values.All(g => g.AllNullable);

        /// <summary>
        ///   The nullable-stripped CLR type of the <see cref="FieldGroup.Source">source</see> property.
        /// </summary>
        private Type CLRType => Nullable.GetUnderlyingType(Source.PropertyType) ?? Source.PropertyType;

        ///
        protected abstract MultiKind Kind { get; }

        ///
        protected MultiFieldGroup(Context context, PropertyInfo source, IReadOnlyDictionary<string, FieldGroup> fields)
            : base(source) {

            Debug.Assert(context is not null);
            Debug.Assert(source is not null);
            Debug.Assert(fields is not null);

            if (fields.IsEmpty()) {
                // MUST BE AT LEAST ONE FIELD
            }

            name_ = source.Name;
            nullabilityAnnotated_ = false;
            nameAnnotated_ = false;
            fields_ = fields.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Clone());
            ProcessAnnotations(context);

            if (!nullabilityAnnotated_) {
                var native = new NullabilityInfoContext().Create(source).ReadState == NullabilityState.Nullable;
                SetNativeNullability(context, native);
            }
        }

        ///
        protected MultiFieldGroup(MultiFieldGroup source)
            : base(source) {

            name_ = source.name_;
            nameAnnotated_ = false;
            nullabilityAnnotated_ = false;
            fields_ = source.fields_.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Clone());
        }

        /// <inheritdoc/>
        public sealed override void ApplyConstraint(Context context, Nested<CheckAttribute> annotation) {
            Debug.Assert(context is not null);
            Debug.Assert(annotation.Annotation is not null);

            if (annotation.AppliesHere) {
                throw new InapplicableConstraintException(context, annotation.Annotation, CLRType, Kind);
            }
            else if (!fields_.TryGetValue(annotation.NextPath, out var group)) {
                throw new InvalidPathException(context, annotation.Annotation);
            }
            else {
                group.ApplyConstraint(context, annotation.Step());
            }
        }

        /// <inheritdoc/>
        public sealed override void ApplyConstraint(Context context, Nested<Check.ComparisonAttribute> annotation) {
            Debug.Assert(context is not null);
            Debug.Assert(annotation.Annotation is not null);

            if (annotation.AppliesHere) {
                throw new InapplicableConstraintException(context, annotation.Annotation, CLRType, Kind);
            }
            else if (!fields_.TryGetValue(annotation.NextPath, out var group)) {
                throw new InvalidPathException(context, annotation.Annotation);
            }
            else {
                group.ApplyConstraint(context, annotation.Step());
            }
        }

        /// <inheritdoc/>
        public sealed override void ApplyConstraint(Context context, Nested<Check.InclusionAttribute> annotation) {
            Debug.Assert(context is not null);
            Debug.Assert(annotation.Annotation is not null);

            if (annotation.AppliesHere) {
                throw new InapplicableConstraintException(context, annotation.Annotation, CLRType, Kind);
            }
            else if (!fields_.TryGetValue(annotation.NextPath, out var group)) {
                throw new InvalidPathException(context, annotation.Annotation);
            }
            else {
                group.ApplyConstraint(context, annotation.Step());
            }
        }

        /// <inheritdoc/>
        public sealed override void ApplyConstraint(Context context, Nested<Check.SignednessAttribute> annotation) {
            Debug.Assert(context is not null);
            Debug.Assert(annotation.Annotation is not null);

            if (annotation.AppliesHere) {
                throw new InapplicableConstraintException(context, annotation.Annotation, CLRType, Kind);
            }
            else if (!fields_.TryGetValue(annotation.NextPath, out var group)) {
                throw new InvalidPathException(context, annotation.Annotation);
            }
            else {
                group.ApplyConstraint(context, annotation.Step());
            }
        }

        /// <inheritdoc/>
        public sealed override void ApplyConstraint(Context context, Nested<Check.StringLengthAttribute> annotation) {
            Debug.Assert(context is not null);
            Debug.Assert(annotation.Annotation is not null);

            if (annotation.AppliesHere) {
                throw new InapplicableConstraintException(context, annotation.Annotation, CLRType, Kind);
            }
            else if (!fields_.TryGetValue(annotation.NextPath, out var group)) {
                throw new InvalidPathException(context, annotation.Annotation);
            }
            else {
                group.ApplyConstraint(context, annotation.Step());
            }
        }

        /// <inheritdoc/>
        public sealed override void SetDefault(Context context, Nested<DefaultAttribute> annotation) {
            Debug.Assert(context is not null);
            Debug.Assert(annotation.Annotation is not null);

            if (annotation.AppliesHere) {
                // INVALID ANNOTATION BUT NOT CONSTRAINT
            }
            else if (!fields_.TryGetValue(annotation.NextPath, out var group)) {
                throw new InvalidPathException(context, annotation.Annotation);
            }
            else {
                group.SetDefault(context, annotation.Step());
            }
        }

        /// <inheritdoc/>
        public sealed override void SetInCandidateKey(Context context, Nested<UniqueAttribute> annotation) {
            Debug.Assert(context is not null);
            Debug.Assert(annotation.Annotation is not null);

            if (annotation.AppliesHere) {
                foreach (var field in fields_.Values) {
                    field.SetInCandidateKey(context, annotation);
                }
            }
            else if (!fields_.TryGetValue(annotation.NextPath, out var group)) {
                throw new InvalidPathException(context, annotation.Annotation);
            }
            else {
                group.SetInCandidateKey(context, annotation.Step());
            }
        }

        /// <inheritdoc/>
        public sealed override void SetInPrimaryKey(Context context, Nested<PrimaryKeyAttribute> annotation) {
            Debug.Assert(context is not null);
            Debug.Assert(annotation.Annotation is not null);

            if (annotation.AppliesHere) {
                foreach (var field in fields_.Values) {
                    field.SetInPrimaryKey(context, annotation);
                }
            }
            else if (!fields_.TryGetValue(annotation.NextPath, out var group)) {
                throw new InvalidPathException(context, annotation.Annotation);
            }
            else {
                group.SetInPrimaryKey(context, annotation.Step());
            }
        }

        /// <inheritdoc/>
        public sealed override void SetName(Context context, Nested<NameAttribute> annotation) {
            if (annotation.AppliesHere) {
                if (nameAnnotated_ && annotation.Annotation.Name != name_) {
                    throw new DuplicateAnnotationException(context, annotation.Annotation.Path, typeof(NameAttribute));
                }
                name_ = annotation.Annotation.Name;
                nameAnnotated_ = true;
            }
            else if (!fields_.TryGetValue(annotation.NextPath, out var group)) {
                throw new InvalidPathException(context, annotation.Annotation);
            }
            else {
                group.SetName(context, annotation.Step());
            }
        }

        /// <inheritdoc/>
        public sealed override void SetNamePrefix(Context context, IEnumerable<string> prefix) {
            var pfx = prefix.Append(name_);
            foreach (var field in fields_.Values) {
                field.SetNamePrefix(context, pfx);
            }
        }

        /// <inheritdoc/>
        public sealed override void SetNullability(Context context, bool nullable) {
            Debug.Assert(context is not null);

            // Nullability on an grouping property is a bit nuanced. As with a scalar/enumeration, the property cannot
            // be annotated as both [Nullable] and [NonNullable]. If the property is annotated as [Nullable], then all
            // the nested Fields are also made nullable. However, if the property is annotated as [NonNullable], this
            // only suppresses the property's native nullability from applying to nested Fields. In particular, nullable
            // nested Fields are not made non-nullable by a [NonNullable] annotation.

            if (nullabilityAnnotated_) {
                throw new ConflictingAnnotationsException(context, typeof(NullableAttribute), typeof(NonNullableAttribute));
            }
            else if (nullable && AllNullable) {
                // AMBIGUOUS NULLABILITY
            }
            else if (nullable) {
                foreach (var group in fields_.Values) {
                    group.SetNullability(context, true);
                }
                nullabilityAnnotated_ = true;
            }
            else {
                nullabilityAnnotated_ = true;
            }
        }

        /// <inheritdoc/>
        protected sealed override void ProcessAnnotations(Context context) {
            base.ProcessAnnotations(context);
            using var guard = context.Push(Source);

            // [DataConverter]
            if (Source.HasAttribute<DataConverterAttribute>()) {
                // INVALID BUT NOT A CONSTRAINT
            }

            // [AsString]
            if (Source.HasAttribute<AsStringAttribute>()) {
                // INVALID BUT NOT A CONSTRAINT
            }

            // [Numeric]
            if (Source.HasAttribute<NumericAttribute>()) {
                // INVALID BUT NOT A CONSTRAINT
            }
        }

        ///
        protected virtual void SetNativeNullability(Context context, bool nullable) {
            Debug.Assert(context is not null);

            // For more details on how we deal with nullability for these kinds of properties, see the more in-depth
            // comment in SetNullability

            if (nullable && AllNullable) {
                // AMBIGUOUS NULLABILITY
            }
            else if (nullable) {
                foreach (var group in fields_.Values) {
                    group.SetNullability(context, true);
                }
            }
        }


        private string name_;
        private bool nullabilityAnnotated_;
        private bool nameAnnotated_;
        private readonly IReadOnlyDictionary<string, FieldGroup> fields_;
    }
}
