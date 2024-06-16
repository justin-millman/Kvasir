using FluentAssertions;
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
            translation.Should().FailWith<NotEnoughFieldsException>()
                .WithLocation("`Nothing`")
                .WithProblem("expected at least 2 Fields, but found 0")
                .EndMessage();
        }

        [TestMethod] public void EntityHasOneField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Integer);

            // Act
            var translation = () => translator[source];

            // Assert
            translation.Should().FailWith<NotEnoughFieldsException>()
                .WithLocation("`Integer`")
                .WithProblem("expected at least 2 Fields, but found 1")
                .EndMessage();
        }

        [TestMethod] public void AggregateHasZeroFields_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(TotemPole);

            // Act
            var translation = () => translator[source];

            // Assert
            translation.Should().FailWith<NotEnoughFieldsException>()
                .WithLocation("`TotemPole` → `Festival` (from \"Dedication\")")
                .WithProblem("expected at least 1 Field, but found 0")
                .EndMessage();
        }

        [TestMethod] public void NonPublicPropertiesAreExcluded() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Animal);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField("Genus").OfTypeText().BeingNonNullable().And
                .HaveField("Species").OfTypeText().BeingNonNullable().And
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
                .HaveField("Letter").OfTypeCharacter().BeingNonNullable().And
                .HaveField("Value").OfTypeUInt8().BeingNonNullable().And
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
                .HaveField("Symbol").OfTypeText().BeingNonNullable().And
                .HaveField("NumAllotropes").OfTypeInt8().BeingNonNullable().And
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
                .HaveField("CenterX").OfTypeInt32().BeingNonNullable().And
                .HaveField("CenterY").OfTypeInt32().BeingNonNullable().And
                .HaveField("Radius").OfTypeUInt64().BeingNonNullable().And
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
                .HaveField("GameID").OfTypeGuid().BeingNonNullable().And
                .HaveField("Team").OfTypeText().BeingNonNullable().And
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
                .HaveField("QuadraticCoefficient").OfTypeInt64().BeingNonNullable().And
                .HaveField("LinearCoefficient").OfTypeInt64().BeingNonNullable().And
                .HaveField("Constant").OfTypeInt64().BeingNonNullable().And
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
                .HaveField("Name").OfTypeText().BeingNonNullable().And
                .HaveField("RomanEquivalent").OfTypeText().BeingNullable().And
                .HaveField("NumChildren").OfTypeUInt32().BeingNonNullable().And
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
                .HaveField("ID").OfTypeGuid().BeingNonNullable().And
                .HaveField("Release").OfTypeDateTime().BeingNonNullable().And
                .HaveField("Runtime").OfTypeUInt8().BeingNonNullable().And
                .HaveField("Director").OfTypeText().BeingNonNullable().And
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
                .HaveField("Date").OfTypeDateTime().BeingNonNullable().And
                .HaveField("Name").OfTypeText().BeingNonNullable().And
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
                .HaveField("POBoxNumber").OfTypeUInt32().BeingNonNullable().And
                .HaveField("KnownAs").OfTypeText().BeingNullable().And
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
                .HaveField("Type").OfTypeText().BeingNonNullable().And
                .HaveField("Nickname").OfTypeText().BeingNonNullable().And
                .HaveField("FirstFlight").OfTypeDateTime().BeingNonNullable().And
                .HaveField("Capacity").OfTypeUInt8().BeingNonNullable().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void IncludeInModel_PublicIndexer_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Language);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPropertyInDataModelException>()
                .WithLocation("`Language` → this[]")
                .WithProblem("an indexer cannot be included in the data model")
                .EndMessage();
        }

        [TestMethod] public void IncludeInModel_PublicWriteOnlyProperty_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(HebrewPrayer);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPropertyInDataModelException>()
                .WithLocation("`HebrewPrayer` → OnShabbat")
                .WithProblem("a write-only property cannot be included in the data model")
                .EndMessage();
        }

        [TestMethod] public void IncludeInModel_PublicStaticProperty() {
            // Arrange
            var translator = new Translator();
            var source = typeof(ChessPiece);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField("Name").OfTypeText().BeingNonNullable().And
                .HaveField("Icon").OfTypeCharacter().BeingNonNullable().And
                .HaveField("Value").OfTypeUInt8().BeingNonNullable().And
                .HaveField("FIDE").OfTypeText().BeingNonNullable().And
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
                .HaveField("Title").OfTypeText().BeingNonNullable().And
                .HaveField("Artist").OfTypeText().BeingNonNullable().And
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
                .HaveField("Exonym").OfTypeText().BeingNonNullable().And
                .HaveField("Endonym").OfTypeText().BeingNonNullable().And
                .HaveField("IndependenceDay").OfTypeDateTime().BeingNonNullable().And
                .HaveField("Population").OfTypeUInt64().BeingNonNullable().And
                .HaveField("LandArea").OfTypeUInt64().BeingNonNullable().And
                .HaveField("Coastline").OfTypeUInt64().BeingNonNullable().And
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
                .HaveField("ISBN").OfTypeUInt64().BeingNonNullable().And
                .HaveField("Title").OfTypeText().BeingNonNullable().And
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
                .HaveField("Name").OfTypeText().BeingNonNullable().And
                .HaveField("UseDrumsticks").OfTypeBoolean().BeingNonNullable().And
                .HaveField("LowestKey").OfTypeText().BeingNullable().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void IncludeInModel_ExplicitInterfaceImplementationScalar() {
            // Arrange
            var translator = new Translator();
            var source = typeof(BasicDiceRoll);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField("RollID").OfTypeGuid().BeingNonNullable().And
                .HaveField("NumDice").OfTypeInt32().BeingNonNullable().And
                .HaveField("DiceSides").OfTypeInt32().BeingNonNullable().And
                .HaveField("Plus").OfTypeInt32().BeingNonNullable().And
                .HaveField("Advantage").OfTypeBoolean().BeingNonNullable().And
                .HaveField("Disadvantage").OfTypeBoolean().BeingNonNullable().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void IncludeInModel_ExplicitInterfaceImplementationAggregate() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Wrestler);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField("InternationalAthleteIdentifier").OfTypeGuid().BeingNonNullable().And
                .HaveField("BirthName").OfTypeText().BeingNonNullable().And
                .HaveField("RingName").OfTypeText().BeingNonNullable().And
                .HaveField("WWETitles").OfTypeInt32().BeingNonNullable().And
                .HaveField("Bio.Height").OfTypeDouble().BeingNonNullable().And
                .HaveField("Bio.Weight").OfTypeDouble().BeingNonNullable().And
                .HaveField("Bio.DOB").OfTypeDateTime().BeingNonNullable().And
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
                .HaveField("Value").OfTypeUInt64().BeingNonNullable().And
                .HaveField("Version").OfTypeUInt64().BeingNonNullable().And
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
                .HaveField("Identifier").OfTypeGuid().BeingNonNullable().And
                .HaveField("Class").OfTypeText().BeingNonNullable().And
                .HaveField("Commissioned").OfTypeDateTime().BeingNonNullable().And
                .HaveField("IsActive").OfTypeBoolean().BeingNonNullable().And
                .HaveField("CrewMembers").OfTypeUInt16().BeingNonNullable().And
                .HaveField("Weight").OfTypeUInt64().BeingNonNullable().And
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
                .HaveField("Volume").OfTypeUInt16().BeingNonNullable().And
                .HaveField("CasePage").OfTypeUInt32().BeingNonNullable().And
                .HaveField("Plaintiff").OfTypeText().BeingNonNullable().And
                .HaveField("Defendant").OfTypeText().BeingNonNullable().And
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
                .HaveField("Latitude").OfTypeDecimal().BeingNonNullable().And
                .HaveField("Longitude").OfTypeDecimal().BeingNonNullable().And
                .HaveField("SurfaceArea").OfTypeUInt64().BeingNonNullable().And
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
                .HaveField("Exonym").OfTypeText().BeingNonNullable().And
                .HaveField("Latitude").OfTypeDecimal().BeingNonNullable().And
                .HaveField("Longitude").OfTypeDecimal().BeingNonNullable().And
                .HaveField("SevenSummits").OfTypeBoolean().BeingNonNullable().And
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
                .HaveField("ID").OfTypeUInt32().BeingNonNullable().And
                .HaveField("LocationCode").OfTypeText().BeingNonNullable().And
                .HaveField("SubjectCode").OfTypeText().BeingNonNullable().And
                .HaveField("TimeCode").OfTypeText().BeingNonNullable().And
                .HaveField("Body").OfTypeText().BeingNonNullable().And
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
                .HaveField("System").OfTypeText().BeingNonNullable().And
                .HaveField("Campus").OfTypeText().BeingNonNullable().And
                .HaveField("UndergradEnrollment").OfTypeUInt64().BeingNonNullable().And
                .HaveField("GraduateEnrollment").OfTypeUInt64().BeingNonNullable().And
                .HaveField("Endowment").OfTypeUInt64().BeingNonNullable().And
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
                .HaveField("Title").OfTypeText().BeingNonNullable().And
                .HaveField("Author").OfTypeText().BeingNonNullable().And
                .HaveField("Line1").OfTypeText().BeingNonNullable().And
                .HaveField("Line2").OfTypeText().BeingNonNullable().And
                .HaveField("Line3").OfTypeText().BeingNonNullable().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void CombinedAnnotation_CodeOnlyAndIncludeInModel_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(CreditCard);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<ConflictingAnnotationsException>()
                .WithLocation("`CreditCard` → CVV")
                .WithProblem("the two annotations are mutually exclusive")
                .WithAnnotations("[IncludeInModel]", "[CodeOnly]")
                .EndMessage();
        }
    }
}
