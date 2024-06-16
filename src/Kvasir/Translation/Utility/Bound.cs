using System.Diagnostics;

namespace Kvasir.Translation {
    /// <summary>
    ///   A boundary, which forms an endpoint of an <see cref="Interval"/> that may be open or closed.
    /// </summary>
    internal readonly record struct Bound(object Value, bool IsInclusive) {
        /// <summary>
        ///   Determines which of two <see cref="Bound">Bounds</see> subsumes the other, assuming both represent upper
        ///   bounds.
        /// </summary>
        /// <remarks>
        ///   The purpose of this method is to "clamp" two upper bounds that both apply to the more restrictive of the
        ///   two. For example, if a range is both "no more than 100" and "no more than 50," then the "clamp" will
        ///   produce the latter. If the endpoints are equal, the <see cref="Bound"/> that is exclusive will be chosen.
        /// </remarks>
        /// <param name="lhs">
        ///   The first of the upper bounds.
        /// </param>
        /// <param name="rhs">
        ///   The second of the upper bounds.
        /// </param>
        /// <returns>
        ///   Whichever of <paramref name="lhs"/> and <paramref name="rhs"/> is a more restrictive upper bound.
        /// </returns>
        public static Bound ClampUpper(Bound lhs, Bound rhs) {
            Debug.Assert(lhs.Value.GetType() == rhs.Value.GetType());

            // We have to use the `CompareTo` method, since there are no binary ordering operators defined for strings.
            // The use of `dynamic` makes it so that we don't have to do reflection to find the `CompareTo` method or to
            // instantiate a default `IComparer` object.
            dynamic lhsValue = lhs.Value;
            dynamic rhsValue = rhs.Value;
            var comp = lhsValue.CompareTo(rhsValue);

            if (comp < 0) {
                return lhs;
            }
            else if (comp > 0) {
                return rhs;
            }
            else {
                // The two endpoint values are equal, so we clamp the bound that is exclusive, since that one is more
                // restrictive; if they're both inclusive, then they're the same Bound, and it doesn't matter which one
                // we return
                return lhs.IsInclusive ? rhs : lhs;
            }
        }

        /// <summary>
        ///   Determines which of two <see cref="Bound">Bounds</see> subsumes the other, assuming both represent lower
        ///   bounds.
        /// </summary>
        /// <remarks>
        ///   The purpose of this method is to "clamp" two lower bounds that both apply to the more restrictive of the
        ///   two. For example, if a range is both "no less than 100" and "no less than 50," then the "clamp" will
        ///   produce the former. If the endpoints are equal, the <see cref="Bound"/> that is exclusive will be chosen.
        /// </remarks>
        /// <param name="lhs">
        ///   The first of the lower bounds.
        /// </param>
        /// <param name="rhs">
        ///   The second of the lower bounds.
        /// </param>
        /// <returns>
        ///   Whichever of <paramref name="lhs"/> and <paramref name="rhs"/> is a more restrictive lower bound.
        /// </returns>
        public static Bound ClampLower(Bound lhs, Bound rhs) {
            Debug.Assert(lhs.Value.GetType() == rhs.Value.GetType());

            // We have to use the `CompareTo` method, since there are no binary ordering operators defined for strings.
            // The use of `dynamic` makes it so that we don't have to do reflection to find the `CompareTo` method or to
            // instantiate a default `IComparer` object.
            dynamic lhsValue = lhs.Value;
            dynamic rhsValue = rhs.Value;
            var comp = lhsValue.CompareTo(rhsValue);

            if (comp < 0) {
                return rhs;
            }
            else if (comp > 0) {
                return lhs;
            }
            else {
                // The two endpoint values are equal, so we clamp the bound that is exclusive, since that one is more
                // restrictive; if they're both inclusive, then they're the same Bound, and it doesn't matter which one
                // we return
                return lhs.IsInclusive ? rhs : lhs;
            }
        }
    }
}
