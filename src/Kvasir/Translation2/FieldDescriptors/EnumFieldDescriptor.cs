using Cybele.Extensions;
using Kvasir.Annotations;
using Optional;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Kvasir.Translation2 {
    /// <summary>
    ///   The concrete class for a <see cref="FieldDescriptor"/> whose data type is an enumeration.
    /// </summary>
    internal sealed class EnumFieldDescriptor : FieldDescriptor {
        public EnumFieldDescriptor(Context context, PropertyInfo source)
            : base(context, source) {

            Debug.Assert(FieldType.IsEnum);
            restrictedImage_ = FieldType.ValidValues().Cast<object>().ToHashSet();
        }

        public EnumFieldDescriptor(Context context, PropertyInfo source, DataConverterAttribute annotation)
            : base(context, source, annotation) {

            Debug.Assert(FieldType.IsEnum);
            
            // If there's a Data Converter and we still have an Enum Field, that means the result type of the data
            // conversion is an enumeration. If the source type is also an enumeration, then we form the restricted
            // image by feeding each of the original enumerators through the Data Converter. But if the source type is
            // not an enumeration, then we simply assume that each of the enumerators of the result type are possible
            // (even if the Data Converter, for example, maps all inputs to a single value).
            if (source.PropertyType.IsEnum) {
                var conv = annotation.DataConverter;
                restrictedImage_ = source.PropertyType.ValidValues().Select(e => conv.Convert(e)!).ToHashSet();
            }
            else {
                restrictedImage_ = FieldType.ValidValues().Cast<object>().ToHashSet();
            }
        }

        protected sealed override EnumFieldDescriptor Clone() {
            return new EnumFieldDescriptor(this);
        }

        protected sealed override Option<object?, string> CoerceUserValue(object? raw) {
            var coercion = base.CoerceUserValue(raw);
            coercion.Filter(v => v is not null && !((Enum)v).IsValid(), $"enumerator {raw.ForDisplay()} is not valid");
            return coercion;
        }

        protected sealed override bool IsValidValue(object? value) {
            return base.IsValidValue(value) && (value is null || restrictedImage_.Contains(value));
        }

        private EnumFieldDescriptor(EnumFieldDescriptor source)
            : base(source) {

            restrictedImage_ = new HashSet<object>(source.restrictedImage_);
        }


        private readonly HashSet<object> restrictedImage_;
    }
}
