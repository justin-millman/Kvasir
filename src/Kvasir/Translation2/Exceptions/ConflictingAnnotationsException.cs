using System;

namespace Kvasir.Translation2 {
    /// <summary>
    ///   An exception that is raised when two annotations that inherently contradictory are both applied to the same
    ///   property.
    /// </summary>
    internal sealed class ConflictingAnnotationsException : TranslationException {
        /// <summary>
        ///   Constructs a new <see cref="ConflictingAnnotationsException"/>.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the invalid annotation was encountered.
        /// </param>
        /// <param name="first">
        ///   The type of the first of the two conflicting annotations.
        /// </param>
        /// <param name="second">
        ///   The type of the second of the two conflicting annotations.
        /// </param>
        public ConflictingAnnotationsException(Context context, Type first, Type second)
            : base(
                new Location(context.ToString()),
                new Problem("the two annotations are mutually exclusive"),
                new Annotation(Display.AnnotationDisplayName(first)),
                new Annotation(Display.AnnotationDisplayName(second))
              )
        {}
    }
}
