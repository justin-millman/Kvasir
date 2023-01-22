using Ardalis.GuardClauses;
using Kvasir.Schema;
using System;

namespace Kvasir.Annotations {
    public static partial class Check {
        /// <summary>
        ///   An annotation that specifies that the value for the Field backing a particular property must be one of a
        ///   discrete set of options.
        /// </summary>
        [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
        public class IsOneOfAttribute : ConstraintAttribute {
            /// <summary>
            ///   Construct a new instance of the <see cref="IsOneOfAttribute"/> class.
            /// </summary>
            /// <param name="values">
            ///   The discrete set of options, of which the value that the Field backing the annotated property must be
            ///   one.
            /// </param>
            public IsOneOfAttribute(params object?[] values)
                : base(InclusionOperator.In, values) {

                Guard.Against.NullOrEmpty(values, nameof(values));
            }
        }

        /// <summary>
        ///   An annotation that specifies that the value for the Field backing a particular property cannot be one of
        ///   a discrete set of options
        /// </summary>
        [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
        public class IsNotOneOfAttribute : ConstraintAttribute {
            /// <summary>
            ///   Construct a new instance of the <see cref="IsNotOneOfAttribute"/> class.
            /// </summary>
            /// <param name="values">
            ///   The discrete set of options, of which the value that the Field backing the annotated property cannot
            ///   be one.
            /// </param>
            public IsNotOneOfAttribute(params object?[] values)
                : base(InclusionOperator.NotIn, values) {}
        }
    }
}
