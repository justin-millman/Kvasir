using Ardalis.GuardClauses;
using Cybele.Core;
using Cybele.Extensions;
using Kvasir.Schema;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Kvasir.Extraction {
    /// <summary>
    ///   A description of the way in which data for a particular Entity type is to be extracted from instances,
    ///   transformed, and prepared to be stored in a back-end database.
    /// </summary>
    public sealed class EntityExtractionPlan {
        /// <summary>
        ///   The <see cref="Type"/> of source object on which this <see cref="EntityExtractionPlan"/> is capable of
        ///   being executed.
        /// </summary>
        public Type ExpectedSource { get; }

        /// <summary>
        ///   Constructs a new <see cref="EntityExtractionPlan"/>.
        /// </summary>
        /// <param name="steps">
        ///   The ordered sequence of <see cref="IExtractionStep">extraction steps</see> that produce the extrinsically
        ///   converted values from a source entity.
        /// </param>
        /// <param name="converters">
        ///   The ordered sequence of <see cref="DataConverter">data converters</see> that intrinsically transform the
        ///   values produced by <paramref name="steps"/>.
        /// </param>
        /// <pre>
        ///   <paramref name="steps"/> is not empty
        ///     --and--
        ///   Each element of <paramref name="steps"/> expects the same type of
        ///   <see cref="IExtractionStep.ExpectedSource">source object</see>
        ///     --and--
        ///   The number of elements in <paramref name="converters"/> matche the number of values, in total, produced
        ///   by <paramref name="steps"/> (note that this is not necessarily the number of elements in
        ///   <paramref name="steps"/>, as an <see cref="IExtractionStep"/> may produce multiple values).
        /// </pre>
        internal EntityExtractionPlan(IEnumerable<IExtractionStep> steps, IEnumerable<DataConverter> converters) {
            Guard.Against.Null(steps, nameof(steps));
            Guard.Against.Null(converters, nameof(converters));
            Debug.Assert(!steps.IsEmpty());
            Debug.Assert(!converters.IsEmpty());
            Debug.Assert(steps.AllSame(e => e.ExpectedSource));

            paramSteps_ = steps;
            converters_ = converters;
            ExpectedSource = steps.First().ExpectedSource;
        }

        /// <summary>
        ///   Execute this <see cref="EntityExtractionPlan"/> on a source object, producing an ordered sequence of
        ///   values that can be stored in a back-end database.
        /// </summary>
        /// <param name="entity">
        ///   The source object on which to execute this <see cref="IExtractionStep"/>.
        /// </param>
        /// <pre>
        ///   <see cref="ExpectedSource"/> is the dynamic type of <paramref name="entity"/> or is a base class or
        ///   interface thereof.
        /// </pre>
        /// <returns>
        ///   An immutable, indexable, ordered sequence of <see cref="DBValue">database values</see> extracted from
        ///   <paramref name="entity"/>.
        /// </returns>
        public IReadOnlyList<DBValue> Execute(object entity) {
            Debug.Assert(entity.GetType().IsInstanceOf(ExpectedSource));

            List<DBValue> results = new List<DBValue>();
            var converterIter = converters_.GetEnumerator();

            foreach (var step in paramSteps_) {
                var extractedParams = step.Execute(entity);
                foreach (var param in extractedParams) {
                    converterIter.MoveNext();

                    // The unwrap-and-rewrap paradigm here is a little annoying, but at least this way we can leverage
                    // the DBValue in the IExtractionStep's return content to ensure that identity conversions are
                    // perfectly valid.
                    results.Add(DBValue.Create(converterIter.Current.Convert(param.Datum)));
                }
            }
            return results;
        }


        private readonly IEnumerable<IExtractionStep> paramSteps_;
        private readonly IEnumerable<DataConverter> converters_;
    }
}
