using Cybele.Extensions;
using System;
using System.Diagnostics;

namespace Kvasir.Schema.Constraints {
    /// <summary>
    ///   A representation of the logical comparison operators
    /// </summary>
    public enum ComparisonOperator : byte {
        /// <summary>The <c>==</c> comparison operator</summary>
        Equal,

        /// <summary>The <c>!=</c> comparison operator</summary>
        NotEqual,

        /// <summary>The <c>&lt;</c> comparison operator</summary>
        LessThan,

        /// <summary>The <c>&gt;</c> comparison operator</summary>
        GreaterThan,

        /// <summary>The <c>&lt;=</c> comparison operator</summary>
        LessThanOrEqual,

        /// <summary>The <c>&gt;=</c> comparison operator</summary>
        GreaterThanOrEqual
    }

    /// <summary>
    ///   A representation of the SQL inclusion operators
    /// </summary>
    public enum InclusionOperator : byte {
        /// <summary>The logical inclusion operator</summary>
        In,

        /// <summary>The logical exclusion operator</summary>
        NotIn
    }

    /// <summary>
    ///   A representation of the SQL nullity operators
    /// </summary>
    public enum NullityOperator : byte {
        /// <summary>The logical is-NULL operator</summary>
        IsNull,

        /// <summary>The logical is-not-NULL operator</summary>
        IsNotNull
    }

    /// <summary>
    ///   A collection of extension methods that operate on Constrant operators.
    /// </summary>
    internal static class OperatorExtensions {
        /// <summary>
        ///   Produces the negation of a <see cref="ComparisonOperator"/>.
        /// </summary>
        /// <param name="op">
        ///   The <see cref="ComparisonOperator"/> to negate.
        /// </param>
        /// <returns>
        ///   The negation of <paramref name="op"/>; that is, the operator <c>~</c> such that if
        ///   <c>X <paramref name="op"/> Y</c> is <see langword="true"/> then <c>X ~ Y</c> is guaranteed to be
        ///   <see langword="false"/>.
        /// </returns>
        public static ComparisonOperator Negation(this ComparisonOperator op) {
            Debug.Assert(op.IsValid());

            return op switch {
                ComparisonOperator.Equal => ComparisonOperator.NotEqual,
                ComparisonOperator.NotEqual => ComparisonOperator.Equal,
                ComparisonOperator.LessThan => ComparisonOperator.GreaterThanOrEqual,
                ComparisonOperator.GreaterThan => ComparisonOperator.LessThanOrEqual,
                ComparisonOperator.LessThanOrEqual => ComparisonOperator.GreaterThan,
                ComparisonOperator.GreaterThanOrEqual => ComparisonOperator.LessThan,
                _ => throw new ApplicationException()
            };
        }
    }
}
