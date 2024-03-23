using Kvasir.Annotations;
using System.Diagnostics;
using System.Reflection;

namespace Kvasir.Translation2 {
    /// <summary>
    ///   The concrete base class for a <see cref="FieldDescriptor"/> whose data type does not allow any additional
    ///   constraints or have any special value-parsing behaviors.
    /// </summary>
    internal sealed class VanillaFieldDescriptor : FieldDescriptor {
        public VanillaFieldDescriptor(Context context, PropertyInfo source)
            : base(context, source) {

            Debug.Assert(FieldType == typeof(bool) || FieldType == typeof(char));
        }

        public VanillaFieldDescriptor(Context context, PropertyInfo source, DataConverterAttribute annotation)
            : base(context, source, annotation) {

            Debug.Assert(FieldType == typeof(bool) || FieldType == typeof(char));
        }

        public sealed override VanillaFieldDescriptor Clone() {
            return new VanillaFieldDescriptor(this);
        }

        private VanillaFieldDescriptor(VanillaFieldDescriptor source)
            : base(source) {}
    }
}
