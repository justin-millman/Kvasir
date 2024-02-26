using Kvasir.Annotations;

namespace Kvasir.Translation2 {
    /// <summary>
    ///   An exception that is raised when the path of a nestable annotation is invalid, either because it is
    ///   <see langword="null"/> or because the no property in the data model exists at that path.
    /// </summary>
    internal sealed class InvalidPathException : TranslationException {
        /// <summary>
        ///   Constructs a new <see cref="InvalidPathException"/>.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the annotation with an invalid path was encountered.
        /// </param>
        /// <param name="annotation">
        ///   The annotation with an invalid path.
        /// </param>
        public InvalidPathException(Context context, INestableAnnotation annotation)
            : base(
                new Location(context.ToString()),
                new Problem(annotation.Path is null ? "the path cannot be 'null'" : $"the path \"{annotation.Path}\" does not exist"),
                new Annotation(Display.AnnotationDisplayName(annotation.GetType()))
              )
        {}
    }
}
