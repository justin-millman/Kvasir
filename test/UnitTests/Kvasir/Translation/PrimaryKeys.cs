using FluentAssertions;
using Kvasir.Exceptions;
using Kvasir.Translation;
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
                .HavePrimaryKey().OfFields(nameof(XKCDComic.URL));
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
                    nameof(Month.Calendar),
                    nameof(Month.Index)
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
                    nameof(SpaceShuttle.Name),
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
                    nameof(ChoppedBasket.AirDate),
                    nameof(ChoppedBasket.Round),
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
                    nameof(Prophecy.ProphecyID),
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
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Psalm.Text))                          // error location
                .WithMessageContaining("refers to a non-scalar")                    // category
                .WithMessageContaining("Verses")                                    // error sub-location
                .WithMessageContaining("[PrimaryKey]");                             // details / explanation
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
                    nameof(Character.Glyph),
                    nameof(Character.CodePoint),
                    nameof(Character.IsASCII)
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
                .HavePrimaryKey().OfFields(nameof(Actor.ID));
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
                .HavePrimaryKey().OfFields(nameof(IntegerSequence.IntegerSequenceID));
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
                .HavePrimaryKey().OfFields(nameof(Function.FunctionID));
        }

        [TestMethod] public void SingleCandidateKeyWithSingleNonNullableField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Star);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HavePrimaryKey().OfFields(nameof(Star.ARICNS)).And
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
                .HavePrimaryKey("PK").OfFields(nameof(Expiration.FeedCode), nameof(Expiration.Underlying));
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
                    nameof(Vitamin.Carbon),
                    nameof(Vitamin.Hydrogen),
                    nameof(Vitamin.Cobalt),
                    nameof(Vitamin.Nitrogen),
                    nameof(Vitamin.Oxygen),
                    nameof(Vitamin.Phosphorus)
                ).And
                .HaveCandidateKey("CK1").OfFields(
                    nameof(Vitamin.Name),
                    nameof(Vitamin.Alternative)
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
                .HavePrimaryKey("ID").OfFields(
                    nameof(Escalator.EscalatorIdentifier)
                ).And
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
                .HavePrimaryKey().OfFields(nameof(Repository.ID)).And
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
                .HavePrimaryKey().OfFields(nameof(Earthquake.SeismicIdentificationNumber));
        }

        [TestMethod] public void SingleAnnotatedNonNullableField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(GeologicEpoch);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HavePrimaryKey().OfFields(nameof(GeologicEpoch.StartingMYA));
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
                    nameof(HotAirBalloon.Manufacturer),
                    nameof(HotAirBalloon.MaxHeight),
                    nameof(HotAirBalloon.MaxAirTemperature),
                    nameof(HotAirBalloon.PassengerCapacity),
                    nameof(HotAirBalloon.Radius)
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
                .HavePrimaryKey().OfFields(nameof(Rollercoaster.RollercoasterID));
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
                    nameof(Doctor.Regeneration),
                    nameof(Doctor.Portrayal)
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
                .HavePrimaryKey().OfFields(nameof(Polyhedron.Name));
        }

        [TestMethod] public void ScalarMarkedPrimaryKeyMultipleTimes_Redundant() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Airport);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HavePrimaryKey().OfFields(nameof(Airport.IATA));
        }

        [TestMethod] public void NullableScalarMarkedPrimaryKey_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(NorseWorld);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(NorseWorld.EddaMentions))             // error location
                .WithMessageContaining("[PrimaryKey]")                              // details / explanation
                .WithMessageContaining("nullable Field");                           // details / explanation
        }

        [TestMethod] public void NullableAggregateMarkedPrimaryKey_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(MedievalCastle);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(MedievalCastle.Drawbridge))           // error location
                .WithMessageContaining("[PrimaryKey]")                              // details / explanation
                .WithMessageContaining("nullable Field");                           // details / explanation
        }

        [TestMethod] public void AggregateWithNullablePropertyMarkedPrimaryKey_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Wizard);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Wizard.Background))                   // error location
                .WithMessageContaining("Schooling.School")                          // error sub-location
                .WithMessageContaining("[PrimaryKey]")                              // details / explanation
                .WithMessageContaining("nullable Field");                           // details / explanation
        }

        [TestMethod] public void NullableReferenceMarkedPrimaryKey_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Avocado);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Avocado.CountryOfOrigin))             // error location
                .WithMessageContaining("[PrimaryKey]")                              // details / explanation
                .WithMessageContaining("nullable Field");                           // details / explanation
        }

        [TestMethod] public void PropertyInAggregateMarkedPrimaryKey_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(LunarCrater);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(nameof(LunarCrater.Coordinate))              // source type
                .WithMessageContaining(nameof(LunarCrater.Coordinate.Longitude))    // error location
                .WithMessageContaining("[PrimaryKey]")                              // details / explanation
                .WithMessageContaining("nested Field");                             // details / explanation
        }

        [TestMethod] public void CannotDeducePrincipalPrimaryKey_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(FederalLaw);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining("could not deduce Primary Key");             // category
        }

        [TestMethod] public void CannotDeduceRelationPrimaryKeyWithoutCandidateKeys_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Lagerstatte);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining("could not deduce Primary Key");             // category
        }

        [TestMethod] public void CannotDeduceRelationPrimaryKeyWithCandidateKeys_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Blockbuster);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining("could not deduce Primary Key");             // category
        }

        [TestMethod] public void PathIsNull_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Alphabet);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Alphabet.Name))                       // error location
                .WithMessageContaining("path is null")                              // category
                .WithMessageContaining("[PrimaryKey]");                             // details / explanation
        }

        [TestMethod] public void PathOnScalar_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Highway);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Highway.Number))                      // error location
                .WithMessageContaining("path*does not exist")                       // category
                .WithMessageContaining("[PrimaryKey]")                              // details / explanation
                .WithMessageContaining("\"---\"");                                  // details / explanation
        }

        [TestMethod] public void NonExistentPathOnAggregate_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(ConfidenceInterval);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(ConfidenceInterval.PlusMinus))        // error location
                .WithMessageContaining("path*does not exist")                       // category
                .WithMessageContaining("[PrimaryKey]")                              // details / explanation
                .WithMessageContaining("\"---\"");                                  // details / explanation
        }

        [TestMethod] public void NonExistentPathOnReference_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(PhoneBooth);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(PhoneBooth.Manufacturer))             // error location
                .WithMessageContaining("path*does not exist")                       // category
                .WithMessageContaining("[PrimaryKey]")                              // details / explanation
                .WithMessageContaining("\"---\"");                                  // details / explanation
        }

        [TestMethod] public void NonPrimaryKeyPathOnReference_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(ScientificExperiment);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(ScientificExperiment.ControlGroup))   // error location
                .WithMessageContaining("path*does not exist")                       // category
                .WithMessageContaining("[PrimaryKey]")                              // details / explanation
                .WithMessageContaining("\"Animate\"");                              // details / explanation
        }

        [TestMethod] public void PathOnReferenceRefersToPartiallyExposedAggregate_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Cryochamber);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Cryochamber.MinTemperature))          // error location
                .WithMessageContaining("path*does not exist")                       // category
                .WithMessageContaining("[PrimaryKey]")                              // details / explanation
                .WithMessageContaining("\"Temp\"");                                 // details / explanation
        }

        [TestMethod] public void NonExistentPathOnRelation_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Missile);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Missile.Manufacturers))               // error location
                .WithMessageContaining("path*does not exist")                       // category
                .WithMessageContaining("[PrimaryKey]")                              // details / explanation
                .WithMessageContaining("\"---\"");                                  // details / explanation
        }

        [TestMethod] public void NonAnchorPrimaryKeyPathOnRelation_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(TreasureMap);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(TreasureMap.SuggestedPath))           // error location
                .WithMessageContaining("path*does not exist")                       // category
                .WithMessageContaining("[PrimaryKey]")                              // details / explanation
                .WithMessageContaining("\"X\"");                                    // details / explanation
        }

        [TestMethod] public void IsGreaterThan_NoPathOnRelation_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Hologram);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Hologram.Copyrights))                 // error location
                .WithMessageContaining("path is required")                          // category
                .WithMessageContaining("[PrimaryKey]");                             // details / explanation
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
                .HavePrimaryKey("LetterPK").OfFields(nameof(HebrewLetter.Letter));
        }

        [TestMethod] public void NamedPrimaryKeyForUnnamedDeducedCandidateKey() {
            // Arrange
            var translator = new Translator();
            var source = typeof(DatabaseField);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HavePrimaryKey("PrimaryKey").OfFields(nameof(DatabaseField.QualifiedName)).And
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
                .HavePrimaryKey("PK").OfFields(nameof(TimeZone.GMT)).And
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
                .HavePrimaryKey("Something").OfFields(nameof(JigsawPuzzle.PuzzleIDFR));
        }

        [TestMethod] public void NamedPrimaryKeySameAsNonDeducedCandidateKey_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Currency);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining("name*is invalid")                           // category
                .WithMessageContaining("[NamedPrimaryKey]")                         // details / explanation
                .WithMessageContaining("Key13")                                     // details / explanation
                .WithMessageContaining("non-deduced Candidate Key");                // details / explanation
        }

        [TestMethod] public void NamedPrimaryKeyIsNull_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(HinduGod);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining("name is null")                              // category
                .WithMessageContaining("[NamedPrimaryKey]");                        // details / explanation
        }

        [TestMethod] public void NamedPrimaryKeyIsEmptyString_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Bay);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining("name*is invalid")                           // category
                .WithMessageContaining("[NamedPrimaryKey]")                         // details / explanation
                .WithMessageContaining("\"\"");                                     // details / explanation
        }
    }
}
