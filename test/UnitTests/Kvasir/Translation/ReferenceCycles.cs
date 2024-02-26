using FluentAssertions;
using Kvasir.Schema;
using Kvasir.Translation2;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using static UT.Kvasir.Translation.ReferenceCycles;

namespace UT.Kvasir.Translation {
    [TestClass, TestCategory("ReferenceCycles")]
    public class ReferenceCycleTests {
        [TestMethod] public void SelfReferentialEntity_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Constitution);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<ReferenceCycleException>()
                .WithLocation("`Constitution` → `Constitution` (from \"Precursor\")")
                .WithProblem("reference cycle detected")
                .EndMessage();
        }

        [TestMethod] public void EntityReferenceChainLength2_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Niqqud);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<ReferenceCycleException>()
                .WithLocation("`Niqqud` → `Vowel` (from \"Pronunciation\") → `Niqqud` (from \"HebrewSymbol\")")
                .WithProblem("reference cycle detected")
                .EndMessage();
        }

        [TestMethod] public void EntityReferenceCycleLength3OrMore_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(RefugeeCamp);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<ReferenceCycleException>()
                .WithLocation("`RefugeeCamp` → `Person` (from \"Director\") → `Country` (from \"BirthCountry\") → `CivilWar` (from \"OngoingCivilWar\") → `RefugeeCamp` (from \"LargestRefugeeCamp\")")
                .WithProblem("reference cycle detected")
                .EndMessage();
        }

        [TestMethod] public void RelationReferenceCycle_DirectElement() {
            // Arrange
            var translator = new Translator();
            var source = typeof(SoftwarePackage);

            // Act
            var translation = translator[source];

            // Assert
            translation.Relations.Should().HaveCount(3);
            translation.Relations[0].Table.Should()
                .HaveName("UT.Kvasir.Translation.ReferenceCycles+SoftwarePackage.BuildDependenciesTable").And
                .HaveField("SoftwarePackage.PackageManager").OfTypeText().BeingNonNullable().And
                .HaveField("SoftwarePackage.Hash").OfTypeText().BeingNonNullable().And
                .HaveField("Item.PackageManager").OfTypeText().BeingNonNullable().And
                .HaveField("Item.Hash").OfTypeText().BeingNonNullable().And
                .HaveNoOtherFields().And
                .HaveForeignKey("SoftwarePackage.Hash", "SoftwarePackage.PackageManager")
                    .Against(translation.Principal.Table)
                    .WithOnDeleteBehavior(OnDelete.Cascade)
                    .WithOnUpdateBehavior(OnUpdate.Cascade).And
                .HaveForeignKey("Item.Hash", "Item.PackageManager")
                    .Against(translation.Principal.Table)
                    .WithOnDeleteBehavior(OnDelete.Cascade)
                    .WithOnUpdateBehavior(OnUpdate.Cascade).And
                .HaveNoOtherForeignKeys();
            translation.Relations[1].Table.Should()
                .HaveName("UT.Kvasir.Translation.ReferenceCycles+SoftwarePackage.FlagsTable").And
                .HaveField("SoftwarePackage.PackageManager").OfTypeText().BeingNonNullable().And
                .HaveField("SoftwarePackage.Hash").OfTypeText().BeingNonNullable().And
                .HaveField("Key").OfTypeText().BeingNonNullable().And
                .HaveField("Value").OfTypeBoolean().BeingNonNullable().And
                .HaveNoOtherFields().And
                .HaveForeignKey("SoftwarePackage.Hash", "SoftwarePackage.PackageManager")
                    .Against(translation.Principal.Table)
                    .WithOnDeleteBehavior(OnDelete.Cascade)
                    .WithOnUpdateBehavior(OnUpdate.Cascade).And
                .HaveNoOtherForeignKeys();
            translation.Relations[2].Table.Should()
                .HaveName("UT.Kvasir.Translation.ReferenceCycles+SoftwarePackage.RunDependenciesTable").And
                .HaveField("SoftwarePackage.PackageManager").OfTypeText().BeingNonNullable().And
                .HaveField("SoftwarePackage.Hash").OfTypeText().BeingNonNullable().And
                .HaveField("Item.PackageManager").OfTypeText().BeingNonNullable().And
                .HaveField("Item.Hash").OfTypeText().BeingNonNullable().And
                .HaveNoOtherFields().And
                .HaveForeignKey("SoftwarePackage.Hash", "SoftwarePackage.PackageManager")
                    .Against(translation.Principal.Table)
                    .WithOnDeleteBehavior(OnDelete.Cascade)
                    .WithOnUpdateBehavior(OnUpdate.Cascade).And
                .HaveForeignKey("Item.Hash", "Item.PackageManager")
                    .Against(translation.Principal.Table)
                    .WithOnDeleteBehavior(OnDelete.Cascade)
                    .WithOnUpdateBehavior(OnUpdate.Cascade).And
                .HaveNoOtherForeignKeys();
        }

        [TestMethod] public void RelationReferenceCycle_AggregateElement() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Indictment);

            // Act
            var translation = translator[source];

            // Assert
            translation.Relations.Should().HaveCount(1);
            translation.Relations[0].Table.Should()
                .HaveName("UT.Kvasir.Translation.ReferenceCycles+Indictment.ChargesTable").And
                .HaveField("Indictment.IndictmentNumber").OfTypeUInt64().BeingNonNullable().And
                .HaveField("Indictment.Defendant").OfTypeText().BeingNonNullable().And
                .HaveField("Item.Classification").OfTypeEnumeration(
                    Indictment.Category.Infraction,
                    Indictment.Category.Misdemeanor,
                    Indictment.Category.Felony
                ).BeingNonNullable().And
                .HaveField("Item.Statute").OfTypeText().BeingNonNullable().And
                .HaveField("Item.Counts").OfTypeUInt32().BeingNonNullable().And
                .HaveField("Item.CarriedBy.IndictmentNumber").OfTypeUInt64().BeingNonNullable().And
                .HaveField("Item.CarriedBy.Defendant").OfTypeText().BeingNonNullable().And
                .HaveNoOtherFields().And
                .HaveForeignKey("Indictment.Defendant", "Indictment.IndictmentNumber")
                    .Against(translation.Principal.Table)
                    .WithOnDeleteBehavior(OnDelete.Cascade)
                    .WithOnUpdateBehavior(OnUpdate.Cascade).And
                .HaveForeignKey("Item.CarriedBy.Defendant", "Item.CarriedBy.IndictmentNumber")
                    .Against(translation.Principal.Table)
                    .WithOnDeleteBehavior(OnDelete.Cascade)
                    .WithOnUpdateBehavior(OnUpdate.Cascade).And
                .HaveNoOtherForeignKeys();
        }

        [TestMethod] public void RelationReferenceCycle_ReferenceElement() {
            // Arrange
            var translator = new Translator();
            var source = typeof(StackFrame);

            // Act
            var translation = translator[source];

            // Assert
            translation.Relations.Should().HaveCount(1);
            translation.Relations[0].Table.Should()
                .HaveName("UT.Kvasir.Translation.ReferenceCycles+StackFrame.BreakpointsTable").And
                .HaveField("StackFrame.ID").OfTypeGuid().BeingNonNullable().And
                .HaveField("Item.FileName").OfTypeText().BeingNonNullable().And
                .HaveField("Item.LineNumber").OfTypeUInt32().BeingNonNullable().And
                .HaveNoOtherFields().And
                .HaveForeignKey("StackFrame.ID")
                    .Against(translation.Principal.Table)
                    .WithOnDeleteBehavior(OnDelete.Cascade)
                    .WithOnUpdateBehavior(OnUpdate.Cascade).And
                .HaveForeignKey("Item.FileName", "Item.LineNumber")
                    .Against(translator[typeof(StackFrame.Breakpoint)].Principal.Table)
                    .WithOnDeleteBehavior(OnDelete.Cascade)
                    .WithOnUpdateBehavior(OnUpdate.Cascade).And
                .HaveNoOtherForeignKeys();
        }

        [TestMethod] public void SystemReferenceCycle_ReferenceRelation() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Filibuster);

            // Act
            var outerTranslation = translator[source];
            var innerTranslation = translator[typeof(Filibuster.Politician)];

            // Assert
            outerTranslation.Relations.Should().BeEmpty();
            innerTranslation.Relations.Should().HaveCount(1);
            innerTranslation.Relations[0].Table.Should()
                .HaveName("UT.Kvasir.Translation.ReferenceCycles+Filibuster+Politician.FilibustersBrokenTable").And
                .HaveField("Politician.FullName").OfTypeText().BeingNonNullable().And
                .HaveField("Item.FilibusterID").OfTypeGuid().BeingNonNullable().And
                .HaveNoOtherFields().And
                .HaveForeignKey("Politician.FullName")
                    .Against(innerTranslation.Principal.Table)
                    .WithOnDeleteBehavior(OnDelete.Cascade)
                    .WithOnUpdateBehavior(OnUpdate.Cascade).And
                .HaveForeignKey("Item.FilibusterID")
                    .Against(outerTranslation.Principal.Table)
                    .WithOnDeleteBehavior(OnDelete.Cascade)
                    .WithOnUpdateBehavior(OnUpdate.Cascade).And
                .HaveNoOtherForeignKeys();
        }
    }
}
