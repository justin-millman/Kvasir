namespace Kvasir.Transcription.Internal {
    /// <summary>
    ///   A collection of factory methods for generating declaratory <see cref="SqlSnippet">SqlSnippets</see> for
    ///   various schema components in a single unified syntax.
    /// </summary>
    internal interface IGeneratorCollection {
        /// <value>
        ///   The <see cref="IFieldDeclGenerator"/> for this collection.
        /// </value>
        IFieldDeclGenerator FieldDeclGenerator { get; }

        /// <value>
        ///   The <see cref="IConstraintDeclGenerator"/> for this collection.
        /// </value>
        IConstraintDeclGenerator ConstraintDeclGenerator { get; }
    }
}
