using Cybele.Extensions;
using Kvasir.Annotations;
using Optional;
using System;
using System.Diagnostics;
using System.Reflection;

namespace Kvasir.Translation2 {
    /// <summary>
    ///   The concrete class for an <see cref="OrderableFieldDescriptor"/> whose data type is <see cref="DateTime"/>.
    /// </summary>
    internal sealed class DateTimeFieldDescriptor : OrderableFieldDescriptor {
        public DateTimeFieldDescriptor(Context context, PropertyInfo source)
            : base(context, source) {

            Debug.Assert(FieldType == typeof(DateTime));
        }

        public DateTimeFieldDescriptor(Context context, PropertyInfo source, DataConverterAttribute annotation)
            : base(context, source, annotation) {

            Debug.Assert(FieldType == typeof(DateTime));
        }

        public sealed override DateTimeFieldDescriptor Clone() {
            return new DateTimeFieldDescriptor(this);
        }

        public sealed override DateTimeFieldDescriptor Reset() {
            return new DateTimeFieldDescriptor(this, RESET);
        }

        protected sealed override Option<object?, string> CoerceUserValue(object? raw) {
            if (raw is null || raw == DBNull.Value) {
                return Option.Some<object?, string>(null);
            }
            else if (raw.GetType() == typeof(string)) {
                // Values for a DateTime must be strings that can be parsed into a DateTime; we rely on the DateTime
                // class's parsing logic to deal with formatting, acceptable dates, leap day calculations, etc.
                if (!DateTime.TryParse((string)raw, out DateTime coercion)) {
                    var msg = $"unable to parse {typeof(string).DisplayName()} value {raw.ForDisplay()} as a {typeof(DateTime).DisplayName()}";
                    return Option.None<object?, string>(msg);
                }
                return Option.Some<object?, string>(coercion);
            }
            else if (raw.GetType().IsArray) {
                var msg = "value cannot be an array";
                return Option.None<object?, string>(msg);
            }
            else {
                var msg = $"value {raw.ForDisplay()} is of type {raw.GetType().DisplayName()}, not {typeof(string).DisplayName()} as expected";
                return Option.None<object?, string>(msg);
            }
        }

        private DateTimeFieldDescriptor(DateTimeFieldDescriptor source)
            : base(source) {}

        private DateTimeFieldDescriptor(DateTimeFieldDescriptor source, ResetTag _)
            : base(source, RESET) {}
    }
}
