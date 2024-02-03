using Cybele.Core;
using Cybele.Extensions;
using System;
using System.Diagnostics;

namespace Kvasir.Extraction {
    /// <summary>
    ///   A <see cref="ISingleExtractor"/> that produces its value by reading from a sequence of properties via
    ///   reflection.
    /// </summary>
    internal sealed class ReadPropertyExtractor : ISingleExtractor {
        /// <inheritdoc/>
        public Type SourceType { get; }

        /// <inheritdoc/>
        public Type ResultType { get; }

        /// <summary>
        ///   Construct a new <see cref="ReadPropertyExtractor"/>.
        /// </summary>
        /// <param name="path">
        ///   The path describing the sequence of properties to be read during
        ///   <see cref="ExtractFrom(object?)">extraction</see>.
        /// </param>
        public ReadPropertyExtractor(PropertyChain path) {
            Debug.Assert(path is not null);

            path_ = path;
            SourceType = path_.ReflectedType;
            ResultType = path_.PropertyType;
        }

        /// <inheritdoc/>
        public object? ExtractFrom(object? source) {
            Debug.Assert(source is null || source.GetType().IsInstanceOf(SourceType));

            if (source is null) {
                return null;
            }
            return path_.GetValue(source);
        }



        private readonly PropertyChain path_;
    }
}
