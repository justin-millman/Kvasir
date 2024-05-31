using System;
using System.Collections;
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
        ///     integers and floating points), Booleans, enumerators, strings, characters, the literal
        ///     <see langword="null"/>, and arrays thereof. This function will return the result of
        ///     <see cref="object.ToString"/> for any other type of argument provided.
        ///   </para>
        ///   <para>
        ///     The display result for any numeric type is its standard <see cref="object.ToString"/> representation. A
        ///     <see cref="bool"/> displays as its keyword (i.e. all lower-case <c>true</c> or <c>false</c>). A
        ///     <see cref="char"/> is wrapped in single quotes (e.g. <c>'x'</c>) and a <see cref="string"/> is wrapped
        ///     in double quotes (e.g. <c>"xyz"</c>). The literal <see langword="null"/>, as well as
        ///     <see cref="DBNull.Value"/>, will render as <c>'null'</c>, which is thereby distinguishable from a string
        ///     containing the character sequence <c>"null"</c>. Enumerations render with type qualification (i.e.
        ///     <c>MyEnum.SomeEnumerator</c>). Arrays are rendered as the recursive comma-separated display
        ///     representation of its elements, enclosed by curly braces.
        ///   </para>
        /// </remarks>
        /// <param name="self">
        ///   The <see cref="object"/> instance on which the extension method is invoked.
        /// </param>
        /// <returns>
        ///   A possibly modified string representation of <paramref name="self"/>.
        /// </returns>
        public static string ForDisplay(this object? self) {
            if (self is null || self == DBNull.Value) {
                return "'null'";
            }
            else if (self.GetType() == typeof(bool)) {
                return self.ToString()!.ToLower();
            }
            else if (self.GetType() == typeof(string)) {
                return $"\"{self}\"";
            }
            else if (self.GetType() == typeof(char)) {
                return $"'{self}'";
            }
            else if (self.GetType().IsArray) {
                var array = (self as IEnumerable)!.Cast<object>().ToArray();
                if (array.Length == 0) {
                    return "{}";
                }
                return $"{{ {string.Join(", ", array.Select(e => e.ForDisplay()))} }}";
            }
            else if (self.GetType().IsEnum) {
                return self.GetType().Name + "." + self.ToString();
            }
            else if (self.GetType() == typeof(float) || self.GetType() == typeof(double) || self.GetType() == typeof(decimal)) {
                var str = self.ToString()!;
                if (str.StartsWith("1E")) {
                    return str;
                }
                else if (str.EndsWith("∞") || str.EndsWith("Infinity")) {
                    // The representation of various infinities is different on Windows vs. Ubuntu
                    return str;
                }
                else if (!str.Contains('.')) {
                    return $"{str}.0";
                }
                else {
                    return str;
                }
            }
            else {
                // This will handle all integral numeric types
                return self.ToString()!;
            }
        }
    }
}
