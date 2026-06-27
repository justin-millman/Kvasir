using Kvasir.Schema;
using System;

namespace Kvasir.Translation {
    /// <summary>
    ///   An exception that is raised when the framework detects that the schema translated from the current code of a
    ///   CLR type does not match the schema of its table that already exists in the back-end database.
    /// </summary>
    internal sealed class SchemaMigrationException : TranslationException {
        /// <summary>
        ///   Constructs a new <see cref="SchemaMigrationException"/> indicating that the schema of a table has changed
        ///   in the CLR.
        /// </summary>
        /// <param name="source">
        ///   The source CLR <see cref="Type"/> that is the source of the migrated schema.
        /// </param>
        /// <param name="table">
        ///   The <see cref="ITable">table</see> whose schema migrated.
        /// </param>
        public SchemaMigrationException(Type source, ITable table)
            : base(
                new Location(source.DisplayName()),
                new Problem($"the schema of table {table.Name} has migrated since it was created in the database")
              )
        {}

        /// <summary>
        ///   Constructs a new <see cref="SchemaMigrationException"/> indicating that the CLR source of a table has been
        ///   deleted.
        /// </summary>
        /// <param name="tableName">
        ///   The name of the <see cref="ITable">table</see> whose CLR source was deleted.
        /// </param>
        public SchemaMigrationException(string tableName)
            : base(
                new Location("<back-end database>"),
                new Problem($"the CLR source of table {tableName} no longer exists")
              )
        {}
    }
}
