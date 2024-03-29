using System;

namespace Kvasir.Annotations {
    /// <summary>
    ///   An annotation that marks the Field backing a particular property as being part of a <c>UNIQUE</c> constraint.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    public sealed class UniqueAttribute : Attribute, INestableAnnotation {
        /// <summary>
        ///   The name prefix reserved for use by Kvasir itself.
        /// </summary>
        public static string ANONYMOUS_PREFIX => "@@@";

        /// <inheritdoc/>
        public string Path { get; init; } = "";

        /// <summary>
        ///   The Candidate Key name specified by the annotation.
        /// </summary>
        internal string Name { get; }

        /// <summary>
        ///   Whether or not a <see cref="Name"/> was provided when the <see cref="UniqueAttribute"/> was constructed.
        /// </summary>
        internal bool IsAnonymous { get; }

        /// <summary>
        ///   Constructs a new instance of the <see cref="UniqueAttribute"/> class with an implementation-defined name.
        /// </summary>
        public UniqueAttribute() {
            lock (LOCK) {
                Name = $"{ANONYMOUS_PREFIX}UNIQUE_FIELD_{sequence_++}";
            }
            IsAnonymous = true;
        }

        /// <summary>
        ///   Constructs a new instance of the <see cref="UniqueAttribute"/> class with a user-defined name.
        /// </summary>
        /// <param name="name">
        ///   The name of the <c>UNIQUE</c> constraint.
        /// </param>
        public UniqueAttribute(string name) {
            Name = name;
            IsAnonymous = false;
        }

        /// <summary>
        ///   Constructs a new instance of the <see cref="UniqueAttribute"/> class with the dedicated Synthetic name.
        /// </summary>
        /// <param name="_">
        ///   A placeholder argument to control overload resolution.
        /// </param>
        internal UniqueAttribute(char _) {
            Name = SYNTHETIC_NAME;
            IsAnonymous = true;
        }

        /// <summary>
        ///   Constructs a new instance of the <see cref="UniqueAttribute"/> class.
        /// </summary>
        /// <param name="name">
        ///   The name of the <c>UNIQUE</c> constraint.
        /// </param>
        /// <param name="anonymous">
        ///   Whether or not the <c>UNIQUE</c> constraint is "anonymous."
        /// </param>
        private UniqueAttribute(string name, bool anonymous) {
            Name = name;
            IsAnonymous = anonymous;
        }

        /// <inheritdoc/>
        INestableAnnotation INestableAnnotation.WithPath(string path) {
            return new UniqueAttribute(Name, IsAnonymous) { Path = path };
        }


        private static readonly object LOCK = new();
        private static int sequence_ = 0;
        private static readonly string SYNTHETIC_NAME = $"{ANONYMOUS_PREFIX}-SYNTHETIC";
    }
}
