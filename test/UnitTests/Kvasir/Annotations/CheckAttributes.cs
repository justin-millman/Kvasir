using Cybele.Core;
using FluentAssertions;
using Kvasir.Annotations;
using Kvasir.Core;
using Kvasir.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UT.Kvasir.Annotations {
    [TestClass, TestCategory("Check Attributes")]
    public class CheckAttributeTests : AnnotationTestBase {
        [TestMethod] public void Check_ErrorConstructingGenerator() {
            // Arrange
            var constraintType = typeof(ErrorConstraint);

            // Act
            var attr = new CheckAttribute<ErrorConstraint>();

            // Assert
            attr.UserError.Should()
                .Match($"*{constraintType.Name}*").And
                .Match($"*{ERROR_STRING}*").And
                .Match("*()*").And
                .Match("*constructing*");
        }

        [TestMethod] public void Check_NoCtorArgs_Direct() {
            // Arrange
            var constraintType = typeof(SimpleConstraint);

            // Act
            var attr = new CheckAttribute<SimpleConstraint>();

            // Assert
            attr.Path.Should().BeEmpty();
            attr.UserError.Should().BeNull();
            attr.ConstraintGenerator.Should().BeOfType(constraintType);
        }

        [TestMethod] public void Check_NoCtorArgs_Nested() {
            // Arrange
            var path = "Nested.Path";
            var constraintType = typeof(SimpleConstraint);

            // Act
            var attr = new CheckAttribute<SimpleConstraint>() { Path = path };

            // Assert
            attr.Path.Should().Be(path);
            attr.UserError.Should().BeNull();
            attr.ConstraintGenerator.Should().BeOfType(constraintType);
        }

        [TestMethod] public void Check_NoCtorArgs_MissingConstructor() {
            // Arrange
            var constraintType = typeof(ComplexConstraint);

            // Act
            var attr = new CheckAttribute<ComplexConstraint>();

            // Assert
            attr.UserError.Should()
                .Match($"*{constraintType.Name}*").And
                .Match("*<none>*").And
                .Match("*construct*");
        }

        [TestMethod] public void Check_CtorArgs_Direct() {
            // Arrange
            var constraintType = typeof(ComplexConstraint);

            // Act
            var attr = new CheckAttribute<ComplexConstraint>(ComplexConstraint.Argument);

            // Assert
            attr.Path.Should().BeEmpty();
            attr.UserError.Should().BeNull();
            attr.ConstraintGenerator.Should().BeOfType(constraintType);
        }

        [TestMethod] public void Check_CtorArgs_Nested() {
            // Arrange
            var path = "Nested.Path";
            var constraintType = typeof(ComplexConstraint);

            // Act
            var attr = new CheckAttribute<ComplexConstraint>(ComplexConstraint.Argument) { Path = path };

            // Assert
            attr.Path.Should().Be(path);
            attr.UserError.Should().BeNull();
            attr.ConstraintGenerator.Should().BeOfType(constraintType);
        }

        [TestMethod] public void Check_CtorArgs_MissingConstructor() {
            // Arrange
            var constraintType = typeof(ComplexConstraint);
            var args = new object[] { 100, "Aloha", 'u' };

            // Act
            var attr = new CheckAttribute<ComplexConstraint>(args);

            // Assert
            attr.UserError.Should()
                .Match($"*{constraintType.Name}*").And
                .Match("*100, \"Aloha\", 'u'*").And
                .Match("*construct*");
        }

        [TestMethod] public void Check_DuplicateWithPath() {
            // Arrange
            var path = "Nested.Path";
            var constraintType = typeof(ComplexConstraint);
            var original = new CheckAttribute<ComplexConstraint>(ComplexConstraint.Argument);

            // Act
            var attr = (original as INestableAnnotation).WithPath(path);

            // Assert
            attr.Should().BeOfType<CheckAttribute<ComplexConstraint>>();
            attr.Path.Should().Be(path);
            (attr as CheckAttribute<ComplexConstraint>)!.UserError.Should().BeNull();
            (attr as CheckAttribute<ComplexConstraint>)!.ConstraintGenerator.Should().BeOfType(constraintType);
            (attr as CheckAttribute<ComplexConstraint>)!.ConstraintGenerator.Should().Be(original.ConstraintGenerator);
        }

        [TestMethod] public void Check_UniqueId() {
            // Arrange
            var attr = new CheckAttribute<SimpleConstraint>();

            // Act
            var isUnique = ids_.Add(attr.TypeId);

            // Assert
            isUnique.Should().BeTrue();
        }

        [TestMethod] public void CheckComplex_ErrorConstructingGenerator() {
            // Arrange
            var constraintType = typeof(ErrorConstraint);

            // Act
            var attr = new Check.ComplexAttribute<ErrorConstraint>(new string[] { "F0", "F1" });

            // Assert
            attr.UserError.Should()
                .Match($"*{constraintType.Name}*").And
                .Match($"*{ERROR_STRING}*").And
                .Match("*()*").And
                .Match("*constructing*");
        }

        [TestMethod] public void CheckComplex_NoCtorArgs() {
            // Arrange
            var constraintType = typeof(SimpleConstraint);
            var fields = new string[] { "F0", "F1" };

            // Act
            var attr = new Check.ComplexAttribute<SimpleConstraint>(fields);

            // Assert
            attr.FieldNames.Should().BeEquivalentTo(fields.Select(n => new FieldName(n)));
            attr.ConstraintGenerator.Should().BeOfType(constraintType);
        }
        
        [TestMethod] public void CheckComplex_NoCtorArgs_MissingConstructor() {
            // Arrange
            var constraintType = typeof(ComplexConstraint);
            var fields = new string[] { "F0", "F1" };

            // Act
            var attr = new Check.ComplexAttribute<ComplexConstraint>(fields);

            // Assert
            attr.UserError.Should()
                .Match($"*{constraintType.Name}*").And
                .Match("*<none>*").And
                .Match("*construct*");
        }

        [TestMethod] public void CheckComplex_CtorArgs() {
            // Arrange
            var constraintType = typeof(ComplexConstraint);
            var fields = new string[] { "F0", "F1" };

            // Act
            var attr = new Check.ComplexAttribute<ComplexConstraint>(fields, ComplexConstraint.Argument);

            // Assert
            attr.FieldNames.Should().BeEquivalentTo(fields.Select(n => new FieldName(n)));
            attr.ConstraintGenerator.Should().BeOfType(constraintType);
        }

        [TestMethod] public void CheckComplex_CtorArgs_MissingConstructor() {
            // Arrange
            var constraintType = typeof(SimpleConstraint);
            var args = new object[] { 100 };
            var fields = new string[] { "F0", "F1" };

            // Act
            var attr = new Check.ComplexAttribute<SimpleConstraint>(fields, args);

            // Assert
            attr.UserError.Should()
                .Match($"*{constraintType.Name}*").And
                .Match("*100*").And
                .Match("*construct*");
        }

        [TestMethod] public void CheckComplex_UniqueId() {
            // Arrange
            var fields = new string[] { "F0", "F1" };
            var attr = new Check.ComplexAttribute<SimpleConstraint>(fields);

            // Act
            var isUnique = ids_.Add(attr.TypeId);

            // Assert
            isUnique.Should().BeTrue();
        }


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
