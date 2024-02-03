using Cybele.Extensions;
using Kvasir.Schema;
using System;
using System.Collections.Generic;
using System.Diagnostics;

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
        /// <param name="resultType">
        ///   The <see cref="ResultType"/> of the new <see cref="DefaultStructCreator"/>.
        /// </param>
        public DefaultStructCreator(Type type) {
            Debug.Assert(type is not null);
            Debug.Assert(!type.IsPrimitive && !type.IsEnum && type.IsValueType);
            type_ = type;
        }

        /// <inheritdoc/>
        public object? CreateFrom(IReadOnlyList<DBValue> dbValues) {
            Debug.Assert(dbValues is not null && !dbValues.IsEmpty());
            return Activator.CreateInstance(type_);
        }


        private readonly Type type_;
    }
}
