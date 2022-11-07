using Kvasir.Annotations;
using Kvasir.Exceptions;
using Optional;
using System.Reflection;

namespace Kvasir.Translation {
    internal sealed partial class Translator {
        /// <summary>
        ///   Determine the <c>0</c>-based column index of the first Field corresponding to a property. This function is
        ///   safe to use for any kind of property.
        /// </summary>
        /// <param name="property">
        ///   The source <see cref="PropertyInfo">property</see>.
        /// </param>
        /// <exception cref="KvasirException">
        ///   if the <c>[Column]</c> annotation applied to <paramref name="property"/> has a negative value
        /// </exception>
        /// <returns>
        ///   A <c>SOME</c> instance wrapping the <c>0</c>-based column index of the first Field backing
        ///   <paramref name="property"/> if such a column index is specified; otherwise, a <c>NONE</c> instance.
        /// </returns>
        private static Option<int> ColumnOf(PropertyInfo property) {
            var annotation = property.GetCustomAttribute<ColumnAttribute>();

            // If there is no [Column] annotation, then there is no explicit column index
            if (annotation is null) {
                return Option.None<int>();
            }

            // It is an error for the [Column] index to be negative
            if (annotation.Column < 0) {
                throw new KvasirException(
                    $"Error translating property {property.Name} of type {property.ReflectedType!.Name}: " +
                    $"[Column] index {annotation.Column} is negative"
                );
            }

            // No errors detected
            return Option.Some(annotation.Column);
        }
    }
}
