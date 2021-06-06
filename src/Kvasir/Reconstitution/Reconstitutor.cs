using Ardalis.GuardClauses;
using Cybele.Extensions;
using Kvasir.Schema;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Kvasir.Reconstitution {
    /// <summary>
    ///   The standard form of <see cref="IReconstitutor"/>, which presumes that the provided data will consist exactly
    ///   of the expected values (i.e. aligned to index <c>0</c>, with nothing extraneous).
    /// </summary>
    public sealed class Reconstitutor : IReconstitutor {
        /// <inheritdoc/>
        public Type Target { get; }

        /// <summary>
        ///   Constructs a new <see cref="Reconstitutor"/>.
        /// </summary>
        /// <param name="creator">
        ///   The <see cref="IObjectCreator"/> with which to create the CLR object shell.
        /// </param>
        /// <param name="mutators">
        ///   A list of zero or more <see cref="IMutationStep">IMutationSteps</see> to be applied to the object created
        ///   by <paramref name="creator"/>.
        /// </param>
        /// <pre>
        ///   The <see cref="IMutationStep.ExpectedSubject">expected subject type</see> of each element of
        ///   <paramref name="mutators"/> is the same as the <see cref="IObjectCreator.Target">target type</see> of
        ///   <paramref name="creator"/>.
        /// </pre>
        internal Reconstitutor(IObjectCreator creator, IEnumerable<IMutationStep> mutators) {
            Guard.Against.Null(creator, nameof(creator));
            Guard.Against.Null(mutators, nameof(mutators));
            Debug.Assert(mutators.All(m => creator.Target.IsInstanceOf(m.ExpectedSubject)));

            Target = creator.Target;
            creator_ = creator;
            mutators_ = mutators;
        }

        /// <inheritdoc/>
        public object? ReconstituteFrom(IReadOnlyList<DBValue> rawValues) {
            Guard.Against.NullOrEmpty(rawValues, nameof(rawValues));

            // Mutators cannot operate on null objects, because there would be no target on which to call the property
            // setter. If the object creator produces a null value, then, that's the end of the reconstitution
            // pipeline.
            var obj = creator_.Execute(rawValues);
            if (obj is null) {
                return obj;
            }

            // If the object creator produces a non-null value, it gets passed to each mutator in order. Technically
            // the order is irrelevant, though. If the list of mutators is empty, then this is effectively a no-op.
            foreach (var mutator in mutators_) {
                mutator.Execute(obj, rawValues);
            }
            return obj;
        }


        private readonly IObjectCreator creator_;
        private readonly IEnumerable<IMutationStep> mutators_;
    }
}
