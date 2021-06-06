using Ardalis.GuardClauses;
using Kvasir.Core;
using Kvasir.Schema;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Kvasir.Reconstitution {
    /// <summary>
    ///   An <see cref="IObjectCreator"/> that produces a single, primitive value.
    /// </summary>
    public sealed class PrimitiveCreator : IObjectCreator {
        /// <inheritdoc/>
        public Type Target { get; }

        /// <summary>
        ///   Constructs a new <see cref="PrimitiveCreator"/>.
        /// </summary>
        /// <param name="index">
        ///   The index in the to-be-provided array of database values at which the target primitive value resides.
        /// </param>
        /// <param name="reverter">
        ///   A <see cref="DataConverter"/> defining how to undo the transform performed on the primitive value. If no
        ///   transformation was applied on storage, an <see cref="DataConverter.Identity{T}">identity conversion</see>
        ///   should be provided.
        /// </param>
        /// <pre>
        ///   <paramref name="reverter"/> supports bidirectional conversion
        ///     --and--
        ///   The <see cref="DataConverter.ResultType"/> of <paramref name="reverter"/> is a data type supported by the
        ///   Framework.
        /// </pre>
        internal PrimitiveCreator(Index index, DataConverter reverter) {
            Guard.Against.Null(reverter, nameof(reverter));
            Debug.Assert(reverter.IsBidirectional);
            Debug.Assert(DBType.IsSupported(reverter.SourceType));

            Target = reverter.SourceType;
            index_ = index;
            reverter_ = reverter;
        }

        /// <inheritdoc/>
        public object? Execute(IReadOnlyList<DBValue> values) {
            Guard.Against.NullOrEmpty(values, nameof(values));

            // DataConverters operate on raw values, of which DBNull is not one. If we pass DBNull to the revert
            // method (or, for that matter, the convert method) we'll get an error because DBNull is not an instance of
            // the expected type.
            if (values[index_] == DBValue.NULL) {
                return reverter_.Revert(null);
            }
            return reverter_.Revert(values[index_].Datum);
        }


        private readonly Index index_;
        private readonly DataConverter reverter_;
    }
}
