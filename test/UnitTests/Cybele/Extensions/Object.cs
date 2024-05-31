using Cybele.Extensions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace UT.Cybele.Extensions {
    [TestClass, TestCategory("Object: ForDisplay")]
    public sealed class Object_ForDispay : ExtensionTests {
        [TestMethod] public void DisplayNull() {
            // Act
            var nullDisplay = ((object?)null).ForDisplay();

            // Assert
            nullDisplay.Should().Be("'null'");
        }

        [TestMethod] public void DisplayDBNull() {
            // Act
            var nullDisplay = DBNull.Value.ForDisplay();

            // Assert
            nullDisplay.Should().Be("'null'");
        }

        [TestMethod] public void DisplayBoolean() {
            // Arrange
            var trueBool = true;
            var falseBool = false;

            // Act
            var trueDisplay = trueBool.ForDisplay();
            var falseDisplay = falseBool.ForDisplay();

            // Assert
            trueDisplay.Should().Be("true");
            falseDisplay.Should().Be("false");
        }

        [TestMethod] public void DisplayInteger() {
            // Arrange
            var int8 = (sbyte)37;
            var int16 = (short)-190;
            var int32 = 590244;
            var int64 = -18273193823;
            var uint8 = (byte)1;
            var uint16 = (ushort)999;
            var uint32 = 73123u;
            var uint64 = 892871023123024UL;

            // Act
            var displayInt8 = int8.ForDisplay();
            var displayInt16 = int16.ForDisplay();
            var displayInt32 = int32.ForDisplay();
            var displayInt64 = int64.ForDisplay();
            var displayUInt8 = uint8.ForDisplay();
            var displayUInt16 = uint16.ForDisplay();
            var displayUInt32 = uint32.ForDisplay();
            var displayUInt64 = uint64.ForDisplay();

            // Assert
            displayInt8.Should().Be(int8.ToString());
            displayInt16.Should().Be(int16.ToString());
            displayInt32.Should().Be(int32.ToString());
            displayInt64.Should().Be(int64.ToString());
            displayUInt8.Should().Be(uint8.ToString());
            displayUInt16.Should().Be(uint16.ToString());
            displayUInt32.Should().Be(uint32.ToString());
            displayUInt64.Should().Be(uint64.ToString());
        }

        [TestMethod] public void DisplayNonIntegralFloatingPoint() {
            // Arrange
            var decimalNumber = (decimal)-0.99813;
            var doubleNumber = 84402.77123;
            var floatNumber = 7261283798123.2814f;

            // Act
            var decimalDisplay = decimalNumber.ForDisplay();
            var doubleDisplay = doubleNumber.ForDisplay();
            var floatDisplay = floatNumber.ForDisplay();

            // Assert
            decimalDisplay.Should().Be(decimalNumber.ToString());
            doubleDisplay.Should().Be(doubleNumber.ToString());
            floatDisplay.Should().Be(floatNumber.ToString());
        }

        [TestMethod] public void DisplayIntegralFloatingPoint() {
            // Arrange
            var decimalNumber = (decimal)18.0;
            var doubleNumber = -671824.0;
            var floatNumber = -99991.0f;

            // Act
            var decimalDisplay = decimalNumber.ForDisplay();
            var doubleDisplay = doubleNumber.ForDisplay();
            var floatDisplay = floatNumber.ForDisplay();

            // Assert
            decimalDisplay.Should().Be(decimalNumber.ToString() + ".0");
            doubleDisplay.Should().Be(doubleNumber.ToString() + ".0");
            floatDisplay.Should().Be(floatNumber.ToString() + ".0");
        }

        [TestMethod] public void DisplayInfinity() {
            // Arrange
            var posInfinity = double.PositiveInfinity;
            var negInfinity = double.NegativeInfinity;

            // Act
            var posInfinityDisplay = posInfinity.ForDisplay();
            var negInfinityDisplay = negInfinity.ForDisplay();

            // Assert
            posInfinityDisplay.Should().Be(posInfinity.ToString());
            negInfinityDisplay.Should().Be(negInfinity.ToString());
        }

        [TestMethod] public void DisplayScientificNotation() {
            // Arrange
            var scientificSmall = 0.0000001f;
            var scientificLarge = 10000000000000f;

            // Act
            var scientificSmallDisplay = scientificSmall.ForDisplay();
            var scientificLargeDisplay = scientificLarge.ForDisplay();

            // Assert
            scientificSmallDisplay.Should().Be("1E-07");
            scientificLargeDisplay.Should().Be("1E+13");
        }

        [TestMethod] public void DisplayCharacter() {
            // Arrange
            var character = '&';

            // Act
            var characterDisplay = character.ForDisplay();

            // Assert
            characterDisplay.Should().Be($"'{character}'");
        }

        [TestMethod] public void DisplayEnumerator() {
            // Arrange
            var enumerator = TestTimeout.Infinite;

            // Act
            var characterDisplay = enumerator.ForDisplay();

            // Assert
            characterDisplay.Should().Be($"{nameof(TestTimeout)}.{enumerator}");
        }

        [TestMethod] public void DisplayEmptyString() {
            // Arrange
            var emptyString = "";

            // Act
            var emptyStringDisplay = emptyString.ForDisplay();

            // Assert
            emptyStringDisplay.Should().Be($"\"{emptyString}\"");
        }

        [TestMethod] public void DisplayNonEmptyString() {
            // Arrange
            var nonEmptyString = "Chesapeake";

            // Act
            var nonEmptyStringDisplay = nonEmptyString.ForDisplay();

            // Assert
            nonEmptyStringDisplay.Should().Be($"\"{nonEmptyString}\"");
        }

        [TestMethod] public void DispalyEmptyArray() {
            // Arrange
            var array = new object[] {};

            // Act
            var arrayDisplay = array.ForDisplay();

            // Assert
            arrayDisplay.Should().Be("{}");
        }

        [TestMethod] public void DisplayNonEmptyArray() {
            // Arrange
            var array = new object?[] { 3, "East Providence", -20000L, false, null };

            // Act
            var arrayDisplay = array.ForDisplay();

            // Assert
            arrayDisplay.Should().Be("{ 3, \"East Providence\", -20000, false, 'null' }");
        }
    }
}
