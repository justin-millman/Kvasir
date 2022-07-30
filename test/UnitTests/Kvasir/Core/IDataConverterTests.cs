using Cybele.Core;
using FluentAssertions;
using Kvasir.Core;
using Kvasir.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace UT.Kvasir.Core {
    [TestClass, TestCategory("IDataConverter")]
    public class IDataConverterTests {
        [TestMethod] public void IDataConverterWithUnsupportedResultType() {
            // Arrange
            var converter = new BadConverter();

            // Act
            Func<DataConverter> action = () => (converter as IDataConverter).ConverterImpl;

            // Assert
            action.Should().ThrowExactly<KvasirException>().WithMessage($"*{typeof(Exception).Name}*");
        }


        private sealed class BadConverter : IDataConverter<int, Exception> {
            public Exception Convert(int source) { throw new NotImplementedException(); }
            public int Revert(Exception result) { throw new NotImplementedException(); }
        }
    }
}
