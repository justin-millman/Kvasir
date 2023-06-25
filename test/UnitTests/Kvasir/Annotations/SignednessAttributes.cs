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
