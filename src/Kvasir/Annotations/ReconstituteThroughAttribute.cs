using System;

namespace Kvasir.Annotations {
    /// <summary>
    ///   An annotation that directs Kvasir to use a particular constructor for reconstitution.
    /// </summary>
    [AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
    public sealed class ReconstituteThroughAttribute : Attribute {}
}
