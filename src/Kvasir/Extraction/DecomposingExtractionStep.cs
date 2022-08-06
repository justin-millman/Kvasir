using Ardalis.GuardClauses;
using Cybele.Extensions;
using Kvasir.Schema;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Kvasir.Extraction {
    /// <summary>
    ///   An <see cref="IExtractionStep"/> that produces a collection of primitive values obtained by recursively
    ///   extracting Field values from an aggregate.
    /// </summary>
    public sealed class DecomposingExtractionStep : IExtractionStep {
        /// <inheritdoc/>
        public Type ExpectedSource { get; }

        /// <summary>
        ///   Constructs a new <see cref="DecomposingExtractionStep"/>.
        /// </summary>
        /// <param name="extractor">
        ///   The <see cref="IFieldExtractor"/> defining how the primitive value is to be obtained.
        /// </param>
        /// <param name="decomposition">
        ///   The steps defining how to decompose the value produced by <paramref name="extractor"/>.
        /// </param>
        /// <pre>
        ///  <paramref name="decomposition"/> is not empty
        ///    --and--
        ///   The <see cref="IExtractionStep.ExpectedSource">ExpectedSource</see> of each element of
        ///   <paramref name="decomposition"/> is the same as the <see cref="IFieldExtractor.FieldType">FieldType</see>
        ///   of <paramref name="extractor"/>, or is a base class or interface thereof
        ///     --and--
        ///   The <see cref="IFieldExtractor.FieldType">FieldType</see> of <paramref name="extractor"/> is a data type
        ///   not supported by the Framework.
        /// </pre>
        internal DecomposingExtractionStep(IFieldExtractor extractor, IEnumerable<IExtractionStep> decomposition) {
            Guard.Against.Null(extractor, nameof(extractor));
            Guard.Against.Null(decomposition, nameof(decomposition));
            Debug.Assert(!decomposition.IsEmpty());
            Debug.Assert(decomposition.All(d => extractor.FieldType.IsInstanceOf(d.ExpectedSource)));
            Debug.Assert(!DBType.IsSupported(extractor.FieldType));         // otherwise decomposition is unnecessary

            extractor_ = extractor;
            decomposition_ = decomposition;
            ExpectedSource = extractor_.ExpectedSource;
        }

        /// <inheritdoc/>
        public DBData Execute(object? source) {
            Debug.Assert(source is null || source.GetType().IsInstanceOf(ExpectedSource));

            // If the source object is null, we cannot simply return a collection with 1 DBValue.NULL instance, as the
            // postcondition of the DecomposingExtractionStep is that each invocation yield a collection with the same
            // number of items. We have no way to know a priori what this size should be, so instead we have to
            // actually perform each of the decompositions (which themselves may be complex pipelines) to produce the
            // individual DBValue.NULL elements.

            var extraction = extractor_.Execute(source);
            var results = new List<DBValue>();

            foreach (var step in decomposition_) {
                results.AddRange(step.Execute(extraction));
            }
            return results;
        }


        private readonly IFieldExtractor extractor_;
        private readonly IEnumerable<IExtractionStep> decomposition_;
    }
}
