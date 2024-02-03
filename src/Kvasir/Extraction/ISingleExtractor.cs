using System;
using System.Collections.Generic;

namespace Kvasir.Extraction {
    /// <summary>
    ///   The interface describing a component that extracts a single value from a CLR source object.
    /// </summary>
    internal interface ISingleExtractor : IMultiExtractor {
        /// <summary>
        ///   The <see cref="Type"/> of object produced by this <see cref="ISingleExtractor"/>.
        /// </summary>
        Type ResultType { get; }

        /// <summary>
        ///   Run the extraction logic over a CLR source object, producing a single value.
        /// </summary>
        /// <param name="source">
        ///   The CLR source object.
        /// </param>
        /// <returns>
        ///   If <paramref name="source"/> is <see langword="null"/>, then <see langword="null"/>. Otherwise, a single
        ///   value extracted from <paramref name="source"/> that is either <see langword="null"/> or an instance of
        ///   <see cref="ResultType"/>.
        /// </returns>
        new object? ExtractFrom(object? source);

        /// <inheritdoc/>
        IEnumerable<object?> IMultiExtractor.ExtractFrom(object? source) {
            yield return ExtractFrom(source);
        }
    }
}
