using Kvasir.Annotations;
using Kvasir.Schema;
using Optional;
using System.Reflection;

namespace Kvasir.Translation2 {
    /// <summary>
    ///   The intermediate base class for an <see cref="OrderableFieldDescriptor"/> whose data type is numeric.
    /// </summary>
    internal abstract class NumericFieldDescriptor : OrderableFieldDescriptor {
        protected NumericFieldDescriptor(NumericFieldDescriptor source)
            : base(source) {

            relativeToZero_ = source.relativeToZero_;
        }

        protected NumericFieldDescriptor(Context context, PropertyInfo source)
            : base(context, source) {

            relativeToZero_ = Option.None<ComparisonOperator>();
        }

        protected NumericFieldDescriptor(Context context, PropertyInfo source, DataConverterAttribute annotation)
            : base(context, source, annotation) {

            relativeToZero_ = Option.None<ComparisonOperator>();
        }


        protected Option<ComparisonOperator> relativeToZero_;
    }
}
