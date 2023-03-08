using Kvasir.Transcription;
using System.Collections.Generic;

namespace Kvasir.Schema {
    /// <summary>
    ///   The interface for a single Table in a relational database.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     A Table is the primary unit of organization in a relational database. Each Table can be thought of as a
    ///     two-dimensional matrix of data, where the columns are Fields and the rows are data entries. A Table is
    ///     principally defined by its name and the collection of constituent Fields; additionally, every Table must
    ///     have exactly one Primary Key - a tuple of Fields that must be unique for all member rows. Tables can
    ///     also have Candidate Keys, which are tuples that <i>could</i> be a Primary Key but just aren't.
    ///   </para>
    ///   <para>
    ///     The structure of a Table can also impose restrictions on the values of its Fields, on top of the implicit
    ///     and/or explicit restrictions imposed by the data type of the Fields themselves. The first form of
    ///     constraint is the Foreign Key constraint, which requires that the values of a tuple of Fields match the
    ///     values of a Primary Key for some data entry row in another Table; this constraint imposes referential
    ///     integrity. The second form of constraint is the <c>CHECK</c> constraint, which allows the specification of
    ///     arbitrary conditional logic on the values of the Fields in a row of data. A Table can have any number of
    ///     either type of constraint, including none at all.
    ///   </para>
    ///   <para>
    ///     This interface is closed: it can only be implemented by types in the Kvasir assembly. It is guaranteed that
    ///     at least on type in the Kvasir assembly implements this interface, though the upper bound is unspecified.
    ///     External assemblies can reference, use, and extend the interface to add cursory functionality and enrich
    ///     their own APIs if desired.
    ///   </para>
    /// </remarks>
    public interface ITable {
        /// <summary>
        ///   The name of this Table.
        /// </summary>
        TableName Name { get; }

        /// <summary>
        ///   The number of Fields that make up this Table.
        /// </summary>
        int Dimension { get; }

        /// <summary>
        ///   Gets the Field in this Table with a specific name.
        /// </summary>
        /// <param name="name">
        ///   The name of the Field to find.
        /// </param>
        /// <exception cref="System.ArgumentException">
        ///   if this Table does not have a Field with name <paramref name="name"/>
        /// </exception>
        IField this[FieldName name] { get; }

        /// <summary>
        ///   The Fields that make up this Table, in columnar order.
        /// </summary>
        IReadOnlyList<IField> Fields { get; }

        /// <summary>
        ///   The Primary Key of this Table.
        /// </summary>
        PrimaryKey PrimaryKey { get; }

        /// <summary>
        ///   The Candidate Keys of this Table.
        /// </summary>
        /// <remarks>
        ///   Although it would be redundant, it is possible for a Candidate Key to be identical to be non-disjoint
        ///   with the Table's <see cref="PrimaryKey">Primary Key</see>. Likewise, it is possible for individual
        ///   Candidate Keys to be non-disjoint with each other.
        /// </remarks>
        IReadOnlyList<CandidateKey> CandidateKeys { get; }

        /// <summary>
        ///   The Foreign Keys of this Table.
        /// </summary>
        IReadOnlyList<ForeignKey> ForeignKeys { get; }

        /// <summary>
        ///   The <c>CHECK</c> constraints applied to this Table.
        /// </summary>
        IReadOnlyList<CheckConstraint> CheckConstraints { get; }

        /// <summary>
        ///   Produces an enumerator that iterates over the Fields in this Table in columnar order. The contents of
        ///   those Fields cannot be modified by the iteration.
        /// </summary>
        /// <remarks>
        ///   The <see cref="ITable"/> interface does not compose the <see cref="FieldSeq"/> interface, preventing
        ///   Tables from interoperating with LINQ. The <see cref="GetEnumerator"/> method exists to allow iteration
        ///   with a <c>foreach</c> loop, which the compiler implicitly converts into an enumerator expression.
        /// </remarks>
        /// <returns>
        ///   An enumerator that iterates over the Fields in this Table in columnar order.
        /// </returns>
        IEnumerator<IField> GetEnumerator();

        /// <summary>
        ///   Produces a declaration that defines this Table.
        /// </summary>
        /// <param name="builderFactory">
        ///   The <see cref="IBuilderFactory{TTableDecl, TFieldDecl, TKeyDecl, TConstraintDecl, Table}"/> with which to
        ///   create the various Builders needed to compose the declaration that defines this Table.
        /// </param>
        /// <pre>
        ///   <paramref name="builderFactory"/> is not <see langword="null"/>.
        /// </pre>
        /// <returns>
        ///   A <typeparamref name="TTableDecl"/> that declares this Table.
        /// </returns>
        internal TTableDecl
        GenerateDeclaration<TTableDecl, TFieldDecl, TKeyDecl, TConstraintDecl, TFKDecl>
        (IBuilderFactory<TTableDecl, TFieldDecl, TKeyDecl, TConstraintDecl, TFKDecl> builderFactory);
    }

    /// <summary>
    ///   A strongly typed <see cref="string"/> representing the name of a Table.
    /// </summary>
    public sealed class TableName : ComponentName<ITable> {
        /// <summary>
        ///   Constructs a new <see cref="TableName"/>.
        /// </summary>
        /// <param name="name">
        ///   The name. Leading and trailing whitespace will be discarded.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        ///   if <paramref name="name"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        ///   if <paramref name="name"/> is zero-length
        ///     --or--
        ///   if <paramref name="name"/> consists only of whitespace.
        /// </exception>
        public TableName(string name)
            : base(name) {}
    }
}
