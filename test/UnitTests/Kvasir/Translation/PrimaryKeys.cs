using FluentAssertions;
using Kvasir.Translation2;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using static UT.Kvasir.Translation.PrimaryKeyIdentification;
using static UT.Kvasir.Translation.PrimaryKeyNaming;

namespace UT.Kvasir.Translation {
    [TestClass, TestCategory("Primary Key Identification")]
    public class PrimaryKeyIdentificationTests {
        [TestMethod] public void SingleScalarMarkedPrimaryKey() {
            // Arrange
            var translator = new Translator();
            var source = typeof(XKCDComic);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HavePrimaryKey().OfFields("URL");
        }

        [TestMethod] public void MultipleScalarsMarkedPrimaryKey() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Month);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HavePrimaryKey().OfFields(
                    "Calendar",
                    "Index"
                );
        }

        [TestMethod] public void AggregateMarkedPrimaryKey() {
            // Arrange
            var translator = new Translator();
            var source = typeof(SpaceShuttle);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HavePrimaryKey().OfFields(
                    "Name",
                    "Specification.SerialNumber",
                    "Specification.Weight",
                    "Specification.ID"
                );
        }

        [TestMethod] public void AggregateNestedScalarMarkedPrimaryKey() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Tepui);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HavePrimaryKey().OfFields(
                    "Location.Latitude",
                    "Location.Longitude"
                );
        }

        [TestMethod] public void NestedAggregateMarkedPrimaryKey() {
            // Arrange
            var translator = new Translator();
            var source = typeof(ChoppedBasket);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HavePrimaryKey().OfFields(
                    "AirDate",
                    "Round",
                    "Ingredient1.Name.English",
                    "Ingredient1.Name.Alternative",
                    "Ingredient2.Name.English",
                    "Ingredient2.Name.Alternative",
                    "Ingredient3.Name.English",
                    "Ingredient3.Name.Alternative",
                    "Ingredient4.Name.English",
                    "Ingredient4.Name.Alternative"
                );
        }

        [TestMethod] public void ReferenceMarkedPrimaryKey() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Etiology);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HavePrimaryKey().OfFields(
                    "Source.Name",
                    "Source.Abbreviation"
                );
        }

        [TestMethod] public void ReferenceNestedScalarMarkedPrimaryKey() {
            // Arrange
            var tranlator = new Translator();
            var source = typeof(PoirotMystery);

            // Act
            var translation = tranlator[source];

            // Assert
            translation.Principal.Table.Should()
                .HavePrimaryKey().OfFields("ISBN.ValuePart1");
        }

        [TestMethod] public void NestedReferenceMarkedPrimaryKey() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Prophecy);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HavePrimaryKey().OfFields(
                    "ProphecyID",
                    "Subjects.P1.FirstName",
                    "Subjects.P1.MiddleInitial",
                    "Subjects.P1.LastName"
                );
        }

        [TestMethod] public void RelationNestedScalarMarkedPrimaryKey() {
            // Arrange
            var translator = new Translator();
            var source = typeof(GrandPrix);

            // Act
            var translation = translator[source];

            // Assert
            translation.Relations[0].Table.Should()
                .HavePrimaryKey().OfFields(
                    "GrandPrix.Year",
                    "GrandPrix.Country",
                    "Key.CarNumber"
                );
        }

        [TestMethod] public void NestedRelationMarkedPrimaryKey_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Psalm);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Psalm` → Text")
                .WithPath("Verses")
                .WithProblem("the annotation cannot be applied to a property of Relation type `RelationMap<ushort, string>`")
                .WithAnnotations("[PrimaryKey]")
                .EndMessage();
        }

        [TestMethod] public void AllScalarsMarkedPrimaryKey() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Character);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HavePrimaryKey().OfFields(
                    "Glyph",
                    "CodePoint",
                    "IsASCII"
                );
        }

        [TestMethod] public void NonNullableScalarFieldNativelyNamedID() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Actor);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HavePrimaryKey().OfFields("ID");
        }

        [TestMethod] public void NonNullableScalarFieldRenamedToID() {
            // Arrange
            var translator = new Translator();
            var source = typeof(PokerHand);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HavePrimaryKey().OfFields("ID");
        }

        [TestMethod] public void NonNullableScalarFieldNativelyNamedEntityID() {
            // Arrange
            var translator = new Translator();
            var source = typeof(IntegerSequence);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HavePrimaryKey().OfFields("IntegerSequenceID");
        }

        [TestMethod] public void NonNullableScalarFieldRenamedToEntityID() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Stadium);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HavePrimaryKey().OfFields("StadiumID");
        }

        [TestMethod] public void NonNullableScalarFieldsNamedTableIDAndEntityID() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Function);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HavePrimaryKey().OfFields("FunctionID");
        }

        [TestMethod] public void SingleCandidateKeyWithSingleNonNullableField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Star);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HavePrimaryKey().OfFields("ARICNS").And
                .HaveNoOtherCandidateKeys();
        }

        [TestMethod] public void SingleCandidateKeyWithMultipleNonNullableFields() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Expiration);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HavePrimaryKey("PK").OfFields(
                    "FeedCode",
                    "Underlying"
                );
        }

        [TestMethod] public void MultipleCandidateKeysOnlyOneAllNonNullable() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Vitamin);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HavePrimaryKey("Formula").OfFields(
                    "Carbon",
                    "Hydrogen",
                    "Cobalt",
                    "Nitrogen",
                    "Oxygen",
                    "Phosphorus"
                ).And
                .HaveCandidateKey("CK1").OfFields(
                    "Name",
                    "Alternative"
                ).And
                .HaveNoOtherCandidateKeys();
        }

        [TestMethod] public void MultipleCandidateKeysOneSubsetOfAllOthers() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Escalator);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HavePrimaryKey("ID").OfFields("EscalatorIdentifier").And
                .HaveNoOtherCandidateKeys();
        }

        [TestMethod] public void SingleCandidateKeyDeducedForOtherReasons() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Repository);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HavePrimaryKey().OfFields("ID").And
                .HaveNoOtherCandidateKeys();
        }

        [TestMethod] public void SingleNativelyNonNullableField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Earthquake);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HavePrimaryKey().OfFields("SeismicIdentificationNumber");
        }

        [TestMethod] public void SingleAnnotatedNonNullableField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(GeologicEpoch);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HavePrimaryKey().OfFields("StartingMYA");
        }

        [TestMethod] public void AllNonNullableFieldsDefaultDeduction() {
            // Arrange
            var translator = new Translator();
            var source = typeof(HotAirBalloon);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HavePrimaryKey().OfFields(
                    "Manufacturer",
                    "MaxHeight",
                    "MaxAirTemperature",
                    "PassengerCapacity",
                    "Radius"
                );
        }

        [TestMethod] public void DefaultPrimaryKeyForListSetRelations() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Brothel);

            // Act
            var translation = translator[source];

            // Assert
            translation.Relations[0].Table.Should()
                .HavePrimaryKey().OfFields(
                    "Brothel.Address",
                    "Item"
                );
            translation.Relations[1].Table.Should()
                .HavePrimaryKey().OfFields(
                    "Brothel.Address",
                    "Item.Description",
                    "Item.CostPerHour"
                );
        }

        [TestMethod] public void DefaultPrimaryKeyForMapRelation() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Cult);

            // Act
            var translation = translator[source];

            // Assert
            translation.Relations[0].Table.Should()
                .HavePrimaryKey().OfFields(
                    "Cult.Title",
                    "Key"
                );
        }

        [TestMethod] public void DefaultPrimaryKeyForOrderedListRelation() {
            // Arrange
            var translator = new Translator();
            var source = typeof(PianoSonata);

            // Act
            var translation = translator[source];

            // Assert
            translation.Relations[0].Table.Should()
                .HavePrimaryKey().OfFields(
                    "PianoSonata.Composer",
                    "PianoSonata.OpusNumber",
                    "Index"
                );
        }

        [TestMethod] public void RelationWithSingleViableCandidateKeyIncludingAnchor() {
            // Arrange
            var translator = new Translator();
            var source = typeof(ChromeExtension);

            // Act
            var translation = translator[source];

            // Assert
            translation.Relations[0].Table.Should()
                .HavePrimaryKey("Key").OfFields(
                    "ChromeExtension.ExtensionID",
                    "Item.Reviewer"
                ).And
                .HaveNoOtherCandidateKeys();
        }

        [TestMethod] public void RelationWithSingleViableCandidateKeyExcludingAnchor() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Zipline);

            // Act
            var translation = translator[source];

            // Assert
            translation.Relations[0].Table.Should()
                .HavePrimaryKey().OfFields(
                    "Item"
                ).And
                .HaveNoOtherCandidateKeys();
        }

        [TestMethod] public void NullableFieldNamedIDSkipped() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Rollercoaster);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HavePrimaryKey().OfFields("RollercoasterID");
        }

        [TestMethod] public void NullableFieldNamedEntityIDSkipped() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Doctor);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HavePrimaryKey("DoctorWho").OfFields(
                    "Regeneration",
                    "Portrayal"
                ).And
                .HaveNoOtherCandidateKeys();
        }

        [TestMethod] public void CandidateKeysWithNullableFieldsSkipped() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Polyhedron);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HavePrimaryKey().OfFields("Name");
        }

        [TestMethod] public void ScalarMarkedPrimaryKeyMultipleTimesDirectly_Redundant() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Airport);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HavePrimaryKey().OfFields("IATA");
        }

        [TestMethod] public void ScalarMarkedPrimaryKeyMultipleTimesIndirectly_Redundant() {
            // Arrange
            var translator = new Translator();
            var source = typeof(CompressionFormat);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HavePrimaryKey().OfFields(
                    "Suffix",
                    "LastStableRelease.Major",
                    "LastStableRelease.Minor",
                    "LastStableRelease.Patch"
                );
        }

        [TestMethod] public void NullableScalarMarkedPrimaryKey_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(NorseWorld);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPrimaryKeyFieldException>()
                .WithLocation("`NorseWorld` → EddaMentions")
                .WithProblem("a nullable Field cannot be part of an Entity's primary key")
                .WithAnnotations("[PrimaryKey]")
                .EndMessage();
        }

        [TestMethod] public void NullableAggregateMarkedPrimaryKey_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(MedievalCastle);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPrimaryKeyFieldException>()
                .WithLocation("`MedievalCastle` → DrawBridge")
                .WithApplicationTo("Length")
                .WithProblem("a nullable Field cannot be part of an Entity's primary key")
                .WithAnnotations("[PrimaryKey]")
                .EndMessage();
        }

        [TestMethod] public void AggregateWithNullablePropertyMarkedPrimaryKey_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Wizard);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPrimaryKeyFieldException>()
                .WithLocation("`Wizard` → Background")
                .WithApplicationTo("Schooling.School")
                .WithProblem("a nullable Field cannot be part of an Entity's primary key")
                .WithAnnotations("[PrimaryKey]")
                .EndMessage();
        }

        [TestMethod] public void NullableReferenceMarkedPrimaryKey_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Avocado);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPrimaryKeyFieldException>()
                .WithLocation("`Avocado` → CountryOfOrigin")
                .WithApplicationTo("Name")
                .WithProblem("a nullable Field cannot be part of an Entity's primary key")
                .WithAnnotations("[PrimaryKey]")
                .EndMessage();
        }

        [TestMethod] public void PropertyInAggregateMarkedPrimaryKey_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(LunarCrater);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPrimaryKeyFieldException>()
                .WithLocation("`LunarCrater` → `Coordinate` (from \"Location\") → Longitude")
                .WithProblem("nested properties cannot be directly annotated as part of an Entity's primary key")
                .WithAnnotations("[PrimaryKey]")
                .EndMessage();
        }

        [TestMethod] public void CannotDeducePrincipalPrimaryKey_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(FederalLaw);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<CannotDeducePrimaryKeyException>()
                .WithLocation("`FederalLaw`")
                .WithProblem("the Primary Key for the Table could not be deduced")
                .EndMessage();
        }

        [TestMethod] public void CannotDeduceRelationPrimaryKeyWithoutCandidateKeys_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Lagerstatte);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<CannotDeducePrimaryKeyException>()
                .WithLocation("`Lagerstatte` → <synthetic> `Fossils`")
                .WithProblem("the primary key for the Table could not be deduced")
                .EndMessage();
        }

        [TestMethod] public void CannotDeduceRelationPrimaryKeyWithCandidateKeys_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Blockbuster);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<CannotDeducePrimaryKeyException>()
                .WithLocation("`Blockbuster` → <synthetic> `Rentals`")
                .WithProblem("the Primary Key for the Table could not be deduced")
                .EndMessage();
        }

        [TestMethod] public void PathIsNull_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Alphabet);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`Alphabet` → Name")
                .WithProblem("the path cannot be 'null'")
                .WithAnnotations("[PrimaryKey]")
                .EndMessage();
        }

        [TestMethod] public void PathOnScalar_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Highway);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`Highway` → Number")
                .WithProblem("the path \"---\" does not exist")
                .WithAnnotations("[PrimaryKey]")
                .EndMessage();
        }

        [TestMethod] public void NonExistentPathOnAggregate_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(ConfidenceInterval);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`ConfidenceInterval` → PlusMinus")
                .WithProblem("the path \"---\" does not exist")
                .WithAnnotations("[PrimaryKey]")
                .EndMessage();
        }

        [TestMethod] public void NonExistentPathOnReference_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(PhoneBooth);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`PhoneBooth` → Manufacturer")
                .WithProblem("the path \"---\" does not exist")
                .WithAnnotations("[PrimaryKey]")
                .EndMessage();
        }

        [TestMethod] public void NonPrimaryKeyPathOnReference_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(ScientificExperiment);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`ScientificExperiment` → ControlGroup")
                .WithProblem("the path \"Animate\" does not exist")
                .WithAnnotations("[PrimaryKey]")
                .EndMessage();
        }

        [TestMethod] public void PathOnReferenceRefersToPartiallyExposedAggregate_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Cryochamber);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`Cryochamber` → MinTemperature")
                .WithProblem("the path \"Temp\" does not exist")
                .WithAnnotations("[PrimaryKey]")
                .EndMessage();
        }

        [TestMethod] public void NonExistentPathOnRelation_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Missile);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`Missile` → <synthetic> `Manufacturers`")
                .WithProblem("the path \"---\" does not exist")
                .WithAnnotations("[PrimaryKey]")
                .EndMessage();
        }

        [TestMethod] public void NonAnchorPrimaryKeyPathOnRelation_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(TreasureMap);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`TreasureMap` → <synthetic> `SuggestedPath`")
                .WithProblem("the path \"TreasureMap.X\" does not exist")
                .WithAnnotations("[PrimaryKey]")
                .EndMessage();
        }

        [TestMethod] public void NoPathOnRelation_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Hologram);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Hologram` → <synthetic> `Copyrights`")
                .WithProblem("the annotation cannot be applied to a property of Relation type `RelationMap<short, string>`")
                .WithAnnotations("[PrimaryKey]")
                .EndMessage();
        }
    }

    [TestClass, TestCategory("Primary Key Naming")]
    public class PrimaryKeyNamingTests {
        [TestMethod] public void NamedPrimaryKey() {
            // Arrange
            var translator = new Translator();
            var source = typeof(HebrewLetter);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HavePrimaryKey("LetterPK").OfFields("Letter");
        }

        [TestMethod] public void NamedPrimaryKeyForUnnamedDeducedCandidateKey() {
            // Arrange
            var translator = new Translator();
            var source = typeof(DatabaseField);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HavePrimaryKey("PrimaryKey").OfFields("QualifiedName").And
                .HaveNoOtherCandidateKeys();
        }

        [TestMethod] public void NamedPrimaryKeyMatchesNamedDeducedCandidateKey_Redundant() {
            // Arrange
            var translator = new Translator();
            var source = typeof(TimeZone);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HavePrimaryKey("PK").OfFields("GMT").And
                .HaveNoOtherCandidateKeys();
        }

        [TestMethod] public void NamedPrimaryKeyOverridesNamedDeducedCandidateKey() {
            // Arrange
            var translator = new Translator();
            var source = typeof(JigsawPuzzle);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HavePrimaryKey("Something").OfFields("PuzzleIDFR");
        }

        [TestMethod] public void NamedPrimaryKeySameAsNonDeducedCandidateKey_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Currency);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<ConflictingKeyNameException>()
                .WithLocation("`Currency`")
                .WithProblem("name \"Key13\" is already taken by a candidate key consisting of {Character, ExchangeRate}")
                .WithAnnotations("[NamedPrimaryKey]")
                .EndMessage();
        }

        [TestMethod] public void NamedPrimaryKeyIsNull_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(HinduGod);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidNameException>()
                .WithLocation("`HinduGod`")
                .WithProblem("the name of a Primary Key cannot be 'null'")
                .WithAnnotations("[NamedPrimaryKey]")
                .EndMessage();
        }

        [TestMethod] public void NamedPrimaryKeyIsEmptyString_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Bay);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidNameException>()
                .WithLocation("`Bay`")
                .WithProblem("the name of a Primary Key cannot be empty")
                .WithAnnotations("[NamedPrimaryKey]")
                .EndMessage();
        }
    }
}
