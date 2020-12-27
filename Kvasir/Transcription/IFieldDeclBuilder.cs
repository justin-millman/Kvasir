using Kvasir.Schema;
using System.Collections.Generic;

namespace Kvasir.Transcription.Internal {
    /// <summary>
    ///   An <see cref="IDeclBuilder"/> that builds declaratory <see cref="SqlSnippet">SqlSnippets</see> for Fields.
    /// </summary>
    internal interface IFieldDeclBuilder : IDeclBuilder {
        /// <summary>
        ///   Sets the name of the Field whose declaration is represented by the current state of this
        ///   <see cref="IFieldDeclBuilder"/>.
        /// </summary>
        /// <param name="name">
        ///   The name.
        /// </param>
        void SetName(FieldName name);

        /// <summary>
        ///   Sets the data type of the Field whose declaration is represented by the current state of this
        ///   <see cref="IFieldDeclBuilder"/>.
        /// </summary>
        /// <param name="dataType">
        ///   The data type.
        /// </param>
        void SetDataType(DBType dataType);

        /// <summary>
        ///   Sets the nullability of the Field whose declaration is represented by the current state of this
        ///   <see cref="IFieldDeclBuilder"/>.
        /// </summary>
        /// <param name="nullability">
        ///   The nullability.
        /// </param>
        void SetNullability(IsNullable nullability);

        /// <summary>
        ///   Sets the default value of the Field whose declaration is represented by the current state of this
        ///   <see cref="IFieldDeclBuilder"/>.
        /// </summary>
        /// <param name="defaultValue">
        ///   The default value.
        /// </param>
        void SetDefaultValue(DBValue defaultValue);

        /// <summary>
        ///   Sets the list of values that are allowed for the Field whose declaration is represented by the current
        ///   state of this <see cref="IFieldDeclBuilder"/>.
        /// </summary>
        /// <param name="enumerators">
        ///   The list of alloed values.
        /// </param>
        void SetAllowedValues(IEnumerable<DBValue> enumerators);
    }
}
