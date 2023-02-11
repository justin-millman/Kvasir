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

            // Test Scenario: Property Annotated with Both [CodeOnly] and [IncludeInModel] {error}
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

            // Test Scenario: Property Annotated with Both [Nullable] and [NonNullable] {error}
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

            // Test Scenario: Valid Default Value for Data-Converted Target Type, Not Original Type {error}
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
            public class BankCheck {
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
                public string Line12 { get; set; } = "";
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

            // Test Scenario: Error Thrown while Constructing Data Converter Type {error}
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

                    // ~~~~~ Signedness Constraints ~~~~~

            // Test Scenario: [IsPositive] Annotation Applied to Numeric Field
            public class GreekLetter {
                [PrimaryKey] public char Majuscule { get; set; }
                public char Miniscule { get; set; }
                public char? WordFinal { get; set; }
                public string Name { get; set; } = "";
                [Check.IsPositive] public int NumericValue { get; set; }
                public string AncientIPA { get; set; } = "";
                public string ModernIPA { get; set; } = "";
            }

            // Test Scenario: [IsPositive] Annotation Applied to Non-Numeric Field {error}
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

            // Test Scenario: <Path> Provided for [IsPositive] Annotation on Scalar Property {error}
            public class Canal {
                [PrimaryKey] public int CanalID { get; set; }
                public string Name { get; set; } = "";
                [Check.IsPositive(Path = "---")] public double Length { get; set; }
                public double MaxBoatLength { get; set; }
                public double MaxBoatBeam { get; set; }
                public double MaxBoatDraft { get; set; }
                public DateTime Opening { get; set; }
            }

            // Test Scenario: Multiple [IsPositive] Annotations {error}
            public class BaseballCard {
                [PrimaryKey] public string Company { get; set; } = "";
                [PrimaryKey] public string Player { get; set; } = "";
                [PrimaryKey, Check.IsPositive, Check.IsPositive] public int CardNumber { get; set; }
                public bool IsMintCondition { get; set; }
                public ushort NumPrinted { get; set; }
                public decimal Value { get; set; }
            }

            // Test Scenario: [IsNegative] Annotation Applied to Signed Numeric Field
            public class Acid {
                [PrimaryKey] public string Name { get; set; } = "";
                public string Formula { get; set; } = "";
                [Check.IsNegative] public float pH { get; set; }
                public int HazardNumber { get; set; }
            }

            // Test Scenario: [IsNegative] Annotation Applied to Unsigned Numeric Field {error}
            public class Cereal {
                [PrimaryKey] public string Brand { get; set; } = "";
                [PrimaryKey] public string Variety { get; set; } = "";
                [Check.IsNegative] public ushort CaloriesPerServing { get; set; }
                public double PotassiumPerServing { get; set; }
                public double SodiumPerServing { get; set; }
                public double CarbsPerServing { get; set; }
                public double ProteinPerServing { get; set; }
            }

            // Test Scenario: [IsNegative] Annotation Applied to Non-Numeric Field {error}
            public class KeySignature {
                [PrimaryKey, Check.IsNegative] public char Note { get; set; }
                [PrimaryKey] public bool Sharp { get; set; }
                [PrimaryKey] public bool Flat { get; set; }
                [PrimaryKey] public bool Natural { get; set; }
            }

            // Test Scenario: <Path> Provided for [IsNegative] Annotation on Scalar Property {error}
            public class CircleOfHell {
                [PrimaryKey, Check.IsNegative(Path = "---")] public byte Level { get; set; }
                public string Title { get; set; } = "";
                public string PrimeResident { get; set; } = "";
                public ulong CantoOfIntroduction { get; set; }
            }

            // Test Scenario: Multiple [IsNegative] Annotations {error}
            public class Alkene {
                [PrimaryKey] public string Formula { get; set; } = "";
                public bool IsCyclic { get; set; }
                [Check.IsNegative, Check.IsNegative] public double FreezingPoint { get; set; }
                public double Density { get; set; }
            }

            // Test Scenario: [IsNonZero] Annotation Applied to Numeric Field
            public class RegularPolygon {
                [PrimaryKey] public ushort NumEdges { get; set; }
                [Check.IsNonZero] public double InternalAngle { get; set; }
                public bool IsConvex { get; set; }
            }

            // Test Scenario: [IsNonZero] Annotation Applied to Non-Numeric Field {error}
            public class Brassiere {
                [PrimaryKey] public Guid ProductID { get; set; }
                public ushort Band { get; set; }
                [Check.IsNonZero] public string CupSize { get; set; } = "";
            }

            // Test Scenario: <Path> Provided for [IsNonZero] Annotation on Scalar Property {error}
            public class Cryptocurrency {
                [PrimaryKey] public string CoinName { get; set; } = "";
                public float Precision { get; set; }
                [Check.IsNonZero(Path = "---")] public decimal ExchangeRate { get; set; }
                public DateTime InitialRelease { get; set; }
                public ulong AccountHolders { get; set; }
            }

            // Test Scenario: Multiple [IsNonZero] Annotations {error}
            public class Shoe {
                [PrimaryKey] public Guid ProductID { get; set; }
                public string Brand { get; set; } = "";
                [Check.IsNonZero, Check.IsNonZero] public float? Mens { get; set; }
                public float? Womens { get; set; }
                public bool IsHighHeel { get; set; }
                public bool IsBoot { get; set; }
                public bool IsSportsShoe { get; set; }
            }

            // Test Scenario: Signedness Annotation on Property Data-Converted to Numeric Type
            public class SwimmingPool {
                [PrimaryKey] public uint Depth { get; set; }
                [PrimaryKey] public uint Length { get; set; }
                [PrimaryKey, DataConverter(typeof(CharToInt)), Check.IsPositive] public char Classification { get; set; }
                public bool HasDivingBoard { get; set; }
            }

            // Test Scenario: Signedness Annotation on Property Data-Converted from Numeric Type {error}
            public class Elevator {
                [PrimaryKey] public Guid ProductNumber { get; set; }
                public DateTime LastInspected { get; set; }
                public float MaxLoad { get; set; }
                [Check.IsNonZero, DataConverter(typeof(IntToString))] public int NumFloors { get; set; }
            }

            // Test Scenario: Property Annotated with Both [IsPositive] and [IsNegative] {error}
            public class Directory {
                [PrimaryKey] public string? Parent { get; set; } = "";
                [PrimaryKey] public string Name { get; set; } = "";
                public uint Owner { get; set; }
                [Check.IsPositive, Check.IsNegative] public short Mode { get; set; }
                public ulong SizeMb { get; set; }
            }

            // Test Scenario: Property Annotated with Both [IsPositive] and [IsNonZero] {error}
            public class Peninsula {
                [PrimaryKey] public string Name { get; set; } = "";
                [Check.IsPositive, Check.IsNonZero] public long Coastline { get; set; }
                public ulong Area { get; set; }
                public string Continent { get; set; } = "";
                public ulong Population { get; set; }
            }

            // Test Scenario: Property Annotated with Both [IsNegative] and [IsNonZero] {error}
            public class HTTPError {
                [PrimaryKey, Check.IsNegative, Check.IsNonZero] public int ErrorCode { get; set; }
                public string Title { get; set; } = "";
                public string Description { get; set; } = "";
                public bool IsDeprecated { get; set; }
            }

                    // ~~~~~ Comparison Constraints ~~~~~

            // Test Scenario: [IsGreaterThan] Annotation Applied to Numeric Field
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

            // Test Scenario: [IsGreaterThan] Annotation Applied to Textual Field
            public class MultipleChoiceQuestion {
                [PrimaryKey] public Guid ID { get; set; }
                [Check.IsGreaterThan('@')] public char CorrectAnswer { get; set; }
                public string QuestionText { get; set; } = "";
                [Check.IsGreaterThan("A. ")] public string ChoiceA { get; set; } = "";
                [Check.IsGreaterThan("A. ")] public string ChoiceB { get; set; } = "";
                [Check.IsGreaterThan("A. ")] public string ChoiceC { get; set; } = "";
                [Check.IsGreaterThan("A. ")] public string ChoiceD { get; set; } = "";
            }

            // Test Scenario: [IsGreaterThan] Annotation Applied to DateTime Field
            public class GoldRush {
                [PrimaryKey] public string Location { get; set; } = "";
                [PrimaryKey, Check.IsGreaterThan("1200-03-18")] public DateTime StartDate { get; set; }
                [PrimaryKey, Check.IsGreaterThan("1176-11-22")] public DateTime EndDate { get; set; }
                public decimal EstimatedGrossWealth { get; set; }
            }

            // Test Scenario: [IsGreaterThan] Annotation Applied to Nullable Orderable Field
            public class Baryon {
                [PrimaryKey] public string Name { get; set; } = "";
                public double Spin { get; set; }
                [Check.IsGreaterThan((short)-5)] public short? Charge { get; set; }
                public float AngularMomentum { get; set; }
                public bool PositiveParity { get; set; }
            }

            // Test Scenario: [IsGreaterThan] Annotation Applied to Non-Ordered Field {error}
            public class Font {
                [PrimaryKey] public string Name { get; set; } = "";
                public ulong YearReleased { get; set; }
                public decimal XHeight { get; set; }
                [Check.IsGreaterThan(false)] public bool HasSerifs { get; set; }
            }

            // Test Scenario: [IsGreaterThan] Anchor is null {error}
            public class UNResolution {
                [PrimaryKey] public int Number { get; set; }
                public string Title { get; set; } = "";
                [Check.IsGreaterThan(null!)] public ushort? NumSignatories { get; set; }
                public DateTime Introduced { get; set; }
                public bool Adopted { get; set; }
            }

            // Test Scenario: [IsGreaterThan] Anchor is Maximum Possible Value {error}
            public class Upanishad {
                [PrimaryKey] public string Name { get; set; } = "";
                [Check.IsGreaterThan(sbyte.MaxValue)] public sbyte Index { get; set; }
                public DateTime WhenAuthored { get; set; }
                public ulong WordCount { get; set; }
            }

            // Test Scenario: Invalid and Unconvertible Anchor Provided to [IsGreaterThan] Annotation {error}
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

            // Test Scenario: Invalid but Convertible Anchor Provided to [IsGreaterThan] Annotation {error}
            public class ChineseCharacter {
                [PrimaryKey, Check.IsGreaterThan((byte)14)] public char Character { get; set; }
                public string PinyinTransliteration { get; set; } = "";
                public string CantoneseTransliteration { get; set; } = "";
                public string PrimaryDefinition { get; set; } = "";
            }

            // Test Scenario: Single-Element Array of Correct as Anchor to [IsGreaterThan] Annotation {error}
            public class Query {
                [PrimaryKey] public string SELECT { get; set; } = "";
                [PrimaryKey] public string FROM { get; set; } = "";
                [PrimaryKey, Check.IsGreaterThan(new[] { "\0" })] public string WHERE { get; set; } = "";
                [PrimaryKey] public string GROUP_BY { get; set; } = "";
                [PrimaryKey] public string ORDER_BY { get; set; } = "";
                [PrimaryKey] public string LIMIT { get; set; } = "";
            }

            // Test Scenario: <Path> Provided for [IsGreaterThan] Annotation on Scalar Property {error}
            public class Canyon {
                [PrimaryKey] public string Name { get; set; } = "";
                public float CenterLatitude { get; set; }
                public float CenterLongitude { get; set; }
                [Check.IsGreaterThan(100.65, Path = "---")] public double Depth { get; set; }
                public ulong TotalArea { get; set; }
            }

            // Test Scenario: Multiple [IsGreaterThan] Annotations {error}
            public class NuclearPowerPlant {
                [PrimaryKey] public string OfficialName { get; set; } = "";
                public DateTime Opened { get; set; }
                public bool IsOnline { get; set; }
                [Check.IsGreaterThan(-1L), Check.IsGreaterThan(37L)] public long Meltdowns { get; set; }
                public ushort Reactors { get; set; }
                public ulong ThermalCapacity { get; set; }
                public ulong PowerGenerated { get; set; }
            }

            // Test Scenario: [IsLessThan] Annotation Applied to Numeric Field
            public class Resistor {
                [PrimaryKey] public Guid CircuitComponentIdentifier { get; set; }
                [Check.IsLessThan(27814L)] public long Resistance { get; set; }
                public bool IsThermistor { get; set; }
                public bool IsVaristor { get; set; }
            }

            // Test Scenario: [IsLessThan] Annotation Applied to Textual Field
            public class Senator {
                [PrimaryKey] public string FirstName { get; set; } = "";
                public string? MiddleName { get; set; }
                [PrimaryKey, Check.IsLessThan("...")] public string LastName { get; set; } = "";
                public string State { get; set; } = "";
                public ulong LastElected { get; set; }
                public string Party { get; set; } = "";
                public bool IsInLeadership { get; set; }
            }

            // Test Scenario: [IsLessThan] Annotation Applied to DateTime Field
            public class Commercial {
                [PrimaryKey] public ushort Channel { get; set; }
                [PrimaryKey, Check.IsLessThan("2300-01-01")] public DateTime TimeSlot { get; set; }
                public byte LengthSeconds { get; set; }
                public bool ForSuperBowl { get; set; }
                public string? Company { get; set; } = "";
            }

            // Test Scenario: [IsLessThan] Annotation Applied to Nullable Orderable Field
            public class AutoRacetrack {
                [PrimaryKey] public string Name { get; set; } = "";
                [Check.IsLessThan(12000000L)] public long? TrackLength { get; set; }
                public byte FIAGrade { get; set; }
                public double TopRecordedSpeed { get; set; }
                public double LapRecord { get; set; }
            }

            // Test Scenario: [IsLessThan] Annotation Applied to Non-Ordered Field {error}
            public class DLL {
                [PrimaryKey, Check.IsLessThan("db5bf338-7dcd-46b5-85a2-ee3c518b9ed2")] public Guid ID { get; set; }
                public byte BitSize { get; set; }
                [PrimaryKey] public string AbsolutePath { get; set; } = "";
                public ulong MemoryKB { get; set; }
                public string? Author { get; set; }
                public float Version { get; set; }
            }

            // Test Scenario: [IsLessThan] Anchor is null {error}
            public class Animation {
                [PrimaryKey] public string File { get; set; } = "";
                [PrimaryKey] public ushort Slide { get; set; }
                [PrimaryKey] public string ObjectName { get; set; } = "";
                [PrimaryKey] public string Trigger { get; set; } = "";
                [Check.IsLessThan(null!)] public double? Duration { get; set; }
                public bool IsOnEntry { get; set; }
                public bool IsOnExit { get; set; }
                public bool IsEmphasis { get; set; }
            }

            // Test Scenario: [IsLessThan] Anchor is Minimum Possible Value {error}
            public class StrategoPiece {
                [PrimaryKey] public string Title { get; set; } = "";
                public int CountPerSide { get; set; }
                [Check.IsLessThan(uint.MinValue)] public uint Value { get; set; }
                public bool IsFlag { get; set; }
                public bool IsBomb { get; set; }
            }

            // Test Scenario: Invalid and Unconvertible Anchor Provided to [IsLessThan] Annotation {error}
            public class Distribution {
                [PrimaryKey] public string Title { get; set; } = "";
                public double Mean { get; set; }
                public double Median { get; set; }
                [Check.IsLessThan("Zero")] public double Mode { get; set; }
                public string PDF { get; set; } = "";
                public string CDF { get; set; } = "";
            }

            // Test Scenario: Invalid but Convertible Anchor Provided to [IsLessThan] Annotation {error}
            public class WebBrowser {
                [PrimaryKey] public string Name { get; set; } = "";
                public bool IsChromiumBased { get; set; }
                public string LatestRelease { get; set; } = "";
                [Check.IsLessThan(100)] public float MarketShare { get; set; }
                public uint HTML5Score { get; set; }
            }

            // Test Scenario: Single-Element Array of Correct as Anchor to [IsLessThan] Annotation {error}
            public class GrammaticalCase {
                [PrimaryKey] public string Case { get; set; } = "";
                public string Abbreviation { get; set; } = "";
                public bool PresentInEnglish { get; set; }
                public bool PresentInLatin { get; set; }
                [Check.IsLessThan(new[] { 'z' })] public char? Affix { get; set; }
            }

            // Test Scenario: <Path> Provided for [IsLessThan] Annotation on Scalar Property {error}
            public class Potato {
                [PrimaryKey] public int GlobaPotatoIdentificationNumber { get; set; }
                [Check.IsLessThan(1.0f, Path = "---")] public float Weight { get; set; }
                public string Preparation { get; set; } = "";
                public string Genus { get; set; } = "";
                public string Species { get; set; } = "";
            }

            // Test Scenario: Multiple [IsLessThan] Annotations {error}
            public class CinemaSins {
                [PrimaryKey] public string URL { get; set; } = "";
                public string Movie { get; set; } = "";
                [Check.IsLessThan(1712312389UL), Check.IsLessThan(18231247121293UL)] public ulong SinCount { get; set; }
                public string Sentence { get; set; } = "";
                public bool PatreonExclusive { get; set; }
            }

            // Test Scenario: [IsGreaterOrEqualTo] Annotation Applied to Numeric Field
            public class Geyser {
                [PrimaryKey] public string Name { get; set; } = "";
                [Check.IsGreaterOrEqualTo(0L)] public long EruptionHeight { get; set; }
                [Check.IsGreaterOrEqualTo(0f)] public float Elevation { get; set; }
                [Check.IsGreaterOrEqualTo(0)] public int EruptionDuration { get; set; }
                public string? NationalParkHome { get; set; }
            }

            // Test Scenario: [IsGreaterOrEqualTo] Annotation Applied to Textual Field
            public class Hotel {
                [PrimaryKey] public Guid HotelID { get; set; }
                [Check.IsGreaterOrEqualTo("")] public string HotelName { get; set; } = "";
                [Check.IsGreaterOrEqualTo('1')] public char Stars { get; set; }
                public ushort NumFloors { get; set; }
                public ushort NumRooms { get; set; }
                public bool ContinentalBreakfast { get; set; }
            }

            // Test Scenario: [IsGreaterOrEqualTo] Annotation Applied to DateTime Field
            public class ETF {
                [PrimaryKey] public string Symbol { get; set; } = "";
                [Check.IsGreaterOrEqualTo("1377-06-19")] public DateTime FirstPosted { get; set; }
                public decimal ClosingPrice { get; set; }
                public string TopConstituent { get; set; } = "";
                public string OfferingOrganization { get; set; } = "";
            }

            // Test Scenario: [IsGreaterOrEqualTo] Annotation Applied to Nullable Orderable Field
            public class Muscle {
                [PrimaryKey] public uint FMAID { get; set; }
                public string Name { get; set; } = "";
                [Check.IsGreaterOrEqualTo("~~~")] public string? Nerve { get; set; }
                public bool IsFlexor { get; set; }
                public bool IsExtensor { get; set; }
            }

            // Test Scenario: [IsGreaterOrEqualTo] Annotation Applied to Non-Ordered Field {error}
            public class Steak {
                [PrimaryKey] public Guid SteakID { get; set; }
                public float Temperature { get; set; }
                public string Cut { get; set; } = "";
                [Check.IsGreaterOrEqualTo(false)] public bool FromSteakhouse { get; set; }
                public double Weight { get; set; }
            }

            // Test Scenario: [IsGreaterOrEqualTo] Anchor is null {error}
            public class Neurotoxin {
                [PrimaryKey] public string Name { get; set; } = "";
                public string ChemicalFormula { get; set; } = "";
                public string? Abbreviation { get; set; }
                [Check.IsGreaterOrEqualTo(null!)] public double MolarMass { get; set; }
                public string Inhibits { get; set; } = "";
            }

            // Test Scenario: [IsGreaterOrEqualTo] Anchor is Minimum Possible Value {error}
            public class Bacterium {
                [PrimaryKey] public string Genus { get; set; } = "";
                [PrimaryKey] public string Species { get; set; } = "";
                public bool GramPositive { get; set; }
                [Check.IsGreaterOrEqualTo(ushort.MinValue)] public ushort NumStrains { get; set; }
                public string Shape { get; set; } = "";
            }

            // Test Scenario: Invalid and Unconvertible Anchor Provided to [IsGreaterOrEqualTo] Annotation {error}
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

            // Test Scenario: Invalid but Convertible Anchor Provided to [IsGreaterOrEqualTo] Annotation {error}
            public class Keystroke {
                [PrimaryKey] public byte Key { get; set; }
                public bool Shift { get; set; }
                public bool Control { get; set; }
                public bool Alt { get; set; }
                public bool Function { get; set; }
                public string Description { get; set; } = "";
                [Check.IsGreaterOrEqualTo(290)] public char? ResultingGlyph { get; set; }
            }

            // Test Scenario: Single-Element Array of Correct as Anchor to [IsGreaterOrEqualTo] Annotation {error}
            public class Zoo {
                [PrimaryKey] public string City { get; set; } = "";
                [PrimaryKey] public string Name { get; set; } = "";
                public ushort AnimalPopulation { get; set; }
                [Check.IsGreaterOrEqualTo(new[] { 4.2213f })] public float AverageVisitorsPerDay { get; set; }
                public bool AZA { get; set; }
            }

            // Test Scenario: <Path> Provided for [IsGreaterOrEqualTo] Annotation on Scalar Property {error}
            public class Hieroglyph {
                [PrimaryKey] public ulong UnicodeValue { get; set; }
                [Check.IsGreaterOrEqualTo("?", Path = "---")] public string Glyph { get; set; } = "";
                public string Name { get; set; } = "";
                public bool IsDeterminative { get; set; }
                public string Semantic { get; set; } = "";
            }

            // Test Scenario: Multiple [IsGreaterOrEqualTo] Annotations {error}
            public class SolarEclipse {
                [PrimaryKey] public DateTime Start { get; set; }
                [PrimaryKey] public DateTime End { get; set; }
                public double Magnitude { get; set; }
                public double Gamma { get; set; }
                [Check.IsGreaterOrEqualTo(3), Check.IsGreaterOrEqualTo(-5)] public int SarosCycle { get; set; }
            }

            // Test Scenario: [IsLessOrEqualTo] Annotation Applied to Numeric Field
            public class Fjord {
                [PrimaryKey] public string Name { get; set; } = "";
                [Check.IsLessOrEqualTo(90f)] public float Latitude { get; set; }
                [Check.IsLessOrEqualTo(90f)] public float Longitude { get; set; }
                [Check.IsLessOrEqualTo(100000UL)] public ulong Length { get; set; }
                [Check.IsLessOrEqualTo((short)6723)] public short Width { get; set; }
            }

            // Test Scenario: [IsLessOrEqualTo] Annotation Applied to Textual Field
            public class ExcelRange {
                [PrimaryKey, Check.IsLessOrEqualTo("XFD")] public string StartColumn { get; set; } = "";
                public ulong StartRow { get; set; }
                [PrimaryKey, Check.IsLessOrEqualTo("XFD")] public string EndColumn { get; set; } = "";
                public ulong EndRow { get; set; }
            }

            // Test Scenario: [IsLessOrEqualTo] Annotation Applied to DateTime Field
            public class Representative {
                [PrimaryKey] public string Name { get; set; } = "";
                public string Party { get; set; } = "";
                public long District { get; set; }
                public string State { get; set; } = "";
                [Check.IsLessOrEqualTo("2688-12-02")] public DateTime FirstElected { get; set; }
            }

            // Test Scenario: [IsLessOrEqualTo] Annotation Applied to Nullable Orderable Field
            public class Subreddit {
                [PrimaryKey] public string Identifier { get; set; } = "";
                public string URL { get; set; } = "";
                [Check.IsLessOrEqualTo("???")] public string? Moderator { get; set; }
                public ulong Posts { get; set; }
                public ulong NetKarma { get; set; }
                public ulong Subscribers { get; set; }
            }

            // Test Scenario: [IsLessOrEqualTo] Annotation Applied to Non-Ordered Field {error}
            public class Sunscreen {
                [PrimaryKey, Check.IsLessOrEqualTo("7242f5a0-e4b2-4834-9485-ec39e4ab8ca4")] public Guid ID { get; set; }
                public string Brand { get; set; } = "";
                public double SPF { get; set; }
                public bool IsNatural { get; set; }
            }

            // Test Scenario: [IsLessOrEqualTo] Anchor is null {error}
            public class VoirDire {
                [PrimaryKey] public DateTime When { get; set; }
                public ushort InitialPoolSize { get; set; }
                public ushort ProsecutionDismissals { get; set; }
                public ushort DefenseDismissals { get; set; }
                [Check.IsLessOrEqualTo(null!)] public ushort BatsonChallenges { get; set; }
            }

            // Test Scenario: [IsLessOrEqualTo] Anchor is Maximum Possible Value {error}
            public class ShellCommand {
                [PrimaryKey] public string Command { get; set; } = "";
                public bool PrintsToStdOut { get; set; }
                public long NumArguments { get; set; }
                [Check.IsLessOrEqualTo(long.MaxValue)] public long NumOptions { get; set; }
                public string HelpText { get; set; } = "";
                public string ManText { get; set; } = "";
            }

            // Test Scenario: Invalid and Unconvertible Anchor Provided to [IsLessOrEqualTo] Annotation {error}
            public class Dreidel {
                [PrimaryKey] public int IsraeliDreidelIdentificationNumber { get; set; }
                [Check.IsLessOrEqualTo((byte)153)] public string? SerialCode { get; set; }
                public bool ShowsShin { get; set; }
                public float Weight { get; set; }
                public bool MadeOutOfClay { get; set; }
            }

            // Test Scenario: Invalid but Convertible Anchor Provided to [IsLessOrEqualTo] Annotation {error}
            public class ArthurianKnight {
                [PrimaryKey] public string Name { get; set; } = "";
                public int RoundTableSeatNumber { get; set; }
                public bool TouchedExcalibur { get; set; }
                [Check.IsLessOrEqualTo(4U)] public ulong MalloryMentions { get; set; }
                public bool AppearsInMerlin { get; set; }
            }

            // Test Scenario: Single-Element Array of Correct as Anchor to [IsLessOrEqualTo] Annotation {error}
            public class Mint {
                [PrimaryKey] public string City { get; set; } = "";
                [PrimaryKey] public string Currency { get; set; } = "";
                [Check.IsLessOrEqualTo(new[] { "1845-08-30" })] public DateTime Established { get; set; }
                public decimal CumulativeValueMinted { get; set; }
                public char Identifier { get; set; }
                public bool Operational { get; set; }
            }

            // Test Scenario: <Path> Provided for [IsLessOrEqualTo] Annotation on Scalar Property {error}
            public class PlaneOfExistence {
                [PrimaryKey, Check.IsLessOrEqualTo("Nether-Plane", Path = "---")] public string Name { get; set; } = "";
                public bool IsElemental { get; set; }
                public bool IsHellPlane { get; set; }
                public bool IsFaeriePlane { get; set; }
            }

            // Test Scenario: Multiple [IsLessOrEqualTo] Annotations {error}
            public class Archbishop {
                [PrimaryKey] public string FirstName { get; set; } = "";
                [PrimaryKey] public string? MiddleName { get; set; }
                [PrimaryKey] public string LastName { get; set; } = "";
                [Check.IsLessOrEqualTo("124"), Check.IsLessOrEqualTo("Uppsala")] public string City { get; set; } = "";
                public DateTime Consecrated { get; set; }
                public DateTime Installed { get; set; }
            }

            // Test Scenario: [IsNot] Annotation Applied to Numeric Field
            public class Bridge {
                [PrimaryKey] public int ID { get; set; }
                [Check.IsNot(34)] public int Length { get; set; }
                [Check.IsNot(15UL)] public ulong Height { get; set; }
                [Check.IsNot(0.23776f)] public float Width { get; set; }
                public DateTime Built { get; set; }
                public bool IsOverWater { get; set; }
            }

            // Test Scenario: [IsNot] Annotation Applied to Textual Field
            public class Quatrain {
                [PrimaryKey] public string Title { get; set; } = "";
                [PrimaryKey] public string Author { get; set; } = "";
                [Check.IsNot("Elephant")] public string Line1 { get; set; } = "";
                [Check.IsNot("Giraffe")] public string Line2 { get; set; } = "";
                [Check.IsNot("Crocodile")] public string Line3 { get; set; } = "";
                [Check.IsNot("Rhinoceros")] public string Line4 { get; set; } = "";
            }

            // Test Scenario: [IsNot] Annotation Applied to DateTime Field
            public class SlotMachine {
                [PrimaryKey] public Guid MachineNumber { get; set; }
                public ulong Jackpot { get; set; }
                public decimal LeverCost { get; set; }
                [Check.IsNot("4431-01-21")] public DateTime InstalledOn { get; set; }
                public ulong NumPlays { get; set; }
            }

            // Test Scenario: [IsNot] Annotation Applied to Boolean Field
            public class PoliceOfficer {
                public string FirstName { get; set; } = "";
                public string LastName { get; set; } = "";
                [PrimaryKey] public string Department { get; set; } = "";
                [PrimaryKey] public ushort BadgeNumber { get; set; }
                [Check.IsNot(false)] public bool IsRetired { get; set; }
            }

            // Test Scenario: [IsNot] Annotation Applied to Guid Field
            public class Church {
                [PrimaryKey, Check.IsNot("a3c3ac24-4cf2-428e-a4db-76b30958cc90")] public Guid ChurchID { get; set; }
                public string Name { get; set; } = "";
                public byte NumSpires { get; set; }
                public double Height { get; set; }
                public ushort NumBells { get; set; }
                public DateTime Established { get; set; }
            }

            // Test Scenario: [IsNot] Annotation Applied to Nullable Field
            public class Fountain {
                [PrimaryKey] public string FountainName { get; set; } = "";
                public ulong Height { get; set; }
                public ulong Length { get; set; }
                [Check.IsNot(35.22)] public double Spout { get; set; }
                public string Masonry { get; set; } = "";
            }

            // Test Scenario: [IsNot] Anchor is null {error}
            public class SecurityBug {
                [PrimaryKey] public string CVEIdentifier { get; set; } = "";
                [Check.IsNot(null!)] public string LibraryAffected { get; set; } = "";
                public string VersionPatched { get; set; } = "";
                public DateTime Discovered { get; set; }
                public DateTime Patched { get; set; }
            }

            // Test Scenario: Invalid and Unconvertible Anchor Provided to [IsNot] Annotation {error}
            public class Candle {
                [PrimaryKey] public uint ProductID { get; set; }
                public string? Scent { get; set; }
                public float Height { get; set; }
                [Check.IsNot("Wide")] public float Width { get; set; }
                public double AverageBurnTime { get; set; }
            }

            // Test Scenario: Invalid but Convertible Anchor Provided to [IsNot] Annotation {error}
            public class CompilerWarning {
                [PrimaryKey] public string WarningID { get; set; } = "";
                public byte Severity { get; set; }
                [Check.IsNot(1)] public bool DebugOnly { get; set; }
                public string VersionIntroduced { get; set; } = "";
                public string WarningText { get; set; } = "";
                public bool IsSuppressed { get; set; }
            }

            // Test Scenario: Single-Element Array of Correct as Anchor to [IsNot] Annotation {error}
            public class Alarm {
                [PrimaryKey] public Guid ID { get; set; }
                public byte Hour { get; set; }
                public byte Minute { get; set; }
                [Check.IsNot(new[] { false })] public bool Snoozeable { get; set; }
                public string NotificationSound { get; set; } = "";
            }

            // Test Scenario: <Path> Provided for [IsNot] Annotation on Scalar Property {error}
            public class Prison {
                [PrimaryKey] public string Name { get; set; } = "";
                public ulong Capacity { get; set; }
                public ulong Population { get; set; }
                [Check.IsNot(10u, Path = "---")] public uint SecurityLevel { get; set; }
                public DateTime Opened { get; set; }
            }

            // Test Scenario: Multiple [IsNot] Annotations {error}
            public class Pterosaur {
                public string Family { get; set; } = "";
                [PrimaryKey] public string Genus { get; set; } = "";
                [PrimaryKey] public string Species { get; set; } = "";
                public double Wingspan { get; set; }
                [Check.IsNot(0u), Check.IsNot(7894520u)] public uint Specimens { get; set; }
                public bool Toothed { get; set; }
            }

            // Test Scenario: Ranged Comparison Annotation Anchor is Invalid for DateTime b/c of Formatting {error}
            public class Lease {
                [PrimaryKey] public string Address { get; set; } = "";
                public uint? Unit { get; set; }
                [Check.IsGreaterOrEqualTo("1637+04+18")] public DateTime StartDate { get; set; }
                public DateTime EndDate { get; set; }
                public decimal MonthlyRent { get; set; }
                public bool ContainsOptOut { get; set; }
            }

            // Test Scenario: Ranged Comparison Annotation Anchor is Invalid for DateTime b/c of Range {error}
            public class LotteryTicket {
                [PrimaryKey] public Guid Ticket { get; set; }
                public byte N0 { get; set; }
                public byte N1 { get; set; }
                public byte N2 { get; set; }
                byte N3 { get; set; }
                public byte N4 { get; set; }
                public byte N5 { get; set; }
                [Check.IsNot("2023-02-29")] public DateTime PurchaseTime { get; set; }
            }

            // Test Scenario: Ranged Comparison Annotation Anchor is Invalid for DateTime b/c of Type {error}
            public class ACT {
                [PrimaryKey] public string Taker { get; set; } = "";
                [PrimaryKey, Check.IsLessThan(100)] public DateTime When { get; set; }
                public byte Composite { get; set; }
                public byte Mathematics { get; set; }
                public byte Science { get; set; }
                public byte Reading { get; set; }
                public byte English { get; set; }
            }

            // Test Scenario: Ranged Comparison Annotation on Data-Converted Property
            public class RingOfPower {
                [PrimaryKey] public string Name { get; set; } = "";
                public string? Holder { get; set; }
                [Check.IsGreaterOrEqualTo(7), DataConverter(typeof(BoolToInt))] public bool Destroyed { get; set; }
                public DateTime Forged { get; set; }
                public string CentralStone { get; set; } = "";
            }

            // Test Scenario: Ranged Comparison Annotation with Valid Anchor for Conversion Target Type {error}
            public class Genie {
                [PrimaryKey] public string Identifier { get; set; } = "";
                [DataConverter(typeof(IntToString)), Check.IsLessThan(144)] public int NumWishes { get; set; }
                public bool IsFriendly { get; set; }
                public string PrincipalForm { get; set; } = "";
            }

                    // ~~~~~ String Length Constraints ~~~~~

            // Test Scenario: [IsNonEmpty] Annotation Applied to String-Type Field
            public class Chocolate {
                [PrimaryKey, Check.IsNonEmpty] public string Name { get; set; } = "";
                public double PercentDark { get; set; }
                public bool AllNatural { get; set; }
            }

            // Test Scenario: [IsNonEmpty] Annotation Applied to Nullable String-Type Field
            public class Scholarship {
                [PrimaryKey] public Guid IdentificationString { get; set; }
                public decimal Amount { get; set; }
                public ulong Submissions { get; set; }
                [Check.IsNonEmpty] public string? Organization { get; set; }
                [Check.IsNonEmpty] public string? TargetSchool { get; set; }
            }

            // Test Scenario: [IsNonEmpty] Annotation Applied to Non-String Field {error}
            public class MovieTicket {
                [PrimaryKey] public string Theater { get; set; } = "";
                [PrimaryKey] public string Film { get; set; } = "";
                [PrimaryKey] public DateTime Showtime { get; set; }
                [Check.IsNonEmpty] public char Row { get; set; }
                public byte SeatNumber { get; set; }
                public bool OnlineOrder { get; set; }
            }

            // Test Scenario: <Path> Provided for [IsNonEmpty] Annotation on Scalar Property {error}
            public class ASLSign {
                [PrimaryKey, Check.IsNonEmpty(Path = "---")] public string Gloss { get; set; } = "";
                public string Location { get; set; } = "";
                public string Movement { get; set; } = "";
                public string HandShape { get; set; } = "";
                public string PalmOrientation { get; set; } = "";
            }

            // Test Scenario: Multiple [IsNonEmpty] Annotations {error}
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

            // Test Scenario: [LengthIsAtLeast] Annotation Applied to String-Type Field
            public class NFLPenalty {
                [PrimaryKey, Check.LengthIsAtLeast(1)] public string Penalty { get; set; } = "";
                public bool OnOffense { get; set; }
                public bool OnDefense { get; set; }
                public bool OnSpecialTeams { get; set; }
                public byte Yards { get; set; }
                public bool LossOfDown { get; set; }
            }

            // Test Scenario: [LengthIsAtLeast] Annotation Applied to Nullable String-Type Field
            public class Ben10Alien {
                [PrimaryKey] public string Name { get; set; } = "";
                [Check.LengthIsAtLeast(7)] public string? AlternateName { get; set; }
                public string HomePlanet { get; set; } = "";
                public uint OrderOfAddition { get; set; }
                public byte Appearances { get; set; }
                public string FirstEpisode { get; set; } = "";
            }

            // Test Scenario: [LengthIsAtLeast] Annotation Given Negative Anchor {error}
            public class LaborOfHeracles {
                [PrimaryKey] public ushort Order { get; set; }
                [Check.LengthIsAtLeast(-144)] public string Target { get; set; } = "";
                public bool WasExtra { get; set; }
                public bool TargetToBeKilled { get; set; }
            }

            // Test Scenario: [LengthIsAtLeast] Annotation Given Anchor of 0 {error}
            public class HolyRomanEmperor {
                [PrimaryKey] public DateTime ReignBegin { get; set; }
                [PrimaryKey] public DateTime ReignEnd { get; set; }
                [Check.LengthIsAtLeast(0)] public string Name { get; set; } = "";
                public string RoyalHouse { get; set; } = "";
            }

            // Test Scenario: [LengthIsAtLeast] Annotation Applied to Non-String Field {error}
            public class HashFunction {
                [PrimaryKey] public string Name { get; set; } = "";
                public ushort DigestSize { get; set; }
                [Check.LengthIsAtLeast(7)] public ushort BlockSize { get; set; }
                public DateTime FirstPublished { get; set; }
                public float CollisionLikelihood { get; set; }
            }

            // Test Scenario: <Path> Provided for [LengthIsAtLeast] Annotation on Scalar Property {error}
            public class Histogram {
                [PrimaryKey] public Guid ID { get; set; }
                public long MinBucket { get; set; }
                public long MaxBucket { get; set; }
                public long BucketSize { get; set; }
                [Check.LengthIsAtLeast(2, Path = "---")] public string BucketUnit { get; set; } = "";
            }

            // Test Scenario: Multiple [LengthIsAtLeast] Annotations {error}
            public class Bagel {
                [PrimaryKey, Check.LengthIsAtLeast(17), Check.LengthIsAtLeast(34)] public string Flavor { get; set; } = "";
                [PrimaryKey] public bool Toasted { get; set; }
                [PrimaryKey] public string Schmear { get; set; } = "";
                public decimal Cost { get; set; }
            }

            // Test Scenario: [LengthIsAtMost] Annotation Applied to String-Type Field
            public class Snake {
                [PrimaryKey, Check.LengthIsAtMost(175)] public string Genus { get; set; } = "";
                [PrimaryKey, Check.LengthIsAtMost(13512)] public string Species { get; set; } = "";
                [Check.LengthIsAtMost(25)] public string CommonName { get; set; } = "";
                public bool IsVenomous { get; set; }
                public double AverageLength { get; set; }
                public double AverageWeight { get; set; }
            }

            // Test Scenario: [LengthIsAtMost] Annotation Applied to Nullable String-Type Field
            public class WinterStorm {
                [PrimaryKey] public DateTime When { get; set; }
                public double Snowfall { get; set; }
                public int LowTemperature { get; set; }
                public float MaxWindSpeed { get; set; }
                [Check.LengthIsAtMost(300)] public string? Name { get; set; }
            }

            // Test Scenario: [LengthIsAtMost] Annotation Given Negative Anchor {error}
            public class Fraternity {
                [PrimaryKey, Check.LengthIsAtMost(-7)] public string Name { get; set; } = "";
                public DateTime Founded { get; set; }
                public string Motto { get; set; } = "";
                public int ActiveChapters { get; set; }
                public bool IsSocial { get; set; }
                public bool IsProfessional { get; set; }
            }

            // Test Scenario: [LengthIsAtMost] Annotation Given Anchor of 0
            public class KnockKnockJoke {
                [PrimaryKey, Check.LengthIsAtMost(0)] public string SetUp { get; set; } = "";
                [PrimaryKey, Check.LengthIsAtMost(0)] public string PunchLine { get; set; } = "";
            }

            // Test Scenario: [LengthIsAtMost] Annotation Applied to Non-String Field {error}
            public class Diamond {
                [PrimaryKey] public Guid SerialNumber { get; set; }
                public ushort Carats { get; set; }
                public double Weight { get; set; }
                public decimal MarketValue { get; set; }
                [Check.LengthIsAtMost(50)] public bool IsBloodDiamond { get; set; }
                public string? CurrentMuseum { get; set; }
            }

            // Test Scenario: <Path> Provided for [LengthIsAtMost] Annotation on Scalar Property {error}
            public class Nebula {
                [PrimaryKey] public uint MessierNumber { get; set; }
                [Check.LengthIsAtMost(30, Path = "---")] public string Name { get; set; } = "";
                public double DistanceKLY { get; set; }
                public double ApparentMagnitude { get; set; }
                public string? Constellation { get; set; }
            }

            // Test Scenario: Multiple [LengthIsAtMost] Annotations {error}
            public class OceanicTrench {
                [PrimaryKey, Check.LengthIsAtMost(187), Check.LengthIsAtMost(60)] public string Location { get; set; } = "";
                public ulong Depth { get; set; }
                public double MaxPressure { get; set; }
            }

            // Test Scenario: [LengthIsBetween] Annotation Applied to String-Type Field
            public class Sorority {
                [PrimaryKey] public string Name { get; set; } = "";
                public DateTime Founded { get; set; }
                [Check.LengthIsBetween(4, 1713)] public string Motto { get; set; } = "";
                public int ActiveChapters { get; set; }
                public bool IsSocial { get; set; }
                public bool IsProfessional { get; set; }
            }

            // Test Scenario: [LengthIsBetween] Annotation Applied to Nullable String-Type Field
            public class Telescope {
                [PrimaryKey] public Guid ID { get; set; }
                [Check.LengthIsBetween(1, int.MaxValue)] public string? Name { get; set; }
                public double Mass { get; set; }
                public ulong Power { get; set; }
                public ushort Length { get; set; }
                public ushort Width { get; set; }
            }

            // Test Scenario: [LengthIsBetween] Minimum and Maximum Anchors are Equal
            public class DNACodon {
                [PrimaryKey, Check.LengthIsBetween(3, 3)] public string CodonSequence { get; set; } = "";
                public bool IsStartCodon { get; set; }
                public bool IsStopCodon { get; set; }
                public char AminoAcid { get; set; }
            }

            // Test Scenario: [LengthIsBetween] Annotation Given Negative Minimum Anchor {error}
            public class ShenGongWu {
                [PrimaryKey] public string Name { get; set; } = "";
                public bool HeldByXiaolinMonks { get; set; }
                public uint Appearances { get; set; }
                [Check.LengthIsBetween(-4, 26)] public string InitialEpisode { get; set; } = "";
                public string Ability { get; set; } = "";
            }

            // Test Scenario: [LengthIsBetween] Annotation Given Minimum Anchor of 0 {error}
            public class MilitaryBase {
                [PrimaryKey] public string BaseName { get; set; } = "";
                [Check.LengthIsBetween(0, 400)] public string Commander { get; set; } = "";
                public DateTime Established { get; set; }
                public ulong Area { get; set; }
                public bool IsActive { get; set; }
            }

            // Test Scenario: [LengthIsBetween] Minimum Anchor is Larger than Maximum Anchor {error}
            public class ChristmasCarol {
                [PrimaryKey] public string Title { get; set; } = "";
                public ushort NumWords { get; set; }
                [Check.LengthIsBetween(28841, 1553)] public string FirstVerse { get; set; } = "";
                public long YearComposed { get; set; }
            }

            // Test Scenario: [LengthIsBetween] Annotation Applied to Non-String Field {error}
            public class Capacitor {
                [PrimaryKey] public uint ID { get; set; }
                [Check.LengthIsBetween(4, 9)] public float Capacitance { get; set; }
                public double PlateArea { get; set; }
                public double Dielectric { get; set; }
            }

            // Test Scenario: <Path> Provided for [LengthIsBetween] Annotation on Scalar Property {error}
            public class SetCard {
                [PrimaryKey] public byte Count { get; set; }
                public string Color { get; set; } = "";
                [Check.LengthIsBetween(3, 13, Path = "---")] public string Pattern { get; set; } = "";
            }

            // Test Scenario: Multiple [LengthIsBetween] Annotations {error}
            public class Aria {
                [PrimaryKey] public string Title { get; set; } = "";
                public string SourceOpera { get; set; } = "";
                public ushort Length { get; set; }
                public uint WordCount { get; set; }
                [Check.LengthIsBetween(1, 100), Check.LengthIsBetween(27, 4999)] public string Lyrics { get; set; } = "";
                public string VocalRange { get; set; } = "";
            }

            // Test Scenario: String Length Annotation on Property Data-Converted to Text Type
            public class AesSedai {
                [PrimaryKey] public string Name { get; set; } = "";
                public bool IsAlive { get; set; }
                public ushort Mentions { get; set; }
                public float Power { get; set; }
                [Check.LengthIsBetween(1, 15), DataConverter(typeof(IntToString))] public int? Ajah { get; set; }
            }

            // Test Scenario: String Length Annotation on Property Data-Converted from Text Type {error}
            public class Campfire {
                [PrimaryKey] public Guid GUID { get; set; }
                public DateTime Started { get; set; }
                public DateTime? Fizzled { get; set; }
                public double Temperature { get; set; }
                [DataConverter(typeof(TextToOther<byte>)), Check.LengthIsAtLeast(4)] public string WoodType { get; set; } = "";
            }

            // Test Scenario: Property Annotated with Both [IsNonEmpty] and [LengthIsAtLeast] {error}
            public class StepPyramid {
                [PrimaryKey] public Guid ID { get; set; }
                public ulong Steps { get; set; }
                public bool IsZiggurat { get; set; }
                [Check.IsNonEmpty, Check.LengthIsAtLeast(5)] public string? KnownAs { get; set; }
                public ushort? ApproximateAge { get; set; }
                public double Latitude { get; set; }
                public double Longitude { get; set; }
            }

            // Test Scenario: Property Annotated with Both [IsNonEmpty] and [LengthIsBetween] {error}
            public class Cave {
                [PrimaryKey] public float Latitude { get; set; }
                [PrimaryKey] public float Longitude { get; set; }
                [Check.IsNonEmpty, Check.LengthIsBetween(75412, 12981147)] public string Name { get; set; } = "";
                public double Length { get; set; }
                public byte Entrances { get; set; }
                public double Depth { get; set; }
            }

            // Test Scenario: Property Annotated with Both [LengthIsAtLeast] and [LengthIsBetween] {error}
            public class Integral {
                [PrimaryKey] public string From { get; set; } = "";
                [PrimaryKey] public string To { get; set; } = "";
                [PrimaryKey, Check.LengthIsAtLeast(555), Check.LengthIsBetween(3, 22)] public string Expression { get; set; } = "";
                [PrimaryKey] public string WithRespectTo { get; set; } = "";
            }

            // Test Scenario: Property Annotated with Both [LengthIsAtMost] and [LengthIsBetween] {error}
            public class Isthmus {
                [PrimaryKey] public Guid ID { get; set; }
                [Check.LengthIsAtMost(413), Check.LengthIsBetween(22, 1994)] public string Name { get; set; } = "";
                public ulong Length { get; set; }
                public bool IsSpannedByCanal { get; set; }
            }

                    // ~~~~~ Inclusion Constraints ~~~~~

            // Test Scenario: [IsOneOf] Annotation with Multiple Options
            public class Astronaut {
                [PrimaryKey] public string AstronautName { get; set; } = "";
                public short MinutesInSpace { get; set; }
                [Check.IsOneOf(0u, 1u, 2u, 3u, 4u, 5u)] public uint NumSpacewalks { get; set; }
                public DateTime Retirement { get; set; }
                [Check.IsOneOf("Apollo", "Gemini", "Mercury", "Artemis")] public string MaidenProgram { get; set; } = "";
                [Check.IsOneOf(true, false)] public bool WalkedOnMoon { get; set; }
            }

            // Test Scenario: [IsOneOf] Annotation with Single Option
            public class Hospital {
                [PrimaryKey] public string Address { get; set; } = "";
                public ulong NumBeds { get; set; }
                [Check.IsOneOf("2000-01-01")] public DateTime Opened { get; set; }
                public bool HasTraumaWard { get; set; }
                public ulong StaffCount { get; set; }
            }

            // Test Scenario: [IsOneOf] Annotation with Duplicated Option
            public class HealingPotion {
                [PrimaryKey] public string Variety { get; set; } = "";
                public byte Rolls { get; set; }
                [Check.IsOneOf(4u, 8u, 10u, 12u, 20u, 100u, 8u, 12u, 8u, 4u, 8u)] public uint DieType { get; set; }
                public sbyte Plus { get; set; }
                public ulong Uses { get; set; }
            }

            // Test Scenario: [IsOneOf] Includes Null Option on Nullable Field {error}
            public class Prophet {
                [PrimaryKey] public string EnglishName { get; set; } = "";
                [Check.IsOneOf("Mosheh", "Noach", null!, "Av'raham", "Yizhak")] public string? HebrewName { get; set; }
                public string? LatinName { get; set; }
                public string? ArabicName { get; set; }
                public bool SpeaksToGod { get; set; }
                public long AgeAtDeath { get; set; }
            }

            // Test Scenario: [IsOneOf] Includes Null Option on Non-Nullable Field {error}
            public class CoralReef {
                [PrimaryKey] public string Name { get; set; } = "";
                public float Latitude { get; set; }
                [Check.IsOneOf(0.0f, 30f, 45f, null!, 75f, 90f)] public float Longitude { get; set; }
                public ulong Length { get; set; }
                public ulong Area { get; set; }
                public double SCUBARating { get; set; }
                public bool IsSaltwater { get; set; }
            }

            // Test Scenario: [IsOneOf] Annotation with Invalid and Unconvertible Option {error}
            public class Battery {
                [PrimaryKey] public Guid ProductID { get; set; }
                public string Cathode { get; set; } = "";
                public string Anode { get; set; } = "";
                [Check.IsOneOf(1, 2, 3, 4, 5, "six", 7, 8, 9)] public int Voltage { get; set; }
            }

            // Test Scenario: [IsOneOf] Annotation with Invalid but Convertible Option {error}
            public class TennisMatch {
                [PrimaryKey] public Guid MatchIdentifier { get; set; }
                public string Player1 { get; set; } = "";
                public string Player2 { get; set; } = "";
                [Check.IsOneOf((byte)0, (byte)15, (byte)30, (byte)40)] public sbyte Player1Score { get; set; }
                public sbyte Player2Score { get; set; }
                public string? Tournament { get; set; }
            }

            // Test Scenario: Single-Element Array of Correct as Anchor to [IsOneOf] Annotation {error}
            public class Flashcard {
                [PrimaryKey] public Guid DeckID { get; set; }
                [PrimaryKey] public int FlaschardNumber { get; set; }
                public string Front { get; set; } = "";
                public string Back { get; set; } = "";
                [Check.IsOneOf(true, new[] { false })] public bool IsLearned { get; set; }
            }

            // Test Scenario: <Path> Provided for [IsOneOf] Annotation on Scalar Property {error}
            public class HomericHymn {
                [PrimaryKey] public string To { get; set; } = "";
                [Check.IsOneOf(1, 5, 25, 50, Path = "---")] public int Lines { get; set; }
                public string OriginalGreekText { get; set; } = "";
                public string EnglishTranslation { get; set; } = "";
            }

            // Test Scenario: Multiple [IsOneOf] Annotations {error}
            public class Cannon {
                [PrimaryKey] public Guid GUID { get; set; }
                public float Weight { get; set; }
                [Check.IsOneOf(7, 2, 4, 1), Check.IsOneOf(2, 6)] public int Capacity { get; set; }
                public bool IsAutomatic { get; set; }
            }

            // Test Scenario: [IsNotOneOf] Annotation with Multiple Options
            public class NationalAnthem {
                [PrimaryKey] public string Endonym { get; set; } = "";
                public string Exonym { get; set; } = "";
                public string Language { get; set; } = "";
                public string ForCountry { get; set; } = "";
                public uint WordCount { get; set; }
                [Check.IsNotOneOf(1.3f, 1.6f, 1.9f, 2.2f, 2.5f, 2.8f, 3.1f, 3.4f)] public float Length { get; set; }
            }

            // Test Scenario: [IsNotOneOf] Annotation with Single Option
            public class GamingConsole {
                [PrimaryKey] public Guid SerialNumber { get; set; }
                [Check.IsNotOneOf("Atari 2600")] public string Name { get; set; } = "";
                public string? AKA { get; set; }
                [Check.IsNotOneOf("1973-04-30")] public DateTime Launched { get; set; }
                public DateTime? Retired { get; set; }
                public ulong UnitsSold { get; set; }
                public float CPUClockCycle { get; set; }
            }

            // Test Scenario: [IsNotOneOf] Annotation with Duplicated Option
            public class Tweet {
                [PrimaryKey] public Guid TweetID { get; set; }
                public string Text { get; set; } = "";
                public uint PosterID { get; set; }
                [Check.IsNotOneOf('A', 'E', 'I', 'E', 'U', 'A', 'O', 'U', 'I')] public char Grading { get; set; }
                public ulong Likes { get; set; }
                public ulong Retweets { get; set; }
                public ulong Favorites { get; set; }
            }

            // Test Scenario: [IsNotOneOf] Includes Null Option on Nullable Field {error}
            public class Ballet {
                [PrimaryKey] public string BalletName { get; set; } = "";
                public string Composer { get; set; } = "";
                public ushort Length { get; set; }
                public uint NumDancers { get; set; }
                [Check.IsNotOneOf(null!, 1, 3, 5, 7, 9)] public int? OpusNumber { get; set; }
            }

            // Test Scenario: [IsNotOneOf] Includes Null Option on Non-Nullable Field {error}
            public class PIERoot {
                [PrimaryKey, Check.IsNotOneOf(null!)] public string Root { get; set; } = "";
                public string Meaning { get; set; } = "";
                public string? FrenchExample { get; set; }
                public string? SpanishExample { get; set; }
                public string? RussianExample { get; set; }
                public string? GaelicExample { get; set; }
                public string? GreekExample { get; set; }
            }

            // Test Scenario: [IsNotOneOf] Annotation with Invalid and Unconvertible Option {error}
            public class Cancer {
                [PrimaryKey] public uint DiseaseID { get; set; }
                public string Name { get; set; } = "";
                public double MortalityRate { get; set; }
                [Check.IsNotOneOf("Spleen", "Earlobe", 17.3f, "Elbow")] public string RegionAffected { get; set; } = "";
                public bool HereditarilyMarked { get; set; }
            }

            // Test Scenario: [IsNotOneOf] Annotation with Invalid but Convertible Option {error}
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

            // Test Scenario: Single-Element Array of Correct as Anchor to [IsNotOneOf] Annotation {error}
            public class Wristwatch {
                [PrimaryKey] public Guid ID { get; set; }
                [Check.IsNotOneOf("Rolex", "Cartier", new[] { "Tag Heuer" })] public string Brand { get; set; } = "";
                public decimal MarketValue { get; set; }
                public bool IsAdjustable { get; set; }
                public bool CanStopwatch { get; set; }
            }

            // Test Scenario: <Path> Provided for [IsNotOneOf] Annotation on Scalar Property {error}
            public class Donut {
                [PrimaryKey] public Guid DonutID { get; set; }
                [Check.IsNotOneOf("Strawberry", "", "Unknown", Path = "---")] public string Flavor { get; set; } = "";
                public bool HasSprinkles { get; set; }
                public bool IsFilled { get; set; }
                public bool IsGlazed { get; set; }
                public bool IsDonutHole { get; set; }
            }

            // Test Scenario: Multiple [IsNotOneOf] Annotations {error}
            public class Eurovision {
                [PrimaryKey, Check.IsNotOneOf((ushort)3), Check.IsNotOneOf((ushort)0)] public ushort Year { get; set; }
                public uint ParticipatingCountries { get; set; }
                public string WinningCountry { get; set; } = "";
                public string WinningGroup { get; set; } = "";
                public string WinningSong { get; set; } = "";
            }

            // Test Scenario: Inclusion Annotation Anchor is Invalid for DateTime b/c of Formatting {error}
            public class Inator {
                public string Name { get; set; } = "";
                public bool StoppedByPerry { get; set; }
                [Check.IsNotOneOf("2018-07-03", "1875~06~22", "73-01-15")] public DateTime Debut { get; set; }
                public string Effects { get; set; } = "";
            }

            // Test Scenario: Inclusion Annotation Anchor is Invalid for DateTime b/c of Range {error}
            public class FinalJeopardy {
                [PrimaryKey, Check.IsOneOf("1299-08-45")] public DateTime AirDate { get; set; }
                public string Category { get; set; } = "";
                public string Clue { get; set; } = "";
                public string Answer { get; set; } = "";
                public decimal? Player1Wager { get; set; }
                public decimal? Player2Wager { get; set; }
                public decimal? Player3Wager { get; set; }
            }

            // Test Scenario: Inclusion Annotation Anchor is Invalid for DateTime b/c of Type {error}
            public class Mayor {
                [PrimaryKey] public string Name { get; set; } = "";
                [PrimaryKey] public string City { get; set; } = "";
                [PrimaryKey] public DateTime TermBegin { get; set; }
                [Check.IsNotOneOf('T')] public DateTime? TermEnd { get; set; }
                public string DeputyMayor { get; set; } = "";
            }

            // Test Scenario: Inclusion Annotation on Data-Converted Field
            public class WaterSlide {
                [PrimaryKey] public Guid SlideID { get; set; }
                public ushort Length { get; set; }
                public float HeightMinimum { get; set; }
                public double WeightMaximum { get; set; }
                [DataConverter(typeof(IntToString)), Check.IsOneOf("Straight", "Curly", "Funnel")] public int Type { get; set; }
                public bool IsTubeSlide { get; set; }
            }

            // Test Scenario: Inclusion Annotation with Valid Anchor for Conversion Source Type {error}
            public class SoccerTeam {
                [PrimaryKey] public string League { get; set; } = "";
                [PrimaryKey] public string Location { get; set; } = "";
                [PrimaryKey] public string Nickname { get; set; } = "";
                public ushort RosterSize { get; set; }
                [Check.IsNotOneOf(0, -3, 111), DataConverter(typeof(IntToString))] public int WorldCupVictories { get; set; }
                public string CurrentCoach { get; set; } = "";
                public string CurrentGoalie { get; set; } = "";
            }

            // Test Scenario: Property Annotated with Both [IsOneOf] and [IsNotOneOf] {error}
            public class SkiSlope {
                [PrimaryKey] public Guid ID { get; set; }
                public uint Height { get; set; }
                [Check.IsOneOf("Black Diamond", "Novice"), Check.IsNotOneOf("Unknown")] public string Level { get; set; } = "";
                public string SkiResort { get; set; } = "";
                public bool BeginnerFriendly { get; set; }
            }

                    // ~~~~~ Custom CHECK Constraint ~~~~~

            // Test Scenario: Custom [Check] Annotation with No Constructor Arguments
            public class VampireSlayer {
                [PrimaryKey] public string Name { get; set; } = "";
                public uint Appearances { get; set; }
                public uint StakesUsed { get; set; }
                [Check(typeof(CustomCheck))] public ushort Deaths { get; set; }
                public bool ActivatedByScythe { get; set; }
            }

            // Test Scenario: Custom [Check] Annotation with Constructor Arguments
            public class Lyric {
                [PrimaryKey] public string SongTitle { get; set; } = "";
                [PrimaryKey] public int LineNumber { get; set; }
                public string Lyrics { get; set; } = "";
                [Check(typeof(CustomCheck), 13, false, "ABC", null)] public bool IsSpoken { get; set; }
                public bool IsChorus { get; set; }
            }

            // Test Scenario: Custom [Check] Annotation on Data-Converted Property
            public class DataStructure {
                [PrimaryKey] public string Name { get; set; } = "";
                public string SearchBigO { get; set; } = "";
                public string InsertBigO { get; set; } = "";
                [Check(typeof(CustomCheck)), DataConverter(typeof(TextToOther<sbyte>))] public string RemoveBigO { get; set; } = "";
                public bool IsOrdered { get; set; }
                public bool IsAssociative { get; set; }
                public bool IsContiguous { get; set; }
            }

            // Test Scenario: Multiple [Check] Annotations on One Property
            public class TarotCard {
                [PrimaryKey] public int DeckID { get; set; }
                [PrimaryKey] public ushort CardNumber { get; set; }
                [Check(typeof(CustomCheck)), Check(typeof(CustomCheck), -14, '%')] public byte Pips { get; set; }
                public string Character { get; set; } = "";
            }

            // Test Scenario: Constraint Generator Type is not a Constraint Generator {error}
            public class Patreon {
                [PrimaryKey] public string URL { get; set; } = "";
                public string Creator { get; set; } = "";
                public decimal Tier1 { get; set; }
                public decimal Tier2 { get; set; }
                [Check(typeof(IWebProtocol))] public decimal Tier3 { get; set; }
            }

            // Test Scenario: Constraint Generator Type Cannot be Constructed {error}
            public class Transistor {
                [PrimaryKey] public Guid ID { get; set; }
                public string Model { get; set; } = "";
                [Check(typeof(CustomCheck), "Dopant", 4)] public string? Dopant { get; set; }
                public float Transconductance { get; set; }
                public int OperatingTemperature { get; set; }
            }

            // Test Scenario: Error Thrown while Constructing Constraint Generator {error}
            public class BasketballPlayer {
                [PrimaryKey] public string Name { get; set; } = "";
                public ulong Points { get; set; }
                public float Pct3Pointer { get; set; }
                [Check(typeof(CustomCheck), false)] public ulong Rebounds { get; set; }
                public ulong Steals { get; set; }
                public ulong Assists { get; set; }
            }

            // Test Scenario: <Path> Provided for [Check] Annotation on Scalar Property {error}
            public class TarPits {
                [PrimaryKey] public string TarPitsName { get; set; } = "";
                public float Area { get; set; }
                [Check(typeof(CustomCheck), Path = "---")] public string FirstFossil { get; set; } = "";
                public bool IsNationalArea { get; set; }
                public double Latitude { get; set; }
                public double Longitude { get; set; }
            }

                    // ~~~~~ Complex CHECK Constraint ~~~~~

            // Test Scenario: [Complex] Annotation with No Constructor Arguments
            [Check.Complex(typeof(CustomCheck), new[] { "FirstLine" })]
            public class CanterburyTale {
                [PrimaryKey] public int Index { get; set; }
                public string Whose { get; set; } = "";
                public string FirstLine { get; set; } = "";
                public ulong WordCount { get; set; }
            }

            // Test Scenario: [Complex] Annotation with Constructor Arguments
            [Check.Complex(typeof(CustomCheck), new[] { "ConclaveRounds" }, -93, true, 'X')]
            public class Pope {
                [PrimaryKey] public string PapalName { get; set; } = "";
                [PrimaryKey] public uint PapalNumber { get; set; }
                public DateTime Elected { get; set; }
                public DateTime? Ceased { get; set; }
                public uint ConclaveRounds { get; set; }
                public string FirstEncyclical { get; set; } = "";
            }

            // Test Scenario: [Complex] Annotation with Multiple Distinct Fields
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

            // Test Scenario: [Complex] Annotation with Single Field Multiple Times
            [Check.Complex(typeof(CustomCheck), new[] {"Name", "Name", "Name", "Name"})]
            public class Muppet {
                [PrimaryKey] public string Name { get; set; } = "";
                public DateTime Debut { get; set; }
                public string Puppeteer { get; set; } = "";
                public ushort MuppetsShowAppearances { get; set; }
                public ushort MuppetsFilmAppearances { get; set; }
            }

            // Test Scenario: [Complex] Annotation over Name-Swapped Fields
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

            // Test Scenario: [Complex] Annotation over Data-Converted Field
            [Check.Complex(typeof(CustomCheck), new[] { "When", "Casualties", "When" })]
            public class Massacre {
                [PrimaryKey] public string Name { get; set; } = "";
                public ulong Casualties { get; set; }
                public bool WarCrime { get; set; }
                [DataConverter(typeof(Nullify<DateTime>))] public DateTime When { get; set; }
            }

            // Test Scenario: Multiple [Complex] Annotations on One Entity Type
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

            // Test Scenario: Constraint Generator Type is not a Constraint Generator {error}
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

            // Test Scenario: Constraint Generator Type Cannot be Constructed {error}
            [Check.Complex(typeof(CustomCheck), new[] { "Omega3s", "Omega6s" }, "O", 'I', 'L', '!')]
            public class CookingOil {
                [PrimaryKey] public string Type { get; set; } = "";
                public decimal SmokePoint { get; set; }
                public double TransFats { get; set; }
                public double Omega3s { get; set; }
                public double Omega6s { get; set; }
            }

            // Test Scenario: Error Thrown while Constructing Constraint Generator {error}
            [Check.Complex(typeof(CustomCheck), new[] { "Born", "Died" }, true)]
            public class Pirate {
                [PrimaryKey] public string PirateName { get; set; } = "";
                [PrimaryKey] public string LandName { get; set; } = "";
                public string Flagship { get; set; } = "";
                public DateTime? Born { get; set; }
                public DateTime? Died { get; set; }
                public ulong Plunder { get; set; }
                public bool IsFictional { get; set; }
            }

            // Test Scenario: [Complex] Annotation over Zero Fields {error}
            [Check.Complex(typeof(CustomCheck), new string[] {})]
            public class Terminator {
                [PrimaryKey] public string Model { get; set; } = "";
                [PrimaryKey] public ushort Number { get; set; }
                public ulong KillCount { get; set; }
                public string Portrayer { get; set; } = "";
                public DateTime FirstAppearance { get; set; }
            }

            // Test Scenario: [Complex] Annotation over Name-Changed Field with Original Name {error}
            [Check.Complex(typeof(CustomCheck), new string[] { "Width" })]
            public class Dam {
                [PrimaryKey] public Guid ID { get; set; }
                public string Name { get; set; } = "";
                public ulong Height { get; set; }
                [Name("WidhtAtBase")] public ulong Width { get; set; }
                public ulong Volume { get; set; }
                public string ImpoundedRiver { get; set; } = "";
                public bool IsHydroelectric { get; set; }
            }

            // Test Scenario: [Complex] Annotation over Unrecognized Field Name {error}
            [Check.Complex(typeof(CustomCheck), new string[] { "Belligerents" })]
            public class PeaceTreaty {
                [PrimaryKey] public string TreatyName { get; set; } = "";
                public DateTime Signed { get; set; }
                public DateTime Effective { get; set; }
                public string Text { get; set; } = "";
                public ushort NumSignatories { get; set; }
            }
    }
}
