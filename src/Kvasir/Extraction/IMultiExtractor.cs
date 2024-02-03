using System;
using System.Collections.Generic;

namespace Kvasir.Extraction {
    /// <summary>
    ///   The interface describing a component that extracts one or more values from a single CLR source object.
    /// </summary>
    internal interface IMultiExtractor {
        /// <summary>
        ///   The <see cref="Type"/> of object from which this <see cref="IMultiExtractor"/> extracts values.
        /// </summary>
        Type SourceType { get; }

        /// <summary>
        ///   Run the extraction logic over a CLR source object, producing one or more values.
        /// </summary>
        /// <param name="source">
        ///   The CLR source object.
        /// </param>
        /// <returns>
        ///   One or more possibly <see langword="null"/> values. If <paramref name="source"/> is
        ///   <see langword="null"/>, then all the values will be <see langword="null"/>. Each invocation of this
        ///   method will return the same number of values.
        /// </returns>
        IEnumerable<object?> ExtractFrom(object? source);
    }
}
