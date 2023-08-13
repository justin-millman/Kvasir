using FluentAssertions;
using Kvasir.Schema;
using Kvasir.Transcription;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
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

            
            var mockConstraintDeclBuilder = Substitute.For<IConstraintDeclBuilder<SqlSnippet>>();
            mockConstraintDeclBuilder.Build().Returns(snippet);
            var mockFieldDeclBuilder = Substitute.For<IFieldDeclBuilder<SqlSnippet>>();
            mockFieldDeclBuilder.Build().Returns(snippet);
            var mockForeignKeyDeclBuilder = Substitute.For<IForeignKeyDeclBuilder<SqlSnippet>>();
            mockForeignKeyDeclBuilder.Build().Returns(snippet);
            var mockKeyDeclBuilder = Substitute.For<IKeyDeclBuilder<SqlSnippet>>();
            mockKeyDeclBuilder.Build().Returns(snippet);
            var mockFactory = Substitute.For<IBuilderFactory<SqlSnippet, SqlSnippet, SqlSnippet, SqlSnippet, SqlSnippet>>();
            mockFactory.NewConstraintDeclBuilder().Returns(mockConstraintDeclBuilder);
            mockFactory.NewFieldDeclBuilder().Returns(mockFieldDeclBuilder);
            mockFactory.NewForeignKeyDeclBuilder().Returns(mockForeignKeyDeclBuilder);
            mockFactory.NewKeyDeclBuilder().Returns(mockKeyDeclBuilder);
            //mockFactory.NewTableDeclBuilder().Returns(Substitute.For());

            // Act
            (table as ITable).GenerateDeclaration(mockFactory);

            // Assert
            mockFactory.NewTableDeclBuilder().Received().SetName(table.Name);
            mockFactory.NewTableDeclBuilder().Received(1).SetPrimaryKeyDeclaration(snippet);
            mockFactory.NewTableDeclBuilder().Received(1).AddCandidateKeyDeclaration(snippet);
            mockFactory.NewTableDeclBuilder().Received(1).AddCheckConstraintDeclaration(snippet);
            mockFactory.NewTableDeclBuilder().Received(3).AddFieldDeclaration(snippet);
            mockFactory.NewTableDeclBuilder().Received(1).AddForeignKeyDeclaration(snippet);
            mockFactory.NewTableDeclBuilder().Received().Build();
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

            var mockField0 = Substitute.For<IField>();
            mockField0.Name.Returns(new FieldName("A"));
            mockField0.Nullability.Returns(IsNullable.No);
            mockField0.DataType.Returns(DBType.Int32);
            mockField0.GenerateDeclaration(Arg.Any<IFieldDeclBuilder<SqlSnippet>>()).Returns(snippet_);
            var mockField1 = Substitute.For<IField>();
            mockField1.Name.Returns(new FieldName("B"));
            mockField1.Nullability.Returns(IsNullable.No);
            mockField1.DataType.Returns(DBType.Guid);
            mockField1.GenerateDeclaration(Arg.Any<IFieldDeclBuilder<SqlSnippet>>()).Returns(snippet_);
            var mockField2 = Substitute.For<IField>();
            mockField2.Name.Returns(new FieldName("C"));
            mockField2.Nullability.Returns(IsNullable.No);
            mockField2.DataType.Returns(DBType.Enumeration);
            mockField2.GenerateDeclaration(Arg.Any<IFieldDeclBuilder<SqlSnippet>>()).Returns(snippet_);

            var pkey = new PrimaryKey(new IField[] { mockField0 });
            var ckey = new CandidateKey(new IField[] { mockField1 });
            var check = new CheckConstraint(Substitute.For<Clause>());

            var mockReferenceTable = Substitute.For<ITable>();
            mockReferenceTable.PrimaryKey.Returns(pkey);
            var fkey = new ForeignKey(mockReferenceTable, pkey.Fields, OnDelete.SetNull, OnUpdate.NoAction);

            table_ = new Table(new TableName("TABLE"),
                new IField[] { mockField1, mockField2, mockField0 }, pkey,
                new CandidateKey[] { ckey }, new ForeignKey[] { fkey }, new CheckConstraint[] { check });
        }
        
        private static readonly Table table_;
        private static readonly SqlSnippet snippet_;
    }
}
