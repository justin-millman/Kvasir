using FluentAssertions;
using Kvasir.Translation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using static UT.Kvasir.Translation.Globals;
using static UT.Kvasir.Translation.ColumnOrdering;

namespace UT.Kvasir.Translation {
    [TestClass, TestCategory("Column Ordering")]
    public class ColumnOrderingTests {
        [TestMethod] public void AllFieldsOrdered() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Fraction);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField("Numerator").AtColumn(2).And
                .HaveField("Denominator").AtColumn(1).And
                .HaveField("IsNegative").AtColumn(0).And
                .HaveNoOtherFields();
        }

        [TestMethod] public void SomeScalarFieldsOrdered() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Parashah);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField("Book").AtColumn(0).And
                .HaveField("StartChapter").AtColumn(1).And
                .HaveField("StartVerse").AtColumn(3).And
                .HaveField("EndChapter").AtColumn(4).And
                .HaveField("EndVerse").AtColumn(2).And
                .HaveNoOtherFields();
        }

        [TestMethod] public void AggregateFieldsOrdered() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Armada);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField("ID").AtColumn(1).And
                .HaveField("Commander").AtColumn(0).And
                .HaveField("Sponsor").AtColumn(2).And
                .HaveField("Flagship.Name").AtColumn(5).And
                .HaveField("Flagship.Class").AtColumn(3).And
                .HaveField("Flagship.Munitions").AtColumn(4).And
                .HaveField("Secondary.Name").AtColumn(9).And
                .HaveField("Secondary.Class").AtColumn(7).And
                .HaveField("Secondary.Munitions").AtColumn(8).And
                .HaveField("Tertiary.Name").AtColumn(12).And
                .HaveField("Tertiary.Class").AtColumn(10).And
                .HaveField("Tertiary.Munitions").AtColumn(11).And
                .HaveField("VictoryPercentage").AtColumn(6).And
                .HaveNoOtherFields();
        }

        [TestMethod] public void ReferenceFieldsOrdered() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(EdibleArrangement);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField("ID").AtColumn(3).And
                .HaveField("Price").AtColumn(8).And
                .HaveField("Strawberries").AtColumn(9).And
                .HaveField("Bananas").AtColumn(0).And
                .HaveField("Grapes").AtColumn(2).And
                .HaveField("Cantaloupe").AtColumn(1).And
                .HaveField("OtherFruit").AtColumn(4).And
                .HaveField("Vessel.FactoryID").AtColumn(6).And
                .HaveField("Vessel.Brand").AtColumn(7).And
                .HaveField("Vessel.Item").AtColumn(5).And
                .HaveNoOtherFields();
        }

        [TestMethod] public void RelationTableOrdering() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
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
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Tapestry);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Tapestry` → <synthetic> `Depictions`")
                .WithProblem("the annotation cannot be applied to a property of Relation type `RelationList<string>`")
                .WithAnnotations("[Column]")
                .EndMessage();
        }

        [TestMethod] public void ReferencePrimaryKeysAreNonSequential() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(MassExtinction);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField("Index").AtColumn(4).And
                .HaveField("ExitBoundary.Name").AtColumn(2).And
                .HaveField("ExitBoundary.MYA").AtColumn(3).And
                .HaveField("EntryBoundary.Name").AtColumn(0).And
                .HaveField("EntryBoundary.MYA").AtColumn(1).And
                .HaveField("Severity").AtColumn(5).And
                .HaveNoOtherFields();
        }

        [TestMethod] public void RelationAnchorPrimaryKeysAreNonSequential() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
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
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Pizza);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<CannotAssignColumnsException>()
                .WithLocation("`Pizza`")
                .WithProblem("two Fields pinned to column index 7 (\"Meat1\" and \"Veggie2\")")
                .EndMessage();
        }

        [TestMethod] public void TwoScalarFieldsOrderedToSameIndexInAggregate_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(BiblicalPlague);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<CannotAssignColumnsException>()
                .WithLocation("`BiblicalPlague` → `Translation` (from \"Terminology\")")
                .WithProblem("two Fields pinned to column index 1 (\"Greek\" and \"Hebrew\")")
                .EndMessage();
        }

        [TestMethod] public void TwoNestedFieldsOrderedToSameIndex_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Coup);

            // Act
            var translate = () => translator[source];

            // Assert
            // Assert
            translate.Should().FailWith<CannotAssignColumnsException>()
                .WithLocation("`Coup`")
                .WithProblem("two Fields pinned to column index 3 (\"Overthrowee.MiddleName\" and \"Overthrower.FirstName\")")
                .EndMessage();
        }

        [TestMethod] public void ScalarAndNestedFieldOrderedToSameIndex_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Bread);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<CannotAssignColumnsException>()
                .WithLocation("`Bread`")
                .WithProblem("two Fields pinned to column index 4 (\"Leavening\" and \"Recipe.Sugar\"")
                .EndMessage();
        }

        [TestMethod] public void ColumnOrderingOfScalarsLeavesGaps_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(PhoneNumber);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<CannotAssignColumnsException>()
                .WithLocation("`PhoneNumber`")
                .WithProblem("unable to assign Fields to columns without introducing gaps")
                .EndMessage();
        }

        [TestMethod] public void ColumnOrderingOfAggregatesLeavesGaps_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Verb);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<CannotAssignColumnsException>()
                .WithLocation("`Verb`")
                .WithProblem("unable to assign Fields to columns without introducing gaps")
                .EndMessage();
        }

        [TestMethod] public void ColumnOrderingOfReferencesLeavesGaps_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Origami);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<CannotAssignColumnsException>()
                .WithLocation("`Origami`")
                .WithProblem("unable to assign Fields to columns without introducing gaps")
                .EndMessage();
        }

        [TestMethod] public void NegativeColumnIndex_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(NationalPark);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidColumnIndexException>()
                .WithLocation("`NationalPark` → Established")
                .WithProblem("the column index -196 is negative")
                .WithAnnotations("[Column]")
                .EndMessage();
        }
    }
}
