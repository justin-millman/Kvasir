using Ardalis.GuardClauses;
using Cybele.Extensions;
using Optional;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Cybele.Core {
    /// <summary>
    ///   A chain of readable properties.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     The <see cref="PropertyChain"/> class is intended to facilitate repeated, iterated property reads via
    ///     reflection. Each "link" in the chain represents a single readable property; the value read for each "link"
    ///     serves as the input to the subsequent "link." If at any point a <see langword="null"/> value is obtained,
    ///     continuation down the chain is immediately terminated. In this way, a <see cref="PropertyChain"/> mimics the
    ///     behavior of performing a property access of the form <c>A?.B?.C?.D</c>.
    ///   </para>
    ///   <para>
    ///     The "links" in a <see cref="PropertyChain"/> are fully covariant: if the value produced by "link" <c>N</c>
    ///     is of the type <c>T</c>, then the subsequent "link" <c>N + 1</c> can expect a source object of <c>T</c> or
    ///     any base class or interface of <c>T</c>. The read operations see through <see langword="virtual"/> and
    ///     <see langword="abstract"/> functions, resolving to the most derived definition regardless of the source
    ///     object. The same is true for explicit interface implementations, provided that the "link" expects the type
    ///     of the interface: the result is the equivalent of <c>A?.B?.(C as SomeInterface)?.D</c>.
    ///   </para>
    ///   <para>
    ///     There are generally three ways to create or extend a <see cref="PropertyChain"/>: by property, by name, and
    ///     by chain. The first and third of these are straightforward: the covariance rules applied to verify that the
    ///     resulting chain is still valid, and a new <see cref="PropertyChain"/> is created. The middle option,
    ///     however, carries a nuance when it come to property hiding. If the expected source type of the chain inherits
    ///     a definition of a property with the given name and then hides it with the <c>new</c> keyword, the hiding
    ///     property will be used rather than the hidden property. To use the hidden property instead, use the
    ///     by-property API and provide a <see cref="PropertyInfo"/> sourced from a base type or interface that knows
    ///     only about the hidden property.
    ///   </para>
    ///   <para>
    ///     Because the purpose of a <see cref="PropertyChain"/> is to enable iterated reads, all properties used in a
    ///     <see cref="PropertyChain"/> must be readable; this means that write-only and init-only properties are
    ///     forbidden. However, there is no limit to the access specifier of the properties used: <c>public</c>,
    ///     <c>private</c>, <c>protected</c>, <c>internal</c>, and <c>internal protected</c> properties are all
    ///     supported. Likewise, both instance properties and <see langword="static"/> properties are supported.
    ///   </para>
    /// </remarks>
    public sealed class PropertyChain {
        /// <summary>
        ///   The type of object from which this <see cref="PropertyChain"/> can read values.
        /// </summary>
        public Type ReflectedType => chain_[0].ReflectedType!;

        /// <summary>
        ///   The type of values produced by this <see cref="PropertyChain"/>.
        /// </summary>
        public Type PropertyType => chain_[^1].PropertyType;

        /// <summary>
        ///   The length of this <see cref="PropertyChain"/>.
        /// </summary>
        public int Length => chain_.Count;

        /// <summary>
        ///   Construct a new <see cref="PropertyChain"/> instance from a particular property.
        /// </summary>
        /// <param name="firstProperty">
        ///   The property with which to initialize the new <see cref="PropertyChain"/>. This property may be public or
        ///   non-public, instance or <see langword="static"/>.
        /// </param>
        /// <exception cref="ArgumentException">
        ///   if <paramref name="firstProperty"/> is not readable.
        /// </exception>
        public PropertyChain(PropertyInfo firstProperty) {
            Guard.Against.Null(firstProperty, nameof(firstProperty));

            if (!firstProperty.CanRead) {
                throw new ArgumentException(BAD_PROPERTY_MSG, nameof(firstProperty));
            }

            chain_ = new List<PropertyInfo>() { firstProperty };
        }

        /// <summary>
        ///   Construct a new <see cref="PropertyChain"/> instance from a property reflected from a particular
        ///   <see cref="Type"/> by name.
        /// </summary>
        /// <param name="source">
        ///   The <see cref="Type"/> that will become the <see cref="ReflectedType"/> of the new
        ///   <see cref="PropertyChain"/>.
        /// </param>
        /// <param name="propertyName">
        ///   The name of the property with which to initialize the new <see cref="PropertyChain"/>.
        /// </param>
        /// <exception cref="ArgumentException">
        ///   if <paramref name="source"/> has no readable property named <paramref name="propertyName"/>.
        /// </exception>
        /// <remarks>
        ///   If <paramref name="source"/> inherits a declaration of a property named <paramref name="propertyName"/>
        ///   and hides it with the <c>new</c> keyword, the hiding property (rather than the hidden property) will be
        ///   used.
        /// </remarks>
        public PropertyChain(Type source, string propertyName) {
            Guard.Against.Null(source, nameof(source));
            Guard.Against.NullOrEmpty(propertyName, nameof(propertyName));

            chain_ = GetProperty(source, propertyName).Match(
                some: p => new List<PropertyInfo>() { p },
                none: () => {
                    var msg = $"No readable property '{propertyName}' found on {source.Name}";
                    throw new ArgumentException(msg, nameof(propertyName));
                }
            );
        }

        /// <summary>
        ///   Implicitly convert an instance of a <see cref="PropertyInfo"/> into a new <see cref="PropertyChain"/>.
        /// </summary>
        /// <param name="property">
        ///   The source <see cref="PropertyInfo"/>.
        /// </param>
        /// <seealso cref="PropertyChain(PropertyInfo)"/>
        public static implicit operator PropertyChain(PropertyInfo property) {
            Guard.Against.Null(property, nameof(property));
            return new PropertyChain(property);
        }

        /// <summary>
        ///   Create a new <see cref="PropertyChain"/> by appending a single property to the current chain.
        /// </summary>
        /// <param name="nextProperty">
        ///   The property to be appended.
        /// </param>
        /// <returns>
        ///   A new <see cref="PropertyChain"/> equivalent to the current chain plus <paramref name="nextProperty"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        ///   if <paramref name="nextProperty"/> is not readable
        ///     --or--
        ///   if <paramref name="nextProperty"/> is not applicable to the <see cref="PropertyType">type produced by the
        ///   current chain</see>.
        /// </exception>
        public PropertyChain Append(PropertyInfo nextProperty) {
            Guard.Against.Null(nextProperty, nameof(nextProperty));

            if (!nextProperty.CanRead) {
                throw new ArgumentException(BAD_PROPERTY_MSG, nameof(nextProperty));
            }
            else if (!PropertyType.IsInstanceOf(nextProperty.ReflectedType!)) {
                var msg = $"Cannot append property sourced from {nextProperty.ReflectedType!.Name} to chain that " +
                    $"produces an instance of {PropertyType.Name}";
                throw new ArgumentException(msg, nameof(nextProperty));
            }

            var newList = new List<PropertyInfo>(chain_) { nextProperty };
            return new PropertyChain(newList);
        }

        /// <summary>
        ///   Create a new <see cref="PropertyChain"/> by appending a single property to the current chain by name.
        /// </summary>
        /// <param name="nextPropertyName">
        ///   The name of the property to be appended. The property can be public or non-public, instance or 
        ///   <see langword="static"/>.
        /// </param>
        /// <returns>
        ///   A new <see cref="PropertyChain"/> equivalent to the current chain plus the property on the current chain's
        ///   <see cref="PropertyChain"/> named <paramref name="nextPropertyName"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        ///   if the current chain's <see cref="PropertyType"/> doest not have a readable property named
        ///   <paramref name="nextPropertyName"/>
        /// </exception>
        /// <remarks>
        ///   If the current chain's <see cref="PropertyType"/> inherits a declaration of a property named
        ///   <paramref name="nextPropertyName"/> and hides it with the <c>new</c> keyword, the hiding property
        ///   (rather than the hidden property) will be used.
        /// </remarks>
        public PropertyChain Append(string nextPropertyName) {
            Guard.Against.NullOrEmpty(nextPropertyName, nameof(nextPropertyName));

            var nextProperty = GetProperty(PropertyType, nextPropertyName).Match(
                some: p => p,
                none: () => {
                    var msg = $"No readable property '{nextPropertyName}' found on {PropertyType.Name}";
                    throw new ArgumentException(msg, nameof(nextPropertyName));
                }
            );

            // We're not going to use the Append overload that takes a PropertyInfo because the readability check is
            // already performed by GetProperty, and the IsInstanceOf check is implicit in the way that GetProperty is
            // being used
            var newList = new List<PropertyInfo>(chain_) { nextProperty };
            return new PropertyChain(newList);
        }

        /// <summary>
        ///   Create a new <see cref="PropertyChain"/> by appending an entire other <see cref="PropertyChain"/> to the
        ///   current one.
        /// </summary>
        /// <param name="nextChain">
        ///   The <see cref="PropertyChain"/> to be appended.
        /// </param>
        /// <returns>
        ///   A new <see cref="PropertyChain"/> equivalent to the current chain plus <paramref name="nextChain"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        ///   if the <see cref="ReflectedType"/> of <paramref name="nextChain"/> is not exactly the
        ///   <see cref="PropertyType"/> of the current chain or a base class or interface thereof.
        /// </exception>
        public PropertyChain Append(PropertyChain nextChain) {
            Guard.Against.Null(nextChain, nameof(nextChain));

            if (!PropertyType.IsInstanceOf(nextChain.ReflectedType)) {
                var msg = $"Cannot append property chain with {nameof(ReflectedType)} {nextChain.ReflectedType.Name} " +
                    $"to chain that produces an instance of {PropertyType.Name}";
                throw new ArgumentException(msg, nameof(nextChain));
            }

            var newList = new List<PropertyInfo>(chain_);
            newList.AddRange(nextChain.chain_);
            return new PropertyChain(newList);
        }

        /// <summary>
        ///   Create a new <see cref="PropertyChain"/> by prepending a single property to the current chain.
        /// </summary>
        /// <param name="prevProperty">
        ///   The property to be prepended.
        /// </param>
        /// <returns>
        ///   A new <see cref="PropertyChain"/> equivalent to the current chain with <paramref name="prevProperty"/>
        ///   as the new starting point.
        /// </returns>
        /// <exception cref="ArgumentException">
        ///   if <paramref name="prevProperty"/> is not readable
        ///     --or--
        ///   if the <see cref="PropertyInfo.PropertyType"/> of <paramref name="prevProperty"/> is not exactly the
        ///   <see cref="ReflectedType"/> of the current chain or a derivation or implementation thereof.
        /// </exception>
        public PropertyChain Prepend(PropertyInfo prevProperty) {
            Guard.Against.Null(prevProperty, nameof(prevProperty));

            if (!prevProperty.CanRead) {
                throw new ArgumentException(BAD_PROPERTY_MSG, nameof(prevProperty));
            }
            else if (!prevProperty.PropertyType.IsInstanceOf(ReflectedType)) {
                var msg = $"Cannot perpend property that produces an instance of {prevProperty.PropertyType.Name} " +
                    $"to chain that expects an instance of {ReflectedType.Name}";
                throw new ArgumentException(msg, nameof(prevProperty));
            }

            var newList = new List<PropertyInfo>(chain_);
            newList.Insert(0, prevProperty);
            return new PropertyChain(newList);
        }

        /// <summary>
        ///   Create a new <see cref="PropertyChain"/> by prepending an entire other <see cref="PropertyChain"/> to the
        ///   current one.
        /// </summary>
        /// <param name="prevChain">
        ///   The <see cref="PropertyChain"/> to be prepended.
        /// </param>
        /// <returns>
        ///   A new <see cref="PropertyChain"/> equivalent to the current chain with <paramref name="prevChain"/> as the
        ///   new starting subsequence.
        /// </returns>
        /// <exception cref="ArgumentException">
        ///   if the <see cref="ReflectedType"/> of the current chain is not exactly the <see cref="PropertyType"/>
        ///   of <paramref name="prevChain"/> or a base class or interface thereof.
        /// </exception>
        public PropertyChain Prepend(PropertyChain prevChain) {
            Guard.Against.Null(prevChain, nameof(prevChain));
            // We could just reuse Append here by calling prevChain.Append(this), but then the error message would be a
            // bit misleading (referring to an Append operation instead of a Prepend operation). So instead we'll just
            // perform a similar check and then create the new internal list ourselves.

            if (!prevChain.PropertyType.IsInstanceOf(ReflectedType)) {
                var msg = $"Cannot prepend property chain that produces an instance of {prevChain.PropertyType.Name} " +
                    $"to chain that expects an instance of {ReflectedType.Name}";
                throw new ArgumentException(msg, nameof(prevChain));
            }

            var newList = new List<PropertyInfo>(prevChain.chain_);
            newList.AddRange(chain_);
            return new PropertyChain(newList);
        }

        /// <summary>
        ///   Get the value of the current <see cref="PropertyChain"/> from a source object.
        /// </summary>
        /// <param name="source">
        ///   The source object.
        /// </param>
        /// <returns>
        ///   The value obtained by reading the value of the first property in the current <see cref="PropertyChain"/>
        ///   from <paramref name="source"/>, and then reading the value of the second property from the first-read
        ///   value, so on until the end of the chain, terminating at the first <see langword="null"/> if encountered.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   if <paramref name="source"/> is <see langword="null"/>
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   if <paramref name="source"/> is not an instance of the current
        ///   <see cref="PropertyChain">PropertyChain's</see> <see cref="ReflectedType"/>.
        /// </exception>"
        public object? GetValue(object? source) {
            if (source is null) {
                throw new ArgumentNullException(nameof(source));
            }
            else if (!ReflectedType.IsInstanceOfType(source)) {
                var msg = $"{nameof(PropertyChain)} cannot read from object of type {source.GetType().Name}, an " +
                    $"object of type {ReflectedType.Name} is expected";
                throw new ArgumentException(msg, nameof(source));
            }

            var lastValue = source;
            foreach (var prop in chain_) {
                lastValue = prop.GetValue(lastValue);
                if (lastValue is null) {
                    return null;
                }
            }
            return lastValue;
        }
        
        /// <summary>
        ///   Construct a new <see cref="PropertyChain"/> from a list of properties.
        /// </summary>
        /// <param name="chain">
        ///   The list of properties from which to construct a new <see cref="PropertyChain"/>.
        /// </param>
        /// <pre>
        ///   <paramref name="chain"/> is not empty
        ///     --and--
        ///   <paramref name="chain"/> constitutes a valid chain of properties; that is, the
        ///   <see cref="PropertyInfo.PropertyType"/> of element <c>N</c> is either exactly the
        ///   <see cref="MemberInfo.ReflectedType"/> of element <c>N + 1</c> or is a base class or interface thereof.
        /// </pre>
        private PropertyChain(List<PropertyInfo> chain) {
            Debug.Assert(chain is not null);
            Debug.Assert(!chain.IsEmpty());

            // We could run a Debug.Assert to make sure that the argument chain is valid, but I think that the
            // invariants enforced by other methods will cover us here
            chain_ = chain;
        }

        /// <summary>
        ///   Attempt to get a property from a type by name via reflection.
        /// </summary>
        /// <param name="source">
        ///   The <see cref="Type"/> from which to extract a property.
        /// </param>
        /// <param name="name">
        ///   The name of the property to extract.
        /// </param>
        /// <returns>
        ///   The readable property (public or non-public, instance or <see langword="static"/>) accessible from
        ///   <paramref name="source"/> named <paramref name="name"/>. If <paramref name="source"/> hides a property
        ///   with that name, the hiding property will be returned. If no such property exists, returns a <c>NONE</c>
        ///   instance.
        /// </returns>
        private static Option<PropertyInfo> GetProperty(Type source, string name) {
            // Here's why we have to use the branchy helper: GetProperty will throw an AmbiguousMatchException if the
            // source type inherits a property and then hides it with the "new" keyword. However, we want to support
            // this by finding the hiding property. We can't key on the DeclaringType up front, because that would
            // wrongly filter out inherited properties, so we have to first winnow the set of all properties down to
            // those matching the specified name and then check our three cases: none found (return null), exactly one
            // found (return it), or multiple found (return the one declared by our source). Note that we could also do
            // a "try" of GetProperty followed by a "catch" on the AmbiguousMatchException that does a GetProperties,
            // but using exceptions for control flow is bad practice.
            PropertyInfo? lookupProperty() {
                var properties = source.GetProperties(ANY_PROPERTY).Where(p => p.Name == name).ToArray();
                if (properties.Length == 0) {
                    return null;
                }
                else if (properties.Length == 1) {
                    return properties[0];
                }
                else {
                    return properties.Single(p => p.DeclaringType == source);
                }
            }

            var prop = lookupProperty();
            if (prop is null || !prop.CanRead) {
                return Option.None<PropertyInfo>();
            }
            return Option.Some(prop);
        }


        private readonly List<PropertyInfo> chain_;

        private const string BAD_PROPERTY_MSG = $"All properties in a {nameof(PropertyChain)} must be readable";
        private const BindingFlags ANY_PROPERTY =
            BindingFlags.Public     |        // list public properties
            BindingFlags.NonPublic  |        // list private, protected, internal, and internal protected properties
            BindingFlags.Static     |        // list static properties
            BindingFlags.Instance   |        // list non-static (instance) properties
            BindingFlags.FlattenHierarchy;   // list properties that are declared by base classes and interfaces
    }
}
