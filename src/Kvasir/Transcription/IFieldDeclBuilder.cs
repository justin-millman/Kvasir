using Kvasir.Schema;
using System.Collections.Generic;

namespace Kvasir.Transcription {
    /// <summary>
    ///   The interface for a builder that produces declarations that define a single Field within a larger
    ///   Table-creating declaration.
    /// </summary>
    /// <typeparam name="TDecl">
    ///   The type of declaration object produced by the builder.
    /// </typeparam>
    public interface IFieldDeclBuilder<TDecl> {
        /// <summary>
        ///   Sets the name of the Field being defined by the current builder's declaration.
        /// </summary>
        /// <param name="name">
        ///   The name.
        /// </param>
        /// <pre>
        ///   <paramref name="name"/> is not <see langword="null"/>.
        /// </pre>
        void SetName(FieldName name);

        /// <summary>
        ///   Sets the data type of the Field being defined by the current builder's declaration.
        /// </summary>
        /// <param name="dataType">
        ///   The data type.
        /// </param>
        void SetDataType(DBType dataType);

        /// <summary>
        ///   Sets the nullability of the Field being defined by the current builder's declaration.
        /// </summary>
        /// <param name="nullability">
        ///   The nullability.
        /// </param>
        /// <pre>
        ///   <paramref name="nullability"/> is valid.
        /// </pre>
        void SetNullability(IsNullable nullability);

        /// <summary>
        ///   Sets the default value of the Field being defined by the current builder's declaration.
        /// </summary>
        /// <param name="value">
        ///   The default value.
        /// </param>
        void SetDefaultValue(DBValue value);

        /// <summary>
        ///   Sets the list of allowed values for the Field being defined by the current builder's declaration.
        /// </summary>
        /// <param name="values">
        ///   The allowed values.
        /// </param>
        /// <pre>
        ///   <paramref name="values"/> is not <see langword="null"/>
        ///     --and--
        ///   <paramref name="values"/> contains at last one element.
        /// </pre>
        void SetAllowedValues(IEnumerable<DBValue> values);

        /// <summary>
        ///   Produce the full declaration that has ben built up by calls into other methods on this
        ///   <see cref="IFieldDeclBuilder{TDecl}"/>.
        /// </summary>
        /// <pre>
        ///   <see cref="SetName(FieldName)"/> has been called at least once
        ///     --and--
        ///   <see cref="SetDataType(DBType)"/> has been called at least once.
        /// </pre>
        /// <returns>
        ///   A <typeparamref name="TDecl"/> declaring a single Field.
        /// </returns>
        TDecl Build();
    }
}
