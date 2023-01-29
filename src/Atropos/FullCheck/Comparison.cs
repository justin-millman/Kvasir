using Optional;
using System;
using System.Reflection;

namespace Atropos {
    /// <summary>
    ///   A class that exposes <see langword="static"/> evaluation methods for checking user-defined equality and
    ///   ordering APIs.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     The purpose of the <see cref="FullCheck"/> class is to provide a simple interface by which to evaluate the
    ///     full slate of API methods that describe equality and ordering for custom types. These functions are
    ///     generally expected to be internally consistent, but it can be tedious to manually evaluate each one
    ///     individually. This is exacerbated by the optionality of most elements of the API and the relative
    ///     difficulty of discovering their presence for an arbitrary type. The functions exposed by
    ///     <see cref="FullCheck"/> allow unit tests to succinctly express their intents without being concerned as to
    ///     the actual methods that are physically invoked.
    ///   </para>
    ///   <para>
    ///     When evaluating equality, <see cref="FullCheck"/> tests the most appropriate overload of any
    ///     <see langword="static"/> binary equality operators, the inherited <see cref="object.Equals(object?)"/>
    ///     method, and any strongly typed <c>Equals(T?)</c> methods brought in via <see cref="IEquatable{T}"/>.
    ///     Furthermore, if equality is expected, the <see cref="object.GetHashCode"/> method is evaluated as well.
    ///     Appropriate symmetry is assumed: i.e. if testing that <c>A</c> is equal to <c>B</c>,
    ///     <see cref="FullCheck"/> will also test that <c>B</c> is equal to <c>A</c>.
    ///   </para>
    ///   <para>
    ///     When evaluating ordering, <see cref="FullCheck"/> tests the most appropriate overload of any
    ///     <see langword="static"/> binary ordering operators and any strongly typed <c>CompareTo(T?)</c> methods
    ///     brought in via <see cref="IComparable{T}"/>. Note that neither <c>operator==</c> nor <c>operator!=</c> is
    ///     evaluated for ordering, as <i>equivalence</i> and <i>equality</i> are treated separately. Appropriate
    ///     symmetry is also assumed: i.e. if testing that <c>A</c> is strictly less than <c>B</c>,
    ///     <see cref="FullCheck"/> will also test that <c>B</c> is strictly greater than <c>A</c>.
    ///   </para>
    ///   <para>
    ///     <see cref="FullCheck"/> is intended as a utility for unit testing but is not tied to any particular unit
    ///     testing framework or paradigm. To support the client's choice of testing platform, all of the APIs exposed
    ///     by <see cref="FullCheck"/> return an optional <see cref="string"/> denoting the result of the evaluation.
    ///     If the evaluation succeeds (i.e. the test passes), the optional will be a <c>NONE</c> instance. If, on the
    ///     other hand, the evaluation fails (i.e. the test fails), the optional will be a <c>SOME</c> instance that
    ///     contains the failed expression. This allows users to build thin adapters on top of the methods to handle
    ///     failure appropriately, including a useful error message if so desired.
    ///   </para>
    /// </remarks>
    public static partial class FullCheck {
        /// <summary>
        ///   Determines if the full ordering API exposed by a type indicates that one instance of that type is
        ///   strictly less than another instance.
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
        ///   A <c>SOME</c> instance with the expression that, when evaluated, indicates that <paramref name="lhs"/> is
        ///   not strictly less than <paramref name="rhs"/> if such an expression exists; otherwise, a <c>NONE</c>
        ///   instance.
        /// </returns>
        public static Option<string> ExpectLessThan<T>(T lhs, T rhs) {
            return TestOrdering(lhs, rhs, -1);
        }

        /// <summary>
        ///   Determines if the full ordering API exposed by a type indicates that one instance of that type is
        ///   equivalent to another instance.
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
        ///   A <c>SOME</c> instance with the expression that, when evaluated, indicates that <paramref name="lhs"/> is
        ///   not equivalent to <paramref name="rhs"/> if such an expression exists; otherwise, a <c>NONE</c> instance.
        /// </returns>
        public static Option<string> ExpectEquivalent<T>(T lhs, T rhs) {
            return TestOrdering(lhs, rhs, 0);
        }

        /// <summary>
        ///   Determines if the full ordering API exposed by a type indicates that one instance of that type is
        ///   strictly greater than another instance.
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
        ///   A <c>SOME</c> instance with the expression that, when evaluated, indicates that <paramref name="lhs"/> is
        ///   not strictly greater than <paramref name="rhs"/> if such an expression exists; otherwise, a <c>NONE</c>
        ///   instance.
        /// </returns>
        public static Option<string> ExpectGreaterThan<T>(T lhs, T rhs) {
            return TestOrdering(lhs, rhs, 1);
        }

        /// <summary>
        ///   Determines if the full ordering API exposed by a type indicates that one instance of that type is
        ///   incomparable to (i.e. neither strictly less than nor equivalent to nor strictly greater than) another
        ///   instance.
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
        ///   A <c>SOME</c> instance with the expression that, when evaluated, indicates that <paramref name="lhs"/> is
        ///   not incomparable to <paramref name="rhs"/> if such an expression exists; otherwise, a <c>NONE</c>
        ///   instance.
        /// </returns>
        public static Option<string> ExpectIncomparable<T>(T lhs, T rhs) {
            return TestOrdering(lhs, rhs, null);
        }

        /// <summary>
        ///   Evaluates the ordering relation between two instancs of a type using the full ordering API of that type.
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
        private static Option<string> TestOrdering<T>(T lhs, T rhs, int? trichotomy) {
            var expectedLess = trichotomy is not null && trichotomy < 0;
            var expectedLessEqual = trichotomy is not null && trichotomy <= 0;
            var expectedGreater = trichotomy > 0;
            var expectedGreaterEqual = trichotomy >= 0;

            var ltLR = GetOperator<T>(Operator.LT, expectedLess);
            var gtLR = GetOperator<T>(Operator.GT, expectedGreater);
            var lteLR = GetOperator<T>(Operator.LTE, expectedLessEqual);
            var gteLR = GetOperator<T>(Operator.GTE, expectedGreaterEqual);
            var ltRL = GetOperator<T>(Operator.LT, expectedGreater);
            var gtRL = GetOperator<T>(Operator.GT, expectedLess);
            var lteRL = GetOperator<T>(Operator.LTE, expectedGreaterEqual);
            var gteRL = GetOperator<T>(Operator.GTE, expectedLessEqual);

            // Symmetrically evaluate operator< if it is present on `T` or a base class of `T`, selecting the overload
            // accepting the most derived type
            if (ltLR(lhs, rhs) != expectedLess) { return Option.Some($"{lhs} < {rhs}"); }
            if (ltRL(rhs, lhs) != expectedGreater) { return Option.Some($"{rhs} < {lhs}"); }

            // Symmetrically evaluate operator> if it is present on `T` or a base class of `T`, selecting the overload
            // accepting the most derived type
            if (gtLR(lhs, rhs) != expectedGreater) { return Option.Some($"{lhs} > {rhs}"); }
            if (gtRL(rhs, lhs) != expectedLess) { return Option.Some($"{rhs} > {lhs}"); }

            // Symmetrically evaluate operator<= if it is present on `T` or a base class of `T`, selecting the overload
            // accepting the most derived type
            if (lteLR(lhs, rhs) != expectedLessEqual) { return Option.Some($"{lhs} <= {rhs}"); }
            if (lteRL(rhs, lhs) != expectedGreaterEqual) { return Option.Some($"{rhs} <= {lhs}"); }

            // Symmetrically evaluate operator>= if it is present on `T` or a base class of `T`, selecting the overload
            // accepting the most derived type
            if (gteLR(lhs, rhs) != expectedGreaterEqual) { return Option.Some($"{lhs} >= {rhs}"); }
            if (gteRL(rhs, lhs) != expectedLessEqual) { return Option.Some($"{rhs} >= {lhs}"); }

            // Symmetrically evaluate each `IComparable<U>` interface implemented by `T` where `U` is either `T`, an
            // interface of `T`, or a base class of `T`
            var intfcs = typeof(T).GetInterfaces();
            var bases = GetBasesOf(typeof(T));
            foreach (var intfc in intfcs) {
                if (intfc.IsGenericType && intfc.GetGenericTypeDefinition() == typeof(IComparable<>)) {
                    var arg = intfc.GetGenericArguments()[0];
                    if (bases.IndexOf(arg) >= 0) {
                        var funcName = nameof(EvaluateIComparable);
                        var funcFlags = BindingFlags.NonPublic | BindingFlags.Static;

                        var method = typeof(FullCheck).GetMethod(funcName, funcFlags)!;
                        var target = method.MakeGenericMethod(arg);
                        var result = (Option<string>)target.Invoke(null, new object?[] { lhs, rhs, trichotomy })!;

                        if (result.HasValue) {
                            return result;
                        }
                    }
                }
            }

            // All checks have passed
            return Option.None<string>();
        }
    }
}
