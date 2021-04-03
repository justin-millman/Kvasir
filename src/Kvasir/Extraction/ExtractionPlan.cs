using Cybele.Extensions;
using Kvasir.Schema;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Kvasir.Extraction {
    /// <summary>
    ///   An <see cref="IExtractionPlan"/> comprised of sub-plans that, collectively, extract one or more values from
    ///   an input object.
    /// </summary>
    public sealed class ExtractionPlan : IExtractionPlan {
        /// <inheritdoc/>
        public Type ExpectedSource { get; }

        /// <summary>
        ///   Constructs a new <see cref="ExtractionPlan"/>.
        /// </summary>
        /// <param name="steps">
        ///   The constituent steps that make up the new <see cref="ExtractionPlan"/> in the order in which they are
        ///   to be executed.
        /// </param>
        /// <pre>
        ///   <paramref name="steps"/> is not empty
        ///     --and--
        ///   each element of <paramref name="steps"/> has the same <see cref="ExpectedSource"/>.
        /// </pre>
        internal ExtractionPlan(IEnumerable<IExtractionPlan> steps) {
            Debug.Assert(steps is not null);
            Debug.Assert(!steps.IsEmpty());
            Debug.Assert(steps.AllSame(p => p.ExpectedSource));

            ExpectedSource = steps.First().ExpectedSource;
            steps_ = steps;
        }

        /// <inheritdoc/>
        public IEnumerable<DBValue> Execute(object? source) {
            Debug.Assert(source is null || ExpectedSource.IsInstanceOfType(source));

            var result = Enumerable.Empty<DBValue>();
            foreach (var step in steps_) {
                result = result.Concat(step.Execute(source));
            }
            return result;
        }


        private readonly IEnumerable<IExtractionPlan> steps_;
    }
}
