using System;

namespace Kvasir.Translation {
    /// <summary>
    ///   An exception that is raised when a Relation or Localization that should be read-only (because it is part of a
    ///   Pre-Defined Entity) but is not.
    /// </summary>
    internal sealed class NotReadOnlyException : TranslationException {
        /// <summary>
        ///   Constructs a new <see cref="NotReadOnlyException"/> for a Relation on a Pre-Defined Entity that is not
        ///   read-only.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> at which the problematic Relation was encountered.
        /// </param>
        public NotReadOnlyException(Context context)
            : base(
                new Location(context.ToString()),
                new Problem("a Relation on a Pre-Defined Entity must be read-only")
              )
        {}

        /// <summary>
        ///   Constructs a new <see cref="NotReadOnlyException"/> for a Pre-Defined Localization taht is not read-only.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> at which the problematic Localization type was encountered.
        /// </param>
        /// <param name="_">
        ///   [tag dispatch]
        /// </param>
        public NotReadOnlyException(Context context, Type _)
            : base(
                new Location(context.ToString()),
                new Problem("a Pre-Defined Localization must be read-only")
              )
        {}
    }
}
