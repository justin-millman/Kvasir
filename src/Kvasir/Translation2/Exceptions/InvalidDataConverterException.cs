using Kvasir.Annotations;
using System;

namespace Kvasir.Translation2 {
    /// <summary>
    ///   An exception that is raised when the contents of a <see cref="DataConverterAttribute">[DataConverter]</see>
    ///   annotation render it invalid for the property on which it is applied.
    /// </summary>
    internal sealed class InvalidDataConverterException : TranslationException {
        /// <summary>
        ///   Constructs a new <see cref="InvalidDataConverterException"/> describing a "user error," such as if the
        ///   type provided to the annotation was not actually a Data Converter.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the invalid annotation was encountered.
        /// </param>
        /// <param name="userError">
        ///   The user error string.
        /// </param>
        public InvalidDataConverterException(Context context, string userError)
            : base(
                new Location(context.ToString()),
                new Problem(userError),
                new Annotation(nameof(DataConverterAttribute)[..^9])
              )
        {}

        /// <summary>
        ///   Constructs a new <see cref="InvalidDataConverterException"/> describing an annotation where the property's
        ///   CLR type and the Data Converter's expected source type are not compatible.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the reference cycle was detected.
        /// </param>
        /// <param name="propertyType">
        ///   The CLR type of the property on which the annotation was placed.
        /// </param>
        /// <param name="converterSourceType">
        ///   The expected source type of the Data Converter.
        /// </param>
        public InvalidDataConverterException(Context context, Type propertyType, Type converterSourceType)
            : base(
                new Location(context.ToString()),
                new Problem($"property of type {propertyType} cannot use a Data Converter that expects {converterSourceType}"),
                new Annotation(nameof(DataConverterAttribute)[..^9])
              )
        {}

        /// <summary>
        ///   Constructs a new <see cref="InvalidDataConverterException"/> describing an annotation where the Data
        ///   Converter's result type is not supported by Kvasir.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the reference cycle was detected.
        /// </param>
        /// <param name="unsupportedResultType">
        ///   The unsupported result type of the Data Converter.
        /// </param>
        public InvalidDataConverterException(Context context, Type unsupportedResultType)
            : base(
                new Location(context.ToString()),
                new Problem($"result type {unsupportedResultType} of Data Converter is not supported"),
                new Annotation(nameof(DataConverterAttribute)[..^9])
              )
        {}
    }
}
