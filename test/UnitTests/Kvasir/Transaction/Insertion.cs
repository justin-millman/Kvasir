using FluentAssertions;
using Kvasir.Core;
using Kvasir.Relations;
using Kvasir.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using static UT.Kvasir.Transaction.Insertion;
using static UT.Kvasir.Translation.TestLocalizations;

namespace UT.Kvasir.Transaction {
    [TestClass, TestCategory("Insertion")]
    public class InsertionTests {
        [TestMethod] public async Task SingleInstanceSingleEntityNoNullsNoRelations() {
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
            await fixture.InitializeSchema();
            await fixture.Transactor.Insert([crossbow]);
            var crossbowCmd = fixture.PrincipalCommands<Crossbow>().InsertCommand(ANY_ROWS);
            var crossbowInserts = fixture.InsertionsFor(crossbowCmd);

            // Assert
            crossbowCmd.Connection.Should().Be(fixture.Connection);
            crossbowCmd.Transaction.Should().Be(fixture.Transaction);
            crossbowInserts.Should().HaveCount(1);
            crossbowInserts.Should().ContainRow(crossbow.BowID, crossbow.Brand, crossbow.Model, crossbow.Weight, crossbow.DrawWeight, crossbow.DrawLength);
            fixture.ShouldBeOrdered(crossbowCmd);
            await fixture.Transaction.Received(1).CommitAsync();
        }

        [TestMethod] public async Task SingleInstanceSingleEntityNullsNoRelations() {
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
            await fixture.InitializeSchema();
            await fixture.Transactor.Insert([doodle]);
            var doodleCmd = fixture.PrincipalCommands<GoogleDoodle>().InsertCommand(ANY_ROWS);
            var doodleInserts = fixture.InsertionsFor(doodleCmd);

            // Assert
            doodleCmd.Connection.Should().Be(fixture.Connection);
            doodleCmd.Transaction.Should().Be(fixture.Transaction);
            doodleInserts.Should().HaveCount(1);
            doodleInserts.Should().ContainRow(doodle.Date, doodle.Artist, doodle.IsForHoliday, doodle.IsAnimated, doodle.ArchiveURL);
            fixture.ShouldBeOrdered(doodleCmd);
            await fixture.Transaction.Received(1).CommitAsync();
        }

        [TestMethod] public async Task MultipleInstancesSingleEntityNoRelations() {
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
            await fixture.InitializeSchema();
            await fixture.Transactor.Insert([funcoot, genghis, sham]);
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
            await fixture.Transaction.Received(1).CommitAsync();
        }

        [TestMethod] public async Task SingleInstanceSingleEntityNonEmptyScalarRelations() {
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
                Inspections = [
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
                ]
            };
            var fixture = new TestFixture(typeof(SodaFountain));

            // Act
            await fixture.InitializeSchema();
            await fixture.Transactor.Insert([fountain]);
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
            await fixture.Transaction.Received(1).CommitAsync();
            fountain.Inspections.Should().HaveUnsavedEntryCount(0);
            fountain.Sodas.Should().HaveUnsavedEntryCount(0);
        }

        [TestMethod] public async Task SingleInstanceSingleEntityEmptyScalarRelations() {
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
            await fixture.InitializeSchema();
            await fixture.Transactor.Insert([recLetter]);
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
            await fixture.Transaction.Received(1).CommitAsync();
        }

        [TestMethod] public async Task MultipleInstancesSingleEntityScalarRelations() {
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
            await fixture.InitializeSchema();
            await fixture.Transactor.Insert([fund0, fund1, fund2]);
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
            await fixture.Transaction.Received(1).CommitAsync();
            fund0.Investors.Should().HaveUnsavedEntryCount(0);
            fund1.Investors.Should().HaveUnsavedEntryCount(0);
            fund2.Investors.Should().HaveUnsavedEntryCount(0);
        }

        [TestMethod] public async Task SingleInstanceSingleLocalization() {
            // Arrange
            var password = new Password(Guid.NewGuid());
            password[false] = "Th!s_IS_my_P@sSw0rd1";
            password[true] = "u8124nlkASFi8124posdf-a09i128412";
            var fixture = new TestFixture(typeof(Password));

            // Act
            await fixture.InitializeSchema();
            await fixture.Transactor.Insert([password]);
            var passwordCmd = fixture.PrincipalCommands<Password>().InsertCommand(ANY_ROWS);
            var passwordInserts = fixture.InsertionsFor(passwordCmd);

            // Assert
            passwordCmd.Connection.Should().Be(fixture.Connection);
            passwordCmd.Transaction.Should().Be(fixture.Transaction);
            passwordInserts.Should().HaveCount(2);
            passwordInserts.Should().ContainRow(password.Key, false, password[false]);
            passwordInserts.Should().ContainRow(password.Key, true, password[true]);
            fixture.ShouldBeOrdered(passwordCmd);
            await fixture.Transaction.Received(1).CommitAsync();
            password.Relation.Should().HaveUnsavedEntryCount(0);
        }

        [TestMethod] public async Task MultipleInstancesSingleLocalization() {
            // Arrange
            var first = new Ordinal(1);
            first[OrdinalMode.Suffixed] = "1st";
            first[OrdinalMode.FullySpelled] = "first";
            var twentyThird = new Ordinal(23);
            twentyThird[OrdinalMode.Suffixed] = "23rd";
            twentyThird[OrdinalMode.FullySpelled] = "twenty-third";
            twentyThird[OrdinalMode.Symbolized] = "23°";
            var eightMillionth = new Ordinal(8000000);
            eightMillionth[OrdinalMode.Suffixed] = "8000000th";
            eightMillionth[OrdinalMode.FullySpelled] = "eight millionth";
            var fixture = new TestFixture(typeof(Ordinal));

            // Act
            await fixture.InitializeSchema();
            await fixture.Transactor.Insert([first, twentyThird, eightMillionth]);
            var ordinalCmd = fixture.PrincipalCommands<Ordinal>().InsertCommand(ANY_ROWS);
            var ordinalInserts = fixture.InsertionsFor(ordinalCmd);

            // Assert
            ordinalCmd.Connection.Should().Be(fixture.Connection);
            ordinalCmd.Transaction.Should().Be(fixture.Transaction);
            ordinalInserts.Should().HaveCount(7);
            ordinalInserts.Should().ContainRow(first.Key, ConversionOf(OrdinalMode.Suffixed), first[OrdinalMode.Suffixed]);
            ordinalInserts.Should().ContainRow(first.Key, ConversionOf(OrdinalMode.FullySpelled), first[OrdinalMode.FullySpelled]);
            ordinalInserts.Should().ContainRow(twentyThird.Key, ConversionOf(OrdinalMode.Suffixed), twentyThird[OrdinalMode.Suffixed]);
            ordinalInserts.Should().ContainRow(twentyThird.Key, ConversionOf(OrdinalMode.FullySpelled), twentyThird[OrdinalMode.FullySpelled]);
            ordinalInserts.Should().ContainRow(twentyThird.Key, ConversionOf(OrdinalMode.Symbolized), twentyThird[OrdinalMode.Symbolized]);
            ordinalInserts.Should().ContainRow(eightMillionth.Key, ConversionOf(OrdinalMode.Suffixed), eightMillionth[OrdinalMode.Suffixed]);
            ordinalInserts.Should().ContainRow(eightMillionth.Key, ConversionOf(OrdinalMode.FullySpelled), eightMillionth[OrdinalMode.FullySpelled]);
            fixture.ShouldBeOrdered(ordinalCmd);
            await fixture.Transaction.Received(1).CommitAsync();
            first.Relation.Should().HaveUnsavedEntryCount(0);
            twentyThird.Relation.Should().HaveUnsavedEntryCount(0);
            eightMillionth.Relation.Should().HaveUnsavedEntryCount(0);
        }

        [TestMethod] public async Task MultipleUnrelatedEntities() {
            // Arrange
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
            await fixture.InitializeSchema();
            await fixture.Transactor.Insert([wheelchair, haka, bkad, invoice]);
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
            await fixture.Transaction.Received(1).CommitAsync();
        }

        [TestMethod] public async Task MultipleUnrelatedLocalizations() {
            // Arrange
            var disclaimer = new Disclaimer(Guid.NewGuid());
            disclaimer[Language.Italian] = "non ci assumiamo responsabilità per le ustioni";
            disclaimer[Language.German] = "keine haftung für Verbrennungen";
            var tagline = new Tagline(-81021295);
            tagline[16] = "For When You Have Nothing Else";
            tagline[3] = "When You've Nothing More, That";
            tagline[109] = "At Times Of List Exhaustion";
            var polynomial = new Polynomial("189x^3 - 44x^2 + 1703x + 4");
            polynomial[0] = 4;
            polynomial[1] = 1703;
            polynomial[2] = -44;
            polynomial[3] = 189;
            var fixture = new TestFixture(typeof(Disclaimer), typeof(Tagline), typeof(Polynomial));

            // Act
            await fixture.InitializeSchema();
            await fixture.Transactor.Insert([disclaimer, tagline, polynomial]);
            var disclaimerCmd = fixture.PrincipalCommands<Disclaimer>().InsertCommand(ANY_ROWS);
            var disclaimerInserts = fixture.InsertionsFor(disclaimerCmd);
            var taglineCmd = fixture.PrincipalCommands<Tagline>().InsertCommand(ANY_ROWS);
            var taglineInserts = fixture.InsertionsFor(taglineCmd);
            var polynomialCmd = fixture.PrincipalCommands<Polynomial>().InsertCommand(ANY_ROWS);
            var polynomialInserts = fixture.InsertionsFor(polynomialCmd);

            // Assert
            disclaimerCmd.Connection.Should().Be(fixture.Connection);
            disclaimerCmd.Transaction.Should().Be(fixture.Transaction);
            taglineCmd.Connection.Should().Be(fixture.Connection);
            taglineCmd.Transaction.Should().Be(fixture.Transaction);
            polynomialCmd.Connection.Should().Be(fixture.Connection);
            polynomialCmd.Transaction.Should().Be(fixture.Transaction);
            disclaimerInserts.Should().HaveCount(2);
            disclaimerInserts.Should().ContainRow(disclaimer.Key, ConversionOf(Language.Italian), disclaimer[Language.Italian]);
            disclaimerInserts.Should().ContainRow(disclaimer.Key, ConversionOf(Language.German), disclaimer[Language.German]);
            taglineInserts.Should().HaveCount(3);
            taglineInserts.Should().ContainRow(tagline.Key, 16, tagline[16]);
            taglineInserts.Should().ContainRow(tagline.Key, 3, tagline[3]);
            taglineInserts.Should().ContainRow(tagline.Key, 109, tagline[109]);
            polynomialInserts.Should().HaveCount(4);
            polynomialInserts.Should().ContainRow(polynomial.Key, (byte)0, polynomial[0]);
            polynomialInserts.Should().ContainRow(polynomial.Key, (byte)1, polynomial[1]);
            polynomialInserts.Should().ContainRow(polynomial.Key, (byte)2, polynomial[2]);
            polynomialInserts.Should().ContainRow(polynomial.Key, (byte)3, polynomial[3]);
            fixture.ShouldBeOrdered((disclaimerCmd, taglineCmd, polynomialCmd));
            await fixture.Transaction.Received(1).CommitAsync();
            disclaimer.Relation.Should().HaveUnsavedEntryCount(0);
            tagline.Relation.Should().HaveUnsavedEntryCount(0);
            polynomial.Relation.Should().HaveUnsavedEntryCount(0);
        }

        [TestMethod] public async Task MultipleEntitiesRelatedByReferenceChain() {
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
            await fixture.InitializeSchema();
            await fixture.Transactor.Insert([rhino, zookeeper, zoo]);
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
            await fixture.Transaction.Received(1).CommitAsync();
        }

        [TestMethod] public async Task MultipleEntitiesRelatedByReferenceTree() {
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
            await fixture.InitializeSchema();
            await fixture.Transactor.Insert([sheriff, election, badge]);
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
            await fixture.Transaction.Received(1).CommitAsync();
        }

        [TestMethod] public async Task MultipleEntitiesRelatedByRelation() {
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
            await fixture.InitializeSchema();
            await fixture.Transactor.Insert([witch0, coven, witch1, witch2, witch3]);
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
            await fixture.Transaction.Received(1).CommitAsync();
            coven.Witches.Should().HaveUnsavedEntryCount(0);
        }

        [TestMethod] public async Task MultipleEntitiesRelatedByScalarLocalization() {
            // Arrange
            var measurement = new LocalizedMeasure(1825125444);
            measurement[Translation.TestLocalizations.System.Imperial] = new Measurement(108, "MiB");
            measurement[Translation.TestLocalizations.System.Metric] = new Measurement(108, "MiB");
            var date = new LocalizedDate(Guid.NewGuid());
            date[Calendar.Gregorian] = new DateOnly(2026, 7, 22);
            var leak = new MemoryLeak() {
                Program = "./foo_bar_baz.sh",
                RunNumber = 10903336105,
                MemoryLeaked = measurement,
                DetectedBySanitizer = false,
                IncidentDate = date
            };
            var fixture = new TestFixture(typeof(MemoryLeak), typeof(LocalizedMeasure), typeof(LocalizedDate));

            // Act
            await fixture.InitializeSchema();
            await fixture.Transactor.Insert([date, measurement, leak]);
            var dateCmd = fixture.PrincipalCommands<LocalizedDate>().InsertCommand(ANY_ROWS);
            var dateInserts = fixture.InsertionsFor(dateCmd);
            var measurementCmd = fixture.PrincipalCommands<LocalizedMeasure>().InsertCommand(ANY_ROWS);
            var measurementInserts = fixture.InsertionsFor(measurementCmd);
            var leakCmd = fixture.PrincipalCommands<MemoryLeak>().InsertCommand(ANY_ROWS);
            var leakInserts = fixture.InsertionsFor(leakCmd);

            // Assert
            dateCmd.Connection.Should().Be(fixture.Connection);
            dateCmd.Transaction.Should().Be(fixture.Transaction);
            measurementCmd.Connection.Should().Be(fixture.Connection);
            measurementCmd.Transaction.Should().Be(fixture.Transaction);
            leakCmd.Connection.Should().Be(fixture.Connection);
            leakCmd.Transaction.Should().Be(fixture.Transaction);
            dateInserts.Should().HaveCount(1);
            dateInserts.Should().ContainRow(date.Key, ConversionOf(Calendar.Gregorian), date[Calendar.Gregorian]);
            measurementInserts.Should().HaveCount(2);
            measurementInserts.Should().ContainRow(measurement.Key, ConversionOf(Translation.TestLocalizations.System.Imperial), "MiB", 108.0);
            measurementInserts.Should().ContainRow(measurement.Key, ConversionOf(Translation.TestLocalizations.System.Metric), "MiB", 108.0);
            leakInserts.Should().HaveCount(1);
            leakInserts.Should().ContainRow(leak.Program, leak.RunNumber, leak.MemoryLeaked.Key, leak.DetectedBySanitizer, leak.IncidentDate.Key);
            await fixture.Transaction.Received(1).CommitAsync();
            measurement.Relation.Should().HaveUnsavedEntryCount(0);
            date.Relation.Should().HaveUnsavedEntryCount(0);
        }

        [TestMethod] public async Task MultipleEntitiesRelatedByReferenceLocalization() {
            // Arrange
            var id0 = new PoliceChase.Identifier() {
                Text = "A78192FF-q",
                Numeric = 819212412,
                IsUniquelyIdentifying = true
            };
            var id1 = new PoliceChase.Identifier() {
                Text = "1.a3p+99tth",
                Numeric = 59192040102,
                IsUniquelyIdentifying = true
            };
            var id2 = new PoliceChase.Identifier() {
                Text = "THUAQWRPAS",
                Numeric = 111,
                IsUniquelyIdentifying = false
            };
            var localization = new PoliceChase.LocalizedID(Guid.NewGuid());
            localization["Hafstadt-Glenberg"] = id0;
            localization["ISO-8192"] = id1;
            localization["Qwevnuryl"] = id2;
            var chase = new PoliceChase() {
                Timestamp = new DateTime(2008, 9, 17, 14, 9, 53),
                DurationMinutes = 141.983,
                DatabaseEntry = localization,
                NumOfficersInvolved = 3,
                Culprit = "Ga'Nen Flureid",
                IsVehicular = true
            };
            var fixture = new TestFixture(typeof(PoliceChase), typeof(PoliceChase.Identifier), typeof(PoliceChase.LocalizedID));

            // Act
            await fixture.InitializeSchema();
            await fixture.Transactor.Insert([chase, id0, id1, id2, localization]);
            var chaseCmd = fixture.PrincipalCommands<PoliceChase>().InsertCommand(ANY_ROWS);
            var chases = fixture.InsertionsFor(chaseCmd);
            var identifierCmd = fixture.PrincipalCommands<PoliceChase.Identifier>().InsertCommand(ANY_ROWS);
            var identifiers = fixture.InsertionsFor(identifierCmd);
            var localizationCmd = fixture.PrincipalCommands<PoliceChase.LocalizedID>().InsertCommand(ANY_ROWS);
            var localizations = fixture.InsertionsFor(localizationCmd);

            // Assert
            chaseCmd.Connection.Should().Be(fixture.Connection);
            chaseCmd.Transaction.Should().Be(fixture.Transaction);
            identifierCmd.Connection.Should().Be(fixture.Connection);
            identifierCmd.Transaction.Should().Be(fixture.Transaction);
            localizationCmd.Connection.Should().Be(fixture.Connection);
            localizationCmd.Transaction.Should().Be(fixture.Transaction);
            chases.Should().HaveCount(1);
            chases.Should().ContainRow(chase.Timestamp, chase.DurationMinutes, chase.DatabaseEntry.Key, chase.NumOfficersInvolved, chase.Culprit, chase.IsVehicular);
            identifiers.Should().HaveCount(3);
            identifiers.Should().ContainRow(id0.Text, id0.Numeric, id0.IsUniquelyIdentifying);
            identifiers.Should().ContainRow(id1.Text, id1.Numeric, id1.IsUniquelyIdentifying);
            identifiers.Should().ContainRow(id2.Text, id2.Numeric, id2.IsUniquelyIdentifying);
            localizations.Should().HaveCount(3);
            localizations.Should().ContainRow(localization.Key, "Hafstadt-Glenberg", localization["Hafstadt-Glenberg"].Text, localization["Hafstadt-Glenberg"].Numeric);
            localizations.Should().ContainRow(localization.Key, "ISO-8192", localization["ISO-8192"].Text, localization["ISO-8192"].Numeric);
            localizations.Should().ContainRow(localization.Key, "Qwevnuryl", localization["Qwevnuryl"].Text, localization["Qwevnuryl"].Numeric);
            fixture.ShouldBeOrdered((chaseCmd, identifierCmd));
            fixture.ShouldBeOrdered(identifierCmd, localizationCmd);
            await fixture.Transaction.Received(1).CommitAsync();
            localization.Relation.Should().HaveUnsavedEntryCount(0);
        }

        [TestMethod] public async Task MultipleEntitiesRelatedByRelationLocalization() {
            // Arrange
            var fiftyDollars = new LocalizedCurrency("$50");
            fiftyDollars["dollar"] = 50M;
            fiftyDollars["peso"] = 866.95M;
            fiftyDollars["euro"] = 43.01M;
            var sixtyDollars = new LocalizedCurrency("$60");
            sixtyDollars["yen"] = 9519.99M;
            sixtyDollars["shekel"] = 175.11M;
            var seventyDollars = new LocalizedCurrency("$70");
            seventyDollars["rupee"] = 6725.04M;
            seventyDollars["yuan"] = 476.56M;
            seventyDollars["pound"] = 52.51M;
            seventyDollars["franc"] = 55.12M;
            var takeover = new HostileTakeover() {
                Company = "Der Pfluggenfeldr",
                Date = new DateOnly(2008, 7, 14),
                Executor = "Count Egzmur von Twarrendwelft-Schmulagon",
                TradedShares = 8000000,
                InitialPercentageControlled = 37.54f,
                BuyPrices = [
                    fiftyDollars,
                    sixtyDollars,
                    seventyDollars
                ],
                WasSuccessful = false,
                ProxyFight = true
            };
            var fixture = new TestFixture(typeof(HostileTakeover), typeof(LocalizedCurrency));

            // Act
            await fixture.InitializeSchema();
            await fixture.Transactor.Insert([takeover, fiftyDollars, sixtyDollars, seventyDollars]);
            var takeoverCmd = fixture.PrincipalCommands<HostileTakeover>().InsertCommand(ANY_ROWS);
            var takeoverInserts = fixture.InsertionsFor(takeoverCmd);
            var currencyCmd = fixture.PrincipalCommands<LocalizedCurrency>().InsertCommand(ANY_ROWS);
            var currencyInserts = fixture.InsertionsFor(currencyCmd);
            var pricesCmd = fixture.RelationCommands<HostileTakeover>(0).InsertCommand(ANY_ROWS);
            var priceInserts = fixture.InsertionsFor(pricesCmd);

            // Assert
            takeoverCmd.Connection.Should().Be(fixture.Connection);
            takeoverCmd.Transaction.Should().Be(fixture.Transaction);
            currencyCmd.Connection.Should().Be(fixture.Connection);
            currencyCmd.Transaction.Should().Be(fixture.Transaction);
            pricesCmd.Connection.Should().Be(fixture.Connection);
            pricesCmd.Transaction.Should().Be(fixture.Transaction);
            takeoverInserts.Should().HaveCount(1);
            takeoverInserts.Should().ContainRow(takeover.Company, takeover.Date, takeover.Executor, takeover.TradedShares, takeover.InitialPercentageControlled, takeover.WasSuccessful, takeover.ProxyFight);
            currencyInserts.Should().HaveCount(9);
            currencyInserts.Should().ContainRow(fiftyDollars.Key, "dollar", fiftyDollars["dollar"]);
            currencyInserts.Should().ContainRow(fiftyDollars.Key, "peso", fiftyDollars["peso"]);
            currencyInserts.Should().ContainRow(fiftyDollars.Key, "euro", fiftyDollars["euro"]);
            currencyInserts.Should().ContainRow(sixtyDollars.Key, "yen", sixtyDollars["yen"]);
            currencyInserts.Should().ContainRow(sixtyDollars.Key, "shekel", sixtyDollars["shekel"]);
            currencyInserts.Should().ContainRow(seventyDollars.Key, "rupee", seventyDollars["rupee"]);
            currencyInserts.Should().ContainRow(seventyDollars.Key, "yuan", seventyDollars["yuan"]);
            currencyInserts.Should().ContainRow(seventyDollars.Key, "pound", seventyDollars["pound"]);
            currencyInserts.Should().ContainRow(seventyDollars.Key, "franc", seventyDollars["franc"]);
            priceInserts.Should().HaveCount(3);
            priceInserts.Should().ContainRow(takeover.Company, takeover.Date, fiftyDollars.Key);
            priceInserts.Should().ContainRow(takeover.Company, takeover.Date, sixtyDollars.Key);
            priceInserts.Should().ContainRow(takeover.Company, takeover.Date, seventyDollars.Key);
            fixture.ShouldBeOrdered(takeoverCmd, pricesCmd);
            fixture.ShouldBeOrdered((takeoverCmd, currencyCmd));
            fixture.ShouldBeOrdered((pricesCmd, currencyCmd));
            await fixture.Transaction.Received(1).CommitAsync();
            fiftyDollars.Relation.Should().HaveUnsavedEntryCount(0);
            sixtyDollars.Relation.Should().HaveUnsavedEntryCount(0);
            seventyDollars.Relation.Should().HaveUnsavedEntryCount(0);
        }

        [TestMethod] public async Task SelfReferentialEntityViaRelation() {
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
            await fixture.InitializeSchema();
            await fixture.Transactor.Insert([ixchel, itzamna, yumKaax]);
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
            await fixture.Transaction.Received(1).CommitAsync();
            yumKaax.Mothers.Should().HaveUnsavedEntryCount(0);
            yumKaax.Fathers.Should().HaveUnsavedEntryCount(0);
        }

        [TestMethod] public async Task SelfReferentialEntityViaLocalization() {
            // Arrange
            var pizzeria = new Pizzeria() {
                Franchise = "Olympian Pies",
                StoreNumber = 37,
                AnnualRevenue = 4871062M,
                Operator = "Kristakos Panagonopoulous IV",
                ParentStore = new Pizzeria.LocalizedStore(Guid.NewGuid()),
                NumVarieties = 315,
                PizzaStyle = Pizzeria.Style.ByTheSlice | Pizzeria.Style.ByThePie | Pizzeria.Style.Regular
            };
            pizzeria.ParentStore["official"] = pizzeria;
            pizzeria.ParentStore["unofficial"] = pizzeria;
            var fixture = new TestFixture(typeof(Pizzeria), typeof(Pizzeria.LocalizedStore));

            // Act
            await fixture.InitializeSchema();
            await fixture.Transactor.Insert([pizzeria, pizzeria.ParentStore]);
            var pizzeriaCmd = fixture.PrincipalCommands<Pizzeria>().InsertCommand(ANY_ROWS);
            var pizzeriaInserts = fixture.InsertionsFor(pizzeriaCmd);
            var storeCmd = fixture.PrincipalCommands<Pizzeria.LocalizedStore>().InsertCommand(ANY_ROWS);
            var storeInserts = fixture.InsertionsFor(storeCmd);

            // Assert
            pizzeriaCmd.Connection.Should().Be(fixture.Connection);
            pizzeriaCmd.Transaction.Should().Be(fixture.Transaction);
            storeCmd.Connection.Should().Be(fixture.Connection);
            storeCmd.Transaction.Should().Be(fixture.Transaction);
            pizzeriaInserts.Should().HaveCount(1);
            pizzeriaInserts.Should().ContainRow(pizzeria.Franchise, pizzeria.StoreNumber, pizzeria.AnnualRevenue, pizzeria.Operator, pizzeria.ParentStore.Key, pizzeria.NumVarieties, ConversionOf(pizzeria.PizzaStyle));
            storeInserts.Should().HaveCount(2);
            storeInserts.Should().ContainRow(pizzeria.ParentStore.Key, "official", pizzeria.ParentStore["official"].Franchise, pizzeria.ParentStore["official"].StoreNumber);
            storeInserts.Should().ContainRow(pizzeria.ParentStore.Key, "unofficial", pizzeria.ParentStore["unofficial"].Franchise, pizzeria.ParentStore["unofficial"].StoreNumber);
            fixture.ShouldBeOrdered(pizzeriaCmd, storeCmd);
            await fixture.Transaction.Received(1).CommitAsync();
            pizzeria.ParentStore.Relation.Should().HaveUnsavedEntryCount(0);
        }

        [TestMethod] public async Task TransactionRolledBack() {
            // Arrange
            var blister = new Blister() {
                Person = "Casimir Natchowski",
                DateAcquired = new DateTime(2017, 4, 11),
                Filling = Blister.Substance.Pus,
                CausedByDermititis = true
            };
            var fixture = new TestFixture(typeof(Blister)).WithCommitError();

            // Act
            await fixture.InitializeSchema();
            var action = async () => await fixture.Transactor.Insert([blister]);

            // Assert
            await action.Should().ThrowExactlyAsync<InvalidOperationException>();
            await fixture.Transaction.Received(1).CommitAsync();
            await fixture.Transaction.Received(1).RollbackAsync();
        }

        [TestMethod] public async Task RollbackFails() {
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
            await fixture.InitializeSchema();
            var action = async () => await fixture.Transactor.Insert([chainsaw]);

            // Assert
            await action.Should().ThrowExactlyAsync<AggregateException>();
            await fixture.Transaction.Received(1).CommitAsync();
            await fixture.Transaction.Received(1).RollbackAsync();
        }


        private static string ConversionOf<T>(T enumerator) where T : Enum {
            var converter = new EnumToStringConverter(typeof(T)).ConverterImpl;
            return (string)converter.Convert(enumerator)!;
        }
        private static readonly IEnumerable<IReadOnlyList<DBValue>> ANY_ROWS = [];
    }
}
