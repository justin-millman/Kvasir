using FluentAssertions;
using Kvasir.Exceptions;
using Kvasir.Translation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

using static UT.Kvasir.Translation.TableNaming;

namespace UT.Kvasir.Translation {
    [TestClass, TestCategory("Table Naming")]
    public class TableNamingTests {
        [TestMethod] public void PrimaryTableRenamedToBrandNewName() {
            // Arrange
            var translator = new Translator();
            var source = typeof(PlayingCard);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Name.Should().Be("DeckOfCards");
        }

        [TestMethod] public void NamespaceExcludedFromDefaultPrimaryTableName() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Pokemon);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Name.Should().Be("PokemonTable");
        }

        [TestMethod] public void NamespaceExcludedFromRelationTableName() {
            // Arrange
            var translator = new Translator();
            var source = typeof(PrisonerExchange);

            // Act
            var translation = translator[source];

            // Assert
            translation.Relations[0].Table.Should().HaveName("PrisonerExchange.AtoBTable");
            translation.Relations[1].Table.Should().HaveName("PrisonerExchange.BtoATable");
        }

        [TestMethod] public void PrimaryTable_DuplicateNameWithPrimaryTable_IsError() {
            // Arrange
            var translator = new Translator();
            var firstSource = typeof(Flight);
            var secondSource = typeof(Battle);

            // Act
            _ = translator[firstSource];
            var translate = () => translator[secondSource];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(secondSource.Name)                           // source type
                .WithMessageContaining("Table name*is already in use")              // category
                .WithMessageContaining("\"Miscellaneous\"");                        // details / explanation
        }

        [TestMethod] public void PrimaryTable_NameIsUnchanged_Redundant() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Bookmark);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Name.Should().Be("UT.Kvasir.Translation.TableNaming+BookmarkTable");
        }

        [TestMethod] public void PrimaryTable_NameChangedToNull_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(SmokeDetector);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining("[Table]")                                   // details / explanation
                .WithMessageContaining("null");                                     // details / explanation
        }

        [TestMethod] public void PrimaryTable_NameChangedToEmptyString_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(LogIn);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining("[Table]")                                   // details / explanation
                .WithMessageContaining("\"\"");                                     // details / explanation
        }

        [TestMethod] public void CombinedAnnotation_TableAndExcludeNamespaceFromName_LatterEnforced() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Blender);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Name.Should().Be("BlenderTable");
        }

        [TestMethod] public void CombinedAnnotation_TableAndExcludeNamespaceFromName_LatterRedundant() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Encryption);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Name.Should().Be("SomeTable");
        }

        [TestMethod] public void RelationTable_NameIsUnchanged_Redundant() {
            // Arrange
            var translator = new Translator();
            var source = typeof(PacerTest);

            // Act
            var translation = translator[source];

            // Assert
            translation.Relations.Should().HaveCount(1);
            translation.Relations[0].Table.Should().HaveName("UT.Kvasir.Translation.PropertyTypes+PacerTest.LapsCompletedTable");
        }

        [TestMethod] public void RelationTable_NameChangedToNull_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Dwarf);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Dwarf.LifeEvents))                    // error location
                .WithMessageContaining("[RelationTable]")                           // details / explanation
                .WithMessageContaining("null");                                     // details / explanation
        }

        [TestMethod] public void RelationTable_NameChangedToEmptyString_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Rodent);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Rodent.Taxonomy))                     // error location
                .WithMessageContaining("[RelationTable]")                           // details / explanation
                .WithMessageContaining("\"\"");                                     // details / explanation
        }

        [TestMethod] public void RelationTable_DuplicateNameWithRelationTable_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Vowel);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining("Table name*is already in use")              // category
                .WithMessageContaining("\"AuxiliaryVowelTable\"");                  // details / explanation
        }

        [TestMethod] public void RelationTable_DuplicateNameWithPrimaryTable_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(VPN);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining("Table name*is already in use")              // category
                .WithMessageContaining("\"OfficialInfoVPN\"");                      // details / explanation
        }

        [TestMethod] public void RelationTable_AppliedToNumericField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Shofar);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Shofar.Tekiah))                       // error location
                .WithMessageContaining("[RelationTable]")                           // details / explanation
                .WithMessageContaining("is not a Relation");                        // details / explanation
        }

        [TestMethod] public void RelationTable_AppliedToTextualField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(LawnGnome);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(LawnGnome.Manufacturer))              // error location
                .WithMessageContaining("[RelationTable]")                           // details / explanation
                .WithMessageContaining("is not a Relation");                        // details / explanation
        }

        [TestMethod] public void RelationTable_AppliedToBooleanField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(GovernmentShutdown);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                     // source type
                .WithMessageContaining(nameof(GovernmentShutdown.RepublicansInCharge))  // error location
                .WithMessageContaining("[RelationTable]")                               // details / explanation
                .WithMessageContaining("is not a Relation");                            // details / explanation
        }

        [TestMethod] public void RelationTable_AppliedToDateTimeField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(CoalMine);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(CoalMine.LastCollapse))               // error location
                .WithMessageContaining("[RelationTable]")                           // details / explanation
                .WithMessageContaining("is not a Relation");                        // details / explanation
        }

        [TestMethod] public void RelationTable_AppliedToGuidField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(LawnMower);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(LawnMower.ApplianceID))               // error location
                .WithMessageContaining("[RelationTable]")                           // details / explanation
                .WithMessageContaining("is not a Relation");                        // details / explanation
        }

        [TestMethod] public void RelationTable_AppliedToEnumerationField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Triplets);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Triplets.Zygosity))                   // error location
                .WithMessageContaining("[RelationTable]")                           // details / explanation
                .WithMessageContaining("is not a Relation");                        // details / explanation
        }

        [TestMethod] public void RelationTable_AppliedToAggregateField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Toothbrush);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Toothbrush.Electric))                 // error location
                .WithMessageContaining("[RelationTable]")                           // details / explanation
                .WithMessageContaining("is not a Relation");                        // details / explanation
        }

        [TestMethod] public void RelationTable_AppliedToReferenceField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Valet);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Valet.Company))                       // error location
                .WithMessageContaining("[RelationTable]")                           // details / explanation
                .WithMessageContaining("is not a Relation");                        // details / explanation
        }
    }
}
