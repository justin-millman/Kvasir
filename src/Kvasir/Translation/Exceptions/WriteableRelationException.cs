namespace Kvasir.Translation {
    /// <summary>
    ///   An exception that is raised when a freely writeable Relation-type property is included in the data model.
    /// </summary>
    internal sealed class WriteableRelationException : TranslationException {
        /// <summary>
        ///   Constructs a new <see cref="WriteableRelationException"/>.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the freely writeable Relation-type property was encountered.
        /// </param>
        public WriteableRelationException(Context context)
            : base(
                new Location(context.ToString()),
                new Problem("if a Relation-type property has a setter, is must be init-only")
              )
        {}
    }
}
