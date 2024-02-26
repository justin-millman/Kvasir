using Optional;
using System.Reflection;

namespace Kvasir.Translation2 {
    ///
    internal abstract class OrderableFieldDescriptor : FieldDescriptor {
        ///
        protected OrderableFieldDescriptor(OrderableFieldDescriptor source)
            : base(source) {
        
            lowerBound_ = source.lowerBound_;
            upperBound_ = source.upperBound_;
        }

        ///
        protected OrderableFieldDescriptor(PropertyInfo source)
            : base(source) {

            lowerBound_ = Option.None<Bound>();
            upperBound_ = Option.None<Bound>();
        }


        protected readonly Option<Bound> lowerBound_;
        protected readonly Option<Bound> upperBound_;
    }
}
