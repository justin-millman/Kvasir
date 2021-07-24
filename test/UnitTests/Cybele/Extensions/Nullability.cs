using Cybele.Extensions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UT.Cybele.Extensions {
    [TestClass, TestCategory("Nullability: Enabled Context")]
    public sealed class Nullability_EnabledContext {
        [TestMethod] public void NonNullableReferenceProperty() {
            // Arrange
            var prop = typeof(NullableEnabled).GetProperty(nameof(NullableEnabled.NonNullableRefProp))!;

            // Act
            var nullability = prop. GetNullability();

            // Assert
            nullability.Should().Be(Nullability.NonNullable);
        }

        [TestMethod] public void NullableReferenceProperty() {
            // Arrange
            var prop = typeof(NullableEnabled).GetProperty(nameof(NullableEnabled.NullableRefProp))!;

            // Act
            var nullability = prop. GetNullability();

            // Assert
            nullability.Should().Be(Nullability.Nullable);
        }

        [TestMethod] public void NonNullablePrimitiveProperty() {
            // Arrange
            var prop = typeof(NullableEnabled).GetProperty(nameof(NullableEnabled.NonNullablePrimitiveProp))!;

            // Act
            var nullability = prop. GetNullability();

            // Assert
            nullability.Should().Be(Nullability.NonNullable);
        }

        [TestMethod] public void NullablePrimitiveProperty() {
            // Arrange
            var prop = typeof(NullableEnabled).GetProperty(nameof(NullableEnabled.NullablePrimitiveProp))!;

            // Act
            var nullability = prop. GetNullability();

            // Assert
            nullability.Should().Be(Nullability.Nullable);
        }

        [TestMethod] public void NonNullableEnumProperty() {
            // Arrange
            var prop = typeof(NullableEnabled).GetProperty(nameof(NullableEnabled.NonNullableEnumProp))!;

            // Act
            var nullability = prop. GetNullability();

            // Assert
            nullability.Should().Be(Nullability.NonNullable);
        }

        [TestMethod] public void NullableEnumProperty() {
            // Arrange
            var prop = typeof(NullableEnabled).GetProperty(nameof(NullableEnabled.NullableEnumProp))!;

            // Act
            var nullability = prop. GetNullability();

            // Assert
            nullability.Should().Be(Nullability.Nullable);
        }

        [TestMethod] public void NonNullableDelegateProperty() {
            // Arrange
            var prop = typeof(NullableEnabled).GetProperty(nameof(NullableEnabled.NonNullableDelegateProp))!;

            // Act
            var nullability = prop. GetNullability();

            // Assert
            nullability.Should().Be(Nullability.NonNullable);
        }

        [TestMethod] public void NullableDelegateProperty() {
            // Arrange
            var prop = typeof(NullableEnabled).GetProperty(nameof(NullableEnabled.NullableDeleateProp))!;

            // Act
            var nullability = prop. GetNullability();

            // Assert
            nullability.Should().Be(Nullability.Nullable);
        }

        [TestMethod] public void NonNullableDynamicProperty() {
            // Arrange
            var prop = typeof(NullableEnabled).GetProperty(nameof(NullableEnabled.NonNullableDynamicProp))!;

            // Act
            var nullability = prop.GetNullability();

            // Assert
            nullability.Should().Be(Nullability.NonNullable);
        }

        [TestMethod] public void NullableDynamicProperty() {
            // Arrange
            var prop = typeof(NullableEnabled).GetProperty(nameof(NullableEnabled.NullableDynamicProp))!;

            // Act
            var nullability = prop.GetNullability();

            // Assert
            nullability.Should().Be(Nullability.Nullable);
        }

        [TestMethod] public void NonNullableReferenceField() {
            // Arrange
            var field = typeof(NullableEnabled).GetField(nameof(NullableEnabled.NonNullableRefVar))!;

            // Act
            var nullability = field.GetNullability();

            // Assert
            nullability.Should().Be(Nullability.NonNullable);
        }

        [TestMethod] public void NullableReferenceField() {
            // Arrange
            var field = typeof(NullableEnabled).GetField(nameof(NullableEnabled.NullableRefVar))!;

            // Act
            var nullability = field.GetNullability();

            // Assert
            nullability.Should().Be(Nullability.Nullable);
        }

        [TestMethod] public void NonNullablePrimitiveField() {
            // Arrange
            var field = typeof(NullableEnabled).GetField(nameof(NullableEnabled.NonNullablePrimitiveVar))!;

            // Act
            var nullability = field.GetNullability();

            // Assert
            nullability.Should().Be(Nullability.NonNullable);
        }

        [TestMethod] public void NullablePrimitiveField() {
            // Arrange
            var field = typeof(NullableEnabled).GetField(nameof(NullableEnabled.NullablePrimitiveVar))!;

            // Act
            var nullability = field.GetNullability();

            // Assert
            nullability.Should().Be(Nullability.Nullable);
        }

        [TestMethod] public void NonNullableEnumField() {
            // Arrange
            var field = typeof(NullableEnabled).GetField(nameof(NullableEnabled.NonNullableEnumVar))!;

            // Act
            var nullability = field.GetNullability();

            // Assert
            nullability.Should().Be(Nullability.NonNullable);
        }

        [TestMethod] public void NullableEnumField() {
            // Arrange
            var field = typeof(NullableEnabled).GetField(nameof(NullableEnabled.NullableEnumVar))!;

            // Act
            var nullability = field.GetNullability();

            // Assert
            nullability.Should().Be(Nullability.Nullable);
        }

        [TestMethod] public void NonNullableDelegateField() {
            // Arrange
            var field = typeof(NullableEnabled).GetField(nameof(NullableEnabled.NonNullableDelegateVar))!;

            // Act
            var nullability = field.GetNullability();

            // Assert
            nullability.Should().Be(Nullability.NonNullable);
        }

        [TestMethod] public void NullableDelegateField() {
            // Arrange
            var field = typeof(NullableEnabled).GetField(nameof(NullableEnabled.NullableDelegateVar))!;

            // Act
            var nullability = field.GetNullability();

            // Assert
            nullability.Should().Be(Nullability.Nullable);
        }

        [TestMethod] public void NonNullableDynamicField() {
            // Arrange
            var field = typeof(NullableEnabled).GetField(nameof(NullableEnabled.NonNullableDynamicVar))!;

            // Act
            var nullability = field.GetNullability();

            // Assert
            nullability.Should().Be(Nullability.NonNullable);
        }

        [TestMethod] public void NullableDynamicField() {
            // Arrange
            var field = typeof(NullableEnabled).GetField(nameof(NullableEnabled.NullableDynamicVar))!;

            // Act
            var nullability = field.GetNullability();

            // Assert
            nullability.Should().Be(Nullability.Nullable);
        }

        [TestMethod] public void NonNullableEvent() {
            // Arrange
            var evnt = typeof(NullableEnabled).GetEvent(nameof(NullableEnabled.NonNullableEvent))!;

            // Act
            var nullability = evnt.GetNullability();

            // Assert
            nullability.Should().Be(Nullability.NonNullable);
        }

        [TestMethod] public void NullableEvent() {
            // Arrange
            var evnt = typeof(NullableEnabled).GetEvent(nameof(NullableEnabled.NullableEvent))!;

            // Act
            var nullability = evnt.GetNullability();

            // Assert
            nullability.Should().Be(Nullability.Nullable);
        }

        [TestMethod] public void NonNullableReferenceParameter() {
            // Arrange
            var param = typeof(NullableEnabled).GetConstructors()[0].GetParameters()[0]!;
            param.Name.Should().Be("NonNullableRefParam");

            // Act
            var nullability = param.GetNullability();

            // Assert
            nullability.Should().Be(Nullability.NonNullable);
        }

        [TestMethod] public void NullableReferenceParameter() {
            // Arrange
            var param = typeof(NullableEnabled).GetConstructors()[0].GetParameters()[1]!;
            param.Name.Should().Be("NullableRefParam");

            // Act
            var nullability = param.GetNullability();

            // Assert
            nullability.Should().Be(Nullability.Nullable);
        }

        [TestMethod] public void NonNullablePrimitiveParameter() {
            // Arrange
            var param = typeof(NullableEnabled).GetConstructors()[0].GetParameters()[2]!;
            param.Name.Should().Be("NonNullablePrimitiveParam");

            // Act
            var nullability = param.GetNullability();

            // Assert
            nullability.Should().Be(Nullability.NonNullable);
        }

        [TestMethod] public void NullablePrimitiveParameter() {
            // Arrange
            var param = typeof(NullableEnabled).GetConstructors()[0].GetParameters()[3]!;
            param.Name.Should().Be("NullablePrimitiveParam");

            // Act
            var nullability = param.GetNullability();

            // Assert
            nullability.Should().Be(Nullability.Nullable);
        }

        [TestMethod] public void NonNullableEnumParameter() {
            // Arrange
            var param = typeof(NullableEnabled).GetConstructors()[0].GetParameters()[4]!;
            param.Name.Should().Be("NonNullableEnumParam");

            // Act
            var nullability = param.GetNullability();

            // Assert
            nullability.Should().Be(Nullability.NonNullable);
        }

        [TestMethod] public void NullableEnumParameter() {
            // Arrange
            var param = typeof(NullableEnabled).GetConstructors()[0].GetParameters()[5]!;
            param.Name.Should().Be("NullableEnumParam");

            // Act
            var nullability = param.GetNullability();

            // Assert
            nullability.Should().Be(Nullability.Nullable);
        }

        [TestMethod] public void NonNullableDelegateParameter() {
            // Arrange
            var param = typeof(NullableEnabled).GetConstructors()[0].GetParameters()[6]!;
            param.Name.Should().Be("NonNullableDelegateParam");

            // Act
            var nullability = param.GetNullability();

            // Assert
            nullability.Should().Be(Nullability.NonNullable);
        }

        [TestMethod] public void NullableDelegateParameter() {
            // Arrange
            var param = typeof(NullableEnabled).GetConstructors()[0].GetParameters()[7]!;
            param.Name.Should().Be("NullableDelegateParam");

            // Act
            var nullability = param.GetNullability();

            // Assert
            nullability.Should().Be(Nullability.Nullable);
        }

        [TestMethod] public void NonNullableDynamicParameter() {
            // Arrange
            var param = typeof(NullableEnabled).GetConstructors()[0].GetParameters()[8]!;
            param.Name.Should().Be("NonNullableDynamicParam");

            // Act
            var nullability = param.GetNullability();

            // Assert
            nullability.Should().Be(Nullability.NonNullable);
        }

        [TestMethod] public void NullableDynamicParameter() {
            // Arrange
            var param = typeof(NullableEnabled).GetConstructors()[0].GetParameters()[9]!;
            param.Name.Should().Be("NullableDynamicParam");

            // Act
            var nullability = param.GetNullability();

            // Assert
            nullability.Should().Be(Nullability.Nullable);
        }

        [TestMethod] public void NonNullableReferenceReturn() {
            // Arrange
            var retval = typeof(NullableEnabled).GetMethod(nameof(NullableEnabled.NonNullableRefReturn))!
                .ReturnParameter;

            // Act
            var nullability = retval.GetNullability();

            // Assert
            nullability.Should().Be(Nullability.NonNullable);
        }

        [TestMethod] public void NullableReferenceReturn() {
            // Arrange
            var retval = typeof(NullableEnabled).GetMethod(nameof(NullableEnabled.NullableRefReturn))!
                .ReturnParameter;

            // Act
            var nullability = retval.GetNullability();

            // Assert
            nullability.Should().Be(Nullability.Nullable);
        }

        [TestMethod] public void NonNullablePrimitiveReturn() {
            // Arrange
            var retval = typeof(NullableEnabled).GetMethod(nameof(NullableEnabled.NonNullablePrimitiveReturn))!
                .ReturnParameter;

            // Act
            var nullability = retval.GetNullability();

            // Assert
            nullability.Should().Be(Nullability.NonNullable);
        }

        [TestMethod] public void NullablePrimitiveReturn() {
            // Arrange
            var retval = typeof(NullableEnabled).GetMethod(nameof(NullableEnabled.NullablePrimitiveReturn))!
                .ReturnParameter;

            // Act
            var nullability = retval.GetNullability();

            // Assert
            nullability.Should().Be(Nullability.Nullable);
        }

        [TestMethod] public void NonNullableEnumReturn() {
            // Arrange
            var retval = typeof(NullableEnabled).GetMethod(nameof(NullableEnabled.NonNullableEnumReturn))!
                .ReturnParameter;

            // Act
            var nullability = retval.GetNullability();

            // Assert
            nullability.Should().Be(Nullability.NonNullable);
        }

        [TestMethod] public void NullableEnumReturn() {
            // Arrange
            var retval = typeof(NullableEnabled).GetMethod(nameof(NullableEnabled.NullableEnumReturn))!
                .ReturnParameter;

            // Act
            var nullability = retval.GetNullability();

            // Assert
            nullability.Should().Be(Nullability.Nullable);
        }

        [TestMethod] public void NonNullableDelegateReturn() {
            // Arrange
            var retval = typeof(NullableEnabled).GetMethod(nameof(NullableEnabled.NonNullableDelegateReturn))!
                .ReturnParameter;

            // Act
            var nullability = retval.GetNullability();

            // Assert
            nullability.Should().Be(Nullability.NonNullable);
        }

        [TestMethod] public void NullableDelegateReturn() {
            // Arrange
            var retval = typeof(NullableEnabled).GetMethod(nameof(NullableEnabled.NullableDelegateReturn))!
                .ReturnParameter;

            // Act
            var nullability = retval.GetNullability();

            // Assert
            nullability.Should().Be(Nullability.Nullable);
        }

        [TestMethod] public void NonNullableDynamicReturn() {
            // Arrange
            var retval = typeof(NullableEnabled).GetMethod(nameof(NullableEnabled.NonNullableDynamicReturn))!
                .ReturnParameter;

            // Act
            var nullability = retval.GetNullability();

            // Assert
            nullability.Should().Be(Nullability.NonNullable);
        }

        [TestMethod] public void NullableDynamicReturn() {
            // Arrange
            var retval = typeof(NullableEnabled).GetMethod(nameof(NullableEnabled.NullableDynamicReturn))!
                .ReturnParameter;

            // Act
            var nullability = retval.GetNullability();

            // Assert
            nullability.Should().Be(Nullability.Nullable);
        }

        [TestMethod] public void UnrestrictedGeneric() {
            // Arrange
            var method = typeof(NullableEnabled).GetMethod(nameof(NullableEnabled.UnrestrictedGeneric))!;
            var ambiguousArg = method.GetParameters()[0]!;
            var ambiguousArg2 = method.GetParameters()[1]!;
            var retval = method.ReturnParameter;

            // Act
            var nullability0 = ambiguousArg.GetNullability();
            var nullability1 = ambiguousArg2.GetNullability();
            var returnNullability = retval.GetNullability();

            // Assert
            nullability0.Should().Be(Nullability.NonNullable);              // limitation in reflection capabilities
            nullability1.Should().Be(Nullability.Nullable);                 // limitation in reflection capabilities
            returnNullability.Should().Be(Nullability.NonNullable);         // limitation in reflection capabilities
        }

        [TestMethod] public void NonNullableClassRestrictedGeneric() {
            // Arrange
            var method = typeof(NullableEnabled).GetMethod(nameof(NullableEnabled.NonNullableClassRestrictedGeneric))!;
            var nonNullableArg = method.GetParameters()[0]!;
            var nullableArg = method.GetParameters()[1]!;
            var retval = method.ReturnParameter;

            var x = ReferenceEquals(nonNullableArg.ParameterType, nullableArg.ParameterType);

            // Act
            var nullability0 = nonNullableArg.GetNullability();
            var nullability1 = nullableArg.GetNullability();
            var returnNullability = retval.GetNullability();

            // Assert
            nullability0.Should().Be(Nullability.NonNullable);
            nullability1.Should().Be(Nullability.Nullable);
            returnNullability.Should().Be(Nullability.NonNullable);
        }

        [TestMethod] public void NullableClassRestrictedGeneric() {
            // Arrange
            var method = typeof(NullableEnabled).GetMethod(nameof(NullableEnabled.NullableClassRestrictedGeneric))!;
            var ambiguousArg = method.GetParameters()[0]!;
            var nullableArg = method.GetParameters()[1]!;
            var retval = method.ReturnParameter;

            // Act
            var nullability0 = ambiguousArg.GetNullability();
            var nullability1 = nullableArg.GetNullability();
            var returnNullability = retval.GetNullability();

            // Assert
            nullability0.Should().Be(Nullability.NonNullable);              // limitation in reflection capabilities
            nullability1.Should().Be(Nullability.Nullable);
            returnNullability.Should().Be(Nullability.NonNullable);         // limitation in reflection capabilities
        }

        [TestMethod] public void StructRestrictedGeneric() {
            // Arrange
            var method = typeof(NullableEnabled).GetMethod(nameof(NullableEnabled.StructRestrictedGeneric))!;
            var nonNullableArg = method.GetParameters()[0]!;
            var nullableArg = method.GetParameters()[1]!;
            var retval = method.ReturnParameter;

            // Act
            var nullability0 = nonNullableArg.GetNullability();
            var nullability1 = nullableArg.GetNullability();
            var returnNullability = retval.GetNullability();

            // Assert
            nullability0.Should().Be(Nullability.NonNullable);
            nullability1.Should().Be(Nullability.Nullable);
            returnNullability.Should().Be(Nullability.NonNullable);
        }

        [TestMethod] public void EnumRestrictedGeneric() {
            // Arrange
            var method = typeof(NullableEnabled).GetMethod(nameof(NullableEnabled.EnumRestrictedGeneric))!;
            var nonNullableArg = method.GetParameters()[0]!;
            var nonNullableArg2 = method.GetParameters()[1]!;
            var retval = method.ReturnParameter;

            // Act
            var nullability0 = nonNullableArg.GetNullability();
            var nullability1 = nonNullableArg2.GetNullability();
            var returnNullability = retval.GetNullability();

            // Assert
            nullability0.Should().Be(Nullability.NonNullable);
            nullability1.Should().Be(Nullability.NonNullable);
            returnNullability.Should().Be(Nullability.NonNullable);
        }

        [TestMethod] public void NotNullRestrictedGeneric() {
            // Arrange
            var method = typeof(NullableEnabled).GetMethod(nameof(NullableEnabled.NotNullRestrictedGeneric))!;
            var nonNullableArg = method.GetParameters()[0]!;
            var ambiguousArg = method.GetParameters()[1]!;
            var retval = method.ReturnParameter;

            // Act
            var nullability0 = nonNullableArg.GetNullability();
            var nullability1 = ambiguousArg.GetNullability();
            var returnNullability = retval.GetNullability();

            // Assert
            nullability0.Should().Be(Nullability.NonNullable);
            nullability1.Should().Be(Nullability.Nullable);                 // limitation in reflection capabilities
            returnNullability.Should().Be(Nullability.NonNullable);
        }
    }

    [TestClass, TestCategory("Nullability: Disabled Context")]
    public sealed class Nullability_DisabledContext {
        [TestMethod] public void ReferenceProperty() {
            // Arrange
            var prop = typeof(NullableDisabled).GetProperty(nameof(NullableDisabled.RefProp))!;

            // Act
            var nullability = prop.GetNullability();

            // Assert
            nullability.Should().Be(Nullability.Nullable);
        }

        [TestMethod] public void NonNullablePrimitiveProperty() {
            // Arrange
            var prop = typeof(NullableDisabled).GetProperty(nameof(NullableDisabled.NonNullablePrimitiveProp))!;

            // Act
            var nullability = prop.GetNullability();

            // Assert
            nullability.Should().Be(Nullability.NonNullable);
        }

        [TestMethod] public void NullablePrimitiveProperty() {
            // Arrange
            var prop = typeof(NullableDisabled).GetProperty(nameof(NullableDisabled.NullablePrimitiveProp))!;

            // Act
            var nullability = prop.GetNullability();

            // Assert
            nullability.Should().Be(Nullability.Nullable);
        }

        [TestMethod] public void NonNullableEnumProperty() {
            // Arrange
            var prop = typeof(NullableDisabled).GetProperty(nameof(NullableDisabled.NonNullableEnumProp))!;

            // Act
            var nullability = prop.GetNullability();

            // Assert
            nullability.Should().Be(Nullability.NonNullable);
        }

        [TestMethod] public void NullableEnumProperty() {
            // Arrange
            var prop = typeof(NullableDisabled).GetProperty(nameof(NullableDisabled.NullableEnumProp))!;

            // Act
            var nullability = prop.GetNullability();

            // Assert
            nullability.Should().Be(Nullability.Nullable);
        }

        [TestMethod] public void DelegateProperty() {
            // Arrange
            var prop = typeof(NullableDisabled).GetProperty(nameof(NullableDisabled.DelegateProp))!;

            // Act
            var nullability = prop.GetNullability();

            // Assert
            nullability.Should().Be(Nullability.Nullable);
        }

        [TestMethod] public void DynamicProperty() {
            // Arrange
            var prop = typeof(NullableDisabled).GetProperty(nameof(NullableDisabled.DynamicProp))!;

            // Act
            var nullability = prop.GetNullability();

            // Assert
            nullability.Should().Be(Nullability.Nullable);
        }

        [TestMethod] public void ReferenceField() {
            // Arrange
            var field = typeof(NullableDisabled).GetField(nameof(NullableDisabled.RefVar))!;

            // Act
            var nullability = field.GetNullability();

            // Assert
            nullability.Should().Be(Nullability.Nullable);
        }

        [TestMethod] public void NonNullablePrimitiveField() {
            // Arrange
            var field = typeof(NullableDisabled).GetField(nameof(NullableDisabled.NonNullablePrimitiveVar))!;

            // Act
            var nullability = field.GetNullability();

            // Assert
            nullability.Should().Be(Nullability.NonNullable);
        }

        [TestMethod] public void NullablePrimitiveField() {
            // Arrange
            var field = typeof(NullableDisabled).GetField(nameof(NullableDisabled.NullablePrimitiveVar))!;

            // Act
            var nullability = field.GetNullability();

            // Assert
            nullability.Should().Be(Nullability.Nullable);
        }

        [TestMethod] public void NonNullableEnumField() {
            // Arrange
            var field = typeof(NullableDisabled).GetField(nameof(NullableDisabled.NonNullableEnumVar))!;

            // Act
            var nullability = field.GetNullability();

            // Assert
            nullability.Should().Be(Nullability.NonNullable);
        }

        [TestMethod] public void NullableEnumField() {
            // Arrange
            var field = typeof(NullableDisabled).GetField(nameof(NullableDisabled.NullableEnumVar))!;

            // Act
            var nullability = field.GetNullability();

            // Assert
            nullability.Should().Be(Nullability.Nullable);
        }

        [TestMethod] public void DelegateField() {
            // Arrange
            var field = typeof(NullableDisabled).GetField(nameof(NullableDisabled.DelegateVar))!;

            // Act
            var nullability = field.GetNullability();

            // Assert
            nullability.Should().Be(Nullability.Nullable);
        }

        [TestMethod] public void DynamicField() {
            // Arrange
            var field = typeof(NullableDisabled).GetField(nameof(NullableDisabled.DynamicVar))!;

            // Act
            var nullability = field.GetNullability();

            // Assert
            nullability.Should().Be(Nullability.Nullable);
        }

        [TestMethod] public void Event() {
            // Arrange
            var evnt = typeof(NullableDisabled).GetEvent(nameof(NullableDisabled.Event))!;

            // Act
            var nullability = evnt.GetNullability();

            // Assert
            nullability.Should().Be(Nullability.Nullable);
        }

        [TestMethod] public void ReferenceParameter() {
            // Arrange
            var param = typeof(NullableDisabled).GetConstructors()[0].GetParameters()[0]!;
            param.Name.Should().Be("RefParam");

            // Act
            var nullability = param.GetNullability();

            // Assert
            nullability.Should().Be(Nullability.Nullable);
        }

        [TestMethod] public void NonNullablePrimitiveParameter() {
            // Arrange
            var param = typeof(NullableDisabled).GetConstructors()[0].GetParameters()[1]!;
            param.Name.Should().Be("NonNullablePrimitiveParam");

            // Act
            var nullability = param.GetNullability();

            // Assert
            nullability.Should().Be(Nullability.NonNullable);
        }

        [TestMethod] public void NullablePrimitiveParameter() {
            // Arrange
            var param = typeof(NullableDisabled).GetConstructors()[0].GetParameters()[2]!;
            param.Name.Should().Be("NullablePrimitiveParam");

            // Act
            var nullability = param.GetNullability();

            // Assert
            nullability.Should().Be(Nullability.Nullable);
        }

        [TestMethod] public void NonNullableEnumParameter() {
            // Arrange
            var param = typeof(NullableDisabled).GetConstructors()[0].GetParameters()[3]!;
            param.Name.Should().Be("NonNullableEnumParam");

            // Act
            var nullability = param.GetNullability();

            // Assert
            nullability.Should().Be(Nullability.NonNullable);
        }

        [TestMethod] public void NullableEnumParameter() {
            // Arrange
            var param = typeof(NullableDisabled).GetConstructors()[0].GetParameters()[4]!;
            param.Name.Should().Be("NullableEnumParam");

            // Act
            var nullability = param.GetNullability();

            // Assert
            nullability.Should().Be(Nullability.Nullable);
        }

        [TestMethod] public void DelegateParameter() {
            // Arrange
            var param = typeof(NullableDisabled).GetConstructors()[0].GetParameters()[5]!;
            param.Name.Should().Be("DelegateParam");

            // Act
            var nullability = param.GetNullability();

            // Assert
            nullability.Should().Be(Nullability.Nullable);
        }

        [TestMethod] public void DynamicParameter() {
            // Arrange
            var param = typeof(NullableDisabled).GetConstructors()[0].GetParameters()[6]!;
            param.Name.Should().Be("DynamicParam");

            // Act
            var nullability = param.GetNullability();

            // Assert
            nullability.Should().Be(Nullability.Nullable);
        }

        [TestMethod] public void ReferenceReturn() {
            // Arrange
            var retval = typeof(NullableDisabled).GetMethod(nameof(NullableDisabled.RefReturn))!.ReturnParameter;

            // Act
            var nullability = retval.GetNullability();

            // Assert
            nullability.Should().Be(Nullability.Nullable);
        }

        [TestMethod] public void NonNullablePrimitiveReturn() {
            // Arrange
            var retval = typeof(NullableDisabled).GetMethod(nameof(NullableDisabled.NonNullablePrimitiveReturn))!
                .ReturnParameter;

            // Act
            var nullability = retval.GetNullability();

            // Assert
            nullability.Should().Be(Nullability.NonNullable);
        }

        [TestMethod] public void NullablePrimitiveReturn() {
            // Arrange
            var retval = typeof(NullableDisabled).GetMethod(nameof(NullableDisabled.NullablePrimitiveReturn))!
                .ReturnParameter;

            // Act
            var nullability = retval.GetNullability();

            // Assert
            nullability.Should().Be(Nullability.Nullable);
        }

        [TestMethod] public void NonNullableEnumReturn() {
            // Arrange
            var retval = typeof(NullableDisabled).GetMethod(nameof(NullableDisabled.NonNullableEnumReturn))!
                .ReturnParameter;

            // Act
            var nullability = retval.GetNullability();

            // Assert
            nullability.Should().Be(Nullability.NonNullable);
        }

        [TestMethod] public void NullableEnumReturn() {
            // Arrange
            var retval = typeof(NullableDisabled).GetMethod(nameof(NullableDisabled.NullableEnumReturn))!
                .ReturnParameter;

            // Act
            var nullability = retval.GetNullability();

            // Assert
            nullability.Should().Be(Nullability.Nullable);
        }

        [TestMethod] public void DelegateReturn() {
            // Arrange
            var retval = typeof(NullableDisabled).GetMethod(nameof(NullableDisabled.DelegateReturn))!.ReturnParameter;

            // Act
            var nullability = retval.GetNullability();

            // Assert
            nullability.Should().Be(Nullability.Nullable);
        }

        [TestMethod] public void DynamicReturn() {
            // Arrange
            var retval = typeof(NullableDisabled).GetMethod(nameof(NullableDisabled.DynamicReturn))!.ReturnParameter;

            // Act
            var nullability = retval.GetNullability();

            // Assert
            nullability.Should().Be(Nullability.Nullable);
        }

        [TestMethod] public void UnrestrictedGeneric() {
            // Arrange
            var method = typeof(NullableDisabled).GetMethod(nameof(NullableDisabled.UnrestrictedGeneric))!;
            var ambiguousArg = method.GetParameters()[0]!;
            var retval = method.ReturnParameter;

            // Act
            var nullability0 = ambiguousArg.GetNullability();
            var returnNullability = retval.GetNullability();

            // Assert
            nullability0.Should().Be(Nullability.Nullable);                 // limitation in reflection capabilities
            returnNullability.Should().Be(Nullability.Nullable);            // limitation in reflection capabilities
        }

        [TestMethod] public void ClassRestrictedGeneric() {
            // Arrange
            var method = typeof(NullableDisabled).GetMethod(nameof(NullableDisabled.ClassRestrictedGeneric))!;
            var nullableArg = method.GetParameters()[0]!;
            var retval = method.ReturnParameter;

            // Act
            var nullability0 = nullableArg.GetNullability();
            var returnNullability = retval.GetNullability();

            // Assert
            nullability0.Should().Be(Nullability.Nullable);
            returnNullability.Should().Be(Nullability.Nullable);
        }

        [TestMethod] public void StructRestrictedGeneric() {
            // Arrange
            var method = typeof(NullableDisabled).GetMethod(nameof(NullableDisabled.StructRestrictedGeneric))!;
            var nonNullableArg = method.GetParameters()[0]!;
            var nullableArg = method.GetParameters()[1]!;
            var retval = method.ReturnParameter;

            // Act
            var nullability0 = nonNullableArg.GetNullability();
            var nullability1 = nullableArg.GetNullability();
            var returnNullability = retval.GetNullability();

            // Assert
            nullability0.Should().Be(Nullability.NonNullable);
            nullability1.Should().Be(Nullability.Nullable);
            returnNullability.Should().Be(Nullability.NonNullable);
        }

        [TestMethod] public void EnumRestrictedGeneric() {
            // Arrange
            var method = typeof(NullableDisabled).GetMethod(nameof(NullableDisabled.EnumRestrictedGeneric))!;
            var nonNullableArg = method.GetParameters()[0]!;
            var retval = method.ReturnParameter;

            // Act
            var nullability0 = nonNullableArg.GetNullability();
            var returnNullability = retval.GetNullability();

            // Assert
            nullability0.Should().Be(Nullability.NonNullable);
            returnNullability.Should().Be(Nullability.NonNullable);
        }

        [TestMethod] public void NotNullRestrictedGeneric() {
            // Arrange
            var method = typeof(NullableDisabled).GetMethod(nameof(NullableDisabled.NotNullRestrictedGeneric))!;
            var ambiguousArg = method.GetParameters()[0]!;
            var retval = method.ReturnParameter;

            // Act
            var nullability0 = ambiguousArg.GetNullability();
            var returnNullability = retval.GetNullability();

            // Assert
            nullability0.Should().Be(Nullability.Nullable);                 // limitation in reflection capabilities
            returnNullability.Should().Be(Nullability.Nullable);            // limitation in reflection capabilities
        }
    }

    [TestClass, TestCategory("Nullability: Nested Context")]
    public sealed class Nullability_NestedContext {
        [TestMethod] public void NestedEnabledNonNullableRef() {
            // Arrange
            var param = typeof(NullableDisabled).GetMethod(nameof(NullableDisabled.NestedMethod))!.GetParameters()[0]!;
            param.Name.Should().Be("NonNullableRefParam");

            // Act
            var nullability = param.GetNullability();

            // Assert
            nullability.Should().Be(Nullability.NonNullable);
        }

        [TestMethod] public void NestedEnabledNullableRef() {
            // Arrange
            var param = typeof(NullableDisabled).GetMethod(nameof(NullableDisabled.NestedMethod))!.GetParameters()[1]!;
            param.Name.Should().Be("NullableRefParam");

            // Act
            var nullability = param.GetNullability();

            // Assert
            nullability.Should().Be(Nullability.Nullable);
        }

        [TestMethod] public void NestedEnabledNonNullablePrimitive() {
            // Arrange
            var param = typeof(NullableDisabled).GetMethod(nameof(NullableDisabled.NestedMethod))!.GetParameters()[2]!;
            param.Name.Should().Be("NonNullablePrimitiveParam");

            // Act
            var nullability = param.GetNullability();

            // Assert
            nullability.Should().Be(Nullability.NonNullable);
        }

        [TestMethod] public void NestedEnabledNullablePrimitive() {
            // Arrange
            var param = typeof(NullableDisabled).GetMethod(nameof(NullableDisabled.NestedMethod))!.GetParameters()[3]!;
            param.Name.Should().Be("NullablePrimitiveParam");

            // Act
            var nullability = param.GetNullability();

            // Assert
            nullability.Should().Be(Nullability.Nullable);
        }

        [TestMethod] public void NestedEnabledNonNullableEnum() {
            // Arrange
            var param = typeof(NullableDisabled).GetMethod(nameof(NullableDisabled.NestedMethod))!.GetParameters()[4]!;
            param.Name.Should().Be("NonNullableEnumParam");

            // Act
            var nullability = param.GetNullability();

            // Assert
            nullability.Should().Be(Nullability.NonNullable);
        }

        [TestMethod] public void NestedEnabledNullableEnum() {
            // Arrange
            var param = typeof(NullableDisabled).GetMethod(nameof(NullableDisabled.NestedMethod))!.GetParameters()[5]!;
            param.Name.Should().Be("NullableEnumParam");

            // Act
            var nullability = param.GetNullability();

            // Assert
            nullability.Should().Be(Nullability.Nullable);
        }

        [TestMethod] public void NestedEnabledNonNullableDelegate() {
            // Arrange
            var param = typeof(NullableDisabled).GetMethod(nameof(NullableDisabled.NestedMethod))!.GetParameters()[6]!;
            param.Name.Should().Be("NonNullableDelegateParam");

            // Act
            var nullability = param.GetNullability();

            // Assert
            nullability.Should().Be(Nullability.NonNullable);
        }

        [TestMethod] public void NestedEnabledNullableDelegate() {
            // Arrange
            var param = typeof(NullableDisabled).GetMethod(nameof(NullableDisabled.NestedMethod))!.GetParameters()[7]!;
            param.Name.Should().Be("NullableDelegateParam");

            // Act
            var nullability = param.GetNullability();

            // Assert
            nullability.Should().Be(Nullability.Nullable);
        }

        [TestMethod] public void NestedEnabledNonNullableDynamic() {
            // Arrange
            var param = typeof(NullableDisabled).GetMethod(nameof(NullableDisabled.NestedMethod))!.GetParameters()[8]!;
            param.Name.Should().Be("NonNullableDynamicParam");

            // Act
            var nullability = param.GetNullability();

            // Assert
            nullability.Should().Be(Nullability.NonNullable);
        }

        [TestMethod] public void NestedEnabledNullableDynamic() {
            // Arrange
            var param = typeof(NullableDisabled).GetMethod(nameof(NullableDisabled.NestedMethod))!.GetParameters()[9]!;
            param.Name.Should().Be("NullableDynamicParam");

            // Act
            var nullability = param.GetNullability();

            // Assert
            nullability.Should().Be(Nullability.Nullable);
        }

        [TestMethod] public void NestedDisabledReference() {
            // Arrange
            var param = typeof(NullableEnabled).GetMethod(nameof(NullableEnabled.NestedMethod))!.GetParameters()[0]!;
            param.Name.Should().Be("RefParam");

            // Act
            var nullability = param.GetNullability();

            // Assert
            nullability.Should().Be(Nullability.Nullable);
        }

        [TestMethod] public void NestedDisabledNonNullablePrimitive() {
            // Arrange
            var param = typeof(NullableEnabled).GetMethod(nameof(NullableEnabled.NestedMethod))!.GetParameters()[1]!;
            param.Name.Should().Be("NonNullablePrimitiveParam");

            // Act
            var nullability = param.GetNullability();

            // Assert
            nullability.Should().Be(Nullability.NonNullable);
        }

        [TestMethod] public void NestedDisabledNullablePrimitive() {
            // Arrange
            var param = typeof(NullableEnabled).GetMethod(nameof(NullableEnabled.NestedMethod))!.GetParameters()[2]!;
            param.Name.Should().Be("NullablePrimitiveParam");

            // Act
            var nullability = param.GetNullability();

            // Assert
            nullability.Should().Be(Nullability.Nullable);
        }

        [TestMethod] public void NestedDisabledNonNullableEnum() {
            // Arrange
            var param = typeof(NullableEnabled).GetMethod(nameof(NullableEnabled.NestedMethod))!.GetParameters()[3]!;
            param.Name.Should().Be("NonNullableEnumParam");

            // Act
            var nullability = param.GetNullability();

            // Assert
            nullability.Should().Be(Nullability.NonNullable);
        }

        [TestMethod] public void NestedDisabledNullableEnum() {
            // Arrange
            var param = typeof(NullableEnabled).GetMethod(nameof(NullableEnabled.NestedMethod))!.GetParameters()[4]!;
            param.Name.Should().Be("NullableEnumParam");

            // Act
            var nullability = param.GetNullability();

            // Assert
            nullability.Should().Be(Nullability.Nullable);
        }

        [TestMethod] public void NestedDisabledDelegate() {
            // Arrange
            var param = typeof(NullableEnabled).GetMethod(nameof(NullableEnabled.NestedMethod))!.GetParameters()[5]!;
            param.Name.Should().Be("DelegateParam");

            // Act
            var nullability = param.GetNullability();

            // Assert
            nullability.Should().Be(Nullability.Nullable);
        }

        [TestMethod] public void NestedDisabledDynamic() {
            // Arrange
            var param = typeof(NullableEnabled).GetMethod(nameof(NullableEnabled.NestedMethod))!.GetParameters()[6]!;
            param.Name.Should().Be("DynamicParam");

            // Act
            var nullability = param.GetNullability();

            // Assert
            nullability.Should().Be(Nullability.Nullable);
        }
    }
}
