using FluentAssertions;
using Kvasir.Exceptions;
using Kvasir.Translation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using static UT.Kvasir.Translation.FieldNaming;

namespace UT.Kvasir.Translation {
    [TestClass, TestCategory("Field Naming")]
    public class FieldNamingTests {
        [TestMethod] public void NonPascalCasedNames() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Surah);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField(nameof(Surah._EnglishName)).OfTypeText().BeingNonNullable().And
                .HaveField(nameof(Surah.__ArabicName)).OfTypeText().BeingNonNullable().And
                .HaveField(nameof(Surah.juz_start)).OfTypeDecimal().BeingNonNullable().And
                .HaveField(nameof(Surah.juzEnd)).OfTypeDecimal().BeingNonNullable().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void FieldNameChangedToBrandNewIdentifier() {
            // Arrange
            var translator = new Translator();
            var source = typeof(FieldNaming.River);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField(nameof(River.Name)).OfTypeText().BeingNonNullable().And
                .HaveField("SourceElevation").OfTypeUInt16().BeingNonNullable().And
                .HaveField("Length").OfTypeUInt16().BeingNonNullable().And
                .HaveField(nameof(River.MouthLatitude)).OfTypeDecimal().BeingNonNullable().And
                .HaveField(nameof(River.MouthLongitude)).OfTypeDecimal().BeingNonNullable().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void FieldsSwapNames() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Episode);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField("Number").OfTypeUInt8().BeingNonNullable().And
                .HaveField("Season").OfTypeInt16().BeingNonNullable().And
                .HaveField(nameof(Episode.Length)).OfTypeSingle().BeingNonNullable().And
                .HaveField(nameof(Episode.Part)).OfTypeInt32().BeingNullable().And
                .HaveField(nameof(Episode.Title)).OfTypeText().BeingNonNullable().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void ChangeToNameOfExistingField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(ComputerLock);

            // Act
            var translate = () => translator[source];

            // Assert
            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining("two or more Fields with name")              // category
                .WithMessageContaining("\"IsReentrant\"");                          // details / explanation
        }

        [TestMethod] public void TwoFieldsNamesChangedToSameName_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Ticket2RideRoute);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining("two or more Fields with name")              // category
                .WithMessageContaining("\"Destination\"");                          // details / explanation
        }

        [TestMethod] public void MultipleNameChangesOnScalarProperty_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(BankAccount);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(BankAccount.RoutingNumber))           // error location
                .WithMessageContaining("duplicated")                                // category
                .WithMessageContaining("[Name]");                                   // details / explanation
        }

        [TestMethod] public void FieldNameIsUnchangedByAnnotation_Redundant() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Opera);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField(nameof(Opera.ID)).OfTypeGuid().BeingNonNullable().And
                .HaveField(nameof(Opera.Composer)).OfTypeText().BeingNonNullable().And
                .HaveField(nameof(Opera.PremiereDate)).OfTypeDateTime().BeingNonNullable().And
                .HaveField(nameof(Opera.Length)).OfTypeUInt32().BeingNonNullable().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void NewNameIsNull() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Longbow);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Longbow.Weight))                      // error location
                .WithMessageContaining("[Name]")                                    // details / explanation
                .WithMessageContaining("null");                                     // details / explanation
        }

        [TestMethod] public void NewNameIsEmptyString_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Volcano);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Volcano.IsActive))                    // error location
                .WithMessageContaining("[Name]")                                    // details / explanation
                .WithMessageContaining("\"\"");                                     // details / explanation
        }

        [TestMethod] public void PathIsNull_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(MedalOfHonor);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(MedalOfHonor.Recipient))              // error location
                .WithMessageContaining("path is null")                              // category
                .WithMessageContaining("[Name]");                                   // details / explanation
        }

        [TestMethod] public void PathOnScalar_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Legume);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Legume.Energy))                       // error location
                .WithMessageContaining("does not exist")                            // category
                .WithMessageContaining("[Name]")                                    // details / explanation
                .WithMessageContaining("\"---\"");                                  // details / explanation
        }
    }
}
