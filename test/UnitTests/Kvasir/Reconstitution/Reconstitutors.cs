using Atropos.NSubstitute;
using FluentAssertions;
using Kvasir.Reconstitution;
using Kvasir.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;

namespace UT.Kvasir.Reconstitution {
    [TestClass, TestCategory("Reconstitutor")]
    public class ReconstitutorTests {
        [TestMethod] public void Construct() {
            // Arrange
            var mockCreator = Substitute.For<IObjectCreator>();
            mockCreator.Target.Returns(typeof(Exception));
            var mutators = new IMutationStep[] {};

            // Act
            var reconstitutor = new Reconstitutor(mockCreator, mutators);

            // Assert
            reconstitutor.Target.Should().Be(typeof(Exception));
        }

        [TestMethod] public void ExecuteProducesNonNull() {
            // Arrange
            var mockCreator = Substitute.For<IObjectCreator>();
            mockCreator.Target.Returns(typeof(Exception));
            mockCreator.Execute(Arg.Any<IReadOnlyList<DBValue>>()).Returns(new Exception());
            var mockMutator0 = Substitute.For<IMutationStep>();
            mockMutator0.ExpectedSubject.Returns(typeof(Exception));
            mockMutator0.Execute(Arg.Any<object>(), Arg.Any<IReadOnlyList<DBValue>>());
            var mockMutator1 = Substitute.For<IMutationStep>();
            mockMutator1.ExpectedSubject.Returns(typeof(Exception));
            mockMutator1.Execute(Arg.Any<object>(), Arg.Any<IReadOnlyList<DBValue>>());
            var mutators = new IMutationStep[] { mockMutator0, mockMutator1 };
            var reconstitutor = new Reconstitutor(mockCreator, mutators);
            var data = new DBValue[] { DBValue.Create(7), DBValue.Create('='), DBValue.NULL };

            // Act
            var _ = reconstitutor.ReconstituteFrom(data);

            // Assert
            mockCreator.Received().Execute(data);
            mockMutator0.Received().Execute(Arg.Any<Exception>(), data);
            mockMutator1.Received().Execute(Arg.Any<Exception>(), data);
        }

        [TestMethod] public void ExecuteProducesNull() {
            // Arrange
            var mockCreator = Substitute.For<IObjectCreator>();
            mockCreator.Target.Returns(typeof(Exception));
            mockCreator.Execute(Arg.Any<IReadOnlyList<DBValue>>()).Returns(null);
            var mockMutator0 = Substitute.For<IMutationStep>();
            mockMutator0.ExpectedSubject.Returns(typeof(Exception));
            var mockMutator1 = Substitute.For<IMutationStep>();
            mockMutator1.ExpectedSubject.Returns(typeof(Exception));
            var mutators = new IMutationStep[] { mockMutator0, mockMutator1 };
            var reconstitutor = new Reconstitutor(mockCreator, mutators);
            var data = new DBValue[] { DBValue.Create(7), DBValue.Create('='), DBValue.NULL };

            // Act
            var _ = reconstitutor.ReconstituteFrom(data);

            // Assert
            mockCreator.Received().Execute(data);
        }
    }

    [TestClass, TestCategory("ReconstitutorFacade")]
    public class ReconstitutorFacadeTests {
        [TestMethod] public void Construct() {
            // Arrange
            var start = new Index(3);
            var length = 2;
            var mockRecon = Substitute.For<IReconstitutor>();
            mockRecon.Target.Returns(typeof(Activator));

            // Act
            var reconstitutor = new ReconstitutorFacade(mockRecon, start, length);

            // Assert
            reconstitutor.Target.Should().Be(typeof(Activator));
        }

        [TestMethod] public void Execute() {
            // Arrange
            var start = new Index(1);
            var length = 2;
            var mockRecon = Substitute.For<IReconstitutor>();
            mockRecon.Target.Returns(typeof(Lazy<string>));
            mockRecon.ReconstituteFrom(Arg.Any<IReadOnlyList<DBValue>>()).Returns(null);
            var reconstitutor = new ReconstitutorFacade(mockRecon, start, length);
            var data = new DBValue[] { DBValue.Create(1), DBValue.Create(2), DBValue.Create(3), DBValue.Create(4) };

            // Act
            var _ = reconstitutor.ReconstituteFrom(data);

            // Assert
            var view = data[1..3];
            mockRecon.Received().ReconstituteFrom(NArg.IsSameSequence<IReadOnlyList<DBValue>>(view));
        }
    }
}
