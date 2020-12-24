using Cybele.Extensions;
using Optional;
using System;
using System.Diagnostics;

namespace Kvasir.Schema {
    /// <summary>
    ///   An <see cref="IField"/> representing a single Field in a back-end database that has no implicit restrictions
    ///   on the values that may be assumed by the Field's data.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     Within Kvasir, the role of any <see cref="IField"/> is to describe the attributes of a Field that are
    ///     independent of the Table in which it appears. For example, the <see cref="IField"/> interface does not
    ///     expose a column index, as this is a function of the Field's presence in a Table rather than an an inherent
    ///     attribute of the Field itself. This also means that most constraints placed on a Field are considered
    ///     extrinsic to the Field definition and are therefore not carried by the <see cref="IField"/> implementation.
    ///   </para>
    ///   <para>
    ///     This pattern naturally extends to <see cref="BasicField"/>, which represents a Field where the set of
    ///     allowed values is implicitly equal to the codomain of the Field's <see cref="IField.DataType"/>. This does
    ///     not mean, however, that the set of allowed values for a <see cref="BasicField"/> cannot be contrained via
    ///     a <c>CHECK</c> (or equivalent) constraint at the Table; such a constriant would be perfectly valid.
    ///     However, a constraint in that vein would be fundamentally different than, for example, the inherent
    ///     constraint placed upon a Field representing an enumeration; such a Field would not be represented within
    ///     Kvasir by a <see cref="BasicField"/>.
    ///   </para>
    /// </remarks>
    public sealed class BasicField : IField {
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
        ///   The <see cref="Name"/> of the new <see cref="BasicField"/>.
        /// </param>
        /// <param name="dataType">
        ///   The <see cref="DataType"/> of the new <see cref="BasicField"/>.
        /// </param>
        /// <param name="nullability">
        ///   The <see cref="Nullability"/> of the new <see cref="BasicField"/>.
        /// </param>
        /// <param name="defaultValue">
        ///   The <see cref="DefaultValue"/> for the new <see cref="BasicField"/>.
        /// </param>
        /// <pre>
        ///   <paramref name="dataType"/> is not <see cref="DBType.Enumeration"/>.
        /// </pre>
        /// <exception cref="ArgumentException">
        ///   if <paramref name="nullability"/> is <see cref="IsNullable.No"/> but <paramref name="defaultValue"/> is
        ///   a <c>SOME</c> instance wrapping <see cref="DBValue.NULL"/>
        ///     --or--
        ///   if <paramref name="defaultValue"/> is a <c>SOME</c> instance wrapping a <see cref="DBValue"/> that is
        ///   <see cref="DBType.IsValidValue(DBValue)">not valid</see> for <paramref name="dataType"/>.
        /// </exception>
        internal BasicField(FieldName name, DBType dataType, IsNullable nullability, Option<DBValue> defaultValue) {
            Debug.Assert(dataType != DBType.Enumeration);
            Debug.Assert(nullability.IsValid());

            // Check #1: If the Field is non-nullable, then the default value must either be a NONE instance or a SOME
            // instance that contains a value other than DBValue.NULL.
            if (nullability == IsNullable.No && defaultValue.Contains(DBValue.NULL)) {
                var msg = "The default value of a non-nullable Field cannot be NULL";
                throw new ArgumentException(msg);
            }

            // Check #2: If the Field has a default value, that value must be compatible with the specified DBType.
            // DBValue.NULL is a valid value for any DBType, so we don't have to do anything extra to account for that
            // edge case.
            defaultValue.MatchSome(v => {
                if (!dataType.IsValidValue(v)) {
                    var msg = $"Value {v} cannot be used as the default for a Field with DBType {dataType}";
                    throw new ArgumentException(msg);
                }
            });

            Name = name;
            DataType = dataType;
            Nullability = nullability;
            DefaultValue = defaultValue;
        }
    }
}
