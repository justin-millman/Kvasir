using System;

namespace Kvasir.Reconstitution {
    /// <summary>
    ///   The interface that defines how a CLR object is created from a list of values extraced from a back-end
    ///   relational database
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     The process of creating a CLR object from a "row" of database values is a two-step process. In the first
    ///     step, the object is <i>created</i>; that is, some valid CLR object is brought into existence according to
    ///     the APIs exposed by the target type. In the second step, that object is modified so that its state reflects
    ///     the full slate of values in the "row." This two-step dance allows for the use of, for exaple, read-only
    ///     properties in conjunction with constructors, which is a more natural object model for most users (as
    ///     compared to requiring that all properties be writeable).
    ///   </para>
    ///   <para>
    ///     The <see cref="IObjectCreator"/> interface describes the shape of the first step in the object-building
    ///     process only. The guarantee of the interface is that a produced object is valid according to the domain
    ///     object being created, though it is not necessarily fully reflective of the database state.
    ///   </para>
    /// </remarks>
    /// <seealso cref="IMutationStep"/>
    /// <seealso cref="Reconstitutor"/>
    public interface IObjectCreator {
        /// <summary>
        ///   The <see cref="Type"/> of CLR object created by this <see cref="IObjectCreator"/>.
        /// </summary>
        Type Target { get; }

        /// <summary>
        ///   Execute this <see cref="IObjectCreator"/> to create a brand new CLR object.
        /// </summary>
        /// <param name="rawValues">
        ///   The raw database values from which to perform the reconstitution.
        /// </param>
        /// <pre>
        ///   <paramref name="rawValues"/> constitutes a collection of values that were produced by an extraction run
        ///   over an instance of <see cref="Target"/>, with relevant values aligned to position <c>0</c>.
        /// </pre>
        /// <returns>
        ///   An object of type <see cref="Target"/> created from <paramref name="rawValues"/> according to the rules
        ///   of this <see cref="IObjectCreator"/>. If the relevant slots of <paramref name="rawValues"/> correspond to
        ///   a <see langword="null"/> object, <see langword="null"/> is returned.
        /// </returns>
        object? Execute(Row rawValues);
    }
}
