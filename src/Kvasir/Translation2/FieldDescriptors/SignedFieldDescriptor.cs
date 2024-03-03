using Kvasir.Annotations;
using System.Diagnostics;
using System.Reflection;

namespace Kvasir.Translation2 {
    /// <summary>
    ///   The concrete base class for a <see cref="OrderableFieldDescriptor"/> whose data type is a signed numeric (but
    ///   is not <see cref="decimal"/>.
    /// </summary>
    internal sealed class SignedFieldDescriptor : OrderableFieldDescriptor {
        public SignedFieldDescriptor(Context context, PropertyInfo source)
            : base(context, source) {

            Debug.Assert(
                FieldType == typeof(sbyte) ||
                FieldType == typeof(short) ||
                FieldType == typeof(int) ||
                FieldType == typeof(long) ||
                FieldType == typeof(float) ||
                FieldType == typeof(double)
            );
        }

        public SignedFieldDescriptor(Context context, PropertyInfo source, DataConverterAttribute annotation)
            : base(context, source, annotation) {

            Debug.Assert(
                FieldType == typeof(sbyte) ||
                FieldType == typeof(short) ||
                FieldType == typeof(int) ||
                FieldType == typeof(long) ||
                FieldType == typeof(float) ||
                FieldType == typeof(double)
            );
        }

        protected sealed override SignedFieldDescriptor Clone() {
            return new SignedFieldDescriptor(this);
        }

        private SignedFieldDescriptor(SignedFieldDescriptor source)
            : base(source) {}
    }
}
