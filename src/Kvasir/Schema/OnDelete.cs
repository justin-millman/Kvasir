namespace Kvasir.Schema {
    /// <summary>
    ///   An enumeration representing the various <c>ON DELETE</c> behaviors that can be applied to a Foreign Key.
    /// </summary>
    /// <remarks>
    ///   Foreign Keys are intended to enforce data integrity across multiple Tables, ensuring that a value (or tuple
    ///   of values) in one Table references a row in another. <c>DELETE</c> operations can therefore disrupt the
    ///   validity of Foreign Keys, as the row(s) being deleted can feasibly be one referenced by a Foreign Key in
    ///   another Table. When this happens, the behavior is controlled by the <c>ON DELETE</c> behavior of the Foreign
    ///   Key that referenes the to-be-deleted row(s). If multiple Foreign Keys reference the targeted row(s) with
    ///   different <c>ON DELETE</c> behavior, the result depends on the back-end RDBMS provider being used.
    /// </remarks>
    public enum OnDelete : byte {
        /// <summary>
        ///   Prevent the deletion of a row that is referenced by a row in another Table via a Foreign Key. This
        ///   behavior requires that the <i>referencing</i> data be modified or removed prior to removal of the
        ///   <i>referenced</i> data. This is the strictest behavior.
        /// </summary>
        Prevent,

        /// <summary>
        ///   When a row that is referenced by a row in another Table via a Foreign Key is deleted, also delete the
        ///   referencing row(s). This behavior turns a single <c>DELETE</c> statement targeting a single Table into a
        ///   collection of <c>DELETE</c> statements targeting multiple Tables, as the referencing row(s) being deleted
        ///   can themselves be <i>referenced</i> rows in another Foreign Key.
        /// </summary>
        Cascade,

        /// <summary>
        ///   When a row that is referenced by a row in another Table via a Foreign Key is deleted, set the values of
        ///   any referencing Fields to <c>NULL</c>. For obvious reasons, this behavior can only be applied to a
        ///   Foreign Key where all the constituent Fields are nullable.
        /// </summary>
        SetNull,

        /// <summary>
        ///   When a rwo that is referenced by a row in another Table via a Foreign Key is deleted, set the valus of
        ///   any referencing Fields to their default value. For obvious reasons, this behavior can only be applied to
        ///   a Foreign Key where all the constituent Fields have explicitly specified defalut values.
        /// </summary>
        SetDefault,

        /// <summary>
        ///   Take no action when a row being deleted is referenced by a row in another Table via a Foreign Key. This
        ///   behavior can lead to data integrity issues, as the referencing row no longer refers to an extant target.
        ///   This is the weakest behavior. Some back-end RDBMS providers will raise errors or exceptions when such
        ///   data integrity violations arse.
        /// </summary>
        NoAction,
    }
}
