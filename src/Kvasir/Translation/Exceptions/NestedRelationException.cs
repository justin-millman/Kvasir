namespace Kvasir.Translation {
    /// <summary>
    ///   An exception that is raised when the element type of a Relation container is itself, or contains, another
    ///   Relation.
    /// </summary>
    internal sealed class NestedRelationException : TranslationException {
        /// <summary>
        ///   Constructs a new <see cref="NestedRelationException"/>.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the nested Relation was encountered.
        /// </param>
        public NestedRelationException(Context context)
            : base(
                new Location(context.ToString()),
                new Problem("nested Relations (i.e. within a Localization or another Relation) are not supported")
              )
        {}
    }
}