using System;
using System.Collections.Generic;

namespace UT.Kvasir.Translation {
    internal static class Globals {
        public static Func<Type, IEnumerable<object>> NO_ENTITIES => _ => Array.Empty<object>();
    }
}
