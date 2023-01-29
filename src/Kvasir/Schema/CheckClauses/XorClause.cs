using Kvasir.Transcription;
using System.Diagnostics;
using System.Linq;

namespace Kvasir.Schema {
    /// <summary>
    ///   A compound <see cref="Clause"/> representing an exclusive disjunction between two logical expressions.
    /// </summary>
    public sealed record XorClause : Clause {
        /// <summary>
        ///   The left-hand operand of this <see cref="XorClause"/>. It is logically interchangeable with the
        ///   <see cref="RHS">right-hand operand</see>.
        /// </summary>
        public Clause LHS { get; }

        /// <summary>
        ///   The right-hand operand of this <see cref="XorClause"/>. It is logically interchangeable with the
        ///   <see cref="LHS">left-hand operand</see>.
        /// </summary>
        public Clause RHS { get; }

        /// <summary>
        ///   Constructs a new <see cref="XorClause"/>.
        /// </summary>
        /// <param name="lhs">
        ///   The <see cref="LHS">left-hand operand</see> of the new <see cref="XorClause"/>.
        /// </param>
        /// <param name="rhs">
        ///   The <see cref="RHS">right-hand operand</see> of the new <see cref="XorClause"/>.
        /// </param>
        /// <pre>
        ///   <paramref name="lhs"/> is not <see langword="null"/>
        ///     --and--
        ///   <paramref name="rhs"/> is not <see langword="null"/>.
        /// </pre>
        internal XorClause(Clause lhs, Clause rhs)
            : this(lhs, rhs, false) {}

        /* Because OrClause is record type, the following methods are synthesized automatically by the compiler:
         *   > public XorClause(XorClause rhs)
         *   > public bool Equals(XorClause? rhs)
         *   > public sealed override bool Equals(object? rhs)
         *   > public sealed override int GetHashCode()
         *   > public sealed override string ToString()
         *   > public static bool operator==(XorClause? lhs, XorClause? rhs)
         *   > public static bool operator!=(XorClause? lhs, XorClause? rhs)
         */

        /// <inheritdoc/>
        public sealed override Clause Negation() {
            return new XorClause(LHS, RHS, !isNegated_);
        }

        /// <inheritdoc/>
        public sealed override FieldSeq GetDependentFields() {
            return LHS.GetDependentFields().Concat(RHS.GetDependentFields());
        }

        /// <inheritdoc/>
        internal sealed override void AddDeclarationTo<TDecl>(IConstraintDeclBuilder<TDecl> builder) {
            Debug.Assert(builder is not null);

            var compoundLHS = !isNegated_ ? LHS.And(RHS.Negation()) : LHS.And(RHS);
            var compoundRHS = !isNegated_ ? RHS.And(LHS.Negation()) : LHS.Negation().And(RHS.Negation());

            builder.StartClause();
            compoundLHS.AddDeclarationTo(builder);
            builder.Or();
            compoundRHS.AddDeclarationTo(builder);
            builder.EndClause();
        }

        /// <summary>
        ///   Constructs a new <see cref="XorClause"/> that may be negated.
        /// </summary>
        /// <param name="lhs">
        ///   The <see cref="LHS">left-hand operand</see> of the new <see cref="XorClause"/>.
        /// </param>
        /// <param name="rhs">
        ///   The <see cref="RHS">right-hand operand</see> of the new <see cref="XorClause"/>.
        /// </param>
        /// <param name="negated">
        ///   Whether or not the new <see cref="XorClause"/> should be treated as negated.
        /// </param>
        /// <pre>
        ///   <paramref name="lhs"/> is not <see langword="null"/>
        ///     --and--
        ///   <paramref name="rhs"/> is not <see langword="null"/>.
        /// </pre>
        private XorClause(Clause lhs, Clause rhs, bool negated) {
            Debug.Assert(lhs is not null);
            Debug.Assert(rhs is not null);

            LHS = lhs;
            RHS = rhs;
            isNegated_ = negated;
        }


        // We use a member variable to track negation so that the generated declaration of a twice- or otherwise
        // multiply-negated XorClause can be sane. The exclusive-or expression (a ^ b) can be represented using only
        // AND and OR as (a && !b) || (b && !a), the negation of which is (!a || b) && (!b || a). While accurate, this
        // is far less intuitive that the logically equivalent (a && b) || (!a && !b). By using a standalone variable,
        // we can achieve the latter rather than the former.
        private readonly bool isNegated_;
    }
}
