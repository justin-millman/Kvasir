using Kvasir.Schema;
using Optional;
using System.Reflection;

namespace Kvasir.Translation2 {
    ///
    internal abstract class NumericFieldDescriptor : OrderableFieldDescriptor {
        ///
        protected NumericFieldDescriptor(NumericFieldDescriptor source)
            : base(source) {

            relativeToZero_ = source.relativeToZero_;
        }

        ///
        protected NumericFieldDescriptor(PropertyInfo source)
            : base(source) {

            relativeToZero_ = Option.None<ComparisonOperator>();
        }


        protected Option<ComparisonOperator> relativeToZero_;
    }
}
