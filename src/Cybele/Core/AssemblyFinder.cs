using System.Diagnostics;
using System.Reflection;

namespace Cybele.Core {
    /// <summary>
    ///   A collection of utility methods for identifying information about assemblies.
    /// </summary>
    public static class AssemblyFinder {
        /// <summary>
        ///   Determines the assembly from which a particular source location was invoked, bypassing any <c>System</c>
        ///   assemblies.
        /// </summary>
        /// <remarks>
        ///   The purpose of this function is to discover where a function was called from. The calling function is
        ///   considered to be in the "source" assembly, while the invocation occurred in the "calling" assembly. This
        ///   process bypasses all C# runtime libraries (i.e. those that are prefixed with <c>System.</c>, which ensures
        ///   that <see langword="async"/> methods see through the compiler-injected machinery. Inlining and JIT may
        ///   still produce nondeterministic results.
        /// </remarks>
        /// <param name="source">
        ///   The <see cref="Assembly"/> that called <i>this</i> method.
        /// </param>
        /// <returns>
        ///   The first <see cref="Assembly"/> in the call stack of this function that is neither
        ///   <paramref name="source"/> nor a C# runtime system library (i.e. one prefixed with <c>System.</c>). If no
        ///   such assembly exists, <see langword="null"/>.
        /// </returns>
        public static Assembly? CallerNotFrom(Assembly source) {
            // skip the first frame, because that's the *current* frame
            foreach (var frame in new StackTrace().GetFrames()[1..]) {
                var caller = frame.GetMethod()?.DeclaringType?.Assembly;
                var isNotSystem = (caller?.FullName?.StartsWith("System.") == false);

                if (caller != source && isNotSystem) {
                    return caller;
                }
            }
            return null;
        }
    }
}
