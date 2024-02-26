using Kvasir.Annotations;

namespace Kvasir.Translation2 {
    /// <summary>
    ///   An exception that is raised when the value provided to a <see cref="DefaultAttribute">[Default]</see>
    ///   annotation is not valid for the Field to which the annotation applies.
    /// </summary>
    internal sealed class InvalidDefaultException : TranslationException {
        /// <summary>
        ///   Constructs a new <see cref="InvalidDefaultException"/>.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the invalid annotation was encountered.
        /// </param>
        /// <param name="path">
        ///   The path to which the problematic <see cref="DefaultAttribute"/> applies.
        /// </param>
        /// <param name="reason">
        ///   An explanation as to why the default value is invalid.
        /// </param>
        public InvalidDefaultException(Context context, string path, string reason)
            : base(
                new Location(context.ToString()),
                new Path(path),
                new Problem(reason),
                new Annotation(Display.AnnotationDisplayName(typeof(DefaultAttribute)))
              )
        {}
    }
}
