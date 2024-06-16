using Cybele.Core;
using Cybele.Extensions;
using Kvasir.Annotations;
using Kvasir.Core;
using Kvasir.Schema;
using Optional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Kvasir.Translation {
    /// <summary>
    ///   The intermediate base class for a <see cref="FieldDescriptor"/> whose data type supports ordering.
    /// </summary>
    internal abstract class OrderableFieldDescriptor : FieldDescriptor {
        protected OrderableFieldDescriptor(OrderableFieldDescriptor source)
            : base(source) {

            comparisonConstraint_ = source.comparisonConstraint_;
        }

        protected OrderableFieldDescriptor(OrderableFieldDescriptor source, ResetTag _)
            : base(source, RESET) {

            comparisonConstraint_ = new Interval(Option.None<Bound>(), Option.None<Bound>());
        }

        protected OrderableFieldDescriptor(Context context, PropertyInfo source)
            : base(context,source) {

            comparisonConstraint_ = new Interval(Option.None<Bound>(), Option.None<Bound>());
        }

        protected OrderableFieldDescriptor(Context context, PropertyInfo source, DataConverterAttribute annotation)
            : base(context, source, annotation) {

            comparisonConstraint_ = new Interval(Option.None<Bound>(), Option.None<Bound>());
        }

        public OrderableFieldDescriptor(Context context, PropertyInfo source, DataConverter converter)
            : base(context, source, converter) {

            comparisonConstraint_ = new Interval(Option.None<Bound>(), Option.None<Bound>());
        }

        protected sealed override void DoApplyConstraint(Context context, Check.ComparisonAttribute annotation) {
            if (annotation.Anchor is null || annotation.Anchor == DBNull.Value) {
                object? n = null;
                throw new InvalidConstraintValueException(context, annotation, $"the constraint boundary cannot be {n.ForDisplay()}");
            }

            var anchor = CoerceUserValue(annotation.Anchor).Match(
                some: v => v!,
                none: msg => throw new InvalidConstraintValueException(context, annotation, msg)
            );


            if (annotation.Operator == ComparisonOperator.LT && MINIMA.Contains(anchor)) {
                throw new InvalidConstraintValueException(context, annotation, "the constraint anchor cannot be the minimum possible value");
            }
            else if (annotation.Operator == ComparisonOperator.GT && MAXIMA.Contains(anchor)) {
                throw new InvalidConstraintValueException(context, annotation, "the constraint anchor cannot be the maximum possible value");
            }
            else if (annotation.Operator == ComparisonOperator.NE) {
                DoApplyNotEqualConstraint(context, annotation);
            }
            else if (annotation.Operator == ComparisonOperator.GT || annotation.Operator == ComparisonOperator.GTE) {
                var lower = new Bound(anchor, annotation.Operator == ComparisonOperator.GTE);
                var clamp = comparisonConstraint_.LowerBound.Match(some: b => Bound.ClampLower(b, lower), none: () => lower);
                comparisonConstraint_ = comparisonConstraint_ with { LowerBound = Option.Some(clamp) };
            }
            else {
                var upper = new Bound(anchor, annotation.Operator == ComparisonOperator.LTE);
                var clamp = comparisonConstraint_.UpperBound.Match(some: b => Bound.ClampUpper(b, upper), none: () => upper);
                comparisonConstraint_ = comparisonConstraint_ with { UpperBound = Option.Some(clamp) };
            }

            if (!comparisonConstraint_.IsValid()) {
                throw new UnsatisfiableConstraintException(context, annotation, comparisonConstraint_);
            }
            CheckForInvalidSingleElementInterval(context, annotation);
        }

        protected sealed override void DoApplyConstraint(Context context, Check.InclusionAttribute annotation) {
            base.DoApplyConstraint(context, annotation);
            CheckForInvalidSingleElementInterval(context, annotation);
        }

        protected override IEnumerable<CheckConstraint> GetCheckConstraints(IField field, Settings settings) {
            foreach (var constraint in base.GetCheckConstraints(field, settings)) {
                yield return constraint;
            }
            if (HasAllowedValues) {
                // If there is a one-of CHECK constraint, then we don't want to produce the comparison constraints: they
                // were used to filter down the allowable values already
                yield break;
            }

            if (comparisonConstraint_.LowerBound.Exists(bl => comparisonConstraint_.UpperBound.Exists(bu => bu == bl))) {
                var bound = comparisonConstraint_.LowerBound.Unwrap();
                var op = ComparisonOperator.EQ;
                var clause = new ConstantClause(new FieldExpression(field), op, DBValue.Create(bound.Value));
                yield return new CheckConstraint(clause);
            }
            else {
                if (comparisonConstraint_.LowerBound.HasValue) {
                    var bound = comparisonConstraint_.LowerBound.Unwrap();
                    var op = bound.IsInclusive ? ComparisonOperator.GTE : ComparisonOperator.GT;
                    var clause = new ConstantClause(new FieldExpression(field), op, DBValue.Create(bound.Value));
                    yield return new CheckConstraint(clause);
                }
                if (comparisonConstraint_.UpperBound.HasValue) {
                    var bound = comparisonConstraint_.UpperBound.Unwrap();
                    var op = bound.IsInclusive ? ComparisonOperator.LTE : ComparisonOperator.LT;
                    var clause = new ConstantClause(new FieldExpression(field), op, DBValue.Create(bound.Value));
                    yield return new CheckConstraint(clause);
                }
            }
        }

        protected override bool IsValidValue(object? value) {
            return base.IsValidValue(value) && (value is null || comparisonConstraint_.Contains(value));
        }

        private void CheckForInvalidSingleElementInterval(Context context, INestableAnnotation annotation) {
            // If the interval allows only one value, then it is an error if that value is invalid. We are conservative
            // with identifying this condition: only doubly-closed intervals can be identified, since otherwise we would
            // need some notion of an epsilon, which doesn't exist for e.g. strings.
            if (comparisonConstraint_.LowerBound.HasValue && comparisonConstraint_.UpperBound.HasValue) {
                var lower = comparisonConstraint_.LowerBound.Unwrap();
                var upper = comparisonConstraint_.UpperBound.Unwrap();

                if (lower.IsInclusive && upper.IsInclusive && lower.Value.Equals(upper.Value)) {
                    if (!IsValidValue(lower.Value)) {
                        throw new UnsatisfiableConstraintException(context, annotation);
                    }
                }
            }
        }


        private Interval comparisonConstraint_;

        private static readonly object[] MINIMA = new object[] {
            byte.MinValue, sbyte.MinValue,
            ushort.MinValue, short.MinValue,
            uint.MinValue, int.MinValue,
            ulong.MinValue, ulong.MinValue,
            double.MinValue, float.MinValue, decimal.MinValue
        };
        private static readonly object[] MAXIMA = new object[] {
            byte.MaxValue, sbyte.MaxValue,
            ushort.MaxValue, short.MaxValue,
            uint.MaxValue, int.MaxValue,
            ulong.MaxValue, ulong.MaxValue,
            double.MaxValue, float.MaxValue, decimal.MaxValue
        };
    }
}
