namespace Kvasir.Translation {
    /// <summary>
    ///   An exception that is raised when the locale type or value type of a Localization is itself, another
    ///   Localization.
    /// </summary>
    /// <seealso cref="InvalidLocalizationKeyException"/>
    internal sealed class NestedLocalizationException : TranslationException {
        /// <summary>
        ///   Constructs a new <see cref="NestedLocalizationException"/>.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the nested Localization was encountered.
        /// </param>
        public NestedLocalizationException(Context context)
            : base(
                new Location(context.ToString()),
                new Problem("nested Localizations (i.e. within another Localization) are not supported")
              )
        {}
    }
}