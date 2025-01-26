using FluentAssertions;
using Kvasir.Core;
using Kvasir.Relations;
using Kvasir.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;

using static UT.Kvasir.Transaction.Insertion;

namespace UT.Kvasir.Transaction {
    [TestClass, TestCategory("Insertion")]
    public class InsertionTests {
        [TestMethod] public void SingleInstanceSingleEntityNoNullsNoRelations() {
            // Arrange
            var crossbow = new Crossbow {
                BowID = Guid.NewGuid(),
                Brand = "Killer Instinct",
                Model = "1002",
                Weight = 8.0,
                DrawWeight = 185.0,
                DrawLength = 13.0
            };
            var fixture = new TestFixture(typeof(Crossbow));

            // Act
            fixture.Transactor.Insert(new object[] { crossbow });
            var crossbowCmd = fixture.PrincipalCommands<Crossbow>().InsertCommand(ANY_ROWS);
            var crossbowInserts = fixture.InsertionsFor(crossbowCmd);

            // Assert
            crossbowCmd.Connection.Should().Be(fixture.Connection);
            crossbowCmd.Transaction.Should().Be(fixture.Transaction);
            crossbowInserts.Should().HaveCount(1);
            crossbowInserts.Should().ContainRow(crossbow.BowID, crossbow.Brand, crossbow.Model, crossbow.Weight, crossbow.DrawWeight, crossbow.DrawLength);
            fixture.ShouldBeOrdered(crossbowCmd);
            fixture.Transaction.Received(1).Commit();
        }

        [TestMethod] public void SingleInstanceSingleEntityNullsNoRelations() {
            // Arrange
            var doodle = new GoogleDoodle {
                Date = new DateTime(2015, 9, 7),
                Artist = null,
                IsForHoliday = true,
                IsAnimated = false,
                ArchiveURL = "https://doodles.google/doodle/brazils-independence-day-2015/"
            };
            var fixture = new TestFixture(typeof(GoogleDoodle));

            // Act
            fixture.Transactor.Insert(new object[] { doodle });
            var doodleCmd = fixture.PrincipalCommands<GoogleDoodle>().InsertCommand(ANY_ROWS);
            var doodleInserts = fixture.InsertionsFor(doodleCmd);

            // Assert
            doodleCmd.Connection.Should().Be(fixture.Connection);
            doodleCmd.Transaction.Should().Be(fixture.Transaction);
            doodleInserts.Should().HaveCount(1);
            doodleInserts.Should().ContainRow(doodle.Date, doodle.Artist, doodle.IsForHoliday, doodle.IsAnimated, doodle.ArchiveURL);
            fixture.ShouldBeOrdered(doodleCmd);
            fixture.Transaction.Received(1).Commit();
        }

        [TestMethod] public void MultipleInstancesSingleEntityNoRelations() {
            // Arrange
            var funcoot = new CountOlafDisguise() {
                Name = "Al Funcoot",
                Appearances = CountOlafDisguise.Book.RR,
                FooledBaudelaires = false
            };
            var genghis = new CountOlafDisguise() {
                Name = "Coach Genghis",
                Appearances = CountOlafDisguise.Book.AA,
                FooledBaudelaires = false
            };
            var sham = new CountOlafDisguise() {
                Name = "Captain Julio Sham",
                Appearances = CountOlafDisguise.Book.WW,
                FooledBaudelaires = false
            };
            var fixture = new TestFixture(typeof(CountOlafDisguise));

            // Act
            fixture.Transactor.Insert(new object[] { funcoot, genghis, sham });
            var disguiseCmd = fixture.PrincipalCommands<CountOlafDisguise>().InsertCommand(ANY_ROWS);
            var disguiseInserts = fixture.InsertionsFor(disguiseCmd);

            // Assert
            disguiseCmd.Connection.Should().Be(fixture.Connection);
            disguiseCmd.Transaction.Should().Be(fixture.Transaction);
            disguiseInserts.Should().HaveCount(3);
            disguiseInserts.Should().ContainRow(funcoot.Name, ConversionOf(funcoot.Appearances), funcoot.FooledBaudelaires);
            disguiseInserts.Should().ContainRow(genghis.Name, ConversionOf(genghis.Appearances), genghis.FooledBaudelaires);
            disguiseInserts.Should().ContainRow(sham.Name, ConversionOf(sham.Appearances), sham.FooledBaudelaires);
            fixture.ShouldBeOrdered(disguiseCmd);
            fixture.Transaction.Received(1).Commit();
        }

        [TestMethod] public void SingleInstanceSingleEntityNonEmptyScalarRelations() {
            // Arrange
            var fountain = new SodaFountain() {
                ProductID = Guid.NewGuid(),
                Location = "AMC Theaters Chicago 15",
                TypeOfInstitute = SodaFountain.Category.MovieTheater,
                IsCokeFreestyle = true,
                Sodas = new RelationMap<string, bool>() {
                    { "Coke", true },
                    { "Diet Coke", true },
                    { "Coke Zero", false },
                    { "Sprite", true },
                    { "Pibb Xtra", true },
                    { "Dr. Pepper", false },
                    { "Barq's Root Beer", true }
                },
                Inspections = new RelationOrderedList<DateTime>() {
                    new DateTime(2024, 1, 3),
                    new DateTime(2024, 2, 18),
                    new DateTime(2024, 3, 11),
                    new DateTime(2024, 4, 26),
                    new DateTime(2024, 6, 5),
                    new DateTime(2024, 7, 7),
                    new DateTime(2024, 8, 1),
                    new DateTime(2024, 9, 15),
                    new DateTime(2024, 10, 15),
                    new DateTime(2024, 11, 9),
                    new DateTime(2024, 12, 6)
                }
            };
            var fixture = new TestFixture(typeof(SodaFountain));

            // Act
            fixture.Transactor.Insert(new object[] { fountain });
            var fountainCmd = fixture.PrincipalCommands<SodaFountain>().InsertCommand(ANY_ROWS);
            var fountainInserts = fixture.InsertionsFor(fountainCmd);
            var inspectionsCmd = fixture.RelationCommands<SodaFountain>(0).InsertCommand(ANY_ROWS);
            var inspectionsInserts = fixture.InsertionsFor(inspectionsCmd);
            var sodasCmd = fixture.RelationCommands<SodaFountain>(1).InsertCommand(ANY_ROWS);
            var sodasInserts = fixture.InsertionsFor(sodasCmd);

            // Assert
            fountainCmd.Connection.Should().Be(fixture.Connection);
            fountainCmd.Transaction.Should().Be(fixture.Transaction);
            inspectionsCmd.Connection.Should().Be(fixture.Connection);
            inspectionsCmd.Transaction.Should().Be(fixture.Transaction);
            sodasCmd.Connection.Should().Be(fixture.Connection);
            sodasCmd.Transaction.Should().Be(fixture.Transaction);
            fountainInserts.Should().HaveCount(1);
            fountainInserts.Should().ContainRow(fountain.ProductID, fountain.Location, ConversionOf(fountain.TypeOfInstitute), fountain.IsCokeFreestyle);
            inspectionsInserts.Should().HaveCount(11);
            inspectionsInserts.Should().ContainRow(fountain.ProductID, 0U, fountain.Inspections[0]);
            inspectionsInserts.Should().ContainRow(fountain.ProductID, 1U, fountain.Inspections[1]);
            inspectionsInserts.Should().ContainRow(fountain.ProductID, 2U, fountain.Inspections[2]);
            inspectionsInserts.Should().ContainRow(fountain.ProductID, 3U, fountain.Inspections[3]);
            inspectionsInserts.Should().ContainRow(fountain.ProductID, 4U, fountain.Inspections[4]);
            inspectionsInserts.Should().ContainRow(fountain.ProductID, 5U, fountain.Inspections[5]);
            inspectionsInserts.Should().ContainRow(fountain.ProductID, 6U, fountain.Inspections[6]);
            inspectionsInserts.Should().ContainRow(fountain.ProductID, 7U, fountain.Inspections[7]);
            inspectionsInserts.Should().ContainRow(fountain.ProductID, 8U, fountain.Inspections[8]);
            inspectionsInserts.Should().ContainRow(fountain.ProductID, 9U, fountain.Inspections[9]);
            inspectionsInserts.Should().ContainRow(fountain.ProductID, 10U, fountain.Inspections[10]);
            sodasInserts.Should().HaveCount(7);
            sodasInserts.Should().ContainRow(fountain.ProductID, "Coke", fountain.Sodas["Coke"]);
            sodasInserts.Should().ContainRow(fountain.ProductID, "Diet Coke", fountain.Sodas["Diet Coke"]);
            sodasInserts.Should().ContainRow(fountain.ProductID, "Coke Zero", fountain.Sodas["Coke Zero"]);
            sodasInserts.Should().ContainRow(fountain.ProductID, "Sprite", fountain.Sodas["Sprite"]);
            sodasInserts.Should().ContainRow(fountain.ProductID, "Pibb Xtra", fountain.Sodas["Pibb Xtra"]);
            sodasInserts.Should().ContainRow(fountain.ProductID, "Dr. Pepper", fountain.Sodas["Dr. Pepper"]);
            sodasInserts.Should().ContainRow(fountain.ProductID, "Barq's Root Beer", fountain.Sodas["Barq's Root Beer"]);
            fixture.ShouldBeOrdered(fountainCmd, (inspectionsCmd, sodasCmd));
            fixture.Transaction.Received(1).Commit();
            fountain.Inspections.Should().HaveUnsavedEntryCount(0);
            fountain.Sodas.Should().HaveUnsavedEntryCount(0);
        }

        [TestMethod] public void SingleInstanceSingleEntityEmptyScalarRelations() {
            // Arrange
            var recLetter = new LetterOfRecommendation() {
                Author = "Dr. Barry Sfagnoulo",
                Recipient = "Sheila Grandersonsdottir VIII",
                Year = 2020,
                Purpose = "Intercontinental University of the Mystical Sandwich Arts",
                Compensation = null,
                Words = new RelationOrderedList<string>()
            };
            var fixture = new TestFixture(typeof(LetterOfRecommendation));

            // Act
            fixture.Transactor.Insert(new object[] { recLetter });
            var letterCmd = fixture.PrincipalCommands<LetterOfRecommendation>().InsertCommand(ANY_ROWS);
            var letterInserts = fixture.InsertionsFor(letterCmd);
            var wordsCmd = fixture.RelationCommands<LetterOfRecommendation>(0).InsertCommand(ANY_ROWS);
            var wordsInserts = fixture.InsertionsFor(wordsCmd);

            // Assert
            letterCmd.Connection.Should().Be(fixture.Connection);
            letterCmd.Transaction.Should().Be(fixture.Transaction);
            letterInserts.Should().HaveCount(1);
            letterInserts.Should().ContainRow(recLetter.Author, recLetter.Recipient, recLetter.Year, recLetter.Purpose, recLetter.Compensation);
            wordsInserts.Should().HaveCount(0);
            fixture.ShouldBeOrdered(letterCmd);
            fixture.Transaction.Received(1).Commit();
        }

        [TestMethod] public void MultipleInstancesSingleEntityScalarRelations() {
            // Arrange
            var fund0 = new MutualFund() {
                FundID = Guid.NewGuid(),
                CompanyID = Guid.NewGuid(),
                NAV = 163.19M,
                FundManager = "Carlos Telorrio",
                ManagementFee = 0.03,
                Investors = new RelationMap<string, decimal>() {
                    { "Thomas Sirenga", 1800000 },
                    { "Maria W. Spantzutzo", 93250 },
                    { "Andrew O'Vanio", 8915600 }
                }
            };
            var fund1 = new MutualFund() {
                FundID = Guid.NewGuid(),
                CompanyID = Guid.NewGuid(),
                NAV = 34.28M,
                FundManager = "Kent Shoshamish",
                ManagementFee = 0.0625,
                Investors = new RelationMap<string, decimal>() {
                    { "Dorothea Thau", 500000 },
                    { "Henrietta Matt", 7150000 }
                }
            };
            var fund2 = new MutualFund() {
                FundID = Guid.NewGuid(),
                CompanyID = Guid.NewGuid(),
                NAV = 351.99M,
                FundManager = "Donald E. Quaja-Huul",
                ManagementFee = 0.07,
                Investors = new RelationMap<string, decimal>() {
                    { "Thomas Sirenga", 26000000 },
                    { "Dorothea Thau", 18500 },
                    { "Gilbert Gaq", 426295 }
                }
            };
            var fixture = new TestFixture(typeof(MutualFund));

            // Act
            fixture.Transactor.Insert(new object[] { fund0, fund1, fund2 });
            var fundCmd = fixture.PrincipalCommands<MutualFund>().InsertCommand(ANY_ROWS);
            var fundInserts = fixture.InsertionsFor(fundCmd);
            var investorsCmd = fixture.RelationCommands<MutualFund>(0).InsertCommand(ANY_ROWS);
            var investorsInserts = fixture.InsertionsFor(investorsCmd);

            // Assert
            fundCmd.Connection.Should().Be(fixture.Connection);
            fundCmd.Transaction.Should().Be(fixture.Transaction);
            investorsCmd.Connection.Should().Be(fixture.Connection);
            investorsCmd.Transaction.Should().Be(fixture.Transaction);
            fundInserts.Should().HaveCount(3);
            fundInserts.Should().ContainRow(fund0.FundID, fund0.CompanyID, fund0.NAV, fund0.FundManager, fund0.ManagementFee);
            fundInserts.Should().ContainRow(fund1.FundID, fund1.CompanyID, fund1.NAV, fund1.FundManager, fund1.ManagementFee);
            fundInserts.Should().ContainRow(fund2.FundID, fund2.CompanyID, fund2.NAV, fund2.FundManager, fund2.ManagementFee);
            investorsInserts.Should().HaveCount(8);
            investorsInserts.Should().ContainRow(fund0.FundID, fund0.CompanyID, "Thomas Sirenga", fund0.Investors["Thomas Sirenga"]);
            investorsInserts.Should().ContainRow(fund0.FundID, fund0.CompanyID, "Maria W. Spantzutzo", fund0.Investors["Maria W. Spantzutzo"]);
            investorsInserts.Should().ContainRow(fund0.FundID, fund0.CompanyID, "Andrew O'Vanio", fund0.Investors["Andrew O'Vanio"]);
            investorsInserts.Should().ContainRow(fund1.FundID, fund1.CompanyID, "Dorothea Thau", fund1.Investors["Dorothea Thau"]);
            investorsInserts.Should().ContainRow(fund1.FundID, fund1.CompanyID, "Henrietta Matt", fund1.Investors["Henrietta Matt"]);
            investorsInserts.Should().ContainRow(fund2.FundID, fund2.CompanyID, "Thomas Sirenga", fund2.Investors["Thomas Sirenga"]);
            investorsInserts.Should().ContainRow(fund2.FundID, fund2.CompanyID, "Dorothea Thau", fund2.Investors["Dorothea Thau"]);
            investorsInserts.Should().ContainRow(fund2.FundID, fund2.CompanyID, "Gilbert Gaq", fund2.Investors["Gilbert Gaq"]);
            fixture.ShouldBeOrdered(fundCmd, investorsCmd);
            fixture.Transaction.Received(1).Commit();
            fund0.Investors.Should().HaveUnsavedEntryCount(0);
            fund1.Investors.Should().HaveUnsavedEntryCount(0);
            fund2.Investors.Should().HaveUnsavedEntryCount(0);
        }

        [TestMethod] public void MultipleUnrelatedEntities() {
            var wheelchair = new Wheelchair() {
                ProductID = Guid.NewGuid(),
                Material = "Alloy Steel",
                Price = 166.71M,
                MaxWeight = null,
                CompliesWithADA = true
            };
            var haka = new Haka() {
                HakaID = Guid.NewGuid(),
                Leader = null,
                BeforeSportingEvent = true,
                PerformedByMaori = true,
                Duration = 4.11
            };
            var bkad = new BetterKnowADistrict() {
                Season = 6,
                Episode = 23,
                State = "Illinois",
                DistrictNumber = 5,
                Congressperson = "Mike Quigley",
                SegmentDuration = 6.25,
                GeorgeBush = BetterKnowADistrict.Response.GreatestPresident
            };
            var invoice = new Invoice() {
                InvoiceNumber = Guid.NewGuid(),
                Buyer = "Alphonso d'Neuma",
                Seller = "Yessika Hazneri",
                Amount = 14892.61M,
                Date = new DateTime(1994, 12, 19),
                IsElectronic = false
            };
            var fixture = new TestFixture(typeof(Wheelchair), typeof(Haka), typeof(BetterKnowADistrict), typeof(Invoice));

            // Act
            fixture.Transactor.Insert(new object[] { wheelchair, haka, bkad, invoice });
            var wheelchairCmd = fixture.PrincipalCommands<Wheelchair>().InsertCommand(ANY_ROWS);
            var wheelchairInserts = fixture.InsertionsFor(wheelchairCmd);
            var hakaCmd = fixture.PrincipalCommands<Haka>().InsertCommand(ANY_ROWS);
            var hakaInserts = fixture.InsertionsFor(hakaCmd);
            var bkadCmd = fixture.PrincipalCommands<BetterKnowADistrict>().InsertCommand(ANY_ROWS);
            var bkadInserts = fixture.InsertionsFor(bkadCmd);
            var invoiceCmd = fixture.PrincipalCommands<Invoice>().InsertCommand(ANY_ROWS);
            var invoiceInserts = fixture.InsertionsFor(invoiceCmd);

            // Assert
            wheelchairCmd.Connection.Should().Be(fixture.Connection);
            wheelchairCmd.Transaction.Should().Be(fixture.Transaction);
            hakaCmd.Connection.Should().Be(fixture.Connection);
            hakaCmd.Transaction.Should().Be(fixture.Transaction);
            bkadCmd.Connection.Should().Be(fixture.Connection);
            bkadCmd.Transaction.Should().Be(fixture.Transaction);
            invoiceCmd.Connection.Should().Be(fixture.Connection);
            invoiceCmd.Transaction.Should().Be(fixture.Transaction);
            wheelchairInserts.Should().HaveCount(1);
            wheelchairInserts.Should().ContainRow(wheelchair.ProductID, wheelchair.Material, wheelchair.Price, wheelchair.MaxWeight, wheelchair.CompliesWithADA);
            hakaInserts.Should().HaveCount(1);
            hakaInserts.Should().ContainRow(haka.HakaID, haka.Leader, haka.BeforeSportingEvent, haka.PerformedByMaori, haka.Duration);
            bkadInserts.Should().HaveCount(1);
            bkadInserts.Should().ContainRow(bkad.Season, bkad.Episode, bkad.State, bkad.DistrictNumber, bkad.Congressperson, bkad.SegmentDuration, ConversionOf(bkad.GeorgeBush));
            invoiceInserts.Should().HaveCount(1);
            invoiceInserts.Should().ContainRow(invoice.InvoiceNumber, invoice.Buyer, invoice.Seller, invoice.Amount, invoice.Date, invoice.IsElectronic);
            fixture.ShouldBeOrdered((wheelchairCmd, hakaCmd, bkadCmd, invoiceCmd));
            fixture.Transaction.Received(1).Commit();
        }

        [TestMethod] public void MultipleEntitiesRelatedByReferenceChain() {
            // Arrange
            var zookeeper = new Rhinoceros.Person() {
                IntelligenceNumber = Guid.NewGuid(),
                FullName = "Sarrah Hjorkull",
                Birthdate = new DateTime(1984, 5, 5)
            };
            var zoo = new Rhinoceros.Zoo() {
                ZooID = Guid.NewGuid(),
                City = "Milwaukee, Wisconsin",
                HeadZookeeper = zookeeper,
                Area = 372.561,
                DaysOpenPerYear = 358
            };
            var rhino = new Rhinoceros() {
                AnimalID = Guid.NewGuid(),
                Genus = "Dicerorhinus",
                Species = "sumatrensis",
                Captivity = null,
                NumHorns = 2
            };
            var fixture = new TestFixture(typeof(Rhinoceros), typeof(Rhinoceros.Zoo), typeof(Rhinoceros.Person));

            // Act
            fixture.Transactor.Insert(new object[] { rhino, zookeeper, zoo });
            var zookeeperCmd = fixture.PrincipalCommands<Rhinoceros.Person>().InsertCommand(ANY_ROWS);
            var zookeeperInserts = fixture.InsertionsFor(zookeeperCmd);
            var zooCmd = fixture.PrincipalCommands<Rhinoceros.Zoo>().InsertCommand(ANY_ROWS);
            var zooInserts = fixture.InsertionsFor(zooCmd);
            var rhinoCmd = fixture.PrincipalCommands<Rhinoceros>().InsertCommand(ANY_ROWS);
            var rhinoInserts = fixture.InsertionsFor(rhinoCmd);

            // Assert
            zookeeperCmd.Connection.Should().Be(fixture.Connection);
            zookeeperCmd.Transaction.Should().Be(fixture.Transaction);
            zooCmd.Connection.Should().Be(fixture.Connection);
            zooCmd.Transaction.Should().Be(fixture.Transaction);
            rhinoCmd.Connection.Should().Be(fixture.Connection);
            rhinoCmd.Transaction.Should().Be(fixture.Transaction);
            zookeeperInserts.Should().HaveCount(1);
            zookeeperInserts.Should().ContainRow(zookeeper.IntelligenceNumber, zookeeper.FullName, zookeeper.Birthdate);
            zooInserts.Should().HaveCount(1);
            zooInserts.Should().ContainRow(zoo.ZooID, zoo.City, zoo.HeadZookeeper.IntelligenceNumber, zoo.Area, zoo.DaysOpenPerYear);
            rhinoInserts.Should().HaveCount(1);
            rhinoInserts.Should().ContainRow(rhino.AnimalID, rhino.Genus, rhino.Species, rhino.Captivity?.ZooID, rhino.NumHorns);
            fixture.ShouldBeOrdered(zookeeperCmd, zooCmd, rhinoCmd);
            fixture.Transaction.Received(1).Commit();
        }

        [TestMethod] public void MultipleEntitiesRelatedByReferenceTree() {
            // Arrange
            var badge = new Sheriff.Badge() {
                Number = 4516,
                Municipality = "Nottingham",
                DateIssued = new DateTime(1381, 2, 16)
            };
            var election = new Sheriff.Election() {
                ElectionID = Guid.NewGuid(),
                Date = new DateTime(1871, 6, 16),
                VotesCast = 5716259
            };
            var sheriff = new Sheriff() {
                SheriffsBadge = badge,
                Name = "Rev. Connor McStaff",
                Arrests = 5612,
                FirstElected = null
            };
            var fixture = new TestFixture(typeof(Sheriff), typeof(Sheriff.Badge), typeof(Sheriff.Election));

            // Act
            fixture.Transactor.Insert(new object[] { sheriff, election, badge });
            var badgeCmd = fixture.PrincipalCommands<Sheriff.Badge>().InsertCommand(ANY_ROWS);
            var badgeInserts = fixture.InsertionsFor(badgeCmd);
            var electionCmd = fixture.PrincipalCommands<Sheriff.Election>().InsertCommand(ANY_ROWS);
            var electionInserts = fixture.InsertionsFor(electionCmd);
            var sheriffCmd = fixture.PrincipalCommands<Sheriff>().InsertCommand(ANY_ROWS);
            var sheriffInserts = fixture.InsertionsFor(sheriffCmd);

            // Assert
            badgeCmd.Connection.Should().Be(fixture.Connection);
            badgeCmd.Transaction.Should().Be(fixture.Transaction);
            electionCmd.Connection.Should().Be(fixture.Connection);
            electionCmd.Transaction.Should().Be(fixture.Transaction);
            sheriffCmd.Connection.Should().Be(fixture.Connection);
            sheriffCmd.Transaction.Should().Be(fixture.Transaction);
            badgeInserts.Should().HaveCount(1);
            badgeInserts.Should().ContainRow(badge.Number, badge.Municipality, badge.DateIssued);
            electionInserts.Should().HaveCount(1);
            electionInserts.Should().ContainRow(election.ElectionID, election.Date, election.VotesCast);
            sheriffInserts.Should().HaveCount(1);
            sheriffInserts.Should().ContainRow(sheriff.SheriffsBadge.Number, sheriff.SheriffsBadge.Municipality, sheriff.Name, sheriff.Arrests, sheriff.FirstElected?.ElectionID);
            fixture.ShouldBeOrdered((badgeCmd, electionCmd), sheriffCmd);
            fixture.Transaction.Received(1).Commit();
        }

        [TestMethod] public void MultipleEntitiesRelatedByRelation() {
            // Arrange
            var witch0 = new Coven.Witch() {
                Name = "Selma Aftorii",
                SpellcasterNumber = 8764,
                Burned = false,
                Age = 93
            };
            var witch1 = new Coven.Witch() {
                Name = "Darla St. Romero",
                SpellcasterNumber = 2291443,
                Burned = false,
                Age = 419
            };
            var witch2 = new Coven.Witch() {
                Name = "Alanna Lannangannon",
                SpellcasterNumber = 2,
                Burned = true,
                Age = 19
            };
            var witch3 = new Coven.Witch() {
                Name = "Christine de Mauve",
                SpellcasterNumber = 71626,
                Burned = false,
                Age = 2188
            };
            var coven = new Coven() {
                CovenID = Guid.NewGuid(),
                Wicca = false,
                OwnsCauldron = true,
                Witches = new RelationList<Coven.Witch>() { witch0, witch1, witch2 }
            };
            var fixture = new TestFixture(typeof(Coven), typeof(Coven.Witch));

            // Act
            fixture.Transactor.Insert(new object[] { witch0, coven, witch1, witch2, witch3 });
            var covenCmd = fixture.PrincipalCommands<Coven>().InsertCommand(ANY_ROWS);
            var covenInserts = fixture.InsertionsFor(covenCmd);
            var witchCmd = fixture.PrincipalCommands<Coven.Witch>().InsertCommand(ANY_ROWS);
            var witchInserts = fixture.InsertionsFor(witchCmd);
            var membersCmd = fixture.RelationCommands<Coven>(0).InsertCommand(ANY_ROWS);
            var membersInserts = fixture.InsertionsFor(membersCmd);

            // Assert
            covenCmd.Connection.Should().Be(fixture.Connection);
            covenCmd.Transaction.Should().Be(fixture.Transaction);
            witchCmd.Connection.Should().Be(fixture.Connection);
            witchCmd.Transaction.Should().Be(fixture.Transaction);
            membersCmd.Connection.Should().Be(fixture.Connection);
            membersCmd.Transaction.Should().Be(fixture.Transaction);
            covenInserts.Should().HaveCount(1);
            covenInserts.Should().ContainRow(coven.CovenID, coven.Wicca, coven.OwnsCauldron);
            witchInserts.Should().HaveCount(4);
            witchInserts.Should().ContainRow(witch0.Name, witch0.SpellcasterNumber, witch0.Burned, witch0.Age);
            witchInserts.Should().ContainRow(witch1.Name, witch1.SpellcasterNumber, witch1.Burned, witch1.Age);
            witchInserts.Should().ContainRow(witch2.Name, witch2.SpellcasterNumber, witch2.Burned, witch2.Age);
            witchInserts.Should().ContainRow(witch3.Name, witch3.SpellcasterNumber, witch3.Burned, witch3.Age);
            membersInserts.Should().HaveCount(3);
            membersInserts.Should().ContainRow(coven.CovenID, witch0.Name, witch0.SpellcasterNumber);
            membersInserts.Should().ContainRow(coven.CovenID, witch1.Name, witch1.SpellcasterNumber);
            membersInserts.Should().ContainRow(coven.CovenID, witch2.Name, witch2.SpellcasterNumber);
            fixture.ShouldBeOrdered((covenCmd, witchCmd), membersCmd);
            fixture.Transaction.Received(1).Commit();
            coven.Witches.Should().HaveUnsavedEntryCount(0);
        }

        [TestMethod] public void SelfReferentialRelation() {
            // Arrange
            var ixchel = new MayanGod() {
                Name = "Ixchel",
                Attestations = MayanGod.Source.Lacandon,
                Domain = "Midwifery, the Moon",
                Mothers = new RelationSet<MayanGod>(),
                Fathers = new RelationSet<MayanGod>()
            };
            var itzamna = new MayanGod() {
                Name = "Itzamna",
                Attestations = MayanGod.Source.PopolVuh | MayanGod.Source.ChilamBilam | MayanGod.Source.Lacandon,
                Domain = "Creation",
                Mothers = new RelationSet<MayanGod>(),
                Fathers = new RelationSet<MayanGod>()
            };
            var yumKaax = new MayanGod() {
                Name = "Yum Kaax",
                Attestations = MayanGod.Source.Lacandon,
                Domain = "Wild Plants",
                Mothers = new RelationSet<MayanGod>() { ixchel },
                Fathers = new RelationSet<MayanGod>() { itzamna }
            };
            var fixture = new TestFixture(typeof(MayanGod));

            // Act
            fixture.Transactor.Insert(new object[] { ixchel, itzamna, yumKaax });
            var godCmd = fixture.PrincipalCommands<MayanGod>().InsertCommand(ANY_ROWS);
            var godInserts = fixture.InsertionsFor(godCmd);
            var fathersCmd = fixture.RelationCommands<MayanGod>(0).InsertCommand(ANY_ROWS);
            var fathersInserts = fixture.InsertionsFor(fathersCmd);
            var mothersCmd = fixture.RelationCommands<MayanGod>(1).InsertCommand(ANY_ROWS);
            var mothersInserts = fixture.InsertionsFor(mothersCmd);

            // Assert
            godCmd.Connection.Should().Be(fixture.Connection);
            godCmd.Transaction.Should().Be(fixture.Transaction);
            fathersCmd.Connection.Should().Be(fixture.Connection);
            fathersCmd.Transaction.Should().Be(fixture.Transaction);
            mothersCmd.Connection.Should().Be(fixture.Connection);
            mothersCmd.Transaction.Should().Be(fixture.Transaction);
            godInserts.Should().HaveCount(3);
            godInserts.Should().ContainRow(ixchel.Name, ConversionOf(ixchel.Attestations), ixchel.Domain);
            godInserts.Should().ContainRow(itzamna.Name, ConversionOf(itzamna.Attestations), itzamna.Domain);
            godInserts.Should().ContainRow(yumKaax.Name, ConversionOf(yumKaax.Attestations), yumKaax.Domain);
            fathersInserts.Should().HaveCount(1);
            fathersInserts.Should().ContainRow(yumKaax.Name, yumKaax.Fathers.First().Name);
            mothersInserts.Should().HaveCount(1);
            mothersInserts.Should().ContainRow(yumKaax.Name, yumKaax.Mothers.First().Name);
            fixture.ShouldBeOrdered(godCmd, (fathersCmd, mothersCmd));
            fixture.Transaction.Received(1).Commit();
            yumKaax.Mothers.Should().HaveUnsavedEntryCount(0);
            yumKaax.Fathers.Should().HaveUnsavedEntryCount(0);
        }

        [TestMethod] public void TransactionRolledBack() {
            // Arrange
            var blister = new Blister() {
                Person = "Casimir Natchowski",
                DateAcquired = new DateTime(2017, 4, 11),
                Filling = Blister.Substance.Pus,
                CausedByDermititis = true
            };
            var fixture = new TestFixture(typeof(Blister)).WithCommitError();

            // Act
            var action = () => fixture.Transactor.Insert(new object[] { blister });

            // Assert
            action.Should().ThrowExactly<InvalidOperationException>();
            fixture.Transaction.Received(1).Commit();
            fixture.Transaction.Received(1).Rollback();
        }

        [TestMethod] public void RollbackFails() {
            // Arrange
            var chainsaw = new Chainsaw() {
                ProductID = Guid.NewGuid(),
                Manufacturer = "Jujubean",
                Horsepower = 19.2,
                Weight = 2.64f,
                IsBatteryPowered = true
            };
            var fixture = new TestFixture(typeof(Chainsaw)).WithRollbackError();

            // Act
            var action = () => fixture.Transactor.Insert(new object[] { chainsaw });

            // Assert
            action.Should().ThrowExactly<AggregateException>();
            fixture.Transaction.Received(1).Commit();
            fixture.Transaction.Received(1).Rollback();
        }


        private static string ConversionOf<T>(T enumerator) where T : Enum {
            var converter = new EnumToStringConverter(typeof(T)).ConverterImpl;
            return (string)converter.Convert(enumerator)!;
        }
        private static readonly IEnumerable<IReadOnlyList<DBValue>> ANY_ROWS = Enumerable.Empty<IReadOnlyList<DBValue>>();
    }
}
