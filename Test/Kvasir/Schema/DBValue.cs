using Kvasir.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Test.Kvasir.Schema {
    [TestClass]
    public class DBValueTests {
        [TestMethod, TestCategory("Boolean")]
        public void CreateBoolean() {
            bool val = true;

            var direct = new DBValue(val);
            var indirect = DBValue.Create(val);

            Assert.AreEqual(val, direct.Datum);
            Assert.AreEqual(val, indirect.Datum);
            Assert.AreEqual(direct, indirect);
        }

        [TestMethod, TestCategory("Integer")]
        public void CreateByte() {
            byte val = 211;

            var direct = new DBValue(val);
            var indirect = DBValue.Create(val);

            Assert.AreEqual(val, direct.Datum);
            Assert.AreEqual(val, indirect.Datum);
            Assert.AreEqual(direct, indirect);
        }

        [TestMethod, TestCategory("Integer")]
        public void CreateSByte() {
            sbyte val = 3;

            var direct = new DBValue(val);
            var indirect = DBValue.Create(val);

            Assert.AreEqual(val, direct.Datum);
            Assert.AreEqual(val, indirect.Datum);
            Assert.AreEqual(direct, indirect);
        }

        [TestMethod, TestCategory("Integer")]
        public void CreateUShort() {
            ushort val = 8770;

            var direct = new DBValue(val);
            var indirect = DBValue.Create(val);

            Assert.AreEqual(val, direct.Datum);
            Assert.AreEqual(val, indirect.Datum);
            Assert.AreEqual(direct, indirect);
        }

        [TestMethod, TestCategory("Integer")]
        public void CreateShort() {
            short val = -65;

            var direct = new DBValue(val);
            var indirect = DBValue.Create(val);

            Assert.AreEqual(val, direct.Datum);
            Assert.AreEqual(val, indirect.Datum);
            Assert.AreEqual(direct, indirect);
        }

        [TestMethod, TestCategory("Integer")]
        public void CreateUInt() {
            uint val = 118192;

            var direct = new DBValue(val);
            var indirect = DBValue.Create(val);

            Assert.AreEqual(val, direct.Datum);
            Assert.AreEqual(val, indirect.Datum);
            Assert.AreEqual(direct, indirect);
        }

        [TestMethod, TestCategory("Integer")]
        public void CreateInt() {
            int val = -88162;

            var direct = new DBValue(val);
            var indirect = DBValue.Create(val);

            Assert.AreEqual(val, direct.Datum);
            Assert.AreEqual(val, indirect.Datum);
            Assert.AreEqual(direct, indirect);
        }

        [TestMethod, TestCategory("Integer")]
        public void CreateULong() {
            ulong val = 717273812;

            var direct = new DBValue(val);
            var indirect = DBValue.Create(val);

            Assert.AreEqual(val, direct.Datum);
            Assert.AreEqual(val, indirect.Datum);
            Assert.AreEqual(direct, indirect);
        }

        [TestMethod, TestCategory("Integer")]
        public void CreateLong() {
            long val = 0;

            var direct = new DBValue(val);
            var indirect = DBValue.Create(val);

            Assert.AreEqual(val, direct.Datum);
            Assert.AreEqual(val, indirect.Datum);
            Assert.AreEqual(direct, indirect);
        }

        [TestMethod, TestCategory("Floating Point")]
        public void CreateFloat() {
            float val = -6.182f;

            var direct = new DBValue(val);
            var indirect = DBValue.Create(val);

            Assert.AreEqual(val, direct.Datum);
            Assert.AreEqual(val, indirect.Datum);
            Assert.AreEqual(direct, indirect);
        }

        [TestMethod, TestCategory("Floating Point")]
        public void CreateDouble() {
            double val = 2091.22108;

            var direct = new DBValue(val);
            var indirect = DBValue.Create(val);

            Assert.AreEqual(val, direct.Datum);
            Assert.AreEqual(val, indirect.Datum);
            Assert.AreEqual(direct, indirect);
        }

        [TestMethod, TestCategory("Floating Point")]
        public void CreateDecimal() {
            decimal val = 0.022103M;

            var direct = new DBValue(val);
            var indirect = DBValue.Create(val);

            Assert.AreEqual(val, direct.Datum);
            Assert.AreEqual(val, indirect.Datum);
            Assert.AreEqual(direct, indirect);
        }

        [TestMethod, TestCategory("Time")]
        public void CreateDateTime() {
            DateTime val = new DateTime(1996, 02, 29);

            var direct = new DBValue(val);
            var indirect = DBValue.Create(val);

            Assert.AreEqual(val, direct.Datum);
            Assert.AreEqual(val, indirect.Datum);
            Assert.AreEqual(direct, indirect);
        }

        [TestMethod, TestCategory("Text")]
        public void CreateChar() {
            char val = '@';

            var direct = new DBValue(val);
            var indirect = DBValue.Create(val);

            Assert.AreEqual(val, direct.Datum);
            Assert.AreEqual(val, indirect.Datum);
            Assert.AreEqual(direct, indirect);
        }

        [TestMethod, TestCategory("Text")]
        public void CreateString() {
            string val = "Avatar: The Last Airbender";

            var direct = new DBValue(val);
            var indirect = DBValue.Create(val);

            Assert.AreEqual(val, direct.Datum);
            Assert.AreEqual(val, indirect.Datum);
            Assert.AreEqual(direct, indirect);
        }

        [TestMethod, TestCategory("Text")]
        public void CreateGuid() {
            Guid val = Guid.NewGuid();

            var direct = new DBValue(val);
            var indirect = DBValue.Create(val);

            Assert.AreEqual(val, direct.Datum);
            Assert.AreEqual(val, indirect.Datum);
            Assert.AreEqual(direct, indirect);
        }

        [TestMethod, TestCategory("Null")]
        public void CreateNull() {
            object? val = null;
            var indirect = DBValue.Create(val);

            Assert.AreEqual(DBValue.NULL, indirect);
        }

        [TestMethod, TestCategory("Null")]
        public void NullSentinelIsNotNullLiteral() {
            Assert.IsNotNull(DBValue.NULL.Datum);
        }

        [TestMethod, TestCategory("Equality")]
        public void StrongEquality() {
            // Scenario #1: LHS and RHS are constructed identically
            var lhs1 = DBValue.Create(-781);
            var rhs1 = DBValue.Create(-781);
            FullCheck.ExpectEqual(lhs1, rhs1);

            // Scenario #2: LHS and RHS are constructed differently but nonetheless equal
            var lhs2 = DBValue.Create("Aaron Rodgers");
            var rhs2 = new DBValue("Aaron Rodgers");
            FullCheck.ExpectEqual(lhs2, rhs2);

            // Scenario #3: LHS and RHS have the same numeric value but the types are different
            var lhs3 = new DBValue(0);
            var rhs3 = new DBValue(0L);
            FullCheck.ExpectUnequal(lhs3, rhs3);

            // Scenario #4: LHS and RHS contain completely different values
            var lhs4 = new DBValue("Pikachu");
            var rhs4 = new DBValue(false);
            FullCheck.ExpectUnequal(lhs4, rhs4);

            // Scenario #5: Exactly one of LHS and RHS is a null DBValue?
            var lhs5 = new DBValue('=');
            DBValue? rhs5 = null;
            FullCheck.ExpectUnequal(lhs5, rhs5);

            // Scenario #6: Both LHS and RHS are null DBValue?
            DBValue? lhs6 = null;
            DBValue? rhs6 = null;
            FullCheck.ExpectEqual(lhs6, rhs6);
        }

        [TestMethod, TestCategory("Equality")]
        public void WeakEquality() {
            // Scenario #1: Exactly one of LHS and RHS is not a DBValue
            var lhs1 = new DBValue('w');
            var rhs1 = 'w';
            FullCheck.ExpectUnequal<object>(lhs1, rhs1);

            // Scenario #2: Exactly one of LHS and RHS is null object?
            var lhs2 = new DBValue(919M);
            object? rhs2 = null;
            FullCheck.ExpectUnequal(lhs2, rhs2);
        }

        [TestMethod, TestCategory("ToString")]
        public void StringifyNumeric() {
            var integer = -794;
            var integerDBV = DBValue.Create(integer);
            Assert.AreEqual(integer.ToString(), integerDBV.ToString());

            var floatingPoint = 451.992;
            var floatingPointDBV = DBValue.Create(floatingPoint);
            Assert.AreEqual(floatingPoint.ToString(), floatingPointDBV.ToString());
        }

        [TestMethod, TestCategory("ToString")]
        public void StringifyDateTime() {
            var datetime = new DateTime(2016, 11, 03);
            var datetimeDBV = DBValue.Create(datetime);
            Assert.AreEqual(datetime.ToString(), datetimeDBV.ToString());
        }

        [TestMethod, TestCategory("ToString")]
        public void StringifyText() {
            var character = '&';
            var characterDBV = DBValue.Create(character);
            Assert.AreEqual($"'{character}'", characterDBV.ToString());

            var text = "President Abraham Lincoln";
            var textDBV = DBValue.Create(text);
            Assert.AreEqual($"\"{text}\"", textDBV.ToString());

            var guid = Guid.NewGuid();
            var guidDBV = DBValue.Create(guid);
            Assert.AreEqual(guid.ToString(), guidDBV.ToString());
        }

        [TestMethod, TestCategory("ToString")]
        public void StringifyNull() {
            var nullDBV = DBValue.Create(null);
            Assert.AreEqual("NULL", nullDBV.ToString());
        }

        [TestMethod, TestCategory("Errors")]
        public void CreateEnumerationType() {
            var value = PlatformID.Unix;
            
            void action() => DBValue.Create(value);
            var exception = Assert.ThrowsException<ArgumentException>(action);
            Assert.AreNotEqual(string.Empty, exception.Message);
        }

        [TestMethod, TestCategory("Errors")]
        public void CreateComplexType() {
            var value = new Exception();

            void action() => DBValue.Create(value);
            var exception = Assert.ThrowsException<ArgumentException>(action);
            Assert.AreNotEqual(string.Empty, exception.Message);
        }
    }
}
