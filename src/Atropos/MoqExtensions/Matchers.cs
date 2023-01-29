using Ardalis.GuardClauses;
using Moq;
using System.Collections;
using System.Collections.Generic;

namespace Atropos.Moq {
    /// <summary>
    ///   A collection of custom Matchers that can be used to verify calls against a <see cref="Mock{T}"/>.
    /// </summary>
    public static class Arg {
        /// <summary>
        ///   Produces a Matcher that returns <see langword="true"/> when the actual argument is an
        ///   <see cref="IEnumerable"/> with the same sequence of elements as another.
        /// </summary>
        /// <typeparam name="T">
        ///   The conrete type of <see cref="IEnumerable"/> to be operated on by the Matcher.
        /// </typeparam>
        /// <param name="expected">
        ///   The sequence of elements.
        /// </param>
        /// <pre>
        ///   <paramref name="expected"/> is not <see langword="null"/>.
        /// </pre>
        /// <returns>
        ///   A dummy instance of <typeparamref name="T"/> for syntactic compliance.
        /// </returns>
        public static T IsSameSequence<T>(IEnumerable expected) where T : IEnumerable {
            return IsSameSequence<T>(expected, EqualityComparer<object>.Default);
        }

        /// <summary>
        ///   Produces a Matcher that returns <see langword="true"/> when the actual argument is an
        ///   <see cref="IEnumerable"/> with the same sequence of elements as another as defined by a custom comparer.
        /// </summary>
        /// <typeparam name="T">
        ///   The conrete type of <see cref="IEnumerable"/> to be operated on by the Matcher.
        /// </typeparam>
        /// <param name="expected">
        ///   The sequence of elements.
        /// </param>
        /// <param name="comparer">
        ///   The comparer with which to determine equality of elements.
        /// </param>
        /// <pre>
        ///   <paramref name="expected"/> is not <see langword="null"/>
        ///     --and--
        ///   <paramref name="comparer"/> is not <see langword="null"/>.
        /// </pre>
        /// <returns>
        ///   A dummy instance of <typeparamref name="T"/> for syntactic compliance.
        /// </returns>
        public static T IsSameSequence<T>(IEnumerable expected, IEqualityComparer comparer) where T : IEnumerable {
            Guard.Against.Null(expected, nameof(expected));
            Guard.Against.Null(comparer, nameof(comparer));

            return Match.Create<T>(actual => {
                var e1 = actual.GetEnumerator();
                var e2 = expected.GetEnumerator();

                while (e1.MoveNext()) {
                    if (!e2.MoveNext() || !comparer.Equals(e1.Current, e2.Current)) {
                        return false;
                    }
                }
                return !e2.MoveNext();
            });
        }
    }
}
