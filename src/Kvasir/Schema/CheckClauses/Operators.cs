namespace Kvasir.Schema {
    /// <summary>
    ///   An enumeration representing the logical binary comparison operators.
    /// </summary>
    public enum ComparisonOperator : byte {
        /// <summary>The equality operator.</summary>
        EQ,

        /// <summary>The non-equality operator.</summary>
        NE,

        /// <summary>The strictly less than operator.</summary>
        LT,

        /// <summary>The strictly greater than operator.</summary>
        GT,

        /// <summary>The less than or equal to operator.</summary>
        LTE,

        /// <summary>The greater than or equal to operator.</summary>
        GTE
    }

    /// <summary>
    ///   An enumeration representing the logical inclusion operators, i.e. checking whether or not a particular item
    ///   appears in a list.
    /// </summary>
    public enum InclusionOperator : byte {
        /// <summary>The does-include operator.</summary>
        In,

        /// <summary>The does-not-include operator.</summary>
        NotIn
    }

    /// <summary>
    ///   An enumeration representing the logical <c>NULL</c>-check operators.
    /// </summary>
    public enum NullityOperator : byte {
        /// <summary>The is-<c>NULL</c> operator.</summary>
        IsNull,

        /// <summary>The is-not-<c>NULL</c> operator.</summary>
        IsNotNull
    }
}
