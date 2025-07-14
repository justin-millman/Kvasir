using System;

namespace Kvasir.Translation {
    /// <summary>
    ///   An exception that is raised when the type of a localization key is not a primitive, built-in type.
    /// </summary>
    /// <seealso cref="NestedLocalizationException"/>
    internal sealed class InvalidLocalizationKeyException : TranslationException {
        /// <summary>
        ///   Constructs a new <see cref="InvalidLocalizationKeyException"/> caused by the localization key being
        ///   something other than a primitive, built-in type.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the invalid localization key type was encountered.
        /// </param>
        /// <param name="type">
        ///   The invalid localization key type.
        /// </param>
        /// <param name="category">
        ///   The category into which the invalid localization key type falls.
        /// </param>
        public InvalidLocalizationKeyException(Context context, Type type, TypeCategory category)
            : base(
                new Location(context.ToString()),
                new Problem($"type {type.DisplayName()} is {category} and cannot be the type of a Localization Key")
              )
        {}
    }
}