using Cybele.Extensions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Reflection;

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

    [TestClass, TestCategory("Type: GetPropertyNamed")]
    public sealed class Type_GetPropertyNamed : ExtensionTests {
        [TestMethod] public void PropertyDoesNotExist() {
            // Arrange
            var type = typeof(Foo);
            var propertyName = "fooness";

            // Act
            var property = type.GetPropertyNamed(propertyName);

            // Assert
            property.HasValue.Should().BeFalse();
        }

        [TestMethod] public void PublicInstanceProperty() {
            // Arrange
            var type = typeof(Foo);
            var propertyName = nameof(Foo.PublicInstanceProperty);

            // Act
            var property = type.GetPropertyNamed(propertyName);

            // Assert
            property.HasValue.Should().BeTrue();
            property.Unwrap().Name.Should().Be(propertyName);
            property.Unwrap().DeclaringType.Should().Be(type);
            property.Unwrap().ReflectedType.Should().Be(type);
            property.Unwrap().PropertyType.Should().Be<char>();
        }

        [TestMethod] public void PrivateInstanceProperty() {
            // Arrange
            var type = typeof(Foo);
            var propertyName = "PrivateInstanceProperty";

            // Act
            var property = type.GetPropertyNamed(propertyName);

            // Assert
            property.HasValue.Should().BeTrue();
            property.Unwrap().Name.Should().Be(propertyName);
            property.Unwrap().DeclaringType.Should().Be(type);
            property.Unwrap().ReflectedType.Should().Be(type);
            property.Unwrap().PropertyType.Should().Be<ushort>();
        }

        [TestMethod] public void InternalInstanceProperty() {
            // Arrange
            var type = typeof(Foo);
            var propertyName = nameof(Foo.InternalInstanceProperty);

            // Act
            var property = type.GetPropertyNamed(propertyName);

            // Assert
            property.HasValue.Should().BeTrue();
            property.Unwrap().Name.Should().Be(propertyName);
            property.Unwrap().DeclaringType.Should().Be(type);
            property.Unwrap().ReflectedType.Should().Be(type);
            property.Unwrap().PropertyType.Should().Be<long>();
        }

        [TestMethod] public void ProtectedInstanceProperty() {
            // Arrange
            var type = typeof(Foo);
            var propertyName = "ProtectedInstanceProperty";

            // Act
            var property = type.GetPropertyNamed(propertyName);

            // Assert
            property.HasValue.Should().BeTrue();
            property.Unwrap().Name.Should().Be(propertyName);
            property.Unwrap().DeclaringType.Should().Be(type);
            property.Unwrap().ReflectedType.Should().Be(type);
            property.Unwrap().PropertyType.Should().Be<ulong>();
        }

        [TestMethod] public void PublicStaticProperty() {
            // Arrange
            var type = typeof(Foo);
            var propertyName = nameof(Foo.PublicStaticProperty);

            // Act
            var property = type.GetPropertyNamed(propertyName);

            // Assert
            property.HasValue.Should().BeTrue();
            property.Unwrap().Name.Should().Be(propertyName);
            property.Unwrap().DeclaringType.Should().Be(type);
            property.Unwrap().ReflectedType.Should().Be(type);
            property.Unwrap().PropertyType.Should().Be<uint>();
        }

        [TestMethod] public void PrivateStaticProperty() {
            // Arrange
            var type = typeof(Foo);
            var propertyName = "PrivateStaticProperty";

            // Act
            var property = type.GetPropertyNamed(propertyName);

            // Assert
            property.HasValue.Should().BeTrue();
            property.Unwrap().Name.Should().Be(propertyName);
            property.Unwrap().DeclaringType.Should().Be(type);
            property.Unwrap().ReflectedType.Should().Be(type);
            property.Unwrap().PropertyType.Should().Be<byte>();
        }

        [TestMethod] public void InternalStaticProperty() {
            // Arrange
            var type = typeof(Foo);
            var propertyName = nameof(Foo.InternalStaticProperty);

            // Act
            var property = type.GetPropertyNamed(propertyName);

            // Assert
            property.HasValue.Should().BeTrue();
            property.Unwrap().Name.Should().Be(propertyName);
            property.Unwrap().DeclaringType.Should().Be(type);
            property.Unwrap().ReflectedType.Should().Be(type);
            property.Unwrap().PropertyType.Should().Be<sbyte>();
        }

        [TestMethod] public void ProtectedStaticProperty() {
            // Arrange
            var type = typeof(Foo);
            var propertyName = "ProtectedStaticProperty";

            // Act
            var property = type.GetPropertyNamed(propertyName);

            // Assert
            property.HasValue.Should().BeTrue();
            property.Unwrap().Name.Should().Be(propertyName);
            property.Unwrap().DeclaringType.Should().Be(type);
            property.Unwrap().ReflectedType.Should().Be(type);
            property.Unwrap().PropertyType.Should().Be<decimal>();
        }

        [TestMethod] public void InheritedProperty() {
            // Arrange
            var type = typeof(Foo);
            var propertyName = nameof(Foo.InheritedProperty);

            // Act
            var property = type.GetPropertyNamed(propertyName);

            // Assert
            property.HasValue.Should().BeTrue();
            property.Unwrap().Name.Should().Be(propertyName);
            property.Unwrap().DeclaringType.Should().Be<FooBase>();
            property.Unwrap().ReflectedType.Should().Be(type);
            property.Unwrap().PropertyType.Should().Be<double>();
        }

        [TestMethod] public void OverridingProperty() {
            // Arrange
            var type = typeof(Foo);
            var propertyName = nameof(Foo.VirtualProperty);

            // Act
            var property = type.GetPropertyNamed(propertyName);

            // Assert
            property.HasValue.Should().BeTrue();
            property.Unwrap().Name.Should().Be(propertyName);
            property.Unwrap().DeclaringType.Should().Be<Foo>();
            property.Unwrap().ReflectedType.Should().Be(type);
            property.Unwrap().PropertyType.Should().Be<string>();
        }

        [TestMethod] public void AbstractProperty() {
            // Arrange
            var type = typeof(Foo);
            var propertyName = nameof(Foo.AbstractProperty);

            // Act
            var property = type.GetPropertyNamed(propertyName);

            // Assert
            property.HasValue.Should().BeTrue();
            property.Unwrap().Name.Should().Be(propertyName);
            property.Unwrap().DeclaringType.Should().Be<FooBase>();
            property.Unwrap().ReflectedType.Should().Be(type);
            property.Unwrap().PropertyType.Should().Be<short>();
        }

        [TestMethod] public void ImplicitInterfaceProperty() {
            // Arrange
            var type = typeof(Foo);
            var propertyName = nameof(Foo.ImplicitProperty);

            // Act
            var property = type.GetPropertyNamed(propertyName);

            // Assert
            property.HasValue.Should().BeTrue();
            property.Unwrap().Name.Should().Be(propertyName);
            property.Unwrap().DeclaringType.Should().Be<Foo>();
            property.Unwrap().ReflectedType.Should().Be(type);
            property.Unwrap().PropertyType.Should().Be<int>();
        }

        [TestMethod] public void ExplicitInterfaceProperty() {
            // Arrange
            var type = typeof(Foo);
            var propertyName = nameof(IFoo.ExplicitProperty);

            // Act
            var property = type.GetPropertyNamed(propertyName);

            // Assert
            property.HasValue.Should().BeTrue();
            property.Unwrap().Name.Should().EndWith($".{propertyName}");
            property.Unwrap().DeclaringType.Should().Be<Foo>();
            property.Unwrap().ReflectedType.Should().Be(type);
            property.Unwrap().PropertyType.Should().Be<bool>();
        }

        [TestMethod] public void ExplicitInterfacePropertyAndRegularProperty_IsError() {
            // Arrange
            var type = typeof(AmbiguousFoo);
            var propertyName = nameof(AmbiguousFoo.ExplicitProperty);

            // Act
            var lookup = () => type.GetPropertyNamed(propertyName);

            // Assert
            lookup.Should().ThrowExactly<AmbiguousMatchException>();
        }

        [TestMethod] public void TwoExplicitInterfacePropertiesSameName_IsError() {
            // Arrange
            var type = typeof(AnotherAmbiguousFoo);
            var propertyName = nameof(IFoo.ExplicitProperty);

            // Act
            var lookup = () => type.GetPropertyNamed(propertyName);

            // Assert
            lookup.Should().ThrowExactly<AmbiguousMatchException>();
        }

        [TestMethod] public void NewHidingProperty() {
            // Arrange
            var type = typeof(Foo);
            var propertyName = nameof(Foo.PropertyToBeHidden);

            // Act
            var property = type.GetPropertyNamed(propertyName);

            // Assert
            property.HasValue.Should().BeTrue();
            property.Unwrap().Name.Should().Be(propertyName);
            property.Unwrap().DeclaringType.Should().Be(type);
            property.Unwrap().ReflectedType.Should().Be(type);
            property.Unwrap().PropertyType.Should().Be<Type>();
        }

        [TestMethod] public void InheritedNewHidingProperty() {
            // Arrange
            var type = typeof(Foo);
            var propertyName = nameof(Foo.PropertyToBeCovered);

            // Act
            var property = type.GetPropertyNamed(propertyName);

            // Assert
            property.HasValue.Should().BeTrue();
            property.Unwrap().Name.Should().Be(propertyName);
            property.Unwrap().DeclaringType.Should().Be<FooIntermediate>();
            property.Unwrap().ReflectedType.Should().Be(type);
            property.Unwrap().PropertyType.Should().Be<DateTime>();
        }

        [TestMethod] public void SingleIndexer() {
            // Arrange
            var type = typeof(Foo);
            var propertyName = "Item";

            // Act
            var property = type.GetPropertyNamed(propertyName);

            // Assert
            property.HasValue.Should().BeTrue();
            property.Unwrap().Name.Should().Be(propertyName);
            property.Unwrap().DeclaringType.Should().Be(type);
            property.Unwrap().ReflectedType.Should().Be(type);
            property.Unwrap().PropertyType.Should().Be<bool?>();
        }

        [TestMethod] public void MultipleIndexers_IsError() {
            // Arrange
            var type = typeof(MultiIndexerFoo);
            var propertyName = "Item";

            // Act
            var lookup = () => type.GetPropertyNamed(propertyName);

            // Assert
            lookup.Should().ThrowExactly<AmbiguousMatchException>();
        }

        [TestMethod] public void NonIndexerPropertyNamedItem() {
            // Arrange
            var type = typeof(ItemFoo);
            var propertyName = nameof(ItemFoo.Item);

            // Act
            var property = type.GetPropertyNamed(propertyName);

            // Assert
            property.HasValue.Should().BeTrue();
            property.Unwrap().Name.Should().Be(propertyName);
            property.Unwrap().DeclaringType.Should().Be(type);
            property.Unwrap().ReflectedType.Should().Be(type);
            property.Unwrap().PropertyType.Should().Be<ushort?>();
        }

        [TestMethod] public void ReadOnlyProperty() {
            // Arrange
            var type = typeof(Foo);
            var propertyName = nameof(Foo.ReadOnlyProperty);

            // Act
            var property = type.GetPropertyNamed(propertyName);

            // Assert
            property.HasValue.Should().BeTrue();
            property.Unwrap().Name.Should().Be(propertyName);
            property.Unwrap().DeclaringType.Should().Be(type);
            property.Unwrap().ReflectedType.Should().Be(type);
            property.Unwrap().PropertyType.Should().Be<int?>();
        }

        [TestMethod] public void WriteOnlyProperty() {
            // Arrange
            var type = typeof(Foo);
            var propertyName = nameof(Foo.WriteOnlyProperty);

            // Act
            var property = type.GetPropertyNamed(propertyName);

            // Assert
            property.HasValue.Should().BeTrue();
            property.Unwrap().Name.Should().Be(propertyName);
            property.Unwrap().DeclaringType.Should().Be(type);
            property.Unwrap().ReflectedType.Should().Be(type);
            property.Unwrap().PropertyType.Should().Be<char?>();
        }

        [TestMethod] public void InitOnlyProperty() {
            // Arrange
            var type = typeof(Foo);
            var propertyName = nameof(Foo.InitOnlyProperty);

            // Act
            var property = type.GetPropertyNamed(propertyName);

            // Assert
            property.HasValue.Should().BeTrue();
            property.Unwrap().Name.Should().Be(propertyName);
            property.Unwrap().DeclaringType.Should().Be(type);
            property.Unwrap().ReflectedType.Should().Be(type);
            property.Unwrap().PropertyType.Should().Be<double?>();
        }


        private interface IFoo {
            int ImplicitProperty { get; set; }
            bool ExplicitProperty { get; set; }
        }
        private abstract class FooBase {
            public abstract short AbstractProperty { get; set; }
            public virtual string VirtualProperty { get; set; } = "";
            public double InheritedProperty { get; set; }
            public float PropertyToBeHidden { get; set; }
            public DateOnly PropertyToBeCovered { get; set; }
        }
        private abstract class FooIntermediate : FooBase {
            public new DateTime PropertyToBeCovered { get; set; }
        }
        private abstract class Foo : FooIntermediate, IFoo {
            public char PublicInstanceProperty { get; set; }
            private ushort PrivateInstanceProperty { get; set; }
            internal long InternalInstanceProperty { get; set; }
            protected ulong ProtectedInstanceProperty { get; set; }

            public static uint PublicStaticProperty { get; set; }
            private static byte PrivateStaticProperty { get; set; }
            internal static sbyte InternalStaticProperty { get; set; }
            protected static decimal ProtectedStaticProperty { get; set; }

            public int ImplicitProperty { get; set; }
            bool IFoo.ExplicitProperty { get; set; }
            public override string VirtualProperty { get; set; } = "";
            public new Type PropertyToBeHidden { get; set; } = typeof(void);

            public bool? this[int _] { get => true; set {} }
            public int? ReadOnlyProperty { get; }
            public char? WriteOnlyProperty { set {} }
            public double? InitOnlyProperty { init {} }
        }
        private abstract class MultiIndexerFoo {
            public short? this[int _] { get => 0; set {} }
            public ulong? this[char _] { get => 0; set {} }
        }
        private abstract class ItemFoo {
            public ushort? Item { get; set; }
        }
        private abstract class AmbiguousFoo : IFoo {
            public int ImplicitProperty { get; set; }
            bool IFoo.ExplicitProperty { get; set; }
            public byte? ExplicitProperty { get; set; }
        }
        private interface IFoo2 {
            float? ExplicitProperty { get; set; }
        }
        private abstract class AnotherAmbiguousFoo  : IFoo, IFoo2 {
            public int ImplicitProperty { get; set; }
            bool IFoo.ExplicitProperty { get; set; }
            float? IFoo2.ExplicitProperty { get; set; }
        }
    }
}
