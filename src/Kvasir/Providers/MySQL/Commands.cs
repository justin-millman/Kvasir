using Kvasir.Schema;
using Kvasir.Transaction;
using MySql.Data.MySqlClient;
using System.Data;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace Kvasir.Providers.MySQL {
    /// <summary>
    ///   An implementation of the <see cref="ICommands"/> interface for a MySQL provider.
    /// </summary>
    internal sealed class Commands : ICommands {
        /// <inheritdoc/>
        public IDbCommand CreateTableCommand {
            get {
                return new MySqlCommand(createCmdText_);
            }
        }

        /// <inheritdoc/>
        public IDbCommand SelectAllQuery {
            get {
                return new MySqlCommand(selectAllQueryText_);
            }
        }

        /// <summary>
        ///   Constructs a new <see cref="Commands"/>.
        /// </summary>
        /// <param name="table">
        ///   The <see cref="ITable">Table</see> with respect to which the new <see cref="Commands"/> instance will
        ///   operate.
        /// </param>
        /// <param name="isPrincipalTable">
        ///   <see langword="true"/> if <paramref name="table"/> is a Principal Table, and <see langword="false"/> if it
        ///   is a Relation Table. (This controls the interpretation of the "Deletion Keys" for the
        ///   <see cref="DeleteCommand(IEnumerable{IReadOnlyList{DBValue}})"/> function.)
        /// </param>
        public Commands(ITable table, bool isPrincipalTable) {
            Debug.Assert(table is not null);
            table_ = table;

            // Primary Key Matching Clause for UPDATE and DELETE
            // (UPDATE commands get the full row and therefore need to index relative to the whole schema.)
            string updateMatcher = "(";
            longRowMatcher_ = "(";
            for (int idx = 0, longCounter = 0; idx < table.Dimension; ++idx) {
                if (table.PrimaryKey.Fields.Contains(table.Fields[idx])) {
                    if (longRowMatcher_ != "(") {
                        updateMatcher += " AND ";
                        longRowMatcher_ += " AND ";
                    }
                    updateMatcher += $"`{table.Fields[idx].Name}` = {{{idx}}}";
                    longRowMatcher_ += $"`{table.Fields[idx].Name}` = {{{longCounter++}}}";
                }
            }
            updateMatcher += ")";
            longRowMatcher_ += ")";

            // Primary Key of Owning Entity Matching Clause for DELTE
            shortRowMatcher_ = "";
            if (!isPrincipalTable) {
                shortRowMatcher_ = "(";
                // The fields in the Relation Table that correspond to the Owning Entity's Primary Key are guaranteed to
                // start from index 0 and be contiguous, so we don't have to do anything awkward with tracking the
                // indices.
                for (int idx = 0; idx < table.ForeignKeys[0].ReferencingFields.Count; ++idx) {
                    if (idx != 0) {
                        shortRowMatcher_ += " AND ";
                    }
                    shortRowMatcher_ += $"`{table.PrimaryKey.Fields[idx].Name}` = {{{idx}}}";
                }
                shortRowMatcher_ += ")";
            }

            // CREATE TABLE
            createCmdText_ = table.GenerateDeclaration(new BuilderFactory()).ToString() + ";";

            // SELECT * FROM
            selectAllQueryText_ = $"SELECT * FROM `{table.Name}`;";

            // INSERT INTO
            insertCmdPreamble_ = $"INSERT INTO `{table.Name}`\n(`{table.Fields[0].Name}`";
            for (int fieldIdx = 1; fieldIdx < table.Dimension; ++fieldIdx) {
                insertCmdPreamble_ += $", `{table.Fields[fieldIdx].Name}`";
            }
            insertCmdPreamble_ += ")\nVALUES";

            // UPDATE
            updateCmdTemplate_ = $"UPDATE `{table.Name}`\nSET ";
            for (int idx = 0; idx < table.Dimension; ++idx) {
                if (!table.PrimaryKey.Fields.Contains(table.Fields[idx])) {
                    if (updateCmdTemplate_[^1] != ' ') {
                        updateCmdTemplate_ += ", ";
                    }
                    updateCmdTemplate_ += $"`{table.Fields[idx].Name}` = {{{idx}}}";
                }
            }
            updateCmdTemplate_ += $"\nWHERE {updateMatcher};";

            // DELETE
            deleteCmdPreamble_ = $"DELETE FROM `{table.Name}`\nWHERE ";
        }

        /// <inheritdoc/>
        public IDbCommand InsertCommand(IEnumerable<IReadOnlyList<DBValue>> rows) {
            var command = new MySqlCommand();
            string paramName() => $"@v{command.Parameters.Count}";

            var commandText = insertCmdPreamble_;
            foreach (var row in rows) {
                if (command.Parameters.Count != 0) {
                    commandText += ",";
                }
                commandText += "\n(";

                commandText += paramName();
                command.Parameters.Add(new MySqlParameter(paramName(), row[0].Datum));
                for (int columnIdx = 1; columnIdx < row.Count; ++columnIdx) {
                    commandText += $", {paramName()}";
                    command.Parameters.Add(new MySqlParameter(paramName(), row[columnIdx].Datum));
                }

                commandText += ")";
            }

            command.CommandText = commandText+ ";";
            return command;
        }

        /// <inheritdoc/>
        public IDbCommand UpdateCommand(IEnumerable<IReadOnlyList<DBValue>> rows) {
            var command = new MySqlCommand();
            string paramName() => $"@v{command.Parameters.Count}";

            if (table_.Dimension == table_.PrimaryKey.Fields.Count) {
                // If a Table's Primary Key is the entire row, then there's no way to do an update, because we assume
                // that a row's Primary Key will never change. To affect a conceptual update, one needs to do a DELETE
                // followed by an INSERT. Relation Tables for non-associative containers will fit this description, as
                // may some Principal Tables.
                return command;
            }

            var commandText = "";
            foreach (var row in rows) {
                if (commandText != "") {
                    commandText += "\n\n";
                }

                var parameterNames = new List<string>();
                foreach (var columnValue in row) {
                    parameterNames.Add(paramName());
                    command.Parameters.Add(new MySqlParameter(paramName(), columnValue.Datum));
                }

                commandText += string.Format(updateCmdTemplate_, parameterNames.ToArray());
            }

            command.CommandText = commandText;
            return command;
        }

        /// <inheritdoc/>
        public IDbCommand DeleteCommand(IEnumerable<IReadOnlyList<DBValue>> keys) {
            int longMatchCount = table_.PrimaryKey.Fields.Count;

            var command = new MySqlCommand();
            var commandText = "";
            string paramName() => $"@v{command.Parameters.Count}";

            foreach (var row in keys) {
                if (commandText != "") {
                    commandText += " OR\n";
                }

                var parameterNames = new List<string>();
                foreach (var columnValue in row) {
                    parameterNames.Add(paramName());
                    command.Parameters.Add(new MySqlParameter(paramName(), columnValue.Datum));
                }

                var matcher = row.Count == longMatchCount ? longRowMatcher_ : shortRowMatcher_;
                commandText += string.Format(matcher, parameterNames.ToArray());
            }

            command.CommandText = deleteCmdPreamble_ + commandText + ";";
            return command;
        }


        private readonly ITable table_;
        private readonly string longRowMatcher_;            // to match a row by Primary Key
        private readonly string shortRowMatcher_;           // to match a row by Primary Key of Owning Entity
        private readonly string createCmdText_;
        private readonly string selectAllQueryText_;
        private readonly string insertCmdPreamble_;
        private readonly string updateCmdTemplate_;
        private readonly string deleteCmdPreamble_;
    }
}
