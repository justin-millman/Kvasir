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
            generator.Verify(g => g.MakeConstraint(singleField_, singleConv_, settings_), Times.Once);
        }

        [TestMethod] public void Check_NoCtorArgs_MissingConstructor() {
            // Arrange

            // Act
            Action act = () => new CheckAttribute(typeof(ComplexConstraint));

            // Assert
            act.Should().ThrowExactly<MissingMethodException>().WithAnyMessage();
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
            generator.Verify(g => g.MakeConstraint(singleField_, singleConv_, settings_), Times.Once);
        }

        [TestMethod] public void Check_CtorArgs_MissingConstructor() {
            // Arrange

            // Act
            Action act = () => new CheckAttribute(typeof(SimpleConstraint), 100);

            // Assert
            act.Should().ThrowExactly<MissingMethodException>().WithAnyMessage();
        }

        [TestMethod] public void Check_UniqueId() {
            // Arrange
            var attr = new CheckAttribute(typeof(SimpleConstraint));

            // Act
            var isUnique = ids_.Add(attr.TypeId);

            // Assert
            isUnique.Should().BeTrue();
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
            var fields = new string[] { "F0", "F1" };

            // Act
            Action act = () => new Check.ComplexAttribute(typeof(ComplexConstraint), fields);

            // Assert
            act.Should().ThrowExactly<MissingMethodException>().WithAnyMessage();
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
            var fields = new string[] { "F0", "F1" };

            // Act
            Action act = () => new Check.ComplexAttribute(typeof(SimpleConstraint), fields, 100);

            // Assert
            act.Should().ThrowExactly<MissingMethodException>().WithAnyMessage();
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
            public static IConstraintGenerator Mock { get; set; }
            public SimpleConstraint() {}
            public Clause MakeConstraint(IEnumerable<IField> fields, IEnumerable<DataConverter> converters, Settings settings) {
                return Mock!.MakeConstraint(fields, converters, settings);
            }
        }
        private class ComplexConstraint : IConstraintGenerator {
            public static int Argument { get; } = 174;
            public static IConstraintGenerator Mock { get; set; }
            public ComplexConstraint(int arg) { if(arg != Argument) { throw new ArgumentException(); } }
            public Clause MakeConstraint(IEnumerable<IField> fields, IEnumerable<DataConverter> converters, Settings settings) {
                return Mock!.MakeConstraint(fields, converters, settings);
            }
        }
    }
}
