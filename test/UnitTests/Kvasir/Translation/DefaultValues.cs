using FluentAssertions;
using Kvasir.Exceptions;
using Kvasir.Translation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Optional;
using System;

using static UT.Kvasir.Translation.TestComponents;

namespace UT.Kvasir.Translation {
    [TestClass, TestCategory("Default Values")]
    public class DefaultValueTests {
        [TestMethod] public void ValidNonNullScalarDefaults() {
            // Arrange
            var translator = new Translator();
            var source = typeof(BloodType);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField("ABO", Option.Some("O")).And
                .HaveField("RHPositive", Option.Some(true)).And
                .HaveField("ApproxPrevalence", Option.Some(0.5f)).And
                .HaveField("NumSubgroups", Option.Some(1)).And
                .HaveField("AnnualDonationsL", Option.None<object>()).And
                .NoOtherFields();
        }

        [TestMethod] public void ValidNonNullDateTimeDefault() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Umpire);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField("UniqueUmpireNumber", Option.None<object>()).And
                .HaveField("Debut", Option.Some(new DateTime(1970, 1, 1))).And
                .HaveField("UniformNumber", Option.None<object>()).And
                .HaveField("Name", Option.None<object>()).And
                .HaveField("Ejections", Option.None<object>()).And
                .NoOtherFields();
        }

        [TestMethod] public void ValidNonNullGuidDefault() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Saint);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField("SainthoodIdentifier", Option.Some(new Guid("81a130d2-502f-4cf1-a376-63edeb000e9f"))).And
                .HaveField("Name", Option.None<object>()).And
                .HaveField("CanonizationDate", Option.None<object>()).And
                .HaveField("FeastMonth", Option.None<object>()).And
                .HaveField("FeastDay", Option.None<object>()).And
                .NoOtherFields();
        }

        [TestMethod] public void NullDefaultForNullableField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Pepper);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField("Genus", Option.None<object>()).And
                .HaveField("Species", Option.None<object>()).And
                .HaveField("CommonName", Option.Some<object>(DBNull.Value)).And
                .HaveField("ScovilleRating", Option.None<object>()).And
                .NoOtherFields();
        }

        [TestMethod] public void InvalidUnconvertibleScalarDefault_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Battleship);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                            // source type
                .WithMessage($"*{nameof(Battleship.Length)}*")              // source property
                .WithMessage("*[Default]*")                                 // annotation
                .WithMessage($"*property of type {nameof(UInt16)}*")        // rationale
                .WithMessage($"*\"100 feet\" (of type {nameof(String)})*"); // details
        }

        [TestMethod] public void InvalidConvertibleScalarDefault_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(County);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                            // source type
                .WithMessage($"*{nameof(County.Population)}*")              // source property
                .WithMessage("*[Default]*")                                 // annotation
                .WithMessage($"*property of type {nameof(UInt64)}*")        // rationale
                .WithMessage($"*5000000 (of type {nameof(Int32)})*");       // details
        }

        [TestMethod] public void OtherwiseValidSingleElementArrayDefault_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(BilliardBall);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                        // source type
                .WithMessage($"*{nameof(BilliardBall.Number)}*")        // source property
                .WithMessage("*[Default]*")                             // annotation
                .WithMessage("*array*");                                // rationale
        }

        [TestMethod] public void InvalidlyFormattedDateTimeDefault_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Tournament);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                            // source type
                .WithMessage($"*{nameof(Tournament.Kickoff)}*")             // source property
                .WithMessage("*[Default]*")                                 // annotation
                .WithMessage($"*could not parse*into {nameof(DateTime)}*")  // rationale
                .WithMessage("*\"20030714\"*");                             // details
        }

        [TestMethod] public void InvalidRangeDateTimeDefault_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Sculpture);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                            // source type
                .WithMessage($"*{nameof(Sculpture.CreationDate)}*")         // source property
                .WithMessage("*[Default]*")                                 // annotation
                .WithMessage($"*could not parse*into {nameof(DateTime)}*")  // rationale
                .WithMessage("*\"1344-18-18\"*");                           // details
        }

        [TestMethod] public void NonStringDateTimeDefault_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(RomanEmperor);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                            // source type
                .WithMessage($"*{nameof(RomanEmperor.ReignEnd)}*")          // source property
                .WithMessage("*[Default]*")                                 // annotation
                .WithMessage($"*property of type {nameof(DateTime)}*")      // rationale
                .WithMessage($"*true (of type {nameof(Boolean)})*")         // details
                .WithMessage("*a string is required*");                     // explanation
        }

        [TestMethod] public void InvalidlyFormattedGuidDefault_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Gene);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                                // source type
                .WithMessage($"*{nameof(Gene.UUID)}*")                          // source property
                .WithMessage("*[Default]*")                                     // annotation
                .WithMessage($"*could not parse*into {nameof(Guid)}*")          // rationale
                .WithMessage("*\"ee98f44827b248a2bb9fc5ef342e7ab2!!!\"*");      // details
        }

        [TestMethod] public void NonStringGuidDefault_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(HogwartsHouse);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                            // source type
                .WithMessage($"*{nameof(HogwartsHouse.TermIndex)}*")        // source property
                .WithMessage("*[Default]*")                                 // annotation
                .WithMessage($"*property of type {nameof(Guid)}*")          // rationale
                .WithMessage($"*'^' (of type {nameof(Char)})*")             // details
                .WithMessage("*a string is required*");                     // explanation
        }

        [TestMethod] public void NullDefaultForNonNullableField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(RadioStation);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                        // source type
                .WithMessage($"*{nameof(RadioStation.CallSign)}*")      // source property
                .WithMessage("*null*for*non-nullable Field*");          // rationale
        }

        [TestMethod] public void ValidDefaultValueForOriginalTypeNotConvertedType() {
            // Arrange
            var translator = new Translator();
            var source = typeof(CrosswordClue);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField("PuzzleID", Option.None<object>()).And
                .HaveField("AcrossOrDown", Option.Some(65)).And
                .HaveField("Number", Option.None<object>()).And
                .HaveField("NumLetters", Option.None<object>()).And
                .HaveField("ClueText", Option.None<object>()).And
                .NoOtherFields();
        }

        [TestMethod] public void ValidDefaultForConvertedTypeNotOriginalType_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Coupon);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                            // source type
                .WithMessage($"*{nameof(Coupon.IsBOGO)}*")                  // source property
                .WithMessage("*[Default]*")                                 // annotation
                .WithMessage($"*property of type {nameof(Boolean)}*")       // rationale
                .WithMessage($"*0 (of type {nameof(Int32)})*");             // details
        }

        [TestMethod] public void MultipleDefaultsForSingleField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(SkeeBall);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                            // source type
                .WithMessage($"*{nameof(SkeeBall.L1Value)}*")               // source property
                .WithMessage("*[Default]*")                                 // annotation
                .WithMessage("*multiple*");                                 // rationale
        }

        [TestMethod] public void PathOnDefaultAnnotationForScalar_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(NativeAmericanTribe);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                        // source type
                .WithMessage($"*{nameof(NativeAmericanTribe.Exonym)}*") // source property
                .WithMessage("*[Default]*")                             // annotation
                .WithMessage("*path*does not exist*")                   // rationale
                .WithMessage("*\"---\"*");                              // details
        }
    }
}