using FluentAssertions;
using Kvasir.Schema;
using Kvasir.Translation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

using static UT.Kvasir.Translation.Globals;
using static UT.Kvasir.Translation.DiscretenessConstraints.IsOneOf;
using static UT.Kvasir.Translation.DiscretenessConstraints.IsNotOneOf;

namespace UT.Kvasir.Translation {
    [TestClass, TestCategory("Constraints - Discreteness")]
    public class IsOneOfTests {
        [TestMethod] public void IsOneOf_NumericFields() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(CoralReef);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("Longitude", InclusionOperator.In,
                    0f, 30f, 45f, 75f, 90f
                ).And
                .HaveConstraint("Length", InclusionOperator.In,
                    1000UL, 2000UL, 3000UL, 4000UL, 5000UL
                ).And
                .HaveConstraint("Area", InclusionOperator.In,
                    17, 190841, 79512759, 857791
                ).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsOneOf_TextualFields() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Encyclopedia);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("Letter", InclusionOperator.In,
                    'A', 'B', 'C', 'D', 'E', 'F', 'G'
                ).And
                .HaveConstraint("Edition", InclusionOperator.In,
                    "First", "Second", "Third", "Fourth"
                ).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsOneOf_BooleanField() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Astronaut);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("WalkedOnMoon", InclusionOperator.In,
                    true, false
                ).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsOneOf_DecimalField() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(CiderMill);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("AnnualTonnage", InclusionOperator.In,
                    0M, 100M, 1000M, 10000M, 100000M, 1000000M
                ).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsOneOf_DateTimeField() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Hospital);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("Opened", InclusionOperator.In,
                    new DateTime(2000, 1, 1), new DateTime(2000, 1, 2), new DateTime(2000, 1, 3)
                ).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsOneOf_GuidField() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Tsunami);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("TsunamiID", ComparisonOperator.EQ, new Guid("b334ae4e-98c3-4f63-83f8-2bc076eae31b")).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsOneOf_EnumerationField() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Tooth);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField("ToothID").OfTypeGuid().BeingNonNullable().And
                .HaveField("Origin").OfTypeEnumeration(
                    Tooth.Source.Animal,
                    Tooth.Source.Human
                ).BeingNonNullable().And
                .HaveField("Type").OfTypeEnumeration(
                    Tooth.ToothType.Incisor,
                    Tooth.ToothType.Bicuspid,
                    Tooth.ToothType.Canine,
                    Tooth.ToothType.Molar
                ).BeingNonNullable().And
                .HaveNoOtherFields().And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsOneOf_AggregateNestedScalarField() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Earring);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("MadeOf.Material", InclusionOperator.In,
                    "Gold", "Silver", "Plastic", "Titanium", "Wood", "Fiberglass", "Leather"
                ).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsOneOf_NestedAggregateProperty_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(PhoneticAlphabet);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`PhoneticAlphabet` → `All` (from \"Alphabet\") → ABCDEFGHIJKLMNOPQRS")
                .WithPath("ABCDEFGHIJK.GHIJK")
                .WithProblem("the annotation cannot be applied to a property of Aggregate type `GHIJK`")
                .WithAnnotations("[Check.IsOneOf]")
                .EndMessage();
        }

        [TestMethod] public void IsOneOf_ReferenceNestedScalarField() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(FerrisWheel);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("Designer.FirstName", InclusionOperator.In,
                    "Alexander", "Randall", "Corrine"
                ).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsOneOf_NestedReferenceProperty_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Pulsar);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Pulsar` → FirstObservedAt")
                .WithPath("Observatory")
                .WithProblem("the annotation cannot be applied to a property of Reference type `Observatory`")
                .WithAnnotations("[Check.IsOneOf]")
                .EndMessage();
        }

        [TestMethod] public void IsOneOf_RelationNestedScalarField() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Dinosaur);

            // Act
            var translation = translator[source];

            // Assert
            translation.Relations[0].Table.Should()
                .HaveConstraint("Item", InclusionOperator.In,
                    "Americas", "Eurasia", "Middle East", "Africa", "Australia", "Pacific Islands", "Arctic",
                    "Antarctica", "Oceans"
                ).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsOneOf_OriginalOnReferenceNestedScalar() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(PullRequest);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsOneOf_NestedRelationProperty_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Cheerleader);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Cheerleader` → PrimaryCheer")
                .WithPath("Moves")
                .WithProblem("the annotation cannot be applied to a property of Relation type `RelationMap<int, string>`")
                .WithAnnotations("[Check.IsOneOf]")
                .EndMessage();
        }

        [TestMethod] public void IsOneOf_NullableFields() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Wildfire);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("Cause", InclusionOperator.In,
                    "Lightning", "Arson", "Electrical"
                ).And
                .HaveConstraint("MaxTemperature", InclusionOperator.In,
                    5718.37, 1984.6, 279124.9
                ).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsOneOf_DuplicatedValues() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(HealingPotion);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("DieType", InclusionOperator.In,
                    4u, 8u, 10u, 12u, 20u, 100u
                ).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsOneOf_InconvertibleValue_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Battery);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidConstraintValueException>()
                .WithLocation("`Battery` → Voltage")
                .WithProblem("value \"six\" is of type `string`, not `int` as expected")
                .WithAnnotations("[Check.IsOneOf]")
                .EndMessage();
        }

        [TestMethod] public void IsOneOf_ConvertibleValue_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(TennisMatch);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidConstraintValueException>()
                .WithLocation("`TennisMatch` → Player1Score")
                .WithProblem("value 0 is of type `byte`, not `sbyte` as expected")
                .WithAnnotations("[Check.IsOneOf]")
                .EndMessage();
        }

        [TestMethod] public void IsOneOf_InvalidEnumerator_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(SpeedLimit);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidConstraintValueException>()
                .WithLocation("`SpeedLimit` → TypeOfStreet")
                .WithProblem("enumerator StreetType.40000 is not valid")
                .WithAnnotations("[Check.IsOneOf]")
                .EndMessage();
        }

        [TestMethod] public void IsOneOf_ArrayValue_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Flashcard);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidConstraintValueException>()
                .WithLocation("`Flashcard` → IsLearned")
                .WithProblem("value cannot be an array")
                .WithAnnotations("[Check.IsOneOf]")
                .EndMessage();
        }

        [TestMethod] public void IsOneOf_NullValue_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Prophet);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidConstraintValueException>()
                .WithLocation("`Prophet` → HebrewName")
                .WithProblem("constraint cannot contain 'null'")
                .WithAnnotations("[Check.IsOneOf]")
                .EndMessage();
        }

        [TestMethod] public void IsOneOf_DecimalValueIsNotDouble_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Carousel);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidConstraintValueException>()
                .WithLocation("`Carousel` → RoundDuration")
                .WithProblem("value 0.4 is of type `float`, not `double` as expected")
                .WithAnnotations("[Check.IsOneOf]")
                .EndMessage();
        }

        [TestMethod] public void IsOneOf_DecimalValueIsOutOfRange_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Borate);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidConstraintValueException>()
                .WithLocation("`Borate` → MolarMass")
                .WithProblem($"`double` {double.MinValue} is outside the supported range for `decimal`")
                .WithAnnotations("[Check.IsOneOf]")
                .EndMessage();
        }

        [TestMethod] public void IsOneOf_DateTimeValueIsNotString_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(DalaiLama);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidConstraintValueException>()
                .WithLocation("`DalaiLama` → Birthdate")
                .WithProblem("value 1824 is of type `ulong`, not `string` as expected")
                .WithAnnotations("[Check.IsOneOf]")
                .EndMessage();
        }

        [TestMethod] public void IsOneOf_DateTimeValueIsMalformatted_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Voicemail);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidConstraintValueException>()
                .WithLocation("`Voicemail` → When")
                .WithProblem("unable to parse `string` value \"Thursday\" as a `DateTime`")
                .WithAnnotations("[Check.IsOneOf]")
                .EndMessage();
        }

        [TestMethod] public void IsOneOf_DateTimeValueIsOutOfRange_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(FinalJeopardy);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidConstraintValueException>()
                .WithLocation("`FinalJeopardy` → AirDate")
                .WithProblem("unable to parse `string` value \"1299-08-45\" as a `DateTime`")
                .WithAnnotations("[Check.IsOneOf]")
                .EndMessage();
        }

        [TestMethod] public void IsOneOf_GuidValueIsNotString_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(BiologicalCycle);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidConstraintValueException>()
                .WithLocation("`BiologicalCycle` → ID")
                .WithProblem("value 'c' is of type `char`, not `string` as expected")
                .WithAnnotations("[Check.IsOneOf]")
                .EndMessage();
        }

        [TestMethod] public void IsOneOf_GuidValueIsMalformatted_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(WaterBottle);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidConstraintValueException>()
                .WithLocation("`WaterBottle` → ProductID")
                .WithProblem("unable to parse `string` value \"A-G-U-I-D\" as a `Guid`")
                .WithAnnotations("[Check.IsOneOf]")
                .EndMessage();
        }

        [TestMethod] public void IsOneOf_ValueMatchesDataConversionSourceType_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Burrito);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidConstraintValueException>()
                .WithLocation("`Burrito` → Protein")
                .WithProblem("value \"Chicken\" is of type `string`, not `int` as expected")
                .WithAnnotations("[Check.IsOneOf]")
                .EndMessage();
        }

        [TestMethod] public void IsOneOf_ValueMatchesDataConversionTargetType() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(WaterSlide);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("Type", InclusionOperator.In,
                    "Straight", "Curly", "Funnel"
                ).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsOneOf_ScalarConstrainedMultipleTimes() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Cannon);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("Capacity", InclusionOperator.In,
                    7, 2, 4, 1, 6
                ).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsOneOf_SingleEnumeratorAllowed() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Treehouse);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField("ID").OfTypeGuid().BeingNonNullable().And
                .HaveField("MadeBy").OfTypeEnumeration(
                    Treehouse.Manufacturing.Professional
                ).BeingNonNullable().And
                .HaveField("Elevation").OfTypeDouble().BeingNonNullable().And
                .HaveField("Height").OfTypeDouble().BeingNonNullable().And
                .HaveField("Length").OfTypeDouble().BeingNonNullable().And
                .HaveField("Width").OfTypeDouble().BeingNonNullable().And
                .HaveField("PrimaryWood").OfTypeText().BeingNonNullable().And
                .HaveNoOtherFields().And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsOneOf_PathIsNull_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Dragon);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`Dragon` → Species")
                .WithProblem("the path cannot be 'null'")
                .WithAnnotations("[Check.IsOneOf]")
                .EndMessage();
        }

        [TestMethod] public void IsOneOf_PathOnScalar_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(HomericHymn);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`HomericHymn` → Lines")
                .WithProblem("the path \"---\" does not exist")
                .WithAnnotations("[Check.IsOneOf]")
                .EndMessage();
        }

        [TestMethod] public void IsOneOf_NonExistentPathOnAggregate_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(MarbleLeague);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`MarbleLeague` → Victor")
                .WithProblem("the path \"---\" does not exist")
                .WithAnnotations("[Check.IsOneOf]")
                .EndMessage();
        }

        [TestMethod] public void IsOneOf_NoPathOnAggregate_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Artery);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Artery` → Name")
                .WithProblem("the annotation cannot be applied to a property of Aggregate type `Naming`")
                .WithAnnotations("[Check.IsOneOf]")
                .EndMessage();
        }

        [TestMethod] public void IsOneOf_NonExistentPathOnReference_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Safari);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`Safari` → Elephants")
                .WithProblem("the path \"---\" does not exist")
                .WithAnnotations("[Check.IsOneOf]")
                .EndMessage();
        }

        [TestMethod] public void IsOneOf_NonPrimaryKeyPathOnReference_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Adverb);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`Adverb` → WordSuffix")
                .WithProblem("the path \"PartOfSpeech\" does not exist")
                .WithAnnotations("[Check.IsOneOf]")
                .EndMessage();
        }

        [TestMethod] public void IsOneOf_NoPathOnReference_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Swamp);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Swamp` → PredominantTree")
                .WithProblem("the annotation cannot be applied to a property of Reference type `Tree`")
                .WithAnnotations("[Check.IsOneOf]")
                .EndMessage();
        }

        [TestMethod] public void IsOneOf_NonExistentPathOnRelation_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(PawnShop);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`PawnShop` → <synthetic> `Inventory`")
                .WithProblem("the path \"---\" does not exist")
                .WithAnnotations("[Check.IsOneOf]")
                .EndMessage();
        }

        [TestMethod] public void IsOneOf_NonAnchorPrimaryKeyPathOnRelation_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(DrinkingFountain);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`DrinkingFountain` → <synthetic> `Inspections`")
                .WithProblem("the path \"DrinkingFountain.WaterPressure\" does not exist")
                .WithAnnotations("[Check.IsOneOf]")
                .EndMessage();
        }

        [TestMethod] public void IsOneOf_NoPathOnRelation_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Hairstyle);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Hairstyle` → <synthetic> `Certifications`")
                .WithProblem("the annotation cannot be applied to a property of Relation type `RelationMap<Guid, bool>`")
                .WithAnnotations("[Check.IsOneOf]")
                .EndMessage();
        }

        [TestMethod] public void IsOneOf_DefaultValueDoesNotSatisfyConstraint_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Guillotine);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidatedDefaultException>()
                .WithLocation("`Guillotine` → Height")
                .WithProblem("the Field's default value of 113 does not pass the constraint")
                .WithAnnotations("[Check.IsOneOf]")
                .EndMessage();
        }

        [TestMethod] public void IsOneOf_ValidDefaultValueIsInvalidatedByConstraint_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(IKEAFurniture);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidatedDefaultException>()
                .WithLocation("`IKEAFurniture` → CatalogEntry")
                .WithPath("Room")
                .WithProblem("the Field's default value of Group.Den does not pass the constraint")
                .WithAnnotations("[Check.IsOneOf]")
                .EndMessage();
        }
    }

    [TestClass, TestCategory("Constraints - Discreteness")]
    public class IsNotOneOfTests {
        [TestMethod] public void IsNotOneOf_NumericFields() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(NationalAnthem);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("WordCount", InclusionOperator.NotIn,
                    0U, 5U
                ).And
                .HaveConstraint("Length", InclusionOperator.NotIn,
                    1.3f, 1.6f, 1.9f, 2.2f, 2.5f, 2.8f, 3.1f, 3.4f
                ).And
                .HaveConstraint("Revision", InclusionOperator.NotIn,
                    0L, 1L, 2L
                ).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNotOneOf_TextualFields() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Taxi);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("Quality", InclusionOperator.NotIn,
                    '1', '3', '5', '7', '9'
                ).And
                .HaveConstraint("Company", InclusionOperator.NotIn,
                    "YellowCab", "Cash Cab", "Uber", "Lyft"
                ).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNotOneOf_BooleanField() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(BirthControl);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("ForWomen", ComparisonOperator.EQ, true).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNotOneOf_DecimalField() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(HouseCommittee);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("Budget", InclusionOperator.NotIn,
                    0M, 1000M, 100000M, 100000000M
                ).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNotOneOf_DateTimeField() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(GamingConsole);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("Launched", InclusionOperator.NotIn,
                    new DateTime(1973, 4, 30), new DateTime(1973, 5, 30)
                ).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNotOneOf_GuidField() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Podcast);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("ID", InclusionOperator.NotIn,
                    new Guid("70324253-a5df-4208-9939-44a11243ceb0"), new Guid("2e748258-29e6-4abd-a1e1-3e93262e4c04")
                ).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNotOneOf_EnumerationField() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(RorschachInkBlot);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField("BlotNumber").OfTypeInt32().BeingNonNullable().And
                .HaveField("MostCommonAnswer").OfTypeEnumeration(
                    RorschachInkBlot.Object.Skin,
                    RorschachInkBlot.Object.Bat,
                    RorschachInkBlot.Object.Humans,
                    RorschachInkBlot.Object.Butterfly
                ).BeingNonNullable().And
                .HaveField("Commentary").OfTypeText().BeingNonNullable().And
                .HaveField("ImageURL").OfTypeText().BeingNonNullable().And
                .HaveNoOtherFields().And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNotOneOf_AggregateNestedScalarField() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Condiment);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("Nutrition.Sugar", InclusionOperator.NotIn,
                    (sbyte)7, (sbyte)12, (sbyte)105
                ).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNotOneOf_NestedAggregateProperty_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Tattoo);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Tattoo` → Ink")
                .WithPath("Color")
                .WithProblem("the annotation cannot be applied to a property of Aggregate type `Color`")
                .WithAnnotations("[Check.IsNotOneOf]")
                .EndMessage();
        }

        [TestMethod] public void IsNotOneOf_ReferenceNestedScalarField() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Lifeguard);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("FirstJob.PoolNumber", InclusionOperator.NotIn,
                    7U, 17U, 27U, 37U
                ).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNotOneOf_OriginalOnReferenceNestedScalar() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(SearchWarrant);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNotOneOf_NestedReferenceProperty_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(NurseryRhyme);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`NurseryRhyme` → MainCharacter")
                .WithPath("Character")
                .WithProblem("the annotation cannot be applied to a property of Reference type `Character`")
                .WithAnnotations("[Check.IsNotOneOf]")
                .EndMessage();
        }

        [TestMethod] public void IsNotOneOf_RelationNestedScalarField() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Infomercial);

            // Act
            var translation = translator[source];

            // Assert
            translation.Relations[0].Table.Should()
                .HaveConstraint("Value", InclusionOperator.NotIn,
                    new DateTime(2022, 3, 17), new DateTime(1965, 11, 14), new DateTime(1333, 1, 2)
                ).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNotOneOf_NestedRelationProperty_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(PersonOfTheYear);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`PersonOfTheYear` → TIME")
                .WithPath("Editions")
                .WithProblem("the annotation cannot be applied to a property of Relation type `RelationList<uint>`")
                .WithAnnotations("[Check.IsNotOneOf]")
                .EndMessage();
        }

        [TestMethod] public void IsNotOneOf_NullableFields() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(PIERoot);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("FrenchExample", InclusionOperator.NotIn,
                    "Manger", "Faire", "Avoir", "Parler"
                ).And
                .HaveConstraint("SpanishExample", InclusionOperator.NotIn,
                    "Comer", "Hacer", "Tener", "Hablar"
                ).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNotOneOf_DuplicatedValues() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Tweet);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("Grading", InclusionOperator.NotIn,
                    'A', 'E', 'I', 'O', 'U'
                ).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNotOneOf_InconvertibleValue_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Cancer);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidConstraintValueException>()
                .WithLocation("`Cancer` → RegionAffected")
                .WithProblem("value 17.3 is of type `float`, not `string` as expected")
                .WithAnnotations("[Check.IsNotOneOf]")
                .EndMessage();
        }

        [TestMethod] public void IsNotOneOf_ConvertibleValue_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Avatar);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidConstraintValueException>()
                .WithLocation("`Avatar` → DebutEpisode")
                .WithProblem("value 8 is of type `byte`, not `ushort` as expected")
                .WithAnnotations("[Check.IsNotOneOf]")
                .EndMessage();
        }

        [TestMethod] public void IsNotOneOf_InvalidEnumerator_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Emotion);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidConstraintValueException>()
                .WithLocation("`Emotion` → Connotation")
                .WithProblem("enumerator EmotionType.-3 is not valid")
                .WithAnnotations("[Check.IsNotOneOf]")
                .EndMessage();
        }

        [TestMethod] public void IsNotOneOf_ArrayValue_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Wristwatch);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidConstraintValueException>()
                .WithLocation("`Wristwatch` → Brand")
                .WithProblem("value cannot be an array")
                .WithAnnotations("[Check.IsNotOneOf]")
                .EndMessage();
        }

        [TestMethod] public void IsNotOneOf_NullValue_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Ballet);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidConstraintValueException>()
                .WithLocation("`Ballet` → OpusNumber")
                .WithProblem("constraint cannot contain 'null'")
                .WithAnnotations("[Check.IsNotOneOf]")
                .EndMessage();
        }

        [TestMethod] public void IsNotOneOf_DecimalValueIsNotDouble_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(AmericanIdol);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidConstraintValueException>()
                .WithLocation("`AmericanIdol` → VoteShare")
                .WithProblem("value \"0.90\" is of type `string`, not `double` as expected")
                .WithAnnotations("[Check.IsNotOneOf]")
                .EndMessage();
        }

        [TestMethod] public void IsNotOneOf_DecimalValueIsOutOfRange_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(RussianTsar);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidConstraintValueException>()
                .WithLocation("`RussianTsar` → DaysReigned")
                .WithProblem($"`double` {double.MaxValue} is outside the supported range for `decimal`")
                .WithAnnotations("[Check.IsNotOneOf]")
                .EndMessage();
        }

        [TestMethod] public void IsNotOneOf_DateTimeValueIsNotString_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Mayor);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidConstraintValueException>()
                .WithLocation("`Mayor` → TermEnd")
                .WithProblem("value 'T' is of type `char`, not `string` as expected")
                .WithAnnotations("[Check.IsNotOneOf]")
                .EndMessage();
        }

        [TestMethod] public void IsNotOneOf_DateTimeValueIsMalformatted_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Inator);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidConstraintValueException>()
                .WithLocation("`Inator` → Debut")
                .WithProblem("unable to parse `string` value \"1875~06~22\" as a `DateTime`")
                .WithAnnotations("[Check.IsNotOneOf]")
                .EndMessage();
        }

        [TestMethod] public void IsNotOneOf_DateTimeValueIsOutOfRange_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Museum);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidConstraintValueException>()
                .WithLocation("`Museum` → GrandOpening")
                .WithProblem("unable to parse `string` value \"1375-49-14\" as a `DateTime`")
                .WithAnnotations("[Check.IsNotOneOf]")
                .EndMessage();
        }

        [TestMethod] public void IsNotOneOf_GuidValueIsNotString_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Cruise);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidConstraintValueException>()
                .WithLocation("`Cruise` → CruiseID")
                .WithProblem("value 'f' is of type `char`, not `string` as expected")
                .WithAnnotations("[Check.IsNotOneOf]")
                .EndMessage();
        }

        [TestMethod] public void IsNotOneOf_GuidValueIsMalformatted_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Union);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidConstraintValueException>()
                .WithLocation("`Union` → UnionID")
                .WithProblem("unable to parse `string` value \"b46cfa0c-545e-4279-93d6-d1236r373a2b\" as a `Guid`")
                .WithAnnotations("[Check.IsNotOneOf]")
                .EndMessage();
        }

        [TestMethod] public void IsNotOneOf_ValueMatchesDataConversionSourceType_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Guitar);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidConstraintValueException>()
                .WithLocation("`Guitar` → Brand")
                .WithProblem("value \"Cardboard\" is of type `string`, not `int` as expected")
                .WithAnnotations("[Check.IsNotOneOf]")
                .EndMessage();
        }

        [TestMethod] public void IsNotOneOf_ValueMatchesDataConversionTargetType() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(SoccerTeam);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("WorldCupVictories", InclusionOperator.NotIn,
                    0, -3, 111
                ).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNotOneOf_BothBooleansDisallowed_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Transformer);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<UnsatisfiableConstraintException>()
                .WithLocation("`Transformer` → IsAutobot")
                .WithProblem("all of the explicitly allowed values fail at least one other constraint")
                .EndMessage();
        }

        [TestMethod] public void IsNotOneOf_AllEnumeratorsDisallowed_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(ProgrammingLanguage);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<UnsatisfiableConstraintException>()
                .WithLocation("`ProgrammingLanguage` → Type")
                .WithProblem("all of the explicitly allowed values fail at least one other constraint")
                .EndMessage();
        }

        [TestMethod] public void IsNotOneOf_ScalarConstrainedMultipleTimes() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Eurovision);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("Year", InclusionOperator.NotIn,
                    (ushort)0, (ushort)3
                ).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNotOneOf_PathIsNull_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Tuxedo);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`Tuxedo` → Size")
                .WithProblem("the path cannot be 'null'")
                .WithAnnotations("[Check.IsNotOneOf]")
                .EndMessage();
        }

        [TestMethod] public void IsNotOneOf_PathOnScalar_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Donut);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`Donut` → Flavor")
                .WithProblem("the path \"---\" does not exist")
                .WithAnnotations("[Check.IsNotOneOf]")
                .EndMessage();
        }

        [TestMethod] public void IsNotOneOf_NonExistentPathOnAggregate_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Necktie);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`Necktie` → Measurements")
                .WithProblem("the path \"---\" does not exist")
                .WithAnnotations("[Check.IsNotOneOf]")
                .EndMessage();
        }

        [TestMethod] public void IsNotOneOf_NoPathOnAggregate_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Scattergories);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Scattergories` → Round")
                .WithProblem("the annotation cannot be applied to a property of Aggregate type `Page`")
                .WithAnnotations("[Check.IsNotOneOf]")
                .EndMessage();
        }

        [TestMethod] public void IsNotOneOf_NonExistentPathOnReference_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Pencil);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`Pencil` → Lead")
                .WithProblem("the path \"---\" does not exist")
                .WithAnnotations("[Check.IsNotOneOf]")
                .EndMessage();
        }

        [TestMethod] public void IsNotOneOf_NonPrimaryKeyPathOnReference_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Mitzvah);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`Mitzvah` → Commandment")
                .WithProblem("the path \"Hebrew\" does not exist")
                .WithAnnotations("[Check.IsNotOneOf]")
                .EndMessage();
        }

        [TestMethod] public void IsNotOneOf_NoPathOnReference_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Eunuch);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Eunuch` → Castrator")
                .WithProblem("the annotation cannot be applied to a property of Reference type `Person`")
                .WithAnnotations("[Check.IsNotOneOf]")
                .EndMessage();
        }

        [TestMethod] public void IsNotOneOf_NonExistentPathOnRelation_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(PhoneBook);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`PhoneBook` → <synthetic> `PhoneNumbers`")
                .WithProblem("the path \"---\" does not exist")
                .WithAnnotations("[Check.IsNotOneOf]")
                .EndMessage();
        }

        [TestMethod] public void IsNotOneOf_NonAnchorPrimaryKeyPathOnRelation_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Bakugan);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`Bakugan` → <synthetic> `AbilityCards`")
                .WithProblem("the path \"Bakugan.BakuganName\" does not exist")
                .WithAnnotations("[Check.IsNotOneOf]")
                .EndMessage();
        }

        [TestMethod] public void IsNotOneOf_NoPathOnRelation_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(QRCode);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`QRCode` → <synthetic> `Vertical`")
                .WithProblem("the annotation cannot be applied to a property of Relation type `RelationList<bool>`")
                .WithAnnotations("[Check.IsNotOneOf]")
                .EndMessage();
        }

        [TestMethod] public void IsNotOneOf_DefaultValueDoesNotSatisfyConstraint_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Pie);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidatedDefaultException>()
                .WithLocation("`Pie` → Flavor")
                .WithProblem("the Field's default value of \"Anise\" does not pass the constraint")
                .WithAnnotations("[Check.IsNotOneOf]")
                .EndMessage();
        }

        [TestMethod] public void IsNotOneOf_ValidDefaultValueIsInvalidatedByConstraint_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(GirlScoutCookie);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidatedDefaultException>()
                .WithLocation("`GirlScoutCookie` → Label")
                .WithPath("Calories")
                .WithProblem("the Field's default value of 0.0 does not pass the constraint")
                .WithAnnotations("[Check.IsNotOneOf]")
                .EndMessage();
        }
    }
}
