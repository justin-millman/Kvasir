using Cybele.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Kvasir.Extraction {
    /// <summary>
    ///   A <see cref="IMultiExtractor"/> that takes a single CLR source object and "decomposes" it into one or more
    ///   constituent components.
    /// </summary>
    internal sealed class DecomposingExtractor : IMultiExtractor {
        /// <inheritdoc/>
        public Type SourceType { get; }

        /// <summary>
        ///   Construct a new <see cref="DecomposingExtractor"/>.
        /// </summary>
        /// <param name="extractors">
        ///   The collection of extractors that produce the constituent values during
        ///   <see cref="ExtractFrom(object?)">extraction</see>.
        /// </param>
        public DecomposingExtractor(IEnumerable<IMultiExtractor> extractors) {
            Debug.Assert(extractors is not null && !extractors.IsEmpty());
            Debug.Assert(extractors.AllSame(e => e.SourceType));

            extractors_ = new List<IMultiExtractor>(extractors);
            SourceType = extractors_[0].SourceType;
        }

        /// <inheritdoc/>
        public IEnumerable<object?> ExtractFrom(object? source) {
            foreach (var extractor in extractors_) {
                foreach (var value in extractor.ExtractFrom(source)) {
                    yield return value;
                }
            }
        }


        private readonly IReadOnlyList<IMultiExtractor> extractors_;
    }
}
