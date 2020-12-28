using Kvasir.Schema;
using Kvasir.Transcription.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Test.Mocks;

namespace Test.Kvasir.Schema {
    [TestClass]
    public class KeyTests {
        [TestMethod, TestCategory("CandidateKey")]
        public void CandidateKeyNoName() {
            var mock = new Mock<IField>();
            mock.Setup(f => f.Name).Returns(new FieldName("StudentID"));

            var key = new CandidateKey();
            key.AddField(mock.Object);
            Assert.IsFalse(key.Name.HasValue);

            var expected = new SqlSnippet("CONSTRAINT /unnamed/ UNIQUE (StudentID)");
            var actual = (key as IKey).GenerateDeclaration(new MockBuilders());
            Assert.AreEqual(expected, actual);
        }

        [TestMethod, TestCategory("CandidateKey")]
        public void CandidateKeyWithName() {
            var mock = new Mock<IField>();
            mock.Setup(f => f.Name).Returns(new FieldName("SSN"));

            var name = new KeyName("KEY_SSN");
            var key = new CandidateKey(name);
            key.AddField(mock.Object);

            Assert.IsTrue(key.Name.Contains(name));
            var expected = new SqlSnippet("CONSTRAINT KEY_SSN UNIQUE (SSN)");
            var actual = (key as IKey).GenerateDeclaration(new MockBuilders());
            Assert.AreEqual(expected, actual);
        }

        [TestMethod, TestCategory("CandidateKey")]
        public void CandidateKeyAddDuplicateField() {
            var mockA = new Mock<IField>();
            mockA.Setup(f => f.Name).Returns(new FieldName("Channel"));
            var mockB = new Mock<IField>();
            mockB.Setup(f => f.Name).Returns(new FieldName("IsNews"));

            var key = new CandidateKey();
            key.AddField(mockA.Object);
            key.AddField(mockB.Object);
            key.AddField(mockB.Object);
            key.AddField(mockA.Object);
            key.AddField(mockB.Object);

            var expected = new SqlSnippet("CONSTRAINT /unnamed/ UNIQUE (Channel, IsNews)");
            var actual = (key as IKey).GenerateDeclaration(new MockBuilders());
            Assert.AreEqual(expected, actual);
        }

        [TestMethod, TestCategory("CandidateKey")]
        public void CandidateKeyAddRepeatNameDifferentField() {
            var mockA = new Mock<IField>();
            mockA.Setup(f => f.Name).Returns(new FieldName("ISBN"));
            var mockB = new Mock<IField>();
            mockB.Setup(f => f.Name).Returns(mockA.Object.Name);

            var key = new CandidateKey();
            key.AddField(mockA.Object);
            key.AddField(mockB.Object);

            var expected = new SqlSnippet("CONSTRAINT /unnamed/ UNIQUE (ISBN)");
            var actual = (key as IKey).GenerateDeclaration(new MockBuilders());
            Assert.AreEqual(expected, actual);
        }

        [TestMethod, TestCategory("PrimaryKey")]
        public void PrimaryKeyNoName() {
            var mock = new Mock<IField>();
            mock.Setup(f => f.Name).Returns(new FieldName("StudentID"));
            mock.Setup(f => f.Nullability).Returns(IsNullable.No);

            var key = new PrimaryKey();
            key.AddField(mock.Object);
            Assert.IsFalse(key.Name.HasValue);

            var expected = new SqlSnippet("CONSTRAINT /unnamed/ PRIMARY-KEY (StudentID)");
            var actual = (key as IKey).GenerateDeclaration(new MockBuilders());
            Assert.AreEqual(expected, actual);
        }

        [TestMethod, TestCategory("PrimaryKey")]
        public void PrimaryKeyWithName() {
            var mock = new Mock<IField>();
            mock.Setup(f => f.Name).Returns(new FieldName("SSN"));
            mock.Setup(f => f.Nullability).Returns(IsNullable.No);

            var name = new KeyName("PKEY_SSN");
            var key = new PrimaryKey(name);
            key.AddField(mock.Object);

            Assert.IsTrue(key.Name.Contains(name));
            var expected = new SqlSnippet("CONSTRAINT PKEY_SSN PRIMARY-KEY (SSN)");
            var actual = (key as IKey).GenerateDeclaration(new MockBuilders());
            Assert.AreEqual(expected, actual);
        }

        [TestMethod, TestCategory("PrimaryKey")]
        public void PrimaryKeyAddDuplicateField() {
            var mockA = new Mock<IField>();
            mockA.Setup(f => f.Name).Returns(new FieldName("Channel"));
            mockA.Setup(f => f.Nullability).Returns(IsNullable.No);
            var mockB = new Mock<IField>();
            mockB.Setup(f => f.Name).Returns(new FieldName("IsNews"));
            mockB.Setup(f => f.Nullability).Returns(IsNullable.No);

            var key = new PrimaryKey();
            key.AddField(mockA.Object);
            key.AddField(mockB.Object);
            key.AddField(mockB.Object);
            key.AddField(mockA.Object);
            key.AddField(mockB.Object);

            var expected = new SqlSnippet("CONSTRAINT /unnamed/ PRIMARY-KEY (Channel, IsNews)");
            var actual = (key as IKey).GenerateDeclaration(new MockBuilders());
            Assert.AreEqual(expected, actual);
        }

        [TestMethod, TestCategory("PrimaryKey")]
        public void PrimaryKeyAddRepeatNameDifferentField() {
            var mockA = new Mock<IField>();
            mockA.Setup(f => f.Name).Returns(new FieldName("ISBN"));
            mockA.Setup(f => f.Nullability).Returns(IsNullable.No);
            var mockB = new Mock<IField>();
            mockB.Setup(f => f.Name).Returns(mockA.Object.Name);
            mockB.Setup(f => f.Nullability).Returns(IsNullable.No);

            var key = new PrimaryKey();
            key.AddField(mockA.Object);
            key.AddField(mockB.Object);

            var expected = new SqlSnippet("CONSTRAINT /unnamed/ PRIMARY-KEY (ISBN)");
            var actual = (key as IKey).GenerateDeclaration(new MockBuilders());
            Assert.AreEqual(expected, actual);
        }

        [TestMethod, TestCategory("PrimaryKey")]
        public void PrimaryKeyNullableField() {
            var mock = new Mock<IField>();
            mock.Setup(f => f.Name).Returns(new FieldName("SaturatedFat"));
            mock.Setup(f => f.Nullability).Returns(IsNullable.Yes);

            var key = new PrimaryKey();
            void action() => key.AddField(mock.Object);
            var exception = Assert.ThrowsException<ArgumentException>(action);
            Assert.AreNotEqual(String.Empty, exception.Message);
        }
    }
}
