using Cybele.Core;
using FluentAssertions;
using Kvasir.Extraction;
using Kvasir.Schema;
using Kvasir.Transcription;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace UT.Kvasir.Extraction {
    [TestClass, TestCategory("ReadPropertyExtractor")]
    public class ReadPropertyExtractorTests {
        [TestMethod] public void ExtractFromExact() {
            // Arrange
            var path = new PropertyChain(typeof(DateTime), nameof(DateTime.Nanosecond));

            // Act
            var source = DateTime.Now;
            var extractor = new ReadPropertyExtractor(path);
            var datum = extractor.ExtractFrom(source);
            var multiDatum = (extractor as IMultiExtractor).ExtractFrom(source);

            // Assert
            extractor.SourceType.Should().Be(path.ReflectedType);
            extractor.ResultType.Should().Be(path.PropertyType);
            datum.Should().Be(source.Nanosecond);
            multiDatum.Should().BeEquivalentTo(new object?[] { datum });
        }

        [TestMethod] public void ExtractFromDerived() {
            // Arrange
            var path = new PropertyChain(typeof(Exception), nameof(Exception.Message));

            // Act
            var source = new ArgumentException("Invalid value for some argument");
            var extractor = new ReadPropertyExtractor(path);
            var datum = extractor.ExtractFrom(source);
            var multiDatum = (extractor as IMultiExtractor).ExtractFrom(source);

            // Assert
            extractor.SourceType.Should().Be(path.ReflectedType);
            extractor.ResultType.Should().Be(path.PropertyType);
            datum.Should().Be(source.Message);
            multiDatum.Should().BeEquivalentTo(new object?[] { datum });
        }

        [TestMethod] public void ExtractFromImplementation() {
            // Arrange
            var path = new PropertyChain(typeof(ICollection<string>), nameof(ICollection<string>.Count));

            // Act
            var source = new List<string>() { "Pisa", "Manaus", "İzmir", "Abbottabad", "Split" };
            var extractor = new ReadPropertyExtractor(path);
            var datum = extractor.ExtractFrom(source);
            var multiDatum = (extractor as IMultiExtractor).ExtractFrom(source);

            // Assert
            extractor.SourceType.Should().Be(path.ReflectedType);
            extractor.ResultType.Should().Be(path.PropertyType);
            datum.Should().Be(source.Count);
            multiDatum.Should().BeEquivalentTo(new object?[] { datum });
        }

        [TestMethod] public void ExtractFromNull() {
            // Arrange
            var path = new PropertyChain(typeof(Random), nameof(Random.Shared));

            // Act
            var extractor = new ReadPropertyExtractor(path);
            var datum = extractor.ExtractFrom(null);
            var multiDatum = (extractor as IMultiExtractor).ExtractFrom(null);

            // Assert
            extractor.SourceType.Should().Be(path.ReflectedType);
            extractor.ResultType.Should().Be(path.PropertyType);
            datum.Should().BeNull();
            multiDatum.Should().BeEquivalentTo(new object?[] { datum });
        }

        [TestMethod] public void ExtractFromNonPublicProperty() {
            // Arrange
            var path = new PropertyChain(typeof(SqlSnippet), "Contents");

            // Act
            var source = new SqlSnippet("INSERT INTO [table] ([column1], [column2]) VALUES ([[value1], [value2])");
            var extractor = new ReadPropertyExtractor(path);
            var datum = extractor.ExtractFrom(source);
            var multiDatum = (extractor as IMultiExtractor).ExtractFrom(source);

            // Assert
            extractor.SourceType.Should().Be(path.ReflectedType);
            extractor.ResultType.Should().Be(path.PropertyType);
            datum.Should().Be(source.ToString());
            multiDatum.Should().BeEquivalentTo(new object?[] { datum });
        }

        [TestMethod] public void ExtractFromStaticProperty() {
            // Arrange
            var path = new PropertyChain(typeof(DBValue), nameof(DBValue.NULL));

            // Act
            var source = DBValue.Create(-12841824.0131996);
            var extractor = new ReadPropertyExtractor(path);
            var datum = extractor.ExtractFrom(source);
            var multiDatum = (extractor as IMultiExtractor).ExtractFrom(source);

            // Assert
            extractor.SourceType.Should().Be(path.ReflectedType);
            extractor.ResultType.Should().Be(path.PropertyType);
            datum.Should().Be(DBValue.NULL);
            multiDatum.Should().BeEquivalentTo(new object?[] { datum });
        }

        [TestMethod] public void ExtractionProducesNull() {
            // Arrange
            var path = new PropertyChain(typeof(Type), nameof(Type.TypeInitializer));

            // Act
            var source = typeof(Action);
            var extractor = new ReadPropertyExtractor(path);
            var datum = extractor.ExtractFrom(source);
            var multiDatum = (extractor as IMultiExtractor).ExtractFrom(source);

            // Assert
            extractor.SourceType.Should().Be(path.ReflectedType);
            extractor.ResultType.Should().Be(path.PropertyType);
            datum.Should().BeNull();
            multiDatum.Should().BeEquivalentTo(new object?[] { datum });
        }

        [TestMethod] public void ExtractionPropertyTypeIsNullable() {
            // Arrange
            var type = typeof(Tuple<int?, char?, double?>);
            var path = new PropertyChain(type, "Item2");

            // Act
            var extractor = new ReadPropertyExtractor(path);

            // Assert
            extractor.SourceType.Should().Be(type);
            extractor.ResultType.Should().Be(typeof(char));
        }
    }

    [TestClass, TestCategory("ConvertingExtractor")]
    public class ConvertingExtractorTests {
        [TestMethod] public void ExtractFromExact() {
            // Arrange
            var unconvertedValue = Guid.NewGuid();
            var originalExtractor = Substitute.For<ISingleExtractor>();
            originalExtractor.SourceType.Returns(typeof(FlagsAttribute));
            originalExtractor.ResultType.Returns(typeof(Guid));
            originalExtractor.ExtractFrom(Arg.Any<Attribute>()).Returns(unconvertedValue);
            var converter = DataConverter.Create<Guid, int>(g => g.ToString().Count(c => c == 'A' || c == 'a'));

            // Act
            var source = new FlagsAttribute();
            var extractor = new ConvertingExtractor(originalExtractor, converter);
            var datum = extractor.ExtractFrom(source);
            var multiDatum = (extractor as IMultiExtractor).ExtractFrom(source);

            // Assert
            extractor.SourceType.Should().Be(originalExtractor.SourceType);
            extractor.ResultType.Should().Be(converter.ResultType);
            datum.Should().Be(converter.Convert(unconvertedValue));
            multiDatum.Should().BeEquivalentTo(new object?[] { datum });
            originalExtractor.Received().ExtractFrom(source);
        }

        [TestMethod] public void ExtractFromDerived() {
            // Arrange
            var unconvertedValue = ".jpeg";
            var originalExtractor = Substitute.For<ISingleExtractor>();
            originalExtractor.SourceType.Returns(typeof(FileSystemInfo));
            originalExtractor.ResultType.Returns(typeof(string));
            originalExtractor.ExtractFrom(Arg.Any<FileSystemInfo>()).Returns(unconvertedValue);
            var converter = DataConverter.Create<string, string>(x => x[1..].ToUpper());

            // Act
            var source = new FileInfo("Ma'an, Jordan");
            var extractor = new ConvertingExtractor(originalExtractor, converter);
            var datum = extractor.ExtractFrom(source);
            var multiDatum = (extractor as IMultiExtractor).ExtractFrom(source);

            // Assert
            extractor.SourceType.Should().Be(originalExtractor.SourceType);
            extractor.ResultType.Should().Be(converter.ResultType);
            datum.Should().Be(converter.Convert(unconvertedValue));
            multiDatum.Should().BeEquivalentTo(new object?[] { datum });
            originalExtractor.Received().ExtractFrom(source);
        }

        [TestMethod] public void ExtractFromImplementation() {
            // Arrange
            var unconvertedValue = DateTime.Now;
            var originalExtractor = Substitute.For<ISingleExtractor>();
            originalExtractor.SourceType.Returns(typeof(IReadOnlySet<string>));
            originalExtractor.ResultType.Returns(typeof(DateTime));
            originalExtractor.ExtractFrom(Arg.Any<HashSet<string>>()).Returns(unconvertedValue);
            var converter = DataConverter.Create<DateTime, DayOfWeek>(dt => dt.DayOfWeek);

            // Act
            var source = new HashSet<string>() { "Dunedin", "Cajamarca", "Bordeaux" };
            var extractor = new ConvertingExtractor(originalExtractor, converter);
            var datum = extractor.ExtractFrom(source);
            var multiDatum = (extractor as IMultiExtractor).ExtractFrom(source);

            // Assert
            extractor.SourceType.Should().Be(originalExtractor.SourceType);
            extractor.ResultType.Should().Be(converter.ResultType);
            datum.Should().Be(converter.Convert(unconvertedValue));
            multiDatum.Should().BeEquivalentTo(new object?[] { datum });
            originalExtractor.Received().ExtractFrom(source);
        }

        [TestMethod] public void ExtractFromNull() {
            // Arrange
            var originalExtractor = Substitute.For<ISingleExtractor>();
            originalExtractor.SourceType.Returns(typeof(Optional.Option<short>));
            originalExtractor.ResultType.Returns(typeof(bool));
            originalExtractor.ExtractFrom(Arg.Any<Optional.Option<ushort>>()).Returns(null);
            var converter = DataConverter.Create<bool, char>(b => b ? 'T' : 'F');

            // Act
            var extractor = new ConvertingExtractor(originalExtractor, converter);
            var datum = extractor.ExtractFrom(null);
            var multiDatum = (extractor as IMultiExtractor).ExtractFrom(null);

            // Assert
            extractor.SourceType.Should().Be(originalExtractor.SourceType);
            extractor.ResultType.Should().Be(converter.ResultType);
            datum.Should().BeNull();
            multiDatum.Should().BeEquivalentTo(new object?[] { datum });
        }

        [TestMethod] public void OriginalExtractionProducesNull() {
            // Arrange
            var originalExtractor = Substitute.For<ISingleExtractor>();
            originalExtractor.SourceType.Returns(typeof(EventArgs));
            originalExtractor.ResultType.Returns(typeof(EventArgs));
            originalExtractor.ExtractFrom(Arg.Any<EventArgs>()).Returns(null);
            var converter = DataConverter.Create<EventArgs, EventArgs>(p => p);

            // Act
            var source = new EventArgs();
            var extractor = new ConvertingExtractor(originalExtractor, converter);
            var datum = extractor.ExtractFrom(source);
            var multiDatum = (extractor as IMultiExtractor).ExtractFrom(source);

            // Assert
            extractor.SourceType.Should().Be(originalExtractor.SourceType);
            extractor.ResultType.Should().Be(converter.ResultType);
            datum.Should().BeNull();
            multiDatum.Should().BeEquivalentTo(new object?[] { datum });
            originalExtractor.Received().ExtractFrom(source);
        }

        [TestMethod] public void ConversionTypeIsNullable() {
            // Arrange
            var originalExtractor = Substitute.For<ISingleExtractor>();
            originalExtractor.SourceType.Returns(typeof(int?));
            originalExtractor.ResultType.Returns(typeof(int));
            var converter = DataConverter.Create<int?, int?>(p => -p, p => -p);

            // Act
            var extractor = new ConvertingExtractor(originalExtractor, converter);

            // Assert
            extractor.SourceType.Should().Be(typeof(int));
            extractor.ResultType.Should().Be(typeof(int));
        }
    }

    [TestClass, TestCategory("DecomposingExtractor")]
    public class DecomposingExtractorTests {
        [TestMethod] public void ExtractFromExact() {
            // Arrange
            var firstValues = new object?[] { "Dresden", 198.55f };
            var firstSubExtractor = Substitute.For<IMultiExtractor>();
            firstSubExtractor.SourceType.Returns(typeof(Queue<string>));
            firstSubExtractor.ExtractFrom(Arg.Any<Queue<string>>()).Returns(firstValues);
            var secondValues = new object?[] { false, null, "Vereeniging" };
            var secondSubExtractor = Substitute.For<IMultiExtractor>();
            secondSubExtractor.SourceType.Returns(typeof(Queue<string>));
            secondSubExtractor.ExtractFrom(Arg.Any<Queue<string>>()).Returns(secondValues);

            // Act
            var source = new Queue<string>();
            var extractor = new DecomposingExtractor(new IMultiExtractor[] { firstSubExtractor, secondSubExtractor });
            var data = extractor.ExtractFrom(source);

            // Assert
            extractor.SourceType.Should().Be(firstSubExtractor.SourceType);
            data.Should().BeEquivalentTo(firstValues.Concat(secondValues));
            firstSubExtractor.Received().ExtractFrom(source);
            secondSubExtractor.Received().ExtractFrom(source);
        }

        [TestMethod] public void ExtractFromDerived() {
            // Arrange
            var firstValues = new object?[] { '8', (sbyte)-99, SeekOrigin.Current, DateTime.Now };
            var firstSubExtractor = Substitute.For<IMultiExtractor>();
            firstSubExtractor.SourceType.Returns(typeof(MemberInfo));
            firstSubExtractor.ExtractFrom(Arg.Any<MemberInfo>()).Returns(firstValues);

            // Act
            var source = typeof(PropertyInfo).GetProperties()[0];
            var extractor = new DecomposingExtractor(new IMultiExtractor[] { firstSubExtractor });
            var data = extractor.ExtractFrom(source);

            // Assert
            extractor.SourceType.Should().Be(firstSubExtractor.SourceType);
            data.Should().BeEquivalentTo(firstValues);
            firstSubExtractor.Received().ExtractFrom(source);
        }

        [TestMethod] public void ExtractFromImplementation() {
            // Arrange
            var firstValues = new object?[] { "" };
            var firstSubExtractor = Substitute.For<IMultiExtractor>();
            firstSubExtractor.SourceType.Returns(typeof(IComparable));
            firstSubExtractor.ExtractFrom(Arg.Any<IComparable>()).Returns(firstValues);
            var secondValues = new object?[] { 45148123UL };
            var secondSubExtractor = Substitute.For<IMultiExtractor>();
            secondSubExtractor.SourceType.Returns(typeof(IComparable));
            secondSubExtractor.ExtractFrom(Arg.Any<IComparable>()).Returns(secondValues);
            var thirdValues = new object?[] { Guid.NewGuid() };
            var thirdSubExtractor = Substitute.For<IMultiExtractor>();
            thirdSubExtractor.SourceType.Returns(typeof(IComparable));
            thirdSubExtractor.ExtractFrom(Arg.Any<IComparable>()).Returns(thirdValues);

            // Act
            var source = "Mayagüez";
            var extractor = new DecomposingExtractor(new IMultiExtractor[] { firstSubExtractor, secondSubExtractor, thirdSubExtractor });
            var data = extractor.ExtractFrom(source);

            // Assert
            extractor.SourceType.Should().Be(firstSubExtractor.SourceType);
            data.Should().BeEquivalentTo(firstValues.Concat(secondValues).Concat(thirdValues));
            firstSubExtractor.Received().ExtractFrom(source);
            secondSubExtractor.Received().ExtractFrom(source);
            thirdSubExtractor.Received().ExtractFrom(source);
        }

        [TestMethod] public void ExtractFromNullWithOneStep() {
            // Arrange
            var firstValues = new object?[] { null, null };
            var firstSubExtractor = Substitute.For<IMultiExtractor>();
            firstSubExtractor.SourceType.Returns(typeof(char));
            firstSubExtractor.ExtractFrom(null).Returns(firstValues);

            // Act
            var extractor = new DecomposingExtractor(new IMultiExtractor[] { firstSubExtractor });
            var data = extractor.ExtractFrom(null);

            // Assert
            extractor.SourceType.Should().Be(firstSubExtractor.SourceType);
            data.Should().BeEquivalentTo(firstValues);
            firstSubExtractor.Received().ExtractFrom(null);
        }

        [TestMethod] public void ExtractFromNullWithMultipleSteps() {
            // Arrange
            var firstValues = new object?[] { null, null, null, null, null, null, null };
            var firstSubExtractor = Substitute.For<IMultiExtractor>();
            firstSubExtractor.SourceType.Returns(typeof(HashCode));
            firstSubExtractor.ExtractFrom(null).Returns(firstValues);
            var secondValues = new object?[] { null, null, null };
            var secondSubExtractor = Substitute.For<IMultiExtractor>();
            secondSubExtractor.SourceType.Returns(typeof(HashCode));
            secondSubExtractor.ExtractFrom(null).Returns(secondValues);

            // Act
            var extractor = new DecomposingExtractor(new IMultiExtractor[] { firstSubExtractor, secondSubExtractor });
            var data = extractor.ExtractFrom(null);

            // Assert
            extractor.SourceType.Should().Be(firstSubExtractor.SourceType);
            data.Should().BeEquivalentTo(firstValues.Concat(secondValues));
            firstSubExtractor.Received().ExtractFrom(null);
            secondSubExtractor.Received().ExtractFrom(null);
        }
    }

    [TestClass, TestCategory("CurryingExtractor")]
    public class CurryingExtractorTests {
        [TestMethod] public void ExtractFromExact() {
            // Arrange
            var curriedValue = "Mosul";
            var singleExtractor = Substitute.For<ISingleExtractor>();
            singleExtractor.SourceType.Returns(typeof(double[]));
            singleExtractor.ResultType.Returns(typeof(string));
            singleExtractor.ExtractFrom(Arg.Any<double[]>()).Returns(curriedValue);
            var resultValues = new object?[] { '-', 17.554f, DateTime.Now, Guid.NewGuid(), Guid.NewGuid() };
            var multiExtractor = Substitute.For<IMultiExtractor>();
            multiExtractor.SourceType.Returns(typeof(string));
            multiExtractor.ExtractFrom(Arg.Any<string>()).Returns(resultValues);

            // Act
            var source = new double[] { 0.0, 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9 };
            var extractor = new CurryingExtractor(singleExtractor, multiExtractor);
            var data = extractor.ExtractFrom(source);

            // Assert
            extractor.SourceType.Should().Be(singleExtractor.SourceType);
            data.Should().BeEquivalentTo(resultValues);
            singleExtractor.Received().ExtractFrom(source);
            multiExtractor.Received().ExtractFrom(curriedValue);
        }

        [TestMethod] public void ExtractFromDerived() {
            // Arrange
            var curriedValue = DateTime.Now;
            var singleExtractor = Substitute.For<ISingleExtractor>();
            singleExtractor.SourceType.Returns(typeof(Exception));
            singleExtractor.ResultType.Returns(typeof(DateTime));
            singleExtractor.ExtractFrom(Arg.Any<Exception>()).Returns(curriedValue);
            var resultValues = new object?[] { "Aleppo", -1440414L };
            var multiExtractor = Substitute.For<IMultiExtractor>();
            multiExtractor.SourceType.Returns(typeof(DateTime));
            multiExtractor.ExtractFrom(Arg.Any<DateTime>()).Returns(resultValues);

            // Act
            var source = new DivideByZeroException();
            var extractor = new CurryingExtractor(singleExtractor, multiExtractor);
            var data = extractor.ExtractFrom(source);

            // Assert
            extractor.SourceType.Should().Be(singleExtractor.SourceType);
            data.Should().BeEquivalentTo(resultValues);
            singleExtractor.Received().ExtractFrom(source);
            multiExtractor.Received().ExtractFrom(curriedValue);
        }

        [TestMethod] public void ExtractFromImplementation() {
            // Arrange
            var curriedValue = "Montego Bay";
            var singleExtractor = Substitute.For<ISingleExtractor>();
            singleExtractor.SourceType.Returns(typeof(IDisposable));
            singleExtractor.ResultType.Returns(typeof(string));
            singleExtractor.ExtractFrom(Arg.Any<IDisposable>()).Returns(curriedValue);
            var resultValues = new object?[] { (ushort)1124 };
            var multiExtractor = Substitute.For<IMultiExtractor>();
            multiExtractor.SourceType.Returns(typeof(string));
            multiExtractor.ExtractFrom(Arg.Any<string>()).Returns(resultValues);

            // Act
            var source = new StreamReader(new MemoryStream());
            var extractor = new CurryingExtractor(singleExtractor, multiExtractor);
            var data = extractor.ExtractFrom(source);

            // Assert
            extractor.SourceType.Should().Be(singleExtractor.SourceType);
            data.Should().BeEquivalentTo(resultValues);
            singleExtractor.Received().ExtractFrom(source);
            multiExtractor.Received().ExtractFrom(curriedValue);
        }

        [TestMethod] public void ExtractFromNull() {
            // Arrange
            var singleExtractor = Substitute.For<ISingleExtractor>();
            singleExtractor.SourceType.Returns(typeof(DateTime));
            singleExtractor.ResultType.Returns(typeof(XorClause));
            singleExtractor.ExtractFrom(Arg.Any<DateTime>()).Returns(null);
            var resultValues = new object?[] { null, null, null, null };
            var multiExtractor = Substitute.For<IMultiExtractor>();
            multiExtractor.SourceType.Returns(typeof(XorClause));
            multiExtractor.ExtractFrom(Arg.Any<XorClause>()).Returns(resultValues);

            // Act
            var extractor = new CurryingExtractor(singleExtractor, multiExtractor);
            var data = extractor.ExtractFrom(null);

            // Assert
            extractor.SourceType.Should().Be(singleExtractor.SourceType);
            data.Should().BeEquivalentTo(resultValues);
            singleExtractor.Received().ExtractFrom(null);
            multiExtractor.Received().ExtractFrom(null);
        }

        [TestMethod] public void OriginalExtractionProducesNull() {
            // Arrange
            var singleExtractor = Substitute.For<ISingleExtractor>();
            singleExtractor.SourceType.Returns(typeof(List<char>));
            singleExtractor.ResultType.Returns(typeof(List<char>));
            singleExtractor.ExtractFrom(Arg.Any<List<char>>()).Returns(null);
            var resultValues = new object?[] { null, null };
            var multiExtractor = Substitute.For<IMultiExtractor>();
            multiExtractor.SourceType.Returns(typeof(List<char>));
            multiExtractor.ExtractFrom(Arg.Any<List<char>>()).Returns(resultValues);

            // Act
            var source = new List<char>() { 'K', 'a', 'n', 'd', 'a', 'h', 'a', 'r' };
            var extractor = new CurryingExtractor(singleExtractor, multiExtractor);
            var data = extractor.ExtractFrom(source);

            // Assert
            extractor.SourceType.Should().Be(singleExtractor.SourceType);
            data.Should().BeEquivalentTo(resultValues);
            singleExtractor.Received().ExtractFrom(source);
            multiExtractor.Received().ExtractFrom(null);
        }
    }
}
