using Kvasir.Transcription;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Kvasir.Schema {
    /// <summary>
    ///   A compound <see cref="Clause"/> representing an conjunction between two logical expressions.
    /// </summary>
    public sealed record AndClause : Clause {
        /// <summary>
        ///   The left-hand operand of this <see cref="AndClause"/>. It is logically interchangeable with the
        ///   <see cref="RHS">right-hand operand</see>.
        /// </summary>
        public Clause LHS { get; }

        /// <summary>
        ///   The right-hand operand of this <see cref="AndClause"/>. It is logically interchangeable with the
        ///   <see cref="LHS">left-hand operand</see>.
        /// </summary>
        public Clause RHS { get; }

        /// <summary>
        ///   Constructs a new <see cref="AndClause"/>.
        /// </summary>
        /// <param name="lhs">
        ///   The <see cref="LHS">left-hand operand</see> of the new <see cref="AndClause"/>.
        /// </param>
        /// <param name="rhs">
        ///   The <see cref="RHS">right-hand operand</see> of the new <see cref="AndClause"/>.
        /// </param>
        /// <pre>
        ///   <paramref name="lhs"/> is not <see langword="null"/>
        ///     --and--
        ///   <paramref name="rhs"/> is not <see langword="null"/>.
        /// </pre>
        internal AndClause(Clause lhs, Clause rhs)
            : this(lhs, rhs, false) {}

        /* Because AndClause is record type, the following methods are synthesized automatically by the compiler:
         *   > public AndClause(AndClause rhs)
         *   > public bool Equals(AndClause? rhs)
         *   > public sealed override bool Equals(object? rhs)
         *   > public sealed override int GetHashCode()
         *   > public sealed override string ToString()
         *   > public static bool operator==(AndClause? lhs, AndClause? rhs)
         *   > public static bool operator!=(AndClause? lhs, AndClause? rhs)
         */

        /// <inheritdoc/>
        public sealed override Clause Negation() {
            return new AndClause(LHS, RHS, !isNegated_);
        }

        /// <inheritdoc/>
        public sealed override IEnumerable<IField> GetDependentFields() {
            return LHS.GetDependentFields().Concat(RHS.GetDependentFields());
        }

        /// <inheritdoc/>
        internal sealed override void AddDeclarationTo(IConstraintDeclBuilder builder) {
            Debug.Assert(builder is not null);

            if (!isNegated_) {
                builder.StartClause();
                LHS.AddDeclarationTo(builder);
                builder.And();
                RHS.AddDeclarationTo(builder);
                builder.EndClause();
            }
            else {
                var negation = LHS.Negation().Or(RHS.Negation());
                negation.AddDeclarationTo(builder);
            }
        }

        /// <summary>
        ///   Constructs a new <see cref="AndClause"/> that may be negated.
        /// </summary>
        /// <param name="lhs">
        ///   The <see cref="LHS">left-hand operand</see> of the new <see cref="AndClause"/>.
        /// </param>
        /// <param name="rhs">
        ///   The <see cref="RHS">right-hand operand</see> of the new <see cref="AndClause"/>.
        /// </param>
        /// <param name="negated">
        ///   Whether or not the new <see cref="XorClause"/> should be treated as negated.
        /// </param>
        /// <pre>
        ///   <paramref name="lhs"/> is not <see langword="null"/>
        ///     --and--
        ///   <paramref name="rhs"/> is not <see langword="null"/>.
        /// </pre>
        private AndClause(Clause lhs, Clause rhs, bool negated) {
            Debug.Assert(lhs is not null);
            Debug.Assert(rhs is not null);

            LHS = lhs;
            RHS = rhs;
            isNegated_ = negated;
        }


        private readonly bool isNegated_;
    }
}
