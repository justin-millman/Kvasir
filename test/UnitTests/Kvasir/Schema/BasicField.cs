using FluentAssertions;
using Kvasir.Schema;
using Kvasir.Transcription;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
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
            var mockBuilder = new Mock<IFieldDeclBuilder>();

            // Act
            _ = field.GenerateSqlDeclaration(mockBuilder.Object);

            // Assert
            mockBuilder.Verify(builder => builder.SetName(name));
            mockBuilder.Verify(builder => builder.SetDataType(dbType));
            mockBuilder.Verify(builder => builder.SetNullability(nullability));
            mockBuilder.Verify(builder => builder.Build());
            mockBuilder.VerifyNoOtherCalls();
        }

        [TestMethod] public void GenerateDeclarationWithDefault() {
            // Arrange
            var name = new FieldName("GDP");
            var dbType = DBType.UInt32;
            var nullability = IsNullable.Yes;
            var defaultValue = DBValue.Create(2500000u);
            var field = new BasicField(name, dbType, nullability, Option.Some(defaultValue));
            var mockBuilder = new Mock<IFieldDeclBuilder>();

            // Act
            _ = field.GenerateSqlDeclaration(mockBuilder.Object);

            // Assert
            mockBuilder.Verify(builder => builder.SetName(name));
            mockBuilder.Verify(builder => builder.SetDataType(dbType));
            mockBuilder.Verify(builder => builder.SetNullability(nullability));
            mockBuilder.Verify(builder => builder.SetDefaultValue(defaultValue));
            mockBuilder.Verify(builder => builder.Build());
            mockBuilder.VerifyNoOtherCalls();
        }
    }
}
