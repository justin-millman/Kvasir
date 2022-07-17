using Ardalis.GuardClauses;
using Cybele.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Kvasir.Reconstitution {
    /// <summary>
    ///   A description of the way in which data for a particular Relation is to be extracted from a back-end database,
    ///   transformed, and reimagined as the elements of some CLR collection.
    /// </summary>
    public sealed class RelationReconstitutionPlan {
        /// <summary>
        ///   The <see cref="Type"/> of object on which this <see cref="RelationReconstitutionPlan"/> is expected to
        ///   operate.
        /// </summary>
        public Type ExpectedSubject { get; }

        /// <summary>
        ///   Constructs a new <see cref="RelationReconstitutionPlan"/>.
        /// </summary>
        /// <param name="plan">
        ///   The <see cref="DataReconstitutionPlan"/> with which to reconstitute the individual elements of the
        ///   relation.
        /// </param>
        /// <param name="repopulator">
        ///   The <see cref="IRepopulator"/> dictating how elements are to be placed into the
        ///   <see cref="Relations.IRelation"/> during repopulation.
        /// </param>
        internal RelationReconstitutionPlan(DataReconstitutionPlan plan, IRepopulator repopulator) {
            Guard.Against.Null(plan, nameof(plan));
            Guard.Against.Null(repopulator, nameof(repopulator));

            plan_ = plan;
            repopulator_ = repopulator;
            ExpectedSubject = repopulator.ExpectedSubject;
        }

        /// <summary>
        ///   Execute this <see cref="RelationReconstitutionPlan"/> to repopulate some
        ///   <see cref="Kvasir.Relations.IRelation"/> on some CLR object.
        /// </summary>
        /// <param name="subject">
        ///   The non-<see langword="null"/> object on which to repopulate a <see cref="Relations.IRelation"/>.
        /// </param>
        /// <param name="rawValues">
        ///   The list of raw database values from which to reconstitute the elements with which to repopulate the
        ///   target <see cref="Relations.IRelation"/>.
        /// </param>
        /// <pre>
        ///   <paramref name="subject"/> is an instance of <see cref="ExpectedSubject"/>
        ///     --and--
        ///   Each of the "rows" in <paramref name="rawValues"/> is non-empty.
        /// </pre>
        public void RepopulateFrom(object subject, IEnumerable<Row> rawValues) {
            Guard.Against.Null(subject, nameof(subject));
            Guard.Against.Null(rawValues, nameof(rawValues));
            Debug.Assert(subject.GetType().IsInstanceOf(ExpectedSubject));
            Debug.Assert(rawValues.All(r => !r.IsEmpty()));

            var entries = new List<object>();
            foreach (var row in rawValues) {
                entries.Add(plan_.ReconstituteFrom(row));
            }

            if (entries.Count > 0) {
                repopulator_.Execute(subject, entries);
            }
        }


        private readonly DataReconstitutionPlan plan_;
        private readonly IRepopulator repopulator_;
    }
}
