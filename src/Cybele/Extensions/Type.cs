using Ardalis.GuardClauses;
using System;
using System.Linq;

namespace Cybele.Extensions {
    /// <summary>
    ///   A collection of <see href="https://tinyurl.com/y8q6ojue">extension methods</see> that extend the public API
    ///   of the <see cref="System.Reflection.MemberInfo"/> class.
    /// </summary>
    public static partial class TypeExtensions {
        /// <summary>
        ///   Determines if an instance of one <see cref="Type"/> would also be an instance of another
        ///   <see cref="Type"/> based on an identity, inheritance, or interface implementation relationship.
        /// </summary>
        /// <param name="derivedType">
        ///   The hypothetical descendant <see cref="Type"/>.
        /// </param>
        /// <param name="ancestorType">
        ///   The hypothetical ancestor <see cref="Type"/>.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if <paramref name="ancestorType"/> is identical to <paramref name="derivedType"/>,
        ///   is a base class of <paramref name="derivedType"/>, or is an interface of <paramref name="derivedType"/>;
        ///   otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsInstanceOf(this Type derivedType, Type ancestorType) {
            Guard.Against.Null(ancestorType, nameof(ancestorType));
            Guard.Against.Null(derivedType, nameof(derivedType));

            if (ancestorType == typeof(object)) {
                // We need this branch called out explicitly because Interfaces do not have object in their inheritance
                // hierarchy, so the "standard" algorithm (i.e. the following branches) would result in a false
                // negative. As set up currently, there is an extra check when recursing through the parents; this
                // could easily be refactored out with a helper function or a for loop if so desired.
                return true;
            }
            else if (ancestorType == derivedType) {
                return true;
            }
            else if (derivedType.GetInterfaces().Contains(ancestorType)) {
                return true;
            }
            else {
                var parent = derivedType.BaseType;
                if (parent is not null) {
                    return IsInstanceOf(parent, ancestorType);
                }
                return false;
            }
        }
    }
}
