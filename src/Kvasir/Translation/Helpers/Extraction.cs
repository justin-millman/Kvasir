using Kvasir.Exceptions;
using System;
using System.Linq;
using System.Reflection;

namespace Kvasir.Translation.Extensions {
    internal static partial class TranslationExtensions {
        /// <summary>
        ///   Extract the single instance of a particular annotation <see cref="Attribute"/> that has been applied to a
        ///   <see cref="PropertyInfo"/>.
        /// </summary>
        /// <typeparam name="TAnnotation">
        ///   The target annotation type.
        /// </typeparam>
        /// <param name="self">
        ///   The <see cref="PropertyInfo"/> on which the extension method is invoked.
        /// </param>
        /// <returns>
        ///   The single instance of <typeparamref name="TAnnotation"/> that is applied to <paramref name="self"/>, if
        ///   such an annotation exists; otherwise, <see langword="null"/>.
        /// </returns>
        /// <exception cref="KvasirException">
        ///   if <paramref name="self"/> is annotated with multiple instances of <typeparamref name="TAnnotation"/>.
        /// </exception>
        public static TAnnotation? Only<TAnnotation>(this PropertyInfo self) where TAnnotation : Attribute {
            var annotations = self.GetCustomAttributes<TAnnotation>();
            if (annotations.Count() > 1) {
                var attrType = typeof(TAnnotation);
                var attrName = attrType.Name[0..^9];
                var display = attrType.DeclaringType is null ? attrName : $"{attrType.DeclaringType.Name}.{attrName}";

                throw new KvasirException(
                    $"Error translating property {self.Name} of type {self.ReflectedType!.Name}: " +
                    $"multiple [{display}] annotations encountered"
                );
            }
            return annotations.FirstOrDefault();
        }
    }
}
