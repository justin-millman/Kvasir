using Kvasir.Schema;
using System;

namespace Kvasir.Annotations {
    public static partial class Check {
        /// <summary>
        ///   An annotation that specifies that the value for the Field backing a particular property cannot have a
        ///   specific value.
        /// </summary>
        /// <seealso cref="NonNullableAttribute"/>
        [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
        public class IsNotAttribute : ConstraintAttribute {
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
        public class IsGreaterThanAttribute : ConstraintAttribute {
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
        public class IsLessThanAttribute : ConstraintAttribute {
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
        public class IsGreaterOrEqualToAttribute : ConstraintAttribute {
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
        public class IsLessOrEqualToAttribute : ConstraintAttribute {
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
