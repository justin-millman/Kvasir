using Cybele.Core;
using Cybele.Extensions;
using Kvasir.Localization;
using Kvasir.Relations;
using Kvasir.Schema;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Kvasir.Extraction {
    /// <summary>
    ///   A plan that extracts rows of database values from a
    ///   <see cref="Localization{TKey, TLocale, TValue}">Localization</see> to be inserted, updated, and/or deleted
    ///   from a back-end table.
    /// </summary>
    internal sealed class LocalizationExtractionPlan {
        /// <summary>
        ///   The <see cref="Type"/> of object from which this <see cref="LocalizationExtractionPlan"/> extracts values.
        /// </summary>
        public Type SourceType { get; }

        /// <summary>
        ///   Constructs a new <see cref="LocalizationExtractionPlan"/>.
        /// </summary>
        /// <param name="valuesExtractor">
        ///   The <see cref="RelationExtractionPlan"/> describing how to obtain localized values from the actual source
        ///   object.
        /// </param>
        public LocalizationExtractionPlan(RelationExtractionPlan valuesExtractor) {
            Debug.Assert(valuesExtractor is not null);

            SourceType = valuesExtractor.SourceType;
            keyExtractor_ = new ReadPropertyExtractor(new PropertyChain(SourceType, "Key"));
            valuesExtractor_ = valuesExtractor;
        }

        /// <summary>
        ///   Run the extraction logic over a CLR source object, producing <see cref="LocalizationData"/>.
        /// </summary>
        /// <param name="source">
        ///   The CLR source object.
        /// </param>
        /// <returns>
        ///   The <see cref="LocalizationData"/> containing the insertions, modifications, and deletions extracted from
        ///   <paramref name="source"/>.
        /// </returns>
        public LocalizationData ExtractFrom(object source) {
            Debug.Assert(source is null || source.GetType().IsInstanceOf(SourceType));

            var insertions = new List<IReadOnlyList<DBValue>>();
            var modifications = new List<IReadOnlyList<DBValue>>();
            var deletions = new List<IReadOnlyList<DBValue>>();

            if (source is not null) {
                var key = DBValue.Create(keyExtractor_.ExtractFrom(source));
                var values = valuesExtractor_.ExtractFrom(source);

                insertions.AddRange(values.Insertions.Select(list => list.Prepend(key).ToList()));
                modifications.AddRange(values.Modifications.Select(list => list.Prepend(key).ToList()));
                deletions.AddRange(values.Deletions.Select(list => list.Prepend(key).ToList()));
            }

            return new LocalizationData(Insertions: insertions, Modifications: modifications, Deletions: deletions);
        }

        /// <summary>
        ///   Canonicalize the target <see cref="Localization{TKey, TLocale, TValue}">Localization</see> on a CLR
        ///   source.
        /// </summary>
        /// <param name="source">
        ///   The CLR source object.
        /// </param>
        /// <seealso cref="IRelation.Canonicalize"/>
        public void Canonicalize(object source) {
            valuesExtractor_.Canonicalize(source);
        }


        private readonly ReadPropertyExtractor keyExtractor_;
        private readonly RelationExtractionPlan valuesExtractor_;
    }

    /// <summary>
    ///   A triple of lists describing the data <see cref="LocalizationExtractionPlan">extracted from a Relation</see>.
    /// </summary>
    internal readonly record struct LocalizationData(
        IReadOnlyList<IReadOnlyList<DBValue>> Insertions,
        IReadOnlyList<IReadOnlyList<DBValue>> Modifications,
        IReadOnlyList<IReadOnlyList<DBValue>> Deletions
    );
}
