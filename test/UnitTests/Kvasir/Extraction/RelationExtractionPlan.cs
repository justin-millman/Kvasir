using FluentAssertions;
using Kvasir.Extraction;
using Kvasir.Relations;
using Kvasir.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;

namespace UT.Kvasir.Extraction {
    [TestClass, TestCategory("RelationExtractionPlan")]
    public class RelationExtractionPlanTests {
        [TestMethod] public void NullRelation() {
            // Arrange
            var relationExtractor = Substitute.For<ISingleExtractor>();
            relationExtractor.SourceType.Returns(typeof(GCGenerationInfo));
            relationExtractor.ResultType.Returns(typeof(IReadOnlyRelationList<int>));
            relationExtractor.ExtractFrom(Arg.Any<GCGenerationInfo>()).Returns(null);
            var elementPlan = MakePlanFor<int>();
            var source = new GCGenerationInfo();

            // Act
            var plan = new RelationExtractionPlan(relationExtractor, elementPlan);
            (var inserts, var modifies, var deletes) = plan.ExtractFrom(source);

            // Assert
            inserts.Should().BeEmpty();
            modifies.Should().BeEmpty();
            deletes.Should().BeEmpty();
        }

        [TestMethod] public void EmptyRelation() {
            // Arrange
            var relation = Substitute.For<IRelation>();
            var relationExtractor = Substitute.For<ISingleExtractor>();
            relationExtractor.SourceType.Returns(typeof(Predicate<char>));
            relationExtractor.ResultType.Returns(typeof(RelationSet<short>));
            relationExtractor.ExtractFrom(Arg.Any<Predicate<char>>()).Returns(relation);
            var elementPlan = MakePlanFor<short>();
            var source = new Predicate<char>(c => char.IsDigit(c));

            // Act
            var plan = new RelationExtractionPlan(relationExtractor, elementPlan);
            (var inserts, var modifies, var deletes) = plan.ExtractFrom(source);

            // Assert
            inserts.Should().BeEmpty();
            modifies.Should().BeEmpty();
            deletes.Should().BeEmpty();
        }

        [TestMethod] public void OnlyNewElements() {
            // Arrange
            var elements = new List<(object, Status)>() {
                new("Poznań", Status.New),
                new("Timișoara", Status.New),
                new("Douala", Status.New)
            };
            var relation = Substitute.For<IRelation>();
            relation.GetEnumerator().Returns(elements.GetEnumerator());
            var relationExtractor = Substitute.For<ISingleExtractor>();
            relationExtractor.SourceType.Returns(typeof(List<string>));
            relationExtractor.ResultType.Returns(typeof(IRelation));
            relationExtractor.ExtractFrom(Arg.Any<List<string>>()).Returns(relation);
            var elementPlan = MakePlanFor<string>();
            var source = new List<string>();

            // Act
            var plan = new RelationExtractionPlan(relationExtractor, elementPlan);
            (var inserts, var modifies, var deletes) = plan.ExtractFrom(source);

            // Assert
            inserts.Should().HaveCount(3);
            inserts.Should().ContainEquivalentOf(new DBValue[] { DBValue.Create(elements[0].Item1) });
            inserts.Should().ContainEquivalentOf(new DBValue[] { DBValue.Create(elements[1].Item1) });
            inserts.Should().ContainEquivalentOf(new DBValue[] { DBValue.Create(elements[2].Item1) });
            modifies.Should().BeEmpty();
            deletes.Should().BeEmpty();
        }

        [TestMethod] public void OnlySavedElements() {
            // Arrange
            var elements = new List<(object, Status)>() {
                new('q', Status.Saved), new('&', Status.Saved), new('\'', Status.Saved),
                new('H', Status.Saved), new(' ', Status.Saved), new('\a', Status.Saved)
            };
            var relation = Substitute.For<IRelation>();
            relation.GetEnumerator().Returns(elements.GetEnumerator());
            var relationExtractor = Substitute.For<ISingleExtractor>();
            relationExtractor.SourceType.Returns(typeof(EventHandler));
            relationExtractor.ResultType.Returns(typeof(IReadOnlyRelationMap<string, DateTime>));
            relationExtractor.ExtractFrom(Arg.Any<List<EventHandler>>()).Returns(relation);
            var elementPlan = MakePlanFor<char>();
            var source = new EventHandler((o, a) => {});

            // Act
            var plan = new RelationExtractionPlan(relationExtractor, elementPlan);
            (var inserts, var modifies, var deletes) = plan.ExtractFrom(source);

            // Assert
            inserts.Should().BeEmpty();
            modifies.Should().BeEmpty();
            deletes.Should().BeEmpty();
        }

        [TestMethod] public void OnlyModifiedElements() {
            // Arrange
            var elements = new List<(object, Status)>() {
                new("Birmingham", Status.Modified),
                new("Praia", Status.Modified),
                new("Kraków", Status.Modified),
                new("Stuttgart", Status.Modified),
                new("Giza", Status.Modified)
            };
            var relation = Substitute.For<IRelation>();
            relation.GetEnumerator().Returns(elements.GetEnumerator());
            var relationExtractor = Substitute.For<ISingleExtractor>();
            relationExtractor.SourceType.Returns(typeof(ValueType));
            relationExtractor.ResultType.Returns(typeof(RelationOrderedList<string>));
            relationExtractor.ExtractFrom(Arg.Any<ValueType>()).Returns(relation);
            var elementPlan = MakePlanFor<string>();
            var source = Substitute.For<ValueType>();

            // Act
            var plan = new RelationExtractionPlan(relationExtractor, elementPlan);
            (var inserts, var modifies, var deletes) = plan.ExtractFrom(source);

            // Assert
            inserts.Should().BeEmpty();
            modifies.Should().HaveCount(5);
            modifies.Should().ContainEquivalentOf(new DBValue[] { DBValue.Create(elements[0].Item1) });
            modifies.Should().ContainEquivalentOf(new DBValue[] { DBValue.Create(elements[1].Item1) });
            modifies.Should().ContainEquivalentOf(new DBValue[] { DBValue.Create(elements[2].Item1) });
            modifies.Should().ContainEquivalentOf(new DBValue[] { DBValue.Create(elements[3].Item1) });
            modifies.Should().ContainEquivalentOf(new DBValue[] { DBValue.Create(elements[4].Item1) });
            deletes.Should().BeEmpty();
        }

        [TestMethod] public void OnlyDeletedElements() {
            // Arrange
            var elements = new List<(object, Status)>() {
                new("Sapporo", Status.Deleted),
                new("Mombasa", Status.Deleted),
                new("Mariupol", Status.Deleted),
                new("Faiyum", Status.Deleted)
            };
            var relation = Substitute.For<IRelation>();
            relation.GetEnumerator().Returns(elements.GetEnumerator());
            var relationExtractor = Substitute.For<ISingleExtractor>();
            relationExtractor.SourceType.Returns(typeof(object));
            relationExtractor.ResultType.Returns(typeof(IRelation));
            relationExtractor.ExtractFrom(Arg.Any<object>()).Returns(relation);
            var elementPlan = MakePlanFor<string>();
            object source = 100000;

            // Act
            var plan = new RelationExtractionPlan(relationExtractor, elementPlan);
            (var inserts, var modifies, var deletes) = plan.ExtractFrom(source);

            // Assert
            inserts.Should().BeEmpty();
            modifies.Should().BeEmpty();
            deletes.Should().HaveCount(4);
            deletes.Should().ContainEquivalentOf(new DBValue[] { DBValue.Create(elements[0].Item1) });
            deletes.Should().ContainEquivalentOf(new DBValue[] { DBValue.Create(elements[1].Item1) });
            deletes.Should().ContainEquivalentOf(new DBValue[] { DBValue.Create(elements[2].Item1) });
            deletes.Should().ContainEquivalentOf(new DBValue[] { DBValue.Create(elements[3].Item1) });
        }

        [TestMethod] public void MixedStatusElements() {
            // Arrange
            var elements = new List<(object, Status)>() {
                new("Medellín", Status.Deleted),
                new("Albany", Status.Deleted),
                new("Juárez", Status.Deleted),
                new("Grand Rapids", Status.Saved),
                new("Düsseldorf", Status.New),
                new("Tarawa", Status.New),
                new("Naypyidaw", Status.Modified),
                new("Walla Walla", Status.Saved),
                new("Oconomowoc", Status.Modified),
                new("Leeds", Status.New)
            };
            var relation = Substitute.For<IRelation>();
            relation.GetEnumerator().Returns(elements.GetEnumerator());
            var relationExtractor = Substitute.For<ISingleExtractor>();
            relationExtractor.SourceType.Returns(typeof(Memory<char>));
            relationExtractor.ResultType.Returns(typeof(RelationList<int>));
            relationExtractor.ExtractFrom(Arg.Any<Memory<char>>()).Returns(relation);
            var elementPlan = MakePlanFor<string>();
            var source = new Memory<char>();

            // Act
            var plan = new RelationExtractionPlan(relationExtractor, elementPlan);
            (var inserts, var modifies, var deletes) = plan.ExtractFrom(source);

            // Assert
            inserts.Should().HaveCount(3);
            inserts.Should().ContainEquivalentOf(new DBValue[] { DBValue.Create(elements[4].Item1) });
            inserts.Should().ContainEquivalentOf(new DBValue[] { DBValue.Create(elements[5].Item1) });
            inserts.Should().ContainEquivalentOf(new DBValue[] { DBValue.Create(elements[9].Item1) });
            modifies.Should().HaveCount(2);
            modifies.Should().ContainEquivalentOf(new DBValue[] { DBValue.Create(elements[6].Item1) });
            modifies.Should().ContainEquivalentOf(new DBValue[] { DBValue.Create(elements[8].Item1) });
            deletes.Should().HaveCount(3);
            deletes.Should().ContainEquivalentOf(new DBValue[] { DBValue.Create(elements[0].Item1) });
            deletes.Should().ContainEquivalentOf(new DBValue[] { DBValue.Create(elements[1].Item1) });
            deletes.Should().ContainEquivalentOf(new DBValue[] { DBValue.Create(elements[2].Item1) });
        }


        private static DataExtractionPlan MakePlanFor<T>() {
            var identity = Substitute.For<IMultiExtractor>();
            identity.SourceType.Returns(typeof(T));
            identity.ExtractFrom(Arg.Any<T>()).Returns(args => new object?[] { args[0] });
            return new DataExtractionPlan(new IMultiExtractor[] { identity });
        }
    }
}
