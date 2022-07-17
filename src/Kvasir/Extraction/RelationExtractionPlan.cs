using Ardalis.GuardClauses;
using Cybele.Extensions;
using Kvasir.Relations;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Kvasir.Extraction {
    /// <summary>
    ///   A description of the way in which data is extracted from an <see cref="IRelation"/> stored on a particular
    ///   CLR object type and then transformed and prepared to be stored in a back-end database.
    /// </summary>
    public sealed class RelationExtractionPlan {
        /// <summary>
        ///   The <see cref="Type"/> of source object on which this <see cref="RelationExtractionPlan"/> is capable of
        ///   being executed.
        /// </summary>
        public Type ExpectedSource { get; }

        /// <summary>
        ///   Constructs a new <see cref="RelationExtractionPlan"/>.
        /// </summary>
        /// <param name="relationExtractor">
        ///   The <see cref="IFieldExtractor"/> that describes how to obtain the target <see cref="IRelation"/> from a
        ///   source object.
        /// </param>
        /// <param name="plan">
        ///   The <see cref="DataExtractionPlan"/> that describes how to extract data from an entry in the relation
        ///   obtained from <paramref name="relationExtractor"/>.
        /// </param>
        /// <pre>
        ///   The <see cref="IFieldExtractor.FieldType"/> of <paramref name="relationExtractor"/> is a type that
        ///   implements the <see cref="IRelation"/> interface (or is that interface itself)
        ///     --and--
        ///   The type of data stored in the relation obtained from <paramref name="relationExtractor"/> is compatible
        ///   with the <see cref="DataExtractionPlan.ExpectedSource"/> of <paramref name="plan"/>.
        /// </pre>
        internal RelationExtractionPlan(IFieldExtractor relationExtractor, DataExtractionPlan plan) {
            Guard.Against.Null(relationExtractor, nameof(relationExtractor));
            Guard.Against.Null(plan, nameof(plan));
            Debug.Assert(relationExtractor.FieldType.IsInstanceOf(typeof(IRelation)));
            
            // There's no good way to check on construction that the type of item exposed by the Relation matches the
            // source type expected by the subunit plan. Instead, we'll have to settle for that being checked when the
            // actual extraction is executed. This isn't ideal (it's heavily delayed from start-up), but it's what we
            // have.

            extractor_ = relationExtractor;
            plan_ = plan;
            ExpectedSource = relationExtractor.ExpectedSource;
        }

        /// <summary>
        ///   Execute this <see cref="RelationExtractionPlan"/> on a source object.
        /// </summary>
        /// <param name="source">
        ///   The source object on which to execute this <see cref="RelationExtractionPlan"/>.
        /// </param>
        /// <pre>
        ///   <see cref="ExpectedSource"/> is the dynamic type of <paramref name="source"/> or is a base class or
        ///   interface thereof.
        /// </pre>
        /// <returns>
        ///   A triplet containing the values extracted from the relation targeted on <paramref name="source"/> by this
        ///   <see cref="RelationExtractionPlan"/>.
        /// </returns>
        public (IEnumerable<Row> Insertions, IEnumerable<Row> Modifications, IEnumerable<Row> Deletions)
        Execute(object source) {
            Debug.Assert(source.GetType().IsInstanceOf(ExpectedSource));

            // In order to keep the API of the extracted data wrapper "read only," we have to build up the containers
            // a priori and then construct the wrapper, rather than constructing the wrapper with empty containers and
            // then incrementally adding.
            var insertions = new List<Row>();
            var modifications = new List<Row>();
            var deletions = new List<Row>();

            // It's a little annoying that we can't just do a for-each loop over the contents of the Relation, but
            // that's inhibited by the GetEnumerator() function in the IRelation interface being internal. We don't
            // want to change that, because we only want the Framework to have access to that introspection.
            IRelation relation = (IRelation)extractor_.Execute(source)!;
            var iter = relation.GetEnumerator();
            while (iter.MoveNext()) {
                switch (iter.Current.Status) {
                    case Status.Saved:
                        continue;
                    case Status.New:
                        insertions.Add(plan_.Execute(iter.Current.Item));
                        break;
                    case Status.Modified:
                        modifications.Add(plan_.Execute(iter.Current.Item));
                        break;
                    case Status.Deleted:
                        deletions.Add(plan_.Execute(iter.Current.Item));
                        break;
                    default:
                        throw new ApplicationException($"Switch statement over {nameof(Status)} exhausted");
                }
            }

            return (insertions, modifications, deletions);
        }


        private readonly IFieldExtractor extractor_;
        private readonly DataExtractionPlan plan_;
    }
}