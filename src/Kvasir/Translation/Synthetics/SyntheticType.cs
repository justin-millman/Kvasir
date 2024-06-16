﻿using Kvasir.Annotations;
using Kvasir.Relations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;

using PropertyGenerator = System.Func<
    Kvasir.Translation.SyntheticType,
    System.Collections.Generic.IEnumerable<Kvasir.Translation.SyntheticPropertyInfo>
>;

namespace Kvasir.Translation {
    /// <summary>
    ///   The reflection representation that models the element of a Relation.
    /// </summary>
    internal sealed partial class SyntheticType : Type {
        /// <inheritdoc/>
        public sealed override Assembly Assembly { get; }

        /// <inheritdoc/>
        public sealed override Type? BaseType => null;

        /// <inheritdoc/>
        public sealed override string? FullName => $"{Namespace}.{Name}";

        /// <inheritdoc/>
        public sealed override string Name { get; }

        /// <inheritdoc/>
        public sealed override string? Namespace { get; }

        /// <inheritdoc/>
        public sealed override Type UnderlyingSystemType => this;

        /// <summary>
        ///   Constructs a new <see cref="SyntheticType"/>.
        /// </summary>
        /// <param name="name">
        ///   The <see cref="Name"/> of the type.
        /// </param>
        /// <param name="ns">
        ///   The <see cref="Namespace"/> of the type.
        /// </param>
        /// <param name="assmebly">
        ///   The <see cref="Assembly"/> of the type.
        /// </param>
        /// <param name="properties">
        ///   A function that, when provided <c>this</c> as an argument, produces an enumerable of the type's
        ///   properties. (This is a function because of the circular definition: the SyntheticType needs to know its
        ///   properties, and each SyntheticProperty needs to know its owning SyntheticType.)
        /// </param>
        /// <seealso cref="MakeSyntheticType(Type, RelationTracker)"/>
        private SyntheticType(string name, string ns, Assembly assmebly, PropertyGenerator properties) {
            Debug.Assert(name is not null && name != "");
            Debug.Assert(ns is not null && ns != "");
            Debug.Assert(assmebly is not null);
            Debug.Assert(properties is not null);

            Name = name;
            Namespace = ns;
            Assembly = assmebly;
            properties_ = properties(this).ToList();

            Debug.Assert(properties_.Count >= 2);
        }

        /// <summary>
        ///   Creates a new <see cref="SyntheticType"/>.
        /// </summary>
        /// <param name="entity">
        ///   The type of the owning Entity, which defines the first property on the new <see cref="SyntheticType"/>.
        /// </param>
        /// <param name="tracker">
        ///   The <see cref="RelationTracker"/> containing the metadata for the property that forms the rest of the
        ///   <see cref="SyntheticType"/>.
        /// </param>
        public static SyntheticType MakeSyntheticType(Type entity, RelationTracker tracker) {
            Debug.Assert(entity is not null && entity.IsClass);
            Debug.Assert(tracker is not null);

            var flags = BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;
            var relationType = tracker.Property.PropertyType;
            var elementType = (Type)relationType.GetProperties(flags)[0]!.GetValue(null)!;
            var nullability = new NullabilityInfoContext().Create(tracker.Property);
            var interfaces = relationType.GetInterfaces().Select(i => i.IsGenericType ? i.GetGenericTypeDefinition() : null);

            if (relationType.IsInterface && relationType.IsGenericType) {
                interfaces = interfaces.Append(relationType.GetGenericTypeDefinition());
            }

            // We will build up the properties' metadata so that we can then write a single constructor call for the
            // SyntheticType, where we'll leverage LINQ to transform the metadata elements into SyntheticProperty
            // instances
            var props = new List<(string Name, Type Type, bool Nullable, bool Unique)>();

            if (interfaces.Contains(typeof(IReadOnlyRelationList<>))) {
                // Entity
                props.Add((entity.Name, entity, false, false));
                // Item
                var itemNullable = nullability.GenericTypeArguments[0].ReadState == NullabilityState.Nullable;
                props.Add(("Item", elementType, itemNullable, false));
            }
            else if (interfaces.Contains(typeof(IReadOnlySet<>))) {
                // Entity
                props.Add((entity.Name, entity, false, true));
                // Item
                var itemNullable = nullability.GenericTypeArguments[0].ReadState == NullabilityState.Nullable;
                props.Add(("Item", elementType, itemNullable, true));
            }
            else if (interfaces.Contains(typeof(IReadOnlyRelationOrderedList<>))) {
                // Entity
                props.Add((entity.Name, entity, false, true));
                // Index
                var indexType = elementType.GetProperty("Key", BindingFlags.Public | BindingFlags.Instance)!.PropertyType;
                props.Add(("Index", indexType, false, true));
                // Item
                var itemNullable = nullability.GenericTypeArguments[0].ReadState == NullabilityState.Nullable;
                var itemType = elementType.GetProperty("Value", BindingFlags.Public | BindingFlags.Instance)!.PropertyType;
                props.Add(("Item", itemType, itemNullable, false));
            }
            else if (interfaces.Contains(typeof(IReadOnlyRelationMap<,>))) {
                // Entity
                props.Add((entity.Name, entity, false, true));
                // Index
                var keyNullable = nullability.GenericTypeArguments[0].ReadState == NullabilityState.Nullable;
                var keyType = elementType.GetProperty("Key", BindingFlags.Public | BindingFlags.Instance)!.PropertyType;
                props.Add(("Key", keyType, keyNullable, true));
                // Item
                var valueNullable = nullability.GenericTypeArguments[1].ReadState == NullabilityState.Nullable;
                var valueType = elementType.GetProperty("Value", BindingFlags.Public | BindingFlags.Instance)!.PropertyType;
                props.Add(("Value", valueType, valueNullable, false));
            }
            else {
                throw new ApplicationException($"If block over {nameof(IRelation)} interfaces exhausted");
            }

            // These shenanigans are necessary to make sure that contextualized error messages display a string that is
            // consistent with that displayed for non-relation properties. In particular, the leading and trailing back
            // ticks are omitted because they will be added by the Context class, but the interior ones are explicitly
            // included.
            var nameParts = tracker.Path.Split('.');
            var first = tracker.Property.ReflectedType!.Name + '`';
            var middle = nameParts.Skip(1).SkipLast(1).Select(n => $"`{n}`");
            var last = "<synthetic> `" + nameParts[^1];
            var name = string.Join(" → ", Enumerable.Repeat(first, 1).Concat(middle).Append(last));

            return new SyntheticType(
                name: name,
                ns: entity.Namespace!,
                assmebly: entity.Assembly,
                properties: t => props.Select(p => {
                    IEnumerable<Attribute> annotations = tracker.AnnotationsFor(p.Name);
                    if (p.Nullable) {
                        annotations = annotations.Append(new NullableAttribute());
                    }
                    if (p.Unique) {
                        annotations = annotations.Append(new UniqueAttribute('\0'));
                    }
                    return new SyntheticPropertyInfo(p.Name, t, p.Type, annotations);
                })
            );
        }

        /// <inheritdoc/>
        public sealed override object[] GetCustomAttributes(bool inherit) {
            return Array.Empty<object>();
        }

        /// <inheritdoc/>
        public sealed override object[] GetCustomAttributes(Type attributeType, bool inherit) {
            return Array.Empty<object>();
        }

        /// <inheritdoc/>
        public sealed override IList<CustomAttributeData> GetCustomAttributesData() {
            return new List<CustomAttributeData>();
        }

        /// <inheritdoc/>
        public sealed override Type[] GetInterfaces() {
            return Array.Empty<Type>();
        }

        /// <inheritdoc/>
        public sealed override PropertyInfo[] GetProperties(BindingFlags bindingAttr) {
            Debug.Assert(bindingAttr.HasFlag(BindingFlags.Public) && bindingAttr.HasFlag(BindingFlags.NonPublic));
            Debug.Assert(bindingAttr.HasFlag(BindingFlags.Instance) && bindingAttr.HasFlag(BindingFlags.Static));

            return properties_.ToArray();
        }

        /// <inheritdoc/>
        protected sealed override TypeAttributes GetAttributeFlagsImpl() {
            return TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.Sealed;
        }

        /// <inheritdoc/>
        protected sealed override PropertyInfo? GetPropertyImpl(string name, BindingFlags bindingAttr, Binder? binder,
            Type? returnType, Type[]? types, ParameterModifier[]? modifiers) {

            Debug.Assert(name is not null && name != "");
            Debug.Assert(bindingAttr.HasFlag(BindingFlags.Public) && bindingAttr.HasFlag(BindingFlags.NonPublic));
            Debug.Assert(bindingAttr.HasFlag(BindingFlags.Instance) && bindingAttr.HasFlag(BindingFlags.Static));

            return properties_.FirstOrDefault(p => p.Name == name);
        }

        /// <inheritdoc/>
        protected sealed override bool IsPrimitiveImpl() {
            return false;
        }


        private readonly IReadOnlyList<SyntheticPropertyInfo> properties_;
    }

    // The functions implemented in this partial definition unconditionally throw NotSupportedExceptions, as they are
    // not needed by Kvasir at all but are necessary parts of the full abstract API of the Type base class
    internal sealed partial class SyntheticType : Type {
        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public sealed override string? AssemblyQualifiedName {
            get {
                throw new NotSupportedException($"{nameof(SyntheticType)}.{nameof(AssemblyQualifiedName)}");
            }
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public sealed override Guid GUID {
            get {
                throw new NotSupportedException($"{nameof(SyntheticType)}.{nameof(GUID)}");
            }
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public sealed override Module Module {
            get {
                throw new NotSupportedException($"{nameof(SyntheticType)}.{nameof(Module)}");
            }
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public sealed override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr) {
            throw new NotSupportedException($"{nameof(SyntheticType)}.{nameof(GetConstructors)}");
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public sealed override Type? GetElementType() {
            throw new NotSupportedException($"{nameof(SyntheticType)}.{nameof(GetElementType)}");
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public sealed override EventInfo? GetEvent(string name, BindingFlags bindingAttr) {
            throw new NotSupportedException($"{nameof(SyntheticType)}.{nameof(GetEvent)}");
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public sealed override EventInfo[] GetEvents(BindingFlags bindingAttr) {
            throw new NotSupportedException($"{nameof(SyntheticType)}.{nameof(GetEvents)}");
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public sealed override FieldInfo? GetField(string name, BindingFlags bindingAttr) {
            throw new NotSupportedException($"{nameof(SyntheticType)}.{nameof(GetField)}");
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public sealed override FieldInfo[] GetFields(BindingFlags bindingAttr) {
            throw new NotSupportedException($"{nameof(SyntheticType)}.{nameof(GetFields)}");
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public sealed override Type? GetInterface(string name, bool ignoreCase) {
            throw new NotSupportedException($"{nameof(SyntheticType)}.{nameof(GetInterface)}");
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public sealed override MemberInfo[] GetMembers(BindingFlags bindingAttr) {
            throw new NotSupportedException($"{nameof(SyntheticType)}.{nameof(GetMembers)}");
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public sealed override MethodInfo[] GetMethods(BindingFlags bindingAttr) {
            throw new NotSupportedException($"{nameof(SyntheticType)}.{nameof(GetMethods)}");
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public sealed override Type? GetNestedType(string name, BindingFlags bindingAttr) {
            throw new NotSupportedException($"{nameof(SyntheticType)}.{nameof(GetNestedType)}");
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public sealed override Type[] GetNestedTypes(BindingFlags bindingAttr) {
            throw new NotSupportedException($"{nameof(SyntheticType)}.{nameof(GetNestedTypes)}");
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public sealed override bool IsDefined(Type attributeType, bool inherit) {
            throw new NotSupportedException($"{nameof(SyntheticType)}.{nameof(IsDefined)}");
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public sealed override object? InvokeMember(string name, BindingFlags invokeAttr, Binder? binder,
            object? target, object?[]? args, ParameterModifier[]? modifiers, CultureInfo? culture,
            string[]? namedParameters) {

            throw new NotSupportedException($"{nameof(SyntheticType)}.{nameof(InvokeMember)}");
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        protected sealed override ConstructorInfo? GetConstructorImpl(BindingFlags bindingAttr, Binder? binder,
            CallingConventions callConvention, Type[] types, ParameterModifier[]? modifiers) {

            throw new NotSupportedException($"{nameof(SyntheticType)}.{nameof(GetConstructorImpl)}");
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        protected sealed override MethodInfo? GetMethodImpl(string name, BindingFlags bindingAttr, Binder? binder,
            CallingConventions callConvention, Type[]? types, ParameterModifier[]? modifiers) {

            throw new NotSupportedException($"{nameof(SyntheticType)}.{nameof(GetMethodImpl)}");
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        protected sealed override bool HasElementTypeImpl() {
            throw new NotSupportedException($"{nameof(SyntheticType)}.{nameof(HasElementTypeImpl)}");
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        protected sealed override bool IsArrayImpl() {
            throw new NotSupportedException($"{nameof(SyntheticType)}.{nameof(IsArrayImpl)}");
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        protected sealed override bool IsByRefImpl() {
            throw new NotSupportedException($"{nameof(SyntheticType)}.{nameof(IsByRefImpl)}");
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        protected sealed override bool IsCOMObjectImpl() {
            throw new NotSupportedException($"{nameof(SyntheticType)}.{nameof(IsCOMObjectImpl)}");
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        protected sealed override bool IsPointerImpl() {
            throw new NotSupportedException($"{nameof(SyntheticType)}.{nameof(IsPointerImpl)}");
        }
    }
}