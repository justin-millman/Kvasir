using FluentAssertions;
using Kvasir.Annotations;
using Kvasir.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UT.Kvasir.Annotations {
    [TestClass, TestCategory("Signedness Attributes")]
    public class SignednessAttributeTests : AnnotationTestBase {
        [TestMethod] public void IsNonZero_Direct() {
            // Arrange

            // Act
            var attr = new Check.IsNonZeroAttribute();

            // Assert
            attr.Path.Should().BeEmpty();
            attr.Operator.Should().Be(ComparisonOperator.NE);
        }

        [TestMethod] public void IsNonZero_Nested() {
            // Arrange
            var path = "Nested.Path";

            // Act
            var attr = new Check.IsNonZeroAttribute() { Path = path };

            // Assert
            attr.Path.Should().Be(path);
            attr.Operator.Should().Be(ComparisonOperator.NE);
        }

        [TestMethod] public void IsNonZero_DuplicateWithPath() {
            // Arrange
            var path = "Nested.Path";
            var original = new Check.IsNonZeroAttribute();

            // Act
            var attr = (original as INestableAnnotation).WithPath(path);

            // Assert
            attr.Should().BeOfType<Check.IsNonZeroAttribute>();
            attr.Path.Should().Be(path);
            (attr as Check.IsNonZeroAttribute)!.Operator.Should().Be(original.Operator);
        }

        [TestMethod] public void IsNonZero_UniqueId() {
            // Arrange
            var attr = new Check.IsNonZeroAttribute();

            // Act
            var isUnique = ids_.Add(attr.TypeId);

            // Assert
            isUnique.Should().BeTrue();
        }

        [TestMethod] public void IsPositive_Direct() {
            // Arrange

            // Act
            var attr = new Check.IsPositiveAttribute();

            // Assert
            attr.Path.Should().BeEmpty();
            attr.Operator.Should().Be(ComparisonOperator.GT);
        }

        [TestMethod] public void IsPositive_Nested() {
            // Arrange
            var path = "Nested.Path";

            // Act
            var attr = new Check.IsPositiveAttribute() { Path = path };

            // Assert
            attr.Path.Should().Be(path);
            attr.Operator.Should().Be(ComparisonOperator.GT);
        }

        [TestMethod] public void IsPositive_DuplicateWithPath() {
            // Arrange
            var path = "Nested.Path";
            var original = new Check.IsPositiveAttribute();

            // Act
            var attr = (original as INestableAnnotation).WithPath(path);

            // Assert
            attr.Should().BeOfType<Check.IsPositiveAttribute>();
            attr.Path.Should().Be(path);
            (attr as Check.IsPositiveAttribute)!.Operator.Should().Be(original.Operator);
        }

        [TestMethod] public void IsPositive_UniqueId() {
            // Arrange
            var attr = new Check.IsPositiveAttribute();

            // Act
            var isUnique = ids_.Add(attr.TypeId);

            // Assert
            isUnique.Should().BeTrue();
        }

        [TestMethod] public void IsNegative_Direct() {
            // Arrange

            // Act
            var attr = new Check.IsNegativeAttribute();

            // Assert
            attr.Path.Should().BeEmpty();
            attr.Operator.Should().Be(ComparisonOperator.LT);
        }

        [TestMethod] public void IsNegative_Nested() {
            // Arrange
            var path = "Nested.Path";

            // Act
            var attr = new Check.IsNegativeAttribute() { Path = path };

            // Assert
            attr.Path.Should().Be(path);
            attr.Operator.Should().Be(ComparisonOperator.LT);
        }

        [TestMethod] public void IsNegative_DuplicateWithPath() {
            // Arrange
            var path = "Nested.Path";
            var original = new Check.IsNegativeAttribute();

            // Act
            var attr = (original as INestableAnnotation).WithPath(path);

            // Assert
            attr.Should().BeOfType<Check.IsNegativeAttribute>();
            attr.Path.Should().Be(path);
            (attr as Check.IsNegativeAttribute)!.Operator.Should().Be(original.Operator);
        }

        [TestMethod] public void IsNegative_UniqueId() {
            // Arrange
            var attr = new Check.IsNegativeAttribute();

            // Act
            var isUnique = ids_.Add(attr.TypeId);

            // Assert
            isUnique.Should().BeTrue();
        }
    }
}
