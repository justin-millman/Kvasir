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
        public abstract class ComparisonAttribute : Attribute {
            /// <summary>
            ///   The dot-separated path, relative to the property on which the annotation is placed, to the property to
            ///   which the annotation actually applies.
            /// </summary>
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
        }
    }
}
