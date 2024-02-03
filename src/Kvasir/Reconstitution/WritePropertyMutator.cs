using Cybele.Extensions;
using Kvasir.Schema;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace Kvasir.Reconstitution {
    /// <summary>
    ///   An <see cref="IMutator"/> that writes a value to a property via reflection.
    /// </summary>
    internal sealed class WritePropertyMutator : IMutator {
        /// <inheritdoc/>
        public Type SourceType { get; }

        /// <summary>
        ///   Construct a new <see cref="WritePropertyMutator"/>.
        /// </summary>
        /// <param name="property">
        ///   The property to which to write a value.
        /// </param>
        /// <param name="valueCreator">
        ///   The <see cref="ICreator"/> with which to create the value to write to <paramref name="property"/>.
        /// </param>
        public WritePropertyMutator(PropertyInfo property, ICreator valueCreator) {
            Debug.Assert(property is not null);
            Debug.Assert(property.ReflectedType is not null);
            Debug.Assert(valueCreator is not null);
            Debug.Assert(property.CanWrite);
            Debug.Assert(valueCreator.ResultType.IsInstanceOf(property.PropertyType));

            property_ = property;
            valueCreator_ = valueCreator;
            SourceType = property.ReflectedType;
        }

        /// <inheritdoc/>
        public void Mutate(object? source, IReadOnlyList<DBValue> dbValues) {
            Debug.Assert(source is null || source.GetType().IsInstanceOf(SourceType));
            Debug.Assert(dbValues is not null && !dbValues.IsEmpty());

            if (source is not null) {
                var value = valueCreator_.CreateFrom(dbValues);
                property_.SetValue(source, value);
            }
        }


        private readonly PropertyInfo property_;
        private readonly ICreator valueCreator_;
    }
}
