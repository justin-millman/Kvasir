using FluentAssertions;
using Kvasir.Exceptions;
using Kvasir.Schema;
using Kvasir.Translation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using static UT.Kvasir.Translation.TestComponents;

namespace UT.Kvasir.Translation {
    [TestClass, TestCategory("Naming")]
    public class NamingTests {
        [TestMethod] public void OddlyShapedFieldNames() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Surah);

            // Act
            var translation = translator[source];

            // Assert
            translation.CLRSource.Should().Be(source);
            translation.Principal.Table.Should()
                .HaveField("_EnglishName", DBType.Text, IsNullable.No).And
                .HaveField("__ArabicName", DBType.Text, IsNullable.No).And
                .HaveField("juz_start", DBType.Decimal, IsNullable.No).And
                .HaveField("juzEnd", DBType.Decimal, IsNullable.No).And
                .NoOtherFields();
        }

        [TestMethod] public void RenamePrimaryTable() {
            // Arrange
            var translator = new Translator();
            var source = typeof(PlayingCard);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should().HaveName("DeckOfCards");
        }

        [TestMethod] public void NamespaceExcludedFromPrimaryTableName() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Pokemon);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should().HaveName("PokemonTable");
        }

        [TestMethod] public void DuplicatePrimaryTableName_IsError() {
            // Arrange
            var translator = new Translator();
            var source1 = typeof(Flight);
            var source2 = typeof(Battle);

            // Act
            _ = translator[source1];
            var act = () => translator[source2];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source2.Name}*")                       // source type
                .WithMessage("*Table name*is already in use*")          // rationale
                .WithMessage("*\"Miscellaneous\"*");                    // details
        }

        [TestMethod] public void TableAnnotationDoesntChangeName_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Bookmark);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                        // source type
                .WithMessage("*[Table]*")                               // annotation
                .WithMessage("*redundant*");                            // rationale
        }

        [TestMethod] public void InvalidPrimaryTableName_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(LogIn);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                        // source type
                .WithMessage("*[Table]*")                               // annotation
                .WithMessage("*not a valid Table name*")                // rationale
                .WithMessage("*\"\"*");                                 // details
        }

        [TestMethod] public void BothTableAndNamespaceExcludeAnnotations_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Encryption);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                        // source type
                .WithMessage("*[ExcludeNamespaceFromName]*[Table]*")    // annotation
                .WithMessage("*both*");                                 // rationale
        }

        [TestMethod] public void RenameFields() {
            // Arrange
            var translator = new Translator();
            var source = typeof(River);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField("Name", DBType.Text, IsNullable.No).And
                .HaveField("SourceElevation", DBType.UInt16, IsNullable.No).And
                .HaveField("Length", DBType.UInt16, IsNullable.No).And
                .HaveField("MouthLatitude", DBType.Decimal, IsNullable.No).And
                .HaveField("MouthLongitude", DBType.Decimal, IsNullable.No).And
                .NoOtherFields();
        }

        [TestMethod] public void SwitchNamesOfFields() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Episode);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField("Number", DBType.UInt8, IsNullable.No).And
                .HaveField("Season", DBType.Int16, IsNullable.No).And
                .HaveField("Length", DBType.Single, IsNullable.No).And
                .HaveField("Part", DBType.Int32, IsNullable.Yes).And
                .HaveField("Title", DBType.Text, IsNullable.No).And
                .NoOtherFields();
        }

        [TestMethod] public void DuplicateFieldName_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Ticket2RideRoute);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                                // source type
                .WithMessage("*duplicate Field name*")                          // rationale
                .WithMessage("*\"Destination\"*");                              // details
        }

        [TestMethod] public void SinglePropertyGivenMultipleNames_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(BankAccount);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                            // source type
                .WithMessage($"*{nameof(BankAccount.RoutingNumber)}*")      // source property
                .WithMessage("*[Name]*")                                    // annotation
                .WithMessage("*multiple*");                                 // rationale
        }

        [TestMethod] public void NameAnnotationDoesntChangeName_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Opera);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                            // source type
                .WithMessage($"*{nameof(Opera.PremiereDate)}*")             // source property
                .WithMessage("*[Name]*")                                    // annotation
                .WithMessage("*redundant*");                                // rationale
        }

        [TestMethod] public void InvalidFieldName_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Volcano);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                        // source type
                .WithMessage($"*{nameof(Volcano.IsActive)}*")           // source property
                .WithMessage("*[Name]*")                                // annotation
                .WithMessage("*not a valid Field name*")                // rationale
                .WithMessage("*\"\"*");                                 // details
        }

        [TestMethod] public void PathOnFieldNameAnnotationForScalar_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Legume);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                        // source type
                .WithMessage($"*{nameof(Legume.Energy)}*")              // source property
                .WithMessage("*[Name]*")                                // annotation
                .WithMessage("*path*does not exist*")                   // rationale
                .WithMessage("*\"---\"*");                              // details
        }
    }
}
