using Kvasir.Exceptions;
using Kvasir.Schema;
using System;
using System.Diagnostics;

namespace Kvasir.Providers.MySQL {
    /// <summary>
    ///   A collection of extension methods for rendering various identifiers, literals, and expressions for MySQL
    ///   queries and statements.
    /// </summary>
    internal static class Rendering {
        /// <summary>The `NULL` literal</summary>
        public static string NULL { get; } = "NULL";

        /// <summary>
        ///   Renders a literal value for a MySQL DDL or query.
        /// </summary>
        /// <param name="value">
        ///   The literal.
        /// </param>
        /// <return>
        ///   A MySQL-compliant rendering of <paramref name="value"/>.
        /// </return>
        public static string Render(this DBValue value) {
            if (value == DBValue.NULL) {
                return NULL;
            }
            else if (value.Datum.GetType() == typeof(string)) {
                return $"\"{value.Datum}\"";
            }
            else if (value.Datum.GetType() == typeof(char)) {
                return $"\"{value.Datum}\"";
            }
            else if (value.Datum.GetType() == typeof(DateOnly)) {
                var date = (DateOnly)value.Datum;
                return $"DATE \"{date:yyyy-MM-dd}\"";
            }
            else if (value.Datum.GetType() == typeof(DateTime)) {
                var datetime = (DateTime)value.Datum;
                return $"DATETIME \"{datetime:yyyy-MM-dd HH:mm:ss}\"";
            }
            else if (value.Datum.GetType() == typeof(Guid)) {
                var guid = (Guid)value.Datum;
                return $"UUID_TO_BIN(\"{guid:D}\")";
            }
            else if (value.Datum.GetType() == typeof(bool)) {
                return value.Datum.ToString()!.ToUpper();
            }
            else {
                return value.Datum.ToString()!;
            }
        }

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

        /// <summary>
        ///   Renders the name of a constraint for a MySQL DDL.
        /// </summary>
        /// <param name="name">
        ///   The constraint name.
        /// </param>
        /// <returns>
        ///   A MySQL-compliant rendering of <paramref name="name"/>.
        /// </returns>
        /// <exception cref="KvasirException">
        ///   if the length of <paramref name="name"/> exceeds 64 characters.
        /// </exception>
        public static string Render(this CheckName name) {
            Debug.Assert(name is not null);

            if (name.Length > MAX_CONSTRAINT_IDENTIFIER_LENGTH) {
                throw new KvasirException(
                    "[MYSQL] — " +
                    $"length of constraint name '{name}' is {name.Length}, " +
                    $"which exceeds the maximum of {MAX_TABLE_IDENTIFIER_LENGTH} characters"
                );
            }
            return $"{IDENTIFIER_DELIMITER}{name}{IDENTIFIER_DELIMITER}";
        }

        /// <summary>
        ///   Renders a comparison operator for a MySQL DDL or query.
        /// </summary>
        /// <param name="op">
        ///   The operator.
        /// </param>
        /// <param name="forBoolean">
        ///   <see langword="true"/> if the rendering should be done for a Boolean value; otherwise,
        ///   <see langword="false"/>.
        /// </param>
        /// <returns>
        ///   A MySQL-compliant rendering of <paramref name="op"/>.
        /// </returns>
        public static string Render(this ComparisonOperator op, bool forBoolean) {
            return op switch {
                ComparisonOperator.EQ => forBoolean ? "IS" : "=",
                ComparisonOperator.NE => forBoolean ? "IS NOT" : "!=",
                ComparisonOperator.LT => "<",
                ComparisonOperator.LTE => "<=",
                ComparisonOperator.GT => ">",
                ComparisonOperator.GTE => ">=",
                _ => throw new UnreachableException($"Switch statement over {nameof(ComparisonOperator)} exhausted")
            };
        }

        /// <summary>
        ///   Renders an inclusion operator for a MySQL DDL or query.
        /// </summary>
        /// <param name="op">
        ///   The operator.
        /// </param>
        /// <returns>
        ///   A MySQL-compliant rendering of <paramref name="op"/>.
        /// </returns>
        public static string Render(this InclusionOperator op) {
            return op switch {
                InclusionOperator.In => "IN",
                InclusionOperator.NotIn => "NOT IN",
                _ => throw new UnreachableException($"Switch statement over {nameof(InclusionOperator)} exhausted")
            };
        }

        /// <summary>
        ///   Renders a nullity operator for a MySQL DDL or query.
        /// </summary>
        /// <param name="op">
        ///   The operator.
        /// </param>
        /// <returns>
        ///   A MySQL-compliant rendering of <paramref name="op"/>.
        /// </returns>
        public static string Render(this NullityOperator op) {
            return op switch {
                NullityOperator.IsNull => "IS",
                NullityOperator.IsNotNull => "IS NOT",
                _ => throw new UnreachableException($"Switch statement over {nameof(NullityOperator)} exhausted")
            };
        }

        /// <summary>
        ///   Renders a <see cref="FieldExpression"/> for a MySQL DDL.
        /// </summary>
        /// <param name="expression">
        ///   The expression.
        /// </param>
        /// <returns>
        ///   A MySQL-compliant rendering of <paramref name="expression"/>.
        /// </returns>
        public static string Render(this FieldExpression expression) {
            return expression.Function.Match(
                some: fn => fn switch {
                    FieldFunction.LengthOf => $"LENGTH({expression.Field.Name.Render()})",
                    _ => throw new UnreachableException($"Switch statement over {nameof(FieldFunction)} exhausted")
                },
                none: () => expression.Field.Name.Render()
            );
        }


        private static readonly char IDENTIFIER_DELIMITER = '`';
        private static readonly int MAX_KEY_IDENTIFIER_LENGTH = 64;
        private static readonly int MAX_FIELD_IDENTIFIER_LENGTH = 64;
        private static readonly int MAX_TABLE_IDENTIFIER_LENGTH = 64;
        private static readonly int MAX_CONSTRAINT_IDENTIFIER_LENGTH = 64;
    }
}
