using System;
using System.Diagnostics;
using System.Reflection;

namespace Kvasir.Translation2 {
    ///
    internal sealed class GuidFieldDescriptor : FieldDescriptor {
        ///
        public GuidFieldDescriptor(PropertyInfo source)
            : base(source) {

            Debug.Assert(FieldType == typeof(Guid));
        }

        ///
        protected sealed override GuidFieldDescriptor Clone() {
            return new GuidFieldDescriptor(this);
        }

        ///
        private GuidFieldDescriptor(GuidFieldDescriptor source)
            : base(source) {}
    }
}
