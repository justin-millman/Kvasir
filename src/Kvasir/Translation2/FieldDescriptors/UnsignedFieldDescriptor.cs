using Kvasir.Annotations;
using System.Diagnostics;
using System.Reflection;

namespace Kvasir.Translation2 {
    /// <summary>
    ///   The concrete base class for an <see cref="OrderableFieldDescriptor"/> whose data type is an unsigned numeric.
    /// </summary>
    internal sealed class UnsignedFieldDescriptor : OrderableFieldDescriptor {
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
