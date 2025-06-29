using FluentAssertions;
using Kvasir.Schema;
using Kvasir.Translation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using static UT.Kvasir.Translation.Globals;
using static UT.Kvasir.Translation.StringLengthConstraints.IsNonEmpty;
using static UT.Kvasir.Translation.StringLengthConstraints.LengthIsAtLeast;
using static UT.Kvasir.Translation.StringLengthConstraints.LengthIsAtMost;
using static UT.Kvasir.Translation.StringLengthConstraints.LengthIsBetween;

namespace UT.Kvasir.Translation {
    [TestClass, TestCategory("Constraints - String Length")]
    public class IsNonEmptyTests {
        [TestMethod] public void IsNonEmpty_NonNullableStringField() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Chocolate);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(FieldFunction.LengthOf, "Name", ComparisonOperator.GTE, 1).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNonEmpty_NullableStringField() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Scholarship);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(FieldFunction.LengthOf, "Organization", ComparisonOperator.GTE, 1).And
                .HaveConstraint(FieldFunction.LengthOf, "TargetSchool", ComparisonOperator.GTE, 1).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNonEmpty_NumericField_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Biography);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Biography` → PageCount")
                .WithProblem("the annotation cannot be applied to a Field of non-string type `ushort`")
                .WithAnnotations("[Check.IsNonEmpty]")
                .EndMessage();
        }

        [TestMethod] public void IsNonEmpty_CharacterField_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(MovieTicket);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`MovieTicket` → Row")
                .WithProblem("the annotation cannot be applied to a Field of non-string type `char`")
                .WithAnnotations("[Check.IsNonEmpty]")
                .EndMessage();
        }

        [TestMethod] public void IsNonEmpty_BooleanField_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(FortuneCookie);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`FortuneCookie` → Eaten")
                .WithProblem("the annotation cannot be applied to a Field of non-string type `bool`")
                .WithAnnotations("[Check.IsNonEmpty]")
                .EndMessage();
        }

        [TestMethod] public void IsNonEmpty_DateOnlyField_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Fudge);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Fudge` → Baked")
                .WithProblem("the annotation cannot be applied to a Field of non-string type `DateOnly`")
                .WithAnnotations("[Check.IsNonEmpty]")
                .EndMessage();
        }

        [TestMethod] public void IsNonEmpty_DateTimeField_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(ScubaDive);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`ScubaDive` → EntryTime")
                .WithProblem("the annotation cannot be applied to a Field of non-string type `DateTime`")
                .WithAnnotations("[Check.IsNonEmpty]")
                .EndMessage();
        }

        [TestMethod] public void IsNonEmpty_GuidField_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Hormone);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Hormone` → HormoneID")
                .WithProblem("the annotation cannot be applied to a Field of non-string type `Guid`")
                .WithAnnotations("[Check.IsNonEmpty]")
                .EndMessage();
        }

        [TestMethod] public void IsNonEmpty_EnumerationField_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Moustache);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Moustache` → Style")
                .WithProblem("the annotation cannot be applied to a Field of non-string type `Kind`")
                .WithAnnotations("[Check.IsNonEmpty]")
                .EndMessage();
        }

        [TestMethod] public void IsNonEmpty_AggregateNestedApplicableScalar() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(BarGraph);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(FieldFunction.LengthOf, "Legend.XAxisLabel", ComparisonOperator.GTE, 1).And
                .HaveConstraint(FieldFunction.LengthOf, "Legend.YAxisLabel", ComparisonOperator.GTE, 1).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNonEmpty_AggregateNestedInapplicableScalar_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(BackyardBaseballPlayer);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`BackyardBaseballPlayer` → Statistics")
                .WithPath("Pitching")
                .WithProblem("the annotation cannot be applied to a Field of non-string type `byte`")
                .WithAnnotations("[Check.IsNonEmpty]")
                .EndMessage();
        }

        [TestMethod] public void IsNonEmpty_NestedAggregate_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(OilField);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`OilField` → Where")
                .WithPath("Place.Coordinate")
                .WithProblem("the annotation cannot be applied to a property of Aggregate type `Coordinate`")
                .WithAnnotations("[Check.IsNonEmpty]")
                .EndMessage();
        }

        [TestMethod] public void IsNonEmpty_ReferenceNestedApplicableScalar() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(VacuumCleaner);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(FieldFunction.LengthOf, "Manufacturer.Name", ComparisonOperator.GTE, 1).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNonEmpty_PreDefinedInstance_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(IronChef);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`IronChef` → Guarnaschelli")
                .WithProblem("the annotation cannot be applied to a pre-defined instance property")
                .WithAnnotations("[Check.IsNonEmpty]")
                .EndMessage();
        }

        [TestMethod] public void IsNonEmpty_ReferenceNestedInapplicableScalar_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Limerick);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Limerick` → Author")
                .WithPath("SSN")
                .WithProblem("the annotation cannot be applied to a Field of non-string type `uint`")
                .WithAnnotations("[Check.IsNonEmpty]")
                .EndMessage();
        }

        [TestMethod] public void IsNonEmpty_OriginalOnReferenceNestedScalar() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(PornStar);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNonEmpty_NestedReference_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(RomanBaths);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`RomanBaths` → Rooms")
                .WithPath("Caldarium")
                .WithProblem("the annotation cannot be applied to a property of Reference type `Bathroom`")
                .WithAnnotations("[Check.IsNonEmpty]")
                .EndMessage();
        }

        [TestMethod] public void IsNonEmpty_RelationNestedApplicableScalar() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Boycott);

            // Act
            var translation = translator[source];

            // Assert
            translation.Relations[0].Table.Should()
                .HaveConstraint(FieldFunction.LengthOf, "Item", ComparisonOperator.GTE, 1).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNonEmpty_RelationNestedInapplicableScalar_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(MallSanta);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`MallSanta` → <synthetic> `Jobs`")
                .WithPath("Value.MallID")
                .WithProblem("the annotation cannot be applied to a Field of non-string type `uint`")
                .WithAnnotations("[Check.IsNonEmpty]")
                .EndMessage();
        }

        [TestMethod] public void IsNonEmpty_NestedRelation_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(ConnectingWall);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`ConnectingWall` → C3")
                .WithPath("Squares")
                .WithProblem("the annotation cannot be applied to a property of Relation type `RelationList<string>`")
                .WithAnnotations("[Check.IsNonEmpty]")
                .EndMessage();
        }

        [TestMethod] public void IsNonEmpty_FieldWithStringDataConversionTarget() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Hourglass);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(FieldFunction.LengthOf, "Duration", ComparisonOperator.GTE, 1).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNonEmpty_FieldWithStringDataConversionSource_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(FoodChain);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`FoodChain` → SecondaryConsumer")
                .WithProblem("the annotation cannot be applied to a Field of non-string type `int`")
                .WithAnnotations("[Check.IsNonEmpty]")
                .EndMessage();
        }

        [TestMethod] public void IsNonEmpty_ScalarConstrainedMultipleTimes_Redundant() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Top10List);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(FieldFunction.LengthOf, "Number9", ComparisonOperator.GTE, 1).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNonEmpty_PathIsNull_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Hoedown);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`Hoedown` → WayneBradyLine")
                .WithProblem("the path cannot be 'null'")
                .WithAnnotations("[Check.IsNonEmpty]")
                .EndMessage();
        }

        [TestMethod] public void IsNonEmpty_PathOnScalar_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(ASLSign);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`ASLSign` → Gloss")
                .WithProblem("the path \"---\" does not exist")
                .WithAnnotations("[Check.IsNonEmpty]")
                .EndMessage();
        }

        [TestMethod] public void IsNonEmpty_NonExistentPathOnAggregate_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Sutra);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`Sutra` → Source")
                .WithProblem("the path \"---\" does not exist")
                .WithAnnotations("[Check.IsNonEmpty]")
                .EndMessage();
        }

        [TestMethod] public void IsNonEmpty_NoPathOnAggregate_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Kaiju);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Kaiju` → Size")
                .WithProblem("the annotation cannot be applied to a property of Aggregate type `Measurements`")
                .WithAnnotations("[Check.IsNonEmpty]")
                .EndMessage();
        }

        [TestMethod] public void IsNonEmpty_NonExistentPathOnReference_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Peerage);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`Peerage` → PeerageTitle")
                .WithProblem("the path \"---\" does not exist")
                .WithAnnotations("[Check.IsNonEmpty]")
                .EndMessage();
        }

        [TestMethod] public void IsNonEmpty_NonPrimaryKeyPathOnReference_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(BountyHunter);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`BountyHunter` → Credentials")
                .WithProblem("the path \"IssuingAgency\" does not exist")
                .WithAnnotations("[Check.IsNonEmpty]")
                .EndMessage();
        }

        [TestMethod] public void IsNonEmpty_NoPathOnReference_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Linker);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Linker` → TargetLanguage")
                .WithProblem("the annotation cannot be applied to a property of Reference type `Language`")
                .WithAnnotations("[Check.IsNonEmpty]")
                .EndMessage();
        }

        [TestMethod] public void IsNonEmpty_NonExistentPathOnRelation_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Nymph);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`Nymph` → <synthetic> `MetamorphosesAppearances`")
                .WithProblem("the path \"---\" does not exist")
                .WithAnnotations("[Check.IsNonEmpty]")
                .EndMessage();
        }

        [TestMethod] public void IsNonEmpty_NonAnchorPrimaryKeyPathOnRelation_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(DatingApp);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`DatingApp` → <synthetic> `CouplesFormed`")
                .WithProblem("the path \"DatingApp.CEO\" does not exist")
                .WithAnnotations("[Check.IsNonEmpty]")
                .EndMessage();
        }

        [TestMethod] public void IsNonEmpty_NoPathOnRelation_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(AdBlocker);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`AdBlocker` → <synthetic> `EffectiveAgainst`")
                .WithProblem("the annotation cannot be applied to a property of Relation type `RelationSet<AdType>`")
                .WithAnnotations("[Check.IsNonEmpty]")
                .EndMessage();
        }

        [TestMethod] public void IsNonEmpty_DefaultValueDoesNotSatisfyConstraint_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(AztecGod);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidatedDefaultException>()
                .WithLocation("`AztecGod` → Festival")
                .WithProblem("the Field's default value of \"\" does not pass the constraint")
                .WithAnnotations("[Check.IsNonEmpty]")
                .EndMessage();
        }

        [TestMethod] public void LengthIsNonEmpty_ValidDefaultValueIsInvalidatedByConstraint_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Lollipop);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidatedDefaultException>()
                .WithLocation("`Lollipop` → LollipopFlavor")
                .WithPath("Name")
                .WithProblem("the Field's default value of \"\" does not pass the constraint")
                .WithAnnotations("[Check.IsNonEmpty]")
                .EndMessage();
        }
    }

    [TestClass, TestCategory("Constraints - String Length")]
    public class LengthIsAtLeastTests {
        [TestMethod] public void LengthIsAtLeast_NonNullableStringField() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(NFLPenalty);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(FieldFunction.LengthOf, "Penalty", ComparisonOperator.GTE, 5).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void LengthIsAtLeast_NullableStringField() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Ben10Alien);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(FieldFunction.LengthOf, "AlternateName", ComparisonOperator.GTE, 7).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void LengthIsAtLeast_NumericField_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(HashFunction);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`HashFunction` → BlockSize")
                .WithProblem("the annotation cannot be applied to a Field of non-string type `ushort`")
                .WithAnnotations("[Check.LengthIsAtLeast]")
                .EndMessage();
        }

        [TestMethod] public void LengthIsAtLeast_CharacterField_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Kanji);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Kanji` → Logograph")
                .WithProblem("the annotation cannot be applied to a Field of non-string type `char`")
                .WithAnnotations("[Check.LengthIsAtLeast]")
                .EndMessage();
        }

        [TestMethod] public void LengthIsAtLeast_BooleanField_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Magazine);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Magazine` → Syndicated")
                .WithProblem("the annotation cannot be applied to a Field of non-string type `bool`")
                .WithAnnotations("[Check.LengthIsAtLeast]")
                .EndMessage();
        }

        [TestMethod] public void LengthIsAtLeast_DateOnlyField_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Skateboard);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Skateboard` → ManufactureDate")
                .WithProblem("the annotation cannot be applied to a Field of non-string type `DateOnly`")
                .WithAnnotations("[Check.LengthIsAtLeast]")
                .EndMessage();
        }

        [TestMethod] public void LengthIsAtLeast_DateTimeField_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Camerlengo);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Camerlengo` → Appointed")
                .WithProblem("the annotation cannot be applied to a Field of non-string type `DateTime`")
                .WithAnnotations("[Check.LengthIsAtLeast]")
                .EndMessage();
        }

        [TestMethod] public void LengthIsAtLeast_GuidField_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Rainforest);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Rainforest` → ID")
                .WithProblem("the annotation cannot be applied to a Field of non-string type `Guid`")
                .WithAnnotations("[Check.LengthIsAtLeast]")
                .EndMessage();
        }

        [TestMethod] public void LengthIsAtLeast_EnumerationField_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Cybersite);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Cybersite` → FirstSeasonAppeared")
                .WithProblem("the annotation cannot be applied to a Field of non-string type `Season`")
                .WithAnnotations("[Check.LengthIsAtLeast]")
                .EndMessage();
        }

        [TestMethod] public void LengthIsAtLeast_AggregateNestedApplicableScalar() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Dubbing);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(FieldFunction.LengthOf, "Dubber.FirstName", ComparisonOperator.GTE, 6).And
                .HaveConstraint(FieldFunction.LengthOf, "Dubber.LastName", ComparisonOperator.GTE, 2).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void LengthIsAtLeast_AggregateNestedInapplicableScalar_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(BaseballMogul);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`BaseballMogul` → Version")
                .WithPath("Patch")
                .WithProblem("the annotation cannot be applied to a Field of non-string type `ushort`")
                .WithAnnotations("[Check.LengthIsAtLeast]")
                .EndMessage();
        }

        [TestMethod] public void LengthIsAtLeast_NestedAggregate_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(MagicSystem);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`MagicSystem` → SandersonsLaws")
                .WithPath("Zeroth")
                .WithProblem("the annotation cannot be applied to a property of Aggregate type `Law`")
                .WithAnnotations("[Check.LengthIsAtLeast]")
                .EndMessage();
        }

        [TestMethod] public void LengthIsAtLeast_ReferenceNestedApplicableScalar() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(TEDTalk);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(FieldFunction.LengthOf, "Speaker.LastName", ComparisonOperator.GTE, 14).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void LengthIsAtLeast_ReferenceNestedInapplicableScalar_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Arrondissement);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Arrondissement` → Department")
                .WithPath("Population")
                .WithProblem("the annotation cannot be applied to a Field of non-string type `ulong`")
                .WithAnnotations("[Check.LengthIsAtLeast]")
                .EndMessage();
        }

        [TestMethod] public void LengthIsAtLeast_PreDefinedInstance_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(LEPBranch);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`LEPBranch` → LEPRecon")
                .WithProblem("the annotation cannot be applied to a pre-defined instance property")
                .WithAnnotations("[Check.LengthIsAtLeast]")
                .EndMessage();
        }

        [TestMethod] public void LengthIsAtLeast_OriginalOnReferenceNestedScalar() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Circus);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void LengthIsAtLeast_NestedReference_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Constellation);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Constellation` → MainAsterism")
                .WithPath("CentralStar")
                .WithProblem("the annotation cannot be applied to a property of Reference type `Star`")
                .WithAnnotations("[Check.LengthIsAtLeast]")
                .EndMessage();
        }

        [TestMethod] public void LengthIsAtLeast_RelationNestedApplicableScalar() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(UNSecretaryGeneral);

            // Act
            var translation = translator[source];

            // Assert
            translation.Relations[0].Table.Should()
                .HaveConstraint(FieldFunction.LengthOf, "UNSecretaryGeneral.Name", ComparisonOperator.GTE, 17).And
                .HaveNoOtherConstraints();
            translation.Relations[1].Table.Should()
                .HaveConstraint(FieldFunction.LengthOf, "Item.Title", ComparisonOperator.GTE, 6).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void LengthIsAtLeast_RelationNestedInapplicableScalar_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(MemoryBuffer);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`MemoryBuffer` → <synthetic> `Bits`")
                .WithPath("MemoryBuffer.EndAddress")
                .WithProblem("the annotation cannot be applied to a Field of non-string type `ulong`")
                .WithAnnotations("[Check.LengthIsAtLeast]")
                .EndMessage();
        }

        [TestMethod] public void LengthIsAtLeast_NestedRelation_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(HotTub);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`HotTub` → TubSettings")
                .WithPath("PresetSpeeds")
                .WithProblem("the annotation cannot be applied to a property of Relation type `RelationMap<string, int>`")
                .WithAnnotations("[Check.LengthIsAtLeast]")
                .EndMessage();
        }

        [TestMethod] public void LengthIsAtLeast_FieldWithStringDataConversionTarget() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Ambassador);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(FieldFunction.LengthOf, "Assumed", ComparisonOperator.GTE, 10).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void LengthIsAtLeast_FieldWithStringDataConversionSource_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Campfire);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Campfire` → WoodType")
                .WithProblem("the annotation cannot be applied to a Field of non-string type `int`")
                .WithAnnotations("[Check.LengthIsAtLeast]")
                .EndMessage();
        }

        [TestMethod] public void LengthIsAtLeast_AnchorIsZero_Redundant() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(HolyRomanEmperor);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(FieldFunction.LengthOf, "Name", ComparisonOperator.GTE, 0).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void LengthIsAtLeast_AnchorIsNegative_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(LaborOfHeracles);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<UnsatisfiableConstraintException>()
                .WithLocation("`LaborOfHeracles` → Target")
                .WithProblem("the minimum string length (-144) cannot be negative")
                .WithAnnotations("[Check.LengthIsAtLeast")
                .EndMessage();
        }

        [TestMethod] public void LengthIsAtLeast_ScalarConstrainedMultipleTimes() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Bagel);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(FieldFunction.LengthOf, "Flavor", ComparisonOperator.GTE, 34).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void LengthIsAtLeast_PathIsNull_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Localization);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`Localization` → LocalizedValue")
                .WithProblem("the path cannot be 'null'")
                .WithAnnotations("[Check.LengthIsAtLeast]")
                .EndMessage();
        }

        [TestMethod] public void LengthIsAtLeast_PathOnScalar_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Histogram);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`Histogram` → BucketUnit")
                .WithProblem("the path \"---\" does not exist")
                .WithAnnotations("[Check.LengthIsAtLeast]")
                .EndMessage();
        }

        [TestMethod] public void LengthIsAtLeast_NonExistentPathOnAggregate_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Cactus);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`Cactus` → ScientificName")
                .WithProblem("the path \"---\" does not exist")
                .WithAnnotations("[Check.LengthIsAtLeast]")
                .EndMessage();
        }

        [TestMethod] public void LengthIsAtLeast_NoPathOnAggregate_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(SederPlate);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`SederPlate` → Karpas")
                .WithProblem("the annotation cannot be applied to a property of Aggregate type `Slot`")
                .WithAnnotations("[Check.LengthIsAtLeast]")
                .EndMessage();
        }

        [TestMethod] public void LengthIsAtLeast_NonExistentPathOnReference_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Crusade);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`Crusade` → MuslimLeader")
                .WithProblem("the path \"---\" does not exist")
                .WithAnnotations("[Check.LengthIsAtLeast]")
                .EndMessage();
        }

        [TestMethod] public void LengthIsAtLeast_NonPrimaryKeyPathOnReference_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(StateOfTheUnion);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`StateOfTheUnion` → DesignatedSurvivor")
                .WithProblem("the path \"Department\" does not exist")
                .WithAnnotations("[Check.LengthIsAtLeast]")
                .EndMessage();
        }

        [TestMethod] public void LengthIsAtLeast_NoPathOnReference_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Triptych);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Triptych` → MiddlePanel")
                .WithProblem("the annotation cannot be applied to a property of Reference type `Panel`")
                .WithAnnotations("[Check.LengthIsAtLeast]")
                .EndMessage();
        }

        [TestMethod] public void LengthIsAtLeast_NonExistentPathOnRelation_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Cigar);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`Cigar` → <synthetic> `Contents`")
                .WithProblem("the path \"---\" does not exist")
                .WithAnnotations("[Check.LengthIsAtLeast]")
                .EndMessage();
        }

        [TestMethod] public void LengthIsAtLeast_NonAnchorPrimaryKeyPathOnRelation_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(MarijuanaStrain);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`MarijuanaStrain` → <synthetic> `SoldAt`")
                .WithProblem("the path \"MarijuanaStrain.StrainName\" does not exist")
                .WithAnnotations("[Check.LengthIsAtLeast]")
                .EndMessage();
        }

        [TestMethod] public void LengthIsAtLeast_NoPathOnRelation_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(BankRobber);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`BankRobber` → <synthetic> `Robberies`")
                .WithProblem("the annotation cannot be applied to a property of Relation type `RelationList<Robbery>`")
                .WithAnnotations("[Check.LengthIsAtLeast]")
                .EndMessage();
        }

        [TestMethod] public void LengthIsAtLeast_DefaultValueDoesNotSatisfyConstraint_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(MaskedSinger);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidatedDefaultException>()
                .WithLocation("`MaskedSinger` → Costume")
                .WithProblem("the Field's default value of \"Pelican\" does not pass the constraint")
                .WithAnnotations("[Check.LengthIsAtLeast]")
                .EndMessage();
        }

        [TestMethod] public void LengthIsAtLeast_ValidDefaultValueIsInvalidatedByConstraint_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Briefcase);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidatedDefaultException>()
                .WithLocation("`Briefcase` → Color")
                .WithPath("PantoneName")
                .WithProblem("the Field's default value of \"unknown\" does not pass the constraint")
                .WithAnnotations("[Check.LengthIsAtLeast]")
                .EndMessage();
        }
    }

    [TestClass, TestCategory("Constraints - String Length")]
    public class LengthIsAtMostTests {
        [TestMethod] public void LengthIsAtMost_NonNullableStringField() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Snake);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(FieldFunction.LengthOf, "Genus", ComparisonOperator.LTE, 175).And
                .HaveConstraint(FieldFunction.LengthOf, "Species", ComparisonOperator.LTE, 13512).And
                .HaveConstraint(FieldFunction.LengthOf, "CommonName", ComparisonOperator.LTE, 25).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void LengthIsAtMost_NullableStringField() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(WinterStorm);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(FieldFunction.LengthOf, "Name", ComparisonOperator.LTE, 300).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void LengthIsAtMost_NumericField_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(GasStation);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`GasStation` → DeiselPrice")
                .WithProblem("the annotation cannot be applied to a Field of non-string type `decimal`")
                .WithAnnotations("[Check.LengthIsAtMost]")
                .EndMessage();
        }

        [TestMethod] public void LengthIsAtMost_CharacterField_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(BinaryTest);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`BinaryTest` → False")
                .WithProblem("the annotation cannot be applied to a Field of non-string type `char`")
                .WithAnnotations("[Check.LengthIsAtMost]")
                .EndMessage();
        }

        [TestMethod] public void LengthIsAtMost_BooleanField_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Diamond);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Diamond` → IsBloodDiamond")
                .WithProblem("the annotation cannot be applied to a Field of non-string type `bool`")
                .WithAnnotations("[Check.LengthIsAtMost]")
                .EndMessage();
        }

        [TestMethod] public void LengthIsAtMost_DateOnlyField_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(MontyPythonSkit);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`MontyPythonSkit` → OriginalAirdate")
                .WithProblem("the annotation cannot be applied to a Field of non-string type `DateOnly`")
                .WithAnnotations("[Check.LengthIsAtMost]")
                .EndMessage();
        }

        [TestMethod] public void LengthIsAtMost_DateTimeField_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Marathon);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Marathon` → Date")
                .WithProblem("the annotation cannot be applied to a Field of non-string type `DateTime`")
                .WithAnnotations("[Check.LengthIsAtMost]")
                .EndMessage();
        }

        [TestMethod] public void LengthIsAtMost_GuidField_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(CppHeader);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`CppHeader` → ModuleID")
                .WithProblem("the annotation cannot be applied to a Field of non-string type `Guid`")
                .WithAnnotations("[Check.LengthIsAtMost]")
                .EndMessage();
        }

        [TestMethod] public void LengthIsAtMost_EnumerationField_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(ComputerVirus);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`ComputerVirus` → Classification")
                .WithProblem("the annotation cannot be applied to a Field of non-string type `Type`")
                .WithAnnotations("[Check.LengthIsAtMost]")
                .EndMessage();
        }

        [TestMethod] public void LengthIsAtMost_AggregateNestedApplicableScalar() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(MafiaFamily);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(FieldFunction.LengthOf, "Capo.FirstName", ComparisonOperator.LTE, 26).And
                .HaveConstraint(FieldFunction.LengthOf, "Capo.MiddleName", ComparisonOperator.LTE, 71).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void LengthIsAtMost_AggregateNestedInapplicableScalar_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Kayak);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Kayak` → KayakSeat")
                .WithPath("Radius")
                .WithProblem("the annotation cannot be applied to a Field of non-string type `double`")
                .WithAnnotations("[Check.LengthIsAtMost]")
                .EndMessage();
        }

        [TestMethod] public void LengthIsAtMost_NestedAggregate_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(MaddenNFL);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`MaddenNFL` → CoverPlayer")
                .WithPath("Name")
                .WithProblem("the annotation cannot be applied to a property of Aggregate type `Person`")
                .WithAnnotations("[Check.LengthIsAtMost]")
                .EndMessage();
        }

        [TestMethod] public void LengthIsAtMost_ReferenceNestedApplicableScalar() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Denarian);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(FieldFunction.LengthOf, "Fallen.Name", ComparisonOperator.LTE, 673).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void LengthIsAtMost_ReferenceNestedInapplicableScalar_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(IceCreamSundae);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`IceCreamSundae` → Scoop3")
                .WithPath("ID")
                .WithProblem("the annotation cannot be applied to a Field of non-string type `Guid`")
                .WithAnnotations("[Check.LengthIsAtMost]")
                .EndMessage();
        }

        [TestMethod] public void LengthIsAtMost_PreDefinedInstance_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(TonyAward);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`TonyAward` → MusicalRevival")
                .WithProblem("the annotation cannot be applied to a pre-defined instance property")
                .WithAnnotations("[Check.LengthIsAtMost]")
                .EndMessage();
        }

        [TestMethod] public void LengthIsAtMost_OriginalOnReferenceNestedScalar() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Bust);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void LengthIsAtMost_NestedReference_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Orgasm);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Orgasm` → Receiver")
                .WithPath("Who")
                .WithProblem("the annotation cannot be applied to a property of Reference type `Person`")
                .WithAnnotations("[Check.LengthIsAtMost]")
                .EndMessage();
        }

        [TestMethod] public void LengthIsAtMost_RelationNestedApplicableScalar() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Golem);

            // Act
            var translation = translator[source];

            // Assert
            translation.Relations[0].Table.Should()
                .HaveConstraint(FieldFunction.LengthOf, "Item", ComparisonOperator.LTE, 26).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void LengthIsAtMost_RelationNestedInapplicableScalar_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(BalsamicVinegar);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`BalsamicVinegar` → <synthetic> `Ingredients`")
                .WithPath("Item.Grams")
                .WithProblem("the annotation cannot be applied to a Field of non-string type `double`")
                .WithAnnotations("[Check.LengthIsAtMost]")
                .EndMessage();
        }

        [TestMethod] public void LengthIsAtMost_NestedRelation_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(TerroristOrganization);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`TerroristOrganization` → Recognition")
                .WithPath("Entities")
                .WithProblem("the annotation cannot be applied to a property of Relation type `RelationMap<string, DateTime>`")
                .WithAnnotations("[Check.LengthIsAtMost]")
                .EndMessage();
        }

        [TestMethod] public void LengthIsAtMost_FieldWithStringDataConversionTarget() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(OilSpill);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(FieldFunction.LengthOf, "Volume", ComparisonOperator.LTE, 14).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void LengthIsAtMost_FieldWithStringDataConversionSource_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(RandomNumberGenerator);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`RandomNumberGenerator` → Algorithm")
                .WithProblem("the annotation cannot be applied to a Field of non-string type `int`")
                .WithAnnotations("[Check.LengthIsAtMost]")
                .EndMessage();
        }

        [TestMethod] public void LengthIsAtMost_AnchorIsZero() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(KnockKnockJoke);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(FieldFunction.LengthOf, "SetUp", ComparisonOperator.LTE, 0).And
                .HaveConstraint(FieldFunction.LengthOf, "PunchLine", ComparisonOperator.LTE, 0).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void LengthIsAtMost_AnchorIsNegative_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Fraternity);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<UnsatisfiableConstraintException>()
                .WithLocation("`Fraternity` → Name")
                .WithProblem("the maximum string length (-7) cannot be negative")
                .WithAnnotations("[Check.LengthIsAtMost]")
                .EndMessage();
        }

        [TestMethod] public void LengthIsAtMost_ScalarConstrainedMultipleTimes() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(OceanicTrench);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(FieldFunction.LengthOf, "Location", ComparisonOperator.LTE, 60).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void LengthIsAtMost_PathIsNull_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Passport);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`Passport` → PassportNumber")
                .WithProblem("the path cannot be 'null'")
                .WithAnnotations("[Check.LengthIsAtMost]")
                .EndMessage();
        }

        [TestMethod] public void LengthIsAtMost_PathOnScalar_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Nebula);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`Nebula` → Name")
                .WithProblem("the path \"---\" does not exist")
                .WithAnnotations("[Check.LengthIsAtMost]")
                .EndMessage();
        }

        [TestMethod] public void LengthIsAtMost_NonExistentPathOnAggregate_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(ImaginaryFriend);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`ImaginaryFriend` → Features")
                .WithProblem("the path \"---\" does not exist")
                .WithAnnotations("[Check.LengthIsAtMost]")
                .EndMessage();
        }

        [TestMethod] public void LengthIsAtMost_NoPathOnAggregate_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Newscast);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Newscast` → Sports")
                .WithProblem("the annotation cannot be applied to a property of Aggregate type `Segment`")
                .WithAnnotations("[Check.LengthIsAtMost]")
                .EndMessage();
        }

        [TestMethod] public void LengthIsAtMost_NonExistentPathOnReference_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(PhaseDiagram);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`PhaseDiagram` → CriticalPoint")
                .WithProblem("the path \"---\" does not exist")
                .WithAnnotations("[Check.LengthIsAtMost]")
                .EndMessage();
        }

        [TestMethod] public void LengthIsAtMost_NonPrimaryKeyPathOnReference_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Sundial);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`Sundial` → CenterLocation")
                .WithProblem("the path \"Identifier\" does not exist")
                .WithAnnotations("[Check.LengthIsAtMost]")
                .EndMessage();
        }

        [TestMethod] public void LengthIsAtMost_NoPathOnReference_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Zoombini);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Zoombini` → LostAt")
                .WithProblem("the annotation cannot be applied to a property of Reference type `Level`")
                .WithAnnotations("[Check.LengthIsAtMost]")
                .EndMessage();
        }

        [TestMethod] public void LengthIsAtMost_NonExistentPathOnRelation_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Antipope);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`Antipope` → <synthetic> `CardinalsCreated`")
                .WithProblem("the path \"---\" does not exist")
                .WithAnnotations("[Check.LengthIsAtMost]")
                .EndMessage();
        }

        [TestMethod] public void LengthIsAtMost_NonAnchorPrimaryKeyPathOnRelation_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Cabaret);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`Cabaret` → <synthetic> `Performers`")
                .WithProblem("the path \"Cabaret.Venue.Name\" does not exist")
                .WithAnnotations("[Check.LengthIsAtMost]")
                .EndMessage();
        }

        [TestMethod] public void LengthIsAtMost_NoPathOnRelation_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(TikTok);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`TikTok` → <synthetic> `Views`")
                .WithProblem("the annotation cannot be applied to a property of Relation type `RelationMap<string, ulong>`")
                .WithAnnotations("[Check.LengthIsAtMost]")
                .EndMessage();
        }

        [TestMethod] public void LengthIsAtMost_DefaultValueDoesNotSatisfyConstraint_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Obi);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidatedDefaultException>()
                .WithLocation("`Obi` → Color")
                .WithProblem("the Field's default value of \"White\" does not pass the constraint")
                .WithAnnotations("[Check.LengthIsAtMost]")
                .EndMessage();
        }

        [TestMethod] public void LengthIsAtMost_ValidDefaultValueIsInvalidatedByConstraint_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Speakeasy);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidatedDefaultException>()
                .WithLocation("`Speakeasy` → Address")
                .WithPath("StreetName")
                .WithProblem("the Field's default value of \"Main First Prime\" does not pass the constraint")
                .WithAnnotations("[Check.LengthIsAtMost]")
                .EndMessage();
        }
    }

    [TestClass, TestCategory("Constraints - String Length")]
    public class LengthIsBetweenTests {
        [TestMethod] public void LengthIsBetween_NonNullableStringField() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Sorority);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(FieldFunction.LengthOf, "Motto", ComparisonOperator.GTE, 4).And
                .HaveConstraint(FieldFunction.LengthOf, "Motto", ComparisonOperator.LTE, 1713).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void LengthIsBetween_NullableStringField() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Telescope);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(FieldFunction.LengthOf, "Name", ComparisonOperator.GTE, 1).And
                .HaveConstraint(FieldFunction.LengthOf, "Name", ComparisonOperator.LTE, int.MaxValue).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void LengthIsBetween_NumericField_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Capacitor);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Capacitor` → Capacitance")
                .WithProblem("the annotation cannot be applied to a Field of non-string type `float`")
                .WithAnnotations("[Check.LengthIsBetween]")
                .EndMessage();
        }

        [TestMethod] public void LengthIsBetween_CharacterField_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Lipstick);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Lipstick` → Quality")
                .WithProblem("the annotation cannot be applied to a Field of non-string type `char`")
                .WithAnnotations("[Check.LengthIsBetween]")
                .EndMessage();
        }

        [TestMethod] public void LengthIsBetween_BooleanField_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Process);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Process` → IsActive")
                .WithProblem("the annotation cannot be applied to a Field of non-string type `bool`")
                .WithAnnotations("[Check.LengthIsBetween]")
                .EndMessage();
        }

        [TestMethod] public void LengthIsBetween_DateOnlyField_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Tumbleweed);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Tumbleweed` → LastViewedOn")
                .WithProblem("the annotation cannot be applied to a Field of non-string type `DateOnly`")
                .WithAnnotations("[Check.LengthIsBetween]")
                .EndMessage();
        }

        [TestMethod] public void LengthIsBetween_DateTimeField_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Mummy);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Mummy` → Discovered")
                .WithProblem("the annotation cannot be applied to a Field of non-string type `DateTime`")
                .WithAnnotations("[Check.LengthIsBetween]")
                .EndMessage();
        }

        [TestMethod] public void LengthIsBetween_GuidField_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(CelticGod);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`CelticGod` → DeityID")
                .WithProblem("the annotation cannot be applied to a Field of non-string type `Guid`")
                .WithAnnotations("[Check.LengthIsBetween]")
                .EndMessage();
        }

        [TestMethod] public void LengthIsBetween_EnumerationField_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Kinesis);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Kinesis` → Kind")
                .WithProblem("the annotation cannot be applied to a Field of non-string type `Group`")
                .WithAnnotations("[Check.LengthIsBetween]")
                .EndMessage();
        }

        [TestMethod] public void LengthIsBetween_AggregateNestedApplicableScalar() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
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

        [TestMethod] public void LengthIsBetween_AggregateNestedInapplicableScalar_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(OvernightCamp);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`OvernightCamp` → Schedule")
                .WithPath("Sessions")
                .WithProblem("the annotation cannot be applied to a Field of non-string type `uint`")
                .WithAnnotations("[Check.LengthIsBetween]")
                .EndMessage();
        }

        [TestMethod] public void LengthIsBetween_NestedAggregate_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Dentist);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Dentist` → Qualification")
                .WithPath("Doctorate")
                .WithProblem("the annotation cannot be applied to a property of Aggregate type `Degree`")
                .WithAnnotations("[Check.LengthIsBetween")
                .EndMessage();
        }

        [TestMethod] public void LengthIsBetween_ReferenceNestedApplicableScalar() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Onomatopoeia);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(FieldFunction.LengthOf, "DictionaryEntry.Dictionary", ComparisonOperator.GTE, 14).And
                .HaveConstraint(FieldFunction.LengthOf, "DictionaryEntry.Dictionary", ComparisonOperator.LTE, 53).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void LengthIsBetween_ReferenceNestedInapplicableScalar_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(TrivialPursuitPie);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`TrivialPursuitPie` → HistoryWedge")
                .WithPath("CardID")
                .WithProblem("the annotation cannot be applied to a Field of non-string type `char`")
                .WithAnnotations("[Check.LengthIsBetween]")
                .EndMessage();
        }

        [TestMethod] public void LengthIsBetween_NestedReference_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(SumoWrestler);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`SumoWrestler` → DOB")
                .WithPath("Month")
                .WithProblem("the annotation cannot be applied to a property of Reference type `Number`")
                .WithAnnotations("[Check.LengthIsBetween]")
                .EndMessage();
        }

        [TestMethod] public void LengthIsBetween_RelationNestedApplicableScalar() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(ComicBook);

            // Act
            var translation = translator[source];

            // Assert
            translation.Relations[0].Table.Should()
                .HaveConstraint(FieldFunction.LengthOf, "Item", ComparisonOperator.GTE, 4).And
                .HaveConstraint(FieldFunction.LengthOf, "Item", ComparisonOperator.LTE, 37).And
                .HaveConstraint(FieldFunction.LengthOf, "ComicBook.Title", ComparisonOperator.GTE, 8).And
                .HaveConstraint(FieldFunction.LengthOf, "ComicBook.Title", ComparisonOperator.LTE, 19).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void LengthIsBetwen_RelationNestedInapplicableScalar_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Wormhole);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Wormhole` → <synthetic> `ConnectedLocations`")
                .WithPath("Item.Z")
                .WithProblem("the annotation cannot be applied to a Field of non-string type `float`")
                .WithAnnotations("[Check.LengthIsBetween]")
                .EndMessage();
        }

        [TestMethod] public void LengthIsBetween_NestedRelation_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(LunarEclipse);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`LunarEclipse` → Visibility")
                .WithPath("Locations")
                .WithProblem("the annotation cannot be applied to a property of Relation type `RelationMap<Coordinate, double>`")
                .WithAnnotations("[Check.LengthIsBetween]")
                .EndMessage();
        }

        [TestMethod] public void LengthIsBetween_PreDefinedInstance_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(HighSchoolMusical);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`HighSchoolMusical` → HSM")
                .WithProblem("the annotation cannot be applied to a pre-defined instance property")
                .WithAnnotations("[Check.LengthIsBetween]")
                .EndMessage();
        }

        [TestMethod] public void LengthIsBetween_OriginalOnReferenceNestedScalar() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Lagoon);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void LengthIsBetween_FieldWithStringDataConversionTarget() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(AesSedai);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(FieldFunction.LengthOf, "Ajah", ComparisonOperator.GTE, 1).And
                .HaveConstraint(FieldFunction.LengthOf, "Ajah", ComparisonOperator.LTE, 15).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void LengthIsBetween_FieldWithStringDataConversionSource_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(AtmosphericLayer);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`AtmosphericLayer` → Name")
                .WithProblem("the annotation cannot be applied to a Field of non-string type `int`")
                .WithAnnotations("[Check.LengthIsBetween]")
                .EndMessage();
        }

        [TestMethod] public void LengthIsBetween_LowerBoundEqualsUpperBound() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(DNACodon);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(FieldFunction.LengthOf, "CodonSequence", ComparisonOperator.EQ, 3).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void LengthIsBetween_LowerBoundGreaterThanUpperBound_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(ChristmasCarol);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<UnsatisfiableConstraintException>()
                .WithLocation("`ChristmasCarol` → FirstVerse")
                .WithProblem("the interval [28841, 1553] of valid string lengths is empty")
                .WithAnnotations("[Check.LengthIsBetween]")
                .EndMessage();
        }

        [TestMethod] public void LengthIsBetween_NegativeLowerBound_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(ShenGongWu);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<UnsatisfiableConstraintException>()
                .WithLocation("`ShenGongWu` → InitialEpisode")
                .WithProblem("the minimum string length (-4) cannot be negative")
                .WithAnnotations("[Check.LengthIsBetween]")
                .EndMessage();
        }

        [TestMethod] public void LengthIsBetween_NegativeLowerAndUpperBounds_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(MilitaryBase);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<UnsatisfiableConstraintException>()
                .WithLocation("`MilitaryBase` → Commander")
                .WithProblem("the minimum string length (-156) cannot be negative")
                .WithAnnotations("[Check.LengthIsBetween]")
                .EndMessage();
        }

        [TestMethod] public void LengthIsBetween_ScalarConstrainedMultipleTimes() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Aria);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint(FieldFunction.LengthOf, "Lyrics", ComparisonOperator.GTE, 27).And
                .HaveConstraint(FieldFunction.LengthOf, "Lyrics", ComparisonOperator.LTE, 100).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void LengthIsBetween_PathIsNull_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Apocalypse);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`Apocalypse` → SourceMaterial")
                .WithProblem("the path cannot be 'null'")
                .WithAnnotations("[Check.LengthIsBetween]")
                .EndMessage();
        }

        [TestMethod] public void LengthIsBetween_PathOnScalar_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(SetCard);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`SetCard` → Pattern")
                .WithProblem("the path \"---\" does not exist")
                .WithAnnotations("[Check.LengthIsBetween]")
                .EndMessage();
        }

        [TestMethod] public void LengthIsBetween_NonExistentPathOnAggregate_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(InternetCraze);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`InternetCraze` → Dangers")
                .WithProblem("the path \"---\" does not exist")
                .WithAnnotations("[Check.LengthIsBetween]")
                .EndMessage();
        }

        [TestMethod] public void LengthIsBetween_NoPathOnAggregate_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(MesopotamianGod);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`MesopotamianGod` → Names")
                .WithProblem("the annotation cannot be applied to a property of Aggregate type `Naming`")
                .WithAnnotations("[Check.LengthIsBetween]")
                .EndMessage();
        }

        [TestMethod] public void LengthIsBetween_NonExistentPathOnReference_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(HeatWave);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`HeatWave` → Low")
                .WithProblem("the path \"---\" does not exist")
                .WithAnnotations("[Check.LengthIsBetween]")
                .EndMessage();
        }

        [TestMethod] public void LengthIsBetween_NonPrimaryKeyPathOnReference_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Sprachbund);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`Sprachbund` → Progenitor")
                .WithProblem("the path \"Endonym\" does not exist")
                .WithAnnotations("[Check.LengthIsBetween]")
                .EndMessage();
        }

        [TestMethod] public void LengthIsBetween_NoPathOnReference_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Leprechaun);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Leprechaun` → Shillelagh")
                .WithProblem("the annotation cannot be applied to a property of Reference type `WalkingStick`")
                .WithAnnotations("[Check.LengthIsBetween]")
                .EndMessage();
        }

        [TestMethod] public void LengthIsBetween_NonExistentPathOnRelation_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(MarsRover);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`MarsRover` → <synthetic> `SpecimensCollected`")
                .WithProblem("the path \"---\" does not exist")
                .WithAnnotations("[Check.LengthIsBetween]")
                .EndMessage();
        }

        [TestMethod] public void LengthIsBetween_NonAnchorPrimaryKeyPathOnRelation_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(BlackOp);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`BlackOp` → <synthetic> `Participants`")
                .WithProblem("the path \"BlackOp.Country\" does not exist")
                .WithAnnotations("[Check.LengthIsBetween]")
                .EndMessage();
        }

        [TestMethod] public void LengthIsBetween_NoPathOnRelation_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(HeartAttack);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`HeartAttack` → <synthetic> `Symptoms`")
                .WithProblem("the annotation cannot be applied to a property of Relation type `RelationSet<string>`")
                .WithAnnotations("[Check.LengthIsBetween]")
                .EndMessage();
        }

        [TestMethod] public void LengthIsBetween_DefaultValueDoesNotSatisfyConstraint_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(PeanutButter);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidatedDefaultException>()
                .WithLocation("`PeanutButter` → Brand")
                .WithProblem("the Field's default value of \"Smucker's\" does not pass the constraint")
                .WithAnnotations("[Check.LengthIsBetween]")
                .EndMessage();
        }

        [TestMethod] public void LengthIsBetween_ValidDefaultValueIsInvalidatedByConstraint_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Kebab);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidatedDefaultException>()
                .WithLocation("`Kebab` → Vendor")
                .WithPath("Name")
                .WithProblem("the Field's default value of \"Ezekiel's Meat-on-a-Stick Emporium\" does not pass the constraint")
                .WithAnnotations("[Check.LengthIsBetween]")
                .EndMessage();
        }
    }
}
