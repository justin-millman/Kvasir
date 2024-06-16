using Kvasir.Annotations;

namespace Kvasir.Translation {
    /// <summary>
    ///   An exception that is raised when a <see cref="PrimaryKeyAttribute">[PrimaryKey]</see> annotation is applied,
    ///   directly or indirectly, to an invalid Field.
    /// </summary>
    internal sealed class InvalidPrimaryKeyFieldException : TranslationException {
        /// <summary>
        ///   Constructs a new <see cref="InvalidPrimaryKeyFieldException"/>.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the invalid annotation was encountered.
        /// </param>
        /// <param name="path">
        ///   The path to which the invalid annotation was applied.
        /// </param>
        /// <param name="cascadePath">
        ///   The path that the invalid annotation affects. This parameter should be used when the annotation is placed
        ///   on an Aggregate or applies to an Aggregate via the <paramref name="path"/>, and that Aggregate itself
        ///   contains a Field that cannot be in an Entity's primary key.
        /// </param>
        /// <param name="reason">
        ///   The reason that the <see cref="PrimaryKeyAttribute">[PrimaryKey]</see> annotation is invalid.
        /// </param>
        public InvalidPrimaryKeyFieldException(Context context, string path, string cascadePath, string reason)
            : base(
                new Location(context.ToString()),
                new Path(path),
                new Cascade(cascadePath),
                new Problem(reason),
                new Annotation(Display.AnnotationDisplayName(typeof(PrimaryKeyAttribute)))
              )
        {}
    }
}
