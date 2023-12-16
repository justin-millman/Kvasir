using Kvasir.Schema;
using System;

namespace Kvasir.Annotations {
    /// <summary>
    ///   An annotation that specifies the name of the Field backing a particular property.
    /// </summary>
    /// <remarks>
    ///   The Kvasir framework will automatically determine the name of backing Fields based on the name of the POCO
    ///   property. The <see cref="NameAttribute"/> can be used to override the default deduction when that deduction
    ///   would be incorrect or undesirable.
    /// </remarks>
    /// <seealso cref="FieldName"/>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    public sealed class NameAttribute : Attribute, INestableAnnotation {
        /// <inheritdoc/>
        public string Path { get; init; } = "";

        /// <summary>
        ///   The Field name specified by the annotation.
        /// </summary>
        internal string Name { get; }

        /// <summary>
        ///   Constructs a new instance of the <see cref="NameAttribute"/>.
        /// </summary>
        /// <param name="name">
        ///   The name of the Field.
        /// </param>
        public NameAttribute(string name) {
            Name = name;
        }

        /// <inheritdoc/>
        INestableAnnotation INestableAnnotation.WithPath(string path) {
            return new NameAttribute(Name) { Path = path };
        }
    }
}
