using Kvasir.Annotations;
using System.Diagnostics;
using System.Reflection;

namespace Kvasir.Translation2 {
    /// <summary>
    ///   The concrete base class for a <see cref="FieldDescriptor"/> whose data type is <see cref="bool"/>.
    /// </summary>
    internal sealed class BooleanFieldDescriptor : FieldDescriptor {
        public BooleanFieldDescriptor(Context context, PropertyInfo source)
            : base(context, source) {

            Debug.Assert(FieldType == typeof(bool));
        }

        public BooleanFieldDescriptor(Context context, PropertyInfo source, DataConverterAttribute annotation)
            : base(context, source, annotation) {

            Debug.Assert(FieldType == typeof(bool) || FieldType == typeof(char));
        }

        public sealed override BooleanFieldDescriptor Clone() {
            return new BooleanFieldDescriptor(this);
        }

        private BooleanFieldDescriptor(BooleanFieldDescriptor source)
            : base(source) {}
    }
}
