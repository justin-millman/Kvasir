using Cybele;
using Kvasir.Transcription.Internal;
using Optional;
using System;
using System.Collections.Generic;

namespace Kvasir.Schema {
    /// <summary>
    ///   The interface representing a single Key in a back-end relational database.
    /// </summary>
    /// <remarks>
    ///   This interface can only be implemented by classes within the <see cref="Kvasir"/> assembly.
    /// </remarks>
    public interface IKey {
        /// <value>
        ///   The name of this <see cref="IKey"/>.
        /// </value>
        Option<KeyName> Name { get; }

        /// <summary>
        ///   Produces an enumerator that iterates over the set of <see cref="IField">Fields</see> that comprise this
        ///   <see cref="IKey"/>. The order of iteration is not defined but is guaranteed to be the same for repeated
        ///   iterations.
        /// </summary>
        /// <returns>
        ///   An enumerator that iterates over the set of <see cref="IField">Fields</see> that comprise this
        ///   <see cref="IKey"/>.
        /// </returns>
        IEnumerator<IField> GetEnumerator();

        /// <summary>
        ///   Generates a <see cref="SqlSnippet"/> that declares this <see cref="IKey"/> as part of a <c>CREATE
        ///   TABLE</c> statement.
        /// </summary>
        /// <param name="syntax">
        ///   The <see cref="IBuilderCollection"/> exposing the <see cref="IDeclBuilder">IDeclBuilders</see> that this
        ///   <see cref="IKey"/> should use to generate its declaratory <see cref="SqlSnippet"/>.
        /// </param>
        /// <returns>
        ///   A <see cref="SqlSnippet"/> that declares this <see cref="IKey"/> within a <c>CREATE TABLE</c>
        ///   statement according to the syntax rules of <paramref name="syntax"/>.
        /// </returns>
        internal SqlSnippet GenerateDeclaration(IBuilderCollection syntax);
    }

    /// <summary>
    ///   The name of an <see cref="IKey"/>.
    /// </summary>
    public sealed class KeyName : ConceptString<KeyName> {
        /// <summary>
        ///   Constructs a new <see cref="KeyName"/>.
        /// </summary>
        /// <param name="name">
        ///   The name. Any leading and trailing whitespace is trimmed.
        /// </param>
        /// <exception cref="ArgumentException">
        ///   if <paramref name="name"/> is the empty string or consists only of whitespace characters.
        /// </exception>
        public KeyName(string name)
            : base(name.Trim()) {

            if (string.IsNullOrWhiteSpace(Contents)) {
                var msg = "The name of a Field must have non-zero length when leading/trailing whitespace is ignored";
                throw new ArgumentException(msg);
            }
        }
    }
}
