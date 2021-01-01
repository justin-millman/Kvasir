using System.Collections.Generic;

namespace Cybele.Extensions {
    /// <summary>
    ///   A collection of extension methods that extend <see cref="IEnumerable{T}"/> and <see cref="IEnumerator{T}"/>.
    /// </summary>
    public static class EnumerableExtensions {
        /// <summary>
        ///   Generates an <see cref="IEnumerable{T}"/> whose contents are defined by an enumerator.
        /// </summary>
        /// <typeparam name="T">
        ///   [deduced] The type of object in the enumerator, and therefore in the resulting
        ///   <see cref="IEnumerable{T}"/>.
        /// </typeparam>
        /// <param name="self">
        ///   The source <see cref="IEnumerator{T}"/>.
        /// </param>
        /// <post>
        ///   <paramref name="self"/> has been reset.
        /// </post>
        /// <returns>
        ///   An <see cref="IEnumerable{T}"/> whose contents match the sequence produced by <paramref name="self"/>.
        /// </returns>
        public static IEnumerable<T> ToEnumerable<T>(this IEnumerator<T> self) {
            while (self.MoveNext()) {
                yield return self.Current;
            }
            self.Reset();
        }
    }
}
