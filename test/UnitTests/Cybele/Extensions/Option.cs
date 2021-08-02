using Cybele.Extensions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Optional;
using System;

namespace UT.Cybele.Extensions {
    [TestClass, TestCategory("Option: Unwrap")]
    public sealed class Option_Unwrap : ExtensionTests {
        [TestMethod] public void UnwrapSome() {
            // Arrange
            var expected = 389;
            var some = Option.Some(expected);

            // Act
            var actual = some.Unwrap();

            // Assert
            actual.Should().Be(expected);
        }

        [TestMethod] public void UnwrapNone() {
            // Arrange
            var none = Option.None<DateTime>();

            // Act
            Func<DateTime> act = () => none.Unwrap();

            // Assert
            act.Should().ThrowExactly<InvalidOperationException>().WithMessage("*empty*");
        }
    }
}
