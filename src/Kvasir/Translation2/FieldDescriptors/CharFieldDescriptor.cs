using System.Diagnostics;
using System.Reflection;

namespace Kvasir.Translation2 {
    ///
    internal sealed class CharFieldDescriptor : OrderableFieldDescriptor {
        ///
        public CharFieldDescriptor(PropertyInfo source)
            : base(source) {

            Debug.Assert(FieldType == typeof(char));
        }

        ///
        protected sealed override CharFieldDescriptor Clone() {
            return new CharFieldDescriptor(this);
        }

        ///
        private CharFieldDescriptor(CharFieldDescriptor source)
            : base(source) { }
    }
}
