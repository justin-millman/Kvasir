namespace Kvasir.Translation {
    /// <summary>
    ///   An exception that is raised when an invalid type is identified as an Entity, either via a direct request for
    ///   translation or because it is the backing type of a translated property.
    /// </summary>
    internal sealed class InvalidEntityTypeException : TranslationException {
        /// <summary>
        ///   Constructs a new <see cref="InvalidEntityTypeException"/> triggered by the actual CLR type.
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

        /// <summary>
        ///   Constructs a new <see cref="InvalidEntityTypeException"/> triggered by a Pre-Defined Entity type that has
        ///   a public constructor.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the invalid Pre-Defined Entity type was encountered
        /// </param>
        /// <param name="_">
        ///   <i>overload discriminator</i>
        /// </param>
        public InvalidEntityTypeException(Context context, PreDefinedTag _)
            : base(
                new Location(context.ToString()),
                new Problem("a Pre-Defined Entity cannot have a public constructor")
              )
        {}
    }
}
