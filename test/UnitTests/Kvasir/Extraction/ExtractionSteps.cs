using FluentAssertions;
using Kvasir.Extraction;
using Kvasir.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;

namespace UT.Kvasir.Extraction {
    [TestClass, TestCategory("PrimitiveExtractionStep")]
    public class PrimitiveExtractionStepTests {
        [TestMethod] public void Construct() {
            // Arrange
            var mockExtractor = Substitute.For<IFieldExtractor>();
            mockExtractor.ExpectedSource.Returns(typeof(string));
            mockExtractor.FieldType.Returns(typeof(int));

            // Act
            var step = new PrimitiveExtractionStep(mockExtractor);

            // Assert
            step.ExpectedSource.Should().Be(typeof(string));
        }

        [TestMethod] public void ExtractFromExact() {
            // Arrange
            var result = 10;
            var mockExtractor = Substitute.For<IFieldExtractor>();
            mockExtractor.ExpectedSource.Returns(typeof(string));
            mockExtractor.FieldType.Returns(typeof(int));
            mockExtractor.Execute(Arg.Any<string>()).Returns(result);
            var step = new PrimitiveExtractionStep(mockExtractor);
            var source = "Grand Rapids";

            // Act
            var value = step.Execute(source);

            // Assert
            mockExtractor.Received(1).Execute(source);
            value.Should().BeEquivalentTo(new DBValue[] { DBValue.Create(result) });
        }

        [TestMethod] public void ExtractFromDerived() {
            // Arrange
            var result = 10;
            var mockExtractor = Substitute.For<IFieldExtractor>();
            mockExtractor.ExpectedSource.Returns(typeof(Exception));
            mockExtractor.FieldType.Returns(typeof(int));
            mockExtractor.Execute(Arg.Any<Exception>()).Returns(result);
            var step = new PrimitiveExtractionStep(mockExtractor);
            var source = new NullReferenceException();

            // Act
            var value = step.Execute(source);

            // Assert
            mockExtractor.Received(1).Execute(source);
            value.Should().BeEquivalentTo(new DBValue[] { DBValue.Create(result) });
        }

        [TestMethod] public void ExtractFromNull() {
            // Arrange
            var result = 10;
            var mockExtractor = Substitute.For<IFieldExtractor>();
            mockExtractor.ExpectedSource.Returns(typeof(string));
            mockExtractor.FieldType.Returns(typeof(int));
            mockExtractor.Execute(Arg.Any<string>()).Returns(result);
            var step = new PrimitiveExtractionStep(mockExtractor);
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
            var mockExtractor = Substitute.For<IFieldExtractor>();
            mockExtractor.ExpectedSource.Returns(typeof(string));
            mockExtractor.FieldType.Returns(typeof(Exception));
            var mockDecomp = Substitute.For<IExtractionStep>();
            mockDecomp.ExpectedSource.Returns(typeof(Exception));
            var decomps = new IExtractionStep[] { mockDecomp };

            // Act
            var step = new DecomposingExtractionStep(mockExtractor, decomps);

            // Assert
            step.ExpectedSource.Should().Be(typeof(string));
        }

        [TestMethod] public void DecomposeWithSingleStep() {
            // Arrange
            var v = 100;
            var value = DBValue.Create(v);
            var mockExtractor = Substitute.For<IFieldExtractor>();
            mockExtractor.ExpectedSource.Returns(typeof(string));
            mockExtractor.FieldType.Returns(typeof(Exception));
            mockExtractor.Execute(Arg.Any<string>()).Returns(v);
            var mockDecomp = Substitute.For<IExtractionStep>();
            mockDecomp.ExpectedSource.Returns(typeof(Exception));
            mockDecomp.Execute(Arg.Any<int>()).Returns(new DBValue[] { value });
            var decomps = new IExtractionStep[] { mockDecomp };
            var step = new DecomposingExtractionStep(mockExtractor, decomps);
            var source = "Walla Walla";

            // Act
            var values = step.Execute(source);

            // Assert
            mockExtractor.Received(1).Execute(source);
            mockDecomp.Received(1).Execute(v);
            values.Should().BeEquivalentTo(new DBValue[] { value });
        }

        [TestMethod] public void DecomposeWithMultipleSteps() {
            // Arrange
            var v = 100;
            var value0 = DBValue.Create(v);
            var value1 = DBValue.Create(v * 10);
            var mockExtractor = Substitute.For<IFieldExtractor>();
            mockExtractor.ExpectedSource.Returns(typeof(string));
            mockExtractor.FieldType.Returns(typeof(Exception));
            mockExtractor.Execute(Arg.Any<string>()).Returns(v);
            var mockDecomp0 = Substitute.For<IExtractionStep>();
            mockDecomp0.ExpectedSource.Returns(typeof(Exception));
            mockDecomp0.Execute(Arg.Any<int>()).Returns(new DBValue[] { value0 });
            var mockDecomp1 = Substitute.For<IExtractionStep>();
            mockDecomp1.ExpectedSource.Returns(typeof(Exception));
            mockDecomp1.Execute(Arg.Any<int>()).Returns(new DBValue[] { value1, value0 });
            var decomps = new IExtractionStep[] { mockDecomp0, mockDecomp1 };
            var step = new DecomposingExtractionStep(mockExtractor, decomps);
            var source = "Albany";

            // Act
            var values = step.Execute(source);

            // Assert
            mockExtractor.Received(1).Execute(source);
            mockDecomp0.Received(1).Execute(v);
            mockDecomp1.Received(1).Execute(v);
            values.Should().BeEquivalentTo(new DBValue[] { value0, value1, value0 });
        }

        [TestMethod] public void ExtractFromNull() {
            // Arrange
            var v = 100;
            var value = DBValue.Create(v);
            var mockExtractor = Substitute.For<IFieldExtractor>();
            mockExtractor.ExpectedSource.Returns(typeof(string));
            mockExtractor.FieldType.Returns(typeof(Exception));
            mockExtractor.Execute(null).Returns(null);
            var mockDecomp = Substitute.For<IExtractionStep>();
            mockDecomp.ExpectedSource.Returns(typeof(Exception));
            mockDecomp.Execute(Arg.Any<int>()).Returns(new DBValue[] { value, value });
            mockDecomp.Execute(null).Returns(new DBValue[] { DBValue.NULL, DBValue.NULL });
            var decomps = new IExtractionStep[] { mockDecomp };
            var step = new DecomposingExtractionStep(mockExtractor, decomps);
            string? source = null;

            // Act
            var values = step.Execute(source);

            // Assert
            values.Should().BeEquivalentTo(new DBValue[] { DBValue.NULL, DBValue.NULL });
        }
    }
}
