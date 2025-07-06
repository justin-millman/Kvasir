namespace Kvasir.Translation {
    /// <summary>
    ///   An exception that is raised when the element type of a Relation container or a Localization container is
    ///   itself, or contains, another Localization.
    /// </summary>
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
                new Problem("nested Localizations (including within Relations) are not supported")
              )
        {}
    }
}
