using Cybele.Extensions;
using Kvasir.Annotations;
using System;

namespace Kvasir.Translation2 {
    /// <summary>
    ///   An exception that is raised when an attempt to execute an operation that is at least partly user-defined
    ///   fails.
    /// </summary>
    internal sealed class FailedOperationException : TranslationException {
        /// <summary>
        ///   Constructs a new <see cref="FailedOperationException"/> caused by an exception raised when performing a
        ///   data conversion.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the failed data conversion occurred.
        /// </param>
        /// <param name="sourceValue">
        ///   The value that could not be converted..
        /// </param>
        /// <param name="ex">
        ///   The exception raised when attempting to convert <paramref name="sourceValue"/>.
        /// </param>
        public FailedOperationException(Context context, object? sourceValue, Exception ex)
            : base(
                new Location(context.ToString()),
                new Problem($"error converting value {sourceValue.ForDisplay()} ({ex.Message})")
              )
        {}

        /// <summary>
        ///   Constructs a new <see cref="FailedOperationException"/> caused by an exception raised when generating a
        ///   custom simple <c>CHECK</c> constraint.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which <paramref name="annotation"/> was encountered.
        /// </param>
        /// <param name="annotation">
        ///   The problematic <see cref="CheckAttribute">[Check]</see> annotation.
        /// </param>
        /// <param name="ex">
        ///   The exception raised when attempting to generate the custom <c>CHECK</c> constraint using the generator on
        ///   <paramref name="annotation"/>.
        /// </param>
        public FailedOperationException(Context context, CheckAttribute annotation, Exception ex)
            : base(
                new Location(context.ToString()),
                new Problem($"unable to generate custom CHECK constraint ({ex.Message})"),
                new Annotation(Display.AnnotationDisplayName(annotation.GetType()))
              )
        {}

        /// <summary>
        ///   Constructs a new <see cref="FailedOperationException"/> caused by an exception raised when generating a
        ///   custom complex <c>CHECK</c> constraint.
        /// </summary>
        public FailedOperationException(Context context, Check.ComplexAttribute annotation, Exception ex)
            : base(
                new Location(context.ToString()),
                new Problem($"unable to generate custom CHECK constraint ({ex.Message})"),
                new Annotation(Display.AnnotationDisplayName(annotation.GetType()))
              )
        {}
    }
}
