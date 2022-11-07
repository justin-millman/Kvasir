using Kvasir.Schema;
using System.Diagnostics;

namespace Kvasir.Translation {
    /// <summary>
    ///   A representation of the data model for a Principal Table (rather than a Relation Table).
    /// </summary>
    internal sealed record class PrincipalTableDef {
        /// <summary>
        ///   The <see cref="ITable">Table</see>.
        /// </summary>
        public ITable Table { get; }

        /// <summary>
        ///   Construct a new <see cref="PrincipalTableDef"/>.
        /// </summary>
        /// <param name="table">
        ///   The <see cref="Table">Principal Table</see>.
        /// </param>
        public PrincipalTableDef(ITable table) {
            Debug.Assert(table is not null);
            Table = table;
        }

        /* Because PrincipalTableDef is a record type, the following methods are synthesized automatically by the compiler:
         *   > public PrincipalTableDef(PrincipalTableDef rhs)
         *   > public bool Equals(PrincipalTableDef? rhs)
         *   > public sealed override bool Equals(object? rhs)
         *   > public sealed override int GetHashCode()
         *   > public sealed override string ToString()
         *   > public static bool operator==(PrincipalTableDef? lhs, PrincipalTableDef? rhs)
         *   > public static bool oeprator!=(PrincipalTableDef? lhs, PrincipalTableDef? rhs)
         */
    }
}
