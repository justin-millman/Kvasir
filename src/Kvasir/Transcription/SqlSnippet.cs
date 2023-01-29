using Cybele.Core;
using System;

namespace Kvasir.Transcription {
    /// <summary>
    ///   A strongly typed <see cref="string"/> that represents a full or partial SQL statement or query.
    /// </summary>
    public sealed class SqlSnippet : ConceptString<SqlSnippet> {
        /// <summary>
        ///   Constructs a new <see cref="SqlSnippet"/>.
        /// </summary>
        /// <param name="sql">
        ///   The contents of the new <see cref="SqlSnippet"/>. Leading and trailing whitspace are discarded.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   if <paramref name="sql"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   if <paramref name="sql"/> is zero-length
        ///     --or--
        ///   if <paramref name="sql"/> consists only of whitespace.
        /// </exception>
        public SqlSnippet(string sql)
            : base(sql.Trim()) {

            if (string.IsNullOrWhiteSpace(Contents)) {
                var msg = $"A {nameof(SqlSnippet)} must have non-zero length after leading and trailing whitespace " +
                    "are trimmed";
                throw new ArgumentException(msg, nameof(sql));
            }
        }
    }
}
