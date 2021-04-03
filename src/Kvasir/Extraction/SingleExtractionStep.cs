using Cybele.Extensions;
using Kvasir.Core;
using Kvasir.Schema;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace Kvasir.Extraction {
    /// <summary>
    ///   An <see cref="IExtractionPlan"/> in which only a single property's value is read via reflection.
    /// </summary>
    public sealed class SingleExtractionStep : IExtractionPlan {
        /// <inheritdoc/>
        public Type ExpectedSource { get; }

        /// <summary>
        ///   Constructs a new <see cref="SingleExtractionStep"/>.
        /// </summary>
        /// <param name="property">
        ///   The <see cref="PropertyInfo"/> describing the property whose value is to be extracted by the new
        ///   <see cref="SingleExtractionStep"/>.
        /// </param>
        /// <param name="converter">
        ///   The converter to apply to the extracted value. If no conversion is necessary, use an
        ///   <see cref="DataConverter.Identity{T}">identity conversion</see>.
        /// </param>
        /// <pre>
        ///   The <see cref="DataConverter.SourceType">SourceType</see> of <paramref name="converter"/> is the same as
        ///   the <see cref="PropertyInfo.PropertyType">ReflectedType</see> of <paramref name="property"/>
        ///     --and--
        ///   The <see cref="DataConverter.ResultType">ResultType</see> of <paramref name="converter"/> is supported by
        ///   the Framework.
        /// </pre>
        internal SingleExtractionStep(PropertyInfo property, DataConverter converter) {
            Debug.Assert(property is not null);
            Debug.Assert(converter is not null);
            Debug.Assert(property.ReflectedType is not null);
            Debug.Assert(property.PropertyType.IsInstanceOf(converter.SourceType));
            Debug.Assert(DBType.IsSupported(converter.ResultType));

            ExpectedSource = property.ReflectedType;
            property_ = property;
            converter_ = converter;
        }

        /// <summary>
        ///   Execute this <see cref="IExtractionPlan"/> on an input object, extracting the single configured value
        ///   from one of its properties.
        /// </summary>
        /// <param name="source">
        ///   The source object.
        /// </param>
        /// <pre>
        ///   <see cref="ExpectedSource"/> is the same as the type of <paramref name="source"/> or is a base class or
        ///   interface thereof.
        /// </pre>
        /// <returns>
        ///   The value extracted from <paramref name="source"/> according to this <see cref="SingleExtractionStep"/>.
        /// </returns>
        public DBValue Execute(object? source) {
            Debug.Assert(source is null || ExpectedSource.IsInstanceOfType(source));

            if (source is null) {
                return DBValue.Create(converter_.Convert(null));
            }

            var raw = property_.GetValue(source);
            return DBValue.Create(converter_.Convert(raw));
        }

        /// <inheritdoc/>
        IEnumerable<DBValue> IExtractionPlan.Execute(object? source) {
            return new List<DBValue>() { Execute(source) };
        }


        private readonly PropertyInfo property_;
        private readonly DataConverter converter_;
    }
}
