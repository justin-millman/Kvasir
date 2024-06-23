using Cybele.Core;
using FluentAssertions;
using Kvasir.Annotations;
using Kvasir.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace UT.Kvasir.Annotations {
    [TestClass, TestCategory("Property Attributes")]
    public class PropertyAttributeTests : AnnotationTestBase {
        [TestMethod] public void Column() {
            // Arrange
            var column = 31;

            // Act
            var attr = new ColumnAttribute(column);

            // Assert
            attr.Column.Should().Be(column);
        }

        [TestMethod] public void Column_UniqueId() {
            // Arrange
            var attr = new ColumnAttribute(0);

            // Act
            var isUnique = ids_.Add(attr.TypeId);

            // Assert
            isUnique.Should().BeTrue();
        }

        [TestMethod] public void Default_Direct() {
            // Arrange
            var value = "Bozeman";

            // Act
            var attr = new DefaultAttribute(value);

            // Assert
            attr.Value.Should().Be(value);
            attr.Path.Should().BeEmpty();
        }

        [TestMethod] public void Default_Neted() {
            // Arrange
            var value = "Gainesville";
            var path = "Nested.Path";

            // Act
            var attr = new DefaultAttribute(value) { Path = path };

            // Assert
            attr.Value.Should().Be(value);
            attr.Path.Should().Be(path);
        }

        [TestMethod] public void Default_Null() {
            // Arrange

            // Act
            var attr = new DefaultAttribute(null);

            // Assert
            attr.Value.Should().Be(DBNull.Value);
        }

        [TestMethod] public void Default_DuplicateWithPath() {
            // Arrange
            var path = "Nested.Path";
            var original = new DefaultAttribute(1000);

            // Act
            var attr = (original as INestableAnnotation).WithPath(path);

            // Assert
            attr.Should().BeOfType<DefaultAttribute>();
            attr.Path.Should().Be(path);
            (attr as DefaultAttribute)!.Value.Should().Be(original.Value);
        }

        [TestMethod] public void Default_UniqueId() {
            // Arrange
            var attr = new DefaultAttribute("Bellevue");

            // Act
            var isUnique = ids_.Add(attr.TypeId);

            // Assert
            isUnique.Should().BeTrue();
        }

        [TestMethod] public void DataConverter_Direct() {
            // Arrange

            // Act
            var attr = new DataConverterAttribute<GoodConverter>();

            // Assert
            attr.DataConverter.Should().BeOfType<DataConverter>();
        }

        [TestMethod] public void DataConverter_ErrorConstructingConverter() {
            // Arrange
            var type = typeof(ErrorConverter);

            // Act
            var attr = new DataConverterAttribute<ErrorConverter>();

            // Assert
            attr.UserError.Should()
                .Match($"*{type.Name}*").And
                .Match($"*{ERROR_STRING}*").And
                .Match("*constructing*");
        }

        [TestMethod] public void DataConverter_UniqueId() {
            // Arrange
            var attr = new DataConverterAttribute<GoodConverter>();

            // Act
            var isUnique = ids_.Add(attr.TypeId);

            // Assert
            isUnique.Should().BeTrue();
        }


        private class GoodConverter : IDataConverter<int, int> {
            public GoodConverter() {}
            public int Convert(int val) { return val; }
            public int Revert(int val) { return val; }
        }
        private class BadConverter : IDataConverter {
            public BadConverter(int _) {}
            DataConverter IDataConverter.ConverterImpl => throw new NotImplementedException();
        }
        private class ErrorConverter : IDataConverter {
            public ErrorConverter() { throw new ArgumentException(ERROR_STRING); }
            DataConverter IDataConverter.ConverterImpl => throw new NotImplementedException();
        }

        private static readonly string ERROR_STRING = "UQYIHUAJSNFPOWEIRHUIBJKNSF";
    }
}

