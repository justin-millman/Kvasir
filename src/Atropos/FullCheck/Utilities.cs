using Optional;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Atropos {
    public static partial class FullCheck {
        /// <summary>
        ///   Produces a list of interfaces and base types of a particular <see cref="Type"/>, including that
        ///   <see cref="Type"/> itself.
        /// </summary>
        /// <param name="type">
        ///   The target <see cref="Type"/>.
        /// </param>
        /// <pre>
        ///   <paramref name="type"/> is not <see langword="null"/>.
        /// </pre>
        /// <returns>
        ///   A list consisting of <paramref name="type"/>, all interfaces implemented by <paramref name="type"/>, and
        ///   all base classes of <paramref name="type"/>. The order of elements in the list is undefined.
        /// </returns>
        private static IList<Type> GetBasesOf(Type type) {
            Debug.Assert(type is not null);

            var results = new List<Type>() { type };
            results.AddRange(type.GetInterfaces());

            for (Type? t = type.BaseType; t is not null; t = t.BaseType) {
                results.Add(t);
            }

            return results;
        }

        /// <summary>
        ///   Symmetrially evaluates the <c>IEquatable&lt;T&gt;.Equals(T?)</c> method for two arguments if the type of
        ///   those arguments implements the <see cref="IEquatable{T}"/> interface.
        /// </summary>
        /// <typeparam name="T">
        ///   [deduced] The type of the arguments.
        /// </typeparam>
        /// <param name="lhs">
        ///   The first of the two arguments.
        /// </param>
        /// <param name="rhs">
        ///   The second of the two arguments.
        /// </param>
        /// <param name="expectedEqual">
        ///   Whether or not <paramref name="lhs"/> and <paramref name="rhs"/> are expected to be equal.
        /// </param>
        /// <returns>
        ///   If the equality relation between <paramref name="lhs"/> and <paramref name="rhs"/>, is not the same as
        ///   that indicated by <paramref name="expectedEqual"/>, a <c>SOME</c> instance whose value is the expression
        ///   that exposed the failed equality relation. Otherwise, a <c>NONE</c> instance.
        /// </returns>
        private static Option<string> EvaluateIEquatable<T>(T lhs, T rhs, bool expectedEqual) {
            // We have to check for equality against the inverse of the expectation so that a null evaluation result,
            // obtained when the first operand is itself null or if when `T` does not implement the `IEquatable<T>`
            // interface, does not yield a false positive. This approach ensures that the test only fails if the method
            // is actually invoked _and_ the result is the opposite of what was expected.
            if ((lhs as IEquatable<T>)?.Equals(rhs) == !expectedEqual) {
                return Option.Some($"IEquatable<{typeof(T).Name}>.Equals({lhs}, {rhs})");
            }
            if ((rhs as IEquatable<T>)?.Equals(lhs) == !expectedEqual) {
                return Option.Some($"IEquatable<{typeof(T).Name}>.Equals({rhs}, {lhs}");
            }

            return Option.None<string>();
        }
 
        /// <summary>
        ///   Symmetrically evaluates the <c>IComparable&lt;T&gt;.CompareTo(T?)</c> method for two arguments if the
        ///   type of those arguments implements the <see cref="IComparable{T}"/> interface.
        /// </summary>
        /// <typeparam name="T">
        ///   [deduced] The type of the arguments.
        /// </typeparam>
        /// <param name="lhs">
        ///   The first of the two arguments.
        /// </param>
        /// <param name="rhs">
        ///   The second of the two arguments.
        /// </param>
        /// <param name="trichotomy">
        ///   A negative integer if <paramref name="lhs"/> is expected to be strictly less than <paramref name="rhs"/>,
        ///   <c>0</c> if <paramref name="lhs"/> is expected to be equivalent to <paramref name="rhs"/>, a positive
        ///   integer if <paramref name="lhs"/> is expected to be strictly greater than <paramref name="rhs"/>, and
        ///   <see langword="null"/> if <paramref name="lhs"/> is expected to be incomparable to
        ///   <paramref name="rhs"/>.
        /// </param>
        /// <returns>
        ///   If the ordering relation between <paramref name="lhs"/> and <paramref name="rhs"/> is not the same as
        ///   that indicated by <paramref name="trichotomy"/>, a <c>SOME</c> instance whose value is the expression
        ///   that exposed the failed ordering. Otherwise, a <c>NONE</c> instance.
        /// </returns>
        private static Option<string> EvaluateIComparable<T>(T lhs, T rhs, int? trichotomy) {
            var lhsCompRhs = (lhs as IComparable<T>)?.CompareTo(rhs);
            var rhsCompLhs = (rhs as IComparable<T>)?.CompareTo(lhs);

            var expected = trichotomy is null ? 0 : (trichotomy < 0 ? 1 : (trichotomy == 0 ? 2 : 3));
            var lrActual = lhsCompRhs is null ? 0 : (lhsCompRhs < 0 ? 1 : (lhsCompRhs == 0 ? 2 : 3));
            var rlActual = rhsCompLhs is null ? 0 : (rhsCompLhs < 0 ? 1 : (rhsCompLhs == 0 ? 2 : 3));

            if (lrActual != expected) {
                return Option.Some($"IComparable<{typeof(T).Name}>.CompareTo({lhs}, {rhs})");
            }
            if (rlActual != (expected * 3) % 4) {
                return Option.Some($"IComparable<{typeof(T).Name}>.CompareTo({rhs}, {lhs})");
            }

            return Option.None<string>();
        }
    }
}
