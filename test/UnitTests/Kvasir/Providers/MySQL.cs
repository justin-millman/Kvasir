using FluentAssertions;
using Kvasir.Exceptions;
using Kvasir.Providers.MySQL;
using Kvasir.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;

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

    [TestClass, TestCategory("MySQL - Constraints")]
    public class MySqlConstraintTests {
        [TestMethod] public void LengthConstraint_Maximum_LessThan() {
            // Arrange
            var field = Substitute.For<IField>();
            field.Name.Returns(new FieldName("Sector Five"));
            field.DataType.Returns(DBType.Text);
            var length = 73;
            var constraint = new ConstantClause(new FieldExpression(FieldFunction.LengthOf, field), ComparisonOperator.LT, DBValue.Create(length));

            // Act
            var builder = new ConstraintBuilder();
            builder.AddClause(constraint);
            var decl = builder.Build();

            // Assert
            decl.Should().BeOfType<MaxLengthConstraintDecl>();
            ((MaxLengthConstraintDecl)decl).Field.Should().Be(field.Name.ToString());
            ((MaxLengthConstraintDecl)decl).MaxLength.Should().Be((ulong)(length - 1));
        }

        [TestMethod] public void LengthConstraint_Maximum_LessOrEqual() {
            // Arrange
            var field = Substitute.For<IField>();
            field.Name.Returns(new FieldName("Lumbria"));
            field.DataType.Returns(DBType.Text);
            var length = 198;
            var constraint = new ConstantClause(new FieldExpression(FieldFunction.LengthOf, field), ComparisonOperator.LTE, DBValue.Create(length));

            // Act
            var builder = new ConstraintBuilder();
            builder.AddClause(constraint);
            var decl = builder.Build();

            // Assert
            decl.Should().BeOfType<MaxLengthConstraintDecl>();
            ((MaxLengthConstraintDecl)decl).Field.Should().Be(field.Name.ToString());
            ((MaxLengthConstraintDecl)decl).MaxLength.Should().Be((ulong)length);
        }

        [TestMethod] public void LengthConstraint_Minimum_GreaterThan() {
            // Arrange
            var field = Substitute.For<IField>();
            field.Name.Returns(new FieldName("Rosas"));
            field.DataType.Returns(DBType.Text);
            var length = 41;
            var constraint = new ConstantClause(new FieldExpression(FieldFunction.LengthOf, field), ComparisonOperator.GT, DBValue.Create(length));

            // Act
            var builder = new ConstraintBuilder();
            builder.AddClause(constraint);
            var decl = builder.Build();

            // Assert
            decl.Should().BeOfType<BasicConstraintDecl>();
            ((BasicConstraintDecl)decl).DDL.Should().Be($"CHECK (LENGTH(`{field.Name}`) > {length})");
        }

        [TestMethod] public void LengthConstraint_Minimum_GreaterOrEqual() {
            // Arrange
            var field = Substitute.For<IField>();
            field.Name.Returns(new FieldName("Nearburg"));
            field.DataType.Returns(DBType.Text);
            var length = 41;
            var constraint = new ConstantClause(new FieldExpression(FieldFunction.LengthOf, field), ComparisonOperator.GT, DBValue.Create(length));

            // Act
            var builder = new ConstraintBuilder();
            builder.AddClause(constraint);
            var decl = builder.Build();

            // Assert
            decl.Should().BeOfType<BasicConstraintDecl>();
            ((BasicConstraintDecl)decl).DDL.Should().Be($"CHECK (LENGTH(`{field.Name}`) > {length})");
        }

        [TestMethod] public void ComparisonConstraint_Equal() {
            // Arrange
            var field = Substitute.For<IField>();
            field.Name.Returns(new FieldName("Hillwood"));
            field.DataType.Returns(DBType.Int32);
            var anchor = 38190;
            var constraint = new ConstantClause(new FieldExpression(field), ComparisonOperator.EQ, DBValue.Create(anchor));

            // Act
            var builder = new ConstraintBuilder();
            builder.AddClause(constraint);
            var decl = builder.Build();

            // Assert
            decl.Should().BeOfType<BasicConstraintDecl>();
            ((BasicConstraintDecl)decl).DDL.Should().Be($"CHECK (`{field.Name}` = {anchor})");
        }

        [TestMethod] public void ComparisonConstraint_NotEqual() {
            // Arrange
            var field = Substitute.For<IField>();
            field.Name.Returns(new FieldName("Opar"));
            field.DataType.Returns(DBType.UInt16);
            var anchor = (ushort)4411;
            var constraint = new ConstantClause(new FieldExpression(field), ComparisonOperator.NE, DBValue.Create(anchor));

            // Act
            var builder = new ConstraintBuilder();
            builder.AddClause(constraint);
            var decl = builder.Build();

            // Assert
            decl.Should().BeOfType<BasicConstraintDecl>();
            ((BasicConstraintDecl)decl).DDL.Should().Be($"CHECK (`{field.Name}` != {anchor})");
        }

        [TestMethod] public void ComparisonConstraint_LessThan() {
            // Arrange
            var field = Substitute.For<IField>();
            field.Name.Returns(new FieldName("Primary Village"));
            field.DataType.Returns(DBType.UInt8);
            var anchor = (byte)61;
            var constraint = new ConstantClause(new FieldExpression(field), ComparisonOperator.LT, DBValue.Create(anchor));

            // Act
            var builder = new ConstraintBuilder();
            builder.AddClause(constraint);
            var decl = builder.Build();

            // Assert
            decl.Should().BeOfType<BasicConstraintDecl>();
            ((BasicConstraintDecl)decl).DDL.Should().Be($"CHECK (`{field.Name}` < {anchor})");
        }

        [TestMethod] public void ComparisonConstraint_LessOrEqual() {
            // Arrange
            var field = Substitute.For<IField>();
            field.Name.Returns(new FieldName("Gundalia"));
            field.DataType.Returns(DBType.Int64);
            var anchor = -912L;
            var constraint = new ConstantClause(new FieldExpression(field), ComparisonOperator.LTE, DBValue.Create(anchor));

            // Act
            var builder = new ConstraintBuilder();
            builder.AddClause(constraint);
            var decl = builder.Build();

            // Assert
            decl.Should().BeOfType<BasicConstraintDecl>();
            ((BasicConstraintDecl)decl).DDL.Should().Be($"CHECK (`{field.Name}` <= {anchor})");
        }

        [TestMethod] public void ComparisonConstraint_GreaterThan() {
            // Arrange
            var field = Substitute.For<IField>();
            field.Name.Returns(new FieldName("Tamagotchi Town"));
            field.DataType.Returns(DBType.UInt32);
            var anchor = 0U;
            var constraint = new ConstantClause(new FieldExpression(field), ComparisonOperator.GT, DBValue.Create(anchor));

            // Act
            var builder = new ConstraintBuilder();
            builder.AddClause(constraint);
            var decl = builder.Build();

            // Assert
            decl.Should().BeOfType<BasicConstraintDecl>();
            ((BasicConstraintDecl)decl).DDL.Should().Be($"CHECK (`{field.Name}` > {anchor})");
        }

        [TestMethod] public void ComparisonConstraint_GreaterOrEqual() {
            // Arrange
            var field = Substitute.For<IField>();
            field.Name.Returns(new FieldName("Ant Island"));
            field.DataType.Returns(DBType.Int32);
            var anchor = -1744;
            var constraint = new ConstantClause(new FieldExpression(field), ComparisonOperator.GTE, DBValue.Create(anchor));

            // Act
            var builder = new ConstraintBuilder();
            builder.AddClause(constraint);
            var decl = builder.Build();

            // Assert
            decl.Should().BeOfType<BasicConstraintDecl>();
            ((BasicConstraintDecl)decl).DDL.Should().Be($"CHECK (`{field.Name}` >= {anchor})");
        }

        [TestMethod] public void ComparisonConstraint_StringValue() {
            // Arrange
            var field = Substitute.For<IField>();
            field.Name.Returns(new FieldName("The Good Place"));
            field.DataType.Returns(DBType.Text);
            var anchor = "Duzakh";
            var constraint = new ConstantClause(new FieldExpression(field), ComparisonOperator.LT, DBValue.Create(anchor));

            // Act
            var builder = new ConstraintBuilder();
            builder.AddClause(constraint);
            var decl = builder.Build();

            // Assert
            decl.Should().BeOfType<BasicConstraintDecl>();
            ((BasicConstraintDecl)decl).DDL.Should().Be($"CHECK (`{field.Name}` < \"{anchor}\")");
        }

        [TestMethod] public void ComparisonConstraint_CharacterValue() {
            // Arrange
            var field = Substitute.For<IField>();
            field.Name.Returns(new FieldName("Drakkenheim"));
            field.DataType.Returns(DBType.Character);
            var anchor = 'w';
            var constraint = new ConstantClause(new FieldExpression(field), ComparisonOperator.GTE, DBValue.Create(anchor));

            // Act
            var builder = new ConstraintBuilder();
            builder.AddClause(constraint);
            var decl = builder.Build();

            // Assert
            decl.Should().BeOfType<BasicConstraintDecl>();
            ((BasicConstraintDecl)decl).DDL.Should().Be($"CHECK (`{field.Name}` >= \"{anchor}\")");
        }

        [TestMethod] public void ComparisonConstraint_DateTimeValue_DateOnly() {
            // Arrange
            var field = Substitute.For<IField>();
            field.Name.Returns(new FieldName("Janloon"));
            field.DataType.Returns(DBType.DateTime);
            var anchor = new DateTime(1865, 5, 30);
            var constraint = new ConstantClause(new FieldExpression(field), ComparisonOperator.NE, DBValue.Create(anchor));

            // Act
            var builder = new ConstraintBuilder();
            builder.AddClause(constraint);
            var decl = builder.Build();

            // Assert
            decl.Should().BeOfType<BasicConstraintDecl>();
            ((BasicConstraintDecl)decl).DDL.Should().Be($"CHECK (`{field.Name}` != DATETIME \"1865-05-30 00:00:00\")");
        }

        [TestMethod] public void ComparisonConstraint_DateTimeValue_WithTime() {
            // Arrange
            var field = Substitute.For<IField>();
            field.Name.Returns(new FieldName("Norgate"));
            field.DataType.Returns(DBType.DateTime);
            var anchor = new DateTime(2188, 11, 4, 1, 7, 2);
            var constraint = new ConstantClause(new FieldExpression(field), ComparisonOperator.LTE, DBValue.Create(anchor));

            // Act
            var builder = new ConstraintBuilder();
            builder.AddClause(constraint);
            var decl = builder.Build();

            // Assert
            decl.Should().BeOfType<BasicConstraintDecl>();
            ((BasicConstraintDecl)decl).DDL.Should().Be($"CHECK (`{field.Name}` <= DATETIME \"2188-11-04 01:07:02\")");
        }

        [TestMethod] public void ComparisonConstraint_BooleanValue_True() {
            // Arrange
            var field = Substitute.For<IField>();
            field.Name.Returns(new FieldName("Wisburg"));
            field.DataType.Returns(DBType.Boolean);
            var constraint = new ConstantClause(new FieldExpression(field), ComparisonOperator.NE, DBValue.Create(true));

            // Act
            var builder = new ConstraintBuilder();
            builder.AddClause(constraint);
            var decl = builder.Build();

            // Assert
            decl.Should().BeOfType<BasicConstraintDecl>();
            ((BasicConstraintDecl)decl).DDL.Should().Be($"CHECK (`{field.Name}` IS NOT TRUE)");
        }

        [TestMethod] public void ComparisonConstraint_BooleanValue_False() {
            // Arrange
            var field = Substitute.For<IField>();
            field.Name.Returns(new FieldName("Pelican Town"));
            field.DataType.Returns(DBType.Boolean);
            var constraint = new ConstantClause(new FieldExpression(field), ComparisonOperator.EQ, DBValue.Create(false));

            // Act
            var builder = new ConstraintBuilder();
            builder.AddClause(constraint);
            var decl = builder.Build();

            // Assert
            decl.Should().BeOfType<BasicConstraintDecl>();
            ((BasicConstraintDecl)decl).DDL.Should().Be($"CHECK (`{field.Name}` IS FALSE)");
        }

        [TestMethod] public void ComparisonConstraint_GuidValue() {
            // Arrange
            var field = Substitute.For<IField>();
            field.Name.Returns(new FieldName("Himavanta"));
            field.DataType.Returns(DBType.Guid);
            var anchor = Guid.NewGuid();
            var constraint = new ConstantClause(new FieldExpression(field), ComparisonOperator.NE, DBValue.Create(anchor));

            // Act
            var builder = new ConstraintBuilder();
            builder.AddClause(constraint);
            var decl = builder.Build();

            // Assert
            decl.Should().BeOfType<BasicConstraintDecl>();
            ((BasicConstraintDecl)decl).DDL.Should().Be($"CHECK (`{field.Name}` != UUID_TO_BIN(\"{anchor:D}\"))");
        }

        [TestMethod] public void InclusionConstraint_OneOf_SingleElement() {
            // Arrange
            var field = Substitute.For<IField>();
            field.Name.Returns(new FieldName("Hadestown"));
            var values = new[] { DBValue.Create(81727) };
            field.DataType.Returns(DBType.Int32);
            var constraint = new InclusionClause(new FieldExpression(field), InclusionOperator.In, values);

            // Act
            var builder = new ConstraintBuilder();
            builder.AddClause(constraint);
            var decl = builder.Build();

            // Assert
            decl.Should().BeOfType<BasicConstraintDecl>();
            ((BasicConstraintDecl)decl).DDL.Should().Be($"CHECK (`{field.Name}` IN ({values[0]}))");
        }

        [TestMethod] public void InclusionConstraint_OneOf_MultipleElements() {
            // Arrange
            var field = Substitute.For<IField>();
            field.Name.Returns(new FieldName("Hork-Bajir Valley"));
            field.DataType.Returns(DBType.Double);
            var values = new[] { DBValue.Create(189.6), DBValue.Create(-31.044), DBValue.Create(10515912.22222) };
            var constraint = new InclusionClause(new FieldExpression(field), InclusionOperator.In, values);

            // Act
            var builder = new ConstraintBuilder();
            builder.AddClause(constraint);
            var decl = builder.Build();

            // Assert
            decl.Should().BeOfType<BasicConstraintDecl>();
            ((BasicConstraintDecl)decl).DDL.Should().Be($"CHECK (`{field.Name}` IN ({values[0]}, {values[1]}, {values[2]}))");
        }

        [TestMethod] public void InclusionConstraint_NotOneOf_SingleElement() {
            // Arrange
            var field = Substitute.For<IField>();
            field.Name.Returns(new FieldName("Terabithia"));
            var values = new[] { DBValue.Create(-78125) };
            field.DataType.Returns(DBType.Int32);
            var constraint = new InclusionClause(new FieldExpression(field), InclusionOperator.NotIn, values);

            // Act
            var builder = new ConstraintBuilder();
            builder.AddClause(constraint);
            var decl = builder.Build();

            // Assert
            decl.Should().BeOfType<BasicConstraintDecl>();
            ((BasicConstraintDecl)decl).DDL.Should().Be($"CHECK (`{field.Name}` NOT IN ({values[0]}))");
        }

        [TestMethod] public void InclusionConstraint_NotOneOf_MultipleElements() {
            // Arrange
            var field = Substitute.For<IField>();
            field.Name.Returns(new FieldName("Naomi"));
            field.DataType.Returns(DBType.Single);
            var values = new[] { DBValue.Create(0.00003f), DBValue.Create(125.125521125521f) };
            var constraint = new InclusionClause(new FieldExpression(field), InclusionOperator.NotIn, values);

            // Act
            var builder = new ConstraintBuilder();
            builder.AddClause(constraint);
            var decl = builder.Build();

            // Assert
            decl.Should().BeOfType<BasicConstraintDecl>();
            ((BasicConstraintDecl)decl).DDL.Should().Be($"CHECK (`{field.Name}` NOT IN ({values[0]}, {values[1]}))");
        }

        [TestMethod] public void NullityConstraint_Null() {
            // Arrange
            var field = Substitute.For<IField>();
            field.Name.Returns(new FieldName("Grouchland"));
            var constraint = new NullityClause(field, NullityOperator.IsNull);

            // Act
            var builder = new ConstraintBuilder();
            builder.AddClause(constraint);
            var decl = builder.Build();

            // Assert
            decl.Should().BeOfType<BasicConstraintDecl>();
            ((BasicConstraintDecl)decl).DDL.Should().Be($"CHECK (`{field.Name}` IS NULL)");
        }

        [TestMethod] public void NullityConstraint_NotNull() {
            // Arrange
            var field = Substitute.For<IField>();
            field.Name.Returns(new FieldName("Sillyville"));
            var constraint = new NullityClause(field, NullityOperator.IsNotNull);

            // Act
            var builder = new ConstraintBuilder();
            builder.AddClause(constraint);
            var decl = builder.Build();

            // Assert
            decl.Should().BeOfType<BasicConstraintDecl>();
            ((BasicConstraintDecl)decl).DDL.Should().Be($"CHECK (`{field.Name}` IS NOT NULL)");
        }

        [TestMethod] public void CrossFieldConstraint() {
            // Arrange
            var field0 = Substitute.For<IField>();
            field0.Name.Returns(new FieldName("Newfaire"));
            field0.DataType.Returns(DBType.Int32);
            var field1 = Substitute.For<IField>();
            field1.Name.Returns(new FieldName("Dream Graveyard"));
            field1.DataType.Returns(DBType.Int32);
            var constraint = new CrossFieldClause(new FieldExpression(field0), ComparisonOperator.NE, new FieldExpression(field1));

            // Act
            var builder = new ConstraintBuilder();
            builder.AddClause(constraint);
            var decl = builder.Build();

            // Assert
            decl.Should().BeOfType<BasicConstraintDecl>();
            ((BasicConstraintDecl)decl).DDL.Should().Be($"CHECK (`{field0.Name}` != `{field1.Name}`)");
        }

        [TestMethod] public void CompoundConstraint_AND_LengthBetween_MinThenMax() {
            // Arrange
            var field = Substitute.For<IField>();
            field.Name.Returns(new FieldName("Battle School"));
            field.DataType.Returns(DBType.Text);
            var minimum = 37;
            var maximum = 199;
            var minClause = new ConstantClause(new FieldExpression(FieldFunction.LengthOf, field), ComparisonOperator.GTE, DBValue.Create(minimum));
            var maxClause = new ConstantClause(new FieldExpression(FieldFunction.LengthOf, field), ComparisonOperator.LT, DBValue.Create(maximum));

            // Act
            var builder = new ConstraintBuilder();
            builder.StartClause();
            builder.AddClause(minClause);
            builder.And();
            builder.AddClause(maxClause);
            builder.EndClause();
            var decl = builder.Build();

            // Assert
            decl.Should().BeOfType<BasicConstraintDecl>();
            ((BasicConstraintDecl)decl).DDL.Should().Be($"CHECK (LENGTH(`{field.Name}`) >= {minimum} && LENGTH(`{field.Name}`) < {maximum})");
        }

        [TestMethod] public void CompoundConstraint_AND_LengthBetween_MaxThenMin() {
            // Arrange
            var field = Substitute.For<IField>();
            field.Name.Returns(new FieldName("East Riverside"));
            field.DataType.Returns(DBType.Text);
            var minimum = 2;
            var maximum = 4091;
            var minClause = new ConstantClause(new FieldExpression(FieldFunction.LengthOf, field), ComparisonOperator.GT, DBValue.Create(minimum));
            var maxClause = new ConstantClause(new FieldExpression(FieldFunction.LengthOf, field), ComparisonOperator.LTE, DBValue.Create(maximum));

            // Act
            var builder = new ConstraintBuilder();
            builder.StartClause();
            builder.AddClause(maxClause);
            builder.And();
            builder.AddClause(minClause);
            builder.EndClause();
            var decl = builder.Build();

            // Assert
            decl.Should().BeOfType<BasicConstraintDecl>();
            ((BasicConstraintDecl)decl).DDL.Should().Be($"CHECK (LENGTH(`{field.Name}`) <= {maximum} && LENGTH(`{field.Name}`) > {minimum})");
        }

        [TestMethod] public void CompoundConstraint_AND_MinMaxLengthDifferentFields() {
            // Arrange
            var field0 = Substitute.For<IField>();
            field0.Name.Returns(new FieldName("Meryton"));
            field0.DataType.Returns(DBType.Text);
            var field1 = Substitute.For<IField>();
            field1.Name.Returns(new FieldName("Sheltered Shrubs"));
            field1.DataType.Returns(DBType.Text);
            var minimum = 2;
            var maximum = 4091;
            var minClause = new ConstantClause(new FieldExpression(FieldFunction.LengthOf, field0), ComparisonOperator.GTE, DBValue.Create(minimum));
            var maxClause = new ConstantClause(new FieldExpression(FieldFunction.LengthOf, field1), ComparisonOperator.LTE, DBValue.Create(maximum));

            // Act
            var builder = new ConstraintBuilder();
            builder.StartClause();
            builder.AddClause(minClause);
            builder.And();
            builder.AddClause(maxClause);
            builder.EndClause();
            var decl = builder.Build();

            // Assert
            decl.Should().BeOfType<BasicConstraintDecl>();
            ((BasicConstraintDecl)decl).DDL.Should().Be($"CHECK (LENGTH(`{field0.Name}`) >= {minimum} && LENGTH(`{field1.Name}`) <= {maximum})");
        }

        [TestMethod] public void CompoundConstraint_AND_WithMaxLength() {
            // Arrange
            var field = Substitute.For<IField>();
            field.Name.Returns(new FieldName("Sewage City"));
            field.DataType.Returns(DBType.Text);
            var anchor = "Pinecrest";
            var maximum = 4091;
            var clause0 = new ConstantClause(new FieldExpression(field), ComparisonOperator.LTE, DBValue.Create(anchor));
            var clause1 = new ConstantClause(new FieldExpression(FieldFunction.LengthOf, field), ComparisonOperator.LT, DBValue.Create(maximum));

            // Act
            var builder = new ConstraintBuilder();
            builder.StartClause();
            builder.AddClause(clause0);
            builder.And();
            builder.AddClause(clause1);
            builder.EndClause();
            var decl = builder.Build();

            // Assert
            decl.Should().BeOfType<BasicConstraintDecl>();
            ((BasicConstraintDecl)decl).DDL.Should().Be($"CHECK (`{field.Name}` <= \"{anchor}\" && LENGTH(`{field.Name}`) < {maximum})");
        }

        [TestMethod] public void CompoundConstraint_AND_WithoutMaxLength() {
            // Arrange
            var field = Substitute.For<IField>();
            field.Name.Returns(new FieldName("Dinosaur World"));
            field.DataType.Returns(DBType.Decimal);
            var minimum = 12.0M;
            var maximum = 19.7M;
            var minClause = new ConstantClause(new FieldExpression(field), ComparisonOperator.GT, DBValue.Create(minimum));
            var maxClause = new ConstantClause(new FieldExpression(field), ComparisonOperator.LTE, DBValue.Create(maximum));

            // Act
            var builder = new ConstraintBuilder();
            builder.StartClause();
            builder.AddClause(minClause);
            builder.And();
            builder.AddClause(maxClause);
            builder.EndClause();
            var decl = builder.Build();

            // Assert
            decl.Should().BeOfType<BasicConstraintDecl>();
            ((BasicConstraintDecl)decl).DDL.Should().Be($"CHECK (`{field.Name}` > {minimum} && `{field.Name}` <= {maximum})");
        }

        [TestMethod] public void CompoundConstraint_OR_WithMaxLength() {
            // Arrange
            var field = Substitute.For<IField>();
            field.Name.Returns(new FieldName("Tristram"));
            field.DataType.Returns(DBType.Text);
            var uppperAnchor = 36;
            var lowerAnchor = 100;
            var clause0 = new ConstantClause(new FieldExpression(FieldFunction.LengthOf, field), ComparisonOperator.LTE, DBValue.Create(uppperAnchor));
            var clause1 = new ConstantClause(new FieldExpression(FieldFunction.LengthOf, field), ComparisonOperator.GTE, DBValue.Create(lowerAnchor));

            // Act
            var builder = new ConstraintBuilder();
            builder.StartClause();
            builder.AddClause(clause0);
            builder.Or();
            builder.AddClause(clause1);
            builder.EndClause();
            var decl = builder.Build();

            // Assert
            decl.Should().BeOfType<BasicConstraintDecl>();
            ((BasicConstraintDecl)decl).DDL.Should().Be($"CHECK (LENGTH(`{field.Name}`) <= {uppperAnchor} || LENGTH(`{field.Name}`) >= {lowerAnchor})");
        }

        [TestMethod] public void CompoundConstraint_OR_WithoutMaxLength() {
            // Arrange
            var field = Substitute.For<IField>();
            field.Name.Returns(new FieldName("Junktown"));
            field.DataType.Returns(DBType.Single);
            var upperAnchor = -10941.6f;
            var lowerAnchor = 606.16f;
            var clause0 = new ConstantClause(new FieldExpression(field), ComparisonOperator.LT, DBValue.Create(upperAnchor));
            var clause1 = new ConstantClause(new FieldExpression(field), ComparisonOperator.GT, DBValue.Create(lowerAnchor));

            // Act
            var builder = new ConstraintBuilder();
            builder.StartClause();
            builder.AddClause(clause0);
            builder.Or();
            builder.AddClause(clause1);
            builder.EndClause();
            var decl = builder.Build();

            // Assert
            decl.Should().BeOfType<BasicConstraintDecl>();
            ((BasicConstraintDecl)decl).DDL.Should().Be($"CHECK (`{field.Name}` < {upperAnchor} || `{field.Name}` > {lowerAnchor})");
        }

        [TestMethod] public void CompoundConstraint_AND_OR() {
            // Arrange
            var field = Substitute.For<IField>();
            field.Name.Returns(new FieldName("Chandrapore"));
            field.DataType.Returns(DBType.Single);
            var anchor0 = -331.6f;
            var clause0 = new ConstantClause(new FieldExpression(field), ComparisonOperator.LT, DBValue.Create(anchor0));
            var anchor1 = 5610.33f;
            var clause1 = new ConstantClause(new FieldExpression(field), ComparisonOperator.LT, DBValue.Create(anchor1));
            var anchor2 = 0.0f;
            var clause2 = new ConstantClause(new FieldExpression(field), ComparisonOperator.GTE, DBValue.Create(anchor2));

            // Act
            var builder = new ConstraintBuilder();
            builder.StartClause();
            builder.AddClause(clause0);
            builder.Or();
            builder.StartClause();
            builder.AddClause(clause1);
            builder.And();
            builder.AddClause(clause2);
            builder.EndClause();
            builder.EndClause();
            var decl = builder.Build();

            // Assert
            decl.Should().BeOfType<BasicConstraintDecl>();
            ((BasicConstraintDecl)decl).DDL.Should().Be($"CHECK (`{field.Name}` < {anchor0} || (`{field.Name}` < {anchor1} && `{field.Name}` >= {anchor2}))");
        }

        [TestMethod] public void CompoundConstraint_Complex() {
            // Arrange
            var field0 = Substitute.For<IField>();
            field0.Name.Returns(new FieldName("Leá Monde"));
            field0.DataType.Returns(DBType.Int32);
            var field1 = Substitute.For<IField>();
            field1.Name.Returns(new FieldName("Malgudi"));
            field1.DataType.Returns(DBType.Int32);
            var clause0 = new CrossFieldClause(new FieldExpression(field0), ComparisonOperator.EQ, new FieldExpression(field1));
            var clause1 = new ConstantClause(new FieldExpression(field1), ComparisonOperator.GTE, DBValue.Create(73));
            var clause2 = new InclusionClause(new FieldExpression(field0), InclusionOperator.In, new[] { DBValue.Create(0), DBValue.Create(1), DBValue.Create(2), DBValue.Create(3), DBValue.Create(4) });
            var clause3 = new ConstantClause(new FieldExpression(field1), ComparisonOperator.GT, DBValue.Create(16));
            var clause4 = new ConstantClause(new FieldExpression(field1), ComparisonOperator.LT, DBValue.Create(41));

            // Act
            var builder = new ConstraintBuilder();
            builder.StartClause();
            builder.StartClause();
            builder.AddClause(clause0);
            builder.Or();
            builder.AddClause(clause1);
            builder.EndClause();
            builder.And();
            builder.StartClause();
            builder.AddClause(clause2);
            builder.Or();
            builder.StartClause();
            builder.AddClause(clause3);
            builder.And();
            builder.AddClause(clause4);
            builder.EndClause();
            builder.EndClause();
            builder.EndClause();
            var decl = builder.Build();

            // Assert
            decl.Should().BeOfType<BasicConstraintDecl>();
            ((BasicConstraintDecl)decl).DDL.Should().Be(
                "CHECK (" +
                  $"(`{field0.Name}` = `{field1.Name}` || `{field1.Name}` >= 73) && " +
                    $"(`{field0.Name}` IN (0, 1, 2, 3, 4) || " +
                      $"(`{field1.Name}` > 16 && `{field1.Name}` < 41)" +
                    ")" +
                ")"
            );
        }

        [TestMethod] public void NamedConstraint() {
            // Arrange
            var field = Substitute.For<IField>();
            field.Name.Returns(new FieldName("Eternos"));
            field.DataType.Returns(DBType.Int16);
            var anchor = (short)3516;
            var constraint = new ConstantClause(new FieldExpression(field), ComparisonOperator.LTE, DBValue.Create(anchor));
            var checkName = new CheckName("ChkConstraint");

            // Act
            var builder = new ConstraintBuilder();
            builder.SetName(checkName);
            builder.AddClause(constraint);
            var decl = builder.Build();

            // Assert
            decl.Should().BeOfType<BasicConstraintDecl>();
            ((BasicConstraintDecl)decl).DDL.Should().Be($"CONSTRAINT `{checkName}` CHECK (`{field.Name}` <= {anchor})");
        }

        [TestMethod] public void ConstraintNameIsMaximumLength() {
            // Arrange
            var field = Substitute.For<IField>();
            field.Name.Returns(new FieldName("Menaphos"));
            field.DataType.Returns(DBType.Int8);
            var anchor = (sbyte)-124;
            var constraint = new ConstantClause(new FieldExpression(field), ComparisonOperator.NE, DBValue.Create(anchor));
            var checkName = new CheckName(new string('i', 64));

            // Act
            var builder = new ConstraintBuilder();
            builder.SetName(checkName);
            builder.AddClause(constraint);
            var decl = builder.Build();

            // Assert
            decl.Should().BeOfType<BasicConstraintDecl>();
            ((BasicConstraintDecl)decl).DDL.Should().Be($"CONSTRAINT `{checkName}` CHECK (`{field.Name}` != {anchor})");
        }

        [TestMethod] public void ConstraintNameExceedsMaximumLength_IsError() {
            // Arrange
            var checkName = new CheckName(new string('P', 46102));

            // Act
            var builder = new ConstraintBuilder();
            var action = () => builder.SetName(checkName);

            // Assert
            action.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining("[MySQL]")
                .WithMessageContaining(checkName.ToString())
                .WithMessageContaining("exceeds the maximum of 64 characters");
        }
    }
}
