using Kvasir.Annotations;
using Kvasir.Relations;
using System;
using System.Collections;
using System.Collections.Generic;

using static UT.Kvasir.Translation.TestConstraints;
using static UT.Kvasir.Translation.TestConverters;

namespace UT.Kvasir.Translation {
    internal static class PropertyTypes {
        // Test Scenario: Non-Nullable Scalars (✓recognized✓)
        public class Smorgasbord {
            public byte Byte { get; set; }
            public char Char { get; set; }
            public DateTime DateTime { get; set; }
            public decimal Decimal { get; set; }
            public double Double { get; set; }
            public float Float { get; set; }
            public Guid Guid { get; set; }
            [PrimaryKey] public int Int { get; set; }
            public long Long { get; set; }
            public sbyte SByte { get; set; }
            public short Short { get; set; }
            public string String { get; set; } = "";
            public uint UInt { get; set; }
            public ulong ULong { get; set; }
            public ushort UShort { get; set; }
        }

        // Test Scenario: Nullable Scalars (✓recognized✓)
        public class Plethora {
            public byte? Byte { get; set; }
            public char? Char { get; set; }
            public DateTime? DateTime { get; set; }
            public decimal? Decimal { get; set; }
            public double? Double { get; set; }
            public float? Float { get; set; }
            public Guid? Guid { get; set; }
            public int? Int { get; set; }
            public long? Long { get; set; }
            public sbyte? SByte { get; set; }
            public short? Short { get; set; }
            public string? String { get; set; } = "";
            public uint? UInt { get; set; }
            public ulong? ULong { get; set; }
            public ushort? UShort { get; set; }
            [PrimaryKey] public int PrimaryKey { get; set; }
        }

        // Test Scenario: Delegate (✗not permitted✗)
        public delegate void HurricaneAction();
        public class Hurricane {
            [PrimaryKey] public short Year { get; set; }
            [PrimaryKey] public byte Number { get; set; }
            public ulong TopWindSpeed { get; set; }
            public ulong Damage { get; set; }
            public uint Casualties { get; set; }
            public HurricaneAction Form { get; set; } = () => {};
        }

        // Test Scenario: `dynamic` (✗not permitted✗)
        public class MonopolyProperty {
            [PrimaryKey] public string Name { get; set; } = "";
            public byte Rent { get; set; }
            public byte Mortgage { get; set; }
            public dynamic HotelCost { get; set; } = 100.0;
        }

        // Test Scenario: `object` (✗not permitted✗)
        public class URL {
            public string Scheme { get; set; } = "";
            public object? NetLoc { get; set; }
            [PrimaryKey] public string Path { get; set; } = "";
            public string Params { get; set; } = "";
            public string? Query { get; set; }
            public string? Fragment { get; set; }
            public string? Username { get; set; }
            public string? Password { get; set; }
            public string? Hostname { get; set; }
            public ushort Port { get; set; }
        }

        // Test Scenario: `System.Enum` (✗not permitted✗)
        public class Enumeration {
            [PrimaryKey] public string Namespace { get; set; } = "";
            [PrimaryKey] public string Typename { get; set; } = "";
            public Enum? ZeroValue { get; set; }
            public uint EnumeratorCount { get; set; }
        }

        // Test Scenario: `System.ValueType` (✗not permitted✗)
        public class YouTubeVideo {
            [PrimaryKey] public string URL { get; set; } = "";
            public uint Length { get; set; }
            public ulong Likes { get; set; }
            public string Channel { get; set; } = "";
            public ValueType CommentCount { get; set; } = 0;
        }

        // Test Scenario: Non-Collection Class Type from the C# Standard Library (✗not permitted✗)
        public class Coin {
            [PrimaryKey] public byte Value { get; set; }
            public float Diameter { get; set; }
            [PrimaryKey] public bool InCirculation { get; set; }
            public Exception CounterfeitResult { get; set; } = new ApplicationException("COUNTERFEIT!");
        }

        // Test Scenario Collection Type from the C# Standard Library (✗not permitted✗)
        public class Eigenvector {
            [PrimaryKey] public Guid ID { get; set; }
            public float Eigenvalue { get; set; }
            public ArrayList Vector { get; set; } = new();
        }

        // Test Scenario: Struct Type from the C# Standard Library (✓recognized✓)
        public class Emoji {
            [Flags] public enum Platform { iOS = 1, Android = 2, Slack = 4, Facebook = 8 }

            [PrimaryKey] public ulong UnicodeNumber { get; set; }
            public Uri Identity { get; set; } = new("");
            public string Developer { get; set; } = "";
            public ulong Usages { get; set; }
            public DateTime Released { get; set; }
            public Platform Platforms { get; set; }
        }

        // Test Scenario: Class Type from a Third-Party NuGet Package (✗not permitted✗)
        public class UUID {
            [PrimaryKey] public string Value { get; set; } = "";
            [PrimaryKey] public byte Version { get; set; }
            public string Signature { get; set; } = "";
            public string Encoding { get; set; } = "";
            public FluentAssertions.Execution.Continuation GenerationScope { get; set; } = null!;
        }

        // Test Scenario: User-Defined Interface (✗not permitted✗)
        public interface IArtist {}
        public class Painting {
            [PrimaryKey] public Guid NGAID { get; set; }
            public decimal Height { get; set; }
            public decimal Width { get; set; }
            public IArtist? Artist { get; set; }
            public short Year { get; set; }
        }

        // Test Scenario: User-Defined Closed Generic (✗not permitted✗)
        public class MessageCount<T> {}
        public class SlackChannel {
            [PrimaryKey] public Guid ID { get; set; }
            public string ChannelName { get; set; } = "";
            public long Members { get; set; }
            public bool IsPrivate { get; set; }
            public MessageCount<short>? NumMessages { get; set; }
        }

        // Test Scenario: User-Defined Abstract Class (✗not permitted✗)
        public abstract class Flower {}
        public class BotanicalGarden {
            [PrimaryKey] public int ID { get; set; }
            public DateTime Opening { get; set; }
            public ulong VisitorsPerYear { get; set; }
            public Flower? OfficialFlower { get; set; }
        }

        // Test Scenario: Array (✗not permitted✗)
        public class MeritBadge {
            [Flags] public enum Group { BoyScouts = 1, GirlScouts = 2, CubScouts = 4 }

            [PrimaryKey] public Guid BadgeID { get; set; }
            public string Name { get; set; } = "";
            public Group AwardedBy { get; set; }
            public uint[] PointsRequired { get; set; } = new uint[] {};
            public double PercentAcquired { get; set; }
            public DateTime Introduced { get; set; }
        }

        // Test Scenario: Pointer (✗not permitted✗)
        public class Assassination {
            [PrimaryKey] public string Victim { get; set; } = "";
            public string Perpetrator { get; set; } = "";
            public DateTime Date { get; set; }
            public unsafe ushort* Witnesses { get; set; }
            public bool Political { get; set; }
            public bool StateSanctioned { get; set; }
        }

        // Test Scenario: Property with Unsupported Type Marked as [CodeOnly] (✓excluded✓)
        public interface IArmor {}
        public class CustomBackground<T> {}
        public class DNDCharacter {
            [PrimaryKey] public string Name { get; set; } = "";
            public byte Charisma { get; set; }
            public byte Constitution { get; set; }
            public byte Dexterity { get; set; }
            public byte Intelligence { get; set; }
            public byte Strength { get; set; }
            public byte Wisdom { get; set; }
            [CodeOnly] public Func<byte, byte, bool, byte>? RollDice { get; set; }
            [CodeOnly] public dynamic Class { get; set; } = -1;
            [CodeOnly] public object? SubClass { get; set; }
            [CodeOnly] public Enum Race { get; set; } = GCCollectionMode.Optimized;
            [CodeOnly] public ValueType HP { get; set; } = 0;
            [CodeOnly] public Tuple<int, int> AttackEconomy { get; set; } = new Tuple<int, int>(1, 1);
            [CodeOnly] public IArmor? Armor { get; set; }
            [CodeOnly] public CustomBackground<DNDCharacter>? Background { get; set; }
        }

        // Test Scenario: Enumerations (✓recognized✓)
        public class DNDWeapon {
            public enum WeaponType { Simple, Martial, Improvised };
            [Flags] public enum WeaponProperty { Ranged = 1, TwoHanded = 2, Finesse = 4, Silvered = 8 };

            [PrimaryKey] public string Name { get; set; } = "";
            public ushort AttackBonus { get; set; }
            public ushort AverageDamage { get; set; }
            public WeaponType Type { get; set; }
            public WeaponProperty Properties { get; set; }
            public DayOfWeek? MostEffectiveOn { get; set; }
        }

        // Test Scenario: Non-Nullable Aggregates (✓recognized✓)
        public class ChineseDynasty {
            public struct Person {
                public string Name { get; set; }
                public short ReignBegin { get; set; }
                public short ReignEnd { get; set; }
                public string? Death { get; set; }
            }
            public record struct City(string Name);

            [PrimaryKey] public string Name { get; set; } = "";
            public Person Founder { get; set; }
            public ulong MaxExtent { get; set; }
            public short Established { get; set; }
            public short Fell { get; set; }
            public ulong Population { get; set; }
            public City Capital { get; set; }
        }

        // Test Scenario: Nullable Aggregates (✓recognized✓)
        public class BarbecueSauce {
            public record struct Nutrition(uint Calories, double Fat, double Sugar, double Carbohydrates);
            public enum Kind { Sweet, Spicy, Tangy, Chocolatey }

            [PrimaryKey] public Guid ID { get; set; }
            public string Brand { get; set; } = "";
            public Nutrition? PerServing { get; set; }
            public bool KetchupBased { get; set; }
            public Kind Style { get; set; }
        }

        // Test Scenario: Aggregates Nested Within Aggregates (✓recognized✓)
        public class DNDMonster {
            public record struct Sight(ushort Distance, bool Darkness, bool Trueness);
            public record struct Senses(byte PassivePerception, Sight Sight);
            public record struct SavingThrow(byte STR, byte CON, byte CHA, byte WIS, byte INT, byte DEX);
            public record struct Abilities(byte STR, byte CON, byte CHA, byte WIS, byte INT, byte DEX, SavingThrow Saves);
            public enum BodySize { Tiny, Small, Medium, Large, Huge, Gargantuan }

            [PrimaryKey] public string Species { get; set; } = "";
            public Abilities? Stats { get; set; }
            public BodySize Size { get; set; }
            public Senses PhysicalSenses { get; set; }
            public ushort CR { get; set; }
            public uint AC { get; set; }
            public uint HP { get; set; }
            public byte LegendaryActions { get; set; }
        }

        // Test Scenario: Non-Nullable References (✓recognized✓)
        public class Scorpion {
            public class TaxonomicGenus {
                public string Family { get; set; } = "";
                [PrimaryKey] public string Genus { get; set; } = "";
            }

            [PrimaryKey] public string CommonName { get; set; } = "";
            public TaxonomicGenus Genus { get; set; } = new();
            public string Species { get; set; } = "";
            public double StingIndex { get; set; }
            public float AverageLength { get; set; }
            public float AverageWeight { get; set; }
        }

        // Test Scenario: Nullable References (✓recognized✓)
        public class Ferry {
            [Flags] public enum Kind { Passenger = 1, Cargo = 2, State = 4 }

            public class Port {
                [PrimaryKey] public Guid PortID { get; set; }
                [PrimaryKey] public string PortName { get; set; } = "";
                public double TaxRate { get; set; }
                public ushort NumDocks { get; set; }
            }

            [PrimaryKey] public Guid RegistrationNumber { get; set; }
            public ulong? PassengerCapacity { get; set; }
            public Kind Type { get; set; }
            public Port? Embarcation { get; set; }
            public Port? Destination { get; set; }
        }

        // Test Scenario: References Nested Within Aggregates (✓recognized✓)
        public class WeekendUpdate {
            public class Actor {
                [PrimaryKey] public string FirstName { get; set; } = "";
                [PrimaryKey] public string LastName { get; set; } = "";
            }
            public class Date {
                public enum MonthOfYear { JAN, FEB, MAR, APR, MAY, JUN, JUL, AUG, SEP, OCT, NOV, DEC }
                [PrimaryKey] public MonthOfYear Month { get; set; }
                [PrimaryKey] public byte Day { get; set; }
                [PrimaryKey] public short Year { get; set; }
            }
            public record struct Character(string Name, Actor Portrayal);

            [PrimaryKey] public Guid ID { get; set; }
            public Date Airing { get; set; } = new();
            public Actor Anchor { get; set; } = new();
            public Character? FirstSegment { get; set; }
            public Character? SecondSegment { get; set; }
            public sbyte NumJokes { get; set; }
        }

        // Test Scenario: References Nested Within References as Non-Primary-Key (✓recognized✓)
        public class DannyPhantomGhost {
            public enum Ability { Intangibiblity, Blast, Overshadowing, Duplication, Telekinesis }

            public class Season {
                [PrimaryKey] public ushort Number { get; set; }
                public DateTime Premiere { get; set; }
                public double Rating { get; set; }
            }
            public class Episode {
                public Season Season { get; set; } = new();
                [PrimaryKey] public short Overall { get; set; }
                public byte Runtime { get; set; }
                public DateTime AirDate { get; set; }
            }

            [PrimaryKey] public string Name { get; set; } = "";
            public Ability Powers { get; set; }
            public int Appearances { get; set; }
            public Episode Debut { get; set; } = new();
        }

        // Test Scenario: Reference Nested Within References as Primary Key (✓recognized✓)
        public class Arson {
            public enum Type { Nature, Building, Vehicle, Creature }

            public class Person {
                [PrimaryKey] public Guid ID { get; set; }
                public ulong SSN { get; set; }
                public string FirstName { get; set; } = "";
                public char? MiddleInitial { get; set; }
                public string? LastName { get; set; }
            }
            public class Criminal {
                [PrimaryKey] public Person Who { get; set; } = new();
                public string Name { get; set; } = "";
                public bool Incarcerated { get; set; }
                public decimal TotalCrimeCost { get; set; }
            }

            [PrimaryKey] public DateTime When { get; set; }
            [PrimaryKey] public Criminal Arsonist { get; set; } = new();
            public Type Variety { get; set; }
            public ulong WaterRequired { get; set; }
            public bool Accelerated { get; set; }
            public decimal Damage { get; set; }
            public float MaxTemperature { get; set; }
        }

        // Test Scenario: Non-Nullable Relations of Non-Nullable Elements (✓recognized✓)
        public class CMakeTarget {
            public enum Mode { Public, Private, Interface };
            public record struct Macro(string Symbol, string Value);

            [PrimaryKey] public string Project { get; set; } = "";
            [PrimaryKey] public string TargetName { get; set; } = "";
            public RelationList<string> Files { get; set; } = new();
            public RelationSet<Macro> Macros { get; set; } = new();
            public RelationMap<Mode, int> OptimizationLevel { get; set; } = new();
            public RelationOrderedList<string> LinkAgainst { get; set; } = new();
        }

        // Test Scenario: Nullable Relations of Non-Nullable Elements (✓recognized✓)
        public class Forecast {
            public enum Extremity { Hurricane, Tornado, Thunderstorm, Blizzard, Hailstorm, Sandstorm }
            public record struct SingleDay(DateTime Date, float HighTemp, float LowTemp, double ChanceRain);

            [PrimaryKey] public string City { get; set; } = "";
            public ulong PersonsImpacted { get; set; }
            public RelationList<SingleDay>? Dailies { get; set; }
            public RelationSet<string>? Meteorologists { get; set; }
            public RelationMap<string, bool>? DataSources { get; set; }
            public RelationOrderedList<Extremity>? ExtremeWeather { get; set; }
        }

        // Test Scenario: Read-Only Relations (✓recognized✓)
        public class CivVIDistrict {
            public enum Terrain { Flat, Grasslands, Marsh, Floodplains, Hills, Desert, Coast, Ocean, Reef, Lake, Mountain }
            public record struct Yield(byte Amount, bool OneTimeOnly, double Multiplier);
            public class CivVIBuilding {
                [PrimaryKey] public string BuildingName { get; set; } = "";
                public ushort ProductionCost { get; set; }
                public ushort GoldCost { get; set; }
                public byte YieldProduction { get; set; }
                public byte YieldGold { get; set; }
                public byte YieldFood { get; set; }
                public double YieldHousing { get; set; }
                public byte YieldFaith { get; set; }
                public byte YieldLoyalty { get; set; }
                public byte YieldCulture { get; set; }
                public byte YieldScience { get; set; }
                public byte YieldAmenities { get; set; }
                public byte GreatWorkSlots { get; set; }
            }

            [PrimaryKey] public string DistrictName { get; set; } = "";
            public ushort ProductionCost { get; set; }
            public string? PrereqTechnology { get; set; }
            public string? PrereqCivic { get; set; }
            public IReadOnlyRelationList<Terrain> AllowedTerrain { get; set; } = new RelationList<Terrain>();
            public IReadOnlyRelationSet<CivVIBuilding> Buildings { get; set; } = new RelationSet<CivVIBuilding>();
            public IReadOnlyRelationMap<int, Yield> Yields { get; set; } = new RelationMap<int, Yield>();
            public IReadOnlyRelationOrderedList<string> Icons { get; set; } = new RelationOrderedList<string>();
        }

        // Test Scenario: Relations Nested Within Aggregates (✓recognized✓)
        public class Gelateria {
            public record struct Owning(DateTime Since, IReadOnlyRelationSet<string> People, decimal LifetimeRevenue);

            [PrimaryKey] public Guid GelateriaID { get; set; }
            public Owning Owners { get; set; }
            public ushort NumFlavors { get; set; }
            public bool AuthenticItalian { get; set; }
            public double InternalTemperature { get; set; }
        }

        // Test Scenario: Relation Nested Within Relation (✗not permitted✗)
        public class BlackHole {
            [PrimaryKey] public string Identifier { get; set; } = "";
            public string? Constellation { get; set; }
            public string? Galaxy { get; set; }
            public RelationSet<string> AKAs { get; set; } = new();
            public RelationMap<string, RelationMap<string, double>> Measurements { get; set; } = new();
        }

        // Test Scenario: Relation Nested Within Aggregate Nested Within Relation (✗not permitted✗)
        public class Poll {
            public record struct Question(string Text, RelationSet<string> Answers);

            [PrimaryKey] public Guid PollID { get; set; }
            public string PollTitle { get; set; } = "";
            public string? Pollster { get; set; }
            public RelationList<Question> Questions { get; set; } = new();
            public ulong Responses { get; set; }
            public double ReponseRate { get; set; }
        }

        // Test Scenario: Relation List/Set of KeyValuePair<X, Y> (✗not permitted - implementation ambiguity✗)
        public class Caricature {
            public enum Location { Circus, Zoo, AmusementPark, SportingEvent, FarmersMarket, Other }

            [PrimaryKey] public Guid ID { get; set; }
            public string Subject { get; set; } = "";
            public string Artist { get; set; } = "";
            public RelationSet<KeyValuePair<DateTime, decimal>> SaleHistory { get; set; } = new();
            public double HeadSize { get; set; }
            public bool Certified { get; set; }
            public Location Source { get; set; }
        }

        // Test Scenario: IRelation (✗not permitted✗)
        public class Perfume {
            [PrimaryKey] public string Brand { get; set; } = "";
            [PrimaryKey] public string Aroma { get; set; } = "";
            public decimal RetailValue { get; set; }
            public DateTime Launched { get; set; }
            public bool ForWomen { get; set; }
            public IRelation PatentNumbers { get; set; } = new RelationSet<ulong>();
        }

        // Test Scenario: Aggregate Consisting of Only Relations (✓recognized✓)
        public class Loch {
            public record struct Geography(RelationMap<char, float> Coordinates, IReadOnlyRelationSet<string> Shires);

            [PrimaryKey] public string Name { get; set; } = "";
            public double Area { get; set; }
            public double Depth { get; set; }
            public double Volume { get; set; }
            public Geography Location { get; set; }
            public bool HomeToNessie { get; set; }
            public ushort Islands { get; set; }
        }
    }

    internal static class EntityShapes {
        // Test Scenario: Record Class (✓allowed✓)
        public record class Color {
            [PrimaryKey] public byte Red { get; set; }
            [PrimaryKey] public byte Green { get; set; }
            [PrimaryKey] public byte Blue { get; set; }
        }

        // Test Scenario: Partial Class (✓allowed✓)
        public partial class PresidentialElection {
            [PrimaryKey] public ushort Year { get; set; }
            public string DemocraticCandidate { get; set; } = "";
            public ulong DemocraticPVs { get; set; }
            public ushort DemocraticEVs { get; set; }
        }
        public partial class PresidentialElection {
            public string RepublicanCandidate { get; set; } = "";
            public ulong RepublicanPVs { get; set; }
            public ushort RepublicanEVs { get; set; }
        }

        // Static Class (✗not permitted✗)
        public static class HighHell {}

        // Test Scenario: Private (✓allowed✓)
        private class GitCommit {
            [PrimaryKey] public string Hash { get; set; } = "";
            public string Author { get; set; } = "";
            public string Message { get; set; } = "";
            public DateTime Timestamp { get; set; }
        }

        // Test Scenario: Internal (✓allowed✓)
        internal class Belt {
            [PrimaryKey] public Guid BeltID { get; set; }
            public decimal Length { get; set; }
            public sbyte NumHoles { get; set; }
            public bool IsBuckled { get; set; }
        }

        // Test Scenario: Struct (✗not permitted✗)
        public struct Carbohydrate {
            [PrimaryKey] public uint Carbon { get; set; }
            [PrimaryKey] public uint Hydrogen { get; set; }
            [PrimaryKey] public uint Oxygen { get; set; }
        }

        // Test Scenario: Record Struct (✗not permitted✗)
        public record struct AminoAcid {
            [PrimaryKey] public char Symbol { get; set; }
            public uint Carbon { get; set; }
            public uint Hydrogen { get; set; }
            public uint Nitrogen { get; set; }
            public uint Oxygen { get; set; }
            public uint Sulfur { get; set; }
        }

        // Test Scenario: Abstract Class (✗not permitted✗)
        public abstract class SuperBowl {
            [PrimaryKey] public ushort Year { get; set; }
            public string HomeTeam { get; set; } = "";
            public string AwayTeam { get; set; } = "";
            public byte HomeScore { get; set; }
            public byte AwayScore { get; set; }
            public bool HomeWins { get; set; }
        }

        // Test Scenario: Generic Type (✗not permitted✗)
        public class Speedometer<TUnit> {
            [PrimaryKey] public long MinSpeed { get; set; }
            [PrimaryKey] public long MaxSpeed { get; set; }
            public string Brand { get; set; } = "";
        }

        // Test Scenario: Interface (✗not permitted✗)
        public interface ILiquor {
            [PrimaryKey] public string Name { get; set; }
            public ushort Proof { get; set; }
            public float AlcoholByVolume { get; set; }
        }

        // Test Scenario: Enumeration (✗not permitted✗)
        public enum Season {
            Winter,
            Spring,
            Summer,
            Fall,
            Autumn = Fall,
        }

        // Test Scenario: Delegate (✗not permitted✗)
        public delegate void SurfingManeuver(object surfboard, float speed, double waveHeight);
    }

    internal static class FieldClusivity {
        // Test Scenario: Zero Identified Fields (✗minimum 2 required✗)
        public class Nothing {}

        // Test Scenario: One Identified Field (✗minimum 2 required✗)
        public class Integer {
            [PrimaryKey] public int Value { get; set; }
        }

        // Test Scenario: Aggregate With No Fields (✗minimum 1 required✗)
        public class TotemPole {
            public struct Festival {}

            [PrimaryKey] public Guid ID { get; set; }
            public float Latitude { get; set; }
            public float Longitude { get; set; }
            public string Culture { get; set; } = "";
            public uint Height { get; set; }
            public DateTime CarvingFinished { get; set; }
            public Festival? Dedication { get; set; }
        }

        // Test Scenario: Non-Public Properties (✓excluded✓)
        public class Animal {
            private string Kingdom { get; set; } = "";
            protected string Phylum { get; set; } = "";
            internal string Class { get; set; } = "";
            protected internal string Order { get; set; } = "";
            private protected string Family { get; set; } = "";
            [PrimaryKey] public string Genus { get; set; } = "";
            [PrimaryKey] public string Species { get; set; } = "";
        }

        // Test Scenario: Public Write-Only Property (✓excluded✓)
        public class ScrabbleTile {
            [PrimaryKey] public char Letter { get; set; }
            public byte Value { get; set; }
            public ushort NumAvailable { set {} }
        }

        // Test Scenario: Public Properties with Non-Public Accessors (✓excluded✓)
        public class ChemicalElement {
            [PrimaryKey] public string Symbol { get; set; } = "";
            public byte AtomicNumber { private get; set; }
            public decimal AtomicWeight { protected get; set; }
            public string Name { internal get; set; } = "";
            public ushort? MeltingPoint { protected internal get; set; }
            public ushort? BoilingPoint { private protected get; set; }
            public sbyte NumAllotropes { get; set; }
        }

        // Test Scenario: Public Static Property (✓excluded✓)
        public class Circle {
            [PrimaryKey] public int CenterX { get; set; }
            [PrimaryKey] public int CenterY { get; set; }
            [PrimaryKey] public ulong Radius { get; set; }
            public static double PI { get; }
        }

        // Test Scenario: Public Indexer (✓excluded✓)
        public class BattingOrder {
            [PrimaryKey] public Guid GameID { get; set; }
            public string Team { get; set; } = "";
            public string this[int position] { get { return ""; } set {} }
        }

        // Test Scenario: [CodeOnly] on Property that would Otherwise be Included (✓excluded✓)
        public class QuadraticEquation {
            [CodeOnly] public string Expression { get; set; } = "";
            [PrimaryKey] public long QuadraticCoefficient { get; set; }
            [PrimaryKey] public long LinearCoefficient { get; set; }
            [PrimaryKey] public long Constant { get; set; }
        }

        // Test Scenario: [CodeOnly] on Relation Property (✓excluded✓)
        public class LazarusPit {
            [PrimaryKey] public Guid PitID { get; set; }
            public string Earth { get; set; } = "";
            public string Location { get; set; } = "";
            public DateTime FirstAppearance { get; set; }
            [CodeOnly] public IReadOnlyRelationSet<string> Resurrections { get; set; } = new RelationSet<string>();
            public bool DiscoveredByRasAlGhul { get; set; }
        }

        // Test Scenario: First Definition of a Virtual Property (✓included✓)
        public class GreekGod {
            [PrimaryKey] public string Name { get; set; } = "";
            public string? RomanEquivalent { get; set; }
            public virtual uint NumChildren { get; set; }
        }

        // Test Scenario: Public Properties Declared by an Interface (✓excluded✓)
        public interface IMultilingual {
            string English { get; set; }
            string French { get; set; }
            string Spanish { get; set; }
        }
        public class Movie : IMultilingual {
            [PrimaryKey] public Guid ID { get; set; }
            public string English { get; set; } = "";
            public string French { get; set; } = "";
            public string Spanish { get; set; } = "";
            public DateTime Release { get; set; }
            public byte Runtime { get; set; }
            public string Director { get; set; } = "";
        }

        // Test Scenario: Public Properties Inherited from a Base Class (✓excluded✓)
        public abstract class Date {
            public byte Day { get; set; }
            public byte Month { get; set; }
            public short Year { get; set; }
        }
        public class Holiday : Date {
            [PrimaryKey] public DateTime Date { get; set; }
            [PrimaryKey] public string Name { get; set; } = "";
        }

        // Test Scenario: Public Property that Overrides Inherited Virtual Property (✓excluded✓)
        public abstract class Address {
            public virtual long Number { get; set; }
            public virtual string Street { get; set; } = "";
            public virtual string City { get; set; } = "";
            public virtual string Country { get; set; } = "";
            public virtual byte? Apartment { get; set; }
        }
        public class POBox : Address {
            public override long Number { get; set; }
            public override string Street { get; set; } = "";
            public override string City { get; set; } = "";
            public override string Country { get; set; } = "";
            public override byte? Apartment { get; set; }
            [PrimaryKey] public uint POBoxNumber { get; set; }
            public string? KnownAs { get; set; }
        }

        // Test Scenario: Public Property that Hides Inherited Property (✓included✓)
        public abstract class Airplane {
            public string Company { get; set; } = "";
            public long Model { get; set; }
            public DateTime FirstFlight { get; set; }
            public ushort Capacity { get; set; }
            public bool InUse { get; set; }
        }
        public class FighterJet : Airplane {
            [PrimaryKey] public string Type { get; set; } = "";
            public string Nickname { get; set; } = "";
            public new DateTime FirstFlight { get; set; }
            public new byte Capacity { get; set; }
        }

        // Test Scenario: Public Indexer Marked as [IncludeInModel] (✗not permitted✗)
        public class Language {
            public string Exonym { get; set; } = "";
            [PrimaryKey] public string Endonym { get; set; } = "";
            public ulong Speakers { get; set; }
            public string ISOCode { get; set; } = "";
            public ushort Letters { get; set; }
            [IncludeInModel] public string this[string word] { get { return ""; } set {} }
        }

        // Test Scenario: Public Write-Only Property Marked as [IncludeInModel (✗not permitted✗)
        public class HebrewPrayer {
            [PrimaryKey] public string Name { get; set; } = "";
            [IncludeInModel] public bool OnShabbat { set {} }
            public string Text { get; set; } = "";
        }

        // Test Scenario: Public Static Property Marked as [IncludeInModel] (✓included✓)
        public class ChessPiece {
            [PrimaryKey] public string Name { get; set; } = "";
            public char Icon { get; set; }
            public byte Value { get; set; }
            [IncludeInModel] public static string FIDE { get; set; } = "FIDE";
        }

        // Test Scenario: Non-Public Properties Marked as [IncludeInModel] (✓included✓)
        public class Song {
            [PrimaryKey] public string Title { get; set; } = "";
            [PrimaryKey] public string Artist { get; set; } = "";
            [IncludeInModel] private string? Album { get; set; } = "";
            [IncludeInModel] protected ushort Length { get; set; }
            [IncludeInModel] internal ushort ReleaseYear { get; set; }
            [IncludeInModel] protected internal double Rating { get; set; }
            [IncludeInModel] private protected byte Grammys { get; set; }
        }

        // Test Scenario: Public Property with Non-Public Accessor Marked as [IncludeInModel] (✓included✓)
        public class Country {
            [PrimaryKey] public string Exonym { get; set; } = "";
            [IncludeInModel] public string Endonym { private get; set; } = "";
            [IncludeInModel] public DateTime IndependenceDay { protected get; set; }
            [IncludeInModel] public ulong Population { internal get; set; }
            [IncludeInModel] public ulong LandArea { protected internal get; set; }
            [IncludeInModel] public ulong Coastline { private protected get; set; }
        }

        // Test Scenario: Public Property Declared by an Interface Marked as [IncludeInModel] (✓included✓)
        public interface ILiterature {
            string Title { get; set; }
            uint PageCount { get; set; }
            uint WordCount { get; set; }
        }
        public class Book : ILiterature {
            [PrimaryKey] public ulong ISBN { get; set; }
            [IncludeInModel] public string Title { get; set; } = "";
            public uint PageCount { get; set; }
            public uint WordCount { get; set; }
        }

        // Test Scenario: Public Property that Overrides Inherited Virtual Property Marked as [IncludeInModel] (✓included✓)
        public abstract class Instrument {
            public string HornbostelSachs { get; set; } = "";
            public virtual string? HighestKey { get; set; }
            public virtual string? LowestKey { get; set; }
        }
        public class Drum : Instrument {
            [PrimaryKey] public string Name { get; set; } = "";
            public bool UseDrumsticks { get; set; }
            [IncludeInModel] public override string? LowestKey { get; set; }
        }

        // Test Scenario: Explicit Interface Implementation Scalar Property Marked as [IncludeInModel] (✓included✓)
        public interface IDiceRoll {
            int NumDice { get; set; }
            int DiceSides { get; set; }
            int Plus { get; set; }
            bool Advantage { get; set; }
            bool Disadvantage { get; set; }
        }
        public class BasicDiceRoll : IDiceRoll {
            [PrimaryKey] public Guid RollID { get; set; }
            [IncludeInModel] int IDiceRoll.NumDice { get; set; }
            [IncludeInModel] int IDiceRoll.DiceSides { get; set; }
            [IncludeInModel] int IDiceRoll.Plus { get; set; }
            [IncludeInModel] bool IDiceRoll.Advantage { get; set; }
            [IncludeInModel] bool IDiceRoll.Disadvantage { get; set; }
        }

        // Test Scenario: Explicit Interface Implementation Aggregate Property Marked as [IncludeInModel] (✓included✓)
        public interface IAthlete {
            public record struct Measurements(double Height, double Weight, DateTime DOB);

            Guid InternationalAthleteIdentifier { get; set; }
            Measurements Bio { get; set; }
        }
        public class Wrestler : IAthlete {
            [IncludeInModel, PrimaryKey] Guid IAthlete.InternationalAthleteIdentifier { get; set; }
            public string BirthName { get; set; } = "";
            public string RingName { get; set; } = "";
            public int WWETitles { get; set; }
            [IncludeInModel] IAthlete.Measurements IAthlete.Bio { get; set; }
        }

        // Test Scenario: Public Property Declared by an Interface Marked as [CodeOnly] (✓redundant✓)
        public interface IWebProtocol {
            int RFC { get; set; }
        }
        public class IPAddress : IWebProtocol {
            [PrimaryKey] public ulong Value { get; set; }
            [PrimaryKey] public ulong Version { get; set; }
            [CodeOnly] public int RFC { get; set; }
        }

        // Test Scenario: Public Property that Overrides Inherited Virtual Property Marked as [CodeOnly] (✓redundant✓)
        public abstract class Vehicle {
            public bool CanFly { get; set; }
            public bool IsAquatic { get; set; }
            public virtual int NumWheels { get; set; }
        }
        public class Submarine : Vehicle {
            [PrimaryKey] public Guid Identifier { get; set; }
            public string Class { get; set; } = "";
            [CodeOnly] public override int NumWheels { get; set; }
            public DateTime Commissioned { get; set; }
            public bool IsActive { get; set; }
            public ushort CrewMembers { get; set; }
            public ulong Weight { get; set; }
        }

        // Test Scenario: Public Write-Only Property Marked as [CodeOnly] (✓redundant✓)
        public class CourtCase {
            [PrimaryKey] public ushort Volume { get; set; }
            [PrimaryKey] public uint CasePage { get; set; }
            [CodeOnly] public ulong Year { set {} }
            public string Plaintiff { get; set; } = "";
            public string Defendant { get; set; } = "";
        }

        // Test Scenario: Non-Public Properties Marked as [CodeOnly] (✓redundant✓)
        public class Lake {
            [PrimaryKey] public decimal Latitude { get; set; }
            [PrimaryKey] public decimal Longitude { get; set; }
            public ulong SurfaceArea { get; set; }
            [CodeOnly] private ulong Depth { get; set; }
            [CodeOnly] protected decimal Salinity { get; set; }
            [CodeOnly] internal ulong DrainageBasin { get; set; }
            [CodeOnly] protected internal string Name { get; set; } = "";
            [CodeOnly] private protected ushort Islands { get; set; }
        }

        // Test Scenario: Public Properties with Non-Public Accessors Marked as [CodeOnly] (✓redundant✓)
        public class Mountain {
            [PrimaryKey] public string Exonym { get; set; } = "";
            [CodeOnly] public string Endonym { private get; set; } = "";
            [CodeOnly] public long Height { protected get; set; }
            [CodeOnly] public ulong Isolation { internal get; set; }
            [CodeOnly] public ushort NumAscentTrails { protected internal get; set; }
            [CodeOnly] public string? Range { private protected get; set; }
            public decimal Latitude { get; set; }
            public decimal Longitude { get; set; }
            public bool SevenSummits { get; set; }
        }

        // Test Scenario: Public Static Property Marked as [CodeOnly] (✓redundant✓)
        public class Tossup {
            [PrimaryKey] public uint ID { get; set; }
            public string LocationCode { get; set; } = "";
            public string SubjectCode { get; set; } = "";
            public string TimeCode { get; set; } = "";
            public string Body { get; set; } = "";
            [CodeOnly] public static byte MinLength { get; set; }
            public static byte MaxLength { get; set; }

        }

        // Test Scenario: Public Indexer Marked as [CodeOnly] (✓redundant✓)
        public class University {
            [PrimaryKey] public string System { get; set; } = "";
            [PrimaryKey] public string Campus { get; set; } = "";
            public ulong UndergradEnrollment { get; set; }
            public ulong GraduateEnrollment { get; set; }
            public ulong Endowment { get; set; }
            [CodeOnly] public int this[int index] { get { return -1; } set {} }
        }

        // Test Scenario: Public, Readable, Instance Property Marked as [IncludeInModel] (✓redundant✓)
        public class Haiku {
            [PrimaryKey] public string Title { get; set; } = "";
            public string Author { get; set; } = "";
            public string Line1 { get; set; } = "";
            [IncludeInModel] public string Line2 { get; set; } = "";
            public string Line3 { get; set; } = "";
        }

        // Test Scenario: Property Marked as [IncludeInModel] and [CodeOnly] (✗conflicting✗)
        public class CreditCard {
            [PrimaryKey] public string Number { get; set; } = "";
            public DateTime Expiration { get; set; }
            [IncludeInModel, CodeOnly] public byte CVV { get; set; }
        }
    }

    internal static class Nullability {
        // Test Scenario: Non-Nullable Scalar Property Marked as [Nullable] (✓becomes nullable✓)
        public class Timestamp {
            [PrimaryKey] public ulong UnixSinceEpoch { get; set; }
            public ushort Hour { get; set; }
            public ushort Minute { get; set; }
            public ushort Second { get; set; }
            [Nullable] public ushort Millisecond { get; set; }
            [Nullable] public ushort Microsecond { get; set; }
            [Nullable] public ushort Nanosecond { get; set; }
        }

        // Test Scenario: Non-Nullable Aggregate Property Marked as [Nullable] (✓cascades as nullable✓)
        public class Bankruptcy {
            public struct Org {
                public string Name { get; set; }
                public DateTime Founded { get; set; }
                public string? TickerSymbol { get; set; }
            }

            [PrimaryKey] public Guid Filing { get; set; }
            [Nullable] public Org Company { get; set; }
            public byte Chapter { get; set; }
            public decimal TotalDebt { get; set; }
            public ulong NumCreditors { get; set; }
        }

        // Test Scenario: Non-Nullable Reference Property Marked as [Nullable] (✓cascades as nullable✓)
        public class Jukebox {
            public class Song {
                [PrimaryKey] public string Title { get; set; } = "";
                [PrimaryKey] public string Singer { get; set; } = "";
                public double Length { get; set; }
            }

            [PrimaryKey] public Guid ProductID { get; set; }
            public ushort NumSongs { get; set; }
            [Nullable] public Song MostPlayed { get; set; } = new();
            public decimal CostPerPlay { get; set; }
            public bool IsDigital { get; set; }
        }

        // Test Scenario: Nullable Scalar Property Marked as [NonNullable] (✓becomes non-nullable✓)
        public class Bone {
            [PrimaryKey] public uint TA2 { get; set; }
            [NonNullable] public string? Name { get; set; }
            public string? LatinName { get; set; } = "";
            public string MeSH { get; set; } = "";
        }

        // Test Scenario: Nullable Aggregate Property Marked as [NonNullable] (✓becomes non-nullable)
        public class Orchestra {
            public record struct Strings(uint? Violins, uint Violas, uint Cellos, uint Basses);
            public record struct Woodwinds(uint Flutes, uint Oboes, uint Clarinets, uint? Saxophones, uint Bassoons);
            public record struct Brass(uint FrenchHorns, uint? Trumpets, uint Trombones, uint Tubas);
            public record struct Instruments(Strings Strings, Woodwinds? Woodwinds, Brass? Brass);

            [PrimaryKey] public Guid ID { get; set; }
            public string Name { get; set; } = "";
            [NonNullable] public Instruments? Composition { get; set; }
        }

        // Test Scenario: Nullable Reference Property Marked as [NonNullabe] (✓becomes non-nullable)
        public class Bodhisattva {
            public enum Denomination { Nikaya, Theravada, Mahayana }

            public class Bhumi {
                [PrimaryKey] public string English { get; set; } = "";
                public string? Sanskrit { get; set; }
                public string Description { get; set; } = "";
            }

            [PrimaryKey] public string Name { get; set; } = "";
            public Denomination Buddhism { get; set; }
            [NonNullable] public Bhumi? LastBhumi { get; set; }
            public DateTime DateOfBirth { get; set; }
            public DateTime DateOfDeath { get; set; }

        }

        // Test Scenario: Nullable Scalar Property Marked as [Nullable] (✓redundant✓)
        public class CivMilitaryUnit {
            [PrimaryKey] public string Identifier { get; set; } = "";
            [Nullable] public string? Promotion { get; set; } = "";
            public byte MeleeStrength { get; set; }
            public byte? RangedStrength { get; set; }
            public bool IsUnique { get; set; }
        }

        // Test Scenario: Non-Nullable Scalar Property Marked as [NonNullable] (✓redundant✓)
        public class Patent {
            [PrimaryKey] public ulong DocumentID { get; set; }
            [NonNullable] public DateTime PublicationDate { get; set; }
            public string? Description { get; set; }
            public ulong ApplicationNumber { get; set; }
        }

        // Test Scenario: Property Marked as [Nullable] and [NonNullable] (✗conflicting✗)
        public class RetailProduct {
            [PrimaryKey] public Guid ID { get; set; }
            public string Name { get; set; } = "";
            public string Description { get; set; } = "";
            public decimal BasePrice { get; set; }
            [Nullable, NonNullable] public decimal SalePrice { get; set; }
            public ulong StockCount { get; set; }
            public uint CategoryID { get; set; }
        }

        // Test Scenario: Aggregate Property with Only Nullable Fields is Nullable (✗ambiguous✗)
        public class Waffle {
            public record struct AddOns(string? Chocolate, string? Fruit, string? Syrup, string? Nuts);
            public enum Shape { Circular, Square, Rectangular, Novelty };

            [PrimaryKey] public Guid WaffleID { get; set; }
            public AddOns? Toppings { get; set; }
            public Shape Design { get; set; }
            public bool BelgianStyle { get; set; }
            public bool GlutenFree { get; set; }
        }

        // Test Scenario: Aggregate Property with Only Nullable Fields is Marked as [Nullable] (✗ambiguous✗)
        public class iPhone {
            public record struct SemVer(int? Major, int? Minor, int? Patch, string? PreRelease, long? Build);

            [PrimaryKey] public Guid ProductID { get; set; }
            public SemVer Version { get; set; }
            [Nullable] public SemVer iOSVersion { get; set; }
            public bool HeadphoneJack { get; set; }
            public double AverageBatteryLife { get; set; }
        }

        // Test Scenario: Relations with Nullable Element Types (✓become nullable✓)
        public class PostOffice {
            public record struct LicensePlate(string Number, string State);
            public record struct Stamp(Guid StampID, decimal Price);

            [PrimaryKey] public Guid ID { get; set; }
            public string Address { get; set; } = "";
            public ulong MailVolume { get; set; }
            public RelationMap<short, string?> POBoxes { get; set; } = new();
            public RelationSet<string?> Employees { get; set; } = new();
            public RelationList<LicensePlate?> MailTrucks { get; set; } = new();
            public RelationMap<DateTime?, Stamp?> Stamps { get; set; } = new();
            public RelationOrderedList<decimal?> Budgets { get; set; } = new();
        }

        // Test Scenario: Relation with Nullable Aggregate Element Type with Only Nullable Fields (✗ambiguous✗)
        public class Parabola {
            public enum Direction { Up, Down, Left, Right }
            public record struct Point(int X, int Y);
            public record struct MaybePoint(int? X, int? Y);

            [PrimaryKey] public Point Vertex { get; set; }
            [PrimaryKey] public float Eccentricity { get; set; }
            public Direction Concavity { get; set; }
            public RelationSet<MaybePoint?> Points { get; set; } = new();
        }

        // Test Scenario: Relation Property Marked as [NonNullable] (✓redundant✓)
        public class Squintern {
            public record struct Episode(uint Season, ushort Number, string Title);

            [PrimaryKey] public string FirstName { get; set; } = "";
            [PrimaryKey] public string LastName { get; set; } = "";
            public bool IsFemale { get; set; }
            [NonNullable] public RelationList<Episode> Appearances { get; set; } = new();
            public bool HasTemperancesApproval { get; set; }
        }

        // Test Scenario: Relation Property Marked as [Nullable] (✗illegal✗)
        public class Axiom {
            [PrimaryKey] public string Name { get; set; } = "";
            public string Formulation { get; set; } = "";
            public string? PostulatedBy { get; set; }
            public bool IsLogical { get; set; }
            [Nullable] public RelationSet<string> DerivedTheories { get; set; } = new();
        }
    }

    internal static class TableNaming {
        // Test Scenario: New Principal Table Name (✓renamed✓)
        [Table("DeckOfCards")]
        public class PlayingCard {
            [PrimaryKey] public byte Suit { get; set; }
            [PrimaryKey] public byte Value { get; set; }
        }

        // Test Scenario: Namespace Excluded with No Relations (✓renamed✓)
        [ExcludeNamespaceFromName]
        public class Pokemon {
            [PrimaryKey] public ushort PokedexNumber { get; set; }
            public string PrimaryType { get; set; } = "";
            public string? SecondaryType { get; set; }
            public string Name { get; set; } = "";
            public string JapaneseName { get; set; } = "";
            public byte HP { get; set; }
        }

        // Test Scenario: Namespace Excluded with Relations (✓renamed✓)
        [ExcludeNamespaceFromName]
        public class PrisonerExchange {
            [PrimaryKey] public Guid SwapID { get; set; }
            public string PartyA { get; set; } = "";
            public string PartyB { get; set; } = "";
            public RelationSet<ushort> AtoB { get; set; } = new();
            public RelationSet<ushort> BtoA { get; set; } = new();
            public DateTime Executed { get; set; }
            public bool Covert { get; set; }
        }

        // Test Scenario: Two Entities Given Same Principal Table Name (✗duplication✗)
        [Table("Miscellaneous")]
        public class Flight {
            [PrimaryKey] public Guid ID { get; set; }
            public string Airline { get; set; } = "";
            public DateTime Departure { get; set; }
            public DateTime Arrival { get; set; }
            public string FromAirport { get; set; } = "";
            public string ToAirport { get; set; } = "";
            public byte Capacity { get; set; }
        }
        [Table("Miscellaneous")]
        public class Battle {
            [PrimaryKey] public string Name { get; set; } = "";
            [PrimaryKey] public string War { get; set; } = "";
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public string WinningCommander { get; set; } = "";
            public string LosingCommander { get; set; } = "";
            public ulong Casualties { get; set; }
        }

        // Test Scenario: Name Unchanged via [Table] (✓redundant✓)
        [Table("UT.Kvasir.Translation.TableNaming+BookmarkTable")]
        public class Bookmark {
            [PrimaryKey] public string URL { get; set; } = "";
            public bool IsFavorite { get; set; }
            public string Icon { get; set; } = "";
            public bool IsOnDesktop { get; set; }
        }

        // Test Scenario: Name Changed to `null` (✗illegal✗)
        [Table(null!)]
        public class SmokeDetector {
            [PrimaryKey] public Guid DetectorID { get; set; }
            public DateTime Installed { get; set; }
            public DateTime LastSericed { get; set; }
            public double Sensitivity { get; set; }
            public bool DetectsViaIonization { get; set; }
        }

        // Test Scenario: Name Changed to the Empty String (✗illegal✗)
        [Table("")]
        public class LogIn {
            [PrimaryKey] public string Username { get; set; } = "";
            public string Password { get; set; } = "";
        }

        // Test Scenario: Principal Table Name Change and [ExcludeNamespaceFromName], Equivalent (✗illegal✗)
        [Table("UT.Kvasir.Translation.TableNaming+"), ExcludeNamespaceFromName]
        public class Umbrella {
            [PrimaryKey] public Guid ProductID { get; set; }
            public double WaterResistance { get; set; }
            public bool ButtonActivated { get; set; }
            public float Height { get; set; }
            public float Span { get; set; }
        }

        // Test Scenario: Principal Table Name Change and [ExcludeNamespaceFromName], Prefixed (✓renamed and removed✓)
        [Table("UT.Kvasir.Translation.TableNaming+BlenderTable"), ExcludeNamespaceFromName]
        public class Blender {
            [PrimaryKey] public Guid ProductID { get; set; }
            public string Brand { get; set; } = "";
            public double PulseSpeed { get; set; }
            public byte NumBlades { get; set; }
        }

        // Test Scenario: Principal Table Name Change and [ExcludeNamespaceFromName], Infixed (✓latter redundant✓)
        [Table("Database.UT.Kvasir.Translation.TableNaming+BoardingSchoolTable"), ExcludeNamespaceFromName]
        public class BoardingSchool {
            [PrimaryKey] public Guid SchoolID { get; set; }
            public string Name { get; set; } = "";
            public ulong Enrollment { get; set; }
            public bool BoysOnly { get; set; }
            public bool MilitaryStyle { get; set; }
            public decimal Tuition { get; set; }
            public string Address { get; set; } = "";
            public byte UpperAge { get; set; }
        }

        // Test Scenario: Principal Table Name Change and [ExcludeNamespaceFromName], Suffixed (✓latter redundant✓)
        [Table("Polygraph.UT.Kvasir.Translation.TableNaming+"), ExcludeNamespaceFromName]
        public class PolygraphTest {
            [PrimaryKey] public DateTime Date { get; set; }
            [PrimaryKey] public string Subject { get; set; } = "";
            public string Administrator { get; set; } = "";
            public double Duration { get; set; }
            public uint QuestionsAsked { get; set; }
            public uint Truths { get; set; }
            public bool AdmissibleInCourt { get; set; }
        }

        // Test Scenario: Principal Table Name Change and [ExcludeNamespaceFromName], No Overlap (✓latter redundant✓)
        [Table("SomeTable"), ExcludeNamespaceFromName]
        public class Encryption {
            [PrimaryKey] public string Scheme { get; set; } = "";
            public ulong PublicKey { get; set; }
            public ulong PrivateKey { get; set; }
        }

        // Test Scenario: New Relation Table Name (✓renamed✓)
        public class MagicalPreserve {
            [PrimaryKey] public string Name { get; set; } = "";
            [RelationTable("CreaturesTable")] public RelationMap<string, int> Population { get; set; } = new();
            public string SecretArtifact { get; set; } = "";
            public DateTime? Founding { get; set; }
            public string PrimaryCaretaker { get; set; } = "";
            public double Area { get; set; }
        }

        // Test Scenario: Relation Table Name Unchanged via [RelationTable] (✓redundant✓)
        public class PacerTest {
            public class Student {
                [PrimaryKey] public Guid StudentID { get; set; }
                public string FirstName { get; set; } = "";
                public string LastName { get; set; } = "";
            }

            [PrimaryKey] public string School { get; set; } = "";
            [PrimaryKey] public ushort Year { get; set; }
            [RelationTable("UT.Kvasir.Translation.TableNaming+PacerTest.LapsCompletedTable")] public RelationMap<Student, byte> LapsCompleted { get; set; } = new();
            public DateTime AdministeredOn { get; set; }
        }

        // Test Scenario: Relation Table Name Changed to `null` (✗illegal✗)
        public class Dwarf {
            [PrimaryKey] public Guid DwarfID { get; set; }
            public bool IsFictional { get; set; }
            public string Name { get; set; } = "";
            [RelationTable(null!)] public RelationMap<DateTime, string> LifeEvents { get; set; } = new();
            public byte Height { get; set; }
            public bool WieldsSword { get; set; }
            public double ShoeSize { get; set; }
        }

        // Test Scenario: Relation Table Named Changed to the Empty String (✗illegal✗)
        public class Rodent {
            public enum Taxon { Domain, Kingdom, Phylum, Class, Infraclass, Superorder, Order, Family, Subfamily, Genus, Species, Subspecies }
            public enum IUCN { Extinct, CaptivityOnly, CriticallyEndangered, Endangered, Vulnerable, NearThreatened, LeastConcern }

            [PrimaryKey] public string CommonName { get; set; } = "";
            [RelationTable("")] public RelationMap<Taxon, string> Taxonomy { get; set; } = new();
            public double AverageAdultMaleWeight { get; set; }
            public double AverageAdultFemaleWeight { get; set; }
            public float TailLength { get; set; }
            public float IncisorLength { get; set; }
            public bool MakesGoodPet { get; set; }
            public IUCN ConservationStatus { get; set; }
            public ulong GlobalPopulation { get; set; }
        }

        // Test Scenario: Duplicate Relation Table Name (✗duplication✗)
        public class Vowel {
            public enum Location { Front, Central, Back }
            [Flags] public enum Feature { Raspy = 1, Nasal = 2, Whispered = 4, Creaky = 8, Devoiced = 16 }

            [PrimaryKey] public char IPASymbol { get; set; }
            public Location Articulation { get; set; }
            public bool Closed { get; set; }
            public bool Long { get; set; }
            public Feature Features { get; set; }
            [RelationTable("AuxiliaryVowelTable")] public RelationSet<string> Languages { get; set; } = new();
            [RelationTable("AuxiliaryVowelTable")] public RelationMap<char, char> Diacritics { get; set; } = new();
        }

        // Test Scenario: Table Name Duplicated between Principal Table and Relation Table (✗duplication✗)
        [Table("OfficialInfoVPN")]
        public class VPN {
            public enum Kind { Private, SiteToSite, Extranet, Other }

            [PrimaryKey] public string IP { get; set; } = "";
            public Kind Type { get; set; }
            public bool PasswordProtected { get; set; }
            [RelationTable("OfficialInfoVPN")] public RelationList<Guid> AuthorizedUsers { get; set; } = new();
        }

        // Test Scenario: [RelationTable] Applied to Numeric Field (✗impermissible✗)
        public class Shofar {
            [PrimaryKey] public Guid ShofarID { get; set; }
            public string Maker { get; set; } = "";
            public double Weight { get; set; }
            public ushort NumTurns { get; set; }
            [RelationTable("---")] public float Tekiah { get; set; }
            public float Teruah { get; set; }
            public float Shevarim { get; set; }
            public float TekiahGedolah { get; set; }
        }

        // Test Scenario: [RelationTable] Applied to Textual Field (✗impermissible✗)
        public class LawnGnome {
            public record struct Color(byte R, byte G, byte B);

            [PrimaryKey] public Guid ProductID { get; set; }
            public ushort Height { get; set; }
            public decimal Price { get; set; }
            [RelationTable("---")] public string Manufacturer { get; set; } = "";
            public Color HatColor { get; set; }
            public bool HoldingTools { get; set; }
            public bool IsWaterSpout { get; set; }
        }

        // Test Scenario: [RelationTable] Applied to Boolean Field (✗impermissible✗)
        public class GovernmentShutdown {
            [PrimaryKey] public DateTime Start { get; set; }
            [PrimaryKey] public DateTime End { get; set; }
            [RelationTable("---")] public bool RepublicansInCharge { get; set; }
            public decimal EventualBudget { get; set; }
            public ulong FurloughedWorkers { get; set; }
            public double PublicApproval { get; set; }
        }

        // Test Scenario: [RelationTable] Applied to DateTime Field (✗impermissible✗)
        public class CoalMine {
            [PrimaryKey] public string MineName { get; set; } = "";
            public string Operator { get; set; } = "";
            public ushort TotalMiners { get; set; }
            public ulong ShortTons { get; set; }
            public DateTime Opened { get; set; }
            [RelationTable("---")] public DateTime? LastCollapse { get; set; }
            public float Latitude { get; set; }
            public float Longitude { get; set; }
        }

        // Test Scenario: [RelationTable] Applied to Guid Field (✗impermissible✗)
        public class LawnMower {
            [PrimaryKey, RelationTable("---")] public Guid ApplianceID { get; set; }
            public double Horsepower { get; set; }
            public ushort WarrantyLength { get; set; }
            public double GrassVolue { get; set; }
            public bool IsGasPowered { get; set; }
            public sbyte NumBlades { get; set; }
        }

        // Test Scenario: [RelationTable] Applied to Enumeration Field (✗impermissible✗)
        public class Triplets {
            public enum Cardinality { Monozygotic, Dizygotic, Trizygotic, Sesquizygotic }

            [PrimaryKey] public string Mother { get; set; } = "";
            [PrimaryKey] public sbyte PregnancyNumber { get; set; }
            public bool Surrogate { get; set; }
            [RelationTable("---")] public Cardinality Zygosity { get; set; }
            public string EldestName { get; set; } = "";
            public string MiddleName { get; set; } = "";
            public string YoungestName { get; set; } = "";
        }

        // Test Scenario: [RelationTable] Applied to Aggregate Field (✗impermissible✗)
        public class Toothbrush {
            public record struct Electricity(double Power, int Duration, bool Vibrating);

            [PrimaryKey] public Guid ProductID { get; set; }
            public string Brand { get; set; } = "";
            public string Make { get; set; } = "";
            public string Model { get; set; } = "";
            [RelationTable("---")] public Electricity? Electric { get; set; }
            public double Rating { get; set; }
        }

        // Test Scenario: [RelationTable] Applied to Reference Field (✗impermissible✗)
        public class Valet {
            public class Organization {
                [PrimaryKey] public Guid CompanyID { get; set; }
                public string Name { get; set; } = "";
                public bool IsNonProfit { get; set; }
                public ushort TotalEmployees { get; set; }
            }

            [PrimaryKey] public Guid ValetID { get; set; }
            public string Location { get; set; } = "";
            [RelationTable("---")] public Organization Company { get; set; } = new();
            public ushort Capacity { get; set; }
            public double AverageDistance { get; set; }
            public decimal CostPerCar { get; set; }
            public bool TippingExpected { get; set; }
        }
    }

    internal static class FieldNaming {
        // Test Scenario: Non-Pascal-Cased Names (✓allowed✓)
        public class Surah {
            [PrimaryKey] public string _EnglishName { get; set; } = "";
            public string __ArabicName { get; set; } = "";
            public decimal juz_start { get; set; }
            public decimal juzEnd { get; set; }
        }

        // Test Scenario: Change Scalar Field Name to New Value (✓renamed✓)
        public class River {
            [PrimaryKey] public string Name { get; set; } = "";
            [Name("SourceElevation")] public ushort Ahuiehknaafuyur { get; set; }
            [Name("Length")] public ushort OEperaehrugyUIWJKuygajk { get; set; }
            public decimal MouthLatitude { get; set; }
            public decimal MouthLongitude { get; set; }
        }

        // Test Scenario: Change Aggregate Field Name (✓renamed✓)
        public class BorderCrossing {
            public record struct Coordinate(float Latitude, float Longitude);

            [PrimaryKey] public string Name { get; set; } = "";
            [Name("Degrees")] public Coordinate Location { get; set; }
            public string CountryA { get; set; } = "";
            public string CountryB { get; set; } = "";
            public ulong Length { get; set; }
            public ulong YearlyCrossings { get; set; }
            public bool IsDriveable { get; set; }
        }

        // Test Scenario: Change Aggregate-Nested Field Name to New Value (✓renamed✓)
        public class Ziggurat {
            public record struct Civilization(string Name, string Location);

            [PrimaryKey] public Guid ZigguratID { get; set; }
            public ulong Height { get; set; }
            public ushort NumTerraces { get; set; }
            public uint NumSteps { get; set; }
            [Name("CivWho", Path = "Name"), Name("CivWhere", Path = "Location")] public Civilization BuiltBy { get; set; }
        }

        // Test Scenario: Change Nested Aggregate Name (✓renamed✓)
        public class DogShow {
            public record struct Creature(string Genus, string Species, string Common);
            public record struct Dog(string Name, Creature Species);

            [PrimaryKey] public ushort Year { get; set; }
            [PrimaryKey] public string Sponsor { get; set; } = "";
            public ulong Participants { get; set; }
            [Name("Breed", Path = "Species")] public Dog BestInShow { get; set; }
        }

        // Test Scenario: Complex Series of Name Changes with Nested Aggregates (✓renamed✓)
        public class Cliff {
            public struct Coordinate {
                [Name("LAT")] public float Latitude { get; set; }
                [Name("LONG")] public float Longitude { get; set; }
            }
            public struct Polity {
                public string Name { get; set; }
                public string? SubLocale { get; set; }
                [Name("LATITUDE", Path = "Latitude"), Name("GridIntersection")] public Coordinate Coordinate { get; set; }
            }
            public struct Location {
                [Name("CityName", Path = "Name")] public Polity City { get; set; }
                public Polity? Polity { get; set; }
                public string Country { get; set; }
            }
            public struct Site {
                [Name("GeoCity", Path = "City"), Name("PolitySubLocale", Path = "Polity.SubLocale")] public Location Location { get; set; }
                public ushort NumEntrances { get; set; }
            }

            [PrimaryKey] public Guid CliffID { get; set; }
            [Name("PolityName", Path = "Location.Polity.Name"), Name("GeoPolity", Path = "Location.Polity")] public Site Place { get; set; }
            public ulong Height { get; set; }
            public double SheerAngle { get; set; }
            public bool IsUNESCO { get; set; }
            public string PrimaryStone { get; set; } = "";
        }

        // Test Scenario: Change Reference Field Name (✓renamed✓)
        public class Ballerina {
            public class Ballet {
                [PrimaryKey] public Guid BalletID { get; set; }
                public string Title { get; set; } = "";
                public ulong Length { get; set; }
            }

            [PrimaryKey] public uint SSN { get; set; }
            public string FirstName { get; set; } = "";
            public string LastName { get; set; } = "";
            public double Height { get; set; }
            public byte ShoeSize { get; set; }
            [Name("DebutBallet")] public Ballet Debut { get; set; } = new();
        }

        // Test Scenario: Change Reference-Nested Field Name to New Value (✓renamed✓)
        public class DMZ {
            public enum LineType { Latitude, Longitude }
            public enum Direction { North, South, East, West }

            public class Location {
                [PrimaryKey] public double Measurement { get; set; }
                [PrimaryKey] public LineType LatLong { get; set; }
                [PrimaryKey] public Direction Dir { get; set; }
            }

            [PrimaryKey] public string DMZName { get; set; } = "";
            public double Length { get; set; }
            [Name("Value", Path = "Measurement"), Name("Definition.Lat_or_Long", Path = "LatLong")] public Location Definition { get; set; } = new();
            public string OverseenBy { get; set; } = "";
            public DateTime Established { get; set; }
        }

        // Test Scenario: Change Nested Reference Name (✓renamed✓)
        public class Carnival {
            public class Carny {
                [PrimaryKey] public int ID { get; set; }
                [PrimaryKey] public string Title { get; set; } = "";
                public double Height { get; set; }
                public decimal Salary { get; set; }
                public uint YearsExperience { get; set; }
            }
            public record struct Staff(Carny HeadCarny, Carny Zookeeper, Carny Janitor, Carny Spokesperson);

            [PrimaryKey] public Guid CarnivalID { get; set; }
            public string CarnivalName { get; set; } = "";
            public string City { get; set; } = "";
            public bool IsTravelling { get; set; }
            [Name("SanitationLord", Path = "Janitor")] public Staff CarnivalStaff { get; set; }
            public decimal PopcornCost { get; set; }
            public ushort NumTents { get; set; }
        }

        // Test Scenario: Change Relation Field Name (✓affects name of Table✓)
        public class KidneyStone {
            [PrimaryKey] public Guid KidneyStoneID { get; set; }
            public float Size { get; set; }
            public string Patient { get; set; } = "";
            [Name("MaterialsTable")] public RelationSet<string> Composition { get; set; } = new();
            public bool HasPassed { get; set; }
        }

        // Test Scenario: Change Relation-Nested Field Name to New Value (✓renamed✓)
        public class SwissCanton {
            [PrimaryKey] public Guid ID { get; set; }
            [Name("Canton", Path = "SwissCanton")] public RelationMap<string, string> Names { get; set; } = new();
            public ulong Area { get; set; }
            public ulong Population { get; set; }
            public string CantonCapital { get; set; } = "";
            public ushort YearJoinedSwissConfederation { get; set; }
            [Name("CantonID", Path = "SwissCanton.ID"), Name("Councilor", Path = "Item")] public RelationSet<string> Councilors { get; set; } = new();
            [Name("Religion", Path = "Key"), Name("%PCNT", Path = "Value")] public RelationMap<string, double> Religions { get; set; } = new();
        }

        // Test Scenario: Change Nested Relation Name (✓affects name of Table✓)
        public class Gulag {
            public enum Org { Cheka, GPU, OGPU, KNVD, InternalAffairs, StalinsPersonalGuard }
            public record struct Naming(string English, string Russian);
            public record struct Personnel(string Commandant, RelationList<string> Overseers, Org AdministeredBy);

            [PrimaryKey] public Guid GulagID { get; set; }
            public Naming Name { get; set; }
            public ulong Laborers { get; set; }
            [Name("GulagOverseers", Path = "Overseers")] public Personnel Management { get; set; }
            public float Latitude { get; set; }
            public float Longitude { get; set; }
        }

        // Test Scenario: Swap Names of Fields (✓renamed✓)
        public class Episode {
            [PrimaryKey, Name("Number")] public byte Season { get; set; }
            [PrimaryKey, Name("Season")] public short Number { get; set; }
            public float Length { get; set; }
            public int? Part { get; set; }
            public string Title { get; set; } = "";
        }

        // Test Scenario: Name Collision from Explicit Interface Implementation (✗duplication✗)
        public interface IProtein {
            int CaloriesPerGram { get; set; }
        }
        public class GroundMeat : IProtein {
            [PrimaryKey] public Guid ID { get; set; }
            public string Kind { get; set; } = "";
            public double PercentFat { get; set; }
            [IncludeInModel] int IProtein.CaloriesPerGram { get; set; }
            public ushort CaloriesPerGram { get; set; }
            public bool IsSausage { get; set; }
        }

        // Test Scenario: Change Field Name to Existing Value in Principal Table (✗duplication✗)
        public class ComputerLock {
            [PrimaryKey] public string Name { get; set; } = "";
            public bool IsReentrant { get; set; }
            [Name("IsReentrant")] public bool IsRecursive { get; set; }
            public ushort? ReadersPermitted { get; set; }
            public ushort? WritersPermitted { get; set; }
        }

        // Test Scenario: Change Two Field Names to Same Value in Principal Table (✗duplication✗)
        public class Ticket2RideRoute {
            [PrimaryKey, Name("Destination")] public string City1 { get; set; } = "";
            [PrimaryKey, Name("Destination")] public string City2 { get; set; } = "";
            public byte Points { get; set; }
        }

        // Test Scenario: Change Field Name to Existing Value in Relation Table (✗duplication✗)
        public class Cookbook {
            public class Recipe {
                public record struct Measure(double Value, string Unit);
                
                [PrimaryKey] public Guid RecipeID { get; set; }
                public string DishName { get; set; } = "";
                public ushort PrepTime { get; set; }
                public ushort CookTime { get; set; }
                public RelationMap<string, Measure> Ingredients { get; set; } = new();
                public RelationOrderedList<string> Steps { get; set; } = new();
            }

            [PrimaryKey] public string ISBN { get; set; } = "";
            public string Title { get; set; } = "";
            public string Author { get; set; } = "";
            public DateTime PublicationDate { get; set; }
            public string AmazonURL { get; set; } = "";
            [Name("Cookbook.ISBN", Path = "Item.RecipeID")] public RelationList<Recipe> Recipes { get; set; } = new();
        }

        // Test Scenario: Change Two Field Names to Same Value in Relation Table (✗duplication✗)
        public class HostageSituation {
            public class Person {
                [PrimaryKey] public uint SSN { get; set; }
                public string FirstName { get; set; } = "";
                public string? MiddleName { get; set; }
                public string? LastName { get; set; }
            }

            [PrimaryKey] public Guid IncidentID { get; set; }
            public string Location { get; set; } = "";
            public Person HostageTaker { get; set; } = new();
            [Name("Value", Path = "HostageSituation.IncidentID"), Name("Value", Path = "Item.SSN")] public RelationList<Person> Hostages { get; set; } = new();
            public ushort Casualties { get; set; }
        }

        // Test Scenario: Scalar Property with Multiple Identical [Name] Changes (✓de-duplicated✓)
        public class Antiparticle {
            [PrimaryKey] public string Name { get; set; } = "";
            public double Spin { get; set; }
            public int Charge { get; set; }
            [Name("Counterpart"), Name("Counterpart")] public string Particle { get; set; } = "";
            public decimal Mass { get; set; }
            public string? DiscoveredBy { get; set; }
        }

        // Test Scenario: Scalar Property with Multiple Different [Name] Changes (✗cardinality✗)
        public class BankAccount {
            public string Bank { get; set; } = "";
            [PrimaryKey] public string AccountNumber { get; set; } = "";
            [Name("Route"), Name("RoutingNumber")] public ulong RoutingNumber { get; set; }
        }

        // Test Scenario: Scalar Property with Redundant and Non-Redundant [Name] Changes (✗cardinality✗)
        public class Billionaire {
            [PrimaryKey] public Guid BillionaireID { get; set; }
            public decimal NetWorth { get; set; }
            public double PercentCash { get; set; }
            public double PercentStock { get; set; }
            public double PercentAssets { get; set; }
            [Name("FirstReached"), Name("When?")] public DateTime FirstReached { get; set; }
            public uint WorldRanking { get; set; }
        }

        // Test Scenario: Aggregate Property with Multiple Identical [Name] Changes (✓de-duplicated✓)
        public class Militia {
            public record struct Personnel(int Generals, int Colonels, int Lieutenants, int Privates, int Corporals);

            [PrimaryKey] public Guid MilitiaID { get; set; }
            public string? Name { get; set; }
            public DateTime Created { get; set; }
            public DateTime? Disbanded { get; set; }
            [Name("Members"), Name("Members")] public Personnel Roster { get; set; }
            public bool WellRegulated { get; set; }
        }

        // Test Scenario: Aggregate Property with Multiple Different [Name] Changes (✗cardinality✗)
        public class Walkabout {
            public record struct Coordinate(float Latitude, float Longitude);

            [PrimaryKey] public Guid ID { get; set; }
            public string Participant { get; set; } = "";
            public string AboriginalGroup { get; set; } = "";
            public DateTime Start { get; set; }
            public DateTime? End { get; set; }
            [Name("StartLoc"), Name("StartingLoc")] public Coordinate InitialLocation { get; set; }
            public Coordinate EndingLocation { get; set; }
        }

        // Test Scenario: Aggregate Property with Redundant and Non-Redundant [Name] Changes (✗cardinality✗)
        public class Treadmill {
            public record struct Company(string ShortName, string LongName);

            [PrimaryKey] public Guid ProductID { get; set; }
            [Name("Manufacturer"), Name("ManufacturingCompany")] public Company Manufacturer { get; set; }
            public double MaxSpeed { get; set; }
            public double MaxIncline { get; set; }
            public DateTime WarrantyExpiration { get; set; }
            public ulong HoursUsed { get; set; }
            public bool CanReverse { get; set; }
        }

        // Test Scenario: Reference Property with Multiple Identical [Name] Changes (✓de-duplicated✓)
        public class MongolKhan {
            public class City {
                [PrimaryKey] public string Name { get; set; } = "";
                public float Latitude { get; set; }
                public float Longitude { get; set; }
                public bool StillStanding { get; set; }
            }

            [PrimaryKey] public string Name { get; set; } = "";
            public DateTime ReignStart { get; set; }
            public DateTime ReignEnd { get; set; }
            public ushort Children { get; set; }
            public ulong LivingDescendants { get; set; }
            [Name("CapitalCity"), Name("CapitalCity")] public City Capital { get; set; } = new();
        }

        // Test Scenario: Reference Property with Multiple Different [Name] Changes (✗cardinality✗)
        public class QuizBowlProtest {
            public enum Type { Short, Tossup, Bonus, Lightning }

            public class Question {
                [PrimaryKey] public Guid QuestionID { get; set; }
                public ushort WriterID { get; set; }
                public string Category { get; set; } = "";
                public string AnswerLine { get; set; } = "";
                public Type QuestionType { get; set; }
            }

            public record struct PacketQuestion(string Tournament, Question Question, byte Packet, byte Number);

            [PrimaryKey] public Guid ProtetID { get; set; }
            [Name("Q", Path = "Question"), Name("Which?", Path = "Question")] public PacketQuestion ProtestedQuestion { get; set; }
            public DateTime TimeLodged { get; set; }
            public string Argument { get; set; } = "";
            public DateTime? TimeAdjudicated { get; set; }
            public bool Rejected { get; set; }
            public bool Moot { get; set; }
        }

        // Test Scenario: Reference Property with Redundant and Non-Redundant [Name] Changes (✗cardinality✗)
        public class Grassland {
            public class Grass {
                [PrimaryKey] public string Genus { get; set; } = "";
                [PrimaryKey] public string Species { get; set; } = "";
            }

            [PrimaryKey] public Guid GrasslandID { get; set; }
            public string? Name { get; set; }
            [Name("DominantGrass"), Name("MainGrass")] public Grass DominantGrass { get; set; } = new();
            public double BiodiversityRating { get; set; }
            public ulong NativeAnimalSpecies { get; set; }
            public ulong NativePlantSpecies { get; set; }
            public double AverageTemperature { get; set; }
            public double AverageRainfall { get; set; }
        }

        // Test Scenario: Relation Property with Multiple Identical [Name] Changes (✓de-duplicated✓)
        public class Necromancer {
            public enum AlignedAs { LawfulGood, LawfulEvil, LawfulNeutral, TrueGood, TrueEvil, TrueNeutral, ChaoticGood, ChaoticEvil, ChaoticNeutral }

            public class Spell {
                [PrimaryKey] public string Name { get; set; } = "";
                public string Incantation { get; set; } = "";
                public RelationSet<string> Components { get; set; } = new();
                public bool TimeSensitive { get; set; }
            }

            [PrimaryKey] public Guid MagicUserID { get; set; }
            public string? Name { get; set; }
            public AlignedAs Alignment { get; set; }
            [Name("Spellbook"), Name("Spellbook")] public RelationSet<Spell> Spells { get; set; } = new();
            public ulong Resurrections { get; set; }
            public byte Level { get; set; }
        }

        // Test Scenario: Relation Property with Multiple Different [Name] Changes (✗cardinality✗)
        public class Genocide {
            public enum Event { Start, End }

            [PrimaryKey] public Guid GenocideID { get; set; }
            public string Identifier { get; set; } = "";
            public string EthnicGroup { get; set; } = "";
            public ulong DeathCount { get; set; }
            [Name("TimelineOfEvents"), Name("Calendar")] public RelationMap<DateTime, Event> Timeline { get; set; } = new();
            public ushort ICCIndictments { get; set; }
        }

        // Test Scenario: Relation Property with Redundant and Non-Redundant [Name] Changes (✗cardinality✗)
        public class PrideParade {
            [Flags] public enum Group { Lesbian = 1, Gay = 2, Bisexual = 4, Transgender = 8, Queer = 16, Asexual = 32, Intersex = 64, TwoSpirit = 128, Questioning = 256, Straight = 512 }

            [PrimaryKey] public Guid ID { get; set; }
            public DateTime Date { get; set; }
            [Name("Participants"), Name("Paraders")] public IReadOnlyRelationSet<string> Participants { get; set; } = new RelationSet<string>();
            public bool Protested { get; set; }
            public uint RainbowFlags { get; set; }
            public Group Representation { get; set; }
        }

        // Test Scenario: Aggregate-Nested [Name] Change on Field that Already Has [Name] Change (✓renamed✓)
        public class HashMap {
            public struct CppType {
                public string Typename { get; set; }
                [Name("IsPointer")] public bool Pointer { get; set; }
                [Name("IsConst")] public bool Const { get; set; }
                [Name("IsReference")] public bool Reference { get; set; }
            }

            [PrimaryKey] public Guid ID { get; set; }
            public ulong MemoryAddress { get; set; }
            public bool ResolveViaChaining { get; set; }
            [Name("ConstQualified", Path = "Const")] public CppType KeyType { get; set; }
            [Name("Ref", Path = "Reference")] public CppType ValueType { get; set; }
        }

        // Test Scenario: Relation-Nested [Name] Change on Field that Already Has [Name] Change (✓renamed✓)
        public class ArchaeologicalSite {
            public struct PointOfInterest {
                public string Name { get; set; }
                public string Description { get; set; }
                public float Latitude { get; set; }
                public float Longitude { get; set; }
                [Name("Area")] public double TotalSpace { get; set; }
            }

            [PrimaryKey] public Guid SiteID { get; set; }
            public string Name { get; set; } = "";
            public string Country { get; set; } = "";
            public ulong Age { get; set; }
            [Name("TotalArea", Path = "Item.TotalSpace")] public RelationSet<PointOfInterest> Ruins { get; set; } = new();
        }

        // Test Scenario: [Name] Change on Nested Aggregate that Already Has [Name] Change (✓renamed✓)
        public class Sarcophagus {
            public record struct Measurements(float Height, float Width, float Length);
            public struct BioDetails {
                [Name("Dim")] public Measurements Dimensions { get; set; }
                public DateTime Discovered { get; set; }
                public float Weight { get; set; }
            }

            [PrimaryKey] public Guid ID { get; set; }
            public string Entombed { get; set; } = "";
            [Name("Measure", Path = "Dimensions")] public BioDetails Details { get; set; }
            public string StoneType { get; set; } = "";
        }

        // Test Scenario: [Name] Change on Nested Reference that Already Has [Name] Change (✓renamed✓)
        public class MariachiBand {
            public class Song {
                [PrimaryKey] public string Name { get; set; } = "";
                public string? Album { get; set; }
                public double Duration { get; set; }
            }

            public struct Songs {
                [Name("One")] public Song NumberOne { get; set; }
                [Name("#2")] public Song NumberTwo { get; set; }
                [Name("#3")] public Song NumberThree { get; set; }
            }

            [PrimaryKey] public Guid ID { get; set; }
            [Name("#1", Path = "NumberOne")] public Songs Repertoire { get; set; }
            public ushort Members { get; set; }
            public bool UsesVihuelas { get; set; }
            public bool UsesGuitars { get; set; }
            public string HomeCity { get; set; } = "";
        }

        // Test Scenario: [Name] Change on Nested Relation that Already Has [Name] Change (✓affects name of Table✓)
        public class PolarVortex {
            public struct Temps {
                [Name("HighTemps")] public RelationMap<DateTime, double> Highs { get; set; }
                [Name("LowTemps")] public RelationMap<DateTime, double> Lows { get; set; }
            }

            [PrimaryKey] public Guid VortexID { get; set; }
            [Name("HIGHS", Path = "Highs"), Name("LOWS", Path = "Lows")] public Temps Temperatures { get; set; }
            public ushort AttributableDeaths { get; set; }
        }

        // Test Scenario: Nested Property with Multiple [Name] Changes (✗cardinality✗)
        public class Helicopter {
            public record struct Date(int Year, int Month, int Day);

            [PrimaryKey] public Guid HelicopterID { get; set; }
            public byte NumBlades { get; set; }
            public string Manufacturer { get; set; } = "";
            public string Model { get; set; } = "";
            [Name("Y", Path = "Year"), Name("YEAR", Path = "Year")] public Date Debut { get; set; }
        }

        // Test Scenario: Name is Unchanged via [Name] (✓redundant✓)
        public class Opera {
            [PrimaryKey] public Guid ID { get; set; }
            public string Composer { get; set; } = "";
            [Name("PremiereDate")] public DateTime PremiereDate { get; set; }
            public uint Length { get; set; }
        }

        // Test Scenario: Name is Changed to `null` (✗illegal✗)
        public class Longbow {
            [PrimaryKey] public Guid SerialNumber { get; set; }
            public string Manufacturer { get; set; } = "";
            [Name(null!)] public double Weight { get; set; }
            public ulong MaxRange { get; set; }
            public float Tension { get; set; }
        }

        // Test Scenario: Name Changed to the Empty String (✗illegal✗)
        public class Volcano {
            [PrimaryKey] public string Name { get; set; } = "";
            public ulong Height { get; set; }
            public DateTime LastEruption { get; set; }
            [Name("")] public bool IsActive { get; set; }
        }

        // Test Scenario: <Path> is `null` (✗illegal✗)
        public class MedalOfHonor {
            [PrimaryKey] public uint MedalID { get; set; }
            [Name("AwardedFor", Path = null!)] public string Recipient { get; set; } = "";
            public DateTime AwardedOn { get; set; }
            public string AwardedBy { get; set; } = "";
            public string AwardedFor { get; set; } = "";
        }

        // Test Scenario: <Path> on Scalar Does Not Exist (✗non-existent path✗)
        public class Legume {
            [PrimaryKey] public Guid LegumeGuid { get; set; }
            public string Name { get; set; } = "";
            [Name("EnergyInKJ", Path = "---")] public decimal Energy { get; set; }
            public double Carbohydrates { get; set; }
            public double Fat { get; set; }
            public double Protein { get; set; }
        }

        // Test Scenario: <Path> on Aggregate Does Not Exist (✗non-existent path✗)
        public class Madonna {
            public record struct Person(string First, char? MiddleInitial, string? Last);

            [PrimaryKey] public Guid PaintingID { get; set; }
            [Name("Middle", Path = "---")] public Person Painter { get; set; }
            public DateTime Created { get; set; }
            public bool JesusDepicted { get; set; }
            public ushort Height { get; set; }
            public ushort Width { get; set; }
        }

        // Test Scenario: <Path> on Reference Does Not Exist (✗non-existent path✗)
        public class CapitolBuilding {
            public class Person {
                [PrimaryKey] public int SSN { get; set; }
                public string FirstName { get; set; } = "";
                public string LastName { get; set; } = "";
            }

            [PrimaryKey] public string Location { get; set; } = "";
            [PrimaryKey] public bool IsActive { get; set; }
            public DateTime Opened { get; set; }
            public ulong Capacity { get; set; }
            public uint NumSteps { get; set; }
            [Name("SocialSecurity", Path = "---")] public Person Architect { get; set; } = new();
        }

        // Test Scenario: <Path> on Reference Refers to Non-Primary-Key Field (✗non-existent path✗)
        public class Rabbi {
            public enum Judaism { Reform, Conservative, Orthodox, Reconstructionist, Haredi }

            public class Synagogue {
                [PrimaryKey] public Guid TempleID { get; set; }
                public string Name { get; set; } = "";
                public string Address { get; set; } = "";
                public Judaism Denomination { get; set; }
                public ulong Membership { get; set; }
            }

            [PrimaryKey] public string FirstName { get; set; } = "";
            [PrimaryKey] public char MiddleInitial { get; set; }
            [PrimaryKey] public string LastName { get; set; } = "";
            public DateTime Ordained { get; set; }
            public ulong SermonsDelivered { get; set; }
            public ulong WeddingsPerformed { get; set; }
            public ulong BneiMitzvotOfficiated { get; set; }
            public ulong BabiesNamed { get; set; }
            public ulong ConversionsOverseen { get; set; }
            [Name("Type", Path = "Denomination")] public Synagogue? CurrentTemple { get; set; }
        }

        // Test Scenario: <Path> on Reference Refers to Aggregate Housing (Part of) Primary Key (✓renamed✓)
        public class CarAccident {
            public class Car {
                public record struct Registration(Guid ID, DateTime Approved, bool International);

                [PrimaryKey(Path = "ID")] public Registration Reg { get; set; }
                public string Make { get; set; } = "";
                public string Model { get; set; } = "";
                public ulong Milage { get; set; }
                public byte NumDoors { get; set; }
            }

            [PrimaryKey] public Guid AccidentReportID { get; set; }
            public ushort Casualties { get; set; }
            public Car Instigator { get; set; } = new();
            [Name("Registration", Path = "Reg")] public Car? Other { get; set; }
        }

        // Test Scenario: <Path> on Relation Does Not Exist (✗non-existent path✗)
        public class ProcessRegister {
            [Flags] public enum Operation { None = 0, Read = 1, Write = 2}

            [PrimaryKey] public string Identifier { get; set; } = "";
            public Operation AllowedOperations { get; set; }
            public short Size { get; set; }
            [Name("eax?", Path = "---")] public RelationSet<string> Architectures { get; set; } = new();
            public bool StackPointer { get; set; }
        }

        // Test Scenario: <Path> on Relation Refers to Non-Primary-Key Field of Anchor Entity (✗non-existent path✗)
        public class Yeshiva {
            public enum Denom { Reform, Conservative, Orthodox, Chabad }
            public record struct Student(string FirstName, string LastName, string HebrewName);

            [PrimaryKey] public string Name { get; set; } = "";
            public string City { get; set; } = "";
            public Denom Denomination { get; set; }
            public string ChiefRabbi { get; set; } = "";
            public DateTime Founded { get; set; }
            [Name("HomeCity", Path = "Yeshiva.City")] public RelationList<Student> Students { get; set; } = new();
        }
    }

    internal static class DefaultValues {
        // Test Scenario: Non-`null` Boolean, Numeric, and Text Defaults (✓valid✓)
        public class BloodType {
            [PrimaryKey, Default("O")] public string ABO { get; set; } = "";
            [PrimaryKey, Default(true)] public bool RHPositive { get; set; }
            [Default(0.5f)] public float ApproxPrevalence { get; set; }
            [Default(1)] public int NumSubgroups { get; set; }
            public decimal AnnualDonationsL { get; set; }
        }

        // Test Scenario: Non-`null` Decimal Default (✓valid✓)
        public class Bestiary {
            [PrimaryKey] public uint ISBN { get; set; }
            public string Title { get; set; } = "";
            public string Author { get; set; } = "";
            [Default(35.78)] public decimal MarketValue { get; set; }
            public ushort NumPages { get; set; }
            public DateTime? Published { get; set; }
            public ushort NumBeasts { get; set; }
        }

        // Test Scenario: Non-`null` DateTime Default (✓valid✓)
        public class Umpire {
            [PrimaryKey] public Guid UniqueUmpireNumber { get; set; }
            public ushort UniformNumber { get; set; }
            public string Name { get; set; } = "";
            [Default("1970-01-01")] public DateTime Debut { get; set; }
            public uint Ejections { get; set; }
        }

        // Test Scenario: Non-`null` Guid Default (✓valid✓)
        public class Saint {
            [Default("81a130d2-502f-4cf1-a376-63edeb000e9f")] public Guid SainthoodIdentifier { get; set; }
            [PrimaryKey] public string Name { get; set; } = "";
            public DateTime CanonizationDate { get; set; }
            public byte FeastMonth { get; set; }
            public byte FeastDay { get; set; }
        }

        // Test Scenario: Non-`null` Valid Enumeration Default (✓valid✓)
        public class Oceanid {
            [Flags] public enum Source { Hesiod = 1, HomericHymn = 2, Apollodorus = 4, Hyginus = 8 };

            public string Name { get; set; } = "";
            [PrimaryKey] public string Greek { get; set; } = "";
            [Default(Source.Hesiod | Source.Hyginus)] public Source MentionedIn { get; set; }
            public int NumChildren { get; set; }
        }

        // Test Scenario: `null` Default for Nullable Scalar Field (✓valid✓)
        public class Pepper {
            [PrimaryKey] public string Genus { get; set; } = "";
            [PrimaryKey] public string Species { get; set; } = "";
            [Default(null)] public string? CommonName { get; set; }
            [Default(null)] public DateTime? FirstCultivated { get; set; }
            public ulong ScovilleRating { get; set; }
        }

        // Test Scenario: `null` Default for Nullable Enumeration Field (✓valid✓)
        public class Cryptid {
            public enum Continent { NorthAmerica, SouthAmerica, Asia, Europe, Africa, Oceania, Antarctica };
            [Flags] public enum Features { Flying = 1, Carnivorous = 2, Humanoid = 4, Aquatic = 8, FireProof = 16, Hematophagous = 32 };

            [PrimaryKey] public string Name { get; set; } = "";
            public uint AllegedSightings { get; set; }
            [Default(null)] public Continent? HomeContinent { get; set; }
            [Default(null)] public Features? FeatureSet { get; set; }
            public bool ProvenHoax { get; set; }
        }

        // Test Scenario: Original Default on Aggregate-Nested Field (✓propagated✓)
        public class Sermon {
            public struct Institute {
                public string Name { get; set; }
                public string Address { get; set; }
                [Default(1756102UL)] public ulong CongregationSize { get; set; }
            }

            [PrimaryKey] public string Clergy { get; set; } = "";
            [PrimaryKey] public DateTime DeliveredAt { get; set; }
            public string? Title { get; set; }
            public string Text { get; set; } = "";
            public Institute HouseOfWorship { get; set; }
            public bool ForHoliday { get; set; }
        }

        // Test Scenario: Default on Aggregate-Nested Field (✓valid✓)
        public class Salsa {
            public struct Pepper {
                public string Name { get; set; }
                public uint ScovilleRating { get; set; }
            }

            [PrimaryKey] public string SalsaName { get; set; } = "";
            [Default(10000U, Path = "ScovilleRating")] public Pepper PrimaryPepper { get; set; }
            public bool Verde { get; set; }
            public sbyte ClovesGarlic { get; set; }
        }

        // Test Scenario: Default on Aggregate-Nested Field that Already Has a Default (✓overrides✓)
        public class Bicycle {
            public struct Alloy {
                public string Metal1 { get; set; }
                [Default(null)] public string? Metal2 { get; set; }
            }
            public struct Wheel {
                public double Diameter { get; set; }
                public byte NumSpokes { get; set; }
                public Alloy Material { get; set; }
            }

            [PrimaryKey] public Guid BikeID { get; set; }
            [Default("Titanium", Path = "Material.Metal2")] public Wheel FrontWheel { get; set; }
            [Default("Copper", Path = "Material.Metal2")] public Wheel BackWheel { get; set; }
            public Wheel? SpareWheel { get; set; }
            public ushort Gears { get; set; }
            public float TopSpeed { get; set; }
        }

        // Test Scenario: Original Default on Reference-Nested Field (✓not propagated✓)
        public class Arch {
            public class Coordinate {
                [PrimaryKey] public float Latitude { get; set; }
                [PrimaryKey, Default(0.0f)] public float Longitude { get; set; }
            }

            [PrimaryKey] public Guid ArchID { get; set; }
            public string Material { get; set; } = "";
            public double Height { get; set; }
            public double Diameter { get; set; }
            public Coordinate Location { get; set; } = new();
            public Guid KeystoneID { get; set; }
        }

        // Test Scenario: Default on Reference-Nested Field (✓valid✓)
        public class Kite {
            public class String {
                public double Length { get; set; }
                [PrimaryKey] public Guid BallSource { get; set; }
                [PrimaryKey] public ushort CutNumber { get; set; }
                public float Weight { get; set; }
            }

            [PrimaryKey] public Guid KiteID { get; set; }
            [Default((ushort)31, Path = "CutNumber")] public String KiteString { get; set; } = new();
            public double MajorAxis { get; set; }
            public double MinorAxis { get; set; }
            public string Material { get; set; } = "";
            public double TopSpeed { get; set; }
        }

        // Test Scenario: Default on Reference-Nested Field that Already Has a Default (✓overrides✓)
        public class EscapeRoom {
            public enum Style { Mathematical, Musical, Physical, Logical, Scientific, Linguistic, Botanical }

            public class Puzzle {
                [PrimaryKey] public string Description { get; set; } = "";
                [PrimaryKey, Default(Style.Logical)] public Style PuzzleType { get; set; }
                public ushort AverageCompletionTime { get; set; }
                public double ChallengeRating { get; set; }
            }

            [PrimaryKey] public Guid RoomID { get; set; }
            public ushort TimeLimit { get; set; }
            public ushort BestTime { get; set; }
            [Default(Style.Linguistic, Path = "PuzzleType")] public Puzzle FirstPuzzle { get; set; } = new();
            [Default(Style.Logical, Path = "PuzzleType")] public Puzzle FinalPuzzle { get; set; } = new();
        }

        // Test Scenario: Original Default on Relation-Nested Field (✓propagated✓)
        public class DockerContainer {
            public struct Directory {
                public string Path { get; set; }
                [Default((ushort)777)] public ushort Permissions { get; set; }
                public bool IsSymlink { get; set; }
                public DateTime Created { get; set; }
                public DateTime Modified { get; set; }
            }

            [PrimaryKey] public string Image { get; set; } = "";
            [PrimaryKey, Default(0)] public int PID { get; set; }
            public string EntryPoint { get; set; } = "";
            public RelationMap<Directory, string> Mounts { get; set; } = new();
        }

        // Test Scenario: Default on Relation-Nested Field (✓valid✓)
        public class Kami {
            [PrimaryKey] public string Name { get; set; } = "";
            public string CultCenter { get; set; } = "";
            [Default("n/a", Path = "Item")] public RelationSet<string> AKAs { get; set; } = new();
            [Default("Susano'o", Path = "Kami.Name"), Default((short)19, Path = "Value")] public RelationMap<string, short> Appearances { get; set; } = new();
            public string? PrimaryTorii { get; set; }
            public bool Female { get; set; }
        }

        // Test Scenario: Default on Relation-Nested Field that Already Has a Default (✓overrides✓)
        public class Tamagotchi {
            public struct Accessory {
                public Guid ID { get; set; }
                public string Name { get; set; }
                [Default(0.50)] public decimal Cost { get; set; }
            }

            [PrimaryKey] public Guid ID { get; set; }
            public string Name { get; set; } = "";
            public DateTime Spawned { get; set; }
            public ushort HatchTime { get; set; }
            public byte Age { get; set; }
            public uint Weight { get; set; }
            [Default(3.75, Path = "Item.Cost")] public RelationList<Accessory> Accessories { get; set; } = new();
        }

        // Test Scenario: `null` Default for Non-Nullable Field (✗invalid✗)
        public class RadioStation {
            [PrimaryKey] public bool IsFM { get; set; }
            [PrimaryKey] public decimal StationNumber { get; set; }
            [Default(null)] public string CallSign { get; set; } = "";
        }

        // Test Scenario: Inconvertible Non-`null` Default (✗invalid✗)
        public class Battleship {
            [PrimaryKey] public string CallSign { get; set; } = "";
            public DateTime? Launched { get; set; }
            [Default("100 feet")] public ushort Length { get; set; }
            public ushort TopSpeedMPH { get; set; }
            public byte GunCount { get; set; }
            public Guid ShipyardIdentifier { get; set; }
        }

        // Test Scenario: Convertible Non-`null` Default (✗invalid✗)
        public class County {
            [PrimaryKey] public ulong GNIS_ID { get; set; }
            public string Name { get; set; } = "";
            public string State { get; set; } = "";
            [Default(5000000)] public ulong Population { get; set; }
            public ulong Area { get; set; }
            public DateTime Incorporation { get; set; }
        }

        // Test Scenario: Enumeration Default on [Numeric] Field (✗invalid✗)
        public class MasterClass {
            public enum Domain : ushort { Sports, Acting, Marketing, Business, Politics, Art, Writing, Comedy, Cooking }

            [PrimaryKey] public Guid ClassID { get; set; }
            public string Title { get; set; } = "";
            public string Teacher { get; set; } = "";
            [Default(Domain.Politics), Numeric] public Domain Category { get; set; }
            public uint Sessions { get; set; }
            public uint Minutes { get; set; }
            public decimal Price { get; set; }
            public string URL { get; set; } = "";
        }

        // Test Scenario: Enumeration Default on [AsString] Field (✗invalid✗)
        public class Orphanage {
            public enum Kind { Public, Private, Medical }

            [PrimaryKey] public float Latitude { get; set; }
            [PrimaryKey] public float Longitude { get; set; }
            public string Name { get; set; } = "";
            [Default(Kind.Private), AsString] public Kind Type { get; set; }
            public ushort Occupants { get; set; }
            public double AdoptionRate { get; set; }
            public string Headmistress { get; set; } = "";
        }

        // Test Scenario: Applied to Nested Aggregate (✗invalid✗)
        public class StuffedAnimal {
            public record struct Face(bool HasEyes, int NumButtons, bool IsSmiling);
            public record struct Construction(string Material, Face Face, bool VisibleTag, string Internals);

            [PrimaryKey] public Guid ID { get; set; }
            public string Animal { get; set; } = "";
            public string? Name { get; set; }
            public bool BuildABear { get; set; }
            [Default(136L, Path = "Face")] public Construction Description { get; set; }
            public double Weight { get; set; }
        }

        // Test Scenario: Applied to Nested Reference (✗invalid✗)
        public class PoetLaureate {
            public enum Type { Country, State, Province, City, Emirate, Oblast, District, County, Constituent, Island }

            public class State {
                [PrimaryKey] public string Endonym { get; set; } = "";
                [PrimaryKey] public string Exoynym { get; set; } = "";
                public ulong Population { get; set; }
                public ulong Area { get; set; }
            }
            public record struct Polity(State Entity, Type Type);

            [PrimaryKey] public string FirstName { get; set; } = "";
            [PrimaryKey] public string LastName { get; set; } = "";
            public DateTime TermBegin { get; set; }
            public DateTime TermEnd { get; set; }
            [Default('x', Path = "Entity")] public Polity Of { get; set; }
            public string InauguralPoemTitle { get; set; } = "";
        }

        // Test Scenario: Applied to Nested Relation (✗invalid✗)
        public class TimeTraveler {
            public record struct Machine(Guid SerialNumber, RelationList<string> Owners);

            [PrimaryKey] public string FirstName { get; set; } = "";
            [PrimaryKey] public string LastName { get; set; } = "";
            public DateTime BirthDate { get; set; }
            [Default("Marty McFly", Path = "Owners")] public Machine TimeMachine { get; set; }
            public RelationSet<DateTime> Visitations { get; set; } = new();
            public uint ParadoxesCaused { get; set; }
        }

        // Test Scenario: Single-Element Array Default (✗invalid✗)
        public class BilliardBall {
            [PrimaryKey] public string Color { get; set; } = "";
            [PrimaryKey, Default(new int[] { 7 })] public int Number { get; set; }
            public bool IsSolid { get; set; }
        }

        // Test Scenario: Decimal Default is Not a Double (✗invalid✗)
        public class Geocache {
            [PrimaryKey] public float Latitude { get; set; }
            [PrimaryKey] public float Longitude { get; set; }
            public string Name { get; set; } = "";
            public ulong TimesFound { get; set; }
            [Default(45109.336f)] public decimal NetTrinketValue { get; set; }
        }

        // Test Scenario: Decimal Default is Out-of-Range (✗invalid✗)
        public class Screwdriver {
            [PrimaryKey] public Guid ScrewdriverID { get; set; }
            public float Length { get; set; }
            [Default(double.MaxValue)] public decimal HeadWidth { get; set; }
        }

        // Test Scenario: DateTime Default is Not a String (✗invalid✗)
        public class RomanEmperor {
            [PrimaryKey] public int ChronologicalIndex { get; set; }
            public string LongName { get; set; } = "";
            public string ShortName { get; set; } = "";
            public DateTime ReignStart { get; set; }
            [Default(true)] public DateTime ReignEnd { get; set; }
        }

        // Test Scenario: DateTime Default is Improperly Formatted (✗invalid✗)
        public class Tournament {
            [PrimaryKey] public string Name { get; set; } = "";
            [Default("20030714")] public DateTime Kickoff { get; set; }
            public DateTime? Conclusion { get; set; }
            public int Participants { get; set; }
            public string Number1 { get; set; } = "";
        }

        // Test Scenario: DateTime Default is Out-of-Range (✗invalid✗)
        public class Sculpture {
            [PrimaryKey, Default("1344-18-18")] public DateTime CreationDate { get; set; }
            public string Sculptor { get; set; } = "";
            public ushort HeightFt { get; set; }
            public ushort WeightLbs { get; set; }
            public bool InOnePiece { get; set; }
        }

        // Test Scenario: Guid Default is Not a String (✗invalid✗)
        public class HogwartsHouse {
            public string Name { get; set; } = "";
            public ushort FirstPageMentioned { get; set; }
            public long TotalMentions { get; set; }
            [Default('^')] public Guid TermIndex { get; set; }
        }

        // Test Scenario: Guid Default is Improperly Formatted (✗invalid✗)
        public class Gene {
            [PrimaryKey, Default("ee98f44827b248a2bb9fc5ef342e7ab2!!!")] public Guid UUID { get; set; }
            public string Identifier { get; set; } = "";
            public long HumanEntrez { get; set; }
            public string UCSCLocation { get; set; } = "";
        }

        // Test Scenario: Enumeration Default is not Named Enumerator (✗invalid✗)
        public class MoonOfJupiter {
            public enum Group { Galilean, Themisto, Himalia, Carpo, Valetudo, Ananke, Carme, Pasiphae };

            [PrimaryKey] public string Name { get; set; } = "";
            public DateTime Discovered { get; set; }
            [Default((Group)87123)] public Group MoonGroup { get; set; }
            public long Volume { get; set; }
            public float SurfaceGravity { get; set; }
            public double ApparentMagnitude { get; set; }
        }

        // Test Scenario: Flag Enumeration Default is not Valid Combination (✗invalid✗)
        public class Newspaper {
            public enum Section { Sports, Politics, Comics, Cultures, FilmReviews, Obituaries, Weather, Classifieds }

            public string City { get; set; } = "";
            [PrimaryKey] public string Title { get; set; } = "";
            public bool PublishedDaily { get; set; }
            [Default((Section)15)] public Section Contents { get; set; }
            public string Jan1Headline { get; set; } = "";
            public long Circulation { get; set; }
        }

        // Test Scenario: Default of Source Type on Data-Converted Property (✗invalid✗)
        public class CrosswordClue {
            [PrimaryKey] public Guid PuzzleID { get; set; }
            [DataConverter(typeof(ToInt<char>)), Default('A')] public char AcrossOrDown { get; set; }
            public ushort Number { get; set; }
            public byte NumLetters { get; set; }
            public string ClueText { get; set; } = "";
        }

        // Test Scenario: Default of Target Type on Data-Converted Property (✓valid✓)
        public class Coupon {
            [PrimaryKey] public Guid Barcode { get; set; }
            public string? Code { get; set; }
            [DataConverter(typeof(ToInt<bool>)), Default(0)] public bool IsBOGO { get; set; }
            public double? DiscountPercentage { get; set; }
            public float? MinimumPurchase { get; set; }
            public DateTime? ExpirationDate { get; set; }
        }

        // Test Scenario: Scalar Property with Multiple Default Values (✗cardinality✗)
        public class SkeeBall {
            public int MachineID { get; set; }
            [Default((ushort)1), Default((ushort)0)] public ushort L1Value { get; set; }
            public ushort L2Value { get; set; }
            public ushort L3Value { get; set; }
            public ushort L4Value { get; set; }
        }

        // Test Scenario: <Path> is `null` (✗illegal✗)
        public class Waterfall {
            [PrimaryKey] public uint InternationalUnifiedWaterfallNumber { get; set; }
            public string Exonym { get; set; } = "";
            public string Endonym { get; set; } = "";
            public ulong Height { get; set; }
            [Default(73UL, Path = null!)] public ulong WorldRanking { get; set; }
        }

        // Test Scenario: <Path> on Scalar Does Not Exist (✗non-existent path✗)
        public class NativeAmericanTribe {
            [PrimaryKey] public string Endonym { get; set; } = "";
            [Default(null, Path = "---")] public string? Exonym { get; set; }
            public ulong Population { get; set; }
            public DateTime Established { get; set; }
            public string GoverningBody { get; set; } = "";
            public ulong Area { get; set; }
        }

        // Test Scenario: <Path> on Aggregate Does Not Exist (✗non-existent path✗)
        public class TourDeFrance {
            public record struct Person(string FirstName, string LastName);

            [PrimaryKey] public short Year { get; set; }
            public string StartingCity { get; set; } = "";
            public string EndingCity { get; set; } = "";
            public long NumCyclists { get; set; }
            [Default("X.", Path = "---")] public Person Victor { get; set; }
        }

        // Test Scenario: <Path> on Aggregate Not Specified (✗missing path✗)
        public class InfinityStone {
            public record struct Color(byte R, byte G, byte B);
            public struct Descriptor {
                [Default(100)] public Color Color { get; set; }
                public ushort Weight { get; set; }
                public uint Luminescence { get; set; }
            }

            [PrimaryKey] public string Domain { get; set; } = "";
            public Descriptor Description { get; set; }
            public string FirstFilmAppearance { get; set; } = "";
            public string FirstComicsAppearance { get; set; } = "";
        }

        // Test Scenario: <Path> on Reference Does Not Exist (✗non-existent path✗)
        public class Hepatitis {
            public class Medication {
                [PrimaryKey] public Guid FDA_ID { get; set; }
                public string Name { get; set; } = "";
                public string Formula { get; set; } = "";
                public bool OTC { get; set; }
                public decimal PrescriptionPrice { get; set; }
                public double LethalDose { get; set; }
            }

            [PrimaryKey] public char Strain { get; set; }
            public ulong Prevalence { get; set; }
            public bool IsComplicatedbyCirrhosis { get; set; }
            [Default(1541923.558, Path = "---")] public Medication Treatment { get; set; } = new();
            public double MortailtyRate { get; set; }
        }

        // Test Scenario: <Path> on Reference Refers to Non-Primary-Key Field (✗non-existent path✗)
        public class Calculator {
            public class MakeAndModel {
                [PrimaryKey] public string Manufacturer { get; set; } = "";
                [PrimaryKey] public string Make { get; set; } = "";
                [PrimaryKey] public string Model { get; set; } = "";
                public bool IsInCirculation { get; set; }
            }

            [PrimaryKey] public Guid ProductID { get; set; }
            [Default(true, Path = "IsInCirculation")] public MakeAndModel MakeModel { get; set; } = new();
            public bool IsGraphing { get; set; }
            public bool IsACTLegal { get; set; }
            public short BatteryLife { get; set; }
            public string LastCalculation { get; set; } = "";
        }

        // Test Scenario: <Path> on Reference Not Specified (✗missing path✗)
        public class PopTart {
            public class Color {
                [PrimaryKey] public byte Red { get; set; }
                [PrimaryKey] public byte Green { get; set; }
                [PrimaryKey] public byte Blue { get; set; }
            }

            [PrimaryKey] public string Flavor { get; set; } = "";
            public bool Discontinued { get; set; }
            public DateTime FirstReleased { get; set; }
            public double RecommendedToasterTime { get; set; }
            [Default(null)] public Color FrostingColor { get; set; } = new();
            public bool IsChocolatey { get; set; }
        }

        // Test Scenario: <Path> on Relation Does Not Exist (✗non-existent path✗)
        public class ArcadeGame {
            [PrimaryKey] public Guid SerialNumber { get; set; }
            public string Title { get; set; } = "";
            public string Name { get; set; } = "";
            [Default(0.00, Path = "---")] public RelationMap<string, double> HighScores { get; set; } = new();
            public ushort NumLevels { get; set; }
            public bool IsSports { get; set; }
        }

        // Test Scenario: <Path> on Relation Refers to Non-Primary-Key Field of Anchor Entity (✗non-existent path✗)
        public class Monad {
            public enum Operation { Map, Join, Unit, Bind, Constructor }

            [PrimaryKey] public string Name { get; set; } = "";
            [Default((sbyte)77, Path = "Monad.ModelsOption")] public RelationMap<Operation, string> Traits { get; set; } = new();
            public bool ModelsOption { get; set; }
        }

        // Test Scenario: <Path> on Relation Not Specified (✗missing path✗)
        public class LaundryDetergent {
            [PrimaryKey] public Guid DetergentID { get; set; }
            public string Brand { get; set; } = "";
            public double VolumePerLoad { get; set; }
            [Default(null)] public RelationSet<string> Ingredients { get; set; } = new();
            public bool ToxiToConsume { get; set; }
        }
    }

    internal static class ColumnOrdering {
        // Test Scenario: All Fields are Manually Ordered (✓ordered✓)
        public class Fraction {
            [PrimaryKey, Column(2)] public decimal Numerator { get; set; }
            [Column(1)] public decimal Denominator { get; set; }
            [Column(0)] public bool IsNegative { get; set; }
        }

        // Test Scenario: Scalar Fields are Manually Ordered (✓ordered✓)
        public class Parashah {
            [PrimaryKey] public string Book { get; set; } = "";
            [PrimaryKey] public ushort StartChapter { get; set; }
            [PrimaryKey] public ushort StartVerse { get; set; }
            [Column(4)] public ushort EndChapter { get; set; }
            [Column(2)] public ushort EndVerse { get; set; }
        }

        // Test Scenario: Aggregate Fields are Manually Ordered (✓ordered✓)
        public class Armada {
            public enum Class { Battleship, AircraftCarrier, PassengerGalley, Dreadnaught, Submarine, Other }
            public record struct Boat {
                [Column(2)] public string Name { get; set; }
                public Class Class { get; set; }
                public ushort Munitions { get; set; }
            }

            [PrimaryKey] public uint ID { get; set; }
            public string Commander { get; set; } = "";
            public string Sponsor { get; set; } = "";
            [Column(3)] public Boat Flagship { get; set; }
            [Column(7)] public Boat? Secondary { get; set; }
            public Boat? Tertiary { get; set; }
            public double VictoryPercentage { get; set; }
        }

        // Test Scenario: Reference Fields are Manually Ordered (✓ordered✓)
        public class EdibleArrangement {
            public class Basket {
                [PrimaryKey, Column(1)] public Guid FactoryID { get; set; }
                [PrimaryKey, Column(2)] public string Brand { get; set; } = "";
                [PrimaryKey, Column(0)] public int Item { get; set; }
                public double Weight { get; set; }
                public bool IsWicker { get; set; }
            }

            [PrimaryKey] public Guid ID { get; set; }
            public decimal Price { get; set; }
            public int Strawberries { get; set; }
            public int Bananas { get; set; }
            public int Grapes { get; set; }
            public int Cantaloupe { get; set; }
            public int OtherFruit { get; set; }
            [Column(5)] public Basket Vessel { get; set; } = new();
        }

        // Test Scenario: Relation Fields are Implicitly Ordered (✓default behavior✓)
        public class DebitCard {
            public enum Permission { CardHolder, AuthorizedSpender, EmergencyContact }
            public record struct Transaction(DateTime Timestamp, decimal Amount, string Location);

            [PrimaryKey] public ulong CardNumber { get; set; }
            [PrimaryKey] public string Issuer { get; set; } = "";
            public short SecurityCode { get; set; }
            public decimal DebitLimit { get; set; }
            public RelationMap<string, Permission> Account { get; set; } = new();
            public DateTime Expiration { get; set; }
            public RelationList<Transaction> Transactions { get; set; } = new();
        }

        // Test Scenario: Relation Fields are Manually Ordered (✗impermissible✗)
        public class Tapestry {
            [PrimaryKey] public Guid TapestryID { get; set; }
            public double Length { get; set; }
            public double Width { get; set; }
            [Column(6)] public RelationList<string> Depictions { get; set; } = new();
            public string? Artist { get; set; }
            public ulong ThreadCount { get; set; }
        }

        // Test Scenario: Reference's Primary Key Fields are Non-Consecutive (✓collapsed✓)
        public class MassExtinction {
            public class GeologicPeriod {
                [PrimaryKey, Column(0)] public string Name { get; set; } = "";
                [PrimaryKey, Column(3)] public ulong MYA { get; set; }
                public string StandardSpecies { get; set; } = "";
                public string GeologicEra { get; set; } = "";
            }

            [PrimaryKey] public int Index { get; set; }
            public GeologicPeriod ExitBoundary { get; set; } = new();
            public GeologicPeriod EntryBoundary { get; set; } = new();
            public double Severity { get; set; }
        }

        // Test Scenario: Anchor Entity's Primary Key Fields are Non-Consecutive (✓collapsed✓)
        public class DuoPush {
            public enum Result { Approve, Reject, TimeOut }

            [PrimaryKey, Column(3)] public Guid DeviceID { get; set; }
            [PrimaryKey, Column(0)] public DateTime Timestamp { get; set; }
            public Result Outcome { get; set; }
            public RelationList<string> Notifications { get; set; } = new();
            public short PassCode { get; set; }
            public bool Mobile { get; set; }
        }

        // Test Scenario: Two Scalar Fields Ordered to Same Index in Entity (✗duplication✗)
        public class Pizza {
            [PrimaryKey] public Guid ID { get; set; }
            public float Diamater { get; set; }
            [Column(1)] public string Chain { get; set; } = "";
            public string Cheese { get; set; } = "";
            [Column(7)] public string? Meat1 { get; set; }
            public string? Meat2 { get; set; }
            [Column(5)] public string? Veggie1 { get; set; }
            [Column(7)] public string? Veggie2 { get; set; }
            public string? Veggie3 { get; set; }
        }

        // Test Scenario: Two Scalar Fields Ordered to Same Index in Aggregate (✗duplication✗)
        public class BiblicalPlague {
            public struct Translation {
                public string English { get; set; }
                [Column(1)] public string Hebrew { get; set; }
                [Column(1)] public string Greek { get; set; }
                public string Latin { get; set; }
            }

            [PrimaryKey] public byte Index { get; set; }
            public Translation Terminology { get; set; }
        }

        // Test Scenario: Two Nested Fields Ordered to Same Index (✗duplication✗)
        public class Coup {
            public record struct Person(string FirstName, string? MiddleName, string LastName);

            [PrimaryKey] public DateTime Date { get; set; }
            [PrimaryKey] public string State { get; set; } = "";
            [Column(3)] public Person Overthrower { get; set; }
            [Column(1)] public Person Overthrowee { get; set; }
        }

        // Test Scenario: Scalar and Nested Fields Ordered to Same Index (✗duplication✗)
        public class Bread {
            public enum LeaveningMethod { Unleavened, Yeast, BakingSoda, Microbes }
            public record struct Ingredients(uint Flour, uint Water, uint Sugar, uint Eggs);

            [PrimaryKey] public Guid BreadID { get; set; }
            [Column(4)] public LeaveningMethod Leavening { get; set; }
            [Column(2)] public Ingredients Recipe { get; set; }
            public string Style { get; set; } = "";
        }

        // Test Scenario: Ordering of Scalars Leaves Gaps (✗non-consecutive✗)
        public class PhoneNumber {
            [PrimaryKey, Column(1)] public byte CountryCode { get; set; }
            [PrimaryKey] public ushort AreaCode { get; set; }
            [PrimaryKey, Column(14)] public ushort Number { get; set; }
        }

        // Test Scenario: Ordering of Aggregates Leaves Gaps (✗non-consecutive✗)
        public class Verb {
            public record struct Conjugation(string FS, string FP, string SS, string SP, string TS, string TP);
            public record struct Misc(string Participle, string Adjectival, string Gerund);

            [PrimaryKey, Column(3)] public string Infinitive { get; set; } = "";
            [Column(18)] public string Language { get; set; } = "";
        }

        // Test Scenario: Ordering of Reference Leaves Gaps (✗non-consecutive✗)
        public class Origami {
            public class Paper {
                [PrimaryKey] public string Brand { get; set; } = "";
                [PrimaryKey] public float Height { get; set; }
                [PrimaryKey] public float Width { get; set; }
                [PrimaryKey] public bool IsCardStock { get; set; }
            }

            [PrimaryKey] public Guid OrigamiObjectID { get; set; }
            public string Shape { get; set; } = "";
            [Column(9)] public Paper Material { get; set; } = new();
            public ushort Folds { get; set; }
            public double NetWeight { get; set; }
        }

        // Test Scenario: Field Manually Ordered to Negative Index (✗invalid✗)
        public class NationalPark {
            [PrimaryKey] public string Name { get; set; } = "";
            public string State { get; set; } = "";
            [Column(-196)] public DateTime Established { get; set; }
            public uint Area { get; set; }
            public ulong AnnualVisitors { get; set; }
        }
    }

    internal static class PrimaryKeyIdentification {
        // Test Scenario: Single Scalar Property Marked as [PrimaryKey] (✓identified✓)
        public class XKCDComic {
            [PrimaryKey] public string URL { get; set; } = "";
            public string Title { get; set; } = "";
            public string ImageURL { get; set; } = "";
            public string AltText { get; set; } = "";
        }

        // Test Scenario: Multiple Scalar Properties Marked as [PrimaryKey] (✓identified✓)
        public class Month {
            [PrimaryKey] public string Calendar { get; set; } = "";
            [PrimaryKey] public uint Index { get; set; }
            public string Name { get; set; } = "";
            public ushort NumDays { get; set; }
            public bool IsLeapMonth { get; set; }
        }

        // Test Scenario: Aggregate Property Marked as [PrimaryKey] (✓identified✓)
        public class SpaceShuttle {
            public record struct Spec(string SerialNumber, ulong Weight, Guid ID);

            [PrimaryKey] public string Name { get; set; } = "";
            public DateTime FirstFlight { get; set; }
            public DateTime? LastFlight { get; set; }
            [PrimaryKey] public Spec Specification { get; set; }
        }

        // Test Scenario: Aggregate-Nested Scalar Property Marked as [PrimaryKey] (✓identified✓)
        public class Tepui {
            public enum Direction { North, South, East, West }
            public record struct Coordinate(float Latitude, Direction LatDir, float Longitude, Direction LongDir);

            [PrimaryKey(Path = "Latitude"), PrimaryKey(Path = "Longitude")] public Coordinate Location { get; set; }
            public ushort Height { get; set; }
            public bool HasWaterfall { get; set; }
            public ulong SurfaceArea { get; set; }
        }

        // Test Scenario: Nested Aggregate Property Marked as [PrimaryKey] (✓identified✓)
        public class ChoppedBasket {
            public enum FoodCategory { Protein, Dairy, Grain, Sweet, Condiment, Fruit, Vegetable, Miscellaneous }
            public enum CompetitionRound { Appetizer, Entree, Dessert }
            public record struct FoodName(string English, string Alternative);
            public record struct Ingredient(FoodName Name, FoodCategory Category);

            [PrimaryKey] public DateTime AirDate { get; set; }
            [PrimaryKey] public CompetitionRound Round { get; set; }
            [PrimaryKey(Path = "Name")] public Ingredient Ingredient1 { get; set; }
            [PrimaryKey(Path = "Name")] public Ingredient Ingredient2 { get; set; }
            [PrimaryKey(Path = "Name")] public Ingredient Ingredient3 { get; set; }
            [PrimaryKey(Path = "Name")] public Ingredient Ingredient4 { get; set; }
        }

        // Test Scenario: Reference Property Marked as [PrimaryKey] (✓identified✓)
        public class Etiology {
            public class Culture {
                [PrimaryKey] public string Name { get; set; } = "";
                [PrimaryKey] public string Abbreviation { get; set; } = "";
                public DateTime? Started { get; set; }
                public DateTime? Ended { get; set; }
            }

            [PrimaryKey] public Culture Source { get; set; } = new();
            public string? Author { get; set; }
            public string? FullText { get; set; }
            public string ExplanationOf { get; set; } = "";
        }

        // Test Scenario: Reference-Nested Scalar Property Marked as [PrimaryKey] (✓identified✓)
        public class PoirotMystery {
            public class Identifier {
                [PrimaryKey] public Guid ValuePart1 { get; set; }
                [PrimaryKey] public Guid ValuePart2 { get; set; }
                public string Style { get; set; } = "";
            }

            [PrimaryKey(Path = "ValuePart1")] public Identifier ISBN { get; set; } = new();
            public string Title { get; set; } = "";
            public ulong Pages { get; set; }
            public ulong WordCount { get; set; }
            public double GoodReads { get; set; }
            public string Killer { get; set; } = "";
        }

        // Test Scenario: Nested Reference Property Marked as [PrimaryKey] (✓identified✓)
        public class Prophecy {
            public class Person {
                [PrimaryKey] public string FirstName { get; set; } = "";
                [PrimaryKey] public char MiddleInitial { get; set; }
                [PrimaryKey] public string LastName { get; set; } = "";
            }
            public record struct People(Person P1, Person? P2);

            [PrimaryKey] public Guid ProphecyID { get; set; }
            public string Prophesizer { get; set; } = "";
            public DateTime MadeOn { get; set; }
            [PrimaryKey(Path = "P1")] public People Subjects { get; set; }
            public bool SelfFulfilling { get; set; }
        }

        // Test Scenario: Relation-Nested Scalar Property Marked as [PrimaryKey] (✓identified✓)
        public class GrandPrix {
            public record struct Driver(byte CarNumber, string Name);

            [PrimaryKey] public short Year { get; set; }
            [PrimaryKey] public string Country { get; set; } = "";
            [PrimaryKey(Path = "GrandPrix.Year"), PrimaryKey(Path = "GrandPrix.Country"), PrimaryKey(Path = "Key.CarNumber")] public RelationMap<Driver, short> Results { get; set; } = new();
            public byte NumLaps { get; set; }
            public ulong TrackLength { get; set; }
            public uint NumCrashes { get; set; }
            public bool FormulaOne { get; set; }
        }

        // Test Scenario: Nested Relation Property Marked as [PrimaryKey] (✗illegal✗)
        public class Psalm {
            public record struct TextInfo(string Title, RelationMap<ushort, string> Verses);

            [PrimaryKey] public int Number { get; set; }
            [PrimaryKey(Path = "Verses")] public TextInfo Text { get; set; } = new();
            public double PercentWeddingsQuoted { get; set; }
        }

        // Test Scenario: All Properties Marked as [PrimaryKey] (✓identified✓)
        public class Character {
            [PrimaryKey] public char Glyph { get; set; }
            [PrimaryKey] public uint CodePoint { get; set; }
            [PrimaryKey] public bool IsASCII { get; set; }
        }

        // Test Scenario: Non-Nullable Field Named `ID` (✓identified✓)
        public class Actor {
            public int ID { get; set; }
            public string FirstName { get; set; } = "";
            public string? MiddleName { get; set; }
            public string LastName { get; set; } = "";
            public DateTime Birthdate { get; set; }
            public uint EmmyAwards { get; set; }
            public uint OscarAwards { get; set; }
        }

        // Test Scenario: Non-Nullable Field Renamed to `ID` (✓identified✓)
        public class PokerHand {
            [Name("ID")] public Guid HandIdentifier { get; set; }
            public long BigBlind { get; set; }
            public long SmallBlind { get; set; }
            public bool HeadsUp { get; set; }
            public long Pot { get; set; }
            public ushort? Flop1 { get; set; }
            public ushort? Flop2 { get; set; }
            public ushort? Flop3 { get; set; }
            public ushort? Turn { get; set; }
            public ushort? River { get; set; }
        }

        // Test Scenario: Non-Nullable Field Named `<EntityType>ID` (✓identified✓)
        public class IntegerSequence {
            public int IntegerSequenceID { get; set; }
            public string Title { get; set; } = "";
            public string Description { get; set; } = "";
            public uint Citations { get; set; }
            public int Element0 { get; set; }
            public int? Element1 { get; set; }
            public int? Element2 { get; set; }
            public int? Element3 { get; set; }
            public int? Element4 { get; set; }
        }

        // Test Scenario: Non-Nullable Field Renamed to `<EntityType>ID` (✓identified✓)
        public class Stadium {
            [Name("StadiumID")] public Guid Identifier { get; set; }
            public DateTime Opened { get; set; }
            public DateTime? Closed { get; set; }
            public ulong Capacity { get; set; }
        }

        // Test Scenario: Non-Nullable Fields Named `<EntityType>ID` and `<TableName>ID` (✓identified✓)
        [Table("Method")]
        public class Function {
            public int FunctionID { get; set; }
            public int MethodID { get; set; }
            public string ReturnType { get; set; } = "";
            public string Name { get; set; } = "";
            public byte ArgumentCount { get; set; }
            public bool IsGlobal { get; set; }
            public bool IsFree { get; set; }
            public bool IsMember { get; set; }
        }

        // Test Scenario: Single Candidate Key with One Non-Nullable Field (✓identified✓)
        public class Star {
            [Unique] public string ARICNS { get; set; } = "";
            public double Mass { get; set; }
            public double Luminosity { get; set; }
            public float Temperature { get; set; }
            public float Distance { get; set; }
            public ulong Period { get; set; }
            public decimal Parallax { get; set; }
        }

        // Test Scenario: Single Candidate Key with Multiple Non-Nullable Fields (✓identified✓)
        public class Expiration {
            [Unique("PK")] public uint FeedCode { get; set; }
            [Unique("PK")] public string Underlying { get; set; } = "";
            public string Product { get; set; } = "";
        }

        // Test Scenario: Multiple Candidate Keys, but Only One has All Non-Nullable Fields (✓identified✓)
        public class Vitamin {
            [Unique("CK1")] public string Name { get; set; } = "";
            [Unique("CK1")] public string? Alternative { get; set; }
            [Unique("Formula")] public ushort Carbon { get; set; }
            [Unique("Formula")] public ushort Hydrogen { get; set; }
            [Unique("Formula")] public ushort Cobalt { get; set; }
            [Unique("Formula")] public ushort Nitrogen { get; set; }
            [Unique("Formula")] public ushort Oxygen { get; set; }
            [Unique("Formula")] public ushort Phosphorus { get; set; }
        }

        // Test Scenario: Multiple Candidate Keys, but One is Subset of the Others (✓identified✓)
        public class Escalator {
            [Unique("ID"), Unique("Key"), Unique("F3")] public Guid EscalatorIdentifier { get; set; }
            public double Height { get; set; }
            [Unique("Key")] public bool GoesUp { get; set; }
            [Unique("F3")] public string Manufacturer { get; set; } = "";
            [Unique("F3")] public bool InMall { get; set; }
        }

        // Test Scenario: Single Candidate Key Deduced for Other Reasons (✓identified✓)
        public class Repository {
            [Unique] public Guid ID { get; set; }
            public string GitHubURL { get; set; } = "";
            public string Author { get; set; } = "";
            public ulong NumCommits { get; set; }
            public string License { get; set; } = "";
            public double CodeSizeKB { get; set; }
            public ushort NumReleases { get; set; }
            public bool IsPublic { get; set; }
        }

        // Test Scenario: Only One Non-Nullable Field (✓identified✓)
        public class Earthquake {
            public Guid SeismicIdentificationNumber { get; set; }
            public DateTime? Occurrence { get; set; }
            public decimal? Magnitude { get; set; }
            public double? EpicenterLatitude { get; set; }
            public double? EpicenterLongitude { get; set; }
        }

        // Test Scenario: Only One Non-Nullable Field via [NonNullable] (✓identified✓)
        public class GeologicEpoch {
            [NonNullable] public ulong? StartingMYA { get; set; }
            public ulong? EndingMYA { get; set; }
            public string? Name { get; set; }
        }

        // Test Scenario: All Fields are Non-Nullable and No Other Deductions Performed (✓identified✓)
        public class HotAirBalloon {
            public string Manufacturer { get; set; } = "";
            public ulong MaxHeight { get; set; }
            public float MaxAirTemperature { get; set; }
            public byte PassengerCapacity { get; set; }
            public double Radius { get; set; }
        }

        // Test Scenario: Default Deduction for List/Set Relations (✓identified✓)
        public class Brothel {
            public record struct Service(string Description, decimal CostPerHour);

            [PrimaryKey] public string Address { get; set; } = "";
            public string Country { get; set; } = "";
            public string? Proprietor { get; set; }
            public RelationList<string> Employees { get; set; } = new();
            public decimal YearlyRevenue { get; set; }
            public bool IsLegal { get; set; }
            public RelationSet<Service> Services { get; set; } = new();
        }

        // Test Scenario: Default Deduction for Map Relation (✓identified✓)
        public class Cult {
            public record struct Info(DateTime Joined, bool Alive, string Position);

            [PrimaryKey] public string Title { get; set; } = "";
            public string Leader { get; set; } = "";
            public DateTime Founded { get; set; }
            public DateTime? Shuttered { get; set; }
            public RelationMap<string, Info> Members { get; set; } = new();
            public bool FederallyMonitored { get; set; }
            public ulong AttributableDeaths { get; set; }
        }

        // Test Scenario: Default Deduction for OrderedList Relation (✓identified✓)
        public class PianoSonata {
            public enum Tone { A, B, C, D, E, F, G, Rest }
            public enum Accentuation { Sharp, Flat, Natural }
            public enum Length { Full, Half, Quarter, Eight, Sixteenth }
            public record struct TimeSignature(byte Top, byte Bottom);
            public record struct Note(Tone Tone, Accentuation? Accent, Length Beat);

            [PrimaryKey] public string Composer { get; set; } = "";
            [PrimaryKey] public ushort OpusNumber { get; set; }
            public string? Nickname { get; set; }
            public double Duration { get; set; }
            public TimeSignature Signature { get; set; }
            public sbyte Movements { get; set; }
            public RelationOrderedList<Note> Score { get; set; } = new();
        }

        // Test Scenario: Single Candidate Key on Relation Including Anchor (✓identified✓)
        public class ChromeExtension {
            public record struct Rating(string Reviewer, DateTime Timestamp, double Stars);

            [PrimaryKey] public Guid ExtensionID { get; set; }
            public string ExtensionName { get; set; } = "";
            public ulong Downloads { get; set; }
            [Unique("Key", Path = "ChromeExtension.ExtensionID"), Unique("Key", Path = "Item.Reviewer")] public RelationList<Rating> Reviews { get; set; } = new();
            public string CurrentVersion { get; set; } = "";
            public ulong Size { get; set; }
            public bool ByGoogle { get; set; }
        }

        // Test Scenario: Single Candidate Key on Relation Excluding Anchor (✓identified✓)
        public class Zipline {
            [PrimaryKey] public Guid ZiplineID { get; set; }
            public string Country { get; set; } = "";
            public ulong Length { get; set; }
            public ulong MaxHeight { get; set; }
            public ulong MinHeight { get; set; }
            public float TopSpeed { get; set; }
            [Unique(Path = "Item")] public RelationSet<string> Precautions { get; set; } = new();
            public bool Aquatic { get; set; }
            public bool Forested { get; set; }
        }

        // Test Scenario: Nullable Field Named `ID` + Non-Nullable Field Named `<EntityType>ID` (✓identified✓)
        public class Rollercoaster {
            public Guid? ID { get; set; }
            public ulong RollercoasterID { get; set; }
            public double Drop { get; set; }
            public double TopSpeed { get; set; }
            public DateTime Opening { get; set; }
        }

        // Test Scenario: Nullable Field Named `<EntityType>ID` + Single Candidate Key with Non-Nullable Fields (✓identified✓)
        public class Doctor {
            public ushort? DoctorID { get; set; }
            [Unique("DoctorWho")] public int Regeneration { get; set; }
            [Unique("DoctorWho")] public string Portrayal { get; set; } = "";
            public uint EpisodeCount { get; set; }
            public bool NewWho { get; set; }
        }

        // Test Scenario: Single Candidate Key with Nullable Fields + Only One Non-Nullable Field (✓identified✓)
        public class Polyhedron {
            public string Name { get; set; } = "";
            [Unique("Euler")] public ulong? Faces { get; set; }
            [Unique("Euler")] public ulong? Vertices { get; set; }
            [Unique("Euler")] public ulong? Edges { get; set; }
            public decimal? DihedralAngle { get; set; }
        }

        // Test Scenario: Scalar Property is Marked as [PrimaryKey] Multiple Times Directly (✓redundant✓)
        public class Airport {
            [PrimaryKey, PrimaryKey] public string IATA { get; set; } = "";
            public string Name { get; set; } = "";
            public string City { get; set; } = "";
            public DateTime Opening { get; set; }
            public float AveragePassengers { get; set; }
        }

        // Test Scenario: Scalar Property is Marked as [PrimaryKey] Multiple Times Indirectly (✓redundant✓)
        public class CompressionFormat {
            public record struct Version(byte Major, byte Minor, byte Patch);

            public string FormatName { get; set; } = "";
            [PrimaryKey] public string Suffix { get; set; } = "";
            public bool IsLossless { get; set; }
            [PrimaryKey, PrimaryKey(Path = "Major")] public Version LastStableRelease { get; set; }
            public string Developers { get; set; } = "";
        }

        // Test Scenario: Nullable Scalar Field is Marked as [PrimaryKey] (✗illegal✗)
        public class NorseWorld {
            [PrimaryKey] public string OldNorse { get; set; } = "";
            public string English { get; set; } = "";
            [PrimaryKey] public int? EddaMentions { get; set; }
        }

        // Test Scenario: Nullable Aggregate Field is Marked as [PrimaryKey] (✗illegal✗)
        public class MedievalCastle {
            public record struct Drawbridge(bool Manpowered, long Weight, ushort Length);

            [PrimaryKey] public Guid CastleID { get; set; }
            public ushort Towers { get; set; }
            public ulong Area { get; set; }
            public ulong WallHeight { get; set; }
            [PrimaryKey] public Drawbridge? DrawBridge { get; set; }
        }

        // Test Scenario: Aggregate with Nullable Nested Field is Marked as [PrimaryKey] (✗illegal✗)
        public class Wizard {
            public enum School { Abjuration, Conjuration, Divination, Enchantment, Evocation, Illusion, Necromancy, Transmutation }
            public record struct Schooling(School? School, ushort? Years);
            public record struct Qualifications(Schooling Schooling);

            [PrimaryKey] public string Name { get; set; } = "";
            public ushort Level { get; set; }
            public uint Age { get; set; }
            public string Council { get; set; } = "";
            [PrimaryKey] public Qualifications Background { get; set; }
            public ulong KnownSpells { get; set; }
            public string DeathCurse { get; set; } = "";
        }

        // Test Scenario: Nullable Reference Field is Marked as [PrimaryKey] (✗illegal✗)
        public class Avocado {
            public class Country {
                [PrimaryKey] public string Name { get; set; } = "";
                public string Endonym { get; set; } = "";
                public ulong Population { get; set; }
                public string CountryCode { get; set; } = "";
                public string CapitalCity { get; set; } = "";
            }

            [PrimaryKey] public Guid AvocadoID { get; set; }
            [PrimaryKey] public Country? CountryOfOrigin { get; set; }
            public double Weight { get; set; }
            public uint Calories { get; set; }
            public bool Hass { get; set; }
        }

        // Test Scenario: Field in Aggregate is Marked as [PrimaryKey] (✗illegal✗)
        public class LunarCrater {
            public struct Coordinate {
                public float Latitude { get; set; }
                [PrimaryKey] public float Longitude { get; set; }
            }

            [PrimaryKey] public Guid CraterID { get; set; }
            public string Name { get; set; } = "";
            public Coordinate Location { get; set; }
            public double Depth { get; set; }
            public double Diameter { get; set; }
        }

        // Test Scenario: Primary Key on Principal Table Cannot Be Deduced (✗illegal✗)
        public class FederalLaw {
            public string CommonName { get; set; } = "";
            public string? ShortName { get; set; }
            public DateTime Enacted { get; set; }
            public float StatuteIdentifier { get; set; }
            public string IntroducedBy { get; set; } = "";
        }

        // Test Scenario: Primary Key on Relation Table with No Candidate Keys Cannot Be Deduced (✗illegal✗)
        public class Lagerstatte {
            public record struct Fossil(string ScientificName, string? CommonName, DateTime FirstDiscovered);

            [PrimaryKey] public string Name { get; set; } = "";
            public float Latitude { get; set; }
            public float Longitude { get; set; }
            public DateTime CarbonDating { get; set; }
            public string GeologicEra { get; set; } = "";
            public RelationSet<Fossil> Fossils { get; set; } = new();
        }

        // Test Scenario: Primary Key on Relation Table with Candidate Keys Cannot Be Deduced (✗illegal✗)
        public class Blockbuster {
            [PrimaryKey] public Guid ID { get; set; }
            public string Manager { get; set; } = "";
            public DateTime Opened { get; set; }
            public DateTime? Closed { get; set; }
            [Unique("V", Path = "Blockbuster.ID"), Unique("V", Path = "Value")] public RelationMap<string, string> Rentals { get; set; } = new();
            public ushort TotalVideos { get; set; }
            public decimal LifetimeRevenue { get; set; }
            public bool IsFranchise { get; set; }
        }

        // Test Scenario: <Path> is `null` (✗illegal✗)
        public class Alphabet {
            [PrimaryKey(Path = null!)] public string Name { get; set; } = "";
            public ushort NumLetters { get; set; }
            public ushort NumConsonants { get; set; }
            public ushort NumVowels { get; set; }
            public bool IsCaseSensitive { get; set; }
            public DateTime? Developed { get; set; }
        }

        // Test Scenario: <Path> on Scalar Does Not Exist (✗non-existent path✗)
        public class Highway {
            [PrimaryKey(Path = "---")] public int Number { get; set; }
            public string StartingState { get; set; } = "";
            public string EndingState { get; set; } = "";
            public ulong LengthMiles { get; set; }
            public float AverageSpeedLimit { get; set; }
        }

        // Test Scenario: <Path> on Aggregate Does Not Exist (✗non-existent path✗)
        public class ConfidenceInterval {
            public record struct MarginOfError(ushort PlusOrMinus);

            public double Percentage { get; set; }
            public double LowerBound { get; set; }
            public double UpperBound { get; set; }
            [PrimaryKey(Path = "---")] public MarginOfError PlusMinus { get; set; }
        }

        // Test Scenario: <Path> on Reference Does Not Exist (✗non-existent path✗)
        public class PhoneBooth {
            public enum Color { Red, Orange, Yellow, Green, Blue, Purple, White, Black, Gray, Gold, Brown, Silver, Pink };

            public class Company {
                [PrimaryKey] public Guid CompanyID { get; set; }
                public string Name { get; set; } = "";
                public decimal AnnualRevenue { get; set; }
                public string CEO { get; set; } = "";
                public string Headquarteres { get; set; } = "";
            }

            [PrimaryKey] public Guid PhoneBoothID { get; set; }
            public double Length { get; set; }
            public double Width { get; set; }
            public double Height { get; set; }
            public ulong CallsMade { get; set; }
            public decimal CostPerCall { get; set; }
            [PrimaryKey(Path = "---")] public Company Manufacturer { get; set; } = new();
            public bool UsedBySuperman { get; set; }
            public bool IsTARDIS { get; set; }
        }

        // Test Scenario: <Path> on Reference Refers to Non-Primary-Key Field (✗non-existent path✗)
        public class ScientificExperiment {
            public enum Branch { Physics, Chemistry, Biology, Geology, Astronomy, EarthScience, ComputerScience, Medicine };

            public class Subject {
                [PrimaryKey] public string Description { get; set; } = "";
                public int Quantity { get; set; }
                public bool Animate { get; set; }
            }

            [PrimaryKey] public string Experimenter { get; set; } = "";
            [PrimaryKey] public string Title { get; set; } = "";
            public DateTime Conducted { get; set; }
            public Branch Science { get; set; }
            public Subject ExperimentalGroup { get; set; } = new();
            [PrimaryKey(Path = "Animate")] public Subject ControlGroup { get; set; } = new();
            public string Hypothesis { get; set; } = "";
            public string Conclusion { get; set; } = "";
        }

        // Test Scenario: <Path> on Reference Refers to Aggregate Housing (Part of) Primary Key (✗non-existent path✗)
        public class Cryochamber {
            public enum Scale { Fahrenheit, Celsius, Kelvin }

            public class Temperature {
                public record struct Temp(double Value, Scale Unit);

                [PrimaryKey] public Temp Measurement { get; set; }
                public double FeelsLike { get; set; }
            }

            public Guid ID { get; set; }
            public string Model { get; set; } = "";
            [PrimaryKey(Path = "Temp")] public Temperature MinTemperature { get; set; } = new();
            public Temperature MaxTemperature { get; set; } = new();
            public DateTime LastInspected { get; set; }
            public ushort FrostbiteIncidents { get; set; }
        }

        // Test Scenario: <Path> on Relation Does Not Exist (✗non-existent path✗)
        public class Missile {
            public enum Terrain { Surface, Air }

            [PrimaryKey] public Guid MissileID { get; set; }
            public string Model { get; set; } = "";
            public Terrain LaunchedFrom { get; set; }
            public Terrain Targeting { get; set; }
            public ulong MaxRange { get; set; }
            [PrimaryKey(Path = "---")] public RelationSet<string> Manufacturers { get; set; } = new();
        }

        // Test Scenario: <Path> on Relation Refers to Non-Primary-Key Field of Anchor Entity (✗non-existent path✗)
        public class TreasureMap {
            public record struct Coordinate(float Latitude, float Longitude);

            [PrimaryKey] public Guid ID { get; set; }
            public string? Author { get; set; }
            public decimal TreasureValue { get; set; }
            public Coordinate X { get; set; }
            [PrimaryKey(Path = "TreasureMap.X")] public RelationList<Coordinate> SuggestedPath { get; set; } = new();
            public bool Damaged { get; set; }
        }

        // Test Scenario: <Path> on Relation Not Specified (✗missing path✗)
        public class Hologram {
            public Guid ID { get; set; }
            public double Height { get; set; }
            public double AspectRatio { get; set; }
            [PrimaryKey] public RelationMap<short, string> Copyrights { get; set; } = new();
            public bool ThreeDimensional { get; set; }
        }
    }

    internal static class PrimaryKeyNaming {
        // Test Scenario: Named Primary Key on Entity Type (✓named✓)
        [NamedPrimaryKey("LetterPK")]
        public class HebrewLetter {
            [PrimaryKey] public char Letter { get; set; }
            public char IPA { get; set; }
            public uint GematriaValue { get; set; }
            public byte Position { get; set; }
            public bool IsMaterLectionis { get; set; }
        }

        // Test Scenario: Named Primary Key when Unnamed Candidate Key was Deduced (✓named✓)
        [NamedPrimaryKey("PrimaryKey")]
        public class DatabaseField {
            [Unique] public string QualifiedName { get; set; } = "";
            public uint ColumnIndex { get; set; }
            public byte DataType { get; set; }
            public bool Nullable { get; set; }
        }

        // Test Scenario: Primary Key Name same as Name of Deduced Candidate Key (✓redundant✓)
        [NamedPrimaryKey("PK")]
        public class TimeZone {
            [Unique("PK")] public float GMT { get; set; }
            public string Name { get; set; } = "";
        }

        // Test Scenario: Primary Key Name different than Name of Deduced Candidate Key (✓overrides✓)
        [NamedPrimaryKey("Something")]
        public class JigsawPuzzle {
            [Unique("GlobalIdentifier")] public Guid PuzzleIDFR { get; set; }
            public ushort NumPieces { get; set; }
            public bool Is3D { get; set; }
        }

        // Test Scenario: Primary Key Name same as Non-Deduced Candidate Key (✗conflicting✗)
        [NamedPrimaryKey("Key13")]
        public class Currency {
            [PrimaryKey] public string CountryOfUse { get; set; } = "";
            [Unique("Key13")] public char Character { get; set; }
            [Unique("Key13")] public double ExchangeRate { get; set; }
            public long MaxDenomination { get; set; }
            public long MinDenomination { get; set; }
        }

        // Test Scenario: Primary Key Name is null (✗illegal✗)
        [NamedPrimaryKey(null!)]
        public class HinduGod {
            [PrimaryKey] public string SanskritName { get; set; } = "";
            public string HindiName { get; set; } = "";
            public string EnglishName { get; set; } = "";
            public string Mount { get; set; } = "";
            public byte NumAvatars { get; set; }
            public bool InTrimurti { get; set; }
        }

        // Test Scenario: Primary Key Name is the Empty String (✗illegal✗)
        [NamedPrimaryKey("")]
        public class Bay {
            [PrimaryKey] public decimal Latitude { get; set; }
            [PrimaryKey] public decimal Longitude { get; set; }
            public string Name { get; set; } = "";
            public ulong SurfaceArea { get; set; }
            public ulong MaxDepth { get; set; }
            public ulong Coastline { get; set; }
        }
    }

    internal static class CandidateKeys {
        // Test Scenario: Multiple Unnamed Candidate Keys (✓recognized✓)
        public class Inmate {
            [PrimaryKey] public Guid PrisonerNumber { get; set; }
            [Unique] public int SSN { get; set; }
            [Unique] public string FullName { get; set; } = "";
        }

        // Test Scenario: Named Candidate Key (✓recognized✓)
        public class BowlGame {
            [PrimaryKey] public string Name { get; set; } = "";
            [Unique("Sponsorship")] public string PrimarySponsor { get; set; } = "";
            [Unique("Sponsorship")] public string? SecondarySponsor { get; set; } = "";
            public DateTime Inception { get; set; }
            public DateTime NextScheduled { get; set; }
        }

        // Test Scenario: Single Field in Multiple Candidate Keys (✓recognized✓)
        public class KingOfEngland {
            [PrimaryKey] public DateTime ReignStart { get; set; }
            [PrimaryKey] public DateTime ReignEnd { get; set; }
            [Unique("Uno"), Unique("Another")] public string RegnalName { get; set; } = "";
            [Unique("Uno"), Unique("Third")] public byte RegnalNumber { get; set; }
            [Unique("Another"), Unique("Third")] public string RoyalHouse { get; set; } = "";
        }

        // Test Scenario: Duplicate Unnamed Candidate Keys Directly (✓de-duplicated✓)
        public class Pigment {
            [PrimaryKey] public string Name { get; set; } = "";
            public string DominantColor { get; set; } = "";
            [Unique, Unique] public string ChemicalFormula { get; set; } = "";
        }

        // Test Scenario: Duplicate Unnamed Candidate Keys Indirectly (✓de-duplicated✓)
        public class Octopus {
            [Flags] public enum Region { Ocean = 1, CoralReef = 2, PelagicZone = 4, Intertidal = 8, Abyssal = 16 }
            public struct Taxonomy {
                [Unique] public string Family { get; set; }
                public string Genus { get; set; }
                public string Species { get; set; }
            }

            [PrimaryKey] public string CommonName { get; set; } = "";
            [Unique(Path = "Family")] public Taxonomy Nomenclature { get; set; }
            public bool CanCamouflage { get; set; }
            public double AverageWeight { get; set; }
            public Region Habitat { get; set; }
        }

        // Test Scenario: Duplicate Named Candidate Keys (✓de-duplicated✓)
        public class BankCheck {
            [PrimaryKey] public Guid CID { get; set; }
            public string Signatory { get; set; } = "";
            public decimal Amount { get; set; }
            public ulong RoutingNumber { get; set; }
            [Unique("N1"), Unique("N2"), Unique("N3")] public byte CheckNumber { get; set; }
        }

        // Test Scenario: Field Placed in the Same Candidate Key Twice (✓de-duplicated✓)
        public class Desert {
            [PrimaryKey] public string Name { get; set; } = "";
            [Unique("Size"), Unique("Size")] public ulong Length { get; set; }
            [Unique("Size")] public ulong Width { get; set; }
            public ulong TotalArea { get; set; }
        }

        // Test Scenario: Aggregate Field in Candidate Key with No Other Fields (✓recognized✓)
        public class Ointment {
            public enum BaseType { Absorption, Emulsifying, Hydrocarbon, Vegetable, WaterSoluble };
            public record struct WaterOil(double PcntWater, double PcntOil);

            [PrimaryKey] public Guid OintmentID { get; set; }
            public string ApplyTo { get; set; } = "";
            public string MedicationName { get; set; } = "";
            public BaseType Base { get; set; }
            [Unique] public WaterOil Composition { get; set; }
        }

        // Test Scenario: Aggregate Field in Candidate Key with Other Fields (✓recognized✓)
        public class Shipwreck {
            public record struct Coordinate(float Latitude, float Longitude);

            [PrimaryKey] public Guid ShipwreckTag { get; set; }
            [Unique("Identity")] public string Ship { get; set; } = "";
            [Unique("Identity")] public Coordinate Location { get; set; }
            [Unique] public Coordinate FurthestExtent { get; set; }
            public DateTime Sinking { get; set; }
            public ulong Area { get; set; }
            public ulong Depth { get; set; }
            public bool IsHeritageSite { get; set; }
        }

        // Test Scenario: Aggregate-Nested Scalar Fields in Candidate Key (✓recognized✓)
        public class SpiderMan {
            public record struct Person(string FirstName, string? MiddleName, string LastName);

            [PrimaryKey] public Guid SpiderManID { get; set; }
            public uint FirstEditionAppearance { get; set; }
            [Unique("AlterEgo", Path = "FirstName"), Unique("AlterEgo", Path = "LastName")] public Person AlterEgo { get; set; }
            [Unique("Portrayal", Path = "FirstName"), Unique("Portrayal", Path = "MiddleName"), Unique("Portrayal", Path = "LastName")] public Person? Portrayal { get; set; }
            public ushort Height { get; set; }
            public bool IsMale { get; set; }
            public string SpiderName { get; set; } = "";
        }

        // Test Scenario: Nested Aggregate Fields in Candidate Key (✓recognized✓)
        public class Neurotransmitter {
            public enum Category { AminoAcid, Monoamine, Peptide, Purine, Other }
            public record struct Naming(string Name, string Abbreviation);
            public record struct Entry(Naming Name, Category Categorization);

            [PrimaryKey] public Guid NeurotransmitterID { get; set; }
            [Unique(Path = "Name")] public Entry Definition { get; set; }
        }

        // Test Scenario: Aggregate Field Natively [Unique] (✓propagated✓)
        public class ZoomMeeting {
            public struct Credentialization {
                [Unique] public Guid MeetingID { get; set; }
                [Unique("JoinKey")] public string MeetingNumber { get; set; }
                [Unique("JoinKey")] public uint PassCode { get; set; }
            }

            [PrimaryKey] public DateTime MeetingTime { get; set; }
            [PrimaryKey] public string Account { get; set; } = "";
            public Credentialization Credentials { get; set; }
            public ushort MeetingLength { get; set; }
            public ushort NumParticipants { get; set; }
        }

        // Test Scenario: Reference Field in Candidate Key with No Other Fields (✓recognized✓)
        public class Luau {
            public enum Hawaii { BigIsland, Oahu, Maui, Molokai, Kauai, Lanai, Niihau, Kahoolawe, Non }

            public class Pig {
                [PrimaryKey] public Guid BatchID { get; set; }
                [PrimaryKey] public int LotNumber { get; set; }
                public ulong Weight { get; set; }
                public string? Name { get; set; }
            }

            [PrimaryKey] public Guid LuauID { get; set; }
            public DateTime Date { get; set; }
            [Unique] public Pig SucklingPig { get; set; } = new();
            public ushort Ukeleles { get; set; }
            public ushort TikiTorches { get; set; }
            public Hawaii HawaiianIsland { get; set; }
        }

        // Test Scenario: Reference Field in Candidate Key with Other Fields (✓recognized✓)
        public class GreatOldOne {
            public class Epithet {
                [PrimaryKey] public string Name { get; set; } = "";
                public string LanguageOfOrigin { get; set; } = "";
                public string Meaning { get; set; } = "";
                public bool InActiveUse { get; set; }
            }

            [PrimaryKey] public string Name { get; set; } = "";
            [Unique("Identity", Path = "Name")] public Epithet PrimaryEpithet { get; set; } = new();
            [Unique("Identity")] public int PantheonNumber { get; set; }
            public bool IsDead { get; set; }
            public ushort Appearances { get; set; }
            public bool CouldDefeatCthulhuInFight { get; set; }
        }

        // Test Scenario: Reference-Nested Scalar Fields in Candidate Key (✓recognized✓)
        public class JapaneseEmperor {
            public class Era {
                [PrimaryKey] public string EnglishEraName { get; set; } = "";
                [PrimaryKey] public string JapaneseEraName { get; set; } = "";
                public DateTime Start { get; set; }
                public DateTime End { get; set; }
            }

            [PrimaryKey] public string RegnalName { get; set; } = "";
            public string PersonalName { get; set; } = "";
            [Unique("Eras", Path = "JapaneseEraName")] public Era StartEra { get; set; } = new();
            [Unique("Eras", Path = "EnglishEraName")] public Era EndEra { get; set; } = new();
        }

        // Test Scenario: Nested Reference Fields in Candidate Key (✓recognized✓)
        public class Kibbutz {
            public class Mekhoza {
                [PrimaryKey] public string Name { get; set; } = "";
                public ulong Area { get; set; }
                public ulong Population { get; set; }
            }
            public record struct Location(Mekhoza District, string NearestMajorCity);

            [PrimaryKey] public string HebrewName { get; set; } = "";
            [Unique("KBTZ")] public string EnglishName { get; set; } = "";
            [Unique("KBTZ", Path = "District")] public Location Where { get; set; }
            public ushort NumResidents { get; set; }
            public DateTime Established { get; set; }
            public string PrimaryAgriculture { get; set; } = "";
        }

        // Test Scenario: Primary Key Reference Fields Natively [Unique] (✓not propagated✓)
        public class DiscworldGuild {
            public class Book {
                [PrimaryKey, Unique] public string Title { get; set; } = "";
                [PrimaryKey] public ulong ISBN { get; set; }
                public ushort PageCount { get; set; }
                public DateTime Released { get; set; }
            }

            [PrimaryKey] public string Guild { get; set; } = "";
            public string Head { get; set; } = "";
            public Book FirstIntroduced { get; set; } = new();
            public ushort Members { get; set; }
            public bool InvestigatedByVimes { get; set; }
        }

        // Test Scenario: Non-Primary-Key Reference Fields Natively [Unique] (✓not propagated✓)
        public class HonestTrailer {
            public class YouTube {
                [PrimaryKey] public string Channel { get; set; } = "";
                [PrimaryKey] public string VideoHash { get; set; } = "";
                [Unique] public string VideoTitle { get; set; } = "";
                public ulong Length { get; set; }
                public ulong Views { get; set; }
            }

            [PrimaryKey] public string SourceFilm { get; set; } = "";
            public YouTube YouTubeVideo { get; set; } = new();
            public string HonestTitle { get; set; } = "";
            public bool EpicVoiceGuy { get; set; }
            public byte BewbsCount { get; set; }
        }

        // Test Scenario: Relation-Nested Scalar Fields in Candidate Key (✓recognized✓)
        public class BigBlockOfCheeseDay {
            public record struct Slot(string Organization, DateTime Time);

            [PrimaryKey] public ushort Episode { get; set; }
            public string ChiefOfStaff { get; set; } = "";
            [PrimaryKey(Path = "Key.Time"), Unique("X", Path = "BigBlockOfCheeseDay"), Unique("X", Path = "Key.Organization")] public RelationMap<Slot, string> Schedule { get; set; } = new();
            public double PercentCrackpots { get; set; }
        }

        // Test Scenario: Nested Relation Field in Candidate Key (✗illegal✗)
        public class RentalCar {
            public record struct Duration(DateTime Start, DateTime End);
            public record struct History(RelationMap<string, Duration> Renters, DateTime Acquired);

            [PrimaryKey] public string LicensePlate { get; set; } = "";
            public decimal CostPerDay { get; set; }
            public ushort GasGallons { get; set; }
            public ulong Miles { get; set; }
            public string Make { get; set; } = "";
            public string Model { get; set; } = "";
            [Unique(Path = "Renters")] public History Report { get; set; }
        }

        // Test Scenario: Relation Field Natively [Unique] (✓propagated✓)
        public class Intifada {
            public struct Country {
                [Unique] public string CountryName { get; set; }
                [Unique("Code")] public short CountryCode { get; set; }
                public bool IsArab { get; set; }
                public ulong Population { get; set; }
            }

            [PrimaryKey] public string Name { get; set; } = "";
            public DateTime WarStart { get; set; }
            public DateTime WarEnd { get; set; }
            [PrimaryKey(Path = "Intifada.Name"), PrimaryKey(Path = "Item.CountryName")] public RelationSet<Country> Belligerents { get; set; } = new();
            public ulong Casualties { get; set; }
        }

        // Test Scenario: Anchor + Key in Candidate Key (✓redundant✓)
        public class VoodooDoll {
            public enum Color { Red, Orange, Yellow, Green, Blue, Purple, White, Black, Gray, Brown, Pink, Gold }
            public enum Thing { Straw, Cloth, Fruit, Hay, Cotton, Paper, Sand }

            [PrimaryKey] public Guid VoodooID { get; set; }
            public string Target { get; set; } = "";
            public string? PatronLoa { get; set; }
            [PrimaryKey(Path = "VoodooDoll.VoodooID"), PrimaryKey(Path = "Value"), Unique("Unique", Path = "VoodooDoll.VoodooID"), Unique("Unique", Path = "Key")] public RelationMap<Color, string> Pins { get; set; } = new();
            public bool Effective { get; set; }
            public Thing Material { get; set; }
        }

        // Test Scenario: Anchor + Index in Candidate Key (✓redundant✓)
        public class OPO {
            [PrimaryKey] public Guid ID { get; set; }
            public string Name { get; set; } = "";
            public string State { get; set; } = "";
            public DateTime Inception { get; set; }
            public ulong HeartsProcessed { get; set; }
            public ulong LungsProcessed { get; set; }
            public ulong LiversProcessed { get; set; }
            public ulong KidneysProcessed { get; set; }
            [PrimaryKey(Path = "OPO.ID"), PrimaryKey(Path = "Item"), Unique("Unique", Path = "OPO.ID"), Unique("Unique", Path = "Index")] public RelationOrderedList<string> Administrators { get; set; } = new();
        }

        // Test Scenario: Scalar Fields in Same Candidate Key as Nested Fields (✓recognized✓)
        public class Sabermetric {
            public enum Phase { Offensive, Defensive, Pitching, Baserunning, Managing }
            public struct Formulation {
                [Unique("Lookup")] public string Formula { get; set; }
                public double? Minimum { get; set; }
                public double? Maximum { get; set; }
            }

            [PrimaryKey] public string StatisticName { get; set; } = "";
            public bool FromBillJames { get; set; }
            public DateTime FirstPublished { get; set; }
            [Unique("Lookup")] public Phase GamePhase { get; set; }
            public Formulation Formula { get; set; }
        }

        // Test Scenario: Candidate Key Named `null` (✗illegal✗)
        public class PlatonicDialogue {
            [PrimaryKey] public string Title { get; set; } = "";
            public string OpposingSocrates { get; set; } = "";
            [Unique(null!)] public ulong WordCount { get; set; }
            public short PublicationYear { get; set; }
        }

        // Test Scenario: Candidate Key Named with the Empty String (✗illegal✗)
        public class Allomancy {
            [PrimaryKey] public char Symbol { get; set; }
            public string Metal { get; set; } = "";
            public byte Categorization { get; set; }
            public bool IsInternal { get; set; }
            public bool IsPushing { get; set; }
            [Unique("")] public string MistingTerm { get; set; } = "";
        }

        // Test Scenario: Candidate Key Named with Reserved Name (✗illegal✗)
        public class Lens {
            [PrimaryKey] public Guid LensID { get; set; }
            public bool AreContacts { get; set; }
            [Unique($"@@@Key")] public double IndexOfRefraction { get; set; }
        }

        // Test Scenario: Candidate Key is Equivalent to the Primary Key (✓redundant✓)
        public class AchaeanNavalContingent {
            [PrimaryKey, Unique("Navy")] public string Identity { get; set; } = "";
            [PrimaryKey, Unique("Navy")] public ushort LineNumber { get; set; }
            public int NumShips { get; set; }
            public string Captain { get; set; } = "";
        }

        // Test Scenario: Candidate Key is Improper Superset of the Primary Key (✓redundant✓)
        public class WorldHeritageSite {
            [PrimaryKey, Unique("X")] public string Name { get; set; } = "";
            [Unique("X")] public DateTime Inscription { get; set; }
            [PrimaryKey, Unique("X")] public ulong Area { get; set; }
            [Unique("X")] public sbyte Components { get; set; }
            public ushort ReferenceNumber { get; set; }
        }

        // Test Scenario: <Path> is `null` (✗illegal✗)
        public class Tendon {
            [PrimaryKey] public string MESH { get; set; } = "";
            [Unique(Path = null!)] public string Name { get; set; } = "";
            public string From { get; set; } = "";
            public string To { get; set; } = "";
        }

        // Test Scenario: <Path> on Scalar Does Not Exist (✗non-existent path✗)
        public class Sonnet {
            [PrimaryKey] public string Author { get; set; } = "";
            [PrimaryKey] public string Title { get; set; } = "";
            [Unique(Path = "---")] public string Line1 { get; set; } = "";
            public string Line2 { get; set; } = "";
            public string Line3 { get; set; } = "";
            public string Line4 { get; set; } = "";
            public string Line5 { get; set; } = "";
            public string Line6 { get; set; } = "";
            public string Line7 { get; set; } = "";
            public string Line8 { get; set; } = "";
            public string Line9 { get; set; } = "";
            public string Line10 { get; set; } = "";
            public string Line11 { get; set; } = "";
            public string Line12 { get; set; } = "";
            public string Line13 { get; set; } = "";
            public string Line14 { get; set; } = "";
        }

        // Test Scenario: <Path> on Aggregate Does Not Exist (✗non-existent path✗)
        public class EgyptianGod {
            public record struct EgyptianName(string English, string? Hieroglyphics);

            [PrimaryKey] public int DeityID { get; set; }
            public uint BookOfTheDeadMentions { get; set; }
            [Unique(Path = "---")] public EgyptianName Name { get; set; }
            public string? PrimaryCultCity { get; set; }
            public string Domain { get; set; } = "";
        }

        // Test Scenario: <Path> on Reference Does Not Exist (✗non-existent path✗)
        public class Bachelorette {
            public class Bachelor {
                [PrimaryKey] public string FirstName { get; set; } = "";
                [PrimaryKey] public string LastName { get; set; } = "";
                public double Height { get; set; }
                public double Weight { get; set; }
                public float Attractiveness { get; set; }
            }

            [PrimaryKey] public string FirstName { get; set; } = "";
            [PrimaryKey] public string LastName { get; set; } = "";
            public double Height { get; set; }
            public double Weight { get; set; }
            public float Attractiveness { get; set; }
            public byte Season { get; set; }
            public bool Engaged { get; set; }
            [Unique(Path = "---")] public Bachelor FinalRose { get; set; } = new();
        }

        // Test Scenario: <Path> on Reference Refers to Non-Primary-Key Field (✗non-existent path✗)
        public class Sherpa {
            public class Mountain {
                [PrimaryKey] public string Name { get; set; } = "";
                public ulong Height { get; set; }
                public bool IsFourteener { get; set; }
                public ulong TotalAscents { get; set; }
            }

            [PrimaryKey] public Guid MountaineeringID { get; set; }
            public string Name { get; set; } = "";
            public ulong TotalAscents { get; set; }
            public DateTime FirstAscent { get; set; }
            [Unique("MOUNTAIN", Path = "TotalAscents")] public Mountain MainMountain { get; set; } = new();
        }

        // Test Scenario: <Path> on Reference Refers to Aggregate Housing (Part of) Primary Key (✓recognized✓)
        public class LawFirm {
            public enum Type { Family, Civil, Malpractice, Criminal, FirstAmendment, Mergers, Divorce, Other }

            public class Lawyer {
                public record struct Licensing(Guid BarNumber, DateTime AsOf);

                [PrimaryKey(Path = "BarNumber")] public Licensing License { get; set; }
                public string FirstName { get; set; } = "";
                public string LastName { get; set; } = "";
                [PrimaryKey] public string LawSchool { get; set; } = "";
            }
            public record struct Partnering(Lawyer FoundingPartner, Lawyer? Associate, Lawyer? Emeritus);

            [PrimaryKey] public string Name { get; set; } = "";
            public ulong Clients { get; set; }
            [Unique(Path = "FoundingPartner")] public Partnering Partners { get; set; }
            public ulong LawyerCount { get; set; }
            public double WinPercentage { get; set; }
        }

        // Test Scenario: <Path> on Relation Does Not Exist (✗non-existent path✗)
        public class Antihistamine {
            [Flags] public enum Symptom { Sneeze = 1, ItchyEyes = 2, RunnyNose = 4, Congestion = 8, Headache = 16, Hives = 32, Anaphylaxis = 64, Rash = 128 }

            [PrimaryKey] public string Name { get; set; } = "";
            public string Formula { get; set; } = "";
            public double Bioavalability { get; set; }
            [Unique(Path = "---")] public RelationMap<string, string> MedicalIdentifiers { get; set; } = new();
            public Symptom SymptomsRelieved { get; set; }
        }

        // Test Scenario: <Path> on Relation Refers to Non-Primary-Key Field of Anchor Entity (✗non-existent path✗)
        public class Oasis {
            [PrimaryKey] public float Latitude { get; set; }
            [PrimaryKey] public float Longitude { get; set; }
            public ulong Water { get; set; }
            [Unique(Path = "Oasis.Water")] public RelationSet<string> TreeSpecies { get; set; } = new();
            public double AverageTemperature { get; set; }
        }

        // Test Scenario: <Path> on Relation Not Specified (✗missing path✗)
        public class LimboCompetition {
            [PrimaryKey] public Guid ID { get; set; }
            public string Song { get; set; } = "";
            public ushort BarLength { get; set; }
            [Unique] public IReadOnlyRelationMap<string, float> Heights { get; set; } = new RelationMap<string, float>();
            public decimal PrizeMoney { get; set; }
            public bool SoberRequired { get; set; }
        }
    }

    internal static class DataConverters {
        // Test Scenario: Data Conversion does not Change Field's Type (✓applied✓)
        public class Cenote {
            [PrimaryKey] public string Name { get; set; } = "";
            public float MaxDepth { get; set; }
            [DataConverter(typeof(Invert))] public bool IsKarst { get; set; }
            public decimal Latitude { get; set; }
            [DataConverter(typeof(Identity<decimal>))] public decimal Longitude { get; set; }
        }

        // Test Scenario: Data Conversion Changes Field's Type to Scalar (✓applied✓)
        public class Comet {
            [PrimaryKey] public Guid AstronomicalIdentifier { get; set; }
            public double Aphelion { get; set; }
            [DataConverter(typeof(RoundDown))] public double Perihelion { get; set; }
            [DataConverter(typeof(RoundDown))] public double Eccentricity { get; set; }
            public ulong MassKg { get; set; }
            public double Albedo { get; set; }
            public float OrbitalPeriod { get; set; }
        }

        // Test Scenario: Data Conversion Changes Field's Type to Enumeration (✓applied✓)
        public class TitleOfYourSexTape {
            [PrimaryKey] public string Title { get; set; } = "";
            public string CharacterSaying { get; set; } = "";
            public string CharacterReceiving { get; set; } = "";
            [DataConverter(typeof(Enumify<int, DayOfWeek>))] public int DayOfWeek { get; set; }
            public int Season { get; set; }
            public int EpisodeNumber { get; set; }
            public double Timestamp { get; set; }
        }

        // Test Scenario: Data Conversion Changes Field's Type to Different Enumeration (✓applied✓)
        public class VestigeOfDivergence {
            public enum Campaign { C1, C2, C3 }
            public enum Party { VoxMachina, MightyNein, BellsHells, DarringtonBrigade, CrownKeepers }
            public enum State { Dormant, Awakened, Exalted }

            [PrimaryKey] public string Name { get; set; } = "";
            public string Deity { get; set; } = "";
            public string? Wielder { get; set; }
            [DataConverter(typeof(SwapEnums<Campaign, Party>))] public Campaign? IntroducedIn { get; set; }
            public byte AttackBonus { get; set; }
            public int AverageDamage { get; set; }
            public State CurrentState { get; set; }
        }

        // Test Scenario: Custom Data Conversion for Enumeration Field (✓applied✓)
        public class HomeRunDerby {
            public enum Variety { Bracketed, SingleElimination, SingleRound, GroupProgression }

            [PrimaryKey] public uint Year { get; set; }
            public string Victor { get; set; } = "";
            public ushort TotalHomers { get; set; }
            public ushort LongestHomer { get; set; }
            public decimal CharityMoney { get; set; }
            [DataConverter(typeof(MakeDate<Variety>))] public Variety Structure { get; set; }
        }

        // Test Scenario: Custom Data Conversion for Boolean Field (✓applied✓)
        public class MathematicalConjecture {
            [PrimaryKey] public string Name { get; set; } = "";
            public bool IsMillenniumPrize { get; set; }
            [DataConverter(typeof(ToInt<bool>))] public bool Solved { get; set; }
            public string? Equation { get; set; }
            public DateTime FirstPosited { get; set; }
        }

        // Test Scenario: [Numeric] Data Conversion for Enumeration Field (✓applied✓)
        public class Quarterback {
            public enum Throws : sbyte { Left, Right }
            public enum Round : short { R1 = 14, R2 = 188, R3 = 2, R4 = -16, R5 = 19054, R6 = -333, R7 = 0, Undrafted = 8 }
            public enum Style : int { PocketPasser, Mobile, Gunslinger, BackUp }
            [Flags] public enum Accolade : long { ProBowl = 1, AllPro = 2, OPOY = 4, MVP = 8, HallOfFame = 16 }
            public enum PlayerStatus : byte { Retired, Active, Injured, PracticeSquad, FreeAgent }
            public enum YesNo : ushort { Yes, No }
            public enum PlayoffRound : uint { Never = 0, WildCard = 1827412, Champ = 44, SuperBowl = 949012, Lombardy = 55 }
            [Flags] public enum League : ulong { NFL = 128, XFL = 2, CFL = 64 }

            [PrimaryKey] public string Name { get; set; } = "";
            [Numeric] public Throws ThrowingArm { get; set; }
            [Numeric] public Round DraftRound { get; set; }
            [Numeric] public Style QBStyle { get; set; }
            [Numeric] public Accolade CareerAchievements { get; set; }
            [Numeric] public PlayerStatus Status { get; set; }
            [Numeric] public YesNo HasBeenTraded { get; set; }
            [Numeric] public PlayoffRound FurthestPlayoffAdvancement { get; set; }
            [Numeric] public League Leagues { get; set; }
        }

        // Test Scenario: [AsString] Data Conversion for Enumeration Field (✓applied✓)
        public class EcumenicalCouncil {
            public enum Recognition { CatholicChurch, EasternOrthodoxChurch, Unrecognized }

            [PrimaryKey] public string Name { get; set; } = "";
            public DateTime Opening { get; set; }
            public DateTime Closing { get; set; }
            [AsString] public Recognition RecognizedBy { get; set; }
            public uint Attendance { get; set; }
        }

        // Test Scenario: [DataConverter] Applied to Aggregate Field (✗impermissible✗)
        public class Joust {
            public record struct Person(string Name, ushort Height, ushort Weight);

            [PrimaryKey] public Guid JoustID { get; set; }
            public string Tourney { get; set; } = "";
            public Person KnightA { get; set; }
            [DataConverter(typeof(ToInt<Person>))] public Person KnightB { get; set; }
            public double Odds { get; set; }
            public bool Fatal { get; set; }
        }

        // Test Scenario: [DataConverter] Applied to Reference Field (✗impermissible✗)
        public class Decathlon {
            public class Athlete {
                [PrimaryKey] public ulong Number { get; set; }
                public string FirstName { get; set; } = "";
                public string LastName { get; set; } = "";
                public float Height { get; set; }
                public double Weight { get; set; }
            }

            [PrimaryKey] public Guid RaceID { get; set; }
            [DataConverter(typeof(ToInt<Athlete>))] public Athlete Winner { get; set; } = new();
            public double BestMeters110 { get; set; }
            public double BestLongJump { get; set; }
            public double BestShotPut { get; set; }
            public double BestHighJump { get; set; }
            public double BestMeters400 { get; set; }
            public double BestHurdles110 { get; set; }
            public double BestDiscusThrow { get; set; }
            public double BestPoleVault { get; set; }
            public double BestJavelinThrow { get; set; }
            public double BestMeters1500 { get; set; }
        }

        // Test Scenario: [DataConverter] Applied to Relation Field (✗impermissible✗)
        public class Bank {
            [PrimaryKey] public uint FDICNumber { get; set; }
            public string Name { get; set; } = "";
            [DataConverter(typeof(ToInt<RelationMap<ulong, decimal>>))] public RelationMap<ulong, decimal> Accounts { get; set; } = new();
            public string VaultModel { get; set; } = "";
            public sbyte NumTellers { get; set; }
            public decimal CashOnHand { get; set; }
            public bool CanMakeLoans { get; set; }
            public byte NumTimesRobbed { get; set; }
            public bool Crypto { get; set; }
        }

        // Test Scenario: Data Conversion Source Type is Non-Nullable on Nullable Field (✓applied✓)
        public class RoyalHouse {
            [PrimaryKey] public string HouseName { get; set; } = "";
            public DateTime Founded { get; set; }
            [DataConverter(typeof(DeNullify<string>))] public string? CurrentHead { get; set; }
            [DataConverter(typeof(DeNullify<int>))] public int? TotalMonarchs { get; set; }
        }

        // Test Scenario: Data Conversion Source Type is Nullable on Non-Nullable Field (✓applied✓)
        public class Planeswalker {
            [PrimaryKey] public string Name { get; set; } = "";
            public sbyte MannaCost { get; set; }
            public sbyte InitialLoyalty { get; set; }
            [DataConverter(typeof(Nullify<char>))] public char SetIcon { get; set; }
            [DataConverter(typeof(Nullify<string>))] public string Ability1 { get; set; } = "";
            [DataConverter(typeof(Nullify<string>))] public string Ability2 { get; set; } = "";
            [DataConverter(typeof(Nullify<string>))] public string Ability3 { get; set; } = "";
            [DataConverter(typeof(Nullify<Guid>))] public Guid SerialNumber { get; set; }
        }

        // Test Scenario: CLR Type is Inconvertible to Data Conversion Source Type (✗invalid✗)
        public class Jedi {
            [PrimaryKey] public int WookiepediaID { get; set; }
            public string FirstName { get; set; } = "";
            [DataConverter(typeof(DeNullify<bool>))] public string? MiddleName { get; set; }
            public string? LastName { get; set; }
            public string LightsaberColor { get; set; } = "";
            public double Height { get; set; }
            public double Weight { get; set; }
            public int NumMovieLines { get; set; }
            public int NumTelevisionLines { get; set; }
        }

        // Test Scenario: CLR Type is Convertible to Data Conversion Source Type (✗invalid✗)
        public class ConstitutionalAmendment {
            [PrimaryKey, DataConverter(typeof(Nullify<long>))] public int Number { get; set; }
            public DateTime Ratified { get; set; }
            public double RatificationPercentage { get; set; }
            public string Text { get; set; } = "";
        }

        // Test Scenario: Data Conversion Target Type is Not Supported (✗illegal✗)
        public class SNLEpisode {
            [PrimaryKey] public byte Season { get; set; }
            [PrimaryKey] public byte EpisodeNumber { get; set; }
            public string Host { get; set; } = "";
            public string MusicalGuest { get; set; } = "";
            [DataConverter(typeof(ToError<DateTime>))] public DateTime AirDate { get; set; }
            public ushort WeekendUpdateDuration { get; set; }
        }

        // Test Scenario: Data Converter is not an `IDataConverter` (✗illegal✗)
        public class MetraRoute {
            [PrimaryKey] public int RouteID { get; set; }
            public string Name { get; set; } = "";
            public string SourceStation { get; set; } = "";
            public string Destination { get; set; } = "";
            [DataConverter(typeof(int))] public string Line { get; set; } = "";
            public ushort DepartureTime { get; set; }
        }

        // Test Scenario: Data Converter Cannot be Default Constructed (✗illegal✗)
        public class Paycheck {
            [PrimaryKey] public ulong Employee { get; set; }
            [PrimaryKey] public DateTime Period { get; set; }
            [DataConverter(typeof(ChangeBase))] public int HoursWorked { get; set; }
            public double RatePerHour { get; set; }
            public decimal Net { get; set; }
            public decimal FederalIncomeTax { get; set; }
            public decimal StateIncomeTax { get; set; }
            public decimal SocialSecurity { get; set; }
            public decimal OtherWithholdings { get; set; }
            public decimal Gross { get; set; }
        }

        // Test Scenario: Data Converter Throws Error upon Construction (✗propagated✗)
        public class Sword {
            [PrimaryKey] public string Name { get; set; } = "";
            public decimal Sharpness { get; set; }
            public float Length { get; set; }
            public float Weight { get; set; }
            public int Kills { get; set; }
            [DataConverter(typeof(Unconstructible<short>))] public short YearForged { get; set; }
        }

        // Test Scenario: Data Converter Throws upon Execution (✗propagated✗)
        public class Ligament {
            public enum Type { Articular, Pretioneal, FetalRemnant }

            [PrimaryKey] public string MeSH { get; set; } = "";
            public string Name { get; set; } = "";
            [DataConverter(typeof(Unconvertible<Type>))] public Type Classification { get; set; }
            public float Length { get; set; }
            public string From { get; set; } = "";
            public string To { get; set; } = "";
        }

        // Test Scenario: [Numeric] Applied to Boolean Field (✗impermissible✗)
        public class Pillow {
            [PrimaryKey] public Guid ID { get; set; }
            public float Length { get; set; }
            public float Width { get; set; }
            [Numeric] public bool IsThrowPillow { get; set; }
            public bool IsChilled { get; set; }
        }

        // Test Scenario: [Numeric] Applied to Textual Field (✗impermissible✗)
        public class VigenereCipher {
            [PrimaryKey, Numeric] public string Key { get; set; } = "";
            public bool IsCracked { get; set; }
        }

        // Test Scenario: [Numeric] Applied to Numeric Field (✗impermissible✗)
        public class Satellite {
            [PrimaryKey] public string Name { get; set; } = "";
            public double Weight { get; set; }
            public ushort MissionDuration { get; set; }
            public DateTime Launch { get; set; }
            [Numeric] public ulong OrbitsCompleted { get; set; }
        }

        // Test Scenario: [Numeric] Applied to DateTime Field (✗impermissible✗)
        public class Symphony {
            [PrimaryKey] public string Composer { get; set; } = "";
            [PrimaryKey] public uint OpusNumber { get; set; }
            public ushort Length { get; set; }
            public string Key { get; set; } = "";
            [Numeric] public DateTime PremiereDate { get; set; }
        }

        // Test Scenario: [Numeric] Applied to Guid Field (✗impermissible✗)
        public class WordSearch {
            [PrimaryKey, Numeric] public Guid PuzzleID { get; set; }
            public uint LettersWide { get; set; }
            public uint LettersTall { get; set; }
            public ulong NumWords { get; set; }
            public bool DiagonalsAllowed { get; set; }
            public bool BackwardsAllowed { get; set; }
            public string Title { get; set; } = "";
        }

        // Test Scenario: [Numeric] Applied to Aggregate Field (✗impermissible✗)
        public class GolfCourse {
            public record struct Hole(byte Par, ulong Distance, bool WaterHazards, bool SandTraps);

            [PrimaryKey] public string CourseName { get; set; } = "";
            public bool CountryClubCourse { get; set; }
            public Hole Hole1 { get; set; }
            public Hole Hole2 { get; set; }
            public Hole Hole3 { get; set; }
            public Hole Hole4 { get; set; }
            public Hole Hole5 { get; set; }
            public Hole Hole6 { get; set; }
            public Hole Hole7 { get; set; }
            public Hole Hole8 { get; set; }
            public Hole Hole9 { get; set; }
            public Hole Hole10 { get; set; }
            public Hole Hole11 { get; set; }
            public Hole Hole12 { get; set; }
            public Hole Hole13 { get; set; }
            [Numeric] public Hole Hole14 { get; set; }
            public Hole Hole15 { get; set; }
            public Hole Hole16 { get; set; }
            public Hole Hole17 { get; set; }
            public Hole Hole18 { get; set; }
        }

        // Test Scenario: [Numeric] Applied to Reference Field (✗impermissible✗)
        public class SlamBallMatch {
            public class SlamBallTeam {
                [PrimaryKey] public string Name { get; set; } = "";
                public string Coach { get; set; } = "";
                public string Handler { get; set; } = "";
                public string Gunner { get; set; } = "";
                public string Stopper { get; set; } = "";
            }

            [PrimaryKey] public Guid MatchID { get; set; }
            public SlamBallTeam Victor { get; set; } = new();
            [Numeric] public SlamBallTeam Defeated { get; set; } = new();
            public uint TotalPoints { get; set; }
            public uint TotalBlocks { get; set; }
            public uint TotalFouls { get; set; }
        }

        // Test Scenario: [Numeric] Applied to Relation Field (✗impermissible✗)
        public class Wadi {
            public record struct Coordinate(float Latitude, float Longitude);

            [PrimaryKey] public string Name { get; set; } = "";
            public ulong Length { get; set; }
            public ulong Elevation { get; set; }
            public Coordinate? Mouth { get; set; }
            [Numeric] public RelationSet<string> MineralDeposits { get; set; } = new();
            public sbyte NumOases { get; set; }
        }

        // Test Scenario: [AsString] Applied to Boolean Field (✗impermissible✗)
        public class BondGirl {
            [PrimaryKey] public string Name { get; set; } = "";
            public string Actress { get; set; } = "";
            public string Debut { get; set; } = "";
            public ushort Appearances { get; set; }
            [AsString] public bool SleptWithBond { get; set; }
        }

        // Test Scenario: [AsString] Applied to Textual Field (✗impermissible✗)
        public class BatmanVillain {
            public string Name { get; set; } = "";
            [PrimaryKey] public string AlterEgo { get; set; } = "";
            public bool IsAlive { get; set; }
            public ulong NumAppearances { get; set; }
            public DateTime Debut { get; set; }
            public bool InCahootsWithJoker { get; set; }
            [AsString] public char Grade { get; set; }
        }

        // Test Scenario: [AsString] Applied to Numeric Field (✗impermissible✗)
        public class Cemetery {
            [PrimaryKey] public double Longitude { get; set; }
            [PrimaryKey, AsString] public double Latitude { get; set; }
            public ulong Area { get; set; }
            public int Capacity { get; set; }
            public bool IsNational { get; set; }
            public uint NumMausoleums { get; set; }
        }

        // Test Scenario: [AsString] Applied to DateTime Field (✗impermissible✗)
        public class ImmaculateGrid {
            [PrimaryKey, AsString] public DateTime Date { get; set; }
            public string V1 { get; set; } = "";
            public string V2 { get; set; } = "";
            public string V3 { get; set; } = "";
            public string H1 { get; set; } = "";
            public string H2 { get; set; } = "";
            public string H3 { get; set; } = "";
            public double ImmaculatePercentage { get; set; }
        }

        // Test Scenario: [AsString] Applied to Guid Field (✗impermissible✗)
        public class Eyeglasses {
            [PrimaryKey, AsString] public Guid GlassesID { get; set; }
            public string Prescription { get; set; } = "";
            public bool IsMonocle { get; set; }
            public bool ForReadingOnly { get; set; }
            public float LensArea { get; set; }
        }

        // Test Scenario: [AsString] Applied to Aggregate Field (✗impermissible✗)
        public class Windmill {
            public record struct EnergyOutput(double Joules, double KilowattHours);

            [PrimaryKey] public float Latitude { get; set; }
            [PrimaryKey] public float Longitude { get; set; }
            public uint Height { get; set; }
            [AsString] public EnergyOutput EnergyGenerated { get; set; }
        }

        // Test Scenario: [AsString] Applied to Reference Field (✗impermissible✗)
        public class Chakra {
            public class Yogini {
                [PrimaryKey] public string Name { get; set; } = "";
                public DateTime DateOfBirth { get; set; }
                public DateTime DateOfDeath { get; set; }
            }

            [PrimaryKey] public string Name { get; set; } = "";
            [AsString] public Yogini? AssociatedYogini { get; set; }
            public string Location { get; set; } = "";
            public string Color { get; set; } = "";
            public long? NumPetals { get; set; }
        }

        // Test Scenario: [AsString] Applied to Relation Field (✗impermissible✗)
        public class Cryptogram {
            [PrimaryKey] public Guid PuzzleID { get; set; }
            public DateTime Published { get; set; }
            public string Author { get; set; } = "";
            public string EncryptedText { get; set; } = "";
            [AsString] public RelationMap<char, char> Solution { get; set; } = new();
        }

        // Test Scenario: Property Marked with [DataConverter] and [Numeric] (✗conflicting✗)
        public class SecretHitlerGame {
            public enum Role { Liberal, Fascist, Hitler }

            [PrimaryKey] public Guid GameID { get; set; }
            public Role Player1 { get; set; }
            public Role Player2 { get; set; }
            public Role Player3 { get; set; }
            public Role Player4 { get; set; }
            public Role Player5 { get; set; }
            public Role? Player6 { get; set; }
            [DataConverter(typeof(ToString<Role>)), Numeric] public Role? Player7 { get; set; }
            public byte LiberalPolicies { get; set; }
            public byte FascistPolicies { get; set; }
            public bool HitlerElectedChancellor { get; set; }
            public bool HitlerKilled { get; set; }
        }

        // Test Scenario: Property Marked with [DataConverter] and [AsString] (✗conflicting✗)
        public class Mezuzah {
            public enum Material { Gold, Silver, Steel, Plastic, Clay, Wood }

            [PrimaryKey] public Guid MezuzahID { get; set; }
            public double Weight { get; set; }
            public double Length { get; set; }
            public decimal? RetailPrice { get; set; }
            [DataConverter(typeof(ToInt<Material>)), AsString] public Material MadeOf { get; set; }
            public bool CurrentlyAffixed { get; set; }
        }

        // Test Scenario: Property Marked with [Numeric] and [AsString] (✗conflicting✗)
        public class Atoll {
            public enum Sea { Arctic, Indian, Pacific, Atlantic }

            [PrimaryKey] public string Name { get; set; } = "";
            [Numeric, AsString] public Sea Ocean { get; set; }
            public uint NumIslands { get; set; }
            public ulong Area { get; set; }
            public ulong Population { get; set; }
            public float Latitude { get; set; }
            public float Longitude { get; set; }
            public bool CoralReefs { get; set; }
        }
    }

    internal static class SignednessConstraints {
        public static class IsPositive {
            // Test Scenario: Applied to Non-Nullable Numeric Fields (✓constrained✓)
            public class GreekLetter {
                [PrimaryKey] public char Majuscule { get; set; }
                public char Miniscule { get; set; }
                public char? WordFinal { get; set; }
                public string Name { get; set; } = "";
                [Check.IsPositive] public int NumericValue { get; set; }
                [Check.IsPositive] public decimal Frequency { get; set; }
                [Check.IsPositive] public byte Index { get; set; }
                public string AncientIPA { get; set; } = "";
                public string ModernIPA { get; set; } = "";
            }

            // Test Scenario: Applied to Nullable Numeric Fields (✓constrained✓)
            public class MedicalSpecialty {
                [PrimaryKey] public string Specialty { get; set; } = "";
                [Check.IsPositive] public ulong? Practitioners { get; set; }
                [Check.IsPositive] public decimal? AverageSalary { get; set; }
                [Check.IsPositive] public sbyte YearsSchool { get; set; }
                public bool RequiresMD { get; set; }
            }

            // Test Scenario: Applied to Textual Field (✗impermissible✗)
            public class FieldGoal {
                [PrimaryKey] public DateTime When { get; set; }
                public bool Made { get; set; }
                public int Doinks { get; set; }
                [Check.IsPositive] public string Kicker { get; set; } = "";
            }

            // Test Scenario: Applied to Boolean Field (✗impermissible✗)
            public class GolfHole {
                [PrimaryKey] public int CourseID { get; set; }
                [PrimaryKey] public byte HoleNumber { get; set; }
                public byte Par { get; set; }
                public ushort DistanceToFlag { get; set; }
                public byte NumSandTraps { get; set; }
                [Check.IsPositive] public bool ContainsWaterHazard { get; set; }
            }

            // Test Scenario: Applied to DateTime Field (✗impermissible✗)
            public class Gymnast {
                [PrimaryKey] public string Name { get; set; } = "";
                public double Height { get; set; }
                [Check.IsPositive] public DateTime Birthdate { get; set; }
                public float AvgFloor { get; set; }
                public float AvgVault { get; set; }
                public float AvgBeam { get; set; }
                public float AvgRings { get; set; }
                public float AvgBars { get; set; }
            }

            // Test Scenario: Applied to Guid Field (✗impermissible✗)
            public class Documentary {
                [PrimaryKey, Check.IsPositive] public Guid ID { get; set; }
                public string Title { get; set; } = "";
                public string SubjectMatter { get; set; } = "";
                public bool OscarNominated { get; set; }
                public ushort Runtime { get; set; }
                public DateTime ReleaseDate { get; set; }
                public bool NeverBeforeSeenFootage { get; set; }
            }

            // Test Scenario: Applied to Enumeration Field (✗impermissible✗)
            public class Mythbusting {
                public enum Resolution { Busted, Plausible, Confirmed }

                [PrimaryKey] public uint Season { get; set; }
                [PrimaryKey] public uint Episode { get; set; }
                [PrimaryKey] public uint MythNumber { get; set; }
                public string MythDescription { get; set; } = "";
                [Check.IsPositive] public Resolution Rating { get; set; }
            }

            // Test Scenario: Applied to Aggregate-Nested Numeric Scalar (✓constrained✓)
            public class IceAge {
                public struct Timespan {
                    public short Length { get; set; }
                    [Check.IsPositive] public short SubLength { get; set; }
                    string Unit { get; set; }
                }

                [PrimaryKey] public string Name { get; set; } = "";
                [Check.IsPositive(Path = "Length")] public Timespan Ago { get; set; }
                public double AverageTemperature { get; set; }
                public float ChangeInAxialTilt { get; set; }
                public bool Global { get; set; }
            }

            // Test Scenario: Applied to Aggregate-Nested Non-Numeric Scalar (✗impermissible✗)
            public class GoldenRaspberry {
                public record struct Nominee(string Name, string Movie);

                [PrimaryKey] public ushort Year { get; set; }
                public string Category { get; set; } = "";
                public Nominee Winner { get; set; }
                [Check.IsPositive(Path = "Movie")] public Nominee? RunnerUp { get; set; }
                public bool AlsoOscarNominated { get; set; }
                public double VoteShare { get; set; }
            }

            // Test Scenario: Applied to Nested Aggregate (✗impermissible✗)
            public class SudokuPuzzle {
                public record struct Row(sbyte Left, sbyte Center, sbyte Right);
                public record struct Square(Row Top, Row Center, Row Bottom);

                [PrimaryKey] public Guid PuzzleID { get; set; }
                public Square UpperLeft { get; set; }
                public Square UpperCenter { get; set; }
                public Square UpperRight { get; set; }
                public Square MiddleLeft { get; set; }
                public Square MiddleCenter { get; set; }
                public Square MiddleRight { get; set; }
                [Check.IsPositive(Path = "Bottom")] public Square LowerLeft { get; set; }
                public Square LowerCenter { get; set; }
                public Square LowerRight { get; set; }
            }

            // Test Scenario: Applied to Reference-Nested Numeric Scalar (✓constrained✓)
            public class Runway {
                public class Airport {
                    [PrimaryKey] public uint ID { get; set; }
                    public string Name { get; set; } = "";
                    public string Servicing { get; set; } = "";
                    public DateTime Constructed { get; set; }
                    public ulong Employees { get; set; }
                    public bool IsHub { get; set; }
                }

                [PrimaryKey] public Guid RunwayID { get; set; }
                [Check.IsPositive(Path = "ID")] public Airport Host { get; set; } = new();
                public byte Number { get; set; }
                public double Heading { get; set; }
                public ulong Length { get; set; }
            }

            // Test Scenario: Applied to Reference-Nested Non-Numeric Scalar (✗impermissible✗)
            public class CaesareanSection {
                public class Person {
                    [PrimaryKey] public string FirstName { get; set; } = "";
                    [PrimaryKey] public string LastName { get; set; } = "";
                    public ulong SSN { get; set; }
                }

                [PrimaryKey] public DateTime Timestamp { get; set; }
                public Person Mother { get; set; } = new();
                [Check.IsPositive(Path = "LastName")] public Person Doctor { get; set; } = new();
                public byte PregnancyNumber { get; set; }
                public double BloodLoss { get; set; }
                public double ScarLength { get; set; }
            }

            // Test Scenario: Applied to Nested Reference (✗impermissible✗)
            public class Lamp {
                public class Unit {
                    public string FullName { get; set; } = "";
                    [PrimaryKey] public string Symbol { get; set; } = "";
                    public string Formula { get; set; } = "";
                    public bool IsSI { get; set; }
                }
                public record struct Output(ushort Amount, Unit Unit);

                [PrimaryKey] public Guid ProductID { get; set; }
                public double Height { get; set; }
                public sbyte NumBulbs { get; set; }
                public bool IsLED { get; set; }
                [Check.IsPositive(Path = "Unit")] public Output Power { get; set; }
            }

            // Test Scenario: Original Constraint on Reference-Nested Field (✓not propagated✓)
            public class Lycanthrope {
                public enum Kind { OnDemand, Emotional, Lunar }

                public class Shapeshifting {
                    [PrimaryKey, Check.IsPositive] public int ID { get; set; }
                    public string Shape { get; set; } = "";
                    public Kind Variety { get; set; }
                    public double Strength { get; set; }
                }

                [PrimaryKey] public int ID { get; set; }
                public string Name { get; set; } = "";
                public Shapeshifting Powers { get; set; } = new();
            }

            // Test Scenario: Applied to Relation-Nested Numeric Scalar (✓constrained✓)
            public class ArtificialIntelligence {
                public enum Type { General, Generative, Recommendatory, Classification, Search, Other }
                public record struct Support(double LatestVersions, RelationSet<double> SupportedVersions);

                [PrimaryKey] public string Name { get; set; } = "";
                public Type Category { get; set; }
                public DateTime Debut { get; set; }
                [Check.IsPositive(Path = "SupportedVersions.Item")] public Support Versions { get; set; }
                public RelationSet<string> Creators { get; set; } = new();
                public bool CanPassTuringTest { get; set; }
            }

            // Test Scenario: Applied to Relation-Nested Non-Numeric Scalar (✗impermissible✗)
            public class Margarita {
                [Flags] public enum Rimming { Salt = 1, Sugar = 2, Tajin = 4 }

                public class Ingredient {
                    [PrimaryKey] public Guid ID { get; set; }
                    public short Name { get; set; }
                    public bool IsFruit { get; set; }
                    public ushort Calories { get; set; }
                }

                [PrimaryKey] public Guid MargaritaID { get; set; }
                public string Tequila { get; set; } = "";
                public Rimming Rim { get; set; }
                [Check.IsPositive(Path = "Item.ID")] public RelationSet<Ingredient> Ingredients { get; set; } = new();
                public double AlcoholVolume { get; set; }
                public decimal Price { get; set; }
            }

            // Test Scenario: Applied to Nested Relation (✗impermissible✗)
            public class PulitzerPrize {
                public enum Categorization { Fiction, Music, Drama, Biography, Poetry, History, Reporting, Photography, Illustration }
                public record struct Committee(RelationList<string> Members, string Chairperson);

                [PrimaryKey] public ushort Year { get; set; }
                [PrimaryKey] public Categorization Category { get; set; }
                public string Recipient { get; set; } = "";
                public string For { get; set; } = "";
                [Check.IsPositive(Path = "Members")] public Committee AwardCommittee { get; set; }
                public decimal PrizeMoney { get; set; }
            }

            // Test Scenario: Applied to Field Data-Converted to Numeric Type (✓constrained✓)
            public class SwimmingPool {
                [PrimaryKey] public uint Depth { get; set; }
                [PrimaryKey] public uint Length { get; set; }
                [PrimaryKey, DataConverter(typeof(ToInt<char>)), Check.IsPositive] public char Classification { get; set; }
                public bool HasDivingBoard { get; set; }
            }

            // Test Scenario: Applied to Field Data-Converted from Numeric Type (✗impermissible✗)
            public class WikipediaPage {
                [PrimaryKey] public string URL { get; set; } = "";
                public DateTime LastEdited { get; set; }
                public ulong WordCount { get; set; }
                [DataConverter(typeof(ToString<ushort>)), Check.IsPositive] public ushort Languages { get; set; }
                public ulong OutboundLinks { get; set; }
                public ulong InboundLinks { get; set; }
                public short Citations { get; set; }
            }

            // Test Scenario: Scalar Property Constrained Multiple Times (✓redundant✓)
            public class BaseballCard {
                [PrimaryKey] public string Company { get; set; } = "";
                [PrimaryKey] public string Player { get; set; } = "";
                [PrimaryKey, Check.IsPositive, Check.IsPositive] public int CardNumber { get; set; }
                public bool IsMintCondition { get; set; }
                public ushort NumPrinted { get; set; }
                public decimal Value { get; set; }
            }

            // Test Scenario: <Path> is `null` (✗illegal✗)
            public class HotSpring {
                [PrimaryKey] public string Name { get; set; } = "";
                public ushort DischargePerMinute { get; set; }
                [Check.IsPositive(Path = null!)] public short Elevation { get; set; }
                public double Temperature { get; set; }
            }

            // Test Scenario: <Path> on Scalar Does Not Exist (✗non-existent path✗)
            public class Canal {
                [PrimaryKey] public int CanalID { get; set; }
                public string Name { get; set; } = "";
                [Check.IsPositive(Path = "---")] public double Length { get; set; }
                public double MaxBoatLength { get; set; }
                public double MaxBoatBeam { get; set; }
                public double MaxBoatDraft { get; set; }
                public DateTime Opening { get; set; }
            }

            // Test Scenario: <Path> on Aggregate Does Not Exist (✗non-existent path✗)
            public class SharkWeek {
                public record struct ProductionInfo(int Season, int StartEpisode, int EndEpisode);

                [PrimaryKey] public Guid SweepstakesID { get; set; }
                [Check.IsPositive(Path = "---")] public ProductionInfo Info { get; set; }
                public string Host { get; set; } = "";
                public ushort NumSharkAppearances { get; set; }
                public double NielsenRating { get; set; }
            }

            // Test Scenario: <Path> on Aggregate Not Specified (✗missing path✗)
            public class Philosopher {
                public enum Philosophy { Metaphysics, Logic, Ethics, Aesthetics, Epistemology, Theology, Realism, Cosmology }
                public record struct Naming(string FirstName, string LastName);

                [PrimaryKey] public Guid PhilosopherID { get; set; }
                public Philosophy School { get; set; }
                [Check.IsPositive] public Naming Name { get; set; }
                public DateTime DateOfBirth { get; set; }
                public DateTime? DateOfDeath { get; set; }
                public ushort Publications { get; set; }
            }

            // Test Scenario: <Path> on Reference Does Not Exist (✗non-existent path✗)
            public class HappyHour {
                public class Bar {
                    [PrimaryKey] public string Name { get; set; } = "";
                    public string Address { get; set; } = "";
                    public string Proprietor { get; set; } = "";
                }

                [PrimaryKey] public Guid ID { get; set; }
                public DateTime Start { get; set; }
                public DateTime End { get; set; }
                [Check.IsPositive(Path = "---")] public Bar Location { get; set; } = new();
                public decimal DrinkPrice { get; set; }
            }

            // Test Scenario: <Path> on Reference Refers to Non-Primary-Key Field (✗non-existent path✗)
            public class Aquifer {
                public enum Continent { NorthAmerica, SouthAmerica, Europe, Asia, Africa, Oceania, Antarctica }

                public class Person {
                    [PrimaryKey] public string Name { get; set; } = "";
                    public DateTime Birthdate { get; set; }
                    public string? Qualifications { get; set; } = "";
                }

                [PrimaryKey] public string Name { get; set; } = "";
                public Continent WhichContinent { get; set; }
                public ulong Area { get; set; }
                public ulong WaterOutput { get; set; }
                [Check.IsPositive(Path = "Qualifications")] public Person DiscoveringGeologist { get; set; } = new();
                public ulong PeopleServiced { get; set; }
            }

            // Test Scenario: <Path> on Reference Not Specified (✗missing path✗)
            public class FactCheck {
                public class Statement {
                    [PrimaryKey] public Guid ID { get; set; }
                    public string Claim { get; set; } = "";
                    public string Claimant { get; set; } = "";
                }

                [PrimaryKey] public Guid ID { get; set; }
                [Check.IsPositive] public Statement Fact { get; set; } = new();
                public string Checker { get; set; } = "";
                public bool JudgedTrue { get; set; }
                public uint? Pinocchios { get; set; }
            }

            // Test Scenario: <Path> on Relation Does Not Exist (✗non-existent path✗)
            public class Sukkah {
                [PrimaryKey] public Guid SukkahID { get; set; }
                public float Height { get; set; }
                public float Width { get; set; }
                public float Length { get; set; }
                [Check.IsPositive(Path = "---")] public RelationMap<string, short> Builders { get; set; } = new();
                public bool IsPermanent { get; set; }
                public int AmountSkakh { get; set; }
                public DateTime Constructed { get; set; }
            }

            // Test Scenario: <Path> on Relation Refers to Non-Primary-Key Field of Anchor Entity (✗non-existent path✗)
            public class Screenwriter {
                public class Script {
                    [PrimaryKey] public ulong ScriptNumber { get; set; }
                    [PrimaryKey] public sbyte Draft { get; set; }
                    public string Title { get; set; } = "";
                    public int WordCount { get; set; }
                    public ushort PageCount { get; set; }
                    public bool OnSpec { get; set; }
                }

                [PrimaryKey] public string FirstName { get; set; } = "";
                [PrimaryKey] public string LastName { get; set; } = "";
                public DateTime Birthdate { get; set; }
                [Check.IsPositive(Path = "Item.WordCount")] public RelationList<Script> Scripts { get; set; } = new();
                public decimal Salary { get; set; }
                public bool WGAMember { get; set; }
            }

            // Test Scenario: <Path> on Relation Not Specified (✗missing path✗)
            public class Typewriter {
                [PrimaryKey] public Guid TypewriterID { get; set; }
                [Check.IsPositive] public RelationSet<char> MissingKeys { get; set; } = new();
                public bool OwnedByTomHanks { get; set; }
                public DateTime Created { get; set; }
                public uint ClickSoundNumber { get; set; }
                public ushort WordsPerMinute { get; set; }
            }

            // Test Scenario: Default Value Does Not Satisfy Constraint (✗contradiction✗)
            public class ScoobyDooFilm {
                [PrimaryKey] public string MovieTitle { get; set; } = "";
                [Check.IsPositive, Default(-89L)] public long Runtime { get; set; }
                public DateTime ReleaseDate { get; set; }
                public bool DirectToDVD { get; set; }
                public string VoiceOfFred { get; set; } = "";
                public string VoiceOfDaphne { get; set; } = "";
                public string VoiceOfVelma { get; set; } = "";
                public string VoiceOfShaggy { get; set; } = "";
                public string VoiceOfScooby { get; set; } = "";
            }

            // Test Scenario: Originally Valid Default Value No Longer Satisfies Constraint (✗contradiction✗)
            public class Cyclops {
                public struct Mention {
                    public string Title { get; set; }
                    [Default(-9)] public int Book { get; set; }
                    public int Chapter { get; set; }
                    public int Verse { get; set; }
                }

                [PrimaryKey] public string Name { get; set; } = "";
                public byte NumEyes { get; set; }
                [Check.IsPositive(Path = "Book")] public Mention FirstMentioned { get; set; }
                public double Height { get; set; }
                public bool FatheredByPoseidon { get; set; }
            }
        }

        public static class IsNegative {
            // Test Scenario: Applied to Signed Numeric Fields (✓constrained✓)
            public class Acid {
                [PrimaryKey] public string Name { get; set; } = "";
                public string Formula { get; set; } = "";
                [Check.IsNegative] public float pH { get; set; }
                [Check.IsNegative] public short FreezingPoint { get; set; }
                public int HazardNumber { get; set; }
            }

            // Test Scenario: Applied to Unsigned Numeric Field (✗unsatisfiable✗)
            public class Cereal {
                [PrimaryKey] public string Brand { get; set; } = "";
                [PrimaryKey] public string Variety { get; set; } = "";
                [Check.IsNegative] public ushort CaloriesPerServing { get; set; }
                public double PotassiumPerServing { get; set; }
                public double SodiumPerServing { get; set; }
                public double CarbsPerServing { get; set; }
                public double ProteinPerServing { get; set; }
            }

            // Test Scenario: Applied to Nullable Signed Numeric Fields (✓constrained✓)
            public class ConcentrationCamp {
                [PrimaryKey] public string Name { get; set; } = "";
                [Check.IsNegative] public double? Inmates { get; set; }
                [Check.IsNegative] public long? Casualties { get; set; }
                [Check.IsNegative] public short? DaysOperational { get; set; }
                public string Commandant { get; set; } = "";
                public bool WasDeathCamp { get; set; }
            }

            // Test Scenario: Applied to Textual Field (✗impermissible✗)
            public class KeySignature {
                [PrimaryKey, Check.IsNegative] public char Note { get; set; }
                [PrimaryKey] public bool Sharp { get; set; }
                [PrimaryKey] public bool Flat { get; set; }
                [PrimaryKey] public bool Natural { get; set; }
            }

            // Test Scenario: Applied to Boolean Field (✗impermissible✗)
            public class SporcleQuiz {
                [PrimaryKey] public string Author { get; set; } = "";
                [PrimaryKey] public string Slug { get; set; } = "";
                public ushort Questions { get; set; }
                public ulong Plays { get; set; }
                [Check.IsNegative] public bool Published { get; set; }
                public bool EditorsPick { get; set; }
                public bool CuratorsPick { get; set; }
                public string Category { get; set; } = "";
            }

            // Test Scenario: Applied to DateTime Field (✗impermissible✗)
            public class Olympiad {
                [PrimaryKey] public ushort Year { get; set; }
                public bool Summer { get; set; }
                [Check.IsNegative] public DateTime OpeningCeremony { get; set; }
                public DateTime ClosingCeremony { get; set; }
                public ushort Events { get; set; }
                public ulong TotalAttendance { get; set; }
                public string TopGoldMedalCountry { get; set; } = "";
            }

            // Test Scenario: Applied to Guid Field (✗impermissible✗)
            public class W2 {
                [PrimaryKey, Check.IsNegative] public Guid FormID { get; set; }
                public ulong SSN { get; set; }
                public string EmployeeEIN { get; set; } = "";
                public decimal Wages { get; set; }
                public decimal FederalWitholdings { get; set; }
                public decimal StateWitholdings { get; set; }
                public decimal SocialSecurityWitholdings { get; set; }
                public decimal MedicareWitholdings { get; set; }
                public decimal Tips { get; set; }
                public decimal DependentBenefits { get; set; }
            }

            // Test Scenario: Applied to Enumeration Field (✗impermissible✗)
            public class SerialKiller {
                public enum Status { AtLarge, Incarcerated, Apprehended, InTrial }

                [PrimaryKey] public string AlterEgo { get; set; } = "";
                public string? Identity { get; set; }
                [Check.IsNegative] public Status CurrentStatus { get; set; }
                public uint KnownVictims { get; set; }
                public bool FBIMostWanted { get; set; }
            }

            // Test Scenario: Applied to Aggregate-Nested Signed Numeric Scalar (✓constrained✓)
            public class Flood {
                public record struct Date(sbyte Day, sbyte Month, ushort Year);

                [PrimaryKey] public Guid FloodID { get; set; }
                [Check.IsNegative(Path = "Month")] public Date Occurrence { get; set; }
                public ushort Casualties { get; set; }
                public ulong WaterVolume { get; set; }
                public string FloodingSource { get; set; } = "";
                public decimal Damage { get; set; }
            }

            // Test Scenario: Applied to Aggregate-Nested Non-Signed-Numeric Scalar (✗impermissible✗)
            public class TrolleyProblem {
                public record struct Option(string Label, double PcntChoice, int PotentialVictims);

                [PrimaryKey] public Guid ID { get; set; }
                public Option NoPull { get; set; }
                [Check.IsNegative(Path = "Label")] public Option Pull { get; set; }
            }

            // Test Scenario: Applied to Nested Aggregate (✗impermissible✗)
            public class Pharaoh {
                public record struct EgyptianKingdom(string Name, bool GoldenAge);
                public record struct EgyptianDynasty(byte Index, EgyptianKingdom Kingdom);

                [PrimaryKey] public string RegnalName { get; set; } = "";
                [PrimaryKey] public uint? RegnalNumber { get; set; }
                public DateTime ReignBegin { get; set; }
                public DateTime ReignEnd { get; set; }
                [Check.IsNegative(Path = "Kingdom")] public EgyptianDynasty Dynasty { get; set; }
                public string GreatRoyalWife { get; set; } = "";
                public ushort TotalWives { get; set; }
                public string? BurialPyramid { get; set; }
            }

            // Test Scenario: Applied to Reference-Nested Signed Numeric Scalar (✓constrained✓)
            public class HawaiianGod {
                public class MaoriGod {
                    [PrimaryKey] public short DeityID { get; set; }
                    public string Name { get; set; } = "";
                    public int NumChildren { get; set; }
                }

                [PrimaryKey] public Guid ID { get; set; }
                public string Name { get; set; } = "";
                public string Domain { get; set; } = "";
                public string Form { get; set; } = "";
                [Check.IsNegative(Path = "DeityID")] public MaoriGod? MaoriEquivalent { get; set; }
                public bool IsAumakua { get; set; }
            }

            // Test Scenario: Applied to Reference-Nested Non-Signed-Numeric Scalar (✗impermissible✗)
            public class OceanCurrent {
                public class Ocean {
                    [PrimaryKey] public string Name { get; set; } = "";
                    public ulong Area { get; set; }
                    public ulong TotalVolume { get; set; }
                    public ulong DeepestDepth { get; set; }
                }

                [PrimaryKey] public Guid ID { get; set; }
                public string CommonIdentifier { get; set; } = "";
                [Check.IsNegative(Path = "Name")] public Ocean WhichOcean { get; set; } = new();
                public bool IsTradeWind { get; set; }
            }

            // Test Scenario: Applied to Nested Reference (✗impermissible✗)
            public class AirBNB {
                public class Politician {
                    [PrimaryKey] public Guid ID { get; set; }
                    public string FirstName { get; set; } = "";
                    public string LastName { get; set; } = "";
                    public bool Democrat { get; set; }
                }
                public class State {
                    [PrimaryKey] public string Abbreviation { get; set; } = "";
                    public string FullName { get; set; } = "";
                    public DateTime AchievedStatehood { get; set; }
                    public ulong Population { get; set; }
                    public ulong Area { get; set; }
                    public Politician Governor { get; set; } = new();
                }
                public record struct Address(uint Number, string Street, string City, State State, ulong ZipCode);

                [PrimaryKey] public Guid ListingID { get; set; }
                [Check.IsNegative(Path = "State")] public Address HouseAddress { get; set; }
                public decimal PerNight { get; set; }
                public Guid OwnerID { get; set; }
            }

            // Test Scenario: Original Constraint on Reference-Nested Field (✓not propagated✓)
            public class DragonRider {
                public class Dragon {
                    [PrimaryKey, Check.IsNegative] public short ID { get; set; }
                    public string Name { get; set; } = "";
                    public string Color { get; set; } = "";
                    public double EldunariWeight { get; set; }
                }

                [PrimaryKey] public string Name { get; set; } = "";
                public uint Age { get; set; }
                public string FirstChapterAppearance { get; set; } = "";
                public string LastChapterAppearance { get; set; } = "";
                public bool IsDead { get; set; }
                public Dragon DragonRidden { get; set; } = new();
            }

            // Test Scenario: Applied to Relation-Nested Numeric Scalar (✓constrained✓)
            public class Almanac {
                public enum Info { Astronomy, Agriculture, Geography, Hydrology, Meteorology, Calendrical, Folkloric }
                public record struct Section(Info Category, int PageStart, int PageEnd);

                [PrimaryKey] public ulong ISBN { get; set; }
                public string Title { get; set; } = "";
                public string Author { get; set; } = "";
                public DateTime Copyright { get; set; }
                [Check.IsNegative(Path = "Item.PageEnd")] public RelationList<Section> Sections { get; set; } = new();
                public ushort NumImages { get; set; }
            }

            // Test Scenario: Applied to Relation-Nested Non-Numeric Scalar (✗impermissible✗)
            public class LandMine {
                public enum Kind { AntiTank, AntiPersonnel }

                [PrimaryKey] public Guid ID { get; set; }
                public float Latitude { get; set; }
                public float Longitude { get; set; }
                public bool Detonated { get; set; }
                [Check.IsNegative(Path = "Item")] public IReadOnlyRelationSet<string> Casualties { get; set; } = new RelationSet<string>();
                public Kind Type { get; set; }
            }

            // Test Scenario: Applied to Nested Relation (✗impermissible✗)
            public class YahtzeeGame {
                public enum Category { Ones, Twos, Threes, Fours, Five, Sixes, ThreeOAK, FourOAK, FullHouse, SmallSTR, LargeSTR, Yahtzee, Chance };
                public record struct Player(Guid PlayerID, string Name, RelationMap<Category, byte> Score);

                [PrimaryKey] public Guid ID { get; set; }
                public string DiceColor { get; set; } = "";
                [Check.IsNegative(Path = "Score")] public Player Player1 { get; set; }
                public Player Player2 { get; set; }
            }

            // Test Scenario: Applied to Field Data-Converted to Numeric Type (✓constrained✓)
            public class Boxer {
                [PrimaryKey] public string FirstName { get; set; } = "";
                [PrimaryKey] public string LastName { get; set; } = "";
                public double Height { get; set; }
                public ushort Weight { get; set; }
                public int Wins { get; set; }
                public int Losses { get; set; }
                [DataConverter(typeof(ToInt<string>)), Check.IsNegative] public string TKOs { get; set; } = "";
            }

            // Test Scenario: Applied to Field Data-Converted from Numeric Type (✗impermissible✗)
            public class Archangel {
                [PrimaryKey] public string Name { get; set; } = "";
                public ulong OldTestamentMentions { get; set; }
                public ulong NewTestamentMentions { get; set; }
                public ulong ApocryphaMentions { get; set; }
                [DataConverter(typeof(ToString<float>)), Check.IsNegative] public float FirstAppearance { get; set; }
                public DateTime FeastDay { get; set; }
            }

            // Test Scenario: Scalar Property Constrained Multiple Times (✓redundant✓)
            public class Alkene {
                [PrimaryKey] public string Formula { get; set; } = "";
                public bool IsCyclic { get; set; }
                [Check.IsNegative, Check.IsNegative] public double FreezingPoint { get; set; }
                public double Density { get; set; }
            }

            // Test Scenario: <Path> is `null` (✗illegal✗)
            public class Climate {
                [PrimaryKey] public string KoppenClassification { get; set; } = "";
                public string? LongForm { get; set; }
                public float AverageHitTemperature { get; set; }
                [Check.IsNegative(Path = null!)] public float AverageLowTemperature { get; set; }
                public float AverageRainfall { get; set; }
                public double MaxLongitude { get; set; }
                public double MinLongitude { get; set; }
            }

            // Test Scenario: <Path> on Scalar Does Not Exist (✗non-existent path✗)
            public class CircleOfHell {
                [PrimaryKey, Check.IsNegative(Path = "---")] public byte Level { get; set; }
                public string Title { get; set; } = "";
                public string PrimeResident { get; set; } = "";
                public ulong CantoOfIntroduction { get; set; }
            }

            // Test Scenario: <Path> on Aggregate Does Not Exist (✗non-existent path✗)
            public class VolleyballMatch {
                public enum Style { Beach, Indoor }
                public record struct Set(ushort HomeTeamScore, ushort AwayTeamScore);

                [PrimaryKey] public string HomeTeam { get; set; } = "";
                [PrimaryKey] public string AwayTeam { get; set; } = "";
                [PrimaryKey] public DateTime Scheduled { get; set; }
                public Style VolleyballType { get; set; }
                public Set FirstSet { get; set; }
                public Set SecondSet { get; set; }
                public Set? ThirdSet { get; set; }
                [Check.IsNegative(Path = "---")] public Set? FourthSet { get; set; }
                public Set? FifthSet { get; set; }
                public ushort TotalKills { get; set; }
                public ushort TotalDigs { get; set; }
                public ushort TotalBlocks { get; set; }
                public ushort TotalAces { get; set; }
                public bool Olympics { get; set; }
            }

            // Test Scenario: <Path> on Aggregate Not Specified (✗missing path✗)
            public class Yacht {
                public enum Kind { Sailing, Super, ClassicalDutch, Competitive, Military }
                public record struct ShipSails(ushort Count, string Material, string Shape);

                [PrimaryKey] public Guid YachtID { get; set; }
                public Kind YachtType { get; set; }
                public string BoatName { get; set; } = "";
                public ushort PassengerLimit { get; set; }
                public decimal Cost { get; set; }
                [Check.IsNegative] public ShipSails Sails { get; set; }
            }

            // Test Scenario: <Path> on Reference Does Not Exist (✗non-existent path✗)
            public class Pharmacy {
                public class Person {
                    [PrimaryKey] public string FirstName { get; set; } = "";
                    [PrimaryKey] public string LastName { get; set; } = "";
                }

                [PrimaryKey] public Guid PharmacyID { get; set; }
                [Check.IsNegative(Path = "---")] public Person HeadPharmacist { get; set; } = new();
                public ulong PrescriptionsFilled { get; set; }
                public bool IsCompounding { get; set; }
                public bool IsWalgreens { get; set; }
            }

            // Test Scenario: <Path> on Reference Refers to Non-Primary-Key Field (✗non-existent path✗)
            public class Popcorn {
                public class Condiment {
                    [PrimaryKey] public Guid ID { get; set; }
                    public string Name { get; set; } = "";
                    public uint Calories { get; set; }
                    public uint Sodium { get; set; }
                }

                [PrimaryKey] public Guid ID { get; set; }
                public string Brand { get; set; } = "";
                public double Volume { get; set; }
                [Check.IsNegative(Path = "Calories")] public Condiment? Topping { get; set; }
                public bool Microwaved { get; set; }
            }

            // Test Scenario: <Path> on Reference Not Specified (✗missing path✗)
            public class WinForm {
                public class Button {
                    [PrimaryKey] public Guid ComponentID { get; set; }
                    public ushort WidthPixels { get; set; }
                    public ushort HeightPixels { get; set; }
                    public string Label { get; set; } = "";
                    public ulong XPos { get; set; }
                    public ulong YPos { get; set; }
                }

                [PrimaryKey] public Guid FormID { get; set; }
                public string Title { get; set; } = "";
                [Check.IsNegative] public Button? SubmitButton { get; set; }
                public string DominantFont { get; set; } = "";
                public ushort NumComponents { get; set; }
                public ulong Height { get; set; }
                public ulong Width { get; set; }
            }

            // Test Scenario: <Path> on Relation Does Not Exist (✗non-existent path✗)
            public class RomanFestival {
                [PrimaryKey] public string Name { get; set; } = "";
                public string? DeityHonored { get; set; }
                public bool Drunken { get; set; }
                [Check.IsNegative(Path = "---")] public RelationSet<DateTime> PossibleDates { get; set; } = new();
                public string? GreekEquivalent { get; set; }
            }

            // Test Scenario: <Path> on Relation Refers to Non-Primary-Key Field of Anchor Entity (✗non-existent path✗)
            public class AmberAlert {
                public enum Category { Make, Model, LicensePlate, Color, NumDoors, Wheels, Other }

                [PrimaryKey] public Guid AlertID { get; set; }
                public string ChildsName { get; set; } = "";
                public DateTime Issued { get; set; }
                [Check.IsNegative(Path = "AmberAlert.EmergencyContactNumber")] public RelationMap<Category, string> VehicleDescription { get; set; } = new();
                public ushort EmergencyContactNumber { get; set; }
            }

            // Test Scenario: <Path> on Relation Not Specified (✗missing path✗)
            public class BoyBand {
                public class Singer {
                    public enum Range { Soprano, MezzoSoprano, Contralto, Countertenor, Tenor, Baritone, Bass }

                    [PrimaryKey] public string FirstName { get; set; } = "";
                    [PrimaryKey] public string LastName { get; set; } = "";
                    public Range VocalRange { get; set; }
                }

                [PrimaryKey] public string BandName { get; set; } = "";
                public DateTime Debuted { get; set; }
                public DateTime? Disbanded { get; set; }
                public DateTime? Renunited { get; set; }
                public ulong RecordsSold { get; set; }
                [Check.IsNegative] public RelationList<Singer> Members { get; set; } = new();
                public ushort NumAlbums { get; set; }
                public ulong InstagramFollowers { get; set; }
            }

            // Test Scenario: Default Value Does Not Satisfy Constraint (✗contradiction✗)
            public class SuperPAC {
                [PrimaryKey] public Guid RegistrationID { get; set; }
                [Check.IsNegative, Default(0L)] public long TotalRaised { get; set; }
                public ushort NumContributors { get; set; }
                public DateTime Established { get; set; }
                public bool IsProDemocrat { get; set; }
            }

            // Test Scenario: Originally Valid Default Value No Longer Satisfies Constraint (✗contradiction✗)
            public class PressSecretary {
                public struct Date {
                    public byte Month { get; set; }
                    public byte Day { get; set; }
                    [Default((short)1563)] public short Year { get; set; }
                }

                [PrimaryKey] public string Name { get; set; } = "";
                public string Boss { get; set; } = "";
                public ulong TotalQuestionsAnswered { get; set; }
                public Date StartDate { get; set; }
                [Check.IsNegative(Path = "Year")] public Date EndDate { get; set; }
                public string FavoredNetwork { get; set; } = "";
            }
        }

        public static class IsNonZero {
            // Test Scenario: Applied to Numeric Fields (✓constrained✓)
            public class RegularPolygon {
                [PrimaryKey, Check.IsNonZero] public ushort NumEdges { get; set; }
                [Check.IsNonZero] public sbyte NumVertices { get; set; }
                [Check.IsNonZero] public double InternalAngle { get; set; }
                [Check.IsNonZero] public decimal ExternalAngle { get; set; }
                public bool IsConvex { get; set; }
            }

            // Test Scenario: Applied to Nullable Numeric Fields (✓constrained✓)
            public class Skittles {
                [PrimaryKey] public string Variety { get; set; } = "";
                public DateTime Introduced { get; set; }
                public bool Discontinued { get; set; }
                [Check.IsNonZero] public double? Weight { get; set; }
                [Check.IsNonZero] public short? ServingSizeCalories { get; set; }
                [Check.IsNonZero] public uint? PiecesPerBag { get; set; }
            }

            // Test Scenario: Applied to Textual Field (✗impermissible✗)
            public class Brassiere {
                [PrimaryKey] public Guid ProductID { get; set; }
                public ushort Band { get; set; }
                [Check.IsNonZero] public string CupSize { get; set; } = "";
            }

            // Test Scenario: Applied to Boolean Field (✗impermissible✗)
            public class FutharkRune {
                [PrimaryKey] public char Rune { get; set; }
                public char EnglishApproximation { get; set; }
                public bool InElderFuthark { get; set; }
                [Check.IsNonZero] public bool InYoungerFuthark { get; set; }
            }

            // Test Scenario: Applied to DateTime Field (✗impermissible✗)
            public class FinalFour {
                [PrimaryKey] public ushort Year { get; set; }
                [PrimaryKey] public bool Womens { get; set; }
                [Check.IsNonZero] public DateTime ChampionshipGame { get; set; }
                public string EastRep { get; set; } = "";
                public string WetRep { get; set; } = "";
                public string CentralRep { get; set; } = "";
                public string SouthRep { get; set; } = "";
            }

            // Test Scenario: Applied to Guid Field (✗impermissible✗)
            public class Fractal {
                [PrimaryKey, Check.IsNonZero] public Guid FractalID { get; set; }
                public string? CommonName { get; set; }
                public string Formula { get; set; } = "";
                public float TopologicalDimension { get; set; }
                public float HausdorffDimension { get; set; }
                public bool CanTesselate { get; set; }
            }

            // Test Scenario: Applied to Enumeration Field (✗impermissible✗)
            public class IPO {
                public enum Method { BestEfforts, FirmCommittment, AllOrNone, BoughtDeal }

                [PrimaryKey] public string Company { get; set; } = "";
                public string Symbol { get; set; } = "";
                public string Exchange { get; set; } = "";
                [Check.IsNonZero] public Method PostingMethod { get; set; }
                public ulong Shares { get; set; }
                public decimal OpeningPrice { get; set; }
                public decimal ClosingPrice { get; set; }
            }

            // Test Scenario: Applied to Aggregate-Nested Numeric Scalar (✓constrained✓)
            public class Essay {
                public record struct Sentence(string Text, int WordCount);
                public record struct Paragraph(Sentence S1, Sentence S2, Sentence S3, Sentence S4, Sentence S5);

                [PrimaryKey] public string Title { get; set; } = "";
                [PrimaryKey] public string Author { get; set; } = "";
                public string ThesisStatement { get; set; } = "";
                [Check.IsNonZero(Path = "S5.WordCount"), Check.IsNonZero(Path = "S3.WordCount")] public Paragraph P1 { get; set; }
                public Paragraph P2 { get; set; }
                [Check.IsNonZero(Path = "S2.WordCount")] public Paragraph P3 { get; set; }
                public Paragraph P4 { get; set; }
                [Check.IsNonZero(Path = "S1.WordCount"), Check.IsNonZero(Path = "S4.WordCount")] public Paragraph P5 { get; set; }
            }

            // Test Scenario: Applied to Aggregate-Nested Non-Numeric Scalar (✗impermissible✗)
            public class IDE {
                public record struct SemVer(uint Major, uint Minor, uint Patch, string? PreRelease, string? Metadata, DateTime Released);

                [PrimaryKey] public string Name { get; set; } = "";
                [Check.IsNonZero(Path = "Released")] public SemVer Version { get; set; }
                public string URL { get; set; } = "";
                public ulong Downloads { get; set; }
                public bool SupportsCPP { get; set; }
                public bool SupportsCS { get; set; }
                public bool SupportsJS { get; set; }
                public bool SupportsPython { get; set; }
                public bool SupportsRust { get; set; }
                public bool SupportsJava { get; set; }
            }

            // Test Scenario: Applied to Nested Aggregate (✗impermissible✗)
            public class PregnancyTest {
                public enum Status { Positive, Negative, Inconclusive }
                public enum Mechanism { Urinary, Blood, Visual, Hormonal }
                public record struct MedicalName(string Generic, string Brand);
                public record struct Listing(Guid ID, MedicalName Name, Mechanism Mechanism);

                [PrimaryKey] public DateTime ExactMeasurementTime { get; set; }
                [Check.IsNonZero(Path = "Name")] public Listing Product { get; set; }
                public DateTime Taken { get; set; }
                public string Taker { get; set; } = "";
                public Status Result { get; set; }
            }

            // Test Scenario: Applied to Reference-Nested Numeric Scalar (✓constrained✓)
            public class EgyptianPyramid {
                public class Pharaoh {
                    [PrimaryKey] public string RegnalName { get; set; } = "";
                    [PrimaryKey] public byte RegnalNumber { get; set; }
                    public byte Dynasty { get; set; }
                }

                [PrimaryKey] public Guid PyramidID { get; set; }
                public float Latitude { get; set; }
                public float Longitude { get; set; }
                [Check.IsNonZero(Path = "RegnalNumber")] public Pharaoh? Entombed { get; set; }
                public ulong Base { get; set; }
                public ulong SlantHeight { get; set; }
                public bool Excavated { get; set; }
            }

            // Test Scenario: Applied to Reference-Nested Non-Numeric Scalar (✗impermissible✗)
            public class Pajamas {
                public enum Kind { TwoPiece, Nightdress, Lingerie, Footie, OnePiece }

                public class Store {
                    [PrimaryKey] public Guid ID { get; set; }
                    public string Name { get; set; } = "";
                    public string? StockSymbol { get; set; }
                    public string CEO { get; set; } = "";
                    public ulong NumStores { get; set; }
                }

                [PrimaryKey] public Guid ProductID { get; set; }
                [Check.IsNonZero(Path = "ID")] public Store Retailer { get; set; } = new();
                public Kind Style { get; set; }
                public string Material { get; set; } = "";
                public bool DaytimeAppropriate { get; set; }
            }

            // Test Scenario: Applied to Nested Reference (✗impermissible✗)
            public class Galaxy {
                public enum GalaxyShape { Elliptical, Lenticular, BarredSpiral, Quasar, Irregular }

                public class Person {
                    [PrimaryKey] public string FirstName { get; set; } = "";
                    [PrimaryKey] public string LastName { get; set; } = "";
                }
                public record struct DiscoveryData(Person Astronomer, DateTime When, bool ImmediatelyAccepted);

                [PrimaryKey] public string Name { get; set; } = "";
                public int MessierNumber { get; set; }
                public GalaxyShape Shape { get; set; }
                public double Declination { get; set; }
                public double RedShift { get; set; }
                public double ApparentMagnitude { get; set; }
                public ulong SizeKiloParsecs { get; set; }
                [Check.IsNonZero(Path = "Astronomer")] public DiscoveryData Discovery { get; set; }
            }

            // Test Scenario: Original Constraint on Reference-Nested Field (✓not propagated✓)
            public class NewsAnchor {
                public class Network {
                    [PrimaryKey] public string Name { get; set; } = "";
                    [PrimaryKey, Check.IsNonZero] public ushort Channel { get; set; }
                    public ulong Viewership { get; set; }
                    public double Ratings { get; set; }
                    public decimal Budget { get; set; }
                }

                [PrimaryKey] public Guid ReporterID { get; set; }
                public string Name { get; set; } = "";
                public Network Station { get; set; } = new();
                public decimal Salary { get; set; }
                public byte NewscastsPerWeek { get; set; }
                public uint PeabodyAwards { get; set; }
            }

            // Test Scenario: Applied to Relation-Nested Numeric Scalar (✓constrained✓)
            public class CircleDance {
                [PrimaryKey] public string Name { get; set; } = "";
                public string Tradition { get; set; } = "";
                public double Duration { get; set; }
                public ushort? MaxParticipants { get; set; }
                [Check.IsNonZero(Path = "Key")] public RelationMap<uint, string> Steps { get; set; } = new();
            }

            // Test Scenario: Applied to Relation-Nested Non-Numeric Scalar (✗impermissible✗)
            public class KosherAgency {
                public enum CertType { Kosher, KohserDairy, KoserForPassover, KohserYisrael }
                public enum Judaism { Reform, Conservative, Orthodox }

                public class Company {
                    [PrimaryKey] public char ID { get; set; }
                    public string Name { get; set; } = "";
                    public decimal Revenue { get; set; }
                }
                public record struct Certification(Company Company, DateTime AsOf, CertType Kind);

                [PrimaryKey] public string Name { get; set; } = "";
                public DateTime Established { get; set; }
                public char CertificationSymbol { get; set; }
                [Check.IsNonZero(Path = "Item.ID")] public RelationSet<Company> CertifiedCompanies { get; set; } = new();
                public Judaism Branch { get; set; }
            }

            // Test Scenario: Applied to Nested Relation (✗impermissible✗)
            public class CarpoolKaraoke {
                public record struct Cantante(string Singer, RelationList<string> Songs);

                [PrimaryKey] public uint EpisodeNumber { get; set; }
                [Check.IsNonZero(Path = "Songs")] public Cantante Guest { get; set; }
                public double Duration { get; set; }
                public ulong YouTubeViews { get; set; }
                public bool Viral { get; set; }
                public uint JamesCordenWords { get; set; }
            }

            // Test Scenario: Applied to Field Data-Converted to Numeric Type (✓constrained✓)
            public class Airline {
                [PrimaryKey] public string Name { get; set; } = "";
                public bool Active { get; set; }
                public ulong FleetSize { get; set; }
                public ulong YearlyPassengers { get; set; }
                public ulong Employees { get; set; }
                [DataConverter(typeof(ToInt<char>)), Check.IsNonZero] public char ConsumerGrade { get; set; }
            }

            // Test Scenario: Applied to Field Data-Converted from Numeric Type (✗impermissible✗)
            public class Elevator {
                [PrimaryKey] public Guid ProductNumber { get; set; }
                public DateTime LastInspected { get; set; }
                public float MaxLoad { get; set; }
                [Check.IsNonZero, DataConverter(typeof(ToString<int>))] public int NumFloors { get; set; }
            }

            // Test Scenario: Scalar Property Constrained Multiple Times (✓redundant✓)
            public class Shoe {
                [PrimaryKey] public Guid ProductID { get; set; }
                public string Brand { get; set; } = "";
                [Check.IsNonZero, Check.IsNonZero] public float? Mens { get; set; }
                public float? Womens { get; set; }
                public bool IsHighHeel { get; set; }
                public bool IsBoot { get; set; }
                public bool IsSportsShoe { get; set; }
            }

            // Test Scenario: <Path> is `null` (✗illegal✗)
            public class Igloo {
                [PrimaryKey] public Guid IglooID { get; set; }
                [Check.IsNonZero(Path = null!)] public uint NumIceBlocks { get; set; }
                public ulong InternalArea { get; set; }
                public ushort Height { get; set; }
                public double InternalTemperature { get; set; }
            }

            // Test Scenario: <Path> on Scalar Does Not Exist (✗non-existent path✗)
            public class Cryptocurrency {
                [PrimaryKey] public string CoinName { get; set; } = "";
                public float Precision { get; set; }
                [Check.IsNonZero(Path = "---")] public decimal ExchangeRate { get; set; }
                public DateTime InitialRelease { get; set; }
                public ulong AccountHolders { get; set; }
            }

            // Test Scenario: <Path> on Aggregate Does Not Exist (✗non-existent path✗)
            public class Encomienda {
                public record struct Coordinate(float Latitude, float Longitude);

                [Check.IsNonZero(Path = "---")] public Coordinate Location { get; set; }
                [PrimaryKey] public string Holder { get; set; } = "";
                [PrimaryKey] public uint Index { get; set; }
                public DateTime Established { get; set; }
                public ushort Workers { get; set; }
                public double PercentMoorish { get; set; }
                public double PercentSpanish { get; set; }
                public double PercentJewish { get; set; }
                public double PercentNative { get; set; }
            }

            // Test Scenario: <Path> on Aggregate Not Specified (✗missing path✗)
            public class Smoothie {
                public record struct SmoothieBase(double AmountIce, double AmountYogurt, double AmountOJ, double AmountCream);

                [PrimaryKey] public string Identifier { get; set; } = "";
                public uint Calories { get; set; }
                [Check.IsNonZero] public SmoothieBase Base { get; set; }
                public string Fruits { get; set; } = "";
                public string Supplements { get; set; } = "";
                public bool HasChocolate { get; set; }
            }

            // Test Scenario: <Path> on Reference Does Not Exist (✗non-existent path✗)
            public class Antibiotic {
                public enum AntibioticClass { BetaLactam, Cephalosporin, Carbapenem, Aminoglycoside, Sulfonamide, Quinolone, Oxazolidinone }

                public class OrganicFormula {
                    [PrimaryKey] public byte Carbon { get; set; }
                    [PrimaryKey] public byte Oxygen { get; set; }
                    [PrimaryKey] public byte Hydrogen { get; set; }
                    [PrimaryKey] public byte Nitrogen { get; set; }
                    [PrimaryKey] public byte Sulfur { get; set; }
                }

                [PrimaryKey] public Guid MedicalID { get; set; }
                public string Name { get; set; } = "";
                public bool IsNaturallyOccurring { get; set; }
                [Check.IsNonZero(Path = "---")] public OrganicFormula Formula { get; set; } = new();
                public AntibioticClass Class { get; set; }
                public double Efficacy { get; set; }
            }

            // Test Scenario: <Path> on Reference Refers to Non-Primary-Key Field (✗non-existent path✗)
            public class Chopsticks {
                public class Chopstick {
                    [PrimaryKey] public Guid ItemID { get; set; }
                    public double Length { get; set; }
                    public double Weight { get; set; }
                    public string Material { get; set; } = "";
                    public bool Reusable { get; set; }
                }

                [PrimaryKey] public Guid ID { get; set; }
                public Chopstick Chopstick1 { get; set; } = new();
                [Check.IsNonZero(Path = "Weight")] public Chopstick Chopstick2 { get; set; } = new();
            }

            // Test Scenario: <Path> on Reference Not Specified (✗missing path✗)
            public class TongueTwister {
                public class Word {
                    [PrimaryKey] public string Text { get; set; } = "";
                    public ulong FrequencyRating { get; set; }
                    public ulong NumericValue { get; set; }
                }

                [PrimaryKey] public Guid ID { get; set; }
                public Word Word1 { get; set; } = new();
                public Word Word2{ get; set; } = new();
                public Word Word3 { get; set; } = new();
                public Word Word4 { get; set; } = new();
                public Word Word5 { get; set; } = new();
                public Word Word6 { get; set; } = new();
                public Word Word7 { get; set; } = new();
                public Word Word8 { get; set; } = new();
                public Word Word9 { get; set; } = new();
                public Word Word10 { get; set; } = new();
                [Check.IsNonZero] public Word? Word11 { get; set; } = new();
                public Word? Word12 { get; set; } = new();
                public Word? Word13 { get; set; } = new();
                public Word? Word14 { get; set; } = new();
                public Word? Word15 { get; set; } = new();
                public Word? Word16 { get; set; } = new();
                public Word? Word17 { get; set; } = new();
                public Word? Word18 { get; set; } = new();
                public Word? Word19 { get; set; } = new();
                public Word? Word20 { get; set; } = new();
            }

            // Test Scenario: <Path> on Relation Does Not Exist (✗non-existent path✗)
            public class HallPass {
                public enum Color { Red, Green, Blue, Orange, Black, White, Pink, Purple, Yellow, Gray, Brown }

                [PrimaryKey] public uint HallPassNumber { get; set; }
                public string IssuedBy { get; set; } = "";
                public string School { get; set; } = "";
                public DateTime LastIssued { get; set; }
                [Check.IsNonZero(Path = "---")] public RelationList<string> PermittedLocations { get; set; } = new();
                public bool IsExpired { get; set; }
            }

            // Test Scenario: <Path> on Relation Refers to Non-Primary-Key Field of Anchor Entity (✗non-existent path✗)
            public class Casserole {
                public enum Kind { American, French, German, Portuguese, Greek, Scandinavian, Polish, Russian, Italian, Other }

                [PrimaryKey] public Guid ID { get; set; }
                public string Name { get; set; } = "";
                public Kind Cuisine { get; set; }
                public float IdealPanDepth { get; set; }
                [Check.IsNonZero(Path = "Casserole.IdealPanDepth")] public RelationMap<string, bool> Ingredients { get; set; } = new();
                public bool IsGratin { get; set; }
            }

            // Test Scenario: <Path> on Relation Not Specified (✗missing path✗)
            public class GarbageTruck {
                [PrimaryKey] public string LicensePlate { get; set; } = "";
                [Check.IsNonZero] public RelationSet<string> RouteStops { get; set; } = new();
                public ulong TonnesProcessed { get; set; }
                public string ManagementCompany { get; set; } = "";
                public string Driver { get; set; } = "";
                public bool AlsoRecycling { get; set; }
            }

            // Test Scenario: Default Value Does Not Satisfy Constraint (✗contradiction✗)
            public class Pulley {
                [PrimaryKey] public float RopeLength { get; set; }
                [PrimaryKey, Check.IsNonZero, Default(0.0)] public double RopeTension { get; set; }
                [PrimaryKey] public byte NumWheels { get; set; }
                [PrimaryKey] public bool Moveable { get; set; }
            }

            // Test Scenario: Originally Valid Default Value No Longer Satisfies Constraint (✗contradiction✗)
            public class Ceviche {
                public enum Fruit { Lemon, Lime, Grapefruit, Pomelo, Orange, Citron, Kumquat, Other }

                public struct Citrus {
                    public Fruit Variety { get; set; }
                    [Default(0.0f)] public float Tablespoons { get; set; }
                    public bool Zest { get; set; }
                }

                [PrimaryKey] public Guid ID { get; set; }
                public string Fish1 { get; set; } = "";
                public string Fish2 { get; set; } = "";
                public string Fish3 { get; set; } = "";
                [Check.IsNonZero(Path = "Tablespoons")] public Citrus CitrusIngredient { get; set; }
                public uint TotalCalories { get; set; }
            }
        }
    }

    internal static class ComparisonConstraints {
        public static class IsGreaterThan {
            // Test Scenario: Applied to Numeric Fields (✓constrained✓)
            public class DNDSpell {
                [PrimaryKey] public string Name { get; set; } = "";
                [Check.IsGreaterThan((ushort)0)] public ushort Range { get; set; }
                [Check.IsGreaterThan(-1)] public int Level { get; set; }
                [Check.IsGreaterThan(2.5f)] public float AverageDamage { get; set; }
                public bool AsBonusAction { get; set; }
                public bool HasVerbalComponents { get; set; }
                public bool HasSomaticComponents { get; set; }
                public bool HasPhysicalComponents { get; set; }
            }

            // Test Scenario: Applied to Textual Fields (✓constrained✓)
            public class MultipleChoiceQuestion {
                [PrimaryKey] public Guid ID { get; set; }
                [Check.IsGreaterThan('*')] public char CorrectAnswer { get; set; }
                public string QuestionText { get; set; } = "";
                [Check.IsGreaterThan("A. ")] public string ChoiceA { get; set; } = "";
                [Check.IsGreaterThan("B. ")] public string ChoiceB { get; set; } = "";
                [Check.IsGreaterThan("C. ")] public string ChoiceC { get; set; } = "";
                [Check.IsGreaterThan("D. ")] public string ChoiceD { get; set; } = "";
            }

            // Test Scenario: Applied to Boolean Field (✗impermissible✗)
            public class Font {
                [PrimaryKey] public string Name { get; set; } = "";
                public ulong YearReleased { get; set; }
                public decimal XHeight { get; set; }
                [Check.IsGreaterThan(false)] public bool HasSerifs { get; set; }
            }

            // Test Scenario: Applied to Decimal Field (✓constrained✓)
            public class AuctionLot {
                [PrimaryKey] public string Auction { get; set; } = "";
                [PrimaryKey] public uint LotNumber { get; set; }
                public string ItemDescription { get; set; } = "";
                public ulong Bidders { get; set; }
                [Check.IsGreaterThan(10000.0)] public decimal TopBid { get; set; }
            }

            // Test Scenario: Applied to DateTime Field (✓constrained✓)
            public class GoldRush {
                [PrimaryKey] public string Location { get; set; } = "";
                [PrimaryKey, Check.IsGreaterThan("1200-03-18")] public DateTime StartDate { get; set; }
                [PrimaryKey, Check.IsGreaterThan("1176-11-22")] public DateTime EndDate { get; set; }
                public decimal EstimatedGrossWealth { get; set; }
            }

            // Test Scenario: Applied to Guid Field (✗impermissible✗)
            public class Skyscraper {
                [PrimaryKey] public string Address { get; set; } = "";
                public ushort Floors { get; set; }
                public decimal BuildingCost { get; set; }
                [Check.IsGreaterThan("be09a584-469f-4ca9-8dd2-6ff3d3c5e7f6")] public Guid RegistryIdentifier { get; set; }
                public ulong Height { get; set; }
                public ulong AntennaHeight { get; set; }
                public DateTime Completed { get; set; }
                public byte Elevators { get; set; }
            }

            // Test Scenario: Applied to Enumeration Field (✗impermissible✗)
            public class Orisha {
                [Flags] public enum Culture { Yoruba = 1, Santeria = 2, Oyotunji = 4, Candomble = 8 }

                [PrimaryKey] public string Name { get; set; } = "";
                [Check.IsGreaterThan(Culture.Santeria)] public Culture BelongsTo { get; set; }
                public string Domain { get; set; } = "";
                public uint WikipediaWords { get; set; }
            }

            // Test Scenario: Applied to Aggregate-Nested Orderable Scalar (✓constrained✓)
            public class Opioid {
                public record struct ChemicalFormula(int C, int H, int N, int O);
                public record struct Entry(ChemicalFormula Formula, string DrugBank);

                [PrimaryKey] public string CAS { get; set; } = "";
                [Check.IsGreaterThan("XP14U339D", Path = "DrugBank"), Check.IsGreaterThan(2, Path = "Formula.O")] public Entry Definition { get; set; }
                public double Mortality { get; set; }
                public double MolarMass { get; set; }
                public bool IsIllegalInUS { get; set; }
            }

            // Test Scenario: Applied to Aggregate-Nested Non-Orderable Scalar (✗impermissible✗)
            public class Wordle {
                public enum Result { Unknown, Correct, Incorrect, WrongLocation }
                public record struct Letter(char Value, Result Hint);
                public record struct Guess(Letter L1, Letter L2, Letter L3, Letter L4, Letter L5);

                [PrimaryKey] public string IP { get; set; } = "";
                [PrimaryKey] public DateTime Date { get; set; }
                public string Answer { get; set; } = "";
                public Guess Guess1 { get; set; }
                public Guess? Guess2 { get; set; }
                [Check.IsGreaterThan(Result.Unknown, Path = "L4.Hint")] public Guess? Guess3 { get; set; }
                public Guess? Guess4 { get; set; }
                public Guess? Guess5 { get; set; }
            }

            // Test Scenario: Applied to Nested Aggregate (✗impermissible✗)
            public class FlashMob {
                public record struct Person(string FirstName, string LastName);
                public record struct Crowd(Person Leader, ulong Size);

                [PrimaryKey] public Guid FlashMobUUID { get; set; }
                public string Song { get; set; } = "";
                public DateTime StartTimestamp { get; set; }
                public DateTime EndTimestamp { get; set; }
                public string Location { get; set; } = "";
                [Check.IsGreaterThan('<', Path = "Leader")] public Crowd Participants { get; set; }
            }

            // Test Scenario: Applied to Reference-Nested Orderable Scalar (✓constrained✓)
            public class Apostle {
                public class Religion {
                    [PrimaryKey] public string Identifier { get; set; } = "";
                    public ulong WorldwideFollowers { get; set; }
                    public bool Monotheistic { get; set; }
                }

                [PrimaryKey] public Guid HolyID { get; set; }
                [Check.IsGreaterThan("Atheism", Path = "Identifier")] public Religion Adherence { get; set; } = new();
                public DateTime Ordination { get; set; }
                public bool OfJesus { get; set; }
                public bool Martyred { get; set; }
                public ulong TotalConversions { get; set; }
            }

            // Test Scenario: Applied to Reference-Nested Non-Orderable Scalar (✗impermissible✗)
            public class Influenza {
                public class Outbreak {
                    [PrimaryKey] public Guid OutbreakID { get; set; }
                    public DateTime Starting { get; set; }
                    public ulong Casualties { get; set; }
                }

                [PrimaryKey] public string Virus { get; set; } = "";
                public bool ExistsVaccine { get; set; }
                [Check.IsGreaterThan("a522c08b-b030-4658-b6cf-c729d6366805", Path = "OutbreakID")] public Outbreak? DeadliestOutbreak { get; set; }
                public bool IsZoonotic { get; set; }
            }

            // Test Scenario: Applied to Nested Reference (✗impermissible✗)
            public class BloodDrive {
                public class Hospital {
                    [PrimaryKey] public Guid HospitalID { get; set; }
                    public string Name { get; set; } = "";
                    public bool PrivatelyOperated { get; set; }
                    public uint NumDoctors { get; set; }
                    public uint NumNurses { get; set; }
                    public bool HasOBGYN { get; set; }
                }
                public record struct Sponsorship(Hospital? Hospital, string? Company, string? University, bool RedCross);

                [PrimaryKey] public Guid DriveID { get; set; }
                public DateTime Scheduled { get; set; }
                [Check.IsGreaterThan('u', Path = "Hospital")] public Sponsorship SponsoredBy { get; set; }
                public ulong Donors { get; set; }
                public double BloodCollected { get; set; }
                public double PlasmaCollected { get; set; }
            }

            // Test Scenario: Original Constraint on Reference-Nested Field (✓not propagated✓)
            public class PapalBull {
                public class Pope {
                    [PrimaryKey] public string PapalName { get; set; } = "";
                    [PrimaryKey, Check.IsGreaterOrEqualTo("C")] public string RegnalNumber { get; set; } = "";
                    public string BirthName { get; set; } = "";
                    public DateTime PontificateBegin { get; set; }
                    public DateTime? PontificatEnd { get; set; }
                }

                [PrimaryKey] public string Title { get; set; } = "";
                public uint WordCount { get; set; }
                public DateTime IssuedOn { get; set; }
                public Pope IssuedBy { get; set; } = new();
            }

            // Test Scenario: Applied to Relation-Nested Orderable Scalar (✓constrained✓)
            public class KidNextDoor {
                [PrimaryKey] public long Number { get; set; }
                public string LegalName { get; set; } = "";
                public string Sector { get; set; } = "";
                public string VoiceActor { get; set; } = "";
                [Check.IsGreaterThan('@', Path = "Key"), Check.IsGreaterThan(0L, Path = "KidNextDoor.Number")] public RelationMap<char, string> DebutMission { get; set; } = new();
                public string? ArchNemesis { get; set; } = "";
            }

            // Test Scenario: Applied to Relation-Nested Non-Orderable Scalar (✗impermissible✗)
            public class Antari {
                public enum LNDN { Grey, Red, White, Black }
                public enum Element { Fire, Earth, Wind, Water, Metal, Bone }
                
                public class Spell {
                    [PrimaryKey] public Guid SpellID { get; set; }
                    public string Incantation { get; set; } = "";
                    public ushort TimesCast { get; set; }
                }

                [PrimaryKey] public string Name { get; set; } = "";
                public LNDN London { get; set; }
                public Element StrongestElement { get; set; }
                [Check.IsGreaterThan("9dc286f1-3ce5-4bd6-9afa-5bb6049e0ffe", Path = "Item.SpellID")] public RelationList<Spell> KnownSpells { get; set; } = new();
                public DateTime Birthdate { get; set; }
                public bool Deceased { get; set; }
                public byte? BestEssenTaschPlacement { get; set; }
            }

            // Test Scenario: Applied to Nested Relation (✗impermissible✗)
            public class Clown {
                public enum Kind { Party, Jester, Demon, Circus, Rodeo, Other }
                public record struct Outfit(decimal Nose, RelationList<string> Accoutrement, double Shoes);

                [PrimaryKey] public Guid ClownID { get; set; }
                public string Name { get; set; } = "";
                [Check.IsGreaterThan(1001, Path = "Accoutrement")] public Outfit Costume { get; set; }
                public Kind Type { get; set; }
            }

            // Test Scenario: Applied to Nullable Fields with Total Orders (✓constrained✓)
            public class Baryon {
                [PrimaryKey] public string Name { get; set; } = "";
                [Check.IsGreaterThan('-')] public char? Symbol { get; set; }
                public double Spin { get; set; }
                [Check.IsGreaterThan((short)-5)] public short? Charge { get; set; }
                public float AngularMomentum { get; set; }
                public bool PositiveParity { get; set; }
                [Check.IsGreaterThan("1344-06-21")] public DateTime? Discovered { get; set; }
            }

            // Test Scenario: Inconvertible Non-`null` Anchor (✗invalid✗)
            public class Racehorse {
                [PrimaryKey] public string Name { get; set; } = "";
                public DateTime Birth { get; set; }
                public DateTime? Death { get; set; }
                [Check.IsGreaterThan(true)] public ulong? FirstDerbyWin { get; set; }
                public ushort Wins { get; set; }
                public ushort Places { get; set; }
                public ushort Shows { get; set; }
                public bool IsStallion { get; set; }
                public double CareerEarnings { get; set; }
            }

            // Test Scenario: Convertible Non-`null` Anchor (✗invalid✗)
            public class ChineseCharacter {
                [PrimaryKey, Check.IsGreaterThan((byte)14)] public char Character { get; set; }
                public string PinyinTransliteration { get; set; } = "";
                public string CantoneseTransliteration { get; set; } = "";
                public string PrimaryDefinition { get; set; } = "";
                public ulong UnicodeCodePoint { get; set; }
            }

            // Test Scenario: Single-Element Array Anchor (✗invalid✗)
            public class Query {
                [PrimaryKey] public string SELECT { get; set; } = "";
                [PrimaryKey] public string FROM { get; set; } = "";
                [PrimaryKey, Check.IsGreaterThan(new[] { "\0" })] public string WHERE { get; set; } = "";
                [PrimaryKey] public string GROUP_BY { get; set; } = "";
                [PrimaryKey] public string ORDER_BY { get; set; } = "";
                [PrimaryKey] public string LIMIT { get; set; } = "";
            }

            // Test Scenario: `null` Anchor (✗invalid✗)
            public class UNResolution {
                [PrimaryKey] public int Number { get; set; }
                public string Title { get; set; } = "";
                [Check.IsGreaterThan(null!)] public ushort? NumSignatories { get; set; }
                public DateTime Introduced { get; set; }
                public bool Adopted { get; set; }
            }

            // Test Scenario: Anchor is Maximum Value (✗unsatisfiable✗)
            public class Upanishad {
                [PrimaryKey] public string Name { get; set; } = "";
                [Check.IsGreaterThan(sbyte.MaxValue)] public sbyte Index { get; set; }
                public DateTime WhenAuthored { get; set; }
                public ulong WordCount { get; set; }
            }

            // Test Scenario: Decimal Anchor is Not a Double (✗invalid✗)
            public class GarageSale {
                [PrimaryKey] public string Address { get; set; } = "";
                [PrimaryKey] public DateTime When { get; set; }
                public ushort NumItems { get; set; }
                [Check.IsGreaterThan(200)] public decimal Gross { get; set; }
            }

            // Test Scenario: Decimal Anchor is Out-of-Range (✗invalid✗)
            public class TalkShow {
                [PrimaryKey] public string Title { get; set; } = "";
                public string Host { get; set; } = "";
                public DateTime Premiered { get; set; }
                public DateTime? Concluded { get; set; }
                public uint NumEpisodes { get; set; }
                [Check.IsGreaterThan(double.MinValue)] public decimal Rating { get; set; }
            }

            // Test Scenario: DateTime Anchor is Not a String (✗invalid✗)
            public class Meme {
                [PrimaryKey] public Guid ID { get; set; }
                public string ImageURL { get; set; } = "";
                public ushort Length { get; set; }
                public ushort Width { get; set; }
                [Check.IsGreaterThan('N')] public DateTime FirstPublished { get; set; }
            }

            // Test Scenario: DateTime Anchor is Improperly Formatted (✗invalid✗)
            public class ChristianDenomination {
                [PrimaryKey] public string Name { get; set; } = "";
                public ulong Adherents { get; set; }
                [Check.IsGreaterThan("0001_01_01")] public DateTime Founded { get; set; }
                public bool IsProtestant { get; set; }
            }

            // Test Scenario: DateTime Anchor is Out-of-Range (✗invalid✗)
            public class GraduateThesis {
                [PrimaryKey] public string Title { get; set; } = "";
                [PrimaryKey] public string Author { get; set; } = "";
                public string Program { get; set; } = "";
                public string? DegreeAwarded { get; set; }
                [Check.IsGreaterThan("1873-15-12")] public DateTime Argued { get; set; }
                public ulong WordCount { get; set; }
                public ushort NumCitations { get; set; }
                public ushort NumFootnotes { get; set; }
            }

            // Test Scenario: Anchor of Source Type on Data-Converted Property (✗invalid✗)
            public class Azeotrope {
                [PrimaryKey] public string Liquid1 { get; set; } = "";
                [PrimaryKey] public string Liquid2 { get; set; } = "";
                [DataConverter(typeof(ToString<float>)), Check.IsGreaterThan(-237.44f)] public float BoilingPoint { get; set; }
            }

            // Test Scenario: Anchor of Target Type on Data-Converted Property (✓valid✓)
            public class BingoCard {
                [PrimaryKey] public int CellR1C1 { get; set; }
                [PrimaryKey] public int CellR1C2 { get; set; }
                [PrimaryKey] public int CellR1C3 { get; set; }
                [PrimaryKey] public int CellR1C4 { get; set; }
                [PrimaryKey] public int CellR1C5 { get; set; }
                [PrimaryKey] public int CellR2C1 { get; set; }
                [PrimaryKey] public int CellR2C2 { get; set; }
                [PrimaryKey] public int CellR2C3 { get; set; }
                [PrimaryKey] public int CellR2C4 { get; set; }
                [PrimaryKey] public int CellR2C5 { get; set; }
                [PrimaryKey] public int CellR3C1 { get; set; }
                [PrimaryKey] public int CellR3C2 { get; set; }
                [PrimaryKey] public int CellR3C4 { get; set; }
                [PrimaryKey] public int CellR3C5 { get; set; }
                [PrimaryKey, DataConverter(typeof(ToString<int>)), Check.IsGreaterThan("-1")] public int CellR4C1 { get; set; }
                [PrimaryKey] public int CellR4C2 { get; set; }
                [PrimaryKey] public int CellR4C3 { get; set; }
                [PrimaryKey] public int CellR4C4 { get; set; }
                [PrimaryKey] public int CellR4C5 { get; set; }
                [PrimaryKey] public int CellR5C1 { get; set; }
                [PrimaryKey] public int CellR5C2 { get; set; }
                [PrimaryKey] public int CellR5C3 { get; set; }
                [PrimaryKey] public int CellR5C4 { get; set; }
                [PrimaryKey] public int CellR5C5 { get; set; }
            }

            // Test Scenario: Scalar Property Constrained Multiple Times (✓maximized✓)
            public class NuclearPowerPlant {
                [PrimaryKey] public string OfficialName { get; set; } = "";
                public DateTime Opened { get; set; }
                public bool IsOnline { get; set; }
                [Check.IsGreaterThan(-1L), Check.IsGreaterThan(37L)] public long Meltdowns { get; set; }
                public ushort Reactors { get; set; }
                public ulong ThermalCapacity { get; set; }
                public ulong PowerGenerated { get; set; }
            }

            // Test Scenario: <Path> is `null` (✗illegal✗)
            public class Domino {
                [PrimaryKey] public byte LeftPips { get; set; }
                [PrimaryKey, Check.IsGreaterThan((byte)0, Path = null!)] public byte RightPips { get; set; }
            }

            // Test Scenario: <Path> on Scalar Does Not Exist (✗non-existent path✗)
            public class Canyon {
                [PrimaryKey] public string Name { get; set; } = "";
                public float CenterLatitude { get; set; }
                public float CenterLongitude { get; set; }
                [Check.IsGreaterThan(100.65, Path = "---")] public double Depth { get; set; }
                public ulong TotalArea { get; set; }
            }

            // Test Scenario: <Path> on Aggregate Does Not Exist (✗non-existent path✗)
            public class Conlang {
                public record struct LanguageCodes(string? ISO6393, string? Glottolog, string? IETF);

                [PrimaryKey] public string Name { get; set; } = "";
                public string Endonym { get; set; } = "";
                public string Conlanger { get; set; } = "";
                public ushort NumConsonants { get; set; }
                public ushort NumVowels { get; set; }
                [Check.IsGreaterThan("Astapori Valyrian", Path = "---")] public LanguageCodes Codes { get; set; }
            }

            // Test Scenario: <Path> on Aggregate Not Specified (✗missing path✗)
            public class LaborStrike {
                public record struct Parties(string Union, string Management, string? Negotiators);

                [PrimaryKey] public Guid ID { get; set; }
                public DateTime StrikeStart { get; set; }
                public DateTime StrikeEnd { get; set; }
                public decimal LostRevenue { get; set; }
                [Check.IsGreaterThan("2023")] public Parties Members { get; set; }
                public bool ResolvedInFavorOfLabor { get; set; }
            }

            // Test Scenario: <Path> on Reference Does Not Exist (✗non-existent path✗)
            public class InstallationWizard {
                public class Software {
                    [PrimaryKey] public string Name { get; set; } = "";
                    [PrimaryKey] public string Publisher { get; set; } = "";
                    public string Version { get; set; } = "";
                }

                [PrimaryKey] public Guid ID { get; set; }
                [Check.IsGreaterThan("173dF-?", Path = "---")] public Software Program { get; set; } = new();
                public byte NumPages { get; set; }
                public bool Standalone { get; set; }
                public bool RequiresWiFi { get; set; }
                public bool UninstallsPreviousVersions { get; set; }
            }

            // Test Scenario: <Path> on Reference Refers to Non-Primary-Key Field (✗non-existent path✗)
            public class BugSpray {
                public class Chemical {
                    [PrimaryKey] public Guid ChemicalID { get; set; }
                    public string Formula { get; set; } = "";
                    public double LethalDose { get; set; }
                }

                [PrimaryKey] public Guid ProductID { get; set; }
                public string Brand { get; set; } = "";
                public double Efficacy { get; set; }
                [Check.IsGreaterThan(1765.12, Path = "LethalDose")] public Chemical ActiveIngredient { get; set; } = new();
                public bool IsInsecticide { get; set; }
            }

            // Test Scenario: <Path> on Reference Not Specified (✗missing path✗)
            public class Intern {
                public class University {
                    [PrimaryKey] public string System { get; set; } = "";
                    [PrimaryKey] public string Campus { get; set; } = "";
                    public bool IsStateSchool { get; set; }
                    public ulong UndergraduateStudents { get; set; }
                    public ulong GraduateStudents { get; set; }
                    public ulong Faculty { get; set; }
                    public decimal Endowment { get; set; }
                }
                public class Employee {
                    public Guid EmployeeID { get; set; }
                    public string FirstName { get; set; } = "";
                    public string LastName { get; set; } = "";
                    public string Title { get; set; } = "";
                    public decimal Salary { get; set; }
                }

                [PrimaryKey] public string FirstName { get; set; } = "";
                [PrimaryKey] public string LastName { get; set; } = "";
                public byte Age { get; set; }
                public decimal WeeklySalary { get; set; }
                public bool? ReturnOffer { get; set; }
                public University College { get; set; } = new();
                [Check.IsGreaterThan("1357-08-16")] public Employee Manager { get; set; } = new();
            }

            // Test Scenario: <Path> on Relation Does Not Exist (✗non-existent path✗)
            public class Delicatessen {
                public class MenuItem {
                    [Flags] public enum Meal { Breakfast = 1, Lunch = 2, Dinner = 4, HolidayOnly = 256 }

                    [PrimaryKey] public ushort ItemNumber { get; set; }
                    public string ItemName { get; set; } = "";
                    public string Description { get; set; } = "";
                    public decimal Price { get; set; }
                    public bool IsSpecial { get; set; }
                    public Meal Availability { get; set; }
                }

                [PrimaryKey] public string Name { get; set; } = "";
                public DateTime Opened { get; set; }
                public string Owner { get; set; } = "";
                public string? URL { get; set; }
                public ushort MaxSeating { get; set; }
                [Check.IsGreaterThan(18752.53f, Path = "---")] public RelationList<MenuItem> MenuItems { get; set; } = new();
                public bool OffersCatering { get; set; }
                public bool JewishStyle { get; set; }
            }

            // Test Scenario: <Path> on Relation Refers to Non-Primary-Key Field of Anchor Entity (✗non-existent path✗)
            public class BlackjackHand {
                public enum FaceValue { Ace, King, Queen, Jack, Ten, Nine, Eight, Seven, Six, Five, Four, Three, Two };
                public enum CardSuit { Hearts, Diamonds, Clubs, Spades }
                public record struct Card(FaceValue Value, CardSuit Suit);

                [PrimaryKey] public Guid HandID { get; set; }
                public RelationSet<Card> PlayerCards { get; set; } = new();
                [Check.IsGreaterThan(17512UL, Path = "BlackjackHand.TotalPot")] public RelationSet<Card> DealerCards { get; set; } = new();
                public ulong TotalPot { get; set; }
                public bool DoubleDown { get; set; }
            }

            // Test Scenario: <Path> on Relation Not Specified (✗missing path✗)
            public class Inquisition {
                [PrimaryKey] public DateTime Start { get; set; }
                [PrimaryKey] public DateTime End { get; set; }
                public string OfficialTitle { get; set; } = "";
                public string GrandInquisitor { get; set; } = "";
                [Check.IsGreaterThan("Religious Persecution")] public RelationSet<string> Victims { get; set; } = new();
                public bool IncludedWitchTrials { get; set; }
            }

            // Test Scenario: Default Value Does Not Satisfy Constraint (✗contradiction✗)
            public class DraftPick {
                [PrimaryKey] public string League { get; set; } = "";
                [PrimaryKey] public ushort Year { get; set; }
                [PrimaryKey] public byte Round { get; set; }
                [PrimaryKey] public byte PickNumber { get; set; }
                [Check.IsGreaterThan(0u), Default(0u)] public uint Overall { get; set; }
                public string Selector { get; set; } = "";
                public string Selection { get; set; } = "";
            }

            // Test Scenario: Originally Valid Default Value No Longer Satisfies Constraint (✗contradiction✗)
            public class Madrasa {
                public enum Branch { Sunni, Shia, Sufi, Druze, Twelver, Other }

                public struct SchoolIdentifier {
                    [Default('s')] public char Class { get; set; }
                    public int Number { get; set; }
                    public int SubGroup { get; set; }
                }

                [PrimaryKey, Check.IsGreaterThan('w', Path = "Class")] public SchoolIdentifier ID { get; set; }
                public string Name { get; set; } = "";
                public Branch BranchOfIslam { get; set; }
                public string Country { get; set; } = "";
                public ulong Enrollment { get; set; }
            }
        }

        public static class IsLessThan {
            // Test Scenario: Applied to Numeric Fields (✓constrained✓)
            public class Resistor {
                [PrimaryKey] public Guid CircuitComponentIdentifier { get; set; }
                [Check.IsLessThan(27814L)] public long Resistance { get; set; }
                [Check.IsLessThan(893.44501f)] public float PhysicalLength { get; set; }
                [Check.IsLessThan(27814UL)] public ulong Power { get; set; }
                public bool IsThermistor { get; set; }
                public bool IsVaristor { get; set; }
            }

            // Test Scenario: Applied to Textual Fields (✓constrained✓)
            public class Senator {
                [PrimaryKey] public string FirstName { get; set; } = "";
                public string? MiddleName { get; set; }
                [PrimaryKey, Check.IsLessThan("...")] public string LastName { get; set; } = "";
                public string State { get; set; } = "";
                public ulong LastElected { get; set; }
                public string Party { get; set; } = "";
                public bool IsInLeadership { get; set; }
                [Check.IsLessThan('G')] public char NRARating { get; set; }
            }

            // Test Scenario: Applied to Boolean Field (✗impermissible✗)
            public class Milkshake {
                [PrimaryKey] public string Proprietor { get; set; } = "";
                [PrimaryKey] public string Flavor { get; set; } = "";
                public decimal Cost { get; set; }
                public ulong Calories { get; set; }
                [Check.IsLessThan(true)] public bool IsDairyFree { get; set; }
            }

            // Test Scenario: Applied to Decimal Field (✓constrained✓)
            public class TreasuryBond {
                [PrimaryKey] public Guid BondID { get; set; }
                [Check.IsLessThan(57182391.33167994)] public decimal BoughtFor { get; set; }
                public ushort Maturity { get; set; }
                public double AverageYield { get; set; }
            }

            // Test Scenario: Applied to DateTime Field (✓constrained✓)
            public class Commercial {
                [PrimaryKey] public ushort Channel { get; set; }
                [PrimaryKey, Check.IsLessThan("2300-01-01")] public DateTime TimeSlot { get; set; }
                public byte LengthSeconds { get; set; }
                public bool ForSuperBowl { get; set; }
                public string? Company { get; set; } = "";
            }

            // Test Scenario: Applied to Guid Field (✗impermissible✗)
            public class DLL {
                [PrimaryKey, Check.IsLessThan("db5bf338-7dcd-46b5-85a2-ee3c518b9ed2")] public Guid ID { get; set; }
                public byte BitSize { get; set; }
                [PrimaryKey] public string AbsolutePath { get; set; } = "";
                public ulong MemoryKB { get; set; }
                public string? Author { get; set; }
                public float Version { get; set; }
            }

            // Test Scenario: Applied to Enumeration Field (✗impermissible✗)
            public class SolicitorGeneral {
                public enum PoliticalParty { Democrat, Republican, Green, Socialist, Independent }

                [PrimaryKey] public string Name { get; set; } = "";
                public string AppointedBy { get; set; } = "";
                [Check.IsLessThan(PoliticalParty.Green)] public PoliticalParty? Affiliation { get; set; }
                public uint CasesArgued { get; set; }
                public DateTime FirstSCOTUS { get; set; }
            }

            // Test Scenario: Applied to Aggregate-Nested Orderable Scalar (✓constrained✓)
            public class Raptor {
                public record struct Taxonomy(string Kingdom, string Phylum, string Clas, string Order, string Family, string Genus, string Species);
                public struct Bios {
                    public double Weight { get; set; }
                    [Check.IsLessThan((ushort)489)] public ushort Wingspan { get; set; }
                    public long TopSpeed { get; set; }
                }

                [PrimaryKey] public string CommonName { get; set; } = "";
                [Check.IsLessThan("Zynovia", Path = "Family")] public Taxonomy Scientific { get; set; }
                [Check.IsLessThan(174.991, Path = "Weight"), Check.IsLessThan(long.MaxValue, Path = "TopSpeed")] public Bios Measurements { get; set; }
                public byte Talons { get; set; }
            }

            // Test Scenario: Applied to Aggregate-Nested Non-Orderable Scalar (✗impermissible✗)
            public class Feruchemy {
                public enum Matrix { Physical, Cognitive, Hybrid, Spiritual }
                public record struct Effect(Matrix Kind, string WhenStoring, string WhenTapping);

                [PrimaryKey] public char Symbol { get; set; }
                public string Metal { get; set; } = "";
                public string FeruchemistTerm { get; set; } = "";
                [Check.IsLessThan(Matrix.Spiritual, Path = "Kind")] public Effect Effects { get; set; }
            }

            // Test Scenario: Applied to Nested Aggregate (✗impermissible✗)
            public class Firefighter {
                public record struct Polity(string City, string State, string Country);
                public record struct Crew(Polity ServiceArea, uint EngineNumber);

                [PrimaryKey] public ulong TranscontinentalFirefightersNumber { get; set; }
                public string Name { get; set; } = "";
                public string Rank { get; set; } = "";
                public uint FiresFought { get; set; }
                [Check.IsLessThan("Fahrenheit 451", Path = "ServiceArea")] public Crew Firehouse { get; set; }
                public bool WorkedSept11 { get; set; }
            }

            // Test Scenario: Applied to Reference-Nested Orderable Scalar (✓constrained✓)
            public class Butterfly {
                public class TaxonomicGenus {
                    [PrimaryKey] public string Name { get; set; } = "";
                    public string Family { get; set; } = "";
                    public string Order { get; set; } = "";
                    public string Class { get; set; } = "";
                    public string Phylum { get; set; } = "";
                    public string Kingdom { get; set; } = "";
                    public string Domain { get; set; } = "";
                }

                [PrimaryKey] public string CommonName { get; set; } = "";
                [Check.IsLessThan("Zojemana", Path = "Name")] public TaxonomicGenus Genus { get; set; } = new();
                public string Species { get; set; } = "";
                public bool Mimic { get; set; }
                public ulong MetamorphosisDays { get; set; }
                public double CaterpillarLength { get; set; }
                public double Wingspan { get; set; }
            }

            // Test Scenario: Applied to Reference-Nested Non-Orderable Scalar (✗impermissible✗)
            public class Cartel {
                public enum CommodityType { Drug, Agriculture, Antique, Fashion, Technology, Energy, Other };

                public class Commodity {
                    [PrimaryKey] public string Name { get; set; } = "";
                    [PrimaryKey] public CommodityType Kind { get; set; }
                    public decimal StreetValue { get; set; }
                    public bool IsIllicit { get; set; }
                }

                [PrimaryKey] public Guid CartelID { get; set; }
                public string? CartelName { get; set; }
                [Check.IsLessThan(CommodityType.Antique, Path = "Kind")] public Commodity Control { get; set; } = new();
                public decimal AnnualRevenue { get; set; }
                public string? Leader { get; set; }
                public uint Members { get; set; }
            }

            // Test Scenario: Applied to Nested Reference (✗impermissible✗)
            public class Hallucination {
                public class Drug {
                    [PrimaryKey] public Guid NarcoID { get; set; }
                    public string Name { get; set; } = "";
                    public bool IsNarcotic { get; set; }
                    public bool IsStimulant { get; set; }
                    public bool IsHallucinogen { get; set; }
                    public double LethalDose { get; set; }
                    public string ChemicalFormula { get; set; } = "";
                }
                public record struct Explanation(Drug? Drug, string? Psychosis, bool Hypnosis);

                [PrimaryKey] public Guid ExperienceID { get; set; }
                public string Hallucinator { get; set; } = "";
                [Check.IsLessThan(0UL, Path = "Drug")] public Explanation Reason { get; set; }
                public bool Fatal { get; set; }
                public double Duration { get; set; }
            }

            // Test Scenario: Original Constraint on Reference-Nested Field (✓not propagated✓)
            public class CVE {
                public class Product {
                    [PrimaryKey] public string Name { get; set; } = "";
                    [PrimaryKey, Check.IsLessThan("3199-11-05")] public DateTime Debut { get; set; }
                    public string Vendor { get; set; } = "";
                    public string URL { get; set; } = "";
                }

                [PrimaryKey] public string Key { get; set; } = "";
                public DateTime Disclosed { get; set; }
                public byte Severity { get; set; }
                public Product AffectedSoftware { get; set; } = new();
                public bool Patched { get; set; }
                public string DiscoveredBy { get; set; } = "";
            }

            // Test Scenario: Applied to Relation-Nested Orderable Scalar (✓constrained✓)
            public class FairyGodparent {
                public enum Color { Red, Green, Orange, Blue, Yellow, Pink, Black, White, Purple, Gray }

                [PrimaryKey] public string Name { get; set; } = "";
                public string Godchild { get; set; } = "";
                public ulong WishesGranted { get; set; }
                public Color HairColor { get; set; }
                public Color EyeColor { get; set; }
                [Check.IsLessThan("Warmonger", Path = "FairyGodparent.Name"), Check.IsLessThan((ushort)1851, Path = "Key"), Check.IsLessThan((ushort)42144, Path = "Value")] public RelationMap<ushort, ushort> LinesByEpisode { get; set; } = new();
                public uint TimesDaRulesBroken { get; set; }
            }

            // Test Scenario: Applied to Relation-Nested Non-Orderable Scalar (✗impermissible✗)
            public class NavalBlockade {
                public enum AquaKind { Ocean, River, Stream, Lake, Estuary, Swamp, Inlet, Bay }
                public record struct Waterway(string Name, AquaKind Kind);

                [PrimaryKey] public Guid ID { get; set; }
                public DateTime Instituted { get; set; }
                public DateTime? Lifted { get; set; }
                [Check.IsLessThan(AquaKind.Swamp, Path = "Item.Kind")] public RelationSet<Waterway> WaterwaysAffected { get; set; } = new();
                public decimal EconomicImpact { get; set; }
                public bool ActOfWar { get; set; }
                public ushort NumShips { get; set; }
            }

            // Test Scenario: Applied to Nested Relation (✗impermissible✗)
            public class Blacksmith {
                public record struct Inventory(RelationSet<string> Metals, RelationSet<string> Hammers);

                [PrimaryKey] public Guid InternationalBlacksmithNumber { get; set; }
                public string Address { get; set; } = "";
                public DateTime Birthdate { get; set; }
                [Check.IsLessThan(1000, Path = "Hammers")] public Inventory Materials { get; set; }
                public RelationMap<string, decimal> Prices { get; set; } = new();
                public double SlagPerDay { get; set; }
                public ushort BenchPress { get; set; }
            }

            // Test Scenario: Applied to Nullable Fields with Total Orders (✓constrained✓)
            public class AutoRacetrack {
                [PrimaryKey] public string Name { get; set; } = "";
                [Check.IsLessThan("Zytrotzko")] public string? Nickname { get; set; }
                [Check.IsLessThan(12000000L)] public long? TrackLength { get; set; }
                public byte FIAGrade { get; set; }
                public double TopRecordedSpeed { get; set; }
                public double LapRecord { get; set; }
                [Check.IsLessThan("4319-02-21")] public DateTime? LastRace { get; set; }
            }

            // Test Scenario: Inconvertible Non-`null` Anchor (✗invalid✗)
            public class Distribution {
                [PrimaryKey] public string Title { get; set; } = "";
                public double Mean { get; set; }
                public double Median { get; set; }
                [Check.IsLessThan("Zero")] public double Mode { get; set; }
                public string PDF { get; set; } = "";
                public string CDF { get; set; } = "";
            }

            // Test Scenario: Convertible Non-`null` Anchor (✗invalid✗)
            public class WebBrowser {
                [PrimaryKey] public string Name { get; set; } = "";
                public bool IsChromiumBased { get; set; }
                public string LatestRelease { get; set; } = "";
                [Check.IsLessThan(100)] public float MarketShare { get; set; }
                public uint HTML5Score { get; set; }
            }

            // Test Scenario: Single-Element Array Anchor (✗invalid✗)
            public class GrammaticalCase {
                [PrimaryKey] public string Case { get; set; } = "";
                public string Abbreviation { get; set; } = "";
                public bool PresentInEnglish { get; set; }
                public bool PresentInLatin { get; set; }
                [Check.IsLessThan(new[] { 'z' })] public char? Affix { get; set; }
            }

            // Test Scenario: `null` Anchor (✗invalid✗)
            public class PowerPointAnimation {
                [PrimaryKey] public string File { get; set; } = "";
                [PrimaryKey] public ushort Slide { get; set; }
                [PrimaryKey] public string ObjectName { get; set; } = "";
                [PrimaryKey] public string Trigger { get; set; } = "";
                [Check.IsLessThan(null!)] public double? Duration { get; set; }
                public bool IsOnEntry { get; set; }
                public bool IsOnExit { get; set; }
                public bool IsEmphasis { get; set; }
            }

            // Test Scenario: Anchor is Minimum Value (✗unsatisfiable✗)
            public class StrategoPiece {
                [PrimaryKey] public string Title { get; set; } = "";
                public int CountPerSide { get; set; }
                [Check.IsLessThan(uint.MinValue)] public uint Value { get; set; }
                public bool IsFlag { get; set; }
                public bool IsBomb { get; set; }
            }

            // Test Scenario: Decimal Anchor is Not a Double (✗invalid✗)
            public class Toothpaste {
                [PrimaryKey] public string Brand { get; set; } = "";
                [PrimaryKey] public string Flavor { get; set; } = "";
                [Check.IsLessThan("100%")] public decimal Efficacy { get; set; }
                public bool KidFriendly { get; set; }
                public bool ContainsFluoride { get; set; }
            }

            // Test Scenario: Decimal Anchor is Out-of-Range (✗invalid✗)
            public class Census {
                [PrimaryKey] public ushort Year { get; set; }
                public ulong Population { get; set; }
                public decimal PercentCaucasian { get; set; }
                public decimal PercentBlack { get; set; }
                public decimal PercentNative { get; set; }
                public decimal PercentAsian { get; set; }
                public decimal PercentMiddleEastern { get; set; }
                [Check.IsLessThan(double.MaxValue)] public decimal PercentIndian { get; set; }
                public decimal PercentOther { get; set; }
            }

            // Test Scenario: DateTime Anchor is Not a String (✗invalid✗)
            public class NobelPrize {
                [PrimaryKey] public string Category { get; set; } = "";
                [PrimaryKey, Check.IsLessThan((sbyte)37)] public DateTime Awarded { get; set; }
                [PrimaryKey] public string Recipient { get; set; } = "";
                public string? Contribution { get; set; }
                public bool Accepted { get; set; }
            }

            // Test Scenario: DateTime Anchor is Improperly Formatted (✗invalid✗)
            public class Shogunate {
                [PrimaryKey] public string BakufuName { get; set; } = "";
                public string FirstShogun { get; set; } = "";
                public string FirstEmperor { get; set; } = "";
                [Check.IsLessThan("Wednesday, August 18, 1988")] public DateTime Established { get; set; }
                public DateTime? Ended { get; set; }
            }

            // Test Scenario: DateTime Anchor is Out-of-Range (✗invalid✗)
            public class ISOStandard {
                [PrimaryKey] public uint Number { get; set; }
                public string Title { get; set; } = "";
                [Check.IsLessThan("1735-02-48")] public DateTime Adopted { get; set; }
            }

            // Test Scenario: Anchor of Source Type on Data-Converted Property (✗invalid✗)
            public class Artiodactyl {
                public string Family { get; set; } = "";
                [PrimaryKey] public string Genus { get; set; } = "";
                [PrimaryKey] public string Species { get; set; } = "";
                public string? CommonName { get; set; }
                [DataConverter(typeof(ToInt<byte>)), Check.IsLessThan((byte)8)] public byte NumToes { get; set; }
            }

            // Test Scenario: Anchor of Target Type on Data-Converted Property (✓valid✓)
            public class Phobia {
                [PrimaryKey] public Guid DSMID { get; set; }
                public string FullName { get; set; } = "";
                public string FearOf { get; set; } = "";
                [DataConverter(typeof(ToString<double>)), Check.IsLessThan("100.00001")] public double Prevalence { get; set; }
            }

            // Test Scenario: Scalar Property Constrained Multiple Times (✓minimized✓)
            public class CinemaSins {
                [PrimaryKey] public string URL { get; set; } = "";
                public string Movie { get; set; } = "";
                [Check.IsLessThan(1712312389UL), Check.IsLessThan(18231247121293UL)] public ulong SinCount { get; set; }
                public string Sentence { get; set; } = "";
                public bool PatreonExclusive { get; set; }
            }

            // Test Scenario: <Path> is `null` (✗illegal✗)
            public class BaseballBat {
                [PrimaryKey] public Guid ID { get; set; }
                public double Length { get; set; }
                [Check.IsLessThan(-3.041, Path = null!)] public double Weight { get; set; }
                public bool IsCorked { get; set; }
                public string Material { get; set; } = "";
            }

            // Test Scenario: <Path> on Scalar Does Not Exist (✗non-existent path✗)
            public class Potato {
                [PrimaryKey] public int GlobaPotatoIdentificationNumber { get; set; }
                [Check.IsLessThan(1.0f, Path = "---")] public float Weight { get; set; }
                public string Preparation { get; set; } = "";
                public string Genus { get; set; } = "";
                public string Species { get; set; } = "";
            }

            // Test Scenario: <Path> on Aggregate Does Not Exist (✗non-existent path✗)
            public class SurgicalMask {
                public enum Hue { White, Black, Red, Green, Yellow, Blue, Orange, Purple, Pink, Gray }
                public record struct ProductInfo(Guid AmazonID, ulong BarCode);

                [PrimaryKey] public ulong Auto { get; set; }
                [Check.IsLessThan((sbyte)-111, Path = "---")] public ProductInfo ID { get; set; }
                public string Type { get; set; } = "";
                public Hue Color { get; set; }
                public double MinimumEfficiency { get; set; }
                public bool CovidApproved { get; set; }
            }

            // Test Scenario: <Path> on Aggregate Not Specified (✗missing path✗)
            public class SecretSociety {
                public record struct Activity(string Description, ushort Length, bool InTheNude);

                [PrimaryKey] public string Name { get; set; } = "";
                public ulong? WorldwideMembership { get; set; }
                public DateTime? Founded { get; set; }
                public bool? Active { get; set; }
                public bool IsStillSecret { get; set; }
                [Check.IsLessThan(false)] public Activity Initiation { get; set; }
            }

            // Test Scenario: <Path> on Reference Does Not Exist (✗non-existent path✗)
            public class NationalMonument {
                public enum Party { Democrat, Republican, DemocraticRepublican, Whig, Federalist, Independent, Green, Populist };

                public class President {
                    [PrimaryKey] public byte Index { get; set; }
                    public string FirstName { get; set; } = "";
                    public string LastName { get; set; } = "";
                    public Party PoliticalParty { get; set; }
                    public DateTime TermBegin { get; set; }
                    public DateTime TermEnd { get; set; }
                    public bool Assassinated { get; set; }
                    public bool DiedInOffice { get; set; }
                }

                [PrimaryKey] public ulong HistoricPlaceReferenceNumber { get; set; }
                public string Name { get; set; } = "";
                public double Area { get; set; }
                public DateTime Established { get; set; }
                [Check.IsLessThan("Roosevelt", Path = "---")] public President EstablishedBy { get; set; } = new();
                public ulong AnnualVisitors { get; set; }
            }

            // Test Scenario: <Path> on Reference Refers to Non-Primary-Key Field (✗non-existent path✗)
            public class YogaPosition {
                public class SanskritWord {
                    [PrimaryKey] public string Translation { get; set; } = "";
                    public string Sanskrit { get; set; } = "";
                    public string IAST { get; set; } = "";
                }

                [PrimaryKey] public string Name { get; set; } = "";
                [Check.IsLessThan('6', Path = "Sanskrit")] public SanskritWord SanskritName { get; set; } = new();
                public bool StandingPose { get; set; }
                public uint MinimumAge { get; set; }
                public DateTime? FirstAttestation { get; set; }
            }

            // Test Scenario: <Path> on Reference Not Specified (✗missing path✗)
            public class PubCrawl {
                public class Pub {
                    [PrimaryKey] public string Name { get; set; } = "";
                    public ulong LiquorLicenseNumber { get; set; }
                    public uint NumMenuItems { get; set; }
                    public decimal Revenue { get; set; }
                    public uint MaxCapacity { get; set; }
                    public string? HeadBartender { get; set; }
                }

                [PrimaryKey] public Guid PubCrawlID { get; set; }
                public string City { get; set; } = "";
                [Check.IsLessThan(true)] public Pub FirstPub { get; set; } = new();
                public Pub LastPub { get; set; } = new();
                public ushort NumPubs { get; set; }
            }

            // Test Scenario: <Path> on Relation Does Not Exist (✗non-existent path✗)
            public class Mime {
                public record struct Performance(DateTime Date, string Venue, ushort Attendance, byte Length);

                [PrimaryKey] public Guid MimeID { get; set; }
                public string FullName { get; set; } = "";
                public DateTime Birthdate { get; set; }
                [Check.IsLessThan(0L, Path = "---")] public RelationList<Performance> Performances { get; set; } = new();
                public ulong LifetimeWordsSpoken { get; set; }
                public string GoToMiming { get; set; } = "";
            }

            // Test Scenario: <Path> on Relation Refers to Non-Primary-Key Field of Anchor Entity (✗non-existent path✗)
            public class CatholicCardinal {
                [PrimaryKey] public string FirstName { get; set; } = "";
                [PrimaryKey] public string LastName { get; set; } = "";
                public DateTime Birthdate { get; set; }
                public DateTime? DeathDate { get; set; }
                public DateTime Elevation { get; set; }
                [Check.IsLessThan("2657-03-19", Path = "CatholicCardinal.DeathDate")] public RelationSet<DateTime> Conclaves { get; set; } = new();
                public bool PreviouslyArchbishop { get; set; }
            }

            // Test Scenario: <Path> on Relation Not Specified (✗missing path✗)
            public class Hemalurgy {
                [PrimaryKey] public string Metal { get; set; } = "";
                [Check.IsLessThan("Allomantic Powers")] public RelationSet<string> Steals { get; set; } = new();
                public bool IsDepictedInBooks { get; set; }
            }

            // Test Scenario: Default Value Does Not Satisfy Constraint (✗contradiction✗)
            public class ParkingGarage {
                [PrimaryKey] public Guid GarageID { get; set; }
                public ushort ParkingSpaces { get; set; }
                public ushort Levels { get; set; }
                [Check.IsLessThan(10.00), Default(15.00)] public double CostPerHour { get; set; }
                public bool AllowsOvernight { get; set; }
            }

            // Test Scenario: Originally Valid Default Value No Longer Satisfies Constraint (✗contradiction✗)
            public class ContactLens {
                public struct HexQuad {
                    public byte R { get; set; }
                    public byte G { get; set; }
                    [Default((byte)197)] public byte B { get; set; }
                    public byte A { get; set; }
                }

                [PrimaryKey] public Guid ProductID { get; set; }
                public bool IsRight { get; set; }
                [Check.IsLessThan((byte)101, Path = "B")] public HexQuad Color { get; set; }
                public double Perscription { get; set; }
                public bool IsHardLens { get; set; }
                public bool IsOrthokeratology { get; set; }
            }
        }

        public static class IsGreaterOrEqualTo {
            // Test Scenario: Applied to Numeric Fields (✓constrained✓)
            public class Geyser {
                [PrimaryKey] public string Name { get; set; } = "";
                [Check.IsGreaterOrEqualTo(0L)] public long EruptionHeight { get; set; }
                [Check.IsGreaterOrEqualTo(0f)] public float Elevation { get; set; }
                [Check.IsGreaterOrEqualTo(0U)] public uint EruptionDuration { get; set; }
                public string? NationalParkHome { get; set; }
            }

            // Test Scenario: Applied to Textual Fields (✓constrained✓)
            public class Hotel {
                [PrimaryKey] public Guid HotelID { get; set; }
                [Check.IsGreaterOrEqualTo("")] public string HotelName { get; set; } = "";
                [Check.IsGreaterOrEqualTo('1')] public char Stars { get; set; }
                public ushort NumFloors { get; set; }
                public ushort NumRooms { get; set; }
                public bool ContinentalBreakfast { get; set; }
            }

            // Test Scenario: Applied to Boolean Field (✗impermissible✗)
            public class Steak {
                [PrimaryKey] public Guid SteakID { get; set; }
                public float Temperature { get; set; }
                public string Cut { get; set; } = "";
                [Check.IsGreaterOrEqualTo(false)] public bool FromSteakhouse { get; set; }
                public double Weight { get; set; }
            }

            // Test Scenario: Applied to Decimal Field (✓constrained✓)
            public class ETF {
                [PrimaryKey] public string Symbol { get; set; } = "";
                public DateTime FirstPosted { get; set; }
                [Check.IsGreaterOrEqualTo(-18.412006)] public decimal ClosingPrice { get; set; }
                public string TopConstituent { get; set; } = "";
                public string OfferingOrganization { get; set; } = "";
            }

            // Test Scenario: Applied to DateTime Field (✓constrained✓)
            public class PEP {
                [PrimaryKey] public int Number { get; set; }
                public string URL { get; set; } = "";
                public string Title { get; set; } = "";
                public ushort NumNewKeywords { get; set; }
                [Check.IsGreaterOrEqualTo("1887-04-29")] public DateTime CreatedOn { get; set; }
                public bool IsActive { get; set; }
            }

            // Test Scenario: Applied to Guid Field (✗impermissible✗)
            public class CoatOfArms {
                [PrimaryKey, Check.IsGreaterOrEqualTo("40dd8027-cb92-4c1d-a036-ce3e9fc2bdaa")] public Guid ID { get; set; }
                public string Background { get; set; } = "";
                public string Foreground { get; set; } = "";
                public string Tressure { get; set; } = "";
            }

            // Test Scenario: Applied to Enumeration Field (✗impermissible✗)
            public class CivCityState {
                public enum Category { Cultural, Scientific, Economic, Militaristic, Industrial, Religious }

                [PrimaryKey] public string Name { get; set; } = "";
                [Check.IsGreaterOrEqualTo(Category.Cultural)] public Category Type { get; set; }
                public ulong RealWorldPopulation { get; set; }
                public string Introduced { get; set; } = "";
                public string SuzerainBonus { get; set; } = "";
            }

            // Test Scenario: Applied to Aggregate-Nested Orderable Scalar (✓constrained✓)
            public class FamilyTree {
                public enum Gender { Male, Female, NonBinary, GenderFluid, Other }
                public enum Direction { TopDown, BottomUp, LeftToRight, RightToLeft, Radial }
                public record struct Person(string FirstName, string LastName, Gender Gender, DateTime DOB);

                [PrimaryKey] public Guid GenealogicalID { get; set; }
                public Direction Orientation { get; set; }
                [Check.IsGreaterOrEqualTo("Tony", Path = "FirstName"), Check.IsGreaterOrEqualTo("1255-09-18", Path = "DOB")] public Person Focal { get; set; }
                public uint AncestralGenerations { get; set; }
                public uint DescendantGenerations { get; set; }
                public uint TotalIndividuals { get; set; }
            }

            // Test Scenario: Applied to Aggregate-Nested Non-Orderable Scalar (✗impermissible✗)
            public class Readymade {
                public record struct Entry(uint CopyrightNumber, bool IsFormallyRegistered);

                [PrimaryKey] public Guid ArtworkID { get; set; }
                public string Title { get; set; } = "";
                public bool ByMarcelDuchamp { get; set; }
                [Check.IsGreaterOrEqualTo(true, Path = "IsFormallyRegistered")] public Entry Registration { get; set; }
                public string PrimaryObject { get; set; } = "";
                public decimal Valuation { get; set; }
            }

            // Test Scenario: Applied to Nested Aggregate (✗impermissible✗)
            public class FitnessCenter {
                public enum Dir { Nort, South, East, West }
                public record struct Street(Dir? Direction, string Name, string StreetType);
                public record struct BuildingAddress(uint Number, Street Street, string Polity, ulong ZipCode);

                [PrimaryKey] public Guid DeedID { get; set; }
                [Check.IsGreaterOrEqualTo('0', Path = "Street")] public BuildingAddress Address { get; set; }
                public decimal MembershipCost { get; set; }
                public ulong MembershipCount { get; set; }
                public int NumTreadmills { get; set; }
                public int NumWeightMachines { get; set; }
                public int NumDumbbells { get; set; }
                public bool HasPool { get; set; }
            }

            // Test Scenario: Applied to Reference-Nested Orderable Scalar (✓constrained✓)
            public class CandyBar {
                public class ManufacturingPlant {
                    [PrimaryKey] public string Name { get; set; } = "";
                    public Guid LicenseNumber { get; set; }
                    public uint Employees { get; set; }
                    public bool KosherCertfied { get; set; }
                    public string State { get; set; } = "";
                }

                [PrimaryKey] public Guid SerialNumber { get; set; }
                public string Brand { get; set; } = "";
                [Check.IsGreaterOrEqualTo("Kraft-Heinz 87", Path = "Name")] public ManufacturingPlant Plant { get; set; } = new();
                public bool ContainsChocolate { get; set; }
                public bool ContainsNuts { get; set; }
                public ulong Calories { get; set; }
            }

            // Test Scenario: Applied to Reference-Nested Non-Orderable Scalar (✗impermissible✗)
            public class SlumberParty {
                public enum CardinalDirection { None = 0, North, South, East, West }
                public enum RoadType { Street, Avenue, Boulevard, Circle, Path, Terrace, Way, Junction, Road, Route }

                public class Address {
                    [PrimaryKey] public uint HouseNumber { get; set; }
                    [PrimaryKey] public CardinalDirection Direction { get; set; }
                    [PrimaryKey] public string StreetName { get; set; } = "";
                    [PrimaryKey] public RoadType StreetSuffix { get; set; }
                    [PrimaryKey] public string City { get; set; } = "";
                    [PrimaryKey] public string State { get; set; } = "";
                    [PrimaryKey] public uint ZipCode { get; set; }
                }

                [PrimaryKey] public DateTime Date { get; set; }
                [PrimaryKey] public string HostFamily { get; set; } = "";
                [Check.IsGreaterOrEqualTo(RoadType.Boulevard, Path = "StreetSuffix")] public Address Place { get; set; } = new();
                public sbyte Attendees { get; set; }
                public bool IsCoed { get; set; }
            }

            // Test Scenario: Applied to Nested Reference (✗impermissible✗)
            public class Barbie {
                public class Ken {
                    [PrimaryKey] public Guid SerialNumber { get; set; }
                    public string Adjective { get; set; } = "";
                    public float Height { get; set; }
                }
                public record struct BF(Ken Ken);
                public record struct Family(BF Boyfriend, string? Mother, string? Father, int NumChildren);

                [PrimaryKey] public Guid SerialNumber { get; set; }
                public string Adjective { get; set; } = "";
                public DateTime Released { get; set; }
                public float Height { get; set; }
                [Check.IsGreaterOrEqualTo(11.3f, Path = "Boyfriend.Ken")] public Family Relationships { get; set; } = new();
                public bool AppearedInMovie { get; set; }
            }

            // Test Scenario: Original Constraint on Reference-Nested Field (✓not propagated✓)
            public class Spa {
                public enum MassageType { Acupressure, Shiatsu, DeepTissue, Erotic, Reflexology, Sports, Other }

                public class Masseuse {
                    [PrimaryKey, Check.IsGreaterOrEqualTo(100000000L)] public long SSN { get; set; }
                    public string Name { get; set; } = "";
                    public DateTime DOB { get; set; }
                    public MassageType Specialty { get; set; }
                    public double Rating { get; set; }
                }

                [PrimaryKey] public Guid SpaID { get; set; }
                public string Country { get; set; } = "";
                public ulong NumHotPools { get; set; }
                public bool IsNatural { get; set; }
                public Masseuse HeadMassageTherapist { get; set; } = new();
                public bool ISBWACertified { get; set; }
            }

            // Test Scenario: Applied to Relation-Nested Orderable Scalar (✓constrained✓)
            public class PuppetShow {
                public enum Kind { Hand, Finger, Sock, Muppet, Shadow, Marionette }
                public record struct Puppet(string Name, string Shape, decimal Value, Kind Kind);

                [PrimaryKey] public Guid ID { get; set; }
                public string ShowTitle { get; set; } = "";
                public double ShowLength { get; set; }
                [Check.IsGreaterOrEqualTo("Elmo", Path = "Item.Name"), Check.IsGreaterOrEqualTo(22.5, Path = "Item.Value")] public RelationSet<Puppet> Puppets { get; set; } = new();
                public decimal TicketPrice { get; set; }
                public ushort Attendees { get; set; }
            }

            // Test Scenario: Applied to Relation-Nested Non-Orderable Scalar (✗impermissible✗)
            public class Horcrux {
                public record struct Pair(DateTime Hidden, bool Discovered);

                [PrimaryKey] public string Creator { get; set; } = "";
                [PrimaryKey] public string Victim { get; set; } = "";
                public bool AvadaKedavra { get; set; }
                public string Object { get; set; } = "";
                [Check.IsGreaterOrEqualTo(false, Path = "Value.Discovered")] public RelationMap<string, Pair> HidingPlaces { get; set; } = new();
            }

            // Test Scenario: Applied to Nested Relation (✗impermissible✗)
            public class WheresWaldo {
                public record struct Coordinate(float X, float Y);
                public record struct Quadrant(byte Number, RelationList<Coordinate> Decoys);

                [PrimaryKey] public Guid PuzzleID { get; set; }
                public string PuzzleTheme { get; set; } = "";
                public Coordinate Waldo { get; set; }
                public Quadrant Q1 { get; set; }
                [Check.IsGreaterOrEqualTo(153.962f, Path = "Decoys")] public Quadrant Q2 { get; set; }
                public Quadrant Q3 { get; set; }
                public Quadrant Q4 { get; set; }
            }

            // Test Scenario: Applied to Nullable Fields with Total Orders (✓constrained✓)
            public class Muscle {
                [PrimaryKey] public uint FMAID { get; set; }
                [Check.IsGreaterOrEqualTo(10U)] public uint? TA2 { get; set; }
                public string Name { get; set; } = "";
                [Check.IsGreaterOrEqualTo("~~~")] public string? Nerve { get; set; }
                public bool IsFlexor { get; set; }
                public bool IsExtensor { get; set; }
                [Check.IsGreaterOrEqualTo("937-12-18")] public DateTime? FirstDocumented { get; set; }
            }

            // Test Scenario: Inconvertible Non-`null` Anchor (✗invalid✗)
            public class LandCard {
                [PrimaryKey] public Guid CardID { get; set; }
                public byte WhiteManna { get; set; }
                public byte GreenManna { get; set; }
                [Check.IsGreaterOrEqualTo("None")] public byte BlueManna { get; set; }
                public byte RedManna { get; set; }
                public byte BlackManna { get; set; }
                public byte UncoloredManna { get; set; }
                public bool EntersTapped { get; set; }
                public string ActivatedAbilities { get; set; } = "";
            }

            // Test Scenario: Convertible Non-`null` Anchor (✗invalid✗)
            public class Keystroke {
                [PrimaryKey] public byte Key { get; set; }
                public bool Shift { get; set; }
                public bool Control { get; set; }
                public bool Alt { get; set; }
                public bool Function { get; set; }
                public string Description { get; set; } = "";
                [Check.IsGreaterOrEqualTo(290)] public char? ResultingGlyph { get; set; }
            }

            // Test Scenario: Single-Element Array Anchor (✗invalid✗)
            public class Zoo {
                [PrimaryKey] public string City { get; set; } = "";
                [PrimaryKey] public string Name { get; set; } = "";
                public ushort AnimalPopulation { get; set; }
                [Check.IsGreaterOrEqualTo(new[] { 4.2213f })] public float AverageVisitorsPerDay { get; set; }
                public bool AZA { get; set; }
            }

            // Test Scenario: `null` Anchor (✗invalid✗)
            public class Neurotoxin {
                [PrimaryKey] public string Name { get; set; } = "";
                public string ChemicalFormula { get; set; } = "";
                public string? Abbreviation { get; set; }
                [Check.IsGreaterOrEqualTo(null!)] public double MolarMass { get; set; }
                public string Inhibits { get; set; } = "";
            }

            // Test Scenario: Anchor is Minimum Value (✓redundant✓)
            public class Bacterium {
                [PrimaryKey] public string Genus { get; set; } = "";
                [PrimaryKey] public string Species { get; set; } = "";
                public bool GramPositive { get; set; }
                [Check.IsGreaterOrEqualTo(ushort.MinValue)] public ushort NumStrains { get; set; }
                public string Shape { get; set; } = "";
            }

            // Test Scenario: Decimal Anchor is Not a Double (✗invalid✗)
            public class GitHook {
                [PrimaryKey] public string Event { get; set; } = "";
                [Check.IsGreaterOrEqualTo(1.0f)] public decimal NumExecutions { get; set; }
                public bool Enabled { get; set; }
                public bool IsClientSide { get; set; }
            }

            // Test Scenario: Decimal Anchor is Out-of-Range (✗invalid✗)
            public class RubeGoldbergMachine {
                [PrimaryKey] public string Identifier { get; set; } = "";
                [Check.IsGreaterOrEqualTo(double.NegativeInfinity)] public decimal MaterialsCost { get; set; }
                public ushort TimeToComplete { get; set; }
                public string Accomplishment { get; set; } = "";
                public uint NumMarbles { get; set; }
                public uint NumCans { get; set; }
                public uint NumDominoes { get; set; }
            }

            // Test Scenario: DateTime Anchor is Not a String (✗invalid✗)
            public class Smurf {
                [PrimaryKey] public string Name { get; set; } = "";
                public bool IsFemale { get; set; }
                [Check.IsGreaterOrEqualTo(318.909f)] public DateTime FirstIntroduced { get; set; }
                public int Appearances { get; set; }
            }

            // Test Scenario: DateTime Anchor is Improperly Formatted (✗invalid✗)
            public class WorldCup {
                [PrimaryKey] public DateTime FirstMatch { get; set; }
                [Check.IsGreaterOrEqualTo("1111(11)11")] public DateTime ChampionshipDate { get; set; }
                public string Champion { get; set; } = "";
                public string RunnerUp { get; set; } = "";
                public string BallonDor { get; set; } = "";
                public ushort TotalGoalsScored { get; set; }
                public ushort TotalYellowCards { get; set; }
                public ushort TotalRedCards { get; set; }
            }

            // Test Scenario: DateTime Anchor is Out-of-Range (✗invalid✗)
            public class SharkTankPitch {
                [PrimaryKey] public string Company { get; set; } = "";
                [PrimaryKey, Check.IsGreaterOrEqualTo("91237-00-16")] public DateTime AirDate { get; set; }
                public string? HookedShark { get; set; }
                public decimal AskAmount { get; set; }
                public double Stake { get; set; }
            }

            // Test Scenario: Anchor of Source Type on Data-Converted Property (✗invalid✗)
            public class Mushroom {
                [PrimaryKey] public string Genus { get; set; } = "";
                [PrimaryKey] public string Species { get; set; } = "";
                public string? CommonName { get; set; }
                public string? PsychedelicID { get; set; }
                public bool IsPoisonousToHumans { get; set; }
                [DataConverter(typeof(ToInt<double>)), Check.IsGreaterOrEqualTo(-18.0933)] public double AverageWeight { get; set; }
            }

            // Test Scenario: Anchor of Target Type on Data-Converted Property (✓valid✓)
            public class EMail {
                [PrimaryKey] public string Sender { get; set; } = "";
                [PrimaryKey] public string Recipient { get; set; } = "";
                [PrimaryKey] public DateTime Sent { get; set; }
                public string Subject { get; set; } = "";
                [DataConverter(typeof(ToInt<string>)), Check.IsGreaterOrEqualTo(73)] public string CC { get; set; } = "";
                public string BCC { get; set; } = "";
                public bool IsJunk { get; set; }
            }

            // Test Scenario: Scalar Property Constrained Multiple Times (✓maximized✓)
            public class SolarEclipse {
                [PrimaryKey] public DateTime Start { get; set; }
                [PrimaryKey] public DateTime End { get; set; }
                public double Magnitude { get; set; }
                public double Gamma { get; set; }
                [Check.IsGreaterOrEqualTo(3), Check.IsGreaterOrEqualTo(-5)] public int SarosCycle { get; set; }
            }

            // Test Scenario: <Path> is `null` (✗illegal✗)
            public class YuGiOhMonster {
                [PrimaryKey] public Guid MonsterID { get; set; }
                public string MonsterName { get; set; } = "";
                public byte Level { get; set; }
                [Check.IsGreaterOrEqualTo((ushort)13, Path = null!)] public ushort Attack { get; set; }
                public ushort Defense { get; set; }
                public string Attribute { get; set; } = "";
            }

            // Test Scenario: <Path> on Scalar Does Not Exist (✗non-existent path✗)
            public class Hieroglyph {
                [PrimaryKey] public ulong UnicodeValue { get; set; }
                [Check.IsGreaterOrEqualTo("?", Path = "---")] public string Glyph { get; set; } = "";
                public string Name { get; set; } = "";
                public bool IsDeterminative { get; set; }
                public string Semantic { get; set; } = "";
            }

            // Test Scenario: <Path> on Aggregate Does Not Exist (✗non-existent path✗)
            public class Pagoda {
                public enum Religion { Buddhism, Taoism, Lay }
                public record struct Coordinate(float Latitude, float Longitude);

                [PrimaryKey] public Guid PagodaID { get; set; }
                [Check.IsGreaterOrEqualTo(57.133f, Path = "---")] public Coordinate Location { get; set; }
                public Religion Tradition { get; set; }
                public ushort NumEaves { get; set; }
                public bool IsActive { get; set; }
                public ulong Height { get; set; }
            }

            // Test Scenario: <Path> on Aggregate Not Specified (✗missing path✗)
            public class Motorcycle {
                public enum Type { Standard, Cruiser, Touring, Sport, OffRoad, Scooter, Moped }
                public record struct Wheel(double Diameter, string Material, ushort NumSpokes);

                [PrimaryKey] public string LicensePlate { get; set; } = "";
                public string Brand { get; set; } = "";
                public Type Kind { get; set; }
                public double Horsepower { get; set; }
                [Check.IsGreaterOrEqualTo("1588-07-07")] public Wheel Wheels { get; set; }
                public decimal MarketValue { get; set; }
                public ushort TopSpeed { get; set; }
                public bool HasKickstand { get; set; }
            }

            // Test Scenario: <Path> on Reference Does Not Exist (✗non-existent path✗)
            public class Druid {
                public enum Sizing { Tiny, Small, Medium, Large, Huge, Gargantuan }
                public record struct Stats(byte Wisdom, byte Strength, byte Charisma, byte Constitution, byte Intelligence, byte Dexterity);

                public class WildShape {
                    [PrimaryKey] public string Creature { get; set; } = "";
                    public ushort PHBPage { get; set; }
                    public Sizing Size { get; set; }
                    public string Attack1 { get; set; } = "";
                    public string Attack2 { get; set; } = "";
                    public string? Attack3 { get; set; }
                    public Stats AbilityScores { get; set; }
                }

                [PrimaryKey] public string CharacterName { get; set; } = "";
                public ushort Level { get; set; }
                public Stats AbilityScores { get; set; }
                public string FavoriteCantrip { get; set; } = "";
                public WildShape WildShape1 { get; set; } = new();
                [Check.IsGreaterOrEqualTo((byte)3, Path = "---")] public WildShape WildShape2 { get; set; } = new();
                public WildShape WildShape3 { get; set; } = new();
                public WildShape? WildShape4 { get; set; }
                public WildShape? WildShape5 { get; set; }
            }

            // Test Scenario: <Path> on Reference Refers to Non-Primary-Key Field (✗non-existent path✗)
            public class Mirror {
                public class Shape {
                    [PrimaryKey] public string GeometricName { get; set; } = "";
                    public ushort Vertices { get; set; }
                    public ushort Sides { get; set; }
                    public bool IsRegular { get; set; }
                    public bool IsConcave { get; set; }
                }

                [PrimaryKey] public Guid MirrorID { get; set; }
                [Check.IsGreaterOrEqualTo((ushort)1, Path = "Sides")] public Shape MirrorShape { get; set; } = new();
                public double Reflectivity { get; set; }
            }

            // Test Scenario: <Path> on Reference Not Specified (✗missing path✗)
            public class Chromosome {
                public class Gene {
                    [PrimaryKey] public string UniProt { get; set; } = "";
                    public string Ensembl { get; set; } = "";
                    public string Name { get; set; } = "";
                }

                [PrimaryKey] public string Species { get; set; } = "";
                [PrimaryKey] public byte ChromosomeNumber { get; set; }
                public ulong BasePairs { get; set; }
                public float Size { get; set; }
                [Check.IsGreaterOrEqualTo("2017-03-11")] public Gene FirstIsolatedGene { get; set; } = new();
            }

            // Test Scenario: <Path> on Relation Does Not Exist (✗non-existent path✗)
            public class HighlanderImmortal {
                [PrimaryKey] public string Name { get; set; } = "";
                public DateTime FirstDeath { get; set; }
                public double Quickening { get; set; }
                [Check.IsGreaterOrEqualTo("MacLeod", Path = "---")] public RelationSet<Guid> Swords { get; set; } = new();
                public double Height { get; set; }
            }

            // Test Scenario: <Path> on Relation Refers to Non-Primary-Key Field of Anchor Entity (✗non-existent path✗)
            public class Synagogue {
                public enum Judaism { Reform, Conservative, Orthodox, Chabad, Kabbalah, Reconstructionist }

                [PrimaryKey] public Guid ID { get; set; }
                public Judaism Denomination { get; set; }
                public ulong SquareFootage { get; set; }
                [Check.IsGreaterOrEqualTo("%999u$", Path = "Synagogue.Denomination")] public RelationSet<string> Congregants { get; set; } = new();
                public DateTime NeirTamidInstalled { get; set; }
                public uint BneiMitzvot { get; set; }
                public string SeniorRabbi { get; set; } = "";
            }

            // Test Scenario: <Path> on Relation Not Specified (✗missing path✗)
            public class Panegyric {
                [PrimaryKey] public string Speaker { get; set; } = "";
                [PrimaryKey] public DateTime Delivered { get; set; }
                public string Topic { get; set; } = "";
                public bool Religious { get; set; }
                [Check.IsGreaterOrEqualTo(68174)] public RelationMap<int, string> Lines { get; set; } = new();
                public bool Recorded { get; set; }
            }

            // Test Scenario: Default Value Does Not Satisfy Constraint (✗contradiction✗)
            public class Camera {
                [PrimaryKey] public string Model { get; set; } = "";
                public double Aperture { get; set; }
                [Check.IsGreaterOrEqualTo(1.3f), Default(0.00001f)] public float ShutterSpeed { get; set; }
                public double LensRadius { get; set; }
                public bool HasFlash { get; set; }
            }

            // Test Scenario: Originally Valid Default Value No Longer Satisfies Constraint (✗contradiction✗)
            public class SlapBet {
                public struct Terms {
                    public DateTime Evaluation { get; set; }
                    [Default(1)] public int Slaps { get; set; }
                    public string Alternative { get; set; }
                }

                [PrimaryKey] public string Bettor1 { get; set; } = "";
                [PrimaryKey] public string Bettor2 { get; set; } = "";
                [PrimaryKey] public DateTime BetDate { get; set; }
                [Check.IsGreaterOrEqualTo(3, Path = "Slaps")] public Terms Wager { get; set; }
            }
        }

        public static class IsLessOrEqualTo {
            // Test Scenario: Applied to Numeric Fields (✓constrained✓)
            public class Fjord {
                [PrimaryKey] public string Name { get; set; } = "";
                [Check.IsLessOrEqualTo(90f)] public float Latitude { get; set; }
                [Check.IsLessOrEqualTo(90f)] public float Longitude { get; set; }
                [Check.IsLessOrEqualTo(100000UL)] public ulong Length { get; set; }
                [Check.IsLessOrEqualTo((short)6723)] public short Width { get; set; }
            }

            // Test Scenario: Applied to Textual Fields (✓constrained✓)
            public class ExcelRange {
                [PrimaryKey, Check.IsLessOrEqualTo('Z')] public char StartColumn { get; set; }
                public ulong StartRow { get; set; }
                [PrimaryKey, Check.IsLessOrEqualTo("XFD")] public string EndColumn { get; set; } = "";
                public ulong EndRow { get; set; }
            }

            // Test Scenario: Applied to Boolean Field (✗impermissible✗)
            public class TectonicPlate {
                [PrimaryKey] public string Name { get; set; } = "";
                public ulong ApproximateSize { get; set; }
                [Check.IsLessOrEqualTo(true)] public bool OnRingOfFire { get; set; }
                public double Speed { get; set; }
            }

            // Test Scenario: Applied to Decimal Field (✓constrained✓)
            public class Caliphate {
                [PrimaryKey] public string Name { get; set; } = "";
                public string FirstCaliph { get; set; } = "";
                public DateTime Founded { get; set; }
                public DateTime? Crumbled { get; set; }
                [Check.IsLessOrEqualTo(8192481241.412841)] public decimal Population { get; set; }
                public ulong Area { get; set; }
            }

            // Test Scenario: Applied to DateTime Field (✓constrained✓)
            public class Representative {
                [PrimaryKey] public string Name { get; set; } = "";
                public string Party { get; set; } = "";
                public long District { get; set; }
                public string State { get; set; } = "";
                [Check.IsLessOrEqualTo("2688-12-02")] public DateTime FirstElected { get; set; }
            }

            // Test Scenario: Applied to Guid Field (✗impermissible✗)
            public class Sunscreen {
                [PrimaryKey, Check.IsLessOrEqualTo("7242f5a0-e4b2-4834-9485-ec39e4ab8ca4")] public Guid ID { get; set; }
                public string Brand { get; set; } = "";
                public double SPF { get; set; }
                public bool IsNatural { get; set; }
            }

            // Test Scenario: Applied to Enumeration Field (✗impermissible✗)
            public class ConcertTour {
                public enum Type { Solo, Band, Reunion, Jukebox }

                [PrimaryKey] public string Name { get; set; } = "";
                public string Artist { get; set; } = "";
                [Check.IsLessOrEqualTo(Type.Jukebox)] public Type ArtistType { get; set; }
                public string StartCity { get; set; } = "";
                public string EndCity { get; set; } = "";
                public uint NumShows { get; set; }
                public DateTime StartDate { get; set; }
                public DateTime EndDate { get; set; }
                public decimal Gross { get; set; }
            }

            // Test Scenario: Applied to Aggregate-Nested Orderable Scalar (✓constrained✓)
            public class Hominin {
                public record struct BinomialNomenclature(string Genus, string Species);

                [Check.IsLessOrEqualTo("Zubeia", Path = "Genus"), Check.IsLessOrEqualTo("Zynectico", Path = "Genus")] public BinomialNomenclature Species { get; set; }
                [PrimaryKey] public string CommonName { get; set; } = "";
                public DateTime Discovered { get; set; }
                public double CranialVolume { get; set; }
                public double AverageHeight { get; set; }
                public float PercentExtantDNA { get; set; }
            }

            // Test Scenario: Applied to Aggregate-Nested Non-Orderable Scalar (✗impermissible✗)
            public class AmazonService {
                public enum SubscriptionType { Free, Monthly, Yearly, PerUse, PerHour, Discretionary }
                public record struct Subscription(bool RequiresSubscription, SubscriptionType Type, decimal AverageCost);

                [PrimaryKey] public string Name { get; set; } = "";
                public string URL { get; set; } = "";
                public bool PoweredByAI { get; set; }
                [Check.IsLessOrEqualTo(SubscriptionType.PerUse, Path = "Type")] public Subscription Plan { get; set; }
                public ulong MonthlyUsers { get; set; }
                public DateTime Launched { get; set; }
            }

            // Test Scenario: Applied to Nested Aggregate (✗impermissible✗)
            public class Shampoo {
                [Flags] public enum Use { AsShampoo = 1, AsBodyWash = 2, AsConditioner = 4 }
                public record struct AgeRange(uint MinAge, uint MaxAge);
                public record struct Instruction(double StorageTemp, AgeRange Ages, string IfIngested);

                [PrimaryKey] public Guid ProductID { get; set; }
                public Use Usage { get; set; }
                public string? Scent { get; set; }
                public bool ForWomen { get; set; }
                [Check.IsLessOrEqualTo((byte)100, Path = "Ages")] public Instruction Directions { get; set; }
            }

            // Test Scenario: Applied to Reference-Nested Orderable Scalar (✓constrained✓)
            public class Gatorade {
                public class BottlingPlant {
                    [PrimaryKey] public Guid PlantID { get; set; }
                    [PrimaryKey] public DateTime Operational { get; set; }
                    public ushort Employees { get; set; }
                    public ulong AnnualVolume { get; set; }
                    public char HealthCode { get; set; }
                }

                [PrimaryKey] public Guid SerialNumber { get; set; }
                public string Flavor { get; set; } = "";
                public double Volume { get; set; }
                [Check.IsLessOrEqualTo("2566-11-15", Path = "Operational")] public BottlingPlant BottledAt { get; set; } = new();
                public ushort NumElectrolytes { get; set; }
            }

            // Test Scenario: Applied to Reference-Nested Non-Orderable Scalar (✗impermissible✗)
            public class Knife {
                public enum Category { Cleaver, Steak, Chefs, Paring, Wilderness, Switchblade, Bread, Butter, Boning }

                public class KnifeCategory {
                    [PrimaryKey] public Category Which { get; set; }
                    public string Listing { get; set; } = "";
                    public ulong WorldwideCount { get; set; }
                }

                [PrimaryKey] public Guid KnifeID { get; set; }
                public double Sharpness { get; set; }
                public bool StainlessSteel { get; set; }
                [Check.IsLessOrEqualTo(Category.Wilderness, Path = "Which")] public KnifeCategory Categorization { get; set; } = new();
                public bool UsedInMurder { get; set; }
            }

            // Test Scenario: Applied to Nested Reference (✗impermissible✗)
            public class Ransomware {
                public class Ransom {
                    [PrimaryKey] public decimal Amount { get; set; }
                    [PrimaryKey] public string Currency { get; set; } = "";
                }
                public record struct Demand(Ransom Ransom, DateTime Deadline);

                [PrimaryKey] public Guid ID { get; set; }
                public string? Claimant { get; set; }
                public ushort MachinesAffected { get; set; }
                [Check.IsLessOrEqualTo('_', Path = "Ransom")] public Demand Extortion { get; set; } = new();
                public decimal Damage { get; set; }
                public bool RansomPaid { get; set; }
            }

            // Test Scenario: Original Constraint on Reference-Nested Field (✓not propagated✓)
            public class WhiteWalker {
                public class Sword {
                    [PrimaryKey, Check.IsLessOrEqualTo("Yearnling")] public string Name { get; set; } = "";
                    public double Length { get; set; }
                    public string AncestralHouse { get; set; } = "";
                    public string Wielder { get; set; } = "";
                    public bool IsValyrianSteel { get; set; }
                }

                [PrimaryKey] public Guid ID { get; set; }
                public Sword? SlainBy { get; set; }
                public string? FirstBookAppearance { get; set; }
                public double? FirstTelevisionAppearance { get; set; }
                public double Height { get; set; }
                public bool IsNightKing { get; set; }
                public ulong WightsCreated { get; set; }
            }

            // Test Scenario: Applied to Relation-Nested Orderable Scalar (✓constrained✓)
            public class Syllabary {
                [PrimaryKey] public string Language { get; set; } = "";
                public DateTime? Created { get; set; }
                public ulong WorldwideUsers { get; set; }
                [Check.IsLessOrEqualTo('|', Path = "Value")] public RelationMap<char, char> IPA { get; set; } = new();
                public bool IsAlphasyllabary { get; set; }
            }

            // Test Scenario: Applied to Relation-Nested Non-Orderable Scalar (✗impermissible✗)
            public class TreehouseOfHorror {
                public enum Role { VoiceActor, VoiceActress, Animator, Writer, Producer, Editor, SoundArist }

                [PrimaryKey] public byte Season { get; set; }
                [PrimaryKey] public byte EpisodeNumber { get; set; }
                public DateTime AirDate { get; set; }
                public bool IsParody { get; set; }
                public RelationMap<string, Role> Cast { get; set; } = new();
                [Check.IsLessOrEqualTo("Robert", Path = "Value")] public RelationMap<string, Role> Crew { get; set; } = new();
                public string Segment1Title { get; set; } = "";
                public string Segment2Title { get; set; } = "";
                public string Segment3Title { get; set; } = "";
                public bool KangAndKodos { get; set; }
            }

            // Test Scenario: Applied to Nested Relation (✗impermissible✗)
            public class CocoaFarm {
                public record struct PersonnelGroup(string Owner, RelationSet<string> Workers, RelationList<string> Regulators);

                [PrimaryKey] public Guid ID { get; set; }
                public uint Area { get; set; }
                public string Country { get; set; } = "";
                public ulong CocoaVolume { get; set; }
                [Check.IsLessOrEqualTo("Nikolai Emunatto", Path = "Regulators")] public PersonnelGroup Personnel { get; set; }
                public decimal Revenue { get; set; }
            }

            // Test Scenario: Applied to Nullable Fields with Total Orders (✓constrained✓)
            public class Subreddit {
                [PrimaryKey] public string Identifier { get; set; } = "";
                public string URL { get; set; } = "";
                [Check.IsLessOrEqualTo("???")] public string? Moderator { get; set; }
                [Check.IsLessOrEqualTo("7771-04-15")] public DateTime? Initiated { get; set; }
                public ulong Posts { get; set; }
                public ulong NetKarma { get; set; }
                public ulong Subscribers { get; set; }
                [Check.IsLessOrEqualTo(47)] public int? TimesQuarantined { get; set; }
            }

            // Test Scenario: Inconvertible Non-`null` Anchor (✗invalid✗)
            public class Dreidel {
                [PrimaryKey] public int IsraeliDreidelIdentificationNumber { get; set; }
                [Check.IsLessOrEqualTo((byte)153)] public string? SerialCode { get; set; }
                public bool ShowsShin { get; set; }
                public float Weight { get; set; }
                public bool MadeOutOfClay { get; set; }
            }

            // Test Scenario: Convertible Non-`null` Anchor (✗invalid✗)
            public class ArthurianKnight {
                [PrimaryKey] public string Name { get; set; } = "";
                public int RoundTableSeatNumber { get; set; }
                public bool TouchedExcalibur { get; set; }
                [Check.IsLessOrEqualTo(4U)] public ulong MalloryMentions { get; set; }
                public bool AppearsInMerlin { get; set; }
            }

            // Test Scenario: Single-Element Array Anchor (✗invalid✗)
            public class Mint {
                [PrimaryKey] public string City { get; set; } = "";
                [PrimaryKey] public string Currency { get; set; } = "";
                [Check.IsLessOrEqualTo(new[] { "1845-08-30" })] public DateTime Established { get; set; }
                public decimal CumulativeValueMinted { get; set; }
                public char Identifier { get; set; }
                public bool Operational { get; set; }
            }

            // Test Scenario: `null` Anchor (✗invalid✗)
            public class VoirDire {
                [PrimaryKey] public DateTime When { get; set; }
                public ushort InitialPoolSize { get; set; }
                public ushort ProsecutionDismissals { get; set; }
                public ushort DefenseDismissals { get; set; }
                [Check.IsLessOrEqualTo(null!)] public ushort BatsonChallenges { get; set; }
            }

            // Test Scenario: Anchor is Maximum Value (✓redundant✓)
            public class ShellCommand {
                [PrimaryKey] public string Command { get; set; } = "";
                public bool PrintsToStdOut { get; set; }
                public long NumArguments { get; set; }
                [Check.IsLessOrEqualTo(long.MaxValue)] public long NumOptions { get; set; }
                public string HelpText { get; set; } = "";
                public string ManText { get; set; } = "";
            }

            // Test Scenario: Decimal Anchor is Not a Double (✗invalid✗)
            public class ChewingGum {
                [PrimaryKey] public string Brand { get; set; } = "";
                [PrimaryKey] public string Flavor { get; set; } = "";
                [Check.IsLessOrEqualTo('(')] public decimal AverageLifetime { get; set; }
                public DateTime Introduced { get; set; }
                public double ConsumerRating { get; set; }
            }

            // Test Scenario: Decimal Anchor is Out-of-Range (✗invalid✗)
            public class Headphones {
                [PrimaryKey] public uint ID { get; set; }
                public string Brand { get; set; } = "";
                [Check.IsLessOrEqualTo(double.PositiveInfinity)] public decimal MaxVolume { get; set; }
                public bool AreEarbuds { get; set; }
                public bool NoiseCancelling { get; set; }
                public bool Waterproof { get; set; }
            }

            // Test Scenario: DateTime Anchor is Not a String (✗invalid✗)
            public class ClockTower {
                [PrimaryKey] public Guid ID { get; set; }
                public ushort Height { get; set; }
                public double ClockRadius { get; set; }
                public byte NumChimesOnTheHour { get; set; }
                [Check.IsLessOrEqualTo(-381723L)] public DateTime Inaugurated { get; set; }
            }

            // Test Scenario: DateTime Anchor is Improperly Formatted (✗invalid✗)
            public class KentuckyDerby {
                [PrimaryKey, Check.IsLessOrEqualTo("2317-04-19 @ 2:00pm")] public DateTime Racetime { get; set; }
                public string Win { get; set; } = "";
                public string Place { get; set; } = "";
                public string Show { get; set; } = "";
                public decimal TotalWagers { get; set; }
                public decimal TotalPayouts { get; set; }
            }

            // Test Scenario: DateTime Anchor is Out-of-Range (✗invalid✗)
            public class Firearm {
                [PrimaryKey] public Guid SerialNumber { get; set; }
                public uint MagazineCapacity { get; set; }
                public float AmmunitionSize { get; set; }
                [Check.IsLessOrEqualTo("1927-03-109")] public DateTime Manufactured { get; set; }
                public ushort NumHomicides { get; set; }
                public bool IsAutomatic { get; set; }
            }

            // Test Scenario: Anchor of Source Type on Data-Converted Property (✗invalid✗)
            public class ShardOfAdonalsium {
                [PrimaryKey] public string Shard { get; set; } = "";
                public string Vessel { get; set; } = "";
                public string Intent { get; set; } = "";
                public string Investiture { get; set; } = "";
                [DataConverter(typeof(ToInt<bool>)), Check.IsLessOrEqualTo(false)] public bool Splintered { get; set; }
            }

            // Test Scenario: Anchor of Target Type on Data-Converted Property (✓valid✓)
            public class HTMLElement {
                [PrimaryKey] public string ID { get; set; } = "";
                public string? Class { get; set; }
                public bool Hidden { get; set; }
                public string? InnerHTML { get; set; }
                public string? InlineStyle { get; set; }
                [DataConverter(typeof(ToString<uint>)), Check.IsLessOrEqualTo("400000")] public uint NumChildren { get; set; }
            }

            // Test Scenario: Scalar Property Constrained Multiple Times (✓minimized✓)
            public class Archbishop {
                [PrimaryKey] public string FirstName { get; set; } = "";
                [PrimaryKey] public string MiddleName { get; set; } = "";
                [PrimaryKey] public string LastName { get; set; } = "";
                [Check.IsLessOrEqualTo("124"), Check.IsLessOrEqualTo("Uppsala")] public string City { get; set; } = "";
                public DateTime Consecrated { get; set; }
                public DateTime Installed { get; set; }
            }

            // Test Scenario: <Path> is `null` (✗illegal✗)
            public class GameOfClue {
                [PrimaryKey] public Guid ID { get; set; }
                public byte NumPlayers { get; set; }
                public string Killer { get; set; } = "";
                [Check.IsLessOrEqualTo("Foyer", Path = null!)] public string CrimeScene { get; set; } = "";
                public string MurderWeapon { get; set; } = "";
            }

            // Test Scenario: <Path> on Scalar Does Not Exist (✗non-existent path✗)
            public class PlaneOfExistence {
                [PrimaryKey, Check.IsLessOrEqualTo("Nether-Plane", Path = "---")] public string Name { get; set; } = "";
                public bool IsElemental { get; set; }
                public bool IsHellPlane { get; set; }
                public bool IsFaeriePlane { get; set; }
            }

            // Test Scenario: <Path> on Aggregate Does Not Exist (✗non-existent path✗)
            public class Mausoleum {
                public record struct Coordinate(float Latitude, float Longitude);
                public record struct Dimension(double Height, double Width, double Length);

                [PrimaryKey] public string Owner { get; set; } = "";
                public byte NumInterred { get; set; }
                public string Cemetery { get; set; } = "";
                [Check.IsLessOrEqualTo(53.2f, Path = "---")] public Coordinate Location { get; set; }
                public Dimension Measurements { get; set; }
            }

            // Test Scenario: <Path> on Aggregate Not Specified (✗missing path✗)
            public class Pseudonym {
                public record struct Name(string Nom, char Separator);

                [PrimaryKey] public Name Moniker { get; set; }
                [Check.IsLessOrEqualTo(']')] public Name For { get; set; }
            }

            // Test Scenario: <Path> on Reference Does Not Exist (✗non-existent path✗)
            public class FoodPantry {
                public class State {
                    [PrimaryKey] public string Abbreviation { get; set; } = "";
                    public string FullName { get; set; } = "";
                    public ulong Population { get; set; }
                    public ulong Area { get; set; }
                    public DateTime Statehood { get; set; }
                    public bool InConfederacy { get; set; }
                }

                [PrimaryKey] public Guid ID { get; set; }
                public string StreetAddress { get; set; } = "";
                [Check.IsLessOrEqualTo(15781293, Path = "---")] public State WhichState { get; set; } = new();
                public string Director { get; set; } = "";
                public ulong NumCans { get; set; }
                public ulong NumPastaBoxes { get; set; }
                public ulong NumCerealBoxes { get; set; }
                public decimal TotalFoodValue { get; set; }
            }

            // Test Scenario: <Path> on Reference Refers to Non-Primary-Key Field (✗non-existent path✗)
            public class FittedSheet {
                public enum Size { King, Queen, Twin, TwinXL, Full, California, Alaskan }

                public class Dimension {
                    [PrimaryKey] public float Length { get; set; }
                    [PrimaryKey] public float Width { get; set; }
                    public short ThreadCount { get; set; }
                }
                public record struct Color(byte R, byte G, byte B);

                [PrimaryKey] public Guid ProductID { get; set; }
                public Size BedSize { get; set; }
                public Color SheetColor { get; set; }
                public ushort AverageFoldingTime { get; set; }
                [Check.IsLessOrEqualTo((short)-531, Path = "ThreadCount")] public Dimension Dimensions { get; set; } = new();
            }

            // Test Scenario: <Path> on Reference Not Specified (✗missing path✗)
            public class Playlist {
                public class Song {
                    [PrimaryKey] public string Title { get; set; } = "";
                    [PrimaryKey] public string Artist { get; set; } = "";
                    public ushort Length { get; set; }
                    public ulong SpotifyStreams { get; set; }
                    public string YouTubeLink { get; set; } = "";
                }

                [PrimaryKey] public string Name { get; set; } = "";
                public ulong NumSongs { get; set; }
                [Check.IsLessOrEqualTo(-0.333f)] public Song MostPlayed { get; set; } = new();
                public ulong TotalDuration { get; set; }
            }

            // Test Scenario: <Path> on Relation Does Not Exist (✗non-existent path✗)
            public class ThumbWar {
                public enum Movement { Up, Down, Left, Right, Wiggle, FlexBack, ForwardPursuit }
                public enum Result { Player1Win, Player2Win, Draw }
                public record struct Play(uint Player, Movement Move);

                [PrimaryKey] public Guid ThumbWarID { get; set; }
                public uint Player1 { get; set; }
                public uint Player2 { get; set; }
                [Check.IsLessOrEqualTo(105, Path = "---")] public RelationList<Play> PlayByPlay { get; set; } = new();
                public Result Outcome { get; set; }
            }

            // Test Scenario: <Path> on Relation Refers to Non-Primary-Key Field of Anchor Entity (✗non-existent path✗)
            public class EngagementRing {
                public enum Stone { Diamond, Ruby, Sapphire, Peridot, Amethyst }
                public record struct Measurement(double Value, string Unit);

                [PrimaryKey] public Guid ID { get; set; }
                public Stone Centerpiece { get; set; }
                [Check.IsLessOrEqualTo(285712905UL, Path = "EngagementRing.Centerpiece")] public RelationMap<string, Measurement> Measurements { get; set; } = new();
                public decimal Price { get; set; }
                public bool FamilyHeirloom { get; set; }
                public string Wearer { get; set; } = "";
            }

            // Test Scenario: <Path> on Relation Not Specified (✗missing path✗)
            public class ImpracticalJoke {
                public enum Joker { Sal, Q, Murr, Joe, Guest }

                [PrimaryKey] public Guid JokeID { get; set; }
                public Joker ImpracticalJoker { get; set; }
                [Check.IsLessOrEqualTo((short)7512)] public RelationSet<string> JokeTargets { get; set; } = new();
                public double ComedyRating { get; set; }
                public double JokeLength { get; set; }
                public bool IsPhysicalComedy { get; set; }
                public string Location { get; set; } = "";
            }

            // Test Scenario: Default Value Does Not Satisfy Constraint (✗contradiction✗)
            public class BowlingFrame {
                [PrimaryKey] public Guid FrameID { get; set; }
                public short Round { get; set; }
                public byte FirstThrowPins { get; set; }
                [Check.IsLessOrEqualTo((byte)10), Default((byte)23)] public byte? SecondThrowPins { get; set; }
            }

            // Test Scenario: Originally Valid Default Value No Longer Satisfies Constraint (✗contradiction✗)
            public class Defenestration {
                public struct Window {
                    public double Height { get; set; }
                    [Default(178.916)] public double Width { get; set; }
                    public bool BoardedUp { get; set; }
                }

                [PrimaryKey] public Guid DefenestrationID { get; set; }
                public DateTime Date { get; set; }
                public string? KnownAs { get; set; }
                public ushort Victims { get; set; }
                [Check.IsLessOrEqualTo(8.9, Path = "Width")] public Window ThrownFrom { get; set; }
            }
        }

        public static class IsNot {
            // Test Scenario: Applied to Numeric Fields (✓constrained✓)
            public class Bridge {
                [PrimaryKey] public int ID { get; set; }
                [Check.IsNot(34)] public int Length { get; set; }
                [Check.IsNot(15UL)] public ulong Height { get; set; }
                [Check.IsNot(0.23776f)] public float Width { get; set; }
                public DateTime Built { get; set; }
                public bool IsOverWater { get; set; }
            }

            // Test Scenario: Applied to Textual Fields (✓constrained✓)
            public class Quatrain {
                [PrimaryKey] public string Title { get; set; } = "";
                [PrimaryKey] public string Author { get; set; } = "";
                [Check.IsNot("Elephant")] public string Line1 { get; set; } = "";
                [Check.IsNot("Giraffe")] public string Line2 { get; set; } = "";
                [Check.IsNot("Crocodile")] public string Line3 { get; set; } = "";
                [Check.IsNot("Rhinoceros")] public string Line4 { get; set; } = "";
                [Check.IsNot('$')] public char FirstLetter { get; set; }
            }

            // Test Scenario: Applied to Boolean Field (✓constrained✓)
            public class PoliceOfficer {
                public string FirstName { get; set; } = "";
                public string LastName { get; set; } = "";
                [PrimaryKey] public string Department { get; set; } = "";
                [PrimaryKey] public ushort BadgeNumber { get; set; }
                [Check.IsNot(false)] public bool IsRetired { get; set; }
            }

            // Test Scenario: Applied to Decimal Field (✓constrained✓)
            public class Therapist {
                [PrimaryKey] public string Name { get; set; } = "";
                public string University { get; set; } = "";
                public string Practice { get; set; } = "";
                public ushort NumPatients { get; set; }
                [Check.IsNot(0.750)] public decimal CostPerHour { get; set; }
                public DateTime NextAvailableAppointment { get; set; }
            }

            // Test Scenario: Applied to DateTime Field (✓constrained✓)
            public class SlotMachine {
                [PrimaryKey] public Guid MachineNumber { get; set; }
                public ulong Jackpot { get; set; }
                public decimal LeverCost { get; set; }
                [Check.IsNot("4431-01-21")] public DateTime InstalledOn { get; set; }
                public ulong NumPlays { get; set; }
            }

            // Test Scenario: Applied to Guid Field (✓constrained✓)
            public class Church {
                [PrimaryKey, Check.IsNot("a3c3ac24-4cf2-428e-a4db-76b30958cc90")] public Guid ChurchID { get; set; }
                public string Name { get; set; } = "";
                public byte NumSpires { get; set; }
                public double Height { get; set; }
                public ushort NumBells { get; set; }
                public DateTime Established { get; set; }
            }

            // Test Scenario: Applied to Enumeration Field (✓constrained✓)
            public class MarianApparition {
                public enum Status { Alleged, Confirmed, Accepted, Recognized, Documented, Ignored }

                [PrimaryKey] public DateTime When { get; set; }
                [PrimaryKey] public string Location { get; set; } = "";
                public string Witnesses { get; set; } = "";
                public string MarianTitle { get; set; } = "";
                [Check.IsNot(Status.Ignored)] public Status Recognition { get; set; }
            }

            // Test Scenario: Applied to Aggregate-Nested Scalar (✓constrained✓)
            public class SeventhInningStretch {
                public record struct MatchUp(string HomeTeam, string AwayTeam, DateTime Date);

                [PrimaryKey] public string BaseballReferenceGameURL { get; set; } = "";
                [Check.IsNot("Savannah Bananas", Path = "AwayTeam"), Check.IsNot("2001-09-11", Path = "Date")] public MatchUp Game { get; set; }
                public string Singer { get; set; } = "";
                public string RootRootRootFor { get; set; } = "";
                public ushort Duration { get; set; }
            }

            // Test Scenario: Applied to Nested Aggregate (✗impermissible✗)
            public class SportsBet {
                public record struct OneDollar(decimal Return);
                public record struct Ratio(bool IsFavored, ulong Number, OneDollar OneDollarPayout);

                [PrimaryKey] public Guid SlipNumber { get; set; }
                public string Bettor { get; set; } = "";
                public string Competition { get; set; } = "";
                public string ExpectedOutcome { get; set; } = "";
                [Check.IsNot(77.44514303f, Path = "OneDollarPayout")] public Ratio Odds { get; set; }
                public bool PlacedOnline { get; set; }
                public bool IsPropBet { get; set; }
            }

            // Test Scenario: Applied to Reference-Nested Scalar (✓constrained✓)
            public class StanleyCup {
                public enum Conf { Eastern, Western, Central }

                public class HockeyTeam {
                    [PrimaryKey] public string Name { get; set; } = "";
                    [PrimaryKey] public string Abbreviation { get; set; } = "";
                    [PrimaryKey] public Conf Conference { get; set; }
                    public string AllTimeLeadingScorer { get; set; } = "";
                    public string Arena { get; set; } = "";
                }

                [PrimaryKey] public ushort Year { get; set; }
                [Check.IsNot("NBC", Path = "Abbreviation")] public HockeyTeam Champion { get; set; } = new();
                [Check.IsNot(Conf.Central, Path = "Conference")] public HockeyTeam RunnerUp { get; set; } = new();
                public ulong TotalGoals { get; set; }
                public ulong TotalPenalties { get; set; }
                public string MVP { get; set; } = "";
            }

            // Test Scenario: Applied to Nested Reference (✗impermissible✗)
            public class FishingRod {
                public enum Style { Fly, CarbonFiber, Tenkara, SpinCast, Baitcast, Spinning, UltraLight, Ice, Telescopic }

                public class Company {
                    [PrimaryKey] public string Name { get; set; } = "";
                    public string CEO { get; set; } = "";
                    public DateTime Founded { get; set; }
                    public decimal Revenue { get; set; }
                    public ulong NumEmployees { get; set; }
                    public ulong NumFactories { get; set; }
                }
                public record struct Info(Company Manufacturer, Guid SerialNumber);

                [PrimaryKey] public Guid UniversalID { get; set; }
                public double Length { get; set; }
                public double Weight { get; set; }
                public ushort NumFishCaught { get; set; }
                [Check.IsNot(false, Path = "Manufacturer")] public Info ManufacturingInfo { get; set; }
                public Style RodType { get; set; }
            }

            // Test Scenario: Original Constraint on Reference-Nested Field (✓not propagated✓)
            public class Diet {
                public class Cookbook {
                    [PrimaryKey, Check.IsNot("471fd196-4866-41b8-b652-41e6bef562b2")] public Guid ID { get; set; }
                    public string Title { get; set; } = "";
                    public string Author { get; set; } = "";
                    public ushort NumPages { get; set; }
                    public ushort NumRecipes { get; set; }
                    public DateTime PublicationDate { get; set; }
                }

                [PrimaryKey] public string Name { get; set; } = "";
                public ulong RecommendedMinCalories { get; set; }
                public ulong RecommendedMaxCalories { get; set; }
                public Cookbook BestSellingCookbook { get; set; } = new();
                public bool CanEatGrains { get; set; }
                public bool CanEatRedMeat { get; set; }
                public bool CanEatFruit { get; set; }
                public bool CanEatVegetables { get; set; }
                public bool CanEatFish { get; set; }
                public bool CanEatDairy { get; set; }
            }

            // Test Scenario: Applied to Relation-Nested Scalar (✓constrained✓)
            public class StandUpComedian {
                public enum Kind { KnockKnock, Observational, WordPlay, HistoricalWhatIf, Political, Other }
                public record struct Joke(Kind Kind, string SetUp, string PunchLine, double LaughCaliber, bool NSFW);

                [PrimaryKey] public string FirstName { get; set; } = "";
                [PrimaryKey] public string LastName { get; set; } = "";
                [Check.IsNot(Kind.Other, Path = "Item.Kind"), Check.IsNot(false, Path = "Item.NSFW")] public RelationList<Joke> Jokes { get; set; } = new();
                public DateTime FirstShow { get; set; }
                public bool LateNightHost { get; set; }
                public ulong TwitterFollowers { get; set; }
            }

            // Test Scenario: Applied to Nested Relation (✗impermissible✗)
            public class Interview {
                public record struct Course(RelationMap<string, double> Questions);

                [PrimaryKey] public Guid ID { get; set; }
                public string Company { get; set; } = "";
                public Guid PositionID { get; set; }
                public string Interviewer { get; set; } = "";
                public string Interviewee { get; set; } = "";
                [Check.IsNot("Traveling Salesman", Path = "Questions")]  public Course Questions { get; set; }
                public bool Pass { get; set; }
            }

            // Test Scenario: Applied to Nullable Fields (✓constrained✓)
            public class Fountain {
                [PrimaryKey] public string FountainName { get; set; } = "";
                [Check.IsNot("926dbe07-875c-46fd-863b-051b98a2d6be")] public Guid? FountainUUID { get; set; }
                [Check.IsNot("1131-08-19")] public DateTime? Unveiled { get; set; }
                public ulong Height { get; set; }
                public ulong Length { get; set; }
                [Check.IsNot(35.22)] public double? Spout { get; set; }
                [Check.IsNot("Play-Doh")] public string? Masonry { get; set; }
                [Check.IsNot(false)] public bool? IsActive { get; set; }
            }

            // Test Scenario: Inconvertible Non-`null` Anchor (✗invalid✗)
            public class Candle {
                [PrimaryKey] public uint ProductID { get; set; }
                public string? Scent { get; set; }
                public float Height { get; set; }
                [Check.IsNot("Wide")] public float Width { get; set; }
                public double AverageBurnTime { get; set; }
            }

            // Test Scenario: Convertible Non-`null` Anchor (✗invalid✗)
            public class CompilerWarning {
                [PrimaryKey] public string WarningID { get; set; } = "";
                public byte Severity { get; set; }
                [Check.IsNot(1)] public bool DebugOnly { get; set; }
                public string VersionIntroduced { get; set; } = "";
                public string WarningText { get; set; } = "";
                public bool IsSuppressed { get; set; }
            }

            // Test Scenario: Single-Element Array Anchor (✗invalid✗)
            public class Alarm {
                [PrimaryKey] public Guid ID { get; set; }
                public byte Hour { get; set; }
                public byte Minute { get; set; }
                [Check.IsNot(new[] { false })] public bool Snoozeable { get; set; }
                public string NotificationSound { get; set; } = "";
            }

            // Test Scenario: `null` Anchor (✗invalid✗)
            public class HallOfFame {
                public enum Category { Sports, Entertainment, Journalism }

                [PrimaryKey] public string For { get; set; } = "";
                public uint Enshrinees { get; set; }
                [Check.IsNot(null!)] public Category Categorization { get; set; }
                public float Latitude { get; set; }
                public float Longitude { get; set; }
                public DateTime Opened { get; set; }
            }

            // Test Scenario: Decimal Anchor is Not a Double (✗invalid✗)
            public class DistrictAttorney {
                [PrimaryKey] public string Name { get; set; } = "";
                [PrimaryKey] public string District { get; set; } = "";
                [PrimaryKey] public ushort Stint { get; set; }
                public DateTime Start { get; set; }
                public DateTime? End { get; set; }
                [Check.IsNot(false)] public decimal ConvictionRate { get; set; }
                public bool WasPublicDefender { get; set; }
            }

            // Test Scenario: Decimal Anchor is Out-of-Range (✗invalid✗)
            public class Ping {
                [PrimaryKey] public DateTime Timestamp { get; set; }
                public string TargetIP { get; set; } = "";
                public string SourceIP { get; set; } = "";
                [Check.IsNot(double.MaxValue - 3.0)] public decimal? RoundTrip { get; set; }
                public bool PacketDropped { get; set; }
            }

            // Test Scenario: DateTime Anchor is Not a String (✗invalid✗)
            public class InsurancePolicy {
                [PrimaryKey] public Guid PolicyID { get; set; }
                [Check.IsNot(-8193.018f)] public DateTime EffectiveAsOf { get; set; }
                public decimal NominalCoverage { get; set; }
                public decimal LiabilityCoverage { get; set; }
                public decimal Deductible { get; set; }
                public string Beneficiary { get; set; } = "";
            }

            // Test Scenario: DateTime Anchor is Improperly Formatted (✗invalid✗)
            public class Mosque {
                public Guid MosqueID { get; set; }
                public string Name { get; set; } = "";
                public byte NumMinarets { get; set; }
                public double Height { get; set; }
                public ulong Capacity { get; set; }
                [Check.IsNot("1.4.5.0.1.0.3.0")] public DateTime Established { get; set; }
            }

            // Test Scenario: DateTime Anchor is Out-of-Range (✗invalid✗)
            public class Lease {
                [PrimaryKey] public string Address { get; set; } = "";
                public uint? Unit { get; set; }
                [Check.IsNot("1637-07-8819")] public DateTime StartDate { get; set; }
                public DateTime EndDate { get; set; }
                public decimal MonthlyRent { get; set; }
                public bool ContainsOptOut { get; set; }
            }

            // Test Scenario: Guid Disallowed Value is Not a String (✗invalid✗)
            public class RainDelay {
                public enum Sport { Baseball, Football, Tennis, Rugby, Soccer, Track, Volleyball, Other }

                [PrimaryKey, Check.IsNot(85819205UL)] public Guid ID { get; set; }
                public DateTime When { get; set; }
                public Sport SportingEvent { get; set; }
                public uint DurationMinutes { get; set; }
                public bool PrePlanned { get; set; }
                public bool Lightning { get; set; }
            }

            // Test Scenario: Guid Disallowed Value is Improperly Formatted (✗invalid✗)
            public class Wiretap {
                public enum Agency { CIA, FBI, NSA, LocalPolice, Military, PrivateSecurity, Other }

                [PrimaryKey, Check.IsNot("This is an INVALID GUID")] public Guid WiretapID { get; set; }
                public string IssuingJudge { get; set; } = "";
                public Agency IssuingAgency { get; set; }
                public bool Active { get; set; }
            }

            // Test Scenario: Anchor of Source Type on Data-Converted Property (✗invalid✗)
            public class FairyTale {
                [PrimaryKey] public string Title { get; set; } = "";
                public string Author { get; set; } = "";
                public string OriginalLanguage { get; set; } = "";
                public bool BrothersGrimm { get; set; }
                [DataConverter(typeof(ToString<bool>)), Check.IsNot(false)] public bool Disneyfied { get; set; }
            }

            // Test Scenario: Anchor of Target Type on Data-Converted Property (✓valid✓)
            public class RingOfPower {
                [PrimaryKey] public string Name { get; set; } = "";
                public string? Holder { get; set; }
                public bool Destroyed { get; set; }
                public DateTime Forged { get; set; }
                public string CentralStone { get; set; } = "";
                [DataConverter(typeof(ToInt<ushort>)), Check.IsNot(7)] public ushort NumPossessors { get; set; }
            }

            // Test Scenario: Scalar Property Constrained Multiple Times with Same Anchor (✓de-duplicated✓)
            public class NazcaLine {
                [PrimaryKey, Check.IsNot("Iguana"), Check.IsNot("Iguana")] public string Name { get; set; } = "";
                public ulong Area { get; set; }
                public float Latitude { get; set; }
                public float Longitude { get; set; }
            }

            // Test Scenario: Scalar Property Constrained Multiple Times with Different Anchors (✓compounded✓)
            public class Pterosaur {
                public string Family { get; set; } = "";
                [PrimaryKey] public string Genus { get; set; } = "";
                [PrimaryKey] public string Species { get; set; } = "";
                public double Wingspan { get; set; }
                [Check.IsNot(0U), Check.IsNot(7894520U)] public uint Specimens { get; set; }
                public bool Toothed { get; set; }
            }

            // Test Scenario: <Path> is `null` (✗illegal✗)
            public class LotteryTicket {
                [PrimaryKey] public Guid Ticket { get; set; }
                public byte N0 { get; set; }
                public byte N1 { get; set; }
                public byte N2 { get; set; }
                public byte N3 { get; set; }
                public byte N4 { get; set; }
                public byte N5 { get; set; }
                [Check.IsNot("2024-02-29", Path = null!)] public DateTime PurchaseTime { get; set; }
            }

            // Test Scenario: <Path> on Scalar Does Not Exist (✗non-existent path✗)
            public class Prison {
                [PrimaryKey] public string Name { get; set; } = "";
                public ulong Capacity { get; set; }
                public ulong Population { get; set; }
                [Check.IsNot(10u, Path = "---")] public uint SecurityLevel { get; set; }
                public DateTime Opened { get; set; }
            }

            // Test Scenario: <Path> on Aggregate Does Not Exist (✗non-existent path✗)
            public class Restaurant {
                [Flags] public enum Culture { American = 1, Chinese = 2, Japanese = 4, Italian = 8, Indian = 16, German = 32, Thai = 64, Korean = 128, Irish = 256, English = 512, Spanish = 1024, Other = 2048 }
                public record struct SaladBarOptions(decimal Price, int MaxReturns, bool AsAppetizer);

                [PrimaryKey] public string Name { get; set; } = "";
                public bool IsChain { get; set; }
                public bool IsFastFood { get; set; }
                public uint NumMenuItems { get; set; }
                [Check.IsNot(true, Path = "---")] public SaladBarOptions? SaladBar { get; set; }
                public bool ServesAlcohol { get; set; }
                public Culture Cuisine { get; set; }
                public decimal Revenue2023 { get; set; }
            }

            // Test Scenario: <Path> on Aggregate Not Specified (✗missing path✗)
            public class Balk {
                [Flags] public enum Base { None = 0, First = 1, Second = 2, Third = 4 }
                public enum Reason { NoStop, DroppedBall, StartedMotion, FakedToFirst, TookSignsOffRubber }
                public record struct Player(string FirstName, string LastName, byte JerseyNumber);

                [PrimaryKey] public Guid BaseballReferencePlayID { get; set; }
                [Check.IsNot("Pablo Sanchez")] public Player Pitcher { get; set; }
                public Base InitialBaserunners { get; set; }
                public Base ResultingBaserunners { get; set; }
                public Reason Call { get; set; }
            }

            // Test Scenario: <Path> on Reference Does Not Exist (✗non-existent path✗)
            public class Planetarium {
                public class Person {
                    [PrimaryKey] public string FirstName { get; set; } = "";
                    [PrimaryKey] public string LastName { get; set; } = "";
                }

                [PrimaryKey] public Guid BuildingID { get; set; }
                public double DomeRadius { get; set; }
                public ulong MaxOccupancy { get; set; }
                public DateTime Built { get; set; }
                [Check.IsNot(100.50f, Path = "---")] public Person Architect { get; set; } = new();
                public bool LargeSynopticSurveyTelescope { get; set; }
            }

            // Test Scenario: <Path> on Reference Refers to Non-Primary-Key Field (✗non-existent path✗)
            public class LiquorStore {
                public enum Color { Red, White, Rose, Other }

                public class Wine {
                    [PrimaryKey] public Guid InternaionalWineRegistry { get; set; }
                    public Color Color { get; set; }
                    public string Vineyard { get; set; } = "";
                    public ushort Year { get; set; }
                }

                [PrimaryKey] public Guid LiquorLicense { get; set; }
                public decimal AnnualSales { get; set; }
                public bool SellsWhiskey { get; set; }
                public bool SellsAbsinthe { get; set; }
                public bool SellsBourbon { get; set; }
                public bool SellsScotch { get; set; }
                public bool SellsBrandy { get; set; }
                [Check.IsNot("Calabrio Farms", Path = "Vineyard")] public Wine? BestSellingWine { get; set; }
                public ushort NumTimesRobbed { get; set; }
            }

            // Test Scenario: <Path> on Reference Not Specified (✗missing path✗)
            public class Waterbending {
                public class Person {
                    [PrimaryKey] public Guid CharacterID { get; set; }
                    public string Name { get; set; } = "";
                    public uint ATLAAppearances { get; set; }
                    public uint LOKAppearances { get; set; }
                    public string VoiceActor { get; set; } = "";
                }

                [PrimaryKey] public string Name { get; set; } = "";
                public int FirstAppearance { get; set; }
                [Check.IsNot("Katara")] public Person StrongestPractitioner { get; set; } = new();
                public bool UsesIce { get; set; }
            }

            // Test Scenario: <Path> on Relation Does Not Exist (✗non-existent path✗)
            public class AutoDaFe {
                [PrimaryKey] public DateTime Date { get; set; }
                public string Sponsor { get; set; } = "";
                public string Religion { get; set; } = "";
                public RelationSet<string> BooksBurned { get; set; } = new();
                public RelationSet<string> PeopleBurned { get; set; } = new();
                [Check.IsNot("Mona Lisa", Path = "---")] public RelationSet<string> ArtworkBurned { get; set; } = new();
            }

            // Test Scenario: <Path> on Relation Refers to Non-Primary-Key Field of Anchor Entity (✗non-existent path✗)
            public class Dream {
                public enum Kind { Fantasy, Daydream, Nightmare, SexDream, NightTerror, Hallucination, Other }

                [PrimaryKey] public string Dreamer { get; set; } = "";
                [PrimaryKey] public DateTime DateOfDream { get; set; }
                [PrimaryKey] public uint SequenceNumber { get; set; }
                public sbyte Length { get; set; }
                [Check.IsNot("Hercules", Path = "Dream.REM")] public RelationList<string> Cameos { get; set; } = new();
                public bool REM { get; set; }
            }

            // Test Scenario: <Path> on Relation Not Specified (✗missing path✗)
            public class BachelorParty {
                public enum Kind { StripClub, Restaurant, SportingEvent, Casino, Home, Beach, Other }
                public record struct Destination(sbyte Order, string Location, decimal AmountSpent, Kind Kind);

                [PrimaryKey] public string Bachelor { get; set; } = "";
                [PrimaryKey] public DateTime Date { get; set; }
                public sbyte Attendees { get; set; }
                [Check.IsNot((byte)121)] public RelationList<Destination> Destinations { get; set; } = new();
                public bool FianceeSanctioned { get; set; }
                public DateTime WeddingDay { get; set; }
            }

            // Test Scenario: Default Value Does Not Satisfy Constraint (✗contradiction✗)
            public class RestStop {
                [PrimaryKey] public string Highway { get; set; } = "";
                [PrimaryKey, Check.IsNot(153U), Default(153U)] public uint Exit { get; set; }
                public bool HasPicnicArea { get; set; }
                public bool IsOasis { get; set; }
                public bool TruckCompatible { get; set; }
            }

            // Test Scenario: Originally Valid Default Value No Longer Satisfies Constraint (✗contradiction✗)
            public class HearthstoneMinion {
                public enum MinionClass { DeathKnight, DemonHunter, Druid, Hunter, Mage, Paladin, Priest, Rogue, Shaman, Warlock, Warrior, Neutral, Dream }
                public enum MinionType { Beast, Demon, Dragon, Elemental, Mech, Murloc, Naga, Pirate, Quilboar, Totem, Undead }

                public struct Stats {
                    public sbyte ManaCost { get; set; }
                    public sbyte Attack { get; set; }
                    [Default((sbyte)-69)] public sbyte Health { get; set; }
                }

                [PrimaryKey] public Guid CardID { get; set; }
                public string Set { get; set; } = "";
                public string Name { get; set; } = "";
                [Check.IsNot((sbyte)-69, Path = "Health")] public Stats Statistics { get; set; }
                public MinionClass Class { get; set; }
                public MinionType Type { get; set; }
            }
        }
    }

    internal static class StringLengthConstraints {
        internal static class IsNonEmpty {
            // Test Scenario: Applied to Non-Nullable String Field (✓constrained✓)
            public class Chocolate {
                [PrimaryKey, Check.IsNonEmpty] public string Name { get; set; } = "";
                public double PercentDark { get; set; }
                public bool AllNatural { get; set; }
            }

            // Test Scenario: Applied to Nullable String Field (✓constrained✓)
            public class Scholarship {
                [PrimaryKey] public Guid ID { get; set; }
                public decimal Amount { get; set; }
                public ulong Submissions { get; set; }
                [Check.IsNonEmpty] public string? Organization { get; set; }
                [Check.IsNonEmpty] public string? TargetSchool { get; set; }
            }

            // Test Scenario: Applied to Numeric Field (✗impermissible✗)
            public class Biography {
                [PrimaryKey] public ulong ISBN { get; set; }
                public string Title { get; set; } = "";
                public string Author { get; set; } = "";
                public string Subject { get; set; } = "";
                [Check.IsNonEmpty] public ushort PageCount { get; set; }
                public ulong WordCount { get; set; }
                public DateTime Published { get; set; }
            }

            // Test Scenario: Applied to Character Field (✗impermissible✗)
            public class MovieTicket {
                [PrimaryKey] public string Theater { get; set; } = "";
                [PrimaryKey] public string Film { get; set; } = "";
                [PrimaryKey] public DateTime Showtime { get; set; }
                [Check.IsNonEmpty] public char Row { get; set; }
                public byte SeatNumber { get; set; }
                public bool OnlineOrder { get; set; }
            }

            // Test Scenario: Applied to Boolean Field (✗impermissible✗)
            public class FortuneCookie {
                [PrimaryKey] public Guid ID { get; set; }
                public string Fortune { get; set; } = "";
                public int LuckyNumber { get; set; }
                [Check.IsNonEmpty] public bool Eaten { get; set; }
            }

            // Test Scenario: Applied to DateTime Field (✗impermissible✗)
            public class ScubaDive {
                [PrimaryKey] public Guid ID { get; set; }
                public string Location { get; set; } = "";
                public bool BoatEntry { get; set; }
                public string Style { get; set; } = "";
                [Check.IsNonEmpty] public DateTime EntryTime { get; set; }
                public DateTime ExitTime { get; set; }
                public double MaxDepth { get; set; }
            }

            // Test Scenario: Applied to Guid Field (✗impermissible✗)
            public class Hormone {
                [PrimaryKey, Check.IsNonEmpty] public Guid HormoneID { get; set; }
                public bool IsSexHormone { get; set; }
                public string ChemicalFormula { get; set; } = "";
                public double ProteinBinding { get; set; }
                public float MolarMass { get; set; }
            }

            // Test Scenario: Applied to Enumeration Field (✗impermissible✗)
            public class Mustache {
                public enum Kind : short { Handlebar, FuManchu, Hitler, Horseshoe, French, Pencil }

                [PrimaryKey] public Guid MustacheID { get; set; }
                public int Age { get; set; }
                [Check.IsNonEmpty] public Kind Style { get; set; }
                public double HairVolume { get; set; }
                public bool Goatee { get; set; }
            }

            // Test Scenario: Applied to Aggregate-Nested String Scalar (✓constrained✓)
            public class BarGraph {
                public record struct Info(string XAxisLabel, string XAxisUnit, string YAxisLabel, string YAxisUnit);

                [PrimaryKey] public string Title { get; set; } = "";
                [Check.IsNonEmpty(Path = "XAxisLabel"), Check.IsNonEmpty(Path = "YAxisLabel")] public Info Legend { get; set; }
                public double MaxValue { get; set; }
                public bool IsStackd { get; set; }
            }

            // Test Scenario: Applied to Aggregate-Nested Non-String Scalar (✗impermissible✗)
            public class BackyardBaseballPlayer {
                public record struct Stats(byte Batting, byte Running, byte Pitching, byte Fielding);

                [PrimaryKey] public string Name { get; set; } = "";
                [PrimaryKey] public ushort Edition { get; set; }
                [Check.IsNonEmpty(Path = "Pitching")] public Stats Statistics { get; set; }
                public string Nickname { get; set; } = "";
                public bool IsFictional { get; set; }
            }

            // Test Scenario: Applied to Nested Aggregate (✗impermissible✗)
            public class OilField {
                public record struct Coordinate(float Latitude, float Longitude);
                public record struct Place(Coordinate Coordinate, string City, string? Subnational, string Country);
                public record struct Location(string? Title, Place Place);

                [PrimaryKey] public Guid OilFieldIdentifier { get; set; }
                public ulong Area { get; set; }
                public ulong AnnualVolume { get; set; }
                public decimal Revenue { get; set; }
                public DateTime FirstTapped { get; set; }
                [Check.IsNonEmpty(Path = "Place.Coordinate")] public Location Where { get; set; }
                public bool IsNationalized { get; set; }
            }

            // Test Scenario: Applied to Reference-Nested String Scalar (✓constrained✓)
            public class VacuumCleaner {
                [Flags] public enum Material { Hardwood = 1, Carpet = 2, Tile = 4, Linoleum = 8 }

                public class Company {
                    [PrimaryKey] public string Name { get; set; } = "";
                    public bool Private { get; set; }
                    public decimal Revenue { get; set; }
                    public string Headquarters { get; set; } = "";
                    public ulong Employees { get; set; }
                }

                [PrimaryKey] public Guid ProductID { get; set; }
                public string Brand { get; set; } = "";
                public string Model { get; set; } = "";
                public double Decibels { get; set; }
                public Material SupportedFloors { get; set; }
                [Check.IsNonEmpty(Path = "Name")] public Company Manufacturer { get; set; } = new();
                public bool IsWireless { get; set; }
            }

            // Test Scenario: Applied to Reference-Nested Non-String Scalar (✗impermissible✗)
            public class Limerick {
                public class Person {
                    [PrimaryKey] public uint SSN { get; set; }
                    public string FirstName { get; set; } = "";
                    public string? MiddleName { get; set; }
                    public string LastName { get; set; } = "";
                }

                [PrimaryKey] public Guid InternationalPoetryIdentificationNumber { get; set; }
                [Check.IsNonEmpty(Path = "SSN")] public Person Author { get; set; } = new();
                public string Line1 { get; set; } = "";
                public string Line2 { get; set; } = "";
                public string Line3 { get; set; } = "";
                public string Line4 { get; set; } = "";
                public string Line5 { get; set; } = "";
                public string? WhereIsTheManFrom { get; set; }
            }

            // Test Scenario: Applied to Nested Reference (✗impermissible✗)
            public class RomanBaths {
                public class Bathroom {
                    [PrimaryKey] public float Latitude { get; set; }
                    [PrimaryKey] public float Longitude { get; set; }
                    public string? Name { get; set; }
                    public DateTime Constructed { get; set; }
                }
                public record struct Daria(Bathroom Frigidarium, Bathroom Caldarium, Bathroom Tepidarium);

                [PrimaryKey] public Guid ArchaeologicalIndex { get; set; }
                public string Emperor { get; set; } = "";
                public double NetVolume { get; set; }
                [Check.IsNonEmpty(Path = "Caldarium")] public Daria Rooms { get; set; }
            }

            // Test Scenario: Original Constraint on Reference-Nested Field (✓not propagated✓)
            public class PornStar {
                public enum Category { Professional, OnlyFans, CamModel, Playboy, Other }
                public enum Identity { Male, Female, Transgender, Genderfluid, Other }

                public class Film {
                    [PrimaryKey, Check.IsNonEmpty] public string Title { get; set; } = "";
                    [PrimaryKey] public DateTime Release { get; set; }
                    public ushort Runtime { get; set; }
                    public bool Hardcore { get; set; }
                }


                [PrimaryKey] public Guid ID { get; set; }
                public string BirthName { get; set; } = "";
                public string ScreenName { get; set; } = "";
                public Category Industry { get; set; }
                public Identity GenderIdentity { get; set; }
                public Film? FirstFilm { get; set; }
                public ulong NumFilms { get; set; }
                public decimal Earnings { get; set; }
                public bool Active { get; set; }
            }

            // Test Scenario: Applied to Relation-Nested String Scalar (✓constrained✓)
            public class Boycott {
                [PrimaryKey] public Guid BoycottID { get; set; }
                public string Target { get; set; } = "";
                public DateTime Started { get; set; }
                public DateTime? Ended { get; set; }
                public ulong Participants { get; set; }
                [Check.IsNonEmpty(Path = "Item")] public RelationSet<string> Sponsors { get; set; } = new();
                public bool BDS { get; set; }
                public decimal Damage { get; set; }
            }

            // Test Scenario: Applied to Relation-Nested Non-String Scalar (✗impermissible✗)
            public class MallSanta {
                public class Mall {
                    [PrimaryKey] public uint MallID { get; set; }
                    public string Address { get; set; } = "";
                    public short NumShops { get; set; }
                    public sbyte NumFloors { get; set; }
                    public ulong SquareFootage { get; set; }
                    public bool HasMovieTheater { get; set; }
                }

                [PrimaryKey] public Guid SantaID { get; set; }
                public string FirstName { get; set; } = "";
                public char MiddleInitial { get; set; }
                public string LastName { get; set; } = "";
                [Check.IsNonEmpty(Path = "Value.MallID")] public RelationMap<ushort, Mall> Jobs { get; set; } = new();
                public ulong TotalKids { get; set; }
                public bool NaturalBeard { get; set; }
            }

            // Test Scenario: Applied to Nested Relation (✗impermissible✗)
            public class ConnectingWall {
                public enum Wall { Lion, Water }
                public record struct Category(uint Color, string Connection, RelationList<string> Squares);

                [PrimaryKey] public sbyte Season { get; set; }
                [PrimaryKey] public sbyte Episode { get; set; }
                [PrimaryKey] public Wall Choice { get; set; }
                public Category C1 { get; set; }
                public Category C2 { get; set; }
                [Check.IsNonEmpty(Path = "Squares")] public Category C3 { get; set; }
                public Category C4 { get; set; }
            }

            // Test Scenario: Applied to Field Data-Converted to String Type (✓constrained✓)
            public class Hourglass {
                [PrimaryKey] public Guid ID { get; set; }
                [DataConverter(typeof(ToString<ushort>)), Check.IsNonEmpty] public ushort Duration { get; set; }
                public double Weight { get; set; }
                public double Height { get; set; }
            }

            // Test Scenario: Applied to Field Data-Converted from String Type (✗impermissible✗)
            public class FoodChain {
                [PrimaryKey] public string Producer { get; set; } = "";
                [PrimaryKey] public string PrimaryConsumer { get; set; } = "";
                [PrimaryKey, DataConverter(typeof(ToInt<string>)), Check.IsNonEmpty] public string SecondaryConsumer { get; set; } = "";
                [PrimaryKey] public string Decomposer { get; set; } = "";
            }

            // Test Scenario: Scalar Property Constrained Multiple Times (✓redundant✓)
            public class Top10List {
                [PrimaryKey] public string Title { get; set; } = "";
                public string Number1 { get; set; } = "";
                public string Number2 { get; set; } = "";
                public string Number3 { get; set; } = "";
                public string Number4 { get; set; } = "";
                public string Number5 { get; set; } = "";
                public string Number6 { get; set; } = "";
                public string Number7 { get; set; } = "";
                public string Number8 { get; set; } = "";
                [Check.IsNonEmpty, Check.IsNonEmpty] public string Number9 { get; set; } = "";
                public string Number10 { get; set; } = "";
            }

            // Test Scenario: <Path> is `null` (✗illegal✗)
            public class Hoedown {
                [PrimaryKey] public uint EpisodeNum { get; set; }
                public string RyanStilesLine { get; set; } = "";
                public string ColinMochrieLine { get; set; } = "";
                [Check.IsNonEmpty(Path = null!)] public string WayneBradyLine { get; set; } = "";
                public string DrewCareyLine { get; set; } = "";
            }

            // Test Scenario: <Path> on Scalar Does Not Exist (✗non-existent path✗)
            public class ASLSign {
                [PrimaryKey, Check.IsNonEmpty(Path = "---")] public string Gloss { get; set; } = "";
                public string Location { get; set; } = "";
                public string Movement { get; set; } = "";
                public string HandShape { get; set; } = "";
                public string PalmOrientation { get; set; } = "";
            }

            // Test Scenario: <Path> on Aggregate Does Not Exist (✗non-existent path✗)
            public class Sutra {
                public enum Group { Buddhism, Hinduism, Jainism, Other }
                public record struct From(string Veda, bool QuotesOnly);

                [PrimaryKey] public string Name { get; set; } = "";
                public ushort WordCount { get; set; }
                public DateTime? Written { get; set; }
                public Group Religion { get; set; }
                [Check.IsNonEmpty(Path = "---")] public From Source { get; set; }
            }

            // Test Scenario: <Path> on Aggregate Not Specified (✗missing path✗)
            public class Kaiju {
                public record struct Measurements(ushort Height, uint Weight, byte NumAppendages);
                public enum Side { Enemy, Ally, Mixed }

                [PrimaryKey] public string Name { get; set; } = "";
                [Check.IsNonEmpty] public Measurements Size { get; set; }
                public Side RelationToGodzilla { get; set; }
                public uint FilmAppearances { get; set; }
            }

            // Test Scenario: <Path> on Aggregate Does Not Exist (✗non-existent path✗)
            public class Peerage {
                public class Title {
                    [PrimaryKey] public string Male { get; set; } = "";
                    [PrimaryKey] public string Female { get; set; } = "";
                    public short Rank { get; set; }
                }

                [PrimaryKey] public string Holder { get; set; } = "";
                [Check.IsNonEmpty(Path = "---")] public Title PeerageTitle { get; set; } = new();
                public bool Hereditary { get; set; }
                public DateTime Obtained { get; set; }
            }

            // Test Scenario: <Path> on Reference Refers to Non-Primary-Key Field (✗non-existent path✗)
            public class BountyHunter {
                public class License {
                    [PrimaryKey] public Guid Number { get; set; }
                    public string IssuingAgency { get; set; } = "";
                    public DateTime IssuedOn { get; set; }
                    public DateTime Expiration { get; set; }
                }

                [PrimaryKey] public string FirstName { get; set; } = "";
                [PrimaryKey] public string LastName { get; set; } = "";
                public double Height { get; set; }
                public double Weight { get; set; }
                public decimal CareerEarnings { get; set; }
                public ushort Captures { get; set; }
                [Check.IsNonEmpty(Path = "IssuingAgency")] public License Credentials { get; set; } = new();
            }

            // Test Scenario: <Path> on Aggregate Not Specified (✗missing path✗)
            public class Linker {
                public class Language {
                    [PrimaryKey] public string Name { get; set; } = "";
                    public ushort Version { get; set; }
                }

                [PrimaryKey] public string BinaryName { get; set; } = "";
                public string Organization { get; set; } = "";
                [Check.IsNonEmpty] public Language TargetLanguage { get; set; } = new();
                public double HelloWorldLinkingDuration { get; set; }
                public bool StandardsCompliant { get; set; }
            }

            // Test Scenario: <Path> on Relation Does Not Exist (✗non-existent path✗)
            public class Nymph {
                public enum Variety { Meliad, Dryad, Naiad, Nereid, Oread }

                [PrimaryKey] public string EnglishName { get; set; } = "";
                public string GreekName { get; set; } = "";
                public Variety Category { get; set; }
                [Check.IsNonEmpty(Path = "---")] public RelationSet<sbyte> MetamorphosesAppearances { get; set; } = new();
                public bool Virgin { get; set; }
                public string? TurnedInto { get; set; }
            }

            // Test Scenario: <Path> on Relation Refers to Non-Primary-Key Field of Anchor Entity (✗non-existent path✗)
            public class DatingApp {
                public record struct Pair(string First, string Second);

                [PrimaryKey] public string AppName { get; set; } = "";
                public string CEO { get; set; } = "";
                public DateTime Launched { get; set; }
                public ulong Users { get; set; }
                [Check.IsNonEmpty(Path = "DatingApp.CEO")] public RelationList<Pair> CouplesFormed { get; set; } = new();
                public bool SwipeBased { get; set; }
                public decimal? MonthlyFee { get; set; }
            }

            // Test Scenario: <Path> on Relation Not Specified (✗missing path✗)
            public class AdBlocker {
                public enum AdType { PopUp, CouponSuggestion, VideoInterrupt, Sponsorships, GoogleAnalytics }

                [PrimaryKey] public string Name { get; set; } = "";
                public ulong Downloads { get; set; }
                public bool Free { get; set; }
                [Check.IsNonEmpty] public RelationSet<AdType> EffectiveAgainst { get; set; } = new();
                public bool Mobile { get; set; }
            }

            // Test Scenario: Default Value Does Not Satisfy Constraint (✗contradiction✗)
            public class AztecGod {
                [PrimaryKey] public string Name { get; set; } = "";
                public string? MayanEquivalent { get; set; }
                [Check.IsNonEmpty, Default("")] public string? Festival { get; set; }
                public string Domain { get; set; } = "";
                public uint CodexAppearances { get; set; }
            }

            // Test Scenario: Originally Valid Default Value No Longer Satisfies Constraint (✗contradiction✗)
            public class Lollipop {
                public enum TasteProfile { Salty, Sweet, Sout, Bitter, Umami }
                public enum Variety { Pinwheel, TootsiePop, ChupaChup, DumDum, Other }

                public struct Flavor {
                    [Default("")] public string Name { get; set; }
                    public TasteProfile Taste { get; set; }
                }

                [PrimaryKey] public Guid ID { get; set; }
                public double Radius { get; set; }
                public Variety Kind { get; set; }
                [Check.IsNonEmpty(Path = "Name")] public Flavor LollipopFlavor { get; set; }
                public ulong LicksRequired { get; set; }
                public ushort Calories { get; set; }
            }
        }

        internal static class LengthIsAtLeast {
            // Test Scenario: Applied to Non-Nullable String Field (✓constrained✓)
            public class NFLPenalty {
                [PrimaryKey, Check.LengthIsAtLeast(5)] public string Penalty { get; set; } = "";
                public bool OnOffense { get; set; }
                public bool OnDefense { get; set; }
                public bool OnSpecialTeams { get; set; }
                public byte Yards { get; set; }
                public bool LossOfDown { get; set; }
            }

            // Test Scenario: Applied to Nullable String Field (✓constrained✓)
            public class Ben10Alien {
                [PrimaryKey] public string Name { get; set; } = "";
                [Check.LengthIsAtLeast(7)] public string? AlternateName { get; set; }
                public string HomePlanet { get; set; } = "";
                public uint OrderOfAddition { get; set; }
                public byte Appearances { get; set; }
                public string FirstEpisode { get; set; } = "";
            }

            // Test Scenario: Applied to Numeric Field (✗impermissible✗)
            public class HashFunction {
                [PrimaryKey] public string Name { get; set; } = "";
                public ushort DigestSize { get; set; }
                [Check.LengthIsAtLeast(7)] public ushort? BlockSize { get; set; }
                public DateTime FirstPublished { get; set; }
                public float CollisionLikelihood { get; set; }
            }

            // Test Scenario: Applied to Character Field (✗impermissible✗)
            public class Kanji {
                [PrimaryKey, Check.LengthIsAtLeast(2)] public char Logograph { get; set; }
                public string Pronunciation { get; set; } = "";
                public char HiraganaEquivalent { get; set; }
                public char KatakanaEquivalent { get; set; }
            }

            // Test Scenario: Applied to Boolean Field (✗impermissible✗)
            public class Magazine {
                [PrimaryKey] public string Name { get; set; } = "";
                public ulong TotalIssues { get; set; }
                [Check.LengthIsAtLeast(430)] public bool Syndicated { get; set; }
                public DateTime FirstPublished { get; set; }
                public string EditorInChief { get; set; } = "";
            }

            // Test Scenario: Applied to DateTime Field (✗impermissible✗)
            public class Camerlengo {
                [PrimaryKey] public string FirstName { get; set; } = "";
                [PrimaryKey] public string LastName { get; set; } = "";
                public string Pope { get; set; } = "";
                [Check.LengthIsAtLeast(13)] public DateTime Appointed { get; set; }
            }

            // Test Scenario: Applied to Guid Field (✗impermissible✗)
            public class Rainforest {
                [PrimaryKey, Check.LengthIsAtLeast(13)] public Guid ID { get; set; }
                public ulong Area { get; set; }
                public ulong NumTrees { get; set; }
                public ulong EndemicSpecies { get; set; }
                public bool IsFederallyProtected { get; set; }
                public float AverageAnnualRainfall { get; set; }
            }

            // Test Scenario: Applied to Enumeration Field (✗impermissible✗)
            public class Cybersite {
                public enum Season { S1, S2, S3, S4, S5, S6, S7, S8, S9, S10, S11, S12, S13, S14 }

                [PrimaryKey] public string Name { get; set; } = "";
                [Check.LengthIsAtLeast(17)] public Season FirstSeasonAppeared { get; set; }
                public bool ControlledByHacker { get; set; }
                public string SkwakopediaEntry { get; set; } = "";
                public uint NumEpisodes { get; set; }
            }

            // Test Scenario: Applied to Aggregate-Nested String Scalar (✓constrained✓)
            public class Dubbing {
                public record struct VoiceOverArtist(string FirstName, char MiddleInitial, string LastName);

                [PrimaryKey] public Guid UUID { get; set; }
                public string Work { get; set; } = "";
                public string Character { get; set; } = "";
                [Check.LengthIsAtLeast(6, Path = "FirstName"), Check.LengthIsAtLeast(2, Path = "LastName")] public VoiceOverArtist Dubber { get; set; }
                public string OriginalLanguage { get; set; } = "";
                public string DubbedLanguage { get; set; } = "";
                public bool IsAnime { get; set; }
            }

            // Test Scenario: Applied to Aggregate-Nested Non-String Scalar (✗impermissible✗)
            public class BaseballMogul {
                public record struct SemVer(ushort Major, ushort Minor, ushort Patch, DateTime Release);

                [PrimaryKey] public Guid VideoGameID { get; set; }
                public ushort SeasonYear { get; set; }
                [Check.LengthIsAtLeast(44, Path = "Patch")] public SemVer Version { get; set; }
                public string Changelog { get; set; } = "";
                public ulong TotalPlayers { get; set; }
                public bool UsesPitchFxData { get; set; }
                public double SimulationSpeed { get; set; }
            }

            // Test Scenario: Applied to Nested Aggregate (✗impermissible✗)
            public class MagicSystem {
                public record struct Law(string Fulfillment);
                public record struct Laws(Law First, Law Second, Law Third, Law? Zeroth);
                public enum Strength { Hard, Soft, Semisoft }

                [PrimaryKey] public string SourceMaterial { get; set; } = "";
                [PrimaryKey] public string Magic { get; set; } = "";
                public Strength HardOrSoft { get; set; }
                [Check.LengthIsAtLeast(1898400, Path = "Zeroth")] public Laws SandersonsLaws { get; set; }
            }

            // Test Scenario: Applied to Reference-Nested String Scalar (✓constrained✓)
            public class TEDTalk {
                public class Person {
                    [PrimaryKey] public string FirstName { get; set; } = "";
                    [PrimaryKey] public string LastName { get; set; } = "";
                    public DateTime Birthday { get; set; }
                }

                [PrimaryKey] public Guid TalkID { get; set; }
                public DateTime Time { get; set; }
                public bool IsTedX { get; set; }
                [Check.LengthIsAtLeast(14, Path = "LastName")] public Person Speaker { get; set; } = new();
                public string TalkTitle { get; set; } = "";
                public double Duration { get; set; }
            }

            // Test Scenario: Applied to Reference-Nested Non-String Scalar (✗impermissible✗)
            public class Arrondissement {
                public class FrenchDepartment {
                    [PrimaryKey] public string Nom { get; set; } = "";
                    [PrimaryKey] public ulong Population { get; set; }
                    public ushort Arrondissements { get; set; }
                    public ulong Area { get; set; }
                }

                [PrimaryKey] public string Nom { get; set; } = "";
                [Check.LengthIsAtLeast(189466, Path = "Population")] public FrenchDepartment Department { get; set; } = new();
                public ushort Communes { get; set; }
                public ulong Population { get; set; }
                public ulong Area { get; set; }
            }

            // Test Scenario: Applied to Nested Reference (✗impermissible✗)
            public class Constellation {
                public class Star {
                    [PrimaryKey] public string Name { get; set; } = "";
                    [PrimaryKey] public uint MessierNumber { get; set; }
                    public double Declination { get; set; }
                    public double ApparentMagnitude { get; set; }
                    public double Distance { get; set; }
                }
                public record struct Asterism(string Name, Star CentralStar, int NumStars, bool NorthernHemisphere);

                [PrimaryKey] public string Name { get; set; } = "";
                public uint NumStars { get; set; }
                public bool InZodiac { get; set; }
                [Check.LengthIsAtLeast(60, Path = "CentralStar")] public Asterism? MainAsterism { get; set; }
                public double Declination { get; set; }
                public string? MeteorShower { get; set; }
            }

            // Test Scenario: Original Constraint on Reference-Nested Field (✓not propagated✓)
            public class Circus {
                public class Circusmaster {
                    [PrimaryKey, Check.LengthIsAtLeast(11)] public string SSN { get; set; } = "";
                    public string Name { get; set; } = "";
                    public DateTime DOB { get; set; }
                    public bool WearsTopHat { get; set; }
                }

                [PrimaryKey] public string Name { get; set; } = "";
                public Circusmaster Master { get; set; } = new();
                public bool IsTraveling { get; set; }
                public ushort NumAnimals { get; set; }
                public ushort NumClowns { get; set; }
                public decimal PriceOfPeanuts { get; set; }
                public byte NumTents { get; set; }
            }

            // Test Scenario: Applied to Relation-Nested String Scalar (✓constrained✓)
            public class UNSecretaryGeneral {
                public record struct Resolution(ushort Number, string Title, double Approval, DateTime Date);

                [PrimaryKey] public string Name { get; set; } = "";
                public DateTime TermBegin { get; set; }
                public DateTime? TermEnd { get; set; }
                [Check.LengthIsAtLeast(6, Path = "Item.Title")] public RelationList<Resolution> ResolutionsPassed { get; set; } = new();
                [Check.LengthIsAtLeast(17, Path = "UNSecretaryGeneral.Name")] public RelationSet<string> CountriesAdmitted { get; set; } = new();
            }

            // Test Scenario: Applied to Relation-Nested Non-String Scalar (✗impermissible✗)
            public class MemoryBuffer {
                [PrimaryKey] public ulong StartAddress { get; set; }
                [PrimaryKey] public ulong EndAddress { get; set; }
                [Check.LengthIsAtLeast(489, Path = "MemoryBuffer.EndAddress")] public RelationList<bool> Bits { get; set; } = new();
                public string IntendedType { get; set; } = "";
                public bool HeapAllocated { get; set; }
            }

            // Test Scenario: Applied to Nested Relation (✗impermissible✗)
            public class HotTub {
                public record struct Settings(RelationMap<string, int> PresetSpeeds, double DefaultTemperature, bool Bubbles);

                [PrimaryKey] public Guid ProductID { get; set; }
                public bool IsJacuzzi { get; set; }
                public double TopTemperature { get; set; }
                public double Volume { get; set; }
                [Check.LengthIsAtLeast(33313, Path = "PresetSpeeds")] public Settings TubSettings { get; set; }
                public sbyte MaxOccupancy { get; set; }
            }

            // Test Scenario: Applied to Field Data-Converted to String Type (✓constrained✓)
            public class Ambassador {
                [PrimaryKey] public string Who { get; set; } = "";
                public string From { get; set; } = "";
                public string To { get; set; } = "";
                [DataConverter(typeof(ToString<DateTime>)), Check.LengthIsAtLeast(10)] public DateTime Assumed { get; set; }
                public DateTime Terminated { get; set; }
            }

            // Test Scenario: Applied to Field Data-Converted from String Type (✗impermissible✗)
            public class Campfire {
                [PrimaryKey] public Guid GUID { get; set; }
                public DateTime Started { get; set; }
                public DateTime? Fizzled { get; set; }
                public double Temperature { get; set; }
                [DataConverter(typeof(ToInt<string>)), Check.LengthIsAtLeast(4)] public string WoodType { get; set; } = "";
            }

            // Test Scenario: Anchor is 0 (✓redundant✓)
            public class HolyRomanEmperor {
                [PrimaryKey] public DateTime ReignBegin { get; set; }
                [PrimaryKey] public DateTime ReignEnd { get; set; }
                [Check.LengthIsAtLeast(0)] public string Name { get; set; } = "";
                public string RoyalHouse { get; set; } = "";
            }

            // Test Scenario: Anchor is Negative (✗illegal✗)
            public class LaborOfHeracles {
                [PrimaryKey] public ushort Order { get; set; }
                [Check.LengthIsAtLeast(-144)] public string Target { get; set; } = "";
                public bool WasExtra { get; set; }
                public bool TargetToBeKilled { get; set; }
            }

            // Test Scenario: Scalar Property Constrained Multiple Times (✓maximized✓)
            public class Bagel {
                [PrimaryKey, Check.LengthIsAtLeast(17), Check.LengthIsAtLeast(34)] public string Flavor { get; set; } = "";
                [PrimaryKey] public bool Toasted { get; set; }
                [PrimaryKey] public string Schmear { get; set; } = "";
                public decimal Cost { get; set; }
            }

            // Test Scenario: <Path> is `null` (✗illegal✗)
            public class Localization {
                [PrimaryKey] public string LocalizationKey { get; set; } = "";
                [PrimaryKey] public uint Locale { get; set; }
                [Check.LengthIsAtLeast(4, Path = null!)] public string LocalizedValue { get; set; } = "";
            }

            // Test Scenario: <Path> on Scalar Does Not Exist (✗non-existent path✗)
            public class Histogram {
                [PrimaryKey] public Guid ID { get; set; }
                public long MinBucket { get; set; }
                public long MaxBucket { get; set; }
                public long BucketSize { get; set; }
                [Check.LengthIsAtLeast(2, Path = "---")] public string BucketUnit { get; set; } = "";
            }

            // Test Scenario: <Path> on Aggregate Does Not Exist (✗non-existent path✗)
            public class Cactus {
                [Flags] public enum Desert { Sahara = 1, Kalahari = 2, Mojave = 4, Gobi = 8, Negev = 16, Namib = 32, Atacama = 64, Sonoran = 128, Arabian = 256 }
                public record struct Taxonomy(string Genus, string Species);

                [Check.LengthIsAtLeast(5, Path = "---")] public Taxonomy ScientificName { get; set; }
                [PrimaryKey] public string CommonName { get; set; } = "";
                public Desert NativeHabitat { get; set; }
                public double AverageHeight { get; set; }
                public uint Lifespan { get; set; }
                public bool Prickly { get; set; }
                public uint WaterRetention { get; set; }
            }

            // Test Scenario: <Path> on Aggregate Not Specified (✗missing path✗)
            public class SederPlate {
                public record struct Slot(string OccupiedBy);

                [PrimaryKey] public Guid ID { get; set; }
                public Slot Charoset { get; set; }
                public Slot Beitzah { get; set; }
                public Slot Maror { get; set; }
                public Slot Zroa { get; set; }
                [Check.LengthIsAtLeast(100)] public Slot Karpas { get; set; }
                public Slot Matzah { get; set; }
                public Slot? Tapuz { get; set; }
            }

            // Test Scenario: <Path> on Aggregate Does Not Exist (✗non-existent path✗)
            public class Crusade {
                public class Ruler {
                    [PrimaryKey] public string RegnalName { get; set; } = "";
                    [PrimaryKey] public uint RegnalNumber { get; set; }
                    public DateTime ReignBegin { get; set; }
                    public DateTime ReignEnd { get; set; }
                    public string Polity { get; set; } = "";
                }

                [PrimaryKey] public byte Index { get; set; }
                public Ruler CrusadersLeader { get; set; } = new();
                [Check.LengthIsAtLeast(16, Path = "---")] public Ruler MuslimLeader { get; set; } = new();
                public Ruler HolyLandLeader { get; set; } = new();
                public DateTime Begin { get; set; }
                public bool CrusadersClaimJerusalem { get; set; }
                public ulong Casualties { get; set; }
            }

            // Test Scenario: <Path> on Reference Refers to Non-Primary-Key Field (✗non-existent path✗)
            public class StateOfTheUnion {
                public class Secretary {
                    [PrimaryKey] public string FirstName { get; set; } = "";
                    [PrimaryKey] public string LastName { get; set; } = "";
                    public DateTime Confirmed { get; set; }
                    public string Department { get; set; } = "";
                    public double ConfirmationPcnt { get; set; }
                }

                [PrimaryKey] public ushort Year { get; set; }
                public string President { get; set; } = "";
                public ulong Length { get; set; }
                public ushort ApplauseBreaks { get; set; }
                [Check.LengthIsAtLeast(31, Path = "Department")] public Secretary DesignatedSurvivor { get; set; } = new();
            }

            // Test Scenario: <Path> on Aggregate Not Specified (✗missing path✗)
            public class Triptych {
                public class Panel {
                    [PrimaryKey] public string Title { get; set; } = "";
                    public double Length { get; set; }
                    public double Height { get; set; }
                    public sbyte NumPeopleDepicted { get; set; }
                }

                [PrimaryKey] public string Title { get; set; } = "";
                public string Artist { get; set; } = "";
                public DateTime Completed { get; set; }
                public Panel LeftPanel { get; set; } = new();
                [Check.LengthIsAtLeast(1892400)] public Panel MiddlePanel { get; set; } = new();
                public Panel RightPanel { get; set; } = new();
                public decimal Appraisal { get; set; }
            }

            // Test Scenario: <Path> on Relation Does Not Exist (✗non-existent path✗)
            public class Cigar {
                public class Tobacco {
                    [PrimaryKey] public Guid TobaccoID { get; set; }
                    public string Name { get; set; } = "";
                    public double Carnicogenicity { get; set; }
                }

                [PrimaryKey] public Guid CigarID { get; set; }
                public float Length { get; set; }
                public bool IsCuban { get; set; }
                [Check.LengthIsAtLeast(52, Path = "---")] public RelationMap<Tobacco, double> Contents { get; set; } = new();
            }

            // Test Scenario: <Path> on Relation Refers to Non-Primary-Key Field of Anchor Entity (✗non-existent path✗)
            public class MarijuanaStrain {
                public class Dispensary {
                    [PrimaryKey] public Guid ID { get; set; }
                    public DateTime Opened { get; set; }
                    public string State { get; set; } = "";
                    public decimal Revenue { get; set; }
                }

                [PrimaryKey] public Guid StrainID { get; set; }
                public string StrainName { get; set; } = "";
                public DateTime Created { get; set; }
                [Check.LengthIsAtLeast(4, Path = "MarijuanaStrain.StrainName")] public RelationSet<Dispensary> SoldAt { get; set; } = new();
                public double Addictiveness { get; set; }
            }

            // Test Scenario: <Path> on Relation Not Specified (✗missing path✗)
            public class BankRobber {
                public enum BankType { Physical, Crypto, Train, MonopolyGame }
                public record struct Robbery(string Bank, decimal Haul, BankType TargetType);

                [PrimaryKey] public string Name { get; set; } = "";
                public Guid? FBINumber { get; set; }
                [Check.LengthIsAtLeast(87)] public RelationList<Robbery> Robberies { get; set; } = new();
                public ushort MurdersCommitted { get; set; }
                public bool Incarcerated { get; set; }
            }

            // Test Scenario: Default Value Does Not Satisfy Constraint (✗contradiction✗)
            public class MaskedSinger {
                [PrimaryKey] public uint Season { get; set; }
                [PrimaryKey, Check.LengthIsAtLeast(289), Default("Pelican")] public string Costume { get; set; } = "";
                public string Identity { get; set; } = "";
                public byte SongsPerformed { get; set; }
            }

            // Test Scenario: Originally Valid Default Value No Longer Satisfies Constraint (✗contradiction✗)
            public class Briefcase {
                public struct Hue {
                    public byte R { get; set; }
                    public byte G { get; set; }
                    public byte B { get; set; }
                    [Default("unknown")] public string PantoneName { get; set; }
                }

                [PrimaryKey] public Guid ProductID { get; set; }
                public double Height { get; set; }
                public double Width { get; set; }
                public double Length { get; set; }
                public decimal Price { get; set; }
                [Check.LengthIsAtLeast(15, Path = "PantoneName")] public Hue Color { get; set; }
                public byte NumHinges { get; set; }
            }
        }

        internal static class LengthIsAtMost {
            // Test Scenario: Applied to Non-Nullable String Field (✓constrained✓)
            public class Snake {
                [PrimaryKey, Check.LengthIsAtMost(175)] public string Genus { get; set; } = "";
                [PrimaryKey, Check.LengthIsAtMost(13512)] public string Species { get; set; } = "";
                [Check.LengthIsAtMost(25)] public string CommonName { get; set; } = "";
                public bool IsVenomous { get; set; }
                public double AverageLength { get; set; }
                public double AverageWeight { get; set; }
            }

            // Test Scenario: Applied to Nullable String Field (✓constrained✓)
            public class WinterStorm {
                [PrimaryKey] public DateTime When { get; set; }
                public double Snowfall { get; set; }
                public int LowTemperature { get; set; }
                public float MaxWindSpeed { get; set; }
                [Check.LengthIsAtMost(300)] public string? Name { get; set; }
            }

            // Test Scenario: Applied to Numeric Field (✗impermissible✗)
            public class GasStation {
                [PrimaryKey] public string Address { get; set; } = "";
                public string Company { get; set; } = "";
                public decimal RegularPrice { get; set; }
                public decimal PremiumPrice { get; set; }
                [Check.LengthIsAtMost(221)] public decimal? DeiselPrice { get; set; }
            }

            // Test Scenario: Applied to Character Field (✗impermissible✗)
            public class BinaryTest {
                [PrimaryKey] public double Specificity { get; set; }
                [PrimaryKey] public double Sensitivity { get; set; }
                public char True { get; set; }
                [Check.LengthIsAtMost(5)] public char False { get; set; }
            }

            // Test Scenario: Applied to Boolean Field (✗impermissible✗)
            public class Diamond {
                [PrimaryKey] public Guid SerialNumber { get; set; }
                public ushort Carats { get; set; }
                public double Weight { get; set; }
                public decimal MarketValue { get; set; }
                [Check.LengthIsAtMost(50)] public bool IsBloodDiamond { get; set; }
                public string? CurrentMuseum { get; set; }
            }

            // Test Scenario: Applied to DateTime Field (✗impermissible✗)
            public class Marathon {
                [PrimaryKey, Check.LengthIsAtMost(15)] public DateTime Date { get; set; }
                [PrimaryKey] public string Location { get; set; } = "";
                public double Length { get; set; }
                public string Winner { get; set; } = "";
                public uint Runners { get; set; }
            }

            // Test Scenario: Applied to Guid Field (✗impermissible✗)
            public class CppHeader {
                [PrimaryKey] public Guid HeaderID { get; set; }
                [Check.LengthIsAtMost(100)] public Guid? ModuleID { get; set; }
                public string FilePath { get; set; } = "";
                public string SourceLibrary { get; set; } = "";
                public bool IsGuarded { get; set; }
                public string FullText { get; set; } = "";
                public bool IsPreCompiled { get; set; }
            }

            // Test Scenario: Applied to Enumeration Field (✗impermissible✗)
            public class ComputerVirus {
                public enum Type { Keylogger, Ransomware, Spyware, Exploitation, Other }

                [PrimaryKey] public string VirusName { get; set; } = "";
                public DateTime Created { get; set; }
                public string Creator { get; set; } = "";
                public decimal Damage { get; set; }
                [Check.LengthIsAtMost(50)] public Type Classification { get; set; }
            }

            // Test Scenario: Applied to Aggregate-Nested String Scalar (✓constrained✓)
            public class MafiaFamily {
                public struct Person {
                    public string FirstName { get; set; }
                    [Check.LengthIsAtMost(71)] public string? MiddleName { get; set; }
                }

                [PrimaryKey] public string FamilyName { get; set; } = "";
                public string Mafia { get; set; } = "";
                [Check.LengthIsAtMost(26, Path = "FirstName")] public Person Capo { get; set; }
                public ushort LivingMembers { get; set; }
                public decimal NetWorth { get; set; }
                public ulong NumMurders { get; set; }
            }

            // Test Scenario: Applied to Aggregate-Nested Non-String Scalar (✗impermissible✗)
            public class Kayak {
                public record struct Seat(float Depth, double Radius);

                [PrimaryKey] public Guid KayakID { get; set; }
                public double Length { get; set; }
                public string WoodComposition { get; set; } = "";
                [Check.LengthIsAtMost(1897, Path = "Radius")] public Seat KayakSeat { get; set; }
                public bool OlympicQuality { get; set; }
                public bool IsAlsoCanoe { get; set; }
            }

            // Test Scenario: Applied to Nested Aggregate (✗impermissible✗)
            public class MaddenNFL {
                public enum Position { QB, RB, WR, TE, OT, OG, C, FB, K, P, KR, PR, ST, DE, DT, NT, OLB, ILB, FS, SS, CB, NB }
                public record struct Person(string FirstName, string LastName);
                public record struct Player(Person Name, Position Position, string Team);

                [PrimaryKey] public ushort Year { get; set; }
                public DateTime Released { get; set; }
                [Check.LengthIsAtMost(31, Path = "Name")] public Player CoverPlayer { get; set; }
                public decimal TotalSales { get; set; }
                public double Rating { get; set; }
            }

            // Test Scenario: Applied to Reference-Nested String Scalar (✓constrained✓)
            public class Denarian {
                public class FallenAngel {
                    [PrimaryKey] public string Name { get; set; } = "";
                    public string Title { get; set; } = "";
                    public ulong Age { get; set; }
                }

                [PrimaryKey] public string Name { get; set; } = "";
                public string DemonicForm { get; set; } = "";
                [Check.LengthIsAtMost(673, Path = "Name")] public FallenAngel Fallen { get; set; } = new();
                public DateTime PickedUpCoin { get; set; }
                public string FirstAppearance { get; set; } = "";
                public ulong WordsSpoken { get; set; }
            }

            // Test Scenario: Applied to Reference-Nested Non-String Scalar (✗impermissible✗)
            public class IceCreamSundae {
                public class Flavor {
                    [PrimaryKey] public Guid ID { get; set; }
                    public string Name { get; set; } = "";
                    public string MixIn { get; set; } = "";
                }

                [PrimaryKey] public Guid SundaeID { get; set; }
                public Flavor Scoop1 { get; set; } = new();
                public Flavor Scoop2 { get; set; } = new();
                [Check.LengthIsAtMost(4, Path = "ID")] public Flavor Scoop3 { get; set; } = new();
                public bool IsBananaSplit { get; set; }
                public uint CalorieCount { get; set; }
            }

            // Test Scenario: Applied to Nested Reference (✗impermissible✗)
            public class Orgasm {
                public enum Manner { Oral, Vaginal, Anal, Mammary, Digital, Psychological }

                public class Person {
                    [PrimaryKey] public string GivenName { get; set; } = "";
                    [PrimaryKey] public string Surname { get; set; } = "";
                }
                public record struct Individual(Person Who);

                [PrimaryKey] public DateTime ExactTimestamp { get; set; }
                [Check.LengthIsAtMost(5029, Path = "Who")] public Individual Receiver { get; set; }
                public Manner Method { get; set; }
                public double LiquidVolume { get; set; }
                public bool ResultedInConception { get; set; }
            }

            // Test Scenario: Original Constraint on Reference-Nested Field (✓not propagated✓)
            public class Bust {
                public enum SculptingMaterial { Granite, Marble, Bronze, Steel, Clay, Limestone, Soapstone, Concrete, Other }

                public class Sculptor {
                    [PrimaryKey, Check.LengthIsAtMost(28)] public string FirstName { get; set; } = "";
                    [PrimaryKey, Check.LengthIsAtMost(106)] public string LastName { get; set; } = "";
                    public DateTime DateOfBirth { get; set; }
                    public DateTime? DateOfDeath { get; set; }
                    public string Nationality { get; set; } = "";
                }

                [PrimaryKey] public Guid ArtworkNumber { get; set; }
                public double Weight { get; set; }
                public SculptingMaterial Material { get; set; }
                public string? Depicting { get; set; }
                public bool Completed { get; set; }
                public string? Museum { get; set; } = "";
            }

            // Test Scenario: Applied to Relation-Nested String Scalar (✓constrained✓)
            public class Golem {
                [PrimaryKey] public Guid ID { get; set; }
                public DateTime Created { get; set; }
                [Check.LengthIsAtMost(26, Path = "Item")] public RelationSet<string> Materials { get; set; } = new();
                public float ShemSize { get; set; }
                public string OwningRabbi { get; set; } = "";
                public ulong Weight { get; set; }
            }

            // Test Scenario: Applied to Relation-Nested Non-String Scalar (✗impermissible✗)
            public class BalsamicVinegar {
                public enum Kind { Fruit, Vegetable, Grain, Oil, Dairy, Protein, Legume, Starch, Water, Other }
                public enum Color { Red, White, Rose }
                public record struct Ingredient(string Name, double Grams, ushort Calories, Kind Kind);

                [PrimaryKey] public Guid BottleID { get; set; }
                public string Flavor { get; set; } = "";
                public double Volume { get; set; }
                public DateTime ExpirationDate { get; set; }
                [Check.LengthIsAtMost(255, Path = "Item.Grams")] public RelationSet<Ingredient> Ingredients { get; set; } = new();
                public Color BalsamicColor { get; set; }
                public bool DOP { get; set; }
            }

            // Test Scenario: Applied to Nested Relation (✗impermissible✗)
            public class TerroristOrganization {
                public record struct Record(RelationMap<string, DateTime> Entities);

                [PrimaryKey] public string Name { get; set; } = "";
                public DateTime ActiveSince { get; set; }
                public string Leader { get; set; } = "";
                public ulong Members { get; set; }
                public ulong Victims { get; set; }
                [Check.LengthIsAtMost(53, Path = "Entities")] public Record Recognition { get; set; } = new();
                public string Motto { get; set; } = "";
            }

            // Test Scenario: Applied to Field Data-Converted to String Type (✓constrained✓)
            public class OilSpill {
                [PrimaryKey] public Guid ID { get; set; }
                public DateTime When { get; set; }
                [DataConverter(typeof(ToString<float>)), Check.LengthIsAtMost(14)] public float Volume { get; set; }
                public decimal CleanupCost { get; set; }
                public string? ResponsibleParty { get; set; }
            }

            // Test Scenario: Applied to Field Data-Converted from String Type (✗impermissible✗)
            public class RandomNumberGenerator {
                [PrimaryKey] public Guid ID { get; set; }
                public ulong Seed { get; set; }
                [DataConverter(typeof(ToInt<string>)), Check.LengthIsAtMost(40)] public string Algorithm { get; set; } = "";
                public long FirstValue { get; set; }
                public bool IsCryptographicallySafe { get; set; }
            }

            // Test Scenario: Anchor is 0 (✓constrained✓)
            public class KnockKnockJoke {
                [PrimaryKey, Check.LengthIsAtMost(0)] public string SetUp { get; set; } = "";
                [PrimaryKey, Check.LengthIsAtMost(0)] public string PunchLine { get; set; } = "";
            }

            // Test Scenario: Anchor is Negative (✗illegal✗)
            public class Fraternity {
                [PrimaryKey, Check.LengthIsAtMost(-7)] public string Name { get; set; } = "";
                public DateTime Founded { get; set; }
                public string Motto { get; set; } = "";
                public int ActiveChapters { get; set; }
                public bool IsSocial { get; set; }
                public bool IsProfessional { get; set; }
            }

            // Test Scenario: Scalar Property Constrained Multiple Times (✓minimized✓)
            public class OceanicTrench {
                [PrimaryKey, Check.LengthIsAtMost(187), Check.LengthIsAtMost(60)] public string Location { get; set; } = "";
                public ulong Depth { get; set; }
                public double MaxPressure { get; set; }
            }

            // Test Scenario: <Path> is `null` (✗illegal✗)
            public class Passport {
                [PrimaryKey, Check.LengthIsAtMost(150, Path = null!)] public string PassportNumber { get; set; } = "";
                public string IssuedTo { get; set; } = "";
                public string IssuedBy { get; set; } = "";
                public DateTime IssuedOn { get; set; }
                public DateTime Expiration { get; set; }
                public byte NumStamps { get; set; }
            }

            // Test Scenario: <Path> on Scalar Does Not Exist (✗non-existent path✗)
            public class Nebula {
                [PrimaryKey] public uint MessierNumber { get; set; }
                [Check.LengthIsAtMost(30, Path = "---")] public string Name { get; set; } = "";
                public double DistanceKLY { get; set; }
                public double ApparentMagnitude { get; set; }
                public string? Constellation { get; set; }
            }

            // Test Scenario: <Path> on Aggregate Does Not Exist (✗non-existent path✗)
            public class ImaginaryFriend {
                [Flags] public enum Color { Red = 1, Orange = 2, Yellow = 4, Green = 8, Blue = 16, Purple = 32, White = 64, Black = 128, Gray = 256, Pink = 512 }
                [Flags] public enum Thing { Claws = 1, Horns = 2, OneEye = 4, Spots = 8, Superpowers = 16 }
                public record struct Description(Color Color, double Height, double Weight, Thing Abilities);

                [PrimaryKey] public uint ID { get; set; }
                public string Name { get; set; } = "";
                public string Child { get; set; } = "";
                public bool OnFostersHome { get; set; }
                [Check.LengthIsAtMost(6, Path = "---")] public Description Features { get; set; }
            }

            // Test Scenario: <Path> on Aggregate Not Specified (✗missing path✗)
            public class Newscast {
                public record struct Person(string FirstName, string LastName);
                public record struct Segment(Person Newscaster, string StoryTitle);

                [PrimaryKey] public string Station { get; set; } = "";
                [PrimaryKey] public DateTime AirTime { get; set; }
                public Segment ABLock { get; set; }
                public Segment BBlock { get; set; }
                public Segment CBlock { get; set; }
                public Segment DBlock { get; set; }
                [Check.LengthIsAtMost(83)] public Segment? Sports { get; set; }
                public Segment? Weather { get; set; }
                public Segment? Traffic { get; set; }
                public Segment? LocalInterest { get; set; }
            }

            // Test Scenario: <Path> on Aggregate Does Not Exist (✗non-existent path✗)
            public class PhaseDiagram {
                public class Point {
                    [PrimaryKey] public double PressurePascals { get; set; }
                    [PrimaryKey] public double TemperatureKelvin { get; set; }
                }

                [PrimaryKey] public string Material { get; set; } = "";
                public Point BoilingPoint { get; set; } = new();
                public Point FreezingPoint { get; set; } = new();
                public Point TriplePoint { get; set; } = new();
                [Check.LengthIsAtMost(63, Path = "---")] public Point CriticalPoint { get; set; } = new();
            }

            // Test Scenario: <Path> on Reference Refers to Non-Primary-Key Field (✗non-existent path✗)
            public class Sundial {
                public class Coordinate {
                    [PrimaryKey] public float Latitude { get; set; }
                    [PrimaryKey] public float Longitude { get; set; }
                    public string? Identifier { get; set; }
                }

                [PrimaryKey] public Guid SundialID { get; set; }
                [Check.LengthIsAtMost(171, Path = "Identifier")] public Coordinate CenterLocation { get; set; } = new();
                public string? Constructor { get; set; }
                public double Accuracy { get; set; }
                public double Radius { get; set; }
            }

            // Test Scenario: <Path> on Aggregate Not Specified (✗missing path✗)
            public class Zoombini {
                public enum Hair { Spikey, Ponytail, BowlCut, BabyTuft, Hat }
                public enum Eyes { Wide, Single, Sleepy, Glasses, Sunglasses }
                public enum Color { Orange, Green, Red, Purple, Blue }
                public enum Legs { Feet, RollerSkates, Spring, Cycle, Twirler }

                public class Level {
                    [PrimaryKey] public byte Number { get; set; }
                    public byte Checkpoint { get; set; }
                    public string Name { get; set; } = "";
                }

                [PrimaryKey] public Guid ZoombiniID { get; set; }
                public Hair Hairstyle { get; set; }
                public Eyes KindOfEyes { get; set; }
                public Color Nose { get; set; }
                public Legs LegKind { get; set; }
                [Check.LengthIsAtMost(3)] public Level? LostAt { get; set; }
            }

            // Test Scenario: <Path> on Relation Does Not Exist (✗non-existent path✗)
            public class Antipope {
                public string BirthName { get; set; } = "";
                [PrimaryKey] public string PapalName { get; set; } = "";
                [PrimaryKey] public sbyte PapalNumber { get; set; }
                public DateTime TermBegin { get; set; }
                public DateTime TermEnd { get; set; }
                public string RuledFrom { get; set; } = "";
                [Check.LengthIsAtMost(30, Path = "---")] public RelationList<string> CardinalsCreated { get; set; } = new();
                public bool Excommunicated { get; set; }
            }

            // Test Scenario: <Path> on Relation Refers to Non-Primary-Key Field of Anchor Entity (✗non-existent path✗)
            public class Cabaret {
                public enum Kind { Bar, Restaurant, Casino, Nightclub, StripClub, Hotel, ParkDistrict, School, Auditorium, Other }
                public record struct Loc(string Name, Kind Kind);

                [PrimaryKey] public Guid CabaretID { get; set; }
                public DateTime Date { get; set; }
                public Loc Venue { get; set; }
                [Check.LengthIsAtMost(102, Path = "Cabaret.Venue.Name")] public RelationMap<string, string> Performers { get; set; } = new();
                public bool Drag { get; set; }
                public bool Burlesque { get; set; }
            }

            // Test Scenario: <Path> on Relation Not Specified (✗missing path✗)
            public class TikTok {
                [Flags] public enum Kind { Music = 1, News = 2, Sports = 4, Food = 8, Fashion = 16, Gaming = 32, FilmTV = 64, Literature = 128, Education = 256, Peronality = 512, Finance = 1024, Politics = 2048, Technology = 4096 }

                [PrimaryKey] public Guid VideoID { get; set; }
                public string Creator { get; set; } = "";
                public ushort Length { get; set; }
                public bool Viral { get; set; }
                [Check.LengthIsAtMost(100000)] public RelationMap<string, ulong> Views { get; set; } = new();
                public Kind Category { get; set; }
            }

            // Test Scenario: Default Value Does Not Satisfy Constraint (✗contradiction✗)
            public class Obi {
                [PrimaryKey] public string MartialArt { get; set; } = "";
                [Check.LengthIsAtMost(3), Default("White")] public string Color { get; set; } = "";
                [PrimaryKey] public ushort Level { get; set; }
                public bool IsStarter { get; set; }
            }

            // Test Scenario: Originally Valid Default Value No Longer Satisfies Constraint (✗contradiction✗)
            public class Speakeasy {
                public enum NESW { North, South, East, West }
                public enum StreetKind { Street, Avenue, Road, Boulevard, Court, Terrace, Way, Circle }

                public struct Location {
                    public ushort Number { get; set; }
                    public NESW? Direction { get; set; }
                    [Default("Main First Prime")] public string StreetName { get; set; }
                    public StreetKind StreetSuffix { get; set; }
                    public ushort? ApartmentNumber { get; set; }
                    public string City { get; set; }
                    public string? Subnational { get; set; }
                    public string Country { get; set; }
                    public uint ZipCode { get; set; }
                }

                [PrimaryKey] public Guid SpeakeasyID { get; set; }
                public string Name { get; set; } = "";
                [Check.LengthIsAtMost(14, Path = "StreetName")] public Location Address { get; set; }
                public string? Proprietor { get; set; }
                public ulong DrinksServed { get; set; }
                public bool CopsOnPayroll { get; set; }
                public DateTime OperationalDate { get; set; }
            }
        }

        internal static class LengthIsBetween {
            // Test Scenario: Applied to Non-Nullable String Field (✓constrained✓)
            public class Sorority {
                [PrimaryKey] public string Name { get; set; } = "";
                public DateTime Founded { get; set; }
                [Check.LengthIsBetween(4, 1713)] public string Motto { get; set; } = "";
                public int ActiveChapters { get; set; }
                public bool IsSocial { get; set; }
                public bool IsProfessional { get; set; }
            }

            // Test Scenario: Applied to Nullable String Field (✓constrained✓)
            public class Telescope {
                [PrimaryKey] public Guid ID { get; set; }
                [Check.LengthIsBetween(1, int.MaxValue)] public string? Name { get; set; }
                public double Mass { get; set; }
                public ulong Power { get; set; }
                public ushort Length { get; set; }
                public ushort Width { get; set; }
            }

            // Test Scenario: Applied to Numeric Field (✗impermissible✗)
            public class Capacitor {
                [PrimaryKey] public uint ID { get; set; }
                [Check.LengthIsBetween(4, 9)] public float Capacitance { get; set; }
                public double PlateArea { get; set; }
                public double Dielectric { get; set; }
            }

            // Test Scenario: Applied to Character Field (✗impermissible✗)
            public class Lipstick {
                [PrimaryKey] public Guid ProductID { get; set; }
                public string Shade { get; set; } = "";
                public decimal RetailPrice { get; set; }
                [Check.LengthIsBetween(4, 4)] public char Quality { get; set; }
                public float Amount { get; set; }
                public bool IsTwist { get; set; }
            }

            // Test Scenario: Applied to Boolean Field (✗impermissible✗)
            public class Process {
                [PrimaryKey] public ulong PID { get; set; }
                public byte ReturnCode { get; set; }
                public ulong MemoryUsage { get; set; }
                public byte Core { get; set; }
                [Check.LengthIsBetween(876522, 1743209)] public bool IsActive { get; set; }
                public string StdOut { get; set; } = "";
                public string StdErr { get; set; } = "";
            }

            // Test Scenario: Applied to DateTime Field (✗impermissible✗)
            public class Mummy {
                [PrimaryKey] public Guid MummyID { get; set; }
                public string? Identity { get; set; }
                public string Tomb { get; set; } = "";
                [Check.LengthIsBetween(0, 175)] public DateTime Discovered { get; set; }
                public ushort Weight { get; set; }
            }

            // Test Scenario: Applied to Guid Field (✗impermissible✗)
            public class CelticGod {
                [PrimaryKey, Check.LengthIsBetween(3, 25)] public Guid DeityID { get; set; }
                public string? IrishVersion { get; set; }
                public string? WelshVersion { get; set; }
                public string? ScottishVersion { get; set; }
                public string? ManxVersion { get; set; }
            }

            // Test Scenario: Applied to Enumeration Field (✗impermissible✗)
            public class Kinesis {
                public enum Group { Elemental, Physical, Psychic, Ethereal, Other }

                [PrimaryKey] public string XxxKinesis { get; set; } = "";
                public string Manipulable { get; set; } = "";
                [Check.LengthIsBetween(1, 10)] public Group Kind { get; set; }
                public uint Practitioners { get; set; }
                public string? XMan { get; set; }
            }

            // Test Scenario: Applied to Aggregate-Nested String Scalar (✓constrained✓)
            public class LiteraryTrope {
                public record struct Usage(string Work, string Author);

                [PrimaryKey] public int ID { get; set; }
                public string Name { get; set; } = "";
                public string TVTropesURL { get; set; } = "";
                [Check.LengthIsBetween(1, 20, Path = "Work"), Check.LengthIsBetween(35, 100, Path = "Author")] public Usage FirstAppearance { get; set; }
                public ulong Appearances { get; set; }
            }

            // Test Scenario: Applied to Aggregate-Nested Non-String Scalar (✗impermissible✗)
            public class OvernightCamp {
                public record struct Program(uint Sessions, ushort WeeksPerSession, ulong MaxCampers, byte NumFieldTrips);

                [PrimaryKey] public Guid CampID { get; set; }
                public string CampName { get; set; } = "";
                [Check.LengthIsBetween(0, 5, Path = "Sessions")] public Program Schedule { get; set; }
                public ulong Attendance { get; set; }
                public bool JewishAffiliated { get; set; }
            }

            // Test Scenario: Applied to Nested Aggregate (✗impermissible✗)
            public class Dentist {
                public enum Specialty { Cleaning, OralSurgery, Orthodontics, CavityFillings, Other }
                public record struct Degree(ushort Year, string University);
                public record struct Degrees(Degree Bachelors, Degree? Masters, Degree Doctorate);

                [PrimaryKey] public Guid MedicalBoardNumber { get; set; }
                public string Name { get; set; } = "";
                public string? Practice { get; set; }
                public Specialty Focus { get; set; }
                [Check.LengthIsBetween(7, 18, Path = "Doctorate")] public Degrees Qualifications { get; set; }
            }

            // Test Scenario: Applied to Reference-Nested String Scalar (✓constrained✓)
            public class Onomatopoeia {
                public class Entry {
                    [PrimaryKey] public string Dictionary { get; set; } = "";
                    [PrimaryKey] public uint Page { get; set; }
                    [PrimaryKey] public byte WordOnPage { get; set; }
                    public short NumDefinitions { get; set; }
                }

                [PrimaryKey] public string Word { get; set; } = "";
                public string SoundOf { get; set; } = "";
                [Check.LengthIsBetween(14, 53, Path = "Dictionary")] public Entry DictionaryEntry { get; set; } = new();
                public string Language { get; set; } = "";
                public bool IsAnimalSound { get; set; }
            }

            // Test Scenario: Applied to Reference-Nested Non-String Scalar (✗impermissible✗)
            public class TrivialPursuitPie {
                public class Question {
                    [PrimaryKey] public char CardID { get; set; }
                    public string QuestionText { get; set; } = "";
                    public string CorrectAnswer { get; set; } = "";
                }

                [PrimaryKey] public DateTime GameDate { get; set; }
                [PrimaryKey] public string Player { get; set; } = "";
                public Question? GeographyWedge { get; set; }
                [Check.LengthIsBetween(1, 100, Path = "CardID")] public Question? HistoryWedge { get; set; }
                public Question? SportsLeisureWedge { get; set; }
                public Question? ScienceNatureWedge { get; set; }
                public Question? EntertainmentWedge { get; set; }
                public Question? ArtsLiteratureWedge { get; set; }
            }

            // Test Scenario: Applied to Nested Reference (✗impermissible✗)
            public class SumoWrestler {
                public class Number {
                    [PrimaryKey] public bool IsPositive { get; set; }
                    [PrimaryKey] public ulong Integral { get; set; }
                    [PrimaryKey] public double Decimal { get; set; }
                }
                public record struct Date(Number Year, Number Month, Number day);

                [PrimaryKey] public Guid SumoID { get; set; }
                public string Name { get; set; } = "";
                [Check.LengthIsBetween(35, 192, Path = "Month")] public Date DOB { get; set; } = new();
                public ulong CareerWins { get; set; }
                public ulong CareerLosses { get; set; }
                public double MaxWeight { get; set; }
                public bool Yokozuna { get; set; }
            }

            // Test Scenario: Original Constraint on Reference-Nested Field (✓not propagated✓)
            public class Lagoon {
                public enum Landform { Isthmus, Reef, BarrierIsland, Atoll, Manmade }

                public class Country {
                    [PrimaryKey] public string Name { get; set; } = "";
                    [PrimaryKey, Check.LengthIsBetween(18, 196)] public string Capital { get; set; } = "";
                    public ulong Population { get; set; }
                    public ulong Area { get; set; }
                    public string HeadOfState { get; set; } = "";
                }

                [PrimaryKey] public string LagoonName { get; set; } = "";
                public double Latitude { get; set; }
                public double Longitude { get; set; }
                public double Depth { get; set; }
                public ulong SurfaceArea { get; set; }
                public double Volume { get; set; }
                public float Salinity { get; set; }
                public Landform Separator { get; set; }
            }

            // Test Scenario: Applied to Relation-Nested String Scalar (✓constrained✓)
            public class ComicBook {
                public enum Pub { Marvel, DC, Archie, Image, DarkHorse, IDW, Boom, Valiant, Chapterhouse, Dynamite, Ohter }

                [PrimaryKey] public string Title { get; set; } = "";
                public string Series { get; set; } = "";
                public Pub Publisher { get; set; }
                public DateTime Published { get; set; }
                [Check.LengthIsBetween(4, 37, Path = "Item"), Check.LengthIsBetween(8, 19, Path = "ComicBook.Title")] public RelationSet<string> Characters { get; set; } = new();
                public decimal Price { get; set; }
                public bool Vintage { get; set; }
            }

            // Test Scenario: Applied to Relation-Nested Non-String Scalar (✗impermissible✗)
            public class Wormhole {
                public enum Kind { Schwarzschild, EinsteinRosenBridge, KleinBottle }
                public record struct Location(float X, float Y, float Z, DateTime Time, uint UniverseID);

                [PrimaryKey] public Guid ID { get; set; }
                public Kind Variety { get; set; }
                [Check.LengthIsBetween(14, 99, Path = "Item.Z")] public RelationSet<Location> ConnectedLocations { get; set; } = new();
                public double Radius { get; set; }
                public ulong Density { get; set; }
            }

            // Test Scenario: Applied to Nested Relation (✗impermissible✗)
            public class LunarEclipse {
                public enum Kind { Penumbral, Partial, Total, Central, Selenion }
                public record struct Coordinate(float Latitude, float Longitude);
                public record struct Viewing(RelationMap<Coordinate, double> Locations, bool NakedEye);

                [PrimaryKey] public DateTime Date { get; set; }
                public Kind Type { get; set; }
                public sbyte Danjon { get; set; }
                public bool BloodMoon { get; set; }
                [Check.LengthIsBetween(5, 15, Path = "Locations")] public Viewing Visibility { get; set; }
                public bool Predicted { get; set; }
            }

            // Test Scenario: Applied to Field Data-Converted to String Type (✓constrained✓)
            public class AesSedai {
                [PrimaryKey] public string Name { get; set; } = "";
                public bool IsAlive { get; set; }
                public ushort Mentions { get; set; }
                public float Power { get; set; }
                [Check.LengthIsBetween(1, 15), DataConverter(typeof(ToString<int>))] public int? Ajah { get; set; }
            }

            // Test Scenario: Applied to Field Data-Converted from String Type (✗impermissible✗)
            public class AtmosphericLayer {
                [PrimaryKey, DataConverter(typeof(ToInt<string>)), Check.LengthIsBetween(12, 29)] public string Name { get; set; } = "";
                public ulong StartingHeight { get; set; }
                public ulong EndingHeight { get; set; }
                public double Temperature { get; set; }
            }

            // Test Scenario: Lower Bound is equal to Upper Bound (✓constrained✓)
            public class DNACodon {
                [PrimaryKey, Check.LengthIsBetween(3, 3)] public string CodonSequence { get; set; } = "";
                public bool IsStartCodon { get; set; }
                public bool IsStopCodon { get; set; }
                public char AminoAcid { get; set; }
            }

            // Test Scenario: Lower Bound is larger than Upper Bound (✗unsatisfiable✗)
            public class ChristmasCarol {
                [PrimaryKey] public string Title { get; set; } = "";
                public ushort NumWords { get; set; }
                [Check.LengthIsBetween(28841, 1553)] public string FirstVerse { get; set; } = "";
                public long YearComposed { get; set; }
            }

            // Test Scenario: Lower Bound is Negative, Upper Bound is Non-Negative (✗illegal✗)
            public class ShenGongWu {
                [PrimaryKey] public string Name { get; set; } = "";
                public bool HeldByXiaolinMonks { get; set; }
                public uint Appearances { get; set; }
                [Check.LengthIsBetween(-4, 26)] public string InitialEpisode { get; set; } = "";
                public string Ability { get; set; } = "";
            }

            // Test Scenario: Lower Bound and Upper Bound are Both Negative (✗illegal✗)
            public class MilitaryBase {
                [PrimaryKey] public string BaseName { get; set; } = "";
                [Check.LengthIsBetween(-156, -29)] public string Commander { get; set; } = "";
                public DateTime Established { get; set; }
                public ulong Area { get; set; }
                public bool IsActive { get; set; }
            }

            // Test Scenario: Scalar Property Constrained Multiple Times (✓minimaxed✓)
            public class Aria {
                [PrimaryKey] public string Title { get; set; } = "";
                public string SourceOpera { get; set; } = "";
                public ushort Length { get; set; }
                public uint WordCount { get; set; }
                [Check.LengthIsBetween(1, 100), Check.LengthIsBetween(27, 4999)] public string Lyrics { get; set; } = "";
                public string VocalRange { get; set; } = "";
            }

            // Test Scenario: <Path> is `null` (✗illegal✗)
            public class Apocalypse {
                [PrimaryKey] public string Name { get; set; } = "";
                [PrimaryKey, Check.LengthIsBetween(1, 100, Path = null!)] public string SourceMaterial { get; set; } = "";
                public bool IsZombieBased { get; set; }
                public bool IsNuclear { get; set; }
                public ulong NumSurvivors { get; set; }
            }

            // Test Scenario: <Path> on Scalar Does Not Exist (✗non-existent path✗)
            public class SetCard {
                [PrimaryKey] public byte Count { get; set; }
                public string Color { get; set; } = "";
                [Check.LengthIsBetween(3, 13, Path = "---")] public string Pattern { get; set; } = "";
            }

            // Test Scenario: <Path> on Aggregate Does Not Exist (✗non-existent path✗)
            public class InternetCraze {
                public enum Service { Facebook, Instagram, TikTok, Twitter, Snapchat, Pinterest, Tumblr, Reddit }
                public record struct Warning(char Grade, uint Deaths, double RiskFactor);

                [PrimaryKey] public string Name { get; set; } = "";
                public string? URL { get; set; }
                public Service SocialMediaPlatform { get; set; }
                [Check.LengthIsBetween(1000, 10000, Path = "---")] public Warning Dangers { get; set; }
            }

            // Test Scenario: <Path> on Aggregate Not Specified (✗missing path✗)
            public class MesopotamianGod {
                public record struct Naming(string Babylonian, string Sumerian, string Akkadian);

                [PrimaryKey] public Guid DeityIdentifier { get; set; }
                [Check.LengthIsBetween(5, 32)] public Naming Names { get; set; }
                public bool InEpicOfGilgamesh { get; set; }
            }

            // Test Scenario: <Path> on Aggregate Does Not Exist (✗non-existent path✗)
            public class HeatWave {
                public enum Unit { Fahrenheit, Celsius, Kelvin }

                public class Temperature {
                    [PrimaryKey] public float Magnitude { get; set; }
                    [PrimaryKey] public Unit Unit { get; set; }
                }

                [PrimaryKey] public Guid HeatWavID { get; set; }
                public Temperature High { get; set; } = new();
                [Check.LengthIsBetween(2, 12, Path = "---")] public Temperature Low { get; set; } = new();
                public DateTime Start { get; set; }
                public DateTime End { get; set; }
                public ulong Deaths { get; set; }
                public bool RecordTemperature { get; set; }
            }

            // Test Scenario: <Path> on Reference Refers to Non-Primary-Key Field (✗non-existent path✗)
            public class Sprachbund {
                public class Language {
                    [PrimaryKey] public string Exonym { get; set; } = "";
                    public string Endonym { get; set; } = "";
                    public string Glottolog { get; set; } = "";
                    public ulong WorldwideSpeakers { get; set; }
                }

                [PrimaryKey] public string SprachbundName { get; set; } = "";
                public ushort LanguagesEncompassed { get; set; }
                public ulong TotalSpeakers { get; set; }
                public ulong LandArea { get; set; }
                [Check.LengthIsBetween(7, 41, Path = "Endonym")] public Language? Progenitor { get; set; }
                public string? AttributedLinguist { get; set; }
            }

            // Test Scenario: <Path> on Aggregate Not Specified (✗missing path✗)
            public class Leprechaun {
                public class WalkingStick {
                    [PrimaryKey] public Guid StickID { get; set; }
                    public double Length { get; set; }
                    public string Material { get; set; } = "";
                }

                [PrimaryKey] public Guid LeprechaunID { get; set; }
                public string Name { get; set; } = "";
                public bool BornInIreland { get; set; }
                public decimal PotOfGold { get; set; }
                public ushort Height { get; set; }
                [Check.LengthIsBetween(891, 39654)] public WalkingStick Shillelagh { get; set; } = new();
            }

            // Test Scenario: <Path> on Relation Does Not Exist (✗non-existent path✗)
            public class MarsRover {
                public class Specimen {
                    [PrimaryKey] public Guid ID { get; set; }
                    public double Weight { get; set; }
                }

                [PrimaryKey] public string Name { get; set; } = "";
                public DateTime Launched { get; set; }
                public decimal Budget { get; set; }
                public string SpaceAgency { get; set; } = "";
                [Check.LengthIsBetween(177, 179, Path = "---")] public RelationList<Specimen> SpecimensCollected { get; set; } = new();
                public double Height { get; set; }
                public double Length { get; set; }
                public double Width { get; set; }
                public ulong DistanceTraveled { get; set; }
                public bool Online { get; set; }
            }

            // Test Scenario: <Path> on Relation Refers to Non-Primary-Key Field of Anchor Entity (✗non-existent path✗)
            public class BlackOp {
                public enum Branch { Army, Navy, AirForce, Marines, SpaceForce, NationalGuard, Paramilitary, Intelligence }

                [PrimaryKey] public string Name { get; set; } = "";
                public string Country { get; set; } = "";
                public Branch MilitaryBranch { get; set; }
                [Check.LengthIsBetween(6, 29, Path = "BlackOp.Country")] public RelationSet<string> Participants { get; set; } = new();
                public DateTime Executed { get; set; }
                public DateTime Declassified { get; set; }
                public ushort Casualites { get; set; }
                public bool SearchAndRescue { get; set; }
            }

            // Test Scenario: <Path> on Relation Not Specified (✗missing path✗)
            public class HeartAttack {
                [PrimaryKey] public string Victim { get; set; } = "";
                [PrimaryKey] public DateTime Occurrence { get; set; }
                public bool Fatal { get; set; }
                [Check.LengthIsBetween(4, 67)] public RelationSet<string> Symptoms { get; set; } = new();
            }

            // Test Scenario: Default Value Does Not Satisfy Constraint (✗contradiction✗)
            public class PeanutButter {
                [PrimaryKey] public Guid ProductID { get; set; }
                public ushort Calories { get; set; }
                public ushort GramsFat { get; set; }
                [Check.LengthIsBetween(4, 8), Default("Smucker's")] public string Brand { get; set; } = "";
                public bool IsSmooth { get; set; }
                public bool IsNatural { get; set; }
            }

            // Test Scenario: Originally Valid Default Value No Longer Satisfies Constraint (✗contradiction✗)
            public class Kebab {
                public enum Protein { Chicken, Lamb, Beef, Pork, Goat, Duck, Turkey, Pheasant, Hen, Rabbit, Alligator, Chorizo}
                public enum Stick { Wood, Steel, Spaghetti, Lemongrass, Paper, Plastic, Other }

                public struct StreetVendor {
                    [Default("Ezekiel's Meat-on-a-Stick Emporium")] public string Name { get; set; }
                    public string Location { get; set; }
                    public bool FoodTruck { get; set; }
                }

                [PrimaryKey] public Guid ID { get; set; }
                public Protein Meat { get; set; }
                public Stick StickMaterial { get; set; }
                public string? Sauce { get; set; }
                [Check.LengthIsBetween(13, 21, Path = "Name")] public StreetVendor? Vendor { get; set; }
                public decimal Price { get; set; }
            }
        }
    }
    
    internal static class DiscretenessConstraints {
        internal static class IsOneOf {
            // Test Scenario: Applied to Numeric Fields (✓constrained✓)
            public class CoralReef {
                [PrimaryKey] public string Name { get; set; } = "";
                public float Latitude { get; set; }
                [Check.IsOneOf(0.0f, 30f, 45f, 75f, 90f)] public float Longitude { get; set; }
                [Check.IsOneOf(1000UL, 2000UL, 3000UL, 4000UL, 5000UL)] public ulong Length { get; set; }
                [Check.IsOneOf(17, 190841, 79512759, 857791)] public int Area { get; set; }
                public double SCUBARating { get; set; }
                public bool IsSaltwater { get; set; }
            }

            // Test Scenario: Applied to Textual Fields (✓constrained✓)
            public class Encyclopedia {
                [PrimaryKey] public string Organization { get; set; } = "";
                [PrimaryKey, Check.IsOneOf('A', 'B', 'C', 'D', 'E', 'F', 'G')] public char Letter { get; set; }
                [PrimaryKey, Check.IsOneOf("First", "Second", "Third", "Fourth")] public string Edition { get; set; } = "";
                public ulong NumPages { get; set; }
                public ulong NumArticles { get; set; }
            }

            // Test Scenario: Applied to Boolean Field (✓constrained✓)
            public class Astronaut {
                [PrimaryKey] public string AstronautName { get; set; } = "";
                public short MinutesInSpace { get; set; }
                public uint NumSpacewalks { get; set; }
                public DateTime Retirement { get; set; }
                public string MaidenProgram { get; set; } = "";
                [Check.IsOneOf(true, false)] public bool WalkedOnMoon { get; set; }
            }

            // Test Scenario: Applied to Decimal Field (✓constrained✓)
            public class CiderMill {
                [PrimaryKey] public uint ID { get; set; }
                public string Address { get; set; } = "";
                public string? MillName { get; set; }
                [Check.IsOneOf(0.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0)] public decimal AnnualTonnage { get; set; }
                public uint AnnualBottles { get; set; }
                public bool MakesAlcoholic { get; set; }
            }

            // Test Scenario: Applied to DateTime Field (✓constrained✓)
            public class Hospital {
                [PrimaryKey] public string Address { get; set; } = "";
                public ulong NumBeds { get; set; }
                [Check.IsOneOf("2000-01-01", "2000-01-02", "2000-01-03")] public DateTime Opened { get; set; }
                public bool HasTraumaWard { get; set; }
                public ulong StaffCount { get; set; }
            }

            // Test Scenario: Applied to Guid Field (✓constrained✓)
            public class Tsunami {
                [PrimaryKey, Check.IsOneOf("b334ae4e-98c3-4f63-83f8-2bc076eae31b")] public Guid TsunamiID { get; set; }
                public double MaxWaveHeight { get; set; }
                public DateTime Landfall { get; set; }
                public ulong Casualties { get; set; }
                public decimal TotalCost { get; set; }
            }

            // Test Scenario: Applied to Enumeration Field (✓limiting✓)
            public class Tooth {
                public enum Source { Human, Animal, Synthetic }
                public enum ToothType { Incisor, Molar, Canine, Bicuspid }

                [PrimaryKey] public Guid ToothID { get; set; }
                [Check.IsOneOf(Source.Human, Source.Animal)] public Source Origin { get; set; }
                [Check.IsOneOf(ToothType.Incisor, ToothType.Molar, ToothType.Canine, ToothType.Bicuspid)] public ToothType Type { get; set; }
            }

            // Test Scenario: Applied to Aggregate-Nested Scalar (✓constrained✓)
            public class Earring {
                public enum Kind { ClipOn, Stud, Latch }
                public struct Composition {
                    [Check.IsOneOf("Fiberglass", "Leather")] public string Material { get; set; }
                    public bool IsMetal { get; set; }
                }

                [PrimaryKey] public Guid ProductID { get; set; }
                public bool IsHoop { get; set; }
                public decimal MarketValue { get; set; }
                [Check.IsOneOf("Gold", "Silver", "Plastic", "Titanium", "Wood", Path = "Material")] public Composition MadeOf { get; set; }
                public ushort Weight { get; set; }
                public Kind Style { get; set; }
            }

            // Test Scenario: Applied to Nested Aggregate (✗impermissible✗)
            public class PhoneticAlphabet {
                public record struct ABC(string A, string B, string C);
                public record struct DEF(string D, string E, string F);
                public record struct GHIJK(string G, string H, string I, string J, string K);
                public record struct LMNOPQRS(string L, string M, string N, string O, string P, string Q, string R, string S);
                public record struct TU(string T, string U);
                public record struct VWXY(string V, string W, string X, string Y);
                public record struct LetterZ(string Z);
                public record struct ABCDEFGHIJK(ABC ABC, DEF DEF, GHIJK GHIJK);
                public record struct TUVWXY(TU TU, VWXY VWXY);
                public record struct ABCDEFGHIJKLMNOPQRS(ABCDEFGHIJK ABCDEFGHIJK, LMNOPQRS LMNOPQRS);
                public struct All {
                    [Check.IsOneOf("Grand", "Golf", "Give", "Gordon", "Gate", Path = "ABCDEFGHIJK.GHIJK")] public ABCDEFGHIJKLMNOPQRS ABCDEFGHIJKLMNOPQRS { get; set; }
                    public TUVWXY TUVWXY { get; set; }
                    public LetterZ Z {get;set;}
                }

                [PrimaryKey] public string Creator { get; set; } = "";
                public All Alphabet { get; set; }
            }

            // Test Scenario: Applied to Reference-Nested Scalar (✓constrained✓)
            public class FerrisWheel {
                public class Person {
                    [PrimaryKey] public string FirstName { get; set; } = "";
                    [PrimaryKey] public string LastName { get; set; } = "";
                }

                [PrimaryKey] public Guid WheelID { get; set; }
                public double Diameter { get; set; }
                public ushort NumSeat { get; set; }
                public ushort WeightLimit { get; set; }
                public DateTime Opened { get; set; }
                [Check.IsOneOf("Alexander", "Randall", "Corrine", Path = "FirstName")] public Person Designer { get; set; } = new();
            }

            // Test Scenario: Applied to Nested Reference (✗impermissible✗)
            public class Pulsar {
                public class Observatory {
                    [PrimaryKey] public string Name { get; set; } = "";
                    [PrimaryKey] public string ChiefAstronomer { get; set; } = "";
                    public DateTime Groundbreaking { get; set; }
                    public ushort NumTelescopes { get; set; }
                }
                public record struct OBS(Observatory Observatory);

                [PrimaryKey] public string Name { get; set; } = "";
                [Check.IsOneOf(1, -3, 18, 220, 6, Path = "Observatory")] public OBS FirstObservedAt { get; set; }
                public double Declination { get; set; }
                public ulong Distance { get; set; }
                public double SpinRate { get; set; }
            }

            // Test Scenario: Original Constraint on Reference-Nested Field (✓not propagated✓)
            public class PullRequest {
                public enum BranchType { Main, Development, Release, Bugfix, Hotfix, Support, Feature, Docs }
                public enum Result { InProgress, Approved, Merged, Declined, ChangesRequested }

                public class Branch {
                    [PrimaryKey] public string BranchName { get; set; } = "";
                    [Check.IsOneOf(true, false)] public bool Visibile { get; set; }
                    public BranchType Type { get; set; }
                    public string TopCommit { get; set; } = "";
                    public uint NumTags { get; set; }
                }

                [PrimaryKey] public Branch TargetBranch { get; set; } = new();
                public Branch SourceBranch { get; set; } = new();
                public ulong RequestNumber { get; set; }
                public string Author { get; set; } = "";
                public Result Status { get; set; }
                public string Description { get; set; } = "";
                public DateTime Opened { get; set; } 
            }

            // Test Scenario: Applied to Relation-Nested Scalar (✓constrained✓)
            public class Dinosaur {
                public enum Period { Jurassic, Cretaceous, Triassic }
                public enum Hips { Saurischia, Ornithischia }
                public enum Diet { Herbivore, Carnivore, Omnivore }

                [PrimaryKey] public string Genus { get; set; } = "";
                [PrimaryKey] public string Species { get; set; } = "";
                public string CommonName { get; set; } = "";
                public Period MesozoicPeriod { get; set; }
                public Hips Clade { get; set; }
                public Diet FoodPreference { get; set; }
                [Check.IsOneOf("Americas", "Eurasia", "Middle East", "Africa", "Australia", "Pacific Islands", "Arctic", "Antarctica", "Oceans", Path = "Item")] public RelationSet<string> FossilLocations { get; set; } = new();
                public int FacialHorns { get; set; }
                public int NumTeeth { get; set; }
            }

            // Test Scenario: Applied to Nested Relation (✗impermissible✗)
            public class Cheerleader {
                [Flags] public enum Item { PomPoms = 1, Batons = 2, Dancing = 4, Sparklers = 8, Instruments = 16 }
                public record struct Cheer(string Title, string CallSign, RelationMap<int, string> Moves, string Music);

                [PrimaryKey] public string Name { get; set; } = "";
                public string Squad { get; set; } = "";
                public ushort Championships { get; set; }
                public int PyramidPosition { get; set; }
                public bool IsCaptain { get; set; }
                public double Height { get; set; }
                [Check.IsOneOf(1, 2, 5, Path = "Moves")] public Cheer PrimaryCheer { get; set; }
                public Item Accessories { get; set; }
            }

            // Test Scenario: Applied to Nullable Fields (✓constrained✓)
            public class Wildfire {
                [PrimaryKey] public Guid WildfireID { get; set; }
                [Check.IsOneOf("Lightning", "Arson", "Electrical")] public string? Cause { get; set; }
                public DateTime Started { get; set; }
                [Check.IsOneOf(5718.37, 1984.6, 279124.9)] public double? MaxTemperature { get; set; }
                public ulong Area { get; set; }
            }

            // Test Scenario: Duplicated Allowed Value (✓de-duplicated✓)
            public class HealingPotion {
                [PrimaryKey] public string Variety { get; set; } = "";
                public byte Rolls { get; set; }
                [Check.IsOneOf(4u, 8u, 10u, 12u, 20u, 100u, 8u, 12u, 8u, 4u, 8u)] public uint DieType { get; set; }
                public sbyte Plus { get; set; }
                public ulong Uses { get; set; }
            }

            // Test Scenario: Inconvertible Non-`null` Allowed Value (✗invalid✗)
            public class Battery {
                [PrimaryKey] public Guid ProductID { get; set; }
                public string Cathode { get; set; } = "";
                public string Anode { get; set; } = "";
                [Check.IsOneOf(1, 2, 3, 4, 5, "six", 7, 8, 9)] public int Voltage { get; set; }
            }

            // Test Scenario: Convertible Non-`null` Allowed Value (✗invalid✗)
            public class TennisMatch {
                [PrimaryKey] public Guid MatchIdentifier { get; set; }
                public string Player1 { get; set; } = "";
                public string Player2 { get; set; } = "";
                [Check.IsOneOf((byte)0, (byte)15, (byte)30, (byte)40)] public sbyte Player1Score { get; set; }
                public sbyte Player2Score { get; set; }
                public string? Tournament { get; set; }
            }

            // Test Scenario: Invalid Enumerator Allowed Value (✗invalid✗)
            public class SpeedLimit {
                public enum StreetType { Residential, Highway, Public, Unincorporated, Personal }

                [PrimaryKey] public string StreetName { get; set; } = "";
                [Check.IsOneOf(StreetType.Highway, StreetType.Public, (StreetType)40000)] public StreetType TypeOfStreet { get; set; }
                [PrimaryKey] public float StartDistance { get; set; }
                [PrimaryKey] public float EndDistance { get; set; }
                public ushort MaximumSpeed { get; set; }
                public ushort MinimumSpeed { get; set; }
            }

            // Test Scenario: Single-Element Array Allowed Value (✗invalid✗)
            public class Flashcard {
                [PrimaryKey] public Guid DeckID { get; set; }
                [PrimaryKey] public int FlaschardNumber { get; set; }
                public string Front { get; set; } = "";
                public string Back { get; set; } = "";
                [Check.IsOneOf(true, new[] { false })] public bool IsLearned { get; set; }
            }

            // Test Scenario: `null` Allowed Value (✗invalid✗)
            public class Prophet {
                [PrimaryKey] public string EnglishName { get; set; } = "";
                [Check.IsOneOf("Mosheh", "Noach", null!, "Av'raham", "Yizhak")] public string? HebrewName { get; set; }
                public string? LatinName { get; set; }
                public string? ArabicName { get; set; }
                public bool SpeaksToGod { get; set; }
                public long AgeAtDeath { get; set; }
            }

            // Test Scenario: Decimal Allowed Value is Not a Double (✗invalid✗)
            public class Carousel {
                [PrimaryKey] public uint CarouselID { get; set; }
                public ushort NumSeats { get; set; }
                public bool AnimalThemed { get; set; }
                [Check.IsOneOf(0.10, 0.20, 0.30, 0.40f, 0.50, 1.0)] public decimal RoundDuration { get; set; }
                public float PlatformRadius { get; set; }
            }

            // Test Scenario: Decimal Allowed Value is Out-of-Range (✗invalid✗)
            public class Borate {
                [PrimaryKey] public string ChemicalFormula { get; set; } = "";
                public string? CommonName { get; set; }
                [Check.IsOneOf(137.441, 162.09, double.MinValue, 55.44444)] public decimal MolarMass { get; set; }
                public ushort AtomsOfBoron { get; set; }
                public double Solubility { get; set; }
                public double BoilingPoint { get; set; }
                public double MeltingPoint { get; set; }
            }

            // Test Scenario: DateTime Allowed Value is Not a String (✗invalid✗)
            public class DalaiLama {
                [PrimaryKey] public uint Number { get; set; }
                public string BirthName { get; set; } = "";
                [Check.IsOneOf("1766-04-19", 1824UL, "2388-08-01")] public DateTime Birthdate { get; set; }
            }

            // Test Scenario: DateTime Allowed Value is Improperly Formatted (✗invalid✗)
            public class Voicemail {
                [PrimaryKey] public Guid VoicemailID { get; set; }
                public string Caller { get; set; } = "";
                [Check.IsOneOf("Thursday")] public DateTime When { get; set; }
                public ushort Length { get; set; }
                public string Contents { get; set; } = "";
            }

            // Test Scenario: DateTime Allowed Value is Out-of-Range (✗invalid✗)
            public class FinalJeopardy {
                [PrimaryKey, Check.IsOneOf("1299-08-45")] public DateTime AirDate { get; set; }
                public string Category { get; set; } = "";
                public string Clue { get; set; } = "";
                public string Answer { get; set; } = "";
                public decimal? Player1Wager { get; set; }
                public decimal? Player2Wager { get; set; }
                public decimal? Player3Wager { get; set; }
            }

            // Test Scenario: Guid Allowed Value is Not a String (✗invalid✗)
            public class BiologicalCycle {
                [PrimaryKey, Check.IsOneOf('c', "e1951075-e11b-4b51-a8a5-83fa08587370")] public Guid ID { get; set; }
                public string Name { get; set; } = "";
                public string Input { get; set; } = "";
                public string Output { get; set; } = "";
                public ushort NumSteps { get; set; }
            }

            // Test Scenario: Guid Allowed Value is Improperly Formatted (✗invalid✗)
            public class WaterBottle {
                [PrimaryKey, Check.IsOneOf("A-G-U-I-D")] public Guid ProductID { get; set; }
                public string Material { get; set; } = "";
                public bool Reusable { get; set; }
                public double Volume { get; set; }
            }

            // Test Scenario: Allowed Value of Source Type on Data-Converted Property (✗invalid✗)
            public class Burrito {
                [PrimaryKey] public string Description { get; set; } = "";
                [DataConverter(typeof(ToInt<string>)), Check.IsOneOf("Chicken", "Steak", "Carnitas", "Barbacoa", "Chorizo")] public string Protein { get; set; } = "";
                public bool HasCheese { get; set; }
                public bool HasSalsa { get; set; }
                public bool HasLettuce { get; set; }
                public bool HasTomatoes { get; set; }
                public bool HasSourCream { get; set; }
                public bool HasGuacamole { get; set; }
                public bool HasOnions { get; set; }
                public string? OtherIngredients { get; set; }
                public ushort Calories { get; set; }
            }

            // Test Scenario: Allowed Value of Target Type on Data-Converted Property (✓valid✓)
            public class WaterSlide {
                [PrimaryKey] public Guid SlideID { get; set; }
                public ushort Length { get; set; }
                public float HeightMinimum { get; set; }
                public double WeightMaximum { get; set; }
                [DataConverter(typeof(ToString<int>)), Check.IsOneOf("Straight", "Curly", "Funnel")] public int Type { get; set; }
                public bool IsTubeSlide { get; set; }
            }

            // Test Scenario: Scalar Property Constrained Multiple Times (✓union✓)
            public class Cannon {
                [PrimaryKey] public Guid GUID { get; set; }
                public float Weight { get; set; }
                [Check.IsOneOf(7, 2, 4, 1), Check.IsOneOf(2, 6)] public int Capacity { get; set; }
                public bool IsAutomatic { get; set; }
            }

            // Test Scenario: Only One Enumerator Permitted (✓valid✓)
            public class Treehouse {
                [Flags] public enum Manufacturing { Amateur = 1, Professional = 2, Kit = 4, Factory = 8 }

                [PrimaryKey] public Guid ID { get; set; }
                [Check.IsOneOf(Manufacturing.Professional)] public Manufacturing MadeBy { get; set; }
                public double Elevation { get; set; }
                public double Height { get; set; }
                public double Length { get; set; }
                public double Width { get; set; }
                public string PrimaryWood { get; set; } = "";
            }

            // Test Scenario: <Path> is `null` (✗illegal✗)
            public class Dragon {
                [PrimaryKey] public Guid DragonID { get; set; }
                [Check.IsOneOf("Dragon", "Wyrm", "Wyvern", Path = null!)] public string Species { get; set; } = "";
                public ulong Length { get; set; }
                public ulong Weight { get; set; }
                public decimal TotalHoard { get; set; }
                public bool Firebreathing { get; set; }
                public ulong NumScales { get; set; }
            }

            // Test Scenario: <Path> on Scalar Does Not Exist (✗non-existent path✗)
            public class HomericHymn {
                [PrimaryKey] public string To { get; set; } = "";
                [Check.IsOneOf(1, 5, 25, 50, Path = "---")] public int Lines { get; set; }
                public string OriginalGreekText { get; set; } = "";
                public string EnglishTranslation { get; set; } = "";
            }

            // Test Scenario: <Path> on Aggregate Does Not Exist (✗non-existent path✗)
            public class MarbleLeague {
                public record struct Team(string Nickname, string Abbreviation, ushort Established);

                [PrimaryKey] public ushort Year { get; set; }
                public Team Host { get; set; }
                [Check.IsOneOf("CHO", "IND", "LIM", "ROJ", Path = "---")] public Team Victor { get; set; }
                public uint NumEvents { get; set; }
                public DateTime Opening { get; set; }
                public DateTime Closing { get; set; }
                public ulong InjuredMarbles { get; set; }
                public ushort LeagueRecordRun { get; set; }
            }

            // Test Scenario: <Path> on Aggregate Not Specified (✗missing path✗)
            public class Artery {
                public record struct Naming(string English, string Latin);

                [PrimaryKey] public string MeSH { get; set; } = "";
                [Check.IsOneOf("Pulmonary", "Aorta", "Femoral")] public Naming Name { get; set; }
                public ushort Length { get; set; }
                public double BloodFlow { get; set; }
                public string Source { get; set; } = "";
            }

            // Test Scenario: <Path> on Reference Does Not Exist (✗non-existent path✗)
            public class Safari {
                public class Animal {
                    public string CommonName { get; set; } = "";
                    [PrimaryKey] public string Genus { get; set; } = "";
                    [PrimaryKey] public string Species { get; set; } = "";
                    public ulong Viewings { get; set; }
                }

                [PrimaryKey] public Guid SafariID { get; set; }
                public string Guide { get; set; } = "";
                public Animal Zebras { get; set; } = new();
                public Animal Lions { get; set; } = new();
                public Animal Giraffes { get; set; } = new();
                [Check.IsOneOf('x', 'p', 'L', '3', '_', '$', '/', Path = "---")] public Animal Elephants { get; set; } = new();
                public Animal Rhinoceroses { get; set; } = new();
                public Animal Gazelles { get; set; } = new();
                public Animal Hippopotamuses { get; set; } = new();
                public uint Duration { get; set; }
            }

            // Test Scenario: <Path> on Reference Refers to Non-Primary-Key Field (✗non-existent path✗)
            public class Adverb {
                public enum POS { Verb, Adverb, Noun, Adjective, Interjection, Conjunction, Interrogative, Article, Pronoun }
                public enum Kind { Pronominal, Flat, Conjunctive, Locative }

                public class Suffix {
                    [PrimaryKey] public string Chars { get; set; } = "";
                    public POS PartOfSpeech { get; set; }
                }

                [PrimaryKey] public string Word { get; set; } = "";
                public string AdjectivalForm { get; set; } = "";
                [Check.IsOneOf(POS.Adverb, Path = "PartOfSpeech")] public Suffix WordSuffix { get; set; } = new();
                public string Language { get; set; } = "";
            }

            // Test Scenario: <Path> on Reference Not Specified (✗missing path✗)
            public class Swamp {
                public enum Water { Salt, Fresh, Brackish }

                public class Tree {
                    public string CommonName { get; set;} = "";
                    [PrimaryKey] public string Genus { get; set; } = "";
                    [PrimaryKey] public string Species { get; set; } = "";
                }

                [PrimaryKey] public string Name { get; set; } = "";
                public Water WaterType { get; set; }
                public ulong Area { get; set; }
                public double WaterVolume { get; set; }
                public float Latitude { get; set; }
                public float Longitude { get; set; }
                [Check.IsOneOf(true, false)] public Tree PredominantTree { get; set; } = new();
                public bool IsProtectedArea { get; set; }
            }

            // Test Scenario: <Path> on Relation Does Not Exist (✗non-existent path✗)
            public class PawnShop {
                public enum Status { Pawned, Sold, Requisitioned, Found }
                public record struct Object(string Name, Status Status, decimal Appraisal, DateTime Acquired);

                [PrimaryKey] public string ShopName { get; set; } = "";
                public RelationList<string> Employees { get; set; } = new();
                public sbyte Floors { get; set; }
                public DateTime Opened { get; set; }
                public Guid License { get; set; }
                [Check.IsOneOf('A', '7', '_', '/', '=', '~', Path = "---")] public RelationList<Object> Inventory { get; set; } = new();
                public decimal Revenue { get; set; }
            }

            // Test Scenario: <Path> on Relation Refers to Non-Primary-Key Field of Anchor Entity (✗non-existent path✗)
            public class DrinkingFountain {
                [PrimaryKey] public Guid ID { get; set; }
                public string Location { get; set; } = "";
                public bool Segregated { get; set; }
                public bool EcoFriendly { get; set; }
                public double WaterPressure { get; set; }
                public ulong VolumeDispensed { get; set; }
                [Check.IsOneOf(37.6, 1158.44, 0.919, 63.6666, Path = "DrinkingFountain.WaterPressure")] public RelationSet<DateTime> Inspections { get; set; } = new();
                public bool MotionSensored { get; set; }
            }

            // Test Scenario: <Path> on Relation Not Specified (✗missing path✗)
            public class Hairstyle {
                public enum Gender { Male, Female, Androgynous, Animal, None }

                [PrimaryKey] public Guid ID { get; set; }
                public string Name { get; set; } = "";
                public string Description { get; set; } = "";
                [Check.IsOneOf(true, false)] public RelationMap<Guid, bool> Certifications { get; set; } = new();
                public DateTime? FirstDocumented { get; set; }
                public string? Culture { get; set; } = "";
                public Gender AppropriateFor { get; set; }
            }

            // Test Scenario: Default Value Does Not Satisfy Constraint (✗contradiction✗)
            public class Guillotine {
                [PrimaryKey] public Guid ItemID { get; set; }
                public ulong Decapitations { get; set; }
                [Check.IsOneOf(30U, 60U, 90U, 120U), Default(113U)] public uint Height { get; set; }
            }

            // Test Scenario: Originally Valid Default Value No Longer Satisfies Constraint (✗contradiction✗)
            public class IKEAFurniture {
                public enum Group { Bedroom, Bathroom, Kitchen, LivingRoom, Office, Den, GameRoom, Lobby, Other }

                public struct Listing {
                    public Guid ListingNumber { get; set; }
                    [Default(Group.Den)] public Group Room { get; set; }
                    public decimal Price { get; set; }
                }

                [PrimaryKey] public Guid ID { get; set; }
                public string Name { get; set; } = "";
                [Check.IsOneOf(Group.Bedroom, Group.Bathroom, Path = "Room")] public Listing CatalogEntry { get; set; }
                public uint NumPieces { get; set; }
                public uint NumScrews { get; set; }
            }
        }

        internal static class IsNotOneOf {
            // Test Scenario: Applied to Numeric Fields (✓constrained✓)
            public class NationalAnthem {
                [PrimaryKey] public string Endonym { get; set; } = "";
                public string Exonym { get; set; } = "";
                public string Language { get; set; } = "";
                public string ForCountry { get; set; } = "";
                [Check.IsNotOneOf(0U, 5U)] public uint WordCount { get; set; }
                [Check.IsNotOneOf(1.3f, 1.6f, 1.9f, 2.2f, 2.5f, 2.8f, 3.1f, 3.4f)] public float Length { get; set; }
                [Check.IsNotOneOf(0L, 1L, 2L)] public long Revision { get; set; }
            }

            // Test Scenario: Applied to Textual Fields (✓constrained✓)
            public class Taxi {
                [PrimaryKey] public uint MedallionNumber { get; set; }
                public string? Driver { get; set; }
                [Check.IsNotOneOf('1', '3', '5', '7', '9')] public char Quality { get; set; }
                public byte NumDoors { get; set; }
                [Check.IsNotOneOf("YellowCab", "Cash Cab", "Uber", "Lyft")] public string Company { get; set; } = "";
            }

            // Test Scenario: Applied to Boolean Field (✓constrained✓)
            public class BirthControl {
                [PrimaryKey] public int FDAID { get; set; }
                public double Efficacy { get; set; }
                public DateTime? FirstAttested { get; set; }
                [Check.IsNotOneOf(false)] public bool ForWomen { get; set; }
                public bool Permanent { get; set; }
            }

            // Test Scenario: Applied to Decimal Field (✓constrained✓)
            public class HouseCommittee {
                [PrimaryKey] public string Committee { get; set; } = "";
                public string Chairperson { get; set; } = "";
                public ushort Seats { get; set; }
                public ushort NumSubcommittees { get; set; }
                public uint NumHearings { get; set; }
                [Check.IsNotOneOf(0.0, 1000.0, 100000.0, 100000000.0)] public decimal Budget { get; set; }
            }

            // Test Scenario: Applied to DateTime Field (✓constrained✓)
            public class GamingConsole {
                [PrimaryKey] public Guid SerialNumber { get; set; }
                public string Name { get; set; } = "";
                public string? AKA { get; set; }
                [Check.IsNotOneOf("1973-04-30", "1973-05-30")] public DateTime Launched { get; set; }
                public DateTime? Retired { get; set; }
                public ulong UnitsSold { get; set; }
                public float CPUClockCycle { get; set; }
            }

            // Test Scenario: Applied to Guid Field (✓constrained✓)
            public class Podcast {
                [PrimaryKey, Check.IsNotOneOf("70324253-a5df-4208-9939-44a11243ceb0", "2e748258-29e6-4abd-a1e1-3e93262e4c04")] public Guid ID { get; set; }
                public string RSSFeed { get; set; } = "";
                public uint NumEpisodes { get; set; }
                public string Title { get; set; } = "";
                public string Podcaster { get; set; } = "";
                public DateTime FirstAired { get; set; }
                public bool OnSpotify { get; set; }
            }

            // Test Scenario: Applied to Enumeration Field (✓limiting✓)
            public class RorschachInkBlot {
                public enum Object { Bat, Butterfly, Humans, Skin, Lobster, Other }

                [PrimaryKey] public int BlotNumber { get; set; }
                [Check.IsNotOneOf(Object.Other, Object.Lobster)] public Object MostCommonAnswer { get; set; }
                public string Commentary { get; set; } = "";
                public string ImageURL { get; set; } = "";
            }

            // Test Scenario: Applied to Aggregate-Nested Scalar (✓constrained✓)
            public class Condiment {
                [Flags] public enum Taste { Sweet = 1, Sour = 2, Salty = 4, Umami = 16, Bitter = 32 }
                public record struct Label(sbyte Calories, sbyte Protein, sbyte Sugar, sbyte Carbohydrates);

                [PrimaryKey] public string Name { get; set; } = "";
                public string Color { get; set; } = "";
                public Taste Flavor { get; set; }
                [Check.IsNotOneOf((sbyte)7, (sbyte)12, (sbyte)105, Path = "Sugar")] public Label Nutrition { get; set; }
                public bool AllowedOnChicagoDog { get; set; }
                public bool HeinzProduct { get; set; }
            }

            // Test Scenario: Applied to Nested Aggregate (✗impermissible✗)
            public class Tattoo {
                public enum TattooKind { Permanent, Temporary, Henna, Childrens }
                public record struct Color(byte R, byte G, byte B);
                public record struct InkDefinition(Color Color, double Quantity);

                [PrimaryKey] public string Subject { get; set; } = "";
                [PrimaryKey] public string Location { get; set; } = "";
                public string Shape { get; set; } = "";
                [Check.IsNotOneOf(true, false, Path = "Color")] public InkDefinition Ink { get; set; }
                public string Artist { get; set; } = "";
                public TattooKind Kind { get; set; }
            }

            // Test Scenario: Applied to Reference-Nested Scalar (✓constrained✓)
            public class Lifeguard {
                public class Pool {
                    [PrimaryKey] public uint PoolNumber { get; set; }
                    public bool IsPublic { get; set; }
                    public bool Indoors { get; set; }
                    public ushort Capacity { get; set; }
                    public ulong Volume { get; set; }
                }

                [PrimaryKey] public Guid License { get; set; }
                public string FirstName { get; set; } = "";
                public string LastName { get; set; } = "";
                public int SwimLevel { get; set; }
                [Check.IsNotOneOf(7U, 17U, 27U, 37U, Path = "PoolNumber")] public Pool? FirstJob { get; set; }
                public ushort Savings { get; set; }
            }

            // Test Scenario: Applied to Nested Reference (✗impermissible✗)
            public class NurseryRhyme {
                public class Character {
                    [PrimaryKey] public string Name { get; set; } = "";
                    public sbyte Mentions { get; set; }
                }
                public record struct One(Character Character);

                [PrimaryKey] public string Title { get; set; } = "";
                public string Text { get; set; } = "";
                [Check.IsNotOneOf("2003-04-17", "1888-08-18", Path = "Character")] public One MainCharacter { get; set; } = new();
                public byte LowerAg { get; set; }
                public byte UpperAge { get; set; }
                public bool MotherGoose { get; set; }
            }

            // Test Scenario: Original Constraint on Reference-Nested Field (✓not propagated✓)
            public class SearchWarrant {
                public class Judge {
                    [PrimaryKey, Check.IsNotOneOf("Anderson", "Cardoza")] public string Name { get; set; } = "";
                    public string LawSchool { get; set; } = "";
                    public sbyte YearsExperience { get; set; }
                    public uint CasesOverseen { get; set; }
                    public bool IsFederalJudge { get; set; }
                    public bool IsElected { get; set; }
                }

                [PrimaryKey] public Guid WarrantID { get; set; }
                public string Subject { get; set; } = "";
                public Judge IssuedBy { get; set; } = new();
                public DateTime IssuedOn { get; set; }
                public bool IsFISA { get; set; }
            }

            // Test Scenario: Applied to Relation-Nested Scalar (✓constrained✓)
            public class Infomercial {
                [PrimaryKey] public Guid ID { get; set; }
                public string Product { get; set; } = "";
                [Check.IsNotOneOf("2022-03-17", "1965-11-14", "1333-01-02", Path = "Value")] public RelationMap<uint, DateTime> Broadcasts { get; set; } = new();
                public string Hawker { get; set; } = "";
                public double Discount { get; set; }
                public double CommercialLength { get; set; }
                public ulong Hits { get; set; }
            }

            // Test Scenario: Applied to Nested Relation (✗impermissible✗)
            public class PersonOfTheYear {
                public enum Category { Politics, FilmTV, Sports, Music, News, Science, Philanthropy, Religion, Art, Other }
                public record struct Magazine(DateTime Publication, RelationList<uint> Editions);

                [PrimaryKey] public ushort Year { get; set; }
                public string Person { get; set; } = "";
                public Category Domain { get; set; }
                [Check.IsNotOneOf(7U, -9U, 195U, 44U, Path = "Editions")] public Magazine TIME { get; set; }
                public bool Controversial { get; set; }
                public string RunnerUp { get; set; } = "";
            }

            // Test Scenario: Applied to Nullable Fields (✓constrained✓)
            public class PIERoot {
                [PrimaryKey] public string Root { get; set; } = "";
                public string Meaning { get; set; } = "";
                [Check.IsNotOneOf("Manger", "Faire", "Avoir", "Parler")] public string? FrenchExample { get; set; }
                [Check.IsNotOneOf("Comer", "Hacer", "Tener", "Hablar")] public string? SpanishExample { get; set; }
                public string? RussianExample { get; set; }
                public string? GaelicExample { get; set; }
                public string? GreekExample { get; set; }
            }

            // Test Scenario: Duplicated Allowed Value (✓de-duplicated✓)
            public class Tweet {
                [PrimaryKey] public Guid TweetID { get; set; }
                public string Text { get; set; } = "";
                public uint PosterID { get; set; }
                [Check.IsNotOneOf('A', 'E', 'I', 'E', 'U', 'A', 'O', 'U', 'I')] public char Grading { get; set; }
                public ulong Likes { get; set; }
                public ulong Retweets { get; set; }
                public ulong Favorites { get; set; }
            }

            // Test Scenario: Inconvertible Non-`null` Disallowed Value (✗invalid✗)
            public class Cancer {
                [PrimaryKey] public uint DiseaseID { get; set; }
                public string Name { get; set; } = "";
                public double MortalityRate { get; set; }
                [Check.IsNotOneOf("Spleen", "Earlobe", 17.3f, "Elbow")] public string RegionAffected { get; set; } = "";
                public bool HereditarilyMarked { get; set; }
            }

            // Test Scenario: Convertible Non-`null` Disallowed Value (✗invalid✗)
            public class Avatar {
                [PrimaryKey] public string Namme { get; set; } = "";
                [Check.IsNotOneOf((byte)8, (byte)111, (byte)217)] public ushort DebutEpisode { get; set; }
                public string HomeNation { get; set; } = "";
                public bool MasteredWater { get; set; }
                public bool MasteredFire { get; set; }
                public bool MasteredEarth { get; set; }
                public bool MasteredAir { get; set; }
                public bool MasteredEnergy { get; set; }
            }

            // Test Scenario: Invalid Enumerator Disallowed Value (✗invalid✗)
            public class Emotion {
                public enum EmotionType { Positive, Negative, Neutral }

                [PrimaryKey] public string Name { get; set; } = "";
                public string? Color { get; set; }
                [Check.IsNotOneOf((EmotionType)(-3))] public EmotionType Connotation { get; set; }
                public bool DeadlySin { get; set; }
            }

            // Test Scenario: Single-Element Array Disallowed Value (✗invalid✗)
            public class Wristwatch {
                [PrimaryKey] public Guid ID { get; set; }
                [Check.IsNotOneOf("Rolex", "Cartier", new[] { "Tag Heuer" })] public string Brand { get; set; } = "";
                public decimal MarketValue { get; set; }
                public bool IsAdjustable { get; set; }
                public bool CanStopwatch { get; set; }
            }

            // Test Scenario: `null` Disallowed Value (✗invalid✗)
            public class Ballet {
                [PrimaryKey] public string BalletName { get; set; } = "";
                public string Composer { get; set; } = "";
                public ushort Length { get; set; }
                public uint NumDancers { get; set; }
                [Check.IsNotOneOf(null!, 1, 3, 5, 7, 9)] public int? OpusNumber { get; set; }
            }

            // Test Scenario: Decimal Anchor is Not a Double (✗invalid✗)
            public class AmericanIdol {
                [PrimaryKey] public byte Season { get; set; }
                public string Singer { get; set; } = "";
                public string RunnerUp { get; set; } = "";
                public string FinalSong { get; set; } = "";
                [Check.IsNotOneOf(0.15, 0.30, 0.45, 0.60, 0.75, "0.90", 1.05)] public decimal VoteShare { get; set; }
            }

            // Test Scenario: Decimal Anchor is Out-of-Range (✗invalid✗)
            public class RussianTsar {
                [PrimaryKey] public string Name { get; set; } = "";
                [PrimaryKey] public byte TsarichNumber { get; set; }
                public string? RoyalHouse { get; set; }
                [Check.IsNotOneOf(double.MaxValue)] public decimal DaysReigned { get; set; }
                public DateTime Coronation { get; set; }
                public bool WasRussianOrthodox { get; set; }
            }

            // Test Scenario: DateTime Disallowed Value is Not a String (✗invalid✗)
            public class Mayor {
                [PrimaryKey] public string Name { get; set; } = "";
                [PrimaryKey] public string City { get; set; } = "";
                [PrimaryKey] public DateTime TermBegin { get; set; }
                [Check.IsNotOneOf('T')] public DateTime? TermEnd { get; set; }
                public string DeputyMayor { get; set; } = "";
            }

            // Test Scenario: DateTime Disallowed Value is Improperly Formatted (✗invalid✗)
            public class Inator {
                public string Name { get; set; } = "";
                public bool StoppedByPerry { get; set; }
                [Check.IsNotOneOf("2018-07-03", "1875~06~22", "73-01-15")] public DateTime Debut { get; set; }
                public string Effects { get; set; } = "";
            }

            // Test Scenario: DateTime Disallowed Value is Out-of-Range (✗invalid✗)
            public class Museum {
                [PrimaryKey] public string Name { get; set; } = "";
                public ushort NumExhibits { get; set; }
                public decimal TicketPrice { get; set; }
                [Check.IsNotOneOf("1375-49-14", "1222-02-20")] public DateTime GrandOpening { get; set; }
                public bool Smithsonian { get; set; }
            }

            // Test Scenario: Guid Disallowed Value is Not a String (✗invalid✗)
            public class Cruise {
                [PrimaryKey, Check.IsNotOneOf("12741762-5df8-47ec-acee-0a11b4942599", 'f')] public Guid CruiseID { get; set; }
                public string OriginPort { get; set; } = "";
                public string DestinationPort { get; set; } = "";
                public string CruiseLine { get; set; } = "";
                public uint PassengerCapacity { get; set; }
                public uint NumRooms { get; set; }
                public bool AllInclusive { get; set; }
                public decimal FirstClassCost { get; set; }
                public decimal CoachCost { get; set; }
            }

            // Test Scenario: Guid Disallowed Value is Improperly Formatted (✗invalid✗)
            public class Union {
                [PrimaryKey, Check.IsNotOneOf("ee8982b5-bf0f-4fc1-9f16-29f197d91802", "b46cfa0c-545e-4279-93d6-d1236r373a2b")] public Guid UnionID { get; set; }
                public string UnionName { get; set; } = "";
                public string Industry { get; set; } = "";
                public decimal? Fee { get; set; }
                public uint Members { get; set; }
            }

            // Test Scenario: Disallowed Value of Source Type on Data-Converted Property (✗invalid✗)
            public class Guitar {
                [PrimaryKey] public Guid ID { get; set; }
                public decimal MarketValue { get; set; }
                public bool IsElectric { get; set; }
                [DataConverter(typeof(ToInt<string>)), Check.IsNotOneOf("Cardboard", 14, "Vend-O-Matic", "Allyzom")] public string Brand { get; set; } = "";
                public byte NumStrings { get; set; }
            }

            // Test Scenario: Disallowed Value of Target Type on Data-Converted Property (✓valid✓)
            public class SoccerTeam {
                [PrimaryKey] public string League { get; set; } = "";
                [PrimaryKey] public string Location { get; set; } = "";
                [PrimaryKey] public string Nickname { get; set; } = "";
                public ushort RosterSize { get; set; }
                [DataConverter(typeof(ToInt<short>)), Check.IsNotOneOf(0, -3, 111)] public short WorldCupVictories { get; set; }
                public string CurrentCoach { get; set; } = "";
                public string CurrentGoalie { get; set; } = "";
            }

            // Test Scenario: Both Boolean Values Disallowed (✗unsatisfiable✗)
            public class Transformer {
                [PrimaryKey] public string RobotName { get; set; } = "";
                public string VehicleForm { get; set; } = "";
                public uint TelevisionApperances { get; set; }
                public uint MovieAppearances { get; set; }
                [Check.IsNotOneOf(true, false)] public bool IsAutobot { get; set; }
            }

            // Test Scenario: All Enumerators Disallowed (✗unsatisfiable✗)
            public class ProgrammingLanguage {
                public enum Style { ObjectOriented, Functional, Logical, Assembly }

                [PrimaryKey] public string Name { get; set; } = "";
                [Check.IsNotOneOf(Style.ObjectOriented, Style.Functional, Style.Logical, Style.Assembly)] public Style Type { get; set; }
                public uint NumKeywords { get; set; }
                public double StackOverflowShare { get; set; }
                public string LatestStandard { get; set; } = "";
                public bool IsCompiled { get; set; }
            }

            // Test Scenario: Scalar Property Constrained Multiple Times (✓union✓)
            public class Eurovision {
                [PrimaryKey, Check.IsNotOneOf((ushort)3), Check.IsNotOneOf((ushort)0)] public ushort Year { get; set; }
                public uint ParticipatingCountries { get; set; }
                public string WinningCountry { get; set; } = "";
                public string WinningGroup { get; set; } = "";
                public string WinningSong { get; set; } = "";
            }

            // Test Scenario: <Path> is `null` (✗illegal✗)
            public class Tuxedo {
                [PrimaryKey] public Guid ProductID { get; set; }
                public string Brand { get; set; } = "";
                [Check.IsNotOneOf('X', 'Y', 'Z', Path = null!)] public char Size { get; set; }
                public decimal PriceTag { get; set; }
                public bool IncludesBowTie { get; set; }
                public ushort NumPockets { get; set; }
            }

            // Test Scenario: <Path> on Scalar Does Not Exist (✗non-existent path✗)
            public class Donut {
                [PrimaryKey] public Guid DonutID { get; set; }
                [Check.IsNotOneOf("Strawberry", "", "Unknown", Path = "---")] public string Flavor { get; set; } = "";
                public bool HasSprinkles { get; set; }
                public bool IsFilled { get; set; }
                public bool IsGlazed { get; set; }
                public bool IsDonutHole { get; set; }
            }

            // Test Scenario: <Path> on Aggregate Does Not Exist (✗non-existent path✗)
            public class Necktie {
                [Flags] public enum Color { Red = 1, Orange = 2, Yellow = 4, Green = 8, Blue = 16, Purple = 32, Pink = 64, Black = 128, White = 256, Gold = 512, Silver = 1024 }
                public record struct Dimension(double Length, uint Width);

                [PrimaryKey] public Guid ProductID { get; set; }
                public decimal Cost { get; set; }
                public Color Colors { get; set; }
                [Check.IsNotOneOf(10924.5152, Path = "---")] public Dimension Measurements { get; set; }
                public bool IsClipOn { get; set; }
            }

            // Test Scenario: <Path> on Aggregate Not Specified (✗missing path✗)
            public class Scattergories {
                public struct Page {
                    public uint Number { get; set; }
                    public string Category1 { get; set; }
                    public string Category2 { get; set; }
                    public string Category3 { get; set; }
                    public string Category4 { get; set; }
                    public string Category5 { get; set; }
                    public string Category6 { get; set; }
                    public string Category7 { get; set; }
                    public string Category8 { get; set; }
                    public string Category9 { get; set; }
                    public string Category10 { get; set; }
                }

                [PrimaryKey] public Guid GameID { get; set; }
                public char Letter { get; set; }
                [Check.IsNotOneOf('u', '=', 'F')] public Page Round { get; set; }
            }

            // Test Scenario: <Path> on Reference Does Not Exist (✗non-existent path✗)
            public class Pencil {
                public class Graphite {
                    [PrimaryKey] public Guid ID { get; set; }
                    public double Size { get; set; }
                    public bool Natural { get; set; }
                }

                [PrimaryKey] public Guid PencilID { get; set; }
                public bool IsMechanical { get; set; }
                [Check.IsNotOneOf('A', 'B', 'C', 'D', 'E', 'F', Path = "---")] public Graphite Lead { get; set; } = new();
                public string Brand { get; set; } = "";
                public bool BeenToSpace { get; set; }
            }

            // Test Scenario: <Path> on Reference Refers to Non-Primary-Key Field (✗non-existent path✗)
            public class Mitzvah {
                public class Verse {
                    [PrimaryKey] public string Book { get; set; } = "";
                    [PrimaryKey] public uint Chapter { get; set; }
                    [PrimaryKey] public uint Number { get; set; }
                    public string English { get; set; } = "";
                    public string Hebrew { get; set; } = "";
                }

                [PrimaryKey] public ushort Number { get; set; }
                [Check.IsNotOneOf("Bereshit Bara Elohim", Path = "Hebrew")] public Verse Commandment { get; set; } = new();
                public bool StillApplicable { get; set; }
            }

            // Test Scenario: <Path> on Reference Not Specified (✗missing path✗)
            public class Eunuch {
                public enum Culture { Chinese, Japanese, Vietnamese, Tibetan, Mesoamerican, European, Fictional }

                public class Person {
                    [PrimaryKey] public string FirstName { get; set; } = "";
                    [PrimaryKey] public string LastName { get; set; } = "";
                }

                [PrimaryKey] public string FirstName { get; set; } = "";
                [PrimaryKey] public string LastName { get; set; } = "";
                public Culture From { get; set; }
                public DateTime DateOfCastration { get; set; }
                [Check.IsNotOneOf(1.337, -98174.84, 0.0)] public Person Castrator { get; set; } = new();
                public bool IsNobleman { get; set; }
                public bool IsServant { get; set; }
            }

            // Test Scenario: <Path> on Relation Does Not Exist (✗non-existent path✗)
            public class PhoneBook {
                [PrimaryKey] public Guid ISBN { get; set; }
                public DateTime Published { get; set; }
                public string Region { get; set; } = "";
                public ulong PageCount { get; set; }
                [Check.IsNotOneOf(8471294811, 2056178955, 8005882340, Path = "---")] public RelationMap<string, ulong> PhoneNumbers { get; set; } = new();
                public double Weight { get; set; }
            }

            // Test Scenario: <Path> on Relation Refers to Non-Primary-Key Field of Anchor Entity (✗non-existent path✗)
            public class Bakugan {
                public enum Attribute { Pyrus, Aquos, Subterra, Haos, Darkus, Ventus, Aurelus }

                [PrimaryKey] public Guid ID { get; set; }
                public string BakuganName { get; set; } = "";
                public Attribute BakuganAttribute { get; set; }
                [Check.IsNotOneOf("Counterspell", "Basic Island", "Lightning Bolt", Path = "Bakugan.BakuganName")] public RelationSet<string> AbilityCards { get; set; } = new();
                public bool InAnime { get; set; }
            }

            // Test Scenario: <Path> on Relation Not Specified (✗missing path✗)
            public class QRCode {
                [PrimaryKey] public Guid ID { get; set; }
                public string URL { get; set; } = "";
                public RelationList<bool> Horizontal { get; set; } = new();
                [Check.IsNotOneOf(false)] public RelationList<bool> Vertical { get; set; } = new();
                public double Version { get; set; }
                public char ErrorCorrection { get; set; }
            }

            // Test Scenario: Default Value Does Not Satisfy Constraint (✗contradiction✗)
            public class Pie {
                [PrimaryKey] public int ID { get; set; }
                [Check.IsNotOneOf("Rhubarb", "Anise"), Default("Anise")] public string Flavor { get; set; } = "";
                public ushort Calories { get; set; }
                public float Diameter { get; set; }
                public bool IsSweet { get; set; }
                public string CrustIngredient { get; set; } = "";
            }

            // Test Scenario: Originally Valid Default Value No Longer Satisfies Constraint (✗contradiction✗)
            public class GirlScoutCookie {
                public struct Nutrition {
                    [Default(0.0f)] public float Calories { get; set; }
                    public double Carbohydrates { get; set; }
                    public double Sodium { get; set; }
                    public double SaturatedFat { get; set; }
                    public double Potassium { get; set; }
                    public double Fiber { get; set; }
                }

                [PrimaryKey] public Guid ID { get; set; }
                public string Variety { get; set; } = "";
                public DateTime Expiration { get; set; }
                public Guid PackageID { get; set; }
                public uint SoldByBadgeNumber { get; set; }
                [Check.IsNotOneOf(0.0f, Path = "Calories")] public Nutrition Label { get; set; }
            }
        }
    }

    internal static class CustomCheckConstraints {
        // Test Scenario: No Constructor Arguments (✓constrained✓)
        public class VampireSlayer {
            [PrimaryKey] public string Name { get; set; } = "";
            public uint Appearances { get; set; }
            public uint StakesUsed { get; set; }
            [Check(typeof(CustomCheck))] public ushort Deaths { get; set; }
            public bool ActivatedByScythe { get; set; }
        }

        // Test Scenario: Constructor Arguments (✓constrained✓)
        public class Lyric {
            [PrimaryKey] public string SongTitle { get; set; } = "";
            [PrimaryKey] public int LineNumber { get; set; }
            public string Lyrics { get; set; } = "";
            [Check(typeof(CustomCheck), 13, false, "ABC", null)] public bool IsSpoken { get; set; }
            public bool IsChorus { get; set; }
        }

        // Test Scenario: Applied to Aggregate-Nested Scalar (✓constrained✓)
        public class Asteroid {
            public struct OrbitalDescription {
                public double Aphelion { get; set; }
                public double Perihelion { get; set; }
                [Check(typeof(CustomCheck))] public float Eccentricity { get; set; }
            }

            [PrimaryKey] public string Name { get; set; } = "";
            [Check(typeof(CustomCheck), Path = "Aphelion")] public OrbitalDescription Orbit { get; set; }
            public DateTime Discovered { get; set; }
            public double Length { get; set; }
            public double Width { get; set; }
            public double Height { get; set; }
            public double Weight { get; set; }
        }

        // Test Scenario: Applied to Nested Aggregate (✗impermissible✗)
        public class Vineyard {
            public enum WineColor { Red, White, Rose }
            public record struct Vintage(string Name, ushort Year, double PercentAlcohol, WineColor Color);
            public record struct Bottling(Vintage Vintage, double Volume, bool Corked);

            [PrimaryKey] public string Name { get; set; } = "";
            public string Owner { get; set; } = "";
            public ulong Acreage { get; set; }
            [Check(typeof(CustomCheck), Path = "Vintage")] public Bottling SignatureWine { get; set; }
            public decimal AnnualRevenue { get; set; }
            public string Country { get; set; } = "";
        }

        // Test Scenario: Applied to Reference-Nested Scalar (✓constrained✓)
        public class Stock {
            public class Exchange {
                [PrimaryKey] public Guid ExchangeID { get; set; }
                public string GatewayTech { get; set; } = "";
                public DateTime Opening { get; set; }
                public decimal DailyVolume { get; set; }
            }
            public record struct Listing(Exchange Exchange, decimal Bid, decimal Ask, double MarketCap, ulong Shares);

            [PrimaryKey] public string Symbol { get; set; } = "";
            public string Company { get; set; } = "";
            public Listing? Chicago { get; set; }
            public Listing? NewYork { get; set; }
            public Listing? London { get; set; }
            [Check(typeof(CustomCheck), Path = "Exchange.ExchangeID")] public Listing? Sydney { get; set; }
            public Listing? HongKong { get; set; }
        }

        // Test Scenario: Applied to Nested Reference (✗impermissible✗)
        public class Werewolf {
            public enum Control { Lunar, Emotional, AtWill, Injurious }

            public class Lycanthropy {
                [PrimaryKey] public string Strain { get; set; } = "";
                public string ZoonoticOrigin { get; set; } = "";
                public bool Curable { get; set; }
                public Control MannerOfControl { get; set; }
            }
            public record struct Curse(Lycanthropy Source, DateTime Afflicted);

            [PrimaryKey] public string Name { get; set; } = "";
            [Check(typeof(CustomCheck), Path = "Source")] public Curse Lycan { get; set; }
            public ushort Weight { get; set; }
            public ulong Kills { get; set; }
        }

        // Test Scenario: Applied to Relation-Nested Scalar (✓constrained✓)
        public class CareBear {
            [PrimaryKey] public string Bear { get; set; } = "";
            public string Color { get; set; } = "";
            public char TummySymbol { get; set; }
            [Check(typeof(CustomCheck), Path = "Item")] public RelationSet<string> MediaAppearances { get; set; } = new();
            public string LeadDesigner { get; set; } = "";
        }

        // Test Scenario: Applied to Nested Relation (✗impermissible✗)
        public class RiverWalk {
            public record struct TimeRange(ushort Open, ushort Close);
            public record struct Schedule(TimeRange M, TimeRange TU, TimeRange W, TimeRange TH, TimeRange F, TimeRange SA, TimeRange SU, RelationSet<string> HolidaysClosed);

            [PrimaryKey] public Guid ID { get; set; }
            public string City { get; set; } = "";
            public string River { get; set; } = "";
            public uint NumShops { get; set; }
            public uint NumRestaurants { get; set; }
            [Check(typeof(CustomCheck), Path = "HolidaysClosed")] public Schedule Hours { get; set; }
            public decimal AnnualRevenue { get; set; }
            public ulong WalkLength { get; set; }
        }

        // Test Scenario: Scalar Property Constrained Multiple Times (✓both applied✓)
        public class TarotCard {
            [PrimaryKey] public int DeckID { get; set; }
            [PrimaryKey] public ushort CardNumber { get; set; }
            [Check(typeof(CustomCheck)), Check(typeof(CustomCheck), -14, '%')] public byte Pips { get; set; }
            public string Character { get; set; } = "";
        }

        // Test Scenario: Applied to Data-Converted Field (✓constrained✓)
        public class DataStructure {
            [PrimaryKey] public string Name { get; set; } = "";
            public string SearchBigO { get; set; } = "";
            public string InsertBigO { get; set; } = "";
            [DataConverter(typeof(ToInt<string>)), Check(typeof(CustomCheck))] public string RemoveBigO { get; set; } = "";
            public bool IsOrdered { get; set; }
            public bool IsAssociative { get; set; }
            public bool IsContiguous { get; set; }
        }

        // Test Scenario: Applied to Name-Changed Field (✓constrained✓)
        public class AronKodesh {
            public struct Door {
                public string Height { get; set; }
                public string Width { get; set; }
                public bool StainedGlass { get; set; }
            }

            [PrimaryKey] public Guid ArkID { get; set; }
            public ushort NumTorahs { get; set; }
            [Name("HeightOf", Path = "Height"), Check(typeof(CustomCheck), Path = "Height")] public Door LeftDoor { get; set; }
            public Door RightDoor { get; set; }
        }

        // Test Scenario: Constraint Generator is not an `IConstraintGenerator` (✗illegal✗)
        public class Patreon {
            [PrimaryKey] public string URL { get; set; } = "";
            public string Creator { get; set; } = "";
            public decimal Tier1 { get; set; }
            public decimal Tier2 { get; set; }
            [Check(typeof(NonSerializedAttribute))] public decimal Tier3 { get; set; }
        }

        // Test Scenario: Constraint Generator Cannot Be Default-Constructed (✗illegal✗)
        public class Seizure {
            [Flags] public enum Category { Epilleptic, GrandMal, Focal, Absence, Myoclonic, Partial }

            [PrimaryKey] public Guid SeizureID { get; set; }
            public double Duration { get; set; }
            [Check(typeof(PrivateCheck))] public string SufferedBy { get; set; } = "";
            public Category Kind { get; set; }
            public bool Fatal { get; set; }
        }

        // Test Scenario: Constraint Generator Cannot Be Complex-Constructed (✗illegal✗)
        public class Transistor {
            [PrimaryKey] public Guid ID { get; set; }
            public string Model { get; set; } = "";
            [Check(typeof(PrivateCheck), "Dopant", 4)] public string? Dopant { get; set; }
            public float Transconductance { get; set; }
            public int OperatingTemperature { get; set; }
        }

        // Test Scenario: Constraint Generator Throws Error upon Default Construction (✗propagated✗)
        public class Buffet {
            public enum Ethnicity { American, Chinese, Italian, Indian, German, Greek, Japanese, Korean, Mexican, Thai }

            [PrimaryKey] public Guid BuffetID { get; set; }
            public string Restaurant { get; set; } = "";
            [Check(typeof(UnconstructibleCheck))] public Ethnicity Cuisine { get; set; }
            public bool AllYouCanEat { get; set; }
            public decimal CostPerPerson { get; set; }
        }

        // Test Scenario: Constraint Generator Throws Error upon Complex Construction (✗propagated✗)
        public class BasketballPlayer {
            [PrimaryKey] public string Name { get; set; } = "";
            public ulong Points { get; set; }
            public float Pct3Pointer { get; set; }
            [Check(typeof(UnconstructibleCheck), false, 17UL)] public ulong Rebounds { get; set; }
            public ulong Steals { get; set; }
            public ulong Assists { get; set; }
        }

        // Test Scenario: Constraint Generator Throws Error when Creating Constraint (✗propagated✗)
        public class Aquarium {
            [PrimaryKey] public string AquariumName { get; set; } = "";
            public ulong NumSpecies { get; set; }
            public ulong TotalGallonsWater { get; set; }
            [Check(typeof(UnusableCheck))] public bool HasDolphins { get; set; }
            public decimal AdmissionPrice { get; set; }
            public bool AZA { get; set; }
        }

        // Test Scenario: <Path> is `null` (✗illegal✗)
        public class Trilogy {
            [PrimaryKey] public string Title { get; set; } = "";
            public string Entry1 { get; set; } = "";
            [Check(typeof(CustomCheck), Path = null!)] public string Entry2 { get; set; } = "";
            public string Entry3 { get; set; } = "";
        }

        // Test Scenario: <Path> on Scalar Does Not Exist (✗non-existent path✗)
        public class TarPits {
            [PrimaryKey] public string TarPitsName { get; set; } = "";
            public float Area { get; set; }
            [Check(typeof(CustomCheck), Path = "---")] public string FirstFossil { get; set; } = "";
            public bool IsNationalArea { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
        }

        // Test Scenario: <Path> on Aggregate Does Not Exist (✗non-existent path✗)
        public class StarCrossedLovers {
            [Flags] public enum Feud { Familial = 1, Religious = 2, Class = 4, SportsFandom = 8, Culinary = 16, Corporate = 32, Political = 64 }
            public record struct Name(string FirstName, char MiddleInitial, string LastName);

            public Name Lover1 { get; set; }
            [Check(typeof(CustomCheck), Path = "---")] public Name Lover2 { get; set; }
            [PrimaryKey] public string SourceMaterial { get; set; } = "";
            public Feud SeparationReason { get; set; }
        }

        // Test Scenario: <Path> on Aggregate Not Specified (✗missing path✗)
        public class Zombie {
            public enum Kind { Disease, Necromancy, Curse }
            public record struct Necrotization(double Percentage);

            [PrimaryKey] public Guid ZombieID { get; set; }
            public string? AliveName { get; set; }
            public Kind ZombieType { get; set; }
            public Necrotization Head { get; set; }
            public Necrotization Torso { get; set; }
            [Check(typeof(CustomCheck))] public Necrotization Legs { get; set; }
            public Necrotization Arms { get; set; }
        }

        // Test Scenario: <Path> on Reference Does Not Exist (✗non-existent path✗)
        public class Piano {
            public class Person {
                [PrimaryKey] public string FirstName { get; set; } = "";
                [PrimaryKey] public string LastName { get; set; } = "";
            }
            public class Company {
                [PrimaryKey] public string Name { get; set; } = "";
                public Person CEO { get; set; } = new();
                public ulong Employees { get; set; }
                public decimal Revenue { get; set; }
            }

            [PrimaryKey] public Guid PianoID { get; set; }
            public ulong Weight { get; set; }
            public byte BlackKeys { get; set; }
            public byte WhiteKeys { get; set; }
            [Check(typeof(Company), Path = "---")] public Company Manufacturer { get; set; } = new();
            public decimal MarketValue { get; set; }
            public bool IsReligious { get; set; }
        }

        // Test Scenario: <Path> on Reference Refers to Non-Primary-Key Field (✗non-existent path✗)
        public class Exorcism {
            [Flags] public enum Symptom { None = 0, Tongues = 1, Lesions = 2, NoPupils = 4, SpinningHead = 8, Levitation = 16, Necrosis = 32 }

            public class Possession {
                [PrimaryKey] public Guid ChurchRecord { get; set; }
                public string Demon { get; set; } = "";
                public DateTime Incipience { get; set; }
                public Symptom Symptoms { get; set; }
            }

            [PrimaryKey] public string Possessed { get; set; } = "";
            [PrimaryKey] public string Exorciser { get; set; } = "";
            [PrimaryKey] public DateTime When { get; set; }
            [Check(typeof(CustomCheck), Path = "Incipience")] public Possession Target { get; set; } = new();
            public bool Successful { get; set; }
            public bool Fatal { get; set; }
        }

        // Test Scenario: <Path> on Reference Not Specified (✗missing path✗)
        public class Pond {
            public class Coordinate {
                [PrimaryKey] public float Latitude { get; set; }
                [PrimaryKey] public float Longitude { get; set; }
            }

            [PrimaryKey] public Guid PondID { get; set; }
            public string Name { get; set; } = "";
            public double Depth { get; set; }
            public double Area { get; set; }
            public double AmountPondScum { get; set; }
            public ushort NumDucks { get; set; }
            public ushort NumFishes { get; set; }
            [Check(typeof(CustomCheck))] public Coordinate Location { get; set; } = new();
        }

        // Test Scenario: <Path> on Relation Does Not Exist (✗non-existent path✗)
        public class CanadianProvince {
            public enum Kind { Province, Territory }
            [Flags] public enum Language { English = 1, French = 2 }
            public record struct Reps(ushort HouseOfCommons, ushort Senate);
            
            public class City {
                [PrimaryKey] public string Name { get; set; } = "";
                public bool IsCapital { get; set; }
                public DateTime Founded { get; set; }
                public ulong Population { get; set; }
            }

            [PrimaryKey] public string Name { get; set; } = "";
            public Kind Classification { get; set; }
            [Check(typeof(CustomCheck), Path = "---")] public RelationList<City> Cities { get; set; } = new();
            public string PostalCode { get; set; } = "";
            public Language OfficialLanguages { get; set; }
            public Reps Representation { get; set; }
        }

        // Test Scenario: <Path> on Relation Refers to Non-Primary-Key Field of Anchor Entity (✗non-existent path✗)
        public class Skydiver {
            [Flags] public enum Vehicle { Plane = 1, Helicopter = 2, Blimp = 4, Glider = 8, Drone = 16, HotAirBalloon = 32, Zipline = 64 }

            [PrimaryKey] public Guid SkydiverID { get; set; }
            public string Name { get; set; } = "";
            public double Height { get; set; }
            public double Weight { get; set; }
            [Check(typeof(CustomCheck), Path = "Skydiver.Height")] public RelationMap<DateTime, long> Dives { get; set; } = new();
            public Vehicle HasJumpedFrom { get; set; }
        }

        // Test Scenario: <Path> on Relation Not Specified (✗missing path✗)
        public class Spring {
            [PrimaryKey] public Guid ID { get; set; }
            [Check(typeof(CustomCheck))] public RelationSet<string> ConstituentMetals { get; set; } = new();
            public double SpringConstant { get; set; }
            public ushort NumCoils { get; set; }
        }
    }

    internal static class ComplexCheckConstraints {
        // Test Scenario: No Constructor Arguments (✓constrained✓)
        [Check.Complex(typeof(CustomCheck), new[] { "FirstLine" })]
        public class CanterburyTale {
            [PrimaryKey] public int Index { get; set; }
            public string Whose { get; set; } = "";
            public string FirstLine { get; set; } = "";
            public ulong WordCount { get; set; }
        }

        // Test Scenario: Constructor Arguments (✓constrained✓)
        [Check.Complex(typeof(CustomCheck), new[] { "ConclaveRounds" }, -93, true, 'X')]
        public class Pope {
            [PrimaryKey] public string PapalName { get; set; } = "";
            [PrimaryKey] public uint PapalNumber { get; set; }
            public DateTime Elected { get; set; }
            public DateTime? Ceased { get; set; }
            public uint ConclaveRounds { get; set; }
            public string FirstEncyclical { get; set; } = "";
        }

        // Test Scenario: Covers Zero Fields (✗illegal✗)
        [Check.Complex(typeof(CustomCheck), new string[] {})]
        public class Terminator {
            [PrimaryKey] public string Model { get; set; } = "";
            [PrimaryKey] public ushort Number { get; set; }
            public ulong KillCount { get; set; }
            public string Portrayer { get; set; } = "";
            public DateTime FirstAppearance { get; set; }
        }

        // Test Scenario: Covers Multiple Distinct Fields (✓constrained✓)
        [Check.Complex(typeof(CustomCheck), new[] { "Major", "Minor", "Patch" })]
        public class LinuxDistribution {
            [PrimaryKey] public string Name { get; set; } = "";
            [PrimaryKey] public ulong Major { get; set; }
            [PrimaryKey] public ulong Minor { get; set; }
            [PrimaryKey] public ulong Patch { get; set; }
            public DateTime FirstReleased { get; set; }
            public string PackageManager { get; set; } = "";
            public bool IsOpenSource { get; set; }
        }

        // Test Scenario: Covers Single Field Multiple Times (✓constrained✓)
        [Check.Complex(typeof(CustomCheck), new[] { "Name", "Name", "Name", "Name" })]
        public class Muppet {
            [PrimaryKey] public string Name { get; set; } = "";
            public DateTime Debut { get; set; }
            public string Puppeteer { get; set; } = "";
            public ushort MuppetsShowAppearances { get; set; }
            public ushort MuppetsFilmAppearances { get; set; }
        }

        // Test Scenario: Covers Name-Swapped Fields (✓constrained✓)
        [Check.Complex(typeof(CustomCheck), new[] { "Cuisine", "ContainsTomatoes" })]
        public class PastaSauce {
            [PrimaryKey] public string Name { get; set; } = "";
            public bool IsMotherSauce { get; set; }
            public bool ContainsButter { get; set; }
            [Name("ContainsTomatoes")] public bool Cuisine { get; set; }
            public bool ContainsCheese { get; set; }
            public bool ContainsCream { get; set; }
            [Name("Cuisine")] public string ContainsTomatoes { get; set; } = "";
        }

        // Test Scenario: Covers Name-Changed Field with Original Name (✗illegal✗)
        [Check.Complex(typeof(CustomCheck), new string[] { "Width" })]
        public class Dam {
            [PrimaryKey] public Guid ID { get; set; }
            public string Name { get; set; } = "";
            public ulong Height { get; set; }
            [Name("WidthAtBase")] public ulong Width { get; set; }
            public ulong Volume { get; set; }
            public string ImpoundedRiver { get; set; } = "";
            public bool IsHydroelectric { get; set; }
        }

        // Test Scenario: Covers Unrecognized Field (✗illegal✗)
        [Check.Complex(typeof(CustomCheck), new string[] { "Belligerents" })]
        public class PeaceTreaty {
            [PrimaryKey] public string TreatyName { get; set; } = "";
            public DateTime Signed { get; set; }
            public DateTime Effective { get; set; }
            public string Text { get; set; } = "";
            public ushort NumSignatories { get; set; }
        }

        // Test Scenario: Covers Data-Converted Field (✓constrained✓)
        [Check.Complex(typeof(CustomCheck), new[] { "When", "Casualties", "When" })]
        public class Massacre {
            [PrimaryKey] public string Name { get; set; } = "";
            public ulong Casualties { get; set; }
            public bool WarCrime { get; set; }
            [DataConverter(typeof(Nullify<DateTime>))] public DateTime When { get; set; }
        }

        // Test Scenario: Applied to Single Entity Type Multiple Times (✓constrained✓)
        [Check.Complex(typeof(CustomCheck), new[] { "LengthMinutes" })]
        [Check.Complex(typeof(CustomCheck), new[] { "SungThrough" })]
        [Check.Complex(typeof(CustomCheck), new[] { "SungThrough" })]
        public class Musical {
            [PrimaryKey] public string Title { get; set; } = "";
            public bool SungThrough { get; set; }
            public ushort SongCount { get; set; }
            public ulong LengthMinutes { get; set; }
            public ushort TonyAwards { get; set; }
        }

        // Test Scenario: Constraint Generator is not an `IConstraintGenerator` (✗illegal✗)
        [Check.Complex(typeof(AssemblyLoadEventArgs), new[] { "Invisibility", "FirstIssue" })]
        public class Mutant {
            [PrimaryKey] public string CodeName { get; set; } = "";
            public string BirthName { get; set; } = "";
            public string Description { get; set; } = "";
            public bool Invisibility { get; set; }
            public bool Flight { get; set; }
            public bool Telekinesis { get; set; }
            public bool ElementalPowers { get; set; }
            public bool SuperSpeed { get; set; }
            public DateTime FirstIssue { get; set; }
            public uint Appearances { get; set; }
        }

        // Test Scenario: Constraint Generator Cannot Be Constructed (✗illegal✗)
        [Check.Complex(typeof(PrivateCheck), new[] { "Omega3s", "Omega6s" }, 'O', 'I', 'L', '!')]
        public class CookingOil {
            [PrimaryKey] public string Type { get; set; } = "";
            public decimal SmokePoint { get; set; }
            public double TransFats { get; set; }
            public double Omega3s { get; set; }
            public double Omega6s { get; set; }
        }

        // Test Scenario: Constraint Generator Throws Error upon Construction (✗propagated✗)
        [Check.Complex(typeof(UnconstructibleCheck), new[] { "Born", "Died" }, "Lifespan", 2918.01f, true)]
        public class Pirate {
            [PrimaryKey] public string PirateName { get; set; } = "";
            [PrimaryKey] public string LandName { get; set; } = "";
            public string Flagship { get; set; } = "";
            public DateTime? Born { get; set; }
            public DateTime? Died { get; set; }
            public ulong Plunder { get; set; }
            public bool IsFictional { get; set; }
        }

        // Test Scenario: Constraint Generator Throws Error when Creating Constraint (✗propagated✗)
        [Check.Complex(typeof(UnusableCheck), new[] { "Namespace", "ClassName" })]
        public class Attribute {
            [PrimaryKey] public string Namespace { get; set; } = "";
            [PrimaryKey] public string ClassName { get; set; } = "";
            public bool IsInherited { get; set; }
            public bool AllowMultiple { get; set; }
        }
    }

    internal static class MixedConstraints {
        // Test Scenario: [IsPositive] + [IsNonZero] (✓latter is redundant✓)
        public class Peninsula {
            [PrimaryKey] public string Name { get; set; } = "";
            [Check.IsPositive, Check.IsNonZero] public long Coastline { get; set; }
            public ulong Area { get; set; }
            public string Continent { get; set; } = "";
            public ulong Population { get; set; }
        }

        // Test Scenario: [IsNegative] + [IsNonZero] (✓latter is redundant✓)
        public class HTTPError {
            [PrimaryKey, Check.IsNegative, Check.IsNonZero] public int ErrorCode { get; set; }
            public string Title { get; set; } = "";
            public string Description { get; set; } = "";
            public bool IsDeprecated { get; set; }
        }

        // Test Scenario: [IsPositive] + [IsNegative] (✗unsatisfiable✗)
        public class Directory {
            [PrimaryKey] public string? Parent { get; set; } = "";
            [PrimaryKey] public string Name { get; set; } = "";
            public uint Owner { get; set; }
            [Check.IsPositive, Check.IsNegative] public short Mode { get; set; }
            public ulong SizeMb { get; set; }
        }

        // Test Scenario: [IsGreaterThan], [IsGreaterOrEqualTo], and [IsPositive] (✓constrained✓)
        public class ACT {
            [PrimaryKey] public string Taker { get; set; } = "";
            [PrimaryKey] public DateTime When { get; set; }
            public sbyte Composite { get; set; }
            [Check.IsGreaterThan((sbyte)10), Check.IsGreaterOrEqualTo((sbyte)10)] public sbyte Mathematics { get; set; }
            [Check.IsGreaterThan((sbyte)-13), Check.IsPositive] public sbyte Science { get; set; }
            [Check.IsGreaterThan((sbyte)4), Check.IsGreaterOrEqualTo((sbyte)9)] public sbyte Reading { get; set; }
            [Check.IsGreaterOrEqualTo((sbyte)8), Check.IsPositive] public sbyte English { get; set; }
        }

        // Test Scenario: [IsLessThan], [IsLessOrEqualTo], and [IsNegative] (✓constrained✓)
        public class Concert {
            [PrimaryKey] public Guid ConcertID { get; set; }
            public string Performer { get; set; } = "";
            public DateTime Date { get; set; }
            public string Location { get; set; } = "";
            [Check.IsLessThan(251U), Check.IsLessOrEqualTo(251U)] public uint Duration { get; set; }
            [Check.IsLessThan(73.0), Check.IsNegative] public decimal AverageTicketPrice { get; set; }
            [Check.IsLessThan(88871726), Check.IsLessOrEqualTo(28172831)] public int Attendees { get; set; }
            [Check.IsLessOrEqualTo((sbyte)11), Check.IsNegative] public sbyte Encores { get; set; }
        }

        // Test Scenario: <Comparisons> Lower Bound is less than Upper Bound (✓constrained✓)
        public class SlaveRevolt {
            [PrimaryKey] public Guid ID { get; set; }
            [Check.IsGreaterThan("Asmodeus"), Check.IsLessThan("Zylaphtanes")] public string Leader { get; set; } = "";
            [Check.IsGreaterOrEqualTo(-3), Check.IsLessOrEqualTo(20491486)] public int SlaveCasualties { get; set; }
            [Check.IsGreaterThan(4UL), Check.IsLessOrEqualTo(510492UL)] public ulong OwnerCasualties { get; set; }
            [Check.IsGreaterOrEqualTo("575-03-19"), Check.IsLessThan("8753-11-26")] public DateTime Date { get; set; }
        }

        // Test Scenario: <Comparisons> Inclusive Lower Bound is equal to Inclusive Upper Bound (✓constrained✓)
        public class Prescription {
            [PrimaryKey] public Guid ID { get; set; }
            public string AuthorizingDoctor { get; set; } = "";
            [Check.IsGreaterOrEqualTo("Bastioquiloquine"), Check.IsLessOrEqualTo("Bastioquiloquine")] public string Medication { get; set; } = "";
            public string Pharmacy { get; set; } = "";
            public DateTime FillBy { get; set; }
            [Check.IsGreaterOrEqualTo((byte)2), Check.IsLessOrEqualTo((byte)2)] public byte Refills { get; set; }
        }

        // Test Scenario: <Comparisons> Inclusive Lower Bound is equal to Exclusive Upper Bound (✗unsatisfiable✗)
        public class Genie {
            [PrimaryKey] public string Identifier { get; set; } = "";
            [Check.IsGreaterOrEqualTo(1), Check.IsLessThan(1)] public int NumWishes { get; set; }
            public bool IsFriendly { get; set; }
            public string PrincipalForm { get; set; } = "";
        }

        // Test Scenario: <Comparisons> Exclusive Lower Bound is equal to Inclusive Upper Bound (✗unsatisfiable✗)
        public class ComicCon {
            [PrimaryKey] public DateTime When { get; set; }
            [PrimaryKey] public string City { get; set; } = "";
            public decimal TicketPrice { get; set; }
            [Check.IsGreaterThan(275U), Check.IsLessOrEqualTo(275U)] public uint NumPanels { get; set; }
        }

        // Test Scenario: <Comparisons> Exclusive Lower Bound is equal to Exclusive Upper Bound (✗unsatisfiable✗)
        public class CabinetDepartment {
            [PrimaryKey] public string Department { get; set; } = "";
            public string Secretary { get; set; } = "";
            public DateTime Established { get; set; }
            public ulong Employees { get; set; }
            [Check.IsGreaterThan(481723.5f), Check.IsLessThan(481723.5f)] public float Budget { get; set; }
            public ushort LineOfSuccession { get; set; }
        }

        // Test Scenario: <Comparisons> Lower Bound is larger than Upper Bound (✗unsatisfiable✗)
        public class Locale {
            [PrimaryKey] public string Language { get; set; } = "";
            [PrimaryKey] public string Territory { get; set; } = "";
            [PrimaryKey, Check.IsLessOrEqualTo("ASCII"), Check.IsGreaterThan("UTF-7")] public string CodeSet { get; set; } = "";
            [PrimaryKey] public string Modifier { get; set; } = "";
        }

        // Test Scenario: [IsNot] with Anchor that is Not in Valid Range (✓redundant✓)
        public class Calendar {
            [PrimaryKey] public string Name { get; set; } = "";
            [Check.IsLessThan(21U), Check.IsNot(473U)] public uint NumMonths { get; set; }
            public bool IsLunar { get; set; }
            public ulong EpochInUnixTime { get; set; }
        }

        // Test Scenario: [LengthIsAtLeast] + [IsNonEmpty] (✓constrained✓)
        public class StepPyramid {
            [PrimaryKey] public Guid ID { get; set; }
            public ulong Steps { get; set; }
            public bool IsZiggurat { get; set; }
            [Check.IsNonEmpty, Check.LengthIsAtLeast(5)] public string? KnownAs { get; set; }
            [Check.IsNonEmpty, Check.LengthIsAtLeast(0)] public string Civilization { get; set; } = "";
            public ushort? ApproximateAge { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
        }

        // Test Scenario: [LengthIsAtMost] + [IsNonEmpty] (✓constrained✓)
        public class UIButton {
            [PrimaryKey, Check.LengthIsAtMost(20), Check.IsNonEmpty] public string ComponentID { get; set; } = "";
            public ushort Height { get; set; }
            public ushort Width { get; set; }
            public int PosX { get; set; }
            public int PosY { get; set; }
            public string DisplayText { get; set; } = "";
            public bool HasOnClickCallback { get; set; }
        }

        // Test Scenario: [LengthIsBetween] + [IsNonEmpty] (✓constrained✓)
        public class Cave {
            [PrimaryKey] public float Latitude { get; set; }
            [PrimaryKey] public float Longitude { get; set; }
            [Check.IsNonEmpty, Check.LengthIsBetween(75412, 12981147)] public string Name { get; set; } = "";
            [Check.IsNonEmpty, Check.LengthIsBetween(0, 74)] public string? ManagingOrg { get; set; }
            public double Length { get; set; }
            public byte Entrances { get; set; }
            public double Depth { get; set; }
        }

        // Test Scenario: [LengthIsAtLeast] Bound is no larger than [LengthIsAtMost] Bound (✓constrained✓)
        public class Vulgarity {
            [PrimaryKey] public string English { get; set; } = "";
            public string Censorship { get; set; } = "";
            public bool IsSevenDirtyWord { get; set; }
            [Check.LengthIsAtLeast(18), Check.LengthIsAtMost(197)] public string Spanish { get; set; } = "";
            [Check.LengthIsAtLeast(14981), Check.LengthIsAtMost(14981)] public string French { get; set; } = "";
            public float Prevalence { get; set; }
        }

        // Test Scenario: [LengthIsAtLeast] Bound is larger than [LengthIsAtMost] Bound (✗unsatisfiable✗)
        public class Generation {
            [PrimaryKey, Check.LengthIsAtLeast(153), Check.LengthIsAtMost(111)] public string Name { get; set; } = "";
            public ushort StartingYear { get; set; }
            public ushort? EndingYear { get; set; }
            public ulong Population { get; set; }
        }

        // Test Scenario: [LengthIsAtLeast] Bound is no larger than [LengthIsBetween] Upper Bound (✓constrained✓)
        public class WitchHunt {
            [PrimaryKey, Check.LengthIsAtLeast(7), Check.LengthIsBetween(15, 30)] public string Name { get; set; } = "";
            public DateTime Start { get; set; }
            public DateTime End { get; set; }
            [Check.LengthIsAtLeast(19), Check.LengthIsBetween(19, 50)] public string Leader { get; set; } = "";
            [Check.LengthIsAtLeast(309), Check.LengthIsBetween(2, 12000000)] public string? FirstVictim { get; set; }
            public ulong WitchesBurned { get; set; }
            [Check.LengthIsAtLeast(100), Check.LengthIsBetween(27, 100)] public string ExecutionMethod { get; set; } = "";
        }

        // Test Scenario: [LengthIsAtLeast] Bound is larger than [LengthIsBetween] Upper Bound (✗unsatisfiable✗)
        public class Integral {
            [PrimaryKey] public string From { get; set; } = "";
            [PrimaryKey] public string To { get; set; } = "";
            [PrimaryKey, Check.LengthIsAtLeast(555), Check.LengthIsBetween(3, 22)] public string Expression { get; set; } = "";
            [PrimaryKey] public string WithRespectTo { get; set; } = "";
        }

        // Test Scenario: [LengthIsAtMost] Bound is no less than [LengthIsBetween] Lower Bound (✓constrained✓)
        public class Isthmus {
            [PrimaryKey] public Guid ID { get; set; }
            [Check.LengthIsAtMost(413), Check.LengthIsBetween(22, 1994)] public string Name { get; set; } = "";
            [Check.LengthIsAtMost(1), Check.LengthIsBetween(1, 5)] public string StarRating { get; set; } = "";
            [Check.LengthIsAtMost(35), Check.LengthIsBetween(18, 35)] public string? MostPopulousCity { get; set; } = "";
            [Check.LengthIsAtMost(122), Check.LengthIsBetween(110, 113)] public string? Management { get; set; } = "";
            public ulong Length { get; set; }
            public bool IsSpannedByCanal { get; set; }
        }

        // Test Scenario: [LengthIsAtMost] Bound is less than [LengthIsBetween] Lower Bound (✗unsatisfiable✗)
        public class NuGetPackage {
            [PrimaryKey] public Guid PackageID { get; set; }
            public string PackageName { get; set; } = "";
            [Check.LengthIsAtMost(10), Check.LengthIsBetween(15, 19)] public string Version { get; set; } = "";
            public string Author { get; set; } = "";
            public DateTime Published { get; set; }
            public ulong Downloads { get; set; }
            public string License { get; set; } = "";
        }

        // Test Scenario: [IsNot] with Anchor that is Not of a Valid Length (✓redundant✓)
        public class LicensePlate {
            [PrimaryKey, Check.LengthIsBetween(1, 8), Check.IsNotOneOf("UNB98X1FF5")] public string PlateNumber { get; set; } = "";
            public bool IsVanity { get; set; }
            public DateTime ExpiresOn { get; set; }
            public string Registrant { get; set; } = "";
        }

        // Test Scenario: [IsOneOf] + [IsNotOneOf] with Disjoint Sets of Values (✓latter is redundant✓)
        public class SkiSlope {
            [PrimaryKey] public Guid ID { get; set; }
            public uint Height { get; set; }
            [Check.IsOneOf("Black Diamond", "Novice"), Check.IsNotOneOf("Unknown")] public string Level { get; set; } = "";
            public string SkiResort { get; set; } = "";
            public bool BeginnerFriendly { get; set; }
        }

        // Test Scenario: [IsOneOf] + [IsNotOneOf] with Overlapping Sets of Values (✓set difference✓)
        public class AmusementPark {
            [PrimaryKey] public uint ID { get; set; }
            [Check.IsOneOf("Six Flags", "Disney World", "Universal Studios"), Check.IsNotOneOf("Disneyland", "Disney World")] public string Name { get; set; } = "";
            public short NumRollercoasters { get; set; }
            public decimal Admission { get; set; }
            public DateTime Opened { get; set; }
            public bool IsWaterPark { get; set; }
        }

        // Test Scenario: [IsOneOf] + [IsNotOneOf] with Equivalent Sets of Values (✗unsatisfiable✗)
        public class MarkUpSymbol {
            [PrimaryKey, Check.IsOneOf('*', '_', '`', '+'), Check.IsNotOneOf('*', '_', '`', '+')] public char Symbol { get; set; }
            public string Description { get; set; } = "";
            public bool IsSupportedByCommonMark { get; set; }
        }

        // Test Scenario: [IsOneOf] + <Comparisons> with At Least One Allowed Value (✓former, evaluated against latter✓)
        public class TicTacToeGame {
            [PrimaryKey] public Guid GameID { get; set; }
            [Check.IsGreaterOrEqualTo('B'), Check.IsOneOf('O', 'X', 'Y')] public char? TopLeft { get; set; }
            [Check.IsGreaterThan('R'), Check.IsOneOf('O', 'X', 'Y')] public char? TopCenter { get; set; }
            public char? TopRight { get; set; }
            [Check.IsNot('X'), Check.IsOneOf('O', 'X', 'Y')] public char? MiddleLeft { get; set; }
            [Check.IsNot('A'), Check.IsOneOf('O', 'X', 'Y')] public char? MiddleCenter { get; set; }
            public char? MiddleRight { get; set; }
            public char? BottomLeft { get; set; }
            [Check.IsGreaterOrEqualTo('X'), Check.IsLessThan('Z'), Check.IsOneOf('O', 'X', 'Y')] public char? BottomCenter { get; set; }
            [Check.IsLessThan('^'), Check.IsOneOf('O', 'X', 'Y')] public char? BottomRight { get; set; }
        }

        // Test Scenario: [IsOneOf] + <Lengths> with At Least One Allowed Value (✓former, evaluated against latter✓)
        public class Printer {
            [PrimaryKey] public Guid PrinterID { get; set; }
            [Check.LengthIsAtLeast(3), Check.IsOneOf("AO", "Hijack", "Mecha-Print")] public string Brand { get; set; } = "";
            public float CyanLevel { get; set; }
            public float MagentaLevel { get; set; }
            public float YellowLevel { get; set; }
            public float BlackLevel { get; set; }
            [Check.LengthIsBetween(4, 19), Check.IsOneOf("S", "M", "L", "XL", "XXL", "Poster Board")] public string PaperSize { get; set; } = "";
        }

        // Test Scenario: [IsOneOf] + <Comparisons> leaves No Allowed Values (✗unsatisfiable✗)
        public class Aqueduct {
            [PrimaryKey] public int GuidID { get; set; }
            public string? Name { get; set; }
            public float Latitude { get; set; }
            public float Longitude { get; set; }
            public ushort Length { get; set; }
            [Check.IsGreaterOrEqualTo(450.3), Check.IsOneOf(200.1, 13.6237, 450.087798)] public double WaterVolume { get; set; }
        }

        // Test Scenario: [IsOneOf] + <Lengths> leaves No Allowed Values (✗unsatisfiable✗)
        public class SurvivorSeason {
            [PrimaryKey] public uint Number { get; set; }
            [Check.LengthIsBetween(2, 8), Check.IsOneOf("Madagascar", "The Serengeti")] public string Location { get; set; } = "";
            public string Winner { get; set; } = "";
            public string Host { get; set; } = "";
            public byte NumEpisodes { get; set; }
        }

        // Test Scenario: [IsOneOf] + <Comparisons> + <Lengths> leaves No Allowed Values (✗unsatisfiable✗)
        public class NorseGod {
            [PrimaryKey] public Guid ID { get; set; }
            [Check.IsGreaterOrEqualTo("Yggradsil"), Check.LengthIsBetween(5, 23), Check.IsOneOf("Thor", "Heimdallr", "Ymir")] public string Name { get; set; } = "";
            public uint ProseEddaMentions { get; set; }
            public uint PoeticEddaMentions { get; set; }
            public bool IsAesir { get; set; }
        }

        // Test Scenario: [IsNotOneOf] + <Comparisons> (✓former, evaluated against latter✓)
        public class PostageStamp {
            [PrimaryKey] public uint ID { get; set; }
            [Check.IsGreaterOrEqualTo(0.05), Check.IsNotOneOf(0.05, 0.7712)] public double Length { get; set; }
            [Check.IsGreaterThan(0.01), Check.IsLessThan(2.37), Check.IsNotOneOf(-2.226, 1.335, 2.012, 2.37)] public double Height { get; set; }
            [Check.IsLessOrEqualTo(1.00), Check.IsNotOneOf(0.25, 0.50, 0.75, 1.00, 1.25, 1.50)] public double Cost { get; set; }
            public string? Design { get; set; }
            public bool IsForeverStamp { get; set; }
        }

        // Test Scenario: [IsNotOneOf] + <Lengths> (✓former, evaluated against latter✓)
        public class Obelisk {
            [PrimaryKey] public Guid ID { get; set; }
            public ushort Height { get; set; }
            [Check.LengthIsAtLeast(9), Check.IsNotOneOf("Limestone", "Marble", "Reinforced Steel")] public string Material { get; set; } = "";
            public DateTime? Created { get; set; }
            public DateTime? Discovered { get; set; }
            public bool Hollow { get; set; }
        }

        // Test Scenario: Single-Valued <Comparisons> Disallowed by [IsNotOneOf] (✗unsatisfiable✗)
        public class Beach {
            [PrimaryKey] public Guid BeachID { get; set; }
            public string Name { get; set; } = "";
            [Check.IsGreaterOrEqualTo(75UL), Check.IsLessOrEqualTo(75UL), Check.IsNotOneOf(75UL)] public ulong Coastline { get; set; }
            public bool ClothingRequired { get; set; }
            public bool IsWhiteSand { get; set; }
            public bool IsSurfSpot { get; set; }
        }

        // Test Scenario: [Numeric] + <Signedness> (✓former, evaluated against latter✓)
        public class Soup {
            public enum Kind { Standard = 14, Bisque = -2, Chowder = 177, Stew = 90 }
            public enum YesNo { Yes = 1, No = -1, Maybe = 0 }
            public enum Protein { Chicken = -8124, Beef = -4, Venison = -99, Rabbit = 185, Seafood = 26667 }

            [PrimaryKey] public string Name { get; set; } = "";
            [Numeric, Check.IsPositive] public Kind Variety { get; set; }
            [Numeric, Check.IsNonZero] public YesNo HasNoodles { get; set; }
            [Numeric, Check.IsNegative] public Protein BrothProtein { get; set; }
            public uint CaloriesPerServing { get; set; }
            public bool ServedHot { get; set; }
        }

        // Test Scenario: [Numeric] + <Comparisons> (✓former, evaluated against latter✓)
        public class CavePainting {
            public enum PaintMaterial { Chalk, Blood, NaturalDye, Graphite, GemDust, Etching }

            [PrimaryKey] public string Cave { get; set; } = "";
            [PrimaryKey] public uint CatalogNumber { get; set; }
            [Check.IsGreaterThan(2), Check.IsLessOrEqualTo(4), Numeric] public PaintMaterial Material { get; set; }
            public double Area { get; set; }
            public DateTime Discovered { get; set; }
            public ulong Age { get; set; }
            public string Description { get; set; } = "";
        }

        // Test Scenario: [Numeric] + <Discreteness> (✓former, evaluated against latter✓)
        public class Triangle {
            [Flags] public enum Variety { Equilateral = 1, Right = 2, Isosceles = 4, Scalene = 8, RightIsosceles = Right | Isosceles, RightScalene = Right | Scalene }

            [PrimaryKey] public Guid ID { get; set; }
            public double X1 { get; set; }
            public double Y1 { get; set; }
            public double X2 { get; set; }
            public double Y2 { get; set; }
            public double X3 { get; set; }
            public double Y3 { get; set; }
            [Check.IsOneOf(0, 1, 2, 4, 8, 6, 10, 17, 186, 222), Numeric] public Variety Kind { get; set; }
        }

        // Test Scenario: [Numeric] + [IsOneOf] Allowing Only Non-Domain Values (✗unsatisfiable✗)
        public class DSM {
            public enum Number { One, Two, Three, Four, Five }

            [PrimaryKey] public ulong ISBN { get; set; }
            public DateTime Published { get; set; }
            [Numeric, Check.IsOneOf(6, 7, 8, 9)] public Number Edition { get; set; } 
            public ushort DisordersCatalogued { get; set; }
            public bool APA_Approved { get; set; }
            public ulong WordCount { get; set; }
        }

        // Test Scenario: [AsString] + <Comparisons> (✓former, evaluated against latter✓)
        public class Casino {
            public enum Game { Blackjack, Poker, Craps, Roulette, Slots, Baccarat, Pachinko, Bingo, SportsBetting }

            [PrimaryKey] public string Name { get; set; } = "";
            public string? Proprietor { get; set; }
            public decimal CashOnHand { get; set; }
            public double FloorArea { get; set; }
            [Check.IsGreaterOrEqualTo("Cards"), Check.IsLessThan("Safety"), AsString] public Game TopMoneyMaker { get; set; }
        }

        // Test Scenario: [AsString] + <Lengths> (✓former, evaluated against latter✓)
        public class FacebookPost {
            public enum Viz { Public, Private, FriendsOnly, FriendsOfFriends, Subscribers }

            [PrimaryKey] public Guid PostID { get; set; }
            public ulong PosterID { get; set; }
            public string Content { get; set; } = "";
            public uint Likes { get; set; }
            public uint Comments { get; set; }
            [Check.LengthIsBetween(7, 15), AsString] public Viz Visibility { get; set; }
        }

        // Test Scenario: [AsString] + <Discreteness> (✓former, evaluated against latter✓)
        public class ZodiacSign {
            public enum Season : short { Winter, Spring, Summer, Autumn }

            [PrimaryKey] public string Name { get; set; } = "";
            [Check.IsNotOneOf("Winter", "Spring", "Fall", "Year-Round"), AsString] public Season SignSeason { get; set; }
            public char ZodiacSymbol { get; set; }
            public string HinduSolarEquivalent { get; set; } = "";
        }

        // Test Scenario: [AsString] + [IsOneOf] Allowing Only Non-Domain Values (✗unsatisfiable✗)
        public class ParkingTicket {
            public enum Classification { PermitOnly, FireHydrant, MeterExpired, DoubleParked, Weather }

            [PrimaryKey] public uint TicketNumber { get; set; }
            public Guid IssuingOfficerBadgeNumber { get; set; }
            [AsString, Check.IsOneOf("Missing Sticker", "Flashing Lights")] public Classification Reason { get; set; }
            public decimal Fine { get; set; }
            public string LicensePlate { get; set; } = "";
            public DateTime DateOfInfraction { get; set; }
            public bool CourtDateRequired { get; set; }
        }

        // Test Scenario: <Comparison> on Nested Field is Altered (✓former, evaluated against latter✓)
        public class TribeOfIsrael {
            public struct Citation {
                [Check.IsGreaterOrEqualTo("Bible")] public string Book { get; set; }
                [Check.IsLessThan((byte)100)] public byte Chapter { get; set; }
                [Check.IsGreaterThan(0L)] public long Verse { get; set; }
            }

            public sbyte Index { get; set; }
            [PrimaryKey] public string Name { get; set; } = "";
            public string Hebrew { get; set; } = "";
            public string Arabic { get; set; } = "";
            [Check.IsLessOrEqualTo("Qur'an", Path = "Book"), Check.IsOneOf((byte)1, (byte)2, (byte)3, (byte)153, Path = "Chapter")] public Citation FirstMentioned { get; set; }
            public bool Priests { get; set; }
            public ulong LandArea { get; set; }
        }

        // Test Scenario: <Signedness> on Nested Field is Altered (✓former, evaluated against latter✓)
        public class SecretPolice {
            public struct Record {
                [Check.IsNonZero] public int Arrests { get; set; }
                [Check.IsPositive] public double Murders { get; set; }
                [Check.IsPositive] public decimal Bribes { get; set; }
                [Check.IsNonZero] public sbyte YearsActive { get; set; }
            }

            [PrimaryKey] public string Endonym { get; set; } = "";
            public string Exonym { get; set; } = "";
            public string CreatedBy { get; set; } = "";
            [Check.IsNotOneOf(7, 196, 4410905, Path = "Arrests"), Check.IsLessThan(195385.96, Path = "Murders"), Check.IsNot(80.0, Path = "Bribes"), Check.IsOneOf((sbyte)10, (sbyte)20, (sbyte)30, Path = "YearsActive")] public Record Statistics { get; set; }
            public bool Wartime { get; set; }
        }

        // Test Scenario: <Lengths> on Nested Field is Altered (✓former, evaluated against latter✓)
        public class SearchEngine {
            public struct URL {
                public string Scheme { get; set; }
                public string Path { get; set; }
                [Check.LengthIsBetween(1, 3)] public string Domain { get; set; }
            }

            [PrimaryKey, Check.IsOneOf("com", "tv", "gov", "net", "arpa", Path = "Domain")] public URL LandingPage { get; set; }
            public bool UsesCookies { get; set; }
            public ulong RequestsPerDay { get; set; }
            public float AverageResponseTime { get; set; }
            public bool SupportsMobile { get; set; }
        }

        // Test Scenario: <Discreteness> on Nested Field is Altered (✓former, evaluated against latter✓)
        public class BeninBronze {
            public enum Unit { Inches, Centimeters }

            public struct Dimension {
                [Check.IsOneOf(1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 9.0, 10.0)] public double Length { get; set; }
                public Unit LengthUnit { get; set; }
                [Check.IsNotOneOf(1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 9.0, 10.0)] public double Width { get; set; }
                public Unit WidthUnit { get; set; }
            }

            [PrimaryKey] public uint Number { get; set; }
            public string? Title { get; set; }
            public bool Repatriated { get; set; }
            [Check.IsGreaterOrEqualTo(3.75, Path = "Length"), Check.IsNotOneOf(11.0, 12.0, 13.0, Path = "Width")] public Dimension Dimensions { get; set; }
        }
    }

    internal static class ReferenceCycles {
        // Test Scenario: Self-Referential Entity with Cycle Length of 1 (✗non-acyclic✗)
        public class Constitution {
            [PrimaryKey] public string Country { get; set; } = "";
            [PrimaryKey] public DateTime Ratified { get; set; }
            public sbyte Articles { get; set; }
            public sbyte Amendments { get; set; }
            public Constitution? Precursor { get; set; }
            public string FullText { get; set; } = "";
        }

        // Test Scenario: Self-Referential Entity with Cycle Length of 2 (✗non-acyclic✗)
        public class Niqqud {
            [Flags] public enum Feature { Front = 1, Central = 2, Back = 4, High = 8, Low = 16, Voiceless = 32, Breathy = 64, Nasal = 128, Creaky = 256 }

            public class Vowel {
                [PrimaryKey] public char IPA { get; set; }
                public Feature Features { get; set; }
                public Niqqud? HebrewSymbol { get; set; }
            }

            [PrimaryKey] public char Symbol { get; set; }
            public Vowel Pronunciation { get; set; } = new();
            public bool StilInUse { get; set; }
            public double RelativeFrequency { get; set; }
            public ulong TorahAppearances { get; set; }
        }

        // Test Scenario: Self-Referential Entity with Cycle Length > 2 (✗non-acyclic✗)
        public class RefugeeCamp {
            public class CivilWar {
                [PrimaryKey] public Guid WarID { get; set; }
                public string BelligerentA { get; set; } = "";
                public string BelligerentB { get; set; } = "";
                public RefugeeCamp LargestRefugeeCamp { get; set; } = new();
            }
            public class Country {
                [PrimaryKey] public string Name { get; set; } = "";
                public string DomainSuffix { get; set; } = "";
                public ulong Population { get; set; }
                public ulong Area { get; set; }
                public CivilWar? OngoingCivilWar { get; set; }
            }
            public class Person {
                [PrimaryKey] public uint SSN { get; set; }
                public string FirstName { get; set; } = "";
                public string LastName { get; set; } = "";
                public Country BirthCountry { get; set; } = new();
            }

            [PrimaryKey] public Guid CampID { get; set; }
            public ulong NumRefugees { get; set; }
            public Person Director { get; set; } = new();
            public bool IsWartime { get; set; }
        }

        // Test Scenario: Self-Referential Relation via Direct Element (✓allowed✓)
        public class SoftwarePackage {
            [PrimaryKey] public string PackageManager { get; set; } = "";
            [PrimaryKey] public string Hash { get; set; } = "";
            public string Name { get; set; } = "";
            public string Version { get; set; } = "";
            public RelationMap<string, bool> Flags { get; set; } = new();
            public RelationSet<SoftwarePackage> BuildDependencies { get; set; } = new();
            public RelationSet<SoftwarePackage> RunDependencies { get; set; } = new();
            public bool Secure { get; set; }
        }

        // Test Scenario: Self-Referential Relation via Aggregate Element (✓allowed✓)
        public class Indictment {
            public enum Level { Local, State, Federal, International }
            public enum Category { Infraction, Misdemeanor, Felony }
            public record struct Charge(Category Classification, string Statute, uint Counts, Indictment CarriedBy);

            [PrimaryKey] public ulong IndictmentNumber { get; set; }
            [PrimaryKey] public string Defendant { get; set; } = "";
            public DateTime Issued { get; set; }
            public RelationList<Charge> Charges { get; set; } = new();
            public Level Government { get; set; }
            public bool? Conviction { get; set; }
        }

        // Test Scenario: Self-Referential Relation via Reference Element (✓allowed✓)
        public class StackFrame {
            public class Breakpoint {
                [PrimaryKey] public string FileName { get; set; } = "";
                [PrimaryKey] public uint LineNumber { get; set; }
                public bool IsConditional { get; set; }
                public StackFrame? Frame { get; set; }
            }

            [PrimaryKey] public Guid ID { get; set; }
            public sbyte FrameNumber { get; set; }
            public RelationList<Breakpoint> Breakpoints { get; set; } = new();
            public string Debugger { get; set; } = "";
            public double Memory { get; set; }
        }

        // Test Scenario: Entity X → Entity Y via Reference → Entity X via Relation (✓allowed✓)
        public class Filibuster {
            public class Politician {
                [PrimaryKey] public string FullName { get; set; } = "";
                public DateTime FirstElected { get; set; }
                public RelationSet<Filibuster> FilibustersBroken { get; set; } = new();
            }

            [PrimaryKey] public Guid FilibusterID { get; set; }
            public string Legislation { get; set; } = "";
            public DateTime Date { get; set; }
            public double Duration { get; set; }
            public Politician Instigator { get; set; } = new();
            public bool Successful { get; set; }
        }
    }
}
