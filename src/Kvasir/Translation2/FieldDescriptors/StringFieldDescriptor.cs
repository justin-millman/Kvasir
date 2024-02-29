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
            lengthConstraint_ = new Interval(Option.None<Bound>(), Option.None<Bound>());
        }

        ///
        protected sealed override StringFieldDescriptor Clone() {
            return new StringFieldDescriptor(this);
        }

        ///
        private StringFieldDescriptor(StringFieldDescriptor source)
            : base(source) {

            lengthConstraint_ = source.lengthConstraint_;
        }



        private readonly Interval lengthConstraint_;
    }
}
