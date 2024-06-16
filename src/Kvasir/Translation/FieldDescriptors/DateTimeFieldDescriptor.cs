using Cybele.Extensions;
using Kvasir.Annotations;
using Optional;
using System;
using System.Diagnostics;
using System.Reflection;

namespace Kvasir.Translation {
    /// <summary>
    ///   The concrete class for an <see cref="OrderableFieldDescriptor"/> whose data type is <see cref="DateTime"/>.
    /// </summary>
    internal sealed class DateTimeFieldDescriptor : OrderableFieldDescriptor {
        protected sealed override Type UserValueType => typeof(string);

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
            var value = base.CoerceUserValue(raw);
            if (value.Exists(v => v is not null)) {
                value = value.FlatMap(v => {
                    // We rely on the DateTime lass's parsing logic to deal with formatting, acceptable dates, leap day
                    // calculations, etc.
                    if (!DateTime.TryParse((string)v!, out DateTime coercion)) {
                        var msg = $"unable to parse {typeof(string).DisplayName()} value {v.ForDisplay()} as a {typeof(DateTime).DisplayName()}";
                        return Option.None<object?, string>(msg);
                    }
                    return Option.Some<object?, string>(coercion);
                });
            }
            return value;
        }

        private DateTimeFieldDescriptor(DateTimeFieldDescriptor source)
            : base(source) {}

        private DateTimeFieldDescriptor(DateTimeFieldDescriptor source, ResetTag _)
            : base(source, RESET) {}
    }
}
