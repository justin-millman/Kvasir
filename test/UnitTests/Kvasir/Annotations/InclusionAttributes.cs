using FluentAssertions;
using Kvasir.Annotations;
using Kvasir.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace UT.Kvasir.Annotations {
    [TestClass, TestCategory("Inclusion Attributes")]
    public class InclusionAttributeTests : AnnotationTestBase {
        [TestMethod] public void IsOneOf_Direct() {
            // Arrange
            var values = new string[] { "Texarkana", "Cambridge", "West Lafayette", "Carlsbad", "Nome" };

            // Act
            var attr = new Check.IsOneOfAttribute(values[0], values[1], values[2], values[3], values[4]);

            // Assert
            attr.Path.Should().BeEmpty();
            attr.Operator.Should().Be(InclusionOperator.In);
            attr.Anchor.Should().BeEquivalentTo(values);
        }

        [TestMethod] public void IsOneOf_Nested() {
            // Arrange
            var values = new string[] { "Hershey", "Beaumont", "Provo", "Providence", "Hot Springs" };
            var path = "Nested.Path";

            // Act
            var attr = new Check.IsOneOfAttribute(values[0], values[1], values[2], values[3], values[4]) { Path = path };

            // Assert
            attr.Path.Should().Be(path);
            attr.Operator.Should().Be(InclusionOperator.In);
            attr.Anchor.Should().BeEquivalentTo(values);
        }

        [TestMethod] public void IsOneOf_UniqueId() {
            // Arrange
            var attr = new Check.IsOneOfAttribute("Concord", "Inglewood", "San Jacinto");

            // Act
            var isUnique = ids_.Add(attr.TypeId);

            // Assert
            isUnique.Should().BeTrue();
        }

        [TestMethod] public void IsOneOf_IncludeNull() {
            // Arrange
            var values = new string?[] { null, "Boca Raton", "Jupiter", "Athens" };

            // Act
            var attr = new Check.IsOneOfAttribute(values[0]!, values[1]!, values[2]!, values[3]!);

            // Assert
            attr.Path.Should().BeEmpty();
            attr.Operator.Should().Be(InclusionOperator.In);
            attr.Anchor.Should().BeEquivalentTo(values.Cast<object?>().Select(v => v ?? DBNull.Value));
        }

        [TestMethod] public void IsOneOf_ForceNull() {
            // Arrange
            var values = new string?[] { null };

            // Act
            var attr = new Check.IsOneOfAttribute(values[0]!);

            // Assert
            attr.Path.Should().BeEmpty();
            attr.Operator.Should().Be(InclusionOperator.In);
            attr.Anchor.Should().BeEquivalentTo(values.Cast<object?>().Select(v => v ?? DBNull.Value));
        }

        [TestMethod] public void IsNotOneOf_Direct() {
            // Arrange
            var values = new string[] { "Wichita Falls", "Selma", "Roanoke" };

            // Act
            var attr = new Check.IsNotOneOfAttribute(values[0], values[1], values[2]);

            // Assert
            attr.Path.Should().BeEmpty();
            attr.Operator.Should().Be(InclusionOperator.NotIn);
            attr.Anchor.Should().BeEquivalentTo(values);
        }

        [TestMethod] public void IsNotOneOf_Nested() {
            // Arrange
            var values = new string[] { "Gary", "Dearborn", "Round Rock", "Roswell", "Los Alamos" };
            var path = "Nested.Path";

            // Act
            var attr = new Check.IsNotOneOfAttribute(values[0], values[1], values[2], values[3], values[4]) { Path = path };

            // Assert
            attr.Path.Should().Be(path);
            attr.Operator.Should().Be(InclusionOperator.NotIn);
            attr.Anchor.Should().BeEquivalentTo(values);
        }

        [TestMethod] public void IsNotOneOf_UniqueId() {
            // Arrange
            var attr = new Check.IsNotOneOfAttribute("Ames", "Sioux Falls", "Cedar Rapids", "Cooperstown");

            // Act
            var isUnique = ids_.Add(attr.TypeId);

            // Assert
            isUnique.Should().BeTrue();
        }

        [TestMethod] public void IsNotOneOf_IncludeNull() {
            // Arrange
            var values = new string?[] { "Gilbert", "Modesto", null, "Sandy Springs" };

            // Act
            var attr = new Check.IsNotOneOfAttribute(values[0]!, values[1]!, values[2]!, values[3]!);

            // Assert
            attr.Path.Should().BeEmpty();
            attr.Operator.Should().Be(InclusionOperator.NotIn);
            attr.Anchor.Should().BeEquivalentTo(values.Cast<object?>().Select(v => v ?? DBNull.Value));
        }

        [TestMethod] public void IsNotOneOf_DisallowNullOnly() {
            // Arrange
            var values = new string?[] { null };

            // Act
            var attr = new Check.IsNotOneOfAttribute(values[0]!);

            // Assert
            attr.Path.Should().BeEmpty();
            attr.Operator.Should().Be(InclusionOperator.NotIn);
            attr.Anchor.Should().BeEquivalentTo(values.Cast<object?>().Select(v => v ?? DBNull.Value));
        }
    }
}
