using Ardalis.GuardClauses;
using Optional;
using System;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace Cybele.Extensions {
    /// <summary>
    ///   A collection of <see href="https://tinyurl.com/y8q6ojue">extension methods</see> that extend the public API
    ///   of the <see cref="Type"/> class.
    /// </summary>
    public static class TypeExtensions {
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

            if (ancestorType.Equals(derivedType) || derivedType.Equals(ancestorType)) {
                // Use `.Equals` in case someone has derived from `Type` and overridden the function; the `operator==`
                // cannot be overridden that way. The symmetric check is for the same reason.
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

        /// <summary>
        ///   Finds the property on a <see cref="Type"/> that has a specific name.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     This method is comprehensive: it searches public and non-public instance and static properties on the
        ///     given type, including properties inherited from base classes or from interfaces. This method can also
        ///     detect properties that are implemented through explicit interfaces, and it can detect the `this[]`
        ///     indexer property as well.
        ///   </para>
        ///   <para>
        ///     Under normal circumstances, there can only be one property with a given name visible through a
        ///     particular type. When a property hides another (with or without the use of the <c>new</c> keyword), the
        ///     one defined by the most-derived type will take precedence and be located. There are only two known
        ///     circumstances in which multiple properties of the same name can otherwise be detected: when there are
        ///     multiple overloads of the `this[]` indexer property, and when a type explicitly implements two
        ///     properties of the same name from different interfaces. In both of these circumstances, an
        ///     <see cref="AmbiguousMatchException"/> will be raised. If a type has both an explicit interface
        ///     implementation and a vanilla property of the same name, the latter will be returned.
        ///   </para>
        ///   <para>
        ///     The only scenario in which this function will fail to locate a property that actually exists is if the
        ///     property is an explicit interface implementation on a base class of the target type.
        ///   </para>
        ///   <para>
        ///     This function is only guaranteed to work with bona fide CLR types. It may work with types that are
        ///     created through reflection, provided that the properties exposed have a non-<see langword="null"/>
        ///     <see cref="MemberInfo.DeclaringType"><c>DeclaringType</c></see>, but it may also fail to function.
        ///   </para>
        /// </remarks>
        /// <param name="type">
        ///   The <see cref="Type"/> to search for properties.
        /// </param>
        /// <param name="propertyName">
        ///   The name of the target property.
        /// </param>
        /// <returns>
        ///   An engaged optional holding the <see cref="PropertyInfo"/> of the property with name
        ///   <paramref name="propertyName"/>, if such a property exists on <paramref name="type"/>; otherwise, a
        ///   disengaged optional.
        /// </returns>
        /// <exception cref="AmbiguousMatchException">
        ///   if multiple properties named <paramref name="propertyName"/> are found on <paramref name="type"/>.
        /// </exception>
        public static Option<PropertyInfo> GetPropertyNamed(this Type type, string propertyName) {
            Guard.Against.Null(type, nameof(type));
            Guard.Against.Null(propertyName, nameof(propertyName));

            var flags = BindingFlags.Public;            // include public properties
            flags |= BindingFlags.NonPublic;            // include non-public properties
            flags |= BindingFlags.Static;               // include static properties
            flags |= BindingFlags.Instance;             // include non-static (instance) properties
            flags |= BindingFlags.FlattenHierarchy;     // include static properties declared on base classes

            // Here's why we have to use the branchy helper. `GetProperty(string)` will throw an
            // `AmbiguousMatchException` if the source type inherits a property and then hides it (with or without the
            // `new` keyword). However, we want to support this by finding the hiding property, even if that hiding
            // property was declared in a base class. We can't key on the `DeclaringType` up front, because that would
            // wrongly filter out inherited properties, so we have to first winnow the set of all properties down to
            // those matching the specified name, and then check our three cases: none found (return null), exactly one
            // found (return it), or multiple found (return the one declared in the most-derived class, and throw an
            // `AmbiguousMatchException` if there are multiple, which should only ever be the case for the indexer
            // property `Item` or if a type explicitly implements two methods with the same name from different
            // interfaces).
            PropertyInfo? lookupProperty() {
                // Explicit interface implementations show up in the reflection metadata with a fully-qualified name,
                // including namespace. This means that it would end with a `.` followed by the target name: the `.`
                // comes from the qualification through the interface. A `.` is not a valid character in a property name
                // otherwise, so we know that if we see something matching that pattern, it must be the property we're
                // interested in. Not even attributes can change the name of a property as it appears in the reflected
                // `PropertyInfo`. (This would break for any properties *created* via reflection, but that is an edge
                // case in which I am not interested.)
                var properties = type.GetProperties(flags).Where(p => p.Name == propertyName || p.Name.EndsWith($".{propertyName}")).ToArray();

                if (properties.Length == 0) {
                    return null;
                }
                else if (properties.Length == 1) {
                    return properties[0];
                }
                else {
                    var declaringTypes = properties.Select(p => p.DeclaringType).Where(p => p is not null).Distinct();
                    var mostDerived = declaringTypes.Single(t => declaringTypes.All(p => t!.IsInstanceOf(p!)));
                    var matches = properties.Where(p => p.DeclaringType == mostDerived).ToArray();

                    if (matches.Length == 1) {
                        return matches[0];
                    }
                    else {
                        throw new AmbiguousMatchException($"more than one property named {propertyName} found");
                    }
                }
            }

            var prop = lookupProperty();
            if (prop is null) {
                return Option.None<PropertyInfo>();
            }
            return Option.Some(prop);
        }
    }
}
