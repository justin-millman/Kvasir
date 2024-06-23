using FluentAssertions;
using Kvasir.Schema;
using Kvasir.Translation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

using static UT.Kvasir.Translation.DataConverters;
using static UT.Kvasir.Translation.TestConverters;

namespace UT.Kvasir.Translation {
    [TestClass, TestCategory("DataConverters")]
    public class DataConverterTests {
        [TestMethod] public void NoChangeToFieldsType_Redundant() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Cenote);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField("Name").OfTypeText().BeingNonNullable().And
                .HaveField("MaxDepth").OfTypeSingle().BeingNonNullable().And
                .HaveField("IsKarst").OfTypeBoolean().BeingNonNullable().And
                .HaveField("Latitude").OfTypeDecimal().BeingNonNullable().And
                .HaveField("Longitude").OfTypeDecimal().BeingNonNullable().And
                .HaveNoOtherFields();
        }
        
        [TestMethod] public void ChangFieldsTypeToScalar() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Comet);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField("AstronomicalIdentifier").OfTypeGuid().BeingNonNullable().And
                .HaveField("Aphelion").OfTypeDouble().BeingNonNullable().And
                .HaveField("Perihelion").OfTypeUInt64().BeingNonNullable().And
                .HaveField("Eccentricity").OfTypeUInt64().BeingNonNullable().And
                .HaveField("MassKg").OfTypeUInt64().BeingNonNullable().And
                .HaveField("Albedo").OfTypeDouble().BeingNonNullable().And
                .HaveField("OrbitalPeriod").OfTypeSingle().BeingNonNullable().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void ChangeFieldsTypeToEnumeration() {
            // Arrange
            var translator = new Translator();
            var source = typeof(TitleOfYourSexTape);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField("Title").OfTypeText().BeingNonNullable().And
                .HaveField("CharacterSaying").OfTypeText().BeingNonNullable().And
                .HaveField("CharacterReceiving").OfTypeText().BeingNonNullable().And
                .HaveField("DayOfWeek").OfTypeEnumeration(
                    DayOfWeek.Sunday,
                    DayOfWeek.Monday,
                    DayOfWeek.Tuesday,
                    DayOfWeek.Wednesday,
                    DayOfWeek.Thursday,
                    DayOfWeek.Friday,
                    DayOfWeek.Saturday
                ).BeingNonNullable().And
                .HaveField("Season").OfTypeInt32().BeingNonNullable().And
                .HaveField("EpisodeNumber").OfTypeInt32().BeingNonNullable().And
                .HaveField("Timestamp").OfTypeDouble().BeingNonNullable().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void ChangeFieldsTypeToDifferentEnumeration() {
            // Arrange
            var translator = new Translator();
            var source = typeof(VestigeOfDivergence);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField("Name").OfTypeText().BeingNonNullable().And
                .HaveField("Deity").OfTypeText().BeingNonNullable().And
                .HaveField("Wielder").OfTypeText().BeingNullable().And
                .HaveField("IntroducedIn").OfTypeEnumeration(
                    VestigeOfDivergence.Party.VoxMachina,
                    VestigeOfDivergence.Party.MightyNein,
                    VestigeOfDivergence.Party.BellsHells
                ).BeingNullable().And
                .HaveField("AttackBonus").OfTypeUInt8().BeingNonNullable().And
                .HaveField("AverageDamage").OfTypeInt32().BeingNonNullable().And
                .HaveField("CurrentState").OfTypeEnumeration(
                    VestigeOfDivergence.State.Dormant,
                    VestigeOfDivergence.State.Awakened,
                    VestigeOfDivergence.State.Exalted
                ).BeingNonNullable().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void EnumerationToUnrelatedType() {
            // Arrange
            var translator = new Translator();
            var source = typeof(HomeRunDerby);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField("Year").OfTypeUInt32().BeingNonNullable().And
                .HaveField("Victor").OfTypeText().BeingNonNullable().And
                .HaveField("TotalHomers").OfTypeUInt16().BeingNonNullable().And
                .HaveField("LongestHomer").OfTypeUInt16().BeingNonNullable().And
                .HaveField("CharityMoney").OfTypeDecimal().BeingNonNullable().And
                .HaveField("Structure").OfTypeDateTime().BeingNonNullable().And
                .HaveNoOtherFields().And
                .HaveConstraint("Structure", InclusionOperator.In,
                    new DateTime(2000, 1, 1),
                    new DateTime(2000, 1, 2),
                    new DateTime(2000, 1, 3),
                    new DateTime(2000, 1, 4)
                ).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void BooleanToUnrelatedType() {
            // Arrange
            var translator = new Translator();
            var source = typeof(MathematicalConjecture);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField("Name").OfTypeText().BeingNonNullable().And
                .HaveField("IsMillenniumPrize").OfTypeBoolean().And
                .HaveField("Solved").OfTypeInt32().And
                .HaveField("Equation").OfTypeText().BeingNullable().And
                .HaveField("FirstPosited").OfTypeDateTime().BeingNonNullable().And
                .HaveNoOtherFields().And
                .HaveConstraint("Solved", InclusionOperator.In, 0, 1).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void EnumerationToNumericBackingType() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Quarterback);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField("Name").OfTypeText().BeingNonNullable().And
                .HaveField("ThrowingArm").OfTypeInt8().BeingNonNullable().And
                .HaveField("DraftRound").OfTypeInt16().BeingNonNullable().And
                .HaveField("QBStyle").OfTypeInt32().BeingNonNullable().And
                .HaveField("CareerAchievements").OfTypeInt64().BeingNonNullable().And
                .HaveField("Status").OfTypeUInt8().BeingNonNullable().And
                .HaveField("HasBeenTraded").OfTypeUInt16().BeingNonNullable().And
                .HaveField("FurthestPlayoffAdvancement").OfTypeUInt32().BeingNonNullable().And
                .HaveField("Leagues").OfTypeUInt64().BeingNonNullable().And
                .HaveNoOtherFields().And
                .HaveConstraint("ThrowingArm", InclusionOperator.In,
                    (sbyte)0, (sbyte)1
                ).And
                .HaveConstraint("DraftRound", InclusionOperator.In,
                    (short)14, (short)188, (short)2, (short)-16, (short)19054, (short)-333, (short)0, (short)8
                ).And
                .HaveConstraint("QBStyle", InclusionOperator.In,
                    0, 1, 2, 3
                ).And
                .HaveConstraint("CareerAchievements", InclusionOperator.In,
                    1L, 2L, 3L, 4L, 5L, 6L, 7L, 8L, 9L, 10L, 11L, 12L, 13L, 14L, 15L, 16L, 17L, 18L, 19L, 20L, 21L, 22L,
                    23L, 24L, 25L, 26L, 27L, 28L, 29L, 30L, 31L
                ).And
                .HaveConstraint("Status", InclusionOperator.In,
                    (byte)0, (byte)1, (byte)2, (byte)3, (byte)4
                ).And
                .HaveConstraint("HasBeenTraded", InclusionOperator.In,
                    (ushort)0, (ushort)1
                ).And
                .HaveConstraint("FurthestPlayoffAdvancement", InclusionOperator.In,
                    0U, 1827412U, 44U, 949012U, 55U
                ).And
                .HaveConstraint("Leagues", InclusionOperator.In,
                    2UL, 64UL, 66UL, 128UL, 130UL, 192UL, 194UL
                ).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void EnumerationToString() {
            // Arrange
            var translator = new Translator();
            var source = typeof(EcumenicalCouncil);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField("Name").OfTypeText().BeingNonNullable().And
                .HaveField("Opening").OfTypeDateTime().BeingNonNullable().And
                .HaveField("Closing").OfTypeDateTime().BeingNonNullable().And
                .HaveField("RecognizedBy").OfTypeText().BeingNonNullable().And
                .HaveField("Attendance").OfTypeUInt32().BeingNonNullable().And
                .HaveNoOtherFields().And
                .HaveConstraint("RecognizedBy", InclusionOperator.In,
                    "CatholicChurch", "EasternOrthodoxChurch", "Unrecognized"
                ).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void ConverterOnAggregateField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Joust);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Joust` → KnightB")
                .WithProblem("the annotation cannot be applied to a property of Aggregate type `Person`")
                .WithAnnotations("[DataConverter]")
                .EndMessage();
        }

        [TestMethod] public void ConverterOnReferenceField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Decathlon);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Decathlon` → Winner")
                .WithProblem("the annotation cannot be applied to a property of Reference type `Athlete`")
                .WithAnnotations("[DataConverter]")
                .EndMessage();
        }

        [TestMethod] public void ConverterOnRelationField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Bank);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Bank` → <synthetic> `Accounts`")
                .WithProblem("the annotation cannot be applied to a property of Relation type `RelationMap<ulong, decimal>`")
                .WithAnnotations("[DataConverter]")
                .EndMessage();
        }

        [TestMethod] public void ConverterOnNullablePropertyHasNonNullableTargetType() {
            // Arrange
            var translator = new Translator();
            var source = typeof(RoyalHouse);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField("HouseName").OfTypeText().BeingNonNullable().And
                .HaveField("Founded").OfTypeDateTime().BeingNonNullable().And
                .HaveField("CurrentHead").OfTypeText().BeingNullable().And
                .HaveField("TotalMonarchs").OfTypeInt32().BeingNullable().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void ConverterOnNonNullablePropertyHasNullableTargetType() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Planeswalker);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField("Name").OfTypeText().BeingNonNullable().And
                .HaveField("MannaCost").OfTypeInt8().BeingNonNullable().And
                .HaveField("InitialLoyalty").OfTypeInt8().BeingNonNullable().And
                .HaveField("SetIcon").OfTypeCharacter().BeingNonNullable().And
                .HaveField("Ability1").OfTypeText().BeingNonNullable().And
                .HaveField("Ability2").OfTypeText().BeingNonNullable().And
                .HaveField("Ability3").OfTypeText().BeingNonNullable().And
                .HaveField("SerialNumber").OfTypeGuid().BeingNonNullable().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void PropertyTypeIsInconvertibleToSourceType_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Jedi);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidDataConverterException>()
                .WithLocation("`Jedi` → MiddleName")
                .WithProblem("a property of type `string` cannot use a Data Converter that expects `bool`")
                .WithAnnotations("[DataConverter]")
                .EndMessage();
        }

        [TestMethod] public void PropertyTypeIsConvertibleToSourceType_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(ConstitutionalAmendment);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidDataConverterException>()
                .WithLocation("`ConstitutionalAmendment` → Number")
                .WithProblem("a property of type `int` cannot use a Data Converter that expects `long`")
                .WithAnnotations("[DataConverter]")
                .EndMessage();
        }

        [TestMethod] public void TargetTypeIsNotSupported_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(SNLEpisode);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidDataConverterException>()
                .WithLocation("`SNLEpisode` → AirDate")
                .WithProblem("the result type `Exception` of the Data Converter is not supported")
                .WithAnnotations("[DataConverter]")
                .EndMessage();
        }

        [TestMethod] public void DataConverterTypeThrowsOnConstruction_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Sword);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidDataConverterException>()
                .WithLocation("`Sword` → YearForged")
                .WithProblem($"error constructing `Unconstructible<short>` ({CANNOT_CONSTRUCT_MSG})")
                .WithAnnotations("[DataConverter]")
                .EndMessage();
        }

        [TestMethod] public void DataConverterTypeThrowsOnConversion_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Ligament);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<FailedOperationException>()
                .WithLocation("`Ligament` → Classification")
                .WithProblem($"error converting value Type.Articular ({CANNOT_CONVERT_MSG})")
                .EndMessage();
        }

        [TestMethod] public void NumericConverterOnBooleanField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Pillow);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidDataConverterException>()
                .WithLocation("`Pillow` → IsThrowPillow")
                .WithProblem("the annotation cannot be applied to a property of non-enumeration type `bool`")
                .WithAnnotations("[Numeric]")
                .EndMessage();
        }

        [TestMethod] public void NumericConverterOnTextualField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(VigenereCipher);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidDataConverterException>()
                .WithLocation("`VigenereCipher` → Key")
                .WithProblem("the annotation cannot be applied to a property of non-enumeration type `string`")
                .WithAnnotations("[Numeric]")
                .EndMessage();
        }

        [TestMethod] public void NumericConverterOnNumericField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Satellite);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidDataConverterException>()
                .WithLocation("`Satellite` → OrbitsCompleted")
                .WithProblem("the annotation cannot be applied to a property of non-enumeration type `ulong`")
                .WithAnnotations("[Numeric]")
                .EndMessage();
        }

        [TestMethod] public void NumericConverterOnDateTimeField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Symphony);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidDataConverterException>()
                .WithLocation("`Symphony` → PremiereDate")
                .WithProblem("the annotation cannot be applied to a property of non-enumeration type `DateTime`")
                .WithAnnotations("[Numeric]")
                .EndMessage();
        }

        [TestMethod] public void NumericConverterOnGuidField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(WordSearch);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidDataConverterException>()
                .WithLocation("`WordSearch` → PuzzleID")
                .WithProblem("the annotation cannot be applied to a property of non-enumeration type `Guid`")
                .WithAnnotations("[Numeric]")
                .EndMessage();
        }

        [TestMethod] public void NumericConverterOnAggregateField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(GolfCourse);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidDataConverterException>()
                .WithLocation("`GolfCourse` → Hole14")
                .WithProblem("the annotation cannot be applied to a property of non-enumeration type `Hole`")
                .WithAnnotations("[Numeric]")
                .EndMessage();
        }

        [TestMethod] public void NumericConverterOnReferenceField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(SlamBallMatch);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidDataConverterException>()
                .WithLocation("`SlamBallMatch` → Defeated")
                .WithProblem("the annotation cannot be applied to a property of non-enumeration type `SlamBallTeam`")
                .WithAnnotations("[Numeric]")
                .EndMessage();
        }

        [TestMethod] public void NumericConverterOnRelationField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Wadi);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidDataConverterException>()
                .WithLocation("`Wadi` → <synthetic> `MineralDeposits`")
                .WithProblem("the annotation cannot be applied to a property of non-enumeration type `RelationSet<string>`")
                .WithAnnotations("[Numeric]")
                .EndMessage();
        }

        [TestMethod] public void AsStringConverterOnBooleanField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(BondGirl);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidDataConverterException>()
                .WithLocation("`BondGirl` → SleptWithBond")
                .WithProblem("the annotation cannot be applied to a property of non-enumeration type `bool`")
                .WithAnnotations("[AsString]")
                .EndMessage();
        }

        [TestMethod] public void AsStringConverterOnTextualField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(BatmanVillain);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidDataConverterException>()
                .WithLocation("`BatmanVillain` → Grade")
                .WithProblem("the annotation cannot be applied to a property of non-enumeration type `char`")
                .WithAnnotations("[AsString]")
                .EndMessage();
        }

        [TestMethod] public void AsStringConverterOnNumericField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Cemetery);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidDataConverterException>()
                .WithLocation("`Cemetery` → Latitude")
                .WithProblem("the annotation cannot be applied to a property of non-enumeration type `double`")
                .WithAnnotations("[AsString]")
                .EndMessage();
        }

        [TestMethod] public void AsStringConverterOnDateTimeField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(ImmaculateGrid);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidDataConverterException>()
                .WithLocation("`ImmaculateGrid` → Date")
                .WithProblem("the annotation cannot be applied to a property of non-enumeration type `DateTime`")
                .WithAnnotations("[AsString]")
                .EndMessage();
        }

        [TestMethod] public void AsStringConverterOnGuidField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Eyeglasses);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidDataConverterException>()
                .WithLocation("`Eyeglasses` → GlassesID")
                .WithProblem("the annotation cannot be applied to a property of non-enumeration type `Guid`")
                .WithAnnotations("[AsString]")
                .EndMessage();
        }

        [TestMethod] public void AsStringConverterOnAggregateField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Windmill);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidDataConverterException>()
                .WithLocation("`Windmill` → EnergyGenerated")
                .WithProblem("the annotation cannot be applied to a property of non-enumeration type `EnergyOutput`")
                .WithAnnotations("[AsString]")
                .EndMessage();
        }

        [TestMethod] public void AsStringConverterOnReferenceField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Chakra);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidDataConverterException>()
                .WithLocation("`Chakra` → AssociatedYogini")
                .WithProblem("the annotation cannot be applied to a property of non-enumeration type `Yogini`")
                .WithAnnotations("[AsString]")
                .EndMessage();
        }

        [TestMethod] public void AsStringConverterOnRelationField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Cryptogram);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidDataConverterException>()
                .WithLocation("`Cryptogram` → <synthetic> `Solution`")
                .WithProblem("the annotation cannot be applied to a property of non-enumeration type `RelationMap<char, char>`")
                .WithAnnotations("[AsString]")
                .EndMessage();
        }

        [TestMethod] public void DataConverterAndNumeric_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(SecretHitlerGame);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<ConflictingAnnotationsException>()
                .WithLocation("`SecretHitlerGame` → Player7")
                .WithProblem("the two annotations are mutually exclusive")
                .WithAnnotations("[DataConverter]", "[Numeric]")
                .EndMessage();
        }

        [TestMethod] public void DataConverterAndAsString_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Mezuzah);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<ConflictingAnnotationsException>()
                .WithLocation("`Mezuzah` → MadeOf")
                .WithProblem("the two annotations are mutually exclusive")
                .WithAnnotations("[DataConverter]", "[AsString]")
                .EndMessage();
        }

        [TestMethod] public void NumericAndAsString_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Atoll);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<ConflictingAnnotationsException>()
                .WithLocation("`Atoll` → Ocean")
                .WithProblem("the two annotations are mutually exclusive")
                .WithAnnotations("[AsString]", "[Numeric]")
                .EndMessage();
        }
    }
}
