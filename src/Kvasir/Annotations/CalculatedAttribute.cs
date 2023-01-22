using System;

namespace Kvasir.Annotations {
    /// <summary>
    ///   An annotation that marks the Field backing a particular property as "calculated."
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     A "calculated" Field is one whose value is stored in the back-end database but not loaded therefrom on read
    ///     operations. The value of such a Field is invariably determined by the value of one or more other Fields in
    ///     the same Table; common examples include alphabetization keys, computed averages/percentages, deduced
    ///     geographies, and checksums. Because the value of such a Field is not loaded, the annotated property does
    ///     not participate in construction resolution.
    ///   </para>
    ///   <para>
    ///     A <see cref="CalculatedAttribute"/> that is applied to a property of aggregate type recursively applies to
    ///     all properties of the aggregate. 
    ///   </para>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class CalculatedAttribute : Attribute {}
}
