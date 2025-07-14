using Cybele.Extensions;
using Kvasir.Localization;
using System.Diagnostics;
using System.Reflection;

namespace Kvasir.Translation {
    /// <summary>
    ///   A utility for tracking the presence of Localizations.
    /// </summary>
    internal sealed class LocalizationTracker {
        /// <summary>
        ///   The source property.
        /// </summary>
        public PropertyInfo Source { get; }

        /// <summary>
        ///   The <see cref="Context"/> in which the property sourcing the tracker was encountered.
        /// </summary>
        public Context Context { get; }

        /// <summary>
        ///   Constructs a new <see cref="LocalizationTracker"/>.
        /// </summary>
        /// <param name="source">
        ///   The source property.
        /// </param>
        /// <param name="context">
        ///   The <see cref="Context"/> of <paramref name="source"/>.
        /// </param>
        public LocalizationTracker(PropertyInfo source, Context context) {
            Debug.Assert(source is not null);
            Debug.Assert(context is not null);
            Debug.Assert(source.PropertyType.IsInstanceOf(typeof(ILocalization)) && source.PropertyType != typeof(ILocalization));

            Source = source;
            Context = context.Clone();
        }
    }
}
