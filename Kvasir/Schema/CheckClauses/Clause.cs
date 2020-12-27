using Kvasir.Transcription.Internal;
using System.Collections.Generic;

namespace Kvasir.Schema.Constraints {
    /// <summary>
    ///   An abstract representation of a single clause in a potentially complex Boolean <c>CHECK</c> constraint.
    /// </summary>
    public abstract class Clause {
        /// <summary>
        ///   Creates a new <see cref="Clause"/> that represents the logical negation of this <see cref="Clause"/>.
        /// </summary>
        /// <returns>
        ///   A new <see cref="Clause"/> that represents the logical negation of this <see cref=" Clause"/>.
        /// </returns>
        public abstract Clause Negation();

        /// <summary>
        ///   Creates a compound clause that represents the logical conjunction (i.e. <c>AND</c>) of this
        ///   <see cref="Clause"/> and another.
        /// </summary>
        /// <param name="rhs">
        ///   The <see cref="Clause"/> with which to conjunct this one.
        /// </param>
        /// <returns>
        ///   A compound clause that represents the logical conjunction of this <see cref="Clause"/> and
        ///   <paramref name="rhs"/>.
        /// </returns>
        public Clause And(Clause rhs) {
            return new AndClause(this, rhs);
        }

        /// <summary>
        ///   Creates a compound clause that represents the logical disjunction (i.e. <c>AND</c>) of this
        ///   <see cref="Clause"/> and another.
        /// </summary>
        /// <param name="rhs">
        ///   The <see cref="Clause"/> with which to disjunct this one.
        /// </param>
        /// <returns>
        ///   A compound clause that represents the logical disjunction of this <see cref="Clause"/> and
        ///   <paramref name="rhs"/>.
        /// </returns>
        public Clause Or(Clause rhs) {
            return new OrClause(this, rhs);
        }

        /// <summary>
        ///   Creates a compound clause that represents the logical exclusive disjunctino (i.e. <c>XOR</c>) of this
        ///   <see cref="Clause"/> and another.
        /// </summary>
        /// <param name="rhs">
        ///   The <see cref="Clause"/> with which to exclusively disjunct this one.
        /// </param>
        /// <returns>
        ///   A compound clause that represents the logical disjunction of this <see cref="Clause"/> and
        ///   <paramref name="rhs"/>.
        /// </returns>
        public Clause Xor(Clause rhs) {
            var xorLeft = new AndClause(this, rhs.Negation());
            var xorRight = new AndClause(this.Negation(), rhs);
            return new OrClause(xorLeft, xorRight);
        }

        /// <summary>
        ///   Gets a collection of the <see cref="IField">Fields</see> on whose value this <see cref="Clause"/> is at
        ///   least partially dependent.
        /// </summary>
        /// <returns>
        ///   A collection of the <see cref="IField">Fields</see> on whose value this <see cref="Clause"/> is at least
        ///   partially dependent. The order of the see Fields is not defined, and Fields may be (but are not
        ///   necessarily) duplicated.
        /// </returns>
        internal abstract IEnumerable<IField> GetDependentFields();

        /// <summary>
        ///   Appends the declaratory SQL for this <see cref="Clause"/> onto the in-progress declaration managed by
        ///   an existing <see cref="IConstraintDeclBuilder"/>.
        /// </summary>
        /// <param name="builder">
        ///   The <see cref="IConstraintDeclBuilder"/> to which to add the declaration for this <see cref="Clause"/>.
        /// </param>
        internal abstract void AddDeclarationTo(IConstraintDeclBuilder builder);

        /// <summary>
        ///   Creates a compound clause that represents a unidirectional logical implication (i.e. <c>if X then Y</c>)
        ///   between two constituent <see cref="Clause">Clauses</see>.
        /// </summary>
        /// <param name="predicate">
        ///   The predicate <see cref="Clause"/> of the logical implication, i.e. the <c>X</c> in <c>if X then Y</c>.
        /// </param>
        /// <param name="consequent">
        ///   The consequent <see cref="Clause"/> of the logical implication, i.e. the <c>Y</c> in <c>if X then Y</c>.
        /// </param>
        /// <returns>
        ///   A compound clause that represents the logical implication of "if <paramref name="predicate"/> then
        ///   <paramref name="consequent"/>."
        /// </returns>
        public static Clause IfThen(Clause predicate, Clause consequent) {
            return new OrClause(consequent, predicate.Negation());
        }

        /// <summary>
        ///   Creates a compound clause that represents a bidrectional logical implication (i.e. <c>X if and only if Y
        ///   </c>) between two constituent <see cref="Clause">Clauses</see>.
        /// </summary>
        /// <param name="lhs">
        ///   The left-hand <see cref="Clause"/> of the logical implication, i.e. the <c>X</c> in <c>X if and only if
        ///   Y</c>, which is logically interchangeable with the right-hand side.
        /// </param>
        /// <param name="rhs">
        ///   The right-hand <see cref="Clause"/> of the logical implication, i.e. the <c>Y</c> in <c>X if and only if
        ///   Y</c>, which is logically interchangeable with the left-hand side.
        /// </param>
        /// <returns>
        ///   A compound clause that represents the logical implication of "<paramref name="lhs"/> if and only if
        ///   <paramref name="rhs"/>."
        /// </returns>
        public static Clause Iff(Clause lhs, Clause rhs) {
            var iffLeft = new AndClause(lhs, rhs);
            var iffRight = new AndClause(lhs.Negation(), rhs.Negation());
            return new OrClause(iffLeft, iffRight);
        }
    }
}
