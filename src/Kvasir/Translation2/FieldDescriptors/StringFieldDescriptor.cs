using Optional;
using System.Diagnostics;
using System.Reflection;

namespace Kvasir.Translation2 {
    ///
    internal sealed class StringFieldDescriptor : OrderableFieldDescriptor {
        ///
        public StringFieldDescriptor(PropertyInfo source)
            : base(source) {

            Debug.Assert(FieldType == typeof(string));
            minimumLength_ = Option.None<Bound>();
            maximumLength_ = Option.None<Bound>();
        }

        ///
        protected sealed override StringFieldDescriptor Clone() {
            return new StringFieldDescriptor(this);
        }

        ///
        private StringFieldDescriptor(StringFieldDescriptor source)
            : base(source) {

            minimumLength_ = source.minimumLength_;
            maximumLength_ = source.maximumLength_;
        }


        private readonly Option<Bound> minimumLength_;
        private readonly Option<Bound> maximumLength_;
    }
}
