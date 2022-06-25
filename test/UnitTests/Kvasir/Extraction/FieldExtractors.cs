using FluentAssertions;
using Kvasir.Extraction;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace UT.Kvasir.Extraction {
    [TestClass, TestCategory("ReadPropertyExtractor")]
    public class ReadPropertyExtractorTests {
        [TestMethod] public void Construct() {
            // Arrange
            var prop = typeof(string).GetProperty(nameof(string.Length))!;

            // Act
            var extractor = new ReadPropertyExtractor(prop);

            // Assert
            extractor.FieldType.Should().Be(prop.PropertyType);
            extractor.ExpectedSource.Should().Be(prop.ReflectedType);
        }

        [TestMethod] public void ExtractFromExact() {
            // Arrange
            var prop = typeof(string).GetProperty(nameof(string.Length))!;
            var extractor = new ReadPropertyExtractor(prop);
            var source = "Oconomowoc";

            // Act
            var value = extractor.Execute(source);

            // Assert
            value.Should().Be(source.Length);
        }

        [TestMethod] public void ExtractFromDerived() {
            // Arrange
            var prop = typeof(Exception).GetProperty(nameof(Exception.HResult))!;
            var extractor = new ReadPropertyExtractor(prop);
            var source = new ArgumentException();

            // Act
            var value = extractor.Execute(source);

            // Assert
            value.Should().Be(source.HResult);
        }

        [TestMethod] public void ExtractFromNull() {
            // Arrange
            var prop = typeof(string).GetProperty(nameof(string.Length))!;
            var extractor = new ReadPropertyExtractor(prop);
            string? source = null;

            // Act
            var value = extractor.Execute(source);

            // Asert
            value.Should().BeNull();
        }
    }

    [TestClass, TestCategory("IdentityExtractor")]
    public class IdentityExtractorTests {
        [TestMethod] public void Construct() {
            // Arrange

            // Act
            var extractor = new IdentityExtractor<int>();

            // Assert
            extractor.FieldType.Should().Be(typeof(int));
            extractor.ExpectedSource.Should().Be(typeof(int));
        }

        [TestMethod] public void ExtractFromPrimitive() {
            // Arrange
            var extractor = new IdentityExtractor<bool>();
            bool? source = false;

            // Act
            var value = extractor.Execute(source);

            // Assert
            value.Should().Be(source);
        }

        [TestMethod] public void ExtractFromNull() {
            // Arrange
            var extractor = new IdentityExtractor<DateTime>();
            DateTime? source = null;

            // Act
            var value = extractor.Execute(source);

            // Assert
            value.Should().BeNull();
        }
    }
}
