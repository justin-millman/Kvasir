using Kvasir.Annotations;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UT.Kvasir.Annotations {
    [TestClass, TestCategory("Tag Attributes")]
    public class TagAttributeTests : AnnotationTestBase {
        [TestMethod] public void Calculated_Construct() {
            // Arrange

            // Act
            _ = new CalculatedAttribute();

            // Assert
        }

        [TestMethod] public void Calculated_UniqueId() {
            // Arrange
            var attr = new CalculatedAttribute();

            // Act
            var isUnique = ids_.Add(attr.TypeId);

            // Assert
            isUnique.Should().BeTrue();
        }

        [TestMethod] public void CodeOnly_Construct() {
            // Arrange

            // Act
            _ = new CodeOnlyAttribute();

            // Assert
        }

        [TestMethod] public void CodeOnly_UniqueId() {
            // Arrange
            var attr = new CodeOnlyAttribute();

            // Act
            var isUnique = ids_.Add(attr.TypeId);

            // Assert
            isUnique.Should().BeTrue();
        }

        [TestMethod] public void NonNullable_Construct() {
            // Arrange

            // Act
            _ = new NonNullableAttribute();

            // Assert
        }

        [TestMethod] public void NonNullable_UniqueId() {
            // Arrange
            var attr = new NonNullableAttribute();

            // Act
            var isUnique = ids_.Add(attr.TypeId);

            // Assert
            isUnique.Should().BeTrue();
        }

        [TestMethod] public void Nullable_Construct() {
            // Arrange

            // Act
            _ = new NullableAttribute();

            // Assert
        }

        [TestMethod] public void Nullable_UniqueId() {
            // Arrange
            var attr = new NullableAttribute();

            // Act
            var isUnique = ids_.Add(attr.TypeId);

            // Assert
            isUnique.Should().BeTrue();
        }

        [TestMethod] public void PrimaryKey_Construct() {
            // Arrange
            var path = "Nested.Path";

            // Act
            var direct = new PrimaryKeyAttribute();
            var nested = new PrimaryKeyAttribute() { Path = path };

            // Assert
            direct.Path.Should().BeEmpty();
            nested.Path.Should().Be(path);
        }

        [TestMethod] public void PrimaryKey_UniqueId() {
            // Arrange
            var attr = new PrimaryKeyAttribute();

            // Act
            var isUnique = ids_.Add(attr.TypeId);

            // Assert
            isUnique.Should().BeTrue();
        }
    }
}
