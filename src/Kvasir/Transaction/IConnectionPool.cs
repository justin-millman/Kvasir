using System.Data;

namespace Kvasir.Transaction {
    /// <summary>
    ///   The interface that abstracts the creation of <see cref="IDbConnection">database connections</see> without
    ///   exposing details about the connection or the back-end provider.
    /// </summary>
    internal interface IConnectionPool {
        /// <summary>
        ///   Creates a connection.
        /// </summary>
        /// <remarks>
        ///   It is recommended that clients leverage connection within a <c>using</c> statement so that it
        ///   automatically closes when it is no longer being used. The connection returned is guaranteed to be open,
        ///   and it *may* be the same object as returned on previous invocations, though that is not requried.
        /// </remarks>
        /// <returns>
        ///   An open <see cref="IDbConnection">database connection</see>.
        /// </returns>
        IDbConnection MakeConnection();
    }
}
