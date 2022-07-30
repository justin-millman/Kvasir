using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

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
            var expectedInner = new NotSupportedException();

            // Act
            var exception = (TException)Activator.CreateInstance(EXCEPTION_TYPE, expectedMessage, expectedInner)!;
            var message = exception.Message;
            var inner = exception.InnerException;

            // Assert
            message.Should().Be(expectedMessage);
            inner.Should().Be(expectedInner);
        }

        [TestMethod] public void Serialization() {
            #pragma warning disable SYSLIB0011 // Type or member is obsolete
            // Arrange
            var expectedMessage = "Serialization/Deserialization";
            var expectedInner = new AggregateException();
            var exception = (TException)Activator.CreateInstance(EXCEPTION_TYPE, expectedMessage, expectedInner)!;

            // Act
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new MemoryStream();
            formatter.Serialize(stream, exception);
            stream.Seek(0, SeekOrigin.Begin);
            var deserialized = (TException)formatter.Deserialize(stream);
            stream.Close();
            var message = deserialized.Message;
            var inner = deserialized.InnerException!;

            // Assert
            message.Should().Be(expectedMessage);
            inner.Message.Should().Be(expectedInner.Message);
            inner.Source.Should().Be(expectedInner.Source);
            inner.StackTrace.Should().Be(expectedInner.StackTrace);
            inner.HResult.Should().Be(expectedInner.HResult);
            #pragma warning restore SYSLIB0011 // Type or member is obsolete
        }


        private static readonly Type EXCEPTION_TYPE = typeof(TException);
    }
}
