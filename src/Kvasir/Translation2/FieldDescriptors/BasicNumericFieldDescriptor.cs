using System;
using System.Reflection;    

namespace Kvasir.Translation2 {
    ///
    internal sealed class BasicNumericFieldDescriptor : NumericFieldDescriptor {
        ///
        public BasicNumericFieldDescriptor(PropertyInfo source)
            : base(source) {}

        ///
        protected sealed override BasicNumericFieldDescriptor Clone() {
            return new BasicNumericFieldDescriptor(this);
        }

        ///
        private BasicNumericFieldDescriptor(BasicNumericFieldDescriptor source)
            : base(source) {}
    }
}
