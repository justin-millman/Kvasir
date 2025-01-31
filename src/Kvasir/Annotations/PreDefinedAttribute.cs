using System;

namespace Kvasir.Annotations {
    /// <summary>
    ///   An annotation that marks an Entity as pre-defined.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     A Pre-Defined Entity is one that exposes a discrete set of instances, much the same way that an enumeration
    ///     does. However, in contrast to an enumeration, the instances of a Pre-Defined Entity can have arbitrary
    ///     metadata, including references and relations. The metadata for a Pre-Defined Entity cannot change (that is,
    ///     all properties must be read- or init-only), and is therefore not loaded out of the database at application
    ///     start-up (the assumption being that all metadata is effectively hard-coded).
    ///   </para>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class PreDefinedAttribute : Attribute {}
}
