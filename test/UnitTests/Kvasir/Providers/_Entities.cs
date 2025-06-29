using Kvasir.Annotations;
using Kvasir.Relations;
using System;

namespace UT.Kvasir.Providers {
    internal static class MySql {
        // Scenario: CREATE TABLE – Principal Table
        public class Mutiny {
            [PrimaryKey, Column(0)] public DateTime Date { get; set; }
            [PrimaryKey, Column(1)] public string Ship { get; set; } = "";
            [Column(2)] public string? LeadMutineer { get; set; }
            [Column(3)] public string OustedCaptain { get; set; } = "";
            [Column(4)] public uint Casualties { get; set; }
        }

        // Scenario: CREATE TABLE – Relation Table
        public class CyrillicLetter {
            [PrimaryKey, Column(0)] public string LetterName { get; set; } = "";
            public RelationMap<ulong, char> Glyphs { get; } = new();
            [Column(1)] public ushort NumericalValue { get; set; }
            [Column(2)] public bool IsVowel { get; set; }
        }

        // Scenario: CREATE TABLE – Pre-Defined Entity Table
        [PreDefined] public class BackstreetBoy {
            [PrimaryKey, Column(0)] public string FirstName { get; private init; }
            [PrimaryKey, Column(1)] public string LastName { get; private init; }
            [Column(2)] public DateTime Birthdate { get; private init; }

            public static BackstreetBoy AJ { get; } = new BackstreetBoy("AJ", "McLean", 1, 9, 1978);
            public static BackstreetBoy Howie { get; } = new BackstreetBoy("Howie", "Dorough", 8, 22, 1973);
            public static BackstreetBoy Nick { get; } = new BackstreetBoy("Nick", "Carter", 1, 28, 1980);
            public static BackstreetBoy Kevin { get; } = new BackstreetBoy("Kevin", "Richardson", 10, 3, 1971);
            public static BackstreetBoy Brian { get; } = new BackstreetBoy("Brian", "Littrell", 2, 20, 1975);

            private BackstreetBoy(string first, string last, int birthMonth, int birthDay, int birthYear) {
                FirstName = first;
                LastName = last;
                Birthdate = new DateTime(birthYear, birthMonth, birthDay);
            }
        }

        // Scenario: SELECT * FROM – Principal Table
        public class CarDealership {
            [PrimaryKey, Column(0)] public Guid DealershipID { get; set; }
            [Column(1)] public string DealershipName { get; set; } = "";
            [Column(2)] public string Brand { get; set; } = "";
            [Column(3)] public string HeadDealer { get; set; } = "";
            [Column(4)] public uint CarsInInventory { get; set; }
            [Column(5)] public decimal MonthlyRevenue { get; set; }
            [Column(6)] public bool IsLuxury { get; set; }
        }

        // Scenario: SELECT * FROM – Relation Table
        public class Hadith {
            public enum Branch { Sunni, Shia, Ibadi }

            [PrimaryKey, Column(0)] public string Title { get; set; } = "";
            [Column(1)] public string Name { get; set; } = "";
            [Column(2)] public Branch IslamicBranches { get; set; }
            [Column(3)] public string ISBN { get; set; } = "";
            public IReadOnlyRelationSet<Branch> Branches { get; } = new RelationSet<Branch>();
        }

        // Scenario: SELECT * FROM – Calculated Field
        public class Grenade {
            public enum Kind { Fragmentation, AntiTank, Incendiary, MolotovCocktail, Chemical }

            [PrimaryKey, Column(0)] public Guid WeaponID { get; set; }
            [Column(1)] public Kind Variety { get; set; }
            [Column(2)] public bool HasBeenUsed { get; set; }
            [Column(3)] public double Volume { get; set; }
            [Column(4), Calculated] public double Lethality { get; set; }
        }

        // Scenario: INSERT INTO – Text and Enumerations
        public class ChipotleOrder {
            public enum MealType { Burrito, Bowl, Tacos, Salad }
            public enum BeansOption { None, Pinto, Black }
            public enum ProteinOption { None, Chicken, Steak, Carnitas, Barbacoa, Tofu, Speciality }
            [Flags] public enum SalsaOption { None = 0, Mild = 1, Medium = 2, Hot = 4, Questo = 8 }
            public enum CheeseOption { None, Single, Extra }
            public enum LettuceOption { None, Salad, AddOn }
            public enum GuacOption { None, Single, Extra, Perk, OnTheSide }

            [PrimaryKey, Column(0)] public string OrderId { get; set; } = "";
            [Column(1)] public MealType Kind { get; set; }
            [Column(2)] public BeansOption Beans { get; set; }
            [Column(3)] public ProteinOption Protein { get; set; }
            [Column(4)] public SalsaOption Salsa { get; set; }
            [Column(5)] public CheeseOption Cheese { get; set; }
            [Column(6)] public LettuceOption Lettuce { get; set; }
            [Column(7)] public GuacOption Guacamole { get; set; }
            [Column(8), Check.LengthIsAtMost(70)] public string? NameForOrder { get; set; }
            [Column(9)] public char CRC { get; set; }
        }

        // Scenario: INSERT INTO – Integers and Booleans
        public class RegularExpression {
            [Column(0)] public bool IsPerlCompatible { get; set; }
            [PrimaryKey, Column(1)] public string Expression { get; set; } = "";
            [Column(2)] public int NumCaptureGroups { get; set; }
            [Column(3)] public ushort? MaxMatchLength { get; set; }
            [Column(4)] public long NumWildcards { get; set; }
        }

        // Scenario: INSERT INTO – Floating Point Numerics
        public class Tariff {
            [PrimaryKey, Column(0)] public ulong ID { get; set; }
            [Column(1)] public string Importer { get; set; } = "";
            [Column(2)] public string Exporter { get; set; } = "";
            [Column(3)] public double RegularRate { get; set; }
            [Column(4)] public float DiscountRate { get; set; }
            [Column(5)] public bool IsInForce { get; set; }
            [Column(6)] public decimal Revenue { get; set; }
        }

        // Scenario: INSERT INTO – Date, DateTime, and GUID
        public class ExecutiveOrder {
            [PrimaryKey, Column(0)] public string President { get; set; } = "";
            [PrimaryKey, Column(1)] public ushort OrderNumber { get; set; }
            [Column(2)] public DateTime Issued { get; set; }
            [Column(3)] public DateOnly? Rescinded { get; set; }
            [Column(4)] public Guid DocumentID { get; set; }
            [Column(5)] public bool EnshrinedInLegislation { get; set; }
        }

        // Scenario: INSERT INTO – Null Value
        public class Codex {
            [PrimaryKey, Column(0)] public string Title { get; set; } = "";
            [Column(1)] public string Civilization { get; set; } = "";
            [Column(2)] public DateTime? Published { get; set; }
            [Column(3)] public double SurfaceArea { get; set; }
        }

        // Scenario: INSERT INTO – 2 Rows
        public class Choreographer {
            [PrimaryKey, Column(0)] public string Name { get; set; } = "";
            [PrimaryKey, Column(1)] public string Specialty { get; set; } = "";
            [Column(2)] public int FilmsChoreographed { get; set; }
            [Column(3)] public DateTime? WalkOfFameStar { get; set; }
        }

        // Scenario: INSERT INTO – 3+ Rows
        public class Nachos {
            public enum KindOfChip { Tostitos, Corn, BlueCorn }
            public enum Role { Appetizer, Breakfast, Lunch, Dinner, Dessert, Snack }

            [PrimaryKey, Column(0)] public Guid NachosID { get; set; }
            [Column(1)] public KindOfChip Chips { get; set; }
            [Column(2)] public double Calories { get; set; }
            [Column(3)] public sbyte TypesOfCheese { get; set; }
            [Column(4)] public bool Beans { get; set; }
            [Column(5)] public bool Salsa { get; set; }
            [Column(6)] public bool Guacamole { get; set; }
            [Column(7)] public Role FoodRole { get; set; }
            [Column(8)] public string Chef { get; set; } = "";
        }

        // Scenario: INSERT INTO – Relation Table
        public class Manicure {
            public enum Hand { Left, Right }
            public enum Position { Thumb, Index, Middle, Ring, Pinkie }
            public record struct Finger(Hand Hand, Position Position);

            [PrimaryKey, Column(0)] public string Manicurist { get; set; } = "";
            [PrimaryKey, Column(1)] public string Manicuree { get; set; } = "";
            [PrimaryKey, Column(2)] public DateTime Timestamp { get; set; }
            public RelationMap<Finger, bool> WasPainted { get; } = new();
            [Column(3)] public bool ManiPedi { get; set; }
            [Column(4)] public decimal Cost { get; set; }
        }

        // Scenario: UPDATE – 1 Row of Principal Table with Single-Field Primary Key
        public class SignLanguage {
            [PrimaryKey, Column(0)] public string ISO6393 { get; set; } = "";
            [Column(1)] public string Name { get; set; } = "";
            [Column(2)] public ulong NativeSigners { get; set; }
            [Column(3)] public ulong L2Signers { get; set; }
            [Column(4)] public bool SingleHandFingerspelling { get; set; }
        }

        // Scenario: UPDATE – 1 Row of Principal Table with Multi-Field Primary Key
        public class Diaspora {
            [PrimaryKey, Column(0)] public string Ethnicity { get; set; } = "";
            [PrimaryKey, Column(1)] public string ExogenousCountry { get; set; } = "";
            [Column(2)] public ulong Population { get; set; }
            [Column(3)] public string DiasporicTerm { get; set; } = "";
            [Column(4)] public bool PrimarilyRefugees { get; set; }
        }

        // Scenario: UPDATE – 1 Row of Principal Table with All-Fields Primary Key
        public class Click {
            [PrimaryKey, Column(0)] public string Description { get; set; } = "";
            [PrimaryKey, Column(1)] public char IPA { get; set; }
            [PrimaryKey, Column(2)] public string Onomatopoeia { get; set; } = "";
        }

        // Scenario: UPDATE – 2 Rows
        public class CutthroatKitchenSabotage {
            public enum Round { Round1, Round2, Round3 };

            [PrimaryKey, Column(0)] public sbyte Season { get; set; }
            [PrimaryKey, Column(1)] public sbyte Episode { get; set; }
            [PrimaryKey, Column(2)] public Round WhichRound { get; set; }
            [PrimaryKey, Column(3)] public sbyte SabotageNumber { get; set; }
            [Column(4)] public string Sabotage { get; set; } = "";
            [Column(5)] public decimal WinningBid { get; set; }
        }

        // Scenario: UPDATE – 3+ Rows
        public class WelshGod {
            [Column(0)] public string Name { get; set; } = "";
            [PrimaryKey, Column(1)] public Guid DeityID { get; set; }
            [Column(2)] public string Domain { get; set; } = "";
            [Column(3)] public ulong MabinogionMentions { get; set; }
        }

        // Scenario: UPDATE – Null Value
        public class Curry {
            public enum Color { Red, Yellow, White, Orange, Green }
            public enum Origin { Indian, Thai, Nepalese, Japan, Myanmar, Malaysia, Veitnam, UK, Cambodia, Other }

            [PrimaryKey, Column(0)] public Guid CurryID { get; set; }
            [Column(1)] public Color? CurryColor { get; set; }
            [Column(2)] public Origin CountryOfOrigin { get; set; }
            [Column(3)] public string? Protein { get; set; }
            [Column(4)] public bool IsVegetarian { get; set; }
            [Column(5)] public byte SpiceLevel { get; set; }
        }

        // Scenario: UPDATE – Non-Associative Relation Table
        public class Overture {
            public record struct Instrumentation(string Instrument, sbyte Count);

            [PrimaryKey, Column(0)] public string Composer { get; set; } = "";
            [PrimaryKey, Column(1)] public int OpusNumber { get; set; }
            public IReadOnlyRelationSet<Instrumentation> Instruments { get; } = new RelationSet<Instrumentation>();
            [Column(2)] public DateTime Premiere { get; set; }
            [Column(3)] public ushort NumSecondsLong { get; set; }
            [Column(4)] public string? CommonTitle { get; set; }
        }

        // Scenario: UPDATE – Single-Key Associative Relation Table
        public class Supermodel {
            public enum Color { Black, Brown, Red, Blonde, DirtyBlonde, White, Other }

            [PrimaryKey, Column(0)] public string Name { get; set; } = "";
            [Column(1)] public DateTime Birthdate { get; set; }
            [Column(2)] public float Height { get; set; }
            [Column(3)] public Color HairColor { get; set; }
            [Column(4)] public bool HasBeenSICoverModel { get; set; }
            [Column(5)] public decimal NetWorth { get; set; }
            public RelationOrderedList<string> Agencies { get; } = new();
        }

        // Scenario: UPDATE – Multi-Key Associative Relation Table
        public class StockIndex {
            public record struct Stock(string Symbol, DateTime Listed);

            [PrimaryKey, Column(0)] public string IndexName { get; set; } = "";
            [Column(1)] public decimal MarketCap { get; set; }
            [Column(2)] public string? NYSESymbol { get; set; }
            public RelationMap<Stock, float> Constituents { get; } = new();
            [Column(3)] public double YearOverYear { get; set; }
        }

        // Scenario: DELETE – 1 Row from Principal Table with Single-Field Primary Key
        public class Mansa {
            [Column(0)] public string Name { get; set; } = "";
            [PrimaryKey, Column(1)] public uint Index { get; set; }
            [Column(2)] public DateTime Birth { get; set; }
            [Column(3)] public DateTime Coronation { get; set; }
            [Column(4)] public DateTime Death { get; set; }
            [Column(5)] public string? RoyalHouse { get; set; }
        }

        // Scenario: DELETE – 1 Row from Principal Table with Multi-Field Primary Key
        public class Hamentaschen {
            [Flags] public enum Ingredient { Chocolate = 1, GrapeJelly = 2, StrawberryJelly = 4, Nutella = 8, ApricotJam = 16, Marshmallow = 32, RaspberryJam = 64 }

            [PrimaryKey, Column(0)] public ushort Year { get; set; }
            [Column(1)] public Ingredient Filling { get; set; }
            [PrimaryKey, Column(2)] public uint BatchNumber { get; set; }
            [Column(3)] public bool IsGlutenFree { get; set; }
            [PrimaryKey, Column(4)] public ushort CookieNumber { get; set; }
            [PrimaryKey, Column(5)] public Guid CookieMakerID { get; set; }
        }

        // Scenario: DELETE – 1 Row from Principal Table with All-Fields Primary Key
        public class Tirthankara {
            [PrimaryKey, Column(0)] public string Name { get; set; } = "";
            [PrimaryKey, Column(1)] public string Emblem { get; set; } = "";
            [PrimaryKey, Column(2)] public uint Iteration { get; set; }
        }

        // Scenario: DELETE – 2 Rows
        public class FoodTruck {
            public enum Style { American, Chinese, Mexican, Seafood, Indian, Thai, Korean, Japanese, German, Italian, Fusion, Other }

            [PrimaryKey, Column(0)] public string TruckName { get; set; } = "";
            [PrimaryKey, Column(1)] public string Proprietor { get; set; } = "";
            [Column(2)] public Style Cuisine { get; set; }
            [Column(3)] public Guid VehicleID { get; set; }
            [Column(4)] public decimal AnnualRevenue { get; set; }
            [Column(5)] public ushort MenuSize { get; set; }
        }

        // Scenario: DELETE – 3+ Rows
        public class Iyalet {
            [Column(0)] public string Domain { get; set; } = "";
            [Column(1)] public uint Members { get; set; }
            [PrimaryKey, Column(2)] public string Name { get; set; } = "";
            [Column(3)] public string? Cozulate { get; set; }
        }

        // Scenario: DELETE – Non-Associative Row from Relation Table
        public class BuzzerSystem {
            [PrimaryKey, Column(0)] public Guid BuzzerID { get; set; }
            [Column(1)] public byte NumBoxes { get; set; }
            [Column(2)] public bool UsesPlungers { get; set; }
            public IReadOnlyRelationSet<ushort> HSNCTs { get; } = new RelationSet<ushort>();
            [Column(3)] public Guid? BuzzNoiseID { get; set; }
        }

        // Scenario: DELETE – Single-Key Associative Row from Relation Table
        public class LineDance {
            public enum Move { ForwardStep, BackStep, LeftStep, RightStep, Kick, Shuffle, Shimmy, Jump, FullSpin, HalfSpin, QuarterSpin }

            [PrimaryKey, Column(0)] public string Song { get; set; } = "";
            [PrimaryKey, Column(1)] public int Discriminator { get; set; }
            [Column(2)] public byte Count { get; set; }
            public IReadOnlyRelationOrderedList<Move> Steps { get; } = new RelationOrderedList<Move>();
            [Column(3)] public bool IsPartnerDance { get; set; }
            [Column(4)] public string? YouTubeURL { get; set; }
        }

        // Scenario: DELETE – Multi-Key Associative Row from Relation Table
        public class CochlearImplant {
            public record struct AudioExam(string Title, float Version);

            [PrimaryKey, Column(0)] public string Individual { get; set; } = "";
            [Column(1)] public bool AudiologistRecommended { get; set; }
            [PrimaryKey, Column(2)] public bool LeftSide { get; set; }
            [Column(3)] public double RecoveryPercent { get; set; }
            [Column(4)] public DateTime Istallation { get; set; }
            [PrimaryKey, Column(5)] public uint Iteration { get; set; }
            public RelationMap<AudioExam, double> ExamResults { get; } = new();
        }

        // Scenario: DELETE – All Rows of an Owning Entity from Non-Associative Relation Table
        public class CongaLine {
            public enum Place { Cruise, Wedding, BneiMitzvah, GraduationParty, Prom, Funeral, SportingEvent, Other }

            [Column(0)] public Place Venue { get; set; }
            [Column(1)] public double Duration { get; set; }
            [PrimaryKey, Column(2)] public int DanceID { get; set; }
            public IReadOnlyRelationSet<string> Participants { get; } = new RelationSet<string>();
        }

        // Scenario: DELETE – All Rows of an Owning Entity from Single-Key Associative Relation Table
        public class Rodeo {
            [PrimaryKey, Column(0)] public Guid RodeoID { get; set; }
            [Column(1)] public string Location { get; set; } = "";
            [Column(2)] public string Promoter { get; set; } = "";
            [Column(3)] public string Bull { get; set; } = "";
            public RelationMap<string, float> Bullriders { get; } = new();
            [Column(4)] public decimal Revenue { get; set; }
        }

        // Scenario: Delete – All Rows of an Owning Entity from Multi-Key Associative Relation Table
        public class Surrogate {
            public enum System { Imperial, Metric }
            public record struct Thing(string Key, System Culture0);

            [PrimaryKey, Column(0)] public Guid PersonID { get; set; }
            [PrimaryKey, Column(1)] public string Agency { get; set; } = "";
            [Column(2)] public string Name { get; set; } = "";
            [Column(3)] public ushort ChildrenBirthed { get; set; }
            [Column(4)] public DateTime FirstPregnancy { get; set; }
            public RelationMap<Thing, double> BioStats { get; } = new();
            [Column(5)] public decimal Cost { get; set; }
        }

        // Scenario: DELETE – Mixed Style (Both By Row and By Owning Entity) from Relation Table
        public class AbortionClinic {
            [PrimaryKey, Column(0)] public Guid ID { get; set; }
            [Column(1)] public bool IsPlannedParenthood { get; set; }
            public RelationSet<string> Doctors { get; } = new();
            [Column(2)] public uint AbortionsPerformed { get; set; }
            [Column(3)] public decimal Budget { get; set; }
            [Column(4)] public string Address { get; set; } = "";
        }
    }
}
