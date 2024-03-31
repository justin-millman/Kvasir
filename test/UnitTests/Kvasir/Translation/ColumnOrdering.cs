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

        [TestMethod] public void ReferenceFieldsOrdered() {
            // Arrange
            var translator = new Translator();
            var source = typeof(EdibleArrangement);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField(nameof(EdibleArrangement.ID)).AtColumn(3).And
                .HaveField(nameof(EdibleArrangement.Price)).AtColumn(8).And
                .HaveField(nameof(EdibleArrangement.Strawberries)).AtColumn(9).And
                .HaveField(nameof(EdibleArrangement.Bananas)).AtColumn(0).And
                .HaveField(nameof(EdibleArrangement.Grapes)).AtColumn(2).And
                .HaveField(nameof(EdibleArrangement.Cantaloupe)).AtColumn(1).And
                .HaveField(nameof(EdibleArrangement.OtherFruit)).AtColumn(4).And
                .HaveField("Vessel.FactoryID").AtColumn(6).And
                .HaveField("Vessel.Brand").AtColumn(7).And
                .HaveField("Vessel.Item").AtColumn(5).And
                .HaveNoOtherFields();
        }

        [TestMethod] public void RelationTableOrdering() {
            // Arrange
            var translator = new Translator();
            var source = typeof(DebitCard);

            // Act
            var translation = translator[source];

            // Assert
            translation.Relations[0].Table.Should()
                .HaveField("DebitCard.CardNumber").AtColumn(0).And
                .HaveField("DebitCard.Issuer").AtColumn(1).And
                .HaveField("Key").AtColumn(2).And
                .HaveField("Value").And
                .HaveNoOtherFields();
            translation.Relations[1].Table.Should()
                .HaveField("DebitCard.CardNumber").AtColumn(0).And
                .HaveField("DebitCard.Issuer").AtColumn(1).And
                .HaveField("Item.Amount").AtColumn(2).And
                .HaveField("Item.Location").AtColumn(3).And
                .HaveField("Item.Timestamp").AtColumn(4).And
                .HaveNoOtherFields();
        }

        [TestMethod] public void RelationFieldsOrdered_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Tapestry);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Tapestry.Depictions))                 // error location
                .WithMessageContaining("[Column]")                                  // details / explanation
                .WithMessageContaining("Relation");                                 // details / explanation
        }

        [TestMethod] public void ReferencePrimaryKeysAreNonSequential() {
            // Arrange
            var translator = new Translator();
            var source = typeof(MassExtinction);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField(nameof(MassExtinction.Index)).AtColumn(4).And
                .HaveField("ExitBoundary.Name").AtColumn(2).And
                .HaveField("ExitBoundary.MYA").AtColumn(3).And
                .HaveField("EntryBoundary.Name").AtColumn(0).And
                .HaveField("EntryBoundary.MYA").AtColumn(1).And
                .HaveField(nameof(MassExtinction.Severity)).AtColumn(5).And
                .HaveNoOtherFields();
        }

        [TestMethod] public void RelationAnchorPrimaryKeysAreNonSequential() {
            // Arrange
            var translator = new Translator();
            var source = typeof(DuoPush);

            // Act
            var translation = translator[source];

            // Assert
            translation.Relations[0].Table.Should()
                .HaveField("DuoPush.Timestamp").AtColumn(0).And
                .HaveField("DuoPush.DeviceID").AtColumn(1).And
                .HaveField("Item").AtColumn(2).And
                .HaveNoOtherFields();
        }

        [TestMethod] public void TwoScalarFieldsOrderedToSameIndexInEntity_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Pizza);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining("two Fields pinned to column index")         // category
                .WithMessageContaining("7");                                        // details / explanation
        }

        [TestMethod] public void TwoScalarFieldsOrderedToSameIndexInAggregate_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(BiblicalPlague);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(nameof(BiblicalPlague.Translation))          // source type
                .WithMessageContaining("two Fields pinned to column index")         // category
                .WithMessageContaining("1");                                        // details / explanation
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

        [TestMethod] public void ColumnOrderingOfReferencesLeavesGaps_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Origami);

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
