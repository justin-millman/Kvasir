using Cybele.Extensions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace UT.Cybele.Extensions {
    [TestClass, TestCategory("Enum: IsValid")]
    public sealed class Enum_IsValid : ExtensionTests {
        [TestMethod] public void ZeroEnumeratorValidity() {
            // Arrange
            var color0 = Color.Black;
            var day0 = (Day)0;
            var month0 = (Month)0;
            var diacritic0 = Diacritic.None;
            var military0 = Military.Civilian;
            var position0 = (BaseballPosition)0;
            var nationality0 = (Nationality)0;
            var resource0 = BonusResource.None;

            // Act
            var color0validity = color0.IsValid();
            var day0validity = day0.IsValid();
            var month0validity = month0.IsValid();
            var diacritic0validity = diacritic0.IsValid();
            var military0validity = military0.IsValid();
            var position0validity = position0.IsValid();
            var nationality0validity = nationality0.IsValid();
            var resourcce0validity = resource0.IsValid();

            // Assert
            color0validity.Should().BeTrue();
            day0validity.Should().BeFalse();
            month0validity.Should().BeFalse();
            diacritic0validity.Should().BeTrue();
            military0validity.Should().BeTrue();
            position0validity.Should().BeFalse();
            nationality0validity.Should().BeFalse();
            resourcce0validity.Should().BeTrue();
        }

        [TestMethod] public void ValidNonFlagEnumerator() {
            static void TestEach<TEnum>() where TEnum : System.Enum {
                foreach (var e in typeof(TEnum).GetEnumValues().Cast<TEnum>()) {
                    // Act
                    var validity = e.IsValid<TEnum>();

                    // Assert
                    validity.Should().BeTrue();
                }
            }

            // Arrange
            TestEach<Day>();
            TestEach<Month>();
            TestEach<Military>();
            TestEach<BonusResource>();
        }

        [TestMethod] public void ValidFlagEnumerator() {
            static void TestEach<TEnum>() where TEnum : System.Enum {
                foreach (var e in typeof(TEnum).GetEnumValues().Cast<TEnum>()) {
                    // Act
                    var validity = e.IsValid<TEnum>();

                    // Assert
                    validity.Should().BeTrue();
                }
            }

            // Arrange
            TestEach<Color>();
            TestEach<Diacritic>();
            TestEach<BaseballPosition>();
            TestEach<Nationality>();
        }

        [TestMethod] public void ValidFlagCombination() {
            static void TestCombinations<TEnum>() where TEnum : Enum {
                var flags = typeof(TEnum).GetEnumValues().Cast<TEnum>().ToArray<TEnum>();   // get all flag enumerators
                for (int i = 0; i < 10; ++i) {                                              // run 10 total tests
                    dynamic value = default(TEnum)!;                                        //   - start with 0
                    for (int j = 0; j < rand.Next(2, flags.Length + 1); ++j) {              //   - at least 2 bits on
                        value |= flags[rand.Next(0, flags.Length)];                         //   - randomly pick one
                    }

                    // Act
                    var validity = ((TEnum)value).IsValid<TEnum>();

                    // Assert
                    validity.Should().BeTrue();
                }
            }

            // Arrange
            TestCombinations<Color>();
            TestCombinations<Diacritic>();
            TestCombinations<BaseballPosition>();
            TestCombinations<Nationality>();
        }

        [TestMethod] public void InvalidNonFlagEnumerator() {
            // Arrange
            var invalidDay0 = (Day)11;
            var invalidDay1 = (Day)(-6);
            var invalidMonth0 = (Month)short.MaxValue;
            var invalidMonth1 = (Month)2710;
            var invalidMilitary0 = (Military)(-874);
            var invalidMilitary1 = (Military)188281;
            var invalidResource0 = (BonusResource)566;
            var invalidResource1 = (BonusResource)ulong.MaxValue;

            // Act
            var invalidDay0validity = invalidDay0.IsValid();
            var invalidDay1validity = invalidDay1.IsValid();
            var invalidMonth0validity = invalidMonth0.IsValid();
            var invalidMonth1validity = invalidMonth1.IsValid();
            var invalidMilitary0validity = invalidMilitary0.IsValid();
            var invalidMilitary1validity = invalidMilitary1.IsValid();
            var invalidResource0validity = invalidResource0.IsValid();
            var invalidResource1validity = invalidResource1.IsValid();

            // Assert
            invalidDay0validity.Should().BeFalse();
            invalidDay1validity.Should().BeFalse();
            invalidMonth0validity.Should().BeFalse();
            invalidMonth1validity.Should().BeFalse();
            invalidMilitary0validity.Should().BeFalse();
            invalidMilitary1validity.Should().BeFalse();
            invalidResource0validity.Should().BeFalse();
            invalidResource1validity.Should().BeFalse();
        }

        [TestMethod] public void InvalidFlagEnumerator() {
            // Arrange
            var invalidColor0 = (Color)(1 << 4);
            var invalidColor1 = (Color)(1 << 7);
            var invalidPosition0 = (BaseballPosition)(1u << 15);
            var invalidPosition1 = (BaseballPosition)(1u << 29);
            var invalidNationality0 = (Nationality)(1L << 12);
            var invalidNationality1 = (Nationality)(1L << 57);

            // Act
            var invalidColor0validity = invalidColor0.IsValid();
            var invalidColor1validity = invalidColor1.IsValid();
            var invalidPosition0validity = invalidPosition0.IsValid();
            var invalidPosition1validity = invalidPosition1.IsValid();
            var invalidNationality0validity = invalidNationality0.IsValid();
            var invalidNationality1validity = invalidNationality1.IsValid();

            // Assert
            invalidColor0validity.Should().BeFalse();
            invalidColor1validity.Should().BeFalse();
            invalidPosition0validity.Should().BeFalse();
            invalidPosition1validity.Should().BeFalse();
            invalidNationality0validity.Should().BeFalse();
            invalidNationality1validity.Should().BeFalse();
        }

        [TestMethod] public void InvalidFlagCombination() {
            // Arrange
            var invalidColor = (Color)(byte.MaxValue);
            var invalidPosition = (BaseballPosition)(1u << 15 | 1u << 7 | 1u << 22 | 1u << 1 | 1u << 30);
            var invalidNationality = (Nationality)(1L << 16 | 1L << 24 | 1L << 0);

            // Act
            var invalidColorValidity = invalidColor.IsValid();
            var invalidPositionValidity = invalidPosition.IsValid();
            var invalidNationalityValidity = invalidNationality.IsValid();

            // Assert
            invalidColorValidity.Should().BeFalse();
            invalidPositionValidity.Should().BeFalse();
            invalidNationalityValidity.Should().BeFalse();
        }
    }
}
