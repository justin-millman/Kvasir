using Kvasir.Schema;
using System;

namespace Kvasir.Annotations {
    /// <summary>
    ///   An annotation that marks the Field backing a particular property as being part of a <c>UNIQUE</c> constraint.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public class UniqueAttribute : Attribute {
        /// <summary>
        ///   The dot-separated path, relative to the property on which the annotation is placed, to the property to
        ///   which the annotation actually applies.
        /// </summary>
        public string Path { internal get; init; } = "";

        /// <summary>
        ///   The key name specified by the annotation
        /// </summary>
        internal KeyName Name { get; }

        /// <summary>
        ///   Constructs a new instance of the <see cref="UniqueAttribute"/> class with an implementation-defined name.
        /// </summary>
        public UniqueAttribute() {
            lock (LOCK) {
                Name = new KeyName($"UNIQUE_FIELD_{sequence_++}");
            }
        }

        /// <summary>
        ///   Constructs a new instance of the <see cref="UniqueAttribute"/> class with a user-defined name.
        /// </summary>
        /// <param name="name">
        ///   The name of the <c>UNIQUE</c> constraint.
        /// </param>
        /// <exception cref="ArgumentException">
        ///   if <paramref name="name"/> is not a valid name for a <c>UNIQUE</c> constraint.
        /// </exception>
        public UniqueAttribute(string name) {
            Name = new KeyName(name);
        }


        private static readonly object LOCK = new();
        private static int sequence_ = 0;
    }
}
