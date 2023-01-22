using Cybele.Core;
using FluentAssertions;
using Kvasir.Annotations;
using Kvasir.Core;
using Kvasir.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UT.Kvasir.Annotations {
    [TestClass, TestCategory("Check Attributes")]
    public class CheckAttributeTests : AnnotationTestBase {
        [TestMethod] public void Check_TypeNotConstraintGenerator() {
            // Arrange
            var constraintType = typeof(int);

            // Act
            var attr = new CheckAttribute(constraintType);

            // Assert
            attr.UserError.Should()
                .Match($"*{constraintType.Name}*").And
                .Match($"*{nameof(IConstraintGenerator)}*").And
                .Match("*does not implement*");
        }

        [TestMethod] public void Check_ErrorConstructingGenerator() {
            // Arrange
            var constraintType = typeof(ErrorConstraint);

            // Act
            var attr = new CheckAttribute(constraintType);

            // Assert
            attr.UserError.Should()
                .Match($"*{constraintType.Name}*").And
                .Match($"*{ERROR_STRING}*").And
                .Match("*()*").And
                .Match("*constructing*");
        }

        [TestMethod] public void Check_NoCtorArgs_Direct() {
            // Arrange
            var clause = new Mock<Clause>().Object;
            var generator = new Mock<IConstraintGenerator>();
            SimpleConstraint.Mock = generator.Object;

            // Act
            var attr = new CheckAttribute(typeof(SimpleConstraint));
            attr.MakeConstraint(singleField_, singleConv_, settings_);

            // Assert
            attr.Path.Should().BeEmpty();
            attr.UserError.Should().BeNull();
            generator.Verify(g => g.MakeConstraint(singleField_, singleConv_, settings_), Times.Once);
        }

        [TestMethod] public void Check_NoCtorArgs_Nested() {
            // Arrange
            var path = "Nested.Path";
            var clause = new Mock<Clause>().Object;
            var generator = new Mock<IConstraintGenerator>();
            SimpleConstraint.Mock = generator.Object;

            // Act
            var attr = new CheckAttribute(typeof(SimpleConstraint)) { Path = path };
            attr.MakeConstraint(singleField_, singleConv_, settings_);

            // Assert
            attr.Path.Should().Be(path);
            attr.UserError.Should().BeNull();
            generator.Verify(g => g.MakeConstraint(singleField_, singleConv_, settings_), Times.Once);
        }

        [TestMethod] public void Check_NoCtorArgs_MissingConstructor() {
            // Arrange
            var constraintType = typeof(ComplexConstraint);

            // Act
            var attr = new CheckAttribute(constraintType);

            // Assert
            attr.UserError.Should()
                .Match($"*{constraintType.Name}*").And
                .Match("*()*").And
                .Match("*construct*");
        }

        [TestMethod] public void Check_CtorArgs_Direct() {
            // Arrange
            var clause = new Mock<Clause>().Object;
            var generator = new Mock<IConstraintGenerator>();
            ComplexConstraint.Mock = generator.Object;

            // Act
            var attr = new CheckAttribute(typeof(ComplexConstraint), ComplexConstraint.Argument);
            attr.MakeConstraint(singleField_, singleConv_, settings_);

            // Assert
            attr.Path.Should().BeEmpty();
            attr.UserError.Should().BeNull();
            generator.Verify(g => g.MakeConstraint(singleField_, singleConv_, settings_), Times.Once);
        }

        [TestMethod] public void Check_CtorArgs_Nested() {
            // Arrange
            var path = "Nested.Path";
            var clause = new Mock<Clause>().Object;
            var generator = new Mock<IConstraintGenerator>();
            ComplexConstraint.Mock = generator.Object;

            // Act
            var attr = new CheckAttribute(typeof(ComplexConstraint), ComplexConstraint.Argument) { Path = path };
            attr.MakeConstraint(singleField_, singleConv_, settings_);

            // Assert
            attr.Path.Should().Be(path);
            attr.UserError.Should().BeNull();
            generator.Verify(g => g.MakeConstraint(singleField_, singleConv_, settings_), Times.Once);
        }

        [TestMethod] public void Check_CtorArgs_MissingConstructor() {
            // Arrange
            var constraintType = typeof(ComplexConstraint);
            var args = new object[] { 100 };

            // Act
            var attr = new CheckAttribute(constraintType, args);

            // Assert
            attr.UserError.Should()
                .Match($"*{constraintType.Name}*").And
                .Match($"*({string.Join(", ", args)})*").And
                .Match("*construct*");
        }

        [TestMethod] public void Check_UniqueId() {
            // Arrange
            var attr = new CheckAttribute(typeof(SimpleConstraint));

            // Act
            var isUnique = ids_.Add(attr.TypeId);

            // Assert
            isUnique.Should().BeTrue();
        }

        [TestMethod] public void CheckComplex_TypeNotConstraintGenerator() {
            // Arrange
            var constraintType = typeof(AnnotationTestBase);

            // Act
            var attr = new Check.ComplexAttribute(constraintType, new string[] { "F0", "F1" });

            // Assert
            attr.UserError.Should()
                .Match($"*{constraintType.Name}*").And
                .Match($"*{nameof(IConstraintGenerator)}*").And
                .Match("*does not implement*");
        }

        [TestMethod] public void CheckComplex_ErrorConstructingGenerator() {
            // Arrange
            var constraintType = typeof(ErrorConstraint);

            // Act
            var attr = new Check.ComplexAttribute(constraintType, new string[] { "F0", "F1" });

            // Assert
            attr.UserError.Should()
                .Match($"*{constraintType.Name}*").And
                .Match($"*{ERROR_STRING}*").And
                .Match("*()*").And
                .Match("*constructing*");
        }

        [TestMethod] public void CheckComplex_NoCtorArgs() {
            // Arrange
            var clause = new Mock<Clause>().Object;
            var generator = new Mock<IConstraintGenerator>();
            SimpleConstraint.Mock = generator.Object;
            var fields = new string[] { "F0", "F1" };

            // Act
            var attr = new Check.ComplexAttribute(typeof(SimpleConstraint), fields);
            attr.MakeConstraint(multiFields_, multiConvs_, settings_);

            // Assert
            attr.FieldNames.Should().BeEquivalentTo(fields.Select(n => new FieldName(n)));
            generator.Verify(g => g.MakeConstraint(multiFields_, multiConvs_, settings_), Times.Once);
        }
        
        [TestMethod] public void CheckComplex_NoCtorArgs_MissingConstructor() {
            // Arrange
            var constraintType = typeof(ComplexConstraint);
            var fields = new string[] { "F0", "F1" };

            // Act
            var attr = new Check.ComplexAttribute(constraintType, fields);

            // Assert
            attr.UserError.Should()
                .Match($"*{constraintType.Name}*").And
                .Match("*()*").And
                .Match("*construct*");
        }

        [TestMethod] public void CheckComplex_CtorArgs() {
            // Arrange
            var clause = new Mock<Clause>().Object;
            var generator = new Mock<IConstraintGenerator>();
            ComplexConstraint.Mock = generator.Object;
            var fields = new string[] { "F0", "F1" };

            // Act
            var attr = new Check.ComplexAttribute(typeof(ComplexConstraint), fields, ComplexConstraint.Argument);
            attr.MakeConstraint(multiFields_, multiConvs_, settings_);

            // Assert
            attr.FieldNames.Should().BeEquivalentTo(fields.Select(n => new FieldName(n)));
            generator.Verify(g => g.MakeConstraint(multiFields_, multiConvs_, settings_), Times.Once);
        }

        [TestMethod] public void CheckComplex_CtorArgs_MissingConstructor() {
            // Arrange
            var constraintType = typeof(SimpleConstraint);
            var args = new object[] { 100 };
            var fields = new string[] { "F0", "F1" };

            // Act
            var attr = new Check.ComplexAttribute(constraintType, fields, args);

            // Assert
            attr.UserError.Should()
                .Match($"*{constraintType.Name}*").And
                .Match($"*({string.Join(", ", args)})*").And
                .Match("*construct*");
        }

        [TestMethod] public void CheckComplex_UniqueId() {
            // Arrange
            var fields = new string[] { "F0", "F1" };
            var attr = new Check.ComplexAttribute(typeof(SimpleConstraint), fields);

            // Act
            var isUnique = ids_.Add(attr.TypeId);

            // Assert
            isUnique.Should().BeTrue();
        }


        static CheckAttributeTests() {
            var f0 = new Mock<IField>();
            f0.Setup(f => f.DataType).Returns(DBType.Text);
            var f1 = new Mock<IField>();
            f1.Setup(f => f.DataType).Returns(DBType.Double);

            singleField_ = new IField[] { f0.Object };
            multiFields_ = new IField[] { f0.Object, f1.Object };
            singleConv_ = new DataConverter[] { DataConverter.Identity<string>() };
            multiConvs_ = new DataConverter[] { DataConverter.Identity<string>(), DataConverter.Identity<double>() };
        }

        private static readonly IField[] singleField_;
        private static readonly IField[] multiFields_;
        private static readonly Settings settings_ = Settings.Default;
        private static readonly DataConverter[] singleConv_;
        private static readonly DataConverter[] multiConvs_;

        private class SimpleConstraint : IConstraintGenerator {
            public static IConstraintGenerator Mock { get; set; } = default!;
            public SimpleConstraint() {}
            public Clause MakeConstraint(IEnumerable<IField> fields, IEnumerable<DataConverter> converters, Settings settings) {
                return Mock!.MakeConstraint(fields, converters, settings);
            }
        }
        private class ComplexConstraint : IConstraintGenerator {
            public static int Argument { get; } = 174;
            public static IConstraintGenerator Mock { get; set; } = default!;
            public ComplexConstraint(int arg) { if(arg != Argument) { throw new ArgumentException(); } }
            public Clause MakeConstraint(IEnumerable<IField> fields, IEnumerable<DataConverter> converters, Settings settings) {
                return Mock!.MakeConstraint(fields, converters, settings);
            }
        }
        private class ErrorConstraint : IConstraintGenerator {
            public ErrorConstraint() { throw new ArgumentException(ERROR_STRING); }
            public Clause MakeConstraint(IEnumerable<IField> fields, IEnumerable<DataConverter> converters, Settings settings) {
                return null!;
            }
        }

        private static readonly string ERROR_STRING = "%&!(@)#!@&#&!@(&#!@$!@$)()_!@#*!@&";
    }
}
