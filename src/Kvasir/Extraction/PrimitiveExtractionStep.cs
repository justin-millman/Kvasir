using Ardalis.GuardClauses;
using Cybele.Core;
using Cybele.Extensions;
using Kvasir.Schema;
using System;
using System.Diagnostics;

namespace Kvasir.Extraction {
    /// <summary>
    ///   An <see cref="IExtractionStep"/> that produces a single, primitive value.
    /// </summary>
    public sealed class PrimitiveExtractionStep : IExtractionStep {
        /// <inheritdoc/>
        public Type ExpectedSource { get; }

        /// <summary>
        ///   Constructs a new <see cref="PrimitiveExtractionStep"/>.
        /// </summary>
        /// <param name="extractor">
        ///   The <see cref="IFieldExtractor"/> defining how the primitive value is to be obtained.
        /// </param>
        /// <pre>
        ///   The <see cref="IFieldExtractor.FieldType">FieldType</see> of <paramref name="extractor"/> is a data type
        ///   supported by the Framework.
        /// </pre>
        internal PrimitiveExtractionStep(IFieldExtractor extractor) {
            Guard.Against.Null(extractor, nameof(extractor));
            extractor_ = extractor;
            ExpectedSource = extractor_.ExpectedSource;
        }

        /// <inheritdoc/>
        public DBData Execute(object? source) {
            Debug.Assert(source is null || source.GetType().IsInstanceOf(ExpectedSource));

            if (source is null) {
                return new DBValue[] { DBValue.NULL };
            }

            // If the source object is not null, then performing the extraction will produce the requisite primitive.
            // However, this primitive itself may be null; this is no problem, as the converter will properly handle
            // this and produce DBValue.NULL. The PrimitiveExtractionStep can only ever return a collection of size 1,
            // so this heuristic is sufficient to satisfy all postconditions.
            var extraction = extractor_.Execute(source);
            return new DBValue[] { DBValue.Create(extraction) };
        }


        private readonly IFieldExtractor extractor_;
    }
}