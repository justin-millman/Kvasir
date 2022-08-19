using Atropos.Moq;
using Cybele.Core;
using FluentAssertions;
using Kvasir.Reconstitution;
using Kvasir.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UT.Kvasir.Reconstitution {
    [TestClass, TestCategory("DataReconstitutionPlan")]
    public class DataReconstitutionPlanTests {
        [TestMethod] public void Construction() {
            // Arrange
            var mockReconstitutor = new Mock<IReconstitutor>();
            mockReconstitutor.Setup(r => r.Target).Returns(typeof(string));
            mockReconstitutor.Setup(r => r.ReconstituteFrom(It.IsAny<IReadOnlyList<DBValue>>())).Returns("");
            var reverter = DataConverter.Identity<int>();

            // Act
            var reverters = new DataConverter[] { reverter };
            var plan = new DataReconstitutionPlan(mockReconstitutor.Object, reverters);

            // Assert
            plan.Target.Should().Be(typeof(string));
        }

        [TestMethod] public void Execute() {
            // Arrange
            var year = DBValue.Create(2011);
            var month = DBValue.Create(11);
            var day = DBValue.Create(30);
            var values = new DBValue[] { year, month, day };

            var offsetCnv = DataConverter.Create<int, int>(i => i - 1, i => i + 1);
            var reverters = Enumerable.Repeat(offsetCnv, 3);

            var reconstitutor = new Mock<IReconstitutor>();
            reconstitutor.Setup(r => r.Target).Returns(typeof(DateTime));
            reconstitutor.Setup(r => r.ReconstituteFrom(It.IsAny<IReadOnlyList<DBValue>>())).Returns(new DateTime());

            var plan = new DataReconstitutionPlan(reconstitutor.Object, reverters);

            // Act
            _ = plan.ReconstituteFrom(values);

            // Assert
            var expYear = DBValue.Create(2012);
            var expMonth = DBValue.Create(12);
            var expDay = DBValue.Create(31);
            var expValues = new DBValue[] { expYear, expMonth, expDay };
            reconstitutor.Verify(r => r.ReconstituteFrom(Arg.IsSameSequence<IReadOnlyList<DBValue>>(expValues)));
        }
    }

    [TestClass, TestCategory("RelationReconstitutionPlan")]
    public class RelationReconstitutionPlanTests {
        [TestMethod] public void Construct() {
            // Arrange
            var converter = DataConverter.Identity<string>();
            var creator = new PrimitiveCreator(0, typeof(string));
            var reconstitutor = new Reconstitutor(creator, Enumerable.Empty<IMutationStep>());
            var entityPlan = new DataReconstitutionPlan(reconstitutor, Enumerable.Repeat(converter, 1));

            var mockRepopulator = new Mock<IRepopulator>();
            mockRepopulator.Setup(r => r.ExpectedSubject).Returns(typeof(object));

            // Act
            var plan = new RelationReconstitutionPlan(entityPlan, mockRepopulator.Object);

            // Assert
            plan.ExpectedSubject.Should().Be(typeof(object));
        }

        [TestMethod] public void ExecuteOnEmptyRows() {
            // Arrange
            var converter = DataConverter.Identity<string>();
            var creator = new PrimitiveCreator(0, typeof(string));
            var reconstitutor = new Reconstitutor(creator, Enumerable.Empty<IMutationStep>());
            var entityPlan = new DataReconstitutionPlan(reconstitutor, Enumerable.Repeat(converter, 1));

            var mockRepopulator = new Mock<IRepopulator>();
            mockRepopulator.Setup(r => r.ExpectedSubject).Returns(typeof(object));
            var plan = new RelationReconstitutionPlan(entityPlan, mockRepopulator.Object);
            var source = new object();
            var data = new List<List<DBValue>>();

            // Act
            plan.RepopulateFrom(source, data);

            // Assert
            mockRepopulator.Verify(r => r.Execute(It.IsAny<object>(), It.IsAny<IEnumerable<object>>()), Times.Never);
        }

        [TestMethod] public void ExecuteOnSingleRow() {
            // Arrange
            var converter = DataConverter.Identity<string>();
            var creator = new PrimitiveCreator(0, typeof(string));
            var reconstitutor = new Reconstitutor(creator, Enumerable.Empty<IMutationStep>());
            var entityPlan = new DataReconstitutionPlan(reconstitutor, Enumerable.Repeat(converter, 1));

            var mockRepopulator = new Mock<IRepopulator>();
            mockRepopulator.Setup(r => r.ExpectedSubject).Returns(typeof(object));
            var plan = new RelationReconstitutionPlan(entityPlan, mockRepopulator.Object);
            var source = new object();

            var values = new string[] { "Davidson" };
            var data = new List<List<DBValue>>() {
                new List<DBValue>(){ DBValue.Create(values[0]) }
            };

            // Act
            plan.RepopulateFrom(source, data);

            // Assert
            mockRepopulator.Verify(r => r.Execute(source, values));
        }

        [TestMethod] public void ExecuteOnMultipleRows() {
            // Arrange
            var converter = DataConverter.Identity<string>();
            var creator = new PrimitiveCreator(0, typeof(string));
            var reconstitutor = new Reconstitutor(creator, Enumerable.Empty<IMutationStep>());
            var entityPlan = new DataReconstitutionPlan(reconstitutor, Enumerable.Repeat(converter, 1));

            var mockRepopulator = new Mock<IRepopulator>();
            mockRepopulator.Setup(r => r.ExpectedSubject).Returns(typeof(object));
            var plan = new RelationReconstitutionPlan(entityPlan, mockRepopulator.Object);
            var source = new object();

            var values = new string[] { "Fairfax", "Highland Park", "Nacogdoches", "Taos Pubelo", "Hilo", "Detroit" };
            var data = new List<List<DBValue>>() {
                new List<DBValue>(){ DBValue.Create(values[0]) },
                new List<DBValue>(){ DBValue.Create(values[1]) },
                new List<DBValue>(){ DBValue.Create(values[2]) },
                new List<DBValue>(){ DBValue.Create(values[3]) },
                new List<DBValue>(){ DBValue.Create(values[4]) },
                new List<DBValue>(){ DBValue.Create(values[5]) }
            };

            // Act
            plan.RepopulateFrom(source, data);

            // Assert
            mockRepopulator.Verify(r => r.Execute(source, values));
        }
    }
}
