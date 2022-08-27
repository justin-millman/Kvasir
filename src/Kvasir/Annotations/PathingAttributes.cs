using System;

namespace Kvasir.Annotations {
    /// <summary>
    ///   An annotation that specifies that the namespace of an Entity should not be included in the corresponding Table
    ///   name.
    /// </summary>
    /// <remarks>
    ///   The Kvasir framework will automatically determine the name of primary backing Tables based on the namespace
    ///   and name of the POCO. The namespace is included to ensure uniqueness among all Tables, since different C#
    ///   namespaces can define classes with the same name. The <see cref="ExcludeNamespaceFromNameAttribute"/> directs
    ///   Kvasir to ignore the POCO's namespace entirely, making a promise that the POCO's name is globally unique among
    ///   types being treated by the framework.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class ExcludeNamespaceFromNameAttribute : Attribute {}

    /// <summary>
    ///   An annotation that specifies how the properties in the path of a POCO property nested in an Aggregate are
    ///   combined into a single Field name.
    /// </summary>
    /// <remarks>
    ///   The Kvasir framework will automatically determine the name of backing Fields for POCO properties defined on
    ///   Aggregates by concatenating the access path to each property with a specific character. The character used by
    ///   default is dependent on the specific back-end database provider, pursuant to its syntax rules. The
    ///   <see cref="PathSeparatorAttribute"/> directs Kvasir to use a specific character instead. This directive
    ///   applies to all Fields dervied from the annotated Aggregate property, except for those whose names are
    ///   explicitly specified by a <see cref="NameAttribute"/>.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class PathSeparatorAttribute : Attribute {
        /// <summary>
        ///   The separator.
        /// </summary>
        public char Separator { internal get; init; }

        /// <summary>
        ///   Constructs a new instance of the <see cref="PathSeparatorAttribute"/>.
        /// </summary>
        /// <param name="separator">
        ///   The separator.
        /// </param>
        public PathSeparatorAttribute(char separator) {
            Separator = separator;
        }
    }
}
