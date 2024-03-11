using Kvasir.Annotations;

namespace Kvasir.Translation2 {
    /// <summary>
    ///   An exception that is raised when a <see cref="CheckAttribute">[Check]</see> or
    ///   <see cref="Check.ComplexAttribute">[Check.Complex]</see> annotation is invalid.
    /// </summary>
    internal sealed class InvalidCustomConstraintException : TranslationException {
        /// <summary>
        ///   Constructs a new <see cref="InvalidCustomConstraintException"/> describing a "user error" on a
        ///   <see cref="CheckAttribute">[Check]</see> annotation, such as if the type provided to the annotation is not
        ///   actually an IConstraintGenerator.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the invalid annotation was encountered.
        /// </param>
        /// <param name="annotation">
        ///   The <see cref="CheckAttribute">[Check]</see> annotation that carries a "user error."
        /// </param>
        public InvalidCustomConstraintException(Context context, CheckAttribute annotation)
            : base(
                new Location(context.ToString()),
                new Path(annotation.Path),
                new Problem(annotation.UserError!),
                new Annotation(nameof(CheckAttribute)[..^9])
              )
        {}
    }
}

