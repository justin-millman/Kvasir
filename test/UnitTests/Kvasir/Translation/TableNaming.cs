using FluentAssertions;
using Kvasir.Exceptions;
using Kvasir.Translation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using static UT.Kvasir.Translation.TableNaming;

namespace UT.Kvasir.Translation {
    [TestClass, TestCategory("Table Naming")]
    public class TableNamingTests {
        [TestMethod] public void TableRenamedToBrandNewName() {
            // Arrange
            var translator = new Translator();
            var source = typeof(PlayingCard);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Name.Should().Be("DeckOfCards");
        }

        [TestMethod] public void NamespaceExcludedFromDefaultTableName() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Pokemon);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Name.Should().Be("PokemonTable");
        }

        [TestMethod] public void Table_DuplicateName_IsError() {
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

        [TestMethod] public void Table_NameIsUnchanged_Redundant() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Bookmark);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Name.Should().Be("UT.Kvasir.Translation.TableNaming+BookmarkTable");
        }

        [TestMethod] public void Table_NameChangedToEmptyString_IsError() {
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
    }
}
