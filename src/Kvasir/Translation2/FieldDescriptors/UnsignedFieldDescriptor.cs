using System.Diagnostics;
using System.Reflection;

namespace Kvasir.Translation2 {
    ///
    internal sealed class UnignedFieldDescriptor : NumericFieldDescriptor {
        ///
        public UnignedFieldDescriptor(PropertyInfo source)
            : base(source) {

            Debug.Assert(
                FieldType == typeof(byte) ||
                FieldType == typeof(ushort) ||
                FieldType == typeof(uint) ||
                FieldType == typeof(ulong)
            );
        }

        ///
        protected sealed override UnignedFieldDescriptor Clone() {
            return new UnignedFieldDescriptor(this);
        }

        ///
        private UnignedFieldDescriptor(UnignedFieldDescriptor source)
            : base(source) {}
    }
}
