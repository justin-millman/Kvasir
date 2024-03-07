using Cybele.Extensions;
using Kvasir.Annotations;
using Kvasir.Schema;
using Optional;
using System.Reflection;

namespace Kvasir.Translation2 {
    /// <summary>
    ///   The intermediate base class for a <see cref="FieldDescriptor"/> whose data type supports ordering.
    /// </summary>
    internal abstract class OrderableFieldDescriptor : FieldDescriptor {
        protected OrderableFieldDescriptor(OrderableFieldDescriptor source)
            : base(source) {

            comparisonConstraint_ = source.comparisonConstraint_;
        }

        protected OrderableFieldDescriptor(Context context, PropertyInfo source)
            : base(context,source) {

            comparisonConstraint_ = new Interval(Option.None<Bound>(), Option.None<Bound>());
        }

        protected OrderableFieldDescriptor(Context context, PropertyInfo source, DataConverterAttribute annotation)
            : base(context, source, annotation) {

            comparisonConstraint_ = new Interval(Option.None<Bound>(), Option.None<Bound>());
        }

        protected sealed override void DoApplyConstraint(Context context, Check.ComparisonAttribute annotation) {
            if (annotation.Anchor is null) {
                object? n = null;
                throw new InvalidConstraintValueException(context, annotation, $"constraint value cannot be {n.ForDisplay()}");
            }

            var anchor = CoerceUserValue(annotation.Anchor).Match(
                some: v => v!,
                none: msg => throw new InvalidConstraintValueException(context, annotation, msg)
            );

            if (annotation.Operator == ComparisonOperator.NE) {
                var isnot = new Check.IsNotOneOfAttribute(anchor);
                DoApplyConstraint(context, isnot);
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
        }

        protected override bool IsValidValue(object? value) {
            return base.IsValidValue(value) && (value is null || comparisonConstraint_.Contains(value));
        }


        private Interval comparisonConstraint_;
    }
}
