namespace Kvasir.Translation {
    /// <summary>
    ///   An exception that is raised when the Fields that comprise the data model of a type cannot be placed into
    ///   consecutive, non-overlapping columns.
    /// </summary>
    internal sealed class CannotAssignColumnsException : TranslationException {
        /// <summary>
        ///   Constructs a new <see cref="CannotAssignColumnsException"/>.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the failed column assignment occurred.
        /// </param>
        /// <param name="reason">
        ///   The reason for the failed column assignment.
        /// </param>
        public CannotAssignColumnsException(Context context, string reason)
            : base(
                new Location(context.ToString()),
                new Problem(reason)
              )
        {}
    }
}
