using FluentAssertions;
using Kvasir.Exceptions;
using Kvasir.Translation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using static UT.Kvasir.Translation.TestComponents;

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
                .HaveCandidateKey("SSN").And
                .HaveCandidateKey("FullName").And
                .NoOtherCandidateKeys();
        }

        [TestMethod] public void NamedCandidateKey() {
            // Arrange
            var translator = new Translator();
            var source = typeof(BowlGame);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveCandidateKey("PrimarySponsor", "SecondarySponsor").WithName("Sponsorship").And
                .NoOtherCandidateKeys();
        }

        [TestMethod] public void SingleFieldInMultipleCandidateKeys() {
            // Arrange
            var translator = new Translator();
            var source = typeof(KingOfEngland);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveCandidateKey("RegnalName", "RegnalNumber").WithName("Uno").And
                .HaveCandidateKey("RegnalName", "RoyalHouse").WithName("Another").And
                .HaveCandidateKey("RegnalNumber", "RoyalHouse").WithName("Third").And
                .NoOtherCandidateKeys();
        }

        [TestMethod] public void DuplicateNamedCandidateKeys_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Check);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                        // source type
                .WithMessage("*Candidate Key*")                         // categorization
                .WithMessage("*comprised*same*Fields*")                 // rationale
                .WithMessage("*N2*N1*");                                // details
        }

        [TestMethod] public void DuplicateAnonymousCandidateKeys_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Pigment);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                        // source type
                .WithMessage($"*{nameof(Pigment.ChemicalFormula)}*")    // source property
                .WithMessage("*[Unique]*")                              // annotation
                .WithMessage("*two or more*");                          // rationale
        }

        [TestMethod] public void FieldInSameCandidateKeyMultipleTimes_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Desert);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                        // source type
                .WithMessage($"*{nameof(Desert.Length)}*")              // source property
                .WithMessage("*[Unique]*")                              // annotation
                .WithMessage("*two or more*")                           // rationale
                .WithMessage("*Size*");                                 // details
        }

        [TestMethod] public void InvalidCandidateKeyName_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Allomancy);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                        // source type
                .WithMessage($"*{nameof(Allomancy.MistingTerm)}*")      // source property
                .WithMessage("*[Unique]*")                              // annotation
                .WithMessage("*not a valid Candidate Key name*")        // rationale
                .WithMessage("*\"\"*");                                 // details
        }

        [TestMethod] public void ReservedCandidateKeyName_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Lens);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                        // source type
                .WithMessage($"*{nameof(Lens.IndexOfRefraction)}*")     // source property
                .WithMessage("*[Unique]*")                              // annotation
                .WithMessage("*reserved character sequence @@@*")       // rationale
                .WithMessage("*@@@Key*");                               // details
        }

        [TestMethod] public void PathOnUniqueAnnotationForScalar_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Sonnet);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                        // source type
                .WithMessage($"*{nameof(Sonnet.Line1)}*")               // source property
                .WithMessage("*[Unique]*")                              // annotation
                .WithMessage("*path*does not exist*")                   // rationale
                .WithMessage("*\"---\"*");                              // details
        }

        [TestMethod] public void CandidateKeyIsSupersetOfPrimaryKey_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(WorldHeritageSite);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                                // source type
                .WithMessage("*Candidate Key*is a superset of*Primary Key*")    // rationale
                .WithMessage("*\"X\"*");                                        // details
        }
    }
}