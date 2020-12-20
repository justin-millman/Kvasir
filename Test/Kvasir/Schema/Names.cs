using Kvasir.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Test.Kvasir.Schema {
    [TestClass]
    public class NameTests {
        [TestMethod, TestCategory("FieldName")]
        public void ValidFieldNameNoTrim() {
            var contents = "FirstName";
            var name = new FieldName(contents);

            Assert.AreEqual(contents, (string?)name);
        }

        [TestMethod, TestCategory("FieldName")]
        public void ValidFieldNameTrim() {
            var contents = "     Gematria ";
            var name = new FieldName(contents);

            Assert.AreEqual(contents.Trim(), (string?)name);
        }

        [TestMethod, TestCategory("FieldName")]
        public void EmptyStringFieldName() {
            var empty = string.Empty;

            void action() => new FieldName(empty);
            var exception = Assert.ThrowsException<ArgumentException>(action);
            Assert.AreNotEqual(String.Empty, exception.Message);
        }

        [TestMethod, TestCategory("FieldName")]
        public void WhitespaceOnlyFieldName() {
            var whitespace = "    \n     \n\t  ";

            void action() => new FieldName(whitespace);
            var exception = Assert.ThrowsException<ArgumentException>(action);
            Assert.AreNotEqual(string.Empty, exception.Message);
        }
    }
}
