using Cybele.Core;
using Cybele.Extensions;
using Kvasir.Annotations;
using Kvasir.Extraction;
using Kvasir.Reconstitution;
using Optional;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Kvasir.Translation {
    /// <summary>
    ///   A <see cref="FieldGroup"/> backed by a scalar or enumeration CLR property, and therefore corresponding to
    ///   exactly one Field, which was taken from the Key Type of a Localization
    /// </summary>
    internal sealed class LocalizationKeyFieldGroup : FieldGroup {
        /// <inheritdoc/>
        public sealed override int Size => 1;

        /// <inheritdoc/>
        public sealed override bool AllNullable => impl_.AllNullable;

        /// <inheritdoc/>
        public sealed override bool IsNativelyNullable { get; }

        /// <inheritdoc/>
        public sealed override string ReconstitutionArgumentName => impl_.ReconstitutionArgumentName;

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
        ///   Constructs a new <see cref="LocalizationKeyFieldGroup"/>.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which <paramref name="source"/> was accessed via reflection. This context
        ///   should include that property.
        /// </param>
        /// <param name="source">
        ///   The CLR property backing the new <see cref="LocalizationKeyFieldGroup"/>.
        /// </param>
        public LocalizationKeyFieldGroup(Context context, PropertyInfo source)
            : base(source) {

            var metadata = LocalizationHelper.GetMetadataFor(source.PropertyType);
            {
                using var guard = context.Push(source.PropertyType);
                LocalizationHelper.ErrorCheck(metadata, context);
            }
            impl_ = new SingleFieldGroup(context, metadata.RepresentativeProperty);
            isNameAnnotated_ = false;
            isAnnotatedNonNullable_ = false;
            IsNativelyNullable = (new NullabilityInfoContext().Create(Source).ReadState == NullabilityState.Nullable);

            ProcessAnnotations(context);

            Extractor = new ReadPropertyExtractor(new PropertyChain(source).Append(metadata.RepresentativeProperty));

            var reconstitutionGroup = new SingleFieldGroup(context, metadata.RepresentativeProperty);
            reconstitutionGroup.SetColumn(context, 0);
            var groups = Enumerable.Repeat(reconstitutionGroup, 1);
            Creator = Option.Some<ICreator>(ReconstitutionHelper.MakeCreator(context, source.PropertyType, groups, false));
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
            isNameAnnotated_ = source.isNameAnnotated_;
            isAnnotatedNonNullable_ = source.isAnnotatedNonNullable_;
            Extractor = source.Extractor;
            Creator = source.Creator;
            IsNativelyNullable = source.IsNativelyNullable;
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
            isNameAnnotated_ = false;
            isAnnotatedNonNullable_ = source.isAnnotatedNonNullable_;
            Extractor = source.Extractor;
            Creator = source.Creator;
            IsNativelyNullable = source.IsNativelyNullable;
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

            if (annotation.AppliesHere) {
                isNameAnnotated_ = true;
            }
        }

        /// <inheritdoc/>
        public sealed override void SetNamePrefix(Context context, IEnumerable<string> prefix) {
            impl_.SetNamePrefix(context, prefix);
        }

        /// <inheritdoc/>
        public sealed override void SetNullability(Context context, bool nullable) {
            impl_.SetNullability(context, nullable);
            isAnnotatedNonNullable_ = (nullable == false);
        }

        /// <inheritdoc/>
        protected sealed override void ProcessNativeNullability(Context context) {
            if (IsNativelyNullable && !isAnnotatedNonNullable_) {
                SetNullability(context, true);
            }
        }

        /// <inheritdoc/>
        protected sealed override void ProcessAnnotations(Context context) {
            base.ProcessAnnotations(context);

            // Data Converters are not allowed on Localization properties because they are treated as-if they were
            // References, and applying any data conversions would break the implicit Foreign Key relationship. If the
            // Key Type of the Localization is an enumeration, there may be an automatic intrinsic conversion to string
            // for storage, but not even the [Numeric] or [AsString] are allowed.
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
            if (!isNameAnnotated_) {
                var name = new NameAttribute(Source.Name);
                impl_.SetName(context, name);
            }
        }


        private readonly SingleFieldGroup impl_;
        private bool isNameAnnotated_;
        private bool isAnnotatedNonNullable_;
    }
}