using Kvasir.Schema;
using System;
using System.Collections.Generic;

namespace Kvasir.Reconstitution {
    /// <summary>
    ///   The interface describing a component that creates a CLR object from one or more
    ///   <see cref="DBValue">database values</see>.
    /// </summary>
    internal interface ICreator {
        /// <summary>
        ///   The <see cref="Type"/> of CLR object created by this <see cref="ICreator"/>.
        /// </summary>
        Type ResultType { get; }

        /// <summary>
        ///   Create a CLR object from one or more <see cref="DBValue">database values</see>.
        /// </summary>
        /// <param name="dbValues">
        ///   A non-empty, ordered collection of <see cref="DBValue">database values</see>, any of which may be
        ///   <see cref="DBValue.NULL"/>, from which to create a new CLR object.
        /// </param>
        /// <returns>
        ///   A CLR object created from <paramref name="dbValues"/> that is either <see langword="null"/> or is an
        ///   instance of <see cref="ResultType"/>.
        /// </returns>
        object? CreateFrom(IReadOnlyList<DBValue> dbValues);
    }
}
