using Cybele.Core;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;

namespace UT.Cybele.Core {
    [TestClass, TestCategory("PropertyChain")]
    public sealed class PropertyChainTests {
        [TestMethod] public void ConstructFromReadableProperty() {
            void evaluate(string propertyName) {
                // Arrange
                var property = typeof(Leaf).GetProperty(propertyName, ANY_PROPERTY)!;

                // Act
                var chain = new PropertyChain(property);

                // Assert
                chain.ReflectedType.Should().Be(property.ReflectedType);
                chain.PropertyType.Should().Be(property.PropertyType);
                chain.Length.Should().Be(1);
            }

            evaluate("Public");
            evaluate("Private");
            evaluate("Protected");
            evaluate("Internal");
            evaluate("InternalProtected");
            evaluate("StaticPublic");
            evaluate("StaticPrivate");
            evaluate("StaticProtected");
            evaluate("StaticInternal");
            evaluate("StaticInternalProtected");
        }

        [TestMethod] public void ConstructFromNonReadableProperty() {
            // Arrange
            var writeOnlyProperty = typeof(Leaf).GetProperty("WriteOnly", ANY_PROPERTY)!;
            var initOnlyProperty = typeof(Leaf).GetProperty("InitOnly", ANY_PROPERTY)!;

            // Act
            var writeOnlyAction = () => new PropertyChain(writeOnlyProperty);
            var initOnlyAction = () => new PropertyChain(initOnlyProperty);

            // Assert
            writeOnlyAction.Should().ThrowExactly<ArgumentException>().WithAnyMessage();
            initOnlyAction.Should().ThrowExactly<ArgumentException>().WithAnyMessage();
        }

        [TestMethod] public void ConstructFromReadablePropertyName() {
            void evaluate(string propertyName) {
                // Arrange
                var property = typeof(Leaf).GetProperty(propertyName, ANY_PROPERTY)!;

                // Act
                var chain = new PropertyChain(property.ReflectedType!, property.Name);

                // Assert
                chain.ReflectedType.Should().Be(property.ReflectedType);
                chain.PropertyType.Should().Be(property.PropertyType);
                chain.Length.Should().Be(1);
            }

            evaluate("Public");
            evaluate("Private");
            evaluate("Protected");
            evaluate("Internal");
            evaluate("InternalProtected");
            evaluate("StaticPublic");
            evaluate("StaticPrivate");
            evaluate("StaticProtected");
            evaluate("StaticInternal");
            evaluate("StaticInternalProtected");
            evaluate("Implicit");
            evaluate("IntermediateChar");
            evaluate("Virtual");
        }

        [TestMethod] public void ConstructFromHidingPropertyName() {
            // Arrange

            // Act
            var chain = new PropertyChain(typeof(Leaf), nameof(Leaf.Hide));

            // Assert
            chain.ReflectedType.Should().Be(typeof(Leaf));
            chain.PropertyType.Should().Be(typeof(DateTimeOffset));
            chain.Length.Should().Be(1);
        }

        [TestMethod] public void ConstructFromExplicitInterfacePropertyName() {
            // Arrange

            // Act
            var action = () => new PropertyChain(typeof(Leaf), nameof(IExplicit.Explicit));

            // Assert
            action.Should().ThrowExactly<ArgumentException>().WithAnyMessage();
        }

        [TestMethod] public void ConstructFromNonReadablePropertyName() {
            // Arrange

            // Act
            var writeOnlyAction = () => new PropertyChain(typeof(Leaf), "WriteOnly");
            var initOnlyAction = () => new PropertyChain(typeof(Leaf), "InitOnly");

            // Assert
            writeOnlyAction.Should().ThrowExactly<ArgumentException>().WithAnyMessage();
            initOnlyAction.Should().ThrowExactly<ArgumentException>().WithAnyMessage();
        }

        [TestMethod] public void ConstructFromNonExistentPropertyName() {
            // Arrange

            // Act
            var action = () => new PropertyChain(typeof(Leaf), "DOES NOT EXIST");

            // Assert
            action.Should().ThrowExactly<ArgumentException>().WithAnyMessage();
        }

        [TestMethod] public void ImplicitConvertFromReadableProperty() {
            void evaluate(string propertyName) {
                // Arrange
                var property = typeof(Leaf).GetProperty(propertyName, ANY_PROPERTY)!;

                // Act
                PropertyChain chain = property;

                // Assert
                chain.ReflectedType.Should().Be(property.ReflectedType);
                chain.PropertyType.Should().Be(property.PropertyType);
                chain.Length.Should().Be(1);
            }

            evaluate("Public");
            evaluate("Private");
            evaluate("Protected");
            evaluate("Internal");
            evaluate("InternalProtected");
            evaluate("StaticPublic");
            evaluate("StaticPrivate");
            evaluate("StaticProtected");
            evaluate("StaticInternal");
            evaluate("StaticInternalProtected");
        }

        [TestMethod] public void ImplicitConvertFromNonReadableProperty() {
            // Arrange
            var writeOnlyProperty = typeof(Leaf).GetProperty("WriteOnly", ANY_PROPERTY)!;
            var initOnlyProperty = typeof(Leaf).GetProperty("InitOnly", ANY_PROPERTY)!;

            // Act
            var writeOnlyAction = () => (PropertyChain)writeOnlyProperty;
            var initOnlyAction = () => (PropertyChain)initOnlyProperty;

            // Assert
            writeOnlyAction.Should().ThrowExactly<ArgumentException>().WithAnyMessage();
            initOnlyAction.Should().ThrowExactly<ArgumentException>().WithAnyMessage();
        }

        [TestMethod] public void AppendInfoMatchingType() {
            // Scenario: Given a PropertyChain whose current PropertyType is T, append a PropertyInfo that represents a
            //   readable property obtained via reflection on exactly T

            void evaluate(string propertyName) {
                // Arrange
                var original = new PropertyChain(typeof(Leaf), nameof(Leaf.Self));
                var property = typeof(Leaf).GetProperty(propertyName, ANY_PROPERTY)!;

                // Act
                var chain = original.Append(property);

                // Assert
                chain.Should().NotBeSameAs(original);
                chain.ReflectedType.Should().Be(original.ReflectedType);
                chain.PropertyType.Should().Be(property.PropertyType);
                chain.Length.Should().Be(original.Length + 1);
            }

            evaluate("Public");
            evaluate("Private");
            evaluate("Protected");
            evaluate("Internal");
            evaluate("InternalProtected");
            evaluate("StaticPublic");
            evaluate("StaticPrivate");
            evaluate("StaticProtected");
            evaluate("StaticInternal");
            evaluate("StaticInternalProtected");
        }

        [TestMethod] public void AppendInfoDerivedFromType() {
            // Scenario: Given a PropertyChain whose current PropertyType is T, append a PropertyInfo that represents a
            //   readable property obtained via reflection on a type derived from T

            // Arrange
            var original = new PropertyChain(typeof(Leaf), nameof(Leaf.IntermediateSelf));
            var property = typeof(Leaf).GetProperty(nameof(Leaf.Public), ANY_PROPERTY)!;

            // Act
            var action = () => original.Append(property);

            // Assert
            action.Should().ThrowExactly<ArgumentException>().WithAnyMessage();
        }

        [TestMethod] public void AppendInfoBaseOfType() {
            // Scenario: Given a PropertyChain whose current PropertyType is T, append a PropertyInfo that represents a
            //   readable property obtained via reflection on a base class of T

            // Arrange
            var original = new PropertyChain(typeof(Leaf), nameof(Leaf.Self));
            var property = typeof(Base).GetProperty(nameof(Base.Hide), ANY_PROPERTY)!;

            // Act
            var chain = original.Append(property);

            // Assert
            chain.Should().NotBeSameAs(original);
            chain.ReflectedType.Should().Be(original.ReflectedType);
            chain.PropertyType.Should().Be(property.PropertyType);
            chain.Length.Should().Be(original.Length + 1);
        }

        [TestMethod] public void AppendInfoImplementsType() {
            // Scenario: Given a PropertyChain whose current PropertyType is T, append a PropertyInfo that represents a
            //   readable property obtained via reflection on a type that implements T

            // Arrange
            var original = new PropertyChain(typeof(Leaf), nameof(Leaf.InterfaceSelf));
            var property = typeof(Leaf).GetProperty(nameof(Leaf.Public), ANY_PROPERTY)!;

            // Act
            var action = () => original.Append(property);

            // Assert
            action.Should().ThrowExactly<ArgumentException>().WithAnyMessage();
        }

        [TestMethod] public void AppendInfoInterfaceOfType() {
            // Scenario: Given a PropertyChain whose current PropertyType is T, append a PropertyInfo that represents a
            //   readable property obtained via reflection on an interface of T

            // Arrange
            var original = new PropertyChain(typeof(Leaf), nameof(Leaf.Self));
            var property = typeof(IImplicit).GetProperty(nameof(IImplicit.Implicit), ANY_PROPERTY)!;

            // Act
            var chain = original.Append(property);

            // Assert
            chain.Should().NotBeSameAs(original);
            chain.ReflectedType.Should().Be(original.ReflectedType);
            chain.PropertyType.Should().Be(property.PropertyType);
            chain.Length.Should().Be(original.Length + 1);
        }

        [TestMethod] public void AppendInfoUnrelatedType() {
            // Scenario: Given a PropertyChain whose current PropertyType is T, append a PropertyInfo that represents a
            //   readable property obtained via reflection on a type that is completely unrelated to T

            // Arrange
            var original = new PropertyChain(typeof(Leaf), nameof(Leaf.Self));
            var property = typeof(string).GetProperty(nameof(string.Length), ANY_PROPERTY)!;

            // Act
            var action = () => original.Append(property);

            // Assert
            action.Should().ThrowExactly<ArgumentException>().WithAnyMessage();
        }

        [TestMethod] public void AppendNonReadableInfo() {
            // Arrange
            var original = new PropertyChain(typeof(Leaf), nameof(Leaf.Self));
            var writeOnlyProperty = typeof(Leaf).GetProperty("WriteOnly", ANY_PROPERTY)!;
            var initOnlyProperty = typeof(Leaf).GetProperty("InitOnly", ANY_PROPERTY)!;

            // Act
            var writeOnlyAction = () => original.Append(writeOnlyProperty);
            var initOnlyAction = () => original.Append(initOnlyProperty);

            // Assert
            writeOnlyAction.Should().ThrowExactly<ArgumentException>().WithAnyMessage();
            initOnlyAction.Should().ThrowExactly<ArgumentException>().WithAnyMessage();
        }

        [TestMethod] public void AppendNameOfPropertyDefinedByType() {
            // Scenario: Given a PropertyChain whose current PropertyType is T, append a property by name that is first
            //   defined by T

            void evaluate(string propertyName) {
                // Arrange
                var original = new PropertyChain(typeof(Leaf), nameof(Leaf.Self));
                var property = typeof(Leaf).GetProperty(propertyName, ANY_PROPERTY)!;

                // Act
                var chain = original.Append(property.Name);

                // Assert
                chain.Should().NotBeSameAs(original);
                chain.ReflectedType.Should().Be(original.ReflectedType);
                chain.PropertyType.Should().Be(property.PropertyType);
                chain.Length.Should().Be(original.Length + 1);
            }

            evaluate("Public");
            evaluate("Private");
            evaluate("Protected");
            evaluate("Internal");
            evaluate("InternalProtected");
            evaluate("StaticPublic");
            evaluate("StaticPrivate");
            evaluate("StaticProtected");
            evaluate("StaticInternal");
            evaluate("StaticInternalProtected");
        }

        [TestMethod] public void AppendNameOfPropertyInheritedByType() {
            // Scenario: Given a PropertyChain whose current PropertyType is T, append a property by name that is
            //   inherited by T

            // Arrange
            var original = new PropertyChain(typeof(Leaf), nameof(Leaf.Self));
            var property = typeof(Leaf).GetProperty(nameof(Leaf.IntermediateChar), ANY_PROPERTY)!;

            // Act
            var chain = original.Append(property.Name);

            // Assert
            chain.Should().NotBeSameAs(original);
            chain.ReflectedType.Should().Be(original.ReflectedType);
            chain.PropertyType.Should().Be(property.PropertyType);
            chain.Length.Should().Be(original.Length + 1);
        }

        [TestMethod] public void AppendNameOfPropertyHidingByType() {
            // Scenario: Given a PropertyChain whose current PropertyType is T, append a property by name that is
            //   defined on a base class or interface of T and then overridden by T with a different property type

            // Arrange
            var original = new PropertyChain(typeof(Leaf), nameof(Leaf.Self));

            // Act
            var chain = original.Append(nameof(Leaf.Hide));

            // Assert
            chain.Should().NotBeSameAs(original);
            chain.ReflectedType.Should().Be(original.ReflectedType);
            chain.PropertyType.Should().Be(typeof(DateTimeOffset));
            chain.Length.Should().Be(original.Length + 1);
        }

        [TestMethod] public void AppendNameOfExplicitInterfaceProperty() {
            // Arrange
            var original = new PropertyChain(typeof(Leaf), nameof(Leaf.Self));

            // Act
            var action = () => original.Append(nameof(IExplicit.Explicit));

            // Assert
            action.Should().ThrowExactly<ArgumentException>().WithAnyMessage();
        }
        
        [TestMethod] public void AppendNameOfNonExistentProperty() {
            // Arrange
            var original = new PropertyChain(typeof(Leaf), nameof(Leaf.Self));

            // Act
            var action = () => original.Append("DOES NOT EXIST");

            // Assert
            action.Should().ThrowExactly<ArgumentException>().WithAnyMessage();
        }

        [TestMethod] public void AppendNameOfNonReadableProperty() {
            // Arrange
            var original = new PropertyChain(typeof(Leaf), nameof(Leaf.Self));

            // Act
            var writeOnlyAction = () => original.Append("WriteOnly");
            var initOnlyAction = () => original.Append("InitOnly");

            // Assert
            writeOnlyAction.Should().ThrowExactly<ArgumentException>().WithAnyMessage();
            initOnlyAction.Should().ThrowExactly<ArgumentException>().WithAnyMessage();
        }

        [TestMethod] public void AppendChainMatchingType() {
            // Scenario: Given a PropertyChain whose current PropertyType is T, append another PropertyChain whose
            //   ReflectedType is exactly T

            // Arrange
            var original = new PropertyChain(typeof(Leaf), nameof(Leaf.Self));
            var appendage = new PropertyChain(typeof(Leaf), nameof(Leaf.Public));

            // Act
            var chain = original.Append(appendage);

            // Assert
            chain.Should().NotBeSameAs(original);
            chain.Should().NotBeSameAs(appendage);
            chain.ReflectedType.Should().Be(original.ReflectedType);
            chain.PropertyType.Should().Be(appendage.PropertyType);
            chain.Length.Should().Be(original.Length + appendage.Length);
        }

        [TestMethod] public void AppendChainDerivedFromType() {
            // Scenario: Given a PropertyChain whose current PropertyType is T, append another PropertyChain whose
            //   ReflectedType is derived from T

            // Arrange
            var original = new PropertyChain(typeof(Leaf), nameof(Leaf.IntermediateSelf));
            var appendage = new PropertyChain(typeof(Leaf), nameof(Leaf.Self));

            // Act
            var action = () => original.Append(appendage);

            // Assert
            action.Should().ThrowExactly<ArgumentException>().WithAnyMessage();
        }

        [TestMethod] public void AppendChainBaseOfType() {
            // Scenario: Given a PropertyChain whose current PropertyType is T, append another PropertyChain whose
            //   ReflectedType is a base class of T

            // Arrange
            var original = new PropertyChain(typeof(Leaf), nameof(Leaf.Self));
            var appendage = new PropertyChain(typeof(Base), nameof(Base.Implicit));

            // Act
            var chain = original.Append(appendage);

            // Assert
            chain.Should().NotBeSameAs(original);
            chain.Should().NotBeSameAs(appendage);
            chain.ReflectedType.Should().Be(original.ReflectedType);
            chain.PropertyType.Should().Be(appendage.PropertyType);
            chain.Length.Should().Be(original.Length + appendage.Length);
        }

        [TestMethod] public void AppendChainImplementsType() {
            // Scenario: Given a PropertyChain whose current PropertyType is T, append another PropertyChain whose
            //   ReflectedType implements T

            // Arrange
            var original = new PropertyChain(typeof(Leaf), nameof(Leaf.InterfaceSelf));
            var appendage = new PropertyChain(typeof(Leaf), nameof(Leaf.Self));

            // Act
            var action = () => original.Append(appendage);

            // Assert
            action.Should().ThrowExactly<ArgumentException>().WithAnyMessage();
        }

        [TestMethod] public void AppendChainInterfaceOfType() {
            // Scenario: Given a PropertyChain whose current PropertyType is T, append another PropertyChain whose
            //   ReflectedType is an interface of T

            // Arrange
            var original = new PropertyChain(typeof(Leaf), nameof(Leaf.Self));
            var appendage = new PropertyChain(typeof(IExplicit), nameof(IExplicit.Explicit));

            // Act
            var chain = original.Append(appendage);

            // Assert
            chain.Should().NotBeSameAs(original);
            chain.Should().NotBeSameAs(appendage);
            chain.ReflectedType.Should().Be(original.ReflectedType);
            chain.PropertyType.Should().Be(appendage.PropertyType);
            chain.Length.Should().Be(original.Length + appendage.Length);
        }

        [TestMethod] public void AppendChainUnrelatedToType() {
            // Scenario: Given a PropertyChain whose current PropertyType is T, append another PropertyChain whose
            //   ReflectedType is completely unrelated to T

            // Arrange
            var original = new PropertyChain(typeof(Leaf), nameof(Leaf.Self));
            var appendage = new PropertyChain(typeof(string), nameof(string.Length));

            // Act
            var action = () => original.Append(appendage);

            // Assert
            action.Should().ThrowExactly<ArgumentException>().WithAnyMessage();
        }

        [TestMethod] public void PrependInfoMatchingType() {
            // Scenario: Given a PropertyChain whose current ReflectedType is T, prepend a PropertyInfo that represents
            //   a readable property whose type is exactly T

            // Arrange
            var original = new PropertyChain(typeof(Leaf), nameof(Leaf.Self));
            var property = typeof(Leaf).GetProperty(nameof(Leaf.Self), ANY_PROPERTY)!;

            // Act
            var chain = original.Prepend(property);

            // Assert
            chain.Should().NotBeSameAs(original);
            chain.ReflectedType.Should().Be(property.ReflectedType);
            chain.PropertyType.Should().Be(original.PropertyType);
            chain.Length.Should().Be(original.Length + 1);
        }

        [TestMethod] public void PrependInfoDerivedFromType() {
            // Scenario: Given a PropertyChain whose current ReflectedType is T, prepend a PropertyInfo that represents
            //   a readable property whose type is derived from T

            // Arrange
            var original = new PropertyChain(typeof(Intermediate), nameof(Intermediate.IntermediateChar));
            var property = typeof(Leaf).GetProperty(nameof(Leaf.Self), ANY_PROPERTY)!;

            // Act
            var chain = original.Prepend(property);

            // Assert
            chain.Should().NotBeSameAs(original);
            chain.ReflectedType.Should().Be(property.ReflectedType);
            chain.PropertyType.Should().Be(original.PropertyType);
            chain.Length.Should().Be(original.Length + 1);
        }

        [TestMethod] public void PrependInfoBaseOfFromType() {
            // Scenario: Given a PropertyChain whose current ReflectedType is T, prepend a PropertyInfo that represents
            //   a readable property whose type is a base class of T

            var original = new PropertyChain(typeof(Leaf), nameof(Leaf.Self));
            var property = typeof(Leaf).GetProperty(nameof(Leaf.IntermediateSelf), ANY_PROPERTY)!;

            // Act
            var action = () => original.Prepend(property);

            // Assert
            action.Should().ThrowExactly<ArgumentException>().WithAnyMessage();
        }

        [TestMethod] public void PrependInfoImplementsOfType() {
            // Scenario: Given a PropertyChain whose current ReflectedType is T, prepend a PropertyInfo that represents
            //   a readable property whose type implements

            // Arrange
            var original = new PropertyChain(typeof(IImplicit), nameof(IImplicit.Implicit));
            var property = typeof(Leaf).GetProperty(nameof(Leaf.Self), ANY_PROPERTY)!;

            // Act
            var chain = original.Prepend(property);

            // Assert
            chain.Should().NotBeSameAs(original);
            chain.ReflectedType.Should().Be(property.ReflectedType);
            chain.PropertyType.Should().Be(original.PropertyType);
            chain.Length.Should().Be(original.Length + 1);
        }

        [TestMethod] public void PrependInfoInterfaceOfType() {
            // Scenario: Given a PropertyChain whose current ReflectedType is T, prepend a PropertyInfo that represents
            //   a readable property whose type is an interface of T

            var original = new PropertyChain(typeof(Leaf), nameof(Leaf.Self));
            var property = typeof(IExplicit).GetProperty(nameof(IExplicit.Explicit), ANY_PROPERTY)!;

            // Act
            var action = () => original.Prepend(property);

            // Assert
            action.Should().ThrowExactly<ArgumentException>().WithAnyMessage();
        }

        [TestMethod] public void PrependInfoUnrelatedToType() {
            // Scenario: Given a PropertyChain whose current ReflectedType is T, prepend a PropertyInfo that represents
            //   a readable property whose type is completed unrelated to T

            var original = new PropertyChain(typeof(Leaf), nameof(Leaf.Self));
            var property = typeof(string).GetProperty(nameof(string.Length), ANY_PROPERTY)!;

            // Act
            var action = () => original.Prepend(property);

            // Assert
            action.Should().ThrowExactly<ArgumentException>().WithAnyMessage();
        }

        [TestMethod] public void PrependInfoNonReadableProperty() {
            // Arrange
            var original = new PropertyChain(typeof(Leaf), nameof(Leaf.Self));
            var writeOnlyProperty = typeof(Leaf).GetProperty("WriteOnly", ANY_PROPERTY)!;
            var initOnlyProperty = typeof(Leaf).GetProperty("InitOnly", ANY_PROPERTY)!;

            // Act
            var writeOnlyAction = () => original.Prepend(writeOnlyProperty);
            var initOnlyAction = () => original.Prepend(initOnlyProperty);

            // Assert
            writeOnlyAction.Should().ThrowExactly<ArgumentException>().WithAnyMessage();
            initOnlyAction.Should().ThrowExactly<ArgumentException>().WithAnyMessage();
        }

        [TestMethod] public void PrependChainMatchingType() {
            // Scenario: Given a PropertyChain whose current ReflectedType is T, prepend another PropertyChain whose
            //   PropertyType is exactly T

            // Arrange
            var original = new PropertyChain(typeof(Leaf), nameof(Leaf.Self));
            var prependage = new PropertyChain(typeof(Leaf), nameof(Leaf.Self));

            // Act
            var chain = original.Prepend(prependage);

            // Assert
            chain.Should().NotBeSameAs(original);
            chain.Should().NotBeSameAs(prependage);
            chain.ReflectedType.Should().Be(prependage.ReflectedType);
            chain.PropertyType.Should().Be(original.PropertyType);
            chain.Length.Should().Be(original.Length + prependage.Length);
        }

        [TestMethod] public void PrependChainDerivedFromType() {
            // Scenario: Given a PropertyChain whose current ReflectedType is T, prepend another PropertyChain whose
            //   PropertyType is derived from T

            // Arrange
            var original = new PropertyChain(typeof(Intermediate), nameof(Intermediate.IntermediateChar));
            var prependage = new PropertyChain(typeof(Leaf), nameof(Leaf.Self));

            // Act
            var chain = original.Prepend(prependage);

            // Assert
            chain.Should().NotBeSameAs(original);
            chain.Should().NotBeSameAs(prependage);
            chain.ReflectedType.Should().Be(prependage.ReflectedType);
            chain.PropertyType.Should().Be(original.PropertyType);
            chain.Length.Should().Be(original.Length + prependage.Length);
        }

        [TestMethod] public void PrependChainBaseOfType() {
            // Scenario: Given a PropertyChain whose current ReflectedType is T, prepend another PropertyChain whose
            //   PropertyType is a base class of T

            // Arrange
            var original = new PropertyChain(typeof(Leaf), nameof(Leaf.Self));
            var prependage = new PropertyChain(typeof(Leaf), nameof(Leaf.IntermediateSelf));

            // Act
            var action = () => original.Prepend(prependage);

            // Assert
            action.Should().ThrowExactly<ArgumentException>().WithAnyMessage();
        }

        [TestMethod] public void PrependChainImplementationOfType() {
            // Scenario: Given a PropertyChain whose current ReflectedType is T, prepend another PropertyChain whose
            //   PropertyType implements T

            // Arrange
            var original = new PropertyChain(typeof(IImplicit), nameof(IImplicit.Implicit));
            var prependage = new PropertyChain(typeof(Leaf), nameof(Leaf.Self));

            // Act
            var chain = original.Prepend(prependage);

            // Assert
            chain.Should().NotBeSameAs(original);
            chain.Should().NotBeSameAs(prependage);
            chain.ReflectedType.Should().Be(prependage.ReflectedType);
            chain.PropertyType.Should().Be(original.PropertyType);
            chain.Length.Should().Be(original.Length + prependage.Length);
        }

        [TestMethod] public void PrependChainInterfaceOfType() {
            // Scenario: Given a PropertyChain whose current ReflectedType is T, prepend another PropertyChain whose
            //   PropertyType is an interface of T

            // Arrange
            var original = new PropertyChain(typeof(Leaf), nameof(Leaf.Self));
            var prependage = new PropertyChain(typeof(IExplicit), nameof(IExplicit.Explicit));

            // Act
            var action = () => original.Prepend(prependage);

            // Assert
            action.Should().ThrowExactly<ArgumentException>().WithAnyMessage();
        }

        [TestMethod] public void PrependChainUnrelatedToType() {
            // Scenario: Given a PropertyChain whose current ReflectedType is T, prepend another PropertyChain whose
            //   PropertyType is completely unrelated to T

            // Arrange
            var original = new PropertyChain(typeof(Leaf), nameof(Leaf.Self));
            var prependage = new PropertyChain(typeof(string), nameof(string.Length));

            // Act
            var action = () => original.Prepend(prependage);

            // Assert
            action.Should().ThrowExactly<ArgumentException>().WithAnyMessage();
        }

        [TestMethod] public void GetValueDeclaredNonVirtualPropertyFromSelf() {
            // Source: [T], a class
            // Property: [P], declared by [T]
            // Input: [T], which defines [P] for the first time

            // Arrange
            var chain = new PropertyChain(typeof(Leaf), nameof(Leaf.Public));
            var input = new Leaf();

            // Act
            var value = chain.GetValue(input);

            // Assert
            value.Should().Be(input.Public);
        }

        [TestMethod] public void GetValueInheritedNonVirtualPropertyFromSelf() {
            // Source: [T], a class
            // Property: [P], declared by a base class [B] of [T] and non-virtual
            // Input: [T], which inherits a definition of [P] that it does not hide

            // Arrange
            var chain = new PropertyChain(typeof(Leaf), nameof(Leaf.IntermediateChar));
            var input = new Leaf();

            // Act
            var value = chain.GetValue(input);

            // Assert
            value.Should().Be(input.IntermediateChar);
        }

        [TestMethod] public void GetValueInheritedNonVirtualPropertyFromDerived() {
            // Source: [B], a class
            // Property: [P], declared by [B] and non-virtual
            // Input: [D], which derives from [B] and inherits a definition of [P] that it does not hide

            // Arrange
            var chain = new PropertyChain(typeof(Intermediate), nameof(Intermediate.IntermediateChar));
            var input = new Leaf();

            // Act
            var value = chain.GetValue(input);

            // Assert
            value.Should().Be(input.IntermediateChar);
        }

        [TestMethod] public void GetValueDeclaredVirtualPropertyFromSelf() {
            // Source: [T], a class
            // Property: [P], declared by [T] and virtual
            // Input: [T], which defines [P] for the first time

            // Arrange
            var chain = new PropertyChain(typeof(Leaf), nameof(Leaf.Own));
            var input = new Leaf();

            // Act
            var value = chain.GetValue(input);

            // Assert
            value.Should().Be(input.Own);
        }
        
        [TestMethod] public void GetValueInheritedVirtualPropertyFromSelf() {
            // Source: [T], a class
            // Property [P], declared by a base class [B] of [T] and virtual
            // Input: [T], which inherits a definition of [P] and does not override or hide it

            // Arrange
            var chain = new PropertyChain(typeof(Leaf), nameof(Leaf.VirtualOnly));
            var input = new Leaf();

            // Act
            var value = chain.GetValue(input);

            // Assert
            value.Should().Be(input.VirtualOnly);
        }

        [TestMethod] public void GetValueInheritedVirtualPropertyFromDerived() {
            // Source: [B], a class
            // Property: [P], declared by [B] and virtual
            // Input: [D], which derives from [B] and inherits a definition of [P] that it does not override or hide

            // Arrange
            var chain = new PropertyChain(typeof(Base), nameof(Base.VirtualOnly));
            var input = new Leaf();

            // Act
            var value = chain.GetValue(input);

            // Assert
            value.Should().Be(input.VirtualOnly);
        }

        [TestMethod] public void GetValueDefinedAbstractPropertyFromSelf() {
            // Source: [T], a class
            // Property: [P], declared by a base class [B] of [T] and abstract
            // Input: [D], which derives from [B] and defines [P] for the first time

            // Arrange
            var chain = new PropertyChain(typeof(Leaf), nameof(Leaf.Abstract));
            var input = new Leaf();

            // Act
            var value = chain.GetValue(input);

            // Assert
            value.Should().Be(input.Abstract);
        }

        [TestMethod] public void GetValueDefinedAbstractPropertyFromDerived() {
            // Source: [B], a class
            // Property: [P], declared by [B] and abstract
            // Input: [D], which derives from [B] and defines [P] for the first time

            // Arrange
            var chain = new PropertyChain(typeof(Intermediate), nameof(Intermediate.Abstract));
            var input = new Leaf();

            // Act
            var value = chain.GetValue(input);

            // Assert
            value.Should().Be(input.Abstract);
        }

        [TestMethod] public void GetValueOverriddenPropertyFromSelf() {
            // Source: [T], a class
            // Property [P], declared by a base class [B] of [T] and virtual
            // Input: [T], which inherits a definition of [P] and overrides it

            // Arrange
            var chain = new PropertyChain(typeof(Leaf), nameof(Leaf.Virtual));
            var input = new Leaf();

            // Act
            var value = chain.GetValue(input);

            // Assert
            value.Should().Be(input.Virtual);
        }

        [TestMethod] public void GetValueOverriddenPropertyFromDerived() {
            // Source: [B], a class
            // Property: [P], declared by [B] and virtual
            // Input: [D], which derives from [B] and inherits a definition of [P] that it overrides

            // Arrange
            var chain = new PropertyChain(typeof(Base), nameof(Base.Virtual));
            var input = new Leaf();

            // Act
            var value = chain.GetValue(input);

            // Assert
            value.Should().Be(input.Virtual);
        }

        [TestMethod] public void GetValueImplementedPropertyFromSelf() {
            // Source: [T], a class
            // Property: [P], declared by an interface [I] of [T]
            // Input: [T], which defines [P] for the first time

            // Arrange
            var chain = new PropertyChain(typeof(Base), nameof(Base.Implicit));
            var input = new Base();

            // Act
            var value = chain.GetValue(input);

            // Assert
            value.Should().Be(input.Implicit);
        }

        [TestMethod] public void GetValueImplementedPropertyFromDerived() {
            // Source: [I], an interface
            // Property: [P], declared by [I]
            // Input: [T], which implements [I] and defines [P] for the first time

            // Arrange
            var chain = new PropertyChain(typeof(IImplicit), nameof(IImplicit.Implicit));
            var input = new Base();

            // Act
            var value = chain.GetValue(input);

            // Assert
            value.Should().Be(input.Implicit);
        }

        [TestMethod] public void GetValueInheritedInterfacePropertyFromSelf() {
            // Source: [T], a class
            // Property: [P], declared by an interface [I] of [T]
            // Input: [T], which inherits a definition of [P] and does not hide it

            // Arrange
            var chain = new PropertyChain(typeof(Leaf), nameof(Leaf.Implicit));
            var input = new Leaf();

            // Act
            var value = chain.GetValue(input);

            // Assert
            value.Should().Be(input.Implicit);
        }

        [TestMethod] public void GetValueInheritedInterfacePropertyFromDerived() {
            // Source: [I], an interface
            // Property: [P], declared by [I]
            // Input: [T], which implements [I] and inherits a definition of [P] and does not hide it

            // Arrange
            var chain = new PropertyChain(typeof(Base), nameof(Base.Implicit));
            var input = new Leaf();

            // Act
            var value = chain.GetValue(input);

            // Assert
            value.Should().Be(input.Implicit);
        }

        [TestMethod] public void GetValueExplicitImplementedProperty() {
            // Source: [I], an interface
            // Property: [P], declared by [I]
            // Input: [T], which implements [I] and defines [P] as an explicit implementation

            // Arrange
            var chain = new PropertyChain(typeof(IExplicit), nameof(IExplicit.Explicit));
            var input = new Base();

            // Act
            var value = chain.GetValue(input);

            // Assert
            value.Should().Be((input as IExplicit).Explicit);
        }

        [TestMethod] public void GetValueHidingProperty() {
            // Source: [T], a class
            // Property: [P], which hides a same-name property declared by a base [B] of [T]
            // Input: [T]

            // Arrange
            var chain = new PropertyChain(typeof(Leaf), nameof(Leaf.Hide));
            var input = new Leaf();

            // Act
            var value = chain.GetValue(input);

            // Assert
            value.Should().Be(input.Hide);
        }

        [TestMethod] public void GetValueHiddenProperty() {
            // Source: [T], a class
            // Property: [P], declared on a base class [B] of [T]
            // Input: [T], which hides [P] with a same-name property of its own

            // Arrange
            var chain = new PropertyChain(typeof(Base), nameof(Base.Hide));
            var input = new Leaf();

            // Act
            var value = chain.GetValue(input);

            // Assert
            value.Should().Be((input as Base).Hide);
        }

        [TestMethod] public void GetValueFromBaseType() {
            // Arrange
            var chain = new PropertyChain(typeof(Leaf), nameof(Leaf.VirtualOnly));
            var input = new Base();

            // Act
            var action = () => chain.GetValue(input);

            // Assert
            action.Should().ThrowExactly<ArgumentException>().WithAnyMessage();
        }

        [TestMethod] public void GetValueFromUnrelatedType() {
            // Arrange
            var chain = new PropertyChain(typeof(Leaf), nameof(Leaf.VirtualOnly));
            var input = 491;

            // Act
            var action = () => chain.GetValue(input);

            // Assert
            action.Should().ThrowExactly<ArgumentException>().WithAnyMessage();
        }

        [TestMethod] public void GetValueFromNull() {
            // Arrange
            var chain = new PropertyChain(typeof(Leaf), nameof(Leaf.VirtualOnly));
            object? input = null;

            // Act
            var action = () => chain.GetValue(input);

            // Assert
            action.Should().ThrowExactly<ArgumentNullException>().WithAnyMessage();
        }

        [TestMethod] public void GetValueReturnsNullImmediately() {
            // Arrange
            var chain = new PropertyChain(typeof(ApplicationException), nameof(ApplicationException.InnerException));
            var input = new ApplicationException();

            // Act
            var value = chain.GetValue(input);

            // Assert
            value.Should().BeNull();
        }

        [TestMethod] public void GetValueReturnsNullTerminally() {
            // Arrange
            var chain = new PropertyChain(typeof(ApplicationException), nameof(ApplicationException.InnerException));
            chain = chain.Append(nameof(Exception.InnerException));
            var input = new ApplicationException("!!!", new ArgumentNullException());

            // Act
            var value = chain.GetValue(input);

            // Assert
            value.Should().BeNull();
        }

        [TestMethod] public void GetValueReturnsNullIntermediately() {
            // Arrange
            var chain = new PropertyChain(typeof(ApplicationException), nameof(ApplicationException.InnerException));
            chain = chain.Append(nameof(Exception.InnerException));
            chain = chain.Append(nameof(Exception.InnerException));
            var input = new ApplicationException("!!!", new ArgumentNullException());

            // Act
            var value = chain.GetValue(input);

            // Assert
            value.Should().BeNull();
        }

        [TestMethod] public void GetValueFromNonPublicProperty() {
            void evaluate(string propertyName, string expectedValue) {
                // Arrange
                var chain = new PropertyChain(typeof(Leaf), propertyName);
                var input = new Leaf();

                // Act
                var value = chain.GetValue(input);

                // Assert
                value.Should().Be(expectedValue);
            }

            evaluate("Private", "Dili");
            evaluate("Protected", "Tashkent");
            evaluate("Internal", "Cairns");
            evaluate("InternalProtected", "Bologna");
        }

        [TestMethod] public void GetValueFromStaticProperty() {
            void evaluate(string propertyName, int expectedValue) {
                // Arrange
                var chain = new PropertyChain(typeof(Leaf), propertyName);
                var input = new Leaf();

                // Act
                var value = chain.GetValue(input);

                // Assert
                value.Should().Be(expectedValue);
            }

            evaluate("StaticPublic", 4000);
            evaluate("StaticPrivate", 5000);
            evaluate("StaticProtected", 6000);
            evaluate("StaticInternal", 7000);
            evaluate("StaticInternalProtected", 8000);
        }

        [TestMethod] public void GetValueFromHeavilyNestedChain() {
            var chain = new PropertyChain(typeof(PropertyInfo), nameof(PropertyInfo.PropertyType));
            chain = chain.Append(nameof(Type.Assembly));
            chain = chain.Append(nameof(Assembly.ManifestModule));
            chain = chain.Append(nameof(Module.Name));
            chain = chain.Append(nameof(string.Length));
            var input = typeof(PropertyChain).GetProperty(nameof(PropertyChain.ReflectedType))!;

            // Act
            var value = chain.GetValue(input);

            // Assert
            value.Should().NotBeNull();
            value.Should().Be(input.PropertyType.Assembly.ManifestModule.Name.Length);
        }


        private interface IImplicit { int Implicit { get; } }
        private interface IExplicit { int Explicit { get; } }
        private class Base : IImplicit, IExplicit {
            public int Implicit { get; } = 1000;
            int IExplicit.Explicit { get; } = 2000;
            public DateTime Hide { get; } = new DateTime(1996, 2, 29);
            public virtual string Virtual { get; } = "Ngerulmud";
            public virtual string VirtualOnly { get; } = "Aswan";
        }
        private abstract class Intermediate : Base {
            public char IntermediateChar { get; } = '%';
            public abstract string Abstract { get; }
        }
        private class Leaf : Intermediate {
            // Properties to allow chains that re-expose access to all the other properties
            public Leaf Self => this;
            public Intermediate IntermediateSelf => this;
            public IImplicit InterfaceSelf => this;

            // Brand New Properties
            public virtual bool Own { get; } = false;

            // Properties that override or hide inherited properties
            public new DateTimeOffset Hide { get; } = new DateTime(1997, 12, 26);
            public override string Virtual { get; } = "Colombo";
            public override string Abstract { get; } = "Phuket";

            // Non-readable properties
            public string WriteOnly { set {} }
            public string InitOnly { init {} }

            // Static properties
            public static int StaticPublic { get; } = 4000;
            private static int StaticPrivate { get; } = 5000;
            protected static int StaticProtected { get; } = 6000;
            internal static int StaticInternal { get; } = 7000;
            protected static int StaticInternalProtected { get; } = 8000;

            // Instance properties
            public string Public { get; } = "Limoges";
            private string Private { get; } = "Dili";
            protected string Protected { get; } = "Tashkent";
            internal string Internal { get; } = "Cairns";
            protected internal string InternalProtected { get; } = "Bologna";
        }

        private const BindingFlags ANY_PROPERTY =
            BindingFlags.Public     |
            BindingFlags.NonPublic  |
            BindingFlags.Static     |
            BindingFlags.Instance   |
            BindingFlags.FlattenHierarchy;
    }
}
