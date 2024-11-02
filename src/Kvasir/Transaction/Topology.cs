using Cybele.Extensions;
using Kvasir.Translation;
using System.Collections.Generic;
using System.Linq;

namespace Kvasir.Transaction {
    /// <summary>
    ///   Some utility functions for performing topological queries needed by the Transaction Layer.
    /// </summary>
    internal static class Topology {
        /// <summary>
        ///   Topologically sorts the Principal Table definitions from a set of
        ///   <see cref="EntityTranslation">translations</see>.
        /// </summary>
        /// <remarks>
        ///   The topological sort represents the order in which the Principal Tables must be created to avoid defining
        ///   Foreign Key references to tables that do not yet exist. Consequently, it is the same order in which data
        ///   must be loaded out of the back-end database in order to ensure that references can be resolved to existing
        ///   CLR objects. If a Principal Table <c>A</c> occurs in the topological order before <c>B</c>, that means
        ///   there is no chain of reference fields from <c>A</c> to <c>B</c>. Relations are not included in the order
        ///   because they are all inherently independent of one another, and if ordered entirely after the Principal
        ///   Tables there will never be any issues.
        /// </remarks>
        /// <param name="translations">
        ///   The source <see cref="EntityTranslation">translatios</see>.
        /// </param>
        /// <returns>
        ///   The topological order of the Principal Tables of <paramref name="translations"/>.
        /// </returns>
        public static IEnumerable<PrincipalTableDef> OrderEntities(IReadOnlyList<EntityTranslation> translations) {
            // The `indexMap` is effectively our collection of graph vertices. Each vertex is a translation (identified
            // by its Principal Table); each vertex's index value doesn't actually matter, we just need a way to mark
            // rows/columns in the adjacency matrix. The Principal Table is used as the key because that's what Foreign
            // Keys reference.
            var indexMap = translations
                .Select((translation, index) => (translation, index))
                .ToDictionary(
                    keySelector: pair => pair.translation.Principal.Table,
                    elementSelector: pair => pair.index
                );

            // This is our directed graph adjacency matrix. A directed edge from the vertex representing Entity A to the
            // vertex representing Entity B means that Entity A is referenced by Entity B and must therefore be handled
            // first; such an edge is indicated by a value of `true` in the matrix. The Translation Layer guarantees
            // that the graph is acyclic.
            var adjacencyMatrix = new List<List<bool>>();
            for (int i = 0; i < translations.Count; ++i) {
                adjacencyMatrix.Add(Enumerable.Repeat(false, translations.Count).ToList());
            }

            // We're also going to keep a degree counter; this will make our topological sort algorithm slightly more
            // efficient.
            var degrees = Enumerable.Repeat(0, translations.Count).ToList();

            // Now we build up the initial adjacency matrix. Multiple Foreign Keys against the same foreign table only
            // count for a single edge. We're also guaranteed to have at least one vertex with no outbound edges, since
            // the graph is guaranteed to be acyclic.
            var leafs = new Queue<int>();
            foreach (var translation in translations) {
                var principalIndex = indexMap[translation.Principal.Table];
                foreach (var fk in translation.Principal.Table.ForeignKeys) {
                    var referenceIndex = indexMap[fk.ReferencedTable];
                    if (!adjacencyMatrix[referenceIndex][principalIndex]) {
                        ++degrees[principalIndex];
                        adjacencyMatrix[referenceIndex][principalIndex] = true;
                    }
                }
                if (translation.Principal.Table.ForeignKeys.IsEmpty()) {
                    leafs.Enqueue(principalIndex);
                }
            }

            // With the initial adjacency matrix complete, we now execute a topological ordering. We will use Kahn's
            // Algorithm to do this; we already have our starting set of vertices (the "leafs"). Note that the resulting
            // topological order is not necessarily unique, but it is reproducible for a given input ordering.
            while (!leafs.IsEmpty()) {
                var referenceIndex = leafs.Dequeue();
                yield return translations[referenceIndex].Principal;

                for (int principalIndex = 0; principalIndex < translations.Count; ++principalIndex) {
                    if (adjacencyMatrix[referenceIndex][principalIndex]) {
                        if (--degrees[principalIndex] == 0) {
                            leafs.Enqueue(principalIndex);
                        }
                    }
                }
            }
        }
    }
}
