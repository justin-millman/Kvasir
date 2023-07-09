using FluentAssertions;
using Kvasir.Exceptions;
using Kvasir.Schema;
using Kvasir.Translation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

using static UT.Kvasir.Translation.SignednessConstraints.IsPositive;
using static UT.Kvasir.Translation.SignednessConstraints.IsNegative;
using static UT.Kvasir.Translation.SignednessConstraints.IsNonZero;

namespace UT.Kvasir.Translation {
    [TestClass, TestCategory("Constraints - Signedness")]
    public class IsPositiveTests {
        [TestMethod] public void IsPositive_NonNullableNumericFields() {
            // Arrange
            var translator = new Translator();
            var source = typeof(GreekLetter);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(GreekLetter.NumericValue), ComparisonOperator.GT, 0).And
                .HaveConstraint(nameof(GreekLetter.Frequency), ComparisonOperator.GT, (decimal)0).And
                .HaveConstraint(nameof(GreekLetter.Index), ComparisonOperator.GT, (byte)0).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsPositive_NullableNumericFields() {
            // Arrange
            var translator = new Translator();
            var source = typeof(MedicalSpecialty);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(MedicalSpecialty.Practitioners), ComparisonOperator.GT, 0UL).And
                .HaveConstraint(nameof(MedicalSpecialty.AverageSalary), ComparisonOperator.GT, (decimal)0).And
                .HaveConstraint(nameof(MedicalSpecialty.YearsSchool), ComparisonOperator.GT, (sbyte)0).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsPositive_TextualField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(FieldGoal);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(FieldGoal.Kicker))                    // error location
                .WithMessageContaining("constraint is inapplicable")                // category
                .WithMessageContaining("[Check.IsPositive]")                        // details / explanation
                .WithMessageContaining("numeric")                                   // details / explanation
                .WithMessageContaining(nameof(String));                             // details / explanation
        }

        [TestMethod] public void IsPositive_BooleanField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(GolfHole);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(GolfHole.ContainsWaterHazard))        // error location
                .WithMessageContaining("constraint is inapplicable")                // category
                .WithMessageContaining("[Check.IsPositive]")                        // details / explanation
                .WithMessageContaining("numeric")                                   // details / explanation
                .WithMessageContaining(nameof(Boolean));                            // details / explanation
        }

        [TestMethod] public void IsPositive_DateTimeField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Gymnast);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Gymnast.Birthdate))                   // error location
                .WithMessageContaining("constraint is inapplicable")                // category
                .WithMessageContaining("[Check.IsPositive]")                        // details / explanation
                .WithMessageContaining("numeric")                                   // details / explanation
                .WithMessageContaining(nameof(DateTime));                           // details / explanation
        }

        [TestMethod] public void IsPositive_GuidField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Documentary);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Documentary.ID))                      // error location
                .WithMessageContaining("constraint is inapplicable")                // category
                .WithMessageContaining("[Check.IsPositive]")                        // details / explanation
                .WithMessageContaining("numeric")                                   // details / explanation
                .WithMessageContaining(nameof(Guid));                               // details / explanation
        }

        [TestMethod] public void IsPositive_EnumerationField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Mythbusting);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Mythbusting.Rating))                  // error location
                .WithMessageContaining("constraint is inapplicable")                // category
                .WithMessageContaining("[Check.IsPositive]")                        // details / explanation
                .WithMessageContaining("numeric")                                   // details / explanation
                .WithMessageContaining(nameof(Mythbusting.Resolution));             // details / explanation
        }

        [TestMethod] public void IsPositive_NestedApplicableScalar() {
            // Arrange
            var translator = new Translator();
            var source = typeof(IceAge);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("Ago.Length", ComparisonOperator.GT, (short)0).And
                .HaveConstraint("Ago.SubLength", ComparisonOperator.GT, (short)0).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsPositive_NestedInapplicableScalar_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(GoldenRaspberry);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(GoldenRaspberry.RunnerUp))            // error location
                .WithMessageContaining("\"Movie\"")                                 // nested path
                .WithMessageContaining("constraint is inapplicable")                // category
                .WithMessageContaining("[Check.IsPositive]")                        // details / explanation
                .WithMessageContaining("numeric")                                   // details / explanation
                .WithMessageContaining(nameof(String));                             // details / explanation
        }

        [TestMethod] public void IsPositive_NestedAggregate_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(SudokuPuzzle);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(SudokuPuzzle.LowerLeft))              // error location
                .WithMessageContaining("refers to a non-scalar")                    // category
                .WithMessageContaining("[Check.IsPositive]")                        // details / explanation
                .WithMessageContaining("\"Bottom\"");                               // details / explanation
        }

        [TestMethod] public void IsPositive_FieldWithNumericDataConversionTarget() {
            // Arrange
            var translator = new Translator();
            var source = typeof(SwimmingPool);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(SwimmingPool.Classification), ComparisonOperator.GT, 0).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsPositive_FieldWithNumericDataConversionSource_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(WikipediaPage);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(WikipediaPage.Languages))             // error location
                .WithMessageContaining("constraint is inapplicable")                // category
                .WithMessageContaining("[Check.IsPositive]")                        // details / explanation
                .WithMessageContaining("numeric")                                   // details / explanation
                .WithMessageContaining(nameof(String));                             // details / explanation
        }

        [TestMethod] public void IsPositive_ScalarConstrainedMultipleTimes_Redundant() {
            // Arrange
            var translator = new Translator();
            var source = typeof(BaseballCard);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(BaseballCard.CardNumber), ComparisonOperator.GT, 0).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsPositive_PathIsNull_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(HotSpring);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(HotSpring.Elevation))                 // error location
                .WithMessageContaining("path is null")                              // category
                .WithMessageContaining("[Check.IsPositive]");                       // details / explanation
        }

        [TestMethod] public void IsPositive_PathOnScalar_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Canal);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Canal.Length))                        // error location
                .WithMessageContaining("path*does not exist")                       // category
                .WithMessageContaining("[Check.IsPositive]")                        // details / explanation
                .WithMessageContaining("\"---\"");                                  // details / explanation
        }

        [TestMethod] public void IsPositive_NonExistentPathOnAggregate_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(SharkWeek);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(SharkWeek.Info))                      // error location
                .WithMessageContaining("path*does not exist")                       // category
                .WithMessageContaining("[Check.IsPositive]")                        // details / explanation
                .WithMessageContaining("\"---\"");                                  // details / explanation
        }

        [TestMethod] public void IsPositive_NoPathOnAggregate_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Philosopher);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Philosopher.Name))                    // error location
                .WithMessageContaining("path is required")                          // category
                .WithMessageContaining("[Check.IsPositive]");                       // details / explanation
        }

        [TestMethod] public void IsPositive_DefaultValueDoesNotSatisfyConstraint_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(ScoobyDooFilm);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(ScoobyDooFilm.Runtime))               // error location
                .WithMessageContaining("default*does not satisfy constraints")      // category
                .WithMessageContaining("one or more [Check.xxx] constraints")       // details / explanation
                .WithMessageContaining("-89")                                       // details / explanation
                .WithMessageContaining("(0, +∞)");                                  // details / explanation
        }
    }

    [TestClass, TestCategory("Constraints - Signedness")]
    public class IsNegativeTests {
        [TestMethod] public void IsNegative_SignedNonNullableNumericFields() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Acid);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(Acid.pH), ComparisonOperator.LT, 0f).And
                .HaveConstraint(nameof(Acid.FreezingPoint), ComparisonOperator.LT, (short)0).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNegative_UnsignedNumericFields_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Cereal);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Cereal.CaloriesPerServing))           // error location
                .WithMessageContaining("constraint is inapplicable")                // category
                .WithMessageContaining("[Check.IsNegative]")                        // details / explanation
                .WithMessageContaining("unsigned")                                  // details / explanation
                .WithMessageContaining(nameof(UInt16));                             // details / explanation
        }

        [TestMethod] public void IsNegative_SignedNullableNumericFields() {
            // Arrange
            var translator = new Translator();
            var source = typeof(ConcentrationCamp);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(ConcentrationCamp.Inmates), ComparisonOperator.LT, 0.0).And
                .HaveConstraint(nameof(ConcentrationCamp.Casualties), ComparisonOperator.LT, 0L).And
                .HaveConstraint(nameof(ConcentrationCamp.DaysOperational), ComparisonOperator.LT, (short)0).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNegative_TextualField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(KeySignature);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(KeySignature.Note))                   // error location
                .WithMessageContaining("constraint is inapplicable")                // category
                .WithMessageContaining("[Check.IsNegative]")                        // details / explanation
                .WithMessageContaining("numeric")                                   // details / explanation
                .WithMessageContaining(nameof(Char));                               // details / explanation
        }

        [TestMethod] public void IsNegative_BooleanField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(SporcleQuiz);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(SporcleQuiz.Published))               // error location
                .WithMessageContaining("constraint is inapplicable")                // category
                .WithMessageContaining("[Check.IsNegative]")                        // details / explanation
                .WithMessageContaining("numeric")                                   // details / explanation
                .WithMessageContaining(nameof(Boolean));                            // details / explanation
        }

        [TestMethod] public void IsNegative_DateTimeField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Olympiad);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Olympiad.OpeningCeremony))            // error location
                .WithMessageContaining("constraint is inapplicable")                // category
                .WithMessageContaining("[Check.IsNegative]")                        // details / explanation
                .WithMessageContaining("numeric")                                   // details / explanation
                .WithMessageContaining(nameof(DateTime));                           // details / explanation
        }

        [TestMethod] public void IsNegative_GuidField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(W2);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(W2.FormID))                           // error location
                .WithMessageContaining("constraint is inapplicable")                // category
                .WithMessageContaining("[Check.IsNegative]")                        // details / explanation
                .WithMessageContaining("numeric")                                   // details / explanation
                .WithMessageContaining(nameof(Guid));                               // details / explanation
        }

        [TestMethod] public void IsNegative_EnumerationField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(SerialKiller);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(SerialKiller.CurrentStatus))          // error location
                .WithMessageContaining("constraint is inapplicable")                // category
                .WithMessageContaining("[Check.IsNegative]")                        // details / explanation
                .WithMessageContaining("numeric")                                   // details / explanation
                .WithMessageContaining(nameof(SerialKiller.Status));                // details / explanation
        }

        [TestMethod] public void IsNegative_NestedApplicableScalar() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Flood);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("Occurrence.Month", ComparisonOperator.LT, (sbyte)0).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNegative_NestedInapplicableScalar_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(TrolleyProblem);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(TrolleyProblem.Pull))                 // error location
                .WithMessageContaining("\"Label\"")                                 // nested path
                .WithMessageContaining("constraint is inapplicable")                // category
                .WithMessageContaining("[Check.IsNegative]")                        // details / explanation
                .WithMessageContaining("numeric")                                   // details / explanation
                .WithMessageContaining(nameof(String));                             // details / explanation
        }

        [TestMethod] public void IsNegative_NestedAggregate_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Pharaoh);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Pharaoh.Dynasty))                     // error location
                .WithMessageContaining("refers to a non-scalar")                    // category
                .WithMessageContaining("[Check.IsNegative]")                        // details / explanation
                .WithMessageContaining("\"Kingdom\"");                              // details / explanation
        }

        [TestMethod] public void IsNegative_FieldWithNumericDataConversionTarget() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Boxer);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(Boxer.TKOs), ComparisonOperator.LT, 0).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNegative_FieldWithNumericDataConversionSource_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Archangel);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Archangel.FirstAppearance))           // error location
                .WithMessageContaining("constraint is inapplicable")                // category
                .WithMessageContaining("[Check.IsNegative]")                        // details / explanation
                .WithMessageContaining("numeric")                                   // details / explanation
                .WithMessageContaining(nameof(String));                             // details / explanation
        }

        [TestMethod] public void IsNegative_ScalarConstrainedMultipleTimes_Redundant() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Alkene);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(Alkene.FreezingPoint), ComparisonOperator.LT, 0.0).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNegative_PathIsNull_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Climate);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Climate.AverageLowTemperature))       // error location
                .WithMessageContaining("path is null")                              // category
                .WithMessageContaining("[Check.IsNegative]");                       // details / explanation
        }

        [TestMethod] public void IsNegative_PathOnScalar_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(CircleOfHell);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(CircleOfHell.Level))                  // error location
                .WithMessageContaining("path*does not exist")                       // category
                .WithMessageContaining("[Check.IsNegative]")                        // details / explanation
                .WithMessageContaining("\"---\"");                                  // details / explanation
        }

        [TestMethod] public void IsNegative_NonExistentPathOnAggregate_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(VolleyballMatch);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(VolleyballMatch.FourthSet))           // error location
                .WithMessageContaining("path*does not exist")                       // category
                .WithMessageContaining("[Check.IsNegative]")                        // details / explanation
                .WithMessageContaining("\"---\"");                                  // details / explanation
        }

        [TestMethod] public void IsNegative_NoPathOnAggregate_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Yacht);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Yacht.Sails))                         // error location
                .WithMessageContaining("path is required")                          // category
                .WithMessageContaining("[Check.IsNegative]");                       // details / explanation
        }

        [TestMethod] public void IsNegative_DefaultValueDoesNotSatisfyConstraint_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(SuperPAC);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(SuperPAC.TotalRaised))                // error location
                .WithMessageContaining("default*does not satisfy constraints")      // category
                .WithMessageContaining("one or more [Check.xxx] constraints")       // details / explanation
                .WithMessageContaining("0")                                         // details / explanation
                .WithMessageContaining("(-∞, 0)");                                  // details / explanation
        }
    }

    [TestClass, TestCategory("Constraints - Signedness")]
    public class IsNonZeroTests {
        [TestMethod] public void IsNonZero_NonNullableNumericFields() {
            // Arrange
            var translator = new Translator();
            var source = typeof(RegularPolygon);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(RegularPolygon.NumEdges), ComparisonOperator.NE, (ushort)0).And
                .HaveConstraint(nameof(RegularPolygon.NumVertices), ComparisonOperator.NE, (sbyte)0).And
                .HaveConstraint(nameof(RegularPolygon.InternalAngle), ComparisonOperator.NE, 0.0).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNonZero_NullableNumericFields() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Skittles);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(Skittles.Weight), ComparisonOperator.NE, 0.0).And
                .HaveConstraint(nameof(Skittles.ServingSizeCalories), ComparisonOperator.NE, (short)0).And
                .HaveConstraint(nameof(Skittles.PiecesPerBag), ComparisonOperator.NE, 0U).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNonZero_TextualField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Brassiere);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Brassiere.CupSize))                   // error location
                .WithMessageContaining("constraint is inapplicable")                // category
                .WithMessageContaining("[Check.IsNonZero]")                         // details / explanation
                .WithMessageContaining("numeric")                                   // details / explanation
                .WithMessageContaining(nameof(String));                             // details / explanation
        }

        [TestMethod] public void IsNonZero_BooleanField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(FutharkRune);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(FutharkRune.InYoungerFuthark))        // error location
                .WithMessageContaining("constraint is inapplicable")                // category
                .WithMessageContaining("[Check.IsNonZero]")                         // details / explanation
                .WithMessageContaining("numeric")                                   // details / explanation
                .WithMessageContaining(nameof(Boolean));                            // details / explanation
        }

        [TestMethod] public void IsNonZero_DateTimeField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(FinalFour);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(FinalFour.ChampionshipGame))          // error location
                .WithMessageContaining("constraint is inapplicable")                // category
                .WithMessageContaining("[Check.IsNonZero]")                         // details / explanation
                .WithMessageContaining("numeric")                                   // details / explanation
                .WithMessageContaining(nameof(DateTime));                           // details / explanation
        }

        [TestMethod] public void IsNonZero_GuidField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Fractal);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Fractal.FractalID))                   // error location
                .WithMessageContaining("constraint is inapplicable")                // category
                .WithMessageContaining("[Check.IsNonZero]")                         // details / explanation
                .WithMessageContaining("numeric")                                   // details / explanation
                .WithMessageContaining(nameof(Guid));                               // details / explanation
        }

        [TestMethod] public void IsNonZero_EnumerationField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(IPO);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(IPO.PostingMethod))                   // error location
                .WithMessageContaining("constraint is inapplicable")                // category
                .WithMessageContaining("[Check.IsNonZero]")                         // details / explanation
                .WithMessageContaining("numeric")                                   // details / explanation
                .WithMessageContaining(nameof(IPO.Method));                         // details / explanation
        }

        [TestMethod] public void IsNonZero_NestedApplicableScalar() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Essay);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("P1.S5.WordCount", ComparisonOperator.NE, 0).And
                .HaveConstraint("P1.S3.WordCount", ComparisonOperator.NE, 0).And
                .HaveConstraint("P3.S2.WordCount", ComparisonOperator.NE, 0).And
                .HaveConstraint("P5.S1.WordCount", ComparisonOperator.NE, 0).And
                .HaveConstraint("P5.S4.WordCount", ComparisonOperator.NE, 0).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNonZero_NestedInapplicableScalar_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(IDE);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(IDE.Version))                         // error location
                .WithMessageContaining("\"Released\"")                              // nested path
                .WithMessageContaining("constraint is inapplicable")                // category
                .WithMessageContaining("[Check.IsNonZero]")                         // details / explanation
                .WithMessageContaining("numeric")                                   // details / explanation
                .WithMessageContaining(nameof(DateTime));                           // details / explanation
        }

        [TestMethod] public void IsNonZero_NestedAggregate_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(PregnancyTest);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(PregnancyTest.Product))               // error location
                .WithMessageContaining("refers to a non-scalar")                    // category
                .WithMessageContaining("[Check.IsNonZero]")                         // details / explanation
                .WithMessageContaining("\"Name\"");                                 // details / explanation
        }

        [TestMethod] public void IsNonZero_FieldWithNumericDataConversionTarget() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Airline);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(Airline.ConsumerGrade), ComparisonOperator.NE, 0).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNonZero_FieldWithNumericDataConversionSource_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Elevator);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Elevator.NumFloors))                  // error location
                .WithMessageContaining("constraint is inapplicable")                // category
                .WithMessageContaining("[Check.IsNonZero]")                         // details / explanation
                .WithMessageContaining("numeric")                                   // details / explanation
                .WithMessageContaining(nameof(String));                             // details / explanation
        }

        [TestMethod] public void IsNonZero_ScalarConstrainedMultipleTimes_Redundant() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Shoe);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(Shoe.Mens), ComparisonOperator.NE, 0f).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNonZero_PathIsNull_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Igloo);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Igloo.NumIceBlocks))                  // error location
                .WithMessageContaining("path is null")                              // category
                .WithMessageContaining("[Check.IsNonZero]");                        // details / explanation
        }

        [TestMethod] public void IsNonZero_PathOnScalar_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Cryptocurrency);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Cryptocurrency.ExchangeRate))         // error location
                .WithMessageContaining("path*does not exist")                       // category
                .WithMessageContaining("[Check.IsNonZero]")                         // details / explanation
                .WithMessageContaining("\"---\"");                                  // details / explanation
        }

        [TestMethod] public void IsNonZero_NonExistentPathOnAggregate_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Encomienda);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Encomienda.Location))                 // error location
                .WithMessageContaining("path*does not exist")                       // category
                .WithMessageContaining("[Check.IsNonZero]")                         // details / explanation
                .WithMessageContaining("\"---\"");                                  // details / explanation
        }

        [TestMethod] public void IsNonZero_NoPathOnAggregate_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Smoothie);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Smoothie.Base))                       // error location
                .WithMessageContaining("path is required")                          // category
                .WithMessageContaining("[Check.IsNonZero]");                        // details / explanation
        }

        [TestMethod] public void IsNonZero_DefaultValueDoesNotSatisfyConstraint_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Pulley);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Pulley.RopeTension))                  // error location
                .WithMessageContaining("default*does not satisfy constraints")      // category
                .WithMessageContaining("one or more [Check.xxx] constraints")       // details / explanation
                .WithMessageContaining("0.0")                                       // details / explanation
                .WithMessageContaining("value is explicitly disallowed");           // details / explanation
        }
    }
}
