using Cybele.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Kvasir.Translation2 {
    /// <summary>
    ///   The reflection representation of a property on a <see cref="SyntheticType"/>.
    /// </summary>
    internal sealed partial class SyntheticPropertyInfo : PropertyInfo {
        /// <inheritdoc/>
        public sealed override Type? DeclaringType { get; }

        /// <inheritdoc/>
        public sealed override string Name { get; }

        /// <inheritdoc/>
        public sealed override Type PropertyType { get; }

        /// <inheritdoc/>
        public sealed override Type? ReflectedType => DeclaringType;

        /// <summary>
        ///   Constructs a new <see cref="SyntheticPropertyInfo"/>.
        /// </summary>
        /// <param name="name">
        ///   The <see cref="Name"/> of the property.
        /// </param>
        /// <param name="source">
        ///   The <see cref="SyntheticType"/> on which the property resides.
        /// </param>
        /// <param name="propertyType">
        ///   The <see cref="PropertyType">type</see> of the property.
        /// </param>
        /// <param name="annotations">
        ///   The set of <see cref="Attribute">annotations</see> applied to the property, in any order.
        /// </param>
        public SyntheticPropertyInfo(string name, SyntheticType source, Type propertyType, IEnumerable<Attribute> annotations) {
            Debug.Assert(name is not null && name != "");
            Debug.Assert(source is not null);
            Debug.Assert(propertyType is not null);
            Debug.Assert(annotations is not null);

            DeclaringType = source;
            Name = name;
            PropertyType = propertyType;
            getter_ = new SyntheticMethodInfo(this);
            annotations_ = annotations.ToList();
        }

        /// <inheritdoc/>
        public sealed override object[] GetCustomAttributes(bool inherit) {
            return annotations_.ToArray();
        }

        /// <inheritdoc/>
        public sealed override object[] GetCustomAttributes(Type attributeType, bool inherit) {
            // This is caused by the inability to cast between an array of `T` and an Enumerable of `D` where `D`
            // derives from `T`. To be able to work within the reflection system, we have to return an array of objects
            // that are somehow internally recognizes as being of `attributeType`. To do this, we have to create an
            // array instance and copy the results into it, then cast it to an `object[]`. That's just the rules, here.
            var matches = annotations_.Where(a => a.GetType().IsInstanceOf(attributeType)).ToArray();
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


        private readonly SyntheticMethodInfo getter_;
        private readonly IReadOnlyList<Attribute> annotations_;
    }

    // The functions implemented in this partial definition unconditionally throw NotSupportedExceptions, as they are
    // not needed by Kvasir at all but are necessary parts of the full abstract API of the PropertyInfo base class
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