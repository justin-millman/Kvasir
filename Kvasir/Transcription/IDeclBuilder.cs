namespace Kvasir.Transcription.Internal {
    /// <summary>
    ///   The interface for a builder that produces <see cref="SqlSnippet">SqlSnippets</see> that can be used in a DDL
    ///   statement.
    /// </summary>
    internal interface IDeclBuilder {
        /// <summary>
        ///   Builds the declaratory <see cref="SqlSnippet"/> based on the current state of this
        ///   <see cref="IDeclBuilder"/>.
        /// </summary>
        /// <returns>
        ///   A <see cref="SqlSnippet"/> that reflects the current state of this <see cref="IDeclBuilder"/>.
        /// </returns>
        SqlSnippet Build();

        /// <summary>
        ///   Resets the current state of this <see cref="IDeclBuilder"/>.
        /// </summary>
        void Reset();
    }
}
