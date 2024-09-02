using Cybele.Extensions;
using Kvasir.Schema;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Kvasir.Reconstitution {
    /// <summary>
    ///   An <see cref="ICreator"/> that creates a default-constructed instance of a <see cref="ValueType"/>.
    /// </summary>
    /// <remarks>
    ///   This implementation is essentially the same as a <see cref="ConstructingCreator"/>. However, the implicit
    ///   default constructor available for every <see cref="ValueType"/> is not exposed via reflection (e.g. via
    ///   <see cref="Type.GetConstructors()"/>.
    /// </remarks>
    internal sealed class DefaultStructCreator : ICreator {
        /// <inheritdoc/>
        public Type ResultType => type_;

        /// <summary>
        ///   Create a new <see cref="DefaultStructCreator"/>.
        /// </summary>
        /// <param name="type">
        ///   The <see cref="ResultType"/> of the new <see cref="DefaultStructCreator"/>.
        /// </param>
        /// <param name="allowAllNulls">
        ///   If <see langword="true"/>, then a new non-<see langword="null"/> CLR object will be constructed even if
        ///   each of the <see cref="DBValue">database values</see> provided is <see cref="DBValue.NULL"/>. If
        ///   <see langword="false"/>, then such a set of values will result in a <see langword="null"/> object.
        /// </param>
        public DefaultStructCreator(Type type, bool allowAllNulls) {
            Debug.Assert(type is not null);
            Debug.Assert(!type.IsPrimitive && !type.IsEnum && type.IsValueType);

            type_ = type;
            constructFromAllNulls_ = allowAllNulls;
        }

        /// <inheritdoc/>
        public object? CreateFrom(IReadOnlyList<DBValue> dbValues) {
            Debug.Assert(dbValues is not null && !dbValues.IsEmpty());

            // If all the values in the row are null, it means one of two things: either we're dealing with a nullable
            // Aggregate (constructFromNulls_ will be true) or we're dealing with a non-nullable Aggregate (in which
            // case constructFromNulls_ will be false). In the former case, the Translation Layer ensures that at least
            // one of the nested Fields is itself non-nullable, and it may be possible that running the argument
            // creators induces an error.

            if (!constructFromAllNulls_ && dbValues.All(v => v == DBValue.NULL)) {
                return null;
            }
            else {
                return Activator.CreateInstance(type_);
            }
        }


        private readonly Type type_;
        private readonly bool constructFromAllNulls_;
    }
}
