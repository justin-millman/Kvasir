using System;

namespace Kvasir.Annotations {
    /// <summary>
    ///   An annotation that specifies that the type of the Field backing a particular property should correspond to
    ///   the underlying numeric type of the property's own type, which must be an <see cref="Enum"/>.
    /// </summary>
    /// <remarks>
    ///   The default storage scheme for Enumeration-type Fields depends on the capabilities of the back-end database,
    ///   but decays to <c>string</c> (with <c>CHECK</c> constraints as possible) in the absence of any explicit
    ///   enumeration support. The <see cref="NumericAttribute"/> can be used to override the default storage behavior
    ///   and insist that the storage be a numeric type instead.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    public sealed class NumericAttribute : Attribute, INestableAnnotation {
        /// <inheritdoc/>
        public string Path { get; init; } = "";

        /// <inheritdoc/>
        INestableAnnotation INestableAnnotation.WithPath(string path) {
            return new NumericAttribute() { Path = path };
        }
    }

    /// <summary>
    ///   An annotation that specifies that the type of the Field backing a particular property should correspond to the
    ///   built-in string representation of the property's own type, which must be an <see cref="Enum"/>.
    /// </summary>
    /// <remarks>
    ///   The default storage scheme for Enumeration-type Fields depends on the capabilities of the back-end database,
    ///   but decays to <c>string</c> (with <c>CHECK</c> constraints as possible) in the absence of any explicit
    ///   enumeration support. the <see cref="AsStringAttribute"/> can be used to override the default storage behavior
    ///   and insist that the storage be a string type even if enumeration support is available.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    public sealed class AsStringAttribute : Attribute, INestableAnnotation {
        /// <inheritdoc/>
        public string Path { get; init; } = "";

        /// <inheritdoc/>
        INestableAnnotation INestableAnnotation.WithPath(string path) {
            return new AsStringAttribute() { Path = path };
        }
    }
}
