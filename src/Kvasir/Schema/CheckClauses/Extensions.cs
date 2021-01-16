using Cybele.Extensions;
using System;
using System.Diagnostics;

namespace Kvasir.Schema {
    /// <summary>
    ///   A collection of <see href="https://tinyurl.com/y8q6ojue">extension methods</see> that extend types involved
    ///   in creating concrete <see cref="Clause">Clauses</see>.
    /// </summary>
    internal static class ClauseExtensions {
        /// <summary>
        ///   Produces the <see cref="ComparisonOperator"/> that is logically opposite another.
        /// </summary>
        /// <param name="self">
        ///   The <see cref="ComparisonOperator"/> on which the extension method is invoked.
        /// </param>
        /// <returns>
        ///   The logical opposite of <paramref name="self"/>.
        /// </returns>
        public static ComparisonOperator Negation(this ComparisonOperator self) {
            Debug.Assert(self.IsValid());

            return self switch {
                ComparisonOperator.EQ  => ComparisonOperator.NE,
                ComparisonOperator.NE  => ComparisonOperator.EQ,
                ComparisonOperator.LT  => ComparisonOperator.GTE,
                ComparisonOperator.GT  => ComparisonOperator.LTE,
                ComparisonOperator.LTE => ComparisonOperator.GT,
                ComparisonOperator.GTE => ComparisonOperator.LT,
                _ => throw new ApplicationException($"Switch statement over {nameof(ComparisonOperator)} exhausted"),
            };
        }

        /// <summary>
        ///   Produces the <see cref="InclusionOperator"/> that is logically opposite another.
        /// </summary>
        /// <param name="self">
        ///   The <see cref="InclusionOperator"/> on which the extension method is invoked.
        /// </param>
        /// <returns>
        ///   The logical opposite of <paramref name="self"/>.
        /// </returns>
        public static InclusionOperator Negation(this InclusionOperator self) {
            Debug.Assert(self.IsValid());

            return self switch {
                InclusionOperator.In    => InclusionOperator.NotIn,
                InclusionOperator.NotIn => InclusionOperator.In,
                _ => throw new ApplicationException($"Switch statement over {nameof(InclusionOperator)} exhausted")
            };
        }

        /// <summary>
        ///   Produces the <see cref="NullityOperator"/> that is logically opposite another.
        /// </summary>
        /// <param name="self">
        ///   The <see cref="NullityOperator"/> on which the extension method is invoked.
        /// </param>
        /// <returns>
        ///   The logical opposite of <paramref name="self"/>.
        /// </returns>
        public static NullityOperator Negation(this NullityOperator self) {
            Debug.Assert(self.IsValid());

            return self switch {
                NullityOperator.IsNull    => NullityOperator.IsNotNull,
                NullityOperator.IsNotNull => NullityOperator.IsNull,
                _ => throw new ApplicationException($"Switch statement over {nameof(NullityOperator)} exhausted")
            };
        }
    }
}
