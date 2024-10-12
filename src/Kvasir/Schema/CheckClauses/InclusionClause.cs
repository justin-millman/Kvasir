using Cybele.Extensions;
using Kvasir.Schema;
using Kvasir.Transcription;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Kvasir.Schema {
    /// <summary>
    ///   A simple <see cref="Clause"/> that evaluates the value of an expression on the value of a Field relative to a
    ///   discrete list of values.
    /// </summary>
    public sealed record InclusionClause : Clause {
        /// <summary>
        ///   The left-hand operand of this <see cref="InclusionClause"/>, which is an expression on the value of a
        ///   Field.
        /// </summary>
        public FieldExpression LHS { get; }

        /// <summary>
        ///   The operator of this <see cref="InclusionClause"/>.
        /// </summary>
        public InclusionOperator Operator { get; }

        /// <summary>
        ///   The right-hand operand of this <see cref="InclusionClause"/>, which is a non-empty list of values.
        /// </summary>
        public IEnumerable<DBValue> RHS { get; }

        /// <summary>
        ///   Constructs a new <see cref="InclusionClause"/>.
        /// </summary>
        /// <param name="lhs">
        ///   The <see cref="LHS">left-hand operand</see> of the new <see cref="InclusionClause"/>.
        /// </param>
        /// <param name="op">
        ///   The <see cref="Operator">operator</see> of the new <see cref="InclusionClause"/>.
        /// </param>
        /// <param name="rhs">
        ///   The <see cref="RHS">right-hand operand</see> of the new <see cref="InclusionClause"/>.
        /// </param>
        /// <pre>
        ///   <paramref name="lhs"/> is not <see langword="null"/>
        ///     --and--
        ///   <paramref name="rhs"/> is neither <see langword="null"/> nor empty
        ///     --and--
        ///   each element of <paramref name="rhs"/> is compatible with the
        ///   <see cref="FieldExpression.DataType">data type</see> of <paramref name="lhs"/>.
        /// </pre>
        internal InclusionClause(FieldExpression lhs, InclusionOperator op, IEnumerable<DBValue> rhs) {
            Debug.Assert(lhs is not null);
            Debug.Assert(op.IsValid());
            Debug.Assert(rhs is not null);
            Debug.Assert(!rhs.IsEmpty());
            Debug.Assert(rhs.All(v => v.IsInstanceOf(lhs.DataType)));

            // Note: It is valid for the data type of the LHS to be DBType.Enumeration because the Clause could be part
            // of a complex expression, such as an implication. It would be redundant to have a standalone
            // InclusionClause on the value of an enumeration-type Field, but putting a check here would prevent other
            // potentially valid use cases.

            LHS = lhs;
            Operator = op;
            RHS = rhs;
        }

        /* Because InclusionClause is record type, the following methods are synthesized automatically by the compiler:
         *   > public InclusionClause(InclusionClause rhs)
         *   > public bool Equals(InclusionClause? rhs)
         *   > public sealed override bool Equals(object? rhs)
         *   > public sealed override int GetHashCode()
         *   > public sealed override string ToString()
         *   > public static bool operator==(InclusionClause? lhs, InclusionClause? rhs)
         *   > public static bool operator!=(InclusionClause? lhs, InclusionClause? rhs)
         */

        /// <inheritdoc/>
        public sealed override Clause Negation() {
            return new InclusionClause(LHS, Operator.Negation(), RHS);
        }

        /// <inheritdoc/>
        public sealed override IEnumerable<IField> GetDependentFields() {
            yield return LHS.Field;
        }
        
        /// <inheritdoc/>
        internal sealed override void AddDeclarationTo<TDecl>(IConstraintDeclBuilder<TDecl> builder) {
            Debug.Assert(builder is not null);
            builder.AddClause(this);
        }
    }
}
