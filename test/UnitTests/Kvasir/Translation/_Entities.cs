using Kvasir.Annotations;
using System;

namespace UT.Kvasir.Translation {
    internal static partial class TestComponents {
        /////// --- Scalar DataTypes

            // Test Scenario: Non-Nullable Scalars
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

            // Test Scenario: Nullable Scalars
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

        /////// --- Various Shapes of Entity Type

            // Test Scenario: Record Class Entity Type
            public record class Color {
                [PrimaryKey] public byte Red { get; set; }
                [PrimaryKey] public byte Green { get; set; }
                [PrimaryKey] public byte Blue { get; set; }
            }

            // Test Scenario: Partial Entity Type
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

            // Test Scenario: Non-Public Entity Type
            private class GitCommit {
                [PrimaryKey] public string Hash { get; set; } = "";
                public string Author { get; set; } = "";
                public string Message { get; set; } = "";
                public DateTime Timestamp { get; set; }
            }

            // Test Scenario: Struct Entity Type {error}
            public struct Carbohydrate {
                [PrimaryKey] public uint Carbon { get; set; }
                [PrimaryKey] public uint Hydrogen { get; set; }
                [PrimaryKey] public uint Oxygen { get; set; }
            }

            // Test Scenario: Record Struct Entity Type {error}
            public record struct AminoAcid {
                [PrimaryKey] public char Symbol { get; set; }
                public uint Carbon { get; set; }
                public uint Hydrogen { get; set; }
                public uint Nitrogen { get; set; }
                public uint Oxygen { get; set; }
                public uint Sulfur { get; set; }
            }

            // Test Scenario: Abstract Entity Type {error}
            public abstract class SuperBowl {
                [PrimaryKey] public ushort Year { get; set; }
                public string HomeTeam { get; set; } = "";
                public string AwayTeam { get; set; } = "";
                public byte HomeScore { get; set; }
                public byte AwayScore { get; set; }
                public bool HomeWins { get; set; }
            }

            // Test Scenario: Generic Entity Type {error}
            public class Speedometer<TUnit> {
                [PrimaryKey] public long MinSpeed { get; set; }
                [PrimaryKey] public long MaxSpeed { get; set; }
                public string Brand { get; set; } = "";
            }

            // Test Scenario: Interface Entity Type {error}
            public interface ILiquor {
                [PrimaryKey] public string Name { get; set; }
                public ushort Proof { get; set; }
                public float AlcoholByVolume { get; set; }
            }

            // Test Scenario: Enumeration Entity Type {error}
            public enum Season {
                Winter,
                Spring,
                Summer,
                Fall,
                Autumn = Fall,
            }

            // Test Scenario: Entity Type with Zero Properties {error}
            public class Nothing {}

            // Test Scenario: Entity type with Exactly One Property {error}
            public class Integer {
                [PrimaryKey] public int Value { get; set; }
            }

        /////// --- Property Inclusion/Exclusion

            // Test Scenario: Non-Public Property is Excluded
            public class Animal {
                private string Kingdom { get; set; } = "";
                protected string Phylum { get; set; } = "";
                internal string Class { get; set; } = "";
                protected internal string Order { get; set; } = "";
                private protected string Family { get; set; } = "";
                [PrimaryKey] public string Genus { get; set; } = "";
                [PrimaryKey] public string Species { get; set; } = "";
            }

            // Test Scenario: Non-Readable Property is Excluded
            public class ScrabbleTile {
                [PrimaryKey] public char Letter { get; set; }
                public byte Value { get; set; }
                public ushort NumAvailable { set {} }
            }

            // Test Scenario: Property with Non-Public Accessor is Excluded
            public class ChemicalElement {
                [PrimaryKey] public string Symbol { get; set; } = "";
                public byte AtomicNumber { private get; set; }
                public decimal AtomicWeight { protected get; set; }
                public string Name { internal get; set; } = "";
                public ushort? MeltingPoint { protected internal get; set; }
                public ushort? BoilingPoint { private protected get; set; }
                public sbyte NumAllotropes { get; set; }
            }

            // Test Scenario: Static Property is Excluded
            public class Circle {
                [PrimaryKey] public int CenterX { get; set; }
                [PrimaryKey] public int CenterY { get; set; }
                [PrimaryKey] public ulong Radius { get; set; }
                public static double PI { get; }
            }

            // Test Scenario: Indexer is Excluded
            public class BattingOrder {
                [PrimaryKey] public Guid GameID { get; set; }
                public string Team { get; set; } = "";
                public string this[int position] { get { return ""; } set {} }
            }

            // Test Scenario: [CodeOnly] Property that would Otherwise be Included
            public class QuadraticEquation {
                [CodeOnly] public string Expression { get; set; } = "";
                [PrimaryKey] public long QuadraticCoefficient { get; set; }
                [PrimaryKey] public long LinearCoefficient { get; set; }
                [PrimaryKey] public long Constant { get; set; }
            }

            // Test Scenario: First-Definition Virtual Property
            public class GreekGod {
                [PrimaryKey] public string Name { get; set; } = "";
                public string? RomanEquivalent { get; set; }
                public virtual uint NumChildren { get; set; }
            }

            // Test Scenario: Property Declared by an Interface is Excluded
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

            // Test Scenario: Property Inherited from a Base Class is Excluded
            public abstract class Date {
                public byte Day { get; set; }
                public byte Month { get; set; }
                public short Year { get; set; }
            }
            public class Holiday : Date {
                [PrimaryKey] public DateTime Date { get; set; }
                [PrimaryKey] public string Name { get; set; } = "";
            }

            // Test Scenario: Overriding Property is Excluded
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

            // Test Scenario: Hiding Property
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

            // Test Scenario: Indexer Annotated as [IncludeInModel] {error}
            public class Language {
                public string Exonym { get; set; } = "";
                [PrimaryKey] public string Endonym { get; set; } = "";
                public ulong Speakers { get; set; }
                public string ISOCode { get; set; } = "";
                public ushort Letters { get; set; }
                [IncludeInModel] public string this[string word] { get { return ""; } set {} }
            }

            // Test Scenario: Non-Readable Property Annotated as [IncludeInModel] {error}
            public class HebrewPrayer {
                [PrimaryKey] public string Name { get; set; } = "";
                [IncludeInModel] public bool OnShabbat { set {} }
                public string Text { get; set; } = "";
            }

            // Test Scenario: Static Property Annotated as [IncludeInModel]
            public class ChessPiece {
                [PrimaryKey] public string Name { get; set; } = "";
                public char Icon { get; set; }
                public byte Value { get; set; }
                [IncludeInModel] public static string FIDE { get; set; } = "FIDE";
            }

            // Test Scenario: Non-Public Property Annotated as [IncludeInModel]
            public class Song {
                [PrimaryKey] public string Title { get; set; } = "";
                [PrimaryKey] public string Artist { get; set; } = "";
                [IncludeInModel] private string? Album { get; set; } = "";
                [IncludeInModel] protected ushort Length { get; set; }
                [IncludeInModel] internal ushort ReleaseYear { get; set; }
                [IncludeInModel] protected internal double Rating { get; set; }
                [IncludeInModel] private protected byte Grammys { get; set; }
            }

            // Test Scenario: Property w/ Non-Public Accessor Annotated as [IncludeInModel]
            public class Country {
                [PrimaryKey] public string Exonym { get; set; } = "";
                [IncludeInModel] public string Endonym { private get; set; } = "";
                [IncludeInModel] public DateTime IndependenceDay { protected get; set; }
                [IncludeInModel] public ulong Population { internal get; set; }
                [IncludeInModel] public ulong LandArea { protected internal get; set; }
                [IncludeInModel] public ulong Coastline { private protected get; set; }
            }

            // Test Scenario: Property Declared by an Interface Annotated as [IncludeInModel] by Implementation {error}
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

            // Test Scenario: Overriding Property Annotated as [IncludeInModel] {error}
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

            // Test Scenario: Property Declared by an Interface Annotated as [CodeOnly] by Implementation {error}
            public interface IWebProtocol {
                int RFC { get; set; }
            }
            public class IPAddress : IWebProtocol {
                [PrimaryKey] public ulong Value { get; set; }
                [PrimaryKey] public ulong Version { get; set; }
                [CodeOnly] public int RFC { get; set; }
            }

            // Test Scenario: Overriding Property Annotated as [CodeOnly] {error}
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

            // Test Scenario: Non-Readable Property Annotated as [CodeOnly] {error}
            public class CourtCase {
                [PrimaryKey] public ushort Volume { get; set; }
                [PrimaryKey] public uint CasePage { get; set; }
                [CodeOnly] public ulong Year { set {} }
                public string Plaintiff { get; set; } = "";
                public string Defendant { get; set; } = "";
            }

            // Test Scenario: Non-Public Property Annotated as [CodeOnly] {error}
            public class Lake {
                [PrimaryKey] public decimal Latitude { get; set; }
                [PrimaryKey] public decimal Longitude { get; set; }
                public ulong SurfaceArea { get; set; }
                [CodeOnly] private ulong Depth { get; set; }
            }

            // Test Scenario: Property w/ Non-Public Accessor Annotated as [CodeOnly] {error}
            public class Mountain {
                [PrimaryKey] public string Name { get; set; } = "";
                [CodeOnly] public long Height { protected get; set; }
                public decimal Latitude { get; set; }
                public decimal Longitude { get; set; }
                public bool SevenSummits { get; set; }
            }

            // Test Scenario: Static Property Annotated as [CodeOnly] {error}
            public class Tossup {
                [PrimaryKey] public uint ID { get; set; }
                public string LocationCode { get; set; } = "";
                public string SubjectCode { get; set; } = "";
                public string TimeCode { get; set; } = "";
                public string Body { get; set; } = "";
                [CodeOnly] public static byte MinLength { get; set; }
                public static byte MaxLength { get; set; }
            }

            // Test Scenario: Indexer Annotated as [CodeOnly] {error}
            public class University {
                [PrimaryKey] public string System { get; set; } = "";
                [PrimaryKey] public string Campus { get; set; } = "";
                public ulong UndergradEnrollment { get; set; }
                public ulong GraduateEnrollment { get; set; }
                public ulong Endowment { get; set; }
                [CodeOnly] public int this[int index] { get { return -1; } set {} }
            }

            // Test Scenario: Property Annotated as Both [CodeOnly] and [IncludeInModel] {error}
            public class CreditCard {
                [PrimaryKey] public string Number { get; set; } = "";
                public DateTime Expiration { get; set; }
                [IncludeInModel, CodeOnly] public byte CVV { get; set; }
            }

            // Test Scenario: Public, Instance Property Annotated as [IncludeInModel] {error}
            public class Haiku {
                [PrimaryKey] public string Title { get; set; } = "";
                public string Author { get; set; } = "";
                public string Line1 { get; set; } = "";
                [IncludeInModel] public string Line2 { get; set; } = "";
                public string Line3 { get; set; } = "";
            }

        /////// --- Invalid Property Types

            // Test Scenario: Property with a Delegate CLR Type {error}
            public delegate void HurricaneAction();
            public class Hurricane {
                [PrimaryKey] public short Year { get; set; }
                [PrimaryKey] public byte Number { get; set; }
                public ulong TopWindSpeed { get; set; }
                public ulong Damage { get; set; }
                public uint Casualties { get; set; }
                public HurricaneAction Form { get; set; } = () => {};
            }

            // Test Scenario: Property with a Dynamic CLR Type {error}
            public class MonopolyProperty {
                [PrimaryKey] public string Name { get; set; } = "";
                public byte Rent { get; set; }
                public byte Mortgage { get; set; }
                public dynamic HotelCost { get; set; } = 100.0;
            }

            // Test Scenario: Property with CLR Type of object {error}
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

            // Test Scenario: Property with a CLR Type of System.Enum {error}
            public class Enumeration {
                [PrimaryKey] public string Namespace { get; set; } = "";
                [PrimaryKey] public string Typename { get; set; } = "";
                public Enum? ZeroValue { get; set; }
                public uint EnumeratorCount { get; set; }
            }

            // Test Scenario: Property with a CLR Type of System.ValueType {error}
            public class YouTubeVideo {
                [PrimaryKey] public string URL { get; set; } = "";
                public uint Length { get; set; }
                public ulong Likes { get; set; }
                public string Channel { get; set; } = "";
                public ValueType CommentCount { get; set; } = 0;
            }

            // Test Scenario: Property with a CLR Type that is a Standard Library Class {error}
            public class Coin {
                [PrimaryKey] public byte Value { get; set; }
                public float Diameter { get; set; }
                [PrimaryKey] public bool InCirculation { get; set; }
                public Exception CounterfeitResult { get; set; } = new ApplicationException("COUNTERFEIT!");
            }

            // Test Scenario: Property with a CLR Type that is a User-Defined Interface {error}
            public interface IArtist {}
            public class Painting {
                [PrimaryKey] public Guid NGAID { get; set; }
                public decimal Height { get; set; }
                public decimal Width { get; set; }
                public IArtist? Artist { get; set; }
                public short Year { get; set; }
            }

            // Test Scenario: Property with a Closed Generic User-Defined Class {error}
            public class MessageCount<T> {}
            public class SlackChannel {
                [PrimaryKey] public Guid ID { get; set; }
                public string ChannelName { get; set; } = "";
                public long Members { get; set; }
                public bool IsPrivate { get; set; }
                public MessageCount<short>? NumMessages { get; set; }
            }

            // Test Scenario: Property with an Abstract User-Defined Class {error}
            public abstract class Flower {}
            public class BotanicalGarden {
                [PrimaryKey] public int ID { get; set; }
                public DateTime Opening { get; set; }
                public ulong VisitorsPerYear { get; set; }
                public Flower? OfficialFlower { get; set; }
            }

            // Test Scenario: [CodeOnly] on Properties with Otherwise Invalid CLR Types
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

        /////// --- Nullability Annotations

            // Test Scenario: Non-Nullable Scalar Annotated as [Nullable]
            public class Timestamp {
                [PrimaryKey] public ulong UnixSinceEpoch { get; set; }
                public ushort Hour { get; set; }
                public ushort Minute { get; set; }
                public ushort Second { get; set; }
                [Nullable] public ushort Millisecond { get; set; }
                [Nullable] public ushort Microsecond { get; set; }
                [Nullable] public ushort Nanosecond { get; set; }
            }

            // Test Scenario: Nullable Scalar Annotated as [NonNullable]
            public class Bone {
                [PrimaryKey] public uint TA2 { get; set; }
                [NonNullable] public string? Name { get; set; }
                public string? LatinName { get; set; } = "";
                public string MeSH { get; set; } = "";
            }

            // Test Scenario: Redundant [Nullable] Annotation {error}
            public class CivMilitaryUnit {
                [PrimaryKey] public string Identifier { get; set; } = "";
                [Nullable] public string? Promotion { get; set; } = "";
                public byte MeleeStrength { get; set; }
                public byte? RangedStrength { get; set; }
                public bool IsUnique { get; set; }
            }

            // Test Scenario: Redundant [NonNullable] Annotation {error}
            public class Patent {
                [PrimaryKey] public ulong DocumentID { get; set; }
                [NonNullable] public DateTime PublicationDate { get; set; }
                public string? Description { get; set; }
                public ulong ApplicationNumber { get; set; }
            }

            // Test Scenario: Both [Nullable] & [NonNullable] Annotation {error}
            public class RetailProduct {
                [PrimaryKey] public Guid ID { get; set; }
                public string Name { get; set; } = "";
                public string Description { get; set; } = "";
                public decimal BasePrice { get; set; }
                [Nullable, NonNullable] public decimal SalePrice { get; set; }
                public ulong StockCount { get; set; }
                public uint CategoryID { get; set; }
            }

        /////// --- Table Renaming

            // Test Scenario: Entity Type Annotated with [Table]
            [Table("DeckOfCards")] public class PlayingCard {
                [PrimaryKey] public byte Suit { get; set; }
                [PrimaryKey] public byte Value { get; set; }
            }

            // Test Scenario: Entity Type Annotated with [ExcludeNamespaceFromName]
            [ExcludeNamespaceFromName] public class Pokemon {
                [PrimaryKey] public ushort PokedexNumber { get; set; }
                public string PrimaryType { get; set; } = "";
                public string? SecondaryType { get; set; }
                public string Name { get; set; } = "";
                public string JapaneseName { get; set; } = "";
                public byte HP { get; set; }
            }

            // Test Scenario: Duplicated Primary [Table] Names {error}
            [Table("Miscellaneous")] public class Flight {
                [PrimaryKey] public Guid ID { get; set; }
                public string Airline { get; set; } = "";
                public DateTime Departure { get; set; }
                public DateTime Arrival { get; set; }
                public string FromAirport { get; set; } = "";
                public string ToAirport { get; set; } = "";
                public byte Capacity { get; set; }
            }
            [Table("Miscellaneous")] public class Battle {
                [PrimaryKey] public string Name { get; set; } = "";
                [PrimaryKey] public string War { get; set; } = "";
                public DateTime StartDate { get; set; }
                public DateTime EndDate { get; set; }
                public string WinningCommander { get; set; } = "";
                public string LosingCommander { get; set; } = "";
                public ulong Casualties { get; set; }
            }

            // [Table] Annotation Given Type's Namespace-Qualified Name + "Table" {error}
            [Table("UT.Kvasir.Translation.TestComponents+BookmarkTable")] public class Bookmark {
                [PrimaryKey] public string URL { get; set; } = "";
                public bool IsFavorite { get; set; }
                public string Icon { get; set; } = "";
                public bool IsOnDesktop { get; set; }
            }

            // Test Scenario: Invalid [Table] Name {error}
            [Table("")] public class LogIn {
                [PrimaryKey] public string Username { get; set; } = "";
                public string Password { get; set; } = "";
            }

            // Test Scenario: Entity Type Annotated with both [Table] ad [ExcludeNamespaceFromName] {error}
            [Table("SomeTable"), ExcludeNamespaceFromName] public class Encryption {
                [PrimaryKey] public string Scheme { get; set; } = "";
                public ulong PublicKey { get; set; }
                public ulong PrivateKey { get; set; }
            }

        /////// --- Field Naming
        
            // Test Scenario: Fields with Oddly-Shaped Names
            public class Surah {
                [PrimaryKey] public string _EnglishName { get; set; } = "";
                public string __ArabicName { get; set; } = "";
                public decimal juz_start { get; set; }
                public decimal juzEnd { get; set; }
            }
        
            // Test Scenario: Rename Field to Brand New Name
            public class River {
                [PrimaryKey] public string Name { get; set; } = "";
                [Name("SourceElevation")] public ushort Ahuiehknaafuyur { get; set; }
                [Name("Length")] public ushort OEperaehrugyUIWJKuygajk { get; set; }
                public decimal MouthLatitude { get; set; }
                public decimal MouthLongitude { get; set; }
            }

            // Test Scenario: Swap Names of Fields
            public class Episode {
                [PrimaryKey, Name("Number")] public byte Season { get; set; }
                [PrimaryKey, Name("Season")] public short Number { get; set; }
                public float Length { get; set; }
                public int? Part { get; set; }
                public string Title { get; set; } = "";
            }

            // Test Scenario: Duplicated Field Names {error}
            public class Ticket2RideRoute {
                [PrimaryKey, Name("Destination")] public string City1 { get; set; } = "";
                [PrimaryKey, Name("Destination")] public string City2 { get; set; } = "";
                public byte Points { get; set; }
            }

            // Test Scenario: Single Property with Multiple [Name] Annotations {error}
            public class BankAccount {
                public string Bank { get; set; } = "";
                [PrimaryKey] public string AccountNumber { get; set; } = "";
                [Name("Route"), Name("RoutingNmbr")] public ulong RoutingNumber { get; set; }
            }

            // Test Scenario: [Name] Annotation Given Property's Name {error}
            public class Opera {
                [PrimaryKey] public Guid ID { get; set; }
                public string Composer { get; set; } = "";
                [Name("PremiereDate")] public DateTime PremiereDate { get; set; }
                public uint Length { get; set; }
            }

            // Test Scenario: Invalid Field [Name] {error}
            public class Volcano {
                [PrimaryKey] public string Name { get; set; } = "";
                public ulong Height { get; set; }
                public DateTime LastEruption { get; set; }
                [Name("")] public bool IsActive { get; set; }
            }

            // Test Scenario: <Path> Provided for [Name] on Scalar Property {error}
            public class Legume {
                [PrimaryKey] public Guid LegumeGuid { get; set; }
                public string Name { get; set; } = "";
                [Name("EnergyInKJ", Path = "---")] public decimal Energy { get; set; }
                public double Carbohydrates { get; set; }
                public double Fat { get; set; }
                public double Protein { get; set; }
            }

        /////// --- Default Values

            // Test Scenario: Valid Non-Null Default Values for Scalar Fields (excluding DateTime & Guid)
            public class BloodType {
                [PrimaryKey, Default("O")] public string ABO { get; set; } = "";
                [PrimaryKey, Default(true)] public bool RHPositive { get; set; }
                [Default(0.5f)] public float ApproxPrevalence { get; set; }
                [Default(1)] public int NumSubgroups { get; set; }
                public decimal AnnualDonationsL { get; set; }
            }

            // Test Scenario: Valid Non-Null Default Value for DateTime Field
            public class Umpire {
                [PrimaryKey] public Guid UniqueUmpireNumber { get; set; }
                public ushort UniformNumber { get; set; }
                public string Name { get; set; } = "";
                [Default("1970-01-01")] public DateTime Debut { get; set; }
                public uint Ejections { get; set; }
            }

            // Test Scenario: Valid Non-Null Default Value for Guid Field
            public class Saint {
                [Default("81a130d2-502f-4cf1-a376-63edeb000e9f")] public Guid SainthoodIdentifier { get; set; }
                [PrimaryKey] public string Name { get; set; } = "";
                public DateTime CanonizationDate { get; set; }
                public byte FeastMonth { get; set; }
                public byte FeastDay { get; set; }
            }

            // Test Scenario: Null Default Value for Nullable Field
            public class Pepper {
                [PrimaryKey] public string Genus { get; set; } = "";
                [PrimaryKey] public string Species { get; set; } = "";
                [Default(null)] public string? CommonName { get; set; }
                public ulong ScovilleRating { get; set; }
            }

            // Test Scenario: Invalid and Unconvertible Non-Null Default Value {error}
            public class Battleship {
                [PrimaryKey] public string CallSign { get; set; } = "";
                public DateTime? Launched { get; set; }
                [Default("100 feet")] public ushort Length { get; set; }
                public ushort TopSpeedMPH { get; set; }
                public byte GunCount { get; set; }
                public Guid ShipyardIdentifier { get; set; }
            }

            // Test Scenario: Invalid but Convertible Non-Null Default Value {error}
            public class County {
                [PrimaryKey] public ulong GNIS_ID { get; set; }
                public string Name { get; set; } = "";
                public string State { get; set; } = "";
                [Default(5000000)] public ulong Population { get; set; }
                public ulong Area { get; set; }
                public DateTime Incorporation { get; set; }
            }

            // Test Scenario: Single-Element Array of Correct Type for Default Value {error}
            public class BilliardBall {
                [PrimaryKey] public string Color { get; set; } = "";
                [PrimaryKey, Default(new int[] { 7 })] public int Number { get; set; }
                public bool IsSolid { get; set; }
            }

            // Test Scenario: Invalid Non-Null Default Value for DateTime Field b/c of Formatting {error}
            public class Tournament {
                [PrimaryKey] public string Name { get; set; } = "";
                [Default("20030714")] public DateTime Kickoff { get; set; }
                public DateTime? Conclusion { get; set; }
                public int Participants { get; set; }
                public string Number1 { get; set; } = "";
            }

            // Test Scenario: Invalid Non-Null Default Value for DateTime Field b/c of Range {error}
            public class Sculpture {
                [PrimaryKey, Default("1344-18-18")] public DateTime CreationDate { get; set; }
                public string Sculptor { get; set; } = "";
                public ushort HeightFt { get; set; }
                public ushort WeightLbs { get; set; }
                public bool InOnePiece { get; set; }
            }

            // Test Scenario: Invalid Non-Null Default Value for DateTime Field b/c of Type {error}
            public class RomanEmperor {
                [PrimaryKey] public int ChronologicalIndex { get; set; }
                public string LongName { get; set; } = "";
                public string ShortName { get; set; } = "";
                public DateTime ReignStart { get; set; }
                [Default(true)] public DateTime ReignEnd { get; set; }
            }

            // Test Scenario: Invalid Non-Null Default Value for Guid Field b/c of Structure {error}
            public class Gene {
                [PrimaryKey, Default("ee98f44827b248a2bb9fc5ef342e7ab2!!!")] public Guid UUID { get; set; }
                public string Identifier { get; set; } = "";
                public long HumanEntrez { get; set; }
                public string UCSCLocation { get; set; } = "";
            }

            // Test Scenario: Invalid Non-Null Default Value for Guid Field b/c of Type {error}
            public class HogwartsHouse {
                public string Name { get; set; } = "";
                public ushort FirstPageMentioned { get; set; }
                public long TotalMentions { get; set; }
                [Default('^')] public Guid TermIndex { get; set; }
            }

            // Test Scenario: Null Default Value for Non-Nullable Field {error}
            public class RadioStation {
                [PrimaryKey] public bool IsFM { get; set; }
                [PrimaryKey] public decimal StationNumber { get; set; }
                [Default(null)] public string CallSign { get; set; } = "";
            }

            // Test Scenario: Valid Default Value on Field with Data Converter
            public class CrosswordClue {
                [PrimaryKey] public Guid PuzzleID { get; set; }
                [DataConverter(typeof(CharToInt)), Default('A')] public char AcrossOrDown { get; set; }
                public ushort Number { get; set; }
                public byte NumLetters { get; set; }
                public string ClueText { get; set; } = "";
            }

            // Test Scenario: Valid Default Value for Data-Coverted Target Type, Not Original Type {error}
            public class Coupon {
                [PrimaryKey] public Guid Barcode { get; set; }
                public string? Code { get; set; }
                [DataConverter(typeof(BoolToInt)), Default(0)] public bool IsBOGO { get; set; }
                public double? DiscountPercentage { get; set; }
                public float? MinimumPurchase { get; set; }
                public DateTime? ExpirationDate { get; set; }
            }

            // Test Scenario: Multiple [Default] Annotations on a Single Field {error}
            public class SkeeBall {
                public int MachineID { get; set; }
                [Default(1), Default(0)] public ushort L1Value { get; set; }
                public ushort L2Value { get; set; }
                public ushort L3Value { get; set; }
                public ushort L4Value { get; set; }
            }

            // Test Scenario: <Path> Provided for [Default] on Scalar Property {error}
            public class NativeAmericanTribe {
                [PrimaryKey] public string Endonym { get; set; } = "";
                [Default(null, Path = "---")] public string? Exonym { get; set; }
                public ulong Population { get; set; }
                public DateTime Established { get; set; }
                public string GoverningBody { get; set; } = "";
                public ulong Area { get; set; }
            }

        /////// --- Column Ordering

            // Test Scenario: Apply [Column] Indices to All Fields
            public class Fraction {
                [PrimaryKey, Column(2)] public decimal Numerator { get; set; }
                [Column(1)] public decimal Denominator { get; set; }
                [Column(0)] public bool IsNegative { get; set; }
            }

            // Test Scenario: Apply [Column] Indices to Some Fields
            public class Parashah {
                [PrimaryKey] public string Book { get; set; } = "";
                [PrimaryKey] public ushort StartChapter { get; set; }
                [PrimaryKey] public ushort StartVerse { get; set; }
                [Column(2)] public ushort EndChapter { get; set; }
                [Column(3)] public ushort EndVerse { get; set; }
            }

            // Test Scenario: Duplicate [Column] Indices on Fields {error}
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

            // Test Scenario: Gaps in [Column] Indices {error}
            public class PhoneNumber {
                [PrimaryKey, Column(1)] public byte CountryCode { get; set; }
                [PrimaryKey] public ushort AreaCode { get; set; }
                [PrimaryKey, Column(14)] public ushort Number { get; set; }
            }

            // Test Scenario: Negative [Column] Index {error}
            public class NationalPark {
                [PrimaryKey] public string Name { get; set; } = "";
                public string State { get; set; } = "";
                [Column(-196)] public DateTime Established { get; set; }
                public uint Area { get; set; }
                public ulong AnnualVisitors { get; set; }
            }

        /////// --- Candidate Keys
        
            // Test Scenario: Multiple Unnamed Candidate Keys
            public class Inmate {
                [PrimaryKey] public Guid PrisonerNumber { get; set; }
                [Unique] public int SSN { get; set; }
                [Unique] public string FullName { get; set; } = "";
            }

            // Test Scenario: Named Candidate Key
            public class BowlGame {
                [PrimaryKey] public string Name { get; set; } = "";
                [Unique("Sponsorship")] public string PrimarySponsor { get; set; } = "";
                [Unique("Sponsorship")] public string? SecondarySponsor { get; set; } = "";
                public DateTime Inception { get; set; }
                public DateTime NextScheduled { get; set; }
            }
        
            // Test Scenario: Single Field in Multiple Candidate Keys
            public class KingOfEngland {
                [PrimaryKey] public DateTime ReignStart { get; set; }
                [PrimaryKey] public DateTime ReignEnd { get; set; }
                [Unique("Uno"), Unique("Another")] public string RegnalName { get; set; } = "";
                [Unique("Uno"), Unique("Third")] public byte RegnalNumber { get; set; }
                [Unique("Another"), Unique("Third")] public string RoyalHouse { get; set; } = "";
            }

            // Test Scenario: Duplicate Named Candidate Keys {error}
            public class Check {
                [PrimaryKey] public Guid CID { get; set; }
                public string Signatory { get; set; } = "";
                public decimal Amount { get; set; }
                public ulong RoutingNumber { get; set; }
                [Unique("N1"), Unique("N2"), Unique("N3")] public byte CheckNumber { get; set; }
            }

            // Test Scenario: Duplicate Anonymous Candidate Keys {error}
            public class Pigment {
                [PrimaryKey] public string Name { get; set; } = "";
                public string DominantColor { get; set; } = "";
                [Unique, Unique] public string ChemicalFormula { get; set; } = "";
            }

            // Test Scenario: Field Placed in Candidate Key Multiple Times {error}
            public class Desert {
                [PrimaryKey] public string Name { get; set; } = "";
                [Unique("Size"), Unique("Size")] public ulong Length { get; set; }
                [Unique("Size")] public ulong Width { get; set; }
                public ulong TotalArea { get; set; }
            }

            // Test Scenario: Invalid [Unique] Name {error}
            public class Allomancy {
                [PrimaryKey] public char Symbol { get; set; }
                public string Metal { get; set; } = "";
                public byte Categorization { get; set; }
                public bool IsInternal { get; set; }
                public bool IsPushing { get; set; }
                [Unique("")] public string MistingTerm { get; set; } = "";
            }

            // Test Scenario: Reserved [Unique] Name {error}
            public class Lens {
                [PrimaryKey] public Guid LensID { get; set; }
                public bool AreContacts { get; set; }
                [Unique("@@@Key")] public double IndexOfRefraction { get; set; }
            }

            // Test Scenario: <Path> Provided for [Unique] on Scalar Property {error}
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
                public string Line12{ get; set; } = "";
                public string Line13 { get; set; } = "";
                public string Line14 { get; set; } = "";
            }

            // Test Scenario: Candidate Key is Improper Superset of Primary Key {error}
            public class WorldHeritageSite {
                [PrimaryKey, Unique("X")] public string Name { get; set; } = "";
                [Unique("X")] public DateTime Inscription { get; set; }
                [PrimaryKey, Unique("X")] public ulong Area { get; set; }
                [Unique("X")] public sbyte Components { get; set; }
                public ushort ReferenceNumber { get; set; }
            }

        /////// --- Primary Key Identification

            // Test Scenario: [PrimaryKey] Annotation Applied to Single Property
            public class XKCDComic {
                [PrimaryKey] public string URL { get; set; } = "";
                public string Title { get; set; } = "";
                public string ImageURL { get; set; } = "";
                public string AltText { get; set; } = "";
            }

            // Test Scenario: [PrimaryKey] Annotation Applied to Multiple Properties
            public class Month {
                [PrimaryKey] public string Calendar { get; set; } = "";
                [PrimaryKey] public uint Index { get; set; }
                public string Name { get; set; } = "";
                public ushort NumDays { get; set; }
                public bool IsLeapMonth { get; set; }
            }

            // Test Scenario: [PrimaryKey] Annotation Applied to All Properties
            public class Character {
                [PrimaryKey] public char Glyph { get; set; }
                [PrimaryKey] public uint CodePoint { get; set; }
                [PrimaryKey] public bool IsASCII { get; set; }
            }

            // Test Scenario: Property Named ID
            public class Actor {
                public int ID { get; set; }
                public string FirstName { get; set; } = "";
                public string? MiddleName { get; set; }
                public string LastName { get; set; } = "";
                public DateTime Birthdate { get; set; }
                public uint EmmyAwards { get; set; }
                public uint OscarAwards { get; set; }
            }

            // Test Scenario: Property Named ID via [Name]
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

            // Test Scenario: Property Named <EntityType>ID
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

            // Test Scenario: Property Named <EntityType>ID via [Name]
            public class Stadium {
                [Name("StadiumID")] public Guid Identifier { get; set; }
                public DateTime Opened { get; set; }
                public DateTime? Closed { get; set; }
                public ulong Capacity { get; set; }
            }

            // Test Scenario: Property Named <EntityType>ID and also <TableName>ID
            [Table("Method")] public class Function {
                public int FunctionID { get; set; }
                public int MethodID { get; set; }
                public string ReturnType { get; set; } = "";
                public string Name { get; set; } = "";
                public byte ArgumentCount { get; set; }
                public bool IsGlobal { get; set; }
                public bool IsFree { get; set; }
                public bool IsMember { get; set; }
            }

            // Test Scenario: Single Candidate Key with Single Non-Nullable Field
            public class Star {
                [Unique] public string ARICNS { get; set; } = "";
                public double Mass { get; set; }
                public double Luminosity { get; set; }
                public float Temperature { get; set; }
                public float Distance { get; set; }
                public ulong Period { get; set; }
                public decimal Parallax { get; set; }
            }

            // Test Scenario: Single Candidate Key with Multiple Non-Nullable Fields
            public class Expiration {
                [Unique("PK")] public uint FeedCode { get; set; }
                [Unique("PK")] public string Underlying { get; set; } = "";
                public string Product { get; set; } = "";
            }

            // Test Scenario: Multiple Candidate Keys, only One with All Non-Nullable Fields
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

            // Test Scenario: Single Non-Nullable Field
            public class Earthquake {
                public Guid SeismicIdentificationNumber { get; set; }
                public DateTime? Occurrence { get; set; }
                public decimal? Magnitude { get; set; }
                public double? EpicenterLatitude { get; set; }
                public double? EpicenterLongitude { get; set; }
            }

            // Test Scenario: Single Non-Nullable Field via [NonNullable]
            public class GeologicEpoch {
                [NonNullable] public ulong? StartingMYA { get; set; }
                public ulong? EndingMYA { get; set; }
                public string? Name { get; set; }
            }

            // Test Scenario: Primary Key Cannot Be Deduced {error}
            public class FederalLaw {
                public string CommonName { get; set; } = "";
                public string? ShortName { get; set; }
                public DateTime Enacted { get; set; }
                public float StatuteIdentifier { get; set; }
                public string IntroducedBy { get; set; } = "";
            }

            // Test Scenario: Nullable Field is Annotated as [PrimaryKey] {error}
            public class NorseWorld {
                [PrimaryKey] public string OldNorse { get; set; } = "";
                public string English { get; set; } = "";
                [PrimaryKey] public int? EddaMentions { get; set; }
            }

            // Test Scenario: Property Named ID is Nullable
            public class Rollercoaster {
                public Guid? ID { get; set; }
                public ulong RollercoasterID { get; set; }
                public double Drop { get; set; }
                public double TopSpeed { get; set; }
                public DateTime Opening { get; set; }
            }

            // Test Scenario: Property Named <EntityType>ID is Nullable
            public class Doctor {
                public ushort? DoctorID { get; set; }
                [Unique("DoctorWho")] public int Regeneration { get; set; }
                [Unique("DoctorWho")] public string Portrayal { get; set; } = "";
                public uint EpisodeCount { get; set; }
                public bool NewWho { get; set; }            
            }

            // Test Scenario: Single Candidate Key Contains a Nullable Field
            public class Polyhedron {
                public string Name { get; set; } = "";
                [Unique("Euler")] public ulong? Faces { get; set; }
                [Unique("Euler")] public ulong? Vertices { get; set; }
                [Unique("Euler")] public ulong? Edges { get; set; }
                public decimal? DihedralAngle { get; set; }
            }

            // Test Scenario: [NamedPrimaryKey]
            [NamedPrimaryKey("LetterPK")] public class HebrewLetter {
                [PrimaryKey] public char Letter { get; set; }
                public char IPA { get; set; }
                public uint GematriaValue { get; set; }
                public byte Position { get; set; }
                public bool IsMaterLectionis { get; set; }
            }

            // Test Scenario: [NamedPrimaryKey] with Unnamed Single Candidate Key
            [NamedPrimaryKey("PrimaryKey")] public class DatabaseField {
                [Unique] public string QualifiedName { get; set; } = "";
                public uint ColumnIndex { get; set; }
                public byte DataType { get; set; }
                public bool Nullable { get; set; }
            }

            // Test Scenario: Invalid [NamedPrimaryKey] {error}
            [NamedPrimaryKey("")] public class Bay {
                [PrimaryKey] public decimal Latitude { get; set; }
                [PrimaryKey] public decimal Longitude { get; set; }
                public string Name { get; set; } = "";
                public ulong SurfaceArea { get; set; }
                public ulong MaxDepth { get; set; }
                public ulong Coastline { get; set; }
            }

            // Test Scenario: [NamedPrimaryKey] Conflicts with Name of Single Candidate Key {error}
            [NamedPrimaryKey("Something")] public class JigsawPuzzle {
                [Unique("GlobalIdentifier")] public Guid PuzzleID { get; set; }
                public ushort NumPieces { get; set; }
                public bool Is3D { get; set; }
            }

            // Test Scenario: [NamedPrimaryKey] Redundant with Name of Single Candidate Key {error}
            [NamedPrimaryKey("PK")] public class TimeZone {
                [Unique("PK")] public float GMT { get; set; }
                public string Name { get; set; } = "";
            }

            // Test Scenario: [NamedPrimaryKey] Is Already Used by Non-PK Candidate Key {error}
            [NamedPrimaryKey("Key13")] public class Currency {
                [PrimaryKey] public string CountryOfUse { get; set; } = "";
                [Unique("Key13")] public char Character { get; set; }
                [Unique("Key13")] public double ExchangeRate { get; set; }
                public long MaxDenomination { get; set; }
                public long MinDenomination { get; set; }
            }

            // Test Scenario: Standalone [Unique] Field Is Marked as [PrimaryKey] {error}
            public class Waterfall {
                [PrimaryKey, Unique] public uint InternationalUnifiedWaterfallNumber { get; set; }
                public string Exonym { get; set; } = "";
                public string Endonym { get; set; } = "";
                public ulong Height { get; set; }
                public ulong WorldRanking { get; set; }
            }

            // Test Scenario: Single Property with Multiple [PrimaryKey] Annotations {error}
            public class Airport {
                [PrimaryKey, PrimaryKey] public string IATA { get; set; } = "";
                public string Name { get; set; } = "";
                public string City { get; set; } = "";
                public DateTime Opening { get; set; }
                public float AveragePassengers { get; set; }
            }

            // Test Scenario: <Path> Provided for [PrimaryKey] on Scalar Property {error}
            public class Highway {
                [PrimaryKey(Path = "---")] public int Number { get; set; }
                public string StartingState { get; set; } = "";
                public string EndingState { get; set; } = "";
                public ulong LengthMiles { get; set; }
                public float AverageSpeedLimit { get; set; }
            }

        /////// --- Data Converters

            // Test Scenario: Data Converter Does Not Change Type
            public class Cenote {
                [PrimaryKey] public string Name { get; set; } = "";
                public float MaxDepth { get; set; }
                [DataConverter(typeof(InvertBoolean))] public bool IsKarst { get; set; }
                public decimal Latitude { get; set; }
                public decimal Longitude { get; set; }
            }

            // Test Scenario: Data Converter Changes Type
            public class Comet {
                [PrimaryKey] public Guid AstronomicalIdentifier { get; set; }
                public double Aphelion { get; set; }
                [DataConverter(typeof(RoundDownDouble))] public double Perihelion { get; set; }
                [DataConverter(typeof(RoundDownDouble))] public double Eccentricity { get; set; }
                public ulong MassKg { get; set; }
                public double Albedo { get; set; }
                public float OrbitalPeriod { get; set; }
            }

            // Test Scenario: Data Converter with Non-Nullable Source on Nullable Field
            public class RoyalHouse {
                [PrimaryKey] public string HouseName { get; set; } = "";
                public DateTime Founded { get; set; }
                [DataConverter(typeof(DeNullify<string>))] public string? CurrentHead { get; set; }
                [DataConverter(typeof(DeNullify<int>))] public int? TotalMonarchs { get; set; }
            }

            // Test Scenario: Data Converter with Nullable Source on Non-Nullable Field
            public class Planeswalker {
                [PrimaryKey] public string Name { get; set; } = "";
                public sbyte MannaCost { get; set; }
                public sbyte InitialLoyalty { get; set; }
                [DataConverter(typeof(Nullify<string>))] public string Ability1 { get; set; } = "";
                [DataConverter(typeof(Nullify<string>))] public string Ability2 { get; set; } = "";
                [DataConverter(typeof(Nullify<string>))] public string Ability3 { get; set; } = "";
            }

            // Test Scenario: Data Converter with Nullable Source for Non-Nullable Field
            public class GolfHole {
                [PrimaryKey] public int CourseID { get; set; }
                [PrimaryKey] public byte HoleNumber { get; set; }
                public byte Par { get; set; }
                public ushort DistanceToFlag { get; set; }
                [DataConverter(typeof(ByteModulo16))] public byte NumSandTraps { get; set; }
                public bool ContainsWaterHazard { get; set; }
            }

            // Test Scenario: Data Converter Source Type Does Not Match and is Not Convertible {error}
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

            // Test Scenario: Data Converter Source Type Does Not Match but is Convertible {error}
            public class ConstitutionalAmendment {
                [PrimaryKey, DataConverter(typeof(Nullify<long>))] public int Number { get; set; }
                public DateTime Ratified { get; set; }
                public double RatificationPercentage { get; set; }
                public string Text { get; set; } = "";
            }

            // Test Scenario: Data Converter Target Type is Not Supported {error}
            public class SNLEpisode {
                [PrimaryKey] public byte Season { get; set; }
                [PrimaryKey] public byte EpisodeNumber { get; set; }
                public string Host { get; set; } = "";
                public string MusicalGuest { get; set; } = "";
                [DataConverter(typeof(DateToError))] public DateTime AirDate { get; set; }
                public ushort WeekendUpdateDuration { get; set; }
            }

            // Test Scenario: Data Converter Type is not a Data Converter {error}
            public class MetraRoute {
                [PrimaryKey] public int RouteID { get; set; }
                public string Name { get; set; } = "";
                public string SourceStation { get; set; } = "";
                public string Destination { get; set; } = "";
                [DataConverter(typeof(int))] public string Line { get; set; } = "";
                public ushort DepartureTime { get; set; }
            }

            // Test Scenario: Data Converter Type is not Default Constructible {error}
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

            // Test Scneario: Error Thrown while Constructing Data Converter Type {error}
            public class Sword {
                [PrimaryKey] public string Name { get; set; } = "";
                public decimal Sharpness { get; set; }
                public float Length { get; set; }
                public float Weight { get; set; }
                public int Kills { get; set; }
                [DataConverter(typeof(AlwaysError))] public short YearForged { get; set; }
            }

            // Test Scenario: Identity Data Converter
            public class FieldGoal {
                [PrimaryKey, DataConverter(typeof(Identity<DateTime>))] public DateTime When { get; set; }
                [DataConverter(typeof(Identity<bool>))] public bool Made { get; set; }
                [DataConverter(typeof(Identity<int>))] public int Doinks { get; set; }
                [DataConverter(typeof(Identity<string>))] public string Kicker { get; set; } = "";
            }

            // Test Scenario: Single Property with Multiple [DataConverter] Annotations {error}
            public class BowlingFrame {
                [PrimaryKey] public Guid FrameID { get; set; }
                public short Round { get; set; }
                [DataConverter(typeof(ByteModulo16)), DataConverter(typeof(Identity<byte>))] public byte FirstThrowPins { get; set; }
                public byte? SecondThrowPins { get; set; }
            }

        /////// --- CHECK Constraints
        
            // None of the attributes ([Check] or anything derived from [Constraint]) exposes the values: they hold on
            // to them and then expose a "create clause" API where the Field is provided. This means that there's
            // limited opportunity for type-checking, and no opportunity for DateTime/GUID parsing. We'll have to do
            // something about that. Elsewise, all the [Constraint]-derived attributes are basically the same and
            // there's little error checking to be done - the exception is StringLength must be applied to a string-type
            // Field.
    }
}
