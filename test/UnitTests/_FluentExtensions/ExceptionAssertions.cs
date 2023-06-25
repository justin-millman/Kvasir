using FluentAssertions.Specialized;
using System;

namespace FluentAssertions {
    internal static partial class AssertionExtensions {
        [CustomAssertion]
        public static ExceptionAssertions<TEx> WithAnyMessage<TEx>(this ExceptionAssertions<TEx> self)
            where TEx : Exception {

            return self.WithMessage("*");
        }

        public static ExceptionAssertions<TEx> WithMessageContaining<TEx>(this ExceptionAssertions<TEx> self, string msg)
            where TEx : Exception {

            return self.WithMessage($"*{msg}*");
        }
    }
}
