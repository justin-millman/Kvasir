using Cybele.Core;
using FluentAssertions;
using Kvasir.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;

namespace UT.Kvasir.Schema {
    public abstract class NameTestsImpl<T> {
        [TestMethod] public void UsesConceptString() {
            // Arrange
            var type = typeof(T);
            var conceptType = typeof(ConceptString<>);

            // Act
            var baseType = type.BaseType;
            while (baseType is not null) {
                if (baseType.IsGenericType && baseType.GetGenericTypeDefinition() == conceptType) {
                    break;
                }
                baseType = baseType.BaseType;
            }

            // Assert
            baseType.Should().NotBeNull();
        }

        [TestMethod] public void WhitespaceIsStripped() {
            // Arrange
            var rawName = "   Name\t\n\r\n";

            // Act
            var name = Create(rawName)!;
            var trimmed = rawName.Trim();
            var asString = name.ToString();

            // Assert
            asString.Should().Be(trimmed);
        }

        [TestMethod] public void EmptyIsIllegal() {
            // Arrange
            var rawName = string.Empty;

            // Act
            Func<T> action = () => Create(rawName);

            // Assert
            action.Should().ThrowExactly<ArgumentException>()
                .WithAnyMessage()
                .And
                .ParamName.Should().NotBeNullOrEmpty();
        }

        [TestMethod] public void WhitespaceOnlyIsIllegal() {
            // Arrange
            var rawName = "   \t    \r\n   ";

            // Act
            Func<T> action = () => Create(rawName);

            // Assert
            action.Should().ThrowExactly<ArgumentException>()
                .WithAnyMessage()
                .And
                .ParamName.Should().NotBeNullOrEmpty();
        }


        private static T Create(string rawName) {
            try {
                return (T)Activator.CreateInstance(typeof(T), new object?[] { rawName })!;
            }
            catch (TargetInvocationException ex) {
                throw ex.GetBaseException();
            }
        }
    }

    [TestClass, TestCategory("CheckName")] public class CheckNameTests : NameTestsImpl<CheckName> {}
    [TestClass, TestCategory("FieldName")] public class FieldNameTests : NameTestsImpl<FieldName> {}
    [TestClass, TestCategory("FKName")] public class FKNameTests : NameTestsImpl<FKName> {}
    [TestClass, TestCategory("KeyName")] public class KeyNameTests : NameTestsImpl<KeyName> {}
    [TestClass, TestCategory("TableName")] public class TableNameTests : NameTestsImpl<TableName> {}
}
