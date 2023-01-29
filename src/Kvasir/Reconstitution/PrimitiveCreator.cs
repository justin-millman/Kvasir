using Ardalis.GuardClauses;
using Kvasir.Schema;
using System;

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
        /// <param name="targetType">
        ///   The type of primitive type that can be handled.
        /// </param>
        /// <pre>
        ///   <paramref name="targetType"/> is a data type supported by the Framework.
        /// </pre>
        internal PrimitiveCreator(Index index, Type targetType) {
            Target = targetType;
            index_ = index;
        }

        /// <inheritdoc/>
        public object? Execute(DBData values) {
            Guard.Against.NullOrEmpty(values, nameof(values));

            if (values[index_] == DBValue.NULL) {
                return null;
            }
            return values[index_].Datum;
        }


        private readonly Index index_;
    }
}
