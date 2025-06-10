using FluentAssertions;
using Kvasir.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;

using static UT.Kvasir.Transaction.TableCreation;

namespace UT.Kvasir.Transaction {
    [TestClass, TestCategory("Table Creation")]
    public class TableCreationTests {
        [TestMethod] public void SingleEntityNoRelations() {
            // Arrange
            var fixture = new TestFixture(typeof(PoliticalRally));

            // Act
            fixture.Transactor.CreateTables();
            var rallyCmd = fixture.PrincipalCommands<PoliticalRally>().CreateTableCommand;

            // Assert
            rallyCmd.Connection.Should().Be(fixture.Connection);
            rallyCmd.Transaction.Should().Be(fixture.Transaction);
            fixture.ShouldBeOrdered(rallyCmd);
            fixture.Transaction.Received(1).Commit();
        }

        [TestMethod] public void SingleEntityScalarRelations() {
            // Arrange
            var fixture = new TestFixture(typeof(Yogurt));

            // Act
            fixture.Transactor.CreateTables();
            var yogurtCmd = fixture.PrincipalCommands<Yogurt>().CreateTableCommand;
            var nutritionCmd = fixture.RelationCommands<Yogurt>(0).CreateTableCommand;
            var traitsCmd = fixture.RelationCommands<Yogurt>(1).CreateTableCommand;

            // Assert
            yogurtCmd.Connection.Should().Be(fixture.Connection);
            yogurtCmd.Transaction.Should().Be(fixture.Transaction);
            nutritionCmd.Connection.Should().Be(fixture.Connection);
            nutritionCmd.Transaction.Should().Be(fixture.Transaction);
            traitsCmd.Connection.Should().Be(fixture.Connection);
            traitsCmd.Transaction.Should().Be(fixture.Transaction);
            fixture.ShouldBeOrdered(yogurtCmd, (nutritionCmd, traitsCmd));
            fixture.Transaction.Received(1).Commit();
        }

        [TestMethod] public void MultipleUnrelatedEntities() {
            // Arrange
            var fixture = new TestFixture(typeof(Gondola), typeof(Scooter), typeof(Muffin));

            // Act
            fixture.Transactor.CreateTables();
            var gondolaCmd = fixture.PrincipalCommands<Gondola>().CreateTableCommand;
            var scooterCmd = fixture.PrincipalCommands<Scooter>().CreateTableCommand;
            var muffinCmd = fixture.PrincipalCommands<Muffin>().CreateTableCommand;

            // Assert
            gondolaCmd.Connection.Should().Be(fixture.Connection);
            gondolaCmd.Transaction.Should().Be(fixture.Transaction);
            scooterCmd.Connection.Should().Be(fixture.Connection);
            scooterCmd.Transaction.Should().Be(fixture.Transaction);
            muffinCmd.Connection.Should().Be(fixture.Connection);
            muffinCmd.Transaction.Should().Be(fixture.Transaction);
            fixture.ShouldBeOrdered((gondolaCmd, scooterCmd, muffinCmd));
            fixture.Transaction.Received(1).Commit();
        }

        [TestMethod] public void MultipleEntitiesRelatedByReferenceChain() {
            // Arrange
            var fixture = new TestFixture(typeof(EpicRapBattle), typeof(EpicRapBattle.Rapper), typeof(EpicRapBattle.Actor));

            // Act
            fixture.Transactor.CreateTables();
            var rapBattleCmd = fixture.PrincipalCommands<EpicRapBattle>().CreateTableCommand;
            var rapperCmd = fixture.PrincipalCommands<EpicRapBattle.Rapper>().CreateTableCommand;
            var actorCmd = fixture.PrincipalCommands<EpicRapBattle.Actor>().CreateTableCommand;

            // Assert
            rapBattleCmd.Connection.Should().Be(fixture.Connection);
            rapBattleCmd.Transaction.Should().Be(fixture.Transaction);
            rapperCmd.Connection.Should().Be(fixture.Connection);
            rapperCmd.Transaction.Should().Be(fixture.Transaction);
            actorCmd.Connection.Should().Be(fixture.Connection);
            actorCmd.Transaction.Should().Be(fixture.Transaction);
            fixture.ShouldBeOrdered(actorCmd, rapperCmd, rapBattleCmd);
            fixture.Transaction.Received(1).Commit();
        }

        [TestMethod] public void MultipleEntitiesRelatedByReferenceTree() {
            // Arrange
            var fixture = new TestFixture(typeof(JackOLantern), typeof(Farmer.Farm), typeof(Farmer));

            // Act
            fixture.Transactor.CreateTables();
            var lanternCmd = fixture.PrincipalCommands<JackOLantern>().CreateTableCommand;
            var farmerCmd = fixture.PrincipalCommands<Farmer>().CreateTableCommand;
            var farmCmd = fixture.PrincipalCommands<Farmer.Farm>().CreateTableCommand;

            // Assert
            lanternCmd.Connection.Should().Be(fixture.Connection);
            lanternCmd.Transaction.Should().Be(fixture.Transaction);
            farmerCmd.Connection.Should().Be(fixture.Connection);
            farmCmd.Transaction.Should().Be(fixture.Transaction);
            farmCmd.Connection.Should().Be(fixture.Connection);
            farmCmd.Transaction.Should().Be(fixture.Transaction);
            fixture.ShouldBeOrdered(farmCmd, (lanternCmd, farmerCmd));
            fixture.Transaction.Received(1).Commit();
        }

        [TestMethod] public void MultipleEntitiesRelatedByRelation() {
            // Arrange
            var fixture = new TestFixture(typeof(CashRegister), typeof(CashRegister.Item), typeof(CashRegister.Currency));

            // Act
            fixture.Transactor.CreateTables();
            var registerCmd = fixture.PrincipalCommands<CashRegister>().CreateTableCommand;
            var itemCmd = fixture.PrincipalCommands<CashRegister.Item>().CreateTableCommand;
            var currencyCmd = fixture.PrincipalCommands<CashRegister.Currency>().CreateTableCommand;
            var cashCmd = fixture.RelationCommands<CashRegister>(0).CreateTableCommand;
            var sellablesCmd = fixture.RelationCommands<CashRegister>(1).CreateTableCommand;

            // Assert
            registerCmd.Connection.Should().Be(fixture.Connection);
            registerCmd.Transaction.Should().Be(fixture.Transaction);
            itemCmd.Connection.Should().Be(fixture.Connection);
            itemCmd.Transaction.Should().Be(fixture.Transaction);
            currencyCmd.Connection.Should().Be(fixture.Connection);
            currencyCmd.Transaction.Should().Be(fixture.Transaction);
            cashCmd.Connection.Should().Be(fixture.Connection);
            cashCmd.Transaction.Should().Be(fixture.Transaction);
            sellablesCmd.Connection.Should().Be(fixture.Connection);
            sellablesCmd.Transaction.Should().Be(fixture.Transaction);
            fixture.ShouldBeOrdered((registerCmd, itemCmd), sellablesCmd);
            fixture.ShouldBeOrdered((registerCmd, currencyCmd), cashCmd);
            fixture.Transaction.Received(1).Commit();
        }

        [TestMethod] public void SelfReferentialEntity() {
            // Arrange
            var fixture = new TestFixture(typeof(Matrix));

            // Act
            fixture.Transactor.CreateTables();
            var matrixCmd = fixture.PrincipalCommands<Matrix>().CreateTableCommand;
            var eigenvaluesCmd = fixture.RelationCommands<Matrix>(0).CreateTableCommand;
            var inversesCmd = fixture.RelationCommands<Matrix>(1).CreateTableCommand;

            // Assert
            matrixCmd.Connection.Should().Be(fixture.Connection);
            matrixCmd.Transaction.Should().Be(fixture.Transaction);
            eigenvaluesCmd.Connection.Should().Be(fixture.Connection);
            eigenvaluesCmd.Transaction.Should().Be(fixture.Transaction);
            inversesCmd.Connection.Should().Be(fixture.Connection);
            inversesCmd.Transaction.Should().Be(fixture.Transaction);
            fixture.ShouldBeOrdered(matrixCmd, (eigenvaluesCmd, inversesCmd));
            fixture.Transaction.Received(1).Commit();
        }

        [TestMethod] public void PreDefinedEntity() {
            // Arrange
            var fixture = new TestFixture(typeof(Dashavatara));

            // Act
            fixture.Transactor.CreateTables();
            var createCmd = fixture.PrincipalCommands<Dashavatara>().CreateTableCommand;
            var insertCmd = fixture.PrincipalCommands<Dashavatara>().InsertCommand(Enumerable.Empty<IReadOnlyList<DBValue>>());
            var inserts = fixture.InsertionsFor(insertCmd);

            // Assert
            createCmd.Connection.Should().Be(fixture.Connection);
            createCmd.Transaction.Should().Be(fixture.Transaction);
            insertCmd.Connection.Should().Be(fixture.Connection);
            insertCmd.Transaction.Should().Be(fixture.Transaction);
            inserts.Should().HaveCount(10);
            inserts.Should().ContainRow(Dashavatara.Matsaya.Index, Dashavatara.Matsaya.Name, Dashavatara.Matsaya.Form);
            inserts.Should().ContainRow(Dashavatara.Kurma.Index, Dashavatara.Kurma.Name, Dashavatara.Kurma.Form);
            inserts.Should().ContainRow(Dashavatara.Varaha.Index, Dashavatara.Varaha.Name, Dashavatara.Varaha.Form);
            inserts.Should().ContainRow(Dashavatara.Narasimha.Index, Dashavatara.Narasimha.Name, Dashavatara.Narasimha.Form);
            inserts.Should().ContainRow(Dashavatara.Vamana.Index, Dashavatara.Vamana.Name, Dashavatara.Vamana.Form);
            inserts.Should().ContainRow(Dashavatara.Parashurama.Index, Dashavatara.Parashurama.Name, Dashavatara.Parashurama.Form);
            inserts.Should().ContainRow(Dashavatara.Rama.Index, Dashavatara.Rama.Name, Dashavatara.Rama.Form);
            inserts.Should().ContainRow(Dashavatara.Krishna.Index, Dashavatara.Krishna.Name, Dashavatara.Krishna.Form);
            inserts.Should().ContainRow(Dashavatara.Buddha.Index, Dashavatara.Buddha.Name, Dashavatara.Buddha.Form);
            inserts.Should().ContainRow(Dashavatara.Kalki.Index, Dashavatara.Kalki.Name, Dashavatara.Kalki.Form);
            fixture.ShouldBeOrdered(createCmd, insertCmd);
            fixture.Transaction.Received(2).Commit();
        }

        [TestMethod] public void TransactionRolledBack() {
            // Arrange
            var fixture = new TestFixture(typeof(Bond)).WithCommitError();

            // Act
            var action = () => fixture.Transactor.CreateTables();

            // Assert
            action.Should().ThrowExactly<InvalidOperationException>();
            fixture.Transaction.Received(1).Commit();
            fixture.Transaction.Received(1).Rollback();
        }

        [TestMethod] public void RollbackFails() {
            // Arrange
            var fixture = new TestFixture(typeof(GrilledCheese)).WithRollbackError();

            // Act
            var action = () => fixture.Transactor.CreateTables();

            // Assert
            action.Should().ThrowExactly<AggregateException>();
            fixture.Transaction.Received(1).Commit();
            fixture.Transaction.Received(1).Rollback();
        }
    }
}
