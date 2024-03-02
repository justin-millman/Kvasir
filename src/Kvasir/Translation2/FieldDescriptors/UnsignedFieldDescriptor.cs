using Kvasir.Annotations;
using System.Diagnostics;
using System.Reflection;

namespace Kvasir.Translation2 {
    /// <summary>
    ///   The concrete base class for a <see cref="NumericFieldDescriptor"/> whose data type is unsigned.
    /// </summary>
    internal sealed class UnsignedFieldDescriptor : NumericFieldDescriptor {
        public UnsignedFieldDescriptor(Context context, PropertyInfo source)
            : base(context, source) {

            Debug.Assert(
                FieldType == typeof(byte) ||
                FieldType == typeof(ushort) ||
                FieldType == typeof(uint) ||
                FieldType == typeof(ulong)
            );
        }

        public UnsignedFieldDescriptor(Context context, PropertyInfo source, DataConverterAttribute annotation)
            : base(context, source, annotation) {

            Debug.Assert(
                FieldType == typeof(byte) ||
                FieldType == typeof(ushort) ||
                FieldType == typeof(uint) ||
                FieldType == typeof(ulong)
            );
        }

        protected sealed override UnsignedFieldDescriptor Clone() {
            return new UnsignedFieldDescriptor(this);
        }

        private UnsignedFieldDescriptor(UnsignedFieldDescriptor source)
            : base(source) {}
    }
}
