using FluentAssertions;
using Kvasir.Exceptions;
using Kvasir.Providers.MySQL;
using Kvasir.Schema;
using Kvasir.Transcription;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using static System.Net.Mime.MediaTypeNames;
using static UT.Kvasir.Translation.ComparisonConstraints.IsLessOrEqualTo;

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

    [TestClass, TestCategory("MySQL - Fields")]
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

        [TestMethod] public void Type_Text() {
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
            decl.Should().Be($"`{name}` TEXT");
        }

        [TestMethod] public void Type_Varchar() {
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

        [TestMethod] public void Default_Text() {
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
            decl.Should().Be($"`{name}` TEXT DEFAULT \"{defaultValue}\"");
        }

        [TestMethod] public void Default_Varchar() {
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
            decl.Should().Be($"`{name}` DATETIME DEFAULT DATETIME \"2009-10-08 00:00:00\"");
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
            decl.Should().Be($"`{name}` DATETIME DEFAULT DATETIME \"1655-01-11 03:58:44\"");
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

        [TestMethod] public void VarcharWithUppercaseTextInDefaultValue() {
            // Arrange
            var name = new FieldName("Tartu");
            var type = DBType.Text;
            var maxLength = 4501UL;
            var defaultValue = "DEFAULT CONTAINS THE WORD TEXT WITH SPACES";

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

    [TestClass, TestCategory("MySQL - Tables")]
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
            var field0 = new FieldDecl(new FieldName("East Rutherford"), "`East Rutherford` TEXT NOT NULL");
            var field1 = new FieldDecl(new FieldName("West Bloomfield"), "`West Bloomfield` TEXT NOT NULL");
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
            var field0 = new FieldDecl(new FieldName("Greenville"), "`Greenville` TEXT NOT NULL");
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
                "`Greenville` TEXT NOT NULL\n" +
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
            var field1 = new FieldDecl(new FieldName("Independence"), "`Independence` TEXT NOT NULL");
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
                "`Independence` TEXT NOT NULL\n" +
                $"{pk}\n" +
                check.DDL.ToString()
            );
        }

        [TestMethod] public void FullTableDeclaration() {
            // Arrange
            var name = new TableName("IlTavolo");
            var field0 = new FieldDecl(new FieldName("Appomattox"), "`Appomattox` BIGINT UNSIGNED NOT NULL");
            var field1 = new FieldDecl(new FieldName("Chickamauga"), "`Chickamauga` TEXT NOT NULL DEFAULT \"--none--\"");
            var field2 = new FieldDecl(new FieldName("Kitty Hawk"), "`Kitty Hawk` INT DEFAULT NULL");
            var field3 = new FieldDecl(new FieldName("Harpers Ferry"), "`Harpers Ferry` BOOLEAN NOT NULL");
            var field4 = new FieldDecl(new FieldName("La Jolla"), "`La Jolla` TEXT NOT NULL");
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
                "`La Jolla` TEXT NOT NULL\n" +
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

    [TestClass, TestCategory("MySQL - Factory")]
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
}
