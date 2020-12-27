using Cybele.Extensions;
using Optional;
using System;
using System.Diagnostics;

namespace Kvasir.Schema.Constraints {
    /// <summary>
    ///   A representation of an expression involving the value of an <see cref="IField"/> that may form part of a
    ///   <see cref="Clause"/>.
    /// </summary>
    public sealed class FieldExpression {
        /// <value>
        ///   The <see cref="FieldFunction"/> applied to the value of <see cref="Field"/> to form this
        ///   <see cref="FieldExpression"/>.
        /// </value>
        public Option<FieldFunction> Function { get; }

        /// <value>
        ///   The <see cref="IField"/> of this <see cref="FieldExpression"/>.
        /// </value>
        public IField Field { get; }

        /// <value>
        ///   The data type of this <see cref="FieldExpression"/>, accounting for the possible transformation applied
        ///   by <see cref="Function"/>.
        /// </value>
        public DBType DataType { get; }

        /// <summary>
        ///   Constructs a new <see cref="FieldExpression"/> with no value transformation function.
        /// </summary>
        /// <param name="field">
        ///   The <see cref="Field"/> of the new <see cref="FieldExpression"/>.
        /// </param>
        internal FieldExpression(IField field) {
            Function = Option.None<FieldFunction>();
            Field = field;
            DataType = field.DataType;
        }

        /// <summary>
        ///   Constructs a new <see cref="FieldExpression"/> that includes a value transformation function.
        /// </summary>
        /// <param name="function">
        ///   The <see cref="Function"/> of the new <see cref="FieldExpression"/>.
        /// </param>
        /// <param name="field">
        ///   The <see cref="Field"/> of the new <see cref="FieldExpression"/>.
        /// </param>
        /// <exception cref="ArgumentException">
        ///   if <paramref name="function"/> is incompatible with the <see cref="IField.DataType"/> of
        ///   <paramref name="field"/>.
        /// </exception>
        internal FieldExpression(FieldFunction function, IField field) {
            Debug.Assert(function.IsValid());

            if (function == FieldFunction.Length && field.DataType != DBType.Text) {
                var msg = $"The length-of function can only be applied to a Field whose type is {DBType.Text}";
                throw new ArgumentException(msg);
            }

            Function = Option.Some(function);
            Field = field;
            DataType = function switch {
                FieldFunction.Length => DBType.Int32,
                _ => throw new ApplicationException()
            };
        }
    }

    /// <summary>
    ///   A representation of the functions that can be applied to the value of a Field as part of a
    ///   <see cref="FieldExpression"/>.
    /// </summary>
    public enum FieldFunction : byte {
        /// <summary>The function that takes the length of a string</summary>
        Length
    }
}
