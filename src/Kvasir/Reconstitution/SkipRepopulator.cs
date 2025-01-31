using Kvasir.Relations;
using System.Collections.Generic;
using System.Diagnostics;

namespace Kvasir.Reconstitution {
    /// <summary>
    ///   An <see cref="IRepopulator"/> that skips the repopulation operation (i.e. turns it into a no-op).
    /// </summary>
    internal sealed class SkipRepopulator : IRepopulator {
        /// <inheritdoc/>
        public void Repopulate(IRelation relation, IEnumerable<object> elements) {
            Debug.Assert(relation is not null);
            Debug.Assert(elements is not null);
        }
    }
}
