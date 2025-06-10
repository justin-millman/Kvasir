using FluentAssertions;
using Kvasir.Schema;
using Kvasir.Translation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using static UT.Kvasir.Translation.Globals;
using static UT.Kvasir.Translation.PreDefinedEntities;

namespace UT.Kvasir.Translation {
    [TestClass, TestCategory("Pre-Defined Entities")]
    public class PreDefinedEntityTests {
        [TestMethod] public void ZeroInstances_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(StainedGlassWindow);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<NotEnoughInstancesException>()
                .WithLocation("`StainedGlassWindow`")
                .WithProblem("expected at least 2 pre-defined instances, but found 0")
                .EndMessage();
        }

        [TestMethod] public void OneInstance_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Gulf);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<NotEnoughInstancesException>()
                .WithLocation("`Gulf`")
                .WithProblem("expected at least 2 pre-defined instances, but found 1")
                .EndMessage();
        }

        [TestMethod] public void TwoPlusInstances() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Disciple);

            // Act
            var translation = translator[source];

            // Assert
            translation.CLRSource.Should().Be(source);
            translation.Principal.Table.Name.Should().Be("UT.Kvasir.Translation.PreDefinedEntities+DiscipleTable");
            translation.Principal.Table.Should()
                .HaveField("Name").OfTypeText().BeingNonNullable().And
                .HaveField("Epithet").OfTypeText().BeingNullable().And
                .HaveField("FeastDay").OfTypeDateTime().BeingNullable().And
                .HaveNoOtherFields().And
                .HavePrimaryKey().OfFields("Name").And
                .HaveNoOtherCandidateKeys().And
                .HaveNoOtherForeignKeys().And
                .HaveNoOtherForeignKeys();
            translation.Principal.PreDefinedInstances.Should().HaveCount(12);
            translation.Principal.PreDefinedInstances.Should().Contain(Disciple.SimonI);
            translation.Principal.PreDefinedInstances.Should().Contain(Disciple.Andrew);
            translation.Principal.PreDefinedInstances.Should().Contain(Disciple.JamesI);
            translation.Principal.PreDefinedInstances.Should().Contain(Disciple.John);
            translation.Principal.PreDefinedInstances.Should().Contain(Disciple.Philip);
            translation.Principal.PreDefinedInstances.Should().Contain(Disciple.Bartholomew);
            translation.Principal.PreDefinedInstances.Should().Contain(Disciple.Thomas);
            translation.Principal.PreDefinedInstances.Should().Contain(Disciple.Matthew);
            translation.Principal.PreDefinedInstances.Should().Contain(Disciple.JamesII);
            translation.Principal.PreDefinedInstances.Should().Contain(Disciple.Thaddeus);
            translation.Principal.PreDefinedInstances.Should().Contain(Disciple.SimonII);
            translation.Principal.PreDefinedInstances.Should().Contain(Disciple.Judas);
        }

        [TestMethod] public void PubliclyWriteableFieldProperty_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Olive);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPropertyInDataModelException>()
                .WithLocation("`Olive` → OilContent")
                .WithProblem("a writeable property cannot be included in the data model for a Pre-Defined Entity")
                .EndMessage();
        }

        [TestMethod] public void NonPubliclyWriteableFieldProperty_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Cloud);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPropertyInDataModelException>()
                .WithLocation("`Cloud` → Abbreviation")
                .WithProblem("a writeable property cannot be included in the data model for a Pre-Defined Entity")
                .EndMessage();
        }

        [TestMethod] public void PubliclyWriteableInstanceProperty_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(LayerOfSkin);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPreDefinedInstanceException>()
                .WithLocation("`LayerOfSkin` → Hypodermis")
                .WithProblem("a writeable property cannot be a pre-defined instance")
                .EndMessage();
        }

        [TestMethod] public void NonPubliclyWriteableInstanceProperty() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(PunctuationMark);

            // Act
            var translation = translator[source];

            // Assert
            translation.CLRSource.Should().Be(source);
            translation.Principal.Table.Name.Should().Be("UT.Kvasir.Translation.PreDefinedEntities+PunctuationMarkTable");
            translation.Principal.Table.Should()
                .HaveField("Character").OfTypeCharacter().BeingNonNullable().And
                .HaveField("Name").OfTypeText().BeingNonNullable().And
                .HaveNoOtherFields().And
                .HavePrimaryKey().OfFields("Character").And
                .HaveNoOtherCandidateKeys().And
                .HaveNoOtherForeignKeys().And
                .HaveNoOtherForeignKeys();
            translation.Principal.PreDefinedInstances.Should().HaveCount(9);
            translation.Principal.PreDefinedInstances.Should().Contain(PunctuationMark.QuestionMark);
            translation.Principal.PreDefinedInstances.Should().Contain(PunctuationMark.ExclamationMark);
            translation.Principal.PreDefinedInstances.Should().Contain(PunctuationMark.Period);
            translation.Principal.PreDefinedInstances.Should().Contain(PunctuationMark.Comma);
            translation.Principal.PreDefinedInstances.Should().Contain(PunctuationMark.QuotationMark);
            translation.Principal.PreDefinedInstances.Should().Contain(PunctuationMark.Apostrophe);
            translation.Principal.PreDefinedInstances.Should().Contain(PunctuationMark.Hyphen);
            translation.Principal.PreDefinedInstances.Should().Contain(PunctuationMark.OpenParenthesis);
            translation.Principal.PreDefinedInstances.Should().Contain(PunctuationMark.CloseParenthesis);
        }

        [TestMethod] public void PublicConstructor_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(FederalReserveDistrict);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidEntityTypeException>()
                .WithLocation("`FederalReserveDistrict`")
                .WithProblem("a Pre-Defined Entity cannot have a public constructor")
                .EndMessage();
        }

        [TestMethod] public void ReferenceToPreDefinedEntity() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(RavnicaGuild);

            // Act
            var translation = translator[source];

            // Assert
            translation.CLRSource.Should().Be(source);
            translation.Principal.Table.Name.Should().Be("UT.Kvasir.Translation.PreDefinedEntities+RavnicaGuildTable");
            translation.Principal.Table.Should()
                .HaveField("Name").OfTypeText().BeingNonNullable().And
                .HaveField("GuildHall").OfTypeText().BeingNonNullable().And
                .HaveField("Parun").OfTypeText().BeingNonNullable().And
                .HaveField("FirstMana.ID").OfTypeUInt32().BeingNonNullable().And
                .HaveField("SecondMana.ID").OfTypeUInt32().BeingNonNullable().And
                .HaveNoOtherFields().And
                .HavePrimaryKey().OfFields("Name").And
                .HaveNoOtherCandidateKeys().And
                .HaveForeignKey("FirstMana.ID")
                    .Against(translator[typeof(RavnicaGuild.Mana)].Principal.Table)
                    .WithOnDeleteBehavior(OnDelete.Cascade)
                    .WithOnUpdateBehavior(OnUpdate.Cascade).And
                .HaveForeignKey("SecondMana.ID")
                    .Against(translator[typeof(RavnicaGuild.Mana)].Principal.Table)
                    .WithOnDeleteBehavior(OnDelete.Cascade)
                    .WithOnUpdateBehavior(OnUpdate.Cascade).And
                .HaveNoOtherForeignKeys();
            translation.Principal.PreDefinedInstances.Should().HaveCount(10);
            translation.Principal.PreDefinedInstances.Should().Contain(RavnicaGuild.Azorius);
            translation.Principal.PreDefinedInstances.Should().Contain(RavnicaGuild.Dimir);
            translation.Principal.PreDefinedInstances.Should().Contain(RavnicaGuild.Rakdos);
            translation.Principal.PreDefinedInstances.Should().Contain(RavnicaGuild.Gruul);
            translation.Principal.PreDefinedInstances.Should().Contain(RavnicaGuild.Selesnya);
            translation.Principal.PreDefinedInstances.Should().Contain(RavnicaGuild.Orzhov);
            translation.Principal.PreDefinedInstances.Should().Contain(RavnicaGuild.Izzet);
            translation.Principal.PreDefinedInstances.Should().Contain(RavnicaGuild.Golgari);
            translation.Principal.PreDefinedInstances.Should().Contain(RavnicaGuild.Boros);
            translation.Principal.PreDefinedInstances.Should().Contain(RavnicaGuild.Simic);
        }

        [TestMethod] public void AggregateNestedReferenceToPreDefinedEntity() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(MarxBrother);

            // Act
            var translation = translator[source];

            // Assert
            translation.CLRSource.Should().Be(source);
            translation.Principal.Table.Name.Should().Be("UT.Kvasir.Translation.PreDefinedEntities+MarxBrotherTable");
            translation.Principal.Table.Should()
                .HaveField("Name").OfTypeText().BeingNonNullable().And
                .HaveField("BirthDate.Day").OfTypeUInt8().BeingNonNullable().And
                .HaveField("BirthDate.Month.Index").OfTypeUInt32().BeingNonNullable().And
                .HaveField("BirthDate.Year").OfTypeUInt16().BeingNonNullable().And
                .HaveField("FilmDebut").OfTypeText().BeingNonNullable().And
                .HaveNoOtherFields().And
                .HavePrimaryKey().OfFields("Name").And
                .HaveNoOtherCandidateKeys().And
                .HaveForeignKey("BirthDate.Month.Index")
                    .Against(translator[typeof(MarxBrother.Month)].Principal.Table)
                    .WithOnDeleteBehavior(OnDelete.Cascade)
                    .WithOnUpdateBehavior(OnUpdate.Cascade).And
                .HaveNoOtherForeignKeys();
            translation.Principal.PreDefinedInstances.Should().HaveCount(5);
            translation.Principal.PreDefinedInstances.Should().Contain(MarxBrother.Chico);
            translation.Principal.PreDefinedInstances.Should().Contain(MarxBrother.Harpo);
            translation.Principal.PreDefinedInstances.Should().Contain(MarxBrother.Groucho);
            translation.Principal.PreDefinedInstances.Should().Contain(MarxBrother.Gummo);
            translation.Principal.PreDefinedInstances.Should().Contain(MarxBrother.Zeppo);
        }

        [TestMethod] public void RelationToPreDefinedEntity() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(ResidentEvil);

            // Act
            var translation = translator[source];

            // Assert
            translation.Relations.Should().HaveCount(1);
            translation.Relations[0].Table.Should()
                .HaveName("UT.Kvasir.Translation.PreDefinedEntities+ResidentEvil.ModesTable").And
                .HaveField("ResidentEvil.GameID").OfTypeUInt64().BeingNonNullable().And
                .HaveField("Item.ModeID").OfTypeUInt32().BeingNonNullable().And
                .HaveNoOtherFields().And
                .HaveForeignKey("ResidentEvil.GameID")
                    .Against(translation.Principal.Table)
                    .WithOnDeleteBehavior(OnDelete.Cascade)
                    .WithOnUpdateBehavior(OnUpdate.Cascade).And
                .HaveForeignKey("Item.ModeID")
                    .Against(translator[typeof(ResidentEvil.GamingMode)].Principal.Table)
                    .WithOnDeleteBehavior(OnDelete.Cascade)
                    .WithOnUpdateBehavior(OnUpdate.Cascade).And
                .HaveNoOtherForeignKeys();
        }

        [TestMethod] public void AggregateNestedRelationToPreDefinedEntity() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(CitrusFruit);

            // Act
            var translation = translator[source];

            // Assert
            translation.Relations.Should().HaveCount(1);
            translation.Relations[0].Table.Should()
                .HaveName("UT.Kvasir.Translation.PreDefinedEntities+CitrusFruit.Bio.TaxonomyTable").And
                .HaveField("CitrusFruit.ID").OfTypeUInt32().BeingNonNullable().And
                .HaveField("Key.Symbol").OfTypeCharacter().BeingNonNullable().And
                .HaveField("Value").OfTypeText().BeingNonNullable().And
                .HaveNoOtherFields().And
                .HaveForeignKey("CitrusFruit.ID")
                    .Against(translation.Principal.Table)
                    .WithOnDeleteBehavior(OnDelete.Cascade)
                    .WithOnUpdateBehavior(OnUpdate.Cascade).And
                .HaveForeignKey("Key.Symbol")
                    .Against(translator[typeof(CitrusFruit.TaxonomicRank)].Principal.Table)
                    .WithOnDeleteBehavior(OnDelete.Cascade)
                    .WithOnUpdateBehavior(OnUpdate.Cascade).And
                .HaveNoOtherForeignKeys();
        }

        [TestMethod] public void ReferenceToNonPreDefinedEntity_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(CapnCrunch);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<PreDefinedReferenceException>()
                .WithLocation("`CapnCrunch` → FirstReleased")
                .WithProblem("a Pre-Defined Entity cannot reference non-Pre-Defined Entity type `Date`")
                .EndMessage();
        }

        [TestMethod] public void AggregateNestedReferenceToNonPreDefinedEntity_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(StageOfGrief);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<PreDefinedReferenceException>()
                .WithLocation("`StageOfGrief` → `Article` (from \"ProposedIn\") → Author")
                .WithProblem("a Pre-Defined Entity cannot reference non-Pre-Defined Entity type `Psychologist`")
                .EndMessage();
        }

        [TestMethod] public void AggregateNestedReferenceToNonPreDefinedEntity_PostMemoization_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Stooge);

            // Act
            var _ = translator[typeof(Stooge.ProductionCompany)];
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<PreDefinedReferenceException>()
                .WithLocation("`Stooge` → `FilmDeal` (from \"InitialDeal\") → FirstFilm")
                .WithProblem("a Pre-Defined Entity cannot reference non-Pre-Defined Entity type `Film`")
                .EndMessage();
        }

        [TestMethod] public void RelationToNonPreDefinedEntity_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(CivVIYield);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<PreDefinedReferenceException>()
                .WithLocation("`CivVIYield` → <synthetic> `ProducedBy` → Item")
                .WithProblem("a Pre-Defined Entity cannot contain a Relation involving non-Pre-Defined Entity type `District`")
                .EndMessage();
        }

        [TestMethod] public void AggregateNestedRelationToNonPreDefinedEntity_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(StateOfMatter);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<PreDefinedReferenceException>()
                .WithLocation("`StateOfMatter` → `Theory` (from \"ExplanatoryTheory\") → <synthetic> `Namesakes` → Item")
                .WithProblem("a Pre-Defined Entity cannot contain a Relation involving non-Pre-Defined Entity type `Scientist`")
                .EndMessage();
        }

        [TestMethod] public void AggregateNestedRelationToNonPreDefinedEntity_PostMemoization_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Primate);

            // Act
            _ = translator[typeof(Primate.Primatologist)];
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<PreDefinedReferenceException>()
                .WithLocation("`Primate` → `Journal` (from \"DedicatedJournal\") → <synthetic> `Studies` → Item")
                .WithProblem("a Pre-Defined Entity cannot contain a Relation involving non-Pre-Defined Entity type `Study`")
                .EndMessage();
        }
    }
}
