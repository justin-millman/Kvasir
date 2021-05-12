using Ardalis.GuardClauses;
using Cybele.Extensions;
using Kvasir.Core;
using Kvasir.Schema;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Kvasir.Extraction {
    /// <summary>
    ///   A <see cref="IExtractionStep"/> that produces a single, primitive value.
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
        /// <param name="converter">
        ///   A <see cref="DataConverter"/> defining how to transform the primitive value extracted according to
        ///   <paramref name="extractor"/>. If no transformation is desired, an
        ///   <see cref="DataConverter.Identity{T}">identity conversion</see> should be provided.
        /// </param>
        /// <pre>
        ///   The <see cref="DataConverter.SourceType">SourceType</see> of <paramref name="converter"/> is the same as
        ///   the <see cref="IFieldExtractor.FieldType">FieldType</see> of <paramref name="extractor"/>, or is a base
        ///   class or interface thereof
        ///     --and--
        ///   The <see cref="IFieldExtractor.FieldType">FieldType</see> of <paramref name="extractor"/> is a data type
        ///   supported by the Framework.
        /// </pre>
        public PrimitiveExtractionStep(IFieldExtractor extractor, DataConverter converter) {
            Guard.Against.Null(extractor, nameof(extractor));
            Guard.Against.Null(converter, nameof(converter));
            Debug.Assert(extractor.FieldType.IsInstanceOf(converter.SourceType));
            Debug.Assert(DBType.IsSupported(converter.SourceType));

            extractor_ = extractor;
            converter_ = converter;
            ExpectedSource = extractor_.ExpectedSource;
        }

        /// <inheritdoc/>
        public IReadOnlyList<DBValue> Execute(object? source) {
            Debug.Assert(source is null || source.GetType().IsInstanceOf(ExpectedSource));

            if (source is null) {
                return new DBValue[] { DBValue.NULL };
            }

            // If the source object is not null, then performing the extraction will produce the requisite primitive.
            // However, this primitive itself may be null; this is no problem, as the converter will properly handle
            // this and produce DBValue.NULL. The PrimitiveExtractionStep can only ever return a collection of size 1,
            // so this heuristic is sufficient to satisfy all postconditions.
            var extraction = extractor_.Execute(source);
            var conversion = converter_.Convert(extraction);
            return new DBValue[] { DBValue.Create(conversion) };
        }


        private readonly IFieldExtractor extractor_;
        private readonly DataConverter converter_;
    }
}