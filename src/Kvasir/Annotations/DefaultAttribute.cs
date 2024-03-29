using System;

namespace Kvasir.Annotations {
    /// <summary>
    ///   An annotation that defines the default value for the Field backing a particular property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    public sealed class DefaultAttribute : Attribute, INestableAnnotation {
        /// <inheritdoc/>
        public string Path { get; init; } = "";

        /// <summary>
        ///   The default value specified by the annotation. This value is guaranteed not to be <see langword="null"/>;
        ///   the sentinel value <see cref="DBNull.Value"/> is used instead.
        /// </summary>
        internal object Value { get; }

        /// <summary>
        ///   Constructs a new instance of the <see cref="DefaultAttribute"/> class.
        /// </summary>
        /// <param name="value">
        ///   The default value.
        /// </param>
        /// <remarks>
        ///   A default value of <c>NULL</c> is fundamentally different than the absence of a default value. The former
        ///   indicates that, in the absence of a user-provided value, a Field should take on the value of <c>NULL</c>.
        ///   The latter, meanwhile, indicates that it is an error for the user to omit a value for a Field. If the
        ///   desire is to make a Field "required," do not place a <see cref="DefaultAttribute"/> on the corresponding
        ///   property.
        /// </remarks>
        public DefaultAttribute(object? value) {
            Value = value ?? DBNull.Value;
        }

        /// <inheritdoc/>
        INestableAnnotation INestableAnnotation.WithPath(string path) {
            return new DefaultAttribute(Value) { Path = path };
        }
    }
}
