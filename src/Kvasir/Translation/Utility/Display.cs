using Cybele.Extensions;
using System;
using System.Diagnostics;
using System.Linq;

namespace Kvasir.Translation {
    /// <summary>
    ///   A collection of utility functions for displaying content to users, generally in the context of error messages.
    /// </summary>
    internal static class Display {
        /// <summary>
        ///   Produces the display name for an annotation type, enclosed in brackets, for use in error messages that
        ///   convey a problem with a user-provided annotation.
        /// </summary>
        /// <param name="type">
        ///   The annotation <see cref="Type"/>.
        /// </param>
        /// <returns>
        ///   The name of <paramref name="type"/> to be used in error messages.
        /// </returns>
        public static string AnnotationDisplayName(Type type) {
            Debug.Assert(type.IsInstanceOf(typeof(Attribute)));

            if (type.Name.StartsWith("CheckAttribute")) {
                return "Check";
            }
            else if (type.Name.Contains("ComplexAttribute")) {
                return "Check.Complex";
            }
            else if (type.FullName!.Contains(".Check+")) {
                return "Check." + type.Name[..^9];
            }
            else {
                var tickIdx = type.Name.IndexOf('`');
                var genericSuffixLen = tickIdx == -1 ? 0 : type.Name.Length - tickIdx;
                return type.Name[..^(9 + genericSuffixLen)];
            }
        }

        /// <summary>
        ///   Produces the display name for a type, enclosed in backticks, for use in error messages that communicate
        ///   the type of a property or value.
        /// </summary>
        /// <param name="self">
        ///   The <see cref="Type"/> on which the extension method is invoked.
        /// </param>
        /// <returns>
        ///   The name of <paramref name="self"/> to be used in error messages.
        /// </returns>
        public static string DisplayName(this Type self) {
            // Built-Ins
            if (self == typeof(bool)) {
                return "`bool`";
            }
            else if (self == typeof(sbyte)) {
                return "`sbyte`";
            }
            else if (self == typeof(byte)) {
                return "`byte`";
            }
            else if (self == typeof(short)) {
                return "`short`";
            }
            else if (self == typeof(ushort)) {
                return "`ushort`";
            }
            else if (self == typeof(int)) {
                return "`int`";
            }
            else if (self == typeof(uint)) {
                return "`uint`";
            }
            else if (self == typeof(long)) {
                return "`long`";
            }
            else if (self == typeof(ulong)) {
                return "`ulong`";
            }
            else if (self == typeof(float)) {
                return "`float`";
            }
            else if (self == typeof(double)) {
                return "`double`";
            }
            else if (self == typeof(decimal)) {
                return "`decimal`";
            }
            else if (self == typeof(string)) {
                return "`string`";
            }
            else if (self == typeof(char)) {
                return "`char`";
            }

            // By-Ref
            else if (self.IsByRef) {
                var underlying = self.GetElementType()!.DisplayName()[1..^1];
                return $"`ref {underlying}`";
            }

            // Array
            else if (self.IsArray) {
                var element = self.GetElementType()!.DisplayName()[1..^1];
                return $"`{element}[]`";
            }

            // Pointers
            else if (self.IsPointer) {
                var referent = self.GetElementType()!.DisplayName()[1..^1];
                return $"`{referent}*`";
            }

            // Generics
            else if (self.IsGenericType) {
                var args = self.GenericTypeArguments.Select(t => t.DisplayName()[1..^1]);

                // Nullable<T>
                if (self.GetGenericTypeDefinition() == typeof(Nullable<>)) {
                    Debug.Assert(args.Count() == 1);
                    return $"`{args.First()}?`";
                }
                // Anything else
                else {
                    var generic = self.GetGenericTypeDefinition().Name;
                    return $"`{generic[..generic.IndexOf("`")]}<{string.Join(", ", args)}>`";
                }
            }

            // All Others
            else {
                return $"`{self.Name}`";
            }
        }
    }
}
