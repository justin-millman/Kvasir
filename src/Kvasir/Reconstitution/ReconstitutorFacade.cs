using Ardalis.GuardClauses;
using Kvasir.Schema;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kvasir.Reconstitution {
    /// <summary>
    ///   An <see cref="IReconstitutor"/> that provides a view over a "row" of data to another
    ///   <see cref="IReconstitutor"/> that is concerned with only a specific subset of values.
    /// </summary>
    /// <remarks>
    ///   Fundamentally, the process for reconstituting an object of type <c>T</c> is independent of the manner in
    ///   which <c>T</c> an instance of <c>T</c> is stored in the back-end database. Specifically, it is independent of
    ///   the Table, the position in the "row," and the names of the back-end Fields. To mirror this independence in
    ///   the Reconstitution Layer requires a mechanism by which individual owning entities can indicate that an
    ///   instance of <c>T</c> is stored at varying offsets. The <see cref="ReconstitutorFacade"/> fulfills this
    ///   responsibility, allowing for a single concrete <see cref="Reconstitutor"/> to be synthesized per type.
    /// </remarks>
    public sealed class ReconstitutorFacade : IReconstitutor {
        /// <inheritdoc/>
        public Type Target { get; }

        /// <summary>
        ///   Constructs a new <see cref="ReconstitutorFacade"/>.
        /// </summary>
        /// <param name="impl">
        ///   The implementation to which the new <see cref="ReconstitutorFacade"/> is to delegate action.
        /// </param>
        /// <param name="startIdx">
        ///   The <c>0</c>-based index in the overall "row" of data values at which the ones relevant to this
        ///   <paramref name="impl"/> begin.
        /// </param>
        /// <param name="length">
        ///   The number of "row" data values that are relevant to <paramref name="impl"/>.
        /// </param>
        /// <pre>
        ///   <paramref name="length"/> <c>&gt; 0</c>
        /// </pre>
        public ReconstitutorFacade(IReconstitutor impl, Index startIdx, int length) {
            Guard.Against.Null(impl, nameof(impl));
            Guard.Against.NegativeOrZero(length, nameof(length));

            Target = impl.Target;
            impl_ = impl;
            start_ = startIdx;
            length_ = length;
        }

        /// <inheritdoc/>
        public object? ReconstituteFrom(IReadOnlyList<DBValue> rawValues) {
            Guard.Against.NullOrEmpty(rawValues, nameof(rawValues));

            var view = rawValues.Skip(start_.Value).Take(length_).ToList();
            return impl_.ReconstituteFrom(view);
        }


        private readonly IReconstitutor impl_;
        private readonly Index start_;
        private readonly int length_;
    }
}
