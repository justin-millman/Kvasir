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
        ///   Determines if an instance of one <see cref="Type"/> is also an instance of another <see cref="Type"/>
        ///   using a limited set of possible conversions.
        /// </summary>
        /// <remarks>
        ///   A type <c>DERIVED</c> is said to be an instance of another type <c>ANCESTOR</c> if, and only if, at least
        ///   one of the following is true:
        ///
        ///       * <c>DERIVED</c> and <c>ANCESTOR</c> are the same, including open (or partially open) generics
        ///       * <c>ANCESTOR</c> is a direct or indirect base of <c>DERIVED</c>; this covers the former being
        ///             <see cref="object"/> for all types, <see cref="ValueType"/> for structs and primitives,
        ///             <see cref="Enum"/> for enumerations, and <see cref="Delegate"/> for delegates
        ///       * <c>ANCESTOR</c> is an interface that is implemented by <c>DERIVED</c>
        ///       * <c>ANCESTOR</c> is an instantiation of <see cref="Nullable{T}"/> and <c>DERIVED</c> is the
        ///             corresponding generic argument
        ///
        ///   Note specifically that a handful of situations that one may otherwise expect to be recognized as
        ///   fulfilling the "is-instance-of" relation specifically do not:
        ///
        ///       * <c>DERIVED</c> is a narrower numeric type compared to <c>ANCESTOR</c>
        ///       * There exists an implicit conversion between otherwise unrelated <c>DERIVED</c> and <c>ANCESTOR</c>
        ///       * <c>DERIVED</c> is an instantiation of the open (or partially open) generic <c>ANCESTOR</c>
        ///       * <c>DERIVED</c> is a co- or contravariant instantiation of <c>ANCESTOR</c>
        ///       * <c>DERIVED</c> and <c>ANCESTOR</c> are interchangeable delegates
        /// </remarks>
        /// <param name="derivedType">
        ///   The hypothetical descendant <see cref="Type"/>.
        /// </param>
        /// <param name="ancestorType">
        ///   The hypothetical ancestor <see cref="Type"/>.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if any of the above-listed relationships holds between
        ///   <paramref name="derivedType"/> and <paramref name="ancestorType"/>; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsInstanceOf(this Type derivedType, Type ancestorType) {
            Guard.Against.Null(ancestorType, nameof(ancestorType));
            Guard.Against.Null(derivedType, nameof(derivedType));

            if (ancestorType == derivedType) {
                return true;
            }
            else if (ancestorType.IsGenericType && ancestorType.GetGenericTypeDefinition() == typeof(Nullable<>)) {
                // Only non-nullable value types are allowed as the generic argument to the Nullable<T> struct, and
                // specifically both Enum and ValueType do not fit this definition. Therefore, the generic argument
                // must be a primitive (which cannot be subclassed) or an enumeration (which also cannot be
                // subclassed). None of the remaining checks can therefore be relevant.
                return ancestorType.GetGenericArguments()[0] == derivedType;
            }
            else if (ancestorType.IsGenericTypeDefinition || derivedType.IsGenericTypeDefinition) {
                return false;
            }
            else if (derivedType.IsSubclassOf(ancestorType)) {
                return true;
            }
            else if (derivedType.GetInterfaces().Contains(ancestorType)) {
                return true;
            }
            else {
                return false;
            }
        }
    }
}
