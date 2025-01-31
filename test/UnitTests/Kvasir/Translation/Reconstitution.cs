using FluentAssertions;
using Kvasir.Core;
using Kvasir.Schema;
using Kvasir.Translation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using static UT.Kvasir.Translation.Globals;
using static UT.Kvasir.Translation.Reconstitution;
using static UT.Kvasir.Translation.TestConverters;

namespace UT.Kvasir.Translation {
    [TestClass, TestCategory("Reconstitution")]
    public class ReconstitutionTests {
        [TestMethod] public void WriteableProperties_NonNullPublicInstanceScalars() {
            // Arrange
            var row = new List<DBValue>() {
                DBValue.Create(Guid.NewGuid()),
                DBValue.Create("Butchart Gardens"),
                DBValue.Create(ConversionOf(Garden.Type.Flower)),
                DBValue.Create(54.36318),
                DBValue.Create(7561920),
                DBValue.Create(true)
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(Garden)];
            var garden = (Garden)translation.Principal.Reconstitutor.ReconstituteFrom(row);

            // Assert
            row[0].Datum.Should().Be(garden.GardenID);
            row[1].Datum.Should().Be(garden.Name);
            row[2].Datum.Should().Be(ConversionOf(garden.Kind));
            row[3].Datum.Should().Be(garden.Acreage);
            row[4].Datum.Should().Be(garden.NumFlowers);
            row[5].Datum.Should().Be(garden.OpenToThePublic);
        }

        [TestMethod] public void WriteableProperties_NonNullPublicStaticScalars() {
            // Arrange
            var row = new List<DBValue>() {
                DBValue.Create("University of Michigan"),
                DBValue.Create("South Quad"),
                DBValue.Create(ConversionOf(Dormitory.Feature.Coed | Dormitory.Feature.Honors | Dormitory.Feature.DrugFree | Dormitory.Feature.Dining)),
                DBValue.Create(true),
                DBValue.Create((ushort)1170),
                DBValue.Create((sbyte)0b11111)
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(Dormitory)];
            var dormitory = (Dormitory)translation.Principal.Reconstitutor.ReconstituteFrom(row);

            // Assert
            row[0].Datum.Should().Be(dormitory.School);
            row[1].Datum.Should().Be(dormitory.Name);
            row[2].Datum.Should().Be(ConversionOf(dormitory.Features));
            row[3].Datum.Should().Be(Dormitory.IsBuilding);
            row[4].Datum.Should().Be(dormitory.Capacity);
            row[5].Datum.Should().Be(Dormitory.GradeBits);
        }

        [TestMethod] public void WriteableProperties_NonNullNonPublicInstanceScalars() {
            // Arrange
            var row = new List<DBValue>() {
                DBValue.Create("Louis"),
                DBValue.Create(14U),
                DBValue.Create("Catholicism"),
                DBValue.Create(new DateTime(1654, 6, 7)),
                DBValue.Create("Bourbon"),
                DBValue.Create(true)
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(KingOfFrance)];
            var king = (KingOfFrance)translation.Principal.Reconstitutor.ReconstituteFrom(row);

            // Assert
            row[0].Datum.Should().Be(king.RegnalName);
            row[1].Datum.Should().Be(king.RegnalNumber);
            row[2].Datum.Should().Be(king.GetReligion());
            row[3].Datum.Should().Be(king.Coronation);
            row[4].Datum.Should().Be(king.RoyalHouse);
            row[5].Datum.Should().Be(king.PreRevolution);
        }

        [TestMethod] public void WriteableProperties_NonNullNonPublicStaticScalars() {
            // Arrange
            var row = new List<DBValue>() {
                DBValue.Create(Guid.NewGuid()),
                DBValue.Create("Pierre de Fermat"),
                DBValue.Create((ushort)31894),
                DBValue.Create(ConversionOf(MathematicalProof.Method.Induction)),
                DBValue.Create(true),
                DBValue.Create(17561829581.58M)
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(MathematicalProof)];
            var proof = (MathematicalProof)translation.Principal.Reconstitutor.ReconstituteFrom(row);

            // Assert
            row[0].Datum.Should().Be(proof.ProofID);
            row[1].Datum.Should().Be(proof.MathematicianName);
            row[2].Datum.Should().Be(MathematicalProof.GetMaxAllowedSteps());
            row[3].Datum.Should().Be(ConversionOf(MathematicalProof.MostPopularMethod));
            row[4].Datum.Should().Be(MathematicalProof.ComputersAllowed);
            row[5].Datum.Should().Be(MathematicalProof.GetPrizeMoneyEarned());
        }

        [TestMethod] public void WriteableProperties_NullScalars() {
            // Arrange
            var row = new List<DBValue>() {
                DBValue.Create(Guid.NewGuid()),
                DBValue.NULL,
                DBValue.Create(185.3146),
                DBValue.NULL,
                DBValue.Create(true),
                DBValue.NULL
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(Banshee)];
            var banshee = (Banshee)translation.Principal.Reconstitutor.ReconstituteFrom(row);

            // Assert
            row[0].Datum.Should().Be(banshee.MonsterID);
            banshee.Name.Should().BeNull();
            row[2].Datum.Should().Be(banshee.WailDecibels);
            banshee.Victims.Should().BeNull();
            row[4].Datum.Should().Be(banshee.Female);
            banshee.Origin.Should().BeNull();
        }

        [TestMethod] public void WriteableProperties_InitOnly() {
            // Arrange
            var row = new List<DBValue>() {
                DBValue.Create(Guid.NewGuid()),
                DBValue.Create("Gold Coast"),
                DBValue.Create("Chicago, IL"),
                DBValue.Create((ushort)881),
                DBValue.Create(13639UL),
                DBValue.Create(2718M),
                DBValue.Create(799.0)
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(Neighborhood)];
            var neighborhood = (Neighborhood)translation.Principal.Reconstitutor.ReconstituteFrom(row);

            // Assert
            row[0].Datum.Should().Be(neighborhood.ID);
            row[1].Datum.Should().Be(neighborhood.Name);
            row[2].Datum.Should().Be(neighborhood.City);
            row[3].Datum.Should().Be(neighborhood.NumHouses);
            row[4].Datum.Should().Be(neighborhood.Population);
            row[5].Datum.Should().Be(neighborhood.AverageRent);
            row[6].Datum.Should().Be(neighborhood.AverageSqFt);
        }

        [TestMethod] public void CalculatedScalarProperties() {
            // Arrange
            var row = new List<DBValue>() {
                DBValue.Create("https://www.patreon.com/ExtraCredits/"),
                DBValue.Create("The Extra Credits Channel"),
                DBValue.Create(10.0M),
                DBValue.Create(20.0M),
                DBValue.Create(50.0M),
                DBValue.Create((10.0M + 20.0M + 50.0M) / 3)
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(Patreon)];
            var patreon = (Patreon)translation.Principal.Reconstitutor.ReconstituteFrom(row);

            // Assert
            row[0].Datum.Should().Be(patreon.URL);
            row[1].Datum.Should().Be(patreon.Creator);
            row[2].Datum.Should().Be(patreon.Tier0);
            row[3].Datum.Should().Be(patreon.Tier1);
            row[4].Datum.Should().Be(patreon.Tier2);
            row[5].Datum.Should().Be(patreon.AverageTier);
        }

        [TestMethod] public void CalculatedAggregateProperties() {
            // Arrange
            var row = new List<DBValue>() {
                DBValue.Create(Guid.NewGuid()),
                DBValue.Create((ushort)173),
                DBValue.Create(173 / 15),
                DBValue.Create(173 / 9),
                DBValue.Create(173 / 23),
                DBValue.Create((sbyte)2),
                DBValue.Create(false)
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(EmergencyRoom)];
            var room = (EmergencyRoom)translation.Principal.Reconstitutor.ReconstituteFrom(row);

            // Assert
            row[0].Datum.Should().Be(room.BuildingID);
            row[1].Datum.Should().Be(room.Capacity);
            row[2].Datum.Should().Be(room.Staff.NumDoctors);
            row[3].Datum.Should().Be(room.Staff.NumNurses);
            row[4].Datum.Should().Be(room.Staff.NumOther);
            row[5].Datum.Should().Be(room.TraumaLevel);
            row[6].Datum.Should().Be(room.IsUrgentCare);
        }

        [TestMethod] public void CalculatedReferenceProperties() {
            // Arrange
            var row = new List<DBValue>() {
                DBValue.Create("87-330-1966"),
                DBValue.Create("Edward G. Norelga"),
                DBValue.Create("Klippity Accounting Co."),
                DBValue.Create("Montana"),
                DBValue.Create(112)
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(Accountant)];
            var accountant = (Accountant)translation.Principal.Reconstitutor.ReconstituteFrom(row);

            // Assert
            row[0].Datum.Should().Be(accountant.SSN);
            row[1].Datum.Should().Be(accountant.Name);
            row[2].Datum.Should().Be(accountant.CurrentFirm.Name);
            row[3].Datum.Should().Be(accountant.CurrentFirm.State);
            row[4].Datum.Should().Be(accountant.NumAccounts);
        }

        [TestMethod] public void CalculatedRelationProperties() {
            // Arrange
            var row = new List<DBValue>() {
                DBValue.Create(Guid.NewGuid()),
                DBValue.Create("Susanna O'Suibhenthaoire"),
                DBValue.Create("Ebhan MacGallamhuille"),
                DBValue.Create(3427891M),
                DBValue.Create("Eamon Nhaing"),
                DBValue.Create("Delaware")
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(Prenup)];
            var prenup = (Prenup)translation.Principal.Reconstitutor.ReconstituteFrom(row);

            // Assert
            translation.Relations[0].Repopulator.Should().BeNull();
            row[0].Datum.Should().Be(prenup.ContractID);
            row[1].Datum.Should().Be(prenup.Spouse1);
            row[2].Datum.Should().Be(prenup.Spouse2);
            row[3].Datum.Should().Be(prenup.TotalNetWorth);
            row[4].Datum.Should().Be(prenup.Notary);
            row[5].Datum.Should().Be(prenup.StateEnforced);
            prenup.VestingSchedule.Count.Should().Be(5);
        }

        [TestMethod] public void WriteableProperties_ExplicitInterfaceImplementations() {
            // Arrange
            var row = new List<DBValue>() {
                DBValue.Create("Pygoscelis"),
                DBValue.Create("adeliae"),
                DBValue.Create("Adélie Penguin"),
                DBValue.Create(false),
                DBValue.Create(6.0),
                DBValue.Create(52.5),
                DBValue.Create(0.0)
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(Penguin)];
            var penguin = (Penguin)translation.Principal.Reconstitutor.ReconstituteFrom(row);

            // Assert
            row[0].Datum.Should().Be(penguin.Genus);
            row[1].Datum.Should().Be(penguin.Species);
            row[2].Datum.Should().Be(penguin.CommonName);
            row[3].Datum.Should().Be((penguin as IBird).CanFly);
            row[4].Datum.Should().Be(penguin.AverageWeightKg);
            row[5].Datum.Should().Be((penguin as IBird).WingspanCm);
            row[6].Datum.Should().Be((penguin as IBird).TopSpeedKph);
        }

        [TestMethod] public void WriteableProperties_VirtualOverride() {
            // Arrange
            var row = new List<DBValue>() {
                DBValue.Create(Guid.NewGuid()),
                DBValue.Create(43.159),
                DBValue.Create(32),
                DBValue.Create(487.6235f)
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(Trampoline)];
            var trampoline = (Trampoline)translation.Principal.Reconstitutor.ReconstituteFrom(row);

            // Assert
            row[0].Datum.Should().Be(trampoline.ID);
            row[1].Datum.Should().Be(trampoline.MaxHeight);
            row[2].Datum.Should().Be(trampoline.NumScrews);
            row[3].Datum.Should().Be(trampoline.WeightLimit);
        }

        [TestMethod] public void WriteableProperties_Hiding() {
            // Arrange
            var row = new List<DBValue>() {
                DBValue.Create(Guid.NewGuid()),
                DBValue.Create(84U),
                DBValue.Create(new DateTime(2017, 4, 12)),
                DBValue.Create(1751.5),
                DBValue.Create(58.125)
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(Quilt)];
            var quilt = (Quilt)translation.Principal.Reconstitutor.ReconstituteFrom(row);

            // Assert
            row[0].Datum.Should().Be(quilt.ID);
            row[1].Datum.Should().Be(quilt.NumSquares);
            row[2].Datum.Should().Be(quilt.CreationDate);
            row[3].Datum.Should().Be(quilt.Length);
            row[4].Datum.Should().Be(quilt.Width);
        }

        [TestMethod] public void ScalarDataConversions() {
            // Arrange
            var row = new List<DBValue>() {
                DBValue.Create("NORTH CENTRAL SERVICE"),
                DBValue.Create("Chicago Union Station"),
                DBValue.Create("Antioch"),
                DBValue.Create(18),
                DBValue.Create(524945U),
                DBValue.Create(52.9)
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(MetraRoute)];
            var route = (MetraRoute)translation.Principal.Reconstitutor.ReconstituteFrom(row);

            // Assert
            new AllCaps().Revert((string)row[0].Datum).Should().Be(route.Line);
            row[1].Datum.Should().Be(route.CityEndpoint);
            row[2].Datum.Should().Be(route.SuburbEndpoint);
            new ToInt<ushort>().Revert((int)row[3].Datum).Should().Be(route.NumStations);
            row[4].Datum.Should().Be(route.Ridership);
            row[5].Datum.Should().Be(route.TrackLength);
        }

        [TestMethod] public void EnumerationNumericConversion() {
            // Arrange
            var row = new List<DBValue>() {
                DBValue.Create("Baron Samedi"),
                DBValue.Create((byte)Loa.Month.NOV),
                DBValue.Create((int)(Loa.Tradition.Haitian | Loa.Tradition.Louisiana | Loa.Tradition.FolkCatholicism)),
                DBValue.Create("Death"),
                DBValue.Create(true)
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(Loa)];
            var loa = (Loa)translation.Principal.Reconstitutor.ReconstituteFrom(row);

            // Assert
            row[0].Datum.Should().Be(loa.Name);
            row[1].Datum.Should().Be((byte)loa.FeastMonth);
            row[2].Datum.Should().Be((int)loa.Traditions);
            row[3].Datum.Should().Be(loa.Domain);
            row[4].Datum.Should().Be(loa.InvolvedWithZombies);
        }

        [TestMethod] public void EnumerationToStringConversion() {
            // Arrange
            var row = new List<DBValue>() {
                DBValue.Create(Guid.NewGuid()),
                DBValue.Create(ConversionOf(LED.Color.White)),
                DBValue.Create(13.944)
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(LED)];
            var led = (LED)translation.Principal.Reconstitutor.ReconstituteFrom(row);

            // Assert
            row[0].Datum.Should().Be(led.ProductID);
            row[1].Datum.Should().Be(ConversionOf(led.EmittedColor));
            row[2].Datum.Should().Be(led.PowerConsumption);
        }

        [TestMethod] public void WriteableProperties_NonNullSingleFieldAggregate() {
            // Arrange
            var row = new List<DBValue>() {
                DBValue.Create(Guid.NewGuid()),
                DBValue.Create("Edward Q. Havenrao"),
                DBValue.Create(ConversionOf(ConspiracyTheory.Industry.Government)),
                DBValue.Create(756182U),
                DBValue.Create(false),
                DBValue.NULL
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(ConspiracyTheory)];
            var theory = (ConspiracyTheory)translation.Principal.Reconstitutor.ReconstituteFrom(row);

            // Assert
            row[0].Datum.Should().Be(theory.ID);
            row[1].Datum.Should().Be(theory.Theorist.Name);
            row[2].Datum.Should().Be(ConversionOf(theory.About));
            row[3].Datum.Should().Be(theory.Believers);
            row[4].Datum.Should().Be(theory.FormallyDebunked);
            theory.WikipediaURL.Should().BeNull();
        }

        [TestMethod] public void WriteableProperties_NonNullMultiFieldAggregate() {
            // Arrange
            var row = new List<DBValue>() {
                DBValue.Create(Guid.NewGuid()),
                DBValue.Create((ushort)173),
                DBValue.Create(false),
                DBValue.Create((byte)12),
                DBValue.Create((byte)185),
                DBValue.Create((byte)66),
                DBValue.Create((byte)71),
                DBValue.Create((byte)2),
                DBValue.Create((byte)244),
                DBValue.Create((byte)160),
                DBValue.Create((byte)251),
                DBValue.Create((byte)29)
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(Mermaid)];
            var mermaid = (Mermaid)translation.Principal.Reconstitutor.ReconstituteFrom(row);

            // Assert
            row[0].Datum.Should().Be(mermaid.MermaidID);
            row[1].Datum.Should().Be(mermaid.HeightCm);
            row[2].Datum.Should().Be(mermaid.IsSiren);
            row[3].Datum.Should().Be(mermaid.HairColor.R);
            row[4].Datum.Should().Be(mermaid.HairColor.G);
            row[5].Datum.Should().Be(mermaid.HairColor.B);
            row[6].Datum.Should().Be(mermaid.BraColor.R);
            row[7].Datum.Should().Be(mermaid.BraColor.G);
            row[8].Datum.Should().Be(mermaid.BraColor.B);
            row[9].Datum.Should().Be(mermaid.TailColor.R);
            row[10].Datum.Should().Be(mermaid.TailColor.G);
            row[11].Datum.Should().Be(mermaid.TailColor.B);
        }

        [TestMethod] public void WriteableProperties_AggregateWithAllNullNestedFields() {
            // Arrange
            var row = new List<DBValue>() {
                DBValue.Create("Gouda"),
                DBValue.Create("Netherlands"),
                DBValue.NULL,
                DBValue.NULL,
                DBValue.NULL,
                DBValue.NULL,
                DBValue.NULL,
                DBValue.Create(ConversionOf(Cheese.Style.Wedges))
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(Cheese)];
            var cheese = (Cheese)translation.Principal.Reconstitutor.ReconstituteFrom(row);

            // Assert
            row[0].Datum.Should().Be(cheese.Name);
            row[1].Datum.Should().Be(cheese.CountryOfOrigin);
            cheese.NutritionalValue.Calories.Should().BeNull();
            cheese.NutritionalValue.GramsFat.Should().BeNull();
            cheese.NutritionalValue.MgSodium.Should().BeNull();
            cheese.NutritionalValue.MgCholesterol.Should().BeNull();
            cheese.NutritionalValue.GramsCarbs.Should().BeNull();
            row[7].Datum.Should().Be(ConversionOf(cheese.BestServedAs!.Value));
        }

        [TestMethod] public void WriteableProperties_NullSingleFieldAggregate() {
            // Arrange
            var row = new List<DBValue>() {
                DBValue.Create(Guid.NewGuid()),
                DBValue.Create("Dr. Alfonse Callu"),
                DBValue.NULL,
                DBValue.Create("Auvi-Q")
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(EpiPen)];
            var epipen = (EpiPen)translation.Principal.Reconstitutor.ReconstituteFrom(row);

            // Assert
            row[0].Datum.Should().Be(epipen.MedicalID);
            row[1].Datum.Should().Be(epipen.PrescribingDoctor);
            epipen.Dose.Should().BeNull();
            row[3].Datum.Should().Be(epipen.Manufacturer);
        }

        [TestMethod] public void WriteableProperties_NullMultiFieldAggregate() {
            // Arrange
            var row = new List<DBValue>() {
                DBValue.Create("Wan Haoxi"),
                DBValue.Create(ConversionOf(Hacker.Hat.White)),
                DBValue.NULL,
                DBValue.NULL,
                DBValue.NULL,
                DBValue.NULL,
                DBValue.Create(0M),
                DBValue.Create(false),
                DBValue.Create(false),
                DBValue.Create(17633L)
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(Hacker)];
            var hacker = (Hacker)translation.Principal.Reconstitutor.ReconstituteFrom(row);

            // Assert
            row[0].Datum.Should().Be(hacker.Name);
            row[1].Datum.Should().Be(ConversionOf(hacker.Role));
            hacker.PreferredTerminal.Should().BeNull();
            row[6].Datum.Should().Be(hacker.RansomExtorted);
            row[7].Datum.Should().Be(hacker.InAnonymous);
            row[8].Datum.Should().Be(hacker.StateSponsored);
            row[9].Datum.Should().Be(hacker.DevicesCompromised);
        }

        [TestMethod] public void WriteableProperties_NestedAggregate() {
            // Arrange
            var row = new List<DBValue>() {
                DBValue.Create(Guid.NewGuid()),
                DBValue.Create(true),
                DBValue.Create(new DateTime(726, 4, 17)),
                DBValue.Create(new DateTime(741, 6, 18)),
                DBValue.Create(109641U),
                DBValue.Create(6623),
                DBValue.Create(571628591028.55M),
                DBValue.Create("Emperor Leo III the Isaurian")
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(Iconoclast)];
            var iconoclast = (Iconoclast)translation.Principal.Reconstitutor.ReconstituteFrom(row);

            // Assert
            row[0].Datum.Should().Be(iconoclast.IconoclastID);
            row[1].Datum.Should().Be(iconoclast.IsByzantine);
            row[2].Datum.Should().Be(iconoclast.History.Activity.Start);
            row[3].Datum.Should().Be(iconoclast.History.Activity.End);
            row[4].Datum.Should().Be(iconoclast.History.Stats.IconsDestroyed);
            row[5].Datum.Should().Be(iconoclast.History.Stats.PaintingsBurned);
            row[6].Datum.Should().Be(iconoclast.History.Stats.PreciousMetalsCollected);
            row[7].Datum.Should().Be(iconoclast.Name);
        }

        [TestMethod] public void AggregateNestedDataConversion() {
            // Arrange
            var row = new List<DBValue>() {
                DBValue.Create("Vampire"),
                DBValue.Create("Rick Worthy"),
                DBValue.Create(ConversionOf(AlphaMonster.Hunter.Sam)),
                DBValue.Create(6),
                DBValue.Create(5),
                DBValue.Create(4U)
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(AlphaMonster)];
            var alpha = (AlphaMonster)translation.Principal.Reconstitutor.ReconstituteFrom(row);

            // Assert
            row[0].Datum.Should().Be(alpha.Monster);
            row[1].Datum.Should().Be(alpha.Actor);
            row[2].Datum.Should().Be(ConversionOf(alpha.Killer!.Value));
            new ToInt<short>().Revert((int)row[3].Datum).Should().Be(alpha.FirstAppearance.SeasonNumber);
            new ToInt<float>().Revert((int)row[4].Datum).Should().Be(alpha.FirstAppearance.EpisodeNumber);
            row[5].Datum.Should().Be(alpha.NumAppearances);
        }

        [TestMethod] public void CalculatedAndNonCalculatedAggregateInSameEntity() {
            // Arrange
            var row = new List<DBValue>() {
                DBValue.Create(Guid.NewGuid()),
                DBValue.Create(2.25M),
                DBValue.Create((ushort)190),
                DBValue.Create("chorizo"),
                DBValue.Create(100.0),
                DBValue.NULL,
                DBValue.NULL,
                DBValue.Create(false)
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(Empanada)];
            var empanada = (Empanada)translation.Principal.Reconstitutor.ReconstituteFrom(row);

            // Assert
            row[0].Datum.Should().Be(empanada.EmpanadaID);
            row[1].Datum.Should().Be(empanada.Price);
            row[2].Datum.Should().Be(empanada.Calories);
            row[3].Datum.Should().Be(empanada.MainFilling.Contents);
            row[4].Datum.Should().Be(empanada.MainFilling.Percentage);
            empanada.SecondaryFilling.Should().BeNull();
            row[7].Datum.Should().Be(empanada.DeepFried);
        }

        [TestMethod] public void WriteableProperties_NonNullReferenceSingleFieldPrimaryKey() {
            // Arrange
            var roosterRow1 = new List<DBValue>() {
                DBValue.Create(Guid.NewGuid()),
                DBValue.Create("Foghorn Leghorn"),
                DBValue.Create(6.25),
                DBValue.Create(214.8)
            };
            var roosterRow2 = new List<DBValue>() {
                DBValue.Create(Guid.NewGuid()),
                DBValue.Create("Panchito Pistoles"),
                DBValue.Create(1.15),
                DBValue.Create(35.666)
            };
            var cockfightRow = new List<DBValue>() {
                DBValue.Create(Guid.NewGuid()),
                roosterRow1[0],
                roosterRow2[0],
                DBValue.Create(ConversionOf(Cockfight.Result.Suspended)),
                DBValue.Create(158172591M),
                DBValue.Create(58.3162241)
            };

            // Act
            var depot = new EntityDepot();
            var translator = new Translator(t => depot[t]);
            var roosterTranslation = translator[typeof(Cockfight.Rooster)];
            var rooster1 = (Cockfight.Rooster)roosterTranslation.Principal.Reconstitutor.ReconstituteFrom(roosterRow1);
            var rooster2 = (Cockfight.Rooster)roosterTranslation.Principal.Reconstitutor.ReconstituteFrom(roosterRow2);
            depot.StoreEntity(rooster1);
            depot.StoreEntity(rooster2);
            var cockfighTranslation = translator[typeof(Cockfight)];
            var cockfight = (Cockfight)cockfighTranslation.Principal.Reconstitutor.ReconstituteFrom(cockfightRow);

            // Assert
            cockfightRow[0].Datum.Should().Be(cockfight.FightID);
            cockfight.CompetitorA.Should().BeSameAs(rooster1);
            cockfight.CompetitorB.Should().BeSameAs(rooster2);
            cockfightRow[3].Datum.Should().Be(ConversionOf(cockfight.Outcome));
            cockfightRow[4].Datum.Should().Be(cockfight.Pot);
            cockfightRow[5].Datum.Should().Be(cockfight.FightDuration);
        }

        [TestMethod] public void WriteableProperties_NonNullReferenceMultiFieldPrimaryKey() {
            // Arrange
            var rangeRow = new List<DBValue>() {
                DBValue.Create("Eastern Rift Mountains"),
                DBValue.Create(ConversionOf(Glacier.Continent.Africa)),
                DBValue.Create((ushort)1319),
                DBValue.Create(4000UL),
                DBValue.Create(5895UL)
            };
            var glacierRow = new List<DBValue>() {
                DBValue.Create("Furtwängler Glacier"),
                DBValue.Create(13.61),
                rangeRow[0],
                rangeRow[1],
                rangeRow[2],
                DBValue.Create(false)
            };

            // Act
            var depot = new EntityDepot();
            var translator = new Translator(t => depot[t]);
            var rangeTranslation = translator[typeof(Glacier.MountainRange)];
            var range = (Glacier.MountainRange)rangeTranslation.Principal.Reconstitutor.ReconstituteFrom(rangeRow);
            depot.StoreEntity(range);
            var glacierTranslation = translator[typeof(Glacier)];
            var glacier = (Glacier)glacierTranslation.Principal.Reconstitutor.ReconstituteFrom(glacierRow);

            // Assert
            glacierRow[0].Datum.Should().Be(glacier.Name);
            glacierRow[1].Datum.Should().Be(glacier.Length);
            glacier.Range.Should().BeSameAs(range);
            glacierRow[5].Datum.Should().Be(glacier.HasMushroomRock);
        }

        [TestMethod] public void WriteableProperties_NullReferenceSingleFieldPrimaryKey() {
            // Arrange
            var row = new List<DBValue>() {
                DBValue.Create("Allegory of the Sunken Shipwrecks"),
                DBValue.NULL,
                DBValue.Create("shipwrecks full of treasure"),
                DBValue.Create("humanity (as a whole)"),
                DBValue.Create(false)
            };

            // Act
            var depot = new EntityDepot();
            var translator = new Translator(t => depot[t]);
            var translation = translator[typeof(Allegory)];
            var allegory = (Allegory)translation.Principal.Reconstitutor.ReconstituteFrom(row);

            // Assert
            row[0].Datum.Should().Be(allegory.Name);
            allegory.Source.Should().BeNull();
            row[2].Datum.Should().Be(allegory.ComparisonSource);
            row[3].Datum.Should().Be(allegory.ComparisonTarget);
            row[4].Datum.Should().Be(allegory.WellUnderstood);
        }

        [TestMethod] public void WriteableProperties_NullReferenceMultiFieldPrimaryKey() {
            // Arrange
            var personRow = new List<DBValue>() {
                DBValue.Create("Stuart"),
                DBValue.Create("Gaffolino"),
                DBValue.Create(new DateTime(1961, 1, 19)),
                DBValue.Create("Malta")
            };
            var clubRow = new List<DBValue>() {
                DBValue.Create("The Naked Afternoon"),
                DBValue.Create(new DateTime(2001, 9, 12)),
                DBValue.NULL,
                personRow[0],
                personRow[1],
                DBValue.Create(287654.04M),
                DBValue.NULL,
                DBValue.NULL,
                DBValue.Create((ushort)29),
                DBValue.Create(true)
            };

            // Act
            var depot = new EntityDepot();
            var translator = new Translator(t => depot[t]);
            var personTranslation = translator[typeof(StripClub.Person)];
            var person = (StripClub.Person)personTranslation.Principal.Reconstitutor.ReconstituteFrom(personRow);
            depot.StoreEntity(person);
            var clubTranslation = translator[typeof(StripClub)];
            var stripClub = (StripClub)clubTranslation.Principal.Reconstitutor.ReconstituteFrom(clubRow);

            // Assert
            clubRow[0].Datum.Should().Be(stripClub.Name);
            clubRow[1].Datum.Should().Be(stripClub.Opened);
            stripClub.Closed.Should().BeNull();
            stripClub.Proprietor.Should().BeSameAs(person);
            clubRow[5].Datum.Should().Be(stripClub.AnnualRevenue);
            stripClub.PrimaryStripper.Should().BeNull();
            clubRow[8].Datum.Should().Be(stripClub.NumEmployees);
            clubRow[9].Datum.Should().Be(stripClub.HasPrivateRoom);
        }

        [TestMethod] public void ReferenceNestedDataConversion() {
            // Arrange
            var painterRow = new List<DBValue>() {
                DBValue.Create("MICHELANGELO"),
                DBValue.Create(new DateTime(1475, 3, 6)),
                DBValue.Create("Italy"),
                DBValue.Create(true)
            };
            var frescoRow = new List<DBValue>() {
                DBValue.Create(Guid.NewGuid()),
                DBValue.Create("The Creation of Adam"),
                painterRow[0],
                DBValue.Create(9.167f),
                DBValue.Create(18.667f),
                DBValue.Create(ConversionOf(Fresco.Surface.Ceiling))
            };

            // Act
            var depot = new EntityDepot();
            var translator = new Translator(t => depot[t]);
            var painterTranslation = translator[typeof(Fresco.Painter)];
            var painter = (Fresco.Painter)painterTranslation.Principal.Reconstitutor.ReconstituteFrom(painterRow);
            depot.StoreEntity(painter);
            var frescoTranslation = translator[typeof(Fresco)];
            var fresco = (Fresco)frescoTranslation.Principal.Reconstitutor.ReconstituteFrom(frescoRow);

            // Assert
            frescoRow[0].Datum.Should().Be(fresco.PaintingID);
            frescoRow[1].Datum.Should().Be(fresco.Title);
            fresco.Artist.Should().BeSameAs(painter);
            frescoRow[3].Datum.Should().Be(fresco.Length);
            frescoRow[4].Datum.Should().Be(fresco.Width);
            frescoRow[5].Datum.Should().Be(ConversionOf(fresco.PaintedOn));
        }

        [TestMethod] public void CalculatedAndNonCalculatedReferenceInSameEntity() {
            // Arrange
            var companyRow = new List<DBValue>() {
                DBValue.Create("GO.KARTS.GLOBAL."),
                DBValue.Create(285UL),
                DBValue.NULL,
                DBValue.Create(new DateTime(1987, 4, 14))
            };
            var gokartRow = new List<DBValue>() {
                DBValue.Create(Guid.NewGuid()),
                DBValue.Create((ushort)35),
                companyRow[0],
                companyRow[0],
                DBValue.Create(true),
                DBValue.Create((byte)4)
            };

            // Act
            var depot = new EntityDepot();
            var translator = new Translator(t => depot[t]);
            var companyTranslation = translator[typeof(GoKart.Company)];
            var company = (GoKart.Company)companyTranslation.Principal.Reconstitutor.ReconstituteFrom(companyRow);
            depot.StoreEntity(company);
            var gokartTranslation = translator[typeof(GoKart)];
            var gokart = (GoKart)gokartTranslation.Principal.Reconstitutor.ReconstituteFrom(gokartRow);

            // Assert
            gokartRow[0].Datum.Should().Be(gokart.GoKartID);
            gokartRow[1].Datum.Should().Be(gokart.TopSpeed);
            gokart.Manufacturer.Should().BeSameAs(company);
            gokart.Operator.Should().BeSameAs(company);
            gokartRow[4].Datum.Should().Be(gokart.DriverOnLeft);
            gokartRow[5].Datum.Should().Be(gokart.NumWheels);
        }

        [TestMethod] public void EmptyRelations() {
            // Arrange
            var boardRow = new List<DBValue>() {
                DBValue.Create(Guid.NewGuid()),
                DBValue.Create("Cherry Wood"),
                DBValue.Create(175.66604)
            };
            var cheeseRows = new List<List<DBValue>>();
            var meatRows = new List<List<DBValue>>();
            var sauceRows = new List<List<DBValue>>();
            var usageRows = new List<List<DBValue>>();

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(CharcuterieBoard)];
            var board = (CharcuterieBoard)translation.Principal.Reconstitutor.ReconstituteFrom(boardRow);
            translation.Relations[0].Repopulator!.Repopulate(board, cheeseRows);
            translation.Relations[1].Repopulator!.Repopulate(board, meatRows);
            translation.Relations[2].Repopulator!.Repopulate(board, sauceRows);
            translation.Relations[3].Repopulator!.Repopulate(board, usageRows);

            // Assert
            board.Cheeses.Count.Should().Be(0);
            board.Meats.Count.Should().Be(0);
            board.Sauces.Count.Should().Be(0);
            board.Usages.Count.Should().Be(0);
        }

        [TestMethod] public void ListSetElements() {
            // Arrange
            var mutantRow = new List<DBValue>() {
                DBValue.Create("Magento"),
                DBValue.Create("Max Eisenhardt"),
                DBValue.Create(true)
            };
            var powerRows = new List<List<DBValue>>() {
                new() { mutantRow[0], DBValue.Create("Magentism Manipulation") },
                new() { mutantRow[0], DBValue.Create("Astral Projection") },
                new() { mutantRow[0], DBValue.Create("Helmet Shielding") },
                new() { mutantRow[0], DBValue.Create("Genius-Level Intelligence") }
            };
            var appearanceRows = new List<List<DBValue>>() {
                new() { mutantRow[0], DBValue.Create(new DateTime(2002, 4, 1)) },
                new() { mutantRow[0], DBValue.Create(new DateTime(2018, 10, 1)) },
                new() { mutantRow[0], DBValue.Create(new DateTime(2019, 7, 1)) },
                new() { mutantRow[0], DBValue.Create(new DateTime(2009, 10, 1)) },
                new() { mutantRow[0], DBValue.Create(new DateTime(2011, 5, 1)) },
                new() { mutantRow[0], DBValue.Create(new DateTime(2012, 5, 1)) },
                new() { mutantRow[0], DBValue.Create(new DateTime(2014, 9, 1)) },
                new() { mutantRow[0], DBValue.Create(new DateTime(2015, 2, 1)) },
                new() { mutantRow[0], DBValue.Create(new DateTime(2015, 7, 1)) },
                new() { mutantRow[0], DBValue.Create(new DateTime(2015, 10, 1)) },
                new() { mutantRow[0], DBValue.Create(new DateTime(2019, 3, 1)) },
                new() { mutantRow[0], DBValue.Create(new DateTime(2021, 1, 1)) },
                new() { mutantRow[0], DBValue.Create(new DateTime(2021, 9, 1)) },
                new() { mutantRow[0], DBValue.Create(new DateTime(2022, 3, 1)) }
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(Mutant)];
            var mutant = (Mutant)translation.Principal.Reconstitutor.ReconstituteFrom(mutantRow);
            translation.Relations[0].Repopulator!.Repopulate(mutant, appearanceRows);
            translation.Relations[1].Repopulator!.Repopulate(mutant, powerRows);

            // Assert
            mutant.Powers.Count.Should().Be(4);
            powerRows[0][1].Datum.Should().Be(mutant.Powers[0]);
            powerRows[1][1].Datum.Should().Be(mutant.Powers[1]);
            powerRows[2][1].Datum.Should().Be(mutant.Powers[2]);
            powerRows[3][1].Datum.Should().Be(mutant.Powers[3]);
            mutant.Appearances.Count.Should().Be(14);
            mutant.Appearances.Contains((DateTime)appearanceRows[0][1].Datum).Should().BeTrue();
            mutant.Appearances.Contains((DateTime)appearanceRows[1][1].Datum).Should().BeTrue();
            mutant.Appearances.Contains((DateTime)appearanceRows[2][1].Datum).Should().BeTrue();
            mutant.Appearances.Contains((DateTime)appearanceRows[3][1].Datum).Should().BeTrue();
            mutant.Appearances.Contains((DateTime)appearanceRows[4][1].Datum).Should().BeTrue();
            mutant.Appearances.Contains((DateTime)appearanceRows[5][1].Datum).Should().BeTrue();
            mutant.Appearances.Contains((DateTime)appearanceRows[6][1].Datum).Should().BeTrue();
            mutant.Appearances.Contains((DateTime)appearanceRows[7][1].Datum).Should().BeTrue();
            mutant.Appearances.Contains((DateTime)appearanceRows[8][1].Datum).Should().BeTrue();
            mutant.Appearances.Contains((DateTime)appearanceRows[9][1].Datum).Should().BeTrue();
            mutant.Appearances.Contains((DateTime)appearanceRows[10][1].Datum).Should().BeTrue();
            mutant.Appearances.Contains((DateTime)appearanceRows[11][1].Datum).Should().BeTrue();
            mutant.Appearances.Contains((DateTime)appearanceRows[12][1].Datum).Should().BeTrue();
            mutant.Appearances.Contains((DateTime)appearanceRows[13][1].Datum).Should().BeTrue();
        }

        [TestMethod] public void MapElements() {
            // Arrange
            var parkRow = new List<DBValue>() {
                DBValue.Create(Guid.NewGuid()),
                DBValue.Create("Kalahari Resort (Wisconsin Dells)"),
                DBValue.Create(true),
                DBValue.Create(65189124UL)
            };
            var slideRows = new List<List<DBValue>>() {
                new() { parkRow[0], DBValue.Create("Elephant's Trunk"), DBValue.Create(270.0) },
                new() { parkRow[0], DBValue.Create("Master Blaster"), DBValue.Create(570.0) },
                new() { parkRow[0], DBValue.Create("Rippling Rhino"), DBValue.Create(185.5) },
                new() { parkRow[0], DBValue.Create("Tanzanian Twister"), DBValue.Create(115.25) },
                new() { parkRow[0], DBValue.Create("Screaming Hyena"), DBValue.Create(60.0) },
                new() { parkRow[0], DBValue.Create("Zig-Zag Zerba"), DBValue.Create(230.0) }
            };
            var admissionRows = new List<List<DBValue>>() {
                new() { parkRow[0], DBValue.Create(ConversionOf(WaterPark.Customer.General)), DBValue.Create(89.99M) },
                new() { parkRow[0], DBValue.Create(ConversionOf(WaterPark.Customer.Veteran)), DBValue.Create(84.99M) }
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(WaterPark)];
            var park = (WaterPark)translation.Principal.Reconstitutor.ReconstituteFrom(parkRow);
            translation.Relations[0].Repopulator!.Repopulate(park, admissionRows);
            translation.Relations[1].Repopulator!.Repopulate(park, slideRows);

            // Assert
            park.WaterSlides.Count.Should().Be(6);
            slideRows[0][2].Datum.Should().Be(park.WaterSlides[(string)slideRows[0][1].Datum]);
            slideRows[1][2].Datum.Should().Be(park.WaterSlides[(string)slideRows[1][1].Datum]);
            slideRows[2][2].Datum.Should().Be(park.WaterSlides[(string)slideRows[2][1].Datum]);
            slideRows[3][2].Datum.Should().Be(park.WaterSlides[(string)slideRows[3][1].Datum]);
            slideRows[4][2].Datum.Should().Be(park.WaterSlides[(string)slideRows[4][1].Datum]);
            slideRows[5][2].Datum.Should().Be(park.WaterSlides[(string)slideRows[5][1].Datum]);
            park.AdmissionPrices.Count.Should().Be(2);
            admissionRows[0][2].Datum.Should().Be(park.AdmissionPrices[WaterPark.Customer.General]);
            admissionRows[1][2].Datum.Should().Be(park.AdmissionPrices[WaterPark.Customer.Veteran]);
        }

        [TestMethod] public void OrderedListElements() {
            var seanceRow = new List<DBValue>() {
                DBValue.Create(new DateTime(2004, 8, 15)),
                DBValue.Create("Xanthar the Deathtalker Supreme"),
                DBValue.Create(1515.00M),
                DBValue.Create((ushort)156)
            };
            var spiritRows = new List<List<DBValue>>() {
                new() { seanceRow[0], seanceRow[1], DBValue.Create(0U), DBValue.Create("Arik Stoneaxe, Lord of the Hill Peoples") },
                new() { seanceRow[0], seanceRow[1], DBValue.Create(1U), DBValue.Create("Hildegarde von Ouverhort IV") },
                new() { seanceRow[0], seanceRow[1], DBValue.Create(2U), DBValue.Create("Kai-Kai") }
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(Seance)];
            var seance = (Seance)translation.Principal.Reconstitutor.ReconstituteFrom(seanceRow);
            translation.Relations[0].Repopulator!.Repopulate(seance, spiritRows);

            // Assert
            seance.SpiritsContacted.Count.Should().Be(3);
            spiritRows[0][3].Datum.Should().Be(seance.SpiritsContacted[0]);
            spiritRows[1][3].Datum.Should().Be(seance.SpiritsContacted[1]);
            spiritRows[2][3].Datum.Should().Be(seance.SpiritsContacted[2]);
        }

        [TestMethod] public void ReadOnlyRelationElements() {
            // Arrange
            var dermatologistRow = new List<DBValue>() {
                DBValue.Create("Dr. Ezra H. Yamanuchi"),
                DBValue.Create(new DateTime(1974, 5, 2)),
                DBValue.Create("44912 S. East Northerly Beach, Suite #666, San Diego, CA, USA 92131"),
                DBValue.Create(true)
            };
            var almaMatersRow = new List<List<DBValue>>() {
                new() { dermatologistRow[0], DBValue.Create(ConversionOf(Dermatologist.Degree.Bachelors)), DBValue.Create("Pepperdine") },
                new() { dermatologistRow[0], DBValue.Create(ConversionOf(Dermatologist.Degree.Doctorate)), DBValue.Create("Penn State") }
            };
            var patientsRow = new List<List<DBValue>>() {
                new() { dermatologistRow[0], DBValue.Create("Susan Offdorf") },
                new() { dermatologistRow[0], DBValue.Create("Mary-Ann Collecki") },
                new() { dermatologistRow[0], DBValue.Create("Russell Davisson") },
                new() { dermatologistRow[0], DBValue.Create("Kristof Nevjuu") },
                new() { dermatologistRow[0], DBValue.Create("Jaime Ule") }
            };
            var salaryRows = new List<List<DBValue>>() {
                new() { dermatologistRow[0], DBValue.Create(0U), DBValue.Create(300000M) },
                new() { dermatologistRow[0], DBValue.Create(1U), DBValue.Create(375000M) },
                new() { dermatologistRow[0], DBValue.Create(2U), DBValue.Create(430000M) }
            };
            var workdayRows = new List<List<DBValue>>() {
                new() { dermatologistRow[0], DBValue.Create(ConversionOf(Dermatologist.Day.Monday)) },
                new() { dermatologistRow[0], DBValue.Create(ConversionOf(Dermatologist.Day.Tuesday)) },
                new() { dermatologistRow[0], DBValue.Create(ConversionOf(Dermatologist.Day.Wednesday)) },
                new() { dermatologistRow[0], DBValue.Create(ConversionOf(Dermatologist.Day.Thursday)) },
                new() { dermatologistRow[0], DBValue.Create(ConversionOf(Dermatologist.Day.Friday)) },
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(Dermatologist)];
            var dermatologist = (Dermatologist)translation.Principal.Reconstitutor.ReconstituteFrom(dermatologistRow);
            translation.Relations[0].Repopulator!.Repopulate(dermatologist, almaMatersRow);
            translation.Relations[1].Repopulator!.Repopulate(dermatologist, patientsRow);
            translation.Relations[2].Repopulator!.Repopulate(dermatologist, salaryRows);
            translation.Relations[3].Repopulator!.Repopulate(dermatologist, workdayRows);

            // Assert
            dermatologist.AlmaMaters.Count.Should().Be(2);
            almaMatersRow[0][2].Datum.Should().Be(dermatologist.AlmaMaters[Dermatologist.Degree.Bachelors]);
            almaMatersRow[1][2].Datum.Should().Be(dermatologist.AlmaMaters[Dermatologist.Degree.Doctorate]);
            dermatologist.Patients.Count.Should().Be(5);
            dermatologist.Patients.Contains((string)patientsRow[0][1].Datum).Should().BeTrue();
            dermatologist.Patients.Contains((string)patientsRow[1][1].Datum).Should().BeTrue();
            dermatologist.Patients.Contains((string)patientsRow[2][1].Datum).Should().BeTrue();
            dermatologist.Patients.Contains((string)patientsRow[3][1].Datum).Should().BeTrue();
            dermatologist.Patients.Contains((string)patientsRow[4][1].Datum).Should().BeTrue();
            dermatologist.SalaryHistory.Count.Should().Be(3);
            salaryRows[0][2].Datum.Should().Be(dermatologist.SalaryHistory[0]);
            salaryRows[1][2].Datum.Should().Be(dermatologist.SalaryHistory[1]);
            salaryRows[2][2].Datum.Should().Be(dermatologist.SalaryHistory[2]);
            dermatologist.Workdays.Count.Should().Be(5);
            dermatologist.Workdays[0].Should().Be(Dermatologist.Day.Monday);
            dermatologist.Workdays[1].Should().Be(Dermatologist.Day.Tuesday);
            dermatologist.Workdays[2].Should().Be(Dermatologist.Day.Wednesday);
            dermatologist.Workdays[3].Should().Be(Dermatologist.Day.Thursday);
            dermatologist.Workdays[4].Should().Be(Dermatologist.Day.Friday);
        }

        [TestMethod] public void NullRelations() {
            // Arrange
            var chickenRow = new List<DBValue>() {
                DBValue.Create("Crazy Clucker's Friend Chicken Wonderland"),
                DBValue.Create(ConversionOf(FriedChicken.ChickenPart.Wing)),
                DBValue.Create((byte)52),
                DBValue.Create(8.95M),
                DBValue.Create("Cornflake"),
                DBValue.Create(true)
            };
            var spiceRows = new List<List<DBValue>>() {};
            var sauceRows = new List<List<DBValue>>() {};

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(FriedChicken)];
            var chicken = (FriedChicken)translation.Principal.Reconstitutor.ReconstituteFrom(chickenRow);
            translation.Relations[0].Repopulator!.Repopulate(chicken, sauceRows);
            translation.Relations[1].Repopulator!.Repopulate(chicken, spiceRows);

            // Assert
            (chicken.Spices is null).Should().BeTrue();
            (chicken.RecommendedSauces is null).Should().BeTrue();
        }

        [TestMethod] public void RelationNestedInNonNullAggregate() {
            // Arrange
            var limousineRow = new List<DBValue>() {
                DBValue.Create("X4-POOC"),
                DBValue.Create(119.316f),
                DBValue.Create((byte)51),
                DBValue.Create("Nikolai Esterfanofky"),
                DBValue.Create("Anton Chegutai"),
                DBValue.Create("Aleksandra Netashkanova"),
                DBValue.Create(ConversionOf(Limousine.Feature.Tinted | Limousine.Feature.Bulletproof))
            };
            var driverRows = new List<List<DBValue>>() {
                new() { limousineRow[0], DBValue.Create("Sandra el-Avrotai") },
                new() { limousineRow[0], DBValue.Create("Christine O'Hallorrann") },
                new() { limousineRow[0], DBValue.Create("Edna St. Suisses") },
                new() { limousineRow[0], DBValue.Create("Caroline Jomanja") }
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(Limousine)];
            var limousine = (Limousine)translation.Principal.Reconstitutor.ReconstituteFrom(limousineRow);
            translation.Relations[0].Repopulator!.Repopulate(limousine, driverRows);

            // Assert
            limousine.People.LicensedDrivers.Count.Should().Be(4);
            limousine.People.LicensedDrivers.Contains((string)driverRows[0][1].Datum).Should().BeTrue();
            limousine.People.LicensedDrivers.Contains((string)driverRows[1][1].Datum).Should().BeTrue();
            limousine.People.LicensedDrivers.Contains((string)driverRows[2][1].Datum).Should().BeTrue();
            limousine.People.LicensedDrivers.Contains((string)driverRows[3][1].Datum).Should().BeTrue();
        }

        [TestMethod] public void RelationNestedInNullAggregate() {
            // Arrange
            var xenomorphRow = new List<DBValue>() {
                DBValue.Create(Guid.NewGuid()),
                DBValue.Create(10.44),
                DBValue.Create(true),
                DBValue.NULL,
                DBValue.NULL,
                DBValue.Create("Alien: Resurrection")
            };
            var impregnationRows = new List<List<DBValue>>() {};

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(Xenomorph)];
            var xenomorph = (Xenomorph)translation.Principal.Reconstitutor.ReconstituteFrom(xenomorphRow);
            translation.Relations[0].Repopulator!.Repopulate(xenomorph, impregnationRows);

            // Assert
            xenomorph.AlienMurders.Should().BeNull();
        }

        [TestMethod] public void RelationNestedAggregate() {
            // Arrange
            var poemRow = new List<DBValue>() {
                DBValue.Create("Os Lusíadas"),
                DBValue.Create("Luís Vaz de Camões"),
                DBValue.Create(new DateTime(1571, 1, 1)),
                DBValue.Create(1102U),
                DBValue.Create("Portuguese")
            };
            var sectionRows = new List<List<DBValue>>() {
                new() { poemRow[0], DBValue.Create(0U), DBValue.Create("Canto I"), DBValue.NULL },
                new() { poemRow[0], DBValue.Create(1U), DBValue.Create("Canto II"), DBValue.NULL },
                new() { poemRow[0], DBValue.Create(2U), DBValue.Create("Canto III"), DBValue.NULL },
                new() { poemRow[0], DBValue.Create(3U), DBValue.Create("Canto IV"), DBValue.NULL },
                new() { poemRow[0], DBValue.Create(4U), DBValue.Create("Canto V"), DBValue.NULL },
                new() { poemRow[0], DBValue.Create(5U), DBValue.Create("Canto VI"), DBValue.NULL },
                new() { poemRow[0], DBValue.Create(6U), DBValue.Create("Canto VII"), DBValue.NULL },
                new() { poemRow[0], DBValue.Create(7U), DBValue.Create("Canto VIII"), DBValue.NULL },
                new() { poemRow[0], DBValue.Create(8U), DBValue.Create("Canto IX"), DBValue.NULL },
                new() { poemRow[0], DBValue.Create(9U), DBValue.Create("Canto X"), DBValue.NULL }
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(EpicPoem)];
            var poem = (EpicPoem)translation.Principal.Reconstitutor.ReconstituteFrom(poemRow);
            translation.Relations[0].Repopulator!.Repopulate(poem, sectionRows);

            // Assert
            poem.Sections.Count.Should().Be(10);
            sectionRows[0][2].Datum.Should().Be(poem.Sections[0].Name);
            poem.Sections[0].Theme.Should().BeNull();
            sectionRows[1][2].Datum.Should().Be(poem.Sections[1].Name);
            poem.Sections[1].Theme.Should().BeNull();
            sectionRows[2][2].Datum.Should().Be(poem.Sections[2].Name);
            poem.Sections[2].Theme.Should().BeNull();
            sectionRows[3][2].Datum.Should().Be(poem.Sections[3].Name);
            poem.Sections[3].Theme.Should().BeNull();
            sectionRows[4][2].Datum.Should().Be(poem.Sections[4].Name);
            poem.Sections[4].Theme.Should().BeNull();
            sectionRows[5][2].Datum.Should().Be(poem.Sections[5].Name);
            poem.Sections[5].Theme.Should().BeNull();
            sectionRows[6][2].Datum.Should().Be(poem.Sections[6].Name);
            poem.Sections[6].Theme.Should().BeNull();
            sectionRows[7][2].Datum.Should().Be(poem.Sections[7].Name);
            poem.Sections[7].Theme.Should().BeNull();
            sectionRows[8][2].Datum.Should().Be(poem.Sections[8].Name);
            poem.Sections[8].Theme.Should().BeNull();
            sectionRows[9][2].Datum.Should().Be(poem.Sections[9].Name);
            poem.Sections[9].Theme.Should().BeNull();
        }

        [TestMethod] public void RelationNestedReference() {
            // Arrange
            var stopRow = new List<DBValue>() {
                DBValue.Create(new DateTime(2019, 5, 4, 14, 12, 18)),
                DBValue.Create("Aaron Eckelz"),
                DBValue.Create(true),
                DBValue.Create(false)
            };
            var officerRow1 = new List<DBValue>() {
                DBValue.Create("Chicago Police Department"),
                DBValue.Create(18571UL),
                DBValue.Create("Andrew Caskalla"),
                DBValue.Create(false),
            };
            var officerRow2 = new List<DBValue>() {
                DBValue.Create("Los Angeles Police Department"),
                DBValue.Create(75434116UL),
                DBValue.Create("Cassandra Coulienne"),
                DBValue.Create(false)
            };
            var officerRow3 = new List<DBValue>() {
                DBValue.Create("New York Police Department"),
                DBValue.Create(522UL),
                DBValue.Create("Eliezer ben Shalit"),
                DBValue.Create(false)
            };
            var officerRow4 = new List<DBValue>() {
                DBValue.Create("Houston Police Department"),
                DBValue.Create(6324UL),
                DBValue.Create("Jean-Édouard Desdallinierres"),
                DBValue.Create(true)
            };
            var officerRows = new List<List<DBValue>>() {
                new() { stopRow[0], officerRow1[0], officerRow1[1] },
                new() { stopRow[0], officerRow2[0], officerRow2[1] },
                new() { stopRow[0], officerRow4[0], officerRow4[1] },
            };

            // Act
            var depot = new EntityDepot();
            var translator = new Translator(t => depot[t]);
            var officerTranslation = translator[typeof(TrafficStop.PoliceOfficer)];
            var officer1 = (TrafficStop.PoliceOfficer)officerTranslation.Principal.Reconstitutor.ReconstituteFrom(officerRow1);
            var officer2 = (TrafficStop.PoliceOfficer)officerTranslation.Principal.Reconstitutor.ReconstituteFrom(officerRow2);
            var officer3 = (TrafficStop.PoliceOfficer)officerTranslation.Principal.Reconstitutor.ReconstituteFrom(officerRow3);
            var officer4 = (TrafficStop.PoliceOfficer)officerTranslation.Principal.Reconstitutor.ReconstituteFrom(officerRow4);
            depot.StoreEntity(officer1);
            depot.StoreEntity(officer2);
            depot.StoreEntity(officer3);
            depot.StoreEntity(officer4);
            var stopTranslation = translator[typeof(TrafficStop)];
            var stop = (TrafficStop)stopTranslation.Principal.Reconstitutor.ReconstituteFrom(stopRow);
            stopTranslation.Relations[0].Repopulator!.Repopulate(stop, officerRows);

            // Assert
            stop.Officers.Count.Should().Be(3);
            stop.Officers[0].Should().BeSameAs(officer1);
            stop.Officers[1].Should().BeSameAs(officer2);
            stop.Officers[2].Should().BeSameAs(officer4);
        }

        [TestMethod] public void RelationReferencesOwningEntity() {
            // Arrange
            var poacherRow1 = new List<DBValue>() {
                DBValue.Create("Michael Smithy-Batcherino"),
                DBValue.Create(ConversionOf(Poacher.Continent.Asia | Poacher.Continent.SouthAmerica)),
                DBValue.Create(ConversionOf(Poacher.AnimalType.Mammal | Poacher.AnimalType.Bird | Poacher.AnimalType.Arachnid)),
                DBValue.Create(1275281.144M)
            };
            var poacherRow2 = new List<DBValue>() {
                DBValue.Create("Oluwambe Katatumbowaleteta"),
                DBValue.Create(ConversionOf(Poacher.Continent.SouthAmerica | Poacher.Continent.Antarctica)),
                DBValue.Create(ConversionOf(Poacher.AnimalType.Mammal)),
                DBValue.Create(76512.41999M)
            };
            var poacherRows = new List<List<DBValue>>() {
                new() { poacherRow1[0], poacherRow1[0] },
                new() { poacherRow1[0], poacherRow2[0] }
            };

            // Act
            var depot = new EntityDepot();
            var translator = new Translator(t => depot[t]);
            var translation = translator[typeof(Poacher)];
            var poacher1 = (Poacher)translation.Principal.Reconstitutor.ReconstituteFrom(poacherRow1);
            var poacher2 = (Poacher)translation.Principal.Reconstitutor.ReconstituteFrom(poacherRow2);
            depot.StoreEntity(poacher1);
            depot.StoreEntity(poacher2);
            translation.Relations[0].Repopulator!.Repopulate(poacher1, poacherRows);

            // Assert
            poacher1.PoachingGroup.Count.Should().Be(2);
            poacher1.PoachingGroup.Contains(poacher1).Should().BeTrue();
            poacher1.PoachingGroup.Contains(poacher2).Should().BeTrue();
            poacher2.PoachingGroup.Count.Should().Be(0);
        }

        [TestMethod] public void RelationNestedDataConversion() {
            // Arrange
            var guideRow = new List<DBValue>() {
                DBValue.Create((ushort)2023),
                DBValue.Create(315U),
                DBValue.Create(false)
            };
            var restaurantRows = new List<List<DBValue>>() {
                new() { guideRow[0], DBValue.Create("Alinea"), DBValue.Create(3), DBValue.Create('A') },
                new() { guideRow[0], DBValue.Create("Ever"), DBValue.Create(2), DBValue.Create('A') },
                new() { guideRow[0], DBValue.Create("Girl and the Goat"), DBValue.Create(0), DBValue.Create('B') },
                new() { guideRow[0], DBValue.Create("Kasama"), DBValue.Create(1), DBValue.Create('B') }
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(MichelinGuide)];
            var guide = (MichelinGuide)translation.Principal.Reconstitutor.ReconstituteFrom(guideRow);
            translation.Relations[0].Repopulator!.Repopulate(guide, restaurantRows);

            // Assert
            guide.Restaurants.Count.Should().Be(4);
            new Stars().Revert((int)restaurantRows[0][2].Datum).Should().Be(guide.Restaurants[(string)restaurantRows[0][1].Datum].Stars);
            restaurantRows[0][3].Datum.Should().Be(guide.Restaurants[(string)restaurantRows[0][1].Datum].Grade);
            new Stars().Revert((int)restaurantRows[1][2].Datum).Should().Be(guide.Restaurants[(string)restaurantRows[1][1].Datum].Stars);
            restaurantRows[1][3].Datum.Should().Be(guide.Restaurants[(string)restaurantRows[1][1].Datum].Grade);
            new Stars().Revert((int)restaurantRows[2][2].Datum).Should().Be(guide.Restaurants[(string)restaurantRows[2][1].Datum].Stars);
            restaurantRows[2][3].Datum.Should().Be(guide.Restaurants[(string)restaurantRows[2][1].Datum].Grade);
            new Stars().Revert((int)restaurantRows[3][2].Datum).Should().Be(guide.Restaurants[(string)restaurantRows[3][1].Datum].Stars);
            restaurantRows[3][3].Datum.Should().Be(guide.Restaurants[(string)restaurantRows[3][1].Datum].Grade);
        }

        [TestMethod] public void SingleViableConstructor_PublicDefault() {
            // Arrange
            var row = new List<DBValue>() {
                DBValue.Create(Guid.NewGuid()),
                DBValue.Create(ConversionOf(Stove.Fuel.Wood)),
                DBValue.Create((byte)4),
                DBValue.Create(false),
                DBValue.Create("Sir Cooks-a-Lot"),
                DBValue.Create("QFQ515S0JZ")
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(Stove)];
            var stove = (Stove)translation.Principal.Reconstitutor.ReconstituteFrom(row);

            // Assert
            stove.ConstructorCalled().Should().BeTrue();
            row[0].Datum.Should().Be(stove.DeviceID);
            row[1].Datum.Should().Be(ConversionOf(stove.PoweredBy));
            row[2].Datum.Should().Be(stove.NumBurners);
            row[3].Datum.Should().Be(stove.IsInduction);
            row[4].Datum.Should().Be(stove.Brand);
            row[5].Datum.Should().Be(stove.Model);
        }

        [TestMethod] public void SingleViableConstructor_Partial() {
            // Arrange
            var row = new List<DBValue>() {
                DBValue.Create("Maximus Tortorelli"),
                DBValue.Create((ushort)26),
                DBValue.Create(false),
                DBValue.Create("Herculaneum, Roman Empire"),
                DBValue.Create(false)
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(Cannibal)];
            var cannibal = (Cannibal)translation.Principal.Reconstitutor.ReconstituteFrom(row);

            // Assert
            row[0].Datum.Should().Be(cannibal.FullName);
            row[1].Datum.Should().Be(cannibal.NumVictims);
            row[2].Datum.Should().Be(cannibal.Imprisoned);
            row[3].Datum.Should().Be(cannibal.Hometown);
            row[4].Datum.Should().Be(cannibal.Filial);
        }

        [TestMethod] public void SingleViableConstructor_Full() {
            // Arrange
            var row = new List<DBValue>() {
                DBValue.Create(Guid.NewGuid()),
                DBValue.Create((sbyte)9),
                DBValue.Create(14.91f),
                DBValue.Create(true),
                DBValue.Create(127.31M),
                DBValue.Create(false)
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(Menorah)];
            var menorah = (Menorah)translation.Principal.Reconstitutor.ReconstituteFrom(row);

            // Assert
            row[0].Datum.Should().Be(menorah.ProductID);
            row[1].Datum.Should().Be(menorah.CandleHolders);
            row[2].Datum.Should().Be(menorah.Weight);
            row[3].Datum.Should().Be(menorah.IsChannukiah);
            row[4].Datum.Should().Be(menorah.PriceTag);
            row[5].Datum.Should().Be(menorah.DishwasherSafe);
        }

        [TestMethod] public void SingleViableConstructor_NonPublic() {
            // Arrange
            var row = new List<DBValue>() {
                DBValue.Create("Eddie Heythordal"),
                DBValue.Create("Esther McKwivven"),
                DBValue.Create(new DateTime(2016, 11, 2)),
                DBValue.Create("New Stanwix's Third Episcopal Roadside Church"),
                DBValue.Create((ushort)344),
                DBValue.Create(true)
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(Wedding)];
            var wedding = (Wedding)translation.Principal.Reconstitutor.ReconstituteFrom(row);

            // Assert
            row[0].Datum.Should().Be(wedding.Partner1);
            row[1].Datum.Should().Be(wedding.Partner2);
            row[2].Datum.Should().Be(wedding.Date);
            row[3].Datum.Should().Be(wedding.Venue);
            row[4].Datum.Should().Be(wedding.Attendance);
            row[5].Datum.Should().Be(wedding.Outdoor);
        }

        [TestMethod] public void SingleViableConstructor_RecordClassPrimary() {
            // Arrange
            var row = new List<DBValue>() {
                DBValue.Create(31985M),
                DBValue.Create("Oain Gharbheis"),
                DBValue.NULL,
                DBValue.Create(new DateTime(2024, 5, 22)),
                DBValue.NULL,
                DBValue.Create("https://www.gofundme.com/f/heeeeeellllllllpppppppp"),
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(GoFundMe)];
            var goFundMe = (GoFundMe)translation.Principal.Reconstitutor.ReconstituteFrom(row);

            // Assert
            row[0].Datum.Should().Be(goFundMe.AmountRaised);
            row[1].Datum.Should().Be(goFundMe.Beneficiary);
            goFundMe.DateClosed.Should().BeNull();
            row[3].Datum.Should().Be(goFundMe.DateOpened);
            goFundMe.Goal.Should().BeNull();
            row[5].Datum.Should().Be(goFundMe.URL);
        }

        [TestMethod] public void NoViableConstructor_AllWriteableProperties_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Tractor);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<ReconstitutionNotPossibleException>()
                .WithLocation("`Tractor`")
                .WithProblem("there are no viable constructors")
                .EndMessage();
        }

        [TestMethod] public void NoViableConstructor_NoneWithReadOnlyProperties_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Deposition);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<ReconstitutionNotPossibleException>()
                .WithLocation("`Deposition`")
                .WithProblem("there are no viable")
                .EndMessage();
        }

        [TestMethod] public void NoViableConstructor_MultipleWithReadOnlyProperties_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Hypnotist);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<ReconstitutionNotPossibleException>()
                .WithLocation("`Hypnotist`")
                .WithProblem("there are no viable constructors")
                .EndMessage();
        }

        [TestMethod] public void NoViableConstructor_ConvertibleArgument_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Paycheck);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<ReconstitutionNotPossibleException>()
                .WithLocation("`Paycheck`")
                .WithProblem("there are no viable constructors")
                .EndMessage();
        }

        [TestMethod] public void NoViableConstructor_InconvertibleArgument_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Disneyland);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<ReconstitutionNotPossibleException>()
                .WithLocation("`Disneyland`")
                .WithProblem("there are no viable constructors")
                .EndMessage();
        }

        [TestMethod] public void NoViableConstructor_NonNullableArgumentForNullableField_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Bakery);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<ReconstitutionNotPossibleException>()
                .WithLocation("`Bakery`")
                .WithProblem("there are no viable constructors")
                .EndMessage();
        }

        [TestMethod] public void NoViableConstructor_MultipleArgumentsMatchSameField_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(ParticleAccelerator);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<ReconstitutionNotPossibleException>()
                .WithLocation("`ParticleAccelerator`")
                .WithProblem("there are no viable constructors")
                .EndMessage();
        }

        [TestMethod] public void SingleViableConstructor_NullableArgumentForNonNullableField() {
            // Arrange
            var row = new List<DBValue>() {
                DBValue.Create("Ammonia"),
                DBValue.Create("N"),
                DBValue.Create("H3"),
                DBValue.Create(17.03052)
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(CovalentBond)];
            var bond = (CovalentBond)translation.Principal.Reconstitutor.ReconstituteFrom(row);

            // Assert
            row[0].Datum.Should().Be(bond.CommonName);
            row[1].Datum.Should().Be(bond.Anion);
            row[2].Datum.Should().Be(bond.Cation);
            row[3].Datum.Should().Be(bond.MolecularWeight);
        }

        [TestMethod] public void MultipleViableConstructors_DifferentArity() {
            // Arrange
            var row = new List<DBValue>() {
                DBValue.Create("Josefine Montoya"),
                DBValue.Create((ushort)1997),
                DBValue.Create("Santa Fe, New Mexico"),
                DBValue.NULL,
                DBValue.Create(3561784UL),
                DBValue.Create(160M)
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(AmericanGirlDoll)];
            var doll = (AmericanGirlDoll)translation.Principal.Reconstitutor.ReconstituteFrom(row);

            // Assert
            row[0].Datum.Should().Be(doll.Name);
            row[1].Datum.Should().Be(doll.YearReleased);
            row[2].Datum.Should().Be(doll.HomeCity);
            doll.Birthdate.Should().BeNull();
            row[4].Datum.Should().Be(doll.DollsSold);
            row[5].Datum.Should().Be(doll.RetailPrice);
        }

        [TestMethod] public void MultipleViableConstructors_SameAritySameFields_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Ambulance);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<ReconstitutionNotPossibleException>()
                .WithLocation("`Ambulance`")
                .WithProblem("2 constructors are all equally the most viable")
                .EndMessage();
        }

        [TestMethod] public void MultipleViableConstructors_SameArityDifferentFields_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(CustomerServiceLine);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<ReconstitutionNotPossibleException>()
                .WithLocation("`CustomerServiceLine`")
                .WithProblem("2 constructors are all equally the most viable")
                .EndMessage();
        }

        [TestMethod] public void MultipleViableConstructors_BothPublicAndNonPublic() {
            // Arrange
            var row = new List<DBValue>() {
                DBValue.Create(Guid.NewGuid()),
                DBValue.Create("Chicago Winery"),
                DBValue.NULL,
                DBValue.Create(87UL),
                DBValue.Create(51UL),
                DBValue.Create(7UL),
                DBValue.Create(Guid.NewGuid())
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(Winery)];
            var winery = (Winery)translation.Principal.Reconstitutor.ReconstituteFrom(row);

            // Assert
            row[0].Datum.Should().Be(winery.WineryID);
            row[1].Datum.Should().Be(winery.Name);
            winery.LeadSommelier.Should().BeNull();
            row[3].Datum.Should().Be(winery.NumReds);
            row[4].Datum.Should().Be(winery.NumWhites);
            row[5].Datum.Should().Be(winery.NumRoses);
            row[6].Datum.Should().Be(winery.LiquorLicense);
        }

        [TestMethod] public void ViableConstructor_PartialClass() {
            // Arrange
            var row = new List<DBValue>() {
                DBValue.Create("Constructor"),
                DBValue.Create((byte)1),
                DBValue.Create(ConversionOf(Constructor.AccessModifier.Public)),
                DBValue.Create(3),
                DBValue.Create(false),
                DBValue.Create(true)
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(Constructor)];
            var constructor = (Constructor)translation.Principal.Reconstitutor.ReconstituteFrom(row);

            // Assert
            row[0].Datum.Should().Be(constructor.Type);
            row[1].Datum.Should().Be(constructor.Index);
            row[2].Datum.Should().Be(ConversionOf(constructor.Visibility));
            row[3].Datum.Should().Be(constructor.Arity);
            row[4].Datum.Should().Be(constructor.CanThrowException);
            row[5].Datum.Should().Be(constructor.IsExplicit);
        }

        [TestMethod] public void ViableConstructor_RenamedFields() {
            // Arrange
            var row = new List<DBValue>() {
                DBValue.Create(Guid.NewGuid()),
                DBValue.Create(42.54222f),
                DBValue.Create(-8.54278f),
                DBValue.Create("Campo Lomeiro, Spain"),
                DBValue.Create(7.112),
                DBValue.Create("Granite")
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(Petroglyph)];
            var petroglyph = (Petroglyph)translation.Principal.Reconstitutor.ReconstituteFrom(row);

            // Assert
            row[0].Datum.Should().Be(petroglyph.ArchaeologicalIdentifer);
            row[1].Datum.Should().Be(petroglyph.Latitude);
            row[2].Datum.Should().Be(petroglyph.Longtiude);
            row[3].Datum.Should().Be(petroglyph.Location);
            row[4].Datum.Should().Be(petroglyph.Height);
            row[5].Datum.Should().Be(petroglyph.TypeOfRock);
        }

        [TestMethod] public void ViableConstructor_CalculatedField() {
            // Arrange
            var row = new List<DBValue>() {
                DBValue.Create(Guid.NewGuid()),
                DBValue.Create((ushort)250),
                DBValue.Create(0.02),
                DBValue.Create(false),
                DBValue.Create(false)
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(PaintballGun)];
            var gun = (PaintballGun)translation.Principal.Reconstitutor.ReconstituteFrom(row);

            // Assert
            row[0].Datum.Should().Be(gun.ProductID);
            row[1].Datum.Should().Be(gun.PaintballCapacity);
            row[2].Datum.Should().Be(gun.DispatchSpeed);
            row[3].Datum.Should().Be(gun.ProLegal);
            row[4].Datum.Should().Be(gun.Semiautomatic);
        }

        [TestMethod] public void ViableConstructor_Aggregate() {
            // Arrange
            var row = new List<DBValue>() {
                DBValue.Create(Guid.NewGuid()),
                DBValue.Create(173UL),
                DBValue.Create(7221UL),
                DBValue.NULL,
                DBValue.Create(23.37f),
                DBValue.Create(30.11f),
                DBValue.Create((ushort)1074)
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(BaobabTree)];
            var baobab = (BaobabTree)translation.Principal.Reconstitutor.ReconstituteFrom(row);

            // Assert
            row[0].Datum.Should().Be(baobab.TreeID);
            row[1].Datum.Should().Be(baobab.Height);
            row[2].Datum.Should().Be(baobab.RootCoverage);
            baobab.Forest.Should().BeNull();
            row[4].Datum.Should().Be(baobab.ExactLocation.Latitude);
            row[5].Datum.Should().Be(baobab.ExactLocation.Longitude);
            row[6].Datum.Should().Be(baobab.Age);
        }

        [TestMethod] public void ViableConstructor_AggregateWithFieldsRenamedInEntity() {
            // Arrange
            var row = new List<DBValue>() {
                DBValue.Create(Guid.NewGuid()),
                DBValue.Create("NVidia"),
                DBValue.Create("MSI GeForce RTX 4080 Super 16G Expert"),
                DBValue.Create(ConversionOf(GPU.Field.Other)),
                DBValue.Create((ushort)2610),
                DBValue.Create((ushort)16),
                DBValue.Create(true),
                DBValue.Create(false)
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(GPU)];
            var gpu = (GPU)translation.Principal.Reconstitutor.ReconstituteFrom(row);

            // Assert
            row[0].Datum.Should().Be(gpu.ChipID);
            row[1].Datum.Should().Be(gpu.Manufacturer);
            row[2].Datum.Should().Be(gpu.Model);
            row[3].Datum.Should().Be(ConversionOf(gpu.Specification.PrimaryField));
            row[4].Datum.Should().Be(gpu.Specification.MegaHertz);
            row[5].Datum.Should().Be(gpu.Specification.GigaBytes);
            row[6].Datum.Should().Be(gpu.HasVideoCard);
            row[7].Datum.Should().Be(gpu.Discontinued);
        }

        [TestMethod] public void NoViableConstructor_AggregateWithAllWriteableProperties_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Gargoyle);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<ReconstitutionNotPossibleException>()
                .WithLocation("`Gargoyle` → `Rock` (from \"Material\")")
                .WithProblem("there are no viable constructors")
                .EndMessage();
        }
        
        [TestMethod] public void NoViableConstructor_AggregateWithReadOnlyProperties_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(SecurityClearance);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<ReconstitutionNotPossibleException>()
                .WithLocation("`SecurityClearance` → `Position` (from \"GovernmentPosition\")")
                .WithProblem("there are no viable constructors")
                .EndMessage();
        }

        [TestMethod] public void ViableConstructor_RelationNestedAggregate() {
            // Arrange
            var powerRow = new List<DBValue>() {
                DBValue.Create("Psionic Backlash"),
                DBValue.Create(ConversionOf(IllithidPower.Act.Act1)),
                DBValue.Create(ConversionOf(IllithidPower.Kind.Reaction))
            };
            var featureRows = new List<List<DBValue>>() {
                new() { powerRow[0], DBValue.Create("perpetual"), DBValue.Create("new reaction ability"), DBValue.Create(1UL) },
                new() { powerRow[0], DBValue.Create("enemy with 9/30 feet"), DBValue.Create("inflict psychic damage"), DBValue.Create(4UL) }
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(IllithidPower)];
            var power = (IllithidPower)translation.Principal.Reconstitutor.ReconstituteFrom(powerRow);
            translation.Relations[0].Repopulator!.Repopulate(power, featureRows);

            // Assert
            power.Features.Count.Should().Be(2);
            featureRows[0][1].Datum.Should().Be(power.Features[0].Trigger);
            featureRows[0][2].Datum.Should().Be(power.Features[0].Modifier);
            featureRows[0][3].Datum.Should().Be(power.Features[0].Value);
            featureRows[1][1].Datum.Should().Be(power.Features[1].Trigger);
            featureRows[1][2].Datum.Should().Be(power.Features[1].Modifier);
            featureRows[1][3].Datum.Should().Be(power.Features[1].Value);
        }

        [TestMethod] public void NoViableConstructor_CalculatedPropertyType() {
            // Arrange
            var row = new List<DBValue>() {
                DBValue.Create("Wolfgang Amadeus Mozart"),
                DBValue.Create(626),
                DBValue.Create(4),
                DBValue.Create(4),
                DBValue.Create(55.0),
                DBValue.NULL
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(Requiem)];
            var requiem = (Requiem)translation.Principal.Reconstitutor.ReconstituteFrom(row);

            // Assert
            row[0].Datum.Should().Be(requiem.Composer);
            row[1].Datum.Should().Be(requiem.OpusNumber);
            row[2].Datum.Should().Be(requiem.Signature.Top);
            row[3].Datum.Should().Be(requiem.Signature.Bottom);
            row[4].Datum.Should().Be(requiem.Length);
            requiem.Premiered.Should().BeNull();
        }

        [TestMethod] public void ReconstituteThrough_ViablePublicConstructor() {
            // Arrange
            var row = new List<DBValue>() {
                DBValue.Create(Guid.NewGuid()),
                DBValue.Create("Samuel LaFarmalle"),
                DBValue.Create(187581924UL),
                DBValue.Create(5811UL),
                DBValue.Create(ConversionOf(Beekeeper.Classification.Hobby)),
                DBValue.Create(3000)
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(Beekeeper)];
            var beekeeper = (Beekeeper)translation.Principal.Reconstitutor.ReconstituteFrom(row);

            // Assert
            row[0].Datum.Should().Be(beekeeper.BeekeeperID);
            row[1].Datum.Should().Be(beekeeper.Name);
            row[2].Datum.Should().Be(beekeeper.BeesKept);
            row[3].Datum.Should().Be(beekeeper.NumTimesStung);
            row[4].Datum.Should().Be(ConversionOf(beekeeper.Kind));
            row[5].Datum.Should().Be(beekeeper.GallonsHoneyProduced);
        }

        [TestMethod] public void ReconstituteThrough_ViableNonPublicConstructor() {
            // Arrange
            var row = new List<DBValue>() {
                DBValue.Create("Hï Ibiza"),
                DBValue.Create("Playa d'en Bossa, Ibiza, Spain"),
                DBValue.Create(350U),
                DBValue.Create(111.73M),
                DBValue.NULL,
                DBValue.Create((sbyte)7)
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(Nightclub)];
            var nightclub = (Nightclub)translation.Principal.Reconstitutor.ReconstituteFrom(row);

            // Assert
            row[0].Datum.Should().Be(nightclub.ClubName);
            row[1].Datum.Should().Be(nightclub.ClubCity);
            row[2].Datum.Should().Be(nightclub.Capacity);
            row[3].Datum.Should().Be(nightclub.Cover);
            nightclub.LiquorLicense.Should().BeNull();
            row[5].Datum.Should().Be(nightclub.NumBouncers);
        }

        [TestMethod] public void ReconstituteThrough_NonViableConstructor_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Condom);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<ReconstitutionNotPossibleException>()
                .WithLocation("`Condom`")
                .WithProblem("the constructor is not viable")
                .WithAnnotations("[ReconstituteThrough]")
                .EndMessage();
        }

        [TestMethod] public void ReconstituteThrough_MultipleConstructors_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(AntColony);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<ReconstitutionNotPossibleException>()
                .WithLocation("`AntColony`")
                .WithProblem("at most 1 constructor can be annotated, but found 2")
                .WithAnnotations("[ReconstituteThrough]")
                .EndMessage();
        }

        [TestMethod] public void ReconstituteThrough_Aggregate() {
            // Arrange
            var row = new List<DBValue>() {
                DBValue.Create("Yohimbine"),
                DBValue.Create(9.333),
                DBValue.Create("C21H26N2O3"),
                DBValue.Create(false),
                DBValue.NULL
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(Aphrodisiac)];
            var aphrodisiac = (Aphrodisiac)translation.Principal.Reconstitutor.ReconstituteFrom(row);

            // Assert
            row[0].Datum.Should().Be(aphrodisiac.Identifier);
            row[1].Datum.Should().Be(aphrodisiac.Strength);
            row[2].Datum.Should().Be(aphrodisiac.ChemicalStructure!.Value.Formula);
            row[3].Datum.Should().Be(aphrodisiac.ChemicalStructure!.Value.IsAromatic);
            aphrodisiac.DiscoveringCivilization.Should().BeNull();
        }

        [TestMethod] public void ReconstituteThrough_PreDefinedEntity_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Vertebra);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<ReconstitutionNotPossibleException>()
                .WithLocation("`Vertebra`")
                .WithProblem("Pre-Defined Entities do not support Reconstitution")
                .WithAnnotations("[ReconstituteThrough]")
                .EndMessage();
        }

        [TestMethod] public void PublicPreDefinedInstance() {
            // Arrange
            var flourRow = new List<DBValue>() {
                DBValue.Create(Tortilla.Flour.Name),
                DBValue.Create(Tortilla.Flour.IsAuthenticMexican),
                DBValue.Create(Tortilla.Flour.CostcoCost)
            };
            var cornRow = new List<DBValue>() {
                DBValue.Create(Tortilla.Corn.Name),
                DBValue.Create(Tortilla.Corn.IsAuthenticMexican),
                DBValue.Create(Tortilla.Corn.CostcoCost)
            };
            var nopalRow = new List<DBValue>() {
                DBValue.Create(Tortilla.Nopal.Name),
                DBValue.Create(Tortilla.Nopal.IsAuthenticMexican),
                DBValue.Create(Tortilla.Nopal.CostcoCost)
            };
            var wholeWheatRow = new List<DBValue>() {
                DBValue.Create(Tortilla.WholeWheat.Name),
                DBValue.Create(Tortilla.WholeWheat.IsAuthenticMexican),
                DBValue.Create(Tortilla.WholeWheat.CostcoCost)
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(Tortilla)];
            var flourTortilla = (Tortilla)translation.Principal.Reconstitutor.ReconstituteFrom(flourRow);
            var cornTortilla = (Tortilla)translation.Principal.Reconstitutor.ReconstituteFrom(cornRow);
            var nopalTortilla = (Tortilla)translation.Principal.Reconstitutor.ReconstituteFrom(nopalRow);
            var wholeWheatTortilla = (Tortilla)translation.Principal.Reconstitutor.ReconstituteFrom(wholeWheatRow);

            // Assert
            flourTortilla.Should().Be(Tortilla.Flour);
            cornTortilla.Should().Be(Tortilla.Corn);
            nopalTortilla.Should().Be(Tortilla.Nopal);
            wholeWheatTortilla.Should().Be(Tortilla.WholeWheat);
        }

        [TestMethod] public void PublicPreDefinedInstance_IncludeInModel_Redundant() {
            // Arrange
            var row = new List<DBValue>() {
                DBValue.Create(Symbiosis.Mutualism.ID),
                DBValue.Create(Symbiosis.Mutualism.Name),
                DBValue.Create(Symbiosis.Mutualism.Example),
                DBValue.Create(ConversionOf(Symbiosis.Mutualism.Arity))
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(Symbiosis)];
            var symbiosis = (Symbiosis)translation.Principal.Reconstitutor.ReconstituteFrom(row);

            // Assert
            symbiosis.Should().Be(Symbiosis.Mutualism);
        }

        [TestMethod] public void PublicPreDefinedInstance_CodeOnly_PathologicalError() {
            // Arrange
            var row = new List<DBValue>() {
                DBValue.Create(MetricPrefix.Quecto.Prefix),
                DBValue.Create(MetricPrefix.Quecto.Base10),
                DBValue.Create(MetricPrefix.Quecto.YearAdopted),
                DBValue.Create(MetricPrefix.Quecto.Symbol)
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(MetricPrefix)];
            var reconstitute = () => translation.Principal.Reconstitutor.ReconstituteFrom(row);

            // Assert
            reconstitute.Should().ThrowExactly<UnreachableException>();
        }

        [TestMethod] public void NonPublicPreDefinedInstance_PathologicalError() {
            // Arrange
            var row = new List<DBValue>() {
                DBValue.Create("Flagellum"),
                DBValue.Create(null),
                DBValue.Create(false),
                DBValue.Create(true)
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(Organelle)];
            var reconstitute = () => translation.Principal.Reconstitutor.ReconstituteFrom(row);

            // Assert
            reconstitute.Should().ThrowExactly<UnreachableException>();
        }

        [TestMethod] public void NonPublicPreDefinedInstance_CodeOnly_Redundant_PathologicalError() {
            // Arrange
            var row = new List<DBValue>() {
                DBValue.Create(1657081),
                DBValue.Create("Law & Order: LA"),
                DBValue.Create(false),
                DBValue.Create(22),
                DBValue.Create(new DateTime(2010, 9, 29))
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(LawAndOrder)];
            var reconstitute = () => translation.Principal.Reconstitutor.ReconstituteFrom(row);

            // Assert
            reconstitute.Should().ThrowExactly<UnreachableException>();
        }

        [TestMethod] public void NonExistentPreDefinedInstance_PathologicalError() {
            // Arrange
            var row = new List<DBValue>() {
                DBValue.Create("Federative Republic of Brazil"),
                DBValue.Create("Brasília"),
                DBValue.Create(212518750)
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(BrazilianState)];
            var reconstitute = () => translation.Principal.Reconstitutor.ReconstituteFrom(row);

            // Assert
            reconstitute.Should().ThrowExactly<UnreachableException>();
        }

        [TestMethod] public void RelationOnPreDefinedEntity() {
            // Arrange
            var pizzaRollRow = new List<DBValue>() {
                DBValue.Create(PizzaRoll.Pepperoni.ID),
                DBValue.Create(PizzaRoll.Pepperoni.Flavor),
                DBValue.Create(PizzaRoll.Pepperoni.Rating)
            };
            var ingredientsRow = new List<List<DBValue>>(
                PizzaRoll.Pepperoni.Ingredients.Select(v => new List<DBValue>() { DBValue.Create(v) })
            );

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(PizzaRoll)];
            var pizzaRoll = (PizzaRoll)translation.Principal.Reconstitutor.ReconstituteFrom(pizzaRollRow);
            translation.Relations[0].Repopulator!.Repopulate(pizzaRoll, ingredientsRow);

            // Assert
            pizzaRoll.Should().Be(PizzaRoll.Pepperoni);
            pizzaRoll.Ingredients.Count.Should().Be(3);
        }


        private static string ConversionOf<T>(T enumerator) where T : Enum {
            var converter = new EnumToStringConverter(typeof(T)).ConverterImpl;
            return (string)converter.Convert(enumerator)!;
        }
    }
}