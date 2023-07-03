using Cybele.Extensions;
using FluentAssertions.Execution;
using Kvasir.Annotations;
using Kvasir.Schema;
using System;
using System.Collections.Generic;
using System.Linq;

using And = FluentAssertions.AndConstraint<FluentAssertions.AssertionExtensions.TableAssertion>;

namespace FluentAssertions {
    internal static partial class AssertionExtensions {
        public static TableAssertion Should(this ITable table) {
            return new TableAssertion(table);
        }

        public class TableAssertion : Primitives.ObjectAssertions {
            public new ITable Subject { get; }
            public TableAssertion(ITable subject)
                : base(subject) { Subject = subject; checkedFields_ = new(); checkedKeys_ = new(); checkedConstraints_ = new(); }
            protected override string Identifier => "Table";

            [CustomAssertion] public And HaveName(string name) {
                Execute.Assertion
                    .ForCondition(Subject.Name == new TableName(name))
                    .FailWith($"Expected {{context:Table}} to have name \"{name}\", but found \"{Subject.Name}\"");

                return new AndConstraint<TableAssertion>(this);
            }

            [CustomAssertion] public FieldAssertion HaveField(string fieldName) {
                for (int column = 0; column < Subject.Fields.Count; column++) {
                    if (Subject.Fields[column].Name == new FieldName(fieldName)) {
                        checkedFields_.Add(new FieldName(fieldName));
                        return new FieldAssertion(Subject.Fields[column], column, this);
                    }
                }

                Execute.Assertion
                    .ForCondition(false)
                    .FailWith($"Expected {{context:Table}} to have Field with name \"{fieldName}\", but no such Field found");

                throw new ApplicationException("UNREACHABLE CODE!");
            }
            [CustomAssertion] public And HaveNoOtherFields() {
                var missing = string.Join(", ", Subject.Fields.Select(f => f.Name).Where(n => !checkedFields_.Contains(n)));
                Execute.Assertion
                    .ForCondition(missing == "")
                    .FailWith($"{{context:Table}} has additional Fields: {missing}");
                return new AndConstraint<TableAssertion>(this);
            }

            public KeyAssertion HavePrimaryKey() {
                return new KeyAssertion(Subject.PrimaryKey, this).WithoutName();
            }
            public KeyAssertion HavePrimaryKey(string keyName) {
                return new KeyAssertion(Subject.PrimaryKey, this).WithName(keyName);
            }

            public CandidateKeysAssertion HaveCandidateKey() {
                return new CandidateKeysAssertion(Subject.CandidateKeys, checkedKeys_, this);
            }
            public KeyAssertion HaveCandidateKey(string keyName) {
                return new CandidateKeysAssertion(Subject.CandidateKeys, checkedKeys_, this).WithName(keyName);
            }
            public CandidateKeysAssertion HaveAnonymousCandidateKey() {
                return new CandidateKeysAssertion(Subject.CandidateKeys, checkedKeys_, this).ThatIsAnonymous();
            }
            [CustomAssertion] public And HaveNoOtherCandidateKeys() {
                var missing = string.Join(", ", Subject.CandidateKeys.Select(k => k.Name.Unwrap()).Where(n => !checkedKeys_.Contains(n)));
                Execute.Assertion
                    .ForCondition(missing == "")
                    .FailWith($"{{context:Table}} have additional Candidate Keys: {missing}");
                return new AndConstraint<TableAssertion>(this);
            }

            [CustomAssertion] public And HaveConstraint(string field, ComparisonOperator op, object anchor) {
                foreach (var constraint in Subject.CheckConstraints) {
                    if (constraint.Condition is ConstantClause cc) {
                        var lhsMatch = (cc.LHS.Function.HasValue, cc.LHS.Field.Name.ToString()) == (false, field);
                        var opMatch = (cc.Operator == op);
                        var rhsMatch = (cc.RHS == DBValue.Create(anchor));

                        if (lhsMatch && opMatch && rhsMatch) {
                            Execute.Assertion
                                .ForCondition(!constraint.Name.HasValue)
                                .BecauseOf("CHECK constraints cannot currently be named")
                                .FailWith("How does this CHECK constraint have a name?");

                            checkedConstraints_.Add(constraint);
                            return new And(this);
                        }
                    }
                }

                Execute.Assertion
                        .ForCondition(false)
                        .FailWith("Expected {context:Table} to have CHECK constraint" +
                                  $"'{field} {op} {anchor.ForDisplay()}', but no such constraint found");

                throw new ApplicationException("UNREACHABLE CODE!");
            }
            [CustomAssertion] public And HaveConstraint(string field, InclusionOperator op, params object[] anchors) {
                foreach (var constraint in Subject.CheckConstraints) {
                    if (constraint.Condition is InclusionClause ic) {
                        var lhsMatch = (ic.LHS.Function.HasValue, ic.LHS.Field.Name.ToString()) == (false, field);
                        var opMatch = (ic.Operator == op);
                        var rhsMatch = ic.RHS.ToHashSet().SetEquals(anchors.Select(v => DBValue.Create(v)));

                        if (lhsMatch && opMatch && rhsMatch) {
                            Execute.Assertion
                                .ForCondition(!constraint.Name.HasValue)
                                .BecauseOf("CHECK constraints cannot currently be named")
                                .FailWith("How does this CHECK constraint have a name?");

                            checkedConstraints_.Add(constraint);
                            return new And(this);
                        }
                    }
                }

                Execute.Assertion
                    .ForCondition(false)
                    .FailWith($"Expected {{context:Table}} to have CHECK constraint " +
                              $"'{field} {op} [{string.Join(", ", anchors.Select(v => v.ForDisplay()))}]', " +
                              "but no such constraint found");

                throw new ApplicationException("UNREACHABLE CODE!");
            }
            [CustomAssertion] public And HaveConstraint(FieldFunction fn, string field, ComparisonOperator op, object anchor) {
                foreach (var constraint in Subject.CheckConstraints) {
                    if (constraint.Condition is ConstantClause cc) {
                        var lhsMatch = (cc.LHS.Function.Contains(fn), cc.LHS.Field.Name.ToString()) == (true, field);
                        var opMatch = (cc.Operator == op);
                        var rhsMatch = (cc.RHS == DBValue.Create(anchor));

                        if (lhsMatch && opMatch && rhsMatch) {
                            Execute.Assertion
                                .ForCondition(!constraint.Name.HasValue)
                                .BecauseOf("CHECK constraints cannot currently be named")
                                .FailWith("How does this CHECK constraint have a name?");

                            checkedConstraints_.Add(constraint);
                            return new And(this);
                        }
                    }
                }

                Execute.Assertion
                        .ForCondition(false)
                        .FailWith("Expected {context:Table} to have CHECK constraint " +
                                  $"'{fn}({field}) {op} {anchor.ForDisplay()}', but no such constraint found");

                throw new ApplicationException("UNREACHABLE CODE!");
            }
            [CustomAssertion] public And HaveNoOtherConstraints() {
                var extras = Subject.CheckConstraints.Count - checkedConstraints_.Count;
                Execute.Assertion
                    .ForCondition(extras == 0)
                    .FailWith($"{{context:Table}} has {extras} additional CHECK constraint{(extras == 1 ? "" : "s")}");

                return new And(this);
            }


            private readonly HashSet<FieldName> checkedFields_;
            private readonly HashSet<KeyName> checkedKeys_;
            private readonly HashSet<CheckConstraint> checkedConstraints_;
        }

        public class FieldAssertion : Primitives.ObjectAssertions {
            public new IField Subject { get; }
            public FieldAssertion(IField subject, int column, TableAssertion parent)
                : base(subject) { Subject = subject; parent_ = parent; column_ = column; }
            protected override string Identifier => "Field";

            public TableAssertion And => parent_;

            public FieldAssertion BeingNullable() { return WithNullability(true); }
            public FieldAssertion BeingNonNullable() { return WithNullability(false); }

            public FieldAssertion OfTypeBoolean() { return OfType(DBType.Boolean); }
            public FieldAssertion OfTypeCharacter() { return OfType(DBType.Character); }
            public FieldAssertion OfTypeDateTime() { return OfType(DBType.DateTime); }
            public FieldAssertion OfTypeDecimal() { return OfType(DBType.Decimal); }
            public FieldAssertion OfTypeDouble() { return OfType(DBType.Double); }
            public FieldAssertion OfTypeGuid() { return OfType(DBType.Guid); }
            public FieldAssertion OfTypeInt8() { return OfType(DBType.Int8); }
            public FieldAssertion OfTypeInt16() { return OfType(DBType.Int16); }
            public FieldAssertion OfTypeInt32() { return OfType(DBType.Int32); }
            public FieldAssertion OfTypeInt64() { return OfType(DBType.Int64); }
            public FieldAssertion OfTypeSingle() { return OfType(DBType.Single); }
            public FieldAssertion OfTypeText() { return OfType(DBType.Text); }
            public FieldAssertion OfTypeUInt8() { return OfType(DBType.UInt8); }
            public FieldAssertion OfTypeUInt16() { return OfType(DBType.UInt16); }
            public FieldAssertion OfTypeUInt32() { return OfType(DBType.UInt32); }
            public FieldAssertion OfTypeUInt64() { return OfType(DBType.UInt64); }

            [CustomAssertion] public FieldAssertion OfTypeEnumeration<TEnum>(params TEnum[] enumerators) where TEnum : Enum {
                var _ = OfType(DBType.Enumeration);
                var enums = enumerators.Select(e => DBValue.Create(e.ToString()!.Replace(", ", "|")));

                Execute.Assertion
                    .ForCondition(Subject is EnumField)
                    .FailWith($"Expected {{context:Field}} to be an EnumField, but found a BasicField");
                Execute.Assertion
                    .ForCondition((Subject as EnumField)!.Enumerators.ToHashSet().SetEquals(enums))
                    .FailWith($"Expected {{context:Field}} to allow enumerators [{string.Join(", ", enums)}], " +
                              $"but found [{string.Join(", ", (Subject as EnumField)!.Enumerators)}]");

                return this;
            }

            public FieldAssertion WithDefault(object? defaultValue) { return HavingDefault(defaultValue ?? DBNull.Value); }
            public FieldAssertion WithNoDefault() { return HavingDefault(null); }

            [CustomAssertion] public FieldAssertion AtColumn(int column) {
                Execute.Assertion
                    .ForCondition(column == column_)
                    .FailWith($"Expected {{context:Field}} to be at column index #{column}, bound found column #{column_}");
                return this;
            }
            public FieldAssertion AtSomeColumn() {
                return this;
            }


            [CustomAssertion] private FieldAssertion WithNullability(bool expectNullable) {
                Execute.Assertion
                    .ForCondition((Subject.Nullability == IsNullable.Yes) == expectNullable)
                    .FailWith($"Expected {{context::Field}} to be {(expectNullable ? "nullable" : "non-nullable")}");
                return this;
            }
            [CustomAssertion] private FieldAssertion OfType(DBType expected) {
                Execute.Assertion
                    .ForCondition(Subject.DataType == expected)
                    .FailWith($"Expected {{context:Field}} to be of type {expected}, but found {Subject.DataType}");
                return this;
            }
            [CustomAssertion] private FieldAssertion HavingDefault(object? value) {
                if (value is not null && value.GetType().IsEnum) {
                    value = value.ToString()!.Replace(", ", "|");
                }
                var expected = value is null ? "no default" : $"default value of {value.ForDisplay()}";

                Subject.DefaultValue.Match(
                    some: v => {
                        Execute.Assertion
                            .ForCondition(v.Datum.Equals(value))
                            .FailWith($"Expected {{context:Field}} to have {expected}, but found default value of {v}");
                    },
                    none: () => {
                        Execute.Assertion
                            .ForCondition(value is null)
                            .FailWith($"Expected {{context:Field}} to have {expected}, but found no default value");
                    }
                );

                return this;
            }


            private readonly TableAssertion parent_;
            private readonly int column_;
        }

        public class KeyAssertion : Primitives.ObjectAssertions {
            public new IKey Subject { get; }
            public KeyAssertion(IKey subject, TableAssertion parent)
                : base(subject)
            {
                Subject = subject;
                Identifier = subject is PrimaryKey ? "Primary Key" : "Candidate Key";
                parent_ = parent;
            }
            protected override string Identifier { get; }

            public TableAssertion And => parent_;

            public KeyAssertion WithName(string keyName) { return HavingName(keyName); }
            public KeyAssertion WithoutName() { return HavingName(null); }

            [CustomAssertion] public KeyAssertion OfFields(string first, params string[] rest) {
                var actual = Subject.Fields.Select(f => f.Name.ToString()).ToHashSet();
                var expected = rest.Prepend(first).ToHashSet();

                Execute.Assertion
                    .ForCondition(actual.SetEquals(expected))
                    .FailWith($"Expected {{Context:Table}} to have {Identifier} with Fields" +
                              $"[{string.Join(", ", expected)}], but found Fields [{string.Join(", ", actual)}]");
                return this;
            }


            [CustomAssertion] private KeyAssertion HavingName(string? keyName) {
                var expected = keyName is null ? "unnamed" : $"named \"{keyName}\"";

                Subject.Name.Match(
                    some: n => {
                        Execute.Assertion
                            .ForCondition(n.ToString() == keyName)
                            .FailWith($"Expected {Identifier} of {{context:Table}} to be {expected}, but it is named \"{n}\"");
                    },
                    none: () => {
                        Execute.Assertion
                            .ForCondition(keyName is null)
                            .FailWith($"Expected {Identifier} of {{context:Table}} to be {expected}, but it is unnamed");
                    }
                );

                return this;
            }


            private readonly TableAssertion parent_;
        }

        public class CandidateKeysAssertion : Primitives.ObjectAssertions {
            public new IEnumerable<CandidateKey> Subject { get; }
            public CandidateKeysAssertion(IEnumerable<CandidateKey> subject, HashSet<KeyName> tracker, TableAssertion parent)
                : base(parent) { Subject = subject; parent_ = parent; tracker_ = tracker; }
            protected override string Identifier => "Candidate Keys";

            public TableAssertion And => parent_;

            [CustomAssertion] public KeyAssertion WithName(string keyName) {
                foreach (var candidate in Subject) {
                    if (candidate.Name.Contains(new KeyName(keyName))) {
                        tracker_.Add(new KeyName(keyName));
                        return new KeyAssertion(candidate, parent_);
                    }
                }

                Execute.Assertion
                    .ForCondition(false)
                    .FailWith($"Expected {{context:Table}} to have Candidate Key with name \"{keyName}\", but no such Candidate Key found");

                throw new ApplicationException("UNREACHABLE CODE!");
            }
            [CustomAssertion] public CandidateKeysAssertion ThatIsAnonymous() {
                var prefix = UniqueAttribute.ANONYMOUS_PREFIX;
                var anonymous = Subject.Where(k => k.Name.Exists(n => n.ToString().StartsWith(prefix)));
                Execute.Assertion
                    .ForCondition(!anonymous.IsEmpty())
                    .FailWith("Expected {context:Table} to have at least one anonymous Candidate Key, but none found");

                return new CandidateKeysAssertion(anonymous, tracker_, parent_);
            }
            [CustomAssertion] public KeyAssertion OfFields(string first, params string[] rest) {
                foreach (var candidate in Subject) {
                    try {
                        var impl = new KeyAssertion(candidate, parent_).OfFields(first, rest);
                        tracker_.Add(candidate.Name.Unwrap());
                        return impl;
                    }
                    catch (Exception) {}
                }

                Execute.Assertion
                    .ForCondition(false)
                    .FailWith("Expected {context:Table} to have Candidate Key with Fields " +
                              $"[{string.Join(", ", rest.Prepend(first))}], but no such Candidate Key found");

                throw new ApplicationException("UNREACHABLE CODE!");
            }


            private readonly TableAssertion parent_;
            private readonly HashSet<KeyName> tracker_;
        }
    }
}