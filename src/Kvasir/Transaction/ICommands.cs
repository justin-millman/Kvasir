using Kvasir.Schema;
using System.Collections.Generic;
using System.Data;

namespace Kvasir.Transaction {
    /// <summary>
    ///   The interface describing the selection and CRUD commands for a single <see cref="ITable">Table</see>.
    /// </summary>
    public interface ICommands {
        /// <summary>
        ///   The command that creates the target <see cref="Table"/> in the back-end database.
        /// </summary>
        IDbCommand CreateTableCommand { get; }

        /// <summary>
        ///   The command that selects all data from the target <see cref="Table"/>. The rows should be returned in
        ///   key-sorted order, which for Relation Tables guarantees that the rows are grouped by "owning Entity."
        /// </summary>
        IDbCommand SelectAllQuery { get; }

        /// <summary>
        ///   Produce a command that, when executed against a back-end database and committed, inserts one or more rows
        ///   of data into the target <see cref="Table"/>.
        /// </summary>
        /// <param name="rows">
        ///   The rows of data to be inserted.
        /// </param>
        IDbCommand InsertCommand(IEnumerable<IReadOnlyList<DBValue>> rows);

        /// <summary>
        ///   Produce a command that, when executed against a back-end database and committed, updates the values of one
        ///   or more rows that already exist in the target <see cref="Table"/>.
        /// </summary>
        /// <remarks>
        ///   Kvasir itself does not do any mutation tracking outside of Relations, and even then its tracking is
        ///   somewhat rudimentary. <see cref="ICommands"/> implementations are free to implement their own state
        ///   tracking, such that an update request does nothing if no state has actually changes. This is not required
        ///   however: implementations may unconditionally perform an update, even if the result is no net difference.
        ///   It is always safe to assume that the values that make up a particular Entity's primary key do not change.
        /// </remarks>
        /// <param name="rows">
        ///   The full rows of data that should exist in the back-end database after the update.
        /// </param>
        IDbCommand UpdateCommand(IEnumerable<IReadOnlyList<DBValue>> rows);

        /// <summary>
        ///   Produce a command that, when executed against a back-end database and committed, deleted one or more rows
        ///   from the target <see cref="Table"/>.
        /// </summary>
        /// <remarks>
        ///   Deleting a row from a Principal Table may result in rows in various other tables (either other Entities'
        ///   Principal Tables or Relation Tables) being invalid due to existing Foreign Keys. <see cref="ICommands"/>
        ///   implementations are encouraged to leverage <c>ON DELETE</c> behavior to account for these situations.
        ///   Implementations may, however, handle such events manually (such as if <c>ON DELETE</c> behavior is not
        ///   supported.)
        /// </remarks>
        /// <param name="keys">
        ///   The "Deletion Keys" of the rows of data that should be deleted. If the <see cref="ICommands"/> instance is
        ///   responsible for a Principal Table, this should be the Primary Key; otherwise, it should be the Primary Key
        ///   of the "owning Entity" of the relation.
        /// </param>
        /// <note>
        ///   The expected shape of the <paramref name="keys"/> depends on whether the <see cref="ICommands"/> instance
        ///   is targeting a Principal Table or a Relation Table. Each element can always be the Primary Key of a single
        ///   row to be deleted; however, if the target is a Relation Table, each element can also be the Primary Key of
        ///   an owning Entity whose entire Relation is to be deleted. This represents an optimization opportunity over
        ///   performing several individual row deletes. Note that, in general, we cannot support accepting the full row
        ///   of data and then extracting the Primary Key; if we tried to do this, we would have an ambiguity in the
        ///   case where the full row is the Primary Key, but the Fields in the Primary Key are listed in a different
        ///   order than their columns naturally fall. In such a case, we'd be unable to determine which orientation the
        ///   values represent, especially if there isn't enough type-based information to discriminate (e.g. if all the
        ///   Fields are of the same type). If the Primary Key is, in fact, the full row, then obviously the full row
        ///   can be provided, though the Fields must be present in the order matching that of the Primary Key.
        /// </note>
        IDbCommand DeleteCommand(IEnumerable<IReadOnlyList<DBValue>> keys);
    }
}
