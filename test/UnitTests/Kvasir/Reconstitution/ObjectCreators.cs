using FluentAssertions;
using Cybele.Core;
using Kvasir.Reconstitution;
using Kvasir.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;

namespace UT.Kvasir.Reconstitution {
    [TestClass, TestCategory("PrimitiveCreator")]
    public class PrimitiveCreatorTests {
        [TestMethod] public void ConstructNoReversion() {
            // Arrange
            var idx = new Index(3);
            var reverter = DataConverter.Identity<int>();

            // Act
            var creator = new PrimitiveCreator(idx, reverter);

            // Assert
            creator.Target.Should().Be(typeof(int));
        }

        [TestMethod] public void ConstructWithReversion() {
            // Arrange
            var idx = new Index(491);
            var reverter = DataConverter.Create<char, int>(c => c, i => (char)i);

            // Act
            var creator = new PrimitiveCreator(idx, reverter);

            // Assert
            creator.Target.Should().Be(typeof(char));
        }

        [TestMethod] public void ProduceNonNullNoReversion() {
            // Arrange
            var idx = new Index(1);
            var reverter = DataConverter.Identity<string>();
            var data = new DBValue[] { DBValue.Create(7), DBValue.Create("Jefferson City") };
            var creator = new PrimitiveCreator(idx, reverter);

            // Act
            var value = creator.Execute(data);

            // Assert
            value.Should().Be(data[idx].Datum);
        }

        [TestMethod] public void ProduceNonNullWithReversion() {
            // Arrange
            var idx = new Index(0);
            var reverter = DataConverter.Create<int, string>(i => i.ToString(), s => int.Parse(s));
            var data = new DBValue[] { DBValue.Create("7"), DBValue.NULL, DBValue.Create(7) };
            var creator = new PrimitiveCreator(idx, reverter);

            // Act
            var value = creator.Execute(data);

            // Assert
            value.Should().Be(7);
        }

        [TestMethod] public void ProduceNullNoReversion() {
            // Arrange
            var idx = new Index(1);
            var reverter = DataConverter.Identity<ushort>();
            var data = new DBValue[] { DBValue.Create('&'), DBValue.NULL, DBValue.Create(-4L) };
            var creator = new PrimitiveCreator(idx, reverter);

            // Act
            var value = creator.Execute(data);

            // Assert
            value.Should().BeNull();
        }

        [TestMethod] public void ProduceNullWithReversion() {
            // Arrange
            var idx = new Index(1);
            var reverter = DataConverter.Create<DateTime, int>(d => d.Year, i => new DateTime(i, 1, 1));
            var data = new DBValue[] { DBValue.Create("Bloomington"), DBValue.NULL };
            var creator = new PrimitiveCreator(idx, reverter);

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
            var mockArgRecon = new Mock<IReconstitutor>();
            mockArgRecon.SetupSequence(r => r.Target).Returns(typeof(char)).Returns(typeof(int));

            // Act
            var creator = new ByConstructorCreator(ctor, new IReconstitutor[] { mockArgRecon.Object });

            // Assert
            creator.Target.Should().Be(typeof(string));
        }

        [TestMethod] public void ProduceFromSingleArgument() {
            // Arrange
            var ctor = typeof(TestCategoryAttribute).GetConstructor(new Type[] { typeof(string) })!;
            var arg = "Fort Lauderdale";
            var mockArgRecon = new Mock<IReconstitutor>();
            mockArgRecon.Setup(r => r.Target).Returns(typeof(string));
            mockArgRecon.Setup(r => r.ReconstituteFrom(It.IsAny<IReadOnlyList<DBValue>>())).Returns(arg);
            var data = new DBValue[] { DBValue.NULL, DBValue.Create(35L) };
            var creator = new ByConstructorCreator(ctor, new IReconstitutor[] { mockArgRecon.Object });

            // Act
            var value = creator.Execute(data);

            // Assert
            var expected = new TestCategoryAttribute(arg);
            value.Should().Equals(expected);
        }

        [TestMethod] public void ProduceFromMultipleArguments() {
            // Arrange
            var ctor = typeof(System.Range).GetConstructor(new Type[] { typeof(Index), typeof(Index) })!;
            var arg0 = new Index(95);
            var arg1 = new Index(2074);
            var mockArgRecon0 = new Mock<IReconstitutor>();
            mockArgRecon0.Setup(r => r.Target).Returns(typeof(Index));
            mockArgRecon0.Setup(r => r.ReconstituteFrom(It.IsAny<IReadOnlyList<DBValue>>())).Returns(arg0);
            var mockArgRecon1 = new Mock<IReconstitutor>();
            mockArgRecon1.Setup(r => r.Target).Returns(typeof(Index));
            mockArgRecon1.Setup(r => r.ReconstituteFrom(It.IsAny<IReadOnlyList<DBValue>>())).Returns(arg1);
            var recons = new IReconstitutor[] { mockArgRecon0.Object, mockArgRecon1.Object };
            var data = new DBValue[] { DBValue.Create('|'), DBValue.Create('>'), DBValue.Create("Eugene") };
            var creator = new ByConstructorCreator(ctor, recons);

            // Act
            var value = creator.Execute(data);

            // Assert
            var expected = new System.Range(arg0, arg1);
            value.Should().Equals(expected);
        }

        [TestMethod] public void ProduceFromAllNulls() {
            // Arrange
            var ctor = typeof(ArgumentException).GetConstructor(new Type[] { typeof(string), typeof(Exception) })!;
            var mockArgRecon0 = new Mock<IReconstitutor>();
            mockArgRecon0.Setup(r => r.Target).Returns(typeof(string));
            mockArgRecon0.Setup(r => r.ReconstituteFrom(It.IsAny<IReadOnlyList<DBValue>>())).Returns(null);
            var mockArgRecon1 = new Mock<IReconstitutor>();
            mockArgRecon1.Setup(r => r.Target).Returns(typeof(Exception));
            mockArgRecon1.Setup(r => r.ReconstituteFrom(It.IsAny<IReadOnlyList<DBValue>>())).Returns(null);
            var recons = new IReconstitutor[] { mockArgRecon0.Object, mockArgRecon1.Object };
            var data = new DBValue[] { DBValue.Create('%') };
            var creator = new ByConstructorCreator(ctor, recons);

            // Act
            var value = creator.Execute(data);

            // Assert
            var expected = new ArgumentException(null, (Exception?)null);
            value.Should().BeEquivalentTo(expected);
        }
    }
}
