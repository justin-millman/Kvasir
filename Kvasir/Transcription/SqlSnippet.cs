using Cybele;
using System.Diagnostics;

namespace Kvasir.Transcription.Internal {
    /// <summary>
    ///   A snippet of SQL that declares a complete or partial schema component according to some provider-specific
    ///   syntax rules.
    /// </summary>
    internal sealed class SqlSnippet : ConceptString<SqlSnippet> {
        /// <summary>
        ///   Constructs a new <see cref="SqlSnippet"/>.
        /// </summary>
        /// <param name="sql">
        ///   The SQL snippet in string form, properly formatted and with any necesary character or substrings already
        ///   escaped.
        /// </param>
        /// <pre>
        ///   <paramref name="sql"/> contains at least one non-whitespace character.
        /// </pre>
        public SqlSnippet(string sql)
            : base(sql.Trim()) {

            Debug.Assert(!string.IsNullOrWhiteSpace(Contents));
        }
    }
}
