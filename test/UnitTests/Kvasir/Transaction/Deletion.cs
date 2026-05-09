using FluentAssertions;
using Kvasir.Core;
using Kvasir.Relations;
using Kvasir.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;

using static UT.Kvasir.Transaction.Deletion;
using static UT.Kvasir.Translation.TestLocalizations;

namespace UT.Kvasir.Transaction {
    [TestClass, TestCategory("Deletion")]
    public class DeletionTests {
        [TestMethod] public void SingleInstanceSingleEntityNoRelationsSingleFieldPrimaryKey() {
            // Arrange
            var pinata = new Pinata() {
                PinataID = Guid.NewGuid(),
                Price = 29.99M,
                Occasion = Pinata.Event.Quinceaneara,
                Breaker = null,
                AmountOfCandy = 37.516
            };
            var fixture = new TestFixture(typeof(Pinata));

            // Act
            fixture.Transactor.Delete(new object[] { pinata });
            var pinataCmd = fixture.PrincipalCommands<Pinata>().DeleteCommand(ANY_ROWS);
            var pinataDeletions = fixture.DeletionsFor(pinataCmd);

            // Assert
            pinataCmd.Connection.Should().Be(fixture.Connection);
            pinataCmd.Transaction.Should().Be(fixture.Transaction);
            pinataDeletions.Should().HaveCount(1);
            pinataDeletions.Should().ContainRow(pinata.PinataID);
            fixture.ShouldBeOrdered(pinataCmd);
            fixture.Transaction.Received(1).Commit();
        }

        [TestMethod] public void SingleInstanceSingleEntityNoRelationsMultiFieldPrimaryKey() {
            // Arrange
            var choir = new Choir() {
                GroupName = "The LaLaLambourghinis",
                Level = 'H',
                Established = new DateTime(1973, 5, 14),
                Members = 26,
                KnownSongs = 1874,
                NextConcert = new DateTime(2025, 3, 1),
                IsReligious = false
            };
            var fixture = new TestFixture(typeof(Choir));

            // Act
            fixture.Transactor.Delete(new object[] { choir });
            var choirCmd = fixture.PrincipalCommands<Choir>().DeleteCommand(ANY_ROWS);
            var choirDeletions = fixture.DeletionsFor(choirCmd);

            // Assert
            choirCmd.Connection.Should().Be(fixture.Connection);
            choirCmd.Transaction.Should().Be(fixture.Transaction);
            choirDeletions.Should().HaveCount(1);
            choirDeletions.Should().ContainRow(choir.GroupName, choir.Level, choir.Established);
            fixture.ShouldBeOrdered(choirCmd);
            fixture.Transaction.Received(1).Commit();
        }

        [TestMethod] public void MultipleInstancesSingleEntityNoRelations() {
            // Arrange
            var microsoft = new EquityOption() {
                Underlying = "MSFT",
                Expiration = new DateTime(2025, 1, 15),
                Strike = 417,
                Side = EquityOption.PutCall.Put,
                Moneyness = EquityOption.Status.InTheMoney,
                NBB = 8.12M,
                NBO = 8.63M
            };
            var nvidia = new EquityOption() {
                Underlying = "NVDA",
                Expiration = new DateTime(2025, 1, 17),
                Strike = 263,
                Side = EquityOption.PutCall.Put,
                Moneyness = EquityOption.Status.OutOfTheMoney,
                NBB = 4.57M,
                NBO = 4.59M
            };
            var disney = new EquityOption() {
                Underlying = "DIS",
                Expiration = new DateTime(2025, 1, 16),
                Strike = 111,
                Side = EquityOption.PutCall.Call,
                Moneyness = EquityOption.Status.AtTheMoney,
                NBB = 3.19M,
                NBO = 3.26M
            };
            var fixture = new TestFixture(typeof(EquityOption));

            // Act
            fixture.Transactor.Delete(new object[] { microsoft, nvidia, disney });
            var optionCmd = fixture.PrincipalCommands<EquityOption>().DeleteCommand(ANY_ROWS);
            var optionDeletions = fixture.DeletionsFor(optionCmd);

            // Assert
            optionCmd.Connection.Should().Be(fixture.Connection);
            optionCmd.Transaction.Should().Be(fixture.Transaction);
            optionDeletions.Should().HaveCount(3);
            optionDeletions.Should().ContainRow(microsoft.Underlying, microsoft.Expiration, microsoft.Strike, ConversionOf(microsoft.Side));
            optionDeletions.Should().ContainRow(nvidia.Underlying, nvidia.Expiration, nvidia.Strike, ConversionOf(nvidia.Side));
            optionDeletions.Should().ContainRow(disney.Underlying, disney.Expiration, disney.Strike, ConversionOf(disney.Side));
            fixture.ShouldBeOrdered(optionCmd);
            fixture.Transaction.Received(1).Commit();
        }

        [TestMethod] public void SingleInstanceSingleEntityNonEmptyScalarRelationsSavedElements() {
            // Arrange
            var k401 = new K401() {
                AccountID = Guid.NewGuid(),
                Provider = "Vanguard",
                CompanySponsor = "Companía La Hormiga",
                PercentMatch = 0.03,
                Balance = 241893.11M,
                Deposits = new RelationMap<DateTime, decimal>() {
                    { new DateTime(2019, 1, 1), 14519M },
                    { new DateTime(2020, 1, 1), 89054.16M },
                    { new DateTime(2021, 1, 1), 73832.44M },
                    { new DateTime(2022, 1, 1), 10615.29M },
                    { new DateTime(2023, 1, 1), 27804.01M },
                    { new DateTime(2024, 1, 1), 26068.21M }
                }
            };
            (k401.Deposits as IRelation).Canonicalize();
            var fixture = new TestFixture(typeof(K401));

            // Act
            fixture.Transactor.Delete(new object[] { k401 });
            var k401Cmd = fixture.PrincipalCommands<K401>().DeleteCommand(ANY_ROWS);
            var k401Deletions = fixture.DeletionsFor(k401Cmd);
            var depositsCmd = fixture.RelationCommands<K401>(0).DeleteCommand(ANY_ROWS);
            var depositsDeletions = fixture.DeletionsFor(depositsCmd);

            // Assert
            k401Cmd.Connection.Should().Be(fixture.Connection);
            k401Cmd.Transaction.Should().Be(fixture.Transaction);
            depositsCmd.Connection.Should().Be(fixture.Connection);
            depositsCmd.Transaction.Should().Be(fixture.Transaction);
            k401Deletions.Should().HaveCount(1);
            k401Deletions.Should().ContainRow(k401.AccountID);
            depositsDeletions.Should().HaveCount(1);
            depositsDeletions.Should().ContainRow(k401.AccountID);
            fixture.ShouldBeOrdered(depositsCmd, k401Cmd);
            fixture.Transaction.Received(1).Commit();
        }

        [TestMethod] public void SingleInstanceSingleEntityNonEmptyScalarRelationsDeletedElements() {
            // Arrange
            var jumpBall = new JumpBall() {
                GameID = Guid.NewGuid(),
                Instance = 4,
                League = JumpBall.Which.JUCO,
                Referee = "Luigi Aspartelli",
                Participants = new RelationMap<string, string>() {
                    { "Orwell's Junior College", "Dorian Carvanhna" },
                    { "Junior College of DeFowler County", "Aron Sat'ta" }
                }
            };
            (jumpBall.Participants as IRelation).Canonicalize();
            jumpBall.Participants.Clear();
            var fixture = new TestFixture(typeof(JumpBall));

            // Act
            fixture.Transactor.Delete(new object[] { jumpBall });
            var jumpBallCmd = fixture.PrincipalCommands<JumpBall>().DeleteCommand(ANY_ROWS);
            var jumpBallDeletions = fixture.DeletionsFor(jumpBallCmd);
            var participantsCmd = fixture.RelationCommands<JumpBall>(0).DeleteCommand(ANY_ROWS);
            var participantsDeletions = fixture.DeletionsFor(participantsCmd);

            // Assert
            jumpBallCmd.Connection.Should().Be(fixture.Connection);
            jumpBallCmd.Transaction.Should().Be(fixture.Transaction);
            participantsCmd.Connection.Should().Be(fixture.Connection);
            participantsCmd.Transaction.Should().Be(fixture.Transaction);
            jumpBallDeletions.Should().HaveCount(1);
            jumpBallDeletions.Should().ContainRow(jumpBall.GameID, jumpBall.Instance);
            participantsDeletions.Should().HaveCount(1);
            participantsDeletions.Should().ContainRow(jumpBall.GameID, jumpBall.Instance);
            fixture.ShouldBeOrdered(participantsCmd, jumpBallCmd);
            fixture.Transaction.Received(1).Commit();
        }

        [TestMethod] public void SingleInstanceSingleEntityNonEmptyScalarRelationsNewElements() {
            // Arrange
            var vault = new BankVault() {
                BankID = Guid.NewGuid(),
                Branch = "Ser Stanley's",
                VaultNumber = 3,
                Storage = new RelationMap<Guid, decimal>() {
                    { Guid.NewGuid(), 4175.51M },
                    { Guid.NewGuid(), 12595.11M },
                    { Guid.NewGuid(), 5819725.93M },
                    { Guid.NewGuid(), 871.20M }
                },
                Combination = new RelationOrderedList<sbyte>() {
                    61,
                    34,
                    17,
                    96
                }
            };
            var fixture = new TestFixture(typeof(BankVault));

            // Act
            fixture.Transactor.Delete(new object[] { vault });
            var vaultCmd = fixture.PrincipalCommands<BankVault>().DeleteCommand(ANY_ROWS);
            var vaultDeletions = fixture.DeletionsFor(vaultCmd);
            var combinationCmd = fixture.RelationCommands<BankVault>(0).DeleteCommand(ANY_ROWS);
            var combinationDeletions = fixture.DeletionsFor(combinationCmd);
            var storageCmd = fixture.RelationCommands<BankVault>(1).DeleteCommand(ANY_ROWS);
            var storageDeletions = fixture.DeletionsFor(storageCmd);

            // Assert
            vaultCmd.Connection.Should().Be(fixture.Connection);
            vaultCmd.Transaction.Should().Be(fixture.Transaction);
            vaultDeletions.Should().HaveCount(1);
            vaultDeletions.Should().ContainRow(vault.BankID, vault.Branch, vault.VaultNumber);
            combinationDeletions.Should().HaveCount(1);
            combinationDeletions.Should().ContainRow(vault.BankID, vault.Branch, vault.VaultNumber);
            storageDeletions.Should().HaveCount(1);
            storageDeletions.Should().ContainRow(vault.BankID, vault.Branch, vault.VaultNumber);
            fixture.ShouldBeOrdered(vaultCmd);
            fixture.Transaction.Received(1).Commit();
        }

        [TestMethod] public void SingleInstanceSingleEntityNonEmptyScalarRelationsMixedElements() {
            // Arrange
            var sushi = new SushiRoll() {
                RollType = "Dragon",
                Restaurant = "The Koi Pond",
                Price = 15.99M,
                RiceType = SushiRoll.Color.White,
                Ingredients = new RelationSet<string>() {
                    "King Crab",
                    "Tobiko",
                    "Cucumber",
                    "Cream Cheese"
                }
            };
            (sushi.Ingredients as IRelation).Canonicalize();
            sushi.Ingredients.Add("Avocado");
            sushi.Ingredients.Add("Jalapeño");
            sushi.Ingredients.Remove("Cream Cheese");
            var fixture = new TestFixture(typeof(SushiRoll));

            // Act
            fixture.Transactor.Delete(new object[] { sushi });
            var sushiCmd = fixture.PrincipalCommands<SushiRoll>().DeleteCommand(ANY_ROWS);
            var sushiDeletions = fixture.DeletionsFor(sushiCmd);
            var ingredientsCmd = fixture.RelationCommands<SushiRoll>(0).DeleteCommand(ANY_ROWS);
            var ingredientsDeletions = fixture.DeletionsFor(ingredientsCmd);

            // Assert
            sushiCmd.Connection.Should().Be(fixture.Connection);
            sushiCmd.Transaction.Should().Be(fixture.Transaction);
            ingredientsCmd.Connection.Should().Be(fixture.Connection);
            ingredientsCmd.Transaction.Should().Be(fixture.Transaction);
            sushiDeletions.Should().HaveCount(1);
            sushiDeletions.Should().ContainRow(sushi.RollType, sushi.Restaurant);
            ingredientsDeletions.Should().HaveCount(1);
            ingredientsDeletions.Should().ContainRow(sushi.RollType, sushi.Restaurant);
            fixture.ShouldBeOrdered(ingredientsCmd, sushiCmd);
            fixture.Transaction.Received(1).Commit();
        }
        
        [TestMethod] public void SingleInstanceSingleEntityEmptyScalarRelations() {
            // Arrange
            var woodshop = new Woodshop() {
                WoodshopID = Guid.NewGuid(),
                Owner = "Mariah Sroru",
                TotalIncidents = 35,
                InsuranceCoverage = 275000M,
                Tools = new RelationSet<string>(),
                TypesOfWood = new RelationOrderedList<Woodshop.Wood>()
            };
            var fixture = new TestFixture(typeof(Woodshop));

            // Act
            fixture.Transactor.Delete(new object[] { woodshop });
            var woodshopCmd = fixture.PrincipalCommands<Woodshop>().DeleteCommand(ANY_ROWS);
            var woodshopDeletions = fixture.DeletionsFor(woodshopCmd);
            var toolsCmd = fixture.RelationCommands<Woodshop>(0).DeleteCommand(ANY_ROWS);
            var toolsDeletions = fixture.DeletionsFor(toolsCmd);
            var woodsCmd = fixture.RelationCommands<Woodshop>(1).DeleteCommand(ANY_ROWS);
            var woodsDeletions = fixture.DeletionsFor(woodsCmd);

            // Assert
            woodshopCmd.Connection.Should().Be(fixture.Connection);
            woodshopCmd.Transaction.Should().Be(fixture.Transaction);
            woodshopDeletions.Should().HaveCount(1);
            woodshopDeletions.Should().ContainRow(woodshop.WoodshopID);
            toolsDeletions.Should().HaveCount(1);
            toolsDeletions.Should().ContainRow(woodshop.WoodshopID);
            woodsDeletions.Should().HaveCount(1);
            woodsDeletions.Should().ContainRow(woodshop.WoodshopID);
            fixture.ShouldBeOrdered(woodshopCmd);
            fixture.Transaction.Received(1).Commit();
        }

        [TestMethod] public void MultipleInstancesSingleEntityScalarRelations() {
            // Arrange
            var store0 = new ConvenienceStore() {
                StoreBrand = "7-Eleven",
                StoreNumber = 71624,
                Address = "16 East Juwanja Avenue",
                IsBodega = false,
                Products = new RelationMap<string, decimal>() {
                    { "Fountain Drink", 3.99M },
                    { "Popcorn", 11.55M },
                    { "Pizza Slice", 13.00M },
                    { "Cigarettes", 18.34M }
                },
                Employees = new RelationOrderedList<string>() {
                    "Donald Evervass",
                    "Kathleen Tuasaa",
                    "Bri'Ann Camelson"
                }
            };
            var store1 = new ConvenienceStore() {
                StoreBrand = "La Bodegita",
                StoreNumber = 1,
                Address = "Corner of Vineyard and St. Charlie's",
                IsBodega = true,
                Products = new RelationMap<string, decimal>() {
                    { "Fountain Drink", 1.50M },
                    { "Churro", 0.75M }
                },
                Employees = new RelationOrderedList<string>() {
                    "Carl X. Wassel"
                }
            };
            (store0.Products as IRelation).Canonicalize();
            (store0.Employees as IRelation).Canonicalize();
            (store1.Products as IRelation).Canonicalize();
            (store1.Employees as IRelation).Canonicalize();
            var fixture = new TestFixture(typeof(ConvenienceStore));

            // Act
            fixture.Transactor.Delete(new object[] { store0, store1 });
            var storeCmd = fixture.PrincipalCommands<ConvenienceStore>().DeleteCommand(ANY_ROWS);
            var storeDeletions = fixture.DeletionsFor(storeCmd);
            var employeesCmd = fixture.RelationCommands<ConvenienceStore>(0).DeleteCommand(ANY_ROWS);
            var employeesDeletions = fixture.DeletionsFor(employeesCmd);
            var productsCmd = fixture.RelationCommands<ConvenienceStore>(1).DeleteCommand(ANY_ROWS);
            var productsDeletions = fixture.DeletionsFor(productsCmd);

            // Assert
            storeCmd.Connection.Should().Be(fixture.Connection);
            storeCmd.Transaction.Should().Be(fixture.Transaction);
            employeesCmd.Connection.Should().Be(fixture.Connection);
            employeesCmd.Transaction.Should().Be(fixture.Transaction);
            productsCmd.Connection.Should().Be(fixture.Connection);
            productsCmd.Transaction.Should().Be(fixture.Transaction);
            storeDeletions.Should().HaveCount(2);
            storeDeletions.Should().ContainRow(store0.StoreBrand, store0.StoreNumber);
            storeDeletions.Should().ContainRow(store1.StoreBrand, store1.StoreNumber);
            employeesDeletions.Should().HaveCount(2);
            employeesDeletions.Should().ContainRow(store0.StoreBrand, store0.StoreNumber);
            employeesDeletions.Should().ContainRow(store1.StoreBrand, store1.StoreNumber);
            productsDeletions.Should().HaveCount(2);
            productsDeletions.Should().ContainRow(store0.StoreBrand, store0.StoreNumber);
            productsDeletions.Should().ContainRow(store1.StoreBrand, store1.StoreNumber);
            fixture.ShouldBeOrdered((employeesCmd, productsCmd), storeCmd);
            fixture.Transaction.Received(1).Commit();
        }

        [TestMethod] public void SingleInstanceSingleLocalizationSavedValues() {
            // Arrange
            var showtime = new Showtime("LOC_SPAMALOT_SHOWTIME");
            showtime["US_CENTRAL"] = 1900;
            showtime["US_EASTERN"] = 2000;
            showtime["GREENWICH_MEAN"] = 100;
            (showtime.Relation as IRelation).Canonicalize();
            var fixture = new TestFixture(typeof(Showtime));

            // Act
            fixture.Transactor.Delete(new object[] { showtime });
            var showtimeCmd = fixture.PrincipalCommands<Showtime>().DeleteCommand(ANY_ROWS);
            var showtimeDeletions = fixture.DeletionsFor(showtimeCmd);

            // Assert
            showtimeCmd.Connection.Should().Be(fixture.Connection);
            showtimeCmd.Transaction.Should().Be(fixture.Transaction);
            showtimeDeletions.Should().HaveCount(1);
            showtimeDeletions.Should().ContainRow(showtime.Key);
            fixture.ShouldBeOrdered(showtimeCmd);
            fixture.Transaction.Received(1).Commit();
        }

        [TestMethod] public void SingleInstanceSingleLocalizationDeletedValues() {
            // Arrange
            var termOfEndearment = new TermOfEndearment("LOC_SWEETHEART");
            termOfEndearment[Language.English] = "Sweetheart";
            (termOfEndearment.Relation as IRelation).Canonicalize();
            termOfEndearment.Delocalize(Language.English);
            var fixture = new TestFixture(typeof(TermOfEndearment));

            // Act
            fixture.Transactor.Delete(new object[] { termOfEndearment });
            var termOfEndearmentCmd = fixture.PrincipalCommands<TermOfEndearment>().DeleteCommand(ANY_ROWS);
            var termOfEndearmentDeletions = fixture.DeletionsFor(termOfEndearmentCmd);

            // Act
            termOfEndearmentCmd.Connection.Should().Be(fixture.Connection);
            termOfEndearmentCmd.Transaction.Should().Be(fixture.Transaction);
            termOfEndearmentDeletions.Should().HaveCount(1);
            termOfEndearmentDeletions.Should().ContainRow(termOfEndearment.Key);
            fixture.ShouldBeOrdered(termOfEndearmentCmd);
            fixture.Transaction.Received(1).Commit();
        }

        [TestMethod] public void SingleInstanceSingleLocalizationNewValues() {
            // Arrange
            var label = new Label(81929125024);
            label['E'] = "Fragile but Unimportant Airborne Cargo";
            label['q'] = "Destination: Nuuk Airport";
            label['$'] = "Paid in Full";
            var fixture = new TestFixture(typeof(Label));

            // Act
            fixture.Transactor.Delete(new object[] { label });
            var labelCmd = fixture.PrincipalCommands<Label>().DeleteCommand(ANY_ROWS);
            var labelDeletions = fixture.DeletionsFor(labelCmd);

            // Act
            labelCmd.Connection.Should().Be(fixture.Connection);
            labelCmd.Transaction.Should().Be(fixture.Transaction);
            labelDeletions.Should().HaveCount(1);
            labelDeletions.Should().ContainRow(label.Key);
            fixture.ShouldBeOrdered(labelCmd);
            fixture.Transaction.Received(1).Commit();
        }

        [TestMethod] public void SingleInstanceSingleLocalizationMixedValues() {
            // Arrange
            var verb = new ConjugatedVerb(-19033);
            verb[1.10] = "(yo) tengo";
            verb[1.11] = "(yo) tuve";
            verb[1.12] = "(yo) tenía";
            verb[1.13] = "(yo) tendría";
            (verb.Relation as IRelation).Canonicalize();
            verb.Delocalize(1.13);
            verb[2.10] = "(nosotros) tenemos";
            verb[2.11] = "(nosotros) tuvimos";
            var fixture = new TestFixture(typeof(ConjugatedVerb));

            // Act
            fixture.Transactor.Delete(new object[] { verb });
            var conjugationCmd = fixture.PrincipalCommands<ConjugatedVerb>().DeleteCommand(ANY_ROWS);
            var conjugationDeletions = fixture.DeletionsFor(conjugationCmd);

            // Act
            conjugationCmd.Connection.Should().Be(fixture.Connection);
            conjugationCmd.Transaction.Should().Be(fixture.Transaction);
            conjugationDeletions.Should().HaveCount(1);
            conjugationDeletions.Should().ContainRow(verb.Key);
            fixture.ShouldBeOrdered(conjugationCmd);
            fixture.Transaction.Received(1).Commit();
        }

        [TestMethod] public void MultipleInstancesSingleLocalization() {
            // Arrange
            var coordinate0 = new Coordinate(Guid.NewGuid());
            var coordinate1 = new Coordinate(Guid.NewGuid());
            var coordinate2 = new Coordinate(Guid.NewGuid());
            var fixture = new TestFixture(typeof(Coordinate));

            // Act
            fixture.Transactor.Delete(new object[] { coordinate0, coordinate1, coordinate2 });
            var coordinateCmd = fixture.PrincipalCommands<Coordinate>().DeleteCommand(ANY_ROWS);
            var coordinateDeletions = fixture.DeletionsFor(coordinateCmd);

            // Act
            coordinateCmd.Connection.Should().Be(fixture.Connection);
            coordinateCmd.Transaction.Should().Be(fixture.Transaction);
            coordinateDeletions.Should().HaveCount(3);
            coordinateDeletions.Should().ContainRow(coordinate0.Key);
            coordinateDeletions.Should().ContainRow(coordinate1.Key);
            coordinateDeletions.Should().ContainRow(coordinate2.Key);
            fixture.ShouldBeOrdered(coordinateCmd);
            fixture.Transaction.Received(1).Commit();
        }

        [TestMethod] public void MultipleUnrelatedEntities() {
            // Arrange
            var shanty = new SeaShanty() {
                Title = "Spanish Ladies",
                EarliestAttestation = null,
                Kind = SeaShanty.Type.Misc
            };
            var marble = new ElginMarble() {
                Number = 27,
                Description = "Weber-Laborde Head",
                HasBeenRepatriated = false,
                Source = ElginMarble.Type.Pediment
            };
            var cepheid = new CepheidVariable() {
                Name = "RS Puppis",
                PeriodDays = 41.5f,
                Distance = 5600,
                Mass = 9.2,
                Luminosity = 21700,
                Class = CepheidVariable.Type.Classical
            };
            var fixture = new TestFixture(typeof(SeaShanty), typeof(ElginMarble), typeof(CepheidVariable));

            // Act
            fixture.Transactor.Delete(new object[] { shanty, marble, cepheid });
            var shantyCmd = fixture.PrincipalCommands<SeaShanty>().DeleteCommand(ANY_ROWS);
            var shantyDeletions = fixture.DeletionsFor(shantyCmd);
            var marbleCmd = fixture.PrincipalCommands<ElginMarble>().DeleteCommand(ANY_ROWS);
            var marbleDeletions = fixture.DeletionsFor(marbleCmd);
            var cepheidCmd = fixture.PrincipalCommands<CepheidVariable>().DeleteCommand(ANY_ROWS);
            var cepheidDeletions = fixture.DeletionsFor(cepheidCmd);

            // Assert
            shantyCmd.Connection.Should().Be(fixture.Connection);
            shantyCmd.Transaction.Should().Be(fixture.Transaction);
            marbleCmd.Connection.Should().Be(fixture.Connection);
            marbleCmd.Transaction.Should().Be(fixture.Transaction);
            cepheidCmd.Connection.Should().Be(fixture.Connection);
            cepheidCmd.Transaction.Should().Be(fixture.Transaction);
            shantyDeletions.Should().HaveCount(1);
            shantyDeletions.Should().ContainRow(shanty.Title);
            marbleDeletions.Should().HaveCount(1);
            marbleDeletions.Should().ContainRow(marble.Number);
            cepheidDeletions.Should().HaveCount(1);
            cepheidDeletions.Should().ContainRow(cepheid.Name);
            fixture.ShouldBeOrdered((shantyCmd, marbleCmd, cepheidCmd));
            fixture.Transaction.Received(1).Commit();
        }

        [TestMethod] public void MultipleUnrelatedLocalizations() {
            // Arrange
            var sunset = new Sunset(new DateOnly(1983, 4, 19));
            sunset["CHICAGO"] = 2013;
            sunset["BEIJING"] = 1429;
            sunset["NAIROBI"] = 756;
            var wingding = new Wingding(67);
            wingding[1] = "👍︎";
            wingding[2] = "👉︎";
            wingding[3] = "⮓";
            var sprite = new Sprite("LOC_PORTABELLO_MUSHROOM_SPRITE");
            sprite["regular"] = Guid.NewGuid();
            sprite["shiny"] = Guid.NewGuid();
            sprite["shadowed"] = Guid.NewGuid();
            sprite["glowing"] = Guid.NewGuid();
            sprite["silhouette"] = Guid.NewGuid();
            var fixture = new TestFixture(typeof(Sunset), typeof(Wingding), typeof(Sprite));

            // Act
            fixture.Transactor.Delete(new object[] { sunset, wingding, sprite });
            var sunsetCmd = fixture.PrincipalCommands<Sunset>().DeleteCommand(ANY_ROWS);
            var sunsetDeletions = fixture.DeletionsFor(sunsetCmd);
            var wingdingCmd = fixture.PrincipalCommands<Wingding>().DeleteCommand(ANY_ROWS);
            var wingdingDeletions = fixture.DeletionsFor(wingdingCmd);
            var spriteCmd = fixture.PrincipalCommands<Sprite>().DeleteCommand(ANY_ROWS);
            var spriteDeletions = fixture.DeletionsFor(spriteCmd);

            // Assert
            sunsetCmd.Connection.Should().Be(fixture.Connection);
            sunsetCmd.Transaction.Should().Be(fixture.Transaction);
            wingdingCmd.Connection.Should().Be(fixture.Connection);
            wingdingCmd.Transaction.Should().Be(fixture.Transaction);
            spriteCmd.Connection.Should().Be(fixture.Connection);
            spriteCmd.Transaction.Should().Be(fixture.Transaction);
            sunsetDeletions.Should().HaveCount(1);
            sunsetDeletions.Should().ContainRow(sunset.Key);
            wingdingDeletions.Should().HaveCount(1);
            wingdingDeletions.Should().ContainRow(wingding.Key);
            spriteDeletions.Should().HaveCount(1);
            spriteDeletions.Should().ContainRow(sprite.Key);
            fixture.ShouldBeOrdered((sunsetCmd, wingdingCmd, spriteCmd));
            fixture.Transaction.Received(1).Commit();
        }

        [TestMethod] public void MultipleEntitiesRelatedByReferenceChain() {
            // Arrange
            var costume = new CostumeParty.Costume() {
                CostumeID = Guid.NewGuid(),
                Character = "Daenerys Targaryen",
                IsHomemade = true,
                Cost = 23.76M
            };
            var partygoer = new CostumeParty.Partygoer() {
                Name = "Ariel Y. Hanossit",
                WasInvited = true,
                Costume = costume
            };
            var party = new CostumeParty() {
                PartyID = Guid.NewGuid(),
                Host = "Darren Nerrad",
                Date = new DateTime(2013, 10, 28),
                ForHalloween = true,
                Winner = partygoer
            };
            var fixture = new TestFixture(typeof(CostumeParty), typeof(CostumeParty.Partygoer), typeof(CostumeParty.Costume));

            // Act
            fixture.Transactor.Delete(new object[] { costume, party, partygoer });
            var costumeCmd = fixture.PrincipalCommands<CostumeParty.Costume>().DeleteCommand(ANY_ROWS);
            var costumeDeletios = fixture.DeletionsFor(costumeCmd);
            var partygoerCmd = fixture.PrincipalCommands<CostumeParty.Partygoer>().DeleteCommand(ANY_ROWS);
            var partygoerDeletions = fixture.DeletionsFor(partygoerCmd);
            var partyCmd = fixture.PrincipalCommands<CostumeParty>().DeleteCommand(ANY_ROWS);
            var partyDeletios = fixture.DeletionsFor(partyCmd);

            // Assert
            costumeCmd.Connection.Should().Be(fixture.Connection);
            costumeCmd.Transaction.Should().Be(fixture.Transaction);
            partygoerCmd.Connection.Should().Be(fixture.Connection);
            partygoerCmd.Transaction.Should().Be(fixture.Transaction);
            partyCmd.Connection.Should().Be(fixture.Connection);
            partyCmd.Transaction.Should().Be(fixture.Transaction);
            costumeDeletios.Should().HaveCount(1);
            costumeDeletios.Should().ContainRow(costume.CostumeID);
            partygoerDeletions.Should().HaveCount(1);
            partygoerDeletions.Should().ContainRow(partygoer.Name);
            partyDeletios.Should().HaveCount(1);
            partyDeletios.Should().ContainRow(party.PartyID);
            fixture.ShouldBeOrdered(costumeCmd, partygoerCmd, partyCmd);
            fixture.Transaction.Received(1).Commit();
        }

        [TestMethod] public void MultipleEntitiesRelatedByReferenceTree() {
            // Arrange
            var album = new WeirdAlParody.Album() {
                Title = "Straight Outta Lynwood",
                Released = new DateTime(2006, 9, 26)
            };
            var beatIt = new WeirdAlParody.Song() {
                Artist = "Michael Jackson",
                Title = "Beat It",
                RecordsSold = 10000000,
                WonGrammy = true
            };
            var eatIt = new WeirdAlParody() {
                Title = "Eat It",
                Released = new DateTime(1984, 2, 28),
                SongAlbum = null,
                Length = 3.19,
                Basis = beatIt,
                Label = "Scotti Brothers"
            };
            var fixture = new TestFixture(typeof(WeirdAlParody), typeof(WeirdAlParody.Song), typeof(WeirdAlParody.Album));

            // Act
            fixture.Transactor.Delete(new object[] {album, beatIt, eatIt});
            var albumCmd = fixture.PrincipalCommands<WeirdAlParody.Album>().DeleteCommand(ANY_ROWS);
            var albumDeletions = fixture.DeletionsFor(albumCmd);
            var songCmd = fixture.PrincipalCommands<WeirdAlParody.Song>().DeleteCommand(ANY_ROWS);
            var songDeletions = fixture.DeletionsFor(songCmd);
            var parodyCmd = fixture.PrincipalCommands<WeirdAlParody>().DeleteCommand(ANY_ROWS);
            var parodyDeletions = fixture.DeletionsFor(parodyCmd);

            // Assert
            albumCmd.Connection.Should().Be(fixture.Connection);
            albumCmd.Transaction.Should().Be(fixture.Transaction);
            songCmd.Connection.Should().Be(fixture.Connection);
            songCmd.Transaction.Should().Be(fixture.Transaction);
            parodyCmd.Connection.Should().Be(fixture.Connection);
            parodyCmd.Transaction.Should().Be(fixture.Transaction);
            albumDeletions.Should().HaveCount(1);
            albumDeletions.Should().ContainRow(album.Title);
            songDeletions.Should().HaveCount(1);
            songDeletions.Should().ContainRow(beatIt.Artist, beatIt.Title);
            parodyDeletions.Should().HaveCount(1);
            parodyDeletions.Should().ContainRow(eatIt.Title);
            fixture.ShouldBeOrdered((songCmd, albumCmd), parodyCmd);
            fixture.Transaction.Received(1).Commit();
        }

        [TestMethod] public void MultipleEntitiesRelatedByRelation() {
            // Arrange
            var lawyer0 = new Affidavit.Lawyer() {
                BarNumber = Guid.NewGuid(),
                Name = "Aimee Maracañó",
                LawSchool = "Columbia",
                Employment = Affidavit.Lawyer.Employer.LawFirm
            };
            var lawyer1 = new Affidavit.Lawyer() {
                BarNumber = Guid.NewGuid(),
                Name = "Sara al-Hammib",
                LawSchool = "Arizona State",
                Employment = Affidavit.Lawyer.Employer.ProBono
            };
            var lawyer2 = new Affidavit.Lawyer() {
                BarNumber = Guid.NewGuid(),
                Name = "Laura Wegson",
                LawSchool = "Yale",
                Employment = Affidavit.Lawyer.Employer.DistrictAttorney
            };
            var affidavit = new Affidavit() {
                ID = Guid.NewGuid(),
                Affiant = "Michelle Kabbell",
                NotarizationDate = new DateTime(1993, 4, 17),
                PartOfPleaDeal = false,
                LawyersInvolved = new RelationOrderedList<Affidavit.Lawyer>() {
                    lawyer0,
                    lawyer2
                }
            };
            var fixture = new TestFixture(typeof(Affidavit), typeof(Affidavit.Lawyer));

            // Act
            fixture.Transactor.Delete(new object[] { affidavit, lawyer0, lawyer1, lawyer2 });
            var affidavitCmd = fixture.PrincipalCommands<Affidavit>().DeleteCommand(ANY_ROWS);
            var affidavitDeletions = fixture.DeletionsFor(affidavitCmd);
            var lawyerCmd = fixture.PrincipalCommands<Affidavit.Lawyer>().DeleteCommand(ANY_ROWS);
            var lawyerDeletions = fixture.DeletionsFor(lawyerCmd);
            var involvementCmd = fixture.RelationCommands<Affidavit>(0).DeleteCommand(ANY_ROWS);
            var involvementDeletions = fixture.DeletionsFor(involvementCmd);

            // Assert
            affidavitCmd.Connection.Should().Be(fixture.Connection);
            affidavitCmd.Transaction.Should().Be(fixture.Transaction);
            lawyerCmd.Connection.Should().Be(fixture.Connection);
            lawyerCmd.Transaction.Should().Be(fixture.Transaction);
            involvementCmd.Connection.Should().Be(fixture.Connection);
            involvementCmd.Transaction.Should().Be(fixture.Transaction);
            affidavitDeletions.Should().HaveCount(1);
            affidavitDeletions.Should().ContainRow(affidavit.ID);
            lawyerDeletions.Should().HaveCount(3);
            lawyerDeletions.Should().ContainRow(lawyer0.BarNumber, lawyer0.Name);
            lawyerDeletions.Should().ContainRow(lawyer1.BarNumber, lawyer1.Name);
            lawyerDeletions.Should().ContainRow(lawyer2.BarNumber, lawyer2.Name);
            involvementDeletions.Should().HaveCount(1);
            involvementDeletions.Should().ContainRow(affidavit.ID);
            fixture.ShouldBeOrdered(involvementCmd, (lawyerCmd, affidavitCmd));
            fixture.Transaction.Received(1).Commit();
        }

        [TestMethod] public void MultipleEntitiesRelatedByScalarLocalization() {
            // Arrange
            var bandeirante = new Bandeirante() {
                ID = Guid.NewGuid(),
                YearsActive = 3,
                HomeState = new LocalizedText("LOC_SAO_PAULO"),
                TotalLooted = new LocalizedCurrency("LOC_TWO_MILLION_DOLLARS"),
                IsMameluco = true,
                SpokePaulistaGeneral = true
            };
            var fixture = new TestFixture(typeof(Bandeirante), typeof(LocalizedText), typeof(LocalizedCurrency));

            // Act
            fixture.Transactor.Delete(new object[] { bandeirante, bandeirante.HomeState, bandeirante.TotalLooted });
            var bandeiranteCmd = fixture.PrincipalCommands<Bandeirante>().DeleteCommand(ANY_ROWS);
            var bandeiranteDeletions = fixture.DeletionsFor(bandeiranteCmd);
            var textCmd = fixture.PrincipalCommands<LocalizedText>().DeleteCommand(ANY_ROWS);
            var textDeletions = fixture.DeletionsFor(textCmd);
            var currencyCmd = fixture.PrincipalCommands<LocalizedCurrency>().DeleteCommand(ANY_ROWS);
            var currencyDeletions = fixture.DeletionsFor(currencyCmd);

            // Assert
            bandeiranteCmd.Connection.Should().Be(fixture.Connection);
            bandeiranteCmd.Transaction.Should().Be(fixture.Transaction);
            textCmd.Connection.Should().Be(fixture.Connection);
            textCmd.Transaction.Should().Be(fixture.Transaction);
            currencyCmd.Connection.Should().Be(fixture.Connection);
            currencyCmd.Transaction.Should().Be(fixture.Transaction);
            bandeiranteDeletions.Should().HaveCount(1);
            bandeiranteDeletions.Should().ContainRow(bandeirante.ID);
            textDeletions.Should().HaveCount(1);
            textDeletions.Should().ContainRow(bandeirante.HomeState.Key);
            currencyDeletions.Should().HaveCount(1);
            currencyDeletions.Should().ContainRow(bandeirante.TotalLooted.Key);
            fixture.ShouldBeOrdered((bandeiranteCmd, textCmd, currencyCmd));
            fixture.Transaction.Received(1).Commit();
        }

        [TestMethod] public void MultipleEntitiesRelatedByReferenceLocalization() {
            // Arrange
            var yyyymmdd = new BirthdayParty.YearMonthDay() {
                Year = 1873,
                Month = 4,
                Day = 11,
                KnownAs = null,
                Spelling = "1873-04-11"
            };
            var wordyDate = new BirthdayParty.YearMonthDay() {
                Year = 1873,
                Month = 4,
                Day = 11,
                KnownAs = null,
                Spelling = "April 11th, 1873"
            };
            var party = new BirthdayParty() {
                Person = "Gerbhardt von Lützütuttüle",
                Date = new BirthdayParty.LocalizedYearMonthDay("LOC_1873|4|11"),
                Location = "Hesse's Spittoon and Salamandery",
                InvitationOnly = true,
                Attendees = 58,
                TotalGiftValue = 1873.09M
            };
            party.Date["YYYYMMDD"] = yyyymmdd;
            party.Date["<month> <ordinal>, <year>"] = wordyDate;
            (party.Date.Relation as IRelation).Canonicalize();
            var fixture = new TestFixture(typeof(BirthdayParty), typeof(BirthdayParty.YearMonthDay), typeof(BirthdayParty.LocalizedYearMonthDay));

            // Act
            fixture.Transactor.Delete(new object[] { yyyymmdd, wordyDate, party, party.Date });
            var partyCmd = fixture.PrincipalCommands<BirthdayParty>().DeleteCommand(ANY_ROWS);
            var partyDeletions = fixture.DeletionsFor(partyCmd);
            var dateCmd = fixture.PrincipalCommands<BirthdayParty.YearMonthDay>().DeleteCommand(ANY_ROWS);
            var dateDeletions = fixture.DeletionsFor(dateCmd);
            var localizationCmd = fixture.PrincipalCommands<BirthdayParty.LocalizedYearMonthDay>().DeleteCommand(ANY_ROWS);
            var localizationDeletions = fixture.DeletionsFor(localizationCmd);

            // Assert
            partyCmd.Connection.Should().Be(fixture.Connection);
            partyCmd.Transaction.Should().Be(fixture.Transaction);
            dateCmd.Connection.Should().Be(fixture.Connection);
            dateCmd.Transaction.Should().Be(fixture.Transaction);
            localizationCmd.Connection.Should().Be(fixture.Connection);
            localizationCmd.Transaction.Should().Be(fixture.Transaction);
            partyDeletions.Should().HaveCount(1);
            partyDeletions.Should().ContainRow(party.Person);
            dateDeletions.Should().HaveCount(2);
            dateDeletions.Should().ContainRow(yyyymmdd.Year, yyyymmdd.Month, yyyymmdd.Day, yyyymmdd.Spelling);
            dateDeletions.Should().ContainRow(wordyDate.Year, wordyDate.Month, wordyDate.Day, wordyDate.Spelling);
            localizationDeletions.Should().HaveCount(1);
            localizationDeletions.Should().ContainRow(party.Date.Key);
            fixture.ShouldBeOrdered((partyCmd, localizationCmd));
            fixture.ShouldBeOrdered(localizationCmd, dateCmd);
        }

        [TestMethod] public void MultipleEntitiesRelatedByRelationLocalization() {
            // Arrange
            var text0 = new LocalizedNullableText("LOC_SCANDERRA");
            var text1 = new LocalizedNullableText("LOC_TA_FLUMIN");
            var lawyer = new Lawyer() {
                BarNumber = Guid.NewGuid(),
                Name = "Lord Stanley Razzellon IV, Eighteenth Earl of Thornentonshire",
                AlmaMater = "London Temporary Secondary School of Greengrass-upon-Thames-upon-Earth",
                Employers = new RelationOrderedList<LocalizedNullableText>() {
                    text0,
                    text1
                },
                WinPercentage = 0.03,
                AnnualSalary = 57000M,
                HasBeenDisbarred = false,
                IsJudge = false,
                Practice = Lawyer.Field.Malpractice
            };
            var fixture = new TestFixture(typeof(Lawyer), typeof(LocalizedNullableText));

            // Act
            fixture.Transactor.Delete(new object[] { text0, text1, lawyer });
            var lawyerCmd = fixture.PrincipalCommands<Lawyer>().DeleteCommand(ANY_ROWS);
            var lawyerDeletions = fixture.DeletionsFor(lawyerCmd);
            var textCmd = fixture.PrincipalCommands<LocalizedNullableText>().DeleteCommand(ANY_ROWS);
            var textDeletions = fixture.DeletionsFor(textCmd);
            var employersCmd = fixture.RelationCommands<Lawyer>(0).DeleteCommand(ANY_ROWS);
            var employersDeletions = fixture.DeletionsFor(employersCmd);

            // Assert
            lawyerCmd.Connection.Should().Be(fixture.Connection);
            lawyerCmd.Transaction.Should().Be(fixture.Transaction);
            textCmd.Connection.Should().Be(fixture.Connection);
            textCmd.Transaction.Should().Be(fixture.Transaction);
            employersCmd.Connection.Should().Be(fixture.Connection);
            employersCmd.Transaction.Should().Be(fixture.Transaction);
            lawyerDeletions.Should().HaveCount(1);
            lawyerDeletions.Should().ContainRow(lawyer.BarNumber);
            textDeletions.Should().HaveCount(2);
            textDeletions.Should().ContainRow(text0.Key);
            textDeletions.Should().ContainRow(text1.Key);
            employersDeletions.Should().HaveCount(1);
            employersDeletions.Should().ContainRow(lawyer.BarNumber);
            fixture.ShouldBeOrdered(employersCmd, lawyerCmd);
            fixture.ShouldBeOrdered((employersCmd, textCmd));
        }

        [TestMethod] public void SelfReferentialEntityViaRelation() {
            // Arrange
            var masseuse0 = new Masseuse() {
                LicenseNumber = Guid.NewGuid(),
                Style = Masseuse.Kind.Shiatsu,
                Name = "Della Mortez",
                IsFreelance = false,
                NumTables = 6,
                Teachers = new RelationSet<Masseuse>()
            };
            var masseuse1 = new Masseuse() {
                LicenseNumber = Guid.NewGuid(),
                Style = Masseuse.Kind.Sports,
                Name = "Aaron-Jack Rowesto",
                IsFreelance = true,
                NumTables = 2,
                Teachers = new RelationSet<Masseuse>() {
                    masseuse0
                }
            };
            var masseuse2 = new Masseuse() {
                LicenseNumber = Guid.NewGuid(),
                Style = Masseuse.Kind.Acupuncture,
                Name = "Houyang Qi",
                IsFreelance = true,
                NumTables = 1,
                Teachers = new RelationSet<Masseuse>() {
                    masseuse0,
                    masseuse1
                }
            };
            masseuse0.Teachers.Canonicalize();
            masseuse1.Teachers.Canonicalize();
            masseuse2.Teachers.Canonicalize();
            var fixture = new TestFixture(typeof(Masseuse));

            // Act
            fixture.Transactor.Delete(new object[] { masseuse0, masseuse1, masseuse2 });
            var masseuseCmd = fixture.PrincipalCommands<Masseuse>().DeleteCommand(ANY_ROWS);
            var masseuseDeletions = fixture.DeletionsFor(masseuseCmd);
            var teachersCmd = fixture.RelationCommands<Masseuse>(0).DeleteCommand(ANY_ROWS);
            var teachersDeletions = fixture.DeletionsFor(teachersCmd);

            // Assert
            masseuseCmd.Connection.Should().Be(fixture.Connection);
            masseuseCmd.Transaction.Should().Be(fixture.Transaction);
            masseuseDeletions.Should().HaveCount(3);
            masseuseDeletions.Should().ContainRow(masseuse0.LicenseNumber, ConversionOf(masseuse0.Style));
            masseuseDeletions.Should().ContainRow(masseuse1.LicenseNumber, ConversionOf(masseuse1.Style));
            masseuseDeletions.Should().ContainRow(masseuse2.LicenseNumber, ConversionOf(masseuse2.Style));
            teachersDeletions.Should().HaveCount(3);
            teachersDeletions.Should().ContainRow(masseuse0.LicenseNumber, ConversionOf(masseuse0.Style));
            teachersDeletions.Should().ContainRow(masseuse1.LicenseNumber, ConversionOf(masseuse1.Style));
            teachersDeletions.Should().ContainRow(masseuse2.LicenseNumber, ConversionOf(masseuse2.Style));
            fixture.ShouldBeOrdered(teachersCmd, masseuseCmd);
            fixture.Transaction.Received(1).Commit();
        }

        [TestMethod] public void SelfReferentialEntityViaLocalization() {
            // Arrange
            var radiologist0 = new Radiologist() {
                MedicalID = Guid.NewGuid(),
                Name = "Dr. Spence-ur Werriolotto",
                CanUseMRI = true,
                BecquerelsExposure = 49103.8082,
                DoesCancerTreatment = false,
                Hospital = null,
                Superior = new Radiologist.OnCallEscalation(new DateOnly(2019, 6, 23))
            };
            var radiologist1 = new Radiologist() {
                MedicalID = Guid.NewGuid(),
                Name = "Dr. Horae Van't'Ulo",
                CanUseMRI = true,
                BecquerelsExposure = 0.0,
                DoesCancerTreatment = true,
                Hospital = "Our Holy Lady of the Savior of the Angel of the Oasis",
                Superior = radiologist0.Superior
            };
            var radiologist2 = new Radiologist() {
                MedicalID = Guid.NewGuid(),
                Name = "Dr. Ez K. LaMoBavann",
                CanUseMRI = false,
                BecquerelsExposure = 1928129124.120412,
                DoesCancerTreatment = false,
                Hospital = "Mount Cozutto National Hospital",
                Superior = radiologist0.Superior
            };
            radiologist0.Superior[1] = radiologist2;
            (radiologist0.Superior.Relation as IRelation).Canonicalize();
            var fixture = new TestFixture(typeof(Radiologist), typeof(Radiologist.OnCallEscalation));

            // Act
            fixture.Transactor.Delete(new object[] { radiologist0, radiologist1, radiologist2, radiologist0.Superior });
            var radiologistCmd = fixture.PrincipalCommands<Radiologist>().DeleteCommand(ANY_ROWS);
            var radiologistDeletions = fixture.DeletionsFor(radiologistCmd);
            var onCallCmd = fixture.PrincipalCommands<Radiologist.OnCallEscalation>().DeleteCommand(ANY_ROWS);
            var onCallDeletions = fixture.DeletionsFor(onCallCmd);

            // Assert
            radiologistCmd.Connection.Should().Be(fixture.Connection);
            radiologistCmd.Transaction.Should().Be(fixture.Transaction);
            onCallCmd.Connection.Should().Be(fixture.Connection);
            onCallCmd.Transaction.Should().Be(fixture.Transaction);
            radiologistDeletions.Should().HaveCount(3);
            radiologistDeletions.Should().ContainRow(radiologist0.MedicalID);
            radiologistDeletions.Should().ContainRow(radiologist1.MedicalID);
            radiologistDeletions.Should().ContainRow(radiologist2.MedicalID);
            onCallDeletions.Should().ContainRow(radiologist0.Superior.Key);
            fixture.ShouldBeOrdered(onCallCmd, radiologistCmd);
            fixture.Transaction.Received(1).Commit();
        }

        [TestMethod] public void TransactionRolledBack() {
            // Arrange
            var gazebo = new Gazebo() {
                GazeboID = Guid.NewGuid(),
                GeneralShape = "octagonal",
                MaxCapacity = 37,
                IsTented = false
            };
            var fixture = new TestFixture(typeof(Gazebo)).WithCommitError();

            // Act
            var action = () => fixture.Transactor.Insert(new object[] { gazebo });

            // Assert
            action.Should().ThrowExactly<InvalidOperationException>();
            fixture.Transaction.Received(1).Commit();
            fixture.Transaction.Received(1).Rollback();
        }

        [TestMethod] public void RollbackFails() {
            // Arrange
            var moai = new Moai() {
                Site = "Ahu Tongariki",
                Number = 39,
                Height = 384.2,
                EyesMaterial = "obsidian",
                HasPukao = false
            };
            var fixture = new TestFixture(typeof(Moai)).WithRollbackError();

            // Act
            var action = () => fixture.Transactor.Delete(new object[] { moai });

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
