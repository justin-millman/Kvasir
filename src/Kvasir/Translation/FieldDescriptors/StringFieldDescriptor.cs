using Cybele.Core;
using Cybele.Extensions;
using Kvasir.Annotations;
using Kvasir.Core;
using Kvasir.Schema;
using Optional;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace Kvasir.Translation {
    /// <summary>
    ///   The concrete base class for an <see cref="OrderableFieldDescriptor"/> whose data type is <see cref="string"/>.
    /// </summary>
    internal sealed class StringFieldDescriptor : OrderableFieldDescriptor {
        public StringFieldDescriptor(Context context, PropertyInfo source)
            : base(context, source) {

            Debug.Assert(FieldType == typeof(string));
            lengthConstraint_ = new Interval(Option.None<Bound>(), Option.None<Bound>());
        }

        public StringFieldDescriptor(Context context, PropertyInfo source, DataConverterAttribute annotation)
            : base(context, source, annotation) {

            Debug.Assert(FieldType == typeof(string));
            lengthConstraint_ = new Interval(Option.None<Bound>(), Option.None<Bound>());
        }

        public StringFieldDescriptor(Context context, PropertyInfo source, DataConverter converter)
            : base(context, source, converter) {

            Debug.Assert(FieldType == typeof(string));
            lengthConstraint_ = new Interval(Option.None<Bound>(), Option.None<Bound>());
        }

        public sealed override StringFieldDescriptor Clone() {
            return new StringFieldDescriptor(this);
        }

        public sealed override StringFieldDescriptor Reset() {
            return new StringFieldDescriptor(this, RESET);
        }

        protected sealed override void DoApplyConstraint(Context context, Check.StringLengthAttribute annotation) {
            // Rather than using Optionals, the Check.StringLengthAttribute uses long.MinValue and long.MaxValue to
            // indicate absent values. Additionally, the values stored in the long are _actually_ ints, that way the
            // min/max value are guaranteed never to match what the user might actually provide.

            if (annotation.Minimum != long.MinValue && annotation.Minimum < 0) {
                var msg = $"the minimum string length ({annotation.Minimum}) cannot be negative";
                throw new UnsatisfiableConstraintException(context, annotation, msg);
            }
            if (annotation.Maximum < 0) {
                var msg = $"the maximum string length ({annotation.Maximum}) cannot be negative";
                throw new UnsatisfiableConstraintException(context, annotation, msg);
            }

            if (annotation.Minimum != long.MinValue) {
                var lower = new Bound((int)annotation.Minimum, true);
                var clamp = lengthConstraint_.LowerBound.Match(some: b => Bound.ClampLower(b, lower), none: () => lower);
                lengthConstraint_ = lengthConstraint_ with { LowerBound = Option.Some(clamp) };
            }
            if (annotation.Maximum != long.MaxValue) {
                var upper = new Bound((int)annotation.Maximum, true);
                var clamp = lengthConstraint_.UpperBound.Match(some: b => Bound.ClampUpper(b, upper), none: () => upper);
                lengthConstraint_ = lengthConstraint_ with { UpperBound = Option.Some(clamp) };
            }

            if (!lengthConstraint_.IsValid()) {
                throw new UnsatisfiableConstraintException(context, annotation, lengthConstraint_);
            }
        }

        protected sealed override IEnumerable<CheckConstraint> GetCheckConstraints(IField field, Settings settings) {
            foreach (var constraint in base.GetCheckConstraints(field, settings)) {
                yield return constraint;
            }
            if (HasAllowedValues) {
                // If there is a one-of CHECK constraint, then we don't want to produce the string length constraints:
                // they were used to filter down the allowable values already
                yield break;
            }

            if (lengthConstraint_.LowerBound.Exists(bl => lengthConstraint_.UpperBound.Exists(bu => bu == bl))) {
                var bound = lengthConstraint_.LowerBound.Unwrap();
                var expr = new FieldExpression(FieldFunction.LengthOf, field);
                var op = ComparisonOperator.EQ;
                var clause = new ConstantClause(expr, op, DBValue.Create(bound.Value));
                yield return new CheckConstraint(clause);
            }
            else {
                if (lengthConstraint_.LowerBound.HasValue) {
                    var bound = lengthConstraint_.LowerBound.Unwrap();
                    var expr = new FieldExpression(FieldFunction.LengthOf, field);
                    var clause = new ConstantClause(expr, ComparisonOperator.GTE, DBValue.Create(bound.Value));
                    yield return new CheckConstraint(clause);
                }
                if (lengthConstraint_.UpperBound.HasValue) {
                    var bound = lengthConstraint_.UpperBound.Unwrap();
                    var expr = new FieldExpression(FieldFunction.LengthOf, field);
                    var clause = new ConstantClause(expr, ComparisonOperator.LTE, DBValue.Create(bound.Value));
                    yield return new CheckConstraint(clause);
                }
            }
        }

        protected sealed override bool IsValidValue(object? value) {
            return base.IsValidValue(value) && (value is null || lengthConstraint_.Contains(((string)value).Length));
        }

        private StringFieldDescriptor(StringFieldDescriptor source)
            : base(source) {

            lengthConstraint_ = source.lengthConstraint_;
        }

        private StringFieldDescriptor(StringFieldDescriptor source, ResetTag _)
            : base(source, RESET) {

            lengthConstraint_ = new Interval(Option.None<Bound>(), Option.None<Bound>());
        }


        private Interval lengthConstraint_;
    }
}
