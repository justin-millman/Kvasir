using FluentAssertions;
using Kvasir.Extraction;
using Kvasir.Reconstitution;
using Kvasir.Relations;
using Kvasir.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UT.Kvasir.Reconstitution {
    [TestClass, TestCategory("RelationRepopulationPlan")]
    public class RelationRepopulationPlanTests {
        [TestMethod] public void NullSource() {
            // Arrange
            var extractor = Substitute.For<ISingleExtractor>();
            extractor.SourceType.Returns(typeof(CallerIdentifier));
            extractor.ResultType.Returns(typeof(IRelation));
            var creator = Substitute.For<ICreator>();
            creator.ResultType.Returns(typeof(ushort));
            var reconstitutor = new DataReconstitutionPlan(creator, Array.Empty<IMutator>());
            var repopulator = Substitute.For<IRepopulator>();

            // Act
            var rows = new List<DBValue>[] { new() { DBValue.Create(100) } };
            var plan = new RelationRepopulationPlan(extractor, reconstitutor, repopulator);
            plan.Repopulate(null, rows);

            // Assert
            plan.SourceType.Should().Be(extractor.SourceType);
            extractor.DidNotReceive().ExtractFrom(Arg.Any<object?>());
            creator.DidNotReceive().CreateFrom(Arg.Any<IReadOnlyList<DBValue>>());
            repopulator.DidNotReceive().Repopulate(Arg.Any<IRelation>(), Arg.Any<IEnumerable<object>>());
        }

        [TestMethod] public void ZeroElements() {
            // Arrange
            var relation = Substitute.For<IRelation>();
            var extractor = Substitute.For<ISingleExtractor>();
            extractor.SourceType.Returns(typeof(Type));
            extractor.ResultType.Returns(typeof(IRelation));
            extractor.ExtractFrom(Arg.Any<object?>()).Returns(relation);
            var creator = Substitute.For<ICreator>();
            creator.ResultType.Returns(typeof(ushort));
            var reconstitutor = new DataReconstitutionPlan(creator, Array.Empty<IMutator>());
            var repopulator = Substitute.For<IRepopulator>();

            // Act
            var source = typeof(Type);
            var rows = Array.Empty<IReadOnlyList<DBValue>>();
            var plan = new RelationRepopulationPlan(extractor, reconstitutor, repopulator);
            plan.Repopulate(source, rows);

            // Assert
            plan.SourceType.Should().Be(extractor.SourceType);
            repopulator.DidNotReceive().Repopulate(Arg.Any<IRelation>(), Arg.Any<IEnumerable<object>>());
        }

        [TestMethod] public void OneElement() {
            // Arrange
            var elements = new object[] { "Basra" };
            var relation = Substitute.For<IRelation>();
            var extractor = Substitute.For<ISingleExtractor>();
            extractor.SourceType.Returns(typeof(Type));
            extractor.ResultType.Returns(typeof(IRelation));
            extractor.ExtractFrom(Arg.Any<object?>()).Returns(relation);
            var creator = Substitute.For<ICreator>();
            creator.ResultType.Returns(typeof(ushort));
            creator.CreateFrom(Arg.Any<IReadOnlyList<DBValue>>()).Returns(elements[0]);
            var reconstitutor = new DataReconstitutionPlan(creator, Array.Empty<IMutator>());
            var repopulator = Substitute.For<IRepopulator>();

            // Act
            var source = typeof(Type);
            var rows = new List<DBValue>[] { new() { DBValue.Create(100) } };
            var plan = new RelationRepopulationPlan(extractor, reconstitutor, repopulator);
            plan.Repopulate(source, rows);

            // Assert
            plan.SourceType.Should().Be(extractor.SourceType);
            repopulator.Received().Repopulate(relation, Arg.Is<IEnumerable<object>>(e => e.SequenceEqual(elements)));
        }

        [TestMethod] public void MultipleElements() {
            // Arrange
            var elements = new object[] { '8', '%', 'U', '\t', '|' };
            var relation = Substitute.For<IRelation>();
            var extractor = Substitute.For<ISingleExtractor>();
            extractor.SourceType.Returns(typeof(Type));
            extractor.ResultType.Returns(typeof(IRelation));
            extractor.ExtractFrom(Arg.Any<object?>()).Returns(relation);
            var creator = Substitute.For<ICreator>();
            creator.ResultType.Returns(typeof(ushort));
            creator.CreateFrom(Arg.Any<IReadOnlyList<DBValue>>()).Returns(elements[0], elements[1..]);
            var reconstitutor = new DataReconstitutionPlan(creator, Array.Empty<IMutator>());
            var repopulator = Substitute.For<IRepopulator>();

            // Act
            var source = typeof(Type);
            var rows = Enumerable.Repeat(new List<DBValue>() { DBValue.Create(0UL) }, 5);
            var plan = new RelationRepopulationPlan(extractor, reconstitutor, repopulator);
            plan.Repopulate(source, rows);

            // Assert
            plan.SourceType.Should().Be(extractor.SourceType);
            repopulator.Received().Repopulate(relation, Arg.Is<IEnumerable<object>>(e => e.SequenceEqual(elements)));
        }
    }
}
