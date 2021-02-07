using Atropos;
using Cybele.Core;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;

namespace UT.Cybele.Core {
    [TestClass, TestCategory("ConceptString")]
    public sealed class ConceptStringTests {
        [TestMethod] public void Length() {
            // Arrange
            var contents = "Hydrogen (H)";
            var concept = new ElementString(contents);

            // Act
            var length = concept.Length;
            var expected = contents.Length;

            // Assert
            length.Should().Be(expected);
        }

        [TestMethod] public void IndexInRange() {
            // Arrange
            var contents = "Cadmium (Cd)";
            var concept = new ElementString(contents);
            var index = 4;

            // Act
            var character = concept[index];
            var expected = contents[index];

            // Assert
            character.Should().Be(expected);
        }

        [TestMethod] public void IndexTooSmall() {
            // Arrange
            var contents = "Selenium (Se)";
            var concept = new ElementString(contents);
            var index = -8;

            // Act
            Func<char> action = () => concept[index];

            // Assert
            action.Should().ThrowExactly<IndexOutOfRangeException>().WithAnyMessage();
        }

        [TestMethod] public void IndexTooLarge() {
            // Arrange
            var contents = "Dysprosium (Dy)";
            var concept = new ElementString(contents);
            var index = contents.Length + 13;

            // Act
            Func<char> action = () => concept[index];

            // Assert
            action.Should().ThrowExactly<IndexOutOfRangeException>().WithAnyMessage();
        }

        [TestMethod] public void SubstringInRange() {
            // Arrange
            var contents = "Livermorium (Lv)";
            var concept = new ElementString(contents);
            var range = new Range(2, 9);

            // Act
            var substring = concept[range].ToString();
            var expected = contents[range];

            // Assert
            substring.Should().Be(expected);
        }

        [TestMethod] public void SusbtringEndOutOfRange() {
            // Arrange
            var contents = "Tungsten (W)";
            var concept = new ElementString(contents);
            var range = new Range(4, 109);

            // Act
            Func<string> action = () => concept[range].ToString();

            // Assert
            action.Should().ThrowExactly<ArgumentOutOfRangeException>().WithAnyMessage();
        }

        [TestMethod] public void View() {
            // Arrange
            var contents = "Chlorine (Cl)";
            var concept = new ElementString(contents);

            // Act
            var view = concept.View;
            var expected = contents.AsSpan();

            // Assert
            view.ToString().Should().Be(expected.ToString());
        }

        [TestMethod] public void ConvertNonNullToString() {
            // Arrange
            var contents = "Niobium (Nb)";
            var concept = new ElementString(contents);

            // Act
            var conversion = (string)concept!;

            // Assert
            conversion.Should().Be(contents);
        }

        [TestMethod] public void ConvertNonNullToCharSpan() {
            // Arrange
            var contents = "Oxygen (O)";
            var concept = new ElementString(contents);

            // Act
            var conversion = ((ReadOnlySpan<char>)concept).ToString();

            // Assert
            conversion.Should().Be(contents);
        }

        [TestMethod] public void ConvertNullToString() {
            // Arrange
            ElementString? concept = null;

            // Act
            var conversion = (string?)concept;

            // Assert
            conversion.Should().BeNull();
        }

        [TestMethod] public void ConvertNullToCharSpan() {
            // Arrange
            ElementString? concept = null;

            // Act
            var conversion = ((ReadOnlySpan<char>)concept).ToString();
            var expected = string.Empty;

            // Assert
            conversion.Should().Be(expected);
        }

        [TestMethod] public void StrongEquality() {
            // Arrange
            var nickel = new ElementString("Nickel (Ni)");
            var copper = new ElementString("Copper (Cu)");
            ElementString? nll = null;

            // Act
            var areEqual = FullCheck.ExpectEqual(nickel, nickel);
            var areNotEqual = FullCheck.ExpectNotEqual(nickel, copper);
            var nullAreEqual = FullCheck.ExpectEqual(nll, nll);
            var nullAreNotEqual = FullCheck.ExpectNotEqual(copper, nll);

            // Assert
            areEqual.Should().NotHaveValue();
            areNotEqual.Should().NotHaveValue();
            nullAreEqual.Should().NotHaveValue();
            nullAreNotEqual.Should().NotHaveValue();
        }

        [TestMethod] public void WeakEquality() {
            // Arrange
            var mercury = new ElementString("Mercury (Hg)");
            var nonElement = "Mercury (Hg)";
            object? nll = null;

            // Act
            var areNotEqual = FullCheck.ExpectNotEqual<object>(mercury, nonElement);
            var nullAreNotEqual = FullCheck.ExpectNotEqual(mercury, nll);

            // Assert
            areNotEqual.Should().NotHaveValue();
            nullAreNotEqual.Should().NotHaveValue();
        }

        [TestMethod] public void StrictTotalOrder() {
            // Arrange
            var dubnium = new ElementString("Dubnium (Db)");
            var einsteinium = new ElementString("Einsteinium (Es)");
            ElementString? nll = null;

            // Act
            var nonEquiv = FullCheck.ExpectLessThan(dubnium, einsteinium);
            var equiv = FullCheck.ExpectEquivalent(dubnium, dubnium);
            var nullNonEquiv = FullCheck.ExpectLessThan(nll, einsteinium);
            var nullEquiv = FullCheck.ExpectEquivalent(nll, nll);

            // Assert
            nonEquiv.Should().NotHaveValue();
            equiv.Should().NotHaveValue();
            nullNonEquiv.Should().NotHaveValue();
            nullEquiv.Should().NotHaveValue();
        }

        [TestMethod] public void Stringification() {
            // Arrange
            var contents = "Beryllium (Be)";
            var concept = new ElementString(contents);

            // Act
            var toString = concept.ToString();

            // Assert
            toString.Should().Be(contents);
        }

        [TestMethod] public void StrongIteration() {
            // Arrange
            var contents = "Silver (Ag)";
            var concept = new ElementString(contents);

            // Act
            var fromIteration = string.Empty;
            foreach (var c in concept) { fromIteration += c; }

            // Assert
            fromIteration.Should().Be(contents);
        }

        [TestMethod] public void WeakIteration() {
            // Arrange
            var contents = "Gold (Au)";
            var concept = new ElementString(contents);

            // Act
            var fromIteration = string.Empty;
            foreach (var c in (IEnumerable)concept) { fromIteration += (char)c; }

            // Assert
            fromIteration.Should().Be(contents);
        }


        private sealed class ElementString : ConceptString<ElementString> {
            public ElementString(string contents) : base(contents) {}
        }
    }
}
