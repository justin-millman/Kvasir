using FluentAssertions;
using Kvasir.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;

using static UT.Kvasir.Transaction.TableCreation;
using static UT.Kvasir.Translation.TestLocalizations;

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

        [TestMethod] public void SingleLocalization() {
            // Arrange
            var fixture = new TestFixture(typeof(Nickname));

            // Act
            fixture.Transactor.CreateTables();
            var nicknameCmd = fixture.PrincipalCommands<Nickname>().CreateTableCommand;

            // Assert
            nicknameCmd.Connection.Should().Be(fixture.Connection);
            nicknameCmd.Transaction.Should().Be(fixture.Transaction);
            fixture.ShouldBeOrdered(nicknameCmd);
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

        [TestMethod] public void MultipleUnrelatedLocalizations() {
            // Arrange
            var fixture = new TestFixture(typeof(Dosage), typeof(GUID));

            // Act
            fixture.Transactor.CreateTables();
            var dosageCmd = fixture.PrincipalCommands<Dosage>().CreateTableCommand;
            var guidCmd = fixture.PrincipalCommands<GUID>().CreateTableCommand;

            // Assert
            dosageCmd.Connection.Should().Be(fixture.Connection);
            dosageCmd.Transaction.Should().Be(fixture.Transaction);
            guidCmd.Connection.Should().Be(fixture.Connection);
            guidCmd.Transaction.Should().Be(fixture.Transaction);
            fixture.ShouldBeOrdered((dosageCmd, guidCmd));
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

        [TestMethod] public void MultipleEntitiesRelatedByScalarLocalization() {
            // Arrange
            var fixture = new TestFixture(typeof(Quesadilla), typeof(LocalizedMeasure));

            // Act
            fixture.Transactor.CreateTables();
            var quesadillaCmd = fixture.PrincipalCommands<Quesadilla>().CreateTableCommand;
            var measureCmd = fixture.PrincipalCommands<LocalizedMeasure>().CreateTableCommand;

            // Assert
            quesadillaCmd.Connection.Should().Be(fixture.Connection);
            quesadillaCmd.Transaction.Should().Be(fixture.Transaction);
            measureCmd.Connection.Should().Be(fixture.Connection);
            measureCmd.Transaction.Should().Be(fixture.Transaction);
            fixture.ShouldBeOrdered((quesadillaCmd, measureCmd));
            fixture.Transaction.Received(1).Commit();
        }

        [TestMethod] public void MultipleEntitiesRelatedByReferenceLocalization() {
            // Arrange
            var fixture = new TestFixture(typeof(Honmoon), typeof(Honmoon.DemonHunter), typeof(Honmoon.LocalizedHunter));

            // Act
            fixture.Transactor.CreateTables();
            var honmoonCmd = fixture.PrincipalCommands<Honmoon>().CreateTableCommand;
            var hunterCmd = fixture.PrincipalCommands<Honmoon.DemonHunter>().CreateTableCommand;
            var localizationCmd = fixture.PrincipalCommands<Honmoon.LocalizedHunter>().CreateTableCommand;

            // Assert
            honmoonCmd.Connection.Should().Be(fixture.Connection);
            honmoonCmd.Transaction.Should().Be(fixture.Transaction);
            hunterCmd.Connection.Should().Be(fixture.Connection);
            hunterCmd.Transaction.Should().Be(fixture.Transaction);
            localizationCmd.Connection.Should().Be(fixture.Connection);
            localizationCmd.Transaction.Should().Be(fixture.Transaction);
            fixture.ShouldBeOrdered((honmoonCmd, hunterCmd));
            fixture.ShouldBeOrdered((honmoonCmd, localizationCmd));
            fixture.ShouldBeOrdered(hunterCmd, localizationCmd);
        }

        [TestMethod] public void MultipleEntitiesRelatedByRelationLocalization() {
            // Arrange
            var fixture = new TestFixture(typeof(LocalizedText), typeof(CrownJewel));

            // Act
            fixture.Transactor.CreateTables();
            var textCmd = fixture.PrincipalCommands<LocalizedText>().CreateTableCommand;
            var jewelCmd = fixture.PrincipalCommands<CrownJewel>().CreateTableCommand;
            var componentsCmd = fixture.RelationCommands<CrownJewel>(0).CreateTableCommand;

            // Assert
            textCmd.Connection.Should().Be(fixture.Connection);
            textCmd.Transaction.Should().Be(fixture.Transaction);
            jewelCmd.Connection.Should().Be(fixture.Connection);
            jewelCmd.Transaction.Should().Be(fixture.Transaction);
            componentsCmd.Connection.Should().Be(fixture.Connection);
            componentsCmd.Transaction.Should().Be(fixture.Transaction);
            fixture.ShouldBeOrdered(jewelCmd, componentsCmd, textCmd);
            fixture.Transaction.Received(1).Commit();
        }

        [TestMethod] public void SelfReferentialEntityViaRelation() {
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

        [TestMethod] public void SelfReferentialEntityViaLocalization() {
            // Arrange
            var fixture = new TestFixture(typeof(ClassActionLawsuit), typeof(LocalizedDate), typeof(ClassActionLawsuit.LocalizedVerdict));

            // Act
            fixture.Transactor.CreateTables();
            var lawsuitCmd = fixture.PrincipalCommands<ClassActionLawsuit>().CreateTableCommand;
            var dateCmd = fixture.PrincipalCommands<LocalizedDate>().CreateTableCommand;
            var verdictCmd = fixture.PrincipalCommands<ClassActionLawsuit.LocalizedVerdict>().CreateTableCommand;

            // Assert
            lawsuitCmd.Connection.Should().Be(fixture.Connection);
            lawsuitCmd.Transaction.Should().Be(fixture.Transaction);
            dateCmd.Connection.Should().Be(fixture.Connection);
            dateCmd.Transaction.Should().Be(fixture.Transaction);
            verdictCmd.Connection.Should().Be(fixture.Connection);
            verdictCmd.Transaction.Should().Be(fixture.Transaction);
            fixture.ShouldBeOrdered(lawsuitCmd, verdictCmd);
            fixture.ShouldBeOrdered((lawsuitCmd, dateCmd));
            fixture.ShouldBeOrdered((verdictCmd, dateCmd));
        }

        [TestMethod] public void PreDefinedEntity() {
            // Arrange
            var fixture = new TestFixture(typeof(Dashavatara));

            // Act
            fixture.Transactor.CreateTables();
            var createCmd = fixture.PrincipalCommands<Dashavatara>().CreateTableCommand;
            var insertCmd = fixture.PrincipalCommands<Dashavatara>().InsertCommand([]);
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

        [TestMethod] public void PreDefinedLocalization() {
            // Arrange
            var fixture = new TestFixture(typeof(CivVITerrain));

            // Act
            fixture.Transactor.CreateTables();
            var createCmd = fixture.PrincipalCommands<CivVITerrain>().CreateTableCommand;
            var insertCmd = fixture.PrincipalCommands<CivVITerrain>().InsertCommand([]);
            var inserts = fixture.InsertionsFor(insertCmd);

            // Assert
            createCmd.Connection.Should().Be(fixture.Connection);
            createCmd.Transaction.Should().Be(fixture.Transaction);
            insertCmd.Connection.Should().Be(fixture.Connection);
            insertCmd.Transaction.Should().Be(fixture.Transaction);
            inserts.Should().HaveCount(16);
            inserts.Should().ContainRow(CivVITerrain.Plains.Key, "FULL", CivVITerrain.Plains["FULL"]);
            inserts.Should().ContainRow(CivVITerrain.Plains.Key, "SHORT", CivVITerrain.Plains["SHORT"]);
            inserts.Should().ContainRow(CivVITerrain.Grassland.Key, "FULL", CivVITerrain.Grassland["FULL"]);
            inserts.Should().ContainRow(CivVITerrain.Grassland.Key, "SHORT", CivVITerrain.Grassland["SHORT"]);
            inserts.Should().ContainRow(CivVITerrain.Desert.Key, "FULL", CivVITerrain.Desert["FULL"]);
            inserts.Should().ContainRow(CivVITerrain.Desert.Key, "SHORT", CivVITerrain.Desert["SHORT"]);
            inserts.Should().ContainRow(CivVITerrain.Tundra.Key, "FULL", CivVITerrain.Tundra["FULL"]);
            inserts.Should().ContainRow(CivVITerrain.Tundra.Key, "SHORT", CivVITerrain.Tundra["SHORT"]);
            inserts.Should().ContainRow(CivVITerrain.Snow.Key, "FULL", CivVITerrain.Snow["FULL"]);
            inserts.Should().ContainRow(CivVITerrain.Snow.Key, "SHORT", CivVITerrain.Snow["SHORT"]);
            inserts.Should().ContainRow(CivVITerrain.Mountain.Key, "FULL", CivVITerrain.Mountain["FULL"]);
            inserts.Should().ContainRow(CivVITerrain.Mountain.Key, "SHORT", CivVITerrain.Mountain["SHORT"]);
            inserts.Should().ContainRow(CivVITerrain.Coast.Key, "FULL", CivVITerrain.Coast["FULL"]);
            inserts.Should().ContainRow(CivVITerrain.Coast.Key, "SHORT", CivVITerrain.Coast["SHORT"]);
            inserts.Should().ContainRow(CivVITerrain.Ocean.Key, "FULL", CivVITerrain.Ocean["FULL"]);
            inserts.Should().ContainRow(CivVITerrain.Ocean.Key, "SHORT", CivVITerrain.Ocean["SHORT"]);
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
