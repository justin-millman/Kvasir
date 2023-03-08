using Atropos.Moq;
using FluentAssertions;
using Kvasir.Schema;
using Kvasir.Transcription;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Optional;
using System.Collections.Generic;

namespace UT.Kvasir.Schema {
    [TestClass, TestCategory("CandidateKey")]
    public class CandidateKeyTests {
        [TestMethod] public void ConstructWithoutName() {
            // Arrange
            var fields = fields_;

            // Act
            var key = new CandidateKey(fields);

            // Assert
            key.Name.Should().NotHaveValue();
            key.Fields.Should().BeEquivalentTo(fields);
        }

        [TestMethod] public void ConstructWithName() {
            // Arrange
            var fields = fields_;
            var name = new KeyName("CandidateKey");

            // Act
            var key = new CandidateKey(name, fields);

            // Assert
            key.Name.Should().HaveValue(name);
            key.Fields.Should().BeEquivalentTo(fields);
        }

        [TestMethod] public void GenerateDeclarationWithoutName() {
            // Arrange
            var fields = fields_;
            var key = new CandidateKey(fields);
            var mockBuilder = new Mock<IKeyDeclBuilder<SqlSnippet>>();

            // Act
            (key as IKey).GenerateDeclaration(mockBuilder.Object);

            // Assert
            mockBuilder.Verify(builder => builder.SetFields(Arg.IsSameSequence<IEnumerable<IField>>(fields)));
            mockBuilder.Verify(builder => builder.Build());
            mockBuilder.VerifyNoOtherCalls();
        }

        [TestMethod] public void GenerateDeclarationWithName() {
            // Arrange
            var fields = fields_;
            var name = new KeyName("KeyName");
            var key = new CandidateKey(name, fields);
            var mockBuilder = new Mock<IKeyDeclBuilder<SqlSnippet>>();

            // Act
            (key as IKey).GenerateDeclaration(mockBuilder.Object);

            // Assert
            mockBuilder.Verify(builder => builder.SetName(name));
            mockBuilder.Verify(builder => builder.SetFields(Arg.IsSameSequence<IEnumerable<IField>>(fields)));
            mockBuilder.Verify(builder => builder.Build());
            mockBuilder.VerifyNoOtherCalls();
        }

        [TestMethod] public void Iteration() {
            // Arrange
            var fields = fields_;

            // Act
            var key = new CandidateKey(fields);
            var enumerator = key.GetEnumerator();
            IEnumerable<IField> list() { while (enumerator.MoveNext()) { yield return enumerator.Current; } };

            // Assert
            list().Should().BeEquivalentTo(fields);
        }


        static CandidateKeyTests() {
            fields_ = new List<IField>() {
                new BasicField(new FieldName("Field0"), DBType.Int32, IsNullable.No, Option.None<DBValue>()),
                new BasicField(new FieldName("Field1"), DBType.DateTime, IsNullable.No, Option.None<DBValue>()),
                new BasicField(new FieldName("Field2"), DBType.Boolean, IsNullable.No, Option.None<DBValue>())
            };
        }

        private static readonly IEnumerable<IField> fields_;
    }
}
