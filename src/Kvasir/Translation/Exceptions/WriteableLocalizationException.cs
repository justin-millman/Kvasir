namespace Kvasir.Translation {
    /// <summary>
    ///   An exception that is raised when a freely writeable Localization-type property is included in the data model.
    /// </summary>
    internal sealed class WriteableLocalizationException : TranslationException {
        /// <summary>
        ///   Constructs a new <see cref="WriteableLocalizationException"/>.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the freely writeable Localization-type property was encountered.
        /// </param>
        public WriteableLocalizationException(Context context)
            : base(
                new Location(context.ToString()),
                new Problem("if a Localization-type property has a setter, is must be init-only")
              )
        {}
    }
}