using Kvasir.Transcription.Internal;
using System.Collections.Generic;
using System.Linq;

namespace Kvasir.Schema.Constraints {
    /// <summary>
    ///   A compound <see cref="Clause"/> that represents the logical disjunction between two constituent
    ///   <see cref="Clause">sub-clauses</see>.
    /// </summary>
    public sealed class OrClause : Clause {
        /// <value>
        ///   The left-hand portion of this <see cref="OrClause"/>.
        /// </value>
        public Clause LHS { get; }

        /// <value>
        ///   The right-hand portion of this <see cref="OrClause"/>.
        /// </value>
        public Clause RHS { get; }

        /// <summary>
        ///   Constructs a new <see cref="OrClause"/>.
        /// </summary>
        /// <param name="lhs">
        ///   The <see cref="LHS">left-hand portion</see> of the new <see cref="OrClause"/>.
        /// </param>
        /// <param name="rhs">
        ///   The <see cref="RHS">right-hand portion</see> of the new <see cref="OrClause"/>.
        /// </param>
        public OrClause(Clause lhs, Clause rhs) {
            LHS = lhs;
            RHS = rhs;
        }

        /// <inheritdoc/>
        public sealed override Clause Negation() {
            return new AndClause(LHS.Negation(), RHS.Negation());
        }

        /// <inheritdoc/>
        internal sealed override IEnumerable<IField> GetDependentFields() {
            return LHS.GetDependentFields().Concat(RHS.GetDependentFields());
        }

        /// <inheritdoc/>
        internal sealed override void AddDeclarationTo(IConstraintDeclGenerator decl) {
            using var _ = decl.NewOrClause();
            LHS.AddDeclarationTo(decl);
            RHS.AddDeclarationTo(decl);
        }
    }
}
