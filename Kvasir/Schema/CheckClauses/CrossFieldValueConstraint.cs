using Cybele.Extensions;
using Kvasir.Transcription.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Kvasir.Schema.Constraints {
    /// <summary>
    ///   A non-compound <see cref="Clause"/> that expresses a condition on the value of an expression relative to a
    ///   the value of another expression.
    /// </summary>
    public sealed class CrossFieldValueConstraint : Clause {
        /// <value>
        ///   The left-hand expression of this <see cref="CrossFieldValueConstraint"/>.
        /// </value>
        public FieldExpression LHS { get; }

        /// <value>
        ///   The operator of this <see cref="CrossFieldValueConstraint"/>.
        /// </value>
        public ComparisonOperator Operator { get; }

        /// <value>
        ///   The right-hand expression of this <see cref="CrossFieldValueConstraint"/>.
        /// </value>
        public FieldExpression RHS { get; }

        /// <summary>
        ///   Constructs a new <see cref="CrossFieldValueConstraint"/>.
        /// </summary>
        /// <param name="lhs">
        ///   The <see cref="LHS">left-hand expression</see> of the new <see cref="CrossFieldValueConstraint"/>.
        /// </param>
        /// <param name="op">
        ///   The <see cref="Operator"/> of the new <see cref="CrossFieldValueConstraint"/>.
        /// </param>
        /// <param name="rhs">
        ///   The <see cref="RHS">right-hand expression</see> of the new <see cref="CrossFieldValueConstraint"/>.
        /// </param>
        /// <exception cref="ArgumentException">
        ///   if the <see cref="FieldExpression.DataType"/> of <paramref name="lhs"/> and <paramref name="rhs"/> are
        ///   not equal.
        /// </exception>
        internal CrossFieldValueConstraint(FieldExpression lhs, ComparisonOperator op, FieldExpression rhs) {
            Debug.Assert(op.IsValid());

            if (lhs.DataType != rhs.DataType) {
                var msg = "The data types of the two Fields in a cross-Field value constraint must be equal";
                throw new ArgumentException(msg);
            }

            LHS = lhs;
            Operator = op;
            RHS = rhs;
        }

        /// <inheritdoc/>
        public sealed override Clause Negation() {
            return new CrossFieldValueConstraint(LHS, Operator.Negation(), RHS);
        }

        /// <inheritdoc/>
        internal sealed override IEnumerable<IField> GetDependentFields() {
            return Enumerable.Repeat(LHS.Field, 1).Append(RHS.Field);
        }

        /// <inheritdoc/>
        internal sealed override void AddDeclarationTo(IConstraintDeclBuilder builder) {
            builder.AddCheck(this);
        }
    }
}
