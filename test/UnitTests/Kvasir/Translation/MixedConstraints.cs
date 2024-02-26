using FluentAssertions;
using Kvasir.Schema;
using Kvasir.Translation2;
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
                .HaveConstraint("Coastline", ComparisonOperator.GT, 0L).And
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
                .HaveConstraint("ErrorCode", ComparisonOperator.LT, 0).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsPositiveIsNegative_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Directory);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<ConflictingAnnotationsException>()
                .WithLocation("`Directory` → Mode")
                .WithProblem("the two annotations are mutually exclusive")
                .WithAnnotations("[Check.IsPositive]", "[Check.IsNegative]")
                .EndMessage();
        }

        [TestMethod] public void GreaterThanConstraints() {
            // Arrange
            var translator = new Translator();
            var source = typeof(ACT);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("Mathematics", ComparisonOperator.GT, (sbyte)10).And
                .HaveConstraint("Science", ComparisonOperator.GT, (sbyte)0).And
                .HaveConstraint("Reading", ComparisonOperator.GTE, (sbyte)9).And
                .HaveConstraint("English", ComparisonOperator.GTE, (sbyte)8).And
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
                .HaveConstraint("Duration", ComparisonOperator.LT, 251U).And
                .HaveConstraint("AverageTicketPrice", ComparisonOperator.LT, (decimal)0).And
                .HaveConstraint("Attendees", ComparisonOperator.LTE, 28172831).And
                .HaveConstraint("Encores", ComparisonOperator.LT, (sbyte)0).And
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
                .HaveConstraint("Leader", ComparisonOperator.GT, "Asmodeus").And
                .HaveConstraint("Leader", ComparisonOperator.LT, "Zylaphtanes").And
                .HaveConstraint("SlaveCasualties", ComparisonOperator.GTE, -3).And
                .HaveConstraint("SlaveCasualties", ComparisonOperator.LTE, 20491486).And
                .HaveConstraint("OwnerCasualties", ComparisonOperator.GT, 4UL).And
                .HaveConstraint("OwnerCasualties", ComparisonOperator.LTE, 510492UL).And
                .HaveConstraint("Date", ComparisonOperator.GTE, new DateTime(575, 3, 19)).And
                .HaveConstraint("Date", ComparisonOperator.LT, new DateTime(8753, 11, 26)).And
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
                .HaveConstraint("Medication", ComparisonOperator.EQ, "Bastioquiloquine").And
                .HaveConstraint("Refills", ComparisonOperator.EQ, (byte)2).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void ComparisonInclusiveLowerEqualsComparisonExclusiveUpper_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Genie);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<UnsatisfiableConstraintException>()
                .WithLocation("`Genie` → NumWishes")
                .WithProblem("the interval [1, 1) of allowed values is empty")
                .WithAnnotations("[Check.IsLessThan]")
                .EndMessage();
        }
        
        [TestMethod] public void ComparisonExclusiveLowerEqualsComparisonInclusiveUpper_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(ComicCon);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<UnsatisfiableConstraintException>()
                .WithLocation("`ComicCon` → NumPanels")
                .WithProblem("the interval (275, 275] of allowed values is empty")
                .WithAnnotations("[Check.IsLessOrEqualTo]")
                .EndMessage();
        }

        [TestMethod] public void ComparisonExclusiveLowerEqualsComparisonExclusiveUpper_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(CabinetDepartment);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<UnsatisfiableConstraintException>()
                .WithLocation("`CabinetDepartment` → Budget")
                .WithProblem("the interval (481723.5, 481723.5) of allowed values is empty")
                .WithAnnotations("[Check.IsLessThan]")
                .EndMessage();
        }

        [TestMethod] public void ComparisonLowerGreaterThanComparisonUpper_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Locale);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<UnsatisfiableConstraintException>()
                .WithLocation("`Locale` → CodeSet")
                .WithProblem("the interval (\"UTF-7\", \"ASCII\"] of allowed values is empty")
                .WithAnnotations("[Check.IsGreaterThan]")
                .EndMessage();
        }

        [TestMethod] public void IsNotValueOutsideComparisonRange_Redundant() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Calendar);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("NumMonths", ComparisonOperator.LT, 21U).And
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
                .HaveConstraint(FieldFunction.LengthOf, "KnownAs", ComparisonOperator.GTE, 5).And
                .HaveConstraint(FieldFunction.LengthOf, "Civilization", ComparisonOperator.GTE, 1).And
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
                .HaveConstraint(FieldFunction.LengthOf, "ComponentID", ComparisonOperator.LTE, 20).And
                .HaveConstraint(FieldFunction.LengthOf, "ComponentID", ComparisonOperator.GTE, 1).And
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
                .HaveConstraint(FieldFunction.LengthOf, "Name", ComparisonOperator.GTE, 75412).And
                .HaveConstraint(FieldFunction.LengthOf, "Name", ComparisonOperator.LTE, 12981147).And
                .HaveConstraint(FieldFunction.LengthOf, "ManagingOrg", ComparisonOperator.GTE, 1).And
                .HaveConstraint(FieldFunction.LengthOf, "ManagingOrg", ComparisonOperator.LTE, 74).And
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
                .HaveConstraint(FieldFunction.LengthOf, "Spanish", ComparisonOperator.GTE, 18).And
                .HaveConstraint(FieldFunction.LengthOf, "Spanish", ComparisonOperator.LTE, 197).And
                .HaveConstraint(FieldFunction.LengthOf, "French", ComparisonOperator.EQ, 14981).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void LengthIsAtLeastBoundGreaterThanLengthIsAtMostBound_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Generation);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<UnsatisfiableConstraintException>()
                .WithLocation("`Generation` → Name")
                .WithProblem("the interval [153, 111] of valid string lengths is empty")
                .WithAnnotations("[Check.LengthIsAtMost]")
                .EndMessage();
        }

        [TestMethod] public void LengthIsAtLeastLengthIsBetween() {
            // Arrange
            var translator = new Translator();
            var source = typeof(WitchHunt);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(FieldFunction.LengthOf, "Name", ComparisonOperator.GTE, 15).And
                .HaveConstraint(FieldFunction.LengthOf, "Name", ComparisonOperator.LTE, 30).And
                .HaveConstraint(FieldFunction.LengthOf, "Leader", ComparisonOperator.GTE, 19).And
                .HaveConstraint(FieldFunction.LengthOf, "Leader", ComparisonOperator.LTE, 50).And
                .HaveConstraint(FieldFunction.LengthOf, "FirstVictim", ComparisonOperator.GTE, 309).And
                .HaveConstraint(FieldFunction.LengthOf, "FirstVictim", ComparisonOperator.LTE, 12000000).And
                .HaveConstraint(FieldFunction.LengthOf, "ExecutionMethod", ComparisonOperator.EQ, 100).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void LengthIsAtLeastBoundGreaterThanLengthIsBetweenUpperBound_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Integral);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<UnsatisfiableConstraintException>()
                .WithLocation("`Integral` → Expression")
                .WithProblem("the interval [555, 22] of valid string lengths is empty")
                .WithAnnotations("[Check.LengthIsBetween]")
                .EndMessage();
        }

        [TestMethod] public void LengthIsAtMostLengthIsBetween() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Isthmus);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(FieldFunction.LengthOf, "Name", ComparisonOperator.GTE, 22).And
                .HaveConstraint(FieldFunction.LengthOf, "Name", ComparisonOperator.LTE, 413).And
                .HaveConstraint(FieldFunction.LengthOf, "StarRating", ComparisonOperator.EQ, 1).And
                .HaveConstraint(FieldFunction.LengthOf, "MostPopulousCity", ComparisonOperator.GTE, 18).And
                .HaveConstraint(FieldFunction.LengthOf, "MostPopulousCity", ComparisonOperator.LTE, 35).And
                .HaveConstraint(FieldFunction.LengthOf, "Management", ComparisonOperator.GTE, 110).And
                .HaveConstraint(FieldFunction.LengthOf, "Management", ComparisonOperator.LTE, 113).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void LengthIsAtMostBoundLessThanLengthIsBetweenLowerBound_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(NuGetPackage);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<UnsatisfiableConstraintException>()
                .WithLocation("`NuGetPackage` → Version")
                .WithProblem("the interval [15, 10] of valid string lengths is empty")
                .WithAnnotations("[Check.LengthIsBetween]")
                .EndMessage();
        }

        [TestMethod] public void IsNotValueHasInvalidLength_Redundant() {
            // Arrange
            var translator = new Translator();
            var source = typeof(LicensePlate);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(FieldFunction.LengthOf, "PlateNumber", ComparisonOperator.GTE, 1).And
                .HaveConstraint(FieldFunction.LengthOf, "PlateNumber", ComparisonOperator.LTE, 8).And
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
                .HaveConstraint("Level", InclusionOperator.In,
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
                .HaveConstraint("Name", InclusionOperator.In,
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
            translate.Should().FailWith<UnsatisfiableConstraintException>()
                .WithLocation("`MarkUpSymbol` → Symbol")
                .WithProblem("all of the explicitly allowed values fail at least one other constraint")
                .EndMessage();
        }

        [TestMethod] public void IsOneOfWithComparisons() {
            // Arrange
            var translator = new Translator();
            var source = typeof(TicTacToeGame);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("TopLeft", InclusionOperator.In,
                    'O', 'X', 'Y'
                ).And
                .HaveConstraint("TopCenter", InclusionOperator.In,
                    'X', 'Y'
                ).And
                .HaveConstraint("MiddleLeft", InclusionOperator.In,
                    'O', 'Y'
                ).And
                .HaveConstraint("MiddleCenter", InclusionOperator.In,
                    'O', 'X', 'Y'
                ).And
                .HaveConstraint("BottomCenter", InclusionOperator.In,
                    'X', 'Y'
                ).And
                .HaveConstraint("BottomRight", InclusionOperator.In,
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
                .HaveConstraint("Brand", InclusionOperator.In,
                    "Hijack", "Mecha-Print"
                ).And
                .HaveConstraint("PaperSize", ComparisonOperator.EQ, "Poster Board").And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsOneOfAllFailComparisonChecks_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Aqueduct);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<UnsatisfiableConstraintException>()
                .WithLocation("`Aqueduct` → WaterVolume")
                .WithProblem("all of the explicitly allowed values fail at least one other constraint")
                .EndMessage();
        }

        [TestMethod] public void IsOneOfAllFailLengthChecks_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(SurvivorSeason);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<UnsatisfiableConstraintException>()
                .WithLocation("`SurvivorSeason` → Location")
                .WithProblem("all of the explicitly allowed values fail at least one other constraint")
                .EndMessage();
        }

        [TestMethod] public void IsOneOfAllFailEitherComparisonOrLengthChecks_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(NorseGod);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<UnsatisfiableConstraintException>()
                .WithLocation("`NorseGod` → Name")
                .WithProblem("all of the explicitly allowed values fail at least one other constraint")
                .EndMessage();
        }

        [TestMethod] public void IsNotOneOfWithComparisons() {
            // Arrange
            var translator = new Translator();
            var source = typeof(PostageStamp);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("Length", ComparisonOperator.GTE, 0.05).And
                .HaveConstraint("Length", InclusionOperator.NotIn,
                    0.05, 0.7712
                ).And
                .HaveConstraint("Height", ComparisonOperator.GT, 0.01).And
                .HaveConstraint("Height", ComparisonOperator.LT, 2.37).And
                .HaveConstraint("Height", InclusionOperator.NotIn,
                    1.335, 2.012
                ).And
                .HaveConstraint("Cost", ComparisonOperator.LTE, 1.0).And
                .HaveConstraint("Cost", InclusionOperator.NotIn,
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
                .HaveConstraint(FieldFunction.LengthOf, "Material", ComparisonOperator.GTE, 9).And
                .HaveConstraint("Material", InclusionOperator.NotIn,
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
            translate.Should().FailWith<UnsatisfiableConstraintException>()
                .WithLocation("`Beach` → Coastline")
                .WithProblem("all of the explicitly allowed values fail at least one other constraint")
                .EndMessage();
        }

        [TestMethod] public void NumericConversionWithSignedness() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Soup);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("Variety", InclusionOperator.In,
                    14, 177, 90
                ).And
                .HaveConstraint("HasNoodles", InclusionOperator.In,
                    1, -1
                ).And
                .HaveConstraint("BrothProtein", InclusionOperator.In,
                    -8124, -4, -99
                ).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void NumericConversionWithComparisons() {
            // Arrange
            var translator = new Translator();
            var source = typeof(CavePainting);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("Material", InclusionOperator.In,
                    3, 4
                ).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void NumericConversionWithDiscreteness() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Triangle);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("Kind", InclusionOperator.In,
                    1, 2, 4, 8, 6, 10
                ).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void AsStringConversionWithComparisons() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Casino);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("TopMoneyMaker", InclusionOperator.In,
                    "Poker", "Craps", "Roulette", "Pachinko"
                ).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void AsStringConversionWithLengths() {
            // Arrange
            var translator = new Translator();
            var source = typeof(FacebookPost);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("Visibility", InclusionOperator.In,
                    "Private", "FriendsOnly", "Subscribers"
                ).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void AsStringConversionWithDiscreteness() {
            // Arrange
            var translator = new Translator();
            var source = typeof(ZodiacSign);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("SignSeason", InclusionOperator.In,
                    "Summer", "Autumn"
                ).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void AlteredComparisonsOnNestedField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(TribeOfIsrael);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("FirstMentioned.Book", ComparisonOperator.GTE, "Bible").And
                .HaveConstraint("FirstMentioned.Book", ComparisonOperator.LTE, "Qur'an").And
                .HaveConstraint("FirstMentioned.Chapter", InclusionOperator.In,
                    (byte)1, (byte)2, (byte)3
                ).And
                .HaveConstraint("FirstMentioned.Verse", ComparisonOperator.GT, 0L).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void AlteredSignednessOnNestedField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(SecretPolice);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("Statistics.Arrests", InclusionOperator.NotIn,
                    0, 7, 196, 4410905
                ).And
                .HaveConstraint("Statistics.Murders", ComparisonOperator.GT, 0.0).And
                .HaveConstraint("Statistics.Murders", ComparisonOperator.LT, 195385.96).And
                .HaveConstraint("Statistics.Bribes", ComparisonOperator.GT, (decimal)0).And
                .HaveConstraint("Statistics.Bribes", ComparisonOperator.NE, (decimal)80.0).And
                .HaveConstraint("Statistics.YearsActive", InclusionOperator.In,
                    (sbyte)10, (sbyte)20, (sbyte)30
                ).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void AlteredLengthsOnNestedField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(SearchEngine);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("LandingPage.Domain", InclusionOperator.In,
                    "com", "tv", "gov", "net"
                ).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void AlteredDiscretenessOnNestedField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(BeninBronze);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("Dimensions.Length", InclusionOperator.In,
                    4.0, 5.0, 6.0, 7.0, 8.0, 9.0, 10.0
                ).And
                .HaveConstraint("Dimensions.Width", InclusionOperator.NotIn,
                    1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 9.0, 10.0, 11.0, 12.0, 13.0
                ).And
                .HaveNoOtherConstraints();
        }
    }
}
