using FluentAssertions;
using Kvasir.Core;
using Kvasir.Exceptions;
using Kvasir.Relations;
using Kvasir.Schema;
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

        [TestMethod] public void EnumerationToUnrelatedType() {
            // Arrange
            var translator = new Translator();
            var source = typeof(HomeRunDerby);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField(nameof(HomeRunDerby.Year)).OfTypeUInt32().BeingNonNullable().And
                .HaveField(nameof(HomeRunDerby.Victor)).OfTypeText().BeingNonNullable().And
                .HaveField(nameof(HomeRunDerby.TotalHomers)).OfTypeUInt16().BeingNonNullable().And
                .HaveField(nameof(HomeRunDerby.LongestHomer)).OfTypeUInt16().BeingNonNullable().And
                .HaveField(nameof(HomeRunDerby.CharityMoney)).OfTypeDecimal().BeingNonNullable().And
                .HaveField(nameof(HomeRunDerby.Structure)).OfTypeDateTime().BeingNonNullable().And
                .HaveNoOtherFields().And
                .HaveConstraint(nameof(HomeRunDerby.Structure), InclusionOperator.In,
                    new DateTime(2000, 1, 1),
                    new DateTime(2000, 1, 2),
                    new DateTime(2000, 1, 3),
                    new DateTime(2000, 1, 4)
                ).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void EnumerationToNumericBackingType() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Quarterback);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField(nameof(Quarterback.Name)).OfTypeText().BeingNonNullable().And
                .HaveField(nameof(Quarterback.ThrowingArm)).OfTypeInt8().BeingNonNullable().And
                .HaveField(nameof(Quarterback.DraftRound)).OfTypeInt16().BeingNonNullable().And
                .HaveField(nameof(Quarterback.QBStyle)).OfTypeInt32().BeingNonNullable().And
                .HaveField(nameof(Quarterback.CareerAchievements)).OfTypeInt64().BeingNonNullable().And
                .HaveField(nameof(Quarterback.Status)).OfTypeUInt8().BeingNonNullable().And
                .HaveField(nameof(Quarterback.HasBeenTraded)).OfTypeUInt16().BeingNonNullable().And
                .HaveField(nameof(Quarterback.FurthestPlayoffAdvancement)).OfTypeUInt32().BeingNonNullable().And
                .HaveField(nameof(Quarterback.Leagues)).OfTypeUInt64().BeingNonNullable().And
                .HaveNoOtherFields().And
                .HaveConstraint(nameof(Quarterback.ThrowingArm), InclusionOperator.In,
                    (sbyte)0, (sbyte)1
                ).And
                .HaveConstraint(nameof(Quarterback.DraftRound), InclusionOperator.In,
                    (short)14, (short)188, (short)2, (short)-16, (short)19054, (short)-333, (short)0, (short)8
                ).And
                .HaveConstraint(nameof(Quarterback.QBStyle), InclusionOperator.In,
                    0, 1, 2, 3
                ).And
                .HaveConstraint(nameof(Quarterback.CareerAchievements), InclusionOperator.In,
                    1L, 2L, 3L, 4L, 5L, 6L, 7L, 8L, 9L, 10L, 11L, 12L, 13L, 14L, 15L, 16L, 17L, 18L, 19L, 20L, 21L, 22L,
                    23L, 24L, 25L, 26L, 27L, 28L, 29L, 30L, 31L
                ).And
                .HaveConstraint(nameof(Quarterback.Status), InclusionOperator.In,
                    (byte)0, (byte)1, (byte)2, (byte)3, (byte)4
                ).And
                .HaveConstraint(nameof(Quarterback.HasBeenTraded), InclusionOperator.In,
                    (ushort)0, (ushort)1
                ).And
                .HaveConstraint(nameof(Quarterback.FurthestPlayoffAdvancement), InclusionOperator.In,
                    0U, 1827412U, 44U, 949012U, 55U
                ).And
                .HaveConstraint(nameof(Quarterback.Leagues), InclusionOperator.In,
                    2UL, 64UL, 66UL, 128UL, 130UL, 192UL, 194UL
                ).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void EnumerationToString() {
            // Arrange
            var translator = new Translator();
            var source = typeof(EcumenicalCouncil);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField(nameof(EcumenicalCouncil.Name)).OfTypeText().BeingNonNullable().And
                .HaveField(nameof(EcumenicalCouncil.Opening)).OfTypeDateTime().BeingNonNullable().And
                .HaveField(nameof(EcumenicalCouncil.Closing)).OfTypeDateTime().BeingNonNullable().And
                .HaveField(nameof(EcumenicalCouncil.RecognizedBy)).OfTypeText().BeingNonNullable().And
                .HaveField(nameof(EcumenicalCouncil.Attendance)).OfTypeUInt32().BeingNonNullable().And
                .HaveNoOtherFields().And
                .HaveConstraint(nameof(EcumenicalCouncil.RecognizedBy), InclusionOperator.In,
                    "CatholicChurch", "EasternOrthodoxChurch", "Unrecognized"
                ).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void ConverterOnAggregateField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Joust);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Joust.KnightB))                       // error location
                .WithMessageContaining("[DataConverter]")                           // details / explanation
                .WithMessageContaining(nameof(Joust.Person))                        // details / explanation
                .WithMessageContaining("neither a scalar nor an enumeration");      // details / explanation
        }

        [TestMethod] public void ConverterOnReferenceField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Decathlon);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Decathlon.Winner))                    // error location
                .WithMessageContaining("[DataConverter]")                           // details / explanation
                .WithMessageContaining(nameof(Decathlon.Athlete))                   // details / explanation
                .WithMessageContaining("neither a scalar nor an enumeration");      // details / explanation
        }

        [TestMethod] public void ConverterOnRelationField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Bank);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Bank.Accounts))                       // error location
                .WithMessageContaining("[DataConverter]")                           // details / explanation
                .WithMessageContaining(nameof(RelationMap<ulong, decimal>))         // details / explanation
                .WithMessageContaining("neither a scalar nor an enumeration");      // details / explanation
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

        [TestMethod] public void DataConverterTypeThrowsOnConversion_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Ligament);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Ligament.Classification))             // error location
                .WithMessageContaining("[DataConverter]")                           // details / explanation
                .WithMessageContaining("error converting")                          // details / explanation
                .WithMessageContaining("Type.Articular")                            // details / explanation
                .WithMessageContaining(CANNOT_CONVERT_MSG);                         // details / explanation
        }

        [TestMethod] public void NumericConverterOnBooleanField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Pillow);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Pillow.IsThrowPillow))                // error location
                .WithMessageContaining("[Numeric]")                                 // details / explanation
                .WithMessageContaining(nameof(Boolean))                             // details / explanation
                .WithMessageContaining("enumeration");                              // details / explanation
        }

        [TestMethod] public void NumericConverterOnTextualField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(VigenereCipher);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(VigenereCipher.Key))                  // error location
                .WithMessageContaining("[Numeric]")                                 // details / explanation
                .WithMessageContaining(nameof(String))                              // details / explanation
                .WithMessageContaining("enumeration");                              // details / explanation
        }

        [TestMethod] public void NumericConverterOnNumericField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Satellite);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Satellite.OrbitsCompleted))           // error location
                .WithMessageContaining("[Numeric]")                                 // details / explanation
                .WithMessageContaining(nameof(UInt64))                              // details / explanation
                .WithMessageContaining("enumeration");                              // details / explanation
        }

        [TestMethod] public void NumericConverterOnDateTimeField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Symphony);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Symphony.PremiereDate))               // error location
                .WithMessageContaining("[Numeric]")                                 // details / explanation
                .WithMessageContaining(nameof(DateTime))                            // details / explanation
                .WithMessageContaining("enumeration");                              // details / explanation
        }

        [TestMethod] public void NumericConverterOnGuidField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(WordSearch);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(WordSearch.PuzzleID))                 // error location
                .WithMessageContaining("[Numeric]")                                 // details / explanation
                .WithMessageContaining(nameof(Guid))                                // details / explanation
                .WithMessageContaining("enumeration");                              // details / explanation
        }

        [TestMethod] public void NumericConverterOnAggregateField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(GolfCourse);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(GolfCourse.Hole14))                   // error location
                .WithMessageContaining("[Numeric]")                                 // details / explanation
                .WithMessageContaining(nameof(GolfCourse.Hole))                     // details / explanation
                .WithMessageContaining("enumeration");                              // details / explanation
        }

        [TestMethod] public void NumericConverterOnReferenceField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(SlamBallMatch);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(SlamBallMatch.Defeated))              // error location
                .WithMessageContaining("[Numeric]")                                 // details / explanation
                .WithMessageContaining(nameof(SlamBallMatch.SlamBallTeam))          // details / explanation
                .WithMessageContaining("enumeration");                              // details / explanation
        }

        [TestMethod] public void NumericConverterOnRelationField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Wadi);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Wadi.MineralDeposits))                // error location
                .WithMessageContaining("[Numeric]")                                 // details / explanation
                .WithMessageContaining(nameof(RelationSet<string>))                 // details / explanation
                .WithMessageContaining("enumeration");                              // details / explanation
        }

        [TestMethod] public void AsStringConverterOnBooleanField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(BondGirl);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(BondGirl.SleptWithBond))              // error location
                .WithMessageContaining("[AsString]")                                // details / explanation
                .WithMessageContaining(nameof(Boolean))                             // details / explanation
                .WithMessageContaining("enumeration");                              // details / explanation
        }

        [TestMethod] public void AsStringConverterOnTextualField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(BatmanVillain);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(BatmanVillain.Grade))                 // error location
                .WithMessageContaining("[AsString]")                                // details / explanation
                .WithMessageContaining(nameof(Char))                                // details / explanation
                .WithMessageContaining("enumeration");                              // details / explanation
        }

        [TestMethod] public void AsStringConverterOnNumericField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Cemetery);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Cemetery.Latitude))                   // error location
                .WithMessageContaining("[AsString]")                                // details / explanation
                .WithMessageContaining(nameof(Double))                              // details / explanation
                .WithMessageContaining("enumeration");                              // details / explanation
        }

        [TestMethod] public void AsStringConverterOnDateTimeField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(ImmaculateGrid);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(ImmaculateGrid.Date))                 // error location
                .WithMessageContaining("[AsString]")                                // details / explanation
                .WithMessageContaining(nameof(DateTime))                            // details / explanation
                .WithMessageContaining("enumeration");                              // details / explanation
        }

        [TestMethod] public void AsStringConverterOnGuidField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Eyeglasses);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Eyeglasses.GlassesID))                // error location
                .WithMessageContaining("[AsString]")                                // details / explanation
                .WithMessageContaining(nameof(Guid))                                // details / explanation
                .WithMessageContaining("enumeration");                              // details / explanation
        }

        [TestMethod] public void AsStringConverterOnAggregateField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Windmill);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Windmill.EnergyGenerated))            // error location
                .WithMessageContaining("[AsString]")                                // details / explanation
                .WithMessageContaining(nameof(Windmill.EnergyOutput))               // details / explanation
                .WithMessageContaining("enumeration");                              // details / explanation
        }

        [TestMethod] public void AsStringConverterOnReferenceField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Chakra);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Chakra.AssociatedYogini))             // error location
                .WithMessageContaining("[AsString]")                                // details / explanation
                .WithMessageContaining(nameof(Chakra.Yogini))                       // details / explanation
                .WithMessageContaining("enumeration");                              // details / explanation
        }

        [TestMethod] public void AsStringConverterOnRelationField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Cryptogram);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Cryptogram.Solution))                 // error location
                .WithMessageContaining("[AsString]")                                // details / explanation
                .WithMessageContaining(nameof(RelationMap<char, char>))             // details / explanation
                .WithMessageContaining("enumeration");                              // details / explanation
        }

        [TestMethod] public void DataConverterAndNumeric_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(SecretHitlerGame);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(SecretHitlerGame.Player7))            // error location
                .WithMessageContaining("mutually exclusive")                        // category
                .WithMessageContaining("[DataConverter]")                           // details / explanation
                .WithMessageContaining("[Numeric]");                                // details / explanation
        }

        [TestMethod] public void DataConverterAndAsString_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Mezuzah);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Mezuzah.MadeOf))                      // error location
                .WithMessageContaining("mutually exclusive")                        // category
                .WithMessageContaining("[DataConverter]")                           // details / explanation
                .WithMessageContaining("[AsString]");                               // details / explanation
        }

        [TestMethod] public void NumericAndAsString_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Atoll);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Atoll.Ocean))                         // error location
                .WithMessageContaining("mutually exclusive")                        // category
                .WithMessageContaining("[Numeric]")                                 // details / explanation
                .WithMessageContaining("[AsString]");                               // details / explanation
        }
    }
}
