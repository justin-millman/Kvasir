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
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public sealed class NumericAttribute : Attribute {
        /// <summary>
        ///   The dot-separated path, relative to the property on which the annotation is placed, to the property to
        ///   which the annotation actually applies.
        /// </summary>
        public string Path { internal get; init; } = "";
    }
}
