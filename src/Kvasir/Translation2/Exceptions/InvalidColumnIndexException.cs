using Kvasir.Annotations;

namespace Kvasir.Translation2 {
    /// <summary>
    ///   An exception that is raised when the column index provided to a <see cref="ColumnAttribute">[Column]</see>
    ///   annotation is not valid.
    /// </summary>
    internal sealed class InvalidColumnIndexException : TranslationException {
        /// <summary>
        ///   Constructs a new <see cref="InvalidColumnIndexException"/>.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the invalid annotation was encountered.
        /// </param>
        /// <param name="index">
        ///   The invalid column index.
        /// </param>
        public InvalidColumnIndexException(Context context, int index)
            : base(
                new Location(context.ToString()),
                new Problem($"the column index {index} is negative"),
                new Annotation(Display.AnnotationDisplayName(typeof(ColumnAttribute)))
              )
        {}
    }
}
