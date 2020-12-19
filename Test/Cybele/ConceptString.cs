using Cybele;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Test.Cybele.Core {
    [TestClass]
    public class ConceptStringTests {
        [TestMethod, TestCategory("Access")]
        public void AsView() {
            var contents = "Nitrogen (N)";
            var concept = new ElementString(contents);

            Assert.AreEqual(contents.Length, concept.Length);
            Assert.IsTrue(concept.View.Equals(contents, StringComparison.Ordinal));
        }

        [TestMethod, TestCategory("Access")]
        public void CharacterIndexInBounds() {
            var contents = "Terbium (Tb)";
            var concept = new ElementString(contents);

            for (var idx = 0; idx < contents.Length; ++idx) {
                Assert.AreEqual(contents[idx], concept[idx]);
            }
        }

        [TestMethod, TestCategory("Access")]
        public void CharacterIndexOutOfBounds() {
            var contents = "Dysprosium (Dy)";
            var concept = new ElementString(contents);

            foreach (var idx in new int[] { -1, contents.Length }) {
                void action() { var _ = concept[idx]; };
                var exception = Assert.ThrowsException<IndexOutOfRangeException>(action);
                Assert.AreNotEqual(string.Empty, exception.Message);
            }
        }

        [TestMethod, TestCategory("Access")]
        public void Iteration() {
            var raw = "Xenon (Xe)";
            var concept = new ElementString(raw);

            var iter = raw.GetEnumerator();
            iter.MoveNext();
            foreach (var c in concept) {
                Assert.AreEqual(iter.Current, c);
                iter.MoveNext();
            }

            iter.Reset();
            iter.MoveNext();
            foreach (var c in (concept as System.Collections.IEnumerable)) {
                Assert.AreEqual(iter.Current, c);
                iter.MoveNext();
            }
        }

        [TestMethod, TestCategory("Conversion")]
        public void ConvertNullToSpan() {
            ElementString? concept = null;
            ReadOnlySpan<char> span = concept;

            var nullSpan = ((string?)null).AsSpan();
            Assert.IsTrue(span.Equals(nullSpan, StringComparison.Ordinal));
        }

        [TestMethod, TestCategory("Conversion")]
        public void ConvertEmptyToSpan() {
            var contents = string.Empty;
            ElementString concept = new ElementString(contents);
            ReadOnlySpan<char> span = concept;

            Assert.IsTrue(span.Equals(contents.AsSpan(), StringComparison.Ordinal));
        }

        [TestMethod, TestCategory("Conversion")]
        public void ConvertNonEmptyToSpan() {
            var contents = "Bromine (Br)";
            ElementString concept = new ElementString(contents);
            ReadOnlySpan<char> span = concept;

            Assert.IsTrue(span.Equals(contents.AsSpan(), StringComparison.Ordinal));
        }

        [TestMethod, TestCategory("Conversion")]
        public void ConvertNullToString() {
            ElementString? concept = null;
            string? str = (string?)concept;

            Assert.IsNull(str);
        }

        [TestMethod, TestCategory("Conversion")]
        public void ConvertEmptyToString() {
            var contents = string.Empty;
            ElementString concept = new ElementString(contents);
            string? str = (string?)concept;

            Assert.AreEqual(contents, str);
        }

        [TestMethod, TestCategory("Conversion")]
        public void ConvertNonEmptyToString() {
            var contents = "Cadmium (Cd)";
            ElementString concept = new ElementString(contents);
            string? str = (string?)concept;

            Assert.AreEqual(contents, str);
        }

        [TestMethod, TestCategory("Equality")]
        public void StrongEquality() {
            // Scenario #1: LHS and RHS are identical
            var lhs1 = new ElementString("Antimony (Sb)");
            var rhs1 = lhs1;
            FullCheck.ExpectEqual(lhs1, rhs1);

            // Scenario #2: LHS and RHS are constructed in the same way
            var lhs2 = new ElementString("Lead (Pb)");
            var rhs2 = new ElementString("Lead (Pb)");
            FullCheck.ExpectEqual(lhs2, rhs2);

            // Scenario #3: LHS and RHS are equal in a case-insensitive evaluation only
            var lhs3 = new ElementString("Europium (Eu)");
            var rhs3 = new ElementString("EuroPiuM (EU)");
            FullCheck.ExpectNotEqual(lhs3, rhs3);

            // Scenario #4: LHS and RHS contain completely different text values
            var lhs4 = new ElementString("Strontium (Sr)");
            var rhs4 = new ElementString("Nickel (Ni)");
            FullCheck.ExpectNotEqual(lhs4, rhs4);

            // Scenario #5: Exactly one of LHS and RHS is a null ElementString?
            var lhs5 = new ElementString("Bismuth (Bi)");
            ElementString? rhs5 = null;
            FullCheck.ExpectNotEqual(lhs5, rhs5);

            // Scenario #6: Both LHS and RHS are null ElementString?
            ElementString? lhs6 = null;
            ElementString? rhs6 = null;
            FullCheck.ExpectEqual(lhs6, rhs6);
        }

        [TestMethod, TestCategory("Equality")]
        public void WeakEquality() {
            // Scenario #1: Exactly one of LHS and RHS is an ElementString and the other is its text contents
            var lhs1 = "Potassium (K)";
            var rhs1 = new ElementString(lhs1);
            FullCheck.ExpectNotEqual<object>(lhs1, rhs1);

            // Scenario #2: Exactly one of LHS and RHS is neither an ElementString nor anything string-like
            var lhs2 = new ElementString("Copper (Cu)");
            var rhs2 = 88192;
            FullCheck.ExpectNotEqual<object>(lhs2, rhs2);

            // Scenario #3: Exactly one of LHS and RHS is a null object?
            var lhs3 = new ElementString("Nihonium (Nh)");
            object? rhs3 = null;
            FullCheck.ExpectNotEqual(lhs3, rhs3);
        }

        [TestMethod, TestCategory("Equality")]
        public void DifferentInstantiationsSameContents() {
            var rawContents = "Hydrogen (H)";
            var element = new ElementString(rawContents);
            var dummy = new DummyString(rawContents);

            Assert.AreEqual((string?)element, (string?)dummy);
            FullCheck.ExpectNotEqual<object>(element, dummy);
        }

        [TestMethod, TestCategory("Comparison")]
        public void StrictTotalOrder() {
            ElementString? small = null;
            var medium = new ElementString("Argon (Ar)");
            var large = new ElementString("Selenium (Se)");
            var giant = new ElementString("Zinc (Zn)");

            FullCheck.ExpectLessThan(small, medium);
            FullCheck.ExpectLessThan(small, large);
            FullCheck.ExpectLessThan(small, giant);
            FullCheck.ExpectLessThan(medium, large);
            FullCheck.ExpectLessThan(medium, giant);
            FullCheck.ExpectLessThan(large, giant);

            FullCheck.ExpectEquivalent(small, small);
            FullCheck.ExpectEquivalent(medium, medium);
            FullCheck.ExpectEquivalent(large, large);
            FullCheck.ExpectEquivalent(giant, giant);
        }

        [TestMethod, TestCategory("ToString")]
        public void EmptyToString() {
            var contents = string.Empty;
            var concept = new ElementString(contents);
            var str = concept.ToString();

            Assert.AreEqual(contents, str);
        }

        [TestMethod, TestCategory("ToString")]
        public void NonEmptyToString() {
            var contents = "Beryllium (Be)";
            var concept = new ElementString(contents);
            var str = concept.ToString();

            Assert.AreEqual(contents, str);
        }


        private sealed class ElementString : ConceptString<ElementString> {
            public ElementString(string contents) : base(contents) {}
        }
        private sealed class DummyString : ConceptString<DummyString> {
            public DummyString(string contents) : base(contents) {}
        }
    }
}
