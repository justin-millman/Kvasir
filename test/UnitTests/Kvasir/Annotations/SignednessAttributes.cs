using Cybele.Core;
using FluentAssertions;
using Kvasir.Annotations;
using Kvasir.Core;
using Kvasir.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace UT.Kvasir.Annotations {
    [TestClass, TestCategory("Signedness Attributes")]
    public class SignednessAttributeTests : AnnotationTestBase {
        [TestMethod] public void IsNonZero_Direct() {
            // Arrange

            // Act
            var attr = new Check.IsNonZeroAttribute();
            var clause = attr.MakeConstraint(field_.Object, converter_, settings_);

            // Assert
            attr.Path.Should().BeEmpty();
            clause.Should().BeOfType<ConstantClause>();
            (clause as ConstantClause)!.LHS.Function.Should().NotHaveValue();
            (clause as ConstantClause)!.LHS.Field.Should().Be(field_.Object);
            (clause as ConstantClause)!.Operator.Should().Be(ComparisonOperator.NE);
            (clause as ConstantClause)!.RHS.Should().Be(DBValue.Create(0));
        }

        [TestMethod] public void IsNonZero_Nested() {
            // Arrange
            var path = "Nested.Path";

            // Act
            var attr = new Check.IsNonZeroAttribute() { Path = path };
            var clause = attr.MakeConstraint(field_.Object, converter_, settings_);

            // Assert
            attr.Path.Should().Be(path);
            clause.Should().BeOfType<ConstantClause>();
            (clause as ConstantClause)!.LHS.Function.Should().NotHaveValue();
            (clause as ConstantClause)!.LHS.Field.Should().Be(field_.Object);
            (clause as ConstantClause)!.Operator.Should().Be(ComparisonOperator.NE);
            (clause as ConstantClause)!.RHS.Should().Be(DBValue.Create(0));
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
            var clause = attr.MakeConstraint(field_.Object, converter_, settings_);

            // Assert
            attr.Path.Should().BeEmpty();
            clause.Should().BeOfType<ConstantClause>();
            (clause as ConstantClause)!.LHS.Function.Should().NotHaveValue();
            (clause as ConstantClause)!.LHS.Field.Should().Be(field_.Object);
            (clause as ConstantClause)!.Operator.Should().Be(ComparisonOperator.GT);
            (clause as ConstantClause)!.RHS.Should().Be(DBValue.Create(0));
        }

        [TestMethod] public void IsPositive_Nested() {
            // Arrange
            var path = "Nested.Path";

            // Act
            var attr = new Check.IsPositiveAttribute() { Path = path };
            var clause = attr.MakeConstraint(field_.Object, converter_, settings_);

            // Assert
            attr.Path.Should().Be(path);
            clause.Should().BeOfType<ConstantClause>();
            (clause as ConstantClause)!.LHS.Function.Should().NotHaveValue();
            (clause as ConstantClause)!.LHS.Field.Should().Be(field_.Object);
            (clause as ConstantClause)!.Operator.Should().Be(ComparisonOperator.GT);
            (clause as ConstantClause)!.RHS.Should().Be(DBValue.Create(0));
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
            var clause = attr.MakeConstraint(field_.Object, converter_, settings_);

            // Assert
            attr.Path.Should().BeEmpty();
            clause.Should().BeOfType<ConstantClause>();
            (clause as ConstantClause)!.LHS.Function.Should().NotHaveValue();
            (clause as ConstantClause)!.LHS.Field.Should().Be(field_.Object);
            (clause as ConstantClause)!.Operator.Should().Be(ComparisonOperator.LT);
            (clause as ConstantClause)!.RHS.Should().Be(DBValue.Create(0));
        }

        [TestMethod] public void IsNegative_Nested() {
            // Arrange
            var path = "Nested.Path";

            // Act
            var attr = new Check.IsNegativeAttribute() { Path = path };
            var clause = attr.MakeConstraint(field_.Object, converter_, settings_);

            // Assert
            attr.Path.Should().Be(path);
            clause.Should().BeOfType<ConstantClause>();
            (clause as ConstantClause)!.LHS.Function.Should().NotHaveValue();
            (clause as ConstantClause)!.LHS.Field.Should().Be(field_.Object);
            (clause as ConstantClause)!.Operator.Should().Be(ComparisonOperator.LT);
            (clause as ConstantClause)!.RHS.Should().Be(DBValue.Create(0));
        }

        [TestMethod] public void IsNegative_UniqueId() {
            // Arrange
            var attr = new Check.IsNegativeAttribute();

            // Act
            var isUnique = ids_.Add(attr.TypeId);

            // Assert
            isUnique.Should().BeTrue();
        }


        static SignednessAttributeTests() {
            field_ = new Mock<IField>();
            field_.Setup(f => f.DataType).Returns(DBType.Double);
        }

        private static readonly Mock<IField> field_;
        private static readonly Settings settings_ = Settings.Default;
        private static readonly DataConverter converter_ = DataConverter.Identity<double>();
    }
}
