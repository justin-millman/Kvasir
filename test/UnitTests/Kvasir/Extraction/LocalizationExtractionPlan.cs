using FluentAssertions;
using Kvasir.Extraction;
using Kvasir.Relations;
using Kvasir.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;

namespace UT.Kvasir.Extraction {
    [TestClass, TestCategory("LocalizationExtractionPlan")]
    public class LocalizationExtractionPlanTests {
        [TestMethod] public void NullLocalization() {
            // Arrange
            var relationExtractor = Substitute.For<ISingleExtractor>();
            relationExtractor.SourceType.Returns(typeof(FakeLocalization));
            relationExtractor.ResultType.Returns(typeof(IReadOnlyRelationMap<double, char>));
            relationExtractor.ExtractFrom(Arg.Any<FakeLocalization>()).Returns(null);
            var elementPlan = MakePlanFor<double>();
            var relationPlan = new RelationExtractionPlan(relationExtractor, elementPlan);
            var source = new FakeLocalization("Punta Cana");

            // Act
            var plan = new LocalizationExtractionPlan(relationPlan);
            (var inserts, var modifies, var deletes) = plan.ExtractFrom(source);
            var canonicalization = () => plan.Canonicalize(source);

            // Assert
            inserts.Should().BeEmpty();
            modifies.Should().BeEmpty();
            deletes.Should().BeEmpty();
            canonicalization.Should().NotThrow();
        }

        [TestMethod] public void NoLocalizedValues() {
            // Arrange
            var relation = Substitute.For<IRelation>();
            var relationExtractor = Substitute.For<ISingleExtractor>();
            relationExtractor.SourceType.Returns(typeof(FakeLocalization));
            relationExtractor.ResultType.Returns(typeof(RelationSet<string>));
            relationExtractor.ExtractFrom(Arg.Any<FakeLocalization>()).Returns(relation);
            var elementPlan = MakePlanFor<string>();
            var relationPlan = new RelationExtractionPlan(relationExtractor, elementPlan);
            var source = new FakeLocalization("Lhasa");

            // Act
            var plan = new LocalizationExtractionPlan(relationPlan);
            (var inserts, var modifies, var deletes) = plan.ExtractFrom(source);
            plan.Canonicalize(source);

            // Assert
            inserts.Should().BeEmpty();
            modifies.Should().BeEmpty();
            deletes.Should().BeEmpty();
            relation.Received(1).Canonicalize();
        }

        [TestMethod] public void OnlyNewLocalizedValues() {
            // Arrange
            var elements = new List<(object, Status)>() {
                new("Delft", Status.New),
                new("Lourdes", Status.New)
            };
            var relation = Substitute.For<IRelation>();
            relation.GetEnumerator().Returns(elements.GetEnumerator());
            var relationExtractor = Substitute.For<ISingleExtractor>();
            relationExtractor.SourceType.Returns(typeof(FakeLocalization));
            relationExtractor.ResultType.Returns(typeof(IReadOnlyRelationList<Guid>));
            relationExtractor.ExtractFrom(Arg.Any<FakeLocalization>()).Returns(relation);
            var elementPlan = MakePlanFor<string>();
            var relationPlan = new RelationExtractionPlan(relationExtractor, elementPlan);
            var source = new FakeLocalization("Zunyi");

            // Act
            var plan = new LocalizationExtractionPlan(relationPlan);
            (var inserts, var modifies, var deletes) = plan.ExtractFrom(source);
            plan.Canonicalize(source);

            // Assert
            inserts.Should().HaveCount(2);
            inserts.Should().ContainEquivalentOf(new DBValue[] { DBValue.Create(source.Key), DBValue.Create(elements[0].Item1) });
            inserts.Should().ContainEquivalentOf(new DBValue[] { DBValue.Create(source.Key), DBValue.Create(elements[1].Item1) });
            modifies.Should().BeEmpty();
            deletes.Should().BeEmpty();
            relation.Received(1).Canonicalize();
        }

        [TestMethod] public void OnlySavedLocalizedValues() {
            // Arrange
            var elements = new List<(object, Status)>() {
                new('_', Status.Saved), new('?', Status.Saved), new('6', Status.Saved), new('h', Status.Saved),
                new('V', Status.Saved), new('.', Status.Saved), new('}', Status.Saved), new('~', Status.Saved),
                new('%', Status.Saved)
            };
            var relation = Substitute.For<IRelation>();
            relation.GetEnumerator().Returns(elements.GetEnumerator());
            var relationExtractor = Substitute.For<ISingleExtractor>();
            relationExtractor.SourceType.Returns(typeof(FakeLocalization));
            relationExtractor.ResultType.Returns(typeof(RelationMap<DateTime, ushort>));
            relationExtractor.ExtractFrom(Arg.Any<FakeLocalization>()).Returns(relation);
            var elementPlan = MakePlanFor<string>();
            var relationPlan = new RelationExtractionPlan(relationExtractor, elementPlan);
            var source = new FakeLocalization("Agra");

            // Act
            var plan = new LocalizationExtractionPlan(relationPlan);
            (var inserts, var modifies, var deletes) = plan.ExtractFrom(source);
            plan.Canonicalize(source);

            // Assert
            inserts.Should().BeEmpty();
            inserts.Should().BeEmpty();
            modifies.Should().BeEmpty();
            deletes.Should().BeEmpty();
            relation.Received(1).Canonicalize();
        }

        [TestMethod] public void OnlyModifiedLocalizedValues() {
            // Arrange
            var elements = new List<(object, Status)>() {
                new("Ashdod", Status.Modified),
                new("Samarinda", Status.Modified),
                new("Nogales", Status.Modified),
                new("Lusail", Status.Modified)
            };
            var relation = Substitute.For<IRelation>();
            relation.GetEnumerator().Returns(elements.GetEnumerator());
            var relationExtractor = Substitute.For<ISingleExtractor>();
            relationExtractor.SourceType.Returns(typeof(FakeLocalization));
            relationExtractor.ResultType.Returns(typeof(RelationOrderedList<sbyte>));
            relationExtractor.ExtractFrom(Arg.Any<FakeLocalization>()).Returns(relation);
            var elementPlan = MakePlanFor<string>();
            var relationPlan = new RelationExtractionPlan(relationExtractor, elementPlan);
            var source = new FakeLocalization("Cagliari");

            // Act
            var plan = new LocalizationExtractionPlan(relationPlan);
            (var inserts, var modifies, var deletes) = plan.ExtractFrom(source);
            plan.Canonicalize(source);

            // Assert
            inserts.Should().BeEmpty();
            modifies.Should().HaveCount(4);
            modifies.Should().ContainEquivalentOf(new DBValue[] { DBValue.Create(source.Key), DBValue.Create(elements[0].Item1) });
            modifies.Should().ContainEquivalentOf(new DBValue[] { DBValue.Create(source.Key), DBValue.Create(elements[1].Item1) });
            modifies.Should().ContainEquivalentOf(new DBValue[] { DBValue.Create(source.Key), DBValue.Create(elements[2].Item1) });
            modifies.Should().ContainEquivalentOf(new DBValue[] { DBValue.Create(source.Key), DBValue.Create(elements[3].Item1) });
            deletes.Should().BeEmpty();
            relation.Received(1).Canonicalize();
        }

        [TestMethod] public void OnlyDeletedLocalizedValues() {
            // Arrange
            var elements = new List<(object, Status)>() {
                new("Ayacucho", Status.Deleted),
                new("Pernambuco", Status.Deleted),
                new("Atafu", Status.Deleted),
                new("Scornicești", Status.Deleted),
                new("Cholula", Status.Deleted),
                new("Akranes", Status.Deleted),
                new("Algeciras", Status.Deleted)
            };
            var relation = Substitute.For<IRelation>();
            relation.GetEnumerator().Returns(elements.GetEnumerator());
            var relationExtractor = Substitute.For<ISingleExtractor>();
            relationExtractor.SourceType.Returns(typeof(FakeLocalization));
            relationExtractor.ResultType.Returns(typeof(IReadOnlyRelationSet<int>));
            relationExtractor.ExtractFrom(Arg.Any<FakeLocalization>()).Returns(relation);
            var elementPlan = MakePlanFor<string>();
            var relationPlan = new RelationExtractionPlan(relationExtractor, elementPlan);
            var source = new FakeLocalization("Limerick");

            // Act
            var plan = new LocalizationExtractionPlan(relationPlan);
            (var inserts, var modifies, var deletes) = plan.ExtractFrom(source);
            plan.Canonicalize(source);

            // Assert
            inserts.Should().BeEmpty();
            modifies.Should().BeEmpty();
            deletes.Should().HaveCount(7);
            deletes.Should().ContainEquivalentOf(new DBValue[] { DBValue.Create(source.Key), DBValue.Create(elements[0].Item1) });
            deletes.Should().ContainEquivalentOf(new DBValue[] { DBValue.Create(source.Key), DBValue.Create(elements[1].Item1) });
            deletes.Should().ContainEquivalentOf(new DBValue[] { DBValue.Create(source.Key), DBValue.Create(elements[2].Item1) });
            deletes.Should().ContainEquivalentOf(new DBValue[] { DBValue.Create(source.Key), DBValue.Create(elements[3].Item1) });
            deletes.Should().ContainEquivalentOf(new DBValue[] { DBValue.Create(source.Key), DBValue.Create(elements[4].Item1) });
            deletes.Should().ContainEquivalentOf(new DBValue[] { DBValue.Create(source.Key), DBValue.Create(elements[5].Item1) });
            deletes.Should().ContainEquivalentOf(new DBValue[] { DBValue.Create(source.Key), DBValue.Create(elements[6].Item1) });
            relation.Received(1).Canonicalize();
        }

        [TestMethod] public void MixedStatusLocalizedValues() {
            // Arrange
            var elements = new List<(object, Status)>() {
                new("Bremen", Status.Deleted),
                new("Llanfairpwllgwyngyll", Status.Saved),
                new("Eleusis", Status.Saved),
                new("Irkutsk", Status.Modified),
                new("Erdenet", Status.Deleted),
                new("Eindhoven", Status.New),
                new("Sevastopol", Status.New),
                new("Islington", Status.Modified),
                new("Waterloo", Status.Deleted),
                new("Fatehpur", Status.New)
            };
            var relation = Substitute.For<IRelation>();
            relation.GetEnumerator().Returns(elements.GetEnumerator());
            var relationExtractor = Substitute.For<ISingleExtractor>();
            relationExtractor.SourceType.Returns(typeof(FakeLocalization));
            relationExtractor.ResultType.Returns(typeof(IReadOnlyRelationOrderedList<ulong>));
            relationExtractor.ExtractFrom(Arg.Any<FakeLocalization>()).Returns(relation);
            var elementPlan = MakePlanFor<string>();
            var relationPlan = new RelationExtractionPlan(relationExtractor, elementPlan);
            var source = new FakeLocalization("Newcastle upon Tyne");

            // Act
            var plan = new LocalizationExtractionPlan(relationPlan);
            (var inserts, var modifies, var deletes) = plan.ExtractFrom(source);
            plan.Canonicalize(source);

            // Assert
            inserts.Should().HaveCount(3);
            inserts.Should().ContainEquivalentOf(new DBValue[] { DBValue.Create(source.Key), DBValue.Create(elements[5].Item1) });
            inserts.Should().ContainEquivalentOf(new DBValue[] { DBValue.Create(source.Key), DBValue.Create(elements[6].Item1) });
            inserts.Should().ContainEquivalentOf(new DBValue[] { DBValue.Create(source.Key), DBValue.Create(elements[9].Item1) });
            modifies.Should().HaveCount(2);
            modifies.Should().ContainEquivalentOf(new DBValue[] { DBValue.Create(source.Key), DBValue.Create(elements[3].Item1) });
            modifies.Should().ContainEquivalentOf(new DBValue[] { DBValue.Create(source.Key), DBValue.Create(elements[7].Item1) });
            deletes.Should().HaveCount(3);
            deletes.Should().ContainEquivalentOf(new DBValue[] { DBValue.Create(source.Key), DBValue.Create(elements[0].Item1) });
            deletes.Should().ContainEquivalentOf(new DBValue[] { DBValue.Create(source.Key), DBValue.Create(elements[4].Item1) });
            deletes.Should().ContainEquivalentOf(new DBValue[] { DBValue.Create(source.Key), DBValue.Create(elements[8].Item1) });
            relation.Received(1).Canonicalize();
        }


        private record class FakeLocalization(string Key);
        private static DataExtractionPlan MakePlanFor<T>() {
            var identity = Substitute.For<IMultiExtractor>();
            identity.SourceType.Returns(typeof(T));
            identity.ExtractFrom(Arg.Any<T>()).Returns(args => new object?[] { args[0] });
            return new DataExtractionPlan(new IMultiExtractor[] { identity });
        }
    }
}
