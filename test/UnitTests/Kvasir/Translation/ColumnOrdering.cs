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
                .HaveField(nameof(Parashah.Book)).AtColumn(0).And
                .HaveField(nameof(Parashah.StartChapter)).AtColumn(1).And
                .HaveField(nameof(Parashah.StartVerse)).AtColumn(3).And
                .HaveField(nameof(Parashah.EndChapter)).AtColumn(4).And
                .HaveField(nameof(Parashah.EndVerse)).AtColumn(2).And
                .HaveNoOtherFields();
        }

        [TestMethod] public void AggregateFieldsOrdered() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Armada);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField(nameof(Armada.ID)).AtColumn(1).And
                .HaveField(nameof(Armada.Commander)).AtColumn(0).And
                .HaveField(nameof(Armada.Sponsor)).AtColumn(2).And
                .HaveField("Flagship.Name").AtColumn(5).And
                .HaveField("Flagship.Class").AtColumn(3).And
                .HaveField("Flagship.Munitions").AtColumn(4).And
                .HaveField("Secondary.Name").AtColumn(9).And
                .HaveField("Secondary.Class").AtColumn(7).And
                .HaveField("Secondary.Munitions").AtColumn(8).And
                .HaveField("Tertiary.Name").AtColumn(12).And
                .HaveField("Tertiary.Class").AtColumn(10).And
                .HaveField("Tertiary.Munitions").AtColumn(11).And
                .HaveField(nameof(Armada.VictoryPercentage)).AtColumn(6).And
                .HaveNoOtherFields();
        }

        [TestMethod] public void TwoScalarFieldsOrderedToSameIndex_IsError() {
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

        [TestMethod] public void TwoNestedFieldsOrderedToSameIndex_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Coup);

            // Act
            var translate = () => translator[source];

            // Assert
            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining("two Fields pinned to column index")         // category
                .WithMessageContaining("3");                                        // details / explanation
        }

        [TestMethod] public void ScalarAndNestedFieldOrderedToSameIndex_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Bread);

            // Act
            var translate = () => translator[source];

            // Assert
            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining("two Fields pinned to column index")         // category
                .WithMessageContaining("4");                                        // details / explanation
        }

        [TestMethod] public void ColumnOrderingOfScalarsLeavesGaps_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(PhoneNumber);

            // Act
            var translate = () => translator[source];

            // Assert
            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining("unable to assign Fields to columns")        // category
                .WithMessageContaining("gaps");                                     // details / explanation
        }

        [TestMethod] public void ColumnOrderingOfAggregatesLeavesGaps_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Verb);

            // Act
            var translate = () => translator[source];

            // Assert
            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining("unable to assign Fields to columns")        // category
                .WithMessageContaining("gaps");                                     // details / explanation
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
