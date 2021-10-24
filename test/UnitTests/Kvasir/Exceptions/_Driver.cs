using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace UT.Kvasir.Exceptions {
    public abstract class ExceptionTester<TException> where TException : Exception {
        [TestMethod] public void DefaultConstruct() {
            // Arrange

            // Act
            var exception = (TException)Activator.CreateInstance(EXCEPTION_TYPE)!;
            var message = exception.Message;
            var inner = exception.InnerException;

            // Assert
            message.Should().NotBeNullOrWhiteSpace();
            inner.Should().BeNull();
        }

        [TestMethod] public void CustomMessage() {
            // Arrange
            var expectedMessage = "Custom error message";

            // Act
            var exception = (TException)Activator.CreateInstance(EXCEPTION_TYPE, expectedMessage)!;
            var message = exception.Message;
            var inner = exception.InnerException;

            // Assert
            message.Should().Be(expectedMessage);
            inner.Should().BeNull();
        }

        [TestMethod] public void InnerException() {
            // Arrange
            var expectedMessage = "Custom error message";
            var expectdInner = new NotSupportedException();

            // Act
            var exception = (TException)Activator.CreateInstance(EXCEPTION_TYPE, expectedMessage, expectdInner)!;
            var message = exception.Message;
            var inner = exception.InnerException;

            // Assert
            message.Should().Be(expectedMessage);
            inner.Should().Be(expectdInner);
        }


        private static readonly Type EXCEPTION_TYPE = typeof(TException);
    }
}
