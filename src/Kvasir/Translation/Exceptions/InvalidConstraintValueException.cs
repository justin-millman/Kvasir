using Kvasir.Annotations;

namespace Kvasir.Translation {
    /// <summary>
    ///   An exception that is raised when a value provided to a constraint annotation is not valid for the Field to
    ///   which the constraint annotation applies.
    /// </summary>
    internal sealed class InvalidConstraintValueException : TranslationException {
        /// <summary>
        ///   Constructs a new <see cref="InvalidConstraintValueException"/>.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which <paramref name="annotation"/> was encountered.
        /// </param>
        /// <param name="annotation">
        ///   The invalid constraint annotation.
        /// </param>
        /// <param name="reason">
        ///   An explanation as to why the default value is invalid.
        /// </param>
        public InvalidConstraintValueException(Context context, INestableAnnotation annotation, string reason)
            : base(
                new Location(context.ToString()),
                new Path(annotation.Path),
                new Problem(reason),
                new Annotation(Display.AnnotationDisplayName(annotation.GetType()))
              )
        {}
    }
}
