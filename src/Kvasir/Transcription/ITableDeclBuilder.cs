using Kvasir.Schema;

namespace Kvasir.Transcription {
    /// <summary>
    ///   The interface for a builder that produces <c>CREATE TABLE</c> statements for a single Table.
    /// </summary>
    public interface ITableDeclBuilder {
        /// <summary>
        ///   Sets the name of the Table being declared by the current builder's <c>CREATE TABLE</c> statement.
        /// </summary>
        /// <param name="name">
        ///   The name.
        /// </param>
        /// <pre>
        ///   <paramref name="name"/> is not <see langword="null"/>.
        /// </pre>
        void SetName(TableName name);

        /// <summary>
        ///   Adds a Field to the Table being declared by the current builder's <c>CREATE TABLE</c>
        ///   statement.
        /// </summary>
        /// <param name="sql">
        ///   The Field declaration statement.
        /// </param>
        /// <pre>
        ///   <paramref name="sql"/> is not <see langword="null"/>.
        /// </pre>
        void AddFieldDeclaration(SqlSnippet sql);

        /// <summary>
        ///   Sets the Primary Key to the Table being declared by the current builder's <c>CREATE TABLE</c> statement.
        /// </summary>
        /// <param name="sql">
        ///   The Primary Key declaration statement.
        /// </param>
        /// <pre>
        ///   <paramref name="sql"/> is not <see langword="null"/>.
        /// </pre>
        void SetPrimaryKeyDeclaration(SqlSnippet sql);

        /// <summary>
        ///   Adds the declaration of a Candidate Key to the Table being declared by the current builder's
        ///   <c>CREATE TABLE</c> statement.
        /// </summary>
        /// <param name="sql">
        ///   The Candidate Key declaration statement.
        /// </param>
        /// <pre>
        ///   <paramref name="sql"/> is not <see langword="null"/>.
        /// </pre>
        void AddCandidateKeyDeclaration(SqlSnippet sql);

        /// <summary>
        ///   Adds a Foreign Key to the Table being declared by the current builder's <c>CREATE TABLE</c> statement.
        /// </summary>
        /// <param name="sql">
        ///   The Foreign Key declaration statement.
        /// </param>
        /// <pre>
        ///   <paramref name="sql"/> is not <see langword="null"/>.
        /// </pre>
        void AddForeignKeyDeclaration(SqlSnippet sql);

        /// <summary>
        ///   Adds a <c>CHECK</c> constraint to the Table being declared by the current builder's <c>CREATE TABLE</c>
        ///   statement.
        /// </summary>
        /// <param name="sql">
        ///   The <c>CHECK</c> constraint declaration statement.
        /// </param>
        /// <pre>
        ///   <paramref name="sql"/> is not <see langword="null"/>.
        /// </pre>
        void AddCheckConstraintDeclaration(SqlSnippet sql);

        /// <summary>
        ///   Produces the full <c>CREATE TABLE</c> statement that has been built up by calls into the other methods on
        ///   this <see cref="IConstraintDeclBuilder"/>.
        /// </summary>
        /// <pre>
        ///   <see cref="SetName(TableName)"/> has been called at least once
        ///     --and--
        ///   <see cref="AddFieldDeclaration(SqlSnippet)"/> has been called at least twice
        ///     --and--
        ///   <see cref="SetPrimaryKeyDeclaration(SqlSnippet)"/> has been called at least once.
        /// </pre>
        /// <returns>
        ///   A syntactically valid <c>CREATE TABLE</c> statement.
        /// </returns>
        SqlSnippet Build();
    }
}
