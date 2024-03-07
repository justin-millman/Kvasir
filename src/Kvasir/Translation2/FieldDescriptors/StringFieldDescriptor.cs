using Kvasir.Annotations;
using Optional;
using System.Diagnostics;
using System.Reflection;

namespace Kvasir.Translation2 {
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

        protected sealed override StringFieldDescriptor Clone() {
            return new StringFieldDescriptor(this);
        }

        protected sealed override void DoApplyConstraint(Context context, Check.StringLengthAttribute annotation) {
            // Rather than using Optionals, the Check.StringLengthAttribute uses long.MinValue and long.MaxValue to
            // indicate absent values. Additionally, the values stored in the long are _actually_ ints, that way the
            // min/max value are guaranteed never to match what the user might actually provide.

            if (annotation.Minimum != long.MinValue && annotation.Minimum < 0) {
                var msg = $"minimum string length {annotation.Minimum} is negative";
                throw new UnsatisfiableConstraintException(context, annotation, msg);
            }
            if (annotation.Maximum < 0) {
                var msg = $"maximum string length {annotation.Maximum} is negative";
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

        protected sealed override bool IsValidValue(object? value) {
            return base.IsValidValue(value) && (value is null || lengthConstraint_.Contains(((string)value).Length));
        }

        private StringFieldDescriptor(StringFieldDescriptor source)
            : base(source) {

            lengthConstraint_ = source.lengthConstraint_;
        }


        private Interval lengthConstraint_;
    }
}
