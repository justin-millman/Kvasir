using Cybele.Core;
using Cybele.Extensions;
using Kvasir.Annotations;
using Kvasir.Extraction;
using Kvasir.Localization;
using Kvasir.Reconstitution;
using Kvasir.Schema;
using Optional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Kvasir.Translation {
    /// <summary>
    ///   A <see cref = "FieldGroup" /> backed by a scalar or enumeration CLR property, and therefore corresponding to
    ///   exactly one Field, that was taken from the key type of a Localization.
    /// </summary>
    internal sealed class LocalizationKeyFieldGroup : FieldGroup {
        /// <inheritdoc/>
        public sealed override int Size => 1;

        /// <inheritdoc/>
        public sealed override bool AllNullable => false;

        /// <inheritdoc/>
        public sealed override bool IsNativelyNullable => false;

        /// <inheritdoc/>
        public sealed override string ReconstitutionArgumentName => impl_.ReconstitutionArgumentName;

        /// <inheritdoc/>
        public sealed override string this[int column] => impl_[column];

        /// <inheritdoc/>
        public sealed override ISingleExtractor Extractor { get; }

        /// <inheritdoc/>
        public sealed override Option<ICreator> Creator { get; protected init; }

        /// <summary>
        ///   The type of the key for the Localization that the <see cref="LocalizationKeyFieldGroup"/> backs.
        /// </summary>
        public Type KeyType { get; }

        /// <summary>
        ///   Constructs a new <see cref="LocalizationKeyFieldGroup"/>.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which <paramref name="source"/> was accessed via reflection. This context
        ///   should include that property.
        /// </param>
        /// <param name="source">
        ///   The CLR property backing the new <see cref="LocalizationKeyFieldGroup"/>.
        /// </param>
        /// <exception cref="InvalidLocalizationKeyException">
        ///   if the <see cref="KeyType"/> of the Localization represented by <paramref name="source"/> is not a type
        ///   natively supported by Kvasir.
        /// </exception>
        public LocalizationKeyFieldGroup(Context context, PropertyInfo source)
            : base(source) {

            var metadata = LocalizationHelper.Reflect(source.PropertyType);
            KeyType = metadata.KeyType;
            if (!DBType.IsSupported(KeyType)) {
                context.Push(source.PropertyType);
                context.Push(metadata.KeyProperty);
                var category = KeyType.TranslationCategory();
                throw new InvalidLocalizationKeyException(context, KeyType, category);
            }

            // We use the `Key` property for the underlying SingleFieldGroup because this gives us the correct type and
            // contributes no native annotations. All annotations on the Localization property itself will be forwarded
            // as appropriate by the LocalizationFieldGroup.
            impl_ = new SingleFieldGroup(context, metadata.KeyProperty);
            ProcessAnnotations(context);

            if (impl_.IsNativelyNullable) {
                context.Push(source.PropertyType);
                context.Push(metadata.KeyProperty);
                throw new InvalidNativeNullabilityException(context, "the Localization Key type of a Localization");
            }

            Extractor = new ReadPropertyExtractor(new PropertyChain(source).Append(metadata.KeyProperty));
            var reconstitutionGroup = new SingleFieldGroup(context, metadata.KeyProperty);
            reconstitutionGroup.SetColumn(context, 0);
            var groups = Enumerable.Repeat(reconstitutionGroup, 1);
            Creator = Option.Some<ICreator>(ReconstitutionHelper.MakeCreator(context, Source.PropertyType, groups, false));
        }

        /// <summary>
        ///   Constructs a new <see cref="LocalizationKeyFieldGroup"/> that is largely identical to another.
        /// </summary>
        /// <param name="source">
        ///   The source <see cref="LocalizationKeyFieldGroup"/>.
        /// </param>
        /// <seealso cref="FieldGroup.Clone"/>
        private LocalizationKeyFieldGroup(LocalizationKeyFieldGroup source)
            : base(source) {

            impl_ = source.impl_.Clone();
            nameAnnotated_ = source.nameAnnotated_;
            annotatedNonNullable_ = source.annotatedNonNullable_;
            Extractor = source.Extractor;
            Creator = source.Creator;
            KeyType = source.KeyType;
        }

        /// <summary>
        ///   Constructs a new <see cref="LocalizationKeyFieldGroup"/> that is largely identical to another, but with the
        ///   constituent Field reset.
        /// </summary>
        /// <param name="source">
        ///   The source <see cref="LocalizationKeyFieldGroup"/>.
        /// </param>
        /// <param name="_">
        ///   <i>overload discriminator</i>
        /// </param>
        /// <seealso cref="Reset"/>
        private LocalizationKeyFieldGroup(LocalizationKeyFieldGroup source, ResetTag _)
            : base(source) {

            impl_ = source.impl_.Reset();
            nameAnnotated_ = false;
            annotatedNonNullable_ = false;
            KeyType = source.KeyType;
            Extractor = source.Extractor;
            Creator = source.Creator;
        }

        /// <inheritdoc/>
        public sealed override void ApplyConstraint(Context context, Nested<CheckAttribute> annotation) {
            impl_.ApplyConstraint(context, annotation);
        }

        /// <inheritdoc/>
        public sealed override void ApplyConstraint(Context context, Nested<Check.ComparisonAttribute> annotation) {
            impl_.ApplyConstraint(context, annotation);
        }

        /// <inheritdoc/>
        public sealed override void ApplyConstraint(Context context, Nested<Check.InclusionAttribute> annotation) {
            impl_.ApplyConstraint(context, annotation);
        }

        /// <inheritdoc/>
        public sealed override void ApplyConstraint(Context context, Nested<Check.SignednessAttribute> annotation) {
            impl_.ApplyConstraint(context, annotation);
        }

        /// <inheritdoc/>
        public sealed override void ApplyConstraint(Context context, Nested<Check.StringLengthAttribute> annotation) {
            impl_.ApplyConstraint(context, annotation);
        }

        /// <inheritdoc/>
        public sealed override LocalizationKeyFieldGroup Clone() {
            return new LocalizationKeyFieldGroup(this);
        }

        /// <inheritdoc/>
        public sealed override Option<FieldGroup> Filter(IEnumerable<FieldDescriptor> constituents) {
            return impl_.Filter(constituents).Map<FieldGroup>(_ => this);
        }

        /// <inheritdoc/>
        public sealed override IEnumerator<FieldDescriptor> GetEnumerator() {
            return impl_.GetEnumerator();
        }

        /// <inheritdoc/>
        public sealed override IEnumerable<ReferenceFieldGroup> References() {
            return impl_.References();
        }

        /// <inheritdoc/>
        public sealed override LocalizationKeyFieldGroup Reset() {
            return new LocalizationKeyFieldGroup(this, RESET);
        }

        /// <inheritdoc/>
        public sealed override void SetDefault(Context context, Nested<DefaultAttribute> annotation) {
            impl_.SetDefault(context, annotation);
        }

        /// <inheritdoc/>
        public sealed override void SetInCandidateKey(Context context, Nested<UniqueAttribute> annotation) {
            impl_.SetInCandidateKey(context, annotation);
        }

        /// <inheritdoc/>
        public sealed override void SetInPrimaryKey(Context context, Nested<PrimaryKeyAttribute> annotation, string cascadePath) {
            impl_.SetInPrimaryKey(context, annotation, cascadePath);
        }

        /// <inheritdoc/>
        public sealed override void SetName(Context context, Nested<NameAttribute> annotation) {
            impl_.SetName(context, annotation);
            nameAnnotated_ = true;
        }

        /// <inheritdoc/>
        public sealed override void SetNamePrefix(Context context, IEnumerable<string> prefix) {
            impl_.SetNamePrefix(context, prefix);
        }

        /// <inheritdoc/>
        public sealed override void SetNullability(Context context, bool nullable) {
            if (nullable) {
                throw new InapplicableAnnotationException(context, typeof(NullableAttribute), Source.PropertyType, MultiKind.Localization);
            }
            else {
                annotatedNonNullable_ = true;
            }
        }

        /// <inheritdoc/>
        protected sealed override void ProcessNativeNullability(Context context) {
            bool nativelyNullable = new NullabilityInfoContext().Create(Source).ReadState == NullabilityState.Nullable;
            if (!annotatedNonNullable_ && nativelyNullable) {
                throw new InvalidNativeNullabilityException(context, "a property of Localization type");
            }
        }

        /// <inheritdoc/>
        protected sealed override void ProcessAnnotations(Context context) {
            base.ProcessAnnotations(context);

            // Data Converters are not allowed on Localization properties because it would break the inherent Foreign
            // Key relation. The data for the Localization will be stored in a single Table shared by all instances of
            // that Localization, so the data in the individual Entity Tables must match exactly - there's no way to
            // change the data type for just some data but not others.
            if (Source.HasAttribute<DataConverterAttribute>()) {
                var type = typeof(DataConverterAttribute);
                throw new InapplicableAnnotationException(context, type, Source.PropertyType, MultiKind.Localization);
            }
            else if (Source.HasAttribute<NumericAttribute>()) {
                var type = typeof(NumericAttribute);
                throw new InapplicableAnnotationException(context, type, Source.PropertyType, MultiKind.Localization);
            }
            else if (Source.HasAttribute<AsStringAttribute>()) {
                var type = typeof(AsStringAttribute);
                throw new InapplicableAnnotationException(context, type, Source.PropertyType, MultiKind.Localization);
            }

            // We use the `Key` property of the Localization to create the underlying SingleFieldGroup, since this gets
            // us the correct property type and has no inherent annotations. However, the name implied by this property
            // is just "Key" and that isn't right: if the Localization property is not itself annotated with a [Name]
            // attribute, then it should have the name of the property itself.
            if (!nameAnnotated_) {
                var name = new NameAttribute(Source.Name);
                impl_.SetName(context, name);
            }
        }


        private readonly SingleFieldGroup impl_;
        private bool nameAnnotated_;
        private bool annotatedNonNullable_;
    }
}
