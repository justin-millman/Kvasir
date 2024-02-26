using Optional;

namespace Kvasir.Translation2 {
    ///
    internal abstract class OrderableFieldDescriptor : FieldDescriptor {
        private readonly Option<Bound> lowerBound_;
        private readonly Option<Bound> upperBound_;
    }
}
