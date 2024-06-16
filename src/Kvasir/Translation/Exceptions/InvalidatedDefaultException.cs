using Cybele.Extensions;
using Kvasir.Annotations;

namespace Kvasir.Translation {
    /// <summary>
    ///   An exception that is raised when a constraint annotation is applied to a Field that causes its default value
    ///   to no longer be valid.
    /// </summary>
    internal sealed class InvalidatedDefaultException : TranslationException {
        /// <summary>
        ///   Constructs a new <see cref="InvalidatedDefaultException"/>.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which <paramref name="annotation"/> was encountered.
        /// </param>
        /// <param name="defaultValue">
        ///   The default value that has been invalidated.
        /// </param>
        /// <param name="annotation">
        ///   The constraint annotation that invalidated <paramref name="defaultValue"/>.
        /// </param>
        public InvalidatedDefaultException(Context context, object? defaultValue, INestableAnnotation annotation)
            : base(
                new Location(context.ToString()),
                new Path(annotation.Path),
                new Problem($"the Field's default value of {defaultValue.ForDisplay()} does not pass the constraint"),
                new Annotation(Display.AnnotationDisplayName(annotation.GetType()))
              )
        {}
    }
}
