using Kvasir.Schema;
using System.Collections.Generic;

namespace Kvasir.Transcription {
    /// <summary>
    ///   The interface for a builder that produces declarations that define a single Foreign Key within a larger
    ///   Table-creating declaration.
    /// </summary>
    /// <typeparam name="TDecl">
    ///   The type of declaration object produced by the builder.
    /// </typeparam>
    public interface IForeignKeyDeclBuilder<TDecl> {
        /// <summary>
        ///   Sets the name of the Foreign Key being defined by the current builder's declaration.
        /// </summary>
        /// <param name="name">
        ///   The name.
        /// </param>
        /// <pre>
        ///   <paramref name="name"/> is not <see langword="null"/>.
        /// </pre>
        void SetName(FKName name);

        /// <summary>
        ///   Sets the <c>ON DELETE</c> behavior of the Foreign Key being defined by the current builder's declaration.
        /// </summary>
        /// <param name="behavior">
        ///   The <c>ON DELETE</c> behavior.
        /// </param>
        /// <pre>
        ///   <paramref name="behavior"/> is valid.
        /// </pre>
        void SetOnDeleteBehavior(OnDelete behavior);

        /// <summary>
        ///   Sets the <c>ON UPDATE</c> behavior of the Foreign Key being defined by the current builder's declaration.
        /// </summary>
        /// <param name="behavior">
        ///   The <c>ON UPDATE</c> behavior.
        /// </param>
        /// <pre>
        ///   <paramref name="behavior"/> is valid.
        /// </pre>
        void SetOnUpdateBehavior(OnUpdate behavior);

        /// <summary>
        ///   Sets the Table referenced by the Foreign Key being defined by the current builder's declaration.
        /// </summary>
        /// <param name="table">
        ///   The Table.
        /// </param>
        /// <pre>
        ///   <paramref name="table"/> is not <see langword="null"/>.
        /// </pre>
        void SetReferencedTable(ITable table);

        /// <summary>
        ///   Sets the collection of Fields that comprise the Foreign Key being defined by the current builder's
        ///   declaration.
        /// </summary>
        /// <param name="fields">
        ///   The Fields.
        /// </param>
        /// <pre>
        ///   <paramref name="fields"/> is not <see langword="null"/>
        /// </pre>
        void SetFields(IEnumerable<IField> fields);

        /// <summary>
        ///   Produces the full declaration that has been built up by calls into other methods on this
        ///   <see cref="IForeignKeyDeclBuilder{TDecl}"/>.
        /// </summary>
        /// <pre>
        ///   <see cref="SetReferencedTable(ITable)"/> has been called at least once
        ///     --and--
        ///   <see cref="SetFields(IEnumerable{IField})"/> has been called at least once.
        /// </pre>
        /// <returns>
        ///   A <typeparamref name="TDecl"/> declaring a single Foreign Key.
        /// </returns>
        TDecl Build();
    }
}
