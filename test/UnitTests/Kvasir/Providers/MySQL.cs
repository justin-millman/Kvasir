using FluentAssertions;
using Kvasir.Core;
using Kvasir.Exceptions;
using Kvasir.Providers.MySQL;
using Kvasir.Schema;
using Kvasir.Transcription;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;

using static UT.Kvasir.Providers.MySql;
using Translator = Kvasir.Translation.Translator;

namespace UT.Kvasir.Providers {
    [TestClass, TestCategory("MySQL - Keys (DDL)")]
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

    [TestClass, TestCategory("MySQL - Foreign Keys (DDL)")]
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

    [TestClass, TestCategory("MySQL - Constraints (DDL)")]
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

        [TestMethod] public void ComparisonConstraint_DateOnly() {
            // Arrange
            var field = Substitute.For<IField>();
            field.Name.Returns(new FieldName("East Highland"));
            field.DataType.Returns(DBType.Date);
            var anchor = new DateOnly(1377, 4, 9);
            var constraint = new ConstantClause(new FieldExpression(field), ComparisonOperator.LT, DBValue.Create(anchor));

            // Act
            var builder = new ConstraintBuilder();
            builder.AddClause(constraint);
            var decl = builder.Build();

            // Assert
            decl.Should().BeOfType<BasicConstraintDecl>();
            ((BasicConstraintDecl)decl).DDL.Should().Be($"CHECK (`{field.Name}` < DATE \"1377-04-09\")");
        }

        [TestMethod] public void ComparisonConstraint_DateTimeValue_NoTime() {
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
            ((BasicConstraintDecl)decl).DDL.Should().Be($"CHECK (`{field.Name}` != TIMESTAMP \"1865-05-30 00:00:00\")");
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
            ((BasicConstraintDecl)decl).DDL.Should().Be($"CHECK (`{field.Name}` <= TIMESTAMP \"2188-11-04 01:07:02\")");
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

    [TestClass, TestCategory("MySQL - Fields (DDL)")]
    public class MySqlFieldTests {
        [TestMethod] public void Type_Boolean() {
            // Arrange
            var name = new FieldName("Peshawar");
            var type = DBType.Boolean;

            // Act
            var builder = new FieldBuilder();
            builder.SetName(name);
            builder.SetDataType(type);
            var intermediate = builder.Build();
            var decl = intermediate.Build();

            // Assert
            intermediate.Name.Should().Be(name.ToString());
            decl.Should().Be($"`{name}` BOOLEAN");
        }

        [TestMethod] public void Type_Int8() {
            // Arrange
            var name = new FieldName("Lucknow");
            var type = DBType.Int8;

            // Act
            var builder = new FieldBuilder();
            builder.SetName(name);
            builder.SetDataType(type);
            var intermediate = builder.Build();
            var decl = intermediate.Build();

            // Assert
            intermediate.Name.Should().Be(name.ToString());
            decl.Should().Be($"`{name}` TINYINT");
        }

        [TestMethod] public void Type_UInt8() {
            // Arrange
            var name = new FieldName("Fez");
            var type = DBType.UInt8;

            // Act
            var builder = new FieldBuilder();
            builder.SetName(name);
            builder.SetDataType(type);
            var intermediate = builder.Build();
            var decl = intermediate.Build();

            // Assert
            intermediate.Name.Should().Be(name.ToString());
            decl.Should().Be($"`{name}` TINYINT UNSIGNED");
        }

        [TestMethod] public void Type_Int16() {
            // Arrange
            var name = new FieldName("Moroni");
            var type = DBType.Int16;

            // Act
            var builder = new FieldBuilder();
            builder.SetName(name);
            builder.SetDataType(type);
            var intermediate = builder.Build();
            var decl = intermediate.Build();

            // Assert
            intermediate.Name.Should().Be(name.ToString());
            decl.Should().Be($"`{name}` SMALLINT");
        }

        [TestMethod] public void Type_UInt16() {
            // Arrange
            var name = new FieldName("Malé");
            var type = DBType.UInt16;

            // Act
            var builder = new FieldBuilder();
            builder.SetName(name);
            builder.SetDataType(type);
            var intermediate = builder.Build();
            var decl = intermediate.Build();

            // Assert
            intermediate.Name.Should().Be(name.ToString());
            decl.Should().Be($"`{name}` SMALLINT UNSIGNED");
        }

        [TestMethod] public void Type_Int32() {
            // Arrange
            var name = new FieldName("Port Vila");
            var type = DBType.Int32;

            // Act
            var builder = new FieldBuilder();
            builder.SetName(name);
            builder.SetDataType(type);
            var intermediate = builder.Build();
            var decl = intermediate.Build();

            // Assert
            intermediate.Name.Should().Be(name.ToString());
            decl.Should().Be($"`{name}` INT");
        }

        [TestMethod] public void Type_UInt32() {
            // Arrange
            var name = new FieldName("Mandalay");
            var type = DBType.UInt32;

            // Act
            var builder = new FieldBuilder();
            builder.SetName(name);
            builder.SetDataType(type);
            var intermediate = builder.Build();
            var decl = intermediate.Build();

            // Assert
            intermediate.Name.Should().Be(name.ToString());
            decl.Should().Be($"`{name}` INT UNSIGNED");
        }

        [TestMethod] public void Type_Int64() {
            // Arrange
            var name = new FieldName("Oran");
            var type = DBType.Int64;

            // Act
            var builder = new FieldBuilder();
            builder.SetName(name);
            builder.SetDataType(type);
            var intermediate = builder.Build();
            var decl = intermediate.Build();

            // Assert
            intermediate.Name.Should().Be(name.ToString());
            decl.Should().Be($"`{name}` BIGINT");
        }

        [TestMethod] public void Type_UInt64() {
            // Arrange
            var name = new FieldName("Gori");
            var type = DBType.UInt64;

            // Act
            var builder = new FieldBuilder();
            builder.SetName(name);
            builder.SetDataType(type);
            var intermediate = builder.Build();
            var decl = intermediate.Build();

            // Assert
            intermediate.Name.Should().Be(name.ToString());
            decl.Should().Be($"`{name}` BIGINT UNSIGNED");
        }

        [TestMethod] public void Type_Single() {
            // Arrange
            var name = new FieldName("Luanda");
            var type = DBType.Single;

            // Act
            var builder = new FieldBuilder();
            builder.SetName(name);
            builder.SetDataType(type);
            var intermediate = builder.Build();
            var decl = intermediate.Build();

            // Assert
            intermediate.Name.Should().Be(name.ToString());
            decl.Should().Be($"`{name}` FLOAT");
        }

        [TestMethod] public void Type_Double() {
            // Arrange
            var name = new FieldName("Nagpur");
            var type = DBType.Double;

            // Act
            var builder = new FieldBuilder();
            builder.SetName(name);
            builder.SetDataType(type);
            var intermediate = builder.Build();
            var decl = intermediate.Build();

            // Assert
            intermediate.Name.Should().Be(name.ToString());
            decl.Should().Be($"`{name}` DOUBLE");
        }

        [TestMethod] public void Type_Decimal() {
            // Arrange
            var name = new FieldName("Kharkiv");
            var type = DBType.Decimal;

            // Act
            var builder = new FieldBuilder();
            builder.SetName(name);
            builder.SetDataType(type);
            var intermediate = builder.Build();
            var decl = intermediate.Build();

            // Assert
            intermediate.Name.Should().Be(name.ToString());
            decl.Should().Be($"`{name}` DECIMAL");
        }

        [TestMethod] public void Type_VarcharDefaultLength() {
            // Arrange
            var name = new FieldName("Kobe");
            var type = DBType.Text;

            // Act
            var builder = new FieldBuilder();
            builder.SetName(name);
            builder.SetDataType(type);
            var intermediate = builder.Build();
            var decl = intermediate.Build();

            // Assert
            intermediate.Name.Should().Be(name.ToString());
            decl.Should().Be($"`{name}` VARCHAR(255)");
        }

        [TestMethod] public void Type_VarcharCustomLength() {
            // Arrange
            var name = new FieldName("Fortaleza");
            var type = DBType.Text;
            var maxLength = 173UL;

            // Act
            var builder = new FieldBuilder();
            builder.SetName(name);
            builder.SetDataType(type);
            var intermediate = builder.Build();
            intermediate.EnforceMaximumLength(maxLength);
            var decl = intermediate.Build();

            // Assert
            intermediate.Name.Should().Be(name.ToString());
            decl.Should().Be($"`{name}` VARCHAR({maxLength})");
        }

        [TestMethod] public void Type_VarcharOverlongLength() {
            // Arrange
            var name = new FieldName("Corrientes");
            var type = DBType.Text;
            var maxLength = 65536UL;

            // Act
            var builder = new FieldBuilder();
            builder.SetName(name);
            builder.SetDataType(type);
            var intermediate = builder.Build();
            intermediate.EnforceMaximumLength(maxLength);
            var decl = intermediate.Build();

            // Assert
            intermediate.Name.Should().Be(name.ToString());
            decl.Should().Be($"`{name}` VARCHAR(65535)");
        }

        [TestMethod] public void Type_Character() {
            // Arrange
            var name = new FieldName("Derry");
            var type = DBType.Character;

            // Act
            var builder = new FieldBuilder();
            builder.SetName(name);
            builder.SetDataType(type);
            var intermediate = builder.Build();
            var decl = intermediate.Build();

            // Assert
            intermediate.Name.Should().Be(name.ToString());
            decl.Should().Be($"`{name}` CHAR(1)");
        }

        [TestMethod] public void Type_Date() {
            // Arrange
            var name = new FieldName("Cuiabá");
            var type = DBType.Date;

            // Act
            var builder = new FieldBuilder();
            builder.SetName(name);
            builder.SetDataType(type);
            var intermediate = builder.Build();
            var decl = intermediate.Build();

            // Assert
            intermediate.Name.Should().Be(name.ToString());
            decl.Should().Be($"`{name}` DATE");
        }

        [TestMethod] public void Type_DateTime() {
            // Arrange
            var name = new FieldName("Jeddah");
            var type = DBType.DateTime;

            // Act
            var builder = new FieldBuilder();
            builder.SetName(name);
            builder.SetDataType(type);
            var intermediate = builder.Build();
            var decl = intermediate.Build();

            // Assert
            intermediate.Name.Should().Be(name.ToString());
            decl.Should().Be($"`{name}` DATETIME");
        }

        [TestMethod] public void Type_Guid() {
            // Arrange
            var name = new FieldName("St. George's");
            var type = DBType.Guid;

            // Act
            var builder = new FieldBuilder();
            builder.SetName(name);
            builder.SetDataType(type);
            var intermediate = builder.Build();
            var decl = intermediate.Build();

            // Assert
            intermediate.Name.Should().Be(name.ToString());
            decl.Should().Be($"`{name}` BINARY(16)");
        }

        [TestMethod] public void Type_Enumeration() {
            // Arrange
            var name = new FieldName("Astana");
            var type = DBType.Enumeration;
            var values = new[] { DBValue.Create("Hamilton"), DBValue.Create("Swansea"), DBValue.Create("Wuhan") };

            // Act
            var builder = new FieldBuilder();
            builder.SetName(name);
            builder.SetDataType(type);
            builder.SetAllowedValues(values);
            var intermediate = builder.Build();
            var decl = intermediate.Build();

            // Assert
            intermediate.Name.Should().Be(name.ToString());
            decl.Should().Be($"`{name}` ENUM({string.Join(", ", values.Select(v => "\"" + v.Datum.ToString() + "\""))})");
        }

        [TestMethod] public void NotNullable() {
            // Arrange
            var name = new FieldName("Wrexham");
            var type = DBType.Int32;

            // Act
            var builder = new FieldBuilder();
            builder.SetName(name);
            builder.SetDataType(type);
            builder.SetNullability(IsNullable.No);
            var intermediate = builder.Build();
            var decl = intermediate.Build();

            // Assert
            intermediate.Name.Should().Be(name.ToString());
            decl.Should().Be($"`{name}` INT NOT NULL");
        }

        [TestMethod] public void Nullable() {
            // Arrange
            var name = new FieldName("Porto-Novo");
            var type = DBType.Int32;

            // Act
            var builder = new FieldBuilder();
            builder.SetName(name);
            builder.SetDataType(type);
            builder.SetNullability(IsNullable.Yes);
            var intermediate = builder.Build();
            var decl = intermediate.Build();

            // Assert
            intermediate.Name.Should().Be(name.ToString());
            decl.Should().Be($"`{name}` INT");
        }

        [TestMethod] public void Default_Boolean_True() {
            // Arrange
            var name = new FieldName("Freetown");
            var type = DBType.Boolean;
            var defaultValue = true;

            // Act
            var builder = new FieldBuilder();
            builder.SetName(name);
            builder.SetDataType(type);
            builder.SetDefaultValue(DBValue.Create(defaultValue));
            var intermediate = builder.Build();
            var decl = intermediate.Build();

            // Assert
            intermediate.Name.Should().Be(name.ToString());
            decl.Should().Be($"`{name}` BOOLEAN DEFAULT TRUE");
        }

        [TestMethod] public void Default_Boolean_False() {
            // Arrange
            var name = new FieldName("Georgetown");
            var type = DBType.Boolean;
            var defaultValue = false;

            // Act
            var builder = new FieldBuilder();
            builder.SetName(name);
            builder.SetDataType(type);
            builder.SetDefaultValue(DBValue.Create(defaultValue));
            var intermediate = builder.Build();
            var decl = intermediate.Build();

            // Assert
            intermediate.Name.Should().Be(name.ToString());
            decl.Should().Be($"`{name}` BOOLEAN DEFAULT FALSE");
        }

        [TestMethod] public void Default_Int8() {
            // Arrange
            var name = new FieldName("Nusantara");
            var type = DBType.Int8;
            var defaultValue = (sbyte)103;

            // Act
            var builder = new FieldBuilder();
            builder.SetName(name);
            builder.SetDataType(type);
            builder.SetDefaultValue(DBValue.Create(defaultValue));
            var intermediate = builder.Build();
            var decl = intermediate.Build();

            // Assert
            intermediate.Name.Should().Be(name.ToString());
            decl.Should().Be($"`{name}` TINYINT DEFAULT {defaultValue}");
        }

        [TestMethod] public void Default_UInt8() {
            // Arrange
            var name = new FieldName("Putrajaya");
            var type = DBType.UInt8;
            var defaultValue = (byte)9;

            // Act
            var builder = new FieldBuilder();
            builder.SetName(name);
            builder.SetDataType(type);
            builder.SetDefaultValue(DBValue.Create(defaultValue));
            var intermediate = builder.Build();
            var decl = intermediate.Build();

            // Assert
            intermediate.Name.Should().Be(name.ToString());
            decl.Should().Be($"`{name}` TINYINT UNSIGNED DEFAULT {defaultValue}");
        }

        [TestMethod] public void Default_Int16() {
            // Arrange
            var name = new FieldName("Kuwait City");
            var type = DBType.Int16;
            var defaultValue = (short)-4810;

            // Act
            var builder = new FieldBuilder();
            builder.SetName(name);
            builder.SetDataType(type);
            builder.SetDefaultValue(DBValue.Create(defaultValue));
            var intermediate = builder.Build();
            var decl = intermediate.Build();

            // Assert
            intermediate.Name.Should().Be(name.ToString());
            decl.Should().Be($"`{name}` SMALLINT DEFAULT {defaultValue}");
        }

        [TestMethod] public void Default_UInt16() {
            // Arrange
            var name = new FieldName("Libreville");
            var type = DBType.UInt16;
            var defaultValue = (ushort)50666;

            // Act
            var builder = new FieldBuilder();
            builder.SetName(name);
            builder.SetDataType(type);
            builder.SetDefaultValue(DBValue.Create(defaultValue));
            var intermediate = builder.Build();
            var decl = intermediate.Build();

            // Assert
            intermediate.Name.Should().Be(name.ToString());
            decl.Should().Be($"`{name}` SMALLINT UNSIGNED DEFAULT {defaultValue}");
        }

        [TestMethod] public void Default_Int32() {
            // Arrange
            var name = new FieldName("Lobamba");
            var type = DBType.Int32;
            var defaultValue = 871920004;

            // Act
            var builder = new FieldBuilder();
            builder.SetName(name);
            builder.SetDataType(type);
            builder.SetDefaultValue(DBValue.Create(defaultValue));
            var intermediate = builder.Build();
            var decl = intermediate.Build();

            // Assert
            intermediate.Name.Should().Be(name.ToString());
            decl.Should().Be($"`{name}` INT DEFAULT {defaultValue}");
        }

        [TestMethod] public void Default_UInt32() {
            // Arrange
            var name = new FieldName("Luxembourg");
            var type = DBType.UInt32;
            var defaultValue = 0U;

            // Act
            var builder = new FieldBuilder();
            builder.SetName(name);
            builder.SetDataType(type);
            builder.SetDefaultValue(DBValue.Create(defaultValue));
            var intermediate = builder.Build();
            var decl = intermediate.Build();

            // Assert
            intermediate.Name.Should().Be(name.ToString());
            decl.Should().Be($"`{name}` INT UNSIGNED DEFAULT {defaultValue}");
        }

        [TestMethod] public void Default_Int64() {
            // Arrange
            var name = new FieldName("Oranjestad");
            var type = DBType.Int64;
            var defaultValue = -127182901258125L;

            // Act
            var builder = new FieldBuilder();
            builder.SetName(name);
            builder.SetDataType(type);
            builder.SetDefaultValue(DBValue.Create(defaultValue));
            var intermediate = builder.Build();
            var decl = intermediate.Build();

            // Assert
            intermediate.Name.Should().Be(name.ToString());
            decl.Should().Be($"`{name}` BIGINT DEFAULT {defaultValue}");
        }

        [TestMethod] public void Default_UInt64() {
            // Arrange
            var name = new FieldName("Thimphu");
            var type = DBType.UInt64;
            var defaultValue = 87100537;

            // Act
            var builder = new FieldBuilder();
            builder.SetName(name);
            builder.SetDataType(type);
            builder.SetDefaultValue(DBValue.Create(defaultValue));
            var intermediate = builder.Build();
            var decl = intermediate.Build();

            // Assert
            intermediate.Name.Should().Be(name.ToString());
            decl.Should().Be($"`{name}` BIGINT UNSIGNED DEFAULT {defaultValue}");
        }

        [TestMethod] public void Default_Single() {
            // Arrange
            var name = new FieldName("Samarkand");
            var type = DBType.Single;
            var defaultValue = -909.44f;

            // Act
            var builder = new FieldBuilder();
            builder.SetName(name);
            builder.SetDataType(type);
            builder.SetDefaultValue(DBValue.Create(defaultValue));
            var intermediate = builder.Build();
            var decl = intermediate.Build();

            // Assert
            intermediate.Name.Should().Be(name.ToString());
            decl.Should().Be($"`{name}` FLOAT DEFAULT {defaultValue}");
        }

        [TestMethod] public void Default_Double() {
            // Arrange
            var name = new FieldName("Axum");
            var type = DBType.Double;
            var defaultValue = -77.777;

            // Act
            var builder = new FieldBuilder();
            builder.SetName(name);
            builder.SetDataType(type);
            builder.SetDefaultValue(DBValue.Create(defaultValue));
            var intermediate = builder.Build();
            var decl = intermediate.Build();

            // Assert
            intermediate.Name.Should().Be(name.ToString());
            decl.Should().Be($"`{name}` DOUBLE DEFAULT {defaultValue}");
        }

        [TestMethod] public void Default_Decimal() {
            // Arrange
            var name = new FieldName("Agadez");
            var type = DBType.Decimal;
            var defaultValue = -11.309M;

            // Act
            var builder = new FieldBuilder();
            builder.SetName(name);
            builder.SetDataType(type);
            builder.SetDefaultValue(DBValue.Create(defaultValue));
            var intermediate = builder.Build();
            var decl = intermediate.Build();

            // Assert
            intermediate.Name.Should().Be(name.ToString());
            decl.Should().Be($"`{name}` DECIMAL DEFAULT {defaultValue}");
        }

        [TestMethod] public void Default_VarcharDefaultLength() {
            // Arrange
            var name = new FieldName("Mbanza Kongo");
            var type = DBType.Text;
            var defaultValue = "Izamal";

            // Act
            var builder = new FieldBuilder();
            builder.SetName(name);
            builder.SetDataType(type);
            builder.SetDefaultValue(DBValue.Create(defaultValue));
            var intermediate = builder.Build();
            var decl = intermediate.Build();

            // Assert
            intermediate.Name.Should().Be(name.ToString());
            decl.Should().Be($"`{name}` VARCHAR(255) DEFAULT \"{defaultValue}\"");
        }

        [TestMethod] public void Default_VarcharCustomLength() {
            // Arrange
            var name = new FieldName("Oaxaca");
            var type = DBType.Text;
            var maxLength = 88UL;
            var defaultValue = "Byblos";

            // Act
            var builder = new FieldBuilder();
            builder.SetName(name);
            builder.SetDataType(type);
            builder.SetDefaultValue(DBValue.Create(defaultValue));
            var intermediate = builder.Build();
            intermediate.EnforceMaximumLength(maxLength);
            var decl = intermediate.Build();

            // Assert
            intermediate.Name.Should().Be(name.ToString());
            decl.Should().Be($"`{name}` VARCHAR({maxLength}) DEFAULT \"{defaultValue}\"");
        }

        [TestMethod] public void Default_Character() {
            // Arrange
            var name = new FieldName("Madurai");
            var type = DBType.Character;
            var defaultValue = 'b';

            // Act
            var builder = new FieldBuilder();
            builder.SetName(name);
            builder.SetDataType(type);
            builder.SetDefaultValue(DBValue.Create(defaultValue));
            var intermediate = builder.Build();
            var decl = intermediate.Build();

            // Assert
            intermediate.Name.Should().Be(name.ToString());
            decl.Should().Be($"`{name}` CHAR(1) DEFAULT \"{defaultValue}\"");
        }

        [TestMethod] public void Default_Date() {
            // Arrange
            var name = new FieldName("Kazan");
            var type = DBType.Date;
            var defaultValue = new DateOnly(1884, 5, 25);

            // Act
            var builder = new FieldBuilder();
            builder.SetName(name);
            builder.SetDataType(type);
            builder.SetDefaultValue(DBValue.Create(defaultValue));
            var intermediate = builder.Build();
            var decl = intermediate.Build();

            // Assert
            intermediate.Name.Should().Be(name.ToString());
            decl.Should().Be($"`{name}` DATE DEFAULT DATE \"1884-05-25\"");
        }

        [TestMethod] public void Deafult_DateTime_NoTime() {
            // Arrange
            var name = new FieldName("Luang Prabang");
            var type = DBType.DateTime;
            var defaultValue = new DateTime(2009, 10, 08);

            // Act
            var builder = new FieldBuilder();
            builder.SetName(name);
            builder.SetDataType(type);
            builder.SetDefaultValue(DBValue.Create(defaultValue));
            var intermediate = builder.Build();
            var decl = intermediate.Build();

            // Assert
            intermediate.Name.Should().Be(name.ToString());
            decl.Should().Be($"`{name}` DATETIME DEFAULT TIMESTAMP \"2009-10-08 00:00:00\"");
        }

        [TestMethod] public void Deafult_DateTime_WithTime() {
            // Arrange
            var name = new FieldName("Cádiz");
            var type = DBType.DateTime;
            var defaultValue = new DateTime(1655, 1, 11, 3, 58, 44);

            // Act
            var builder = new FieldBuilder();
            builder.SetName(name);
            builder.SetDataType(type);
            builder.SetDefaultValue(DBValue.Create(defaultValue));
            var intermediate = builder.Build();
            var decl = intermediate.Build();

            // Assert
            intermediate.Name.Should().Be(name.ToString());
            decl.Should().Be($"`{name}` DATETIME DEFAULT TIMESTAMP \"1655-01-11 03:58:44\"");
        }

        [TestMethod] public void Default_Guid() {
            // Arrange
            var name = new FieldName("Heraklion");
            var type = DBType.Guid;
            var defaultValue = Guid.NewGuid();

            // Act
            var builder = new FieldBuilder();
            builder.SetName(name);
            builder.SetDataType(type);
            builder.SetDefaultValue(DBValue.Create(defaultValue));
            var intermediate = builder.Build();
            var decl = intermediate.Build();

            // Assert
            intermediate.Name.Should().Be(name.ToString());
            decl.Should().Be($"`{name}` BINARY(16) DEFAULT UUID_TO_BIN(\"{defaultValue:D}\")");
        }

        [TestMethod] public void Default_Enumeration() {
            // Arrange
            var name = new FieldName("Syracuse");
            var type = DBType.Enumeration;
            var values = new[] { DBValue.Create("Dubrovnik"), DBValue.Create("Colchester"), DBValue.Create("Geelong"), DBValue.Create("Valparaíso") };
            var defaultValue = values[1].Datum;

            // Act
            var builder = new FieldBuilder();
            builder.SetName(name);
            builder.SetDataType(type);
            builder.SetAllowedValues(values);
            builder.SetDefaultValue(DBValue.Create(defaultValue));
            var intermediate = builder.Build();
            var decl = intermediate.Build();

            // Assert
            intermediate.Name.Should().Be(name.ToString());
            decl.Should().Be($"`{name}` ENUM({string.Join(", ", values.Select(v => "\"" + v.Datum.ToString() + "\""))}) DEFAULT \"{defaultValue}\"");
        }

        [TestMethod] public void Default_Null() {
            // Arrange
            var name = new FieldName("Arequipa");
            var type = DBType.Guid;

            // Act
            var builder = new FieldBuilder();
            builder.SetName(name);
            builder.SetDataType(type);
            builder.SetDefaultValue(DBValue.NULL);
            var intermediate = builder.Build();
            var decl = intermediate.Build();

            // Assert
            intermediate.Name.Should().Be(name.ToString());
            decl.Should().Be($"`{name}` BINARY(16) DEFAULT NULL");
        }

        [TestMethod] public void VarcharWithUppercaseVarcharInDefaultValue() {
            // Arrange
            var name = new FieldName("Tartu");
            var type = DBType.Text;
            var maxLength = 4501UL;
            var defaultValue = "DEFAULT CONTAINS THE WORD VARCHAR(255) WITH SPACES";

            // Act
            var builder = new FieldBuilder();
            builder.SetName(name);
            builder.SetDataType(type);
            builder.SetDefaultValue(DBValue.Create(defaultValue));
            var intermediate = builder.Build();
            intermediate.EnforceMaximumLength(maxLength);
            var decl = intermediate.Build();

            // Assert
            intermediate.Name.Should().Be(name.ToString());
            decl.Should().Be($"`{name}` VARCHAR({maxLength}) DEFAULT \"{defaultValue}\"");
        }

        [TestMethod] public void FieldNameIsMaximumLength() {
            // Arrange
            var name = new FieldName(new string('L', 64));
            var type = DBType.Boolean;

            // Act
            var builder = new FieldBuilder();
            builder.SetName(name);
            builder.SetDataType(type);
            var intermediate = builder.Build();
            var decl = intermediate.Build();

            // Assert
            intermediate.Name.Should().Be(name.ToString());
            decl.Should().Be($"`{name}` BOOLEAN");
        }

        [TestMethod] public void FieldNameExceedsMaximumLength_IsError() {
            // Arrange
            var fieldName = new FieldName(new string('s', 65));

            // Act
            var builder = new FieldBuilder();
            var action = () => builder.SetName(fieldName);

            // Assert
            action.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining("[MySQL]")
                .WithMessageContaining(fieldName.ToString())
                .WithMessageContaining("exceeds the maximum of 64 characters");
        }
    }

    [TestClass, TestCategory("MySQL - Tables (DDL)")]
    public class MySqlTableTests {
        [TestMethod] public void RegularFields() {
            // Arrange
            var name = new TableName("TheTable");
            var field0 = new FieldDecl(new FieldName("Ojai"), "`Ojai` INT UNSIGNED NOT NULL");
            var field1 = new FieldDecl(new FieldName("West Hartford"), "`West Hartford` BOOLEAN NOT NULL DEFAULT TRUE");
            var field2 = new FieldDecl(new FieldName("Blackfoot"), "`Blackfoot` DOUBLE");
            var pk = new SqlSnippet($"PRIMARY KEY (`{field0.Name}`)");

            // Act
            var builder = new TableBuilder();
            builder.SetName(name);
            builder.AddFieldDeclaration(field0);
            builder.AddFieldDeclaration(field1);
            builder.AddFieldDeclaration(field2);
            builder.SetPrimaryKeyDeclaration(pk);
            var decl = builder.Build();

            // Assert
            decl.Should().Be(
                $"CREATE TABLE IF NOT EXISTS `{name}`\n" +
                "`Ojai` INT UNSIGNED NOT NULL\n" +
                "`West Hartford` BOOLEAN NOT NULL DEFAULT TRUE\n" +
                "`Blackfoot` DOUBLE\n" +
                pk.ToString()
            );
        }

        [TestMethod] public void VarcharFields() {
            // Arrange
            var name = new TableName("HaShulkhan");
            var field0 = new FieldDecl(new FieldName("East Rutherford"), "`East Rutherford` VARCHAR(255) NOT NULL");
            var field1 = new FieldDecl(new FieldName("West Bloomfield"), "`West Bloomfield` VARCHAR(255) NOT NULL");
            var constraint0 = new MaxLengthConstraintDecl(field0.Name, 153);
            var constraint1 = new MaxLengthConstraintDecl(field1.Name, 22);
            var pk = new SqlSnippet($"PRIMARY KEY (`{field0.Name}`, `{field1.Name}`)");

            // Act
            var builder = new TableBuilder();
            builder.SetName(name);
            builder.AddFieldDeclaration(field0);
            builder.AddFieldDeclaration(field1);
            builder.SetPrimaryKeyDeclaration(pk);
            builder.AddCheckConstraintDeclaration(constraint0);
            builder.AddCheckConstraintDeclaration(constraint1);
            var decl = builder.Build();

            // Assert
            decl.Should().Be(
                $"CREATE TABLE IF NOT EXISTS `{name}`\n" +
                $"`East Rutherford` VARCHAR({constraint0.MaxLength}) NOT NULL\n" +
                $"`West Bloomfield` VARCHAR({constraint1.MaxLength}) NOT NULL\n" +
                pk.ToString()
            );
        }

        [TestMethod] public void CandidateKeys() {
            // Arrange
            var name = new TableName("LaMesa");
            var field0 = new FieldDecl(new FieldName("Greenville"), "`Greenville` VARCHAR(255) NOT NULL");
            var field1 = new FieldDecl(new FieldName("Buffalo Grove"), "`Buffalo Grove` BIGINT UNSIGNED NOT NULL");
            var field2 = new FieldDecl(new FieldName("Grambling"), "`Grambling` FLOAT NOT NULL");
            var pk = new SqlSnippet($"PRIMARY KEY (`{field1.Name}`)");
            var ck = new SqlSnippet($"CONSTRAINT unique_0 UNIQUE (`{field2.Name}`)");

            // Act
            var builder = new TableBuilder();
            builder.SetName(name);
            builder.AddFieldDeclaration(field0);
            builder.AddFieldDeclaration(field1);
            builder.AddFieldDeclaration(field2);
            builder.SetPrimaryKeyDeclaration(pk);
            builder.AddCandidateKeyDeclaration(ck);
            var decl = builder.Build();

            // Assert
            decl.Should().Be(
                $"CREATE TABLE IF NOT EXISTS `{name}`\n" +
                "`Greenville` VARCHAR(255) NOT NULL\n" +
                "`Buffalo Grove` BIGINT UNSIGNED NOT NULL\n" +
                "`Grambling` FLOAT NOT NULL\n" +
                $"{pk}\n" +
                ck.ToString()
            );
        }

        [TestMethod] public void ForeignKeys() {
            // Arrange
            var name = new TableName("LaTable");
            var field0 = new FieldDecl(new FieldName("Muskegon"), "`Muskegon` SMALLINT NOT NULL");
            var field1 = new FieldDecl(new FieldName("Poughkeepsie"), "`Poughkeepsie` TINYINT UNSIGNED NOT NULL");
            var pk = new SqlSnippet($"PRIMARY KEY (`{field0.Name}`, `{field1.Name}`)");
            var fk = new SqlSnippet($"FOREIGN KEY (`{field0.Name}`) REFERENCES `DerTisch` (`Natchitoches`)");

            // Act
            var builder = new TableBuilder();
            builder.SetName(name);
            builder.AddFieldDeclaration(field0);
            builder.AddFieldDeclaration(field1);
            builder.SetPrimaryKeyDeclaration(pk);
            builder.AddForeignKeyDeclaration(fk);
            var decl = builder.Build();

            // Assert
            decl.Should().Be(
                $"CREATE TABLE IF NOT EXISTS `{name}`\n" +
                "`Muskegon` SMALLINT NOT NULL\n" +
                "`Poughkeepsie` TINYINT UNSIGNED NOT NULL\n" +
                $"{pk}\n" +
                fk.ToString()
            );
        }

        [TestMethod] public void CheckConstraints() {
            // Arrange
            var name = new TableName("ToTrapezi");
            var field0 = new FieldDecl(new FieldName("Santa Claus"), "`Santa Claus` INT UNSIGNED NOT NULL");
            var field1 = new FieldDecl(new FieldName("Independence"), "`Independence` VARCHAR(255) NOT NULL");
            var pk = new SqlSnippet($"PRIMARY KEY (`{field0.Name}`");
            var check = new BasicConstraintDecl(new SqlSnippet($"CHECK (`{field0}` <= 200000)"));

            // Act
            var builder = new TableBuilder();
            builder.SetName(name);
            builder.AddFieldDeclaration(field0);
            builder.AddFieldDeclaration(field1);
            builder.SetPrimaryKeyDeclaration(pk);
            builder.AddCheckConstraintDeclaration(check);
            var decl = builder.Build();

            // Assert
            decl.Should().Be(
                $"CREATE TABLE IF NOT EXISTS `{name}`\n" +
                "`Santa Claus` INT UNSIGNED NOT NULL\n" +
                "`Independence` VARCHAR(255) NOT NULL\n" +
                $"{pk}\n" +
                check.DDL.ToString()
            );
        }

        [TestMethod] public void FullTableDeclaration() {
            // Arrange
            var name = new TableName("IlTavolo");
            var field0 = new FieldDecl(new FieldName("Appomattox"), "`Appomattox` BIGINT UNSIGNED NOT NULL");
            var field1 = new FieldDecl(new FieldName("Chickamauga"), "`Chickamauga` VARCHAR(255) NOT NULL DEFAULT \"--none--\"");
            var field2 = new FieldDecl(new FieldName("Kitty Hawk"), "`Kitty Hawk` INT DEFAULT NULL");
            var field3 = new FieldDecl(new FieldName("Harpers Ferry"), "`Harpers Ferry` BOOLEAN NOT NULL");
            var field4 = new FieldDecl(new FieldName("La Jolla"), "`La Jolla` VARCHAR(255) NOT NULL");
            var field5 = new FieldDecl(new FieldName("Compton"), "`Compton` INT UNSIGNED NOT NULL");
            var pk = new SqlSnippet($"PRIMARY KEY (`{field0.Name}`, `{field5.Name}`)");
            var ck0 = new SqlSnippet($"UNIQUE (`{field2.Name}`)");
            var ck1 = new SqlSnippet($"UNIQUE (`{field3.Name}`, `{field0.Name}`)");
            var fk = new SqlSnippet($"FOREIGN KEY (`{field2.Name}`) REFERENCES `KaPapaʻaina` (`Amherst`)");
            var check0 = new MaxLengthConstraintDecl(field1.Name, 45);
            var check1 = new BasicConstraintDecl(new SqlSnippet($"CHECK (`{field4.Name}` != \"Salt Lake City\")"));
            var check2 = new BasicConstraintDecl(new SqlSnippet($"CHECK (`{field2.Name}` < `{field5.Name}`)"));

            // Act
            var builder = new TableBuilder();
            builder.SetName(name);
            builder.AddFieldDeclaration(field0);
            builder.AddFieldDeclaration(field1);
            builder.AddFieldDeclaration(field2);
            builder.AddFieldDeclaration(field3);
            builder.AddFieldDeclaration(field4);
            builder.AddFieldDeclaration(field5);
            builder.SetPrimaryKeyDeclaration(pk);
            builder.AddCandidateKeyDeclaration(ck0);
            builder.AddCandidateKeyDeclaration(ck1);
            builder.AddForeignKeyDeclaration(fk);
            builder.AddCheckConstraintDeclaration(check0);
            builder.AddCheckConstraintDeclaration(check1);
            builder.AddCheckConstraintDeclaration(check2);
            var decl = builder.Build();

            // Assert
            decl.Should().Be(
                $"CREATE TABLE IF NOT EXISTS `{name}`\n" +
                "`Appomattox` BIGINT UNSIGNED NOT NULL\n" +
                $"`Chickamauga` VARCHAR({check0.MaxLength}) NOT NULL DEFAULT \"--none--\"\n" +
                "`Kitty Hawk` INT DEFAULT NULL\n" +
                "`Harpers Ferry` BOOLEAN NOT NULL\n" +
                "`La Jolla` VARCHAR(255) NOT NULL\n" +
                "`Compton` INT UNSIGNED NOT NULL\n" +
                $"{pk}\n" +
                $"{ck0}\n" +
                $"{ck1}\n" +
                $"{check1.DDL}\n" +
                $"{check2.DDL}\n" +
                fk.ToString()
            );
        }

        [TestMethod] public void TableNameIsMaximumLength() {
            // Arrange
            var name = new TableName(new string('M', 64));
            var field0 = new FieldDecl(new FieldName("Battle Ground"), "`Battle Ground` SMALLINT UNSIGNED NOT NULL");
            var field1 = new FieldDecl(new FieldName("Ponce"), "`Ponce` DECIMAL NOT NULL");
            var field2 = new FieldDecl(new FieldName("Līhuʻe"), "`Līhuʻe` DATETIME");
            var pk = new SqlSnippet($"PRIMARY KEY (`{field0.Name}`)");

            // Act
            var builder = new TableBuilder();
            builder.SetName(name);
            builder.AddFieldDeclaration(field0);
            builder.AddFieldDeclaration(field1);
            builder.AddFieldDeclaration(field2);
            builder.SetPrimaryKeyDeclaration(pk);
            var decl = builder.Build();

            // Assert
            decl.Should().Be(
                $"CREATE TABLE IF NOT EXISTS `{name}`\n" +
                "`Battle Ground` SMALLINT UNSIGNED NOT NULL\n" +
                "`Ponce` DECIMAL NOT NULL\n" +
                "`Līhuʻe` DATETIME\n" +
                pk.ToString()
            );
        }

        [TestMethod] public void TableNameExceedsMaximumLength_IsError() {
            // Arrange
            var tableName = new TableName(new string('7', 1780));

            // Act
            var builder = new TableBuilder();
            var action = () => builder.SetName(tableName);

            // Assert
            action.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining("[MySQL]")
                .WithMessageContaining(tableName.ToString())
                .WithMessageContaining("exceeds the maximum of 64 characters");
        }
    }

    [TestClass, TestCategory("MySQL - Factory (DDL)")]
    public class MySqlBuilderFactoryTests {
        [TestMethod] public void UniqueConstraintBuilder() {
            // Arrange
            var factory = new BuilderFactory();

            // Act
            var builder0 = factory.NewConstraintDeclBuilder();
            var builder1 = factory.NewConstraintDeclBuilder();

            // Assert
            builder0.Should().NotBe(builder1);
        }

        [TestMethod] public void UniqueKeyBuilder() {
            // Arrange
            var factory = new BuilderFactory();

            // Act
            var builder0 = factory.NewKeyDeclBuilder();
            var builder1 = factory.NewKeyDeclBuilder();

            // Assert
            builder0.Should().NotBe(builder1);
        }

        [TestMethod] public void UniqueForeignKeyBuilder() {
            // Arrange
            var factory = new BuilderFactory();

            // Act
            var builder0 = factory.NewForeignKeyDeclBuilder();
            var builder1 = factory.NewForeignKeyDeclBuilder();

            // Assert
            builder0.Should().NotBe(builder1);
        }

        [TestMethod] public void UniqueFieldBuilder() {
            // Arrange
            var factory = new BuilderFactory();

            // Act
            var builder0 = factory.NewFieldDeclBuilder();
            var builder1 = factory.NewFieldDeclBuilder();

            // Assert
            builder0.Should().NotBe(builder1);
        }

        [TestMethod] public void UniqueTableBuilder() {
            // Arrange
            var factory = new BuilderFactory();

            // Act
            var builder0 = factory.NewTableDeclBuilder();
            var builder1 = factory.NewTableDeclBuilder();

            // Assert
            builder0.Should().NotBe(builder1);
        }
    }

    [TestClass, TestCategory("MySQL - Commands (DML)")]
    public class MySqlCommandTests {
        [TestMethod] public void CreateTable_Principal() {
            // Arrange
            var source = typeof(Mutiny);
            var translator = new Translator(NO_ENTITIES);
            var table = translator[source].Principal.Table;

            // Act
            var commands = new Commands(table, true);
            var command = commands.CreateTableCommand;

            // Assert
            command.Connection.Should().BeNull();
            command.Transaction.Should().BeNull();
            command.CommandText.Should().Be(
                $"CREATE TABLE IF NOT EXISTS `{table.Name}`\n" +
                "`Date` DATETIME NOT NULL\n" +
                "`Ship` VARCHAR(255) NOT NULL\n" +
                "`LeadMutineer` VARCHAR(255)\n" +
                "`OustedCaptain` VARCHAR(255) NOT NULL\n" +
                "`Casualties` INT UNSIGNED NOT NULL\n" +
                "PRIMARY KEY (`Date`, `Ship`);"
            );
        }

        [TestMethod] public void CreateTable_Relation() {
            // Arrange
            var source = typeof(CyrillicLetter);
            var translator = new Translator(NO_ENTITIES);
            var principalTable = translator[source].Principal.Table;
            var relationTable = translator[source].Relations[0].Table;

            // Act
            var commands = new Commands(relationTable, false);
            var command = commands.CreateTableCommand;

            // Assert
            command.Connection.Should().BeNull();
            command.Transaction.Should().BeNull();
            command.CommandText.Should().Be(
                $"CREATE TABLE IF NOT EXISTS `{relationTable.Name}`\n" +
                "`CyrillicLetter.LetterName` VARCHAR(255) NOT NULL\n" +
                "`Key` BIGINT UNSIGNED NOT NULL\n" +
                "`Value` CHAR(1) NOT NULL\n" +
                "PRIMARY KEY (`CyrillicLetter.LetterName`, `Key`)\n" +
                $"FOREIGN KEY (`CyrillicLetter.LetterName`) REFERENCES `{principalTable.Name}` (`LetterName`) ON DELETE CASCADE ON UPDATE CASCADE;"
            );
        }

        [TestMethod] public void CreateTable_PreDefinedEntity() {
            // Arrange
            var source = typeof(BackstreetBoy);
            var translator = new Translator(NO_ENTITIES);
            var table = translator[source].Principal.Table;

            // Act
            var commands = new Commands(table, true);
            var command = commands.CreateTableCommand;

            // Assert
            command.Connection.Should().BeNull();
            command.Transaction.Should().BeNull();
            command.CommandText.Should().Be(
                $"CREATE TABLE IF NOT EXISTS `{table.Name}`\n" +
                "`FirstName` VARCHAR(255) NOT NULL\n" +
                "`LastName` VARCHAR(255) NOT NULL\n" +
                "`Birthdate` DATETIME NOT NULL\n" +
                "PRIMARY KEY (`FirstName`, `LastName`);"
            );
        }

        [TestMethod] public void SelectAll_Principal() {
            // Arrange
            var source = typeof(CarDealership);
            var translator = new Translator(NO_ENTITIES);
            var table = translator[source].Principal.Table;

            // Act
            var commands = new Commands(table, true);
            var command = commands.SelectAllQuery;

            // Assert
            command.Connection.Should().BeNull();
            command.Transaction.Should().BeNull();
            command.CommandText.Should().Be($"SELECT * FROM `{table.Name}`;");
        }

        [TestMethod] public void SelectAll_Relation() {
            // Arrange
            var source = typeof(Hadith);
            var translator = new Translator(NO_ENTITIES);
            var table = translator[source].Relations[0].Table;

            // Act
            var commands = new Commands(table, false);
            var command = commands.SelectAllQuery;

            // Assert
            command.Connection.Should().BeNull();
            command.Transaction.Should().BeNull();
            command.CommandText.Should().Be($"SELECT * FROM `{table.Name}`;");
        }

        [TestMethod] public void SelectAll_WithCalculatedField() {
            // Arrange
            var source = typeof(Grenade);
            var translator = new Translator(NO_ENTITIES);
            var table = translator[source].Principal.Table;

            // Act
            var commands = new Commands(table, true);
            var command = commands.SelectAllQuery;

            // Assert
            command.Connection.Should().BeNull();
            command.Transaction.Should().BeNull();
            command.CommandText.Should().Be($"SELECT * FROM `{table.Name}`;");
        }

        [TestMethod] public void Insert_TextEnumerations() {
            // Arrange
            var source = typeof(ChipotleOrder);
            var translator = new Translator(NO_ENTITIES);
            var table = translator[source].Principal.Table;
            var rows = new List<List<DBValue>>() {
                new() {
                    DBValue.Create("CHPTL-1Tj47EqP0"),
                    DBValue.Create(ConversionOf(ChipotleOrder.MealType.Burrito)),
                    DBValue.Create(ConversionOf(ChipotleOrder.BeansOption.Pinto)),
                    DBValue.Create(ConversionOf(ChipotleOrder.ProteinOption.Chicken)),
                    DBValue.Create(ConversionOf(ChipotleOrder.SalsaOption.Hot)),
                    DBValue.Create(ConversionOf(ChipotleOrder.CheeseOption.Single)),
                    DBValue.Create(ConversionOf(ChipotleOrder.LettuceOption.None)),
                    DBValue.Create(ConversionOf(ChipotleOrder.GuacOption.OnTheSide)),
                    DBValue.Create("Rahul Shamazz"),
                    DBValue.Create('U')
                }
            };

            // Act
            var commands = new Commands(table, true);
            var command = commands.InsertCommand(rows);

            // Assert
            command.Connection.Should().BeNull();
            command.Transaction.Should().BeNull();
            command.CommandText.Should().Be(
                $"INSERT INTO `{table.Name}`\n" +
                "(`OrderId`, `Kind`, `Beans`, `Protein`, `Salsa`, `Cheese`, `Lettuce`, `Guacamole`, `NameForOrder`, `CRC`)\n" +
                "VALUES\n" +
                "(@v0, @v1, @v2, @v3, @v4, @v5, @v6, @v7, @v8, @v9);"
            );
            command.Parameters.Should().HaveCount(10).And.BeForMySql().And
                .HaveParameter("@v0", rows[0][0].Datum).And
                .HaveParameter("@v1", rows[0][1].Datum).And
                .HaveParameter("@v2", rows[0][2].Datum).And
                .HaveParameter("@v3", rows[0][3].Datum).And
                .HaveParameter("@v4", rows[0][4].Datum).And
                .HaveParameter("@v5", rows[0][5].Datum).And
                .HaveParameter("@v6", rows[0][6].Datum).And
                .HaveParameter("@v7", rows[0][7].Datum).And
                .HaveParameter("@v8", rows[0][8].Datum).And
                .HaveParameter("@v9", rows[0][9].Datum);
        }

        [TestMethod] public void Insert_IntegersBooleans() {
            // Arrange
            var source = typeof(RegularExpression);
            var translator = new Translator(NO_ENTITIES);
            var table = translator[source].Principal.Table;
            var rows = new List<List<DBValue>>() {
                new() {
                    DBValue.Create(true),
                    DBValue.Create("^([a-zA-Z0-9._%-]+)@([a-zA-Z0-9.-]+)\\.([a-zA-Z]{2,6})*$"),
                    DBValue.Create(3),
                    DBValue.Create((ushort)15783),
                    DBValue.Create(3L)
                }
            };

            // Act
            var commands = new Commands(table, true);
            var command = commands.InsertCommand(rows);

            // Assert
            command.Connection.Should().BeNull();
            command.Transaction.Should().BeNull();
            command.CommandText.Should().Be(
                $"INSERT INTO `{table.Name}`\n" +
                "(`IsPerlCompatible`, `Expression`, `NumCaptureGroups`, `MaxMatchLength`, `NumWildcards`)\n" +
                "VALUES\n" +
                "(@v0, @v1, @v2, @v3, @v4);"
            );
            command.Parameters.Should().HaveCount(5).And.BeForMySql().And
                .HaveParameter("@v0", rows[0][0].Datum).And
                .HaveParameter("@v1", rows[0][1].Datum).And
                .HaveParameter("@v2", rows[0][2].Datum).And
                .HaveParameter("@v3", rows[0][3].Datum).And
                .HaveParameter("@v4", rows[0][4].Datum);
        }

        [TestMethod] public void Insert_FloatingPoint() {
            // Arrange
            var source = typeof(Tariff);
            var translator = new Translator(NO_ENTITIES);
            var table = translator[source].Principal.Table;
            var rows = new List<List<DBValue>>() {
                new() {
                    DBValue.Create(18571924UL),
                    DBValue.Create("Brazil"),
                    DBValue.Create("Palau"),
                    DBValue.Create(18.65),
                    DBValue.Create(13.10f),
                    DBValue.Create(false),
                    DBValue.Create(40000M)
                }
            };

            // Act
            var commands = new Commands(table, true);
            var command = commands.InsertCommand(rows);

            // Assert
            command.Connection.Should().BeNull();
            command.Transaction.Should().BeNull();
            command.CommandText.Should().Be(
                $"INSERT INTO `{table.Name}`\n" +
                "(`ID`, `Importer`, `Exporter`, `RegularRate`, `DiscountRate`, `IsInForce`, `Revenue`)\n" +
                "VALUES\n" +
                "(@v0, @v1, @v2, @v3, @v4, @v5, @v6);"
            );
            command.Parameters.Should().HaveCount(7).And.BeForMySql().And
                .HaveParameter("@v0", rows[0][0].Datum).And
                .HaveParameter("@v1", rows[0][1].Datum).And
                .HaveParameter("@v2", rows[0][2].Datum).And
                .HaveParameter("@v3", rows[0][3].Datum).And
                .HaveParameter("@v4", rows[0][4].Datum).And
                .HaveParameter("@v5", rows[0][5].Datum).And
                .HaveParameter("@v6", rows[0][6].Datum);
        }

        [TestMethod] public void Insert_DateGuid() {
            // Arrange
            var source = typeof(ExecutiveOrder);
            var translator = new Translator(NO_ENTITIES);
            var table = translator[source].Principal.Table;
            var rows = new List<List<DBValue>>() {
                new() {
                    DBValue.Create("Ezra Steinenbergstein"),
                    DBValue.Create((ushort)7361),
                    DBValue.Create(new DateTime(2314, 7, 19)),
                    DBValue.Create(new DateOnly(2314, 9, 27)),
                    DBValue.Create(Guid.NewGuid()),
                    DBValue.Create(true)
                }
            };

            // Act
            var commands = new Commands(table, true);
            var command = commands.InsertCommand(rows);

            // Assert
            command.Connection.Should().BeNull();
            command.Transaction.Should().BeNull();
            command.CommandText.Should().Be(
                $"INSERT INTO `{table.Name}`\n" +
                "(`President`, `OrderNumber`, `Issued`, `Rescinded`, `DocumentID`, `EnshrinedInLegislation`)\n" +
                "VALUES\n" +
                "(@v0, @v1, @v2, @v3, @v4, @v5);"
            );
            command.Parameters.Should().HaveCount(6).And.BeForMySql().And
                .HaveParameter("@v0", rows[0][0].Datum).And
                .HaveParameter("@v1", rows[0][1].Datum).And
                .HaveParameter("@v2", rows[0][2].Datum).And
                .HaveParameter("@v3", rows[0][3].Datum).And
                .HaveParameter("@v4", rows[0][4].Datum).And
                .HaveParameter("@v5", rows[0][5].Datum);
        }

        [TestMethod] public void Insert_Null() {
            // Arrange
            var source = typeof(Codex);
            var translator = new Translator(NO_ENTITIES);
            var table = translator[source].Principal.Table;
            var rows = new List<List<DBValue>>() {
                new() {
                    DBValue.Create("Codex Mendoza"),
                    DBValue.Create("Aztec Empire"),
                    DBValue.Create(DBNull.Value),
                    DBValue.Create(3290.0)
                }
            };

            // Act
            var commands = new Commands(table, true);
            var command = commands.InsertCommand(rows);

            // Assert
            command.Connection.Should().BeNull();
            command.Transaction.Should().BeNull();
            command.CommandText.Should().Be(
                $"INSERT INTO `{table.Name}`\n" +
                "(`Title`, `Civilization`, `Published`, `SurfaceArea`)\n" +
                "VALUES\n" +
                "(@v0, @v1, @v2, @v3);"
            );
            command.Parameters.Should().HaveCount(4).And.BeForMySql().And
                .HaveParameter("@v0", rows[0][0].Datum).And
                .HaveParameter("@v1", rows[0][1].Datum).And
                .HaveParameter("@v2", rows[0][2].Datum).And
                .HaveParameter("@v3", rows[0][3].Datum);
        }

        [TestMethod] public void Insert_TwoRows() {
            // Arrange
            var source = typeof(Choreographer);
            var translator = new Translator(NO_ENTITIES);
            var table = translator[source].Principal.Table;
            var rows = new List<List<DBValue>>() {
                new() {
                    DBValue.Create("Kenny Ortega"),
                    DBValue.Create("Pop"),
                    DBValue.Create(14),
                    DBValue.Create(new DateTime(2019, 7, 24))
                },
                new() {
                    DBValue.Create("Bob Fosse"),
                    DBValue.Create("Jazz"),
                    DBValue.Create(7),
                    DBValue.Create(DBNull.Value)
                }
            };

            // Act
            var commands = new Commands(table, true);
            var command = commands.InsertCommand(rows);

            // Assert
            command.Connection.Should().BeNull();
            command.Transaction.Should().BeNull();
            command.CommandText.Should().Be(
                $"INSERT INTO `{table.Name}`\n" +
                "(`Name`, `Specialty`, `FilmsChoreographed`, `WalkOfFameStar`)\n" +
                "VALUES\n" +
                "(@v0, @v1, @v2, @v3),\n" +
                "(@v4, @v5, @v6, @v7);"
            );
            command.Parameters.Should().HaveCount(8).And.BeForMySql().And
                .HaveParameter("@v0", rows[0][0].Datum).And
                .HaveParameter("@v1", rows[0][1].Datum).And
                .HaveParameter("@v2", rows[0][2].Datum).And
                .HaveParameter("@v3", rows[0][3].Datum).And
                .HaveParameter("@v4", rows[1][0].Datum).And
                .HaveParameter("@v5", rows[1][1].Datum).And
                .HaveParameter("@v6", rows[1][2].Datum).And
                .HaveParameter("@v7", rows[1][3].Datum);
        }

        [TestMethod] public void Insert_ThreePlusRows() {
            // Arrange
            var source = typeof(Nachos);
            var translator = new Translator(NO_ENTITIES);
            var table = translator[source].Principal.Table;
            var rows = new List<List<DBValue>>() {
                new() {
                    DBValue.Create(Guid.NewGuid()),
                    DBValue.Create(ConversionOf(Nachos.KindOfChip.BlueCorn)),
                    DBValue.Create(579.44),
                    DBValue.Create((sbyte)3),
                    DBValue.Create(true),
                    DBValue.Create(true),
                    DBValue.Create(true),
                    DBValue.Create(ConversionOf(Nachos.Role.Appetizer)),
                    DBValue.Create("Sandra Uguetta")
                },
                new() {
                    DBValue.Create(Guid.NewGuid()),
                    DBValue.Create(ConversionOf(Nachos.KindOfChip.Tostitos)),
                    DBValue.Create(1439.986),
                    DBValue.Create((sbyte)8),
                    DBValue.Create(true),
                    DBValue.Create(false),
                    DBValue.Create(true),
                    DBValue.Create(ConversionOf(Nachos.Role.Lunch)),
                    DBValue.Create("Bonnie St. Prierrelle")
                },
                new() {
                    DBValue.Create(Guid.NewGuid()),
                    DBValue.Create(ConversionOf(Nachos.KindOfChip.Corn)),
                    DBValue.Create(1874.41),
                    DBValue.Create((sbyte)0),
                    DBValue.Create(false),
                    DBValue.Create(false),
                    DBValue.Create(false),
                    DBValue.Create(ConversionOf(Nachos.Role.Dessert)),
                    DBValue.Create("Edna Kuqul")
                }
            };

            // Act
            var commands = new Commands(table, true);
            var command = commands.InsertCommand(rows);

            // Assert
            command.Connection.Should().BeNull();
            command.Transaction.Should().BeNull();
            command.CommandText.Should().Be(
                $"INSERT INTO `{table.Name}`\n" +
                "(`NachosID`, `Chips`, `Calories`, `TypesOfCheese`, `Beans`, `Salsa`, `Guacamole`, `FoodRole`, `Chef`)\n" +
                "VALUES\n" +
                "(@v0, @v1, @v2, @v3, @v4, @v5, @v6, @v7, @v8),\n" +
                "(@v9, @v10, @v11, @v12, @v13, @v14, @v15, @v16, @v17),\n" +
                "(@v18, @v19, @v20, @v21, @v22, @v23, @v24, @v25, @v26);"
            );
            command.Parameters.Should().HaveCount(27).And.BeForMySql().And
                .HaveParameter("@v0", rows[0][0].Datum).And
                .HaveParameter("@v1", rows[0][1].Datum).And
                .HaveParameter("@v2", rows[0][2].Datum).And
                .HaveParameter("@v3", rows[0][3].Datum).And
                .HaveParameter("@v4", rows[0][4].Datum).And
                .HaveParameter("@v5", rows[0][5].Datum).And
                .HaveParameter("@v6", rows[0][6].Datum).And
                .HaveParameter("@v7", rows[0][7].Datum).And
                .HaveParameter("@v8", rows[0][8].Datum).And
                .HaveParameter("@v9", rows[1][0].Datum).And
                .HaveParameter("@v10", rows[1][1].Datum).And
                .HaveParameter("@v11", rows[1][2].Datum).And
                .HaveParameter("@v12", rows[1][3].Datum).And
                .HaveParameter("@v13", rows[1][4].Datum).And
                .HaveParameter("@v14", rows[1][5].Datum).And
                .HaveParameter("@v15", rows[1][6].Datum).And
                .HaveParameter("@v16", rows[1][7].Datum).And
                .HaveParameter("@v17", rows[1][8].Datum).And
                .HaveParameter("@v18", rows[2][0].Datum).And
                .HaveParameter("@v19", rows[2][1].Datum).And
                .HaveParameter("@v20", rows[2][2].Datum).And
                .HaveParameter("@v21", rows[2][3].Datum).And
                .HaveParameter("@v22", rows[2][4].Datum).And
                .HaveParameter("@v23", rows[2][5].Datum).And
                .HaveParameter("@v24", rows[2][6].Datum).And
                .HaveParameter("@v25", rows[2][7].Datum).And
                .HaveParameter("@v26", rows[2][8].Datum);
        }

        [TestMethod] public void Insert_Relation() {
            // Arrange
            var source = typeof(Manicure);
            var translator = new Translator(NO_ENTITIES);
            var table = translator[source].Relations[0].Table;
            var rows = new List<List<DBValue>>() {
                new() {
                    DBValue.Create("Louisa d'Alesssao"),
                    DBValue.Create("Oren Swayziei"),
                    DBValue.Create(DateTime.Now),
                    DBValue.Create(ConversionOf(Manicure.Hand.Left)),
                    DBValue.Create(ConversionOf(Manicure.Position.Middle)),
                    DBValue.Create(true)
                }
            };

            // Act
            var commands = new Commands(table, false);
            var command = commands.InsertCommand(rows);

            // Assert
            command.Connection.Should().BeNull();
            command.Transaction.Should().BeNull();
            command.CommandText.Should().Be(
                $"INSERT INTO `{table.Name}`\n" +
                "(`Manicure.Manicurist`, `Manicure.Manicuree`, `Manicure.Timestamp`, `Key.Hand`, `Key.Position`, `Value`)\n" +
                "VALUES\n" +
                "(@v0, @v1, @v2, @v3, @v4, @v5);"
            );
            command.Parameters.Should().HaveCount(6).And.BeForMySql().And
                .HaveParameter("@v0", rows[0][0].Datum).And
                .HaveParameter("@v1", rows[0][1].Datum).And
                .HaveParameter("@v2", rows[0][2].Datum).And
                .HaveParameter("@v3", rows[0][3].Datum).And
                .HaveParameter("@v4", rows[0][4].Datum).And
                .HaveParameter("@v5", rows[0][5].Datum);
        }

        [TestMethod] public void Update_SingleFieldPrimaryKey() {
            // Arrange
            var source = typeof(SignLanguage);
            var translator = new Translator(NO_ENTITIES);
            var table = translator[source].Principal.Table;
            var rows = new List<List<DBValue>>() {
                new() {
                    DBValue.Create("ase"),
                    DBValue.Create("American Sign Language"),
                    DBValue.Create(730000UL),
                    DBValue.Create(130000UL),
                    DBValue.Create(true)
                }
            };

            // Act
            var commands = new Commands(table, true);
            var command = commands.UpdateCommand(rows);

            // Assert
            command.Connection.Should().BeNull();
            command.Transaction.Should().BeNull();
            command.CommandText.Should().Be(
                $"UPDATE `{table.Name}`\n" +
                "SET `Name` = @v1, `NativeSigners` = @v2, `L2Signers` = @v3, `SingleHandFingerspelling` = @v4\n" +
                "WHERE (`ISO6393` = @v0);"
            );
            command.Parameters.Should().HaveCount(5).And.BeForMySql().And
                .HaveParameter("@v0", rows[0][0].Datum).And
                .HaveParameter("@v1", rows[0][1].Datum).And
                .HaveParameter("@v2", rows[0][2].Datum).And
                .HaveParameter("@v3", rows[0][3].Datum).And
                .HaveParameter("@v4", rows[0][4].Datum);
        }

        [TestMethod] public void Update_MultiFieldPrimaryKey() {
            // Arrange
            var source = typeof(Diaspora);
            var translator = new Translator(NO_ENTITIES);
            var table = translator[source].Principal.Table;
            var rows = new List<List<DBValue>>() {
                new() {
                    DBValue.Create("Jewish"),
                    DBValue.Create("Israel"),
                    DBValue.Create(8500000UL),
                    DBValue.Create("Yordim"),
                    DBValue.Create(false)
                }
            };

            // Act
            var commands = new Commands(table, true);
            var command = commands.UpdateCommand(rows);

            // Assert
            command.Connection.Should().BeNull();
            command.Transaction.Should().BeNull();
            command.CommandText.Should().Be(
                $"UPDATE `{table.Name}`\n" +
                "SET `Population` = @v2, `DiasporicTerm` = @v3, `PrimarilyRefugees` = @v4\n" +
                "WHERE (`Ethnicity` = @v0 AND `ExogenousCountry` = @v1);"
            );
            command.Parameters.Should().HaveCount(5).And.BeForMySql().And
                .HaveParameter("@v0", rows[0][0].Datum).And
                .HaveParameter("@v1", rows[0][1].Datum).And
                .HaveParameter("@v2", rows[0][2].Datum).And
                .HaveParameter("@v3", rows[0][3].Datum).And
                .HaveParameter("@v4", rows[0][4].Datum);
        }

        [TestMethod] public void Update_AllFieldsPrimaryKey() {
            // Arrange
            var source = typeof(Click);
            var translator = new Translator(NO_ENTITIES);
            var table = translator[source].Principal.Table;
            var rows = new List<List<DBValue>>() {
                new() {
                    DBValue.Create("Bilabial Clik"),
                    DBValue.Create('ʘ'),
                    DBValue.Create("mwah")
                }
            };

            // Act
            var commands = new Commands(table, true);
            var command = commands.UpdateCommand(rows);

            // Assert
            command.Connection.Should().BeNull();
            command.Transaction.Should().BeNull();
            command.CommandText.Should().Be("");
            command.Parameters.Should().HaveCount(0).And.BeForMySql();
        }

        [TestMethod] public void Update_TwoRows() {
            // Arrange
            var source = typeof(CutthroatKitchenSabotage);
            var translator = new Translator(NO_ENTITIES);
            var table = translator[source].Principal.Table;
            var rows = new List<List<DBValue>>() {
                new() {
                    DBValue.Create((sbyte)1),
                    DBValue.Create((sbyte)1),
                    DBValue.Create(ConversionOf(CutthroatKitchenSabotage.Round.Round1)),
                    DBValue.Create((sbyte)5),
                    DBValue.Create("replace opponent's cheese with Kraft powder"),
                    DBValue.Create(4000M)
                },
                new() {
                    DBValue.Create((sbyte)7),
                    DBValue.Create((sbyte)13),
                    DBValue.Create(ConversionOf(CutthroatKitchenSabotage.Round.Round3)),
                    DBValue.Create((sbyte)9),
                    DBValue.Create("force opponent to use a Swiss army knife for their only utensils"),
                    DBValue.Create(6300M)
                }
            };

            // Act
            var commands = new Commands(table, true);
            var command = commands.UpdateCommand(rows);

            // Assert
            command.Connection.Should().BeNull();
            command.Transaction.Should().BeNull();
            command.CommandText.Should().Be(
                $"UPDATE `{table.Name}`\n" +
                "SET `Sabotage` = @v4, `WinningBid` = @v5\n" +
                "WHERE (`Season` = @v0 AND `Episode` = @v1 AND `WhichRound` = @v2 AND `SabotageNumber` = @v3);\n" +
                "\n" +
                $"UPDATE `{table.Name}`\n" +
                "SET `Sabotage` = @v10, `WinningBid` = @v11\n" +
                "WHERE (`Season` = @v6 AND `Episode` = @v7 AND `WhichRound` = @v8 AND `SabotageNumber` = @v9);"
            );
            command.Parameters.Should().HaveCount(12).And.BeForMySql().And
                .HaveParameter("@v0", rows[0][0].Datum).And
                .HaveParameter("@v1", rows[0][1].Datum).And
                .HaveParameter("@v2", rows[0][2].Datum).And
                .HaveParameter("@v3", rows[0][3].Datum).And
                .HaveParameter("@v4", rows[0][4].Datum).And
                .HaveParameter("@v5", rows[0][5].Datum).And
                .HaveParameter("@v6", rows[1][0].Datum).And
                .HaveParameter("@v7", rows[1][1].Datum).And
                .HaveParameter("@v8", rows[1][2].Datum).And
                .HaveParameter("@v9", rows[1][3].Datum).And
                .HaveParameter("@v10", rows[1][4].Datum).And
                .HaveParameter("@v11", rows[1][5].Datum);
        }

        [TestMethod] public void Update_ThreePlusRows() {
            // Arrange
            var source = typeof(WelshGod);
            var translator = new Translator(NO_ENTITIES);
            var table = translator[source].Principal.Table;
            var rows = new List<List<DBValue>>() {
                new() {
                    DBValue.Create("Arianrhod"),
                    DBValue.Create(Guid.NewGuid()),
                    DBValue.Create("Sky and Stars"),
                    DBValue.Create(18UL)
                },
                new() {
                    DBValue.Create("Llŷr Llediaith"),
                    DBValue.Create(Guid.NewGuid()),
                    DBValue.Create("Sea"),
                    DBValue.Create(17UL)
                },
                new() {
                    DBValue.Create("Gwydion"),
                    DBValue.Create(Guid.NewGuid()),
                    DBValue.Create("Trickster"),
                    DBValue.Create(51UL)
                },
                new() {
                    DBValue.Create("Taran"),
                    DBValue.Create(Guid.NewGuid()),
                    DBValue.Create("Storm"),
                    DBValue.Create(2UL)
                }
            };

            // Act
            var commands = new Commands(table, true);
            var command = commands.UpdateCommand(rows);

            // Assert
            command.Connection.Should().BeNull();
            command.Transaction.Should().BeNull();
            command.CommandText.Should().Be(
                $"UPDATE `{table.Name}`\n" +
                "SET `Name` = @v0, `Domain` = @v2, `MabinogionMentions` = @v3\n" +
                "WHERE (`DeityID` = @v1);\n" +
                "\n" +
                $"UPDATE `{table.Name}`\n" +
                "SET `Name` = @v4, `Domain` = @v6, `MabinogionMentions` = @v7\n" +
                "WHERE (`DeityID` = @v5);\n" +
                "\n" +
                $"UPDATE `{table.Name}`\n" +
                "SET `Name` = @v8, `Domain` = @v10, `MabinogionMentions` = @v11\n" +
                "WHERE (`DeityID` = @v9);\n" +
                "\n" +
                $"UPDATE `{table.Name}`\n" +
                "SET `Name` = @v12, `Domain` = @v14, `MabinogionMentions` = @v15\n" +
                "WHERE (`DeityID` = @v13);"
            );
            command.Parameters.Should().HaveCount(16).And.BeForMySql().And
                .HaveParameter("@v0", rows[0][0].Datum).And
                .HaveParameter("@v1", rows[0][1].Datum).And
                .HaveParameter("@v2", rows[0][2].Datum).And
                .HaveParameter("@v3", rows[0][3].Datum).And
                .HaveParameter("@v4", rows[1][0].Datum).And
                .HaveParameter("@v5", rows[1][1].Datum).And
                .HaveParameter("@v6", rows[1][2].Datum).And
                .HaveParameter("@v7", rows[1][3].Datum).And
                .HaveParameter("@v8", rows[2][0].Datum).And
                .HaveParameter("@v9", rows[2][1].Datum).And
                .HaveParameter("@v10", rows[2][2].Datum).And
                .HaveParameter("@v11", rows[2][3].Datum).And
                .HaveParameter("@v12", rows[3][0].Datum).And
                .HaveParameter("@v13", rows[3][1].Datum).And
                .HaveParameter("@v14", rows[3][2].Datum).And
                .HaveParameter("@v15", rows[3][3].Datum);
        }

        [TestMethod] public void Update_NullValue() {
            // Arrange
            var source = typeof(Curry);
            var translator = new Translator(NO_ENTITIES);
            var table = translator[source].Principal.Table;
            var rows = new List<List<DBValue>>() {
                new() {
                    DBValue.Create(Guid.NewGuid()),
                    DBValue.Create(DBNull.Value),
                    DBValue.Create(ConversionOf(Curry.Origin.Indian)),
                    DBValue.Create(DBNull.Value),
                    DBValue.Create(false),
                    DBValue.Create(byte.MaxValue)
                }
            };

            // Act
            var commands = new Commands(table, true);
            var command = commands.UpdateCommand(rows);

            // Assert
            command.Connection.Should().BeNull();
            command.Transaction.Should().BeNull();
            command.CommandText.Should().Be(
                $"UPDATE `{table.Name}`\n" +
                "SET `CurryColor` = @v1, `CountryOfOrigin` = @v2, `Protein` = @v3, `IsVegetarian` = @v4, `SpiceLevel` = @v5\n" +
                "WHERE (`CurryID` = @v0);"
            );
            command.Parameters.Should().HaveCount(6).And.BeForMySql().And
                .HaveParameter("@v0", rows[0][0].Datum).And
                .HaveParameter("@v1", rows[0][1].Datum).And
                .HaveParameter("@v2", rows[0][2].Datum).And
                .HaveParameter("@v3", rows[0][3].Datum).And
                .HaveParameter("@v4", rows[0][4].Datum).And
                .HaveParameter("@v5", rows[0][5].Datum);
        }

        [TestMethod] public void Update_NonAssociativeRelation() {
            // Arrange
            var source = typeof(Overture);
            var translator = new Translator(NO_ENTITIES);
            var table = translator[source].Relations[0].Table;
            var rows = new List<List<DBValue>>() {
                new() {
                    DBValue.Create("Pytor Ilyich Tchaikovsky"),
                    DBValue.Create(49),
                    DBValue.Create("Cor Anglais"),
                    DBValue.Create((sbyte)1)
                }
            };

            // Act
            var commands = new Commands(table, false);
            var command = commands.UpdateCommand(rows);

            // Assert
            command.Connection.Should().BeNull();
            command.Transaction.Should().BeNull();
            command.CommandText.Should().Be("");
            command.Parameters.Should().HaveCount(0).And.BeForMySql();
        }

        [TestMethod] public void Update_SingleKeyAssociativeRelation() {
            // Arrange
            var source = typeof(Supermodel);
            var translator = new Translator(NO_ENTITIES);
            var table = translator[source].Relations[0].Table;
            var rows = new List<List<DBValue>>() {
                new() {
                    DBValue.Create("Gisele Bündchen"),
                    DBValue.Create(1),
                    DBValue.Create("Model Management")
                }
            };

            // Act
            var commands = new Commands(table, false);
            var command = commands.UpdateCommand(rows);

            // Assert
            command.Connection.Should().BeNull();
            command.Transaction.Should().BeNull();
            command.CommandText.Should().Be(
                $"UPDATE `{table.Name}`\n" +
                "SET `Item` = @v2\n" +
                "WHERE (`Supermodel.Name` = @v0 AND `Index` = @v1);"
            );
            command.Parameters.Should().HaveCount(3).And.BeForMySql().And
                .HaveParameter("@v0", rows[0][0].Datum).And
                .HaveParameter("@v1", rows[0][1].Datum).And
                .HaveParameter("@v2", rows[0][2].Datum);
        }

        [TestMethod] public void Update_MultiKeyAssociativeRelation() {
            // Arrange
            var source = typeof(StockIndex);
            var translator = new Translator(NO_ENTITIES);
            var table = translator[source].Relations[0].Table;
            var rows = new List<List<DBValue>>() {
                new() {
                    DBValue.Create("S&P 500"),
                    DBValue.Create(new DateTime(2021, 3, 22)),
                    DBValue.Create("CZR"),
                    DBValue.Create(0.01f)
                }
            };

            // Act
            var commands = new Commands(table, false);
            var command = commands.UpdateCommand(rows);

            // Assert
            command.Connection.Should().BeNull();
            command.Transaction.Should().BeNull();
            command.CommandText.Should().Be(
                $"UPDATE `{table.Name}`\n" +
                "SET `Value` = @v3\n" +
                "WHERE (`StockIndex.IndexName` = @v0 AND `Key.Listed` = @v1 AND `Key.Symbol` = @v2);"
            );
            command.Parameters.Should().HaveCount(4).And.BeForMySql().And
                .HaveParameter("@v0", rows[0][0].Datum).And
                .HaveParameter("@v1", rows[0][1].Datum).And
                .HaveParameter("@v2", rows[0][2].Datum).And
                .HaveParameter("@v3", rows[0][3].Datum);
        }

        [TestMethod] public void Delete_SingleFieldPrimaryKey() {
            // Arrange
            var source = typeof(Mansa);
            var translator = new Translator(NO_ENTITIES);
            var table = translator[source].Principal.Table;
            var rows = new List<List<DBValue>>() {
                new() {
                    DBValue.Create(9U)
                }
            };

            // Act
            var commands = new Commands(table, true);
            var command = commands.DeleteCommand(rows);

            // Assert
            command.Connection.Should().BeNull();
            command.Transaction.Should().BeNull();
            command.CommandText.Should().Be(
                $"DELETE FROM `{table.Name}`\n" +
                "WHERE (`Index` = @v0);"
            );
            command.Parameters.Should().HaveCount(1).And.BeForMySql().And
                .HaveParameter("@v0", rows[0][0].Datum);
        }

        [TestMethod] public void Delete_MultiFieldPrimaryKey() {
            // Arrange
            var source = typeof(Hamentaschen);
            var translator = new Translator(NO_ENTITIES);
            var table = translator[source].Principal.Table;
            var rows = new List<List<DBValue>>() {
                new() {
                    DBValue.Create((ushort)2025),
                    DBValue.Create(7U),
                    DBValue.Create((ushort)17),
                    DBValue.Create(Guid.NewGuid())
                }
            };

            // Act
            var commands = new Commands(table, true);
            var command = commands.DeleteCommand(rows);

            // Assert
            command.Connection.Should().BeNull();
            command.Transaction.Should().BeNull();
            command.CommandText.Should().Be(
                $"DELETE FROM `{table.Name}`\n" +
                "WHERE (`Year` = @v0 AND `BatchNumber` = @v1 AND `CookieNumber` = @v2 AND `CookieMakerID` = @v3);"
            );
            command.Parameters.Should().HaveCount(4).And.BeForMySql().And
                .HaveParameter("@v0", rows[0][0].Datum).And
                .HaveParameter("@v1", rows[0][1].Datum).And
                .HaveParameter("@v2", rows[0][2].Datum).And
                .HaveParameter("@v3", rows[0][3].Datum);
        }

        [TestMethod] public void Delete_AllFieldsPrimaryKey() {
            // Arrange
            var source = typeof(Tirthankara);
            var translator = new Translator(NO_ENTITIES);
            var table = translator[source].Principal.Table;
            var rows = new List<List<DBValue>>() {
                new() {
                    DBValue.Create("Mahavira"),
                    DBValue.Create("Lion"),
                    DBValue.Create(24U)
                }
            };

            // Act
            var commands = new Commands(table, true);
            var command = commands.DeleteCommand(rows);

            // Assert
            command.Connection.Should().BeNull();
            command.Transaction.Should().BeNull();
            command.CommandText.Should().Be(
                $"DELETE FROM `{table.Name}`\n" +
                "WHERE (`Name` = @v0 AND `Emblem` = @v1 AND `Iteration` = @v2);"
            );
            command.Parameters.Should().HaveCount(3).And.BeForMySql().And
                .HaveParameter("@v0", rows[0][0].Datum).And
                .HaveParameter("@v1", rows[0][1].Datum).And
                .HaveParameter("@v2", rows[0][2].Datum);
        }

        [TestMethod] public void Delete_TwoRows() {
            // Arrange
            var source = typeof(FoodTruck);
            var translator = new Translator(NO_ENTITIES);
            var table = translator[source].Principal.Table;
            var rows = new List<List<DBValue>>() {
                new() {
                    DBValue.Create("Lobster 'n' Stuff"),
                    DBValue.Create("NicoAngelo Buenarottazzini")
                },
                new() {
                    DBValue.Create("Der Schnitzel"),
                    DBValue.Create("Hans Neuen")
                }
            };

            // Act
            var commands = new Commands(table, true);
            var command = commands.DeleteCommand(rows);

            // Assert
            command.Connection.Should().BeNull();
            command.Transaction.Should().BeNull();
            command.CommandText.Should().Be(
                $"DELETE FROM `{table.Name}`\n" +
                "WHERE (`TruckName` = @v0 AND `Proprietor` = @v1) OR\n" +
                "(`TruckName` = @v2 AND `Proprietor` = @v3);"
            );
            command.Parameters.Should().HaveCount(4).And.BeForMySql().And
                .HaveParameter("@v0", rows[0][0].Datum).And
                .HaveParameter("@v1", rows[0][1].Datum).And
                .HaveParameter("@v2", rows[1][0].Datum).And
                .HaveParameter("@v3", rows[1][1].Datum);
        }

        [TestMethod] public void Delete_ThreePlusRows() {
            // Arrange
            var source = typeof(Iyalet);
            var translator = new Translator(NO_ENTITIES);
            var table = translator[source].Principal.Table;
            var rows = new List<List<DBValue>>() {
                new() {
                    DBValue.Create("Iudex")
                },
                new() {
                    DBValue.Create("Apothetikals")
                },
                new() {
                    DBValue.Create("Engineers")
                },
                new() {
                    DBValue.Create("Legion")
                }
            };

            // Act
            var commands = new Commands(table, true);
            var command = commands.DeleteCommand(rows);

            // Assert
            command.Connection.Should().BeNull();
            command.Transaction.Should().BeNull();
            command.CommandText.Should().Be(
                $"DELETE FROM `{table.Name}`\n" +
                "WHERE (`Name` = @v0) OR\n" +
                "(`Name` = @v1) OR\n" +
                "(`Name` = @v2) OR\n" +
                "(`Name` = @v3);"
            );
            command.Parameters.Should().HaveCount(4).And.BeForMySql().And
                .HaveParameter("@v0", rows[0][0].Datum).And
                .HaveParameter("@v1", rows[1][0].Datum).And
                .HaveParameter("@v2", rows[2][0].Datum).And
                .HaveParameter("@v3", rows[3][0].Datum);
        }

        [TestMethod] public void Delete_NonAssociativeRelation() {
            // Arrange
            var source = typeof(BuzzerSystem);
            var translator = new Translator(NO_ENTITIES);
            var table = translator[source].Relations[0].Table;
            var rows = new List<List<DBValue>>() {
                new() {
                    DBValue.Create(Guid.NewGuid()),
                    DBValue.Create((ushort)2024)
                }
            };

            // Act
            var commands = new Commands(table, false);
            var command = commands.DeleteCommand(rows);

            // Assert
            command.Connection.Should().BeNull();
            command.Transaction.Should().BeNull();
            command.CommandText.Should().Be(
                $"DELETE FROM `{table.Name}`\n" +
                "WHERE (`BuzzerSystem.BuzzerID` = @v0 AND `Item` = @v1);"
            );
            command.Parameters.Should().HaveCount(2).And.BeForMySql().And
                .HaveParameter("@v0", rows[0][0].Datum).And
                .HaveParameter("@v1", rows[0][1].Datum);
        }

        [TestMethod] public void Delete_SingleKeyAssociativeRelation() {
            // Arrange
            var source = typeof(LineDance);
            var translator = new Translator(NO_ENTITIES);
            var table = translator[source].Relations[0].Table;
            var rows = new List<List<DBValue>>() {
                new() {
                    DBValue.Create("The One That Got Away"),
                    DBValue.Create("(by Katy Perry)"),
                    DBValue.Create(18U)
                }
            };

            // Act
            var commands = new Commands(table, false);
            var command = commands.DeleteCommand(rows);

            // Assert
            command.Connection.Should().BeNull();
            command.Transaction.Should().BeNull();
            command.CommandText.Should().Be(
                $"DELETE FROM `{table.Name}`\n" +
                "WHERE (`LineDance.Song` = @v0 AND `LineDance.Discriminator` = @v1 AND `Index` = @v2);"
            );
            command.Parameters.Should().HaveCount(3).And.BeForMySql().And
                .HaveParameter("@v0", rows[0][0].Datum).And
                .HaveParameter("@v1", rows[0][1].Datum).And
                .HaveParameter("@v2", rows[0][2].Datum);
        }

        [TestMethod] public void Delete_MultiKeyAssociativeRelation() {
            // Arrange
            var source = typeof(CochlearImplant);
            var translator = new Translator(NO_ENTITIES);
            var table = translator[source].Relations[0].Table;
            var rows = new List<List<DBValue>>() {
                new() {
                    DBValue.Create("Samuel Kolerr"),
                    DBValue.Create(true),
                    DBValue.Create(3U),
                    DBValue.Create("Barzensson's Exam"),
                    DBValue.Create(2.0f)
                }
            };

            // Act
            var commands = new Commands(table, false);
            var command = commands.DeleteCommand(rows);

            // Assert
            command.Connection.Should().BeNull();
            command.Transaction.Should().BeNull();
            command.CommandText.Should().Be(
                $"DELETE FROM `{table.Name}`\n" +
                "WHERE (`CochlearImplant.Individual` = @v0 AND `CochlearImplant.LeftSide` = @v1 AND `CochlearImplant.Iteration` = @v2 AND `Key.Title` = @v3 AND `Key.Version` = @v4);"
            );
            command.Parameters.Should().HaveCount(5).And.BeForMySql().And
                .HaveParameter("@v0", rows[0][0].Datum).And
                .HaveParameter("@v1", rows[0][1].Datum).And
                .HaveParameter("@v2", rows[0][2].Datum).And
                .HaveParameter("@v3", rows[0][3].Datum).And
                .HaveParameter("@v4", rows[0][4].Datum);
        }

        [TestMethod] public void Delete_OwningEntity_NonAssociativeRelation() {
            // Arrange
            var source = typeof(CongaLine);
            var translator = new Translator(NO_ENTITIES);
            var table = translator[source].Relations[0].Table;
            var rows = new List<List<DBValue>>() {
                new() {
                    DBValue.Create(71804419)
                }
            };

            // Act
            var commands = new Commands(table, false);
            var command = commands.DeleteCommand(rows);

            // Assert
            command.Connection.Should().BeNull();
            command.Transaction.Should().BeNull();
            command.CommandText.Should().Be(
                $"DELETE FROM `{table.Name}`\n" +
                "WHERE (`CongaLine.DanceID` = @v0);"
            );
            command.Parameters.Should().HaveCount(1).And.BeForMySql().And
                .HaveParameter("@v0", rows[0][0].Datum);
        }

        [TestMethod] public void Delete_OwningEntity_SingleKeyAssociativeRelation() {
            // Arrange
            var source = typeof(Rodeo);
            var translator = new Translator(NO_ENTITIES);
            var table = translator[source].Relations[0].Table;
            var rows = new List<List<DBValue>>() {
                new() {
                    DBValue.Create(Guid.NewGuid())
                },
                new() {
                    DBValue.Create(Guid.NewGuid())
                }
            };

            // Act
            var commands = new Commands(table, false);
            var command = commands.DeleteCommand(rows);

            // Assert
            command.Connection.Should().BeNull();
            command.Transaction.Should().BeNull();
            command.CommandText.Should().Be(
                $"DELETE FROM `{table.Name}`\n" +
                "WHERE (`Rodeo.RodeoID` = @v0) OR\n" +
                "(`Rodeo.RodeoID` = @v1);"
            );
            command.Parameters.Should().HaveCount(2).And.BeForMySql().And
                .HaveParameter("@v0", rows[0][0].Datum).And
                .HaveParameter("@v1", rows[1][0].Datum);
        }

        [TestMethod] public void Delete_OwningEntity_MultiKeyAssociativeRelation() {
            // Arrange
            var source = typeof(Surrogate);
            var translator = new Translator(NO_ENTITIES);
            var table = translator[source].Relations[0].Table;
            var rows = new List<List<DBValue>>() {
                new() {
                    DBValue.Create(Guid.NewGuid()),
                    DBValue.Create("Linda Elenhenney")
                },
                new() {
                    DBValue.Create(Guid.NewGuid()),
                    DBValue.Create("Sasha Spozane")
                },
                new() {
                    DBValue.Create(Guid.NewGuid()),
                    DBValue.Create("Helena Bar-Esther")
                }
            };

            // Act
            var commands = new Commands(table, false);
            var command = commands.DeleteCommand(rows);

            // Assert
            command.Connection.Should().BeNull();
            command.Transaction.Should().BeNull();
            command.CommandText.Should().Be(
                $"DELETE FROM `{table.Name}`\n" +
                "WHERE (`Surrogate.PersonID` = @v0 AND `Surrogate.Agency` = @v1) OR\n" +
                "(`Surrogate.PersonID` = @v2 AND `Surrogate.Agency` = @v3) OR\n" +
                "(`Surrogate.PersonID` = @v4 AND `Surrogate.Agency` = @v5);"
            );
            command.Parameters.Should().HaveCount(6).And.BeForMySql().And
                .HaveParameter("@v0", rows[0][0].Datum).And
                .HaveParameter("@v1", rows[0][1].Datum).And
                .HaveParameter("@v2", rows[1][0].Datum).And
                .HaveParameter("@v3", rows[1][1].Datum).And
                .HaveParameter("@v4", rows[2][0].Datum).And
                .HaveParameter("@v5", rows[2][1].Datum);
        }

        [TestMethod] public void Delete_MixedStyleFromRelation() {
            // Arrange
            var source = typeof(AbortionClinic);
            var translator = new Translator(NO_ENTITIES);
            var table = translator[source].Relations[0].Table;
            var rows = new List<List<DBValue>>() {
                new() {
                    DBValue.Create(Guid.NewGuid())
                },
                new() {
                    DBValue.Create(Guid.NewGuid()),
                    DBValue.Create("Dr. Maxwell Correlli")
                },
                new() {
                    DBValue.Create(Guid.NewGuid()),
                    DBValue.Create("Dr. Amanda S. Ovwell")
                },
                new() {
                    DBValue.Create(Guid.NewGuid())
                },
                new() {
                    DBValue.Create(Guid.NewGuid())
                }
            };

            // Act
            var commands = new Commands(table, false);
            var command = commands.DeleteCommand(rows);

            // Assert
            command.Connection.Should().BeNull();
            command.Transaction.Should().BeNull();
            command.CommandText.Should().Be(
                $"DELETE FROM `{table.Name}`\n" +
                "WHERE (`AbortionClinic.ID` = @v0) OR\n" +
                "(`AbortionClinic.ID` = @v1 AND `Item` = @v2) OR\n" +
                "(`AbortionClinic.ID` = @v3 AND `Item` = @v4) OR\n" +
                "(`AbortionClinic.ID` = @v5) OR\n" +
                "(`AbortionClinic.ID` = @v6);"
            );
            command.Parameters.Should().HaveCount(7).And.BeForMySql().And
                .HaveParameter("@v0", rows[0][0].Datum).And
                .HaveParameter("@v1", rows[1][0].Datum).And
                .HaveParameter("@v2", rows[1][1].Datum).And
                .HaveParameter("@v3", rows[2][0].Datum).And
                .HaveParameter("@v4", rows[2][1].Datum).And
                .HaveParameter("@v5", rows[3][0].Datum).And
                .HaveParameter("@v6", rows[4][0].Datum);
        }


        private static string ConversionOf<T>(T enumerator) where T : Enum {
            var converter = new EnumToStringConverter(typeof(T)).ConverterImpl;
            return (string)converter.Convert(enumerator)!;
        }
        private static Func<Type, IEnumerable<object>> NO_ENTITIES => _ => Array.Empty<object>();
    }    
}
