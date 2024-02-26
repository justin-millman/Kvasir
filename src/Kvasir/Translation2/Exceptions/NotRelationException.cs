using System;

namespace Kvasir.Translation2 {
    /// <summary>
    ///   An exception that is raised when an annotation that is only applicable to Relations is applied to a property
    ///   that is not a Relation.
    /// </summary>
    internal sealed class NotRelationException : TranslationException {
        /// <summary>
        ///   Constructs a new <see cref="NotRelationException"/>.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the annotation applicable only to Relations was encountered.
        /// </param>
        /// <param name="propertyType">
        ///   The type of the property to which the annotation was applied.
        /// </param>
        /// <param name="annotationType">
        ///   The type of the improperly applied annotation.
        /// </param>
        public NotRelationException(Context context, Type propertyType, Type annotationType)
            : base(
                new Location(context.ToString()),
                new Problem($"the property type {propertyType.DisplayName()} is not a Relation"),
                new Annotation(Display.AnnotationDisplayName(annotationType))
              )
        {}
    }
}
