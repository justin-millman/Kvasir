using Ardalis.GuardClauses;
using Kvasir.Extraction;
using Kvasir.Schema;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Kvasir.Reconstitution {
    /// <summary>
    ///   An <see cref="IObjectCreator"/> that looks up an existing CLR object by a primary or secondary key.
    /// </summary>
    public sealed class ByKeyLookupCreator : IObjectCreator {
        /// <inheritdoc/>
        public Type Target { get; }

        /// <summary>
        ///   Constructs a new <see cref="ByKeyLookupCreator"/>.
        /// </summary>
        /// <param name="factory">
        ///   A function that produces the collection of possible Entity matches, which can produce different results
        ///   each time it is invoked (if so desired).
        /// </param>
        /// <param name="keyExtractor">
        ///   The <see cref="DataExtractionPlan"/> that produces the key for possible Entity matches against which to
        ///   compare the raw values upon <see cref="Execute(IReadOnlyList{DBValue})">execution</see>.
        /// </param>
        /// <pre>
        ///   The <see cref="DataExtractionPlan.ExpectedSource"/> of <paramref name="keyExtractor"/> is the dynamic type
        ///   of each item produced by every invocation of <paramref name="factory"/>, or is a base class or interface
        ///   thereof.
        /// </pre>
        internal ByKeyLookupCreator(Func<IEnumerable<object>> factory, DataExtractionPlan keyExtractor) {
            Debug.Assert(factory is not null);
            Debug.Assert(keyExtractor is not null);

            Target = keyExtractor.ExpectedSource;
            factory_ = factory;
            keyExtractor_ = keyExtractor;
        }

        /// <inheritdoc/>
        public object? Execute(DBData rawValues) {
            Guard.Against.NullOrEmpty(rawValues, nameof(rawValues));

            if (rawValues.Any(v => v == DBValue.NULL)) {
                Debug.Assert(rawValues.All(v => v == DBValue.NULL));
                return null;
            }
            return Lookup.ByKey(rawValues, factory_(), keyExtractor_);
        }


        private readonly Func<IEnumerable<object>> factory_;
        private readonly DataExtractionPlan keyExtractor_;
    }
}
