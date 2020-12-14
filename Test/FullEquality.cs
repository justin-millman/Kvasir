using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Test {
    internal static class FullCheck {
        public static void ExpectEqual<T>(T lhs, T rhs) {
            TestEquality(lhs, rhs, true);
        }

        public static void ExpectUnequal<T>(T lhs, T rhs) {
            TestEquality(lhs, rhs, false);
        }

        private static void TestEquality<T>(T lhs, T rhs, bool expectedEqual) {
            dynamic? dynLhs = lhs;
            dynamic? dynRhs = rhs;

            // Evaluate operator== symmetrically if it is present
            Evaluate(() => Assert.AreEqual(dynLhs == dynRhs, expectedEqual));
            Evaluate(() => Assert.AreEqual(dynRhs == dynLhs, expectedEqual));

            // Evaluate operator!= symmetrically if it is present
            Evaluate(() => Assert.AreNotEqual(dynLhs != dynRhs, expectedEqual));
            Evaluate(() => Assert.AreNotEqual(dynRhs != dynLhs, expectedEqual));

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
                        target.Invoke(null, new object?[] { dynLhs, dynRhs, expectedEqual });
                    }
                }
            }

            // Evaluate the inherited Object.Equals method
            Assert.AreNotEqual(lhs?.Equals(rhs), !expectedEqual);
            Assert.AreNotEqual(rhs?.Equals(lhs), !expectedEqual);

            // Evaluate the inherited Object.GetHashCode method if the objects are expected to be equal
            if (expectedEqual) {
                Assert.AreEqual(lhs?.GetHashCode(), rhs?.GetHashCode());
            }
        }

        private static void Evaluate(Action action) {
            try {
                action();
            }
            catch (Exception) {}
        }

        private static void EvaluateIEquatable<U>(U lhs, U rhs, bool expectedEqual) {
            Assert.AreNotEqual((lhs as IEquatable<U>)?.Equals(rhs), !expectedEqual);
            Assert.AreNotEqual((rhs as IEquatable<U>)?.Equals(lhs), !expectedEqual);
        }

        private static List<Type> GetBasesOf(Type type) {
            var results = new List<Type>() { type };
            results.AddRange(type.GetInterfaces());

            for (Type? t = type.BaseType; !(t is null); t = t.BaseType) {
                results.Add(t);
            }

            return results;
        }
    }
}
