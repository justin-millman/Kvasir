using FluentAssertions;
using Cybele.Core;
using Kvasir.Extraction;
using Kvasir.Relations;
using Kvasir.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UT.Kvasir.Extraction {
    [TestClass, TestCategory("DataExtractionPlan")]
    public class DataExtractionPlanTests {
        [TestMethod] public void Construct() {
            // Arrange
            var mockExtractor = new Mock<IExtractionStep>();
            mockExtractor.Setup(e => e.ExpectedSource).Returns(typeof(string));
            mockExtractor.Setup(e => e.Execute(It.IsAny<string>())).Returns(Array.Empty<DBValue>());
            var converter = DataConverter.Identity<int>();

            // Act
            var extractors = new IExtractionStep[] { mockExtractor.Object };
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
            var ymd = new Mock<IExtractionStep>();
            ymd.Setup(e => e.ExpectedSource).Returns(typeof(DateTime));
            ymd.Setup(e => e.Execute(It.IsAny<DateTime>())).Returns(new DBValue[] { year, month, day });

            var hour = DBValue.Create(23);
            var minute = DBValue.Create(59);
            var second = DBValue.Create(59);
            var hms = new Mock<IExtractionStep>();
            hms.Setup(e => e.ExpectedSource).Returns(typeof(DateTime));
            hms.Setup(e => e.Execute(It.IsAny<DateTime>())).Returns(new DBValue[] { hour, minute, second });

            var offsetCnv = DataConverter.Create<int, int>(i => i - 1);
            var identityCnv = DataConverter.Identity<int>();

            var extractors = new IExtractionStep[] { ymd.Object, hms.Object };
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
            ymd.Verify(e => e.Execute(source));
            hms.Verify(e => e.Execute(source));
        }
    }

    [TestClass, TestCategory("RelationExtractionPlan")]
    public class RelationExtractionPlanTests {
        [TestMethod] public void Construct() {
            // Arrange
            var tupleType = typeof(Tuple<RelationList<double>>);
            var mockExtractor = new Mock<IFieldExtractor>();
            mockExtractor.Setup(e => e.ExpectedSource).Returns(tupleType);
            mockExtractor.Setup(e => e.FieldType).Returns(typeof(RelationList<double>));

            var mockStep = new Mock<IExtractionStep>();
            mockStep.Setup(s => s.ExpectedSource).Returns(typeof(RelationList<double>));

            var steps = new IExtractionStep[] { mockStep.Object };
            var converters = new DataConverter[] { DataConverter.Identity<double>() };
            var entityPlan = new DataExtractionPlan(steps, converters);

            // Act
            var relationPlan = new RelationExtractionPlan(mockExtractor.Object, entityPlan);

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

            var mockRelation = new Mock<IRelation>();
            mockRelation.Setup(r => r.GetEnumerator()).Returns(data.GetEnumerator());

            var type = typeof(Tuple<RelationSet<string>>);
            var mockExtractor = new Mock<IFieldExtractor>();
            mockExtractor.Setup(e => e.ExpectedSource).Returns(type);
            mockExtractor.Setup(e => e.FieldType).Returns(typeof(RelationSet<string>));
            mockExtractor.Setup(e => e.Execute(It.IsAny<object>())).Returns(mockRelation.Object);

            var step = new IdentityExtractor<string>();
            var steps = new IExtractionStep[] { new PrimitiveExtractionStep(step, DataConverter.Identity<string>()) };
            var converters = new DataConverter[] { DataConverter.Identity<string>() };
            var entityPlan = new DataExtractionPlan(steps, converters);

            var relationPlan = new RelationExtractionPlan(mockExtractor.Object, entityPlan);

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
