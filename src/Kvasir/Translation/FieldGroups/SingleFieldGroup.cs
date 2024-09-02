using Cybele.Extensions;
using Kvasir.Annotations;
using Kvasir.Core;
using Kvasir.Extraction;
using Kvasir.Reconstitution;
using Optional;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Kvasir.Translation {
    /// <summary>
    ///   A <see cref="FieldGroup"/> backed by a scalar or enumeration CLR property, and therefore corresponding to
    ///   exactly one Field.
    /// </summary>
    internal sealed class SingleFieldGroup : FieldGroup {
        /// <inheritdoc/>
        public sealed override int Size => 1;

        /// <inheritdoc/>
        public sealed override bool AllNullable => field_.IsNullable;

        /// <inheritdoc/>
        public sealed override bool IsNativelyNullable { get; }

        /// <inheritdoc/>
        public sealed override string ReconstitutionArgumentName => reconstitutionArgumentName_;

        /// <inheritdoc/>
        public sealed override string this[int column] {
            get {
                Debug.Assert(column == 0);
                Debug.Assert(Column.HasValue);
                return Source.Name;
            }
        }

        /// <inheritdoc/>
        public sealed override ISingleExtractor Extractor { get; }

        /// <inheritdoc/>
        public sealed override Option<ICreator> Creator { get; protected init; }

        /// <summary>
        ///   Constructs a new <see cref="SingleFieldGroup"/>.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which <paramref name="source"/> was accessed via reflection. This context
        ///   should include that property.
        /// </param>
        /// <param name="source">
        ///   The CLR property backing the new <see cref="SingleFieldGroup"/>.
        /// </param>
        public SingleFieldGroup(Context context, PropertyInfo source)
            : base(source) {

            Debug.Assert(context is not null);
            Debug.Assert(source is not null);

            field_ = MakeField(context, source);
            reconstitutionArgumentName_ = source.Name;
            ProcessAnnotations(context);
            IsNativelyNullable = field_.IsNullable;

            // Extraction for a SingleFieldGroup consists of two parts: reading the value out of the source property,
            // and then applying any data conversion to it. If the Field has an Enumeration type, we have to
            // additionally convert the post-conversion result into a string, since Enumerations cannot be stored
            // directly.
            Extractor = new ConvertingExtractor(new ReadPropertyExtractor(source), field_.Converter);
            if (field_.Converter.ResultType.IsEnum) {
                var toString = new EnumToStringConverter(field_.Converter.ResultType);
                Extractor = new ConvertingExtractor(Extractor, toString.ConverterImpl);
            }

            // Creation for a SingleFieldGroup is the reverse of Extraction. If the Field has an Enumeration type, we
            // have to first revert the original value from a string, since Enumerations cannot be stored directly.
            // Then we apply any additional data reversion before finishing with our value. If the source property is
            // annotated as being [Calculated], though, we skip everything.
            Creator = Option.None<ICreator>();
            if (!Source.HasAttribute<CalculatedAttribute>()) {
                if (field_.Converter.ResultType.IsEnum) {
                    var toString = new EnumToStringConverter(field_.Converter.ResultType);
                    var baseCreator = new RevertingCreator(new IdentityCreator(typeof(string)), toString.ConverterImpl);
                    Creator = Option.Some<ICreator>(new RevertingCreator(baseCreator, field_.Converter));
                }
                else {
                    var baseCreator = new IdentityCreator(field_.Converter.ResultType);
                    Creator = Option.Some<ICreator>(new RevertingCreator(baseCreator, field_.Converter));
                }
            }
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
            reconstitutionArgumentName_ = source.reconstitutionArgumentName_;
            IsNativelyNullable = source.IsNativelyNullable;
            Extractor = source.Extractor;
            Creator = source.Creator;
        }

        /// <summary>
        ///   Constructs a new <see cref="SingleFieldGroup"/> that is largely identical to another, but with the
        ///   constituent Field reset.
        /// </summary>
        /// <param name="source">
        ///   The source <see cref="SingleFieldGroup"/>.
        /// </param>
        /// <param name="_">
        ///   <i>overload discriminator</i>
        /// </param>
        /// <seealso cref="Reset"/>
        private SingleFieldGroup(SingleFieldGroup source, ResetTag _)
            : base(source) {

            field_ = source.field_.Reset();
            reconstitutionArgumentName_ = source.reconstitutionArgumentName_;
            IsNativelyNullable = source.IsNativelyNullable;
            Extractor = source.Extractor;
            Creator = source.Creator;
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
        public sealed override Option<FieldGroup> Filter(IEnumerable<FieldDescriptor> constituents) {
            if (constituents.Contains(field_)) {
                return Option.Some<FieldGroup>(Clone());
            }
            else {
                return Option.None<FieldGroup>();
            }
        }

        /// <inheritdoc/>
        public sealed override IEnumerator<FieldDescriptor> GetEnumerator() {
            yield return field_;
        }

        /// <inheritdoc/>
        public sealed override IEnumerable<ReferenceFieldGroup> References() {
            return Enumerable.Empty<ReferenceFieldGroup>();
        }

        /// <inheritdoc/>
        public sealed override SingleFieldGroup Reset() {
            return new SingleFieldGroup(this, RESET);
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
        public sealed override void SetInPrimaryKey(Context context, Nested<PrimaryKeyAttribute> annotation, string cascadePath) {
            Debug.Assert(context is not null);
            Debug.Assert(annotation.Annotation is not null);

            if (!annotation.AppliesHere) {
                throw new InvalidPathException(context, annotation.Annotation);
            }
            field_.SetInPrimaryKey(context, annotation.Annotation, cascadePath[..^Math.Min(1, cascadePath.Length)]);
        }

        /// <inheritdoc/>
        public sealed override void SetName(Context context, Nested<NameAttribute> annotation) {
            Debug.Assert(context is not null);
            Debug.Assert(annotation.Annotation is not null);

            if (!annotation.AppliesHere) {
                throw new InvalidPathException(context, annotation.Annotation);
            }
            field_.SetName(context, annotation.Annotation);

            // only update the Reconstitution Argument Name if the [Name] annotation is directly applied
            if (annotation.Annotation.Path == "") {
                reconstitutionArgumentName_ = annotation.Annotation.Name;
            }
        }

        /// <inheritdoc/>
        public sealed override void SetNamePrefix(Context context, IEnumerable<string> prefix) {
            Debug.Assert(context is not null);
            Debug.Assert(prefix is not null && !prefix.IsEmpty());
            Debug.Assert(prefix.None(s => s is null || s == ""));

            field_.SetNamePrefix(context, new List<string>(prefix));
            // changing the name prefix is only done for nested fields, so it doesn't affect the Reconstitution argument
        }

        /// <inheritdoc/>
        public sealed override void SetNullability(Context context, bool nullable) {
            Debug.Assert(context is not null);
            field_.SetNullability(context, nullable);
        }

        /// <inheritdoc/>
        protected sealed override void ProcessNativeNullability(Context context) {
            // Native nullability is IGNORED by SingleFieldGroups, because the individual FieldDescriptors handle it on
            // their own. They can do this because there's no possibility for ambiguity, unlike with MultiFieldGroups.
        }

        /// <summary>
        ///   Make a <see cref="FieldDescriptor"/> of the correct derived type from a property.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which <paramref name="source"/> was accessed via reflection. This context
        ///   should include that property.
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

            // Determine Effective Type
            Type determineEffectiveType() {
                if (asStringAnnotation is not null) {
                    return typeof(string);
                }
                else if (numericAnnotation is not null) {
                    return Enum.GetUnderlyingType(propertyType);
                }
                else if (dataConverterAnnotation is null) {
                    return propertyType;
                }
                else if (dataConverterAnnotation.UserError is not null) {
                    // It does not matter what Type we return here, since the invalid annotation will eventually be
                    // flagged as an error. But since there is a problem, we can't safely access the Converter impl.
                    return typeof(int);
                }
                else {
                    return dataConverterAnnotation.DataConverter.ResultType;
                }
            }
            var effectiveType = determineEffectiveType();

            // Enumeration
            if (effectiveType.IsEnum) {
                return dataConverterAnnotation is null ?
                    new EnumFieldDescriptor(context, source) :
                    new EnumFieldDescriptor(context, source, dataConverterAnnotation);
            }

            // Bool / Char / DateTime / Decimal / Guid / String
            else if (effectiveType == typeof(bool)) {
                return dataConverterAnnotation is null ?
                    new BooleanFieldDescriptor(context, source) :
                    new BooleanFieldDescriptor(context, source, dataConverterAnnotation);
            }
            else if (effectiveType == typeof(char)) {
                return dataConverterAnnotation is null ?
                    new CharFieldDescriptor(context, source) :
                    new CharFieldDescriptor(context, source, dataConverterAnnotation);
            }
            else if (effectiveType == typeof(DateTime)) {
                return dataConverterAnnotation is null ?
                    new DateTimeFieldDescriptor(context, source) :
                    new DateTimeFieldDescriptor(context, source, dataConverterAnnotation);
            }
            else if (effectiveType == typeof(decimal)) {
                return dataConverterAnnotation is null ?
                    new DecimalFieldDescriptor(context, source) :
                    new DecimalFieldDescriptor(context, source, dataConverterAnnotation);
            }
            else if (effectiveType == typeof(Guid)) {
                return dataConverterAnnotation is null ?
                    new GuidFieldDescriptor(context, source) :
                    new GuidFieldDescriptor(context, source, dataConverterAnnotation);
            }
            else if (effectiveType == typeof(string)) {
                if (asStringAnnotation is null) {
                    return dataConverterAnnotation is null ?
                        new StringFieldDescriptor(context, source) :
                        new StringFieldDescriptor(context, source, dataConverterAnnotation);
                }
                else {
                    var converter = new EnumToStringConverter(propertyType);
                    return new StringFieldDescriptor(context, source, converter.ConverterImpl);
                }
            }

            // Unsigned Numeric
            else if (effectiveType == typeof(byte) || effectiveType == typeof(ushort) ||
                     effectiveType == typeof(uint) || effectiveType == typeof(ulong)) {

                if (numericAnnotation is null) {
                    return dataConverterAnnotation is null ?
                        new UnsignedFieldDescriptor(context, source) :
                        new UnsignedFieldDescriptor(context, source, dataConverterAnnotation);
                }
                else {
                    var converter = new EnumToNumericConverter(propertyType);
                    return new UnsignedFieldDescriptor(context, source, converter.ConverterImpl);
                }
            }

            // Signed Numeric
            else {
                if (numericAnnotation is null) {
                    return dataConverterAnnotation is null ?
                        new SignedFieldDescriptor(context, source) :
                        new SignedFieldDescriptor(context, source, dataConverterAnnotation);
                }
                else {
                    var converter = new EnumToNumericConverter(propertyType);
                    return new SignedFieldDescriptor(context, source, converter.ConverterImpl);
                }
            }
        }


        private readonly FieldDescriptor field_;
        private string reconstitutionArgumentName_;
    }
}
