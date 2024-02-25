using Cybele.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Kvasir.Extraction {
    /// <summary>
    ///   A <see cref="IMultiExtractor"/> that takes a single CLR object, extracts a single value from it, and then runs
    ///   extraction logic over that single value.
    /// </summary>
    internal sealed class CurryingExtractor : IMultiExtractor {
        /// <inheritdoc/>
        public Type SourceType { get; }

        /// <summary>
        ///   Construct a new <see cref="CurryingExtractor"/>.
        /// </summary>
        /// <param name="sourceExtractor">
        ///   The <see cref="ISingleExtractor"/> that produces the single nested value on which to run additional
        ///   extraction logic.
        /// </param>
        /// <param name="valuesExtractor">
        ///   The additional extraction logic to run over the value produced by <paramref name="sourceExtractor"/>.
        /// </param>
        public CurryingExtractor(ISingleExtractor sourceExtractor, IMultiExtractor valuesExtractor) {
            Debug.Assert(sourceExtractor is not null);
            Debug.Assert(valuesExtractor is not null);
            Debug.Assert(sourceExtractor.ResultType == valuesExtractor.SourceType);

            sourceExtractor_ = sourceExtractor;
            valuesExtractor_ = valuesExtractor;
            SourceType = sourceExtractor_.SourceType;
        }

        /// <inheritdoc/>
        public IEnumerable<object?> ExtractFrom(object? source) {
            Debug.Assert(source is null || source.GetType().IsInstanceOf(SourceType));
            return valuesExtractor_.ExtractFrom(sourceExtractor_.ExtractFrom(source));
        }


        private readonly ISingleExtractor sourceExtractor_;
        private readonly IMultiExtractor valuesExtractor_;
    }
}
