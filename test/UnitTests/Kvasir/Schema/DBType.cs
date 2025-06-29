using Atropos;
using FluentAssertions;
using Kvasir.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace UT.Kvasir.Schema {
    [TestClass, TestCategory("DBType")]
    public class DBTypeTests {
        [TestMethod] public void DefaultIsInt32() {
            // Arrange
            var defaulted = new DBType();

            // No Action
            // Assert
            defaulted.Should().Be(DBType.Int32);
        }

        [TestMethod] public void LookupBool() {
            // Arrange
            var type = typeof(bool);

            // Act
            var supported = DBType.IsSupported(type);
            var dbType = DBType.Lookup(type);

            // Assert
            supported.Should().BeTrue();
            dbType.Should().Be(DBType.Boolean);
        }

        [TestMethod] public void LookupChar() {
            // Arrange
            var type = typeof(char);

            // Act
            var supported = DBType.IsSupported(type);
            var dbType = DBType.Lookup(type);

            // Assert
            supported.Should().BeTrue();
            dbType.Should().Be(DBType.Character);
        }

        [TestMethod] public void LookupInt() {
            // Arrange
            var type = typeof(int);

            // Act
            var supported = DBType.IsSupported(type);
            var dbType = DBType.Lookup(type);

            // Assert
            supported.Should().BeTrue();
            dbType.Should().Be(DBType.Int32);
        }

        [TestMethod] public void LookupUInt() {
            // Arrange
            var type = typeof(uint);

            // Act
            var supported = DBType.IsSupported(type);
            var dbType = DBType.Lookup(type);

            // Assert
            supported.Should().BeTrue();
            dbType.Should().Be(DBType.UInt32);
        }

        [TestMethod] public void LookupShort() {
            // Arrange
            var type = typeof(short);

            // Act
            var supported = DBType.IsSupported(type);
            var dbType = DBType.Lookup(type);

            // Assert
            supported.Should().BeTrue();
            dbType.Should().Be(DBType.Int16);
        }

        [TestMethod] public void LookupUShort() {
            // Arrange
            var type = typeof(ushort);

            // Act
            var supported = DBType.IsSupported(type);
            var dbType = DBType.Lookup(type);

            // Assert
            supported.Should().BeTrue();
            dbType.Should().Be(DBType.UInt16);
        }

        [TestMethod] public void LookupLong() {
            // Arrange
            var type = typeof(long);

            // Act
            var supported = DBType.IsSupported(type);
            var dbType = DBType.Lookup(type);

            // Assert
            supported.Should().BeTrue();
            dbType.Should().Be(DBType.Int64);
        }

        [TestMethod] public void LookupULong() {
            // Arrange
            var type = typeof(ulong);

            // Act
            var supported = DBType.IsSupported(type);
            var dbType = DBType.Lookup(type);

            // Assert
            supported.Should().BeTrue();
            dbType.Should().Be(DBType.UInt64);
        }

        [TestMethod] public void LookupByte() {
            // Arrange
            var type = typeof(byte);

            // Act
            var supported = DBType.IsSupported(type);
            var dbType = DBType.Lookup(type);

            // Assert
            supported.Should().BeTrue();
            dbType.Should().Be(DBType.UInt8);
        }

        [TestMethod] public void LookupSbyte() {
            // Arrange
            var type = typeof(sbyte);

            // Act
            var supported = DBType.IsSupported(type);
            var dbType = DBType.Lookup(type);

            // Assert
            supported.Should().BeTrue();
            dbType.Should().Be(DBType.Int8);
        }

        [TestMethod] public void LookupDateOnly() {
            // Arrange
            var type = typeof(DateOnly);

            // Act
            var supported = DBType.IsSupported(type);
            var dbType = DBType.Lookup(type);

            // Assert
            supported.Should().BeTrue();
            dbType.Should().Be(DBType.Date);
        }

        [TestMethod] public void LookupDateTime() {
            // Arrange
            var type = typeof(DateTime);

            // Act
            var supported = DBType.IsSupported(type);
            var dbType = DBType.Lookup(type);

            // Assert
            supported.Should().BeTrue();
            dbType.Should().Be(DBType.DateTime);
        }

        [TestMethod] public void LookupGuid() {
            // Arrange
            var type = typeof(Guid);

            // Act
            var supported = DBType.IsSupported(type);
            var dbType = DBType.Lookup(type);

            // Assert
            supported.Should().BeTrue();
            dbType.Should().Be(DBType.Guid);
        }

        [TestMethod] public void LookupString() {
            // Arrange
            var type = typeof(string);

            // Act
            var supported = DBType.IsSupported(type);
            var dbType = DBType.Lookup(type);

            // Assert
            supported.Should().BeTrue();
            dbType.Should().Be(DBType.Text);
        }

        [TestMethod] public void LookupDouble() {
            // Arrange
            var type = typeof(double);

            // Act
            var supported = DBType.IsSupported(type);
            var dbType = DBType.Lookup(type);

            // Assert
            supported.Should().BeTrue();
            dbType.Should().Be(DBType.Double);
        }

        [TestMethod] public void LookupFloat() {
            // Arrange
            var type = typeof(float);

            // Act
            var supported = DBType.IsSupported(type);
            var dbType = DBType.Lookup(type);

            // Assert
            supported.Should().BeTrue();
            dbType.Should().Be(DBType.Single);
        }

        [TestMethod] public void LookupDecimal() {
            // Arrange
            var type = typeof(decimal);

            // Act
            var supported = DBType.IsSupported(type);
            var dbType = DBType.Lookup(type);

            // Assert
            supported.Should().BeTrue();
            dbType.Should().Be(DBType.Decimal);
        }

        [TestMethod] public void LookupEnum() {
            // Arrange
            var type = typeof(DataAccessMethod);

            // Act
            var supported = DBType.IsSupported(type);
            var dbType = DBType.Lookup(type);

            // Assert
            supported.Should().BeTrue();
            dbType.Should().Be(DBType.Enumeration);
        }

        [TestMethod] public void LookupNullableBool() {
            // Arrange
            var type = typeof(bool?);

            // Act
            var supported = DBType.IsSupported(type);
            var dbType = DBType.Lookup(type);

            // Assert
            supported.Should().BeTrue();
            dbType.Should().Be(DBType.Boolean);
        }

        [TestMethod] public void LookupNullableChar() {
            // Arrange
            var type = typeof(char?);

            // Act
            var supported = DBType.IsSupported(type);
            var dbType = DBType.Lookup(type);

            // Assert
            supported.Should().BeTrue();
            dbType.Should().Be(DBType.Character);
        }

        [TestMethod] public void LookupNullableInt() {
            // Arrange
            var type = typeof(int?);

            // Act
            var supported = DBType.IsSupported(type);
            var dbType = DBType.Lookup(type);

            // Assert
            supported.Should().BeTrue();
            dbType.Should().Be(DBType.Int32);
        }

        [TestMethod] public void LookupNullableUInt() {
            // Arrange
            var type = typeof(uint?);

            // Act
            var supported = DBType.IsSupported(type);
            var dbType = DBType.Lookup(type);

            // Assert
            supported.Should().BeTrue();
            dbType.Should().Be(DBType.UInt32);
        }

        [TestMethod] public void LookupNullableShort() {
            // Arrange
            var type = typeof(short?);

            // Act
            var supported = DBType.IsSupported(type);
            var dbType = DBType.Lookup(type);

            // Assert
            supported.Should().BeTrue();
            dbType.Should().Be(DBType.Int16);
        }

        [TestMethod] public void LookupNullableUShort() {
            // Arrange
            var type = typeof(ushort?);

            // Act
            var supported = DBType.IsSupported(type);
            var dbType = DBType.Lookup(type);

            // Assert
            supported.Should().BeTrue();
            dbType.Should().Be(DBType.UInt16);
        }

        [TestMethod] public void LookupNullableLong() {
            // Arrange
            var type = typeof(long?);

            // Act
            var supported = DBType.IsSupported(type);
            var dbType = DBType.Lookup(type);

            // Assert
            supported.Should().BeTrue();
            dbType.Should().Be(DBType.Int64);
        }

        [TestMethod] public void LookupNullableULong() {
            // Arrange
            var type = typeof(ulong?);

            // Act
            var supported = DBType.IsSupported(type);
            var dbType = DBType.Lookup(type);

            // Assert
            supported.Should().BeTrue();
            dbType.Should().Be(DBType.UInt64);
        }

        [TestMethod] public void LookupNullableByte() {
            // Arrange
            var type = typeof(byte?);

            // Act
            var supported = DBType.IsSupported(type);
            var dbType = DBType.Lookup(type);

            // Assert
            supported.Should().BeTrue();
            dbType.Should().Be(DBType.UInt8);
        }

        [TestMethod] public void LookupNullableSbyte() {
            // Arrange
            var type = typeof(sbyte?);

            // Act
            var supported = DBType.IsSupported(type);
            var dbType = DBType.Lookup(type);

            // Assert
            supported.Should().BeTrue();
            dbType.Should().Be(DBType.Int8);
        }

        [TestMethod] public void LookupNullableDate() {
            // Arrange
            var type = typeof(DateOnly?);

            // Act
            var supported = DBType.IsSupported(type);
            var dbType = DBType.Lookup(type);

            // Assert
            supported.Should().BeTrue();
            dbType.Should().Be(DBType.Date);
        }

        [TestMethod] public void LookupNullableDateTime() {
            // Arrange
            var type = typeof(DateTime?);

            // Act
            var supported = DBType.IsSupported(type);
            var dbType = DBType.Lookup(type);

            // Assert
            supported.Should().BeTrue();
            dbType.Should().Be(DBType.DateTime);
        }

        [TestMethod] public void LookupNullableGuid() {
            // Arrange
            var type = typeof(Guid?);

            // Act
            var supported = DBType.IsSupported(type);
            var dbType = DBType.Lookup(type);

            // Assert
            supported.Should().BeTrue();
            dbType.Should().Be(DBType.Guid);
        }

        [TestMethod] public void LookupNullableDouble() {
            // Arrange
            var type = typeof(double?);

            // Act
            var supported = DBType.IsSupported(type);
            var dbType = DBType.Lookup(type);

            // Assert
            supported.Should().BeTrue();
            dbType.Should().Be(DBType.Double);
        }

        [TestMethod] public void LookupNullableFloat() {
            // Arrange
            var type = typeof(float?);

            // Act
            var supported = DBType.IsSupported(type);
            var dbType = DBType.Lookup(type);

            // Assert
            supported.Should().BeTrue();
            dbType.Should().Be(DBType.Single);
        }

        [TestMethod] public void LookupNullableDecimal() {
            // Arrange
            var type = typeof(decimal?);

            // Act
            var supported = DBType.IsSupported(type);
            var dbType = DBType.Lookup(type);

            // Assert
            supported.Should().BeTrue();
            dbType.Should().Be(DBType.Decimal);
        }

        [TestMethod] public void LookupNullableEnum() {
            // Arrange
            var type = typeof(DataAccessMethod?);

            // Act
            var supported = DBType.IsSupported(type);
            var dbType = DBType.Lookup(type);

            // Assert
            supported.Should().BeTrue();
            dbType.Should().Be(DBType.Enumeration);
        }

        [TestMethod] public void LookupException() {
            // Arrange
            var type = typeof(Exception);

            // Act
            var supported = DBType.IsSupported(type);
            Func<DBType> action = () => DBType.Lookup(type);

            // Assert
            supported.Should().BeFalse();
            action.Should().ThrowExactly<ArgumentException>()
                .WithAnyMessage()
                .And
                .ParamName.Should().NotBeNullOrEmpty();
        }

        [TestMethod] public void LookupArray() {
            // Arrange
            var type = typeof(int[]);

            // Act
            var supported = DBType.IsSupported(type);
            Func<DBType> action = () => DBType.Lookup(type);

            // Assert
            supported.Should().BeFalse();
            action.Should().ThrowExactly<ArgumentException>()
                .WithAnyMessage()
                .And
                .ParamName.Should().NotBeNullOrEmpty();
        }

        [TestMethod] public void LookupCollection() {
            // Arrange
            var type = typeof(List<string>);

            // Act
            var supported = DBType.IsSupported(type);
            Func<DBType> action = () => DBType.Lookup(type);

            // Assert
            supported.Should().BeFalse();
            action.Should().ThrowExactly<ArgumentException>()
                .WithAnyMessage()
                .And
                .ParamName.Should().NotBeNullOrEmpty();
        }

        [TestMethod] public void StrongEquality() {
            // Arrange
            var textType = DBType.Text;
            var longType = DBType.Int64;
            DBType? nullType = null;

            // Act
            var areEqual = FullCheck.ExpectEqual(textType, textType);
            var areNotEqual = FullCheck.ExpectNotEqual(textType, longType);
            var nullAreEqual = FullCheck.ExpectEqual(nullType, nullType);
            var nullAreNotEqual = FullCheck.ExpectNotEqual(longType, nullType);

            // Assert
            areEqual.Should().NotHaveValue();
            areNotEqual.Should().NotHaveValue();
            nullAreEqual.Should().NotHaveValue();
            nullAreNotEqual.Should().NotHaveValue();
        }

        [TestMethod] public void WeakEquality() {
            // Arrange
            var byteType = DBType.UInt8;
            var nonDbType = typeof(byte);
            object? nll = null;

            // Act
            var areNotEqual = FullCheck.ExpectNotEqual<object>(byteType, nonDbType);
            var nullAreNotEqual = FullCheck.ExpectNotEqual(byteType, nll);

            // Assert
            areNotEqual.Should().NotHaveValue();
            nullAreNotEqual.Should().NotHaveValue();
        }

        [TestMethod] public void Stringification() {
            // Arrange

            // Act
            var boolStr = DBType.Boolean.ToString();
            var byteStr = DBType.UInt8.ToString();
            var charStr = DBType.Character.ToString();
            var dateStr = DBType.Date.ToString();
            var datetimeStr = DBType.DateTime.ToString();
            var decimalStr = DBType.Decimal.ToString();
            var doubleStr = DBType.Double.ToString();
            var enumStr = DBType.Enumeration.ToString();
            var floatStr = DBType.Single.ToString();
            var guidStr = DBType.Guid.ToString();
            var intStr = DBType.Int32.ToString();
            var longStr = DBType.Int64.ToString();
            var sbyteStr = DBType.Int8.ToString();
            var shortStr = DBType.Int16.ToString();
            var textStr = DBType.Text.ToString();
            var uintStr = DBType.UInt32.ToString();
            var ulongStr = DBType.UInt64.ToString();
            var ushortStr = DBType.UInt16.ToString();

            // Assert
            boolStr.Should().Be("Boolean");
            byteStr.Should().Be("UInt8");
            charStr.Should().Be("Character");
            dateStr.Should().Be("Date");
            datetimeStr.Should().Be("Date/Time");
            decimalStr.Should().Be("Decimal");
            doubleStr.Should().Be("Double");
            enumStr.Should().Be("Enumeration");
            floatStr.Should().Be("Single");
            guidStr.Should().Be("GUID");
            intStr.Should().Be("Int32");
            longStr.Should().Be("Int64");
            sbyteStr.Should().Be("Int8");
            shortStr.Should().Be("Int16");
            textStr.Should().Be("Text");
            uintStr.Should().Be("UInt32");
            ulongStr.Should().Be("UInt64");
            ushortStr.Should().Be("UInt16");
        }
    }
}
