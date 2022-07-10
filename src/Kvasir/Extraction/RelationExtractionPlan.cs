﻿using Ardalis.GuardClauses;
using Cybele.Extensions;
using Kvasir.Relations;
using Kvasir.Schema;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Kvasir.Extraction {
    /// <summary>
    ///   A grouping of data extracted from an <see cref="Kvasir.Relations.IRelation"/>.
    /// </summary>
    public sealed record ExtractedRelationData(
        IEnumerable<IReadOnlyList<DBValue>> Insertions,
        IEnumerable<IReadOnlyList<DBValue>> Modifications,
        IEnumerable<IReadOnlyList<DBValue>> Deletions
    );

    /// <summary>
    ///   A description of the way in which data is extracted from an <see cref="IRelation"/> stored on a particular
    ///   Entity type and then transformed and prepared to be stored in a back-end database.
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
        ///   source entity.
        /// </param>
        /// <param name="plan">
        ///   The <see cref="EntityExtractionPlan"/> that describes how to extract data from an entry in the relation
        ///   obtained from <paramref name="relationExtractor"/>.
        /// </param>
        /// <pre>
        ///   The <see cref="IFieldExtractor.FieldType"/> of <paramref name="relationExtractor"/> is a type that
        ///   implements the <see cref="IRelation"/> interface (or is that interface itself)
        ///     --and--
        ///   The type of data stored in the relation obtained from <paramref name="relationExtractor"/> is compatible
        ///   with the <see cref="EntityExtractionPlan.ExpectedSource"/> of <paramref name="plan"/>.
        /// </pre>
        internal RelationExtractionPlan(IFieldExtractor relationExtractor, EntityExtractionPlan plan) {
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
        /// <param name="entity">
        ///   The source object on which to execute this <see cref="RelationExtractionPlan"/>.
        /// </param>
        /// <pre>
        ///   <see cref="ExpectedSource"/> is the dynamic type of <paramref name="entity"/> or is a base class or
        ///   interface thereof.
        /// </pre>
        /// <returns>
        ///   A <see cref="ExtractedRelationData"/> containing the values extracted from the relation targeted on
        ///   <paramref name="entity"/> by this <see cref="RelationExtractionPlan"/>.
        /// </returns>
        public ExtractedRelationData Execute(object entity) {
            Debug.Assert(entity.GetType().IsInstanceOf(ExpectedSource));

            // In order to keep the API of the extracted data wrapper "read only," we have to build up the containers
            // a priori and then construct the wrapper, rather than constructing the wrapper with empty containers and
            // then incrementally adding.
            var insertions = new List<IReadOnlyList<DBValue>>();
            var modifications = new List<IReadOnlyList<DBValue>>();
            var deletions = new List<IReadOnlyList<DBValue>>();

            // It's a little annoying that we can't just do a for-each loop over the contents of the Relation, but
            // that's inhibited by the GetEnumerator() function in the IRelation interface being internal. We don't
            // want to change that, because we only want the Framework to have access to that introspection.
            IRelation relation = (IRelation)extractor_.Execute(entity)!;
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

            return new ExtractedRelationData(insertions, modifications, deletions);
        }


        private readonly IFieldExtractor extractor_;
        private readonly EntityExtractionPlan plan_;
    }
}