using Atropos;
using FluentAssertions;
using Kvasir.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace UT.Kvasir.Schema {
    [TestClass, TestCategory("DBValue")]
    public class DBValueTests {
        [TestMethod] public void CreateFromBoolDirectly() {
            // Arrange
            var rawValue = false;
            var expectedType = DBType.Boolean;

            // Act
            var value = new DBValue(rawValue);
            var matches = value.IsInstanceOf(expectedType);

            // Assert
            value.Datum.Should().Be(rawValue);
            matches.Should().BeTrue();
        }

        [TestMethod] public void CreateFromCharDirectly() {
            // Arrange
            var rawValue = '%';
            var expectedType = DBType.Character;

            // Act
            var value = new DBValue(rawValue);
            var matches = value.IsInstanceOf(expectedType);

            // Assert
            value.Datum.Should().Be(rawValue);
            matches.Should().BeTrue();
        }

        [TestMethod] public void CreateFromIntDirectly() {
            // Arrange
            var rawValue = -8319;
            var expectedType = DBType.Int32;

            // Act
            var value = new DBValue(rawValue);
            var matches = value.IsInstanceOf(expectedType);

            // Assert
            value.Datum.Should().Be(rawValue);
            matches.Should().BeTrue();
        }

        [TestMethod] public void CreateFromUIntDirectly() {
            // Arrange
            var rawValue = 9182u;
            var expectedType = DBType.UInt32;

            // Act
            var value = new DBValue(rawValue);
            var matches = value.IsInstanceOf(expectedType);

            // Assert
            value.Datum.Should().Be(rawValue);
            matches.Should().BeTrue();
        }

        [TestMethod] public void CreateFromShortDirectly() {
            // Arrange
            var rawValue = (short)0;
            var expectedType = DBType.Int16;

            // Act
            var value = new DBValue(rawValue);
            var matches = value.IsInstanceOf(expectedType);

            // Assert
            value.Datum.Should().Be(rawValue);
            matches.Should().BeTrue();
        }

        [TestMethod] public void CreateFromUShortDirectly() {
            // Arrange
            var rawValue = (ushort)102;
            var expectedType = DBType.UInt16;

            // Act
            var value = new DBValue(rawValue);
            var matches = value.IsInstanceOf(expectedType);

            // Assert
            value.Datum.Should().Be(rawValue);
            matches.Should().BeTrue();
        }

        [TestMethod] public void CreateFromLongDirectly() {
            // Arrange
            var rawValue = 499182371L;
            var expectedType = DBType.Int64;

            // Act
            var value = new DBValue(rawValue);
            var matches = value.IsInstanceOf(expectedType);

            // Assert
            value.Datum.Should().Be(rawValue);
            matches.Should().BeTrue();
        }

        [TestMethod] public void CreateFromULongDirectly() {
            // Arrange
            var rawValue = 111110108ul;
            var expectedType = DBType.UInt64;

            // Act
            var value = new DBValue(rawValue);
            var matches = value.IsInstanceOf(expectedType);

            // Assert
            value.Datum.Should().Be(rawValue);
            matches.Should().BeTrue();
        }

        [TestMethod] public void CreateFromByteDirectly() {
            // Arrange
            var rawValue = (byte)77;
            var expectedType = DBType.UInt8;

            // Act
            var value = new DBValue(rawValue);
            var matches = value.IsInstanceOf(expectedType);

            // Assert
            value.Datum.Should().Be(rawValue);
            matches.Should().BeTrue();
        }

        [TestMethod] public void CreateFromSbyteDirectly() {
            // Arrange
            var rawValue = (sbyte)4;
            var expectedType = DBType.Int8;

            // Act
            var value = new DBValue(rawValue);
            var matches = value.IsInstanceOf(expectedType);

            // Assert
            value.Datum.Should().Be(rawValue);
            matches.Should().BeTrue();
        }

        [TestMethod] public void CreateFromDateTimeDirectly() {
            // Arrange
            var rawValue = new DateTime(1996, 02, 29);
            var expectedType = DBType.DateTime;

            // Act
            var value = new DBValue(rawValue);
            var matches = value.IsInstanceOf(expectedType);

            // Assert
            value.Datum.Should().Be(rawValue);
            matches.Should().BeTrue();
        }

        [TestMethod] public void CreateFromGuidDirectly() {
            // Arrange
            var rawValue = new Guid(0, 1, 1, new byte[] { 2, 3, 5, 8, 13, 21, 34, 55 });
            var expectedType = DBType.Guid;

            // Act
            var value = new DBValue(rawValue);
            var matches = value.IsInstanceOf(expectedType);

            // Assert
            value.Datum.Should().Be(rawValue);
            matches.Should().BeTrue();
        }

        [TestMethod] public void CreateFromStringDirectly() {
            // Arrange
            var rawValue = "Kvasir";
            var expectedType = DBType.Text;

            // Act
            var value = new DBValue(rawValue);
            var matches = value.IsInstanceOf(expectedType);

            // Assert
            value.Datum.Should().Be(rawValue);
            matches.Should().BeTrue();
        }

        [TestMethod] public void CreateFromDoubleDirectly() {
            // Arrange
            var rawValue = -317.445;
            var expectedType = DBType.Double;

            // Act
            var value = new DBValue(rawValue);
            var matches = value.IsInstanceOf(expectedType);

            // Assert
            value.Datum.Should().Be(rawValue);
            matches.Should().BeTrue();
        }

        [TestMethod] public void CreateFromFloatDirectly() {
            // Arrange
            var rawValue = -99.000102f;
            var expectedType = DBType.Single;

            // Act
            var value = new DBValue(rawValue);
            var matches = value.IsInstanceOf(expectedType);

            // Assert
            value.Datum.Should().Be(rawValue);
            matches.Should().BeTrue();
        }

        [TestMethod] public void CreateFromDecimalDirectly() {
            // Arrange
            var rawValue = (decimal)86.77;
            var expectedType = DBType.Decimal;

            // Act
            var value = new DBValue(rawValue);
            var matches = value.IsInstanceOf(expectedType);

            // Assert
            value.Datum.Should().Be(rawValue);
            matches.Should().BeTrue();
        }

        [TestMethod] public void CreateFromNull() {
            // Arrange
            object? rawValue = null;

            // Act
            var value = DBValue.Create(rawValue);

            // Assert
            value.Should().Be(DBValue.NULL);
        }

        [TestMethod] public void CreateFromBoolInirectly() {
            // Arrange
            var rawValue = false;
            var expectedType = DBType.Boolean;

            // Act
            var value = DBValue.Create(rawValue);
            var matches = value.IsInstanceOf(expectedType);

            // Assert
            value.Datum.Should().Be(rawValue);
            matches.Should().BeTrue();
        }

        [TestMethod] public void CreateFromCharIndirectly() {
            // Arrange
            var rawValue = '%';
            var expectedType = DBType.Character;

            // Act
            var value = DBValue.Create(rawValue);
            var matches = value.IsInstanceOf(expectedType);

            // Assert
            value.Datum.Should().Be(rawValue);
            matches.Should().BeTrue();
        }

        [TestMethod] public void CreateFromIntIndirectly() {
            // Arrange
            var rawValue = -8319;
            var expectedType = DBType.Int32;

            // Act
            var value = DBValue.Create(rawValue);
            var matches = value.IsInstanceOf(expectedType);

            // Assert
            value.Datum.Should().Be(rawValue);
            matches.Should().BeTrue();
        }

        [TestMethod] public void CreateFromUIntIndirectly() {
            // Arrange
            var rawValue = 9182u;
            var expectedType = DBType.UInt32;

            // Act
            var value = DBValue.Create(rawValue);
            var matches = value.IsInstanceOf(expectedType);

            // Assert
            value.Datum.Should().Be(rawValue);
            matches.Should().BeTrue();
        }

        [TestMethod] public void CreateFromShortIndirectly() {
            // Arrange
            var rawValue = (short)0;
            var expectedType = DBType.Int16;

            // Act
            var value = DBValue.Create(rawValue);
            var matches = value.IsInstanceOf(expectedType);

            // Assert
            value.Datum.Should().Be(rawValue);
            matches.Should().BeTrue();
        }

        [TestMethod] public void CreateFromUShortIndirectly() {
            // Arrange
            var rawValue = (ushort)102;
            var expectedType = DBType.UInt16;

            // Act
            var value = DBValue.Create(rawValue);
            var matches = value.IsInstanceOf(expectedType);

            // Assert
            value.Datum.Should().Be(rawValue);
            matches.Should().BeTrue();
        }

        [TestMethod] public void CreateFromLongIndirectly() {
            // Arrange
            var rawValue = 499182371L;
            var expectedType = DBType.Int64;

            // Act
            var value = DBValue.Create(rawValue);
            var matches = value.IsInstanceOf(expectedType);

            // Assert
            value.Datum.Should().Be(rawValue);
            matches.Should().BeTrue();
        }

        [TestMethod] public void CreateFromULongIndirectly() {
            // Arrange
            var rawValue = 111110108ul;
            var expectedType = DBType.UInt64;

            // Act
            var value = DBValue.Create(rawValue);
            var matches = value.IsInstanceOf(expectedType);

            // Assert
            value.Datum.Should().Be(rawValue);
            matches.Should().BeTrue();
        }

        [TestMethod] public void CreateFromByteIndirectly() {
            // Arrange
            var rawValue = (byte)77;
            var expectedType = DBType.UInt8;

            // Act
            var value = DBValue.Create(rawValue);
            var matches = value.IsInstanceOf(expectedType);

            // Assert
            value.Datum.Should().Be(rawValue);
            matches.Should().BeTrue();
        }

        [TestMethod] public void CreateFromSbyteIndirectly() {
            // Arrange
            var rawValue = (sbyte)4;
            var expectedType = DBType.Int8;

            // Act
            var value = DBValue.Create(rawValue);
            var matches = value.IsInstanceOf(expectedType);

            // Assert
            value.Datum.Should().Be(rawValue);
            matches.Should().BeTrue();
        }

        [TestMethod] public void CreateFromDateTimeIndirectly() {
            // Arrange
            var rawValue = new DateTime(1996, 02, 29);
            var expectedType = DBType.DateTime;

            // Act
            var value = DBValue.Create(rawValue);
            var matches = value.IsInstanceOf(expectedType);

            // Assert
            value.Datum.Should().Be(rawValue);
            matches.Should().BeTrue();
        }

        [TestMethod] public void CreateFromGuidIndirectly() {
            // Arrange
            var rawValue = new Guid(0, 1, 1, new byte[] { 2, 3, 5, 8, 13, 21, 34, 55 });
            var expectedType = DBType.Guid;

            // Act
            var value = DBValue.Create(rawValue);
            var matches = value.IsInstanceOf(expectedType);

            // Assert
            value.Datum.Should().Be(rawValue);
            matches.Should().BeTrue();
        }

        [TestMethod] public void CreateFromStringIndirectly() {
            // Arrange
            var rawValue = "Kvasir";
            var expectedType = DBType.Text;

            // Act
            var value = DBValue.Create(rawValue);
            var matches = value.IsInstanceOf(expectedType);

            // Assert
            value.Datum.Should().Be(rawValue);
            matches.Should().BeTrue();
        }

        [TestMethod] public void CreateFromDoubleIndirectly() {
            // Arrange
            var rawValue = -317.445;
            var expectedType = DBType.Double;

            // Act
            var value = DBValue.Create(rawValue);
            var matches = value.IsInstanceOf(expectedType);

            // Assert
            value.Datum.Should().Be(rawValue);
            matches.Should().BeTrue();
        }

        [TestMethod] public void CreateFromFloatIndirectly() {
            // Arrange
            var rawValue = -99.000102f;
            var expectedType = DBType.Single;

            // Act
            var value = DBValue.Create(rawValue);
            var matches = value.IsInstanceOf(expectedType);

            // Assert
            value.Datum.Should().Be(rawValue);
            matches.Should().BeTrue();
        }

        [TestMethod] public void CreateFromDecimalIndirectly() {
            // Arrange
            var rawValue = (decimal)86.77;
            var expectedType = DBType.Decimal;

            // Act
            var value = DBValue.Create(rawValue);
            var matches = value.IsInstanceOf(expectedType);

            // Assert
            value.Datum.Should().Be(rawValue);
            matches.Should().BeTrue();
        }

        [TestMethod] public void CreateFromEnumerationFails() {
            // Arrange
            var rawValue = EnvironmentVariableTarget.Machine;

            // Act
            Func<DBValue> action = () => DBValue.Create(rawValue);

            // Assert
            action.Should().ThrowExactly<ArgumentException>()
                .WithAnyMessage()
                .And
                .ParamName.Should().NotBeNullOrEmpty();
        }

        [TestMethod] public void CreateFromExceptionFails() {
            // Arrange
            var rawValue = new Exception();

            // Act
            Func<DBValue> action = () => DBValue.Create(rawValue);

            // Assert
            action.Should().ThrowExactly<ArgumentException>()
                .WithAnyMessage()
                .And
                .ParamName.Should().NotBeNullOrEmpty();
        }

        [TestMethod] public void CreateFromArrayFails() {
            // Arrange
            var rawValue = new double[] { 1.1, 2.2, 3.3, 4.4, 5.5 };

            // Act
            Func<DBValue> action = () => DBValue.Create(rawValue);

            // Assert
            action.Should().ThrowExactly<ArgumentException>()
                .WithAnyMessage()
                .And
                .ParamName.Should().NotBeNullOrEmpty();
        }

        [TestMethod] public void CreateFromCollectionFails() {
            // Arrange
            var rawValue = new List<char>() { 'K', 'v', 'a', 's', 'i', 'r', '!' };

            // Act
            Func<DBValue> action = () => DBValue.Create(rawValue);

            // Assert
            action.Should().ThrowExactly<ArgumentException>()
                .WithAnyMessage()
                .And
                .ParamName.Should().NotBeNullOrEmpty();
        }

        [TestMethod] public void NullIsInstanceOfAll() {
            // Arrange
            var nullValue = DBValue.NULL;
            var allTypes = typeof(DBType).GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Where(p => p.PropertyType == typeof(DBType))
                .Select(p => p.GetValue(null))
                .Cast<DBType>();

            
            foreach (var dbType in allTypes) {
                // Act
                var isInstance = nullValue.IsInstanceOf(dbType);

                // Assert
                isInstance.Should().BeTrue();
            }
        }

        [TestMethod] public void StringValueFitsEnumeration() {
            // Arrange
            var type = DBType.Enumeration;
            var value = DBValue.Create("String");

            // Act
            var isInstance = value.IsInstanceOf(type);

            // Assert
            isInstance.Should().BeTrue();
        }

        [TestMethod] public void MismatchIsInstanceQuery() {
            // Arrange
            var boolType = DBType.Boolean;
            var intType = DBType.UInt64;
            var floatType = DBType.Single;
            var textType = DBType.Text;
            var enumType = DBType.Enumeration;
            var value = DBValue.Create(1);

            // Act
            var boolQuery = value.IsInstanceOf(boolType);
            var intQuery = value.IsInstanceOf(intType);
            var floatQuery = value.IsInstanceOf(floatType);
            var textQuery = value.IsInstanceOf(textType);
            var enumQuery = value.IsInstanceOf(enumType);

            // Assert
            boolQuery.Should().BeFalse();
            intQuery.Should().BeFalse();
            floatQuery.Should().BeFalse();
            textQuery.Should().BeFalse();
            enumQuery.Should().BeFalse();
        }

        [TestMethod] public void ZeroIsInstanceOfAnyNumeric() {
            // Arrange
            DBType[] numerics = new DBType[] {
                DBType.Int8, DBType.Int16, DBType.Int32, DBType.Int64, DBType.UInt8, DBType.UInt16, DBType.UInt32,
                DBType.UInt64, DBType.Single, DBType.Double
            };
            var zero = DBValue.Create(0);

            // Act & Assert
            foreach (var type in numerics) {
                var isValid = zero.IsInstanceOf(type);
                isValid.Should().BeTrue();
            }
        }

        [TestMethod] public void StrongEquality() {
            // Arrange
            var zero = DBValue.Create(0);
            var hash = DBValue.Create('#');
            var nullValue = DBValue.Create(null);
            DBValue? nll = null;

            // Act
            var areEqual1 = FullCheck.ExpectEqual(zero, zero);
            var areEqual2 = FullCheck.ExpectEqual(nullValue, nullValue);
            var areNotEqual1 = FullCheck.ExpectNotEqual(zero, hash);
            var areNotEqual2 = FullCheck.ExpectNotEqual(hash, nullValue);
            var nullAreEqual = FullCheck.ExpectEqual(nll, nll);
            var nullAreNotEqual = FullCheck.ExpectNotEqual(zero, nll);

            // Assert
            areEqual1.Should().NotHaveValue();
            areEqual2.Should().NotHaveValue();
            areNotEqual1.Should().NotHaveValue();
            areNotEqual2.Should().NotHaveValue();
            nullAreEqual.Should().NotHaveValue();
            nullAreNotEqual.Should().NotHaveValue();
        }

        [TestMethod] public void WeakEquality() {
            // Arrange
            var now = new DBValue(DateTime.Now);
            var nonDbValue = now.Datum;
            object? nll = null;

            // Act
            var areNotEqual = FullCheck.ExpectNotEqual(now, nonDbValue);
            var nullAreNotEqual = FullCheck.ExpectNotEqual(now, nll);

            // Assert
            areNotEqual.Should().NotHaveValue();
            nullAreNotEqual.Should().NotHaveValue();
        }

        [TestMethod] public void StringifyRegular() {
            // Arrange
            var rawBool = true;
            var rawInt = 89;
            var rawFloat = -123.11106;
            var rawDate = new DateTime(2016, 11, 03);
            var boolValue = DBValue.Create(rawBool);
            var intValue = DBValue.Create(rawInt);
            var floatValue = DBValue.Create(rawFloat);
            var dateValue = DBValue.Create(rawDate);

            // Act
            var rawBoolStr = rawBool.ToString();
            var rawIntStr = rawInt.ToString();
            var rawFloatStr = rawFloat.ToString();
            var rawDateStr = rawDate.ToString();
            var boolStr = boolValue.ToString();
            var intStr = intValue.ToString();
            var floatStr = floatValue.ToString();
            var dateStr = dateValue.ToString();

            // Assert
            boolStr.Should().Be(rawBoolStr);
            intStr.Should().Be(rawIntStr);
            floatStr.Should().Be(rawFloatStr);
            dateStr.Should().Be(rawDateStr);
        }

        [TestMethod] public void StringifyCharacter() {
            // Arrange
            var rawChar = '&';
            var charValue = DBValue.Create(rawChar);

            // Act
            var expected = $"'{rawChar}'";
            var charStr = charValue.ToString();

            // Assert
            charStr.Should().Be(expected);
        }

        [TestMethod] public void StringifyText() {
            // Arrange
            var rawString = "Justin Millman";
            var stringValue = DBValue.Create(rawString);

            // Act
            var expected = $"\"{rawString}\"";
            var stringStr = stringValue.ToString();

            // Assert
            stringStr.Should().Be(expected);
        }

        [TestMethod] public void StringifyNull() {
            // Arrange
            var value = DBValue.NULL;

            // Act
            var str = value.ToString();

            // Assert
            str.Should().Be("NULL");
        }
    }
}
