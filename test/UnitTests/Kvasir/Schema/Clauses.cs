using FluentAssertions;
using Kvasir.Schema;
using Kvasir.Transcription;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace UT.Kvasir.Schema {
    [TestClass]
    public class CheckConstraintClauseTests {
        [TestMethod, TestCategory("FieldExpression")]
        public void ConstructFieldExpressionNoFunction() {
            // Arrange
            var mockField = Substitute.For<IField>();
            var dbType = DBType.Character;
            mockField.DataType.Returns(dbType);

            // Act
            var expr = new FieldExpression(mockField);

            // Assert
            expr.Field.Should().Be(mockField);
            expr.DataType.Should().Be(dbType);
            expr.Function.Should().NotHaveValue();
        }

        [TestMethod, TestCategory("FieldExpression")]
        public void ConstructFieldExpresionStringLengthFunction() {
            // Arrange
            var mockField = Substitute.For<IField>();
            var dbType = DBType.Text;
            var func = FieldFunction.LengthOf;
            mockField.DataType.Returns(dbType);

            // Act
            var expr = new FieldExpression(func, mockField);

            // Assert
            expr.Field.Should().Be(mockField);
            expr.DataType.Should().Be(DBType.Int32);
            expr.Function.Should().HaveValue(func);
        }

        [TestMethod, TestCategory("SimpleClauses")]
        public void ConstantClauseNotNegated() {
            // Arrange
            var mockField = Substitute.For<IField>();
            var expr = new FieldExpression(mockField);
            var op = ComparisonOperator.LT;
            var value = DBValue.Create(100);
            mockField.DataType.Returns(DBType.Int32);

            // Act
            var clause = new ConstantClause(expr, op, value);
            var fields = clause.GetDependentFields();

            // Assert
            clause.LHS.Should().Be(expr);
            clause.Operator.Should().Be(op);
            clause.RHS.Should().Be(value);
            fields.Should().BeEquivalentTo(new IField[] { mockField });
        }

        [TestMethod, TestCategory("SimpleClauses")]
        public void ConstantClauseNegated_EQ() {
            // Arrange
            var mockField = Substitute.For<IField>();
            var expr = new FieldExpression(mockField);
            var op = ComparisonOperator.EQ;
            var value = DBValue.Create(100);
            mockField.DataType.Returns(DBType.Int32);
            var clause = new ConstantClause(expr, op, value);
            var mockBuilder = Substitute.For<IConstraintDeclBuilder<SqlSnippet>>();

            // Act
            var negation = clause.Negation();
            negation.AddDeclarationTo(mockBuilder);

            // Assert
            var expected = new ConstantClause(expr, ComparisonOperator.NE, value);
            mockBuilder.Received().AddClause(expected.Matcher());
            mockBuilder.ReceivedCalls().Should().HaveCount(1);
        }

        [TestMethod, TestCategory("SimpleClauses")]
        public void ConstantClauseNegated_NE() {
            // Arrange
            var mockField = Substitute.For<IField>();
            var expr = new FieldExpression(mockField);
            var op = ComparisonOperator.NE;
            var value = DBValue.Create(100);
            mockField.DataType.Returns(DBType.Int32);
            var clause = new ConstantClause(expr, op, value);
            var mockBuilder = Substitute.For<IConstraintDeclBuilder<SqlSnippet>>();

            // Act
            var negation = clause.Negation();
            negation.AddDeclarationTo(mockBuilder);

            // Assert
            var expected = new ConstantClause(expr, ComparisonOperator.EQ, value);
            mockBuilder.Received().AddClause(expected.Matcher());
            mockBuilder.ReceivedCalls().Should().HaveCount(1);
        }

        [TestMethod, TestCategory("SimpleClauses")]
        public void ConstantClauseNegated_LT() {
            // Arrange
            var mockField = Substitute.For<IField>();
            var expr = new FieldExpression(mockField);
            var op = ComparisonOperator.LT;
            var value = DBValue.Create(100);
            mockField.DataType.Returns(DBType.Int32);
            var clause = new ConstantClause(expr, op, value);
            var mockBuilder = Substitute.For<IConstraintDeclBuilder<SqlSnippet>>();

            // Act
            var negation = clause.Negation();
            negation.AddDeclarationTo(mockBuilder);

            // Assert
            var expected = new ConstantClause(expr, ComparisonOperator.GTE, value);
            mockBuilder.Received().AddClause(expected.Matcher());
            mockBuilder.ReceivedCalls().Should().HaveCount(1);
        }

        [TestMethod, TestCategory("SimpleClauses")]
        public void ConstantClauseNegated_LTE() {
            // Arrange
            var mockField = Substitute.For<IField>();
            var expr = new FieldExpression(mockField);
            var op = ComparisonOperator.LTE;
            var value = DBValue.Create(100);
            mockField.DataType.Returns(DBType.Int32);
            var clause = new ConstantClause(expr, op, value);
            var mockBuilder = Substitute.For<IConstraintDeclBuilder<SqlSnippet>>();

            // Act
            var negation = clause.Negation();
            negation.AddDeclarationTo(mockBuilder);

            // Assert
            var expected = new ConstantClause(expr, ComparisonOperator.GT, value);
            mockBuilder.Received().AddClause(expected.Matcher());
            mockBuilder.ReceivedCalls().Should().HaveCount(1);
        }

        [TestMethod, TestCategory("SimpleClauses")]
        public void ConstantClauseNegated_GT() {
            // Arrange
            var mockField = Substitute.For<IField>();
            var expr = new FieldExpression(mockField);
            var op = ComparisonOperator.GT;
            var value = DBValue.Create(100);
            mockField.DataType.Returns(DBType.Int32);
            var clause = new ConstantClause(expr, op, value);
            var mockBuilder = Substitute.For<IConstraintDeclBuilder<SqlSnippet>>();

            // Act
            var negation = clause.Negation();
            negation.AddDeclarationTo(mockBuilder);

            // Assert
            var expected = new ConstantClause(expr, ComparisonOperator.LTE, value);
            mockBuilder.Received().AddClause(expected.Matcher());
            mockBuilder.ReceivedCalls().Should().HaveCount(1);
        }

        [TestMethod, TestCategory("SimpleClauses")]
        public void ConstantClauseNegated_GTE() {
            // Arrange
            var mockField = Substitute.For<IField>();
            var expr = new FieldExpression(mockField);
            var op = ComparisonOperator.GTE;
            var value = DBValue.Create(100);
            mockField.DataType.Returns(DBType.Int32);
            var clause = new ConstantClause(expr, op, value);
            var mockBuilder = Substitute.For<IConstraintDeclBuilder<SqlSnippet>>();

            // Act
            var negation = clause.Negation();
            negation.AddDeclarationTo(mockBuilder);

            // Assert
            var expected = new ConstantClause(expr, ComparisonOperator.LT, value);
            mockBuilder.Received().AddClause(expected.Matcher());
            mockBuilder.ReceivedCalls().Should().HaveCount(1);
        }

        [TestMethod, TestCategory("SimpleClauses")]
        public void CrossFieldClauseNotNegated() {
            // Arrange
            var mockField = Substitute.For<IField>();
            var expr = new FieldExpression(mockField);
            var op = ComparisonOperator.LT;

            // Act
            var clause = new CrossFieldClause(expr, op, expr);
            var fields = clause.GetDependentFields();

            // Assert
            clause.LHS.Should().Be(expr);
            clause.Operator.Should().Be(op);
            clause.RHS.Should().Be(expr);
            fields.Should().BeEquivalentTo(new IField[] { mockField, mockField });
        }

        [TestMethod, TestCategory("SimpleClauses")]
        public void CrossFieldClauseNegated_EQ() {
            // Arrange
            var mockField = Substitute.For<IField>();
            var expr = new FieldExpression(mockField);
            var op = ComparisonOperator.EQ;
            mockField.DataType.Returns(DBType.Int32);
            var clause = new CrossFieldClause(expr, op, expr);
            var mockBuilder = Substitute.For<IConstraintDeclBuilder<SqlSnippet>>();

            // Act
            var negation = clause.Negation();
            negation.AddDeclarationTo(mockBuilder);

            // Assert
            var expected = new CrossFieldClause(expr, ComparisonOperator.NE, expr);
            mockBuilder.Received().AddClause(expected.Matcher());
            mockBuilder.ReceivedCalls().Should().HaveCount(1);
        }

        [TestMethod, TestCategory("SimpleClauses")]
        public void CrossFieldClauseNegated_NE() {
            // Arrange
            var mockField = Substitute.For<IField>();
            var expr = new FieldExpression(mockField);
            var op = ComparisonOperator.NE;
            mockField.DataType.Returns(DBType.Int32);
            var clause = new CrossFieldClause(expr, op, expr);
            var mockBuilder = Substitute.For<IConstraintDeclBuilder<SqlSnippet>>();

            // Act
            var negation = clause.Negation();
            negation.AddDeclarationTo(mockBuilder);

            // Assert
            var expected = new CrossFieldClause(expr, ComparisonOperator.EQ, expr);
            mockBuilder.Received().AddClause(expected.Matcher());
            mockBuilder.ReceivedCalls().Should().HaveCount(1);
        }

        [TestMethod, TestCategory("SimpleClauses")]
        public void CrossFieldClauseNegated_LT() {
            // Arrange
            var mockField = Substitute.For<IField>();
            var expr = new FieldExpression(mockField);
            var op = ComparisonOperator.LT;
            mockField.DataType.Returns(DBType.Int32);
            var clause = new CrossFieldClause(expr, op, expr);
            var mockBuilder = Substitute.For<IConstraintDeclBuilder<SqlSnippet>>();

            // Act
            var negation = clause.Negation();
            negation.AddDeclarationTo(mockBuilder);

            // Assert
            var expected = new CrossFieldClause(expr, ComparisonOperator.GTE, expr);
            mockBuilder.Received().AddClause(expected.Matcher());
            mockBuilder.ReceivedCalls().Should().HaveCount(1);
        }

        [TestMethod, TestCategory("SimpleClauses")]
        public void CrossFieldClauseNegated_LTE() {
            // Arrange
            var mockField = Substitute.For<IField>();
            var expr = new FieldExpression(mockField);
            var op = ComparisonOperator.LTE;
            mockField.DataType.Returns(DBType.Int32);
            var clause = new CrossFieldClause(expr, op, expr);
            var mockBuilder = Substitute.For<IConstraintDeclBuilder<SqlSnippet>>();

            // Act
            var negation = clause.Negation();
            negation.AddDeclarationTo(mockBuilder);

            // Assert
            var expected = new CrossFieldClause(expr, ComparisonOperator.GT, expr);
            mockBuilder.Received().AddClause(expected.Matcher());
            mockBuilder.ReceivedCalls().Should().HaveCount(1);
        }

        [TestMethod, TestCategory("SimpleClauses")]
        public void CrossFieldClauseNegated_GT() {
            // Arrange
            var mockField = Substitute.For<IField>();
            var expr = new FieldExpression(mockField);
            var op = ComparisonOperator.GT;
            mockField.DataType.Returns(DBType.Int32);
            var clause = new CrossFieldClause(expr, op, expr);
            var mockBuilder = Substitute.For<IConstraintDeclBuilder<SqlSnippet>>();

            // Act
            var negation = clause.Negation();
            negation.AddDeclarationTo(mockBuilder);

            // Assert
            var expected = new CrossFieldClause(expr, ComparisonOperator.LTE, expr);
            mockBuilder.Received().AddClause(expected.Matcher());
            mockBuilder.ReceivedCalls().Should().HaveCount(1);
        }

        [TestMethod, TestCategory("SimpleClauses")]
        public void CrossFieldClauseNegated_GTE() {
            // Arrange
            var mockField = Substitute.For<IField>();
            var expr = new FieldExpression(mockField);
            var op = ComparisonOperator.GTE;
            mockField.DataType.Returns(DBType.Int32);
            var clause = new CrossFieldClause(expr, op, expr);
            var mockBuilder = Substitute.For<IConstraintDeclBuilder<SqlSnippet>>();

            // Act
            var negation = clause.Negation();
            negation.AddDeclarationTo(mockBuilder);

            // Assert
            var expected = new CrossFieldClause(expr, ComparisonOperator.LT, expr);
            mockBuilder.Received().AddClause(expected.Matcher());
            mockBuilder.ReceivedCalls().Should().HaveCount(1);
        }

        [TestMethod, TestCategory("SimpleClauses")]
        public void InclusionClauseNotNegated() {
            // Arrange
            var mockField = Substitute.For<IField>();
            var expr = new FieldExpression(mockField);
            var op = InclusionOperator.In;
            var values = new DBValue[] { DBValue.Create(100), DBValue.Create(200), DBValue.Create(300) };
            mockField.DataType.Returns(DBType.Int32);

            // Act
            var clause = new InclusionClause(expr, op, values);
            var fields = clause.GetDependentFields();

            // Assert
            clause.LHS.Should().Be(expr);
            clause.Operator.Should().Be(op);
            clause.RHS.Should().BeEquivalentTo(values);
            fields.Should().BeEquivalentTo(new IField[] { mockField });
        }

        [TestMethod, TestCategory("SimpleClauses")]
        public void InclusionClauseNegated_In() {
            // Arrange
            var mockField = Substitute.For<IField>();
            var expr = new FieldExpression(mockField);
            var op = InclusionOperator.In;
            var values = new DBValue[] { DBValue.Create(100), DBValue.Create(200), DBValue.Create(300) };
            mockField.DataType.Returns(DBType.Int32);
            var clause = new InclusionClause(expr, op, values);
            var mockBuilder = Substitute.For<IConstraintDeclBuilder<SqlSnippet>>();

            // Act
            var negation = clause.Negation();
            negation.AddDeclarationTo(mockBuilder);

            // Assert
            var expected = new InclusionClause(expr, InclusionOperator.NotIn, values);
            mockBuilder.Received().AddClause(expected.Matcher());
            mockBuilder.ReceivedCalls().Should().HaveCount(1);
        }

        [TestMethod, TestCategory("SimpleClauses")]
        public void InclusionClauseNegated_NotIn() {
            // Arrange
            var mockField = Substitute.For<IField>();
            var expr = new FieldExpression(mockField);
            var op = InclusionOperator.NotIn;
            var values = new DBValue[] { DBValue.Create(100), DBValue.Create(200), DBValue.Create(300) };
            mockField.DataType.Returns(DBType.Int32);
            var clause = new InclusionClause(expr, op, values);
            var mockBuilder = Substitute.For<IConstraintDeclBuilder<SqlSnippet>>();

            // Act
            var negation = clause.Negation();
            negation.AddDeclarationTo(mockBuilder);

            // Assert
            var expected = new InclusionClause(expr, InclusionOperator.In, values);
            mockBuilder.Received().AddClause(expected.Matcher());
            mockBuilder.ReceivedCalls().Should().HaveCount(1);
        }

        [TestMethod, TestCategory("SimpleClauses")]
        public void NullityClauseNotNegated() {
            // Arrange
            var mockField = Substitute.For<IField>();
            var op = NullityOperator.IsNotNull;

            // Act
            var clause = new NullityClause(mockField, op);
            var fields = clause.GetDependentFields();

            // Assert
            clause.LHS.Field.Should().Be(mockField);
            clause.LHS.Function.Should().NotHaveValue();
            clause.Operator.Should().Be(op);
            fields.Should().BeEquivalentTo(new IField[] { mockField });
        }

        [TestMethod, TestCategory("SimpleClauses")]
        public void NullityClauseNegated_IsNull() {
            // Arrange
            var mockField = Substitute.For<IField>();
            var op = NullityOperator.IsNull;
            var clause = new NullityClause(mockField, op);
            var mockBuilder = Substitute.For<IConstraintDeclBuilder<SqlSnippet>>();

            // Act
            var negation = clause.Negation();
            negation.AddDeclarationTo(mockBuilder);

            // Assert
            var expected = new NullityClause(mockField, NullityOperator.IsNotNull);
            mockBuilder.Received().AddClause(expected.Matcher());
            mockBuilder.ReceivedCalls().Should().HaveCount(1);
        }

        [TestMethod, TestCategory("SimpleClauses")]
        public void NullityClauseNegated_IsNotNull() {
            // Arrange
            var mockField = Substitute.For<IField>();
            var op = NullityOperator.IsNotNull;
            var clause = new NullityClause(mockField, op);
            var mockBuilder = Substitute.For<IConstraintDeclBuilder<SqlSnippet>>();

            // Act
            var negation = clause.Negation();
            negation.AddDeclarationTo(mockBuilder);

            // Assert
            var expected = new NullityClause(mockField, NullityOperator.IsNull);
            mockBuilder.Received().AddClause(expected.Matcher());
            mockBuilder.ReceivedCalls().Should().HaveCount(1);
        }

        [TestMethod, TestCategory("CompoundClauses")]
        public void AndClauseNotNegated() {
            // Arrange
            var mockField = Substitute.For<IField>();
            var expr = new FieldExpression(mockField);
            var lhs = new ConstantClause(expr, ComparisonOperator.GTE, DBValue.Create(100));
            var rhs = new ConstantClause(expr, ComparisonOperator.LTE, DBValue.Create(200));
            var clause = lhs.And(rhs);
            var mockBuilder = Substitute.For<IConstraintDeclBuilder<SqlSnippet>>();

            // Act
            var fields = clause.GetDependentFields();
            clause.AddDeclarationTo(mockBuilder);

            // Assert
            fields.Should().BeEquivalentTo(new IField[] { mockField, mockField });
            Received.InOrder(() => {
                mockBuilder.StartClause();
                mockBuilder.AddClause(lhs.Matcher());
                mockBuilder.And();
                mockBuilder.AddClause(rhs.Matcher());
                mockBuilder.EndClause();
            });
            mockBuilder.ReceivedCalls().Should().HaveCount(5);
        }

        [TestMethod, TestCategory("CompoundClauses")]
        public void AndClauseNegated() {
            // Arrange
            var mockField = Substitute.For<IField>();
            var expr = new FieldExpression(mockField);
            var lhs = new ConstantClause(expr, ComparisonOperator.GTE, DBValue.Create(100));
            var lhsNeg = new ConstantClause(expr, ComparisonOperator.LT, DBValue.Create(100));
            var rhs = new ConstantClause(expr, ComparisonOperator.LTE, DBValue.Create(200));
            var rhsNeg = new ConstantClause(expr, ComparisonOperator.GT, DBValue.Create(200));
            var clause = lhs.And(rhs).Negation();
            var mockBuilder = Substitute.For<IConstraintDeclBuilder<SqlSnippet>>();

            // Act
            var fields = clause.GetDependentFields();
            clause.AddDeclarationTo(mockBuilder);

            // Assert
            fields.Should().BeEquivalentTo(new IField[] { mockField, mockField });
            Received.InOrder(() => {
                mockBuilder.StartClause();
                mockBuilder.AddClause(lhsNeg.Matcher());
                mockBuilder.Or();
                mockBuilder.AddClause(rhsNeg.Matcher());
                mockBuilder.EndClause();
            });
            mockBuilder.ReceivedCalls().Should().HaveCount(5);
        }

        [TestMethod, TestCategory("CompoundClauses")]
        public void OrClauseNotNegated() {
            // Arrange
            var mockField = Substitute.For<IField>();
            var expr = new FieldExpression(mockField);
            var lhs = new ConstantClause(expr, ComparisonOperator.GTE, DBValue.Create(100));
            var rhs = new ConstantClause(expr, ComparisonOperator.LTE, DBValue.Create(200));
            var clause = lhs.Or(rhs);
            var mockBuilder = Substitute.For<IConstraintDeclBuilder<SqlSnippet>>();

            // Act
            var fields = clause.GetDependentFields();
            clause.AddDeclarationTo(mockBuilder);

            // Assert
            fields.Should().BeEquivalentTo(new IField[] { mockField, mockField });
            Received.InOrder(() => {
                mockBuilder.StartClause();
                mockBuilder.AddClause(lhs.Matcher());
                mockBuilder.Or();
                mockBuilder.AddClause(rhs.Matcher());
                mockBuilder.EndClause();
            });
            mockBuilder.ReceivedCalls().Should().HaveCount(5);
        }

        [TestMethod, TestCategory("CompoundClauses")]
        public void OrClauseNegated() {
            // Arrange
            var mockField = Substitute.For<IField>();
            var expr = new FieldExpression(mockField);
            var lhs = new ConstantClause(expr, ComparisonOperator.GTE, DBValue.Create(100));
            var lhsNeg = new ConstantClause(expr, ComparisonOperator.LT, DBValue.Create(100));
            var rhs = new ConstantClause(expr, ComparisonOperator.LTE, DBValue.Create(200));
            var rhsNeg = new ConstantClause(expr, ComparisonOperator.GT, DBValue.Create(200));
            var clause = lhs.Or(rhs).Negation();
            var mockBuilder = Substitute.For<IConstraintDeclBuilder<SqlSnippet>>();

            // Act
            var fields = clause.GetDependentFields();
            clause.AddDeclarationTo(mockBuilder);

            // Assert
            fields.Should().BeEquivalentTo(new IField[] { mockField, mockField });
            Received.InOrder(() => {
                mockBuilder.StartClause();
                mockBuilder.AddClause(lhsNeg.Matcher());
                mockBuilder.And();
                mockBuilder.AddClause(rhsNeg.Matcher());
                mockBuilder.EndClause();
            });
            mockBuilder.ReceivedCalls().Should().HaveCount(5);
        }

        [TestMethod, TestCategory("CompoundClauses")]
        public void XorClauseNotNegated() {
            // Arrange
            var mockField = Substitute.For<IField>();
            var expr = new FieldExpression(mockField);
            var lhs = new ConstantClause(expr, ComparisonOperator.GTE, DBValue.Create(100));
            var lhsNeg = new ConstantClause(expr, ComparisonOperator.LT, DBValue.Create(100));
            var rhs = new ConstantClause(expr, ComparisonOperator.LTE, DBValue.Create(200));
            var rhsNeg = new ConstantClause(expr, ComparisonOperator.GT, DBValue.Create(200));
            var clause = lhs.Xor(rhs);
            var mockBuilder = Substitute.For<IConstraintDeclBuilder<SqlSnippet>>();

            // Act
            var fields = clause.GetDependentFields();
            clause.AddDeclarationTo(mockBuilder);

            // Assert
            fields.Should().BeEquivalentTo(new IField[] { mockField, mockField });
            Received.InOrder(() => {
                mockBuilder.StartClause();
                mockBuilder.StartClause();
                mockBuilder.AddClause(lhs.Matcher());
                mockBuilder.And();
                mockBuilder.AddClause(rhsNeg.Matcher());
                mockBuilder.EndClause();
                mockBuilder.Or();
                mockBuilder.StartClause();
                mockBuilder.AddClause(rhs.Matcher());
                mockBuilder.And();
                mockBuilder.AddClause(lhsNeg.Matcher());
                mockBuilder.EndClause();
                mockBuilder.EndClause();
            });
            mockBuilder.ReceivedCalls().Should().HaveCount(13);
        }

        [TestMethod, TestCategory("CompoundClauses")]
        public void XorClauseNegated() {
            // Arrange
            var mockField = Substitute.For<IField>();
            var expr = new FieldExpression(mockField);
            var lhs = new ConstantClause(expr, ComparisonOperator.GTE, DBValue.Create(100));
            var lhsNeg = new ConstantClause(expr, ComparisonOperator.LT, DBValue.Create(100));
            var rhs = new ConstantClause(expr, ComparisonOperator.LTE, DBValue.Create(200));
            var rhsNeg = new ConstantClause(expr, ComparisonOperator.GT, DBValue.Create(200));
            var clause = lhs.Xor(rhs).Negation();
            var mockBuilder = Substitute.For<IConstraintDeclBuilder<SqlSnippet>>();

            // Act
            var fields = clause.GetDependentFields();
            clause.AddDeclarationTo(mockBuilder);

            // Assert
            fields.Should().BeEquivalentTo(new IField[] { mockField, mockField });
            Received.InOrder(() => {
                mockBuilder.StartClause();
                mockBuilder.StartClause();
                mockBuilder.AddClause(lhs.Matcher());
                mockBuilder.And();
                mockBuilder.AddClause(rhs.Matcher());
                mockBuilder.EndClause();
                mockBuilder.Or();
                mockBuilder.StartClause();
                mockBuilder.AddClause(lhsNeg.Matcher());
                mockBuilder.And();
                mockBuilder.AddClause(rhsNeg.Matcher());
                mockBuilder.EndClause();
                mockBuilder.EndClause();
            });
            mockBuilder.ReceivedCalls().Should().HaveCount(13);
        }

        [TestMethod, TestCategory("CompoundClauses")]
        public void IfThenClauseNotNegated() {
            // Arrange
            var mockField = Substitute.For<IField>();
            var expr = new FieldExpression(mockField);
            var pred = new ConstantClause(expr, ComparisonOperator.GTE, DBValue.Create(100));
            var predNeg = new ConstantClause(expr, ComparisonOperator.LT, DBValue.Create(100));
            var subseq = new ConstantClause(expr, ComparisonOperator.LTE, DBValue.Create(200));
            var clause = Clause.IfThen(pred, subseq);
            var mockBuilder = Substitute.For<IConstraintDeclBuilder<SqlSnippet>>();

            // Act
            var fields = clause.GetDependentFields();
            clause.AddDeclarationTo(mockBuilder);

            // Assert
            fields.Should().BeEquivalentTo(new IField[] { mockField, mockField });
            Received.InOrder(() => {
                mockBuilder.StartClause();
                mockBuilder.AddClause(subseq.Matcher());
                mockBuilder.Or();
                mockBuilder.AddClause(predNeg.Matcher());
                mockBuilder.EndClause();
            });
            mockBuilder.ReceivedCalls().Should().HaveCount(5);
        }

        [TestMethod, TestCategory("CompoundClauses")]
        public void IfThenClauseNegated() {
            // Arrange
            var mockField = Substitute.For<IField>();
            var expr = new FieldExpression(mockField);
            var pred = new ConstantClause(expr, ComparisonOperator.GTE, DBValue.Create(100));
            var subseq = new ConstantClause(expr, ComparisonOperator.LTE, DBValue.Create(200));
            var subseqNeg = new ConstantClause(expr, ComparisonOperator.GT, DBValue.Create(200));
            var clause = Clause.IfThen(pred, subseq).Negation();
            var mockBuilder = Substitute.For<IConstraintDeclBuilder<SqlSnippet>>();

            // Act
            var fields = clause.GetDependentFields();
            clause.AddDeclarationTo(mockBuilder);

            // Assert
            fields.Should().BeEquivalentTo(new IField[] { mockField, mockField });
            Received.InOrder(() => {
                mockBuilder.StartClause();
                mockBuilder.AddClause(subseqNeg.Matcher());
                mockBuilder.And();
                mockBuilder.AddClause(pred.Matcher());
                mockBuilder.EndClause();
            });
            mockBuilder.ReceivedCalls().Should().HaveCount(5);
        }

        [TestMethod, TestCategory("CompoundClauses")]
        public void IffClauseNotNegated() {
            // Arrange
            var mockField = Substitute.For<IField>();
            var expr = new FieldExpression(mockField);
            var lhs = new ConstantClause(expr, ComparisonOperator.GTE, DBValue.Create(100));
            var lhsNeg = new ConstantClause(expr, ComparisonOperator.LT, DBValue.Create(100));
            var rhs = new ConstantClause(expr, ComparisonOperator.LTE, DBValue.Create(200));
            var rhsNeg = new ConstantClause(expr, ComparisonOperator.GT, DBValue.Create(200));
            var clause = Clause.Iff(lhs, rhs);
            var mockBuilder = Substitute.For<IConstraintDeclBuilder<SqlSnippet>>();

            // Act
            var fields = clause.GetDependentFields();
            clause.AddDeclarationTo(mockBuilder);

            // Assert
            fields.Should().BeEquivalentTo(new IField[] { mockField, mockField });
            Received.InOrder(() => {
                mockBuilder.StartClause();
                mockBuilder.StartClause();
                mockBuilder.AddClause(lhs.Matcher());
                mockBuilder.And();
                mockBuilder.AddClause(rhs.Matcher());
                mockBuilder.EndClause();
                mockBuilder.Or();
                mockBuilder.StartClause();
                mockBuilder.AddClause(lhsNeg.Matcher());
                mockBuilder.And();
                mockBuilder.AddClause(rhsNeg.Matcher());
                mockBuilder.EndClause();
                mockBuilder.EndClause();
            });
            mockBuilder.ReceivedCalls().Should().HaveCount(13);
        }

        [TestMethod, TestCategory("CompoundClauses")]
        public void IffClauseNegated() {
            // Arrange
            var mockField = Substitute.For<IField>();
            var expr = new FieldExpression(mockField);
            var lhs = new ConstantClause(expr, ComparisonOperator.GTE, DBValue.Create(100));
            var lhsNeg = new ConstantClause(expr, ComparisonOperator.LT, DBValue.Create(100));
            var rhs = new ConstantClause(expr, ComparisonOperator.LTE, DBValue.Create(200));
            var rhsNeg = new ConstantClause(expr, ComparisonOperator.GT, DBValue.Create(200));
            var clause = Clause.Iff(lhs, rhs).Negation();
            var mockBuilder = Substitute.For<IConstraintDeclBuilder<SqlSnippet>>();

            // Act
            var fields = clause.GetDependentFields();
            clause.AddDeclarationTo(mockBuilder);

            // Assert
            fields.Should().BeEquivalentTo(new IField[] { mockField, mockField });
            Received.InOrder(() => {
                mockBuilder.StartClause();
                mockBuilder.StartClause();
                mockBuilder.AddClause(lhs.Matcher());
                mockBuilder.And();
                mockBuilder.AddClause(rhsNeg.Matcher());
                mockBuilder.EndClause();
                mockBuilder.Or();
                mockBuilder.StartClause();
                mockBuilder.AddClause(rhs.Matcher());
                mockBuilder.And();
                mockBuilder.AddClause(lhsNeg.Matcher());
                mockBuilder.EndClause();
                mockBuilder.EndClause();
            });
            mockBuilder.ReceivedCalls().Should().HaveCount(13);
        }
    }
}
