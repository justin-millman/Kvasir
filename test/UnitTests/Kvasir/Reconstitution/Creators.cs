using Cybele.Core;
using FluentAssertions;
using Kvasir.Extraction;
using Kvasir.Reconstitution;
using Kvasir.Schema;
using Kvasir.Transcription;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;

namespace UT.Kvasir.Reconstitution {
    [TestClass, TestCategory("IdentityCreator")]
    public class IdentityCreatorTests {
        [TestMethod] public void NullValue() {
            // Arrange
            var source = DBValue.NULL;

            // Act
            var creator = new IdentityCreator(typeof(string));
            var value = creator.CreateFrom(new DBValue[] { source });

            // Assert
            creator.ResultType.Should().Be(typeof(string));
            value.Should().BeNull();
        }

        [TestMethod] public void NumericValues() {
            // Arrange
            var sbyteSource = DBValue.Create((sbyte)122);
            var byteSource = DBValue.Create((byte)254);
            var shortSource = DBValue.Create((short)-1689);
            var ushortSource = DBValue.Create((ushort)8);
            var intSource = DBValue.Create(int.MaxValue);
            var uintSource = DBValue.Create(0U);
            var longSource = DBValue.Create(-81884919248107L);
            var ulongSource = DBValue.Create(5623773501UL);

            // Act
            var sbyteCreator = new IdentityCreator(typeof(sbyte));
            var sbyteValue = sbyteCreator.CreateFrom(new DBValue[] { sbyteSource });
            var byteCreator = new IdentityCreator(typeof(byte));
            var byteValue = byteCreator.CreateFrom(new DBValue[] { byteSource });
            var shortCreator = new IdentityCreator(typeof(short));
            var shortValue = shortCreator.CreateFrom(new DBValue[] { shortSource });
            var ushortCreator = new IdentityCreator(typeof(ushort));
            var ushortValue = ushortCreator.CreateFrom(new DBValue[] { ushortSource });
            var intCreator = new IdentityCreator(typeof(int));
            var intValue = intCreator.CreateFrom(new DBValue[] { intSource });
            var uintCreator = new IdentityCreator(typeof(uint));
            var uintValue = uintCreator.CreateFrom(new DBValue[] { uintSource });
            var longCreator = new IdentityCreator(typeof(long));
            var longValue = longCreator.CreateFrom(new DBValue[] { longSource });
            var ulongCreator = new IdentityCreator(typeof(ulong));
            var ulongValue = ulongCreator.CreateFrom(new DBValue[] { ulongSource });

            // Assert
            sbyteCreator.ResultType.Should().Be(typeof(sbyte));
            sbyteValue.Should().Be(sbyteSource.Datum);
            byteCreator.ResultType.Should().Be(typeof(byte));
            byteValue.Should().Be(byteSource.Datum);
            shortCreator.ResultType.Should().Be(typeof(short));
            shortValue.Should().Be(shortSource.Datum);
            ushortCreator.ResultType.Should().Be(typeof(ushort));
            ushortValue.Should().Be(ushortSource.Datum);
            intCreator.ResultType.Should().Be(typeof(int));
            intValue.Should().Be(intSource.Datum);
            uintCreator.ResultType.Should().Be(typeof(uint));
            uintValue.Should().Be(uintSource.Datum);
            longCreator.ResultType.Should().Be(typeof(long));
            longValue.Should().Be(longSource.Datum);
            ulongCreator.ResultType.Should().Be(typeof(ulong));
            ulongValue.Should().Be(ulongSource.Datum);
        }

        [TestMethod] public void TextualValues() {
            // Arrange
            var charSource = DBValue.Create('-');
            var stringSource = DBValue.Create("Belo Horizonte");

            // Act
            var charCreator = new IdentityCreator(typeof(char));
            var charValue = charCreator.CreateFrom(new DBValue[] { charSource });
            var stringCreator = new IdentityCreator(typeof(string));
            var stringValue = stringCreator.CreateFrom(new DBValue[] { stringSource });

            // Assert
            charCreator.ResultType.Should().Be(typeof(char));
            charValue.Should().Be(charSource.Datum);
            stringCreator.ResultType.Should().Be(typeof(string));
            stringValue.Should().Be(stringSource.Datum);
        }

        [TestMethod] public void OtherValues() {
            // Arrange
            var boolSource = DBValue.Create(false);
            var datetimeSource = DBValue.Create(DateTime.Now);
            var guidSource = DBValue.Create(new Guid());

            // Act
            var boolCreator = new IdentityCreator(typeof(bool));
            var boolValue = boolCreator.CreateFrom(new DBValue[] { boolSource });
            var datetimeCreator = new IdentityCreator(typeof(DateTime));
            var datetimeValue = datetimeCreator.CreateFrom(new DBValue[] { datetimeSource });
            var guidCreator = new IdentityCreator(typeof(Guid));
            var guidValue = guidCreator.CreateFrom(new DBValue[] { guidSource });

            // Assert
            boolCreator.ResultType.Should().Be(typeof(bool));
            boolValue.Should().Be(boolSource.Datum);
            datetimeCreator.ResultType.Should().Be(typeof(DateTime));
            datetimeValue.Should().Be(datetimeSource.Datum);
            guidCreator.ResultType.Should().Be(typeof(Guid));
            guidValue.Should().Be(guidSource.Datum);
        }
    }

    [TestClass, TestCategory("RevertingCreator")]
    public class RevertingCreatorTests {
        [TestMethod] public void RevertNull() {
            // Arrange
            var originalCreator = Substitute.For<ICreator>();
            originalCreator.ResultType.Returns(typeof(string));
            originalCreator.CreateFrom(Arg.Any<IReadOnlyList<DBValue>>()).Returns(null);
            var converter = DataConverter.Identity<string>();

            // Act
            var row = new DBValue[] { DBValue.Create("Nacogdoches") };
            var creator = new RevertingCreator(originalCreator, converter);
            var value = creator.CreateFrom(row);

            // Assert
            creator.ResultType.Should().Be(converter.SourceType);
            value.Should().BeNull();
            originalCreator.Received().CreateFrom(row);
        }

        [TestMethod] public void RevertNoChange() {
            // Arrange
            var unrevertedValue = "Highland Park";
            var originalCreator = Substitute.For<ICreator>();
            originalCreator.ResultType.Returns(typeof(string));
            originalCreator.CreateFrom(Arg.Any<IReadOnlyList<DBValue>>()).Returns(unrevertedValue);
            var converter = DataConverter.Identity<string>();

            // Act
            var row = new DBValue[] { DBValue.Create(100), DBValue.Create('y') };
            var creator = new RevertingCreator(originalCreator, converter);
            var value = creator.CreateFrom(row);

            // Assert
            creator.ResultType.Should().Be(converter.SourceType);
            value.Should().Be(converter.Revert(unrevertedValue));
            originalCreator.Received().CreateFrom(row);
        }

        [TestMethod] public void RevertChangeValueSameType() {
            // Arrange
            var unrevertedValue = "Bloomington";
            var originalCreator = Substitute.For<ICreator>();
            originalCreator.ResultType.Returns(typeof(string));
            originalCreator.CreateFrom(Arg.Any<IReadOnlyList<DBValue>>()).Returns(unrevertedValue);
            var converter = DataConverter.Create<string, string>(s => s, s => "Genoa");

            // Act
            var row = new DBValue[] { DBValue.Create("Tirana"), DBValue.Create("Bamako"), DBValue.Create(0L) };
            var creator = new RevertingCreator(originalCreator, converter);
            var value = creator.CreateFrom(row);

            // Assert
            creator.ResultType.Should().Be(converter.SourceType);
            value.Should().Be(converter.Revert(unrevertedValue));
            originalCreator.Received().CreateFrom(row);
        }

        [TestMethod] public void RevertChangeType() {
            // Arrange
            var unrevertedValue = 1785812;
            var originalCreator = Substitute.For<ICreator>();
            originalCreator.ResultType.Returns(typeof(int));
            originalCreator.CreateFrom(Arg.Any<IReadOnlyList<DBValue>>()).Returns(unrevertedValue);
            var converter = DataConverter.Create<double, int>(d => (int)(d * d), i => Math.Sqrt(i));

            // Act
            var row = new DBValue[] { DBValue.Create((ushort)13845) };
            var creator = new RevertingCreator(originalCreator, converter);
            var value = creator.CreateFrom(row);

            // Assert
            creator.ResultType.Should().Be(converter.SourceType);
            value.Should().Be(converter.Revert(unrevertedValue));
            originalCreator.Received().CreateFrom(row);
        }

        [TestMethod] public void RevertProduceEnumeration() {
            // Arrange
            var unrevertedValue = false;
            var originalCreator = Substitute.For<ICreator>();
            originalCreator.ResultType.Returns(typeof(bool));
            originalCreator.CreateFrom(Arg.Any<IReadOnlyList<DBValue>>()).Returns(unrevertedValue);
            var converter = DataConverter.Create<IsNullable, bool>(n => n == IsNullable.Yes, b => b ? IsNullable.Yes : IsNullable.No);

            // Act
            var row = new DBValue[] { DBValue.Create("Seneca Falls") };
            var creator = new RevertingCreator(originalCreator, converter);
            var value = creator.CreateFrom(row);

            // Assert
            creator.ResultType.Should().Be(converter.SourceType);
            value.Should().Be(converter.Revert(unrevertedValue));
            originalCreator.Received().CreateFrom(row);
        }
    }

    [TestClass, TestCategory("ConstructingCreator")]
    public class ConstructingCreatorTests {
        [TestMethod] public void DefaultConstructor() {
            // Arrange
            var ctor = typeof(MemoryStream).GetConstructors()[0];

            // Act
            var row = new DBValue[] { DBValue.Create(100), DBValue.Create(3.7M) };
            var args = Enumerable.Empty<ICreator>();
            var creator = new ConstructingCreator(ctor, args, true);
            var value = creator.CreateFrom(row);

            // Assert
            creator.ResultType.Should().Be(typeof(MemoryStream));
            value.Should().BeOfType<MemoryStream>();
            (value as MemoryStream)!.CanRead.Should().Be(true);
            (value as MemoryStream)!.CanWrite.Should().Be(true);
            (value as MemoryStream)!.CanSeek.Should().Be(true);
            (value as MemoryStream)!.Capacity.Should().Be(0);
        }

        [TestMethod] public void ConstructFromSingleNonNull() {
            // Arrange
            var contents = "Valladolid";
            var ctor = typeof(SqlSnippet).GetConstructors()[0];
            var arg0creator = Substitute.For<ICreator>();
            arg0creator.ResultType.Returns(typeof(string));
            arg0creator.CreateFrom(Arg.Any<IReadOnlyList<DBValue>>()).Returns(contents);

            // Act
            var row = new DBValue[] { DBValue.Create(true), DBValue.Create("Whanganui") };
            var args = new ICreator[] { arg0creator };
            var creator = new ConstructingCreator(ctor, args, true);
            var value = creator.CreateFrom(row);

            // Assert
            creator.ResultType.Should().Be(typeof(SqlSnippet));
            value.Should().BeOfType<SqlSnippet>();
            (value as SqlSnippet)!.ToString().Should().Be(contents);
            arg0creator.Received().CreateFrom(row);
        }

        [TestMethod] public void ConstructFromMultipleNonNull() {
            // Arrange
            var first = "Chișinău";
            var second = 71828.4812f;
            var third = DateTime.Now;
            var ctor = typeof(Tuple<string, float, DateTime>).GetConstructors()[0];
            var arg0creator = Substitute.For<ICreator>();
            arg0creator.ResultType.Returns(typeof(string));
            arg0creator.CreateFrom(Arg.Any<IReadOnlyList<DBValue>>()).Returns(first);
            var arg1creator = Substitute.For<ICreator>();
            arg1creator.ResultType.Returns(typeof(float));
            arg1creator.CreateFrom(Arg.Any<IReadOnlyList<DBValue>>()).Returns(second);
            var arg2creator = Substitute.For<ICreator>();
            arg2creator.ResultType.Returns(typeof(DateTime));
            arg2creator.CreateFrom(Arg.Any<IReadOnlyList<DBValue>>()).Returns(third);

            // Act
            var row = new DBValue[] { DBValue.Create('_') };
            var args = new ICreator[] { arg0creator, arg1creator, arg2creator };
            var creator = new ConstructingCreator(ctor, args, true);
            var value = creator.CreateFrom(row);

            // Assert
            creator.ResultType.Should().Be(typeof(Tuple<string, float, DateTime>));
            value.Should().BeOfType<Tuple<string, float, DateTime>>();
            (value as Tuple<string, float, DateTime>)!.Item1.Should().Be(first);
            (value as Tuple<string, float, DateTime>)!.Item2.Should().Be(second);
            (value as Tuple<string, float, DateTime>)!.Item3.Should().Be(third);
            arg0creator.Received().CreateFrom(row);
            arg1creator.Received().CreateFrom(row);
            arg2creator.Received().CreateFrom(row);
        }

        [TestMethod] public void ConstructFromSomeNulls() {
            // Arrange
            var publicKeyToken = typeof(ICreator).Assembly.GetName().GetPublicKeyToken();
            var name = "Caesarea";
            var version = new Version(7, 3, 19);
            var ctor = typeof(ApplicationId).GetConstructors()[0];
            var arg0creator = Substitute.For<ICreator>();
            arg0creator.ResultType.Returns(typeof(byte[]));
            arg0creator.CreateFrom(Arg.Any<IReadOnlyList<DBValue>>()).Returns(publicKeyToken);
            var arg1creator = Substitute.For<ICreator>();
            arg1creator.ResultType.Returns(typeof(string));
            arg1creator.CreateFrom(Arg.Any<IReadOnlyList<DBValue>>()).Returns(name);
            var arg2creator = Substitute.For<ICreator>();
            arg2creator.ResultType.Returns(typeof(Version));
            arg2creator.CreateFrom(Arg.Any<IReadOnlyList<DBValue>>()).Returns(version);
            var arg3creator = Substitute.For<ICreator>();
            arg3creator.ResultType.Returns(typeof(string));
            arg3creator.CreateFrom(Arg.Any<IReadOnlyList<DBValue>>()).Returns(null);
            var arg4creator = Substitute.For<ICreator>();
            arg4creator.ResultType.Returns(typeof(string));
            arg4creator.CreateFrom(Arg.Any<IReadOnlyList<DBValue>>()).Returns(null);

            // Act
            var row = new DBValue[] { DBValue.Create("Caesarea"), DBValue.Create("Monte Carlo") };
            var args = new ICreator[] { arg0creator, arg1creator, arg2creator, arg3creator, arg4creator };
            var creator = new ConstructingCreator(ctor, args, false);
            var value = creator.CreateFrom(row);

            // Assert
            creator.ResultType.Should().Be(typeof(ApplicationId));
            value.Should().BeOfType<ApplicationId>();
            (value as ApplicationId)!.PublicKeyToken.Should().BeEquivalentTo(publicKeyToken);
            (value as ApplicationId)!.Name.Should().Be(name);
            (value as ApplicationId)!.Version.Should().Be(version);
            (value as ApplicationId)!.ProcessorArchitecture.Should().BeNull();
            (value as ApplicationId)!.Culture.Should().BeNull();
            arg0creator.Received().CreateFrom(row);
            arg1creator.Received().CreateFrom(row);
            arg2creator.Received().CreateFrom(row);
            arg3creator.Received().CreateFrom(row);
        }

        [TestMethod] public void ConstructFromAllNullRowAllowed() {
            // Arrange
            var ctor = typeof(Tuple<string?, DateTime?, uint?>).GetConstructors()[0];
            var arg0creator = Substitute.For<ICreator>();
            arg0creator.ResultType.Returns(typeof(string));
            arg0creator.CreateFrom(Arg.Any<IReadOnlyList<DBValue>>()).Returns(null);
            var arg1creator = Substitute.For<ICreator>();
            arg1creator.ResultType.Returns(typeof(DateTime?));
            arg1creator.CreateFrom(Arg.Any<IReadOnlyList<DBValue>>()).Returns(null);
            var arg2creator = Substitute.For<ICreator>();
            arg2creator.ResultType.Returns(typeof(uint?));
            arg2creator.CreateFrom(Arg.Any<IReadOnlyList<DBValue>>()).Returns(null);

            // Act
            var row = new DBValue[] { DBValue.NULL };
            var args = new ICreator[] { arg0creator, arg1creator, arg2creator };
            var creator = new ConstructingCreator(ctor, args, true);
            var value = creator.CreateFrom(row);

            // Assert
            creator.ResultType.Should().Be(typeof(Tuple<string?, DateTime?, uint?>));
            value.Should().BeOfType<Tuple<string?, DateTime?, uint?>>();
            (value as Tuple<string?, DateTime?, uint?>)!.Item1.Should().BeNull();
            (value as Tuple<string?, DateTime?, uint?>)!.Item2.Should().BeNull();
            (value as Tuple<string?, DateTime?, uint?>)!.Item3.Should().BeNull();
            arg0creator.Received().CreateFrom(row);
            arg1creator.Received().CreateFrom(row);
            arg2creator.Received().CreateFrom(row);
        }

        [TestMethod] public void ConstructFromAllNullRowDisallowed() {
            // Arrange
            var ctor = typeof(ApplicationException).GetConstructors()[1];
            var arg0creator = Substitute.For<ICreator>();
            arg0creator.ResultType.Returns(typeof(string));
            arg0creator.CreateFrom(Arg.Any<IReadOnlyList<DBValue>>()).Returns(null);

            // Act
            var row = new DBValue[] { DBValue.NULL, DBValue.NULL, DBValue.NULL };
            var args = new ICreator[] { arg0creator };
            var creator = new ConstructingCreator(ctor, args, false);
            var value = creator.CreateFrom(row);

            // Assert
            creator.ResultType.Should().Be(typeof(ApplicationException));
            value.Should().BeNull();
            arg0creator.DidNotReceive().CreateFrom(Arg.Any<IReadOnlyList<DBValue>>());
        }

        [TestMethod] public void ConstructFromAllNullRowButNotAllNullArguments() {
            // Arrange
            var ticks = 974987124024566;
            var ctor = typeof(DateTime).GetConstructors()[0];
            var arg0creator = Substitute.For<ICreator>();
            arg0creator.ResultType.Returns(typeof(long));
            arg0creator.CreateFrom(Arg.Any<IReadOnlyList<DBValue>>()).Returns(ticks);

            // Act
            var row = new DBValue[] { DBValue.NULL, DBValue.NULL, DBValue.NULL, DBValue.NULL };
            var args = new ICreator[] { arg0creator };
            var creator = new ConstructingCreator(ctor, args, true);
            var value = creator.CreateFrom(row);

            // Assert
            creator.ResultType.Should().Be(typeof(DateTime));
            value.Should().BeOfType<DateTime>();
            ((DateTime)value!).Ticks.Should().Be(ticks);
            arg0creator.Received().CreateFrom(row);
        }

        [TestMethod] public void NonPublicConstructor() {
            // Arrange
            var properties = new List<PropertyInfo>() { typeof(Type).GetProperty("Name")! };
            var ctor = typeof(PropertyChain).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)[0];
            var arg0creator = Substitute.For<ICreator>();
            arg0creator.ResultType.Returns(typeof(List<PropertyInfo>));
            arg0creator.CreateFrom(Arg.Any<IReadOnlyList<DBValue>>()).Returns(properties);

            // Act
            var row = new DBValue[] { DBValue.Create("Detroit"), DBValue.Create("Richmond") };
            var args = new ICreator[] { arg0creator };
            var creator = new ConstructingCreator(ctor, args, false);
            var value = creator.CreateFrom(row);

            // Assert
            creator.ResultType.Should().Be(typeof(PropertyChain));
            value.Should().BeOfType<PropertyChain>();
            (value as PropertyChain)!.Length.Should().Be(properties.Count);
            (value as PropertyChain)!.GetValue(typeof(NetPipeStyleUriParser)).Should().Be(typeof(NetPipeStyleUriParser).Name);
            arg0creator.Received().CreateFrom(row);
        }
    }

    [TestClass, TestCategory("DefaultStructCreator")]
    public class DefaultStructCreatorTests {
        [TestMethod] public void DefaultStruct() {
            // Arrange
            var type = typeof(Point);

            // Act
            var row = new DBValue[] { DBValue.Create("Cologne"), DBValue.Create("Siem Reap") };
            var creator = new DefaultStructCreator(type, false);
            var value = creator.CreateFrom(row);

            // Assert
            creator.ResultType.Should().Be(type);
            value.Should().BeOfType<Point>();
            ((Point)value!).IsEmpty.Should().BeTrue();
        }

        [TestMethod] public void ConstructFromSomeNulls() {
            // Arrange
            var type = typeof(Point);

            // Act
            var row = new DBValue[] { DBValue.Create("Chittagong"), DBValue.NULL };
            var creator = new DefaultStructCreator(type, false);
            var value = creator.CreateFrom(row);

            // Assert
            creator.ResultType.Should().Be(type);
            value.Should().BeOfType<Point>();
            ((Point)value!).IsEmpty.Should().BeTrue();
        }

        [TestMethod] public void ConstructFromAllNullRowAllowed() {
            // Arrange
            var type = typeof(Point);

            // Act
            var row = new DBValue[] { DBValue.NULL, DBValue.NULL };
            var creator = new DefaultStructCreator(type, true);
            var value = creator.CreateFrom(row);

            // Assert
            creator.ResultType.Should().Be(type);
            value.Should().BeOfType<Point>();
            ((Point)value!).IsEmpty.Should().BeTrue();
        }

        [TestMethod] public void ConstructFromAllNullRowDisallowed() {
            // Arrange
            var type = typeof(Point);

            // Act
            var row = new DBValue[] { DBValue.NULL, DBValue.NULL };
            var creator = new DefaultStructCreator(type, false);
            var value = creator.CreateFrom(row);

            // Assert
            creator.ResultType.Should().Be(type);
            value.Should().BeNull();
        }
    }

    [TestClass, TestCategory("ReconstitutingCreator")]
    public class ReconstitutingCreatorTests {
        [TestMethod] public void NoMutations() {
            // Arrange
            var str = "Taos Pueblo";
            var originalCreator = Substitute.For<ICreator>();
            originalCreator.ResultType.Returns(typeof(string));
            originalCreator.CreateFrom(Arg.Any<IReadOnlyList<DBValue>>()).Returns(str);

            // Act
            var row = new DBValue[] { DBValue.Create("Hackensack") };
            var creator = new ReconstitutingCreator(originalCreator, Array.Empty<IMutator>());
            var value = creator.CreateFrom(row);

            // Assert
            creator.ResultType.Should().Be(typeof(string));
            value.Should().Be(str);
        }

        [TestMethod] public void OneMutation() {
            // Arrange
            var str = "Victoria";
            var originalCreator = Substitute.For<ICreator>();
            originalCreator.ResultType.Returns(typeof(string));
            originalCreator.CreateFrom(Arg.Any<IReadOnlyList<DBValue>>()).Returns(str);
            var mutator = Substitute.For<IMutator>();
            mutator.SourceType.Returns(typeof(string));

            // Act
            var row = new DBValue[] { DBValue.Create("Corfu"), DBValue.NULL, DBValue.Create(1859) };
            var mutators = new IMutator[] { mutator };
            var creator = new ReconstitutingCreator(originalCreator, mutators);
            var value = creator.CreateFrom(row);

            // Assert
            creator.ResultType.Should().Be(typeof(string));
            value.Should().Be(str);
            mutator.Received().Mutate(value, row);
        }

        [TestMethod] public void MultipleMutations() {
            // Arrange
            var type = typeof(Math);
            var originalCreator = Substitute.For<ICreator>();
            originalCreator.ResultType.Returns(typeof(Type));
            originalCreator.CreateFrom(Arg.Any<IReadOnlyList<DBValue>>()).Returns(type);
            var mutator0 = Substitute.For<IMutator>();
            mutator0.SourceType.Returns(typeof(Type));
            var mutator1 = Substitute.For<IMutator>();
            mutator1.SourceType.Returns(typeof(Type));
            var mutator2 = Substitute.For<IMutator>();
            mutator2.SourceType.Returns(typeof(Type));

            // Act
            var row = new DBValue[] { DBValue.Create(true), DBValue.Create((sbyte)-3), DBValue.Create("Cluj-Napoca") };
            var mutators = new IMutator[] { mutator0, mutator1, mutator2 };
            var creator = new ReconstitutingCreator(originalCreator, mutators);
            var value = creator.CreateFrom(row);

            // Assert
            creator.ResultType.Should().Be(typeof(Type));
            value.Should().Be(type);
            Received.InOrder(() => { mutator0.Mutate(value, row); mutator1.Mutate(value, row); mutator2.Mutate(value, row); });
        }
    }

    [TestClass, TestCategory("KeyLookupCreator")]
    public class KeyLookupCreatorTests {
        [TestMethod] public void LookupMatchesFirstOption() {
            // Arrange
            var options = new List<string>() { "Hilo", "Eugene", "Davidson", "Fairfax" };
            var keys = new List<List<object?>>() { new() { 100 }, new() { 200 }, new() { 300 }, new() { 400 } };
            var keyExtractor = Substitute.For<IMultiExtractor>();
            keyExtractor.SourceType.Returns(typeof(string));
            keyExtractor.ExtractFrom(options[0]).Returns(keys[0]);
            keyExtractor.ExtractFrom(options[1]).Returns(keys[1]);
            keyExtractor.ExtractFrom(options[2]).Returns(keys[2]);
            keyExtractor.ExtractFrom(options[3]).Returns(keys[3]);
            var keyPlan = new DataExtractionPlan(new IMultiExtractor[] { keyExtractor });

            // Act
            var key = keys[0];
            var matcher = new KeyMatcher(() => options, keyPlan);
            var creator = new KeyLookupCreator(matcher);
            var value = creator.CreateFrom(key.Select(v => DBValue.Create(v)).ToList());

            // Assert
            matcher.ResultType.Should().Be(keyPlan.SourceType);
            creator.ResultType.Should().Be(matcher.ResultType);
            value.Should().Be(options[0]);
            keyExtractor.Received().ExtractFrom(options[0]);
            keyExtractor.DidNotReceive().ExtractFrom(options[1]);
            keyExtractor.DidNotReceive().ExtractFrom(options[2]);
            keyExtractor.DidNotReceive().ExtractFrom(options[3]);
        }

        [TestMethod] public void LookupMatchesIntermediateOption() {
            // Arrange
            var options = new List<NetTcpStyleUriParser>() { new(), new(), new(), new() };
            var keys = new List<List<object?>>() { new() { 'X', 5L }, new() { 'X', 4L }, new() { 'X', 9L }, new() { 'P', -7L } };
            var keyExtractor = Substitute.For<IMultiExtractor>();
            keyExtractor.SourceType.Returns(typeof(NetTcpStyleUriParser));
            keyExtractor.ExtractFrom(options[0]).Returns(keys[0]);
            keyExtractor.ExtractFrom(options[1]).Returns(keys[1]);
            keyExtractor.ExtractFrom(options[2]).Returns(keys[2]);
            keyExtractor.ExtractFrom(options[3]).Returns(keys[3]);
            var keyPlan = new DataExtractionPlan(new IMultiExtractor[] { keyExtractor });

            // Act
            var key = keys[1];
            var matcher = new KeyMatcher(() => options, keyPlan);
            var creator = new KeyLookupCreator(matcher);
            var value = creator.CreateFrom(key.Select(v => DBValue.Create(v)).ToList());

            // Assert
            matcher.ResultType.Should().Be(keyPlan.SourceType);
            creator.ResultType.Should().Be(matcher.ResultType);
            value.Should().Be(options[1]);
            keyExtractor.Received().ExtractFrom(options[0]);
            keyExtractor.Received().ExtractFrom(options[1]);
            keyExtractor.DidNotReceive().ExtractFrom(options[2]);
            keyExtractor.DidNotReceive().ExtractFrom(options[3]);
        }

        [TestMethod] public void LookupMatchesFinalOption() {
            // Arrange
            var options = new List<NullReferenceException>() { new(), new(), new() };
            var keys = new List<List<object?>>() { new() { "Bissau" }, new() { "Gladstone" }, new() { "Eugene" } };
            var keyExtractor = Substitute.For<IMultiExtractor>();
            keyExtractor.SourceType.Returns(typeof(NullReferenceException));
            keyExtractor.ExtractFrom(options[0]).Returns(keys[0]);
            keyExtractor.ExtractFrom(options[1]).Returns(keys[1]);
            keyExtractor.ExtractFrom(options[2]).Returns(keys[2]);
            var keyPlan = new DataExtractionPlan(new IMultiExtractor[] { keyExtractor });

            // Act
            var key = keys[^1];
            var matcher = new KeyMatcher(() => options, keyPlan);
            var creator = new KeyLookupCreator(matcher);
            var value = creator.CreateFrom(key.Select(v => DBValue.Create(v)).ToList());

            // Assert
            matcher.ResultType.Should().Be(keyPlan.SourceType);
            creator.ResultType.Should().Be(matcher.ResultType);
            value.Should().Be(options[^1]);
            keyExtractor.Received().ExtractFrom(options[0]);
            keyExtractor.Received().ExtractFrom(options[1]);
            keyExtractor.Received().ExtractFrom(options[2]);
        }

        [TestMethod] public void LookupMatchesPreviouslyMatchedOption() {
            // Arrange
            var options = new List<string>() { "Pune", "Kikuyu", "Hiroshima", "Da Nang" };
            var keys = new List<List<object?>>() { new() { true, false }, new() { true, true }, new() { false, false }, new() { false, true } };
            var keyExtractor = Substitute.For<IMultiExtractor>();
            keyExtractor.SourceType.Returns(typeof(string));
            keyExtractor.ExtractFrom(options[0]).Returns(keys[0]);
            keyExtractor.ExtractFrom(options[1]).Returns(keys[1]);
            keyExtractor.ExtractFrom(options[2]).Returns(keys[2]);
            keyExtractor.ExtractFrom(options[3]).Returns(keys[3]);
            var keyPlan = new DataExtractionPlan(new IMultiExtractor[] { keyExtractor });

            // Act
            var key = keys[1];
            var matcher = new KeyMatcher(() => options, keyPlan);
            var creator = new KeyLookupCreator(matcher);
            var firstValue = creator.CreateFrom(key.Select(v => DBValue.Create(v)).ToList());
            var secondValue = creator.CreateFrom(key.Select(v => DBValue.Create(v)).ToList());

            // Assert
            matcher.ResultType.Should().Be(keyPlan.SourceType);
            creator.ResultType.Should().Be(matcher.ResultType);
            firstValue.Should().Be(options[1]);
            secondValue.Should().Be(firstValue);
            keyExtractor.Received(1).ExtractFrom(options[0]);
            keyExtractor.Received(1).ExtractFrom(options[1]);
            keyExtractor.DidNotReceive().ExtractFrom(options[2]);
            keyExtractor.DidNotReceive().ExtractFrom(options[3]);
        }

        [TestMethod] public void LookupTwoDifferentValues() {
            // Arrange
            var options = new List<string>() { "Chengdu", "Neuquén", "Beersheba" };
            var keys = new List<List<object?>>() { new() { '0', '1', '2' }, new() { '3', '4', '5' }, new() { '6', '7', '8' } };
            var keyExtractor = Substitute.For<IMultiExtractor>();
            keyExtractor.SourceType.Returns(typeof(string));
            keyExtractor.ExtractFrom(options[0]).Returns(keys[0]);
            keyExtractor.ExtractFrom(options[1]).Returns(keys[1]);
            keyExtractor.ExtractFrom(options[2]).Returns(keys[2]);
            var keyPlan = new DataExtractionPlan(new IMultiExtractor[] { keyExtractor });

            // Act
            var firstKey = keys[1];
            var secondKey = keys[2];
            var matcher = new KeyMatcher(() => options, keyPlan);
            var creator = new KeyLookupCreator(matcher);
            var firstValue = creator.CreateFrom(firstKey.Select(v => DBValue.Create(v)).ToList());
            var secondValue = creator.CreateFrom(secondKey.Select(v => DBValue.Create(v)).ToList());

            // Assert
            matcher.ResultType.Should().Be(keyPlan.SourceType);
            creator.ResultType.Should().Be(matcher.ResultType);
            firstValue.Should().Be(options[1]);
            secondValue.Should().Be(options[2]);
            keyExtractor.Received(1).ExtractFrom(options[0]);
            keyExtractor.Received(1).ExtractFrom(options[1]);
            keyExtractor.Received(1).ExtractFrom(options[2]);
        }

        [TestMethod] public void LookupNullKey() {
            // Arrange
            var options = new List<string>() { "Lelydorp", "Shizuoka", "Trieste", "Mississauga" };
            var keyExtractor = Substitute.For<IMultiExtractor>();
            var keyPlan = new DataExtractionPlan(new IMultiExtractor[] { keyExtractor });

            // Act
            var matcher = new KeyMatcher(() => options, keyPlan);
            var creator = new KeyLookupCreator(matcher);
            var value = creator.CreateFrom(new List<DBValue>() { DBValue.NULL, DBValue.NULL });

            // Assert
            matcher.ResultType.Should().Be(keyPlan.SourceType);
            creator.ResultType.Should().Be(matcher.ResultType);
            value.Should().BeNull();
            keyExtractor.DidNotReceive().ExtractFrom(Arg.Any<object?>());
        }
    }

    [TestClass, TestCategory("CreatorFacade")]
    public class CreatorFacadeTests {
        [TestMethod] public void FacadeOverFullList() {
            // Arrange
            var reader = new BinaryReader(new MemoryStream());
            var creator = Substitute.For<ICreator>();
            creator.ResultType.Returns(typeof(BinaryReader));
            creator.CreateFrom(Arg.Any<IReadOnlyList<DBValue>>()).Returns(reader);

            // Act
            var row = new DBValue[] { DBValue.Create('u'), DBValue.Create(false), DBValue.Create("Iqaluit") };
            var facade = new CreatorFacade(creator, 0, row.Length);
            var value = facade.CreateFrom(row);

            // Assert
            facade.ResultType.Should().Be(creator.ResultType);
            value.Should().Be(reader);
            var slice = row[0..row.Length];
            creator.Received().CreateFrom(Arg.Is<IReadOnlyList<DBValue>>(r => r.SequenceEqual(slice)));
        }

        [TestMethod] public void FacadeOverPrefix() {
            // Arrange
            var attribute = new ContextStaticAttribute();
            var creator = Substitute.For<ICreator>();
            creator.ResultType.Returns(typeof(ContextStaticAttribute));
            creator.CreateFrom(Arg.Any<IReadOnlyList<DBValue>>()).Returns(attribute);

            // Act
            var row = new DBValue[] { DBValue.Create(17), DBValue.Create(1999), DBValue.Create(new Guid()) };
            var facade = new CreatorFacade(creator, 0, 1);
            var value = facade.CreateFrom(row);

            // Assert
            facade.ResultType.Should().Be(creator.ResultType);
            value.Should().Be(attribute);
            var slice = row[0..1];
            creator.Received().CreateFrom(Arg.Is<IReadOnlyList<DBValue>>(r => r.SequenceEqual(slice)));
        }

        [TestMethod] public void FacadeOverInfix() {
            // Arrange
            var name = new FieldName("FIELD");
            var creator = Substitute.For<ICreator>();
            creator.ResultType.Returns(typeof(FieldName));
            creator.CreateFrom(Arg.Any<IReadOnlyList<DBValue>>()).Returns(name);

            // Act
            var row = new DBValue[] { DBValue.Create(-1U), DBValue.Create(DateTime.Now), DBValue.Create(new Guid()), DBValue.Create('f') };
            var facade = new CreatorFacade(creator, 1, 2);
            var value = facade.CreateFrom(row);

            // Assert
            facade.ResultType.Should().Be(creator.ResultType);
            value.Should().Be(name);
            var slice = row[1..3];
            creator.Received().CreateFrom(Arg.Is<IReadOnlyList<DBValue>>(r => r.SequenceEqual(slice)));
        }

        [TestMethod] public void FacadeOverSufix() {
            // Arrange
            var colors = new ColorConverter();
            var creator = Substitute.For<ICreator>();
            creator.ResultType.Returns(typeof(ColorConverter));
            creator.CreateFrom(Arg.Any<IReadOnlyList<DBValue>>()).Returns(colors);

            // Act
            var row = new DBValue[] { DBValue.Create("Ogden"), DBValue.Create(9.6f), DBValue.Create((ushort)37), DBValue.Create(100M), DBValue.Create(false) };
            var facade = new CreatorFacade(creator, 2, 3);
            var value = facade.CreateFrom(row);

            // Assert
            facade.ResultType.Should().Be(creator.ResultType);
            value.Should().Be(colors);
            var slice = row[2..5];
            creator.Received().CreateFrom(Arg.Is<IReadOnlyList<DBValue>>(r => r.SequenceEqual(slice)));
        }
    }
}
