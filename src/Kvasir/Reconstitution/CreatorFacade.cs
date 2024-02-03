using Cybele.Extensions;
using Kvasir.Schema;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Kvasir.Reconstitution {
    /// <summary>
    ///   An <see cref="ICreator"/> that forwards a contiguous subset of <see cref="DBValue">database values</see> to
    ///   another <see cref="ICreator"/>.
    /// </summary>
    /// <remarks>
    ///   The purpose of the <see cref="CreatorFacade"/> is to allow the reconstitution logic for an Aggregate type to
    ///   be reused, both within a single Entity and across Entities. The logic for reconstituting an Aggregate depends
    ///   only on the values extracted from that Aggregate, which are guaranteed to be contiguous within the data of the
    ///   owning Entity. Likewise, any data reversions that must applied are known at the scope of the Aggregate, since
    ///   data converters cannot be applied to nested properties. The <see cref="CreatorFacade"/> therefore allows
    ///   Kvasir to build an <see cref="ICreator"/> (generally a <see cref="ReconstitutingCreator"/>) for a particular
    ///   Aggregate assuming that the values of interest are 0-index, then wrap that instance multiple times based on
    ///   the actual offset into the row of values.
    /// </remarks>
    internal sealed class CreatorFacade : ICreator {
        /// <inheritdoc/>
        public Type ResultType { get; }

        /// <summary>
        ///   Create a new <see cref="CreatorFacade"/>.
        /// </summary>
        /// <param name="creator">
        ///   The <see cref="ICreator"/> to which the new <see cref="CreatorFacade"/> is to delegate.
        /// </param>
        /// <param name="startIdx">
        ///   The index into the collection of <see cref="DBValue">database values</see> at which the first relevant
        ///   value for <paramref name="creator"/> is located.
        /// </param>
        /// <param name="length">
        ///   The number of <see cref="DBValue">database values</see> to forward to <paramref name="creator"/>.
        /// </param>
        public CreatorFacade(ICreator creator, int startIdx, int length) {
            Debug.Assert(creator is not null);
            Debug.Assert(startIdx >= 0);
            Debug.Assert(length >= 1);

            creator_ = creator;
            start_ = startIdx;
            length_ = length;
            ResultType = creator_.ResultType;
        }

        /// <inheritdoc/>
        public object? CreateFrom(IReadOnlyList<DBValue> dbValues) {
            Debug.Assert(dbValues is not null && !dbValues.IsEmpty());
            Debug.Assert(start_ + length_ <= dbValues.Count);

            var view = dbValues.Skip(start_).Take(length_);
            return creator_.CreateFrom(view.ToList());
        }


        private readonly ICreator creator_;
        private readonly int start_;
        private readonly int length_;
    }
}
