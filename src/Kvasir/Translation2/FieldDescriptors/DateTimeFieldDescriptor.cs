using Kvasir.Annotations;
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

        protected sealed override DateTimeFieldDescriptor Clone() {
            return new DateTimeFieldDescriptor(this);
        }

        private DateTimeFieldDescriptor(DateTimeFieldDescriptor source)
            : base(source) {}
    }
}
