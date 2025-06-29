using Cybele.Extensions;
using Kvasir.Annotations;
using Optional;
using System;
using System.Diagnostics;
using System.Reflection;

namespace Kvasir.Translation {
    /// <summary>
    ///   The concrete class for an <see cref="OrderableFieldDescriptor"/> whose data type is either
    ///   <see cref="DateOnly"/> or <see cref="DateTime"/>.
    /// </summary>
    internal sealed class CalendarFieldDescriptor : OrderableFieldDescriptor {
        protected sealed override Type UserValueType => typeof(string);

        public CalendarFieldDescriptor(Context context, PropertyInfo source)
            : base(context, source) {

            Debug.Assert(FieldType == typeof(DateOnly) || FieldType == typeof(DateTime));
            includeTimestap_ = (FieldType == typeof(DateTime));
        }

        public CalendarFieldDescriptor(Context context, PropertyInfo source, DataConverterAttribute annotation)
            : base(context, source, annotation) {

            Debug.Assert(FieldType == typeof(DateOnly) || FieldType == typeof(DateTime));
            includeTimestap_ = (FieldType == typeof(DateTime));
        }

        public sealed override CalendarFieldDescriptor Clone() {
            return new CalendarFieldDescriptor(this);
        }

        public sealed override CalendarFieldDescriptor Reset() {
            return new CalendarFieldDescriptor(this, RESET);
        }

        protected sealed override Option<object?, string> CoerceUserValue(object? raw) {
            var value = base.CoerceUserValue(raw);
            if (value.Exists(v => v is not null)) {
                value = value.FlatMap(v => {
                    // We rely on either the DateOnly class's parsing logic or the DateTime class's parsing logic to
                    // deal with formatting, acceptable dates, leap day calculations, etc.
                    if (includeTimestap_) {
                        if (!DateTime.TryParse((string)v!, out DateTime coercion)) {
                            var msg = $"unable to parse {typeof(string).DisplayName()} value {v.ForDisplay()} as a {typeof(DateTime).DisplayName()}";
                            return Option.None<object?, string>(msg);
                        }
                        return Option.Some<object?, string>(coercion);
                    }
                    else {
                        if (!DateOnly.TryParse((string)v!, out DateOnly coercion)) {
                            var msg = $"unable to parse {typeof(string).DisplayName()} value {v.ForDisplay()} as a {typeof(DateOnly).DisplayName()}";
                            return Option.None<object?, string>(msg);
                        }
                        return Option.Some<object?, string>(coercion);
                    }
                });
            }
            return value;
        }

        private CalendarFieldDescriptor(CalendarFieldDescriptor source)
            : base(source) {

            includeTimestap_ = source.includeTimestap_;
        }

        private CalendarFieldDescriptor(CalendarFieldDescriptor source, ResetTag _)
            : base(source, RESET) {

            includeTimestap_ = source.includeTimestap_;
        }


        public readonly bool includeTimestap_;
    }
}
