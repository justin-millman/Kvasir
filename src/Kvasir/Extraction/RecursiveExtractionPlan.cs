using Cybele.Extensions;
using Kvasir.Core;
using Kvasir.Schema;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace Kvasir.Extraction {
    /// <summary>
    ///   An <see cref="IExtractionPlan"/> that recursively executes an <see cref="IExtractionPlan"/> on a single
    ///   extracted value.
    /// </summary>
    public sealed class RecursiveExtractionPlan : IExtractionPlan {
        /// <inheritdoc/>
        public Type ExpectedSource { get; }

        /// <summary>
        ///   Constructs a new <see cref="RecursiveExtractionPlan"/>.
        /// </summary>
        /// <param name="property">
        ///   The <see cref="PropertyInfo"/> describing the property whose value is to be initially extracted by the
        ///   new <see cref="RecursiveExtractionPlan"/>. That extracted value is then the input to the recursive plan.
        /// </param>
        /// <param name="converter">
        ///   The converter to apply to the extracted value. If no conversion is necessary, use an
        ///   <see cref="DataConverter.Identity{T}">identity conversion</see>.
        /// </param>
        /// <param name="recursiveSteps">
        ///   The <see cref="IExtractionPlan"/> to be recursively executed on the extracted value.
        /// </param>
        /// <pre>
        ///   The <see cref="IExtractionPlan.ExpectedSource">expected source type</see> of
        ///   <paramref name="recursiveSteps"/> is the same as the property type of <paramref name="property"/> or is
        ///   a base class or interface thereof
        ///     --and--
        ///   The <see cref="DataConverter.SourceType">SourceType</see> of <paramref name="converter"/> is the same as
        ///   the <see cref="PropertyInfo.PropertyType">ReflectedType</see> of <paramref name="property"/>
        /// </pre>
        internal RecursiveExtractionPlan(PropertyInfo property, DataConverter converter,
            IExtractionPlan recursiveSteps) {

            Debug.Assert(property is not null);
            Debug.Assert(converter is not null);
            Debug.Assert(recursiveSteps is not null);
            Debug.Assert(property.ReflectedType is not null);
            Debug.Assert(property.PropertyType.IsInstanceOf(converter.SourceType));
            Debug.Assert(converter.SourceType.IsInstanceOf(recursiveSteps.ExpectedSource));

            ExpectedSource = property.ReflectedType!;
            property_ = property;
            converter_ = converter;
            recursiveSteps_ = recursiveSteps;
        }

        /// <inheritdoc/>
        public IEnumerable<DBValue> Execute(object? source) {
            Debug.Assert(source is null || ExpectedSource.IsInstanceOfType(source));

            if (source is null) {
                return recursiveSteps_.Execute(null);
            }

            var raw = property_.GetValue(source);
            var converted = converter_.Convert(raw);
            return recursiveSteps_.Execute(converted);
        }


        private readonly PropertyInfo property_;
        private readonly DataConverter converter_;
        private readonly IExtractionPlan recursiveSteps_;
    }
}
