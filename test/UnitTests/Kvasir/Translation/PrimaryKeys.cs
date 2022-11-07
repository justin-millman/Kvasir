using FluentAssertions;
using Kvasir.Exceptions;
using Kvasir.Translation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using static UT.Kvasir.Translation.TestComponents;

namespace UT.Kvasir.Translation {
    [TestClass, TestCategory("Primary Keys")]
    public class PrimaryKeyTests {
        [TestMethod] public void AnnotatedPrimaryKeySingleField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(XKCDComic);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should().HavePrimaryKey("URL").WithoutName();
        }

        [TestMethod] public void AnnotatedPrimaryKeyMultipleFields() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Month);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should().HavePrimaryKey("Calendar", "Index").WithoutName();
        }

        [TestMethod] public void AnnotatedPrimaryKeyAllFields() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Character);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should().HavePrimaryKey("Glyph", "CodePoint", "IsASCII").WithoutName();
        }

        [TestMethod] public void DeducedPrimaryKeyNamedID() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Actor);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should().HavePrimaryKey("ID").WithoutName();
        }

        [TestMethod] public void DeducedPrimaryKeyRenamedToID() {
            // Arrange
            var translator = new Translator();
            var source = typeof(PokerHand);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should().HavePrimaryKey("ID").WithoutName();
        }

        [TestMethod] public void DeducedPrimaryKeyNamedEntityID() {
            // Arrange
            var translator = new Translator();
            var source = typeof(IntegerSequence);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should().HavePrimaryKey("IntegerSequenceID").WithoutName();
        }

        [TestMethod] public void DeducedPrimaryKeyRenamedToEntityID() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Stadium);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should().HavePrimaryKey("StadiumID").WithoutName();
        }

        [TestMethod] public void DeducedPrimaryKeyArbitrateBetweenEntityIDandTableID() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Function);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should().HavePrimaryKey("FunctionID").WithoutName();
        }

        [TestMethod] public void DeducedPrimaryKeySingleCandidateSingleNonNullableField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Star);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should().HavePrimaryKey("ARICNS");
            translation.Principal.Table.CandidateKeys.Should().BeEmpty();
        }

        [TestMethod] public void DeducedPrimaryKeySingleCandidateMultipleNonNullableFields() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Expiration);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HavePrimaryKey("FeedCode", "Underlying")
                .WithName("PK");
            translation.Principal.Table.CandidateKeys.Should().BeEmpty();
        }

        [TestMethod] public void DeducedPrimaryKeyMultipleCandidateKeysOnlyOneAllNonNullable() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Vitamin);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HavePrimaryKey("Carbon", "Hydrogen", "Cobalt", "Nitrogen", "Oxygen", "Phosphorus")
                .WithName("Formula");
            translation.Principal.Table.CandidateKeys.Count.Should().Be(1);
        }

        [TestMethod] public void DeducedPrimaryKeySingleNonNullableField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Earthquake);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should().HavePrimaryKey("SeismicIdentificationNumber").WithoutName();
        }

        [TestMethod] public void DeducedPrimaryKeySingleAnnotatedNonNullableField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(GeologicEpoch);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should().HavePrimaryKey("StartingMYA").WithoutName();
        }

        [TestMethod] public void NonDeduciblePrimaryKey_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(FederalLaw);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                        // source type
                .WithMessage("*unable to deduce Primary Key*");         // rationale
        }

        [TestMethod] public void NullableFieldAnnotatedPrimaryKey_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(NorseWorld);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                        // source type
                .WithMessage($"*{nameof(NorseWorld.EddaMentions)}*")    // source property
                .WithMessage("*[PrimaryKey]*")                          // annotation
                .WithMessage("*nullable*");                             // rationale
        }

        [TestMethod] public void DeducedPrimaryKeySkipNullableFieldNamedID() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Rollercoaster);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should().HavePrimaryKey("RollercoasterID").WithoutName();
        }

        [TestMethod] public void DeducedPrimaryKeySkipNullableFieldNamedEntityID() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Doctor);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HavePrimaryKey("Regeneration", "Portrayal")
                .WithName("DoctorWho");
        }

        [TestMethod] public void DeducedPrimaryKeySkipCandidateKeyWithNullableField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Polyhedron);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should().HavePrimaryKey("Name").WithoutName();
        }

        [TestMethod] public void NamedPrimaryKey() {
            // Arrange
            var translator = new Translator();
            var source = typeof(HebrewLetter);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HavePrimaryKey("Letter")
                .WithName("LetterPK");
        }

        [TestMethod] public void NamedPrimaryKeyOverridesUnnamedCandidateKeyName() {
            // Arrange
            var translator = new Translator();
            var source = typeof(DatabaseField);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HavePrimaryKey("QualifiedName")
                .WithName("PrimaryKey");
            translation.Principal.Table.CandidateKeys.Should().BeEmpty();
        }

        [TestMethod] public void InvalidPrimaryKeyName_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Bay);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                        // source type
                .WithMessage("*[NamedPrimaryKey]*")                     // annotation
                .WithMessage("*not a valid Primary Key name*")          // rationale
                .WithMessage("*\"\"*");                                 // details
        }

        [TestMethod] public void NamedPrimaryKeyConflictsWithNameOfCandidateKeyPK_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(JigsawPuzzle);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                                        // source type
                .WithMessage("*[NamedPrimaryKey]*")                                     // annotation
                .WithMessage("*Candidate Key*deduced as*Primary Key*conflicting*")      // rationale
                .WithMessage("*\"GlobalIdentifier\"*\"Something\"*");                   // details
        }

        [TestMethod] public void NamedPrimaryKeyRedundantWithNameOfCandidateKeyPK_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(TimeZone);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                                        // source type
                .WithMessage("*[NamedPrimaryKey]*")                                     // annotation
                .WithMessage("*Candidate Key*deduced as*Primary Key*redundant*")        // rationale
                .WithMessage("*\"PK\"*");                                               // details
        }

        [TestMethod] public void NamedPrimaryKeyInUseByCandidateKeyNotPK_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Currency);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                        // source type
                .WithMessage("*[NamedPrimaryKey]*")                     // annotation
                .WithMessage("*clashes*name of*Candidate Key*")         // rationale
                .WithMessage("*\"Key13\"*");                            // details
        }

        [TestMethod] public void AnnotatedSolePrimaryKeyFieldAlsoUnnamedCandidateKey_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Waterfall);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                                            // source type
                .WithMessage("*Candidate Key*is a superset of*Primary Key*")                // rationale
                .WithMessage($"*{nameof(Waterfall.InternationalUnifiedWaterfallNumber)}*"); // details
        }

        [TestMethod] public void MultiplePrimaryKeyAnnotations_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Airport);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                            // source type
                .WithMessage($"*{nameof(Airport.IATA)}*")                   // source property
                .WithMessage("*[PrimaryKey]*")                              // annotation
                .WithMessage("*multiple*");                                 // rationale
        }

        [TestMethod] public void PathOnPrimaryKeyAnnotationForScalar_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Highway);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                        // source type
                .WithMessage($"*{nameof(Highway.Number)}*")             // source property
                .WithMessage("*[PrimaryKey]*")                          // annotation
                .WithMessage("*path*does not exist*")                   // rationale
                .WithMessage("*\"---\"*");                              // details
        }
    }
}
