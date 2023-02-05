using Kvasir.Schema;
using System;

namespace Kvasir.Annotations {
    /// <summary>
    ///   A standalone type that represents the notion of "zero" without being locked into a specific type.
    /// </summary>
    /// <remarks>
    ///   The signedness annotations are all effectively comparison annotations against a singular anchor point of
    ///   zero. However, the Translation Layer will require that all anchors match the annotation property's type
    ///   exactly, which we cannot do at this point: we don't know what that type should be. So we use the Zero sentinel
    ///   as a signal, and let the Translation Layer handle it appropriately.
    /// </remarks>
    internal struct Zero {}

    public static partial class Check {
        /// <summary>
        ///   An annotation that specifies that the value for the Field backing a particular property cannot be
        ///   <c>0</c>.
        /// </summary>
        [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
        public sealed class IsNonZeroAttribute : ComparisonAttribute {
            /// <summary>
            ///   Constructs a new instance of the <see cref="IsNonZeroAttribute"/> class.
            /// </summary>
            public IsNonZeroAttribute()
                : base(ComparisonOperator.NE, new Zero()) {}
        }

        /// <summary>
        ///   An annotation that specifies that the value for the Field backing a particular property must be positive
        ///   (i.e. <c>&gt;= 0</c>).
        /// </summary>
        [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
        public sealed class IsPositiveAttribute : ComparisonAttribute {
            /// <summary>
            ///   Constructs a new instance of the <see cref="IsPositiveAttribute"/> class.
            /// </summary>
            public IsPositiveAttribute()
                : base(ComparisonOperator.GT, new Zero()) {}
        }

        /// <summary>
        ///   An annotation that specifies that the value for the Field backing a particular property must be positive
        ///   (i.e. <c>&lt;= 0</c>).
        /// </summary>
        [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
        public class IsNegativeAttribute : ComparisonAttribute {
            /// <summary>
            ///   Constructs a new instance of the <see cref="IsNegativeAttribute"/> class.
            /// </summary>
            public IsNegativeAttribute()
                : base(ComparisonOperator.LT, new Zero()) {}
        }
    }
}
