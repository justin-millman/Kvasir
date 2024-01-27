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
    }
}
