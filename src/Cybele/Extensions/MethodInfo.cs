using System;
using System.Linq;
using System.Reflection;

namespace Cybele.Extensions {
    /// <summary>
    ///   A collection of <see href="https://tinyurl.com/y8q6ojue">extension methods</see> that extend the public API
    ///   of the <see cref="MethodInfo"/> class.
    /// </summary>
    public static class MethodExtensions {
        /// <summary>
        ///   Determines if a method is inherited.
        /// </summary>
        /// <param name="self">
        ///   The <see cref="MethodInfo"/> instance on which the extension method is inokved.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if <paramref name="self"/> is a method that overrides a <see langword="virtual"/>
        ///   function inherited from a base class, or if it is a function whose implementation is inherited from a base
        ///   class, or if it is an implementation of an interface method; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsInherited(this MethodInfo self) {
            // Methods that are directly inherited (virtual or non-virtual) are accessed via reflection via one type but
            // are actually declared by another type, so they must be inherited.
            if (self.DeclaringType != self.ReflectedType) {
                return true;
            }

            // Abstract and virtual methods that are overridden in a derived class have a "base definition" where the
            // method was first declared which is necessarily different than the method itself; this does not work with
            // interface implementations
            if (self.GetBaseDefinition() != self) {
                return true;
            }

            // As best I can tell, this is the only way to determine if a method is an interface implementation. There
            // is no attribute or other marking on the MethodInfo, so we have to iterate over the source type's
            // interfaces and look for the method in the mapping.
            foreach (var intfc in self.ReflectedType!.GetInterfaces()) {
                if (self.ReflectedType!.GetInterfaceMap(intfc).TargetMethods.Contains(self)) {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///   Returns <see langword="false"/>, as constructors by definition are never inherited.
        /// </summary>
        /// <paramm name="self">
        ///   The <see cref="ConstructorInfo"/> instance on which the extension method is invoked.
        /// </paramm>
        /// <returns>
        ///   <see langword="false"/>.
        /// </returns>
        public static bool IsInherited(this ConstructorInfo self) {
            return false;
        }
    }
}
