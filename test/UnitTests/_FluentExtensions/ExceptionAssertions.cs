using FluentAssertions.Specialized;
using System;

namespace FluentAssertions {
    internal static partial class AssertionExtensions {
        [CustomAssertion]
        public static ExceptionAssertions<TEx> WithAnyMessage<TEx>(this ExceptionAssertions<TEx> self)
            where TEx : Exception {

            return self.WithMessage("*");
        }
    }
}
