using FluentAssertions;
using Kvasir.Schema;
using Kvasir.Translation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

using static UT.Kvasir.Translation.ComparisonConstraints.IsGreaterThan;
using static UT.Kvasir.Translation.ComparisonConstraints.IsLessThan;
using static UT.Kvasir.Translation.ComparisonConstraints.IsGreaterOrEqualTo;
using static UT.Kvasir.Translation.ComparisonConstraints.IsLessOrEqualTo;
using static UT.Kvasir.Translation.ComparisonConstraints.IsNot;

namespace UT.Kvasir.Translation {
    [TestClass, TestCategory("Constraints - Comparison")]
    public class IsGreaterThanTests {
        [TestMethod] public void IsGreaterThan_NumericFields() {
            // Arrange
            var translator = new Translator();
            var source = typeof(DNDSpell);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("Range", ComparisonOperator.GT, (ushort)0).And
                .HaveConstraint("Level", ComparisonOperator.GT, -1).And
                .HaveConstraint("AverageDamage", ComparisonOperator.GT, 2.5f).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsGreaterThan_TextualFields() {
            // Arrange
            var translator = new Translator();
            var source = typeof(MultipleChoiceQuestion);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("CorrectAnswer", ComparisonOperator.GT, '*').And
                .HaveConstraint("ChoiceA", ComparisonOperator.GT, "A. ").And
                .HaveConstraint("ChoiceB", ComparisonOperator.GT, "B. ").And
                .HaveConstraint("ChoiceC", ComparisonOperator.GT, "C. ").And
                .HaveConstraint("ChoiceD", ComparisonOperator.GT, "D. ").And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsGreaterThan_BooleanField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Font);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Font` → HasSerifs")
                .WithProblem("the annotation cannot be applied to a Field of non-orderable type `bool`")
                .WithAnnotations("[Check.IsGreaterThan]")
                .EndMessage();
        }

        [TestMethod] public void IsGreaterThan_DecimalField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(AuctionLot);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("TopBid", ComparisonOperator.GT, (decimal)10000).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsGreaterThan_DateTimeField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(GoldRush);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("StartDate", ComparisonOperator.GT, new DateTime(1200, 3, 18)).And
                .HaveConstraint("EndDate", ComparisonOperator.GT, new DateTime(1176, 11, 22)).And
                .HaveNoOtherCandidateKeys();
        }

        [TestMethod] public void IsGreaterThan_GuidField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Skyscraper);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Skyscraper` → RegistryIdentifier")
                .WithProblem("the annotation cannot be applied to a Field of non-orderable type `Guid`")
                .WithAnnotations("[Check.IsGreaterThan]")
                .EndMessage();
        }

        [TestMethod] public void IsGreaterThan_EnumerationField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Orisha);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Orisha` → BelongsTo")
                .WithProblem("the annotation cannot be applied to a Field of non-orderable type `Culture`")
                .WithAnnotations("[Check.IsGreaterThan")
                .EndMessage();
        }

        [TestMethod] public void IsGreaterThan_AggregateNestedApplicableScalar() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Opioid);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("Definition.DrugBank", ComparisonOperator.GT, "XP14U339D").And
                .HaveConstraint("Definition.Formula.O", ComparisonOperator.GT, 2).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsGreaterThan_AggregateNestedInapplicableScalar_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Wordle);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Wordle` → Guess3")
                .WithPath("L4.Hint")
                .WithProblem("the annotation cannot be applied to a Field of non-orderable type `Result`")
                .WithAnnotations("[Check.IsGreaterThan]")
                .EndMessage();
        }

        [TestMethod] public void IsGreaterThan_NestedAggregate_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(FlashMob);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`FlashMob` → Participants")
                .WithPath("Leader")
                .WithProblem("the annotation cannot be applied to a property of Aggregate type `Person`")
                .WithAnnotations("[Check.IsGreaterThan]")
                .EndMessage();
        }

        [TestMethod] public void IsGreaterThan_ReferenceNestedApplicableScalar() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Apostle);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("Adherence.Identifier", ComparisonOperator.GT, "Atheism").And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsGreaterThan_ReferenceNestedInapplicableScalar_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Influenza);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Influenza` → DeadliestOutbreak")
                .WithPath("OutbreakID")
                .WithProblem("the annotation cannot be applied to a Field of non-orderable type `Guid`")
                .WithAnnotations("[Check.IsGreaterThan]")
                .EndMessage();
        }

        [TestMethod] public void IsGreaterThan_OriginalOnReferenceNestedScalar() {
            // Arrange
            var translator = new Translator();
            var source = typeof(PapalBull);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsGreaterThan_NestedReference_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(BloodDrive);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`BloodDrive` → SponsoredBy")
                .WithPath("Hospital")
                .WithProblem("the annotation cannot be applied to a property of Reference type `Hospital`")
                .WithAnnotations("[Check.IsGreaterThan]")
                .EndMessage();
        }

        [TestMethod] public void IsGreaterThan_RelationNestedApplicableScalar() {
            // Arrange
            var translator = new Translator();
            var source = typeof(KidNextDoor);

            // Act
            var translation = translator[source];

            // Assert
            translation.Relations[0].Table.Should()
                .HaveConstraint("KidNextDoor.Number", ComparisonOperator.GT, 0L).And
                .HaveConstraint("Key", ComparisonOperator.GT, '@').And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsGreaterThan_RelationNestedInapplicableScalar_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Antari);

            // Act
            var translate = () => translator[source];

            // Act
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Antari` → <synthetic> `KnownSpells`")
                .WithPath("Item.SpellID")
                .WithProblem("the annotation cannot be applied to a Field of non-orderable type `Guid`")
                .WithAnnotations("[Check.IsGreaterThan]")
                .EndMessage();
        }

        [TestMethod] public void IsGreaterThan_NestedRelation_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Clown);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Clown` → Costume")
                .WithPath("Accoutrement")
                .WithProblem("the annotation cannot be applied to a property of Relation type `RelationList<string>`")
                .WithAnnotations("[Check.IsGreaterThan]")
                .EndMessage();
        }

        [TestMethod] public void IsGreaterThan_NullableTotallyOrderedFields() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Baryon);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("Symbol", ComparisonOperator.GT, '-').And
                .HaveConstraint("Charge", ComparisonOperator.GT, (short)-5).And
                .HaveConstraint("Discovered", ComparisonOperator.GT, new DateTime(1344, 6, 21)).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsGreaterThan_InconvertibleAnchor_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Racehorse);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidConstraintValueException>()
                .WithLocation("`Racehorse` → FirstDerbyWin")
                .WithProblem("value true is of type `bool`, not `ulong` as expected")
                .WithAnnotations("[Check.IsGreaterThan]")
                .EndMessage();
        }

        [TestMethod] public void IsGreaterThan_ConvertibleAnchor_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(ChineseCharacter);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidConstraintValueException>()
                .WithLocation("`ChineseCharacter` → Character")
                .WithProblem("value 14 is of type `byte`, not `char` as expected")
                .WithAnnotations("[Check.IsGreaterThan]")
                .EndMessage();
        }

        [TestMethod] public void IsGreaterThan_ArrayAnchor_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Query);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidConstraintValueException>()
                .WithLocation("`Query` → WHERE")
                .WithProblem("value cannot be an array")
                .WithAnnotations("[Check.IsGreaterThan]")
                .EndMessage();
        }

        [TestMethod] public void IsGreaterThan_NullAnchor_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(UNResolution);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidConstraintValueException>()
                .WithLocation("`UNResolution` → NumSignatories")
                .WithProblem("the constraint boundary cannot be 'null'")
                .WithAnnotations("[Check.IsGreaterThan]")
                .EndMessage();
        }

        [TestMethod] public void IsGreaterThan_AnchorIsMaximum_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Upanishad);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidConstraintValueException>()
                .WithLocation("`Upanishad` → Index")
                .WithProblem("the constraint anchor cannot be the maximum possible value")
                .WithAnnotations("[Check.IsGreaterThan]")
                .EndMessage();
        }

        [TestMethod] public void IsGreaterThan_DecimalAnchorIsNotDouble_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(GarageSale);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidConstraintValueException>()
                .WithLocation("`GarageSale` → Gross")
                .WithProblem("value 200 is of type `int`, not `double` as expected")
                .WithAnnotations("[Check.IsGreaterThan]")
                .EndMessage();
        }

        [TestMethod] public void IsGreaterThan_DecimalAnchorIsOutOfRange_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(TalkShow);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidConstraintValueException>()
                .WithLocation("`TalkShow` → Rating")
                .WithProblem($"`double` {double.MinValue} is outside the supported range for `decimal`")
                .WithAnnotations("[Check.IsGreaterThan]")
                .EndMessage();
        }

        [TestMethod] public void IsGreaterThan_DateTimeAnchorIsNotString_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Meme);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidConstraintValueException>()
                .WithLocation("`Meme` → FirstPublished")
                .WithProblem("value 'N' is of type `char`, not `string` as expected")
                .WithAnnotations("[Check.IsGreaterThan]")
                .EndMessage();
        }

        [TestMethod] public void IsGreaterThan_DateTimeAnchorIsMalformatted_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(ChristianDenomination);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidConstraintValueException>()
                .WithLocation("`ChristianDenomination` → Founded")
                .WithProblem($"unable to parse `string` value \"0001_01_01\" as a `DateTime`")
                .WithAnnotations("[Check.IsGreaterThan]")
                .EndMessage();
        }

        [TestMethod] public void IsGreaterThan_DateTimeAnchorIsOutOfRange_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(GraduateThesis);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidConstraintValueException>()
                .WithLocation("`GraduateThesis` → Argued")
                .WithProblem($"unable to parse `string` value \"1873-15-12\" as a `DateTime`")
                .WithAnnotations("[Check.IsGreaterThan]")
                .EndMessage();
        }

        [TestMethod] public void IsGreaterThan_AnchorMatchesDataConversionSourceType_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Azeotrope);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidConstraintValueException>()
                .WithLocation("`Azeotrope` → BoilingPoint")
                .WithProblem("value -237.44 is of type `float`, not `string` as expected")
                .WithAnnotations("[Check.IsGreaterThan]")
                .EndMessage();
        }

        [TestMethod] public void IsGreaterThan_AnchorMatchesDataConversionTargetType() {
            // Arrange
            var translator = new Translator();
            var source = typeof(BingoCard);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("CellR4C1", ComparisonOperator.GT, "-1").And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsGreaterThan_ScalarConstrainedMultipleTimes() {
            // Arrange
            var translator = new Translator();
            var source = typeof(NuclearPowerPlant);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("Meltdowns", ComparisonOperator.GT, 37L).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsGreaterThan_PathIsNull_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Domino);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`Domino` → RightPips")
                .WithProblem("the path cannot be 'null'")
                .WithAnnotations("[Check.IsGreaterThan]")
                .EndMessage();
        }

        [TestMethod] public void IsGreaterThan_PathOnScalar_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Canyon);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`Canyon` → Depth")
                .WithProblem("the path \"---\" does not exist")
                .WithAnnotations("[Check.IsGreaterThan]")
                .EndMessage();
        }

        [TestMethod] public void IsGreaterThan_NonExistentPathOnAggregate_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Conlang);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`Conlang` → Codes")
                .WithProblem("the path \"---\" does not exist")
                .WithAnnotations("[Check.IsGreaterThan]")
                .EndMessage();
        }

        [TestMethod] public void IsGreaterThan_NoPathOnAggregate_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(LaborStrike);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`LaborStrike` → Members")
                .WithProblem("the annotation cannot be applied to a property of Aggregate type `Parties`")
                .WithAnnotations("[Check.IsGreaterThan]")
                .EndMessage();
        }

        [TestMethod] public void IsGreaterThan_NonExistentPathOnReference_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(InstallationWizard);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`InstallationWizard` → Program")
                .WithProblem("the path \"---\" does not exist")
                .WithAnnotations("[Check.IsGreaterThan]")
                .EndMessage();
        }

        [TestMethod] public void IsGreaterThan_NonPrimaryKeyPathOnReference_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(BugSpray);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`BugSpray` → ActiveIngredient")
                .WithProblem("the path \"LethalDose\" does not exist")
                .WithAnnotations("[Check.IsGreaterThan]")
                .EndMessage();
        }

        [TestMethod] public void IsGreaterThan_NoPathOnReference_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Intern);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Intern` → Manager")
                .WithProblem("the annotation cannot be applied to a property of Reference type `Employee`")
                .WithAnnotations("[Check.IsGreaterThan]")
                .EndMessage();
        }

        [TestMethod] public void IsGreaterThan_NonExistentPathOnRelation_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Delicatessen);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`Delicatessen` → <synthetic> `MenuItems`")
                .WithProblem("the path \"---\" does not exist")
                .WithAnnotations("[Check.IsGreaterThan]")
                .EndMessage();
        }

        [TestMethod] public void IsGreaterThan_NonAnchorPrimaryKeyPathOnRelation_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(BlackjackHand);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`BlackjackHand` → <synthetic> `DealerCards`")
                .WithProblem("the path \"BlackjackHand.TotalPot\" does not exist")
                .WithAnnotations("[Check.IsGreaterThan]")
                .EndMessage();
        }

        [TestMethod] public void IsGreaterThan_NoPathOnRelation_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Inquisition);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Inquisition` → <synthetic> `Victims`")
                .WithProblem("the annotation cannot be applied to a property of Relation type `RelationSet<string>`")
                .WithAnnotations("[Check.IsGreaterThan]")
                .EndMessage();
        }

        [TestMethod] public void IsGreaterThan_DefaultValueDoesNotSatisfyConstraint_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(DraftPick);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidatedDefaultException>()
                .WithLocation("`DraftPick` → Overall")
                .WithProblem("the Field's default value of 0 does not pass the constraint")
                .WithAnnotations("[Check.IsGreaterThan]")
                .EndMessage();
        }

        [TestMethod] public void IsGreaterThan_ValidDefaultValueIsInvalidatedByConstraint_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Madrasa);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidatedDefaultException>()
                .WithLocation("`Madrasa` → ID")
                .WithPath("Class")
                .WithProblem("the Field's default value of 's' does not pass the constraint")
                .WithAnnotations("[Check.IsGreaterThan]")
                .EndMessage();
        }
    }

    [TestClass, TestCategory("Constraints - Comparison")]
    public class IsLessThanTests {
        [TestMethod] public void IsLessThan_NumericFields() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Resistor);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("Resistance", ComparisonOperator.LT, 27814L).And
                .HaveConstraint("PhysicalLength", ComparisonOperator.LT, 893.44501f).And
                .HaveConstraint("Power", ComparisonOperator.LT, 27814UL).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsLessThan_TextualFields() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Senator);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("LastName", ComparisonOperator.LT, "...").And
                .HaveConstraint("NRARating", ComparisonOperator.LT, 'G').And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsLessThan_BooleanField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Milkshake);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Milkshake` → IsDairyFree")
                .WithProblem("the annotation cannot be applied to a Field of non-orderable type `bool`")
                .WithAnnotations("[Check.IsLessThan")
                .EndMessage();
        }

        [TestMethod] public void IsLessThan_DecimalField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(TreasuryBond);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("BoughtFor", ComparisonOperator.LT, (decimal)57182391.33167994).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsLessThan_DateTimeField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Commercial);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("TimeSlot", ComparisonOperator.LT, new DateTime(2300, 1, 1)).And
                .HaveNoOtherCandidateKeys();
        }

        [TestMethod] public void IsLessThan_GuidField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(DLL);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`DLL` → ID")
                .WithProblem("the annotation cannot be applied to a Field of non-orderable type `Guid`")
                .WithAnnotations("[Check.IsLessThan")
                .EndMessage();
        }

        [TestMethod] public void IsLessThan_EnumerationField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(SolicitorGeneral);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`SolicitorGeneral` → Affiliation")
                .WithProblem("the annotation cannot be applied to a Field of non-orderable type `PoliticalParty`")
                .WithAnnotations("[Check.IsLessThan")
                .EndMessage();
        }

        [TestMethod] public void IsLessThan_AggregateNestedApplicableScalar() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Raptor);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("Scientific.Family", ComparisonOperator.LT, "Zynovia").And
                .HaveConstraint("Measurements.Weight", ComparisonOperator.LT, 174.991).And
                .HaveConstraint("Measurements.TopSpeed", ComparisonOperator.LT, long.MaxValue).And
                .HaveConstraint("Measurements.Wingspan", ComparisonOperator.LT, (ushort)489).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsLessThan_AggregateNestedInapplicableScalar_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Feruchemy);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Feruchemy` → Effects")
                .WithPath("Kind")
                .WithProblem("the annotation cannot be applied to a Field of non-orderable type `Matrix`")
                .WithAnnotations("[Check.IsLessThan]")
                .EndMessage();
        }

        [TestMethod] public void IsLessThan_NestedAggregate_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Firefighter);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Firefighter` → Firehouse")
                .WithPath("ServiceArea")
                .WithProblem("the annotation cannot be applied to a property of Aggregate type `Polity`")
                .WithAnnotations("[Check.IsLessThan]")
                .EndMessage();
        }

        [TestMethod] public void IsLessThan_ReferenceNestedApplicableScalar() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Butterfly);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("Genus.Name", ComparisonOperator.LT, "Zojemana").And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsLessThan_ReferenceNestedInapplicableScalar_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Cartel);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Cartel` → Control")
                .WithPath("Kind")
                .WithProblem("the annotation cannot be applied to a Field of non-orderable type `CommodityType`")
                .WithAnnotations("[Check.IsLessThan]")
                .EndMessage();
        }

        [TestMethod] public void IsLessThan_OriginalOnReferenceNestedScalar() {
            // Arrange
            var translator = new Translator();
            var source = typeof(CVE);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsLessThan_NestedReference_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Hallucination);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Hallucination` → Reason")
                .WithPath("Drug")
                .WithProblem("the annotation cannot be applied to a property of Reference type `Drug`")
                .WithAnnotations("[Check.IsLessThan]")
                .EndMessage();
        }

        [TestMethod] public void IsLessThan_RelationNestedApplicableScalar() {
            // Arrange
            var translator = new Translator();
            var source = typeof(FairyGodparent);

            // Act
            var translation = translator[source];

            // Assert
            translation.Relations[0].Table.Should()
                .HaveConstraint("FairyGodparent.Name", ComparisonOperator.LT, "Warmonger").And
                .HaveConstraint("Key", ComparisonOperator.LT, (ushort)1851).And
                .HaveConstraint("Value", ComparisonOperator.LT, (ushort)42144).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsLessThan_RelationNestedInapplicableScalar_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(NavalBlockade);

            // Act
            var translate = () => translator[source];

            // Act
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`NavalBlockade` → <synthetic> `WaterwaysAffected`")
                .WithPath("Item.Kind")
                .WithProblem("the annotation cannot be applied to a Field of non-orderable type `AquaKind`")
                .WithAnnotations("[Check.IsLessThan]")
                .EndMessage();
        }

        [TestMethod] public void IsLessThan_NestedRelation_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Blacksmith);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Blacksmith` → Materials")
                .WithPath("Hammers")
                .WithProblem("the annotation cannot be applied to a property of Relation type `RelationSet<string>`")
                .WithAnnotations("[Check.IsLessThan]")
                .EndMessage();
        }

        [TestMethod] public void IsLessThan_NullableTotallyOrderedFields() {
            // Arrange
            var translator = new Translator();
            var source = typeof(AutoRacetrack);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("Nickname", ComparisonOperator.LT, "Zytrotzko").And
                .HaveConstraint("TrackLength", ComparisonOperator.LT, 12000000L).And
                .HaveConstraint("LastRace", ComparisonOperator.LT, new DateTime(4319, 2, 21)).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsLessThan_InconvertibleAnchor_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Distribution);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidConstraintValueException>()
                .WithLocation("`Distribution` → Mode")
                .WithProblem("value \"Zero\" is of type `string`, not `double` as expected")
                .WithAnnotations("[Check.IsLessThan]")
                .EndMessage();
        }

        [TestMethod] public void IsLessThan_ConvertibleAnchor_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(WebBrowser);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidConstraintValueException>()
                .WithLocation("`WebBrowser` → MarketShare")
                .WithProblem("value 100 is of type `int`, not `float` as expected")
                .WithAnnotations("[Check.IsLessThan]")
                .EndMessage();
        }

        [TestMethod] public void IsLessThan_ArrayAnchor_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(GrammaticalCase);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidConstraintValueException>()
                .WithLocation("`GrammaticalCase` → Affix")
                .WithProblem("value cannot be an array")
                .WithAnnotations("[Check.IsLessThan]")
                .EndMessage();
        }

        [TestMethod] public void IsLessThan_NullAnchor_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(PowerPointAnimation);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidConstraintValueException>()
                .WithLocation("`PowerPointAnimation` → Duration")
                .WithProblem("the constraint boundary cannot be 'null'")
                .WithAnnotations("[Check.IsLessThan]")
                .EndMessage();
        }

        [TestMethod] public void IsLessThan_AnchorIsMinimum_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(StrategoPiece);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidConstraintValueException>()
                .WithLocation("`StrategoPiece` → Value")
                .WithProblem("the constraint anchor cannot be the minimum possible value")
                .WithAnnotations("[Check.IsLessThan]")
                .EndMessage();
        }

        [TestMethod] public void IsLessThan_DecimalAnchorIsNotDouble_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Toothpaste);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidConstraintValueException>()
                .WithLocation("`Toothpaste` → Efficacy")
                .WithProblem("value \"100%\" is of type `string`, not `double` as expected")
                .WithAnnotations("[Check.IsLessThan]")
                .EndMessage();
        }

        [TestMethod] public void IsLessThan_DecimalAnchorIsOutOfRange_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Census);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidConstraintValueException>()
                .WithLocation("`Census` → PercentIndian")
                .WithProblem($"`double` {double.MaxValue} is outside the supported range for `decimal`")
                .WithAnnotations("[Check.IsLessThan]")
                .EndMessage();
        }

        [TestMethod] public void IsLessThan_DateTimeAnchorIsNotString_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(NobelPrize);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidConstraintValueException>()
                .WithLocation("`NobelPrize` → Awarded")
                .WithProblem("value 37 is of type `sbyte`, not `string` as expected")
                .WithAnnotations("[Check.IsLessThan]")
                .EndMessage();
        }

        [TestMethod] public void IsLessThan_DateTimeAnchorIsMalformatted_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Shogunate);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidConstraintValueException>()
                .WithLocation("`Shogunate` → Established")
                .WithProblem($"unable to parse `string` value \"Wednesday, August 18, 1988\" as a `DateTime`")
                .WithAnnotations("[Check.IsLessThan]")
                .EndMessage();
        }

        [TestMethod] public void IsLessThan_DateTimeAnchorIsOutOfRange_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(ISOStandard);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidConstraintValueException>()
                .WithLocation("`ISOStandard` → Adopted")
                .WithProblem($"unable to parse `string` value \"1735-02-48\" as a `DateTime`")
                .WithAnnotations("[Check.IsLessThan]")
                .EndMessage();
        }

        [TestMethod] public void IsLessThan_AnchorMatchesDataConversionSourceType_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Artiodactyl);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidConstraintValueException>()
                .WithLocation("`Artiodactyl` → NumToes")
                .WithProblem("value 8 is of type `byte`, not `int` as expected")
                .WithAnnotations("[Check.IsLessThan]")
                .EndMessage();
        }

        [TestMethod] public void IsLessThan_AnchorMatchesDataConversionTargetType() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Phobia);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("Prevalence", ComparisonOperator.LT, "100.00001").And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsLessThan_ScalarConstrainedMultipleTimes() {
            // Arrange
            var translator = new Translator();
            var source = typeof(CinemaSins);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("SinCount", ComparisonOperator.LT, 1712312389UL).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsLessThan_PathIsNull_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(BaseballBat);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`BaseballBat` → Weight")
                .WithProblem("the path cannot be 'null'")
                .WithAnnotations("[Check.IsLessThan]")
                .EndMessage();
        }

        [TestMethod] public void IsLessThan_PathOnScalar_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Potato);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`Potato` → Weight")
                .WithProblem("the path \"---\" does not exist")
                .WithAnnotations("[Check.IsLessthan]")
                .EndMessage();
        }

        [TestMethod] public void IsLessThan_NonExistentPathOnAggregate_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(SurgicalMask);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`SurgicalMask` → ID")
                .WithProblem("the path \"---\" does not exist")
                .WithAnnotations("[Check.IsLessThan]")
                .EndMessage();
        }

        [TestMethod] public void IsLessThan_NoPathOnAggregate_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(SecretSociety);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`SecretSociety` → Initiation")
                .WithProblem("the annotation cannot be applied to a property of Aggregate type `Activity`")
                .WithAnnotations("[Check.IsLessThan]")
                .EndMessage();
        }

        [TestMethod] public void IsLessThan_NonExistentPathOnReference_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(NationalMonument);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`NationalMonument` → EstablishedBy")
                .WithProblem("the path \"---\" does not exist")
                .WithAnnotations("[Check.IsLessThan]")
                .EndMessage();
        }

        [TestMethod] public void IsLessThan_NonPrimaryKeyPathOnReference_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(YogaPosition);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`YogaPosition` → SanskritName")
                .WithProblem("the path \"Sanskrit\" does not exist")
                .WithAnnotations("[Check.IsLessThan]")
                .EndMessage();
        }

        [TestMethod] public void IsLessThan_NoPathOnReference_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(PubCrawl);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`PubCrawl` → FirstPub")
                .WithProblem("the annotation cannot be applied to a property of Reference type `Pub`")
                .WithAnnotations("[Check.IsLessThan]")
                .EndMessage();
        }

        [TestMethod] public void IsLessThan_NonExistentPathOnRelation_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Mime);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`Mime` → <synthetic> `Performances`")
                .WithProblem("the path \"---\" does not exist")
                .WithAnnotations("[Check.IsLessThan]")
                .EndMessage();
        }

        [TestMethod] public void IsLessThan_NonAnchorPrimaryKeyPathOnRelation_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(CatholicCardinal);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`CatholicCardinal` → <synthetic> `Conclaves`")
                .WithProblem("the path \"CatholicCardinal.DeathDate\" does not exist")
                .WithAnnotations("[Check.IsLessThan]")
                .EndMessage();
        }

        [TestMethod] public void IsLessThan_NoPathOnRelation_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Hemalurgy);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Hemalurgy` → <synthetic> `Steals`")
                .WithProblem("the annotation cannot be applied to a property of Relation type `RelationSet<string>`")
                .WithAnnotations("[Check.IsLessThan]")
                .EndMessage();
        }

        [TestMethod] public void IsLessThan_DefaultValueDoesNotSatisfyConstraint_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(ParkingGarage);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidatedDefaultException>()
                .WithLocation("`ParkingGarage` → CostPerHour")
                .WithProblem("the Field's default value of 15.0 does not pass the constraint")
                .WithAnnotations("[Check.IsLessThan]")
                .EndMessage();
        }

        [TestMethod] public void IsLessThan_ValidDefaultValueIsInvalidatedByConstraint_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(ContactLens);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidatedDefaultException>()
                .WithLocation("`ContactLens` → Color")
                .WithPath("B")
                .WithProblem("the Field's default value of 197 does not pass the constraint")
                .WithAnnotations("[Check.IsLessThan]")
                .EndMessage();
        }
    }

    [TestClass, TestCategory("Constraints - Comparison")]
    public class IsGreaterOrEqualToTests {
        [TestMethod] public void IsGreaterOrEqualTo_NumericFields() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Geyser);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("EruptionHeight", ComparisonOperator.GTE, 0L).And
                .HaveConstraint("Elevation", ComparisonOperator.GTE, 0f).And
                .HaveConstraint("EruptionDuration", ComparisonOperator.GTE, 0U).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsGreaterOrEqualTo_TextualFields() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Hotel);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("HotelName", ComparisonOperator.GTE, "").And
                .HaveConstraint("Stars", ComparisonOperator.GTE, '1').And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsGreaterOrEqualTo_BooleanField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Steak);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Steak` → FromSteakhouse")
                .WithProblem("the annotation cannot be applied to a Field of non-orderable type `bool`")
                .WithAnnotations("[Check.IsGreaterOrEqualTo")
                .EndMessage();
        }

        [TestMethod] public void IsGreaterOrEqualTo_DecimalField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(ETF);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("ClosingPrice", ComparisonOperator.GTE, (decimal)-18.412006).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsGreaterOrEqualTo_DateTimeField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(PEP);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("CreatedOn", ComparisonOperator.GTE, new DateTime(1887, 4, 29)).And
                .HaveNoOtherCandidateKeys();
        }

        [TestMethod] public void IsGreaterOrEqualTo_GuidField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(CoatOfArms);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`CoatOfArms` → ID")
                .WithProblem("the annotation cannot be applied to a Field of non-orderable type `Guid`")
                .WithAnnotations("[Check.IsGreaterOrEqualTo")
                .EndMessage();
        }

        [TestMethod] public void IsGreaterOrEqualTo_EnumerationField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(CivCityState);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`CivCityState` → Type")
                .WithProblem("the annotation cannot be applied to a Field of non-orderable type `Category`")
                .WithAnnotations("[Check.IsGreaterOrEqualTo")
                .EndMessage();
        }

        [TestMethod] public void IsGreaterOrEqualTo_AggregateNestedApplicableScalar() {
            // Arrange
            var translator = new Translator();
            var source = typeof(FamilyTree);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("Focal.FirstName", ComparisonOperator.GTE, "Tony").And
                .HaveConstraint("Focal.DOB", ComparisonOperator.GTE, new DateTime(1255, 9, 18)).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsGreaterOrEqualTo_AggregateNestedInapplicableScalar_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Readymade);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Readymade` → Registration")
                .WithPath("IsFormallyRegistered")
                .WithProblem("the annotation cannot be applied to a Field of non-orderable type `bool`")
                .WithAnnotations("[Check.IsGreaterOrEqualTo]")
                .EndMessage();
        }

        [TestMethod] public void IsGreaterOrEqualTo_NestedAggregate_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(FitnessCenter);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`FitnessCenter` → Address")
                .WithPath("Street")
                .WithProblem("the annotation cannot be applied to a property of Aggregate type `Street`")
                .WithAnnotations("[Check.IsGreaterOrEqualTo]")
                .EndMessage();
        }

        [TestMethod] public void IsGreaterOrEqualTo_ReferenceNestedApplicableScalar() {
            // Arrange
            var translator = new Translator();
            var source = typeof(CandyBar);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("Plant.Name", ComparisonOperator.GTE, "Kraft-Heinz 87").And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsGreaterOrEqualTo_ReferenceNestedInapplicableScalar_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(SlumberParty);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`SlumberParty` → Place")
                .WithPath("StreetSuffix")
                .WithProblem("the annotation cannot be applied to a Field of non-orderable type `RoadType`")
                .WithAnnotations("[Check.IsGreaterOrEqualTo]")
                .EndMessage();
        }

        [TestMethod] public void IsGreaterOrEqualTo_OriginalOnReferenceNestedScalar() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Spa);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsGreaterOrEqualTo_NestedReference_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Barbie);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Barbie` → Relationships")
                .WithPath("Boyfriend.Ken")
                .WithProblem("the annotation cannot be applied to a property of Reference type `Ken`")
                .WithAnnotations("[Check.IsGreaterOrEqualTo]")
                .EndMessage();
        }

        [TestMethod] public void IsGreaterOrEqualTo_RelationNestedApplicableScalar() {
            // Arrange
            var translator = new Translator();
            var source = typeof(PuppetShow);

            // Act
            var translation = translator[source];

            // Assert
            translation.Relations[0].Table.Should()
                .HaveConstraint("Item.Name", ComparisonOperator.GTE, "Elmo").And
                .HaveConstraint("Item.Value", ComparisonOperator.GTE, (decimal)22.5).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsGreaterOrEqualTo_RelationNestedInapplicableScalar_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Horcrux);

            // Act
            var translate = () => translator[source];

            // Act
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Horcrux` → <synthetic> `HidingPlaces`")
                .WithPath("Value.Discovered")
                .WithProblem("the annotation cannot be applied to a Field of non-orderable type `bool`")
                .WithAnnotations("[Check.IsGreaterOrEqualTo]")
                .EndMessage();
        }

        [TestMethod] public void IsGreaterOrEqualTo_NestedRelation_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(WheresWaldo);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`WheresWaldo` → Q2")
                .WithPath("Decoys")
                .WithProblem("the annotation cannot be applied to a property of Relation type `RelationList<Coordinate>`")
                .WithAnnotations("[Check.IsGreaterOrEqualTo]")
                .EndMessage();
        }

        [TestMethod] public void IsGreaterOrEqualTo_NullableTotallyOrderedFields() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Muscle);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("TA2", ComparisonOperator.GTE, 10U).And
                .HaveConstraint("Nerve", ComparisonOperator.GTE, "~~~").And
                .HaveConstraint("FirstDocumented", ComparisonOperator.GTE, new DateTime(937, 12, 18)).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsGreaterOrEqualTo_InconvertibleAnchor_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(LandCard);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidConstraintValueException>()
                .WithLocation("`LandCard` → BlueManna")
                .WithProblem("value \"None\" is of type `string`, not `byte` as expected")
                .WithAnnotations("[Check.IsGreaterOrEqualTo]")
                .EndMessage();
        }

        [TestMethod] public void IsGreaterOrEqualTo_ConvertibleAnchor_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Keystroke);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidConstraintValueException>()
                .WithLocation("`Keystroke` → ResultingGlyph")
                .WithProblem("value 290 is of type `int`, not `char` as expected")
                .WithAnnotations("[Check.IsGreaterOrEqualTo]")
                .EndMessage();
        }

        [TestMethod] public void IsGreaterOrEqualTo_ArrayAnchor_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Zoo);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidConstraintValueException>()
                .WithLocation("`Zoo` → AverageVisitorsPerDay")
                .WithProblem("value cannot be an array")
                .WithAnnotations("[Check.IsGreaterOrEqualTo]")
                .EndMessage();
        }

        [TestMethod] public void IsGreaterOrEqualTo_NullAnchor_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Neurotoxin);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidConstraintValueException>()
                .WithLocation("`Neurotoxin` → MolarMass")
                .WithProblem("the constraint boundary cannot be 'null'")
                .WithAnnotations("[Check.IsGreaterOrEqualTo]")
                .EndMessage();
        }

        [TestMethod] public void IsGreaterOrEqualTo_AnchorIsMinimum_Redundant() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Bacterium);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("NumStrains", ComparisonOperator.GTE, ushort.MinValue).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsGreaterOrEqualTo_DecimalAnchorIsNotDouble_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(GitHook);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidConstraintValueException>()
                .WithLocation("`GitHook` → NumExecutions")
                .WithProblem("value 1.0 is of type `float`, not `double` as expected")
                .WithAnnotations("[Check.IsGreaterOrEqualTo]")
                .EndMessage();
        }

        [TestMethod] public void IsGreaterOrEqualTo_DecimalAnchorIsOutOfRange_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(RubeGoldbergMachine);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidConstraintValueException>()
                .WithLocation("`RubeGoldbergMachine` → MaterialsCost")
                .WithProblem($"`double` {double.NegativeInfinity} is outside the supported range for `decimal`")
                .WithAnnotations("[Check.IsGreaterOrEqualTo]")
                .EndMessage();
        }

        [TestMethod] public void IsGreaterOrEqualTo_DateTimeAnchorIsNotString_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Smurf);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidConstraintValueException>()
                .WithLocation("`Smurf` → FirstIntroduced")
                .WithProblem("value 318.909 is of type `float`, not `string` as expected")
                .WithAnnotations("[Check.IsGreaterOrEqualTo]")
                .EndMessage();
        }

        [TestMethod] public void IsGreaterOrEqualTo_DateTimeAnchorIsMalformatted_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(WorldCup);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidConstraintValueException>()
                .WithLocation("`WorldCup` → ChampionshipDate")
                .WithProblem($"unable to parse `string` value \"1111(11)11\" as a `DateTime`")
                .WithAnnotations("[Check.IsGreaterOrEqualTo]")
                .EndMessage();
        }

        [TestMethod] public void IsGreaterOrEqualTo_DateTimeAnchorIsOutOfRange_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(SharkTankPitch);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidConstraintValueException>()
                .WithLocation("`SharkTankPitch` → AirDate")
                .WithProblem($"unable to parse `string` value \"91237-00-16\" as a `DateTime`")
                .WithAnnotations("[Check.IsGreaterOrEqualTo]")
                .EndMessage();
        }

        [TestMethod] public void IsGreaterOrEqualTo_AnchorMatchesDataConversionSourceType_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Mushroom);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidConstraintValueException>()
                .WithLocation("`Mushroom` → AverageWeight")
                .WithProblem("value -18.0933 is of type `double`, not `int` as expected")
                .WithAnnotations("[Check.IsGreaterOrEqualTo]")
                .EndMessage();
        }

        [TestMethod] public void IsGreaterOrEqualTo_AnchorMatchesDataConversionTargetType() {
            // Arrange
            var translator = new Translator();
            var source = typeof(EMail);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("CC", ComparisonOperator.GTE, 73).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsGreaterOrEqualTo_ScalarConstrainedMultipleTimes() {
            // Arrange
            var translator = new Translator();
            var source = typeof(SolarEclipse);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("SarosCycle", ComparisonOperator.GTE, 3).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsGreaterOrEqualTo_PathIsNull_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(YuGiOhMonster);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`YuGiOhMonster` → Attack")
                .WithProblem("the path cannot be 'null'")
                .WithAnnotations("[Check.IsGreaterOrEqualTo]")
                .EndMessage();
        }

        [TestMethod] public void IsGreaterOrEqualTo_PathOnScalar_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Hieroglyph);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`Hieroglyph` → Glyph")
                .WithProblem("the path \"---\" does not exist")
                .WithAnnotations("[Check.IsGreaterOrEqualTo]")
                .EndMessage();
        }

        [TestMethod] public void IsGreaterOrEqualTo_NonExistentPathOnAggregate_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Pagoda);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`Pagoda` → Location")
                .WithProblem("the path \"---\" does not exist")
                .WithAnnotations("[Check.IsGreaterOrEqualTo]")
                .EndMessage();
        }

        [TestMethod] public void IsGreaterOrEqualTo_NoPathOnAggregate_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Motorcycle);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Motorcycle` → Wheels")
                .WithProblem("the annotation cannot be applied to a property of Aggregate type `Wheel`")
                .WithAnnotations("[Check.IsGreaterOrEqualTo]")
                .EndMessage();
        }

        [TestMethod] public void IsGreaterOrEqualTo_NonExistentPathOnReference_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Druid);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`Druid` → WildShape2")
                .WithProblem("the path \"---\" does not exist")
                .WithAnnotations("[Check.IsGreaterOrEqualTo]")
                .EndMessage();
        }

        [TestMethod] public void IsGreaterOrEqualTo_NonPrimaryKeyPathOnReference_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Mirror);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`Mirror` → MirrorShape")
                .WithProblem("the path \"Sides\" does not exist")
                .WithAnnotations("[Check.IsGreaterOrEqualTo]")
                .EndMessage();
        }

        [TestMethod] public void IsGreaterOrEqualTo_NoPathOnReference_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Chromosome);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Chromosome` → FirstIsolatedGene")
                .WithProblem("the annotation cannot be applied to a property of Reference type `Gene`")
                .WithAnnotations("[Check.IsGreaterOrEqualTo]")
                .EndMessage();
        }

        [TestMethod] public void IsGreaterOrEqualTo_NonExistentPathOnRelation_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(HighlanderImmortal);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`HighlanderImmortal` → <synthetic> `Swords`")
                .WithProblem("the path \"---\" does not exist")
                .WithAnnotations("[Check.IsGreaterOrEqualTo]")
                .EndMessage();
        }

        [TestMethod] public void IsGreaterOrEqualTo_NonAnchorPrimaryKeyPathOnRelation_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Synagogue);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`Synagogue` → <synthetic> `Congregants`")
                .WithProblem("the path \"Synagogue.Denomination\" does not exist")
                .WithAnnotations("[Check.IsGreaterOrEqualTo]")
                .EndMessage();
        }

        [TestMethod] public void IsGreaterOrEqualTo_NoPathOnRelation_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Panegyric);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Panegyric` → <synthetic> `Lines`")
                .WithProblem("the annotation cannot be applied to a property of Relation type `RelationMap<int, string>`")
                .WithAnnotations("[Check.IsGreaterOrEqualTo]")
                .EndMessage();
        }

        [TestMethod] public void IsGreaterOrEqualTo_DefaultValueDoesNotSatisfyConstraint_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Camera);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidatedDefaultException>()
                .WithLocation("`Camera` → ShutterSpeed")
                .WithProblem("the Field's default value of 1E-05 does not pass the constraint")
                .WithAnnotations("[Check.IsGreaterOrEqualTo]")
                .EndMessage();
        }

        [TestMethod] public void IsGreaterOrEqualTo_ValidDefaultValueIsInvalidatedByConstraint_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(SlapBet);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidatedDefaultException>()
                .WithLocation("`SlapBet` → Wager")
                .WithPath("Slaps")
                .WithProblem("the Field's default value of 1 does not pass the constraint")
                .WithAnnotations("[Check.IsGreaterOrEqualTo]")
                .EndMessage();
        }
    }

    [TestClass, TestCategory("Constraints - Comparison")]
    public class IsLessOrEqualToTests {
        [TestMethod] public void IsLessOrEqualTo_NumericFields() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Fjord);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("Latitude", ComparisonOperator.LTE, 90f).And
                .HaveConstraint("Longitude", ComparisonOperator.LTE, 90f).And
                .HaveConstraint("Length", ComparisonOperator.LTE, 100000UL).And
                .HaveConstraint("Width", ComparisonOperator.LTE, (short)6723).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsLessOrEqualTo_TextualFields() {
            // Arrange
            var translator = new Translator();
            var source = typeof(ExcelRange);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("StartColumn", ComparisonOperator.LTE, 'Z').And
                .HaveConstraint("EndColumn", ComparisonOperator.LTE, "XFD").And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsLessOrEqualTo_BooleanField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(TectonicPlate);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`TectonicPlate` → OnRingOfFire")
                .WithProblem("the annotation cannot be applied to a Field of non-orderable type `bool`")
                .WithAnnotations("[Check.IsLessOrEqualTo")
                .EndMessage();
        }

        [TestMethod] public void IsLessOrEqualTo_DecimalField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Caliphate);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("Population", ComparisonOperator.LTE, (decimal)8192481241.412841).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsLessOrEqualTo_DateTimeField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Representative);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("FirstElected", ComparisonOperator.LTE, new DateTime(2688, 12, 2)).And
                .HaveNoOtherCandidateKeys();
        }

        [TestMethod] public void IsLessOrEqualTo_GuidField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Sunscreen);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Sunscreen` → ID")
                .WithProblem("the annotation cannot be applied to a Field of non-orderable type `Guid`")
                .WithAnnotations("[Check.IsLessOrEqualTo")
                .EndMessage();
        }

        [TestMethod] public void IsLessOrEqualTo_EnumerationField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(ConcertTour);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`ConcertTour` → ArtistType")
                .WithProblem("the annotation cannot be applied to a Field of non-orderable type `Type`")
                .WithAnnotations("[Check.IsLessOrEqualTo")
                .EndMessage();
        }

        [TestMethod] public void IsLessOrEqualTo_AggregateNestedApplicableScalar() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Hominin);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("Species.Genus", ComparisonOperator.LTE, "Zubeia").And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsLessOrEqualTo_AggregateNestedInapplicableScalar_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(AmazonService);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`AmazonService` → Plan")
                .WithPath("Type")
                .WithProblem("the annotation cannot be applied to a Field of non-orderable type `SubscriptionType`")
                .WithAnnotations("[Check.IsLessOrEqualTo]")
                .EndMessage();
        }

        [TestMethod] public void IsLessOrEqualTo_NestedAggregate_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Shampoo);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Shampoo` → Directions")
                .WithPath("Ages")
                .WithProblem("the annotation cannot be applied to a property of Aggregate type `AgeRange`")
                .WithAnnotations("[Check.IsLessOrEqualTo]")
                .EndMessage();
        }

        [TestMethod] public void IsLessOrEqualTo_ReferenceNestedApplicableScalar() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Gatorade);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("BottledAt.Operational", ComparisonOperator.LTE, new DateTime(2566, 11, 15)).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsLessOrEqualTo_ReferenceNestedInapplicableScalar_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Knife);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Knife` → Categorization")
                .WithPath("Which")
                .WithProblem("the annotation cannot be applied to a Field of non-orderable type `Category`")
                .WithAnnotations("[Check.IsLessOrEqualTo]")
                .EndMessage();
        }

        [TestMethod] public void IsLessOrEqualTo_OriginalOnReferenceNestedScalar() {
            // Arrange
            var translator = new Translator();
            var source = typeof(WhiteWalker);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsLessOrEqualTo_NestedReference_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Ransomware);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Ransomware` → Extortion")
                .WithPath("Ransom")
                .WithProblem("the annotation cannot be applied to a property of Reference type `Ransom`")
                .WithAnnotations("[Check.IsLessOrEqualTo]")
                .EndMessage();
        }

        [TestMethod] public void IsLessOrEqualTo_RelationNestedApplicableScalar() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Syllabary);

            // Act
            var translation = translator[source];

            // Assert
            translation.Relations[0].Table.Should()
                .HaveConstraint("Value", ComparisonOperator.LTE, '|').And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsLessOrEqualTo_RelationNestedInapplicableScalar_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(TreehouseOfHorror);

            // Act
            var translate = () => translator[source];

            // Act
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`TreehouseOfHorror` → <synthetic> `Crew`")
                .WithPath("Value")
                .WithProblem("the annotation cannot be applied to a Field of non-orderable type `Role`")
                .WithAnnotations("[Check.IsLessOrEqualTo]")
                .EndMessage();
        }

        [TestMethod] public void IsLessOrEqualTo_NestedRelation_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(CocoaFarm);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`CocoaFarm` → Personnel")
                .WithPath("Regulators")
                .WithProblem("the annotation cannot be applied to a property of Relation type `RelationList<string>`")
                .WithAnnotations("[Check.IsLessOrEqualTo]")
                .EndMessage();
        }

        [TestMethod] public void IsLessOrEqualTo_NullableTotallyOrderedFields() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Subreddit);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("Moderator", ComparisonOperator.LTE, "???").And
                .HaveConstraint("Initiated", ComparisonOperator.LTE, new DateTime(7771, 4, 15)).And
                .HaveConstraint("TimesQuarantined", ComparisonOperator.LTE, 47).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsLessOrEqualTo_InconvertibleAnchor_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Dreidel);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidConstraintValueException>()
                .WithLocation("`Dreidel` → SerialCode")
                .WithProblem("value 153 is of type `byte`, not `string` as expected")
                .WithAnnotations("[Check.IsLessOrEqualTo]")
                .EndMessage();
        }

        [TestMethod] public void IsLessOrEqualTo_ConvertibleAnchor_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(ArthurianKnight);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidConstraintValueException>()
                .WithLocation("`ArthurianKnight` → MalloryMentions")
                .WithProblem("value 4 is of type `uint`, not `ulong` as expected")
                .WithAnnotations("[Check.IsLessOrEqualTo]")
                .EndMessage();
        }

        [TestMethod] public void IsLessOrEqualTo_ArrayAnchor_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Mint);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidConstraintValueException>()
                .WithLocation("`Mint` → Established")
                .WithProblem("value cannot be an array")
                .WithAnnotations("[Check.IsLessOrEqualTo]")
                .EndMessage();
        }

        [TestMethod] public void IsLessOrEqualTo_NullAnchor_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(VoirDire);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidConstraintValueException>()
                .WithLocation("`VoirDire` → BatsonChallenges")
                .WithProblem("the constraint boundary cannot be 'null'")
                .WithAnnotations("[Check.IsLessOrEqualTo]")
                .EndMessage();
        }

        [TestMethod] public void IsLessOrEqualTo_AnchorIsMaximum_Redundant() {
            // Arrange
            var translator = new Translator();
            var source = typeof(ShellCommand);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("NumOptions", ComparisonOperator.LTE, long.MaxValue).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsLessOrEqualTo_DecimalAnchorIsNotDouble_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(ChewingGum);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidConstraintValueException>()
                .WithLocation("`ChewingGum` → AverageLifetime")
                .WithProblem("value '(' is of type `char`, not `double` as expected")
                .WithAnnotations("[Check.IsLessOrEqualTo]")
                .EndMessage();
        }

        [TestMethod] public void IsLessOrEqualTo_DecimalAnchorIsOutOfRange_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Headphones);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidConstraintValueException>()
                .WithLocation("`Headphones` → MaxVolume")
                .WithProblem($"`double` {double.PositiveInfinity} is outside the supported range for `decimal`")
                .WithAnnotations("[Check.IsLessOrEqualTo]")
                .EndMessage();
        }

        [TestMethod] public void IsLessOrEqualTo_DateTimeAnchorIsNotString_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(ClockTower);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidConstraintValueException>()
                .WithLocation("`ClockTower` → Inaugurated")
                .WithProblem("value -381723 is of type `long`, not `string` as expected")
                .WithAnnotations("[Check.IsLessOrEqualTo]")
                .EndMessage();
        }

        [TestMethod] public void IsLessOrEqualTo_DateTimeAnchorIsMalformatted_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(KentuckyDerby);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidConstraintValueException>()
                .WithLocation("`KentuckyDerby` → Racetime")
                .WithProblem($"unable to parse `string` value \"2317-04-19 @ 2:00pm\" as a `DateTime`")
                .WithAnnotations("[Check.IsLessOrEqualTo]")
                .EndMessage();
        }

        [TestMethod] public void IsLessOrEqualTo_DateTimeAnchorIsOutOfRange_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Firearm);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidConstraintValueException>()
                .WithLocation("`Firearm` → Manufactured")
                .WithProblem($"unable to parse `string` value \"1927-03-109\" as a `DateTime`")
                .WithAnnotations("[Check.IsLessOrEqualTo]")
                .EndMessage();
        }

        [TestMethod] public void IsLessOrEqualTo_AnchorMatchesDataConversionSourceType_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(ShardOfAdonalsium);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidConstraintValueException>()
                .WithLocation("`ShardOfAdonalsium` → Splintered")
                .WithProblem("value false is of type `bool`, not `int` as expected")
                .WithAnnotations("[Check.IsLessOrEqualTo]")
                .EndMessage();
        }

        [TestMethod] public void IsLessOrEqualTo_AnchorMatchesDataConversionTargetType() {
            // Arrange
            var translator = new Translator();
            var source = typeof(HTMLElement);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("NumChildren", ComparisonOperator.LTE, "400000").And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsLessOrEqualTo_ScalarConstrainedMultipleTimes() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Archbishop);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("City", ComparisonOperator.LTE, "124").And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsLessOrEqualTo_PathIsNull_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(GameOfClue);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`GameOfClue` → CrimeScene")
                .WithProblem("the path cannot be 'null'")
                .WithAnnotations("[Check.IsLessOrEqualTo]")
                .EndMessage();
        }

        [TestMethod] public void IsLessOrEqualTo_PathOnScalar_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(PlaneOfExistence);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`PlaneOfExistence` → Name")
                .WithProblem("the path \"---\" does not exist")
                .WithAnnotations("[Check.IsLessOrEqualTo]")
                .EndMessage();
        }

        [TestMethod] public void IsLessOrEqualTo_NonExistentPathOnAggregate_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Mausoleum);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`Mausoleum` → Location")
                .WithProblem("the path \"---\" does not exist")
                .WithAnnotations("[Check.IsLessOrEqualTo]")
                .EndMessage();
        }

        [TestMethod] public void IsLessOrEqualTo_NoPathOnAggregate_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Pseudonym);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Pseudonym` → For")
                .WithProblem("the annotation cannot be applied to a property of Aggregate type `Name`")
                .WithAnnotations("[Check.IsLessOrEqualTo]")
                .EndMessage();
        }

        [TestMethod] public void IsLessOrEqualTo_NonExistentPathOnReference_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(FoodPantry);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`FoodPantry` → WhichState")
                .WithProblem("the path \"---\" does not exist")
                .WithAnnotations("[Check.IsLessOrEqualTo]")
                .EndMessage();
        }

        [TestMethod] public void IsLessOrEqualTo_NonPrimaryKeyPathOnReference_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(FittedSheet);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`FittedSheet` → Dimensions")
                .WithProblem("the path \"ThreadCount\" does not exist")
                .WithAnnotations("[Check.IsLessOrEqualTo]")
                .EndMessage();
        }

        [TestMethod] public void IsLessOrEqualTo_NoPathOnReference_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Playlist);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Playlist` → MostPlayed")
                .WithProblem("the annotation cannot be applied to a property of Reference type `Song`")
                .WithAnnotations("[Check.IsLessOrEqualTo]")
                .EndMessage();
        }

        [TestMethod] public void IsLessOrEqualTo_NonExistentPathOnRelation_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(ThumbWar);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`ThumbWar` → <synthetic> `PlayByPlay`")
                .WithProblem("the path \"---\" does not exist")
                .WithAnnotations("[Check.IsLessOrEqualTo]")
                .EndMessage();
        }

        [TestMethod] public void IsLessOrEqualTo_NonAnchorPrimaryKeyPathOnRelation_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(EngagementRing);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`EngagementRing` → <synthetic> `Measurements`")
                .WithProblem("the path \"EngagementRing.Centerpiece\" does not exist")
                .WithAnnotations("[Check.IsLessOrEqualTo]")
                .EndMessage();
        }

        [TestMethod] public void IsLessOrEqualTo_NoPathOnRelation_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(ImpracticalJoke);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`ImpracticalJoke` → <synthetic> `JokeTargets`")
                .WithProblem("the annotation cannot be applied to a property of Relation type `RelationSet<string>`")
                .WithAnnotations("[Check.IsLessOrEqualTo]")
                .EndMessage();
        }

        [TestMethod] public void IsLessOrEqualTo_DefaultValueDoesNotSatisfyConstraint_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(BowlingFrame);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidatedDefaultException>()
                .WithLocation("`BowlingFrame` → SecondThrowPins")
                .WithProblem("the Field's default value of 23 does not pass the constraint")
                .WithAnnotations("[Check.IsLessOrEqualTo]")
                .EndMessage();
        }

        [TestMethod] public void IsLessOrEqualTo_ValidDefaultValueIsInvalidatedByConstraint_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Defenestration);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidatedDefaultException>()
                .WithLocation("`Defenestration` → ThrownFrom")
                .WithPath("Width")
                .WithProblem("the Field's default value of 178.916 does not pass the constraint")
                .WithAnnotations("[Check.IsLessOrEqualTo]")
                .EndMessage();
        }
    }

    [TestClass, TestCategory("Constraints - Comparison")]
    public class IsNotTests {
        [TestMethod] public void IsNot_NumericFields() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Bridge);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("Length", ComparisonOperator.NE, 34).And
                .HaveConstraint("Height", ComparisonOperator.NE, 15UL).And
                .HaveConstraint("Width", ComparisonOperator.NE, 0.23776f).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNot_TextualFields() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Quatrain);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("Line1", ComparisonOperator.NE, "Elephant").And
                .HaveConstraint("Line2", ComparisonOperator.NE, "Giraffe").And
                .HaveConstraint("Line3", ComparisonOperator.NE, "Crocodile").And
                .HaveConstraint("Line4", ComparisonOperator.NE, "Rhinoceros").And
                .HaveConstraint("FirstLetter", ComparisonOperator.NE, '$').And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNot_BooleanField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(PoliceOfficer);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("IsRetired", ComparisonOperator.EQ, true).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNot_DecimalField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Therapist);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("CostPerHour", ComparisonOperator.NE, (decimal)0.750).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNot_DateTimeField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(SlotMachine);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("InstalledOn", ComparisonOperator.NE, new DateTime(4431, 1, 21)).And
                .HaveNoOtherCandidateKeys();
        }

        [TestMethod] public void IsNot_GuidField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Church);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("ChurchID", ComparisonOperator.NE, new Guid("a3c3ac24-4cf2-428e-a4db-76b30958cc90")).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNot_EnumerationField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(MarianApparition);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField("When").OfTypeDateTime().BeingNonNullable().And
                .HaveField("Location").OfTypeText().BeingNonNullable().And
                .HaveField("Witnesses").OfTypeText().BeingNonNullable().And
                .HaveField("MarianTitle").OfTypeText().BeingNonNullable().And
                .HaveField("Recognition").OfTypeEnumeration(
                    MarianApparition.Status.Accepted,
                    MarianApparition.Status.Alleged,
                    MarianApparition.Status.Confirmed,
                    MarianApparition.Status.Documented,
                    MarianApparition.Status.Recognized
                ).BeingNonNullable().And
                .HaveNoOtherFields().And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNot_AggregateNestedScalar() {
            // Arrange
            var translator = new Translator();
            var source = typeof(SeventhInningStretch);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("Game.AwayTeam", ComparisonOperator.NE, "Savannah Bananas").And
                .HaveConstraint("Game.Date", ComparisonOperator.NE, new DateTime(2001, 9, 11)).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNot_NestedAggregate_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(SportsBet);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`SportsBet` → Odds")
                .WithPath("OneDollarPayout")
                .WithProblem("the annotation cannot be applied to a property of Aggregate type `OneDollar`")
                .WithAnnotations("[Check.IsNot]")
                .EndMessage();
        }

        [TestMethod] public void IsNot_ReferenceNestedScalar() {
            // Arrange
            var translator = new Translator();
            var source = typeof(StanleyCup);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("Champion.Abbreviation", ComparisonOperator.NE, "NBC").And
                .HaveNoOtherConstraints().And
                .HaveField("RunnerUp.Conference").OfTypeEnumeration(
                    StanleyCup.Conf.Eastern,
                    StanleyCup.Conf.Western
                );
        }

        [TestMethod] public void IsNot_OriginalOnReferenceNestedScalar() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Diet);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNot_NestedReference_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(FishingRod);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`FishingRod` → ManufacturingInfo")
                .WithPath("Manufacturer")
                .WithProblem("the annotation cannot be applied to a property of Reference type `Company`")
                .WithAnnotations("[Check.IsNot]")
                .EndMessage();
        }

        [TestMethod] public void IsNot_RelationNestedScalar() {
            // Arrange
            var translator = new Translator();
            var source = typeof(StandUpComedian);

            // Act
            var translation = translator[source];

            // Assert
            translation.Relations[0].Table.Should()
                .HaveField("Item.Kind").OfTypeEnumeration(
                    StandUpComedian.Kind.KnockKnock,
                    StandUpComedian.Kind.Observational,
                    StandUpComedian.Kind.WordPlay,
                    StandUpComedian.Kind.HistoricalWhatIf,
                    StandUpComedian.Kind.Political
                ).BeingNonNullable().And
                .HaveConstraint("Item.NSFW", ComparisonOperator.EQ, true).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNot_NestedRelation_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Interview);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Interview` → Questions")
                .WithPath("Questions")
                .WithProblem("the annotation cannot be applied to a property of Relation type `RelationMap<string, double>`")
                .WithAnnotations("[Check.IsNot]")
                .EndMessage();
        }

        [TestMethod] public void IsNot_NullableFields() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Fountain);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("FountainUUID", ComparisonOperator.NE, new Guid("926dbe07-875c-46fd-863b-051b98a2d6be")).And
                .HaveConstraint("Unveiled", ComparisonOperator.NE, new DateTime(1131, 8, 19)).And
                .HaveConstraint("Spout", ComparisonOperator.NE, 35.22).And
                .HaveConstraint("Masonry", ComparisonOperator.NE, "Play-Doh").And
                .HaveConstraint("IsActive", ComparisonOperator.EQ, true).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNot_InconvertibleAnchor_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Candle);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidConstraintValueException>()
                .WithLocation("`Candle` → Width")
                .WithProblem("value \"Wide\" is of type `string`, not `float` as expected")
                .WithAnnotations("[Check.IsNot]")
                .EndMessage();
        }

        [TestMethod] public void IsNot_ConvertibleAnchor_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(CompilerWarning);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidConstraintValueException>()
                .WithLocation("`CompilerWarning` → DebugOnly")
                .WithProblem("value 1 is of type `int`, not `bool` as expected")
                .WithAnnotations("[Check.IsNot]")
                .EndMessage();
        }

        [TestMethod] public void IsNot_ArrayAnchor_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Alarm);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidConstraintValueException>()
                .WithLocation("`Alarm` → Snoozeable")
                .WithProblem("value cannot be an array")
                .WithAnnotations("[Check.IsNot]")
                .EndMessage();
        }

        [TestMethod] public void IsNot_NullAnchor_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(HallOfFame);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidConstraintValueException>()
                .WithLocation("`HallOfFame` → Categorization")
                .WithProblem("the constraint value cannot be 'null'")
                .WithAnnotations("[Check.IsNot]")
                .EndMessage();
        }

        [TestMethod] public void IsNot_DecimalAnchorIsNotDouble_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(DistrictAttorney);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidConstraintValueException>()
                .WithLocation("`DistrictAttorney` → ConvictionRate")
                .WithProblem("value false is of type `bool`, not `double` as expected")
                .WithAnnotations("[Check.IsNot]")
                .EndMessage();
        }

        [TestMethod] public void IsNot_DecimalAnchorIsOutOfRange_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Ping);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidConstraintValueException>()
                .WithLocation("`Ping` → RoundTrip")
                .WithProblem($"`double` {double.MaxValue - 3.0} is outside the supported range for `decimal`")
                .WithAnnotations("[Check.IsNot]")
                .EndMessage();
        }

        [TestMethod] public void IsNot_DateTimeAnchorIsNotString_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(InsurancePolicy);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidConstraintValueException>()
                .WithLocation("`InsurancePolicy` → EffectiveAsOf")
                .WithProblem("value -8193.018 is of type `float`, not `string` as expected")
                .WithAnnotations("[Check.IsNot]")
                .EndMessage();
        }

        [TestMethod] public void IsNot_DateTimeAnchorIsMalformatted_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Mosque);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidConstraintValueException>()
                .WithLocation("`Mosque` → Established")
                .WithProblem($"unable to parse `string` value \"1.4.5.0.1.0.3.0\" as a `DateTime`")
                .WithAnnotations("[Check.IsNot]")
                .EndMessage();
        }

        [TestMethod] public void IsNot_DateTimeAnchorIsOutOfRange_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Lease);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidConstraintValueException>()
                .WithLocation("`Lease` → StartDate")
                .WithProblem($"unable to parse `string` value \"1637-07-8819\" as a `DateTime`")
                .WithAnnotations("[Check.IsNot]")
                .EndMessage();
        }

        [TestMethod] public void IsOneOf_GuidValueIsNotString_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(RainDelay);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidConstraintValueException>()
                .WithLocation("`RainDelay` → ID")
                .WithProblem("value 85819205 is of type `ulong`, not `string` as expected")
                .WithAnnotations("[Check.IsNot]")
                .EndMessage();
        }

        [TestMethod] public void IsOneOf_GuidValueIsMalformatted_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Wiretap);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidConstraintValueException>()
                .WithLocation("`Wiretap` → WiretapID")
                .WithProblem("unable to parse `string` value \"This is an INVALID GUID\" as a `Guid`")
                .WithAnnotations("[Check.IsNot]")
                .EndMessage();
        }

        [TestMethod] public void IsNot_AnchorMatchesDataConversionSourceType_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(FairyTale);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidConstraintValueException>()
                .WithLocation("`FairyTale` → Disneyfied")
                .WithProblem("value false is of type `bool`, not `string` as expected")
                .WithAnnotations("[Check.IsNot]")
                .EndMessage();
        }

        [TestMethod] public void IsNot_AnchorMatchesDataConversionTargetType() {
            // Arrange
            var translator = new Translator();
            var source = typeof(RingOfPower);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("NumPossessors", ComparisonOperator.NE, 7).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNot_ScalarConstrainedMultipleTimesSameAnchor() {
            // Arrange
            var translator = new Translator();
            var source = typeof(NazcaLine);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("Name", ComparisonOperator.NE, "Iguana").And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNot_ScalarConstrainedMultipleTimesDifferentAnchors() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Pterosaur);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveConstraint("Specimens", InclusionOperator.NotIn,
                    0U, 7894520U
                ).And
                .HaveNoOtherConstraints();
        }

        [TestMethod] public void IsNot_PathIsNull_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(LotteryTicket);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`LotteryTicket` → PurchaseTime")
                .WithProblem("the path cannot be 'null'")
                .WithAnnotations("[Check.IsNot]")
                .EndMessage();
        }

        [TestMethod] public void IsNot_PathOnScalar_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Prison);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`Prison` → SecurityLevel")
                .WithProblem("the path \"---\" does not exist")
                .WithAnnotations("[Check.IsNot]")
                .EndMessage();
        }

        [TestMethod] public void IsNot_NonExistentPathOnAggregate_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Restaurant);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`Restaurant` → SaladBar")
                .WithProblem("the path \"---\" does not exist")
                .WithAnnotations("[Check.IsNot]")
                .EndMessage();
        }

        [TestMethod] public void IsNot_NoPathOnAggregate_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Balk);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Balk` → Pitcher")
                .WithProblem("the annotation cannot be applied to a property of Aggregate type `Player`")
                .WithAnnotations("[Check.IsNot]")
                .EndMessage();
        }

        [TestMethod] public void IsNot_NonExistentPathOnReference_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Planetarium);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`Planetarium` → Architect")
                .WithProblem("the path \"---\" does not exist")
                .WithAnnotations("[Check.IsNot]")
                .EndMessage();
        }

        [TestMethod] public void IsNot_NonPrimaryKeyPathOnReference_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(LiquorStore);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`LiquorStore` → BestSellingWine")
                .WithProblem("the path \"Vineyard\" does not exist")
                .WithAnnotations("[Check.IsNot]")
                .EndMessage();
        }

        [TestMethod] public void IsNot_NoPathOnReference_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Waterbending);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Waterbending` → StrongestPractitioner")
                .WithProblem("the annotation cannot be applied to a property of Reference type `Person`")
                .WithAnnotations("[Check.IsNot]")
                .EndMessage();
        }

        [TestMethod] public void IsNot_NonExistentPathOnRelation_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(AutoDaFe);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`AutoDaFe` → <synthetic> `ArtworkBurned`")
                .WithProblem("the path \"---\" does not exist")
                .WithAnnotations("[Check.IsNot]")
                .EndMessage();
        }

        [TestMethod] public void IsNot_NonAnchorPrimaryKeyPathOnRelation_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Dream);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`Dream` → <synthetic> `Cameos`")
                .WithProblem("the path \"Dream.REM\" does not exist")
                .WithAnnotations("[Check.IsNot]")
                .EndMessage();
        }

        [TestMethod] public void IsNot_NoPathOnRelation_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(BachelorParty);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`BachelorParty` → <synthetic> `Destinations`")
                .WithProblem("the annotation cannot be applied to a property of Relation type `RelationList<Destination>`")
                .WithAnnotations("[Check.IsNot]")
                .EndMessage();
        }

        [TestMethod] public void IsNot_DefaultValueDoesNotSatisfyConstraint_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(RestStop);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidatedDefaultException>()
                .WithLocation("`RestStop` → Exit")
                .WithProblem("the Field's default value of 153 does not pass the constraint")
                .WithAnnotations("[Check.IsNot]")
                .EndMessage();
        }

        [TestMethod] public void IsNot_ValidDefaultValueIsInvalidatedByConstraint_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(HearthstoneMinion);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidatedDefaultException>()
                .WithLocation("`HearthstoneMinion` → Statistics")
                .WithPath("Health")
                .WithProblem("the Field's default value of -69 does not pass the constraint")
                .WithAnnotations("[Check.IsNot]")
                .EndMessage();
        }
    }
}
