using Kvasir.Schema.Constraints;
using System;

namespace Kvasir.Transcription.Internal {
    /// <summary>
    ///   A factory that provides methods for generating declaratory <see cref="SqlSnippet">SqlSnippets</see> for
    ///   Constraints and Keys.
    /// </summary>
    internal interface IConstraintDeclGenerator {
        /// <summary>
        ///   Creates a <see cref="SqlSnippet"/> from the contents that have been added to the current constraint
        ///   declaration. The current state is cleared, and a subsequent call to <see cref="MakeSnippet"/> prior to
        ///   adding any additional clauses will produce undefined behavior.
        /// </summary>
        /// <pre>
        ///   All <c>AND</c> and <c>OR</c> clauses have been closed
        ///     --and--
        ///   At least one clause has been added to this <see cref="IConstraintDeclGenerator"/>.
        /// </pre>
        /// <returns>
        ///   A <see cref="SqlSnippet"/> consisting of the constraint declaration that has been built up.
        /// </returns>
        SqlSnippet MakeSnippet();

        /// <summary>
        ///   Begins a new <c>AND</c> clause in the current constraint declaration.
        /// </summary>
        /// <returns>
        ///   A handle that, when disposed of, terminates the new <c>AND</c> clause that has been started.
        /// </returns>
        IDisposable NewAndClause();

        /// <summary>
        ///   Begins a new <c>OR</c> clause in the current constraint declaration.
        /// </summary>
        /// <returns>
        ///   A handle that, when disposed of, terminates the new <c>OR</c> clause that has been started.
        /// </returns>
        IDisposable NewOrClause();

        /// <summary>
        ///   Adds a new <see cref="NullityClause"/> to the current constraint declaration.
        /// </summary>
        /// <param name="clause">
        ///   The <see cref="NullityClause"/> to add to the constraint declaration.
        /// </param>
        void AddCheck(NullityClause clause);

        /// <summary>
        ///   Adds a new <see cref="InclusionClause"/> to the current constraint declaration.
        /// </summary>
        /// <param name="clause">
        ///   The <see cref="InclusionClause"/> to add to the constraint declaration.
        /// </param>
        void AddCheck(InclusionClause clause);

        /// <summary>
        ///   Adds a new <see cref="ConstantValueClause"/> to the current constraint declaration.
        /// </summary>
        /// <param name="clause">
        ///   The <see cref="ConstantValueClause"/> to add to the constraint declaration.
        /// </param>
        void AddCheck(ConstantValueClause clause);

        /// <summary>
        ///   Adds a new <see cref="CrossFieldValueConstraint"/> to the current constraint declaration.
        /// </summary>
        /// <param name="clause">
        ///   The <see cref="CrossFieldValueConstraint"/> to add to the constraint declaration.
        /// </param>
        void AddCheck(CrossFieldValueConstraint clause);
    }
}
