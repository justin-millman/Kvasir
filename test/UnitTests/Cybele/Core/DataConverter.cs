using Cybele.Core;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UT.Cybele.Core {
    [TestClass, TestCategory("DataConverter")]
    public class DataConverterTests {
        [TestMethod] public void CreateUnidirectional() {
            // Arrange
            Converter<int, string> fn = i => i.ToString();

            // Act
            var converter = DataConverter.Create(fn);

            // Assert
            converter.SourceType.Should().Be(typeof(int));
            converter.ResultType.Should().Be(typeof(string));
            converter.IsBidirectional.Should().BeFalse();
        }

        [TestMethod] public void CreateBidirectional() {
            // Arrange
            Converter<int, string> fwd = i => i.ToString();
            Converter<string, int> bwd = s => int.Parse(s);

            // Act
            var converter = DataConverter.Create(fwd, bwd);

            // Assert
            converter.SourceType.Should().Be(typeof(int));
            converter.ResultType.Should().Be(typeof(string));
            converter.IsBidirectional.Should().BeTrue();
        }

        [TestMethod] public void ForwardConversionActualType() {
            // Arrange
            Converter<string, int> fn = s => s.Length;
            var converter = DataConverter.Create(fn);
            var source = "Indianapolis";

            // Act
            var expected = fn(source);
            var conversion = converter.Convert(source);

            // Assert
            conversion!.GetType().Should().Be(typeof(int));
            conversion.Should().Be(expected);
        }

        [TestMethod] public void ForwardConversionDerivedType() {
            // Arrange
            Converter<Exception, string> fn = e => e.Message;
            var converter = DataConverter.Create(fn);
            var source = new ArgumentNullException("San Jose");

            // Act
            var expected = fn(source);
            var conversion = converter.Convert(source);

            // Assert
            conversion!.GetType().Should().Be(typeof(string));
            conversion.Should().Be(expected);
        }

        [TestMethod] public void ForwardConversionImplementationType() {
            // Arrange
            Converter<IReadOnlyList<string>, int> fn = e => e.Count;
            var converter = DataConverter.Create(fn);
            var source = new List <string>() { "Greensboro", "Amarillo", "Augusta", "New York City" };

            // Act
            var expected = fn(source);
            var conversion = converter.Convert(source);

            // Assert
            conversion!.GetType().Should().Be(typeof(int));
            conversion.Should().Be(expected);
        }

        [TestMethod] public void BackwardConversionActualType() {
            // Arrange
            Converter<string, int> fwd = s => s.Length;
            Converter<int, string> bwd = i => string.Concat(Enumerable.Repeat('+', i));
            var converter = DataConverter.Create(fwd, bwd);
            var result = 13;

            // Act
            var expected = bwd(result);
            var reversion = converter.Revert(result);

            // Assert
            reversion!.GetType().Should().Be(typeof(string));
            reversion.Should().Be(expected);
        }

        [TestMethod] public void BackwardConversionDerivedType() {
            // Arrange
            Converter<string, Exception> fwd = s => new Exception(s);
            Converter<Exception, string> bwd = e => e.Message;
            var converter = DataConverter.Create(fwd, bwd);
            var result = new ArgumentNullException("Ypsilanti");

            // Act
            var expected = bwd(result);
            var reversion = converter.Revert(result);

            // Assert
            reversion!.GetType().Should().Be(typeof(string));
            reversion.Should().Be(expected);
        }

        [TestMethod] public void BackwardConversionImplementationType() {
            // Arrange
            Converter<int, IReadOnlyList<string>> fwd = i => Enumerable.Repeat("???", i).ToList();
            Converter<IReadOnlyList<string>, int> bwd = e => e.Count;
            var converter = DataConverter.Create(fwd, bwd);
            var result = new List<string>() { "Toledo", "Norfolk", "Carson City", "Minneapolis", "Miami" };

            // Act
            var expected = bwd(result);
            var reversion = converter.Revert(result);

            // Assert
            reversion!.GetType().Should().Be(typeof(int));
            reversion.Should().Be(expected);
        }
        
        [TestMethod] public void BackwardConversionNotSupported() {
            // Arrange
            Converter<string, int> fn = s => s.Length;
            var converter = DataConverter.Create(fn);

            // Act
            Action action = () => converter.Revert(57);

            // Assert
            action.Should().ThrowExactly<NotSupportedException>().WithAnyMessage();
        }

        [TestMethod] public void ForwardConversionUnrelatedType() {
            // Arrange
            Converter<string, int> fwd = s => s.Length;
            var converter = DataConverter.Create(fwd);

            // Act
            Action action = () => converter.Convert(DateTime.Today);

            // Assert
            action.Should().ThrowExactly<ArgumentException>().WithAnyMessage();
        }

        [TestMethod] public void BackwardConversionUnrelatedType() {
            // Arrange
            Converter<string, int> fwd = s => s.Length;
            Converter<int, string> bwd = i => string.Concat(Enumerable.Repeat('+', i));
            var converter = DataConverter.Create(fwd, bwd);

            // Act
            Action action = () => converter.Revert(DateTime.Today);

            // Assert
            action.Should().ThrowExactly<ArgumentException>().WithAnyMessage();
        }

        [TestMethod] public void IdentityConversionFromGeneric() {
            // Arrange
            var converter = DataConverter.Identity<double>();
            var source = 88.5;

            // Act
            var conversion = converter.Convert(source);
            var reversion = converter.Revert(conversion);

            // Assert
            converter.SourceType.Should().Be(typeof(double));
            converter.ResultType.Should().Be(typeof(double));
            converter.IsBidirectional.Should().BeTrue();
            conversion.Should().Be(source);
            reversion.Should().Be(source);
        }

        [TestMethod] public void IdentityConversionFromArgument() {
            // Arrange
            var converter = DataConverter.Identity(typeof(double));
            var source = -119.0226;

            // Act
            var conversion = converter.Convert(source);
            var reversion = converter.Revert(conversion);

            // Assert
            converter.SourceType.Should().Be(typeof(double));
            converter.ResultType.Should().Be(typeof(double));
            converter.IsBidirectional.Should().BeTrue();
            conversion.Should().Be(source);
            reversion.Should().Be(source);
        }

        [TestMethod] public void ConvertNull() {
            // Arrange
            var classConverter = DataConverter.Identity<string>();
            var structConverter = DataConverter.Identity<int>();

            // Act
            var classConversion = classConverter.Convert(null);
            var structConversion = structConverter.Convert(null);

            // Assert
            classConversion.Should().BeNull();
            structConversion.Should().BeNull();
        }

        [TestMethod] public void RevertNull() {
            // Arrange
            var classConverter = DataConverter.Identity<string>();
            var structConverter = DataConverter.Identity<int>();

            // Act
            var classReversion = classConverter.Revert(null);
            var structReversion = structConverter.Revert(null);

            // Assert
            classReversion.Should().BeNull();
            structReversion.Should().BeNull();
        }
    }
}
