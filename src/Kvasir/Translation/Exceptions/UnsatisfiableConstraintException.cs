using Kvasir.Annotations;

namespace Kvasir.Translation {
    /// <summary>
    ///   An exception that is raised when the combination of one or more constraints leaves no valid values for a
    ///   Field.
    /// </summary>
    internal sealed class UnsatisfiableConstraintException : TranslationException {
        /// <summary>
        ///   Constructs a <see cref="UnsatisfiableConstraintException"/> that arises when a constraint causes each of
        ///   the explicitly allowed values of a Field to be invalid.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which <paramref name="annotation"/> was encountered.
        /// </param>
        /// <param name="annotation">
        ///   The annotation that invalidated all of the explicitly allowed value.
        /// </param>
        public UnsatisfiableConstraintException(Context context, INestableAnnotation annotation)
            : base(
                new Location(context.ToString()),
                new Path(annotation.Path),
                new Problem("all of the explicitly allowed values fail at least one other constraint")
              )
        {}

        /// <summary>
        ///   Constructs a <see cref="UnsatisfiableConstraintException"/> that arises when a constraint produces an
        ///   empty interval.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which <paramref name="annotation"/> was encountered.
        /// </param>
        /// <param name="annotation">
        ///   The annotation that produced <paramref name="interval"/>.
        /// </param>
        /// <param name="interval">
        ///   The empty interval.
        /// </param>
        public UnsatisfiableConstraintException(Context context, INestableAnnotation annotation, Interval interval)
            : base(
                new Location(context.ToString()),
                new Path(annotation.Path),
                new Problem(
                    $"the interval {interval} of " +
                    (annotation is Check.StringLengthAttribute ? "valid string lengths " : "allowed values ") +
                    "is empty"
                ),
                new Annotation(Display.AnnotationDisplayName(annotation.GetType()))
              )
        {}

        /// <summary>
        ///   Constructs a <see cref="UnsatisfiableConstraintException"/> that arises for an arbitrary reason.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which <paramref name="annotation"/> was encountered.
        /// </param>
        /// <param name="annotation">
        ///   The annotation.
        /// </param>
        /// <param name="reason">
        ///   The reason that <paramref name="annotation"/> is unsatisfiable.
        /// </param>
        public UnsatisfiableConstraintException(Context context, INestableAnnotation annotation, string reason)
            : base(
                new Location(context.ToString()),
                new Path(annotation.Path),
                new Problem(reason),
                new Annotation(Display.AnnotationDisplayName(annotation.GetType()))
              )
        {}
    }
}
