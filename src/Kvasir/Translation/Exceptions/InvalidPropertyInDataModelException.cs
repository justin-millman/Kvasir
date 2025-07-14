using System;
using System.Reflection;

namespace Kvasir.Translation {
    /// <summary>
    ///   An exception that is raised when a property that is part of the data model cannot be translated for some
    ///   reason.
    /// </summary>
    internal sealed class InvalidPropertyInDataModelException : TranslationException {
        /// <summary>
        ///   Constructs a new <see cref="InvalidPropertyInDataModelException"/> caused by the property being backed by
        ///   an unsupported type.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the invalid property type was encountered.
        /// </param>
        /// <param name="type">
        ///   The invalid property type.
        /// </param>
        /// <param name="category">
        ///   The category into which the invalid property type falls.
        /// </param>
        public InvalidPropertyInDataModelException(Context context, Type type, TypeCategory category)
            : base(
                new Location(context.ToString()),
                new Problem($"type {type.DisplayName()} is {category} and cannot be the backing type of a property included in the data model")
              )
        {}

        /// <summary>
        ///   Constructs a new <see cref="InvalidPropertyInDataModelException"/> caused the property being fundamentally
        ///   un-translatable (e.g. a write-only property or an indexer).
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the invalid property was encountered.
        /// </param>
        /// <param name="category">
        ///   The category into which the invalid property falls.
        /// </param>
        public InvalidPropertyInDataModelException(Context context, PropertyCategory category)
            : base(
                new Location(context.ToString()),
                new Problem($"{category} cannot be included in the data model, either implicitly or explicitly")
              )
        {}

        /// <summary>
        ///   Constructs a new <see cref="InvalidPropertyInDataModelException"/> caused by the property being backed by
        ///   a type that is from the wrong assembly.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the invalid property type was encountered.
        /// </param>
        /// <param name="type">
        ///   The invalid property type.
        /// </param>
        /// <param name="expected">
        ///   The assembly in which <paramref name="type"/> was expected to have been from.
        /// </param>
        public InvalidPropertyInDataModelException(Context context, Type type, Assembly expected)
            : base(
                new Location(context.ToString()),
                new Problem($"type {type.DisplayName()} comes from assembly {type.Assembly}, not from user assembly {expected}")
              )
        {}

        /// <summary>
        ///   Constructs a new <see cref="InvalidPropertyInDataModelException"/> caused by a writeable property of a
        ///   Pre-Defined Entity.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the writeable property was encountered.
        /// </param>
        /// <param name="_">
        ///   <i>overload discriminator</i>
        /// </param>
        public InvalidPropertyInDataModelException(Context context, PreDefinedTag _)
            : base(
                new Location(context.ToString()),
                new Problem("a writeable property cannot be included in the data model for a Pre-Defined Entity")
              )
        {}

        /// <summary>
        ///   Constructs a new <see cref="InvalidPropertyInDataModelException"/> caused by a property in a derived
        ///   Localization.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the writeable property was encountered.
        /// </param>
        /// <param name="_">
        ///   <i>overload discriminator</i>
        /// </param>
        public InvalidPropertyInDataModelException(Context context, DerivedLocalizationTag _)
            : base(
                new Location(context.ToString()),
                new Problem("a property in a derived Localization class cannot be included in the data model")
              )
        {}
    }

    // Discrimination types
    internal readonly struct DerivedLocalizationTag {}
}
