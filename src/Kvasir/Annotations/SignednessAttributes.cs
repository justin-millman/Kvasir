using Cybele.Extensions;
using Kvasir.Schema;
using System;
using System.Diagnostics;

namespace Kvasir.Annotations {
    public static partial class Check {
        /// <summary>
        ///   The base class for all annotations that restrict the value for the Field backing a particular numeric-type
        ///   property relative to zero.
        /// </summary>
        public abstract class SignednessAttribute : Attribute {
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
            ///   Constructs a new <see cref="SignednessAttribute"/> instance.
            /// </summary>
            /// <param name="op">
            ///   The <see cref="ComparisonAttribute.Operator"/> for the constraint.
            /// </param>
            private protected SignednessAttribute(ComparisonOperator op) {
                Debug.Assert(op.IsValid());
                Operator = op;
            }
        }

        /// <summary>
        ///   An annotation that specifies that the value for the Field backing a particular property cannot be
        ///   <c>0</c>.
        /// </summary>
        [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
        public sealed class IsNonZeroAttribute : SignednessAttribute {
            /// <summary>
            ///   Constructs a new instance of the <see cref="IsNonZeroAttribute"/> class.
            /// </summary>
            public IsNonZeroAttribute()
                : base(ComparisonOperator.NE) {}
        }

        /// <summary>
        ///   An annotation that specifies that the value for the Field backing a particular property must be positive
        ///   (i.e. <c>&gt;= 0</c>).
        /// </summary>
        [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
        public sealed class IsPositiveAttribute : SignednessAttribute {
            /// <summary>
            ///   Constructs a new instance of the <see cref="IsPositiveAttribute"/> class.
            /// </summary>
            public IsPositiveAttribute()
                : base(ComparisonOperator.GT) {}
        }

        /// <summary>
        ///   An annotation that specifies that the value for the Field backing a particular property must be positive
        ///   (i.e. <c>&lt;= 0</c>).
        /// </summary>
        [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
        public class IsNegativeAttribute : SignednessAttribute {
            /// <summary>
            ///   Constructs a new instance of the <see cref="IsNegativeAttribute"/> class.
            /// </summary>
            public IsNegativeAttribute()
                : base(ComparisonOperator.LT) {}
        }
    }
}
