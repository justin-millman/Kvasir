using Kvasir.Annotations;
using System.Diagnostics;
using System.Reflection;

namespace Kvasir.Translation2 {
    /// <summary>
    ///   The concrete class for an <see cref="OrderableFieldDescriptor"/> whose data type is <see cref="char"/>.
    /// </summary>
    internal sealed class CharFieldDescriptor : OrderableFieldDescriptor {
        public CharFieldDescriptor(Context context, PropertyInfo source)
            : base(context, source) {

            Debug.Assert(FieldType == typeof(bool) || FieldType == typeof(char));
        }

        public CharFieldDescriptor(Context context, PropertyInfo source, DataConverterAttribute annotation)
            : base(context, source, annotation) {

            Debug.Assert(FieldType == typeof(char));
        }

        public sealed override CharFieldDescriptor Clone() {
            return new CharFieldDescriptor(this);
        }

        private CharFieldDescriptor(CharFieldDescriptor source)
            : base(source) {}
    }
}
