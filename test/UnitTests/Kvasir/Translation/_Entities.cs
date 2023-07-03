using Kvasir.Annotations;
using System;

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

        // Test Scenario: Type from the C# Standard Library (✗not permitted✗)
        public class Coin {
            [PrimaryKey] public byte Value { get; set; }
            public float Diameter { get; set; }
            [PrimaryKey] public bool InCirculation { get; set; }
            public Exception CounterfeitResult { get; set; } = new ApplicationException("COUNTERFEIT!");
        }

        // Test Scenario: Type from a Third-Party NuGet Package (✗not permitted✗)
        public class UUID {
            [PrimaryKey] public string Value { get; set; } = "";
            [PrimaryKey] public byte Version { get; set; }
            public Optional.Option<string> Signature { get; set; }
            public string Encoding { get; set; } = "";
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
        public enum WeaponType { Simple, Martial, Improvised };
        [Flags] public enum WeaponProperty { Ranged = 1, TwoHanded = 2, Finesse = 4, Silvered = 8 };
        public class DNDWeapon {
            [PrimaryKey] public string Name { get; set; } = "";
            public ushort AttackBonus { get; set; }
            public ushort AverageDamage { get; set; }
            public WeaponType Type { get; set; }
            public WeaponProperty Properties { get; set; }
            public DayOfWeek? MostEffectiveOn { get; set; }
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
            [PrimaryKey] public string Exoynym { get; set; } = "";
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

        // Test Scenario: Nullable Scalar Property Marked as [NonNullable] (✓becomes non-nullable✓)
        public class Bone {
            [PrimaryKey] public uint TA2 { get; set; }
            [NonNullable] public string? Name { get; set; }
            public string? LatinName { get; set; } = "";
            public string MeSH { get; set; } = "";
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
    }

    internal static class TableNaming {
        // Test Scenario: New Name (✓renamed✓)
        [Table("DeckOfCards")]
        public class PlayingCard {
            [PrimaryKey] public byte Suit { get; set; }
            [PrimaryKey] public byte Value { get; set; }
        }

        // Test Scenario: Namespace Excluded (✓renamed✓)
        [ExcludeNamespaceFromName]
        public class Pokemon {
            [PrimaryKey] public ushort PokedexNumber { get; set; }
            public string PrimaryType { get; set; } = "";
            public string? SecondaryType { get; set; }
            public string Name { get; set; } = "";
            public string JapaneseName { get; set; } = "";
            public byte HP { get; set; }
        }

        // Test Scenario: Two Entities Given Same Primary Table Name (✗duplication✗)
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

        // Test Scenario: Name Changed to the Empty String (✗illegal✗)
        [Table("")]
        public class LogIn {
            [PrimaryKey] public string Username { get; set; } = "";
            public string Password { get; set; } = "";
        }

        // Test Scenario: Type with Unchanged Name Marked with [Table] and [ExcludeNamespaceFromName] (✓renamed✓)
        [Table("UT.Kvasir.Translation.TableNaming+BlenderTable"), ExcludeNamespaceFromName]
        public class Blender {
            [PrimaryKey] public Guid ProductID { get; set; }
            public string Brand { get; set; } = "";
            public double PulseSpeed { get; set; }
            public byte NumBlades { get; set; }
        }

        // Test Scenario: Type with Changed Name Marked with [Table] and [ExcludeNamespaceFromName] (✓redundant✓)
        [Table("SomeTable"), ExcludeNamespaceFromName]
        public class Encryption {
            [PrimaryKey] public string Scheme { get; set; } = "";
            public ulong PublicKey { get; set; }
            public ulong PrivateKey { get; set; }
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

        // Test Scenario: Change Field Name to New Value (✓renamed✓)
        public class River {
            [PrimaryKey] public string Name { get; set; } = "";
            [Name("SourceElevation")] public ushort Ahuiehknaafuyur { get; set; }
            [Name("Length")] public ushort OEperaehrugyUIWJKuygajk { get; set; }
            public decimal MouthLatitude { get; set; }
            public decimal MouthLongitude { get; set; }
        }

        // Test Scenario: Swap Names of Fields (✓renamed✓)
        public class Episode {
            [PrimaryKey, Name("Number")] public byte Season { get; set; }
            [PrimaryKey, Name("Season")] public short Number { get; set; }
            public float Length { get; set; }
            public int? Part { get; set; }
            public string Title { get; set; } = "";
        }

        // Test Scenario: Change Field Name to Existing Value (✗duplication✗)
        public class ComputerLock {
            [PrimaryKey] public string Name { get; set; } = "";
            public bool IsReentrant { get; set; }
            [Name("IsReentrant")] public bool IsRecursive { get; set; }
            public ushort? ReadersPermitted { get; set; }
            public ushort? WritersPermitted { get; set; }
        }

        // Test Scenario: Change To Field Names to Same Value (✗duplication✗)
        public class Ticket2RideRoute {
            [PrimaryKey, Name("Destination")] public string City1 { get; set; } = "";
            [PrimaryKey, Name("Destination")] public string City2 { get; set; } = "";
            public byte Points { get; set; }
        }

        // Test Scenario: Scalar Property with Multiple [Name] Changes (✗cardinality✗)
        public class BankAccount {
            public string Bank { get; set; } = "";
            [PrimaryKey] public string AccountNumber { get; set; } = "";
            [Name("Route"), Name("RoutingNumber")] public ulong RoutingNumber { get; set; }
        }

        // Test Scenario: Name is Unchanged via [Name] (✓redundant✓)
        public class Opera {
            [PrimaryKey] public Guid ID { get; set; }
            public string Composer { get; set; } = "";
            [Name("PremiereDate")] public DateTime PremiereDate { get; set; }
            public uint Length { get; set; }
        }

        // Test Scenario: Name is Changed to null (✗illegal✗)
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

        // Test Scenario: Non-`null` Invalid Enumeration Default (✗invalid✗)
        public class HallOfFame {
            public enum Category { Sports, Entertainment, Journalism }

            [PrimaryKey] public string For { get; set; } = "";
            public uint Enshrinees { get; set; }
            [Default((Category)185)] public Category Categorization { get; set; }
            public float Latitude { get; set; }
            public float Longitude { get; set; }
            public DateTime Opened { get; set; }
        }

        // Test Scenario: `null` Default for Nullable Scalar Field (✓valid✓)
        public class Pepper {
            [PrimaryKey] public string Genus { get; set; } = "";
            [PrimaryKey] public string Species { get; set; } = "";
            [Default(null)] public string? CommonName { get; set; }
            [Default(null)] public DateTime? FirstCultivated { get; set; }
            public ulong ScovilleRating { get; set; }
        }

        // Test Scenario: `null` Default for Nullable Enumeration Filed (✓valid✓)
        public class Cryptid {
            public enum Continent { NorthAmerica, SouthAmerica, Asia, Europe, Africa, Oceania, Antarctica };
            [Flags] public enum Features { Flying = 1, Carnivorous = 2, Humanoid = 4, Aquatic = 8, FireProof = 16, Hematophagous = 32 };

            [PrimaryKey] public string Name { get; set; } = "";
            public uint AllegedSightings { get; set; }
            [Default(null)] public Continent? HomeContinent { get; set; }
            [Default(null)] public Features? FeatureSet { get; set; }
            public bool ProvenHoax { get; set; }
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
            [Column(2)] public ushort EndChapter { get; set; }
            [Column(3)] public ushort EndVerse { get; set; }
        }

        // Test Scenario: Two Fields Ordered to Same Index (✗duplication✗)
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

        // Test Scenario: Manual Ordering Leaves Gaps (✗non-consecutive✗)
        public class PhoneNumber {
            [PrimaryKey, Column(1)] public byte CountryCode { get; set; }
            [PrimaryKey] public ushort AreaCode { get; set; }
            [PrimaryKey, Column(14)] public ushort Number { get; set; }
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

        // Test Scenario: Scalar Property is Marked as [PrimaryKey] Multiple Times (✓redundant✓)
        public class Airport {
            [PrimaryKey, PrimaryKey] public string IATA { get; set; } = "";
            public string Name { get; set; } = "";
            public string City { get; set; } = "";
            public DateTime Opening { get; set; }
            public float AveragePassengers { get; set; }
        }

        // Test Scenario: Nullable Field is Marked as [PrimaryKey] (✗illegal✗)
        public class NorseWorld {
            [PrimaryKey] public string OldNorse { get; set; } = "";
            public string English { get; set; } = "";
            [PrimaryKey] public int? EddaMentions { get; set; }
        }

        // Test Scenario: Primary Key Cannot Be Deduced (✗illegal✗)
        public class FederalLaw {
            public string CommonName { get; set; } = "";
            public string? ShortName { get; set; }
            public DateTime Enacted { get; set; }
            public float StatuteIdentifier { get; set; }
            public string IntroducedBy { get; set; } = "";
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
    }

    internal static class PrimaryKeyNaming {
        // Test Scenario: Named Primary Key (✓named✓)
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

        // Test Scenario: Duplicate Unnamed Candidate Keys (✓de-duplicated✓)
        public class Pigment {
            [PrimaryKey] public string Name { get; set; } = "";
            public string DominantColor { get; set; } = "";
            [Unique, Unique] public string ChemicalFormula { get; set; } = "";
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

        // Test Scenario: Candidate Key Named null (✗illegal✗)
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
    }

    internal static class DataConverters {
        // Test Scenario: Data Conversion does not Change Field's Type (✓applied✓)
        public class Cenote {
            [PrimaryKey] public string Name { get; set; } = "";
            public float MaxDepth { get; set; }
            [DataConverter(typeof(Invert))] public bool IsKarst { get; set; }
            public decimal Latitude { get; set; }
            public decimal Longitude { get; set; }
        }

        // Test Scenario: Data Conversion Changes Field's Type (✓applied✓)
        public class Comet {
            [PrimaryKey] public Guid AstronomicalIdentifier { get; set; }
            public double Aphelion { get; set; }
            [DataConverter(typeof(RoundDown))] public double Perihelion { get; set; }
            [DataConverter(typeof(RoundDown))] public double Eccentricity { get; set; }
            public ulong MassKg { get; set; }
            public double Albedo { get; set; }
            public float OrbitalPeriod { get; set; }
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
            [DataConverter(typeof(Nullify<string>))] public string Ability1 { get; set; } = "";
            [DataConverter(typeof(Nullify<string>))] public string Ability2 { get; set; } = "";
            [DataConverter(typeof(Nullify<string>))] public string Ability3 { get; set; } = "";
            [DataConverter(typeof(Nullify<uint>))] public uint SerialNumber { get; set; }
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
                public enum Status { AtLarge, Incarerated, Apprehended, InTrial }

                [PrimaryKey] public string AlterEgo { get; set; } = "";
                public string? Identity { get; set; }
                [Check.IsNegative] public Status CurrentStatus { get; set; }
                public uint KnownVictims { get; set; }
                public bool FBIMostWanted { get; set; }
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

            // Test Scenario: Default Value Does Not Satisfy Constraint (✗contradiction✗)
            public class SuperPAC {
                [PrimaryKey] public Guid RegistrationID { get; set; }
                [Check.IsNegative, Default(0L)] public long TotalRaised { get; set; }
                public ushort NumContributors { get; set; }
                public DateTime Established { get; set; }
                public bool IsProDemocrat { get; set; }
            }
        }

        public static class IsNonZero {
            // Test Scenario: Applied to Numeric Fields (✓constrained✓)
            public class RegularPolygon {
                [PrimaryKey, Check.IsNonZero] public ushort NumEdges { get; set; }
                [Check.IsNonZero] public sbyte NumVertices { get; set; }
                [Check.IsNonZero] public double InternalAngle { get; set; }
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

            // Test Scenario: Default Value Does Not Satisfy Constraint (✗contradiction✗)
            public class Pulley {
                [PrimaryKey] public float RopeLength { get; set; }
                [PrimaryKey, Check.IsNonZero, Default(0.0)] public double RopeTension { get; set; }
                [PrimaryKey] public byte NumWheels { get; set; }
                [PrimaryKey] public bool Moveable { get; set; }
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
                [Check.IsGreaterThan("NEVER")] public DateTime FirstPublished { get; set; }
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

            // Test Scenario: Default Value Does Not Satisfy Constraint (✗contradiction✗)
            public class ParkingGarage {
                [PrimaryKey] public Guid GarageID { get; set; }
                public ushort ParkingSpaces { get; set; }
                public ushort Levels { get; set; }
                [Check.IsLessThan(10.00), Default(15.00)] public double CostPerHour { get; set; }
                public bool AllowsOvernight { get; set; }
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

            // Test Scenario: Default Value Does Not Satisfy Constraint (✗contradiction✗)
            public class Camera {
                [PrimaryKey] public string Model { get; set; } = "";
                public double Aperture { get; set; }
                [Check.IsGreaterOrEqualTo(1.3f), Default(0.00001f)] public float ShutterSpeed { get; set; }
                public double LensRadius { get; set; }
                public bool HasFlash { get; set; }
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

            // Test Scenario: Default Value Does Not Satisfy Constraint (✗contradiction✗)
            public class BowlingFrame {
                [PrimaryKey] public Guid FrameID { get; set; }
                public short Round { get; set; }
                public byte FirstThrowPins { get; set; }
                [Check.IsLessOrEqualTo((byte)10), Default((byte)23)] public byte? SecondThrowPins { get; set; }
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
            public class SecurityBug {
                [PrimaryKey] public string CVEIdentifier { get; set; } = "";
                public string LibraryAffected { get; set; } = "";
                [Check.IsNot(null!)] public string? VersionPatched { get; set; }
                public DateTime Discovered { get; set; }
                public DateTime Patched { get; set; }
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
                [DataConverter(typeof(ToInt<bool>)), Check.IsNot(7)] public bool Destroyed { get; set; }
                public DateTime Forged { get; set; }
                public string CentralStone { get; set; } = "";
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
                byte N3 { get; set; }
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

            // Test Scenario: Default Value Does Not Satisfy Constraint (✗contradiction✗)
            public class RestStop {
                [PrimaryKey] public string Highway { get; set; } = "";
                [PrimaryKey, Check.IsNot(153U), Default(153U)] public uint Exit { get; set; }
                public bool HasPicnicArea { get; set; }
                public bool IsOasis { get; set; }
                public bool TruckCompatible { get; set; }
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

            // Test Scenario: Default Value Does Not Satisfy Constraint (✗contradiction✗)
            public class AztecGod {
                [PrimaryKey] public string Name { get; set; } = "";
                public string? MayanEquivalent { get; set; }
                [Check.IsNonEmpty, Default("")] public string? Festival { get; set; }
                public string Domain { get; set; } = "";
                public uint CodexAppearances { get; set; }
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

            // Test Scenario: Default Value Does Not Satisfy Constraint (✗contradiction✗)
            public class MaskedSinger {
                [PrimaryKey] public uint Season { get; set; }
                [PrimaryKey, Check.LengthIsAtLeast(289), Default("Pelican")] public string Costume { get; set; } = "";
                public string Identity { get; set; } = "";
                public byte SongsPerformed { get; set; }
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

            // Test Scenario: Default Value Does Not Satisfy Constraint (✗contradiction✗)
            public class Obi {
                [PrimaryKey] public string MartialArt { get; set; } = "";
                [Check.LengthIsAtMost(3), Default("White")] public string Color { get; set; } = "";
                [PrimaryKey] public ushort Level { get; set; }
                public bool IsStarter { get; set; }
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

            // Test Scenario: Default Value Does Not Satisfy Constraint (✗contradiction✗)
            public class PeanutButter {
                [PrimaryKey] public Guid ProductID { get; set; }
                public ushort Calories { get; set; }
                public ushort GramsFat { get; set; }
                [Check.LengthIsBetween(4, 8), Default("Smucker's")] public string Brand { get; set; } = "";
                public bool IsSmooth { get; set; }
                public bool IsNatural { get; set; }
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
                [Flags] public enum Manufacturing { Amateur, Professional, Kit, Factory }

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

            // Test Scenario: Default Value Does Not Satisfy Constraint (✗contradiction✗)
            public class Guillotine {
                [PrimaryKey] public Guid ItemID { get; set; }
                public ulong Decapitations { get; set; }
                [Check.IsOneOf(30U, 60U, 90U, 120U), Default(113U)] public uint Height { get; set; }
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
                [Check.IsNot(false)] public bool ForWomen { get; set; }
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

            // Test Scenario: Default Value Does Not Satisfy Constraint (✗contradiction✗)
            public class Pie {
                [PrimaryKey] public int ID { get; set; }
                [Check.IsNotOneOf("Rhubarb", "Anise"), Default("Anise")] public string Flavor { get; set; } = "";
                public ushort Calories { get; set; }
                public float Diameter { get; set; }
                public bool IsSweet { get; set; }
                public string CrustIngredient { get; set; } = "";
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

        // Test Scenario: Constraint Generator is not an `IConstraintGenerator` (✗illegal✗)
        public class Patreon {
            [PrimaryKey] public string URL { get; set; } = "";
            public string Creator { get; set; } = "";
            public decimal Tier1 { get; set; }
            public decimal Tier2 { get; set; }
            [Check(typeof(NonSerializedAttribute))] public decimal Tier3 { get; set; }
        }

        // Test Scenario: Constraint Generator Cannot Be Constructed (✗illegal✗)
        public class Transistor {
            [PrimaryKey] public Guid ID { get; set; }
            public string Model { get; set; } = "";
            [Check(typeof(PrivateCheck), "Dopant", 4)] public string? Dopant { get; set; }
            public float Transconductance { get; set; }
            public int OperatingTemperature { get; set; }
        }

        // Test Scenario: Constraint Generator Throws Error upon Construction (✗propagated✗)
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
            [PrimaryKey, Check.IsGreaterThan("UTF-7"), Check.IsLessOrEqualTo("ASCII")] public string CodeSet { get; set; } = "";
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
    }
}
