using Kvasir.Annotations;
using System;
using System.Diagnostics;
using System.Reflection;

namespace Kvasir.Translation2 {
    /// <summary>
    ///   The concrete class for a <see cref="FieldDescriptor"/> whose data type is <see cref="Guid"/>.
    /// </summary>
    internal sealed class GuidFieldDescriptor : FieldDescriptor {
        public GuidFieldDescriptor(Context context, PropertyInfo source)
            : base(context, source) {

            Debug.Assert(FieldType == typeof(Guid));
        }

        public GuidFieldDescriptor(Context context, PropertyInfo source, DataConverterAttribute annotation)
            : base(context, source, annotation) {

            Debug.Assert(FieldType == typeof(Guid));
        }

        protected sealed override GuidFieldDescriptor Clone() {
            return new GuidFieldDescriptor(this);
        }

        private GuidFieldDescriptor(GuidFieldDescriptor source)
            : base(source) {}
    }
}
