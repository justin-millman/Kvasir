using Kvasir.Schema;
using Kvasir.Schema.Constraints;
using Kvasir.Transcription.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Test.Mocks;

namespace Test.Kvasir.Schema {
    [TestClass]
    public class ConstraintTests {
        [TestMethod, TestCategory("NullityClause")]
        public void NullityClauseFromNullableField() {
            var mock = new Mock<IField>();
            mock.Setup(f => f.Name).Returns(new FieldName("Author"));
            mock.Setup(f => f.Nullability).Returns(IsNullable.Yes);

            var field = mock.Object;
            var op = NullityOperator.IsNotNull;
            var clause = new NullityClause(field, op);

            Assert.IsFalse(clause.LHS.Function.HasValue);
            Assert.AreSame(field, clause.LHS.Field);
            Assert.AreEqual(op, clause.Operator);
        }

        [TestMethod, TestCategory("NullityClause")]
        public void NullityClauseFromNonNullableField() {
            var mock = new Mock<IField>();
            mock.Setup(f => f.Nullability).Returns(IsNullable.No);

            void action() => new NullityClause(mock.Object, NullityOperator.IsNull);
            var exception = Assert.ThrowsException<ArgumentException>(action);
            Assert.AreNotEqual(string.Empty, exception.Message);
        }

        [TestMethod, TestCategory("NullityClause")]
        public void NegateIsNullClause() {
            var mock = new Mock<IField>();
            mock.Setup(f => f.Name).Returns(new FieldName("Breed"));
            mock.Setup(f => f.Nullability).Returns(IsNullable.Yes);

            var field = mock.Object;
            var op = NullityOperator.IsNull;
            var clause = new NullityClause(field, op);
            var negation = clause.Negation();

            var mockGenerator = new MockConstraintSyntaxGenerator();
            negation.AddDeclarationTo(mockGenerator);
            var expected = new SqlSnippet($"{field.Name} IS NOT NULL");
            var actual = mockGenerator.MakeSnippet();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod, TestCategory("NullityClause")]
        public void NegateIsNotNullClause() {
            var mock = new Mock<IField>();
            mock.Setup(f => f.Name).Returns(new FieldName("Stadium"));
            mock.Setup(f => f.Nullability).Returns(IsNullable.Yes);

            var field = mock.Object;
            var op = NullityOperator.IsNotNull;
            var clause = new NullityClause(field, op);
            var negation = clause.Negation();

            var mockGenerator = new MockConstraintSyntaxGenerator();
            negation.AddDeclarationTo(mockGenerator);
            var expected = new SqlSnippet($"{field.Name} IS NULL");
            var actual = mockGenerator.MakeSnippet();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod, TestCategory("NullityClause")]
        public void NullityClauseGetFields() {
            var mock = new Mock<IField>();
            mock.Setup(f => f.Name).Returns(new FieldName("MiddleInitial"));
            mock.Setup(f => f.Nullability).Returns(IsNullable.Yes);

            var field = mock.Object;
            var op = NullityOperator.IsNotNull;
            var clause = new NullityClause(field, op);
            var dependencies = clause.GetDependentFields().ToArray();

            Assert.AreEqual(1, dependencies.Length);
            Assert.AreSame(field, dependencies[0]);
        }

        [TestMethod, TestCategory("InclusionClause")]
        public void InclusionClauseValidValues() {
            var mock = new Mock<IField>();
            mock.Setup(f => f.Name).Returns(new FieldName("Digit"));
            mock.Setup(f => f.DataType).Returns(DBType.Int32);

            var expr = new FieldExpression(mock.Object);
            var op = InclusionOperator.In;
            var values = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }.Select(d => new DBValue(d));
            var clause = new InclusionClause(expr, op, values);

            Assert.AreSame(expr, clause.LHS);
            Assert.AreEqual(op, clause.Operator);
            Assert.IsTrue(values.SequenceEqual(clause.RHS));
        }

        [TestMethod, TestCategory("InclusionClause")]
        public void InclusionClauseMismatchedValues() {
            var mock = new Mock<IField>();
            mock.Setup(f => f.Name).Returns(new FieldName("Flavor"));
            mock.Setup(f => f.DataType).Returns(DBType.Text);

            var expr = new FieldExpression(mock.Object);
            var op = InclusionOperator.NotIn;
            var values = new List<DBValue>() { new DBValue("Cherry"), new DBValue(1.3), new DBValue("Grape") };

            void action() => new InclusionClause(expr, op, values);
            var exception = Assert.ThrowsException<ArgumentException>(action);
            Assert.AreNotEqual(string.Empty, exception.Message);
        }

        [TestMethod, TestCategory("InclusionClause")]
        public void NegateIsInClause() {
            var mock = new Mock<IField>();
            mock.Setup(f => f.Name).Returns(new FieldName("Direction"));
            mock.Setup(f => f.DataType).Returns(DBType.Character);

            var expr = new FieldExpression(mock.Object);
            var op = InclusionOperator.In;
            var values = new List<char>() { 'N', 'S', 'E', 'W' }.Select(c => new DBValue(c));
            var clause = new InclusionClause(expr, op, values);
            var negation = clause.Negation();

            var mockGenerator = new MockConstraintSyntaxGenerator();
            negation.AddDeclarationTo(mockGenerator);
            var expected = new SqlSnippet($"{expr.Field.Name} NOT IN ('N', 'S', 'E', 'W')");
            var actual = mockGenerator.MakeSnippet();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod, TestCategory("InclusionClause")]
        public void NegateIsNotInClause() {
            var mock = new Mock<IField>();
            mock.Setup(f => f.Name).Returns(new FieldName("Grouper"));
            mock.Setup(f => f.DataType).Returns(DBType.Character);

            var expr = new FieldExpression(mock.Object);
            var op = InclusionOperator.NotIn;
            var values = new List<char>() { '(', ')', '[', ']', '{', '}' }.Select(c => new DBValue(c));
            var clause = new InclusionClause(expr, op, values);
            var negation = clause.Negation();

            var mockGenerator = new MockConstraintSyntaxGenerator();
            negation.AddDeclarationTo(mockGenerator);
            var expected = new SqlSnippet($"{expr.Field.Name} IN ('(', ')', '[', ']', '{{', '}}')");
            var actual = mockGenerator.MakeSnippet();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod, TestCategory("InclusionClause")]
        public void InclusionClauseGetFields() {
            var mock = new Mock<IField>();
            mock.Setup(f => f.Name).Returns(new FieldName("Hundred"));
            mock.Setup(f => f.DataType).Returns(DBType.UInt8);

            var expr = new FieldExpression(mock.Object);
            var op = InclusionOperator.In;
            var values = new List<byte>() { 0, 100, 200 }.Select(c => new DBValue(c));
            var clause = new InclusionClause(expr, op, values);
            var dependencies = clause.GetDependentFields().ToArray();

            Assert.AreEqual(1, dependencies.Length);
            Assert.AreSame(expr.Field, dependencies[0]);
        }

        [TestMethod, TestCategory("ValueClause")]
        public void ValueClauseValidConstant() {
            var mock = new Mock<IField>();
            mock.Setup(f => f.Name).Returns(new FieldName("Value"));
            mock.Setup(f => f.DataType).Returns(DBType.Int32);

            var expr = new FieldExpression(mock.Object);
            var op = ComparisonOperator.LessThan;
            var constant = new DBValue(2123);
            var clause = new ConstantValueClause(expr, op, constant);

            Assert.AreSame(expr, clause.LHS);
            Assert.AreEqual(op, clause.Operator);
            Assert.AreEqual(constant, clause.RHS);
        }

        [TestMethod, TestCategory("ValueClause")]
        public void ValueClauseInvalidConstant() {
            var mock = new Mock<IField>();
            mock.Setup(f => f.Name).Returns(new FieldName("Value"));
            mock.Setup(f => f.DataType).Returns(DBType.Int32);

            var expr = new FieldExpression(mock.Object);
            var op = ComparisonOperator.LessThanOrEqual;
            var constant = new DBValue(false);

            void action() => new ConstantValueClause(expr, op, constant);
            var exception = Assert.ThrowsException<ArgumentException>(action);
            Assert.AreNotEqual(string.Empty, exception.Message);
        }

        [TestMethod, TestCategory("ValueClause")]
        public void ValueClauseMatchingTypes() {
            var mockA = new Mock<IField>();
            mockA.Setup(f => f.Name).Returns(new FieldName("First"));
            mockA.Setup(f => f.DataType).Returns(DBType.Int32);
            var mockB = new Mock<IField>();
            mockB.Setup(f => f.Name).Returns(new FieldName("Second"));
            mockB.Setup(f => f.DataType).Returns(DBType.Int32);

            var lhs = new FieldExpression(mockA.Object);
            var op = ComparisonOperator.NotEqual;
            var rhs = new FieldExpression(mockB.Object);
            var clause = new CrossFieldValueConstraint(lhs, op, rhs);

            Assert.AreSame(lhs, clause.LHS);
            Assert.AreEqual(op, clause.Operator);
            Assert.AreSame(rhs, clause.RHS);
        }

        [TestMethod, TestCategory("ValueClause")]
        public void ValueClauseMismatchedTypes() {
            var mockA = new Mock<IField>();
            mockA.Setup(f => f.Name).Returns(new FieldName("First"));
            mockA.Setup(f => f.DataType).Returns(DBType.Int32);
            var mockB = new Mock<IField>();
            mockB.Setup(f => f.Name).Returns(new FieldName("Second"));
            mockB.Setup(f => f.DataType).Returns(DBType.Single);

            var lhs = new FieldExpression(mockA.Object);
            var op = ComparisonOperator.GreaterThanOrEqual;
            var rhs = new FieldExpression(mockB.Object);

            void action() => new CrossFieldValueConstraint(lhs, op, rhs);
            var exception = Assert.ThrowsException<ArgumentException>(action);
            Assert.AreNotEqual(string.Empty, exception.Message);
        }

        [TestMethod, TestCategory("ValueClause")]
        public void NegateEqualsConstantClause() {
            var mock = new Mock<IField>();
            mock.Setup(f => f.Name).Returns(new FieldName("Value"));
            mock.Setup(f => f.DataType).Returns(DBType.Int32);

            var expr = new FieldExpression(mock.Object);
            var op = ComparisonOperator.Equal;
            var constant = new DBValue(-881);
            var clause = new ConstantValueClause(expr, op, constant);
            var negation = clause.Negation();

            var mockGenerator = new MockConstraintSyntaxGenerator();
            negation.AddDeclarationTo(mockGenerator);
            var expected = new SqlSnippet($"{expr.Field.Name} != {constant}");
            var actual = mockGenerator.MakeSnippet();
            Assert.AreEqual(expected, actual);
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
            var negation = clause.Negation();

            var mockGenerator = new MockConstraintSyntaxGenerator();
            negation.AddDeclarationTo(mockGenerator);
            var expected = new SqlSnippet($"{expr.Field.Name} == {constant}");
            var actual = mockGenerator.MakeSnippet();
            Assert.AreEqual(expected, actual);
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
            var negation = clause.Negation();

            var mockGenerator = new MockConstraintSyntaxGenerator();
            negation.AddDeclarationTo(mockGenerator);
            var expected = new SqlSnippet($"{expr.Field.Name} >= {constant}");
            var actual = mockGenerator.MakeSnippet();
            Assert.AreEqual(expected, actual);
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
            var negation = clause.Negation();

            var mockGenerator = new MockConstraintSyntaxGenerator();
            negation.AddDeclarationTo(mockGenerator);
            var expected = new SqlSnippet($"{expr.Field.Name} <= {constant}");
            var actual = mockGenerator.MakeSnippet();
            Assert.AreEqual(expected, actual);
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
            var negation = clause.Negation();

            var mockGenerator = new MockConstraintSyntaxGenerator();
            negation.AddDeclarationTo(mockGenerator);
            var expected = new SqlSnippet($"{expr.Field.Name} > {constant}");
            var actual = mockGenerator.MakeSnippet();
            Assert.AreEqual(expected, actual);
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
            var negation = clause.Negation();

            var mockGenerator = new MockConstraintSyntaxGenerator();
            negation.AddDeclarationTo(mockGenerator);
            var expected = new SqlSnippet($"{expr.Field.Name} < {constant}");
            var actual = mockGenerator.MakeSnippet();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod, TestCategory("ValueClause")]
        public void NegateEqualsExpressionClause() {
            var mockA = new Mock<IField>();
            mockA.Setup(f => f.Name).Returns(new FieldName("First"));
            mockA.Setup(f => f.DataType).Returns(DBType.Int32);
            var mockB = new Mock<IField>();
            mockB.Setup(f => f.Name).Returns(new FieldName("Second"));
            mockB.Setup(f => f.DataType).Returns(DBType.Int32);

            var lhs = new FieldExpression(mockA.Object);
            var op = ComparisonOperator.Equal;
            var rhs = new FieldExpression(mockB.Object);
            var clause = new CrossFieldValueConstraint(lhs, op, rhs);
            var negation = clause.Negation();

            var mockGenerator = new MockConstraintSyntaxGenerator();
            negation.AddDeclarationTo(mockGenerator);
            var expected = new SqlSnippet($"{lhs.Field.Name} != {rhs.Field.Name}");
            var actual = mockGenerator.MakeSnippet();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod, TestCategory("ValueClause")]
        public void NegateNotEqualsExpressionClause() {
            var mockA = new Mock<IField>();
            mockA.Setup(f => f.Name).Returns(new FieldName("First"));
            mockA.Setup(f => f.DataType).Returns(DBType.Int32);
            var mockB = new Mock<IField>();
            mockB.Setup(f => f.Name).Returns(new FieldName("Second"));
            mockB.Setup(f => f.DataType).Returns(DBType.Int32);

            var lhs = new FieldExpression(mockA.Object);
            var op = ComparisonOperator.NotEqual;
            var rhs = new FieldExpression(mockB.Object);
            var clause = new CrossFieldValueConstraint(lhs, op, rhs);
            var negation = clause.Negation();

            var mockGenerator = new MockConstraintSyntaxGenerator();
            negation.AddDeclarationTo(mockGenerator);
            var expected = new SqlSnippet($"{lhs.Field.Name} == {rhs.Field.Name}");
            var actual = mockGenerator.MakeSnippet();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod, TestCategory("ValueClause")]
        public void NegateLessThanExpressionClause() {
            var mockA = new Mock<IField>();
            mockA.Setup(f => f.Name).Returns(new FieldName("First"));
            mockA.Setup(f => f.DataType).Returns(DBType.Int32);
            var mockB = new Mock<IField>();
            mockB.Setup(f => f.Name).Returns(new FieldName("Second"));
            mockB.Setup(f => f.DataType).Returns(DBType.Int32);

            var lhs = new FieldExpression(mockA.Object);
            var op = ComparisonOperator.LessThan;
            var rhs = new FieldExpression(mockB.Object);
            var clause = new CrossFieldValueConstraint(lhs, op, rhs);
            var negation = clause.Negation();

            var mockGenerator = new MockConstraintSyntaxGenerator();
            negation.AddDeclarationTo(mockGenerator);
            var expected = new SqlSnippet($"{lhs.Field.Name} >= {rhs.Field.Name}");
            var actual = mockGenerator.MakeSnippet();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod, TestCategory("ValueClause")]
        public void NegateGreaterThanExpressionClause() {
            var mockA = new Mock<IField>();
            mockA.Setup(f => f.Name).Returns(new FieldName("First"));
            mockA.Setup(f => f.DataType).Returns(DBType.Int32);
            var mockB = new Mock<IField>();
            mockB.Setup(f => f.Name).Returns(new FieldName("Second"));
            mockB.Setup(f => f.DataType).Returns(DBType.Int32);

            var lhs = new FieldExpression(mockA.Object);
            var op = ComparisonOperator.GreaterThan;
            var rhs = new FieldExpression(mockB.Object);
            var clause = new CrossFieldValueConstraint(lhs, op, rhs);
            var negation = clause.Negation();

            var mockGenerator = new MockConstraintSyntaxGenerator();
            negation.AddDeclarationTo(mockGenerator);
            var expected = new SqlSnippet($"{lhs.Field.Name} <= {rhs.Field.Name}");
            var actual = mockGenerator.MakeSnippet();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod, TestCategory("ValueClause")]
        public void NegateLessOrEqualExpressionClause() {
            var mockA = new Mock<IField>();
            mockA.Setup(f => f.Name).Returns(new FieldName("First"));
            mockA.Setup(f => f.DataType).Returns(DBType.Int32);
            var mockB = new Mock<IField>();
            mockB.Setup(f => f.Name).Returns(new FieldName("Second"));
            mockB.Setup(f => f.DataType).Returns(DBType.Int32);

            var lhs = new FieldExpression(mockA.Object);
            var op = ComparisonOperator.LessThanOrEqual;
            var rhs = new FieldExpression(mockB.Object);
            var clause = new CrossFieldValueConstraint(lhs, op, rhs);
            var negation = clause.Negation();

            var mockGenerator = new MockConstraintSyntaxGenerator();
            negation.AddDeclarationTo(mockGenerator);
            var expected = new SqlSnippet($"{lhs.Field.Name} > {rhs.Field.Name}");
            var actual = mockGenerator.MakeSnippet();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod, TestCategory("ValueClause")]
        public void NegateGreaterOrEqualExpressionClause() {
            var mockA = new Mock<IField>();
            mockA.Setup(f => f.Name).Returns(new FieldName("First"));
            mockA.Setup(f => f.DataType).Returns(DBType.Int32);
            var mockB = new Mock<IField>();
            mockB.Setup(f => f.Name).Returns(new FieldName("Second"));
            mockB.Setup(f => f.DataType).Returns(DBType.Int32);

            var lhs = new FieldExpression(mockA.Object);
            var op = ComparisonOperator.GreaterThanOrEqual;
            var rhs = new FieldExpression(mockB.Object);
            var clause = new CrossFieldValueConstraint(lhs, op, rhs);
            var negation = clause.Negation();

            var mockGenerator = new MockConstraintSyntaxGenerator();
            negation.AddDeclarationTo(mockGenerator);
            var expected = new SqlSnippet($"{lhs.Field.Name} < {rhs.Field.Name}");
            var actual = mockGenerator.MakeSnippet();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod, TestCategory("ValueClause")]
        public void ConstantValueClauseGetFields() {
            var mock = new Mock<IField>();
            mock.Setup(f => f.Name).Returns(new FieldName("Value"));
            mock.Setup(f => f.DataType).Returns(DBType.Int32);

            var expr = new FieldExpression(mock.Object);
            var op = ComparisonOperator.GreaterThan;
            var constant = new DBValue(-881);
            var clause = new ConstantValueClause(expr, op, constant);
            var dependencies = clause.GetDependentFields().ToArray();

            Assert.AreEqual(1, dependencies.Length);
            Assert.AreSame(expr.Field, dependencies[0]);
        }

        [TestMethod, TestCategory("ValueClause")]
        public void ExpressionValueClauseGetFields() {
            var mockA = new Mock<IField>();
            mockA.Setup(f => f.Name).Returns(new FieldName("First"));
            mockA.Setup(f => f.DataType).Returns(DBType.Int32);
            var mockB = new Mock<IField>();
            mockB.Setup(f => f.Name).Returns(new FieldName("Second"));
            mockB.Setup(f => f.DataType).Returns(DBType.Int32);

            var lhs = new FieldExpression(mockA.Object);
            var op = ComparisonOperator.Equal;
            var rhs = new FieldExpression(mockB.Object);
            var clause = new CrossFieldValueConstraint(lhs, op, rhs);
            var dependencies = clause.GetDependentFields().ToArray();

            Assert.AreEqual(2, dependencies.Length);
            Assert.AreSame(lhs.Field, dependencies[0]);
            Assert.AreSame(rhs.Field, dependencies[1]);
        }

        [TestMethod, TestCategory("Compounds")]
        public void CreateCompoundAnd() {
            var mock = new Mock<IField>();
            mock.Setup(f => f.Name).Returns(new FieldName("URL"));
            mock.Setup(f => f.Nullability).Returns(IsNullable.Yes);
            mock.Setup(f => f.DataType).Returns(DBType.Text);

            var field = mock.Object;
            var fieldExpr = new FieldExpression(field);
            var lhs = new NullityClause(field, NullityOperator.IsNotNull);
            var rhs = new ConstantValueClause(fieldExpr, ComparisonOperator.NotEqual, new DBValue("www.google.com"));
            var clause = lhs.And(rhs);

            var mockGenerator = new MockConstraintSyntaxGenerator();
            clause.AddDeclarationTo(mockGenerator);
            var expected = "(URL IS NOT NULL <AND> URL != \"www.google.com\")";
            var actual = mockGenerator.MakeSnippet();
            Assert.AreEqual(new SqlSnippet(expected), actual);
        }

        [TestMethod, TestCategory("Compounds")]
        public void NegateCompoundAnd() {
            var mock = new Mock<IField>();
            mock.Setup(f => f.Name).Returns(new FieldName("URL"));
            mock.Setup(f => f.Nullability).Returns(IsNullable.Yes);
            mock.Setup(f => f.DataType).Returns(DBType.Text);

            var field = mock.Object;
            var fieldExpr = new FieldExpression(field);
            var lhs = new NullityClause(field, NullityOperator.IsNotNull);
            var rhs = new ConstantValueClause(fieldExpr, ComparisonOperator.NotEqual, new DBValue("www.google.com"));
            var clause = lhs.And(rhs);
            var negation = clause.Negation();

            var mockGenerator = new MockConstraintSyntaxGenerator();
            negation.AddDeclarationTo(mockGenerator);
            var expected = "(URL IS NULL <OR> URL == \"www.google.com\")";
            var actual = mockGenerator.MakeSnippet();
            Assert.AreEqual(new SqlSnippet(expected), actual);
        }

        [TestMethod, TestCategory("Compounds")]
        public void CompoundAndGetFields() {
            var mock = new Mock<IField>();
            mock.Setup(f => f.Name).Returns(new FieldName("URL"));
            mock.Setup(f => f.Nullability).Returns(IsNullable.Yes);
            mock.Setup(f => f.DataType).Returns(DBType.Text);

            var field = mock.Object;
            var fieldExpr = new FieldExpression(field);
            var lhs = new NullityClause(field, NullityOperator.IsNotNull);
            var rhs = new ConstantValueClause(fieldExpr, ComparisonOperator.NotEqual, new DBValue("www.google.com"));
            var clause = lhs.And(rhs);
            var dependencies = clause.GetDependentFields().ToArray();

            Assert.AreEqual(2, dependencies.Length);
            Assert.AreSame(field, dependencies[0]);
            Assert.AreSame(field, dependencies[1]);
        }

        [TestMethod, TestCategory("Compounds")]
        public void CreateCompoundOr() {
            var mock = new Mock<IField>();
            mock.Setup(f => f.Name).Returns(new FieldName("Age"));
            mock.Setup(f => f.DataType).Returns(DBType.Int64);

            var field = mock.Object;
            var fieldExpr = new FieldExpression(field);
            var lhs = new ConstantValueClause(fieldExpr, ComparisonOperator.GreaterThan, new DBValue(1L));
            var rhs = new ConstantValueClause(fieldExpr, ComparisonOperator.LessThanOrEqual, new DBValue(100L));
            var clause = lhs.Or(rhs);

            var mockGenerator = new MockConstraintSyntaxGenerator();
            clause.AddDeclarationTo(mockGenerator);
            var expected = "(Age > 1 <OR> Age <= 100)";
            var actual = mockGenerator.MakeSnippet();
            Assert.AreEqual(new SqlSnippet(expected), actual);
        }

        [TestMethod, TestCategory("Compounds")]
        public void NegateCompoundOr() {
            var mock = new Mock<IField>();
            mock.Setup(f => f.Name).Returns(new FieldName("Age"));
            mock.Setup(f => f.DataType).Returns(DBType.Int64);

            var field = mock.Object;
            var fieldExpr = new FieldExpression(field);
            var lhs = new ConstantValueClause(fieldExpr, ComparisonOperator.GreaterThan, new DBValue(1L));
            var rhs = new ConstantValueClause(fieldExpr, ComparisonOperator.LessThanOrEqual, new DBValue(100L));
            var clause = lhs.Or(rhs);
            var negation = clause.Negation();

            var mockGenerator = new MockConstraintSyntaxGenerator();
            negation.AddDeclarationTo(mockGenerator);
            var expected = "(Age <= 1 <AND> Age > 100)";
            var actual = mockGenerator.MakeSnippet();
            Assert.AreEqual(new SqlSnippet(expected), actual);
        }

        [TestMethod, TestCategory("Compounds")]
        public void CompoundOrGetFields() {
            var mock = new Mock<IField>();
            mock.Setup(f => f.Name).Returns(new FieldName("Age"));
            mock.Setup(f => f.DataType).Returns(DBType.Int64);

            var field = mock.Object;
            var fieldExpr = new FieldExpression(field);
            var lhs = new ConstantValueClause(fieldExpr, ComparisonOperator.GreaterThan, new DBValue(1L));
            var rhs = new ConstantValueClause(fieldExpr, ComparisonOperator.LessThanOrEqual, new DBValue(100L));
            var clause = lhs.Or(rhs);
            var dependencies = clause.GetDependentFields().ToArray();

            Assert.AreEqual(2, dependencies.Length);
            Assert.AreSame(field, dependencies[0]);
            Assert.AreSame(field, dependencies[1]);
        }

        [TestMethod, TestCategory("Compounds")]
        public void CreateCompoundXor() {
            var mock = new Mock<IField>();
            mock.Setup(f => f.Name).Returns(new FieldName("FirstLetter"));
            mock.Setup(f => f.DataType).Returns(DBType.Character);

            var field = mock.Object;
            var fieldExpr = new FieldExpression(field);
            var lhs = new ConstantValueClause(fieldExpr, ComparisonOperator.NotEqual, new DBValue('~'));
            var rhs = new ConstantValueClause(fieldExpr, ComparisonOperator.GreaterThan, new DBValue('Z'));
            var clause = lhs.Xor(rhs);

            var name = field.Name;
            var mockGenerator = new MockConstraintSyntaxGenerator();
            clause.AddDeclarationTo(mockGenerator);
            var expected = $"(({name} != '~' <AND> {name} <= 'Z') <OR> ({name} == '~' <AND> {name} > 'Z'))";
            var actual = mockGenerator.MakeSnippet();
            Assert.AreEqual(new SqlSnippet(expected), actual);
        }

        [TestMethod, TestCategory("Compounds")]
        public void NegateCompoundXor() {
            var mock = new Mock<IField>();
            mock.Setup(f => f.Name).Returns(new FieldName("FirstLetter"));
            mock.Setup(f => f.DataType).Returns(DBType.Character);

            var field = mock.Object;
            var fieldExpr = new FieldExpression(field);
            var lhs = new ConstantValueClause(fieldExpr, ComparisonOperator.NotEqual, new DBValue('~'));
            var rhs = new ConstantValueClause(fieldExpr, ComparisonOperator.GreaterThan, new DBValue('Z'));
            var clause = lhs.Xor(rhs);
            var negation = clause.Negation();

            var name = field.Name;
            var mockGenerator = new MockConstraintSyntaxGenerator();
            negation.AddDeclarationTo(mockGenerator);
            var expected = $"(({name} == '~' <OR> {name} > 'Z') <AND> ({name} != '~' <OR> {name} <= 'Z'))";
            var actual = mockGenerator.MakeSnippet();
            Assert.AreEqual(new SqlSnippet(expected), actual);
        }

        [TestMethod, TestCategory("Compounds")]
        public void CompoundXorGetFields() {
            var mock = new Mock<IField>();
            mock.Setup(f => f.Name).Returns(new FieldName("FirstLetter"));
            mock.Setup(f => f.DataType).Returns(DBType.Character);

            var field = mock.Object;
            var fieldExpr = new FieldExpression(field);
            var lhs = new ConstantValueClause(fieldExpr, ComparisonOperator.NotEqual, new DBValue('~'));
            var rhs = new ConstantValueClause(fieldExpr, ComparisonOperator.GreaterThan, new DBValue('Z'));
            var clause = lhs.Xor(rhs);
            var dependencies = clause.GetDependentFields().ToArray();

            Assert.AreEqual(4, dependencies.Length);
            Assert.AreSame(field, dependencies[0]);
            Assert.AreSame(field, dependencies[1]);
            Assert.AreSame(field, dependencies[2]);
            Assert.AreSame(field, dependencies[3]);
        }

        [TestMethod, TestCategory("Compounds")]
        public void CreateCompoundConditional() {
            var mock = new Mock<IField>();
            mock.Setup(f => f.Name).Returns(new FieldName("Temperature"));
            mock.Setup(f => f.DataType).Returns(DBType.Double);

            var field = mock.Object;
            var fieldExpr = new FieldExpression(field);
            var lhs = new ConstantValueClause(fieldExpr, ComparisonOperator.LessThanOrEqual, new DBValue(0.0));
            var rhs = new ConstantValueClause(fieldExpr, ComparisonOperator.Equal, new DBValue(-35.2));
            var clause = Clause.IfThen(lhs, rhs);

            var mockGenerator = new MockConstraintSyntaxGenerator();
            clause.AddDeclarationTo(mockGenerator);
            var expected = "(Temperature == -35.2 <OR> Temperature > 0)";
            var actual = mockGenerator.MakeSnippet();
            Assert.AreEqual(new SqlSnippet(expected), actual);
        }

        [TestMethod, TestCategory("Compounds")]
        public void NegateCompoundConditional() {
            var mock = new Mock<IField>();
            mock.Setup(f => f.Name).Returns(new FieldName("Temperature"));
            mock.Setup(f => f.DataType).Returns(DBType.Double);

            var field = mock.Object;
            var fieldExpr = new FieldExpression(field);
            var lhs = new ConstantValueClause(fieldExpr, ComparisonOperator.LessThanOrEqual, new DBValue(0.0));
            var rhs = new ConstantValueClause(fieldExpr, ComparisonOperator.Equal, new DBValue(-35.2));
            var clause = Clause.IfThen(lhs, rhs);
            var negation = clause.Negation();

            var mockGenerator = new MockConstraintSyntaxGenerator();
            negation.AddDeclarationTo(mockGenerator);
            var expected = "(Temperature != -35.2 <AND> Temperature <= 0)";
            var actual = mockGenerator.MakeSnippet();
            Assert.AreEqual(new SqlSnippet(expected), actual);
        }

        [TestMethod, TestCategory("Compounds")]
        public void CompoundConditionalGetFields() {
            var mock = new Mock<IField>();
            mock.Setup(f => f.Name).Returns(new FieldName("Temperature"));
            mock.Setup(f => f.DataType).Returns(DBType.Double);

            var field = mock.Object;
            var fieldExpr = new FieldExpression(field);
            var lhs = new ConstantValueClause(fieldExpr, ComparisonOperator.LessThanOrEqual, new DBValue(0.0));
            var rhs = new ConstantValueClause(fieldExpr, ComparisonOperator.Equal, new DBValue(-35.2));
            var clause = Clause.IfThen(lhs, rhs);
            var dependencies = clause.GetDependentFields().ToArray();

            Assert.AreEqual(2, dependencies.Length);
            Assert.AreSame(field, dependencies[0]);
            Assert.AreSame(field, dependencies[1]);
        }

        [TestMethod, TestCategory("Compounds")]
        public void CreateCompoundBiconditional() {
            var mock = new Mock<IField>();
            mock.Setup(f => f.Name).Returns(new FieldName("Address"));
            mock.Setup(f => f.Nullability).Returns(IsNullable.Yes);
            mock.Setup(f => f.DataType).Returns(DBType.Text);

            var field = mock.Object;
            var fieldExpr = new FieldExpression(FieldFunction.Length, field);
            var lhs = new NullityClause(field, NullityOperator.IsNotNull);
            var rhs = new ConstantValueClause(fieldExpr, ComparisonOperator.GreaterThan, new DBValue(0));
            var clause = Clause.Iff(lhs, rhs);

            var name = field.Name;
            var mockGenerator = new MockConstraintSyntaxGenerator();
            clause.AddDeclarationTo(mockGenerator);
            var expected = $"(({name} IS NOT NULL <AND> Length({name}) > 0) <OR> ({name} IS NULL <AND> Length({name}) <= 0))";
            var actual = mockGenerator.MakeSnippet();
            Assert.AreEqual(new SqlSnippet(expected), actual);
        }
        
        [TestMethod, TestCategory("Compounds")]
        public void NegateCompoundBiconditional() {
            var mock = new Mock<IField>();
            mock.Setup(f => f.Name).Returns(new FieldName("Address"));
            mock.Setup(f => f.Nullability).Returns(IsNullable.Yes);
            mock.Setup(f => f.DataType).Returns(DBType.Text);

            var field = mock.Object;
            var fieldExpr = new FieldExpression(FieldFunction.Length, field);
            var lhs = new NullityClause(field, NullityOperator.IsNotNull);
            var rhs = new ConstantValueClause(fieldExpr, ComparisonOperator.GreaterThan, new DBValue(0));
            var clause = Clause.Iff(lhs, rhs);
            var negation = clause.Negation();

            var name = field.Name;
            var mockGenerator = new MockConstraintSyntaxGenerator();
            negation.AddDeclarationTo(mockGenerator);
            var expected = $"(({name} IS NULL <OR> Length({name}) <= 0) <AND> ({name} IS NOT NULL <OR> Length({name}) > 0))";
            var actual = mockGenerator.MakeSnippet();
            Assert.AreEqual(new SqlSnippet(expected), actual);
        }

        [TestMethod, TestCategory("Compounds")]
        public void CompoundBiconditionalGetFields() {
            var mock = new Mock<IField>();
            mock.Setup(f => f.Name).Returns(new FieldName("Address"));
            mock.Setup(f => f.Nullability).Returns(IsNullable.Yes);
            mock.Setup(f => f.DataType).Returns(DBType.Text);

            var field = mock.Object;
            var fieldExpr = new FieldExpression(FieldFunction.Length, field);
            var lhs = new NullityClause(field, NullityOperator.IsNotNull);
            var rhs = new ConstantValueClause(fieldExpr, ComparisonOperator.GreaterThan, new DBValue(0));
            var clause = Clause.Iff(lhs, rhs);
            var dependencies = clause.GetDependentFields().ToArray();

            Assert.AreEqual(4, dependencies.Length);
            Assert.AreSame(field, dependencies[0]);
            Assert.AreSame(field, dependencies[1]);
            Assert.AreSame(field, dependencies[2]);
            Assert.AreSame(field, dependencies[3]);
        }

        [TestMethod, TestCategory("Expression")]
        public void ExpressionWithoutFunction() {
            var mock = new Mock<IField>();
            mock.Setup(f => f.DataType).Returns(DBType.DateTime);

            var field = mock.Object;
            var expr = new FieldExpression(field);

            Assert.IsFalse(expr.Function.HasValue);
            Assert.AreSame(field, expr.Field);
            Assert.AreEqual(field.DataType, expr.DataType);
        }

        [TestMethod, TestCategory("Expression")]
        public void StringLengthExpression() {
            var mock = new Mock<IField>();
            mock.Setup(f => f.DataType).Returns(DBType.Text);

            var func = FieldFunction.Length;
            var field = mock.Object;
            var expr = new FieldExpression(func, field);

            Assert.IsTrue(expr.Function.Contains(func));
            Assert.AreSame(field, expr.Field);
            Assert.AreEqual(DBType.Int32, expr.DataType);
        }

        [TestMethod, TestCategory("Expression")]
        public void StringLengthExpressionWrongType() {
            var mock = new Mock<IField>();
            mock.Setup(f => f.DataType).Returns(DBType.Guid);

            var func = FieldFunction.Length;
            var field = mock.Object;

            void action() => new FieldExpression(func, field);
            var exception = Assert.ThrowsException<ArgumentException>(action);
            Assert.AreNotEqual(string.Empty, exception.Message);
        }
    }
}
