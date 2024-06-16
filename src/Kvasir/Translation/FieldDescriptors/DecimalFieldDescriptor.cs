using Cybele.Extensions;
using Kvasir.Annotations;
using Kvasir.Schema;
using Optional;
using System;
using System.Diagnostics;
using System.Reflection;

namespace Kvasir.Translation {
    /// <summary>
    ///   The concrete base class for an <see cref="OrderableFieldDescriptor"/> whose data type is
    ///   <see cref="decimal"/>.
    /// </summary>
    internal sealed class DecimalFieldDescriptor : OrderableFieldDescriptor {
        protected sealed override Type UserValueType => typeof(double);

        public DecimalFieldDescriptor(Context context, PropertyInfo source)
            : base(context, source) {

            Debug.Assert(FieldType == typeof(decimal));
        }

        public DecimalFieldDescriptor(Context context, PropertyInfo source, DataConverterAttribute annotation)
            : base(context, source, annotation) {

            Debug.Assert(FieldType == typeof(decimal));
        }

        public sealed override DecimalFieldDescriptor Clone() {
            return new DecimalFieldDescriptor(this);
        }

        public sealed override DecimalFieldDescriptor Reset() {
            return new DecimalFieldDescriptor(this, RESET);
        }

        protected sealed override Option<object?, string> CoerceUserValue(object? raw) {
            var value = base.CoerceUserValue(raw);
            if (value.Exists(v => v is not null)) {
                value = value.FlatMap(v => {
                    var dbl = (double)v!;
                    if (dbl < (double)decimal.MinValue || dbl > (double)decimal.MaxValue) {
                        var msg = $"{typeof(double).DisplayName()} {v.ForDisplay()} is outside the supported range for {typeof(decimal).DisplayName()}";
                        return Option.None<object?, string>(msg);
                    }
                    return Option.Some<object?, string>((decimal)dbl);
                });
            }
            return value;
        }

        protected sealed override void DoApplyConstraint(Context context, Check.SignednessAttribute annotation) {
            switch (annotation.Operator) {
                case ComparisonOperator.LT:
                    DoApplyConstraint(context, new Check.IsLessThanAttribute(0.0));
                    break;
                case ComparisonOperator.GT:
                    DoApplyConstraint(context, new Check.IsGreaterThanAttribute(0.0));
                    break;
                case ComparisonOperator.NE:
                    DoApplyConstraint(context, new Check.IsNotAttribute(0.0));
                    break;
                default:
                    throw new ApplicationException($"Switch statement over {nameof(ComparisonOperator)} exhausted");
            }
        }

        private DecimalFieldDescriptor(DecimalFieldDescriptor source)
            : base(source) {}

        private DecimalFieldDescriptor(DecimalFieldDescriptor source, ResetTag _)
            : base(source, RESET) {}
    }
}
