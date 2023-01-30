using Cybele.Extensions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

using BF = System.Reflection.BindingFlags;

namespace UT.Cybele.Extensions {
    [TestClass, TestCategory("MethodInfo: IsInherited")]
    public sealed class MethodInfo_IsInherited : ExtensionTests {
        [TestMethod] public void FirstDeclarationNonVirtualMethodIsNotInherited() {
            // Arrange
            var method = typeof(Base).GetMethod(nameof(Base.NonVirtualFunction))!;

            // Act
            var inherited = method.IsInherited();

            // Assert
            inherited.Should().BeFalse();
        }

        [TestMethod] public void InheritedNonVirtualMethodIsInherited() {
            // Arrange
            var method = typeof(Derived).GetMethod(nameof(Derived.NonVirtualFunction))!;

            // Act
            var inherited = method.IsInherited();

            // Assert
            inherited.Should().BeTrue();
        }

        [TestMethod] public void FirstDeclarationAbstractMethodIsNotInherited() {
            // Arrange
            var method = typeof(Base).GetMethod(nameof(Base.AbstractFunction))!;

            // Act
            var inherited = method.IsInherited();

            // Assert
            inherited.Should().BeFalse();
        }

        [TestMethod] public void FirstDeclarationVirtualMethodIsNotInherited() {
            // Arrange
            var method = typeof(Base).GetMethod(nameof(Base.VirtualFunction))!;

            // Act
            var inherited = method.IsInherited();

            // Assert
            inherited.Should().BeFalse();
        }

        [TestMethod] public void OverriddenVirtualMethodIsInherited() {
            // Arrange
            var method = typeof(Derived).GetMethod(nameof(Derived.VirtualFunction))!;

            // Act
            var inherited = method.IsInherited();

            // Assert
            inherited.Should().BeTrue();
        }

        [TestMethod] public void OverriddenAbstractMethodIsInherited() {
            // Arrange
            var method = typeof(Derived).GetMethod(nameof(Derived.AbstractFunction))!;

            // Act
            var inherited = method.IsInherited();

            // Assert
            inherited.Should().BeTrue();
        }

        [TestMethod] public void InheritedVirtualMethodIsInherited() {
            // Arrange
            var attrs = BF.Public | BF.NonPublic | BF.Instance | BF.Static | BF.FlattenHierarchy;
            var methods = typeof(MoreDerived).GetMethods(attrs);
            var method = methods.Where(m => m.Name.Contains(nameof(Base.NotOverriddenVirtualFunction))).First()!;

            // Act
            var inherited = method.IsInherited();

            // Assert
            inherited.Should().BeTrue();
        }

        [TestMethod] public void HidingVirtualMethodIsNotInherited() {
            // Arrange
            var method = typeof(Derived).GetMethod(nameof(Derived.ToBeHidden))!;

            // Act
            var inherited = method.IsInherited();

            // Assert
            inherited.Should().BeFalse();
        }

        [TestMethod] public void HiddenVirtualMethodIsInherited() {
            // Arrange
            var methods = typeof(Derived).GetMethods();
            var method = methods.Where(m => m.Name == nameof(Base.ToBeHidden) && m.DeclaringType == typeof(Base)).First()!;

            // Act
            var inherited = method.IsInherited();

            // Assert
            inherited.Should().BeTrue();
        }

        [TestMethod] public void DeclaredMethodInInterfaceIsNotInherited() {
            // Arrange
            var method = typeof(IInterface).GetMethod(nameof(IInterface.ImplicitInterfaceFunction))!;

            // Act
            var inherited = method.IsInherited();

            // Assert
            inherited.Should().BeFalse();
        }

        [TestMethod] public void ImplicitInterfaceImplementationIsInherited() {
            // Arrange
            var method = typeof(Derived).GetMethod(nameof(Derived.ImplicitInterfaceFunction))!;

            // Act
            var inherited = method.IsInherited();

            // Assert
            inherited.Should().BeTrue();
        }

        [TestMethod] public void ExplicitInterfaceImplementationIsInherited() {
            // Arrange
            var attrs = BF.Public | BF.NonPublic | BF.Instance | BF.Static | BF.FlattenHierarchy;
            var methods = typeof(Derived).GetMethods(attrs);
            var method = methods.Where(m => m.Name.Contains(nameof(IInterface.ExplicitInterfaceFunction))).First()!;

            // Act
            var inherited = method.IsInherited();

            // Assert
            inherited.Should().BeTrue();
        }

        [TestMethod] public void InheritedImplicitInterfaceImplementationIsInherited() {
            // Arrange
            var method = typeof(MoreDerived).GetMethod(nameof(MoreDerived.ImplicitInterfaceFunction))!;

            // Act
            var inherited = method.IsInherited();

            // Assert
            inherited.Should().BeTrue();
        }

        [TestMethod] public void InheritedExplicitInterfaceIplementationIsInherited() {
            // Arrange
            var attrs = BF.Public | BF.NonPublic | BF.Instance | BF.Static | BF.FlattenHierarchy;
            var methods = typeof(MoreDerived).GetMethods(attrs);
            var method = methods.Where(m => m.Name.Contains(nameof(IInterface.ExplicitInterfaceFunction))).First()!;

            // Act
            var inherited = method.IsInherited();

            // Assert
            inherited.Should().BeTrue();
        }

        [TestMethod] public void FirstDeclarationStaticMethodIsNotInherited() {
            // Arrange
            var method = typeof(Derived).GetMethod(nameof(Derived.Static))!;

            // Assert
            var inherited = method.IsInherited();

            // Assert
            inherited.Should().BeFalse();
        }

        [TestMethod] public void InheritedStaticMethodIsInherited() {
            // Arrange
            var attrs = BF.Public | BF.NonPublic | BF.Instance | BF.Static | BF.FlattenHierarchy;
            var methods = typeof(MoreDerived).GetMethods(attrs);
            var method = methods.Where(m => m.Name.Contains(nameof(Derived.Static))).First()!;

            // Assert
            var inherited = method.IsInherited();

            // Assert
            inherited.Should().BeTrue();
        }

        [TestMethod] public void ConstructIsNotInherited() {
            // Arrange
            var method = typeof(Derived).GetConstructors()[0];

            // Act
            var inherited = method.IsInherited();

            // Assert
            inherited.Should().BeFalse();
        }
    }
}
