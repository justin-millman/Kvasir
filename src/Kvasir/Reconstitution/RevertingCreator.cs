using Cybele.Core;
using Cybele.Extensions;
using Kvasir.Schema;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Kvasir.Reconstitution {
    /// <summary>
    ///   An <see cref="ICreator"/> that executes a <see cref="DataConverter"/> after creating an object.
    /// </summary>
    internal sealed class RevertingCreator : ICreator {
        /// <inheritdoc/>
        public Type ResultType { get; }

        /// <summary>
        ///   Create a new <see cref="RevertingCreator"/>.
        /// </summary>
        /// <param name="creator">
        ///   The <see cref="ICreator"/> that produces the unreverted value.
        /// </param>
        /// <param name="reverter">
        ///   The <see cref="DataConverter"/> to run over the value produced by <paramref name="creator"/>.
        /// </param>
        public RevertingCreator(ICreator creator, DataConverter reverter) {
            Debug.Assert(creator is not null);
            Debug.Assert(reverter is not null && reverter.IsBidirectional);
            Debug.Assert(creator.ResultType.IsInstanceOf(reverter.ResultType));

            creator_ = creator;
            reverter_ = reverter;
            ResultType = reverter.SourceType;
        }

        /// <inheritdoc/>
        public object? CreateFrom(IReadOnlyList<DBValue> dbValues) {
            Debug.Assert(dbValues is not null && !dbValues.IsEmpty());

            var original = creator_.CreateFrom(dbValues);
            return reverter_.Revert(original);
        }


        private readonly ICreator creator_;
        private readonly DataConverter reverter_;
    }
}
