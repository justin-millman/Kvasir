using System;

namespace Kvasir.Reconstitution {
    /// <summary>
    ///   The interface that defines how an existing CLR object is modified based on a list of values extracted from a
    ///   back-end relational database
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     The process of creating a CLR object from a "row" of database values is a two-step process. In the first
    ///     step, the object is <i>created</i>; that is, some valid CLR object is brought into existence according to
    ///     the APIs exposed by the target type. In the second step, that object is modified so that its state reflects
    ///     the full slate of values in the "row." This two-step dance allows for the use of, for example, read-only
    ///     properties in conjunction with constructors, which is a more natural object model for most users (as
    ///     compared to requiring that all properties be writeable).
    ///   </para>
    ///   <para>
    ///     The <see cref="IMutationStep"/> interface describes the shape of the second step in the object-building
    ///     process only. The guarantee of the interface is that the provided subject, modified in place, will be in a
    ///     state that fully reflects the provided database state.
    ///   </para>
    /// </remarks>
    /// <seealso cref="IObjectCreator"/>
    /// <seealso cref="IRepopulator"/>
    /// <seealso cref="Reconstitutor"/>
    public interface IMutationStep {
        /// <summary>
        ///   The <see cref="Type"/> of object on which this <see cref="IMutationStep"/> is expected to operate.
        /// </summary>
        Type ExpectedSubject { get; }

        /// <summary>
        ///   Execute this <see cref="IMutationStep"/> to modify an existing CLR object in-place.
        /// </summary>
        /// <param name="subject">
        ///   The non-<see langword="null"/> object to mutate.
        /// </param>
        /// <param name="rawValues">
        ///   The raw database values with which to perform the mutation.
        /// </param>
        /// <pre>
        ///   <paramref name="subject"/> is an instance of <see cref="ExpectedSubject"/>.
        /// </pre>
        void Execute(object subject, DBData rawValues);
    }
}
