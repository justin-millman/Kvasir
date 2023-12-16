using Cybele.Extensions;
using Kvasir.Schema;
using System;
using System.Diagnostics;

namespace Kvasir.Annotations {
    public static partial class Check {
        /// <summary>
        ///   The base class for all annotations that restrict the value for the Field backing a particular property
        ///   relative to a single anchor point.
        /// </summary>
        public abstract class ComparisonAttribute : Attribute, INestableAnnotation {
            /// <inheritdoc/>
            public string Path { get; init; } = "";

            /// <summary>
            ///   The operator of the comparison restriction.
            /// </summary>
            internal ComparisonOperator Operator { get; private init; }

            /// <summary>
            ///   The anchor of the comparison restriction.
            /// </summary>
            /// <remarks>
            ///   This value is taken directly from the user, and no compatibility checks are (or, really, can be)
            ///   performed. This means, for example, that the anchor <i>might</i> be an array of values, even though
            ///   that is incongruous with the purpose of a <see cref="ComparisonAttribute"/>. The one exception is that
            ///   a literal user-provided <see langword="null"/> will be converted into <see cref="DBNull"/>, though
            ///   that will also induce an error during Translation.
            /// </remarks>
            internal object Anchor { get; private init; }

            /// <summary>
            ///   Constructs a new <see cref="ComparisonAttribute"/> instance.
            /// </summary>
            /// <param name="op">
            ///   The <see cref="Operator"/> for the constraint.
            /// </param>
            /// <param name="anchor">
            ///   The <see cref="Anchor"/> for the constraint.
            /// </param>
            private protected ComparisonAttribute(ComparisonOperator op, object? anchor) {
                Debug.Assert(op.IsValid());

                Operator = op;
                Anchor = anchor ?? DBNull.Value;
            }

            /// <summary>
            ///   Creates an exact copy of a <see cref="ComparisonAttribute"/>, but with a different <see cref="Path"/>.
            /// </summary>
            /// <param name="path">
            ///   The new <see cref="Path"/>.
            /// </param>
            /// <returns>
            ///   A <see cref="ComparisonAttribute"/> of the same most-derived type as <c>this</c>, whose
            ///   <see cref="Path"/> attribute is exactly <paramref name="path"/>.
            /// </returns>
            private protected abstract ComparisonAttribute WithPath(string path);

            /// <inheritdoc/>
            INestableAnnotation INestableAnnotation.WithPath(string path) {
                return WithPath(path);
            }
        }

        /// <summary>
        ///   An annotation that specifies that the value for the Field backing a particular property cannot have a
        ///   specific value.
        /// </summary>
        /// <seealso cref="NonNullableAttribute"/>
        [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
        public sealed class IsNotAttribute : ComparisonAttribute {
            /// <summary>
            ///   Constructs a new instance of the <see cref="IsNotAttribute"/> class.
            /// </summary>
            /// <param name="value">
            ///   The value that the Field backing the annotated property cannot take.
            /// </param>
            public IsNotAttribute(object value)
                : base(ComparisonOperator.NE, value) {}

            /// <inheritdoc/>
            private protected sealed override ComparisonAttribute WithPath(string path) {
                return new IsNotAttribute(Anchor) { Path = path };
            }
        }

        /// <summary>
        ///   An annotation that specifies that the value for the Field backing a particular property must be strictly
        ///   greater than a specific value.
        /// </summary>
        [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
        public sealed class IsGreaterThanAttribute : ComparisonAttribute {
            /// <summary>
            ///   Constructs a new instance of the <see cref="IsGreaterThanAttribute"/> class.
            /// </summary>
            /// <param name="lowerBound">
            ///   The value that the Field backing the annotated property must be strictly greater than.
            /// </param>
            public IsGreaterThanAttribute(object lowerBound)
                : base(ComparisonOperator.GT, lowerBound) {}

            /// <inheritdoc/>
            private protected sealed override ComparisonAttribute WithPath(string path) {
                return new IsGreaterThanAttribute(Anchor) { Path = path };
            }
        }

        /// <summary>
        ///   An annotation that specifies that the value for the Field backing a particular property must be strictly
        ///   less than a specific value.
        /// </summary>
        [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
        public sealed class IsLessThanAttribute : ComparisonAttribute {
            /// <summary>
            ///   Constructs a new instance of the <see cref="IsLessThanAttribute"/> class.
            /// </summary>
            /// <param name="upperBound">
            ///   The value that the Field backing the annotated property must be strictly less than.
            /// </param>
            public IsLessThanAttribute(object upperBound)
                : base(ComparisonOperator.LT, upperBound) {}

            /// <inheritdoc/>
            private protected sealed override ComparisonAttribute WithPath(string path) {
                return new IsLessThanAttribute(Anchor) { Path = path };
            }
        }

        /// <summary>
        ///   An annotation that specifies that the value for the Field backing a particular property must be greater
        ///   than or equal to a specific value.
        /// </summary>
        [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
        public sealed class IsGreaterOrEqualToAttribute : ComparisonAttribute {
            /// <summary>
            ///   Constructs a new instance of the <see cref="IsGreaterOrEqualToAttribute"/> class.
            /// </summary>
            /// <param name="lowerBound">
            ///   The value that the Field backing the annotated property must be greater than or equal to.
            /// </param>
            public IsGreaterOrEqualToAttribute(object lowerBound)
                : base(ComparisonOperator.GTE, lowerBound) {}

            /// <inheritdoc/>
            private protected sealed override ComparisonAttribute WithPath(string path) {
                return new IsGreaterOrEqualToAttribute(Anchor) { Path = path };
            }
        }

        /// <summary>
        ///   An annotation that specifies that the value for the Field backing a particular property must be less
        ///   than or equal to a specific value.
        /// </summary>
        [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
        public sealed class IsLessOrEqualToAttribute : ComparisonAttribute {
            /// <summary>
            ///   Constructs a new instance of the <see cref="IsLessOrEqualToAttribute"/> class.
            /// </summary>
            /// <param name="upperBound">
            ///   The value that the Field backing the annotated property must be less than or equal to.
            /// </param>
            public IsLessOrEqualToAttribute(object upperBound)
                : base(ComparisonOperator.LTE, upperBound) {}

            /// <inheritdoc/>
            private protected sealed override ComparisonAttribute WithPath(string path) {
                return new IsLessOrEqualToAttribute(Anchor) { Path = path };
            }
        }
    }
}
