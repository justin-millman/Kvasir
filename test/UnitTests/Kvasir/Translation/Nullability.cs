using FluentAssertions;
using Kvasir.Schema;
using Kvasir.Translation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using static UT.Kvasir.Translation.Nullability;

namespace UT.Kvasir.Translation {
    [TestClass, TestCategory("Nullability")]
    public class NullabilityTests {
        [TestMethod] public void NonNullableScalarsMarkedNullable() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Timestamp);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField("UnixSinceEpoch").OfTypeUInt64().BeingNonNullable().And
                .HaveField("Hour").OfTypeUInt16().BeingNonNullable().And
                .HaveField("Minute").OfTypeUInt16().BeingNonNullable().And
                .HaveField("Second").OfTypeUInt16().BeingNonNullable().And
                .HaveField("Millisecond").OfTypeUInt16().BeingNullable().And
                .HaveField("Microsecond").OfTypeUInt16().BeingNullable().And
                .HaveField("Nanosecond").OfTypeUInt16().BeingNullable().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void NonNullableAggregateMarkedNullable() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Bankruptcy);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField("Filing").OfTypeGuid().BeingNonNullable().And
                .HaveField("Company.Name").OfTypeText().BeingNullable().And
                .HaveField("Company.Founded").OfTypeDateTime().BeingNullable().And
                .HaveField("Company.TickerSymbol").OfTypeText().BeingNullable().And
                .HaveField("Chapter").OfTypeUInt8().BeingNonNullable().And
                .HaveField("TotalDebt").OfTypeDecimal().BeingNonNullable().And
                .HaveField("NumCreditors").OfTypeUInt64().BeingNonNullable().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void NonNullableReferenceMarkedNullable() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Jukebox);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField("ProductID").OfTypeGuid().BeingNonNullable().And
                .HaveField("NumSongs").OfTypeUInt16().BeingNonNullable().And
                .HaveField("MostPlayed.Title").OfTypeText().BeingNullable().And
                .HaveField("MostPlayed.Singer").OfTypeText().BeingNullable().And
                .HaveField("CostPerPlay").OfTypeDecimal().BeingNonNullable().And
                .HaveField("IsDigital").OfTypeBoolean().BeingNonNullable().And
                .HaveNoOtherFields().And
                .HaveForeignKey("MostPlayed.Singer", "MostPlayed.Title")
                    .Against(translator[typeof(Jukebox.Song)].Principal.Table)
                    .WithOnDeleteBehavior(OnDelete.Cascade)
                    .WithOnUpdateBehavior(OnUpdate.Cascade).And
                .HaveNoOtherForeignKeys();
        }

        [TestMethod] public void NullableScalarsMarkedNonNullable() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Bone);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField("TA2").OfTypeUInt32().BeingNonNullable().And
                .HaveField("Name").OfTypeText().BeingNonNullable().And
                .HaveField("LatinName").OfTypeText().BeingNullable().And
                .HaveField("MeSH").OfTypeText().BeingNonNullable().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void NullableAggregateMarkedNonNullable() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Orchestra);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField("ID").OfTypeGuid().BeingNonNullable().And
                .HaveField("Name").OfTypeText().BeingNonNullable().And
                .HaveField("Composition.Strings.Violins").OfTypeUInt32().BeingNullable().And
                .HaveField("Composition.Strings.Violas").OfTypeUInt32().BeingNonNullable().And
                .HaveField("Composition.Strings.Cellos").OfTypeUInt32().BeingNonNullable().And
                .HaveField("Composition.Strings.Basses").OfTypeUInt32().BeingNonNullable().And
                .HaveField("Composition.Woodwinds.Flutes").OfTypeUInt32().BeingNullable().And
                .HaveField("Composition.Woodwinds.Oboes").OfTypeUInt32().BeingNullable().And
                .HaveField("Composition.Woodwinds.Clarinets").OfTypeUInt32().BeingNullable().And
                .HaveField("Composition.Woodwinds.Saxophones").OfTypeUInt32().BeingNullable().And
                .HaveField("Composition.Woodwinds.Bassoons").OfTypeUInt32().BeingNullable().And
                .HaveField("Composition.Brass.FrenchHorns").OfTypeUInt32().BeingNullable().And
                .HaveField("Composition.Brass.Trumpets").OfTypeUInt32().BeingNullable().And
                .HaveField("Composition.Brass.Trombones").OfTypeUInt32().BeingNullable().And
                .HaveField("Composition.Brass.Tubas").OfTypeUInt32().BeingNullable().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void NullableReferenceMarkedNonNullable() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Bodhisattva);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField("Name").OfTypeText().BeingNonNullable().And
                .HaveField("Buddhism").OfTypeEnumeration(
                    Bodhisattva.Denomination.Nikaya,
                    Bodhisattva.Denomination.Theravada,
                    Bodhisattva.Denomination.Mahayana
                ).BeingNonNullable().And
                .HaveField("LastBhumi.English").OfTypeText().BeingNonNullable().And
                .HaveField("DateOfBirth").OfTypeDateTime().BeingNonNullable().And
                .HaveField("DateOfDeath").OfTypeDateTime().BeingNonNullable().And
                .HaveNoOtherFields().And
                .HaveForeignKey("LastBhumi.English")
                    .Against(translator[typeof(Bodhisattva.Bhumi)].Principal.Table)
                    .WithOnDeleteBehavior(OnDelete.Cascade)
                    .WithOnUpdateBehavior(OnUpdate.Cascade).And
                .HaveNoOtherForeignKeys();
        }

        [TestMethod] public void NullableScalarsMarkedAsNullable_Redundant() {
            // Arrange
            var translator = new Translator();
            var source = typeof(CivMilitaryUnit);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField("Identifier").OfTypeText().BeingNonNullable().And
                .HaveField("Promotion").OfTypeText().BeingNullable().And
                .HaveField("MeleeStrength").OfTypeUInt8().BeingNonNullable().And
                .HaveField("RangedStrength").OfTypeUInt8().BeingNullable().And
                .HaveField("IsUnique").OfTypeBoolean().BeingNonNullable().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void NonNullableScalarsMarkedAsNonNullable_Redundant() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Patent);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField("DocumentID").OfTypeUInt64().BeingNonNullable().And
                .HaveField("PublicationDate").OfTypeDateTime().BeingNonNullable().And
                .HaveField("Description").OfTypeText().BeingNullable().And
                .HaveField("ApplicationNumber").OfTypeUInt64().BeingNonNullable().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void CombinedAnnotation_NullableAndNonNullable_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(RetailProduct);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<ConflictingAnnotationsException>()
                .WithLocation("`RetailProduct` → SalePrice")
                .WithProblem("the two annotations are mutually exclusive")
                .WithAnnotations("[Nullable]", "[NonNullable]")
                .EndMessage();
        }

        [TestMethod] public void NativelyNullableAggregateContainsOnlyNullableFields_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Waffle);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<AmbiguousNullabilityException>()
                .WithLocation("`Waffle` → Toppings")
                .WithProblem("the property cannot be nullable because all nested Fields are already nullable")
                .EndMessage();
        }

        [TestMethod] public void AnnotatedNullableAggregateContainsOnlyNullableFields_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(iPhone);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<AmbiguousNullabilityException>()
                .WithLocation("`iPhone` → iOSVersion")
                .WithProblem("the property cannot be nullable because all nested Fields are already nullable")
                .WithAnnotations("[Nullable]")
                .EndMessage();
        }

        [TestMethod] public void RelationWithNullableElements() {
            // Arrange
            var translator = new Translator();
            var source = typeof(PostOffice);

            // Act
            var translation = translator[source];

            // Assert
            translation.Relations[0].Table.Should()
                .HaveField("PostOffice.ID").OfTypeGuid().BeingNonNullable().And
                .HaveField("Index").OfTypeUInt32().BeingNonNullable().And
                .HaveField("Item").OfTypeDecimal().BeingNullable().And
                .HaveNoOtherFields().And
                .HaveForeignKey("PostOffice.ID")
                    .Against(translation.Principal.Table)
                    .WithOnDeleteBehavior(OnDelete.Cascade)
                    .WithOnUpdateBehavior(OnUpdate.Cascade).And
                .HaveNoOtherForeignKeys();
            translation.Relations[1].Table.Should()
                .HaveField("PostOffice.ID").OfTypeGuid().BeingNonNullable().And
                .HaveField("Item").OfTypeText().BeingNullable().And
                .HaveNoOtherFields().And
                .HaveForeignKey("PostOffice.ID")
                    .Against(translation.Principal.Table)
                    .WithOnDeleteBehavior(OnDelete.Cascade)
                    .WithOnUpdateBehavior(OnUpdate.Cascade).And
                .HaveNoOtherForeignKeys();
            translation.Relations[2].Table.Should()
                .HaveField("PostOffice.ID").OfTypeGuid().BeingNonNullable().And
                .HaveField("Item.Number").OfTypeText().BeingNullable().And
                .HaveField("Item.State").OfTypeText().BeingNullable().And
                .HaveNoOtherFields().And
                .HaveForeignKey("PostOffice.ID")
                    .Against(translation.Principal.Table)
                    .WithOnDeleteBehavior(OnDelete.Cascade)
                    .WithOnUpdateBehavior(OnUpdate.Cascade).And
                .HaveNoOtherForeignKeys();
            translation.Relations[3].Table.Should()
                .HaveField("PostOffice.ID").OfTypeGuid().BeingNonNullable().And
                .HaveField("Key").OfTypeInt16().BeingNonNullable().And
                .HaveField("Value").OfTypeText().BeingNullable().And
                .HaveNoOtherFields().And
                .HaveForeignKey("PostOffice.ID")
                    .Against(translation.Principal.Table)
                    .WithOnDeleteBehavior(OnDelete.Cascade)
                    .WithOnUpdateBehavior(OnUpdate.Cascade).And
                .HaveNoOtherForeignKeys();
            translation.Relations[4].Table.Should()
                .HaveField("PostOffice.ID").OfTypeGuid().BeingNonNullable().And
                .HaveField("Key").OfTypeDateTime().BeingNullable().And
                .HaveField("Value.StampID").OfTypeGuid().BeingNullable().And
                .HaveField("Value.Price").OfTypeDecimal().BeingNullable().And
                .HaveNoOtherFields().And
                .HaveForeignKey("PostOffice.ID")
                    .Against(translation.Principal.Table)
                    .WithOnDeleteBehavior(OnDelete.Cascade)
                    .WithOnUpdateBehavior(OnUpdate.Cascade).And
                .HaveNoOtherForeignKeys();
        }

        [TestMethod] public void RelationElementNullableAggregateContainsOnlyNullableFields_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Parabola);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<AmbiguousNullabilityException>()
                .WithLocation("`Parabola` → <synthetic> `Points`")
                .WithProblem("the property cannot be nullable because all nested Fields are already nullable")
                .WithAnnotations("[Nullable]")
                .EndMessage();
        }

        [TestMethod] public void RelationMarkedNonNullable_Redundant() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Squintern);

            // Act
            var translation = translator[source];

            // Assert
            translation.Relations.Should().HaveCount(1);
            translation.Relations[0].Table.Should()
                .HaveField("Squintern.FirstName").OfTypeText().BeingNonNullable().And
                .HaveField("Squintern.LastName").OfTypeText().BeingNonNullable().And
                .HaveField("Item.Season").OfTypeUInt32().BeingNonNullable().And
                .HaveField("Item.Number").OfTypeUInt16().BeingNonNullable().And
                .HaveField("Item.Title").OfTypeText().BeingNonNullable().And
                .HaveNoOtherFields().And
                .HaveForeignKey("Squintern.FirstName", "Squintern.LastName")
                    .Against(translation.Principal.Table)
                    .WithOnDeleteBehavior(OnDelete.Cascade)
                    .WithOnUpdateBehavior(OnUpdate.Cascade).And
                .HaveNoOtherForeignKeys();
        }

        [TestMethod] public void RelationMarkedNullable_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Axiom);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Axiom` → <synthetic> `DerivedTheories`")
                .WithProblem("the annotation cannot be applied to a property of Relation type `RelationSet<string>`")
                .WithAnnotations("[Nullable]");
        }
    }
}
