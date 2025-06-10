using Cybele.Extensions;
using Kvasir.Core;
using Kvasir.Reconstitution;
using Kvasir.Schema;
using Kvasir.Translation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;

namespace Kvasir.Transaction {
    /// <summary>
    ///   The Transactor, which bridges the C# entity code with the back-end database based on the translation of those
    ///   entities and a particular back-end provider/system.
    /// </summary>
    internal sealed class Transactor {
        /// <summary>
        ///   Constructs a new <see cref="Translator"/> that uses default <see cref="Settings"/>.
        /// </summary>
        /// <param name="translations">
        ///   The set of <see cref="EntityTranslation">translations</see> that define the schema of the back-end
        ///   database on which the new <see cref="Transactor"/> will operate.
        /// </param>
        /// <param name="connection">
        ///   The database connection through which all commands and queries will be executed.
        /// </param>
        /// <param name="commandsFactory">
        ///   The <see cref="ICommandsFactory"/> with which to create the necessary commands and queries to interact
        ///   with the tables for <paramref name="translations"/>.
        /// </param>
        /// <param name="entityStorage">
        ///   The functor through which reconstituted Entities will be stored for later look-up.
        /// </param>
        public Transactor(
            IEnumerable<EntityTranslation> translations,
            IDbConnection connection,
            ICommandsFactory commandsFactory,
            Action<object> entityStorage
        )
            : this(translations, connection, commandsFactory, entityStorage, Settings.Default) {}

        /// <summary>
        ///   Constructs a new <see cref="Translator"/> that uses custom <see cref="Settings"/>.
        /// </summary>
        /// <param name="translations">
        ///   The set of <see cref="EntityTranslation">translations</see> that define the schema of the back-end
        ///   database on which the new <see cref="Transactor"/> will operate.
        /// </param>
        /// <param name="connection">
        ///   The database connection through which all commands and queries will be executed.
        /// </param>
        /// <param name="commandsFactory">
        ///   The <see cref="ICommandsFactory"/> with which to create the necessary commands and queries to interact
        ///   with the tables for <paramref name="translations"/>.
        /// </param>
        /// <param name="entityStorage">
        ///   The functor through which reconstituted Entities will be stored for later look-up.
        /// </param>
        /// <param name="settings">
        ///   The <see cref="Settings"/> according to which to perform the transaction activity.
        /// </param>
        /// <remarks>
        ///   Note that the settings are not currently used for anything; in fact, there are no traits available in the
        ///   <see cref="Settings"/> class. Instead, the settings serve as a forward compatibility mechanism that allows
        ///   us to provide customization of behaviors in the future without necessitating a significant redesign.
        /// </remarks>
        public Transactor(
            IEnumerable<EntityTranslation> translations,
            IDbConnection connection,
            ICommandsFactory commandsFactory,
            Action<object> entityStorage,
            Settings settings
        ) {
            Debug.Assert(translations is not null && !translations.IsEmpty());
            Debug.Assert(connection is not null && connection.State == ConnectionState.Open);
            Debug.Assert(commandsFactory is not null);
            Debug.Assert(entityStorage is not null);
            Debug.Assert(settings is not null);

            settings_ = settings;
            translations_ = translations.ToDictionary(t => t.CLRSource, t => t);
            connection_ = connection;
            commandsFactory_ = commandsFactory;
            entityStorage_ = entityStorage;

            var principalCommands = new List<ICommands>();
            var relationCommands = new List<ICommands>();
            var tables = new Dictionary<ITable, int>();
            foreach (var principal in Topology.OrderEntities(translations.ToList())) {
                tables[principal.Table] = principalCommands.Count;
                principalCommands.Add(commandsFactory_.CreateCommands(principal.Table));

                // using `Extractor.SourceType` is a little awkward here, but it's the best we have
                foreach (var relation in translations_[principal.Extractor.SourceType].Relations) {
                    tables[relation.Table] = translations_.Count + relationCommands.Count;
                    relationCommands.Add(commandsFactory_.CreateCommands(relation.Table));
                }
            }

            // Relation Tables are automatically ordered after Principal Tables; this is guaranteed to be correct, as a
            // Relation Table can only reference Principal Tables. The relative order of the Relation Tables is not
            // relevant.
            commands_ = principalCommands.Concat(relationCommands).ToList();
            tables_ = tables;
        }

        /// <summary>
        ///   Creates each of the constituent Principal and Relation Tables in the back-end database. Any rows for
        ///   Pre-Defined Entities are inserted into their corresponding Principal Table.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        ///   if the transaction creating the necessary tables fails and is successfully rolled back.
        /// </exception>
        /// <exception cref="AggregateException">
        ///   if the transaction creating the necessary tables fails, and the attempt to roll the transaction back also
        ///   fails.
        /// </exception>
        public void CreateTables() {
            using var transaction = connection_.BeginTransaction();
            foreach (var command in commands_.Select(c => c.CreateTableCommand)) {
                command.Connection = connection_;
                command.Transaction = transaction;
                command.ExecuteNonQuery();
            }
            TryCommitTransaction(transaction);

            // Rather than do the ordering ourselves here, just let the insertion function handle it. This will also
            // make everything on transaction, though that's not really super important.
            var preDefineds = translations_.Values.SelectMany(translation => translation.Principal.PreDefinedInstances);
            if (!preDefineds.IsEmpty()) {
                Insert(preDefineds.ToList());
            }
        }

        /// <summary>
        ///   Selects all of the data out of the back-end database and creates corresponding CLR objects.
        /// </summary>
        public void SelectAll() {
            var translations = translations_.Values.OrderBy(t => tables_[t.Principal.Table]).ToList();
            int numEntityTypes = translations_.Count;
            var allEntities = new Dictionary<int, List<object>>();

            // Load all the data from Principal Tables and create the Entity instances first
            for (int idx = 0; idx < numEntityTypes; idx++) {
                var query = commands_[idx].SelectAllQuery;
                query.Connection = connection_;
                var reader = query.ExecuteReader();

                var creations = new List<object>();
                while (reader.Read()) {
                    var fields = Enumerable.Range(0, reader.FieldCount).Select(i => DBValue.Create(reader[i])).ToList();
                    var entity = translations[idx].Principal.Reconstitutor.ReconstituteFrom(fields);
                    entityStorage_(entity);
                    creations.Add(entity);
                }
                allEntities[idx] = creations;
            }

            // Load all the data from Relation Tables and repopulate the Relations second
            foreach (var translation in translations_.Values) {
                var principalIndex = tables_[translation.Principal.Table];
                var options = allEntities[principalIndex];
                var matcher = new KeyMatcher(() => options, translation.Principal.KeyExtractor);

                foreach (var relation in translation.Relations) {
                    var query = commands_[tables_[relation.Table]].SelectAllQuery;
                    query.Connection = connection_;
                    var reader = query.ExecuteReader();

                    var rows = options.ToDictionary(e => e, _ => new List<IReadOnlyList<DBValue>>());
                    while (reader.Read()) {
                        var fields = Enumerable.Range(0, reader.FieldCount).Select(i => DBValue.Create(reader[i])).ToList();
                        var owningEntityKey = fields.Take(translation.Principal.Table.PrimaryKey.Fields.Count).ToList();
                        var owningEntity = matcher.Lookup(owningEntityKey);
                        rows[owningEntity].Add(fields);
                    }

                    foreach (var (owningEntity, relationRows) in rows) {
                        relation.Repopulator.Repopulate(owningEntity, relationRows);
                    }
                }
            }
        }

        /// <summary>
        ///   Inserts data for one or more Entities into the back-end database.
        /// </summary>
        /// <param name="entities">
        ///   The Entities to be inserted. This collection must not be empty, and the order is irrelevant. The
        ///   collection can contain Entities of any number of types.
        /// </param>
        /// <exception cref="InvalidOperationException">
        ///   if the transaction creating the necessary tables fails and is successfully rolled back.
        /// </exception>
        /// <exception cref="AggregateException">
        ///   if the transaction creating the necessary tables fails, and the attempt to roll the transaction back also
        ///   fails.
        /// </exception>
        public void Insert(IEnumerable<object> entities) {
            Debug.Assert(entities is not null);
            Debug.Assert(!entities.IsEmpty());

            var mapping = entities.GroupBy(e => e.GetType()).ToDictionary(g => g.Key, g => g.ToList());
            var translations = mapping.Keys.Select(k => translations_[k]).OrderBy(t => tables_[t.Principal.Table]).ToList();

            using var transaction = connection_.BeginTransaction();

            // Insert all the data into Principal Tables first
            foreach (var translation in translations) {
                var rows = new List<IReadOnlyList<DBValue>>();
                foreach (var entity in mapping[translation.CLRSource]) {
                    rows.Add(translation.Principal.Extractor.ExtractFrom(entity));
                }

                var command = commands_[tables_[translation.Principal.Table]].InsertCommand(rows);
                command.Connection = connection_;
                command.Transaction = transaction;
                command.ExecuteNonQuery();
            }

            // Insert all the data into Relation Tables second
            var canonicalizations = new List<Action>();
            foreach (var translation in translations) {
                foreach (var relation in translation.Relations) {
                    var rows = new List<IReadOnlyList<DBValue>>();
                    foreach (var entity in mapping[translation.CLRSource]) {
                        var ownerFields = translation.Principal.KeyExtractor.ExtractFrom(entity);
                        var relationFields = relation.Extractor.ExtractFrom(entity);

                        var insertionRows = relationFields.Insertions.Select(r => ownerFields.Concat(r).ToList()).ToList();
                        Debug.Assert(relationFields.Modifications.IsEmpty());
                        Debug.Assert(relationFields.Deletions.IsEmpty());

                        rows.AddRange(insertionRows);
                        canonicalizations.Add(() => relation.Extractor.Canonicalize(entity));
                    }

                    var command = commands_[tables_[relation.Table]].InsertCommand(rows);
                    command.Connection = connection_;
                    command.Transaction = transaction;
                    command.ExecuteNonQuery();
                }
            }

            // Commit the transaction; if it succeeds (failure is indicated by a `throw`), then we run all of the
            // canonicalization functions
            TryCommitTransaction(transaction);
            foreach (var canonicalize in canonicalizations) {
                canonicalize();
            }
        }

        /// <summary>
        ///   Updates data for one or more Entities in the back-end database.
        /// </summary>
        /// <param name="entities">
        ///   The Entities whose data to update. This collection must not be empty, and the order is irrelevant. The
        ///   collection can contain Entities of any number of types.
        /// </param>
        /// <exception cref="InvalidOperationException">
        ///   if the transaction updating the necessary data fails and is successfully rolled back.
        /// </exception>
        /// <exception cref="AggregateException">
        ///   if the transaction updating the necessary data fails, and the attempt to roll the transaction back also
        ///   fails.
        /// </exception>
        public void Update(IEnumerable<object> entities) {
            Debug.Assert(entities is not null);
            Debug.Assert(!entities.IsEmpty());

            var mapping = entities.GroupBy(e => e.GetType()).ToDictionary(g => g.Key, g => g.ToList());
            var translations = mapping.Keys.Select(k => translations_[k]).OrderBy(t => tables_[t.Principal.Table]).ToList();

            using var transaction = connection_.BeginTransaction();

            // Update all the data in Principal Tables first
            foreach (var translation in translations) {
                var rows = new List<IReadOnlyList<DBValue>>();
                foreach (var entity in mapping[translation.CLRSource]) {
                    rows.Add(translation.Principal.Extractor.ExtractFrom(entity));
                }

                var command = commands_[tables_[translation.Principal.Table]].UpdateCommand(rows);
                command.Connection = connection_;
                command.Transaction = transaction;
                command.ExecuteNonQuery();
            }

            // Update all the data in Relation Tables first: deletes, then updates, then inserts
            var canonicalizations = new List<Action>();
            foreach (var translation in translations) {
                foreach (var relation in translation.Relations) {
                    var deleteRows = new List<IReadOnlyList<DBValue>>();
                    var updateRows = new List<IReadOnlyList<DBValue>>();
                    var insertRows = new List<IReadOnlyList<DBValue>>();
                    foreach (var entity in mapping[translation.CLRSource]) {
                        var ownerFields = translation.Principal.KeyExtractor.ExtractFrom(entity);
                        var relationFields = relation.Extractor.ExtractFrom(entity);

                        deleteRows.AddRange(relationFields.Deletions.Select(r => ownerFields.Concat(r).ToList()));
                        updateRows.AddRange(relationFields.Modifications.Select(r => ownerFields.Concat(r).ToList()));
                        insertRows.AddRange(relationFields.Insertions.Select(r => ownerFields.Concat(r).ToList()));

                        canonicalizations.Add(() => relation.Extractor.Canonicalize(entity));
                    }

                    var deleteCommand = commands_[tables_[relation.Table]].DeleteCommand(deleteRows);
                    var updateCommand = commands_[tables_[relation.Table]].UpdateCommand(updateRows);
                    var insertCommand = commands_[tables_[relation.Table]].InsertCommand(insertRows);
                    foreach (var command in new IDbCommand[] { deleteCommand, updateCommand, insertCommand }) {
                        command.Connection = connection_;
                        command.Transaction = transaction;
                        command.ExecuteNonQuery();
                    }
                }
            }

            // Commit the transaction; if it succeeds (failure is indicated by a `throw`), then we run all of the
            // canonicalization functions
            TryCommitTransaction(transaction);
            foreach (var canonicalize in canonicalizations) {
                canonicalize();
            }
        }

        /// <summary>
        ///   Deletes data for one or more Entities from the back-end database.
        /// </summary>
        /// <param name="entities">
        ///   The Entities whose data to delete. This collection must not be empty, and the order is irrelevant. The
        ///   collection can contain Entities of any number of types.
        /// </param>
        /// <exception cref="InvalidOperationException">
        ///   if the transaction deleting the necessary data fails and is successfully rolled back.
        /// </exception>
        /// <exception cref="AggregateException">
        ///   if the transaction deleting the necessary data fails, and the attempt to roll the transaction back also
        ///   fails.
        /// </exception>
        public void Delete(IEnumerable<object> entities) {
            Debug.Assert(entities is not null);
            Debug.Assert(!entities.IsEmpty());

            var mapping = entities.GroupBy(e => e.GetType()).ToDictionary(g => g.Key, g => g.ToList());
            var translations = mapping.Keys.Select(k => translations_[k]).OrderBy(t => tables_[t.Principal.Table]).ToList();

            using var transaction = connection_.BeginTransaction();

            // Delete all the data from Relation Tables first
            foreach (var translation in translations) {
                foreach (var relation in translation.Relations) {
                    var rows = new List<IReadOnlyList<DBValue>>();
                    foreach (var entity in mapping[translation.CLRSource]) {
                        // For efficiency, we'll issue a DELETE based on the Primary Key of the "owning Entity." This
                        // reduces the amount of reflection we have to do (since we don't have to touch the Relation
                        // itself at all), reduces the complexity of the DELETE command, and handles the fact that the
                        // RelationExtractionPlan does not expose SAVED elements (which have to be deleted). However,
                        // this does mean that we'll issue a DELETE command for empty Relations and Relations of only
                        // NEW elements.
                        rows.Add(translation.Principal.KeyExtractor.ExtractFrom(entity));
                    }

                    var command = commands_[tables_[relation.Table]].DeleteCommand(rows);
                    command.Connection = connection_;
                    command.Transaction = transaction;
                    command.ExecuteNonQuery();
                }
            }

            // Delete all the data from Principal Tables second
            foreach (var translation in translations) {
                var rows = new List<IReadOnlyList<DBValue>>();
                foreach (var entity in mapping[translation.CLRSource]) {
                    rows.Add(translation.Principal.KeyExtractor.ExtractFrom(entity));
                }

                var command = commands_[tables_[translation.Principal.Table]].DeleteCommand(rows);
                command.Connection = connection_;
                command.Transaction = transaction;
                command.ExecuteNonQuery();
            }

            // Commit the transaction; since we're doing deletes, we don't need to canonicalize anything - it's on the
            // caller to throw away their objects and create new ones
            TryCommitTransaction(transaction);
        }

        /// <summary>
        ///   Attempts to commit a <see cref="IDbTransaction">database transaction</see>. If the commit operation fails,
        ///   an attempt to roll the transaction back is made.
        /// </summary>
        /// <param name="transaction">
        ///   The <see cref="IDbTransaction">database transaction</see>.
        /// </param>
        /// <exception cref="InvalidOperationException">
        ///   if the attempt to commit <paramref name="transaction"/> fails and is successfully rolled back.
        /// </exception>
        /// <exception cref="AggregateException">
        ///   if the attempt to commit <paramref name="transaction"/> fails, and the attempt to roll it back also fails.
        /// </exception>
        private static void TryCommitTransaction(IDbTransaction transaction) {
            try {
                transaction.Commit();
            }
            catch (InvalidOperationException commitEx) {
                try {
                    transaction.Rollback();
                }
                catch (InvalidOperationException rollbackEx) {
                    throw new AggregateException(commitEx, rollbackEx);
                }

                // Re-throw the exception from the failed transaction so that the caller can handle it as they want
                throw;
            }
        }


        private readonly Settings settings_;
        private readonly IReadOnlyList<ICommands> commands_;                // topologically ordered
        private readonly IReadOnlyDictionary<Type, EntityTranslation> translations_;
        private readonly IReadOnlyDictionary<ITable, int> tables_;          // maps to index in `commands_` list
        private readonly IDbConnection connection_;
        private readonly ICommandsFactory commandsFactory_;
        private readonly Action<object> entityStorage_;
    }
}
