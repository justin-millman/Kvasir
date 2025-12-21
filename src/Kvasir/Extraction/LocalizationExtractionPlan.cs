using Kvasir.Localization;
using Kvasir.Relations;
using System;

namespace Kvasir.Extraction {
    /// <summary>
    ///   A plan that extracts rows of database values from one or more <see cref="ILocalization">Localizations</see> to
    ///   be inserted, updated, and/or deleted from a back-end table.
    /// </summary>
    internal sealed class LocalizationExtractionPlan {
        /// <summary>
        ///   The <see cref="Type"/> of object from which this <see cref="LocalizationExtractionPlan"/> extracts values.
        /// </summary>
        public Type SourceType { get; }

        // [TOOD] - Constructor(s)

        /// <summary>
        ///   Run the extraction logic over a CLR source object, producing <see cref="RelationData"/>.
        /// </summary>
        /// <param name="source">
        ///   The CLR source object.
        /// </param>
        /// <returns>
        ///   The <see cref="RelationData"/> containing the insertions, modifications, and deletions of the
        ///   <see cref="ILocalization">Localization</see> extracted from <paramref name="source"/>.
        /// </returns>
        public RelationData ExtractFrom(object source) {
            throw new NotImplementedException();
        }

        /// <summary>
        ///   Canonicalize the target <see cref="ILocalization">Localization(s)</see> on a CLR source.
        /// </summary>
        /// <param name="source">
        ///   The CLR source object.
        /// </param>
        /// <seealso cref="IRelation.Canonicalize"/>
        public void Canonicalize(object source) {
            throw new NotImplementedException();
        }
    }
}
