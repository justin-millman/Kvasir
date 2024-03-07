using Kvasir.Annotations;

namespace Kvasir.Translation2 {
    /// <summary>
    ///   An exception that is raised when the combination of one or more constraints leaves no valid values for a
    ///   Field.
    /// </summary>
    internal sealed class UnsatisfiableConstraintException : TranslationException {
        ///
        public UnsatisfiableConstraintException(Context context, INestableAnnotation annotation)
            : base(
                new Location(context.ToString()),
                new Path(annotation.Path),
                new Problem("all of the explicitly allowed values are disallowed by the constraint"),
                new Annotation(annotation.GetType().Name[..^9])
              )
        {}

        ///
        public UnsatisfiableConstraintException(Context context, INestableAnnotation annotation, Interval interval)
            : base(
                new Location(context.ToString()),
                new Path(annotation.Path),
                new Problem(
                    $"the interval {interval} of " +
                    (annotation is Check.StringLengthAttribute ? "valid string lengths " : "allowed values ") +
                    "is empty"
                ),
                new Annotation(annotation.GetType().Name[..^9])
              )
        {}

        ///
        public UnsatisfiableConstraintException(Context context, INestableAnnotation annotation, string reason)
            : base(
                new Location(context.ToString()),
                new Path(annotation.Path),
                new Problem(reason),
                new Annotation(annotation.GetType().Name[..^9]))
        {}
    }
}
