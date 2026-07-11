using FluentAssertions;
using Kvasir.Administration;
using Kvasir.Translation;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;

using static UT.Kvasir.Translation.Globals;
using static UT.Kvasir.Translation.EntityShapes;
using static UT.Kvasir.Translation.TestLocalizations;

namespace UT.Kvasir.Translation {
    [TestClass, TestCategory("Entity Shapes")]
    public class EntityShapeTests {
        [TestMethod] public void EntityTypeIsAdministrative() {
            var translator = new Translator(NO_ENTITIES, NullLogger.Instance);
            var source = typeof(TableHash);

            // Act
            var translation = translator[source];

            // Assert
            translation.CLRSource.Should().Be(source);
            translation.Principal.Table.Name.Should().Be("_Kvasir_TableSchemaAdmin");
            translation.Principal.Table.Should()
                .HaveField("TableName").OfTypeText().BeingNonNullable().And
                .HaveField("Hash").OfTypeInt32().BeingNonNullable().And
                .HaveNoOtherFields();
            translation.Principal.PreDefinedInstances.Should().BeEmpty();
        }

        [TestMethod] public void EntityTypeIsRecordClass() {
            // Arrange
            var translator = new Translator(NO_ENTITIES, NullLogger.Instance);
            var source = typeof(Color);

            // Act
            var translation = translator[source];

            // Assert
            translation.CLRSource.Should().Be(source);
            translation.Principal.Table.Name.Should().Be("UT.Kvasir.Translation.EntityShapes+ColorTable");
            translation.Principal.Table.Should()
                .HaveField("Red").OfTypeUInt8().BeingNonNullable().And
                .HaveField("Green").OfTypeUInt8().BeingNonNullable().And
                .HaveField("Blue").OfTypeUInt8().BeingNonNullable().And
                .HaveNoOtherFields();
            translation.Principal.PreDefinedInstances.Should().BeEmpty();
        }

        [TestMethod] public void EntityTypeIsPartialClass() {
            // Arrange
            var translator = new Translator(NO_ENTITIES, NullLogger.Instance);
            var source = typeof(PresidentialElection);

            // Act
            var translation = translator[source];

            // Assert
            translation.CLRSource.Should().Be(source);
            translation.Principal.Table.Name.Should().Be("UT.Kvasir.Translation.EntityShapes+PresidentialElectionTable");
            translation.Principal.Table.Should()
                .HaveField("Year").OfTypeUInt16().BeingNonNullable().And
                .HaveField("DemocraticCandidate").OfTypeText().BeingNonNullable().And
                .HaveField("DemocraticPVs").OfTypeUInt64().BeingNonNullable().And
                .HaveField("DemocraticEVs").OfTypeUInt16().BeingNonNullable().And
                .HaveField("RepublicanCandidate").OfTypeText().BeingNonNullable().And
                .HaveField("RepublicanPVs").OfTypeUInt64().BeingNonNullable().And
                .HaveField("RepublicanEVs").OfTypeUInt16().BeingNonNullable().And
                .HaveNoOtherFields();
            translation.Principal.PreDefinedInstances.Should().BeEmpty();
        }

        [TestMethod] public void EntityTypeIsStaticClass_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES, NullLogger.Instance);
            var source = typeof(HighHell);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidEntityTypeException>()
                .WithLocation("`HighHell`")
                .WithProblem("a static class cannot be an Entity type")
                .EndMessage();
        }

        [TestMethod] public void EntityTypeIsPrivate() {
            // Arrange
            var translator = new Translator(NO_ENTITIES, NullLogger.Instance);
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
            translation.Principal.PreDefinedInstances.Should().BeEmpty();
        }

        [TestMethod] public void EntityTypeIsInternal() {
            // Arrange
            var translator = new Translator(NO_ENTITIES, NullLogger.Instance);
            var source = typeof(Belt);

            // Act
            var translation = translator[source];

            // Assert
            translation.CLRSource.Should().Be(source);
            translation.Principal.Table.Name.Should().Be("UT.Kvasir.Translation.EntityShapes+BeltTable");
            translation.Principal.Table.Should()
                .HaveField("BeltID").OfTypeGuid().BeingNonNullable().And
                .HaveField("Length").OfTypeDecimal().BeingNonNullable().And
                .HaveField("NumHoles").OfTypeInt8().BeingNonNullable().And
                .HaveField("IsBuckled").OfTypeBoolean().BeingNonNullable().And
                .HaveNoOtherFields();
            translation.Principal.PreDefinedInstances.Should().BeEmpty();
        }

        [TestMethod] public void EntityTypeIsLocalization() {
            // Arrange
            var translator = new Translator(NO_ENTITIES, NullLogger.Instance);
            var source = typeof(MatchPair);

            // Act
            var translation = translator[source, Translator.AsLocalzation];

            // Assert
            translation.CLRSource.Should().Be(source);
            translation.Principal.Table.Name.Should().Be("UT.Kvasir.Translation.EntityShapes+MatchPairTable");
            translation.Principal.Table.Should()
                .HaveField("Key").OfTypeUInt32().BeingNonNullable().And
                .HaveField("Locale").OfTypeEnumeration(
                    Language.English,
                    Language.Spanish,
                    Language.French,
                    Language.Hebrew,
                    Language.Hindi,
                    Language.German,
                    Language.Arabic,
                    Language.Japanese,
                    Language.Esperanto,
                    Language.Italian
                ).BeingNonNullable().And
                .HaveField("Value").OfTypeText().BeingNonNullable().And
                .HaveNoOtherFields();
            translation.Principal.PreDefinedInstances.Should().BeEmpty();
        }

        [TestMethod] public void EntityTypeIsStruct_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES, NullLogger.Instance);
            var source = typeof(Carbohydrate);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidEntityTypeException>()
                .WithLocation("`Carbohydrate`")
                .WithProblem("a struct or a record struct cannot be an Entity type")
                .EndMessage();
        }

        [TestMethod] public void EntityTypeIsRecordStruct_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES, NullLogger.Instance);
            var source = typeof(AminoAcid);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidEntityTypeException>()
                .WithLocation("`AminoAcid`")
                .WithProblem("a struct or a record struct cannot be an Entity type")
                .EndMessage();
        }

        [TestMethod] public void EntityTypeIsAbstractClass_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES, NullLogger.Instance);
            var source = typeof(SuperBowl);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidEntityTypeException>()
                .WithLocation("`SuperBowl`")
                .WithProblem("an abstract class cannot be an Entity type")
                .EndMessage();
        }

        [TestMethod] public void EntityTypeIsAbstractLocalization_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES, NullLogger.Instance);
            var source = typeof(Trademark);

            // Act
            var translate = () => translator[source, Translator.AsLocalzation];

            // Assert
            translate.Should().FailWith<InvalidEntityTypeException>()
                .WithLocation("`Trademark`")
                .WithProblem("an abstract class cannot be an Entity type")
                .EndMessage();
        }

        [TestMethod] public void EntityTypeIsOpenGeneric_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES, NullLogger.Instance);
            var source = typeof(Speedometer<>);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidEntityTypeException>()
                .WithLocation("`Speedometer<>`")
                .WithProblem("an open generic type cannot be an Entity type")
                .EndMessage();
        }

        [TestMethod] public void EntityTypeIsOpenGenericLocalization_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES, NullLogger.Instance);
            var source = typeof(Emoticon<>);

            // Act
            var translate = () => translator[source, Translator.AsLocalzation];

            // Assert
            translate.Should().FailWith<InvalidEntityTypeException>()
                .WithLocation("`Emoticon<>`")
                .WithProblem("an open generic type cannot be an Entity type")
                .EndMessage();
        }

        [TestMethod] public void EntityTypeIsClosedGeneric() {
            // Arrange
            var translator = new Translator(NO_ENTITIES, NullLogger.Instance);
            var source = typeof(Speedometer<double>);

            // Act
            var translation = translator[source];

            // Assert
            translation.CLRSource.Should().Be(source);
            translation.Principal.Table.Name.Should().Be("UT.Kvasir.Translation.EntityShapes+Speedometer<Double>Table");
            translation.Principal.Table.Should()
                .HaveField("MinSpeed").OfTypeDouble().BeingNonNullable().And
                .HaveField("MaxSpeed").OfTypeDouble().BeingNonNullable().And
                .HaveField("Brand").OfTypeText().BeingNonNullable().And
                .HaveNoOtherFields();
            translation.Principal.PreDefinedInstances.Should().BeEmpty();
        }

        [TestMethod] public void EntityTypeIsInterface_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES, NullLogger.Instance);
            var source = typeof(ILiquor);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidEntityTypeException>()
                .WithLocation("`ILiquor`")
                .WithProblem("an interface cannot be an Entity type")
                .EndMessage();
        }

        [TestMethod] public void EntityTypeIsEnumeration_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES, NullLogger.Instance);
            var source = typeof(Season);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidEntityTypeException>()
                .WithLocation("`Season`")
                .WithProblem("an enumeration type cannot be an Entity type")
                .EndMessage();
        }

        [TestMethod] public void EntityTypeIsDelegate_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES, NullLogger.Instance);
            var source = typeof(SurfingManeuver);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidEntityTypeException>()
                .WithLocation("`SurfingManeuver`")
                .WithProblem("a delegate cannot be an Entity type")
                .EndMessage();
        }
    }
}
