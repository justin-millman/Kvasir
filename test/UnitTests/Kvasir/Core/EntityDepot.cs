using FluentAssertions;
using Kvasir.Core;
using Kvasir.Transcription;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UT.Kvasir.Core {
    [TestClass, TestCategory("EntityDepot")]
    public class EntityDepotTests {
        [TestMethod] public void AddEntityFirstOfType() {
            // Arrange
            var depot = new EntityDepot();
            var entity = "Garrison Oaks";

            // Act
            depot.StoreEntity(entity);
            var untyped = depot[typeof(string)];
            var typed = depot.GetEntities<string>();

            // Assert
            untyped.Should().HaveCount(1);
            typed.Should().HaveCount(1);
            untyped.Should().Contain(entity);
            typed.Should().Contain(entity);
        }

        [TestMethod] public void AddEntityRepeatOfType() {
            // Arrange
            var depot = new EntityDepot();
            var entity1 = "T'Telir";
            var entity2 = "Basin City";

            // Act
            depot.StoreEntity(entity1);
            depot.StoreEntity(entity2);
            var untyped = depot[typeof(string)];
            var typed = depot.GetEntities<string>();

            // Assert
            untyped.Should().HaveCount(2);
            typed.Should().HaveCount(2);
            untyped.Should().Contain(entity1);
            untyped.Should().Contain(entity2);
            typed.Should().Contain(entity1);
            typed.Should().Contain(entity2);
        }

        [TestMethod] public void AddSameEntityTwice() {
            // Arrange
            var depot = new EntityDepot();
            var entity = "Rarohenga";

            // Act
            depot.StoreEntity(entity);
            depot.StoreEntity(entity);
            var untyped = depot[typeof(string)];
            var typed = depot.GetEntities<string>();

            // Assert
            untyped.Should().HaveCount(1);
            typed.Should().HaveCount(1);
            untyped.Should().Contain(entity);
            typed.Should().Contain(entity);
        }

        [TestMethod] public void AddEqualEntities() {
            // Arrange
            var depot = new EntityDepot();
            var entity1 = new SqlSnippet("Genoa City");
            var entity2 = new SqlSnippet("Genoa City");

            // Act
            depot.StoreEntity(entity1);
            depot.StoreEntity(entity2);
            var untyped = depot[typeof(SqlSnippet)];
            var typed = depot.GetEntities<SqlSnippet>();

            // Assert
            untyped.Should().HaveCount(2);
            typed.Should().HaveCount(2);
            untyped.Should().Contain(entity1);
            untyped.Should().Contain(entity2);
            typed.Should().Contain(entity1);
            typed.Should().Contain(entity2);
        }

        [TestMethod] public void LookupEntityTypeNoResults() {
            // Arrange
            var depot = new EntityDepot();

            // Act
            var untyped = depot[typeof(string)];
            var typed = depot.GetEntities<string>();

            // Assert
            untyped.Should().BeEmpty();
            typed.Should().BeEmpty();
        }
    }
}
