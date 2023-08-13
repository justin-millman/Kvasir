using Atropos.NSubstitute;
using FluentAssertions;
using Kvasir.Schema;
using Kvasir.Transcription;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Optional;
using System.Collections.Generic;

namespace UT.Kvasir.Schema {
    [TestClass, TestCategory("PrimaryKey")]
    public class PrimaryKeyTests {
        [TestMethod] public void ConstructWithoutName() {
            // Arrange
            var fields = fields_;

            // Act
            var key = new PrimaryKey(fields);

            // Assert
            key.Name.Should().NotHaveValue();
            key.Fields.Should().BeEquivalentTo(fields);
        }

        [TestMethod] public void ConstructWithName() {
            // Arrange
            var fields = fields_;
            var name = new KeyName("PrimaryKey");

            // Act
            var key = new PrimaryKey(name, fields);

            // Assert
            key.Name.Should().HaveValue(name);
            key.Fields.Should().BeEquivalentTo(fields);
        }

        [TestMethod] public void GenerateDeclarationWithoutName() {
            // Arrange
            var fields = fields_;
            var key = new PrimaryKey(fields);
            var mockBuilder = Substitute.For<IKeyDeclBuilder<SqlSnippet>>();

            // Act
            (key as IKey).GenerateDeclaration(mockBuilder);

            // Assert
            mockBuilder.Received().SetFields(NArg.IsSameSequence<IEnumerable<IField>>(fields));
            mockBuilder.Received().SetAsPrimaryKey();
            mockBuilder.Received().Build();
            mockBuilder.ReceivedCalls().Should().HaveCount(3);
        }

        [TestMethod] public void GenerateDeclarationWithName() {
            // Arrange
            var fields = fields_;
            var name = new KeyName("KeyName");
            var key = new PrimaryKey(name, fields);
            var mockBuilder = Substitute.For<IKeyDeclBuilder<SqlSnippet>>();

            // Act
            (key as IKey).GenerateDeclaration(mockBuilder);

            // Assert
            mockBuilder.Received().SetName(name);
            mockBuilder.Received().SetFields(NArg.IsSameSequence<IEnumerable<IField>>(fields));
            mockBuilder.Received().SetAsPrimaryKey();
            mockBuilder.Received().Build();
            mockBuilder.ReceivedCalls().Should().HaveCount(4);
        }

        [TestMethod] public void Iteration() {
            // Arrange
            var fields = fields_;

            // Act
            var key = new PrimaryKey(fields);
            var enumerator = key.GetEnumerator();
            IEnumerable<IField> list() { while (enumerator.MoveNext()) { yield return enumerator.Current; } };

            // Assert
            list().Should().BeEquivalentTo(fields);
        }


        static PrimaryKeyTests() {
            fields_ = new List<IField>() {
                new BasicField(new FieldName("Field0"), DBType.Int32, IsNullable.No, Option.None<DBValue>()),
                new BasicField(new FieldName("Field1"), DBType.DateTime, IsNullable.No, Option.None<DBValue>()),
                new BasicField(new FieldName("Field2"), DBType.Boolean, IsNullable.No, Option.None<DBValue>())
            };
        }

        private static readonly IEnumerable<IField> fields_;
    }
}
