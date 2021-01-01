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

        [TestMethod, TestCategory("ConstraintName")]
        public void ValidConstraintNameNoTrim() {
            var contents = "CHK_IsPrimeNumber";
            var name = new ConstraintName(contents);

            Assert.AreEqual(contents, (string?)name);
        }

        [TestMethod, TestCategory("ConstraintName")]
        public void ValidConstraintNameTrim() {
            var contents = "     CHK_GraduationYearIsAfterBirthdate ";
            var name = new ConstraintName(contents);

            Assert.AreEqual(contents.Trim(), (string?)name);
        }

        [TestMethod, TestCategory("ConstraintName")]
        public void EmptyStringConstraintName() {
            var empty = string.Empty;

            void action() => new ConstraintName(empty);
            var exception = Assert.ThrowsException<ArgumentException>(action);
            Assert.AreNotEqual(string.Empty, exception.Message);
        }

        [TestMethod, TestCategory("ConstraintName")]
        public void WhitespaceOnlyConstraintName() {
            var whitespace = "    \n     \n\t  ";

            void action() => new ConstraintName(whitespace);
            var exception = Assert.ThrowsException<ArgumentException>(action);
            Assert.AreNotEqual(string.Empty, exception.Message);
        }

        [TestMethod, TestCategory("KeyName")]
        public void ValidKeyNameNoTrim() {
            var contents = "KEY_StartEndPair";
            var name = new KeyName(contents);

            Assert.AreEqual(contents, (string?)name);
        }

        [TestMethod, TestCategory("KeyName")]
        public void ValidKeyNameTrim() {
            var contents = "     PK_SAG_Name ";
            var name = new KeyName(contents);

            Assert.AreEqual(contents.Trim(), (string?)name);
        }

        [TestMethod, TestCategory("KeyName")]
        public void EmptyStringKeyName() {
            var empty = string.Empty;

            void action() => new KeyName(empty);
            var exception = Assert.ThrowsException<ArgumentException>(action);
            Assert.AreNotEqual(string.Empty, exception.Message);
        }

        [TestMethod, TestCategory("KeyName")]
        public void WhitespaceOnlyKeyName() {
            var whitespace = "    \n     \n\t  ";

            void action() => new KeyName(whitespace);
            var exception = Assert.ThrowsException<ArgumentException>(action);
            Assert.AreNotEqual(string.Empty, exception.Message);
        }
    }
}
