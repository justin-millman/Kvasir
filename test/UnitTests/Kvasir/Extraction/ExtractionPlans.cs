using Cybele.Core;
using FluentAssertions;
using Kvasir.Extraction;
using Kvasir.Relations;
using Kvasir.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UT.Kvasir.Extraction {
    [TestClass, TestCategory("DataExtractionPlan")]
    public class DataExtractionPlanTests {
        [TestMethod] public void Construct() {
            // Arrange
            var mockExtractor = Substitute.For<IExtractionStep>();
            mockExtractor.ExpectedSource.Returns(typeof(string));
            mockExtractor.Execute(Arg.Any<string>()).Returns(Array.Empty<DBValue>());
            var converter = DataConverter.Identity<int>();

            // Act
            var extractors = new IExtractionStep[] { mockExtractor };
            var converters = new DataConverter[] { converter };
            var plan = new DataExtractionPlan(extractors, converters);

            // Assert
            plan.ExpectedSource.Should().Be(typeof(string));
        }

        [TestMethod] public void Execute() {
            // Arrange
            var year = DBValue.Create(2012);
            var month = DBValue.Create(12);
            var day = DBValue.Create(31);
            var ymd = Substitute.For<IExtractionStep>();
            ymd.ExpectedSource.Returns(typeof(DateTime));
            ymd.Execute(Arg.Any<DateTime>()).Returns(new DBValue[] { year, month, day });

            var hour = DBValue.Create(23);
            var minute = DBValue.Create(59);
            var second = DBValue.Create(59);
            var hms = Substitute.For<IExtractionStep>();
            hms.ExpectedSource.Returns(typeof(DateTime));
            hms.Execute(Arg.Any<DateTime>()).Returns(new DBValue[] { hour, minute, second });

            var offsetCnv = DataConverter.Create<int, int>(i => i - 1);
            var identityCnv = DataConverter.Identity<int>();

            var extractors = new IExtractionStep[] { ymd, hms };
            var converters = Enumerable.Repeat(offsetCnv, 3).Concat(Enumerable.Repeat(identityCnv, 3));
            var plan = new DataExtractionPlan(extractors, converters);
            var source = DateTime.Now;

            // Act
            var values = plan.Execute(source);

            // Assert
            values.Should().HaveCount(6);
            values[0].Should().Be(DBValue.Create((int)year.Datum - 1));
            values[1].Should().Be(DBValue.Create((int)month.Datum - 1));
            values[2].Should().Be(DBValue.Create((int)day.Datum - 1));
            values[3].Should().Be(hour);
            values[4].Should().Be(minute);
            values[5].Should().Be(second);
            ymd.Received().Execute(source);
            hms.Received().Execute(source);
        }
    }

    [TestClass, TestCategory("RelationExtractionPlan")]
    public class RelationExtractionPlanTests {
        [TestMethod] public void Construct() {
            // Arrange
            var tupleType = typeof(Tuple<RelationList<double>>);
            var mockExtractor = Substitute.For<IFieldExtractor>();
            mockExtractor.ExpectedSource.Returns(tupleType);
            mockExtractor.FieldType.Returns(typeof(RelationList<double>));

            var mockStep = Substitute.For<IExtractionStep>();
            mockStep.ExpectedSource.Returns(typeof(RelationList<double>));

            var steps = new IExtractionStep[] { mockStep };
            var converters = new DataConverter[] { DataConverter.Identity<double>() };
            var entityPlan = new DataExtractionPlan(steps, converters);

            // Act
            var relationPlan = new RelationExtractionPlan(mockExtractor, entityPlan);

            // Assert
            relationPlan.ExpectedSource.Should().Be(tupleType);
        }

        [TestMethod] public void Execute() {
            // Arrange
            var data = new List<(object Item, Status Status)>() {
                ("Birmingham", Status.Deleted),
                ("Praia", Status.Deleted),
                ("Kraków", Status.Deleted),
                ("Stuttgart", Status.Saved),
                ("Sapporo", Status.Modified),
                ("Mombasa", Status.New),
                ("Mariupol", Status.Saved),
                ("Faiyum", Status.New),
                ("Medellín", Status.Saved)
            };

            var mockRelation = Substitute.For<IRelation>();
            mockRelation.GetEnumerator().Returns(data.GetEnumerator());

            var type = typeof(Tuple<RelationSet<string>>);
            var mockExtractor = Substitute.For<IFieldExtractor>();
            mockExtractor.ExpectedSource.Returns(type);
            mockExtractor.FieldType.Returns(typeof(RelationSet<string>));
            mockExtractor.Execute(Arg.Any<object>()).Returns(mockRelation);

            var step = new IdentityExtractor<string>();
            var steps = new IExtractionStep[] { new PrimitiveExtractionStep(step) };
            var converters = new DataConverter[] { DataConverter.Identity<string>() };
            var entityPlan = new DataExtractionPlan(steps, converters);

            var relationPlan = new RelationExtractionPlan(mockExtractor, entityPlan);

            // Act
            var results = relationPlan.Execute(new Tuple<RelationSet<string>>(null!));

            // Assert
            results.Deletions.Count().Should().Be(3);
            results.Modifications.Count().Should().Be(1);
            results.Insertions.Count().Should().Be(2);
            results.Deletions.ToList()[0][0].Datum.Should().Be("Birmingham");
            results.Deletions.ToList()[1][0].Datum.Should().Be("Praia");
            results.Deletions.ToList()[2][0].Datum.Should().Be("Kraków");
            results.Modifications.ToList()[0][0].Datum.Should().Be("Sapporo");
            results.Insertions.ToList()[0][0].Datum.Should().Be("Mombasa");
            results.Insertions.ToList()[1][0].Datum.Should().Be("Faiyum");
        }
    }
}
