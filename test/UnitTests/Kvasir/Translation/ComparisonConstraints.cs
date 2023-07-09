using FluentAssertions;
using Kvasir.Exceptions;
using Kvasir.Schema;
using Kvasir.Translation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

using static UT.Kvasir.Translation.ComparisonConstraints.IsGreaterThan;
using static UT.Kvasir.Translation.ComparisonConstraints.IsLessThan;
using static UT.Kvasir.Translation.ComparisonConstraints.IsGreaterOrEqualTo;
using static UT.Kvasir.Translation.ComparisonConstraints.IsLessOrEqualTo;
using static UT.Kvasir.Translation.ComparisonConstraints.IsNot;

namespace UT.Kvasir.Translation {
    [TestClass, TestCategory("Constraints - Comparison")]
    public class IsGreaterThanTests {
        [TestMethod] public void IsGreaterThan_NumericFields() {
            // Arrange
            var translator = new Translator();
            var source = typeof(DNDSpell);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(DNDSpell.Range), ComparisonOperator.GT, (ushort)0).And
                .HaveConstraint(nameof(DNDSpell.Level), ComparisonOperator.GT, -1).And
                .HaveConstraint(nameof(DNDSpell.AverageDamage), ComparisonOperator.GT, 2.5f).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsGreaterThan_TextualFields() {
            // Arrange
            var translator = new Translator();
            var source = typeof(MultipleChoiceQuestion);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(MultipleChoiceQuestion.CorrectAnswer), ComparisonOperator.GT, '*').And
                .HaveConstraint(nameof(MultipleChoiceQuestion.ChoiceA), ComparisonOperator.GT, "A. ").And
                .HaveConstraint(nameof(MultipleChoiceQuestion.ChoiceB), ComparisonOperator.GT, "B. ").And
                .HaveConstraint(nameof(MultipleChoiceQuestion.ChoiceC), ComparisonOperator.GT, "C. ").And
                .HaveConstraint(nameof(MultipleChoiceQuestion.ChoiceD), ComparisonOperator.GT, "D. ").And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsGreaterThan_BooleanField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Font);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Font.HasSerifs))                      // error location
                .WithMessageContaining("constraint is inapplicable")                // category
                .WithMessageContaining("[Check.IsGreaterThan]")                     // details / explanation
                .WithMessageContaining("totally ordered")                           // details / explanation
                .WithMessageContaining(nameof(Boolean));                            // details / explanation
        }

        [TestMethod] public void IsGreaterThan_DecimalField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(AuctionLot);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(AuctionLot.TopBid), ComparisonOperator.GT, (decimal)10000).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsGreaterThan_DateTimeField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(GoldRush);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(GoldRush.StartDate), ComparisonOperator.GT, new DateTime(1200, 3, 18)).And
                .HaveConstraint(nameof(GoldRush.EndDate), ComparisonOperator.GT, new DateTime(1176, 11, 22)).And
                .HaveNoOtherCandidateKeys();
        }

        [TestMethod] public void IsGreaterThan_GuidField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Skyscraper);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Skyscraper.RegistryIdentifier))       // error location
                .WithMessageContaining("constraint is inapplicable")                // category
                .WithMessageContaining("[Check.IsGreaterThan]")                     // details / explanation
                .WithMessageContaining("totally ordered")                           // details / explanation
                .WithMessageContaining(nameof(Guid));                               // details / explanation
        }

        [TestMethod] public void IsGreaterThan_EnumerationField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Orisha);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Orisha.BelongsTo))                    // error location
                .WithMessageContaining("constraint is inapplicable")                // category
                .WithMessageContaining("[Check.IsGreaterThan]")                     // details / explanation
                .WithMessageContaining("totally ordered")                           // details / explanation
                .WithMessageContaining(nameof(Orisha.Culture));                     // details / explanation
        }

        [TestMethod] public void IsGreaterThan_NestedApplicableScalar() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Opioid);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("Definition.DrugBank", ComparisonOperator.GT, "XP14U339D").And
                .HaveConstraint("Definition.Formula.O", ComparisonOperator.GT, 2).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsGreaterThan_NestedInapplicableScalar_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Wordle);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Wordle.Guess3))                       // error location
                .WithMessageContaining("\"L4.Hint\"")                               // nested path
                .WithMessageContaining("constraint is inapplicable")                // category
                .WithMessageContaining("[Check.IsGreaterThan]")                     // details / explanation
                .WithMessageContaining("totally ordered")                           // details / explanation
                .WithMessageContaining(nameof(Wordle.Result));                      // details / explanation
        }

        [TestMethod] public void IsGreaterThan_NestedAggregate_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(FlashMob);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(FlashMob.Participants))               // error location
                .WithMessageContaining("refers to a non-scalar")                    // category
                .WithMessageContaining("[Check.IsGreaterThan]")                     // details / explanation
                .WithMessageContaining("\"Leader\"");                               // details / explanation
        }

        [TestMethod] public void IsGreaterThan_NullableTotallyOrderedFields() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Baryon);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(Baryon.Symbol), ComparisonOperator.GT, '-').And
                .HaveConstraint(nameof(Baryon.Charge), ComparisonOperator.GT, (short)-5).And
                .HaveConstraint(nameof(Baryon.Discovered), ComparisonOperator.GT, new DateTime(1344, 6, 21)).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsGreaterThan_InconvertibleAnchor_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Racehorse);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Racehorse.FirstDerbyWin))             // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Check.IsGreaterThan]")                     // details / explanation
                .WithMessageContaining($"true of type {nameof(Boolean)}")           // details / explanation
                .WithMessageContaining(nameof(UInt64));                             // details / explanation
        }

        [TestMethod] public void IsGreaterThan_ConvertibleAnchor_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(ChineseCharacter);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(ChineseCharacter.Character))          // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Check.IsGreaterThan]")                     // details / explanation
                .WithMessageContaining($"14 of type {nameof(Byte)}")                // details / explanation
                .WithMessageContaining(nameof(Char));                               // details / explanation
        }

        [TestMethod] public void IsGreaterThan_ArrayAnchor_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Query);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Query.WHERE))                         // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Check.IsGreaterThan]")                     // details / explanation
                .WithMessageContaining("array")                                     // details / explanation
                .WithMessageContaining(nameof(String));                             // details / explanation
        }

        [TestMethod] public void IsGreaterThan_NullAnchor_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(UNResolution);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(UNResolution.NumSignatories))         // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Check.IsGreaterThan]")                     // details / explanation
                .WithMessageContaining("null");                                     // details / explanation
        }

        [TestMethod] public void IsGreaterThan_AnchorIsMaximum_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Upanishad);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Upanishad.Index))                     // error location
                .WithMessageContaining("constraint is unsatisfiable")               // category
                .WithMessageContaining("[Check.IsGreaterThan]")                     // details / explanation
                .WithMessageContaining("exclusive lower bound")                     // details / explanation
                .WithMessageContaining($"maximum value {sbyte.MaxValue}")           // details / explanation
                .WithMessageContaining(nameof(SByte));                              // details / explanation
        }

        [TestMethod] public void IsGreaterThan_DecimalAnchorIsNotDouble_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(GarageSale);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(GarageSale.Gross))                    // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Check.IsGreaterThan]")                     // details / explanation
                .WithMessageContaining($"200 of type {nameof(Int32)}")              // details / explanation
                .WithMessageContaining(nameof(Decimal))                             // details / explanation
                .WithMessageContaining(nameof(Double));                             // details / explanation
        }

        [TestMethod] public void IsGreaterThan_DecimalAnchorIsOutOfRange_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(TalkShow);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(TalkShow.Rating))                     // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Check.IsGreaterThan]")                     // details / explanation
                .WithMessageContaining(double.MinValue.ToString())                  // details / explanation
                .WithMessageContaining("could not convert")                         // details / explanation
                .WithMessageContaining(nameof(Decimal));                            // details / explanation
        }

        [TestMethod] public void IsGreaterThan_DateTimeAnchorIsNotString_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Meme);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Meme.FirstPublished))                 // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Check.IsGreaterThan]")                     // details / explanation
                .WithMessageContaining($"\"NEVER\" of type {nameof(String)}")       // details / explanation
                .WithMessageContaining(nameof(DateTime))                            // details / explanation
                .WithMessageContaining(nameof(String));                             // details / explanation
        }

        [TestMethod] public void IsGreaterThan_DateTimeAnchorIsMalformatted_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(ChristianDenomination);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(ChristianDenomination.Founded))       // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Check.IsGreaterThan]")                     // details / explanation
                .WithMessageContaining("\"0001_01_01\"")                            // details / explanation
                .WithMessageContaining("could not parse")                           // details / explanation
                .WithMessageContaining(nameof(DateTime));                           // details / explanation
        }

        [TestMethod] public void IsGreaterThan_DateTimeAnchorIsOutOfRange_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(GraduateThesis);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(GraduateThesis.Argued))               // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Check.IsGreaterThan]")                     // details / explanation
                .WithMessageContaining("\"1873-15-12\"")                            // details / explanation
                .WithMessageContaining("could not parse")                           // details / explanation
                .WithMessageContaining(nameof(DateTime));                           // details / explanation
        }

        [TestMethod] public void IsGreaterThan_AnchorMatchesDataConversionSourceType_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Azeotrope);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Azeotrope.BoilingPoint))              // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Check.IsGreaterThan]")                     // details / explanation
                .WithMessageContaining($"-237.44 of type {nameof(Single)}")         // details / explanation
                .WithMessageContaining(nameof(String));                             // details / explanation
        }

        [TestMethod] public void IsGreaterThan_AnchorMatchesDataConversionTargetType() {
            // Arrange
            var translator = new Translator();
            var source = typeof(BingoCard);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(BingoCard.CellR4C1), ComparisonOperator.GT, "-1").And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsGreaterThan_ScalarConstrainedMultipleTimes() {
            // Arrange
            var translator = new Translator();
            var source = typeof(NuclearPowerPlant);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(NuclearPowerPlant.Meltdowns), ComparisonOperator.GT, 37L).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsGreaterThan_PathIsNull_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Domino);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Domino.RightPips))                    // error location
                .WithMessageContaining("path is null")                              // category
                .WithMessageContaining("[Check.IsGreaterThan]");                    // details / explanation
        }

        [TestMethod] public void IsGreaterThan_PathOnScalar_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Canyon);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Canyon.Depth))                        // error location
                .WithMessageContaining("path*does not exist")                       // category
                .WithMessageContaining("[Check.IsGreaterThan]")                     // details / explanation
                .WithMessageContaining("\"---\"");                                  // details / explanation
        }

        [TestMethod] public void IsGreaterThan_NonExistentPathOnAggregate_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Conlang);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Conlang.Codes))                       // error location
                .WithMessageContaining("path*does not exist")                       // category
                .WithMessageContaining("[Check.IsGreaterThan]")                     // details / explanation
                .WithMessageContaining("\"---\"");                                  // details / explanation
        }

        [TestMethod] public void IsGreaterThan_NoPathOnAggregate_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(LaborStrike);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(LaborStrike.Members))                 // error location
                .WithMessageContaining("path is required")                          // category
                .WithMessageContaining("[Check.IsGreaterThan]");                    // details / explanation
        }

        [TestMethod] public void IsGreaterThan_DefaultValueDoesNotSatisfyConstraint_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(DraftPick);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(DraftPick.Overall))                   // error location
                .WithMessageContaining("default*does not satisfy constraints")      // category
                .WithMessageContaining("one or more [Check.xxx] constraints")       // details / explanation
                .WithMessageContaining("0")                                         // details / explanation
                .WithMessageContaining("is not in interval (0, +∞)");               // details / explanation
        }
    }

    [TestClass, TestCategory("Constraints - Comparison")]
    public class IsLessThanTests {
        [TestMethod] public void IsLessThan_NumericFields() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Resistor);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(Resistor.Resistance), ComparisonOperator.LT, 27814L).And
                .HaveConstraint(nameof(Resistor.PhysicalLength), ComparisonOperator.LT, 893.44501f).And
                .HaveConstraint(nameof(Resistor.Power), ComparisonOperator.LT, 27814UL).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsLessThan_TextualFields() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Senator);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(Senator.LastName), ComparisonOperator.LT, "...").And
                .HaveConstraint(nameof(Senator.NRARating), ComparisonOperator.LT, 'G').And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsLessThan_BooleanField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Milkshake);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Milkshake.IsDairyFree))               // error location
                .WithMessageContaining("constraint is inapplicable")                // category
                .WithMessageContaining("[Check.IsLessThan]")                        // details / explanation
                .WithMessageContaining("totally ordered")                           // details / explanation
                .WithMessageContaining(nameof(Boolean));                            // details / explanation
        }

        [TestMethod] public void IsLessThan_DecimalField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(TreasuryBond);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(TreasuryBond.BoughtFor), ComparisonOperator.LT, (decimal)57182391.33167994).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsLessThan_DateTimeField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Commercial);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(Commercial.TimeSlot), ComparisonOperator.LT, new DateTime(2300, 1, 1)).And
                .HaveNoOtherCandidateKeys();
        }

        [TestMethod] public void IsLessThan_GuidField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(DLL);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(DLL.ID))                              // error location
                .WithMessageContaining("constraint is inapplicable")                // category
                .WithMessageContaining("[Check.IsLessThan]")                        // details / explanation
                .WithMessageContaining("totally ordered")                           // details / explanation
                .WithMessageContaining(nameof(Guid));                               // details / explanation
        }

        [TestMethod] public void IsLessThan_EnumerationField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(SolicitorGeneral);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(SolicitorGeneral.Affiliation))        // error location
                .WithMessageContaining("constraint is inapplicable")                // category
                .WithMessageContaining("[Check.IsLessThan]")                        // details / explanation
                .WithMessageContaining("totally ordered")                           // details / explanation
                .WithMessageContaining(nameof(SolicitorGeneral.PoliticalParty));    // details / explanation
        }

        [TestMethod] public void IsLessThan_NestedApplicableScalar() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Raptor);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("Scientific.Family", ComparisonOperator.LT, "Zynovia").And
                .HaveConstraint("Measurements.Weight", ComparisonOperator.LT, 174.991).And
                .HaveConstraint("Measurements.TopSpeed", ComparisonOperator.LT, long.MaxValue).And
                .HaveConstraint("Measurements.Wingspan", ComparisonOperator.LT, (ushort)489).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsLessThan_NestedInapplicableScalar_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Feruchemy);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Feruchemy.Effects))                   // error location
                .WithMessageContaining("\"Kind\"")                                  // nested path
                .WithMessageContaining("constraint is inapplicable")                // category
                .WithMessageContaining("[Check.IsLessThan]")                        // details / explanation
                .WithMessageContaining("totally ordered")                           // details / explanation
                .WithMessageContaining(nameof(Feruchemy.Matrix));                   // details / explanation
        }

        [TestMethod] public void IsLessThan_NestedAggregate_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Firefighter);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Firefighter.Firehouse))               // error location
                .WithMessageContaining("refers to a non-scalar")                    // category
                .WithMessageContaining("[Check.IsLessThan]")                        // details / explanation
                .WithMessageContaining("\"ServiceArea\"");                          // details / explanation
        }

        [TestMethod] public void IsLessThan_NullableTotallyOrderedFields() {
            // Arrange
            var translator = new Translator();
            var source = typeof(AutoRacetrack);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(AutoRacetrack.Nickname), ComparisonOperator.LT, "Zytrotzko").And
                .HaveConstraint(nameof(AutoRacetrack.TrackLength), ComparisonOperator.LT, 12000000L).And
                .HaveConstraint(nameof(AutoRacetrack.LastRace), ComparisonOperator.LT, new DateTime(4319, 2, 21)).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsLessThan_InconvertibleAnchor_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Distribution);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Distribution.Mode))                   // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Check.IsLessThan]")                        // details / explanation
                .WithMessageContaining($"\"Zero\" of type {nameof(String)}")        // details / explanation
                .WithMessageContaining(nameof(Double));                             // details / explanation
        }

        [TestMethod] public void IsLessThan_ConvertibleAnchor_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(WebBrowser);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(WebBrowser.MarketShare))              // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Check.IsLessThan]")                        // details / explanation
                .WithMessageContaining($"100 of type {nameof(Int32)}")              // details / explanation
                .WithMessageContaining(nameof(Single));                             // details / explanation
        }

        [TestMethod] public void IsLessThan_ArrayAnchor_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(GrammaticalCase);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(GrammaticalCase.Affix))               // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Check.IsLessThan]")                        // details / explanation
                .WithMessageContaining("array")                                     // details / explanation
                .WithMessageContaining(nameof(Char));                               // details / explanation
        }

        [TestMethod] public void IsLessThan_NullAnchor_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(PowerPointAnimation);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(PowerPointAnimation.Duration))        // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Check.IsLessThan]")                        // details / explanation
                .WithMessageContaining("null");                                     // details / explanation
        }

        [TestMethod] public void IsLessThan_AnchorIsMinimum_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(StrategoPiece);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(StrategoPiece.Value))                 // error location
                .WithMessageContaining("constraint is unsatisfiable")               // category
                .WithMessageContaining("[Check.IsLessThan]")                        // details / explanation
                .WithMessageContaining("exclusive upper bound")                     // details / explanation
                .WithMessageContaining($"minimum value {uint.MinValue}")            // details / explanation
                .WithMessageContaining(nameof(UInt32));                             // details / explanation
        }

        [TestMethod] public void IsLessThan_DecimalAnchorIsNotDouble_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Toothpaste);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Toothpaste.Efficacy))                 // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Check.IsLessThan]")                        // details / explanation
                .WithMessageContaining($"\"100%\" of type {nameof(String)}")        // details / explanation
                .WithMessageContaining(nameof(Decimal))                             // details / explanation
                .WithMessageContaining(nameof(Double));                             // details / explanation
        }

        [TestMethod] public void IsLessThan_DecimalAnchorIsOutOfRange_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Census);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Census.PercentIndian))                // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Check.IsLessThan]")                        // details / explanation
                .WithMessageContaining(double.MaxValue.ToString())                  // details / explanation
                .WithMessageContaining("could not convert")                         // details / explanation
                .WithMessageContaining(nameof(Decimal));                            // details / explanation
        }

        [TestMethod] public void IsLessThan_DateTimeAnchorIsNotString_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(NobelPrize);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(NobelPrize.Awarded))                  // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Check.IsLessThan]")                        // details / explanation
                .WithMessageContaining($"37 of type {nameof(SByte)}")               // details / explanation
                .WithMessageContaining(nameof(DateTime))                            // details / explanation
                .WithMessageContaining(nameof(String));                             // details / explanation
        }

        [TestMethod] public void IsLessThan_DateTimeAnchorIsMalformatted_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Shogunate);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Shogunate.Established))               // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Check.IsLessThan]")                        // details / explanation
                .WithMessageContaining("\"Wednesday, August 18, 1988\"")            // details / explanation
                .WithMessageContaining("could not parse")                           // details / explanation
                .WithMessageContaining(nameof(DateTime));                           // details / explanation
        }

        [TestMethod] public void IsLessThan_DateTimeAnchorIsOutOfRange_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(ISOStandard);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(ISOStandard.Adopted))                 // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Check.IsLessThan]")                        // details / explanation
                .WithMessageContaining("\"1735-02-48\"")                            // details / explanation
                .WithMessageContaining("could not parse")                           // details / explanation
                .WithMessageContaining(nameof(DateTime));                           // details / explanation
        }

        [TestMethod] public void IsLessThan_AnchorMatchesDataConversionSourceType_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Artiodactyl);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Artiodactyl.NumToes))                 // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Check.IsLessThan]")                        // details / explanation
                .WithMessageContaining($"8 of type {nameof(Byte)}")                 // details / explanation
                .WithMessageContaining(nameof(Int32));                              // details / explanation
        }

        [TestMethod] public void IsLessThan_AnchorMatchesDataConversionTargetType() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Phobia);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(Phobia.Prevalence), ComparisonOperator.LT, "100.00001").And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsLessThan_ScalarConstrainedMultipleTimes() {
            // Arrange
            var translator = new Translator();
            var source = typeof(CinemaSins);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(CinemaSins.SinCount), ComparisonOperator.LT, 1712312389UL).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsLessThan_PathIsNull_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(BaseballBat);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(BaseballBat.Weight))                  // error location
                .WithMessageContaining("path is null")                              // category
                .WithMessageContaining("[Check.IsLessThan]");                       // details / explanation
        }

        [TestMethod] public void IsLessThan_PathOnScalar_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Potato);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Potato.Weight))                       // error location
                .WithMessageContaining("path*does not exist")                       // category
                .WithMessageContaining("[Check.IsLessThan]")                        // details / explanation
                .WithMessageContaining("\"---\"");                                  // details / explanation
        }

        [TestMethod] public void IsLessThan_NonExistentPathOnAggregate_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(SurgicalMask);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(SurgicalMask.ID))                     // error location
                .WithMessageContaining("path*does not exist")                       // category
                .WithMessageContaining("[Check.IsLessThan]")                        // details / explanation
                .WithMessageContaining("\"---\"");                                  // details / explanation
        }

        [TestMethod] public void IsLessThan_NoPathOnAggregate_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(SecretSociety);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(SecretSociety.Initiation))            // error location
                .WithMessageContaining("path is required")                          // category
                .WithMessageContaining("[Check.IsLessThan]");                       // details / explanation
        }

        [TestMethod] public void IsLessThan_DefaultValueDoesNotSatisfyConstraint_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(ParkingGarage);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(ParkingGarage.CostPerHour))           // error location
                .WithMessageContaining("default*does not satisfy constraints")      // category
                .WithMessageContaining("one or more [Check.xxx] constraints")       // details / explanation
                .WithMessageContaining("15.0")                                      // details / explanation
                .WithMessageContaining("is not in interval (-∞, 10.0)");            // details / explanation
        }
    }

    [TestClass, TestCategory("Constraints - Comparison")]
    public class IsGreaterOrEqualToTests {
        [TestMethod] public void IsGreaterOrEqualTo_NumericFields() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Geyser);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(Geyser.EruptionHeight), ComparisonOperator.GTE, 0L).And
                .HaveConstraint(nameof(Geyser.Elevation), ComparisonOperator.GTE, 0f).And
                .HaveConstraint(nameof(Geyser.EruptionDuration), ComparisonOperator.GTE, 0U).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsGreaterOrEqualTo_TextualFields() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Hotel);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(Hotel.HotelName), ComparisonOperator.GTE, "").And
                .HaveConstraint(nameof(Hotel.Stars), ComparisonOperator.GTE, '1').And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsGreaterOrEqualTo_BooleanField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Steak);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Steak.FromSteakhouse))                // error location
                .WithMessageContaining("constraint is inapplicable")                // category
                .WithMessageContaining("[Check.IsGreaterOrEqualTo]")                // details / explanation
                .WithMessageContaining("totally ordered")                           // details / explanation
                .WithMessageContaining(nameof(Boolean));                            // details / explanation
        }

        [TestMethod] public void IsGreaterOrEqualTo_DecimalField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(ETF);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(ETF.ClosingPrice), ComparisonOperator.GTE, (decimal)-18.412006).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsGreaterOrEqualTo_DateTimeField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(PEP);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(PEP.CreatedOn), ComparisonOperator.GTE, new DateTime(1887, 4, 29)).And
                .HaveNoOtherCandidateKeys();
        }

        [TestMethod] public void IsGreaterOrEqualTo_GuidField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(CoatOfArms);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(CoatOfArms.ID))                       // error location
                .WithMessageContaining("constraint is inapplicable")                // category
                .WithMessageContaining("[Check.IsGreaterOrEqualTo]")                // details / explanation
                .WithMessageContaining("totally ordered")                           // details / explanation
                .WithMessageContaining(nameof(Guid));                               // details / explanation
        }

        [TestMethod] public void IsGreaterOrEqualTo_EnumerationField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(CivCityState);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(CivCityState.Type))                   // error location
                .WithMessageContaining("constraint is inapplicable")                // category
                .WithMessageContaining("[Check.IsGreaterOrEqualTo]")                // details / explanation
                .WithMessageContaining("totally ordered")                           // details / explanation
                .WithMessageContaining(nameof(CivCityState.Category));              // details / explanation
        }

        [TestMethod] public void IsGreaterOrEqualTo_NestedApplicableScalar() {
            // Arrange
            var translator = new Translator();
            var source = typeof(FamilyTree);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("Focal.FirstName", ComparisonOperator.GTE, "Tony").And
                .HaveConstraint("Focal.DOB", ComparisonOperator.GTE, new DateTime(1255, 9, 18)).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsGreaterOrEqualTo_NestedInapplicableScalar_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Readymade);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Readymade.Registration))              // error location
                .WithMessageContaining("\"IsFormallyRegistered\"")                  // nested path
                .WithMessageContaining("constraint is inapplicable")                // category
                .WithMessageContaining("[Check.IsGreaterOrEqualTo]")                // details / explanation
                .WithMessageContaining("totally ordered")                           // details / explanation
                .WithMessageContaining(nameof(Boolean));                            // details / explanation
        }

        [TestMethod] public void IsGreaterOrEqualTo_NestedAggregate_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(FitnessCenter);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(FitnessCenter.Address))               // error location
                .WithMessageContaining("refers to a non-scalar")                    // category
                .WithMessageContaining("[Check.IsGreaterOrEqualTo]")                // details / explanation
                .WithMessageContaining("\"Street\"");                               // details / explanation
        }

        [TestMethod] public void IsGreaterOrEqualTo_NullableTotallyOrderedFields() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Muscle);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(Muscle.TA2), ComparisonOperator.GTE, 10U).And
                .HaveConstraint(nameof(Muscle.Nerve), ComparisonOperator.GTE, "~~~").And
                .HaveConstraint(nameof(Muscle.FirstDocumented), ComparisonOperator.GTE, new DateTime(937, 12, 18)).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsGreaterOrEqualTo_InconvertibleAnchor_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(LandCard);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(LandCard.BlueManna))                  // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Check.IsGreaterOrEqualTo]")                // details / explanation
                .WithMessageContaining($"\"None\" of type {nameof(String)}")        // details / explanation
                .WithMessageContaining(nameof(Byte));                               // details / explanation
        }

        [TestMethod] public void IsGreaterOrEqualTo_ConvertibleAnchor_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Keystroke);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Keystroke.ResultingGlyph))            // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Check.IsGreaterOrEqualTo]")                // details / explanation
                .WithMessageContaining($"290 of type {nameof(Int32)}")              // details / explanation
                .WithMessageContaining(nameof(Char));                               // details / explanation
        }

        [TestMethod] public void IsGreaterOrEqualTo_ArrayAnchor_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Zoo);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Zoo.AverageVisitorsPerDay))           // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Check.IsGreaterOrEqualTo]")                // details / explanation
                .WithMessageContaining("array")                                     // details / explanation
                .WithMessageContaining(nameof(Single));                             // details / explanation
        }

        [TestMethod] public void IsGreaterOrEqualTo_NullAnchor_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Neurotoxin);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Neurotoxin.MolarMass))                // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Check.IsGreaterOrEqualTo]")                // details / explanation
                .WithMessageContaining("null");                                     // details / explanation
        }

        [TestMethod] public void IsGreaterOrEqualTo_AnchorIsMinimum_Redundant() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Bacterium);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(Bacterium.NumStrains), ComparisonOperator.GTE, ushort.MinValue).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsGreaterOrEqualTo_DecimalAnchorIsNotDouble_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(GitHook);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(GitHook.NumExecutions))               // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Check.IsGreaterOrEqualTo]")                // details / explanation
                .WithMessageContaining($"1.0 of type {nameof(Single)}")             // details / explanation
                .WithMessageContaining(nameof(Decimal))                             // details / explanation
                .WithMessageContaining(nameof(Double));                             // details / explanation
        }

        [TestMethod] public void IsGreaterOrEqualTo_DecimalAnchorIsOutOfRange_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(RubeGoldbergMachine);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(RubeGoldbergMachine.MaterialsCost))   // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Check.IsGreaterOrEqualTo]")                // details / explanation
                .WithMessageContaining(double.NegativeInfinity.ToString())          // details / explanation
                .WithMessageContaining("could not convert")                         // details / explanation
                .WithMessageContaining(nameof(Decimal));                            // details / explanation
        }

        [TestMethod] public void IsGreaterOrEqualTo_DateTimeAnchorIsNotString_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Smurf);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Smurf.FirstIntroduced))               // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Check.IsGreaterOrEqualTo]")                // details / explanation
                .WithMessageContaining($"318.909 of type {nameof(Single)}")         // details / explanation
                .WithMessageContaining(nameof(DateTime))                            // details / explanation
                .WithMessageContaining(nameof(String));                             // details / explanation
        }

        [TestMethod] public void IsGreaterOrEqualTo_DateTimeAnchorIsMalformatted_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(WorldCup);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(WorldCup.ChampionshipDate))           // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Check.IsGreaterOrEqualTo]")                // details / explanation
                .WithMessageContaining("\"1111(11)11\"")                            // details / explanation
                .WithMessageContaining("could not parse")                           // details / explanation
                .WithMessageContaining(nameof(DateTime));                           // details / explanation
        }

        [TestMethod] public void IsGreaterOrEqualTo_DateTimeAnchorIsOutOfRange_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(SharkTankPitch);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(SharkTankPitch.AirDate))              // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Check.IsGreaterOrEqualTo]")                // details / explanation
                .WithMessageContaining("\"91237-00-16\"")                           // details / explanation
                .WithMessageContaining("could not parse")                           // details / explanation
                .WithMessageContaining(nameof(DateTime));                           // details / explanation
        }

        [TestMethod] public void IsGreaterOrEqualTo_AnchorMatchesDataConversionSourceType_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Mushroom);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Mushroom.AverageWeight))              // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Check.IsGreaterOrEqualTo]")                // details / explanation
                .WithMessageContaining($"-18.0933 of type {nameof(Double)}")        // details / explanation
                .WithMessageContaining(nameof(Int32));                              // details / explanation
        }

        [TestMethod] public void IsGreaterOrEqualTo_AnchorMatchesDataConversionTargetType() {
            // Arrange
            var translator = new Translator();
            var source = typeof(EMail);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(EMail.CC), ComparisonOperator.GTE, 73).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsGreaterOrEqualTo_ScalarConstrainedMultipleTimes() {
            // Arrange
            var translator = new Translator();
            var source = typeof(SolarEclipse);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(SolarEclipse.SarosCycle), ComparisonOperator.GTE, 3).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsGreaterOrEqualTo_PathIsNull_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(YuGiOhMonster);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(YuGiOhMonster.Attack))                // error location
                .WithMessageContaining("path is null")                              // category
                .WithMessageContaining("[Check.IsGreaterOrEqualTo]");               // details / explanation
        }

        [TestMethod] public void IsGreaterOrEqualTo_PathOnScalar_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Hieroglyph);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Hieroglyph.Glyph))                    // error location
                .WithMessageContaining("path*does not exist")                       // category
                .WithMessageContaining("[Check.IsGreaterOrEqualTo]")                // details / explanation
                .WithMessageContaining("\"---\"");                                  // details / explanation
        }

        [TestMethod] public void IsGreaterOrEqualTo_NonExistentPathOnAggregate_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Pagoda);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Pagoda.Location))                     // error location
                .WithMessageContaining("path*does not exist")                       // category
                .WithMessageContaining("[Check.IsGreaterOrEqualTo]")                // details / explanation
                .WithMessageContaining("\"---\"");                                  // details / explanation
        }

        [TestMethod] public void IsGreaterOrEqualTo_NoPathOnAggregate_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Motorcycle);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Motorcycle.Wheels))                   // error location
                .WithMessageContaining("path is required")                          // category
                .WithMessageContaining("[Check.IsGreaterOrEqualTo]");               // details / explanation
        }

        [TestMethod] public void IsGreaterOrEqualTo_DefaultValueDoesNotSatisfyConstraint_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Camera);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Camera.ShutterSpeed))                 // error location
                .WithMessageContaining("default*does not satisfy constraints")      // category
                .WithMessageContaining("one or more [Check.xxx] constraints")       // details / explanation
                .WithMessageContaining("1E-05")                                     // details / explanation
                .WithMessageContaining("is not in interval [1.3, +∞)");             // details / explanation
        }
    }

    [TestClass, TestCategory("Constraints - Comparison")]
    public class IsLessOrEqualToTests {
        [TestMethod] public void IsLessOrEqualTo_NumericFields() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Fjord);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(Fjord.Latitude), ComparisonOperator.LTE, 90f).And
                .HaveConstraint(nameof(Fjord.Longitude), ComparisonOperator.LTE, 90f).And
                .HaveConstraint(nameof(Fjord.Length), ComparisonOperator.LTE, 100000UL).And
                .HaveConstraint(nameof(Fjord.Width), ComparisonOperator.LTE, (short)6723).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsLessOrEqualTo_TextualFields() {
            // Arrange
            var translator = new Translator();
            var source = typeof(ExcelRange);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(ExcelRange.StartColumn), ComparisonOperator.LTE, 'Z').And
                .HaveConstraint(nameof(ExcelRange.EndColumn), ComparisonOperator.LTE, "XFD").And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsLessOrEqualTo_BooleanField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(TectonicPlate);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(TectonicPlate.OnRingOfFire))          // error location
                .WithMessageContaining("constraint is inapplicable")                // category
                .WithMessageContaining("[Check.IsLessOrEqualTo]")                   // details / explanation
                .WithMessageContaining("totally ordered")                           // details / explanation
                .WithMessageContaining(nameof(Boolean));                            // details / explanation
        }

        [TestMethod] public void IsLessOrEqualTo_DecimalField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Caliphate);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(Caliphate.Population), ComparisonOperator.LTE, (decimal)8192481241.412841).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsLessOrEqualTo_DateTimeField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Representative);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(Representative.FirstElected), ComparisonOperator.LTE, new DateTime(2688, 12, 2)).And
                .HaveNoOtherCandidateKeys();
        }

        [TestMethod] public void IsLessOrEqualTo_GuidField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Sunscreen);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Sunscreen.ID))                        // error location
                .WithMessageContaining("constraint is inapplicable")                // category
                .WithMessageContaining("[Check.IsLessOrEqualTo]")                   // details / explanation
                .WithMessageContaining("totally ordered")                           // details / explanation
                .WithMessageContaining(nameof(Guid));                               // details / explanation
        }

        [TestMethod] public void IsLessOrEqualTo_EnumerationField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(ConcertTour);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(ConcertTour.ArtistType))              // error location
                .WithMessageContaining("constraint is inapplicable")                // category
                .WithMessageContaining("[Check.IsLessOrEqualTo]")                   // details / explanation
                .WithMessageContaining("totally ordered")                           // details / explanation
                .WithMessageContaining(nameof(ConcertTour.Type));                   // details / explanation
        }

        [TestMethod] public void IsLessOrEqualTo_NestedApplicableScalar() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Hominin);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("Species.Genus", ComparisonOperator.LTE, "Zubeia").And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsLessOrEqualTo_NestedInapplicableScalar_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(AmazonService);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(AmazonService.Plan))                  // error location
                .WithMessageContaining("\"Type\"")                                  // nested path
                .WithMessageContaining("constraint is inapplicable")                // category
                .WithMessageContaining("[Check.IsLessOrEqualTo]")                   // details / explanation
                .WithMessageContaining("totally ordered")                           // details / explanation
                .WithMessageContaining(nameof(AmazonService.SubscriptionType));     // details / explanation
        }

        [TestMethod] public void IsLessOrEqualTo_NestedAggregate_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Shampoo);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Shampoo.Directions))                  // error location
                .WithMessageContaining("refers to a non-scalar")                    // category
                .WithMessageContaining("[Check.IsLessOrEqualTo]")                   // details / explanation
                .WithMessageContaining("\"Ages\"");                                 // details / explanation
        }

        [TestMethod] public void IsLessOrEqualTo_NullableTotallyOrderedFields() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Subreddit);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(Subreddit.Moderator), ComparisonOperator.LTE, "???").And
                .HaveConstraint(nameof(Subreddit.Initiated), ComparisonOperator.LTE, new DateTime(7771, 4, 15)).And
                .HaveConstraint(nameof(Subreddit.TimesQuarantined), ComparisonOperator.LTE, 47).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsLessOrEqualTo_InconvertibleAnchor_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Dreidel);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Dreidel.SerialCode))                  // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Check.IsLessOrEqualTo]")                   // details / explanation
                .WithMessageContaining($"153 of type {nameof(Byte)}")               // details / explanation
                .WithMessageContaining(nameof(String));                             // details / explanation
        }

        [TestMethod] public void IsLessOrEqualTo_ConvertibleAnchor_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(ArthurianKnight);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(ArthurianKnight.MalloryMentions))     // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Check.IsLessOrEqualTo]")                   // details / explanation
                .WithMessageContaining($"4 of type {nameof(UInt32)}")               // details / explanation
                .WithMessageContaining(nameof(UInt64));                             // details / explanation
        }

        [TestMethod] public void IsLessOrEqualTo_ArrayAnchor_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Mint);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Mint.Established))                    // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Check.IsLessOrEqualTo]")                   // details / explanation
                .WithMessageContaining("array")                                     // details / explanation
                .WithMessageContaining(nameof(String));                             // details / explanation
        }

        [TestMethod] public void IsLessOrEqualTo_NullAnchor_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(VoirDire);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(VoirDire.BatsonChallenges))           // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Check.IsLessOrEqualTo]")                   // details / explanation
                .WithMessageContaining("null");                                     // details / explanation
        }

        [TestMethod] public void IsLessOrEqualTo_AnchorIsMaximum_Redundant() {
            // Arrange
            var translator = new Translator();
            var source = typeof(ShellCommand);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(ShellCommand.NumOptions), ComparisonOperator.LTE, long.MaxValue).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsLessOrEqualTo_DecimalAnchorIsNotDouble_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(ChewingGum);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(ChewingGum.AverageLifetime))          // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Check.IsLessOrEqualTo]")                   // details / explanation
                .WithMessageContaining($"'(' of type {nameof(Char)}")               // details / explanation
                .WithMessageContaining(nameof(Decimal))                             // details / explanation
                .WithMessageContaining(nameof(Double));                             // details / explanation
        }

        [TestMethod] public void IsLessOrEqualTo_DecimalAnchorIsOutOfRange_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Headphones);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Headphones.MaxVolume))                // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Check.IsLessOrEqualTo]")                   // details / explanation
                .WithMessageContaining(double.PositiveInfinity.ToString())          // details / explanation
                .WithMessageContaining("could not convert")                         // details / explanation
                .WithMessageContaining(nameof(Decimal));                            // details / explanation
        }

        [TestMethod] public void IsLessOrEqualTo_DateTimeAnchorIsNotString_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(ClockTower);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(ClockTower.Inaugurated))              // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Check.IsLessOrEqualTo]")                   // details / explanation
                .WithMessageContaining($"-381723 of type {nameof(Int64)}")          // details / explanation
                .WithMessageContaining(nameof(DateTime))                            // details / explanation
                .WithMessageContaining(nameof(String));                             // details / explanation
        }

        [TestMethod] public void IsLessOrEqualTo_DateTimeAnchorIsMalformatted_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(KentuckyDerby);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(KentuckyDerby.Racetime))              // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Check.IsLessOrEqualTo]")                   // details / explanation
                .WithMessageContaining("\"2317-04-19 @ 2:00pm\"")                   // details / explanation
                .WithMessageContaining("could not parse")                           // details / explanation
                .WithMessageContaining(nameof(DateTime));                           // details / explanation
        }

        [TestMethod] public void IsLessOrEqualTo_DateTimeAnchorIsOutOfRange_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Firearm);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Firearm.Manufactured))                // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Check.IsLessOrEqualTo]")                   // details / explanation
                .WithMessageContaining("\"1927-03-109\"")                           // details / explanation
                .WithMessageContaining("could not parse")                           // details / explanation
                .WithMessageContaining(nameof(DateTime));                           // details / explanation
        }

        [TestMethod] public void IsLessOrEqualTo_AnchorMatchesDataConversionSourceType_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(ShardOfAdonalsium);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(ShardOfAdonalsium.Splintered))        // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Check.IsLessOrEqualTo]")                   // details / explanation
                .WithMessageContaining($"false of type {nameof(Boolean)}")          // details / explanation
                .WithMessageContaining(nameof(Int32));                              // details / explanation
        }

        [TestMethod] public void IsLessOrEqualTo_AnchorMatchesDataConversionTargetType() {
            // Arrange
            var translator = new Translator();
            var source = typeof(HTMLElement);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(HTMLElement.NumChildren), ComparisonOperator.LTE, "400000").And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsLessOrEqualTo_ScalarConstrainedMultipleTimes() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Archbishop);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(Archbishop.City), ComparisonOperator.LTE, "124").And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsLessOrEqualTo_PathIsNull_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(GameOfClue);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(GameOfClue.CrimeScene))               // error location
                .WithMessageContaining("path is null")                              // category
                .WithMessageContaining("[Check.IsLessOrEqualTo]");                  // details / explanation
        }

        [TestMethod] public void IsLessOrEqualTo_PathOnScalar_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(PlaneOfExistence);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(PlaneOfExistence.Name))               // error location
                .WithMessageContaining("path*does not exist")                       // category
                .WithMessageContaining("[Check.IsLessOrEqualTo]")                   // details / explanation
                .WithMessageContaining("\"---\"");                                  // details / explanation
        }

        [TestMethod] public void IsLessOrEqualTo_NonExistentPathOnAggregate_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Mausoleum);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Mausoleum.Location))                  // error location
                .WithMessageContaining("path*does not exist")                       // category
                .WithMessageContaining("[Check.IsLessOrEqualTo]")                   // details / explanation
                .WithMessageContaining("\"---\"");                                  // details / explanation
        }

        [TestMethod] public void IsLessOrEqualTo_NoPathOnAggregate_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Pseudonym);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Pseudonym.For))                       // error location
                .WithMessageContaining("path is required")                          // category
                .WithMessageContaining("[Check.IsLessOrEqualTo]");                  // details / explanation
        }

        [TestMethod] public void IsLessOrEqualTo_DefaultValueDoesNotSatisfyConstraint_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(BowlingFrame);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(BowlingFrame.SecondThrowPins))        // error location
                .WithMessageContaining("default*does not satisfy constraints")      // category
                .WithMessageContaining("one or more [Check.xxx] constraints")       // details / explanation
                .WithMessageContaining("23")                                        // details / explanation
                .WithMessageContaining("is not in interval (-∞, 10]");              // details / explanation
        }
    }

    [TestClass, TestCategory("Constraints - Comparison")]
    public class IsNotTests {
        [TestMethod] public void IsNot_NumericFields() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Bridge);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(Bridge.Length), ComparisonOperator.NE, 34).And
                .HaveConstraint(nameof(Bridge.Height), ComparisonOperator.NE, 15UL).And
                .HaveConstraint(nameof(Bridge.Width), ComparisonOperator.NE, 0.23776f).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNot_TextualFields() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Quatrain);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(Quatrain.Line1), ComparisonOperator.NE, "Elephant").And
                .HaveConstraint(nameof(Quatrain.Line2), ComparisonOperator.NE, "Giraffe").And
                .HaveConstraint(nameof(Quatrain.Line3), ComparisonOperator.NE, "Crocodile").And
                .HaveConstraint(nameof(Quatrain.Line4), ComparisonOperator.NE, "Rhinoceros").And
                .HaveConstraint(nameof(Quatrain.FirstLetter), ComparisonOperator.NE, '$').And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNot_BooleanField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(PoliceOfficer);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(PoliceOfficer.IsRetired), ComparisonOperator.NE, false).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNot_DecimalField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Therapist);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(Therapist.CostPerHour), ComparisonOperator.NE, (decimal)0.750).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNot_DateTimeField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(SlotMachine);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(SlotMachine.InstalledOn), ComparisonOperator.NE, new DateTime(4431, 1, 21)).And
                .HaveNoOtherCandidateKeys();
        }

        [TestMethod] public void IsNot_GuidField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Church);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(Church.ChurchID), ComparisonOperator.NE, new Guid("a3c3ac24-4cf2-428e-a4db-76b30958cc90")).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNot_EnumerationField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(MarianApparition);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField(nameof(MarianApparition.When)).OfTypeDateTime().BeingNonNullable().And
                .HaveField(nameof(MarianApparition.Location)).OfTypeText().BeingNonNullable().And
                .HaveField(nameof(MarianApparition.Witnesses)).OfTypeText().BeingNonNullable().And
                .HaveField(nameof(MarianApparition.MarianTitle)).OfTypeText().BeingNonNullable().And
                .HaveField(nameof(MarianApparition.Recognition)).OfTypeEnumeration(
                    MarianApparition.Status.Accepted,
                    MarianApparition.Status.Alleged,
                    MarianApparition.Status.Confirmed,
                    MarianApparition.Status.Documented,
                    MarianApparition.Status.Recognized
                ).BeingNonNullable().And
                .HaveNoOtherFields().And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNot_NestedScalar() {
            // Arrange
            var translator = new Translator();
            var source = typeof(SeventhInningStretch);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("Game.AwayTeam", ComparisonOperator.NE, "Savannah Bananas").And
                .HaveConstraint("Game.Date", ComparisonOperator.NE, new DateTime(2001, 9, 11)).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNot_NestedAggregate_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(SportsBet);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(SportsBet.Odds))                      // error location
                .WithMessageContaining("refers to a non-scalar")                    // category
                .WithMessageContaining("[Check.IsNot]")                             // details / explanation
                .WithMessageContaining("\"OneDollarPayout\"");                      // details / explanation
        }

        [TestMethod] public void IsNot_NullableTotallyOrderedFields() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Fountain);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(Fountain.FountainUUID), ComparisonOperator.NE, new Guid("926dbe07-875c-46fd-863b-051b98a2d6be")).And
                .HaveConstraint(nameof(Fountain.Unveiled), ComparisonOperator.NE, new DateTime(1131, 8, 19)).And
                .HaveConstraint(nameof(Fountain.Spout), ComparisonOperator.NE, 35.22).And
                .HaveConstraint(nameof(Fountain.Masonry), ComparisonOperator.NE, "Play-Doh").And
                .HaveConstraint(nameof(Fountain.IsActive), ComparisonOperator.NE, false).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNot_InconvertibleAnchor_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Candle);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Candle.Width))                        // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Check.IsNot]")                             // details / explanation
                .WithMessageContaining($"\"Wide\" of type {nameof(String)}")        // details / explanation
                .WithMessageContaining(nameof(Single));                             // details / explanation
        }

        [TestMethod] public void IsNot_ConvertibleAnchor_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(CompilerWarning);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(CompilerWarning.DebugOnly))           // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Check.IsNot]")                             // details / explanation
                .WithMessageContaining($"1 of type {nameof(Int32)}")                // details / explanation
                .WithMessageContaining(nameof(Boolean));                            // details / explanation
        }

        [TestMethod] public void IsNot_ArrayAnchor_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Alarm);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Alarm.Snoozeable))                    // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Check.IsNot]")                             // details / explanation
                .WithMessageContaining("array")                                     // details / explanation
                .WithMessageContaining(nameof(Boolean));                            // details / explanation
        }

        [TestMethod] public void IsNot_NullAnchor_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(SecurityBug);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(SecurityBug.VersionPatched))          // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Check.IsNot]")                             // details / explanation
                .WithMessageContaining("null");                                     // details / explanation
        }

        [TestMethod] public void IsNot_DecimalAnchorIsNotDouble_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(DistrictAttorney);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(DistrictAttorney.ConvictionRate))     // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Check.IsNot]")                             // details / explanation
                .WithMessageContaining($"false of type {nameof(Boolean)}")          // details / explanation
                .WithMessageContaining(nameof(Decimal))                             // details / explanation
                .WithMessageContaining(nameof(Double));                             // details / explanation
        }

        [TestMethod] public void IsNot_DecimalAnchorIsOutOfRange_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Ping);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Ping.RoundTrip))                      // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Check.IsNot]")                             // details / explanation
                .WithMessageContaining((double.MaxValue - 3.0).ToString())          // details / explanation
                .WithMessageContaining("could not convert")                         // details / explanation
                .WithMessageContaining(nameof(Decimal));                            // details / explanation
        }

        [TestMethod] public void IsNot_DateTimeAnchorIsNotString_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(InsurancePolicy);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(InsurancePolicy.EffectiveAsOf))       // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Check.IsNot]")                             // details / explanation
                .WithMessageContaining($"-8193.018 of type {nameof(Single)}")       // details / explanation
                .WithMessageContaining(nameof(DateTime))                            // details / explanation
                .WithMessageContaining(nameof(String));                             // details / explanation
        }

        [TestMethod] public void IsNot_DateTimeAnchorIsMalformatted_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Mosque);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Mosque.Established))                  // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Check.IsNot]")                             // details / explanation
                .WithMessageContaining("\"1.4.5.0.1.0.3.0\"")                       // details / explanation
                .WithMessageContaining("could not parse")                           // details / explanation
                .WithMessageContaining(nameof(DateTime));                           // details / explanation
        }

        [TestMethod] public void IsNot_DateTimeAnchorIsOutOfRange_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Lease);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Lease.StartDate))                     // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Check.IsNot]")                             // details / explanation
                .WithMessageContaining("\"1637-07-8819\"")                          // details / explanation
                .WithMessageContaining("could not parse")                           // details / explanation
                .WithMessageContaining(nameof(DateTime));                           // details / explanation
        }

        [TestMethod] public void IsNot_AnchorMatchesDataConversionSourceType_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(FairyTale);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(FairyTale.Disneyfied))                // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Check.IsNot]")                             // details / explanation
                .WithMessageContaining($"false of type {nameof(Boolean)}")          // details / explanation
                .WithMessageContaining(nameof(String));                             // details / explanation
        }

        [TestMethod] public void IsNot_AnchorMatchesDataConversionTargetType() {
            // Arrange
            var translator = new Translator();
            var source = typeof(RingOfPower);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(RingOfPower.Destroyed), ComparisonOperator.NE, 7).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNot_ScalarConstrainedMultipleTimesSameAnchor() {
            // Arrange
            var translator = new Translator();
            var source = typeof(NazcaLine);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(NazcaLine.Name), ComparisonOperator.NE, "Iguana").And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNot_ScalarConstrainedMultipleTimesDifferentAnchors() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Pterosaur);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(Pterosaur.Specimens), InclusionOperator.NotIn,
                    0U, 7894520U
                ).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNot_PathIsNull_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(LotteryTicket);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(LotteryTicket.PurchaseTime))          // error location
                .WithMessageContaining("path is null")                              // category
                .WithMessageContaining("[Check.IsNot]");                            // details / explanation
        }

        [TestMethod] public void IsNot_PathOnScalar_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Prison);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Prison.SecurityLevel))                // error location
                .WithMessageContaining("path*does not exist")                       // category
                .WithMessageContaining("[Check.IsNot]")                             // details / explanation
                .WithMessageContaining("\"---\"");                                  // details / explanation
        }

        [TestMethod] public void IsNot_NonExistentPathOnAggregate_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Restaurant);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Restaurant.SaladBar))                 // error location
                .WithMessageContaining("path*does not exist")                       // category
                .WithMessageContaining("[Check.IsNot]")                             // details / explanation
                .WithMessageContaining("\"---\"");                                  // details / explanation
        }

        [TestMethod] public void IsNot_NoPathOnAggregate_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Balk);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Balk.Pitcher))                        // error location
                .WithMessageContaining("path is required")                          // category
                .WithMessageContaining("[Check.IsNot]");                            // details / explanation
        }

        [TestMethod] public void IsNot_DefaultValueDoesNotSatisfyConstraint_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(RestStop);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(RestStop.Exit))                       // error location
                .WithMessageContaining("default*does not satisfy constraints")      // category
                .WithMessageContaining("one or more [Check.xxx] constraints")       // details / explanation
                .WithMessageContaining("153")                                       // details / explanation
                .WithMessageContaining("is explicitly disallowed");                 // details / explanation
        }
    }
}
