using FluentAssertions;
using Kvasir.Exceptions;
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
                .HaveField(nameof(Timestamp.UnixSinceEpoch)).OfTypeUInt64().BeingNonNullable().And
                .HaveField(nameof(Timestamp.Hour)).OfTypeUInt16().BeingNonNullable().And
                .HaveField(nameof(Timestamp.Minute)).OfTypeUInt16().BeingNonNullable().And
                .HaveField(nameof(Timestamp.Second)).OfTypeUInt16().BeingNonNullable().And
                .HaveField(nameof(Timestamp.Millisecond)).OfTypeUInt16().BeingNullable().And
                .HaveField(nameof(Timestamp.Microsecond)).OfTypeUInt16().BeingNullable().And
                .HaveField(nameof(Timestamp.Nanosecond)).OfTypeUInt16().BeingNullable().And
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
                .HaveField(nameof(Bankruptcy.Filing)).OfTypeGuid().BeingNonNullable().And
                .HaveField("Company.Name").OfTypeText().BeingNullable().And
                .HaveField("Company.Founded").OfTypeDateTime().BeingNullable().And
                .HaveField("Company.TickerSymbol").OfTypeText().BeingNullable().And
                .HaveField(nameof(Bankruptcy.Chapter)).OfTypeUInt8().BeingNonNullable().And
                .HaveField(nameof(Bankruptcy.TotalDebt)).OfTypeDecimal().BeingNonNullable().And
                .HaveField(nameof(Bankruptcy.NumCreditors)).OfTypeUInt64().BeingNonNullable().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void NonNullableReferenceMarkedNullable() {
            // Arrange
            var source = typeof(Jukebox);
            var translator = new Translator();

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField(nameof(Jukebox.ProductID)).OfTypeGuid().BeingNonNullable().And
                .HaveField(nameof(Jukebox.NumSongs)).OfTypeUInt16().BeingNonNullable().And
                .HaveField("MostPlayed.Title").OfTypeText().BeingNullable().And
                .HaveField("MostPlayed.Singer").OfTypeText().BeingNullable().And
                .HaveField(nameof(Jukebox.CostPerPlay)).OfTypeDecimal().BeingNonNullable().And
                .HaveField(nameof(Jukebox.IsDigital)).OfTypeBoolean().BeingNonNullable().And
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
                .HaveField(nameof(Bone.TA2)).OfTypeUInt32().BeingNonNullable().And
                .HaveField(nameof(Bone.Name)).OfTypeText().BeingNonNullable().And
                .HaveField(nameof(Bone.LatinName)).OfTypeText().BeingNullable().And
                .HaveField(nameof(Bone.MeSH)).OfTypeText().BeingNonNullable().And
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
                .HaveField(nameof(Orchestra.ID)).OfTypeGuid().BeingNonNullable().And
                .HaveField(nameof(Orchestra.Name)).OfTypeText().BeingNonNullable().And
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
            var source = typeof(Bodhisattva);
            var translator = new Translator();

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField(nameof(Bodhisattva.Name)).OfTypeText().BeingNonNullable().And
                .HaveField(nameof(Bodhisattva.Buddhism)).OfTypeEnumeration(
                    Bodhisattva.Denomination.Nikaya,
                    Bodhisattva.Denomination.Theravada,
                    Bodhisattva.Denomination.Mahayana
                ).BeingNonNullable().And
                .HaveField("LastBhumi.English").OfTypeText().BeingNonNullable().And
                .HaveField(nameof(Bodhisattva.DateOfBirth)).OfTypeDateTime().BeingNonNullable().And
                .HaveField(nameof(Bodhisattva.DateOfDeath)).OfTypeDateTime().BeingNonNullable().And
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
                .HaveField(nameof(CivMilitaryUnit.Identifier)).OfTypeText().BeingNonNullable().And
                .HaveField(nameof(CivMilitaryUnit.Promotion)).OfTypeText().BeingNullable().And
                .HaveField(nameof(CivMilitaryUnit.MeleeStrength)).OfTypeUInt8().BeingNonNullable().And
                .HaveField(nameof(CivMilitaryUnit.RangedStrength)).OfTypeUInt8().BeingNullable().And
                .HaveField(nameof(CivMilitaryUnit.IsUnique)).OfTypeBoolean().BeingNonNullable().And
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
                .HaveField(nameof(Patent.DocumentID)).OfTypeUInt64().BeingNonNullable().And
                .HaveField(nameof(Patent.PublicationDate)).OfTypeDateTime().BeingNonNullable().And
                .HaveField(nameof(Patent.Description)).OfTypeText().BeingNullable().And
                .HaveField(nameof(Patent.ApplicationNumber)).OfTypeUInt64().BeingNonNullable().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void CombinedAnnotation_NullableAndNonNullable_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(RetailProduct);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(RetailProduct.SalePrice))             // error location
                .WithMessageContaining("mutually exclusive")                        // category
                .WithMessageContaining("[Nullable]")                                // details / explanation
                .WithMessageContaining("[NonNullable]");                            // details / explanation
        }

        [TestMethod] public void NativelyNullableAggregateContainsOnlyNullableFields_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Waffle);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Waffle.Toppings))                     // error location
                .WithMessageContaining("nullability of Aggregate is ambiguous");    // category
        }

        [TestMethod] public void AnnotatedNullableAggregateContainsOnlyNullableFields_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(iPhone);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(iPhone.iOSVersion))                   // error location
                .WithMessageContaining("nullability of Aggregate is ambiguous");    // category
        }
    }
}
