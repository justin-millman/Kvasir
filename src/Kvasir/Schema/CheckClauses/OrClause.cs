using Kvasir.Transcription;
using System.Diagnostics;
using System.Linq;

namespace Kvasir.Schema {
    /// <summary>
    ///   A compound <see cref="Clause"/> representing an disjunction between two logical expressions.
    /// </summary>
    public sealed record OrClause : Clause {
        /// <summary>
        ///   The left-hand operand of this <see cref="OrClause"/>. It is logically interchangeable with the
        ///   <see cref="RHS">right-hand operand</see>.
        /// </summary>
        public Clause LHS { get; }

        /// <summary>
        ///   The right-hand operand of this <see cref="OrClause"/>. It is logically interchangeable with the
        ///   <see cref="LHS">left-hand operand</see>.
        /// </summary>
        public Clause RHS { get; }

        /// <summary>
        ///   Constructs a new <see cref="AndClause"/>.
        /// </summary>
        /// <param name="lhs">
        ///   The <see cref="LHS">left-hand operand</see> of the new <see cref="OrClause"/>.
        /// </param>
        /// <param name="rhs">
        ///   The <see cref="RHS">right-hand operand</see> of the new <see cref="OrClause"/>.
        /// </param>
        /// <pre>
        ///   <paramref name="lhs"/> is not <see langword="null"/>
        ///     --and--
        ///   <paramref name="rhs"/> is not <see langword="null"/>.
        /// </pre>
        internal OrClause(Clause lhs, Clause rhs)
            : this(lhs, rhs, false) {}

        /* Because OrClause is record type, the following methods are synthesized automatically by the compiler:
         *   > public OrClause(OrClause rhs)
         *   > public bool Equals(OrClause? rhs)
         *   > public sealed override bool Equals(object? rhs)
         *   > public sealed override int GetHashCode()
         *   > public sealed override string ToString()
         *   > public static bool operator==(OrClause? lhs, OrClause? rhs)
         *   > public static bool operator!=(OrClause? lhs, OrClause? rhs)
         */

        /// <inheritdoc/>
        public sealed override Clause Negation() {
            return new OrClause(LHS, RHS, !isNegated_);
        }

        /// <inheritdoc/>
        public sealed override FieldSeq GetDependentFields() {
            return LHS.GetDependentFields().Concat(RHS.GetDependentFields());
        }

        /// <inheritdoc/>
        internal sealed override void AddDeclarationTo<TDecl>(IConstraintDeclBuilder<TDecl> builder) {
            Debug.Assert(builder is not null);

            if (!isNegated_) {
                builder.StartClause();
                LHS.AddDeclarationTo(builder);
                builder.Or();
                RHS.AddDeclarationTo(builder);
                builder.EndClause();
            }
            else {
                var negation = LHS.Negation().And(RHS.Negation());
                negation.AddDeclarationTo(builder);
            }
        }

        /// <summary>
        ///   Constructs a new <see cref="AndClause"/> that may be negated.
        /// </summary>
        /// <param name="lhs">
        ///   The <see cref="LHS">left-hand operand</see> of the new <see cref="OrClause"/>.
        /// </param>
        /// <param name="rhs">
        ///   The <see cref="RHS">right-hand operand</see> of the new <see cref="OrClause"/>.
        /// </param>
        /// <param name="negated">
        ///   Whether or not the new <see cref="XorClause"/> should be treated as negated.
        /// </param>
        /// <pre>
        ///   <paramref name="lhs"/> is not <see langword="null"/>
        ///     --and--
        ///   <paramref name="rhs"/> is not <see langword="null"/>.
        /// </pre>
        private OrClause(Clause lhs, Clause rhs, bool negated) {
            Debug.Assert(lhs is not null);
            Debug.Assert(rhs is not null);

            LHS = lhs;
            RHS = rhs;
            isNegated_ = negated;
        }


        private readonly bool isNegated_;
    }
}
