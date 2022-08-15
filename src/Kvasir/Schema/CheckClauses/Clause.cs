using Kvasir.Transcription;
using System.Collections.Generic;

namespace Kvasir.Schema {
    /// <summary>
    ///   A piece of a conditional expression.
    /// </summary>
    public abstract record Clause {
        /// <summary>
        ///   Produces a new <see cref="Clause"/> that represents the negation of this one.
        /// </summary>
        /// <returns>
        ///   A new <see cref="Clause"/> whose evaluation, logically, yields <c>true</c> if and only if the evaluation
        ///   of this one yields <c>false</c>.
        /// </returns>
        public abstract Clause Negation();

        /// <summary>
        ///   Creates a conjunctive <see cref="Clause"/> involving this <see cref="Clause"/> and another. This
        ///   <see cref="Clause"/> is the left-hand operand.
        /// </summary>
        /// <param name="rhs">
        ///   The right-hand operand of the new conjunctive <see cref="Clause"/>.
        /// </param>
        /// <pre>
        ///   <paramref name="rhs"/> is not <see langword="null"/>.
        /// </pre>
        /// <returns>
        ///   A new conjunctive clause where this <see cref="Clause"/> is the left-hand operand and
        ///   <paramref name="rhs"/> is the right-hand operand.
        /// </returns>
        public Clause And(Clause rhs) {
            return new AndClause(this, rhs);
        }

        /// <summary>
        ///   Creates a disjunctive <see cref="Clause"/> involving this <see cref="Clause"/> and another. This
        ///   <see cref="Clause"/> is the left-hand operand.
        /// </summary>
        /// <param name="rhs">
        ///   The right-hand operand of the new disjunctive <see cref="Clause"/>.
        /// </param>
        /// <pre>
        ///   <paramref name="rhs"/> is not <see langword="null"/>.
        /// </pre>
        /// <returns>
        ///   A new disjunctive clause where this <see cref="Clause"/> is the left-hand operand and
        ///   <paramref name="rhs"/> is the right-hand operand.
        /// </returns>
        public Clause Or(Clause rhs) {
            return new OrClause(this, rhs);
        }

        /// <summary>
        ///   Creates an exclusively disjunctive <see cref="Clause"/> involving this <see cref="Clause"/> and another.
        ///   This <see cref="Clause"/> is the left-hand operand.
        /// </summary>
        /// <param name="rhs">
        ///   The right-hand operand of the new exclusively disjunctive <see cref="Clause"/>.
        /// </param>
        /// <pre>
        ///   <paramref name="rhs"/> is not <see langword="null"/>.
        /// </pre>
        /// <returns>
        ///   A new exclusively disjunctive clause where this <see cref="Clause"/> is the left-hand operand and
        ///   <paramref name="rhs"/> is the right-hand operand.
        /// </returns>
        public Clause Xor(Clause rhs) {
            return new XorClause(this, rhs);
        }

        /* Because Clause is record type, the following methods are synthesized automatically by the compiler:
         *   > public bool Equals(Clause? rhs)
         *   > public override bool Equals(object? rhs)
         *   > public override int GetHashCode()
         *   > public override string ToString()
         *   > public static bool operator==(Clause? lhs, Clause? rhs)
         *   > public static bool operator!=(Clause? lhs, Clause? rhs)
         */

        /// <summary>
        ///   Produces the Fields on whose values this <see cref="Clause"/> is dependent in an undefined order.
        ///   If a Field's value is involved in multiple pieces of this <see cref="Clause"/>, that Field will appear
        ///   multiple times.
        /// </summary>
        /// <returns>
        ///   A list of the Fields on whose values this <see cref="Clause"/> is dependent.
        /// </returns>
        public abstract IEnumerable<IField> GetDependentFields();

        /// <summary>
        ///   Adds this <see cref="Clause"/> to an ongoing declaration.
        /// </summary>
        /// <typeparam name="TDecl">
        ///   [deduced] The type of declaration produced by <paramref name="builder"/>.
        /// </typeparam>
        /// <param name="builder">
        ///   The <see cref="IConstraintDeclBuilder{TDecl}"/> to which to add the declaration of this
        ///   <see cref="Clause"/>.
        /// </param>
        /// <pre>
        ///   <paramref name="builder"/> is not <see langword="null"/>.
        /// </pre>
        internal abstract void AddDeclarationTo<TDecl>(IConstraintDeclBuilder<TDecl> builder);

        /// <summary>
        ///   Creates a new <see cref="Clause"/> that represents a unidirectional implication, i.e. an <c>if-then</c>
        ///   relation.
        /// </summary>
        /// <param name="predicate">
        ///   The predicate of the new implication <see cref="Clause"/>.
        /// </param>
        /// <param name="consequent">
        ///   The consequent of the new implication <see cref="Clause"/>.
        /// </param>
        /// <pre>
        ///   <paramref name="predicate"/> is not <see langword="null"/>
        ///     --and--
        ///   <paramref name="consequent"/> is not <see langword="null"/>.
        /// </pre>
        /// <returns>
        ///   A new <see cref="Clause"/> that represents the logical expression "if <paramref name="predicate"/> then
        ///   <paramref name="consequent"/>."
        /// </returns>
        public static Clause IfThen(Clause predicate, Clause consequent) {
            return consequent.Or(predicate.Negation());
        }

        /// <summary>
        ///   Creates a new <see cref="Clause"/> that represents a bidirectional implication, i.e. an<c>if and only
        ///   if</c> relation.
        /// </summary>
        /// <param name="lhs">
        ///   The left-hand operand of the new implication <see cref="Clause"/>.
        /// </param>
        /// <param name="rhs">
        ///   The right-hand operand of the new implication <see cref="Clause"/>.
        /// </param>
        /// <pre>
        ///   <paramref name="lhs"/> is not <see langword="null"/>
        ///     --and--
        ///   <paramref name="rhs"/> is not <see langword="null"/>.
        /// </pre>
        /// <returns>
        ///   A new <see cref="Clause"/> that represents the logical expression "<paramref name="lhs"/> if and only if
        ///   <paramref name="rhs"/>."
        /// </returns>
        public static Clause Iff(Clause lhs, Clause rhs) {
            return lhs.Xor(rhs).Negation();
        }
    }
}
