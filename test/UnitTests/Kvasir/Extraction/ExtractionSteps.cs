using FluentAssertions;
using Kvasir.Core;
using Kvasir.Extraction;
using Kvasir.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;

namespace UT.Kvasir.Extraction {
    [TestClass, TestCategory("PrimitiveExtractionStep")]
    public class PrimitiveExtractionStepTests {
        [TestMethod] public void Construct() {
            // Arrange
            var mockExtractor = new Mock<IFieldExtractor>();
            mockExtractor.Setup(e => e.ExpectedSource).Returns(typeof(string));
            mockExtractor.Setup(e => e.FieldType).Returns(typeof(int));
            var conv = DataConverter.Identity<int>();

            // Act
            var step = new PrimitiveExtractionStep(mockExtractor.Object, conv);

            // Assert
            step.ExpectedSource.Should().Be(typeof(string));
        }

        [TestMethod] public void ExtractWithIdentityConversion() {
            // Arrange
            var result = 10;
            var mockExtractor = new Mock<IFieldExtractor>();
            mockExtractor.Setup(e => e.ExpectedSource).Returns(typeof(string));
            mockExtractor.Setup(e => e.FieldType).Returns(typeof(int));
            mockExtractor.Setup(e => e.Execute(It.IsAny<string>())).Returns(result);
            var conv = DataConverter.Identity<int>();
            var step = new PrimitiveExtractionStep(mockExtractor.Object, conv);
            var source = "Grand Rapids";

            // Act
            var value = step.Execute(source);

            // Assert
            mockExtractor.Verify(e => e.Execute(source), Times.Once);
            value.Should().BeEquivalentTo(new DBValue[] { DBValue.Create(result) });
        }

        [TestMethod] public void ExtractWithNonIdentityConversion() {
            // Arrange
            var result = 10;
            var mockExtractor = new Mock<IFieldExtractor>();
            mockExtractor.Setup(e => e.ExpectedSource).Returns(typeof(string));
            mockExtractor.Setup(e => e.FieldType).Returns(typeof(int));
            mockExtractor.Setup(e => e.Execute(It.IsAny<string>())).Returns(result);
            var conv = DataConverter.Create<int, int>(i => i * 2);
            var step = new PrimitiveExtractionStep(mockExtractor.Object, conv);
            var source = "Detroit";

            // Act
            var value = step.Execute(source);

            // Assert
            mockExtractor.Verify(e => e.Execute(source), Times.Once);
            value.Should().BeEquivalentTo(new DBValue[] { DBValue.Create(conv.Convert(result)) });
        }

        [TestMethod] public void ExtractFromDerived() {
            // Arrange
            var result = 10;
            var mockExtractor = new Mock<IFieldExtractor>();
            mockExtractor.Setup(e => e.ExpectedSource).Returns(typeof(Exception));
            mockExtractor.Setup(e => e.FieldType).Returns(typeof(int));
            mockExtractor.Setup(e => e.Execute(It.IsAny<Exception>())).Returns(result);
            var conv = DataConverter.Identity<int>();
            var step = new PrimitiveExtractionStep(mockExtractor.Object, conv);
            var source = new NullReferenceException();

            // Act
            var value = step.Execute(source);

            // Assert
            mockExtractor.Verify(e => e.Execute(source), Times.Once);
            value.Should().BeEquivalentTo(new DBValue[] { DBValue.Create(result) });
        }

        [TestMethod] public void ExtractFromNull() {
            // Arrange
            var result = 10;
            var mockExtractor = new Mock<IFieldExtractor>();
            mockExtractor.Setup(e => e.ExpectedSource).Returns(typeof(string));
            mockExtractor.Setup(e => e.FieldType).Returns(typeof(int));
            mockExtractor.Setup(e => e.Execute(It.IsAny<string>())).Returns(result);
            var conv = DataConverter.Identity<int>();
            var step = new PrimitiveExtractionStep(mockExtractor.Object, conv);
            string? source = null;

            // Act
            var value = step.Execute(source);

            // Assert
            value.Should().BeEquivalentTo(new DBValue[] { DBValue.NULL });
        }
    }

    [TestClass, TestCategory("DecomposingExtractionStep")]
    public class DecomposingExtractionStepTests {
        [TestMethod] public void Construct() {
            // Arrange
            var mockExtractor = new Mock<IFieldExtractor>();
            mockExtractor.Setup(e => e.ExpectedSource).Returns(typeof(string));
            mockExtractor.Setup(e => e.FieldType).Returns(typeof(Exception));
            var mockDecomp = new Mock<IExtractionStep>();
            mockDecomp.Setup(d => d.ExpectedSource).Returns(typeof(Exception));
            var decomps = new IExtractionStep[] { mockDecomp.Object };

            // Act
            var step = new DecomposingExtractionStep(mockExtractor.Object, decomps);

            // Assert
            step.ExpectedSource.Should().Be(typeof(string));
        }

        [TestMethod] public void DecomposeWithSingleStep() {
            // Arrange
            var v = 100;
            var value = DBValue.Create(v);
            var mockExtractor = new Mock<IFieldExtractor>();
            mockExtractor.Setup(e => e.ExpectedSource).Returns(typeof(string));
            mockExtractor.Setup(e => e.FieldType).Returns(typeof(Exception));
            mockExtractor.Setup(e => e.Execute(It.IsAny<string>())).Returns(v);
            var mockDecomp = new Mock<IExtractionStep>();
            mockDecomp.Setup(d => d.ExpectedSource).Returns(typeof(Exception));
            mockDecomp.Setup(d => d.Execute(It.IsAny<int>())).Returns(new DBValue[] { value });
            var decomps = new IExtractionStep[] { mockDecomp.Object };
            var step = new DecomposingExtractionStep(mockExtractor.Object, decomps);
            var source = "Walla Walla";

            // Act
            var values = step.Execute(source);

            // Assert
            mockExtractor.Verify(e => e.Execute(source), Times.Once);
            mockDecomp.Verify(d => d.Execute(v), Times.Once);
            values.Should().BeEquivalentTo(new DBValue[] { value });
        }

        [TestMethod] public void DecomposeWithMultipleSteps() {
            // Arrange
            var v = 100;
            var value0 = DBValue.Create(v);
            var value1 = DBValue.Create(v * 10);
            var mockExtractor = new Mock<IFieldExtractor>();
            mockExtractor.Setup(e => e.ExpectedSource).Returns(typeof(string));
            mockExtractor.Setup(e => e.FieldType).Returns(typeof(Exception));
            mockExtractor.Setup(e => e.Execute(It.IsAny<string>())).Returns(v);
            var mockDecomp0 = new Mock<IExtractionStep>();
            mockDecomp0.Setup(d => d.ExpectedSource).Returns(typeof(Exception));
            mockDecomp0.Setup(d => d.Execute(It.IsAny<int>())).Returns(new DBValue[] { value0 });
            var mockDecomp1 = new Mock<IExtractionStep>();
            mockDecomp1.Setup(d => d.ExpectedSource).Returns(typeof(Exception));
            mockDecomp1.Setup(d => d.Execute(It.IsAny<int>())).Returns(new DBValue[] { value1, value0 });
            var decomps = new IExtractionStep[] { mockDecomp0.Object, mockDecomp1.Object };
            var step = new DecomposingExtractionStep(mockExtractor.Object, decomps);
            var source = "Albany";

            // Act
            var values = step.Execute(source);

            // Assert
            mockExtractor.Verify(e => e.Execute(source), Times.Once);
            mockDecomp0.Verify(d => d.Execute(v), Times.Once);
            mockDecomp1.Verify(d => d.Execute(v), Times.Once);
            values.Should().BeEquivalentTo(new DBValue[] { value0, value1, value0 });
        }

        [TestMethod] public void ExtractFromNull() {
            // Arrange
            var v = 100;
            var value = DBValue.Create(v);
            var mockExtractor = new Mock<IFieldExtractor>();
            mockExtractor.Setup(e => e.ExpectedSource).Returns(typeof(string));
            mockExtractor.Setup(e => e.FieldType).Returns(typeof(Exception));
            mockExtractor.Setup(e => e.Execute(null)).Returns(null);
            var mockDecomp = new Mock<IExtractionStep>();
            mockDecomp.Setup(d => d.ExpectedSource).Returns(typeof(Exception));
            mockDecomp.Setup(d => d.Execute(It.IsAny<int>())).Returns(new DBValue[] { value, value });
            mockDecomp.Setup(d => d.Execute(null)).Returns(new DBValue[] { DBValue.NULL, DBValue.NULL });
            var decomps = new IExtractionStep[] { mockDecomp.Object };
            var step = new DecomposingExtractionStep(mockExtractor.Object, decomps);
            string? source = null;

            // Act
            var values = step.Execute(source);

            // Assert
            values.Should().BeEquivalentTo(new DBValue[] { DBValue.NULL, DBValue.NULL });
        }
    }
}
