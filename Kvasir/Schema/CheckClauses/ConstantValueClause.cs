using Cybele.Extensions;
using Kvasir.Transcription.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Kvasir.Schema.Constraints {
    /// <summary>
    ///   A non-compound <see cref="Clause"/> that expresses a condition on the value of an expression relative to a
    ///   single constant.
    /// </summary>
    public sealed class ConstantValueClause : Clause {
        /// <value>
        ///   The left-hand expression of this <see cref="ConstantValueClause"/>.
        /// </value>
        public FieldExpression LHS { get; }

        /// <value>
        ///   The operator of this <see cref="ConstantValueClause"/>.
        /// </value>
        public ComparisonOperator Operator { get; }

        /// <value>
        ///   The right-hand expression of this <see cref="ConstantValueClause"/>.
        /// </value>
        public DBValue RHS { get; }

        /// <summary>
        ///   Constructs a new <see cref="ConstantValueClause"/>.
        /// </summary>
        /// <param name="lhs">
        ///   The <see cref="LHS">left-hand expression</see> of the new <see cref="ConstantValueClause"/>.
        /// </param>
        /// <param name="op">
        ///   The <see cref="Operator"/> of the new <see cref="ConstantValueClause"/>.
        /// </param>
        /// <param name="rhs">
        ///   The <see cref="RHS">right-hand expression</see> of the new <see cref="ConstantValueClause"/>.
        /// </param>
        /// <pre>
        ///   <paramref name="rhs"/> is not <see cref="DBValue.NULL"/>.
        /// </pre>
        /// <exception cref="ArgumentException">
        ///   if <paramref name="rhs"/> is <see cref="DBType.IsValidValue(DBValue)">not valid</see> for the
        ///   <see cref="FieldExpression.DataType"/> of <paramref name="lhs"/>.
        /// </exception>
        internal ConstantValueClause(FieldExpression lhs, ComparisonOperator op, DBValue rhs) {
            Debug.Assert(op.IsValid());
            Debug.Assert(rhs != DBValue.NULL);

            if (!lhs.DataType.IsValidValue(rhs)) {
                var msg = $"Cannot use constant value {rhs} as the value in a CHECK constraint against an " +
                    $"expression of type {lhs.DataType}";
                throw new ArgumentException(msg);
            }

            LHS = lhs;
            Operator = op;
            RHS = rhs;
        }

        /// <inheritdoc/>
        public sealed override Clause Negation() {
            return new ConstantValueClause(LHS, Operator.Negation(), RHS);
        }

        /// <inheritdoc/>
        internal sealed override IEnumerable<IField> GetDependentFields() {
            return Enumerable.Repeat(LHS.Field, 1);
        }

        /// <inheritdoc/>
        internal sealed override void AddDeclarationTo(IConstraintDeclBuilder builder) {
            builder.AddCheck(this);
        }
    }
}
