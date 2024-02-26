namespace Kvasir.Translation2 {
    /// <summary>
    ///   An exception that is raised when an impermissible referential cycle is detected.
    /// </summary>
    internal sealed class ReferenceCycleException : TranslationException {
        /// <summary>
        ///   Constructs a new <see cref="ReferenceCycleException"/>.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the reference cycle was detected, consisting of the full cycle.
        /// </param>
        public ReferenceCycleException(Context context)
            : base(
                new Location(context.ToString()),
                new Problem($"reference cycle detected")
              )
        {}
    }
}
