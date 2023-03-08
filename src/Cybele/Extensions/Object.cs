using System.Linq;

namespace Cybele.Extensions {
    /// <summary>
    ///   A collection of <see href="https://tinyurl.com/y8q6ojue">extension methods</see> that extend the public API
    ///   of the objects.
    /// </summary>
    public static class ObjectExtensions {
        /// <summary>
        ///   Converts a possibly <see langword="null"/> object to a string suitable for display in error messages to
        ///   users.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     This function is specifically designed to operate on objects that may be passed to the constructor of an
        ///     attribute. According to the C# standard, the only viable such types are numerics (signed/unsigned
        ///     integers and floating points), Booleans, strings, characters, the literal <see langword="null"/>, and
        ///     array thereof. This function will return the result of <see cref="object.ToString"/> for any other type
        ///     of argument provided.
        ///   </para>
        ///   <para>
        ///     The display result for a <see cref="bool"/> and for any numeric type is its standard
        ///     <see cref="object.ToString"/> representation. <see cref="char"/> is wrapped in single quotes (e.g.
        ///     <c>'x'</c>) and <see cref="string"/> is wrapped in double quotes (e.g. <c>"xyz"</c>). The literal
        ///     <see langword="null"/> will render as <c>'null'</c>, which is thereby distinguishable from a string
        ///     containing the character sequence <c>"null"</c>. Arrays are rendered as the recursive comma-separated
        ///     display representation of its elements, enclosed by curly braces.
        ///   </para>
        /// </remarks>
        /// <param name="self">
        ///   The <see cref="object"/> instance on which the extension method is invoked.
        /// </param>
        /// <returns>
        ///   A possibly modified string representation of <paramref name="self"/>.
        /// </returns>
        public static string ForDisplay(this object? self) {
            if (self is null) {
                return "'null'";
            }
            if (self.GetType() == typeof(bool)) {
                return self.ToString()!.ToLower();
            }
            if (self.GetType() == typeof(string)) {
                return $"\"{self}\"";
            }
            if (self.GetType() == typeof(char)) {
                return $"'{self}'";
            }
            if (self.GetType().IsArray) {
                var array = (object[])self;
                if (array.Length == 0) {
                    return "{}";
                }
                return $"{{ {string.Join(", ", array.Select(e => e.ForDisplay()))} }}";
            }

            // This will handle signed integers, unsigned integers, floating point numbers, and Booleans
            return self.ToString()!;
        }
    }
}
