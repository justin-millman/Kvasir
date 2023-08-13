using FluentAssertions;
using Kvasir.Schema;
using Kvasir.Transcription;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Optional;

namespace UT.Kvasir.Schema {
    [TestClass, TestCategory("BasicField")]
    public class BasicFieldTests {
        [TestMethod] public void Construct() {
            // Arrange
            var name = new FieldName("Population");
            var dbType = DBType.Int32;
            var nullability = IsNullable.No;
            var defaultValue = Option.Some(DBValue.Create(100));

            // Act
            var field = new BasicField(name, dbType, nullability, defaultValue);

            // Assert
            field.Name.Should<FieldName>().Be(name);
            field.DataType.Should().Be(dbType);
            field.Nullability.Should().Be(nullability);
            field.DefaultValue.Should().Be(defaultValue);
        }

        [TestMethod] public void GenerateDeclarationWithoutDefault() {
            // Arrange
            var name = new FieldName("CapitalCity");
            var dbType = DBType.Text;
            var nullability = IsNullable.No;
            var field = new BasicField(name, dbType, nullability, Option.None<DBValue>());
            var mockBuilder = Substitute.For<IFieldDeclBuilder<SqlSnippet>>();

            // Act
            _ = (field as IField).GenerateDeclaration(mockBuilder);

            // Assert
            mockBuilder.Received().SetName(name);
            mockBuilder.Received().SetDataType(dbType);
            mockBuilder.Received().SetNullability(nullability);
            mockBuilder.Received().Build();
            mockBuilder.ReceivedCalls().Should().HaveCount(4);
        }

        [TestMethod] public void GenerateDeclarationWithDefault() {
            // Arrange
            var name = new FieldName("GDP");
            var dbType = DBType.UInt32;
            var nullability = IsNullable.Yes;
            var defaultValue = DBValue.Create(2500000u);
            var field = new BasicField(name, dbType, nullability, Option.Some(defaultValue));
            var mockBuilder = Substitute.For<IFieldDeclBuilder<SqlSnippet>>();

            // Act
            _ = (field as IField).GenerateDeclaration(mockBuilder);

            // Assert
            mockBuilder.Received().SetName(name);
            mockBuilder.Received().SetDataType(dbType);
            mockBuilder.Received().SetNullability(nullability);
            mockBuilder.Received().SetDefaultValue(defaultValue);
            mockBuilder.Received().Build();
            mockBuilder.ReceivedCalls().Should().HaveCount(5);
        }
    }
}
