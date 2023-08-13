using FluentAssertions;
using Kvasir.Extraction;
using Kvasir.Reconstitution;
using Kvasir.Relations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;

namespace UT.Kvasir.Reconstitution {
    [TestClass, TestCategory("FromPropertyRepopulator")]
    public class FromPropertyRepopulatorTests {
        [TestMethod] public void Construct() {
            // Arrange
            var type = typeof(Tuple<IReadOnlyRelationSet<int>>);
            var prop = type.GetProperty("Item1")!;
            var extractor = new IdentityExtractor<Tuple<IReadOnlyRelationSet<int>>>();

            // Act
            var repopulator = new FromPropertyRepopulator(extractor, prop);

            // Assert
            repopulator.ExpectedSubject.Should().Be(type);
        }

        [TestMethod] public void Execute() {
            // Arrange
            var mockRelation = Substitute.For<IRelation>();
            var source = new Tuple<IRelation>(mockRelation);
            var entries = new List<string>() { "Seneca Falls", "Richmond", "Hackensack", "Ogden", "Bloomington" };
            var extractor = new IdentityExtractor<Tuple<IRelation>>();
            var repopulator = new FromPropertyRepopulator(extractor, source.GetType().GetProperty("Item1")!);

            // Act
            repopulator.Execute(source, entries);

            // Assert
            Received.InOrder(() => {
                mockRelation.Repopulate(entries[0]);
                mockRelation.Repopulate(entries[1]);
                mockRelation.Repopulate(entries[2]);
                mockRelation.Repopulate(entries[3]);
                mockRelation.Repopulate(entries[4]);
            });
            mockRelation.ReceivedCalls().Should().HaveCount(5);
        }
    }
}
