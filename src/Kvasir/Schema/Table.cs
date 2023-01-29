using Cybele.Extensions;
using Kvasir.Transcription;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Kvasir.Schema {
    /// <summary>
    ///   A single Table in a back-end relational database.
    /// </summary>
    /// <remarks>
    ///   At present, there is only one implementation of the <see cref="ITable"/> interface in Kvasir. The
    ///   <see cref="Table"/> class is used to represent entity Tables, list Tables, and relation Tables.
    /// </remarks>
    public sealed record Table : ITable {
        /// <inheritdoc/>
        public TableName Name { get; }

        /// <inheritdoc/>
        public int Dimension => Fields.Count;

        /// <inheritdoc/>
        public IField this[FieldName name] {
            get {
                var found = Fields.FirstOrDefault(f => f.Name == name);
                if (found is null) {
                    var msg = $"Table {Name} has no Field with name {name}";
                    throw new ArgumentException(msg);
                }
                return found;
            }
        }

        /// <inheritdoc/>
        public IReadOnlyList<IField> Fields { get; }

        /// <inheritdoc/>
        public PrimaryKey PrimaryKey { get; }

        /// <inheritdoc/>
        public IReadOnlyList<CandidateKey> CandidateKeys { get; }

        /// <inheritdoc/>
        public IReadOnlyList<ForeignKey> ForeignKeys { get; }

        /// <inheritdoc/>
        public IReadOnlyList<CheckConstraint> CheckConstraints { get; }

        /// <summary>
        ///   Constructs a new <see cref="Table"/>.
        /// </summary>
        /// <param name="name">
        ///   The <see cref="Name">name</see> of the new <see cref="Table"/>.
        /// </param>
        /// <param name="fields">
        ///   The <see cref="Fields">Fields</see> that make up the new <see cref="Table"/>.
        /// </param>
        /// <param name="primaryKey">
        ///   The <see cref="PrimaryKey">Primary Key</see> of the new <see cref="Table"/>.
        /// </param>
        /// <param name="candidateKeys">
        ///   The <see cref="CandidateKeys">Candidate Keys</see> of the new <see cref="Table"/>.
        /// </param>
        /// <param name="foreignKeys">
        ///   The <see cref="ForeignKeys">Foreign Keys</see> of the new <see cref="Table"/>.
        /// </param>
        /// <param name="checkConstraints">
        ///   The <see cref="CheckConstraints"><c>CHECK</c> constraints</see> applied to the new <see cref="Table"/>.
        /// </param>
        /// <pre>
        ///   The arguments provided to the constructor must, collectively, define a valid Table. In addition to
        ///   requiring that all arguments be non-<see langword="null"/>, this means that there are at least two 
        ///   <paramref name="fields">Fields</paramref> that all lateral Field references (i.e. from the 
        ///   <paramref name="primaryKey">Primary Key</paramref>, all
        ///   <paramref name="candidateKeys">Candidate Keys</paramref>, all
        ///   <paramref name="foreignKeys">Foreign Keys</paramref>, and all
        ///   <paramref name="checkConstraints"><c>CHECK</c> constraints</paramref> are to Fields that are present in
        ///   the Table. Additionally, all Fields must have a unique <see cref="IField.Name">name</see>.
        /// </pre>
        internal Table(TableName name, FieldSeq fields, PrimaryKey primaryKey, IEnumerable<CandidateKey> candidateKeys,
            IEnumerable<ForeignKey> foreignKeys, IEnumerable<CheckConstraint> checkConstraints) {

            Debug.Assert(name is not null);
            Debug.Assert(fields is not null);
            Debug.Assert(fields.Count() >= 2);
            Debug.Assert(fields.Select(f => f.Name).Distinct().Count() == fields.Count());
            Debug.Assert(primaryKey is not null);
            Debug.Assert(primaryKey.Fields.All(f => fields.Contains(f)));
            Debug.Assert(candidateKeys is not null);
            Debug.Assert(candidateKeys.All(k => k.Fields.All(f => fields.Contains(f))));
            Debug.Assert(foreignKeys is not null);
            Debug.Assert(foreignKeys.None(k => ReferenceEquals(k.ReferencedTable, this)));
            Debug.Assert(foreignKeys.All(k => k.ReferencingFields.All(f => fields.Contains(f))));
            Debug.Assert(checkConstraints is not null);
            Debug.Assert(checkConstraints.All(c => c.Condition.GetDependentFields().All(f => fields.Contains(f))));

            Name = name;
            Fields = new List<IField>(fields);
            PrimaryKey = primaryKey;
            CandidateKeys = new List<CandidateKey>(candidateKeys);
            ForeignKeys = new List<ForeignKey>(foreignKeys);
            CheckConstraints = new List<CheckConstraint>(checkConstraints);
        }

        /* Because Table is record type, the following methods are synthesized automatically by the compiler:
         *   > public Table(Table rhs)
         *   > public bool Equals(Table? rhs)
         *   > public sealed override bool Equals(object? rhs)
         *   > public sealed override int GetHashCode()
         *   > public sealed override string ToString()
         *   > public static bool operator==(Table? lhs, Table? rhs)
         *   > public static bool operator!=(Table? lhs, Table? rhs)
         */

        /// <inheritdoc/>
        public IEnumerator<IField> GetEnumerator() {
            return Fields.GetEnumerator();
        }

        /// <inheritdoc/>
        TTableDecl
        ITable.GenerateDeclaration<TTableDecl, TFielDecl, TKeyDecl, TConstraintDecl, TFKDecl>
        (IBuilderFactory<TTableDecl, TFielDecl, TKeyDecl, TConstraintDecl, TFKDecl> builderFactory) {
            Debug.Assert(builderFactory is not null);

            // Core Table Content
            var builder = builderFactory.NewTableDeclBuilder();
            builder.SetName(Name);
            
            // Fields
            foreach (var field in Fields) {
                var fieldDecl = field.GenerateDeclaration(builderFactory.NewFieldDeclBuilder());
                builder.AddFieldDeclaration(fieldDecl);
            }

            // Primary Key
            var pkDecl = (PrimaryKey as IKey).GenerateDeclaration(builderFactory.NewKeyDeclBuilder());
            builder.SetPrimaryKeyDeclaration(pkDecl);

            // Candidate Keys
            foreach (IKey ck in CandidateKeys) {
                var ckDecl = ck.GenerateDeclaration(builderFactory.NewKeyDeclBuilder());
                builder.AddCandidateKeyDeclaration(ckDecl);
            }

            // Foreign Keys
            foreach (var fk in ForeignKeys) {
                var fkDecl = fk.GenerateDeclaration(builderFactory.NewForeignKeyDeclBuilder());
                builder.AddForeignKeyDeclaration(fkDecl);
            }

            // Check Constraints
            foreach (var check in CheckConstraints) {
                var checkDecl = check.GenerateDeclaration(builderFactory.NewConstraintDeclBuilder());
                builder.AddCheckConstraintDeclaration(checkDecl);
            }

            // Build Result
            return builder.Build();
        }
    }
}
