using Kvasir.Schema;
using System.Collections.Generic;
using System.Data.Common;

namespace Kvasir.Transaction {
    ///
    public interface ICommands {
        /// <summary>
        ///   The <see cref="ITable">Table</see> against which the commands execute.
        /// </summary>
        ITable Table { get; set; }

        /// <summary>
        ///   The command that creates <see cref="Table"/> in the back-end database.
        /// </summary>
        DbCommand CreateTableCommand { get; }

        /// <summary>
        ///   The command that selects all data from <see cref="Table"/>. The rows should be returned in key-sorted
        ///   order, which for Relation Tables guarantees that the rows are grouped by "owning Entity."
        /// </summary>
        DbCommand SelectAllQuery { get; }

        /// <summary>
        ///   Produce a command that, when executed against a back-end database and committed, inserts one or more rows
        ///   of data into <see cref="Table"/>.
        /// </summary>
        /// <param name="rows">
        ///   The rows of data to be inserted.
        /// </param>
        DbCommand InsertCommand(IEnumerable<IReadOnlyList<DBValue>> rows);

        /// <summary>
        ///   Produce a command that, when executed against a back-end database and committed, updates the values of one
        ///   or more rows that already exist in <see cref="Table"/>.
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
        DbCommand UpdateCommand(IEnumerable<IReadOnlyList<DBValue>> rows);

        /// <summary>
        ///   Produce a command that, when executed against a back-end database and committed, deleted one or more rows
        ///   from <see cref="Table"/>.
        /// </summary>
        /// <remarks>
        ///   Deleting a row from a Principal Table may result in rows in various other tables (either other Entities'
        ///   Principal Tables or Relation Tables) being invalid due to existing Foreign Keys. <see cref="ICommands"/>
        ///   implementations are encouraged to leverage <c>ON DELETE</c> behavior to account for these situations.
        ///   Implementations may, however, handle such events manually (such as if <c>ON DELETE</c> behavior is not
        ///   supported.)
        /// </remarks>
        /// <param name="rows">
        ///   The rows of data that should be deleted. (Note that this is <b>not</b> just the rows' Primary Keys.)
        /// </param>
        DbCommand DeleteCommand(IEnumerable<IReadOnlyList<DBValue>> rows);
    }
}
