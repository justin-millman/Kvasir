using Kvasir.Schema;
using Kvasir.Transaction;
using System.Data;
using System.Diagnostics;
using System.Collections.Generic;

namespace Kvasir.Providers.MySQL {
    /// <summary>
    ///   An implementation of the <see cref="ICommands"/> interface for a MySQL provider.
    /// </summary>
    internal sealed class Commands : ICommands {
        /// <inheritdoc/>
        public IDbCommand CreateTableCommand => throw new System.NotImplementedException();

        /// <inheritdoc/>
        public IDbCommand SelectAllQuery => throw new System.NotImplementedException();

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
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        public IDbCommand InsertCommand(IEnumerable<IReadOnlyList<DBValue>> rows) {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        public IDbCommand UpdateCommand(IEnumerable<IReadOnlyList<DBValue>> rows) {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        public IDbCommand DeleteCommand(IEnumerable<IReadOnlyList<DBValue>> keys) {
            throw new System.NotImplementedException();
        }
    }
}
