using Kvasir.Annotations;
using System;

namespace Kvasir.Translation {
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
                new Annotation(Display.AnnotationDisplayName(typeof(DataConverterAttribute)))
              )
        {}

        /// <summary>
        ///   Constructs a new <see cref="InvalidDataConverterException"/> describing an annotation where the property's
        ///   CLR type and the Data Converter's expected source type are not compatible.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the invalid annotation was encountered.
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
                new Problem(
                    $"a property of type {propertyType.DisplayName()} cannot use a Data Converter that " +
                    $"expects {converterSourceType.DisplayName()}"
                ),
                new Annotation(Display.AnnotationDisplayName(typeof(DataConverterAttribute)))
              )
        {}

        /// <summary>
        ///   Constructs a new <see cref="InvalidDataConverterException"/> describing an annotation where the Data
        ///   Converter's result type is not supported by Kvasir.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the invalid annotation was encountered.
        /// </param>
        /// <param name="unsupportedResultType">
        ///   The unsupported result type of the Data Converter.
        /// </param>
        public InvalidDataConverterException(Context context, Type unsupportedResultType)
            : base(
                new Location(context.ToString()),
                new Problem($"the result type {unsupportedResultType.DisplayName()} of the Data Converter is not supported"),
                new Annotation(Display.AnnotationDisplayName(typeof(DataConverterAttribute)))
              )
        {}

        /// <summary>
        ///   Constructs a new <see cref="InvalidDataConverterException"/> describing an annotation placed on a
        ///   non-enumeration-type property that affects a data conversion that requires an enumerator source.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which <paramref name="annotation"/> was encountered.
        /// </param>
        /// <param name="propertyType">
        ///   The type of the property on which <paramref name="annotation"/> was placed.
        /// </param>
        /// <param name="annotation">
        ///   The invalid annotation.
        /// </param>
        public InvalidDataConverterException(Context context, Type propertyType, Attribute annotation)
            : base(
                new Location(context.ToString()),
                new Problem($"the annotation cannot be applied to a property of non-enumeration type {propertyType.DisplayName()}"),
                new Annotation(Display.AnnotationDisplayName(annotation.GetType()))
              )
        {}
    }
}
