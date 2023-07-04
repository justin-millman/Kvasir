using FluentAssertions;
using Kvasir.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace UT.Kvasir.Core {
    [TestClass, TestCategory("Built-In Enum Converters")]
    public class EnumConverterTests {
        [TestMethod] public void NonNullByteEnumToNumeric() {
            // Arrange
            var type = typeof(Byte);
            var enumerator = Byte.B1;

            // Act
            var converter = new EnumToNumericConverter(type);
            var impl = converter.ConverterImpl;
            var conversion = impl.Convert(enumerator);
            var reversion = impl.Revert(conversion);

            // Assert
            impl.SourceType.Should().Be(type);
            impl.ResultType.Should().Be(typeof(byte));
            conversion.Should().Be((byte)1);
            reversion.Should().Be(enumerator);
        }

        [TestMethod] public void NonNullIntEnumToNumeric() {
            // Arrange
            var type = typeof(Int);
            var enumerator = Int.I3;

            // Act
            var converter = new EnumToNumericConverter(type);
            var impl = converter.ConverterImpl;
            var conversion = impl.Convert(enumerator);
            var reversion = impl.Revert(conversion);

            // Assert
            impl.SourceType.Should().Be(type);
            impl.ResultType.Should().Be(typeof(int));
            conversion.Should().Be(3);
            reversion.Should().Be(enumerator);
        }

        [TestMethod] public void NonNullLongEnumToNumeric() {
            // Arrange
            var type = typeof(Long);
            var enumerator = Long.L2;

            // Act
            var converter = new EnumToNumericConverter(type);
            var impl = converter.ConverterImpl;
            var conversion = impl.Convert(enumerator);
            var reversion = impl.Revert(conversion);

            // Assert
            impl.SourceType.Should().Be(type);
            impl.ResultType.Should().Be(typeof(long));
            conversion.Should().Be(2L);
            reversion.Should().Be(enumerator);
        }

        [TestMethod] public void NonNullSByteEnumToNumeric() {
            // Arrange
            var type = typeof(SByte);
            var enumerator = SByte.SB2;

            // Act
            var converter = new EnumToNumericConverter(type);
            var impl = converter.ConverterImpl;
            var conversion = impl.Convert(enumerator);
            var reversion = impl.Revert(conversion);

            // Assert
            impl.SourceType.Should().Be(type);
            impl.ResultType.Should().Be(typeof(sbyte));
            conversion.Should().Be((sbyte)2);
            reversion.Should().Be(enumerator);
        }

        [TestMethod] public void NonNullShortEnumToNumeric() {
            // Arrange
            var type = typeof(Short);
            var enumerator = Short.S1;

            // Act
            var converter = new EnumToNumericConverter(type);
            var impl = converter.ConverterImpl;
            var conversion = impl.Convert(enumerator);
            var reversion = impl.Revert(conversion);

            // Assert
            impl.SourceType.Should().Be(type);
            impl.ResultType.Should().Be(typeof(short));
            conversion.Should().Be((short)1);
            reversion.Should().Be(enumerator);
        }

        [TestMethod] public void NonNullUIntEnumToNumeric() {
            // Arrange
            var type = typeof(UInt);
            var enumerator = UInt.UI0;

            // Act
            var converter = new EnumToNumericConverter(type);
            var impl = converter.ConverterImpl;
            var conversion = impl.Convert(enumerator);
            var reversion = impl.Revert(conversion);

            // Assert
            impl.SourceType.Should().Be(type);
            impl.ResultType.Should().Be(typeof(uint));
            conversion.Should().Be(0U);
            reversion.Should().Be(enumerator);
        }

        [TestMethod] public void NonNullULongEnumToNumeric() {
            // Arrange
            var type = typeof(ULong);
            var enumerator = ULong.UL3;

            // Act
            var converter = new EnumToNumericConverter(type);
            var impl = converter.ConverterImpl;
            var conversion = impl.Convert(enumerator);
            var reversion = impl.Revert(conversion);

            // Assert
            impl.SourceType.Should().Be(type);
            impl.ResultType.Should().Be(typeof(ulong));
            conversion.Should().Be(3UL);
            reversion.Should().Be(enumerator);
        }

        [TestMethod] public void NonNullUShortEnumToNumeric() {
            // Arrange
            var type = typeof(UShort);
            var enumerator = UShort.US2;

            // Act
            var converter = new EnumToNumericConverter(type);
            var impl = converter.ConverterImpl;
            var conversion = impl.Convert(enumerator);
            var reversion = impl.Revert(conversion);

            // Assert
            impl.SourceType.Should().Be(type);
            impl.ResultType.Should().Be(typeof(ushort));
            conversion.Should().Be((ushort)2);
            reversion.Should().Be(enumerator);
        }

        [TestMethod] public void NullByteEnumToNumeric() {
            // Arrange
            var type = typeof(Byte);
            Byte? enumerator = null;

            // Act
            var converter = new EnumToNumericConverter(type);
            var impl = converter.ConverterImpl;
            var conversion = impl.Convert(enumerator);
            var reversion = impl.Revert(conversion);

            // Assert
            impl.SourceType.Should().Be(type);
            impl.ResultType.Should().Be(typeof(byte));
            conversion.Should().Be(null);
            reversion.Should().Be(enumerator);
        }

        [TestMethod] public void NullIntEnumToNumeric() {
            // Arrange
            var type = typeof(Int);
            Int? enumerator = null;

            // Act
            var converter = new EnumToNumericConverter(type);
            var impl = converter.ConverterImpl;
            var conversion = impl.Convert(enumerator);
            var reversion = impl.Revert(conversion);

            // Assert
            impl.SourceType.Should().Be(type);
            impl.ResultType.Should().Be(typeof(int));
            conversion.Should().Be(null);
            reversion.Should().Be(enumerator);
        }

        [TestMethod] public void NullLongEnumToNumeric() {
            // Arrange
            var type = typeof(Long);
            Long? enumerator = null;

            // Act
            var converter = new EnumToNumericConverter(type);
            var impl = converter.ConverterImpl;
            var conversion = impl.Convert(enumerator);
            var reversion = impl.Revert(conversion);

            // Assert
            impl.SourceType.Should().Be(type);
            impl.ResultType.Should().Be(typeof(long));
            conversion.Should().Be(null);
            reversion.Should().Be(enumerator);
        }

        [TestMethod] public void NullSByteEnumToNumeric() {
            // Arrange
            var type = typeof(SByte);
            SByte? enumerator = null;

            // Act
            var converter = new EnumToNumericConverter(type);
            var impl = converter.ConverterImpl;
            var conversion = impl.Convert(enumerator);
            var reversion = impl.Revert(conversion);

            // Assert
            impl.SourceType.Should().Be(type);
            impl.ResultType.Should().Be(typeof(sbyte));
            conversion.Should().Be(null);
            reversion.Should().Be(enumerator);
        }

        [TestMethod] public void NullShortEnumToNumeric() {
            // Arrange
            var type = typeof(Short);
            Short? enumerator = null;

            // Act
            var converter = new EnumToNumericConverter(type);
            var impl = converter.ConverterImpl;
            var conversion = impl.Convert(enumerator);
            var reversion = impl.Revert(conversion);

            // Assert
            impl.SourceType.Should().Be(type);
            impl.ResultType.Should().Be(typeof(short));
            conversion.Should().Be(null);
            reversion.Should().Be(enumerator);
        }

        [TestMethod] public void NullUIntEnumToNumeric() {
            // Arrange
            var type = typeof(UInt);
            UInt? enumerator = null;

            // Act
            var converter = new EnumToNumericConverter(type);
            var impl = converter.ConverterImpl;
            var conversion = impl.Convert(enumerator);
            var reversion = impl.Revert(conversion);

            // Assert
            impl.SourceType.Should().Be(type);
            impl.ResultType.Should().Be(typeof(uint));
            conversion.Should().Be(null);
            reversion.Should().Be(enumerator);
        }

        [TestMethod] public void NullULongEnumToNumeric() {
            // Arrange
            var type = typeof(ULong);
            ULong? enumerator = null;

            // Act
            var converter = new EnumToNumericConverter(type);
            var impl = converter.ConverterImpl;
            var conversion = impl.Convert(enumerator);
            var reversion = impl.Revert(conversion);

            // Assert
            impl.SourceType.Should().Be(type);
            impl.ResultType.Should().Be(typeof(ulong));
            conversion.Should().Be(null);
            reversion.Should().Be(enumerator);
        }

        [TestMethod] public void NullUShortEnumToNumeric() {
            // Arrange
            var type = typeof(UShort);
            UShort? enumerator = null;

            // Act
            var converter = new EnumToNumericConverter(type);
            var impl = converter.ConverterImpl;
            var conversion = impl.Convert(enumerator);
            var reversion = impl.Revert(conversion);

            // Assert
            impl.SourceType.Should().Be(type);
            impl.ResultType.Should().Be(typeof(ushort));
            conversion.Should().Be(null);
            reversion.Should().Be(enumerator);
        }

        [TestMethod] public void NonFlagEnumToString() {
            // Arrange
            var type = typeof(String);
            var enumerator = String.Water;

            // Act
            var converter = new EnumToStringConverter(type);
            var impl = converter.ConverterImpl;
            var conversion = impl.Convert(enumerator);
            var reversion = impl.Revert(conversion);

            // Assert
            impl.SourceType.Should().Be(type);
            impl.ResultType.Should().Be(typeof(string));
            conversion.Should().Be("Water");
            reversion.Should().Be(enumerator);
        }

        [TestMethod] public void FlagEnumFlagToString() {
            // Arrange
            var type = typeof(Flags);
            var enumerator = Flags.Disabled;

            // Act
            var converter = new EnumToStringConverter(type);
            var impl = converter.ConverterImpl;
            var conversion = impl.Convert(enumerator);
            var reversion = impl.Revert(conversion);

            // Assert
            impl.SourceType.Should().Be(type);
            impl.ResultType.Should().Be(typeof(string));
            conversion.Should().Be("Disabled");
            reversion.Should().Be(enumerator);
        }

        [TestMethod] public void FlagEnumCombinationToString() {
            // Arrange
            var type = typeof(Flags);
            var enumerator = Flags.On | Flags.Mixed | Flags.Enabled;

            // Act
            var converter = new EnumToStringConverter(type);
            var impl = converter.ConverterImpl;
            var conversion = impl.Convert(enumerator);
            var reversion = impl.Revert(conversion);

            // Assert
            impl.SourceType.Should().Be(type);
            impl.ResultType.Should().Be(typeof(string));
            conversion.Should().Be("On|Enabled|Mixed");
            reversion.Should().Be(enumerator);
        }

        [TestMethod] public void NullEnumToString() {
            // Arrange
            var type = typeof(String);
            String? enumerator = null;

            // Act
            var converter = new EnumToStringConverter(type);
            var impl = converter.ConverterImpl;
            var conversion = impl.Convert(enumerator);
            var reversion = impl.Revert(conversion);

            // Assert
            conversion.Should().Be(null);
            reversion.Should().Be(enumerator);
        }


        enum Byte : byte { B0, B1, B2, B3 };
        enum Int : int { I0, I1, I2, I3 };
        enum Long : long { L0, L1, L2, L3 };
        enum SByte : sbyte { SB0, SB1, SB2, SB3 };
        enum Short : short { S0, S1, S2, S3 };
        enum UInt : uint { UI0, UI1, UI2, UI3 };
        enum ULong : ulong { UL0, UL1, UL2, UL3 };
        enum UShort : ushort { US0, US1, US2, US3 };

        enum String { Water, Earth, Fire, Air, Energy }
        [Flags] enum Flags { None = 0, On = 1, Off = 2, Enabled = 4, Disabled = 8, Partial = 16, Mixed = 32 }
    }
}
