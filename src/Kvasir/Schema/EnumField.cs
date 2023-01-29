using Cybele.Extensions;
using Kvasir.Transcription;
using Optional;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Kvasir.Schema {
    /// <summary>
    ///   A Field whose data type has an intrinsic constraint limiting the allowed values to a discrete set of strings.
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
    ///     The <see cref="EnumField"/> class models a Field to which is applied an extrinsic enueration restriction.
    ///     Such a Field is necessarily treated as a string by users, but the allowed values are discrete rather than
    ///     infinite. Some back-end RDBMS providers (such as MySQL) actually offer an optimized storage mechanism for
    ///     such Fields, where the data is <i>stored</i> as an integer but <i>read and written</i> as a string. Similar
    ///     restrictions on non-text data types require a <see cref="CheckConstraint"/> with a condition based around
    ///     an <see cref="InclusionClause"/>.
    ///   </para>
    /// </remarks>
    public sealed record EnumField : IField {
        /// <inheritdoc/>
        public FieldName Name { get; }

        /// <inheritdoc/>
        public DBType DataType { get; }

        /// <inheritdoc/>
        public IsNullable Nullability { get; }

        /// <inheritdoc/>
        public Option<DBValue> DefaultValue { get; }

        /// <summary>
        ///   The list of values that are allowed for this <see cref="EnumField"/>.
        /// </summary>
        public DBData Enumerators { get; }

        /// <summary>
        ///   Constructs a new <see cref="EnumField"/>.
        /// </summary>
        /// <param name="name">
        ///   The <see cref="Name">name</see> of the new <see cref="EnumField"/>.
        /// </param>
        /// <param name="nullability">
        ///   The <see cref="Nullability">nullability</see> of the new <see cref="EnumField"/>.
        /// </param>
        /// <param name="defaultValue">
        ///   The <see cref="DefaultValue">default value</see> of the new <see cref="EnumField"/>.
        /// </param>
        /// <param name="enumerators">
        ///   The <see cref="Enumerators">restricted enumerator values</see> of the new <see cref="EnumField"/>.
        /// </param>
        /// <pre>
        ///   The arguments provided to the constructor must, collectively, define a valid Field. In addition to
        ///   requiring that all arguments be non-<see langword="null"/>, this means that the
        ///   <paramref name="defaultValue">default value</paramref>, if present, is compatible with the Field's
        ///   <see cref="DBType.Enumeration"/> data type. Furthermore, that compatible default value can only be
        ///   <see cref="DBValue.NULL"/> if the <paramref name="nullability"/> of the Field is
        ///   <see cref="IsNullable.Yes"/>. Finally, there must be at least one
        ///   <paramref name="enumerators">allowed value</paramref>, and all such values must be something other than
        ///   <see cref="DBValue.NULL"/> while maintaining compatability with the <see cref="DBType.Enumeration"/> data
        ///   type.
        /// </pre>
        internal EnumField(FieldName name, IsNullable nullability, Option<DBValue> defaultValue,
            IEnumerable<DBValue> enumerators) {

            Debug.Assert(name is not null);
            Debug.Assert(enumerators is not null);
            Debug.Assert(nullability.IsValid());
            Debug.Assert(nullability == IsNullable.Yes || !defaultValue.Contains(DBValue.NULL));
            Debug.Assert(!enumerators.IsEmpty());
            
            Name = name;
            DataType = DBType.Enumeration;
            Nullability = nullability;
            DefaultValue = defaultValue;
            Enumerators = new List<DBValue>(enumerators);

            // These two Asserts are placed after the constructor body so that we can refer to the DataType property
            // after its value is set, eliminating some duplication of the constant value
            Debug.Assert(defaultValue.ValueOr(DBValue.NULL).IsInstanceOf(DataType));
            Debug.Assert(enumerators.All(v => v.IsInstanceOf(DataType)));
        }

        /* Because EnumField is record type, the following methods are synthesized automatically by the compiler:
         *   > public EnumField(EnumField rhs)
         *   > public bool Equals(EnumField? rhs)
         *   > public sealed override bool Equals(object? rhs)
         *   > public sealed override int GetHashCode()
         *   > public sealed override string ToString()
         *   > public static bool operator==(EnumField? lhs, EnumField? rhs)
         *   > public static bool operator!=(EnumField? lhs, EnumField? rhs)
         */

        /// <inheritdoc/>
        TDecl IField.GenerateDeclaration<TDecl>(IFieldDeclBuilder<TDecl> builder) {
            Debug.Assert(builder is not null);

            builder.SetName(Name);
            builder.SetDataType(DataType);
            builder.SetNullability(Nullability);
            DefaultValue.MatchSome(v => builder.SetDefaultValue(v));
            builder.SetAllowedValues(Enumerators);

            return builder.Build();
        }
    }
}
