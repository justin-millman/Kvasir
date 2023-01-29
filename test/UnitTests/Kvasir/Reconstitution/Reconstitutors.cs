using Atropos.Moq;
using FluentAssertions;
using Kvasir.Reconstitution;
using Kvasir.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;

namespace UT.Kvasir.Reconstitution {
    [TestClass, TestCategory("Reconstitutor")]
    public class ReconstitutorTests {
        [TestMethod] public void Construct() {
            // Arrange
            var mockCreator = new Mock<IObjectCreator>();
            mockCreator.Setup(c => c.Target).Returns(typeof(Exception));
            var mutators = new IMutationStep[] {};

            // Act
            var reconstitutor = new Reconstitutor(mockCreator.Object, mutators);

            // Assert
            reconstitutor.Target.Should().Be(typeof(Exception));
        }

        [TestMethod] public void ExecuteProducesNonNull() {
            // Arrange
            var mockCreator = new Mock<IObjectCreator>();
            mockCreator.Setup(c => c.Target).Returns(typeof(Exception));
            mockCreator.Setup(c => c.Execute(It.IsAny<IReadOnlyList<DBValue>>())).Returns(new Exception());
            var mockMutator0 = new Mock<IMutationStep>();
            mockMutator0.Setup(m => m.ExpectedSubject).Returns(typeof(Exception));
            mockMutator0.Setup(m => m.Execute(It.IsAny<object>(), It.IsAny<IReadOnlyList<DBValue>>()));
            var mockMutator1 = new Mock<IMutationStep>();
            mockMutator1.Setup(m => m.ExpectedSubject).Returns(typeof(Exception));
            mockMutator1.Setup(m => m.Execute(It.IsAny<object>(), It.IsAny<IReadOnlyList<DBValue>>()));
            var mutators = new IMutationStep[] { mockMutator0.Object, mockMutator1.Object };
            var reconstitutor = new Reconstitutor(mockCreator.Object, mutators);
            var data = new DBValue[] { DBValue.Create(7), DBValue.Create('='), DBValue.NULL };

            // Act
            var _ = reconstitutor.ReconstituteFrom(data);

            // Assert
            mockCreator.Verify(c => c.Execute(data));
            mockMutator0.Verify(c => c.Execute(It.IsAny<Exception>(), data));
            mockMutator1.Verify(c => c.Execute(It.IsAny<Exception>(), data));
        }

        [TestMethod] public void ExecuteProducesNull() {
            // Arrange
            var mockCreator = new Mock<IObjectCreator>();
            mockCreator.Setup(c => c.Target).Returns(typeof(Exception));
            mockCreator.Setup(c => c.Execute(It.IsAny<IReadOnlyList<DBValue>>())).Returns(null);
            var mockMutator0 = new Mock<IMutationStep>();
            mockMutator0.Setup(m => m.ExpectedSubject).Returns(typeof(Exception));
            var mockMutator1 = new Mock<IMutationStep>();
            mockMutator1.Setup(m => m.ExpectedSubject).Returns(typeof(Exception));
            var mutators = new IMutationStep[] { mockMutator0.Object, mockMutator1.Object };
            var reconstitutor = new Reconstitutor(mockCreator.Object, mutators);
            var data = new DBValue[] { DBValue.Create(7), DBValue.Create('='), DBValue.NULL };

            // Act
            var _ = reconstitutor.ReconstituteFrom(data);

            // Assert
            mockCreator.Verify(c => c.Execute(data));
        }
    }

    [TestClass, TestCategory("ReconstitutorFacade")]
    public class ReconstitutorFacadeTests {
        [TestMethod] public void Construct() {
            // Arrange
            var start = new Index(3);
            var length = 2;
            var mockRecon = new Mock<IReconstitutor>();
            mockRecon.Setup(r => r.Target).Returns(typeof(Activator));

            // Act
            var reconstitutor = new ReconstitutorFacade(mockRecon.Object, start, length);

            // Assert
            reconstitutor.Target.Should().Be(typeof(Activator));
        }

        [TestMethod] public void Execute() {
            // Arrange
            var start = new Index(1);
            var length = 2;
            var mockRecon = new Mock<IReconstitutor>();
            mockRecon.Setup(r => r.Target).Returns(typeof(Lazy<string>));
            mockRecon.Setup(r => r.ReconstituteFrom(It.IsAny<IReadOnlyList<DBValue>>())).Returns(null);
            var reconstitutor = new ReconstitutorFacade(mockRecon.Object, start, length);
            var data = new DBValue[] { DBValue.Create(1), DBValue.Create(2), DBValue.Create(3), DBValue.Create(4) };

            // Act
            var _ = reconstitutor.ReconstituteFrom(data);

            // Assert
            var view = data[1..3];
            mockRecon.Verify(r => r.ReconstituteFrom(Arg.IsSameSequence<IReadOnlyList<DBValue>>(view)));
        }
    }
}
