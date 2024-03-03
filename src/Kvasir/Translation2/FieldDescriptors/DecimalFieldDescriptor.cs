using Cybele.Extensions;
using Kvasir.Annotations;
using Optional;
using System;
using System.Diagnostics;
using System.Reflection;

namespace Kvasir.Translation2 {
    /// <summary>
    ///   The concrete base class for a <see cref="NumericFieldDescriptor"/> whose data type is <see cref="decimal"/>.
    /// </summary>
    internal sealed class DecimalFieldDescriptor : NumericFieldDescriptor {
        public DecimalFieldDescriptor(Context context, PropertyInfo source)
            : base(context, source) {

            Debug.Assert(FieldType == typeof(decimal));
        }

        public DecimalFieldDescriptor(Context context, PropertyInfo source, DataConverterAttribute annotation)
            : base(context, source, annotation) {

            Debug.Assert(FieldType == typeof(decimal));
        }

        protected sealed override DecimalFieldDescriptor Clone() {
            return new DecimalFieldDescriptor(this);
        }

        protected sealed override Option<object?, string> CoerceUserValue(object? raw) {
            if (raw is null) {
                return Option.Some<object?, string>(null);
            }
            else if (raw.GetType() == typeof(double)) {
                // Values for a decimal must be doubles that can be converted into a decimal, which has a larger range
                // of supported values than does decimal
                var dbl = (double)raw;
                if (dbl < (double)decimal.MinValue || dbl > (double)decimal.MaxValue) {
                    var msg = $"double {raw.ForDisplay()} is outside the supported range for decimals";
                    return Option.None<object?, string>(msg);
                }
                return Option.Some<object?, string>(dbl);
            }
            else if (raw.GetType().IsArray) {
                var msg = "value cannot be an array";
                return Option.None<object?, string>(msg);
            }
            else {
                var msg = $"value {raw.ForDisplay()} is of type {raw.GetType().Name}, not string as expected";
                return Option.None<object?, string>(msg);
            }
        }

        private DecimalFieldDescriptor(DecimalFieldDescriptor source)
            : base(source) {}
    }
}
