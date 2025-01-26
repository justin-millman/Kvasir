using Kvasir.Annotations;
using Kvasir.Relations;
using System;

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
            public RelationMap<string, double> NutritionInfo { get; init; } = new();
            public RelationSet<Trait> Traits { get; init; } = new();
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
            public RelationMap<Currency, ushort> CashOnHand { get; init; } = new RelationMap<Currency, ushort>();
            public RelationSet<Item> Sellables { get; init; } = new RelationSet<Item>();
            public double Weight { get; set; }
            public bool IsDigital { get; set; }
            public string AdminCode { get; set; } = "";
        }

        // Test Scenario: Single Entity with Self-Referential Relation
        public class Matrix {
            [Flags] public enum Traits { None = 0, Identity = 1, Diagonal = 2, Triangular = 4, Square = 8, Definite = 16, Unitary = 32 }

            [PrimaryKey] public Guid MatrixID { get; set; }
            public ushort NumRows { get; set; }
            public ushort NumColumns { get; set; }
            public IReadOnlyRelationList<double> Eigenvalues { get; init; } = new RelationList<double>();
            public RelationSet<Matrix> Inverses { get; init; } = new RelationSet<Matrix>();
            public Traits MatrixTraits { get; set; }
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
            public RelationMap<int, double> MaleDeathProbability { get; init; } = new RelationMap<int, double>();
            public RelationMap<int, double> FemaleDeathProbability { get; init; } = new RelationMap<int, double>();
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
            public RelationOrderedList<string> Scents { get; init; } = new RelationOrderedList<string>();
            [Column(1)] public string Brand { get; set; } = "";
            [Column(2)] public decimal Price { get; set; }
            [Column(3)] public bool Antipersperant { get; set; }
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

        // Test Scenario: Single Entity with Self-Referential Relation
        public class IranianShah {
            [PrimaryKey, Column(0)] public string Name { get; set; } = "";
            [Column(1)] public DateTime ReignStart { get; set; }
            [Column(2)] public DateTime ReignEnd { get; set; }
            [Column(3)] public string RoyalHouse { get; set; } = "";
            [Column(4)] public string Capital { get; set; } = "";
            public IReadOnlyRelationSet<IranianShah> Predecessor { get; init; } = new RelationSet<IranianShah>();
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
            public RelationOrderedList<DateTime> Inspections { get; init; } = new();
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
            public RelationMap<string, decimal> Investors { get; init; } = new();
            [Column(3)] public string FundManager { get; set; } = "";
            [Column(4)] public double ManagementFee { get; set; }
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

        // Test Scenario: Single Entity with Self-Referential Relation
        public class MayanGod {
            [Flags] public enum Source { PopolVuh = 1, ChilamBilam = 2, MadridCodex = 4, Lacandon = 8, DiegoDeLanda = 16 }

            [PrimaryKey, Column(0)] public string Name { get; set; } = "";
            public IReadOnlyRelationSet<MayanGod> Mothers { get; init; } = new RelationSet<MayanGod>();
            public IReadOnlyRelationSet<MayanGod> Fathers { get; init; } = new RelationSet<MayanGod>();
            [Column(1)] public Source Attestations { get; set; }
            [Column(2)] public string Domain { get; set; } = "";
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
            public RelationMap<DateTime, decimal> Deposits { get; init; } = new RelationMap<DateTime, decimal>();
        }

        // Test Scenario: Single Instance of Single Entity with Scalar Relation of Deleted Elements
        public class JumpBall {
            public enum Which { NBA, WNBA, NCAA, JUCO, NAIA, HighSchool, MiddleSchool, PickUp }

            [PrimaryKey, Column(0)] public Guid GameID { get; set; }
            [PrimaryKey, Column(1)] public uint Instance { get; set; }
            [Column(2)] public Which League { get; set; }
            public RelationMap<string, string> Participants { get; init; } = new RelationMap<string, string>();
            [Column(3)] public string Referee { get; set; } = "";
        }

        // Test Scenario: Single Instance of Single Entity with Scalar Relation of New Elements
        public class BankVault {
            [PrimaryKey, Column(0)] public Guid BankID { get; set; }
            [PrimaryKey, Column(1)] public string Branch { get; set; } = "";
            [PrimaryKey, Column(2)] public short VaultNumber { get; set; }
            public RelationMap<Guid, decimal> Storage { get; init; } = new();
            public RelationOrderedList<sbyte> Combination { get; init; } = new();
        }

        // Test Scenario: Single Instance of Single Entity with Scalar Relation of Mixed-Status Elements
        public class SushiRoll {
            public enum Color { Brown, White };

            [PrimaryKey, Column(0)] public string RollType { get; set; } = "";
            [PrimaryKey, Column(1)] public string Restaurant { get; set; } = "";
            [Column(2)] public decimal Price { get; set; }
            public RelationSet<string> Ingredients { get; init; } = new();
            [Column(3)] public Color RiceType { get; set; }
        }

        // Test Scenario: Single Instance of Single Entity with Empty Scalar Relation
        public class Woodshop {
            public enum Wood { Birch, Cedar, Ash, Cherry, Cypress, Fir, Pine, Elm, Mahogany, Walnut, Maple, Bamboo }

            [PrimaryKey, Column(0)] public Guid WoodshopID { get; set; }
            public RelationSet<string> Tools { get; init; } = new();
            [Column(1)] public string Owner { get; set; } = "";
            public RelationOrderedList<Wood> TypesOfWood { get; init; } = new();
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
            public RelationOrderedList<string> Employees { get; init; } = new();
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
            public RelationMap<string, Team> Teams { get; init; } = new();
            [Column(2)] public ulong YouTubeViews { get; set; }
            [Column(3)] public double TelevisionRating { get; set; }
        }

        // Test Scenario: Single Instance of Single Entity with Scalar Relation of Deleted Elements
        public class Guacamole {
            [PrimaryKey, Column(0)] public Guid GuacamoleID { get; set; }
            [Column(1)] public string Preparer { get; set; } = "";
            [Column(2)] public ushort NumServings { get; set; }
            public RelationSet<string> Ingredients { get; init; } = new();
            [Column(3)] public bool MadeInMolcajete { get; set; }
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
            public RelationMap<Dimension, Measurement> Dimensions { get; init; } = new();
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
            public RelationOrderedList<Reference> References { get; init; } = new();
            [Column(3)] public byte NumPages { get; set; }
        }

        // Test Scenario: Single Instance of Single Entity with Scalar Relation of Mixed-Status Elements
        public class MarketMaker {
            [PrimaryKey, Column(0)] public string PrimaryMPID { get; set; } = "";
            [Column(1)] public string FirmName { get; set; } = "";
            [Column(2)] public decimal NetCapital { get; set; }
            public RelationOrderedList<string> Symbols { get; init; } = new();
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
            public RelationSet<string> VerifiedAuthors { get; init; } = new();
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
            public RelationMap<string, DateRange> Bishops { get; init; } = new();
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
            public RelationMap<PanemCitizen, PanemCitizen?> Killers { get; init; } = new();
            [Column(2)] public bool IsQuarterQuell { get; set; }
            [Column(3)] public string President { get; set; } = "";
        }

        // Test Scenario: Single Entity with Self-Referential Relation
        public class Pandava {
            [PrimaryKey, Column(0)] public string Name { get; set; } = "";
            [Column(1)] public string Father { get; set; } = "";
            [Column(2)] public ulong MahabharataMentions { get; set; }
            public RelationList<Pandava> Brothers { get; init; } = new();
            [Column(3)] public string PrimaryWeapon { get; set; } = "";
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
}
