using Cybele.Extensions;
using Kvasir.Schema;
using Kvasir.Transcription.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Linq;

namespace Test.Kvasir.Schema {
    [TestClass]
    public class KeyTests {
        [TestMethod, TestCategory("CandidateKey")]
        public void CandidateKeyNoName() {
            var mockField = new Mock<IField>().WithName("StudentID");
            var key = new CandidateKey();
            key.AddField(mockField.Object);
            var fields = key.GetEnumerator().ToEnumerable().ToArray();

            Assert.IsFalse(key.Name.HasValue);
            Assert.AreEqual(1, fields.Length);
            Assert.AreSame(mockField.Object, fields[0]);
        }

        [TestMethod, TestCategory("CandidateKey")]
        public void CandidateKeyWithName() {
            var mockField = new Mock<IField>().WithName("SSN");
            var name = new KeyName("KEY_SSN");
            var key = new CandidateKey(name);
            key.AddField(mockField.Object);
            var fields = key.GetEnumerator().ToEnumerable().ToArray();

            Assert.IsTrue(key.Name.Contains(name));
            Assert.AreEqual(1, fields.Length);
            Assert.AreSame(mockField.Object, fields[0]);
        }

        [TestMethod, TestCategory("CandidateKey")]
        public void CandidateKeyAddDuplicateField() {
            var mockFieldA = new Mock<IField>().WithName("Channel");
            var mockFieldB = new Mock<IField>().WithName("IsNews");
            var key = new CandidateKey();
            key.AddField(mockFieldA.Object);
            key.AddField(mockFieldB.Object);
            key.AddField(mockFieldB.Object);
            key.AddField(mockFieldA.Object);
            key.AddField(mockFieldB.Object);
            var fields = key.GetEnumerator().ToEnumerable().ToArray();

            Assert.IsFalse(key.Name.HasValue);
            Assert.AreEqual(2, fields.Length);
            Assert.AreSame(mockFieldA.Object, fields[0]);
            Assert.AreSame(mockFieldB.Object, fields[1]);
        }

        [TestMethod, TestCategory("CandidateKey")]
        public void CandidateKeyAddRepeatNameDifferentField() {
            var mockFieldA = new Mock<IField>().WithName("ISBN");
            var mockFieldB = new Mock<IField>().WithName("ISBN");
            var key = new CandidateKey();
            key.AddField(mockFieldA.Object);
            key.AddField(mockFieldB.Object);
            var fields = key.GetEnumerator().ToEnumerable().ToArray();

            Assert.IsFalse(key.Name.HasValue);
            Assert.AreEqual(1, fields.Length);
            Assert.AreSame(mockFieldA.Object, fields[0]);
        }

        [TestMethod, TestCategory("CandidateKey")]
        public void CandidateKeySQLNoName() {
            var mockFieldA = new Mock<IField>().WithName("City");
            var mockFieldB = new Mock<IField>().WithName("State");
            var key = new CandidateKey();
            key.AddField(mockFieldA.Object);
            key.AddField(mockFieldB.Object);

            var mockBuilders = new Mock<IBuilderCollection>().MockByDefault();
            (key as IKey).GenerateDeclaration(mockBuilders.Object);
            mockBuilders.KeyBuilder().Verify(k => k.AddField(mockFieldA.Object.Name), Times.Exactly(1));
            mockBuilders.KeyBuilder().Verify(k => k.AddField(mockFieldB.Object.Name), Times.Exactly(1));
            mockBuilders.KeyBuilder().Verify(k => k.Build(), Times.Exactly(1));
            mockBuilders.KeyBuilder().VerifyNoOtherCalls();
        }

        [TestMethod, TestCategory("CandidateKey")]
        public void CandidateKeySQLWithName() {
            var mockFieldA = new Mock<IField>().WithName("City");
            var mockFieldB = new Mock<IField>().WithName("State");
            var name = new KeyName("KEY_Location");
            var key = new CandidateKey(name);
            key.AddField(mockFieldA.Object);
            key.AddField(mockFieldB.Object);

            var mockBuilders = new Mock<IBuilderCollection>().MockByDefault();
            (key as IKey).GenerateDeclaration(mockBuilders.Object);
            mockBuilders.KeyBuilder().Verify(k => k.SetName(name), Times.AtLeastOnce());
            mockBuilders.KeyBuilder().Verify(k => k.AddField(mockFieldA.Object.Name), Times.Exactly(1));
            mockBuilders.KeyBuilder().Verify(k => k.AddField(mockFieldB.Object.Name), Times.Exactly(1));
            mockBuilders.KeyBuilder().Verify(k => k.Build(), Times.Exactly(1));
            mockBuilders.KeyBuilder().VerifyNoOtherCalls();
        }

        [TestMethod, TestCategory("PrimaryKey")]
        public void PrimaryKeyNoName() {
            var mockField = new Mock<IField>().WithName("StudentID");
            var key = new PrimaryKey();
            key.AddField(mockField.Object);
            var fields = key.GetEnumerator().ToEnumerable().ToArray();

            Assert.IsFalse(key.Name.HasValue);
            Assert.AreEqual(1, fields.Length);
            Assert.AreSame(mockField.Object, fields[0]);
        }

        [TestMethod, TestCategory("PrimaryKey")]
        public void PrimaryKeyWithName() {
            var mockField = new Mock<IField>().WithName("SSN");
            var name = new KeyName("PKEY_SSN");
            var key = new PrimaryKey(name);
            key.AddField(mockField.Object);
            var fields = key.GetEnumerator().ToEnumerable().ToArray();

            Assert.IsTrue(key.Name.Contains(name));
            Assert.AreEqual(1, fields.Length);
            Assert.AreSame(mockField.Object, fields[0]);
        }

        [TestMethod, TestCategory("PrimaryKey")]
        public void PrimaryKeyAddDuplicateField() {
            var mockFieldA = new Mock<IField>().WithName("Channel");
            var mockFieldB = new Mock<IField>().WithName("IsNews");
            var key = new PrimaryKey();
            key.AddField(mockFieldA.Object);
            key.AddField(mockFieldB.Object);
            key.AddField(mockFieldB.Object);
            key.AddField(mockFieldA.Object);
            key.AddField(mockFieldB.Object);
            var fields = key.GetEnumerator().ToEnumerable().ToArray();

            Assert.IsFalse(key.Name.HasValue);
            Assert.AreEqual(2, fields.Length);
            Assert.AreSame(mockFieldA.Object, fields[0]);
            Assert.AreSame(mockFieldB.Object, fields[1]);
        }

        [TestMethod, TestCategory("PrimaryKey")]
        public void PrimaryKeyAddRepeatNameDifferentField() {
            var mockFieldA = new Mock<IField>().WithName("ISBN");
            var mockFieldB = new Mock<IField>().WithName("ISBN");
            var key = new PrimaryKey();
            key.AddField(mockFieldA.Object);
            key.AddField(mockFieldB.Object);
            var fields = key.GetEnumerator().ToEnumerable().ToArray();

            Assert.IsFalse(key.Name.HasValue);
            Assert.AreEqual(1, fields.Length);
            Assert.AreSame(mockFieldA.Object, fields[0]);
        }

        [TestMethod, TestCategory("PrimaryKey")]
        public void PrimaryKeyNullableField() {
            var mockField = new Mock<IField>().WithName("SaturatedFat").AsNullable();
            var key = new PrimaryKey();
            void action() => key.AddField(mockField.Object);
            var exception = Assert.ThrowsException<ArgumentException>(action);
            Assert.AreNotEqual(string.Empty, exception.Message);
        }

        [TestMethod, TestCategory("PrimaryKey")]
        public void PrimaryKeySQLNoName() {
            var mockFieldA = new Mock<IField>().WithName("City");
            var mockFieldB = new Mock<IField>().WithName("State");
            var key = new PrimaryKey();
            key.AddField(mockFieldA.Object);
            key.AddField(mockFieldB.Object);

            var mockBuilders = new Mock<IBuilderCollection>().MockByDefault();
            (key as IKey).GenerateDeclaration(mockBuilders.Object);
            mockBuilders.KeyBuilder().Verify(k => k.SetAsPrimary(), Times.AtLeast(1));
            mockBuilders.KeyBuilder().Verify(k => k.AddField(mockFieldA.Object.Name), Times.Exactly(1));
            mockBuilders.KeyBuilder().Verify(k => k.AddField(mockFieldB.Object.Name), Times.Exactly(1));
            mockBuilders.KeyBuilder().Verify(k => k.Build(), Times.Exactly(1));
            mockBuilders.KeyBuilder().VerifyNoOtherCalls();
        }

        [TestMethod, TestCategory("PrimaryKey")]
        public void PrimaryKeySQLWithName() {
            var mockFieldA = new Mock<IField>().WithName("City");
            var mockFieldB = new Mock<IField>().WithName("State");
            var name = new KeyName("PK_Location");
            var key = new PrimaryKey(name);
            key.AddField(mockFieldA.Object);
            key.AddField(mockFieldB.Object);

            var mockBuilders = new Mock<IBuilderCollection>().MockByDefault();
            (key as IKey).GenerateDeclaration(mockBuilders.Object);
            mockBuilders.KeyBuilder().Verify(k => k.SetName(name), Times.AtLeastOnce());
            mockBuilders.KeyBuilder().Verify(k => k.SetAsPrimary(), Times.AtLeastOnce());
            mockBuilders.KeyBuilder().Verify(k => k.AddField(mockFieldA.Object.Name), Times.Exactly(1));
            mockBuilders.KeyBuilder().Verify(k => k.AddField(mockFieldB.Object.Name), Times.Exactly(1));
            mockBuilders.KeyBuilder().Verify(k => k.Build(), Times.Exactly(1));
            mockBuilders.KeyBuilder().VerifyNoOtherCalls();
        }
    }
}
