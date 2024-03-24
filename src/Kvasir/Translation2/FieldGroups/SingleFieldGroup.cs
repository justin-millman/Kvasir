using Cybele.Extensions;
using Kvasir.Annotations;
using Kvasir.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace Kvasir.Translation2 {
    /// <summary>
    ///   The intermediate base class for a <see cref="FieldGroup"/> backed by a scalar or enumeration CLR property,
    ///   and therefore corresponding to exactly one Field.
    /// </summary>
    internal sealed class SingleFieldGroup : FieldGroup {
        /// <inheritdoc/>
        public sealed override int Size => 1;

        /// <inheritdoc/>
        public sealed override bool AllNullable => field_.IsNullable;

        /// <summary>
        ///   Constructs a new <see cref="SingleFieldGroup"/>.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which <paramref name="source"/> was accessed via reflection. This context
        ///   should not include that property.
        /// </param>
        /// <param name="source">
        ///   The CLR property backing the new <see cref="SingleFieldGroup"/>.
        /// </param>
        public SingleFieldGroup(Context context, PropertyInfo source)
            : base(source) {

            Debug.Assert(context is not null);
            Debug.Assert(source is not null);

            field_ = MakeField(context, source);
            ProcessAnnotations(context);
        }

        /// <summary>
        ///   Constructs a new <see cref="SingleFieldGroup"/> that is largely identical to another.
        /// </summary>
        /// <param name="source">
        ///   The source <see cref="SingleFieldGroup"/>.
        /// </param>
        /// <seealso cref="FieldGroup.Clone"/>
        private SingleFieldGroup(SingleFieldGroup source)
            : base(source) {

            field_ = source.field_.Clone();
        }

        /// <inheritdoc/>
        public sealed override SingleFieldGroup Clone() {
            return new SingleFieldGroup(this);
        }

        /// <inheritdoc/>
        public sealed override void ApplyConstraint(Context context, Nested<CheckAttribute> annotation) {
            Debug.Assert(context is not null);
            Debug.Assert(annotation.Annotation is not null);

            if (!annotation.AppliesHere) {
                throw new InvalidPathException(context, annotation.Annotation);
            }
            field_.ApplyConstraint(context, annotation.Annotation);
        }

        /// <inheritdoc/>
        public sealed override void ApplyConstraint(Context context, Nested<Check.ComparisonAttribute> annotation) {
            Debug.Assert(context is not null);
            Debug.Assert(annotation.Annotation is not null);

            if (!annotation.AppliesHere) {
                throw new InvalidPathException(context, annotation.Annotation);
            }
            field_.ApplyConstraint(context, annotation.Annotation);
        }

        /// <inheritdoc/>
        public sealed override void ApplyConstraint(Context context, Nested<Check.InclusionAttribute> annotation) {
            Debug.Assert(context is not null);
            Debug.Assert(annotation.Annotation is not null);

            if (!annotation.AppliesHere) {
                throw new InvalidPathException(context, annotation.Annotation);
            }
            field_.ApplyConstraint(context, annotation.Annotation);
        }

        /// <inheritdoc/>
        public sealed override void ApplyConstraint(Context context, Nested<Check.SignednessAttribute> annotation) {
            Debug.Assert(context is not null);
            Debug.Assert(annotation.Annotation is not null);

            if (!annotation.AppliesHere) {
                throw new InvalidPathException(context, annotation.Annotation);
            }
            field_.ApplyConstraint(context, annotation.Annotation);
        }

        /// <inheritdoc/>
        public sealed override void ApplyConstraint(Context context, Nested<Check.StringLengthAttribute> annotation) {
            Debug.Assert(context is not null);
            Debug.Assert(annotation.Annotation is not null);

            if (!annotation.AppliesHere) {
                throw new InvalidPathException(context, annotation.Annotation);
            }
            field_.ApplyConstraint(context, annotation.Annotation);
        }

        /// <inheritdoc/>
        public sealed override void SetDefault(Context context, Nested<DefaultAttribute> annotation) {
            Debug.Assert(context is not null);
            Debug.Assert(annotation.Annotation is not null);

            if (!annotation.AppliesHere) {
                throw new InvalidPathException(context, annotation.Annotation);
            }
            field_.SetDefault(context, annotation.Annotation);
        }

        /// <inheritdoc/>
        public sealed override void SetInCandidateKey(Context context, Nested<UniqueAttribute> annotation) {
            Debug.Assert(context is not null);
            Debug.Assert(annotation.Annotation is not null);

            if (!annotation.AppliesHere) {
                throw new InvalidPathException(context, annotation.Annotation);
            }
            field_.SetInCandidateKey(context, annotation.Annotation);
        }

        /// <inheritdoc/>
        public sealed override void SetInPrimaryKey(Context context, Nested<PrimaryKeyAttribute> annotation) {
            Debug.Assert(context is not null);
            Debug.Assert(annotation.Annotation is not null);

            if (!annotation.AppliesHere) {
                throw new InvalidPathException(context, annotation.Annotation);
            }
            field_.SetInPrimaryKey(context, annotation.Annotation);
        }

        /// <inheritdoc/>
        public sealed override void SetName(Context context, Nested<NameAttribute> annotation) {
            Debug.Assert(context is not null);
            Debug.Assert(annotation.Annotation is not null);

            if (!annotation.AppliesHere) {
                throw new InvalidPathException(context, annotation.Annotation);
            }
            field_.SetName(context, annotation.Annotation);
        }

        /// <inheritdoc/>
        public sealed override void SetNamePrefix(Context context, IEnumerable<string> prefix) {
            Debug.Assert(context is not null);
            Debug.Assert(prefix is not null && !prefix.IsEmpty());
            Debug.Assert(prefix.None(s => s is null || s == ""));

            field_.SetNamePrefix(context, new List<string>(prefix));
        }

        /// <inheritdoc/>
        public sealed override void SetNullability(Context context, bool nullable) {
            Debug.Assert(context is not null);
            field_.SetNullability(context, nullable);
        }

        /// <summary>
        ///   Make a <see cref="FieldDescriptor"/> of the correct derived type from a property.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which <paramref name="source"/> was accessed via reflection. This context
        ///   should not include that property.
        /// </param>
        /// <param name="source">
        ///   The CLR property from which to create a <see cref="FieldDescriptor"/>.
        /// </param>
        /// <returns>
        ///   A <see cref="FieldDescriptor"/> based on <paramref name="source"/>, accounting only for annotations that
        ///   impart data conversions.
        /// </returns>
        /// <exception cref="InvalidDataConverterException">
        ///   if <paramref name="source"/> is annotated with either <see cref="AsStringAttribute">[AsString]</see> or
        ///   <see cref="NumericAttribute">[Numeric]</see> but is not of enumeration type.
        /// </exception>
        /// <exception cref="ConflictingAnnotationsException">
        ///   if <paramref name="source"/> is annotated with at least 2 of
        ///   <see cref="AsStringAttribute">[AsString]</see>, <see cref="DataConverterAttribute">[DataConverter]</see>,
        ///   and <see cref="NumericAttribute">[Numeric]</see>.
        /// </exception>
        private static FieldDescriptor MakeField(Context context, PropertyInfo source) {
            Debug.Assert(context is not null);
            Debug.Assert(source is not null);
            using var guard = context.Push(source);

            var propertyType = Nullable.GetUnderlyingType(source.PropertyType) ?? source.PropertyType;
            var dataConverterAnnotation = source.GetCustomAttribute<DataConverterAttribute>();
            var asStringAnnotation = source.GetCustomAttribute<AsStringAttribute>();
            var numericAnnotation = source.GetCustomAttribute<NumericAttribute>();

            // Error Conditions
            if (asStringAnnotation is not null && !propertyType.IsEnum) {
                throw new InvalidDataConverterException(context, propertyType, asStringAnnotation);
            }
            else if (numericAnnotation is not null && !propertyType.IsEnum) {
                throw new InvalidDataConverterException(context, propertyType, numericAnnotation);
            }
            else if (asStringAnnotation is not null && numericAnnotation is not null) {
                throw new ConflictingAnnotationsException(context, typeof(AsStringAttribute), typeof(NumericAttribute));
            }
            else if (dataConverterAnnotation is not null && asStringAnnotation is not null) {
                throw new ConflictingAnnotationsException(context, typeof(DataConverterAttribute), typeof(AsStringAttribute));
            }
            else if (dataConverterAnnotation is not null && numericAnnotation is not null) {
                throw new ConflictingAnnotationsException(context, typeof(DataConverterAttribute), typeof(NumericAttribute));
            }

            // Enumeration-type Property with [Numeric]
            else if (numericAnnotation is not null) {
                var converter = new EnumToNumericConverter(propertyType);
                return new EnumFieldDescriptor(context, source, converter.ConverterImpl);
            }

            // Enumeration-type Property with [AsString]
            else if (asStringAnnotation is not null) {
                var converter = new EnumToStringConverter(propertyType);
                return new EnumFieldDescriptor(context, source, converter.ConverterImpl);
            }

            // Enumeration-type Property, with or without [DataConverter]
            else if (propertyType.IsEnum) {
                return dataConverterAnnotation is null ?
                    new EnumFieldDescriptor(context, source) :
                    new EnumFieldDescriptor(context, source, dataConverterAnnotation);
            }

            // Bool / Char / DateTime / Decimal / Guid / String, with or without [DataConverter]
            else if (propertyType == typeof(bool)) {
                return dataConverterAnnotation is null ?
                    new BooleanFieldDescriptor(context, source) :
                    new BooleanFieldDescriptor(context, source, dataConverterAnnotation);
            }
            else if (propertyType == typeof(char)) {
                return dataConverterAnnotation is null ?
                    new CharFieldDescriptor(context, source) :
                    new CharFieldDescriptor(context, source, dataConverterAnnotation);
            }
            else if (propertyType == typeof(DateTime)) {
                return dataConverterAnnotation is null ?
                    new DateTimeFieldDescriptor(context, source) :
                    new DateTimeFieldDescriptor(context, source, dataConverterAnnotation);
            }
            else if (propertyType == typeof(decimal)) {
                return dataConverterAnnotation is null ?
                    new DecimalFieldDescriptor(context, source) :
                    new DecimalFieldDescriptor(context, source, dataConverterAnnotation);
            }
            else if (propertyType == typeof(Guid)) {
                return dataConverterAnnotation is null ?
                    new GuidFieldDescriptor(context, source) :
                    new GuidFieldDescriptor(context, source, dataConverterAnnotation);
            }
            else if (propertyType == typeof(string)) {
                return dataConverterAnnotation is null ?
                    new StringFieldDescriptor(context, source) :
                    new StringFieldDescriptor(context, source, dataConverterAnnotation);
            }

            // Unsigned Numeric-type Property, with or without [DataConverter]
            else if (propertyType == typeof(byte) || propertyType == typeof(ushort) ||
                     propertyType == typeof(uint) || propertyType == typeof(ulong)) {

                return dataConverterAnnotation is null ?
                    new UnsignedFieldDescriptor(context, source) :
                    new UnsignedFieldDescriptor(context, source, dataConverterAnnotation);
            }

            // Signed Numeric-type Property, with or without [DataConverter]
            else {
                return dataConverterAnnotation is null ?
                    new SignedFieldDescriptor(context, source) :
                    new SignedFieldDescriptor(context, source, dataConverterAnnotation);
            }
        }


        private readonly FieldDescriptor field_;
    }
}
