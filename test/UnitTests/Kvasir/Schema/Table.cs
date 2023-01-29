using FluentAssertions;
using Kvasir.Schema;
using Kvasir.Transcription;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;

namespace UT.Kvasir.Schema {
    [TestClass, TestCategory("Table")]
    public class TableTests {
        [TestMethod] public void Dimension() {
            // Arrange
            var table = table_;

            // Act
            var dimension = table.Dimension;

            // Assert
            dimension.Should().Be(table.Fields.Count);
        }

        [TestMethod] public void LookupExistingFieldByName() {
            // Arrange
            var table = table_;
            var probe = new FieldName("A");

            // Act
            var field = table[probe];

            // Assert
            field.Name.Should<FieldName>().Be(probe);
            table.Fields.Should().Contain(field);
        }

        [TestMethod] public void LookupNonExistingFieldByName() {
            // Arrange
            var table = table_;
            var probe = new FieldName("b");

            // Act
            Func<IField> action = () => table[probe];

            // Assert
            action.Should().ThrowExactly<ArgumentException>().WithAnyMessage();
        }

        [TestMethod] public void GenerateDeclaration() {
            // Arrange
            var table = table_;
            var snippet = snippet_;

            var mockFactory = new Mock<IBuilderFactory<SqlSnippet, SqlSnippet, SqlSnippet, SqlSnippet, SqlSnippet>>();
            mockFactory.DefaultValue = DefaultValue.Mock;

            var mockConstraintBuilder = Mock.Get(mockFactory.Object.NewConstraintDeclBuilder());
            mockConstraintBuilder.Setup(builder => builder.Build()).Returns(snippet);
            var mockFieldBuilder = Mock.Get(mockFactory.Object.NewFieldDeclBuilder());
            mockFieldBuilder.Setup(builder => builder.Build()).Returns(snippet);
            var mockForeignKeyDeclBuilder = Mock.Get(mockFactory.Object.NewForeignKeyDeclBuilder());
            mockForeignKeyDeclBuilder.Setup(builder => builder.Build()).Returns(snippet);
            var mockKeyDeclBuilder = Mock.Get(mockFactory.Object.NewKeyDeclBuilder());
            mockKeyDeclBuilder.Setup(builder => builder.Build()).Returns(snippet);
            var mockTableBuilder = Mock.Get(mockFactory.Object.NewTableDeclBuilder());

            // Act
            (table as ITable).GenerateDeclaration(mockFactory.Object);

            // Assert
            mockTableBuilder.Verify(builder => builder.SetName(table.Name));
            mockTableBuilder.Verify(builder => builder.SetPrimaryKeyDeclaration(snippet), Times.Once);
            mockTableBuilder.Verify(builder => builder.AddCandidateKeyDeclaration(snippet), Times.Once);
            mockTableBuilder.Verify(builder => builder.AddCheckConstraintDeclaration(snippet), Times.Once);
            mockTableBuilder.Verify(builder => builder.AddFieldDeclaration(snippet), Times.Exactly(3));
            mockTableBuilder.Verify(builder => builder.AddForeignKeyDeclaration(snippet), Times.Once);
            mockTableBuilder.Verify(builder => builder.Build());
            mockTableBuilder.VerifyNoOtherCalls();
        }

        [TestMethod] public void Iteration() {
            // Arrange
            var table = table_;

            // Act
            var enumerator = table.GetEnumerator();
            IEnumerable<IField> list() { while (enumerator.MoveNext()) { yield return enumerator.Current; } };

            // Assert
            list().Should().BeEquivalentTo(table.Fields);
        }


        static TableTests() {
            snippet_ = new SqlSnippet("SQL");

            var mockField0 = new Mock<IField>();
            mockField0.Setup(field => field.Name).Returns(new FieldName("A"));
            mockField0.Setup(field => field.Nullability).Returns(IsNullable.No);
            mockField0.Setup(field => field.DataType).Returns(DBType.Int32);
            mockField0.Setup(field => field.GenerateDeclaration(It.IsAny<IFieldDeclBuilder<SqlSnippet>>())).Returns(snippet_);
            var mockField1 = new Mock<IField>();
            mockField1.Setup(field => field.Name).Returns(new FieldName("B"));
            mockField1.Setup(field => field.Nullability).Returns(IsNullable.No);
            mockField1.Setup(field => field.DataType).Returns(DBType.Guid);
            mockField1.Setup(field => field.GenerateDeclaration(It.IsAny<IFieldDeclBuilder<SqlSnippet>>())).Returns(snippet_);
            var mockField2 = new Mock<IField>();
            mockField2.Setup(field => field.Name).Returns(new FieldName("C"));
            mockField2.Setup(field => field.Nullability).Returns(IsNullable.No);
            mockField2.Setup(field => field.DataType).Returns(DBType.Enumeration);
            mockField2.Setup(field => field.GenerateDeclaration(It.IsAny<IFieldDeclBuilder<SqlSnippet>>())).Returns(snippet_);

            var pkey = new PrimaryKey(new IField[] { mockField0.Object });
            var ckey = new CandidateKey(new IField[] { mockField1.Object });
            var check = new CheckConstraint(new Mock<Clause>().Object);

            var mockReferenceTable = new Mock<ITable>();
            mockReferenceTable.Setup(table => table.PrimaryKey).Returns(pkey);
            var fkey = new ForeignKey(mockReferenceTable.Object, pkey.Fields, OnDelete.SetNull, OnUpdate.NoAction);

            table_ = new Table(new TableName("TABLE"),
                new IField[] { mockField1.Object, mockField2.Object, mockField0.Object }, pkey,
                new CandidateKey[] { ckey }, new ForeignKey[] { fkey }, new CheckConstraint[] { check });
        }
        
        private static readonly Table table_;
        private static readonly SqlSnippet snippet_;
    }
}
