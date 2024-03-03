using Kvasir.Annotations;
using Optional;
using System.Reflection;

namespace Kvasir.Translation2 {
    /// <summary>
    ///   The intermediate base class for a <see cref="FieldDescriptor"/> whose data type supports ordering.
    /// </summary>
    internal abstract class OrderableFieldDescriptor : FieldDescriptor {
        protected OrderableFieldDescriptor(OrderableFieldDescriptor source)
            : base(source) {

            comparisonConstraint_ = source.comparisonConstraint_;
        }

        protected OrderableFieldDescriptor(Context context, PropertyInfo source)
            : base(context,source) {

            comparisonConstraint_ = new Interval(Option.None<Bound>(), Option.None<Bound>());
        }

        protected OrderableFieldDescriptor(Context context, PropertyInfo source, DataConverterAttribute annotation)
            : base(context, source, annotation) {

            comparisonConstraint_ = new Interval(Option.None<Bound>(), Option.None<Bound>());
        }

        protected override bool IsValidValue(object? value) {
            return base.IsValidValue(value) && (value is null || comparisonConstraint_.Contains(value));
        }


        private Interval comparisonConstraint_;
    }
}
