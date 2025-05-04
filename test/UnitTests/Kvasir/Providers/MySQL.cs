using FluentAssertions;
using Kvasir.Exceptions;
using Kvasir.Providers.MySQL;
using Kvasir.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace UT.Kvasir.Providers {
    [TestClass, TestCategory("MySQL - Keys")]
    public class MySqlKeyTests {
        [TestMethod] public void CandidateKeySingleFieldUnnamed() {
            // Arrange
            var field = Substitute.For<IField>();
            field.Name.Returns(new FieldName("Storybrooke"));

            // Act
            var builder = new KeyBuilder();
            builder.SetFields(new[] { field });
            var sql = builder.Build();

            // Assert
            sql.Should().Be($"UNIQUE (`{field.Name}`)");
        }

        [TestMethod] public void CandidateKeySingleFieldNamed() {
            // Arrange
            var field = Substitute.For<IField>();
            field.Name.Returns(new FieldName("Talagray"));
            var keyName = new KeyName("KEY_NAME");

            // Act
            var builder = new KeyBuilder();
            builder.SetName(keyName);
            builder.SetFields(new[] { field });
            var sql = builder.Build();

            // Assert
            sql.Should().Be($"CONSTRAINT `{keyName}` UNIQUE (`{field.Name}`)");
        }

        [TestMethod] public void CandidateKeyMultipleFieldsUnnamed() {
            // Arrange
            var field0 = Substitute.For<IField>();
            field0.Name.Returns(new FieldName("Alera Imperia"));
            var field1 = Substitute.For<IField>();
            field1.Name.Returns(new FieldName("Sinegard"));
            var field2 = Substitute.For<IField>();
            field2.Name.Returns(new FieldName("Mermaid Lagoon"));

            // Act
            var builder = new KeyBuilder();
            builder.SetFields(new[] { field0, field1, field2 });
            var sql = builder.Build();

            // Assert
            sql.Should().Be($"UNIQUE (`{field0.Name}`, `{field1.Name}`, `{field2.Name}`)");
        }

        [TestMethod] public void CandidateKeyMultipleFieldsNamed() {
            // Arrange
            var field0 = Substitute.For<IField>();
            field0.Name.Returns(new FieldName("Tree Hill"));
            var field1 = Substitute.For<IField>();
            field1.Name.Returns(new FieldName("Rosewood"));
            var keyName = new KeyName("CANDIDATE");

            // Act
            var builder = new KeyBuilder();
            builder.SetName(keyName);
            builder.SetFields(new[] { field0, field1 });
            var sql = builder.Build();

            // Assert
            sql.Should().Be($"CONSTRAINT `{keyName}` UNIQUE (`{field0.Name}`, `{field1.Name}`)");
        }

        [TestMethod] public void PrimaryKeySingleFieldUnnamed() {
            // Arrange
            var field = Substitute.For<IField>();
            field.Name.Returns(new FieldName("Mudville"));

            // Act
            var builder = new KeyBuilder();
            builder.SetFields(new[] { field });
            builder.SetAsPrimaryKey();
            var sql = builder.Build();

            // Assert
            sql.Should().Be($"PRIMARY KEY (`{field.Name}`)");
        }

        [TestMethod] public void PrimaryKeySingleFieldNamed() {
            // Arrange
            var field = Substitute.For<IField>();
            field.Name.Returns(new FieldName("Wyrmroost"));
            var keyName = new KeyName("PK");

            // Act
            var builder = new KeyBuilder();
            builder.SetName(keyName);
            builder.SetFields(new[] { field });
            builder.SetAsPrimaryKey();
            var sql = builder.Build();

            // Assert
            sql.Should().Be($"CONSTRAINT `{keyName}` PRIMARY KEY (`{field.Name}`)");
        }

        [TestMethod] public void PrimaryKeyMultipleFieldsUnnamed() {
            // Arrange
            var field0 = Substitute.For<IField>();
            field0.Name.Returns(new FieldName("Tuonela"));
            var field1 = Substitute.For<IField>();
            field1.Name.Returns(new FieldName("Port Charles"));
            var field2 = Substitute.For<IField>();
            field2.Name.Returns(new FieldName("Farhampton"));
            var field3 = Substitute.For<IField>();
            field3.Name.Returns(new FieldName("Eastwick"));

            // Act
            var builder = new KeyBuilder();
            builder.SetFields(new[] { field0, field1, field2, field3 });
            builder.SetAsPrimaryKey();
            var sql = builder.Build();

            // Assert
            sql.Should().Be($"PRIMARY KEY (`{field0.Name}`, `{field1.Name}`, `{field2.Name}`, `{field3.Name}`)");
        }

        [TestMethod] public void PrimaryKeyMultipleFieldsNamed() {
            // Arrange
            var field0 = Substitute.For<IField>();
            field0.Name.Returns(new FieldName("Resembool"));
            var field1 = Substitute.For<IField>();
            field1.Name.Returns(new FieldName("Diggen's Point"));
            var keyName = new KeyName("TABLE_PRIMARY_KEY");

            // Act
            var builder = new KeyBuilder();
            builder.SetName(keyName);
            builder.SetFields(new[] { field0, field1 });
            builder.SetAsPrimaryKey();
            var sql = builder.Build();

            // Assert
            sql.Should().Be($"CONSTRAINT `{keyName}` PRIMARY KEY (`{field0.Name}`, `{field1.Name}`)");
        }

        [TestMethod] public void KeyNameIsMaximumLength() {
            // Arrange
            var field = Substitute.For<IField>();
            field.Name.Returns(new FieldName("Hope Springs"));
            var keyName = new KeyName(new string('h', 64));

            // Act
            var builder = new KeyBuilder();
            builder.SetName(keyName);
            builder.SetFields(new[] { field });
            var sql = builder.Build();

            // Assert
            sql.Should().Be($"CONSTRAINT `{keyName}` UNIQUE (`{field.Name}`)");
        }

        [TestMethod] public void KeyNameExceedsMaximumLength_IsError() {
            // Arrange
            var keyName = new KeyName(new string('q', 129));

            // Act
            var builder = new KeyBuilder();
            var action = () => builder.SetName(keyName);

            // Assert
            action.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining("[MySQL]")
                .WithMessageContaining(keyName.ToString())
                .WithMessageContaining("exceeds the maximum of 64 characters");
        }
    }
}
