using FluentAssertions;
using FluentAssertions.Execution;
using Kvasir.Core;
using Kvasir.Schema;
using Kvasir.Transaction;
using Kvasir.Translation;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;

namespace UT.Kvasir.Transaction {
    internal sealed class TestFixture {
        public IReadOnlyDictionary<ITable, ICommands> Commands => commands_;
        public IDbConnection Connection { get; }
        public IDbTransaction Transaction { get; }
        public EntityDepot Depot { get; }
        public Transactor Transactor { get; }


        public TestFixture(params Type[] types) {
            commands_ = new Dictionary<ITable, ICommands>();
            commandsFactory_ = Substitute.For<ICommandsFactory>();
            Connection = Substitute.For<IDbConnection>();
            Transaction = Connection.BeginTransaction();
            Depot = new EntityDepot();
            translator_ = new Translator(t => Depot[t]);
            ordering_ = new Dictionary<IDbCommand, int>();

            Connection.State.Returns(ConnectionState.Open);

            var translations = types.Select(t => translator_[t]);
            var tables = translations.SelectMany(x => x.Relations.Select(r => r.Table).Append(x.Principal.Table));
            foreach (var table in tables) {
                var commands = Substitute.For<ICommands>();

                var createTable = Substitute.For<IDbCommand>();
                createTable.CommandText = $"CREATE TABLE {table.Name}";
                commands.CreateTableCommand.Returns(createTable);
                commands.CreateTableCommand.When(c => c.ExecuteNonQuery()).Do(_ => ordering_[createTable] = ordering_.Count + 1);

                commandsFactory_.CreateCommands(Arg.Is<ITable>(t => t == table)).Returns(commands);
                commands_[table] = commands;
            }

            Transactor = new Transactor(translations, Connection, commandsFactory_, e => Depot.StoreEntity(e));
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

        public ICommands PrincipalCommands<TEntity>() {
            var table = translator_[typeof(TEntity)].Principal.Table;
            return commands_[table];
        }
        public ICommands RelationCommands<TEntity>(int index) {
            var table = translator_[typeof(TEntity)].Relations[index].Table;
            return commands_[table];
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


        private readonly ICommandsFactory commandsFactory_;
        private readonly Translator translator_;
        private readonly Dictionary<ITable, ICommands> commands_;
        private readonly Dictionary<IDbCommand, int> ordering_;
    }
}
