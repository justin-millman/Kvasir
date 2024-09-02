using FluentAssertions;
using Kvasir.Core;
using Kvasir.Relations;
using Kvasir.Translation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

using static UT.Kvasir.Translation.Globals;
using static UT.Kvasir.Translation.DataExtraction;
using static UT.Kvasir.Translation.TestConverters;

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

            // Assert
            data.Should().HaveCount(7);
            data[0].Datum.Should().Be(morgue.Name);
            data[1].Datum.Should().Be(morgue.ChiefMedicalExaminer);
            data[2].Datum.Should().Be(morgue.Capacity);
            data[3].Datum.Should().Be(morgue.Budget);
            data[4].Datum.Should().Be(morgue.FederalGrade);
            data[5].Datum.Should().Be(ConversionOf(morgue.AvailableServices));
            data[6].Datum.Should().Be(morgue.GovernmentRun);
        }

        [TestMethod] public void NonNullPublicStaticScalars() {
            // Arrange
            var interpreter = new PythonInterpreter() {
                ProgramID = new Guid(),
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

            // Assert
            data.Should().HaveCount(6);
            data[0].Datum.Should().Be(interpreter.ProgramID);
            data[1].Datum.Should().Be(interpreter.Path);
            data[2].Datum.Should().Be(interpreter.InstalledOn);
            data[3].Datum.Should().Be(PythonInterpreter.MinVersion);
            data[4].Datum.Should().Be(PythonInterpreter.MaxVersion);
            data[5].Datum.Should().Be(ConversionOf(PythonInterpreter.BackEndLanguage));
        }

        [TestMethod] public void NonNullNonPublicInstanceScalars() {
            // Arrange
            var ship = new PirateShip() {
                ID = new Guid(),
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

            // Assert
            data.Should().HaveCount(7);
            data[0].Datum.Should().Be(ship.ID);
            data[1].Datum.Should().Be(ship.ShipName);
            data[2].Datum.Should().Be(ship.Captain);
            data[3].Datum.Should().Be(ship.GetLength());
            data[4].Datum.Should().Be(ship.GetNumCannons());
            data[5].Datum.Should().Be(ConversionOf(ship.Style));
            data[6].Datum.Should().Be(ship.CarriedSlaves);
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

            // Assert
            data.Should().HaveCount(6);
            data[0].Datum.Should().Be(enzyme.EnzymeCommissionNumber);
            data[1].Datum.Should().Be(enzyme.CommonName);
            data[2].Datum.Should().Be(Enzyme.GetIsEnzme());
            data[3].Datum.Should().Be(Enzyme.GetNumEnzymesTotal());
            data[4].Datum.Should().Be(Enzyme.Regulator);
            data[5].Datum.Should().Be(Enzyme.FirstDiscovered);
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

            // Assert
            data.Should().HaveCount(7);
            data[0].Datum.Should().Be(ode.Title);
            data[1].Datum.Should().Be(ode.Author);
            data[2].Datum.Should().Be(ode.Lines);
            data[3].Datum.Should().Be(ode.WordCount);
            data[4].Datum.Should().Be(DBNull.Value);
            data[5].Datum.Should().Be(ode.Collection);
            data[6].Datum.Should().Be(DBNull.Value);
        }

        [TestMethod] public void ExplicitInterfaceImplementation() {
            // Arrange
            var tlatoani = new Tlatoani() {
                ID = new Guid(),
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
                AuroraID = new Guid(),
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
                MazeID = new Guid(),
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
                FightID = new Guid(),
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
                QuestionID = new Guid(),
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
                LibraryID = new Guid(),
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
                ID = new Guid(),
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
                PretzelID = new Guid(),
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
                GrillID = new Guid(),
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
                ID = new Guid(),
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
                DoctoralTheses = null,
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

        [TestMethod] public void NonNullRelationBecomesNull() {
            // Arrange
            var orogene = new Orogene() {
                FulcrumName = "Alabaster",
                BirthName = null,
                BirthComm = null,
                Rings = 10,
                Appearances = new RelationMap<Orogene.Book, bool>() {
                    { Orogene.Book.FifthSeason, true },
                    { Orogene.Book.ObeliskGate, true },
                    { Orogene.Book.StoneSky, false }
                },
                AtNodeStation = false
            };

            // Act
            var translator = new Translator(NO_ENTITIES);
            var translation = translator[typeof(Orogene)];
            var _ = translation.Relations[0].Extractor.ExtractFrom(orogene);
            orogene.Appearances = null;
            var data = translation.Relations[0].Extractor.ExtractFrom(orogene);

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
                        ID = new Guid(),
                        Claim = new Impeachment.Charge() {
                            Claim = "Improper Conduct during the Trial of John Fries",
                            Severity = Impeachment.Charge.Category.Misdemeanor
                        },
                        Guilty = false
                    },
                    new Impeachment.Count() {
                        ID = new Guid(),
                        Claim = new Impeachment.Charge() {
                            Claim = "Improper Conduct during the Trial of James T. Callendar",
                            Severity = Impeachment.Charge.Category.Misdemeanor
                        },
                        Guilty = false
                    },
                    new Impeachment.Count() {
                        ID = new Guid(),
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
                ReptileID = new Guid(),
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


        private static string ConversionOf<T>(T enumerator) where T : Enum {
            var converter = new EnumToStringConverter(typeof(T)).ConverterImpl;
            return (string)converter.Convert(enumerator)!;
        }
    }
}
