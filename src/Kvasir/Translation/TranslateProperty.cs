using System;
using System.Collections.Generic;
using System.Reflection;

namespace Kvasir.Translation {
    internal sealed partial class Translator {
        /// <summary>
        ///   Translate a single CLR property of a particular.
        /// </summary>
        /// <param name="property">
        ///   The <see cref="PropertyInfo">CLR property</see> to translate.
        /// </param>
        /// <returns>
        ///   The <see cref="PropertyDescriptor"/> describing <paramref name="property"/>.
        /// </returns>
        private PropertyDescriptor TranslateProperty(PropertyInfo property) {
            var category = CategoryOf(property);

            if (category == PropertyCategory.Scalar) {
                var fields = new List<FieldDescriptor>() { TranslateScalar(property) };
                return new PropertyDescriptor(Fields: fields);
            }
            else {
                throw new NotSupportedException("Only scalar properties can be translated at this time");
            }
        }
    }
}
