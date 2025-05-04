using Cybele.Extensions;
using Kvasir.Schema;
using Kvasir.Transcription;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Kvasir.Providers.MySQL {
    /// <summary>
    ///   An implementation of the <see cref="IKeyDeclBuilder{TDecl}"/> interface for a MySQL provider.
    /// </summary>
    internal sealed class KeyBuilder : IKeyDeclBuilder<SqlSnippet> {
        /// <summary>
        ///   Constructs a new <see cref="KeyBuilder"/>.
        /// </summary>
        public KeyBuilder() {
            declaration_ = TEMPLATE;
        }

        /// <inheritdoc/>
        public void SetName(KeyName name) {
            Debug.Assert(name is not null);
            Debug.Assert(declaration_.Contains(NAME_PLACEHOLDER));

            declaration_ = declaration_.Replace(NAME_PLACEHOLDER, $"CONSTRAINT {name.Render()} ");
        }

        /// <inheritdoc/>
        public void SetFields(IEnumerable<IField> fields) {
            Debug.Assert(fields is not null && !fields.IsEmpty());
            Debug.Assert(declaration_.Contains(FIELDS_PLACEHOLDER));

            declaration_ = declaration_.Replace(FIELDS_PLACEHOLDER, string.Join(", ", fields.Select(f => f.Name.Render())));
        }

        /// <inheritdoc/>
        public void SetAsPrimaryKey() {
            Debug.Assert(declaration_.Contains(TYPE_PLACEHOLDER));
            declaration_ = declaration_.Replace(TYPE_PLACEHOLDER, "PRIMARY KEY");
        }

        /// <inheritdoc/>
        public SqlSnippet Build() {
            Debug.Assert(!declaration_.Contains(FIELDS_PLACEHOLDER));

            declaration_ = declaration_.Replace(NAME_PLACEHOLDER, "");          // by default, a key has no name
            declaration_ = declaration_.Replace(TYPE_PLACEHOLDER, "UNIQUE");    // by default, a key is a Candidate Key
            return new SqlSnippet(declaration_);
        }


        private static readonly string NAME_PLACEHOLDER = "{:0:}";
        private static readonly string TYPE_PLACEHOLDER = "{:1:}";
        private static readonly string FIELDS_PLACEHOLDER = "{:2:}";
        private static readonly string TEMPLATE = $"{NAME_PLACEHOLDER}{TYPE_PLACEHOLDER} ({FIELDS_PLACEHOLDER})";

        private string declaration_;
    }
}
