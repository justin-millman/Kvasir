using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Test {
    internal static partial class FullCheck {
        public static void ExpectEqual<T>(T lhs, T rhs) {
            TestEquality(lhs, rhs, true);
        }

        public static void ExpectNotEqual<T>(T lhs, T rhs) {
            TestEquality(lhs, rhs, false); 
        }

        public static void ExpectLessThan<T>(T lhs, T rhs) {
            TestComparison(lhs, rhs, -1);
        }

        public static void ExpectEquivalent<T>(T lhs, T rhs) {
            TestComparison(lhs, rhs, 0);
        }

        public static void ExpectGreaterThan<T>(T lhs, T rhs) {
            TestComparison(lhs, rhs, 1);
        }

        public static void ExpectIncomparable<T>(T lhs, T rhs) {
            TestComparison(lhs, rhs, null);
        }

        //===========================================================================================================

        private static void TestEquality<T>(T lhs, T rhs, bool expectAreEqual) {
            var eq = GetOperator<T>(Operator.EQ, expectAreEqual);
            var neq = GetOperator<T>(Operator.NEQ, !expectAreEqual);

            // Evaluate operator== symmetrically if it is defined on T or a base class of T, selecting the overload
            // defined on the most-derived class
            Assert.AreEqual(expectAreEqual, eq(lhs, rhs));
            Assert.AreEqual(expectAreEqual, eq(rhs, lhs));

            // Evaluate operator!= symmetrically if it is definedo n T or a base class of T, selecting the overload
            // defined on the most-derived class
            Assert.AreEqual(!expectAreEqual, neq(lhs, rhs));
            Assert.AreEqual(!expectAreEqual, neq(rhs, lhs));

            // For each instantiation of IEquatable<U> implemented by T where U is T or a base class or interface of T,
            // evaluate the corresponding IEquatable<U>.Equals method symmetrically
            var intfcs = typeof(T).GetInterfaces();
            var bases = GetBasesOf(typeof(T));
            foreach (var intfc in intfcs) {
                if (intfc.IsGenericType && intfc.GetGenericTypeDefinition() == typeof(IEquatable<>)) {
                    var arg = intfc.GetGenericArguments()[0];
                    if (bases.IndexOf(arg) >= 0) {
                        var name = nameof(EvaluateIEquatable);
                        var flags = BindingFlags.Static | BindingFlags.NonPublic;

                        var method = typeof(FullCheck).GetMethod(name, flags)!;
                        var target = method.MakeGenericMethod(arg);
                        target.Invoke(null, new object?[] { lhs, rhs, expectAreEqual });
                    }
                }
            }

            // Evaluate the inherited Object.Equals method
            Assert.AreNotEqual(!expectAreEqual, lhs?.Equals(rhs));
            Assert.AreNotEqual(!expectAreEqual, rhs?.Equals(lhs));

            // Evaluate the inherited Object.GetHashCode method if the objects are expected to be equal
            if (expectAreEqual) {
                Assert.AreEqual(lhs?.GetHashCode(), rhs?.GetHashCode());
            }
        }

        private static void TestComparison<T>(T lhs, T rhs, int? trichotomy) {
            var lt = GetOperator<T>(Operator.LT, !(trichotomy is null) && trichotomy < 0);
            var gt = GetOperator<T>(Operator.GT, trichotomy > 0);
            var lte = GetOperator<T>(Operator.LTE, !(trichotomy is null) && trichotomy <= 0);
            var gte = GetOperator<T>(Operator.GTE, trichotomy >= 0);

            // Evaluate operator< and operator> symmetrically if they are defined on T or a base class of T, selecting
            // the overload defined on the most-derived class
            Assert.AreEqual(!(trichotomy is null) && trichotomy < 0, lt(lhs, rhs));
            Assert.AreEqual(trichotomy > 0, lt(rhs, lhs));
            Assert.AreEqual(trichotomy > 0, gt(lhs, rhs));
            Assert.AreEqual(!(trichotomy is null) && trichotomy < 0, gt(rhs, lhs));

            // Evaluate operator<= and operator>= symmetrically if they are defined on T or a base class of T,
            // selecting the overload defined on the most-derived class
            Assert.AreEqual(!(trichotomy is null) && trichotomy <= 0, lte(lhs, rhs));
            Assert.AreEqual(trichotomy >= 0, lte(rhs, lhs));
            Assert.AreEqual(trichotomy >= 0, gte(lhs, rhs));
            Assert.AreEqual(!(trichotomy is null) && trichotomy <= 0, gte(rhs, lhs));

            // For each instantiation of IComparable<U> implemented by T where U is T or a base class or interface of
            // T, evaluate the corresponding IComparable<U>.CompareTo method symmetrically
            var intfcs = typeof(T).GetInterfaces();
            var bases = GetBasesOf(typeof(T));
            foreach (var intfc in intfcs) {
                if (intfc.IsGenericType && intfc.GetGenericTypeDefinition() == typeof(IComparable<>)) {
                    var arg = intfc.GetGenericArguments()[0];
                    if (bases.IndexOf(arg) >= 0) {
                        var name = nameof(EvaluateIComparable);
                        var flags = BindingFlags.Static | BindingFlags.NonPublic;

                        var method = typeof(FullCheck).GetMethod(name, flags)!;
                        var target = method.MakeGenericMethod(arg);
                        target.Invoke(null, new object?[] { lhs, rhs, trichotomy });
                    }
                }
            }
        }

        // ==========================================================================================================

        private static BinaryOp GetOperator<T>(Operator op, bool fallback) {
            var pair = (typeof(T), op);
            if (!opMemoizer_.TryGetValue(pair, out BinaryOp? result)) {
                // Using the .NET library reflection tools allows us to avoid manually performing complex overload
                // resolution rules. That being said, the Type.GetMethod function only performs a limited subset of
                // C#'s full overload resolution. Specifically:
                //   > Non-narrowing implicit numeric conversions
                //   > Derived-to-Base implicit conversions
                //   > Implementation-to-Interface implicit conversions
                //   > Non-Reference argument to By-Reference argument
                // Numeric conversions are not of interest to us, because the built-in numeric primitives do not
                // implement these operators as static members the way user-defined types do: rather, they are
                // implemented directly in the IL. Conspicuously absent from this list of supported resolution
                // conversions are user-defined implicit conversions and conversions based on covariance and/or
                // contravariance. Therefore, operators defined relying on those conversions will not be detected,
                // which is fine for the current use case as those operators would have lower precedence than ones
                // based on inheritance conversions (and thus there will be no false positives).
                var flags = BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy;
                var types = new Type[] { typeof(T), typeof(T) };
                var method = typeof(T).GetMethod(opNames_[op], flags, Type.DefaultBinder, types, null);

                // The C# language specification does not require that the binary relational operators return a
                // Boolean, but this is a requirement for our semantic use case
                if (method?.ReturnType == typeof(bool)) {
                    // The first argument to MethodInfo.Invoke is the 'this' argument for the method. These operators
                    // are static, so a valid instance is not necessary. This is fortuitous, because we don't actually
                    // have a valid instance to use.
                    result = (l, r) => (bool)method.Invoke(null, new object?[] { l, r })!;
                }
                opMemoizer_[pair] = result;
            }

            return (l, r) => result?.Invoke(l, r) ?? fallback;
        }

        private static void EvaluateIEquatable<U>(U lhs, U rhs, bool expectedEqual) {
            Assert.AreNotEqual((lhs as IEquatable<U>)?.Equals(rhs), !expectedEqual);
            Assert.AreNotEqual((rhs as IEquatable<U>)?.Equals(lhs), !expectedEqual);
        }

        private static void EvaluateIComparable<U>(U lhs, U rhs, int? trichotomy) {
            var lhsCompRhs = (lhs as IComparable<U>)?.CompareTo(rhs);
            var rhsCompLhs = (rhs as IComparable<U>)?.CompareTo(lhs);

            var expected = trichotomy is null ? 0 : (trichotomy < 0 ? 1 : (trichotomy == 0 ? 2 : 3));
            var lrActaul = lhsCompRhs is null ? 0 : (lhsCompRhs < 0 ? 1 : (lhsCompRhs == 0 ? 2 : 3));
            var rlActual = rhsCompLhs is null ? 0 : (rhsCompLhs < 0 ? 1 : (rhsCompLhs == 0 ? 2 : 3));

            if (!(lhs is null)) {
                Assert.AreEqual(expected, lrActaul);
            }
            if (!(rhs is null)) {
                Assert.AreEqual((expected * 3) % 4, rlActual);
            }

            // I'm not sure of the exact math behind the expression (expected * 3) % 4, but it achieves the desired
            // affect of mapping the space of {0,1,2,3} like so:
            //    0 ---> 0
            //    1 ---> 3
            //    2 ---> 2
            //    3 ---> 1
            // This is the intended mapping as:
            //    LHS incomp. RHS ===> RHS incomp. LHS [0 --> 0]
            //    LHS < RHS ===> RHS > LHS [1 --> 3]
            //    LHS == RHS ===> RHS == LHS [2 --> 2]
            //    LHS > RHS ===> RHS < LHS [3 --> 1]
        }

        private static List<Type> GetBasesOf(Type type) {
            var results = new List<Type>() { type };
            results.AddRange(type.GetInterfaces());

            for (Type? t = type.BaseType; !(t is null); t = t.BaseType) {
                results.Add(t);
            }

            return results;
        }

        static FullCheck() {
            opMemoizer_ = new Dictionary<(Type, Operator), BinaryOp?>();
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
        private enum Operator : byte { EQ, NEQ, LT, GT, LTE, GTE }

        private static IDictionary<(Type, Operator), BinaryOp?> opMemoizer_;
        private static IDictionary<Operator, string> opNames_;
    }
}
