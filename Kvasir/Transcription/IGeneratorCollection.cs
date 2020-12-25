namespace Kvasir.Transcription.Internal {
    /// <summary>
    ///   A collection of factory methods for generating declaratory <see cref="SqlSnippet">SqlSnippets</see> for
    ///   various schema components in a single unified syntax.
    /// </summary>
    internal interface IGeneratorCollection {
        /// <value>
        ///   The <see cref="IFieldDeclarationGenerator"/> for this collection.
        /// </value>
        public IFieldDeclarationGenerator FieldDeclarationGenerator { get; }
    }
}
