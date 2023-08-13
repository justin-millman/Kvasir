using FluentAssertions;
using Kvasir.Schema;
using Kvasir.Transcription;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace UT.Kvasir.Schema {
    [TestClass, TestCategory("CheckConstraint")]
    public class CheckConstraintTests {
        [TestMethod] public void ConstructWithoutName() {
            // Arrange
            var mockClause = Substitute.For<Clause>();

            // Act
            var constraint = new CheckConstraint(mockClause);

            // Assert
            constraint.Name.Should().NotHaveValue();
            constraint.Condition.Should().BeSameAs(mockClause);
            mockClause.ReceivedCalls().Should().HaveCount(0);
        }

        [TestMethod] public void ConstructWithName() {
            // Arrange
            var name = new CheckName("CHECK");
            var mockClause = Substitute.For<Clause>();

            // Act
            var constraint = new CheckConstraint(name, mockClause);

            // Assert
            constraint.Name.Should().HaveValue(name);
            constraint.Condition.Should().BeSameAs(mockClause);
            mockClause.ReceivedCalls().Should().HaveCount(0);
        }

        [TestMethod] public void GenerateDeclarationWithoutName() {
            // Arrange
            var mockClause = Substitute.For<Clause>();
            var constraint = new CheckConstraint(mockClause);
            var mockBuilder = Substitute.For<IConstraintDeclBuilder<SqlSnippet>>();

            // Act
            constraint.GenerateDeclaration(mockBuilder);

            // Assert
            mockClause.Received().AddDeclarationTo(mockBuilder);
            mockBuilder.Received().Build();
            mockClause.ReceivedCalls().Should().HaveCount(1);
            mockBuilder.ReceivedCalls().Should().HaveCount(1);
        }

        [TestMethod] public void GenerateDeclarationWithName() {
            // Arrange
            var name = new CheckName("CHECK");
            var mockClause = Substitute.For<Clause>();
            var constraint = new CheckConstraint(name, mockClause);
            var mockBuilder = Substitute.For<IConstraintDeclBuilder<SqlSnippet>>();

            // Act
            constraint.GenerateDeclaration(mockBuilder);

            // Assert
            mockBuilder.Received().SetName(name);
            mockClause.Received().AddDeclarationTo(mockBuilder);
            mockBuilder.Received().Build();
            mockClause.ReceivedCalls().Should().HaveCount(1);
            mockBuilder.ReceivedCalls().Should().HaveCount(2);
        }
    }
}
