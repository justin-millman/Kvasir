using Cybele.Core;
using FluentAssertions;
using Kvasir.Annotations;
using Kvasir.Core;
using Kvasir.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;

namespace UT.Kvasir.Annotations {
    [TestClass, TestCategory("String Length Attributes")]
    public class StringLengthAttributeTests : AnnotationTestBase {
        [TestMethod] public void IsNotEmpty_Direct() {
            // Arrange

            // Act
            var attr = new Check.IsNonEmptyAttribute();
            var clause = attr.MakeConstraint(field_.Object, converter_, settings_);

            // Assert
            attr.Path.Should().BeEmpty();
            clause.Should().BeOfType<ConstantClause>();
            (clause as ConstantClause)!.LHS.Function.Should().HaveValue(FieldFunction.LengthOf);
            (clause as ConstantClause)!.LHS.Field.Should().Be(field_.Object);
            (clause as ConstantClause)!.Operator.Should().Be(ComparisonOperator.GTE);
            (clause as ConstantClause)!.RHS.Should().Be(DBValue.Create(1));
        }

        [TestMethod] public void IsNotEmpty_Nested() {
            // Arrange
            var path = "Nested.Path";

            // Act
            var attr = new Check.IsNonEmptyAttribute() { Path = path };
            var clause = attr.MakeConstraint(field_.Object, converter_, settings_);

            // Assert
            attr.Path.Should().Be(path);
            clause.Should().BeOfType<ConstantClause>();
            (clause as ConstantClause)!.LHS.Function.Should().HaveValue(FieldFunction.LengthOf);
            (clause as ConstantClause)!.LHS.Field.Should().Be(field_.Object);
            (clause as ConstantClause)!.Operator.Should().Be(ComparisonOperator.GTE);
            (clause as ConstantClause)!.RHS.Should().Be(DBValue.Create(1));
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
            var bound = 1;

            // Act
            var attr = new Check.LengthIsAtLeastAttribute(1);
            var clause = attr.MakeConstraint(field_.Object, converter_, settings_);

            // Assert
            attr.Path.Should().BeEmpty();
            clause.Should().BeOfType<ConstantClause>();
            (clause as ConstantClause)!.LHS.Function.Should().HaveValue(FieldFunction.LengthOf);
            (clause as ConstantClause)!.LHS.Field.Should().Be(field_.Object);
            (clause as ConstantClause)!.Operator.Should().Be(ComparisonOperator.GTE);
            (clause as ConstantClause)!.RHS.Should().Be(DBValue.Create(bound));
        }

        [TestMethod] public void LengthIsAtLeast_Nested() {
            // Arrange
            var bound = 211;
            var path = "Nested.Path";

            // Act
            var attr = new Check.LengthIsAtLeastAttribute(bound) { Path = path };
            var clause = attr.MakeConstraint(field_.Object, converter_, settings_);

            // Assert
            attr.Path.Should().Be(path);
            clause.Should().BeOfType<ConstantClause>();
            (clause as ConstantClause)!.LHS.Function.Should().HaveValue(FieldFunction.LengthOf);
            (clause as ConstantClause)!.LHS.Field.Should().Be(field_.Object);
            (clause as ConstantClause)!.Operator.Should().Be(ComparisonOperator.GTE);
            (clause as ConstantClause)!.RHS.Should().Be(DBValue.Create(bound));
        }

        [TestMethod] public void LengthIsAtLeast_UniqueId() {
            // Arrange
            var attr = new Check.LengthIsAtLeastAttribute(19);

            // Act
            var isUnique = ids_.Add(attr.TypeId);

            // Assert
            isUnique.Should().BeTrue();
        }

        [TestMethod] public void LengthIsAtLeast_NegativeBound() {
            // Arrange
            var bound = -83;

            // Act
            Action act = () => new Check.LengthIsAtLeastAttribute(bound);

            // Assert
            act.Should().ThrowExactly<ArgumentException>().WithAnyMessage();
        }

        [TestMethod] public void LengthIsAtMost_Direct() {
            // Arrange
            var bound = 1;

            // Act
            var attr = new Check.LengthIsAtMostAttribute(1);
            var clause = attr.MakeConstraint(field_.Object, converter_, settings_);

            // Assert
            attr.Path.Should().BeEmpty();
            clause.Should().BeOfType<ConstantClause>();
            (clause as ConstantClause)!.LHS.Function.Should().HaveValue(FieldFunction.LengthOf);
            (clause as ConstantClause)!.LHS.Field.Should().Be(field_.Object);
            (clause as ConstantClause)!.Operator.Should().Be(ComparisonOperator.LTE);
            (clause as ConstantClause)!.RHS.Should().Be(DBValue.Create(bound));
        }

        [TestMethod] public void LengthIsAtMost_Nested() {
            // Arrange
            var bound = 211;
            var path = "Nested.Path";

            // Act
            var attr = new Check.LengthIsAtMostAttribute(bound) { Path = path };
            var clause = attr.MakeConstraint(field_.Object, converter_, settings_);

            // Assert
            attr.Path.Should().Be(path);
            clause.Should().BeOfType<ConstantClause>();
            (clause as ConstantClause)!.LHS.Function.Should().HaveValue(FieldFunction.LengthOf);
            (clause as ConstantClause)!.LHS.Field.Should().Be(field_.Object);
            (clause as ConstantClause)!.Operator.Should().Be(ComparisonOperator.LTE);
            (clause as ConstantClause)!.RHS.Should().Be(DBValue.Create(bound));
        }

        [TestMethod] public void LengthIsAtMost_UniqueId() {
            // Arrange
            var attr = new Check.LengthIsAtMostAttribute(19);

            // Act
            var isUnique = ids_.Add(attr.TypeId);

            // Assert
            isUnique.Should().BeTrue();
        }

        [TestMethod] public void LengthIsAtMost_NegativeBound() {
            // Arrange
            var bound = -83;

            // Act
            Action act = () => new Check.LengthIsAtMostAttribute(bound);

            // Assert
            act.Should().ThrowExactly<ArgumentException>().WithAnyMessage();
        }

        [TestMethod] public void LengthIsAtMost_ZeroBound() {
            // Arrange
            var bound = 0;

            // Act
            Action act = () => new Check.LengthIsAtMostAttribute(bound);

            // Assert
            act.Should().ThrowExactly<ArgumentException>().WithAnyMessage();
        }

        [TestMethod] public void LengthIsBetween_Direct() {
            // Arrange
            var lower = 7;
            var upper = 19;

            // Act
            var attr = new Check.LengthIsBetweenAttribute(lower, upper);
            var clause = attr.MakeConstraint(new IField[] { field_.Object }, new DataConverter[] { converter_ }, settings_);

            // Assert
            attr.Path.Should().BeEmpty();
            clause.Should().BeOfType<AndClause>();
            (clause as AndClause)!.LHS.Should().BeOfType<ConstantClause>();
            ((clause as AndClause)!.LHS as ConstantClause)!.LHS.Function.Should().HaveValue(FieldFunction.LengthOf);
            ((clause as AndClause)!.LHS as ConstantClause)!.LHS.Field.Should().Be(field_.Object);
            ((clause as AndClause)!.LHS as ConstantClause)!.Operator.Should().Be(ComparisonOperator.GTE);
            ((clause as AndClause)!.LHS as ConstantClause)!.RHS.Should().Be(DBValue.Create(lower));
            (clause as AndClause)!.RHS.Should().BeOfType<ConstantClause>();
            ((clause as AndClause)!.RHS as ConstantClause)!.LHS.Function.Should().HaveValue(FieldFunction.LengthOf);
            ((clause as AndClause)!.RHS as ConstantClause)!.LHS.Field.Should().Be(field_.Object);
            ((clause as AndClause)!.RHS as ConstantClause)!.Operator.Should().Be(ComparisonOperator.LTE);
            ((clause as AndClause)!.RHS as ConstantClause)!.RHS.Should().Be(DBValue.Create(upper));
        }

        [TestMethod] public void LengthIsBetween_Nested() {
            // Arrange
            var path = "Nested.Path";
            var lower = 44;
            var upper = 1902;

            // Act
            var attr = new Check.LengthIsBetweenAttribute(lower, upper) { Path = path };
            var clause = attr.MakeConstraint(new IField[] { field_.Object }, new DataConverter[] { converter_ }, settings_);

            // Assert
            attr.Path.Should().Be(path);
            clause.Should().BeOfType<AndClause>();
            (clause as AndClause)!.LHS.Should().BeOfType<ConstantClause>();
            ((clause as AndClause)!.LHS as ConstantClause)!.LHS.Function.Should().HaveValue(FieldFunction.LengthOf);
            ((clause as AndClause)!.LHS as ConstantClause)!.LHS.Field.Should().Be(field_.Object);
            ((clause as AndClause)!.LHS as ConstantClause)!.Operator.Should().Be(ComparisonOperator.GTE);
            ((clause as AndClause)!.LHS as ConstantClause)!.RHS.Should().Be(DBValue.Create(lower));
            (clause as AndClause)!.RHS.Should().BeOfType<ConstantClause>();
            ((clause as AndClause)!.RHS as ConstantClause)!.LHS.Function.Should().HaveValue(FieldFunction.LengthOf);
            ((clause as AndClause)!.RHS as ConstantClause)!.LHS.Field.Should().Be(field_.Object);
            ((clause as AndClause)!.RHS as ConstantClause)!.Operator.Should().Be(ComparisonOperator.LTE);
            ((clause as AndClause)!.RHS as ConstantClause)!.RHS.Should().Be(DBValue.Create(upper));
        }

        [TestMethod] public void LengthIsBetween_UniqueId() {
            // Arrange
            var attr = new Check.LengthIsBetweenAttribute(22, 25);

            // Act
            var isUnique = ids_.Add(attr.TypeId);

            // Assert
            isUnique.Should().BeTrue();
        }

        [TestMethod] public void LengthIsBetween_NegativeLowerBound() {
            // Arrange
            var lower = -2;
            var upper = 13;

            // Act
            Action act = () => new Check.LengthIsBetweenAttribute(lower, upper);

            // Assert
            act.Should().ThrowExactly<ArgumentException>().WithAnyMessage();
        }

        [TestMethod] public void LengthIsBetween_UpperLargerThanLower() {
            // Arrange
            var lower = 4;
            var upper = 1;

            // Act
            Action act = () => new Check.LengthIsBetweenAttribute(lower, upper);

            // Assert
            act.Should().ThrowExactly<ArgumentException>().WithAnyMessage();
        }


        static StringLengthAttributeTests() {
            field_ = new Mock<IField>();
            field_.Setup(f => f.DataType).Returns(DBType.Text);
        }

        private static readonly Mock<IField> field_;
        private static readonly Settings settings_ = Settings.Default;
        private static readonly DataConverter converter_ = DataConverter.Identity<string>();
    }
}
