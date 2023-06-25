using FluentAssertions;
using Kvasir.Exceptions;
using Kvasir.Translation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using static UT.Kvasir.Translation.Nullability;

namespace UT.Kvasir.Translation {
    [TestClass, TestCategory("Nullability")]
    public class NullabilityTests {
        [TestMethod] public void NonNullableScalarsMarkedNullable() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Timestamp);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField(nameof(Timestamp.UnixSinceEpoch)).OfTypeUInt64().BeingNonNullable().And
                .HaveField(nameof(Timestamp.Hour)).OfTypeUInt16().BeingNonNullable().And
                .HaveField(nameof(Timestamp.Minute)).OfTypeUInt16().BeingNonNullable().And
                .HaveField(nameof(Timestamp.Second)).OfTypeUInt16().BeingNonNullable().And
                .HaveField(nameof(Timestamp.Millisecond)).OfTypeUInt16().BeingNullable().And
                .HaveField(nameof(Timestamp.Microsecond)).OfTypeUInt16().BeingNullable().And
                .HaveField(nameof(Timestamp.Nanosecond)).OfTypeUInt16().BeingNullable().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void NullableScalarsMarkedNonNullable() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Bone);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField(nameof(Bone.TA2)).OfTypeUInt32().BeingNonNullable().And
                .HaveField(nameof(Bone.Name)).OfTypeText().BeingNonNullable().And
                .HaveField(nameof(Bone.LatinName)).OfTypeText().BeingNullable().And
                .HaveField(nameof(Bone.MeSH)).OfTypeText().BeingNonNullable().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void NullableScalarsMarkedAsNullable_Redundant() {
            // Arrange
            var translator = new Translator();
            var source = typeof(CivMilitaryUnit);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField(nameof(CivMilitaryUnit.Identifier)).OfTypeText().BeingNonNullable().And
                .HaveField(nameof(CivMilitaryUnit.Promotion)).OfTypeText().BeingNullable().And
                .HaveField(nameof(CivMilitaryUnit.MeleeStrength)).OfTypeUInt8().BeingNonNullable().And
                .HaveField(nameof(CivMilitaryUnit.RangedStrength)).OfTypeUInt8().BeingNullable().And
                .HaveField(nameof(CivMilitaryUnit.IsUnique)).OfTypeBoolean().BeingNonNullable().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void NonNullableScalarsMarkedAsNonNullable_Redundant() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Patent);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField(nameof(Patent.DocumentID)).OfTypeUInt64().BeingNonNullable().And
                .HaveField(nameof(Patent.PublicationDate)).OfTypeDateTime().BeingNonNullable().And
                .HaveField(nameof(Patent.Description)).OfTypeText().BeingNullable().And
                .HaveField(nameof(Patent.ApplicationNumber)).OfTypeUInt64().BeingNonNullable().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void CombinedAnnotation_NullableAndNonNullable_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(RetailProduct);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(RetailProduct.SalePrice))             // error location
                .WithMessageContaining("mutually exclusive")                        // category
                .WithMessageContaining("[Nullable]")                                // details / explanation
                .WithMessageContaining("[NonNullable]");                            // details / explanation
        }
    }
}
