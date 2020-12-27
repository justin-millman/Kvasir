using Kvasir.Schema;
using Optional;
using System.Linq;

namespace Kvasir.Transcription.Internal {
    /// <summary>
    ///   A collection of extension methods that operate on types in the <see cref="Internal"/> namespace.
    /// </summary>
    internal static class TranscriptionExtensions {
        /// <summary>
        ///   Generates an declaratory <see cref="SqlSnippet"/> for an <see cref="IField"/> that has no implicit value
        ///   restrictions.
        /// </summary>
        /// <param name="self">
        ///   The <see cref="IFieldDeclGenerator"/> with which to generate the <see cref="SqlSnippet"/>.
        /// </param>
        /// <param name="name">
        ///   The name of the Field.
        /// </param>
        /// <param name="dataType">
        ///   The <see cref="DBType"/> of the Field.
        /// </param>
        /// <param name="nullability">
        ///   Whether or not the Field is nullable
        /// </param>
        /// <param name="defaultValue">
        ///   The default value for the Field.
        /// </param>
        public static SqlSnippet GenerateSql(this IFieldDeclGenerator self, FieldName name, DBType dataType,
            IsNullable nullability, Option<DBValue> defaultValue) {

            return self.GenerateSql(name, dataType, nullability, defaultValue, Enumerable.Empty<DBValue>());
        }
    }
}
