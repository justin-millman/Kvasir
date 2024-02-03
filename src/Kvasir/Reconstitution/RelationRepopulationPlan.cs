using Cybele.Extensions;
using Kvasir.Extraction;
using Kvasir.Relations;
using Kvasir.Schema;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Kvasir.Reconstitution {
    /// <summary>
    ///   A plan that describes how to repopulate a <see cref="IRelation">Relation</see> from a collection of rows of
    ///   <see cref="DBValue">database values</see>.
    /// </summary>
    internal sealed class RelationRepopulationPlan {
        /// <summary>
        ///   The <see cref="Type"/> of object on which the <see cref="IRelation">Relation</see> repopulated by this
        ///   <see cref="RelationRepopulationPlan"/> resides.
        /// </summary>
        public Type SourceType { get; }

        /// <summary>
        ///   Construct a new <see cref="RelationRepopulationPlan"/>.
        /// </summary>
        /// <param name="relationExtractor">
        ///   The <see cref="ISingleExtractor"/> that pulls the target <see cref="IRelation">Relation</see>  out of the
        ///   original CLR object.
        /// </param>
        /// <param name="elementReconstitutor">
        ///   The <see cref="DataReconstitutionPlan"/> that describes how to reconstitute an element of the
        ///   <see cref="IRelation">Relation</see> for a list of <see cref="DBValue">database values</see>.
        /// </param>
        /// <param name="repopulator">
        ///   The <see cref="IRepopulator"/> that describes how to repopulate elements into the
        ///   <see cref="IRelation">Relation</see>.
        /// </param>
        public RelationRepopulationPlan(ISingleExtractor relationExtractor, DataReconstitutionPlan elementReconstitutor,
            IRepopulator repopulator) {

            Debug.Assert(relationExtractor is not null);
            Debug.Assert(relationExtractor.ResultType.IsInstanceOf(typeof(IRelation)));
            Debug.Assert(elementReconstitutor is not null);
            Debug.Assert(repopulator is not null);

            relationExtractor_ = relationExtractor;
            elementReconstitutor_ = elementReconstitutor;
            repopulator_ = repopulator;
            SourceType = relationExtractor_.SourceType;
        }

        /// <summary>
        ///   Repopulate a <see cref="IRelation">Relation</see> with a collection of rows of
        ///   <see cref="DBValue">database values</see>.
        /// </summary>
        /// <param name="source">
        ///   The object on which the target <see cref="IRelation">Relation</see> resides. If this is
        ///   <see langword="null"/>, no repopulation will be performed.
        /// </param>
        /// <param name="dbRows">
        ///   The rows of <see cref="DBValue">database values</see> from which to repopulate the target
        ///   <see cref="IRelation">Relation</see>.
        /// </param>
        public void Repopulate(object? source, IEnumerable<IReadOnlyList<DBValue>> dbRows) {
            Debug.Assert(source is null || source.GetType().IsInstanceOf(SourceType));
            Debug.Assert(dbRows is not null);

            if (source is not null && !dbRows.IsEmpty()) {
                var relation = (IRelation)relationExtractor_.ExtractFrom(source)!;
                var elements = dbRows.Select(dbRow => elementReconstitutor_.ReconstituteFrom(dbRow));
                repopulator_.Repopulate(relation, elements);
            }
        }


        private readonly ISingleExtractor relationExtractor_;
        private readonly DataReconstitutionPlan elementReconstitutor_;
        private readonly IRepopulator repopulator_;
    }
}
