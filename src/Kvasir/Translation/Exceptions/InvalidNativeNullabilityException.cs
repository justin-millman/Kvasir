namespace Kvasir.Translation {
    /// <summary>
    ///   An exception that is raised when a property that cannot be natively nullable is, and the property is not
    ///   marked with a <c>[NonNullable]</c> annotation.
    /// </summary>
    internal sealed class InvalidNativeNullabilityException : TranslationException {
        /// <summary>
        ///   Constructs a new <see cref="NotRelationException"/>.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the invalid natively nullable property was encountered.
        /// </param>
        /// <param name="description">
        ///   A brief description of what the property that cannot be natively nullable actually is.
        /// </param>
        public InvalidNativeNullabilityException(Context context, string description)
            : base(
                new Location(context.ToString()),
                new Problem($"{description} cannot be nullable")
              )
        {}
    }
}
