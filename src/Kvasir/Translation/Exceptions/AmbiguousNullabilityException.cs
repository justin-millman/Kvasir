using Kvasir.Annotations;

namespace Kvasir.Translation {
    /// <summary>
    ///   An exception that is raised when the nullability of an Aggregate property causes an ambiguity.
    /// </summary>
    internal sealed class AmbiguousNullabilityException : TranslationException {
        /// <summary>
        ///   Constructs a new <see cref="AmbiguousNullabilityException"/> caused by a natively nullable Aggregate
        ///   property.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the ambiguously nullable Aggregate was encountered.
        /// </param>
        public AmbiguousNullabilityException(Context context)
            : base(
                new Location(context.ToString()),
                new Problem("the property cannot be nullable because all nested Fields are already nullable")
              )
        {}

        /// <summary>
        ///   Constructs a new <see cref="AmbiguousNullabilityException"/> caused by an Aggregate property that is
        ///   directly or indirectly marked with a <see cref="NullableAttribute">[Nullable]</see> annotation.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the ambiguously nullable Aggregate was encountered.
        /// </param>
        /// <param name="_">
        ///   <i>overload discriminator</i>
        /// </param>
        public AmbiguousNullabilityException(Context context, NullableAttribute _)
            : base(
                new Location(context.ToString()),
                new Problem("the property cannot be nullable because all nested Fields are already nullable"),
                new Annotation(Display.AnnotationDisplayName(typeof(NullableAttribute)))
              )
        {}
    }
}
