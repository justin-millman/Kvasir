namespace Kvasir.Transcription.Internal {
    /// <summary>
    ///   A collection of <see cref="IDeclBuilder">IDeclBuilders</see> for generating the declaratory
    ///   <see cref="SqlSnippet">SqlSnippets</see> of various schema components with a provider-unified syntax.
    /// </summary>
    internal interface IBuilderCollection {
        /// <summary>
        ///   Exposes an <see cref="IFieldDeclBuilder"/> using the syntax of this <see cref="IBuilderCollection"/>
        ///   that is in its initial (or "reset") state.
        /// </summary>
        /// <returns>
        ///   An <see cref="IFieldDeclBuilder"/> in its initial (or "reset") state that uses the rules of this
        ///   <see cref="IBuilderCollection"/>.
        /// </returns>
        IFieldDeclBuilder FieldDeclBuilder();

        /// <summary>
        ///   Exposes an <see cref="IConstraintDeclBuilder"/> using the syntax of this <see cref="IBuilderCollection"/>
        ///   that is in its initial (or "reset") state.
        /// </summary>
        /// <returns>
        ///   An <see cref="IConstraintDeclBuilder"/> in its initial (or "reset") state that uses the rules of this
        ///   <see cref="IBuilderCollection"/>.
        /// </returns>
        IConstraintDeclBuilder ConstraintDeclBuilder();
    }
}
