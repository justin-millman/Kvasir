using Optional;
using System;

namespace Cybele.Extensions {
    /// <summary>
    ///   A collection of <see href="https://tinyurl.com/y8q6ojue">extension methods</see> that extend the public API
    ///   of the <see cref="Option{T}"/> class.
    /// </summary>
    public static class OptionExtensions {
        /// <summary>
        ///   Unwraps an <see cref="Option{T}"/>, exposing the wrapped value.
        /// </summary>
        /// <typeparam name="T">
        ///   [deduced] The type of value wrapped in the option.
        /// </typeparam>
        /// <param name="self">
        ///   The option on which the extension method is invoked.
        /// </param>
        /// <returns>
        ///   The value wrapped by <paramref name="self"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        ///   if <paramref name="self"/> is a <c>NONE</c> instance (i.e. an empty option).
        /// </exception>
        public static T Unwrap<T>(this Option<T> self) {
            return self.Match(
                some: t => t,
                none: () => throw new InvalidOperationException("Cannot unwrap an empty option")
            );
        }
    }
}
