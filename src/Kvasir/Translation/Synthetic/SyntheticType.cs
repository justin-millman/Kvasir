using Cybele.Extensions;
using Kvasir.Annotations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Kvasir.Translation.Synthetic {
    /// <summary>
    ///   A reflection representation of the "type" that models the hypothetical Entity of a Relation Table.
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
        ///   Constructs a new instance of the <see cref="SyntheticType"/> class.
        /// </summary>
        /// <param name="descriptor">
        ///   The <see cref="IRelationDescriptor"/> that describes the structure of the new <see cref="SyntheticType"/>.
        /// </param>
        /// <param name="owningEntity">
        ///   The <see cref="Type"/> of the Entity on which the Relation was originally defined.
        /// </param>
        public SyntheticType(IRelationDescriptor descriptor, Type owningEntity) {
            Debug.Assert(descriptor.Attributes.Count >= 2);
            Debug.Assert(descriptor.Attributes.ContainsKey(owningEntity.Name));
            Debug.Assert(owningEntity is not null);

            var simpleName = string.Join(".", descriptor.Name);
            Assembly = owningEntity.Assembly;
            Namespace = owningEntity.Namespace;
            Name = $"<synthetic>::{owningEntity.Name}.{simpleName}";
            properties_ = descriptor.FieldTypes.Select(
                kvp => new SyntheticPropertyInfo(kvp.Key, this, kvp.Value, descriptor.Attributes[kvp.Key])
            ).ToList();

            var attributes = new List<Attribute>();
            attributes_ = attributes;
            attributes.Add(new TableAttribute(descriptor.TableName.ValueOr($"{owningEntity.FullName}.{simpleName}Table")));
            if (owningEntity.HasAttribute<ExcludeNamespaceFromNameAttribute>()) {
                attributes.Add(new ExcludeNamespaceFromNameAttribute());
            }
        }

        /// <inheritdoc/>
        public sealed override object[] GetCustomAttributes(bool inherit) {
            return attributes_.ToArray();
        }

        /// <inheritdoc/>
        public sealed override object[] GetCustomAttributes(Type attributeType, bool inherit) {
            var matches = attributes_.Where(a => a.GetType().IsInstanceOf(attributeType)).ToArray();
            var result = Array.CreateInstance(attributeType, matches.Length);
            matches.CopyTo(result, 0);
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


        private readonly IReadOnlyList<Attribute> attributes_;
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
