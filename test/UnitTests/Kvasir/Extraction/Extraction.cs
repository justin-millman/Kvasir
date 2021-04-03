using FluentAssertions;
using Kvasir.Core;
using Kvasir.Extraction;
using Kvasir.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UT.Kvasir.Extraction {
    [TestClass]
    public class SingleStepExtractionPlanTests {
        [TestMethod, TestCategory("Single Step")]
        public void Construction() {
            // Arrange
            var property = typeof(string).GetProperty(nameof(string.Length))!;
            var converter = DataConverter.Identity<int>();

            // Act
            var plan = new SingleExtractionStep(property, converter);

            // Assert
            plan.ExpectedSource.Should().Be(typeof(string));
        }

        [TestMethod, TestCategory("Single Step")]
        public void ExtractFromActualWithoutConversion() {
            // Arrange
            var property = typeof(string).GetProperty(nameof(string.Length))!;
            var converter = DataConverter.Identity<int>();
            var plan = new SingleExtractionStep(property, converter);
            var source = "Corpus Christi";

            // Act
            var value = plan.Execute(source);

            // Assert
            value.Should().Be(DBValue.Create(converter.Convert(source.Length)));
        }

        [TestMethod, TestCategory("Single Step")]
        public void ExtractFromActualWithConversion() {
            // Arrange
            var property = typeof(string).GetProperty(nameof(string.Length))!;
            var converter = DataConverter.Create<int, string>(i => i.ToString());
            var plan = new SingleExtractionStep(property, converter);
            var source = "Sioux City";

            // Act
            var value = (plan as IExtractionPlan).Execute(source);

            // Assert
            value.Count().Should().Be(1);
            value.First().Should().Be(DBValue.Create(converter.Convert(source.Length)));
        }

        [TestMethod, TestCategory("Single Step")]
        public void ExtractFromDerived() {
            // Arrange
            var property = typeof(Exception).GetProperty(nameof(Exception.Message))!;
            var converter = DataConverter.Identity<string>();
            var plan = new SingleExtractionStep(property, converter);
            var source = new NullReferenceException("Galveston");

            // Act
            var value = plan.Execute(source);

            // Assert
            value.Should().Be(DBValue.Create(converter.Convert(source.Message)));
        }

        [TestMethod, TestCategory("Single Step")]
        public void ExtractFromImplementation() {
            // Arrange
            var property = typeof(ICollection<string>).GetProperty(nameof(ICollection<string>.Count))!;
            var converter = DataConverter.Identity<int>();
            var plan = new SingleExtractionStep(property, converter);
            var source = new List<string>() { "Philadelphia", "Dayton", "Savannah", "Salem", "Annapolis" };

            // Act
            var value = plan.Execute(source);

            // Assert
            value.Should().Be(DBValue.Create(converter.Convert(source.Count)));
        }

        [TestMethod, TestCategory("Single Step")]
        public void ExtractFromNull() {
            // Arrange
            var property = typeof(string).GetProperty(nameof(string.Length))!;
            var converter = DataConverter.Identity<int>();
            var plan = new SingleExtractionStep(property, converter);
            string? source = null;

            // Act
            var value = plan.Execute(source);

            // Assert
            value.Should().Be(DBValue.NULL);
        }
    }

    [TestClass]
    public class RecursiveExtractionPlanTets {
        [TestMethod, TestCategory("Recursive")]
        public void ConstructionIdenticalConnection() {
            // Arrange
            var property = typeof(Exception).GetProperty(nameof(Exception.Message))!;
            var converter = DataConverter.Identity<string>();
            var mockRest = new Mock<IExtractionPlan>();
            mockRest.Setup(p => p.ExpectedSource).Returns(typeof(string));

            // Act
            var plan = new RecursiveExtractionPlan(property, converter, mockRest.Object);

            // Assert
            plan.ExpectedSource.Should().Be(typeof(Exception));
        }

        [TestMethod, TestCategory("Recursive")]
        public void ConstructBaseClassConnection() {
            // Arrange
            var property = typeof(Exception).GetProperty(nameof(Exception.Message))!;
            var converter = DataConverter.Identity<string>();
            var mockRest = new Mock<IExtractionPlan>();
            mockRest.Setup(p => p.ExpectedSource).Returns(typeof(object));

            // Act
            var plan = new RecursiveExtractionPlan(property, converter, mockRest.Object);

            // Assert
            plan.ExpectedSource.Should().Be(typeof(Exception));
        }

        [TestMethod, TestCategory("Recursive")]
        public void ConstructInterfaceConnection() {
            // Arrange
            var property = typeof(Exception).GetProperty(nameof(Exception.Message))!;
            var converter = DataConverter.Identity<string>();
            var mockRest = new Mock<IExtractionPlan>();
            mockRest.Setup(p => p.ExpectedSource).Returns(typeof(IEquatable<string>));

            // Act
            var plan = new RecursiveExtractionPlan(property, converter, mockRest.Object);

            // Assert
            plan.ExpectedSource.Should().Be(typeof(Exception));
        }

        [TestMethod, TestCategory("Recursive")]
        public void ExtractFromActualWithoutConversion() {
            // Arrange
            var property = typeof(string).GetProperty(nameof(string.Length))!;
            var converter = DataConverter.Identity<int>();
            var mockRest = new Mock<IExtractionPlan>();
            mockRest.Setup(p => p.ExpectedSource).Returns(typeof(int));
            mockRest.Setup(p => p.Execute(It.IsAny<object>())).Returns(new List<DBValue>());
            var plan = new RecursiveExtractionPlan(property, converter, mockRest.Object);
            var source = "Manchester";

            // Act
            plan.Execute(source);

            // Assert
            mockRest.Verify(p => p.Execute(source.Length));
        }

        [TestMethod, TestCategory("Recursive")]
        public void ExtractFromActualWithConversion() {
            // Arrange
            var property = typeof(string).GetProperty(nameof(string.Length))!;
            var converter = DataConverter.Create<int, string>(i => i.ToString());
            var mockRest = new Mock<IExtractionPlan>();
            mockRest.Setup(p => p.ExpectedSource).Returns(typeof(int));
            mockRest.Setup(p => p.Execute(It.IsAny<object>())).Returns(new List<DBValue>());
            var plan = new RecursiveExtractionPlan(property, converter, mockRest.Object);
            var source = "Lexington";

            // Act
            plan.Execute(source);

            // Assert
            mockRest.Verify(p => p.Execute(converter.Convert(source.Length)));
        }

        [TestMethod, TestCategory("Recursive")]
        public void ExtractFromDerived() {
            // Arrange
            var property = typeof(Exception).GetProperty(nameof(Exception.Message))!;
            var converter = DataConverter.Identity<string>();
            var mockRest = new Mock<IExtractionPlan>();
            mockRest.Setup(p => p.ExpectedSource).Returns(typeof(string));
            mockRest.Setup(p => p.Execute(It.IsAny<object>())).Returns(new List<DBValue>());
            var plan = new RecursiveExtractionPlan(property, converter, mockRest.Object);
            var source = new NullReferenceException("Mesa");

            // Act
            plan.Execute(source);

            // Assert
            mockRest.Verify(p => p.Execute(source.Message));
        }

        [TestMethod, TestCategory("Recursive")]
        public void ExtractFromImplementation() {
            // Arrange
            var property = typeof(ICollection<string>).GetProperty(nameof(ICollection<string>.Count))!;
            var converter = DataConverter.Identity<int>();
            var mockRest = new Mock<IExtractionPlan>();
            mockRest.Setup(p => p.ExpectedSource).Returns(typeof(int));
            mockRest.Setup(p => p.Execute(It.IsAny<object>())).Returns(new List<DBValue>());
            var plan = new RecursiveExtractionPlan(property, converter, mockRest.Object);
            var source = new List<string>() { "Long Beach", "St. Paul", "Chattanooga", "Dallas" };

            // Act
            plan.Execute(source);

            // Assert
            mockRest.Verify(p => p.Execute(source.Count));
        }

        [TestMethod, TestCategory("Recursive")]
        public void ExtractFromNull() {
            // Arrange
            var property = typeof(string).GetProperty(nameof(string.Length))!;
            var converter = DataConverter.Identity<int>();
            var mockRest = new Mock<IExtractionPlan>();
            mockRest.Setup(p => p.ExpectedSource).Returns(typeof(int));
            mockRest.Setup(p => p.Execute(It.IsAny<object>())).Returns(new List<DBValue>());
            var plan = new RecursiveExtractionPlan(property, converter, mockRest.Object);
            string? source = null;

            // Act
            plan.Execute(source);

            // Assert
            mockRest.Verify(p => p.Execute(null));
        }
    }

    [TestClass]
    public class ExtractionPlanTests {
        [TestMethod, TestCategory("Full Plan")]
        public void Construction() {
            // Arrange
            var mockPlan1 = new Mock<IExtractionPlan>();
            mockPlan1.Setup(p => p.ExpectedSource).Returns(typeof(string));
            var mockPlan2 = new Mock<IExtractionPlan>();
            mockPlan2.Setup(p => p.ExpectedSource).Returns(typeof(string));
            var mockPlans = new List<IExtractionPlan>() { mockPlan1.Object, mockPlan2.Object };

            // Act
            var plan = new ExtractionPlan(mockPlans);

            // Assert
            plan.ExpectedSource.Should().Be(typeof(string));
        }

        [TestMethod, TestCategory("Full Plan")]
        public void ExtractFromActual() {
            // Arrange
            var plan1Results = new List<DBValue>() { DBValue.Create(50), DBValue.Create("ABC") };
            var mockPlan1 = new Mock<IExtractionPlan>();
            mockPlan1.Setup(p => p.ExpectedSource).Returns(typeof(string));
            mockPlan1.Setup(p => p.Execute(It.IsAny<object>())).Returns(plan1Results);
            var plan2Results = new List<DBValue>() { DBValue.Create("DEF"), DBValue.Create('-'), DBValue.NULL };
            var mockPlan2 = new Mock<IExtractionPlan>();
            mockPlan2.Setup(p => p.ExpectedSource).Returns(typeof(string));
            mockPlan2.Setup(p => p.Execute(It.IsAny<object>())).Returns(plan2Results);
            var mockPlans = new List<IExtractionPlan>() { mockPlan1.Object, mockPlan2.Object };
            var plan = new ExtractionPlan(mockPlans);
            var source = "Hialeah";

            // Act
            var results = plan.Execute(source);

            // Assert
            results.Count().Should().Be(plan1Results.Count() + plan2Results.Count());
            results.Should().StartWith(plan1Results);
            results.Should().EndWith(plan2Results);
            mockPlan1.Verify(p => p.Execute(source));
            mockPlan2.Verify(p => p.Execute(source));
        }

        [TestMethod, TestCategory("Full Plan")]
        public void ExtractFromDerived() {
            // Arrange
            var plan1Results = new List<DBValue>() { DBValue.Create(50), DBValue.Create("ABC") };
            var mockPlan1 = new Mock<IExtractionPlan>();
            mockPlan1.Setup(p => p.ExpectedSource).Returns(typeof(Exception));
            mockPlan1.Setup(p => p.Execute(It.IsAny<object>())).Returns(plan1Results);
            var plan2Results = new List<DBValue>() { DBValue.Create("DEF"), DBValue.Create('-'), DBValue.NULL };
            var mockPlan2 = new Mock<IExtractionPlan>();
            mockPlan2.Setup(p => p.ExpectedSource).Returns(typeof(Exception));
            mockPlan2.Setup(p => p.Execute(It.IsAny<object>())).Returns(plan2Results);
            var mockPlans = new List<IExtractionPlan>() { mockPlan1.Object, mockPlan2.Object };
            var plan = new ExtractionPlan(mockPlans);
            var source = new NullReferenceException("Anchorage");

            // Act
            var results = plan.Execute(source);

            // Assert
            results.Count().Should().Be(plan1Results.Count() + plan2Results.Count());
            results.Should().StartWith(plan1Results);
            results.Should().EndWith(plan2Results);
            mockPlan1.Verify(p => p.Execute(source));
            mockPlan2.Verify(p => p.Execute(source));
        }

        [TestMethod, TestCategory("Full Plan")]
        public void ExtractFromImplementation() {
            // Arrange
            var plan1Results = new List<DBValue>() { DBValue.Create(50), DBValue.Create("ABC") };
            var mockPlan1 = new Mock<IExtractionPlan>();
            mockPlan1.Setup(p => p.ExpectedSource).Returns(typeof(IEquatable<string>));
            mockPlan1.Setup(p => p.Execute(It.IsAny<object>())).Returns(plan1Results);
            var plan2Results = new List<DBValue>() { DBValue.Create("DEF"), DBValue.Create('-'), DBValue.NULL };
            var mockPlan2 = new Mock<IExtractionPlan>();
            mockPlan2.Setup(p => p.ExpectedSource).Returns(typeof(IEquatable<string>));
            mockPlan2.Setup(p => p.Execute(It.IsAny<object>())).Returns(plan2Results);
            var mockPlans = new List<IExtractionPlan>() { mockPlan1.Object, mockPlan2.Object };
            var plan = new ExtractionPlan(mockPlans);
            var source = "Colorado Springs";

            // Act
            var results = plan.Execute(source);

            // Assert
            results.Count().Should().Be(plan1Results.Count() + plan2Results.Count());
            results.Should().StartWith(plan1Results);
            results.Should().EndWith(plan2Results);
            mockPlan1.Verify(p => p.Execute(source));
            mockPlan2.Verify(p => p.Execute(source));
        }

        [TestMethod, TestCategory("Full Plan")]
        public void ExtractFromNull() {
            // Arrange
            var plan1Results = new List<DBValue>() { DBValue.Create(50), DBValue.Create("ABC") };
            var mockPlan1 = new Mock<IExtractionPlan>();
            mockPlan1.Setup(p => p.ExpectedSource).Returns(typeof(string));
            mockPlan1.Setup(p => p.Execute(It.IsAny<object>())).Returns(plan1Results);
            var plan2Results = new List<DBValue>() { DBValue.Create("DEF"), DBValue.Create('-'), DBValue.NULL };
            var mockPlan2 = new Mock<IExtractionPlan>();
            mockPlan2.Setup(p => p.ExpectedSource).Returns(typeof(string));
            mockPlan2.Setup(p => p.Execute(It.IsAny<object>())).Returns(plan2Results);
            var mockPlans = new List<IExtractionPlan>() { mockPlan1.Object, mockPlan2.Object };
            var plan = new ExtractionPlan(mockPlans);
            string? source = null;

            // Act
            var results = plan.Execute(source);

            // Assert
            results.Count().Should().Be(plan1Results.Count() + plan2Results.Count());
            results.Should().StartWith(plan1Results);
            results.Should().EndWith(plan2Results);
            mockPlan1.Verify(p => p.Execute(source));
            mockPlan2.Verify(p => p.Execute(source));
        }
    }
}
