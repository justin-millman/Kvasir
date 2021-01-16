using Optional;
using System;
using System.Reflection;

namespace Atropos {
    public static partial class FullCheck {
        /// <summary>
        ///   Determines if the full equality API exposed by a type indicates that two instances of that type are
        ///   equal.
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
        /// <returns>
        ///   A <c>SOME</c> instance with the expression that, when evaluated, indicates that <paramref name="lhs"/>
        ///   and <paramref name="rhs"/> are not equal if such an expression exists; otherwise, a <c>NONE</c> instance.
        /// </returns>
        public static Option<string> ExpectEqual<T>(T lhs, T rhs) {
            return TestEquality(lhs, rhs, true);
        }

        /// <summary>
        ///   Determines if the full equality API exposed by a type indicates that two instances of that type are not
        ///   equal.
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
        /// <returns>
        ///   A <c>SOME</c> instance with the expression that, when evaluated, indicates that <paramref name="lhs"/>
        ///   and <paramref name="rhs"/> are equal if such an expression exists; otherwise, a <c>NONE</c> instance.
        /// </returns>
        public static Option<string> ExpectNotEqual<T>(T lhs, T rhs) {
            return TestEquality(lhs, rhs, false);
        }

        /// <summary>
        ///   Evaluates the equality relation between two instancs of a type using the full ordering API of that type.
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
        private static Option<string> TestEquality<T>(T lhs, T rhs, bool expectedEqual) {
            var eq = GetOperator<T>(Operator.EQ, expectedEqual);
            var neq = GetOperator<T>(Operator.NEQ, !expectedEqual);

            // Symmetrically evaluate operator== if it is present on `T` or a base class of `T`, selecting the overload
            // accepting the most derived type
            if (eq(lhs, rhs) != expectedEqual) { return Option.Some($"{lhs} == {rhs}"); }
            if (eq(rhs, lhs) != expectedEqual) { return Option.Some($"{rhs} == {lhs}"); }

            // Symmetrically evaluate operator!= if it is present on `T` or a base class of `T`, selecting the overload
            // accepting the most derived type
            if (neq(lhs, rhs) != !expectedEqual) { return Option.Some($"{lhs} != {rhs}"); }
            if (neq(rhs, lhs) != !expectedEqual) { return Option.Some($"{rhs} != {lhs}"); }

            // Symmetrically evaluate each `IEquatable<U>` interface implemented by `T` where `U` is either `T`, an
            // interface of `T`, or a base class of `T`
            var intfcs = typeof(T).GetInterfaces();
            var bases = GetBasesOf(typeof(T));
            foreach (var intfc in intfcs) {
                if (intfc.IsGenericType && intfc.GetGenericTypeDefinition() == typeof(IEquatable<>)) {
                    var arg = intfc.GetGenericArguments()[0];
                    if (bases.IndexOf(arg) >= 0) {
                        var funcName = nameof(EvaluateIEquatable);
                        var funcFlags = BindingFlags.NonPublic | BindingFlags.Static;

                        var method = typeof(FullCheck).GetMethod(funcName, funcFlags)!;
                        var target = method.MakeGenericMethod(arg);
                        var result = (Option<string>)target.Invoke(null, new object?[] { lhs, rhs, expectedEqual })!;

                        if (result.HasValue) {
                            return result;
                        }
                    }
                }
            }

            // Symmetrically evaluate the inherited `object.Equals` method. We have to check for equality against the
            // inverse of the expectation so that a null evaluation result, obtained when the first operand is itself
            // null, does not yield a false positive. This approach ensures that the test only fails if the method is
            // actually invoked _and_ the result is the opposite of what was expected.
            if (lhs?.Equals(rhs) == !expectedEqual) { return Option.Some($"object.Equals({lhs}, {rhs})"); }
            if (rhs?.Equals(lhs) == !expectedEqual) { return Option.Some($"object.Equals({rhs}, {lhs})"); }

            // We can only assert on the relation between hash values if the inputs are expected to be equal due to the
            // possibility of collisions. If the two values _are_ expected to be equal, then we shouldn't have any
            // false negatives from null, as null can only ever be expected to be equal to null.
            if (expectedEqual && (lhs?.GetHashCode() != rhs?.GetHashCode())) {
                return Option.Some($"object.GetHashCode({lhs}) == object.GetHashCode({rhs})");
            }

            // All checks have passed
            return Option.None<string>();
        }
    }
}
