using Kvasir.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Optional;
using System;

namespace Test.Kvasir.Schema {
    [TestClass]
    public class BasicFieldTests {
        [TestMethod, TestCategory("Default Value")]
        public void ConstructNoDefault() {
            var name = new FieldName("Country");
            var type = DBType.Text;
            var nullability = IsNullable.No;
            var defaultValue = Option.None<DBValue>();

            var field = new BasicField(name, type, nullability, defaultValue);
            Assert.AreEqual(name, field.Name);
            Assert.AreEqual(type, field.DataType);
            Assert.AreEqual(nullability, field.Nullability);
            Assert.AreEqual(defaultValue, field.DefaultValue);
        }

        [TestMethod, TestCategory("Default Value")]
        public void NullDefaultAllowed() {
            var name = new FieldName("PreviousRecord");
            var type = DBType.UInt16;
            var nullability = IsNullable.Yes;
            var defaultValue = Option.Some(DBValue.NULL);

            var field = new BasicField(name, type, nullability, defaultValue);
            Assert.AreEqual(name, field.Name);
            Assert.AreEqual(type, field.DataType);
            Assert.AreEqual(nullability, field.Nullability);
            Assert.AreEqual(defaultValue, field.DefaultValue);
        }

        [TestMethod, TestCategory("Default Value")]
        public void NullDefaultDisallowed() {
            var name = new FieldName("ASCII");
            var type = DBType.Character;
            var nullability = IsNullable.No;
            var defaultValue = Option.Some(DBValue.NULL);

            void action() => new BasicField(name, type, nullability, defaultValue);
            var exception = Assert.ThrowsException<ArgumentException>(action);
            Assert.AreNotEqual(string.Empty, exception.Message);
        }

        [TestMethod, TestCategory("Default Value")]
        public void NonNullDefaultCorrectType() {
            var name = new FieldName("DOB");
            var type = DBType.DateTime;
            var nullability = IsNullable.Yes;
            var defaultValue = Option.Some(DBValue.Create(new DateTime(1467, 04, 02)));

            var field = new BasicField(name, type, nullability, defaultValue);
            Assert.AreEqual(name, field.Name);
            Assert.AreEqual(type, field.DataType);
            Assert.AreEqual(nullability, field.Nullability);
            Assert.AreEqual(defaultValue, field.DefaultValue);
        }

        [TestMethod, TestCategory("Default Value")]
        public void NonNullDefaultIncorrectType() {
            var name = new FieldName("Cost");
            var type = DBType.Decimal;
            var nullability = IsNullable.No;
            var defaultValue = Option.Some(DBValue.Create(-1934));

            void action() => new BasicField(name, type, nullability, defaultValue);
            var exception = Assert.ThrowsException<ArgumentException>(action);
            Assert.AreNotEqual(string.Empty, exception.Message);
        }
    }
}
