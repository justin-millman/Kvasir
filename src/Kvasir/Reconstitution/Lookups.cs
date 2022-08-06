using Cybele.Extensions;
using Kvasir.Extraction;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Kvasir.Reconstitution {
    /// <summary>
    ///   A collection of utilities for performing look-up operations during Reconstitution.
    /// </summary>
    internal static partial class Lookup {
        /// <summary>
        ///   Lookup the Entity from a list of options that matches a given key.
        /// </summary>
        /// <param name="key">
        ///   The probe key, which may be the primary key of the Entities or any other collection of unique Fields.
        /// </param>
        /// <param name="entities">
        ///   The collection of possible Entity matches.
        /// </param>
        /// <param name="extractor">
        ///   The <see cref="DataExtractionPlan"/> that produces the key for possible Entity matches against which to
        ///   compare <paramref name="key"/>.
        /// </param>
        /// <pre>
        ///   The <see cref="DataExtractionPlan.ExpectedSource"/> of <paramref name="extractor"/> is the dynamic type
        ///   of each item in <paramref name="entities"/>, or is a base class or interface thereof.
        /// </pre>
        public static object ByKey(DBData key, IEnumerable<object> entities, DataExtractionPlan extractor) {
            Debug.Assert(entities.All(e => e.GetType().IsInstanceOf(extractor.ExpectedSource)));

            foreach (var entity in entities) {
                var keyIdx = 0;
                foreach (var value in extractor.ExecutePiecewise(entity)) {
                    if (!value.Equals(key[keyIdx++])) {
                        goto nextEntity;
                    }
                }
                return entity;
                nextEntity: continue;
            }

            // This code should be unreachable
            throw new ApplicationException($"No entity found to match key: {string.Join(", ", key)}");
        }
    }
}
