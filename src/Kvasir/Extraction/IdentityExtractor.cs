using Cybele.Extensions;
using System;
using System.Diagnostics;

namespace Kvasir.Extraction {
    /// <summary>
    ///   An <see cref="IFieldExtractor"/> that simply returns the value it was provided.
    /// </summary>
    /// <remarks>
    ///   The <see cref="IdentityExtractor{T}"/> class is intended to be used with collection elements, where primitive
    ///   values may be stored directly. In such circumstances, there is no wrapping object and therefore no property
    ///   or function that can be used to access the value. Note, however, that if the element type of a collection is
    ///   itself a complex type (either an Entity or an Aggregate), the <see cref="IdentityExtractor{T}"/> should not
    ///   be used.
    /// </remarks>
    public sealed class IdentityExtractor<T> : IFieldExtractor {
        /// <inheritdoc/>
        public Type ExpectedSource => typeof(T);

        /// <inheritdoc/>
        public Type FieldType => typeof(T);

        /// <summary>
        ///   Constructs a new <see cref="IdentityExtractor{T}"/>.
        /// </summary>
        internal IdentityExtractor() {}

        /// <inheritdoc/>
        public object? Execute(object? source) {
            Debug.Assert(source is null || source.GetType().IsInstanceOf(ExpectedSource));
            return source;
        }
    }
}
