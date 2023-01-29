namespace Kvasir.Schema {
    /// <summary>
    ///   An enumeration that specifies whether or not <c>NULL</c> is an acceptable value, particularly for a Field.
    /// </summary>
    public enum IsNullable : byte {
        /// <summary>
        ///   <c>NULL</c> <i>is</i> an acceptable value.
        /// </summary>
        Yes,

        /// <summary>
        ///   <c>NULL</c> is <i>not</i> an acceptable value.
        /// </summary>
        No
    }
}
