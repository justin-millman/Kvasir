using Ardalis.GuardClauses;
using Kvasir.Core;
using Kvasir.Schema;
using System;
using System.Diagnostics;
using System.Linq;

namespace Kvasir.Annotations {
    public static partial class Check {
        /// <summary>
        ///   An annotation that specifies that the value for the Field backing a particular string-type property must
        ///   be non-empty.
        /// </summary>
        [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
        public class IsNonEmptyAttribute : LengthIsAtLeastAttribute {
            /// <summary>
            ///   Constructs a new instance of the <see cref="IsNonEmptyAttribute"/> class.
            /// </summary>
            public IsNonEmptyAttribute()
                : base(1) {}
        }

        /// <summary>
        ///   An annotation that specifies that the value for the Field backing a particular string-type property must
        ///   be at least a certain length.
        /// </summary>///
        [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
        public class LengthIsAtLeastAttribute : ConstraintAttribute {
            /// <summary>
            ///   Constructs a new instance of the <see cref="LengthIsAtLeastAttribute"/> class.
            /// </summary>
            /// <param name="lowerBound">
            ///   The length (<i>inclusive</i>) that the Field backing the annotated property must be no shorter than.
            /// </param>
            public LengthIsAtLeastAttribute(int lowerBound)
                : base(FieldFunction.LengthOf, ComparisonOperator.GTE, lowerBound) {}
        }

        /// <summary>
        ///   An annotation that specifies that the value for the Field backing a particular string-type property can
        ///   be at most a certain length.
        /// </summary>
        [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
        public class LengthIsAtMostAttribute : ConstraintAttribute {
            /// <summary>
            ///   Constructs a new instance of the <see cref="LengthIsAtMostAttribute"/> class.
            /// </summary>
            /// <param name="upperBound">
            ///   The length (<i>inclusive</i>) that the Field backing the annotated property must be no shorter than.
            /// </param>
            public LengthIsAtMostAttribute(int upperBound)
                : base(FieldFunction.LengthOf, ComparisonOperator.LTE, upperBound) {}
        }

        /// <summary>
        ///   An annotation that specifies that the value for the Field backing a particular string-type property must
        ///   have a length within a certain range.
        /// </summary>
        [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
        public class LengthIsBetweenAttribute : CheckAttribute {
            /// <summary>
            ///   Constructs a new instance of the <see cref="LengthIsBetweenAttribute"/> class.
            /// </summary>
            /// <param name="lowerBound">
            ///   The length (<i>inclusive</i>) that the Field backing the annotated property must be no shorter than.
            /// </param>
            /// <param name="upperBound">
            ///   The length (<i>inclusive</i>) that the Field backing the annotated property must be no shorter than.
            /// </param>
            public LengthIsBetweenAttribute(int lowerBound, int upperBound)
                : base(typeof(Constraint), lowerBound, upperBound) {}


            private struct Constraint : IConstraintGenerator {
                public Constraint(int lowerBound, int upperBound) {
                    lower_ = new DBValue(lowerBound);
                    upper_ = new DBValue(upperBound);
                }
                public Clause MakeConstraint(FieldSeq fields, ConverterSeq converters, Settings settings) {
                    Guard.Against.Null(fields, nameof(fields));
                    Guard.Against.InvalidInput(fields, nameof(fields), f => f.Count() == 1);
                    Debug.Assert(converters is not null);
                    Debug.Assert(settings is not null);

                    var expr = new FieldExpression(FieldFunction.LengthOf, fields.First()!);
                    var lower = new ConstantClause(expr, ComparisonOperator.GTE, lower_);
                    var upper = new ConstantClause(expr, ComparisonOperator.LTE, upper_);
                    return lower.And(upper);
                }


                private readonly DBValue lower_;
                private readonly DBValue upper_;
            }
        }
    }
}
