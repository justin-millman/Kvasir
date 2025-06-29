using Cybele.Extensions;
using Kvasir.Schema;
using Kvasir.Transcription;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Kvasir.Providers.MySQL {
    /// <summary>
    ///   An intermediate for a MySQL declaration of a Field that allows for changing the default textual backing type
    ///   from <c>LONGTEXT</c> to <c>VARCHAR(N)</c>.
    /// </summary>
    internal struct FieldDecl {
        /// <summary>The name of the Field being declared.</summary>
        public FieldName Name { get; }

        /// <summary>
        ///   Constructs a new <see cref="FieldDecl"/>.
        /// </summary>
        /// <param name="name">
        ///   The <see cref="Name"/> of the Field being declared.
        /// </param>
        /// <param name="declaration">
        ///   The full declaration of the Field, with no placeholder text.
        /// </param>
        public FieldDecl(FieldName name, string declaration) {
            Debug.Assert(name is not null);
            Debug.Assert(declaration is not null && declaration != "");

            Name = name;
            declaration_ = declaration;
        }

        /// <summary>
        ///   Enforces a maximum length on the Field.
        /// </summary>
        /// <param name="maxLength">
        ///   The maximum length.
        /// </param>
        public void EnforceMaximumLength(ulong maxLength) {
            // We can't just do a find-and-replace of LONGTEXT, even with spaces surrounding it, because that specific
            // sequence may appear in the literal default value. The C# string.Replace library method has no way to
            // indicate "replace only the first instance."
            var startIdx = Name.Length + 11;         // +2 for the backticks, +1 for space, +8 for LONGTEXT
            var varchar = $"VARCHAR({maxLength})";
            declaration_ = $"{Name.Render()} {varchar}{declaration_[startIdx..]}";
        }

        /// <summary>
        ///   Produces the MySQL-compliant declaration.
        /// </summary>
        /// <returns>
        ///   A MySQL-compliant declaration of a Field.
        /// </returns>
        public readonly SqlSnippet Build() {
            return new SqlSnippet(declaration_);
        }


        private string declaration_;
    }


    /// <summary>
    ///   An implementation of the <see cref="IConstraintDeclBuilder{TDecl}"/> interface for a MySQL provider.
    /// </summary>
    internal sealed class FieldBuilder : IFieldDeclBuilder<FieldDecl> {
        /// <summary>
        ///   Constructs a new <see cref="FieldBuilder"/>.
        /// </summary>
        public FieldBuilder() {
            declaration_ = TEMPLATE;
        }

        /// <inheritdoc/>
        public void SetName(FieldName name) {
            Debug.Assert(name is not null);
            Debug.Assert(declaration_.Contains(NAME_PLACEHOLDER));

            declaration_ = declaration_.Replace(NAME_PLACEHOLDER, name.Render());
        }

        /// <inheritdoc/>
        public void SetDataType(DBType dataType) {
            Debug.Assert(declaration_.Contains(TYPE_PLACEHOLDER));

            if (dataType == DBType.Boolean) {
                declaration_ = declaration_.Replace(TYPE_PLACEHOLDER, "BOOLEAN");
            }
            else if (dataType == DBType.Character) {
                declaration_ = declaration_.Replace(TYPE_PLACEHOLDER, "CHAR(1)");
            }
            else if (dataType == DBType.Date) {
                declaration_ = declaration_.Replace(TYPE_PLACEHOLDER, "DATE");
            }
            else if (dataType == DBType.DateTime) {
                declaration_ = declaration_.Replace(TYPE_PLACEHOLDER, "DATETIME");
            }
            else if (dataType == DBType.Decimal) {
                declaration_ = declaration_.Replace(TYPE_PLACEHOLDER, "DECIMAL");
            }
            else if (dataType == DBType.Double) {
                declaration_ = declaration_.Replace(TYPE_PLACEHOLDER, "DOUBLE");
            }
            else if (dataType == DBType.Enumeration) {
                declaration_ = declaration_.Replace(TYPE_PLACEHOLDER, "ENUM");
            }
            else if (dataType == DBType.Guid) {
                declaration_ = declaration_.Replace(TYPE_PLACEHOLDER, "BINARY(16)");
            }
            else if (dataType == DBType.Int16) {
                declaration_ = declaration_.Replace(TYPE_PLACEHOLDER, "SMALLINT");
            }
            else if (dataType == DBType.Int32) {
                declaration_ = declaration_.Replace(TYPE_PLACEHOLDER, "INT");
            }
            else if (dataType == DBType.Int64) {
                declaration_ = declaration_.Replace(TYPE_PLACEHOLDER, "BIGINT");
            }
            else if (dataType == DBType.Int8) {
                declaration_ = declaration_.Replace(TYPE_PLACEHOLDER, "TINYINT");
            }
            else if (dataType == DBType.Single) {
                declaration_ = declaration_.Replace(TYPE_PLACEHOLDER, "FLOAT");
            }
            else if (dataType == DBType.Text) {
                declaration_ = declaration_.Replace(TYPE_PLACEHOLDER, "LONGTEXT");
            }
            else if (dataType == DBType.UInt16) {
                declaration_ = declaration_.Replace(TYPE_PLACEHOLDER, "SMALLINT UNSIGNED");
            }
            else if (dataType == DBType.UInt32) {
                declaration_ = declaration_.Replace(TYPE_PLACEHOLDER, "INT UNSIGNED");
            }
            else if (dataType == DBType.UInt64) {
                declaration_ = declaration_.Replace(TYPE_PLACEHOLDER, "BIGINT UNSIGNED");
            }
            else if (dataType == DBType.UInt8) {
                declaration_ = declaration_.Replace(TYPE_PLACEHOLDER, "TINYINT UNSIGNED");
            }
            else {
                throw new UnreachableException($"If branch over {nameof(DBType)} is exhausted");
            }
        }

        /// <inheritdoc/>
        public void SetNullability(IsNullable nullability) {
            Debug.Assert(nullability.IsValid());
            Debug.Assert(declaration_.Contains(NULLITY_PLACEHOLDER));

            if (nullability == IsNullable.No) {
                declaration_ = declaration_.Replace(NULLITY_PLACEHOLDER, " NOT NULL");
            }
            else {
                declaration_ = declaration_.Replace(NULLITY_PLACEHOLDER, "");
            }
        }

        /// <inheritdoc/>
        public void SetDefaultValue(DBValue value) {
            Debug.Assert(declaration_.Contains(DEFAULT_PLACEHOLDER));
            declaration_ = declaration_.Replace(DEFAULT_PLACEHOLDER, $" DEFAULT {value.Render()}");
        }

        /// <inheritdoc/>
        public void SetAllowedValues(IEnumerable<DBValue> values) {
            Debug.Assert(values is not null && !values.IsEmpty());
            Debug.Assert(values.All(v => v.Datum.GetType() == typeof(string)));
            Debug.Assert(declaration_.Contains(ENUM_VALUES_PLACEHOLDER));

            var renderedValues = string.Join(", ", values.Select(v => v.Render()));
            declaration_ = declaration_.Replace(ENUM_VALUES_PLACEHOLDER, $"({renderedValues})");
        }

        /// <inheritdoc/>
        public FieldDecl Build() {
            Debug.Assert(!declaration_.Contains(NAME_PLACEHOLDER));
            Debug.Assert(!declaration_.Contains(TYPE_PLACEHOLDER));

            declaration_ = declaration_.Replace(ENUM_VALUES_PLACEHOLDER, "");   // by default, a field has no enumerators
            declaration_ = declaration_.Replace(NULLITY_PLACEHOLDER, "");       // by default, a field is nullable
            declaration_ = declaration_.Replace(DEFAULT_PLACEHOLDER, "");       // by default, a field has no default value

            // This isn't the prettiest, but for some reason I find this more logical than having an Optional that is
            // guaranteed to be eventually populated. This logic falls apart if there is a backtick in the actual name
            // of the Field, but I'm pretty sure that's not permitted.
            var name = new FieldName(declaration_[1..declaration_.IndexOf("`", 1)]);
            return new FieldDecl(name, declaration_);
        }


        private static readonly string NAME_PLACEHOLDER = "{:0:}";
        private static readonly string TYPE_PLACEHOLDER = "{:1:}";
        private static readonly string ENUM_VALUES_PLACEHOLDER = "{:2:}";
        private static readonly string NULLITY_PLACEHOLDER = "{:3:}";
        private static readonly string DEFAULT_PLACEHOLDER = "{:4:}";
        private static readonly string TEMPLATE = $"{NAME_PLACEHOLDER} {TYPE_PLACEHOLDER}{ENUM_VALUES_PLACEHOLDER}{NULLITY_PLACEHOLDER}{DEFAULT_PLACEHOLDER}";

        private string declaration_;
    }
}
