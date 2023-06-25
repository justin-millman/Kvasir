using FluentAssertions;
using Kvasir.Exceptions;
using Kvasir.Schema;
using Kvasir.Translation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

using static UT.Kvasir.Translation.MixedConstraints;

namespace UT.Kvasir.Translation {
    [TestClass, TestCategory("Constraints - Mixed")]
    public class MixedConstraintTests {
        [TestMethod] public void IsPositiveIsNonZero_Redundant() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Peninsula);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(Peninsula.Coastline), ComparisonOperator.GT, 0L).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNegativeIsNonZero_Redundant() {
            // Arrange
            var translator = new Translator();
            var source = typeof(HTTPError);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(HTTPError.ErrorCode), ComparisonOperator.LT, 0).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsPositiveIsNegative_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Directory);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Directory.Mode))                      // error location
                .WithMessageContaining("mutually exclusive")                        // category
                .WithMessageContaining("[Check.IsPositive]")                        // details / explanation
                .WithMessageContaining("[Check.IsNegative]");                       // details / explanation
        }

        [TestMethod] public void GreaterThanConstraints() {
            // Arrange
            var translator = new Translator();
            var source = typeof(ACT);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(ACT.Mathematics), ComparisonOperator.GT, (sbyte)10).And
                .HaveConstraint(nameof(ACT.Science), ComparisonOperator.GT, (sbyte)0).And
                .HaveConstraint(nameof(ACT.Reading), ComparisonOperator.GTE, (sbyte)9).And
                .HaveConstraint(nameof(ACT.English), ComparisonOperator.GTE, (sbyte)8).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void LessThanConstraints() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Concert);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(Concert.Duration), ComparisonOperator.LT, 251U).And
                .HaveConstraint(nameof(Concert.AverageTicketPrice), ComparisonOperator.LT, (decimal)0).And
                .HaveConstraint(nameof(Concert.Attendees), ComparisonOperator.LTE, 28172831).And
                .HaveConstraint(nameof(Concert.Encores), ComparisonOperator.LT, (sbyte)0).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void ComparisonLowerLessThanComparisonUpper() {
            // Arrange
            var translator = new Translator();
            var source = typeof(SlaveRevolt);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(SlaveRevolt.Leader), ComparisonOperator.GT, "Asmodeus").And
                .HaveConstraint(nameof(SlaveRevolt.Leader), ComparisonOperator.LT, "Zylaphtanes").And
                .HaveConstraint(nameof(SlaveRevolt.SlaveCasualties), ComparisonOperator.GTE, -3).And
                .HaveConstraint(nameof(SlaveRevolt.SlaveCasualties), ComparisonOperator.LTE, 20491486).And
                .HaveConstraint(nameof(SlaveRevolt.OwnerCasualties), ComparisonOperator.GT, 4UL).And
                .HaveConstraint(nameof(SlaveRevolt.OwnerCasualties), ComparisonOperator.LTE, 510492UL).And
                .HaveConstraint(nameof(SlaveRevolt.Date), ComparisonOperator.GTE, new DateTime(575, 3, 19)).And
                .HaveConstraint(nameof(SlaveRevolt.Date), ComparisonOperator.LT, new DateTime(8753, 11, 26)).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void ComparisonInclusiveLowerEqualsComparisonInclusiveUpper() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Prescription);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(Prescription.Medication), ComparisonOperator.EQ, "Bastioquiloquine").And
                .HaveConstraint(nameof(Prescription.Refills), ComparisonOperator.EQ, (byte)2).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void ComparisonInclusiveLowerEqualsComparisonExclusiveUpper_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Genie);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Genie.NumWishes))                     // error location
                .WithMessageContaining("conflicting constraints")                   // category
                .WithMessageContaining("one or more [Check.xxx] constraints")       // details / explanation
                .WithMessageContaining("[1, 1)");                                   // details / explanation
        }
        
        [TestMethod] public void ComparisonExclusiveLowerEqualsComparisonInclusiveUpper_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(ComicCon);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(ComicCon.NumPanels))                  // error location
                .WithMessageContaining("conflicting constraints")                   // category
                .WithMessageContaining("one or more [Check.xxx] constraints")       // details / explanation
                .WithMessageContaining("(275, 275]");                               // details / explanation
        }

        [TestMethod] public void ComparisonExclusiveLowerEqualsComparisonExclusiveUpper_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(CabinetDepartment);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(CabinetDepartment.Budget))            // error location
                .WithMessageContaining("conflicting constraints")                   // category
                .WithMessageContaining("one or more [Check.xxx] constraints")       // details / explanation
                .WithMessageContaining("(481723.5, 481723.5)");                     // details / explanation
        }

        [TestMethod] public void ComparisonLowerGreaterThanComparisonUpper_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Locale);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Locale.CodeSet))                      // error location
                .WithMessageContaining("conflicting constraints")                   // category
                .WithMessageContaining("one or more [Check.xxx] constraints")       // details / explanation
                .WithMessageContaining("(\"UTF-7\", \"ASCII\"]");                   // details / explanation
        }

        [TestMethod] public void IsNotValueOutsideComparisonRange_Redundant() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Calendar);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(Calendar.NumMonths), ComparisonOperator.LT, 21U).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void LengthIsAtLeastIsNonEmpty() {
            // Arrange
            var translator = new Translator();
            var source = typeof(StepPyramid);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(FieldFunction.LengthOf, nameof(StepPyramid.KnownAs), ComparisonOperator.GTE, 5).And
                .HaveConstraint(FieldFunction.LengthOf, nameof(StepPyramid.Civilization), ComparisonOperator.GTE, 1).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void LengthIsAtMostIsNonEmpty() {
            // Arrange
            var translator = new Translator();
            var source = typeof(UIButton);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(FieldFunction.LengthOf, nameof(UIButton.ComponentID), ComparisonOperator.LTE, 20).And
                .HaveConstraint(FieldFunction.LengthOf, nameof(UIButton.ComponentID), ComparisonOperator.GTE, 1).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void LengthIsBetweenIsNonEmpty() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Cave);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(FieldFunction.LengthOf, nameof(Cave.Name), ComparisonOperator.GTE, 75412).And
                .HaveConstraint(FieldFunction.LengthOf, nameof(Cave.Name), ComparisonOperator.LTE, 12981147).And
                .HaveConstraint(FieldFunction.LengthOf, nameof(Cave.ManagingOrg), ComparisonOperator.GTE, 1).And
                .HaveConstraint(FieldFunction.LengthOf, nameof(Cave.ManagingOrg), ComparisonOperator.LTE, 74).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void LengthIsAtLeastBoundNoLargerThanLengthIsAtMostBound() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Vulgarity);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(FieldFunction.LengthOf, nameof(Vulgarity.Spanish), ComparisonOperator.GTE, 18).And
                .HaveConstraint(FieldFunction.LengthOf, nameof(Vulgarity.Spanish), ComparisonOperator.LTE, 197).And
                .HaveConstraint(FieldFunction.LengthOf, nameof(Vulgarity.French), ComparisonOperator.EQ, 14981).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void LengthIsAtLeastBoundGreaterThanLengthIsAtMostBound_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Generation);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Generation.Name))                     // error location
                .WithMessageContaining("conflicting constraints")                   // category
                .WithMessageContaining("one or more [Check.xxx] constraints")       // details / explanation
                .WithMessageContaining("[153, 111]");                               // details / explanation
        }

        [TestMethod] public void LengthIsAtLeastLengthIsBetween() {
            // Arrange
            var translator = new Translator();
            var source = typeof(WitchHunt);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(FieldFunction.LengthOf, nameof(WitchHunt.Name), ComparisonOperator.GTE, 15).And
                .HaveConstraint(FieldFunction.LengthOf, nameof(WitchHunt.Name), ComparisonOperator.LTE, 30).And
                .HaveConstraint(FieldFunction.LengthOf, nameof(WitchHunt.Leader), ComparisonOperator.GTE, 19).And
                .HaveConstraint(FieldFunction.LengthOf, nameof(WitchHunt.Leader), ComparisonOperator.LTE, 50).And
                .HaveConstraint(FieldFunction.LengthOf, nameof(WitchHunt.FirstVictim), ComparisonOperator.GTE, 309).And
                .HaveConstraint(FieldFunction.LengthOf, nameof(WitchHunt.FirstVictim), ComparisonOperator.LTE, 12000000).And
                .HaveConstraint(FieldFunction.LengthOf, nameof(WitchHunt.ExecutionMethod), ComparisonOperator.EQ, 100).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void LengthIsAtLeastBoundGreaterThanLengthIsBetweenUpperBound_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Integral);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Integral.Expression))                 // error location
                .WithMessageContaining("conflicting constraints")                   // category
                .WithMessageContaining("one or more [Check.xxx] constraints")       // details / explanation
                .WithMessageContaining("[555, 22]");                                // details / explanation
        }

        [TestMethod] public void LengthIsAtMostLengthIsBetween() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Isthmus);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(FieldFunction.LengthOf, nameof(Isthmus.Name), ComparisonOperator.GTE, 22).And
                .HaveConstraint(FieldFunction.LengthOf, nameof(Isthmus.Name), ComparisonOperator.LTE, 413).And
                .HaveConstraint(FieldFunction.LengthOf, nameof(Isthmus.StarRating), ComparisonOperator.EQ, 1).And
                .HaveConstraint(FieldFunction.LengthOf, nameof(Isthmus.MostPopulousCity), ComparisonOperator.GTE, 18).And
                .HaveConstraint(FieldFunction.LengthOf, nameof(Isthmus.MostPopulousCity), ComparisonOperator.LTE, 35).And
                .HaveConstraint(FieldFunction.LengthOf, nameof(Isthmus.Management), ComparisonOperator.GTE, 110).And
                .HaveConstraint(FieldFunction.LengthOf, nameof(Isthmus.Management), ComparisonOperator.LTE, 113).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void LengthIsAtMostBoundLessThanLengthIsBetweenLowerBound_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(NuGetPackage);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(NuGetPackage.Version))                // error location
                .WithMessageContaining("conflicting constraints")                   // category
                .WithMessageContaining("one or more [Check.xxx] constraints")       // details / explanation
                .WithMessageContaining("[15, 10]");                                 // details / explanation
        }

        [TestMethod] public void IsNotValueHasInvalidLength_Redundant() {
            // Arrange
            var translator = new Translator();
            var source = typeof(LicensePlate);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(FieldFunction.LengthOf, nameof(LicensePlate.PlateNumber), ComparisonOperator.GTE, 1).And
                .HaveConstraint(FieldFunction.LengthOf, nameof(LicensePlate.PlateNumber), ComparisonOperator.LTE, 8).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsOneOfIsNotOneOfDisjoint() {
            // Arrange
            var translator = new Translator();
            var source = typeof(SkiSlope);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(SkiSlope.Level), InclusionOperator.In,
                    "Black Diamond", "Novice"
                ).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsOneOfIsNotOneOfOverlapping() {
            // Arrange
            var translator = new Translator();
            var source = typeof(AmusementPark);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(AmusementPark.Name), InclusionOperator.In,
                    "Six Flags", "Universal Studios"
                ).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsOneOfIsNotOneOfEquivalent_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(MarkUpSymbol);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(MarkUpSymbol.Symbol))                 // error location
                .WithMessageContaining("conflicting constraints")                   // category
                .WithMessageContaining("one or more [Check.xxx] constraints")       // details / explanation
                .WithMessageContaining("{ '*', '_', '`', '+' }");                   // details / explanation
        }

        [TestMethod] public void IsOneOfWithComparisons() {
            // Arrange
            var translator = new Translator();
            var source = typeof(TicTacToeGame);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(TicTacToeGame.TopLeft), InclusionOperator.In,
                    'O', 'X', 'Y'
                ).And
                .HaveConstraint(nameof(TicTacToeGame.TopCenter), InclusionOperator.In,
                    'X', 'Y'
                ).And
                .HaveConstraint(nameof(TicTacToeGame.MiddleLeft), InclusionOperator.In,
                    'O', 'Y'
                ).And
                .HaveConstraint(nameof(TicTacToeGame.MiddleCenter), InclusionOperator.In,
                    'O', 'X', 'Y'
                ).And
                .HaveConstraint(nameof(TicTacToeGame.BottomCenter), InclusionOperator.In,
                    'X', 'Y'
                ).And
                .HaveConstraint(nameof(TicTacToeGame.BottomRight), InclusionOperator.In,
                    'O', 'X', 'Y'
                ).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsOneOfWithLengths() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Printer);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(Printer.Brand), InclusionOperator.In,
                    "Hijack", "Mecha-Print"
                ).And
                .HaveConstraint(nameof(Printer.PaperSize), ComparisonOperator.EQ, "Poster Board").And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsOneOfAllFailComparisonChecks_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Aqueduct);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Aqueduct.WaterVolume))                // error location
                .WithMessageContaining("conflicting constraints")                   // category
                .WithMessageContaining("one or more [Check.xxx] constraints")       // details / explanation
                .WithMessageContaining("each of the allowed values*is disallowed")  // details / explanation
                .WithMessageContaining("{ 200.1, 13.6237, 450.087798 }");           // details / explanation
        }

        [TestMethod] public void IsOneOfAllFailLengthChecks_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(SurvivorSeason);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(SurvivorSeason.Location))             // error location
                .WithMessageContaining("conflicting constraints")                   // category
                .WithMessageContaining("one or more [Check.xxx] constraints")       // details / explanation
                .WithMessageContaining("each of the allowed values*is disallowed")  // details / explanation
                .WithMessageContaining("{ \"Madagascar\", \"The Serengeti\" }");    // details / explanation
        }

        [TestMethod] public void IsOneOfAllFailEitherComparisonOrLengthChecks_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(NorseGod);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(SurvivorSeason.Location))             // error location
                .WithMessageContaining("conflicting constraints")                   // category
                .WithMessageContaining("one or more [Check.xxx] constraints")       // details / explanation
                .WithMessageContaining("each of the allowed values*is disallowed")  // details / explanation
                .WithMessageContaining("{ \"Thor\", \"Heimdallr\", \"Ymir\" }");    // details / explanation
        }

        [TestMethod] public void IsNotOneOfWithComparisons() {
            // Arrange
            var translator = new Translator();
            var source = typeof(PostageStamp);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(PostageStamp.Length), ComparisonOperator.GTE, 0.05).And
                .HaveConstraint(nameof(PostageStamp.Length), InclusionOperator.NotIn,
                    0.05, 0.7712
                ).And
                .HaveConstraint(nameof(PostageStamp.Height), ComparisonOperator.GT, 0.01).And
                .HaveConstraint(nameof(PostageStamp.Height), ComparisonOperator.LT, 2.37).And
                .HaveConstraint(nameof(PostageStamp.Height), InclusionOperator.NotIn,
                    1.335, 2.012
                ).And
                .HaveConstraint(nameof(PostageStamp.Cost), ComparisonOperator.LTE, 1.0).And
                .HaveConstraint(nameof(PostageStamp.Cost), InclusionOperator.NotIn,
                    0.25, 0.5, 0.75, 1.0
                ).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNotOneOfWithLengths() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Obelisk);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(FieldFunction.LengthOf, nameof(Obelisk.Material), ComparisonOperator.GTE, 9).And
                .HaveConstraint(nameof(Obelisk.Material), InclusionOperator.NotIn,
                    "Limestone", "Reinforced Steel"
                ).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNotOneOfDisallowsSingleComparisonValue_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Beach);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Beach.Coastline))                     // error location
                .WithMessageContaining("conflicting constraints")                   // category
                .WithMessageContaining("one or more [Check.xxx] constraints")       // details / explanation
                .WithMessageContaining("each of the allowed values*is disallowed")  // details / explanation
                .WithMessageContaining("{ 75 }");                                   // details / explanation
        }
    }
}
