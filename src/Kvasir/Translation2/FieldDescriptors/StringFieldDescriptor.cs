using Optional;

namespace Kvasir.Translation2 {
    ///
    internal sealed class StringFieldDescriptor : OrderableFieldDescriptor {
        private readonly Option<Bound> minimumLength_;
        private readonly Option<Bound> maximumLength_;
    }
}
