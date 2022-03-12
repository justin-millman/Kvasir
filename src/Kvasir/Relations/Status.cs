namespace Kvasir.Relations {
    /// <summary>
    ///   An enumeration indicating the state of an entry in a relational collection relative to the back-end database.
    /// </summary>
    internal enum Status {
        /// <summary>The relation connection has yet to be written to the back-end database.</summary>
        New,

        /// <summary>The relation connection is already in the back-end database.</summary>
        Saved,

        /// <summary>The relation connection needs to be deleted from the back-end database.</summary>
        Deleted,

        /// <summary>The relation connection has been modified relative to the entry in the back-end datbase.</summary>
        /// <remarks>This status is generally useful only for ordered collections, to indicate a reordering.</remarks>
        Modified
    }
}
