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
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class ExcludeNamespaceFromNameAttribute : Attribute {}
}
