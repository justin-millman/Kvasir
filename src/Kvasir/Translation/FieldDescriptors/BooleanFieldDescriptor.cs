using Kvasir.Annotations;
using System.Diagnostics;
using System.Reflection;

namespace Kvasir.Translation {
    /// <summary>
    ///   The concrete base class for a <see cref="FieldDescriptor"/> whose data type is <see cref="bool"/>.
    /// </summary>
    internal sealed class BooleanFieldDescriptor : FieldDescriptor {
        public BooleanFieldDescriptor(Context context, PropertyInfo source)
            : base(context, source) {

            Debug.Assert(FieldType == typeof(bool));
            SetDomain(new object[] { true, false });
        }

        public BooleanFieldDescriptor(Context context, PropertyInfo source, DataConverterAttribute annotation)
            : base(context, source, annotation) {

            Debug.Assert(FieldType == typeof(bool));

            var converter = annotation.DataConverter;
            SetDomain(new object[] { converter.Convert(true)!, converter.Convert(false)! });
        }

        public sealed override BooleanFieldDescriptor Clone() {
            return new BooleanFieldDescriptor(this);
        }

        public sealed override BooleanFieldDescriptor Reset() {
            return new BooleanFieldDescriptor(this, RESET);
        }

        private BooleanFieldDescriptor(BooleanFieldDescriptor source)
            : base(source) {}

        private BooleanFieldDescriptor(BooleanFieldDescriptor source, ResetTag _)
            : base(source, RESET) {}
    }
}
