using Cybele.Core;
using FluentAssertions;
using Kvasir.Annotations;
using Kvasir.Core;
using Kvasir.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace UT.Kvasir.Annotations {
    [TestClass, TestCategory("Comparison Attributes")]
    public class ComparisonAttributeTests : AnnotationTestBase {
        [TestMethod] public void IsNot_Direct() {
            // Arrange
            var value = "Riverside";

            // Act
            var attr = new Check.IsNotAttribute(value);
            var clause = attr.MakeConstraint(field_.Object, converter_, settings_);

            // Assert
            attr.Path.Should().BeEmpty();
            clause.Should().BeOfType<ConstantClause>();
            (clause as ConstantClause)!.LHS.Function.Should().NotHaveValue();
            (clause as ConstantClause)!.LHS.Field.Should().Be(field_.Object);
            (clause as ConstantClause)!.Operator.Should().Be(ComparisonOperator.NE);
            (clause as ConstantClause)!.RHS.Should().Be(DBValue.Create(value));
        }

        [TestMethod] public void IsNot_Nested() {
            // Arrange
            var path = "Nested.Path";
            var value = "Alpharetta";

            // Act
            var attr = new Check.IsNotAttribute(value) { Path = path };
            var clause = attr.MakeConstraint(field_.Object, converter_, settings_);

            // Assert
            attr.Path.Should().Be(path);
            clause.Should().BeOfType<ConstantClause>();
            (clause as ConstantClause)!.LHS.Function.Should().NotHaveValue();
            (clause as ConstantClause)!.LHS.Field.Should().Be(field_.Object);
            (clause as ConstantClause)!.Operator.Should().Be(ComparisonOperator.NE);
            (clause as ConstantClause)!.RHS.Should().Be(DBValue.Create(value));
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
            var clause = attr.MakeConstraint(field_.Object, converter_, settings_);

            // Assert
            attr.Path.Should().BeEmpty();
            clause.Should().BeOfType<ConstantClause>();
            (clause as ConstantClause)!.LHS.Function.Should().NotHaveValue();
            (clause as ConstantClause)!.LHS.Field.Should().Be(field_.Object);
            (clause as ConstantClause)!.Operator.Should().Be(ComparisonOperator.GT);
            (clause as ConstantClause)!.RHS.Should().Be(DBValue.Create(value));
        }

        [TestMethod] public void IsGreaterThan_Nested() {
            // Arrange
            var path = "Nested.Path";
            var value = "Waukesha";

            // Act
            var attr = new Check.IsGreaterThanAttribute(value) { Path = path };
            var clause = attr.MakeConstraint(field_.Object, converter_, settings_);

            // Assert
            attr.Path.Should().Be(path);
            clause.Should().BeOfType<ConstantClause>();
            (clause as ConstantClause)!.LHS.Function.Should().NotHaveValue();
            (clause as ConstantClause)!.LHS.Field.Should().Be(field_.Object);
            (clause as ConstantClause)!.Operator.Should().Be(ComparisonOperator.GT);
            (clause as ConstantClause)!.RHS.Should().Be(DBValue.Create(value));
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
            var clause = attr.MakeConstraint(field_.Object, converter_, settings_);

            // Assert
            attr.Path.Should().BeEmpty();
            clause.Should().BeOfType<ConstantClause>();
            (clause as ConstantClause)!.LHS.Function.Should().NotHaveValue();
            (clause as ConstantClause)!.LHS.Field.Should().Be(field_.Object);
            (clause as ConstantClause)!.Operator.Should().Be(ComparisonOperator.GTE);
            (clause as ConstantClause)!.RHS.Should().Be(DBValue.Create(value));
        }

        [TestMethod] public void IsGreaterOrEqualTo_Nested() {
            // Arrange
            var path = "Nested.Path";
            var value = "East St. Louis";

            // Act
            var attr = new Check.IsGreaterOrEqualToAttribute(value) { Path = path };
            var clause = attr.MakeConstraint(field_.Object, converter_, settings_);

            // Assert
            attr.Path.Should().Be(path);
            clause.Should().BeOfType<ConstantClause>();
            (clause as ConstantClause)!.LHS.Function.Should().NotHaveValue();
            (clause as ConstantClause)!.LHS.Field.Should().Be(field_.Object);
            (clause as ConstantClause)!.Operator.Should().Be(ComparisonOperator.GTE);
            (clause as ConstantClause)!.RHS.Should().Be(DBValue.Create(value));
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
            var clause = attr.MakeConstraint(field_.Object, converter_, settings_);

            // Assert
            attr.Path.Should().BeEmpty();
            clause.Should().BeOfType<ConstantClause>();
            (clause as ConstantClause)!.LHS.Function.Should().NotHaveValue();
            (clause as ConstantClause)!.LHS.Field.Should().Be(field_.Object);
            (clause as ConstantClause)!.Operator.Should().Be(ComparisonOperator.LT);
            (clause as ConstantClause)!.RHS.Should().Be(DBValue.Create(value));
        }

        [TestMethod] public void IsLessThan_Nested() {
            // Arrange
            var path = "Nested.Path";
            var value = "Syracuse";

            // Act
            var attr = new Check.IsLessThanAttribute(value) { Path = path };
            var clause = attr.MakeConstraint(field_.Object, converter_, settings_);

            // Assert
            attr.Path.Should().Be(path);
            clause.Should().BeOfType<ConstantClause>();
            (clause as ConstantClause)!.LHS.Function.Should().NotHaveValue();
            (clause as ConstantClause)!.LHS.Field.Should().Be(field_.Object);
            (clause as ConstantClause)!.Operator.Should().Be(ComparisonOperator.LT);
            (clause as ConstantClause)!.RHS.Should().Be(DBValue.Create(value));
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
            var clause = attr.MakeConstraint(field_.Object, converter_, settings_);

            // Assert
            attr.Path.Should().BeEmpty();
            clause.Should().BeOfType<ConstantClause>();
            (clause as ConstantClause)!.LHS.Function.Should().NotHaveValue();
            (clause as ConstantClause)!.LHS.Field.Should().Be(field_.Object);
            (clause as ConstantClause)!.Operator.Should().Be(ComparisonOperator.LTE);
            (clause as ConstantClause)!.RHS.Should().Be(DBValue.Create(value));
        }

        [TestMethod] public void IsLessOrEqualTo_Nested() {
            // Arrange
            var path = "Nested.Path";
            var value = "Abilene";

            // Act
            var attr = new Check.IsLessOrEqualToAttribute(value) { Path = path };
            var clause = attr.MakeConstraint(field_.Object, converter_, settings_);

            // Assert
            attr.Path.Should().Be(path);
            clause.Should().BeOfType<ConstantClause>();
            (clause as ConstantClause)!.LHS.Function.Should().NotHaveValue();
            (clause as ConstantClause)!.LHS.Field.Should().Be(field_.Object);
            (clause as ConstantClause)!.Operator.Should().Be(ComparisonOperator.LTE);
            (clause as ConstantClause)!.RHS.Should().Be(DBValue.Create(value));
        }

        [TestMethod] public void IsLessOrEqualTo_UniqueId() {
            // Arrange
            var attr = new Check.IsLessOrEqualToAttribute("Palm Beach");

            // Act
            var isUnique = ids_.Add(attr.TypeId);

            // Assert
            isUnique.Should().BeTrue();
        }


        static ComparisonAttributeTests() {
            field_ = new Mock<IField>();
            field_.Setup(f => f.DataType).Returns(DBType.Text);
        }

        private static readonly Mock<IField> field_;
        private static readonly Settings settings_ = Settings.Default;
        private static readonly DataConverter converter_ = DataConverter.Identity<string>();
    }
}
