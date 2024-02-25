using Cybele.Core;
using Cybele.Extensions;
using System;
using System.Diagnostics;

namespace Kvasir.Extraction {
    /// <summary>
    ///   An <see cref="ISingleExtractor"/> that executes a <see cref="DataConverter"/> after extracting a value.
    /// </summary>
    internal sealed class ConvertingExtractor : ISingleExtractor {
        /// <inheritdoc/>
        public Type SourceType { get; }

        /// <inheritdoc/>
        public Type ResultType { get; }

        /// <summary>
        ///   Construct a new <see cref="ConvertingExtractor"/>.
        /// </summary>
        /// <param name="extractor">
        ///   The <see cref="ISingleExtractor"/> that produces the unconverted value.
        /// </param>
        /// <param name="converter">
        ///   The <see cref="DataConverter"/> to run over the value produced by <paramref name="extractor"/> during
        ///   <see cref="ExtractFrom(object?)">extraction</see>.
        /// </param>
        public ConvertingExtractor(ISingleExtractor extractor, DataConverter converter) {
            Debug.Assert(extractor is not null);
            Debug.Assert(converter is not null);
            Debug.Assert(extractor.ResultType.IsInstanceOf(converter.SourceType));

            extractor_ = extractor;
            converter_ = converter;
            SourceType = Nullable.GetUnderlyingType(extractor_.SourceType) ?? extractor_.SourceType;
            ResultType = Nullable.GetUnderlyingType(converter_.ResultType) ?? converter_.ResultType;
        }

        /// <inheritdoc/>
        public object? ExtractFrom(object? source) {
            Debug.Assert(source is null || source.GetType().IsInstanceOf(SourceType));
            return converter_.Convert(extractor_.ExtractFrom(source));
        }


        private readonly ISingleExtractor extractor_;
        private readonly DataConverter converter_;
    }
}
