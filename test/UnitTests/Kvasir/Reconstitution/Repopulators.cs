using Kvasir.Reconstitution;
using Kvasir.Relations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;

namespace UT.Kvasir.Reconstitution {
    [TestClass, TestCategory("DirectRepopulator")]
    public class DirectRepopulatorTests {
        [TestMethod] public void RepopulateZeroElements() {
            // Arrange
            var relation = Substitute.For<IRelation>();
            var elements = Array.Empty<object>();

            // Act
            var repopulator = new DirectRepopulator();
            repopulator.Repopulate(relation, elements);

            // Assert
            relation.DidNotReceive().Repopulate(Arg.Any<object>());
        }

        [TestMethod] public void RepopulateSingleElement() {
            // Arrange
            var relation = Substitute.For<IRelation>();
            var elements = new object[] { "Davao" };

            // ACt
            var repopulator = new DirectRepopulator();
            repopulator.Repopulate(relation, elements);

            // Assert
            relation.Received().Repopulate(elements[0]);
        }

        [TestMethod] public void RepopulateMultipleElements() {
            // Arrange
            var relation = Substitute.For<IRelation>();
            var elements = new object[] { 100, 101, 103, 106, 110, 115, 121, 128, 136, 145, 155, 166, 178, 191, 205 };

            // Act
            var repopulator = new DirectRepopulator();
            repopulator.Repopulate(relation, elements);

            // Assert
            Received.InOrder(() => {
                relation.Repopulate(elements[0]);
                relation.Repopulate(elements[1]);
                relation.Repopulate(elements[2]);
                relation.Repopulate(elements[3]);
                relation.Repopulate(elements[4]);
                relation.Repopulate(elements[5]);
                relation.Repopulate(elements[6]);
                relation.Repopulate(elements[7]);
                relation.Repopulate(elements[8]);
                relation.Repopulate(elements[9]);
                relation.Repopulate(elements[10]);
                relation.Repopulate(elements[11]);
                relation.Repopulate(elements[12]);
                relation.Repopulate(elements[13]);
            });
        }
    }
}
