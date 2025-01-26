using FluentAssertions;
using Kvasir.Core;
using Kvasir.Relations;
using Kvasir.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;

using static UT.Kvasir.Transaction.Update;

namespace UT.Kvasir.Transaction {
    [TestClass, TestCategory("Modification")]
    public class UpdateTests {
        [TestMethod] public void SingleInstanceSingleEntityNoRelations() {
            // Arrange
            var diffEq = new DifferentialEquation() {
                EquationId = Guid.NewGuid(),
                Equation = "dy/dx = y/x + 7",
                IsPartial = false,
                NumSolutions = null
            };
            var fixture = new TestFixture(typeof(DifferentialEquation));

            // Act
            fixture.Transactor.Update(new object[] { diffEq });
            var diffEqCmd = fixture.PrincipalCommands<DifferentialEquation>().UpdateCommand(ANY_ROWS);
            var diffEqUpdates = fixture.UpdatesFor(diffEqCmd);

            // Assert
            diffEqCmd.Connection.Should().Be(fixture.Connection);
            diffEqCmd.Transaction.Should().Be(fixture.Transaction);
            diffEqUpdates.Should().HaveCount(1);
            diffEqUpdates.Should().ContainRow(diffEq.EquationId, diffEq.Equation, diffEq.IsPartial, diffEq.NumSolutions);
            fixture.ShouldBeOrdered(diffEqCmd);
            fixture.Transaction.Received(1).Commit();
        }

        [TestMethod] public void MultipleInstancesSingleEntityNoRelations() {
            // Arrange
            var cortes = new Conquistador() {
                Name = "Hernán Cortés de Monroy y Pizarro Altamirano",
                Conquest = Conquistador.Civilization.Aztec,
                DateOfBirth = new DateTime(1485, 12, 1),
                DateOfDeath = new DateTime(1547, 12, 2)
            };
            var pizarro = new Conquistador() {
                Name = "Francisco Pizarro",
                Conquest = Conquistador.Civilization.Inca,
                DateOfBirth = new DateTime(1478, 3, 16),
                DateOfDeath = new DateTime(1541, 6, 26)
            };
            var fixture = new TestFixture(typeof(Conquistador));

            // Act
            fixture.Transactor.Update(new object[] { cortes, pizarro });
            var conquistadorCmd = fixture.PrincipalCommands<Conquistador>().UpdateCommand(ANY_ROWS);
            var conquistadorUpdates = fixture.UpdatesFor(conquistadorCmd);

            // Assert
            conquistadorCmd.Connection.Should().Be(fixture.Connection);
            conquistadorCmd.Transaction.Should().Be(fixture.Transaction);
            conquistadorUpdates.Should().HaveCount(2);
            conquistadorUpdates.Should().ContainRow(cortes.Name, ConversionOf(cortes.Conquest), cortes.DateOfBirth, cortes.DateOfDeath);
            conquistadorUpdates.Should().ContainRow(pizarro.Name, ConversionOf(pizarro.Conquest), pizarro.DateOfBirth, pizarro.DateOfDeath);
            fixture.ShouldBeOrdered(conquistadorCmd);
            fixture.Transaction.Received(1).Commit();
        }

        [TestMethod] public void SingleInstanceSingleEntityNonEmptyScalarRelationsSavedElements() {
            // Arrange
            var quiz = new BigFatQuiz() {
                Airdate = new DateTime(2024, 12, 27),
                Variety = BigFatQuiz.Kind.OfTheYear,
                YouTubeViews = 139000,
                TelevisionRating = 0.0,
                Teams = new RelationMap<string, BigFatQuiz.Team>() {
                    { "Moose & Loose", new BigFatQuiz.Team() { Player1 = "Richard Ayoade", Player2 = "Katherine Ryan" } },
                    { "Dirty South", new BigFatQuiz.Team() { Player1 = "Rob Beckett", Player2 = "Judi Love" } },
                    { "Angels of the North", new BigFatQuiz.Team() { Player1 = "Chris McCausland", Player2 = "Maisie Adam" } }
                }
            };
            (quiz.Teams as IRelation).Canonicalize();
            var fixture = new TestFixture(typeof(BigFatQuiz));

            // Act
            fixture.Transactor.Update(new object[] { quiz });
            var quizCmd = fixture.PrincipalCommands<BigFatQuiz>().UpdateCommand(ANY_ROWS);
            var quizUpdates = fixture.UpdatesFor(quizCmd);
            var teamsInsertCmd = fixture.RelationCommands<BigFatQuiz>(0).InsertCommand(ANY_ROWS);
            var teamsInsertions = fixture.InsertionsFor(teamsInsertCmd);
            var teamsDeleteCmd = fixture.RelationCommands<BigFatQuiz>(0).DeleteCommand(ANY_ROWS);
            var teamsDeletions = fixture.DeletionsFor(teamsDeleteCmd);
            var teamsUpdateCmd = fixture.RelationCommands<BigFatQuiz>(0).UpdateCommand(ANY_ROWS);
            var teamsUpdates = fixture.UpdatesFor(teamsUpdateCmd);

            // Assert
            quizCmd.Connection.Should().Be(fixture.Connection);
            quizCmd.Transaction.Should().Be(fixture.Transaction);
            quizUpdates.Should().HaveCount(1);
            quizUpdates.Should().ContainRow(quiz.Airdate, ConversionOf(quiz.Variety), quiz.YouTubeViews, quiz.TelevisionRating);
            teamsInsertions.Should().HaveCount(0);
            teamsDeletions.Should().HaveCount(0);
            teamsUpdates.Should().HaveCount(0);
            fixture.ShouldBeOrdered(quizCmd);
            fixture.Transaction.Received(1).Commit();
        }

        [TestMethod] public void SingleInstanceSingleEntityNonEmptyScalarRelationsDeletedElements() {
            // Arrange
            var guacamole = new Guacamole() {
                GuacamoleID = Guid.NewGuid(),
                Preparer = "Chef Miguel C. Espadillo",
                NumServings = 4,
                MadeInMolcajete = true,
                Ingredients = new RelationSet<string>() {
                    "Avocado",
                    "Cilantro",
                    "Jalapeño",
                    "Lime Juice",
                    "Salt"
                }
            };
            (guacamole.Ingredients as IRelation).Canonicalize();
            guacamole.Ingredients.Clear();
            var fixture = new TestFixture(typeof(Guacamole));

            // Act
            fixture.Transactor.Update(new object[] { guacamole });
            var guacamoleCmd = fixture.PrincipalCommands<Guacamole>().UpdateCommand(ANY_ROWS);
            var guacamoleUpdates = fixture.UpdatesFor(guacamoleCmd);
            var ingredientsInsertCmd = fixture.RelationCommands<Guacamole>(0).InsertCommand(ANY_ROWS);
            var ingredientsInsertions = fixture.InsertionsFor(ingredientsInsertCmd);
            var ingredientsDeleteCmd = fixture.RelationCommands<Guacamole>(0).DeleteCommand(ANY_ROWS);
            var ingredientsDeletions = fixture.DeletionsFor(ingredientsDeleteCmd);
            var ingredientsUpdateCmd = fixture.RelationCommands<Guacamole>(0).UpdateCommand(ANY_ROWS);
            var ingredientsUpdates = fixture.UpdatesFor(ingredientsUpdateCmd);

            // Assert
            guacamoleCmd.Connection.Should().Be(fixture.Connection);
            guacamoleCmd.Transaction.Should().Be(fixture.Transaction);
            ingredientsDeleteCmd.Connection.Should().Be(fixture.Connection);
            ingredientsDeleteCmd.Transaction.Should().Be(fixture.Transaction);
            guacamoleUpdates.Should().HaveCount(1);
            guacamoleUpdates.Should().ContainRow(guacamole.GuacamoleID, guacamole.Preparer, guacamole.NumServings, guacamole.MadeInMolcajete);
            ingredientsInsertions.Should().HaveCount(0);
            ingredientsDeletions.Should().HaveCount(5);
            ingredientsDeletions.Should().ContainRow(guacamole.GuacamoleID, "Avocado");
            ingredientsDeletions.Should().ContainRow(guacamole.GuacamoleID, "Cilantro");
            ingredientsDeletions.Should().ContainRow(guacamole.GuacamoleID, "Jalapeño");
            ingredientsDeletions.Should().ContainRow(guacamole.GuacamoleID, "Lime Juice");
            ingredientsDeletions.Should().ContainRow(guacamole.GuacamoleID, "Salt");
            ingredientsUpdates.Should().HaveCount(0);
            fixture.ShouldBeOrdered(guacamoleCmd, ingredientsDeleteCmd);
            fixture.Transaction.Received(1).Commit();
            guacamole.Ingredients.Should().HaveUnsavedEntryCount(0);
        }

        [TestMethod] public void SingleInstanceSingleEntityNonEmptyScalarRelationsNewElements() {
            // Arrange
            var statue = new EquestrianStatue() {
                ArtworkID = Guid.NewGuid(),
                Title = "Gattamelata",
                Artist = "Donatello",
                HorsebackFigure = "Erasmo da Narni",
                NumHorses = 1,
                Dimensions = new RelationMap<EquestrianStatue.Dimension, EquestrianStatue.Measurement>() {
                    { EquestrianStatue.Dimension.Depth, new EquestrianStatue.Measurement() { Value = 780, Unit = EquestrianStatue.Unit.Centimeter } },
                    { EquestrianStatue.Dimension.Width, new EquestrianStatue.Measurement() { Value = 410, Unit = EquestrianStatue.Unit.Centimeter } }
                }
            };
            var fixture = new TestFixture(typeof(EquestrianStatue));

            // Act
            fixture.Transactor.Update(new object[] { statue });
            var statueCmd = fixture.PrincipalCommands<EquestrianStatue>().UpdateCommand(ANY_ROWS);
            var statueUpdates = fixture.UpdatesFor(statueCmd);
            var dimensionsInsertCmd = fixture.RelationCommands<EquestrianStatue>(0).InsertCommand(ANY_ROWS);
            var dimensionsInsertions = fixture.InsertionsFor(dimensionsInsertCmd);
            var dimensionsDeleteCmd = fixture.RelationCommands<EquestrianStatue>(0).DeleteCommand(ANY_ROWS);
            var dimensionsDeletions = fixture.DeletionsFor(dimensionsDeleteCmd);
            var dimensionsUpdateCmd = fixture.RelationCommands<EquestrianStatue>(0).UpdateCommand(ANY_ROWS);
            var dimensionsUpdates = fixture.UpdatesFor(dimensionsUpdateCmd);

            // Assert
            statueCmd.Connection.Should().Be(fixture.Connection);
            statueCmd.Transaction.Should().Be(fixture.Transaction);
            dimensionsInsertCmd.Connection.Should().Be(fixture.Connection);
            dimensionsInsertCmd.Transaction.Should().Be(fixture.Transaction);
            statueUpdates.Should().HaveCount(1);
            statueUpdates.Should().ContainRow(statue.ArtworkID, statue.Title, statue.Artist, statue.HorsebackFigure, statue.NumHorses);
            dimensionsInsertions.Should().HaveCount(2);
            dimensionsInsertions.Should().ContainRow(statue.ArtworkID, ConversionOf(EquestrianStatue.Dimension.Depth), statue.Dimensions[EquestrianStatue.Dimension.Depth].Value, ConversionOf(statue.Dimensions[EquestrianStatue.Dimension.Depth].Unit));
            dimensionsInsertions.Should().ContainRow(statue.ArtworkID, ConversionOf(EquestrianStatue.Dimension.Width), statue.Dimensions[EquestrianStatue.Dimension.Width].Value, ConversionOf(statue.Dimensions[EquestrianStatue.Dimension.Width].Unit));
            dimensionsDeletions.Should().HaveCount(0);
            dimensionsUpdates.Should().HaveCount(0);
            fixture.ShouldBeOrdered(statueCmd, dimensionsInsertCmd);
            fixture.Transaction.Received(1).Commit();
            statue.Dimensions.Should().HaveUnsavedEntryCount(0);
        }

        [TestMethod] public void SingleInstanceSingleEntityNonEmptyScalarRelationsModifiedElements() {
            // Arrange
            var bibliography = new Bibliography() {
                ID = Guid.NewGuid(),
                For = "Anticoagulant Properties of Cnidarian Saliva on Gluten-Free Females Aged 17-23",
                Style = Bibliography.Standard.Harvard,
                NumPages = 218,
                References = new RelationOrderedList<Bibliography.Reference>() {
                    new Bibliography.Reference() {
                        Title = "Nervous Systems of the Animal Kingdom",
                        Author = "F. Maury Cappaneck",
                        TypeOfWork = Bibliography.Literature.Book
                    },
                    new Bibliography.Reference() {
                        Title = "Sally McGilliCuddy: My Experience as a Gluten-Free Woman",
                        Author = "Sally McGilliCuddy",
                        TypeOfWork = Bibliography.Literature.Article
                    },
                    new Bibliography.Reference() {
                        Title = "Jellyfish Stings Local Man",
                        Author = "Gary Zedersztrom",
                        TypeOfWork = Bibliography.Literature.Newscast
                    }
                }
            };
            (bibliography.References as IRelation).Canonicalize();
            bibliography.References[1] = bibliography.References[1] with { TypeOfWork = Bibliography.Literature.Interview };
            var fixture = new TestFixture(typeof(Bibliography));

            // Act
            fixture.Transactor.Update(new object[] { bibliography });
            var bibliographyCmd = fixture.PrincipalCommands<Bibliography>().UpdateCommand(ANY_ROWS);
            var bibliographyUpdates = fixture.UpdatesFor(bibliographyCmd);
            var referencesInsertCmd = fixture.RelationCommands<Bibliography>(0).InsertCommand(ANY_ROWS);
            var referencesInsertions = fixture.InsertionsFor(referencesInsertCmd);
            var referencesDeleteCmd = fixture.RelationCommands<Bibliography>(0).DeleteCommand(ANY_ROWS);
            var referencesDeletions = fixture.DeletionsFor(referencesDeleteCmd);
            var referencesUpdateCmd = fixture.RelationCommands<Bibliography>(0).UpdateCommand(ANY_ROWS);
            var referencesUpdates = fixture.UpdatesFor(referencesUpdateCmd);

            // Assert
            bibliographyCmd.Connection.Should().Be(fixture.Connection);
            bibliographyCmd.Transaction.Should().Be(fixture.Transaction);
            referencesUpdateCmd.Connection.Should().Be(fixture.Connection);
            referencesUpdateCmd.Transaction.Should().Be(fixture.Transaction);
            bibliographyUpdates.Should().HaveCount(1);
            bibliographyUpdates.Should().ContainRow(bibliography.ID, bibliography.For, ConversionOf(bibliography.Style), bibliography.NumPages);
            referencesInsertions.Should().HaveCount(0);
            referencesDeletions.Should().HaveCount(0);
            referencesUpdates.Should().HaveCount(1);
            referencesUpdates.Should().ContainRow(bibliography.ID, 1U, bibliography.References[1].Title, bibliography.References[1].Author, ConversionOf(bibliography.References[1].TypeOfWork));
            fixture.ShouldBeOrdered(bibliographyCmd, referencesUpdateCmd);
            fixture.Transaction.Received(1).Commit();
            bibliography.References.Should().HaveUnsavedEntryCount(0);
        }

        [TestMethod] public void SingleInstanceSingleEntityNonEmptyScalarRelationsMixedElements() {
            // Arrange
            var akuna = new MarketMaker() {
                PrimaryMPID = "4619",
                FirmName = "Akuna Capital",
                NetCapital = 618000000M,
                IsDesignated = false,
                Symbols = new RelationOrderedList<string>() {
                    "NVDA",
                    "IBIT",
                    "APL",
                    "TSLA",
                    "MSFT"
                }
            };
            (akuna.Symbols as IRelation).Canonicalize();
            akuna.Symbols[2] = "AAPL";
            akuna.Symbols.Add("QQQ");
            var fixture = new TestFixture(typeof(MarketMaker));

            // Act
            fixture.Transactor.Update(new object[] { akuna });
            var marketMakerCmd = fixture.PrincipalCommands<MarketMaker>().UpdateCommand(ANY_ROWS);
            var marketMakerUpdates = fixture.UpdatesFor(marketMakerCmd);
            var symbolsInsertCmd = fixture.RelationCommands<MarketMaker>(0).InsertCommand(ANY_ROWS);
            var symbolsInsertions = fixture.InsertionsFor(symbolsInsertCmd);
            var symbolsDeleteCmd = fixture.RelationCommands<MarketMaker>(0).DeleteCommand(ANY_ROWS);
            var symbolsDeletions = fixture.DeletionsFor(symbolsDeleteCmd);
            var symbolsUpdateCmd = fixture.RelationCommands<MarketMaker>(0).UpdateCommand(ANY_ROWS);
            var symbolsUpdates = fixture.UpdatesFor(symbolsUpdateCmd);

            // Assert
            marketMakerCmd.Connection.Should().Be(fixture.Connection);
            marketMakerCmd.Transaction.Should().Be(fixture.Transaction);
            symbolsInsertCmd.Connection.Should().Be(fixture.Connection);
            symbolsInsertCmd.Transaction.Should().Be(fixture.Transaction);
            symbolsUpdateCmd.Connection.Should().Be(fixture.Connection);
            symbolsUpdateCmd.Transaction.Should().Be(fixture.Transaction);
            marketMakerUpdates.Should().HaveCount(1);
            marketMakerUpdates.Should().ContainRow(akuna.PrimaryMPID, akuna.FirmName, akuna.NetCapital, akuna.IsDesignated);
            symbolsInsertions.Should().HaveCount(1);
            symbolsInsertions.Should().ContainRow(akuna.PrimaryMPID, 5U, akuna.Symbols[5]);
            symbolsDeletions.Should().HaveCount(0);
            symbolsUpdates.Should().HaveCount(1);
            symbolsUpdates.Should().ContainRow(akuna.PrimaryMPID, 2U, akuna.Symbols[2]);
            fixture.ShouldBeOrdered(marketMakerCmd, symbolsUpdateCmd, symbolsInsertCmd);
            fixture.Transaction.Received(1).Commit();
            akuna.Symbols.Should().HaveUnsavedEntryCount(0);
        }

        [TestMethod] public void SingleInstanceSingleEntityEmptyScalarRelations() {
            // Arrange
            var scroll = new DeadSeaScroll() {
                Cave = "Qumran Cave 3",
                Identifier = "3QEzek",
                ScrollLanguage = DeadSeaScroll.Language.Hebrew,
                BiblicalSource = "Ezekiel 16:31-33",
                DiscoveryYear = 1952,
                VerifiedAuthors = new RelationSet<string>()
            };
            var fixture = new TestFixture(typeof(DeadSeaScroll));

            // Act
            fixture.Transactor.Update(new object[] { scroll });
            var scrollCmd = fixture.PrincipalCommands<DeadSeaScroll>().UpdateCommand(ANY_ROWS);
            var scrollUpdates = fixture.UpdatesFor(scrollCmd);
            var authorsInsertCmd = fixture.RelationCommands<DeadSeaScroll>(0).InsertCommand(ANY_ROWS);
            var authorsInsertions = fixture.InsertionsFor(authorsInsertCmd);
            var authorsDeleteCmd = fixture.RelationCommands<DeadSeaScroll>(0).DeleteCommand(ANY_ROWS);
            var authorsDeletions = fixture.DeletionsFor(authorsDeleteCmd);
            var authorsUpdateCmd = fixture.RelationCommands<DeadSeaScroll>(0).UpdateCommand(ANY_ROWS);
            var authorsUpdates = fixture.UpdatesFor(authorsUpdateCmd);

            // Assert
            scrollCmd.Connection.Should().Be(fixture.Connection);
            scrollCmd.Transaction.Should().Be(fixture.Transaction);
            scrollUpdates.Should().HaveCount(1);
            scrollUpdates.Should().ContainRow(scroll.Cave, scroll.Identifier, ConversionOf(scroll.ScrollLanguage), scroll.BiblicalSource, scroll.DiscoveryYear);
            authorsInsertions.Should().HaveCount(0);
            authorsDeletions.Should().HaveCount(0);
            authorsUpdates.Should().HaveCount(0);
            fixture.ShouldBeOrdered(scrollCmd);
            fixture.Transaction.Received(1).Commit();
        }

        [TestMethod] public void MultipleInstancesSingleEntityScalarRelations() {
            // Arrange
            var chicago = new Diocese() {
                Name = "Archdiocese of Chicago",
                Church = "Roman Catholic Church",
                IsArchdiocese = true,
                Parishes = 216,
                Bishops = new RelationMap<string, Diocese.DateRange>() {
                    { "Blase J. Cupich", new() { Start = new DateTime(2014, 11, 18), End = null } },
                    { "Francis George", new() { Start = new DateTime(1997, 5, 7), End = new DateTime(2014, 11, 18) } }
                }
            };
            var derby = new Diocese() {
                Name = "Diocese of Derby",
                Church = "Roman Catholic Church",
                IsArchdiocese = false,
                Parishes = 255,
                Bishops = new RelationMap<string, Diocese.DateRange>() {
                    { "Libby Lane", new() { Start = new DateTime(2019, 2, 11), End = null } }
                }
            };
            var perth = new Diocese() {
                Name = "Diocese of Perth",
                Church = "Anglican Church",
                IsArchdiocese = false,
                Parishes = 119,
                Bishops = new RelationMap<string, Diocese.DateRange>() {
                    { "Kay Goldsworthy", new() { Start = new DateTime(2018, 2, 10), End = null } }
                }
            };
            var fixture = new TestFixture(typeof(Diocese));

            // Act
            fixture.Transactor.Update(new object[] { chicago, derby, perth });
            var dioceseCmd = fixture.PrincipalCommands<Diocese>().UpdateCommand(ANY_ROWS);
            var dioceseUpdates = fixture.UpdatesFor(dioceseCmd);
            var bishopsInsertCmd = fixture.RelationCommands<Diocese>(0).InsertCommand(ANY_ROWS);
            var bishopsInsertions = fixture.InsertionsFor(bishopsInsertCmd);
            var bishopsDeleteCmd = fixture.RelationCommands<Diocese>(0).DeleteCommand(ANY_ROWS);
            var bishopsDeletions = fixture.DeletionsFor(bishopsDeleteCmd);
            var bishopsUpdateCmd = fixture.RelationCommands<Diocese>(0).UpdateCommand(ANY_ROWS);
            var bishopsUpdates = fixture.UpdatesFor(bishopsUpdateCmd);

            // Assert
            dioceseCmd.Connection.Should().Be(fixture.Connection);
            dioceseCmd.Transaction.Should().Be(fixture.Transaction);
            bishopsInsertCmd.Connection.Should().Be(fixture.Connection);
            bishopsInsertCmd.Transaction.Should().Be(fixture.Transaction);
            dioceseUpdates.Should().HaveCount(3);
            dioceseUpdates.Should().ContainRow(chicago.Name, chicago.Church, chicago.IsArchdiocese, chicago.Parishes);
            dioceseUpdates.Should().ContainRow(derby.Name, derby.Church, derby.IsArchdiocese, derby.Parishes);
            dioceseUpdates.Should().ContainRow(perth.Name, perth.Church, perth.IsArchdiocese, perth.Parishes);
            bishopsInsertions.Should().HaveCount(4);
            bishopsInsertions.Should().ContainRow(chicago.Name, chicago.Church, "Blase J. Cupich", chicago.Bishops["Blase J. Cupich"].Start, chicago.Bishops["Blase J. Cupich"].End);
            bishopsInsertions.Should().ContainRow(chicago.Name, chicago.Church, "Francis George", chicago.Bishops["Francis George"].Start, chicago.Bishops["Francis George"].End);
            bishopsInsertions.Should().ContainRow(derby.Name, derby.Church, "Libby Lane", derby.Bishops["Libby Lane"].Start, derby.Bishops["Libby Lane"].End);
            bishopsInsertions.Should().ContainRow(perth.Name, perth.Church, "Kay Goldsworthy", perth.Bishops["Kay Goldsworthy"].Start, perth.Bishops["Kay Goldsworthy"].End);
            bishopsDeletions.Should().HaveCount(0);
            bishopsUpdates.Should().HaveCount(0);
            fixture.ShouldBeOrdered(dioceseCmd, bishopsInsertCmd);
            fixture.Transaction.Received(1).Commit();
            chicago.Bishops.Should().HaveUnsavedEntryCount(0);
            derby.Bishops.Should().HaveUnsavedEntryCount(0);
            perth.Bishops.Should().HaveUnsavedEntryCount(0);
        }

        [TestMethod] public void MultipleUnrelatedEntities() {
            // Arrange
            var waltz = new Waltz() {
                PieceTitle = "The Blue Danube",
                Composer = "Johann Strauss II",
                Number = 314,
                Length = 9 + 1/6,
                Premiere = new DateTime(1867, 2, 15),
                DancedToOnDWTS = true
            };
            var pacemaker = new Pacemaker() {
                ProductID = Guid.NewGuid(),
                Recipient = "Robert Tobor",
                InstalledOn = new DateTime(2023, 7, 19),
                InstalledBy = "Dr. Patricia Eversung",
                Pacing = Pacemaker.Method.Epicardial
            };
            var camp = new DemigodCamp() {
                Name = "Camp Half-Blood",
                Location = "New York City",
                Mythology = DemigodCamp.Pantheon.Greek,
                Campers = 137,
                NumCabins = 20,
                FirstAppearance = "Percy Jackson & the Olympians: The Lightning Thief"
            };
            var fixture = new TestFixture(typeof(Waltz), typeof(Pacemaker), typeof(DemigodCamp));

            // Act
            fixture.Transactor.Update(new object[] { waltz, pacemaker, camp });
            var waltzCmd = fixture.PrincipalCommands<Waltz>().UpdateCommand(ANY_ROWS);
            var waltzUpdates = fixture.UpdatesFor(waltzCmd);
            var pacemakerCmd = fixture.PrincipalCommands<Pacemaker>().UpdateCommand(ANY_ROWS);
            var pacemakerUpdates = fixture.UpdatesFor(pacemakerCmd);
            var campCmd = fixture.PrincipalCommands<DemigodCamp>().UpdateCommand(ANY_ROWS);
            var campUpdates = fixture.UpdatesFor(campCmd);

            // Assert
            waltzCmd.Connection.Should().Be(fixture.Connection);
            waltzCmd.Transaction.Should().Be(fixture.Transaction);
            pacemakerCmd.Connection.Should().Be(fixture.Connection);
            pacemakerCmd.Transaction.Should().Be(fixture.Transaction);
            campCmd.Connection.Should().Be(fixture.Connection);
            campCmd.Transaction.Should().Be(fixture.Transaction);
            waltzUpdates.Should().HaveCount(1);
            waltzUpdates.Should().ContainRow(waltz.PieceTitle, waltz.Composer, waltz.Number, waltz.Length, waltz.Premiere, waltz.DancedToOnDWTS);
            pacemakerUpdates.Should().HaveCount(1);
            pacemakerUpdates.Should().ContainRow(pacemaker.ProductID, pacemaker.Recipient, pacemaker.InstalledOn, pacemaker.InstalledBy, ConversionOf(pacemaker.Pacing));
            campUpdates.Should().HaveCount(1);
            campUpdates.Should().ContainRow(camp.Name, camp.Location, ConversionOf(camp.Mythology), camp.Campers, camp.NumCabins, camp.FirstAppearance);
            fixture.ShouldBeOrdered((waltzCmd, pacemakerCmd, campCmd));
            fixture.Transaction.Received(1).Commit();
        }

        [TestMethod] public void MultipleEntitiesRelatedByReferenceChain() {
            // Arrange
            var company = new PromoCode.Company() {
                Name = "Gary's Pizza Emporium of the West",
                Discriminator = "OREGON",
                IsPubliclyTraded = false,
                AnnualRevenue = 3000000M
            };
            var discount = new PromoCode.Discout() {
                ID = Guid.NewGuid(),
                Percentage = 0.15f,
                IsBOGO = false,
                ValidAt = company,
                Expiration = null
            };
            var promoCode = new PromoCode() {
                Code = "XY81UIQP7",
                Discount = discount,
                IsActive = true,
                OnlineOnly = false
            };
            var fixture = new TestFixture(typeof(PromoCode), typeof(PromoCode.Discout), typeof(PromoCode.Company));

            // Act
            fixture.Transactor.Update(new object[] { company, discount, promoCode });
            var promoCodeCmd = fixture.PrincipalCommands<PromoCode>().UpdateCommand(ANY_ROWS);
            var promoCodeUpdates = fixture.UpdatesFor(promoCodeCmd);
            var companyCmd = fixture.PrincipalCommands<PromoCode.Company>().UpdateCommand(ANY_ROWS);
            var companyUpdates = fixture.UpdatesFor(companyCmd);
            var discountCmd = fixture.PrincipalCommands<PromoCode.Discout>().UpdateCommand(ANY_ROWS);
            var discountUpdates = fixture.UpdatesFor(discountCmd);

            // Assert
            promoCodeCmd.Connection.Should().Be(fixture.Connection);
            promoCodeCmd.Transaction.Should().Be(fixture.Transaction);
            companyCmd.Connection.Should().Be(fixture.Connection);
            companyCmd.Transaction.Should().Be(fixture.Transaction);
            discountCmd.Connection.Should().Be(fixture.Connection);
            discountCmd.Transaction.Should().Be(fixture.Transaction);
            promoCodeUpdates.Should().HaveCount(1);
            promoCodeUpdates.Should().ContainRow(promoCode.Code, promoCode.Discount.ID, promoCode.IsActive, promoCode.OnlineOnly);
            companyUpdates.Should().HaveCount(1);
            companyUpdates.Should().ContainRow(company.Name, company.Discriminator, company.IsPubliclyTraded, company.AnnualRevenue);
            discountUpdates.Should().HaveCount(1);
            discountUpdates.Should().ContainRow(discount.ID, discount.Percentage, discount.IsBOGO, discount.ValidAt.Name, discount.ValidAt.Discriminator, discount.Expiration);
            fixture.ShouldBeOrdered(companyCmd, discountCmd, promoCodeCmd);
            fixture.Transaction.Received(1).Commit();
        }

        [TestMethod] public void MultipleEntitiesRelatedByReferenceTree() {
            // Arrange
            var president = new WritOfCertiorari.President() {
                Number = 37,
                Name = "Richard Nixon",
                PoliticalParty = WritOfCertiorari.President.Party.Republican
            };
            var courtCase = new WritOfCertiorari.Case() {
                DocketNumber = 1924233,
                Variety = WritOfCertiorari.Case.Type.Criminal,
                Plaintiff = "DeFryi, LLC.",
                Defendant = "State of Arkansas"
            };
            var justice = new WritOfCertiorari.Justice() {
                Name = "Carla Samenslau",
                YearAssumedBench = 1973,
                AppointedBy = president,
                IsChiefJustice = false
            };
            var writ = new WritOfCertiorari() {
                WritID = Guid.NewGuid(),
                CourtCase = courtCase,
                IssuingJudge = justice,
                JusticesInFavor = 6,
                OriginatingCircuit = 2
            };
            var fixture = new TestFixture(typeof(WritOfCertiorari), typeof(WritOfCertiorari.Justice), typeof(WritOfCertiorari.Case), typeof(WritOfCertiorari.President));

            // Act
            fixture.Transactor.Update(new object[] { writ, justice, courtCase, president });
            var writeCmd = fixture.PrincipalCommands<WritOfCertiorari>().UpdateCommand(ANY_ROWS);
            var writUpdates = fixture.UpdatesFor(writeCmd);
            var justiceCmd = fixture.PrincipalCommands<WritOfCertiorari.Justice>().UpdateCommand(ANY_ROWS);
            var justiceUpdates = fixture.UpdatesFor(justiceCmd);
            var caseCmd = fixture.PrincipalCommands<WritOfCertiorari.Case>().UpdateCommand(ANY_ROWS);
            var caseUpdates = fixture.UpdatesFor(caseCmd);
            var presidentCmd = fixture.PrincipalCommands<WritOfCertiorari.President>().UpdateCommand(ANY_ROWS);
            var presidentUpdates = fixture.UpdatesFor(presidentCmd);

            // Assert
            writeCmd.Connection.Should().Be(fixture.Connection);
            writeCmd.Transaction.Should().Be(fixture.Transaction);
            justiceCmd.Connection.Should().Be(fixture.Connection);
            justiceCmd.Transaction.Should().Be(fixture.Transaction);
            caseCmd.Connection.Should().Be(fixture.Connection);
            caseCmd.Transaction.Should().Be(fixture.Transaction);
            presidentCmd.Connection.Should().Be(fixture.Connection);
            presidentCmd.Transaction.Should().Be(fixture.Transaction);
            writUpdates.Should().HaveCount(1);
            writUpdates.Should().ContainRow(writ.WritID, writ.CourtCase.DocketNumber, writ.IssuingJudge.Name, writ.JusticesInFavor, writ.OriginatingCircuit);
            justiceUpdates.Should().HaveCount(1);
            justiceUpdates.Should().ContainRow(justice.Name, justice.YearAssumedBench, justice.AppointedBy.Number, justice.IsChiefJustice);
            caseUpdates.Should().HaveCount(1);
            caseUpdates.Should().ContainRow(courtCase.DocketNumber, ConversionOf(courtCase.Variety), courtCase.Plaintiff, courtCase.Defendant);
            presidentUpdates.Should().HaveCount(1);
            presidentUpdates.Should().ContainRow(president.Number, president.Name, ConversionOf(president.PoliticalParty));
            fixture.ShouldBeOrdered((caseCmd, justiceCmd), writeCmd);
            fixture.ShouldBeOrdered(presidentCmd, justiceCmd);
            fixture.Transaction.Received(1).Commit();
        }

        [TestMethod] public void MultipleEntitiesRelatedByRelation() {
            // Arrange
            var katniss = new HungerGames.PanemCitizen() {
                Name = "Katniss Everdeen",
                District = 12,
                IsFemale = true
            };
            var rue = new HungerGames.PanemCitizen() {
                Name = "Rue",
                District = 11,
                IsFemale = true
            };
            var marvel = new HungerGames.PanemCitizen() {
                Name = "Marvel",
                District = 1,
                IsFemale = false
            };
            var games = new HungerGames() {
                Incarnation = 74,
                Gamemaker = "Seneca Crane",
                IsQuarterQuell = false,
                President = "Coriolanus Snow",
                Killers = new RelationMap<HungerGames.PanemCitizen, HungerGames.PanemCitizen?>() {
                    { katniss, null },
                    { rue, marvel },
                    { marvel, katniss }
                }
            };
            var fixture = new TestFixture(typeof(HungerGames), typeof(HungerGames.PanemCitizen));

            // Act
            fixture.Transactor.Update(new object[] { games, katniss, rue, marvel });
            var hungerGamesCmd = fixture.PrincipalCommands<HungerGames>().UpdateCommand(ANY_ROWS);
            var hungerGamesUpdates = fixture.UpdatesFor(hungerGamesCmd);
            var citizenCmd = fixture.PrincipalCommands<HungerGames.PanemCitizen>().UpdateCommand(ANY_ROWS);
            var citizenUpdates = fixture.UpdatesFor(citizenCmd);
            var killersInsertCmd = fixture.RelationCommands<HungerGames>(0).InsertCommand(ANY_ROWS);
            var killersInsertions = fixture.InsertionsFor(killersInsertCmd);
            var killersDeleteCmd = fixture.RelationCommands<HungerGames>(0).DeleteCommand(ANY_ROWS);
            var killersDeletions = fixture.DeletionsFor(killersDeleteCmd);
            var killersUpdateCmd = fixture.RelationCommands<HungerGames>(0).UpdateCommand(ANY_ROWS);
            var killersUpdates = fixture.UpdatesFor(killersUpdateCmd);

            // Assert
            hungerGamesCmd.Connection.Should().Be(fixture.Connection);
            hungerGamesCmd.Transaction.Should().Be(fixture.Transaction);
            citizenCmd.Connection.Should().Be(fixture.Connection);
            citizenCmd.Transaction.Should().Be(fixture.Transaction);
            killersInsertCmd.Connection.Should().Be(fixture.Connection);
            killersInsertCmd.Transaction.Should().Be(fixture.Transaction);
            hungerGamesUpdates.Should().HaveCount(1);
            hungerGamesUpdates.Should().ContainRow(games.Incarnation, games.Gamemaker, games.IsQuarterQuell, games.President);
            citizenUpdates.Should().HaveCount(3);
            citizenUpdates.Should().ContainRow(katniss.Name, katniss.District, katniss.IsFemale);
            citizenUpdates.Should().ContainRow(rue.Name, rue.District, rue.IsFemale);
            citizenUpdates.Should().ContainRow(marvel.Name, marvel.District, marvel.IsFemale);
            killersInsertions.Should().HaveCount(3);
            killersInsertions.Should().ContainRow(games.Incarnation, katniss.Name, games.Killers[katniss]?.Name);
            killersInsertions.Should().ContainRow(games.Incarnation, rue.Name, games.Killers[rue]?.Name);
            killersInsertions.Should().ContainRow(games.Incarnation, marvel.Name, games.Killers[marvel]?.Name);
            killersDeletions.Should().HaveCount(0);
            killersUpdates.Should().HaveCount(0);
            fixture.ShouldBeOrdered((citizenCmd, hungerGamesCmd), killersInsertCmd);
            fixture.Transaction.Received(1).Commit();
            games.Killers.Should().HaveUnsavedEntryCount(0);
        }

        [TestMethod] public void SelfReferentialRelation() {
            // Arrange
            var yudhishthira = new Pandava() {
                Name = "Yudhishthira",
                Father = "Dharma Raja",
                MahabharataMentions = 204,
                PrimaryWeapon = "spear",
                Brothers = new RelationList<Pandava>()
            };
            var arjuna = new Pandava() {
                Name = "Arjuna",
                Father = "Indra",
                MahabharataMentions = 320,
                PrimaryWeapon = "bow and arrow",
                Brothers = new RelationList<Pandava>() { yudhishthira }
            };
            var bhima = new Pandava() {
                Name = "Bhima",
                Father = "Vayu",
                MahabharataMentions = 141,
                PrimaryWeapon = "mace",
                Brothers = new RelationList<Pandava>() { yudhishthira, arjuna }
            };
            var nakula = new Pandava() {
                Name = "Nakula",
                Father = "Nasatya",
                MahabharataMentions = 13,
                PrimaryWeapon = "sword",
                Brothers = new RelationList<Pandava>() { yudhishthira, arjuna, bhima }
            };
            var sahadeva = new Pandava() {
                Name = "Sahadeva",
                Father = "Kumara",
                MahabharataMentions = 16,
                PrimaryWeapon = "sword",
                Brothers = new RelationList<Pandava>() { yudhishthira, arjuna, bhima, nakula }
            };
            (yudhishthira.Brothers as IRelation).Canonicalize();
            (arjuna.Brothers as IRelation).Canonicalize();
            (bhima.Brothers as IRelation).Canonicalize();
            (nakula.Brothers as IRelation).Canonicalize();
            (sahadeva.Brothers as IRelation).Canonicalize();
            yudhishthira.Brothers.AddRange(new Pandava[] { arjuna, bhima, nakula, sahadeva });
            arjuna.Brothers.AddRange(new Pandava[] {bhima, nakula, sahadeva });
            bhima.Brothers.AddRange(new Pandava[] { nakula, sahadeva });
            nakula.Brothers.AddRange(new Pandava[] { sahadeva });
            var fixture = new TestFixture(typeof(Pandava));

            // Act
            fixture.Transactor.Update(new object[] { yudhishthira, arjuna, bhima, nakula, sahadeva });
            var pandavaCmd = fixture.PrincipalCommands<Pandava>().UpdateCommand(ANY_ROWS);
            var pandavaUpdates = fixture.UpdatesFor(pandavaCmd);
            var brothersInsertCmd = fixture.RelationCommands<Pandava>(0).InsertCommand(ANY_ROWS);
            var brothersInsertions = fixture.InsertionsFor(brothersInsertCmd);
            var brothersDeleteCmd = fixture.RelationCommands<Pandava>(0).DeleteCommand(ANY_ROWS);
            var brothersDeletions = fixture.DeletionsFor(brothersDeleteCmd);
            var brothersUpdateCmd = fixture.RelationCommands<Pandava>(0).UpdateCommand(ANY_ROWS);
            var brothersUpdates = fixture.UpdatesFor(brothersUpdateCmd);

            // Assert
            pandavaCmd.Connection.Should().Be(fixture.Connection);
            pandavaCmd.Transaction.Should().Be(fixture.Transaction);
            brothersInsertCmd.Connection.Should().Be(fixture.Connection);
            brothersInsertCmd.Transaction.Should().Be(fixture.Transaction);
            pandavaUpdates.Should().HaveCount(5);
            pandavaUpdates.Should().ContainRow(yudhishthira.Name, yudhishthira.Father, yudhishthira.MahabharataMentions, yudhishthira.PrimaryWeapon);
            pandavaUpdates.Should().ContainRow(arjuna.Name, arjuna.Father, arjuna.MahabharataMentions, arjuna.PrimaryWeapon);
            pandavaUpdates.Should().ContainRow(bhima.Name, bhima.Father, bhima.MahabharataMentions, bhima.PrimaryWeapon);
            pandavaUpdates.Should().ContainRow(nakula.Name, nakula.Father, nakula.MahabharataMentions, nakula.PrimaryWeapon);
            pandavaUpdates.Should().ContainRow(sahadeva.Name, sahadeva.Father, sahadeva.MahabharataMentions, sahadeva.PrimaryWeapon);
            brothersInsertions.Should().HaveCount(10);
            brothersInsertions.Should().ContainRow(yudhishthira.Name, arjuna.Name);
            brothersInsertions.Should().ContainRow(yudhishthira.Name, bhima.Name);
            brothersInsertions.Should().ContainRow(yudhishthira.Name, nakula.Name);
            brothersInsertions.Should().ContainRow(yudhishthira.Name, sahadeva.Name);
            brothersInsertions.Should().ContainRow(arjuna.Name, bhima.Name);
            brothersInsertions.Should().ContainRow(arjuna.Name, nakula.Name);
            brothersInsertions.Should().ContainRow(arjuna.Name, sahadeva.Name);
            brothersInsertions.Should().ContainRow(bhima.Name, nakula.Name);
            brothersInsertions.Should().ContainRow(bhima.Name, sahadeva.Name);
            brothersInsertions.Should().ContainRow(nakula.Name, sahadeva.Name);
            brothersDeletions.Should().HaveCount(0);
            brothersUpdates.Should().HaveCount(0);
            fixture.ShouldBeOrdered(pandavaCmd, brothersInsertCmd);
            fixture.Transaction.Received(1).Commit();
            yudhishthira.Brothers.Should().HaveUnsavedEntryCount(0);
            arjuna.Brothers.Should().HaveUnsavedEntryCount(0);
            bhima.Brothers.Should().HaveUnsavedEntryCount(0);
            nakula.Brothers.Should().HaveUnsavedEntryCount(0);
        }

        [TestMethod] public void TransactionRolledBack() {
            // Arrange
            var chatbot = new ChatBot() {
                BotID = Guid.NewGuid(),
                Name = "Der ChatBot",
                CanPassTuringTest = false,
                URL = null,
                UsesPrompts = true,
                IsGenerativeAI = false
            };
            var fixture = new TestFixture(typeof(ChatBot)).WithCommitError();

            // Act
            var action = () => fixture.Transactor.Insert(new object[] { chatbot });

            // Assert
            action.Should().ThrowExactly<InvalidOperationException>();
            fixture.Transaction.Received(1).Commit();
            fixture.Transaction.Received(1).Rollback();
        }

        [TestMethod] public void RollbackFails() {
            // Arrange
            var surgeonGeneral = new SurgeonGeneral() {
                Name = "Vivek Murthy",
                Iteration = 21,
                AppointedBy = "Joe Biden",
                TermBegin = new DateTime(2021, 3, 25),
                TermEnd = new DateTime(2025, 1, 20),
                MedicalSchool = "Yale University"
            };
            var fixture = new TestFixture(typeof(SurgeonGeneral)).WithRollbackError();

            // Act
            var action = () => fixture.Transactor.Update(new object[] { surgeonGeneral });

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
