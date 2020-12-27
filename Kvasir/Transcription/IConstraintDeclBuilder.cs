using Kvasir.Schema;
using Kvasir.Schema.Constraints;
using System;

namespace Kvasir.Transcription.Internal {
    /// <summary>
    ///   An <see cref="IDeclBuilder"/> that builds declaratory <see cref="SqlSnippet">SqlSnippets</see> for non-Key
    ///   Constraints.
    /// </summary>
    internal interface IConstraintDeclBuilder : IDeclBuilder {
        /// <summary>
        ///   Sets the name of the Constraint whose declaration is represented by the current state of this
        ///   <see cref="IConstraintDeclBuilder"/>.
        /// </summary>
        /// <param name="name">
        ///   The name.
        /// </param>
        void SetName(ConstraintName name);

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
