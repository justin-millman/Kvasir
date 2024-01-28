using FluentAssertions;
using Kvasir.Translation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

using static UT.Kvasir.Translation.DataExtraction;
using static UT.Kvasir.Translation.TestConverters;

namespace UT.Kvasir.Translation {
    [TestClass, TestCategory("Data Extraction")]
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
            var translator = new Translator();
            var translation = translator[typeof(Morgue)];
            var data = translation.Principal.Extractor.Execute(morgue);

            // Assert
            data.Should().HaveCount(7);
            data[0].Should().Be(morgue.Name);
            data[1].Should().Be(morgue.ChiefMedicalExaminer);
            data[2].Should().Be(morgue.Capacity);
            data[3].Should().Be(morgue.Budget);
            data[4].Should().Be(morgue.FederalGrade);
            data[5].Should().Be(morgue.AvailableServices);
            data[6].Should().Be(morgue.GovernmentRun);
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
            var translator = new Translator();
            var translation = translator[typeof(PythonInterpreter)];
            var data = translation.Principal.Extractor.Execute(interpreter);

            // Assert
            data.Should().HaveCount(6);
            data[0].Should().Be(interpreter.ProgramID);
            data[1].Should().Be(interpreter.Path);
            data[2].Should().Be(interpreter.InstalledOn);
            data[3].Should().Be(PythonInterpreter.MinVersion);
            data[4].Should().Be(PythonInterpreter.MaxVersion);
            data[5].Should().Be(PythonInterpreter.BackEndLanguage);
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
            var translator = new Translator();
            var translation = translator[typeof(PirateShip)];
            var data = translation.Principal.Extractor.Execute(ship);

            // Assert
            data.Should().HaveCount(7);
            data[0].Should().Be(ship.ID);
            data[1].Should().Be(ship.ShipName);
            data[2].Should().Be(ship.Captain);
            data[3].Should().Be(ship.GetLength());
            data[4].Should().Be(ship.GetNumCannons());
            data[5].Should().Be(ship.Style);
            data[6].Should().Be(ship.CarriedSlaves);
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
            var translator = new Translator();
            var translation = translator[typeof(Enzyme)];
            var data = translation.Principal.Extractor.Execute(enzyme);

            // Assert
            data.Should().HaveCount(6);
            data[0].Should().Be(enzyme.EnzymeCommissionNumber);
            data[1].Should().Be(enzyme.CommonName);
            data[2].Should().Be(Enzyme.GetIsEnzme());
            data[3].Should().Be(Enzyme.GetNumEnzymesTotal());
            data[4].Should().Be(Enzyme.Regulator);
            data[5].Should().Be(Enzyme.FirstDiscovered);
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
            var translator = new Translator();
            var translation = translator[typeof(Ode)];
            var data = translation.Principal.Extractor.Execute(ode);

            // Assert
            data.Should().HaveCount(7);
            data[0].Should().Be(ode.Title);
            data[1].Should().Be(ode.Author);
            data[2].Should().Be(ode.Lines);
            data[3].Should().Be(ode.WordCount);
            data[4].Should().Be(ode.Publication);
            data[5].Should().Be(ode.Collection);
            data[6].Should().Be(ode.Style);
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
            var translator = new Translator();
            var translation = translator[typeof(Tlatoani)];
            var data = translation.Principal.Extractor.Execute(tlatoani);

            // Assert
            data.Should().HaveCount(6);
            data[0].Should().Be(tlatoani.ID);
            data[1].Should().Be((tlatoani as IWorldLeader).Name);
            data[2].Should().Be((tlatoani as IWorldLeader).Polity);
            data[3].Should().Be(tlatoani.Death);
            data[4].Should().Be(tlatoani.EncounteredConquistadors);
            data[5].Should().Be(tlatoani.CoronationYear);
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
            var translator = new Translator();
            var translation = translator[typeof(StateQuarter)];
            var data = translation.Principal.Extractor.Execute(quarter);

            // Assert
            data.Should().HaveCount(5);
            data[0].Should().Be(quarter.State);
            data[1].Should().Be(quarter.Denomination);
            data[2].Should().Be(quarter.Year);
            data[3].Should().Be(quarter.Engraver);
            data[4].Should().Be(quarter.Mintage);
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
            var translator = new Translator();
            var translation = translator[typeof(Aurora)];
            var data = translation.Principal.Extractor.Execute(aurora);

            // Assert
            data.Should().HaveCount(4);
            data[0].Should().Be(aurora.AuroraID);
            data[1].Should().Be(aurora.Name);
            data[2].Should().Be(aurora.AKA);
            data[3].Should().Be(aurora.Intensity);
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
            var translator = new Translator();
            var translation = translator[typeof(Underworld)];
            var data = translation.Principal.Extractor.Execute(underworld);

            // Act
            data[0].Should().Be(underworld.Name);
            data[1].Should().Be(underworld.Civilization);
            data[2].Should().Be(underworld.Lord);
            data[3].Should().Be(new Invert().Convert(underworld.ForMortals));
            data[4].Should().Be(new MakeDate<int>().Convert(underworld.GoogleResults));
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
            var translator = new Translator();
            var translation = translator[typeof(CornMaze)];
            var data = translation.Principal.Extractor.Execute(maze);

            // Assert
            data.Should().HaveCount(7);
            data[0].Should().Be(maze.MazeID);
            data[1].Should().Be((byte)maze.CornType);
            data[2].Should().Be((ulong)maze.MazeShape);
            data[3].Should().Be(maze.StalkCount);
            data[4].Should().Be(maze.MazeArea);
            data[5].Should().Be(maze.SuccessRate);
            data[6].Should().Be(maze.RecordTime);
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
            var translator = new Translator();
            var translation = translator[typeof(MarioKartRacetrack)];
            var data = translation.Principal.Extractor.Execute(racetrack);

            // Assert
            data.Should().HaveCount(5);
            data[0].Should().Be(racetrack.Name);
            data[1].Should().Be(racetrack.FirstAppearance);
            data[2].Should().Be(racetrack.Series.ToString());
            data[3].Should().Be(racetrack.TrackLength);
            data[4].Should().Be(racetrack.AvailableOnline);
        }

        [TestMethod] public void Calculated() {
            // Arrange
            var lighthouse = new Lighthouse() {
                Name = "Pemaquid Point Lighthouse",
                Location = "Muscongus Bay in Bristol, Maine",
                Height = 11.5,
                FocalLength = 79
            };

            // Act
            var translator = new Translator();
            var translation = translator[typeof(Lighthouse)];
            var data = translation.Principal.Extractor.Execute(lighthouse);

            // Assert
            data.Should().HaveCount(5);
            data[0].Should().Be(lighthouse.Name);
            data[1].Should().Be(lighthouse.Location);
            data[2].Should().Be(lighthouse.Height);
            data[3].Should().Be(lighthouse.FocalLength);
            data[4].Should().Be(lighthouse.LighthouseRating);
        }

        [TestMethod] public void NonNullSingleFieldAggregate() {
            // Arrange
            var nucleobase = new Nucleobase {
                Symbol = new Nucleobase.Letter() { Value = 'G' },
                Name = "Guanine",
                ChemicalFormula = "C5H5N5O"
            };

            // Act
            var translator = new Translator();
            var translation = translator[typeof(Nucleobase)];
            var data = translation.Principal.Extractor.Execute(nucleobase);

            // Assert
            data.Should().HaveCount(3);
            data[0].Should().Be(nucleobase.Symbol.Value);
            data[1].Should().Be(nucleobase.Name);
            data[2].Should().Be(nucleobase.ChemicalFormula);
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
            var translator = new Translator();
            var translation = translator[typeof(LegoSet)];
            var data = translation.Principal.Extractor.Execute(legos);

            // Assert
            data.Should().HaveCount(9);
            data[0].Should().Be(legos.ItemNumber);
            data[1].Should().Be(legos.Title);
            data[2].Should().Be(legos.Catalog.Price);
            data[3].Should().Be(legos.Catalog.Stars);
            data[4].Should().Be(legos.Catalog.URL);
            data[5].Should().Be(legos.Catalog.InsiderPoints);
            data[6].Should().Be(legos.Catalog.Theme);
            data[7].Should().Be(legos.Pieces);
            data[8].Should().Be(legos.LowerBoundAge);
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
            var translator = new Translator();
            var translation = translator[typeof(SnowballFight)];
            var data = translation.Principal.Extractor.Execute(fight);

            // Assert
            data.Should().HaveCount(7);
            data[0].Should().Be(fight.FightID);
            data[1].Should().Be(fight.KickOff);
            data[2].Should().Be(fight.FightStructure.NumTeams);
            data[3].Should().Be(fight.FightStructure.HitsAllowed);
            data[4].Should().Be(fight.FightStructure.MaxBallRadius);
            data[5].Should().Be(fight.Length);
            data[6].Should().Be(fight.LowTemperature);
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
            var translator = new Translator();
            var translation = translator[typeof(Knot)];
            var data = translation.Principal.Extractor.Execute(knot);

            // Assert
            data.Should().HaveCount(4);
            data[0].Should().Be(knot.Name);
            data[1].Should().Be(knot.Shape?.ConwayNotation);
            data[2].Should().Be(knot.Efficiency);
            data[3].Should().Be(knot.AshleyBookOfKnotsPage);
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
            var translator = new Translator(); ;
            var translation = translator[typeof(Armory)];
            var data = translation.Principal.Extractor.Execute(armory);

            // Assert
            data.Should().HaveCount(6);
            data[0].Should().Be(armory.Name);
            data[1].Should().Be(armory.Decommissioned);
            data[2].Should().Be(armory.Location?.Latitude);
            data[3].Should().Be(armory.Location?.Longitude);
            data[4].Should().Be(armory.WeaponsCount);
            data[5].Should().Be(armory.Owner);
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
            var translator = new Translator();
            var translation = translator[typeof(MillionaireQuestion)];
            var data = translation.Principal.Extractor.Execute(question);

            // Assert
            data.Should().HaveCount(19);
            data[0].Should().Be(question.QuestionID);
            data[1].Should().Be(question.Category);
            data[2].Should().Be(question.Question);
            data[3].Should().Be(question.Answers.A.Text);
            data[4].Should().Be(question.Answers.A.FiftyFiftyEliminated);
            data[5].Should().Be(question.Answers.A.AudiencePercentage);
            data[6].Should().Be(question.Answers.A.IsCorrect);
            data[7].Should().Be(question.Answers.B.Text);
            data[8].Should().Be(question.Answers.B.FiftyFiftyEliminated);
            data[9].Should().Be(question.Answers.B.AudiencePercentage);
            data[10].Should().Be(question.Answers.B.IsCorrect);
            data[11].Should().Be(question.Answers.C.Text);
            data[12].Should().Be(question.Answers.C.FiftyFiftyEliminated);
            data[13].Should().Be(question.Answers.C.AudiencePercentage);
            data[14].Should().Be(question.Answers.C.IsCorrect);
            data[15].Should().Be(question.Answers.D.Text);
            data[16].Should().Be(question.Answers.D.FiftyFiftyEliminated);
            data[17].Should().Be(question.Answers.D.AudiencePercentage);
            data[18].Should().Be(question.Answers.D.IsCorrect);
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
            var translator = new Translator();
            var translation = translator[typeof(GroceryGame)];
            var data = translation.Principal.Extractor.Execute(game);

            // Assert
            data.Should().HaveCount(8);
            data[0].Should().Be(game.Name);
            data[1].Should().Be(game.Description);
            data[2].Should().Be(new ToInt<byte>().Convert(game.FirstAppearance.Season));
            data[3].Should().Be(new ToInt<byte>().Convert(game.FirstAppearance.Number));
            data[4].Should().Be(game.FirstAppearance.Judge1);
            data[5].Should().Be(game.FirstAppearance.Judge2);
            data[6].Should().Be(game.FirstAppearance.Judge3);
            data[7].Should().Be(game.NumTimesPlayed);
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
            var translator = new Translator();
            var translation = translator[typeof(PapalConclave)];
            var data = translation.Principal.Extractor.Execute(conclave);

            // Assert
            data.Count.Should().Be(5);
            data[0].Should().Be(conclave.Date);
            data[1].Should().Be(conclave.Ballots);
            data[2].Should().Be(conclave.ElectedPope.Name);
            data[3].Should().Be(conclave.NumElectors);
            data[4].Should().Be(conclave.Dean.Name);
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
            var translator = new Translator();
            var translation = translator[typeof(Cytonic)];
            var data = translation.Principal.Extractor.Execute(cytonic);

            // Assert
            data.Should().HaveCount(6);
            data[0].Should().Be(cytonic.Name);
            data[1].Should().Be(cytonic.CallSign);
            data[2].Should().Be(cytonic.SelfSpecies.Grouping);
            data[3].Should().Be(cytonic.SelfSpecies.SubNumber);
            data[4].Should().Be(cytonic.Abilities);
            data[5].Should().Be(cytonic.Appearances);
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
            var translator = new Translator();
            var translation = translator[typeof(SoapOpera)];
            var data = translation.Principal.Extractor.Execute(soapOpera);

            // Assert
            data.Should().HaveCount(8);
            data[0].Should().Be(soapOpera.Title);
            data[1].Should().Be(soapOpera.IsStillAiring);
            data[2].Should().Be(soapOpera.Premiere);
            data[3].Should().Be(soapOpera.NumSeasons);
            data[4].Should().Be(soapOpera.NumEpisodes);
            data[5].Should().Be(soapOpera.NumCastMembers);
            data[6].Should().Be(soapOpera.OwningNetwork?.Name);
            data[7].Should().Be(soapOpera.IsTelenovela);
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
            var translator = new Translator();
            var translation = translator[typeof(Library)];
            var data = translation.Principal.Extractor.Execute(library);

            // Assert
            data.Should().HaveCount(6);
            data[0].Should().Be(library.LibraryID);
            data[1].Should().Be(library.NumBooks);
            data[2].Should().Be(library.HeadLibrarian?.FirstName);
            data[3].Should().Be(library.HeadLibrarian?.LastName);
            data[4].Should().Be(library.Endowment);
            data[5].Should().Be(library.Branches);
        }

        [TestMethod] public void RelationNestedDataConversion() {
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
            var translator = new Translator();
            var translation = translator[typeof(CurlingMatch)];
            var data = translation.Principal.Extractor.Execute(match);

            // Assert
            data.Should().HaveCount(8);
            data[0].Should().Be(match.ID);
            data[1].Should().Be(new AllCaps().Convert(match.TeamA.Code));
            data[2].Should().Be(new AllCaps().Convert(match.TeamB.Code));
            data[3].Should().Be(match.ScoreA);
            data[4].Should().Be(match.ScoreB);
            data[5].Should().Be(match.Date);
            data[6].Should().Be(match.Olympiad);
            data[7].Should().Be(match.HammerForA);
        }
    }
}
