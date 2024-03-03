using Kvasir.Annotations;
using Optional;
using System.Diagnostics;
using System.Reflection;

namespace Kvasir.Translation2 {
    /// <summary>
    ///   The concrete base class for an <see cref="OrderableFieldDescriptor"/> whose data type is <see cref="string"/>.
    /// </summary>
    internal sealed class StringFieldDescriptor : OrderableFieldDescriptor {
        public StringFieldDescriptor(Context context, PropertyInfo source)
            : base(context, source) {

            Debug.Assert(FieldType == typeof(string));
            lengthConstraint_ = new Interval(Option.None<Bound>(), Option.None<Bound>());
        }

        public StringFieldDescriptor(Context context, PropertyInfo source, DataConverterAttribute annotation)
            : base(context, source, annotation) {

            Debug.Assert(FieldType == typeof(string));
            lengthConstraint_ = new Interval(Option.None<Bound>(), Option.None<Bound>());
        }

        protected sealed override StringFieldDescriptor Clone() {
            return new StringFieldDescriptor(this);
        }

        protected sealed override bool IsValidValue(object? value) {
            return base.IsValidValue(value) && (value is null || lengthConstraint_.Contains(((string)value).Length));
        }

        private StringFieldDescriptor(StringFieldDescriptor source)
            : base(source) {

            lengthConstraint_ = source.lengthConstraint_;
        }



        private Interval lengthConstraint_;
    }
}
