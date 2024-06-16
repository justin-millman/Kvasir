using Cybele.Extensions;
using Kvasir.Annotations;
using Optional;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Kvasir.Translation {
    /// <summary>
    ///   The concrete class for a <see cref="FieldDescriptor"/> whose data type is an enumeration.
    /// </summary>
    internal sealed class EnumFieldDescriptor : FieldDescriptor {
        public EnumFieldDescriptor(Context context, PropertyInfo source)
            : base(context, source) {

            Debug.Assert(FieldType.IsEnum);
            SetDomain(FieldType.ValidValues());
        }

        public EnumFieldDescriptor(Context context, PropertyInfo source, DataConverterAttribute annotation)
            : base(context, source, annotation) {

            Debug.Assert(FieldType.IsEnum);

            // If there's a Data Converter and we still have an Enum Field, that means the result type of the data
            // conversion is an enumeration. If the source type is also an enumeration, then we form the restricted
            // image by feeding each of the original enumerators through the Data Converter. But if the source type is
            // not an enumeration, then we simply assume that each of the enumerators of the result type are possible
            // (even if the Data Converter, for example, maps all inputs to a single value).
            var sanitizedPropType = Nullable.GetUnderlyingType(source.PropertyType) ?? source.PropertyType;
            if (sanitizedPropType.IsEnum) {
                var conv = annotation.DataConverter;
                SetDomain(sanitizedPropType.ValidValues().Select(e => conv.TryConvert(e, context)!));
            }
            else {
                SetDomain(FieldType.ValidValues());
            }
        }

        public sealed override EnumFieldDescriptor Clone() {
            return new EnumFieldDescriptor(this);
        }

        public sealed override EnumFieldDescriptor Reset() {
            var reset = new EnumFieldDescriptor(this, RESET);

            // Discreteness constraints are considered non-transferable, but the allowed values of an Enumeration need
            // to be carried with the reset
            var enumerators = FieldType.ValidValues().Cast<object>().ToArray();
            var context = new Context(typeof(DBNull));              // Context is irrelevant here
            reset.DoApplyConstraint(context, new Check.IsOneOfAttribute(enumerators[0], enumerators[1..]));
            return reset;
        }

        protected sealed override Option<object?, string> CoerceUserValue(object? raw) {
            var coercion = base.CoerceUserValue(raw);
            return coercion.Filter(v => v is null || ((Enum)v).IsValid(), $"enumerator {raw.ForDisplay()} is not valid");
        }

        private EnumFieldDescriptor(EnumFieldDescriptor source)
            : base(source) {}

        private EnumFieldDescriptor(EnumFieldDescriptor source, ResetTag _)
            : base(source, RESET) {}
    }
}
