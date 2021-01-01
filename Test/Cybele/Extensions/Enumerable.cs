using Cybele.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Test.Cybele.Extensions {
    [TestClass]
    public class EnumerableExtensionTests {
        [TestMethod, TestCategory("Enumerator-to-Enumerable")]
        public void EmptyEnumeratorToEnumerable() {
            var list = new List<int>();
            var enumerator = list.GetEnumerator();

            var enumerable = enumerator.ToEnumerable();
            Assert.IsFalse(enumerable.Any());
        }

        [TestMethod, TestCategory("Enumerator-to-Enumerable")]
        public void NonEmptyEnumeratorToEnumerable() {
            var list = new List<string>() { "Chopped", "Supermarket Stakeout", "Good Eats", "Guys Grocery Games" };
            var enumerator = list.GetEnumerator();

            var enumerable = enumerator.ToEnumerable();
            Assert.IsTrue(enumerable.SequenceEqual(list));
        }

        [TestMethod, TestCategory("Enumerator-to-Enumerable")]
        public void EnumeratorResetAfterToEnumerable() {
            var list = new List<char>() { 'C', 'h', 'i', 'c', 'a', 'g', 'o', ' ', 'C', 'u', 'b', 's' };
            var enumerator = list.GetEnumerator();

            var _ = enumerator.ToEnumerable();
            foreach (var c in list) {
                enumerator.MoveNext();
                Assert.AreEqual(c, enumerator.Current);
            }
        }
    }
}
