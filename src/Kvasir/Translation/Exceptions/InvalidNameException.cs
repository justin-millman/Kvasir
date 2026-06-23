using Kvasir.Annotations;

namespace Kvasir.Translation {
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
                new Annotation(Display.AnnotationDisplayName(typeof(NameAttribute)))
              )
        {}

        /// <summary>
        ///   Constructs a new <see cref="InvalidNameException"/> caused by an invalid Primary Key name.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which <paramref name="annotation"/> was encountered.
        /// </param>
        /// <param name="annotation">
        ///   The invalid <see cref="NamedPrimaryKeyAttribute">[NamedPrimaryKey]</see> annotation.
        /// </param>
        public InvalidNameException(Context context, NamedPrimaryKeyAttribute annotation)
            : base(
                new Location(context.ToString()),
                new Problem("the name of a Primary Key cannot be " + (annotation.Name is null ? "'null'" : "empty")),
                new Annotation(Display.AnnotationDisplayName(typeof(NamedPrimaryKeyAttribute)))
              )
        {}

        /// <summary>
        ///   Constructs a new <see cref="InvalidNameException"/> caused by an invalid Principal Table name, as defined
        ///   by a <see cref="TableAttribute">[Table]</see> annotation.
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
                new Problem($"the name of a Primary Table cannot {TableErrorMessageReason(annotation.Name)}"),
                new Annotation(Display.AnnotationDisplayName(typeof(TableAttribute)))
              )
        {}

        /// <summary>
        ///   Constructs a new <see cref="InvalidNameException"/> caused by an invalid Principal Table name resulting
        ///   from the interplay between a <see cref="TableAttribute">[Table]</see> annotation and an
        ///   <see cref="ExcludeNamespaceFromNameAttribute">[ExcludeNamespaceFromName]</see> annotation.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which <paramref name="annotation"/> was encountered.
        /// </param>
        /// <param name="annotation">
        ///   The invalid <see cref="ExcludeNamespaceFromNameAttribute">[ExcludeNamespaceFromName]</see> annotation.
        /// </param>
        public InvalidNameException(Context context, ExcludeNamespaceFromNameAttribute annotation)
            : base(
                new Location(context.ToString()),
                new Problem("the name of a Primary Table cannot be empty"),
                new Annotation(Display.AnnotationDisplayName(annotation.GetType()))
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
                new Problem($"the name of a Relation Table cannot {TableErrorMessageReason(annotation.Name)}"),
                new Annotation(Display.AnnotationDisplayName(typeof(RelationTableAttribute)))
              )
        {}

        /// <summary>
        ///   Constructs a new <see cref="InvalidNameException"/> caused by an invalid Candidate Key name.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which <paramref name="annotation"/> was encountered.
        /// </param>
        /// <param name="annotation">
        ///   The invalid <see cref="UniqueAttribute">[Unique]</see> annotation.
        /// </param>
        public InvalidNameException(Context context, UniqueAttribute annotation)
            : base(
                new Location(context.ToString()),
                new Problem("the name of a Candidate Key cannot " +
                       (
                         annotation.Name is null ? "be 'null'" :
                         annotation.Name == "" ? "be empty" :
                         $"begin with the reserved character sequence \"{UniqueAttribute.ANONYMOUS_PREFIX}\""
                        )
                    ),
                new Annotation(Display.AnnotationDisplayName(typeof(UniqueAttribute)))
              )
        {}

        /// <summary>
        ///   Produces the reason that a particular table name string is invalid.
        /// </summary>
        /// <param name="name">
        ///   The table name string.
        /// </param>
        /// <returns>
        ///   The portion of the error message after "cannot" that explains why <paramref name="name"/> is not a valid
        ///   table name.
        /// </returns>
        private static string TableErrorMessageReason(string? name) {
            if (name is null) {
                return "be 'null'";
            }
            else if (name == "") {
                return "be empty";
            }
            else {
                return "begin with the reserved prefix '_Kvasir_'";
            }
        }
    }
}
