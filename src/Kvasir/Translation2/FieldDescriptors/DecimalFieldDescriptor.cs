using Kvasir.Annotations;
using System.Diagnostics;
using System.Reflection;

namespace Kvasir.Translation2 {
    /// <summary>
    ///   The concrete base class for a <see cref="NumericFieldDescriptor"/> whose data type is <see cref="decimal"/>.
    /// </summary>
    internal sealed class DecimalFieldDescriptor : NumericFieldDescriptor {
        public DecimalFieldDescriptor(Context context, PropertyInfo source)
            : base(context, source) {

            Debug.Assert(FieldType == typeof(decimal));
        }

        public DecimalFieldDescriptor(Context context, PropertyInfo source, DataConverterAttribute annotation)
            : base(context, source, annotation) {

            Debug.Assert(FieldType == typeof(decimal));
        }

        protected sealed override DecimalFieldDescriptor Clone() {
            return new DecimalFieldDescriptor(this);
        }

        private DecimalFieldDescriptor(DecimalFieldDescriptor source)
            : base(source) { }
    }
}
