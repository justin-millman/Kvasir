using Cybele.Extensions;
using Kvasir.Transcription;
using System.Collections.Generic;
using System.Diagnostics;

namespace Kvasir.Schema {
    /// <summary>
    ///   A simple <see cref="Clause"/> that evaluates the value of an expression on the value of a Field relative to
    ///   another such expression.
    /// </summary>
    public sealed record CrossFieldClause : Clause {
        /// <summary>
        ///   The left-hand operand of this <see cref="CrossFieldClause"/>, which is an expression on the value of a
        ///   Field.
        /// </summary>
        public FieldExpression LHS { get; }

        /// <summary>
        ///   The operator of this <see cref="ConstantClause"/>.
        /// </summary>
        public ComparisonOperator Operator { get; }

        /// <summary>
        ///   The right-hand operand of this <see cref="CrossFieldClause"/>, which is an expression on the value of a
        ///   Field.
        /// </summary>
        public FieldExpression RHS { get; }

        /// <summary>
        ///   Constructs a new <see cref="CrossFieldClause"/>.
        /// </summary>
        /// <param name="lhs">
        ///   The <see cref="LHS">left-hand operand</see> of the new <see cref="CrossFieldClause"/>.
        /// </param>
        /// <param name="op">
        ///   The <see cref="Operator">operator</see> of the new <see cref="CrossFieldClause"/>.
        /// </param>
        /// <param name="rhs">
        ///   The <see cref="RHS">right-hand operand</see> of the new <see cref="CrossFieldClause"/>.
        /// </param>
        /// <pre>
        ///   <paramref name="lhs"/> is not <see langword="null"/>
        ///     --and--
        ///   <paramref name="rhs"/> is not <see langword="null"/>
        ///     --and--
        ///   the <see cref="FieldExpression.DataType">data type</see> of <paramref name="lhs"/> is the same as that of
        ///   <paramref name="rhs"/>.
        /// </pre>
        internal CrossFieldClause(FieldExpression lhs, ComparisonOperator op, FieldExpression rhs) {
            Debug.Assert(lhs is not null);
            Debug.Assert(op.IsValid());
            Debug.Assert(rhs is not null);
            Debug.Assert(lhs.DataType == rhs.DataType);

            LHS = lhs;
            Operator = op;
            RHS = rhs;
        }

        /* Because CrossFieldClause is record type, the following methods are synthesized automatically by the
         * compiler:
         *   > public CrossFieldClause(CrossFieldClause rhs)
         *   > public bool Equals(CrossFieldClause? rhs)
         *   > public sealed override bool Equals(object? rhs)
         *   > public sealed override int GetHashCode()
         *   > public sealed override string ToString()
         *   > public static bool operator==(CrossFieldClause? lhs, CrossFieldClause? rhs)
         *   > public static bool operator!=(CrossFieldClause? lhs, CrossFieldClause? rhs)
         */

        /// <inheritdoc/>
        public sealed override Clause Negation() {
            return new CrossFieldClause(LHS, Operator.Negation(), RHS);
        }

        /// <inheritdoc/>
        public sealed override IEnumerable<IField> GetDependentFields() {
            yield return LHS.Field;
            yield return RHS.Field;
        }

        /// <inheritdoc/>
        internal sealed override void AddDeclarationTo<TDecl>(IConstraintDeclBuilder<TDecl> builder) {
            Debug.Assert(builder is not null);
            builder.AddClause(this);
        }
    }
}
