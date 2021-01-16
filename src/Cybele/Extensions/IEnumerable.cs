using Ardalis.GuardClauses;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cybele.Extensions {
    /// <summary>
    ///   A collection of <see href="https://tinyurl.com/y8q6ojue">extension methods</see> that extend the public API
    ///   of the <see cref="IEnumerable{T}"/> interface.
    /// </summary>
    public static class IEnumerableExtensions {
        /// <summary>
        ///   Checks if all elements of an enumerable pass a given predicate. The index of the element is passed to
        ///   the predicate along with the element for evaluation.
        /// </summary>
        /// <typeparam name="T">
        ///   [deduced] The type of element in the enumerable.
        /// </typeparam>
        /// <param name="self">
        ///   The <see cref="IEnumerable{T}"/> on which the extension method is invoked.
        /// </param>
        /// <param name="predicate">
        ///   The predicate against which to check each <c>(index, element)</c> pair of <paramref name="self"/>.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if <paramref name="predicate"/> returns <see langword="true"/> for all
        ///   <c>(index, element)</c> pairs of <paramref name="self"/>; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool All<T>(this IEnumerable<T> self, Func<int, T, bool> predicate) {
            Guard.Against.Null(self, nameof(self));
            Guard.Against.Null(predicate, nameof(predicate));

            var index = 0;
            var enumerator = self.GetEnumerator();
            while (enumerator.MoveNext()) {
                if (!predicate(index, enumerator.Current)) {
                    return false;
                }
                ++index;
            }
            return true;
        }

        /// <summary>
        ///   Checks if any element of an enumerable passes a given predicate. The index of the element is passed to
        ///   the predicate along with the element for evaluation.
        /// </summary>
        /// <typeparam name="T">
        ///   [deduced] The type of element in the enumerable.
        /// </typeparam>
        /// <param name="self">
        ///   The <see cref="IEnumerable{T}"/> on which the extension method is invoked.
        /// </param>
        /// <param name="predicate">
        ///   The predicate against which to check each <c>(index, element)</c> pair of <paramref name="self"/>.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if <paramref name="predicate"/> returns <see langword="true"/> for any
        ///   <c>(index, element)</c> pair of <paramref name="self"/>; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool Any<T>(this IEnumerable<T> self, Func<int, T, bool> predicate) {
            Guard.Against.Null(self, nameof(self));
            Guard.Against.Null(predicate, nameof(predicate));

            var index = 0;
            var enumerator = self.GetEnumerator();
            while (enumerator.MoveNext()) {
                if (predicate(index, enumerator.Current)) {
                    return true;
                }
                ++index;
            }
            return false;
        }

        /// <summary>
        ///   Checks if an enumerable contains only unique elements, using the default
        ///   <see cref="IEqualityComparer{T}"/> for the element type.
        /// </summary>
        /// <typeparam name="T">
        ///   [deduced] The type of element in the enumerable.
        /// </typeparam>
        /// <param name="self">
        ///   The <see cref="IEnumerable{T}"/> on which the extension method is invoked.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if, according to the default <see cref="IEqualityComparer{T}"/> for
        ///   <typeparamref name="T"/>, <paramref name="self"/> contains no duplicate elements; otherwise,
        ///   <see langword="false"/>.
        /// </returns>
        public static bool ContainsNoDuplicates<T>(this IEnumerable<T> self) {
            Guard.Against.Null(self, nameof(self));
            return ContainsNoDuplicates(self, EqualityComparer<T>.Default);
        }

        /// <summary>
        ///   Checks if an enumerable contains only unique elements, using a specific
        ///   <see cref="IEqualityComparer{T}"/> for the element type.
        /// </summary>
        /// <typeparam name="T">
        ///   [deduced] The type of element in the enumerable.
        /// </typeparam>
        /// <param name="self">
        ///   The <see cref="IEnumerable{T}"/> on which the extension method is invoked.
        /// </param>
        /// <param name="comparer">
        ///   The <see cref="IEqualityComparer{T}"/> with which to compare elements of <paramref name="self"/> for
        ///   uniqueness.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if, according to <paramref name="comparer"/>, <paramref name="self"/> contains no
        ///   duplicate elements; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool ContainsNoDuplicates<T>(this IEnumerable<T> self, IEqualityComparer<T> comparer) {
            Guard.Against.Null(self, nameof(self));
            Guard.Against.Null(comparer, nameof(comparer));

            var distinct = self.Distinct(comparer);
            return distinct.LongCount() == self.LongCount();
        }

        /// <summary>
        ///   Checks if an enumerable is empty, i.e. contains no elements.
        /// </summary>
        /// <typeparam name="T">
        ///   [deduced] The type of element in the enumerable.
        /// </typeparam>
        /// <param name="self">
        ///   The <see cref="IEnumerable{T}"/> on which the extension method is invoked.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if <paramref name="self"/> contains no elements; otherwise,
        ///   <see langword="false"/>.
        /// </returns>
        public static bool IsEmpty<T>(this IEnumerable<T> self) {
            Guard.Against.Null(self, nameof(self));

            var enumerator = self.GetEnumerator();
            return !enumerator.MoveNext();
        }

        /// <summary>
        ///   Checks if all elements of an enumerable fail a given predicate.
        /// </summary>
        /// <typeparam name="T">
        ///   [deduced] The type of element in the enumerable.
        /// </typeparam>
        /// <param name="self">
        ///   The <see cref="IEnumerable{T}"/> on which the extension method is invoked.
        /// </param>
        /// <param name="predicate">
        ///   The predicate against which to check each element of <paramref name="self"/>.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if <paramref name="predicate"/> returns <see langword="false"/> for all
        ///   elements of <paramref name="self"/>; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool None<T>(this IEnumerable<T> self, Func<T, bool> predicate) {
            Guard.Against.Null(self, nameof(self));
            Guard.Against.Null(predicate, nameof(predicate));

            var index = 0;
            var enumerator = self.GetEnumerator();
            while (enumerator.MoveNext()) {
                if (predicate(enumerator.Current)) {
                    return false;
                }
                ++index;
            }
            return true;
        }

        /// <summary>
        ///   Checks if all elements of an enumerable fail a given predicate. The index of the element is passed to
        ///   the predicate along with the element for evaluation.
        /// </summary>
        /// <typeparam name="T">
        ///   [deduced] The type of element in the enumerable.
        /// </typeparam>
        /// <param name="self">
        ///   The <see cref="IEnumerable{T}"/> on which the extension method is invoked.
        /// </param>
        /// <param name="predicate">
        ///   The predicate against which to check each <c>(index, element)</c> pair of <paramref name="self"/>.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if <paramref name="predicate"/> returns <see langword="false"/> for all
        ///   <c>(index, element)</c> pairs of <paramref name="self"/>; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool None<T>(this IEnumerable<T> self, Func<int, T, bool> predicate) {
            Guard.Against.Null(self, nameof(self));
            Guard.Against.Null(predicate, nameof(predicate));

            var index = 0;
            var enumerator = self.GetEnumerator();
            while (enumerator.MoveNext()) {
                if (predicate(index, enumerator.Current)) {
                    return false;
                }
                ++index;
            }
            return true;
        }
    }
}
