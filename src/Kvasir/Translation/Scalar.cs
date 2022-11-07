using System.Linq;
using System.Reflection;

namespace Kvasir.Translation {
    internal sealed partial class Translator {
        /// <summary>
        ///   Translate a scalar property into a <see cref="FieldDescriptor"/>.
        /// </summary>
        /// <param name="property">
        ///   The source <see cref="PropertyInfo">property</see>.
        /// </param>
        /// <returns>
        ///   The translation of <paramref name="property"/>.
        /// </returns>
        private static FieldDescriptor TranslateScalar(PropertyInfo property) {
            var nullability = NullabilityOf(property);
            return new FieldDescriptor(
                SourceType: property.ReflectedType!,
                AccessPath: property.Name,
                Name: NameOf(property),
                Nullability: nullability,
                Column: ColumnOf(property),
                CLRType: property.PropertyType,
                Converter: ConverterFor(property),
                RawDefault: DefaultValueOf(property, nullability),
                IsInPrimaryKey: IsInPrimaryKey(property, nullability),
                KeyMemberships: CandidateKeysContaining(property).ToList()
            );
        }
    }
}
