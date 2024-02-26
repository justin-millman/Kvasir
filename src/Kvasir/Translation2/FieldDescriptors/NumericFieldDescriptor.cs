using Kvasir.Schema;
using Optional;

namespace Kvasir.Translation2 {
    ///
    internal abstract class NumericFieldDescriptor : OrderableFieldDescriptor {
        private readonly Option<ComparisonOperator> relativeToZero_;
    }
}
