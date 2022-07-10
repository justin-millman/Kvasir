using Atropos.Moq;
using FluentAssertions;
using Kvasir.Reconstitution;
using Kvasir.Relations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;

namespace UT.Kvasir.Reconstitution {
    [TestClass, TestCategory("FromPropertyRepopulator")]
    public class FromPropertyRepopulatorTests {
        [TestMethod] public void Construct() {
            // Arrange
            var type = typeof(Tuple<IReadOnlyRelationSet<int>>);
            var prop = type.GetProperty("Item1")!;

            // Act
            var repopulator = new FromPropertyRepopulator(prop);

            // Assert
            repopulator.ExpectedSubject.Should().Be(type);
        }

        [TestMethod] public void Execute() {
            // Arrange
            var mockRelation = new Mock<IRelation>();
            var source = new Tuple<IRelation>(mockRelation.Object);
            var entries = new List<string>() { "Seneca Falls", "Richmond", "Hackensack", "Ogden" };
            var repopulator = new FromPropertyRepopulator(source.GetType().GetProperty("Item1")!);

            // Sequence
            var sequence = mockRelation.MakeSequence();
            sequence.Add(r => r.Repopulate(entries[0]));
            sequence.Add(r => r.Repopulate(entries[1]));
            sequence.Add(r => r.Repopulate(entries[2]));
            sequence.Add(r => r.Repopulate(entries[3]));

            // Act
            repopulator.Execute(source, entries);

            // Assert
            sequence.VerifyCompleted();
            mockRelation.VerifyNoOtherCalls();
        }
    }
}
