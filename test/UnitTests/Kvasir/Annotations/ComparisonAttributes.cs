using FluentAssertions;
using Kvasir.Annotations;
using Kvasir.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace UT.Kvasir.Annotations {
    [TestClass, TestCategory("Comparison Attributes")]
    public class ComparisonAttributeTests : AnnotationTestBase {
        [TestMethod] public void IsNot_Direct() {
            // Arrange
            var value = "Riverside";

            // Act
            var attr = new Check.IsNotAttribute(value);

            // Assert
            attr.Path.Should().BeEmpty();
            attr.Operator.Should().Be(ComparisonOperator.NE);
            attr.Anchor.Should().Be(value);
        }

        [TestMethod] public void IsNot_Nested() {
            // Arrange
            var path = "Nested.Path";
            var value = "Alpharetta";

            // Act
            var attr = new Check.IsNotAttribute(value) { Path = path };

            // Assert
            attr.Path.Should().Be(path);
            attr.Operator.Should().Be(ComparisonOperator.NE);
            attr.Anchor.Should().Be(value);
        }

        [TestMethod] public void IsNot_Null() {
            // Arrange
            string? value = null;

            // Act
            var attr = new Check.IsNotAttribute(value!);

            // Assert
            attr.Path.Should().BeEmpty();
            attr.Operator.Should().Be(ComparisonOperator.NE);
            attr.Anchor.Should().Be(DBNull.Value);
        }

        [TestMethod] public void IsNot_DuplicateWithPath() {
            // Arrange
            var path = "Nested.Path";
            var original = new Check.IsNotAttribute("Scottsbluff");

            // Act
            var attr = (original as INestableAnnotation).WithPath(path);

            // Assert
            attr.Should().BeOfType<Check.IsNotAttribute>();
            attr.Path.Should().Be(path);
            (attr as Check.IsNotAttribute)!.Operator.Should().Be(original.Operator);
            (attr as Check.IsNotAttribute)!.Anchor.Should().Be(original.Anchor);
        }

        [TestMethod] public void IsNot_UniqueId() {
            // Arrange
            var attr = new Check.IsNotAttribute("Chula Vista");

            // Act
            var isUnique = ids_.Add(attr.TypeId);

            // Assert
            isUnique.Should().BeTrue();
        }

        [TestMethod] public void IsGreaterThan_Direct() {
            // Arrange
            var value = "Wilkes-Barre";

            // Act
            var attr = new Check.IsGreaterThanAttribute(value);

            // Assert
            attr.Path.Should().BeEmpty();
            attr.Operator.Should().Be(ComparisonOperator.GT);
            attr.Anchor.Should().Be(value);
        }

        [TestMethod] public void IsGreaterThan_Nested() {
            // Arrange
            var path = "Nested.Path";
            var value = "Waukesha";

            // Act
            var attr = new Check.IsGreaterThanAttribute(value) { Path = path };

            // Assert
            attr.Path.Should().Be(path);
            attr.Operator.Should().Be(ComparisonOperator.GT);
            attr.Anchor.Should().Be(value);
        }

        [TestMethod] public void IsGreaterThan_Null() {
            // Arrange
            string? value = null;

            // Act
            var attr = new Check.IsGreaterThanAttribute(value!);

            // Assert
            attr.Path.Should().BeEmpty();
            attr.Operator.Should().Be(ComparisonOperator.GT);
            attr.Anchor.Should().Be(DBNull.Value);
        }

        [TestMethod] public void IsGreaterThan_DuplicateWithPath() {
            // Arrange
            var path = "Nested.Path";
            var original = new Check.IsGreaterThanAttribute("Fredericksburg");

            // Act
            var attr = (original as INestableAnnotation).WithPath(path);

            // Assert
            attr.Should().BeOfType<Check.IsGreaterThanAttribute>();
            attr.Path.Should().Be(path);
            (attr as Check.IsGreaterThanAttribute)!.Operator.Should().Be(original.Operator);
            (attr as Check.IsGreaterThanAttribute)!.Anchor.Should().Be(original.Anchor);
        }

        [TestMethod] public void IsGreaterThan_UniqueId() {
            // Arrange
            var attr = new Check.IsGreaterThanAttribute("San Bernardino");

            // Act
            var isUnique = ids_.Add(attr.TypeId);

            // Assert
            isUnique.Should().BeTrue();
        }

        [TestMethod] public void IsGreaterOrEqualTo_Direct() {
            // Arrange
            var value = "Lafayette";

            // Act
            var attr = new Check.IsGreaterOrEqualToAttribute(value);

            // Assert
            attr.Path.Should().BeEmpty();
            attr.Operator.Should().Be(ComparisonOperator.GTE);
            attr.Anchor.Should().Be(value);
        }

        [TestMethod] public void IsGreaterOrEqualTo_Nested() {
            // Arrange
            var path = "Nested.Path";
            var value = "East St. Louis";

            // Act
            var attr = new Check.IsGreaterOrEqualToAttribute(value) { Path = path };

            // Assert
            attr.Path.Should().Be(path);
            attr.Operator.Should().Be(ComparisonOperator.GTE);
            attr.Anchor.Should().Be(value);
        }

        [TestMethod] public void IsGreaterOrEqualTo_Null() {
            // Arrange
            string? value = null;

            // Act
            var attr = new Check.IsGreaterOrEqualToAttribute(value!);

            // Assert
            attr.Path.Should().BeEmpty();
            attr.Operator.Should().Be(ComparisonOperator.GTE);
            attr.Anchor.Should().Be(DBNull.Value);
        }

        [TestMethod] public void IsGreaterOrEqual_DuplicateWithPath() {
            // Arrange
            var path = "Nested.Path";
            var original = new Check.IsGreaterOrEqualToAttribute("Spotsylvania Courthouse");

            // Act
            var attr = (original as INestableAnnotation).WithPath(path);

            // Assert
            attr.Should().BeOfType<Check.IsGreaterOrEqualToAttribute>();
            attr.Path.Should().Be(path);
            (attr as Check.IsGreaterOrEqualToAttribute)!.Operator.Should().Be(original.Operator);
            (attr as Check.IsGreaterOrEqualToAttribute)!.Anchor.Should().Be(original.Anchor);
        }

        [TestMethod] public void IsGreaterOrEqualTo_UniqueId() {
            // Arrange
            var attr = new Check.IsGreaterOrEqualToAttribute("Salinas");

            // Act
            var isUnique = ids_.Add(attr.TypeId);

            // Assert
            isUnique.Should().BeTrue();
        }

        [TestMethod] public void IsLessThan_Direct() {
            // Arrange
            var value = "Pearl City";

            // Act
            var attr = new Check.IsLessThanAttribute(value);

            // Assert
            attr.Path.Should().BeEmpty();
            attr.Operator.Should().Be(ComparisonOperator.LT);
            attr.Anchor.Should().Be(value);
        }

        [TestMethod] public void IsLessThan_Nested() {
            // Arrange
            var path = "Nested.Path";
            var value = "Syracuse";

            // Act
            var attr = new Check.IsLessThanAttribute(value) { Path = path };

            // Assert
            attr.Path.Should().Be(path);
            attr.Operator.Should().Be(ComparisonOperator.LT);
            attr.Anchor.Should().Be(value);
        }

        [TestMethod] public void IsLessThan_Null() {
            // Arrange
            string? value = null;

            // Act
            var attr = new Check.IsLessThanAttribute(value!);

            // Assert
            attr.Path.Should().BeEmpty();
            attr.Operator.Should().Be(ComparisonOperator.LT);
            attr.Anchor.Should().Be(DBNull.Value);
        }

        [TestMethod] public void IsLessThan_DuplicateWithPath() {
            // Arrange
            var path = "Nested.Path";
            var original = new Check.IsLessThanAttribute("Matewan");

            // Act
            var attr = (original as INestableAnnotation).WithPath(path);

            // Assert
            attr.Should().BeOfType<Check.IsLessThanAttribute>();
            attr.Path.Should().Be(path);
            (attr as Check.IsLessThanAttribute)!.Operator.Should().Be(original.Operator);
            (attr as Check.IsLessThanAttribute)!.Anchor.Should().Be(original.Anchor);
        }

        [TestMethod] public void IsLessThan_UniqueId() {
            // Arrange
            var attr = new Check.IsLessThanAttribute("Columbia");

            // Act
            var isUnique = ids_.Add(attr.TypeId);

            // Assert
            isUnique.Should().BeTrue();
        }

        [TestMethod] public void IsLessOrEqualTo_Direct() {
            // Arrange
            var value = "Columbus";

            // Act
            var attr = new Check.IsLessOrEqualToAttribute(value);

            // Assert
            attr.Path.Should().BeEmpty();
            attr.Operator.Should().Be(ComparisonOperator.LTE);
            attr.Anchor.Should().Be(value);
        }

        [TestMethod] public void IsLessOrEqualTo_Nested() {
            // Arrange
            var path = "Nested.Path";
            var value = "Abilene";

            // Act
            var attr = new Check.IsLessOrEqualToAttribute(value) { Path = path };

            // Assert
            attr.Path.Should().Be(path);
            attr.Operator.Should().Be(ComparisonOperator.LTE);
            attr.Anchor.Should().Be(value);
        }

        [TestMethod] public void IsLessOrEqualTo_Null() {
            // Arrange
            string? value = null;

            // Act
            var attr = new Check.IsLessOrEqualToAttribute(value!);

            // Assert
            attr.Path.Should().BeEmpty();
            attr.Operator.Should().Be(ComparisonOperator.LTE);
            attr.Anchor.Should().Be(DBNull.Value);
        }

        [TestMethod] public void IsLessOrEqualTo_DuplicateWithPath() {
            // Arrange
            var path = "Nested.Path";
            var original = new Check.IsLessOrEqualToAttribute("Oswego");

            // Act
            var attr = (original as INestableAnnotation).WithPath(path);

            // Assert
            attr.Should().BeOfType<Check.IsLessOrEqualToAttribute>();
            attr.Path.Should().Be(path);
            (attr as Check.IsLessOrEqualToAttribute)!.Operator.Should().Be(original.Operator);
            (attr as Check.IsLessOrEqualToAttribute)!.Anchor.Should().Be(original.Anchor);
        }

        [TestMethod] public void IsLessOrEqualTo_UniqueId() {
            // Arrange
            var attr = new Check.IsLessOrEqualToAttribute("Palm Beach");

            // Act
            var isUnique = ids_.Add(attr.TypeId);

            // Assert
            isUnique.Should().BeTrue();
        }
    }
}
