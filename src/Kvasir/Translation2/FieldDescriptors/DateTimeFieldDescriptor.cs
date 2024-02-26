using System;
using System.Diagnostics;
using System.Reflection;

namespace Kvasir.Translation2 {
    ///
    internal sealed class DateTimeFieldDescriptor : OrderableFieldDescriptor {
        ///
        public DateTimeFieldDescriptor(PropertyInfo source)
            : base(source) {

            Debug.Assert(FieldType == typeof(DateTime));
        }

        ///
        protected sealed override DateTimeFieldDescriptor Clone() {
            return new DateTimeFieldDescriptor(this);
        }

        ///
        private DateTimeFieldDescriptor(DateTimeFieldDescriptor source)
            : base(source) {}
    }
}
