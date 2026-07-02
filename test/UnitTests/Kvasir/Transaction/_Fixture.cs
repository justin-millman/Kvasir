using Cybele.Extensions;
using FluentAssertions;
using FluentAssertions.Execution;
using Kvasir.Administration;
using Kvasir.Schema;
using Kvasir.Transaction;
using Kvasir.Translation;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NSubstitute.Core;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using Rows = System.Collections.Generic.IEnumerable<System.Collections.Generic.IReadOnlyList<Kvasir.Schema.DBValue>>;

namespace UT.Kvasir.Transaction {
    internal sealed class TestFixture {
        public IReadOnlyDictionary<ITable, ICommands> Commands => commands_;
        public DbConnection Connection { get; } = Substitute.For<DbConnection>();
        public DbTransaction Transaction { get; }
        public Dictionary<Type, List<object>> Depot { get; }
        public Transactor Transactor {
            get {
                return transactor_;
            }
        }

        public int AdminCommits { get; private set; }

        public TestFixture(params Type[] types) {
            commands_ = [];
            dbRows_ = [];
            connectionPool_ = Substitute.For<IConnectionPool>();
            commandsFactory_ = Substitute.For<ICommandsFactory>();
            Connection = Substitute.For<DbConnection>();
            Transaction = Substitute.For<DbTransaction>();
            Depot = types.ToDictionary(t => t, _ => new List<object>());
            translator_ = new Translator(t => Depot[t], NullLogger.Instance);
            ordering_ = [];
            invocationArgs_ = [];

            withCommitError_ = false;
            withRollbackError_ = false;

            connectionPool_.MakeConnectionAsync().Returns(Connection);
            Connection.BeginTransactionAsync().Returns(Transaction);

            var adminTypes = new Type[] { typeof(TableHash) };

            var entityTranslations = types.Where(t => !Translator.IsLocalizationType(t)).Select(t => translator_[t]);
            var localizationTranslations = types.Where(t => Translator.IsLocalizationType(t)).Select(t => translator_[t, Translator.AsLocalzation]);
            var adminTranslations = adminTypes.Select(t => translator_[t]);
            
            var tables = entityTranslations.SelectMany(x => x.Relations.Select(r => r.Table).Append(x.Principal.Table));
            tables = tables.Concat(localizationTranslations.Select(x => x.Principal.Table));
            tables = tables.Concat(adminTranslations.Select(x => x.Principal.Table));

            foreach (var table in tables) {
                var commands = Substitute.For<ICommands>();

                var createTable = Substitute.For<DbCommand>();
                createTable.CommandText = $"CREATE TABLE {table.Name}";
                createTable.When(c => c.ExecuteNonQueryAsync()).Do(_ => ordering_[createTable] = ordering_.Count + 1);
                commands.CreateTableCommand.Returns(createTable);

                var reader = Substitute.For<DbDataReader>();
                reader.Read().Returns(_ => dbRows_[table].MoveNext());
                reader.FieldCount.Returns(_ => dbRows_[table].Current.Count);
                reader[Arg.Any<int>()].Returns(args => dbRows_[table].Current[args.ArgAt<int>(0)]);
                reader.ClearReceivedCalls();

                var selectAll = Substitute.For<DbCommand>();
                selectAll.CommandText = $"SELECT * FROM {table.Name}";
                selectAll.ExecuteReaderAsync().Returns(reader);
                selectAll.When(c => c.ExecuteReaderAsync()).Do(_ => ordering_[selectAll] = ordering_.Count + 1);
                commands.SelectAllQuery.Returns(selectAll);

                var insert = Substitute.For<DbCommand>();
                insert.CommandText = $"INSERT INTO {table.Name}";
                insert.When(c => c.ExecuteNonQueryAsync()).Do(_ => ordering_[insert] = ordering_.Count + 1);
                commands.InsertCommand(Arg.Any<Rows>()).Returns(insert);
                commands.When(c => c.InsertCommand(Arg.Any<Rows>())).Do(call => SetInvocationArguments(call, insert));

                var delete = Substitute.For<DbCommand>();
                delete.CommandText = $"DELETE FROM {table.Name}";
                delete.When(c => c.ExecuteNonQueryAsync()).Do(_ => ordering_[delete] = ordering_.Count + 1);
                commands.DeleteCommand(Arg.Any<Rows>()).Returns(delete);
                commands.When(c => c.DeleteCommand(Arg.Any<Rows>())).Do(call => SetInvocationArguments(call, delete));

                var update = Substitute.For<DbCommand>();
                update.CommandText = $"UPDATE {table.Name}";
                update.When(c => c.ExecuteNonQueryAsync()).Do(_ => ordering_[update] = ordering_.Count + 1);
                commands.UpdateCommand(Arg.Any<Rows>()).Returns(update);
                commands.When(c => c.UpdateCommand(Arg.Any<Rows>())).Do(call => SetInvocationArguments(call, update));

                commandsFactory_.CreateCommands(Arg.Is<ITable>(t => t.Name == table.Name), Arg.Any<bool>()).Returns(commands);
                commands_[table] = commands;
                dbRows_[table] = new List<IReadOnlyList<object>>().GetEnumerator();
            }

            transactor_ = null!;
            transactorInitializer_ = async () => transactor_ = await Transactor.New(entityTranslations, localizationTranslations, connectionPool_, commandsFactory_, e => Depot[e.GetType()].Add(e), NullLogger.Instance);            
        }
        public async Task InitializeSchema() {
            if (transactor_ is null) {
                await transactorInitializer_();
                AdminCommits = Transaction.ReceivedCalls().Count(c => c.GetMethodInfo().Name == "CommitAsync");
                Transaction.ClearReceivedCalls();

                if (withCommitError_) {
                    Transaction.When(x => x.CommitAsync()).Throw<InvalidOperationException>();
                }
                if (withRollbackError_) {
                    Transaction.When(x => x.RollbackAsync()).Throw<InvalidOperationException>();
                }
            }
        }

        public TestFixture WithCommitError() {
            withCommitError_ = true;
            return this;
        }
        public TestFixture WithRollbackError() {
            withCommitError_ = true;
            withRollbackError_ = true;
            return this;
        }

        public TestFixture WithEntityRow<TEntity>(object[] values) {
            var table = translator_[typeof(TEntity)].Principal.Table;
            var rows = new List<IReadOnlyList<object>>();
            while (dbRows_[table].MoveNext()) {
                rows.Add(dbRows_[table].Current);
            }
            rows.Add([..values]);

            dbRows_[table] = rows.GetEnumerator();
            return this;
        }
        public TestFixture WithRelationRow<TEntity>(int index, object[] values) {
            var table = translator_[typeof(TEntity)].Relations[index].Table;
            var rows = new List<IReadOnlyList<object>>();
            while (dbRows_[table].MoveNext()) {
                rows.Add(dbRows_[table].Current);
            }
            rows.Add([..values]);

            dbRows_[table] = rows.GetEnumerator();
            return this;
        }
        public TestFixture WithLocalizationRow<TEntity>(object[] values) {
            var table = translator_[typeof(TEntity), Translator.AsLocalzation].Principal.Table;
            var rows = new List<IReadOnlyList<object>>();
            while (dbRows_[table].MoveNext()) {
                rows.Add(dbRows_[table].Current);
            }
            rows.Add([..values]);

            dbRows_[table] = rows.GetEnumerator();
            return this;
        }

        public ICommands PrincipalCommands<TEntity>() {
            var type = typeof(TEntity);

            if (!Translator.IsLocalizationType(type)) {
                var table = translator_[type].Principal.Table;
                return commands_[table];
            }
            else {
                var table = translator_[type, Translator.AsLocalzation].Principal.Table;
                return commands_[table];
            }
        }
        public ICommands RelationCommands<TEntity>(int index) {
            var table = translator_[typeof(TEntity)].Relations[index].Table;
            return commands_[table];
        }

        public Rows InsertionsFor(DbCommand command) {
            if (invocationArgs_.TryGetValue(command, out IReadOnlyList<IReadOnlyList<DBValue>>? value)) {
                return value;
            }
            return [];
        }
        public Rows UpdatesFor(DbCommand command) {
            if (invocationArgs_.TryGetValue(command, out IReadOnlyList<IReadOnlyList<DBValue>>? value)) {
                return value;
            }
            return [];
        }
        public Rows DeletionsFor(DbCommand command) {
            if (invocationArgs_.TryGetValue(command, out IReadOnlyList<IReadOnlyList<DBValue>>? value)) {
                return value;
            }
            return [];
        }

        public ITable PrincipalTableOf<TEntity>() {
            return translator_[typeof(TEntity)].Principal.Table;
        }
        public ITable RelationTableOf<TEntity>(int index) {
            return translator_[typeof(TEntity)].Relations[index].Table;
        }

        [CustomAssertion]
        public void ShouldBeOrdered(params object[] commands) {
            int getOrderingOf(DbCommand cmd) {
                if (!ordering_.TryGetValue(cmd, out int order)) {
                    Execute.Assertion
                        .ForCondition(false)
                        .FailWith($"Command '{cmd.CommandText}' was never executed");
                }
                return order;
            }

            var previousOrdering = -1;
            foreach (var command in commands) {
                if (command is DbCommand cmd) {
                    int order = getOrderingOf(cmd);
                    if (order < previousOrdering) {
                        Execute.Assertion
                            .ForCondition(false)
                            .FailWith($"Command '{cmd.CommandText}' was executed out-of-order");
                    }
                    previousOrdering = order;
                }
                else {
                    var group = command as ITuple;
                    var items = Enumerable.Range(0, group!.Length).Select(i => group[i]).Cast<DbCommand>();
                    var sorted = items.Select(Command => (Command, Order: getOrderingOf(Command))).OrderBy(p => p.Order);
                    if (sorted.First().Order < previousOrdering) {
                        Execute.Assertion
                            .ForCondition(false)
                            .FailWith($"Command '{sorted.First().Command}' was executed out-of-order");
                    }
                    previousOrdering = sorted.Last().Order;
                }
            }
        }

        private void SetInvocationArguments(CallInfo info, DbCommand cmd) {
            var rows = info.ArgAt<Rows>(0).ToList();
            if (!rows.IsEmpty()) {
                invocationArgs_[cmd] = rows;
            }
        }


        private readonly IConnectionPool connectionPool_;
        private readonly ICommandsFactory commandsFactory_;
        private readonly Translator translator_;
        private readonly Dictionary<ITable, ICommands> commands_;
        private readonly Dictionary<ITable, IEnumerator<IReadOnlyList<object>>> dbRows_;
        private readonly Dictionary<DbCommand, int> ordering_;
        private readonly Dictionary<DbCommand, IReadOnlyList<IReadOnlyList<DBValue>>> invocationArgs_;

        private bool withCommitError_;
        private bool withRollbackError_;

        // We need to be able to lazily initialize the Transactor so that we can set up the contents of administrative
        // tables. This is a little janky, but I don't care at this point.
        private Transactor transactor_;
        private readonly Func<Task> transactorInitializer_;
    }
}
