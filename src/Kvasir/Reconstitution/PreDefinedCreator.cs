using Kvasir.Schema;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Kvasir.Reconstitution {
    /// <summary>
    ///   An <see cref="ICreator"/> that selects the pre-defined instance that matches the source database row.
    /// </summary>
    internal sealed class PreDefinedCreator : ICreator {
        /// <inheritdoc/>
        public Type ResultType { get; }

        /// <summary>
        ///   Constructs a new <see cref="PreDefinedCreator"/>.
        /// </summary>
        /// <param name="table">
        ///   The Principal Table for the Pre-Defined Entity to be created.
        /// </param>
        /// <param name="matcher">
        ///   The <see cref="KeyMatcher"/> that links a Primary Key for one of the pre-defined instances to the actual
        ///   instance itself.
        /// </param>
        public PreDefinedCreator(ITable table, KeyMatcher matcher) {
            Debug.Assert(table is not null);
            Debug.Assert(matcher is not null);

            ResultType = matcher.ResultType;
            lookup_ = new KeyLookupCreator(matcher);
            primaryKeyIndices_ = table.Fields
                .Select((field, idx) => (Field: field, Index: idx))
                .Where(pair => table.PrimaryKey.Fields.Contains(pair.Field))
                .Select(pair => pair.Index)
                .ToList();

        }

        /// <inheritdoc/>
        public object? CreateFrom(IReadOnlyList<DBValue> dBValues) {
            var primaryKey = new List<DBValue>();
            foreach (int idx in primaryKeyIndices_) {
                primaryKey.Add(dBValues[idx]);
            }
            return lookup_.CreateFrom(primaryKey);
        }


        private readonly KeyLookupCreator lookup_;
        private readonly IReadOnlyList<int> primaryKeyIndices_;
    }
}
