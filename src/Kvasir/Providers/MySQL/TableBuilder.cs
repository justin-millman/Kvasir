using Kvasir.Schema;
using Kvasir.Transcription;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Kvasir.Providers.MySQL {
    /// <summary>
    ///   An implementation of the <see cref="ITableDeclBuilder{TTableDecl, TFieldDecl, TKeyDecl, TConstraintDecl, TFKDecl}"/>
    ///   interface for a MySQL provider.
    /// </summary>
    internal sealed class TableBuilder : ITableDeclBuilder<SqlSnippet, FieldDecl, SqlSnippet, IConstraintDecl, SqlSnippet> {
        /// <summary>
        ///   Constructs a new <see cref="TableBuilder"/>.
        /// </summary>
        public TableBuilder() {
            declaration_ = TEMPLATE;
            fields_ = new List<FieldDecl>();
            candidateKeys_ = new List<SqlSnippet>();
            constraints_ = new List<IConstraintDecl>();
            foreignKeys_ = new List<SqlSnippet>();
            fieldIndexFromName_ = new Dictionary<FieldName, int>();
        }

        /// <inheritdoc/>
        public void SetName(TableName name) {
            Debug.Assert(name is not null);
            Debug.Assert(declaration_.Contains(NAME_PLACEHOLDER));

            declaration_ = declaration_.Replace(NAME_PLACEHOLDER, name.Render());
        }

        /// <inheritdoc/>
        public void AddFieldDeclaration(FieldDecl decl) {
            Debug.Assert(!fieldIndexFromName_.ContainsKey(decl.Name));

            fields_.Add(decl);
            fieldIndexFromName_[decl.Name] = fields_.Count - 1;
        }

        /// <inheritdoc/>
        public void SetPrimaryKeyDeclaration(SqlSnippet decl) {
            Debug.Assert(decl is not null && decl.Length != 0);
            Debug.Assert(declaration_.Contains(PK_PLACEHOLDER));

            declaration_ = declaration_.Replace(PK_PLACEHOLDER, decl.ToString());
        }

        /// <inheritdoc/>
        public void AddCandidateKeyDeclaration(SqlSnippet decl) {
            Debug.Assert(decl is not null && decl.Length != 0);
            candidateKeys_.Add(decl);
        }

        /// <inheritdoc/>
        public void AddForeignKeyDeclaration(SqlSnippet decl) {
            Debug.Assert(decl is not null && decl.Length != 0);
            foreignKeys_.Add(decl);
        }

        /// <inheritdoc/>
        public void AddCheckConstraintDeclaration(IConstraintDecl decl) {
            Debug.Assert(decl is not null);
            Debug.Assert(decl is BasicConstraintDecl || decl is MaxLengthConstraintDecl);

            constraints_.Add(decl);
        }

        /// <inheritdoc/>
        public SqlSnippet Build() {
            Debug.Assert(!declaration_.Contains(NAME_PLACEHOLDER));
            Debug.Assert(!declaration_.Contains(PK_PLACEHOLDER));
            Debug.Assert(declaration_.Contains(FIELDS_PLACEHOLDER));
            Debug.Assert(declaration_.Contains(CKS_PLACEHOLDER));
            Debug.Assert(declaration_.Contains(FKS_PLACEHOLDER));
            Debug.Assert(fields_.Count >= 2);

            var maxLengthConstraints = constraints_.OfType<MaxLengthConstraintDecl>();
            foreach (var constraint in maxLengthConstraints) {
                var idx = fieldIndexFromName_[constraint.Field];
                var decl = fields_[idx];
                decl.EnforceMaximumLength(constraint.MaxLength);

                // `FieldDecl` is a struct, so the index access produces a copy; the enforcement function manipulates
                // the struct in-place, so we have to write it back into the list
                fields_[idx] = decl;
            }

            var regularConstraints = constraints_.OfType<BasicConstraintDecl>();
            var fields = string.Join("\n", fields_.Select(f => f.Build()));
            var cks = string.Join("\n", candidateKeys_.Select(ck => ck.ToString()));
            var constraints = string.Join("\n", regularConstraints.Select(chk => chk.DDL.ToString()));
            var fks = string.Join("\n", foreignKeys_.Select(fk => fk.ToString()));

            declaration_ = declaration_.Replace(FIELDS_PLACEHOLDER, fields);
            declaration_ = declaration_.Replace(CKS_PLACEHOLDER, cks == "" ? "" : $"{cks}\n");
            declaration_ = declaration_.Replace(CONSTRAINTS_PLACEHOLDER, constraints == "" ? "" : $"{constraints}\n");
            declaration_ = declaration_.Replace(FKS_PLACEHOLDER, fks == "" ? "" : $"{fks}\n");

            // SqlSnippet automatically strips whitespace, so we don't have to worry about the trailing newline
            // character that is guaranteed to be present
            return new SqlSnippet(declaration_);
        }


        private static readonly string NAME_PLACEHOLDER = "{:0:}";
        private static readonly string FIELDS_PLACEHOLDER = "{:1:}";
        private static readonly string PK_PLACEHOLDER = "{:2:}";
        private static readonly string CKS_PLACEHOLDER = "{:3:}";
        private static readonly string CONSTRAINTS_PLACEHOLDER = "{:4:}";
        private static readonly string FKS_PLACEHOLDER = "{:5:}";
        private static readonly string TEMPLATE =
            $"CREATE TABLE IF NOT EXISTS {NAME_PLACEHOLDER}\n" +
            FIELDS_PLACEHOLDER + "\n" +
            PK_PLACEHOLDER + "\n" +
            CKS_PLACEHOLDER +
            CONSTRAINTS_PLACEHOLDER +
            FKS_PLACEHOLDER;

        private string declaration_;
        private List<FieldDecl> fields_;
        private List<SqlSnippet> candidateKeys_;
        private List<IConstraintDecl> constraints_;
        private List<SqlSnippet> foreignKeys_;
        private Dictionary<FieldName, int> fieldIndexFromName_;
    }
}
