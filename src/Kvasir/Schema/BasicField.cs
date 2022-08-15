using Cybele.Extensions;
using Kvasir.Transcription;
using Optional;
using System.Diagnostics;

namespace Kvasir.Schema {
    /// <summary>
    ///   A Field whose data type has no intrinsic constraints beyond those imposed by the back-end storage.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     The value of a Field can be restricted either intrinsically or extrinsically. An intrinsic restriction is
    ///     one imposed by the data type and nullability of the Field; this restriction is imposed by the actual
    ///     storage mechanism in the back-end provider. Intrinsic restrictions are common to all Fields of a particular
    ///     data type, e.g. the maximum range of allowable integer values for a Field of type <see cref="DBType.Int8"/>
    ///     is the same regardless of what Table the Field belongs to. Extrinsic restrictions, conversely, are those
    ///     that are further imposed on a Field-by-Field basis and can therefore differ inter- and intra-Table. While
    ///     most intrinsic constraints are abstracted behind the <see cref="DBType"/> class, most extrinsic
    ///     restrictions must be manually imposed by a <see cref="CheckConstraint"/> on the Table to which the Field
    ///     belongs. However, some extrinsic restrictions are modeled directly by the Schema Layer of Kvasir.
    ///   </para>
    ///   <para>
    ///     The <see cref="BasicField"/> class models a Field that does not have any such extrinsic restrictions.
    ///   </para>
    /// </remarks>
    public sealed record BasicField : IField {
        /// <inheritdoc/>
        public FieldName Name { get; }

        /// <inheritdoc/>
        public DBType DataType { get; }

        /// <inheritdoc/>
        public IsNullable Nullability { get; }

        /// <inheritdoc/>
        public Option<DBValue> DefaultValue { get; }

        /// <summary>
        ///   Constructs a new <see cref="BasicField"/>.
        /// </summary>
        /// <param name="name">
        ///   The <see cref="Name">name</see> of the new <see cref="BasicField"/>.
        /// </param>
        /// <param name="dataType">
        ///   The <see cref="DataType">data type</see> of the new <see cref="BasicField"/>.
        /// </param>
        /// <param name="nullability">
        ///   The <see cref="Nullability">nullability</see> of the new <see cref="BasicField"/>.
        /// </param>
        /// <param name="defaultValue">
        ///   The <see cref="DefaultValue">default value</see> of the new <see cref="BasicField"/>.
        /// </param>
        /// <pre>
        ///   The arguments provided to the constructor must, collectively, define a valid Field. In addition to
        ///   requiring that all arguments be non-<see langword="null"/>, this means that the
        ///   <paramref name="defaultValue">default value</paramref>, if present, is compatible with the Field's
        ///   <paramref name="dataType">data type</paramref>. Furthermore, that compatible default value can
        ///   only be <see cref="DBValue.NULL"/> if the <paramref name="nullability"/> of the Field is
        ///   <see cref="IsNullable.Yes"/>.
        /// </pre>
        internal BasicField(FieldName name, DBType dataType, IsNullable nullability, Option<DBValue> defaultValue) {
            Debug.Assert(name is not null);
            Debug.Assert(nullability.IsValid());
            Debug.Assert(dataType != DBType.Enumeration);
            Debug.Assert(nullability == IsNullable.Yes || !defaultValue.Contains(DBValue.NULL));
            Debug.Assert(defaultValue.ValueOr(DBValue.NULL).IsInstanceOf(dataType));

            Name = name;
            DataType = dataType;
            Nullability = nullability;
            DefaultValue = defaultValue;
        }

        /* Because BasicField is record type, the following methods are synthesized automatically by the compiler:
         *   > public BasicField(BasicField rhs)
         *   > public bool Equals(BasicField? rhs)
         *   > public sealed override bool Equals(object? rhs)
         *   > public sealed override int GetHashCode()
         *   > public sealed override string ToString()
         *   > public static bool operator==(BasicField? lhs, BasicField? rhs)
         *   > public static bool operator!=(BasicField? lhs, BasicField? rhs)
         */

        /// <inheritdoc/>
        TDecl IField.GenerateDeclaration<TDecl>(IFieldDeclBuilder<TDecl> builder) {
            Debug.Assert(builder is not null);

            builder.SetName(Name);
            builder.SetDataType(DataType);
            builder.SetNullability(Nullability);
            DefaultValue.MatchSome(v => builder.SetDefaultValue(v));

            return builder.Build();
        }
    }
}
