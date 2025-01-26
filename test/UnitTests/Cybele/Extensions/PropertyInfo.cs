using Cybele.Extensions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UT.Cybele.Extensions {
    [TestClass, TestCategory("PropertyInfo: IsInitOnly")]
    public sealed class PropertyInfo_IsInitOnly : ExtensionTests {
        [TestMethod] public void ReadOnlyPropertyIsNotInitOnly() {
            // Arrange
            var property = typeof(TestStruct).GetProperty(nameof(TestStruct.ReadOnly))!;

            // Act
            var isInitOnly = property.IsInitOnly();

            // Assert
            isInitOnly.Should().BeFalse();
        }

        [TestMethod] public void InitOnlyPropertyIsInitOnly() {
            // Arrange
            var property = typeof(TestStruct).GetProperty(nameof(TestStruct.InitOnly))!;

            // Act
            var isInitOnly = property.IsInitOnly();

            // Assert
            isInitOnly.Should().BeTrue();
        }
        
        [TestMethod] public void WriteablePropertyIsNotInitOnly() {
            // Arrange
            var publicProperty = typeof(TestStruct).GetProperty(nameof(TestStruct.PublicWrite))!;
            var nonPublicProperty = typeof(TestStruct).GetProperty(nameof(TestStruct.NonPublicWrite))!;

            // Act
            var publicIsInitOnly = publicProperty.IsInitOnly();
            var nonPublicIsInitOnly = nonPublicProperty.IsInitOnly();

            // Assert
            publicIsInitOnly.Should().BeFalse();
            nonPublicIsInitOnly.Should().BeFalse();
        }
    }
    

    struct TestStruct {
        public bool ReadOnly { get; }
        public bool InitOnly { get; init; }
        public bool PublicWrite { get; set; }
        public bool NonPublicWrite { get; private set; }
    }
}
