using System;
using System.Collections.Generic;

namespace Kvasir.Reconstitution {
    /// <summary>
    ///   The interface that defines how CLR objects that have been reconstituted from a back-end relation database are
    ///   placed into a <see cref="Kvasir.Relations.IRelation"/> on another CLR instance.
    /// </summary>
    public interface IRepopulator {
        /// <summary>
        ///   The <see cref="Type"/> of object on which this <see cref="IRepopulator"/> is expected to operate.
        /// </summary>
        Type ExpectedSubject { get; }

        /// <summary>
        ///   Execute this <see cref="IRepopulator"/> to modify a relation on an existing CLR object in-place.
        /// </summary>
        /// <param name="subject">
        ///   The non-<see langword="null"/> object to mutate.
        /// </param>
        /// <param name="entries">
        ///   The reconstituted elements that are to be placed into the relation.
        /// </param>
        /// <pre>
        ///   <paramref name="subject"/> is an instance of <see cref="ExpectedSubject"/>
        ///     --and--
        ///   <paramref name="entries"/> is non-empty.
        /// </pre>
        void Execute(object subject, IEnumerable<object> entries);
    }
}
