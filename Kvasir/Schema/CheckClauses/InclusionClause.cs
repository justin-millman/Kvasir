using Cybele.Extensions;
using Kvasir.Transcription.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Kvasir.Schema.Constraints {
    /// <summary>
    ///   A non-compound <see cref="Clause"/> that expresses a condition on the value of an expression relative to a
    ///   list of constants.
    /// </summary>
    public sealed class InclusionClause : Clause {
        /// <value>
        ///   The left-hand expression of this <see cref="InclusionClause"/>.
        /// </value>
        public FieldExpression LHS { get; }

        /// <value>
        ///   The operator of this <see cref="InclusionClause"/>.
        /// </value>
        public InclusionOperator Operator { get; }

        /// <value>
        ///   The right-hand expression of this <see cref="InclusionClause"/>.
        /// </value>
        public IEnumerable<DBValue> RHS { get; }

        /// <summary>
        ///   Constructs a new <see cref="InclusionClause"/>.
        /// </summary>
        /// <param name="lhs">
        ///   The <see cref="LHS">left-hand expression</see> of the new <see cref="InclusionClause"/>.
        /// </param>
        /// <param name="op">
        ///   The <see cref="Operator"/> of the new <see cref="InclusionClause"/>.
        /// </param>
        /// <param name="rhs">
        ///   The <see cref="RHS">right-hand expression</see> of the new <see cref="InclusionClause"/>.
        /// </param>
        /// <pre>
        ///   <paramref name="rhs"/> is not empty
        ///     --and--
        ///   None of the <see cref="DBValue">DBValues</see> in <paramref name="rhs"/> is <see cref="DBValue.NULL"/>.
        /// </pre>
        /// <exception cref="ArgumentException">
        ///   if any of the <see cref="DBValue">DBValues</see> in <paramref name="rhs"/> is
        ///   <see cref="DBType.IsValidValue(DBValue)">not valid</see> for the <see cref="FieldExpression.DataType"/>
        ///   of <paramref name="lhs"/>.
        /// </exception>
        internal InclusionClause(FieldExpression lhs, InclusionOperator op, IEnumerable<DBValue> rhs) {
            Debug.Assert(op.IsValid());
            Debug.Assert(rhs.Any());
            Debug.Assert(!rhs.Any(v => v == DBValue.NULL));

            if (rhs.Any(v => !lhs.DataType.IsValidValue(v))) {
                var msg = "All values in an inclusion constraint must be compatible with the type of the left-hand " +
                    "expression";
                throw new ArgumentException(msg);
            }

            LHS = lhs;
            Operator = op;
            RHS = rhs;
        }

        /// <inheritdoc/>
        public sealed override Clause Negation() {
            if (Operator == InclusionOperator.In) {
                return new InclusionClause(LHS, InclusionOperator.NotIn, RHS);
            }
            else {
                return new InclusionClause(LHS, InclusionOperator.In, RHS);
            }
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
