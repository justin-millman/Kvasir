using FluentAssertions;
using Kvasir.Administration;
using Kvasir.Schema;
using Kvasir.Translation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;

using static UT.Kvasir.Transaction.Administration;

namespace UT.Kvasir.Transaction {
    [TestClass, TestCategory("Administration")]
    public class AdministrationTests {
        [TestMethod] public async Task SchemaManagement_TableCreatedOnConstruction() {
            // Arrange
            var fixture = new TestFixture(typeof(DogWalker));
            await fixture.InitializeSchema();

            // Act
            var adminCmd = fixture.PrincipalCommands<TableHash>().CreateTableCommand;

            // Assert
            adminCmd.Connection.Should().Be(fixture.Connection);
            adminCmd.Transaction.Should().Be(fixture.Transaction);
            fixture.ShouldBeOrdered(adminCmd);
            fixture.AdminCommits.Should().Be(2);
        }

        [TestMethod] public async Task SchemaManagement_Unpopulated() {
            // Arrange
            var fixture = new TestFixture(typeof(MapProjection), typeof(Hymn));
            await fixture.InitializeSchema();

            // Act
            var creationCmd = fixture.PrincipalCommands<TableHash>().CreateTableCommand;
            var insertCmd = fixture.PrincipalCommands<TableHash>().InsertCommand(ANY_ROWS);
            var insertions = fixture.InsertionsFor(insertCmd);
            var projectionTable = fixture.PrincipalTableOf<MapProjection>();
            var designersTable = fixture.RelationTableOf<MapProjection>(0);
            var hymnTable = fixture.PrincipalTableOf<Hymn>();

            // Assert
            creationCmd.Connection.Should().Be(fixture.Connection);
            creationCmd.Transaction.Should().Be(fixture.Transaction);
            insertCmd.Connection.Should().Be(fixture.Connection);
            insertCmd.Transaction.Should().Be(fixture.Transaction);
            insertions.Should().HaveCount(3);
            insertions.Should().ContainRow(projectionTable.Name.ToString(), projectionTable.GetHashCode());
            insertions.Should().ContainRow(designersTable.Name.ToString(), designersTable.GetHashCode());
            insertions.Should().ContainRow(hymnTable.Name.ToString(), hymnTable.GetHashCode());
            fixture.ShouldBeOrdered(creationCmd, insertCmd);
            fixture.AdminCommits.Should().Be(2);
        }

        [TestMethod] public async Task SchemaManagement_FullyPopulated() {
            // Arrange
            var fixture = new TestFixture(typeof(Amulet), typeof(Outlier), typeof(Shaman));
            var amuletTable = fixture.PrincipalTableOf<Amulet>();
            var amuletMetadata = new object[] { amuletTable.Name.ToString(), amuletTable.GetHashCode() };
            var outlierTable = fixture.PrincipalTableOf<Outlier>();
            var outlierMetadata = new object[] { outlierTable.Name.ToString(), outlierTable.GetHashCode() };
            var shamanTable = fixture.PrincipalTableOf<Shaman>();
            var shamanMetadata = new object[] {shamanTable.Name.ToString(), shamanTable.GetHashCode() };
            fixture = fixture.WithEntityRow<TableHash>(amuletMetadata).WithEntityRow<TableHash>(outlierMetadata).WithEntityRow<TableHash>(shamanMetadata);
            await fixture.InitializeSchema();

            // Act
            var creationCmd = fixture.PrincipalCommands<TableHash>().CreateTableCommand;
            var insertCmd = fixture.PrincipalCommands<TableHash>().InsertCommand(ANY_ROWS);
            var insertions = fixture.InsertionsFor(insertCmd);

            // Assert
            creationCmd.Connection.Should().Be(fixture.Connection);
            creationCmd.Transaction.Should().Be(fixture.Transaction);
            insertions.Should().HaveCount(0);
            fixture.ShouldBeOrdered(creationCmd);
            fixture.AdminCommits.Should().Be(1);
        }

        [TestMethod] public async Task SchemaManagement_PrincipalTableUnpopulated() {
            // Arrange
            var fixture = new TestFixture(typeof(IndenturedServant), typeof(Oatmeal));
            var servantTable = fixture.PrincipalTableOf<IndenturedServant>();
            var servantMetadata = new object[] { servantTable.Name.ToString(), servantTable.GetHashCode() };
            fixture = fixture.WithEntityRow<TableHash>(servantMetadata);
            await fixture.InitializeSchema();

            // Act
            var creationCmd = fixture.PrincipalCommands<TableHash>().CreateTableCommand;
            var insertCmd = fixture.PrincipalCommands<TableHash>().InsertCommand(ANY_ROWS);
            var insertions = fixture.InsertionsFor(insertCmd);
            var oatmealTable = fixture.PrincipalTableOf<Oatmeal>();

            // Assert
            creationCmd.Connection.Should().Be(fixture.Connection);
            creationCmd.Transaction.Should().Be(fixture.Transaction);
            insertCmd.Connection.Should().Be(fixture.Connection);
            insertCmd.Transaction.Should().Be(fixture.Transaction);
            insertions.Should().HaveCount(1);
            insertions.Should().ContainRow(oatmealTable.Name.ToString(), oatmealTable.GetHashCode());
            fixture.ShouldBeOrdered(creationCmd, insertCmd);
            fixture.AdminCommits.Should().Be(2);
        }

        [TestMethod] public async Task SchemaManagement_RelationTableUnpopulated() {
            // Arrange
            var fixture = new TestFixture(typeof(LorcanaCharacter));
            var characterTable = fixture.PrincipalTableOf<LorcanaCharacter>();
            var characterMetadata = new object[] { characterTable.Name.ToString(), characterTable.GetHashCode() };
            fixture = fixture.WithEntityRow<TableHash>(characterMetadata);
            await fixture.InitializeSchema();

            // Act
            var creationCmd = fixture.PrincipalCommands<TableHash>().CreateTableCommand;
            var insertCmd = fixture.PrincipalCommands<TableHash>().InsertCommand(ANY_ROWS);
            var insertions = fixture.InsertionsFor(insertCmd);
            var effectsTable = fixture.RelationTableOf<LorcanaCharacter>(0);

            // Assert
            creationCmd.Connection.Should().Be(fixture.Connection);
            creationCmd.Transaction.Should().Be(fixture.Transaction);
            insertCmd.Connection.Should().Be(fixture.Connection);
            insertCmd.Transaction.Should().Be(fixture.Transaction);
            insertions.Should().HaveCount(1);
            insertions.Should().ContainRow(effectsTable.Name.ToString(), effectsTable.GetHashCode());
            fixture.ShouldBeOrdered(creationCmd, insertCmd);
            fixture.AdminCommits.Should().Be(2);
        }

        [TestMethod] public async Task SchemaManagement_PrincipalTableMismatch_IsError() {
            // Arrange
            var fixture = new TestFixture(typeof(GrandRelic));
            var relicTable = fixture.PrincipalTableOf<GrandRelic>();
            var relicMetadata = new object[] { relicTable.Name.ToString(), relicTable.GetHashCode() / 2 };
            fixture = fixture.WithEntityRow<TableHash>(relicMetadata);

            // Act
            var action = async () => await fixture.InitializeSchema();

            // Assert
            await action.Should().ThrowExactlyAsync<SchemaMigrationException>()
                .WithMessage("*\n  • Location: `GrandRelic\n*")
                .WithMessage($"*\n  • Problem: *the schema of table {relicTable.Name} has migrated since it was created in the database*\n*");
        }

        [TestMethod] public async Task SchemaManagement_RelationTableMismatch_IsError() {
            // Arrange
            var fixture = new TestFixture(typeof(Metazooa));
            var scientificTable = fixture.RelationTableOf<Metazooa>(0);
            var scientificMetadata = new object[] { scientificTable.Name.ToString(), scientificTable.GetHashCode() / 2 };
            fixture = fixture.WithEntityRow<TableHash>(scientificMetadata);

            // Act
            var action = async () => await fixture.InitializeSchema();

            // Assert
            await action.Should().ThrowExactlyAsync<SchemaMigrationException>()
                .WithMessage("*\n  • Location: `Metazooa\n*")
                .WithMessage($"*\n  • Problem: *the schema of table {scientificTable.Name} has migrated since it was created in the database*\n*");
        }

        [TestMethod] public async Task SchemaManagement_TableHasBeenDeleted() {
            // Arrange
            var fixture = new TestFixture(typeof(PillPocket));
            var nonexistentMetadata = new object[] { "NonExistentEntityTable", 871725 };
            fixture = fixture.WithEntityRow<TableHash>(nonexistentMetadata);

            // Act
            var action = async () => await fixture.InitializeSchema();

            // Assert
            await action.Should().ThrowExactlyAsync<SchemaMigrationException>()
                .WithMessage("*\n  • Location: <back-end database>\n*")
                .WithMessage($"*\n  • Problem: *the CLR source of table {nonexistentMetadata[0]} no longer exists*\n*");
        }


        private static readonly IEnumerable<IReadOnlyList<DBValue>> ANY_ROWS = [];
    }
}