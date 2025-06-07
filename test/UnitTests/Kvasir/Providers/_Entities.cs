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

        // Scenario: INSERT INTO – Text and Enumerations
        public class ChipotleOrder {
            public enum MealType { Burrito, Bowl, Tacos, Salad }
            public enum BeansOption { None, Pinto, Black }
            public enum ProteinOption { None, Chicken, Steak, Carnitas, Barbacoa, Tofu, Speciality }
            [Flags] public enum SalsaOption { None = 0, Mild = 1, Medium = 2, Hot = 4, Questo = 8 }
            public enum CheeseOption { None, Single, Extra }
            public enum LettuceOption { None, Salad, AddOn }
            public enum GuacOption { None, Extra, Perk, OnTheSide }

            [PrimaryKey, Column(0)] public string OrderId { get; set; } = "";
            [Column(1)] public MealType Kind { get; set; }
            [Column(2)] public BeansOption Beans { get; set; }
            [Column(3)] public ProteinOption Protein { get; set; }
            [Column(4)] public SalsaOption Salsa { get; set; }
            [Column(5)] public CheeseOption Cheese { get; set; }
            [Column(6)] public LettuceOption Lettuce { get; set; }
            [Column(7)] public GuacOption Guacamole { get; set; }
            [Column(8)] public string? NameForOrder { get; set; }
            [Column(9)] public char CRC { get; set; }
        }

        // Scenario: INSERT INTO – Integers and Booleans
        public class RegularExpression {
            [Column(0)] public bool IsPearlCompatible { get; set; }
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
        }

        // Scenario: INSERT INTO – DateTime and GUID
        public class ExecutiveOrder {
            [PrimaryKey, Column(0)] public string President { get; set; } = "";
            [PrimaryKey, Column(1)] public ushort OrderNumber { get; set; }
            [Column(2)] public DateTime Issued { get; set; }
            [Column(3)] public Guid DocumentID { get; set; }
            [Column(4)] public bool EnshrinedInLegislation { get; set; }
        }

        // Scenario: INSERT INTO – Null Value
        // [TODO] - Manicure

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
        // [TODO] - Codex

        // Scenario: UPDATE – 1 Row of Principal Table with Single-Field Primary Key
        // [TODO] - Sign Language

        // Scenario: UPDATE – 1 Row of Principal Table with Multi-Field Primary Key
        // [TODO] - Diaspora

        // Scenario: UPDATE – 1 Row of Principal Table with All-Fields Primary Key
        // [TODO] - Pidgin

        // Scenario: UPDATE – 2 Rows
        public class WelshGod {
            [PrimaryKey, Column(0)] public Guid DeityID { get; set; }
            [Column(1)] public string Name { get; set; } = "";
            [Column(2)] public string Domain { get; set; } = "";
            [Column(3)] public ulong MabinogionMentions { get; set; }
        }

        // Scenario: UPDATE – 3+ Rows
        public class CutthroatKitchenSabotage {
            public enum Round { Round1, Round2, Round3 };

            [PrimaryKey, Column(0)] public sbyte Season { get; set; }
            [PrimaryKey, Column(1)] public sbyte Episode { get; set; }
            [PrimaryKey, Column(2)] public Round WhichRound { get; set; }
            [PrimaryKey, Column(3)] public sbyte SabotageNumber { get; set; }
            [Column(4)] public string Sabotage { get; set; } = "";
            [Column(5)] public decimal WinningBid { get; set; }
            [Column(6)] public string HighestBidder { get; set; } = "";
        }

        // Scenario: UPDATE – Null Value
        // [TODO] - ?????

        // Scenario: UPDATE – Non-Associative Relation Table
        // [TODO] - Overture

        // Scenario: UPDATE – Associative Relation Table
        // [TODO] - Supermodel

        // Scenario: DELETE – 1 Row of Principal Table with Single-Field Primary Key
        public class Mansa {
            [Column(0)] public string Name { get; set; } = "";
            [PrimaryKey, Column(1)] public uint Index { get; set; }
            [Column(2)] public DateTime Birth { get; set; }
            [Column(3)] public DateTime Coronation { get; set; }
            [Column(4)] public DateTime Death { get; set; }
            [Column(5)] public string? RoyalHouse { get; set; }
        }

        // Scenario: DELETE – 1 Row of Principal Table with Multi-Field Primary Key
        public class Hamentaschen {
            [Flags] public enum Ingredient { Chocolate = 1, GrapeJelly = 2, StrawberryJelly = 4, Nutella = 8, ApricotJam = 16, Marshmallow = 32, RaspberryJam = 64 }

            [PrimaryKey, Column(0)] public ushort Year { get; set; }
            [Column(1)] public Ingredient Filling { get; set; }
            [PrimaryKey, Column(2)] public uint BatchNumber { get; set; }
            [Column(3)] public bool IsGlutenFree { get; set; }
            [PrimaryKey, Column(4)] public ushort CookieNumber { get; set; }
            [PrimaryKey, Column(5)] public Guid CookieMakerID { get; set; }
        }

        // Scenario: DELETE – 2 Rows
        public class Iyalet {
            [Column(0)] public string Domain { get; set; } = "";
            [Column(1)] public uint Members { get; set; }
            [PrimaryKey, Column(2)] public string Name { get; set; } = "";
            [Column(3)] public string? Cozulate { get; set; }
        }

        // Scenario: DELETE – 3+ Rows
        public class FoodTruck {
            public enum Style { American, Chinese, Mexican, Seafood, Indian, Thai, Korean, Japanese, German, Italian, Fusion, Other }

            [PrimaryKey, Column(0)] public string TruckName { get; set; } = "";
            [PrimaryKey, Column(1)] public string Proprietor { get; set; } = "";
            [Column(2)] public Style Cuisine { get; set; }
            [Column(3)] public Guid VehicleID { get; set; }
            [Column(4)] public decimal AnnualRevenue { get; set; }
            [Column(5)] public ushort MenuSize { get; set; }
        }

        // Scenario: DELETE – Non-Associative Row from Relation Table
        // [TODO] - Line Dance

        // Scenario: DELETE – Associative Row from Relation Table
        // [TODO] - Buzzer System

        // Scenario: DELETE – All Rows of an Owning Entity from Relation Table
        // [TODO] - Fugue

        // Scenario: DELETE – Mixed Style from Relation Table
        // [TODO] - Abortion Clinic
    }
}
