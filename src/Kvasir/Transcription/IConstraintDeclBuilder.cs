using Kvasir.Schema;

namespace Kvasir.Transcription {
    /// <summary>
    ///   The interface for a builder that produces SQL expressions that declare a single <c>CHECK</c> constraint
    ///   within a <c>CREATE TABLE</c> statement.ent.
    /// </summary>
    public interface IConstraintDeclBuilder {
        /// <summary>
        ///   Sets the name of the <c>CHECK</c> constraint whose declaration is being built by this
        ///   <see cref="IConstraintDeclBuilder"/>.
        /// </summary>
        /// <param name="name">
        ///   The name.
        /// </param>
        /// <pre>
        ///   <paramref name="name"/> is not <see langword="null"/>.
        /// </pre>
        void SetName(CheckName name);

        /// <summary>
        ///   Begins a new compound clause in the <c>CHECK</c> constraint being declared by the current builder's SQL
        ///   expression. This is the equivalent of placing an open parenthesis when writing the full condition from
        ///   left-to-right.
        /// </summary>
        void StartClause();

        /// <summary>
        ///   Ends the most-recently-started compound clause in the <c>CHECK</c> constraint being declared by the
        ///   current builder's SQL expression. This is the equivalent of placing a close parenthesis when writing the
        ///   full condition from right-to-left.
        /// </summary>
        void EndClause();

        /// <summary>
        ///   Places an <c>AND</c> operator into the <c>CHECK</c> constraint being declared by the current builder's
        ///   SQL expression.
        /// </summary>
        void And();

        /// <summary>
        ///   Placs an <c>OR</c> operator into the <c>CHECK</c> constraint being declared by the current builder's SQL
        ///   expression.
        /// </summary>
        void Or();

        /// <summary>
        ///   Adds a new <see cref="ConstantClause"/> to the <c>CHECK</c> constraint being declared by the current
        ///   builder's SQL expression.
        /// </summary>
        /// <param name="clause">
        ///   The <see cref="ConstantClause"/>.
        /// </param>
        /// <pre>
        ///   <paramref name="clause"/> is not <see langword="null"/>.
        /// </pre>
        void AddClause(ConstantClause clause);

        /// <summary>
        ///   Adds a new <see cref="CrossFieldClause"/> to the <c>CHECK</c> constraint being declared by the current
        ///   builder's SQL expression.
        /// </summary>
        /// <param name="clause">
        ///   The <see cref="CrossFieldClause"/>.
        /// </param>
        /// <pre>
        ///   <paramref name="clause"/> is not <see langword="null"/>.
        /// </pre>
        void AddClause(CrossFieldClause clause);

        /// <summary>
        ///   Adds a new <see cref="InclusionClause"/> to the <c>CHECK</c> constraint being declared by the current
        ///   builder's SQL expression.
        /// </summary>
        /// <param name="clause">
        ///   The <see cref="InclusionClause"/>.
        /// </param>
        /// <pre>
        ///   <paramref name="clause"/> is not <see langword="null"/>.
        /// </pre>
        void AddClause(InclusionClause clause);

        /// <summary>
        ///   Adds a new <see cref="NullityClause"/> to the <c>CHECK</c> constraint being declared by the current
        ///   builder's SQL expression.
        /// </summary>
        /// <param name="clause">
        ///   The <see cref="NullityClause"/>.
        /// </param>
        /// <pre>
        ///   <paramref name="clause"/> is not <see langword="null"/>.
        /// </pre>
        void AddClause(NullityClause clause);

        /// <summary>
        ///   Produces the full SQL expression that has been built up by calls into other methods on this
        ///   <see cref="IConstraintDeclBuilder"/>.
        /// </summary>
        /// <pre>
        ///   Collectively, <see cref="AddClause(ConstantClause)"/>, <see cref="AddClause(CrossFieldClause)"/>,
        ///   <see cref="AddClause(InclusionClause)"/>, and <see cref="AddClause(NullityClause)"/> have been called at
        ///   least once
        ///     --and--
        ///   <see cref="StartClause"/> has been called the same number of times as <see cref="EndClause"/>.
        /// </pre>
        /// <returns>
        ///   A syntactically valid SQL expression declaring a single <c>CHECK</c> constraint.
        /// </returns>
        SqlSnippet Build();
    }
}
