﻿using System;
using System.Collections.Generic;

namespace Kvasir.Relations {
    /// <summary>
    ///   The internal framework interface denoting a custom collection type that serves as a one-to-many relation.
    /// </summary>
    public interface IRelation {
        /// <summary>
        ///   The type of the Relation connection (the "many" in the one-to-many).
        /// </summary>
        internal Type ConnectionType { get; }

        /// <summary>
        ///   Updates the internal state tracking to reflect the Relation being fully synchronized with the back-end
        ///   relational database.
        /// </summary>
        internal void Canonicalize();

        /// <summary>
        ///   Produces an enumerator that iterates over the connections that are tracked by this Relation, including
        ///   those that have been deleted fro it.
        /// </summary>
        /// <remarks>
        ///   The order in which connections are exposed by this method is only softly defined. Any connections that
        ///   have been <see cref="Status.Deleted">deleted</see> since the last
        ///   <see cref="Canonicalize">canonicalization</see> will be exposed first, followed by all
        ///   <see cref="Status.New">new</see> and <see cref="Status.Saved">saved</see> ones (which may be interspersed
        ///   together). Within the two groupings, the relative order of connections is undefined. No connection will
        ///   more than once, and implementations are encouraged to ensure that repeated enuerations of the same
        ///   Relation without intervening changes produce the same sequence of items.
        /// </remarks>
        /// <returns>
        ///   An enumerator that iterates over the connections that are tracked by this Relation.
        /// </returns>
        internal IEnumerator<(object Item, Status Status)> GetEnumerator();
    }
}
