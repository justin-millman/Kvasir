using Cybele.Extensions;
using Kvasir.Transcription;
using System.Collections.Generic;
using System.Diagnostics;

namespace Kvasir.Schema {
    /// <summary>
    ///   A simple <see cref="Clause"/> that evaluates the value of an expression on the value of a Field relative to a
    ///   single constant value.
    /// </summary>
    public sealed record ConstantClause : Clause {
        /// <summary>
        ///   The left-hand operand of this <see cref="ConstantClause"/>, which is an expression on the value of a
        ///   Field.
        /// </summary>
        public FieldExpression LHS { get; }

        /// <summary>
        ///   The operator of this <see cref="ConstantClause"/>.
        /// </summary>
        public ComparisonOperator Operator { get; }

        /// <summary>
        ///   The right-hand operand of this <see cref="ConstantClause"/>, which is a single value.
        /// </summary>
        public DBValue RHS { get; }

        /// <summary>
        ///   Constructs a new <see cref="ConstantClause"/>.
        /// </summary>
        /// <param name="lhs">
        ///   The <see cref="LHS">left-hand operand</see> of the new <see cref="ConstantClause"/>.
        /// </param>
        /// <param name="op">
        ///   The <see cref="Operator">operator</see> of the new <see cref="ConstantClause"/>.
        /// </param>
        /// <param name="rhs">
        ///   The <see cref="RHS">right-hand operand</see> of the new <see cref="ConstantClause"/>.
        /// </param>
        /// <pre>
        ///   <paramref name="lhs"/> is not <see langword="null"/>
        ///     --and--
        ///   <paramref name="rhs"/> is not <see cref="DBValue.NULL"/>
        ///     --and--
        ///   <paramref name="rhs"/> is compatible with the <see cref="FieldExpression.DataType">data type</see> of
        ///   <paramref name="lhs"/>.
        /// </pre>
        internal ConstantClause(FieldExpression lhs, ComparisonOperator op, DBValue rhs) {
            Debug.Assert(lhs is not null);
            Debug.Assert(op.IsValid());
            Debug.Assert(rhs != DBValue.NULL);
            Debug.Assert(rhs.IsInstanceOf(lhs.DataType));

            LHS = lhs;
            Operator = op;
            RHS = rhs;
        }

        /* Because ConstantClause is record type, the following methods are synthesized automatically by the compiler:
         *   > public ConstantClause(ConstantClause rhs)
         *   > public bool Equals(ConstantClause? rhs)
         *   > public sealed override bool Equals(object? rhs)
         *   > public sealed override int GetHashCode()
         *   > public sealed override string ToString()
         *   > public static bool operator==(ConstantClause? lhs, ConstantClause? rhs)
         *   > public static bool operator!=(ConstantClause? lhs, ConstantClause? rhs)
         */

        /// <inheritdoc/>
        public sealed override Clause Negation() {
            return new ConstantClause(LHS, Operator.Negation(), RHS);
        }

        /// <inheritdoc/>
        public sealed override IEnumerable<IField> GetDependentFields() {
            yield return LHS.Field;
        }

        /// <inheritdoc/>
        internal sealed override void AddDeclarationTo(IConstraintDeclBuilder builder) {
            Debug.Assert(builder is not null);
            builder.AddClause(this);
        }
    }
}
