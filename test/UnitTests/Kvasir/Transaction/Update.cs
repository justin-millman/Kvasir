using FluentAssertions;
using Kvasir.Core;
using Kvasir.Relations;
using Kvasir.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using static UT.Kvasir.Transaction.Update;
using static UT.Kvasir.Translation.TestLocalizations;

namespace UT.Kvasir.Transaction {
    [TestClass, TestCategory("Modification")]
    public class UpdateTests {
        [TestMethod] public async Task SingleInstanceSingleEntityNoRelations() {
            // Arrange
            var diffEq = new DifferentialEquation() {
                EquationId = Guid.NewGuid(),
                Equation = "dy/dx = y/x + 7",
                IsPartial = false,
                NumSolutions = null
            };
            var fixture = new TestFixture(typeof(DifferentialEquation));

            // Act
            await fixture.InitializeSchema();
            await fixture.Transactor.Update([diffEq]);
            var diffEqCmd = fixture.PrincipalCommands<DifferentialEquation>().UpdateCommand(ANY_ROWS);
            var diffEqUpdates = fixture.UpdatesFor(diffEqCmd);

            // Assert
            diffEqCmd.Connection.Should().Be(fixture.Connection);
            diffEqCmd.Transaction.Should().Be(fixture.Transaction);
            diffEqUpdates.Should().HaveCount(1);
            diffEqUpdates.Should().ContainRow(diffEq.EquationId, diffEq.Equation, diffEq.IsPartial, diffEq.NumSolutions);
            fixture.ShouldBeOrdered(diffEqCmd);
            await fixture.Transaction.Received(1).CommitAsync();
        }

        [TestMethod] public async Task MultipleInstancesSingleEntityNoRelations() {
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
            await fixture.InitializeSchema();
            await fixture.Transactor.Update([cortes, pizarro]);
            var conquistadorCmd = fixture.PrincipalCommands<Conquistador>().UpdateCommand(ANY_ROWS);
            var conquistadorUpdates = fixture.UpdatesFor(conquistadorCmd);

            // Assert
            conquistadorCmd.Connection.Should().Be(fixture.Connection);
            conquistadorCmd.Transaction.Should().Be(fixture.Transaction);
            conquistadorUpdates.Should().HaveCount(2);
            conquistadorUpdates.Should().ContainRow(cortes.Name, ConversionOf(cortes.Conquest), cortes.DateOfBirth, cortes.DateOfDeath);
            conquistadorUpdates.Should().ContainRow(pizarro.Name, ConversionOf(pizarro.Conquest), pizarro.DateOfBirth, pizarro.DateOfDeath);
            fixture.ShouldBeOrdered(conquistadorCmd);
            await fixture.Transaction.Received(1).CommitAsync();
        }

        [TestMethod] public async Task SingleInstanceSingleEntityNonEmptyScalarRelationsSavedElements() {
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
            await fixture.InitializeSchema();
            await fixture.Transactor.Update([quiz]);
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
            await fixture.Transaction.Received(1).CommitAsync();
        }

        [TestMethod] public async Task SingleInstanceSingleEntityNonEmptyScalarNonAssociativeRelationsDeletedElements() {
            // Arrange
            var guacamole = new Guacamole() {
                GuacamoleID = Guid.NewGuid(),
                Preparer = "Chef Miguel C. Espadillo",
                NumServings = 4,
                MadeInMolcajete = true,
                Ingredients = [
                    "Avocado",
                    "Cilantro",
                    "Jalapeño",
                    "Lime Juice",
                    "Salt"
                ]
            };
            (guacamole.Ingredients as IRelation).Canonicalize();
            guacamole.Ingredients.Clear();
            var fixture = new TestFixture(typeof(Guacamole));

            // Act
            await fixture.InitializeSchema();
            await fixture.Transactor.Update([guacamole]);
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
            await fixture.Transaction.Received(1).CommitAsync();
            guacamole.Ingredients.Should().HaveUnsavedEntryCount(0);
        }

        [TestMethod] public async Task SingleInstanceSingleEntityNonEmptyScalarAssociativeRelationsDeletedElements() {
            // Arrange
            var eyeDrops = new EyeDrops() {
                DropsID = Guid.NewGuid(),
                BrandName = "Mr. Sees-All",
                Prescriptions = 1876504,
                TreatmentPlan = new RelationMap<EyeDrops.Condition, bool>() {
                    { EyeDrops.Condition.Glaucoma, false },
                    { EyeDrops.Condition.Conjunctivitis, true },
                    { EyeDrops.Condition.Cataracts, true },
                    { EyeDrops.Condition.NearSightedness, false },
                    { EyeDrops.Condition.FarSightedness, false },
                    { EyeDrops.Condition.Astigmatism, true }
                },
                SafeForChildren = false,
                Dilatory = true,
                MaxDropsPerDay = 7
            };
            (eyeDrops.TreatmentPlan as IRelation).Canonicalize();
            eyeDrops.TreatmentPlan.Clear();
            var fixture = new TestFixture(typeof(EyeDrops));

            // Act
            await fixture.InitializeSchema();
            await fixture.Transactor.Update([eyeDrops]);
            var eyeDropsCmd = fixture.PrincipalCommands<EyeDrops>().UpdateCommand(ANY_ROWS);
            var eyeDropsUpdates = fixture.UpdatesFor(eyeDropsCmd);
            var treatmentsInsertCmd = fixture.RelationCommands<EyeDrops>(0).InsertCommand(ANY_ROWS);
            var treatmentsInsertions = fixture.InsertionsFor(treatmentsInsertCmd);
            var treatmentsDeleteCmd = fixture.RelationCommands<EyeDrops>(0).DeleteCommand(ANY_ROWS);
            var treatmentsDeletions = fixture.DeletionsFor(treatmentsDeleteCmd);
            var treatmentsUpdateCmd = fixture.RelationCommands<EyeDrops>(0).UpdateCommand(ANY_ROWS);
            var treatmentsUpdates = fixture.UpdatesFor(treatmentsUpdateCmd);

            // Assert
            eyeDropsCmd.Connection.Should().Be(fixture.Connection);
            eyeDropsCmd.Transaction.Should().Be(fixture.Transaction);
            treatmentsDeleteCmd.Connection.Should().Be(fixture.Connection);
            treatmentsDeleteCmd.Transaction.Should().Be(fixture.Transaction);
            eyeDropsUpdates.Should().HaveCount(1);
            eyeDropsUpdates.Should().ContainRow(eyeDrops.DropsID, eyeDrops.BrandName, eyeDrops.Prescriptions, eyeDrops.SafeForChildren, eyeDrops.Dilatory, eyeDrops.MaxDropsPerDay);
            treatmentsInsertions.Should().HaveCount(0);
            treatmentsDeletions.Should().HaveCount(6);
            treatmentsDeletions.Should().ContainRow(eyeDrops.DropsID, ConversionOf(EyeDrops.Condition.Astigmatism));
            treatmentsDeletions.Should().ContainRow(eyeDrops.DropsID, ConversionOf(EyeDrops.Condition.NearSightedness));
            treatmentsDeletions.Should().ContainRow(eyeDrops.DropsID, ConversionOf(EyeDrops.Condition.FarSightedness));
            treatmentsDeletions.Should().ContainRow(eyeDrops.DropsID, ConversionOf(EyeDrops.Condition.Cataracts));
            treatmentsDeletions.Should().ContainRow(eyeDrops.DropsID, ConversionOf(EyeDrops.Condition.Conjunctivitis));
            treatmentsDeletions.Should().ContainRow(eyeDrops.DropsID, ConversionOf(EyeDrops.Condition.Glaucoma));
            treatmentsUpdates.Should().HaveCount(0);
            fixture.ShouldBeOrdered(eyeDropsCmd, treatmentsDeleteCmd);
            await fixture.Transaction.Received(1).CommitAsync();
            eyeDrops.TreatmentPlan.Should().HaveUnsavedEntryCount(0);
        }

        [TestMethod] public async Task SingleInstanceSingleEntityNonEmptyScalarRelationsNewElements() {
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
            await fixture.InitializeSchema();
            await fixture.Transactor.Update([statue]);
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
            await fixture.Transaction.Received(1).CommitAsync();
            statue.Dimensions.Should().HaveUnsavedEntryCount(0);
        }

        [TestMethod] public async Task SingleInstanceSingleEntityNonEmptyScalarRelationsModifiedElements() {
            // Arrange
            var bibliography = new Bibliography() {
                ID = Guid.NewGuid(),
                For = "Anticoagulant Properties of Cnidarian Saliva on Gluten-Free Females Aged 17-23",
                Style = Bibliography.Standard.Harvard,
                NumPages = 218,
                References = [
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
                ]
            };
            (bibliography.References as IRelation).Canonicalize();
            bibliography.References[1] = bibliography.References[1] with { TypeOfWork = Bibliography.Literature.Interview };
            var fixture = new TestFixture(typeof(Bibliography));

            // Act
            await fixture.InitializeSchema();
            await fixture.Transactor.Update([bibliography]);
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
            await fixture.Transaction.Received(1).CommitAsync();
            bibliography.References.Should().HaveUnsavedEntryCount(0);
        }

        [TestMethod] public async Task SingleInstanceSingleEntityNonEmptyScalarRelationsMixedElements() {
            // Arrange
            var akuna = new MarketMaker() {
                PrimaryMPID = "4619",
                FirmName = "Akuna Capital",
                NetCapital = 618000000M,
                IsDesignated = false,
                Symbols = [
                    "NVDA",
                    "IBIT",
                    "APL",
                    "TSLA",
                    "MSFT"
                ]
            };
            (akuna.Symbols as IRelation).Canonicalize();
            akuna.Symbols[2] = "AAPL";
            akuna.Symbols.Add("QQQ");
            var fixture = new TestFixture(typeof(MarketMaker));

            // Act
            await fixture.InitializeSchema();
            await fixture.Transactor.Update([akuna]);
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
            await fixture.Transaction.Received(1).CommitAsync();
            akuna.Symbols.Should().HaveUnsavedEntryCount(0);
        }

        [TestMethod] public async Task SingleInstanceSingleEntityEmptyScalarRelations() {
            // Arrange
            var scroll = new DeadSeaScroll() {
                Cave = "Qumran Cave 3",
                Identifier = "3QEzek",
                ScrollLanguage = DeadSeaScroll.Language.Hebrew,
                BiblicalSource = "Ezekiel 16:31-33",
                DiscoveryYear = 1952,
                VerifiedAuthors = []
            };
            var fixture = new TestFixture(typeof(DeadSeaScroll));

            // Act
            await fixture.InitializeSchema();
            await fixture.Transactor.Update([scroll]);
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
            await fixture.Transaction.Received(1).CommitAsync();
        }

        [TestMethod] public async Task MultipleInstancesSingleEntityScalarRelations() {
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
            await fixture.InitializeSchema();
            await fixture.Transactor.Update([chicago, derby, perth]);
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
            await fixture.Transaction.Received(1).CommitAsync();
            chicago.Bishops.Should().HaveUnsavedEntryCount(0);
            derby.Bishops.Should().HaveUnsavedEntryCount(0);
            perth.Bishops.Should().HaveUnsavedEntryCount(0);
        }

        [TestMethod] public async Task SingleInstanceSingleLocalizationSavedValues() {
            // Arrange
            var placement = new Placement("LOC_HORSE_RACING");
            placement[1] = "win";
            placement[2] = "place";
            placement[3] = "show";
            (placement.Relation as IRelation).Canonicalize();
            var fixture = new TestFixture(typeof(Placement));

            // Act
            await fixture.InitializeSchema();
            await fixture.Transactor.Update([placement]);
            var placementInsertCmd = fixture.PrincipalCommands<Placement>().InsertCommand(ANY_ROWS);
            var placementInsertions = fixture.InsertionsFor(placementInsertCmd);
            var placementDeleteCmd = fixture.PrincipalCommands<Placement>().DeleteCommand(ANY_ROWS);
            var placementDeletions = fixture.DeletionsFor(placementDeleteCmd);
            var placementUpdateCmd = fixture.PrincipalCommands<Placement>().UpdateCommand(ANY_ROWS);
            var placementUpdates = fixture.UpdatesFor(placementUpdateCmd);

            // Assert
            placementInsertCmd.Received(0).ExecuteNonQuery();
            placementDeleteCmd.Received(0).ExecuteNonQuery();
            placementUpdateCmd.Received(0).ExecuteNonQuery();
            placementInsertions.Should().HaveCount(0);
            placementDeletions.Should().HaveCount(0);
            placementUpdates.Should().HaveCount(0);
            placement.Relation.Should().HaveUnsavedEntryCount(0);
        }

        [TestMethod] public async Task SingleInstanceSingleLocalizationDeletedValues() {
            // Arrange
            var permissions = new Permissions("LOC_ESTHER_GROENBERGEN");
            permissions["production"] = Operation.CanRead;
            permissions["development"] = Operation.CanRead | Operation.CanWrite | Operation.CanModify | Operation.CanDelete | Operation.CanCreate;
            permissions["qa"] = Operation.CanRead | Operation.CanCreate;
            permissions["cloud-test"] = Operation.CanAdmin;
            (permissions.Relation as IRelation).Canonicalize();
            permissions.Delocalize("cloud-test");
            permissions.Delocalize("qa");
            var fixture = new TestFixture(typeof(Permissions));

            // Act
            await fixture.InitializeSchema();
            await fixture.Transactor.Update([permissions]);
            var permissionsInsertCmd = fixture.PrincipalCommands<Permissions>().InsertCommand(ANY_ROWS);
            var permissionsInsertions = fixture.InsertionsFor(permissionsInsertCmd);
            var permissionsDeleteCmd = fixture.PrincipalCommands<Permissions>().DeleteCommand(ANY_ROWS);
            var permissionsDeletions = fixture.DeletionsFor(permissionsDeleteCmd);
            var permissionsUpdateCmd = fixture.PrincipalCommands<Permissions>().UpdateCommand(ANY_ROWS);
            var permissionsUpdates = fixture.UpdatesFor(permissionsUpdateCmd);

            // Assert
            permissionsInsertCmd.Received(0).ExecuteNonQuery();
            permissionsDeleteCmd.Connection.Should().Be(fixture.Connection);
            permissionsDeleteCmd.Transaction.Should().Be(fixture.Transaction);
            permissionsUpdateCmd.Received(0).ExecuteNonQuery();
            permissionsInsertions.Should().HaveCount(0);
            permissionsDeletions.Should().HaveCount(2);
            permissionsDeletions.Should().ContainRow(permissions.Key, "qa");
            permissionsDeletions.Should().ContainRow(permissions.Key, "cloud-test");
            permissionsUpdates.Should().HaveCount(0);
            fixture.ShouldBeOrdered(permissionsDeleteCmd);
            await fixture.Transaction.Received(1).CommitAsync();
            permissions.Relation.Should().HaveUnsavedEntryCount(0);
        }

        [TestMethod] public async Task SingleInstanceSingleLocalizationNewValues() {
            // Arrange
            var binary = new BinaryValue("LOC_ON/OFF");
            binary[true] = "on";
            binary[false] = "off";
            var fixture = new TestFixture(typeof(BinaryValue));

            // Act
            await fixture.InitializeSchema();
            await fixture.Transactor.Update([binary]);
            var binaryInsertCmd = fixture.PrincipalCommands<BinaryValue>().InsertCommand(ANY_ROWS);
            var binaryInsertions = fixture.InsertionsFor(binaryInsertCmd);
            var binaryDeleteCmd = fixture.PrincipalCommands<BinaryValue>().DeleteCommand(ANY_ROWS);
            var binaryDeletions = fixture.DeletionsFor(binaryDeleteCmd);
            var binaryUpdateCmd = fixture.PrincipalCommands<BinaryValue>().DeleteCommand(ANY_ROWS);
            var binaryUpdates = fixture.UpdatesFor(binaryUpdateCmd);

            // Assert
            binaryInsertCmd.Connection.Should().Be(fixture.Connection);
            binaryInsertCmd.Transaction.Should().Be(fixture.Transaction);
            binaryDeleteCmd.Received(0).ExecuteNonQuery();
            binaryUpdateCmd.Received(0).ExecuteNonQuery();
            binaryInsertions.Should().HaveCount(2);
            binaryInsertions.Should().ContainRow(binary.Key, true, binary[true]);
            binaryInsertions.Should().ContainRow(binary.Key, false, binary[false]);
            binaryDeletions.Should().HaveCount(0);
            binaryUpdates.Should().HaveCount(0);
            fixture.ShouldBeOrdered(binaryInsertCmd);
            await fixture.Transaction.Received(1).CommitAsync();
            binary.Relation.Should().HaveUnsavedEntryCount(0);
        }

        [TestMethod] public async Task SingleInstanceSingleLocalizationMixedValues() {
            // Arrange
            var bigBad = new BigBad("LOC_BUFFY");
            bigBad[0] = "The Master";
            bigBad[1] = "Spike & Drusilla";
            bigBad[2] = "Mayor Richard Wilkins III";
            bigBad[3] = "Adam (and the Initiative)";
            bigBad[4] = "Glorificus (a.k.a. Glory)";
            (bigBad.Relation as IRelation).Canonicalize();
            bigBad[1] = "Angelus";
            bigBad[5] = "Dark Willow";
            bigBad[6] = "The First Evil";
            var fixture = new TestFixture(typeof(BigBad));

            // Act
            await fixture.InitializeSchema();
            await fixture.Transactor.Update([bigBad]);
            var bigBadInsertCmd = fixture.PrincipalCommands<BigBad>().InsertCommand(ANY_ROWS);
            var bigBadInsertions = fixture.InsertionsFor(bigBadInsertCmd);
            var bigBadDeleteCmd = fixture.PrincipalCommands<BigBad>().DeleteCommand(ANY_ROWS);
            var bigBadDeletions = fixture.DeletionsFor(bigBadDeleteCmd);
            var bigBadUpdateCmd = fixture.PrincipalCommands<BigBad>().UpdateCommand(ANY_ROWS);
            var bigBadUpdates = fixture.UpdatesFor(bigBadUpdateCmd);

            // Assert
            bigBadInsertCmd.Connection.Should().Be(fixture.Connection);
            bigBadInsertCmd.Transaction.Should().Be(fixture.Transaction);
            bigBadDeleteCmd.Connection.Should().Be(fixture.Connection);
            bigBadDeleteCmd.Transaction.Should().Be(fixture.Transaction);
            bigBadInsertions.Should().HaveCount(3);
            bigBadInsertions.Should().ContainRow(bigBad.Key, (sbyte)1, bigBad[1]);
            bigBadInsertions.Should().ContainRow(bigBad.Key, (sbyte)5, bigBad[5]);
            bigBadInsertions.Should().ContainRow(bigBad.Key, (sbyte)6, bigBad[6]);
            bigBadDeletions.Should().HaveCount(1);
            bigBadDeletions.Should().ContainRow(bigBad.Key, (sbyte)1);
            bigBadUpdates.Should().HaveCount(0);
            fixture.ShouldBeOrdered(bigBadDeleteCmd, bigBadInsertCmd);
            await fixture.Transaction.Received(1).CommitAsync();
            bigBad.Relation.Should().HaveUnsavedEntryCount(0);
        }

        [TestMethod] public async Task MultipleInstancesSingleLocalization() {
            // Arrange
            var illinois = new MinimumWage("LOC_ILLINOIS");
            illinois[1972] = 1.40M;
            illinois[1976] = 2.10M;
            illinois[1979] = 2.30M;
            illinois[2025] = 15.0M;
            illinois[1998] = 5.15M;
            var federal = new MinimumWage("LOC_FEDERAL");
            federal[1938] = 0.25M;
            federal[2009] = 7.25M;
            federal[1968] = 1.60M;
            var hawaii = new MinimumWage("LOC_HAWAII");
            hawaii[2018] = 10.10M;
            hawaii[2022] = 12.00M;
            hawaii[2024] = 14.00M;
            hawaii[2026] = 16.00M;
            var fixture = new TestFixture(typeof(MinimumWage));

            // Act
            await fixture.InitializeSchema();
            await fixture.Transactor.Update([illinois, federal, hawaii]);
            var wageInsertCmd = fixture.PrincipalCommands<MinimumWage>().InsertCommand(ANY_ROWS);
            var wageInsertions = fixture.InsertionsFor(wageInsertCmd);
            var wageDeleteCmd = fixture.PrincipalCommands<MinimumWage>().DeleteCommand(ANY_ROWS);
            var wageDeletions = fixture.DeletionsFor(wageDeleteCmd);
            var wageUpdateCmd = fixture.PrincipalCommands<MinimumWage>().UpdateCommand(ANY_ROWS);
            var wageUpdates = fixture.UpdatesFor(wageUpdateCmd);

            // Assert
            wageInsertCmd.Connection.Should().Be(fixture.Connection);
            wageInsertCmd.Transaction.Should().Be(fixture.Transaction);
            wageDeleteCmd.Received(0).ExecuteNonQuery();
            wageUpdateCmd.Received(0).ExecuteNonQuery();
            wageInsertions.Should().HaveCount(12);
            wageInsertions.Should().ContainRow(illinois.Key, (short)1972, illinois[1972]);
            wageInsertions.Should().ContainRow(illinois.Key, (short)1976, illinois[1976]);
            wageInsertions.Should().ContainRow(illinois.Key, (short)1979, illinois[1979]);
            wageInsertions.Should().ContainRow(illinois.Key, (short)2025, illinois[2025]);
            wageInsertions.Should().ContainRow(illinois.Key, (short)1998, illinois[1998]);
            wageInsertions.Should().ContainRow(federal.Key, (short)1938, federal[1938]);
            wageInsertions.Should().ContainRow(federal.Key, (short)2009, federal[2009]);
            wageInsertions.Should().ContainRow(federal.Key, (short)1968, federal[1968]);
            wageInsertions.Should().ContainRow(hawaii.Key, (short)2018, hawaii[2018]);
            wageInsertions.Should().ContainRow(hawaii.Key, (short)2022, hawaii[2022]);
            wageInsertions.Should().ContainRow(hawaii.Key, (short)2024, hawaii[2024]);
            wageInsertions.Should().ContainRow(hawaii.Key, (short)2026, hawaii[2026]);
            wageDeletions.Should().HaveCount(0);
            wageUpdates.Should().HaveCount(0);
            fixture.ShouldBeOrdered(wageInsertCmd);
            await fixture.Transaction.Received(1).CommitAsync();
            illinois.Relation.Should().HaveUnsavedEntryCount(0);
            federal.Relation.Should().HaveUnsavedEntryCount(0);
            hawaii.Relation.Should().HaveUnsavedEntryCount(0);
        }

        [TestMethod] public async Task MultipleUnrelatedEntities() {
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
            await fixture.InitializeSchema();
            await fixture.Transactor.Update([waltz, pacemaker, camp]);
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
            await fixture.Transaction.Received(1).CommitAsync();
        }

        [TestMethod] public async Task MultipleUnrelatedLocalizations() {
            // Arrange
            var philosophy = new Philosophy("LOC_EXISTENTIALISM");
            philosophy[Language.English] = "existentialism";
            var alterEgo = new AlterEgo("LOC_AE1");
            alterEgo["general public"] = "Fernando Ollallalla";
            alterEgo["family & friends"] = "Gabriel Zurpp";
            alterEgo["the media"] = "Willie C. Uvu";
            var pain = new Pain(679093324);
            pain["shoulder"] = 1.53;
            pain["groin"] = 3.11;
            pain["head"] = 9.76;
            pain["elbow"] = 5.90;
            var fixture = new TestFixture(typeof(Philosophy), typeof(AlterEgo), typeof(Pain));

            // Act
            await fixture.InitializeSchema();
            await fixture.Transactor.Update([philosophy, alterEgo, pain]);
            var philosophyInsertCmd = fixture.PrincipalCommands<Philosophy>().InsertCommand(ANY_ROWS);
            var philosophyInsertions = fixture.InsertionsFor(philosophyInsertCmd);
            var philosophyDeleteCmd = fixture.PrincipalCommands<Philosophy>().DeleteCommand(ANY_ROWS);
            var philosophyDeletions = fixture.DeletionsFor(philosophyDeleteCmd);
            var philosophyUpdateCmd = fixture.PrincipalCommands<Philosophy>().UpdateCommand(ANY_ROWS);
            var philosophyUpdates = fixture.UpdatesFor(philosophyUpdateCmd);
            var alterEgoInsertCmd = fixture.PrincipalCommands<AlterEgo>().InsertCommand(ANY_ROWS);
            var alterEgoInsertions = fixture.InsertionsFor(alterEgoInsertCmd);
            var alterEgoDeleteCmd = fixture.PrincipalCommands<AlterEgo>().DeleteCommand(ANY_ROWS);
            var alterEgoDeletions = fixture.DeletionsFor(alterEgoDeleteCmd);
            var alterEgoUpdateCmd = fixture.PrincipalCommands<AlterEgo>().UpdateCommand(ANY_ROWS);
            var alterEgoUpdates = fixture.UpdatesFor(alterEgoUpdateCmd);
            var painInsertCmd = fixture.PrincipalCommands<Pain>().InsertCommand(ANY_ROWS);
            var painInsertions = fixture.InsertionsFor(painInsertCmd);
            var painDeleteCmd = fixture.PrincipalCommands<Pain>().DeleteCommand(ANY_ROWS);
            var painDeletions = fixture.DeletionsFor(painDeleteCmd);
            var painUpdateCmd = fixture.PrincipalCommands<Pain>().UpdateCommand(ANY_ROWS);
            var painUpdates = fixture.UpdatesFor(painUpdateCmd);

            // Assert
            philosophyInsertCmd.Connection.Should().Be(fixture.Connection);
            philosophyInsertCmd.Transaction.Should().Be(fixture.Transaction);
            philosophyDeleteCmd.Received(0).ExecuteNonQuery();
            philosophyUpdateCmd.Received(0).ExecuteNonQuery();
            alterEgoInsertCmd.Connection.Should().Be(fixture.Connection);
            alterEgoInsertCmd.Transaction.Should().Be(fixture.Transaction);
            alterEgoDeleteCmd.Received(0).ExecuteNonQuery();
            alterEgoUpdateCmd.Received(0).ExecuteNonQuery();
            painInsertCmd.Connection.Should().Be(fixture.Connection);
            painInsertCmd.Transaction.Should().Be(fixture.Transaction);
            painDeleteCmd.Received(0).ExecuteNonQuery();
            painUpdateCmd.Received(0).ExecuteNonQuery();
            philosophyInsertions.Should().HaveCount(1);
            philosophyInsertions.Should().ContainRow(philosophy.Key, ConversionOf(Language.English), philosophy[Language.English]);
            philosophyDeletions.Should().HaveCount(0);
            philosophyUpdates.Should().HaveCount(0);
            alterEgoInsertions.Should().HaveCount(3);
            alterEgoInsertions.Should().ContainRow(alterEgo.Key, "general public", alterEgo["general public"]);
            alterEgoInsertions.Should().ContainRow(alterEgo.Key, "family & friends", alterEgo["family & friends"]);
            alterEgoInsertions.Should().ContainRow(alterEgo.Key, "the media", alterEgo["the media"]);
            alterEgoDeletions.Should().HaveCount(0);
            alterEgoUpdates.Should().HaveCount(0);
            painInsertions.Should().HaveCount(4);
            painInsertions.Should().ContainRow(pain.Key, "shoulder", pain["shoulder"]);
            painInsertions.Should().ContainRow(pain.Key, "groin", pain["groin"]);
            painInsertions.Should().ContainRow(pain.Key, "head", pain["head"]);
            painInsertions.Should().ContainRow(pain.Key, "elbow", pain["elbow"]);
            painDeletions.Should().HaveCount(0);
            painUpdates.Should().HaveCount(0);
            fixture.ShouldBeOrdered((philosophyInsertCmd, alterEgoInsertCmd, painInsertCmd));
            await fixture.Transaction.Received(1).CommitAsync();
            philosophy.Relation.Should().HaveUnsavedEntryCount(0);
            alterEgo.Relation.Should().HaveUnsavedEntryCount(0);
            pain.Relation.Should().HaveUnsavedEntryCount(0);
        }

        [TestMethod] public async Task MultipleEntitiesRelatedByReferenceChain() {
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
            await fixture.InitializeSchema();
            await fixture.Transactor.Update([company, discount, promoCode]);
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
            await fixture.Transaction.Received(1).CommitAsync();
        }

        [TestMethod] public async Task MultipleEntitiesRelatedByReferenceTree() {
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
            await fixture.InitializeSchema();
            await fixture.Transactor.Update([writ, justice, courtCase, president]);
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
            await fixture.Transaction.Received(1).CommitAsync();
        }

        [TestMethod] public async Task MultipleEntitiesRelatedByRelation() {
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
            await fixture.InitializeSchema();
            await fixture.Transactor.Update([games, katniss, rue, marvel]);
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
            await fixture.Transaction.Received(1).CommitAsync();
            games.Killers.Should().HaveUnsavedEntryCount(0);
        }

        [TestMethod] public async Task MultipleEntitiesRelatedByScalarLocalization() {
            // Arrange
            var daemon = new Daemon() {
                Human = "Lyra (Belacqua) Silvertongue",
                Name = "Pantalaimon",
                Animal = new LocalizedText("LOC_PINE_MARTEN"),
                IsZombi = false,
                CompletedAkterrakeh = false
            };
            (daemon.Animal.Relation as IRelation).Canonicalize();
            daemon.Animal[Language.English] = "Pine Marten";
            var fixture = new TestFixture(typeof(Daemon), typeof(LocalizedText));

            // Act
            await fixture.InitializeSchema();
            await fixture.Transactor.Update([daemon, daemon.Animal]);
            var daemonCmd = fixture.PrincipalCommands<Daemon>().UpdateCommand(ANY_ROWS);
            var daemonUpdates = fixture.UpdatesFor(daemonCmd);
            var textInsertCmd = fixture.PrincipalCommands<LocalizedText>().InsertCommand(ANY_ROWS);
            var textInsertions = fixture.InsertionsFor(textInsertCmd);
            var textDeleteCmd = fixture.PrincipalCommands<LocalizedText>().DeleteCommand(ANY_ROWS);
            var textDeletions = fixture.DeletionsFor(textDeleteCmd);
            var textUpdateCmd = fixture.PrincipalCommands<LocalizedText>().UpdateCommand(ANY_ROWS);
            var textUpdates = fixture.UpdatesFor(textUpdateCmd);

            // Assert
            daemonCmd.Connection.Should().Be(fixture.Connection);
            daemonCmd.Transaction.Should().Be(fixture.Transaction);
            textInsertCmd.Connection.Should().Be(fixture.Connection);
            textInsertCmd.Transaction.Should().Be(fixture.Transaction);
            textDeleteCmd.Received(0).ExecuteNonQuery();
            textUpdateCmd.Received(0).ExecuteNonQuery();
            daemonUpdates.Should().HaveCount(1);
            daemonUpdates.Should().ContainRow(daemon.Human, daemon.Name, daemon.Animal.Key, daemon.IsZombi, daemon.CompletedAkterrakeh);
            textInsertions.Should().HaveCount(1);
            textInsertions.Should().ContainRow(daemon.Animal.Key, ConversionOf(Language.English), daemon.Animal[Language.English]);
            textDeletions.Should().HaveCount(0);
            textUpdates.Should().HaveCount(0);
            fixture.ShouldBeOrdered((daemonCmd, textInsertCmd));
            await fixture.Transaction.Received(1).CommitAsync();
            daemon.Animal.Relation.Should().HaveUnsavedEntryCount(0);
        }

        [TestMethod] public async Task MultipleEntitiesRelatedByReferenceLocalization() {
            // Arrange
            var doctor0 = new Vasectomy.Doctor() {
                MedicalID = Guid.NewGuid(),
                Name = "Dr. Edmond Duilleierres",
                AlmaMater = "Paris School for Medical Professionals of Arrondissement #8",
                Specialty = "holistic medicine"
            };
            var doctor1 = new Vasectomy.Doctor() {
                MedicalID = Guid.NewGuid(),
                Name = "Dr. Salla O'Uiell",
                AlmaMater = "Dublin College of Surgical Sciences",
                Specialty = "surgery"
            };
            var vasectomy = new Vasectomy() {
                SurgeryID = Guid.NewGuid(),
                Patient = "Barry Hluek",
                Date = new DateOnly(1993, 4, 19),
                Doctors = new Vasectomy.LocalizedStage(Guid.NewGuid()),
                Reversible = true
            };
            vasectomy.Doctors[Vasectomy.Stage.Anesthetic] = doctor0;
            vasectomy.Doctors[Vasectomy.Stage.Surgery] = doctor1;
            vasectomy.Doctors[Vasectomy.Stage.Recovery] = doctor0;
            vasectomy.Doctors[Vasectomy.Stage.Monitoring] = doctor0;
            var fixture = new TestFixture(typeof(Vasectomy), typeof(Vasectomy.Doctor), typeof(Vasectomy.LocalizedStage));

            // Act
            await fixture.InitializeSchema();
            await fixture.Transactor.Update([doctor0, doctor1, vasectomy, vasectomy.Doctors]);
            var doctorUpdateCmd = fixture.PrincipalCommands<Vasectomy.Doctor>().UpdateCommand(ANY_ROWS);
            var doctorUpdates = fixture.UpdatesFor(doctorUpdateCmd);
            var vasectomyUpdateCmd = fixture.PrincipalCommands<Vasectomy>().UpdateCommand(ANY_ROWS);
            var vasectomyUpdates = fixture.UpdatesFor(vasectomyUpdateCmd);
            var stageInsertCmd = fixture.PrincipalCommands<Vasectomy.LocalizedStage>().InsertCommand(ANY_ROWS);
            var stageInsertions = fixture.InsertionsFor(stageInsertCmd);
            var stageDeleteCmd = fixture.PrincipalCommands<Vasectomy.LocalizedStage>().DeleteCommand(ANY_ROWS);
            var stageDeletions = fixture.DeletionsFor(stageDeleteCmd);
            var stageUpdateCmd = fixture.PrincipalCommands<Vasectomy.LocalizedStage>().UpdateCommand(ANY_ROWS);
            var stageUpdates = fixture.UpdatesFor(stageUpdateCmd);

            // Assert
            doctorUpdateCmd.Connection.Should().Be(fixture.Connection);
            doctorUpdateCmd.Transaction.Should().Be(fixture.Transaction);
            vasectomyUpdateCmd.Connection.Should().Be(fixture.Connection);
            vasectomyUpdateCmd.Transaction.Should().Be(fixture.Transaction);
            stageInsertCmd.Connection.Should().Be(fixture.Connection);
            stageInsertCmd.Transaction.Should().Be(fixture.Transaction);
            stageDeleteCmd.Received(0).ExecuteNonQuery();
            stageUpdateCmd.Received(0).ExecuteNonQuery();
            doctorUpdates.Should().HaveCount(2);
            doctorUpdates.Should().ContainRow(doctor0.MedicalID, doctor0.Name, doctor0.AlmaMater, doctor0.Specialty);
            doctorUpdates.Should().ContainRow(doctor1.MedicalID, doctor1.Name, doctor1.AlmaMater, doctor1.Specialty);
            vasectomyUpdates.Should().HaveCount(1);
            vasectomyUpdates.Should().ContainRow(vasectomy.SurgeryID, vasectomy.Patient, vasectomy.Date, vasectomy.Doctors.Key, vasectomy.Reversible);
            stageInsertions.Should().HaveCount(4);
            stageInsertions.Should().ContainRow(vasectomy.Doctors.Key, ConversionOf(Vasectomy.Stage.Anesthetic), vasectomy.Doctors[Vasectomy.Stage.Anesthetic].MedicalID);
            stageInsertions.Should().ContainRow(vasectomy.Doctors.Key, ConversionOf(Vasectomy.Stage.Surgery), vasectomy.Doctors[Vasectomy.Stage.Surgery].MedicalID);
            stageInsertions.Should().ContainRow(vasectomy.Doctors.Key, ConversionOf(Vasectomy.Stage.Recovery), vasectomy.Doctors[Vasectomy.Stage.Recovery].MedicalID);
            stageInsertions.Should().ContainRow(vasectomy.Doctors.Key, ConversionOf(Vasectomy.Stage.Monitoring), vasectomy.Doctors[Vasectomy.Stage.Monitoring].MedicalID);
            stageDeletions.Should().HaveCount(0);
            stageUpdates.Should().HaveCount(0);

            fixture.ShouldBeOrdered((vasectomyUpdateCmd, stageInsertCmd));
            fixture.ShouldBeOrdered(doctorUpdateCmd, stageInsertCmd);
            await fixture.Transaction.Received(1).CommitAsync();
            vasectomy.Doctors.Relation.Should().HaveUnsavedEntryCount(0);
        }

        [TestMethod] public async Task MultipleEntitiesRelatedByRelationLocalization() {
            // Arrange
            var petapsco = new LocalizedText("LOC_PETAPSCO");
            petapsco[Language.English] = "Petapsco River";
            petapsco[Language.Spanish] = "Río Petapsco";
            var harbor = new Harbor() {
                Name = "Baltimore Harbor",
                ShippingTons = 50000000,
                DraftDepth = 50,
                AirDraft = 182,
                Rivers = new RelationMap<LocalizedText, Harbor.Flow>() {
                    { petapsco, Harbor.Flow.OutFlow }
                },
                OperatedBy = "Maryland Port Administration"
            };
            var fixture = new TestFixture(typeof(Harbor), typeof(LocalizedText));

            // Act
            await fixture.InitializeSchema();
            await fixture.Transactor.Update([petapsco, harbor]);
            var harborCmd = fixture.PrincipalCommands<Harbor>().UpdateCommand(ANY_ROWS);
            var harborUpdates = fixture.UpdatesFor(harborCmd);
            var textInsertCmd = fixture.PrincipalCommands<LocalizedText>().InsertCommand(ANY_ROWS);
            var textInsertions = fixture.InsertionsFor(textInsertCmd);
            var textDeleteCmd = fixture.PrincipalCommands<LocalizedText>().DeleteCommand(ANY_ROWS);
            var textDeletions = fixture.InsertionsFor(textDeleteCmd);
            var textUpdateCmd = fixture.PrincipalCommands<LocalizedText>().UpdateCommand(ANY_ROWS);
            var textUpdates = fixture.UpdatesFor(textUpdateCmd);
            var riversInsertCmd = fixture.RelationCommands<Harbor>(0).InsertCommand(ANY_ROWS);
            var riversInsertions = fixture.InsertionsFor(riversInsertCmd);
            var riversDeleteCmd = fixture.RelationCommands<Harbor>(0).DeleteCommand(ANY_ROWS);
            var riversDeletions = fixture.DeletionsFor(riversDeleteCmd);
            var riversUpdateCmd = fixture.RelationCommands<Harbor>(0).UpdateCommand(ANY_ROWS);
            var riversUpdates = fixture.UpdatesFor(riversUpdateCmd);

            // Assert
            harborCmd.Connection.Should().Be(fixture.Connection);
            harborCmd.Transaction.Should().Be(fixture.Transaction);
            textInsertCmd.Connection.Should().Be(fixture.Connection);
            textInsertCmd.Transaction.Should().Be(fixture.Transaction);
            textDeleteCmd.Received(0).ExecuteNonQuery();
            textUpdateCmd.Received(0).ExecuteNonQuery();
            riversInsertCmd.Connection.Should().Be(fixture.Connection);
            riversInsertCmd.Transaction.Should().Be(fixture.Transaction);
            riversDeleteCmd.Received(0).ExecuteNonQuery();
            riversUpdateCmd.Received(0).ExecuteNonQuery();
            harborUpdates.Should().HaveCount(1);
            harborUpdates.Should().ContainRow(harbor.Name, harbor.ShippingTons, harbor.DraftDepth, harbor.AirDraft, harbor.OperatedBy);
            textInsertions.Should().HaveCount(2);
            textInsertions.Should().ContainRow(petapsco.Key, ConversionOf(Language.English), petapsco[Language.English]);
            textInsertions.Should().ContainRow(petapsco.Key, ConversionOf(Language.Spanish), petapsco[Language.Spanish]);
            textDeletions.Should().HaveCount(0);
            textUpdates.Should().HaveCount(0);
            riversInsertions.Should().HaveCount(1);
            riversInsertions.Should().ContainRow(harbor.Name, petapsco.Key, ConversionOf(harbor.Rivers[petapsco]));
            riversDeletions.Should().HaveCount(0);
            riversUpdates.Should().HaveCount(0);
            fixture.ShouldBeOrdered((harborCmd, riversInsertCmd), riversInsertCmd);
            await fixture.Transaction.Received(1).CommitAsync();
            petapsco.Relation.Should().HaveUnsavedEntryCount(0);
            harbor.Rivers.Should().HaveUnsavedEntryCount(0);
        }

        [TestMethod] public async Task SelfReferentialEntityViaRelation() {
            // Arrange
            var yudhishthira = new Pandava() {
                Name = "Yudhishthira",
                Father = "Dharma Raja",
                MahabharataMentions = 204,
                PrimaryWeapon = "spear",
                Brothers = []
            };
            var arjuna = new Pandava() {
                Name = "Arjuna",
                Father = "Indra",
                MahabharataMentions = 320,
                PrimaryWeapon = "bow and arrow",
                Brothers = [yudhishthira]
            };
            var bhima = new Pandava() {
                Name = "Bhima",
                Father = "Vayu",
                MahabharataMentions = 141,
                PrimaryWeapon = "mace",
                Brothers = [yudhishthira, arjuna]
            };
            var nakula = new Pandava() {
                Name = "Nakula",
                Father = "Nasatya",
                MahabharataMentions = 13,
                PrimaryWeapon = "sword",
                Brothers = [yudhishthira, arjuna, bhima]
            };
            var sahadeva = new Pandava() {
                Name = "Sahadeva",
                Father = "Kumara",
                MahabharataMentions = 16,
                PrimaryWeapon = "sword",
                Brothers = [yudhishthira, arjuna, bhima, nakula]
            };
            (yudhishthira.Brothers as IRelation).Canonicalize();
            (arjuna.Brothers as IRelation).Canonicalize();
            (bhima.Brothers as IRelation).Canonicalize();
            (nakula.Brothers as IRelation).Canonicalize();
            (sahadeva.Brothers as IRelation).Canonicalize();
            yudhishthira.Brothers.AddRange([arjuna, bhima, nakula, sahadeva]);
            arjuna.Brothers.AddRange([bhima, nakula, sahadeva]);
            bhima.Brothers.AddRange([nakula, sahadeva]);
            nakula.Brothers.AddRange([sahadeva]);
            var fixture = new TestFixture(typeof(Pandava));

            // Act
            await fixture.InitializeSchema();
            await fixture.Transactor.Update([yudhishthira, arjuna, bhima, nakula, sahadeva]);
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
            await fixture.Transaction.Received(1).CommitAsync();
            yudhishthira.Brothers.Should().HaveUnsavedEntryCount(0);
            arjuna.Brothers.Should().HaveUnsavedEntryCount(0);
            bhima.Brothers.Should().HaveUnsavedEntryCount(0);
            nakula.Brothers.Should().HaveUnsavedEntryCount(0);
        }

        [TestMethod] public async Task SelfReferentialEntityViaLocalization() {
            // Arrange
            var jasmine = new DisneyPrincess() {
                Name = "Jasmine",
                FilmSeries = "Aladdin",
                TotalAppearances = 70,
                Height = 5.33,
                HasAnimalSidekick = true,
                LivingParents = DisneyPrincess.Parent.Father,
                ThoughtsOnOthers = new DisneyPrincess.Opinion("LOC_JASMINE_OPINION")
            };
            var nala = new DisneyPrincess() {
                Name = "Nala",
                FilmSeries = "The Lion King",
                TotalAppearances = 32,
                Height = 3.75,
                HasAnimalSidekick = false,
                LivingParents = DisneyPrincess.Parent.Mother | DisneyPrincess.Parent.Father,
                ThoughtsOnOthers = new DisneyPrincess.Opinion("LOC_NALA_OPINOIN")
            };
            jasmine.ThoughtsOnOthers[jasmine] = 10.0;
            jasmine.ThoughtsOnOthers[nala] = 8.9;
            nala.ThoughtsOnOthers[nala] = 10.0;
            nala.ThoughtsOnOthers[jasmine] = 7.1;
            var fixture = new TestFixture(typeof(DisneyPrincess), typeof(DisneyPrincess.Opinion));

            // Act
            await fixture.InitializeSchema();
            await fixture.Transactor.Update([jasmine, jasmine.ThoughtsOnOthers, nala, nala.ThoughtsOnOthers]);
            var princessUpdateCmd = fixture.PrincipalCommands<DisneyPrincess>().UpdateCommand(ANY_ROWS);
            var princessUpdates = fixture.UpdatesFor(princessUpdateCmd);
            var opinionInsertCmd = fixture.PrincipalCommands<DisneyPrincess.Opinion>().InsertCommand(ANY_ROWS);
            var opinionInsertions = fixture.InsertionsFor(opinionInsertCmd);
            var opinionDeleteCmd = fixture.PrincipalCommands<DisneyPrincess.Opinion>().DeleteCommand(ANY_ROWS);
            var opinionDeletions = fixture.DeletionsFor(opinionDeleteCmd);
            var opinionUpdateCmd = fixture.PrincipalCommands<DisneyPrincess.Opinion>().UpdateCommand(ANY_ROWS);
            var opinionUpdates = fixture.UpdatesFor(opinionUpdateCmd);

            // Assert
            princessUpdateCmd.Connection.Should().Be(fixture.Connection);
            princessUpdateCmd.Transaction.Should().Be(fixture.Transaction);
            opinionInsertCmd.Connection.Should().Be(fixture.Connection);
            opinionInsertCmd.Transaction.Should().Be(fixture.Transaction);
            opinionDeleteCmd.Received(0).ExecuteNonQuery();
            opinionUpdateCmd.Received(0).ExecuteNonQuery();
            princessUpdates.Should().HaveCount(2);
            princessUpdates.Should().ContainRow(jasmine.Name, jasmine.FilmSeries, jasmine.TotalAppearances, jasmine.Height, jasmine.HasAnimalSidekick, ConversionOf(jasmine.LivingParents), jasmine.ThoughtsOnOthers.Key);
            princessUpdates.Should().ContainRow(nala.Name, nala.FilmSeries, nala.TotalAppearances, nala.Height, nala.HasAnimalSidekick, ConversionOf(nala.LivingParents), nala.ThoughtsOnOthers.Key);
            opinionInsertions.Should().HaveCount(4);
            opinionInsertions.Should().ContainRow(jasmine.ThoughtsOnOthers.Key, jasmine.Name, jasmine.ThoughtsOnOthers[jasmine]);
            opinionInsertions.Should().ContainRow(jasmine.ThoughtsOnOthers.Key, nala.Name, jasmine.ThoughtsOnOthers[nala]);
            opinionInsertions.Should().ContainRow(nala.ThoughtsOnOthers.Key, nala.Name, nala.ThoughtsOnOthers[nala]);
            opinionInsertions.Should().ContainRow(nala.ThoughtsOnOthers.Key, jasmine.Name, nala.ThoughtsOnOthers[jasmine]);
            opinionDeletions.Should().HaveCount(0);
            opinionUpdates.Should().HaveCount(0);
            fixture.ShouldBeOrdered(princessUpdateCmd, opinionInsertCmd);
            await fixture.Transaction.Received(1).CommitAsync();
            jasmine.ThoughtsOnOthers.Relation.Should().HaveUnsavedEntryCount(0);
            nala.ThoughtsOnOthers.Relation.Should().HaveUnsavedEntryCount(0);
        }

        [TestMethod] public async Task TransactionRolledBack() {
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
            await fixture.InitializeSchema();
            var action = async () => await fixture.Transactor.Update([chatbot]);

            // Assert
            await action.Should().ThrowExactlyAsync<InvalidOperationException>();
            await fixture.Transaction.Received(1).CommitAsync();
            await fixture.Transaction.Received(1).RollbackAsync();
        }

        [TestMethod] public async Task RollbackFails() {
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
            await fixture.InitializeSchema();
            var action = async () => await fixture.Transactor.Update([surgeonGeneral]);

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
