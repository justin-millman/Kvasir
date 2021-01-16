using Kvasir.Schema;
using System.Collections.Generic;

namespace Kvasir.Transcription {
    /// <summary>
    ///   The interface for a builder that produces SQL expressions that declare a single Primary Key or a single
    ///   Candidate Key within a <c>CREATE TABLE</c> statement.
    /// </summary>
    public interface IKeyDeclBuilder {
        /// <summary>
        ///   Sets the name of the Primary Key or Candidate Key being declared by the current builder's SQL expression.
        /// </summary>
        /// <param name="name">
        ///   The name.
        /// </param>
        /// <pre>
        ///   <paramref name="name"/> is not <see langword="null"/>.
        /// </pre>
        void SetName(KeyName name);

        /// <summary>
        ///   Sets the collection of Fields that comprise the Primary Key or Candidate Key being declared by the
        ///   current builder's SQL expression. expression being built.
        /// </summary>
        /// <param name="fields">
        ///   The Fields.
        /// </param>
        /// <pre>
        ///   <paramref name="fields"/> is not <see langword="null"/>
        ///     --and--
        ///   <paramref name="fields"/> contains at least one element.
        /// </pre>
        void SetFields(IEnumerable<IField> fields);

        /// <summary>
        ///   Marks the SQL expression being built as one for a Primary Key rather than a Candidate Key.
        /// </summary>
        void SetAsPrimaryKey();

        /// <summary>
        ///   Produces the full SQL expression that has been built up by the current builder's SQL expression.
        ///   <see cref="IKeyDeclBuilder"/>.
        /// </summary>
        /// <pre>
        ///   <see cref="SetFields(IEnumerable{IField})"/> has been called at least once.
        /// </pre>
        /// <returns>
        ///   A syntactically valid SQL expression declaring a single Primary Key or a single Candidate Key.
        /// </returns>
        SqlSnippet Build();
    }
}
