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

    [TestClass, TestCategory("MySQL - Foreign Keys")]
    public class MySqlForeignKeyTests {
        [TestMethod] public void SingleFieldUnnamed() {
            // Arrange
            var field = Substitute.For<IField>();
            field.Name.Returns(new FieldName("Chinquapin"));

            var table = Substitute.For<ITable>();
            table.Name.Returns(new TableName("OtherTable"));
            var pkField = Substitute.For<IField>();
            pkField.Name.Returns(new FieldName("Clanton"));
            var pk = new PrimaryKey(new[] { pkField });
            table.PrimaryKey.Returns(pk);

            // Act
            var builder = new ForeignKeyBuilder();
            builder.SetFields(new[] { field });
            builder.SetReferencedTable(table);
            var sql = builder.Build();

            // Assert
            sql.Should().Be(
                $"FOREIGN KEY (`{field.Name}`) " +
                $"REFERENCES `{table.Name}` (`{pkField.Name}`)"
            );
        }

        [TestMethod] public void SingleFieldNamed() {
            // Arrange
            var field = Substitute.For<IField>();
            field.Name.Returns(new FieldName("Aleero City"));
            var keyName = new FKName("FK1");

            var table = Substitute.For<ITable>();
            table.Name.Returns(new TableName("TableBeingReferenced"));
            var pkField = Substitute.For<IField>();
            pkField.Name.Returns(new FieldName("Baldur's Gate"));
            var pk = new PrimaryKey(new[] { pkField });
            table.PrimaryKey.Returns(pk);

            // Act
            var builder = new ForeignKeyBuilder();
            builder.SetName(keyName);
            builder.SetFields(new[] { field });
            builder.SetReferencedTable(table);
            var sql = builder.Build();

            // Assert
            sql.Should().Be(
                $"CONSTRAINT `{keyName}` " +
                $"FOREIGN KEY (`{field.Name}`) " +
                $"REFERENCES `{table.Name}` (`{pkField.Name}`)"
            );
        }

        [TestMethod] public void MultipleFieldsUnnamed() {
            // Arrange
            var field0 = Substitute.For<IField>();
            field0.Name.Returns(new FieldName("Amritsar"));
            var field1 = Substitute.For<IField>();
            field1.Name.Returns(new FieldName("Rawalpindi"));

            var table = Substitute.For<ITable>();
            table.Name.Returns(new TableName("TargetTable"));
            var pkField0 = Substitute.For<IField>();
            pkField0.Name.Returns(new FieldName("Ciudad Juárez"));
            var pkField1 = Substitute.For<IField>();
            pkField1.Name.Returns(new FieldName("Bandung"));
            var pk = new PrimaryKey(new[] { pkField0, pkField1 });
            table.PrimaryKey.Returns(pk);

            // Act
            var builder = new ForeignKeyBuilder();
            builder.SetFields(new[] { field0, field1 });
            builder.SetReferencedTable(table);
            var sql = builder.Build();

            // Assert
            sql.Should().Be(
                $"FOREIGN KEY (`{field0.Name}`, `{field1.Name}`) " +
                $"REFERENCES `{table.Name}` (`{pkField0.Name}`, `{pkField1.Name}`)"
            );
        }

        [TestMethod] public void MultipleFieldsNamed() {
            // Arrange
            var field0 = Substitute.For<IField>();
            field0.Name.Returns(new FieldName("Tabriz"));
            var field1 = Substitute.For<IField>();
            field1.Name.Returns(new FieldName("Varanasi"));
            var field2 = Substitute.For<IField>();
            field2.Name.Returns(new FieldName("Rostov-on-Don"));
            var keyName = new FKName("ForeignKey");

            var table = Substitute.For<ITable>();
            table.Name.Returns(new TableName("Table2"));
            var pkField0 = Substitute.For<IField>();
            pkField0.Name.Returns(new FieldName("Gold Coast"));
            var pkField1 = Substitute.For<IField>();
            pkField1.Name.Returns(new FieldName("Belém"));
            var pkField2 = Substitute.For<IField>();
            pkField2.Name.Returns(new FieldName("Donetsk"));
            var pk = new PrimaryKey(new[] { pkField0, pkField1, pkField2 });
            table.PrimaryKey.Returns(pk);

            // Act
            var builder = new ForeignKeyBuilder();
            builder.SetName(keyName);
            builder.SetFields(new[] { field0, field1, field2 });
            builder.SetReferencedTable(table);
            var sql = builder.Build();

            // Assert
            sql.Should().Be(
                $"CONSTRAINT `{keyName}` " +
                $"FOREIGN KEY (`{field0.Name}`, `{field1.Name}`, `{field2.Name}`) " +
                $"REFERENCES `{table.Name}` (`{pkField0.Name}`, `{pkField1.Name}`, `{pkField2.Name}`)"
            );
        }

        [TestMethod] public void OnActionCascade() {
            // Arrange
            var field = Substitute.For<IField>();
            field.Name.Returns(new FieldName("Durban"));

            var table = Substitute.For<ITable>();
            table.Name.Returns(new TableName("TableNumber2"));
            var pkField = Substitute.For<IField>();
            pkField.Name.Returns(new FieldName("Katowice"));
            var pk = new PrimaryKey(new[] { pkField });
            table.PrimaryKey.Returns(pk);

            // Act
            var builder = new ForeignKeyBuilder();
            builder.SetFields(new[] { field });
            builder.SetReferencedTable(table);
            builder.SetOnDeleteBehavior(OnDelete.Cascade);
            builder.SetOnUpdateBehavior(OnUpdate.Cascade);
            var sql = builder.Build();

            // Assert
            sql.Should().Be(
                $"FOREIGN KEY (`{field.Name}`) " +
                $"REFERENCES `{table.Name}` (`{pkField.Name}`) " +
                "ON DELETE CASCADE " +
                "ON UPDATE CASCADE"
            );
        }

        [TestMethod] public void OnActionPrevent() {
            // Arrange
            var field = Substitute.For<IField>();
            field.Name.Returns(new FieldName("Bonn"));

            var table = Substitute.For<ITable>();
            table.Name.Returns(new TableName("AdjacentTable"));
            var pkField = Substitute.For<IField>();
            pkField.Name.Returns(new FieldName("Bhopal"));
            var pk = new PrimaryKey(new[] { pkField });
            table.PrimaryKey.Returns(pk);

            // Act
            var builder = new ForeignKeyBuilder();
            builder.SetFields(new[] { field });
            builder.SetReferencedTable(table);
            builder.SetOnDeleteBehavior(OnDelete.Prevent);
            builder.SetOnUpdateBehavior(OnUpdate.Prevent);
            var sql = builder.Build();

            // Assert
            sql.Should().Be(
                $"FOREIGN KEY (`{field.Name}`) " +
                $"REFERENCES `{table.Name}` (`{pkField.Name}`) " +
                "ON DELETE RESTRICT " +
                "ON UPDATE RESTRICT"
            );
        }

        [TestMethod] public void OnActionDoNothing() {
            // Arrange
            var field = Substitute.For<IField>();
            field.Name.Returns(new FieldName("Nizhny Novgorod"));

            var table = Substitute.For<ITable>();
            table.Name.Returns(new TableName("NotThisTable"));
            var pkField = Substitute.For<IField>();
            pkField.Name.Returns(new FieldName("Almaty"));
            var pk = new PrimaryKey(new[] { pkField });
            table.PrimaryKey.Returns(pk);

            // Act
            var builder = new ForeignKeyBuilder();
            builder.SetFields(new[] { field });
            builder.SetReferencedTable(table);
            builder.SetOnDeleteBehavior(OnDelete.NoAction);
            builder.SetOnUpdateBehavior(OnUpdate.NoAction);
            var sql = builder.Build();

            // Assert
            sql.Should().Be(
                $"FOREIGN KEY (`{field.Name}`) " +
                $"REFERENCES `{table.Name}` (`{pkField.Name}`) " +
                "ON DELETE NO ACTION " +
                "ON UPDATE NO ACTION"
            );
        }

        [TestMethod] public void OnActionSetDefault() {
            // Arrange
            var field = Substitute.For<IField>();
            field.Name.Returns(new FieldName("Goiânia"));

            var table = Substitute.For<ITable>();
            table.Name.Returns(new TableName("PartnerTable"));
            var pkField = Substitute.For<IField>();
            pkField.Name.Returns(new FieldName("Kumasi"));
            var pk = new PrimaryKey(new[] { pkField });
            table.PrimaryKey.Returns(pk);

            // Act
            var builder = new ForeignKeyBuilder();
            builder.SetFields(new[] { field });
            builder.SetReferencedTable(table);
            builder.SetOnDeleteBehavior(OnDelete.SetDefault);
            builder.SetOnUpdateBehavior(OnUpdate.SetDefault);
            var sql = builder.Build();

            // Assert
            sql.Should().Be(
                $"FOREIGN KEY (`{field.Name}`) " +
                $"REFERENCES `{table.Name}` (`{pkField.Name}`) " +
                "ON DELETE SET DEFAULT " +
                "ON UPDATE SET DEFAULT"
            );
        }

        [TestMethod] public void OnActionSetNull() {
            // Arrange
            var field = Substitute.For<IField>();
            field.Name.Returns(new FieldName("Calgary"));

            var table = Substitute.For<ITable>();
            table.Name.Returns(new TableName("TableTheSecond"));
            var pkField = Substitute.For<IField>();
            pkField.Name.Returns(new FieldName("Soweto"));
            var pk = new PrimaryKey(new[] { pkField });
            table.PrimaryKey.Returns(pk);

            // Act
            var builder = new ForeignKeyBuilder();
            builder.SetFields(new[] { field });
            builder.SetReferencedTable(table);
            builder.SetOnDeleteBehavior(OnDelete.SetNull);
            builder.SetOnUpdateBehavior(OnUpdate.SetNull);
            var sql = builder.Build();

            // Assert
            sql.Should().Be(
                $"FOREIGN KEY (`{field.Name}`) " +
                $"REFERENCES `{table.Name}` (`{pkField.Name}`) " +
                "ON DELETE SET NULL " +
                "ON UPDATE SET NULL"
            );
        }

        [TestMethod] public void OnDeleteDifferentThanOnUpdate() {
            // Arrange
            var field = Substitute.For<IField>();
            field.Name.Returns(new FieldName("Bishkek"));

            var table = Substitute.For<ITable>();
            table.Name.Returns(new TableName("TableNumeroDos"));
            var pkField = Substitute.For<IField>();
            pkField.Name.Returns(new FieldName("Tunisia"));
            var pk = new PrimaryKey(new[] { pkField });
            table.PrimaryKey.Returns(pk);

            // Act
            var builder = new ForeignKeyBuilder();
            builder.SetFields(new[] { field });
            builder.SetReferencedTable(table);
            builder.SetOnDeleteBehavior(OnDelete.Cascade);
            builder.SetOnUpdateBehavior(OnUpdate.SetNull);
            var sql = builder.Build();

            // Assert
            sql.Should().Be(
                $"FOREIGN KEY (`{field.Name}`) " +
                $"REFERENCES `{table.Name}` (`{pkField.Name}`) " +
                "ON DELETE CASCADE " +
                "ON UPDATE SET NULL"
            );
        }

        [TestMethod] public void OnDeleteOnly() {
            // Arrange
            var field = Substitute.For<IField>();
            field.Name.Returns(new FieldName("Mysore"));

            var table = Substitute.For<ITable>();
            table.Name.Returns(new TableName("ThatOtherTableOverThere"));
            var pkField = Substitute.For<IField>();
            pkField.Name.Returns(new FieldName("Cuernavaca"));
            var pk = new PrimaryKey(new[] { pkField });
            table.PrimaryKey.Returns(pk);

            // Act
            var builder = new ForeignKeyBuilder();
            builder.SetFields(new[] { field });
            builder.SetReferencedTable(table);
            builder.SetOnDeleteBehavior(OnDelete.NoAction);
            var sql = builder.Build();

            // Assert
            sql.Should().Be(
                $"FOREIGN KEY (`{field.Name}`) " +
                $"REFERENCES `{table.Name}` (`{pkField.Name}`) " +
                "ON DELETE NO ACTION"
            );
        }

        [TestMethod] public void OnUpdateOnly() {
            // Arrange
            var field = Substitute.For<IField>();
            field.Name.Returns(new FieldName("Gitega"));

            var table = Substitute.For<ITable>();
            table.Name.Returns(new TableName("GodTable"));
            var pkField = Substitute.For<IField>();
            pkField.Name.Returns(new FieldName("Bulawayo"));
            var pk = new PrimaryKey(new[] { pkField });
            table.PrimaryKey.Returns(pk);

            // Act
            var builder = new ForeignKeyBuilder();
            builder.SetFields(new[] { field });
            builder.SetReferencedTable(table);
            builder.SetOnUpdateBehavior(OnUpdate.Prevent);
            var sql = builder.Build();

            // Assert
            sql.Should().Be(
                $"FOREIGN KEY (`{field.Name}`) " +
                $"REFERENCES `{table.Name}` (`{pkField.Name}`) " +
                "ON UPDATE RESTRICT"
            );
        }

        [TestMethod] public void KeyNameIsMaximumLength() {
            // Arrange
            var field = Substitute.For<IField>();
            field.Name.Returns(new FieldName("Sheffield"));
            var keyName = new FKName(new string('R', 64));

            var table = Substitute.For<ITable>();
            table.Name.Returns(new TableName("CousinTable"));
            var pkField = Substitute.For<IField>();
            pkField.Name.Returns(new FieldName("Curitiba"));
            var pk = new PrimaryKey(new[] { pkField });
            table.PrimaryKey.Returns(pk);

            // Act
            var builder = new ForeignKeyBuilder();
            builder.SetName(keyName);
            builder.SetFields(new[] { field });
            builder.SetReferencedTable(table);
            var sql = builder.Build();

            // Assert
            sql.Should().Be(
                $"CONSTRAINT `{keyName}` " +
                $"FOREIGN KEY (`{field.Name}`) " +
                $"REFERENCES `{table.Name}` (`{pkField.Name}`)"
            );
        }

        [TestMethod] public void KeyNameExceedsMaximumLength_IsError() {
            // Arrange
            var keyName = new FKName(new string(':', 2617));

            // Act
            var builder = new ForeignKeyBuilder();
            var action = () => builder.SetName(keyName);

            // Assert
            action.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining("[MySQL]")
                .WithMessageContaining(keyName.ToString())
                .WithMessageContaining("exceeds the maximum of 64 characters");
        }
    }
}
