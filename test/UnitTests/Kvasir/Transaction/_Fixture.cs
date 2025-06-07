using Cybele.Extensions;
using FluentAssertions;
using FluentAssertions.Execution;
using Kvasir.Schema;
using Kvasir.Transaction;
using Kvasir.Translation;
using NSubstitute;
using NSubstitute.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;

using Rows = System.Collections.Generic.IEnumerable<System.Collections.Generic.IReadOnlyList<Kvasir.Schema.DBValue>>;

namespace UT.Kvasir.Transaction {
    internal sealed class TestFixture {
        public IReadOnlyDictionary<ITable, ICommands> Commands => commands_;
        public IDbConnection Connection { get; }
        public IDbTransaction Transaction { get; }
        public Dictionary<Type, List<object>> Depot { get; }
        public Transactor Transactor { get; }


        public TestFixture(params Type[] types) {
            commands_ = new Dictionary<ITable, ICommands>();
            dbRows_ = new Dictionary<ITable, IEnumerator<IReadOnlyList<object>>>();
            commandsFactory_ = Substitute.For<ICommandsFactory>();
            Connection = Substitute.For<IDbConnection>();
            Transaction = Connection.BeginTransaction();
            Depot = types.ToDictionary(t => t, _ => new List<object>());
            translator_ = new Translator(t => Depot[t]);
            ordering_ = new Dictionary<IDbCommand, int>();
            invocationArgs_ = new Dictionary<IDbCommand, IReadOnlyList<IReadOnlyList<DBValue>>>();

            Connection.State.Returns(ConnectionState.Open);

            var translations = types.Select(t => translator_[t]);
            var tables = translations.SelectMany(x => x.Relations.Select(r => r.Table).Append(x.Principal.Table));
            foreach (var table in tables) {
                var commands = Substitute.For<ICommands>();

                var createTable = Substitute.For<IDbCommand>();
                createTable.CommandText = $"CREATE TABLE {table.Name}";
                createTable.When(c => c.ExecuteNonQuery()).Do(_ => ordering_[createTable] = ordering_.Count + 1);
                commands.CreateTableCommand.Returns(createTable);

                var reader = Substitute.For<IDataReader>();
                reader.Read().Returns(_ => dbRows_[table].MoveNext());
                reader.FieldCount.Returns(_ => dbRows_[table].Current.Count);
                reader[Arg.Any<int>()].Returns(args => dbRows_[table].Current[args.ArgAt<int>(0)]);
                reader.ClearReceivedCalls();

                var selectAll = Substitute.For<IDbCommand>();
                selectAll.CommandText = $"SELECT * FROM {table.Name}";
                selectAll.ExecuteReader().Returns(reader);
                selectAll.When(c => c.ExecuteReader()).Do(_ => ordering_[selectAll] = ordering_.Count + 1);
                commands.SelectAllQuery.Returns(selectAll);

                var insert = Substitute.For<IDbCommand>();
                insert.CommandText = $"INSERT INTO {table.Name}";
                insert.When(c => c.ExecuteNonQuery()).Do(_ => ordering_[insert] = ordering_.Count + 1);
                commands.InsertCommand(Arg.Any<Rows>()).Returns(insert);
                commands.When(c => c.InsertCommand(Arg.Any<Rows>())).Do(call => SetInvocationArguments(call, insert));

                var delete = Substitute.For<IDbCommand>();
                delete.CommandText = $"DELETE FROM {table.Name}";
                delete.When(c => c.ExecuteNonQuery()).Do(_ => ordering_[delete] = ordering_.Count + 1);
                commands.DeleteCommand(Arg.Any<Rows>()).Returns(delete);
                commands.When(c => c.DeleteCommand(Arg.Any<Rows>())).Do(call => SetInvocationArguments(call, delete));

                var update = Substitute.For<IDbCommand>();
                update.CommandText = $"UPDATE {table.Name}";
                update.When(c => c.ExecuteNonQuery()).Do(_ => ordering_[update] = ordering_.Count + 1);
                commands.UpdateCommand(Arg.Any<Rows>()).Returns(update);
                commands.When(c => c.UpdateCommand(Arg.Any<Rows>())).Do(call => SetInvocationArguments(call, update));

                commandsFactory_.CreateCommands(Arg.Is<ITable>(t => t == table), Arg.Any<bool>()).Returns(commands);
                commands_[table] = commands;
                dbRows_[table] = new List<IReadOnlyList<object>>().GetEnumerator();
            }

            Transactor = new Transactor(translations, Connection, commandsFactory_, e => Depot[e.GetType()].Add(e));
        }
        public TestFixture WithCommitError() {
            Transaction.When(x => x.Commit()).Throw<InvalidOperationException>();
            return this;
        }
        public TestFixture WithRollbackError() {
            WithCommitError();
            Transaction.When(x => x.Rollback()).Throw<InvalidOperationException>();
            return this;
        }

        public TestFixture WithEntityRow<TEntity>(object[] values) {
            var table = translator_[typeof(TEntity)].Principal.Table;
            var rows = new List<IReadOnlyList<object>>();
            while (dbRows_[table].MoveNext()) {
                rows.Add(dbRows_[table].Current);
            }
            rows.Add(new List<object>(values));

            dbRows_[table] = rows.GetEnumerator();
            return this;
        }
        public TestFixture WithRelationRow<TEntity>(int index, object[] values) {
            var table = translator_[typeof(TEntity)].Relations[index].Table;
            var rows = new List<IReadOnlyList<object>>();
            while (dbRows_[table].MoveNext()) {
                rows.Add(dbRows_[table].Current);
            }
            rows.Add(new List<object>(values));

            dbRows_[table] = rows.GetEnumerator();
            return this;
        }

        public ICommands PrincipalCommands<TEntity>() {
            var table = translator_[typeof(TEntity)].Principal.Table;
            return commands_[table];
        }
        public ICommands RelationCommands<TEntity>(int index) {
            var table = translator_[typeof(TEntity)].Relations[index].Table;
            return commands_[table];
        }

        public Rows InsertionsFor(IDbCommand command) {
            if (invocationArgs_.TryGetValue(command, out IReadOnlyList<IReadOnlyList<DBValue>>? value)) {
                return value;
            }
            return Enumerable.Empty<IReadOnlyList<DBValue>>();
        }
        public Rows UpdatesFor(IDbCommand command) {
            if (invocationArgs_.TryGetValue(command, out IReadOnlyList<IReadOnlyList<DBValue>>? value)) {
                return value;
            }
            return Enumerable.Empty<IReadOnlyList<DBValue>>();
        }
        public Rows DeletionsFor(IDbCommand command) {
            if (invocationArgs_.TryGetValue(command, out IReadOnlyList<IReadOnlyList<DBValue>>? value)) {
                return value;
            }
            return Enumerable.Empty<IReadOnlyList<DBValue>>();
        }

        [CustomAssertion]
        public void ShouldBeOrdered(params object[] commands) {
            int getOrderingOf(IDbCommand cmd) {
                if (!ordering_.TryGetValue(cmd, out int order)) {
                    Execute.Assertion
                        .ForCondition(false)
                        .FailWith($"Command '{cmd.CommandText}' was never executed");
                }
                return order;
            }

            var previousOrdering = -1;
            foreach (var command in commands) {
                if (command is IDbCommand cmd) {
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
                    var items = Enumerable.Range(0, group!.Length).Select(i => group[i]).Cast<IDbCommand>();
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

        private void SetInvocationArguments(CallInfo info, IDbCommand cmd) {
            var rows = info.ArgAt<Rows>(0).ToList();
            if (!rows.IsEmpty()) {
                invocationArgs_[cmd] = rows;
            }
        }


        private readonly ICommandsFactory commandsFactory_;
        private readonly Translator translator_;
        private readonly Dictionary<ITable, ICommands> commands_;
        private readonly Dictionary<ITable, IEnumerator<IReadOnlyList<object>>> dbRows_;
        private readonly Dictionary<IDbCommand, int> ordering_;
        private readonly Dictionary<IDbCommand, IReadOnlyList<IReadOnlyList<DBValue>>> invocationArgs_;
    }
}
