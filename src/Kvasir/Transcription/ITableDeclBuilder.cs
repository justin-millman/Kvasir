using Kvasir.Schema;

namespace Kvasir.Transcription {
    /// <summary>
    ///   The interface for a builder that produces declarations that define a single Table.
    /// </summary>
    /// <typeparam name="TTableDecl">
    ///   The type of declaration object produced by the builder.
    /// </typeparam>
    /// <typeparam name="TFieldDecl">
    ///   The type of declaration object through which the builder will manage Fields.
    /// </typeparam>
    /// <typeparam name="TKeyDecl">
    ///   The type of declaration object through which the builder will manage Primary Keys and Candidate Keys.
    /// </typeparam>
    /// <typeparam name="TConstraintDecl">
    ///   The type of declaration object through which the builder will manage <c>CHECK</c>-style constraints.
    /// </typeparam>
    /// <typeparam name="TFKDecl">
    ///   The type of declaration object through which the builder will manage Foreign Keys.
    /// </typeparam>
    public interface ITableDeclBuilder<TTableDecl, TFieldDecl, TKeyDecl, TConstraintDecl, TFKDecl> {
        /// <summary>
        ///   Sets the name of the Table being declared by the current builder's overall declaration.
        /// </summary>
        /// <param name="name">
        ///   The name.
        /// </param>
        /// <pre>
        ///   <paramref name="name"/> is not <see langword="null"/>.
        /// </pre>
        void SetName(TableName name);

        /// <summary>
        ///   Adds a Field to the Table being declared by the current builder's overall declaration.
        /// </summary>
        /// <param name="decl">
        ///   The Field declaration.
        /// </param>
        /// <pre>
        ///   <paramref name="decl"/> is not <see langword="null"/>.
        /// </pre>
        void AddFieldDeclaration(TFieldDecl decl);

        /// <summary>
        ///   Sets the Primary Key of the Table being declared by the current builder's overall declaration.
        /// </summary>
        /// <param name="decl">
        ///   The Primary Key declaration.
        /// </param>
        /// <pre>
        ///   <paramref name="decl"/> is not <see langword="null"/>.
        /// </pre>
        void SetPrimaryKeyDeclaration(TKeyDecl decl);

        /// <summary>
        ///   Adds the declaration of a Candidate Key to the Table being declared by the current builder's overall
        ///   declaration.
        /// </summary>
        /// <param name="decl">
        ///   The Candidate Key declaration.
        /// </param>
        /// <pre>
        ///   <paramref name="decl"/> is not <see langword="null"/>.
        /// </pre>
        void AddCandidateKeyDeclaration(TKeyDecl decl);

        /// <summary>
        ///   Adds a Foreign Key to the Table being declared by the current builder's overall declaration.
        /// </summary>
        /// <param name="decl">
        ///   The Foreign Key declaration.
        /// </param>
        /// <pre>
        ///   <paramref name="decl"/> is not <see langword="null"/>.
        /// </pre>
        void AddForeignKeyDeclaration(TFKDecl decl);

        /// <summary>
        ///   Adds a <c>CHECK</c>-style constraint to the Table being declared by the current builder's overall
        ///   declaration.
        /// </summary>
        /// <param name="decl">
        ///   The <c>CHECK</c>-style constraint declaration.
        /// </param>
        /// <pre>
        ///   <paramref name="decl"/> is not <see langword="null"/>.
        /// </pre>
        void AddCheckConstraintDeclaration(TConstraintDecl decl);

        /// <summary>
        ///   Produces the full statement that has been built up by calls into the other methods on this
        ///   <see cref="ITableDeclBuilder{TTableDecl, TFieldDecl, TKeyDecl, TConstraintDecl, TFKDecl}"/>.
        /// </summary>
        /// <pre>
        ///   <see cref="SetName(TableName)"/> has been called at least once
        ///     --and--
        ///   <see cref="AddFieldDeclaration(TFieldDecl)"/> has been called at least twice
        ///     --and--
        ///   <see cref="SetPrimaryKeyDeclaration(TKeyDecl)"/> has been called at least once.
        /// </pre>
        /// <returns>
        ///   A <typeparamref name="TTableDecl"/> declaring a single Table.
        /// </returns>
        TTableDecl Build();
    }
}
