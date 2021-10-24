using Kvasir.Schema;
using System;

namespace Kvasir.Annotations {
    public static partial class Check {
        /// <summary>
        ///   An annotation that specifies that the value for the Field backing a particular property cannot be
        ///   <c>0</c>.
        /// </summary>
        [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
        public class IsNonZeroAttribute : ConstraintAttribute {
            /// <summary>
            ///   Constructs a new instance of the <see cref="IsNonZeroAttribute"/> class.
            /// </summary>
            public IsNonZeroAttribute()
                : base(ComparisonOperator.NE, 0) {}
        }

        /// <summary>
        ///   An annotation that specifies that the value for the Field backing a particular property must be positive
        ///   (i.e. <c>&gt;= 0</c>).
        /// </summary>
        [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
        public class IsPositiveAttribute : ConstraintAttribute {
            /// <summary>
            ///   Constructs a new instance of the <see cref="IsPositiveAttribute"/> class.
            /// </summary>
            public IsPositiveAttribute()
                : base(ComparisonOperator.GT, 0) {}
        }

        /// <summary>
        ///   An annotation that specifies that the value for the Field backing a particular property must be positive
        ///   (i.e. <c>&lt;= 0</c>).
        /// </summary>
        [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
        public class IsNegativeAttribute : ConstraintAttribute {
            /// <summary>
            ///   Constructs a new instance of the <see cref="IsNegativeAttribute"/> class.
            /// </summary>
            public IsNegativeAttribute()
                : base(ComparisonOperator.LT, 0) {}
        }
    }
}
