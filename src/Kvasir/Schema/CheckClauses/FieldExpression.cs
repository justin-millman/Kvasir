using Cybele.Extensions;
using Optional;
using System;
using System.Diagnostics;

namespace Kvasir.Schema {
    /// <summary>
    ///   An expression over the value of a Field that can be used as part of a conditional <see cref="Clause"/>.
    /// </summary>
    public sealed record FieldExpression {
        /// <summary>
        ///   The function applied to the value of the <see cref="Field">Field</see> in this
        ///   <see cref="FieldExpression"/>.
        /// </summary>
        public Option<FieldFunction> Function { get; }

        /// <summary>
        ///   The Field whose value is the subject of this <see cref="FieldExpression"/>.
        /// </summary>
        public IField Field { get; }

        /// <summary>
        ///   The data type of the result of this <see cref="FieldExpression"/>, accounting for the
        ///   <see cref="Function">function applied</see>.
        /// </summary>
        public DBType DataType { get; }

        /// <summary>
        ///   Constructs a new <see cref="FieldExpression"/> with no applied function.
        /// </summary>
        /// <param name="field">
        ///   The <see cref="Field">subject Field</see> of the new <see cref="FieldExpression"/>.
        /// </param>
        /// <pre>
        ///   <paramref name="field"/> is not <see langword="null"/>.
        /// </pre>
        internal FieldExpression(IField field) {
            Debug.Assert(field is not null);

            Function = Option.None<FieldFunction>();
            Field = field;
            DataType = Field.DataType;
        }

        /// <summary>
        ///   Constructs a new <see cref="FieldExpression"/> with an applied function.
        /// </summary>
        /// <param name="function">
        ///   The <see cref="Function">applied function</see>.
        /// </param>
        /// <param name="field">
        ///   The <see cref="Field">subject Field</see> of the new <see cref="FieldExpression"/>.
        /// </param>
        /// <pre>
        ///   <paramref name="field"/> is not <see langword="null"/>
        ///     --and--
        ///   if <paramref name="function"/> is <see cref="FieldFunction.LengthOf"/>, then the
        ///   <see cref="IField.DataType">data type</see> of <paramref name="field"/> is <see cref="DBType.Text"/>.
        /// </pre>
        internal FieldExpression(FieldFunction function, IField field) {
            Debug.Assert(function.IsValid());
            Debug.Assert(field is not null);
            Debug.Assert(function != FieldFunction.LengthOf || field.DataType == DBType.Text);

            Function = Option.Some(function);
            Field = field;
            DataType = function switch {
                FieldFunction.LengthOf => DBType.Int32,
                _ => throw new UnreachableException($"Switch statement over {nameof(FieldFunction)} exhausted")
            };
        }

        /* Because FieldExpression is record type, the following methods are synthesized automatically by the compiler:
         *   > public FieldExpression(FieldExpression rhs)
         *   > public bool Equals(FieldExpression? rhs)
         *   > public sealed override bool Equals(object? rhs)
         *   > public sealed override int GetHashCode()
         *   > public sealed override string ToString()
         *   > public static bool operator==(FieldExpression? lhs, FieldExpression? rhs)
         *   > public static bool operator!=(FieldExpression? lhs, FieldExpression? rhs)
         */
    }

    /// <summary>
    ///   An enumeration representing the functions that can be applied to the value of a Field.
    /// </summary>
    public enum FieldFunction : byte {
        /// <summary>
        ///   The function that takes the length of a value. This function is only valid for string-like fields, which
        ///   does not include enumerations.
        /// </summary>
        LengthOf
    }
}
