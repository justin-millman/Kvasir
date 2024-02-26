using System.Diagnostics;
using System.Reflection;

namespace Kvasir.Translation2 {
    ///
    internal sealed class DecimalFieldDescriptor : NumericFieldDescriptor {
        ///
        public DecimalFieldDescriptor(PropertyInfo source)
            : base(source) {

            Debug.Assert(FieldType == typeof(decimal));
        }

        ///
        protected sealed override DecimalFieldDescriptor Clone() {
            return new DecimalFieldDescriptor(this);
        }

        ///
        private DecimalFieldDescriptor(DecimalFieldDescriptor source)
            : base(source) { }
    }
}
