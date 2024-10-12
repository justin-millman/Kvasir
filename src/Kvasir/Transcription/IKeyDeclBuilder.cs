using Kvasir.Schema;
using System.Collections.Generic;

namespace Kvasir.Transcription {
    /// <summary>
    ///   The interface for a builder that produces declarations that define a single Primary Key or a single Candidate
    ///   Key within a larger Table-creating declaration.
    /// </summary>
    /// <typeparam name="TDecl">
    ///   The type of declaration object produced by the builder.
    /// </typeparam>
    public interface IKeyDeclBuilder<TDecl> {
        /// <summary>
        ///   Sets the name of the Primary Key or Candidate Key being defined by the current builder's declaration.
        /// </summary>
        /// <param name="name">
        ///   The name.
        /// </param>
        /// <pre>
        ///   <paramref name="name"/> is not <see langword="null"/>.
        /// </pre>
        void SetName(KeyName name);

        /// <summary>
        ///   Sets the collection of Fields that comprise the Primary Key or Candidate Key being defined by the
        ///   current builder's declaration.
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
        ///   Marks the declaration being built as one for a Primary Key rather than a Candidate Key.
        /// </summary>
        void SetAsPrimaryKey();

        /// <summary>
        ///   Produces the full declaration that has been built up by calls into other methods on this
        ///   <see cref="IKeyDeclBuilder{TDecl}"/>.
        /// </summary>
        /// <pre>
        ///   <see cref="SetFields(IEnumerable{IField})"/> has been called at least once.
        /// </pre>
        /// <returns>
        ///   A <typeparamref name="TDecl"/> declaring a single Primary Key or a single Candidate Key.
        /// </returns>
        TDecl Build();
    }
}
