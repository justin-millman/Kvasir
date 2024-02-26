using Cybele.Extensions;
using Kvasir.Annotations;
using Optional;
using System;
using System.Diagnostics;
using System.Reflection;

namespace Kvasir.Translation2 {
    /// <summary>
    ///   The concrete class for a <see cref="FieldDescriptor"/> whose data type is <see cref="Guid"/>.
    /// </summary>
    internal sealed class GuidFieldDescriptor : FieldDescriptor {
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
            if (raw is null || raw == DBNull.Value) {
                return Option.Some<object?, string>(null);
            }
            else if (raw.GetType() == typeof(string)) {
                // Values for a Guid must be strings that can be parsed into a Guid; we rely on the Guid class's parsing
                // logic to deal with formatting, version management, etc.
                if (!Guid.TryParse((string)raw, out Guid coercion)) {
                    var msg = $"unable to parse {typeof(string).DisplayName()} value {raw.ForDisplay()} as a {typeof(Guid).DisplayName()}";
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

        private GuidFieldDescriptor(GuidFieldDescriptor source)
            : base(source) {}

        private GuidFieldDescriptor(GuidFieldDescriptor source, ResetTag _)
            : base(source, RESET) {}
    }
}
