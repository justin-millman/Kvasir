using FluentAssertions.Execution;
using Kvasir.Localization;
using Kvasir.Relations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FluentAssertions {
    internal static partial class AssertionExtensions {
        public static RelationAssertion Should<T>(this RelationList<T> self) where T : notnull {
            return new RelationAssertion(self);
        }
        public static RelationAssertion Should<T>(this IReadOnlyRelationList<T> self) where T : notnull {
            return new RelationAssertion(self);
        }
        public static RelationAssertion Should<T>(this RelationSet<T> self) where T : notnull {
            return new RelationAssertion(self);
        }
        public static RelationAssertion Should<T>(this IReadOnlyRelationSet<T> self) where T : notnull {
            return new RelationAssertion(self);
        }
        public static RelationAssertion Should<K, V>(this RelationMap<K, V> self) where K : notnull where V : notnull {
            return new RelationAssertion(self);
        }
        public static RelationAssertion Should<K, V>(this IReadOnlyRelationMap<K, V> self) where K : notnull where V : notnull {
            return new RelationAssertion(self);
        }
        public static RelationAssertion Should<T>(this RelationOrderedList<T> self) where T : notnull {
            return new RelationAssertion(self);
        }
        public static RelationAssertion Should<T>(this IReadOnlyRelationOrderedList<T> self) where T : notnull {
            return new RelationAssertion(self);
        }
        public static RelationAssertion Should<K, C, V>(this Localization<K, C, V> self) where K : notnull where C : notnull {
            return new RelationAssertion(self);
        }
        public static RelationAssertion Should<K, C, V>(this IReadOnlyLocalization<K, C, V> self) where K : notnull where C : notnull {
            return new RelationAssertion(self);
        }

        public class RelationAssertion : Primitives.ObjectAssertions {
            public new IRelation Subject { get; }
            public List<(object Item, Status Status)> SubjectList { get; }
            public RelationAssertion(IRelation subject)
                : base(subject) {

                Subject = subject;
                SubjectList = new List<(object Item, Status status)>();

                var iter = subject.GetEnumerator();
                while (iter.MoveNext()) {
                    SubjectList.Add(iter.Current);
                }
            }
            protected override string Identifier => "Relation";

            [CustomAssertion]
            public AndConstraint<RelationAssertion> HaveEntryCount(int count, string because = "",
                params object[] becauseArgs) {

                Execute.Assertion
                    .BecauseOf(because, becauseArgs)
                    .ForCondition(SubjectList.Count == count)
                    .FailWith($"Expected {{context:relation}} to expose {count} entr{(count == 1 ? "y" : "ies")}" +
                              $"{{reason}}, but got {SubjectList.Count}");

                return new AndConstraint<RelationAssertion>(this);
            }

            [CustomAssertion]
            public AndConstraint<RelationAssertion> HaveUnsavedEntryCount(int count, string because = "",
                params object[] becauseArgs) {

                var actual = 0;
                var iter = Subject.GetEnumerator();
                while (iter.MoveNext()) {
                    if (iter.Current.Status != Status.Saved) {
                        ++actual;
                    }
                }

                Execute.Assertion
                    .BecauseOf(because, becauseArgs)
                    .ForCondition(actual == count)
                    .FailWith($"Expected {{context:relation}} to have {count} unsaved entr{(count == 1 ? "y" : "ies")}" +
                              $"{{reason}}, but got {actual}");

                return new AndConstraint<RelationAssertion>(this);
            }

            [CustomAssertion]
            public AndConstraint<RelationAssertion> HaveConnectionType<T>(string because = "",
                params object[] becauseArgs) {

                var flags = BindingFlags.Static | BindingFlags.NonPublic;
                var name = nameof(IRelation.ConnectionType);
                var reader = Subject.GetType().GetProperties(flags).First(p => p.Name.Contains(name))!.GetMethod!;
                var connectionType = (Type)reader.Invoke(null, new object?[] {})!;

                Execute.Assertion
                    .BecauseOf(because, becauseArgs)
                    .ForCondition(connectionType == typeof(T))
                    .FailWith($"Expected {{context:relation}} to have connection type of {typeof(T).Name}{{reason}}");

                return new AndConstraint<RelationAssertion>(this);
            }

            [CustomAssertion]
            public AndConstraint<RelationAssertion> ExposeDeletesFirst(string because = "",
                params object[] becauseArgs) {

                var firstNonDeleted = SubjectList.FindIndex(e => e.Status != Status.Deleted);
                var lastDeleted = SubjectList.FindIndex(e => e.Status == Status.Deleted);

                Execute.Assertion
                    .BecauseOf(because, becauseArgs)
                    .ForCondition(firstNonDeleted == -1 || firstNonDeleted > lastDeleted)
                    .FailWith("Expected {context:relation} to expose all DELETED entries first{reason}");

                return new AndConstraint<RelationAssertion>(this);
            }

            [CustomAssertion]
            public AndConstraint<RelationAssertion> ExposeEntry(object item, Status status, int count = 1,
                string because = "", params object[] becauseArgs) {

                Execute.Assertion
                    .BecauseOf(because, becauseArgs)
                    .ForCondition(SubjectList.Count(entry => entry.Item.Equals(item) && entry.Status.Equals(status)) == count)
                    .FailWith($"Expected {{context:relation}} to expose the entry ({item}, {status}){{reason}} (x{count})");

                return new AndConstraint<RelationAssertion>(this);
            }

            [CustomAssertion]
            public AndConstraint<RelationAssertion> NotExposeEntryFor(object item, string because = "",
                params object[] becauseArgs) {

                Execute.Assertion
                    .BecauseOf(because, becauseArgs)
                    .ForCondition(SubjectList.FindIndex(entry => entry.Item.Equals(item)) == -1)
                    .FailWith($"Expected {{context:relation}} not to expose an entry for {item}{{reason}}");

                return new AndConstraint<RelationAssertion>(this);
            }
        }
    }
}
