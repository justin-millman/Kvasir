using FluentAssertions;
using Kvasir.Schema;
using Kvasir.Transcription;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace UT.Kvasir.Schema {
    [TestClass, TestCategory("CheckConstraint")]
    public class CheckConstraintTests {
        [TestMethod] public void ConstructWithoutName() {
            // Arrange
            var mockClause = new Mock<Clause>();

            // Act
            var constraint = new CheckConstraint(mockClause.Object);

            // Assert
            constraint.Name.Should().NotHaveValue();
            constraint.Condition.Should().BeSameAs(mockClause.Object);
            mockClause.VerifyNoOtherCalls();
        }

        [TestMethod] public void ConstructWithName() {
            // Arrange
            var name = new CheckName("CHECK");
            var mockClause = new Mock<Clause>();

            // Act
            var constraint = new CheckConstraint(name, mockClause.Object);

            // Assert
            constraint.Name.Should().HaveValue(name);
            constraint.Condition.Should().BeSameAs(mockClause.Object);
            mockClause.VerifyNoOtherCalls();
        }

        [TestMethod] public void GenerateDeclarationWithoutName() {
            // Arrange
            var mockClause = new Mock<Clause>();
            var constraint = new CheckConstraint(mockClause.Object);
            var mockBuilder = new Mock<IConstraintDeclBuilder>();

            // Act
            constraint.GenerateDeclaration(mockBuilder.Object);

            // Assert
            mockClause.Verify(clause => clause.AddDeclarationTo(mockBuilder.Object));
            mockBuilder.Verify(builder => builder.Build());
            mockClause.VerifyNoOtherCalls();
            mockBuilder.VerifyNoOtherCalls();
        }

        [TestMethod] public void GenerateDeclarationWithName() {
            // Arrange
            var name = new CheckName("CHECK");
            var mockClause = new Mock<Clause>();
            var constraint = new CheckConstraint(name, mockClause.Object);
            var mockBuilder = new Mock<IConstraintDeclBuilder>();

            // Act
            constraint.GenerateDeclaration(mockBuilder.Object);

            // Assert
            mockBuilder.Verify(builder => builder.SetName(name));
            mockClause.Verify(clause => clause.AddDeclarationTo(mockBuilder.Object));
            mockBuilder.Verify(builder => builder.Build());
            mockClause.VerifyNoOtherCalls();
            mockBuilder.VerifyNoOtherCalls();
        }
    }
}
