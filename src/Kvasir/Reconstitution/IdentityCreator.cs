using Cybele.Extensions;
using Kvasir.Schema;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Kvasir.Reconstitution {
    /// <summary>
    ///   An <see cref="ICreator"/> that simply unwraps a <see cref="DBValue">database value</see>.
    /// </summary>
    internal sealed class IdentityCreator : ICreator {
        /// <inheritdoc/>
        public Type ResultType { get; }

        /// <summary>
        ///   Construct a new <see cref="IdentityCreator"/>.
        /// </summary>
        /// <param name="resultType">
        ///   The <see cref="ResultType"/> of the new <see cref="IdentityCreator"/>.
        /// </param>
        public IdentityCreator(Type resultType) {
            Debug.Assert(resultType is not null);
            Debug.Assert(DBType.IsSupported(resultType));
            ResultType = resultType;
        }

        /// <inheritdoc/>
        public object? CreateFrom(IReadOnlyList<DBValue> dbValues) {
            Debug.Assert(dbValues is not null);
            Debug.Assert(dbValues.Count == 1);

            var datum = dbValues[0].Datum;
            if (datum == DBNull.Value) {
                return null;
            }
            else {
                Debug.Assert(datum.GetType().IsInstanceOf(ResultType));
                return datum;
            }
        }
    }
}
