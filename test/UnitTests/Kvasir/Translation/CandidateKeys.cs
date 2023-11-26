using FluentAssertions;
using Kvasir.Exceptions;
using Kvasir.Translation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using static UT.Kvasir.Translation.CandidateKeys;
namespace UT.Kvasir.Translation {
    [TestClass, TestCategory("Candidate Keys")]
    public class CandidateKeyTests {
        [TestMethod] public void MultipleUnnamedCandidateKeys() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Inmate);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveAnonymousCandidateKey().OfFields(nameof(Inmate.SSN)).And
                .HaveAnonymousCandidateKey().OfFields(nameof(Inmate.FullName)).And
                .HaveNoOtherCandidateKeys();
        }

        [TestMethod] public void NamedCandidateKey() {
            // Arrange
            var translator = new Translator();
            var source = typeof(BowlGame);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveCandidateKey("Sponsorship").OfFields(
                    nameof(BowlGame.PrimarySponsor),
                    nameof(BowlGame.SecondarySponsor)
                ).And
                .HaveNoOtherCandidateKeys();
        }

        [TestMethod] public void SingleFieldInMultipleCandidateKeys() {
            // Arrange
            var translator = new Translator();
            var source = typeof(KingOfEngland);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveCandidateKey("Uno").OfFields(
                    nameof(KingOfEngland.RegnalName),
                    nameof(KingOfEngland.RegnalNumber)
                ).And
                .HaveCandidateKey("Another").OfFields(
                    nameof(KingOfEngland.RegnalName),
                    nameof(KingOfEngland.RoyalHouse)
                ).And
                .HaveCandidateKey("Third").OfFields(
                    nameof(KingOfEngland.RegnalNumber),
                    nameof(KingOfEngland.RoyalHouse)
                ).And
                .HaveNoOtherCandidateKeys();
        }

        [TestMethod] public void MultipleIdenticalUnnamedCandidateKeys() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Pigment);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveAnonymousCandidateKey().OfFields(nameof(Pigment.ChemicalFormula)).And
                .HaveNoOtherCandidateKeys();
        }

        [TestMethod] public void MultipleIdenticalNamedCandidateKeys_() {
            // Arrange
            var translator = new Translator();
            var source = typeof(BankCheck);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveCandidateKey("N3").OfFields(nameof(BankCheck.CheckNumber)).And
                .HaveNoOtherCandidateKeys();
        }

        [TestMethod] public void FieldInSameCandidateKeyMultipleTimes() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Desert);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveCandidateKey("Size").OfFields(
                    nameof(Desert.Length),
                    nameof(Desert.Width)
                ).And
                .HaveNoOtherCandidateKeys();
        }

        [TestMethod] public void AggregateInCandidateKeyAlone() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Ointment);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveAnonymousCandidateKey().OfFields(
                    "Composition.PcntWater",
                    "Composition.PcntOil"
                ).And
                .HaveNoOtherCandidateKeys();
        }

        [TestMethod] public void AggregateInCandidateKeyWithOtherFields() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Shipwreck);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveCandidateKey("Identity").OfFields(
                    nameof(Shipwreck.Ship),
                    "Location.Latitude",
                    "Location.Longitude"
                ).And
                .HaveAnonymousCandidateKey().OfFields(
                    "FurthestExtent.Latitude",
                    "FurthestExtent.Longitude"
                ).And
                .HaveNoOtherCandidateKeys();
        }

        [TestMethod] public void AggregateNestedScalarsInCandidateKey() {
            // Arrange
            var translator = new Translator();
            var source = typeof(SpiderMan);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveCandidateKey("AlterEgo").OfFields(
                    "AlterEgo.FirstName",
                    "AlterEgo.LastName"
                ).And
                .HaveCandidateKey("Portrayal").OfFields(
                    "Portrayal.FirstName",
                    "Portrayal.MiddleName",
                    "Portrayal.LastName"
                ).And
                .HaveNoOtherCandidateKeys();
        }

        [TestMethod] public void NestedAggregatesInCandidateKey() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Neurotransmitter);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveAnonymousCandidateKey().OfFields(
                    "Definition.Name.Name",
                    "Definition.Name.Abbreviation"
                ).And
                .HaveNoOtherCandidateKeys();
        }

        [TestMethod] public void AggregateFieldsNativelyInCandidateKey() {
            // Arrange
            var translator = new Translator();
            var source = typeof(ZoomMeeting);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveAnonymousCandidateKey().OfFields("Credentials.MeetingID").And
                .HaveCandidateKey("JoinKey").OfFields(
                    "Credentials.MeetingNumber",
                    "Credentials.PassCode"
                ).And
                .HaveNoOtherCandidateKeys();
        }

        [TestMethod] public void ReferenceInCandidateKeyAlone() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Luau);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveAnonymousCandidateKey().OfFields(
                    "SucklingPig.BatchID",
                    "SucklingPig.LotNumber"
                ).And
                .HaveNoOtherCandidateKeys();
        }

        [TestMethod] public void ReferenceInCandidateKeyWithOtherFields() {
            // Arrange
            var translator = new Translator();
            var source = typeof(GreatOldOne);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveCandidateKey("Identity").OfFields(
                    nameof(GreatOldOne.PantheonNumber),
                    "PrimaryEpithet.Name"
                ).And
                .HaveNoOtherCandidateKeys();
        }

        [TestMethod] public void ReferenceNestedScalarsInCandidateKey() {
            // Arrange
            var translator = new Translator();
            var source = typeof(JapaneseEmperor);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveCandidateKey("Eras").OfFields(
                    "EndEra.EnglishEraName",
                    "StartEra.JapaneseEraName"
                ).And
                .HaveNoOtherCandidateKeys();
        }

        [TestMethod] public void NestedReferencesInCandidateKey() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Kibbutz);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveCandidateKey("KBTZ").OfFields(
                    nameof(Kibbutz.EnglishName),
                    "Where.District.Name"
                ).And
                .HaveNoOtherCandidateKeys();
        }

        [TestMethod] public void PrimaryKeyReferenceFieldsNativelyInCandidateKeyNotPropagated() {
            // Arrange
            var translator = new Translator();
            var source = typeof(DiscworldGuild);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveNoOtherCandidateKeys();
        }
        
        [TestMethod] public void NonPrimaryKeyReferenceFieldsNativelyInCandidateKeyNotPropagated() {
            // Arrange
            var translator = new Translator();
            var source = typeof(HonestTrailer);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveNoOtherCandidateKeys();
        }

        [TestMethod] public void RelationNestedScalarsInCandidateKey() {
            // Arrange
            var translator = new Translator();
            var source = typeof(BigBlockOfCheeseDay);

            // Act
            var translation = translator[source];

            // Assert
            translation.Relations[0].Table.Should()
                .HaveCandidateKey("X").OfFields(
                    "BigBlockOfCheeseDay.Episode",
                    "Key.Organization"
                ).And
                .HaveNoOtherCandidateKeys();
        }

        [TestMethod] public void NestedRelationsInCandidateKey_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(RentalCar);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(RentalCar.Report))                    // error location
                .WithMessageContaining("Renters")                                   // error sub-location
                .WithMessageContaining("refers to a non-scalar")                    // category
                .WithMessageContaining("[Unique]");                                 // details / explanation
        }

        [TestMethod] public void RelationFieldsNativelyInCandidateKey() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Intifada);

            // Act
            var translation = translator[source];

            // Assert
            translation.Relations[0].Table.Should()
                .HaveAnonymousCandidateKey().OfFields(
                    "Item.CountryName"
                ).And
                .HaveCandidateKey("Code").OfFields(
                    "Item.CountryCode"
                ).And
                .HaveNoOtherCandidateKeys();
        }

        [TestMethod] public void AnchorPlusMapKeyIsCandidateKey_Redundant() {
            // Arrange
            var translator = new Translator();
            var source = typeof(VoodooDoll);

            // Act
            var translation = translator[source];

            // Assert
            translation.Relations[0].Table.Should()
                .HaveCandidateKey("Unique").OfFields(
                    "VoodooDoll.VoodooID",
                    "Key"
                ).And
                .HaveNoOtherCandidateKeys();
        }

        [TestMethod] public void ScalarAndNestedFieldsInSameCandidateKey() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Sabermetric);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveCandidateKey("Lookup").OfFields(
                    nameof(Sabermetric.GamePhase),
                    "Formula.Formula"
                ).And
                .HaveNoOtherCandidateKeys();
        }

        [TestMethod] public void CandidateKeyNameIsNull_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(PlatonicDialogue);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(PlatonicDialogue.WordCount))          // error location
                .WithMessageContaining("name is null")                              // category
                .WithMessageContaining("[Unique]");                                 // details / explanation
        }

        [TestMethod] public void CandidateKeyNameIsEmptyString_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Allomancy);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Allomancy.MistingTerm))               // error location
                .WithMessageContaining("name*is invalid")                           // category
                .WithMessageContaining("[Unique]")                                  // details / explanation
                .WithMessageContaining("\"\"");                                     // details / explanation
        }

        [TestMethod] public void CandidateKeyNameIsReserved_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Lens);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Lens.IndexOfRefraction))              // error location
                .WithMessageContaining("name*is invalid")                           // category
                .WithMessageContaining("[Unique]")                                  // details / explanation
                .WithMessageContaining("\"@@@Key\"")                                // details / explanation
                .WithMessageContaining("reserved character sequence \"@@@\"");      // details / explanation
        }

        [TestMethod] public void CandidateKeyIsEquivalentToPrimaryKey() {
            // Arrange
            var translator = new Translator();
            var source = typeof(AchaeanNavalContingent);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should().HaveNoOtherCandidateKeys();
        }

        [TestMethod] public void CandidateKeyIsSupersetOfPrimaryKey() {
            // Arrange
            var translator = new Translator();
            var source = typeof(WorldHeritageSite);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should().HaveNoOtherCandidateKeys();
        }

        [TestMethod] public void PathIsNull_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Tendon);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Tendon.Name))                         // error location
                .WithMessageContaining("path is null")                              // category
                .WithMessageContaining("[Unique]");                                 // details / explanation
        }

        [TestMethod] public void PathOnScalar_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Sonnet);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Sonnet.Line1))                        // error location
                .WithMessageContaining("path*does not exist")                       // category
                .WithMessageContaining("[Unique]")                                  // details / explanation
                .WithMessageContaining("\"---\"");                                  // details / explanation
        }

        [TestMethod] public void NonExistentPathOnAggregate_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(EgyptianGod);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(EgyptianGod.Name))                    // error location
                .WithMessageContaining("path*does not exist")                       // category
                .WithMessageContaining("[Unique]")                                  // details / explanation
                .WithMessageContaining("\"---\"");                                  // details / explanation
        }

        [TestMethod] public void NonExistentPathOnReference_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Bachelorette);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Bachelorette.FinalRose))              // error location
                .WithMessageContaining("path*does not exist")                       // category
                .WithMessageContaining("[Unique]")                                  // details / explanation
                .WithMessageContaining("\"---\"");                                  // details / explanation
        }

        [TestMethod] public void NonPrimaryKeyPathOnReference_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Sherpa);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Sherpa.MainMountain))                 // error location
                .WithMessageContaining("path*does not exist")                       // category
                .WithMessageContaining("[Unique]")                                  // details / explanation
                .WithMessageContaining("\"TotalAscents\"");                         // details / explanation
        }

        [TestMethod] public void PathOnReferenceRefersToPartiallyExposedAggregate() {
            // Arrange
            var translator = new Translator();
            var source = typeof(LawFirm);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveAnonymousCandidateKey().OfFields(
                    "Partners.FoundingPartner.LawSchool",
                    "Partners.FoundingPartner.License.BarNumber"
                ).And
                .HaveNoOtherCandidateKeys();
        }

        [TestMethod] public void NonExistentPathOnRelation_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Antihistamine);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Antihistamine.MedicalIdentifiers))    // error location
                .WithMessageContaining("path*does not exist")                       // category
                .WithMessageContaining("[Unique]")                                  // details / explanation
                .WithMessageContaining("\"---\"");                                  // details / explanation
        }

        [TestMethod] public void NonAnchorPrimaryKeyPathOnRelation_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Oasis);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Oasis.TreeSpecies))                   // error location
                .WithMessageContaining("path*does not exist")                       // category
                .WithMessageContaining("[Unique]")                                  // details / explanation
                .WithMessageContaining("\"Water\"");                                // details / explanation
        }

        [TestMethod] public void NoPathOnRelation_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(LimboCompetition);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(LimboCompetition.Heights))            // error location
                .WithMessageContaining("path is required")                          // category
                .WithMessageContaining("[Unique]");                                 // details / explanation
        }
    }
}
