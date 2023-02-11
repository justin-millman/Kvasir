using Atropos.Moq;
using Cybele.Core;
using FluentAssertions;
using Kvasir.Core;
using Kvasir.Exceptions;
using Kvasir.Schema;
using Kvasir.Translation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;

using static UT.Kvasir.Translation.TestComponents;

namespace UT.Kvasir.Translation {
    [TestClass, TestCategory("CHECK Constraints")]
    public class ConstraintTests {
        [TestMethod] public void IsPositive_AppliedToNumericField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(GreekLetter);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(GreekLetter.NumericValue), ComparisonOperator.GT, 0).And
                .NoOtherConstraints();
        }

        [TestMethod] public void IsPositive_AppliedToNonNumericField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Gymnast);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                        // source type
                .WithMessage($"*{nameof(Gymnast.Birthdate)}*")          // source property
                .WithMessage("*[Check.IsPositive]*")                    // annotation
                .WithMessage("*numeric*")                               // rationale
                .WithMessage($"*{nameof(DateTime)}*");                  // details
        }

        [TestMethod] public void IsPositive_PathOnAnnotationForScalar_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Canal);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                        // source type
                .WithMessage($"*{nameof(Canal.Length)}*")               // source property
                .WithMessage("*[Check.IsPositive]*")                    // annotation
                .WithMessage("*path*does not exist*")                   // rationale
                .WithMessage("*\"---\"*");                              // details
        }

        [TestMethod] public void IsPositive_MultipleAnnotationsOnSingleProperty_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(BaseballCard);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                            // source type
                .WithMessage($"*{nameof(BaseballCard.CardNumber)}*")        // source property
                .WithMessage("*[Check.IsPositive]*")                        // annotation
                .WithMessage("*multiple*");                                 // rationale
        }

        [TestMethod] public void IsNegative_AppliedToSignedNumericField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Acid);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(Acid.pH), ComparisonOperator.LT, 0f).And
                .NoOtherConstraints();
        }

        [TestMethod] public void IsNegative_AppliedToUnsignedNumericField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Cereal);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                        // source type
                .WithMessage($"*{nameof(Cereal.CaloriesPerServing)}*")  // source property
                .WithMessage("*[Check.IsNegative]*")                    // annotation
                .WithMessage("*unsigned*integer*")                      // rationale
                .WithMessage($"*{nameof(UInt16)}*");                    // details
        }

        [TestMethod] public void IsNegative_AppliedToNonNumericField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(KeySignature);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                        // source type
                .WithMessage($"*{nameof(KeySignature.Note)}*")          // source property
                .WithMessage("*[Check.IsNegative]*")                    // annotation
                .WithMessage("*numeric*")                               // rationale
                .WithMessage($"*{nameof(Char)}*");                      // details
        }

        [TestMethod] public void IsNegative_PathOnAnnotationForScalar_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(CircleOfHell);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                        // source type
                .WithMessage($"*{nameof(CircleOfHell.Level)}*")         // source property
                .WithMessage("*[Check.IsNegative]*")                    // annotation
                .WithMessage("*path*does not exist*")                   // rationale
                .WithMessage("*\"---\"*");                              // details
        }

        [TestMethod] public void IsNegative_MultipleAnnotationsOnSingleProperty_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Alkene);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                            // source type
                .WithMessage($"*{nameof(Alkene.FreezingPoint)}*")           // source property
                .WithMessage("*[Check.IsNegative]*")                        // annotation
                .WithMessage("*multiple*");                                 // rationale
        }

        [TestMethod] public void IsNonZero_AppliedToNumericField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(RegularPolygon);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(RegularPolygon.InternalAngle), ComparisonOperator.NE, 0.0).And
                .NoOtherConstraints();
        }

        [TestMethod] public void IsNonZero_AppliedToNonNumericField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Brassiere);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                        // source type
                .WithMessage($"*{nameof(Brassiere.CupSize)}*")          // source property
                .WithMessage("*[Check.IsNonZero]*")                     // annotation
                .WithMessage("*numeric*")                               // rationale
                .WithMessage($"*{nameof(String)}*");                    // details
        }

        [TestMethod] public void IsNonZero_PathOnAnnotationForScalar_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Cryptocurrency);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                            // source type
                .WithMessage($"*{nameof(Cryptocurrency.ExchangeRate)}*")    // source property
                .WithMessage("*[Check.IsNonZero]*")                         // annotation
                .WithMessage("*path*does not exist*")                       // rationale
                .WithMessage("*\"---\"*");                                  // details
        }

        [TestMethod] public void IsNonZero_MultipleAnnotationsOnSingleProperty_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Shoe);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                            // source type
                .WithMessage($"*{nameof(Shoe.Mens)}*")                      // source property
                .WithMessage("*[Check.IsNonZero]*")                         // annotation
                .WithMessage("*multiple*");                                 // rationale
        }

        [TestMethod] public void Signedness_PropertyDataConvertedToNumeric() {
            // Arrange
            var translator = new Translator();
            var source = typeof(SwimmingPool);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(SwimmingPool.Classification), ComparisonOperator.GT, 0).And
                .NoOtherConstraints();
        }

        [TestMethod] public void Signedness_PropertyDataConvertedFromNumeric_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Elevator);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                        // source type
                .WithMessage($"*{nameof(Elevator.NumFloors)}*")         // source property
                .WithMessage("*[Check.IsNonZero]*")                     // annotation
                .WithMessage("*numeric*")                               // rationale
                .WithMessage($"*{nameof(String)}*");                    // details
        }

        [TestMethod] public void Signedness_PropertyBothIsPositiveAndIsNegative_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Directory);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                            // source type
                .WithMessage($"*{nameof(Directory.Mode)}*")                 // source property
                .WithMessage("*[Check.IsPositive]*[Check.IsNegative]*")     // annotation
                .WithMessage("*mutually exclusive*");                       // rationale
        }

        [TestMethod] public void Signedness_PropertyBothIsPositiveAndIsNonZero_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Peninsula);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                            // source type
                .WithMessage($"*{nameof(Peninsula.Coastline)}*")            // source property
                .WithMessage("*[Check.IsPositive]*[Check.IsNonZero]*")      // annotation
                .WithMessage("*mutually exclusive*");                       // rationale
        }

        [TestMethod] public void Signedness_PropertyBothIsNegativeAndIsNonZero_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(HTTPError);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                            // source type
                .WithMessage($"*{nameof(HTTPError.ErrorCode)}*")            // source property
                .WithMessage("*[Check.IsNegative]*[Check.IsNonZero]*")      // annotation
                .WithMessage("*mutually exclusive*");                       // rationale
        }


        [TestMethod] public void IsGreaterThan_AppliedToNumericField() {
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
                .NoOtherConstraints();
        }

        [TestMethod] public void IsGreaterThan_AppliedToTextualField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(MultipleChoiceQuestion);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(MultipleChoiceQuestion.CorrectAnswer), ComparisonOperator.GT, '@').And
                .HaveConstraint(nameof(MultipleChoiceQuestion.ChoiceA), ComparisonOperator.GT, "A. ").And
                .HaveConstraint(nameof(MultipleChoiceQuestion.ChoiceB), ComparisonOperator.GT, "A. ").And
                .HaveConstraint(nameof(MultipleChoiceQuestion.ChoiceC), ComparisonOperator.GT, "A. ").And
                .HaveConstraint(nameof(MultipleChoiceQuestion.ChoiceD), ComparisonOperator.GT, "A. ").And
                .NoOtherConstraints();
        }

        [TestMethod] public void IsGreaterThan_AppliedToDateTimeField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(GoldRush);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(GoldRush.StartDate), ComparisonOperator.GT, new DateTime(1200, 3, 18)).And
                .HaveConstraint(nameof(GoldRush.EndDate), ComparisonOperator.GT, new DateTime(1176, 11, 22)).And
                .NoOtherConstraints();
        }

        [TestMethod] public void IsGreaterThan_AppliedToNullableField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Baryon);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(Baryon.Charge), ComparisonOperator.GT, (short)-5).And
                .NoOtherConstraints();
        }

        [TestMethod] public void IsGreaterThan_AppliedToNonOrderedField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Font);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                        // source type
                .WithMessage($"*{nameof(Font.HasSerifs)}*")             // source property
                .WithMessage("*[Check.IsGreaterThan]*")                 // annotation
                .WithMessage("*ordering*")                              // rationale
                .WithMessage($"*{nameof(Boolean)}*");                   // details
        }

        [TestMethod] public void IsGreaterThan_AnchorIsNull_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(UNResolution);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                            // source type
                .WithMessage($"*{nameof(UNResolution.NumSignatories)}*")    // source property
                .WithMessage("*[Check.IsGreaterThan]*")                     // annotation
                .WithMessage("*null*");                                     // rationale
        }

        [TestMethod] public void IsGreaterThan_AnchorIsMaximum_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Upanishad);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                        // source type
                .WithMessage($"*{nameof(Upanishad.Index)}*")            // source property
                .WithMessage("*[Check.IsGreaterThan]*")                 // annotation
                .WithMessage("*maximum value*")                         // rationale
                .WithMessage($"*{sbyte.MaxValue}*{nameof(SByte)}*");    // details
        }

        [TestMethod] public void IsGreaterThan_InvalidUnconvertibleAnchor_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Racehorse);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                            // source type
                .WithMessage($"*{nameof(Racehorse.FirstDerbyWin)}*")        // source property
                .WithMessage("*[Check.IsGreaterThan]*")                     // annotation
                .WithMessage($"*property of type {nameof(UInt64)}*")        // rationale
                .WithMessage($"*true (of type {nameof(Boolean)})*");        // details
        }

        [TestMethod] public void IsGreaterThan_InvalidConvertibleAnchor_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(ChineseCharacter);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                            // source type
                .WithMessage($"*{nameof(ChineseCharacter.Character)}*")     // source property
                .WithMessage("*[Check.IsGreaterThan]*")                     // annotation
                .WithMessage($"*property of type {nameof(Char)}*")          // rationale
                .WithMessage($"*14 (of type {nameof(Byte)})*");             // details
        }

        [TestMethod] public void IsGreaterThan_OtherwiseValidSingleElementArrayDefault_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Query);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                        // source type
                .WithMessage($"*{nameof(Query.WHERE)}*")                // source property
                .WithMessage("*[Check.IsGreaterThan]*")                 // annotation
                .WithMessage("*array*");                                // rationale
        }

        [TestMethod] public void IsGreaterThan_PathOnAnnotationForScalar_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Canyon);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                            // source type
                .WithMessage($"*{nameof(Canyon.Depth)}*")                   // source property
                .WithMessage("*[Check.IsGreaterThan]*")                     // annotation
                .WithMessage("*path*does not exist*")                       // rationale
                .WithMessage("*\"---\"*");                                  // details
        }

        [TestMethod] public void IsGreaterThan_MultipleAnnotationsOnSingleProperty_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(NuclearPowerPlant);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                            // source type
                .WithMessage($"*{nameof(NuclearPowerPlant.Meltdowns)}*")    // source property
                .WithMessage("*[Check.IsGreaterThan]*")                     // annotation
                .WithMessage("*multiple*");                                 // rationale
        }

        [TestMethod] public void IsLessThan_AppliedToNumericField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Resistor);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(Resistor.Resistance), ComparisonOperator.LT, 27814L).And
                .NoOtherConstraints();
        }

        [TestMethod] public void IsLessThan_AppliedToTextualField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Senator);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(Senator.LastName), ComparisonOperator.LT, "...").And
                .NoOtherConstraints();
        }

        [TestMethod] public void IsLessThan_AppliedToDateTimeField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Commercial);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(Commercial.TimeSlot), ComparisonOperator.LT, new DateTime(2300, 1, 1)).And
                .NoOtherConstraints();
        }

        [TestMethod] public void IsLessThan_AppliedToNullableField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(AutoRacetrack);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(AutoRacetrack.TrackLength), ComparisonOperator.LT, 12000000L).And
                .NoOtherConstraints();
        }

        [TestMethod] public void IsLessThan_AppliedToNonOrderedField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(DLL);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                        // source type
                .WithMessage($"*{nameof(DLL.ID)}*")                     // source property
                .WithMessage("*[Check.IsLessThan]*")                    // annotation
                .WithMessage("*ordering*")                              // rationale
                .WithMessage($"*{nameof(Guid)}*");                      // details
        }

        [TestMethod] public void IsLessThan_AnchorIsNull_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Animation);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                    // source type
                .WithMessage($"*{nameof(Animation.Duration)}*")     // source property
                .WithMessage("*[Check.IsLessThan]*")                // annotation
                .WithMessage("*null*");                             // rationale
        }

        [TestMethod] public void IsLessThan_AnchorIsMinimum_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(StrategoPiece);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                        // source type
                .WithMessage($"*{nameof(StrategoPiece.Value)}*")        // source property
                .WithMessage("*[Check.IsLessThan]*")                    // annotation
                .WithMessage("*minimum value*")                         // rationale
                .WithMessage($"*{uint.MinValue}*{nameof(UInt32)}*");    // details
        }

        [TestMethod] public void IsLessThan_InvalidUnconvertibleAnchor_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Distribution);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                            // source type
                .WithMessage($"*{nameof(Distribution.Mode)}*")              // source property
                .WithMessage("*[Check.IsLessThan]*")                        // annotation
                .WithMessage($"*property of type {nameof(Double)}*")        // rationale
                .WithMessage($"*\"Zero\" (of type {nameof(String)})*");     // details
        }

        [TestMethod] public void IsLessThan_InvalidConvertibleAnchor_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(WebBrowser);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                            // source type
                .WithMessage($"*{nameof(WebBrowser.MarketShare)}*")         // source property
                .WithMessage("*[Check.IsLessThan]*")                        // annotation
                .WithMessage($"*property of type {nameof(Single)}*")        // rationale
                .WithMessage($"*100 (of type {nameof(Int32)})*");           // details
        }

        [TestMethod] public void IsLessThan_OtherwiseValidSingleElementArrayDefault_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(GrammaticalCase);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                        // source type
                .WithMessage($"*{nameof(GrammaticalCase.Affix)}*")      // source property
                .WithMessage("*[Check.IsLessThan]*")                    // annotation
                .WithMessage("*array*");                                // rationale
        }

        [TestMethod] public void IsLessThan_PathOnAnnotationForScalar_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Potato);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                            // source type
                .WithMessage($"*{nameof(Potato.Weight)}*")                  // source property
                .WithMessage("*[Check.IsLessThan]*")                        // annotation
                .WithMessage("*path*does not exist*")                       // rationale
                .WithMessage("*\"---\"*");                                  // details
        }

        [TestMethod] public void IsLessThan_MultipleAnnotationsOnSingleProperty_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(CinemaSins);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                            // source type
                .WithMessage($"*{nameof(CinemaSins.SinCount)}*")            // source property
                .WithMessage("*[Check.IsLessThan]*")                        // annotation
                .WithMessage("*multiple*");                                 // rationale
        }

        [TestMethod] public void IsGreaterOrEqualTo_AppliedToNumericField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Geyser);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(Geyser.EruptionHeight), ComparisonOperator.GTE, 0L).And
                .HaveConstraint(nameof(Geyser.Elevation), ComparisonOperator.GTE, 0f).And
                .HaveConstraint(nameof(Geyser.EruptionDuration), ComparisonOperator.GTE, 0).And
                .NoOtherConstraints();
        }

        [TestMethod] public void IsGreaterOrEqualTo_AppliedToTextualField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Hotel);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(Hotel.HotelName), ComparisonOperator.GTE, "").And
                .HaveConstraint(nameof(Hotel.Stars), ComparisonOperator.GTE, '1').And
                .NoOtherConstraints();
        }

        [TestMethod] public void IsGreaterOrEqualTo_AppliedToDateTimeField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(ETF);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(ETF.FirstPosted), ComparisonOperator.GTE, new DateTime(1377, 6, 19)).And
                .NoOtherConstraints();
        }

        [TestMethod] public void IsGreaterOrEqualTo_AppliedToNullableField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Muscle);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(Muscle.Nerve), ComparisonOperator.GTE, "~~~").And
                .NoOtherConstraints();
        }

        [TestMethod] public void IsGreaterOrEqualTo_AppliedToNonOrderedField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Steak);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                        // source type
                .WithMessage($"*{nameof(Steak.FromSteakhouse)}*")       // source property
                .WithMessage("*[Check.IsGreaterOrEqualTo]*")            // annotation
                .WithMessage("*ordering*")                              // rationale
                .WithMessage($"*{nameof(Boolean)}*");                   // details
        }

        [TestMethod] public void IsGreaterOrEqualTo_AnchorIsNull_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Neurotoxin);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                    // source type
                .WithMessage($"*{nameof(Neurotoxin.MolarMass)}*")   // source property
                .WithMessage("*[Check.IsGreaterOrEqualTo]*")        // annotation
                .WithMessage("*null*");                             // rationale
        }

        [TestMethod] public void IsGreaterOrEqualTo_AnchorIsMinimum_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Bacterium);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                        // source type
                .WithMessage($"*{nameof(Bacterium.NumStrains)}*")       // source property
                .WithMessage("*[Check.IsGreaterOrEqualTo]*")            // annotation
                .WithMessage("*minimum value*")                         // rationale
                .WithMessage($"*{ushort.MinValue}*{nameof(UInt16)}*");  // details
        }

        [TestMethod] public void IsGreaterOrEqualTo_InvalidUnconvertibleAnchor_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(LandCard);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                            // source type
                .WithMessage($"*{nameof(LandCard.BlueManna)}*")             // source property
                .WithMessage("*[Check.IsGreaterOrEqualTo]*")                // annotation
                .WithMessage($"*property of type {nameof(Byte)}*")          // rationale
                .WithMessage($"*\"None\" (of type {nameof(String)})*");     // details
        }

        [TestMethod] public void IsGreaterOrEqualTo_InvalidConvertibleAnchor_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Keystroke);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                            // source type
                .WithMessage($"*{nameof(Keystroke.ResultingGlyph)}*")       // source property
                .WithMessage("*[Check.IsGreaterOrEqualTo]*")                // annotation
                .WithMessage($"*property of type {nameof(Char)}*")          // rationale
                .WithMessage($"*290 (of type {nameof(Int32)})*");           // details
        }

        [TestMethod] public void IsGreaterOrEqualTo_OtherwiseValidSingleElementArrayDefault_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Zoo);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                        // source type
                .WithMessage($"*{nameof(Zoo.AverageVisitorsPerDay)}*")  // source property
                .WithMessage("*[Check.IsGreaterOrEqualTo]*")            // annotation
                .WithMessage("*array*");                                // rationale
        }

        [TestMethod] public void IsGreaterOrEqualTo_PathOnAnnotationForScalar_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Hieroglyph);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                            // source type
                .WithMessage($"*{nameof(Hieroglyph.Glyph)}*")               // source property
                .WithMessage("*[Check.IsGreaterOrEqualTo]*")                // annotation
                .WithMessage("*path*does not exist*")                       // rationale
                .WithMessage("*\"---\"*");                                  // details
        }

        [TestMethod] public void IsGreaterOrEqualTo_MultipleAnnotationsOnSingleProperty_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(SolarEclipse);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                            // source type
                .WithMessage($"*{nameof(SolarEclipse.SarosCycle)}*")        // source property
                .WithMessage("*[Check.IsGreaterOrEqualTo]*")                // annotation
                .WithMessage("*multiple*");                                 // rationale
        }

        [TestMethod] public void IsLessOrEqualTo_AppliedToNumericField() {
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
                .NoOtherConstraints();
        }

        [TestMethod] public void IsLessOrEqualTo_AppliedToTextualField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(ExcelRange);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(ExcelRange.StartColumn), ComparisonOperator.LTE, "XFD").And
                .HaveConstraint(nameof(ExcelRange.EndColumn), ComparisonOperator.LTE, "XFD").And
                .NoOtherConstraints();
        }

        [TestMethod] public void IsLessOrEqualTo_AppliedToDateTimeField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Representative);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(Representative.FirstElected), ComparisonOperator.LTE, new DateTime(2688, 12, 2)).And
                .NoOtherConstraints();
        }

        [TestMethod] public void IsLessOrEqualTo_AppliedToNullableField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Subreddit);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(Subreddit.Moderator), ComparisonOperator.LTE, "???").And
                .NoOtherConstraints();
        }

        [TestMethod] public void IsLessOrEqualTo_AppliedToNonOrderedField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Sunscreen);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                        // source type
                .WithMessage($"*{nameof(Sunscreen.ID)}*")               // source property
                .WithMessage("*[Check.IsLessOrEqualTo]*")               // annotation
                .WithMessage("*ordering*")                              // rationale
                .WithMessage($"*{nameof(Guid)}*");                      // details
        }

        [TestMethod] public void IsLessOrEqualTo_AnchorIsNull_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(VoirDire);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                            // source type
                .WithMessage($"*{nameof(VoirDire.BatsonChallenges)}*")      // source property
                .WithMessage("*[Check.IsLessOrEqualTo]*")                   // annotation
                .WithMessage("*null*");                                     // rationale
        }

        [TestMethod] public void IsLessOrEqualTo_AnchorIsMaximum_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(ShellCommand);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                        // source type
                .WithMessage($"*{nameof(ShellCommand.NumOptions)}*")    // source property
                .WithMessage("*[Check.IsLessOrEqualTo]*")               // annotation
                .WithMessage("*maximum value*")                         // rationale
                .WithMessage($"*{long.MaxValue}*{nameof(Int64)}*");     // details
        }

        [TestMethod] public void IsLessOrEqualTo_InvalidUnconvertibleAnchor_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Dreidel);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                            // source type
                .WithMessage($"*{nameof(Dreidel.SerialCode)}*")             // source property
                .WithMessage("*[Check.IsLessOrEqualTo]*")                   // annotation
                .WithMessage($"*property of type {nameof(String)}*")        // rationale
                .WithMessage($"*153 (of type {nameof(Byte)})*");            // details
        }

        [TestMethod] public void IsLessOrEqualTo_InvalidConvertibleAnchor_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(ArthurianKnight);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                                // source type
                .WithMessage($"*{nameof(ArthurianKnight.MalloryMentions)}*")    // source property
                .WithMessage("*[Check.IsLessOrEqualTo]*")                       // annotation
                .WithMessage($"*property of type {nameof(UInt64)}*")            // rationale
                .WithMessage($"*4 (of type {nameof(UInt32)})*");                // details
        }

        [TestMethod] public void IsLessOrEqualTo_OtherwiseValidSingleElementArrayDefault_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Mint);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                        // source type
                .WithMessage($"*{nameof(Mint.Established)}*")           // source property
                .WithMessage("*[Check.IsLessOrEqualTo]*")               // annotation
                .WithMessage("*array*");                                // rationale
        }

        [TestMethod] public void IsLessOrEqualTo_PathOnAnnotationForScalar_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(PlaneOfExistence);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                            // source type
                .WithMessage($"*{nameof(PlaneOfExistence.Name)}*")          // source property
                .WithMessage("*[Check.IsLessOrEqualTo]*")                   // annotation
                .WithMessage("*path*does not exist*")                       // rationale
                .WithMessage("*\"---\"*");                                  // details
        }

        [TestMethod] public void IsLessOrEqualTo_MultipleAnnotationsOnSingleProperty_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Archbishop);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                            // source type
                .WithMessage($"*{nameof(Archbishop.City)}*")                // source property
                .WithMessage("*[Check.IsLessOrEqualTo]*")                   // annotation
                .WithMessage("*multiple*");                                 // rationale
        }

        [TestMethod] public void IsNot_AppliedToNumericField() {
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
                .NoOtherConstraints();
        }

        [TestMethod] public void IsNot_AppliedToTextualField() {
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
                .NoOtherConstraints();
        }

        [TestMethod] public void IsNot_AppliedToDateTimeField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(SlotMachine);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(SlotMachine.InstalledOn), ComparisonOperator.NE, new DateTime(4431, 1, 21)).And
                .NoOtherConstraints();
        }

        [TestMethod] public void IsNot_AppliedToBooleanField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(PoliceOfficer);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(PoliceOfficer.IsRetired), ComparisonOperator.NE, false).And
                .NoOtherConstraints();
        }

        [TestMethod] public void IsNot_AppliedToGuidField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Church);
            var guid = new Guid("a3c3ac24-4cf2-428e-a4db-76b30958cc90");

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(Church.ChurchID), ComparisonOperator.NE, guid).And
                .NoOtherConstraints();
        }

        [TestMethod] public void IsNot_AppliedToNullableField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Fountain);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(Fountain.Spout), ComparisonOperator.NE, 35.22).And
                .NoOtherConstraints();
        }

        [TestMethod] public void IsNot_AnchorIsNull_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(SecurityBug);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                            // source type
                .WithMessage($"*{nameof(SecurityBug.LibraryAffected)}*")    // source property
                .WithMessage("*[Check.IsNot]*")                             // annotation
                .WithMessage("*null*");                                     // rationale
        }

        [TestMethod] public void IsNot_InvalidUnconvertibleAnchor_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Candle);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                            // source type
                .WithMessage($"*{nameof(Candle.Width)}*")                   // source property
                .WithMessage("*[Check.IsNot]*")                             // annotation
                .WithMessage($"*property of type {nameof(Single)}*")        // rationale
                .WithMessage($"*\"Wide\" (of type {nameof(String)})*");     // details
        }

        [TestMethod] public void IsNot_InvalidConvertibleAnchor_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(CompilerWarning);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                                // source type
                .WithMessage($"*{nameof(CompilerWarning.DebugOnly)}*")          // source property
                .WithMessage("*[Check.IsNot]*")                                 // annotation
                .WithMessage($"*property of type {nameof(Boolean)}*")           // rationale
                .WithMessage($"*1 (of type {nameof(Int32)})*");                 // details
        }

        [TestMethod] public void IsNot_OtherwiseValidSingleElementArrayDefault_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Alarm);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                        // source type
                .WithMessage($"*{nameof(Alarm.Snoozeable)}*")           // source property
                .WithMessage("*[Check.IsNot]*")                         // annotation
                .WithMessage("*array*");                                // rationale
        }

        [TestMethod] public void IsNot_PathOnAnnotationForScalar_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Prison);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                            // source type
                .WithMessage($"*{nameof(Prison.SecurityLevel)}*")           // source property
                .WithMessage("*[Check.IsNot]*")                             // annotation
                .WithMessage("*path*does not exist*")                       // rationale
                .WithMessage("*\"---\"*");                                  // details
        }

        [TestMethod] public void IsNot_MultipleAnnotationsOnSingleProperty_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Pterosaur);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                            // source type
                .WithMessage($"*{nameof(Pterosaur.Specimens)}*")            // source property
                .WithMessage("*[Check.IsNot]*")                             // annotation
                .WithMessage("*multiple*");                                 // rationale
        }

        [TestMethod] public void RangeComparison_InvalidlyFormattedDateTimeAnchor_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Lease);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                            // source type
                .WithMessage($"*{nameof(Lease.StartDate)}*")                // source property
                .WithMessage("*[Check.IsGreaterOrEqualTo]*")                // annotation
                .WithMessage($"*could not parse*into {nameof(DateTime)}*")  // rationale
                .WithMessage("*\"1637+04+18\"*");                           // details
        }

        [TestMethod] public void RangeComparison_InvalidRangeDateTimeAnchor_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(LotteryTicket);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                            // source type
                .WithMessage($"*{nameof(LotteryTicket.PurchaseTime)}*")     // source property
                .WithMessage("*[Check.IsNot]*")                             // annotation
                .WithMessage($"*could not parse*into {nameof(DateTime)}*")  // rationale
                .WithMessage("*\"2023-02-29\"*");                           // details
        }

        [TestMethod] public void RangeComparison_NonStringDateTimeAnchor_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(ACT);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                            // source type
                .WithMessage($"*{nameof(ACT.When)}*")                       // source property
                .WithMessage("*[Check.IsLessThan]*")                        // annotation
                .WithMessage($"*property of type {nameof(DateTime)}*")      // rationale
                .WithMessage($"*100 (of type {nameof(Int32)})*")            // details
                .WithMessage("*a string is required*");                     // explanation
        }

        [TestMethod] public void RangeComparison_AnchorValidForDataConvertedTarget() {
            // Arrange
            var translator = new Translator();
            var source = typeof(RingOfPower);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(RingOfPower.Destroyed), ComparisonOperator.GTE, 7).And
                .NoOtherConstraints();
        }

        [TestMethod] public void RangedComparison_AnchorValidForDataConvertedSource_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Genie);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                            // source type
                .WithMessage($"*{nameof(Genie.NumWishes)}*")                // source property
                .WithMessage("*[Check.IsLessThan]*")                        // annotation
                .WithMessage($"*property of type {nameof(String)}*")        // rationale
                .WithMessage($"*144 (of type {nameof(Int32)})*");           // details
        }


        [TestMethod] public void IsNonEmpty_AppliedToStringField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Chocolate);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(FieldFunction.LengthOf, nameof(Chocolate.Name), ComparisonOperator.GT, 0).And
                .NoOtherConstraints();
        }

        [TestMethod] public void IsNonEmpty_AppliedToNullableStringField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Scholarship);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(FieldFunction.LengthOf, nameof(Scholarship.Organization), ComparisonOperator.GT, 0).And
                .HaveConstraint(FieldFunction.LengthOf, nameof(Scholarship.TargetSchool), ComparisonOperator.GT, 0).And
                .NoOtherConstraints();
        }

        [TestMethod] public void IsNonEmpty_AppliedToNonStringField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(MovieTicket);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                            // source type
                .WithMessage($"*{nameof(MovieTicket.Row)}*")                // source property
                .WithMessage("*[Check.IsNonEmpty]*")                        // annotation
                .WithMessage($"*Field of type {nameof(String)}*")           // rationale
                .WithMessage($"*Field of type {nameof(Char)}*");            // details
        }

        [TestMethod] public void IsNonEmpty_PathOnAnnotationForScalar_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(ASLSign);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                            // source type
                .WithMessage($"*{nameof(ASLSign.Gloss)}*")                  // source property
                .WithMessage("*[Check.IsNonEmpty]*")                        // annotation
                .WithMessage("*path*does not exist*")                       // rationale
                .WithMessage("*\"---\"*");                                  // details
        }

        [TestMethod] public void IsNonEmpty_MultipleAnnotationsOnSingleProperty_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Top10List);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                            // source type
                .WithMessage($"*{nameof(Top10List.Number9)}*")              // source property
                .WithMessage("*[Check.IsNonEmpty]*")                        // annotation
                .WithMessage("*multiple*");                                 // rationale
        }

        [TestMethod] public void LengthIsAtLeast_AppliedToStringField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(NFLPenalty);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(FieldFunction.LengthOf, nameof(NFLPenalty.Penalty), ComparisonOperator.GTE, 1).And
                .NoOtherConstraints();
        }

        [TestMethod] public void LengthIsAtLeast_AppliedToNullableStringField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Ben10Alien);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(FieldFunction.LengthOf, nameof(Ben10Alien.AlternateName), ComparisonOperator.GTE, 7).And
                .NoOtherConstraints();
        }

        [TestMethod] public void LengthIsAtLeast_NegativeAnchor_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(LaborOfHeracles);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                            // source type
                .WithMessage($"*{nameof(LaborOfHeracles.Target)}*")         // source property
                .WithMessage("*[Check.LengthIsAtLeast]*")                   // annotation
                .WithMessage("*minimum*length*at least 1*")                 // rationale
                .WithMessage("*-144*");                                     // details
        }

        [TestMethod] public void LengthIsAtLeast_ZeroAnchor_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(HolyRomanEmperor);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                            // source type
                .WithMessage($"*{nameof(HolyRomanEmperor.Name)}*")          // source property
                .WithMessage("*[Check.LengthIsAtLeast]*")                   // annotation
                .WithMessage("*minimum*length*at least 1*")                 // rationale
                .WithMessage("*0*");                                        // details
        }

        [TestMethod] public void LengthIsAtLeast_AppliedToNonStringField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(HashFunction);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                            // source type
                .WithMessage($"*{nameof(HashFunction.BlockSize)}*")         // source property
                .WithMessage("*[Check.LengthIsAtLeast]*")                   // annotation
                .WithMessage($"*Field of type {nameof(String)}*")           // rationale
                .WithMessage($"*Field of type {nameof(UInt16)}*");          // details
        }

        [TestMethod] public void LengthIsAtLeast_PathOnAnnotationForScalar_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Histogram);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                            // source type
                .WithMessage($"*{nameof(Histogram.BucketUnit)}*")           // source property
                .WithMessage("*[Check.LengthIsAtLeast]*")                   // annotation
                .WithMessage("*path*does not exist*")                       // rationale
                .WithMessage("*\"---\"*");                                  // details
        }

        [TestMethod] public void LengthIsAtLeast_MultipleAnnotationsOnSingleProperty_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Bagel);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                            // source type
                .WithMessage($"*{nameof(Bagel.Flavor)}*")                   // source property
                .WithMessage("*[Check.LengthIsAtLeast]*")                   // annotation
                .WithMessage("*multiple*");                                 // rationale
        }

        [TestMethod] public void LengthIsAtMost_AppliedToStringField() {
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
                .NoOtherConstraints();
        }

        [TestMethod] public void LengthIsAtMost_AppliedToNullableStringField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(WinterStorm);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(FieldFunction.LengthOf, nameof(WinterStorm.Name), ComparisonOperator.LTE, 300).And
                .NoOtherConstraints();
        }

        [TestMethod] public void LengthIsAtMost_NegativeAnchor_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Fraternity);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                            // source type
                .WithMessage($"*{nameof(Fraternity.Name)}*")                // source property
                .WithMessage("*[Check.LengthIsAtMost]*")                    // annotation
                .WithMessage("*maximum*length*at least 0*")                 // rationale
                .WithMessage("*-7*");                                       // details
        }

        [TestMethod] public void LengthIsAtMost_ZeroAnchor() {
            // Arrange
            var translator = new Translator();
            var source = typeof(KnockKnockJoke);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(FieldFunction.LengthOf, nameof(KnockKnockJoke.SetUp), ComparisonOperator.EQ, 0).And
                .HaveConstraint(FieldFunction.LengthOf, nameof(KnockKnockJoke.PunchLine), ComparisonOperator.EQ, 0).And
                .NoOtherConstraints();
        }

        [TestMethod] public void LengthIsAtMost_AppliedToNonStringField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Diamond);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                            // source type
                .WithMessage($"*{nameof(Diamond.IsBloodDiamond)}*")         // source property
                .WithMessage("*[Check.LengthIsAtMost]*")                    // annotation
                .WithMessage($"*Field of type {nameof(String)}*")           // rationale
                .WithMessage($"*Field of type {nameof(Boolean)}*");         // details
        }

        [TestMethod] public void LengthIsAtMost_PathOnAnnotationForScalar_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Nebula);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                            // source type
                .WithMessage($"*{nameof(Nebula.Name)}*")                    // source property
                .WithMessage("*[Check.LengthIsAtMost]*")                    // annotation
                .WithMessage("*path*does not exist*")                       // rationale
                .WithMessage("*\"---\"*");                                  // details
        }

        [TestMethod] public void LengthIsAtMost_MultipleAnnotationsOnSingleProperty_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(OceanicTrench);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                            // source type
                .WithMessage($"*{nameof(OceanicTrench.Location)}*")         // source property
                .WithMessage("*[Check.LengthIsAtMost]*")                    // annotation
                .WithMessage("*multiple*");                                 // rationale
        }

        [TestMethod] public void LengthIsBetween_AppliedToStringField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Sorority);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(FieldFunction.LengthOf, nameof(Sorority.Motto), ComparisonOperator.GTE, 4).And
                .HaveConstraint(FieldFunction.LengthOf, nameof(Sorority.Motto), ComparisonOperator.LTE, 1713).And
                .NoOtherConstraints();
        }

        [TestMethod] public void LengthIsBetween_AppliedToNullableStringField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Telescope);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(FieldFunction.LengthOf, nameof(Telescope.Name), ComparisonOperator.GTE, 1).And
                .HaveConstraint(FieldFunction.LengthOf, nameof(Telescope.Name), ComparisonOperator.LTE, int.MaxValue).And
                .NoOtherConstraints();
        }

        [TestMethod] public void LengthIsBetween_MinimumEqualsMaxium() {
            // Arrange
            var translator = new Translator();
            var source = typeof(DNACodon);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(FieldFunction.LengthOf, nameof(DNACodon.CodonSequence), ComparisonOperator.EQ, 3).And
                .NoOtherConstraints();
        }

        [TestMethod] public void LengthIsBetween_NegativeMinimum_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(ShenGongWu);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                            // source type
                .WithMessage($"*{nameof(ShenGongWu.InitialEpisode)}*")      // source property
                .WithMessage("*[Check.LengthIsBetween]*")                   // annotation
                .WithMessage("*minimum*length*at least 1*")                 // rationale
                .WithMessage("*-4*");                                       // details
        }

        [TestMethod] public void LengthIsBetween_ZeroMinimum_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(MilitaryBase);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                            // source type
                .WithMessage($"*{nameof(MilitaryBase.Commander)}*")         // source property
                .WithMessage("*[Check.LengthIsBetween]*")                   // annotation
                .WithMessage("*minimum*length*at least 1*")                 // rationale
                .WithMessage("*0*");                                        // details
        }

        [TestMethod] public void LengthIsBetween_MinimumLargerThanMaximum_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(ChristmasCarol);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                                // source type
                .WithMessage($"*{nameof(ChristmasCarol.FirstVerse)}*")          // source property
                .WithMessage("*[Check.LengthIsBetween]*")                       // annotation
                .WithMessage("*maximum*length*less than*minimum*length*")       // rationale
                .WithMessage("*1553*28841*");                                   // details
        }

        [TestMethod] public void LengthIsBetween_AppliedToNonStringField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Capacitor);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                            // source type
                .WithMessage($"*{nameof(Capacitor.Capacitance)}*")          // source property
                .WithMessage("*[Check.LengthIsBetween]*")                   // annotation
                .WithMessage($"*Field of type {nameof(String)}*")           // rationale
                .WithMessage($"*Field of type {nameof(Single)}*");          // details
        }

        [TestMethod] public void LengthIsBetween_PathOnAnnotationForScalar_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(SetCard);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                            // source type
                .WithMessage($"*{nameof(SetCard.Pattern)}*")                // source property
                .WithMessage("*[Check.LengthIsBetween]*")                   // annotation
                .WithMessage("*path*does not exist*")                       // rationale
                .WithMessage("*\"---\"*");                                  // details
        }

        [TestMethod] public void LengthIsBetween_MultipleAnnotationsOnSingleProperty_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Aria);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                            // source type
                .WithMessage($"*{nameof(Aria.Lyrics)}*")                    // source property
                .WithMessage("*[Check.LengthIsBetween]*")                   // annotation
                .WithMessage("*multiple*");                                 // rationale
        }

        [TestMethod] public void StringLength_PropertyDataConvertedToString() {
            // Arrange
            var translator = new Translator();
            var source = typeof(AesSedai);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(FieldFunction.LengthOf, nameof(AesSedai.Ajah), ComparisonOperator.GTE, 1).And
                .HaveConstraint(FieldFunction.LengthOf, nameof(AesSedai.Ajah), ComparisonOperator.LTE, 15).And
                .NoOtherConstraints();
        }

        [TestMethod] public void StringLength_PropertyDataConvertedFromString_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Campfire);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                            // source type
                .WithMessage($"*{nameof(Campfire.WoodType)}*")              // source property
                .WithMessage("*[Check.LengthIsAtLeast]*")                   // annotation
                .WithMessage($"*Field of type {nameof(String)}*")           // rationale
                .WithMessage($"*Field of type {nameof(Byte)}*");            // details
        }

        [TestMethod] public void StringLength_PropertyBothLengthIsAtLeastAndIsNonEmpty_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(StepPyramid);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                                // source type
                .WithMessage($"*{nameof(StepPyramid.KnownAs)}*")                // source property
                .WithMessage("*[Check.LengthIsAtLeast]*[Check.IsNonEmpty]*")    // annotation
                .WithMessage("*mutually exclusive*");                           // rationale
        }

        [TestMethod] public void StringLength_PropertyBothLengthIsBetweenAndIsNonEmpty_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Cave);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                                // source type
                .WithMessage($"*{nameof(Cave.Name)}*")                          // source property
                .WithMessage("*[Check.LengthIsBetween]*[Check.IsNonEmpty]*")    // annotation
                .WithMessage("*mutually exclusive*");                           // rationale
        }

        [TestMethod] public void StringLength_PropertyBothLengthIsAtLeastAndLengthIsBetween_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Integral);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                                    // source type
                .WithMessage($"*{nameof(Integral.Expression)}*")                    // source property
                .WithMessage("*[Check.LengthIsAtLeast]*[Check.LengthIsBetween]*")   // annotation
                .WithMessage("*mutually exclusive*");                               // rationale
        }

        [TestMethod] public void StringLength_PropertyBothLengthIsAtMostAndLengthIsBetween_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Isthmus);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                                    // source type
                .WithMessage($"*{nameof(Isthmus.Name)}*")                           // source property
                .WithMessage("*[Check.LengthIsAtMost]*[Check.LengthIsBetween]*")    // annotation
                .WithMessage("*mutually exclusive*");                               // rationale
        }


        [TestMethod] public void IsOneOf_MultipleOptions() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Astronaut);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(
                    nameof(Astronaut.NumSpacewalks),
                    InclusionOperator.In,
                    new object[] { 0u, 1u, 2u, 3u, 4u, 5u }).And
                .HaveConstraint(
                    nameof(Astronaut.MaidenProgram),
                    InclusionOperator.In,
                    new object[] { "Apollo", "Gemini", "Mercury", "Artemis" }).And
                .HaveConstraint(
                    nameof(Astronaut.WalkedOnMoon),
                    InclusionOperator.In,
                    new object[] { true, false }).And
                .NoOtherConstraints();
        }

        [TestMethod] public void IsOneOf_SingleOption() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Hospital);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(Hospital.Opened), ComparisonOperator.EQ, new DateTime(2000, 1, 1)).And
                .NoOtherConstraints();
        }

        [TestMethod] public void IsOneOf_DuplicatedOption() {
            // Arrange
            var translator = new Translator();
            var source = typeof(HealingPotion);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(
                    nameof(HealingPotion.DieType),
                    InclusionOperator.In,
                    new object[] { 4u, 8u, 10u, 12u, 20u, 100u }).And
                .NoOtherConstraints();
        }

        [TestMethod] public void IsOneOf_AllowNullOnNullableField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Prophet);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                                // source type
                .WithMessage($"*{nameof(Prophet.HebrewName)}*")                 // source property
                .WithMessage("*[Check.IsOneOf]*")                               // annotation
                .WithMessage("*null*");                                         // rationale
        }

        [TestMethod] public void IsOneOf_AllowNullOnNonNullableField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(CoralReef);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                                // source type
                .WithMessage($"*{nameof(CoralReef.Longitude)}*")                // source property
                .WithMessage("*[Check.IsOneOf]*")                               // annotation
                .WithMessage("*null*");                                         // rationale
        }

        [TestMethod] public void IsOneOf_InvalidAndUnconvertibleOption_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Battery);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                            // source type
                .WithMessage($"*{nameof(Battery.Voltage)}*")                // source property
                .WithMessage("*[Check.IsOneOf]*")                           // annotation
                .WithMessage($"*property of type {nameof(Int32)}*")         // rationale
                .WithMessage($"*\"six\" (of type {nameof(String)})*");      // details
        }

        [TestMethod] public void IsOneOf_InvalidButConvertibleOption_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(TennisMatch);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                            // source type
                .WithMessage($"*{nameof(TennisMatch.Player1Score)}*")       // source property
                .WithMessage("*[Check.IsOneOf]*")                           // annotation
                .WithMessage($"*property of type {nameof(SByte)}*")         // rationale
                .WithMessage($"*0 (of type {nameof(Byte)})*");              // details
        }

        [TestMethod] public void IsOneOf_OtherwiseValidSingleElementArrayDefault_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Flashcard);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                        // source type
                .WithMessage($"*{nameof(Flashcard.IsLearned)}*")        // source property
                .WithMessage("*[Check.IsOneOf]*")                       // annotation
                .WithMessage("*array*");                                // rationale
        }

        [TestMethod] public void IsOneOf_PathOnAnnotationForScalar_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(HomericHymn);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                            // source type
                .WithMessage($"*{nameof(HomericHymn.Lines)}*")              // source property
                .WithMessage("*[Check.IsOneOf]*")                           // annotation
                .WithMessage("*path*does not exist*")                       // rationale
                .WithMessage("*\"---\"*");                                  // details
        }

        [TestMethod] public void IsOneOf_MultipleAnnotationsOnSingleProperty_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Cannon);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                            // source type
                .WithMessage($"*{nameof(Cannon.Capacity)}*")                // source property
                .WithMessage("*[Check.IsOneOf]*")                           // annotation
                .WithMessage("*multiple*");                                 // rationale
        }

        [TestMethod] public void IsNotOneOf_MultipleOptions() {
            // Arrange
            var translator = new Translator();
            var source = typeof(NationalAnthem);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(
                    nameof(NationalAnthem.Length),
                    InclusionOperator.NotIn,
                    new object[] { 1.3f, 1.6f, 1.9f, 2.2f, 2.5f, 2.8f, 3.1f, 3.4f }).And
                .NoOtherConstraints();
        }

        [TestMethod] public void IsNotOneOf_SingleOption() {
            // Arrange
            var translator = new Translator();
            var source = typeof(GamingConsole);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(nameof(GamingConsole.Name), ComparisonOperator.NE, "Atari 2600").And
                .HaveConstraint(nameof(GamingConsole.Launched), ComparisonOperator.NE, new DateTime(1973, 4, 30)).And
                .NoOtherConstraints();
        }

        [TestMethod] public void IsNotOneOf_DuplicatedOption() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Tweet);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(
                    nameof(Tweet.Grading),
                    InclusionOperator.NotIn,
                    new object[] { 'A', 'E', 'I', 'O', 'U' }).And
                .NoOtherConstraints();
        }

        [TestMethod] public void IsNotOneOf_DisallowNullOnNullableField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Ballet);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                                // source type
                .WithMessage($"*{nameof(Ballet.OpusNumber)}*")                  // source property
                .WithMessage("*[Check.IsNotOneOf]*")                            // annotation
                .WithMessage("*null*");                                         // rationale
        }

        [TestMethod] public void IsNotOneOf_DisallowNullOnNonNullableField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(PIERoot);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                                // source type
                .WithMessage($"*{nameof(PIERoot.Root)}*")                       // source property
                .WithMessage("*[Check.IsNotOneOf]*")                            // annotation
                .WithMessage("*null*");                                         // rationale
        }

        [TestMethod] public void IsNotOneOf_InvalidAndUnconvertibleOption_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Cancer);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                            // source type
                .WithMessage($"*{nameof(Cancer.RegionAffected)}*")          // source property
                .WithMessage("*[Check.IsNotOneOf]*")                        // annotation
                .WithMessage($"*property of type {nameof(String)}*")        // rationale
                .WithMessage($"*17.3 (of type {nameof(Single)})*");         // details
        }

        [TestMethod] public void IsNotOneOf_InvalidButConvertibleOption_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Avatar);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                            // source type
                .WithMessage($"*{nameof(Avatar.DebutEpisode)}*")            // source property
                .WithMessage("*[Check.IsNotOneOf]*")                        // annotation
                .WithMessage($"*property of type {nameof(UInt16)}*")        // rationale
                .WithMessage($"*8 (of type {nameof(Byte)})*");              // details
        }

        [TestMethod] public void IsNotOneOf_OtherwiseValidSingleElementArrayDefault_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Wristwatch);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                        // source type
                .WithMessage($"*{nameof(Wristwatch.Brand)}*")           // source property
                .WithMessage("*[Check.IsNotOneOf]*")                    // annotation
                .WithMessage("*array*");                                // rationale
        }

        [TestMethod] public void IsNotOneOf_MultipleAnnotationsOnSingleProperty_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Eurovision);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                            // source type
                .WithMessage($"*{nameof(Eurovision.Year)}*")                // source property
                .WithMessage("*[Check.IsNotOneOf]*")                        // annotation
                .WithMessage("*multiple*");                                 // rationale
        }

        [TestMethod] public void IsNotOneOf_PathOnAnnotationForScalar_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Donut);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                            // source type
                .WithMessage($"*{nameof(Donut.Flavor)}*")                   // source property
                .WithMessage("*[Check.IsNotOneOf]*")                        // annotation
                .WithMessage("*path*does not exist*")                       // rationale
                .WithMessage("*\"---\"*");                                  // details
        }

        [TestMethod] public void InclusionComparison_InvalidlyFormattedDateTimeAnchor_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Inator);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                            // source type
                .WithMessage($"*{nameof(Inator.Debut)}*")                   // source property
                .WithMessage("*[Check.IsNotOneOf]*")                        // annotation
                .WithMessage($"*could not parse*into {nameof(DateTime)}*")  // rationale
                .WithMessage("*\"1875~06~22\"*");                           // details
        }

        [TestMethod] public void InclusionComparison_InvalidRangeDateTimeAnchor_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(FinalJeopardy);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                            // source type
                .WithMessage($"*{nameof(FinalJeopardy.AirDate)}*")          // source property
                .WithMessage("*[Check.IsOneOf]*")                           // annotation
                .WithMessage($"*could not parse*into {nameof(DateTime)}*")  // rationale
                .WithMessage("*\"1299-08-45\"*");                           // details
        }

        [TestMethod] public void InclusionComparison_NonStringDateTimeAnchor_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Mayor);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                            // source type
                .WithMessage($"*{nameof(Mayor.TermEnd)}*")                  // source property
                .WithMessage("*[Check.IsNotOneOf]*")                        // annotation
                .WithMessage($"*property of type {nameof(DateTime)}*")      // rationale
                .WithMessage($"*'T' (of type {nameof(Char)})*")             // details
                .WithMessage("*a string is required*");                     // explanation
        }

        [TestMethod] public void InclusionComparison_AnchorValidForDataConvertedTarget() {
            // Arrange
            var translator = new Translator();
            var source = typeof(WaterSlide);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(
                    nameof(WaterSlide.Type),
                    InclusionOperator.In,
                    new object[] { "Straight", "Curly", "Funnel" }).And
                .NoOtherConstraints();
        }

        [TestMethod] public void InclusionComparison_AnchorValidForDataConvertedSource_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(SoccerTeam);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                            // source type
                .WithMessage($"*{nameof(SoccerTeam.WorldCupVictories)}*")   // source property
                .WithMessage("*[Check.IsNotOneOf]*")                        // annotation
                .WithMessage($"*property of type {nameof(String)}*")        // rationale
                .WithMessage($"*0 (of type {nameof(Int32)})*");             // details
        }

        [TestMethod] public void InclusionComparison_PropertyBothIsOneOfAndIsNotOneOf_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(SkiSlope);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                            // source type
                .WithMessage($"*{nameof(SkiSlope.Level)}*")                 // source property
                .WithMessage("*[Check.IsOneOf]*[Check.IsNotOneOf]*")        // annotation
                .WithMessage("*mutually exclusive*");                       // rationale
        }


        [TestMethod] public void CustomCheck_DefaultConstructed() {
            // Arrange
            var translator = new Translator();
            var source = typeof(VampireSlayer);

            // Act
            var translation = translator[source];
            var table = translation.Principal.Table;

            // Assert
            table.CheckConstraints.Should().HaveCount(1);
            table.CheckConstraints[0].Condition.Should().BeSameAs(CustomCheck.Clause);
            table.CheckConstraints[0].Name.Should().NotHaveValue();
            CustomCheck.CtorArgs.Should().BeEmpty();
            CustomCheck.Mock.Verify(icg => icg.MakeConstraint(
                Arg.IsSameSequence<IEnumerable<IField>>(
                    new IField[] {
                        table[new FieldName(nameof(VampireSlayer.Deaths))]
                    }
                ),
                It.Is<IEnumerable<DataConverter>>(s => s.Count() == 1),
                Settings.Default
            ));
        }

        [TestMethod] public void CustomCheck_ConstructedFromArguments() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Lyric);

            // Act
            var translation = translator[source];
            var table = translation.Principal.Table;

            // Assert
            table.CheckConstraints.Should().HaveCount(1);
            table.CheckConstraints[0].Condition.Should().BeSameAs(CustomCheck.Clause);
            table.CheckConstraints[0].Name.Should().NotHaveValue();
            CustomCheck.CtorArgs.Should().BeEquivalentTo(new object?[] { 13, false, "ABC", null });
            CustomCheck.Mock.Verify(icg => icg.MakeConstraint(
                Arg.IsSameSequence<IEnumerable<IField>>(
                    new IField[] {
                        table[new FieldName(nameof(Lyric.IsSpoken))]
                    }
                ),
                It.Is<IEnumerable<DataConverter>>(s => s.Count() == 1),
                Settings.Default
            ));
        }

        [TestMethod] public void CustomCheck_WithDataConverterApplied() {
            // Arrange
            var translator = new Translator();
            var source = typeof(DataStructure);

            // Act
            var translation = translator[source];
            var table = translation.Principal.Table;

            // Assert
            table.CheckConstraints.Should().HaveCount(1);
            table.CheckConstraints[0].Condition.Should().BeSameAs(CustomCheck.Clause);
            table.CheckConstraints[0].Name.Should().NotHaveValue();
            CustomCheck.CtorArgs.Should().BeEmpty();
            CustomCheck.Mock.Verify(icg => icg.MakeConstraint(
                Arg.IsSameSequence<IEnumerable<IField>>(
                    new IField[] {
                        table[new FieldName(nameof(DataStructure.RemoveBigO))]
                    }
                ),
                It.Is<IEnumerable<DataConverter>>(s => s.Count() == 1),
                Settings.Default
            ));
        }

        [TestMethod] public void CustomCheck_MultipleOnOneProperty() {
            // Arrange
            var translator = new Translator();
            var source = typeof(TarotCard);

            // Act
            var translation = translator[source];
            var table = translation.Principal.Table;

            // Assert
            table.CheckConstraints.Should().HaveCount(2);
            table.CheckConstraints[0].Condition.Should().BeSameAs(CustomCheck.Clause);
            table.CheckConstraints[0].Name.Should().NotHaveValue();
            CustomCheck.CtorArgs.Should().BeEquivalentTo(new object?[] { -14, '%' });
            CustomCheck.Mock.Verify(icg => icg.MakeConstraint(
                Arg.IsSameSequence<IEnumerable<IField>>(
                    new IField[] {
                        table[new FieldName(nameof(TarotCard.Pips))]
                    }
                ),
                It.Is<IEnumerable<DataConverter>>(s => s.Count() == 1),
                Settings.Default
            ), Times.Exactly(2));
        }

        [TestMethod] public void CustomCheck_TypeIsNotConstraintGenerator_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Patreon);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                                        // source type
                .WithMessage($"*{nameof(Patreon.Tier3)}*")                              // source property
                .WithMessage("*[Check]*")                                               // annotation
                .WithMessage($"*does not implement*{nameof(IConstraintGenerator)}*")    // rationale
                .WithMessage($"*{nameof(IWebProtocol)}*");                              // details
        }

        [TestMethod] public void CustomCheck_IsNotConstructibleFromArguments_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Transistor);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                                    // source type
                .WithMessage($"*{nameof(Transistor.Dopant)}*")                      // source property
                .WithMessage("*[Check]*")                                           // annotation
                .WithMessage("*not*construct*")                                     // rationale
                .WithMessage($"*{nameof(CustomCheck)}*")                            // details
                .WithMessage("*from arguments: \"Dopant\", 4*");                    // more details
        }

        [TestMethod] public void CustomCheck_ThrowsOnConstruction_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(BasketballPlayer);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                                    // source type
                .WithMessage($"*{nameof(BasketballPlayer.Rebounds)}*")              // source property
                .WithMessage("*[Check]*")                                           // annotation
                .WithMessage("*constructing*")                                      // rationale
                .WithMessage("*System Failure!*");                                  // details
        }

        [TestMethod] public void CustomCheck_PathOnAnnotationForScalar_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(TarPits);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                            // source type
                .WithMessage($"*{nameof(TarPits.FirstFossil)}*")            // source property
                .WithMessage("*[Check]*")                                   // annotation
                .WithMessage("*path*does not exist*")                       // rationale
                .WithMessage("*\"---\"*");                                  // details
        }

        
        [TestMethod] public void ComplexCheck_DefaultConstructed() {
            // Arrange
            var translator = new Translator();
            var source = typeof(CanterburyTale);

            // Act
            var translation = translator[source];
            var table = translation.Principal.Table;

            // Assert
            table.CheckConstraints.Should().HaveCount(1);
            table.CheckConstraints[0].Condition.Should().BeSameAs(CustomCheck.Clause);
            table.CheckConstraints[0].Name.Should().NotHaveValue();
            CustomCheck.CtorArgs.Should().BeEmpty();
            CustomCheck.Mock.Verify(icg => icg.MakeConstraint(
                Arg.IsSameSequence<IEnumerable<IField>>(
                    new IField[] {
                        table[new FieldName(nameof(CanterburyTale.FirstLine))]
                    }
                ),
                It.Is<IEnumerable<DataConverter>>(s => s.Count() == 1),
                Settings.Default
            ));
        }

        [TestMethod] public void ComplexCheck_ConstructedFromArguments() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Pope);

            // Act
            var translation = translator[source];
            var table = translation.Principal.Table;

            // Assert
            table.CheckConstraints.Should().HaveCount(1);
            table.CheckConstraints[0].Condition.Should().BeSameAs(CustomCheck.Clause);
            table.CheckConstraints[0].Name.Should().NotHaveValue();
            CustomCheck.CtorArgs.Should().BeEquivalentTo(new object?[] { -93, true, 'X' });
            CustomCheck.Mock.Verify(icg => icg.MakeConstraint(
                Arg.IsSameSequence<IEnumerable<IField>>(
                    new IField[] {
                        table[new FieldName(nameof(Pope.ConclaveRounds))]
                    }
                ),
                It.Is<IEnumerable<DataConverter>>(s => s.Count() == 1),
                Settings.Default
            ));
        }

        [TestMethod] public void ComplexCheck_MultipleDifferentFields() {
            // Arrange
            var translator = new Translator();
            var source = typeof(LinuxDistribution);

            // Act
            var translation = translator[source];
            var table = translation.Principal.Table;

            // Assert
            table.CheckConstraints.Should().HaveCount(1);
            table.CheckConstraints[0].Condition.Should().BeSameAs(CustomCheck.Clause);
            table.CheckConstraints[0].Name.Should().NotHaveValue();
            CustomCheck.CtorArgs.Should().BeEmpty();
            CustomCheck.Mock.Verify(icg => icg.MakeConstraint(
                Arg.IsSameSequence<IEnumerable<IField>>(
                    new IField[] {
                        table[new FieldName(nameof(LinuxDistribution.Major))],
                        table[new FieldName(nameof(LinuxDistribution.Minor))],
                        table[new FieldName(nameof(LinuxDistribution.Patch))]
                    }
                ),
                It.Is<IEnumerable<DataConverter>>(s => s.Count() == 3),
                Settings.Default
            ));
        }

        [TestMethod] public void ComplexCheck_FieldRepeated() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Muppet);

            // Act
            var translation = translator[source];
            var table = translation.Principal.Table;

            // Assert
            table.CheckConstraints.Should().HaveCount(1);
            table.CheckConstraints[0].Condition.Should().BeSameAs(CustomCheck.Clause);
            table.CheckConstraints[0].Name.Should().NotHaveValue();
            CustomCheck.CtorArgs.Should().BeEmpty();
            CustomCheck.Mock.Verify(icg => icg.MakeConstraint(
                Arg.IsSameSequence<IEnumerable<IField>>(
                    new IField[] {
                        table[new FieldName(nameof(Muppet.Name))],
                        table[new FieldName(nameof(Muppet.Name))],
                        table[new FieldName(nameof(Muppet.Name))],
                        table[new FieldName(nameof(Muppet.Name))]
                    }
                ),
                It.Is<IEnumerable<DataConverter>>(s => s.Count() == 4),
                Settings.Default
            ));
        }

        [TestMethod] public void ComplexCheck_NameSwappedFields() {
            // Arrange
            var translator = new Translator();
            var source = typeof(PastaSauce);

            // Act
            var translation = translator[source];
            var table = translation.Principal.Table;

            // Assert
            table.CheckConstraints.Should().HaveCount(1);
            table.CheckConstraints[0].Condition.Should().BeSameAs(CustomCheck.Clause);
            table.CheckConstraints[0].Name.Should().NotHaveValue();
            CustomCheck.CtorArgs.Should().BeEmpty();
            CustomCheck.Mock.Verify(icg => icg.MakeConstraint(
                Arg.IsSameSequence<IEnumerable<IField>>(
                    new IField[] {
                        table[new FieldName(nameof(PastaSauce.Cuisine))],
                        table[new FieldName(nameof(PastaSauce.ContainsTomatoes))]
                    }
                ),
                It.Is<IEnumerable<DataConverter>>(s => s.Count() == 2),
                Settings.Default
            ));
        }

        [TestMethod] public void ComplexCheck_DataConvertedField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Massacre);

            // Act
            var translation = translator[source];
            var table = translation.Principal.Table;

            // Assert
            table.CheckConstraints.Should().HaveCount(1);
            table.CheckConstraints[0].Condition.Should().BeSameAs(CustomCheck.Clause);
            table.CheckConstraints[0].Name.Should().NotHaveValue();
            CustomCheck.CtorArgs.Should().BeEmpty();
            CustomCheck.Mock.Verify(icg => icg.MakeConstraint(
                Arg.IsSameSequence<IEnumerable<IField>>(
                    new IField[] {
                        table[new FieldName(nameof(Massacre.When))],
                        table[new FieldName(nameof(Massacre.Casualties))],
                        table[new FieldName(nameof(Massacre.When))]
                    }
                ),
                It.Is<IEnumerable<DataConverter>>(s => s.Count() == 3),
                Settings.Default
            ));
        }

        [TestMethod] public void ComplexCheck_MultipleOnSingleEntity() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Musical);

            // Act
            var translation = translator[source];
            var table = translation.Principal.Table;

            // Assert
            table.CheckConstraints.Should().HaveCount(3);
            table.CheckConstraints[0].Condition.Should().BeSameAs(CustomCheck.Clause);
            table.CheckConstraints[1].Condition.Should().BeSameAs(CustomCheck.Clause);
            table.CheckConstraints[2].Condition.Should().BeSameAs(CustomCheck.Clause);
            table.CheckConstraints[0].Name.Should().NotHaveValue();
            table.CheckConstraints[1].Name.Should().NotHaveValue();
            table.CheckConstraints[2].Name.Should().NotHaveValue();
            CustomCheck.Mock.Verify(icg => icg.MakeConstraint(
                Arg.IsSameSequence<IEnumerable<IField>>(
                    new IField[] {
                        table[new FieldName(nameof(Musical.LengthMinutes))]
                    }
                ),
                It.Is<IEnumerable<DataConverter>>(s => s.Count() == 1),
                Settings.Default
            ));
            CustomCheck.Mock.Verify(icg => icg.MakeConstraint(
                Arg.IsSameSequence<IEnumerable<IField>>(
                    new IField[] {
                        table[new FieldName(nameof(Musical.SungThrough))]
                    }
                ),
                It.Is<IEnumerable<DataConverter>>(s => s.Count() == 1),
                Settings.Default
            ), Times.Exactly(2));
        }

        [TestMethod] public void ComplexCheck_TypeIsNotConstraintGenerator_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Mutant);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                                        // source type
                .WithMessage("*[Check.Complex]*")                                       // annotation
                .WithMessage($"*does not implement*{nameof(IConstraintGenerator)}*")    // rationale
                .WithMessage($"*{nameof(AssemblyLoadEventArgs)}*");                     // details
        }

        [TestMethod] public void ComplexCheck_IsNotConstructibleFromArguments_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(CookingOil);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                                    // source type
                .WithMessage("*[Check.Complex]*")                                   // annotation
                .WithMessage("*not*construct*")                                     // rationale
                .WithMessage($"*{nameof(CustomCheck)}*")                            // details
                .WithMessage("*from arguments: \"O\", 'I', 'L'*");                  // more details
        }

        [TestMethod] public void ComplexCheck_ThrowsOnConstruction_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Pirate);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                                    // source type
                .WithMessage("*[Check.Complex]*")                                   // annotation
                .WithMessage("*constructing*")                                      // rationale
                .WithMessage("*System Failure!*");                                  // details
        }

        [TestMethod] public void ComplexCheck_NoFieldsSpecified_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Terminator);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                                    // source type
                .WithMessage("*[Check.Complex]*")                                   // annotation
                .WithMessage("*at least one*Field*");                               // rationale
        }

        [TestMethod] public void ComplexCheck_NameSwappedFieldOriginalUsed_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Dam);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                                    // source type
                .WithMessage("*[Check.Complex]*")                                   // annotation
                .WithMessage("*does not exist*")                                    // rationale
                .WithMessage("*\"Width\"*");                                        // details
        }

        [TestMethod] public void ComplexCheck_UnrecognizedFieldName_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(PeaceTreaty);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                                    // source type
                .WithMessage("*[Check.Complex]*")                                   // annotation
                .WithMessage("*does not exist*")                                    // rationale
                .WithMessage("*\"Belligerents\"*");                                 // details
        }
    }
}
