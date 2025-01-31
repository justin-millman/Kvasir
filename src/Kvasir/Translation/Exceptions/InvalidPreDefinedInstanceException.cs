using Kvasir.Annotations;

namespace Kvasir.Translation {
    /// <summary>
    ///   An exception that is raised when an invalid property is translated into the data model as a pre-defined
    ///   instance.
    /// </summary>
    internal sealed class InvalidPreDefinedInstanceException : TranslationException {
        /// <summary>
        ///   Constructs a new <see cref="InvalidPreDefinedInstanceException"/> caused by a writeable property being
        ///   identified as a pre-defined instance.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the invalid pre-defined instance was encountered.
        /// </param>
        public InvalidPreDefinedInstanceException(Context context)
            : base(
                new Location(context.ToString()),
                new Problem("a writeable property cannot be a pre-defined instance")
              )
        {}

        /// <summary>
        ///   Constructs a new <see cref="InvalidPreDefinedInstanceException"/> caused by a non-public and/or write-only
        ///   property being identified as a pre-defined instance.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the invalid pre-defined instance was encountered.
        /// </param>
        /// <param name="annotation">
        ///   The annotation applied to the non-public property causing it to be identified as a pre-defined instance.
        /// </param>
        public InvalidPreDefinedInstanceException(Context context, IncludeInModelAttribute annotation)
            : base(
                new Location(context.ToString()),
                new Problem("a non-public and/or write-only property cannot be a pre-defined instance"),
                new Annotation(Display.AnnotationDisplayName(annotation.GetType()))
              )
        {}
    }
}
