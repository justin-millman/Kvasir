using Cybele.Extensions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace UT.Cybele.Extensions {
    [TestClass, TestCategory("Type: IsInstanceOf")]
    public sealed class Type_IsInstanceOf : ExtensionTests {
        [TestMethod] public void Identity() {
            // Arrange
            var classType = typeof(string);
            var structType = typeof(DateTime);
            var primitiveType = typeof(ulong);
            var nullableType = typeof(int?);
            var enumType = typeof(Color);
            var delegateType = typeof(Predicate<bool>);
            var interfaceType = typeof(ICustomFormatter);
            var openGenericType = typeof(Lazy<>);

            // Act
            var classResult = classType.IsInstanceOf(classType);
            var structResult = structType.IsInstanceOf(structType);
            var primitiveResult = primitiveType.IsInstanceOf(primitiveType);
            var nullableResult = nullableType.IsInstanceOf(nullableType);
            var enumResult = enumType.IsInstanceOf(enumType);
            var delegateResult = delegateType.IsInstanceOf(delegateType);
            var interfaceResult = interfaceType.IsInstanceOf(interfaceType);
            var openGenericResult = openGenericType.IsInstanceOf(openGenericType);

            // Assert
            classResult.Should().BeTrue();
            structResult.Should().BeTrue();
            primitiveResult.Should().BeTrue();
            nullableResult.Should().BeTrue();
            enumResult.Should().BeTrue();
            delegateResult.Should().BeTrue();
            interfaceResult.Should().BeTrue();
            openGenericResult.Should().BeTrue();
        }

        [TestMethod] public void DerivedTypeIsInstanceOfBase() {
            // Arrange
            var parent = typeof(Exception);
            var child = typeof(ArgumentNullException);

            // Act
            var result = child.IsInstanceOf(parent);

            // Assert
            result.Should().BeTrue();
        }

        [TestMethod] public void BaseTypeIsNotInstanceOfDerived() {
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
            var nullablePrimitive = typeof(char?);
            var nullableStruct = typeof(DateTime?);
            var nullableEnum = typeof(Color?);
            var nonNullablePrimitive = typeof(char);
            var nonNullableStruct = typeof(DateTime);
            var nonNullableEnum = typeof(Color);

            // Act
            var primitiveResult = nonNullablePrimitive.IsInstanceOf(nullablePrimitive);
            var structResult = nonNullableStruct.IsInstanceOf(nullableStruct);
            var enumResult = nonNullableEnum.IsInstanceOf(nullableEnum);

            // Assert
            primitiveResult.Should().BeTrue();
            structResult.Should().BeTrue();
            enumResult.Should().BeTrue();
        }

        [TestMethod] public void NullableIsNotInstanceOfNonNullable() {
            // Arrange
            var nullablePrimitive = typeof(char?);
            var nullableStruct = typeof(DateTime?);
            var nullableEnum = typeof(Color?);
            var nonNullablePrimitive = typeof(char);
            var nonNullableStruct = typeof(DateTime);
            var nonNullableEnum = typeof(Color);

            // Act
            var primitiveResult = nullablePrimitive.IsInstanceOf(nonNullablePrimitive);
            var structResult = nullableStruct.IsInstanceOf(nonNullableStruct);
            var enumResult = nullableEnum.IsInstanceOf(nonNullableEnum);

            // Assert
            primitiveResult.Should().BeFalse();
            structResult.Should().BeFalse();
            enumResult.Should().BeFalse();
        }

        [TestMethod] public void ClosedGenericIsNotInstanceOfOpenGeneric() {
            // Arrange
            var openGeneric = typeof(ReadOnlySpan<>);
            var closedGeneric = typeof(ReadOnlySpan<char>);

            // Act
            var result = closedGeneric.IsInstanceOf(openGeneric);

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod] public void CovariantCompatibleGenericIsNotInstanceOfBase() {
            // Arrange
            var original = typeof(IReadOnlyCollection<object>);
            var covariant = typeof(IReadOnlyCollection<string?>);

            // Act
            var result = covariant.IsInstanceOf(original);

            // Assert
            result.Should().BeFalse();
        }


        delegate void First(int a, int b, char c);
        delegate void Second(int a, int b, char c);
        [TestMethod] public void EquivalentDelegatesAreNotInstances() {
            // Arrange
            var first = typeof(First);
            var second = typeof(Second);

            // Act
            var result = first.IsInstanceOf(second);

            // Assert
            result.Should().BeFalse();
        }
    }
}
