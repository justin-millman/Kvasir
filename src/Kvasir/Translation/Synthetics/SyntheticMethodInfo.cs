using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;

namespace Kvasir.Translation {
    /// <summary>
    ///   The reflection representation of the <c>GET</c> method of a <see cref="SyntheticPropertyInfo">Synthetic
    ///   Property</see>.
    /// </summary>
    internal sealed partial class SyntheticMethodInfo : MethodInfo {
        /// <inheritdoc/>
        public sealed override MethodAttributes Attributes => MethodAttributes.Public;

        /// <inheritdoc/>
        public sealed override Type? DeclaringType { get; }

        /// <inheritdoc/>
        public sealed override Type? ReflectedType => DeclaringType;

        /// <inheritdoc/>
        public sealed override ParameterInfo ReturnParameter { get; }

        /// <inheritdoc/>
        public sealed override Type ReturnType { get; }

        /// <summary>
        ///   Constructs a new <see cref="SyntheticMethodInfo"/>.
        /// </summary>
        /// <param name="source">
        ///   The <see cref="SyntheticPropertyInfo">property</see> whose <c>GET</c> method is modeled by the new
        ///   <see cref="SyntheticMethodInfo"/>.
        /// </param>
        public SyntheticMethodInfo(SyntheticPropertyInfo source) {
            Debug.Assert(source is not null);

            DeclaringType = source.DeclaringType;
            ReturnParameter = new SyntheticReturnInfo();
            ReturnType = source.PropertyType;
        }

        /// <inheritdoc/>
        public sealed override MethodInfo GetBaseDefinition() {
            return this;
        }
    }

    // The functions implemented in this partial definition unconditionally throw NotSupportedExceptions, as they are
    // not needed by Kvasir at all but are necessary parts of the full abstract API of the MethodInfo base class
    internal sealed partial class SyntheticMethodInfo : MethodInfo {
        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public sealed override RuntimeMethodHandle MethodHandle {
            get {
                throw new NotSupportedException($"{nameof(SyntheticMethodInfo)}.{nameof(MethodHandle)}");
            }
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public sealed override string Name {
            get {
                throw new NotSupportedException($"{nameof(SyntheticMethodInfo)}.{nameof(Name)}");
            }
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public sealed override ICustomAttributeProvider ReturnTypeCustomAttributes {
            get {
                throw new NotSupportedException($"{nameof(SyntheticMethodInfo)}.{nameof(ReturnTypeCustomAttributes)}");
            }
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public sealed override IList<CustomAttributeData> GetCustomAttributesData() {
            throw new NotSupportedException($"{nameof(SyntheticMethodInfo)}.{nameof(GetCustomAttributesData)}");
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public sealed override MethodImplAttributes GetMethodImplementationFlags() {
            throw new NotSupportedException($"{nameof(SyntheticMethodInfo)}.{nameof(GetMethodImplementationFlags)}");
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public sealed override object[] GetCustomAttributes(bool inherit) {
            throw new NotSupportedException($"{nameof(SyntheticMethodInfo)}.{nameof(GetCustomAttributes)}");
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public sealed override object[] GetCustomAttributes(Type attributeType, bool inherit) {
            throw new NotSupportedException($"{nameof(SyntheticMethodInfo)}.{nameof(GetCustomAttributes)}");
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public sealed override ParameterInfo[] GetParameters() {
            throw new NotSupportedException($"{nameof(SyntheticMethodInfo)}.{nameof(GetParameters)}");
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public sealed override object? Invoke(object? obj, BindingFlags invokeAttr, Binder? binder,
            object?[]? parameters, CultureInfo? culture) {

            throw new NotSupportedException($"{nameof(SyntheticMethodInfo)}.{nameof(Invoke)}");
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public sealed override bool IsDefined(Type attributeType, bool inherit) {
            throw new NotSupportedException($"{nameof(SyntheticMethodInfo)}.{nameof(IsDefined)}");
        }
    }
}
