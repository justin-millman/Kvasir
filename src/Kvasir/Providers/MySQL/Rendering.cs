using Kvasir.Exceptions;
using Kvasir.Schema;
using System.Diagnostics;

namespace Kvasir.Providers.MySQL {
    /// <summary>
    ///   A collection of extension methods for rendering various identifiers, literals, and expressions for MySQL
    ///   queries and statements.
    /// </summary>
    internal static class Rendering {
        /// <summary>
        ///   Renders the name of a key (primary or candidate) for a MySQL DDL.
        /// </summary>
        /// <param name="name">
        ///   The key name.
        /// </param>
        /// <returns>
        ///   A MySQL-compliant rendering of <paramref name="name"/>.
        /// </returns>
        /// <exception cref="KvasirException">
        ///   if the length of <paramref name="name"/> exceeds 64 characters.
        /// </exception>
        public static string Render(this KeyName name) {
            Debug.Assert(name is not null);

            if (name.Length > MAX_KEY_IDENTIFIER_LENGTH) {
                throw new KvasirException(
                    "[MYSQL] — " +
                    $"length of key name '{name}' is {name.Length}, " +
                    $"which exceeds the maximum of {MAX_KEY_IDENTIFIER_LENGTH} characters"
                );
            }
            return $"{IDENTIFIER_DELIMITER}{name}{IDENTIFIER_DELIMITER}";
        }

        /// <summary>
        ///   Renders the name of a Field for a MySQL DDL or query.
        /// </summary>
        /// <param name="name">
        ///   The Field name.
        /// </param>
        /// <returns>
        ///   A MySQL-compliant rendering of <paramref name="name"/>.
        /// </returns>
        /// <exception cref="KvasirException">
        ///   if the length of <paramref name="name"/> exceeds 64 characters.
        /// </exception>
        public static string Render(this FieldName name) {
            Debug.Assert(name is not null);

            if (name.Length > MAX_FIELD_IDENTIFIER_LENGTH) {
                throw new KvasirException(
                    "[MYSQL] — " +
                    $"length of field name '{name}' is {name.Length}, " +
                    $"which exceeds the maximum of {MAX_FIELD_IDENTIFIER_LENGTH} characters"
                );
            }
            return $"{IDENTIFIER_DELIMITER}{name}{IDENTIFIER_DELIMITER}";
        }

        /// <summary>
        ///   Renders the name of a foreign key for a MySQL DDL.
        /// </summary>
        /// <param name="name">
        ///   The foreign key name.
        /// </param>
        /// <returns>
        ///   A MySQL-compliant rendering of <paramref name="name"/>.
        /// </returns>
        /// <exception cref="KvasirException">
        ///   if the length of <paramref name="name"/> exceeds 64 characters.
        /// </exception>
        public static string Render(this FKName name) {
            Debug.Assert(name is not null);

            if (name.Length > MAX_KEY_IDENTIFIER_LENGTH) {
                throw new KvasirException(
                    "[MYSQL] — " +
                    $"length of foreign key name '{name}' is {name.Length}, " +
                    $"which exceeds the maximum of {MAX_KEY_IDENTIFIER_LENGTH} characters"
                );
            }
            return $"{IDENTIFIER_DELIMITER}{name}{IDENTIFIER_DELIMITER}";
        }

        /// <summary>
        ///   Renders the name of a table for a MySQL DDL or query.
        /// </summary>
        /// <param name="name">
        ///   The table name.
        /// </param>
        /// <returns>
        ///   A MySQL-compliant rendering of <paramref name="name"/>.
        /// </returns>
        /// <exception cref="KvasirException">
        ///   if the length of <paramref name="name"/> exceeds 64 characters.
        /// </exception>
        public static string Render(this TableName name) {
            Debug.Assert(name is not null);

            if (name.Length > MAX_TABLE_IDENTIFIER_LENGTH) {
                throw new KvasirException(
                    "[MYSQL] — " +
                    $"length of table name '{name}' is {name.Length}, " +
                    $"which exceeds the maximum of {MAX_TABLE_IDENTIFIER_LENGTH} characters"
                );
            }
            return $"{IDENTIFIER_DELIMITER}{name}{IDENTIFIER_DELIMITER}";
        }


        private static readonly char IDENTIFIER_DELIMITER = '`';
        private static readonly int MAX_KEY_IDENTIFIER_LENGTH = 64;
        private static readonly int MAX_FIELD_IDENTIFIER_LENGTH = 64;
        private static readonly int MAX_TABLE_IDENTIFIER_LENGTH = 64;
    }
}
