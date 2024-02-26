using System.Reflection;

namespace Kvasir.Translation2 {
    ///
    internal sealed class BasicNonOrderableFieldDescriptor : FieldDescriptor {
        ///
        public BasicNonOrderableFieldDescriptor(PropertyInfo source)
            : base(source) {}

        ///
        protected sealed override BasicNonOrderableFieldDescriptor Clone() {
            return new BasicNonOrderableFieldDescriptor(this);
        }

        ///
        private BasicNonOrderableFieldDescriptor(BasicNonOrderableFieldDescriptor source)
            : base(source) {}
    }
}
