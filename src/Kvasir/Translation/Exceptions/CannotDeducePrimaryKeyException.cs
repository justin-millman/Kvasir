namespace Kvasir.Translation {
    /// <summary>
    ///   An exception that is raised when the Primary Key for a table cannot be determined.
    /// </summary>
    internal sealed class CannotDeducePrimaryKeyException : TranslationException {
        /// <summary>
        ///   Constructs a new <see cref="CannotDeducePrimaryKeyException"/>.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the failed deduction occurred.
        /// </param>
        public CannotDeducePrimaryKeyException(Context context)
            : base(
                new Location(context.ToString()),
                new Problem("the Primary Key for the Table could not be deduced")
              )
        {}
    }
}
