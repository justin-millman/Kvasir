using FluentAssertions;
using Kvasir.Extraction;
using Kvasir.Reconstitution;
using Kvasir.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Collections.Generic;

namespace UT.Kvasir.Reconstitution {
    [TestClass, TestCategory("SetPropertyMutationStep")]
    public class SetPropertyMutationTests {
        [TestMethod] public void Construct() {
            // Arrange
            var prop = typeof(PODClass).GetProperty(nameof(PODClass.String))!;
            var mockArgRecon = Substitute.For<IReconstitutor>();
            mockArgRecon.Target.Returns(typeof(string));

            // Act
            var mutator = new SetPropertyMutationStep(new IdentityExtractor<PODClass>(), prop, mockArgRecon);

            // Assert
            mutator.ExpectedSubject.Should().Be(typeof(PODClass));
        }

        [TestMethod] public void ExecuteOnClass() {
            // Arrange
            var prop = typeof(PODClass).GetProperty(nameof(PODClass.String))!;
            var arg = "Santa Clara";
            var mockArgRecon = Substitute.For<IReconstitutor>();
            mockArgRecon.Target.Returns(typeof(string));
            mockArgRecon.ReconstituteFrom(Arg.Any<IReadOnlyList<DBValue>>()).Returns(arg);
            var data = new DBValue[] { DBValue.NULL, DBValue.Create(0), DBValue.Create('@') };
            var mutator = new SetPropertyMutationStep(new IdentityExtractor<PODClass>(), prop, mockArgRecon);
            object source = new PODClass();

            // Act
            ((PODClass)source).String.Should().BeNull();
            mutator.Execute(source, data);

            // Assert
            ((PODClass)source).String.Should().Be(arg);
        }

        [TestMethod] public void ExecuteOnStruct() {
            // Arrange
            var prop = typeof(PODStruct).GetProperty(nameof(PODStruct.String))!;
            var arg = "Valparaiso";
            var mockArgRecon = Substitute.For<IReconstitutor>();
            mockArgRecon.Target.Returns(typeof(string));
            mockArgRecon.ReconstituteFrom(Arg.Any<IReadOnlyList<DBValue>>()).Returns(arg);
            var data = new DBValue[] { DBValue.NULL, DBValue.Create(0), DBValue.Create('@') };
            var mutator = new SetPropertyMutationStep(new IdentityExtractor<PODStruct>(), prop, mockArgRecon);
            object source = new PODStruct();

            // Act
            ((PODStruct)source).String.Should().BeNull();
            mutator.Execute(source, data);

            // Assert
            ((PODStruct)source).String.Should().Be(arg);
        }

        [TestMethod] public void ExecuteWithNullValue() {
            // Arrange
            var prop = typeof(PODClass).GetProperty(nameof(PODClass.Character))!;
            var mockArgRecon = Substitute.For<IReconstitutor>();
            mockArgRecon.Target.Returns(typeof(char));
            mockArgRecon.ReconstituteFrom(Arg.Any<IReadOnlyList<DBValue>>()).Returns(null);
            var data = new DBValue[] { DBValue.NULL, DBValue.Create(0), DBValue.Create('@') };
            var mutator = new SetPropertyMutationStep(new IdentityExtractor<PODClass>(), prop, mockArgRecon);
            var source = new PODClass() { Character = '.' };

            // Act
            source.Character.Should().NotBeNull();
            mutator.Execute(source, data);

            // Assert
            source.Character.Should().BeNull();
        }
    }


    internal class PODClass {
        public int? Integer { get; set; }
        public char? Character { get; set; }
        public string? String { get; set; }
    }
    internal struct PODStruct {
        public int? Integer { get; set; }
        public char? Character { get; set; }
        public string? String { get; set; }
    }
}
