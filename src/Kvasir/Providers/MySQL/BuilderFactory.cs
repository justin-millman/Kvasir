using Kvasir.Transcription;

namespace Kvasir.Providers.MySQL {
    /// <summary>
    ///   An implementation of the <see cref="IBuilderFactory{TTableDecl, TFieldDecl, TKeyDecl, TConstraintDecl, TFKDecl}"/>
    ///   interface for a MySQL provider.
    /// </summary>
    internal sealed class BuilderFactory : IBuilderFactory<SqlSnippet, FieldDecl, SqlSnippet, IConstraintDecl, SqlSnippet> {
        /// <inheritdoc/>
        public IConstraintDeclBuilder<IConstraintDecl> NewConstraintDeclBuilder() {
            return new ConstraintBuilder();
        }

        /// <inheritdoc/>
        public IForeignKeyDeclBuilder<SqlSnippet> NewForeignKeyDeclBuilder() {
            return new ForeignKeyBuilder();
        }

        /// <inheritdoc/>
        public IKeyDeclBuilder<SqlSnippet> NewKeyDeclBuilder() {
            return new KeyBuilder();
        }

        /// <inheritdoc/>
        public IFieldDeclBuilder<FieldDecl> NewFieldDeclBuilder() {
            return new FieldBuilder();
        }

        /// <inheritdoc/>
        public ITableDeclBuilder<SqlSnippet, FieldDecl, SqlSnippet, IConstraintDecl, SqlSnippet>
        NewTableDeclBuilder() {
            return new TableBuilder();
        }
    }
}
