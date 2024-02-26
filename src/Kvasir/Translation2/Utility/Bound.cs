namespace Kvasir.Translation2 {
    ///
    internal readonly struct Bound {
        ///
        public static Bound ClampUpper(Bound lhs, Bound rhs) {
            // We have to use the `CompareTo` method, since there are no binary ordering operators defined for strings.
            // The use of `dynamic` makes it so that we don't have to do reflection to find the `CompareTo` method or to
            // instantiate a default `IComparer` object.
            dynamic lhsValue = lhs.value_;
            dynamic rhsValue = rhs.value_;
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
                return lhs.isInclusive_ ? rhs : lhs;
            }
        }

        ///
        public static Bound ClampLower(Bound lhs, Bound rhs) {
            // We have to use the `CompareTo` method, since there are no binary ordering operators defined for strings.
            // The use of `dynamic` makes it so that we don't have to do reflection to find the `CompareTo` method or to
            // instantiate a default `IComparer` object.
            dynamic lhsValue = lhs.value_;
            dynamic rhsValue = rhs.value_;
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
                return lhs.isInclusive_ ? rhs : lhs;
            }
        }

        ///
        public bool FormsValidIntervalWith(Bound upper) {
            if (value_.Equals(upper)) {
                // If the endpoints of the interval have the same value but are not both closed, then the interval is
                // invalid. This would be e.g. [100, 100) or (100, 100] or (100, 100), all of which are empty.
                return isInclusive_ && upper.isInclusive_;
            }
            else {
                // We have to use the `CompareTo` method, since there are no binary ordering operators defined for strings.
                // The use of `dynamic` makes it so that we don't have to do reflection to find the `CompareTo` method or to
                // instantiate a default `IComparer` object.
                dynamic lowerValue = value_;
                dynamic upperValue = upper.value_;
                return lowerValue.CompareTo(upperValue) < 0;
            }
        }


        private readonly object value_;
        private readonly bool isInclusive_;
    }
}
