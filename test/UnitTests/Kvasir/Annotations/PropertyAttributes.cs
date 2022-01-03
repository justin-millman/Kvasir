﻿using Cybele.Core;
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

        [TestMethod] public void Column_Negative() {
            // Arrange
            var column = -2;

            // Act
            Action act = () => new ColumnAttribute(column);

            // Assert
            act.Should().ThrowExactly<ArgumentException>().WithAnyMessage();
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
            var type = typeof(GoodConverter);

            // Act
            var attr = new DataConverterAttribute(type);

            // Assert
            attr.DataConverter.Should().BeOfType<DataConverter>();
            attr.Path.Should().BeEmpty();
        }

        [TestMethod] public void DataConverter_Nested() {
            // Arrange
            var type = typeof(GoodConverter);
            var path = "Nested.Path";

            // Act
            var attr = new DataConverterAttribute(type) { Path = path };

            // Assert
            attr.DataConverter.Should().BeOfType<DataConverter>();
            attr.Path.Should().Be(path);
        }

        [TestMethod] public void DataConverter_WrongInterface() {
            // Arrange
            var type = typeof(string);

            // Act
            Action act = () => new DataConverterAttribute(type);

            // Assert
            act.Should().ThrowExactly<ArgumentException>().WithAnyMessage();
        }

        [TestMethod] public void DataConverter_NotDefaultConstructible() {
            // Arrange
            var type = typeof(BadConverter);

            // Act
            Action act = () => new DataConverterAttribute(type);

            // Assert
            act.Should().ThrowExactly<MissingMethodException>().WithAnyMessage();
        }

        [TestMethod] public void DataConverter_UniqueId() {
            // Arrange
            var attr = new DataConverterAttribute(typeof(GoodConverter));

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
    }
}

