using Atropos.Moq;
using FluentAssertions;
using Kvasir.Schema;
using Kvasir.Transcription;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace UT.Kvasir.Schema {
    [TestClass]
    public class CheckConstraintClauseTests {
        [TestMethod, TestCategory("FieldExpression")]
        public void ConstructFieldExpressionNoFunction() {
            // Arrange
            var mockField = new Mock<IField>();
            var dbType = DBType.Character;
            mockField.Setup(field => field.DataType).Returns(dbType);

            // Act
            var expr = new FieldExpression(mockField.Object);

            // Assert
            expr.Field.Should().Be(mockField.Object);
            expr.DataType.Should().Be(dbType);
            expr.Function.Should().NotHaveValue();
        }

        [TestMethod, TestCategory("FieldExpression")]
        public void ConstructFieldExpresionStringLengthFunction() {
            // Arrange
            var mockField = new Mock<IField>();
            var dbType = DBType.Text;
            var func = FieldFunction.LengthOf;
            mockField.Setup(field => field.DataType).Returns(dbType);

            // Act
            var expr = new FieldExpression(func, mockField.Object);

            // Assert
            expr.Field.Should().Be(mockField.Object);
            expr.DataType.Should().Be(DBType.Int32);
            expr.Function.Should().HaveValue(func);
        }

        [TestMethod, TestCategory("SimpleClauses")]
        public void ConstantClauseNotNegated() {
            // Arrange
            var mockField = new Mock<IField>();
            var expr = new FieldExpression(mockField.Object);
            var op = ComparisonOperator.LT;
            var value = DBValue.Create(100);
            mockField.Setup(field => field.DataType).Returns(DBType.Int32);

            // Act
            var clause = new ConstantClause(expr, op, value);
            var fields = clause.GetDependentFields();

            // Assert
            clause.LHS.Should().Be(expr);
            clause.Operator.Should().Be(op);
            clause.RHS.Should().Be(value);
            fields.Should().BeEquivalentTo(new IField[] { mockField.Object });
        }

        [TestMethod, TestCategory("SimpleClauses")]
        public void ConstantClauseNegated_EQ() {
            // Arrange
            var mockField = new Mock<IField>();
            var expr = new FieldExpression(mockField.Object);
            var op = ComparisonOperator.EQ;
            var value = DBValue.Create(100);
            mockField.Setup(field => field.DataType).Returns(DBType.Int32);
            var clause = new ConstantClause(expr, op, value);
            var mockBuilder = new Mock<IConstraintDeclBuilder<SqlSnippet>>();

            // Act
            var negation = clause.Negation();
            negation.AddDeclarationTo(mockBuilder.Object);

            // Assert
            var expected = new ConstantClause(expr, ComparisonOperator.NE, value);
            mockBuilder.Verify(builder => builder.AddClause(expected.Matcher()));
            mockBuilder.VerifyNoOtherCalls();
        }

        [TestMethod, TestCategory("SimpleClauses")]
        public void ConstantClauseNegated_NE() {
            // Arrange
            var mockField = new Mock<IField>();
            var expr = new FieldExpression(mockField.Object);
            var op = ComparisonOperator.NE;
            var value = DBValue.Create(100);
            mockField.Setup(field => field.DataType).Returns(DBType.Int32);
            var clause = new ConstantClause(expr, op, value);
            var mockBuilder = new Mock<IConstraintDeclBuilder<SqlSnippet>>();

            // Act
            var negation = clause.Negation();
            negation.AddDeclarationTo(mockBuilder.Object);

            // Assert
            var expected = new ConstantClause(expr, ComparisonOperator.EQ, value);
            mockBuilder.Verify(builder => builder.AddClause(expected.Matcher()));
            mockBuilder.VerifyNoOtherCalls();
        }

        [TestMethod, TestCategory("SimpleClauses")]
        public void ConstantClauseNegated_LT() {
            // Arrange
            var mockField = new Mock<IField>();
            var expr = new FieldExpression(mockField.Object);
            var op = ComparisonOperator.LT;
            var value = DBValue.Create(100);
            mockField.Setup(field => field.DataType).Returns(DBType.Int32);
            var clause = new ConstantClause(expr, op, value);
            var mockBuilder = new Mock<IConstraintDeclBuilder<SqlSnippet>>();

            // Act
            var negation = clause.Negation();
            negation.AddDeclarationTo(mockBuilder.Object);

            // Assert
            var expected = new ConstantClause(expr, ComparisonOperator.GTE, value);
            mockBuilder.Verify(builder => builder.AddClause(expected.Matcher()));
            mockBuilder.VerifyNoOtherCalls();
        }

        [TestMethod, TestCategory("SimpleClauses")]
        public void ConstantClauseNegated_LTE() {
            // Arrange
            var mockField = new Mock<IField>();
            var expr = new FieldExpression(mockField.Object);
            var op = ComparisonOperator.LTE;
            var value = DBValue.Create(100);
            mockField.Setup(field => field.DataType).Returns(DBType.Int32);
            var clause = new ConstantClause(expr, op, value);
            var mockBuilder = new Mock<IConstraintDeclBuilder<SqlSnippet>>();

            // Act
            var negation = clause.Negation();
            negation.AddDeclarationTo(mockBuilder.Object);

            // Assert
            var expected = new ConstantClause(expr, ComparisonOperator.GT, value);
            mockBuilder.Verify(builder => builder.AddClause(expected.Matcher()));
            mockBuilder.VerifyNoOtherCalls();
        }

        [TestMethod, TestCategory("SimpleClauses")]
        public void ConstantClauseNegated_GT() {
            // Arrange
            var mockField = new Mock<IField>();
            var expr = new FieldExpression(mockField.Object);
            var op = ComparisonOperator.GT;
            var value = DBValue.Create(100);
            mockField.Setup(field => field.DataType).Returns(DBType.Int32);
            var clause = new ConstantClause(expr, op, value);
            var mockBuilder = new Mock<IConstraintDeclBuilder<SqlSnippet>>();

            // Act
            var negation = clause.Negation();
            negation.AddDeclarationTo(mockBuilder.Object);

            // Assert
            var expected = new ConstantClause(expr, ComparisonOperator.LTE, value);
            mockBuilder.Verify(builder => builder.AddClause(expected.Matcher()));
            mockBuilder.VerifyNoOtherCalls();
        }

        [TestMethod, TestCategory("SimpleClauses")]
        public void ConstantClauseNegated_GTE() {
            // Arrange
            var mockField = new Mock<IField>();
            var expr = new FieldExpression(mockField.Object);
            var op = ComparisonOperator.GTE;
            var value = DBValue.Create(100);
            mockField.Setup(field => field.DataType).Returns(DBType.Int32);
            var clause = new ConstantClause(expr, op, value);
            var mockBuilder = new Mock<IConstraintDeclBuilder<SqlSnippet>>();

            // Act
            var negation = clause.Negation();
            negation.AddDeclarationTo(mockBuilder.Object);

            // Assert
            var expected = new ConstantClause(expr, ComparisonOperator.LT, value);
            mockBuilder.Verify(builder => builder.AddClause(expected.Matcher()));
            mockBuilder.VerifyNoOtherCalls();
        }

        [TestMethod, TestCategory("SimpleClauses")]
        public void CrossFieldClauseNotNegated() {
            // Arrange
            var mockField = new Mock<IField>();
            var expr = new FieldExpression(mockField.Object);
            var op = ComparisonOperator.LT;

            // Act
            var clause = new CrossFieldClause(expr, op, expr);
            var fields = clause.GetDependentFields();

            // Assert
            clause.LHS.Should().Be(expr);
            clause.Operator.Should().Be(op);
            clause.RHS.Should().Be(expr);
            fields.Should().BeEquivalentTo(new IField[] { mockField.Object, mockField.Object });
        }

        [TestMethod, TestCategory("SimpleClauses")]
        public void CrossFieldClauseNegated_EQ() {
            // Arrange
            var mockField = new Mock<IField>();
            var expr = new FieldExpression(mockField.Object);
            var op = ComparisonOperator.EQ;
            mockField.Setup(field => field.DataType).Returns(DBType.Int32);
            var clause = new CrossFieldClause(expr, op, expr);
            var mockBuilder = new Mock<IConstraintDeclBuilder<SqlSnippet>>();

            // Act
            var negation = clause.Negation();
            negation.AddDeclarationTo(mockBuilder.Object);

            // Assert
            var expected = new CrossFieldClause(expr, ComparisonOperator.NE, expr);
            mockBuilder.Verify(builder => builder.AddClause(expected.Matcher()));
            mockBuilder.VerifyNoOtherCalls();
        }

        [TestMethod, TestCategory("SimpleClauses")]
        public void CrossFieldClauseNegated_NE() {
            // Arrange
            var mockField = new Mock<IField>();
            var expr = new FieldExpression(mockField.Object);
            var op = ComparisonOperator.NE;
            mockField.Setup(field => field.DataType).Returns(DBType.Int32);
            var clause = new CrossFieldClause(expr, op, expr);
            var mockBuilder = new Mock<IConstraintDeclBuilder<SqlSnippet>>();

            // Act
            var negation = clause.Negation();
            negation.AddDeclarationTo(mockBuilder.Object);

            // Assert
            var expected = new CrossFieldClause(expr, ComparisonOperator.EQ, expr);
            mockBuilder.Verify(builder => builder.AddClause(expected.Matcher()));
            mockBuilder.VerifyNoOtherCalls();
        }

        [TestMethod, TestCategory("SimpleClauses")]
        public void CrossFieldClauseNegated_LT() {
            // Arrange
            var mockField = new Mock<IField>();
            var expr = new FieldExpression(mockField.Object);
            var op = ComparisonOperator.LT;
            mockField.Setup(field => field.DataType).Returns(DBType.Int32);
            var clause = new CrossFieldClause(expr, op, expr);
            var mockBuilder = new Mock<IConstraintDeclBuilder<SqlSnippet>>();

            // Act
            var negation = clause.Negation();
            negation.AddDeclarationTo(mockBuilder.Object);

            // Assert
            var expected = new CrossFieldClause(expr, ComparisonOperator.GTE, expr);
            mockBuilder.Verify(builder => builder.AddClause(expected.Matcher()));
            mockBuilder.VerifyNoOtherCalls();
        }

        [TestMethod, TestCategory("SimpleClauses")]
        public void CrossFieldClauseNegated_LTE() {
            // Arrange
            var mockField = new Mock<IField>();
            var expr = new FieldExpression(mockField.Object);
            var op = ComparisonOperator.LTE;
            mockField.Setup(field => field.DataType).Returns(DBType.Int32);
            var clause = new CrossFieldClause(expr, op, expr);
            var mockBuilder = new Mock<IConstraintDeclBuilder<SqlSnippet>>();

            // Act
            var negation = clause.Negation();
            negation.AddDeclarationTo(mockBuilder.Object);

            // Assert
            var expected = new CrossFieldClause(expr, ComparisonOperator.GT, expr);
            mockBuilder.Verify(builder => builder.AddClause(expected.Matcher()));
            mockBuilder.VerifyNoOtherCalls();
        }

        [TestMethod, TestCategory("SimpleClauses")]
        public void CrossFieldClauseNegated_GT() {
            // Arrange
            var mockField = new Mock<IField>();
            var expr = new FieldExpression(mockField.Object);
            var op = ComparisonOperator.GT;
            mockField.Setup(field => field.DataType).Returns(DBType.Int32);
            var clause = new CrossFieldClause(expr, op, expr);
            var mockBuilder = new Mock<IConstraintDeclBuilder<SqlSnippet>>();

            // Act
            var negation = clause.Negation();
            negation.AddDeclarationTo(mockBuilder.Object);

            // Assert
            var expected = new CrossFieldClause(expr, ComparisonOperator.LTE, expr);
            mockBuilder.Verify(builder => builder.AddClause(expected.Matcher()));
            mockBuilder.VerifyNoOtherCalls();
        }

        [TestMethod, TestCategory("SimpleClauses")]
        public void CrossFieldClauseNegated_GTE() {
            // Arrange
            var mockField = new Mock<IField>();
            var expr = new FieldExpression(mockField.Object);
            var op = ComparisonOperator.GTE;
            mockField.Setup(field => field.DataType).Returns(DBType.Int32);
            var clause = new CrossFieldClause(expr, op, expr);
            var mockBuilder = new Mock<IConstraintDeclBuilder<SqlSnippet>>();

            // Act
            var negation = clause.Negation();
            negation.AddDeclarationTo(mockBuilder.Object);

            // Assert
            var expected = new CrossFieldClause(expr, ComparisonOperator.LT, expr);
            mockBuilder.Verify(builder => builder.AddClause(expected.Matcher()));
            mockBuilder.VerifyNoOtherCalls();
        }

        [TestMethod, TestCategory("SimpleClauses")]
        public void InclusionClauseNotNegated() {
            // Arrange
            var mockField = new Mock<IField>();
            var expr = new FieldExpression(mockField.Object);
            var op = InclusionOperator.In;
            var values = new DBValue[] { DBValue.Create(100), DBValue.Create(200), DBValue.Create(300) };
            mockField.Setup(field => field.DataType).Returns(DBType.Int32);

            // Act
            var clause = new InclusionClause(expr, op, values);
            var fields = clause.GetDependentFields();

            // Assert
            clause.LHS.Should().Be(expr);
            clause.Operator.Should().Be(op);
            clause.RHS.Should().BeEquivalentTo(values);
            fields.Should().BeEquivalentTo(new IField[] { mockField.Object });
        }

        [TestMethod, TestCategory("SimpleClauses")]
        public void InclusionClauseNegated_In() {
            // Arrange
            var mockField = new Mock<IField>();
            var expr = new FieldExpression(mockField.Object);
            var op = InclusionOperator.In;
            var values = new DBValue[] { DBValue.Create(100), DBValue.Create(200), DBValue.Create(300) };
            mockField.Setup(field => field.DataType).Returns(DBType.Int32);
            var clause = new InclusionClause(expr, op, values);
            var mockBuilder = new Mock<IConstraintDeclBuilder<SqlSnippet>>();

            // Act
            var negation = clause.Negation();
            negation.AddDeclarationTo(mockBuilder.Object);

            // Assert
            var expected = new InclusionClause(expr, InclusionOperator.NotIn, values);
            mockBuilder.Verify(builder => builder.AddClause(expected.Matcher()));
            mockBuilder.VerifyNoOtherCalls();
        }

        [TestMethod, TestCategory("SimpleClauses")]
        public void InclusionClauseNegated_NotIn() {
            // Arrange
            var mockField = new Mock<IField>();
            var expr = new FieldExpression(mockField.Object);
            var op = InclusionOperator.NotIn;
            var values = new DBValue[] { DBValue.Create(100), DBValue.Create(200), DBValue.Create(300) };
            mockField.Setup(field => field.DataType).Returns(DBType.Int32);
            var clause = new InclusionClause(expr, op, values);
            var mockBuilder = new Mock<IConstraintDeclBuilder<SqlSnippet>>();

            // Act
            var negation = clause.Negation();
            negation.AddDeclarationTo(mockBuilder.Object);

            // Assert
            var expected = new InclusionClause(expr, InclusionOperator.In, values);
            mockBuilder.Verify(builder => builder.AddClause(expected.Matcher()));
            mockBuilder.VerifyNoOtherCalls();
        }

        [TestMethod, TestCategory("SimpleClauses")]
        public void NullityClauseNotNegated() {
            // Arrange
            var mockField = new Mock<IField>();
            var op = NullityOperator.IsNotNull;

            // Act
            var clause = new NullityClause(mockField.Object, op);
            var fields = clause.GetDependentFields();

            // Assert
            clause.LHS.Field.Should().Be(mockField.Object);
            clause.LHS.Function.Should().NotHaveValue();
            clause.Operator.Should().Be(op);
            fields.Should().BeEquivalentTo(new IField[] { mockField.Object });
        }

        [TestMethod, TestCategory("SimpleClauses")]
        public void NullityClauseNegated_IsNull() {
            // Arrange
            var mockField = new Mock<IField>();
            var op = NullityOperator.IsNull;
            var clause = new NullityClause(mockField.Object, op);
            var mockBuilder = new Mock<IConstraintDeclBuilder<SqlSnippet>>();

            // Act
            var negation = clause.Negation();
            negation.AddDeclarationTo(mockBuilder.Object);

            // Assert
            var expected = new NullityClause(mockField.Object, NullityOperator.IsNotNull);
            mockBuilder.Verify(builder => builder.AddClause(expected.Matcher()));
            mockBuilder.VerifyNoOtherCalls();
        }

        [TestMethod, TestCategory("SimpleClauses")]
        public void NullityClauseNegated_IsNotNull() {
            // Arrange
            var mockField = new Mock<IField>();
            var op = NullityOperator.IsNotNull;
            var clause = new NullityClause(mockField.Object, op);
            var mockBuilder = new Mock<IConstraintDeclBuilder<SqlSnippet>>();

            // Act
            var negation = clause.Negation();
            negation.AddDeclarationTo(mockBuilder.Object);

            // Assert
            var expected = new NullityClause(mockField.Object, NullityOperator.IsNull);
            mockBuilder.Verify(builder => builder.AddClause(expected.Matcher()));
            mockBuilder.VerifyNoOtherCalls();
        }

        [TestMethod, TestCategory("CompoundClauses")]
        public void AndClauseNotNegated() {
            // Arrange
            var mockField = new Mock<IField>().Object;
            var expr = new FieldExpression(mockField);
            var lhs = new ConstantClause(expr, ComparisonOperator.GTE, DBValue.Create(100));
            var rhs = new ConstantClause(expr, ComparisonOperator.LTE, DBValue.Create(200));
            var clause = lhs.And(rhs);
            var mockBuilder = new Mock<IConstraintDeclBuilder<SqlSnippet>>();

            // Sequence
            var sequence = mockBuilder.MakeSequence();
            sequence.Add(builder => builder.StartClause());
            sequence.Add(builder => builder.AddClause(lhs.Matcher()));
            sequence.Add(builder => builder.And());
            sequence.Add(builder => builder.AddClause(rhs.Matcher()));
            sequence.Add(builder => builder.EndClause());

            // Act
            var fields = clause.GetDependentFields();
            clause.AddDeclarationTo(mockBuilder.Object);

            // Assert
            fields.Should().BeEquivalentTo(new IField[] { mockField, mockField });
            sequence.VerifyCompleted();
            mockBuilder.VerifyNoOtherCalls();
        }

        [TestMethod, TestCategory("CompoundClauses")]
        public void AndClauseNegated() {
            // Arrange
            var mockField = new Mock<IField>().Object;
            var expr = new FieldExpression(mockField);
            var lhs = new ConstantClause(expr, ComparisonOperator.GTE, DBValue.Create(100));
            var lhsNeg = new ConstantClause(expr, ComparisonOperator.LT, DBValue.Create(100));
            var rhs = new ConstantClause(expr, ComparisonOperator.LTE, DBValue.Create(200));
            var rhsNeg = new ConstantClause(expr, ComparisonOperator.GT, DBValue.Create(200));
            var clause = lhs.And(rhs).Negation();
            var mockBuilder = new Mock<IConstraintDeclBuilder<SqlSnippet>>();

            // Sequence
            var sequence = mockBuilder.MakeSequence();
            sequence.Add(builder => builder.StartClause());
            sequence.Add(builder => builder.AddClause(lhsNeg.Matcher()));
            sequence.Add(builder => builder.Or());
            sequence.Add(builder => builder.AddClause(rhsNeg.Matcher()));
            sequence.Add(builder => builder.EndClause());

            // Act
            var fields = clause.GetDependentFields();
            clause.AddDeclarationTo(mockBuilder.Object);

            // Assert
            fields.Should().BeEquivalentTo(new IField[] { mockField, mockField });
            sequence.VerifyCompleted();
            mockBuilder.VerifyNoOtherCalls();
        }

        [TestMethod, TestCategory("CompoundClauses")]
        public void OrClauseNotNegated() {
            // Arrange
            var mockField = new Mock<IField>().Object;
            var expr = new FieldExpression(mockField);
            var lhs = new ConstantClause(expr, ComparisonOperator.GTE, DBValue.Create(100));
            var rhs = new ConstantClause(expr, ComparisonOperator.LTE, DBValue.Create(200));
            var clause = lhs.Or(rhs);
            var mockBuilder = new Mock<IConstraintDeclBuilder<SqlSnippet>>();

            // Sequence
            var sequence = mockBuilder.MakeSequence();
            sequence.Add(builder => builder.StartClause());
            sequence.Add(builder => builder.AddClause(lhs.Matcher()));
            sequence.Add(builder => builder.Or());
            sequence.Add(builder => builder.AddClause(rhs.Matcher()));
            sequence.Add(builder => builder.EndClause());

            // Act
            var fields = clause.GetDependentFields();
            clause.AddDeclarationTo(mockBuilder.Object);

            // Assert
            fields.Should().BeEquivalentTo(new IField[] { mockField, mockField });
            sequence.VerifyCompleted();
            mockBuilder.VerifyNoOtherCalls();
        }

        [TestMethod, TestCategory("CompoundClauses")]
        public void OrClauseNegated() {
            // Arrange
            var mockField = new Mock<IField>().Object;
            var expr = new FieldExpression(mockField);
            var lhs = new ConstantClause(expr, ComparisonOperator.GTE, DBValue.Create(100));
            var lhsNeg = new ConstantClause(expr, ComparisonOperator.LT, DBValue.Create(100));
            var rhs = new ConstantClause(expr, ComparisonOperator.LTE, DBValue.Create(200));
            var rhsNeg = new ConstantClause(expr, ComparisonOperator.GT, DBValue.Create(200));
            var clause = lhs.Or(rhs).Negation();
            var mockBuilder = new Mock<IConstraintDeclBuilder<SqlSnippet>>();

            // Sequence
            var sequence = mockBuilder.MakeSequence();
            sequence.Add(builder => builder.StartClause());
            sequence.Add(builder => builder.AddClause(lhsNeg.Matcher()));
            sequence.Add(builder => builder.And());
            sequence.Add(builder => builder.AddClause(rhsNeg.Matcher()));
            sequence.Add(builder => builder.EndClause());

            // Act
            var fields = clause.GetDependentFields();
            clause.AddDeclarationTo(mockBuilder.Object);

            // Assert
            fields.Should().BeEquivalentTo(new IField[] { mockField, mockField });
            sequence.VerifyCompleted();
            mockBuilder.VerifyNoOtherCalls();
        }

        [TestMethod, TestCategory("CompoundClauses")]
        public void XorClauseNotNegated() {
            // Arrange
            var mockField = new Mock<IField>().Object;
            var expr = new FieldExpression(mockField);
            var lhs = new ConstantClause(expr, ComparisonOperator.GTE, DBValue.Create(100));
            var lhsNeg = new ConstantClause(expr, ComparisonOperator.LT, DBValue.Create(100));
            var rhs = new ConstantClause(expr, ComparisonOperator.LTE, DBValue.Create(200));
            var rhsNeg = new ConstantClause(expr, ComparisonOperator.GT, DBValue.Create(200));
            var clause = lhs.Xor(rhs);
            var mockBuilder = new Mock<IConstraintDeclBuilder<SqlSnippet>>();

            // Sequence
            var sequence = mockBuilder.MakeSequence();
            sequence.Add(builder => builder.StartClause());
            sequence.Add(builder => builder.StartClause());
            sequence.Add(builder => builder.AddClause(lhs.Matcher()));
            sequence.Add(builder => builder.And());
            sequence.Add(builder => builder.AddClause(rhsNeg.Matcher()));
            sequence.Add(builder => builder.EndClause());
            sequence.Add(builder => builder.Or());
            sequence.Add(builder => builder.StartClause());
            sequence.Add(builder => builder.AddClause(rhs.Matcher()));
            sequence.Add(builder => builder.And());
            sequence.Add(builder => builder.AddClause(lhsNeg.Matcher()));
            sequence.Add(builder => builder.EndClause());
            sequence.Add(builder => builder.EndClause());

            // Act
            var fields = clause.GetDependentFields();
            clause.AddDeclarationTo(mockBuilder.Object);

            // Assert
            fields.Should().BeEquivalentTo(new IField[] { mockField, mockField });
            sequence.VerifyCompleted();
            mockBuilder.VerifyNoOtherCalls();
        }

        [TestMethod, TestCategory("CompoundClauses")]
        public void XorClauseNegated() {
            // Arrange
            var mockField = new Mock<IField>().Object;
            var expr = new FieldExpression(mockField);
            var lhs = new ConstantClause(expr, ComparisonOperator.GTE, DBValue.Create(100));
            var lhsNeg = new ConstantClause(expr, ComparisonOperator.LT, DBValue.Create(100));
            var rhs = new ConstantClause(expr, ComparisonOperator.LTE, DBValue.Create(200));
            var rhsNeg = new ConstantClause(expr, ComparisonOperator.GT, DBValue.Create(200));
            var clause = lhs.Xor(rhs).Negation();
            var mockBuilder = new Mock<IConstraintDeclBuilder<SqlSnippet>>();

            // Sequence
            var sequence = mockBuilder.MakeSequence();
            sequence.Add(builder => builder.StartClause());
            sequence.Add(builder => builder.StartClause());
            sequence.Add(builder => builder.AddClause(lhs.Matcher()));
            sequence.Add(builder => builder.And());
            sequence.Add(builder => builder.AddClause(rhs.Matcher()));
            sequence.Add(builder => builder.EndClause());
            sequence.Add(builder => builder.Or());
            sequence.Add(builder => builder.StartClause());
            sequence.Add(builder => builder.AddClause(lhsNeg.Matcher()));
            sequence.Add(builder => builder.And());
            sequence.Add(builder => builder.AddClause(rhsNeg.Matcher()));
            sequence.Add(builder => builder.EndClause());
            sequence.Add(builder => builder.EndClause());

            // Act
            var fields = clause.GetDependentFields();
            clause.AddDeclarationTo(mockBuilder.Object);

            // Assert
            fields.Should().BeEquivalentTo(new IField[] { mockField, mockField });
            sequence.VerifyCompleted();
            mockBuilder.VerifyNoOtherCalls();
        }

        [TestMethod, TestCategory("CompoundClauses")]
        public void IfThenClauseNotNegated() {
            // Arrange
            var mockField = new Mock<IField>().Object;
            var expr = new FieldExpression(mockField);
            var pred = new ConstantClause(expr, ComparisonOperator.GTE, DBValue.Create(100));
            var predNeg = new ConstantClause(expr, ComparisonOperator.LT, DBValue.Create(100));
            var subseq = new ConstantClause(expr, ComparisonOperator.LTE, DBValue.Create(200));
            var clause = Clause.IfThen(pred, subseq);
            var mockBuilder = new Mock<IConstraintDeclBuilder<SqlSnippet>>();

            // Sequence
            var sequence = mockBuilder.MakeSequence();
            sequence.Add(builder => builder.StartClause());
            sequence.Add(builder => builder.AddClause(subseq.Matcher()));
            sequence.Add(builder => builder.Or());
            sequence.Add(builder => builder.AddClause(predNeg.Matcher()));
            sequence.Add(builder => builder.EndClause());

            // Act
            var fields = clause.GetDependentFields();
            clause.AddDeclarationTo(mockBuilder.Object);

            // Assert
            fields.Should().BeEquivalentTo(new IField[] { mockField, mockField });
            sequence.VerifyCompleted();
            mockBuilder.VerifyNoOtherCalls();
        }

        [TestMethod, TestCategory("CompoundClauses")]
        public void IfThenClauseNegated() {
            // Arrange
            var mockField = new Mock<IField>().Object;
            var expr = new FieldExpression(mockField);
            var pred = new ConstantClause(expr, ComparisonOperator.GTE, DBValue.Create(100));
            var subseq = new ConstantClause(expr, ComparisonOperator.LTE, DBValue.Create(200));
            var subseqNeg = new ConstantClause(expr, ComparisonOperator.GT, DBValue.Create(200));
            var clause = Clause.IfThen(pred, subseq).Negation();
            var mockBuilder = new Mock<IConstraintDeclBuilder<SqlSnippet>>();

            // Sequence
            var sequence = mockBuilder.MakeSequence();
            sequence.Add(builder => builder.StartClause());
            sequence.Add(builder => builder.AddClause(subseqNeg.Matcher()));
            sequence.Add(builder => builder.And());
            sequence.Add(builder => builder.AddClause(pred.Matcher()));
            sequence.Add(builder => builder.EndClause());

            // Act
            var fields = clause.GetDependentFields();
            clause.AddDeclarationTo(mockBuilder.Object);

            // Assert
            fields.Should().BeEquivalentTo(new IField[] { mockField, mockField });
            sequence.VerifyCompleted();
            mockBuilder.VerifyNoOtherCalls();
        }

        [TestMethod, TestCategory("CompoundClauses")]
        public void IffClauseNotNegated() {
            // Arrange
            var mockField = new Mock<IField>().Object;
            var expr = new FieldExpression(mockField);
            var lhs = new ConstantClause(expr, ComparisonOperator.GTE, DBValue.Create(100));
            var lhsNeg = new ConstantClause(expr, ComparisonOperator.LT, DBValue.Create(100));
            var rhs = new ConstantClause(expr, ComparisonOperator.LTE, DBValue.Create(200));
            var rhsNeg = new ConstantClause(expr, ComparisonOperator.GT, DBValue.Create(200));
            var clause = Clause.Iff(lhs, rhs);
            var mockBuilder = new Mock<IConstraintDeclBuilder<SqlSnippet>>();

            // Sequence
            var sequence = mockBuilder.MakeSequence();
            sequence.Add(builder => builder.StartClause());
            sequence.Add(builder => builder.StartClause());
            sequence.Add(builder => builder.AddClause(lhs.Matcher()));
            sequence.Add(builder => builder.And());
            sequence.Add(builder => builder.AddClause(rhs.Matcher()));
            sequence.Add(builder => builder.EndClause());
            sequence.Add(builder => builder.Or());
            sequence.Add(builder => builder.StartClause());
            sequence.Add(builder => builder.AddClause(lhsNeg.Matcher()));
            sequence.Add(builder => builder.And());
            sequence.Add(builder => builder.AddClause(rhsNeg.Matcher()));
            sequence.Add(builder => builder.EndClause());
            sequence.Add(builder => builder.EndClause());

            // Act
            var fields = clause.GetDependentFields();
            clause.AddDeclarationTo(mockBuilder.Object);

            // Assert
            fields.Should().BeEquivalentTo(new IField[] { mockField, mockField });
            sequence.VerifyCompleted();
            mockBuilder.VerifyNoOtherCalls();
        }

        [TestMethod, TestCategory("CompoundClauses")]
        public void IffClauseNegated() {
            // Arrange
            var mockField = new Mock<IField>().Object;
            var expr = new FieldExpression(mockField);
            var lhs = new ConstantClause(expr, ComparisonOperator.GTE, DBValue.Create(100));
            var lhsNeg = new ConstantClause(expr, ComparisonOperator.LT, DBValue.Create(100));
            var rhs = new ConstantClause(expr, ComparisonOperator.LTE, DBValue.Create(200));
            var rhsNeg = new ConstantClause(expr, ComparisonOperator.GT, DBValue.Create(200));
            var clause = Clause.Iff(lhs, rhs).Negation();
            var mockBuilder = new Mock<IConstraintDeclBuilder<SqlSnippet>>();

            // Sequence
            var sequence = mockBuilder.MakeSequence();
            sequence.Add(builder => builder.StartClause());
            sequence.Add(builder => builder.StartClause());
            sequence.Add(builder => builder.AddClause(lhs.Matcher()));
            sequence.Add(builder => builder.And());
            sequence.Add(builder => builder.AddClause(rhsNeg.Matcher()));
            sequence.Add(builder => builder.EndClause());
            sequence.Add(builder => builder.Or());
            sequence.Add(builder => builder.StartClause());
            sequence.Add(builder => builder.AddClause(rhs.Matcher()));
            sequence.Add(builder => builder.And());
            sequence.Add(builder => builder.AddClause(lhsNeg.Matcher()));
            sequence.Add(builder => builder.EndClause());
            sequence.Add(builder => builder.EndClause());

            // Act
            var fields = clause.GetDependentFields();
            clause.AddDeclarationTo(mockBuilder.Object);

            // Assert
            fields.Should().BeEquivalentTo(new IField[] { mockField, mockField });
            sequence.VerifyCompleted();
            mockBuilder.VerifyNoOtherCalls();
        }
    }
}
