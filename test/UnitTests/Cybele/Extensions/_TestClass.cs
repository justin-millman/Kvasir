﻿using System;

namespace UT.Cybele.Extensions {
    public class ExtensionTests {
        [Flags] protected enum Color : byte {
            Black = 0,
            Red = 1 << 1,
            Yellow = 1 << 2,
            Blue = 1 << 3,
            White = Red | Yellow | Blue
        }
        protected enum Day : sbyte {
            Monday = 1,
            Tuesday = 2,
            Wednesday = 3,
            Thursday = 4,
            Friday = 5,
            Saturday = 6,
            Sunday = 7
        }
        protected enum Month : ushort {
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
        [Flags] protected enum Diacritic : short {
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
        protected enum Military : int {
            Civilian = 0,
            Army = 1,
            Navy = 25712,
            Marines = -663,
            AirForce = int.MaxValue,
            CoastGuard = int.MinValue,
            SpaceForce = 4000001,
            ROTC = -2177096
        }
        [Flags] protected enum BaseballPosition : uint {
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
        [Flags] protected enum Nationality : long {
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
        protected enum BonusResource : ulong {
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

        [AttributeUsage(AttributeTargets.All, Inherited = true)] protected class InheritedAttribute : Attribute {}
        [AttributeUsage(AttributeTargets.All, Inherited = false)] protected class UninheritedAttribute : Attribute {}

        [Inherited, Uninherited] protected class Base {
            [Inherited, Uninherited] public int NonVirtualProperty => 10;
            [Inherited, Uninherited] public virtual int VirtualProperty => 100;
            [Inherited, Uninherited] public void NonVirtualFunction() {}
            [Inherited, Uninherited] public virtual void VirtualFunction() {}
        }
        protected class Derived : Base {
            public override int VirtualProperty => 200;
            public override void VirtualFunction() {}
        }


        protected static readonly Random rand = new Random(02291996);
    }
}
