using System;

namespace Kvasir.Translation2 {
    /// <summary>
    ///   An exception that is raised when two or more of a single annotation are concurrently applied to the same
    ///   property from the same translation scope.
    /// </summary>
    internal sealed class DuplicateAnnotationException : TranslationException {
        /// <summary>
        ///   Constructs a new <see cref="DuplicateAnnotationException"/>.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the invalid annotation was encountered.
        /// </param>
        /// <param name="path">
        ///   The path to which the duplicated annotations are applied.
        /// </param>
        /// <param name="annotationType">
        ///   The <see cref="Type"/> of the duplicated annotation.
        /// </param>
        public DuplicateAnnotationException(Context context, string path, Type annotationType)
            : base(
                new Location(context.ToString()),
                new Path(path),
                new Problem("only one copy of the annotation can be applied to a given Field at a time"),
                new Annotation(Display.AnnotationDisplayName(annotationType))
              )
        {}
    }
}
