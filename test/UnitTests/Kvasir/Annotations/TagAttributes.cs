using FluentAssertions;
using Kvasir.Annotations;
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

        [TestMethod] public void PrimaryKey_DuplicateWithPath() {
            // Arrange
            var path = "Nested.Path";
            var original = new PrimaryKeyAttribute();

            // Act
            var attr = (original as INestableAnnotation).WithPath(path);

            // Assert
            attr.Should().BeOfType<PrimaryKeyAttribute>();
            attr.Path.Should().Be(path);
        }

        [TestMethod] public void PrimaryKey_UniqueId() {
            // Arrange
            var attr = new PrimaryKeyAttribute();

            // Act
            var isUnique = ids_.Add(attr.TypeId);

            // Assert
            isUnique.Should().BeTrue();
        }

        [TestMethod] public void Numeric_Construct() {
            // Arrange
            var path = "Nested.Path";

            // Act
            var direct = new NumericAttribute();
            var nested = new NumericAttribute() { Path = path };

            // Assert
            direct.Path.Should().BeEmpty();
            nested.Path.Should().Be(path);
        }

        [TestMethod] public void Numeric_DuplicateWithPath() {
            // Arrange
            var path = "Nested.Path";
            var original = new NumericAttribute();

            // Act
            var attr = (original as INestableAnnotation).WithPath(path);

            // Assert
            attr.Should().BeOfType<NumericAttribute>();
            attr.Path.Should().Be(path);
        }

        [TestMethod] public void Numeric_UniqueId() {
            // Arrange
            var attr = new NumericAttribute();

            // Act
            var isUnique = ids_.Add(attr.TypeId);

            // Assert
            isUnique.Should().BeTrue();
        }

        [TestMethod] public void ExcludeNamespace_Construct() {
            // Arrange

            // Act
            _ = new ExcludeNamespaceFromNameAttribute();

            // Assert
        }

        [TestMethod] public void ExcludeNamespace_UniqueId() {
            // Arrange
            var attr = new ExcludeNamespaceFromNameAttribute();

            // Act
            var isUnique = ids_.Add(attr.TypeId);

            // Assert
            isUnique.Should().BeTrue();
        }

        [TestMethod] public void IncludeInModel_Construct() {
            // Arrange

            // Act
            _ = new IncludeInModelAttribute();

            // Assert
        }

        [TestMethod] public void IncludeInModel_UniqueId() {
            // Arrange
            var attr = new IncludeInModelAttribute();

            // Act
            var isUnique = ids_.Add(attr.TypeId);

            // Assert
            isUnique.Should().BeTrue();
        }

        [TestMethod] public void AsString_Construct() {
            // Arrange
            var path = "Nested.Path";

            // Act
            var direct = new AsStringAttribute();
            var nested = new AsStringAttribute() { Path = path };

            // Assert
            direct.Path.Should().BeEmpty();
            nested.Path.Should().Be(path);
        }

        [TestMethod] public void AsString_DuplicateWithPath() {
            // Arrange
            var path = "Nested.Path";
            var original = new AsStringAttribute();

            // Act
            var attr = (original as INestableAnnotation).WithPath(path);

            // Assert
            attr.Should().BeOfType<AsStringAttribute>();
            attr.Path.Should().Be(path);
        }

        [TestMethod] public void AsString_UniqueId() {
            // Arrange
            var attr = new AsStringAttribute();

            // Act
            var isUnique = ids_.Add(attr.TypeId);

            // Assert
            isUnique.Should().BeTrue();
        }
    }
}
