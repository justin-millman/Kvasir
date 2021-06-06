using Cybele.Extensions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace UT.Cybele.Extensions {
    [TestClass, TestCategory("Type: IsInstanceOf")]
    public sealed class Type_IsInstanceOf : ExtensionTests {
        [TestMethod] public void Identity() {
            // Arrange
            var type = typeof(string);

            // Act
            var result = type.IsInstanceOf(type);

            // Assert
            result.Should().BeTrue();
        }

        [TestMethod] public void DerivedClassIsInstanceOfBase() {
            // Arrange
            var parent = typeof(Exception);
            var child = typeof(ArgumentNullException);

            // Act
            var result = child.IsInstanceOf(parent);

            // Assert
            result.Should().BeTrue();
        }

        [TestMethod] public void BaseClassIsNotInstanceOfDerived() {
            // Arrange
            var parent = typeof(Exception);
            var child = typeof(ArgumentNullException);

            // Act
            var result = parent.IsInstanceOf(child);

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod] public void ImplementationIsInstanceOfInterface() {
            // Arrange
            var intfc = typeof(IEquatable<string>);
            var impl = typeof(string);

            // Act
            var result = impl.IsInstanceOf(intfc);

            // Assert
            result.Should().BeTrue();
        }

        [TestMethod] public void InterfaceIsNotInstanceOfImplementation() {
            // Arrange
            var intfc = typeof(IEquatable<string>);
            var impl = typeof(string);

            // Act
            var result = intfc.IsInstanceOf(impl);

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod] public void WhatIsInstanceOfObject() {
            // Arrange
            var obj = typeof(object);

            // Act
            var classResult = typeof(string).IsInstanceOf(obj);
            var structResult = typeof(DateTime).IsInstanceOf(obj);
            var primitiveResult = typeof(short).IsInstanceOf(obj);
            var enumResult = typeof(Color).IsInstanceOf(obj);
            var delegateResult = typeof(Func<int, int>).IsInstanceOf(obj);
            var interfaceResult = typeof(IDisposable).IsInstanceOf(obj);

            // Assert
            classResult.Should().BeTrue();
            structResult.Should().BeTrue();
            primitiveResult.Should().BeTrue();
            enumResult.Should().BeTrue();
            delegateResult.Should().BeTrue();
            interfaceResult.Should().BeTrue();
        }

        [TestMethod] public void WhatIsInstanceOfValueType() {
            // Arrange
            var vt = typeof(ValueType);

            // Act
            var classResult = typeof(string).IsInstanceOf(vt);
            var structResult = typeof(DateTime).IsInstanceOf(vt);
            var primitiveResult = typeof(short).IsInstanceOf(vt);
            var enumResult = typeof(Color).IsInstanceOf(vt);
            var delegateResult = typeof(Func<int, int>).IsInstanceOf(vt);
            var interfaceResult = typeof(IDisposable).IsInstanceOf(vt);

            // Assert
            classResult.Should().BeFalse();
            structResult.Should().BeTrue();
            primitiveResult.Should().BeTrue();
            enumResult.Should().BeTrue();
            delegateResult.Should().BeFalse();
            interfaceResult.Should().BeFalse();
        }

        [TestMethod] public void WhatIsInstanceOfEnum() {
            // Arrange
            var enumeration = typeof(Enum);

            // Act
            var classResult = typeof(string).IsInstanceOf(enumeration);
            var structResult = typeof(DateTime).IsInstanceOf(enumeration);
            var primitiveResult = typeof(short).IsInstanceOf(enumeration);
            var enumResult = typeof(Color).IsInstanceOf(enumeration);
            var delegateResult = typeof(Func<int, int>).IsInstanceOf(enumeration);
            var interfaceResult = typeof(IDisposable).IsInstanceOf(enumeration);

            // Assert
            classResult.Should().BeFalse();
            structResult.Should().BeFalse();
            primitiveResult.Should().BeFalse();
            enumResult.Should().BeTrue();
            delegateResult.Should().BeFalse();
            interfaceResult.Should().BeFalse();
        }

        [TestMethod] public void WhatIsInstanceOfDelegate() {
            // Arrange
            var dgte = typeof(Delegate);

            // Act
            var classResult = typeof(string).IsInstanceOf(dgte);
            var structResult = typeof(DateTime).IsInstanceOf(dgte);
            var primitiveResult = typeof(short).IsInstanceOf(dgte);
            var enumResult = typeof(Color).IsInstanceOf(dgte);
            var delegateResult = typeof(Func<int, int>).IsInstanceOf(dgte);
            var interfaceResult = typeof(IDisposable).IsInstanceOf(dgte);

            // Assert
            classResult.Should().BeFalse();
            structResult.Should().BeFalse();
            primitiveResult.Should().BeFalse();
            enumResult.Should().BeFalse();
            delegateResult.Should().BeTrue();
            interfaceResult.Should().BeFalse();
        }

        [TestMethod] public void NonNullableIsInstanceOfNullable() {
            // Arrange
            var parent = typeof(char?);
            var child = typeof(char);

            // Act
            var result = child.IsInstanceOf(parent);

            // Assert
            result.Should().BeTrue();
        }
    }
}
