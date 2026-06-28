using Kvasir.Annotations;
using Kvasir.Localization;
using Kvasir.Relations;
using System;

using static UT.Kvasir.Translation.TestLocalizations;

namespace UT.Kvasir.Transaction {
    internal static class TableCreation {
        // Test Scenario: Single Entity without Relations
        public class PoliticalRally {
            [PrimaryKey] public Guid ID { get; set; }
            public string Candidate { get; set; } = "";
            public DateTime Date { get; set; }
            public ushort Attendance { get; set; }
            public bool IsNationalConvention { get; set; }
            public string KeynoteSpeaker { get; set; } = "";
        }

        // Test Scenario: Single Entity with Scalar Relations
        public class Yogurt {
            public enum Fruit { Cherry, Banana, Orange, Strawberry, Raspberry, Blueberry, Lemon, Passionfruit }
            public enum Trait { Cultured, NonFat, LowFat, Organic, DairyFree }

            [PrimaryKey] public Guid ProductID { get; set; }
            public string Brand { get; set; } = "";
            public Fruit Flavor { get; set; }
            public RelationMap<string, double> NutritionInfo { get; init; } = [];
            public RelationSet<Trait> Traits { get; init; } = [];
        }

        // Test Scenario: Single Localization
        public class Nickname : Localization<string, Language, string> {
            public Nickname(string key) : base(key) {}
        }

        // Test Scenario: Multiple Unrelated Entities
        public class Gondola {
            [PrimaryKey] public Guid BoatID { get; set; }
            public double Length { get; set; }
            public bool Venetian { get; set; }
            public byte MaxCapacity { get; set; }
            public string? MainGondolier { get; set; }
        }
        public class Scooter {
            public enum Kind { Electric, Mobility, Segway, Water }

            [PrimaryKey] public Guid ProductID { get; set; }
            public decimal Price { get; set; }
            public float TopSpeed { get; set; }
            public Kind Variety { get; set; }
            public bool IsSingleRider { get; set; }
            public int CasualtiesCaused { get; set; }
        }
        public class Muffin {
            [PrimaryKey] public Guid MuffinID { get; set; }
            public string Flavor { get; set; } = "";
            public string SoldBy { get; set; } = "";
            public decimal Price { get; set; }
            public uint Calories { get; set; }
            public bool InBasket { get; set; }
            public bool FromBran { get; set; }
        }

        // Test Scenario: Multiple Unrelated Localizations
        public class Dosage : Localization<string, bool, Measurement> {
            public Dosage(string key) : base(key) {}
        }
        public class GUID : Localization<string, int, string> {
            public GUID(string key) : base(key) {}
        }

        // Test Scenario: Multiple Entities Related via Reference Chain
        public class EpicRapBattle {
            public class Actor {
                [PrimaryKey] public ulong IMDBNumber { get; set; }
                public string FirstName { get; set; } = "";
                public string LastName { get; set; } = "";
                public DateTime DateOfBirth { get; set; }
            }
            public class Rapper {
                [PrimaryKey] public Guid PersonID { get; set; }
                public string Name { get; set; } = "";
                public bool IsRealPerson { get; set; }
                public Actor Portrayer { get; set; } = new();
                public string Lines { get; set; } = "";
            }

            [PrimaryKey] public uint EpisodeNumber { get; set; }
            public string YouTubeURL { get; set; } = "";
            public Rapper Contestant1 { get; set; } = new();
            public Rapper Contestant2 { get; set; } = new();
            public DateTime OriginalAirdate { get; set; }
        }

        // Test Scenario: Multiple Entities Related via Reference Tree
        public class JackOLantern {
            public enum Gourd { Pumpkin, Squash }

            [PrimaryKey] public Guid ID { get; set; }
            public Gourd MadeFrom { get; set; }
            public double Weight { get; set; }
            public ulong NumSeeds { get; set; }
            public bool Candlelit { get; set; }
            public Farmer.Farm PumpkinPatch { get; set; } = new();
        }
        public class Farmer {
            public class Farm {
                [PrimaryKey] public Guid FarmID { get; set; }
                public float Acreage { get; set; }
                public decimal AnnualGross { get; set; }
                public string? RegisteredName { get; set; }
                public bool FederallyInsured { get; set; }
            }

            [PrimaryKey] public string SSN { get; set; } = "";
            public string Name { get; set; } = "";
            public Farm PrincipalFarm { get; set; } = new();
            public Farm? BackUpFarm { get; set; } = new();
            public decimal NetWorth { get; set; }
            public ushort NumTractorsOwned { get; set; }
        }

        // Test Scenario: Multiple Entities Related via Relation
        public class CashRegister {
            public class Item {
                [PrimaryKey] public Guid ID { get; set; }
                public string Name { get; set; } = "";
                public decimal DollarCost { get; set; }
                public uint Inventory { get; set; }
            }
            public class Currency {
                [PrimaryKey] public string Name { get; set; } = "";
                public char Symbol { get; set; }
                public bool IsVirtual { get; set; }
                public decimal DollarExchangeRate { get; set; }
            }

            [PrimaryKey] public Guid RegisterID { get; set; }
            public string Location { get; set; } = "";
            public RelationMap<Currency, ushort> CashOnHand { get; init; } = [];
            public RelationSet<Item> Sellables { get; init; } = [];
            public double Weight { get; set; }
            public bool IsDigital { get; set; }
            public string AdminCode { get; set; } = "";
        }

        // Test Scenario: Multiple Entities Related via Scalar Localization
        public class Quesadilla {
            [Flags] public enum Topping { None = 0, Salsa = 1, Queso = 2, Guacamole = 4, SourCream = 8, QuesoFresco = 16, PicoDeGallo = 32 }

            [PrimaryKey] public Guid ID { get; set; }
            public ushort Calories { get; set; }
            public decimal Cost { get; set; }
            public LocalizedMeasure AmountOfCheese { get; init; } = new(0);
            public bool IsVegetarian { get; set; }
            public Topping Toppings { get; set; }
        }

        // Test Scenario: Multiple Entities Related via Reference Localization
        public class Honmoon {
            public class DemonHunter {
                [PrimaryKey] public string FirstName { get; set; } = "";
                public string VocalRange { get; set; } = "";
                public ushort Age { get; set; }
            }

            public class LocalizedHunter : Localization<uint, string, DemonHunter> {
                public LocalizedHunter(uint key) : base(key) {}
            }


            [PrimaryKey] public Guid ID { get; set; }
            public bool IsActive { get; set; }
            public LocalizedHunter Creator { get; init; } = new(0);
            public ulong DemonsPrevented { get; set; }
        }

        // Test Scenario: Multiple Entities Related via Relation Localization
        public class CrownJewel {
            [PrimaryKey] public Guid ID { get; set; }
            public string KnownAs { get; set; } = "";
            public RelationList<LocalizedText> Components { get; init; } = [];
            public double WeightPounds { get; set; }
            public string Monarchy { get; set; } = "";
            public ulong? AgeYears { get; set; }
        }

        // Test Scenario: Single Entity with Self-Referential Relation
        public class Matrix {
            [Flags] public enum Traits { None = 0, Identity = 1, Diagonal = 2, Triangular = 4, Square = 8, Definite = 16, Unitary = 32 }

            [PrimaryKey] public Guid MatrixID { get; set; }
            public ushort NumRows { get; set; }
            public ushort NumColumns { get; set; }
            public IReadOnlyRelationList<double> Eigenvalues { get; init; } = new RelationList<double>();
            public RelationSet<Matrix> Inverses { get; init; } = [];
            public Traits MatrixTraits { get; set; }
        }

        // Test Scenario: Single Entity with Self-Referential Localization
        public class ClassActionLawsuit {
            public class LocalizedVerdict : Localization<Guid, ClassActionLawsuit, decimal> {
                public LocalizedVerdict(Guid key) : base(key) {}
            }


            [PrimaryKey] public string CaseID { get; set; } = "";
            public string Plaintiff { get; set; } = "";
            public string Defendant { get; set; } = "";
            public LocalizedDate Certification { get; init; } = new(Guid.NewGuid());
            public LocalizedVerdict Verdict { get; init; } = new(Guid.NewGuid());
            public ulong ClassSize { get; set; }
        }

        // Test Scenario: Pre-Defined Entity
        [PreDefined] public class Dashavatara {
            [PrimaryKey, Column(0)] public int Index { get; private init; }
            [Column(1)] public string Name { get; private init; }
            [Column(2)] public string Form { get; private init; }

            public static Dashavatara Matsaya { get; } = new Dashavatara(1, "Matsaya", "fish");
            public static Dashavatara Kurma { get; } = new Dashavatara(2, "Kurma", "tortoise");
            public static Dashavatara Varaha { get; } = new Dashavatara(3, "Varaha", "boar");
            public static Dashavatara Narasimha { get; } = new Dashavatara(4, "Narasimha", "man-lion");
            public static Dashavatara Vamana { get; } = new Dashavatara(5, "Vamana", "dwarf-god");
            public static Dashavatara Parashurama { get; } = new Dashavatara(6, "Parashurama", "Brahmin warrior");
            public static Dashavatara Rama { get; } = new Dashavatara(7, "Rama", "god");
            public static Dashavatara Krishna { get; } = new Dashavatara(8, "Krishna", "god");
            public static Dashavatara Buddha { get; } = new Dashavatara(9, "Buddha", "enlightened individual");
            public static Dashavatara Kalki { get; } = new Dashavatara(10, "Kalki", "prophesied warrior");

            private Dashavatara(int index, string name, string form) {
                Index = index;
                Name = name;
                Form = form;
            }
        }

        // Test Scenario: Pre-Defined Localization
        [PreDefined] public class CivVITerrain : Localization<string, string, string> {
            public static CivVITerrain Plains { get; } = new CivVITerrain("LOC_PLAINS", "Plains", "PLN");
            public static CivVITerrain Grassland { get; } = new CivVITerrain("LOC_GRASSLAND", "Grassland", "GRS");
            public static CivVITerrain Desert { get; } = new CivVITerrain("LOC_DESERT", "Desert", "DES");
            public static CivVITerrain Tundra { get; } = new CivVITerrain("LOC_TUNDRA", "Tundra", "TND");
            public static CivVITerrain Snow { get; } = new CivVITerrain("LOC_SNOW", "Snow", "SNW");
            public static CivVITerrain Mountain { get; } = new CivVITerrain("LOC_MOUNTAIN", "Mountain", "MNT");
            public static CivVITerrain Coast { get; } = new CivVITerrain("LOC_COAST", "Coast", "CST");
            public static CivVITerrain Ocean { get; } = new CivVITerrain("LOC_OCEAN", "Ocean", "OCN");


            private CivVITerrain(string key, string full, string shortened) : base(key) {
                this["FULL"] = full;
                this["SHORT"] = shortened;
            }
        }

        // Test Scenario: Transaction Rolled Back
        public class Bond {
            public enum Category { Corporate, Municipal, Treasury, Foreign }

            [PrimaryKey] public Guid BondNumber { get; set; }
            public Category Kind { get; set; }
            public string Issuer { get; set; } = "";
            public decimal Par { get; set; }
            public double CouponRate { get; set; }
            public DateTime Maturity { get; set; }
            public decimal? CallValue { get; set; }
        }

        // Test Scenario: Rollback Fails
        public class GrilledCheese {
            [Flags] public enum Cheese { American = 1, Swiss = 2, Gouda = 4, Goat = 8, PepperJack = 16, Gruyere = 32, Muenster = 64, Other = 128 }
            public enum Bread { White, Rye, Pumpernickel, Sourdough, TexasToast, WholeGrain, Other }

            [PrimaryKey] public Cheese Cheeses { get; set; }
            [PrimaryKey] public Bread TopBread { get; set; }
            [PrimaryKey] public Bread BottomBread { get; set; }
            [PrimaryKey] public double CheeseToBreadRatio { get; set; }
            public uint Calories { get; set; }
            public bool WithCrusts { get; set; }
            public byte NumPiecesCut { get; set; }
            public double CheeseFullRating { get; set; }
        }
    }

    internal static class Selection {
        // Test Scenario: Zero Instances of Single Entity
        public class Samurai {
            [PrimaryKey, Column(0)] public Guid SamuraiID { get; set; }
            [Column(1)] public string Era { get; set; } = "";
            [Column(2)] public string Clan { get; set; } = "";
            [Column(3)] public bool IsRonin { get; set; }
            [Column(4)] public double SwordLength { get; set; }
        }

        // Test Scenario: Single Instance of Single Entity with Non-Null Values and No Relations
        public class Diaper {
            [PrimaryKey, Column(0)] public Guid DiaperID { get; set; }
            [Column(1)] public bool IsUsed { get; set; }
            [Column(2)] public float Volume { get; set; }
            [Column(3)] public string Brand { get; set; } = "";
            [Column(4)] public bool ForAdults { get; set; }
        }

        // Test Scenario: Single Instance of Single Entity with Null Values and No Relations
        public class PersonalityTest {
            [PrimaryKey, Column(0)] public string TestName { get; set; } = "";
            [Column(1)] public uint NumQuestions { get; set; }
            [Column(2)] public double? AccuracyPercentage { get; set; }
            [Column(3)] public byte Discriminations { get; set; }
            [Column(4)] public bool IsAMAApproved { get; set; }
            [Column(5)] public ulong? DebutYear { get; set; }
        }

        // Test Scenario: Multiple Instances of Single Entity without Relations
        public class Artery {
            [PrimaryKey, Column(0)] public string Name { get; set; } = "";
            [Column(1)] public byte NumPerPerson { get; set; }
            [Column(2)] public uint BloodFlowPSV { get; set; }
            [Column(3)] public string Vein { get; set; } = "";
        }

        // Test Scenario: Single Instance of Single Entity with Non-Empty Scalar Relations
        public class ActuarialTable {
            [PrimaryKey, Column(0)] public string TableID { get; set; } = "";
            [PrimaryKey, Column(1)] public ushort Year { get; set; }
            [Column(2)] public bool EndorsedBySSA { get; set; }
            public RelationMap<int, double> MaleDeathProbability { get; init; } = [];
            public RelationMap<int, double> FemaleDeathProbability { get; init; } = [];
        }

        // Test Scenario: Single Instance of Single Entity with Empty Scalar Relation
        public class Quasar {
            [PrimaryKey, Column(0)] public string Designation { get; set; } = "";
            public IReadOnlyRelationSet<string> Discoverers { get; init; } = new RelationSet<string>();
            [Column(1)] public string? Constellation { get; set; }
            [Column(2)] public double Redshift { get; set; }
            [Column(3)] public ulong Distance { get; set; }
        }

        // Test Scenario: Multiple Instances of Single Entities with Scalar Relations
        public class Deodorant {
            [PrimaryKey, Column(0)] public Guid ProductID { get; set; }
            public RelationOrderedList<string> Scents { get; init; } = [];
            [Column(1)] public string Brand { get; set; } = "";
            [Column(2)] public decimal Price { get; set; }
            [Column(3)] public bool Antipersperant { get; set; }
        }

        // Test Scenario: Single Instance of Single Localization
        public class VocalRange : Localization<string, Language, double> {
            public VocalRange(string key) : base(key) {}
        }

        // Test Scenario: Multiple Instances of Single Localization
        public class Pleasantry : Localization<string, char, string> {
            public Pleasantry(string key) : base(key) {}
        }

        // Test Scenario: Multiple Unrelated Entities
        public class DrunkHistory {
            [PrimaryKey, Column(0)] public byte Season { get; set; }
            [PrimaryKey, Column(1)] public byte EpisodeNumber { get; set; }
            [PrimaryKey, Column(2)] public string Segment { get; set; } = "";
            [Column(3)] public string Title { get; set; } = "";
            [Column(4)] public string Narrator { get; set; } = "";
            [Column(5)] public DateTime AirDate { get; set; }
        }
        public class Allergen {
            public enum Kind { FoodBorne, AnimalBorne, PlantBorne, MaterialBorne, FungusBorne, Immunodeficiency, Other }

            [PrimaryKey, Column(0)] public Guid AllergenID { get; set; }
            [Column(1)] public Kind Category { get; set; }
            [Column(2)] public string Name { get; set; } = "";
            [Column(3)] public bool FDARecognized { get; set; }
            [Column(4)] public double? Prevalence { get; set; }
        }
        public class Colonscopy {
            [PrimaryKey, Column(0)] public string Patient { get; set; } = "";
            [Column(1)] public DateTime Date { get; set; }
            [Column(2)] public string Doctor { get; set; } = "";
            [Column(3)] public bool IsPreventative { get; set; }
            [Column(4)] public sbyte Discomfort { get; set; }
            [Column(5)] public bool Biopsy { get; set; }
        }

        // Test Scenario: Multiple Unrelated Localizations
        public class WordOfTheDay : Localization<DateOnly, Language, string> {
            public WordOfTheDay(DateOnly key) : base(key) {}
        }
        public class Exclamation : Localization<int, string, ulong> {
            public Exclamation(int key) : base(key) {}
        }
        public class IrrationalNumber : Localization<string, int, double> {
            public IrrationalNumber(string key) : base(key) {}
        }

        // Test Scenario: Multiple Entities Related via Reference Chain
        public class Annuity {
            public enum Stage { Accumulation, Annuitization }

            public class Gender {
                [PrimaryKey, Column(0)] public char Symbol { get; set; }
                [Column(1)] public string Designation { get; set; } = "";
                [Column(2)] public double Prevalence { get; set; }
            }
            public class Person {
                [PrimaryKey, Column(0)] public string SSN { get; set; } = "";
                [Column(1)] public string FirstName { get; set; } = "";
                [Column(2)] public string LastName { get; set; } = "";
                [Column(3)] public Gender Gender { get; set; } = new();
            }
            public class Company {
                [PrimaryKey, Column(0)] public string CompanyName { get; set; } = "";
                [PrimaryKey, Column(1)] public char Classification { get; set; }
                [Column(2)] public Person CEO { get; set; } = new();
                [Column(3)] public decimal Revenue { get; set; }
                [Column(4)] public decimal MarketCap { get; set; }
            }

            [PrimaryKey, Column(0)] public Guid ID { get; set; }
            [Column(1)] public Stage Phase { get; set; }
            [Column(2)] public decimal MarketValue { get; set; }
            [Column(3)] public Person Annuitant { get; set; } = new();
            [Column(4)] public Company Guarantor { get; set; } = new();
            [Column(6)] public bool IsVariable { get; set; }
        }

        // Test Scenario: Multiple Entities Related via Reference Tree
        public class ACapellaGroup {
            public class Songwriter {
                [PrimaryKey, Column(0)] public Guid ID { get; set; }
                [Column(1)] public string FirtName { get; set; } = "";
                [Column(2)] public string LastName { get; set; } = "";
            }
            public class Song {
                [PrimaryKey, Column(0)] public string Title { get; set; } = "";
                [Column(1)] public short SecondsLong { get; set; }
                [Column(2)] public Songwriter Writer { get; set; } = new();
                [Column(3)] public bool ContainsRap { get; set; }
            }
            public class University {
                [PrimaryKey, Column(0)] public Guid InternationalSchoolIdentifier { get; set; }
                [Column(1)] public string Name { get; set; } = "";
                [Column(2)] public ulong Enrollment { get; set; }
                [Column(3)] public decimal Endowment { get; set; }
            }

            [PrimaryKey, Column(0)] public string GroupName { get; set; } = "";
            [Column(1)] public University? College { get; set; }
            [Column(2)] public Song EncoreSong { get; set; } = new();
            [Column(3)] public int NumAltos { get; set; }
            [Column(4)] public int NumSopranos { get; set; }
            [Column(5)] public int NumBaritones { get; set; }
            [Column(6)] public bool IsCoed { get; set; }
        }

        // Test Scenario: Multiple Entities Related via Relation
        public class HailMary {
            public class FootballPlayer {
                [PrimaryKey, Column(0)] public string Team { get; set; } = "";
                [PrimaryKey, Column(1)] public int JerseyNumber { get; set; }
                [Column(2)] public string Name { get; set; } = "";
                [Column(3)] public string Position { get; set; } = "";
            }

            [PrimaryKey, Column(0)] public DateTime Date { get; set; }
            [PrimaryKey, Column(1)] public uint PlayNumber { get; set; }
            [Column(2)] public string Opponent { get; set; } = "";
            public IReadOnlyRelationList<FootballPlayer> PlayersInvolved { get; init; } = new RelationList<FootballPlayer>();
            [Column(3)] public bool ResultedInTouchdown { get; set; }
        }

        // Test Scenario: Multiple Entities Related via Scalar Localization
        public class EventContract {
            [Flags] public enum Category { Politics = 1, Sports = 2, Weather = 4, Entertainment = 8, Media = 16, Economics = 32, WorldEvents = 64, Other = 128 }

            [PrimaryKey, Column(0)] public Guid SecurityID { get; set; }
            [Column(1)] public LocalizedCurrency StrikePrice { get; init; } = new("");
            [Column(2)] public LocalizedDate Expiration { get; init; } = new(Guid.NewGuid());
            [Column(3)] public Category Categorization { get; set; }
            [Column(4)] public LocalizedNullableText ListingExchange { get; init; } = new("");
            [Column(5)] public LocalizedCurrency Fee { get; init; } = new("");
        }

        // Test Scenario: Multiple Entities Related via Reference Localization
        public class DJ {
            public enum Event { Wedding, Graduation, BneiMitzvah, SportingEvent, Concert, Other }

            public class Cost {
                [PrimaryKey, Column(0)] public Guid Entry { get; set; }
                [Column(1)] public int UnitSeconds { get; set; }
                [Column(2)] public decimal Value { get; set; }
            }

            public class LocalizedCost : Localization<string, Event, Cost> {
                public LocalizedCost(string key) : base(key) {}
            }

            [PrimaryKey, Column(0)] public Guid EntertainerID { get; set; }
            [Column(1)] public string Name { get; set; } = "";
            [Column(2)] public DateOnly BirthDate { get; set; }
            [Column(3)] public LocalizedCost Charge { get; set; } = new("");
            [Column(4)] public ulong SongRepertoireSize { get; set; }
            [Column(5)] public double AvgLoudness { get; set; }
        }

        // Test Scenario: Multiple Entities Related via Relation Localization
        public class Whale {
            public enum Dimension { Weight, Height, Length, Lifespan, GestationPeriod }
            public enum IUCN { Extinct, ExtinctInTheWild, CriticallyEndangered, Endangered, Vulnerable, NearThreatened, LeastConcern }

            [PrimaryKey, Column(0)] public string CommonName { get; set; } = "";
            [Column(1)] public string Genus { get; set; } = "";
            [Column(2)] public string Species { get; set; } = "";
            public IReadOnlyRelationMap<Dimension, LocalizedMeasure> Measurements { get; init; } = new RelationMap<Dimension, LocalizedMeasure>();
            [Column(3)] public bool Toothed { get; set; }
            [Column(4)] public IUCN Vulnerability { get; set; }
        }

        // Test Scenario: Single Entity with Self-Referential Relation
        public class IranianShah {
            [PrimaryKey, Column(0)] public string Name { get; set; } = "";
            [Column(1)] public DateTime ReignStart { get; set; }
            [Column(2)] public DateTime ReignEnd { get; set; }
            [Column(3)] public string RoyalHouse { get; set; } = "";
            [Column(4)] public string Capital { get; set; } = "";
            public IReadOnlyRelationSet<IranianShah> Predecessor { get; init; } = new RelationSet<IranianShah>();
        }

        // Test Scenario: Single Entity with Self-Referential Localization
        public class StemCell {
            public class LocalizedCell : Localization<char, bool, StemCell> {
                public LocalizedCell(char key) : base(key) {}
            }


            [PrimaryKey, Column(0)] public Guid CellID { get; set; }
            [Column(1)] public LocalizedCell? ParentCell { get; init; } = new('\0');
            [Column(2)] public float Length { get; set; }
            [Column(3)] public bool IsPluripotent { get; set; }
            [Column(4)] public string Owner { get; set; } = "";
        }
    }

    internal static class Insertion {
        // Test Scenario: Single Instance of Single Entity with Non-Null Values and No Relations
        public class Crossbow {
            [PrimaryKey, Column(0)] public Guid BowID { get; set; }
            [Column(1)] public string Brand { get; set; } = "";
            [Column(2)] public string Model { get; set; } = "";
            [Column(3)] public double Weight { get; set; }
            [Column(4)] public double DrawWeight { get; set; }
            [Column(5)] public double DrawLength { get; set; }
        }

        // Test Scenario: Single Instance of Single Entity with Null Values and No Relations
        public class GoogleDoodle {
            [PrimaryKey, Column(0)] public DateTime Date { get; set; }
            [Column(1)] public string? Artist { get; set; }
            [Column(2)] public bool IsForHoliday { get; set; }
            [Column(3)] public bool IsAnimated { get; set; }
            [Column(4)] public string ArchiveURL { get; set; } = "";
        }

        // Test Scenario: Multiple Instances of Single Entity without Relations
        public class CountOlafDisguise {
            [Flags] public enum Book { BB = 1, RR = 2, WW = 4, MM = 8, AA = 16, EE = 32, VV = 64, HH = 128, CC = 256, SS = 512, GG = 1024, PP = 2048, E = 5096 }

            [PrimaryKey, Column(0)] public string Name { get; set; } = "";
            [Column(1)] public Book Appearances { get; set; }
            [Column(2)] public bool FooledBaudelaires { get; set; }
        }

        // Test Scenario: Single Instance of Single Entity with Non-Empty Scalar Relations
        public class SodaFountain {
            public enum Category { Restaurant, MovieTheater, Stadium, Dwelling, Hotel, Other }

            [PrimaryKey, Column(0)] public Guid ProductID { get; set; }
            [Column(1)] public string Location { get; set; } = "";
            [Column(2)] public Category TypeOfInstitute { get; set; }
            public IReadOnlyRelationMap<string, bool> Sodas { get; init; } = new RelationMap<string, bool>();
            [Column(3)] public bool IsCokeFreestyle { get; set; }
            public RelationOrderedList<DateTime> Inspections { get; init; } = [];
        }

        // Test Scenario: Single Instance of Single Entity with Empty Scalar Relation
        public class LetterOfRecommendation {
            [PrimaryKey, Column(0)] public string Author { get; set; } = "";
            [PrimaryKey, Column(1)] public string Recipient { get; set; } = "";
            [PrimaryKey, Column(2)] public ushort Year { get; set; }
            [Column(3)] public string Purpose { get; set; } = "";
            [Column(4)] public decimal? Compensation { get; set; }
            public IReadOnlyRelationOrderedList<string> Words { get; init; } = new RelationOrderedList<string>();
        }

        // Test Scenario: Multiple Instances of Single Entities with Scalar Relations
        public class MutualFund {
            [PrimaryKey, Column(0)] public Guid FundID { get; set; }
            [PrimaryKey, Column(1)] public Guid CompanyID { get; set; }
            [Column(2)] public decimal NAV { get; set; }
            public RelationMap<string, decimal> Investors { get; init; } = [];
            [Column(3)] public string FundManager { get; set; } = "";
            [Column(4)] public double ManagementFee { get; set; }
        }

        // Test Scenario: Single Instance of Single Localization
        public class Password : Localization<Guid, bool, string> {
            public Password(Guid key) : base(key) {}
            public new string this[bool locale] {
                get { return base[locale]; }
                set { base[locale] = value; }
            }
        }

        // Test Scenario: Multiple Instances of Single Localization
        public enum OrdinalMode { Suffixed, FullySpelled, Symbolized }
        public class Ordinal : Localization<int, OrdinalMode, string> {
            public Ordinal(int key) : base(key) {}
            public new string this[OrdinalMode locale] {
                get { return base[locale]; }
                set { base[locale] = value; }
            }
        }

        // Test Scenario: Multiple Unrelated Entities
        public class Wheelchair {
            [PrimaryKey, Column(0)] public Guid ProductID { get; set; }
            [Column(1)] public string Material { get; set; } = "";
            [Column(2)] public decimal Price { get; set; }
            [Column(3)] public double? MaxWeight { get; set; }
            [Column(4)] public bool CompliesWithADA { get; set; }
        }
        public class Haka {
            [PrimaryKey, Column(0)] public Guid HakaID { get; set; }
            [Column(1)] public string? Leader { get; set; }
            [Column(2)] public bool BeforeSportingEvent { get; set; }
            [Column(3)] public bool PerformedByMaori { get; set; }
            [Column(4)] public double Duration { get; set; }
        }
        public class BetterKnowADistrict {
            public enum Response { GreatPresident, GreatestPresident }

            [PrimaryKey, Column(0)] public byte Season { get; set; }
            [PrimaryKey, Column(1)] public ushort Episode { get; set; }
            [Column(2)] public string State { get; set; } = "";
            [Column(3)] public int DistrictNumber { get; set; }
            [Column(4)] public string Congressperson { get; set; } = "";
            [Column(5)] public double SegmentDuration { get; set; }
            [Column(6)] public Response GeorgeBush { get; set; }
        }
        public class Invoice {
            [PrimaryKey, Column(0)] public Guid InvoiceNumber { get; set; }
            [Column(1)] public string Buyer { get; set; } = "";
            [Column(2)] public string Seller { get; set; } = "";
            [Column(3)] public decimal Amount { get; set; }
            [Column(4)] public DateTime Date { get; set; }
            [Column(5)] public bool IsElectronic { get; set; }
        }

        // Test Scenario: Multiple Unrelated Localizations
        public class Disclaimer : Localization<Guid, Language, string> {
            public Disclaimer(Guid key) : base(key) {}
            public new string this[Language locale] {
                get { return base[locale]; }
                set { base[locale] = value; }
            }
        }
        public class Tagline : Localization<int, int, string> {
            public Tagline(int key) : base(key) {}
            public new string this[int locale] {
                get { return base[locale]; }
                set { base[locale] = value; }
            }
        }
        public class Polynomial : Localization<string, byte, int> {
            public Polynomial(string key) : base(key) {}
            public new int this[byte locale] {
                get { return base[locale]; }
                set { base[locale] = value; }
            }
        }

        // Test Scenario: Multiple Entities Related via Reference Chain
        public class Rhinoceros {
            public class Person {
                [PrimaryKey, Column(0)] public Guid IntelligenceNumber { get; set; }
                [Column(1)] public string FullName { get; set; } = "";
                [Column(2)] public DateTime Birthdate { get; set; }
            }
            public class Zoo {
                [PrimaryKey, Column(0)] public Guid ZooID { get; set; }
                [Column(1)] public string City { get; set; } = "";
                [Column(2)] public Person HeadZookeeper { get; set; } = new();
                [Column(3)] public double Area { get; set; }
                [Column(4)] public ushort DaysOpenPerYear { get; set; }
            }

            [PrimaryKey, Column(0)] public Guid AnimalID { get; set; }
            [Column(1)] public string Genus { get; set; } = "";
            [Column(2)] public string Species { get; set; } = "";
            [Column(3)] public Zoo? Captivity { get; set; }
            [Column(4)] public sbyte NumHorns { get; set; }
        }

        // Test Scenario: Multiple Entities Related via Reference Tree
        public class Sheriff {
            public class Badge {
                [PrimaryKey, Column(0)] public uint Number { get; set; }
                [PrimaryKey, Column(1)] public string Municipality { get; set; } = "";
                [Column(2)] public DateTime DateIssued { get; set; }
            }
            public class Election {
                [PrimaryKey, Column(0)] public Guid ElectionID { get; set; }
                [Column(1)] public DateTime Date { get; set; }
                [Column(2)] public ulong VotesCast { get; set; }
            }

            [PrimaryKey, Column(0)] public Badge SheriffsBadge { get; set; } = new();
            [Column(2)] public string Name { get; set; } = "";
            [Column(3)] public uint Arrests { get; set; }
            [Column(4)] public Election? FirstElected { get; set; }
        }

        // Test Scenario: Multiple Entities Related via Relation
        public class Coven {
            public class Witch {
                [PrimaryKey, Column(0)] public string Name { get; set; } = "";
                [PrimaryKey, Column(1)] public uint SpellcasterNumber { get; set; }
                [Column(2)] public bool Burned { get; set; }
                [Column(3)] public ushort Age { get; set; }
            }

            [PrimaryKey, Column(0)] public Guid CovenID { get; set; }
            public IReadOnlyRelationList<Witch> Witches { get; init; } = new RelationList<Witch>();
            [Column(1)] public bool Wicca { get; set; }
            [Column(2)] public bool OwnsCauldron { get; set; }
        }

        // Test Scenario: Multiple Entities Related via Scalar Localization
        public class MemoryLeak {
            [PrimaryKey, Column(0)] public string Program { get; set; } = "";
            [PrimaryKey, Column(1)] public ulong RunNumber { get; set; }
            [Column(2)] public LocalizedMeasure MemoryLeaked { get; init; } = new(0);
            [Column(3)] public bool DetectedBySanitizer { get; set; }
            [Column(4)] public LocalizedDate IncidentDate { get; init; } = new(Guid.NewGuid());
        }

        // Test Scenario: Multiple Entities Related via Reference Localization
        public class PoliceChase {
            public class Identifier {
                [PrimaryKey, Column(0)] public string Text { get; set; } = "";
                [PrimaryKey, Column(1)] public ulong Numeric { get; set; }
                [Column(2)] public bool IsUniquelyIdentifying { get; set; }
            }

            public class LocalizedID : Localization<Guid, string, Identifier> {
                public LocalizedID(Guid key) : base(key) {}
                public new Identifier this[string locale] {
                    get { return base[locale]; }
                    set { base[locale] = value; }
                }
            }

            [PrimaryKey, Column(0)] public DateTime Timestamp { get; set; }
            [Column(1)] public double DurationMinutes { get; set; }
            [Column(2)] public LocalizedID DatabaseEntry { get; init; } = new LocalizedID(Guid.NewGuid());
            [Column(3)] public ushort NumOfficersInvolved { get; set; }
            [Column(4)] public string Culprit { get; set; } = "";
            [Column(5)] public bool IsVehicular { get; set; }
        }

        // Test Scenario: Multiple Entities Related via Relation Localization
        public class HostileTakeover {
            [PrimaryKey, Column(0)] public string Company { get; set; } = "";
            [PrimaryKey, Column(1)] public DateOnly Date { get; set; }
            [Column(2)] public string Executor { get; set; } = "";
            [Column(3)] public ulong TradedShares { get; set; }
            [Column(4)] public float InitialPercentageControlled { get; set; }
            public RelationSet<LocalizedCurrency> BuyPrices { get; init; } = [];
            [Column(5)] public bool WasSuccessful { get; set; }
            [Column(6)] public bool ProxyFight { get; set; }
        }

        // Test Scenario: Single Entity with Self-Referential Relation
        public class MayanGod {
            [Flags] public enum Source { PopolVuh = 1, ChilamBilam = 2, MadridCodex = 4, Lacandon = 8, DiegoDeLanda = 16 }

            [PrimaryKey, Column(0)] public string Name { get; set; } = "";
            public IReadOnlyRelationSet<MayanGod> Mothers { get; init; } = new RelationSet<MayanGod>();
            public IReadOnlyRelationSet<MayanGod> Fathers { get; init; } = new RelationSet<MayanGod>();
            [Column(1)] public Source Attestations { get; set; }
            [Column(2)] public string Domain { get; set; } = "";
        }

        // Test Scenario: Single Entity with Self-Referential Localization
        public class Pizzeria {
            [Flags] public enum Style { ByTheSlice = 1, ByThePie = 2, Buffet = 4, ThinCrust = 8, Regular = 16, DeepDish = 32, NewYork = 64, Detroit = 128, Bagel = 256 }

            public class LocalizedStore : Localization<Guid, string, Pizzeria> {
                public LocalizedStore(Guid key) : base(key) {}
                public new Pizzeria this[string locale] {
                    get { return base[locale]; }
                    set { base[locale] = value; }
                }
            }

            [PrimaryKey, Column(0)] public string Franchise { get; set; } = "";
            [PrimaryKey, Column(1)] public ulong StoreNumber { get; set; }
            [Column(2)] public decimal AnnualRevenue { get; set; }
            [Column(3)] public string Operator { get; set; } = "";
            [Column(4)] public LocalizedStore? ParentStore { get; init; } = new(Guid.NewGuid());
            [Column(5)] public ushort NumVarieties { get; set; }
            [Column(6)] public Style PizzaStyle { get; set; }
        }

        // Test Scenario: Transaction Rolled Back
        public class Blister {
            public enum Substance { Pus, Blood, Lymph, Serum, Plasma }

            [PrimaryKey, Column(0)] public string Person { get; set; } = "";
            [PrimaryKey, Column(1)] public DateTime DateAcquired { get; set; }
            [Column(2)] public Substance Filling { get; set; }
            [Column(3)] public bool CausedByDermititis { get; set; }
        }

        // Test Scenario: Rollback Fails
        public class Chainsaw {
            [PrimaryKey, Column(0)] public Guid ProductID { get; set; }
            [Column(1)] public string Manufacturer { get; set; } = "";
            [Column(2)] public double Horsepower { get; set; }
            [Column(3)] public float Weight { get; set; }
            [Column(4)] public bool IsBatteryPowered { get; set; }
        }
    }

    internal static class Deletion {
        // Test Scenario: Single Instance of Single Entity with No Relations and Single-Field Primary Key
        public class Pinata {
            public enum Event { Birthday, Quinceaneara, Wedding, Sleepover, Prom, Graduation, Other }

            [PrimaryKey, Column(0)] public Guid PinataID { get; set; }
            [Column(1)] public decimal Price { get; set; }
            [Column(2)] public Event Occasion { get; set; }
            [Column(3)] public string? Breaker { get; set; }
            [Column(4)] public double AmountOfCandy { get; set; }
        }

        // Test Scenario: Single Instance of Single Entity with No Relations and Multi-Field Primary Key
        public class Choir {
            [PrimaryKey, Column(0)] public string GroupName { get; set; } = "";
            [PrimaryKey, Column(1)] public char Level { get; set; }
            [PrimaryKey, Column(2)] public DateTime Established { get; set; }
            [Column(3)] public uint Members { get; set; }
            [Column(4)] public ushort KnownSongs { get; set; }
            [Column(5)] public DateTime? NextConcert { get; set; }
            [Column(6)] public bool IsReligious { get; set; }
        }

        // Test Scenario: Multiple Instances of Single Entity without Relations
        public class EquityOption {
            public enum PutCall { Put, Call };
            public enum Status { InTheMoney, AtTheMoney, OutOfTheMoney }

            [PrimaryKey, Column(0)] public string Underlying { get; set; } = "";
            [PrimaryKey, Column(1)] public DateTime Expiration { get; set; }
            [PrimaryKey, Column(2)] public decimal Strike { get; set; }
            [PrimaryKey, Column(3)] public PutCall Side { get; set; }
            [Column(4)] public Status Moneyness { get; set; }
            [Column(5)] public decimal NBB { get; set; }
            [Column(6)] public decimal NBO { get; set; }
        }

        // Test Scenario: Single Instance of Single Entity with Scalar Relations of Saved Elements
        public class K401 {
            [PrimaryKey, Column(0)] public Guid AccountID { get; set; }
            [Column(1)] public string Provider { get; set; } = "";
            [Column(2)] public string CompanySponsor { get; set; } = "";
            [Column(3)] public double PercentMatch { get; set; }
            [Column(4)] public decimal Balance { get; set; }
            public RelationMap<DateTime, decimal> Deposits { get; init; } = [];
        }

        // Test Scenario: Single Instance of Single Entity with Scalar Relation of Deleted Elements
        public class JumpBall {
            public enum Which { NBA, WNBA, NCAA, JUCO, NAIA, HighSchool, MiddleSchool, PickUp }

            [PrimaryKey, Column(0)] public Guid GameID { get; set; }
            [PrimaryKey, Column(1)] public uint Instance { get; set; }
            [Column(2)] public Which League { get; set; }
            public RelationMap<string, string> Participants { get; init; } = [];
            [Column(3)] public string Referee { get; set; } = "";
        }

        // Test Scenario: Single Instance of Single Entity with Scalar Relation of New Elements
        public class BankVault {
            [PrimaryKey, Column(0)] public Guid BankID { get; set; }
            [PrimaryKey, Column(1)] public string Branch { get; set; } = "";
            [PrimaryKey, Column(2)] public short VaultNumber { get; set; }
            public RelationMap<Guid, decimal> Storage { get; init; } = [];
            public RelationOrderedList<sbyte> Combination { get; init; } = [];
        }

        // Test Scenario: Single Instance of Single Entity with Scalar Relation of Mixed-Status Elements
        public class SushiRoll {
            public enum Color { Brown, White };

            [PrimaryKey, Column(0)] public string RollType { get; set; } = "";
            [PrimaryKey, Column(1)] public string Restaurant { get; set; } = "";
            [Column(2)] public decimal Price { get; set; }
            public RelationSet<string> Ingredients { get; init; } = [];
            [Column(3)] public Color RiceType { get; set; }
        }

        // Test Scenario: Single Instance of Single Entity with Empty Scalar Relation
        public class Woodshop {
            public enum Wood { Birch, Cedar, Ash, Cherry, Cypress, Fir, Pine, Elm, Mahogany, Walnut, Maple, Bamboo }

            [PrimaryKey, Column(0)] public Guid WoodshopID { get; set; }
            public RelationSet<string> Tools { get; init; } = [];
            [Column(1)] public string Owner { get; set; } = "";
            public RelationOrderedList<Wood> TypesOfWood { get; init; } = [];
            [Column(2)] public uint TotalIncidents { get; set; }
            [Column(3)] public decimal InsuranceCoverage { get; set; }
        }

        // Test Scenario: Multiple Instances of Single Entities with Scalar Relations
        public class ConvenienceStore {
            [PrimaryKey, Column(0)] public string StoreBrand { get; set; } = "";
            [PrimaryKey, Column(1)] public ulong StoreNumber { get; set; }
            [Column(2)] public string Address { get; set; } = "";
            [Column(3)] public bool IsBodega { get; set; }
            public IReadOnlyRelationMap<string, decimal> Products { get; init; } = new RelationMap<string, decimal>();
            public RelationOrderedList<string> Employees { get; init; } = [];
        }

        // Test Scenario: Single Instance of Single Localization with Saved Values
        public class Showtime : Localization<string, string, uint> {
            public Showtime(string key) : base(key) {}
            public new uint this[string locale] {
                get { return  base[locale]; }
                set { base[locale] = value; }
            }
        }

        // Test Scenario: Single Instance of Single Localization with Deleted Values
        public class TermOfEndearment : Localization<string, Language, string> {
            public TermOfEndearment(string key) : base(key) {}
            public new string this[Language locale] {
                get { return base[locale]; }
                set { base[locale] = value; }
            }
            public void Delocalize(Language locale) {
                RemoveLocalizationFor(locale);
            }
        }

        // Test Scenario: Single Instance of Single Localization with New Values
        public class Label : Localization<ulong, char, string> {
            public Label(ulong key) : base(key) {}
            public new string this[char locale] {
                get { return base[locale]; }
                set { base[locale] = value; }
            }
        }

        // Test Scenario: Single Instance of Single Localization with Mixed-Status Values
        public class ConjugatedVerb : Localization<int, double, string> {
            public ConjugatedVerb(int key) : base(key) {}
            public new string this[double locale] {
                get { return base[locale]; }
                set { base[locale] = value; }
            }
            public void Delocalize(double locale) {
                RemoveLocalizationFor(locale);
            }
        }

        // Test Scenario: Multiple Instances of Single Localization
        public enum CoordinateForm { Cartesian, Polar, English }
        public class Coordinate : Localization<Guid, CoordinateForm, string> {
            public Coordinate(Guid key) : base(key) {}
            public new string this[CoordinateForm locale] {
                get { return this[locale]; }
                set { base[locale] = value; }
            }
        }

        // Test Scenario: Multiple Unrelated Entities
        public class SeaShanty {
            public enum Type { LongDrag, ShortDrag, SweatingUp, HandOverHand, Bunt, Capstan, Pump, Windlass, Coastwise, Longshore, Misc }

            [PrimaryKey, Column(0)] public string Title { get; set; } = "";
            [Column(1)] public DateTime? EarliestAttestation { get; set; }
            [Column(2)] public Type Kind { get; set; }
        }
        public class ElginMarble {
            public enum Type { Frieze, Metope, Pediment }

            [PrimaryKey, Column(0)] public uint Number { get; set; }
            [Column(1)] public string Description { get; set; } = "";
            [Column(2)] public bool HasBeenRepatriated { get; set; }
            [Column(3)] public Type Source { get; set; }
        }
        public class CepheidVariable {
            public enum Type { Classical, TypeII, Anomalous, DoubleMode }
            
            [PrimaryKey, Column(0)] public string Name { get; set; } = "";
            [Column(1)] public float PeriodDays { get; set; }
            [Column(2)] public double Distance { get; set; }
            [Column(3)] public double Mass { get; set; }
            [Column(4)] public ulong Luminosity { get; set; }
            [Column(5)] public Type Class { get; set; }
        }

        // Test Scenario: Multiple Unrelated Localizations
        public class Sunset : Localization<DateOnly, string, int> {
            public Sunset(DateOnly key) : base(key) {}
            public new int this[string locale] {
                get { return base[locale]; }
                set { base[locale] = value; }
            }
        }
        public class Wingding : Localization<int, byte, string> {
            public Wingding(int key) : base(key) {}
            public new string this[byte locale] {
                get { return base[locale]; }
                set { base[locale] = value; }
            }
        }
        public class Sprite : Localization<string, string, Guid> {
            public Sprite(string key) : base(key) {}
            public new Guid this[string locale] {
                get { return base[locale]; }
                set { base[locale] = value; }
            }
        }

        // Test Scenario: Multiple Entities Related via Reference Chain
        public class CostumeParty {
            public class Costume {
                [PrimaryKey, Column(0)] public Guid CostumeID { get; set; }
                [Column(1)] public string Character { get; set; } = "";
                [Column(2)] public bool IsHomemade { get; set; }
                [Column(3)] public decimal? Cost { get; set; }
            }
            public class Partygoer {
                [PrimaryKey, Column(0)] public string Name { get; set; } = "";
                [Column(1)] public bool WasInvited { get; set; }
                [Column(2)] public Costume Costume { get; set; } = new();
            }

            [PrimaryKey, Column(0)] public Guid PartyID { get; set; }
            [Column(1)] public string Host { get; set; } = "";
            [Column(2)] public DateTime Date { get; set; }
            [Column(3)] public bool ForHalloween { get; set; }
            [Column(4)] public Partygoer Winner { get; set; } = new();
        }

        // Test Scenario: Multiple Entities Related via Reference Tree
        public class WeirdAlParody {
            public class Album {
                [PrimaryKey, Column(0)] public string Title { get; set; } = "";
                [Column(1)] public DateTime Released { get; set; }
            }
            public class Song {
                [PrimaryKey, Column(0)] public string Artist { get; set; } = "";
                [PrimaryKey, Column(1)] public string Title { get; set; } = "";
                [Column(2)] public ulong RecordsSold { get; set; }
                [Column(3)] public bool WonGrammy { get; set; }
            }

            [PrimaryKey, Column(0)] public string Title { get; set; } = "";
            [Column(1)] public DateTime Released { get; set; }
            [Column(2)] public Album? SongAlbum { get; set; }
            [Column(3)] public double Length { get; set; }
            [Column(4)] public Song Basis { get; set; } = new();
            [Column(6)] public string Label { get; set; } = "";
        }

        // Test Scenario: Multiple Entities Related via Relation
        public class Affidavit {
            public class Lawyer {
                public enum Employer { ProBono, LawFirm, DistrictAttorney, Government, DOJ }

                [PrimaryKey, Column(0)] public Guid BarNumber { get; set; }
                [PrimaryKey, Column(1)] public string Name { get; set; } = "";
                [Column(2)] public string LawSchool { get; set; } = "";
                [Column(3)] public Employer Employment { get; set; }
            }

            [PrimaryKey, Column(0)] public Guid ID { get; set; }
            [Column(1)] public string Affiant { get; set; } = "";
            [Column(2)] public DateTime NotarizationDate { get; set; }
            public IReadOnlyRelationOrderedList<Lawyer> LawyersInvolved { get; init; } = new RelationOrderedList<Lawyer>();
            [Column(3)] public bool PartOfPleaDeal { get; set; }
        }

        // Test Scenario: Multiple Entities Related via Scalar Localization
        public class Bandeirante {
            [PrimaryKey, Column(0)] public Guid ID { get; set; }
            [Column(1)] public ushort YearsActive { get; set; }
            [Column(2)] public LocalizedText HomeState { get; init; } = new("");
            [Column(3)] public LocalizedCurrency TotalLooted { get; init; } = new("");
            [Column(4)] public bool IsMameluco { get; set; }
            [Column(5)] public bool SpokePaulistaGeneral { get; set; }
        }

        // Test Scenario: Multiple Entities Related via Reference Localization
        public class BirthdayParty {
            public class YearMonthDay {
                [PrimaryKey, Column(0)] public ushort Year { get; set; }
                [PrimaryKey, Column(1)] public sbyte Month { get; set; }
                [PrimaryKey, Column(2)] public sbyte Day { get; set; }
                [Column(3)] public string? KnownAs { get; set; }
                [PrimaryKey, Column(4)] public string Spelling { get; set; } = "";
            }

            public class LocalizedYearMonthDay : Localization<string, string, YearMonthDay> {
                public LocalizedYearMonthDay(string key) : base(key) {}
                public new YearMonthDay this[string locale] {
                    get { return base[locale]; }
                    set { base[locale] = value; }
                }
            }

            [PrimaryKey, Column(0)] public string Person { get; set; } = "";
            [Column(1)] public LocalizedYearMonthDay Date { get; init; } = new("");
            [Column(2)] public string Location { get; set; } = "";
            [Column(3)] public bool InvitationOnly { get; set; }
            [Column(4)] public ulong Attendees { get; set; }
            [Column(5)] public decimal TotalGiftValue { get; set; }
        }

        // Test Scenario: Multiple Entities Related via Relation Localization
        public class Lawyer {
            public enum Field { Corporate, Copyright, Broadcast, Family, RealEstate, Criminal, Defense, PersonalInjury, Malpractice, Other }

            [PrimaryKey, Column(0)] public Guid BarNumber { get; set; }
            [Column(1)] public string Name { get; set; } = "";
            [Column(2)] public string AlmaMater { get; set; } = "";
            public RelationOrderedList<LocalizedNullableText> Employers { get; init; } = [];
            [Column(3)] public double WinPercentage { get; set; }
            [Column(4)] public decimal AnnualSalary { get; set; }
            [Column(5)] public bool HasBeenDisbarred { get; set; }
            [Column(6)] public bool IsJudge { get; set; }
            [Column(7)] public Field Practice { get; set; }
        }

        // Test Scenario: Single Entity with Self-Referential Relation
        public class Masseuse {
            public enum Kind { Shiatsu, DeepTissue, Acupuncture, Sports, Erotic, Reflexology, Swedish, Other }

            [PrimaryKey, Column(0)] public Guid LicenseNumber { get; set; }
            [PrimaryKey, Column(1)] public Kind Style { get; set; }
            [Column(2)] public string Name { get; set; } = "";
            public IReadOnlyRelationSet<Masseuse> Teachers { get; init; } = new RelationSet<Masseuse>();
            [Column(3)] public bool IsFreelance { get; set; }
            [Column(4)] public sbyte NumTables { get; set; }
        }

        // Test Scenario: Single Entity with Self-Referential Localization
        public class Radiologist {
            public class OnCallEscalation : Localization<DateOnly, byte, Radiologist> {
                public OnCallEscalation(DateOnly key) : base(key) {}
                public new Radiologist this[byte locale] {
                    get { return base[locale]; }
                    set { base[locale] = value; }
                }
            }

            [PrimaryKey, Column(0)] public Guid MedicalID { get; set; }
            [Column(1)] public string Name { get; set; } = "";
            [Column(2)] public bool CanUseMRI { get; set; }
            [Column(3)] public double BecquerelsExposure { get; set; }
            [Column(4)] public bool DoesCancerTreatment { get; set; }
            [Column(5)] public string? Hospital { get; set; }
            [Column(6)] public OnCallEscalation Superior { get; init; } = new(DateOnly.FromDateTime(DateTime.Now));
        }

        // Test Scenario: Transaction Rolled Back
        public class Gazebo {
            [PrimaryKey, Column(0)] public Guid GazeboID { get; set; }
            [Column(1)] public string GeneralShape { get; set; } = "";
            [Column(2)] public uint MaxCapacity { get; set; }
            [Column(3)] public bool IsTented { get; set; }
        }

        // Test Scenario: Rollback Fails
        public class Moai {
            [PrimaryKey, Column(0)] public string Site { get; set; } = "";
            [PrimaryKey, Column(1)] public uint Number { get; set; }
            [Column(2)] public double Height { get; set; }
            [Column(3)] public string EyesMaterial { get; set; } = "";
            [Column(4)] public bool HasPukao { get; set; }
        }
    }

    internal static class Update {
        // Test Scenario: Single Instance of Single Entity with No Relations
        public class DifferentialEquation {
            [PrimaryKey, Column(0)] public Guid EquationId { get; set; }
            [Column(1)] public string Equation { get; set; } = "";
            [Column(2)] public bool IsPartial { get; set; }
            [Column(3)] public float? NumSolutions { get; set; }
        }

        // Test Scenario: Multiple Instances of Single Entity without Relations
        public class Conquistador {
            public enum Civilization { Aztec, Inca, Olmec, Zapotec, Other }

            [PrimaryKey, Column(0)] public string Name { get; set; } = "";
            [Column(1)] public Civilization Conquest { get; set; }
            [Column(2)] public DateTime DateOfBirth { get; set; }
            [Column(3)] public DateTime DateOfDeath { get; set; }
        }

        // Test Scenario: Single Instance of Single Entity with Scalar Relations of Saved Elements
        public class BigFatQuiz {
            public enum Kind { OfTheYear, OfTheDecade, OfEverything, OfSports, OfTelly, Anniversary }

            public struct Team {
                [Column(0)] public string Player1 { get; set; }
                [Column(1)] public string Player2 { get; set; }
            }

            [PrimaryKey, Column(0)] public DateTime Airdate { get; set; }
            [Column(1)] public Kind Variety { get; set; }
            public RelationMap<string, Team> Teams { get; init; } = [];
            [Column(2)] public ulong YouTubeViews { get; set; }
            [Column(3)] public double TelevisionRating { get; set; }
        }

        // Test Scenario: Single Instance of Single Entity with Scalar Non-Associative Relation of Deleted Elements
        public class Guacamole {
            [PrimaryKey, Column(0)] public Guid GuacamoleID { get; set; }
            [Column(1)] public string Preparer { get; set; } = "";
            [Column(2)] public ushort NumServings { get; set; }
            public RelationSet<string> Ingredients { get; init; } = [];
            [Column(3)] public bool MadeInMolcajete { get; set; }
        }

        // Test Scenario: Single Instance of Single Entity with Scalar Associative Relation of Deleted Elements
        public class EyeDrops {
            public enum Condition { Astigmatism, NearSightedness, FarSightedness, Glaucoma, Cataracts, Conjunctivitis }

            [PrimaryKey, Column(0)] public Guid DropsID { get; set; }
            [Column(1)] public string BrandName { get; set; } = "";
            [Column(2)] public ulong Prescriptions { get; set; }
            public RelationMap<Condition, bool> TreatmentPlan { get; init; } = [];
            [Column(3)] public bool SafeForChildren { get; set; }
            [Column(4)] public bool Dilatory { get; set; }
            [Column(5)] public sbyte MaxDropsPerDay { get; set; }
        }

        // Test Scenario: Single Instance of Single Entity with Scalar Relation of New Elements
        public class EquestrianStatue {
            public enum Unit { Inch, Centimeter, Pound, Kilogram }
            public enum Dimension { Height, Weight, Width, Depth,  }

            public struct Measurement {
                [Column(0)] public double Value { get; set; }
                [Column(1)] public Unit Unit { get; set; }
            }

            [PrimaryKey, Column(0)] public Guid ArtworkID { get; set; }
            [Column(1)] public string Title { get; set; } = "";
            [Column(2)] public string? Artist { get; set; }
            [Column(3)] public string HorsebackFigure { get; set; } = "";
            [Column(4)] public sbyte NumHorses { get; set; }
            public RelationMap<Dimension, Measurement> Dimensions { get; init; } = [];
        }

        // Test Scenario: Single Instance of Single Entity with Scalar Relation of Modified Elements
        public class Bibliography {
            public enum Standard { APA, Chicago, MLA, Harvard, Other }
            public enum Literature { Book, Article, Newscast, Multimedia, Newspaper, Interview, Magazine, Other }

            public record struct Reference {
                [Column(0)] public string Title { get; set; }
                [Column(1)] public string Author { get; set; }
                [Column(2)] public Literature TypeOfWork { get; set; }
            }

            [PrimaryKey, Column(0)] public Guid ID { get; set; }
            [Column(1)] public string For { get; set; } = "";
            [Column(2)] public Standard Style { get; set; }
            public RelationOrderedList<Reference> References { get; init; } = [];
            [Column(3)] public byte NumPages { get; set; }
        }

        // Test Scenario: Single Instance of Single Entity with Scalar Relation of Mixed-Status Elements
        public class MarketMaker {
            [PrimaryKey, Column(0)] public string PrimaryMPID { get; set; } = "";
            [Column(1)] public string FirmName { get; set; } = "";
            [Column(2)] public decimal NetCapital { get; set; }
            public RelationOrderedList<string> Symbols { get; init; } = [];
            [Column(3)] public bool IsDesignated { get; set; }
        }

        // Test Scenario: Single Instance of Single Entity with Empty Scalar Relation
        public class DeadSeaScroll {
            public enum Language { Hebrew, Aramaic, Greek, Latin, English, Demotic }

            [PrimaryKey, Column(0)] public string Cave { get; set; } = "";
            [PrimaryKey, Column(1)] public string Identifier { get; set; } = "";
            [Column(2)] public Language ScrollLanguage { get; set; }
            [Column(3)] public string BiblicalSource { get; set; } = "";
            [Column(4)] public ushort DiscoveryYear { get; set; }
            public RelationSet<string> VerifiedAuthors { get; init; } = [];
        }

        // Test Scenario: Multiple Instances of Single Entities with Scalar Relations
        public class Diocese {
            public struct DateRange {
                [Column(0)] public DateTime Start { get; set; }
                [Column(1)] public DateTime? End { get; set; }
            }

            [PrimaryKey, Column(0)] public string Name { get; set; } = "";
            [PrimaryKey, Column(1)] public string Church { get; set; } = "";
            [Column(2)] public bool IsArchdiocese { get; set; }
            [Column(3)] public ushort Parishes { get; set; }
            public RelationMap<string, DateRange> Bishops { get; init; } = [];
        }

        // Test Scenario: Single Instance of Single Localization with Saved Values
        public class Placement : Localization<string, int, string> {
            public Placement(string key) : base(key) {}
            public new string this[int locale] {
                get { return base[locale]; }
                set { base[locale] = value; }
            }
        }

        // Test Scenario: Single Instance of Single Localization with Deleted Values
        [Flags] public enum Operation { None = 0, CanRead = 1, CanWrite = 2, CanModify = 4, CanDelete = 8, CanCreate = 16, CanAdmin = 32 }
        public class Permissions : Localization<string, string, Operation> {
            public Permissions(string key) : base(key) {}
            public new Operation this[string locale] {
                get { return base[locale]; }
                set { base[locale] = value; }
            }
            public void Delocalize(string locale) {
                RemoveLocalizationFor(locale);
            }
        }

        // Test Scenario: Single Instance of Single Localization with New Values
        public class BinaryValue : Localization<string, bool, string> {
            public BinaryValue(string key) : base(key) {}
            public new string this[bool locale] {
                get { return base[locale]; }
                set { base[locale] = value; }
            }
        }

        // Test Scenario: Single Instance of Single Localization with Mixed-Status Values
        public class BigBad : Localization<string, sbyte, string> {
            public BigBad(string key) : base(key) {}
            public new string this[sbyte locale] {
                get { return base[locale]; }
                set { base[locale] = value; }
            }
            public void Delocalize(sbyte locale) {
                RemoveLocalizationFor(locale);
            }
        }

        // Test Scenario: Multiple Instances of Single Localization
        public class MinimumWage : Localization<string, short, decimal> {
            public MinimumWage(string key) : base(key) {}
            public new decimal this[short locale] {
                get { return base[locale]; }
                set { base[locale] = value; }
            }
        }

        // Test Scenario: Multiple Unrelated Entities
        public class Waltz {
            [PrimaryKey, Column(0)] public string PieceTitle { get; set; } = "";
            [PrimaryKey, Column(1)] public string Composer { get; set; } = "";
            [PrimaryKey, Column(2)] public uint Number { get; set; }
            [Column(3)] public double Length { get; set; }
            [Column(4)] public DateTime Premiere { get; set; }
            [Column(5)] public bool DancedToOnDWTS { get; set; }
        }
        public class Pacemaker {
            public enum Method { Percussive, Subcutaneous, Epicardial, TemporaryTransvenous, PermanentTransvenous, Leadless }

            [PrimaryKey, Column(0)] public Guid ProductID { get; set; }
            [Column(1)] public string Recipient { get; set; } = "";
            [Column(2)] public DateTime InstalledOn { get; set; }
            [Column(3)] public string InstalledBy { get; set; } = "";
            [Column(4)] public Method Pacing { get; set; }
        }
        public class DemigodCamp {
            public enum Pantheon { Greek, Roman, Norse, Egyptian, Hindu, Celtic, NativeAmerican, Slavic, Aztec, Other }

            [PrimaryKey, Column(0)] public string Name { get; set; } = "";
            [Column(1)] public string Location { get; set; } = "";
            [Column(2)] public Pantheon Mythology { get; set; }
            [Column(3)] public uint Campers { get; set; }
            [Column(4)] public sbyte NumCabins { get; set; }
            [Column(5)] public string FirstAppearance { get; set; } = "";
        }

        // Test Scenario: Multiple Unrelated Localizations
        public class Philosophy : Localization<string, Language, string> {
            public Philosophy(string key) : base(key) {}
            public new string this[Language locale] {
                get { return base[locale]; }
                set { base[locale] = value; }
            }
        }
        public class AlterEgo : Localization<string, string, string> {
            public AlterEgo(string key) : base(key) {}
            public new string this[string locale] {
                get { return base[locale]; }
                set { base[locale] = value; }
            }
        }
        public class Pain : Localization<int, string, double> {
            public Pain(int key) : base(key) {}
            public new double this[string locale] {
                get { return base[locale]; }
                set { base[locale] = value; }
            }
        }

        // Test Scenario: Multiple Entities Related via Reference Chain
        public class PromoCode {
            public class Company {
                [PrimaryKey, Column(0)] public string Name { get; set; } = "";
                [PrimaryKey, Column(1)] public string Discriminator { get; set; } = "";
                [Column(2)] public bool IsPubliclyTraded { get; set; }
                [Column(3)] public decimal AnnualRevenue { get; set; }
            }
            public class Discout {
                [PrimaryKey, Column(0)] public Guid ID { get; set; }
                [Column(1)] public float Percentage { get; set; }
                [Column(2)] public bool IsBOGO { get; set; }
                [Column(3)] public Company ValidAt { get; set; } = new();
                [Column(5)] public DateTime? Expiration { get; set; }
            }

            [Column(0)] public string Code { get; set; } = "";
            [PrimaryKey, Column(1)] public Discout Discount { get; set; } = new();
            [Column(2)] public bool IsActive { get; set; }
            [Column(3)] public bool OnlineOnly { get; set; }
        }

        // Test Scenario: Multiple Entities Related via Reference Tree
        public class WritOfCertiorari {
            public class President {
                public enum Party { Democratic, Republican, DemocraticRepublican, Whig, Green, Populist, Libertarian, Communist }

                [PrimaryKey, Column(0)] public sbyte Number { get; set; }
                [Column(1)] public string Name { get; set; } = "";
                [Column(2)] public Party PoliticalParty { get; set; }
            }
            public class Case {
                public enum Type { Criminal, Civil }

                [PrimaryKey, Column(0)] public ulong DocketNumber { get; set; }
                [Column(1)] public Type Variety { get; set; }
                [Column(2)] public string Plaintiff { get; set; } = "";
                [Column(3)] public string Defendant { get; set; } = "";
            }
            public class Justice {
                [PrimaryKey, Column(0)] public string Name { get; set; } = "";
                [Column(1)] public ushort YearAssumedBench { get; set; }
                [Column(2)] public President AppointedBy { get; set; } = new();
                [Column(3)] public bool IsChiefJustice { get; set; }
            }

            [PrimaryKey, Column(0)] public Guid WritID { get; set; }
            [Column(1)] public Case CourtCase { get; set; } = new();
            [Column(2)] public Justice IssuingJudge { get; set; } = new();
            [Column(3)] public sbyte JusticesInFavor { get; set; }
            [Column(4)] public sbyte? OriginatingCircuit { get; set; }
        }

        // Test Scenario: Multiple Entities Related via Relation
        public class HungerGames {
            public class PanemCitizen {
                [PrimaryKey, Column(0)] public string Name { get; set; } = "";
                [Column(1)] public sbyte District { get; set; }
                [Column(2)] public bool IsFemale { get; set; }
            }

            [PrimaryKey, Column(0)] public int Incarnation { get; set; }
            [Column(1)] public string Gamemaker { get; set; } = "";
            public RelationMap<PanemCitizen, PanemCitizen?> Killers { get; init; } = [];
            [Column(2)] public bool IsQuarterQuell { get; set; }
            [Column(3)] public string President { get; set; } = "";
        }

        // Test Scenario: Multiple Entities Related via Scalar Localization
        public class Daemon {
            [PrimaryKey, Column(0)] public string Human { get; set; } = "";
            [Column(1)] public string Name { get; set; } = "";
            [Column(2)] public LocalizedText Animal { get; init; } = new("");
            [Column(3)] public bool IsZombi { get; set; }
            [Column(4)] public bool CompletedAkterrakeh { get; set; }
        }

        // Test Scenario: Multiple Entities Related via Reference Localization
        public class Vasectomy {
            public enum Stage { Anesthetic, Surgery, Recovery, Monitoring }

            public class Doctor {
                [PrimaryKey, Column(0)] public Guid MedicalID { get; set; }
                [Column(1)] public string Name { get; set; } = "";
                [Column(2)] public string AlmaMater { get; set; } = "";
                [Column(3)] public string Specialty { get; set; } = "";
            }

            public class LocalizedStage : Localization<Guid, Stage, Doctor> {
                public LocalizedStage(Guid key) : base(key) {}
                public new Doctor this[Stage locale] {
                    get { return base[locale]; }
                    set { base[locale] = value; }
                }
            }

            [PrimaryKey, Column(0)] public Guid SurgeryID { get; set; }
            [Column(1)] public string Patient { get; set; } = "";
            [Column(2)] public DateOnly Date { get; set; }
            [Column(3)] public LocalizedStage Doctors { get; init; } = new(Guid.NewGuid());
            [Column(4)] public bool Reversible { get; set; }
        }

        // Test Scenario: Multiple Entities Related via Relation Localization
        public class Harbor {
            public enum Flow { InFlow, OutFlow }

            [PrimaryKey, Column(0)] public string Name { get; set; } = "";
            [Column(1)] public ulong ShippingTons { get; set; }
            [Column(2)] public double DraftDepth { get; set; }
            [Column(3)] public double AirDraft { get; set; }
            public RelationMap<LocalizedText, Flow> Rivers { get; init; } = [];
            [Column(4)] public string OperatedBy { get; set; } = "";
        }

        // Test Scenario: Single Entity with Self-Referential Relation
        public class Pandava {
            [PrimaryKey, Column(0)] public string Name { get; set; } = "";
            [Column(1)] public string Father { get; set; } = "";
            [Column(2)] public ulong MahabharataMentions { get; set; }
            public RelationList<Pandava> Brothers { get; init; } = [];
            [Column(3)] public string PrimaryWeapon { get; set; } = "";
        }

        // Test Scenario: Single Entity with Self-Referential Localization
        public class DisneyPrincess {
            [Flags] public enum Parent { None = 0, Mother = 1, Father = 2 };

            public class Opinion : Localization<string, DisneyPrincess, double> {
                public Opinion(string key) : base(key) {}
                public new double this[DisneyPrincess locale] {
                    get { return base[locale]; }
                    set { base[locale] = value; }
                }
            }

            [PrimaryKey, Column(0)] public string Name { get; set; } = "";
            [Column(1)] public string FilmSeries { get; set; } = "";
            [Column(2)] public ushort TotalAppearances { get; set; }
            [Column(3)] public double Height { get; set; }
            [Column(4)] public bool HasAnimalSidekick { get; set; }
            [Column(5)] public Parent LivingParents { get; set; }
            [Column(6)] public Opinion ThoughtsOnOthers { get; init; } = new("");
        }

        // Test Scenario: Transaction Rolled Back
        public class ChatBot {
            [PrimaryKey, Column(0)] public Guid BotID { get; set; }
            [Column(1)] public string? Name { get; set; }
            [Column(2)] public bool CanPassTuringTest { get; set; }
            [Column(3)] public string? URL { get; set; }
            [Column(4)] public bool UsesPrompts { get; set; }
            [Column(5)] public bool IsGenerativeAI { get; set; }
        }

        // Test Scenario: Rollback Fails
        public class SurgeonGeneral {
            [PrimaryKey, Column(0)] public string Name { get; set; } = "";
            [PrimaryKey, Column(1)] public int Iteration { get; set; }
            [Column(2)] public string AppointedBy { get; set; } = "";
            [Column(3)] public DateTime TermBegin { get; set; }
            [Column(4)] public DateTime? TermEnd { get; set; }
            [Column(5)] public string? MedicalSchool { get; set; }
        }
    }

    internal static class Administration {
        // Scenario: Schema Management Table Does Not Yet Exist
        public class DogWalker {
            [PrimaryKey] public Guid ID { get; set; }
            public string Name { get; set; } = "";
            public decimal HourlyRate { get; set; }
            public byte MaxDogsAtOnce { get; set; }
            public ulong NumPoopBagsCarrying { get; set; }
            public bool WillWalkPitbulls { get; set; }
            public ushort NumCurrentEngagements { get; set; }
        }

        // Scenario: Schema Management Table is Unpopulated
        public class MapProjection {
            [PrimaryKey] public string Name { get; set; } = "";
            public ushort IntroducedIn { get; set; }
            public double TissotsEccentricity { get; set; }
            public bool NorthernHemisphereAtTop { get; set; }
            public float LongitudeCenter { get; set; }
            public float LatitudeCenter { get; set; }
            public IReadOnlyRelationSet<string> Designers { get; init; } = new RelationSet<string>();
        }
        public class Hymn {
            [PrimaryKey] public string Title { get; set; } = "";
            public bool IsChristian { get; set; }
            public string Incipit { get; set; } = "";
            public bool IsChoral { get; set; }
        }

        // Scenario: Schema Management Table is Fully Populated
        public class Amulet {
            [PrimaryKey] public Guid JewelryID { get; set; }
            public string PrimaryColor { get; set; } = "";
            public decimal MarketValue { get; set; }
            public string? Gemstone { get; set; }
            public float ChainLength { get; set; }
            public bool IsCursed { get; set; }
        }
        public class Outlier {
            [PrimaryKey] public Guid DataSetID { get; set; }
            [PrimaryKey] public ulong DataPoint { get; set; }
            public double Value { get; set; }
            public double ZScore { get; set; }
        }
        public class Shaman {
            public enum Variety { MedicineMan, Spiritualist, Soulcatcher, Other }

            [PrimaryKey] public string Name { get; set; } = "";
            public Variety Kind { get; set; }
            public ushort YearsPracticing { get; set; }
            public bool UsesDrugs { get; set; }
        }

        // Scenario: Schema Management Table is Missing at Least One Principal Table
        public class IndenturedServant {
            [PrimaryKey] public string Name { get; set; } = "";
            public DateOnly DateOfIndeturitude { get; set; }
            public ushort DaysServitude { get; set; }
            public bool IsManumitted { get; set; }
            public string Overseer { get; set; } = "";
        }
        public class Oatmeal {
            [PrimaryKey] public Guid ID { get; set; }
            public string Brand { get; set; } = "";
            public string Flavor { get; set; } = "";
            public bool Instant { get; set; }
            public bool WholeGrains { get; set; }
        }

        // Scenario: Schema Management Table is Missing at Least One Relation Table
        public class LorcanaCharacter {
            public enum Color { Amber, Amethyst, Emerald, Ruby, Sapphire, Steel }

            [PrimaryKey] public string Name { get; set; } = "";
            public sbyte Strength { get; set; }
            public sbyte Willpower { get; set; }
            public sbyte Lore { get; set; }
            public Color InkColor { get; set; }
            public RelationOrderedList<string> Effects { get; init; } = [];
        }

        // Scenario: Schema Management Table Contains Mismatched Hash for at Least One Principal Table (✗error✗)
        public class GrandRelic {
            public enum School { Abjuration, Conjuration, Divination, Enchantment, Evocation, Illusion, Necromancy, Transmutation }

            [PrimaryKey] public string Name { get; set; } = "";
            public sbyte EpisodeIntroduced { get; set; }
            public string? DiscoveringSeeker { get; set; }
            public School SchoolOfMagic { get; set; }
            public bool IsDestroyed { get; set; }
        }

        // Scenario: Schema Management Table Contains Mismatched Hash for at Least One Relation Table (✗error✗)
        public class Metazooa {
            [PrimaryKey] public uint GameID { get; set; }
            public DateTime GameRelease { get; set; }
            public string SolutionCommon { get; set; } = "";
            public RelationMap<string, string> SolutionScientific { get; init; } = [];
            public double AverageGuesses { get; set; }
        }

        // Scenario: Schema Management Table Contains a Hash for a Non-Existent Table (✗error✗)
        public class PillPocket {
            [Flags] public enum Pet { Dog = 1, Cat = 2, Ferret = 4, Chinchilla = 8, Rabbit = 16, Pig = 32, Horse = 64, Iguana = 128 }

            [PrimaryKey] public Guid BatchID { get; set; }
            [PrimaryKey] public uint ItemNumber { get; set; }
            public float Weight { get; set; }
            public string Flavor { get; set; } = "";
            public Pet PetsSafeFor { get; set; }
            public double PillVolume { get; set; }
        }
    }
}
