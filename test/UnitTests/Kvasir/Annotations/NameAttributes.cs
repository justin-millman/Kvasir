using FluentAssertions;
using Kvasir.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace UT.Kvasir.Annotations {
    [TestClass, TestCategory("Name-Override Attributes")]
    public class NameOverrideAttributes : AnnotationTestBase {
        [TestMethod] public void FieldName_Direct() {
            // Arrange
            var value = "Field";

            // Act
            var attr = new NameAttribute(value);

            // Assert
            attr.Name.ToString().Should().Be(value);
            attr.Path.Should().BeEmpty();
        }

        [TestMethod] public void FieldName_Nested() {
            // Arrange
            var value = "Field";
            var path = "Nested.Path";

            // Act
            var attr = new NameAttribute(value) { Path = path };

            // Assert
            attr.Name.ToString().Should().Be(value);
            attr.Path.Should().Be(path);
        }

        [TestMethod] public void FieldName_UniqueId() {
            // Arrange
            var attr = new NameAttribute("Field");

            // Act
            var isUnique = ids_.Add(attr.TypeId);

            // Assert
            isUnique.Should().BeTrue();
        }
        
        [TestMethod] public void FieldName_Invalid() {
            // Arrange
            var invalid = "";

            // Act
            Action act = () => new NameAttribute(invalid);

            // Assert
            act.Should().ThrowExactly<ArgumentException>().WithAnyMessage();
        }

        [TestMethod] public void TableName() {
            // Arrange
            var value = "Table";

            // Act
            var attr = new TableAttribute(value);

            // Assert
            attr.Name.ToString().Should().Be(value);
        }

        [TestMethod] public void TableName_UniqueId() {
            // Arrange
            var attr = new TableAttribute("Table");

            // Act
            var isUnique = ids_.Add(attr.TypeId);

            // Assert
            isUnique.Should().BeTrue();
        }

        [TestMethod] public void TableName_Invalid() {
            // Arrange
            var invalid = "";

            // Act
            Action act = () => new TableAttribute(invalid);

            // Assert
            act.Should().ThrowExactly<ArgumentException>().WithAnyMessage();
        }

        [TestMethod] public void NamedPrimaryKey() {
            // Arrange
            var value = "Key";

            // Act
            var attr = new NamedPrimaryKeyAttribute(value);

            // Assert
            attr.Name.ToString().Should().Be(value);
        }

        [TestMethod] public void NamedPrimaryKey_UniqueId() {
            // Arrange
            var attr = new NamedPrimaryKeyAttribute("Key");

            // Act
            var isUnique = ids_.Add(attr.TypeId);

            // Assert
            isUnique.Should().BeTrue();
        }

        [TestMethod] public void NamedPrimaryKey_Invalid() {
            // Arrange
            var invalid = "";

            // Act
            Action act = () => new NamedPrimaryKeyAttribute(invalid);

            // Assert
            act.Should().ThrowExactly<ArgumentException>().WithAnyMessage();
        }

        [TestMethod] public void PathSeparator() {
            // Arrange
            var value = ':';

            // Act
            var attr = new PathSeparatorAttribute(value);

            // Assert
            attr.Separator.Should().Be(value);
        }

        [TestMethod] public void PathSeparator_UniqueId() {
            // Arrange
            var attr = new PathSeparatorAttribute(' ');

            // Act
            var isUnique = ids_.Add(attr.TypeId);

            // Assert
            isUnique.Should().BeTrue();
        }
    }
}
