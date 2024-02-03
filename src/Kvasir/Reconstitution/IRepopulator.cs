using Kvasir.Relations;
using System.Collections.Generic;

namespace Kvasir.Reconstitution {
    /// <summary>
    ///   The interface describing a component that repopulates a <see cref="IRelation">Relation</see>.
    /// </summary>
    internal interface IRepopulator {
        /// <summary>
        ///   Repopulate a <see cref="IRelation">Relation</see>.
        /// </summary>
        /// <param name="relation">
        ///   The <see cref="IRelation">Relation</see> to repopulate.
        /// </param>
        /// <param name="elements">
        ///   A possibly empty collection of element to repopulate into <paramref name="relation"/>.
        /// </param>
        void Repopulate(IRelation relation, IEnumerable<object> elements);
    }
}
