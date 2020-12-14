using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Atropos {
#if DEBUG
    internal sealed class DebugTestMethod : TestMethodAttribute {}
    internal sealed class ReleaseTestMethod : Attribute {}
#else
    internal sealed class DebugTestMethod : Attribute {}
    internal sealed class ReleaseTestMethod : TestMethodAttribute {}
#endif
}
