using Kvasir.Transaction;
using MySql.Data.MySqlClient;
using System.Data.Common;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Kvasir.Providers.MySQL {
    /// <summary>
    ///   The <see cref="IConnectionPool"/> implementation for the MySQL provider.
    /// </summary>
    internal sealed class ConnectionPool : IConnectionPool {
        /// <summary>
        ///   Creates a new MySQL <see cref="ConnectionPool"/>.
        /// </summary>
        /// <param name="server">
        ///   The database server.
        /// </param>
        /// <param name="database">
        ///   The database name.
        /// </param>
        /// <param name="username">
        ///   The connection username.
        /// </param>
        /// <param name="password">
        ///   The connection password.
        /// </param>
        public ConnectionPool(string server, string database, string username, string password) {
            Debug.Assert(server is not null && server != "");
            Debug.Assert(database is not null && database != "");
            Debug.Assert(username is not null && username != "");
            Debug.Assert(password is not null && password != "");

            connectionString_ = new MySqlConnectionStringBuilder() {
                Server = server,
                Database = database,
                UserID = username,
                Password = password
            }.ConnectionString;
        }

        /// <see cref="IConnectionPool.MakeConnectionAsync"/>
        public async Task<DbConnection> MakeConnectionAsync() {
            var connection = new MySqlConnection(connectionString_);
            await connection.OpenAsync();
            return connection;
        }


        private readonly string connectionString_;
    }
}
