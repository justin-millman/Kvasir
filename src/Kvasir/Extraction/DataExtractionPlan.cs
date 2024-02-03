using Cybele.Extensions;
using Kvasir.Schema;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Kvasir.Extraction {
    /// <summary>
    ///   A plan that describes how to extract a set of <see cref="DBValue">database values</see> from a single CLR
    ///   source object.
    /// </summary>
    internal sealed class DataExtractionPlan {
        /// <summary>
        ///   The <see cref="Type"/> of object from which this <see cref="DataExtractionPlan"/> extracts values.
        /// </summary>
        public Type SourceType { get; }

        /// <summary>
        ///   Construct a new <see cref="DataExtractionPlan"/>.
        /// </summary>
        /// <param name="extractors">
        ///   The collection of extractors that produce the constituent values during <see cref="Extraction"/>.
        /// </param>
        public DataExtractionPlan(IEnumerable<IMultiExtractor> extractors) {
            Debug.Assert(extractors is not null && !extractors.IsEmpty());
            Debug.Assert(extractors.AllSame(e => e.SourceType));

            extractors_ = new List<IMultiExtractor>(extractors);
            SourceType = extractors_[0].SourceType;
        }

        /// <summary>
        ///   Run the extraction logic over a CLR source object, producing one or more database values.
        /// </summary>
        /// <param name="source">
        ///   The CLR source object.
        /// </param>
        /// <returns>
        ///   One or more possibly database values, any of which may be <see cref="DBValue.NULL"/>. Each invocation of
        ///   this method on a particular <see cref="DataExtractionPlan"/> instance will return the same number of
        ///   database values.
        /// </returns>
        public IReadOnlyList<DBValue> ExtractFrom(object source) {
            Debug.Assert(source is not null && source.GetType().IsInstanceOf(SourceType));

            var results = new List<DBValue>();
            foreach (var extractor in extractors_) {
                results.AddRange(extractor.ExtractFrom(source).Select(v => DBValue.Create(v)));
            }
            return results;
        }


        private readonly IReadOnlyList<IMultiExtractor> extractors_;
    }
}
