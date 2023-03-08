namespace Kvasir.Schema {
    /// <summary>
    ///   An enumeration representing the various <c>ON UPDATE</c> behaviors that can be applied to a Foreign Key.
    /// </summary>
    /// <remarks>
    ///   Foreign Keys are intended to enforce data integrity across multiple Tables, ensuring that a value (or tuple
    ///   of values) in one Table references a row in another. <c>UPDATE</c> operations can therefore disrupt the
    ///   validity of Foreign Keys, as the row(s) being updated can feasibly be one referenced by a Foreign Key in
    ///   another Table. When this happens, the behavior is controlled by the <c>ON UPDATE</c> behavior of the Foreign
    ///   Key that references the to-be-updated row(s). If multiple Foreign Keys reference the targeted row(s) with
    ///   different <c>ON UPDATE</c> behavior, the result depends on the back-end RDBMS provider being used.
    /// </remarks>
    public enum OnUpdate : byte {
        /// <summary>
        ///   Prevent the updating of a row that is referenced by a row in another Table via a Foreign Key. This
        ///   behavior effectively disables updating <i>referenced</i> data unless the <i>referencing</i> data is first
        ///   deleted or updated itself to refer to a different, still-valid target.
        /// </summary>
        Prevent,

        /// <summary>
        ///   When a row that is referenced by a row in another Table via a Foreign Key is updated, also update the
        ///   referencing row(s). This behavior turns a single <c>UPDATE</c> statement targeting a single Table into a
        ///   collection of <c>UPDATE</c> statements targeting multiple Tables, as the referencing row(s) being updated
        ///   can themselves be <i>referenced</i> rows in another Foreign Key.
        /// </summary>
        Cascade,

        /// <summary>
        ///   When a row that is referenced by a row in another Table via a Foreign Key is updated, set the values of
        ///   any referencing Fields to <c>NULL</c>. For obvious reasons, this behavior can only be applied to a
        ///   Foreign Key where all the constituent Fields are nullable.
        /// </summary>
        SetNull,

        /// <summary>
        ///   When a row that is referenced by a row in another Table via a Foreign Key is updated, set the values of
        ///   any referencing Fields to their default value. For obvious reasons, this behavior can only be applied to
        ///   a Foreign Key where all the constituent Fields have explicitly specified default values.
        /// </summary>
        SetDefault,

        /// <summary>
        ///   Take no action when a row being updated is referenced by a row in another Table via a Foreign Key. This
        ///   behavior can lead to data integrity issues, as the referencing row no longer refers to an extant target.
        ///   This is the weakest behavior. Some back-end RDBMS providers will raise errors or exceptions when such
        ///   data integrity violations arise.
        /// </summary>
        NoAction,
    }
}
