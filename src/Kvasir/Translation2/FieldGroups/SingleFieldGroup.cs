using Cybele.Extensions;
using Kvasir.Annotations;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace Kvasir.Translation2 {
    /// <summary>
    ///   The intermediate base class for a <see cref="FieldGroup"/> backed by a scalar or enumeration CLR property,
    ///   and therefore corresponding to exactly one Field.
    /// </summary>
    internal abstract class SingleFieldGroup : FieldGroup {
        /// <inheritdoc/>
        public sealed override int Size => 1;

        /// <summary>
        ///   Constructs a new <see cref="SingleFieldGroup"/>.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which <paramref name="source"/> was accessed via reflection. This context
        ///   should not include that property.
        /// </param>
        /// <param name="source">
        ///   The CLR property backing <paramref name="field"/>.
        /// </param>
        /// <param name="field">
        ///   The single constituent Field of the new <paramref name="field"/>. This must be created by the derived
        ///   class to account for <c>[AsString]</c> and <c>[Numeric]</c> annotations only being applicable to
        ///   enumeration-type Fields.
        /// </param>
        protected SingleFieldGroup(Context context, PropertyInfo source, FieldDescriptor field)
            : base(source) {

            Debug.Assert(context is not null);
            Debug.Assert(field is not null);

            field_ = field;
            ProcessAnnotations(context, this);
        }

        /// <summary>
        ///   Constructs a new <see cref="SingleFieldGroup"/> that is largely identical to another.
        /// </summary>
        /// <param name="source">
        ///   The source <see cref="SingleFieldGroup"/>.
        /// </param>
        /// <seealso cref="FieldGroup.Clone"/>
        protected SingleFieldGroup(SingleFieldGroup source)
            : base(source) {

            field_ = source.field_.Clone();
        }

        /// <inheritdoc/>
        protected sealed override void ApplyConstraint(Context context, Nested<CheckAttribute> annotation) {
            Debug.Assert(context is not null);
            Debug.Assert(annotation.Annotation is not null);

            if (!annotation.AppliesHere) {
                throw new InvalidPathException(context, annotation.Annotation);
            }
            field_.ApplyConstraint(context, annotation.Annotation);
        }

        /// <inheritdoc/>
        protected sealed override void ApplyConstraint(Context context, Nested<Check.ComparisonAttribute> annotation) {
            Debug.Assert(context is not null);
            Debug.Assert(annotation.Annotation is not null);

            if (!annotation.AppliesHere) {
                throw new InvalidPathException(context, annotation.Annotation);
            }
            field_.ApplyConstraint(context, annotation.Annotation);
        }

        /// <inheritdoc/>
        protected sealed override void ApplyConstraint(Context context, Nested<Check.InclusionAttribute> annotation) {
            Debug.Assert(context is not null);
            Debug.Assert(annotation.Annotation is not null);

            if (!annotation.AppliesHere) {
                throw new InvalidPathException(context, annotation.Annotation);
            }
            field_.ApplyConstraint(context, annotation.Annotation);
        }

        /// <inheritdoc/>
        protected sealed override void ApplyConstraint(Context context, Nested<Check.SignednessAttribute> annotation) {
            Debug.Assert(context is not null);
            Debug.Assert(annotation.Annotation is not null);

            if (!annotation.AppliesHere) {
                throw new InvalidPathException(context, annotation.Annotation);
            }
            field_.ApplyConstraint(context, annotation.Annotation);
        }

        /// <inheritdoc/>
        protected sealed override void ApplyConstraint(Context context, Nested<Check.StringLengthAttribute> annotation) {
            Debug.Assert(context is not null);
            Debug.Assert(annotation.Annotation is not null);

            if (!annotation.AppliesHere) {
                throw new InvalidPathException(context, annotation.Annotation);
            }
            field_.ApplyConstraint(context, annotation.Annotation);
        }

        /// <inheritdoc/>
        protected sealed override void SetDefault(Context context, Nested<DefaultAttribute> annotation) {
            Debug.Assert(context is not null);
            Debug.Assert(annotation.Annotation is not null);

            if (!annotation.AppliesHere) {
                throw new InvalidPathException(context, annotation.Annotation);
            }
            field_.SetDefault(context, annotation.Annotation);
        }

        /// <inheritdoc/>
        protected sealed override void SetInCandidateKey(Context context, Nested<UniqueAttribute> annotation) {
            Debug.Assert(context is not null);
            Debug.Assert(annotation.Annotation is not null);

            if (!annotation.AppliesHere) {
                throw new InvalidPathException(context, annotation.Annotation);
            }
            field_.SetInCandidateKey(context, annotation.Annotation);
        }

        /// <inheritdoc/>
        protected sealed override void SetInPrimaryKey(Context context, Nested<PrimaryKeyAttribute> annotation) {
            Debug.Assert(context is not null);
            Debug.Assert(annotation.Annotation is not null);

            if (!annotation.AppliesHere) {
                throw new InvalidPathException(context, annotation.Annotation);
            }
            field_.SetInPrimaryKey(context, annotation.Annotation);
        }

        /// <inheritdoc/>
        protected sealed override void SetName(Context context, Nested<NameAttribute> annotation) {
            Debug.Assert(context is not null);
            Debug.Assert(annotation.Annotation is not null);

            if (!annotation.AppliesHere) {
                throw new InvalidPathException(context, annotation.Annotation);
            }
            field_.SetName(context, annotation.Annotation);
        }

        /// <inheritdoc/>
        protected sealed override void SetNamePrefix(Context context, IReadOnlyList<string> prefix) {
            Debug.Assert(context is not null);
            Debug.Assert(prefix is not null && !prefix.IsEmpty());
            Debug.Assert(prefix.None(s => s is null || s == ""));

            field_.SetNamePrefix(context, prefix);
        }

        /// <inheritdoc/>
        protected sealed override void SetNullability(Context context, bool nullable) {
            Debug.Assert(context is not null);
            field_.SetNullability(context, nullable);
        }


        private readonly FieldDescriptor field_;
    }
}
