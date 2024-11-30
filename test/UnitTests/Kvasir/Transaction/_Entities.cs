﻿using Kvasir.Annotations;
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
            public RelationMap<string, double> NutritionInfo { get; } = new();
            public RelationSet<Trait> Traits { get; } = new();
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
            public RelationMap<Currency, ushort> CashOnHand { get; set; } = new RelationMap<Currency, ushort>();
            public RelationSet<Item> Sellables { get; set; } = new RelationSet<Item>();
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
            public IReadOnlyRelationList<double> Eigenvalues { get; set; } = new RelationList<double>();
            public RelationSet<Matrix> Inverses { get; set; } = new RelationSet<Matrix>();
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

        // Test Scenario: Single Entity with Non-Empty Scalar Relations
        public class ActuarialTable {
            [PrimaryKey, Column(0)] public string TableID { get; set; } = "";
            [PrimaryKey, Column(1)] public ushort Year { get; set; }
            [Column(2)] public bool EndorsedBySSA { get; set; }
            public RelationMap<int, double> MaleDeathProbability { get; set; } = new RelationMap<int, double>();
            public RelationMap<int, double> FemaleDeathProbability { get; set; } = new RelationMap<int, double>();
        }

        // Test Scenario: Single Entity with Empty Scalar Relation
        public class Quasar {
            [PrimaryKey, Column(0)] public string Designation { get; set; } = "";
            public IReadOnlyRelationSet<string> Discoverers { get; set; } = new RelationSet<string>();
            [Column(1)] public string? Constellation { get; set; }
            [Column(2)] public double Redshift { get; set; }
            [Column(3)] public ulong Distance { get; set; }
        }

        // Test Scenario: Multiple Entities with Scalar Relations
        public class Deodorant {
            [PrimaryKey, Column(0)] public Guid ProductID { get; set; }
            public RelationOrderedList<string> Scents { get; set; } = new RelationOrderedList<string>();
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
            public IReadOnlyRelationList<FootballPlayer> PlayersInvolved { get; set; } = new RelationList<FootballPlayer>();
            [Column(3)] public bool ResultedInTouchdown { get; set; }
        }

        // Test Scenario: Single Entity with Self-Referential Relation
        public class IranianShah {
            [PrimaryKey, Column(0)] public string Name { get; set; } = "";
            [Column(1)] public DateTime ReignStart { get; set; }
            [Column(2)] public DateTime ReignEnd { get; set; }
            [Column(3)] public string RoyalHouse { get; set; } = "";
            [Column(4)] public string Capital { get; set; } = "";
            public IReadOnlyRelationSet<IranianShah> Predecessor { get; set; } = new RelationSet<IranianShah>();
        }
    }
}
