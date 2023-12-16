using FluentAssertions;
using Kvasir.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UT.Kvasir.Annotations {
    [TestClass, TestCategory("Unique Attribute")]
    public class UniqueAttributeTests : AnnotationTestBase {
        [TestMethod] public void UniqueWithName_Direct() {
            // Arrange
            var name = "UNIQUE_CONSTRAINT";

            // Act
            var attr = new UniqueAttribute(name);

            // Assert
            attr.IsAnonymous.Should().BeFalse();
            attr.Path.Should().BeEmpty();
            attr.Name.ToString().Should().Be(name);
        }

        [TestMethod] public void UniqueWithName_Indirect() {
            // Arrange
            var path = "Nested.Path";
            var name = "UNIQUE_CONSTRAINT";

            // Act
            var attr = new UniqueAttribute(name) { Path = path };

            // Assert
            attr.IsAnonymous.Should().BeFalse();
            attr.Path.Should().Be(path);
            attr.Name.ToString().Should().Be(name);
        }

        [TestMethod] public void UniqueWithoutName_Direct() {
            // Arrange

            // Act
            var attr = new UniqueAttribute();

            // Assert
            attr.IsAnonymous.Should().BeTrue();
            attr.Path.Should().BeEmpty();
            attr.Name.ToString().Should().StartWith(UniqueAttribute.ANONYMOUS_PREFIX);
        }

        [TestMethod] public void UniqueWithoutName_Indirect() {
            // Arrange
            var path = "Nested.Path";

            // Act
            var attr = new UniqueAttribute() { Path = path };

            // Assert
            attr.IsAnonymous.Should().BeTrue();
            attr.Path.Should().Be(path);
            attr.Name.ToString().Should().StartWith(UniqueAttribute.ANONYMOUS_PREFIX);
        }

        [TestMethod] public void Unique_DuplicateWithPath() {
            // Arrange
            var path = "Nested.Path";
            var original = new UniqueAttribute();

            // Act
            var attr = (original as INestableAnnotation).WithPath(path);

            // Assert
            attr.Should().BeOfType<UniqueAttribute>();
            attr.Path.Should().Be(path);
            (attr as UniqueAttribute)!.Name.Should().Be(original.Name);
            (attr as UniqueAttribute)!.IsAnonymous.Should().Be(original.IsAnonymous);
        }

        [TestMethod] public void Unique_UniqueId() {
            // Arrange
            var attr = new UniqueAttribute();

            // Act
            var isUnique = ids_.Add(attr.TypeId);

            // Assert
            isUnique.Should().BeTrue();
        }
    }
}
