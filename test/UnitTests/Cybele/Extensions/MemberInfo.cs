using Cybele.Extensions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace UT.Cybele.Extensions {
    [TestClass, TestCategory("MemberInfo: HasAttribute")]
    public sealed class MemberInfo_HasAttribute : ExtensionTests {
        [TestMethod] public void EnumHasFlagsAttribute() {
            // Arrange
            var noFlagsEnumType = typeof(Day);
            var flagsEnumType = typeof(Color);

            // Act
            var noFlagsEnumHasAttribute = noFlagsEnumType.HasAttribute<FlagsAttribute>();
            var flagsEnumHasAttribute = flagsEnumType.HasAttribute<FlagsAttribute>();

            // Assert
            noFlagsEnumHasAttribute.Should().BeFalse();
            flagsEnumHasAttribute.Should().BeTrue();
        }

        [TestMethod] public void ClassDoesNotInheritAttribute() {
            // Arrange
            var baseType = typeof(Base);
            var derivedType = typeof(Derived);

            // Act
            var baseHasAttribute = baseType.HasAttribute<UninheritedAttribute>();
            var derivedHasAttribute = derivedType.HasAttribute<UninheritedAttribute>();

            //
            baseHasAttribute.Should().BeTrue();
            derivedHasAttribute.Should().BeFalse();
        }

        [TestMethod] public void ClassDoesInheritAttribute() {
            // Arrange
            var baseType = typeof(Base);
            var derivedType = typeof(Derived);

            // Act
            var baseHasAttribute = baseType.HasAttribute<InheritedAttribute>();
            var derivedHasAttribute = derivedType.HasAttribute<InheritedAttribute>();

            // Assert
            baseHasAttribute.Should().BeTrue();
            derivedHasAttribute.Should().BeTrue();
        }

        [TestMethod] public void MethodDoesNotInheritAttribute() {
            // Arrange
            var baseNonVirtualMethod = typeof(Base).GetMethod(nameof(Base.NonVirtualFunction))!;
            var baseVirtualMethod = typeof(Base).GetMethod(nameof(Base.VirtualFunction))!;
            var derivedNonVirtualMethod = typeof(Derived).GetMethod(nameof(Derived.NonVirtualFunction))!;
            var derivedVirtualMethod = typeof(Derived).GetMethod(nameof(Derived.VirtualFunction))!;

            // Act
            var baseNonVirtualHasAttribute = baseNonVirtualMethod.HasAttribute<UninheritedAttribute>();
            var baseVirtualHasAttribute = baseVirtualMethod.HasAttribute<UninheritedAttribute>();
            var derivedNonVirtualHasAttribute = derivedNonVirtualMethod.HasAttribute<UninheritedAttribute>();
            var derivedVirtualHasAttribute = derivedVirtualMethod.HasAttribute<UninheritedAttribute>();

            // Assert
            baseNonVirtualHasAttribute.Should().BeTrue();
            baseVirtualHasAttribute.Should().BeTrue();
            derivedNonVirtualHasAttribute.Should().BeTrue();
            derivedVirtualHasAttribute.Should().BeFalse();
        }

        [TestMethod] public void MethodDoesInheritAttribute() {
            // Arrange
            var baseNonVirtualMethod = typeof(Base).GetMethod(nameof(Base.NonVirtualFunction))!;
            var baseVirtualMethod = typeof(Base).GetMethod(nameof(Base.VirtualFunction))!;
            var derivedNonVirtualMethod = typeof(Base).GetMethod(nameof(Derived.NonVirtualFunction))!;
            var derivedVirtualMethod = typeof(Base).GetMethod(nameof(Derived.VirtualFunction))!;

            // Act
            var baseNonVirtualHasAttribute = baseNonVirtualMethod.HasAttribute<InheritedAttribute>();
            var baseVirtualHasAttribute = baseVirtualMethod.HasAttribute<InheritedAttribute>();
            var derivedNonVirtualHasAttribute = derivedNonVirtualMethod.HasAttribute<InheritedAttribute>();
            var derivedVirtualHasAttribute = derivedVirtualMethod.HasAttribute<InheritedAttribute>();

            // Assert
            baseNonVirtualHasAttribute.Should().BeTrue();
            baseVirtualHasAttribute.Should().BeTrue();
            derivedNonVirtualHasAttribute.Should().BeTrue();
            derivedVirtualHasAttribute.Should().BeTrue();
        }

        [TestMethod] public void PropertyDoesNotInheritAttribute() {
            // Arrange
            var baseNonVirtualProp = typeof(Base).GetProperty(nameof(Base.NonVirtualProperty))!;
            var baseVirtualProp = typeof(Base).GetProperty(nameof(Base.VirtualProperty))!;
            var derivedNonVirtualProp = typeof(Derived).GetProperty(nameof(Derived.NonVirtualProperty))!;
            var derivedVirtualProp = typeof(Derived).GetProperty(nameof(Derived.VirtualProperty))!;

            // Act
            var baseNonVirtualHasAttribute = baseNonVirtualProp.HasAttribute<UninheritedAttribute>();
            var baseVirtualHasAttribute = baseVirtualProp.HasAttribute<UninheritedAttribute>();
            var derivedNonVirtualHasAttribute = derivedNonVirtualProp.HasAttribute<UninheritedAttribute>();
            var derivedVirtualHasAttribute = derivedVirtualProp.HasAttribute<UninheritedAttribute>();

            // Assert
            baseNonVirtualHasAttribute.Should().BeTrue();
            baseVirtualHasAttribute.Should().BeTrue();
            derivedNonVirtualHasAttribute.Should().BeTrue();
            derivedVirtualHasAttribute.Should().BeFalse();
        }

        [TestMethod] public void PropertyDoesInheritAttribute() {
            // Arrange
            var baseNonVirtualProp = typeof(Base).GetProperty(nameof(Base.NonVirtualProperty))!;
            var baseVirtualProp = typeof(Base).GetProperty(nameof(Base.VirtualProperty))!;
            var derivedNonVirtualProp = typeof(Base).GetProperty(nameof(Derived.NonVirtualProperty))!;
            var derivedVirtualProp = typeof(Base).GetProperty(nameof(Derived.VirtualProperty))!;

            // Act
            var baseNonVirtualHasAttribute = baseNonVirtualProp.HasAttribute<InheritedAttribute>();
            var baseVirtualHasAttribute = baseVirtualProp.HasAttribute<InheritedAttribute>();
            var derivedNonVirtualHasAttribute = derivedNonVirtualProp.HasAttribute<InheritedAttribute>();
            var derivedVirtualHasAttribute = derivedVirtualProp.HasAttribute<InheritedAttribute>();

            // Assert
            baseNonVirtualHasAttribute.Should().BeTrue();
            baseVirtualHasAttribute.Should().BeTrue();
            derivedNonVirtualHasAttribute.Should().BeTrue();
            derivedVirtualHasAttribute.Should().BeTrue();
        }
    }
}
