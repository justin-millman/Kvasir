using Cybele.Extensions;
using Kvasir.Schema;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Kvasir.Reconstitution {
    /// <summary>
    ///   An <see cref="ICreator"/> that may apply <see cref="IMutator">modifications</see> after creating the bare CLR
    ///   object.
    /// </summary>
    internal sealed class ReconstitutingCreator : ICreator {
        /// <inheritdoc/>
        public Type ResultType { get; }

        /// <summary>
        ///   Create a new <see cref="ReconstitutingCreator"/>.
        /// </summary>
        /// <param name="creator">
        ///   The <see cref="ICreator"/> with which to create the bare CLR object.
        /// </param>
        /// <param name="mutators">
        ///   Zero or more <see cref="IMutator"/> to be applied to the CLR object produced by <paramref name="creator"/>
        ///   in the given order.
        /// </param>
        public ReconstitutingCreator(ICreator creator, IEnumerable<IMutator> mutators) {
            Debug.Assert(creator is not null);
            Debug.Assert(mutators is not null);
            Debug.Assert(mutators.All(m => creator.ResultType.IsInstanceOf(m.SourceType)));

            creator_ = creator;
            mutators_ = new List<IMutator>(mutators);
            ResultType = creator_.ResultType;
        }

        /// <inheritdoc/>
        public object? CreateFrom(IReadOnlyList<DBValue> dbValues) {
            Debug.Assert(dbValues is not null && !dbValues.IsEmpty());

            var result = creator_.CreateFrom(dbValues);
            foreach (var mutator in mutators_) {
                mutator.Mutate(result, dbValues);
            }
            return result;
        }


        private readonly ICreator creator_;
        private readonly IReadOnlyList<IMutator> mutators_;
    }
}
