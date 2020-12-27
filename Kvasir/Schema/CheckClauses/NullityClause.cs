using Cybele.Extensions;
using Kvasir.Transcription.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Kvasir.Schema.Constraints {
    /// <summary>
    ///   A non-compound <see cref="Clause"/> that expresses a condition on the nullity (<i>not</i> nullability) of an
    ///   <see cref="IField"/>.
    /// </summary>
    public sealed class NullityClause : Clause {
        /// <value>
        ///   The left-hand expression of this <see cref="NullityClause"/>.
        /// </value>
        public FieldExpression LHS { get; }

        /// <value>
        ///   The operator of this <see cref="NullityClause"/>.
        /// </value>
        public NullityOperator Operator { get; }

        /// <summary>
        ///   Constructs a new <see cref="NullityClause"/>.
        /// </summary>
        /// <param name="field">
        ///   The <see cref="LHS">field on which</see> the new <see cref="NullityClause"/> is predicated.
        /// </param>
        /// <param name="op">
        ///   The <see cref="Operator"/> of the new <see cref="NullityClause"/>.
        /// </param>
        /// <exception cref="ArgumentException">
        ///   if <paramref name="field"/> is not nullable.
        /// </exception>
        internal NullityClause(IField field, NullityOperator op) {
            Debug.Assert(op.IsValid());

            if (field.Nullability != IsNullable.Yes) {
                var msg = "A Field in a nullity constraint must be nullable";
                throw new ArgumentException(msg);
            }

            LHS = new FieldExpression(field);
            Operator = op;
        }

        /// <inheritdoc/>
        public sealed override Clause Negation() {
            if (Operator == NullityOperator.IsNotNull) {
                return new NullityClause(LHS.Field, NullityOperator.IsNull);
            }
            else {
                return new NullityClause(LHS.Field, NullityOperator.IsNotNull);
            }
        }

        /// <inheritdoc/>
        internal sealed override IEnumerable<IField> GetDependentFields() {
            return Enumerable.Repeat(LHS.Field, 1);
        }

        /// <inheritdoc/>
        internal sealed override void AddDeclarationTo(IConstraintDeclBuilder builder) {
            builder.AddCheck(this);
        }
    }
}
