using FluentAssertions;
using Kvasir.Extraction;
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
            value.Should().BeEquivalentTo(new object?[] { result });
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
            value.Should().BeEquivalentTo(new object?[] { result });
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
            value.Should().BeEquivalentTo(new object?[] { null });
        }

        [TestMethod] public void ExtractYieldsEnumeration() {
            // Arrange
            var result = DayOfWeek.Thursday;
            var mockExtractor = Substitute.For<IFieldExtractor>();
            mockExtractor.ExpectedSource.Returns(typeof(Tuple<DayOfWeek>));
            mockExtractor.FieldType.Returns(typeof(DayOfWeek));
            mockExtractor.Execute(Arg.Any<Tuple<DayOfWeek>>()).Returns(result);
            var step = new PrimitiveExtractionStep(mockExtractor);
            var source = new Tuple<DayOfWeek>(result);

            // Act
            var value = step.Execute(source);

            // Assert
            mockExtractor.Received(1).Execute(source);
            value.Should().BeEquivalentTo(new object?[] { result });
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
            var value = 100;
            var mockExtractor = Substitute.For<IFieldExtractor>();
            mockExtractor.ExpectedSource.Returns(typeof(string));
            mockExtractor.FieldType.Returns(typeof(Exception));
            mockExtractor.Execute(Arg.Any<string>()).Returns(value);
            var mockDecomp = Substitute.For<IExtractionStep>();
            mockDecomp.ExpectedSource.Returns(typeof(Exception));
            mockDecomp.Execute(Arg.Any<int>()).Returns(new object?[] { value });
            var decomps = new IExtractionStep[] { mockDecomp };
            var step = new DecomposingExtractionStep(mockExtractor, decomps);
            var source = "Walla Walla";

            // Act
            var values = step.Execute(source);

            // Assert
            mockExtractor.Received(1).Execute(source);
            mockDecomp.Received(1).Execute(value);
            values.Should().BeEquivalentTo(new object?[] { value });
        }

        [TestMethod] public void DecomposeWithMultipleSteps() {
            // Arrange
            var value0 = 100;
            var value1 = 1000;
            var mockExtractor = Substitute.For<IFieldExtractor>();
            mockExtractor.ExpectedSource.Returns(typeof(string));
            mockExtractor.FieldType.Returns(typeof(Exception));
            mockExtractor.Execute(Arg.Any<string>()).Returns(value0);
            var mockDecomp0 = Substitute.For<IExtractionStep>();
            mockDecomp0.ExpectedSource.Returns(typeof(Exception));
            mockDecomp0.Execute(Arg.Any<int>()).Returns(new object?[] { value0 });
            var mockDecomp1 = Substitute.For<IExtractionStep>();
            mockDecomp1.ExpectedSource.Returns(typeof(Exception));
            mockDecomp1.Execute(Arg.Any<int>()).Returns(new object?[] { value1, value0 });
            var decomps = new IExtractionStep[] { mockDecomp0, mockDecomp1 };
            var step = new DecomposingExtractionStep(mockExtractor, decomps);
            var source = "Albany";

            // Act
            var values = step.Execute(source);

            // Assert
            mockExtractor.Received(1).Execute(source);
            mockDecomp0.Received(1).Execute(value0);
            mockDecomp1.Received(1).Execute(value0);
            values.Should().BeEquivalentTo(new object?[] { value0, value1, value0 });
        }

        [TestMethod] public void ExtractFromNull() {
            // Arrange
            var value = 100;
            var mockExtractor = Substitute.For<IFieldExtractor>();
            mockExtractor.ExpectedSource.Returns(typeof(string));
            mockExtractor.FieldType.Returns(typeof(Exception));
            mockExtractor.Execute(null).Returns(null);
            var mockDecomp = Substitute.For<IExtractionStep>();
            mockDecomp.ExpectedSource.Returns(typeof(Exception));
            mockDecomp.Execute(Arg.Any<int>()).Returns(new object?[] { value, value });
            mockDecomp.Execute(null).Returns(new object?[] { DBNull.Value, DBNull.Value });
            var decomps = new IExtractionStep[] { mockDecomp };
            var step = new DecomposingExtractionStep(mockExtractor, decomps);
            string? source = null;

            // Act
            var values = step.Execute(source);

            // Assert
            values.Should().BeEquivalentTo(new object?[] { DBNull.Value, DBNull.Value });
        }
    }
}
