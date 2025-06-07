using FluentAssertions.Execution;
using MySql.Data.MySqlClient;
using System.Data;

namespace FluentAssertions {
    internal static partial class AssertionExtensions {
        public static DataParametersAssertions Should(this IDataParameterCollection self) {
            return new DataParametersAssertions(self);
        }

        public class DataParametersAssertions : Primitives.ObjectAssertions {
            public new IDataParameterCollection Subject { get; }
            public DataParametersAssertions(IDataParameterCollection subject)
                : base(subject) {

                Subject = subject;
            }
            protected override string Identifier => "Collection of Parameters";

            [CustomAssertion]
            public AndConstraint<DataParametersAssertions> HaveCount(int count, string because = "", params object[] becauseArgs) {
                Execute.Assertion
                    .BecauseOf(because, becauseArgs)
                    .ForCondition(Subject.Count == count)
                    .FailWith($"Expected {{context:collection}} to have {count} parameters but found {Subject.Count}");

                return new AndConstraint<DataParametersAssertions>(this);
            }

            [CustomAssertion]
            public AndConstraint<MySqlParametersAssertion> BeForMySql(string because = "", params object[] becauseArgs) {
                var mysqlCollection = Subject as MySqlParameterCollection;

                Execute.Assertion
                    .BecauseOf(because, becauseArgs)
                    .ForCondition(mysqlCollection is not null)
                    .FailWith($"Expected {{context:collection}} to be for MySql but its type is {Subject.GetType()}");

                var mysqlAssertions = new MySqlParametersAssertion(mysqlCollection!);
                return new AndConstraint<MySqlParametersAssertion>(mysqlAssertions);
            }
        }

        public class MySqlParametersAssertion : Primitives.ObjectAssertions {
            public new MySqlParameterCollection Subject { get; }
            public MySqlParametersAssertion(MySqlParameterCollection subject)
                : base(subject) {

                Subject = subject;
            }
            protected override string Identifier => "Collection of MySQL Parameters";

            [CustomAssertion]
            public AndConstraint<MySqlParametersAssertion> HaveParameter(string name, object value, string because = "", params object[] becauseArgs) {

                Execute.Assertion
                    .BecauseOf(because, becauseArgs)
                    .ForCondition(Subject.Contains(name))
                    .FailWith($"No parameter with name {name} found in {{context:collection}}");

                Execute.Assertion
                    .BecauseOf(because, becauseArgs)
                    .ForCondition(Subject[name].Value.Equals(value))
                    .FailWith($"Expected value of parameter with name {name} to be {value} but found {Subject[name].Value}");

                return new AndConstraint<MySqlParametersAssertion>(this);
            }
        }
    }
}
