using FluentAssertions;
using Kvasir.Translation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using static UT.Kvasir.Translation.Globals;
using static UT.Kvasir.Translation.CandidateKeys;

namespace UT.Kvasir.Translation {
    [TestClass, TestCategory("Candidate Keys")]
    public class CandidateKeyTests {
        [TestMethod] public void MultipleUnnamedCandidateKeys() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Inmate);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveAnonymousCandidateKey().OfFields("SSN").And
                .HaveAnonymousCandidateKey().OfFields("FullName").And
                .HaveNoOtherCandidateKeys();
        }

        [TestMethod] public void NamedCandidateKey() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(BowlGame);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveCandidateKey("Sponsorship").OfFields(
                    "PrimarySponsor",
                    "SecondarySponsor"
                ).And
                .HaveNoOtherCandidateKeys();
        }

        [TestMethod] public void SingleFieldInMultipleCandidateKeys() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(KingOfEngland);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveCandidateKey("Uno").OfFields(
                    "RegnalName",
                    "RegnalNumber"
                ).And
                .HaveCandidateKey("Another").OfFields(
                    "RegnalName",
                    "RoyalHouse"
                ).And
                .HaveCandidateKey("Third").OfFields(
                    "RegnalNumber",
                    "RoyalHouse"
                ).And
                .HaveNoOtherCandidateKeys();
        }

        [TestMethod] public void MultipleDirectIdenticalUnnamedCandidateKeys() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Pigment);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveAnonymousCandidateKey().OfFields("ChemicalFormula").And
                .HaveNoOtherCandidateKeys();
        }

        [TestMethod] public void MultipleIndirectIdenticalUnnamedCandidateKeys() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Octopus);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveAnonymousCandidateKey().OfFields("Nomenclature.Family").And
                .HaveNoOtherCandidateKeys();
        }

        [TestMethod] public void MultipleIdenticalNamedCandidateKeys() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(BankCheck);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveCandidateKey("N1").OfFields("CheckNumber").And
                .HaveNoOtherCandidateKeys();
        }

        [TestMethod] public void FieldInSameCandidateKeyMultipleTimes() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Desert);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveCandidateKey("Size").OfFields(
                    "Length",
                    "Width"
                ).And
                .HaveNoOtherCandidateKeys();
        }

        [TestMethod] public void AggregateInCandidateKeyAlone() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
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
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Shipwreck);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveCandidateKey("Identity").OfFields(
                    "Ship",
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
            var translator = new Translator(NO_ENTITIES);
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
            var translator = new Translator(NO_ENTITIES);
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
            var translator = new Translator(NO_ENTITIES);
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
            var translator = new Translator(NO_ENTITIES);
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
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(GreatOldOne);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveCandidateKey("Identity").OfFields(
                    "PantheonNumber",
                    "PrimaryEpithet.Name"
                ).And
                .HaveNoOtherCandidateKeys();
        }

        [TestMethod] public void ReferenceNestedScalarsInCandidateKey() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
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
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Kibbutz);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveCandidateKey("KBTZ").OfFields(
                    "EnglishName",
                    "Where.District.Name"
                ).And
                .HaveNoOtherCandidateKeys();
        }

        [TestMethod] public void PrimaryKeyReferenceFieldsNativelyInCandidateKeyNotPropagated() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(DiscworldGuild);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveNoOtherCandidateKeys();
        }
        
        [TestMethod] public void NonPrimaryKeyReferenceFieldsNativelyInCandidateKeyNotPropagated() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(HonestTrailer);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveNoOtherCandidateKeys();
        }

        [TestMethod] public void RelationNestedScalarsInCandidateKey() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
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
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(RentalCar);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`RentalCar` → Report")
                .WithPath("Renters")
                .WithProblem("the annotation cannot be applied to a property of Relation type `RelationMap<string, Duration>`")
                .WithAnnotations("[Unique]")
                .EndMessage();
        }

        [TestMethod] public void RelationFieldsNativelyInCandidateKey() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
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
            var translator = new Translator(NO_ENTITIES);
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

        [TestMethod] public void AnchorPlusOrderedListIndexIsCandidateKey_Redundant() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(OPO);

            // Act
            var translation = translator[source];

            // Assert
            translation.Relations[0].Table.Should()
                .HaveCandidateKey("Unique").OfFields(
                    "OPO.ID",
                    "Index"
                ).And
                .HaveNoOtherCandidateKeys();
        }

        [TestMethod] public void ScalarAndNestedFieldsInSameCandidateKey() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Sabermetric);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveCandidateKey("Lookup").OfFields(
                    "GamePhase",
                    "Formula.Formula"
                ).And
                .HaveNoOtherCandidateKeys();
        }

        [TestMethod] public void CandidateKeyNameIsNull_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(PlatonicDialogue);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidNameException>()
                .WithLocation("`PlatonicDialogue` → WordCount")
                .WithProblem("the name of a Candidate Key cannot be 'null'")
                .WithAnnotations("[Unique]")
                .EndMessage();
        }

        [TestMethod] public void CandidateKeyNameIsEmptyString_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Allomancy);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidNameException>()
                .WithLocation("`Allomancy` → MistingTerm")
                .WithProblem("the name of a Candidate Key cannot be empty")
                .WithAnnotations("[Unique]")
                .EndMessage();
        }

        [TestMethod] public void CandidateKeyNameIsReserved_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Lens);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidNameException>()
                .WithLocation("`Lens` → IndexOfRefraction")
                .WithProblem("the name of a Candidate Key cannot begin with the reserved character sequence \"@@@\"")
                .WithAnnotations("[Unique]")
                .EndMessage();
        }

        [TestMethod] public void CandidateKeyIsEquivalentToPrimaryKey() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(AchaeanNavalContingent);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should().HaveNoOtherCandidateKeys();
        }

        [TestMethod] public void CandidateKeyIsSupersetOfPrimaryKey() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(WorldHeritageSite);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should().HaveNoOtherCandidateKeys();
        }

        [TestMethod] public void PreDefinedInstanceInCandidateKey_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Cheesecake);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Cheesecake` → Ube")
                .WithProblem("the annotation cannot be applied to a pre-defined instance property")
                .WithAnnotations("[Unique]")
                .EndMessage();
        }

        [TestMethod] public void PathIsNull_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Tendon);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`Tendon` → Name")
                .WithProblem("the path cannot be 'null'")
                .WithAnnotations("[Unique]")
                .EndMessage();
        }

        [TestMethod] public void PathOnScalar_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Sonnet);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`Sonnet` → Line1")
                .WithProblem("the path \"---\" does not exist")
                .WithAnnotations("[Unique]")
                .EndMessage();
        }

        [TestMethod] public void NonExistentPathOnAggregate_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(EgyptianGod);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`EgyptianGod` → Name")
                .WithProblem("the path \"---\" does not exist")
                .WithAnnotations("[Unique]")
                .EndMessage();
        }

        [TestMethod] public void NonExistentPathOnReference_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Bachelorette);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`Bachelorette` → FinalRose")
                .WithProblem("the path \"---\" does not exist")
                .WithAnnotations("[Unique]")
                .EndMessage();
        }

        [TestMethod] public void NonPrimaryKeyPathOnReference_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Sherpa);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`Sherpa` → MainMountain")
                .WithProblem("the path \"TotalAscents\" does not exist")
                .WithAnnotations("[Unique]")
                .EndMessage();
        }

        [TestMethod] public void PathOnReferenceRefersToPartiallyExposedAggregate() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
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
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Antihistamine);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`Antihistamine` → <synthetic> `MedicalIdentifiers`")
                .WithProblem("the path \"---\" does not exist")
                .WithAnnotations("[Unique]")
                .EndMessage();
        }

        [TestMethod] public void NonAnchorPrimaryKeyPathOnRelation_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Oasis);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`Oasis` → <synthetic> `TreeSpecies`")
                .WithProblem("the path \"Oasis.Water\" does not exist")
                .WithAnnotations("[Unique]")
                .EndMessage();
        }

        [TestMethod] public void NoPathOnRelation_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(LimboCompetition);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`LimboCompetition` → <synthetic> `Heights`")
                .WithProblem("the annotation cannot be applied to a property of Relation type `IReadOnlyRelationMap<string, float>`")
                .WithAnnotations("[Unique]")
                .EndMessage();
        }
    }
}
