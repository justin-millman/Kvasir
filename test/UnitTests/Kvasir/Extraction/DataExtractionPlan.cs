using FluentAssertions;
using Kvasir.Extraction;
using Kvasir.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Linq;
using System.Reflection;

namespace UT.Kvasir.Extraction {
    [TestClass, TestCategory("DataExtractionPlan")]
    public class DataExtractionPlanTests {
        [TestMethod] public void SingleExtractorProducesOneValue() {
            // Arrange
            var data = new object?[] { (ushort)18773 };
            var extractor = Substitute.For<IMultiExtractor>();
            extractor.SourceType.Returns(typeof(string));
            extractor.ExtractFrom(Arg.Any<string>()).Returns(data);

            // Act
            var source = "Puerto Baquerizo Moreno";
            var extractors = new IMultiExtractor[] { extractor };
            var plan = new DataExtractionPlan(extractors);
            var row = plan.ExtractFrom(source);

            // Assert
            plan.SourceType.Should().Be(extractor.SourceType);
            row.Should().BeEquivalentTo(data.Select(v => DBValue.Create(v)));
            extractor.Received().ExtractFrom(source);
        }

        [TestMethod] public void SingleExtractorProducesMultipleValues() {
            // Arrange
            var data = new object[] { false, 'v', DateTime.Now, -9281023814L };
            var extractor = Substitute.For<IMultiExtractor>();
            extractor.SourceType.Returns(typeof(UriFormat));
            extractor.ExtractFrom(Arg.Any<UriFormat>()).Returns(data);

            // Act
            var source = UriFormat.UriEscaped;
            var extractors = new IMultiExtractor[] { extractor };
            var plan = new DataExtractionPlan(extractors);
            var row = plan.ExtractFrom(source);

            // Assert
            plan.SourceType.Should().Be(extractor.SourceType);
            row.Should().BeEquivalentTo(data.Select(v => DBValue.Create(v)));
            extractor.Received().ExtractFrom(source);
        }

        [TestMethod] public void MultipleExtractors() {
            // Arrange
            var firstData = new object?[] { true, 'X' };
            var firstExtractor = Substitute.For<IMultiExtractor>();
            firstExtractor.SourceType.Returns(typeof(Assembly));
            firstExtractor.ExtractFrom(Arg.Any<Assembly>()).Returns(firstData);
            var secondData = new object?[] {"", DateTime.Now, float.Pi };
            var secondExtractor = Substitute.For<IMultiExtractor>();
            secondExtractor.SourceType.Returns(typeof(Assembly));
            secondExtractor.ExtractFrom(Arg.Any<Assembly>()).Returns(secondData);
            var thirdData = new object?[] { true };
            var thirdExtractor = Substitute.For<IMultiExtractor>();
            thirdExtractor.SourceType.Returns(typeof(Assembly));
            thirdExtractor.ExtractFrom(Arg.Any<Assembly>()).Returns(thirdData);
            var fourthData = new object?[] { 0.004, "Lübeck", "Darwin", "Zaporizhzhia" };
            var fourthExtractor = Substitute.For<IMultiExtractor>();
            fourthExtractor.SourceType.Returns(typeof(Assembly));
            fourthExtractor.ExtractFrom(Arg.Any<Assembly>()).Returns(fourthData);

            // Act
            var source = typeof(Assembly).Assembly;
            var extractors = new IMultiExtractor[] { firstExtractor, secondExtractor, thirdExtractor, fourthExtractor };
            var plan = new DataExtractionPlan(extractors);
            var row = plan.ExtractFrom(source);

            // Assert
            plan.SourceType.Should().Be(firstExtractor.SourceType);
            row.Should().BeEquivalentTo(firstData.Concat(secondData).Concat(thirdData).Concat(fourthData).Select(v => DBValue.Create(v)));
            firstExtractor.Received().ExtractFrom(source);
            secondExtractor.Received().ExtractFrom(source);
            thirdExtractor.Received().ExtractFrom(source);
            fourthExtractor.Received().ExtractFrom(source);
        }

        [TestMethod] public void ExtractorsProduceSomeNulls() {
            // Arrange
            var firstData = new object?[] { -1894, null };
            var firstExtractor = Substitute.For<IMultiExtractor>();
            firstExtractor.SourceType.Returns(typeof(double?));
            firstExtractor.ExtractFrom(Arg.Any<double?>()).Returns(firstData);
            var secondData = new object?[] { "Yekaterinburg", '~', -3842.33 };
            var secondExtractor = Substitute.For<IMultiExtractor>();
            secondExtractor.SourceType.Returns(typeof(double?));
            secondExtractor.ExtractFrom(Arg.Any<double?>()).Returns(secondData);
            var thirdData = new object?[] { null, null, null };
            var thirdExtractor = Substitute.For<IMultiExtractor>();
            thirdExtractor.SourceType.Returns(typeof(double?));
            thirdExtractor.ExtractFrom(Arg.Any<double?>()).Returns(thirdData);

            // Act
            double? source = 185712.914;
            var extractors = new IMultiExtractor[] { firstExtractor, secondExtractor, thirdExtractor };
            var plan = new DataExtractionPlan(extractors);
            var row = plan.ExtractFrom(source);

            // Assert
            plan.SourceType.Should().Be(firstExtractor.SourceType);
            row.Should().BeEquivalentTo(firstData.Concat(secondData).Concat(thirdData).Select(v => DBValue.Create(v)));
            firstExtractor.Received().ExtractFrom(source);
            secondExtractor.Received().ExtractFrom(source);
            thirdExtractor.Received().ExtractFrom(source);
        }

        [TestMethod] public void ExtractorsProduceAllNulls() {
            // Arrange
            var firstData = new object?[] { null, null, null, null, null };
            var firstExtractor = Substitute.For<IMultiExtractor>();
            firstExtractor.SourceType.Returns(typeof(NullReferenceException));
            firstExtractor.ExtractFrom(Arg.Any<NullReferenceException>()).Returns(firstData);
            var secondData = new object?[] { null };
            var secondExtractor = Substitute.For<IMultiExtractor>();
            secondExtractor.SourceType.Returns(typeof(NullReferenceException));
            secondExtractor.ExtractFrom(Arg.Any<NullReferenceException>()).Returns(secondData);

            // Act
            var source = new NullReferenceException();
            var extractors = new IMultiExtractor[] { firstExtractor, secondExtractor };
            var plan = new DataExtractionPlan(extractors);
            var row = plan.ExtractFrom(source);

            // Assert
            plan.SourceType.Should().Be(firstExtractor.SourceType);
            row.Should().BeEquivalentTo(firstData.Concat(secondData).Select(v => DBValue.Create(v)));
            firstExtractor.Received().ExtractFrom(source);
            secondExtractor.Received().ExtractFrom(source);
        }
    }
}
