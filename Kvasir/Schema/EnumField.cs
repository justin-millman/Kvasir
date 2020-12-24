using Cybele.Extensions;
using Optional;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Kvasir.Schema {
    ///
    public sealed class EnumField : IField {
        /// <inheritdoc/>
        public FieldName Name { get; }

        /// <inheritdoc/>
        public DBType DataType { get; }

        /// <inheritdoc/>
        public IsNullable Nullability { get; }

        /// <inheritdoc/>
        public Option<DBValue> DefaultValue { get; }

        /// <value>
        ///   The values that this <see cref="EnumField"/> is allowed to take on.
        /// </value>
        public IReadOnlyCollection<DBValue> Enumerators { get; }

        /// Constructs a new <see cref="EnumField"/>.
        /// <param name="name">
        ///   The <see cref="Name"/> of the new <see cref="EnumField"/>.
        /// </param>
        /// <param name="nullability">
        ///   The <see cref="Nullability"/> of the new <see cref="EnumField"/>.
        /// </param>
        /// <param name="defaultValue">
        ///   The <see cref="DefaultValue"/> for the new <see cref="EnumField"/>.
        /// </param>
        /// <param name="enumerators">
        ///   The allowed values for the new <see cref="EnumField"/>.
        /// </param>
        /// <pre>
        ///   None of the values in <paramref name="enumerators"/> is <see cref="DBValue.NULL"/>
        ///     --and--
        ///   Each value in <paramref name="enumerators"/> is <see cref="DBType.IsValidValue(DBValue)">valid</see> for
        ///   <see cref="DBType.Enumeration"/>.
        /// </pre>
        /// <exception cref="ArgumentException">
        ///   if <paramref name="nullability"/> is <see cref="IsNullable.No"/> but <paramref name="defaultValue"/> is
        ///   a <c>SOME</c> instance wrapping <see cref="DBValue.NULL"/>
        ///     --or--
        ///   if <paramref name="enumerators"/> is empty
        ///     --or--
        ///   if <paramref name="defaultValue"/> is a <c>SOME</c> instance wrapping a <see cref="DBValue"/> that is
        ///   not <see cref="DBValue.NULL"/> and is not one of the values in <paramref name="enumerators"/>
        /// </exception>
        internal EnumField(FieldName name, IsNullable nullability, Option<DBValue> defaultValue,
            IEnumerable<DBValue> enumerators) {

            Debug.Assert(nullability.IsValid());

            // Check #1: If the Field is non-nullable, then the default value must either be a NONE instance or a SOME
            // instance that contains a value other than DBValue.NULL.
            if (nullability == IsNullable.No && defaultValue.Contains(DBValue.NULL)) {
                var msg = "The default value of a non-nullable Field cannot be NULL";
                throw new ArgumentException(msg);
            }

            // Check #2: There must be at least one provided enumerator
            if (!enumerators.Any()) {
                var msg = $"A Field of type {DBType.Enumeration} must have at least one enumerator";
                throw new ArgumentException(msg);
            }
            Debug.Assert(enumerators.All(v => v != DBValue.NULL && DBType.Enumeration.IsValidValue(v)));

            // Check #3: If the Field has a default value, that value must be one of the provided enumerators. Because
            // all of the enumerators are compatible with DBType.Enumerator (per precondition), we do not need to
            // explicitly check the compatibility of the default value.
            defaultValue.MatchSome(v => {if (!enumerators.Contains(v) && v != DBValue.NULL) {
                    var msg = $"Value {v} cannot be used as the default for a Field restricted to the enumerators [" +
                        string.Join(", ", enumerators) + "]";
                    throw new ArgumentException(msg);
                }
            });

            Name = name;
            DataType = DBType.Enumeration;
            Nullability = nullability;
            DefaultValue = defaultValue;
            Enumerators = new HashSet<DBValue>(enumerators);
        }
    }
}
