using Cybele.Extensions;
using Optional;
using System.Diagnostics;

namespace Kvasir.Translation {
    /// <summary>
    ///   A basic representation of a bound (or an endpoint) on an interval.
    /// </summary>
    /// <param name="Value">
    ///   The value of the <see cref="Bound"/>. Note that "infinity" and "negative infinity" cannot be represented.
    /// </param>
    /// <param name="IsInclusive">
    ///   If <see langword="true"/>, then the <see cref="Bound"/> represents an inclusive (or "closed") endpoint;
    ///   otherwise, it represents an exclusive (or "open") endpoint.
    /// </param>
    internal readonly record struct Bound(object Value, bool IsInclusive);

    internal sealed partial class Translator {
        /// <summary>
        ///   Determine the smaller of two <see cref="Bound">upper bounds</see>, one of which may not actually exist.
        /// </summary>
        /// <param name="lhs">
        ///   The first of the two <see cref="Bound">upper bounds</see>, which may be a <c>NONE</c> instance.
        /// </param>
        /// <param name="rhs">
        ///   The second of the two <see cref="Bound">upper bounds</see>.
        /// </param>
        /// <returns>
        ///   The smaller of <paramref name="lhs"/> and <paramref name="rhs"/>. If <paramref name="lhs"/> is a
        ///   <c>NONE</c> instance, this is tautologically <paramref name="rhs"/>. Otherwise, it is the argument with
        ///   the smaller <see cref="Bound.Value">value</see>. If both bounds' value are the same, an exclusive bound is
        ///   considered "smaller" than an inclusive bound.
        /// </returns>
        private static Bound MinUpperBound(Option<Bound> lhs, Bound rhs) {
            if (!lhs.HasValue) {
                return rhs;
            }

            // We use dynamic here so that we can easily use the operators to determine precedence, since the values are
            // already type-erased and there's no other great way to do it; we know, given the set of possible dynamic
            // types, that the CompareTo function will exist (we have to use this because strings do not have comparison
            // operators defined)
            dynamic lhsValue = lhs.Unwrap().Value;
            dynamic rhsValue = rhs.Value;
            var comp = lhsValue.CompareTo(rhsValue);

            if (comp < 0) {
                return lhs.Unwrap();
            }
            else if (comp > 0) {
                return rhs;
            }
            else {
                return lhs.Unwrap().IsInclusive ? rhs : lhs.Unwrap();
            }
        }

        /// <summary>
        ///   Determine the larger of two <see cref="Bound">lower bounds</see>, one of which may not actually exist.
        /// </summary>
        /// <param name="lhs">
        ///   The first of the two <see cref="Bound">lower bounds</see>, which may be a <c>NONE</c> instance.
        /// </param>
        /// <param name="rhs">
        ///   The second of the two <see cref="Bound">lower bounds</see>.
        /// </param>
        /// <returns>
        ///   The larger of <paramref name="lhs"/> and <paramref name="rhs"/>. If <paramref name="lhs"/> is a
        ///   <c>NONE</c> instance, this is tautologically <paramref name="rhs"/>. Otherwise, it is the argument with
        ///   the larger <see cref="Bound.Value">value</see>. If both bounds' value are the same, an exclusive bound is
        ///   considered "larger" than an inclusive bound.
        /// </returns>
        private static Bound MaxLowerBound(Option<Bound> lhs, Bound rhs) {
            if (!lhs.HasValue) {
                return rhs;
            }

            // We use dynamic here so that we can easily use the operators to determine precedence, since the values are
            // already type-erased and there's no other great way to do it; we know, given the set of possible dynamic
            // types, that the CompareTo function will exist (we have to use this because strings do not have comparison
            // operators defined)
            dynamic lhsValue = lhs.Unwrap().Value;
            dynamic rhsValue = rhs.Value;
            var comp = lhsValue.CompareTo(rhsValue);

            if (comp > 0) {
                return lhs.Unwrap();
            }
            else if (comp < 0) {
                return rhs;
            }
            else {
                return lhs.Unwrap().IsInclusive ? rhs : lhs.Unwrap();
            }
        }

        /// <summary>
        ///   Check if an interval defined by two endpoints is valid, i.e. non-empty.
        /// </summary>
        /// <param name="lower">
        ///   The lower bound of the interval.
        /// </param>
        /// <param name="upper">
        ///   The upper bound of the interval.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if the interval defined by <paramref name="lower"/> and <paramref name="upper"/>
        ///   could theoretically contain at least one value, agnostic of the domain of said values; otherwise,
        ///   <see langword="false"/>.
        /// </returns>
        private static bool IsValidInterval(Option<Bound> lower, Option<Bound> upper) {
            if (!lower.HasValue || !upper.HasValue) {
                return true;
            }

            var lowerBound = lower.Unwrap();
            var upperBound = upper.Unwrap();

            if (lowerBound.Value.Equals(upperBound.Value)) {
                // If the bounds have the same anchor but are not both closed (inclusive), then the interval is invalid;
                // this would be e.g. [100, 100) or (100, 100] or (100, 100) -- all of which are empty
                return lowerBound.IsInclusive && upperBound.IsInclusive;
            }
            else {
                // We use dynamic here so that we can easily use the operator to determine precedence, since the values
                // are already type-erased and there's no other great way to do it; we know, given the set of possible
                // dynamic types, that the operator will exist
                dynamic lowerVal = lowerBound.Value;
                dynamic upperVal = upperBound.Value;

                // We have to use CompareTo here because strings do not have comparison operators defined
                return lowerVal.CompareTo(upperVal) < 0;
            }
        }

        /// <summary>
        ///   Determine if a value lies within an interval.
        /// </summary>
        /// <param name="value">
        ///   The probe.
        /// </param>
        /// <param name="lower">
        ///   The lower bound of the interval.
        /// </param>
        /// <param name="upper">
        ///   The upper bound of the interval.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if <paramref name="value"/> is within the interval formed by
        ///   <paramref name="lower"/> and <paramref name="upper"/>; otherwise, <see langword="false"/>.
        /// </returns>
        private static bool IsWithinInterval(object value, Option<Bound> lower, Option<Bound> upper) {
            Debug.Assert(IsValidInterval(lower, upper));
            
            // We use dynamic here so that we can easily use the operators to determine precedence, since the values
            // are already type-erased and there's no other great way to do it; we know, given the set of possible
            // dynamic types, that the operators will exist
            dynamic probe = value;

            if (lower.HasValue) {
                dynamic lowerVal = lower.Unwrap().Value;
                if (probe.CompareTo(lowerVal) < 0 || (probe == lowerVal && !lower.Unwrap().IsInclusive)) {
                    return false;
                }
            }
            if (upper.HasValue) {
                dynamic upperVal = upper.Unwrap().Value;
                if (probe.CompareTo(upperVal) > 0 || (probe == upperVal && !upper.Unwrap().IsInclusive)) {
                    return false;
                }
            }

            // We broke the two halves of the interval into separate checks; if the probe value has passed the lower
            // bound check and the upper bound check, then it falls within the interval
            return true;
        }

        /// <summary>
        ///   Convert an interval, in the form of two <see cref="Bound">bounds</see>, into a string using standard
        ///   mathematical notation.
        /// </summary>
        /// <param name="lowerBound">
        ///   The lower bound of the interval.
        /// </param>
        /// <param name="upperBound">
        ///   The upper bound of the interval.
        /// </param>
        /// <returns>
        ///   A string representation of the interval between <paramref name="lowerBound"/> and
        ///   <paramref name="upperBound"/>, using <c>∞</c> to represent absent endpoints.
        /// </returns>
        private static string ToString(Option<Bound> lowerBound, Option<Bound> upperBound) {
            var lowerStr = lowerBound.Match(
                some: b => b.IsInclusive ? $"[{b.Value.ForDisplay()}" : $"({b.Value.ForDisplay()}",
                none: () => "(-∞"
            );
            var upperStr = upperBound.Match(
                some: b => b.IsInclusive ? $"{b.Value.ForDisplay()}]" : $"{b.Value.ForDisplay()})",
                none: () => "+∞)"
            );

            return $"{lowerStr}, {upperStr}";
        }
    }
}
