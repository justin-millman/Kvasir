using FluentAssertions;
using Kvasir.Core;
using Kvasir.Extraction;
using Kvasir.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Linq;

namespace UT.Kvasir.Extraction {
    [TestClass, TestCategory("EntityExtractionPlan")]
    public class EntityExtractionPlanTests {
        [TestMethod] public void Construct() {
            // Arrange
            var mockExtractor = new Mock<IExtractionStep>();
            mockExtractor.Setup(e => e.ExpectedSource).Returns(typeof(string));
            mockExtractor.Setup(e => e.Execute(It.IsAny<string>())).Returns(Array.Empty<DBValue>());
            var converter = DataConverter.Identity<int>();

            // Act
            var extractors = new IExtractionStep[] { mockExtractor.Object };
            var converters = new DataConverter[] { converter };
            var plan = new EntityExtractionPlan(extractors, converters);

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
            var plan = new EntityExtractionPlan(extractors, converters);
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
}
