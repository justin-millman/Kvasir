using Cybele.Extensions;
using FluentAssertions.Execution;
using Kvasir.Schema;
using Optional;
using System.Collections.Generic;
using System.Linq;

namespace FluentAssertions {
    internal static partial class AssertionExtensions {
        public static TableAssertion Should(this ITable self) {
            return new TableAssertion(self);
        }

        public class TableAssertion : Primitives.ObjectAssertions {
            public new ITable Subject { get; }
            public TableAssertion(ITable subject)
                : base(subject) {

                Subject = subject;
            }
            protected override string Identifier => "Table";
            private readonly HashSet<string> checkedFieldNames_ = new HashSet<string>();
            private readonly HashSet<string> checkedKeyNames_ = new HashSet<string>();
            private readonly HashSet<CheckConstraint> checkedConstraints_ = new HashSet<CheckConstraint>();

            [CustomAssertion]
            public AndConstraint<TableAssertion> HaveName(string name, string because = "",
                params object[] becauseArgs) {

                Execute.Assertion
                    .BecauseOf(because, becauseArgs)
                    .ForCondition(Subject.Name.ToString() == name)
                    .FailWith($"Expected {{context:Table}} to have name '{name}'{{reason}}, but found {Subject.Name}");

                return new AndConstraint<TableAssertion>(this);
            }

            [CustomAssertion]
            private IField HaveField(string name, string because = "", params object[] becauseArgs) {
                IField? field = Subject.Fields.FirstOrDefault(f => f.Name.ToString() == name);
                Execute.Assertion
                    .BecauseOf(because, becauseArgs)
                    .ForCondition(field is not null)
                    .FailWith($"Expected {{context:Table}} to contain a Field with name '{name}'{{reason}}");

                checkedFieldNames_.Add(name);
                return field!;
            }

            [CustomAssertion]
            public AndConstraint<TableAssertion> HaveField(string name, DBType dataType, IsNullable nullable,
                string because = "", params object[] becauseArgs) {

                var field = HaveField(name, because, becauseArgs);
                string expectedNullability = (nullable == IsNullable.Yes) ? "nullable" : "non-nullable";

                Execute.Assertion
                    .BecauseOf(because, becauseArgs)
                    .ForCondition(field.DataType == dataType)
                    .FailWith($"Expected Field '{name}' to have data type {dataType}{{reason}}, but its data type is " +
                              field.DataType.ToString());
                Execute.Assertion
                    .BecauseOf(because, becauseArgs)
                    .ForCondition(field.Nullability == nullable)
                    .FailWith($"Expected Field '{name}' to be {expectedNullability}{{reason}}");

                return new AndConstraint<TableAssertion>(this);
            }

            [CustomAssertion]
            public AndConstraint<TableAssertion> HaveField(string name, int column, string because = "",
                params object[] becauseArgs) {

                Execute.Assertion
                    .BecauseOf(because, becauseArgs)
                    .ForCondition(column < Subject.Dimension)
                    .FailWith($"Expected Field '{name}' at column index #{column}{{reason}}, but the " +
                              $"{{context:Table}} only has {Subject.Dimension} " +
                              $"Field{(Subject.Dimension > 1 ? "s" : "")}");

                HaveField(name);
                var actualAtColumn = Subject.Fields[column];

                Execute.Assertion
                    .BecauseOf(because, becauseArgs)
                    .ForCondition(actualAtColumn.Name.ToString() == name)
                    .FailWith($"Expected Field '{name}' at column index #{column}{{reason}}, but found Field " +
                              actualAtColumn.Name.ToString());

                return new AndConstraint<TableAssertion>(this);
            }

            [CustomAssertion]
            public AndConstraint<TableAssertion> HaveField<T>(string name, Option<T> defaultValue,
                string because = "", params object[] becauseArgs) {

                var field = HaveField(name);

                defaultValue.Match(
                    some: v => {
                        Execute.Assertion
                            .BecauseOf(because, becauseArgs)
                            .ForCondition(field.DefaultValue.HasValue)
                            .FailWith($"Expected Field '{name}' to have a default value{{reason}}");
                        Execute.Assertion
                            .BecauseOf(because, becauseArgs)
                            .ForCondition(field.DefaultValue.Unwrap().Datum.Equals(v))
                            .FailWith($"Expected Field '{name}' to have default value " +
                                      $"<{DBValue.Create(v)}>{{reason}}, but found <{field.DefaultValue}>");
                    },
                    none: () => {
                        Execute.Assertion
                            .BecauseOf(because, becauseArgs)
                            .ForCondition(!field.DefaultValue.HasValue)
                            .FailWith($"Expected Field '{name}' to have no default value{{reason}}");
                    }
                );

                return new AndConstraint<TableAssertion>(this);
            }

            [CustomAssertion]
            public AndConstraint<TableAssertion> NotHaveField(string name, string because = "",
                params object[] becauseArgs) {

                Execute.Assertion
                    .BecauseOf(because, becauseArgs)
                    .ForCondition(Subject.Fields.FirstOrDefault(f => f.Name.ToString() == name) is null)
                    .FailWith($"Expected {{context:Table}} to not contain a Field with name '{name}'{{reason}}");

                return new AndConstraint<TableAssertion>(this);
            }

            [CustomAssertion]
            public KeyAssertion HavePrimaryKey(params string[] fieldNames) {
                var pkFields = new HashSet<string>(Subject.PrimaryKey.Fields.Select(f => f.Name.ToString()));
                var expected = new HashSet<string>(fieldNames);

                var missing = pkFields.Except(expected);
                var extra = expected.Except(pkFields);

                foreach (var name in fieldNames) {
                    // Deliberately use a new Assertion object so that the Field names do not get recorded in the
                    // running list, since this is not strictly a Field-presence check
                    Subject.Should().HaveField(name);
                }

                Execute.Assertion
                    .BecauseOf("")
                    .ForCondition(missing.IsEmpty() && extra.IsEmpty())
                    .FailWith($"Expected {{context:Table}} to have Primary Key consisting of Fields " +
                              $"[{string.Join(", ", expected)}]{{reason}}, but found Fields " +
                              $"[{string.Join(", ", pkFields)}]");

                return new KeyAssertion(Subject.PrimaryKey, this, "Primary");
            }

            [CustomAssertion]
            public KeyAssertion HaveCandidateKey(params string[] fieldNames) {
                var cks = Subject.CandidateKeys;
                var expected = new HashSet<string>(fieldNames);

                foreach (var name in fieldNames) {
                    // Deliberately use a new Assertion object so that the Field names do not get recorded in the
                    // running list, since this is not strictly a Field-presence check
                    Subject.Should().HaveField(name);
                }

                var key = cks.FirstOrDefault(k => k.Fields.Select(f => f.Name.ToString()).ToHashSet().SetEquals(expected));
                Execute.Assertion
                    .BecauseOf("")
                    .ForCondition(key is not null)
                    .FailWith($"Expected {{context:Table}} to have a Candidate Key with Fields " +
                              $"[{string.Join(", ", expected)}]{{reason}}, but no such Candidate Key was found");

                checkedKeyNames_.Add(key!.Name.ToString());
                return new KeyAssertion(key!, this, "Candidate");
            }

            [CustomAssertion]
            public void NoOtherFields(string because = "", params object[] becauseArgs) {
                var fieldNames = new HashSet<string>(Subject.Fields.Select(f => f.Name.ToString()));
                var extras = fieldNames.Except(checkedFieldNames_);

                Execute.Assertion
                    .BecauseOf(because, becauseArgs)
                    .ForCondition(extras.IsEmpty())
                    .FailWith($"{{context:Table}} also has the following Fields: {string.Join(", ", extras)}");
            }

            [CustomAssertion]
            public void NoOtherCandidateKeys(string because = "", params object[] becauseArgs) {
                var keyNames = new HashSet<string>(Subject.CandidateKeys.Select(f => f.Name.ToString()));
                var extras = keyNames.Except(checkedKeyNames_);

                Execute.Assertion
                    .BecauseOf(because, becauseArgs)
                    .ForCondition(extras.IsEmpty())
                    .FailWith($"{{context:Table}} also has the following Candidate Keys: {string.Join(", ", extras)}");
            }

            [CustomAssertion]
            public AndConstraint<TableAssertion> HaveConstraint(string field, ComparisonOperator op, object value,
                string because = "", params object[] becauseArgs) {

                var repr = $"'{field} {op} {value}'";
                bool found = false;
                foreach (var constraint in Subject.CheckConstraints) {
                    if (constraint.Condition is ConstantClause cc) {
                        var lhsMatch = (cc.LHS.Function.HasValue, cc.LHS.Field.Name.ToString()) == (false, field);
                        var opMatch = cc.Operator == op;
                        var rhsMatch = cc.RHS == DBValue.Create(value);

                        if (lhsMatch && opMatch && rhsMatch) {
                            found = true;
                            checkedConstraints_.Add(constraint);

                            Execute.Assertion
                                .BecauseOf("CHECK constraints cannot currently be named")
                                .ForCondition(!constraint.Name.HasValue)
                                .FailWith($"Expected CHECK constraint {repr} to be unnamed, but found name");
                            break;
                        }
                    }
                }

                Execute.Assertion
                    .BecauseOf(because, becauseArgs)
                    .ForCondition(found)
                    .FailWith($"Expected to find CHECK constraint {repr}, but the {{context:Table}} has no such " +
                              "constraint");

                return new AndConstraint<TableAssertion>(this);
            }

            [CustomAssertion]
            public AndConstraint<TableAssertion> HaveConstraint(string field, InclusionOperator op,
                IEnumerable<object?> values, string because = "", params object[] becauseArgs) {

                var repr = $"'{field} {op} ({string.Join(", ", values)})'";
                bool found = false;
                foreach (var constraint in Subject.CheckConstraints) {
                    if (constraint.Condition is InclusionClause cc) {
                        var lhsMatch = (cc.LHS.Function.HasValue, cc.LHS.Field.Name.ToString()) == (false, field);
                        var opMatch = cc.Operator == op;
                        var rhsMatch = cc.RHS.ToHashSet().SetEquals(values.Select(v => DBValue.Create(v)).ToHashSet());

                        if (lhsMatch && opMatch && rhsMatch) {
                            found = true;
                            checkedConstraints_.Add(constraint);

                            Execute.Assertion
                                .BecauseOf("CHECK constraints cannot currently be named")
                                .ForCondition(!constraint.Name.HasValue)
                                .FailWith($"Expected CHECK constraint {repr} to be unnamed, but  found name");
                            break;
                        }
                    }
                }

                Execute.Assertion
                    .BecauseOf(because, becauseArgs)
                    .ForCondition(found)
                    .FailWith($"Expected to find CHECK constraint {repr}, but the {{context:Table}} has not such " +
                              "constraint");

                return new AndConstraint<TableAssertion>(this);
            }

            [CustomAssertion]
            public AndConstraint<TableAssertion> HaveConstraint(string field, NullityOperator op, string because = "",
                params object[] becauseArgs) {

                var repr = $"'{field} {op}'";
                bool found = false;
                foreach (var constraint in Subject.CheckConstraints) {
                    if (constraint.Condition is NullityClause cc) {
                        var lhsMatch = (cc.LHS.Function.HasValue, cc.LHS.Field.Name.ToString()) == (false, field);
                        var opMatch = cc.Operator == op;

                        if (lhsMatch && opMatch) {
                            found = true;
                            checkedConstraints_.Add(constraint);

                            Execute.Assertion
                                .BecauseOf("CHECK constraints cannot currently be named")
                                .ForCondition(!constraint.Name.HasValue)
                                .FailWith($"Expected CHECK constraint {repr} to be unnamed, but  found name");
                            break;
                        }
                    }
                }

                Execute.Assertion
                    .BecauseOf(because, becauseArgs)
                    .ForCondition(found)
                    .FailWith($"Expected to find CHECK constraint {repr}, but the {{context:Table}} has no such " +
                              "constraint");

                return new AndConstraint<TableAssertion>(this);
            }

            [CustomAssertion]
            public AndConstraint<TableAssertion> HaveConstraint(FieldFunction func, string field, ComparisonOperator op,
                int value, string because = "", params object[] becauseArgs) {

                var repr = $"'{func} {field} {op} {value}'";
                bool found = false;
                foreach (var constraint in Subject.CheckConstraints) {
                    if (constraint.Condition is ConstantClause cc) {
                        var lhsMatch = (cc.LHS.Function.Contains(func), cc.LHS.Field.Name.ToString()) == (true, field);
                        var opMatch = cc.Operator == op;
                        var rhsMatch = cc.RHS == DBValue.Create(value);

                        if (lhsMatch && opMatch && rhsMatch) {
                            found = true;
                            checkedConstraints_.Add(constraint);

                            Execute.Assertion
                                .BecauseOf("CHECK constraints cannot currently be named")
                                .ForCondition(!constraint.Name.HasValue)
                                .FailWith($"Expected CHECK constraint {repr} to be unnamed, but  found name");
                            break;
                        }
                    }
                }

                Execute.Assertion
                    .BecauseOf(because, becauseArgs)
                    .ForCondition(found)
                    .FailWith($"Expected to find CHECK constraint {repr}, but the {{context:Table}} has no such " +
                              "constraint");

                return new AndConstraint<TableAssertion>(this);
            }

            [CustomAssertion]
            public void NoOtherConstraints(string because = "", params object[] becauseArgs) {
                var excess = Subject.CheckConstraints.Count - checkedConstraints_.Count;

                Execute.Assertion
                    .BecauseOf(because, becauseArgs)
                    .ForCondition(excess == 0)
                    .FailWith($"{{context:Table}} has {excess} additional CHECK constraint{(excess == 1 ? "" : "s")}");
            }
        }

        public class KeyAssertion : TableAssertion {
            public new IKey Subject { get; }
            public KeyAssertion(IKey key, TableAssertion parent, string flavor)
                : base(parent.Subject) {

                Subject = key;
                Identifier = $"{flavor} Key";
                And = parent;
            }
            protected override string Identifier { get; }
            public TableAssertion And { get; }

            [CustomAssertion]
            public AndConstraint<TableAssertion> WithName(string name, string because = "",
                params object[] becauseArgs) {

                Execute.Assertion
                    .BecauseOf(because, becauseArgs)
                    .ForCondition(Subject.Name.HasValue)
                    .FailWith($"Expected {{context:{Identifier}}} to have name{{reason}}");
                Execute.Assertion
                    .BecauseOf(because, becauseArgs)
                    .ForCondition(Subject.Name.Unwrap().ToString() == name)
                    .FailWith($"Expected {{context:{Identifier}}} to have name '{name}'{{reason}}, but found " +
                              $"'{Subject.Name.Unwrap()}'");

                return new AndConstraint<TableAssertion>(And);
            }

            [CustomAssertion]
            public AndConstraint<TableAssertion> WithoutName(string because = "", params object[] becauseArgs) {
                Execute.Assertion
                    .BecauseOf(because, becauseArgs)
                    .ForCondition(!Subject.Name.HasValue)
                    .FailWith(() =>
                        new FailReason(
                            $"Expected {{context:{Identifier}}} to have no name, " +
                            $"but found '{Subject.Name.Unwrap()}'")
                    );

                return new AndConstraint<TableAssertion>(And);
            }
        }
    }
}