using Cybele.Extensions;
using Kvasir.Transcription;
using System.Collections.Generic;
using System.Diagnostics;

namespace Kvasir.Schema {
    /// <summary>
    ///   A simple <see cref="Clause"/> that evaluates the nullity of a Field.
    /// </summary>
    public sealed record NullityClause : Clause {
        /// <summary>
        ///   The left-hand operand of this <see cref="NullityClause"/>, which is the expression being evaluated for
        ///   nullity. This is guaranteed to be an direct expression, i.e. one without an evaluatory function.
        /// </summary>
        public FieldExpression LHS { get; }

        /// <summary>
        ///   The operator of this <see cref="NullityClause"/>.
        /// </summary>
        public NullityOperator Operator { get; }

        /// <summary>
        ///   Constructs a new <see cref="NullityClause"/>.
        /// </summary>
        /// <param name="field">
        ///   The Field being evaluated for <c>NULL</c> in the new <see cref="NullityClause"/>.
        /// </param>
        /// <param name="op">
        ///   The <see cref="Operator">operator</see> of the new <see cref="NullityClause"/>.
        /// </param>
        /// <pre>
        ///   <paramref name="field"/> is not <see langword="null"/>.
        /// </pre>
        internal NullityClause(IField field, NullityOperator op) {
            Debug.Assert(field is not null);
            Debug.Assert(op.IsValid());

            LHS = new FieldExpression(field);
            Operator = op;
        }

        /* Because NullityClause is record type, the following methods are synthesized automatically by the compiler:
         *   > public NullityClause(NullityClause rhs)
         *   > public bool Equals(NullityClause? rhs)
         *   > public sealed override bool Equals(object? rhs)
         *   > public sealed override int GetHashCode()
         *   > public sealed override string ToString()
         *   > public static bool operator==(NullityClause? lhs, NullityClause? rhs)
         *   > public static bool operator!=(NullityClause? lhs, NullityClause? rhs)
         */

        /// <inheritdoc/>
        public sealed override Clause Negation() {
            return new NullityClause(LHS.Field, Operator.Negation());
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
