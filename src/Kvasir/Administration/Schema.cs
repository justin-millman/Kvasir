using Kvasir.Annotations;
using Kvasir.Schema;
using System;
using System.Data.Common;
using System.Security.Cryptography;
using System.Text;

namespace Kvasir.Administration {
    /// <summary>
    ///   The CLR object representation of the administrative table for managing <see cref="ITable">Table</see> schemas.
    /// </summary>
    [Table("_Kvasir_TableSchemaAdmin")]
    internal sealed class TableFingerprint {
        /// <summary>
        ///   The name of the <see cref="ITable">Table</see>.
        /// </summary>
        [PrimaryKey, Column(0)]
        public string TableName { get; init; } = "";

        /// <summary>
        ///   The fingerprint for the <see cref="ITable">Table</see>, synthesized from all the constituent pieces
        ///   (fields, keys, and constraints).
        /// </summary>
        [Column(1), Check.LengthIsExactly(64)]
        public string Fingerprint { get; init; } = "";

        /// <summary>
        ///   Produces the <see cref="Fingerprint">fingerprint</see> for a given <see cref="ITable">Table</see>, based
        ///   on its <c>CREATE TABLE</c> command.
        /// </summary>
        /// <param name="createTableCommand">
        ///   The <c>CREATE TABLE</c> command from which the fingerprint is calculated.
        /// </param>
        /// <returns>
        ///   The fingerprint calculated from <paramref name="createTableCommand"/>.
        /// </returns>
        public static string For(DbCommand createTableCommand) {
            byte[] data = Encoding.UTF8.GetBytes(createTableCommand.CommandText);
            byte[] hash = SHA256.HashData(data);
            return Convert.ToHexStringLower(hash);
        }
    }
}
