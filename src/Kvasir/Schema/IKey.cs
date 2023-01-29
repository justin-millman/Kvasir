using Kvasir.Transcription;
using Optional;
using System.Collections.Generic;

namespace Kvasir.Schema {
    /// <summary>
    ///   The interface for a single internal Key on a Table of a relational database.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     In a relational database, a Key is a non-empty, possibly proper subset of a Table's Fields that are
    ///     required to form a unique tuple for each row in that Table. Keys often reflect real-world domain identity
    ///     relations, such as social security numbers, client usernames, or any of various IDs. Key uniqueness is
    ///     imposed by the database provider, rejecting <c>INSERT</c> and <c>UPDATE</c> statements that would create
    ///     multiple rows with the same tuple of values.
    ///   </para>
    ///   <para>
    ///     This interface is closed: it can only be implemented by types in the Kvasir assembly. It is guaranteed that
    ///     at least on type in the Kvasir assembly implements this interface, though the upper bound is unspecified.
    ///     External assemblies can reference, use, and extend the interface to add cursory functionality and enrich
    ///     their own APIs if desired.
    ///   </para>
    /// </remarks>
    public interface IKey {
        /// <summary>
        ///   The name of this Key.
        /// </summary>
        Option<KeyName> Name { get; }

        /// <summary>
        ///   The Fields that make up this Key.
        /// </summary>
        IReadOnlyList<IField> Fields { get; }

        /// <summary>
        ///   Produces an enumerator that iterates over the Fields in this Key. The contents of those Fields cannot be
        ///   modified by the iteration.
        /// </summary>
        /// <remarks>
        ///   The <see cref="IKey"/> interface does not compose the <see cref="FieldSeq"/> interface, preventing Keys
        ///   from interoperating with LINQ. The <see cref="GetEnumerator"/> method exists to allow iteration with a
        ///   <c>foreach</c> loop, which the compiler implicitly converts into an enumerator expression.
        /// </remarks>
        /// <returns>
        ///   An enumerator that iterates over the Fields in this Key.
        /// </returns>
        IEnumerator<IField> GetEnumerator();

        /// <summary>
        ///   Produces a declaration that, when used as part of a larget Table-creating declaration, defines this
        ///   Key as applying to the subject Table.
        /// </summary>
        /// <typeparam name="TDecl">
        ///   [deduced] The type of declaration produced by <paramref name="builder"/>.
        /// </typeparam>
        /// <param name="builder">
        ///   The <see cref="IKeyDeclBuilder{TDecl}"/> to use to create the declaration.
        /// </param>
        /// <pre>
        ///   <paramref name="builder"/> is not <see langword="null"/>.
        /// </pre>
        /// <returns>
        ///   A <typeparamref name="TDecl"/> that declares this Key.
        /// </returns>
        internal TDecl GenerateDeclaration<TDecl>(IKeyDeclBuilder<TDecl> builder);
    }

    /// <summary>
    ///   A strongly typed <see cref="string"/> repreenting the name of a Key.
    /// </summary>
    /// <remarks>
    ///   Note: <see cref="KeyName"/> is not suitable for representing the name of a Foreign Key.
    /// </remarks>
    /// <seealso cref="FKName"/>
    public sealed class KeyName : ComponentName<IKey> {
        /// <summary>
        ///   Constructs a new <see cref="KeyName"/>.
        /// </summary>
        /// <param name="name">
        ///   The name. Leading and trailing whitspace will be discarded.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        ///   if <paramref name="name"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        ///   if <paramref name="name"/> is zero-length
        ///     --or--
        ///   if <paramref name="name"/> consists only of whitspace.
        /// </exception>
        public KeyName(string name)
            : base(name) {}
    }
}
