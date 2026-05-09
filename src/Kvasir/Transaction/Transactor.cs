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
        /// <param name="entityTranslations">
        ///   The set of <see cref="EntityTranslation">translations</see> for non-Localization types that define the
        ///   schema of the back-end database on which the new <see cref="Transactor"/> will operate.
        /// </param>
        /// <param name="localizationTranslations">
        ///   The set of <see cref="LocalizationTranslation">translations</see> for Localization types that define the
        ///   schema of the back-end database on which the new <see cref="Transactor"/> will operate.
        /// </param>
        /// <param name="connection">
        ///   The database connection through which all commands and queries will be executed.
        /// </param>
        /// <param name="commandsFactory">
        ///   The <see cref="ICommandsFactory"/> with which to create the necessary commands and queries to interact
        ///   with the tables for <paramref name="entityTranslations"/> and <paramref name="localizationTranslations"/>.
        /// </param>
        /// <param name="entityStorage">
        ///   The functor through which reconstituted Entities will be stored for later look-up.
        /// </param>
        public Transactor(
            IEnumerable<EntityTranslation> entityTranslations,
            IEnumerable<LocalizationTranslation> localizationTranslations,
            IDbConnection connection,
            ICommandsFactory commandsFactory,
            Action<object> entityStorage
        )
            : this(entityTranslations, localizationTranslations, connection, commandsFactory, entityStorage, Settings.Default) {}

        /// <summary>
        ///   Constructs a new <see cref="Translator"/> that uses custom <see cref="Settings"/>.
        /// </summary>
        /// <param name="entityTranslations">
        ///   The set of <see cref="EntityTranslation">translations</see> for non-Localization types that define the
        ///   schema of the back-end database on which the new <see cref="Transactor"/> will operate.
        /// </param>
        /// <param name="localizationTranslations">
        ///   The set of <see cref="LocalizationTranslation">translations</see> for Localization types that define the
        ///   schema of the back-end database on which the new <see cref="Transactor"/> will operate.
        /// </param>
        /// <param name="connection">
        ///   The database connection through which all commands and queries will be executed.
        /// </param>
        /// <param name="commandsFactory">
        ///   The <see cref="ICommandsFactory"/> with which to create the necessary commands and queries to interact
        ///   with the tables for <paramref name="entityTranslations"/> and <paramref name="localizationTranslations"/>.
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
            IEnumerable<EntityTranslation> entityTranslations,
            IEnumerable<LocalizationTranslation> localizationTranslations,
            IDbConnection connection,
            ICommandsFactory commandsFactory,
            Action<object> entityStorage,
            Settings settings
        ) {
            Debug.Assert(entityTranslations is not null);
            Debug.Assert(localizationTranslations is not null);
            Debug.Assert(!entityTranslations.IsEmpty() || !localizationTranslations.IsEmpty());
            Debug.Assert(connection is not null && connection.State == ConnectionState.Open);
            Debug.Assert(commandsFactory is not null);
            Debug.Assert(entityStorage is not null);
            Debug.Assert(settings is not null);

            settings_ = settings;
            entityTranslations_ = entityTranslations.ToDictionary(t => t.CLRSource, t => t);
            localizationTranslations_ = localizationTranslations.ToDictionary(t => t.CLRSource, t => t);
            connection_ = connection;
            commandsFactory_ = commandsFactory;
            entityStorage_ = entityStorage;

            var principalCommands = new List<ICommands>();
            var relationCommands = new List<ICommands>();
            var localizationCommands = new List<ICommands>();
            var tables = new Dictionary<ITable, int>();
            foreach (var principal in Topology.OrderEntities(entityTranslations.ToList())) {
                tables[principal.Table] = principalCommands.Count;
                principalCommands.Add(commandsFactory_.CreateCommands(principal.Table, isPrincipalTable: true));

                // using `Extractor.SourceType` is a little awkward here, but it's the best we have
                foreach (var relation in entityTranslations_[principal.Extractor.SourceType].Relations) {
                    tables[relation.Table] = entityTranslations_.Count + relationCommands.Count;
                    relationCommands.Add(commandsFactory_.CreateCommands(relation.Table, isPrincipalTable: false));
                }
            }
            foreach (var principal in localizationTranslations.Select(x => x.Principal)) {
                tables[principal.Table] = principalCommands.Count + relationCommands.Count + localizationCommands.Count;
                localizationCommands.Add(commandsFactory_.CreateCommands(principal.Table, isPrincipalTable: true));
            }

            // The order here is Regular Entities -> Relations -> Localizations, with the former in topological order.
            // Nothing ever references a Localization Table (we omit foreign keys to support "empty" Localizations), so
            // they can safely go last. For the same reason, the Localizations can be arbitrarily ordered relative to
            // one another. The relative order of the Relation Tables is likewise not relevant because they cannot
            // reference each other, either.
            localizationStartIdx_ = principalCommands.Count + relationCommands.Count;
            commands_ = principalCommands.Concat(relationCommands).Concat(localizationCommands).ToList();
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
            // make everything one transaction, though that's not really super important.
            var preDefineds = entityTranslations_.Values.SelectMany(translation => translation.Principal.PreDefinedInstances);
            preDefineds = preDefineds.Concat(localizationTranslations_.Values.SelectMany(translation => translation.Principal.PreDefinedInstances));
            if (!preDefineds.IsEmpty()) {
                Insert(preDefineds.ToList());
            }
        }

        /// <summary>
        ///   Selects all of the data out of the back-end database and creates corresponding CLR objects.
        /// </summary>
        public void SelectAll() {
            var entityTranslations = entityTranslations_.Values.OrderBy(t => tables_[t.Principal.Table]).ToList();
            var localizationTranslations = localizationTranslations_.Values.ToList();
            int numEntityTypes = entityTranslations_.Count;
            int numLocalizationTypes = localizationTranslations_.Count;
            var allEntities = new Dictionary<int, List<object>>();

            // Load all the data from Localization Tables and create the Localization instances first
            var locRowsByKey = new List<Dictionary<DBValue, List<List<DBValue>>>>();
            var locInstByKey = new List<Dictionary<DBValue, object>>();
            for (int idx = 0; idx < numLocalizationTypes; ++idx) {
                var query = commands_[idx + localizationStartIdx_].SelectAllQuery;
                query.Connection = connection_;
                var reader = query.ExecuteReader();

                var rows = new Dictionary<DBValue, List<List<DBValue>>>();
                var instances = new Dictionary<DBValue, object>();

                while (reader.Read()) {
                    var fields = Enumerable.Range(0, reader.FieldCount).Select(i => DBValue.Create(reader[i])).ToList();
                    if (!rows.TryGetValue(fields[0], out List<List<DBValue>>? currentRows)) {
                        currentRows = new List<List<DBValue>>();
                        var instance = localizationTranslations[idx].Principal.Reconstitutor.ReconstituteFrom(fields.Take(1).ToList());
                        rows[fields[0]] = currentRows;
                        instances[fields[0]] = instance;
                    }
                    currentRows.Add(fields);
                }

                locRowsByKey.Add(rows);
                locInstByKey.Add(instances);
            }
            foreach (var dict in locInstByKey) {
                foreach (var (_, instance) in dict.OrderBy(kvp => kvp.Key.Datum)) {
                    entityStorage_(instance);
                }
            }

            // Load all the data from Principal Tables and create the Entity instances second
            for (int idx = 0; idx < numEntityTypes; ++idx) {
                var query = commands_[idx].SelectAllQuery;
                query.Connection = connection_;
                var reader = query.ExecuteReader();

                var creations = new List<object>();
                while (reader.Read()) {
                    var fields = Enumerable.Range(0, reader.FieldCount).Select(i => DBValue.Create(reader[i])).ToList();
                    var entity = entityTranslations[idx].Principal.Reconstitutor.ReconstituteFrom(fields);
                    entityStorage_(entity);
                    creations.Add(entity);
                }
                allEntities[idx] = creations;
            }

            // Load all the data from Relation Tables and repopulate the Relations third
            foreach (var translation in entityTranslations_.Values) {
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

            // Repopulate the Localization instances last
            for (int idx = 0; idx < numLocalizationTypes; ++idx) {
                foreach (var (key, rows) in locRowsByKey[idx].OrderBy(kvp => kvp.Key.Datum)) {
                    var instance = locInstByKey[idx][key];
                    localizationTranslations[idx].Principal.Repopulator.Repopulate(instance, rows);
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
            var entityTranslations = mapping.Keys.Where(t => !Translator.IsLocalizationType(t)).Select(k => entityTranslations_[k]).OrderBy(t => tables_[t.Principal.Table]).ToList();
            var localizationTranslations = mapping.Keys.Where(t => Translator.IsLocalizationType(t)).Select(k => localizationTranslations_[k]).ToList();

            using var transaction = connection_.BeginTransaction();

            // Insert all the data into Principal Tables first
            foreach (var translation in entityTranslations) {
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
            foreach (var translation in entityTranslations) {
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

            // Insert all the data into Localization Tables third
            foreach (var translation in localizationTranslations) {
                var rows = new List<IReadOnlyList<DBValue>>();
                foreach (var entity in mapping[translation.CLRSource]) {
                    var localizationFields = translation.Principal.Extractor.ExtractFrom(entity);

                    var insertionRows = localizationFields.Insertions;
                    Debug.Assert(localizationFields.Modifications.IsEmpty());
                    Debug.Assert(localizationFields.Deletions.IsEmpty());

                    rows.AddRange(insertionRows);
                    canonicalizations.Add(() => translation.Principal.Extractor.Canonicalize(entity));
                }

                var command = commands_[tables_[translation.Principal.Table]].InsertCommand(rows);
                command.Connection = connection_;
                command.Transaction = transaction;
                command.ExecuteNonQuery();
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
            var entityTranslations = mapping.Keys.Where(t => !Translator.IsLocalizationType(t)).Select(k => entityTranslations_[k]).OrderBy(t => tables_[t.Principal.Table]).ToList();
            var localizationTranslations = mapping.Keys.Where(t => Translator.IsLocalizationType(t)).Select(k => localizationTranslations_[k]).ToList();

            using var transaction = connection_.BeginTransaction();

            // Update all the data in Principal Tables first
            foreach (var translation in entityTranslations) {
                var rows = new List<IReadOnlyList<DBValue>>();
                foreach (var entity in mapping[translation.CLRSource]) {
                    rows.Add(translation.Principal.Extractor.ExtractFrom(entity));
                }

                var command = commands_[tables_[translation.Principal.Table]].UpdateCommand(rows);
                command.Connection = connection_;
                command.Transaction = transaction;
                command.ExecuteNonQuery();
            }

            // Update all the data in Relation Tables second: deletes, then updates, then inserts
            var canonicalizations = new List<Action>();
            foreach (var translation in entityTranslations) {
                foreach (var relation in translation.Relations) {
                    var deleteRows = new List<IReadOnlyList<DBValue>>();
                    var updateRows = new List<IReadOnlyList<DBValue>>();
                    var insertRows = new List<IReadOnlyList<DBValue>>();
                    foreach (var entity in mapping[translation.CLRSource]) {
                        var ownerFields = translation.Principal.KeyExtractor.ExtractFrom(entity);
                        var relationFields = relation.Extractor.ExtractFrom(entity);
                        var relationPKFieldsCount = relation.Table.PrimaryKey.Fields.Count - ownerFields.Count();

                        deleteRows.AddRange(relationFields.Deletions.Select(r => ownerFields.Concat(r.Take(relationPKFieldsCount)).ToList()));
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

            // Update all the data in Localization Tables third: deletes then inserts
            foreach (var translation in localizationTranslations) {
                var deleteRows = new List<IReadOnlyList<DBValue>>();
                var insertRows = new List<IReadOnlyList<DBValue>>();
                foreach (var entity in mapping[translation.CLRSource]) {
                    var localizationFields = translation.Principal.Extractor.ExtractFrom(entity);

                    // (Localization Key, Locale) are uniquely identifying
                    var deletionRows = localizationFields.Deletions.Select(r => r.Take(2).ToList());
                    var insertionRows = localizationFields.Insertions;
                    Debug.Assert(localizationFields.Modifications.IsEmpty());

                    deleteRows.AddRange(deletionRows);
                    insertRows.AddRange(insertionRows);

                    canonicalizations.Add(() => translation.Principal.Extractor.Canonicalize(entity));
                }

                var deleteCommand = commands_[tables_[translation.Principal.Table]].DeleteCommand(deleteRows);
                var insertCommand = commands_[tables_[translation.Principal.Table]].InsertCommand(insertRows);
                foreach (var command in new IDbCommand[] { deleteCommand, insertCommand }) {
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
            var entityTranslations = mapping.Keys.Where(t => !Translator.IsLocalizationType(t)).Select(k => entityTranslations_[k]).OrderBy(t => tables_[t.Principal.Table]).ToList();
            var localizationTranslations = mapping.Keys.Where(t => Translator.IsLocalizationType(t)).Select(k => localizationTranslations_[k]).ToList();

            using var transaction = connection_.BeginTransaction();

            // Delete all the data from Localization Tables first
            foreach (var translation in localizationTranslations) {
                var rows = new List<IReadOnlyList<DBValue>>();
                foreach (var entity in mapping[translation.CLRSource]) {
                    // For efficiency, we'll issue a DELETE based on the Primary Key of the Localization. This reduces
                    // the amount of reflection we have to do (since we don't have to touch the Locale/Value at all),
                    // reduces the complexity of the DELETE command, and handles the fact that the
                    // RelationExtractionPlan does not expose SAVED elements (which have to be deleted). However, this
                    // does mean that we'll issue a DELETE command for empty Localizations.
                    rows.Add(translation.Principal.KeyExtractor.ExtractFrom(entity));
                }

                var command = commands_[tables_[translation.Principal.Table]].DeleteCommand(rows);
                command.Connection = connection_;
                command.Transaction = transaction;
                command.ExecuteNonQuery();
            }

            // Delete all the data from Relation Tables second
            foreach (var translation in entityTranslations) {
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

            // Delete all the data from Principal Tables third
            foreach (var translation in entityTranslations) {
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
        private readonly int localizationStartIdx_;
        private readonly IReadOnlyDictionary<Type, EntityTranslation> entityTranslations_;
        private readonly IReadOnlyDictionary<Type, LocalizationTranslation> localizationTranslations_;
        private readonly IReadOnlyDictionary<ITable, int> tables_;          // maps to index in `commands_` list
        private readonly IDbConnection connection_;
        private readonly ICommandsFactory commandsFactory_;
        private readonly Action<object> entityStorage_;
    }
}
