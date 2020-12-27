using Kvasir.Transcription.Internal;
using System.Collections.Generic;
using System.Linq;

namespace Kvasir.Schema.Constraints {
    /// <summary>
    ///   A compound <see cref="Clause"/> that represents the logical conjunction between two constituent
    ///   <see cref="Clause">sub-clauses</see>.
    /// </summary>
    public sealed class AndClause : Clause {
        /// <value>
        ///   The left-hand portion of this <see cref="AndClause"/>.
        /// </value>
        public Clause LHS { get; }

        /// <value>
        ///   The right-hand portion of this <see cref="AndClause"/>.
        /// </value>
        public Clause RHS { get; }

        /// <summary>
        ///   Constructs a new <see cref="AndClause"/>.
        /// </summary>
        /// <param name="lhs">
        ///   The <see cref="LHS">left-hand portion</see> of the new <see cref="AndClause"/>.
        /// </param>
        /// <param name="rhs">
        ///   The <see cref="RHS">right-hand portion</see> of the new <see cref="AndClause"/>.
        /// </param>
        public AndClause(Clause lhs, Clause rhs) {
            LHS = lhs;
            RHS = rhs;
        }

        /// <inheritdoc/>
        public sealed override Clause Negation() {
            return new OrClause(LHS.Negation(), RHS.Negation());
        }

        /// <inheritdoc/>
        internal sealed override IEnumerable<IField> GetDependentFields() {
            return LHS.GetDependentFields().Concat(RHS.GetDependentFields());
        }

        /// <inheritdoc/>
        internal sealed override void AddDeclarationTo(IConstraintDeclBuilder builder) {
            using var _ = builder.NewAndClause();
            LHS.AddDeclarationTo(builder);
            RHS.AddDeclarationTo(builder);
        }
    }
}
