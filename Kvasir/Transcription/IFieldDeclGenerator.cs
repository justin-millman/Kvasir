using Kvasir.Schema;
using Optional;
using System.Collections.Generic;

namespace Kvasir.Transcription.Internal {
    /// <summary>
    ///   A factory that provides methods for generating declaratory <see cref="SqlSnippet">SqlSnippets</see> for
    ///   Fields.
    /// </summary>
    internal interface IFieldDeclGenerator {
        /// <summary>
        ///   Generates a declaratory <see cref="SqlSnippet"/> for a Field with various traits.
        /// </summary>
        /// <param name="name">
        ///   The name of the Field.
        /// </param>
        /// <param name="dataType">
        ///   The <see cref="DBType"/> of the Field.
        /// </param>
        /// <param name="nullability">
        ///   Whether or not the Field is nullable.
        /// </param>
        /// <param name="defaultValue">
        ///   The default value of the Field.
        /// </param>
        /// <param name="allowedValues">
        ///   The collection of values that the Field is allowed to take.
        /// </param>
        /// <returns>
        ///   A <see cref="SqlSnippet"/> consisting of the declaration for an <see cref="IField"/> with the given
        ///   traits.
        /// </returns>
        /// <seealso cref="IField.GenerateDeclaration(IGeneratorCollection)"/>
        SqlSnippet GenerateSql(FieldName name, DBType dataType, IsNullable nullability, Option<DBValue> defaultValue,
            IEnumerable<DBValue> allowedValues);
    }
}
