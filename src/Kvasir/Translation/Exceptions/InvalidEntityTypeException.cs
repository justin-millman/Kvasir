namespace Kvasir.Translation {
    /// <summary>
    ///   An exception that is raised when an invalid type is identified as an Entity, either via a direct request for
    ///   translation or because it is the backing type of a translated property.
    /// </summary>
    internal sealed class InvalidEntityTypeException : TranslationException {
        /// <summary>
        ///   Constructs a new <see cref="InvalidEntityTypeException"/>.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the invalid Entity type was encountered
        /// </param>
        /// <param name="category">
        ///   The category into which the invalid Entity type falls.
        /// </param>
        public InvalidEntityTypeException(Context context, TypeCategory category)
            : base(
                new Location(context.ToString()),
                new Problem($"{category} cannot be an Entity type or the backing type of a Reference property")
              )
        {}
    }
}
