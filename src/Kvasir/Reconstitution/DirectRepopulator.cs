using Kvasir.Relations;
using System.Collections.Generic;
using System.Diagnostics;

namespace Kvasir.Reconstitution {
    /// <summary>
    ///   An <see cref="IRepopulator"/> that passes each element directly to the
    ///   <see cref="IRelation.Repopulate(object)"/> method of the target <see cref="IRelation">Relation</see>.
    /// </summary>
    internal sealed class DirectRepopulator : IRepopulator {
        /// <inheritdoc/>
        public void Repopulate(IRelation relation, IEnumerable<object> elements) {
            Debug.Assert(relation is not null && !relation.GetEnumerator().MoveNext());
            Debug.Assert(elements is not null);

            foreach (var element in elements) {
                relation.Repopulate(element);
            }
        }
    }
}
