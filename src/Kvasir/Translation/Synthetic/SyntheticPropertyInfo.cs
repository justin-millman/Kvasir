using Cybele.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Kvasir.Translation.Synthetic {
    /// <summary>
    ///   A reflection representation of a property on a <see cref="SyntheticType"/>.
    /// </summary>
    internal sealed partial class SyntheticPropertyInfo : PropertyInfo {
        /// <inheritdoc/>
        public sealed override Type? DeclaringType => ReflectedType;

        /// <inheritdoc/>
        public sealed override string Name { get; }

        /// <inheritdoc/>
        public sealed override Type PropertyType { get; }

        /// <inheritdoc/>
        public sealed override Type? ReflectedType { get; }

        /// <summary>
        ///   Constructs a new instance of the <see cref="SyntheticPropertyInfo"/> class.
        /// </summary>
        /// <param name="name">
        ///   The <see cref="Name"/> of the property represented by the new <see cref="SyntheticPropertyInfo"/>.
        /// </param>
        /// <param name="source">
        ///   The <see cref="SyntheticType"/> to which the property represented by the new
        ///   <see cref="SyntheticPropertyInfo"/> belongs.
        /// </param>
        /// <param name="propertyType">
        ///   The <see cref="Type"/> of the property represented by the new <see cref="SyntheticPropertyInfo"/>.
        /// </param>
        /// <param name="attributes">
        ///   The collection of <see cref="Attribute">Attributes</see> that have been applied to the property
        ///   represented by the new <see cref="SyntheticPropertyInfo"/>.
        /// </param>
        public SyntheticPropertyInfo(string name, SyntheticType source, Type propertyType, IEnumerable<Attribute> attributes) {
            Debug.Assert(name is not null && name != "");
            Debug.Assert(source is not null);
            Debug.Assert(propertyType is not null);
            Debug.Assert(attributes is not null);

            Name = name;
            ReflectedType = source;
            PropertyType = propertyType;
            attributes_ = new List<Attribute>(attributes);
            getter_ = new SyntheticMethodInfo(this, propertyType);
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
        public sealed override MethodInfo? GetGetMethod(bool nonPublic) {
            return getter_;
        }

        /// <inheritdoc/>
        public sealed override ParameterInfo[] GetIndexParameters() {
            return Array.Empty<ParameterInfo>();
        }

        /// <inheritdoc/>
        public sealed override MethodInfo? GetSetMethod(bool nonPublic) {
            return null;
        }


        private readonly IReadOnlyList<Attribute> attributes_;
        private readonly SyntheticMethodInfo getter_;
    }

    // The functions implemented in this partial definition unconditionally throw NotSupportedExceptions, as they are
    // not needed by Kvasir at all but are necessary parts of the full abstract API of the MethodInfo base class
    internal sealed partial class SyntheticPropertyInfo : PropertyInfo {
        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public sealed override PropertyAttributes Attributes {
            get {
                throw new NotSupportedException($"{nameof(SyntheticPropertyInfo)}.{nameof(Attributes)}");
            }
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public sealed override bool CanRead {
            get {
                throw new NotSupportedException($"{nameof(SyntheticPropertyInfo)}.{nameof(CanRead)}");
            }
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public sealed override bool CanWrite {
            get {
                throw new NotSupportedException($"{nameof(SyntheticPropertyInfo)}.{nameof(CanWrite)}");
            }
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public sealed override MethodInfo[] GetAccessors(bool nonPublic) {
            throw new NotSupportedException($"{nameof(SyntheticPropertyInfo)}.{nameof(GetAccessors)}");
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public sealed override object? GetValue(object? obj, BindingFlags invokeAttr, Binder? binder, object?[]? index,
            CultureInfo? culture) {

            throw new NotSupportedException($"{nameof(SyntheticPropertyInfo)}.{nameof(GetValue)}");
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public sealed override bool IsDefined(Type attributeType, bool inherit) {
            throw new NotSupportedException($"{nameof(SyntheticPropertyInfo)}.{nameof(IsDefined)}");
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public sealed override void SetValue(object? obj, object? value, BindingFlags invokeAttr, Binder? binder,
            object?[]? index, CultureInfo? culture) {

            throw new NotSupportedException($"{nameof(SyntheticPropertyInfo)}.{nameof(SetValue)}");
        }
    }
}
