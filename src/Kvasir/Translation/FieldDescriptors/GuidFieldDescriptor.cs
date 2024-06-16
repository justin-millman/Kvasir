using Cybele.Extensions;
using Kvasir.Annotations;
using Optional;
using System;
using System.Diagnostics;
using System.Reflection;

namespace Kvasir.Translation {
    /// <summary>
    ///   The concrete class for a <see cref="FieldDescriptor"/> whose data type is <see cref="Guid"/>.
    /// </summary>
    internal sealed class GuidFieldDescriptor : FieldDescriptor {
        protected sealed override Type UserValueType => typeof(string);

        public GuidFieldDescriptor(Context context, PropertyInfo source)
            : base(context, source) {

            Debug.Assert(FieldType == typeof(Guid));
        }

        public GuidFieldDescriptor(Context context, PropertyInfo source, DataConverterAttribute annotation)
            : base(context, source, annotation) {

            Debug.Assert(FieldType == typeof(Guid));
        }

        public sealed override GuidFieldDescriptor Clone() {
            return new GuidFieldDescriptor(this);
        }

        public sealed override GuidFieldDescriptor Reset() {
            return new GuidFieldDescriptor(this, RESET);
        }

        protected sealed override Option<object?, string> CoerceUserValue(object? raw) {
            var value = base.CoerceUserValue(raw);
            if (value.Exists(v => v is not null)) {
                value = value.FlatMap(v => {
                    // We rely on the Guid class's parsing logic to deal with formatting, version management, etc.
                    if (!Guid.TryParse((string)v!, out Guid coercion)) {
                        var msg = $"unable to parse {typeof(string).DisplayName()} value {v.ForDisplay()} as a {typeof(Guid).DisplayName()}";
                        return Option.None<object?, string>(msg);
                    }
                    return Option.Some<object?, string>(coercion);
                });
            }
            return value;
        }

        private GuidFieldDescriptor(GuidFieldDescriptor source)
            : base(source) {}

        private GuidFieldDescriptor(GuidFieldDescriptor source, ResetTag _)
            : base(source, RESET) {}
    }
}
