using Cybele.Extensions;
using Kvasir.Schema;
using Kvasir.Transcription;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Kvasir.Providers.MySQL {
    /// <summary>
    ///   An implementation of the <see cref="IForeignKeyDeclBuilder{TDecl}"/> interface for a MySQL provider.
    /// </summary>
    internal sealed class ForeignKeyBuilder : IForeignKeyDeclBuilder<SqlSnippet> {
        /// <summary>
        ///   Constructs a new <see cref="ForeignKeyBuilder"/>.
        /// </summary>
        public ForeignKeyBuilder() {
            declaration_ = TEMPALTE;
        }

        /// <inheritdoc/>
        public void SetName(FKName name) {
            Debug.Assert(name is not null);
            Debug.Assert(declaration_.Contains(NAME_PLACEHOLDER));

            declaration_ = declaration_.Replace(NAME_PLACEHOLDER, $"CONSTRAINT {name.Render()} ");
        }

        /// <inheritdoc/>
        public void SetOnDeleteBehavior(OnDelete behavior) {
            Debug.Assert(behavior.IsValid());
            Debug.Assert(declaration_.Contains(ON_DELETE_PLACEHOLDER));

            declaration_ = behavior switch {
                OnDelete.Cascade => declaration_.Replace(ON_DELETE_PLACEHOLDER, " ON DELETE CASCADE"),
                OnDelete.NoAction => declaration_.Replace(ON_DELETE_PLACEHOLDER, " ON DELETE NO ACTION"),
                OnDelete.Prevent => declaration_.Replace(ON_DELETE_PLACEHOLDER, " ON DELETE RESTRICT"),
                OnDelete.SetNull => declaration_.Replace(ON_DELETE_PLACEHOLDER, " ON DELETE SET NULL"),
                OnDelete.SetDefault => declaration_.Replace(ON_DELETE_PLACEHOLDER, " ON DELETE SET DEFAULT"),
                _ => throw new UnreachableException($"Switch statement over {nameof(OnDelete)} exhausted"),
            };
        }

        /// <inheritdoc/>
        public void SetOnUpdateBehavior(OnUpdate behavior) {
            Debug.Assert(behavior.IsValid());
            Debug.Assert(declaration_.Contains(ON_UPDATE_PLACEHOLDER));

            declaration_ = behavior switch {
                OnUpdate.Cascade => declaration_.Replace(ON_UPDATE_PLACEHOLDER, " ON UPDATE CASCADE"),
                OnUpdate.NoAction => declaration_.Replace(ON_UPDATE_PLACEHOLDER, " ON UPDATE NO ACTION"),
                OnUpdate.Prevent => declaration_.Replace(ON_UPDATE_PLACEHOLDER, " ON UPDATE RESTRICT"),
                OnUpdate.SetNull => declaration_.Replace(ON_UPDATE_PLACEHOLDER, " ON UPDATE SET NULL"),
                OnUpdate.SetDefault => declaration_.Replace(ON_UPDATE_PLACEHOLDER, " ON UPDATE SET DEFAULT"),
                _ => throw new UnreachableException($"Switch statement over {nameof(OnUpdate)} exhausted"),
            };
        }

        /// <inheritdoc/>
        public void SetReferencedTable(ITable table) {
            Debug.Assert(table is not null);
            Debug.Assert(declaration_.Contains(TABLE_PLACEHOLDER));
            Debug.Assert(declaration_.Contains(REF_FIELDS_PLACEHOLDER));

            declaration_ = declaration_.Replace(TABLE_PLACEHOLDER, table.Name.Render());
            declaration_ = declaration_.Replace(REF_FIELDS_PLACEHOLDER, string.Join(", ", table.PrimaryKey.Fields.Select(f => f.Name.Render())));
        }

        /// <inheritdoc/>
        public void SetFields(IEnumerable<IField> fields) {
            Debug.Assert(fields is not null && !fields.IsEmpty());
            Debug.Assert(declaration_.Contains(KEY_FIELDS_PLACEHOLDER));

            declaration_ = declaration_.Replace(KEY_FIELDS_PLACEHOLDER, string.Join(", ", fields.Select(f => f.Name.Render())));
        }

        /// <inheritdoc/>
        public SqlSnippet Build() {
            Debug.Assert(!declaration_.Contains(KEY_FIELDS_PLACEHOLDER));
            Debug.Assert(!declaration_.Contains(TABLE_PLACEHOLDER));
            Debug.Assert(!declaration_.Contains(REF_FIELDS_PLACEHOLDER));

            declaration_ = declaration_.Replace(NAME_PLACEHOLDER, "");          // by default, a foreign key has no name
            declaration_ = declaration_.Replace(ON_DELETE_PLACEHOLDER, "");     // by default, a foreign key has no on-delete behavior
            declaration_ = declaration_.Replace(ON_UPDATE_PLACEHOLDER, "");     // by default, a foreign key has no on-update behavior
            return new SqlSnippet(declaration_);
        }


        private static readonly string NAME_PLACEHOLDER = "{:0:}";
        private static readonly string KEY_FIELDS_PLACEHOLDER = "{:1:}";
        private static readonly string TABLE_PLACEHOLDER = "{:2:}";
        private static readonly string REF_FIELDS_PLACEHOLDER = "{:3:}";
        private static readonly string ON_DELETE_PLACEHOLDER = "{:4:}";
        private static readonly string ON_UPDATE_PLACEHOLDER = "{:5:}";
        private static readonly string TEMPALTE =
            $"{NAME_PLACEHOLDER}FOREIGN KEY ({KEY_FIELDS_PLACEHOLDER}) " +
            $"REFERENCES {TABLE_PLACEHOLDER} ({REF_FIELDS_PLACEHOLDER})" +
            $"{ON_DELETE_PLACEHOLDER}{ON_UPDATE_PLACEHOLDER}";

        private string declaration_;
    }
}
