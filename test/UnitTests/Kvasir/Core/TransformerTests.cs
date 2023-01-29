using Cybele.Core;
using FluentAssertions;
using Kvasir.Core;
using Kvasir.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;

using static Kvasir.Core.TransformUserValues;

namespace UT.Kvasir.Core {
    [TestClass, TestCategory("TransformUserValues")]
    public class TransformerTests {
        [TestMethod] public void TransformPrimitiveNoConverter() {
            // Arrange
            var value = 374;
            var field = new Mock<IField>();
            field.Setup(f => f.DataType).Returns(DBType.Int32);
            var converter = DataConverter.Identity<int>();
            var settings = Settings.Default;

            // Act
            var transform = Transform(value, field.Object, converter, settings);
            var expected = DBValue.Create(value);

            // Assert
            transform.Should().Be(expected);
        }

        [TestMethod] public void TransformPrimitiveWithConverter() {
            // Arrange
            var value = -716239;
            var field = new Mock<IField>();
            field.Setup(f => f.DataType).Returns(DBType.Int32);
            var converter = DataConverter.Create<int, int>(i => i * -1, i => i * -1);
            var settings = Settings.Default;

            // Act
            var transform = Transform(value, field.Object, converter, settings);
            var expected = DBValue.Create(value);

            // Assert
            transform.Should().Be(expected);
        }

        [TestMethod] public void TransformEnum() {
            // Arrange
            var value = TestTimeout.Infinite;
            var field = new Mock<IField>();
            field.Setup(f => f.DataType).Returns(DBType.Enumeration);
            var converter = DataConverter.Create<TestTimeout, string>(e => e.ToString(), s => TestTimeout.Infinite);
            var settings = Settings.Default;

            // Act
            var transform = Transform(value, field.Object, converter, settings);
            var expected = DBValue.Create(converter.Convert(value));

            // Assert
            transform.Should().Be(expected);
        }

        [TestMethod] public void TransformDateTimeString() {
            // Arrange
            var value = "08/17/1853";
            var field = new Mock<IField>();
            field.Setup(f => f.DataType).Returns(DBType.DateTime);
            var converter = DataConverter.Identity<DateTime>();
            var settings = Settings.Default;

            // Act
            var transform = Transform(value, field.Object, converter, settings);
            var expected = DBValue.Create(DateTime.Parse(value, null));

            // Assert
            transform.Should().Be(expected);
        }

        [TestMethod] public void TransformGuidString() {
            // Arrange
            var value = "7123afb1-c485-4906-8e2e-c6375934a77e";
            var field = new Mock<IField>();
            field.Setup(f => f.DataType).Returns(DBType.Guid);
            var converter = DataConverter.Identity<Guid>();
            var settings = Settings.Default;

            // Act
            var transform = Transform(value, field.Object, converter, settings);
            var expected = DBValue.Create(Guid.Parse(value));

            // Assert
            transform.Should().Be(expected);
        }

        [TestMethod] public void TransformListOfPrimitivesNoConverter() {
            // Arrange
            var values = new HashSet<char>() { '8', '=', 'r', ',', '~', '/', ':', '+' };
            var field = new Mock<IField>();
            field.Setup(f => f.DataType).Returns(DBType.Character);
            var converter = DataConverter.Identity<char>();
            var settings = Settings.Default;

            // Act
            var transform = Transform(values, field.Object, converter, settings);
            var expected = values.Select(c => DBValue.Create(c));

            // Assert
            transform.Should().BeEquivalentTo(expected);
        }

        [TestMethod] public void TransformListOfPrimitivesWithConverter() {
            // Arrange
            var values = new HashSet<string> { "Carlsbad", "Hot Springs", "San Jacinto", "Nome" };
            var field = new Mock<IField>();
            field.Setup(f => f.DataType).Returns(DBType.Text);
            var converter = DataConverter.Create<string, string>(s => $"!!!{s}", s => s[3..]);
            var settings = Settings.Default;

            // Act
            var transform = Transform(values, field.Object, converter, settings);
            var expected = values.Select(c => DBValue.Create(c));

            // Assert
            transform.Should().BeEquivalentTo(expected);
        }

        [TestMethod] public void TransformListOfEnums() {
            // Arrange
            var values = new HashSet<ConsoleColor> { ConsoleColor.Red, ConsoleColor.Green, ConsoleColor.Cyan };
            var field = new Mock<IField>();
            field.Setup(f => f.DataType).Returns(DBType.Enumeration);
            var converter = DataConverter.Create<ConsoleColor, int>(c => (int)c, i => (ConsoleColor)i);
            var settings = Settings.Default;

            // Act
            var transform = Transform(values, field.Object, converter, settings);
            var expected = values.Select(c => DBValue.Create(converter.Convert(c)));

            // Assert
            transform.Should().BeEquivalentTo(expected);
        }

        [TestMethod] public void TransformListOfDateTimeStrings() {
            // Arrange
            var values = new HashSet<string> { "01/01/2001", "12/22/1733", "06/09/477" };
            var field = new Mock<IField>();
            field.Setup(f => f.DataType).Returns(DBType.DateTime);
            var converter = DataConverter.Identity<DateTime>();
            var settings = Settings.Default;

            // Act
            var transform = Transform(values, field.Object, converter, settings);
            var expected = values.Select(d => DBValue.Create(DateTime.Parse(d, null)));

            // Assert
            transform.Should().BeEquivalentTo(expected);
        }

        [TestMethod] public void TransformListOfGuidStrings() {
            // Arrange
            var values = new HashSet<string> { "3b17cdc7-33e2-4082-a41c-6ef966acd947", "b62888e3-f732-4fad-8531-5407f741f1da" };
            var field = new Mock<IField>();
            field.Setup(f => f.DataType).Returns(DBType.Guid);
            var converter = DataConverter.Identity<Guid>();
            var settings = Settings.Default;

            // Act
            var transform = Transform(values, field.Object, converter, settings);
            var expected = values.Select(g => DBValue.Create(Guid.Parse(g)));

            // Assert
            transform.Should().BeEquivalentTo(expected);
        }

        [TestMethod] public void TransformNullValueForPrimitive() {
            // Arrange
            var field = new Mock<IField>();
            field.Setup(f => f.DataType).Returns(DBType.Double);
            var converter = DataConverter.Identity<double>();
            var settings = Settings.Default;

            // Act
            var transform = Transform(null, field.Object, converter, settings);

            // Assert
            transform.Should().Be(DBValue.Create(null));
        }

        [TestMethod] public void TransformNullValueForEnum() {
            // Arrange
            var field = new Mock<IField>();
            field.Setup(f => f.DataType).Returns(DBType.Enumeration);
            var converter = DataConverter.Create<DataAccessMethod, int>(d => (int)d, i => (DataAccessMethod)i);
            var settings = Settings.Default;

            // Act
            var transform = Transform(null, field.Object, converter, settings);

            // Assert
            transform.Should().Be(DBValue.Create(null));
        }

        [TestMethod] public void TransformNullValueForDateTime() {
            // Arrange
            var field = new Mock<IField>();
            field.Setup(f => f.DataType).Returns(DBType.DateTime);
            var converter = DataConverter.Identity<DateTime>();
            var settings = Settings.Default;

            // Act
            var transform = Transform(null, field.Object, converter, settings);

            // Assert
            transform.Should().Be(DBValue.Create(null));
        }

        [TestMethod] public void TransformNullValueForGuid() {
            // Arrange
            var field = new Mock<IField>();
            field.Setup(f => f.DataType).Returns(DBType.Guid);
            var converter = DataConverter.Identity<Guid>();
            var settings = Settings.Default;

            // Act
            var transform = Transform(null, field.Object, converter, settings);

            // Assert
            transform.Should().Be(DBValue.Create(null));
        }
    }
}