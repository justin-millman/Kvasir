using Cybele.Extensions;
using Kvasir.Relations;
using Kvasir.Schema;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Kvasir.Extraction {
    /// <summary>
    ///   A plan that extracts rows of database values from a <see cref="IRelation">Relation</see> to be inserted,
    ///   updated, and/or deleted from a back-end table.
    /// </summary>
    internal sealed class RelationExtractionPlan {
        /// <summary>
        ///   The <see cref="Type"/> of object from which this <see cref="RelationExtractionPlan"/> extracts values.
        /// </summary>
        public Type SourceType { get; }
         
        /// <summary>
        ///   Construct a new <see cref="RelationExtractionPlan"/>.
        /// </summary>
        /// <param name="relationExtractor">
        ///   The <see cref="ISingleExtractor"/> describing how to obtain the <see cref="IRelation">Relation</see> from
        ///   which to extract actions from the actual source object.
        /// </param>
        /// <param name="elementExtractionPlan">
        ///   The <see cref="DataExtractionPlan"/> describing how to turn a single element of the target Relation into a
        ///   row of database values.
        /// </param>
        public RelationExtractionPlan(ISingleExtractor relationExtractor, DataExtractionPlan elementExtractionPlan) {
            Debug.Assert(relationExtractor is not null);
            Debug.Assert(elementExtractionPlan is not null);
            Debug.Assert(relationExtractor.ResultType.IsInstanceOf(typeof(IRelation)));

            relationExtractor_ = relationExtractor;
            elementExtractionPlan_ = elementExtractionPlan;
            SourceType = relationExtractor_.SourceType;
        }

        /// <summary>
        ///   Run the extraction logic over a CLR source object, producing <see cref="RelationData"/>.
        /// </summary>
        /// <param name="source">
        ///   The CLR source object.
        /// </param>
        /// <returns>
        ///   The <see cref="RelationData"/> containing the insertions, modifications, and deletions of the
        ///   <see cref="IRelation">Relation</see> extracted from <paramref name="source"/>.
        /// </returns>
        public RelationData ExtractFrom(object source) {
            Debug.Assert(source is null || source.GetType().IsInstanceOf(SourceType));

            var insertions = new List<IReadOnlyList<DBValue>>();
            var modifications = new List<IReadOnlyList<DBValue>>();
            var deletions = new List<IReadOnlyList<DBValue>>();

            var relation = (IRelation?)relationExtractor_.ExtractFrom(source);
            if (relation is not null) {
                // It's annoying that we can just use a standard foreach loop, but that is inhibited by the fact that
                // the GetEnumerator() function on the IRelation interface is internal; even though we're in the same
                // assembly, the language doesn't recognize it properly. We don't want to change the access modifier, so
                // we have to do a bit of manual iteration.
                var iter = relation.GetEnumerator();
                while (iter.MoveNext()) {
                    if (iter.Current.Status == Status.Saved) {
                        continue;
                    }

                    var elementData = elementExtractionPlan_.ExtractFrom(iter.Current.Item);
                    switch (iter.Current.Status) {
                        case Status.New:
                            insertions.Add(elementData);
                            break;
                        case Status.Modified:
                            modifications.Add(elementData);
                            break;
                        case Status.Deleted:
                            deletions.Add(elementData);
                            break;
                        default:
                            throw new ApplicationException($"Switch statement over {nameof(Status)} exhausted");
                    }
                }
            }

            return new RelationData(Insertions: insertions, Modifications: modifications, Deletions: deletions);
        }


        private readonly ISingleExtractor relationExtractor_;
        private readonly DataExtractionPlan elementExtractionPlan_;
    }

    /// <summary>
    ///   A triple of lists describing the data <see cref="RelationExtractionPlan">extracted from a Relation</see>.
    /// </summary>
    internal readonly record struct RelationData(
        IReadOnlyList<IReadOnlyList<DBValue>> Insertions,
        IReadOnlyList<IReadOnlyList<DBValue>> Modifications,
        IReadOnlyList<IReadOnlyList<DBValue>> Deletions
    );
}
