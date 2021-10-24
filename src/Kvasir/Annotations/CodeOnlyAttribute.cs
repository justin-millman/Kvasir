using System;

namespace Kvasir.Annotations {
    /// <summary>
    ///   An annotation that marks a particular property as "code-only."
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     A "code-only" property is one whose value is not stored in the back-end database. The value of such a
    ///     property is the domain of the application solely; examples include internal state tracking, sensitive user
    ///     data that is otherwise persisted, or content that is temporally dependent. Because the value of a code-only
    ///     property never propagates to the database, the property itself does not participate in construction
    ///     resolution (that is, it must have an implicit default value).
    ///   </para>
    ///   <para>
    ///     A <see cref="CodeOnlyAttribute"/> that is applied to a property of aggregate type <i>without</i> a path
    ///     parameterization recursively applies to all nested properties.
    ///   </para>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class CodeOnlyAttribute : Attribute {
        /// <summary>
        ///   The dot-separated path, relative to the property on which the annotation is placed, to the property to
        ///   which the annotation actually applies.
        /// </summary>
        public string Path { internal get; init; } = "";
    }
}
