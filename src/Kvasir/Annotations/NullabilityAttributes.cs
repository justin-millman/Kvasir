using System;

namespace Kvasir.Annotations {
    /// <summary>
    ///   An annotation that specifies that the value for the Field backing a particular property <i>cannot</i> be
    ///   <c>NULL</c>.
    /// </summary>
    /// <remarks>
    ///   The Kvasir framework will automatically determine the nullability of backing Fields based on the nullability
    ///   of the POCO property's type. The <see cref="NullableAttribute"/> can be used to override the default
    ///   deduction when that deduction would be incorrect or undesirable.
    /// </remarks>
    /// <seealso cref="NonNullableAttribute"/>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class NullableAttribute : Attribute {}

    /// <summary>
    ///   An annotation that specifies that the value for the Field backing a particular property <i>can</i> be
    ///   <c>NULL</c>.
    /// </summary>
    /// <remarks>
    ///   The Kvasir framework will automatically determine the nullability of backing Fields based on the nullability
    ///   of the POCO property's type. The <see cref="NonNullableAttribute"/> can be used to override the default
    ///   deduction when that deduction would be incorrect.
    /// </remarks>
    /// <seealso cref="NonNullableAttribute"/>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class NonNullableAttribute : Attribute {}
}
