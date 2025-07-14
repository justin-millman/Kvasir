using Cybele.Extensions;
using System;

namespace Kvasir.Translation {
    /// <summary>
    ///   An exception that is raised when a Pre-Defined Entity references or "owns" a Relation involving a
    ///   non-Pre-Defined Entity.
    /// </summary>
    internal sealed class PreDefinedReferenceException : TranslationException {
        /// <summary>
        ///   Constructs a new <see cref="PreDefinedReferenceException"/>.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the invalid property type was encountered.
        /// </param>
        /// <param name="entityType">
        ///   The non-Pre-Defined Entity type.
        /// </param>
        /// <param name="isReference">
        ///   <see langword="true"/> if the invalid property was a Reference-type property; if it was a Relation-type
        ///   property, then <see langword="false"/>.
        /// </param>
        public PreDefinedReferenceException(Context context, Type entityType,  bool isReference)
            : base(
                new Location(context.ToString()),
                new Problem(
                    "a Pre-Defined Entity cannot " +
                    (isReference ? "reference " : "contain a Relation or a Localization involving ") +
                    $"non-Pre-Defined Entity type {entityType.DisplayName()}"
                )
              )
        {}
    }
}
