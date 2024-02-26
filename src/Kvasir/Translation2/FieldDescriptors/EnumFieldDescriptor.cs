using Cybele.Extensions;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Kvasir.Translation2 {
    ///
    internal sealed class EnumFieldDescriptor : FieldDescriptor {
        ///
        public EnumFieldDescriptor(PropertyInfo source)
            : base(source) {

            Debug.Assert(FieldType.IsEnum);
            restrictedImage_ = source.PropertyType.ValidValues().Cast<object>().ToHashSet();
        }

        ///
        protected sealed override EnumFieldDescriptor Clone() {
            return new EnumFieldDescriptor(this);
        }

        ///
        private EnumFieldDescriptor(EnumFieldDescriptor source)
            : base(source) {

            restrictedImage_ = new HashSet<object>(source.restrictedImage_);
        }


        private IReadOnlySet<object> restrictedImage_;
    }
}
