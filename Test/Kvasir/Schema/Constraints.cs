using Kvasir.Schema;
using Kvasir.Schema.Constraints;
using Kvasir.Transcription.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Test.Kvasir.Schema {
    [TestClass]
    public class ConstraintTests {
        [TestMethod, TestCategory("NullityClause")]
        public void NullityClauseFromNullableField() {
            var mockField = new Mock<IField>().WithName("Author").AsNullable();
            var op = NullityOperator.IsNotNull;
            var clause = new NullityClause(mockField.Object, op);

            Assert.IsFalse(clause.LHS.Function.HasValue);
            Assert.AreSame(mockField.Object, clause.LHS.Field);
            Assert.AreEqual(op, clause.Operator);
        }

        [TestMethod, TestCategory("NullityClause")]
        public void NullityClauseFromNonNullableField() {
            var mockField = new Mock<IField>();
            void action() => new NullityClause(mockField.Object, NullityOperator.IsNull);
            var exception = Assert.ThrowsException<ArgumentException>(action);
            Assert.AreNotEqual(string.Empty, exception.Message);
        }

        [TestMethod, TestCategory("NullityClause")]
        public void NegateIsNullClause() {
            var mock = new Mock<IField>().WithName("Breed").AsNullable();
            var mockField = new Mock<IField>().WithName("Breed").AsNullable();
            var op = NullityOperator.IsNull;
            var clause = new NullityClause(mockField.Object, op);

            var expected = new NullityClause(mockField.Object, NullityOperator.IsNotNull);
            var mockBuilder = new Mock<IBuilderCollection>().MockByDefault().ConstraintBuilder();
            var sequence = new CallSequence();
            mockBuilder.Setup(c => c.AddCheck(It.Is(expected.Matcher()))).NextIn(sequence);

            var actual = clause.Negation();
            actual.AddDeclarationTo(mockBuilder.Object);
            sequence.VerifyCompletedBy(mockBuilder);
            mockBuilder.VerifyNoOtherCalls();
        }
        
        [TestMethod, TestCategory("NullityClause")]
        public void NegateIsNotNullClause() {
            var mockField = new Mock<IField>().WithName("Stadium").AsNullable();
            var op = NullityOperator.IsNotNull;
            var clause = new NullityClause(mockField.Object, op);
            var negation = clause.Negation();

            var expected = new NullityClause(mockField.Object, NullityOperator.IsNull);
            var mockBuilder = new Mock<IBuilderCollection>().MockByDefault().ConstraintBuilder();
            var sequence = new CallSequence();
            mockBuilder.Setup(c => c.AddCheck(It.Is(expected.Matcher()))).NextIn(sequence);

            var actual = clause.Negation();
            actual.AddDeclarationTo(mockBuilder.Object);
            sequence.VerifyCompletedBy(mockBuilder);
            mockBuilder.VerifyNoOtherCalls();
        }
        
        [TestMethod, TestCategory("NullityClause")]
        public void NullityClauseGetFields() {
            var mockField = new Mock<IField>().WithName("MiddleInitial").AsNullable();
            var op = NullityOperator.IsNotNull;
            var clause = new NullityClause(mockField.Object, op);
            var dependencies = clause.GetDependentFields().ToArray();

            Assert.AreEqual(1, dependencies.Length);
            Assert.AreSame(mockField.Object, dependencies[0]);
        }
        
        [TestMethod, TestCategory("InclusionClause")]
        public void InclusionClauseValidValues() {
            var mockField = new Mock<IField>().WithName("Digit").WithType(DBType.Int32);
            var expr = new FieldExpression(mockField.Object);
            var op = InclusionOperator.In;
            var values = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }.Select(d => new DBValue(d));
            var clause = new InclusionClause(expr, op, values);

            Assert.AreSame(expr, clause.LHS);
            Assert.AreEqual(op, clause.Operator);
            Assert.IsTrue(values.SequenceEqual(clause.RHS));
        }

        [TestMethod, TestCategory("InclusionClause")]
        public void InclusionClauseMismatchedValues() {
            var mockField = new Mock<IField>().WithName("Flavor").WithType(DBType.Text);
            var expr = new FieldExpression(mockField.Object);
            var op = InclusionOperator.NotIn;
            var values = new List<DBValue>() { new DBValue("Cherry"), new DBValue(1.3), new DBValue("Grape") };

            void action() => new InclusionClause(expr, op, values);
            var exception = Assert.ThrowsException<ArgumentException>(action);
            Assert.AreNotEqual(string.Empty, exception.Message);
        }
        
        [TestMethod, TestCategory("InclusionClause")]
        public void NegateIsInClause() {
            var mockField = new Mock<IField>().WithName("Direction").WithType(DBType.Character);
            var expr = new FieldExpression(mockField.Object);
            var op = InclusionOperator.In;
            var values = new List<char>() { 'N', 'S', 'E', 'W' }.Select(c => new DBValue(c));
            var clause = new InclusionClause(expr, op, values);

            var expected = new InclusionClause(expr, InclusionOperator.NotIn, values);
            var mockBuilder = new Mock<IBuilderCollection>().MockByDefault().ConstraintBuilder();
            var sequence = new CallSequence();
            mockBuilder.Setup(c => c.AddCheck(It.Is(expected.Matcher()))).NextIn(sequence);

            var actual = clause.Negation();
            actual.AddDeclarationTo(mockBuilder.Object);
            sequence.VerifyCompletedBy(mockBuilder);
            mockBuilder.VerifyNoOtherCalls();
        }
        
        [TestMethod, TestCategory("InclusionClause")]
        public void NegateIsNotInClause() {
            var mockField = new Mock<IField>().WithName("Grouper").WithType(DBType.Character);
            var expr = new FieldExpression(mockField.Object);
            var op = InclusionOperator.NotIn;
            var values = new List<char>() { '(', ')', '[', ']', '{', '}' }.Select(c => new DBValue(c));
            var clause = new InclusionClause(expr, op, values);

            var expected = new InclusionClause(expr, InclusionOperator.In, values);
            var mockBuilder = new Mock<IBuilderCollection>().MockByDefault().ConstraintBuilder();
            var sequence = new CallSequence();
            mockBuilder.Setup(c => c.AddCheck(It.Is(expected.Matcher()))).NextIn(sequence);

            var actual = clause.Negation();
            actual.AddDeclarationTo(mockBuilder.Object);
            sequence.VerifyCompletedBy(mockBuilder);
            mockBuilder.VerifyNoOtherCalls();
        }

        [TestMethod, TestCategory("InclusionClause")]
        public void InclusionClauseGetFields() {
            var mockField = new Mock<IField>().WithName("Hundred").WithType(DBType.UInt8);
            var expr = new FieldExpression(mockField.Object);
            var op = InclusionOperator.In;
            var values = new List<byte>() { 0, 100, 200 }.Select(c => new DBValue(c));
            var clause = new InclusionClause(expr, op, values);
            var dependencies = clause.GetDependentFields().ToArray();

            Assert.AreEqual(1, dependencies.Length);
            Assert.AreSame(expr.Field, dependencies[0]);
        }
        
        [TestMethod, TestCategory("ValueClause")]
        public void ValueClauseValidConstant() {
            var mockField = new Mock<IField>().WithName("Value").WithType(DBType.Int32);
            var expr = new FieldExpression(mockField.Object);
            var op = ComparisonOperator.LessThan;
            var constant = new DBValue(2123);
            var clause = new ConstantValueClause(expr, op, constant);

            Assert.AreSame(expr, clause.LHS);
            Assert.AreEqual(op, clause.Operator);
            Assert.AreEqual(constant, clause.RHS);
        }

        [TestMethod, TestCategory("ValueClause")]
        public void ValueClauseInvalidConstant() {
            var mockField = new Mock<IField>().WithName("Value").WithType(DBType.Int32);
            var expr = new FieldExpression(mockField.Object);
            var op = ComparisonOperator.LessThanOrEqual;
            var constant = new DBValue(false);

            void action() => new ConstantValueClause(expr, op, constant);
            var exception = Assert.ThrowsException<ArgumentException>(action);
            Assert.AreNotEqual(string.Empty, exception.Message);
        }

        [TestMethod, TestCategory("ValueClause")]
        public void ValueClauseMatchingTypes() {
            var mockFieldA = new Mock<IField>().WithName("First").WithType(DBType.Int32);
            var mockFieldB = new Mock<IField>().WithName("Second").WithType(DBType.Int32);
            var lhs = new FieldExpression(mockFieldA.Object);
            var op = ComparisonOperator.NotEqual;
            var rhs = new FieldExpression(mockFieldB.Object);
            var clause = new CrossFieldValueConstraint(lhs, op, rhs);

            Assert.AreSame(lhs, clause.LHS);
            Assert.AreEqual(op, clause.Operator);
            Assert.AreSame(rhs, clause.RHS);
        }

        [TestMethod, TestCategory("ValueClause")]
        public void ValueClauseMismatchedTypes() {
            var mockFieldA = new Mock<IField>().WithName("First").WithType(DBType.Int32);
            var mockFieldB = new Mock<IField>().WithName("Second").WithType(DBType.Single);

            var lhs = new FieldExpression(mockFieldA.Object);
            var op = ComparisonOperator.GreaterThanOrEqual;
            var rhs = new FieldExpression(mockFieldB.Object);

            void action() => new CrossFieldValueConstraint(lhs, op, rhs);
            var exception = Assert.ThrowsException<ArgumentException>(action);
            Assert.AreNotEqual(string.Empty, exception.Message);
        }
        
        [TestMethod, TestCategory("ValueClause")]
        public void NegateEqualsConstantClause() {
            var mockField = new Mock<IField>().WithName("Value").WithType(DBType.Int32);
            var expr = new FieldExpression(mockField.Object);
            var op = ComparisonOperator.Equal;
            var constant = new DBValue(-881);
            var clause = new ConstantValueClause(expr, op, constant);

            var expected = new ConstantValueClause(expr, ComparisonOperator.NotEqual, constant);
            var builder = new Mock<IBuilderCollection>().MockByDefault().ConstraintBuilder();
            var sequence = new CallSequence();
            builder.Setup(c => c.AddCheck(It.Is(expected.Matcher()))).NextIn(sequence);

            var actual = clause.Negation();
            actual.AddDeclarationTo(builder.Object);
            sequence.VerifyCompletedBy(builder);
            builder.VerifyNoOtherCalls();
        }

        [TestMethod, TestCategory("ValueClause")]
        public void NegateNotEqualsConstantClause() {
            var mock = new Mock<IField>();
            mock.Setup(f => f.Name).Returns(new FieldName("Value"));
            mock.Setup(f => f.DataType).Returns(DBType.Int32);

            var expr = new FieldExpression(mock.Object);
            var op = ComparisonOperator.NotEqual;
            var constant = new DBValue(-881);
            var clause = new ConstantValueClause(expr, op, constant);

            var expected = new ConstantValueClause(expr, ComparisonOperator.Equal, constant);
            var builder = new Mock<IBuilderCollection>().MockByDefault().ConstraintBuilder();
            var sequence = new CallSequence();
            builder.Setup(c => c.AddCheck(It.Is(expected.Matcher()))).NextIn(sequence);

            var actual = clause.Negation();
            actual.AddDeclarationTo(builder.Object);
            sequence.VerifyCompletedBy(builder);
            builder.VerifyNoOtherCalls();
        }

        [TestMethod, TestCategory("ValueClause")]
        public void NegateLessThanConstantClause() {
            var mock = new Mock<IField>();
            mock.Setup(f => f.Name).Returns(new FieldName("Value"));
            mock.Setup(f => f.DataType).Returns(DBType.Int32);

            var expr = new FieldExpression(mock.Object);
            var op = ComparisonOperator.LessThan;
            var constant = new DBValue(-881);
            var clause = new ConstantValueClause(expr, op, constant);

            var expected = new ConstantValueClause(expr, ComparisonOperator.GreaterThanOrEqual, constant);
            var builder = new Mock<IBuilderCollection>().MockByDefault().ConstraintBuilder();
            var sequence = new CallSequence();
            builder.Setup(c => c.AddCheck(It.Is(expected.Matcher()))).NextIn(sequence);

            var actual = clause.Negation();
            actual.AddDeclarationTo(builder.Object);
            sequence.VerifyCompletedBy(builder);
            builder.VerifyNoOtherCalls();
        }

        [TestMethod, TestCategory("ValueClause")]
        public void NegateGreaterThanConstantClause() {
            var mock = new Mock<IField>();
            mock.Setup(f => f.Name).Returns(new FieldName("Value"));
            mock.Setup(f => f.DataType).Returns(DBType.Int32);

            var expr = new FieldExpression(mock.Object);
            var op = ComparisonOperator.GreaterThan;
            var constant = new DBValue(-881);
            var clause = new ConstantValueClause(expr, op, constant);

            var expected = new ConstantValueClause(expr, ComparisonOperator.LessThanOrEqual, constant);
            var builder = new Mock<IBuilderCollection>().MockByDefault().ConstraintBuilder();
            var sequence = new CallSequence();
            builder.Setup(c => c.AddCheck(It.Is(expected.Matcher()))).NextIn(sequence);

            var actual = clause.Negation();
            actual.AddDeclarationTo(builder.Object);
            sequence.VerifyCompletedBy(builder);
            builder.VerifyNoOtherCalls();
        }

        [TestMethod, TestCategory("ValueClause")]
        public void NegateLessOrEqualConstantClause() {
            var mock = new Mock<IField>();
            mock.Setup(f => f.Name).Returns(new FieldName("Value"));
            mock.Setup(f => f.DataType).Returns(DBType.Int32);

            var expr = new FieldExpression(mock.Object);
            var op = ComparisonOperator.LessThanOrEqual;
            var constant = new DBValue(-881);
            var clause = new ConstantValueClause(expr, op, constant);

            var expected = new ConstantValueClause(expr, ComparisonOperator.GreaterThan, constant);
            var builder = new Mock<IBuilderCollection>().MockByDefault().ConstraintBuilder();
            var sequence = new CallSequence();
            builder.Setup(c => c.AddCheck(It.Is(expected.Matcher()))).NextIn(sequence);

            var actual = clause.Negation();
            actual.AddDeclarationTo(builder.Object);
            sequence.VerifyCompletedBy(builder);
            builder.VerifyNoOtherCalls();
        }

        [TestMethod, TestCategory("ValueClause")]
        public void NegateGreaterOrEqualConstantClause() {
            var mock = new Mock<IField>();
            mock.Setup(f => f.Name).Returns(new FieldName("Value"));
            mock.Setup(f => f.DataType).Returns(DBType.Int32);

            var expr = new FieldExpression(mock.Object);
            var op = ComparisonOperator.GreaterThanOrEqual;
            var constant = new DBValue(-881);
            var clause = new ConstantValueClause(expr, op, constant);

            var expected = new ConstantValueClause(expr, ComparisonOperator.LessThan, constant);
            var builder = new Mock<IBuilderCollection>().MockByDefault().ConstraintBuilder();
            var sequence = new CallSequence();
            builder.Setup(c => c.AddCheck(It.Is(expected.Matcher()))).NextIn(sequence);

            var actual = clause.Negation();
            actual.AddDeclarationTo(builder.Object);
            sequence.VerifyCompletedBy(builder);
            builder.VerifyNoOtherCalls();
        }
        
        [TestMethod, TestCategory("ValueClause")]
        public void NegateEqualsExpressionClause() {
            var mockFieldA = new Mock<IField>().WithName("First").WithType(DBType.Int32);
            var mockFieldB = new Mock<IField>().WithName("Second").WithType(DBType.Int32);
            var lhs = new FieldExpression(mockFieldA.Object);
            var op = ComparisonOperator.Equal;
            var rhs = new FieldExpression(mockFieldB.Object);
            var clause = new CrossFieldValueConstraint(lhs, op, rhs);

            var expected = new CrossFieldValueConstraint(lhs, ComparisonOperator.NotEqual, rhs);
            var builder = new Mock<IBuilderCollection>().MockByDefault().ConstraintBuilder();
            var sequence = new CallSequence();
            builder.Setup(c => c.AddCheck(It.Is(expected.Matcher()))).NextIn(sequence);

            var actual = clause.Negation();
            actual.AddDeclarationTo(builder.Object);
            sequence.VerifyCompletedBy(builder);
            builder.VerifyNoOtherCalls();
        }

        [TestMethod, TestCategory("ValueClause")]
        public void NegateNotEqualsExpressionClause() {
            var mockFieldA = new Mock<IField>().WithName("First").WithType(DBType.Int32);
            var mockFieldB = new Mock<IField>().WithName("Second").WithType(DBType.Int32);
            var lhs = new FieldExpression(mockFieldA.Object);
            var op = ComparisonOperator.NotEqual;
            var rhs = new FieldExpression(mockFieldB.Object);
            var clause = new CrossFieldValueConstraint(lhs, op, rhs);

            var expected = new CrossFieldValueConstraint(lhs, ComparisonOperator.Equal, rhs);
            var builder = new Mock<IBuilderCollection>().MockByDefault().ConstraintBuilder();
            var sequence = new CallSequence();
            builder.Setup(c => c.AddCheck(It.Is(expected.Matcher()))).NextIn(sequence);

            var actual = clause.Negation();
            actual.AddDeclarationTo(builder.Object);
            sequence.VerifyCompletedBy(builder);
            builder.VerifyNoOtherCalls();
        }

        [TestMethod, TestCategory("ValueClause")]
        public void NegateLessThanExpressionClause() {
            var mockFieldA = new Mock<IField>().WithName("First").WithType(DBType.Int32);
            var mockFieldB = new Mock<IField>().WithName("Second").WithType(DBType.Int32);
            var lhs = new FieldExpression(mockFieldA.Object);
            var op = ComparisonOperator.LessThan;
            var rhs = new FieldExpression(mockFieldB.Object);
            var clause = new CrossFieldValueConstraint(lhs, op, rhs);

            var expected = new CrossFieldValueConstraint(lhs, ComparisonOperator.GreaterThanOrEqual, rhs);
            var builder = new Mock<IBuilderCollection>().MockByDefault().ConstraintBuilder();
            var sequence = new CallSequence();
            builder.Setup(c => c.AddCheck(It.Is(expected.Matcher()))).NextIn(sequence);

            var actual = clause.Negation();
            actual.AddDeclarationTo(builder.Object);
            sequence.VerifyCompletedBy(builder);
            builder.VerifyNoOtherCalls();
        }

        [TestMethod, TestCategory("ValueClause")]
        public void NegateGreaterThanExpressionClause() {
            var mockFieldA = new Mock<IField>().WithName("First").WithType(DBType.Int32);
            var mockFieldB = new Mock<IField>().WithName("Second").WithType(DBType.Int32);
            var lhs = new FieldExpression(mockFieldA.Object);
            var op = ComparisonOperator.GreaterThan;
            var rhs = new FieldExpression(mockFieldB.Object);
            var clause = new CrossFieldValueConstraint(lhs, op, rhs);

            var expected = new CrossFieldValueConstraint(lhs, ComparisonOperator.LessThanOrEqual, rhs);
            var builder = new Mock<IBuilderCollection>().MockByDefault().ConstraintBuilder();
            var sequence = new CallSequence();
            builder.Setup(c => c.AddCheck(It.Is(expected.Matcher()))).NextIn(sequence);

            var actual = clause.Negation();
            actual.AddDeclarationTo(builder.Object);
            sequence.VerifyCompletedBy(builder);
            builder.VerifyNoOtherCalls();
        }

        [TestMethod, TestCategory("ValueClause")]
        public void NegateLessOrEqualExpressionClause() {
            var mockFieldA = new Mock<IField>().WithName("First").WithType(DBType.Int32);
            var mockFieldB = new Mock<IField>().WithName("Second").WithType(DBType.Int32);
            var lhs = new FieldExpression(mockFieldA.Object);
            var op = ComparisonOperator.LessThanOrEqual;
            var rhs = new FieldExpression(mockFieldB.Object);
            var clause = new CrossFieldValueConstraint(lhs, op, rhs);

            var expected = new CrossFieldValueConstraint(lhs, ComparisonOperator.GreaterThan, rhs);
            var builder = new Mock<IBuilderCollection>().MockByDefault().ConstraintBuilder();
            var sequence = new CallSequence();
            builder.Setup(c => c.AddCheck(It.Is(expected.Matcher()))).NextIn(sequence);

            var actual = clause.Negation();
            actual.AddDeclarationTo(builder.Object);
            sequence.VerifyCompletedBy(builder);
            builder.VerifyNoOtherCalls();
        }

        [TestMethod, TestCategory("ValueClause")]
        public void NegateGreaterOrEqualExpressionClause() {
            var mockFieldA = new Mock<IField>().WithName("First").WithType(DBType.Int32);
            var mockFieldB = new Mock<IField>().WithName("Second").WithType(DBType.Int32);
            var lhs = new FieldExpression(mockFieldA.Object);
            var op = ComparisonOperator.GreaterThanOrEqual;
            var rhs = new FieldExpression(mockFieldB.Object);
            var clause = new CrossFieldValueConstraint(lhs, op, rhs);

            var expected = new CrossFieldValueConstraint(lhs, ComparisonOperator.LessThan, rhs);
            var builder = new Mock<IBuilderCollection>().MockByDefault().ConstraintBuilder();
            var sequence = new CallSequence();
            builder.Setup(c => c.AddCheck(It.Is(expected.Matcher()))).NextIn(sequence);

            var actual = clause.Negation();
            actual.AddDeclarationTo(builder.Object);
            sequence.VerifyCompletedBy(builder);
            builder.VerifyNoOtherCalls();
        }
        
        [TestMethod, TestCategory("ValueClause")]
        public void ConstantValueClauseGetFields() {
            var mockField = new Mock<IField>().WithName("Value").WithType(DBType.Int32);
            var expr = new FieldExpression(mockField.Object);
            var op = ComparisonOperator.GreaterThan;
            var constant = new DBValue(-881);
            var clause = new ConstantValueClause(expr, op, constant);
            var dependencies = clause.GetDependentFields().ToArray();

            Assert.AreEqual(1, dependencies.Length);
            Assert.AreSame(expr.Field, dependencies[0]);
        }

        [TestMethod, TestCategory("ValueClause")]
        public void ExpressionValueClauseGetFields() {
            var mockFieldA = new Mock<IField>().WithName("First").WithType(DBType.Int32);
            var mockFieldB = new Mock<IField>().WithName("Second").WithType(DBType.Int32);
            var lhs = new FieldExpression(mockFieldA.Object);
            var op = ComparisonOperator.Equal;
            var rhs = new FieldExpression(mockFieldB.Object);
            var clause = new CrossFieldValueConstraint(lhs, op, rhs);
            var dependencies = clause.GetDependentFields().ToArray();

            Assert.AreEqual(2, dependencies.Length);
            Assert.AreSame(lhs.Field, dependencies[0]);
            Assert.AreSame(rhs.Field, dependencies[1]);
        }
        
        [TestMethod, TestCategory("Compounds")]
        public void CreateCompoundAnd() {
            var mockField = new Mock<IField>().WithName("URL").WithType(DBType.Text).AsNullable();
            var fieldExpr = new FieldExpression(mockField.Object);
            var lhs = new NullityClause(mockField.Object, NullityOperator.IsNotNull);
            var rhs = new ConstantValueClause(fieldExpr, ComparisonOperator.NotEqual, new DBValue("www.google.com"));
            var clause = lhs.And(rhs);

            var mockBuilder = new Mock<IBuilderCollection>().MockByDefault().ConstraintBuilder();
            var sequence = new CallSequence();
            mockBuilder.Setup(c => c.NewAndClause()).NextIn(sequence);
            mockBuilder.Setup(c => c.AddCheck(It.Is(lhs.Matcher()))).NextIn(sequence);
            mockBuilder.Setup(c => c.AddCheck(It.Is(rhs.Matcher()))).NextIn(sequence);

            clause.AddDeclarationTo(mockBuilder.Object);
            sequence.VerifyCompletedBy(mockBuilder);
            mockBuilder.VerifyNoOtherCalls();
        }

        [TestMethod, TestCategory("Compounds")]
        public void NegateCompoundAnd() {
            var mockField = new Mock<IField>().WithName("URL").WithType(DBType.Text).AsNullable();
            var fieldExpr = new FieldExpression(mockField.Object);
            var lhs = new NullityClause(mockField.Object, NullityOperator.IsNotNull);
            var rhs = new ConstantValueClause(fieldExpr, ComparisonOperator.NotEqual, new DBValue("www.google.com"));
            var clause = lhs.And(rhs);

            var expected0 = new NullityClause(mockField.Object, NullityOperator.IsNull);
            var expected1 = new ConstantValueClause(fieldExpr, ComparisonOperator.Equal, rhs.RHS);
            var mockBuilder = new Mock<IBuilderCollection>().MockByDefault().ConstraintBuilder();
            var sequence = new CallSequence();
            mockBuilder.Setup(c => c.NewOrClause()).NextIn(sequence);
            mockBuilder.Setup(c => c.AddCheck(It.Is(expected0.Matcher()))).NextIn(sequence);
            mockBuilder.Setup(c => c.AddCheck(It.Is(expected1.Matcher()))).NextIn(sequence);

            var actual = clause.Negation();
            actual.AddDeclarationTo(mockBuilder.Object);
            sequence.VerifyCompletedBy(mockBuilder);
            mockBuilder.VerifyNoOtherCalls();
        }

        [TestMethod, TestCategory("Compounds")]
        public void CompoundAndGetFields() {
            var mockField = new Mock<IField>().WithName("URL").WithType(DBType.Text).AsNullable();
            var fieldExpr = new FieldExpression(mockField.Object);
            var lhs = new NullityClause(mockField.Object, NullityOperator.IsNotNull);
            var rhs = new ConstantValueClause(fieldExpr, ComparisonOperator.NotEqual, new DBValue("www.google.com"));
            var clause = lhs.And(rhs);
            var dependencies = clause.GetDependentFields().ToArray();

            Assert.AreEqual(2, dependencies.Length);
            Assert.AreSame(mockField.Object, dependencies[0]);
            Assert.AreSame(mockField.Object, dependencies[1]);
        }
        
        [TestMethod, TestCategory("Compounds")]
        public void CreateCompoundOr() {
            var mockField = new Mock<IField>().WithName("Age").WithType(DBType.Int64);
            var fieldExpr = new FieldExpression(mockField.Object);
            var lhs = new ConstantValueClause(fieldExpr, ComparisonOperator.GreaterThan, new DBValue(1L));
            var rhs = new ConstantValueClause(fieldExpr, ComparisonOperator.LessThanOrEqual, new DBValue(100L));
            var clause = lhs.Or(rhs);

            var mockBuilder = new Mock<IBuilderCollection>().MockByDefault().ConstraintBuilder();
            var sequence = new CallSequence();
            mockBuilder.Setup(c => c.NewOrClause()).NextIn(sequence);
            mockBuilder.Setup(c => c.AddCheck(It.Is(lhs.Matcher()))).NextIn(sequence);
            mockBuilder.Setup(c => c.AddCheck(It.Is(rhs.Matcher()))).NextIn(sequence);

            clause.AddDeclarationTo(mockBuilder.Object);
            sequence.VerifyCompletedBy(mockBuilder);
            mockBuilder.VerifyNoOtherCalls();
        }

        [TestMethod, TestCategory("Compounds")]
        public void NegateCompoundOr() {
            var mockField = new Mock<IField>().WithName("Age").WithType(DBType.Int64);
            var fieldExpr = new FieldExpression(mockField.Object);
            var lhs = new ConstantValueClause(fieldExpr, ComparisonOperator.GreaterThan, new DBValue(1L));
            var rhs = new ConstantValueClause(fieldExpr, ComparisonOperator.LessThanOrEqual, new DBValue(100L));
            var clause = lhs.Or(rhs);

            var expected0 = new ConstantValueClause(fieldExpr, ComparisonOperator.LessThanOrEqual, lhs.RHS);
            var expected1 = new ConstantValueClause(fieldExpr, ComparisonOperator.GreaterThan, rhs.RHS);
            var mockBuilder = new Mock<IBuilderCollection>().MockByDefault().ConstraintBuilder();
            var sequence = new CallSequence();
            mockBuilder.Setup(c => c.NewAndClause()).NextIn(sequence);
            mockBuilder.Setup(c => c.AddCheck(It.Is(expected0.Matcher()))).NextIn(sequence);
            mockBuilder.Setup(c => c.AddCheck(It.Is(expected1.Matcher()))).NextIn(sequence);

            var actual = clause.Negation();
            actual.AddDeclarationTo(mockBuilder.Object);
            sequence.VerifyCompletedBy(mockBuilder);
            mockBuilder.VerifyNoOtherCalls();
        }

        [TestMethod, TestCategory("Compounds")]
        public void CompoundOrGetFields() {
            var mockField = new Mock<IField>().WithName("Age").WithType(DBType.Int64);
            var fieldExpr = new FieldExpression(mockField.Object);
            var lhs = new ConstantValueClause(fieldExpr, ComparisonOperator.GreaterThan, new DBValue(1L));
            var rhs = new ConstantValueClause(fieldExpr, ComparisonOperator.LessThanOrEqual, new DBValue(100L));
            var clause = lhs.Or(rhs);
            var dependencies = clause.GetDependentFields().ToArray();

            Assert.AreEqual(2, dependencies.Length);
            Assert.AreSame(mockField.Object, dependencies[0]);
            Assert.AreSame(mockField.Object, dependencies[1]);
        }
        
        [TestMethod, TestCategory("Compounds")]
        public void CreateCompoundXor() {
            var mockField = new Mock<IField>().WithName("FirstLetter").WithType(DBType.Character);
            var fieldExpr = new FieldExpression(mockField.Object);
            var lhs = new ConstantValueClause(fieldExpr, ComparisonOperator.NotEqual, new DBValue('~'));
            var rhs = new ConstantValueClause(fieldExpr, ComparisonOperator.GreaterThan, new DBValue('Z'));
            var clause = lhs.Xor(rhs);

            var expected0 = new ConstantValueClause(fieldExpr, ComparisonOperator.NotEqual, lhs.RHS);
            var expected1 = new ConstantValueClause(fieldExpr, ComparisonOperator.LessThanOrEqual, rhs.RHS);
            var expected2 = new ConstantValueClause(fieldExpr, ComparisonOperator.Equal, lhs.RHS);
            var expected3 = new ConstantValueClause(fieldExpr, ComparisonOperator.GreaterThan, rhs.RHS);
            var mockBuilder = new Mock<IBuilderCollection>().MockByDefault().ConstraintBuilder();
            var sequence = new CallSequence();
            mockBuilder.Setup(c => c.NewOrClause()).NextIn(sequence);
            mockBuilder.Setup(c => c.NewAndClause()).NextIn(sequence);
            mockBuilder.Setup(c => c.AddCheck(It.Is(expected0.Matcher()))).NextIn(sequence);
            mockBuilder.Setup(c => c.AddCheck(It.Is(expected1.Matcher()))).NextIn(sequence);
            mockBuilder.Setup(c => c.NewAndClause()).NextIn(sequence);
            mockBuilder.Setup(c => c.AddCheck(It.Is(expected2.Matcher()))).NextIn(sequence);
            mockBuilder.Setup(c => c.AddCheck(It.Is(expected3.Matcher()))).NextIn(sequence);

            clause.AddDeclarationTo(mockBuilder.Object);
            sequence.VerifyCompletedBy(mockBuilder);
            mockBuilder.VerifyNoOtherCalls();
        }

        [TestMethod, TestCategory("Compounds")]
        public void NegateCompoundXor() {
            var mockField = new Mock<IField>().WithName("FirstLetter").WithType(DBType.Character);
            var fieldExpr = new FieldExpression(mockField.Object);
            var lhs = new ConstantValueClause(fieldExpr, ComparisonOperator.NotEqual, new DBValue('~'));
            var rhs = new ConstantValueClause(fieldExpr, ComparisonOperator.GreaterThan, new DBValue('Z'));
            var clause = lhs.Xor(rhs);

            var expected0 = new ConstantValueClause(fieldExpr, ComparisonOperator.Equal, lhs.RHS);
            var expected1 = new ConstantValueClause(fieldExpr, ComparisonOperator.GreaterThan, rhs.RHS);
            var expected2 = new ConstantValueClause(fieldExpr, ComparisonOperator.NotEqual, lhs.RHS);
            var expected3 = new ConstantValueClause(fieldExpr, ComparisonOperator.LessThanOrEqual, rhs.RHS);
            var mockBuilder = new Mock<IBuilderCollection>().MockByDefault().ConstraintBuilder();
            var sequence = new CallSequence();
            mockBuilder.Setup(c => c.NewAndClause()).NextIn(sequence);
            mockBuilder.Setup(c => c.NewOrClause()).NextIn(sequence);
            mockBuilder.Setup(c => c.AddCheck(It.Is(expected0.Matcher()))).NextIn(sequence);
            mockBuilder.Setup(c => c.AddCheck(It.Is(expected1.Matcher()))).NextIn(sequence);
            mockBuilder.Setup(c => c.NewOrClause()).NextIn(sequence);
            mockBuilder.Setup(c => c.AddCheck(It.Is(expected2.Matcher()))).NextIn(sequence);
            mockBuilder.Setup(c => c.AddCheck(It.Is(expected3.Matcher()))).NextIn(sequence);

            var actual = clause.Negation();
            actual.AddDeclarationTo(mockBuilder.Object);
            sequence.VerifyCompletedBy(mockBuilder);
            mockBuilder.VerifyNoOtherCalls();
        }

        [TestMethod, TestCategory("Compounds")]
        public void CompoundXorGetFields() {
            var mockField = new Mock<IField>().WithName("FirstLetter").WithType(DBType.Character);
            var fieldExpr = new FieldExpression(mockField.Object);
            var lhs = new ConstantValueClause(fieldExpr, ComparisonOperator.NotEqual, new DBValue('~'));
            var rhs = new ConstantValueClause(fieldExpr, ComparisonOperator.GreaterThan, new DBValue('Z'));
            var clause = lhs.Xor(rhs);
            var dependencies = clause.GetDependentFields().ToArray();

            Assert.AreEqual(4, dependencies.Length);
            Assert.AreSame(mockField.Object, dependencies[0]);
            Assert.AreSame(mockField.Object, dependencies[1]);
            Assert.AreSame(mockField.Object, dependencies[2]);
            Assert.AreSame(mockField.Object, dependencies[3]);
        }
        
        [TestMethod, TestCategory("Compounds")]
        public void CreateCompoundConditional() {
            var mockField = new Mock<IField>().WithName("Temperature").WithType(DBType.Double);
            var fieldExpr = new FieldExpression(mockField.Object);
            var lhs = new ConstantValueClause(fieldExpr, ComparisonOperator.LessThanOrEqual, new DBValue(0.0));
            var rhs = new ConstantValueClause(fieldExpr, ComparisonOperator.Equal, new DBValue(-35.2));
            var clause = Clause.IfThen(lhs, rhs);

            var expected1 = new ConstantValueClause(fieldExpr, ComparisonOperator.GreaterThan, lhs.RHS);
            var mockBuilder = new Mock<IBuilderCollection>().MockByDefault().ConstraintBuilder();
            var sequence = new CallSequence();
            mockBuilder.Setup(c => c.NewOrClause()).NextIn(sequence);
            mockBuilder.Setup(c => c.AddCheck(It.Is(rhs.Matcher()))).NextIn(sequence);
            mockBuilder.Setup(c => c.AddCheck(It.Is(expected1.Matcher()))).NextIn(sequence);

            clause.AddDeclarationTo(mockBuilder.Object);
            sequence.VerifyCompletedBy(mockBuilder);
            mockBuilder.VerifyNoOtherCalls();
        }

        [TestMethod, TestCategory("Compounds")]
        public void NegateCompoundConditional() {
            var mockField = new Mock<IField>().WithName("Temperature").WithType(DBType.Double);
            var fieldExpr = new FieldExpression(mockField.Object);
            var lhs = new ConstantValueClause(fieldExpr, ComparisonOperator.LessThanOrEqual, new DBValue(0.0));
            var rhs = new ConstantValueClause(fieldExpr, ComparisonOperator.Equal, new DBValue(-35.2));
            var clause = Clause.IfThen(lhs, rhs);

            var expected0 = new ConstantValueClause(fieldExpr, ComparisonOperator.NotEqual, rhs.RHS);
            var mockBuilder = new Mock<IBuilderCollection>().MockByDefault().ConstraintBuilder();
            var sequence = new CallSequence();
            mockBuilder.Setup(c => c.NewAndClause()).NextIn(sequence);
            mockBuilder.Setup(c => c.AddCheck(It.Is(expected0.Matcher()))).NextIn(sequence);
            mockBuilder.Setup(c => c.AddCheck(It.Is(lhs.Matcher()))).NextIn(sequence);

            var actual = clause.Negation();
            actual.AddDeclarationTo(mockBuilder.Object);
            sequence.VerifyCompletedBy(mockBuilder);
            mockBuilder.VerifyNoOtherCalls();
        }

        [TestMethod, TestCategory("Compounds")]
        public void CompoundConditionalGetFields() {
            var mockField = new Mock<IField>().WithName("Temperature").WithType(DBType.Double);
            var fieldExpr = new FieldExpression(mockField.Object);
            var lhs = new ConstantValueClause(fieldExpr, ComparisonOperator.LessThanOrEqual, new DBValue(0.0));
            var rhs = new ConstantValueClause(fieldExpr, ComparisonOperator.Equal, new DBValue(-35.2));
            var clause = Clause.IfThen(lhs, rhs);
            var dependencies = clause.GetDependentFields().ToArray();

            Assert.AreEqual(2, dependencies.Length);
            Assert.AreSame(mockField.Object, dependencies[0]);
            Assert.AreSame(mockField.Object, dependencies[1]);
        }
        
        [TestMethod, TestCategory("Compounds")]
        public void CreateCompoundBiconditional() {
            var mockField = new Mock<IField>().WithName("Address").WithType(DBType.Text).AsNullable();
            var fieldExpr = new FieldExpression(FieldFunction.Length, mockField.Object);
            var lhs = new NullityClause(mockField.Object, NullityOperator.IsNotNull);
            var rhs = new ConstantValueClause(fieldExpr, ComparisonOperator.GreaterThan, new DBValue(0));
            var clause = Clause.Iff(lhs, rhs);

            var expected2 = new NullityClause(mockField.Object, NullityOperator.IsNull);
            var expected3 = new ConstantValueClause(fieldExpr, ComparisonOperator.LessThanOrEqual, rhs.RHS);
            var mockBuilder = new Mock<IBuilderCollection>().MockByDefault().ConstraintBuilder();
            var sequence = new CallSequence();
            mockBuilder.Setup(c => c.NewOrClause()).NextIn(sequence);
            mockBuilder.Setup(c => c.NewAndClause()).NextIn(sequence);
            mockBuilder.Setup(c => c.AddCheck(It.Is(lhs.Matcher()))).NextIn(sequence);
            mockBuilder.Setup(c => c.AddCheck(It.Is(rhs.Matcher()))).NextIn(sequence);
            mockBuilder.Setup(c => c.NewAndClause()).NextIn(sequence);
            mockBuilder.Setup(c => c.AddCheck(It.Is(expected2.Matcher()))).NextIn(sequence);
            mockBuilder.Setup(c => c.AddCheck(It.Is(expected3.Matcher()))).NextIn(sequence);

            clause.AddDeclarationTo(mockBuilder.Object);
            sequence.VerifyCompletedBy(mockBuilder);
            mockBuilder.VerifyNoOtherCalls();
        }
        
        [TestMethod, TestCategory("Compounds")]
        public void NegateCompoundBiconditional() {
            var mockField = new Mock<IField>().WithName("Address").WithType(DBType.Text).AsNullable();
            var fieldExpr = new FieldExpression(FieldFunction.Length, mockField.Object);
            var lhs = new NullityClause(mockField.Object, NullityOperator.IsNotNull);
            var rhs = new ConstantValueClause(fieldExpr, ComparisonOperator.GreaterThan, new DBValue(0));
            var clause = Clause.Iff(lhs, rhs);

            var expected0 = new NullityClause(mockField.Object, NullityOperator.IsNull);
            var expected1 = new ConstantValueClause(fieldExpr, ComparisonOperator.LessThanOrEqual, rhs.RHS);
            var mockBuilder = new Mock<IBuilderCollection>().MockByDefault().ConstraintBuilder();
            var sequence = new CallSequence();
            mockBuilder.Setup(c => c.NewAndClause()).NextIn(sequence);
            mockBuilder.Setup(c => c.NewOrClause()).NextIn(sequence);
            mockBuilder.Setup(c => c.AddCheck(It.Is(expected0.Matcher()))).NextIn(sequence);
            mockBuilder.Setup(c => c.AddCheck(It.Is(expected1.Matcher()))).NextIn(sequence);
            mockBuilder.Setup(c => c.NewOrClause()).NextIn(sequence);
            mockBuilder.Setup(c => c.AddCheck(It.Is(lhs.Matcher()))).NextIn(sequence);
            mockBuilder.Setup(c => c.AddCheck(It.Is(rhs.Matcher()))).NextIn(sequence);

            var actual = clause.Negation();
            actual.AddDeclarationTo(mockBuilder.Object);
            sequence.VerifyCompletedBy(mockBuilder);
            mockBuilder.VerifyNoOtherCalls();
        }

        [TestMethod, TestCategory("Compounds")]
        public void CompoundBiconditionalGetFields() {
            var mockField = new Mock<IField>().WithName("Address").WithType(DBType.Text).AsNullable();
            var fieldExpr = new FieldExpression(FieldFunction.Length, mockField.Object);
            var lhs = new NullityClause(mockField.Object, NullityOperator.IsNotNull);
            var rhs = new ConstantValueClause(fieldExpr, ComparisonOperator.GreaterThan, new DBValue(0));
            var clause = Clause.Iff(lhs, rhs);
            var dependencies = clause.GetDependentFields().ToArray();

            Assert.AreEqual(4, dependencies.Length);
            Assert.AreSame(mockField.Object, dependencies[0]);
            Assert.AreSame(mockField.Object, dependencies[1]);
            Assert.AreSame(mockField.Object, dependencies[2]);
            Assert.AreSame(mockField.Object, dependencies[3]);
        }
        
        [TestMethod, TestCategory("Expression")]
        public void ExpressionWithoutFunction() {
            var mockField = new Mock<IField>().WithType(DBType.DateTime);
            var expr = new FieldExpression(mockField.Object);

            Assert.IsFalse(expr.Function.HasValue);
            Assert.AreSame(mockField.Object, expr.Field);
            Assert.AreEqual(mockField.Object.DataType, expr.DataType);
        }

        [TestMethod, TestCategory("Expression")]
        public void StringLengthExpression() {
            var mockField = new Mock<IField>().WithType(DBType.Text);
            var func = FieldFunction.Length;
            var expr = new FieldExpression(func, mockField.Object);

            Assert.IsTrue(expr.Function.Contains(func));
            Assert.AreSame(mockField.Object, expr.Field);
            Assert.AreEqual(DBType.Int32, expr.DataType);
        }

        [TestMethod, TestCategory("Expression")]
        public void StringLengthExpressionWrongType() {
            var mockField = new Mock<IField>().WithType(DBType.Guid);
            var func = FieldFunction.Length;

            void action() => new FieldExpression(func, mockField.Object);
            var exception = Assert.ThrowsException<ArgumentException>(action);
            Assert.AreNotEqual(string.Empty, exception.Message);
        }
        
        [TestMethod, TestCategory("CheckConstraint")]
        public void CheckConstraintNoName() {
            var mockFieldA = new Mock<IField>().WithName("Position").WithType(DBType.Text).AsNullable();
            var mockFieldB = new Mock<IField>().WithName("JerseyNum").WithType(DBType.Int32);
            var aExpr = new FieldExpression(mockFieldA.Object);
            var bExpr = new FieldExpression(mockFieldB.Object);
            var aLenExpr = new FieldExpression(FieldFunction.Length, mockFieldA.Object);

            var zeroClause = new ConstantValueClause(bExpr, ComparisonOperator.GreaterThan, new DBValue(0));
            var posNotNullClause = new NullityClause(mockFieldA.Object, NullityOperator.IsNotNull);
            var posLengthClause = new ConstantValueClause(aLenExpr, ComparisonOperator.GreaterThan, new DBValue(0));
            var jersey20Clause = new ConstantValueClause(bExpr, ComparisonOperator.LessThan, new DBValue(20));
            var qbClause = new ConstantValueClause(aExpr, ComparisonOperator.Equal, new DBValue("QB"));
            var qbLongClause = new ConstantValueClause(aExpr, ComparisonOperator.Equal, new DBValue("Quarterback"));
            var clause = zeroClause
                .And(Clause.IfThen(posNotNullClause, posLengthClause))
                .And(Clause.IfThen(jersey20Clause, qbClause.Or(qbLongClause)));
            var constraint = new CheckConstraint(clause);

            var expected5 = new NullityClause(mockFieldA.Object, NullityOperator.IsNull);
            var expected11 = new ConstantValueClause(bExpr, ComparisonOperator.GreaterThanOrEqual, jersey20Clause.RHS);
            var mockBuilders = new Mock<IBuilderCollection>().MockByDefault();
            var sequence = new CallSequence();
            mockBuilders.ConstraintBuilder().Setup(c => c.NewAndClause()).NextIn(sequence);
            mockBuilders.ConstraintBuilder().Setup(c => c.NewAndClause()).NextIn(sequence);
            mockBuilders.ConstraintBuilder().Setup(c => c.AddCheck(It.Is(zeroClause.Matcher()))).NextIn(sequence);
            mockBuilders.ConstraintBuilder().Setup(c => c.NewOrClause()).NextIn(sequence);
            mockBuilders.ConstraintBuilder().Setup(c => c.AddCheck(It.Is(posLengthClause.Matcher()))).NextIn(sequence);
            mockBuilders.ConstraintBuilder().Setup(c => c.AddCheck(It.Is(expected5.Matcher()))).NextIn(sequence);
            mockBuilders.ConstraintBuilder().Setup(c => c.NewOrClause()).NextIn(sequence);
            mockBuilders.ConstraintBuilder().Setup(c => c.NewOrClause()).NextIn(sequence);
            mockBuilders.ConstraintBuilder().Setup(c => c.AddCheck(It.Is(qbClause.Matcher()))).NextIn(sequence);
            mockBuilders.ConstraintBuilder().Setup(c => c.AddCheck(It.Is(qbLongClause.Matcher()))).NextIn(sequence);
            mockBuilders.ConstraintBuilder().Setup(c => c.AddCheck(It.Is(expected11.Matcher()))).NextIn(sequence);
            mockBuilders.ConstraintBuilder().Setup(c => c.Build()).NextIn(sequence);

            constraint.GenerateDeclaration(mockBuilders.Object);
            sequence.VerifyCompletedBy(mockBuilders.ConstraintBuilder());
            mockBuilders.ConstraintBuilder().VerifyNoOtherCalls();
        }

        [TestMethod, TestCategory("CheckConstraint")]
        public void CheckConstraintWithName() {
            var mockFieldA = new Mock<IField>().WithName("Position").WithType(DBType.Text).AsNullable();
            var mockFieldB = new Mock<IField>().WithName("JerseyNum").WithType(DBType.Int32);
            var aExpr = new FieldExpression(mockFieldA.Object);
            var bExpr = new FieldExpression(mockFieldB.Object);
            var aLenExpr = new FieldExpression(FieldFunction.Length, mockFieldA.Object);

            var zeroClause = new ConstantValueClause(bExpr, ComparisonOperator.GreaterThan, new DBValue(0));
            var posNotNullClause = new NullityClause(mockFieldA.Object, NullityOperator.IsNotNull);
            var posLengthClause = new ConstantValueClause(aLenExpr, ComparisonOperator.GreaterThan, new DBValue(0));
            var jersey20Clause = new ConstantValueClause(bExpr, ComparisonOperator.LessThan, new DBValue(20));
            var qbClause = new ConstantValueClause(aExpr, ComparisonOperator.Equal, new DBValue("QB"));
            var qbLongClause = new ConstantValueClause(aExpr, ComparisonOperator.Equal, new DBValue("Quarterback"));
            var clause = zeroClause
                .And(Clause.IfThen(posNotNullClause, posLengthClause))
                .And(Clause.IfThen(jersey20Clause, qbClause.Or(qbLongClause)));
            var name = new ConstraintName("CHK_QB#");
            var constraint = new CheckConstraint(name, clause);

            var expected5 = new NullityClause(mockFieldA.Object, NullityOperator.IsNull);
            var expected11 = new ConstantValueClause(bExpr, ComparisonOperator.GreaterThanOrEqual, jersey20Clause.RHS);
            var mockBuilders = new Mock<IBuilderCollection>().MockByDefault();
            var sequence = new CallSequence();
            mockBuilders.ConstraintBuilder().Setup(c => c.NewAndClause()).NextIn(sequence);
            mockBuilders.ConstraintBuilder().Setup(c => c.NewAndClause()).NextIn(sequence);
            mockBuilders.ConstraintBuilder().Setup(c => c.AddCheck(It.Is(zeroClause.Matcher()))).NextIn(sequence);
            mockBuilders.ConstraintBuilder().Setup(c => c.NewOrClause()).NextIn(sequence);
            mockBuilders.ConstraintBuilder().Setup(c => c.AddCheck(It.Is(posLengthClause.Matcher()))).NextIn(sequence);
            mockBuilders.ConstraintBuilder().Setup(c => c.AddCheck(It.Is(expected5.Matcher()))).NextIn(sequence);
            mockBuilders.ConstraintBuilder().Setup(c => c.NewOrClause()).NextIn(sequence);
            mockBuilders.ConstraintBuilder().Setup(c => c.NewOrClause()).NextIn(sequence);
            mockBuilders.ConstraintBuilder().Setup(c => c.AddCheck(It.Is(qbClause.Matcher()))).NextIn(sequence);
            mockBuilders.ConstraintBuilder().Setup(c => c.AddCheck(It.Is(qbLongClause.Matcher()))).NextIn(sequence);
            mockBuilders.ConstraintBuilder().Setup(c => c.AddCheck(It.Is(expected11.Matcher()))).NextIn(sequence);
            mockBuilders.ConstraintBuilder().Setup(c => c.Build()).NextIn(sequence);

            constraint.GenerateDeclaration(mockBuilders.Object);
            mockBuilders.ConstraintBuilder().Verify(c => c.SetName(name), Times.AtLeastOnce());
            sequence.VerifyCompletedBy(mockBuilders.ConstraintBuilder());
            mockBuilders.ConstraintBuilder().VerifyNoOtherCalls();
        }

        [TestMethod, TestCategory("CheckConstraint")]
        public void CheckConstraintGetFields() {
            var mockFieldA = new Mock<IField>().WithName("Position").WithType(DBType.Text).AsNullable();
            var mockFieldB = new Mock<IField>().WithName("JerseyNum").WithType(DBType.Int32);
            var aExpr = new FieldExpression(mockFieldA.Object);
            var bExpr = new FieldExpression(mockFieldB.Object);
            var aLenExpr = new FieldExpression(FieldFunction.Length, mockFieldA.Object);

            var zeroClause = new ConstantValueClause(bExpr, ComparisonOperator.GreaterThan, new DBValue(0));
            var posNotNullClause = new NullityClause(mockFieldA.Object, NullityOperator.IsNotNull);
            var posLengthClause = new ConstantValueClause(aLenExpr, ComparisonOperator.GreaterThan, new DBValue(0));
            var jersey20Clause = new ConstantValueClause(bExpr, ComparisonOperator.LessThan, new DBValue(20));
            var qbClause = new ConstantValueClause(aExpr, ComparisonOperator.Equal, new DBValue("QB"));
            var qbLongClause = new ConstantValueClause(aExpr, ComparisonOperator.Equal, new DBValue("Quarterback"));
            var clause = zeroClause
                .And(Clause.IfThen(posNotNullClause, posLengthClause))
                .And(Clause.IfThen(jersey20Clause, qbClause.Or(qbLongClause)));
            var constraint = new CheckConstraint(clause);
            var dependencies = constraint.GetDependentFields().ToArray();

            Assert.AreEqual(6, dependencies.Length);
            Assert.AreSame(mockFieldB.Object, dependencies[0]);
            Assert.AreSame(mockFieldA.Object, dependencies[1]);
            Assert.AreSame(mockFieldA.Object, dependencies[2]);
            Assert.AreSame(mockFieldA.Object, dependencies[3]);
            Assert.AreSame(mockFieldA.Object, dependencies[4]);
            Assert.AreSame(mockFieldB.Object, dependencies[5]);
        }
    }
}
