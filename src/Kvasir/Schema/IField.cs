using Kvasir.Transcription;
using Optional;

namespace Kvasir.Schema {
    /// <summary>
    ///   The interface for a single Field in a Table of a relational database.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     Fields in a relational database are analogous to columns in the two-dimensional matrix that is a Table.
    ///     Each Field has, at a minimum, a name, data type, and nullability; collectively, this metadata controls how
    ///     the Field is referenced by other Tables and SQL statements/expressions and what values are allowed for the
    ///     Field. Additionally, a Field can have a default value that is used when the Field is omitted from an
    ///     <c>INSERT</c> statement.
    ///   </para>
    ///   <para>
    ///     The representation of Fields at the Schema Layer of Kvasir is an abstraction over the actual realization of
    ///     the Field in a back-end RDBMS. Specifically, the data type of a Field is specified in a storage-agnostic
    ///     manner that makes no guarantees as to the actual back-end storage mechanisms. For example, the data type of
    ///     a Field does not manage the precision of floating-point values or the master domain of integers. These
    ///     aspects of the data type can differ from provider to provider; the abstract representation of the Field
    ///     therefore allows a single Schema translation to be used for any back-end RDBMS provider.
    ///   </para>
    ///   <para>
    ///     This interface is closed: it can only be implemented by types in the Kvasir assembly. It is guaranteed that
    ///     at least on type in the Kvasir assembly implements this interface, though the upper bound is unspecified.
    ///     External assemblies can reference, use, and extend the interface to add cursory functionality and enrich
    ///     their own APIs if desired.
    ///   </para>
    /// </remarks>
    public interface IField {
        /// <summary>
        ///   The name of this Field.
        /// </summary>
        FieldName Name { get; }

        /// <summary>
        ///   The data type of this Field.
        /// </summary>
        DBType DataType { get; }

        /// <summary>
        ///   The nullability of this Field.
        /// </summary>
        /// <remarks>
        ///   Nullability refers to whether or not the value of a Field can be <c>NULL</c>. This status restricts
        ///   several facets of a Schema, such as the <see cref="DefaultValue">default value</see> that a Field can
        ///   take on and the reactionary behaviors of a Foreign Key that includes the Field. The nullability of a
        ///   Field does not depend on the Field's <see cref="DataType">data type</see>.
        /// </remarks>
        IsNullable Nullability { get; }

        /// <summary>
        ///   The default value of this Field.
        /// </summary>
        /// <remarks>
        ///   A Field is not required to have a default value, and the default value may be <see cref="DBValue.NULL"/>
        ///   if the Field is <see cref="Nullability">nullable</see>. A <c>SOME</c> instance indicates the presence of a
        ///   default value, with that value being wrapped by the <see cref="Option{DBValue}"/>. Conversely, a
        ///   <c>NONE</c> instance indicates the absence of a default value.
        /// </remarks>
        Option<DBValue> DefaultValue { get; }

        /// <summary>
        ///   Produces a declaration that, when used as part of a larger Table-creating declaration, defines this
        ///   Field as a member of the subject Table.
        /// </summary>
        /// <typeparam name="TDecl">
        ///   [deduced] The type of declaration produced by <paramref name="builder"/>.
        /// </typeparam>
        /// <param name="builder">
        ///   The <see cref="IFieldDeclBuilder{TDecl}"/> to use to create the declaration.
        /// </param>
        /// <pre>
        ///   <paramref name="builder"/> is not <see langword="null"/>.
        /// </pre>
        /// <returns>
        ///   A <typeparamref name="TDecl"/> that declares this Table.
        /// </returns>
        internal TDecl GenerateDeclaration<TDecl>(IFieldDeclBuilder<TDecl> builder);
    }

    /// <summary>
    ///   A strongly typed <see cref="string"/> representing the name of a Field.
    /// </summary>
    public sealed class FieldName : ComponentName<IField> {
        /// <summary>
        ///   Constructs a new <see cref="FieldName"/>.
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
        public FieldName(string name)
            : base(name) {}
    }
}
