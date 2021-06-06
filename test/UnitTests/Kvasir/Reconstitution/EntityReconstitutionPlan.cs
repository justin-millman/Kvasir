using Atropos.Moq;
using FluentAssertions;
using Kvasir.Core;
using Kvasir.Reconstitution;
using Kvasir.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UT.Kvasir.Reconstitution {
    [TestClass, TestCategory("EntityReconstitutionPlan")]
    public class EntityReconstitutionPlanTests {
        [TestMethod] public void Construction() {
            // Arrange
            var mockReconstitutor = new Mock<IReconstitutor>();
            mockReconstitutor.Setup(r => r.Target).Returns(typeof(string));
            mockReconstitutor.Setup(r => r.ReconstituteFrom(It.IsAny<IReadOnlyList<DBValue>>())).Returns("");
            var reverter = DataConverter.Identity<int>();

            // Act
            var reverters = new DataConverter[] { reverter };
            var plan = new EntityReconstitutionPlan(mockReconstitutor.Object, reverters);

            // Assert
            plan.Target.Should().Be(typeof(string));
        }

        [TestMethod]
        public void Execute() {
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

            var plan = new EntityReconstitutionPlan(reconstitutor.Object, reverters);

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
}
