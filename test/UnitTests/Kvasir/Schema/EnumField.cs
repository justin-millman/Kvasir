using Atropos.NSubstitute;
using FluentAssertions;
using Kvasir.Schema;
using Kvasir.Transcription;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Optional;
using System.Collections.Generic;

namespace UT.Kvasir.Schema {
    [TestClass, TestCategory("EnumField")]
    public class EnumFieldTests {
        [TestMethod] public void Construct() {
            // Arrange
            var name = new FieldName("Continent");
            var nullability = IsNullable.Yes;
            var defaultValue = Option.Some(DBValue.Create(null));
            var enums = new DBValue[] { DBValue.Create("Europe"), DBValue.Create("Asia"), DBValue.Create("Africa") };

            // Act
            var field = new EnumField(name, nullability, defaultValue, enums);

            // Assert
            field.Name.Should<FieldName>().Be(name);
            field.DataType.Should().Be(DBType.Enumeration);
            field.Nullability.Should().Be(nullability);
            field.DefaultValue.Should().Be(defaultValue);
            field.Enumerators.Should().BeEquivalentTo(enums);
        }

        [TestMethod] public void GenerateDeclarationWithoutDefault() {
            // Arrange
            var name = new FieldName("Status");
            var nullability = IsNullable.No;
            var enums = new DBValue[] { DBValue.Create("Independent"), DBValue.Create("Colony") };
            var mockBuilder = Substitute.For<IFieldDeclBuilder<SqlSnippet>>();
            var field = new EnumField(name, nullability, Option.None<DBValue>(), enums);

            // Act
            _ = (field as IField).GenerateDeclaration(mockBuilder);

            // Assert
            mockBuilder.Received().SetName(name);
            mockBuilder.Received().SetDataType(DBType.Enumeration);
            mockBuilder.Received().SetNullability(nullability);
            mockBuilder.Received().SetAllowedValues(NArg.IsSameSequence<IEnumerable<DBValue>>(enums));
            mockBuilder.Received().Build();
            mockBuilder.ReceivedCalls().Should().HaveCount(5);
        }

        [TestMethod] public void GenerateDeclarationWithDefault() {
            // Arrange
            var name = new FieldName("Government");
            var nullability = IsNullable.No;
            var enums = new DBValue[] { DBValue.Create("Monarchy"), DBValue.Create("Democracy") };
            var defaultValue = enums[1];
            var mockBuilder = Substitute.For<IFieldDeclBuilder<SqlSnippet>>();
            var field = new EnumField(name, nullability, Option.Some(defaultValue), enums);

            // Act
            _ = (field as IField).GenerateDeclaration(mockBuilder);

            // Assert
            mockBuilder.Received().SetName(name);
            mockBuilder.Received().SetDataType(DBType.Enumeration);
            mockBuilder.Received().SetNullability(nullability);
            mockBuilder.Received().SetDefaultValue(defaultValue);
            mockBuilder.Received().SetAllowedValues(NArg.IsSameSequence<IEnumerable<DBValue>>(enums));
            mockBuilder.Received().Build();
            mockBuilder.ReceivedCalls().Should().HaveCount(6);
        }
    }
}
