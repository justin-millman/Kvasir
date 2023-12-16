using FluentAssertions;
using Kvasir.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UT.Kvasir.Annotations {
    [TestClass, TestCategory("String Length Attributes")]
    public class StringLengthAttributeTests : AnnotationTestBase {
        [TestMethod] public void IsNotEmpty_Direct() {
            // Arrange

            // Act
            var attr = new Check.IsNonEmptyAttribute();

            // Assert
            attr.Path.Should().BeEmpty();
            attr.Minimum.Should().Be(1);
            attr.Maximum.Should().Be(long.MaxValue);
        }

        [TestMethod] public void IsNotEmpty_Nested() {
            // Arrange
            var path = "Nested.Path";

            // Act
            var attr = new Check.IsNonEmptyAttribute() { Path = path };

            // Assert
            attr.Path.Should().Be(path);
            attr.Minimum.Should().Be(1);
            attr.Maximum.Should().Be(long.MaxValue);
        }

        [TestMethod] public void IsNonEmpty_DuplicateWithPath() {
            // Arrange
            var path = "Nested.Path";
            var original = new Check.IsNonEmptyAttribute();

            // Act
            var attr = (original as INestableAnnotation).WithPath(path);

            // Assert
            attr.Should().BeOfType<Check.IsNonEmptyAttribute>();
            attr.Path.Should().Be(path);
            (attr as Check.IsNonEmptyAttribute)!.Minimum.Should().Be(original.Minimum);
            (attr as Check.IsNonEmptyAttribute)!.Maximum.Should().Be(original.Maximum);
        }

        [TestMethod] public void IsNotEmpty_UniqueId() {
            // Arrange
            var attr = new Check.IsNonEmptyAttribute();

            // Act
            var isUnique = ids_.Add(attr.TypeId);

            // Assert
            isUnique.Should().BeTrue();
        }

        [TestMethod] public void LengthIsAtLeast_Direct() {
            // Arrange
            var bound = 418;

            // Act
            var attr = new Check.LengthIsAtLeastAttribute(bound);

            // Assert
            attr.Path.Should().BeEmpty();
            attr.Minimum.Should().Be(bound);
            attr.Maximum.Should().Be(long.MaxValue);
        }

        [TestMethod] public void LengthIsAtLeast_Nested() {
            // Arrange
            var bound = 211;
            var path = "Nested.Path";

            // Act
            var attr = new Check.LengthIsAtLeastAttribute(bound) { Path = path };

            // Assert
            attr.Path.Should().Be(path);
            attr.Minimum.Should().Be(bound);
            attr.Maximum.Should().Be(long.MaxValue);
        }

        [TestMethod] public void LengthIsAtLeast_DuplicateWithPath() {
            // Arrange
            var path = "Nested.Path";
            var original = new Check.LengthIsAtLeastAttribute(17);

            // Act
            var attr = (original as INestableAnnotation).WithPath(path);

            // Assert
            attr.Should().BeOfType<Check.LengthIsAtLeastAttribute>();
            attr.Path.Should().Be(path);
            (attr as Check.LengthIsAtLeastAttribute)!.Minimum.Should().Be(original.Minimum);
            (attr as Check.LengthIsAtLeastAttribute)!.Maximum.Should().Be(original.Maximum);
        }

        [TestMethod] public void LengthIsAtLeast_UniqueId() {
            // Arrange
            var attr = new Check.LengthIsAtLeastAttribute(19);

            // Act
            var isUnique = ids_.Add(attr.TypeId);

            // Assert
            isUnique.Should().BeTrue();
        }

        [TestMethod] public void LengthIsAtMost_Direct() {
            // Arrange
            var bound = 22;

            // Act
            var attr = new Check.LengthIsAtMostAttribute(bound);

            // Assert
            attr.Path.Should().BeEmpty();
            attr.Minimum.Should().Be(long.MinValue);
            attr.Maximum.Should().Be(bound);
        }

        [TestMethod] public void LengthIsAtMost_Nested() {
            // Arrange
            var bound = 41728300;
            var path = "Nested.Path";

            // Act
            var attr = new Check.LengthIsAtMostAttribute(bound) { Path = path };

            // Assert
            attr.Path.Should().Be(path);
            attr.Minimum.Should().Be(long.MinValue);
            attr.Maximum.Should().Be(bound);
        }

        [TestMethod] public void LengthIsAtMost_DuplicateWithPath() {
            // Arrange
            var path = "Nested.Path";
            var original = new Check.LengthIsAtMostAttribute(4918251);

            // Act
            var attr = (original as INestableAnnotation).WithPath(path);

            // Assert
            attr.Should().BeOfType<Check.LengthIsAtMostAttribute>();
            attr.Path.Should().Be(path);
            (attr as Check.LengthIsAtMostAttribute)!.Minimum.Should().Be(original.Minimum);
            (attr as Check.LengthIsAtMostAttribute)!.Maximum.Should().Be(original.Maximum);
        }

        [TestMethod] public void LengthIsAtMost_UniqueId() {
            // Arrange
            var attr = new Check.LengthIsAtMostAttribute(19);

            // Act
            var isUnique = ids_.Add(attr.TypeId);

            // Assert
            isUnique.Should().BeTrue();
        }

        [TestMethod] public void LengthIsBetween_Direct() {
            // Arrange
            var lower = 7;
            var upper = 19;

            // Act
            var attr = new Check.LengthIsBetweenAttribute(lower, upper);

            // Assert
            attr.Path.Should().BeEmpty();
            attr.Minimum.Should().Be(lower);
            attr.Maximum.Should().Be(upper);
        }

        [TestMethod] public void LengthIsBetween_Nested() {
            // Arrange
            var path = "Nested.Path";
            var lower = 44;
            var upper = 1902;

            // Act
            var attr = new Check.LengthIsBetweenAttribute(lower, upper) { Path = path };

            // Assert
            attr.Path.Should().Be(path);
            attr.Minimum.Should().Be(lower);
            attr.Maximum.Should().Be(upper);
        }

        [TestMethod] public void LengthIsBetween_DuplicateWithPath() {
            // Arrange
            var path = "Nested.Path";
            var original = new Check.LengthIsBetweenAttribute(189, 5124910);

            // Act
            var attr = (original as INestableAnnotation).WithPath(path);

            // Assert
            attr.Should().BeOfType<Check.LengthIsBetweenAttribute>();
            attr.Path.Should().Be(path);
            (attr as Check.LengthIsBetweenAttribute)!.Minimum.Should().Be(original.Minimum);
            (attr as Check.LengthIsBetweenAttribute)!.Maximum.Should().Be(original.Maximum);
        }

        [TestMethod] public void LengthIsBetween_UniqueId() {
            // Arrange
            var attr = new Check.LengthIsBetweenAttribute(22, 25);

            // Act
            var isUnique = ids_.Add(attr.TypeId);

            // Assert
            isUnique.Should().BeTrue();
        }
    }
}
