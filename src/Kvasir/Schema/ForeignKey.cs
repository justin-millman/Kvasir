using Cybele.Extensions;
using Kvasir.Transcription;
using Optional;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Kvasir.Schema {
    /// <summary>
    ///   A referential integrity constraint on a Table in a relational database, requiring the value of some tuple of
    ///   rows to match that of the primary key of some row in another Table.
    /// </summary>
    public sealed record ForeignKey {
        /// <summary>
        ///   The name of this Foreign Key.
        /// </summary>
        public Option<FKName> Name { get; }

        /// <summary>
        ///   The Fields that make up this Foreign Key, in the order that corresponds to the Primary Key of the
        ///   <see cref="ReferencedTable">referenced Table</see>.
        /// </summary>
        public IReadOnlyList<IField> ReferencingFields { get; }

        /// <summary>
        ///   The Table being referenced by this Foreign Key.
        /// </summary>
        public ITable ReferencedTable { get; }

        /// <summary>
        ///   The <c>ON DELETE</c> behavior of this Foreign Key.
        /// </summary>
        public OnDelete OnDelete { get; }

        /// <summary>
        ///   The <c>ON UPDATE</c> behavior of this Foreign Key.
        /// </summary>
        public OnUpdate OnUpdate { get; }

        /// <summary>
        ///   Constructs a new <see cref="ForeignKey"/>.
        /// </summary>
        /// <param name="reference">
        ///   The <see cref="ReferencedTable">Table being referenced</see> by the new Foreign Key.
        /// </param>
        /// <param name="fields">
        ///   The <see cref="ReferencingFields">Fields</see> that make up the new Foreign Key.
        /// </param>
        /// <param name="onDelete">
        ///   The <see cref="OnDelete"><c>ON DELETE</c> behavior</see> of the new Foreign Key.
        /// </param>
        /// <param name="onUpdate">
        ///   The <see cref="OnUpdate"><c>ON UPDATE</c> behavior</see> of the new Foreign Key.
        /// </param>
        internal ForeignKey(ITable reference, IEnumerable<IField> fields, OnDelete onDelete, OnUpdate onUpdate)
            : this(Option.None<FKName>(), reference, fields, onDelete, onUpdate) {}

        /// <summary>
        ///   Constructs a new <see cref="ForeignKey"/>.
        /// </summary>
        /// <param name="name">
        ///   The <see cref="Name">name</see> of the new Foreign Key.
        /// </param>
        /// <param name="reference">
        ///   The <see cref="ReferencedTable">Table being referenced</see> by the new Foreign Key.
        /// </param>
        /// <param name="fields">
        ///   The <see cref="ReferencingFields">Fields</see> that make up the new Foreign Key.
        /// </param>
        /// <param name="onDelete">
        ///   The <see cref="OnDelete"><c>ON DELETE</c> behavior</see> of the new Foreign Key.
        /// </param>
        /// <param name="onUpdate">
        ///   The <see cref="OnUpdate"><c>ON UPDATE</c> behavior</see> of the new Foreign Key.
        /// </param>
        internal ForeignKey(FKName name, ITable reference, IEnumerable<IField> fields, OnDelete onDelete,
            OnUpdate onUpdate)
            : this(Option.Some(name), reference, fields, onDelete, onUpdate) {}

        /* Because ForeignKey is record type, the following methods are synthesized automatically by the compiler:
         *   > public ForeignKey(ForeignKey rhs)
         *   > public bool Equals(ForeignKey? rhs)
         *   > public sealed override bool Equals(object? rhs)
         *   > public sealed override int GetHashCode()
         *   > public sealed override string ToString()
         *   > public static bool operator==(ForeignKey? lhs, ForeignKey? rhs)
         *   > public static bool operator!=(ForeignKey? lhs, ForeignKey? rhs)
         */

        /// <summary>
        ///   Produces an enumerator that iterates over the Fields in this Foreign Key. The contents of those Fields
        ///   cannot be modified by the iteration.
        /// </summary>
        /// <remarks>
        ///   The <see cref="ForeignKey"/> class does not implement the <see cref="IEnumerable{IField}"/> interface,
        ///   preventing Foreign Keys from interoperating with LINQ. The <see cref="GetEnumerator"/> method exists to
        ///   allow iteration with a <c>foreach</c> loop, which the compiler implicitly converts into an enumerator
        ///   expression.
        /// </remarks>
        /// <returns>
        ///   An enumerator that iterates over the Fields in this Foreign Key.
        /// </returns>
        public IEnumerator<IField> GetEnumerator() {
            return ReferencingFields.GetEnumerator();
        }

        /// <summary>
        ///   Produces a declaration that, when used as part of a larget Table-creating declaration, defines this
        ///   Foreign Key as applying to the subject Table.
        /// </summary>
        /// <typeparam name="TDecl">
        ///   [deduced] The type of declaration produced by <paramref name="builder"/>.
        /// </typeparam>
        /// <param name="builder">
        ///   The <see cref="IForeignKeyDeclBuilder{TDecl}"/> to use to create the declaration.
        /// </param>
        /// <pre>
        ///   <paramref name="builder"/> is not <see langword="null"/>.
        /// </pre>
        /// <returns>
        ///   A <typeparamref name="TDecl"/> that declares this Foreign Key.
        /// </returns>
        internal TDecl GenerateDeclaration<TDecl>(IForeignKeyDeclBuilder<TDecl> builder) {
            Debug.Assert(builder is not null);

            Name.MatchSome(n => builder.SetName(n));
            builder.SetFields(ReferencingFields);
            builder.SetReferencedTable(ReferencedTable);
            builder.SetOnDeleteBehavior(OnDelete);
            builder.SetOnUpdateBehavior(OnUpdate);

            return builder.Build();
        }

        /// <summary>
        ///   Constructs a new <see cref="ForeignKey"/>.
        /// </summary>
        /// <param name="name">
        ///   The <see cref="Name">name</see> of the new Foreign Key.
        /// </param>
        /// <param name="reference">
        ///   The <see cref="ReferencedTable">Table being referenced</see> by the new Foreign Key.
        /// </param>
        /// <param name="fields">
        ///   The <see cref="ReferencingFields">Fields</see> that make up the new Foreign Key.
        /// </param>
        /// <param name="onDelete">
        ///   The <see cref="OnDelete"><c>ON DELETE</c> behavior</see> of the new Foreign Key.
        /// </param>
        /// <param name="onUpdate">
        ///   The <see cref="OnUpdate"><c>ON UPDATE</c> behavior</see> of the new Foreign Key.
        /// </param>
        /// <pre>
        ///   The arguments provided to the constructor must, collectively, define a valid Foreign Key. In addition to
        ///   requiring that all arguments be non-<see langword="null"/>, this means that there be at least one
        ///   <paramref name="fields">constituent Field</paramref>, that the number of <paramref name="fields"/> is
        ///   the same as the number of fields in the Primary Key of <paramref name="reference"/>, and that Fields in
        ///   <paramref name="fields"/> have the same data type as the correponding referenced Field.
        /// </pre>
        private ForeignKey(Option<FKName> name, ITable reference, IEnumerable<IField> fields, OnDelete onDelete,
            OnUpdate onUpdate) {

            Debug.Assert(!name.Exists(n => n is null));
            Debug.Assert(reference is not null);
            Debug.Assert(fields is not null);
            Debug.Assert(!fields.IsEmpty());
            Debug.Assert(fields.Count() == reference.PrimaryKey.Fields.Count);
            Debug.Assert(fields.All((i, f) => reference.PrimaryKey.Fields[i].DataType == f.DataType));
            Debug.Assert(onDelete.IsValid());
            Debug.Assert(OnUpdate.IsValid());

            Name = name;
            ReferencingFields = new List<IField>(fields);
            ReferencedTable = reference;
            OnDelete = onDelete;
            OnUpdate = onUpdate;
        }
    }

    /// <summary>
    ///   A strongly typed <see cref="string"/> repreenting the name of a Foreign Key.
    /// </summary>
    public sealed class FKName : ComponentName<ForeignKey> {
        /// <summary>
        ///   Constructs a new <see cref="FKName"/>.
        /// </summary>
        /// <param name="name">
        ///   The name. Leading and trailing whitspace will be discarded.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        ///   if <paramref name="name"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        ///   if <paramref name="name"/> is zero-length
        ///     --or--
        ///   if <paramref name="name"/> consists only of whitspace.
        /// </exception>
        public FKName(string name)
            : base(name) {}
    }
}
