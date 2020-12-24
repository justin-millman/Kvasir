using Cybele.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace Test.Cybele.Extensions {
    [TestClass]
    public class EnumExtensionTests {
        [TestMethod, TestCategory("IsValid")]
        public void ZeroEnumerator() {
            var color0 = Color.Black;
            var day0 = (Day)0;
            var month0 = (Month)0;
            var diacritic0 = Diacritic.None;
            var military0 = Military.Civilian;
            var position0 = (BaseballPosition)0;
            var latino0 = (Latino)0;
            var resource0 = BonusResource.None;

            Assert.AreEqual(color0, (Color)0);
            Assert.AreEqual(diacritic0, (Diacritic)0);
            Assert.AreEqual(military0, (Military)0);
            Assert.AreEqual(resource0, (BonusResource)0);

            Assert.IsTrue(color0.IsValid());
            Assert.IsFalse(day0.IsValid());
            Assert.IsFalse(month0.IsValid());
            Assert.IsTrue(diacritic0.IsValid());
            Assert.IsTrue(military0.IsValid());
            Assert.IsFalse(position0.IsValid());
            Assert.IsFalse(latino0.IsValid());
            Assert.IsTrue(resource0.IsValid());
        }

        [TestMethod, TestCategory("IsValid")]
        public void ValidNonFlagEnumerator() {
            static void TestEach<TEnum>() where TEnum : Enum {
                foreach (var e in typeof(TEnum).GetEnumValues()) {
                    var enumerator = (TEnum)e!;
                    Assert.IsTrue(enumerator.IsValid());
                }
            }

            TestEach<Day>();
            TestEach<Month>();
            TestEach<Military>();
            TestEach<BonusResource>();
        }

        [TestMethod, TestCategory("IsValid")]
        public void ValidFlagEnumerator() {
            static void TestEach<TEnum>() where TEnum : Enum {
                foreach (var e in typeof(TEnum).GetEnumValues().Cast<TEnum>()) {
                    Assert.IsTrue(e.IsValid());
                }
            }

            TestEach<Color>();
            TestEach<Diacritic>();
            TestEach<BaseballPosition>();
            TestEach<Latino>();
        }

        [TestMethod, TestCategory("IsValid")]
        public void ValidFlagCombination() {
            static void TestCombinations<TEnum>() where TEnum : Enum {
                var flags = typeof(TEnum).GetEnumValues().Cast<TEnum>().ToArray();
                for (int i = 0; i < 10; ++i) {
                    dynamic value = default(TEnum)!;
                    for (int j = 0; j < rand.Next(2, flags.Length + 1); ++j) {
                        value |= flags[rand.Next(0, flags.Length)];
                    }

                    var testValue = (TEnum)value;
                    Assert.IsTrue(testValue.IsValid());
                }
            }

            TestCombinations<Color>();
            TestCombinations<Diacritic>();
            TestCombinations<BaseballPosition>();
            TestCombinations<Latino>();
        }

        [TestMethod, TestCategory("IsValid")]
        public void InvalidNonFlagEnumerator() {
            var invalidDay0 = (Day)11;
            var invalidDay1 = (Day)(-6);
            var invalidMonth0 = (Month)short.MaxValue;
            var invalidMonth1 = (Month)2710;
            var invalidMilitary0 = (Military)(-874);
            var invalidMilitary1 = (Military)188281;
            var invalidBonusResource0 = (BonusResource)566;
            var invalidBonusResource1 = (BonusResource)ulong.MaxValue;

            Assert.IsFalse(invalidDay0.IsValid());
            Assert.IsFalse(invalidDay1.IsValid());
            Assert.IsFalse(invalidMonth0.IsValid());
            Assert.IsFalse(invalidMonth1.IsValid());
            Assert.IsFalse(invalidMilitary0.IsValid());
            Assert.IsFalse(invalidMilitary1.IsValid());
            Assert.IsFalse(invalidBonusResource0.IsValid());
            Assert.IsFalse(invalidBonusResource1.IsValid());
        }

        [TestMethod, TestCategory("IsValid")]
        public void InvalidFlagEnumerator() {
            var invalidColor0 = (Color)(1 << 4);
            var invalidColor1 = (Color)(1 << 7);
            var invalidBaseballPosition0 = (BaseballPosition)(1u << 15);
            var invalidBaseballPosition1 = (BaseballPosition)(1u << 29);
            var invalidLatino0 = (Latino)(1L << 12);
            var invalidLatino1 = (Latino)(1L << 57);

            Assert.IsFalse(invalidColor0.IsValid());
            Assert.IsFalse(invalidColor1.IsValid());
            Assert.IsFalse(invalidBaseballPosition0.IsValid());
            Assert.IsFalse(invalidBaseballPosition1.IsValid());
            Assert.IsFalse(invalidLatino0.IsValid());
            Assert.IsFalse(invalidLatino1.IsValid());
        }

        [TestMethod, TestCategory("IsValid")]
        public void InvalidFlagCombination() {
            var invalidColor = (Color)(byte.MaxValue);
            var invalidBaseballPosition = (BaseballPosition)(1u << 15 | 1u << 7 | 1u << 22 | 1u << 1 | 1u << 30);
            var invalidLatino = (Latino)(1L << 16 | 1L << 24 | 1L << 0);

            Assert.IsFalse(invalidColor.IsValid());
            Assert.IsFalse(invalidBaseballPosition.IsValid());
            Assert.IsFalse(invalidLatino.IsValid());
        }


        [Flags] private enum Color : byte {
            Black = 0,
            Red = 1 << 1,
            Yellow = 1 << 2,
            Blue = 1 << 3,
            White = Red | Yellow | Blue
        }
        private enum Day : sbyte {
            Monday = 1,
            Tuesday = 2,
            Wednesday = 3,
            Thursday = 4,
            Friday = 5,
            Saturday = 6,
            Sunday = 7
        }
        private enum Month : ushort {
            January = 4,
            February = 21,
            March = 199,
            April = 357,
            May = 1001,
            June = 1002,
            July = 2322,
            August = 3496,
            September = 4898,
            October = 5187,
            November = 5995,
            December = 6374
        }
        [Flags] private enum Diacritic : short {
            None = 0,
            AcuteAccent = 1 << 0,
            GraveAccent = 1 << 1,
            Diaeresis = 1 << 2,
            Circumflex = 1 << 3,
            Cedilla = 1 << 4,
            Macron = 1 << 5,
            Overring = 1 << 6,
            Underdot = 1 << 7,
            Caron = 1 << 8,
            Tilde = 1 << 9,
            Breve = 1 << 10,
            Ogonek = 1 << 11,
            HookAbove = 1 << 12,
            Throughbar = 1 << 13,
            RoughBreathing = 1 << 14,
            SmoothBreathing = -32768
        }
        private enum Military : int {
            Civilian = 0,
            Army = 1,
            Navy = 25712,
            Marines = -663,
            AirForce = int.MaxValue,
            CoastGuard = int.MinValue,
            SpaceForce = 4000001,
            ROTC = -2177096
        }
        [Flags] public enum BaseballPosition : uint {
            StartingPitcher = 1u << 31,
            ReliefPitcher = 1u << 18,
            Closer = 1u << 4,
            Catcher = 1u << 27,
            FirstBaseman = 1u << 11,
            SecondBaseman = 1u << 19,
            ThirdBaseman = 1u << 9,
            Shortstop = 1u << 0,
            LeftFielder = 1u << 30,
            CenterFielder = 1u << 22,
            RightFielder = 1u << 16,
            DesignatedHitter = 1u << 8
        }
        [Flags] public enum Latino : long {
            Mexican = 1L << 1,
            Dominican = 1L << 3,
            Panamanian = 1L << 5,
            Salvadoran = 1L << 7,
            Nicaraguan = 1L << 9,
            Argentine = 1L << 11,
            Cuban = 1L << 13,
            PuertoRican = 1L << 15,
            Guatemalan = 1L << 17,
            CostaRican = 1L << 19,
            Chilean = 1L << 21,
            Colombian = 1L << 23,
            Venezuelan = 1L << 25,
            Bolivian = 1L << 27,
            Peruvian = 1L << 29,
            Ecuadoran = 1L << 31,
            Honduran = 1L << 33,
            Paraguayan = 1L << 35,
            Uruguayan = 1L << 37,
            Nahua = 1L << 39,
            Mayan = 1L << 41,
            Chicano = 1L << 43
        }
        public enum BonusResource : ulong {
            None = 0,
            Bananas,
            Cattle,
            Copper,
            Crabs,
            Deer,
            Fish,
            Maize,
            Rice,
            Sheep,
            Stone,
            Wheat
        }


        private static readonly Random rand = new Random(02291996);
    }
}
