using FluentAssertions;
using Kvasir.Reconstitution;
using Kvasir.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;

namespace UT.Kvasir.Reconstitution {
    [TestClass, TestCategory("DataReconstitutionPlan")]
    public class DataReconstitutionPlanTets {
        [TestMethod] public void NoMutations() {
            // Arrange
            var str = "Antalya";
            var creator = Substitute.For<ICreator>();
            creator.ResultType.Returns(typeof(string));
            creator.CreateFrom(Arg.Any<IReadOnlyList<DBValue>>()).Returns(str);

            // Act
            var row = new DBValue[] { DBValue.Create(18124.0f), DBValue.Create(false), DBValue.NULL };
            var mutators = Array.Empty<IMutator>();
            var plan = new DataReconstitutionPlan(new ReconstitutingCreator(creator, mutators));
            var value = plan.ReconstituteFrom(row);

            // Assert
            plan.ResultType.Should().Be(creator.ResultType);
            value.Should().Be(str);
        }

        [TestMethod] public void OneMutation() {
            // Arrange
            var exception = new FieldAccessException();
            var creator = Substitute.For<ICreator>();
            creator.ResultType.Returns(typeof(FieldAccessException));
            creator.CreateFrom(Arg.Any<IReadOnlyList<DBValue>>()).Returns(exception);
            var mutator = Substitute.For<IMutator>();
            mutator.SourceType.Returns(typeof(FieldAccessException));

            // Act
            var row = new DBValue[] { DBValue.Create(long.MinValue), DBValue.Create(DateTime.Now) };
            var mutators = new IMutator[] { mutator };
            var plan = new DataReconstitutionPlan(new ReconstitutingCreator(creator, mutators));
            var value = plan.ReconstituteFrom(row);

            // Assert
            plan.ResultType.Should().Be(creator.ResultType);
            value.Should().Be(exception);
            mutator.Received().Mutate(exception, row);
        }

        [TestMethod] public void MultipleMutations() {
            // Arrange
            var dbnull = DBNull.Value;
            var creator = Substitute.For<ICreator>();
            creator.ResultType.Returns(typeof(DBNull));
            creator.CreateFrom(Arg.Any<IReadOnlyList<DBValue>>()).Returns(dbnull);
            var mutator0 = Substitute.For<IMutator>();
            mutator0.SourceType.Returns(typeof(DBNull));
            var mutator1 = Substitute.For<IMutator>();
            mutator1.SourceType.Returns(typeof(DBNull));
            var mutator2 = Substitute.For<IMutator>();
            mutator2.SourceType.Returns(typeof(DBNull));
            var mutator3 = Substitute.For<IMutator>();
            mutator3.SourceType.Returns(typeof(DBNull));
            var mutator4 = Substitute.For<IMutator>();
            mutator4.SourceType.Returns(typeof(DBNull));

            // Act
            var row = new DBValue[] { DBValue.Create(new Guid()) };
            var mutators = new IMutator[] { mutator0, mutator1, mutator2, mutator3, mutator4 };
            var plan = new DataReconstitutionPlan(new ReconstitutingCreator(creator, mutators));
            var value = plan.ReconstituteFrom(row);

            // Assert
            plan.ResultType.Should().Be(creator.ResultType);
            value.Should().Be(dbnull);
            Received.InOrder(() => {
                mutator0.Mutate(value, row);
                mutator1.Mutate(value, row);
                mutator2.Mutate(value, row);
                mutator3.Mutate(value, row);
                mutator4.Mutate(value, row);
            });
        }
    }
}
