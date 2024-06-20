using Kvasir.Schema;
using System.Collections.Generic;
using System.Linq;

namespace FluentAssertions {
    internal static partial class AssertionExtensions {
        public static DataRowsAssertion Should(this IEnumerable<IReadOnlyList<DBValue>> self) {
            return new DataRowsAssertion(self);
        }

        public class DataRowsAssertion : Collections.GenericCollectionAssertions<IReadOnlyList<DBValue>> {
            public new IEnumerable<IReadOnlyList<DBValue>> Subject { get; }
            public DataRowsAssertion(IEnumerable<IReadOnlyList<DBValue>> subject)
                : base(subject) {

                Subject = subject;
            }

            [CustomAssertion]
            public AndConstraint<DataRowsAssertion> ContainRow(params object?[] values) {
                base.Contain(e => e.SequenceEqual(values.Select(v => DBValue.Create(v))));
                return new AndConstraint<DataRowsAssertion>(this);
            }
        }
    }
}
