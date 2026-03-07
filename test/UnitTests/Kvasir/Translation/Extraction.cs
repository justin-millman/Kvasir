using FluentAssertions;
using Kvasir.Core;
using Kvasir.Relations;
using Kvasir.Translation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

using static UT.Kvasir.Translation.Globals;
using static UT.Kvasir.Translation.DataExtraction;
using static UT.Kvasir.Translation.TestConverters;
using static UT.Kvasir.Translation.TestLocalizations;

namespace UT.Kvasir.Translation {
    [TestClass, TestCategory("Extraction")]
    public class DataExtractionTests {
        [TestMethod] public void NonNullPublicInstanceScalars() {
            // Arrange
            var morgue = new Morgue() {
                Name = "Central Chicago Morgue",
                ChiefMedicalExaminer = "Dr. Andrew Q. McDaroida",
                Capacity = 60000,
                Budget = 8000000,
                FederalGrade = 'B',
                AvailableServices = Morgue.Service.Autopsy | Morgue.Service.Identification | Morgue.Service.Cremation,
                GovernmentRun = true
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(Morgue)];
            var data = translation.Principal.Extractor.ExtractFrom(morgue);
            var pk = translation.Principal.KeyExtractor.ExtractFrom(morgue);

            // Assert
            data.Should().HaveCount(7);
            data[0].Datum.Should().Be(morgue.Name);
            data[1].Datum.Should().Be(morgue.ChiefMedicalExaminer);
            data[2].Datum.Should().Be(morgue.Capacity);
            data[3].Datum.Should().Be(morgue.Budget);
            data[4].Datum.Should().Be(morgue.FederalGrade);
            data[5].Datum.Should().Be(ConversionOf(morgue.AvailableServices));
            data[6].Datum.Should().Be(morgue.GovernmentRun);
            pk.Should().HaveCount(1);
            pk[0].Datum.Should().Be(morgue.Name);
        }

        [TestMethod] public void NonNullPublicStaticScalars() {
            // Arrange
            var interpreter = new PythonInterpreter() {
                ProgramID = Guid.NewGuid(),
                Path = "/usr/bin/python",
                InstalledOn = new DateTime(2022, 3, 17)
            };
            PythonInterpreter.MinVersion = 3.5;
            PythonInterpreter.MaxVersion = 3.8;
            PythonInterpreter.BackEndLanguage = PythonInterpreter.Language.CPP;

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(PythonInterpreter)];
            var data = translation.Principal.Extractor.ExtractFrom(interpreter);
            var pk = translation.Principal.KeyExtractor.ExtractFrom(interpreter);

            // Assert
            data.Should().HaveCount(6);
            data[0].Datum.Should().Be(interpreter.ProgramID);
            data[1].Datum.Should().Be(interpreter.Path);
            data[2].Datum.Should().Be(interpreter.InstalledOn);
            data[3].Datum.Should().Be(PythonInterpreter.MinVersion);
            data[4].Datum.Should().Be(PythonInterpreter.MaxVersion);
            data[5].Datum.Should().Be(ConversionOf(PythonInterpreter.BackEndLanguage));
            pk.Should().HaveCount(1);
            pk[0].Datum.Should().Be(interpreter.ProgramID);
        }

        [TestMethod] public void NonNullNonPublicInstanceScalars() {
            // Arrange
            var ship = new PirateShip() {
                ID = Guid.NewGuid(),
                ShipName = "Queen Anne's Revenge",
                Captain = "Blackbeard",
                Style = PirateShip.ShipKind.Frigate,
                CarriedSlaves = false
            };
            ship.SetLength(103);
            ship.SetNumCannons(30);

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(PirateShip)];
            var data = translation.Principal.Extractor.ExtractFrom(ship);
            var pk = translation.Principal.KeyExtractor.ExtractFrom(ship);

            // Assert
            data.Should().HaveCount(7);
            data[0].Datum.Should().Be(ship.ID);
            data[1].Datum.Should().Be(ship.ShipName);
            data[2].Datum.Should().Be(ship.Captain);
            data[3].Datum.Should().Be(ship.GetLength());
            data[4].Datum.Should().Be(ship.GetNumCannons());
            data[5].Datum.Should().Be(ConversionOf(ship.Style));
            data[6].Datum.Should().Be(ship.CarriedSlaves);
            pk.Should().HaveCount(1);
            pk[0].Datum.Should().Be(ship.ID);
        }

        [TestMethod] public void NonNullNonPublicStaticScalars() {
            // Arrange
            var enzyme = new Enzyme() {
                EnzymeCommissionNumber = "3.2.1.108",
                CommonName = "Lactase"
            };
            Enzyme.SetIsEnzyme(true);
            Enzyme.SetNumEnzymesTotal(75000);
            Enzyme.Regulator = "National Academy of Human Enzymes";
            Enzyme.FirstDiscovered = new DateTime(1906, 7, 22);

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(Enzyme)];
            var data = translation.Principal.Extractor.ExtractFrom(enzyme);
            var pk = translation.Principal.KeyExtractor.ExtractFrom(enzyme);

            // Assert
            data.Should().HaveCount(6);
            data[0].Datum.Should().Be(enzyme.EnzymeCommissionNumber);
            data[1].Datum.Should().Be(enzyme.CommonName);
            data[2].Datum.Should().Be(Enzyme.GetIsEnzme());
            data[3].Datum.Should().Be(Enzyme.GetNumEnzymesTotal());
            data[4].Datum.Should().Be(Enzyme.Regulator);
            data[5].Datum.Should().Be(Enzyme.FirstDiscovered);
            pk.Should().HaveCount(1);
            pk[0].Datum.Should().Be(enzyme.EnzymeCommissionNumber);
        }

        [TestMethod] public void NullScalars() {
            // Arrange
            var ode = new Ode() {
                Title = "Ode on a Grecian Urn",
                Author = "John Keats",
                Lines = 50,
                WordCount = 373,
                Publication = null,
                Collection = "Annals of the Fine Arts for 1819",
                Style = null
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(Ode)];
            var data = translation.Principal.Extractor.ExtractFrom(ode);
            var pk = translation.Principal.KeyExtractor.ExtractFrom(ode);

            // Assert
            data.Should().HaveCount(7);
            data[0].Datum.Should().Be(ode.Title);
            data[1].Datum.Should().Be(ode.Author);
            data[2].Datum.Should().Be(ode.Lines);
            data[3].Datum.Should().Be(ode.WordCount);
            data[4].Datum.Should().Be(DBNull.Value);
            data[5].Datum.Should().Be(ode.Collection);
            data[6].Datum.Should().Be(DBNull.Value);
            pk.Should().HaveCount(2);
            data[0].Datum.Should().Be(ode.Title);
            data[1].Datum.Should().Be(ode.Author);
        }

        [TestMethod] public void ExplicitInterfaceImplementation() {
            // Arrange
            var tlatoani = new Tlatoani() {
                ID = Guid.NewGuid(),
                Death = new DateTime(1520, 6, 30),
                EncounteredConquistadors = true,
                CoronationYear = 1502
            };
            (tlatoani as IWorldLeader).Name = "Montezuma II";

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(Tlatoani)];
            var data = translation.Principal.Extractor.ExtractFrom(tlatoani);

            // Assert
            data.Should().HaveCount(6);
            data[0].Datum.Should().Be(tlatoani.ID);
            data[1].Datum.Should().Be((tlatoani as IWorldLeader).Name);
            data[2].Datum.Should().Be((tlatoani as IWorldLeader).Polity);
            data[3].Datum.Should().Be(tlatoani.Death);
            data[4].Datum.Should().Be(tlatoani.EncounteredConquistadors);
            data[5].Datum.Should().Be(tlatoani.CoronationYear);
        }

        [TestMethod] public void VirtualOverride() {
            // Arrange
            var quarter = new StateQuarter() {
                State = "Louisiana",
                Denomination = 0.25,
                Year = 2002,
                Engraver = "John Mercanti",
                Mintage = 764204000
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(StateQuarter)];
            var data = translation.Principal.Extractor.ExtractFrom(quarter);

            // Assert
            data.Should().HaveCount(5);
            data[0].Datum.Should().Be(quarter.State);
            data[1].Datum.Should().Be(quarter.Denomination);
            data[2].Datum.Should().Be(quarter.Year);
            data[3].Datum.Should().Be(quarter.Engraver);
            data[4].Datum.Should().Be(quarter.Mintage);
        }

        [TestMethod] public void Hiding() {
            // Arrange
            var aurora = new Aurora() {
                AuroraID = Guid.NewGuid(),
                Name = "Aurora Borealis",
                AKA = "Northern Lights",
                Intensity = 173.912884f
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(Aurora)];
            var data = translation.Principal.Extractor.ExtractFrom(aurora);

            // Assert
            data.Should().HaveCount(4);
            data[0].Datum.Should().Be(aurora.AuroraID);
            data[1].Datum.Should().Be(aurora.Name);
            data[2].Datum.Should().Be(aurora.AKA);
            data[3].Datum.Should().Be(aurora.Intensity);
        }

        [TestMethod] public void PreDefinedEntity() {
            // Arrange
            var numeral = RomanNumeral.D;

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(RomanNumeral)];
            var data = translation.Principal.Extractor.ExtractFrom(numeral);

            // Assert
            data.Should().HaveCount(2);
            data[0].Datum.Should().Be(numeral.Numeral);
            data[1].Datum.Should().Be(numeral.Value);
        }

        [TestMethod] public void ScalarDataConversion() {
            // Arrange
            var underworld = new Underworld() {
                Name = "Mictlān",
                Civilization = "Aztec Empire",
                Lord = "Mictlāntēcutli",
                ForMortals = true,
                GoogleResults = 1980000
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(Underworld)];
            var data = translation.Principal.Extractor.ExtractFrom(underworld);

            // Act
            data[0].Datum.Should().Be(underworld.Name);
            data[1].Datum.Should().Be(underworld.Civilization);
            data[2].Datum.Should().Be(underworld.Lord);
            data[3].Datum.Should().Be(new Invert().Convert(underworld.ForMortals));
            data[4].Datum.Should().Be(new MakeDate<int>().Convert(underworld.GoogleResults));
        }

        [TestMethod] public void EnumerationNumericConversion() {
            // Arrange
            var maze = new CornMaze() {
                MazeID = Guid.NewGuid(),
                CornType = CornMaze.Corn.Sweet,
                MazeShape = CornMaze.Shape.Animal | CornMaze.Shape.Character | CornMaze.Shape.Person,
                StalkCount = 48913,
                MazeArea = 269,
                SuccessRate = 29.56,
                RecordTime = 196.7
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(CornMaze)];
            var data = translation.Principal.Extractor.ExtractFrom(maze);

            // Assert
            data.Should().HaveCount(7);
            data[0].Datum.Should().Be(maze.MazeID);
            data[1].Datum.Should().Be((byte)maze.CornType);
            data[2].Datum.Should().Be((ulong)maze.MazeShape);
            data[3].Datum.Should().Be(maze.StalkCount);
            data[4].Datum.Should().Be(maze.MazeArea);
            data[5].Datum.Should().Be(maze.SuccessRate);
            data[6].Datum.Should().Be(maze.RecordTime);
        }

        [TestMethod] public void EnumerationToStringConversion() {
            // Arrange
            var racetrack = new MarioKartRacetrack() {
                Name = "Dino Dino Jungle",
                FirstAppearance = "Mario Kart: Double Dash!!",
                Series = MarioKartRacetrack.Cup.Special,
                TrackLength = null,
                AvailableOnline = false
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(MarioKartRacetrack)];
            var data = translation.Principal.Extractor.ExtractFrom(racetrack);

            // Assert
            data.Should().HaveCount(5);
            data[0].Datum.Should().Be(racetrack.Name);
            data[1].Datum.Should().Be(racetrack.FirstAppearance);
            data[2].Datum.Should().Be(ConversionOf(racetrack.Series));
            data[3].Datum.Should().Be(DBNull.Value);
            data[4].Datum.Should().Be(racetrack.AvailableOnline);
        }

        [TestMethod] public void CalculatedScalar() {
            // Arrange
            var lighthouse = new Lighthouse() {
                Name = "Pemaquid Point Lighthouse",
                Location = "Muscongus Bay in Bristol, Maine",
                Height = 11.5,
                FocalLength = 79
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(Lighthouse)];
            var data = translation.Principal.Extractor.ExtractFrom(lighthouse);

            // Assert
            data.Should().HaveCount(5);
            data[0].Datum.Should().Be(lighthouse.Name);
            data[1].Datum.Should().Be(lighthouse.Location);
            data[2].Datum.Should().Be(lighthouse.Height);
            data[3].Datum.Should().Be(lighthouse.FocalLength);
            data[4].Datum.Should().Be(lighthouse.LighthouseRating);
        }

        [TestMethod] public void NonNullSingleFieldAggregate() {
            // Arrange
            var nucleobase = new Nucleobase {
                Symbol = new Nucleobase.Letter() { Value = 'G' },
                Name = "Guanine",
                ChemicalFormula = "C5H5N5O"
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(Nucleobase)];
            var data = translation.Principal.Extractor.ExtractFrom(nucleobase);

            // Assert
            data.Should().HaveCount(3);
            data[0].Datum.Should().Be(nucleobase.Symbol.Value);
            data[1].Datum.Should().Be(nucleobase.Name);
            data[2].Datum.Should().Be(nucleobase.ChemicalFormula);
        }

        [TestMethod] public void NonNullMultiFieldAggregate() {
            // Arrange
            var legos = new LegoSet() {
                ItemNumber = 75912,
                Title = "Millennium Falcon",
                Catalog = new LegoSet.Listing() {
                    Price = 849.99M,
                    Stars = LegoSet.Rating.FourPointFive,
                    URL = "https://www.lego.com/en-us/product/millennium-falcon-75192",
                    InsiderPoints = 5525,
                    Theme = LegoSet.Series.StarWars
                },
                Pieces = 7541,
                LowerBoundAge = 16
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(LegoSet)];
            var data = translation.Principal.Extractor.ExtractFrom(legos);

            // Assert
            data.Should().HaveCount(9);
            data[0].Datum.Should().Be(legos.ItemNumber);
            data[1].Datum.Should().Be(legos.Title);
            data[2].Datum.Should().Be(legos.Catalog.Price);
            data[3].Datum.Should().Be(ConversionOf(legos.Catalog.Stars));
            data[4].Datum.Should().Be(legos.Catalog.URL);
            data[5].Datum.Should().Be(legos.Catalog.InsiderPoints);
            data[6].Datum.Should().Be(ConversionOf(legos.Catalog.Theme));
            data[7].Datum.Should().Be(legos.Pieces);
            data[8].Datum.Should().Be(legos.LowerBoundAge);
        }

        [TestMethod] public void AggregateWithAllNullNestedFields() {
            // Arrange
            var fight = new SnowballFight() {
                FightID = Guid.NewGuid(),
                KickOff = new DateTime(2023, 12, 7),
                FightStructure = new SnowballFight.Structure() {
                    NumTeams = null,
                    HitsAllowed = null,
                    MaxBallRadius = null
                },
                Length = 120,
                LowTemperature = 14.3
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(SnowballFight)];
            var data = translation.Principal.Extractor.ExtractFrom(fight);

            // Assert
            data.Should().HaveCount(7);
            data[0].Datum.Should().Be(fight.FightID);
            data[1].Datum.Should().Be(fight.KickOff);
            data[2].Datum.Should().Be(DBNull.Value);
            data[3].Datum.Should().Be(DBNull.Value);
            data[4].Datum.Should().Be(DBNull.Value);
            data[5].Datum.Should().Be(fight.Length);
            data[6].Datum.Should().Be(fight.LowTemperature);
        }

        [TestMethod] public void NullSingleFieldAggregate() {
            // Arrange
            var knot = new Knot() {
                Name = "Savoy Knot",
                Shape = null,
                Efficiency = 0.8,
                AshleyBookOfKnotsPage = null
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(Knot)];
            var data = translation.Principal.Extractor.ExtractFrom(knot);

            // Assert
            data.Should().HaveCount(4);
            data[0].Datum.Should().Be(knot.Name);
            data[1].Datum.Should().Be(DBNull.Value);
            data[2].Datum.Should().Be(knot.Efficiency);
            data[3].Datum.Should().Be(DBNull.Value);
        }

        [TestMethod] public void NullMultiFieldAggregate() {
            // Arrange
            var armory = new Armory() {
                Name = "Sergeant Camilo's Principal Weapons Warehouse",
                Decommissioned = false,
                Location = null,
                WeaponsCount = 876182491284,
                Owner = Armory.Level.Vigilante
            };

            // Act
            var translator = new Translator(NO_ENTITIES); ;
            var translation = translator[typeof(Armory)];
            var data = translation.Principal.Extractor.ExtractFrom(armory);

            // Assert
            data.Should().HaveCount(6);
            data[0].Datum.Should().Be(armory.Name);
            data[1].Datum.Should().Be(armory.Decommissioned);
            data[2].Datum.Should().Be(DBNull.Value);
            data[3].Datum.Should().Be(DBNull.Value);
            data[4].Datum.Should().Be(armory.WeaponsCount);
            data[5].Datum.Should().Be(ConversionOf(armory.Owner));
        }

        [TestMethod] public void NestedAggregate() {
            // Arrange
            var question = new MillionaireQuestion() {
                QuestionID = Guid.NewGuid(),
                Category = "European Ruins",
                Question = "The ruins of Urquhart Castle stand on the banks of which loch?",
                Answers = new MillionaireQuestion.Options() {
                    A = new MillionaireQuestion.Option() {
                        Text = "Loch Lomond",
                        FiftyFiftyEliminated = true,
                        AudiencePercentage = 0.08,
                        IsCorrect = false,
                    },
                    B = new MillionaireQuestion.Option() {
                        Text = "Loch Ness",
                        FiftyFiftyEliminated = false,
                        AudiencePercentage = 0.81,
                        IsCorrect = true
                    },
                    C = new MillionaireQuestion.Option() {
                        Text = "Loch Broom",
                        FiftyFiftyEliminated = false,
                        AudiencePercentage = 0.03,
                        IsCorrect = false
                    },
                    D = new MillionaireQuestion.Option() {
                        Text = "Loch Maree",
                        FiftyFiftyEliminated = true,
                        AudiencePercentage = 0.08,
                        IsCorrect = false
                    }
                }
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(MillionaireQuestion)];
            var data = translation.Principal.Extractor.ExtractFrom(question);

            // Assert
            data.Should().HaveCount(19);
            data[0].Datum.Should().Be(question.QuestionID);
            data[1].Datum.Should().Be(question.Category);
            data[2].Datum.Should().Be(question.Question);
            data[3].Datum.Should().Be(question.Answers.A.Text);
            data[4].Datum.Should().Be(question.Answers.A.FiftyFiftyEliminated);
            data[5].Datum.Should().Be(question.Answers.A.AudiencePercentage);
            data[6].Datum.Should().Be(question.Answers.A.IsCorrect);
            data[7].Datum.Should().Be(question.Answers.B.Text);
            data[8].Datum.Should().Be(question.Answers.B.FiftyFiftyEliminated);
            data[9].Datum.Should().Be(question.Answers.B.AudiencePercentage);
            data[10].Datum.Should().Be(question.Answers.B.IsCorrect);
            data[11].Datum.Should().Be(question.Answers.C.Text);
            data[12].Datum.Should().Be(question.Answers.C.FiftyFiftyEliminated);
            data[13].Datum.Should().Be(question.Answers.C.AudiencePercentage);
            data[14].Datum.Should().Be(question.Answers.C.IsCorrect);
            data[15].Datum.Should().Be(question.Answers.D.Text);
            data[16].Datum.Should().Be(question.Answers.D.FiftyFiftyEliminated);
            data[17].Datum.Should().Be(question.Answers.D.AudiencePercentage);
            data[18].Datum.Should().Be(question.Answers.D.IsCorrect);
        }

        [TestMethod] public void AggregateNestedDataConversion() {
            // Arrange
            var game = new GroceryGame() {
                Name = "No Carts Allowed",
                Description = "Contestants must shop for ingredients without their carts, carrying everything by hand",
                FirstAppearance = new GroceryGame.Episode() {
                    Season = 1,
                    Number = 4,
                    Judge1 = "Melissa d'Arabian",
                    Judge2 = "Troy Johnson",
                    Judge3 = "Lorena Garcia"
                },
                NumTimesPlayed = 11
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(GroceryGame)];
            var data = translation.Principal.Extractor.ExtractFrom(game);

            // Assert
            data.Should().HaveCount(8);
            data[0].Datum.Should().Be(game.Name);
            data[1].Datum.Should().Be(game.Description);
            data[2].Datum.Should().Be(new ToInt<byte>().Convert(game.FirstAppearance.Season));
            data[3].Datum.Should().Be(new ToInt<byte>().Convert(game.FirstAppearance.Number));
            data[4].Datum.Should().Be(game.FirstAppearance.Judge1);
            data[5].Datum.Should().Be(game.FirstAppearance.Judge2);
            data[6].Datum.Should().Be(game.FirstAppearance.Judge3);
            data[7].Datum.Should().Be(game.NumTimesPlayed);
        }

        [TestMethod] public void NonNullReferenceSingleFieldPrimaryKey() {
            // Arrange
            var conclave = new PapalConclave() {
                Date = new DateTime(2013, 3, 12),
                Ballots = 5,
                ElectedPope = new PapalConclave.Cardinal() {
                    Name = "Jorge Mario Bergoglio",
                    Country = "Argentina",
                    Age = 76
                },
                NumElectors = 115,
                Dean = new PapalConclave.Cardinal() {
                    Name = "Angelo Sodano",
                    Country = "Italy",
                    Age = 85
                }
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(PapalConclave)];
            var data = translation.Principal.Extractor.ExtractFrom(conclave);

            // Assert
            data.Should().HaveCount(5);
            data[0].Datum.Should().Be(conclave.Date);
            data[1].Datum.Should().Be(conclave.Ballots);
            data[2].Datum.Should().Be(conclave.ElectedPope.Name);
            data[3].Datum.Should().Be(conclave.NumElectors);
            data[4].Datum.Should().Be(conclave.Dean.Name);
        }

        [TestMethod] public void NonNullReferenceMultiFieldPrimaryKey() {
            // Arrange
            var cytonic = new Cytonic() {
                Name = "Doomslug",
                CallSign = null,
                SelfSpecies = new Cytonic.Species() {
                    Grouping = 498,
                    Name = "Taynix",
                    SubNumber = 3,
                    PrimaryIntelligence = false,
                },
                Abilities = Cytonic.Power.Hyperjump,
                Appearances = Cytonic.Book.Skyward | Cytonic.Book.Starsight | Cytonic.Book.Cytonic | Cytonic.Book.Defiant
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(Cytonic)];
            var data = translation.Principal.Extractor.ExtractFrom(cytonic);

            // Assert
            data.Should().HaveCount(6);
            data[0].Datum.Should().Be(cytonic.Name);
            data[1].Datum.Should().Be(DBNull.Value);
            data[2].Datum.Should().Be(cytonic.SelfSpecies.Grouping);
            data[3].Datum.Should().Be(cytonic.SelfSpecies.SubNumber);
            data[4].Datum.Should().Be(ConversionOf(cytonic.Abilities));
            data[5].Datum.Should().Be(ConversionOf(cytonic.Appearances));
        }

        [TestMethod] public void NullReferenceSingleFieldPrimaryKey() {
            // Arrange
            var soapOpera = new SoapOpera() {
                Title = "Days of Our Lives",
                IsStillAiring = true,
                Premiere = new DateTime(1965, 11, 8),
                NumSeasons = 59,
                NumEpisodes = 14430,
                NumCastMembers = 69,
                OwningNetwork = null,
                IsTelenovela = false
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(SoapOpera)];
            var data = translation.Principal.Extractor.ExtractFrom(soapOpera);

            // Assert
            data.Should().HaveCount(8);
            data[0].Datum.Should().Be(soapOpera.Title);
            data[1].Datum.Should().Be(soapOpera.IsStillAiring);
            data[2].Datum.Should().Be(soapOpera.Premiere);
            data[3].Datum.Should().Be(soapOpera.NumSeasons);
            data[4].Datum.Should().Be(soapOpera.NumEpisodes);
            data[5].Datum.Should().Be(soapOpera.NumCastMembers);
            data[6].Datum.Should().Be(DBNull.Value);
            data[7].Datum.Should().Be(soapOpera.IsTelenovela);
        }

        [TestMethod] public void NullReferenceMultiFieldPrimaryKey() {
            // Arrange
            var library = new Library() {
                LibraryID = Guid.NewGuid(),
                NumBooks = 716284,
                HeadLibrarian = null,
                Endowment = 7000000,
                Branches = 40
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(Library)];
            var data = translation.Principal.Extractor.ExtractFrom(library);

            // Assert
            data.Should().HaveCount(6);
            data[0].Datum.Should().Be(library.LibraryID);
            data[1].Datum.Should().Be(library.NumBooks);
            data[2].Datum.Should().Be(DBNull.Value);
            data[3].Datum.Should().Be(DBNull.Value);
            data[4].Datum.Should().Be(library.Endowment);
            data[5].Datum.Should().Be(library.Branches);
        }

        [TestMethod] public void ReferenceNestedDataConversion() {
            // Arrange
            var match = new CurlingMatch() {
                ID = Guid.NewGuid(),
                TeamA = new CurlingMatch.OlympicOrganization() {
                    Code = "ita",
                    Country = "Italy",
                    Recognized = 1915
                },
                TeamB = new CurlingMatch.OlympicOrganization() {
                    Code = "",
                    Country = "Norway",
                    Recognized = 1861
                },
                ScoreA = 8,
                ScoreB = 5,
                Date = new DateTime(2022, 2, 8),
                Olympiad = 24,
                HammerForA = true
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(CurlingMatch)];
            var data = translation.Principal.Extractor.ExtractFrom(match);

            // Assert
            data.Should().HaveCount(8);
            data[0].Datum.Should().Be(match.ID);
            data[1].Datum.Should().Be(new AllCaps().Convert(match.TeamA.Code));
            data[2].Datum.Should().Be(new AllCaps().Convert(match.TeamB.Code));
            data[3].Datum.Should().Be(match.ScoreA);
            data[4].Datum.Should().Be(match.ScoreB);
            data[5].Datum.Should().Be(match.Date);
            data[6].Datum.Should().Be(match.Olympiad);
            data[7].Datum.Should().Be(match.HammerForA);
        }

        [TestMethod] public void NonNullRelationWithZeroElements() {
            // Arrange
            var pretzel = new Pretzel() {
                PretzelID = Guid.NewGuid(),
                Name = "Plain Pretzel",
                Toppings = new(),
                RetailPrice = 2.75M,
                DoughSource = "Flour"
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(Pretzel)];
            var data = translation.Relations[0].Extractor.ExtractFrom(pretzel);

            // Assert
            data.Insertions.Should().BeEmpty();
            data.Modifications.Should().BeEmpty();
            data.Deletions.Should().BeEmpty();
        }

        [TestMethod] public void NonNullSequenceRelationWithOnlyNewElements() {
            // Arrange
            var teppanyaki = new Teppanyaki() {
                GrillID = Guid.NewGuid(),
                GrillSurfaceArea = 88.5,
                AuthorizedChefs = new() {
                    "Daisuke Orinaka",
                    "Kaidon Hotosata",
                    "Hideki Iwanatsuo",
                },
                SupportedFoods = new() {
                    "Chicken",
                    "Beef",
                    "Onion",
                    "Egg",
                    "Shrimp",
                    "Daikon",
                    "Fried Rice"
                },
                MaxTemperature = 140,
                Restaurant = null,
                IsHibachi = false
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(Teppanyaki)];
            var setData = translation.Relations[0].Extractor.ExtractFrom(teppanyaki);
            var listData = translation.Relations[1].Extractor.ExtractFrom(teppanyaki);

            // Assert
            listData.Insertions.Should().HaveCount(7);
            listData.Insertions.Should().ContainRow("Chicken");
            listData.Insertions.Should().ContainRow("Beef");
            listData.Insertions.Should().ContainRow("Onion");
            listData.Insertions.Should().ContainRow("Egg");
            listData.Insertions.Should().ContainRow("Shrimp");
            listData.Insertions.Should().ContainRow("Daikon");
            listData.Insertions.Should().ContainRow("Fried Rice");
            listData.Modifications.Should().BeEmpty();
            listData.Deletions.Should().BeEmpty();
            setData.Insertions.Should().HaveCount(3);
            setData.Insertions.Should().ContainRow("Daisuke Orinaka");
            setData.Insertions.Should().ContainRow("Kaidon Hotosata");
            setData.Insertions.Should().ContainRow("Hideki Iwanatsuo");
            setData.Modifications.Should().BeEmpty();
            setData.Deletions.Should().BeEmpty();
        }

        [TestMethod] public void NonNullMapRelationWithOnlyNewElements() {
            // Arrange
            var spellingBee = new SpellingBee() {
                Year = 2023,
                NumRounds = 13,
                Champion = "Dev Shah",
                EliminationWords = new() {
                    { 46, "chthonic" },
                    { 119, "querken" },
                    { 6, "pataca" },
                    { 122, "pharetone" }
                }
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(SpellingBee)];
            var data = translation.Relations[0].Extractor.ExtractFrom(spellingBee);

            // Assert
            data.Insertions.Should().HaveCount(4);
            data.Insertions.Should().ContainRow(46U, "chthonic");
            data.Insertions.Should().ContainRow(119U, "querken");
            data.Insertions.Should().ContainRow(6U, "pataca");
            data.Insertions.Should().ContainRow(122U, "pharetone");
            data.Modifications.Should().BeEmpty();
            data.Deletions.Should().BeEmpty();
        }

        [TestMethod] public void NonNullOrderedListRelationWithOnlyNewElements() {
            // Arrange
            var troupe = new ImprovTroupe() {
                Name = "Players of Locura",
                Created = new DateTime(2015, 9, 19),
                NumShows = 496,
                Lineup = {
                    "Amanda Corningsweather",
                    "Randy Cappaco",
                    "Edith Sumak",
                    "Nicole d'Francia",
                    "Aaron Goliin",
                    "Harrison B. Tarmalonz"
                },
                URL = "https://locuraplayers.org/"
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(ImprovTroupe)];
            var data = translation.Relations[0].Extractor.ExtractFrom(troupe);

            // Assert
            data.Insertions.Should().HaveCount(6);
            data.Insertions.Should().ContainRow(0U, "Amanda Corningsweather");
            data.Insertions.Should().ContainRow(1U, "Randy Cappaco");
            data.Insertions.Should().ContainRow(2U, "Edith Sumak");
            data.Insertions.Should().ContainRow(3U, "Nicole d'Francia");
            data.Insertions.Should().ContainRow(4U, "Aaron Goliin");
            data.Insertions.Should().ContainRow(5U, "Harrison B. Tarmalonz");
            data.Modifications.Should().BeEmpty();
            data.Deletions.Should().BeEmpty();
        }

        [TestMethod] public void NonNullRelationWithOnlySavedElements() {
            // Arrange
            var tip = new NedsDeclassifiedTip() {
                ID = Guid.NewGuid(),
                Category = "Photo Day",
                Tip = "Bring a hair brush to fix your hair before the picture",
                For = {
                    "Jennifer Mosely",
                    "Suzie Crabgrass",
                    "Bitsy Johnson"
                }
            };
            (tip.For as IRelation).Canonicalize();

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(NedsDeclassifiedTip)];
            var data = translation.Relations[0].Extractor.ExtractFrom(tip);

            // Assert
            data.Insertions.Should().BeEmpty();
            data.Modifications.Should().BeEmpty();
            data.Deletions.Should().BeEmpty();
        }

        [TestMethod] public void NonNullRelationWithAtLeastOneModifiedElement() {
            // Arrange
            var territory = new PendragonTerritory() {
                Name = "Zadaa",
                Travellers = {
                    "Osa",
                    "Loor"
                },
                FirstAppearance = "The Lost City of Faar",
                Capital = "Xhaxhu"
            };
            (territory.Travellers as IRelation).Canonicalize();
            territory.Travellers[0] = "Loor";
            territory.Travellers[1] = "Osa";

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(PendragonTerritory)];
            var data = translation.Relations[0].Extractor.ExtractFrom(territory);

            // Assert
            data.Insertions.Should().BeEmpty();
            data.Modifications.Should().HaveCount(2);
            data.Modifications.Should().ContainRow(0U, "Loor");
            data.Modifications.Should().ContainRow(1U, "Osa");
        }

        [TestMethod] public void NonNullRelationWithAtLeastOneDeletedElement() {
            // Arrange
            var caucus = new IowaCaucus() {
                Year = 2008,
                Party = IowaCaucus.PoliticalParty.Democratic,
                Date = new DateTime(2008, 1, 3),
                DelegatesEarned = {
                    { "Barack Obama", 16.0 },
                    { "John Edwards", 14.0 },
                    { "Hillary Clinton", 15.0 },
                    { "Bill Richardson", 0.0 },
                    { "Joe Biden", 0.0 },
                    { "uncommitted", 0.0 },
                    { "Christopher Dodd", 0.0 }
                },
                Bellwether = true
            };
            (caucus.DelegatesEarned as IRelation).Canonicalize();
            caucus.DelegatesEarned.Remove("uncommitted");

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(IowaCaucus)];
            var data = translation.Relations[0].Extractor.ExtractFrom(caucus);

            // Assert
            data.Insertions.Should().BeEmpty();
            data.Modifications.Should().BeEmpty();
            data.Deletions.Should().HaveCount(1);
            data.Deletions.Should().ContainRow("uncommitted", 0.0);
        }

        [TestMethod] public void NullRelation() {
            // Arrange
            var existentialist = new Existentialist() {
                Name = "Jean-Paul Sartre",
                DoctoralTheses = null!,
                DateOfBirth = new DateTime(1905, 6, 21),
                DateOfDeath = new DateTime(1980, 4, 15),
                ExistentialSchool = "Phenomenlogy"
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(Existentialist)];
            var data = translation.Relations[0].Extractor.ExtractFrom(existentialist);

            // Assert
            data.Insertions.Should().BeEmpty();
            data.Modifications.Should().BeEmpty();
            data.Deletions.Should().BeEmpty();
        }

        [TestMethod] public void RelationNestedAggregate() {
            // Arrange
            var boon = new OlympianBoon() {
                BoonName = "Curse of Pain",
                Benefactor = OlympianBoon.Deity.Ares,
                AbilityAffected = OlympianBoon.Ability.Special,
                Progressions = new() {
                    new OlympianBoon.Benefit() { ParameterName = "Damage", ParameterValue = 60 },
                    new OlympianBoon.Benefit() { ParameterName = "Damage", ParameterValue = 80 },
                    new OlympianBoon.Benefit() { ParameterName = "Damage", ParameterValue = 100 },
                    new OlympianBoon.Benefit() { ParameterName = "Damage", ParameterValue = 120 }
                },
                Likelihood = 0.2
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(OlympianBoon)];
            var data = translation.Relations[0].Extractor.ExtractFrom(boon);

            // Assert
            data.Insertions.Should().HaveCount(4);
            data.Insertions.Should().ContainRow(0U, "Damage", 60.0);
            data.Insertions.Should().ContainRow(1U, "Damage", 80.0);
            data.Insertions.Should().ContainRow(2U, "Damage", 100.0);
            data.Insertions.Should().ContainRow(3U, "Damage", 120.0);
            data.Modifications.Should().BeEmpty();
            data.Deletions.Should().BeEmpty();
        }

        [TestMethod] public void RelationNestedReference() {
            // Arrange
            var impeachment = new Impeachment() {
                Official = "Samuel Chase",
                Position = "Associate Justice of the U.S. Supreme Court",
                Commenced = new DateTime(1804, 3, 12),
                Counts = new() {
                    new Impeachment.Count() {
                        ID = Guid.NewGuid(),
                        Claim = new Impeachment.Charge() {
                            Claim = "Improper Conduct during the Trial of John Fries",
                            Severity = Impeachment.Charge.Category.Misdemeanor
                        },
                        Guilty = false
                    },
                    new Impeachment.Count() {
                        ID = Guid.NewGuid(),
                        Claim = new Impeachment.Charge() {
                            Claim = "Improper Conduct during the Trial of James T. Callendar",
                            Severity = Impeachment.Charge.Category.Misdemeanor
                        },
                        Guilty = false
                    },
                    new Impeachment.Count() {
                        ID = Guid.NewGuid(),
                        Claim = new Impeachment.Charge() {
                            Claim = "Conduct Unbecoming in front of a Baltimore Grand Jury",
                            Severity = Impeachment.Charge.Category.Misdemeanor
                        },
                        Guilty = false
                    }
                }
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(Impeachment)];
            var data = translation.Relations[0].Extractor.ExtractFrom(impeachment);

            // Assert
            data.Insertions.Should().HaveCount(3);
            data.Insertions.Should().ContainRow(impeachment.Counts[0].ID, false);
            data.Insertions.Should().ContainRow(impeachment.Counts[1].ID, false);
            data.Insertions.Should().ContainRow(impeachment.Counts[2].ID, false);
            data.Modifications.Should().BeEmpty();
            data.Deletions.Should().BeEmpty();
        }

        [TestMethod] public void RelationReferencesOwningEntity() {
            // Arrange
            var god = new MaoriGod() {
                Name = "Tangaroa",
                Domain = "Sea",
                Family = new(),
                IsAtua = true,
                EncounteredMaui = true
            };
            god.Family[god] = MaoriGod.Relation.Self;

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(MaoriGod)];
            var data = translation.Relations[0].Extractor.ExtractFrom(god);

            // Assert
            data.Insertions.Should().HaveCount(1);
            data.Insertions.Should().ContainRow(god.Name, ConversionOf(MaoriGod.Relation.Self));
            data.Modifications.Should().BeEmpty();
            data.Deletions.Should().BeEmpty();
        }

        [TestMethod] public void AggregateNestedRelation() {
            // Arrange
            var center = new DataCenter() {
                ID = Guid.NewGuid(),
                LiquidCooled = true,
                Statistics = new DataCenter.Stats() {
                    CO2 = 17895.233,
                    Electricity = 380,
                    Dimensions = new() {
                        { "Length", 800 },
                        { "Height", 300 },
                        { "Width", 475 },
                    },
                    Machines = new DataCenter.Computers() {
                        NumCabinets = 50,
                        RacksPerCabinet = 25,
                        Brands = new() {
                            "Apple",
                            "Microsoft",
                            "Lenovo",
                            "Dell"
                        }
                    }
                }
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(DataCenter)];
            var data0 = translation.Relations[0].Extractor.ExtractFrom(center);
            var data1 = translation.Relations[1].Extractor.ExtractFrom(center);

            // Assert
            data0.Insertions.Should().HaveCount(3);
            data0.Insertions.Should().ContainRow("Length", 800.0);
            data0.Insertions.Should().ContainRow("Height", 300.0);
            data0.Insertions.Should().ContainRow("Width", 475.0);
            data0.Modifications.Should().BeEmpty();
            data0.Deletions.Should().BeEmpty();
            data1.Insertions.Should().HaveCount(4);
            data1.Insertions.Should().ContainRow("Apple");
            data1.Insertions.Should().ContainRow("Microsoft");
            data1.Insertions.Should().ContainRow("Lenovo");
            data1.Insertions.Should().ContainRow("Dell");
            data1.Modifications.Should().BeEmpty();
            data1.Deletions.Should().BeEmpty();
        }

        [TestMethod] public void CalculatedRelation() {
            // Arrange
            var chameleon = new Chameleon() {
                ReptileID = Guid.NewGuid(),
                TimesChangedColor = 8719287581,
                Genus = "Furcifer",
                Species = "Pardalis"
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(Chameleon)];
            var data = translation.Relations[0].Extractor.ExtractFrom(chameleon);

            // Assert
            data.Insertions.Should().HaveCount(2);
            data.Insertions.Should().ContainRow(0U, chameleon.Eyes[0].ConeDensity, chameleon.Eyes[0].RodDensity, chameleon.Eyes[0].VisionRange);
            data.Insertions.Should().ContainRow(1U, chameleon.Eyes[1].ConeDensity, chameleon.Eyes[1].RodDensity, chameleon.Eyes[1].VisionRange);
            data.Modifications.Should().BeEmpty();
            data.Deletions.Should().BeEmpty();
        }
        
        [TestMethod] public void RelationNestedDataConversion() {
            // Arrange
            var horoscope = new Horoscope() {
                Sign = Horoscope.Zodiac.Gemini,
                Readings = new() {
                    {
                        new DateTime(2024, 2, 3),
                        new Horoscope.Listing() {
                            Prediction = "[gobbledy gook]",
                            Sex = 'q',
                            Hustle = '!',
                            Vibe = 'B',
                            Success = '9'
                        }
                    }
                },
                RangeLower = new DateTime(2024, 5, 21),
                RangeUpper = new DateTime(2024, 6, 20)
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(Horoscope)];
            var data = translation.Relations[0].Extractor.ExtractFrom(horoscope);

            // Assert
            data.Insertions.Should().HaveCount(1);
            data.Insertions.Should().ContainRow(new DateTime(2024, 2, 3), "[gobbledy gook]", (int)'q', (int)'!', (int)'B', (int)'9');
            data.Modifications.Should().BeEmpty();
            data.Deletions.Should().BeEmpty();
        }

        [TestMethod] public void NonNullLocalizationPropertyWithNonEnumerationKey() {
            // Arrange
            var orogene = new Orogene() {
                FulcrumName = "Alabaster",
                BirthName = null,
                BirthComm = null,
                BirthDate = new LocalizedDate(Guid.NewGuid()),
                Rings = 10,
                Appearances = Orogene.Book.FifthSeason | Orogene.Book.ObeliskGate | Orogene.Book.StoneSky,
                AtNodeStation = false
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(Orogene)];
            var data = translation.Principal.Extractor.ExtractFrom(orogene);

            // Assert
            data.Should().HaveCount(7);
            data[0].Datum.Should().Be(orogene.FulcrumName);
            data[1].Datum.Should().Be(DBNull.Value);
            data[2].Datum.Should().Be(DBNull.Value);
            data[3].Datum.Should().Be(orogene.BirthDate.Key);
            data[4].Datum.Should().Be(orogene.Rings);
            data[5].Datum.Should().Be(ConversionOf(orogene.Appearances));
            data[6].Datum.Should().Be(orogene.AtNodeStation);
        }

        [TestMethod] public void NonNullLocalizationPropertyWithEnumerationKey() {
            // Arrange
            var couple = new DWTSCouple() {
                Season = 22,
                Professional = "Peta Murgatroyd",
                Celebrity = "Nyle DiMarco",
                FinishingPlace = 1,
                BestScore = new DWTSCouple.LocalizedScore(DWTSCouple.Dance.VienneseWaltz)
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(DWTSCouple)];
            var data = translation.Principal.Extractor.ExtractFrom(couple);

            // Assert
            data.Should().HaveCount(5);
            data[0].Datum.Should().Be(couple.Season);
            data[1].Datum.Should().Be(couple.Professional);
            data[2].Datum.Should().Be(couple.Celebrity);
            data[3].Datum.Should().Be(couple.FinishingPlace);
            data[4].Datum.Should().Be(ConversionOf(couple.BestScore.Key));
        }

        [TestMethod] public void NullLocalizationProperty() {
            // Arrange
            var arcanum = new Arcanum() {
                PrimalSource = Arcanum.Domain.Sun,
                Season = 3,
                Elves = "Sunfire Elves",
                Archdragon = "Sol Regem",
                NumKnownSpells = 33,
                XadianRanking = null
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(Arcanum)];
            var data = translation.Principal.Extractor.ExtractFrom(arcanum);

            // Assert
            data.Should().HaveCount(6);
            data[0].Datum.Should().Be(ConversionOf(arcanum.PrimalSource));
            data[1].Datum.Should().Be(arcanum.Season);
            data[2].Datum.Should().Be(arcanum.Elves);
            data[3].Datum.Should().Be(arcanum.Archdragon);
            data[4].Datum.Should().Be(arcanum.NumKnownSpells);
            data[5].Datum.Should().Be(DBNull.Value);
        }

        [TestMethod] public void AggregateNestedLocalizationProperty() {
            // Arrange
            var command = new CombatantCommand() {
                Name = new CombatantCommand.Naming("CENTCOM", new LocalizedText("CENTCOM_NAME_LOC")),
                Founded = new DateTime(1983, 1, 1),
                HQ = "MacDill Air Force Base (FL)",
                Commander = "Admiral Charles B. Cooper II",
                NuclearCapable = true
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(CombatantCommand)];
            var data = translation.Principal.Extractor.ExtractFrom(command);

            // Assert
            data.Should().HaveCount(6);
            data[0].Datum.Should().Be(command.Name.Acronym);
            data[1].Datum.Should().Be(command.Name.Full.Key);
            data[2].Datum.Should().Be(command.Founded);
            data[3].Datum.Should().Be(command.HQ);
            data[4].Datum.Should().Be(command.Commander);
            data[5].Datum.Should().Be(command.NuclearCapable);
        }

        [TestMethod] public void CalculatedLocalizationProperty() {
            // Arrange
            var sapa = new SapaInca() {
                Name = "Pachacuti",
                Index = 9,
                ReignStart = new LocalizedDate(Guid.NewGuid()),
                ReignDays = 12053,
                TocapuMotif = '\0',
                WasConquered = false
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(SapaInca)];
            var data = translation.Principal.Extractor.ExtractFrom(sapa);

            // Assert
            data.Should().HaveCount(6);
            data[0].Datum.Should().Be(sapa.Name);
            data[1].Datum.Should().Be(sapa.Index);
            data[2].Datum.Should().Be(sapa.ReignStart.Key);
            data[3].Datum.Should().Be(sapa.ReignDays);
            data[4].Datum.Should().Be(sapa.TocapuMotif);
            data[5].Datum.Should().Be(sapa.WasConquered);
        }

        [TestMethod] public void LocalizationListElement() {
            // Arrange
            var dragonlord = new Dragonlord() {
                HumanSoul = "Linden",
                DragonSoul = "Rathan",
                Age = 600,
                HasSoulTwin = true,
                Marking = "scar",
                Councils = new RelationList<LocalizedText>() {
                    new LocalizedText("GREAT_COUNCIL_LOC"),
                    new LocalizedText("CASSORIN_COUNCIL_LOC"),
                    new LocalizedText("COUNCIL_OF_THE_TRUEDRAGONS_LOC"),
                    new LocalizedText("JEHANGLAN_PHOENIX_COUNCIL_LOC")
                }
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(Dragonlord)];
            var data = translation.Relations[0].Extractor.ExtractFrom(dragonlord);

            // Assert
            data.Insertions.Should().HaveCount(4);
            data.Insertions.Should().ContainRow(dragonlord.Councils[0].Key);
            data.Insertions.Should().ContainRow(dragonlord.Councils[1].Key);
            data.Insertions.Should().ContainRow(dragonlord.Councils[2].Key);
            data.Insertions.Should().ContainRow(dragonlord.Councils[3].Key);
            data.Modifications.Should().BeEmpty();
            data.Deletions.Should().BeEmpty();
        }

        [TestMethod] public void LocalizationSetElement() {
            // Arrange
            var showstopper = new Showstopper() {
                Series = 16,
                Episode = 5,
                Challenge = "Chocolate Fondue Display",
                WinningBaker = "Aaron Mountford-Myles",
                WorstBaker = "Nadia Mercuri",
                Ingredients = new RelationSet<LocalizedText>() {
                    new LocalizedText("CHOCOLATE_LOC"),
                    new LocalizedText("RASPBERRY_LOC"),
                    new LocalizedText("LEMON_LOC"),
                    new LocalizedText("ALMONDS_LOC")
                }
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(Showstopper)];
            var data = translation.Relations[0].Extractor.ExtractFrom(showstopper);

            // Assert
            data.Insertions.Should().HaveCount(4);
            data.Insertions.Should().ContainRow("CHOCOLATE_LOC");
            data.Insertions.Should().ContainRow("RASPBERRY_LOC");
            data.Insertions.Should().ContainRow("LEMON_LOC");
            data.Insertions.Should().ContainRow("ALMONDS_LOC");
            data.Modifications.Should().BeEmpty();
            data.Deletions.Should().BeEmpty();
        }

        [TestMethod] public void LocalizationMapKey() {
            // Arrange
            var expedition = new AntarcticExpedition() {
                ExpeditionID = Guid.NewGuid(),
                Leader = "Ernest Shackelton",
                LeadScientist = "Edgeworth Davis",
                ExpeditionName = "Nimrod Expedition",
                WasSponsoredByRGS = false,
                Discoveries = new RelationMap<LocalizedText, DateTime>() {
                    { new LocalizedText("BEARDMORE_GLACIER"), new DateTime(1908, 12, 3) },
                    { new LocalizedText("MOUNT_EREBUS_PEAK"), new DateTime(1908, 3, 11) },
                    { new LocalizedText("MAGNETIC_SOUTH_POLE"), new DateTime(1909, 1, 17) }
                },
                NumShips = 1,
                StartDate = new DateOnly(1907, 8, 11)
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(AntarcticExpedition)];
            var data = translation.Relations[0].Extractor.ExtractFrom(expedition);

            // Assert
            data.Insertions.Should().HaveCount(3);
            data.Insertions.Should().ContainRow("BEARDMORE_GLACIER", new DateTime(1908, 12, 3));
            data.Insertions.Should().ContainRow("MOUNT_EREBUS_PEAK", new DateTime(1908, 3, 11));
            data.Insertions.Should().ContainRow("MAGNETIC_SOUTH_POLE", new DateTime(1909, 1, 17));
            data.Modifications.Should().BeEmpty();
            data.Deletions.Should().BeEmpty();
        }

        [TestMethod] public void LocalizationMapValue() {
            // Arrange
            var iditarod = new Iditarod() {
                Year = 2023,
                StartDate = new DateTime(2023, 3, 4),
                EndDate = new DateTime(2023, 3, 17),
                RaceTimes = new RelationMap<string, LocalizedMeasure>() {
                    { "Ryan Redington", new LocalizedMeasure(8211258) },
                    { "Peter Kaiser", new LocalizedMeasure(8223640) },
                    { "Richie Diehl", new LocalizedMeasure(8234020) },
                    { "Matt Hall", new LocalizedMeasure(9022157) },
                    { "Jessie Holmes", new LocalizedMeasure(9040853) },
                    { "Kelly Maixner", new LocalizedMeasure(9050015) },
                    { "Eddie Burke, Jr.", new LocalizedMeasure(9083754) },
                    { "Matthew Failor", new LocalizedMeasure(9092036) },
                    { "Millie Porsild", new LocalizedMeasure(9124232) },
                    { "Wade Marrs", new LocalizedMeasure(9130756) },
                    { "Hunter Keefe", new LocalizedMeasure(9233944) },
                    { "Dan Kaduce", new LocalizedMeasure(10002304) },
                    { "Christian Turner", new LocalizedMeasure(10011606) },
                    { "Jessie Royer", new LocalizedMeasure(10013507) },
                    { "Aaron Peck", new LocalizedMeasure(10031517) },
                    { "KattiJo Deeter", new LocalizedMeasure(10064400) },
                    { "Nicolas Petit", new LocalizedMeasure(10100918) },
                    { "Riley Dyche", new LocalizedMeasure(10141146) },
                    { "Ramey Smyth", new LocalizedMeasure(10152434) },
                    { "Deke Naaktgeboren", new LocalizedMeasure(10190502) },
                    { "Kristy Berington", new LocalizedMeasure(10235050) },
                    { "Anna Berington", new LocalizedMeasure(10235106) },
                    { "Michael Williams, Jr.", new LocalizedMeasure(11045716) },
                    { "Bailey Vitello", new LocalizedMeasure(11164957) },
                    { "Joanna Jagow", new LocalizedMeasure(11170723) },
                    { "Gerhardt Thiart", new LocalizedMeasure(11210026) },
                    { "Bridgett Watkins", new LocalizedMeasure(11210848) },
                    { "Jed Stephensen", new LocalizedMeasure(12004424) },
                    { "Jason Mackey", new LocalizedMeasure(12020307) }
                },
                TotalSledDogs = 462,
                PrizeMoney = 500000M
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(Iditarod)];
            var data = translation.Relations[0].Extractor.ExtractFrom(iditarod);

            // Assert
            data.Insertions.Should().HaveCount(29);
            data.Insertions.Should().ContainRow("Ryan Redington", iditarod.RaceTimes["Ryan Redington"].Key);
            data.Insertions.Should().ContainRow("Peter Kaiser", iditarod.RaceTimes["Peter Kaiser"].Key);
            data.Insertions.Should().ContainRow("Richie Diehl", iditarod.RaceTimes["Richie Diehl"].Key);
            data.Insertions.Should().ContainRow("Matt Hall", iditarod.RaceTimes["Matt Hall"].Key);
            data.Insertions.Should().ContainRow("Jessie Holmes", iditarod.RaceTimes["Jessie Holmes"].Key);
            data.Insertions.Should().ContainRow("Kelly Maixner", iditarod.RaceTimes["Kelly Maixner"].Key);
            data.Insertions.Should().ContainRow("Eddie Burke, Jr.", iditarod.RaceTimes["Eddie Burke, Jr."].Key);
            data.Insertions.Should().ContainRow("Matthew Failor", iditarod.RaceTimes["Matthew Failor"].Key);
            data.Insertions.Should().ContainRow("Millie Porsild", iditarod.RaceTimes["Millie Porsild"].Key);
            data.Insertions.Should().ContainRow("Wade Marrs", iditarod.RaceTimes["Wade Marrs"].Key);
            data.Insertions.Should().ContainRow("Hunter Keefe", iditarod.RaceTimes["Hunter Keefe"].Key);
            data.Insertions.Should().ContainRow("Dan Kaduce", iditarod.RaceTimes["Dan Kaduce"].Key);
            data.Insertions.Should().ContainRow("Christian Turner", iditarod.RaceTimes["Christian Turner"].Key);
            data.Insertions.Should().ContainRow("Jessie Royer", iditarod.RaceTimes["Jessie Royer"].Key);
            data.Insertions.Should().ContainRow("Aaron Peck", iditarod.RaceTimes["Aaron Peck"].Key);
            data.Insertions.Should().ContainRow("KattiJo Deeter", iditarod.RaceTimes["KattiJo Deeter"].Key);
            data.Insertions.Should().ContainRow("Nicolas Petit", iditarod.RaceTimes["Nicolas Petit"].Key);
            data.Insertions.Should().ContainRow("Riley Dyche", iditarod.RaceTimes["Riley Dyche"].Key);
            data.Insertions.Should().ContainRow("Ramey Smyth", iditarod.RaceTimes["Ramey Smyth"].Key);
            data.Insertions.Should().ContainRow("Deke Naaktgeboren", iditarod.RaceTimes["Deke Naaktgeboren"].Key);
            data.Insertions.Should().ContainRow("Kristy Berington", iditarod.RaceTimes["Kristy Berington"].Key);
            data.Insertions.Should().ContainRow("Anna Berington", iditarod.RaceTimes["Anna Berington"].Key);
            data.Insertions.Should().ContainRow("Michael Williams, Jr.", iditarod.RaceTimes["Michael Williams, Jr."].Key);
            data.Insertions.Should().ContainRow("Bailey Vitello", iditarod.RaceTimes["Bailey Vitello"].Key);
            data.Insertions.Should().ContainRow("Joanna Jagow", iditarod.RaceTimes["Joanna Jagow"].Key);
            data.Insertions.Should().ContainRow("Gerhardt Thiart", iditarod.RaceTimes["Gerhardt Thiart"].Key);
            data.Insertions.Should().ContainRow("Bridgett Watkins", iditarod.RaceTimes["Bridgett Watkins"].Key);
            data.Insertions.Should().ContainRow("Jed Stephensen", iditarod.RaceTimes["Jed Stephensen"].Key);
            data.Insertions.Should().ContainRow("Jason Mackey", iditarod.RaceTimes["Jason Mackey"].Key);
            data.Modifications.Should().BeEmpty();
            data.Deletions.Should().BeEmpty();
        }

        [TestMethod] public void LocalizationOrderedListElement() {
            // Arrange
            var festigal = new Festigal() {
                Year = 2010,
                HostCity = "Tel Aviv",
                Songs = new RelationOrderedList<LocalizedText>() {
                    new LocalizedText("Lailah M'toref Festigal"),
                    new LocalizedText("Yeled Im Khalom"),
                    new LocalizedText("LaGa'at Ba'avar"),
                    new LocalizedText("Shir Ahavah Tzarafti"),
                    new LocalizedText("Ahavah Rishonah"),
                    new LocalizedText("Loko"),
                    new LocalizedText("LaNetzakh Nisha'ar"),
                    new LocalizedText("K'sh'emtza Otkha"),
                    new LocalizedText("Al Tafsik Tigrov"),
                    new LocalizedText("Nig'm'ru Li HaMilim")
                },
                Opening = new DateOnly(2010, 12, 2),
                Theme = "History",
                NumPerformers = 137
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(Festigal)];
            var data = translation.Relations[0].Extractor.ExtractFrom(festigal);

            // Assert
            data.Insertions.Should().HaveCount(10);
            data.Insertions.Should().ContainRow(0U, "Lailah M'toref Festigal");
            data.Insertions.Should().ContainRow(1U, "Yeled Im Khalom");
            data.Insertions.Should().ContainRow(2U, "LaGa'at Ba'avar");
            data.Insertions.Should().ContainRow(3U, "Shir Ahavah Tzarafti");
            data.Insertions.Should().ContainRow(4U, "Ahavah Rishonah");
            data.Insertions.Should().ContainRow(5U, "Loko");
            data.Insertions.Should().ContainRow(6U, "LaNetzakh Nisha'ar");
            data.Insertions.Should().ContainRow(7U, "K'sh'emtza Otkha");
            data.Insertions.Should().ContainRow(8U, "Al Tafsik Tigrov");
            data.Insertions.Should().ContainRow(9U, "Nig'm'ru Li HaMilim");
            data.Modifications.Should().BeEmpty();
            data.Deletions.Should().BeEmpty();
        }

        [TestMethod] public void LocalizationWithNoValues() {
            // Arrange
            var mural = new Mural(Guid.NewGuid());

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(Mural), Translator.AsLocalzation];
            var data = translation.Principal.Extractor.ExtractFrom(mural);

            // Assert
            data.Insertions.Should().BeEmpty();
            data.Modifications.Should().BeEmpty();
            data.Deletions.Should().BeEmpty();
        }

        [TestMethod] public void LocalizationWithOnlyNewValues() {
            // Arrange
            var carpeting = new Carpeting("POLYESTER_CARPET_LOC");
            carpeting[7] = "Polyester Carpeting";
            carpeting[-38124] = "Carpeting of Polyester";
            carpeting[15516026] = "Polyestered Carpet";

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(Carpeting), Translator.AsLocalzation];
            var data = translation.Principal.Extractor.ExtractFrom(carpeting);

            // Assert
            data.Insertions.Should().HaveCount(3);
            data.Insertions.Should().ContainRow(carpeting.Key, 7, carpeting[7]);
            data.Insertions.Should().ContainRow(carpeting.Key, -38124, carpeting[-38124]);
            data.Insertions.Should().ContainRow(carpeting.Key, 15516026, carpeting[15516026]);
            data.Modifications.Should().BeEmpty();
            data.Deletions.Should().BeEmpty();
        }

        [TestMethod] public void LocalizationWithOnlySavedValues() {
            // Arrange
            var taste = new Taste('u');
            taste[Language.English] = "umami";
            taste[Language.Hebrew] = "אומאמי";
            (taste.Relation as IRelation).Canonicalize();

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(Taste), Translator.AsLocalzation];
            var data = translation.Principal.Extractor.ExtractFrom(taste);

            // Assert
            data.Insertions.Should().BeEmpty();
            data.Modifications.Should().BeEmpty();
            data.Deletions.Should().BeEmpty();
        }

        [TestMethod] public void LocalizationWithDeletedValues() {
            // Arrange
            var trait = new HumanTrait(Guid.NewGuid());
            trait[Language.Italian] = "Timidezza";
            trait[Language.German] = "Schüchternheit";
            trait[Language.Esperanto] = "Timemo";
            trait[Language.Japanese] = "内気";
            (trait.Relation as IRelation).Canonicalize();
            trait.Delocalize(Language.Italian);
            trait.Delocalize(Language.Esperanto);
            trait.Delocalize(Language.Japanese);

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(HumanTrait), Translator.AsLocalzation];
            var data = translation.Principal.Extractor.ExtractFrom(trait);

            // Assert
            data.Insertions.Should().BeEmpty();
            data.Modifications.Should().BeEmpty();
            data.Deletions.Should().HaveCount(3);
            data.Deletions.Should().ContainRow(trait.Key, ConversionOf(Language.Italian), "Timidezza");
            data.Deletions.Should().ContainRow(trait.Key, ConversionOf(Language.Esperanto), "Timemo");
            data.Deletions.Should().ContainRow(trait.Key, ConversionOf(Language.Japanese), "内気");
        }

        [TestMethod] public void LocalizationWithNestedAggregate() {
            // Arrange
            var position = new CSuitePosition('e');
            position["ND"] = new Position("CEO", 150970M);
            position["AZ"] = new Position("Chief Execution Officer", 132964M);
            position["GA"] = new Position("C.E.O.", 120479M);
            position["HI"] = new Position("CEO", 148241M);

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(CSuitePosition), Translator.AsLocalzation];
            var data = translation.Principal.Extractor.ExtractFrom(position);

            // Assert
            data.Insertions.Should().HaveCount(4);
            data.Insertions.Should().ContainRow(position.Key, "ND", position["ND"].AverageSalary, position["ND"].Name);
            data.Insertions.Should().ContainRow(position.Key, "AZ", position["AZ"].AverageSalary, position["AZ"].Name);
            data.Insertions.Should().ContainRow(position.Key, "GA", position["GA"].AverageSalary, position["GA"].Name);
            data.Insertions.Should().ContainRow(position.Key, "HI", position["HI"].AverageSalary, position["HI"].Name);
            data.Modifications.Should().BeEmpty();
            data.Deletions.Should().BeEmpty();
        }

        [TestMethod] public void LocalizationWithNestedReference() {
            // Arrange
            var cookie = new KeeblerCookie() {
                Name = "Fudge Stripes",
                Chocolatey = true,
                RetailPrice = 5.72M,
                CaloriesPerServing = 140
            };
            var schedule = new CookieSchedule("SCHED_A_LOC");
            schedule[DayOfWeek.Wednesday] = cookie;

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(CookieSchedule), Translator.AsLocalzation];
            var data = translation.Principal.Extractor.ExtractFrom(schedule);

            // Assert
            data.Insertions.Should().HaveCount(1);
            data.Insertions.Should().ContainRow(schedule.Key, ConversionOf(DayOfWeek.Wednesday), cookie.Name, cookie.Chocolatey);
            data.Modifications.Should().HaveCount(0);
            data.Deletions.Should().HaveCount(0);
        }

        [TestMethod] public void PreDefinedLocalization() {
            // Arrange
            var era = TaylorSwiftEra.Folklore;

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(TaylorSwiftEra), Translator.AsLocalzation];
            var data = translation.Principal.Extractor.ExtractFrom(era);

            // Assert
            data.Insertions.Should().HaveCount(2);
            data.Insertions.Should().ContainRow(era.Key, ConversionOf(Language.English), era[Language.English]);
            data.Insertions.Should().ContainRow(era.Key, ConversionOf(Language.Spanish), era[Language.Spanish]);
            data.Modifications.Should().BeEmpty();
            data.Deletions.Should().BeEmpty();
        }


        private static string ConversionOf<T>(T enumerator) where T : Enum {
            var converter = new EnumToStringConverter(typeof(T)).ConverterImpl;
            return (string)converter.Convert(enumerator)!;
        }
    }
}
