using Kvasir.Annotations;

namespace Kvasir.Translation {
    /// <summary>
    ///   An exception that is raised when a type cannot be Reconstituted.
    /// </summary>
    internal sealed class ReconstitutionNotPossibleException : TranslationException {
        /// <summary>
        ///   Constructs a new <see cref="ReconstitutionNotPossibleException"/> caused by a type not having any viable
        ///   constructors.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the invalid annotation was encountered.
        /// </param>
        public ReconstitutionNotPossibleException(Context context)
            : base(
                new Location(context.ToString()),
                new Problem("there are no viable constructors")
              )
        {}

        /// <summary>
        ///   Constructs a new <see cref="ReconstitutionNotPossibleException"/> cased by a
        ///   <see cref="ReconstituteThroughAttribute">[ReconstituteThrough] annotation</see> being placed on a
        ///   non-viable constructor.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the invalid annotation was encountered.
        /// </param>
        /// <param name="annotation">
        ///   A <see cref="ReconstituteThroughAttribute">[ReconstituteThrough] annotation</see>.
        /// </param>
        public ReconstitutionNotPossibleException(Context context, ReconstituteThroughAttribute annotation)
            : base(
                new Location(context.ToString()),
                new Problem("the constructor is not viable"),
                new Annotation(Display.AnnotationDisplayName(annotation.GetType()))
              )
        {}

        /// <summary>
        ///   Constructs a new <see cref="ReconstitutionNotPossibleException"/> caused by two or more constructors all
        ///   being viable and having equivalent highest precedence.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the invalid annotation was encountered.
        /// </param>
        /// <param name="numViableConstructors">
        ///   The number of constructors that are equally the most viable.
        /// </param>
        public ReconstitutionNotPossibleException(Context context, int numViableConstructors)
            : base(
                new Location(context.ToString()),
                new Problem($"{numViableConstructors} constructors are all equally the most viable")
              )
        {}

        /// <summary>
        ///   Constructs a new <see cref="ReconstitutionNotPossibleException"/> caused by two or more constructors
        ///   (viable or non-viable) being annotated with
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the invalid annotation was encountered.
        /// </param>
        /// <param name="annotation">
        ///   A <see cref="ReconstituteThroughAttribute">[ReconstituteThrough] annotation</see>.
        /// </param>
        /// <param name="numAnnotations">
        ///   The number of constructors that are annotated.
        /// </param>
        public ReconstitutionNotPossibleException(Context context, ReconstituteThroughAttribute annotation, int numAnnotations)
            : base(
                new Location(context.ToString()),
                new Problem($"at most 1 constructor can be annotated, but found {numAnnotations}"),
                new Annotation(Display.AnnotationDisplayName(annotation.GetType()))
              )
        {}

        /// <summary>
        ///   Constructs a new <see cref="ReconstitutionNotPossibleException"/> cased by a
        ///   <see cref="ReconstituteThroughAttribute">[ReconstituteThrough] annotation</see> being placed on a
        ///   constructor of a Pre-Defined Entity.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the invalid annotation was encountered.
        /// </param>
        /// <param name="annotation">
        ///   A <see cref="ReconstituteThroughAttribute">[ReconstituteThrough] annotation</see>.
        /// </param>
        /// <param name="_">
        ///   <i>overload discriminator</i>
        /// </param>
        public ReconstitutionNotPossibleException(Context context, ReconstituteThroughAttribute annotation, PreDefinedTag _)
            : base(
                new Location(context.ToString()),
                new Problem("Pre-Defined Entities do not support Reconstitution"),
                new Annotation(Display.AnnotationDisplayName(annotation.GetType()))
              )
        {}
    }
}
