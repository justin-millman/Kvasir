using FluentAssertions;
using Kvasir.Core;
using Kvasir.Exceptions;
using Kvasir.Schema;
using Kvasir.Translation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

using static UT.Kvasir.Translation.TestComponents;

namespace UT.Kvasir.Translation {
    [TestClass, TestCategory("Data Converters")]
    public class DataConverterTests {
        [TestMethod] public void DataCoverterDoesNotChangeDataType() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Cenote);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField("Name", DBType.Text, IsNullable.No).And
                .HaveField("MaxDepth", DBType.Single, IsNullable.No).And
                .HaveField("IsKarst", DBType.Boolean, IsNullable.No).And
                .HaveField("Latitude", DBType.Decimal, IsNullable.No).And
                .HaveField("Longitude", DBType.Decimal, IsNullable.No).And
                .NoOtherFields();
        }

        [TestMethod] public void DataConverterChangesDataType() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Comet);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField("AstronomicalIdentifier", DBType.Guid, IsNullable.No).And
                .HaveField("Aphelion", DBType.Double, IsNullable.No).And
                .HaveField("Perihelion", DBType.Int32, IsNullable.No).And
                .HaveField("Eccentricity", DBType.Int32, IsNullable.No).And
                .HaveField("MassKg", DBType.UInt64, IsNullable.No).And
                .HaveField("Albedo", DBType.Double, IsNullable.No).And
                .HaveField("OrbitalPeriod", DBType.Single, IsNullable.No).And
                .NoOtherFields();
        }

        [TestMethod] public void DataConverterResultTypeIsNonNullableWhileSourceIsNullable() {
            // Arrange
            var translator = new Translator();
            var source = typeof(RoyalHouse);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField("HouseName", DBType.Text, IsNullable.No).And
                .HaveField("Founded", DBType.DateTime, IsNullable.No).And
                .HaveField("CurrentHead", DBType.Text, IsNullable.Yes).And
                .HaveField("TotalMonarchs", DBType.Int32, IsNullable.Yes).And
                .NoOtherFields();
        }

        [TestMethod] public void DataCoverterResultTypeIsNullableWhileSourceIsNonNullable() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Planeswalker);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField("Name", DBType.Text, IsNullable.No).And
                .HaveField("MannaCost", DBType.Int8, IsNullable.No).And
                .HaveField("InitialLoyalty", DBType.Int8, IsNullable.No).And
                .HaveField("Ability1", DBType.Text, IsNullable.No).And
                .HaveField("Ability2", DBType.Text, IsNullable.No).And
                .HaveField("Ability3", DBType.Text, IsNullable.No).And
                .NoOtherFields();
        }

        [TestMethod] public void DataCoverterSourceTypeIsNullableOnNonNullableField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(GolfHole);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField("CourseID", DBType.Int32, IsNullable.No).And
                .HaveField("HoleNumber", DBType.UInt8, IsNullable.No).And
                .HaveField("Par", DBType.UInt8, IsNullable.No).And
                .HaveField("DistanceToFlag", DBType.UInt16, IsNullable.No).And
                .HaveField("NumSandTraps", DBType.UInt8, IsNullable.No).And
                .HaveField("ContainsWaterHazard", DBType.Boolean, IsNullable.No).And
                .NoOtherFields();
        }

        [TestMethod] public void DataCoverterSourceTypeUnrelatedToFieldType_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Jedi);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                        // source type
                .WithMessage($"*{nameof(Jedi.MiddleName)}*")            // source property
                .WithMessage("*[DataConverter]*")                       // annotation
                .WithMessage($"*property of type {nameof(String)}*")    // rationale
                .WithMessage($"*operates on {nameof(Boolean)}*");       // details
        }

        [TestMethod] public void DataConverterSourceTypeCovertibleWithFieldType_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(ConstitutionalAmendment);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                                // source type
                .WithMessage($"*{nameof(ConstitutionalAmendment.Number)}*")     // source property
                .WithMessage("*[DataConverter]*")                               // annotation
                .WithMessage($"*property of type {nameof(Int32)}*")             // rationale
                .WithMessage($"*operates on {nameof(Int64)}*");                 // details
        }

        [TestMethod] public void DataConverterResultTypeNotSupported_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(SNLEpisode);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                        // source type
                .WithMessage($"*{nameof(SNLEpisode.AirDate)}*")         // source property
                .WithMessage("*[DataConverter]*")                       // annotation
                .WithMessage("*result type*is not supported*")          // rationale
                .WithMessage($"*{nameof(ArgumentException)}*");         // details
        }

        [TestMethod] public void DataConverterIsNotDataConverter_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(MetraRoute);

            // Act
            var act = () => translator[source];
            
            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                                   // source type
                .WithMessage($"*{nameof(MetraRoute.Line)}*")                       // source property
                .WithMessage("*[DataConverter]*")                                  // annotation
                .WithMessage($"*does not implement*{nameof(IDataConverter)}*")     // rationale
                .WithMessage($"*{nameof(Int32)}*");                                // details
        }

        [TestMethod] public void DataConverterIsNotDefaultConstructible_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Paycheck);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                                   // source type
                .WithMessage($"*{nameof(Paycheck.HoursWorked)}*")                  // source property
                .WithMessage("*[DataConverter]*")                                  // annotation
                .WithMessage("*not*default*constructor*");                         // rationale
        }

        [TestMethod] public void DataConverterThrowsOnConstruction_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Sword);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                                   // source type
                .WithMessage($"*{nameof(Sword.YearForged)}*")                      // source property
                .WithMessage("*[DataConverter]*")                                  // annotation
                .WithMessage("*constructing*")                                     // rationale
                .WithMessage("*System Failure!*");                                 // details
        }

        [TestMethod] public void IdentityDataConverter() {
            // Arrange
            var translator = new Translator();
            var source = typeof(FieldGoal);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField("When", DBType.DateTime, IsNullable.No).And
                .HaveField("Made", DBType.Boolean, IsNullable.No).And
                .HaveField("Doinks", DBType.Int32, IsNullable.No).And
                .HaveField("Kicker", DBType.Text, IsNullable.No).And
                .NoOtherFields();
        }

        [TestMethod] public void MultipleDataConverters_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(BowlingFrame);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                            // source type
                .WithMessage($"*{nameof(BowlingFrame.FirstThrowPins)}*")    // source property
                .WithMessage("*[DataConverter]*")                           // annotation
                .WithMessage("*multiple*");                                 // rationale
        }
    }
}
