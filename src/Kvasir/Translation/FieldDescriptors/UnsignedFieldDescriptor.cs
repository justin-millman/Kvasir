using Cybele.Core;
using Kvasir.Annotations;
using Kvasir.Schema;
using System;
using System.Diagnostics;
using System.Reflection;

namespace Kvasir.Translation {
    /// <summary>
    ///   The concrete base class for an <see cref="OrderableFieldDescriptor"/> whose data type is an unsigned numeric.
    /// </summary>
    internal sealed class UnsignedFieldDescriptor : OrderableFieldDescriptor {
        public UnsignedFieldDescriptor(Context context, PropertyInfo source)
            : base(context, source) {

            Debug.Assert(
                FieldType == typeof(byte) ||
                FieldType == typeof(ushort) ||
                FieldType == typeof(uint) ||
                FieldType == typeof(ulong)
            );
        }

        public UnsignedFieldDescriptor(Context context, PropertyInfo source, DataConverterAttribute annotation)
            : base(context, source, annotation) {

            Debug.Assert(
                FieldType == typeof(byte) ||
                FieldType == typeof(ushort) ||
                FieldType == typeof(uint) ||
                FieldType == typeof(ulong)
            );
        }

        public UnsignedFieldDescriptor(Context context, PropertyInfo source, DataConverter converter)
            : base(context, source, converter) {

            Debug.Assert(
                FieldType == typeof(byte) ||
                FieldType == typeof(ushort) ||
                FieldType == typeof(uint) ||
                FieldType == typeof(ulong)
            );
        }

        public sealed override UnsignedFieldDescriptor Clone() {
            return new UnsignedFieldDescriptor(this);
        }

        public sealed override UnsignedFieldDescriptor Reset() {
            return new UnsignedFieldDescriptor(this, RESET);
        }

        protected sealed override void DoApplyConstraint(Context context, Check.SignednessAttribute annotation) {
            DoApplySignednessConstraint(context, annotation);
            var zero = Convert.ChangeType(0, FieldType);

            switch (annotation.Operator) {
                case ComparisonOperator.LT:
                    throw new InapplicableAnnotationException(context, annotation, FieldType, new UnsignedTag());
                case ComparisonOperator.GT:
                    DoApplyConstraint(context, new Check.IsGreaterThanAttribute(zero));
                    break;
                case ComparisonOperator.NE:
                    DoApplyConstraint(context, new Check.IsNotAttribute(zero));
                    break;
                default:
                    throw new ApplicationException($"Switch statement over {nameof(ComparisonOperator)} exhausted");
            }
        }

        private UnsignedFieldDescriptor(UnsignedFieldDescriptor source)
            : base(source) {}

        private UnsignedFieldDescriptor(UnsignedFieldDescriptor source, ResetTag _)
            : base(source, RESET) {}
    }
}
