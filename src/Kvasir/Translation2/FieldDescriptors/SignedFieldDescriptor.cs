using System.Diagnostics;
using System.Reflection;

namespace Kvasir.Translation2 {
    ///
    internal sealed class SignedFieldDescriptor : NumericFieldDescriptor {
        ///
        public SignedFieldDescriptor(PropertyInfo source)
            : base(source) {

            Debug.Assert(
                FieldType == typeof(sbyte) ||
                FieldType == typeof(short) ||
                FieldType == typeof(int) ||
                FieldType == typeof(long) ||
                FieldType == typeof(float) ||
                FieldType == typeof(double)
            );
        }

        ///
        protected sealed override SignedFieldDescriptor Clone() {
            return new SignedFieldDescriptor(this);
        }

        ///
        private SignedFieldDescriptor(SignedFieldDescriptor source)
            : base(source) {}
    }
}
