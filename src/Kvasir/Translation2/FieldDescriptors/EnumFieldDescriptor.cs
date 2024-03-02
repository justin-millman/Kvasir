using Cybele.Extensions;
using Kvasir.Annotations;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Kvasir.Translation2 {
    /// <summary>
    ///   The concrete class for a <see cref="FieldDescriptor"/> whose data type is an enumeration.
    /// </summary>
    internal sealed class EnumFieldDescriptor : FieldDescriptor {
        public EnumFieldDescriptor(Context context, PropertyInfo source)
            : base(context, source) {

            Debug.Assert(FieldType.IsEnum);
            restrictedImage_ = FieldType.ValidValues().Cast<object>().ToHashSet();
        }

        public EnumFieldDescriptor(Context context, PropertyInfo source, DataConverterAttribute annotation)
            : base(context, source, annotation) {

            Debug.Assert(FieldType.IsEnum);
            restrictedImage_ = FieldType.ValidValues().Cast<object>().ToHashSet();
        }

        protected sealed override EnumFieldDescriptor Clone() {
            return new EnumFieldDescriptor(this);
        }

        private EnumFieldDescriptor(EnumFieldDescriptor source)
            : base(source) {

            restrictedImage_ = new HashSet<object>(source.restrictedImage_);
        }


        private readonly IReadOnlySet<object> restrictedImage_;
    }
}
