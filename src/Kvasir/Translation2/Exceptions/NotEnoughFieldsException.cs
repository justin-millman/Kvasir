namespace Kvasir.Translation2 {
    /// <summary>
    ///   An exception that is raised when a type does not contribute enough Fields to the data model.
    /// </summary>
    internal sealed class NotEnoughFieldsException : TranslationException {
        /// <summary>
        ///   Constructs a new <see cref="NotEnoughFieldsException"/>.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the underpopulated type was encountered.
        /// </param>
        /// <param name="minExpected">
        ///   The minimum number of Fields that were expected to be contributed to the data model.
        /// </param>
        /// <param name="actual">
        ///   The actual number of Fields that were contributed to the data model.
        /// </param>
        public NotEnoughFieldsException(Context context, int minExpected, int actual)
            : base(
                new Location(context.ToString()),
                new Problem($"expected at least {minExpected} Field{(minExpected > 1 ? "s" : "")}, but found {actual}")
              )
        {}
    }
}
