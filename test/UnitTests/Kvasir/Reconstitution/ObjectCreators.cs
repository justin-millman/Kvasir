using Cybele.Core;
using FluentAssertions;
using Kvasir.Extraction;
using Kvasir.Reconstitution;
using Kvasir.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;

namespace UT.Kvasir.Reconstitution {
    [TestClass, TestCategory("PrimitiveCreator")]
    public class PrimitiveCreatorTests {
        [TestMethod] public void Construct() {
            // Arrange
            var idx = new Index(3);

            // Act
            var creator = new PrimitiveCreator(idx, typeof(int));

            // Assert
            creator.Target.Should().Be(typeof(int));
        }

        [TestMethod] public void ProduceNonNull() {
            // Arrange
            var idx = new Index(1);
            var data = new DBValue[] { DBValue.Create(7), DBValue.Create("Jefferson City") };
            var creator = new PrimitiveCreator(idx, typeof(string));

            // Act
            var value = creator.Execute(data);

            // Assert
            value.Should().Be(data[idx].Datum);
        }

        [TestMethod] public void ProduceNull() {
            // Arrange
            var idx = new Index(1);
            var data = new DBValue[] { DBValue.Create('&'), DBValue.NULL, DBValue.Create(-4L) };
            var creator = new PrimitiveCreator(idx, typeof(ushort));

            // Act
            var value = creator.Execute(data);

            // Assert
            value.Should().BeNull();
        }
    }

    [TestClass, TestCategory("ByConstructorCreator")]
    public class ByConstructorCreatorTests {
        [TestMethod] public void Construct() {
            // Arrange
            var ctor = typeof(string).GetConstructor(new Type[] { typeof(char), typeof(int) })!;
            var mockArgRecon = Substitute.For<IReconstitutor>();
            mockArgRecon.Target.Returns(typeof(char), typeof(int));

            // Act
            var creator = new ByConstructorCreator(ctor, new IReconstitutor[] { mockArgRecon }, false);

            // Assert
            creator.Target.Should().Be(typeof(string));
        }

        [TestMethod] public void ProduceFromSingleNonNullArgument() {
            // Arrange
            var ctor = typeof(TestCategoryAttribute).GetConstructor(new Type[] { typeof(string) })!;
            var arg = "Fort Lauderdale";
            var mockArgRecon = Substitute.For<IReconstitutor>();
            mockArgRecon.Target.Returns(typeof(string));
            mockArgRecon.ReconstituteFrom(Arg.Any<IReadOnlyList<DBValue>>()).Returns(arg);
            var data = new DBValue[] { DBValue.NULL, DBValue.Create(35L) };
            var creator = new ByConstructorCreator(ctor, new IReconstitutor[] { mockArgRecon }, false);

            // Act
            var value = creator.Execute(data);

            // Assert
            var expected = new TestCategoryAttribute(arg);
            value.Should().BeOfType<TestCategoryAttribute>();
            (value as TestCategoryAttribute)!.TestCategories.Should().Equal(expected.TestCategories);
        }

        [TestMethod] public void ProduceFromMultipleNonNullArguments() {
            // Arrange
            var ctor = typeof(System.Range).GetConstructor(new Type[] { typeof(Index), typeof(Index) })!;
            var arg0 = new Index(95);
            var arg1 = new Index(2074);
            var mockArgRecon0 = Substitute.For<IReconstitutor>();
            mockArgRecon0.Target.Returns(typeof(Index));
            mockArgRecon0.ReconstituteFrom(Arg.Any<IReadOnlyList<DBValue>>()).Returns(arg0);
            var mockArgRecon1 = Substitute.For<IReconstitutor>();
            mockArgRecon1.Target.Returns(typeof(Index));
            mockArgRecon1.ReconstituteFrom(Arg.Any<IReadOnlyList<DBValue>>()).Returns(arg1);
            var recons = new IReconstitutor[] { mockArgRecon0, mockArgRecon1 };
            var data = new DBValue[] { DBValue.Create('|'), DBValue.Create('>'), DBValue.Create("Eugene") };
            var creator = new ByConstructorCreator(ctor, recons, false);

            // Act
            var value = creator.Execute(data);

            // Assert
            var expected = new System.Range(arg0, arg1);
            value.Should().Be(expected);
        }

        [TestMethod] public void ProduceRequiredObjectFromAllNulls() {
            // Arrange
            var ctor = typeof(ArgumentException).GetConstructor(new Type[] { typeof(string), typeof(Exception) })!;
            var mockArgRecon0 = Substitute.For<IReconstitutor>();
            mockArgRecon0.Target.Returns(typeof(string));
            mockArgRecon0.ReconstituteFrom(Arg.Any<IReadOnlyList<DBValue>>()).Returns(null);
            var mockArgRecon1 = Substitute.For<IReconstitutor>();
            mockArgRecon1.Target.Returns(typeof(Exception));
            mockArgRecon1.ReconstituteFrom(Arg.Any<IReadOnlyList<DBValue>>()).Returns(null);
            var recons = new IReconstitutor[] { mockArgRecon0, mockArgRecon1 };
            var data = new DBValue[] { DBValue.Create('%') };
            var creator = new ByConstructorCreator(ctor, recons, false);

            // Act
            var value = creator.Execute(data);

            // Assert
            var expected = new ArgumentException(null, (Exception?)null);
            value.Should().BeEquivalentTo(expected);
        }

        [TestMethod] public void ProduceOptionalObjectFromAllNulls() {
            // Arrange
            var ctor = typeof(DateTime).GetConstructor(new Type[] { typeof(int), typeof(int), typeof(int) })!;
            var mockArgRecon = Substitute.For<IReconstitutor>();
            mockArgRecon.Target.Returns(typeof(int));
            mockArgRecon.ReconstituteFrom(Arg.Any<IReadOnlyList<DBValue>>()).Returns(null);
            var recons = new IReconstitutor[] { mockArgRecon, mockArgRecon, mockArgRecon };
            var data = new DBValue[] { DBValue.NULL, DBValue.NULL, DBValue.NULL };
            var creator = new ByConstructorCreator(ctor, recons, true);

            // Act
            var value = creator.Execute(data);

            // Assert
            value.Should().BeNull();
        }

        [TestMethod] public void ProduceObjectFromSomeNulls() {
            // Arrange
            var ctor = typeof(KeyValuePair<int, string?>).GetConstructor(new Type[] { typeof(int), typeof(string) })!;
            var arg0 = 381;
            string? arg1 = null;
            var mockArgRecon0 = Substitute.For<IReconstitutor>();
            mockArgRecon0.Target.Returns(typeof(int));
            mockArgRecon0.ReconstituteFrom(Arg.Any<IReadOnlyList<DBValue>>()).Returns(arg0);
            var mockArgRecon1 = Substitute.For<IReconstitutor>();
            mockArgRecon1.Target.Returns(typeof(string));
            mockArgRecon1.ReconstituteFrom(Arg.Any<IReadOnlyList<DBValue>>()).Returns(arg1);
            var recons = new IReconstitutor[] { mockArgRecon0, mockArgRecon1 };
            var data = new DBValue[] { DBValue.NULL, DBValue.NULL, DBValue.Create('=') };
            var optCreator = new ByConstructorCreator(ctor, recons, true);
            var reqCreator = new ByConstructorCreator(ctor, recons, false);

            // Act
            var optValue = optCreator.Execute(data);
            var reqValue = reqCreator.Execute(data);

            // Assert
            var expected = new KeyValuePair<int, string?>(arg0, arg1);
            optValue.Should().Be(expected);
            reqValue.Should().Be(expected);
        }
    }

    [TestClass, TestCategory("ByKeyLookupCreator")]
    public class ByKeyLookupCreatorTests {
        [TestMethod] public void Construct() {
            // Arrange
            var conv = DataConverter.Identity<string>();
            var step = new PrimitiveExtractionStep(new IdentityExtractor<string>());
            var plan = new DataExtractionPlan(new IExtractionStep[] { step }, new DataConverter[] { conv });

            // Act
            var creator = new ByKeyLookupCreator(() => Array.Empty<string>(), plan);

            // Assert
            creator.Target.Should().Be(typeof(string));
        }

        [TestMethod] public void ProduceFromNonNullKey() {
            // Arrange
            var conv = DataConverter.Identity<string>();
            var step = new PrimitiveExtractionStep(new IdentityExtractor<string>());
            var plan = new DataExtractionPlan(new IExtractionStep[] { step, step }, new DataConverter[] { conv, conv });
            var entities = new string[] { "Belo Horizonte", "Gladstone", "Cluj-Napoca" };
            var target = entities[2];
            var data = new DBValue[] { DBValue.Create(target), DBValue.Create(target) };
            var creator = new ByKeyLookupCreator(() => entities, plan);

            // Act
            var value = creator.Execute(data);

            // Assert
            value.Should().Be(target);
        }

        [TestMethod] public void ProduceFromNullKey() {
            // Arrange
            var conv = DataConverter.Identity<string>();
            var step = new PrimitiveExtractionStep(new IdentityExtractor<string>());
            var plan = new DataExtractionPlan(new IExtractionStep[] { step, step }, new DataConverter[] { conv, conv });
            var entities = new string[] { "Whanganui", "Valladolid", "Chișinău" };
            var target = entities[0];
            var data = new DBValue[] { DBValue.NULL, DBValue.NULL };
            var creator = new ByKeyLookupCreator(() => entities, plan);

            // Act
            var value = creator.Execute(data);

            // Assert
            value.Should().BeNull();
        }
    }
}
