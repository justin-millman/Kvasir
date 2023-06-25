using FluentAssertions;
using Kvasir.Core;
using Kvasir.Exceptions;
using Kvasir.Translation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

using static UT.Kvasir.Translation.DataConverters;
using static UT.Kvasir.Translation.TestConverters;

namespace UT.Kvasir.Translation {
    [TestClass, TestCategory("DataConverters")]
    public class DataConverterTests {

        [TestMethod] public void NoChangeToFieldsType_Redundant() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Cenote);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField(nameof(Cenote.Name)).OfTypeText().BeingNonNullable().And
                .HaveField(nameof(Cenote.MaxDepth)).OfTypeSingle().BeingNonNullable().And
                .HaveField(nameof(Cenote.IsKarst)).OfTypeBoolean().BeingNonNullable().And
                .HaveField(nameof(Cenote.Latitude)).OfTypeDecimal().BeingNonNullable().And
                .HaveField(nameof(Cenote.Longitude)).OfTypeDecimal().BeingNonNullable().And
                .HaveNoOtherFields();
        }
        
        [TestMethod] public void ChangeToFieldsType() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Comet);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField(nameof(Comet.AstronomicalIdentifier)).OfTypeGuid().BeingNonNullable().And
                .HaveField(nameof(Comet.Aphelion)).OfTypeDouble().BeingNonNullable().And
                .HaveField(nameof(Comet.Perihelion)).OfTypeInt64().BeingNonNullable().And
                .HaveField(nameof(Comet.Eccentricity)).OfTypeInt64().BeingNonNullable().And
                .HaveField(nameof(Comet.MassKg)).OfTypeUInt64().BeingNonNullable().And
                .HaveField(nameof(Comet.Albedo)).OfTypeDouble().BeingNonNullable().And
                .HaveField(nameof(Comet.OrbitalPeriod)).OfTypeSingle().BeingNonNullable().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void ConverterOnNullablePropertyHasNonNullableTargetType() {
            // Arrange
            var translator = new Translator();
            var source = typeof(RoyalHouse);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField(nameof(RoyalHouse.HouseName)).OfTypeText().BeingNonNullable().And
                .HaveField(nameof(RoyalHouse.Founded)).OfTypeDateTime().BeingNonNullable().And
                .HaveField(nameof(RoyalHouse.CurrentHead)).OfTypeText().BeingNullable().And
                .HaveField(nameof(RoyalHouse.TotalMonarchs)).OfTypeInt32().BeingNullable().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void ConverterOnNonNullablePropertyHasNullableTargetType() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Planeswalker);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField(nameof(Planeswalker.Name)).OfTypeText().BeingNonNullable().And
                .HaveField(nameof(Planeswalker.MannaCost)).OfTypeInt8().BeingNonNullable().And
                .HaveField(nameof(Planeswalker.InitialLoyalty)).OfTypeInt8().BeingNonNullable().And
                .HaveField(nameof(Planeswalker.Ability1)).OfTypeText().BeingNonNullable().And
                .HaveField(nameof(Planeswalker.Ability2)).OfTypeText().BeingNonNullable().And
                .HaveField(nameof(Planeswalker.Ability3)).OfTypeText().BeingNonNullable().And
                .HaveField(nameof(Planeswalker.SerialNumber)).OfTypeUInt32().BeingNonNullable().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void PropertyTypeIsInconvertibleToSourceType_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Jedi);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Jedi.MiddleName))                     // error location
                .WithMessageContaining("[DataConverter]")                           // details / explanation
                .WithMessageContaining(nameof(Boolean))                             // details / explanation
                .WithMessageContaining(nameof(String));                             // details / explanation
        }

        [TestMethod] public void PropertyTypeIsConvertibleToSourceType_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(ConstitutionalAmendment);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(ConstitutionalAmendment.Number))      // error location
                .WithMessageContaining("[DataConverter]")                           // details / explanation
                .WithMessageContaining(nameof(Int64))                               // details / explanation
                .WithMessageContaining(nameof(Int32));                              // details / explanation
        }

        [TestMethod] public void TargetTypeIsNotSupported_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(SNLEpisode);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(SNLEpisode.AirDate))                  // error location
                .WithMessageContaining("[DataConverter]")                           // details / explanation
                .WithMessageContaining("type*is not supported")                     // details / explanation
                .WithMessageContaining(nameof(Exception));                          // details / explanation
        }

        [TestMethod] public void DataConverterTypeDoesNotImplementInterface_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(MetraRoute);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(MetraRoute.Line))                     // error location
                .WithMessageContaining("[DataConverter]")                           // details / explanation
                .WithMessageContaining("does not implement")                        // details / explanation
                .WithMessageContaining(nameof(Int32))                               // details / explanation
                .WithMessageContaining(nameof(IDataConverter));                     // details / explanation
        }

        [TestMethod] public void DataConverterTypeCannotBeDefaultConstructed_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Paycheck);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Paycheck.HoursWorked))                // error location
                .WithMessageContaining("[DataConverter]")                           // details / explanation
                .WithMessageContaining("does not have a default*constructor")       // details / explanation
                .WithMessageContaining(nameof(ChangeBase));                         // details / explanation
        }

        [TestMethod] public void DataConverterTypeThrowsOnConstruction_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Sword);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Sword.YearForged))                    // error location
                .WithMessageContaining("[DataConverter]")                           // details / explanation
                .WithMessageContaining("error constructing")                        // details / explanation
                .WithMessageContaining(nameof(Unconstructible<short>))              // details / explanation
                .WithMessageContaining(CANNOT_CONSTRUCT_MSG);                       // details / explanation
        }
    }
}
