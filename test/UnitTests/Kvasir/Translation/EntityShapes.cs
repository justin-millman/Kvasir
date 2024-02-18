using FluentAssertions;
using Kvasir.Exceptions;
using Kvasir.Translation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;

using static UT.Kvasir.Translation.EntityShapes;

namespace UT.Kvasir.Translation {
    [TestClass, TestCategory("Entity Shapes")]
    public class EntityShapeTests {
        [TestMethod] public void EntityTypeIsRecordClass() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Color);

            // Act
            var translation = translator[source];

            // Assert
            translation.CLRSource.Should().Be(source);
            translation.Principal.Table.Name.Should().Be("UT.Kvasir.Translation.EntityShapes+ColorTable");
            translation.Principal.Table.Should()
                .HaveField(nameof(Color.Red)).OfTypeUInt8().BeingNonNullable().And
                .HaveField(nameof(Color.Green)).OfTypeUInt8().BeingNonNullable().And
                .HaveField(nameof(Color.Blue)).OfTypeUInt8().BeingNonNullable().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void EntityTypeIsPartialClass() {
            // Arrange
            var translator = new Translator();
            var source = typeof(PresidentialElection);

            // Act
            var translation = translator[source];

            // Assert
            translation.CLRSource.Should().Be(source);
            translation.Principal.Table.Name.Should().Be("UT.Kvasir.Translation.EntityShapes+PresidentialElectionTable");
            translation.Principal.Table.Should()
                .HaveField(nameof(PresidentialElection.Year)).OfTypeUInt16().BeingNonNullable().And
                .HaveField(nameof(PresidentialElection.DemocraticCandidate)).OfTypeText().BeingNonNullable().And
                .HaveField(nameof(PresidentialElection.DemocraticPVs)).OfTypeUInt64().BeingNonNullable().And
                .HaveField(nameof(PresidentialElection.DemocraticEVs)).OfTypeUInt16().BeingNonNullable().And
                .HaveField(nameof(PresidentialElection.RepublicanCandidate)).OfTypeText().BeingNonNullable().And
                .HaveField(nameof(PresidentialElection.RepublicanPVs)).OfTypeUInt64().BeingNonNullable().And
                .HaveField(nameof(PresidentialElection.RepublicanEVs)).OfTypeUInt16().BeingNonNullable().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void EntityTypeIsStaticClass_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(HighHell);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining("cannot be an Entity type")                  // category
                .WithMessageContaining("static");                                   // details / explanation
        }

        [TestMethod] public void EntityTypeIsPrivate() {
            // Arrange
            var translator = new Translator();
            var source = typeof(EntityShapes).GetNestedType("GitCommit", BindingFlags.NonPublic)!;

            // Act
            var translation = translator[source];

            // Assert
            translation.CLRSource.Should().Be(source);
            translation.Principal.Table.Name.Should().Be("UT.Kvasir.Translation.EntityShapes+GitCommitTable");
            translation.Principal.Table.Should()
                .HaveField("Hash").OfTypeText().BeingNonNullable().And
                .HaveField("Author").OfTypeText().BeingNonNullable().And
                .HaveField("Message").OfTypeText().BeingNonNullable().And
                .HaveField("Timestamp").OfTypeDateTime().BeingNonNullable().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void EntityTypeIsInternal() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Belt);

            // Act
            var translation = translator[source];

            // Assert
            translation.CLRSource.Should().Be(source);
            translation.Principal.Table.Name.Should().Be("UT.Kvasir.Translation.EntityShapes+BeltTable");
            translation.Principal.Table.Should()
                .HaveField(nameof(Belt.BeltID)).OfTypeGuid().BeingNonNullable().And
                .HaveField(nameof(Belt.Length)).OfTypeDecimal().BeingNonNullable().And
                .HaveField(nameof(Belt.NumHoles)).OfTypeInt8().BeingNonNullable().And
                .HaveField(nameof(Belt.IsBuckled)).OfTypeBoolean().BeingNonNullable().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void EntityTypeIsStruct_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Carbohydrate);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining("cannot be an Entity type")                  // category
                .WithMessageContaining("struct");                                   // details / explanation
        }

        [TestMethod] public void EntityTypeIsRecordStruct_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(AminoAcid);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining("cannot be an Entity type")                  // category
                .WithMessageContaining("record struct");                            // details / explanation
        }

        [TestMethod] public void EntityTypeIsAbstractClass_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(SuperBowl);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining("cannot be an Entity type")                  // category
                .WithMessageContaining("abstract");                                 // details / explanation
        }

        [TestMethod] public void EntityTypeIsOpenGeneric_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Speedometer<>);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining("cannot be an Entity type")                  // category
                .WithMessageContaining("open generic");                             // details / explanation
        }

        [TestMethod] public void EntityTypeIsClosedGeneric_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Speedometer<double>);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining("cannot be an Entity type")                  // category
                .WithMessageContaining("closed generic");                             // details / explanation
        }

        [TestMethod] public void EntityTypeIsInterface_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(ILiquor);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining("cannot be an Entity type")                  // category
                .WithMessageContaining("interface");                                // details / explanation
        }

        [TestMethod] public void EntityTypeIsEnumeration_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Season);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining("cannot be an Entity type")                  // category
                .WithMessageContaining("enumeration");                              // details / explanation
        }

        [TestMethod] public void EntityTypeIsDelegate_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(SurfingManeuver);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining("cannot be an Entity type")                  // category
                .WithMessageContaining("delegate");                                 // details / explanation
        }
    }
}
