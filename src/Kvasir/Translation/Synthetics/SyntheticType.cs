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
        /// <summary>
        ///   The <see cref="Type"/> that the <see cref="SyntheticType"/> is masquerading as.
        /// </summary>
        public Type ActualType { get; }

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
        /// <param name="actualType">
        ///   The <see cref="Type"/> that the new <see cref="SyntheticType"/> is a façade for.
        /// </param>
        /// <seealso cref="MakeSyntheticType(Type, RelationTracker)"/>
        private SyntheticType(string name, string ns, Assembly assmebly, PropertyGenerator properties, Type actualType) {
            Debug.Assert(name is not null && name != "");
            Debug.Assert(ns is not null && ns != "");
            Debug.Assert(assmebly is not null);
            Debug.Assert(properties is not null);
            Debug.Assert(actualType is not null && actualType is not SyntheticType);

            Name = name;
            Namespace = ns;
            ActualType = actualType;
            Assembly = assmebly;
            properties_ = properties(this).ToList();

            // We want to be able to construct a SyntheticType, conceptually, from just the element; the first property
            // is always that of the owning Entity
            constructors_ = new ConstructorInfo[] { new SyntheticConstructorInfo(this, properties_.Skip(1)) };

            Debug.Assert(properties_.Count >= 2);
        }

        /// <inheritdoc/>
        public sealed override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr) {
            return constructors_;
        }

        /// <inheritdoc/>
        public sealed override bool Equals(Type? rhs) {
            if (rhs == ActualType) {
                return true;
            }
            else {
                return base.Equals(rhs);
            }
        }

        /// <inheritdoc/>
        protected sealed override bool IsArrayImpl() {
            return false;
        }

        /// <inheritdoc/>
        protected sealed override bool IsByRefImpl() {
            return false;
        }

        /// <inheritdoc/>
        protected sealed override bool IsPointerImpl() {
            return false;
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
            var props = new List<(string Name, Type Type, Metadata Flags)>();

            // Just a helper function to go from the internal Roslyn nullability APIs to the `Metadata` flags
            Metadata nullabilityOf(int index) {
                var flag = nullability.GenericTypeArguments[index].ReadState;
                return flag == NullabilityState.Nullable ? Metadata.Nullable : Metadata.None;
            }

            if (interfaces.Contains(typeof(IReadOnlyRelationList<>))) {
                // Entity
                props.Add((entity.Name, entity, Metadata.ColumnZero));
                // Item
                var itemNullable = nullabilityOf(0);
                props.Add(("Item", elementType, itemNullable));
            }
            else if (interfaces.Contains(typeof(IReadOnlySet<>))) {
                // Entity
                props.Add((entity.Name, entity, Metadata.ColumnZero | Metadata.Unique));
                // Item
                var itemNullable = nullabilityOf(0);
                props.Add(("Item", elementType, itemNullable | Metadata.Unique));
            }
            else if (interfaces.Contains(typeof(IReadOnlyRelationOrderedList<>))) {
                // Entity
                props.Add((entity.Name, entity, Metadata.ColumnZero | Metadata.Unique));
                // Index
                var indexType = elementType.GetProperty("Key", BindingFlags.Public | BindingFlags.Instance)!.PropertyType;
                props.Add(("Index", indexType, Metadata.Unique));
                // Item
                var itemNullable = nullabilityOf(0);
                var itemType = elementType.GetProperty("Value", BindingFlags.Public | BindingFlags.Instance)!.PropertyType;
                props.Add(("Item", itemType, itemNullable));
            }
            else if (interfaces.Contains(typeof(IReadOnlyRelationMap<,>))) {
                // Entity
                props.Add((entity.Name, entity, Metadata.ColumnZero | Metadata.Unique));
                // Index
                var keyNullable = nullabilityOf(0);
                var keyType = elementType.GetProperty("Key", BindingFlags.Public | BindingFlags.Instance)!.PropertyType;
                props.Add(("Key", keyType, keyNullable | Metadata.Unique));
                // Item
                var valueNullable = nullabilityOf(1);
                var valueType = elementType.GetProperty("Value", BindingFlags.Public | BindingFlags.Instance)!.PropertyType;
                props.Add(("Value", valueType, valueNullable));
            }
            else {
                throw new UnreachableException($"If block over {nameof(IRelation)} interfaces exhausted");
            }

            // These shenanigans are necessary to make sure that contextualized error messages display a string that is
            // consistent with that displayed for non-relation properties. In particular, the leading and trailing back
            // ticks are stripped because they will be added by the Context class, but the interior ones are kept.
            var name = tracker.GetSyntheticTypenameOn(entity);
            name = name[1..^1];

            return new SyntheticType(
                name: name,
                ns: entity.Namespace!,
                assmebly: entity.Assembly,
                properties: t => props.Select(p => {
                    IEnumerable<Attribute> annotations = tracker.AnnotationsFor(p.Name);
                    if (p.Flags.HasFlag(Metadata.Nullable)) {
                        annotations = annotations.Append(new NullableAttribute());
                    }
                    if (p.Flags.HasFlag(Metadata.Unique)) {
                        annotations = annotations.Append(new UniqueAttribute('\0'));
                    }
                    if (p.Flags.HasFlag(Metadata.ColumnZero)) {
                        annotations = annotations.Append(new ColumnAttribute(0));
                    }
                    return new SyntheticPropertyInfo(p.Name, t, p.Type, annotations);
                }),
                actualType: elementType
            );
        }

        /// <inheritdoc/>
        public sealed override object[] GetCustomAttributes(Type attributeType, bool inherit) {
            var result = Array.CreateInstance(typeof(Attribute), 0);
            return (object[])result;
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


        private readonly IReadOnlyList<SyntheticPropertyInfo> properties_;
        private readonly ConstructorInfo[] constructors_;
        [Flags] private enum Metadata { None = 0, Nullable = 1, Unique = 2, ColumnZero = 4 }
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
        public sealed override object[] GetCustomAttributes(bool inherit) {
            throw new NotSupportedException($"{nameof(SyntheticType)}.{nameof(GetCustomAttributes)}");
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
        protected sealed override PropertyInfo? GetPropertyImpl(string name, BindingFlags bindingAttr, Binder? binder,
            Type? returnType, Type[]? types, ParameterModifier[]? modifiers) {

            throw new NotSupportedException($"{nameof(SyntheticType)}.{nameof(GetPropertyImpl)}");
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        protected sealed override bool HasElementTypeImpl() {
            throw new NotSupportedException($"{nameof(SyntheticType)}.{nameof(HasElementTypeImpl)}");
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        protected sealed override bool IsCOMObjectImpl() {
            throw new NotSupportedException($"{nameof(SyntheticType)}.{nameof(IsCOMObjectImpl)}");
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        protected sealed override bool IsPrimitiveImpl() {
            throw new NotSupportedException($"{nameof(SyntheticType)}.{nameof(IsPrimitiveImpl)}");
        }
    }
}
