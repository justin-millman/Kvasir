using Cybele.Core;
using FluentAssertions;
using Kvasir.Annotations;
using Kvasir.Core;
using Kvasir.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Linq;

namespace UT.Kvasir.Annotations {
    [TestClass, TestCategory("Inclusion Attributes")]
    public class InclusionAttributeTests : AnnotationTestBase {
        [TestMethod] public void IsOneOf_Direct() {
            // Arrange
            var values = new string[] { "Texarkana", "Cambridge", "West Lafayette" };

            // Act
            var attr = new Check.IsOneOfAttribute(values);
            var clause = attr.MakeConstraint(field_.Object, converter_, settings_);

            // Assert
            attr.Path.Should().BeEmpty();
            clause.Should().BeOfType<InclusionClause>();
            (clause as InclusionClause)!.LHS.Function.Should().NotHaveValue();
            (clause as InclusionClause)!.LHS.Field.Should().Be(field_.Object);
            (clause as InclusionClause)!.Operator.Should().Be(InclusionOperator.In);
            (clause as InclusionClause)!.RHS.Should().BeEquivalentTo(values.Select(v => new DBValue(v)));
        }

        [TestMethod] public void IsOneOf_Nested() {
            // Arrange
            var values = new string[] { "Hershey", "Beaumont", "Provo", "Providence" };
            var path = "Nested.Path";

            // Act
            var attr = new Check.IsOneOfAttribute(values) { Path = path };
            var clause = attr.MakeConstraint(field_.Object, converter_, settings_);

            // Assert
            attr.Path.Should().Be(path);
            clause.Should().BeOfType<InclusionClause>();
            (clause as InclusionClause)!.LHS.Function.Should().NotHaveValue();
            (clause as InclusionClause)!.LHS.Field.Should().Be(field_.Object);
            (clause as InclusionClause)!.Operator.Should().Be(InclusionOperator.In);
            (clause as InclusionClause)!.RHS.Should().BeEquivalentTo(values.Select(v => new DBValue(v)));
        }

        [TestMethod] public void IsOneOf_UniqueId() {
            // Arrange
            var attr = new Check.IsOneOfAttribute("Concord", "Inglewood");

            // Act
            var isUnique = ids_.Add(attr.TypeId);

            // Assert
            isUnique.Should().BeTrue();
        }

        [TestMethod] public void IsOneOf_Empty() {
            // Arrange

            // Act
            Action act = () => new Check.IsOneOfAttribute(Array.Empty<object>());

            // Assert
            act.Should().ThrowExactly<ArgumentException>().WithAnyMessage();
        }

        [TestMethod] public void IsOneOf_IncludeNull() {
            // Arrange
            var values = new string?[] { null, "Boca Raton", "Jupiter", "Athens" };

            // Act
            var attr = new Check.IsOneOfAttribute(values);
            var clause = attr.MakeConstraint(field_.Object, converter_, settings_);

            // Assert
            attr.Path.Should().BeEmpty();
            clause.Should().BeOfType<InclusionClause>();
            (clause as InclusionClause)!.LHS.Function.Should().NotHaveValue();
            (clause as InclusionClause)!.LHS.Field.Should().Be(field_.Object);
            (clause as InclusionClause)!.Operator.Should().Be(InclusionOperator.In);
            (clause as InclusionClause)!.RHS.Should().BeEquivalentTo(values.Select(v => DBValue.Create(v)));
        }

        [TestMethod] public void IsOneOf_ForceNull() {
            // Arrange
            var values = new string?[] { null };

            // Act
            var attr = new Check.IsOneOfAttribute(values);
            var clause = attr.MakeConstraint(field_.Object, converter_, settings_);

            // Assert
            attr.Path.Should().BeEmpty();
            clause.Should().BeOfType<InclusionClause>();
            (clause as InclusionClause)!.LHS.Function.Should().NotHaveValue();
            (clause as InclusionClause)!.LHS.Field.Should().Be(field_.Object);
            (clause as InclusionClause)!.Operator.Should().Be(InclusionOperator.In);
            (clause as InclusionClause)!.RHS.Should().BeEquivalentTo(values.Select(v => DBValue.Create(v)));
        }

        [TestMethod] public void IsNotOneOf_Direct() {
            // Arrange
            var values = new string[] { "Wichita Falls", "Selma", "Roanoke" };

            // Act
            var attr = new Check.IsNotOneOfAttribute(values);
            var clause = attr.MakeConstraint(field_.Object, converter_, settings_);

            // Assert
            attr.Path.Should().BeEmpty();
            clause.Should().BeOfType<InclusionClause>();
            (clause as InclusionClause)!.LHS.Function.Should().NotHaveValue();
            (clause as InclusionClause)!.LHS.Field.Should().Be(field_.Object);
            (clause as InclusionClause)!.Operator.Should().Be(InclusionOperator.NotIn);
            (clause as InclusionClause)!.RHS.Should().BeEquivalentTo(values.Select(v => new DBValue(v)));
        }

        [TestMethod] public void IsNotOneOf_Nested() {
            // Arrange
            var values = new string[] { "Gary", "Dearborn", "Round Rock", "Roswell", "Los Alamos" };
            var path = "Nested.Path";

            // Act
            var attr = new Check.IsNotOneOfAttribute(values) { Path = path };
            var clause = attr.MakeConstraint(field_.Object, converter_, settings_);

            // Assert
            attr.Path.Should().Be(path);
            clause.Should().BeOfType<InclusionClause>();
            (clause as InclusionClause)!.LHS.Function.Should().NotHaveValue();
            (clause as InclusionClause)!.LHS.Field.Should().Be(field_.Object);
            (clause as InclusionClause)!.Operator.Should().Be(InclusionOperator.NotIn);
            (clause as InclusionClause)!.RHS.Should().BeEquivalentTo(values.Select(v => new DBValue(v)));
        }

        [TestMethod] public void IsNotOneOf_UniqueId() {
            // Arrange
            var attr = new Check.IsNotOneOfAttribute("Ames", "Sioux Falls", "Cedar Rapids", "Cooperstown");

            // Act
            var isUnique = ids_.Add(attr.TypeId);

            // Assert
            isUnique.Should().BeTrue();
        }

        [TestMethod] public void IsNotOneOf_Empty() {
            // Arrange

            // Act
            Action act = () => new Check.IsNotOneOfAttribute(Array.Empty<object>());

            // Assert
            act.Should().ThrowExactly<ArgumentException>().WithAnyMessage();
        }

        [TestMethod] public void IsNotOneOf_IncludeNull() {
            // Arrange
            var values = new string?[] { "Gilbert", "Modesto", null, "Sandy Springs" };

            // Act
            var attr = new Check.IsNotOneOfAttribute(values);
            var clause = attr.MakeConstraint(field_.Object, converter_, settings_);

            // Assert
            attr.Path.Should().BeEmpty();
            clause.Should().BeOfType<InclusionClause>();
            (clause as InclusionClause)!.LHS.Function.Should().NotHaveValue();
            (clause as InclusionClause)!.LHS.Field.Should().Be(field_.Object);
            (clause as InclusionClause)!.Operator.Should().Be(InclusionOperator.NotIn);
            (clause as InclusionClause)!.RHS.Should().BeEquivalentTo(values.Select(v => DBValue.Create(v)));
        }

        [TestMethod] public void IsNotOneOf_DisallowNullOnly() {
            // Arrange
            var values = new string?[] { null };

            // Act
            var attr = new Check.IsNotOneOfAttribute(values);
            var clause = attr.MakeConstraint(field_.Object, converter_, settings_);

            // Assert
            attr.Path.Should().BeEmpty();
            clause.Should().BeOfType<InclusionClause>();
            (clause as InclusionClause)!.LHS.Function.Should().NotHaveValue();
            (clause as InclusionClause)!.LHS.Field.Should().Be(field_.Object);
            (clause as InclusionClause)!.Operator.Should().Be(InclusionOperator.NotIn);
            (clause as InclusionClause)!.RHS.Should().BeEquivalentTo(values.Select(v => DBValue.Create(v)));
        }


        static InclusionAttributeTests() {
            field_ = new Mock<IField>();
            field_.Setup(f => f.DataType).Returns(DBType.Text);
        }

        private static readonly Mock<IField> field_;
        private static readonly Settings settings_ = Settings.Default;
        private static readonly DataConverter converter_ = DataConverter.Identity<string>();
    }
}
