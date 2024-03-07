using System.Reflection;

namespace Kvasir.Translation2 {
    /// <summary>
    ///   An exception that is raised when an impermissible referential cycle is detected.
    /// </summary>
    internal sealed class ReferenceCycleException : TranslationException {
        /// <summary>
        ///   Constructs a new <see cref="ReferenceCycleException"/>.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the reference cycle was detected.
        /// </param>
        /// <param name="finalLink">
        ///   The property that "completes" the reference cycle.
        /// </param>
        public ReferenceCycleException(Context context, PropertyInfo finalLink)
            : base(
                new Location(context.ToString()),
                new Problem($"the property '{finalLink.Name}' causes a reference cycle")
              )
        {}
    }
}
