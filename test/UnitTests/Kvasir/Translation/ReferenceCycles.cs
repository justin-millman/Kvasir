using FluentAssertions;
using Kvasir.Exceptions;
using Kvasir.Translation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using static UT.Kvasir.Translation.ReferenceCycles;

namespace UT.Kvasir.Translation {
    [TestClass, TestCategory("ReferenceCycles")]
    public class ReferenceCycleTests {
        [TestMethod] public void SelfReferentialEntity_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Constitution);

            // Act
            var translate = () => translator[source];
            var cycle = "Constitution → Constitution";

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining("reference cycle detected")                  // category
                .WithMessageContaining(cycle);                                      // details / explanation
        }

        [TestMethod] public void EntityReferenceChainLength2_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Niqqud);

            // Act
            var translate = () => translator[source];
            var cycle = "Niqqud → Vowel → Niqqud";

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining("reference cycle detected")                  // category
                .WithMessageContaining(cycle);                                      // details / explanation
        }

        [TestMethod] public void EntityReferenceCycleLength3OrMore_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(RefugeeCamp);

            // Act
            var translate = () => translator[source];
            var cycle = "RefugeeCamp → Person → Country → CivilWar → RefugeeCamp";

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining("reference cycle detected")                  // category
                .WithMessageContaining(cycle);                                      // details / explanation
        }
    }
}
