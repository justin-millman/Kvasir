using Kvasir.Annotations;

namespace Kvasir.Translation2 {
    /// <summary>
    ///   An exception that is raised when a user-provided name is invalid.
    /// </summary>
    internal sealed class InvalidNameException : TranslationException {
        /// <summary>
        ///   Constructs a new <see cref="InvalidNameException"/> caused by an invalid Field name.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which <paramref name="annotation"/> was encountered.
        /// </param>
        /// <param name="annotation">
        ///   The invalid <see cref="NameAttribute">[Name]</see> annotation.
        /// </param>
        public InvalidNameException(Context context, NameAttribute annotation)
            : base(
                new Location(context.ToString()),
                new Path(annotation.Path),
                new Problem("the name of a Field cannot be " + (annotation.Name is null ? "'null'" : "empty")),
                new Annotation(nameof(NameAttribute)[..^9]))
        {}

        /// <summary>
        ///   Constructs a new <see cref="InvalidNameException"/> caused by an invalid Principal Table name.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which <paramref name="annotation"/> was encountered.
        /// </param>
        /// <param name="annotation">
        ///   The invalid <see cref="TableAttribute">[Table]</see> annotation.
        /// </param>
        public InvalidNameException(Context context, TableAttribute annotation)
            : base(
                new Location(context.ToString()),
                new Problem("the name of a Table cannot be " + (annotation.Name is null ? "'null'" : "empty")),
                new Annotation(nameof(NameAttribute)[..^9])
              )
        {}

        /// <summary>
        ///   Constructs a new <see cref="InvalidNameException"/> caused by an invalid Relation Table name.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which <paramref name="annotation"/> was encountered.
        /// </param>
        /// <param name="annotation">
        ///   The invalid <see cref="RelationTableAttribute">[RelationTable]</see> annotation.
        /// </param>
        public InvalidNameException(Context context, RelationTableAttribute annotation)
            : base(
                new Location(context.ToString()),
                new Problem("the name of a relation's Table cannot be " + (annotation.Name is null ? "'null'" : "empty")),
                new Annotation(nameof(RelationTableAttribute)[..^9])
              )
        {}
    }
}
