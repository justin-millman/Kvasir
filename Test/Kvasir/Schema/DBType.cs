using Kvasir.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Test.Kvasir.Schema {
    [TestClass]
    public class DBTypeTests {
        [TestMethod, TestCategory("Default")]
        public void DefaultConstruct() {
            var defaulted = new DBType();

            Assert.AreEqual(DBType.Int32, defaulted);
        }

        [TestMethod, TestCategory("Boolean")]
        public void LookupBoolean() {
            var type = typeof(bool);
            var nullableType = typeof(bool?);
            Assert.IsTrue(DBType.IsSupported(type));
            Assert.IsTrue(DBType.IsSupported(nullableType));

            var dbtype0 = DBType.Lookup(type);
            var dbtype1 = DBType.Lookup(nullableType);
            Assert.AreEqual(DBType.Boolean, dbtype0);
            Assert.AreEqual(DBType.Boolean, dbtype1);
        }

        [TestMethod, TestCategory("Integer")]
        public void LookupByte() {
            var type = typeof(byte);
            var nullableType = typeof(byte?);
            Assert.IsTrue(DBType.IsSupported(type));
            Assert.IsTrue(DBType.IsSupported(nullableType));

            var dbtype0 = DBType.Lookup(type);
            var dbtype1 = DBType.Lookup(nullableType);
            Assert.AreEqual(DBType.UInt8, dbtype0);
            Assert.AreEqual(DBType.UInt8, dbtype1);
        }

        [TestMethod, TestCategory("Integer")]
        public void LookupSByte() {
            var type = typeof(sbyte);
            var nullableType = typeof(sbyte?);
            Assert.IsTrue(DBType.IsSupported(type));
            Assert.IsTrue(DBType.IsSupported(nullableType));

            var dbtype0 = DBType.Lookup(type);
            var dbtype1 = DBType.Lookup(nullableType);
            Assert.AreEqual(DBType.Int8, dbtype0);
            Assert.AreEqual(DBType.Int8, dbtype1);
        }

        [TestMethod, TestCategory("Integer")]
        public void LookupUShort() {
            var type = typeof(ushort);
            var nullableType = typeof(ushort?);
            Assert.IsTrue(DBType.IsSupported(type));
            Assert.IsTrue(DBType.IsSupported(nullableType));

            var dbtype0 = DBType.Lookup(type);
            var dbtype1 = DBType.Lookup(nullableType);
            Assert.AreEqual(DBType.UInt16, dbtype0);
            Assert.AreEqual(DBType.UInt16, dbtype1);
        }

        [TestMethod, TestCategory("Integer")]
        public void LookupShort() {
            var type = typeof(short);
            var nullableType = typeof(short?);
            Assert.IsTrue(DBType.IsSupported(type));
            Assert.IsTrue(DBType.IsSupported(nullableType));

            var dbtype0 = DBType.Lookup(type);
            var dbtype1 = DBType.Lookup(nullableType);
            Assert.AreEqual(DBType.Int16, dbtype0);
            Assert.AreEqual(DBType.Int16, dbtype1);
        }

        [TestMethod, TestCategory("Integer")]
        public void LookupUInt() {
            var type = typeof(uint);
            var nullableType = typeof(uint?);
            Assert.IsTrue(DBType.IsSupported(type));
            Assert.IsTrue(DBType.IsSupported(nullableType));

            var dbtype0 = DBType.Lookup(type);
            var dbtype1 = DBType.Lookup(nullableType);
            Assert.AreEqual(DBType.UInt32, dbtype0);
            Assert.AreEqual(DBType.UInt32, dbtype1);
        }

        [TestMethod, TestCategory("Integer")]
        public void LookupInt() {
            var type = typeof(int);
            var nullableType = typeof(int?);
            Assert.IsTrue(DBType.IsSupported(type));
            Assert.IsTrue(DBType.IsSupported(nullableType));

            var dbtype0 = DBType.Lookup(type);
            var dbtype1 = DBType.Lookup(nullableType);
            Assert.AreEqual(DBType.Int32, dbtype0);
            Assert.AreEqual(DBType.Int32, dbtype1);
        }

        [TestMethod, TestCategory("Integer")]
        public void LookupULong() {
            var type = typeof(ulong);
            var nullableType = typeof(ulong?);
            Assert.IsTrue(DBType.IsSupported(type));
            Assert.IsTrue(DBType.IsSupported(nullableType));

            var dbtype0 = DBType.Lookup(type);
            var dbtype1 = DBType.Lookup(nullableType);
            Assert.AreEqual(DBType.UInt64, dbtype0);
            Assert.AreEqual(DBType.UInt64, dbtype1);
        }

        [TestMethod, TestCategory("Integer")]
        public void LookupLong() {
            var type = typeof(long);
            var nullableType = typeof(long?);
            Assert.IsTrue(DBType.IsSupported(type));
            Assert.IsTrue(DBType.IsSupported(nullableType));

            var dbtype0 = DBType.Lookup(type);
            var dbtype1 = DBType.Lookup(nullableType);
            Assert.AreEqual(DBType.Int64, dbtype0);
            Assert.AreEqual(DBType.Int64, dbtype1);
        }

        [TestMethod, TestCategory("Floating Point")]
        public void LookupFloat() {
            var type = typeof(float);
            var nullableType = typeof(float?);
            Assert.IsTrue(DBType.IsSupported(type));
            Assert.IsTrue(DBType.IsSupported(nullableType));

            var dbtype0 = DBType.Lookup(type);
            var dbtype1 = DBType.Lookup(nullableType);
            Assert.AreEqual(DBType.Single, dbtype0);
            Assert.AreEqual(DBType.Single, dbtype1);
        }

        [TestMethod, TestCategory("Floating Point")]
        public void LookupDouble() {
            var type = typeof(double);
            var nullableType = typeof(double?);
            Assert.IsTrue(DBType.IsSupported(type));
            Assert.IsTrue(DBType.IsSupported(nullableType));

            var dbtype0 = DBType.Lookup(type);
            var dbtype1 = DBType.Lookup(nullableType);
            Assert.AreEqual(DBType.Double, dbtype0);
            Assert.AreEqual(DBType.Double, dbtype1);
        }

        [TestMethod, TestCategory("Floating Point")]
        public void LookupDecimal() {
            var type = typeof(decimal);
            var nullableType = typeof(decimal?);
            Assert.IsTrue(DBType.IsSupported(type));
            Assert.IsTrue(DBType.IsSupported(nullableType));

            var dbtype0 = DBType.Lookup(type);
            var dbtype1 = DBType.Lookup(nullableType);
            Assert.AreEqual(DBType.Decimal, dbtype0);
            Assert.AreEqual(DBType.Decimal, dbtype1);
        }

        [TestMethod, TestCategory("Enumeration")]
        public void LookupRegularEnum() {
            var type = typeof(ConsoleSpecialKey);
            var nullableType = typeof(StringSplitOptions?);
            Assert.IsTrue(DBType.IsSupported(type));
            Assert.IsTrue(DBType.IsSupported(nullableType));

            var dbtype0 = DBType.Lookup(type);
            var dbtype1 = DBType.Lookup(nullableType);
            Assert.AreEqual(DBType.Enumeration, dbtype0);
            Assert.AreEqual(DBType.Enumeration, dbtype1);
        }

        [TestMethod, TestCategory("Enumeration")]
        public void LookupFlagEnumeration() {
            var type = typeof(AttributeTargets);
            var nullableType = typeof(ConsoleModifiers?);
            Assert.IsTrue(DBType.IsSupported(type));
            Assert.IsTrue(DBType.IsSupported(nullableType));

            var dbtype0 = DBType.Lookup(type);
            var dbtype1 = DBType.Lookup(nullableType);
            Assert.AreEqual(DBType.Enumeration, dbtype0);
            Assert.AreEqual(DBType.Enumeration, dbtype1);
        }

        [TestMethod, TestCategory("Time")]
        public void LookupDateTime() {
            var type = typeof(DateTime);
            var nullableType = typeof(DateTime?);
            Assert.IsTrue(DBType.IsSupported(type));
            Assert.IsTrue(DBType.IsSupported(nullableType));

            var dbtype0 = DBType.Lookup(type);
            var dbtype1 = DBType.Lookup(nullableType);
            Assert.AreEqual(DBType.DateTime, dbtype0);
            Assert.AreEqual(DBType.DateTime, dbtype1);
        }

        [TestMethod, TestCategory("Text")]
        public void LookupChar() {
            var type = typeof(char);
            var nullableType = typeof(char?);
            Assert.IsTrue(DBType.IsSupported(type));
            Assert.IsTrue(DBType.IsSupported(nullableType));

            var dbtype0 = DBType.Lookup(type);
            var dbtype1 = DBType.Lookup(nullableType);
            Assert.AreEqual(DBType.Character, dbtype0);
            Assert.AreEqual(DBType.Character, dbtype1);
        }

        [TestMethod, TestCategory("Text")]
        public void LookupString() {
            var type = typeof(string);
            Assert.IsTrue(DBType.IsSupported(type));

            var dbtype = DBType.Lookup(type);
            Assert.AreEqual(DBType.Text, dbtype);
        }

        [TestMethod, TestCategory("Text")]
        public void LookupGuid() {
            var type = typeof(Guid);
            var nullableType = typeof(Guid?);
            Assert.IsTrue(DBType.IsSupported(type));
            Assert.IsTrue(DBType.IsSupported(nullableType));

            var dbtype0 = DBType.Lookup(type);
            var dbtype1 = DBType.Lookup(nullableType);
            Assert.AreEqual(DBType.Guid, dbtype0);
            Assert.AreEqual(DBType.Guid, dbtype1);
        }

        [TestMethod, TestCategory("IsValidValue")]
        public void NullIsAlwaysValid() {
            foreach (var type in ALL_DBTYPES) {
                Assert.IsTrue(type.IsValidValue(DBValue.NULL));
            }
        }

        [TestMethod, TestCategory("IsValidValue")]
        public void ValueIsValid() {
            Assert.IsTrue(DBType.Boolean.IsValidValue(DBValue.Create(true)));
            Assert.IsTrue(DBType.Character.IsValidValue(DBValue.Create('h')));
            Assert.IsTrue(DBType.DateTime.IsValidValue(DBValue.Create(new DateTime(1865, 04, 15))));
            Assert.IsTrue(DBType.Decimal.IsValidValue(DBValue.Create(91286.12M)));
            Assert.IsTrue(DBType.Double.IsValidValue(DBValue.Create(-0.182831)));
            Assert.IsTrue(DBType.Guid.IsValidValue(DBValue.Create(Guid.NewGuid())));
            Assert.IsTrue(DBType.Int16.IsValidValue(DBValue.Create((short)12)));
            Assert.IsTrue(DBType.Int32.IsValidValue(DBValue.Create(-88427)));
            Assert.IsTrue(DBType.Int64.IsValidValue(DBValue.Create(12L)));
            Assert.IsTrue(DBType.Int8.IsValidValue(DBValue.Create((sbyte)44)));
            Assert.IsTrue(DBType.Single.IsValidValue(DBValue.Create(763.5f)));
            Assert.IsTrue(DBType.Text.IsValidValue(DBValue.Create("Tegucigalpa, Honduras")));
            Assert.IsTrue(DBType.UInt16.IsValidValue(DBValue.Create((ushort)0)));
            Assert.IsTrue(DBType.UInt32.IsValidValue(DBValue.Create(662u)));
            Assert.IsTrue(DBType.UInt64.IsValidValue(DBValue.Create(47810UL)));
            Assert.IsTrue(DBType.UInt8.IsValidValue(DBValue.Create((byte)1)));
        }

        [TestMethod, TestCategory("IsValidValue")]
        public void ValueIsNotValid() {
            Assert.IsFalse(DBType.Boolean.IsValidValue(DBValue.Create('^')));
            Assert.IsFalse(DBType.Character.IsValidValue(DBValue.Create(false)));
            Assert.IsFalse(DBType.DateTime.IsValidValue(DBValue.Create(-66.0M)));
            Assert.IsFalse(DBType.Decimal.IsValidValue(DBValue.Create(new DateTime(2020, 11, 08))));
            Assert.IsFalse(DBType.Double.IsValidValue(DBValue.Create(Guid.NewGuid())));
            Assert.IsFalse(DBType.Guid.IsValidValue(DBValue.Create(-71.3)));
            Assert.IsFalse(DBType.Int16.IsValidValue(DBValue.Create(40909)));
            Assert.IsFalse(DBType.Int32.IsValidValue(DBValue.Create((short)0)));
            Assert.IsFalse(DBType.Int64.IsValidValue(DBValue.Create((sbyte)123)));
            Assert.IsFalse(DBType.Int8.IsValidValue(DBValue.Create(-64123912L)));
            Assert.IsFalse(DBType.Single.IsValidValue(DBValue.Create("Beowulf")));
            Assert.IsFalse(DBType.Text.IsValidValue(DBValue.Create(471.111f)));
            Assert.IsFalse(DBType.UInt16.IsValidValue(DBValue.Create(0u)));
            Assert.IsFalse(DBType.UInt32.IsValidValue(DBValue.Create((ushort)19)));
            Assert.IsFalse(DBType.UInt64.IsValidValue(DBValue.Create((byte)53)));
            Assert.IsFalse(DBType.UInt8.IsValidValue(DBValue.Create(8281727391023UL)));
        }

        [TestMethod, TestCategory("IsValidValue")]
        public void EnumerationExpectsStringValue() {
            Assert.IsTrue(DBType.Enumeration.IsValidValue(DBValue.Create("President Josiah Bartlet")));
            Assert.IsFalse(DBType.Enumeration.IsValidValue(DBValue.Create(182)));
        }

        [TestMethod, TestCategory("Equality")]
        public void StrongEquality() {
            // Scenario #1: LHS and RHS are the same named instance
            foreach (var type in ALL_DBTYPES) {
                FullCheck.ExpectEqual(type, type);
            }

            // Scenario #2: LHS and RHS are different named instances
            for (int i = 0; i < ALL_DBTYPES.Length; ++i) {
                for (int j = i + 1; j < ALL_DBTYPES.Length; ++j) {
                    FullCheck.ExpectNotEqual(ALL_DBTYPES[i], ALL_DBTYPES[j]);
                }
            }

            // Scenario #3: Exactly one of LHS and RHS is a null DBType?
            var lhs3 = DBType.Double;
            DBType? rhs3 = null;
            FullCheck.ExpectNotEqual(lhs3, rhs3);

            // Scenario #4: Both LHS and RHS are null DBType?
            DBType? lhs4 = null;
            DBType? rhs4 = null;
            FullCheck.ExpectEqual(lhs4, rhs4);
        }

        [TestMethod, TestCategory("Equality")]
        public void WeakEquality() {
            // Scenario #1: Exactly one of LHS and RHS is not a DBType
            var lhs1 = DBType.Enumeration;
            var rhs1 = "Drosophila melanogaster";
            FullCheck.ExpectNotEqual<object>(lhs1, rhs1);

            // Scenario #2: Exactly one of LHS and RHS is a null object?
            var lhs2 = DBType.UInt32;
            object? rhs2 = null;
            FullCheck.ExpectNotEqual<object?>(lhs2, rhs2);
        }

        [TestMethod, TestCategory("Errors")]
        public void LookupUnsupportedType() {
            var unsupportedType = typeof(Exception);
            Assert.IsFalse(DBType.IsSupported(unsupportedType));

            void action() => DBType.Lookup(unsupportedType);
            var exception = Assert.ThrowsException<ArgumentException>(action);
            Assert.AreNotEqual(string.Empty, exception.Message);
        }


        static DBTypeTests() {
            ALL_DBTYPES = new DBType[] {
                DBType.Boolean, DBType.Character, DBType.DateTime, DBType.Decimal, DBType.Double, DBType.Enumeration,
                DBType.Guid, DBType.Int16, DBType.Int32, DBType.Int64, DBType.Int8, DBType.Single, DBType.Text,
                DBType.UInt16, DBType.UInt32, DBType.UInt64, DBType.UInt8
            };
        }

        private static readonly DBType[] ALL_DBTYPES;
    }
}
