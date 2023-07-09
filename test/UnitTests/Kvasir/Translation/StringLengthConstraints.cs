using FluentAssertions;
using Kvasir.Exceptions;
using Kvasir.Schema;
using Kvasir.Translation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

using static UT.Kvasir.Translation.StringLengthConstraints.IsNonEmpty;
using static UT.Kvasir.Translation.StringLengthConstraints.LengthIsAtLeast;
using static UT.Kvasir.Translation.StringLengthConstraints.LengthIsAtMost;
using static UT.Kvasir.Translation.StringLengthConstraints.LengthIsBetween;

namespace UT.Kvasir.Translation {
    [TestClass, TestCategory("Constraints - String Length")]
    public class IsNonEmptyTests {
        [TestMethod] public void IsNonEmpty_NonNullableStringField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Chocolate);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(FieldFunction.LengthOf, nameof(Chocolate.Name), ComparisonOperator.GTE, 1).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNonEmpty_NullableStringField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Scholarship);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(FieldFunction.LengthOf, nameof(Scholarship.Organization), ComparisonOperator.GTE, 1).And
                .HaveConstraint(FieldFunction.LengthOf, nameof(Scholarship.TargetSchool), ComparisonOperator.GTE, 1).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNonEmpty_NumericField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Biography);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Biography.PageCount))                 // error location
                .WithMessageContaining("constraint is inapplicable")                // category
                .WithMessageContaining("[Check.IsNonEmpty]")                        // details / explanation
                .WithMessageContaining(nameof(String))                              // details / explanation
                .WithMessageContaining(nameof(UInt16));                             // details / explanation
        }

        [TestMethod] public void IsNonEmpty_CharacterField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(MovieTicket);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(MovieTicket.Row))                     // error location
                .WithMessageContaining("constraint is inapplicable")                // category
                .WithMessageContaining("[Check.IsNonEmpty]")                        // details / explanation
                .WithMessageContaining(nameof(String))                              // details / explanation
                .WithMessageContaining(nameof(Char));                               // details / explanation
        }

        [TestMethod] public void IsNonEmpty_BooleanField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(FortuneCookie);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(FortuneCookie.Eaten))                 // error location
                .WithMessageContaining("constraint is inapplicable")                // category
                .WithMessageContaining("[Check.IsNonEmpty]")                        // details / explanation
                .WithMessageContaining(nameof(String))                              // details / explanation
                .WithMessageContaining(nameof(Boolean));                            // details / explanation
        }

        [TestMethod] public void IsNonEmpty_DateTimeField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(ScubaDive);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(ScubaDive.EntryTime))                 // error location
                .WithMessageContaining("constraint is inapplicable")                // category
                .WithMessageContaining("[Check.IsNonEmpty]")                        // details / explanation
                .WithMessageContaining(nameof(String))                              // details / explanation
                .WithMessageContaining(nameof(DateTime));                           // details / explanation
        }

        [TestMethod] public void IsNonEmpty_GuidField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Hormone);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Hormone.HormoneID))                   // error location
                .WithMessageContaining("constraint is inapplicable")                // category
                .WithMessageContaining("[Check.IsNonEmpty]")                        // details / explanation
                .WithMessageContaining(nameof(String))                              // details / explanation
                .WithMessageContaining(nameof(Guid));                               // details / explanation
        }

        [TestMethod] public void IsNonEmpty_EnumerationField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Mustache);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Mustache.Style))                      // error location
                .WithMessageContaining("constraint is inapplicable")                // category
                .WithMessageContaining("[Check.IsNonEmpty]")                        // details / explanation
                .WithMessageContaining(nameof(String))                              // details / explanation
                .WithMessageContaining(nameof(Mustache.Kind));                      // details / explanation
        }

        [TestMethod] public void IsNonEmpty_NestedApplicableScalar() {
            // Arrange
            var translator = new Translator();
            var source = typeof(BarGraph);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(FieldFunction.LengthOf, "Legend.XAxisLabel", ComparisonOperator.GTE, 1).And
                .HaveConstraint(FieldFunction.LengthOf, "Legend.YAxisLabel", ComparisonOperator.GTE, 1).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNonError_NestedInapplicableScalar_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(BackyardBaseballPlayer);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(BackyardBaseballPlayer.Statistics))   // error location
                .WithMessageContaining("\"Pitching\"")                              // nested path
                .WithMessageContaining("constraint is inapplicable")                // category
                .WithMessageContaining("[Check.IsNonEmpty")                         // details / explanation
                .WithMessageContaining(nameof(String))                              // details / explanation
                .WithMessageContaining(nameof(Byte));                               // details / explanation
        }

        [TestMethod] public void IsNonZero_NestedAggregate_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(OilField);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(OilField.Where))                      // error location
                .WithMessageContaining("refers to a non-scalar")                    // category
                .WithMessageContaining("[Check.IsNonEmpty]")                        // details / explanation
                .WithMessageContaining("\"Place.Coordinate\"");                     // details / explanation
        }

        [TestMethod] public void IsNonEmpty_FieldWithStringDataConversionTarget() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Hourglass);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(FieldFunction.LengthOf, nameof(Hourglass.Duration), ComparisonOperator.GTE, 1).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNonEmpty_FieldWithNumericDataConversionSource_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(FoodChain);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(FoodChain.SecondaryConsumer))         // error location
                .WithMessageContaining("constraint is inapplicable")                // category
                .WithMessageContaining("[Check.IsNonEmpty]")                        // details / explanation
                .WithMessageContaining(nameof(String))                              // details / explanation
                .WithMessageContaining(nameof(Int32));                              // details / explanation
        }

        [TestMethod] public void IsNonEmpty_ScalarConstrainedMultipleTimes_Redundant() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Top10List);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(FieldFunction.LengthOf, nameof(Top10List.Number9), ComparisonOperator.GTE, 1).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNonEmpty_PathIsNull_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Hoedown);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Hoedown.WayneBradyLine))              // error location
                .WithMessageContaining("path is null")                              // category
                .WithMessageContaining("[Check.IsNonEmpty]");                       // details / explanation
        }

        [TestMethod] public void IsNonEmpty_PathOnScalar_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(ASLSign);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(ASLSign.Gloss))                       // error location
                .WithMessageContaining("path*does not exist")                       // category
                .WithMessageContaining("[Check.IsNonEmpty]")                        // details / explanation
                .WithMessageContaining("\"---\"");                                  // details / explanation
        }

        [TestMethod] public void IsNonEmpty_NonExistentPathOnAggregate_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Sutra);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Sutra.Source))                        // error location
                .WithMessageContaining("path*does not exist")                       // category
                .WithMessageContaining("[Check.IsNonEmpty]")                        // details / explanation
                .WithMessageContaining("\"---\"");                                  // details / explanation
        }

        [TestMethod] public void IsNonEmpty_NoPathOnAggregate_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Kaiju);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Kaiju.Size))                          // error location
                .WithMessageContaining("path is required")                          // category
                .WithMessageContaining("[Check.IsNonEmpty]");                       // details / explanation
        }

        [TestMethod] public void IsNonEmpty_DefaultValueDoesNotSatisfyConstraint_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(AztecGod);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(AztecGod.Festival))                   // error location
                .WithMessageContaining("default*does not satisfy constraints")      // category
                .WithMessageContaining("one or more [Check.xxx] constraints")       // details / explanation
                .WithMessageContaining("\"\"")                                      // details / explanation
                .WithMessageContaining("length is 0")                               // details / explanation
                .WithMessageContaining("is not in interval [1, +∞)");               // details / explanation
        }
    }

    [TestClass, TestCategory("Constraints - String Length")]
    public class LengthIsAtLeastTests {
        [TestMethod] public void LengthIsAtLeast_NonNullableStringField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(NFLPenalty);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(FieldFunction.LengthOf, nameof(NFLPenalty.Penalty), ComparisonOperator.GTE, 5).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void LengthIsAtLeast_NullableStringField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Ben10Alien);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(FieldFunction.LengthOf, nameof(Ben10Alien.AlternateName), ComparisonOperator.GTE, 7).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void LengthIsAtLeast_NumericField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(HashFunction);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(HashFunction.BlockSize))              // error location
                .WithMessageContaining("constraint is inapplicable")                // category
                .WithMessageContaining("[Check.LengthIsAtLeast]")                   // details / explanation
                .WithMessageContaining(nameof(String))                              // details / explanation
                .WithMessageContaining(nameof(UInt16));                             // details / explanation
        }

        [TestMethod] public void LengthIsAtLeast_CharacterField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Kanji);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Kanji.Logograph))                     // error location
                .WithMessageContaining("constraint is inapplicable")                // category
                .WithMessageContaining("[Check.LengthIsAtLeast]")                   // details / explanation
                .WithMessageContaining(nameof(String))                              // details / explanation
                .WithMessageContaining(nameof(Char));                               // details / explanation
        }

        [TestMethod] public void LengthIsAtLeast_BooleanField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Magazine);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Magazine.Syndicated))                 // error location
                .WithMessageContaining("constraint is inapplicable")                // category
                .WithMessageContaining("[Check.LengthIsAtLeast]")                   // details / explanation
                .WithMessageContaining(nameof(String))                              // details / explanation
                .WithMessageContaining(nameof(Boolean));                            // details / explanation
        }

        [TestMethod] public void LengthIsAtLeast_DateTimeField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Camerlengo);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Camerlengo.Appointed))                // error location
                .WithMessageContaining("constraint is inapplicable")                // category
                .WithMessageContaining("[Check.LengthIsAtLeast]")                   // details / explanation
                .WithMessageContaining(nameof(String))                              // details / explanation
                .WithMessageContaining(nameof(DateTime));                           // details / explanation
        }

        [TestMethod] public void LengthIsAtLeast_GuidField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Rainforest);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Rainforest.ID))                       // error location
                .WithMessageContaining("constraint is inapplicable")                // category
                .WithMessageContaining("[Check.LengthIsAtLeast]")                   // details / explanation
                .WithMessageContaining(nameof(String))                              // details / explanation
                .WithMessageContaining(nameof(Guid));                               // details / explanation
        }

        [TestMethod] public void LengthIsAtLeast_EnumerationField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Cybersite);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Cybersite.FirstSeasonAppeared))       // error location
                .WithMessageContaining("constraint is inapplicable")                // category
                .WithMessageContaining("[Check.LengthIsAtLeast]")                   // details / explanation
                .WithMessageContaining(nameof(String))                              // details / explanation
                .WithMessageContaining(nameof(Cybersite.Season));                   // details / explanation
        }

        [TestMethod] public void LengthIsAtLeast_NestedApplicableScalar() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Dubbing);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(FieldFunction.LengthOf, "Dubber.FirstName", ComparisonOperator.GTE, 6).And
                .HaveConstraint(FieldFunction.LengthOf, "Dubber.LastName", ComparisonOperator.GTE, 2).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void LengthIsAtLeast_NestedInapplicableScalar_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(BaseballMogul);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(BaseballMogul.Version))               // error location
                .WithMessageContaining("\"Patch\"")                                 // nested path
                .WithMessageContaining("constraint is inapplicable")                // category
                .WithMessageContaining("[Check.LengthIsAtLeast]")                   // details / explanation
                .WithMessageContaining(nameof(String))                              // details / explanation
                .WithMessageContaining(nameof(UInt16));                             // details / explanation
        }

        [TestMethod] public void LengthIsAtLeast_NestedAggregate_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(MagicSystem);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(MagicSystem.SandersonsLaws))          // error location
                .WithMessageContaining("refers to a non-scalar")                    // category
                .WithMessageContaining("[Check.LengthIsAtLeast]")                   // details / explanation
                .WithMessageContaining("\"Zeroth\"");                               // details / explanation
        }

        [TestMethod] public void LengthIsAtLeast_FieldWithStringDataConversionTarget() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Ambassador);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(FieldFunction.LengthOf, nameof(Ambassador.Assumed), ComparisonOperator.GTE, 10).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void LengthIsAtLeast_FieldWithNumericDataConversionSource_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Campfire);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Campfire.WoodType))                   // error location
                .WithMessageContaining("constraint is inapplicable")                // category
                .WithMessageContaining("[Check.LengthIsAtLeast]")                   // details / explanation
                .WithMessageContaining(nameof(String))                              // details / explanation
                .WithMessageContaining(nameof(Int32));                              // details / explanation
        }

        [TestMethod] public void LengthIsAtLeast_AnchorIsZero_Redundant() {
            // Arrange
            var translator = new Translator();
            var source = typeof(HolyRomanEmperor);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(FieldFunction.LengthOf, nameof(HolyRomanEmperor.Name), ComparisonOperator.GTE, 0).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void LengthIsAtLeast_AnchorIsNegative_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(LaborOfHeracles);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(LaborOfHeracles.Target))              // error location
                .WithMessageContaining("[Check.LengthIsAtLeast]")                   // details / explanation
                .WithMessageContaining("negative")                                  // details / explanation
                .WithMessageContaining("-144");                                     // details / explanation
        }

        [TestMethod] public void LengthIsAtLeast_ScalarConstrainedMultipleTimes() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Bagel);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(FieldFunction.LengthOf, nameof(Bagel.Flavor), ComparisonOperator.GTE, 34).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void LengthIsAtLeast_PathIsNull_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Localization);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Localization.LocalizedValue))         // error location
                .WithMessageContaining("path is null")                              // category
                .WithMessageContaining("[Check.LengthIsAtLeast]");                  // details / explanation
        }

        [TestMethod] public void LengthIsAtLeast_PathOnScalar_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Histogram);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Histogram.BucketUnit))                // error location
                .WithMessageContaining("path*does not exist")                       // category
                .WithMessageContaining("[Check.LengthIsAtLeast]")                   // details / explanation
                .WithMessageContaining("\"---\"");                                  // details / explanation
        }

        [TestMethod] public void LengthIsAtLeast_NonExistentPathOnAggregate_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Cactus);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Cactus.ScientificName))               // error location
                .WithMessageContaining("path*does not exist")                       // category
                .WithMessageContaining("[Check.LengthIsAtLeast]")                   // details / explanation
                .WithMessageContaining("\"---\"");                                  // details / explanation
        }

        [TestMethod] public void LengthIsAtLeast_NoPathOnAggregate_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(SederPlate);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(SederPlate.Karpas))                   // error location
                .WithMessageContaining("path is required")                          // category
                .WithMessageContaining("[Check.LengthIsAtLeast]");                  // details / explanation
        }

        [TestMethod] public void LengthIsAtLeast_DefaultValueDoesNotSatisfyConstraint_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(MaskedSinger);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(MaskedSinger.Costume))                // error location
                .WithMessageContaining("default*does not satisfy constraints")      // category
                .WithMessageContaining("one or more [Check.xxx] constraints")       // details / explanation
                .WithMessageContaining("\"Pelican\"")                               // details / explanation
                .WithMessageContaining("length is 7")                               // details / explanation
                .WithMessageContaining("is not in interval [289, +∞)");             // details / explanation
        }
    }

    [TestClass, TestCategory("Constraints - String Length")]
    public class LengthIsAtMostTests {
        [TestMethod] public void LengthIsAtMost_NonNullableStringField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Snake);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(FieldFunction.LengthOf, nameof(Snake.Genus), ComparisonOperator.LTE, 175).And
                .HaveConstraint(FieldFunction.LengthOf, nameof(Snake.Species), ComparisonOperator.LTE, 13512).And
                .HaveConstraint(FieldFunction.LengthOf, nameof(Snake.CommonName), ComparisonOperator.LTE, 25).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void LengthIsAtMost_NullableStringField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(WinterStorm);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(FieldFunction.LengthOf, nameof(WinterStorm.Name), ComparisonOperator.LTE, 300).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void LengthIsAtMost_NumericField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(GasStation);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(GasStation.DeiselPrice))              // error location
                .WithMessageContaining("constraint is inapplicable")                // category
                .WithMessageContaining("[Check.LengthIsAtMost]")                    // details / explanation
                .WithMessageContaining(nameof(String))                              // details / explanation
                .WithMessageContaining(nameof(Decimal));                            // details / explanation
        }

        [TestMethod] public void LengthIsAtMost_CharacterField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(BinaryTest);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(BinaryTest.False))                    // error location
                .WithMessageContaining("constraint is inapplicable")                // category
                .WithMessageContaining("[Check.LengthIsAtMost]")                    // details / explanation
                .WithMessageContaining(nameof(String))                              // details / explanation
                .WithMessageContaining(nameof(Char));                               // details / explanation
        }

        [TestMethod] public void LengthIsAtMost_BooleanField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Diamond);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Diamond.IsBloodDiamond))              // error location
                .WithMessageContaining("constraint is inapplicable")                // category
                .WithMessageContaining("[Check.LengthIsAtMost]")                    // details / explanation
                .WithMessageContaining(nameof(String))                              // details / explanation
                .WithMessageContaining(nameof(Boolean));                            // details / explanation
        }

        [TestMethod] public void LengthIsAtMost_DateTimeField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Marathon);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Marathon.Date))                       // error location
                .WithMessageContaining("constraint is inapplicable")                // category
                .WithMessageContaining("[Check.LengthIsAtMost]")                    // details / explanation
                .WithMessageContaining(nameof(String))                              // details / explanation
                .WithMessageContaining(nameof(DateTime));                           // details / explanation
        }

        [TestMethod] public void LengthIsAtMost_GuidField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(CppHeader);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(CppHeader.ModuleID))                  // error location
                .WithMessageContaining("constraint is inapplicable")                // category
                .WithMessageContaining("[Check.LengthIsAtMost]")                    // details / explanation
                .WithMessageContaining(nameof(String))                              // details / explanation
                .WithMessageContaining(nameof(Guid));                               // details / explanation
        }

        [TestMethod] public void LengthIsAtMost_EnumerationField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(ComputerVirus);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(ComputerVirus.Classification))        // error location
                .WithMessageContaining("constraint is inapplicable")                // category
                .WithMessageContaining("[Check.LengthIsAtMost]")                    // details / explanation
                .WithMessageContaining(nameof(String))                              // details / explanation
                .WithMessageContaining(nameof(ComputerVirus.Type));                 // details / explanation
        }

        [TestMethod] public void LengthIsAtMost_NestedApplicableScalar() {
            // Arrange
            var translator = new Translator();
            var source = typeof(MafiaFamily);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(FieldFunction.LengthOf, "Capo.FirstName", ComparisonOperator.LTE, 26).And
                .HaveConstraint(FieldFunction.LengthOf, "Capo.MiddleName", ComparisonOperator.LTE, 71).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void LengthIsAtMost_NestedInapplicableScalar_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Kayak);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Kayak.KayakSeat))                     // error location
                .WithMessageContaining("\"Radius\"")                                // nested path
                .WithMessageContaining("constraint is inapplicable")                // category
                .WithMessageContaining("[Check.LengthIsAtMost]")                    // details / explanation
                .WithMessageContaining(nameof(String))                              // details / explanation
                .WithMessageContaining(nameof(Double));                             // details / explanation
        }

        [TestMethod] public void LengthIsAtMost_NestedAggregate_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(MaddenNFL);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(MaddenNFL.CoverPlayer))               // error location
                .WithMessageContaining("refers to a non-scalar")                    // category
                .WithMessageContaining("[Check.LengthIsAtMost]")                    // details / explanation
                .WithMessageContaining("\"Name\"");                                 // details / explanation
        }

        [TestMethod] public void LengthIsAtMost_FieldWithStringDataConversionTarget() {
            // Arrange
            var translator = new Translator();
            var source = typeof(OilSpill);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(FieldFunction.LengthOf, nameof(OilSpill.Volume), ComparisonOperator.LTE, 14).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void LengthIsAtMost_FieldWithNumericDataConversionSource_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(RandomNumberGenerator);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(RandomNumberGenerator.Algorithm))     // error location
                .WithMessageContaining("constraint is inapplicable")                // category
                .WithMessageContaining("[Check.LengthIsAtMost]")                    // details / explanation
                .WithMessageContaining(nameof(String))                              // details / explanation
                .WithMessageContaining(nameof(Int32));                              // details / explanation
        }

        [TestMethod] public void LengthIsAtMost_AnchorIsZero() {
            // Arrange
            var translator = new Translator();
            var source = typeof(KnockKnockJoke);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(FieldFunction.LengthOf, nameof(KnockKnockJoke.SetUp), ComparisonOperator.LTE, 0).And
                .HaveConstraint(FieldFunction.LengthOf, nameof(KnockKnockJoke.PunchLine), ComparisonOperator.LTE, 0).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void LengthIsAtMost_AnchorIsNegative_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Fraternity);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Fraternity.Name))                     // error location
                .WithMessageContaining("[Check.LengthIsAtMost]")                    // details / explanation
                .WithMessageContaining("negative")                                  // details / explanation
                .WithMessageContaining("-7");                                       // details / explanation
        }

        [TestMethod] public void LengthIsAtMost_ScalarConstrainedMultipleTimes() {
            // Arrange
            var translator = new Translator();
            var source = typeof(OceanicTrench);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(FieldFunction.LengthOf, nameof(OceanicTrench.Location), ComparisonOperator.LTE, 60).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void LengthIsAtMost_PathIsNull_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Passport);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Passport.PassportNumber))             // error location
                .WithMessageContaining("path is null")                              // category
                .WithMessageContaining("[Check.LengthIsAtMost]");                   // details / explanation
        }

        [TestMethod] public void LengthIsAtMost_PathOnScalar_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Nebula);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Nebula.Name))                         // error location
                .WithMessageContaining("path*does not exist")                       // category
                .WithMessageContaining("[Check.LengthIsAtMost]")                    // details / explanation
                .WithMessageContaining("\"---\"");                                  // details / explanation
        }

        [TestMethod] public void LengthIsAtMost_NonExistentPathOnAggregate_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(ImaginaryFriend);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(ImaginaryFriend.Features))            // error location
                .WithMessageContaining("path*does not exist")                       // category
                .WithMessageContaining("[Check.LengthIsAtMost]")                    // details / explanation
                .WithMessageContaining("\"---\"");                                  // details / explanation
        }

        [TestMethod] public void LengthIsAtMost_NoPathOnAggregate_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Newscast);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Newscast.Sports))                     // error location
                .WithMessageContaining("path is required")                          // category
                .WithMessageContaining("[Check.LengthIsAtMost]");                   // details / explanation
        }

        [TestMethod] public void LengthIsAtMost_DefaultValueDoesNotSatisfyConstraint_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Obi);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Obi.Color))                           // error location
                .WithMessageContaining("default*does not satisfy constraints")      // category
                .WithMessageContaining("one or more [Check.xxx] constraints")       // details / explanation
                .WithMessageContaining("\"White\"")                                 // details / explanation
                .WithMessageContaining("length is 5")                               // details / explanation
                .WithMessageContaining("is not in interval [0, 3]");                // details / explanation
        }
    }

    [TestClass, TestCategory("Constraints - String Length")]
    public class LengthIsBetweenTests {
        [TestMethod] public void LengthIsBetween_NonNullableStringField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Sorority);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(FieldFunction.LengthOf, nameof(Sorority.Motto), ComparisonOperator.GTE, 4).And
                .HaveConstraint(FieldFunction.LengthOf, nameof(Sorority.Motto), ComparisonOperator.LTE, 1713).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void LengthIsBetween_NullableStringField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Telescope);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(FieldFunction.LengthOf, nameof(Telescope.Name), ComparisonOperator.GTE, 1).And
                .HaveConstraint(FieldFunction.LengthOf, nameof(Telescope.Name), ComparisonOperator.LTE, int.MaxValue).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void LengthIsBetween_NumericField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Capacitor);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Capacitor.Capacitance))               // error location
                .WithMessageContaining("constraint is inapplicable")                // category
                .WithMessageContaining("[Check.LengthIsBetween]")                   // details / explanation
                .WithMessageContaining(nameof(String))                              // details / explanation
                .WithMessageContaining(nameof(Single));                             // details / explanation
        }

        [TestMethod] public void LengthIsBetween_CharacterField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Lipstick);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Lipstick.Quality))                    // error location
                .WithMessageContaining("constraint is inapplicable")                // category
                .WithMessageContaining("[Check.LengthIsBetween]")                   // details / explanation
                .WithMessageContaining(nameof(String))                              // details / explanation
                .WithMessageContaining(nameof(Char));                               // details / explanation
        }

        [TestMethod] public void LengthIsBetween_BooleanField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Process);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Process.IsActive))                    // error location
                .WithMessageContaining("constraint is inapplicable")                // category
                .WithMessageContaining("[Check.LengthIsBetween]")                   // details / explanation
                .WithMessageContaining(nameof(String))                              // details / explanation
                .WithMessageContaining(nameof(Boolean));                            // details / explanation
        }

        [TestMethod] public void LengthIsBetween_DateTimeField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Mummy);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Mummy.Discovered))                    // error location
                .WithMessageContaining("constraint is inapplicable")                // category
                .WithMessageContaining("[Check.LengthIsBetween]")                   // details / explanation
                .WithMessageContaining(nameof(String))                              // details / explanation
                .WithMessageContaining(nameof(DateTime));                           // details / explanation
        }

        [TestMethod] public void LengthIsBetween_GuidField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(CelticGod);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(CelticGod.DeityID))                   // error location
                .WithMessageContaining("constraint is inapplicable")                // category
                .WithMessageContaining("[Check.LengthIsBetween]")                   // details / explanation
                .WithMessageContaining(nameof(String))                              // details / explanation
                .WithMessageContaining(nameof(Guid));                               // details / explanation
        }

        [TestMethod] public void LengthIsBetween_EnumerationField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Kinesis);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Kinesis.Kind))                        // error location
                .WithMessageContaining("constraint is inapplicable")                // category
                .WithMessageContaining("[Check.LengthIsBetween]")                   // details / explanation
                .WithMessageContaining(nameof(String))                              // details / explanation
                .WithMessageContaining(nameof(Kinesis.Group));                      // details / explanation
        }

        [TestMethod] public void LengthIsBetween_NestedApplicableScalar() {
            // Arrange
            var translator = new Translator();
            var source = typeof(LiteraryTrope);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(FieldFunction.LengthOf, "FirstAppearance.Work", ComparisonOperator.GTE, 1).And
                .HaveConstraint(FieldFunction.LengthOf, "FirstAppearance.Work", ComparisonOperator.LTE, 20).And
                .HaveConstraint(FieldFunction.LengthOf, "FirstAppearance.Author", ComparisonOperator.GTE, 35).And
                .HaveConstraint(FieldFunction.LengthOf, "FirstAppearance.Author", ComparisonOperator.LTE, 100).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void LengthIsBetween_NestedInapplicableScalar_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(OvernightCamp);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(OvernightCamp.Schedule))              // error location
                .WithMessageContaining("\"Sessions\"")                              // nested path
                .WithMessageContaining("constraint is inapplicable")                // category
                .WithMessageContaining("[Check.LengthIsBetween]")                   // details / explanation
                .WithMessageContaining(nameof(String))                              // details / explanation
                .WithMessageContaining(nameof(UInt32));                             // details / explanation
        }

        [TestMethod] public void LengthIsBetween_NestedAggregate_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Dentist);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Dentist.Qualifications))              // error location
                .WithMessageContaining("refers to a non-scalar")                    // category
                .WithMessageContaining("[Check.LengthIsBetween]")                   // details / explanation
                .WithMessageContaining("\"Doctorate\"");                            // details / explanation
        }

        [TestMethod] public void LengthIsBetween_FieldWithStringDataConversionTarget() {
            // Arrange
            var translator = new Translator();
            var source = typeof(AesSedai);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(FieldFunction.LengthOf, nameof(AesSedai.Ajah), ComparisonOperator.GTE, 1).And
                .HaveConstraint(FieldFunction.LengthOf, nameof(AesSedai.Ajah), ComparisonOperator.LTE, 15).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void LengthIsBetween_FieldWithNumericDataConversionSource_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(AtmosphericLayer);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(AtmosphericLayer.Name))               // error location
                .WithMessageContaining("constraint is inapplicable")                // category
                .WithMessageContaining("[Check.LengthIsBetween]")                   // details / explanation
                .WithMessageContaining(nameof(String))                              // details / explanation
                .WithMessageContaining(nameof(Int32));                              // details / explanation
        }

        [TestMethod] public void LengthIsBetween_LowerBoundEqualsUpperBound() {
            // Arrange
            var translator = new Translator();
            var source = typeof(DNACodon);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(FieldFunction.LengthOf, nameof(DNACodon.CodonSequence), ComparisonOperator.EQ, 3).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void LengthIsBetween_LowerBoundGreaterThanUpperBound_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(ChristmasCarol);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(ChristmasCarol.FirstVerse))           // error location
                .WithMessageContaining("conflicting constraints")                   // category
                .WithMessageContaining("one or more [Check.xxx] constraints")       // details / explanation
                .WithMessageContaining("[28841, 1553]");                            // details / explanation
        }

        [TestMethod] public void LengthIsBetween_NegativeLowerBound_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(ShenGongWu);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(ShenGongWu.InitialEpisode))           // error location
                .WithMessageContaining("[Check.LengthIsBetween]")                   // details / explanation
                .WithMessageContaining("negative")                                  // details / explanation
                .WithMessageContaining("-4");                                       // details / explanation
        }

        [TestMethod] public void LengthIsBetween_NegativeLowerAndUpperBounds_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(MilitaryBase);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(MilitaryBase.Commander))              // error location
                .WithMessageContaining("[Check.LengthIsBetween]")                   // details / explanation
                .WithMessageContaining("negative")                                  // details / explanation
                .WithMessageContaining("-156");                                     // details / explanation
        }

        [TestMethod] public void LengthIsBetween_ScalarConstrainedMultipleTimes() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Aria);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(FieldFunction.LengthOf, nameof(Aria.Lyrics), ComparisonOperator.GTE, 27).And
                .HaveConstraint(FieldFunction.LengthOf, nameof(Aria.Lyrics), ComparisonOperator.LTE, 100).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void LengthIsBetween_PathIsNull_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Apocalypse);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Apocalypse.SourceMaterial))           // error location
                .WithMessageContaining("path is null")                              // category
                .WithMessageContaining("[Check.LengthIsBetween]");                  // details / explanation
        }

        [TestMethod] public void LengthIsBetween_PathOnScalar_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(SetCard);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(SetCard.Pattern))                     // error location
                .WithMessageContaining("path*does not exist")                       // category
                .WithMessageContaining("[Check.LengthIsBetween]")                   // details / explanation
                .WithMessageContaining("\"---\"");                                  // details / explanation
        }

        [TestMethod] public void LengthIsBetween_NonExistentPathOnAggregate_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(InternetCraze);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(InternetCraze.Dangers))               // error location
                .WithMessageContaining("path*does not exist")                       // category
                .WithMessageContaining("[Check.LengthIsBetween]")                   // details / explanation
                .WithMessageContaining("\"---\"");                                  // details / explanation
        }

        [TestMethod] public void LengthIsBetween_NoPathOnAggregate_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(MesopotamianGod);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(MesopotamianGod.Names))               // error location
                .WithMessageContaining("path is required")                          // category
                .WithMessageContaining("[Check.LengthIsBetween]");                  // details / explanation
        }

        [TestMethod] public void LengthIsBetween_DefaultValueDoesNotSatisfyConstraint_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(PeanutButter);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(PeanutButter.Brand))                  // error location
                .WithMessageContaining("default*does not satisfy constraints")      // category
                .WithMessageContaining("one or more [Check.xxx] constraints")       // details / explanation
                .WithMessageContaining("\"Smucker's\"")                             // details / explanation
                .WithMessageContaining("length is 9")                               // details / explanation
                .WithMessageContaining("is not in interval [4, 8]");                // details / explanation
        }
    }
}
