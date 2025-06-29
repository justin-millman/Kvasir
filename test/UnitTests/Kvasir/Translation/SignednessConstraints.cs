using FluentAssertions;
using Kvasir.Schema;
using Kvasir.Translation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using static UT.Kvasir.Translation.Globals;
using static UT.Kvasir.Translation.SignednessConstraints.IsPositive;
using static UT.Kvasir.Translation.SignednessConstraints.IsNegative;
using static UT.Kvasir.Translation.SignednessConstraints.IsNonZero;

namespace UT.Kvasir.Translation {
    [TestClass, TestCategory("Constraints - Signedness")]
    public class IsPositiveTests {
        [TestMethod] public void IsPositive_NonNullableNumericFields() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(GreekLetter);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("NumericValue", ComparisonOperator.GT, 0).And
                .HaveConstraint("Frequency", ComparisonOperator.GT, 0M).And
                .HaveConstraint("Index", ComparisonOperator.GT, (byte)0).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsPositive_NullableNumericFields() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(MedicalSpecialty);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("Practitioners", ComparisonOperator.GT, 0UL).And
                .HaveConstraint("AverageSalary", ComparisonOperator.GT, 0M).And
                .HaveConstraint("YearsSchool", ComparisonOperator.GT, (sbyte)0).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsPositive_TextualField_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(FieldGoal);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`FieldGoal` → Kicker")
                .WithProblem("the annotation cannot be applied to a Field of non-numeric type `string`")
                .WithAnnotations("[Check.IsPositive]")
                .EndMessage();
        }

        [TestMethod] public void IsPositive_BooleanField_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(GolfHole);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`GolfHole` → ContainsWaterHazard")
                .WithProblem("the annotation cannot be applied to a Field of non-numeric type `bool`")
                .WithAnnotations("[Check.IsPositive]")
                .EndMessage();
        }

        [TestMethod] public void IsPositive_DateOnlyField_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Jotunn);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Jotunn` → FirstMentioned")
                .WithProblem("the annotation cannot be applied to a Field of non-numeric type `DateOnly`")
                .WithAnnotations("[Check.IsPositive]")
                .EndMessage();
        }

        [TestMethod] public void IsPositive_DateTimeField_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Gymnast);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Gymnast` → Birthdate")
                .WithProblem("the annotation cannot be applied to a Field of non-numeric type `DateTime`")
                .WithAnnotations("[Check.IsPositive]")
                .EndMessage();
        }

        [TestMethod] public void IsPositive_GuidField_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Documentary);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Documentary` → ID")
                .WithProblem("the annotation cannot be applied to a Field of non-numeric type `Guid`")
                .WithAnnotations("[Check.IsPositive]")
                .EndMessage();
        }

        [TestMethod] public void IsPositive_EnumerationField_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Mythbusting);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Mythbusting` → Rating")
                .WithProblem("the annotation cannot be applied to a Field of non-numeric type `Resolution`")
                .WithAnnotations("[Check.IsPositive]")
                .EndMessage();
        }

        [TestMethod] public void IsPositive_AggregateNestedApplicableScalar() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(IceAge);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("Ago.Length", ComparisonOperator.GT, (short)0).And
                .HaveConstraint("Ago.SubLength", ComparisonOperator.GT, (short)0).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsPositive_AggregateNestedInapplicableScalar_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(GoldenRaspberry);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`GoldenRaspberry` → RunnerUp")
                .WithPath("Movie")
                .WithProblem("the annotation cannot be applied to a Field of non-numeric type `string`")
                .WithAnnotations("[Check.IsPositive]")
                .EndMessage();
        }

        [TestMethod] public void IsPositive_NestedAggregate_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(SudokuPuzzle);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`SudokuPuzzle` → LowerLeft")
                .WithPath("Bottom")
                .WithProblem("the annotation cannot be applied to a property of Aggregate type `Row`")
                .WithAnnotations("[Check.IsPositive]")
                .EndMessage();
        }

        [TestMethod] public void IsPositive_ReferenceNestedApplicableScalar() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Runway);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("Host.ID", ComparisonOperator.GT, 0U).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsPositive_ReferenceNestedInapplicableScalar_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(CaesareanSection);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`CaesareanSection` → Doctor")
                .WithPath("LastName")
                .WithProblem("the annotation cannot be applied to a Field of non-numeric type `string`")
                .WithAnnotations("[Check.IsPositive]")
                .EndMessage();
        }

        [TestMethod] public void IsPositive_NestedReference_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Lamp);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Lamp` → Power")
                .WithPath("Unit")
                .WithProblem("the annotation cannot be applied to a property of Reference type `Unit`")
                .WithAnnotations("[Check.IsPositive]")
                .EndMessage();
        }

        [TestMethod] public void IsPositive_PreDefinedInstance_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Russo);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Russo` → Justin")
                .WithProblem("the annotation cannot be applied to a pre-defined instance property")
                .WithAnnotations("[Check.IsPositive]")
                .EndMessage();
        }
        
        [TestMethod] public void IsPositive_OriginalOnReferenceNestedScalar() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Lycanthrope);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsPositive_RelationNestedApplicableScalar() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(ArtificialIntelligence);

            // Act
            var translation = translator[source];

            // Assert
            translation.Relations[0].Table.Should()
                .HaveNoOtherConstraints();
            translation.Relations[1].Table.Should()
                .HaveConstraint("Item", ComparisonOperator.GT, 0.0).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsPositive_RelationNestedInapplicableScalar_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Margarita);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Margarita` → <synthetic> `Ingredients`")
                .WithPath("Item.ID")
                .WithProblem("the annotation cannot be applied to a Field of non-numeric type `Guid`")
                .WithAnnotations("[Check.IsPositive]")
                .EndMessage();
        }

        [TestMethod] public void IsPositive_NestedRelation_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(PulitzerPrize);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`PulitzerPrize` → AwardCommittee")
                .WithPath("Members")
                .WithProblem("the annotation cannot be applied to a property of Relation type `RelationList<string>`")
                .WithAnnotations("[Check.IsPositive]")
                .EndMessage();
        }

        [TestMethod] public void IsPositive_FieldWithNumericDataConversionTarget() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(SwimmingPool);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("Classification", ComparisonOperator.GT, 0).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsPositive_FieldWithNumericDataConversionSource_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(WikipediaPage);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`WikipediaPage` → Languages")
                .WithProblem("the annotation cannot be applied to a Field of non-numeric type `string`")
                .WithAnnotations("[Check.IsPositive]")
                .EndMessage();
        }

        [TestMethod] public void IsPositive_ScalarConstrainedMultipleTimes_Redundant() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(BaseballCard);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("CardNumber", ComparisonOperator.GT, 0).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsPositive_PathIsNull_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(HotSpring);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`HotSpring` → Elevation")
                .WithProblem("the path cannot be 'null'")
                .WithAnnotations("[Check.IsPositive]")
                .EndMessage();
        }

        [TestMethod] public void IsPositive_PathOnScalar_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Canal);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`Canal` → Length")
                .WithProblem("the path \"---\" does not exist")
                .WithAnnotations("[Check.IsPositive]")
                .EndMessage();
        }

        [TestMethod] public void IsPositive_NonExistentPathOnAggregate_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(SharkWeek);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`SharkWeek` → Info")
                .WithProblem("the path \"---\" does not exist")
                .WithAnnotations("[Check.IsPositive]")
                .EndMessage();
        }

        [TestMethod] public void IsPositive_NoPathOnAggregate_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Philosopher);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Philosopher` → Name")
                .WithProblem("the annotation cannot be applied to a property of Aggregate type `Naming`")
                .WithAnnotations("[Check.IsPositive]")
                .EndMessage();
        }

        [TestMethod] public void IsPositive_NonExistentPathOnReference_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(HappyHour);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`HappyHour` → Location")
                .WithProblem("the path \"---\" does not exist")
                .WithAnnotations("[Check.IsPositive]")
                .EndMessage();
        }

        [TestMethod] public void IsPositive_NonPrimaryKeyPathOnReference_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Aquifer);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`Aquifer` → DiscoveringGeologist")
                .WithProblem("the path \"Qualifications\" does not exist")
                .WithAnnotations("[Check.IsPositive]")
                .EndMessage();
        }

        [TestMethod] public void IsPositive_NoPathOnReference_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(FactCheck);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`FactCheck` → Fact")
                .WithProblem("the annotation cannot be applied to a property of Reference type `Statement`")
                .WithAnnotations("[Check.IsPositive]")
                .EndMessage();
        }

        [TestMethod] public void IsPositive_NonExistentPathOnRelation_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Sukkah);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`Sukkah` → <synthetic> `Builders`")
                .WithProblem("the path \"---\" does not exist")
                .WithAnnotations("[Check.IsPositive]")
                .EndMessage();
        }

        [TestMethod] public void IsPositive_NonAnchorPrimaryKeyPathOnRelation_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Screenwriter);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`Screenwriter` → <synthetic> `Scripts`")
                .WithProblem("the path \"Item.WordCount\" does not exist")
                .WithAnnotations("[Check.IsPositive]")
                .EndMessage();
        }

        [TestMethod] public void IsPositive_NoPathOnRelation_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Typewriter);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Typewriter` → <synthetic> `MissingKeys`")
                .WithProblem("the annotation cannot be applied to a property of Relation type `RelationSet<char>`")
                .WithAnnotations("[Check.IsPositive]")
                .EndMessage();
        }

        [TestMethod] public void IsPositive_DefaultValueDoesNotSatisfyConstraint_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(ScoobyDooFilm);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidatedDefaultException>()
                .WithLocation("`ScoobyDooFilm` → Runtime")
                .WithProblem("the Field's default value of -89 does not pass the constraint")
                .WithAnnotations("[Check.IsPositive]")
                .EndMessage();
        }

        [TestMethod] public void IsPositive_ValidDefaultValueIsInvalidatedByConstraint_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Cyclops);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidatedDefaultException>()
                .WithLocation("`Cyclops` → FirstMentioned")
                .WithPath("Book")
                .WithProblem("the Field's default value of -9 does not pass the constraint")
                .WithAnnotations("[Check.IsPositive]")
                .EndMessage();
        }
    }

    [TestClass, TestCategory("Constraints - Signedness")]
    public class IsNegativeTests {
        [TestMethod] public void IsNegative_SignedNonNullableNumericFields() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Acid);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("pH", ComparisonOperator.LT, 0f).And
                .HaveConstraint("FreezingPoint", ComparisonOperator.LT, (short)0).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNegative_UnsignedNumericFields_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Cereal);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Cereal` → CaloriesPerServing")
                .WithProblem("the annotation cannot be applied to a Field of unsigned numeric type `ushort`")
                .WithAnnotations("[Check.IsNegative]")
                .EndMessage();
        }

        [TestMethod] public void IsNegative_SignedNullableNumericFields() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(ConcentrationCamp);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("Inmates", ComparisonOperator.LT, 0.0).And
                .HaveConstraint("Casualties", ComparisonOperator.LT, 0L).And
                .HaveConstraint("DaysOperational", ComparisonOperator.LT, (short)0).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNegative_TextualField_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(KeySignature);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`KeySignature` → Note")
                .WithProblem("the annotation cannot be applied to a Field of non-numeric type `char`")
                .WithAnnotations("[Check.IsNegative]")
                .EndMessage();
        }

        [TestMethod] public void IsNegative_BooleanField_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(SporcleQuiz);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`SporcleQuiz` → Published")
                .WithProblem("the annotation cannot be applied to a Field of non-numeric type `bool`")
                .WithAnnotations("[Check.IsNegative]")
                .EndMessage();
        }

        [TestMethod] public void IsNegative_DateOnlyField_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(VenmoRequest);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`VenmoRequest` → RequestedOn")
                .WithProblem("the annotation cannot be applied to a Field of non-numeric type `DateOnly`")
                .WithAnnotations("[Check.IsNegative]")
                .EndMessage();
        }

        [TestMethod] public void IsNegative_DateTimeField_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Olympiad);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Olympiad` → OpeningCeremony")
                .WithProblem("the annotation cannot be applied to a Field of non-numeric type `DateTime`")
                .WithAnnotations("[Check.IsNegative]")
                .EndMessage();
        }

        [TestMethod] public void IsNegative_GuidField_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(W2);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`W2` → FormID")
                .WithProblem("the annotation cannot be applied to a Field of non-numeric type `Guid`")
                .WithAnnotations("[Check.IsNegative]")
                .EndMessage();
        }

        [TestMethod] public void IsNegative_EnumerationField_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(SerialKiller);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`SerialKiller` → CurrentStatus")
                .WithProblem("the annotation cannot be applied to a Field of non-numeric type `Status`")
                .WithAnnotations("[Check.IsNegative]")
                .EndMessage();
        }

        [TestMethod] public void IsNegative_AggregateNestedApplicableScalar() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Flood);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("Occurrence.Month", ComparisonOperator.LT, (sbyte)0).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNegative_AggregateNestedInapplicableScalar_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(TrolleyProblem);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`TrolleyProblem` → Pull")
                .WithPath("Label")
                .WithProblem("the annotation cannot be applied to a Field of non-numeric type `string`")
                .WithAnnotations("[Check.IsNegative]")
                .EndMessage();
        }

        [TestMethod] public void IsNegative_NestedAggregate_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Pharaoh);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Pharaoh` → Dynasty")
                .WithPath("Kingdom")
                .WithProblem("the annotation cannot be applied to a property of Aggregate type `EgyptianKingdom`")
                .WithAnnotations("[Check.IsNegative]")
                .EndMessage();
        }

        [TestMethod] public void IsNegative_ReferenceNestedApplicableScalar() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(HawaiianGod);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("MaoriEquivalent.DeityID", ComparisonOperator.LT, (short)0).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNegative_ReferenceNestedInapplicableScalar_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(OceanCurrent);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`OceanCurrent` → WhichOcean")
                .WithPath("Name")
                .WithProblem("the annotation cannot be applied to a Field of non-numeric type `string`")
                .WithAnnotations("[Check.IsNegative]")
                .EndMessage();
        }

        [TestMethod] public void IsNegative_NestedReference_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(AirBNB);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`AirBNB` → HouseAddress")
                .WithPath("State")
                .WithProblem("the annotation cannot be applied to a property of Reference type `State`")
                .WithAnnotations("[Check.IsNegative]")
                .EndMessage();
        }

        [TestMethod] public void IsNegative_PreDefinedInstance_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(MBTI);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`MBTI` → ISFP")
                .WithProblem("the annotation cannot be applied to a pre-defined instance property")
                .WithAnnotations("[Check.IsNegative]")
                .EndMessage();
        }

        [TestMethod] public void IsNegative_OriginalOnReferenceNestedScalar() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(DragonRider);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNegative_RelationNestedApplicableScalar() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Almanac);

            // Act
            var translation = translator[source];

            // Assert
            translation.Relations[0].Table.Should()
                .HaveConstraint("Item.PageEnd", ComparisonOperator.LT, 0).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNegative_RelationNestedInapplicableScalar_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(LandMine);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`LandMine` → <synthetic> `Casualties`")
                .WithPath("Item")
                .WithProblem("the annotation cannot be applied to a Field of non-numeric type `string`")
                .WithAnnotations("[Check.IsNegative]")
                .EndMessage();
        }

        [TestMethod] public void IsNegative_NestedRelation_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(YahtzeeGame);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`YahtzeeGame` → Player1")
                .WithPath("Score")
                .WithProblem("the annotation cannot be applied to a property of Relation type `RelationMap<Category, byte>`")
                .WithAnnotations("[Check.IsNegative]")
                .EndMessage();
        }

        [TestMethod] public void IsNegative_FieldWithNumericDataConversionTarget() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Boxer);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("TKOs", ComparisonOperator.LT, 0).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNegative_FieldWithNumericDataConversionSource_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Archangel);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Archangel` → FirstAppearance")
                .WithProblem("the annotation cannot be applied to a Field of non-numeric type `string`")
                .WithAnnotations("[Check.IsNegative]")
                .EndMessage();
        }

        [TestMethod] public void IsNegative_ScalarConstrainedMultipleTimes_Redundant() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Alkene);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("FreezingPoint", ComparisonOperator.LT, 0.0).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNegative_PathIsNull_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Climate);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`Climate` → AverageLowTemperature")
                .WithProblem("the path cannot be 'null'")
                .WithAnnotations("[Check.IsNegative]")
                .EndMessage();
        }

        [TestMethod] public void IsNegative_PathOnScalar_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(CircleOfHell);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`CircleOfHell` → Level")
                .WithProblem("the path \"---\" does not exist")
                .WithAnnotations("[Check.IsNegative]")
                .EndMessage();
        }

        [TestMethod] public void IsNegative_NonExistentPathOnAggregate_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(VolleyballMatch);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`VolleyballMatch` → FourthSet")
                .WithProblem("the path \"---\" does not exist")
                .WithAnnotations("[Check.IsNegative]")
                .EndMessage();
        }

        [TestMethod] public void IsNegative_NoPathOnAggregate_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Yacht);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Yacht` → Sails")
                .WithProblem("the annotation cannot be applied to a property of Aggregate type `ShipSails`")
                .WithAnnotations("[Check.IsNegative]")
                .EndMessage();
        }

        [TestMethod] public void IsNegative_NonExistentPathOnReference_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Pharmacy);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`Pharmacy` → HeadPharmacist")
                .WithProblem("the path \"---\" does not exist")
                .WithAnnotations("[Check.IsNegative]")
                .EndMessage();
        }

        [TestMethod] public void IsNegative_NonPrimaryKeyPathOnReference_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Popcorn);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`Popcorn` → Topping")
                .WithProblem("the path \"Calories\" does not exist")
                .WithAnnotations("[Check.IsNegative]")
                .EndMessage();
        }

        [TestMethod] public void IsNegative_NoPathOnReference_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(WinForm);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`WinForm` → SubmitButton")
                .WithProblem("the annotation cannot be applied to a property of Reference type `Button`")
                .WithAnnotations("[Check.IsNegative]")
                .EndMessage();
        }

        [TestMethod] public void IsNegative_NonExistentPathOnRelation_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(RomanFestival);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`RomanFestival` → <synthetic> `PossibleDates`")
                .WithProblem("the path \"---\" does not exist")
                .WithAnnotations("[Check.IsNegative]")
                .EndMessage();
        }

        [TestMethod] public void IsNegative_NonAnchorPrimaryKeyPathOnRelation_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(AmberAlert);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`AmberAlert` → <synthetic> `VehicleDescription`")
                .WithProblem("the path \"AmberAlert.EmergencyContactNumber\" does not exist")
                .WithAnnotations("[Check.IsNegative]")
                .EndMessage();
        }

        [TestMethod] public void IsNegative_NoPathOnRelation_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(BoyBand);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`BoyBand` → <synthetic> `Members`")
                .WithProblem("the annotation cannot be applied to a property of Relation type `RelationList<Singer>`")
                .WithAnnotations("[Check.IsNegative]")
                .EndMessage();
        }

        [TestMethod] public void IsNegative_DefaultValueDoesNotSatisfyConstraint_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(SuperPAC);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidatedDefaultException>()
                .WithLocation("`SuperPAC` → TotalRaised")
                .WithProblem("the Field's default value of 0 does not pass the constraint")
                .WithAnnotations("[Check.IsNegative]")
                .EndMessage();
        }

        [TestMethod] public void IsNegative_ValidDefaultValueIsInvalidatedByConstraint_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(PressSecretary);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidatedDefaultException>()
                .WithLocation("`PressSecretary` → EndDate")
                .WithPath("Year")
                .WithProblem("the Field's default value of 1563 does not pass the constraint")
                .WithAnnotations("[Check.IsNegative]")
                .EndMessage();
        }
    }

    [TestClass, TestCategory("Constraints - Signedness")]
    public class IsNonZeroTests {
        [TestMethod] public void IsNonZero_NonNullableNumericFields() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(RegularPolygon);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("NumEdges", ComparisonOperator.NE, (ushort)0).And
                .HaveConstraint("NumVertices", ComparisonOperator.NE, (sbyte)0).And
                .HaveConstraint("InternalAngle", ComparisonOperator.NE, 0.0).And
                .HaveConstraint("ExternalAngle", ComparisonOperator.NE, 0M).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNonZero_NullableNumericFields() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Skittles);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("Weight", ComparisonOperator.NE, 0.0).And
                .HaveConstraint("ServingSizeCalories", ComparisonOperator.NE, (short)0).And
                .HaveConstraint("PiecesPerBag", ComparisonOperator.NE, 0U).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNonZero_TextualField_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Brassiere);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Brassiere` → CupSize")
                .WithProblem("the annotation cannot be applied to a Field of non-numeric type `string`")
                .WithAnnotations("[Check.IsNonZero]")
                .EndMessage();
        }

        [TestMethod] public void IsNonZero_BooleanField_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(FutharkRune);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`FutharkRune` → InYoungerFuthark")
                .WithProblem("the annotation cannot be applied to a Field of non-numeric type `bool`")
                .WithAnnotations("[Check.IsNonZero]")
                .EndMessage();
        }

        [TestMethod] public void IsNonZero_DateOnlyField_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(TieDye);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`TieDye` → DateCreated")
                .WithProblem("the annotation cannot be applied to a Field of non-numeric type `DateOnly`")
                .WithAnnotations("[Check.IsNonZero]")
                .EndMessage();
        }

        [TestMethod] public void IsNonZero_DateTimeField_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(FinalFour);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`FinalFour` → ChampionshipGame")
                .WithProblem("the annotation cannot be applied to a Field of non-numeric type `DateTime`")
                .WithAnnotations("[Check.IsNonZero]")
                .EndMessage();
        }

        [TestMethod] public void IsNonZero_GuidField_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Fractal);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Fractal` → FractalID")
                .WithProblem("the annotation cannot be applied to a Field of non-numeric type `Guid`")
                .WithAnnotations("[Check.IsNonZero]")
                .EndMessage();
        }

        [TestMethod] public void IsNonZero_EnumerationField_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(IPO);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`IPO` → PostingMethod")
                .WithProblem("the annotation cannot be applied to a Field of non-numeric type `Method`")
                .WithAnnotations("[Check.IsNonZero]")
                .EndMessage();
        }

        [TestMethod] public void IsNonZero_AggregateNestedApplicableScalar() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
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

        [TestMethod] public void IsNonZero_AggregateNestedInapplicableScalar_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(IDE);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`IDE` → Version")
                .WithPath("Released")
                .WithProblem("the annotation cannot be applied to a Field of non-numeric type `DateTime`")
                .WithAnnotations("[Check.IsNonZero]")
                .EndMessage();
        }

        [TestMethod] public void IsNonZero_NestedAggregate_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(PregnancyTest);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`PregnancyTest` → Product")
                .WithPath("Name")
                .WithProblem("the annotation cannot be applied to a property of Aggregate type `MedicalName`")
                .WithAnnotations("[Check.IsNonZero]")
                .EndMessage();
        }

        [TestMethod] public void IsNonZero_ReferenceNestedApplicableScalar() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(EgyptianPyramid);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("Entombed.RegnalNumber", ComparisonOperator.NE, (byte)0).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNonZero_ReferenceNestedInapplicableScalar_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Pajamas);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Pajamas` → Retailer")
                .WithPath("ID")
                .WithProblem("the annotation cannot be applied to a Field of non-numeric type `Guid`")
                .WithAnnotations("[Check.IsNonZero]")
                .EndMessage();
        }

        [TestMethod] public void IsNonZero_PreDefinedInstance_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(ChronicleOfNarnia);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`ChronicleOfNarnia` → TLTWATW")
                .WithProblem("the annotation cannot be applied to a pre-defined instance property")
                .WithAnnotations("[Check.IsNonZero]")
                .EndMessage();
        }

        [TestMethod] public void IsNonZero_OriginalOnReferenceNestedScalar() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(NewsAnchor);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNonZero_NestedReference_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Galaxy);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Galaxy` → Discovery")
                .WithPath("Astronomer")
                .WithProblem("the annotation cannot be applied to a property of Reference type `Person`")
                .WithAnnotations("[Check.IsNonZero]")
                .EndMessage();
        }

        [TestMethod] public void IsNonZero_RelationNestedApplicableScalar() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(CircleDance);

            // Act
            var translation = translator[source];

            // Assert
            translation.Relations[0].Table.Should()
                .HaveConstraint("Key", ComparisonOperator.NE, 0U).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNonZero_RelationNestedInapplicableScalar_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(KosherAgency);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`KosherAgency` → <synthetic> `CertifiedCompanies`")
                .WithPath("Item.ID")
                .WithProblem("the annotation cannot be applied to a Field of non-numeric type `char`")
                .WithAnnotations("[Check.IsNonZero]")
                .EndMessage();
        }

        [TestMethod] public void IsNonZero_NestedRelation_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(CarpoolKaraoke);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`CarpoolKaraoke` → Guest")
                .WithPath("Songs")
                .WithProblem("the annotation cannot be applied to a property of Relation type `RelationList<string>`")
                .WithAnnotations("[Check.IsNonZero]")
                .EndMessage();
        }

        [TestMethod] public void IsNonZero_FieldWithNumericDataConversionTarget() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Airline);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("ConsumerGrade", ComparisonOperator.NE, 0).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNonZero_FieldWithNumericDataConversionSource_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Elevator);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Elevator` → NumFloors")
                .WithProblem("the annotation cannot be applied to a Field of non-numeric type `string`")
                .WithAnnotations("[Check.IsNonZero]")
                .EndMessage();
        }

        [TestMethod] public void IsNonZero_ScalarConstrainedMultipleTimes_Redundant() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Shoe);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("Mens", ComparisonOperator.NE, 0f).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNonZero_PathIsNull_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Igloo);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`Igloo` → NumIceBlocks")
                .WithProblem("the path cannot be 'null'")
                .WithAnnotations("[Check.IsNonZero]")
                .EndMessage();
        }

        [TestMethod] public void IsNonZero_PathOnScalar_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Cryptocurrency);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`Cryptocurrency` → ExchangeRate")
                .WithProblem("the path \"---\" does not exist")
                .WithAnnotations("[Check.IsNonZero]")
                .EndMessage();
        }

        [TestMethod] public void IsNonZero_NonExistentPathOnAggregate_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Encomienda);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`Encomienda` → Location")
                .WithProblem("the path \"---\" does not exist")
                .WithAnnotations("[Check.IsNonZero]")
                .EndMessage();
        }

        [TestMethod] public void IsNonZero_NoPathOnAggregate_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Smoothie);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Smoothie` → Base")
                .WithProblem("the annotation cannot be applied to a property of Aggregate type `SmoothieBase`")
                .WithAnnotations("[Check.IsNonZero]")
                .EndMessage();
        }

        [TestMethod] public void IsNonZero_NonExistentPathOnReference_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Antibiotic);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`Antibiotic` → Formula")
                .WithProblem("the path \"---\" does not exist")
                .WithAnnotations("[Check.IsNonZero]")
                .EndMessage();
        }

        [TestMethod] public void IsNonZero_NonPrimaryKeyPathOnReference_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Chopsticks);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`Chopsticks` → Chopstick2")
                .WithProblem("the path \"Weight\" does not exist")
                .WithAnnotations("[Check.IsNonZero]")
                .EndMessage();
        }

        [TestMethod] public void IsNonZero_NoPathOnReference_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(TongueTwister);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`TongueTwister` → Word11")
                .WithProblem("the annotation cannot be applied to a property of Reference type `Word`")
                .WithAnnotations("[Check.IsNonZero]")
                .EndMessage();
        }

        [TestMethod] public void IsNonZero_NonExistentPathOnRelation_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(HallPass);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`HallPass` → <synthetic> `PermittedLocations`")
                .WithProblem("the path \"---\" does not exist")
                .WithAnnotations("[Check.IsNonZero]")
                .EndMessage();
        }

        [TestMethod] public void IsNonZero_NonAnchorPrimaryKeyPathOnRelation_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Casserole);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`Casserole` → <synthetic> `Ingredients`")
                .WithProblem("the path \"Casserole.IdealPanDepth\" does not exist")
                .WithAnnotations("[Check.IsNonZero]")
                .EndMessage();
        }

        [TestMethod] public void IsNonZero_NoPathOnRelation_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(GarbageTruck);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`GarbageTruck` → <synthetic> `RouteStops`")
                .WithProblem("the annotation cannot be applied to a property of Relation type `RelationSet<string>`")
                .WithAnnotations("[Check.IsNonZero]")
                .EndMessage();
        }

        [TestMethod] public void IsNonZero_DefaultValueDoesNotSatisfyConstraint_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Pulley);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidatedDefaultException>()
                .WithLocation("`Pulley` → RopeTension")
                .WithProblem("the Field's default value of 0.0 does not pass the constraint")
                .WithAnnotations("[Check.IsNonZero]")
                .EndMessage();
        }

        [TestMethod] public void IsNonZero_ValidDefaultValueIsInvalidatedByConstraint_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Ceviche);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidatedDefaultException>()
                .WithLocation("`Ceviche` → CitrusIngredient")
                .WithPath("Tablespoons")
                .WithProblem("the Field's default value of 0.0 does not pass the constraint")
                .WithAnnotations("[Check.IsNonZero]")
                .EndMessage();
        }
    }
}
