using FluentAssertions;
using Kvasir.Exceptions;
using Kvasir.Translation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using static UT.Kvasir.Translation.ColumnOrdering;

namespace UT.Kvasir.Translation {
    [TestClass, TestCategory("Column Ordering")]
    public class ColumnOrderingTests {
        [TestMethod] public void AllFieldsOrdered() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Fraction);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField(nameof(Fraction.Numerator)).AtColumn(2).And
                .HaveField(nameof(Fraction.Denominator)).AtColumn(1).And
                .HaveField(nameof(Fraction.IsNegative)).AtColumn(0).And
                .HaveNoOtherFields();
        }

        [TestMethod] public void SomeScalarFieldsOrdered() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Parashah);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField(nameof(Parashah.Book)).AtSomeColumn().And
                .HaveField(nameof(Parashah.StartChapter)).AtSomeColumn().And
                .HaveField(nameof(Parashah.StartVerse)).AtSomeColumn().And
                .HaveField(nameof(Parashah.EndChapter)).AtColumn(2).And
                .HaveField(nameof(Parashah.EndVerse)).AtColumn(3).And
                .HaveNoOtherFields();
        }

        [TestMethod] public void TwoFieldsOrderedToSameIndex_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Pizza);

            // Act
            var translate = () => translator[source];

            // Assert
            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining("two Fields pinned to column index")         // category
                .WithMessageContaining("7");                                        // details / explanation
        }

        [TestMethod] public void ColumnOrderingLeavesGaps_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(PhoneNumber);

            // Act
            var translate = () => translator[source];

            // Assert
            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining("gaps at column index(es)")                  // category
                .WithMessageContaining("2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13");   // details / explanation
        }

        [TestMethod] public void NegativeColumnIndex_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(NationalPark);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(NationalPark.Established))            // error location
                .WithMessageContaining("[Column]")                                  // details / explanation
                .WithMessageContaining("negative")                                  // details / explanation
                .WithMessageContaining("-196");                                     // details / explanation
        }
    }
}
