using Kvasir.Schema;

namespace Kvasir.Transcription.Internal {
    /// <summary>
    ///   An <see cref="IDeclBuilder"/> that builds declaratory <see cref="SqlSnippet">SqlSnippets</see> for internal
    ///   (as opposed to foreign) Keys.
    /// </summary>
    internal interface IKeyDeclBuilder : IDeclBuilder {
        /// <summary>
        ///   Sets the name of the Key whose declaration is represented by the current state of this
        ///   <see cref="IKeyDeclBuilder"/>.
        /// </summary>
        /// <param name="name">
        ///   The name.
        /// </param>
        void SetName(KeyName name);

        /// <summary>
        ///   Adds an <see cref="IField"/> by name to the collection of Fields that make up the Key whose declaration
        ///   is represented by the current state of this <see cref="IKeyDeclBuilder"/>.
        /// </summary>
        /// <param name="fieldName">
        ///   The name of the Field.
        /// </param>
        void AddField(FieldName fieldName);

        /// <summary>
        ///   Sets the Key whose declaration is represented by the current state of this <see cref="IKeyDeclBuilder"/>
        ///   as a <c>PRIMARY KEY</c>.
        /// </summary>
        void SetAsPrimary();
    }
}
