using System;
using System.Runtime.Serialization;

namespace Kvasir.Exceptions {
    /// <summary>
    ///   A base exception to be raised by the Kvasir framework when malformed, invalid, or contradictory user input
    ///   is encountered.
    /// </summary>
    [Serializable]
    public class KvasirException : Exception {
        /// <summary>
        ///   Constructs a new instance of the <see cref="KvasirException"/> class, setting the <c>Message</c> property
        ///   to an implementation-defined default value and the <c>InnerException</c> property to
        ///   <see langword="null"/>.
        /// </summary>
        public KvasirException()
            : base() {}

        /// <summary>
        ///   Constructs a new instance of the <see cref="KvasirException"/> class, setting the <c>Message</c> property
        ///   to a user-defined value and the <c>InnerException</c> property to <see langword="null"/>.
        /// </summary>
        /// <param name="message">
        ///   A description of the error.
        /// </param>
        public KvasirException(string message)
            : base(message) {}

        /// <summary>
        ///   Constructs a new instance of the <see cref="KvasirException"/> class, setting both the <c>Message</c> and
        ///   <c>InnerException</c> properties to user-defined values.
        /// </summary>
        /// <param name="message">
        ///   A description of the error.
        /// </param>
        /// <param name="innerException">
        ///   The cause of the new exception.
        /// </param>
        public KvasirException(string message, Exception innerException)
            : base(message, innerException) {}

        /// <summary>
        ///   Constructs a new instance of the <see cref="KvasirException"/> class from serialized data.
        /// </summary>
        /// <param name="info">
        ///   The object that holds the serialized data.
        /// </param>
        /// <param name="context">
        ///   The contextual information about the source or destination.
        /// </param>
        private KvasirException(SerializationInfo info, StreamingContext context)
            : base(info, context) {}
    }
}
