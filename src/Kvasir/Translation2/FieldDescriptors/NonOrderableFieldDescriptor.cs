using System.Reflection;

namespace Kvasir.Translation2 {
    ///
    internal abstract class NonOrderableFieldDescriptor : FieldDescriptor {
        ///
        public NonOrderableFieldDescriptor(PropertyInfo source)
            : base(source) {}

        ///
        protected NonOrderableFieldDescriptor(NonOrderableFieldDescriptor source)
            : base(source) {}
    }
}
