using Cybele;
using Kvasir.Transcription.Internal;
using Optional;
using System;

namespace Kvasir.Schema {
    /// <summary>
    ///   The interface representing a single Field in a back-end relational database.
    /// </summary>
    /// <remarks>
    ///   This interface can only be implemented by classes within the <see cref="Kvasir"/> assembly.
    /// </remarks>
    public interface IField {
        /// <value>
        ///   The name of this <see cref="IField"/>.
        /// </value>
        FieldName Name { get; }

        /// <value>
        ///   The <see cref="DBType"/> of this <see cref="IField"/>.
        /// </value>
        DBType DataType { get; }

        /// <value>
        ///   Whether or not <c>NULL</c> is an acceptable value for this <see cref="IField"/>.
        /// </value>
        IsNullable Nullability { get; }

        /// <value>
        ///   The default value for this <see cref="IField"/>.
        /// </value>
        /// <post>
        ///   <see cref="DefaultValue"/> is a <c>SOME</c> instance wrapping <see cref="DBValue.NULL"/> only if
        ///   <see cref="IsNullable"/> is <see cref="IsNullable.Yes"/>
        ///     --and--
        ///   if <see cref="DefaultValue"/> iis a <c>SOME</c> instance, the value that it wraps is a
        ///   <see cref="DBType.IsValidValue(DBValue)">valid value</see> for <see cref="DataType"/>.
        /// </post>
        /// <remarks>
        ///   A <c>NONE</c> instance indicates that this <see cref="IField"/> does not have a default value, while a
        ///   <c>SOME</c> instance indicates that a default value does exist. Notably, a <c>SOME</c> instance wrapping
        ///   <see cref="DBValue.NULL"/> denotes the presence of a default with this specific datum value rather than
        ///   the absence of a default value.
        /// </remarks>
        Option<DBValue> DefaultValue { get; }

        /// <summary>
        ///   Generates a <see cref="SqlSnippet"/> that declares this <see cref="IField"/> as part of a <c>CREATE
        ///   TABLE</c> statement.
        /// </summary>
        /// <param name="syntax">
        ///   The <see cref="IGeneratorCollection"/> containing the factories that this <see cref="IField"/> should use
        ///   to generate its declaratory <see cref="SqlSnippet"/>.
        /// </param>
        /// <returns>
        ///   A <see cref="SqlSnippet"/> that declares this <see cref="IField"/> within a <c>CREATE TABLE</c>
        ///   statement according to the syntax rules of <paramref name="syntax"/>.
        /// </returns>
        internal SqlSnippet GenerateDeclaration(IGeneratorCollection syntax);
    }

    /// <summary>
    ///   The name of an <see cref="IField"/>.
    /// </summary>
    public sealed class FieldName : ConceptString<FieldName> {
        /// <summary>
        ///   Constructs a new <see cref="FieldName"/>.
        /// </summary>
        /// <param name="name">
        ///   The name. Any leading and trailing whitespace is trimmed.
        /// </param>
        /// <exception cref="ArgumentException">
        ///   if <paramref name="name"/> is the empty string or consists only of whitespace characters.
        /// </exception>
        public FieldName(string name)
            : base(name.Trim()) {

            if (string.IsNullOrWhiteSpace(Contents)) {
                var msg = "The name of a Field must have non-zero length when leading/trailing whitespae is ignored";
                throw new ArgumentException(msg);
            }
        }
    }

    /// <summary>
    ///   A strongly typed binary value representing the nullability of an <see cref="IField"/>
    /// </summary>
    public enum IsNullable : byte {
        /// <summary>Indicates that an <see cref="IField"/> is not nullable</summary>
        No = 0,

        /// <summary>Indicates that an <see cref="IField"/> is nullable</summary>
        Yes = 1
    }
}
