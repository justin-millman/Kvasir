using Cybele.Extensions;
using Kvasir.Annotations;
using Kvasir.Exceptions;
using Kvasir.Schema;
using Optional;
using System;
using System.Linq;
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
            // It is an error for a property to be annotated with multiple [Default] attributes
            var annotations = property.GetCustomAttributes<DefaultAttribute>();
            if (annotations.Count() > 1) {
                throw new KvasirException(
                    $"Error translating property {property.Name} of type {property.ReflectedType!.Name}: " +
                    "multiple [Default] annotations encountered"
                );
            }
            var annotation = annotations.FirstOrDefault();

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

            // It is an error for the [Default] value of a non-nullable Field to be 'null'
            if (annotation.Value == DBNull.Value && nullability == IsNullable.No) {
                throw new KvasirException(
                    $"Error translating property {property.Name} of type {property.ReflectedType!.Name}: " +
                    "[Default] value of 'null' is not valid for a non-nullable Field"
                );
            }

            // If the value of the [Default] annotation is 'null', then the rest of the checks are unnecessary
            if (annotation.Value == DBNull.Value) {
                return Option.Some<object?>(null);
            }

            // It is an error for the [Default] value of a Field to be an array
            if (annotation.Value.GetType().IsArray) {
                throw new KvasirException(
                    $"Error translating property {property.Name} of type {property.ReflectedType!.Name}: " +
                    "[Default] value cannot be an array"
                );
            }

            // It is an error for the [Default] value of a Field to be different than its pre-conversion CLR type; for
            // Fields whose pre-conversion CLR type is 'DateTime' or 'Guid', the [Default] value must be a 'string'
            // (that will later be parsed)
            var argStrWrapper =
                annotation.Value.GetType() == typeof(string) ? "\"" :
                annotation.Value.GetType() == typeof(char) ? "'" : "";
            if (property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(Guid)) {
                if (annotation.Value.GetType() != typeof(string)) {
                    throw new KvasirException(
                        $"Error translating property {property.Name} of type {property.ReflectedType!.Name}: " +
                        $"[Default] value of {argStrWrapper}{annotation.Value}{argStrWrapper} " +
                        $"(of type {annotation.Value.GetType().Name}) " +
                        $"is not valid for a Field of type {property.PropertyType.Name} " +
                        "(a string is required, which will then be parsed)"
                    );
                }
            }
            else if (!annotation.Value.GetType().IsInstanceOf(property.PropertyType)) {
                throw new KvasirException(
                    $"Error translating property {property.Name} of type {property.ReflectedType!.Name}: " +
                    $"[Default] value of {argStrWrapper}{annotation.Value}{argStrWrapper} " +
                    $"(of type {annotation.Value.GetType().Name}) " +
                    $"is not valid for a Field of type {property.PropertyType.Name}"
                );
            }

            // Parse value if necessary
            if (property.PropertyType == typeof(DateTime)) {
                if (!DateTime.TryParse((string)annotation.Value, out DateTime result)) {
                    throw new KvasirException(
                        $"Error translating property {property.Name} of type {property.ReflectedType!.Name}: " +
                        $"could not parse [Default] value \"{annotation.Value}\" into {nameof(DateTime)}"
                    );
                }
                return Option.Some<object?>(result);
            }
            if (property.PropertyType == typeof(Guid)) {
                if (!Guid.TryParse((string)annotation.Value, out Guid result)) {
                    throw new KvasirException(
                        $"Error translating property {property.Name} of type {property.ReflectedType!.Name}: " +
                        $"could not parse [Default] value \"{annotation.Value}\" into {nameof(Guid)}"
                    );
                }
                return Option.Some<object?>(result);
            }
            return Option.Some<object?>(annotation.Value);
        }
    }
}
