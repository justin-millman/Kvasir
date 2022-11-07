using Cybele.Extensions;
using Kvasir.Annotations;
using Kvasir.Exceptions;
using Kvasir.Schema;
using System.Reflection;

namespace Kvasir.Translation {
    internal sealed partial class Translator {
        /// <summary>
        ///   Determine the nullability imparted by a property, using a combination of the property's native CLR type
        ///   and user-provided attributes. This function is safe to use for any kind of property.
        /// </summary>
        /// <param name="property">
        ///   The source <see cref="PropertyInfo">property</see>.
        /// </param>
        /// <exception cref="KvasirException">
        ///   if <paramref name="property"/> is annotated with both <c>[Nullable]</c> and <c>[NonNullable]</c>
        ///     --or--
        ///   if <paramref name="property"/> is an inherently nullable property and it is still annotated with
        ///   <c>[Nullable]</c>
        ///     --or--
        ///   if <paramref name="property"/> is an inherently non-nullable property and it is still annotated with
        ///   <c>[NonNullable]</c>
        /// </exception>
        /// <returns>
        ///   <see cref="IsNullable.Yes"/> if <paramref name="property"/> implies nullability for any corresponding
        ///   Fields; otherwise, <see cref="IsNullable.No"/>.
        /// </returns>
        private static IsNullable NullabilityOf(PropertyInfo property) {
            var forceNullable = property.HasAttribute<NullableAttribute>();
            var forceNonNullable = property.HasAttribute<NonNullableAttribute>();
            var deduction = property.GetNullability();

            // It is an error for a property to be annotated as both [Nullable] and [NonNullable]
            if (forceNullable && forceNonNullable) {
                throw new KvasirException(
                    $"Error translating property {property.Name} of type {property.ReflectedType!.Name}: " +
                    "property is annotated as with both [Nullable] and [NonNullable]"
                );
            }

            // It is an error for a property that would otherwise be deduced as nullable to be annotated as [Nullable]
            if (forceNullable && deduction == Nullability.Nullable) {
                throw new KvasirException(
                    $"Error translating property {property.Name} of type {property.ReflectedType!.Name}: " +
                    "[Nullable] annotation is redundant (Field is already nullable)"
                );
            }

            // It is an error for a property that would otherwise be deduced as non-nullable to be annotated as
            // [NonNullable]
            if (forceNonNullable && deduction == Nullability.NonNullable) {
                throw new KvasirException(
                    $"Error translating property {property.Name} of type {property.ReflectedType!.Name}: " +
                    "[NonNullable] annotation is redundant (Field is already non-nullable)"
                );
            }

            // No errors detected
            if (forceNullable || (deduction == Nullability.Nullable && !forceNonNullable)) {
                return IsNullable.Yes;
            }
            return IsNullable.No;
        }
    }
}
