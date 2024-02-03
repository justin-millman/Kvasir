using Kvasir.Schema;
using System;
using System.Collections.Generic;

namespace Kvasir.Reconstitution {
    /// <summary>
    ///   The interface describing a component that modifies a CLR object that has already been created by an
    ///   <see cref="ICreator"/>.
    /// </summary>
    internal interface IMutator {
        /// <summary>
        ///   The <see cref="Type"/> of object acted upon by this <see cref="IMutator"/>.
        /// </summary>
        Type SourceType { get; }

        /// <summary>
        ///   Modify a CLR object using one or more database values.
        /// </summary>
        /// <param name="source">
        ///   The possibly-<see langword="null"/> CLR object to modify.
        /// </param>
        /// <param name="dbValues">
        ///   A non-empty, ordered collection of <see cref="DBValue">database values</see>, any of which may be
        ///   <see cref="DBValue.NULL"/>, with which to modify <paramref name="source"/>
        /// </param>
        /// <remarks>
        ///   If <paramref name="source"/> is <see langword="null"/>, no modification occurs.
        /// </remarks>
        void Mutate(object? source, IReadOnlyList<DBValue> dbValues);
    }
}
