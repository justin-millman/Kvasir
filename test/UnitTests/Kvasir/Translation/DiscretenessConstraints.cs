using FluentAssertions;
using Kvasir.Exceptions;
using Kvasir.Schema;
using Kvasir.Translation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

using static UT.Kvasir.Translation.DiscretenessConstraints.IsOneOf;
using static UT.Kvasir.Translation.DiscretenessConstraints.IsNotOneOf;

namespace UT.Kvasir.Translation {
    [TestClass, TestCategory("Constraints - Discreteness")]
    public class IsOneOfTests {
        [TestMethod] public void IsOneOf_NumericFields() {
            // Arrange
            var translator = new Translator();
            var source = typeof(CoralReef);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(CoralReef.Longitude), InclusionOperator.In,
                    0f, 30f, 45f, 75f, 90f
                ).And
                .HaveConstraint(nameof(CoralReef.Length), InclusionOperator.In,
                    1000UL, 2000UL, 3000UL, 4000UL, 5000UL
                ).And
                .HaveConstraint(nameof(CoralReef.Area), InclusionOperator.In,
                    17, 190841, 79512759, 857791
                ).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsOneOf_TextualFields() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Encyclopedia);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(Encyclopedia.Letter), InclusionOperator.In,
                    'A', 'B', 'C', 'D', 'E', 'F', 'G'
                ).And
                .HaveConstraint(nameof(Encyclopedia.Edition), InclusionOperator.In,
                    "First", "Second", "Third", "Fourth"
                ).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsOneOf_BooleanField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Astronaut);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(Astronaut.WalkedOnMoon), InclusionOperator.In,
                    true, false
                ).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsOneOf_DecimalField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(CiderMill);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(CiderMill.AnnualTonnage), InclusionOperator.In,
                    (decimal)0, (decimal)100, (decimal)1000, (decimal)10000, (decimal)100000, (decimal)1000000
                ).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsOneOf_DateTimeField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Hospital);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(Hospital.Opened), InclusionOperator.In,
                    new DateTime(2000, 1, 1), new DateTime(2000, 1, 2), new DateTime(2000, 1, 3)
                ).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsOneOf_GuidField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Tsunami);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(Tsunami.TsunamiID), ComparisonOperator.EQ, new Guid("b334ae4e-98c3-4f63-83f8-2bc076eae31b")).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsOneOf_NullableFields() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Wildfire);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(Wildfire.Cause), InclusionOperator.In,
                    "Lightning", "Arson", "Electrical"
                ).And
                .HaveConstraint(nameof(Wildfire.MaxTemperature), InclusionOperator.In,
                    5718.37, 1984.6, 279124.9
                ).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsOneOf_DuplicatedValues() {
            // Arrange
            var translator = new Translator();
            var source = typeof(HealingPotion);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(HealingPotion.DieType), InclusionOperator.In,
                    4u, 8u, 10u, 12u, 20u, 100u
                ).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsOneOf_InconvertibleValue_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Battery);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Battery.Voltage))                     // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Check.IsOneOf]")                           // details / explanation
                .WithMessageContaining($"\"six\" of type {nameof(String)}")         // details / explanation
                .WithMessageContaining(nameof(Int32));                              // details / explanation
        }

        [TestMethod] public void IsOneOf_ConvertibleValue_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(TennisMatch);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(TennisMatch.Player1Score))            // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Check.IsOneOf]")                           // details / explanation
                .WithMessageContaining($"0 of type {nameof(Byte)}")                 // details / explanation
                .WithMessageContaining(nameof(SByte));                              // details / explanation
        }

        [TestMethod] public void IsOneOf_ArrayValue_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Flashcard);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Flashcard.IsLearned))                 // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Check.IsOneOf]")                           // details / explanation
                .WithMessageContaining("array")                                     // details / explanation
                .WithMessageContaining(nameof(Boolean));                            // details / explanation
        }

        [TestMethod] public void IsOneOf_NullValue_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Prophet);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Prophet.HebrewName))                  // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Check.IsOneOf]")                           // details / explanation
                .WithMessageContaining("null");                                     // details / explanation
        }

        [TestMethod] public void IsOneOf_DecimalValueIsNotDouble_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Carousel);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Carousel.RoundDuration))              // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Check.IsOneOf]")                           // details / explanation
                .WithMessageContaining($"0.4 of type {nameof(Single)}")             // details / explanation
                .WithMessageContaining(nameof(Decimal))                             // details / explanation
                .WithMessageContaining(nameof(Double));                             // details / explanation
        }

        [TestMethod] public void IsOneOf_DecimalValueIsOutOfRange_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Borate);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Borate.MolarMass))                    // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Check.IsOneOf]")                           // details / explanation
                .WithMessageContaining(double.MinValue.ToString())                  // details / explanation
                .WithMessageContaining("could not convert")                         // details / explanation
                .WithMessageContaining(nameof(Decimal));                            // details / explanation
        }

        [TestMethod] public void IsOneOf_DateTimeValueIsNotString_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(DalaiLama);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(DalaiLama.Birthdate))                 // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Check.IsOneOf]")                           // details / explanation
                .WithMessageContaining($"1824 of type {nameof(UInt64)}")            // details / explanation
                .WithMessageContaining(nameof(DateTime))                            // details / explanation
                .WithMessageContaining(nameof(String));                             // details / explanation
        }

        [TestMethod] public void IsOneOf_DateTimeValueIsMalformatted_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Voicemail);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Voicemail.When))                      // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Check.IsOneOf]")                           // details / explanation
                .WithMessageContaining("\"Thursday\"")                              // details / explanation
                .WithMessageContaining("could not parse")                           // details / explanation
                .WithMessageContaining(nameof(DateTime));                           // details / explanation
        }

        [TestMethod] public void IsOneOf_DateTimeValueIsOutOfRange_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(FinalJeopardy);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(FinalJeopardy.AirDate))               // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Check.IsOneOf]")                           // details / explanation
                .WithMessageContaining("\"1299-08-45\"")                            // details / explanation
                .WithMessageContaining("could not parse")                           // details / explanation
                .WithMessageContaining(nameof(DateTime));                           // details / explanation
        }

        [TestMethod] public void IsOneOf_GuidValueIsNotString_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(BiologicalCycle);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(BiologicalCycle.ID))                  // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Check.IsOneOf]")                           // details / explanation
                .WithMessageContaining($"'c' of type {nameof(Char)}")               // details / explanation
                .WithMessageContaining(nameof(Guid))                                // details / explanation
                .WithMessageContaining(nameof(String));                             // details / explanation
        }

        [TestMethod] public void IsOneOf_GuidValueIsMalformatted_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(WaterBottle);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(WaterBottle.ProductID))               // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Check.IsOneOf]")                           // details / explanation
                .WithMessageContaining("\"A-G-U-I-D\"")                             // details / explanation
                .WithMessageContaining("could not parse")                           // details / explanation
                .WithMessageContaining(nameof(Guid));                               // details / explanation
        }

        [TestMethod] public void IsOneOf_ValueMatchesDataConversionSourceType_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Burrito);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Burrito.Protein))                     // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Check.IsOneOf]")                           // details / explanation
                .WithMessageContaining($"\"Chicken\" of type {nameof(String)}")     // details / explanation
                .WithMessageContaining(nameof(Int32));                              // details / explanation
        }

        [TestMethod] public void IsOneOf_ValueMatchesDataConversionTargetType() {
            // Arrange
            var translator = new Translator();
            var source = typeof(WaterSlide);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(WaterSlide.Type), InclusionOperator.In,
                    "Straight", "Curly", "Funnel"
                ).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsOneOf_ScalarConstrainedMultipleTimes() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Cannon);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(Cannon.Capacity), InclusionOperator.In,
                    7, 2, 4, 1, 6
                ).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsOneOf_PathIsNull_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Dragon);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Dragon.Species))                      // error location
                .WithMessageContaining("path is null")                              // category
                .WithMessageContaining("[Check.IsOneOf]");                          // details / explanation
        }

        [TestMethod] public void IsOneOf_PathOnScalar_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(HomericHymn);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(HomericHymn.Lines))                   // error location
                .WithMessageContaining("path*does not exist")                       // category
                .WithMessageContaining("[Check.IsOneOf]")                           // details / explanation
                .WithMessageContaining("\"---\"");                                  // details / explanation
        }

        [TestMethod] public void IsOneOf_DefaultValueDoesNotSatisfyConstraint_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Guillotine);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Guillotine.Height))                   // error location
                .WithMessageContaining("default*does not satisfy constraints")      // category
                .WithMessageContaining("one or more [Check.xxx] constraints")       // details / explanation
                .WithMessageContaining("13")                                        // details / explanation
                .WithMessageContaining("{ 30, 60, 90, 120 }");                      // details / explanation
        }
    }

    [TestClass, TestCategory("Constraints - Discreteness")]
    public class IsNotOneOfTests {
        [TestMethod] public void IsNotOneOf_NumericFields() {
            // Arrange
            var translator = new Translator();
            var source = typeof(NationalAnthem);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(NationalAnthem.WordCount), InclusionOperator.NotIn,
                    0U, 5U
                ).And
                .HaveConstraint(nameof(NationalAnthem.Length), InclusionOperator.NotIn,
                    1.3f, 1.6f, 1.9f, 2.2f, 2.5f, 2.8f, 3.1f, 3.4f
                ).And
                .HaveConstraint(nameof(NationalAnthem.Revision), InclusionOperator.NotIn,
                    0L, 1L, 2L
                ).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNotOneOf_TextualFields() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Taxi);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(Taxi.Quality), InclusionOperator.NotIn,
                    '1', '3', '5', '7', '9'
                ).And
                .HaveConstraint(nameof(Taxi.Company), InclusionOperator.NotIn,
                    "YellowCab", "Cash Cab", "Uber", "Lyft"
                ).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNotOneOf_BooleanField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(BirthControl);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(BirthControl.ForWomen), ComparisonOperator.NE, false).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNotOneOf_DecimalField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(HouseCommittee);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(HouseCommittee.Budget), InclusionOperator.NotIn,
                    (decimal)0, (decimal)1000, (decimal)100000, (decimal)100000000
                ).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNotOneOf_DateTimeField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(GamingConsole);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(GamingConsole.Launched), InclusionOperator.NotIn,
                    new DateTime(1973, 4, 30), new DateTime(1973, 5, 30)
                ).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNotOneOf_GuidField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Podcast);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(Podcast.ID), InclusionOperator.NotIn,
                    new Guid("70324253-a5df-4208-9939-44a11243ceb0"), new Guid("2e748258-29e6-4abd-a1e1-3e93262e4c04")
                ).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNotOneOf_NullableFields() {
            // Arrange
            var translator = new Translator();
            var source = typeof(PIERoot);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(PIERoot.FrenchExample), InclusionOperator.NotIn,
                    "Manger", "Faire", "Avoir", "Parler"
                ).And
                .HaveConstraint(nameof(PIERoot.SpanishExample), InclusionOperator.NotIn,
                    "Comer", "Hacer", "Tener", "Hablar"
                ).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNotOneOf_DuplicatedValues() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Tweet);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(Tweet.Grading), InclusionOperator.NotIn,
                    'A', 'E', 'I', 'O', 'U'
                ).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNotOneOf_InconvertibleValue_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Cancer);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Cancer.RegionAffected))               // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Check.IsNotOneOf]")                        // details / explanation
                .WithMessageContaining($"17.3 of type {nameof(Single)}")            // details / explanation
                .WithMessageContaining(nameof(String));                             // details / explanation
        }

        [TestMethod] public void IsNotOneOf_ConvertibleValue_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Avatar);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Avatar.DebutEpisode))                 // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Check.IsNotOneOf]")                        // details / explanation
                .WithMessageContaining($"8 of type {nameof(Byte)}")                 // details / explanation
                .WithMessageContaining(nameof(UInt16));                             // details / explanation
        }

        [TestMethod] public void IsNotOneOf_ArrayValue_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Wristwatch);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Wristwatch.Brand))                    // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Check.IsNotOneOf]")                        // details / explanation
                .WithMessageContaining("array")                                     // details / explanation
                .WithMessageContaining(nameof(String));                             // details / explanation
        }

        [TestMethod] public void IsNotOneOf_NullValue_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Ballet);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Ballet.OpusNumber))                   // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Check.IsNotOneOf]")                        // details / explanation
                .WithMessageContaining("null");                                     // details / explanation
        }

        [TestMethod] public void IsNotOneOf_DecimalValueIsNotDouble_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(AmericanIdol);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(AmericanIdol.VoteShare))              // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Check.IsNotOneOf]")                        // details / explanation
                .WithMessageContaining($"\"0.90\" of type {nameof(String)}")        // details / explanation
                .WithMessageContaining(nameof(Decimal))                             // details / explanation
                .WithMessageContaining(nameof(Double));                             // details / explanation
        }

        [TestMethod] public void IsNotOneOf_DecimalValueIsOutOfRange_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(RussianTsar);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(RussianTsar.DaysReigned))             // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Check.IsNotOneOf]")                        // details / explanation
                .WithMessageContaining(double.MaxValue.ToString())                  // details / explanation
                .WithMessageContaining("could not convert")                         // details / explanation
                .WithMessageContaining(nameof(Decimal));                            // details / explanation
        }

        [TestMethod] public void IsNotOneOf_DateTimeValueIsNotString_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Mayor);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Mayor.TermEnd))                       // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Check.IsNotOneOf]")                        // details / explanation
                .WithMessageContaining($"'T' of type {nameof(Char)}")               // details / explanation
                .WithMessageContaining(nameof(DateTime))                            // details / explanation
                .WithMessageContaining(nameof(String));                             // details / explanation
        }

        [TestMethod] public void IsNotOneOf_DateTimeValueIsMalformatted_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Inator);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Inator.Debut))                        // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Check.IsNotOneOf]")                        // details / explanation
                .WithMessageContaining("\"1875~06~22\"")                            // details / explanation
                .WithMessageContaining("could not parse")                           // details / explanation
                .WithMessageContaining(nameof(DateTime));                           // details / explanation
        }

        [TestMethod] public void IsNotOneOf_DateTimeValueIsOutOfRange_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Museum);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Museum.GrandOpening))                 // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Check.IsNotOneOf]")                        // details / explanation
                .WithMessageContaining("\"1375-49-14\"")                            // details / explanation
                .WithMessageContaining("could not parse")                           // details / explanation
                .WithMessageContaining(nameof(DateTime));                           // details / explanation
        }

        [TestMethod] public void IsNotOneOf_GuidValueIsNotString_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Cruise);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Cruise.CruiseID))                     // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Check.IsNotOneOf]")                        // details / explanation
                .WithMessageContaining($"'f' of type {nameof(Char)}")               // details / explanation
                .WithMessageContaining(nameof(Guid))                                // details / explanation
                .WithMessageContaining(nameof(String));                             // details / explanation
        }

        [TestMethod] public void IsNotOneOf_GuidValueIsMalformatted_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Union);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Union.UnionID))                       // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Check.IsNotOneOf]")                        // details / explanation
                .WithMessageContaining("\"b46cfa0c-545e-4279-93d6-d1236r373a2b\"")  // details / explanation
                .WithMessageContaining("could not parse")                           // details / explanation
                .WithMessageContaining(nameof(Guid));                               // details / explanation
        }

        [TestMethod] public void IsNotOneOf_ValueMatchesDataConversionSourceType_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Guitar);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Guitar.Brand))                        // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Check.IsNotOneOf]")                        // details / explanation
                .WithMessageContaining($"\"Cardboard\" of type {nameof(String)}")   // details / explanation
                .WithMessageContaining(nameof(Int32));                              // details / explanation
        }

        [TestMethod] public void IsNotOneOf_ValueMatchesDataConversionTargetType() {
            // Arrange
            var translator = new Translator();
            var source = typeof(SoccerTeam);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(SoccerTeam.WorldCupVictories), InclusionOperator.NotIn,
                    0, -3, 111
                ).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNotOneOf_BothBooleansDisallowed_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Transformer);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Transformer.IsAutobot))               // error location
                .WithMessageContaining("both true and false explicitly disallowed") // category
                .WithMessageContaining("one or more [Check.xxx] constraints");      // details / explanation
        }

        [TestMethod] public void IsNotOneOf_ScalarConstrainedMultipleTimes() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Eurovision);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(Eurovision.Year), InclusionOperator.NotIn,
                    (ushort)0, (ushort)3
                ).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNotOneOf_PathIsNull_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Tuxedo);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Tuxedo.Size))                         // error location
                .WithMessageContaining("path is null")                              // category
                .WithMessageContaining("[Check.IsNotOneOf]");                       // details / explanation
        }

        [TestMethod] public void IsNotOneOf_PathOnScalar_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Donut);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Donut.Flavor))                        // error location
                .WithMessageContaining("path*does not exist")                       // category
                .WithMessageContaining("[Check.IsNotOneOf]")                        // details / explanation
                .WithMessageContaining("\"---\"");                                  // details / explanation
        }

        [TestMethod] public void IsNotOneOf_DefaultValueDoesNotSatisfyConstraint_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Pie);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Pie.Flavor))                          // error location
                .WithMessageContaining("default*does not satisfy constraints")      // category
                .WithMessageContaining("one or more [Check.xxx] constraints")       // details / explanation
                .WithMessageContaining("\"Anise\"")                                 // details / explanation
                .WithMessageContaining("value is explicitly disallowed");           // details / explanation
        }
    }
}
