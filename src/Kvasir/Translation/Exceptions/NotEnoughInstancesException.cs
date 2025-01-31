namespace Kvasir.Translation {
    /// <summary>
    ///   An exception that is raised when a Pre-Defined Entity type does not expose enough instances.
    /// </summary>
    internal sealed class NotEnoughInstancesException : TranslationException {
        /// <summary>
        ///   Constructs a new <see cref="NotEnoughInstancesException"/>.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the underpopulated type was encountered.
        /// </param>
        /// <param name="minExpected">
        ///   The minimum number of pre-defined instances that were expected to be exposed.
        /// </param>
        /// <param name="actual">
        ///   The actual number of pre-defined instances that were exposed.
        /// </param>
        public NotEnoughInstancesException(Context context, int minExpected, int actual)
            : base(
                new Location(context.ToString()),
                new Problem($"expected at least {minExpected} pre-defined instance{(minExpected > 1 ? "s" : "")}, but found {actual}")
              )
        {}
    }
}
