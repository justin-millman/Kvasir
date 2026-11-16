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
            var adminCmd = fixture.PrincipalCommands<TableFingerprint>().CreateTableCommand;

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
            var creationCmd = fixture.PrincipalCommands<TableFingerprint>().CreateTableCommand;
            var insertCmd = fixture.PrincipalCommands<TableFingerprint>().InsertCommand(ANY_ROWS);
            var insertions = fixture.InsertionsFor(insertCmd);
            var projectionTable = fixture.PrincipalTableOf<MapProjection>();
            var projectionCommand = fixture.PrincipalCommands<MapProjection>().CreateTableCommand;
            var designersTable = fixture.RelationTableOf<MapProjection>(0);
            var designersCommand = fixture.RelationCommands<MapProjection>(0).CreateTableCommand;
            var hymnTable = fixture.PrincipalTableOf<Hymn>();
            var hymnCommand = fixture.PrincipalCommands<Hymn>().CreateTableCommand;

            // Assert
            creationCmd.Connection.Should().Be(fixture.Connection);
            creationCmd.Transaction.Should().Be(fixture.Transaction);
            insertCmd.Connection.Should().Be(fixture.Connection);
            insertCmd.Transaction.Should().Be(fixture.Transaction);
            insertions.Should().HaveCount(3);
            insertions.Should().ContainRow(projectionTable.Name.ToString(), TableFingerprint.For(projectionCommand));
            insertions.Should().ContainRow(designersTable.Name.ToString(), TableFingerprint.For(designersCommand));
            insertions.Should().ContainRow(hymnTable.Name.ToString(), TableFingerprint.For(hymnCommand));
            fixture.ShouldBeOrdered(creationCmd, insertCmd);
            fixture.AdminCommits.Should().Be(2);
        }

        [TestMethod] public async Task SchemaManagement_FullyPopulated() {
            // Arrange
            var fixture = new TestFixture(typeof(Amulet), typeof(Outlier), typeof(Shaman));
            var amuletTable = fixture.PrincipalTableOf<Amulet>();
            var amuletCommand = fixture.PrincipalCommands<Amulet>().CreateTableCommand;
            var amuletMetadata = new object[] { amuletTable.Name.ToString(), TableFingerprint.For(amuletCommand) };
            var outlierTable = fixture.PrincipalTableOf<Outlier>();
            var outlierCommand = fixture.PrincipalCommands<Outlier>().CreateTableCommand;
            var outlierMetadata = new object[] { outlierTable.Name.ToString(), TableFingerprint.For(outlierCommand) };
            var shamanTable = fixture.PrincipalTableOf<Shaman>();
            var shamanCommand = fixture.PrincipalCommands<Shaman>().CreateTableCommand;
            var shamanMetadata = new object[] {shamanTable.Name.ToString(), TableFingerprint.For(shamanCommand) };
            fixture = fixture.WithEntityRow<TableFingerprint>(amuletMetadata).WithEntityRow<TableFingerprint>(outlierMetadata).WithEntityRow<TableFingerprint>(shamanMetadata);
            await fixture.InitializeSchema();

            // Act
            var creationCmd = fixture.PrincipalCommands<TableFingerprint>().CreateTableCommand;
            var insertCmd = fixture.PrincipalCommands<TableFingerprint>().InsertCommand(ANY_ROWS);
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
            var servantCommand = fixture.PrincipalCommands<IndenturedServant>().CreateTableCommand;
            var servantMetadata = new object[] { servantTable.Name.ToString(), TableFingerprint.For(servantCommand) };
            fixture = fixture.WithEntityRow<TableFingerprint>(servantMetadata);
            await fixture.InitializeSchema();

            // Act
            var creationCmd = fixture.PrincipalCommands<TableFingerprint>().CreateTableCommand;
            var insertCmd = fixture.PrincipalCommands<TableFingerprint>().InsertCommand(ANY_ROWS);
            var insertions = fixture.InsertionsFor(insertCmd);
            var oatmealTable = fixture.PrincipalTableOf<Oatmeal>();
            var oatmealCommand = fixture.PrincipalCommands<Oatmeal>().CreateTableCommand;

            // Assert
            creationCmd.Connection.Should().Be(fixture.Connection);
            creationCmd.Transaction.Should().Be(fixture.Transaction);
            insertCmd.Connection.Should().Be(fixture.Connection);
            insertCmd.Transaction.Should().Be(fixture.Transaction);
            insertions.Should().HaveCount(1);
            insertions.Should().ContainRow(oatmealTable.Name.ToString(), TableFingerprint.For(oatmealCommand));
            fixture.ShouldBeOrdered(creationCmd, insertCmd);
            fixture.AdminCommits.Should().Be(2);
        }

        [TestMethod] public async Task SchemaManagement_RelationTableUnpopulated() {
            // Arrange
            var fixture = new TestFixture(typeof(LorcanaCharacter));
            var characterTable = fixture.PrincipalTableOf<LorcanaCharacter>();
            var characterCommand = fixture.PrincipalCommands<LorcanaCharacter>().CreateTableCommand;
            var characterMetadata = new object[] { characterTable.Name.ToString(), TableFingerprint.For(characterCommand) };
            fixture = fixture.WithEntityRow<TableFingerprint>(characterMetadata);
            await fixture.InitializeSchema();

            // Act
            var creationCmd = fixture.PrincipalCommands<TableFingerprint>().CreateTableCommand;
            var insertCmd = fixture.PrincipalCommands<TableFingerprint>().InsertCommand(ANY_ROWS);
            var insertions = fixture.InsertionsFor(insertCmd);
            var effectsTable = fixture.RelationTableOf<LorcanaCharacter>(0);
            var effectsCommand = fixture.RelationCommands<LorcanaCharacter>(0).CreateTableCommand;

            // Assert
            creationCmd.Connection.Should().Be(fixture.Connection);
            creationCmd.Transaction.Should().Be(fixture.Transaction);
            insertCmd.Connection.Should().Be(fixture.Connection);
            insertCmd.Transaction.Should().Be(fixture.Transaction);
            insertions.Should().HaveCount(1);
            insertions.Should().ContainRow(effectsTable.Name.ToString(), TableFingerprint.For(effectsCommand));
            fixture.ShouldBeOrdered(creationCmd, insertCmd);
            fixture.AdminCommits.Should().Be(2);
        }

        [TestMethod] public async Task SchemaManagement_PrincipalTableMismatch_IsError() {
            // Arrange
            var fixture = new TestFixture(typeof(GrandRelic));
            var relicTable = fixture.PrincipalTableOf<GrandRelic>();
            var relicMetadata = new object[] { relicTable.Name.ToString(), "xyzabc123" };
            fixture = fixture.WithEntityRow<TableFingerprint>(relicMetadata);

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
            var scientificMetadata = new object[] { scientificTable.Name.ToString(), "ffffffff" };
            fixture = fixture.WithEntityRow<TableFingerprint>(scientificMetadata);

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
            var nonexistentMetadata = new object[] { "NonExistentEntityTable", "0000000000000000000000" };
            fixture = fixture.WithEntityRow<TableFingerprint>(nonexistentMetadata);

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