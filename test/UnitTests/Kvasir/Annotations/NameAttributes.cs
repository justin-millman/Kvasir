using FluentAssertions;
using Kvasir.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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

        [TestMethod] public void FieldName_DuplicateWithPath() {
            // Arrange
            var path = "Nested.Path";
            var original = new NameAttribute("Field");

            // Act
            var attr = (original as INestableAnnotation).WithPath(path);

            // Assert
            attr.Should().BeOfType<NameAttribute>();
            attr.Path.Should().Be(path);
            (attr as NameAttribute)!.Name.Should().Be(original.Name);
        }

        [TestMethod] public void FieldName_UniqueId() {
            // Arrange
            var attr = new NameAttribute("Field");

            // Act
            var isUnique = ids_.Add(attr.TypeId);

            // Assert
            isUnique.Should().BeTrue();
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

        [TestMethod] public void RelationTableName() {
            // Arrange
            var value = "RelationTable";

            // Act
            var attr = new RelationTableAttribute(value);

            // Assert
            attr.Name.ToString().Should().Be(value);
        }

        [TestMethod] public void RelationTableName_UniqueId() {
            // Arrange
            var attr = new RelationTableAttribute("RelationTable");

            // Act
            var isUnique = ids_.Add(attr.TypeId);

            // Assert
            isUnique.Should().BeTrue();
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
    }
}
