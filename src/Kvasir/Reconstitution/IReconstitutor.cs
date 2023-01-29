using System;

namespace Kvasir.Reconstitution {
    /// <summary>
    ///   The interface that defines the mechanism by which a "row" of database values is converted into an equivalent
    ///   CLR object.
    /// </summary>
    public interface IReconstitutor {
        /// <summary>
        ///   The <see cref="Type"/> of object produced by this <see cref="Reconstitutor"/>.
        /// </summary>
        Type Target { get; }

        /// <summary>
        ///   Execute this <see cref="IReconstitutor"/> to create a brand new CLR object from a "row" of datbase
        ///   values.
        /// </summary>
        /// <param name="rawValues">
        ///   The database values from which to reconstitute a CLR object.
        /// </param>
        /// <pre>
        ///   <paramref name="rawValues"/> is not empty.
        /// </pre>
        /// <returns>
        ///   A CLR object of type <see cref="Target"/> that, when run through the dedicated extractor for
        ///   <see cref="Target"/>, produces <paramref name="rawValues"/>.
        /// </returns>
        object? ReconstituteFrom(DBData rawValues);
    }
}
