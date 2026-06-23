using Kvasir.Annotations;
using Kvasir.Schema;

namespace Kvasir.Administration {
    /// <summary>
    ///   The CLR object representation of the administrative table for managing <see cref="ITable">Table</see> schemas.
    /// </summary>
    [Table("_Kvasir_TableSchemaAdmin")]
    internal sealed class TableHash {
        /// <summary>
        ///   The name of the <see cref="ITable">Table</see>.
        /// </summary>
        [PrimaryKey, Column(0)] public string TableName { get; init; } = "";

        /// <summary>
        ///   The hash value for the <see cref="ITable">Table</see>, synthesized from all the constituent pieces
        ///   (fields, keys, and constraints).
        /// </summary>
        [Column(1)] public int Hash { get; init; }
    }
}
