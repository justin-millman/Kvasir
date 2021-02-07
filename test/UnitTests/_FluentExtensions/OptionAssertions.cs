using FluentAssertions.Execution;
using Optional;

namespace FluentAssertions {
    internal static partial class AssertionExtensions {
        public static OptionAssertions<T> Should<T>(this Option<T> self) {
            return new OptionAssertions<T>(self);
        }

        public class OptionAssertions<T> : Primitives.ObjectAssertions {
            public new Option<T> Subject { get; }
            public OptionAssertions(Option<T> subject)
                : base(subject) {

                Subject = subject;
            }
            protected override string Identifier => "Option";

            [CustomAssertion]
            public AndConstraint<OptionAssertions<T>> HaveValue(string because = "", params object[] becauseArgs) {
                Execute.Assertion
                    .BecauseOf(because, becauseArgs)
                    .ForCondition(Subject.HasValue)
                    .FailWith("Expected {context:option} to contain a value{reason}, but found an empty option");

                return new AndConstraint<OptionAssertions<T>>(this);
            }

            [CustomAssertion]
            public AndConstraint<OptionAssertions<T>> HaveValue(T value, string because = "",
                params object[] becauseArgs) {

                Execute.Assertion
                    .BecauseOf(because, becauseArgs)
                    .ForCondition(Subject.Exists(v => Equals(v, value)))
                    .FailWith("Expected {context:option} to contain value " + (value?.ToString() ?? "NULL") +
                              "{reason}");

                return new AndConstraint<OptionAssertions<T>>(this);
            }

            [CustomAssertion]
            public AndConstraint<OptionAssertions<T>> NotHaveValue(string because = "", params object[] becauseArgs) {
                Execute.Assertion
                    .BecauseOf(because, becauseArgs)
                    .ForCondition(!Subject.HasValue)
                    .FailWith("Expected {context:option} to be empty{reason}, but found an option containing " +
                        $"the value <{Subject.ValueOr(() => default!)}>");

                return new AndConstraint<OptionAssertions<T>>(this);
            }
        }
    }
}
