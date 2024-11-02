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
}
