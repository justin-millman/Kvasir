using Ardalis.GuardClauses;
using Cybele.Core;
using Cybele.Extensions;
using Kvasir.Core;
using Kvasir.Schema;
using Optional;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using static Kvasir.Core.TransformUserValues;

namespace Kvasir.Annotations {
    /// <summary>
    ///   An attribute that imposes a simple <c>CHECK</c> constraint (one of the form <c>[field] [op] [value]</c>) on
    ///   the Field backing a particular property.
    /// </summary>
    public abstract class ConstraintAttribute : Attribute {
        /// <summary>
        ///   The dot-separated path, relative to the property on which the annotation is placed, to the property to
        ///   which the annotation actually applies.
        /// </summary>
        public string Path { get; init; } = "";

        /// <summary>
        ///   Creates a <c>CHECK</c> constraint <see cref="Clause"/> for one or more Fields using the generator type
        ///   specified in the annotation.
        /// </summary>
        /// <param name="field">
        ///   The Field to which the constraint applies.
        /// </param>
        /// <param name="converter">
        ///   The list of data converter that corresponds to <paramref name="field"/>, which is to be used as needed
        ///   to transform any user-provided input.
        /// </param>
        /// <param name="settings">
        ///   The Kvasir framework settings.
        /// </param>
        /// <returns>
        ///   A new <c>CHECK</c> constraint clause that applies to <paramref name="field"/>.
        /// </returns>
        internal Clause MakeConstraint(IField field, DataConverter converter, Settings settings) {
            Debug.Assert(field is not null);
            Debug.Assert(converter is not null);
            Debug.Assert(settings is not null);

            var expr = func_.Match(some: f => new FieldExpression(f, field), none: () => new FieldExpression(field));
            var rhs = Transform(anchor_, field, converter, settings);

            return (op_, rhs) switch {
                (ComparisonOperator oprtr, DBValue value) => new ConstantClause(expr, oprtr, value),
                (InclusionOperator oprtr, IEnumerable<DBValue> value) => new InclusionClause(expr, oprtr, value),
                (_, _) => throw new ApplicationException("Internal Error: Wrong Types when Creating Attribute Clause")
            };
        }

        /// <summary>
        ///   Constructs a new instance of the <see cref="ConstraintAttribute"/> that imposes a comparison-based
        ///   constraint on the Field backing the annotated property.
        /// </summary>
        /// <param name="op">
        ///   The comparison operator.
        /// </param>
        /// <param name="anchor">
        ///   The anchor value.
        /// </param>
        internal ConstraintAttribute(ComparisonOperator op, object anchor)
            : this(Option.None<FieldFunction>(), op, anchor) {}

        /// <summary>
        ///   Constructs a new instance of the <see cref="ConstraintAttribute"/> that imposes an inclusion-based
        ///   constraint on the Field backing the annotated property.
        /// </summary>
        /// <param name="op">
        ///   The inclusion operator.
        /// </param>
        /// <param name="anchor">
        ///   The list of allowd or disallowed values.
        /// </param>
        internal ConstraintAttribute(InclusionOperator op, IEnumerable<object?> anchor)
            : this(Option.None<FieldFunction>(), op, anchor) {}

        /// <summary>
        ///   Constructs a new instance of the <see cref="ConstraintAttribute"/> that imposes a comparison-based
        ///   constraint on the transformation of the Field backing the annotated property.
        /// </summary>
        /// <param name="func">
        ///   The transformation function be applied.
        /// </param>
        /// <param name="op">
        ///   The comparison operator.
        /// </param>
        /// <param name="anchor">
        ///   The anchor value.
        /// </param>
        internal ConstraintAttribute(FieldFunction func, ComparisonOperator op, object anchor)
            : this(Option.Some(func), op, anchor) {}

        /// <summary>
        ///   Constructs a new instance of the <see cref="ConstraintAttribute"/> that imposes an inclusion-based
        ///   constraint on the transformation of the Field backing the annotated property.
        /// </summary>
        /// <param name="func">
        ///   The transformation function be applied.
        /// </param>
        /// <param name="op">
        ///   The comparison operator.
        /// </param>
        /// <param name="anchor">
        ///   The list of allowd or disallowed values.
        /// </param>
        internal ConstraintAttribute(FieldFunction func, InclusionOperator op, IEnumerable<object?> anchor)
            : this(Option.Some(func), op, anchor) {}

        /// <summary>
        ///   Constructs a new instance of the <see cref="ConstraintAttribute"/> that imposes a comparison-based
        ///   constraint on the Field backing the annotated property.
        /// </summary>
        /// <param name="func">
        ///   The optional transformation function be applied.
        /// </param>
        /// <param name="op">
        ///   The comparison operator.
        /// </param>
        /// <param name="anchor">
        ///   The anchor value.
        /// </param>
        private ConstraintAttribute(Option<FieldFunction> func, ComparisonOperator op, object anchor) {
            func.MatchSome(f => Guard.Against.InvalidInput(f, nameof(func), f => f.IsValid()));
            Guard.Against.InvalidInput(op, nameof(op), o => o.IsValid());
            Guard.Against.Null(anchor, nameof(anchor));
            Guard.Against.InvalidInput(anchor, nameof(anchor), a => DBType.IsSupported(a.GetType()));

            func_ = func;
            op_ = op;
            anchor_ = anchor;
        }

        /// <summary>
        ///   Constructs a new instance of the <see cref="ConstraintAttribute"/> that imposes an inclusion-based
        ///   constraint on the Field backing the annotated property.
        /// </summary>
        /// <param name="func">
        ///   The optional transformation function be applied.
        /// </param>
        /// <param name="op">
        ///   The inclusion operator.
        /// </param>
        /// <param name="anchor">
        ///   The list of allowd or disallowed values.
        /// </param>
        private ConstraintAttribute(Option<FieldFunction> func, InclusionOperator op, IEnumerable<object?> anchor) {
            func.MatchSome(f => Guard.Against.InvalidInput(f, nameof(func), f => f.IsValid()));
            Guard.Against.InvalidInput(op, nameof(op), o => o.IsValid());
            Guard.Against.NullOrEmpty(anchor, nameof(anchor));

            var t = anchor.FirstOrDefault(o => o is not null)?.GetType();
            Guard.Against.InvalidInput(anchor, nameof(anchor), _ => t is null || DBType.IsSupported(t));
            Guard.Against.InvalidInput(anchor, nameof(anchor), a => a.AllSame(o => o?.GetType() ?? t));

            func_ = func;
            op_ = op;
            anchor_ = anchor.ToHashSet();
        }


        private readonly Option<FieldFunction> func_;
        private readonly object op_;       // an InclusionOperator or a ComparisonOperator
        private readonly object anchor_;   // a primitive numeric, a string, an enumerator, null, or a set thereof
    }
}
