using FluentAssertions;
using Kvasir.Exceptions;
using Kvasir.Schema;
using Kvasir.Translation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using static UT.Kvasir.Translation.FieldClusivity;

namespace UT.Kvasir.Translation {
    [TestClass, TestCategory("Field Clusivity")]
    public class FieldClusivityTests {
        [TestMethod] public void EntityHasZeroFields_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Nothing);

            // Act
            var translation = () => translator[source];

            // Assert
            translation.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining("at least 2 Fields")                         // details / explanation
                .WithMessageContaining("0 found");                                  // details / explanation
        }

        [TestMethod] public void EntityHasOneField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Integer);

            // Act
            var translation = () => translator[source];

            // Assert
            translation.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining("at least 2 Fields required")                // details / explanation
                .WithMessageContaining("1 found");                                  // details / explanation
        }

        [TestMethod] public void AggregateHasZeroFields_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(TotemPole);

            // Act
            var translation = () => translator[source];

            // Assert
            translation.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(nameof(TotemPole.Festival))                  // source type
                .WithMessageContaining("at least 1 Field")                          // details / explanation
                .WithMessageContaining("0 found");                                  // details / explanation
        }

        [TestMethod] public void NonPublicPropertiesAreExcluded() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Animal);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField(nameof(Animal.Genus)).OfTypeText().BeingNonNullable().And
                .HaveField(nameof(Animal.Species)).OfTypeText().BeingNonNullable().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void PublicWriteOnlyPropertiesAreExcluded() {
            // Arrange
            var translator = new Translator();
            var source = typeof(ScrabbleTile);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField(nameof(ScrabbleTile.Letter)).OfTypeCharacter().BeingNonNullable().And
                .HaveField(nameof(ScrabbleTile.Value)).OfTypeUInt8().BeingNonNullable().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void PublicPropertiesWithNonPublicAccessorsAreExcluded() {
            // Arrange
            var translator = new Translator();
            var source = typeof(ChemicalElement);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField(nameof(ChemicalElement.Symbol)).OfTypeText().BeingNonNullable().And
                .HaveField(nameof(ChemicalElement.NumAllotropes)).OfTypeInt8().BeingNonNullable().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void PublicStaticPropertiesAreExcluded() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Circle);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField(nameof(Circle.CenterX)).OfTypeInt32().BeingNonNullable().And
                .HaveField(nameof(Circle.CenterY)).OfTypeInt32().BeingNonNullable().And
                .HaveField(nameof(Circle.Radius)).OfTypeUInt64().BeingNonNullable().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void PublicIndexerPropertiesAreExcluded() {
            // Arrange
            var translator = new Translator();
            var source = typeof(BattingOrder);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField(nameof(BattingOrder.GameID)).OfTypeGuid().BeingNonNullable().And
                .HaveField(nameof(BattingOrder.Team)).OfTypeText().BeingNonNullable().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void CodeOnly_PublicReadableInstanceProperty() {
            // Arrange
            var translator = new Translator();
            var source = typeof(QuadraticEquation);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField(nameof(QuadraticEquation.QuadraticCoefficient)).OfTypeInt64().BeingNonNullable().And
                .HaveField(nameof(QuadraticEquation.LinearCoefficient)).OfTypeInt64().BeingNonNullable().And
                .HaveField(nameof(QuadraticEquation.Constant)).OfTypeInt64().BeingNonNullable().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void CodeOnly_RelationProperty() {
            // Arrange
            var translator = new Translator();
            var source = typeof(LazarusPit);

            // Act
            var translation = translator[source];

            // Assert
            translation.Relations.Count.Should().Be(0);
        }

        [TestMethod] public void FirstDefinedVirtualPropertyIsIncluded() {
            // Arrange
            var translator = new Translator();
            var source = typeof(GreekGod);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField(nameof(GreekGod.Name)).OfTypeText().BeingNonNullable().And
                .HaveField(nameof(GreekGod.RomanEquivalent)).OfTypeText().BeingNullable().And
                .HaveField(nameof(GreekGod.NumChildren)).OfTypeUInt32().BeingNonNullable().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void InterfacePropertiesAreExcluded() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Movie);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField(nameof(Movie.ID)).OfTypeGuid().BeingNonNullable().And
                .HaveField(nameof(Movie.Release)).OfTypeDateTime().BeingNonNullable().And
                .HaveField(nameof(Movie.Runtime)).OfTypeUInt8().BeingNonNullable().And
                .HaveField(nameof(Movie.Director)).OfTypeText().BeingNonNullable().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void InheritedClassPropertiesAreExcluded() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Holiday);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField(nameof(Holiday.Date)).OfTypeDateTime().BeingNonNullable().And
                .HaveField(nameof(Holiday.Name)).OfTypeText().BeingNonNullable().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void VirtualPropertyOverrideIsExcluded() {
            // Arrange
            var translator = new Translator();
            var source = typeof(POBox);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField(nameof(POBox.POBoxNumber)).OfTypeUInt32().BeingNonNullable().And
                .HaveField(nameof(POBox.KnownAs)).OfTypeText().BeingNullable().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void HidingPropertiesAreIncluded() {
            // Arrange
            var translator = new Translator();
            var source = typeof(FighterJet);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField(nameof(FighterJet.Type)).OfTypeText().BeingNonNullable().And
                .HaveField(nameof(FighterJet.Nickname)).OfTypeText().BeingNonNullable().And
                .HaveField(nameof(FighterJet.FirstFlight)).OfTypeDateTime().BeingNonNullable().And
                .HaveField(nameof(FighterJet.Capacity)).OfTypeUInt8().BeingNonNullable().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void IncludeInModel_PublicIndexer_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Language);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining("indexer")                                   // error location
                .WithMessageContaining("[IncludeInModel]");                         // category
        }

        [TestMethod] public void IncludeInModel_PublicWriteOnlyProperty_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(HebrewPrayer);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(HebrewPrayer.OnShabbat))              // error location
                .WithMessageContaining("[IncludeInModel]")                          // category
                .WithMessageContaining("write-only");                               // details / explanation
        }

        [TestMethod] public void IncludeInModel_PublicStaticProperty() {
            // Arrange
            var translator = new Translator();
            var source = typeof(ChessPiece);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField(nameof(ChessPiece.Name)).OfTypeText().BeingNonNullable().And
                .HaveField(nameof(ChessPiece.Icon)).OfTypeCharacter().BeingNonNullable().And
                .HaveField(nameof(ChessPiece.Value)).OfTypeUInt8().BeingNonNullable().And
                .HaveField(nameof(ChessPiece.FIDE)).OfTypeText().BeingNonNullable().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void IncludeInModel_NonPublicProperties() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Song);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField(nameof(Song.Title)).OfTypeText().BeingNonNullable().And
                .HaveField(nameof(Song.Artist)).OfTypeText().BeingNonNullable().And
                .HaveField("Album").OfTypeText().BeingNullable().And
                .HaveField("Length").OfTypeUInt16().BeingNonNullable().And
                .HaveField("ReleaseYear").OfTypeUInt16().BeingNonNullable().And
                .HaveField("Rating").OfTypeDouble().BeingNonNullable().And
                .HaveField("Grammys").OfTypeUInt8().BeingNonNullable().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void IncludeInModel_PublicPropertiesWithNonPublicAccessors() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Country);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField(nameof(Country.Exonym)).OfTypeText().BeingNonNullable().And
                .HaveField(nameof(Country.Endonym)).OfTypeText().BeingNonNullable().And
                .HaveField(nameof(Country.IndependenceDay)).OfTypeDateTime().BeingNonNullable().And
                .HaveField(nameof(Country.Population)).OfTypeUInt64().BeingNonNullable().And
                .HaveField(nameof(Country.LandArea)).OfTypeUInt64().BeingNonNullable().And
                .HaveField(nameof(Country.Coastline)).OfTypeUInt64().BeingNonNullable().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void IncludeInModel_InterfaceProperty() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Book);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField(nameof(Book.ISBN)).OfTypeUInt64().BeingNonNullable().And
                .HaveField(nameof(Book.Title)).OfTypeText().BeingNonNullable().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void IncludeInModel_VirtualPropertyOverride() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Drum);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField(nameof(Drum.Name)).OfTypeText().BeingNonNullable().And
                .HaveField(nameof(Drum.UseDrumsticks)).OfTypeBoolean().BeingNonNullable().And
                .HaveField(nameof(Drum.LowestKey)).OfTypeText().BeingNullable().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void CodeOnly_InterfaceProperty_Redundant() {
            // Arrange
            var translator = new Translator();
            var source = typeof(IPAddress);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField(nameof(IPAddress.Value)).OfTypeUInt64().BeingNonNullable().And
                .HaveField(nameof(IPAddress.Version)).OfTypeUInt64().BeingNonNullable().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void CodeOnly_VirtualPropertyOverride_Redundant() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Submarine);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField(nameof(Submarine.Identifier)).OfTypeGuid().BeingNonNullable().And
                .HaveField(nameof(Submarine.Class)).OfTypeText().BeingNonNullable().And
                .HaveField(nameof(Submarine.Commissioned)).OfTypeDateTime().BeingNonNullable().And
                .HaveField(nameof(Submarine.IsActive)).OfTypeBoolean().BeingNonNullable().And
                .HaveField(nameof(Submarine.CrewMembers)).OfTypeUInt16().BeingNonNullable().And
                .HaveField(nameof(Submarine.Weight)).OfTypeUInt64().BeingNonNullable().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void CodeOnly_PublicWriteOnlyProperty_Redundant() {
            // Arrange
            var translator = new Translator();
            var source = typeof(CourtCase);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField(nameof(CourtCase.Volume)).OfTypeUInt16().BeingNonNullable().And
                .HaveField(nameof(CourtCase.CasePage)).OfTypeUInt32().BeingNonNullable().And
                .HaveField(nameof(CourtCase.Plaintiff)).OfTypeText().BeingNonNullable().And
                .HaveField(nameof(CourtCase.Defendant)).OfTypeText().BeingNonNullable().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void CodeOnly_NonPublicProperties_Redundant() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Lake);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField(nameof(Lake.Latitude)).OfTypeDecimal().BeingNonNullable().And
                .HaveField(nameof(Lake.Longitude)).OfTypeDecimal().BeingNonNullable().And
                .HaveField(nameof(Lake.SurfaceArea)).OfTypeUInt64().BeingNonNullable().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void CodeOnly_PublicPropertiesWithNonPublicAccessors_Redundant() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Mountain);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField(nameof(Mountain.Exoynym)).OfTypeText().BeingNonNullable().And
                .HaveField(nameof(Mountain.Latitude)).OfTypeDecimal().BeingNonNullable().And
                .HaveField(nameof(Mountain.Longitude)).OfTypeDecimal().BeingNonNullable().And
                .HaveField(nameof(Mountain.SevenSummits)).OfTypeBoolean().BeingNonNullable().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void CodeOnly_PublicStaticProperty_Redundant() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Tossup);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField(nameof(Tossup.ID)).OfTypeUInt32().BeingNonNullable().And
                .HaveField(nameof(Tossup.LocationCode)).OfTypeText().BeingNonNullable().And
                .HaveField(nameof(Tossup.SubjectCode)).OfTypeText().BeingNonNullable().And
                .HaveField(nameof(Tossup.TimeCode)).OfTypeText().BeingNonNullable().And
                .HaveField(nameof(Tossup.Body)).OfTypeText().BeingNonNullable().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void CodeOnly_PublicIndexer_Redundant() {
            // Arrange
            var translator = new Translator();
            var source = typeof(University);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField(nameof(University.System)).OfTypeText().BeingNonNullable().And
                .HaveField(nameof(University.Campus)).OfTypeText().BeingNonNullable().And
                .HaveField(nameof(University.UndergradEnrollment)).OfTypeUInt64().BeingNonNullable().And
                .HaveField(nameof(University.GraduateEnrollment)).OfTypeUInt64().BeingNonNullable().And
                .HaveField(nameof(University.Endowment)).OfTypeUInt64().BeingNonNullable().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void IncludeInModel_PublicReadableInstanceProperty_Redundant() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Haiku);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField(nameof(Haiku.Title)).OfTypeText().BeingNonNullable().And
                .HaveField(nameof(Haiku.Author)).OfTypeText().BeingNonNullable().And
                .HaveField(nameof(Haiku.Line1)).OfTypeText().BeingNonNullable().And
                .HaveField(nameof(Haiku.Line2)).OfTypeText().BeingNonNullable().And
                .HaveField(nameof(Haiku.Line3)).OfTypeText().BeingNonNullable().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void CombinedAnnotation_CodeOnlyAndIncludeInModel_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(CreditCard);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(CreditCard.CVV))                      // error location
                .WithMessageContaining("mutually exclusive")                        // category
                .WithMessageContaining("[IncludeInModel]")                          // details / explanation
                .WithMessageContaining("[CodeOnly]");                               // details / explanation
        }
    }
}
