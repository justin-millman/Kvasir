using FluentAssertions;
using Kvasir.Translation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using static UT.Kvasir.Translation.Globals;
using static UT.Kvasir.Translation.Nullability;
using static UT.Kvasir.Translation.TableNaming;

namespace UT.Kvasir.Translation {
    [TestClass, TestCategory("Table Naming")]
    public class TableNamingTests {
        [TestMethod] public void PrimaryTableRenamedToBrandNewName() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(PlayingCard);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Name.Should().Be("DeckOfCards");
        }

        [TestMethod] public void NamespaceExcludedFromDefaultPrimaryTableName() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Pokemon);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Name.Should().Be("PokemonTable");
        }

        [TestMethod] public void NamespaceExcludedFromRelationTableName() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(PrisonerExchange);

            // Act
            var translation = translator[source];

            // Assert
            translation.Relations[0].Table.Should().HaveName("PrisonerExchange.AtoBTable");
            translation.Relations[1].Table.Should().HaveName("PrisonerExchange.BtoATable");
        }

        [TestMethod] public void PrimaryTable_DuplicateNameWithPrimaryTable_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var firstSource = typeof(Flight);
            var secondSource = typeof(Battle);

            // Act
            _ = translator[firstSource];
            var translate = () => translator[secondSource];

            // Assert
            translate.Should().FailWith<DuplicateNameException>()
                .WithLocation("`Battle`")
                .WithProblem("Table name \"Miscellaneous\" is already in use for the Principal Table of `Flight`")
                .EndMessage();
        }

        [TestMethod] public void PrimaryTable_NameIsUnchanged_Redundant() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Bookmark);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Name.Should().Be("UT.Kvasir.Translation.TableNaming+BookmarkTable");
        }

        [TestMethod] public void PrimaryTable_NameChangedToNull_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(SmokeDetector);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidNameException>()
                .WithLocation("`SmokeDetector`")
                .WithProblem("the name of a Primary Table cannot be 'null'")
                .WithAnnotations("[Table]")
                .EndMessage();
        }

        [TestMethod] public void PrimaryTable_NameChangedToEmptyString_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(LogIn);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidNameException>()
                .WithLocation("`LogIn`")
                .WithProblem("the name of a Primary Table cannot be empty")
                .WithAnnotations("[Table]")
                .EndMessage();
        }

        [TestMethod] public void CombinedAnnotation_TableAndExcludeNamespaceFromName_Equivalent_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Umbrella);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidNameException>()
                .WithLocation("`Umbrella`")
                .WithProblem("the name of a Primary Table cannot be empty")
                .WithAnnotations("[ExcludeNamespaceFromName]")
                .EndMessage();
        }

        [TestMethod] public void CombinedAnnotation_TableAndExcludeNamespaceFromName_Prefixed_LatterEnforced() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Blender);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Name.Should().Be("BlenderTable");
        }

        [TestMethod] public void CombinedAnnotation_TableAndExcludeNamespaceFromName_Infixed_LatterRedundant() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(BoardingSchool);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should().HaveName("Database.UT.Kvasir.Translation.TableNaming+BoardingSchoolTable");
        }

        [TestMethod] public void CombinedAnnotation_TableAndExcludeNamespaceFromName_Suffixed_LatterRedundant() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(PolygraphTest);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should().HaveName("Polygraph.UT.Kvasir.Translation.TableNaming+");
        }

        [TestMethod] public void CombinedAnnotation_TableAndExcludeNamespaceFromName_NoOverlap_LatterRedundant() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Encryption);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Name.Should().Be("SomeTable");
        }

        [TestMethod] public void RelationTableRenamedToBrandNewName() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(MagicalPreserve);

            // Act
            var translation = translator[source];

            // Assert
            translation.Relations.Should().HaveCount(1);
            translation.Relations[0].Table.Should().HaveName("CreaturesTable");
        }

        [TestMethod] public void RelationTable_NameIsUnchanged_Redundant() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(PacerTest);

            // Act
            var translation = translator[source];

            // Assert
            translation.Relations.Should().HaveCount(1);
            translation.Relations[0].Table.Should().HaveName("UT.Kvasir.Translation.TableNaming+PacerTest.LapsCompletedTable");
        }

        [TestMethod] public void RelationTable_NameChangedToNull_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Dwarf);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidNameException>()
                .WithLocation("`Dwarf` → <synthetic> `LifeEvents`")
                .WithProblem("the name of a Relation Table cannot be 'null'")
                .WithAnnotations("[RelationTable]")
                .EndMessage();
        }

        [TestMethod] public void RelationTable_NameChangedToEmptyString_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Rodent);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidNameException>()
                .WithLocation("`Rodent` → <synthetic> `Taxonomy`")
                .WithProblem("the name of a Relation Table cannot be empty")
                .WithAnnotations("[RelationTable]")
                .EndMessage();
        }

        [TestMethod] public void RelationTable_DuplicateNameWithRelationTable_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Vowel);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<DuplicateNameException>()
                .WithLocation("`Vowel` → <synthetic> `Languages`")
                .WithProblem("Table name \"AuxiliaryVowelTable\" is already in use for the Relation Table of `Vowel` → <synthetic> `Diacritics`")
                .EndMessage();
        }

        [TestMethod] public void RelationTable_DuplicateNameWithPrimaryTable_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(VPN);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<DuplicateNameException>()
                .WithLocation("`VPN` → <synthetic> `AuthorizedUsers`")
                .WithProblem("Table name \"OfficialInfoVPN\" is already in use for the Principal Table of `VPN`")
                .EndMessage();
        }

        [TestMethod] public void RelationTable_AppliedToNumericField_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Shofar);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<NotRelationException>()
                .WithLocation("`Shofar` → Tekiah")
                .WithProblem("the property type `float` is not a Relation")
                .WithAnnotations("[RelationTable]")
                .EndMessage();
        }

        [TestMethod] public void RelationTable_AppliedToTextualField_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(LawnGnome);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<NotRelationException>()
                .WithLocation("`LawnGnome` → Manufacturer")
                .WithProblem("the property type `string` is not a Relation")
                .WithAnnotations("[RelationTable]")
                .EndMessage();
        }

        [TestMethod] public void RelationTable_AppliedToBooleanField_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(GovernmentShutdown);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<NotRelationException>()
                .WithLocation("`GovernmentShutdown` → RepublicansInCharge")
                .WithProblem("the property type `bool` is not a Relation")
                .WithAnnotations("[RelationTable]")
                .EndMessage();
        }

        [TestMethod] public void RelationTable_AppliedToDateTimeField_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(CoalMine);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<NotRelationException>()
                .WithLocation("`CoalMine` → LastCollapse")
                .WithProblem("the property type `DateTime?` is not a Relation")
                .WithAnnotations("[RelationTable]")
                .EndMessage();
        }

        [TestMethod] public void RelationTable_AppliedToGuidField_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(LawnMower);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<NotRelationException>()
                .WithLocation("`LawnMower` → ApplianceID")
                .WithProblem("the property type `Guid` is not a Relation")
                .WithAnnotations("[RelationTable]")
                .EndMessage();
        }

        [TestMethod] public void RelationTable_AppliedToEnumerationField_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Triplets);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<NotRelationException>()
                .WithLocation("`Triplets` → Zygosity")
                .WithProblem("the property type `Cardinality` is not a Relation")
                .WithAnnotations("[RelationTable]")
                .EndMessage();
        }

        [TestMethod] public void RelationTable_AppliedToAggregateField_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Toothbrush);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<NotRelationException>()
                .WithLocation("`Toothbrush` → Electric")
                .WithProblem("the property type `Electricity?` is not a Relation")
                .WithAnnotations("[RelationTable]")
                .EndMessage();
        }

        [TestMethod] public void RelationTable_AppliedToReferenceField_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Valet);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<NotRelationException>()
                .WithLocation("`Valet` → Company")
                .WithProblem("the property type `Organization` is not a Relation")
                .WithAnnotations("[RelationTable]")
                .EndMessage();
        }

        [TestMethod] public void RelationTable_AppliedToPreDefinedInstance_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(DEFCON);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`DEFCON` → Four")
                .WithProblem("the annotation cannot be applied to a pre-defined instance property")
                .WithAnnotations("[RelationTable]");
        }
    }
}
