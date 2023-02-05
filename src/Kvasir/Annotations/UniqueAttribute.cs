using Ardalis.GuardClauses;
using System;

namespace Kvasir.Annotations {
    /// <summary>
    ///   An annotation that marks the Field backing a particular property as being part of a <c>UNIQUE</c> constraint.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    public sealed class UniqueAttribute : Attribute {
        /// <summary>
        ///   The name prefix reserved for use by Kvasir itself.
        /// </summary>
        public static string ANONYMOUS_PREFIX => "@@@";

        /// <summary>
        ///   The dot-separated path, relative to the property on which the annotation is placed, to the property to
        ///   which the annotation actually applies.
        /// </summary>
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
            Name = Guard.Against.Null(name);
            IsAnonymous = false;
        }


        private static readonly object LOCK = new();
        private static int sequence_ = 0;
    }
}
