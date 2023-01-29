using Atropos.Moq;
using FluentAssertions;
using Kvasir.Schema;
using Kvasir.Transcription;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
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
            var mockBuilder = new Mock<IForeignKeyDeclBuilder<SqlSnippet>>();

            // Act
            fkey.GenerateDeclaration(mockBuilder.Object);

            // Assert
            mockBuilder.Verify(builder => builder.SetOnDeleteBehavior(onDelete));
            mockBuilder.Verify(builder => builder.SetOnUpdateBehavior(onUpdate));
            mockBuilder.Verify(builder => builder.SetReferencedTable(reference));
            mockBuilder.Verify(builder => builder.SetFields(Arg.IsSameSequence<IEnumerable<IField>>(fields)));
            mockBuilder.Verify(builder => builder.Build());
            mockBuilder.VerifyNoOtherCalls();
        }

        [TestMethod] public void GenerateDeclarationWithName() {
            // Arrange
            var name = new FKName("FKEY");
            var reference = referenceTable_;
            var fields = fields_;
            var onDelete = OnDelete.SetDefault;
            var onUpdate = OnUpdate.Cascade;
            var fkey = new ForeignKey(name, reference, fields, onDelete, onUpdate);
            var mockBuilder = new Mock<IForeignKeyDeclBuilder<SqlSnippet>>();

            // Act
            fkey.GenerateDeclaration(mockBuilder.Object);

            // Assert
            mockBuilder.Verify(builder => builder.SetName(name));
            mockBuilder.Verify(builder => builder.SetOnDeleteBehavior(onDelete));
            mockBuilder.Verify(builder => builder.SetOnUpdateBehavior(onUpdate));
            mockBuilder.Verify(builder => builder.SetReferencedTable(reference));
            mockBuilder.Verify(builder => builder.SetFields(Arg.IsSameSequence<IEnumerable<IField>>(fields)));
            mockBuilder.Verify(builder => builder.Build());
            mockBuilder.VerifyNoOtherCalls();
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
            var mockKeyField0 = new Mock<IField>();
            mockKeyField0.Setup(field => field.Nullability).Returns(IsNullable.No);
            mockKeyField0.Setup(field => field.DataType).Returns(DBType.Int32);
            var mockKeyField1 = new Mock<IField>();
            mockKeyField1.Setup(field => field.Nullability).Returns(IsNullable.No);
            mockKeyField1.Setup(field => field.DataType).Returns(DBType.Text);

            fields_ = new List<IField>() { mockKeyField0.Object, mockKeyField1.Object };

            var pk = new PrimaryKey(fields_);
            var mockTable = new Mock<ITable>();
            mockTable.Setup(table => table.PrimaryKey).Returns(pk);

            referenceTable_ = mockTable.Object;
        }

        private static readonly ITable referenceTable_;
        private static readonly IEnumerable<IField> fields_;
    }
}
