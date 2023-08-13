using Atropos.NSubstitute;
using FluentAssertions;
using Kvasir.Schema;
using Kvasir.Transcription;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Collections.Generic;

namespace UT.Kvasir.Schema {
    [TestClass, TestCategory("ForeignKey")]
    public class ForeignKeyTests {
        [TestMethod] public void ConstructWithoutName() {
            // Arrange
            var reference = referenceTable_;
            var fields = fields_;
            var onDelete = OnDelete.SetDefault;
            var onUpdate = OnUpdate.Cascade;

            // Act
            var fkey = new ForeignKey(reference, fields, onDelete, onUpdate);

            // Assert
            fkey.Name.Should().NotHaveValue();
            fkey.ReferencedTable.Should().BeSameAs(reference);
            fkey.ReferencingFields.Should().BeEquivalentTo(fields);
            fkey.OnDelete.Should().Be(onDelete);
            fkey.OnUpdate.Should().Be(onUpdate);
        }

        [TestMethod] public void ConstructWithName() {
            // Arrange
            var name = new FKName("FKEY");
            var reference = referenceTable_;
            var fields = fields_;
            var onDelete = OnDelete.SetDefault;
            var onUpdate = OnUpdate.Cascade;

            // Act
            var fkey = new ForeignKey(name, reference, fields, onDelete, onUpdate);

            // Assert
            fkey.Name.Should().HaveValue(name);
            fkey.ReferencedTable.Should().BeSameAs(reference);
            fkey.ReferencingFields.Should().BeEquivalentTo(fields);
            fkey.OnDelete.Should().Be(onDelete);
            fkey.OnUpdate.Should().Be(onUpdate);
        }

        [TestMethod] public void GenerateDeclarationWithoutName() {
            // Arrange
            var reference = referenceTable_;
            var fields = fields_;
            var onDelete = OnDelete.SetDefault;
            var onUpdate = OnUpdate.Cascade;
            var fkey = new ForeignKey(reference, fields, onDelete, onUpdate);
            var mockBuilder = Substitute.For<IForeignKeyDeclBuilder<SqlSnippet>>();

            // Act
            fkey.GenerateDeclaration(mockBuilder);

            // Assert
            mockBuilder.Received().SetOnDeleteBehavior(onDelete);
            mockBuilder.Received().SetOnUpdateBehavior(onUpdate);
            mockBuilder.Received().SetReferencedTable(reference);
            mockBuilder.Received().SetFields(NArg.IsSameSequence<IEnumerable<IField>>(fields));
            mockBuilder.Received().Build();
            mockBuilder.ReceivedCalls().Should().HaveCount(5);
        }

        [TestMethod] public void GenerateDeclarationWithName() {
            // Arrange
            var name = new FKName("FKEY");
            var reference = referenceTable_;
            var fields = fields_;
            var onDelete = OnDelete.SetDefault;
            var onUpdate = OnUpdate.Cascade;
            var fkey = new ForeignKey(name, reference, fields, onDelete, onUpdate);
            var mockBuilder = Substitute.For<IForeignKeyDeclBuilder<SqlSnippet>>();

            // Act
            fkey.GenerateDeclaration(mockBuilder);

            // Assert
            mockBuilder.Received().SetName(name);
            mockBuilder.Received().SetOnDeleteBehavior(onDelete);
            mockBuilder.Received().SetOnUpdateBehavior(onUpdate);
            mockBuilder.Received().SetReferencedTable(reference);
            mockBuilder.Received().SetFields(NArg.IsSameSequence<IEnumerable<IField>>(fields));
            mockBuilder.Received().Build();
            mockBuilder.ReceivedCalls().Should().HaveCount(6);
        }

        [TestMethod] public void Iteration() {
            // Arrange
            var reference = referenceTable_;
            var fields = fields_;

            // Act
            var key = new ForeignKey(reference, fields, OnDelete.Cascade, OnUpdate.Prevent);
            var enumerator = key.GetEnumerator();
            IEnumerable<IField> list() { while (enumerator.MoveNext()) { yield return enumerator.Current; } };

            // Assert
            list().Should().BeEquivalentTo(fields);
        }


        static ForeignKeyTests() {
            var mockKeyField0 = Substitute.For<IField>();
            mockKeyField0.Nullability.Returns(IsNullable.No);
            mockKeyField0.DataType.Returns(DBType.Int32);
            var mockKeyField1 = Substitute.For<IField>();
            mockKeyField1.Nullability.Returns(IsNullable.No);
            mockKeyField1.DataType.Returns(DBType.Text);

            fields_ = new List<IField>() { mockKeyField0, mockKeyField1 };

            var pk = new PrimaryKey(fields_);
            var mockTable = Substitute.For<ITable>();
            mockTable.PrimaryKey.Returns(pk);

            referenceTable_ = mockTable;
        }

        private static readonly ITable referenceTable_;
        private static readonly IEnumerable<IField> fields_;
    }
}
