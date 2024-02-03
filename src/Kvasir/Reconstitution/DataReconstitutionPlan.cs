using Cybele.Extensions;
using Kvasir.Schema;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Kvasir.Reconstitution {
    /// <summary>
    ///   A plan that describes how to reconstitute a single CLR source object from a set of
    ///   <see cref="DBValue">database values</see>.
    /// </summary>
    internal sealed class DataReconstitutionPlan {
        /// <summary>
        ///   The <see cref="Type"/> of CLR object reconstituted by this <see cref="DataReconstitutionPlan"/>.
        /// </summary>
        public Type ResultType { get; }

        /// <summary>
        ///   Construct a new <see cref="DataReconstitutionPlan"/>.
        /// </summary>
        /// <param name="creator">
        ///   The <see cref="ICreator"/> with which to create the bare CLR object.
        /// </param>
        /// <param name="mutators">
        ///   Zero or more <see cref="IMutator"/> to be applied to the CLR object produced by <paramref name="creator"/>
        ///   in the given order.
        /// </param>
        public DataReconstitutionPlan(ICreator creator, IEnumerable<IMutator> mutators) {
            Debug.Assert(creator is not null);
            Debug.Assert(mutators is not null);
            Debug.Assert(mutators.AllSame(m => m.SourceType));
            Debug.Assert(mutators.IsEmpty() || creator.ResultType.IsInstanceOf(mutators.First().SourceType));

            creator_ = creator;
            mutators_ = new List<IMutator>(mutators);
            ResultType = creator_.ResultType;
        }

        /// <summary>
        ///   Reconstitute a CLR object from one or more <see cref="DBValue">database values</see>.
        /// </summary>
        /// <param name="dbValues">
        ///   A non-empty, ordered collection of <see cref="DBValue">database values</see>, any of which may be
        ///   <see cref="DBValue.NULL"/>, from which to reconstitute a new CLR object.
        /// </param>
        /// <returns>
        ///   A non-<see langword="null"/> CLR object created from <paramref name="dbValues"/> that is an instance of
        ///   <see cref="ResultType"/>.
        /// </returns>
        public object ReconstituteFrom(IReadOnlyList<DBValue> dbValues) {
            Debug.Assert(dbValues is not null && !dbValues.IsEmpty());

            var reconstitution = creator_.CreateFrom(dbValues)!;
            foreach (var mutator in mutators_) {
                mutator.Mutate(reconstitution, dbValues);
            }
            return reconstitution;
        }


        private readonly ICreator creator_;
        private readonly IReadOnlyList<IMutator> mutators_;
    }
}
