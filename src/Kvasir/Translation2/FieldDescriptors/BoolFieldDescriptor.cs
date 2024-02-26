using System.Diagnostics;
using System.Reflection;

namespace Kvasir.Translation2 {
    ///
    internal sealed class BoolFieldDescriptor : NonOrderableFieldDescriptor {
        ///
        public BoolFieldDescriptor(PropertyInfo source)
            : base(source) {

            Debug.Assert(FieldType == typeof(bool));
        }

        ///
        protected sealed override BoolFieldDescriptor Clone() {
            return new BoolFieldDescriptor(this);
        }

        ///
        private BoolFieldDescriptor(BoolFieldDescriptor source)
            : base(source) {}
    }
}
