﻿using Kvasir.Annotations;
using Kvasir.Localization;
using Kvasir.Relations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using static UT.Kvasir.Translation.TestConstraints;
using static UT.Kvasir.Translation.TestConverters;

namespace UT.Kvasir.Translation {
    internal static class PropertyTypes {
        // Test Scenario: Non-Nullable Scalars (✓recognized✓)
        public class Smorgasbord {
            public byte Byte { get; set; }
            public char Char { get; set; }
            public DateOnly Date { get; set; }
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
            public DateOnly? Date { get; set; }
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

        // Test Scenario: By-Ref Property (✗not permitted✗)
        public class Spider {
            [PrimaryKey] public string Genus { get; set; } = "";
            [PrimaryKey] public string Species { get; set; } = "";
            public string CommonName { get; set; } = "";
            public ref bool Venomous { get { return ref venomous_; } }
            public ushort NumEyes { get; set; }

            private bool venomous_;
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
            public RelationList<string> Files { get; init; } = new();
            public RelationSet<Macro> Macros { get; init; } = new();
            public RelationMap<Mode, int> OptimizationLevel { get; init; } = new();
            public RelationOrderedList<string> LinkAgainst { get; init; } = new();
        }

        // Test Scenario: Nullable Relations of Non-Nullable Elements (✓recognized✓)
        public class Forecast {
            public enum Extremity { Hurricane, Tornado, Thunderstorm, Blizzard, Hailstorm, Sandstorm }
            public record struct SingleDay(DateTime Date, float HighTemp, float LowTemp, double ChanceRain);

            [PrimaryKey] public string City { get; set; } = "";
            public ulong PersonsImpacted { get; set; }
            public RelationList<SingleDay>? Dailies { get; init; }
            public RelationSet<string>? Meteorologists { get; init; }
            public RelationMap<string, bool>? DataSources { get; init; }
            public RelationOrderedList<Extremity>? ExtremeWeather { get; init; }
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
            public IReadOnlyRelationList<Terrain> AllowedTerrain { get; init; } = new RelationList<Terrain>();
            public IReadOnlyRelationSet<CivVIBuilding> Buildings { get; init; } = new RelationSet<CivVIBuilding>();
            public IReadOnlyRelationMap<int, Yield> Yields { get; init; } = new RelationMap<int, Yield>();
            public IReadOnlyRelationOrderedList<string> Icons { get; init; } = new RelationOrderedList<string>();
        }

        // Test Scenario: Relations Nested Within Aggregates (✓recognized✓)
        public class Gelateria {
            public struct Owning {
                public DateTime Since { get; set; }
                public IReadOnlyRelationSet<string> People { get; init; }
                public decimal LifetimeRevenue { get; set; }
            }

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
            public RelationSet<string> AKAs { get; init; } = new();
            public RelationMap<string, RelationMap<string, double>> Measurements { get; init; } = new();
        }

        // Test Scenario: Relation Nested Within Localization (✗not permitted✗)
        public class Retrovirus {
            public enum Class { Oncoretrovirus, Lentivirus, Spumavirus }

            [PrimaryKey] public Guid VirusID { get; set; }
            public Class Variety { get; set; }
            public double Incidence { get; set; }
            public Localization<int, string, RelationList<string>> Name { get; } = new(0);
        }

        // Test Scenario: Relation Nested Within Relation Nested Within Aggregate (✗not permitted✗)
        public class IntelligenceAgency {
            public struct Leadership {
                public DateTime EffectiveStart { get; set; }
                public DateTime? EffectiveEnd { get; set; }
                public RelationMap<string, RelationSet<string>> Roles { get; }
            }

            [PrimaryKey] public string Name { get; set; } = "";
            [PrimaryKey] public string Country { get; set; } = "";
            public Leadership Board { get; set; }
            public uint KnownAssassinations { get; set; }
            public bool? PerformsCyberEspionage { get; set; }
            public decimal Budget { get; set; }
            public ulong Employees { get; set; }
        }

        // Test Scenario: Relation Nested Within Localization Nested Within Aggregate (✗not permitted✗)
        public class Bodybuilder {
            public struct Biography {
                public DateOnly Birthday { get; set; }
                public string SSN { get; set; }
                public Localization<string, sbyte, RelationSet<double>> Measurements { get; }
            }

            [PrimaryKey] public Guid ID { get; set; }
            public Biography Bio { get; set; }
            public float BMI { get; set; }
            public bool Olympia { get; set; }
        }

        // Test Scenario: Relation Nested Within Aggregate Nested Within Relation (✗not permitted✗)
        public class Poll {
            public record struct Question(string Text, RelationSet<string> Answers);

            [PrimaryKey] public Guid PollID { get; set; }
            public string PollTitle { get; set; } = "";
            public string? Pollster { get; set; }
            public RelationList<Question> Questions { get; init; } = new();
            public ulong Responses { get; set; }
            public double ReponseRate { get; set; }
        }

        // Test Scenario: Relation Nested Within Aggregate Nested Within Localization (✗not permitted✗)
        public class Macro {
            public record struct Argument(byte Index, string Token, IReadOnlyRelationSet<string> AllowedTypes);

            [PrimaryKey] public string File { get; set; } = "";
            [PrimaryKey] public string Name { get; set; } = "";
            public Localization<char, string, Argument> Arguments { get; } = new('\0');
            public bool IsInOverloadSet { get; set; }
            public bool StandardsCompliant { get; set; }
        }

        // Test Scenario: Relation Nested Within Aggregate Nested Within Relation, post-Memoization (✗not permitted✗)
        public class Quinceanera {
            public record struct Gift {
                public string Description { get; set; }
                public decimal Price { get; set; }
                public RelationSet<string> Adjectives { get; }
            }

            [PrimaryKey] public Guid PartyID { get; set; }
            public DateTime Date { get; set; }
            public string BirthdayGirl { get; set; } = "";
            public Gift BestGift { get; set; }
            public RelationMap<string, Gift> Presents { get; } = new();
            public bool InMexico { get; set; }
        }

        // Test Scenario: Relation Nested Within Aggregate Nested Within Localization, post-Memoization (✗not permitted✗)
        public class Parable {
            public enum Version { KingJames, Wicked, Septuagint, Vaticanus }
            public record struct Citation(string Book, byte Chapter, RelationOrderedList<uint> Verses);

            [PrimaryKey] public Guid StoryID { get; set; }
            public string? Title { get; set; }
            public Citation Incipience { get; set; }
            public Localization<string, Version, Citation> Mentions { get; } = new("");
            public bool HistoricallyAttested { get; set; }
        }

        // Test Scenario: Relation List/Set of KeyValuePair<X, Y> (✗not permitted - implementation ambiguity✗)
        public class Caricature {
            public enum Location { Circus, Zoo, AmusementPark, SportingEvent, FarmersMarket, Other }

            [PrimaryKey] public Guid ID { get; set; }
            public string Subject { get; set; } = "";
            public string Artist { get; set; } = "";
            public RelationSet<KeyValuePair<DateTime, decimal>> SaleHistory { get; init; } = new();
            public double HeadSize { get; set; }
            public bool Certified { get; set; }
            public Location Source { get; set; }
        }

        // Test Scenario: Relation List/Set of Tuple<X, Y, Z> (✗not permitted - implementation ambiguity✗)
        public class StirFry {
            [PrimaryKey] public Guid ID { get; set; }
            public string? Variety { get; set; }
            public RelationOrderedList<Tuple<int, string, string>> Ingredients { get; } = new();
            public double Calories { get; set; }
            public bool MadeInWok { get; set; }
            public double Scovilles { get; set; }
        }

        // Test Scenario: IRelation (✗not permitted✗)
        public class Perfume {
            [PrimaryKey] public string Brand { get; set; } = "";
            [PrimaryKey] public string Aroma { get; set; } = "";
            public decimal RetailValue { get; set; }
            public DateTime Launched { get; set; }
            public bool ForWomen { get; set; }
            public IRelation PatentNumbers { get; init; } = new RelationSet<ulong>();
        }

        // Test Scenario: Writeable Relation (✗not permitted✗)
        public class BugZapper {
            [PrimaryKey] public Guid ProductID { get; set; }
            public decimal RetailPrice { get; set; }
            public RelationSet<string> SusceptibleSpecies { get; set; } = new();
            public bool IsScented { get; set; }
            public float MaxVoltage { get; set; }
        }

        // Test Scenario: Aggregate Consisting of Only Relations (✓recognized✓)
        public class Loch {
            public record struct Geography {
                public RelationMap<char, float> Coordinates { get; }
                public IReadOnlyRelationSet<string> Shires { get; }
            }

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

    internal static class PreDefinedEntities {
        // Test Scenario: Zero Identified Instances (✗minimum 2 required✗)
        [PreDefined] public class StainedGlassWindow {
            [PrimaryKey] public Guid ID { get; private init; }
            public ushort NumPanes { get; private init; }
            public double SurfaceArea { get; private init; }
            public bool IsReligious { get; private init; }
            public double Transparency { get; private init; }
            public decimal InstallationCost { get; private init; }
        }

        // Test Scenario: One Identified Instance (✗minimum 2 required✗)
        [PreDefined] public class Gulf {
            [PrimaryKey] public string Name { get; private init; }
            public float SurfaceArea { get; private init; }
            public float MaxDepth { get; private init; }
            public string PrimaryInflow { get; private init; }

            public static Gulf Aqaba { get; } = new Gulf("Gulf of Aqaba", 239, 1850, "Red Sea");

            private Gulf(string name, float surfaceArea, float maxDepth, string primaryInflow) {
                Name = name;
                SurfaceArea = surfaceArea;
                MaxDepth = maxDepth;
                PrimaryInflow = primaryInflow;
            }
        }

        // Test Scenario: Two or More Identified Instances (✓allowed✓)
        [PreDefined] public class Disciple {
            [PrimaryKey] public string Name { get; private init; }
            public string? Epithet { get; private init; }
            public DateTime? FeastDay { get; private init; }

            public static Disciple SimonI { get; } = new Disciple("Simon Peter", null, new DateTime(2000, 6, 29));
            public static Disciple Andrew { get; } = new Disciple("Andrew", "Simon's brother", new DateTime(2000, 11, 30));
            public static Disciple JamesI { get; } = new Disciple("James the Great", "son of Zebedee", new DateTime(2000, 7, 25));
            public static Disciple John { get; } = new Disciple("John", "James's brother", new DateTime(2000, 12, 27));
            public static Disciple Philip { get; } = new Disciple("Philip", null, new DateTime(2000, 5, 3));
            public static Disciple Bartholomew { get; } = new Disciple("Batholomew", null, new DateTime(2000, 8, 24));
            public static Disciple Thomas { get; } = new Disciple("Thomas", "Didymus", new DateTime(2000, 7, 3));
            public static Disciple Matthew { get; } = new Disciple("Matthew", "the publican", new DateTime(2000, 9, 21));
            public static Disciple JamesII { get; } = new Disciple("James", "son of Alphaeus", new DateTime(2000, 5, 3));
            public static Disciple Thaddeus { get; } = new Disciple("Thaddeus", null, new DateTime(2000, 10, 28));
            public static Disciple SimonII { get; } = new Disciple("Simon", "the Canaanite", new DateTime(2000, 10, 28));
            public static Disciple Judas { get; } = new Disciple("Judas Iscariot", null, null);

            private Disciple(string name, string? epithet, DateTime? feastDay) {
                Name = name;
                Epithet = epithet;
                FeastDay = feastDay;
            }
        }

        // Test Scenario: Publicly Writeable Field Properties (✗not permitted✗)
        [PreDefined] public class Olive {
            [PrimaryKey] public string CommonName { get; private init; }
            public double CaloriePer100g { get; private init; }
            public sbyte OilContent { get; set; }
            public string CountryOfOrigin { get; private init; }

            public static Olive Kalamata { get; } = new Olive("Kalamata", 284, 6, "Greece");
            public static Olive Aglandau { get; } = new Olive("Aglandau", 187, 3, "France");
            public static Olive Tirilye { get; } = new Olive("Tirilye", 261, 6, "Turkey");
            public static Olive Picual { get; } = new Olive("Picual", 135, 2, "Spain");
            public static Olive Manzanilla { get; } = new Olive("Manzanilla", 279, 8, "Spain");

            private Olive(string commonName, double calories, sbyte oilContent, string countryOfOrigin) {
                CommonName = commonName;
                CaloriePer100g = calories;
                OilContent = oilContent;
                CountryOfOrigin = countryOfOrigin;
            }
        }

        // Test Scenario: Non-Publicly Writeable Field Properties (✗not permitted✗)
        [PreDefined] public class Cloud {
            public enum Layer { Mesosphere, Stratosphere, Troposphere, Any }

            [PrimaryKey] public string Name { get; private init; }
            public string Abbreviation { get; protected internal set; }
            public Layer AtmosphericLayer { get; private init; }

            public static Cloud Cirrus { get; } = new Cloud("Cirrus", "Ci", Layer.Troposphere);
            public static Cloud Stratus { get; } = new Cloud("Stratus", "St", Layer.Any);
            public static Cloud Cumulonimbus { get; } = new Cloud("Cumulonimbus", "Cb", Layer.Any);
            public static Cloud Cumulus { get; } = new Cloud("Cumulus", "Cu", Layer.Any);
            public static Cloud Fog { get; } = new Cloud("Fog", "Fg", Layer.Mesosphere);

            private Cloud(string name, string abbr, Layer layer) {
                Name = name;
                Abbreviation = abbr;
                AtmosphericLayer = layer;
            }
        }

        // Test Scenario: Publicly Writeable Instance Properties (✗not permitted✗)
        [PreDefined] public class LayerOfSkin {
            [PrimaryKey] public uint Number { get; private init; }
            public string Name { get; private init; }
            public string MeSH { get; private init; }

            public static LayerOfSkin Epidermis { get; } = new LayerOfSkin(0, "Epidermis", "D004817");
            public static LayerOfSkin Dermis { get; } = new LayerOfSkin(1, "Dermis", "D020405");
            public static LayerOfSkin Hypodermis { get; set; } = new LayerOfSkin(2, "Hypodermis", "D040521");

            private LayerOfSkin(uint number, string name, string mesh) {
                Number = number;
                Name = name;
                MeSH = mesh;
            }
        }

        // Test Scenario: Non-Publicly Writeable Instance Property (✓allowed✓)
        [PreDefined] public class PunctuationMark {
            [PrimaryKey] public char Character { get; private init; }
            public string Name { get; private init; }

            public static PunctuationMark QuestionMark { get; private set; }
            public static PunctuationMark ExclamationMark { get; private set; }
            public static PunctuationMark Period { get; private set; }
            public static PunctuationMark Comma { get; private set; }
            public static PunctuationMark QuotationMark { get; private set; }
            public static PunctuationMark Apostrophe { get; private set; }
            public static PunctuationMark Hyphen { get; private set; }
            public static PunctuationMark OpenParenthesis { get; private set; }
            public static PunctuationMark CloseParenthesis { get; private set; }

            private PunctuationMark(char character, string name) {
                Character = character;
                Name = name;
            }
            static PunctuationMark() {
                QuestionMark = new PunctuationMark('?', "Question Mark");
                ExclamationMark = new PunctuationMark('!', "Exclamation Mark");
                Period = new PunctuationMark('.', "Period");
                Comma = new PunctuationMark(',', "Comma");
                QuotationMark = new PunctuationMark('"', "Quotation Mark");
                Apostrophe = new PunctuationMark('\'', "Apostrophe");
                Hyphen = new PunctuationMark('-', "Hyphen");
                OpenParenthesis = new PunctuationMark('(', "Open Parenthesis");
                CloseParenthesis = new PunctuationMark(')', "Close Parenthesis");
            }
        }

        // Test Scenario: Public Constructor (✗not permitted✗)
        [PreDefined] public class FederalReserveDistrict {
            [PrimaryKey, Column(0)] public int Number { get; private init; }
            [Column(1)] public string BankHQ { get; private init; }
            [Column(2)] public DateTime Established { get; private init; }

            public static FederalReserveDistrict First { get; } = new FederalReserveDistrict(1, "Boston", new DateTime(1914, 5, 18));
            public static FederalReserveDistrict Second { get; } = new FederalReserveDistrict(2, "New York City", new DateTime(1914, 11, 16));
            public static FederalReserveDistrict Third { get; } = new FederalReserveDistrict(3, "Philadelphia", new DateTime(1914, 5, 18));
            public static FederalReserveDistrict Fourth { get; } = new FederalReserveDistrict(4, "Cleveland", new DateTime(1914, 5, 18));
            public static FederalReserveDistrict Fifth { get; } = new FederalReserveDistrict(5, "Richmond", new DateTime(1914, 5, 18));
            public static FederalReserveDistrict Sixth { get; } = new FederalReserveDistrict(6, "Atlanta", new DateTime(1914, 5, 18));
            public static FederalReserveDistrict Seventh { get; } = new FederalReserveDistrict(7, "Chicago", new DateTime(1914, 5, 18));
            public static FederalReserveDistrict Eighth { get; } = new FederalReserveDistrict(8, "St. Louis", new DateTime(1914, 5, 18));
            public static FederalReserveDistrict Ninth { get; } = new FederalReserveDistrict(9, "Minneapolis", new DateTime(1914, 5, 18));
            public static FederalReserveDistrict Tenth { get; } = new FederalReserveDistrict(10, "Kansas City", new DateTime(1914, 5, 18));
            public static FederalReserveDistrict Eleventh { get; } = new FederalReserveDistrict(11, "Dallas", new DateTime(1914, 5, 18));
            public static FederalReserveDistrict Twelfth { get; } = new FederalReserveDistrict(12, "San Francisco", new DateTime(1914, 5, 18));

            public FederalReserveDistrict(int number, string hq, DateTime established) {
                Number = number;
                BankHQ = hq;
                Established = established;
            }
        }

        // Test Scenario: Reference to Pre-Defined Entity (✓allowed✓)
        [PreDefined] public class RavnicaGuild {
            [PreDefined] public class Color {
                [PrimaryKey] public byte R { get; private init; }
                [PrimaryKey] public byte G { get; private init; }
                [PrimaryKey] public byte B { get; private init; }

                public static Color Blue { get; } = new Color() { R = 0, G = 0, B = 255 };
                public static Color Red { get; } = new Color() { R = 255, G = 0, B = 0 };
                public static Color Green { get; } = new Color() { R = 0, G = 255, B = 0 };
                public static Color White { get; } = new Color() { R = 255, G = 255, B = 255 };
                public static Color Black { get; } = new Color() { R = 0, G = 0, B = 0 };

                private Color() {}
            }
            [PreDefined] public class Mana {
                [PrimaryKey] public uint ID { get; private init; }
                public Color? Color { get; private init; }
                public string BasicLand { get; private init; }

                public static Mana Blue { get; } = new Mana(0, Color.Blue, "island");
                public static Mana Red { get; } = new Mana(1, Color.Red, "mountain");
                public static Mana Green { get; } = new Mana(2, Color.Green, "forest");
                public static Mana White { get; } = new Mana(3, Color.White, "plains");
                public static Mana Black { get; } = new Mana(4, Color.Black, "swamp");
                public static Mana Colorless { get; } = new Mana(5, null, "wastes");
                public static Mana Phyrexian { get; } = new Mana(6, null, "life");

                private Mana(uint id, Color? color, string basic) {
                    ID = id;
                    Color = color;
                    BasicLand = basic;
                }
            }

            [PrimaryKey] public string Name { get; private init; }
            public string GuildHall { get; private init; }
            public string Parun { get; private init; }
            public Mana FirstMana { get; private init; }
            public Mana SecondMana { get; private init; }

            public static RavnicaGuild Azorius { get; } = new RavnicaGuild("Azorius Senate", "Prahv", "Azor I", Mana.White, Mana.Blue);
            public static RavnicaGuild Dimir { get; } = new RavnicaGuild("House Dimir", "Duskmantle", "Lazav", Mana.Blue, Mana.Black);
            public static RavnicaGuild Rakdos { get; } = new RavnicaGuild("Cult of Rakdos", "Rix Maadi", "Rakdos", Mana.Black, Mana.Red);
            public static RavnicaGuild Gruul { get; } = new RavnicaGuild("Gruul Clans", "Skarrg", "Cisarzim", Mana.Red, Mana.Green);
            public static RavnicaGuild Selesnya { get; } = new RavnicaGuild("Selesnya Conclave", "Vitu-Ghazi", "Mat'Selesnya", Mana.Green, Mana.White);
            public static RavnicaGuild Orzhov { get; } = new RavnicaGuild("Orzhov Syndicate", "Orzhova", "Obzedat", Mana.White, Mana.Black);
            public static RavnicaGuild Izzet { get; } = new RavnicaGuild("Izzet League", "Nivix", "Niv-Mizzet", Mana.Blue, Mana.Red);
            public static RavnicaGuild Golgari { get; } = new RavnicaGuild("Golgari Swarm", "Svogthos", "Svogthir", Mana.Black, Mana.Green);
            public static RavnicaGuild Boros { get; } = new RavnicaGuild("Boros Legion", "Sunhomme", "Razia", Mana.Red, Mana.White);
            public static RavnicaGuild Simic { get; } = new RavnicaGuild("Simic Combine", "Novijen", "Simic", Mana.Green, Mana.Blue);

            private RavnicaGuild(string name, string hall, string parun, Mana first, Mana second) {
                Name = name;
                GuildHall = hall;
                Parun = parun;
                FirstMana = first;
                SecondMana = second;
            }
        }

        // Test Scenario: Aggregate-Nested Reference to Pre-Defined Entity (✓allowed✓)
        [PreDefined] public class MarxBrother {
            [PreDefined] public class Month {
                [PrimaryKey] public uint Index { get; private init; }
                public string Name { get; private init; }

                public static Month January { get; } = new Month(1, "January");
                public static Month February { get; } = new Month(2, "February");
                public static Month March { get; } = new Month(3, "March");
                public static Month April { get; } = new Month(4, "April");
                public static Month May { get; } = new Month(5, "May");
                public static Month June { get; } = new Month(6, "June");
                public static Month July { get; } = new Month(7, "July");
                public static Month August { get; } = new Month(8, "August");
                public static Month September { get; } = new Month(9, "September");
                public static Month October { get; } = new Month(10, "October");
                public static Month Novemeber { get; } = new Month(11, "November");
                public static Month December { get; } = new Month(12, "December");

                private Month(uint index, string name) {
                    Index = index;
                    Name = name;
                }
            }

            public readonly record struct Date(byte Day, Month Month, ushort Year);

            [PrimaryKey] public string Name { get; private init; }
            public Date BirthDate { get; private init; }
            public string FilmDebut { get; private init; }

            public static MarxBrother Chico { get; } = new MarxBrother("Chico Marx", new Date(22, Month.March, 1887), "The Cocoanuts");
            public static MarxBrother Harpo { get; } = new MarxBrother("Harpo Marx", new Date(23, Month.Novemeber, 1888), "Humor Risk");
            public static MarxBrother Groucho { get; } = new MarxBrother("Groucho Marx", new Date(2, Month.October, 1890), "Humor Risk");
            public static MarxBrother Gummo { get; } = new MarxBrother("Gummo Marx", new Date(23, Month.October, 1892), "n/a");
            public static MarxBrother Zeppo { get; } = new MarxBrother("Zeppo Marx", new Date(25, Month.February, 1901), "Humor Risk");

            private MarxBrother(string name, Date dob, string debut) {
                Name = name;
                BirthDate = dob;
                FilmDebut = debut;
            }
        }

        // Test Scenario: Relation to Pre-Defined Entity (✓allowed✓)
        [PreDefined] public class ResidentEvil {
            [PreDefined] public class GamingMode {
                [PrimaryKey] public uint ModeID { get; private init; }
                public string Mode { get; private init; }

                public static GamingMode SinglePlayer { get; } = new GamingMode(8172, "Single-Player");
                public static GamingMode MultiPlayer { get; } = new GamingMode(9182005, "Multi-Player");
                public static GamingMode Online { get; } = new GamingMode(61, "Online");

                private GamingMode(uint id, string mode) {
                    ModeID = id;
                    Mode = mode;
                }
            }

            [PrimaryKey] public ulong GameID { get; private init; }
            public string Title { get; private init; }
            public DateTime ReleaseDate { get; private init; }
            public RelationSet<GamingMode> Modes { get; }

            public static ResidentEvil Original { get; } = new ResidentEvil(41099, "Resident Evil", new DateTime(1996, 3, 22));
            public static ResidentEvil Two { get; } = new ResidentEvil(875, "Resident Evil 2", new DateTime(1998, 1, 21));
            public static ResidentEvil Nemesis { get; } = new ResidentEvil(6710294, "Resident Evil 3: Nemesis", new DateTime(1999, 9, 22));
            public static ResidentEvil CodeVeronica { get; } = new ResidentEvil(765, "Resident Evil - Code: Veronica", new DateTime(2000, 2, 3));
            public static ResidentEvil Zero { get; } = new ResidentEvil(6120, "Resident Evil Zero", new DateTime(2002, 11, 12));
            public static ResidentEvil Four { get; } = new ResidentEvil(4, "Resident Evil 4", new DateTime(2005, 1, 11));
            public static ResidentEvil Five { get; } = new ResidentEvil(81008, "Resident Evil 5", new DateTime(2009, 3, 5));
            public static ResidentEvil Six { get; } = new ResidentEvil(9511361672, "Resident Evil 6", new DateTime(2012, 10, 2));
            public static ResidentEvil Biohazard { get; } = new ResidentEvil(7561200, "Resident Evil 7: Biohazard", new DateTime(2017, 1, 24));
            public static ResidentEvil Village { get; } = new ResidentEvil(5405, "Resident Evil Village", new DateTime(2021, 5, 7));

            private ResidentEvil(ulong id, string title, DateTime release) {
                GameID = id;
                Title = title;
                ReleaseDate = release;
                Modes = new RelationSet<GamingMode>() { GamingMode.SinglePlayer, GamingMode.MultiPlayer };
            }
        }

        // Test Scenario: Aggregate-Nested Relation to Pre-Defined Entity (✓allowed✓)
        [PreDefined] public class CitrusFruit {
            [PreDefined] public class TaxonomicRank {
                [PrimaryKey] public char Symbol { get; private init; }
                public string Name { get; private init; }

                public static TaxonomicRank Kingdom { get; } = new TaxonomicRank('K', "kingdom");
                public static TaxonomicRank Phylum { get; } = new TaxonomicRank('P', "phylum");
                public static TaxonomicRank Class { get; } = new TaxonomicRank('C', "class");
                public static TaxonomicRank Order { get; } = new TaxonomicRank('O', "order");
                public static TaxonomicRank Family { get; } = new TaxonomicRank('F', "family");
                public static TaxonomicRank Genus { get; } = new TaxonomicRank('G', "genus");
                public static TaxonomicRank Species { get; } = new TaxonomicRank('S', "species");

                private TaxonomicRank(char symbol, string name) {
                    Symbol = symbol;
                    Name = name;
                }
            }

            public readonly struct BioData {
                public bool IsHybrid { get; init; }
                public IReadOnlyRelationMap<TaxonomicRank, string> Taxonomy { get; init; }
            }

            [PrimaryKey] public uint ID { get; private init; }
            public string CommonName { get; private init; }
            public BioData Bio { get; private init; }

            public static CitrusFruit Kumquat { get; } = new CitrusFruit(0, "kumqut", false, "margarita");
            public static CitrusFruit Mandarin { get; } = new CitrusFruit(1, "mandarin", false, "reticulata");
            public static CitrusFruit Citron { get; } = new CitrusFruit(2, "citron", false, "medica");
            public static CitrusFruit Pomelo { get; } = new CitrusFruit(3, "pomelo", false, "maxima");
            public static CitrusFruit Micrantha { get; } = new CitrusFruit(4, "micrantha", false, "micrantha");
            public static CitrusFruit Grapefruit { get; } = new CitrusFruit(5, "grapefruit", true, "× paradisi");
            public static CitrusFruit Lemon { get; } = new CitrusFruit(6, "lemon", true, "× limon");
            public static CitrusFruit Lime { get; } = new CitrusFruit(7, "lime", true, "× latifolia");
            public static CitrusFruit Mangshanyegan { get; } = new CitrusFruit(8, "mangshanyegan", false, "mangshanensis");
            public static CitrusFruit Papeda { get; } = new CitrusFruit(9, "papeda", false, "cavalerieri");
            public static CitrusFruit Tangerine { get; } = new CitrusFruit(10, "tagerine", true, "× tangerina");
            public static CitrusFruit Orange { get; } = new CitrusFruit(11, "orange", true, "× sinensis");
            public static CitrusFruit Tangelo { get; } = new CitrusFruit(12, "tangelo", true, "× tangelo");
            public static CitrusFruit Satsuma { get; } = new CitrusFruit(13, "satsuma", false, "unshiu");
            public static CitrusFruit BuddhasHand { get; } = new CitrusFruit(14, "Buddha's hand", false, "media var. sarcodactylis");
            public static CitrusFruit Clymenia { get; } = new CitrusFruit(15, "clymenia", false, "platypoda");
            public static CitrusFruit Clementine { get; } = new CitrusFruit(16, "clementine", true, "× clementina");
            public static CitrusFruit Jabara { get; } = new CitrusFruit(17, "jabara", true, "× jabara");
            public static CitrusFruit Yuzu { get; } = new CitrusFruit(18, "yuzu", true, "× junos");

            private CitrusFruit(uint id, string commonName, bool isHybrid, string species) {
                ID = id;
                CommonName = commonName;
                Bio = new BioData() {
                    IsHybrid = isHybrid,
                    Taxonomy = new RelationMap<TaxonomicRank, string>() {
                        { TaxonomicRank.Kingdom, "Plantae" },
                        { TaxonomicRank.Order, "Sapindales" },
                        { TaxonomicRank.Family, "Rutaceae" },
                        { TaxonomicRank.Genus, "Citrus" },
                        { TaxonomicRank.Species, species }
                    }
                };
            }
        }

        // Test Scenario: Reference to Non-Pre-Defined Entity (✗not permitted✗)
        [PreDefined] public class CapnCrunch {
            public class Date {
                [PrimaryKey] public byte Day { get; set; }
                [PrimaryKey] public byte Month { get; set; }
                [PrimaryKey] public byte Year { get; set; }
            }

            [PrimaryKey] public Guid ID { get; private init; }
            public string Name { get; private init; }
            public bool Discontinued { get; private init; }
            public Date FirstReleased { get; private init; }

            public static CapnCrunch Regular { get; } = new CapnCrunch("Cap'n Crunch", false);
            public static CapnCrunch CrunchBerries { get; } = new CapnCrunch("Cap'n Crunch Crunch Berries", false);
            public static CapnCrunch PeanutButter { get; } = new CapnCrunch("Peanut Butter Crunch", false);
            public static CapnCrunch AllBerries { get; } = new CapnCrunch("Oops! All Berries", false);
            public static CapnCrunch Chocolate { get; } = new CapnCrunch("Chocolatey Crunch", true);

            private CapnCrunch(string name, bool discontinued) {
                ID = Guid.NewGuid();
                Name = name;
                Discontinued = discontinued;
                FirstReleased = new Date();
            }
        }

        // Test Scenario: Aggregate-Nested Reference to Non-Pre-Defined Entity (✗not permitted✗)
        [PreDefined] public class StageOfGrief {
            public class Psychologist {
                [PrimaryKey] public Guid MedicalID { get; set; }
                public string FirstName { get; set; } = "";
                public string LastName { get; set; } = "";
                public ushort Publications { get; set; }
                public bool IsPracticing { get; set; }
            }
            
            public readonly record struct Article(string Title, DateTime PublishedOn, Psychologist Author);

            [PrimaryKey] public byte Index { get; private init; }
            public string Name { get; private init; }
            public Article? ProposedIn { get; private init; }

            public static StageOfGrief Denial { get; } = new StageOfGrief(0, "Denial");
            public static StageOfGrief Anger { get; } = new StageOfGrief(1, "Anger");
            public static StageOfGrief Bargaining { get; } = new StageOfGrief(2, "Bargaining");
            public static StageOfGrief Depression { get; } = new StageOfGrief(3, "Depression");
            public static StageOfGrief Acceptance { get; } = new StageOfGrief(4, "Acceptance");

            private StageOfGrief(byte index, string name) {
                Index = index;
                Name = name;
                ProposedIn = null;
            }
        }

        // Test Scenario: Aggregate-Nested Reference to Non-Pre-Defined Entity, post-Memoization (✗not permitted✗)
        [PreDefined] public class Stooge {
            public class Film {
                [PrimaryKey] public ulong IMDbID { get; set; }
                public string Title { get; set; } = "";
                public DateTime ReleaseDate { get; set; }
                public bool IsSilent { get; set; }
            }
            public class ProductionCompany {
                [PrimaryKey] public Guid ID { get; set; }
                public string Name { get; set; } = "";
                public DateTime Founded { get; set; }
                public FilmDeal MostLucrativeDeal { get; set; }
            }

            public readonly record struct FilmDeal(decimal Budget, decimal Revenue, Film FirstFilm);

            [PrimaryKey] public string Name { get; private init; }
            public DateTime DateOfBirth { get; private init; }
            public bool WasMainstay { get; private init; }
            public FilmDeal InitialDeal { get; private init; }

            public static Stooge Larry { get; } = new Stooge("Larry Fine", new DateTime(1902, 10, 4), true);
            public static Stooge Moe { get; } = new Stooge("Moe Howard", new DateTime(1897, 6, 19), true);
            public static Stooge Curly { get; } = new Stooge("Curly Howard", new DateTime(1903, 10, 22), false);

            private Stooge(string name, DateTime dob, bool mainstay) {
                Name = name;
                DateOfBirth = dob;
                WasMainstay = mainstay;
            }
        }

        // Test Scenario: Relation to Non-Pre-Defined Entity (✗not permitted✗)
        [PreDefined] public class CivVIYield {
            public class District {
                [PrimaryKey] public Guid AssetID { get; set; }
                public string Name { get; set; } = "";
                public bool IsSpecialty { get; set; }
                public ushort GoldCost { get; set; }
                public ushort ProductionCost { get; set; }
                public ushort? FaithCost { get; set; }
            }

            [PrimaryKey] public Guid AssetId { get; private init; }
            public string Name { get; private init; }
            public bool InBaseGame { get; private init; }
            public IReadOnlyRelationList<District> ProducedBy { get; }

            public static CivVIYield Amenities { get; } = new CivVIYield("Amenities", true);
            public static CivVIYield Culture { get; } = new CivVIYield("Culture", true);
            public static CivVIYield Faith { get; } = new CivVIYield("Faith", true);
            public static CivVIYield Food { get; } = new CivVIYield("Food", true);
            public static CivVIYield Gold { get; } = new CivVIYield("Gold", true);
            public static CivVIYield Power { get; } = new CivVIYield("Power", false);
            public static CivVIYield Production { get; } = new CivVIYield("Production", true);
            public static CivVIYield Science { get; } = new CivVIYield("Science", true);
            public static CivVIYield Tourism { get; } = new CivVIYield("Tourism", true);

            private CivVIYield(string name, bool baseGame) {
                AssetId = Guid.NewGuid();
                Name = name;
                InBaseGame = baseGame;
                ProducedBy = new RelationList<District>();
            }
        }

        // Test Scenario: Aggregate-Nested Relation to Non-Pre-Defined Entity (✗not permitted✗)
        [PreDefined] public class StateOfMatter {
            public class Scientist {
                [PrimaryKey] public Guid ScientistID { get; set; }
                public string Name { get; set; } = "";
                public string DegreeFrom { get; set; } = "";
                public bool IsPhysicist { get; set; }
                public bool IsChemisty { get; set; }
            }

            public record struct Theory {
                public string? Name { get; set; }
                public DateTime FirstProposed { get; set; }
                public RelationSet<Scientist> Namesakes { get; }
            }

            [PrimaryKey] public string Name { get; private init; }
            public string? AsWater { get; private init; }
            public Theory? ExplanatoryTheory { get; private init; }

            public static StateOfMatter Solid { get; } = new StateOfMatter("Solid", "ice");
            public static StateOfMatter Liquid { get; } = new StateOfMatter("Liquid", "water");
            public static StateOfMatter Gas { get; } = new StateOfMatter("Gas", "water vapor / steam");
            public static StateOfMatter Plasma { get; } = new StateOfMatter("Plasma", null);
            public static StateOfMatter BEC { get; } = new StateOfMatter("Bose-Einstein Condensate", null);
            public static StateOfMatter QGP { get; } = new StateOfMatter("Quark-Gluon Plasma", null);

            private StateOfMatter(string name, string? water) {
                Name = name;
                AsWater = water;
                ExplanatoryTheory = null;
            }
        }

        // Test Scenario: Aggregate-Nested Relation to Non-Pre-Defined Entity, post-Memoization (✗not permitted✗)
        [PreDefined] public class Primate {
            public enum Kind { Monkey, Ape }

            public class Study {
                [PrimaryKey] public Guid ID { get; set; }
                public DateTime Initiated { get; set; }
                public DateTime? Concluded { get; set; }
                public string? Title { get; set; }
                public bool OnLocatio { get; set; }
            }
            public class Primatologist {
                [PrimaryKey] public Guid ID { get; set; }
                public Journal? PrimaryJournal { get; set; }
                public DateTime DOB { get; set; }
            }

            public struct Journal {
                public string Title { get; set; }
                public IReadOnlyRelationOrderedList<Study> Studies { get; }
            }

            [PrimaryKey] public string Genus { get; private init; }
            [PrimaryKey] public string Species { get; private init; }
            public string CommonName { get; private init; }
            public Kind Type { get; private init; }
            public Journal? DedicatedJournal { get; private init; }

            public static Primate AllensSwampMonkey { get; } = new Primate("Allenopithecus", "nigroviridis", "Allen's Swamp Monkey", true);
            public static Primate Guenon { get; } = new Primate("Allochorecubs", "preussi", "Guenon", true);
            public static Primate Mangabey { get; } = new Primate("Lophocebus", "albigena", "Magabey", true);
            public static Primate Macaque { get; } = new Primate("Macaca", "mulatta", "Macaque", true);
            public static Primate Mandrill { get; } = new Primate("Mandrillus", "sphinx", "Mandrill", true);
            public static Primate Talapoin { get; } = new Primate("Miopithecus", "talapoin", "Talapoin", true);
            public static Primate Baboon { get; } = new Primate("Papio", "anubis", "Baboon", true);
            public static Primate Kipunji { get; } = new Primate("Rungwecebus", "kipunji", "Kipuji", true);
            public static Primate Gelada { get; } = new Primate("Theropithecus", "gelada", "Gelada", true);
            public static Primate Colobus { get; } = new Primate("Colobus", "angolensis", "Colobus", true);
            public static Primate ProboscisMonkey { get; } = new Primate("Nasalis", "larvatus", "Proboscis Monkey", true);
            public static Primate Langur { get; } = new Primate("Presbytis", "canicrus", "Langur", true);
            public static Primate Douc { get; } = new Primate("Pygathrix", "cinerea", "Douc", true);
            public static Primate Gorilla { get; } = new Primate("Gorilla", "gorilla", "Gorilla", false);
            public static Primate Human { get; } = new Primate("Homo", "sapiens", "Human", false);
            public static Primate Chimpanzee { get; } = new Primate("Pan", "troglodytes", "Chimpanzee", false);
            public static Primate Bonobo { get; } = new Primate("Pan", "paniscus", "Bonobo", false);
            public static Primate Orangutan { get; } = new Primate("Pongo", "pygmaeus", "Orangutan", false);
            public static Primate Gibbon { get; } = new Primate("Nomascus", "gabriellae", "Gibbon", true);
            public static Primate Siamang { get; } = new Primate("Symphalangus", "syndactylus", "Siamang", true);
            public static Primate HowlerMonkey { get; } = new Primate("Alouatta", "arctoidea", "Howler Monkey", true);
            public static Primate SpiderMonkey { get; } = new Primate("Ateles", "belzebuth", "Spider Monkey", true);
            public static Primate Muriqui { get; } = new Primate("Brachyteles", "arachnoides", "Muriqui", true);
            public static Primate Marmoset { get; } = new Primate("Callithrix", "flaviceps", "Marmoset", true);
            public static Primate Tamarin { get; } = new Primate("Leontopithecus", "chrysomelas", "Tamarin", true);
            public static Primate Capuchin { get; } = new Primate("Cebus", "albifrons", "Capuchin", true);
            public static Primate Tarsier { get; } = new Primate("Tarsius", "dentatus", "Tarsier", true);
            public static Primate Uakari { get; } = new Primate("Cacajao", "ayresi", "Uakari", true);
            public static Primate Lemur { get; } = new Primate("Lemur", "catta", "Lemur", true);
            public static Primate AyeAye { get; } = new Primate("Daubentonia", "madagascariensis", "Aye-Aye", true);
            public static Primate Sifaka { get; } = new Primate("Propithecus", "candidus", "Sifaka", true);
            public static Primate Bushbaby { get; } = new Primate("Galago", "senegalensis", "Bushbaby", true);
            public static Primate Loris { get; } = new Primate("Nyctecibus", "hilleri", "Loris", true);

            private Primate(string genus, string species, string common, bool isMonkey) {
                Genus = genus;
                Species = species;
                CommonName = common;
                Type = isMonkey ? Kind.Monkey : Kind.Ape;
                DedicatedJournal = null;
            }
        }
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
            [CodeOnly] public IReadOnlyRelationSet<string> Resurrections { get; } = new RelationSet<string>();
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

        // Test Scenario: Public Write-Only Property Marked as [IncludeInModel] (✗not permitted✗)
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

        // Test Scenario: Non-Public Pre-Defined Instance Marked as [IncludeInModel] (✗not permitted✗)
        [PreDefined] public class SortingAlgorithm {
            [PrimaryKey] public string Name { get; private init; }
            public string BestCase { get; private init; }
            public string AverageCase { get; private init; }
            public string WorstCase { get; private init; }

            public static SortingAlgorithm Bubble { get; } = new SortingAlgorithm("Bubble Sort", "n", "n^2", "n^2");
            public static SortingAlgorithm Merge { get; } = new SortingAlgorithm("Merge Sort", "n*log(n)", "n*log(n)", "n*log(n)");
            public static SortingAlgorithm Heap { get; } = new SortingAlgorithm("Heap Sort", "n*log(n)", "n*log(n)", "n*log(n)");
            [IncludeInModel] private static SortingAlgorithm Bogo { get; } = new SortingAlgorithm("Bogo Sort", "n", "n*n!", "infinite");
            public static SortingAlgorithm Insertion { get; } = new SortingAlgorithm("Insertion Sort", "n", "n^2", "n^2");
            public static SortingAlgorithm Selection { get; } = new SortingAlgorithm("Selection Sort", "n^2", "n^2", "n^2");
            public static SortingAlgorithm Quick { get; } = new SortingAlgorithm("Quick Sort", "n*log(n)", "n*log(n)", "n^2");

            private SortingAlgorithm(string name, string best, string average, string worst) {
                Name = name;
                BestCase = $"O({best})";
                AverageCase = $"O({average})";
                WorstCase = $"O({worst})";
            }
        }

        // Test Scenario: Write-Only Pre-Defined Instance Marked as [IncludeInModel] (✗not permitted✗)
        [PreDefined] public class AnimalPhylum {
            [Calculated] public string Kingdom { get; } = "Animalia";
            [PrimaryKey] public string Name { get; private init; }
            public bool? FromCambrian { get; private init; }
            public ulong? ExtantSpecies { get; private init; }

            public static AnimalPhylum Acanthocephala { get; } = new AnimalPhylum("Acanthocephala", null, 750);
            public static AnimalPhylum Acoelomorpha { get; } = new AnimalPhylum("Acoelomorpha", null, null);
            public static AnimalPhylum Annelida { get; } = new AnimalPhylum("Annelida", true, 17000);
            public static AnimalPhylum Arthropoda { get; } = new AnimalPhylum("Arthropoda", true, 1100000);
            public static AnimalPhylum Brachiopoda { get; } = new AnimalPhylum("Brachiopoda", true, 400);
            public static AnimalPhylum Bryozoa { get; } = new AnimalPhylum("Bryozoa", false, 5000);
            public static AnimalPhylum Chaetognatha { get; } = new AnimalPhylum("Chaetognatha", true, 100);
            public static AnimalPhylum Chordata { get; } = new AnimalPhylum("Chordata", true, 60000);
            public static AnimalPhylum Cnidaria { get; } = new AnimalPhylum("Cnidaria", true, 11000);
            public static AnimalPhylum Ctenophora { get; } = new AnimalPhylum("Ctenophora", true, 100);
            public static AnimalPhylum Cycliophora { get; } = new AnimalPhylum("Cycliophora", null, 3);
            public static AnimalPhylum Echinodermata { get; } = new AnimalPhylum("Echinodermata", true, 7000);
            public static AnimalPhylum Echiura { get; } = new AnimalPhylum("Echiura", null, 130);
            public static AnimalPhylum Entoprocta { get; } = new AnimalPhylum("Entoprocta", null, 150);
            public static AnimalPhylum Gastrotricha { get; } = new AnimalPhylum("Gastrotricha", null, 690);
            public static AnimalPhylum Gnathostomulida { get; } = new AnimalPhylum("Gnathostomulida", null, 100);
            public static AnimalPhylum Hemichordata { get; } = new AnimalPhylum("Hemichordata", true, 100);
            public static AnimalPhylum Kinorhyncha { get; } = new AnimalPhylum("Kinorhyncha", null, 150);
            public static AnimalPhylum Loricifera { get; } = new AnimalPhylum("Loricifera", null, 120);
            public static AnimalPhylum Micrognathozoa { get; } = new AnimalPhylum("Micrognathozoa", null, 1);
            public static AnimalPhylum Mollusca { get; } = new AnimalPhylum("Mollusca", true, 112000);
            public static AnimalPhylum Nematoda { get; } = new AnimalPhylum("Nematoda", null, 1000000);
            public static AnimalPhylum Nematomorpha { get; } = new AnimalPhylum("Nematomorpha", null, 300);
            public static AnimalPhylum Nemertea { get; } = new AnimalPhylum("Nemertea", null, 1000);
            public static AnimalPhylum Onychophora { get; } = new AnimalPhylum("Onychophora", null, 200);
            public static AnimalPhylum Orthonectida { get; } = new AnimalPhylum("Orthonectida", null, 20);
            public static AnimalPhylum Phoronida { get; } = new AnimalPhylum("Phoronida", true, 20);
            public static AnimalPhylum Placozoa { get; } = new AnimalPhylum("Placozoa", null, 1);
            public static AnimalPhylum Platyhelminthes { get; } = new AnimalPhylum("Platyhelminthes", false, 25000);
            [IncludeInModel] public static AnimalPhylum Porifera { set { throw new NotImplementedException(); } }
            public static AnimalPhylum Priapulida { get; } = new AnimalPhylum("Priapulida", true, 17);
            public static AnimalPhylum Rhombozoa { get; } = new AnimalPhylum("Rhombozoa", null, null);
            public static AnimalPhylum Rotifera { get; } = new AnimalPhylum("Rotifera", false, null);
            public static AnimalPhylum Sipuncula { get; } = new AnimalPhylum("Sipuncula", true, null);
            public static AnimalPhylum Tardigrada { get; } = new AnimalPhylum("Tardigrada", true, 0);
            public static AnimalPhylum Xenoturbellida { get; } = new AnimalPhylum("Xenoturbellida", null, 2);

            private AnimalPhylum(string name, bool? cambrian, ulong? numSpecies) {
                Name = name;
                FromCambrian = cambrian;
                ExtantSpecies = numSpecies;
            }
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
            public RelationMap<short, string?> POBoxes { get; } = new();
            public RelationSet<string?> Employees { get; } = new();
            public RelationList<LicensePlate?> MailTrucks { get; } = new();
            public RelationMap<DateTime?, Stamp?> Stamps { get; } = new();
            public RelationOrderedList<decimal?> Budgets { get; } = new();
        }

        // Test Scenario: Relation with Nullable Aggregate Element Type with Only Nullable Fields (✗ambiguous✗)
        public class Parabola {
            public enum Direction { Up, Down, Left, Right }
            public record struct Point(int X, int Y);
            public record struct MaybePoint(int? X, int? Y);

            [PrimaryKey] public Point Vertex { get; set; }
            [PrimaryKey] public float Eccentricity { get; set; }
            public Direction Concavity { get; set; }
            public RelationSet<MaybePoint?> Points { get; } = new();
        }

        // Test Scenario: Relation Property Marked as [NonNullable] (✓redundant✓)
        public class Squintern {
            public record struct Episode(uint Season, ushort Number, string Title);

            [PrimaryKey] public string FirstName { get; set; } = "";
            [PrimaryKey] public string LastName { get; set; } = "";
            public bool IsFemale { get; set; }
            [NonNullable] public RelationList<Episode> Appearances { get; } = new();
            public bool HasTemperancesApproval { get; set; }
        }

        // Test Scenario: Relation Property Marked as [Nullable] (✗illegal✗)
        public class Axiom {
            [PrimaryKey] public string Name { get; set; } = "";
            public string Formulation { get; set; } = "";
            public string? PostulatedBy { get; set; }
            public bool IsLogical { get; set; }
            [Nullable] public RelationSet<string> DerivedTheories { get; } = new();
        }

        // Test Scenario: Pre-Defined Instance Property Marked as [NonNullable] (✓redundant✓)
        [PreDefined] public class Rugrat {
            [PrimaryKey] public uint ID { get; private init; }
            public string Name { get; private init; }
            public string VoiceActor { get; private init; }
            public byte AgeYears { get; private init; }

            [NonNullable] public static Rugrat Tommy { get; } = new Rugrat(1, "Tommy Pickles", "E. G. Daily", 1);
            [NonNullable] public static Rugrat Chuckie { get; } = new Rugrat(2, "Chuckie Finster", "Nancy Cartwright", 1);
            [NonNullable] public static Rugrat Angelica { get; } = new Rugrat(3, "Angelica Pickles", "Cheryl Chase", 3);
            [NonNullable] public static Rugrat Phil { get; } = new Rugrat(4, "Phil DeVille", "Kath Soucie", 1);
            [NonNullable] public static Rugrat Lil { get; } = new Rugrat(5, "Lil DeVille", "Kath Soucie", 1);
            [NonNullable] public static Rugrat Susie { get; } = new Rugrat(6, "Susie Carmichael", "Cree Summer", 3);
            [NonNullable] public static Rugrat Dil { get; } = new Rugrat(7, "Dil Pickles", "Tara Strong", 0);
            [NonNullable] public static Rugrat Kimi { get; } = new Rugrat(8, "Kimi Watanabe-Finster", "Charlotte Chung", 1);

            private Rugrat(uint id, string name, string voice, byte years) {
                ID = id;
                Name = name;
                VoiceActor = voice;
                AgeYears = years;
            }
        }

        // Test Scenario: Pre-Defined Instance Property Marked as [Nullable] (✗illegal✗)
        [PreDefined] public class Cutlery {
            [PrimaryKey] public Guid ID { get; private init; }
            public string Name { get; private init; }

            public static Cutlery Fork { get; } = new Cutlery("dinner fork");
            public static Cutlery Spoon { get; } = new Cutlery("spoon");
            public static Cutlery Knife { get; } = new Cutlery("table knife");
            public static Cutlery SaladFork { get; } = new Cutlery("salad fork");
            public static Cutlery SteakKnife { get; } = new Cutlery("steak knife");
            public static Cutlery ButterKnife { get; } = new Cutlery("butter knife");
            public static Cutlery Spork { get; } = new Cutlery("spork");
            [Nullable] public static Cutlery Chopsticks { get; } = new Cutlery("chopsticks");
            public static Cutlery SoupSpoon { get; } = new Cutlery("soup spoon");
            public static Cutlery DesertFork { get; } = new Cutlery("desert fork");
            public static Cutlery OysterFork { get; } = new Cutlery("oyster fork");

            private Cutlery(string name) {
                ID = Guid.NewGuid();
                Name = name;
            }
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
            public RelationSet<ushort> AtoB { get; } = new();
            public RelationSet<ushort> BtoA { get; } = new();
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
            [RelationTable("CreaturesTable")] public RelationMap<string, int> Population { get; } = new();
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
            [RelationTable("UT.Kvasir.Translation.TableNaming+PacerTest.LapsCompletedTable")] public RelationMap<Student, byte> LapsCompleted { get; } = new();
            public DateTime AdministeredOn { get; set; }
        }

        // Test Scenario: Relation Table Name Changed to `null` (✗illegal✗)
        public class Dwarf {
            [PrimaryKey] public Guid DwarfID { get; set; }
            public bool IsFictional { get; set; }
            public string Name { get; set; } = "";
            [RelationTable(null!)] public RelationMap<DateTime, string> LifeEvents { get; } = new();
            public byte Height { get; set; }
            public bool WieldsSword { get; set; }
            public double ShoeSize { get; set; }
        }

        // Test Scenario: Relation Table Named Changed to the Empty String (✗illegal✗)
        public class Rodent {
            public enum Taxon { Domain, Kingdom, Phylum, Class, Infraclass, Superorder, Order, Family, Subfamily, Genus, Species, Subspecies }
            public enum IUCN { Extinct, CaptivityOnly, CriticallyEndangered, Endangered, Vulnerable, NearThreatened, LeastConcern }

            [PrimaryKey] public string CommonName { get; set; } = "";
            [RelationTable("")] public RelationMap<Taxon, string> Taxonomy { get; } = new();
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
            [RelationTable("AuxiliaryVowelTable")] public RelationSet<string> Languages { get; } = new();
            [RelationTable("AuxiliaryVowelTable")] public RelationMap<char, char> Diacritics { get; } = new();
        }

        // Test Scenario: Table Name Duplicated between Principal Table and Relation Table (✗duplication✗)
        [Table("OfficialInfoVPN")]
        public class VPN {
            public enum Kind { Private, SiteToSite, Extranet, Other }

            [PrimaryKey] public string IP { get; set; } = "";
            public Kind Type { get; set; }
            public bool PasswordProtected { get; set; }
            [RelationTable("OfficialInfoVPN")] public RelationList<Guid> AuthorizedUsers { get; } = new();
        }

        // Test Scenario: [RelationTable] Applied to Numeric Field (✗impermissible✗)
        public class Shofar {
            [PrimaryKey] public Guid ShofarID { get; set; }
            public string Maker { get; set; } = "";
            public double Weight { get; set; }
            public ushort NumTurns { get; set; }
            [RelationTable("---")] public float Tekiah { get; set;}
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

        // Test Scenario: [RelationTable] Applied to Pre-Defined Instance (✗impermissible✗)
        [PreDefined] public class DEFCON {
            [PrimaryKey] public sbyte Level { get; private init; }
            public string ExerciseTerm { get; private init; }
            public string Readiness { get; private init; }

            public static DEFCON One { get; } = new DEFCON(1, "COCKED PISTOL", "maximum");
            public static DEFCON Two { get; } = new DEFCON(2, "FAST PACE", "Army < 6hrs");
            public static DEFCON Three { get; } = new DEFCON(3, "ROUND HOUSE", "Air Force < 15min");
            [RelationTable("---")] public static DEFCON Four { get; } = new DEFCON(4, "DOUBLE TAKE", "above normal");
            public static DEFCON Five { get; } = new DEFCON(5, "FADE OUT", "normal");

            private DEFCON(sbyte level, string exerciseTerm, string readiness) {
                Level = level;
                ExerciseTerm = exerciseTerm;
                Readiness = readiness;
            }
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
            [Name("MaterialsTable")] public RelationSet<string> Composition { get; } = new();
            public bool HasPassed { get; set; }
        }

        // Test Scenario: Change Relation-Nested Field Name to New Value (✓renamed✓)
        public class SwissCanton {
            [PrimaryKey] public Guid ID { get; set; }
            [Name("Canton", Path = "SwissCanton")] public RelationMap<string, string> Names { get; } = new();
            public ulong Area { get; set; }
            public ulong Population { get; set; }
            public string CantonCapital { get; set; } = "";
            public ushort YearJoinedSwissConfederation { get; set; }
            [Name("CantonID", Path = "SwissCanton.ID"), Name("Councilor", Path = "Item")] public RelationSet<string> Councilors { get; } = new();
            [Name("Religion", Path = "Key"), Name("%PCNT", Path = "Value")] public RelationMap<string, double> Religions { get; } = new();
        }

        // Test Scenario: Change Nested Relation Name (✓affects name of Table✓)
        public class Gulag {
            public enum Org { Cheka, GPU, OGPU, KNVD, InternalAffairs, StalinsPersonalGuard }
            public record struct Naming(string English, string Russian);
            public struct Personnel {
                public string Commandant { get; set; }
                public RelationList<string> Overseers { get; }
                public Org Administeredby { get; set; }
            }

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
                public RelationMap<string, Measure> Ingredients { get; } = new();
                public RelationOrderedList<string> Steps { get; } = new();
            }

            [PrimaryKey] public string ISBN { get; set; } = "";
            public string Title { get; set; } = "";
            public string Author { get; set; } = "";
            public DateTime PublicationDate { get; set; }
            public string AmazonURL { get; set; } = "";
            [Name("Cookbook.ISBN", Path = "Item.RecipeID")] public RelationList<Recipe> Recipes { get; } = new();
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
            [Name("Value", Path = "HostageSituation.IncidentID"), Name("Value", Path = "Item.SSN")] public RelationList<Person> Hostages { get; } = new();
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
                public RelationSet<string> Components { get; } = new();
                public bool TimeSensitive { get; set; }
            }

            [PrimaryKey] public Guid MagicUserID { get; set; }
            public string? Name { get; set; }
            public AlignedAs Alignment { get; set; }
            [Name("Spellbook"), Name("Spellbook")] public RelationSet<Spell> Spells { get; } = new();
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
            [Name("TimelineOfEvents"), Name("Calendar")] public RelationMap<DateTime, Event> Timeline { get; } = new();
            public ushort ICCIndictments { get; set; }
        }

        // Test Scenario: Relation Property with Redundant and Non-Redundant [Name] Changes (✗cardinality✗)
        public class PrideParade {
            [Flags] public enum Group { Lesbian = 1, Gay = 2, Bisexual = 4, Transgender = 8, Queer = 16, Asexual = 32, Intersex = 64, TwoSpirit = 128, Questioning = 256, Straight = 512 }

            [PrimaryKey] public Guid ID { get; set; }
            public DateTime Date { get; set; }
            [Name("Participants"), Name("Paraders")] public IReadOnlyRelationSet<string> Participants { get; } = new RelationSet<string>();
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
            [Name("TotalArea", Path = "Item.TotalSpace")] public RelationSet<PointOfInterest> Ruins { get; } = new();
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
                [Name("HighTemps")] public RelationMap<DateTime, double> Highs { get; }
                [Name("LowTemps")] public RelationMap<DateTime, double> Lows { get; }
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

        // Test Scenario: [Name] Applied to Pre-Defined Instance (✗impermissible✗)
        [PreDefined] public class Murder {
            [PrimaryKey] public string What { get; private init; }
            public string Who { get; private init; }
            public ulong? WordFrequencyRank { get; private init; }

            public static Murder Suicide { get; } = new Murder("suicide", "self", 2288);
            public static Murder Matricide { get; } = new Murder("matricide", "mother", 55721);
            public static Murder Patricide { get; } = new Murder("patricide", "father", null);
            public static Murder Sororicide { get; } = new Murder("sororicide", "sister", null);
            public static Murder Fratricide { get; } = new Murder("fratricide", "brother", 39880);
            public static Murder Infanticide { get; } = new Murder("infanticide", "infants", 28692);
            [Name("King-Killing")] public static Murder Regicide { get; } = new Murder("regicide", "king", 51563);
            public static Murder Deicide { get; } = new Murder("deicide", "god", null);
            public static Murder Filicide { get; } = new Murder("filicide", "child", null);
            public static Murder Genocide { get; } = new Murder("genocide", "group", 7692);

            private Murder(string what, string who, ulong? rank) {
                What = what;
                Who = who;
                WordFrequencyRank = rank;
            }
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
            [Name("eax?", Path = "---")] public RelationSet<string> Architectures { get; } = new();
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
            [Name("HomeCity", Path = "Yeshiva.City")] public RelationList<Student> Students { get; } = new();
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

        // Test Scenario: Non-`null` Date-ish Default (✓valid✓)
        public class Umpire {
            [PrimaryKey] public Guid UniqueUmpireNumber { get; set; }
            public ushort UniformNumber { get; set; }
            public string Name { get; set; } = "";
            [Default("1970-01-01")] public DateTime Debut { get; set; }
            [Default("1984-03-07")] public DateOnly JoinedUnion { get; set; }
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
            public RelationMap<Directory, string> Mounts { get; } = new();
        }

        // Test Scenario: Default on Relation-Nested Field (✓valid✓)
        public class Kami {
            [PrimaryKey] public string Name { get; set; } = "";
            public string CultCenter { get; set; } = "";
            [Default("n/a", Path = "Item")] public RelationSet<string> AKAs { get; } = new();
            [Default("Susano'o", Path = "Kami.Name"), Default((short)19, Path = "Value")] public RelationMap<string, short> Appearances { get; } = new();
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
            [Default(3.75, Path = "Item.Cost")] public RelationList<Accessory> Accessories { get; } = new();
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
            public struct Machine {
                public Guid SerialNumber { get; set; }
                public RelationList<string> Owners { get; }
            }

            [PrimaryKey] public string FirstName { get; set; } = "";
            [PrimaryKey] public string LastName { get; set; } = "";
            public DateTime BirthDate { get; set; }
            [Default("Marty McFly", Path = "Owners")] public Machine TimeMachine { get; set; }
            public RelationSet<DateTime> Visitations { get; } = new();
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

        // Test Scenario: Date Default is Not as String (✗invalid✗)
        public class Doula {
            [PrimaryKey] public Guid CaregiverID { get; set; }
            public ushort NumBirthsAssisted { get; set; }
            [Default(45681UL)] public DateOnly DateOfOwnBirth { get; set; }
            public bool HasLicense { get; set; }
            public bool WaterBirthCertified { get; set; }
        }

        // Test Scenario: Date Default is Improperly Formatted (✗invalid✗)
        public class Unicorn {
            [PrimaryKey] public Guid CreatureID { get; set; }
            public string? Name { get; set; }
            [Default("1500-06-19 08:17:44")] public DateOnly FirstHornMolting { get; set; }
            public DateOnly? SecondHornMolting { get; set; }
            public ulong Age { get; set; }
            public sbyte Strength { get; set; }
        }

        // Test Scenario: Date Default is Out-of-Range (✗invalid✗)
        public class LargeLanguageModel {
            [PrimaryKey] public string Name { get; set; } = "";
            public string Developer { get; set; } = "";
            [Default("1654-02-48")] public DateOnly Released { get; set; }
            public ulong Parameters { get; set; }
            public ulong Compute { get; set; }
            public decimal? TokenCost { get; set; }
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
            [PrimaryKey, Default("1344-12-11 27:14:05")] public DateTime CreationDate { get; set; }
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
            [DataConverter<ToInt<char>>, Default('A')] public char AcrossOrDown { get; set; }
            public ushort Number { get; set; }
            public byte NumLetters { get; set; }
            public string ClueText { get; set; } = "";
        }

        // Test Scenario: Default of Target Type on Data-Converted Property (✓valid✓)
        public class Coupon {
            [PrimaryKey] public Guid Barcode { get; set; }
            public string? Code { get; set; }
            [DataConverter<ToInt<bool>>, Default(0)] public bool IsBOGO { get; set; }
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

        // Test Scenario: [Default] Applied to Pre-Defined Instance (✗impermissible✗)
        [PreDefined] public class InspectorClouseau {
            [PrimaryKey] public string Actor { get; private init; }
            public ushort FilmAppearances { get; private init; }
            public DateTime FirstAppearance { get; private init; }

            [Default(1000)] public static InspectorClouseau Sellers { get; } = new InspectorClouseau("Peter Sellers", 7, new DateTime(1963, 12, 18));
            public static InspectorClouseau Arkin { get; } = new InspectorClouseau("Alan Arkin", 1, new DateTime(1968, 5, 28));
            public static InspectorClouseau Moore { get; } = new InspectorClouseau("Roger Moore", 1, new DateTime(1983, 8, 12));
            public static InspectorClouseau Martin { get; } = new InspectorClouseau("Steve Martin", 2, new DateTime(2006, 2, 10));

            private InspectorClouseau(string actor, ushort appearances, DateTime first) {
                Actor = actor;
                FilmAppearances = appearances;
                FirstAppearance = first;
            }
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
            [Default(0.00, Path = "---")] public RelationMap<string, double> HighScores { get; } = new();
            public ushort NumLevels { get; set; }
            public bool IsSports { get; set; }
        }

        // Test Scenario: <Path> on Relation Refers to Non-Primary-Key Field of Anchor Entity (✗non-existent path✗)
        public class Monad {
            public enum Operation { Map, Join, Unit, Bind, Constructor }

            [PrimaryKey] public string Name { get; set; } = "";
            [Default((sbyte)77, Path = "Monad.ModelsOption")] public RelationMap<Operation, string> Traits { get; } = new();
            public bool ModelsOption { get; set; }
        }

        // Test Scenario: <Path> on Relation Not Specified (✗missing path✗)
        public class LaundryDetergent {
            [PrimaryKey] public Guid DetergentID { get; set; }
            public string Brand { get; set; } = "";
            public double VolumePerLoad { get; set; }
            [Default(null)] public RelationSet<string> Ingredients { get; } = new();
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
            public RelationMap<string, Permission> Account { get; } = new();
            public DateTime Expiration { get; set; }
            public RelationList<Transaction> Transactions { get; } = new();
        }

        // Test Scenario: Relation Fields are Manually Ordered (✗impermissible✗)
        public class Tapestry {
            [PrimaryKey] public Guid TapestryID { get; set; }
            public double Length { get; set; }
            public double Width { get; set; }
            [Column(6)] public RelationList<string> Depictions { get; } = new();
            public string? Artist { get; set; }
            public ulong ThreadCount { get; set; }
        }

        // Test Scenario: [Column] Applied to Pre-Defined Instance (✗impermissible✗)
        [PreDefined] public class Birthstone {
            [PrimaryKey] public string Month { get; private init; }
            public string Stone { get; private init; }
            public bool IsQuartz { get; private init; }

            public static Birthstone Garnet { get; } = new Birthstone("JAN", "garnet", false);
            [Column(7)] public static Birthstone Amethyst { get; } = new Birthstone("FEB", "amethyst", true);
            public static Birthstone Aquamarine { get; } = new Birthstone("MAR", "aquamarine", false);
            public static Birthstone Diamond { get; } = new Birthstone("APR", "diamond", false);
            public static Birthstone Emerald { get; } = new Birthstone("MAY", "emerald", false);
            public static Birthstone Pearl { get; } = new Birthstone("JUNE", "pearl", false);
            public static Birthstone Ruby { get; } = new Birthstone("JULY", "ruby", false);
            public static Birthstone Peridot { get; } = new Birthstone("AUG", "peridot", false);
            public static Birthstone Sapphire { get; } = new Birthstone("SEP", "sapphire", false);
            public static Birthstone Opal { get; } = new Birthstone("OCT", "opal", false);
            public static Birthstone Topaz { get; } = new Birthstone("NOV", "topaz", false);
            public static Birthstone Turquoise { get; } = new Birthstone("DEC", "turquoise", false);

            private Birthstone(string month, string stone, bool quartz) {
                Month = month;
                Stone = stone;
                IsQuartz = quartz;
            }
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
            public RelationList<string> Notifications { get; } = new();
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
            [PrimaryKey(Path = "GrandPrix.Year"), PrimaryKey(Path = "GrandPrix.Country"), PrimaryKey(Path = "Key.CarNumber")] public RelationMap<Driver, short> Results { get; } = new();
            public byte NumLaps { get; set; }
            public ulong TrackLength { get; set; }
            public uint NumCrashes { get; set; }
            public bool FormulaOne { get; set; }
        }

        // Test Scenario: Nested Relation Property Marked as [PrimaryKey] (✗illegal✗)
        public class Psalm {
            public record struct TextInfo {
                public string Title { get; set; }
                public RelationMap<ushort, string> Verses { get; }
            }

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
            public RelationList<string> Employees { get; } = new();
            public decimal YearlyRevenue { get; set; }
            public bool IsLegal { get; set; }
            public RelationSet<Service> Services { get; } = new();
        }

        // Test Scenario: Default Deduction for Map Relation (✓identified✓)
        public class Cult {
            public record struct Info(DateTime Joined, bool Alive, string Position);

            [PrimaryKey] public string Title { get; set; } = "";
            public string Leader { get; set; } = "";
            public DateTime Founded { get; set; }
            public DateTime? Shuttered { get; set; }
            public RelationMap<string, Info> Members { get; } = new();
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
            public RelationOrderedList<Note> Score { get; } = new();
        }

        // Test Scenario: Single Candidate Key on Relation Including Anchor (✓identified✓)
        public class ChromeExtension {
            public record struct Rating(string Reviewer, DateTime Timestamp, double Stars);

            [PrimaryKey] public Guid ExtensionID { get; set; }
            public string ExtensionName { get; set; } = "";
            public ulong Downloads { get; set; }
            [Unique("Key", Path = "ChromeExtension.ExtensionID"), Unique("Key", Path = "Item.Reviewer")] public RelationList<Rating> Reviews { get; } = new();
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
            [Unique(Path = "Item")] public RelationSet<string> Precautions { get; } = new();
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
            public RelationSet<Fossil> Fossils { get; } = new();
        }

        // Test Scenario: Primary Key on Relation Table with Candidate Keys Cannot Be Deduced (✗illegal✗)
        public class Blockbuster {
            [PrimaryKey] public Guid ID { get; set; }
            public string Manager { get; set; } = "";
            public DateTime Opened { get; set; }
            public DateTime? Closed { get; set; }
            [Unique("V", Path = "Blockbuster.ID"), Unique("V", Path = "Value")] public RelationMap<string, string> Rentals { get; } = new();
            public ushort TotalVideos { get; set; }
            public decimal LifetimeRevenue { get; set; }
            public bool IsFranchise { get; set; }
        }

        // Test Scenario: [PrimaryKey] Applied to Pre-Defined Instance (✗illegal✗)
        [PreDefined] public class FunctionalGroup {
            [PrimaryKey] public string Group { get; private init; }
            public string Formula { get; private init; }
            public string? Prefix { get; private init; }
            public string? Suffix { get; private init; }

            public static FunctionalGroup Alkane { get; } = new FunctionalGroup("Alkyl", "R(CH2)nH", "alkyl-", "-ane");
            public static FunctionalGroup Alkene { get; } = new FunctionalGroup("Alkenyl", "R2C=CR2", "alkenyl-", "-ene");
            public static FunctionalGroup Alkyne { get; } = new FunctionalGroup("Akynyl", "RC==CR'", "alkynyl", "-yne");
            public static FunctionalGroup Alcohol { get; } = new FunctionalGroup("Hydroxyl", "ROH", "hydroxy-", "-ol");
            public static FunctionalGroup Ketone { get; } = new FunctionalGroup("Ketone", "RCOR'", null, "-one");
            public static FunctionalGroup Aldehyde { get; } = new FunctionalGroup("Aldehyde", "RCHO", "formyl-", "-al");
            public static FunctionalGroup Carbonate { get; } = new FunctionalGroup("Carbonate Ester", "ROCOOR'", null, null);
            public static FunctionalGroup CarboxylicAcid { get; } = new FunctionalGroup("Carboxyl", "RCOOH", "carboxy-", "-oic acid");
            public static FunctionalGroup Ester { get; } = new FunctionalGroup("Carboalkoxy", "RCOOR'", "alkanoyloxy-", null);
            [PrimaryKey] public static FunctionalGroup Peroxide { get; } = new FunctionalGroup("Peroxy", "ROOR'", "peroxy-", null);
            public static FunctionalGroup Ether { get; } = new FunctionalGroup("Ether", "ROR'", "alkoxy-", null);
            public static FunctionalGroup Amide { get; } = new FunctionalGroup("Carboxamide", "RCONR'R\"", "carboxamido-", "-amide");
            public static FunctionalGroup Amine { get; } = new FunctionalGroup("Tertiary Amine", "R3N", "amino-", "-amine");
            public static FunctionalGroup Azide { get; } = new FunctionalGroup("Azide", "RN3", "azido-", null);
            public static FunctionalGroup Cyanate { get; } = new FunctionalGroup("Cyanate", "ROCN", "cyanato-", null);
            public static FunctionalGroup Nitrate { get; } = new FunctionalGroup("Nitrate", "RONO2", "nitroxy-", null);
            public static FunctionalGroup Nitrite { get; } = new FunctionalGroup("Nitrite", "RONO", "nitrosooxy-", null);
            public static FunctionalGroup Nitrile { get; } = new FunctionalGroup("Nitrile", "RCN", "cyano-", null);
            public static FunctionalGroup Thiol { get; } = new FunctionalGroup("Sulfhydryl", "RSH", "sulfanyl-", "-thiol");
            public static FunctionalGroup Disulfide { get; } = new FunctionalGroup("Disulfide", "RSSR'", null, null);
            public static FunctionalGroup Phosphate { get; } = new FunctionalGroup("Phosphate", "ROP(=O)(OH)2", "phosphonooxy-", null);

            private FunctionalGroup(string group, string formula, string? prefix, string? suffix) {
                Group = group;
                Formula = formula;
                Prefix = prefix;
                Suffix = suffix;
            }
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
            [PrimaryKey(Path = "---")] public RelationSet<string> Manufacturers { get; } = new();
        }

        // Test Scenario: <Path> on Relation Refers to Non-Primary-Key Field of Anchor Entity (✗non-existent path✗)
        public class TreasureMap {
            public record struct Coordinate(float Latitude, float Longitude);

            [PrimaryKey] public Guid ID { get; set; }
            public string? Author { get; set; }
            public decimal TreasureValue { get; set; }
            public Coordinate X { get; set; }
            [PrimaryKey(Path = "TreasureMap.X")] public RelationList<Coordinate> SuggestedPath { get; } = new();
            public bool Damaged { get; set; }
        }

        // Test Scenario: <Path> on Relation Not Specified (✗missing path✗)
        public class Hologram {
            public Guid ID { get; set; }
            public double Height { get; set; }
            public double AspectRatio { get; set; }
            [PrimaryKey] public RelationMap<short, string> Copyrights { get; } = new();
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
            [PrimaryKey(Path = "Key.Time"), Unique("X", Path = "BigBlockOfCheeseDay"), Unique("X", Path = "Key.Organization")] public RelationMap<Slot, string> Schedule { get; } = new();
            public double PercentCrackpots { get; set; }
        }

        // Test Scenario: Nested Relation Field in Candidate Key (✗illegal✗)
        public class RentalCar {
            public record struct Duration(DateTime Start, DateTime End);
            public record struct History {
                public RelationMap<string, Duration> Renters { get; }
                public DateTime Acquired { get; set; }
            }

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
            [PrimaryKey(Path = "Intifada.Name"), PrimaryKey(Path = "Item.CountryName")] public RelationSet<Country> Belligerents { get; } = new();
            public ulong Casualties { get; set; }
        }

        // Test Scenario: Anchor + Key in Candidate Key (✓redundant✓)
        public class VoodooDoll {
            public enum Color { Red, Orange, Yellow, Green, Blue, Purple, White, Black, Gray, Brown, Pink, Gold }
            public enum Thing { Straw, Cloth, Fruit, Hay, Cotton, Paper, Sand }

            [PrimaryKey] public Guid VoodooID { get; set; }
            public string Target { get; set; } = "";
            public string? PatronLoa { get; set; }
            [PrimaryKey(Path = "VoodooDoll.VoodooID"), PrimaryKey(Path = "Value"), Unique("Unique", Path = "VoodooDoll.VoodooID"), Unique("Unique", Path = "Key")] public RelationMap<Color, string> Pins { get; } = new();
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
            [PrimaryKey(Path = "OPO.ID"), PrimaryKey(Path = "Item"), Unique("Unique", Path = "OPO.ID"), Unique("Unique", Path = "Index")] public RelationOrderedList<string> Administrators { get; } = new();
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

        // Test Scenario: [Unique] Applied to Pre-Defined Instance (✗impermissible✗)
        [PreDefined] public class Cheesecake {
            [PrimaryKey] public Guid ID { get; private init; }
            public string Name { get; private init; }
            public bool AmericanOrigin { get; private init; }

            public static Cheesecake NoBake { get; } = new Cheesecake("no-bake cheesecake", true);
            public static Cheesecake NewYork { get; } = new Cheesecake("New York cheesecake", true);
            public static Cheesecake Ricotta { get; } = new Cheesecake("ricotta cheesecake", true);
            public static Cheesecake Japanese { get; } = new Cheesecake("Japanese cheesecake", false);
            [Unique] public static Cheesecake Ube { get; } = new Cheesecake("ube", false);
            public static Cheesecake Basque { get; } = new Cheesecake("Basque cheesecake", false);
            public static Cheesecake Flao { get; } = new Cheesecake("flaó", false);

            private Cheesecake(string name, bool isAmerican) {
                ID = Guid.NewGuid();
                Name = name;
                AmericanOrigin = isAmerican;
            }
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
            [Unique(Path = "---")] public RelationMap<string, string> MedicalIdentifiers { get; } = new();
            public Symptom SymptomsRelieved { get; set; }
        }

        // Test Scenario: <Path> on Relation Refers to Non-Primary-Key Field of Anchor Entity (✗non-existent path✗)
        public class Oasis {
            [PrimaryKey] public float Latitude { get; set; }
            [PrimaryKey] public float Longitude { get; set; }
            public ulong Water { get; set; }
            [Unique(Path = "Oasis.Water")] public RelationSet<string> TreeSpecies { get; } = new();
            public double AverageTemperature { get; set; }
        }

        // Test Scenario: <Path> on Relation Not Specified (✗missing path✗)
        public class LimboCompetition {
            [PrimaryKey] public Guid ID { get; set; }
            public string Song { get; set; } = "";
            public ushort BarLength { get; set; }
            [Unique] public IReadOnlyRelationMap<string, float> Heights { get; } = new RelationMap<string, float>();
            public decimal PrizeMoney { get; set; }
            public bool SoberRequired { get; set; }
        }
    }

    internal static class DataConverters {
        // Test Scenario: Data Conversion does not Change Field's Type (✓applied✓)
        public class Cenote {
            [PrimaryKey] public string Name { get; set; } = "";
            public float MaxDepth { get; set; }
            [DataConverter<Invert>] public bool IsKarst { get; set; }
            public decimal Latitude { get; set; }
            [DataConverter<Identity<decimal>>] public decimal Longitude { get; set; }
        }

        // Test Scenario: Data Conversion Changes Field's Type to Scalar (✓applied✓)
        public class Comet {
            [PrimaryKey] public Guid AstronomicalIdentifier { get; set; }
            public double Aphelion { get; set; }
            [DataConverter<RoundDown>] public double Perihelion { get; set; }
            [DataConverter<RoundDown>] public double Eccentricity { get; set; }
            public ulong MassKg { get; set; }
            public double Albedo { get; set; }
            public float OrbitalPeriod { get; set; }
        }

        // Test Scenario: Data Conversion Changes Field's Type to Enumeration (✓applied✓)
        public class TitleOfYourSexTape {
            [PrimaryKey] public string Title { get; set; } = "";
            public string CharacterSaying { get; set; } = "";
            public string CharacterReceiving { get; set; } = "";
            [DataConverter<Enumify<int, DayOfWeek>>] public int DayOfWeek { get; set; }
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
            [DataConverter<SwapEnums<Campaign, Party>>] public Campaign? IntroducedIn { get; set; }
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
            [DataConverter<MakeDate<Variety>>] public Variety Structure { get; set; }
        }

        // Test Scenario: Custom Data Conversion for Boolean Field (✓applied✓)
        public class MathematicalConjecture {
            [PrimaryKey] public string Name { get; set; } = "";
            public bool IsMillenniumPrize { get; set; }
            [DataConverter<ToInt<bool>>] public bool Solved { get; set; }
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
            [DataConverter<ToInt<Person>>] public Person KnightB { get; set; }
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
            [DataConverter<ToInt<Athlete>>] public Athlete Winner { get; set; } = new();
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
            [DataConverter<ToInt<RelationMap<ulong, decimal>>>] public RelationMap<ulong, decimal> Accounts { get; } = new();
            public string VaultModel { get; set; } = "";
            public sbyte NumTellers { get; set; }
            public decimal CashOnHand { get; set; }
            public bool CanMakeLoans { get; set; }
            public byte NumTimesRobbed { get; set; }
            public bool Crypto { get; set; }
        }

        // Test Scenario: [DataConverter] Applied to Pre-Defined Instance (✗impermissible✗)
        [PreDefined] public class PowerRanger {
            [PrimaryKey] public string Color { get; private init; }
            public string Name { get; private init; }
            public string Dinozord { get; private init; }

            public static PowerRanger Pink { get; } = new PowerRanger("pink", "Kimberly Hart", "Pterodactyl");
            public static PowerRanger Red { get; } = new PowerRanger("red", "Jason Lee Scott", "Tyrannosaurus");
            public static PowerRanger Black { get; } = new PowerRanger("black", "Zack Taylor", "Mastodon");
            [DataConverter<Identity<PowerRanger>>] public static PowerRanger Blue { get; } = new PowerRanger("blue", "Billy Cranston", "Triceratops");
            public static PowerRanger Yellow { get; } = new PowerRanger("yellow", "Trini Kwan", "Sabertooth Tiger");

            private PowerRanger(string color, string name, string dinozord) {
                Color = color;
                Name = name;
                Dinozord = dinozord;
            }
        }

        // Test Scenario: Data Conversion Source Type is Non-Nullable on Nullable Field (✓applied✓)
        public class RoyalHouse {
            [PrimaryKey] public string HouseName { get; set; } = "";
            public DateTime Founded { get; set; }
            [DataConverter<DeNullify<string>>] public string? CurrentHead { get; set; }
            [DataConverter<DeNullify<int>>] public int? TotalMonarchs { get; set; }
        }

        // Test Scenario: Data Conversion Source Type is Nullable on Non-Nullable Field (✓applied✓)
        public class Planeswalker {
            [PrimaryKey] public string Name { get; set; } = "";
            public sbyte MannaCost { get; set; }
            public sbyte InitialLoyalty { get; set; }
            [DataConverter<Nullify<char>>] public char SetIcon { get; set; }
            [DataConverter<Nullify<string>>] public string Ability1 { get; set; } = "";
            [DataConverter<Nullify<string>>] public string Ability2 { get; set; } = "";
            [DataConverter<Nullify<string>>] public string Ability3 { get; set; } = "";
            [DataConverter<Nullify<Guid>>] public Guid SerialNumber { get; set; }
        }

        // Test Scenario: CLR Type is Inconvertible to Data Conversion Source Type (✗invalid✗)
        public class Jedi {
            [PrimaryKey] public int WookiepediaID { get; set; }
            public string FirstName { get; set; } = "";
            [DataConverter<DeNullify<bool>>] public string? MiddleName { get; set; }
            public string? LastName { get; set; }
            public string LightsaberColor { get; set; } = "";
            public double Height { get; set; }
            public double Weight { get; set; }
            public int NumMovieLines { get; set; }
            public int NumTelevisionLines { get; set; }
        }

        // Test Scenario: CLR Type is Convertible to Data Conversion Source Type (✗invalid✗)
        public class ConstitutionalAmendment {
            [PrimaryKey, DataConverter<Nullify<long>>] public int Number { get; set; }
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
            [DataConverter<ToError<DateTime>>] public DateTime AirDate { get; set; }
            public ushort WeekendUpdateDuration { get; set; }
        }

        // Test Scenario: Data Converter Throws Error upon Construction (✗propagated✗)
        public class Sword {
            [PrimaryKey] public string Name { get; set; } = "";
            public decimal Sharpness { get; set; }
            public float Length { get; set; }
            public float Weight { get; set; }
            public int Kills { get; set; }
            [DataConverter<Unconstructible<short>>] public short YearForged { get; set; }
        }

        // Test Scenario: Data Converter Throws upon Execution (✗propagated✗)
        public class Ligament {
            public enum Type { Articular, Pretioneal, FetalRemnant }

            [PrimaryKey] public string MeSH { get; set; } = "";
            public string Name { get; set; } = "";
            [DataConverter<Unconvertible<Type>>] public Type Classification { get; set; }
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

        // Test Scenario: [Numeric] Applied to DateOnly Field (✗impermissible✗)
        public class PonziScheme {
            [PrimaryKey] public string Perpetrator { get; set; } = "";
            [PrimaryKey, Numeric] public DateOnly Initiated { get; set; }
            public DateOnly? Discovred { get; set; }
            public decimal TotalCost { get; set; }
            public bool InvestigatedBySEC { get; set; }
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
            [Numeric] public RelationSet<string> MineralDeposits { get; } = new();
            public sbyte NumOases { get; set; }
        }

        // Test Scenario: [Numeric] Applied to Pre-Defined Instance (✗impermissible✗)
        [PreDefined] public class Friend {
            [PrimaryKey] public string FirstName { get; private init; }
            [PrimaryKey] public string LastName { get; private init; }
            public string Actor { get; private init; }
            public ulong LineCount { get; private init; }

            public static Friend Chandler { get; } = new Friend("Chandler", "Bing", "Matthew Perry", 8465);
            [Numeric] public static Friend Joey { get; } = new Friend("Joey", "Tribbiani", "Matt LeBlanc", 8171);
            public static Friend Monica { get; } = new Friend("Monica", "Gellar-Bing", "Courteney Cox", 8441);
            public static Friend Phoebe { get; } = new Friend("Phoebe", "Buffay", "Lisa Kudrow", 7501);
            public static Friend Rachel { get; } = new Friend("Rachel", "Green", "Jennifer Aniston", 9312);
            public static Friend Ross { get; } = new Friend("Ross", "Gellar", "David Schwimmer", 9157);

            private Friend(string firstName, string lastName, string actor, ulong lines) {
                FirstName = firstName;
                LastName = lastName;
                Actor = actor;
                LineCount = lines;
            }
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

        // Test Scenario: [AsString] Applied to DateOnly Field (✗impermissible✗)
        public class Vaccine {
            [PrimaryKey] public Guid MedicineID { get; set; }
            public string DiseaseTargeted { get; set; } = "";
            public string SynthesizedBy { get; set; } = "";
            [AsString] public DateOnly SynthesizedOn { get; set; }
            public ulong DosesAdministeredPerYear { get; set; }
            public bool RecommendedForNewborns { get; set; }
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
            [AsString] public RelationMap<char, char> Solution { get; } = new();
        }

        // Test Scenario: [AsString] Applied to Pre-Defined Instance (✗impermissible✗)
        [PreDefined] public class Teletubby {
            [PrimaryKey] public string Name { get; private init; }
            public string Color { get; private init; }
            public string AntennaShape { get; private init; }

            public static Teletubby TinkyWinky { get; } = new Teletubby("Tinky Winky", "purple", "triangle");
            [AsString] public static Teletubby Dipsy { get; } = new Teletubby("Dipsy", "green", "dipstick");
            public static Teletubby LaaLaa { get; } = new Teletubby("Laa-Laa", "yellow", "curl");
            public static Teletubby Po { get; } = new Teletubby("Po", "red", "bubble blower");

            private Teletubby(string name, string color, string shape) {
                Name = name;
                Color = color;
                AntennaShape = shape;
            }
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
            [DataConverter<ToString<Role>>, Numeric] public Role? Player7 { get; set; }
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
            [DataConverter<ToInt<Material>>, AsString] public Material MadeOf { get; set; }
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

            // Test Scenario: Applied to DateOnly Field (✗impermissible✗)
            public class Jotunn {
                public enum Varity { Fire, Ice, Earth }

                [PrimaryKey] public string Name { get; set; } = "";
                public bool LivesInJotunheim { get; set; }
                public bool ParticipatesInRagnarok { get; set; }
                [Check.IsPositive] public DateOnly? FirstMentioned { get; set; }
                public uint EddaMentions { get; set; }
                public Varity Kind { get; set; }
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

            // Test Scenario: Applied to Pre-Defined Instance (✗impermissible✗)
            [PreDefined] public class Russo {
                public enum Wizardry { Current, Past, Never }

                [PrimaryKey] public ulong IMDbID { get; private init; }
                public string Name { get; private init; }
                public string Actor { get; private init; }
                public ushort Episodes { get; private init; }
                public Wizardry WizardStatus { get; private init; }

                public static Russo Alex { get; } = new Russo(1411125, "Alex Russo", "Selena Gomez", 106, Wizardry.Current);
                [Check.IsPositive] public static Russo Justin { get; } = new Russo(1273708, "Justin Russo", "David Henrie", 106, Wizardry.Past);
                public static Russo Max { get; } = new Russo(1545471, "Max Russo", "Jake T. Austin", 106, Wizardry.Past);
                public static Russo Jerry { get; } = new Russo(217936, "Jerry Russo", "David DeLuise", 106, Wizardry.Past);
                public static Russo Theresa { get; } = new Russo(133566, "Theresa Russo", "Maria Canals-Barrera", 106, Wizardry.Never);
                public static Russo Kelbo { get; } = new Russo(307531, "Kelbo Russo", "Jeff Garlin", 3, Wizardry.Current);

                private Russo(ulong imdb, string name, string actor, ushort episodes, Wizardry status) {
                    IMDbID = imdb;
                    Name = name;
                    Actor = actor;
                    Episodes = episodes;
                    WizardStatus = status;
                }
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
                public struct Support {
                    public double LatestVersions { get; set; }
                    public RelationSet<double> SupportedVersions { get; }
                }

                [PrimaryKey] public string Name { get; set; } = "";
                public Type Category { get; set; }
                public DateTime Debut { get; set; }
                [Check.IsPositive(Path = "SupportedVersions.Item")] public Support Versions { get; set; }
                public RelationSet<string> Creators { get; } = new();
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
                [Check.IsPositive(Path = "Item.ID")] public RelationSet<Ingredient> Ingredients { get; } = new();
                public double AlcoholVolume { get; set; }
                public decimal Price { get; set; }
            }

            // Test Scenario: Applied to Nested Relation (✗impermissible✗)
            public class PulitzerPrize {
                public enum Categorization { Fiction, Music, Drama, Biography, Poetry, History, Reporting, Photography, Illustration }
                public record struct Committee {
                    public RelationList<string> Members { get; }
                    public string Chairperson { get; set; }
                }

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
                [PrimaryKey, DataConverter<ToInt<char>>, Check.IsPositive] public char Classification { get; set; }
                public bool HasDivingBoard { get; set; }
            }

            // Test Scenario: Applied to Field Data-Converted from Numeric Type (✗impermissible✗)
            public class WikipediaPage {
                [PrimaryKey] public string URL { get; set; } = "";
                public DateTime LastEdited { get; set; }
                public ulong WordCount { get; set; }
                [DataConverter<ToString<ushort>>, Check.IsPositive] public ushort Languages { get; set; }
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
                [Check.IsPositive(Path = "---")] public RelationMap<string, short> Builders { get; } = new();
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
                [Check.IsPositive(Path = "Item.WordCount")] public RelationList<Script> Scripts { get; } = new();
                public decimal Salary { get; set; }
                public bool WGAMember { get; set; }
            }

            // Test Scenario: <Path> on Relation Not Specified (✗missing path✗)
            public class Typewriter {
                [PrimaryKey] public Guid TypewriterID { get; set; }
                [Check.IsPositive] public RelationSet<char> MissingKeys { get;  } = new();
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

            // Test Scenario: Applied to DateOnly Field (✗impermissible✗)
            public class VenmoRequest {
                [PrimaryKey] public Guid ID { get; set; }
                public string Requestor { get; set; } = "";
                public string Requestee { get; set; } = "";
                public decimal Amount { get; set; }
                [Check.IsNegative] public DateOnly RequestedOn { get; set; }
                public bool Fulfilled { get; set; }
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

            // Test Scenario: Applied to Pre-Defined Instance (✗impermissible✗)
            [PreDefined] public class MBTI {
                public enum Attitude { Introversion, Extroversion };
                public enum Function1 { Intuition, Sensing };
                public enum Function2 { Thinking, Feeling };
                public enum Lifestyle { Judging, Perceiving };

                [PrimaryKey] public Attitude IE { get; private init; }
                [PrimaryKey] public Function1 NS { get; private init; }
                [PrimaryKey] public Function2 TF { get; private init; }
                [PrimaryKey] public Lifestyle JP { get; private init; }

                public static MBTI INTJ { get; } = new MBTI(Attitude.Introversion, Function1.Intuition, Function2.Thinking, Lifestyle.Judging);
                public static MBTI INTP { get; } = new MBTI(Attitude.Introversion, Function1.Intuition, Function2.Thinking, Lifestyle.Perceiving);
                public static MBTI ENTJ { get; } = new MBTI(Attitude.Extroversion, Function1.Intuition, Function2.Thinking, Lifestyle.Judging);
                public static MBTI ENTP { get; } = new MBTI(Attitude.Extroversion, Function1.Intuition, Function2.Thinking, Lifestyle.Perceiving);
                public static MBTI INFJ { get; } = new MBTI(Attitude.Introversion, Function1.Intuition, Function2.Feeling, Lifestyle.Judging);
                public static MBTI INFP { get; } = new MBTI(Attitude.Introversion, Function1.Intuition, Function2.Feeling, Lifestyle.Perceiving);
                public static MBTI ENFJ { get; } = new MBTI(Attitude.Extroversion, Function1.Intuition, Function2.Feeling, Lifestyle.Judging);
                public static MBTI ENFP { get; } = new MBTI(Attitude.Extroversion, Function1.Intuition, Function2.Feeling, Lifestyle.Perceiving);
                public static MBTI ISTJ { get; } = new MBTI(Attitude.Introversion, Function1.Sensing, Function2.Thinking, Lifestyle.Judging);
                public static MBTI ISTP { get; } = new MBTI(Attitude.Introversion, Function1.Sensing, Function2.Thinking, Lifestyle.Perceiving);
                public static MBTI ESTJ { get; } = new MBTI(Attitude.Extroversion, Function1.Sensing, Function2.Thinking, Lifestyle.Judging);
                public static MBTI ESTP { get; } = new MBTI(Attitude.Extroversion, Function1.Sensing, Function2.Thinking, Lifestyle.Perceiving);
                public static MBTI ISFJ { get; } = new MBTI(Attitude.Introversion, Function1.Sensing, Function2.Feeling, Lifestyle.Judging);
                [Check.IsNegative] public static MBTI ISFP { get; } = new MBTI(Attitude.Introversion, Function1.Sensing, Function2.Feeling, Lifestyle.Perceiving);
                public static MBTI ESFJ { get; } = new MBTI(Attitude.Extroversion, Function1.Sensing, Function2.Feeling, Lifestyle.Judging);
                public static MBTI ESFP { get; } = new MBTI(Attitude.Extroversion, Function1.Sensing, Function2.Feeling, Lifestyle.Perceiving);

                private MBTI(Attitude attitude, Function1 function1, Function2 function2, Lifestyle lifestyle) {
                    IE = attitude;
                    NS = function1;
                    TF = function2;
                    JP = lifestyle;
                }
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
                [Check.IsNegative(Path = "Item.PageEnd")] public RelationList<Section> Sections { get; } = new();
                public ushort NumImages { get; set; }
            }

            // Test Scenario: Applied to Relation-Nested Non-Numeric Scalar (✗impermissible✗)
            public class LandMine {
                public enum Kind { AntiTank, AntiPersonnel }

                [PrimaryKey] public Guid ID { get; set; }
                public float Latitude { get; set; }
                public float Longitude { get; set; }
                public bool Detonated { get; set; }
                [Check.IsNegative(Path = "Item")] public IReadOnlyRelationSet<string> Casualties { get; } = new RelationSet<string>();
                public Kind Type { get; set; }
            }

            // Test Scenario: Applied to Nested Relation (✗impermissible✗)
            public class YahtzeeGame {
                public enum Category { Ones, Twos, Threes, Fours, Five, Sixes, ThreeOAK, FourOAK, FullHouse, SmallSTR, LargeSTR, Yahtzee, Chance };
                public record struct Player {
                    public Guid PlayerID { get; set; }
                    public string Name { get; set; }
                    public RelationMap<Category, byte> Score { get; }
                }

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
                [DataConverter<ToInt<string>>, Check.IsNegative] public string TKOs { get; set; } = "";
            }

            // Test Scenario: Applied to Field Data-Converted from Numeric Type (✗impermissible✗)
            public class Archangel {
                [PrimaryKey] public string Name { get; set; } = "";
                public ulong OldTestamentMentions { get; set; }
                public ulong NewTestamentMentions { get; set; }
                public ulong ApocryphaMentions { get; set; }
                [DataConverter<ToString<float>>, Check.IsNegative] public float FirstAppearance { get; set; }
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
                [Check.IsNegative(Path = "---")] public RelationSet<DateTime> PossibleDates { get; } = new();
                public string? GreekEquivalent { get; set; }
            }

            // Test Scenario: <Path> on Relation Refers to Non-Primary-Key Field of Anchor Entity (✗non-existent path✗)
            public class AmberAlert {
                public enum Category { Make, Model, LicensePlate, Color, NumDoors, Wheels, Other }

                [PrimaryKey] public Guid AlertID { get; set; }
                public string ChildsName { get; set; } = "";
                public DateTime Issued { get; set; }
                [Check.IsNegative(Path = "AmberAlert.EmergencyContactNumber")] public RelationMap<Category, string> VehicleDescription { get; } = new();
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
                [Check.IsNegative] public RelationList<Singer> Members { get; } = new();
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

            // Test Scenario: Applied to DateOnly Field (✗impermissible✗)
            public class TieDye {
                public enum Garment { Shirt, Sock, Underwear, Shorts, Pants, Dress, Skirt, Hat, Jacket, Bra, Glove, Other }

                [PrimaryKey] public Guid ID { get; set; }
                [Check.IsNonZero] public DateOnly DateCreated { get; set; }
                public Garment Article { get; set; }
                public byte NumColors { get; set; }
                public decimal? RetailPrice { get; set; }
                public string? Artist { get; set; }
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

            // Test Scenario: Applied to Pre-Defined Instance (✗impermissible✗)
            [PreDefined] public class ChronicleOfNarnia {
                [PrimaryKey] public string Title { get; private init; }
                public ushort YearPublished { get; private init; }
                public ulong WordCount { get; private init; }

                public static ChronicleOfNarnia TMN { get; } = new ChronicleOfNarnia("The Magician's Nephew", 1955, 64000);
                [Check.IsNonZero] public static ChronicleOfNarnia TLTWATW { get; } = new ChronicleOfNarnia("The Lion, the Witch, and the Wardrobe", 1950, 38000);
                public static ChronicleOfNarnia THAHB { get; } = new ChronicleOfNarnia("The Horse and His Boy", 1954, 48000);
                public static ChronicleOfNarnia PC { get; } = new ChronicleOfNarnia("Prince Caspian", 1951, 46000);
                public static ChronicleOfNarnia TVOTDT { get; } = new ChronicleOfNarnia("The Voyage of the Dawn Treader", 1952, 54000);
                public static ChronicleOfNarnia TSC { get; } = new ChronicleOfNarnia("The Silver Chair", 1953, 51000);
                public static ChronicleOfNarnia TLB { get; } = new ChronicleOfNarnia("The Last Battle", 1956, 43000);

                private ChronicleOfNarnia(string title, ushort yearPublished, ulong wordCount) {
                    Title = title;
                    YearPublished = yearPublished;
                    WordCount = wordCount;
                }
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
                [Check.IsNonZero(Path = "Key")] public RelationMap<uint, string> Steps { get; } = new();
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
                [Check.IsNonZero(Path = "Item.ID")] public RelationSet<Company> CertifiedCompanies { get; } = new();
                public Judaism Branch { get; set; }
            }

            // Test Scenario: Applied to Nested Relation (✗impermissible✗)
            public class CarpoolKaraoke {
                public record struct Cantante {
                    public string Singer { get; set; }
                    public RelationList<string> Songs { get; }
                }

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
                [DataConverter<ToInt<char>>, Check.IsNonZero] public char ConsumerGrade { get; set; }
            }

            // Test Scenario: Applied to Field Data-Converted from Numeric Type (✗impermissible✗)
            public class Elevator {
                [PrimaryKey] public Guid ProductNumber { get; set; }
                public DateTime LastInspected { get; set; }
                public float MaxLoad { get; set; }
                [Check.IsNonZero, DataConverter<ToString<int>>] public int NumFloors { get; set; }
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
                [Check.IsNonZero(Path = "---")] public RelationList<string> PermittedLocations { get; } = new();
                public bool IsExpired { get; set; }
            }

            // Test Scenario: <Path> on Relation Refers to Non-Primary-Key Field of Anchor Entity (✗non-existent path✗)
            public class Casserole {
                public enum Kind { American, French, German, Portuguese, Greek, Scandinavian, Polish, Russian, Italian, Other }

                [PrimaryKey] public Guid ID { get; set; }
                public string Name { get; set; } = "";
                public Kind Cuisine { get; set; }
                public float IdealPanDepth { get; set; }
                [Check.IsNonZero(Path = "Casserole.IdealPanDepth")] public RelationMap<string, bool> Ingredients { get; } = new();
                public bool IsGratin { get; set; }
            }

            // Test Scenario: <Path> on Relation Not Specified (✗missing path✗)
            public class GarbageTruck {
                [PrimaryKey] public string LicensePlate { get; set; } = "";
                [Check.IsNonZero] public RelationSet<string> RouteStops { get; } = new();
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

            // Test Scenario: Applied to Date-ish Field (✓constrained✓)
            public class GoldRush {
                [PrimaryKey] public string Location { get; set; } = "";
                [PrimaryKey, Check.IsGreaterThan("1200-03-18")] public DateTime StartDate { get; set; }
                [PrimaryKey, Check.IsGreaterThan("1176-11-22")] public DateOnly EndDate { get; set; }
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

            // Test Scenario: Applied to Pre-Defined Instance (✗impermissible✗)
            [PreDefined] public class Gorgon {
                [PrimaryKey] public string EnglishName { get; private init; }
                public string GreekName { get; private init; }
                public bool CanPetrify { get; private init; }

                [Check.IsGreaterThan(100)] public static Gorgon Medusa { get; } = new Gorgon("Medusa", "Μέδουσα", true);
                public static Gorgon Stheno { get; } = new Gorgon("Stheno", "Σθενώ", false);
                public static Gorgon Euryale { get; } = new Gorgon("Euryale", "Εὐρυάλη", false);

                private Gorgon(string english, string greek, bool petrification) {
                    EnglishName = english;
                    GreekName = greek;
                    CanPetrify = petrification;
                }
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
                [Check.IsGreaterThan('@', Path = "Key"), Check.IsGreaterThan(0L, Path = "KidNextDoor.Number")] public RelationMap<char, string> DebutMission { get; } = new();
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
                [Check.IsGreaterThan("9dc286f1-3ce5-4bd6-9afa-5bb6049e0ffe", Path = "Item.SpellID")] public RelationList<Spell> KnownSpells { get; } = new();
                public DateTime Birthdate { get; set; }
                public bool Deceased { get; set; }
                public byte? BestEssenTaschPlacement { get; set; }
            }

            // Test Scenario: Applied to Nested Relation (✗impermissible✗)
            public class Clown {
                public enum Kind { Party, Jester, Demon, Circus, Rodeo, Other }
                public record struct Outfit {
                    public decimal Nose { get; set; }
                    public RelationList<string> Accoutrement { get; }
                    public double Shoes { get; set; }
                }

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
                [DataConverter<ToString<float>>, Check.IsGreaterThan(-237.44f)] public float BoilingPoint { get; set; }
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
                [PrimaryKey, DataConverter<ToString<int>>, Check.IsGreaterThan("-1")] public int CellR4C1 { get; set; }
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
                [Check.IsGreaterThan(18752.53f, Path = "---")] public RelationList<MenuItem> MenuItems { get; } = new();
                public bool OffersCatering { get; set; }
                public bool JewishStyle { get; set; }
            }

            // Test Scenario: <Path> on Relation Refers to Non-Primary-Key Field of Anchor Entity (✗non-existent path✗)
            public class BlackjackHand {
                public enum FaceValue { Ace, King, Queen, Jack, Ten, Nine, Eight, Seven, Six, Five, Four, Three, Two };
                public enum CardSuit { Hearts, Diamonds, Clubs, Spades }
                public record struct Card(FaceValue Value, CardSuit Suit);

                [PrimaryKey] public Guid HandID { get; set; }
                public RelationSet<Card> PlayerCards { get; } = new();
                [Check.IsGreaterThan(17512UL, Path = "BlackjackHand.TotalPot")] public RelationSet<Card> DealerCards { get; } = new();
                public ulong TotalPot { get; set; }
                public bool DoubleDown { get; set; }
            }

            // Test Scenario: <Path> on Relation Not Specified (✗missing path✗)
            public class Inquisition {
                [PrimaryKey] public DateTime Start { get; set; }
                [PrimaryKey] public DateTime End { get; set; }
                public string OfficialTitle { get; set; } = "";
                public string GrandInquisitor { get; set; } = "";
                [Check.IsGreaterThan("Religious Persecution")] public RelationSet<string> Victims { get; } = new();
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

            // Test Scenario: Applied to Date-ish Field (✓constrained✓)
            public class Commercial {
                [PrimaryKey] public ushort Channel { get; set; }
                [PrimaryKey, Check.IsLessThan("2300-01-01")] public DateTime TimeSlot { get; set; }
                public byte LengthSeconds { get; set; }
                public bool ForSuperBowl { get; set; }
                public string? Company { get; set; } = "";
                [Check.IsLessThan("2137-04-17")] public DateOnly? DateOfPitch { get; set; }
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

            // Test Scenario: Applied to Pre-Defined Instance (✗impermissible✗)
            [PreDefined] public class CharacterEncoding {
                [PrimaryKey] public string Name { get; private init; }
                public sbyte CodeUnitBits { get; private init; }
                public bool IsFixedLength { get; private init; }

                public static CharacterEncoding ASCII { get; } = new CharacterEncoding("ASCII", 7, true);
                public static CharacterEncoding UTF8 { get; } = new CharacterEncoding("UTF-8", 8, false);
                public static CharacterEncoding EBCDIC { get; } = new CharacterEncoding("EBCDIC", 8, false);
                public static CharacterEncoding GB18030 { get; } = new CharacterEncoding("GB 18030", 8, false);
                public static CharacterEncoding UTF16 { get; } = new CharacterEncoding("UTF-16", 16, false);
                [Check.IsLessThan(100)] public static CharacterEncoding UTF32 { get; } = new CharacterEncoding("UTF-32", 32, true);

                private CharacterEncoding(string name, sbyte codeUnitBits, bool isFixedLength) {
                    Name = name;
                    CodeUnitBits = codeUnitBits;
                    IsFixedLength = isFixedLength;
                }
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
                [Check.IsLessThan("Warmonger", Path = "FairyGodparent.Name"), Check.IsLessThan((ushort)1851, Path = "Key"), Check.IsLessThan((ushort)42144, Path = "Value")] public RelationMap<ushort, ushort> LinesByEpisode { get; } = new();
                public uint TimesDaRulesBroken { get; set; }
            }

            // Test Scenario: Applied to Relation-Nested Non-Orderable Scalar (✗impermissible✗)
            public class NavalBlockade {
                public enum AquaKind { Ocean, River, Stream, Lake, Estuary, Swamp, Inlet, Bay }
                public record struct Waterway(string Name, AquaKind Kind);

                [PrimaryKey] public Guid ID { get; set; }
                public DateTime Instituted { get; set; }
                public DateTime? Lifted { get; set; }
                [Check.IsLessThan(AquaKind.Swamp, Path = "Item.Kind")] public RelationSet<Waterway> WaterwaysAffected { get; } = new();
                public decimal EconomicImpact { get; set; }
                public bool ActOfWar { get; set; }
                public ushort NumShips { get; set; }
            }

            // Test Scenario: Applied to Nested Relation (✗impermissible✗)
            public class Blacksmith {
                public struct Inventory {
                    public RelationSet<string> Metals { get; }
                    public RelationSet<string> Hammers { get; }
                }

                [PrimaryKey] public Guid InternationalBlacksmithNumber { get; set; }
                public string Address { get; set; } = "";
                public DateTime Birthdate { get; set; }
                [Check.IsLessThan(1000, Path = "Hammers")] public Inventory Materials { get; set; }
                public RelationMap<string, decimal> Prices { get; } = new();
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
                [DataConverter<ToInt<byte>>, Check.IsLessThan((byte)8)] public byte NumToes { get; set; }
            }

            // Test Scenario: Anchor of Target Type on Data-Converted Property (✓valid✓)
            public class Phobia {
                [PrimaryKey] public Guid DSMID { get; set; }
                public string FullName { get; set; } = "";
                public string FearOf { get; set; } = "";
                [DataConverter<ToString<double>>, Check.IsLessThan("100.00001")] public double Prevalence { get; set; }
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
                [Check.IsLessThan(0L, Path = "---")] public RelationList<Performance> Performances { get; } = new();
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
                [Check.IsLessThan("2657-03-19", Path = "CatholicCardinal.DeathDate")] public RelationSet<DateTime> Conclaves { get; } = new();
                public bool PreviouslyArchbishop { get; set; }
            }

            // Test Scenario: <Path> on Relation Not Specified (✗missing path✗)
            public class Hemalurgy {
                [PrimaryKey] public string Metal { get; set; } = "";
                [Check.IsLessThan("Allomantic Powers")] public RelationSet<string> Steals { get; } = new();
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

            // Test Scenario: Applied to Date-ish Field (✓constrained✓)
            public class PEP {
                [PrimaryKey] public int Number { get; set; }
                public string URL { get; set; } = "";
                public string Title { get; set; } = "";
                public ushort NumNewKeywords { get; set; }
                [Check.IsGreaterOrEqualTo("1887-04-29")] public DateTime CreatedOn { get; set; }
                [Check.IsGreaterOrEqualTo("1902-11-04")] public DateOnly AdoptedOn { get; set; }
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

            // Test Scenario: Applied to Pre-Defined Instance (✗impermissible✗)
            [PreDefined] public class Squash {
                [PrimaryKey] public string Genus { get; private init; }
                [PrimaryKey] public string Species { get; private init; }
                public string CommonName { get; private init; }

                public static Squash Pumpkin { get; } = new Squash("Cucurbita", "maxima", "pumpkin");
                public static Squash Fingerleaf { get; } = new Squash("Cucurbita", "digitata", "fingerleaf gourd");
                [Check.IsGreaterOrEqualTo(100)] public static Squash Chilacayote { get; } = new Squash("Cucurbita", "ficifolia", "chilacayote");
                public static Squash Stinking { get; } = new Squash("Cucurbita", "foetidissima", "stinking gourd");
                public static Squash Butternut { get; } = new Squash("Cucurbita", "moschata", "butternut squash");
                public static Squash Zucchini { get; } = new Squash("Cucurbita", "pepo", "zucchini");
                public static Squash Calabacilla { get; } = new Squash("Cucurbita", "radicans", "calabacilla");
                public static Squash Ecuadorian { get; } = new Squash("Cucurbita", "ecuadorensis", "Ecuadorian squash");
                public static Squash Okeechobee { get; } = new Squash("Cucurbita", "okeechobeensis", "Okeechobee gourd");

                private Squash(string genus, string species, string common) {
                    Genus = genus;
                    Species = species;
                    CommonName = common;
                }
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
                [Check.IsGreaterOrEqualTo("Elmo", Path = "Item.Name"), Check.IsGreaterOrEqualTo(22.5, Path = "Item.Value")] public RelationSet<Puppet> Puppets { get; } = new();
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
                [Check.IsGreaterOrEqualTo(false, Path = "Value.Discovered")] public RelationMap<string, Pair> HidingPlaces { get; } = new();
            }

            // Test Scenario: Applied to Nested Relation (✗impermissible✗)
            public class WheresWaldo {
                public record struct Coordinate(float X, float Y);
                public struct Quadrant {
                    public byte Number { get; set; }
                    public RelationList<Coordinate> Decoys { get; }
                }

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
                [DataConverter<ToInt<double>>, Check.IsGreaterOrEqualTo(-18.0933)] public double AverageWeight { get; set; }
            }

            // Test Scenario: Anchor of Target Type on Data-Converted Property (✓valid✓)
            public class EMail {
                [PrimaryKey] public string Sender { get; set; } = "";
                [PrimaryKey] public string Recipient { get; set; } = "";
                [PrimaryKey] public DateTime Sent { get; set; }
                public string Subject { get; set; } = "";
                [DataConverter<ToInt<string>>, Check.IsGreaterOrEqualTo(73)] public string CC { get; set; } = "";
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
                [Check.IsGreaterOrEqualTo("MacLeod", Path = "---")] public RelationSet<Guid> Swords { get; } = new();
                public double Height { get; set; }
            }

            // Test Scenario: <Path> on Relation Refers to Non-Primary-Key Field of Anchor Entity (✗non-existent path✗)
            public class Synagogue {
                public enum Judaism { Reform, Conservative, Orthodox, Chabad, Kabbalah, Reconstructionist }

                [PrimaryKey] public Guid ID { get; set; }
                public Judaism Denomination { get; set; }
                public ulong SquareFootage { get; set; }
                [Check.IsGreaterOrEqualTo("%999u$", Path = "Synagogue.Denomination")] public RelationSet<string> Congregants { get; } = new();
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
                [Check.IsGreaterOrEqualTo(68174)] public RelationMap<int, string> Lines { get; } = new();
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

            // Test Scenario: Applied to Date-ish Field (✓constrained✓)
            public class Representative {
                [PrimaryKey] public string Name { get; set; } = "";
                public string Party { get; set; } = "";
                public long District { get; set; }
                public string State { get; set; } = "";
                [Check.IsLessOrEqualTo("2688-12-02")] public DateTime FirstElected { get; set; }
                [Check.IsLessOrEqualTo("4199-08-08")] public DateOnly? DateOfResignation { get; set; }
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

            // Test Scenario: Applied to Pre-Defined Instance (✗impermissible✗)
            [PreDefined] public class ChannelIsland {
                [PrimaryKey] public string Name { get; private init; }
                public string? Capital { get; private init; }
                public double Area { get; private init; }

                [Check.IsLessOrEqualTo(100)] public static ChannelIsland Jersey { get; } = new ChannelIsland("Jersey", "St. Helier", 46.2);
                public static ChannelIsland Guernsey { get; } = new ChannelIsland("Guernsey", "Saint Peter POrt", 24.0);
                public static ChannelIsland Alderney { get; } = new ChannelIsland("Alderney", null, 3.0);
                public static ChannelIsland Sark { get; } = new ChannelIsland("Sark", null, 2.1);
                public static ChannelIsland Herm { get; } = new ChannelIsland("Herm", null, 0.77);
                public static ChannelIsland Jethou { get; } = new ChannelIsland("Jethou", null, 0.077);
                public static ChannelIsland Brecqhou { get; } = new ChannelIsland("Brecqhou", null, 0.12);

                private ChannelIsland(string name, string? capital, double area) {
                    Name = name;
                    Capital = capital;
                    Area = area;
                }
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
                [Check.IsLessOrEqualTo('|', Path = "Value")] public RelationMap<char, char> IPA { get; } = new();
                public bool IsAlphasyllabary { get; set; }
            }

            // Test Scenario: Applied to Relation-Nested Non-Orderable Scalar (✗impermissible✗)
            public class TreehouseOfHorror {
                public enum Role { VoiceActor, VoiceActress, Animator, Writer, Producer, Editor, SoundArist }

                [PrimaryKey] public byte Season { get; set; }
                [PrimaryKey] public byte EpisodeNumber { get; set; }
                public DateTime AirDate { get; set; }
                public bool IsParody { get; set; }
                public RelationMap<string, Role> Cast { get; } = new();
                [Check.IsLessOrEqualTo("Robert", Path = "Value")] public RelationMap<string, Role> Crew { get; } = new();
                public string Segment1Title { get; set; } = "";
                public string Segment2Title { get; set; } = "";
                public string Segment3Title { get; set; } = "";
                public bool KangAndKodos { get; set; }
            }

            // Test Scenario: Applied to Nested Relation (✗impermissible✗)
            public class CocoaFarm {
                public struct PersonnelGroup {
                    public string Owner { get; set; }
                    public RelationSet<string> Workers { get; }
                    public RelationList<string> Regulators { get; }
                }

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
                [DataConverter<ToInt<bool>>, Check.IsLessOrEqualTo(false)] public bool Splintered { get; set; }
            }

            // Test Scenario: Anchor of Target Type on Data-Converted Property (✓valid✓)
            public class HTMLElement {
                [PrimaryKey] public string ID { get; set; } = "";
                public string? Class { get; set; }
                public bool Hidden { get; set; }
                public string? InnerHTML { get; set; }
                public string? InlineStyle { get; set; }
                [DataConverter<ToString<uint>>, Check.IsLessOrEqualTo("400000")] public uint NumChildren { get; set; }
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
                [Check.IsLessOrEqualTo(105, Path = "---")] public RelationList<Play> PlayByPlay { get; } = new();
                public Result Outcome { get; set; }
            }

            // Test Scenario: <Path> on Relation Refers to Non-Primary-Key Field of Anchor Entity (✗non-existent path✗)
            public class EngagementRing {
                public enum Stone { Diamond, Ruby, Sapphire, Peridot, Amethyst }
                public record struct Measurement(double Value, string Unit);

                [PrimaryKey] public Guid ID { get; set; }
                public Stone Centerpiece { get; set; }
                [Check.IsLessOrEqualTo(285712905UL, Path = "EngagementRing.Centerpiece")] public RelationMap<string, Measurement> Measurements { get; } = new();
                public decimal Price { get; set; }
                public bool FamilyHeirloom { get; set; }
                public string Wearer { get; set; } = "";
            }

            // Test Scenario: <Path> on Relation Not Specified (✗missing path✗)
            public class ImpracticalJoke {
                public enum Joker { Sal, Q, Murr, Joe, Guest }

                [PrimaryKey] public Guid JokeID { get; set; }
                public Joker ImpracticalJoker { get; set; }
                [Check.IsLessOrEqualTo((short)7512)] public RelationSet<string> JokeTargets { get; } = new();
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

            // Test Scenario: Applied to Date-ish Field (✓constrained✓)
            public class SlotMachine {
                [PrimaryKey] public Guid MachineNumber { get; set; }
                public ulong Jackpot { get; set; }
                public decimal LeverCost { get; set; }
                [Check.IsNot("4431-01-21")] public DateTime InstalledOn { get; set; }
                [Check.IsNot("1010-10-10")] public DateOnly FirstPlayed { get; set; }
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

            // Test Scenario: Applied to Pre-Defined Instance (✗impermissible✗)
            [PreDefined] public class Sexuality {
                [PrimaryKey] public Guid ID { get; private init; }
                public string Name { get; private init; }

                public static Sexuality Heterosexuality { get; } = new Sexuality("heterosexuality");
                public static Sexuality Homosexuality { get; } = new Sexuality("homosexuality");
                [Check.IsNot(100)] public static Sexuality Bisexuality { get; } = new Sexuality("bisexuality");
                public static Sexuality Pansexuality { get; } = new Sexuality("pansexuality");
                public static Sexuality Asexuality { get; } = new Sexuality("asexuality");
                public static Sexuality Sapiosexuality { get; } = new Sexuality("sapiosexuality");
                public static Sexuality Polysexuality { get; } = new Sexuality("polysexuality");

                private Sexuality(string name) {
                    ID = Guid.NewGuid();
                    Name = name;
                }
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
                [Check.IsNot(Kind.Other, Path = "Item.Kind"), Check.IsNot(false, Path = "Item.NSFW")] public RelationList<Joke> Jokes { get; } = new();
                public DateTime FirstShow { get; set; }
                public bool LateNightHost { get; set; }
                public ulong TwitterFollowers { get; set; }
            }

            // Test Scenario: Applied to Nested Relation (✗impermissible✗)
            public class Interview {
                public struct Course {
                    public RelationMap<string, double> Questions { get; }
                }

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
                [DataConverter<ToString<bool>>, Check.IsNot(false)] public bool Disneyfied { get; set; }
            }

            // Test Scenario: Anchor of Target Type on Data-Converted Property (✓valid✓)
            public class RingOfPower {
                [PrimaryKey] public string Name { get; set; } = "";
                public string? Holder { get; set; }
                public bool Destroyed { get; set; }
                public DateTime Forged { get; set; }
                public string CentralStone { get; set; } = "";
                [DataConverter<ToInt<ushort>>, Check.IsNot(7)] public ushort NumPossessors { get; set; }
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
                public RelationSet<string> BooksBurned { get; } = new();
                public RelationSet<string> PeopleBurned { get; } = new();
                [Check.IsNot("Mona Lisa", Path = "---")] public RelationSet<string> ArtworkBurned { get; } = new();
            }

            // Test Scenario: <Path> on Relation Refers to Non-Primary-Key Field of Anchor Entity (✗non-existent path✗)
            public class Dream {
                public enum Kind { Fantasy, Daydream, Nightmare, SexDream, NightTerror, Hallucination, Other }

                [PrimaryKey] public string Dreamer { get; set; } = "";
                [PrimaryKey] public DateTime DateOfDream { get; set; }
                [PrimaryKey] public uint SequenceNumber { get; set; }
                public sbyte Length { get; set; }
                [Check.IsNot("Hercules", Path = "Dream.REM")] public RelationList<string> Cameos { get; } = new();
                public bool REM { get; set; }
            }

            // Test Scenario: <Path> on Relation Not Specified (✗missing path✗)
            public class BachelorParty {
                public enum Kind { StripClub, Restaurant, SportingEvent, Casino, Home, Beach, Other }
                public record struct Destination(sbyte Order, string Location, decimal AmountSpent, Kind Kind);

                [PrimaryKey] public string Bachelor { get; set; } = "";
                [PrimaryKey] public DateTime Date { get; set; }
                public sbyte Attendees { get; set; }
                [Check.IsNot((byte)121)] public RelationList<Destination> Destinations { get; } = new();
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

            // Test Scenario: Applied to DateOnly Field (✗impermissible✗)
            public class Fudge {
                [PrimaryKey] public Guid ID { get; set; }
                [Check.IsNonEmpty] public DateOnly Baked { get; set; }
                public string Flavor { get; set; }
                public double CaloriesPerGram { get; set; }
                public decimal PricePerPound { get; set; }
                public bool IsEaten { get; set; }
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
            public class Moustache {
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

            // Test Scenario: Applied to Pre-Defined Instance (✗impermissible✗)
            [PreDefined] public class IronChef {
                public enum Show { Japanese, American }

                [PrimaryKey] public string Name { get; private init; }
                [PrimaryKey] public Show Iteration { get; private init; }
                public double WinPercentage { get; private init; }

                public static IronChef Kenichi { get; } = new IronChef("Chen Kenichi", Show.Japanese, 72.6);
                public static IronChef Ishinabe { get; } = new IronChef("Yutaka Ishiabe", Show.Japanese, 87.5);
                public static IronChef Sakai { get; } = new IronChef("Hiroyuki Sakai", Show.Japanese, 82.4);
                public static IronChef Kobe { get; } = new IronChef("Masahiko Sabe", Show.Japanese, 68.8);
                public static IronChef Michiba { get; } = new IronChef("Rokusaburo Michiba", Show.Japanese, 85.9);
                public static IronChef Nakamura { get; } = new IronChef("Koumei Nakamura", Show.Japanese, 66.2);
                public static IronChef MorimotoJ { get; } = new IronChef("Masaharu Morimoto", Show.Japanese, 67.3);
                public static IronChef Batali { get; } = new IronChef("Mario Batali", Show.American, 79.2);
                public static IronChef Cora { get; } = new IronChef("Cat Cora", Show.American, 63.2);
                public static IronChef Flay { get; } = new IronChef("Bobby Flay", Show.American, 72.1);
                public static IronChef Forgione { get; } = new IronChef("Marc Forgione", Show.American, 53.0);
                public static IronChef Garces { get; } = new IronChef("Jose Garces", Show.American, 69.6);
                [Check.IsNonEmpty] public static IronChef Guarnaschelli { get; } = new IronChef("Alex Guarnaschelli", Show.American, 63.6);
                public static IronChef Izard { get; } = new IronChef("Stephanie Izard", Show.American, 100.0);
                public static IronChef MorimotoA { get; } = new IronChef("Masaharu Morimoto", Show.American, 60.2);
                public static IronChef Puck { get; } = new IronChef("Wolfgang Puck", Show.American, 100.0);
                public static IronChef Symon { get; } = new IronChef("Michael Symon", Show.American, 82.1);
                public static IronChef Zakarian { get; } = new IronChef("Geoffrey Zakarian", Show.American, 64.3);

                private IronChef(string name, Show version, double winPercent) {
                    Name = name;
                    Iteration = version;
                    WinPercentage = winPercent;
                }
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
                [Check.IsNonEmpty(Path = "Item")] public RelationSet<string> Sponsors { get; } = new();
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
                [Check.IsNonEmpty(Path = "Value.MallID")] public RelationMap<ushort, Mall> Jobs { get; } = new();
                public ulong TotalKids { get; set; }
                public bool NaturalBeard { get; set; }
            }

            // Test Scenario: Applied to Nested Relation (✗impermissible✗)
            public class ConnectingWall {
                public enum Wall { Lion, Water }
                public struct Category {
                    public uint Color { get; set; }
                    public string Connection { get; set; }
                    public RelationList<string> Squares { get; }
                }

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
                [DataConverter<ToString<ushort>>, Check.IsNonEmpty] public ushort Duration { get; set; }
                public double Weight { get; set; }
                public double Height { get; set; }
            }

            // Test Scenario: Applied to Field Data-Converted from String Type (✗impermissible✗)
            public class FoodChain {
                [PrimaryKey] public string Producer { get; set; } = "";
                [PrimaryKey] public string PrimaryConsumer { get; set; } = "";
                [PrimaryKey, DataConverter<ToInt<string>>, Check.IsNonEmpty] public string SecondaryConsumer { get; set; } = "";
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
                [Check.IsNonEmpty(Path = "---")] public RelationSet<sbyte> MetamorphosesAppearances { get; } = new();
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
                [Check.IsNonEmpty(Path = "DatingApp.CEO")] public RelationList<Pair> CouplesFormed { get; } = new();
                public bool SwipeBased { get; set; }
                public decimal? MonthlyFee { get; set; }
            }

            // Test Scenario: <Path> on Relation Not Specified (✗missing path✗)
            public class AdBlocker {
                public enum AdType { PopUp, CouponSuggestion, VideoInterrupt, Sponsorships, GoogleAnalytics }

                [PrimaryKey] public string Name { get; set; } = "";
                public ulong Downloads { get; set; }
                public bool Free { get; set; }
                [Check.IsNonEmpty] public RelationSet<AdType> EffectiveAgainst { get; } = new();
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

            // Test Scenario: Applied to DateOnly Field (✗impermissible✗)
            public class Skateboard {
                [PrimaryKey] public Guid ID { get; set; }
                public byte NumWheels { get; set; }
                [Check.LengthIsAtLeast(73)] public DateOnly ManufactureDate { get; set; }
                public float Length { get; set; }
                public bool OwnedByTonyHawk { get; set; }
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

            // Test Scenario: Applied to Pre-Defined Instance (✗impermissible✗)
            [PreDefined] public class LEPBranch {
                [PrimaryKey] public string BranchName { get; private init; }
                public string? Captain { get; private init; }

                public static LEPBranch Immigration { get; } = new LEPBranch("Immigration", null);
                public static LEPBranch InternalAffairs { get; } = new LEPBranch("Internal Affairs", "Ark Sool");
                public static LEPBranch KrakenWatch { get; } = new LEPBranch("Kraken Watch", null);
                public static LEPBranch Geological { get; } = new LEPBranch("LEP Geological Unit", "Foaly");
                public static LEPBranch LEPmarine { get; } = new LEPBranch("LEPmarine", null);
                [Check.LengthIsAtLeast(100)] public static LEPBranch LEPRecon { get; } = new LEPBranch("LEPRecon", "Holly Short");
                public static LEPBranch LEPRetrieval { get; } = new LEPBranch("LEPRetrieval", "Trouble Kelp");
                public static LEPBranch LEPtraffic { get; } = new LEPBranch("LEPtraffic", null);
                public static LEPBranch RapidResponse { get; } = new LEPBranch("Rapid Response Team", null);
                public static LEPBranch SectionEight { get; } = new LEPBranch("Section Eight", "Raine Vinyáya");
                public static LEPBranch Telekinetic { get; } = new LEPBranch("Telekinetic Branch", null);

                private LEPBranch(string name, string? captain) {
                    BranchName = name;
                    Captain = captain;
                }
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
                [Check.LengthIsAtLeast(6, Path = "Item.Title")] public RelationList<Resolution> ResolutionsPassed { get; } = new();
                [Check.LengthIsAtLeast(17, Path = "UNSecretaryGeneral.Name")] public RelationSet<string> CountriesAdmitted { get; } = new();
            }

            // Test Scenario: Applied to Relation-Nested Non-String Scalar (✗impermissible✗)
            public class MemoryBuffer {
                [PrimaryKey] public ulong StartAddress { get; set; }
                [PrimaryKey] public ulong EndAddress { get; set; }
                [Check.LengthIsAtLeast(489, Path = "MemoryBuffer.EndAddress")] public RelationList<bool> Bits { get; } = new();
                public string IntendedType { get; set; } = "";
                public bool HeapAllocated { get; set; }
            }

            // Test Scenario: Applied to Nested Relation (✗impermissible✗)
            public class HotTub {
                public struct Settings {
                    public RelationMap<string, int> PresetSpeeds { get; }
                    public double DefaultTemperature { get; set; }
                    public bool Bubbles { get; set; }
                }

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
                [DataConverter<ToString<DateTime>>, Check.LengthIsAtLeast(10)] public DateTime Assumed { get; set; }
                public DateTime Terminated { get; set; }
            }

            // Test Scenario: Applied to Field Data-Converted from String Type (✗impermissible✗)
            public class Campfire {
                [PrimaryKey] public Guid GUID { get; set; }
                public DateTime Started { get; set; }
                public DateTime? Fizzled { get; set; }
                public double Temperature { get; set; }
                [DataConverter<ToInt<string>>, Check.LengthIsAtLeast(4)] public string WoodType { get; set; } = "";
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
                [Check.LengthIsAtLeast(52, Path = "---")] public RelationMap<Tobacco, double> Contents { get; } = new();
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
                [Check.LengthIsAtLeast(4, Path = "MarijuanaStrain.StrainName")] public RelationSet<Dispensary> SoldAt { get; } = new();
                public double Addictiveness { get; set; }
            }

            // Test Scenario: <Path> on Relation Not Specified (✗missing path✗)
            public class BankRobber {
                public enum BankType { Physical, Crypto, Train, MonopolyGame }
                public record struct Robbery(string Bank, decimal Haul, BankType TargetType);

                [PrimaryKey] public string Name { get; set; } = "";
                public Guid? FBINumber { get; set; }
                [Check.LengthIsAtLeast(87)] public RelationList<Robbery> Robberies { get; } = new();
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

            // Test Scenario: Applied to DateOnly Field (✗impermissible✗)
            public class MontyPythonSkit {
                [Flags] public enum Python { Cleese = 1, Chapman = 2, Idle = 4, Gilliam = 8, Palin = 16, Jones = 32 }

                [PrimaryKey] public string Title { get; set; } = "";
                public double Duration { get; set; }
                public Python Cast { get; set; }
                [Check.LengthIsAtMost(180)] public DateOnly OriginalAirdate { get; set; }
                public bool InFilm { get; set; }
                public string Script { get; set; } = "";
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

            // Test Scenario: Applied to Pre-Defined Instance (✗impermissible✗)
            [PreDefined] public class TonyAward {
                public enum ShowType { Musical, Play }

                [PrimaryKey] public string Category { get; private init; }
                [PrimaryKey] public ShowType Kind { get; private init; }

                public static TonyAward PlayLeadingActor { get; } = new TonyAward("Leading Actor", ShowType.Play);
                public static TonyAward PlayLeadingActress { get; } = new TonyAward("Leading Actress", ShowType.Play);
                public static TonyAward PlayFeaturedActor { get; } = new TonyAward("Featured Actor", ShowType.Play);
                public static TonyAward PlayFeaturedActress { get; } = new TonyAward("Featured Actress", ShowType.Play);
                public static TonyAward MusicalLeadingActor { get; } = new TonyAward("Leading Actor", ShowType.Musical);
                public static TonyAward MusicalLeadingActress { get; } = new TonyAward("Leading Actress", ShowType.Musical);
                public static TonyAward MusicalFeaturedActor { get; } = new TonyAward("Featured Actor", ShowType.Musical);
                public static TonyAward MusicalFeaturedActress { get; } = new TonyAward("Featured Actress", ShowType.Musical);
                public static TonyAward Play { get; } = new TonyAward("Play", ShowType.Play);
                public static TonyAward PlayRevival { get; } = new TonyAward("Revival", ShowType.Play);
                public static TonyAward PlayDirection { get; } = new TonyAward("Direction", ShowType.Play);
                public static TonyAward PlayScenicDesign { get; } = new TonyAward("Scenic Design", ShowType.Play);
                public static TonyAward PlaySoundDesign { get; } = new TonyAward("Sound Design", ShowType.Play);
                public static TonyAward PlayCostumeDesign { get; } = new TonyAward("Costume Design", ShowType.Play);
                public static TonyAward PlayLightingDesign { get; } = new TonyAward("Lighting Design", ShowType.Play);
                public static TonyAward Musical { get; } = new TonyAward("Musical", ShowType.Musical);
                [Check.LengthIsAtMost(100)] public static TonyAward MusicalRevival { get; } = new TonyAward("Revival", ShowType.Musical);
                public static TonyAward MusicalDirection { get; } = new TonyAward("Direction", ShowType.Musical);
                public static TonyAward Score { get; } = new TonyAward("Score", ShowType.Musical);
                public static TonyAward Orchestrations { get; } = new TonyAward("Orchestrations", ShowType.Musical);
                public static TonyAward Choreography { get; } = new TonyAward("Choreography", ShowType.Musical);
                public static TonyAward MusicalScenicDesign { get; } = new TonyAward("Scenic Design", ShowType.Musical);
                public static TonyAward MusicalSoundDesign { get; } = new TonyAward("Sound Design", ShowType.Musical);
                public static TonyAward MusicalCostumeDesign { get; } = new TonyAward("Costume Design", ShowType.Musical);
                public static TonyAward MusicalLightingDesign { get; } = new TonyAward("Lighting Design", ShowType.Musical);

                private TonyAward(string category, ShowType kind) {
                    Category = category;
                    Kind = kind;
                }
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
                [Check.LengthIsAtMost(26, Path = "Item")] public RelationSet<string> Materials { get; } = new();
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
                [Check.LengthIsAtMost(255, Path = "Item.Grams")] public RelationSet<Ingredient> Ingredients { get; } = new();
                public Color BalsamicColor { get; set; }
                public bool DOP { get; set; }
            }

            // Test Scenario: Applied to Nested Relation (✗impermissible✗)
            public class TerroristOrganization {
                public struct Record {
                    public RelationMap<string, DateTime> Entities { get; }
                }

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
                [DataConverter<ToString<float>>, Check.LengthIsAtMost(14)] public float Volume { get; set; }
                public decimal CleanupCost { get; set; }
                public string? ResponsibleParty { get; set; }
            }

            // Test Scenario: Applied to Field Data-Converted from String Type (✗impermissible✗)
            public class RandomNumberGenerator {
                [PrimaryKey] public Guid ID { get; set; }
                public ulong Seed { get; set; }
                [DataConverter<ToInt<string>>, Check.LengthIsAtMost(40)] public string Algorithm { get; set; } = "";
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
                [Check.LengthIsAtMost(30, Path = "---")] public RelationList<string> CardinalsCreated { get; } = new();
                public bool Excommunicated { get; set; }
            }

            // Test Scenario: <Path> on Relation Refers to Non-Primary-Key Field of Anchor Entity (✗non-existent path✗)
            public class Cabaret {
                public enum Kind { Bar, Restaurant, Casino, Nightclub, StripClub, Hotel, ParkDistrict, School, Auditorium, Other }
                public record struct Loc(string Name, Kind Kind);

                [PrimaryKey] public Guid CabaretID { get; set; }
                public DateTime Date { get; set; }
                public Loc Venue { get; set; }
                [Check.LengthIsAtMost(102, Path = "Cabaret.Venue.Name")] public RelationMap<string, string> Performers { get; } = new();
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
                [Check.LengthIsAtMost(100000)] public RelationMap<string, ulong> Views { get; } = new();
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

            // Test Scenario: Applied to DateOnly Field (✗impermissible✗)
            public class Tumbleweed {
                [PrimaryKey] public Guid ID { get; set; }
                public float Weight { get; set; }
                [Check.LengthIsBetween(7, 7777)] public DateOnly LastViewedOn { get; set; }
                public bool UsedInWesternFilm { get; set; }
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

            // Test Scenario: Applied to Pre-Defined Instance (✗impermissible✗)
            [PreDefined] public class HighSchoolMusical {
                [PrimaryKey] public int Index { get; private init; }
                public string Title { get; private init; }
                public DateTime Premiere { get; private init; }
                public bool WasDCOM { get; private init; }

                [Check.LengthIsBetween(1, 100)] public static HighSchoolMusical HSM { get; } = new HighSchoolMusical(1, "High School Musical", new DateTime(2006, 1, 20), true);
                public static HighSchoolMusical HSM2 { get; } = new HighSchoolMusical(2, "High School Musical 2", new DateTime(2007, 8, 17), true);
                public static HighSchoolMusical HSM3 { get; } = new HighSchoolMusical(3, "High School Musical 3: Senior Year", new DateTime(2008, 10, 24), false);
                public static HighSchoolMusical SharpaysFabulousAdventure { get; } = new HighSchoolMusical(4, "Sharpay's Fabulous Adventure", new DateTime(2011, 4, 19), false);

                private HighSchoolMusical(int index, string title, DateTime premiere, bool dcom) {
                    Index = index;
                    Title = title;
                    Premiere = premiere;
                    WasDCOM = dcom;
                }
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
                [Check.LengthIsBetween(4, 37, Path = "Item"), Check.LengthIsBetween(8, 19, Path = "ComicBook.Title")] public RelationSet<string> Characters { get; } = new();
                public decimal Price { get; set; }
                public bool Vintage { get; set; }
            }

            // Test Scenario: Applied to Relation-Nested Non-String Scalar (✗impermissible✗)
            public class Wormhole {
                public enum Kind { Schwarzschild, EinsteinRosenBridge, KleinBottle }
                public record struct Location(float X, float Y, float Z, DateTime Time, uint UniverseID);

                [PrimaryKey] public Guid ID { get; set; }
                public Kind Variety { get; set; }
                [Check.LengthIsBetween(14, 99, Path = "Item.Z")] public RelationSet<Location> ConnectedLocations { get; } = new();
                public double Radius { get; set; }
                public ulong Density { get; set; }
            }

            // Test Scenario: Applied to Nested Relation (✗impermissible✗)
            public class LunarEclipse {
                public enum Kind { Penumbral, Partial, Total, Central, Selenion }
                public record struct Coordinate(float Latitude, float Longitude);
                public struct Viewing {
                    public RelationMap<Coordinate, double> Locations { get; }
                    public bool NakedEye { get; set; }
                }

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
                [Check.LengthIsBetween(1, 15), DataConverter<ToString<int>>] public int? Ajah { get; set; }
            }

            // Test Scenario: Applied to Field Data-Converted from String Type (✗impermissible✗)
            public class AtmosphericLayer {
                [PrimaryKey, DataConverter<ToInt<string>>, Check.LengthIsBetween(12, 29)] public string Name { get; set; } = "";
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
                [Check.LengthIsBetween(177, 179, Path = "---")] public RelationList<Specimen> SpecimensCollected { get; } = new();
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
                [Check.LengthIsBetween(6, 29, Path = "BlackOp.Country")] public RelationSet<string> Participants { get; } = new();
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
                [Check.LengthIsBetween(4, 67)] public RelationSet<string> Symptoms { get; } = new();
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

            // Test Scenario: Applied to Date-ish Field (✓constrained✓)
            public class Hospital {
                [PrimaryKey] public string Address { get; set; } = "";
                public ulong NumBeds { get; set; }
                [Check.IsOneOf("2000-01-01", "2000-01-02", "2000-01-03")] public DateTime Opened { get; set; }
                [Check.IsOneOf("2100-01-01", "2100-01-02", "2100-01-03")] public DateOnly? Closed { get; set; }
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

            // Test Scenario: Pre-Defined Instance (✗impermissible✗)
            [PreDefined] public class WorldWonder {
                [PrimaryKey] public string Name { get; private init; }
                public string Country { get; private init; }
                public bool IsExtant { get; private init; }

                public static WorldWonder Mausoleum { get; } = new WorldWonder("Mausoleum of Halicarnassus", "Turkey", false);
                public static WorldWonder Pyramid { get; } = new WorldWonder("Great Pyramid of Giza", "Egypt", true);
                public static WorldWonder HangingGardens { get; } = new WorldWonder("Hanging Gardens of Babylon", "Iraq", false);
                public static WorldWonder Pharos { get; } = new WorldWonder("Lighthouse of Alexandria", "Egypt", false);
                public static WorldWonder Colossus { get; } = new WorldWonder("Colossus of Rhodes", "Greece", false);
                public static WorldWonder OlympianZeus { get; } = new WorldWonder("Statue of Zeus at Olympia", "Greece", false);
                [Check.IsOneOf(1, 2, 3)] public static WorldWonder Artemision { get; } = new WorldWonder("Temple of Artemis", "Greece", false);

                private WorldWonder(string name, string country, bool extant) {
                    Name = name;
                    Country = country;
                    IsExtant = extant;
                }
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
                [Check.IsOneOf("Americas", "Eurasia", "Middle East", "Africa", "Australia", "Pacific Islands", "Arctic", "Antarctica", "Oceans", Path = "Item")] public RelationSet<string> FossilLocations { get; } = new();
                public int FacialHorns { get; set; }
                public int NumTeeth { get; set; }
            }

            // Test Scenario: Applied to Nested Relation (✗impermissible✗)
            public class Cheerleader {
                [Flags] public enum Item { PomPoms = 1, Batons = 2, Dancing = 4, Sparklers = 8, Instruments = 16 }
                public struct Cheer {
                    public string Title { get; set; }
                    public string CallSign { get; set; }
                    public RelationMap<int, string> Moves { get; }
                    public string Music { get; set; }
                }

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
                [DataConverter<ToInt<string>>, Check.IsOneOf("Chicken", "Steak", "Carnitas", "Barbacoa", "Chorizo")] public string Protein { get; set; } = "";
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
                [DataConverter<ToString<int>>, Check.IsOneOf("Straight", "Curly", "Funnel")] public int Type { get; set; }
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
                public RelationList<string> Employees { get; } = new();
                public sbyte Floors { get; set; }
                public DateTime Opened { get; set; }
                public Guid License { get; set; }
                [Check.IsOneOf('A', '7', '_', '/', '=', '~', Path = "---")] public RelationList<Object> Inventory { get; } = new();
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
                [Check.IsOneOf(37.6, 1158.44, 0.919, 63.6666, Path = "DrinkingFountain.WaterPressure")] public RelationSet<DateTime> Inspections { get; } = new();
                public bool MotionSensored { get; set; }
            }

            // Test Scenario: <Path> on Relation Not Specified (✗missing path✗)
            public class Hairstyle {
                public enum Gender { Male, Female, Androgynous, Animal, None }

                [PrimaryKey] public Guid ID { get; set; }
                public string Name { get; set; } = "";
                public string Description { get; set; } = "";
                [Check.IsOneOf(true, false)] public RelationMap<Guid, bool> Certifications { get; } = new();
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

            // Test Scenario: Applied to Date-ish Field (✓constrained✓)
            public class GamingConsole {
                [PrimaryKey] public Guid SerialNumber { get; set; }
                public string Name { get; set; } = "";
                public string? AKA { get; set; }
                [Check.IsNotOneOf("1973-04-30", "1973-05-30")] public DateTime Launched { get; set; }
                [Check.IsNotOneOf("1984-06-22", "1988-10-17")] public DateOnly? Recalled { get; set; }
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

            // Test Scenario: Pre-Defined Instance (✗impermissible✗)
            [PreDefined] public class Pentomino {
                [PrimaryKey] public char Label { get; private init; }
                public sbyte ConcaveCorners { get; private init; }
                public sbyte ConvexCorners { get; private init; }

                public static Pentomino F { get; } = new Pentomino('F', 3, 7);
                public static Pentomino I { get; } = new Pentomino('I', 0, 4);
                public static Pentomino L { get; } = new Pentomino('L', 1, 5);
                public static Pentomino N { get; } = new Pentomino('N', 2, 6);
                public static Pentomino P { get; } = new Pentomino('P', 1, 5);
                public static Pentomino T { get; } = new Pentomino('T', 2, 6);
                public static Pentomino U { get; } = new Pentomino('U', 2, 6);
                [Check.IsNotOneOf(1, 2, 3)] public static Pentomino V { get; } = new Pentomino('V', 1, 5);
                public static Pentomino W { get; } = new Pentomino('W', 3, 7);
                public static Pentomino X { get; } = new Pentomino('X', 4, 8);
                public static Pentomino Y { get; } = new Pentomino('Y', 2, 6);
                public static Pentomino Z { get; } = new Pentomino('Z', 2, 6);
                
                private Pentomino(char label, sbyte concaves, sbyte convexes) {
                    Label = label;
                    ConcaveCorners = concaves;
                    ConvexCorners = convexes;
                }
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
                [Check.IsNotOneOf("2022-03-17", "1965-11-14", "1333-01-02", Path = "Value")] public RelationMap<uint, DateTime> Broadcasts { get; } = new();
                public string Hawker { get; set; } = "";
                public double Discount { get; set; }
                public double CommercialLength { get; set; }
                public ulong Hits { get; set; }
            }

            // Test Scenario: Applied to Nested Relation (✗impermissible✗)
            public class PersonOfTheYear {
                public enum Category { Politics, FilmTV, Sports, Music, News, Science, Philanthropy, Religion, Art, Other }
                public struct Magazine {
                    public DateTime Publication { get; set; }
                    public RelationList<uint> Editions { get; }
                }

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
                [DataConverter<ToInt<string>>, Check.IsNotOneOf("Cardboard", 14, "Vend-O-Matic", "Allyzom")] public string Brand { get; set; } = "";
                public byte NumStrings { get; set; }
            }

            // Test Scenario: Disallowed Value of Target Type on Data-Converted Property (✓valid✓)
            public class SoccerTeam {
                [PrimaryKey] public string League { get; set; } = "";
                [PrimaryKey] public string Location { get; set; } = "";
                [PrimaryKey] public string Nickname { get; set; } = "";
                public ushort RosterSize { get; set; }
                [DataConverter<ToInt<short>>, Check.IsNotOneOf(0, -3, 111)] public short WorldCupVictories { get; set; }
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
                [Check.IsNotOneOf(8471294811, 2056178955, 8005882340, Path = "---")] public RelationMap<string, ulong> PhoneNumbers { get; } = new();
                public double Weight { get; set; }
            }

            // Test Scenario: <Path> on Relation Refers to Non-Primary-Key Field of Anchor Entity (✗non-existent path✗)
            public class Bakugan {
                public enum Attribute { Pyrus, Aquos, Subterra, Haos, Darkus, Ventus, Aurelus }

                [PrimaryKey] public Guid ID { get; set; }
                public string BakuganName { get; set; } = "";
                public Attribute BakuganAttribute { get; set; }
                [Check.IsNotOneOf("Counterspell", "Basic Island", "Lightning Bolt", Path = "Bakugan.BakuganName")] public RelationSet<string> AbilityCards { get; } = new();
                public bool InAnime { get; set; }
            }

            // Test Scenario: <Path> on Relation Not Specified (✗missing path✗)
            public class QRCode {
                [PrimaryKey] public Guid ID { get; set; }
                public string URL { get; set; } = "";
                public RelationList<bool> Horizontal { get; } = new();
                [Check.IsNotOneOf(false)] public RelationList<bool> Vertical { get; } = new();
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
            [Check<CustomCheck>] public ushort Deaths { get; set; }
            public bool ActivatedByScythe { get; set; }
        }

        // Test Scenario: Constructor Arguments (✓constrained✓)
        public class Lyric {
            [PrimaryKey] public string SongTitle { get; set; } = "";
            [PrimaryKey] public int LineNumber { get; set; }
            public string Lyrics { get; set; } = "";
            [Check<CustomCheck>(13, false, "ABC", null)] public bool IsSpoken { get; set; }
            public bool IsChorus { get; set; }
        }

        // Test Scenario: Applied to Aggregate-Nested Scalar (✓constrained✓)
        public class Asteroid {
            public struct OrbitalDescription {
                public double Aphelion { get; set; }
                public double Perihelion { get; set; }
                [Check<CustomCheck>] public float Eccentricity { get; set; }
            }

            [PrimaryKey] public string Name { get; set; } = "";
            [Check<CustomCheck>(Path = "Aphelion")] public OrbitalDescription Orbit { get; set; }
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
            [Check<CustomCheck>(Path = "Vintage")] public Bottling SignatureWine { get; set; }
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
            [Check<CustomCheck>(Path = "Exchange.ExchangeID")] public Listing? Sydney { get; set; }
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
            [Check<CustomCheck>(Path = "Source")] public Curse Lycan { get; set; }
            public ushort Weight { get; set; }
            public ulong Kills { get; set; }
        }

        // Test Scenario: Pre-Defined Instance (✗impermissible✗)
        [PreDefined] public class Ubuntu {
            [PrimaryKey] public double Version { get; private init; }
            public string Codename { get; private init; }
            public DateTime ReleaseDate { get; private init;}

            public static Ubuntu U410 { get; } = new Ubuntu(4.10, "Warty Warthog", new DateTime(2004, 10, 20));
            public static Ubuntu U504 { get; } = new Ubuntu(5.04, "Hoary Hedgehog", new DateTime(2005, 4, 8));
            public static Ubuntu U510 { get; } = new Ubuntu(5.10, "Breezy Badger", new DateTime(2005, 10, 12));
            public static Ubuntu U606 { get; } = new Ubuntu(6.06, "Dapper Drake", new DateTime(2006, 6, 1));
            public static Ubuntu U610 { get; } = new Ubuntu(6.10, "Edgy Eft", new DateTime(2006, 10, 26));
            public static Ubuntu U704 { get; } = new Ubuntu(7.04, "Feisty Fawn", new DateTime(2007, 4, 19));
            public static Ubuntu U710 { get; } = new Ubuntu(7.10, "Gutsy Gibbon", new DateTime(2007, 10, 18));
            public static Ubuntu U804 { get; } = new Ubuntu(8.04, "Hardy Heron", new DateTime(2008, 4, 24));
            public static Ubuntu U810 { get; } = new Ubuntu(8.10, "Intrepid Ibex", new DateTime(2008, 10, 30));
            public static Ubuntu U904 { get; } = new Ubuntu(9.04, "Jaunty Jackalope", new DateTime(2009, 4, 23));
            public static Ubuntu U910 { get; } = new Ubuntu(9.10, "Karmic Koala", new DateTime(2009, 10, 29));
            [Check<CustomCheck>] public static Ubuntu U1004 { get; } = new Ubuntu(10.04, "Lucid Lynx", new DateTime(2010, 4, 29));
            public static Ubuntu U1010 { get; } = new Ubuntu(10.10, "Maverick Meerkat", new DateTime(2010, 10, 10));
            public static Ubuntu U1104 { get; } = new Ubuntu(11.04, "Natty Narwhal", new DateTime(2011, 4, 28));
            public static Ubuntu U1110 { get; } = new Ubuntu(11.10, "Oneiric Ocelot", new DateTime(2011, 10, 13));
            public static Ubuntu U1204 { get; } = new Ubuntu(12.04, "Precise Pangolin", new DateTime(2012, 4, 26));
            public static Ubuntu U1210 { get; } = new Ubuntu(12.10, "Quantal Quetzal", new DateTime(2012, 10, 18));
            public static Ubuntu U1304 { get; } = new Ubuntu(13.04, "Raring Ringtail", new DateTime(2013, 4, 13));
            public static Ubuntu U1310 { get; } = new Ubuntu(13.10, "Saucy Salamander", new DateTime(2013, 10, 17));
            public static Ubuntu U1404 { get; } = new Ubuntu(14.04, "Trusty Tahr", new DateTime(2014, 4, 17));
            public static Ubuntu U1410 { get; } = new Ubuntu(14.10, "Utopic Unicorn", new DateTime(2014, 10, 23));
            public static Ubuntu U1504 { get; } = new Ubuntu(15.04, "Vivid Vervet", new DateTime(2015, 4, 23));
            public static Ubuntu U1510 { get; } = new Ubuntu(15.10, "Wily Werewolf", new DateTime(2015, 10, 22));
            public static Ubuntu U1604 { get; } = new Ubuntu(16.04, "Xenial Xerus", new DateTime(2016, 4, 21));
            public static Ubuntu U1610 { get; } = new Ubuntu(16.10, "Yakkety Yak", new DateTime(2016, 10, 13));
            public static Ubuntu U1704 { get; } = new Ubuntu(17.04, "Zesty Zapus", new DateTime(2017, 4, 13));
            public static Ubuntu U1710 { get; } = new Ubuntu(17.10, "Artful Aarvark", new DateTime(2017, 10, 19));
            public static Ubuntu U1804 { get; } = new Ubuntu(18.04, "Bionic Beaver", new DateTime(2018, 4, 26));
            public static Ubuntu U1810 { get; } = new Ubuntu(18.10, "Cosmic Cuttlefish", new DateTime(2018, 10, 18));
            public static Ubuntu U1904 { get; } = new Ubuntu(19.04, "Disco Dingo", new DateTime(2019, 4, 18));
            public static Ubuntu U1910 { get; } = new Ubuntu(19.10, "Eoan Ermine", new DateTime(2019, 10, 17));
            public static Ubuntu U2004 { get; } = new Ubuntu(20.04, "Focal Fossa", new DateTime(2020, 4, 23));
            public static Ubuntu U2010 { get; } = new Ubuntu(20.10, "Groovy Gorilla", new DateTime(2020, 10, 22));
            public static Ubuntu U2104 { get; } = new Ubuntu(21.04, "Hirsute Hippo", new DateTime(2021, 4, 22));
            public static Ubuntu U2110 { get; } = new Ubuntu(21.10, "Impish Indri", new DateTime(2021, 10, 14));
            public static Ubuntu U2204 { get; } = new Ubuntu(22.04, "Jammy Jellyfish", new DateTime(2022, 4, 21));
            public static Ubuntu U2210 { get; } = new Ubuntu(22.10, "Kinetic Kudu", new DateTime(2022, 10, 20));
            public static Ubuntu U2304 { get; } = new Ubuntu(23.04, "Lunar Lobster", new DateTime(2023, 4, 20));
            public static Ubuntu U2310 { get; } = new Ubuntu(23.10, "Mantic Minotaur", new DateTime(2023, 10, 12));
            public static Ubuntu U2404 { get; } = new Ubuntu(24.04, "Noble Numbat", new DateTime(2024, 4, 25));
            public static Ubuntu U2410 { get; } = new Ubuntu(24.10, "Oracular Oriole", new DateTime(2024, 10, 10));
            public static Ubuntu U2504 { get; } = new Ubuntu(25.04, "Plucky Puffin", new DateTime(2025, 4, 17));

            private Ubuntu(double version, string codename, DateTime releaseDate) {
                Version = version;
                Codename = codename;
                ReleaseDate = releaseDate;
            }
        }

        // Test Scenario: Applied to Relation-Nested Scalar (✓constrained✓)
        public class CareBear {
            [PrimaryKey] public string Bear { get; set; } = "";
            public string Color { get; set; } = "";
            public char TummySymbol { get; set; }
            [Check<CustomCheck>(Path = "Item")] public RelationSet<string> MediaAppearances { get; } = new();
            public string LeadDesigner { get; set; } = "";
        }

        // Test Scenario: Applied to Nested Relation (✗impermissible✗)
        public class RiverWalk {
            public record struct TimeRange(ushort Open, ushort Close);
            public struct Schedule {
                public TimeRange M { get; set; }
                public TimeRange TU { get; set; }
                public TimeRange W { get; set; }
                public TimeRange TH { get; set; }
                public TimeRange F { get; set; }
                public TimeRange SA { get; set; }
                public TimeRange SU { get; set; }
                public RelationSet<string> HolidaysClosed { get; }
            }

            [PrimaryKey] public Guid ID { get; set; }
            public string City { get; set; } = "";
            public string River { get; set; } = "";
            public uint NumShops { get; set; }
            public uint NumRestaurants { get; set; }
            [Check<CustomCheck>(Path = "HolidaysClosed")] public Schedule Hours { get; set; }
            public decimal AnnualRevenue { get; set; }
            public ulong WalkLength { get; set; }
        }

        // Test Scenario: Scalar Property Constrained Multiple Times (✓both applied✓)
        public class TarotCard {
            [PrimaryKey] public int DeckID { get; set; }
            [PrimaryKey] public ushort CardNumber { get; set; }
            [Check<CustomCheck>, Check<CustomCheck>(-14, '%')] public byte Pips { get; set; }
            public string Character { get; set; } = "";
        }

        // Test Scenario: Applied to Data-Converted Field (✓constrained✓)
        public class DataStructure {
            [PrimaryKey] public string Name { get; set; } = "";
            public string SearchBigO { get; set; } = "";
            public string InsertBigO { get; set; } = "";
            [DataConverter<ToInt<string>>, Check<CustomCheck>] public string RemoveBigO { get; set; } = "";
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
            [Name("HeightOf", Path = "Height"), Check<CustomCheck>(Path = "Height")] public Door LeftDoor { get; set; }
            public Door RightDoor { get; set; }
        }

        // Test Scenario: Constraint Generator Cannot Be Default-Constructed (✗illegal✗)
        public class Seizure {
            [Flags] public enum Category { Epilleptic, GrandMal, Focal, Absence, Myoclonic, Partial }

            [PrimaryKey] public Guid SeizureID { get; set; }
            public double Duration { get; set; }
            [Check<PrivateCheck>] public string SufferedBy { get; set; } = "";
            public Category Kind { get; set; }
            public bool Fatal { get; set; }
        }

        // Test Scenario: Constraint Generator Cannot Be Complex-Constructed (✗illegal✗)
        public class Transistor {
            [PrimaryKey] public Guid ID { get; set; }
            public string Model { get; set; } = "";
            [Check<PrivateCheck>("Dopant", 4)] public string? Dopant { get; set; }
            public float Transconductance { get; set; }
            public int OperatingTemperature { get; set; }
        }

        // Test Scenario: Constraint Generator Throws Error upon Default Construction (✗propagated✗)
        public class Buffet {
            public enum Ethnicity { American, Chinese, Italian, Indian, German, Greek, Japanese, Korean, Mexican, Thai }

            [PrimaryKey] public Guid BuffetID { get; set; }
            public string Restaurant { get; set; } = "";
            [Check<UnconstructibleCheck>] public Ethnicity Cuisine { get; set; }
            public bool AllYouCanEat { get; set; }
            public decimal CostPerPerson { get; set; }
        }

        // Test Scenario: Constraint Generator Throws Error upon Complex Construction (✗propagated✗)
        public class BasketballPlayer {
            [PrimaryKey] public string Name { get; set; } = "";
            public ulong Points { get; set; }
            public float Pct3Pointer { get; set; }
            [Check<UnconstructibleCheck>(false, 17UL)] public ulong Rebounds { get; set; }
            public ulong Steals { get; set; }
            public ulong Assists { get; set; }
        }

        // Test Scenario: Constraint Generator Throws Error when Creating Constraint (✗propagated✗)
        public class Aquarium {
            [PrimaryKey] public string AquariumName { get; set; } = "";
            public ulong NumSpecies { get; set; }
            public ulong TotalGallonsWater { get; set; }
            [Check<UnusableCheck>] public bool HasDolphins { get; set; }
            public decimal AdmissionPrice { get; set; }
            public bool AZA { get; set; }
        }

        // Test Scenario: <Path> is `null` (✗illegal✗)
        public class Trilogy {
            [PrimaryKey] public string Title { get; set; } = "";
            public string Entry1 { get; set; } = "";
            [Check<CustomCheck>(Path = null!)] public string Entry2 { get; set; } = "";
            public string Entry3 { get; set; } = "";
        }

        // Test Scenario: <Path> on Scalar Does Not Exist (✗non-existent path✗)
        public class TarPits {
            [PrimaryKey] public string TarPitsName { get; set; } = "";
            public float Area { get; set; }
            [Check<CustomCheck>(Path = "---")] public string FirstFossil { get; set; } = "";
            public bool IsNationalArea { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
        }

        // Test Scenario: <Path> on Aggregate Does Not Exist (✗non-existent path✗)
        public class StarCrossedLovers {
            [Flags] public enum Feud { Familial = 1, Religious = 2, Class = 4, SportsFandom = 8, Culinary = 16, Corporate = 32, Political = 64 }
            public record struct Name(string FirstName, char MiddleInitial, string LastName);

            public Name Lover1 { get; set; }
            [Check<CustomCheck>(Path = "---")] public Name Lover2 { get; set; }
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
            [Check<CustomCheck>] public Necrotization Legs { get; set; }
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
            [Check<CustomCheck>(Path = "---")] public Company Manufacturer { get; set; } = new();
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
            [Check<CustomCheck>(Path = "Incipience")] public Possession Target { get; set; } = new();
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
            [Check<CustomCheck>] public Coordinate Location { get; set; } = new();
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
            [Check<CustomCheck>(Path = "---")] public RelationList<City> Cities { get; } = new();
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
            [Check<CustomCheck>(Path = "Skydiver.Height")] public RelationMap<DateTime, long> Dives { get; } = new();
            public Vehicle HasJumpedFrom { get; set; }
        }

        // Test Scenario: <Path> on Relation Not Specified (✗missing path✗)
        public class Spring {
            [PrimaryKey] public Guid ID { get; set; }
            [Check<CustomCheck>] public RelationSet<string> ConstituentMetals { get; } = new();
            public double SpringConstant { get; set; }
            public ushort NumCoils { get; set; }
        }
    }

    internal static class ComplexCheckConstraints {
        // Test Scenario: No Constructor Arguments (✓constrained✓)
        [Check.Complex<CustomCheck>(new[] { "FirstLine" })]
        public class CanterburyTale {
            [PrimaryKey] public int Index { get; set; }
            public string Whose { get; set; } = "";
            public string FirstLine { get; set; } = "";
            public ulong WordCount { get; set; }
        }

        // Test Scenario: Constructor Arguments (✓constrained✓)
        [Check.Complex<CustomCheck>(new[] { "ConclaveRounds" }, -93, true, 'X')]
        public class Pope {
            [PrimaryKey] public string PapalName { get; set; } = "";
            [PrimaryKey] public uint PapalNumber { get; set; }
            public DateTime Elected { get; set; }
            public DateTime? Ceased { get; set; }
            public uint ConclaveRounds { get; set; }
            public string FirstEncyclical { get; set; } = "";
        }

        // Test Scenario: Covers Zero Fields (✗illegal✗)
        [Check.Complex<CustomCheck>(new string[] {})]
        public class Terminator {
            [PrimaryKey] public string Model { get; set; } = "";
            [PrimaryKey] public ushort Number { get; set; }
            public ulong KillCount { get; set; }
            public string Portrayer { get; set; } = "";
            public DateTime FirstAppearance { get; set; }
        }

        // Test Scenario: Covers Multiple Distinct Fields (✓constrained✓)
        [Check.Complex<CustomCheck>(new[] { "Major", "Minor", "Patch" })]
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
        [Check.Complex<CustomCheck>(new[] { "Name", "Name", "Name", "Name" })]
        public class Muppet {
            [PrimaryKey] public string Name { get; set; } = "";
            public DateTime Debut { get; set; }
            public string Puppeteer { get; set; } = "";
            public ushort MuppetsShowAppearances { get; set; }
            public ushort MuppetsFilmAppearances { get; set; }
        }

        // Test Scenario: Covers Name-Swapped Fields (✓constrained✓)
        [Check.Complex<CustomCheck>(new[] { "Cuisine", "ContainsTomatoes" })]
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
        [Check.Complex<CustomCheck>(new string[] { "Width" })]
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
        [Check.Complex<CustomCheck>(new string[] { "Belligerents" })]
        public class PeaceTreaty {
            [PrimaryKey] public string TreatyName { get; set; } = "";
            public DateTime Signed { get; set; }
            public DateTime Effective { get; set; }
            public string Text { get; set; } = "";
            public ushort NumSignatories { get; set; }
        }

        // Test Scenario: Covers Data-Converted Field (✓constrained✓)
        [Check.Complex<CustomCheck>(new[] { "When", "Casualties", "When" })]
        public class Massacre {
            [PrimaryKey] public string Name { get; set; } = "";
            public ulong Casualties { get; set; }
            public bool WarCrime { get; set; }
            [DataConverter<Nullify<DateTime>>] public DateTime When { get; set; }
        }

        // Test Scenario: Applied to Single Entity Type Multiple Times (✓constrained✓)
        [Check.Complex<CustomCheck>(new[] { "LengthMinutes" })]
        [Check.Complex<CustomCheck>(new[] { "SungThrough" })]
        [Check.Complex<CustomCheck>(new[] { "SungThrough" })]
        public class Musical {
            [PrimaryKey] public string Title { get; set; } = "";
            public bool SungThrough { get; set; }
            public ushort SongCount { get; set; }
            public ulong LengthMinutes { get; set; }
            public ushort TonyAwards { get; set; }
        }

        // Test Scenario: Constraint Generator Cannot Be Constructed (✗illegal✗)
        [Check.Complex<PrivateCheck>(new[] { "Omega3s", "Omega6s" }, 'O', 'I', 'L', '!')]
        public class CookingOil {
            [PrimaryKey] public string Type { get; set; } = "";
            public decimal SmokePoint { get; set; }
            public double TransFats { get; set; }
            public double Omega3s { get; set; }
            public double Omega6s { get; set; }
        }

        // Test Scenario: Constraint Generator Throws Error upon Construction (✗propagated✗)
        [Check.Complex<UnconstructibleCheck>(new[] { "Born", "Died" }, "Lifespan", 2918.01f, true)]
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
        [Check.Complex<UnusableCheck>(new[] { "Namespace", "ClassName" })]
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
            public RelationMap<string, bool> Flags { get; } = new();
            public RelationSet<SoftwarePackage> BuildDependencies { get; } = new();
            public RelationSet<SoftwarePackage> RunDependencies { get; } = new();
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
            public RelationList<Charge> Charges { get; } = new();
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
            public RelationList<Breakpoint> Breakpoints { get; } = new();
            public string Debugger { get; set; } = "";
            public double Memory { get; set; }
        }

        // Test Scenario: Entity X → Entity Y via Reference → Entity X via Relation (✓allowed✓)
        public class Filibuster {
            public class Politician {
                [PrimaryKey] public string FullName { get; set; } = "";
                public DateTime FirstElected { get; set; }
                public RelationSet<Filibuster> FilibustersBroken { get; } = new();
            }

            [PrimaryKey] public Guid FilibusterID { get; set; }
            public string Legislation { get; set; } = "";
            public DateTime Date { get; set; }
            public double Duration { get; set; }
            public Politician Instigator { get; set; } = new();
            public bool Successful { get; set; }
        }
    }

    internal static class DataExtraction {
        // Scenario: Non-Null, Public, Instance Scalars and Enumerations (✓values extracted✓)
        public class Morgue {
            [Flags] public enum Service { Autopsy = 1, Identification = 2, Cremation = 4, Refrigeration = 8, Sequestration = 16 }

            [PrimaryKey, Column(0)] public string Name { get; set; } = "";
            [Column(1)] public string ChiefMedicalExaminer { get; set; } = "";
            [Column(2)] public uint Capacity { get; set; }
            [Column(3)] public decimal Budget { get; set; }
            [Column(4)] public char FederalGrade { get; set; }
            [Column(5)] public Service AvailableServices { get; set; }
            [Column(6)] public bool GovernmentRun { get; set; }
        }

        // Scenario: Non-Null, Public, Static Scalars and Enumerations (✓values extracted✓)
        public class PythonInterpreter {
            public enum Language { C, CPP, Python, Rust, Java }

            [PrimaryKey, Column(0)] public Guid ProgramID { get; set; }
            [Column(1)] public string Path { get; set; } = "";
            [Column(2)] public DateTime InstalledOn { get; set; }
            [IncludeInModel, Column(3)] public static double MinVersion { get; set; }
            [IncludeInModel, Column(4)] public static double MaxVersion { get; set; }
            [IncludeInModel, Column(5)] public static Language BackEndLanguage { get; set; }
        }

        // Scenario: Non-Null, Non-Public, Instance Scalars and Enumerations (✓values extracted✓)
        public class PirateShip {
            public enum ShipKind { Sloop, Schooner, Frigate, Brigantine, Galleon, Barque, Carrack, Other }

            [PrimaryKey, Column(0)] public Guid ID { get; set; }
            [Column(1)] public string ShipName { get; set; } = "";
            [Column(2)] public string Captain { get; set; } = "";
            [IncludeInModel, Column(3)] private double Length { get; set; }
            [IncludeInModel, Column(4)] protected int NumCannons { get; set; }
            [IncludeInModel, Column(5)] internal ShipKind Style { get; set; }
            [IncludeInModel, Column(6)] protected internal bool CarriedSlaves { get; set; }

            public void SetLength(double length) => Length = length;
            public double GetLength() => Length;
            public void SetNumCannons(int numCannons) => NumCannons = numCannons;
            public int GetNumCannons() => NumCannons;
        }

        // Scenario: Non-Null, Non-Public, Static Scalars and Enumerations (✓values extracted✓)
        public class Enzyme {
            [PrimaryKey, Column(0)] public string EnzymeCommissionNumber { get; set; } = "";
            [Column(1)] public string CommonName { get; set; } = "";
            [IncludeInModel, Column(2)] private static bool IsEnzyme { get; set; }
            [IncludeInModel, Column(3)] protected static double NumEnzymesTotal { get; set; }
            [IncludeInModel, Column(4)] internal static string Regulator { get; set; } = "";
            [IncludeInModel, Column(5)] protected internal static DateTime FirstDiscovered { get; set; }

            public static void SetIsEnzyme(bool isEnzyme) => IsEnzyme = isEnzyme;
            public static bool GetIsEnzme() => IsEnzyme;
            public static void SetNumEnzymesTotal(double numEnzymesTotal) => NumEnzymesTotal = numEnzymesTotal;
            public static double GetNumEnzymesTotal() => NumEnzymesTotal;
        }

        // Scenario: Null Scalars and Enumerations (✓null extracted✓)
        public class Ode {
            public enum Kind { Victory, Eulogy, Musical, Romantic, Other }

            [PrimaryKey, Column(0)] public string Title { get; set; } = "";
            [PrimaryKey, Column(1)] public string Author { get; set; } = "";
            [Column(2)] public ushort Lines { get; set; }
            [Column(3)] public ushort WordCount { get; set; }
            [Column(4)] public DateTime? Publication { get; set; }
            [Column(5)] public string? Collection { get; set; }
            [Column(6)] public Kind? Style { get; set; }
        }

        // Scenario: Explicit Interface Implementation Property (✓values extracted✓)
        public interface IWorldLeader {
            string Name { get; set; }
            string Polity { get; set; }
        }
        public class Tlatoani : IWorldLeader {
            [PrimaryKey, Column(0)] public Guid ID { get; set; }
            [IncludeInModel, Column(1)] string IWorldLeader.Name { get; set; } = "";
            [IncludeInModel, Column(2)] string IWorldLeader.Polity { get; set; } = "Aztec Empire";
            [Column(3)] public DateTime Death { get; set; }
            [Column(4)] public bool EncounteredConquistadors { get; set; }
            [Column(5)] public ushort CoronationYear { get; set; }
        }

        // Scenario: Virtual Override Property (✓most-derived values extracted✓)
        public abstract class Coin {
            public virtual double Denomination { get; set; } = 2.0;
        }
        public sealed class StateQuarter : Coin {
            [PrimaryKey, Column(0)] public string State { get; set; } = "";
            [IncludeInModel, Column(1)] public sealed override double Denomination { get; set; }
            [Column(2)] public ushort Year { get; set; }
            [Column(3)] public string Engraver { get; set; } = "";
            [Column(4)] public ulong Mintage { get; set; }
        }

        // Scenario: Hiding Property (✓hiding values extracted✓)
        public abstract class Light {
            public double Intensity { get; set; }
            public byte Red { get; set; }
            public byte Green { get; set; }
            public byte Blue { get; set; }
            public byte? Alpha { get; set; }
        }
        public class Aurora : Light {
            [PrimaryKey, Column(0)] public Guid AuroraID { get; set; }
            [Column(1)] public string Name { get; set; } = "";
            [Column(2)] public string? AKA { get; set; }
            [Column(3)] public new float Intensity { get; set; }
        }

        // Scenario: Pre-Defined Entity (✓values extracted✓)
        [PreDefined] public class RomanNumeral {
            [PrimaryKey, Column(0)] public char Numeral { get; private init; }
            [Column(1)] public ushort Value { get; private init; }

            public static RomanNumeral I { get; } = new RomanNumeral('I', 1);
            public static RomanNumeral V { get; } = new RomanNumeral('V', 5);
            public static RomanNumeral X { get; } = new RomanNumeral('X', 10);
            public static RomanNumeral L { get; } = new RomanNumeral('L', 50);
            public static RomanNumeral C { get; } = new RomanNumeral('C', 100);
            public static RomanNumeral D { get; } = new RomanNumeral('D', 500);
            public static RomanNumeral M { get; } = new RomanNumeral('M', 1000);

            private RomanNumeral(char numeral, ushort value) {
                Numeral = numeral;
                Value = value;
            }
        }

        // Scenario: [DataConverter] Applied to Scalar Property (✓converted values extracted✓)
        public class Underworld {
            [PrimaryKey, Column(0)] public string Name { get; set; } = "";
            [Column(1)] public string Civilization { get; set; } = "";
            [Column(2)] public string Lord { get; set; } = "";
            [Column(3), DataConverter<Invert>] public bool ForMortals { get; set; }
            [Column(4), DataConverter<MakeDate<int>>] public int GoogleResults { get; set; }
        }

        // Scenario: [Numeric] Applied to Enumeration Property (✓converted values extracted✓)
        public class CornMaze {
            public enum Corn : byte { Field, Sweet, Flint, BlackAztec, BloodyButcher, Blue, PaintedMountain, Other }
            [Flags] public enum Shape : ulong { Animal = 1, Geometry = 2, Character = 4, Vehicle = 8, Person = 16, Object = 32, Foodstuff = 64, Weapon = 128, Geography = 256, Other = 2048 }

            [PrimaryKey, Column(0)] public Guid MazeID { get; set; }
            [Numeric, Column(1)] public Corn CornType { get; set; }
            [Numeric, Column(2)] public Shape MazeShape { get; set; }
            [Column(3)] public ulong StalkCount { get; set; }
            [Column(4)] public ulong MazeArea { get; set; }
            [Column(5)] public double SuccessRate { get; set; }
            [Column(6)] public double RecordTime { get; set; }
        }

        // Scenario: [AsString] Applied to Enumeration Property (✓converted values extracted✓)
        public class MarioKartRacetrack {
            public enum Cup { Mushroom, Flower, Star, Special, Shell, Banana, Leaf, Lightning, Battle, RetroBattle }

            [PrimaryKey, Column(0)] public string Name { get; set; } = "";
            [Column(1)] public string FirstAppearance { get; set; } = "";
            [AsString, Column(2)] public Cup Series { get; set; }
            [Column(3)] public ulong? TrackLength { get; set; }
            [Column(4)] public bool AvailableOnline { get; set; }
        }

        // Scenario: [Calculated] Property (✓values extracted✓)
        public class Lighthouse {
            [PrimaryKey, Column(0)] public string Name { get; set; } = "";
            [Column(1)] public string Location { get; set; } = "";
            [Column(2)] public double Height { get; set; }
            [Column(3)] public ushort FocalLength { get; set; }
            [Calculated, Column(4)] public ulong LighthouseRating => (ulong)(Math.Sqrt(Height) + 137.54) * FocalLength;
        }

        // Scenario: Non-Null Aggregate Property with Single Scalar/Enumeration Nested Field (✓values extracted✓)
        public class Nucleobase {
            public struct Letter {
                [Column(0)] public char Value { get; set; }
            }

            [PrimaryKey(Path = "Value"), Column(0)] public Letter Symbol { get; set; }
            [Column(1)] public string Name { get; set; } = "";
            [Column(2)] public string ChemicalFormula { get; set; } = "";
        }

        // Scenario: Non-Null Aggregate Property with Multiple Scalar/Enumeration Nested Fields (✓values extracted✓)
        public class LegoSet {
            public enum Rating { One, OnePointFive, Two, TwoPointFive, Three, ThreePointFive, Four, FourPointFive, Five }
            public enum Series { Architecture, Batman, Brikz, City, Classic, Creator, DC, Disney, HarryPotter, Ideas, Jurassic, IndianaJones, SuperMario, LOTR, Marvel, Minecraft, Ninjago, StarWars, Technic }

            public struct Listing {
                [Column(0)] public decimal Price { get; set; }
                [Column(1)] public Rating Stars { get; set; }
                [Column(2)] public string URL { get; set; }
                [Column(3)] public ulong InsiderPoints { get; set; }
                [Column(4)] public Series Theme { get; set; }
            }

            [PrimaryKey, Column(0)] public uint ItemNumber { get; set; }
            [Column(1)] public string Title { get; set; } = "";
            [Column(2)] public Listing Catalog { get; set; }
            [Column(7)] public ulong Pieces { get; set; }
            [Column(8)] public byte LowerBoundAge { get; set; }
        }

        // Scenario: Non-Null Aggregate Property with All Null Nested Fields (✓null values extracted✓)
        public class SnowballFight {
            public struct Structure {
                [Column(0)] public byte? NumTeams { get; set; }
                [Column(1)] public byte? HitsAllowed { get; set; }
                [Column(2)] public double? MaxBallRadius { get; set; }
            }

            [PrimaryKey, Column(0)] public Guid FightID { get; set; }
            [Column(1)] public DateTime KickOff { get; set; }
            [Column(2)] public Structure FightStructure { get; set; }
            [Column(5)] public ulong Length { get; set; }
            [Column(6)] public double LowTemperature { get; set; }
        }

        // Scenario: Null Aggregate Property with One Nested Field (✓null values extracted✓)
        public class Knot {
            public struct Geometry {
                [Column(0)] public Double ConwayNotation { get; set; }
            }

            [PrimaryKey, Column(0)] public string Name { get; set; } = "";
            [Column(1)] public Geometry? Shape { get; set; }
            [Column(2)] public double Efficiency { get; set; }
            [Column(3)] public ushort? AshleyBookOfKnotsPage { get; set; }
        }

        // Scenario: Null Aggregate Property with Multiple Nested Fields (✓null values extracted✓)
        public class Armory {
            public enum Level { International, Federal, State, Local, Private, Vigilante }

            public struct Coordinate {
                [Column(0)] public float Latitude { get; set; }
                [Column(1)] public float Longitude { get; set; }
            }

            [PrimaryKey, Column(0)] public string Name { get; set; } = "";
            [Column(1)] public bool Decommissioned { get; set; }
            [Column(2)] public Coordinate? Location { get; set; }
            [Column(4)] public ulong WeaponsCount { get; set; }
            [Column(5)] public Level Owner { get; set; }
        }

        // Scenario: Nested Aggregate Property (✓values extracted✓)
        public class MillionaireQuestion {
            public struct Option {
                [Column(0)] public string Text { get; set; }
                [Column(1)] public bool FiftyFiftyEliminated { get; set; }
                [Column(2)] public double AudiencePercentage { get; set; }
                [Column(3)] public bool IsCorrect { get; set; }
            }
            public struct Options {
                [Column(0)] public Option A { get; set; }
                [Column(4)] public Option B { get; set; }
                [Column(8)] public Option C { get; set; }
                [Column(12)] public Option D { get; set; }
            }

            [PrimaryKey, Column(0)] public Guid QuestionID { get; set; }
            [Column(1)] public string Category { get; set; } = "";
            [Column(2)] public string Question { get; set; } = "";
            [Column(3)] public Options Answers { get; set; }
        }

        // Scenario: Data Conversion Applied to Aggregate-Nested Fields (✓converted values extracted✓)
        public class GroceryGame {
            public struct Episode {
                [Column(0), DataConverter<ToInt<byte>>] public byte Season { get; set; }
                [Column(1), DataConverter<ToInt<byte>>] public byte Number { get; set; }
                [Column(2)] public string Judge1 { get; set; }
                [Column(3)] public string Judge2 { get; set; }
                [Column(4)] public string Judge3 { get; set; }
            }

            [PrimaryKey, Column(0)] public string Name { get; set; } = "";
            [Column(1)] public string Description { get; set; } = "";
            [Column(2)] public Episode FirstAppearance { get; set; }
            [Column(7)] public ulong NumTimesPlayed { get; set; }
        }

        // Scenario: Non-Null Reference Property with Single-Field Primary Key (✓values extracted✓)
        public class PapalConclave {
            public class Cardinal {
                [PrimaryKey, Column(0)] public string Name { get; set; } = "";
                [Column(1)] public string Country { get; set; } = "";
                [Column(2)] public byte Age { get; set; }
            }

            [PrimaryKey, Column(0)] public DateTime Date { get; set; }
            [Column(1)] public byte Ballots { get; set; }
            [Column(2)] public Cardinal ElectedPope { get; set; } = new();
            [Column(3)] public ushort NumElectors { get; set; }
            [Column(4)] public Cardinal Dean { get; set; } = new();
        }

        // Scenario: Non-Null Reference Property with Multi-Field Primary Key (✓values extracted✓)
        public class Cytonic {
            [Flags] public enum Power { Hyperjump = 1, Cytosense = 2, Mindblade = 4, Bolts = 8, Inhibition = 16, Illusions = 32 }
            [Flags] public enum Book { Skyward = 1, Starsight = 2, Cytonic = 4, Defiant = 8 }

            public class Species {
                [PrimaryKey, Column(0)] public int Grouping { get; set; }
                [Column(1)] public string Name { get; set; } = "";
                [PrimaryKey, Column(2)] public int SubNumber { get; set; }
                [Column(3)] public bool PrimaryIntelligence { get; set; }
            }

            [PrimaryKey, Column(0)] public string Name { get; set; } = "";
            [Column(1)] public string? CallSign { get; set; }
            [Column(2)] public Species SelfSpecies { get; set; } = new();
            [Column(4)] public Power Abilities { get; set; }
            [Column(5)] public Book Appearances { get; set; }
        }

        // Scenario: Null Reference Property with Single-Field Primary Key (✓null values extracted✓)
        public class SoapOpera {
            public class Network {
                [PrimaryKey, Column(0)] public string Name { get; set; } = "";
                [Column(1)] public string StockSymbol { get; set; } = "";
                [Column(2)] public ulong NumEmployees { get; set; }
                [Column(3)] public DateTime Founded { get; set; }
            }

            [PrimaryKey, Column(0)] public string Title { get; set; } = "";
            [Column(1)] public bool IsStillAiring { get; set; }
            [Column(2)] public DateTime Premiere { get; set; }
            [Column(3)] public ushort NumSeasons { get; set; }
            [Column(4)] public ushort NumEpisodes { get; set; }
            [Column(5)] public ushort NumCastMembers { get; set; }
            [Column(6)] public Network? OwningNetwork { get; set; }
            [Column(7)] public bool IsTelenovela { get; set; }
        }

        // Scenario: Null Reference Property with Multi-Field Primary Key (✓null values extracted✓)
        public class Library {
            public class Person {
                [PrimaryKey, Column(0)] public string FirstName { get; set; } = "";
                [PrimaryKey, Column(1)] public string LastName { get; set; } = "";
                [Column(2)] public DateTime DOB { get; set; }
            }

            [PrimaryKey, Column(0)] public Guid LibraryID { get; set; }
            [Column(1)] public ulong NumBooks { get; set; }
            [Column(2)] public Person? HeadLibrarian { get; set; }
            [Column(4)] public decimal Endowment { get; set; }
            [Column(5)] public ushort Branches { get; set; }
        }

        // Scenario: Data Conversion Applied to Reference-Nested Field (✓converted values extracted✓)
        public class CurlingMatch {
            public class OlympicOrganization {
                [PrimaryKey, DataConverter<AllCaps>, Column(0)] public string Code { get; set; } = "";
                [Column(1)] public string Country { get; set; } = "";
                [Column(2)] public short Recognized { get; set; }
            }

            [PrimaryKey, Column(0)] public Guid ID { get; set; }
            [Column(1)] public OlympicOrganization TeamA { get; set; } = new();
            [Column(2)] public OlympicOrganization TeamB { get; set; } = new();
            [Column(3)] public sbyte ScoreA { get; set; }
            [Column(4)] public sbyte ScoreB { get; set; }
            [Column(5)] public DateTime Date { get; set; }
            [Column(6)] public ushort? Olympiad { get; set; }
            [Column(7)] public bool HammerForA { get; set; }
        }

        // Scenario: Non-Null Relation Property with Zero Elements (✓no values extracted✓)
        public class Pretzel {
            [PrimaryKey, Column(0)] public Guid PretzelID { get; set; }
            [Column(1)] public string Name { get; set; } = "";
            public RelationSet<string> Toppings { get; init;  } = new();
            [Column(2)] public decimal RetailPrice { get; set; }
            [Column(3)] public string DoughSource { get; set; } = "";
        }

        // Scenario: Non-Null List/Set Relation Property with Only New Elements (✓values extracted per element✓)
        public class Teppanyaki {
            [PrimaryKey, Column(0)] public Guid GrillID { get; set; }
            [Column(1)] public double GrillSurfaceArea { get; set; }
            public RelationSet<string> AuthorizedChefs { get; init; } = new();
            public RelationList<string> SupportedFoods { get; init; } = new();
            [Column(2)] public float MaxTemperature { get; set; }
            [Column(3)] public string? Restaurant { get; set; }
            [Column(4)] public bool IsHibachi { get; set; }
        }

        // Scenario: Non-Null Map Relation Property with Only New Elements (✓values extracted per element✓)
        public class SpellingBee {
            [PrimaryKey, Column(0)] public ushort Year { get; set; }
            [Column(1)] public byte NumRounds { get; set; }
            [Column(2)] public string Champion { get; set; } = "";
            public RelationMap<uint, string> EliminationWords { get; init; } = new();
        }

        // Scenario: Non-Null Ordered List Relation Property with Only New Elements (✓values extracted per element✓)
        public class ImprovTroupe {
            [PrimaryKey, Column(0)] public string Name { get; set; } = "";
            [Column(1)] public DateTime Created { get; set; }
            [Column(2)] public uint NumShows { get; set; }
            public RelationOrderedList<string> Lineup { get; init; } = new();
            [Column(3)] public string Location { get; set; } = "";
            [Column(4)] public string URL { get; set; } = "";
        }

        // Scenario: Non-Null Relation Property with Only Saved Elements (✓no values extracted✓)
        public class NedsDeclassifiedTip {
            [PrimaryKey] public Guid ID { get; set; }
            public string Category { get; set; } = "";
            public string Tip { get; set; } = "";
            public RelationSet<string> For { get; init; } = new();
        }

        // Scenario: Non-Null Relation Property with At Least One Modified Element (✓values extracted per element✓)
        public class PendragonTerritory {
            [PrimaryKey] public string Name { get; set; } = "";
            public RelationOrderedList<string> Travellers { get; init; } = new();
            public string FirstAppearance { get; set; } = "";
            public string Capital { get; set; } = "";
        }

        // Scenario: Non-Null Relation Property with At Least One Deleted Element (✓values extracted per element✓)
        public class IowaCaucus {
            public enum PoliticalParty { Republican, Democratic }

            [PrimaryKey] public ushort Year { get; set; }
            [PrimaryKey] public PoliticalParty Party { get; set; }
            public DateTime Date { get; set; }
            public RelationMap<string, double> DelegatesEarned { get; init; } = new();
            public bool Bellwether { get; set; }
        }

        // Scenario: Null Relation Property (✓no values extracted✓)
        public class Existentialist {
            public class Thesis {
                [PrimaryKey, Column(0)] public string Title { get; set; } = "";
                [Column(1)] public ushort YearSubmitted { get; set; }
                [Column(2)] public ushort PageCount { get; set; }
            }

            [PrimaryKey, Column(0)] public string Name { get; set; } = "";
            public RelationOrderedList<Thesis>? DoctoralTheses { get; init; }
            [Column(1)] public DateTime DateOfBirth { get; set; }
            [Column(2)] public DateTime? DateOfDeath { get; set; }
            [Column(3)] public string ExistentialSchool { get; set; } = "";
        }

        // Scenario: Non-Null Relation Property becomes Null (✓no values extracted✓)
        public class Orogene {
            public enum Book { FifthSeason, ObeliskGate, StoneSky }

            [PrimaryKey, Column(0)] public string FulcrumName { get; set; } = "";
            [Column(1)] public string? BirthName { get; set; }
            [Column(2)] public string? BirthComm { get; set; }
            [Column(3)] public byte Rings { get; set; }
            public RelationMap<Book, bool>? Appearances { get; init; }
            [Column(4)] public bool AtNodeStation { get; set; }
        }

        // Scenario: Non-Null Relation Property with Nested Aggregate and At Least One Element (✓values extracted✓)
        public class OlympianBoon {
            public enum Deity { Ares, Athena, Aphrodite, Artemis, Demeter, Dionysus, Hermes, Poseidon, Zeus }
            public enum Ability { Attack, Special, Cast, Dash, Upper, Call }

            public struct Benefit {
                [Column(0)] public string ParameterName { get; set; }
                [Column(1)] public double ParameterValue { get; set; }
            }

            [PrimaryKey, Column(0)] public string BoonName { get; set; } = "";
            [Column(1)] public Deity Benefactor { get; set; }
            [Column(2)] public Ability AbilityAffected { get; set; }
            public RelationOrderedList<Benefit> Progressions { get; init; } = new();
            [Column(3)] public double Likelihood { get; set; }
        }

        // Scenario: Non-Null Relation Property with Nested Reference and At Least One Element (✓values extracted✓)
        public class Impeachment {
            public struct Charge {
                public enum Category { HighCrime, Misdemeanor, Treason }

                [Column(0)] public string Claim { get; set; }
                [Column(1)] public Category Severity { get; set; }
            }
            public class Count {
                [PrimaryKey, Column(0)] public Guid ID { get; set; }
                [Column(1)] public Charge Claim { get; set; }
                [PrimaryKey, Column(3)] public bool Guilty { get; set; }
            }

            [PrimaryKey, Column(0)] public string Official { get; set; } = "";
            [Column(1)] public string Position { get; set; } = "";
            [PrimaryKey, Column(2)] public DateTime Commenced { get; set; }
            public RelationList<Count> Counts { get; init; } = new();
            [Calculated, Column(3)] public bool Convicted => Counts.Any(c => c.Guilty);
        }

        // Scenario: Non-Null Relation Property with Owning Entity in Element (✓values extracted✓)
        public class MaoriGod {
            public enum Relation { Self, Parent, Grandparent, Child, Grandchild, Sibling, Cousing, Nibling, AuntUncle };

            [PrimaryKey, Column(0)] public string Name { get; set; } = "";
            [Column(1)] public string Domain { get; set; } = "";
            public RelationMap<MaoriGod, Relation> Family { get; init; } = new();
            [Column(2)] public bool IsAtua { get; set; }
            [Column(3)] public bool EncounteredMaui { get; set; }
        }

        // Scenario: Aggregate-Nested Relation Property (✓values extracted✓)
        public class DataCenter {
            public struct Computers {
                [Column(0)] public ushort NumCabinets { get; set; }
                [Column(1)] public ushort RacksPerCabinet { get; set; }
                public RelationSet<string> Brands { get; init; }
            }
            public struct Stats {
                [Column(0)] public double CO2 { get; set; }
                [Column(1)] public ushort Electricity { get; set; }
                public RelationMap<string, double> Dimensions { get; init; }
                [Column(2)] public Computers Machines { get; set; }
            }

            [PrimaryKey, Column(0)] public Guid ID { get; set; }
            [Column(1)] public bool LiquidCooled { get; set; }
            [Column(2)] public Stats Statistics { get; set; }
            [Column(6)] public string Operator { get; set; } = "";
        }

        // Scenario: [Calculated] Relation (✓values extracted✓)
        public class Chameleon {
            public struct Eye {
                [Column(0)] public ushort ConeDensity { get; set; }
                [Column(1)] public ushort RodDensity { get; set; }
                [Column(2)] public double VisionRange { get; set; }
            }

            [PrimaryKey, Column(0)] public Guid ReptileID { get; set; }
            [Column(1)] public ulong TimesChangedColor { get; set; }
            [Column(2)] public string Genus { get; set; } = "";
            [Column(3)] public string Species { get; set; } = "";
            [Calculated] public RelationOrderedList<Eye> Eyes { get; init; }

            public Chameleon() {
                Eyes = new RelationOrderedList<Eye>();
                Eyes.Add(new Eye() { ConeDensity = 8571, RodDensity = 4409, VisionRange = 360 });
                Eyes.Add(new Eye() { ConeDensity = 12099, RodDensity = 6776, VisionRange = 360 });
            }
        }

        // Scenario: Data Conversion Applied to Relation-Nested Field (✓converted values extracted✓)
        public class Horoscope {
            public enum Zodiac { Aries, Taurus, Gemini, Cancer, Leo, Virgo, Libra, Scorpio, Saggitarius, Capricorn, Aquarius, Pisces }

            public struct Listing {
                [Column(0)] public string Prediction { get; set; }
                [Column(1), DataConverter<ToInt<char>>] public char Sex { get; set; }
                [Column(2), DataConverter<ToInt<char>>] public char Hustle { get; set; }
                [Column(3), DataConverter<ToInt<char>>] public char Vibe { get; set; }
                [Column(4), DataConverter<ToInt<char>>] public char Success { get; set; }
            }

            [PrimaryKey, Column(0)] public Zodiac Sign { get; set; }
            public RelationMap<DateTime, Listing> Readings { get; init; } = new();
            [Column(1)] public DateTime RangeLower { get; set; }
            [Column(2)] public DateTime RangeUpper { get; set; }
        }
    }

    internal static class Reconstitution {
        // Scenario: Non-Null, Public, Instance Writeable Scalar and Enumeration Properties (✓values reconstituted✓)
        public class Garden {
            public enum Type { Bontaical, Zen, Community, Chinese, Japanese, Hanging, Bonsai, Flower, Other }

            [PrimaryKey, Column(0)] public Guid GardenID { get; set; }
            [Column(1)] public string Name { get; set; } = "";
            [Column(2)] public Type Kind { get; set; }
            [Column(3)] public double Acreage { get; set; }
            [Column(4)] public int NumFlowers { get; set; }
            [Column(5)] public bool OpenToThePublic { get; set; }
        }

        // Scenario: Non-Null, Public, Static Writeable Scalar an Enumeration Properties (✓values reconstituted✓)
        public class Dormitory {
            [Flags] public enum Feature { Coed = 1, PetFriendly = 2, DrugFree = 4, Smoking = 8, Honors = 16, Dining = 32 }

            [PrimaryKey, Column(0)] public string School { get; set; } = "";
            [PrimaryKey, Column(1)] public string Name { get; set; } = "";
            [Column(2)] public Feature Features { get; set; }
            [IncludeInModel, Column(3)] public static bool IsBuilding { get; set; }
            [Column(4)] public ushort Capacity { get; set; }
            [IncludeInModel, Column(5)] public static sbyte GradeBits { get; set; }
        }

        // Scenario: Non-Null, Non-Public, Instance Writeable Scalar and Enumeration Properties (✓values reconstituted✓)
        public class KingOfFrance {
            [PrimaryKey, Column(0)] public string RegnalName { get; private set; } = "";
            [PrimaryKey, Column(1)] public uint RegnalNumber { get; private set; }
            [IncludeInModel, Column(2)] protected string Religion { get; set; } = "";
            [IncludeInModel, Column(3)] internal DateTime Coronation { get; set; }
            [Column(4)] public string RoyalHouse { get; protected internal set; } = "";
            [Column(5)] public bool PreRevolution { get; set; }

            public string GetReligion() => Religion;
        }

        // Scenario: Non-Null, Non-Public, Static Writeable Scalar and Enumeration Properties (✓values reconstituted✓)
        public class MathematicalProof {
            public enum Method { Direct, Exhaustion, Contradiction, Contraposition, Induction, Construction }

            [PrimaryKey, Column(0)] public Guid ProofID { get; set; }
            [Column(1)] public string MathematicianName { get; set; } = "";
            [IncludeInModel, Column(2)] protected static ushort MaxAllowedSteps { get; set; }
            [IncludeInModel, Column(3)] public static Method MostPopularMethod { get; internal set; }
            [IncludeInModel, Column(4)] protected internal static bool ComputersAllowed { get; set; }
            [IncludeInModel, Column(5)] private static decimal PrizeMoneyEarned { get; set; }

            public static ushort GetMaxAllowedSteps() => MaxAllowedSteps;
            public static decimal GetPrizeMoneyEarned() => PrizeMoneyEarned;
        }

        // Scenario: Null Writeable Scalar and Enumeration Properties (✓values reconstituted✓)
        public class Banshee {
            public enum Nationality { Irish, Scottish, Manx, Cornish, Welsh }

            [PrimaryKey, Column(0)] public Guid MonsterID { get; set; }
            [Column(1)] public string? Name { get; set; }
            [Column(2)] public double WailDecibels { get; set; }
            [Column(3)] public uint? Victims { get; set; }
            [Column(4)] public bool Female { get; set; }
            [Column(5)] public Nationality? Origin { get; set; }
        }

        // Scenario: Init-Only Scalar and Enumeration Properties (✓values reconstituted✓)
        public class Neighborhood {
            [PrimaryKey, Column(0)] public Guid ID { get; init; }
            [Column(1)] public string Name { get; init; } = "";
            [Column(2)] public string City { get; init; } = "";
            [Column(3)] public ushort NumHouses { get; init; }
            [Column(4)] public ulong Population { get; init; }
            [Column(5)] public decimal? AverageRent { get; init; }
            [Column(6)] public double AverageSqFt { get; init; }
        }

        // Scenario: [Calculated] Scalar Properties (✓values skipped✓)
        public class Patreon {
            [PrimaryKey, Column(0)] public string URL { get; set; } = "";
            [Column(1)] public string Creator { get; set; } = "";
            [Column(2)] public decimal Tier0 { get; set; }
            [Column(3)] public decimal Tier1 { get; set; }
            [Column(4)] public decimal Tier2 { get; set; }
            [Calculated, Column(5)] public decimal AverageTier => (Tier0 + Tier1 + Tier2) / 3;
        }

        // Scenario: [Calculated] Aggregate Properties (✓values skipped✓)
        public class EmergencyRoom {
            public struct Staffing {
                [Column(0)] public int NumDoctors { get; set; }
                [Column(1)] public int NumNurses { get; set; }
                [Column(2)] public int NumOther { get; set; }
            }

            [PrimaryKey, Column(0)] public Guid BuildingID { get; set; }
            [Column(1)] public ushort Capacity { get; set; }
            [Calculated, Column(2)] public Staffing Staff => new Staffing() { NumDoctors = Capacity / 15, NumNurses = Capacity / 9, NumOther = Capacity / 23 };
            [Column(5)] public sbyte TraumaLevel { get; set; }
            [Column(6)] public bool IsUrgentCare { get; set; }
        }

        // Scenario: [Calculated] Reference Properties (✓values skipped✓)
        public class Accountant {
            public class AccountingFirm {
                [PrimaryKey, Column(0)] public string Name { get; set; } = "Klippity Accounting Co.";
                [PrimaryKey, Column(1)] public string State { get; set; } = "Montana";
                [Column(2)] public ulong Employees { get; set; } = 1870;
                [Column(3)] public bool IndividualAccounting { get; set; } = true;
                [Column(4)] public decimal Revenue { get; set; } = 50000000;
            }

            [PrimaryKey, Column(0)] public string SSN { get; set; } = "";
            [Column(1)] public string Name { get; set; } = "";
            [Calculated, Column(2)] public AccountingFirm CurrentFirm { get; } = new();
            [Column(4)] public int NumAccounts { get; set; }
        }

        // Scenario: [Calculated] Relation Properties (✓values skipped✓)
        public class Prenup {
            [Column(0)] public Guid ContractID { get; set; }
            [Column(1)] public string Spouse1 { get; set; } = "";
            [Column(2)] public string Spouse2 { get; set; } = "";
            [Calculated] public RelationSet<DateTime> VestingSchedule {
                get {
                    return new RelationSet<DateTime>() {
                        new DateTime(2035, 1, 1),
                        new DateTime(2036, 1, 1),
                        new DateTime(2037, 1, 1),
                        new DateTime(2038, 1, 1),
                        new DateTime(2039, 1, 1),
                    };
                }
            }
            [Column(3)] public decimal TotalNetWorth { get; set; }
            [Column(4)] public string Notary { get; set; } = "";
            [Column(5)] public string StateEnforced { get; set; } = "";
        }

        // Scenario: Writeable Explicit Interface Implementation Property (✓values reconstituted✓)
        public interface IBird {
            bool CanFly { get; set; }
            double WingspanCm { get; set; }
            double TopSpeedKph { get; set; }
        }
        public class Penguin : IBird {
            [PrimaryKey, Column(0)] public string Genus { get; set; } = "";
            [PrimaryKey, Column(1)] public string Species { get; set; } = "";
            [Column(2)] public string CommonName { get; set; } = "";
            [IncludeInModel, Column(3)] bool IBird.CanFly { get; set; }
            [Column(4)] public double AverageWeightKg { get; set; }
            [IncludeInModel, Column(5)] double IBird.WingspanCm { get; set; }
            [IncludeInModel, Column(6)] double IBird.TopSpeedKph { get; set; }
        }

        // Scenario: Writeable Virtual Property (✓values reconstituted into most-derived✓)
        public abstract class BouncingObject {
            public abstract double MaxHeight { get; set; }
            public virtual float WeightLimit { get; set; }
        }
        public class Trampoline : BouncingObject {
            [PrimaryKey, Column(0)] public Guid ID { get; set; }
            [IncludeInModel, Column(1)] public override double MaxHeight { get; set; }
            [Column(2)] public int NumScrews { get; set; }
            [IncludeInModel, Column(3)] public override float WeightLimit { get; set; }
        }

        // Scenario: Writeable Hiding Property (✓values reconstituted into hider✓)
        public class Blanket {
            public Guid ID { get; set; }
            public double Length { get; set; }
            public double Width { get; set; }
        }
        public class Quilt : Blanket {
            [PrimaryKey, Column(0)] public new Guid ID { get; set; }
            [Column(1)] public uint NumSquares { get; set; }
            [Column(2)] public DateTime CreationDate { get; set; }
            [Column(3)] public new double Length { get; set; }
            [Column(4)] public new double Width { get; set; }
        }

        // Scenario: [DataConverter] Applied to Scalar Property (✓converted values reconstituted✓)
        public class MetraRoute {
            [PrimaryKey, DataConverter<AllCaps>, Column(0)] public string Line { get; set; } = "";
            [Column(1)] public string CityEndpoint { get; set; } = "";
            [Column(2)] public string SuburbEndpoint { get; set; } = "";
            [DataConverter<ToInt<ushort>>, Column(3)] public ushort NumStations { get; set; }
            [Column(4)] public uint Ridership { get; set; }
            [Column(5)] public double TrackLength { get; set; }
        }

        // Scenario: [Numeric] Applied to Enumeration Property (✓converted values reconstituted✓)
        public class Loa {
            public enum Month : byte { JAN, FEB, MAR, APR, MAY, JUNE, JULY, AUG, SEP, OCT, NOV, DEC }
            [Flags] public enum Tradition { Haitian = 1, Louisiana = 2, FolkCatholicism = 4, Santeria = 8, Candomble = 16 }

            [PrimaryKey, Column(0)] public string Name { get; set; } = "";
            [Numeric, Column(1)] public Month FeastMonth { get; set; }
            [Numeric, Column(2)] public Tradition Traditions { get; set; }
            [Column(3)] public string Domain { get; set; } = "";
            [Column(4)] public bool InvolvedWithZombies { get; set; }
        }

        // Scenario: [AsString] Applied to Enumeration Property (✓converted values reconstituted✓)
        public class LED {
            public enum Color { White, Blue, Green, Red }

            [PrimaryKey, Column(0)] public Guid ProductID { get; set; }
            [AsString, Column(1)] public Color EmittedColor { get; set; }
            [Column(2)] public double PowerConsumption { get; set; }
        }

        // Scenario: Non-Null Writeable Aggregate Property with Single Scalar/Enumeration Nested Fields (✓values reconstituted✓)
        public class ConspiracyTheory {
            public enum Industry { Government, Science, Entertainment, Athletics, Finances, Religion, Aliens }

            public record struct Person(string Name);

            [PrimaryKey, Column(0)] public Guid ID { get; set; }
            [Column(1)] public Person Theorist { get; set; }
            [Column(2)] public Industry About { get; set; }
            [Column(3)] public uint Believers { get; set; }
            [Column(4)] public bool FormallyDebunked { get; set; }
            [Column(5)] public string? WikipediaURL { get; set; }
        }

        // Scenario: Non-Null Writeable Aggregate Property with Multiple Scalar/Enumeration Nested Fields (✓values reconstituted✓)
        public class Mermaid {
            public struct Color {
                [Column(0)] public byte R { get; set; }
                [Column(1)] public byte G { get; set; }
                [Column(2)] public byte B { get; set; }
            }

            [PrimaryKey, Column(0)] public Guid MermaidID { get; set; }
            [Column(1)] public ushort HeightCm { get; set; }
            [Column(2)] public bool IsSiren { get; set; }
            [Column(3)] public Color HairColor { get; set; }
            [Column(6)] public Color BraColor { get; set; }
            [Column(9)] public Color TailColor { get; set; }
        }

        // Scenario: Non-Null Writeable Aggregate Property with All Null Nested Fields (✓null values reconstituted✓)
        public class Cheese {
            public enum Style { Slices, Shredded, Cubes, Wedges, Wheel, Sauce }

            public struct Nutrition {
                [Column(0)] public ushort? Calories { get; set; }
                [Column(1)] public ushort? GramsFat { get; set; }
                [Column(2)] public ushort? MgSodium { get; set; }
                [Column(3)] public ushort? MgCholesterol { get; set; }
                [Column(4)] public ushort? GramsCarbs { get; set; }
            }

            [PrimaryKey, Column(0)] public string Name { get; set; } = "";
            [Column(1)] public string CountryOfOrigin { get; set; } = "";
            [Column(2)] public Nutrition NutritionalValue { get; set; }
            [Column(7)] public Style? BestServedAs { get; set; }
        }

        // Scenario: Null Writeable Aggregate Property with One Nested Field (✓null values reconstituted✓)
        public class EpiPen {
            public record struct Dosage(double Dose);

            [PrimaryKey, Column(0)] public Guid MedicalID { get; set; }
            [Column(1)] public string PrescribingDoctor { get; set; } = "";
            [Column(2)] public Dosage? Dose { get; set; }
            [Column(3)] public string Manufacturer { get; set; } = "";
        }

        // Scenario: Null Writeable Aggregate Property with Multiple Nested Fields (✓null values reconstituted✓)
        public class Hacker {
            public enum Hat { White, Black, Unknown }

            public struct Terminal {
                [Column(0)] public string Name { get; set; }
                [Column(1)] public sbyte FontSize { get; set; }
                [Column(2)] public bool DarkMode { get; set; }
                [Column(3)] public uint HistoryLimit { get; set; }
            }

            [PrimaryKey, Column(0)] public string Name { get; set; } = "";
            [Column(1)] public Hat Role { get; set; }
            [Column(2)] public Terminal? PreferredTerminal { get; set; }
            [Column(6)] public decimal RansomExtorted { get; set; }
            [Column(7)] public bool InAnonymous { get; set; }
            [Column(8)] public bool StateSponsored { get; set; }
            [Column(9)] public long DevicesCompromised { get; set; }
        }

        // Scenario: Nested Writeable Aggregate Property (✓values reconstituted✓)
        public class Iconoclast {
            public struct Timeline {
                [Column(0)] public DateTime Start { get; set; }
                [Column(1)] public DateTime? End { get; set; }
            }
            public struct Statistics {
                [Column(0)] public uint IconsDestroyed { get; set; }
                [Column(1)] public int PaintingsBurned { get; set; }
                [Column(2)] public decimal PreciousMetalsCollected { get; set; }
            }
            public struct Career {
                [Column(0)] public Timeline Activity { get; set; }
                [Column(2)] public Statistics Stats { get; set; }
            }

            [PrimaryKey, Column(0)] public Guid IconoclastID { get; set; }
            [Column(1)] public bool IsByzantine { get; set; }
            [Column(2)] public Career History { get; set; }
            [Column(7)] public string Name { get; set; } = "";
        }

        // Scenario: Data Conversion Applied to Aggregate-Nested Fields (✓converted values reconstituted✓)
        public class AlphaMonster {
            public enum Hunter { Sam, Dean, Bobby, Castiel, Charlie, Rowena, Ketch, Donna, Jody, Other }

            public struct Episode {
                [Column(0), DataConverter<ToInt<short>>] public short SeasonNumber { get; set; }
                [Column(1), DataConverter<ToInt<float>>] public float EpisodeNumber { get; set; }
            }

            [PrimaryKey, Column(0)] public string Monster { get; set; } = "";
            [Column(1)] public string? Actor { get; set; }
            [Column(2)] public Hunter? Killer { get; set; }
            [Column(3)] public Episode FirstAppearance { get; set; }
            [Column(5)] public uint NumAppearances { get; set; }
        }

        // Scenario: Calculated and Non-Calculated Fields of Same Aggregate Type (✓handled appropriately✓)
        public class Empanada {
            public struct Filling {
                [Column(0)] public string Contents { get; set; }
                [Calculated, Column(1)] public double Percentage => 100.0;
            }

            [PrimaryKey, Column(0)] public Guid EmpanadaID { get; set; }
            [Column(1)] public decimal Price { get; set; }
            [Column(2)] public ushort Calories { get; set; }
            [Column(3)] public Filling MainFilling { get; set; }
            [Calculated, Column(5)] public Filling? SecondaryFilling { get; } = null;
            [Column(7)] public bool DeepFried { get; set; }
        }

        // Scenario: Non-Null Writeable Reference Property with Single-Field Primary Key (✓values reconstituted✓)
        public class Cockfight {
            public enum Result { Win1, Win2, Draw, Suspended }

            public class Rooster {
                [PrimaryKey, Column(0)] public Guid AnimalID { get; set; }
                [Column(1)] public string? Name { get; set; }
                [Column(2)] public double Height { get; set; }
                [Column(3)] public double Weight { get; set; }
            }

            [PrimaryKey, Column(0)] public Guid FightID { get; set; }
            [Column(1)] public Rooster CompetitorA { get; set; } = new();
            [Column(2)] public Rooster CompetitorB { get; set; } = new();
            [Column(3)] public Result Outcome { get; set; }
            [Column(4)] public decimal Pot { get; set; }
            [Column(5)] public double FightDuration { get; set; }
        }

        // Scenario: Non-Null Writeable Reference Property with Multi-Field Primary Key (✓values reconstituted✓)
        public class Glacier {
            public enum Continent { NorthAmerica, SouthAmerica, Europe, Asia, Oceania, Africa, Antarctica, Arctic }

            public class MountainRange {
                [PrimaryKey, Column(0)] public string Name { get; set; } = "";
                [PrimaryKey, Column(1)] public Continent HomeContinent { get; set; }
                [PrimaryKey, Column(2)] public ushort Discriminator { get; set; }
                [Column(3)] public ulong Length { get; set; }
                [Column(4)] public ulong MaxElevation { get; set; }
            }

            [PrimaryKey, Column(0)] public string Name { get; set; } = "";
            [Column(1)] public double Length { get; set; }
            [Column(2)] public MountainRange Range { get; set; } = new();
            [Column(5)] public bool HasMushroomRock { get; set; }
        }

        // Scenario: Null Writeable Reference Property with Single-Field Primary Key (✓null values reconstituted✓)
        public class Allegory {
            public enum Kind { Literature, Music, Art, Philosophy }

            public class Work {
                [PrimaryKey, Column(0)] public Guid ID { get; set; }
                [Column(1)] public string Title { get; set; } = "";
                [Column(2)] public string? Creator { get; set; }
                [Column(3)] public Kind TypeOfWork { get; set; }
            }

            [PrimaryKey, Column(0)] public string Name { get; set; } = "";
            [Column(1)] public Work? Source { get; set; }
            [Column(2)] public string ComparisonSource { get; set; } = "";
            [Column(3)] public string ComparisonTarget { get; set; } = "";
            [Column(4)] public bool WellUnderstood { get; set; }
        }

        // Scenario: Null Writeable Reference Property with Multi-Field Primary Key (✓null values reconstituted✓)
        public class StripClub {
            public class Person {
                [PrimaryKey, Column(0)] public string FirstName { get; set; } = "";
                [PrimaryKey, Column(1)] public string LastName { get; set; } = "";
                [Column(2)] public DateTime DateOfBirth { get; set; }
                [Column(3)] public string CountryOfOrigin { get; set; } = "";
            }

            [PrimaryKey, Column(0)] public string Name { get; set; } = "";
            [Column(1)] public DateTime Opened { get; set; }
            [Column(2)] public DateTime? Closed { get; set; }
            [Column(3)] public Person Proprietor { get; set; } = new();
            [Column(5)] public decimal AnnualRevenue { get; set; }
            [Column(6)] public Person? PrimaryStripper { get; set; }
            [Column(8)] public ushort NumEmployees { get; set; }
            [Column(9)] public bool HasPrivateRoom { get; set; }
        }

        // Scenario: Data Conversion Applied to Reference-Nested Field (✓converted values reconstituted✓)
        public class Fresco {
            public enum Surface { Wall, Door, Ceiling, Floor, Canvas, Other }

            public class Painter {
                [PrimaryKey, Column(0), DataConverter<AllCaps>] public string Name { get; set; } = "";
                [Column(1)] public DateTime DateOfBirth { get; set; }
                [Column(2)] public string Country { get; set; } = "";
                [Column(3)] public bool DisplayedInTheLouvre { get; set; }
            }

            [PrimaryKey, Column(0)] public Guid PaintingID { get; set; }
            [Column(1)] public string Title { get; set; } = "";
            [Column(2)] public Painter Artist { get; set; } = new();
            [Column(3)] public float Length { get; set; }
            [Column(4)] public float Width { get; set; }
            [Column(5)] public Surface PaintedOn { get; set; }
        }

        // Scenario: Calculated and Non-Calculated Fields of Same Reference Type (✓handled appropriately✓)
        public class GoKart {
            public class Company {
                [PrimaryKey, Column(0)] public string Name { get; set; } = "";
                [Column(1)] public ulong Employees { get; set; }
                [Column(2)] public string? TickerSymbol { get; set; }
                [Column(3)] public DateTime Incorporated { get; set; }
            }

            [PrimaryKey, Column(0)] public Guid GoKartID { get; set; }
            [Column(1)] public ushort TopSpeed { get; set; }
            [Calculated, Column(2)] public Company Manufacturer => Operator;
            [Column(3)] public Company Operator { get; set; } = new();
            [Column(4)] public bool DriverOnLeft { get; set; }
            [Column(5)] public byte NumWheels { get; set; }
        }

        // Scenario: Zero Relation Elements (✓no values reconstituted✓)
        public class CharcuterieBoard {
            [PrimaryKey, Column(0)] public Guid ID { get; set; }
            [Column(1)] public string BoardMaterial { get; set; } = "";
            [Column(2)] public double BoardArea { get; set; }
            public RelationList<string> Cheeses { get; } = new();
            public RelationSet<string> Meats { get; } = new();
            public RelationOrderedList<string> Sauces { get; } = new();
            public RelationMap<DateTime, uint> Usages { get; } = new();
        }

        // Scenario: List/Set Relation Elements (✓values reconstituted per element✓)
        public class Mutant {
            [PrimaryKey, Column(0)] public string CodeName { get; set; } = "";
            [Column(1)] public string BirthName { get; set; } = "";
            public RelationList<string> Powers { get; } = new();
            public RelationSet<DateTime> Appearances { get; } = new();
            [Column(2)] public bool InXMenMovies { get; set; }
        }

        // Scenario: Map Relation Elements (✓values reconstituted per element✓)
        public class WaterPark {
            public enum Customer { General, Infant, Child, Adult, Senior, Student, Veteran }

            [PrimaryKey, Column(0)] public Guid WaterParkID { get; set; }
            [Column(1)] public string Name { get; set; } = "";
            public RelationMap<string, double> WaterSlides { get; } = new();
            [Column(2)] public bool LazyRiver { get; set; }
            [Column(3)] public ulong GallonsWater { get; set; }
            public RelationMap<Customer, decimal> AdmissionPrices { get; } = new();
        }

        // Scenario: Ordered List Relation Elements (✓values reconstituted per element✓)
        public class Seance {
            [PrimaryKey, Column(0)] public DateTime Timestamp { get; set; }
            [PrimaryKey, Column(1)] public string Medium { get; set; } = "";
            public RelationOrderedList<string> SpiritsContacted { get; } = new();
            [Column(2)] public decimal PriceTag { get; set; }
            [Column(3)] public ushort NumCandles { get; set; }
        }

        // Scenario: Read-Only Relations (✓values reconstituted per element✓)
        public class Dermatologist {
            public enum Day { Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday }
            public enum Degree { GED, HighSchoolDiploma, Bachelors, Masters, Doctorate }

            [PrimaryKey, Column(0)] public string Name { get; set; } = "";
            [Column(1)] public DateTime DateOfBirth { get; set; }
            [Column(2)] public string OfficeAddress { get; set; } = "";
            [Column(3)] public bool OwnsPractice { get; set; }
            public IReadOnlyRelationMap<Degree, string> AlmaMaters { get; } = new RelationMap<Degree, string>();
            public IReadOnlyRelationSet<string> Patients { get; } = new RelationSet<string>();
            public IReadOnlyRelationOrderedList<decimal> SalaryHistory { get; } = new RelationOrderedList<decimal>();
            public IReadOnlyRelationList<Day> Workdays { get; } = new RelationList<Day>();
        }

        // Scenario: Null Relation (✓no reconstitution [should be impossible unless empty]✓)
        public class FriedChicken {
            public enum ChickenPart { Leg, Wing, Breast, Thigh, Tender, Giblet, Foot, Liver, Gizzard }

            [PrimaryKey, Column(0)] public string Restaurant { get; set; } = "";
            [PrimaryKey, Column(1)] public ChickenPart Part { get; set; }
            [PrimaryKey, Column(2)] public byte SpiceLevel { get; set; }
            [Column(3)] public decimal Price { get; set; }
            [Column(4)] public string Breading { get; set; } = "";
            [Column(5)] public bool ButtermilkMarinated { get; set; }
            public RelationSet<string> Spices { get; } = null!;
            public RelationList<string> RecommendedSauces { get; } = null!;
        }

        // Scenario: Relation Nested in Non-Null Aggregate (✓values reconstituted per element✓)
        public class Limousine {
            [Flags] public enum Feature { Tinted = 1, HandRolled = 2, Bulletproof = 4 }

            public struct Personnel {
                [Column(0)] public string Owner { get; set; } = "";
                public RelationSet<string> LicensedDrivers { get; init;  }
                [Column(1)] public string Mechanic { get; set; } = "";
                [Column(2)] public string? DecalMaker { get; set; }

                public Personnel() {
                    LicensedDrivers = new RelationSet<string>();
                }
            }

            [PrimaryKey, Column(0)] public string LicensePlate { get; set; } = "";
            [Column(1)] public float Length { get; set; }
            [Column(2)] public byte Capacity { get; set; }
            [Column(3)] public Personnel People { get; set; }
            [Column(6)] public Feature WindowFeatures { get; set; }
        }

        // Scenario: Relation Nested in Null Aggregate (✓no reconstitution [should be impossible unless empty]✓)
        public class Xenomorph {
            public struct Victims {
                [Column(0)] public uint ConfirmedKills { get; set; }
                public RelationMap<string, bool> Impregnations { get; }
                [Column(1)] public bool Suicide { get; set; }
            }

            [PrimaryKey, Column(0)] public Guid AlienID { get; set; }
            [Column(1)] public double Height { get; set; }
            [Column(2)] public bool StillAlive { get; set; }
            [Column(3)] public Victims? AlienMurders { get; set; }
            [Column(5)] public string FirstFilmAppearance { get; set; } = "";
        }

        // Scenario: Relation-Nested Aggregate Elements (✓values reconstituted per element✓)
        public class EpicPoem {
            public struct Section {
                [Column(0)] public string Name { get; set; }
                [Column(1)] public string? Theme { get; set; }
            }

            [PrimaryKey, Column(0)] public string Title { get; set; } = "";
            [Column(1)] public string Author { get; set; } = "";
            [Column(2)] public DateTime Published { get; set; }
            [Column(3)] public uint StanzaCount { get; set; }
            public RelationOrderedList<Section> Sections { get; } = new();
            [Column(4)] public string Language { get; set; } = "";
        }

        // Scenario: Relation-Nested Reference Elements (✓values reconstituted per element✓)
        public class TrafficStop {
            public class PoliceOfficer {
                [PrimaryKey, Column(0)] public string PoliceForce { get; set; } = "";
                [PrimaryKey, Column(1)] public ulong BadgeNumber { get; set; }
                [Column(2)] public string Name { get; set; } = "";
                [Column(3)] public bool HasK9 { get; set; }
            }

            [PrimaryKey, Column(0)] public DateTime Timestamp { get; set; }
            [Column(1)] public string Driver { get; set; } = "";
            public RelationList<PoliceOfficer> Officers { get; } = new();
            [Column(2)] public bool TicketIssued { get; set; }
            [Column(3)] public bool ArrestMade { get; set; }
        }

        // Scenario: Owning Entity in Relation Elements (✓values reconstituted per element✓)
        public class Poacher {
            [Flags] public enum Continent { NorthAmerica = 1, SouthAmerica = 2, Asia = 4, Africa = 8, Europe = 16, Oceania = 32, Antarctica = 64, Arctic = 128 }
            [Flags] public enum AnimalType { Mammal = 1, Bird = 2, Reptile = 4, Insect = 8, Arachnid = 16, Fish = 32, Amphibian = 64, Mollusk = 128 }

            [PrimaryKey, Column(0)] public string Name { get; set; } = "";
            [Column(1)] public Continent ActiveLocations { get; set; }
            [Column(2)] public AnimalType Targets { get; set; }
            public RelationSet<Poacher> PoachingGroup { get; } = new();
            [Column(3)] public decimal Income { get; set; }
        }

        // Scenario: Data Conversion Applied to Relation-Nested Field (✓converted values reconstituted per element✓)
        public class MichelinGuide {
            public struct Rating {
                [Column(0), DataConverter<Stars>] public string Stars { get; set; }
                [Column(1)] public char Grade { get; set; }
            }

            [PrimaryKey, Column(0)] public ushort Year { get; set; }
            [Column(1)] public uint PageCount { get; set; }
            public RelationMap<string, Rating> Restaurants { get; } = new();
            [Column(2)] public bool IsGreenGuide { get; set; }
        }

        // Scenario: Single Viable Public Default Constructor (✓reconstituted✓)
        public class Stove {
            public enum Fuel { Electricity, Gas, Charcoal, Wood }

            [PrimaryKey, Column(0)] public Guid DeviceID { get; set; }
            [Column(1)] public Fuel PoweredBy { get; set; }
            [Column(2)] public byte NumBurners { get; set; }
            [Column(3)] public bool IsInduction { get; set; }
            [Column(4)] public string Brand { get; set; } = "";
            [Column(5)] public string Model { get; set; } = "";

            public Stove() { constructorCalled_ = true; }
            public bool ConstructorCalled() { return constructorCalled_; }
            private readonly bool constructorCalled_ = false;
        }

        // Scenario: Single Viable Public Partial Constructor (✓reconstituted✓)
        public class Cannibal {
            [PrimaryKey, Column(0)] public string FullName { get; }
            [Column(1)] public ushort NumVictims { get; }
            [Column(2)] public bool Imprisoned { get; set; }
            [Column(3)] public string Hometown {
                get { return hometown_; }
                set { throw new InvalidOperationException(); }
            }
            [Column(4)] public bool Filial { get; set; }

            public Cannibal(string fullName, string hometown, ushort numVictims) {
                FullName = fullName;
                hometown_ = hometown;
                NumVictims = numVictims;
            }
            private readonly string hometown_;
        }

        // Scenario: Single Viable Public Full Constructor (✓reconstituted✓)
        public class Menorah {
            [PrimaryKey, Column(0)] public Guid ProductID { get; }
            [Column(1)] public sbyte CandleHolders { get; }
            [Column(2)] public float Weight { get; }
            [Column(3)] public bool IsChannukiah { get; }
            [Column(4)] public decimal PriceTag { get; }
            [Column(5)] public bool DishwasherSafe { get; }

            public Menorah(Guid productID, bool isChannukiah, decimal priceTag, float weight, sbyte candleHolders, bool dishwasherSafe) {
                ProductID = productID;
                IsChannukiah = isChannukiah;
                PriceTag = priceTag;
                Weight = weight;
                CandleHolders = candleHolders;
                DishwasherSafe = dishwasherSafe;
            }
        }

        // Scenario: Single Viable Non-Public Constructor (✓reconstituted✓)
        public class Wedding {
            [PrimaryKey, Column(0)] public string Partner1 { get; }
            [PrimaryKey, Column(1)] public string Partner2 { get; }
            [Column(2)] public DateTime Date { get; }
            [Column(3)] public string Venue { get; }
            [Column(4)] public ushort Attendance { get; }
            [Column(5)] public bool Outdoor { get; }

            private Wedding(string partner1, string partner2, DateTime date, string venue, ushort attendance, bool outdoor) {
                Partner1 = partner1;
                Partner2 = partner2;
                Date = date;
                Venue = venue;
                Attendance = attendance;
                Outdoor = outdoor;
            }
        }

        // Scenario: Single Viable Primary Record Constructor (✓reconstituted✓)
        public record class GoFundMe(
            string URL,
            string? Beneficiary,
            decimal? AmountRaised,
            decimal? Goal,
            DateTime? DateOpened,
            DateTime? DateClosed
        );

        // Scenario: No Viable Constructor and All Properties are Writeable (✗cannot reconstitute✗)
        public class Tractor {
            [PrimaryKey, Column(0)] public Guid VehicleID { get; set; }
            [Column(1)] public string LicensePlate { get; set; } = "";
            [Column(2)] public byte NumWheels { get; set; }
            [Column(3)] public bool Agricultural { get; set; }
            [Column(4)] public ulong Horsepower { get; set; }

            public Tractor(Guid vehicleID, string licensePlate, byte numWheels, int extraArgument) {
                throw new InvalidOperationException("NON-VIABLE CONSTRUCTOR");
            }
        }

        // Scenario: No Constructor and At Least One Read-Only Property (✗cannot reconstitute✗)
        public class Deposition {
            [PrimaryKey, Column(0)] public string Deponent { get; set; } = "";
            [PrimaryKey, Column(1)] public string Questioner { get; set; } = "";
            [Column(2)] public DateTime Date { get; }
            [Column(3)] public double Length { get; set; }
            [Column(4)] public ushort NumQuestions { get; set; }
            [Column(5)] public decimal Cost { get; set; }
            [Column(6)] public bool Videotaped { get; }
        }

        // Scenario: No Viable Constructor and At Least One Read-Only Property (✗cannot reconstitute✗)
        public class Hypnotist {
            [PrimaryKey, Column(0)] public string Name { get; }
            [Column(1)] public byte Level { get; set; }
            [Column(2)] public ulong PeopleHypnotized { get; set; }
            [Column(3)] public bool Medical { get; }
            [Column(4)] public float AverageTimeToPutSomeoneUnder { get; }

            public Hypnotist(string name, byte level, ulong peopleHypnotized) {
                throw new InvalidOperationException("NON-VIABLE CONSTRUCTOR");
            }
            public Hypnotist(string name, byte level, ulong peopleHypnotized, bool medical, int extra, float averageTimeToPutSomeoneUnder) {
                throw new InvalidOperationException("NON-VIABLE CONSTRUCTOR");
            }
        }

        // Scenario: Constructor would be Viable but Argument is Convertible Type (✗cannot reconstitute✗)
        public class Paycheck {
            [PrimaryKey, Column(0)] public ulong Employee { get; }
            [PrimaryKey, Column(1)] public DateTime Period { get;}
            [Column(2)] public int HoursWorked { get; }
            [Column(3)] public double RatePerHour { get; }
            [Column(4)] public decimal Net { get; }
            [Column(5)] public decimal FederalIncomeTax { get; }
            [Column(6)] public decimal StateIncomeTax { get; }
            [Column(7)] public decimal SocialSecurity { get; }
            [Column(8)] public decimal OtherWithholdings { get; }
            [Column(9)] public decimal Gross { get; }

            public Paycheck(uint employee, DateTime period, int hoursWorked, double ratePerHour, decimal net,
                decimal federalIncomeTax, decimal stateIncomeTax, decimal socialSecurity, decimal otherWitholdings,
                decimal gross) {

                throw new InvalidOperationException("NON-VIABLE CONSTRUCTOR");
            }
        }

        // Scenario: Constructor would be Viable but Argument is Unconvertible Type (✗cannot reconstitute✗)
        public class Disneyland {
            [PrimaryKey, Column(0)] public string Name { get; }
            [Column(1)] public DateTime Opened { get; }
            [Column(2)] public ulong AnnualAttendance { get; }
            [Column(3)] public decimal AverageTicketePrice { get; }
            [Column(4)] public ushort NumImagineersEmployed { get; }
            [Column(5)] public ulong Area { get; }
            [Column(6)] public sbyte NumRollercoasters { get; }

            public Disneyland(string name, char opened, ulong annualAttendance, decimal averageTicketPrice,
                ushort numImagineersEmployed, ulong area, sbyte numRollercoasters) {

                throw new InvalidOperationException("NON-VIABLE CONSTRUCTOR");
            }
        }

        // Scenario: Constructor would be Viable but Argument for Nullable Field is Not Nullable (✗cannot reconstitute✗)
        public class Bakery {
            [PrimaryKey, Column(0)] public string BakeryName { get; set; } = "";
            [Column(1)] public string? Address { get; }
            [Column(2)] public byte? NumEmployees { get; }
            [Column(3)] public decimal AverageBagelCost { get; set; }
            [Column(4)] public decimal AverageMuffinCost { get; set; }
            [Column(5)] public decimal AverageCookieCost { get; set; }
            [Column(6)] public decimal AverageCakeCost { get; set; }
            [Column(7)] public DateTime GrandOpening { get; set; }
            [Column(8), Nullable] public bool ServesCoffee { get; set; }

            public Bakery(string address, byte? numEmployees, bool? servesCoffee) {
                throw new InvalidOperationException("NON-VIABLE CONSTRUCTOR");
            }
            public Bakery(string? address, byte numEmployees, bool? servesCoffee) {
                throw new InvalidOperationException("NON-VIABLE CONSTRUCTOR");
            }
            public Bakery(string? address, byte? numEmployees, bool servesCoffee) {
                throw new InvalidOperationException("NON-VIABLE CONSTRUCTOR");
            }
        }

        // Scenario: Constructor Not Viable because Multiple Arguments match Same Field (✗cannot reconstitute✗)
        public class ParticleAccelerator {
            [PrimaryKey, Column(0)] public string Name { get; }
            [Column(1)] public double MaxEnergy { get; }
            [Column(2)] public double MaxLuminosity { get; }
            [Column(3)] public bool DiscoveredHiggsBoson { get; set; }

            public ParticleAccelerator(string name, double maxEnergy, double maxLuminosity, double MAXENERGY) {
                throw new InvalidOperationException("NON-VIABLE CONSTRUCTOR");
            }
        }

        // Scenario: Viable Constructor with Nullable Argument for Non-Nullable Field (✓reconstituted✓)
        public class CovalentBond {
            [Column(0)] public string CommonName { get; set; } = "";
            [PrimaryKey, Column(1)] public string Anion { get; } = "";
            [PrimaryKey, Column(2), NonNullable] public string? Cation { get; } = "";
            [Column(3)] public double MolecularWeight { get; }

            public CovalentBond(string? anion, string? cation, double? molecularWeight) {
                Anion = anion!;
                Cation = cation;
                MolecularWeight = molecularWeight!.Value;
            }
        }

        // Scenario: Multiple Viable Public Constructors of Differing Arity (✓reconstituted✓)
        public class AmericanGirlDoll {
            [PrimaryKey, Column(0)] public string Name { get; set; } = "";
            [Column(1)] public ushort YearReleased { get; }
            [Column(2)] public string HomeCity { get; set; } = "";
            [Column(3)] public DateTime? Birthdate { get; }
            [Column(4)] public ulong DollsSold { get; set; }
            [Column(5)] public decimal RetailPrice { get; }

            public AmericanGirlDoll(ushort yearReleased, DateTime? birthdate, decimal retailPrice) {
                throw new InvalidOperationException("WRONG VIABLE CONSTRUCTOR");
            }
            public AmericanGirlDoll(string name, ushort yearReleased, DateTime? birthdate, decimal retailPrice) {
                throw new InvalidOperationException("WRONG VIABLE CONSTRUCTOR");
            }
            public AmericanGirlDoll(ushort yearReleased, DateTime? birthdate, string homeCity, decimal retailPrice, ulong dollsSold) {
                YearReleased = yearReleased;
                HomeCity = homeCity;
                Birthdate = birthdate;
                DollsSold = dollsSold;
                RetailPrice = retailPrice;
            }
        }

        // Scenario: Multiple Viable Public Constructors of Equal Arity for Same Fields (✗ambiguous✗)
        public class Ambulance {
            [PrimaryKey, Column(0)] public Guid VehicleID { get; set; }
            [Column(1)] public ulong PatientsTransported { get; }
            [Column(2)] public bool MedicalHelicopter { get; }
            [Column(3)] public string PrimaryHospital { get; set; } = "";
            [Column(4)] public bool RedCross { get; }

            public Ambulance(ulong patientsTransported, bool medicalHelicopter, bool redCross) {
                throw new InvalidOperationException("AMBIGUOUS CONSTRUCTOR");
            }
            public Ambulance(bool redCross, ulong patientsTransported, bool medicalHelicopter) {
                throw new InvalidOperationException("AMBIGUOUS CONSTRUCTOR");
            }
        }

        // Scenario: Multiple Viable Public Constructors of Equal Arity for Different Fields (✗ambiguous✗)
        public class CustomerServiceLine {
            [PrimaryKey, Column(0)] public string PhoneNumber { get; }
            [Column(1)] public string Company { get; set; }
            [Column(2)] public byte InitialMenuOptions { get; set; }
            [Column(3)] public bool SpanishAvailable { get; }
            [Column(4)] public double AverageWaitTime { get; set; }

            public CustomerServiceLine(string phoneNumber, bool spanishAvailable, double averageWaitTime) {
                throw new InvalidOperationException("AMBIGUOUS CONSTRUCTOR");
            }
            public CustomerServiceLine(string phoneNumber, bool spanishAvailable, byte initialMenuOptions) {
                throw new InvalidOperationException("AMBIGUOUS CONSTRUCTOR");
            }
        }

        // Scenario: Public and Non-Public Constructor where Latter Has Larger Arity (✓reconstituted via public✓)
        public class Winery {
            [PrimaryKey, Column(0)] public Guid WineryID { get; set; }
            [Column(1)] public string Name { get; set; } = "";
            [Column(2)] public string? LeadSommelier { get; }
            [Column(3)] public ulong NumReds { get; set; }
            [Column(4)] public ulong NumWhites { get; set; }
            [Column(5)] public ulong NumRoses { get; set; }
            [Column(6)] public Guid LiquorLicense { get; set; }

            public Winery(string? leadSommelier) {
                LeadSommelier = leadSommelier;
            }
            private Winery(string? leadSommelier, ulong numReds, ulong numWhites, ulong numRoses) {
                throw new InvalidOperationException("WRONG VIABLE CONSTRUCTOR");
            }
        }

        // Scenario: Viable Constructor in Partial Class (✓reconstituted✓)
        public partial class Constructor {
            public enum AccessModifier { Public, Private, Protected, Internal, InternalProtected };

            [PrimaryKey, Column(0)] public string Type { get; }
            [PrimaryKey, Column(1)] public byte Index { get; }
            [Column(2)] public AccessModifier Visibility { get; }
            [Column(3)] public int Arity { get; set; }
            [Column(4)] public bool CanThrowException { get; set; }
            [Column(5)] public bool IsExplicit { get; set; }
        }
        public partial class Constructor {
            public Constructor(string type, byte index, AccessModifier visibility) {
                Type = type;
                Index = index;
                Visibility = visibility;
            }
        }

        // Scenario: Viable Constructor with Fields Renamed (✓reconstituted✓)
        public class Petroglyph {
            [PrimaryKey, Column(0), Name("Identifier")] public Guid ArchaeologicalIdentifer { get; }
            [Column(1)] public float Latitude { get; set; }
            [Column(2)] public float Longtiude { get; set; }
            [Column(3), Name("Rock")] public string Location { get; }
            [Column(4)] public double Height { get; set; }
            [Column(5)] public string TypeOfRock { get; set; } = "";

            public Petroglyph(Guid identifier, string rock) {
                ArchaeologicalIdentifer = identifier;
                Location = rock;
            }
            public Petroglyph(Guid archaeologicalIdentifier, float latitude, float longitude, string location, double height, string typeOfRock) {
                throw new InvalidOperationException("NON-VIABLE CONSTRUCTOR");
            }
        }

        // Scenario: Viable Constructor with [Calculated] Fields (✓reconstituted✓)
        public class PaintballGun {
            [PrimaryKey, Column(0)] public Guid ProductID { get; }
            [Column(1)] public ushort PaintballCapacity { get; }
            [Column(2)] public double DispatchSpeed { get; }
            [Calculated, Column(3)] public bool ProLegal => PaintballCapacity <= 100 && DispatchSpeed > 0.05;
            [Column(4)] public bool Semiautomatic { get; }

            public PaintballGun(Guid productID, ushort paintballCapacity, double dispatchSpeed, bool semiautomatic) {
                ProductID = productID;
                PaintballCapacity = paintballCapacity;
                DispatchSpeed = dispatchSpeed;
                Semiautomatic = semiautomatic;
            }
        }

        // Scenario: Viable Constructor for Aggregate (✓reconstituted✓)
        public class BaobabTree {
            public readonly struct Coordinate {
                [Column(0)] public float Latitude { get; }
                [Column(1)] public float Longitude { get; }

                public Coordinate(float longitude, float latitude) {
                    Latitude = latitude;
                    Longitude = longitude;
                }
            }

            [PrimaryKey, Column(0)] public Guid TreeID { get; set; }
            [Column(1)] public ulong Height { get; set; }
            [Column(2)] public ulong RootCoverage { get; set; }
            [Column(3)] public string? Forest { get; set; }
            [Column(4)] public Coordinate ExactLocation { get; set; }
            [Column(6)] public ushort Age { get; set; }
        }

        // Scenario: Viable Constructor for Aggregate with Fields Renamed in Entity (✓reconstituted✓)
        public class GPU {
            public enum Field { Gaming, CloudGaming, Workstation, CloudWorkstation, AI, DriverlessCars, Other }

            public struct Spec {
                [Column(0)] public Field PrimaryField { get; }
                [Column(1)] public ushort MegaHertz { get; }
                [Column(2)] public ushort GigaBytes { get; }

                public Spec(ushort megahertz, ushort gigabytes, Field primaryField) {
                    PrimaryField = primaryField;
                    MegaHertz = megahertz;
                    GigaBytes = gigabytes;
                }
            }

            [PrimaryKey, Column(0)] public Guid ChipID { get; set; }
            [Column(1)] public string Manufacturer { get; set; } = "";
            [Column(2)] public string Model { get; set; } = "";
            [Column(3), Name("ClockSpeed", Path = "MegaHertz"), Name("Memory", Path ="GigaBytes")] public Spec Specification { get; set; }
            [Column(6)] public bool HasVideoCard { get; set; }
            [Column(7)] public bool Discontinued { get; set; }
        }

        // Scenario: No Viable Constructor for Aggregate and All Properties are Writeable (✗cannot reconstitute✗)
        public class Gargoyle {
            public struct Rock {
                [Column(0)] public string Name { get; set; }
                [Column(1)] public double Hardnes { get; set; }
                [Column(2)] public double Weight { get; set; }

                public Rock(Guid rockID) {
                    throw new InvalidOperationException("NON-VIABLE CONSTRUCTOR");
                }
            }

            [PrimaryKey, Column(0)] public Guid GargoyleID { get; set; }
            [Column(1)] public Rock Material { get; set; }
            [Column(2)] public string Shape { get; set; } = "";
            [Column(3)] public string Location { get; set; } = "";
            [Column(4)] public bool FullyIntact { get; set; }
        }

        // Scenario: No Viable Constructor for Aggregate and At Least One Read-Only Property (✗cannot reconstitute✗)
        public class SecurityClearance {
            public enum SecurityLevel { Secret, TopSecret, Confidential, L, Q }
            [Flags] public enum Agency { CIA = 1, NSA = 2, FBI = 4, JSOC = 8, EOP = 16 }

            public struct Position {
                [Column(0)] public string Department { get; }
                [Column(1)] public string Role { get; set; }
            }

            [PrimaryKey, Column(0)] public string Individual { get; set; } = "";
            [PrimaryKey, Column(1)] public SecurityLevel Level { get; set; }
            [Column(2)] public Position GovernmentPosition { get; set; }
            [Column(4)] public bool IsActive { get; set; }
            [Column(5)] public Agency IssuingAgency { get; set; }
        }

        // Scenario: Viable Constructor for Relation-Nested Aggregate (✓reconstituted✓)
        public class IllithidPower {
            public enum Act { Act1, Act2, Act3 }
            public enum Kind { PassiveAction, ClassAbility, ToggleablePassiveFeature, Reaction }

            public readonly struct Feature {
                [Column(0)] public string Trigger { get; }
                [Column(1)] public string Modifier { get; }
                [Column(2)] public ulong Value { get; }

                public Feature(string trigger, string modifier, ulong value) {
                    Trigger = trigger;
                    Modifier = modifier;
                    Value = value;
                }
            }

            [PrimaryKey, Column(0)] public string PowerName { get; set; } = "";
            [Column(1)] public Act ActUnlocked { get; set; }
            [Column(2)] public Kind KindOfPower { get; set; }
            public RelationList<Feature> Features { get; } = new RelationList<Feature>();
        }

        // Scenario: No Viable Constructor for Aggregate Type Backing only [Calculated] Properties (✓reconstituted✓)
        public class Requiem {
            public readonly struct TimeSignature {
                [Column(0)] public int Top { get; }
                [Column(1)] public int Bottom { get; }

                public TimeSignature(string signature) {
                    Top = int.Parse(signature.Split("/")[0]);
                    Bottom = int.Parse(signature.Split("/")[1]);
                }
            }

            [PrimaryKey, Column(0)] public string Composer { get; set; } = "";
            [PrimaryKey, Column(1)] public int OpusNumber { get; set; }
            [Calculated, Column(2)] public TimeSignature Signature { get; } = new TimeSignature("4/4");
            [Column(4)] public double Length { get; set; }
            [Column(5)] public DateTime? Premiered { get; set; }
        }

        // Scenario: [ReconstituteThrough] on Viable Public Constructor (✓reconstituted✓)
        public class Beekeeper {
            public enum Classification { Commercial, Hobby, Sideline, Poaching }

            [PrimaryKey, Column(0)] public Guid BeekeeperID { get; set; }
            [Column(1)] public string Name { get; set; } = "";
            [Column(2)] public ulong BeesKept { get; }
            [Column(3)] public ulong NumTimesStung { get; }
            [Column(4)] public Classification Kind { get; set; }
            [Column(5)] public int GallonsHoneyProduced { get; set; }

            public Beekeeper(Guid beekeeperId, string name, ulong beesKept, ulong numTimesStung, Classification kind, int gallonsHoneyProduced) {
                throw new InvalidOperationException("WRONG CONSTRUCTOR");
            }

            [ReconstituteThrough]
            public Beekeeper(ulong beesKept, ulong numTimesStung) {
                BeesKept = beesKept;
                NumTimesStung = numTimesStung;
            }
        }

        // Scenario: [ReconstituteThrough] on Viable Non-Public Constructor (✓reconstituted✓)
        public class Nightclub {
            [PrimaryKey, Column(0)] public string ClubName { get; } = "";
            [PrimaryKey, Column(1)] public string ClubCity { get; } = "";
            [Column(2)] public uint Capacity { get; set; }
            [Column(3)] public decimal Cover { get; set; }
            [Column(4)] public Guid? LiquorLicense { get; set; }
            [Column(5)] public sbyte NumBouncers { get; }

            public Nightclub(string clubName, string clubCity, sbyte numBouncers) {
                throw new InvalidOperationException("WRONG CONSTRUCTOR");
            }

            [ReconstituteThrough]
            private Nightclub(string clubCity, sbyte numBouncers, string clubName) {
                ClubCity = clubCity;
                NumBouncers = numBouncers;
                ClubName = clubName;
            }
        }

        // Scenario: [ReconstituteThrough] on Non-Viable Constructor (✗impermissible✗)
        public class Condom {
            [PrimaryKey, Column(0)] public Guid ProductID { get; }
            [Column(1)] public string Brand { get; }
            [Column(2)] public string Size { get; }
            [Column(3)] public bool IsLatex { get; }
            [Column(4)] public bool IsFlavored { get; }
            [Column(5)] public bool IsUsed { get; }

            [ReconstituteThrough]
            public Condom(Guid productId, string size, bool isUsed, bool isLatex) {
                throw new InvalidOperationException("NON-VIABLE CONSTRUCTOR");
            }
        }

        // Scenario: Multiple Constructors Marked as [ReconstituteThrough] (✗cardinality✗)
        public class AntColony {
            public enum Relationship { Monogyny, Polygyny, Oligogyny, Haplometrosis, Pleometrosis, Monodomy, Polydomy }

            public record struct Species(string Genus, string SpeciesName);

            [PrimaryKey, Column(0)] public Guid ColonyID { get; }
            [Column(1)] public ulong Population { get; }
            [Column(2)] public Species AntSpecies { get; }
            [Column(4)] public ulong NumMounds { get; }
            [Column(5)] public Relationship Organization { get; }

            [ReconstituteThrough]
            public AntColony() {
                throw new InvalidOperationException("NON-VIABLE CONSTRUCTOR");
            }

            [ReconstituteThrough]
            public AntColony(Guid colonyId, ulong population, Species species, ulong numMounds, Relationship organization) {
                throw new InvalidOperationException("NON-VIABLE CONSTRUCTOR");
            }
        }

        // Scenario: [ReconsituteThrough] for an Aggregate (✓reconstituted✓)
        public class Aphrodisiac {
            public struct MakeUp {
                [Column(0)] public string Formula { get; }
                [Column(1)] public bool IsAromatic { get; set; }

                public MakeUp(string formula, bool isAromatic) {
                    throw new InvalidOperationException("WRONG CONSTRUCTOR");
                }

                [ReconstituteThrough]
                public MakeUp(string formula) {
                    Formula = formula;
                }
            }

            [PrimaryKey, Column(0)] public string Identifier { get; set; } = "";
            [Column(1)] public double Strength { get; set; }
            [Column(2)] public MakeUp? ChemicalStructure { get; set; }
            [Column(4)] public string? DiscoveringCivilization { get; set; }
        }

        // Scenario: [ReconstituteThrough] on Private Pre-Defined Entity Constructor (✗impermissible✗)
        [PreDefined] public class Vertebra {
            public enum Section { Cervical, Thoracic, Lumbar, Sacral, Coccygeal }

            [PrimaryKey, Column(0)] public Guid ID { get; private init; }
            [Column(1)] public string Designation { get; private init; }
            [Column(2)] public Section SpinalSegment { get; private init; }

            public static Vertebra C1 { get; } = new Vertebra("C1", Section.Cervical);
            public static Vertebra C2 { get; } = new Vertebra("C2", Section.Cervical);
            public static Vertebra C3 { get; } = new Vertebra("C3", Section.Cervical);
            public static Vertebra C4 { get; } = new Vertebra("C4", Section.Cervical);
            public static Vertebra C5 { get; } = new Vertebra("C5", Section.Cervical);
            public static Vertebra C6 { get; } = new Vertebra("C6", Section.Cervical);
            public static Vertebra C7 { get; } = new Vertebra("C7", Section.Cervical);
            public static Vertebra T1 { get; } = new Vertebra("T1", Section.Thoracic);
            public static Vertebra T2 { get; } = new Vertebra("T2", Section.Thoracic);
            public static Vertebra T3 { get; } = new Vertebra("T3", Section.Thoracic);
            public static Vertebra T4 { get; } = new Vertebra("T4", Section.Thoracic);
            public static Vertebra T5 { get; } = new Vertebra("T5", Section.Thoracic);
            public static Vertebra T6 { get; } = new Vertebra("T6", Section.Thoracic);
            public static Vertebra T7 { get; } = new Vertebra("T7", Section.Thoracic);
            public static Vertebra T8 { get; } = new Vertebra("T8", Section.Thoracic);
            public static Vertebra T9 { get; } = new Vertebra("T9", Section.Thoracic);
            public static Vertebra T10 { get; } = new Vertebra("T10", Section.Thoracic);
            public static Vertebra T11 { get; } = new Vertebra("T11", Section.Thoracic);
            public static Vertebra T12 { get; } = new Vertebra("T12", Section.Thoracic);
            public static Vertebra L1 { get; } = new Vertebra("L1", Section.Lumbar);
            public static Vertebra L2 { get; } = new Vertebra("L2", Section.Lumbar);
            public static Vertebra L3 { get; } = new Vertebra("L3", Section.Lumbar);
            public static Vertebra L4 { get; } = new Vertebra("L4", Section.Lumbar);
            public static Vertebra L5 { get; } = new Vertebra("L5", Section.Lumbar);
            public static Vertebra S1 { get; } = new Vertebra("S1", Section.Sacral);
            public static Vertebra S2 { get; } = new Vertebra("S2", Section.Sacral);
            public static Vertebra S3 { get; } = new Vertebra("S3", Section.Sacral);
            public static Vertebra S4 { get; } = new Vertebra("S4", Section.Sacral);
            public static Vertebra S5 { get; } = new Vertebra("S5", Section.Sacral);
            public static Vertebra Co1 { get; } = new Vertebra("Co1", Section.Coccygeal);
            public static Vertebra Co2 { get; } = new Vertebra("Co2", Section.Coccygeal);
            public static Vertebra Co3 { get; } = new Vertebra("Co3", Section.Coccygeal);
            public static Vertebra Co4 { get; } = new Vertebra("Co4", Section.Coccygeal);
            public static Vertebra Co5 { get; } = new Vertebra("Co5", Section.Coccygeal);

            [ReconstituteThrough] private Vertebra(string designation, Section segment) {
                ID = Guid.NewGuid();
                Designation = designation;
                SpinalSegment = segment;
            }
        }

        // Scenario: Public Pre-Defined Instance (✓reconstituted✓)
        [PreDefined] public class Tortilla {
            [PrimaryKey, Column(0)] public string Name { get; private init; }
            [Column(1)] public bool IsAuthenticMexican { get; private init; }
            [Column(2)] public decimal? CostcoCost { get; private init; }

            public static Tortilla Flour { get; } = new Tortilla("Flour", true, null);
            public static Tortilla Corn { get; } = new Tortilla("Corn", true, null);
            public static Tortilla Nopal { get; } = new Tortilla("Nopal", true, null);
            public static Tortilla WholeWheat { get; } = new Tortilla("Whole Wheat", false, null);

            private Tortilla(string name, bool isAuthentic, decimal? cost) {
                Name = name;
                IsAuthenticMexican = isAuthentic;
                CostcoCost = cost;
            }
        }

        // Test Scenario: Public Pre-Defined Instance that was Marked as [IncludeInModel] (✓reconstituted✓)
        [PreDefined] public class Symbiosis {
            public enum Cardinality { None, SingleBenefitNoHarm, SingleHarmNoBenefit, SingleBenefitSingleHarm, DoubleBenefit, DoubleHarm }

            [PrimaryKey, Column(0)] public int ID { get; private init; }
            [Column(1)] public string Name { get; private init; }
            [Column(2)] public string Example { get; private init; }
            [Column(3)] public Cardinality Arity { get; private init; }

            [IncludeInModel] public static Symbiosis Commensalism { get; } = new Symbiosis(0, "Commensalism", "Sea Turtle & Remora", Cardinality.SingleBenefitNoHarm);
            [IncludeInModel] public static Symbiosis Parasitismm { get; } = new Symbiosis(1, "Parasitism", "Human & Mosquito", Cardinality.SingleBenefitSingleHarm);
            [IncludeInModel] public static Symbiosis Mutualism { get; } = new Symbiosis(2, "Mutualism", "Clownfish & Sea Anemone", Cardinality.DoubleBenefit);
            [IncludeInModel] public static Symbiosis Amensalism { get; } = new Symbiosis(3, "Amensalism", "Spanish Ibex & Timarcha Weevils", Cardinality.SingleHarmNoBenefit);

            private Symbiosis(int id, string name, string example, Cardinality arity) {
                ID = id;
                Name = name;
                Example = example;
                Arity = arity;
            }
        }

        // Scenario: Public Pre-Defined Instance that was Marked as [CodeOnly] (✗pathologically fails✗)
        [PreDefined] public class MetricPrefix {
            [PrimaryKey, Column(0)] public string Prefix { get; private init; }
            [Column(1)] public int Base10 { get; private init; }
            [Column(2)] public ushort YearAdopted { get; private init; }
            [Column(3)] public char Symbol { get; private init; }

            public static MetricPrefix Quetta { get; } = new MetricPrefix("quetta", 'Q', 30, 2022);
            public static MetricPrefix Ronna { get; } = new MetricPrefix("ronna", 'R', 27, 2022);
            public static MetricPrefix Yotta { get; } = new MetricPrefix("yotta", 'Y', 24, 1991);
            public static MetricPrefix Zetta { get; } = new MetricPrefix("zetta", 'Z', 21, 1991);
            public static MetricPrefix Exa { get; } = new MetricPrefix("exa", 'E', 18, 1975);
            public static MetricPrefix Peta { get; } = new MetricPrefix("peta", 'P', 15, 1975);
            public static MetricPrefix Tera { get; } = new MetricPrefix("tera", 'T', 12, 1960);
            public static MetricPrefix Giga { get; } = new MetricPrefix("giga", 'G', 9, 1960);
            public static MetricPrefix Mega { get; } = new MetricPrefix("mega", 'M', 6, 1873);
            public static MetricPrefix Kilo { get; } = new MetricPrefix("kilo", 'k', 3, 1795);
            public static MetricPrefix Hecto { get; } = new MetricPrefix("hecto", 'h', 2, 1795);
            public static MetricPrefix Deci { get; } = new MetricPrefix("deci", 'd', -1, 1795);
            public static MetricPrefix Centi { get; } = new MetricPrefix("centi", 'c', -2, 1795);
            public static MetricPrefix Milli { get; } = new MetricPrefix("milli", 'm', -3, 1795);
            public static MetricPrefix Micro { get; } = new MetricPrefix("micro", 'μ', -6, 1873);
            public static MetricPrefix Nano { get; } = new MetricPrefix("nano", 'n', -9, 1960);
            public static MetricPrefix Pico { get; } = new MetricPrefix("pico", 'p', -12, 1960);
            public static MetricPrefix Femto { get; } = new MetricPrefix("femto", 'f', -15, 1964);
            public static MetricPrefix Atto { get; } = new MetricPrefix("atto", 'a', -18, 1964);
            public static MetricPrefix Zepto { get; } = new MetricPrefix("zepto", 'z', -21, 1991);
            public static MetricPrefix Yocto { get; } = new MetricPrefix("yocto", 'y', -24, 1991);
            public static MetricPrefix Ronto { get; } = new MetricPrefix("ronto", 'r', -27, 2022);
            [CodeOnly] public static MetricPrefix Quecto { get; } = new MetricPrefix("quecto", 'q', -30, 2022);

            private MetricPrefix(string prefix, char symbol, int base10, ushort yearAdopted) {
                Prefix = prefix;
                Base10 = base10;
                YearAdopted = yearAdopted;
                Symbol = symbol;
            }
        }

        // Scenario: Non-Public Pre-Defined Instance (✗pathologically fails✗)
        [PreDefined] public class Organelle {
            [PrimaryKey, Column(0)] public string Name { get; private init; }
            [Column(1)] public string? AKA { get; private init; }
            [Column(2)] public bool FoundInAnimals { get; private init; }
            [Column(3)] public bool MembraneBound { get; private init; }

            public static Organelle Nucleolus { get; } = new Organelle("Nucleolus", null, true, true);
            public static Organelle Nucleus { get; } = new Organelle("Nucleus", "Cell Nucleus", true, true);
            public static Organelle Ribosome { get; } = new Organelle("Ribosome", null, true, true);
            public static Organelle Vesicle { get; } = new Organelle("Vesicle", null, true, false);
            public static Organelle SmoothER { get; } = new Organelle("Smooth Endoplasmic Reticulum", "Smooth ER", true, true);
            public static Organelle RoughER { get; } = new Organelle("Rough Endoplasmic Reticulum", "Rough ER", true, true);
            public static Organelle GolgiBody { get; } = new Organelle("Golgi Body", "Golgi Apparatus", true, true);
            public static Organelle Mitochondrion { get; } = new Organelle("Mitochondrion", null, true, true);
            public static Organelle Vacuole { get; } = new Organelle("Vacuole", null, true, true);
            public static Organelle Lysosome { get; } = new Organelle("Lysosome", null, true, true);
            public static Organelle Centrosome { get; } = new Organelle("Centrosome", null, true, false);
            public static Organelle Chloroplast { get; } = new Organelle("Chloroplast", null, false, true);
            private static Organelle Flagellum { get; } = new Organelle("Flagellum", null, false, true);

            private Organelle(string name, string? aka, bool animal, bool membraneBound) {
                Name = name;
                AKA = aka;
                FoundInAnimals = animal;
                MembraneBound = membraneBound;
            }
        }

        // Test Scenario: Non-Public Pre-Defined Instance that was Marked as [CodeOnly] (✗pathologically fails✗)
        [PreDefined] public class LawAndOrder {
            [PrimaryKey, Column(0)] public ulong IMDbID { get; private init; }
            [Column(1)] public string Title { get; private init; }
            [Column(2)] public bool IsActive { get; private init; }
            [Column(3)] public ulong NumEpisodes { get; private init; }
            [Column(4)] public DateTime Premiere { get; private init; }

            public static LawAndOrder Original { get; } = new LawAndOrder(99844, "Law & Order", true, 512, new DateTime(1990, 9, 13));
            public static LawAndOrder SVU { get; } = new LawAndOrder(203259, "Law & Order: Special Victims Unit", true, 562, new DateTime(1999, 9, 20));
            public static LawAndOrder TrialByJury { get; } = new LawAndOrder(406429, "Law & Order: Trial by Jury", false, 13, new DateTime(2005, 3, 5));
            public static LawAndOrder CriminalIntent { get; } = new LawAndOrder(275140, "Law & Order: Criminal Intent", false, 195, new DateTime(2001, 9, 30));
            public static LawAndOrder OrganizedCrime { get; } = new LawAndOrder(12677870, "Law & Order: Organized Crime", true, 65, new DateTime(2021, 4, 1));
            [CodeOnly] protected static LawAndOrder LA { get; } = new LawAndOrder(1657081, "Law & Order: LA", false, 22, new DateTime(2010, 9, 29));

            private LawAndOrder(ulong id, string title, bool active, ulong episodes, DateTime premiere) {
                IMDbID = id;
                Title = title;
                IsActive = active;
                NumEpisodes = episodes;
                Premiere = premiere;
            }
        }

        // Scenario: Non-Existent Pre-Defined Instance (✗pathologically fails✗)
        [PreDefined] public class BrazilianState {
            [PrimaryKey, Column(0)] public string Name { get; private init; }
            [Column(1)] public string Capital { get; private init; }
            [Column(2)] public ulong Population { get; private init; }

            public static BrazilianState Acre { get; } = new BrazilianState("Acre", "Rio Branco", 830018);
            public static BrazilianState Alagoas { get; } = new BrazilianState("Alagoas", "Maceió", 3127683);
            public static BrazilianState Amapa { get; } = new BrazilianState("Amapá", "Macapá", 733759);
            public static BrazilianState Amazonas { get; } = new BrazilianState("Amazonas", "Manaus", 3941613);
            public static BrazilianState Bahia { get; } = new BrazilianState("Bahia", "Salvador", 14141626);
            public static BrazilianState Ceara { get; } = new BrazilianState("Ceará", "Fortaleza", 8794957);
            public static BrazilianState DistritoFederal { get; } = new BrazilianState("Distrito Federal", "Brasília", 2817381);
            public static BrazilianState EspiritoSanto { get; } = new BrazilianState("Espírito Santo", "Vitória", 3833712);
            public static BrazilianState Goias { get; } = new BrazilianState("Goiás", "Goiânia", 7056495);
            public static BrazilianState Maranhao { get; } = new BrazilianState("Maranhão", "São Luís", 6776699);
            public static BrazilianState MatoGrosso { get; } = new BrazilianState("Mato Grosso", "Cuiabá", 3658649);
            public static BrazilianState MatoGrossoDoSul { get; } = new BrazilianState("Mato Groso do Sul", "Campo Grande", 2880308);
            public static BrazilianState MinasGerais { get; } = new BrazilianState("Minas Gerais", "Belo Horizonte", 21279353);
            public static BrazilianState Para { get; } = new BrazilianState("Pará", "Belém", 8639532);
            public static BrazilianState Paraiba { get; } = new BrazilianState("Paraíba", "João Pessoa", 4175326);
            public static BrazilianState Parana { get; } = new BrazilianState("Paraná", "Curitiba", 11623091);
            public static BrazilianState Pernambuco { get; } = new BrazilianState("Pernambuco", "Recife", 9645321);
            public static BrazilianState Piaui { get; } = new BrazilianState("Piauí", "Teresina", 3341352);
            public static BrazilianState RioDeJaneiro { get; } = new BrazilianState("Rio de Janeiro", "Rio de Janeiro", 16055174);
            public static BrazilianState RioGrandeDoNorte { get; } = new BrazilianState("Rio Grande do Norte", "Natal", 3619619);
            public static BrazilianState RioGrandeDoSul { get; } = new BrazilianState("Rio Grande do Sul", "Porto Alegre", 10882965);
            public static BrazilianState Rondonia { get; } = new BrazilianState("Rondônia", "Porto Velho", 1837905);
            public static BrazilianState Roraima { get; } = new BrazilianState("Roraima", "Boa Vista", 708352);
            public static BrazilianState SantaCatarina { get; } = new BrazilianState("Santa Catarina", "Florianópolis", 7218704);
            public static BrazilianState SaoPaulo { get; } = new BrazilianState("São Paulo", "São Paulo", 44411238);
            public static BrazilianState Sergipe { get; } = new BrazilianState("Sergipe", "Aracaju", 2403563);
            public static BrazilianState Tocantins { get; } = new BrazilianState("Tocantins", "Palmas", 1692452);

            private BrazilianState(string name, string capital, ulong population) {
                Name = name;
                Capital = capital;
                Population = population;
            }
        }

        // Scenario: Relation on Pre-Defined Entity (✓skipped✓)
        [PreDefined] public class PizzaRoll {
            [PrimaryKey, Column(0)] public Guid ID { get; private init; }
            [Column(1)] public string Flavor { get; private init; }
            public IReadOnlyRelationSet<string> Ingredients { get; private init; }
            [Column(2)] public double Rating { get; private init; }

            public static PizzaRoll Cheese { get; } = new PizzaRoll("cheese", 4.5, "cheese", "pizza sauce");
            public static PizzaRoll Pepperoni { get; } = new PizzaRoll("pepperoni", 4.2, "cheese", "pepperoni", "pizza sauce");
            public static PizzaRoll BuffaloChicken { get; } = new PizzaRoll("buffalo-style chicken", 4.4, "chicken", "cheese", "hot sauce");
            public static PizzaRoll Supreme { get; } = new PizzaRoll("supreme", 3.9, "cheese", "chicken", "pork", "pepperoni", "onion", "green pepper", "pizza sauce");
            public static PizzaRoll TripleMeat { get; } = new PizzaRoll("triple meat", 4.7, "cheese", "chicken", "pepperoni", "beef", "pizza sauce");
            public static PizzaRoll Combination { get; } = new PizzaRoll("combination", 4.6, "cheese", "pepperoni", "sausage", "pizza sauce");

            private PizzaRoll(string flavor, double rating, params string[] ingredients) {
                ID = Guid.NewGuid();
                Flavor = flavor;
                Rating = rating;
                Ingredients = new RelationSet<string>(ingredients);
            }
        }
    }
}
