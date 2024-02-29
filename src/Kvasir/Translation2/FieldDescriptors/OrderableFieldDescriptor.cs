using Optional;
using System.Reflection;

namespace Kvasir.Translation2 {
    ///
    internal abstract class OrderableFieldDescriptor : FieldDescriptor {
        ///
        protected OrderableFieldDescriptor(OrderableFieldDescriptor source)
            : base(source) {

            comparisonConstraint_ = source.comparisonConstraint_;
        }

        ///
        protected OrderableFieldDescriptor(PropertyInfo source)
            : base(source) {

            comparisonConstraint_ = new Interval(Option.None<Bound>(), Option.None<Bound>());
        }


        protected readonly Interval comparisonConstraint_;
    }
}
