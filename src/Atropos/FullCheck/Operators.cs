using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace Atropos {
    public static partial class FullCheck {
        /// <summary>
        ///   Looks up a binary operator function for a particular <see cref="Type"/>. The result is a generalized form
        ///   of a binary operator that can be invoked, producing a default fallback if the target <see cref="Type"/>
        ///   does not actually support the specified operator.
        /// </summary>
        /// <typeparam name="T">
        ///   [explicit] The <see cref="Type"/> on which to look up the binary operator.
        /// </typeparam>
        /// <param name="op">
        ///   The binary operator function to look up.
        /// </param>
        /// <param name="fallback">
        ///   The fallback result, in the event that <typeparamref name="T"/> does not support binary operator
        ///   <paramref name="op"/>.
        /// </param>
        /// <pre>
        ///   <paramref name="op"/> is valid.
        /// </pre>
        /// <returns>
        ///   A function that can be invoked on two instances of <typeparamref name="T"/> and either returns the result
        ///   of evaluating <paramref name="op"/> on those two instances (if <typeparamref name="T"/> supports the
        ///   binary operator) or returns <paramref name="fallback"/> (otherwise).
        /// </returns>
        private static BinaryOp GetOperator<T>(Operator op, bool fallback) {
            Debug.Assert(opNames_.ContainsKey(op));

            var pair = (typeof(T), op);
            if (!memoizer_.TryGetValue(pair, out BinaryOp? result)) {
                // Using the .NET library's reflection facilities allows us to avoid manually performing complex
                // overload resolution rules. That being said, the `Type.GetMethod` function only performs a subset of
                // C#'s full overload resolution. Specifically, it resolves:
                //   > Non-narrowing numeric implicit conversions
                //   > Derived-to-Base implicit conversions
                //   > Implementation-to-Interface implicit conversions
                //   > Non-Reference argument to By-Reference argument conversions
                // Numeric conversions are not of interest to us, as operators for built-in numeric types are
                // implemented directly in the IL rather than as a standard static function. Conspicuously absent from
                // this list is implicit user-defined conversions, which will therefore not be considered.
                var argTypes = new Type[] { typeof(T), typeof(T) };
                var method = typeof(T).GetMethod(opNames_[op], OPERATOR_FLAGS, Type.DefaultBinder, argTypes, null);

                // The C# language specification does not require that the operator functions return a Boolean, but
                // that is the only logical return type for our purposes. If the operator function is found but the
                // return type is not `bool`, we'll ignore the operator. Since functions cannot be overloaded by return
                // type, this can't yield false negatives.
                if (method?.ReturnType == typeof(bool)) {
                    // The first argument to `MethodInfo.Invoke` is the `this` parameter for the method. These
                    // operators are all static, so the `this` parameter is irrelevant; this is fortuitous, because we
                    // don't have a way to obtain a guaranteed-non-null instance of T.
                    result = (l, r) => (bool)method.Invoke(null, new object?[] { l, r })!;
                }

                // Memoize the result so that we only perform the reflection on time per type per operator
                memoizer_[pair] = result;
            }

            // If the operator is not available, the result will be null and the fallback result will be returned.
            // Otherwise, the underlying static operator method will be invoked with the provided arguments.
            return (l, r) => result?.Invoke(l, r) ?? fallback;
        }

        /// <summary>
        ///   Initializes the <see langword="static"/> state of the <see cref="FullCheck"/> class.
        /// </summary>
        static FullCheck() {
            OPERATOR_FLAGS = BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy;
            memoizer_ = new ConcurrentDictionary<(Type, Operator), BinaryOp?>();
            opNames_ = new Dictionary<Operator, string>() {
                [Operator.EQ] = "op_Equality",
                [Operator.NEQ] = "op_Inequality",
                [Operator.LT] = "op_LessThan",
                [Operator.GT] = "op_GreaterThan",
                [Operator.LTE] = "op_LessThanOrEqual",
                [Operator.GTE] = "op_GreaterThanOrEqual"
            };
        }


        private delegate bool BinaryOp(object? lhs, object? rhs);
        private enum Operator : byte { EQ, NEQ, LT, GT, LTE, GTE };

        private static readonly IDictionary<(Type, Operator), BinaryOp?> memoizer_;
        private static readonly IReadOnlyDictionary<Operator, string> opNames_;
        private static readonly BindingFlags OPERATOR_FLAGS;
    }
}
