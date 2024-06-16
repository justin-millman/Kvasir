using Cybele.Extensions;
using Optional;
using System.Diagnostics;

namespace Kvasir.Translation {
    /// <summary>
    ///   An interval, which may infinite, one-sided, partially open, or closed.
    /// </summary>
    ///
    /// <param name="LowerBound">The endpoint that defines the lower boundary of the <see cref="Interval"/></param>
    /// <param name="UpperBound">The endpoint that defines the upper boundary of the <see cref="Interval"/></param>
    internal readonly record struct Interval(Option<Bound> LowerBound, Option<Bound> UpperBound) {
        /// <summary>
        ///   Checks if a value is contained within the <see cref="Interval"/>.
        /// </summary>
        /// <param name="value">
        ///   The probe value.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if <paramref name="value"/> is within the <see cref="Interval"/>; otherwise,
        ///   <see langword="false"/>.
        /// </returns>
        public bool Contains(object value) {
            Debug.Assert(value is not null);
            Debug.Assert(!LowerBound.Exists(b => b.Value.GetType() != value.GetType()));
            Debug.Assert(!UpperBound.Exists(b => b.Value.GetType() != value.GetType()));

            // We have to use the `CompareTo` method, since there are no binary ordering operators defined for `string`.
            // The use of `dynamic` makes it so that we don't have to use reflection to find the `CompareTo` method or
            // to instantiate a default `IComparer` object.

            if (LowerBound.HasValue) {
                var lower = LowerBound.Unwrap();
                var comp = (lower.Value as dynamic).CompareTo(value as dynamic);

                if (comp > 0 || (comp == 0 && !lower.IsInclusive)) {
                    return false;
                }
            }
            if (UpperBound.HasValue) {
                var upper = UpperBound.Unwrap();
                var comp = (upper.Value as dynamic).CompareTo(value as dynamic);

                if (comp < 0 || (comp == 0 && !upper.IsInclusive)) {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        ///   Checks if the <see cref="Interval"/> is valid.
        /// </summary>
        /// <remarks>
        ///   A valid Interval is one that is empty, assuming that there can be at least one value in between any two
        ///   distinct values. An infinite Interval is always valid, as is a one-sided Interval. A partially open
        ///   Interval and a closed Interval is valid if the Interval's lower boundary is less than its upper boundary.
        ///   For example, the Interval [1, 10] is obviously valid, as are [1, 10), (1, 10], and (1, 10). The Interval
        ///   [1, 1] is valid (it contains just a single value), but the Intervals [1, 1), (1, 1], and (1, 1) are all
        ///   invalid. Perhaps counterintuitively, the Interval (1, 2) is valid, even though it is mathematically empty;
        ///   this makes it so that we don't have to figure out epsilons, which don't exist for all orderable types
        ///   (e.g. strings).
        /// </remarks>
        /// <returns>
        ///   <see langword="true"/> if the <see cref="Interval"/> is valid; otherwise, <see langword="false"/>.
        /// </returns>
        public bool IsValid() {
            Debug.Assert(!LowerBound.Exists(b => b.Value is null));
            Debug.Assert(!UpperBound.Exists(b => b.Value is null));

            // If it weren't for CS1673 preventing lambdas in structs from accessing members of 'this', and the
            // bulkiness of the actual evaluation logic, I'd do this monadically with Option.Match. As it is, though,
            // we'll first check for values and then unwrap.
            
            if (!LowerBound.HasValue || !UpperBound.HasValue) {
                // A single-sided interval is always valid
                return true;
            }

            var lower = LowerBound.Unwrap();
            var upper = UpperBound.Unwrap();

            if (lower.Value.Equals(upper.Value)) {
                // If the endpoints of the interval have the same value but are not both closed, then the interval is
                // invalid. This would be e.g. [100, 100) or (100, 100] or (100, 100), all of which are empty.
                return lower.IsInclusive && upper.IsInclusive;
            }
            else {
                // We have to use the `CompareTo` method, since there are no binary ordering operators defined for
                // `string`. The use of `dynamic` makes it so that we don't have to use reflection to find the
                // `CompareTo` method or to instantiate a default `IComparer` object.
                return (lower.Value as dynamic).CompareTo(upper.Value as dynamic) < 0;
            }
        }

        /// <summary>
        ///   Produces a human-readable string representation of the <see cref="Interval"/>.
        /// </summary>
        /// <returns>
        ///   A representation of the <see cref="Interval"/> in mathematical notation, with an absent endpoint being
        ///   indicated by the infinity symbol.
        /// </returns>
        public override string ToString() {
            var lower = LowerBound.Match(some: b => (b.IsInclusive ? "[" : "(") + b.Value.ForDisplay(), none: () => "(∞");
            var upper = UpperBound.Match(some: b => b.Value.ForDisplay() + (b.IsInclusive ? "]" : ")"), none: () => "∞)");
            return $"{lower}, {upper}";
        }
    }
}
