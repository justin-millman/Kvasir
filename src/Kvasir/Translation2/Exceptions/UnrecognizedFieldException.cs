using Kvasir.Annotations;

namespace Kvasir.Translation2 {
    /// <summary>
    ///   An exception that is thrown when one of the Field names provided to a
    ///   <see cref="Check.ComplexAttribute"><c>[Check.Complex]</c></see> annotation cannot be matched to a Field on the
    ///   Entity's Primary Table.
    /// </summary>
    internal sealed class UnrecognizedFieldException : TranslationException {
        /// <summary>
        ///   Constructs a new <see cref="UnrecognizedFieldException"/>.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the <see cref="Check.ComplexAttribute"><c>[Check.Complex]</c></see>
        ///   annotation with the unrecognized Field was encountered.
        /// </param>
        /// <param name="fieldName">
        ///   The unrecognized Field name.
        /// </param>
        public UnrecognizedFieldException(Context context, string fieldName)
            : base(
                new Location(context.ToString()),
                new Problem($"no Field named \"{fieldName}\" exists on the Table"),
                new Annotation(Display.AnnotationDisplayName(typeof(Check.ComplexAttribute)))
              )
        {}
    }
}
