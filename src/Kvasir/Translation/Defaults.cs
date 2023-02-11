using Kvasir.Annotations;
using Kvasir.Exceptions;
using Kvasir.Schema;
using Kvasir.Translation.Extensions;
using Optional;
using System;
using System.Reflection;

namespace Kvasir.Translation {
    internal sealed partial class Translator {
        /// <summary>
        ///   Get the pre-conversion default value for a Field backing a scalar property.
        /// </summary>
        /// <param name="property">
        ///   The source <see cref="PropertyInfo">property</see>.
        /// </param>
        /// <param name="nullability">
        ///   The nullability of the Field backing <paramref name="property"/>.
        /// </param>
        /// <exception cref="KvasirException">
        ///   if <paramref name="property"/> is annotated with multiple <c>[Default]</c> attributes
        ///     --or--
        ///   if the <c>[Default]</c> annotation applied to <paramref name="property"/> has a non-empty
        ///   <see cref="DefaultAttribute.Path">Path</see>
        ///     --or--
        ///   if <paramref name="nullability"/> is <see cref="IsNullable.No"/> but the value of the <c>[Default]</c>
        ///   annotation applied to <paramref name="property"/> is <see langword="null"/>
        ///     --or--
        ///   if the value of the <c>[Default]</c> annotation applied to <paramref name="property"/> is an array, even
        ///   if it is a single-element array whose element is the same type as that of <paramref name="property"/>
        ///     --or--
        ///   if the value of the <c>[Default]</c> annotation applied to <paramref name="property"/> is not
        ///   <see langword="null"/>, not an array, and is also not the of the expected type.
        /// </exception>
        /// <returns>
        ///   A <c>SOME</c> instance wrapping the unconverted (i.e. raw) default value for the Field backing
        ///   <paramref name="property"/> if such a default value is specified; otherwise, a <c>NONE</c> instance.
        ///   The default value for <see cref="DateTime"/>- and <see cref="Guid"/>-type properties will be parsed from
        ///   the original string.
        /// </returns>
        private static Option<object?> DefaultValueOf(PropertyInfo property, IsNullable nullability) {
            var annotation = property.Only<DefaultAttribute>();

            // If there is no [Default] annotation, then there is no default value
            if (annotation is null) {
                return Option.None<object?>();
            }

            // It is an error for the [Default] attribute of a scalar property to have a non-empty <Path> value
            if (annotation.Path != "") {
                throw new KvasirException(
                    $"Error translating property {property.Name} of type {property.ReflectedType!.Name}: " +
                    $"path \"{annotation.Path}\" of [Default] annotation does not exist"
                );
            }

            // All of the error handling for the value is managed by the parsing function
            return Option.Some(annotation.Value.ParseFor(property, property.PropertyType, nullability, "[Default] value"));
        }
    }
}
