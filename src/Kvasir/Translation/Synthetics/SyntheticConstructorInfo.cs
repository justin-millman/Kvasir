using Kvasir.Schema;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Kvasir.Translation {
    /// <summary>
    ///   The reflection representation of the constructor of a <see cref="SyntheticType"/>, or of the default
    ///   constructor present on all structs.
    /// </summary>
    internal sealed partial class SyntheticConstructorInfo : ConstructorInfo {
        /// <inheritdoc/>
        public override MethodAttributes Attributes => MethodAttributes.Public;

        /// <inheritdoc/>
        public override Type? DeclaringType { get; }

        /// <inheritdoc/>
        public override Type? ReflectedType => DeclaringType;

        /// <inheritdoc/>
        public override object[] GetCustomAttributes(Type attributeType, bool inherit) {
            // This is caused by the inability to cast between an array of `T` and an Enumerable of `D` where `D`
            // derives from `T`. To be able to work within the reflection system, we have to return an array of objects
            // that are somehow internally recognizes as being of `attributeType`. To do this, we have to create an
            // array instance and copy the results into it, then cast it to an `object[]`. That's just the rules, here.
            var result = Array.CreateInstance(attributeType, 0);
            return (object[])result;
        }

        /// <summary>
        ///   Constructs a new <see cref="SyntheticConstructorInfo"/> representing the implicit default constructor for
        ///   a struct.
        /// </summary>
        /// <param name="type">
        ///   The <see cref="Type"/> of the struct.
        /// </param>
        public SyntheticConstructorInfo(Type type) {
            Debug.Assert(type is not null);
            Debug.Assert(type is not SyntheticType && type.IsValueType);

            DeclaringType = type;
            parameters_ = Array.Empty<SyntheticParameterInfo>();
        }

        /// <summary>
        ///   Constructs a new <see cref="SyntheticConstructorInfo"/> for <see cref="SyntheticType"/>.
        /// </summary>
        /// <param name="type">
        ///   The <see cref="SyntheticType"/> for which the new <see cref="SyntheticConstructorInfo"/> serves.
        /// </param>
        /// <param name="properties">
        ///   The properties of the <see cref="SyntheticType"/> for which the new <see cref="SyntheticConstructorInfo"/> is
        ///   being defined. Each property corresponds to exactly one argument in the constructor.
        /// </param>
        public SyntheticConstructorInfo(SyntheticType type, IEnumerable<SyntheticPropertyInfo> properties) {
            Debug.Assert(type is not null);
            Debug.Assert(properties is not null);

            DeclaringType = type;
            parameters_ = properties.Select(p => new SyntheticParameterInfo(this, p.Name, p.PropertyType)).ToArray();
        }

        /// <inheritdoc/>
        public sealed override IList<CustomAttributeData> GetCustomAttributesData() {
            return new List<CustomAttributeData>();
        }

        /// <inheritdoc/>
        public override ParameterInfo[] GetParameters() {
            return parameters_;
        }

        /// <inheritdoc/>
        public override object Invoke(BindingFlags invokeAttr, Binder? binder, object?[]? parameters, CultureInfo? culture) {
            Debug.Assert(parameters is not null && parameters.Length == parameters_.Length);
            Debug.Assert(parameters.Any(p => p is not null));

            if (parameters_.Length == 1) {
                return parameters[0]!;
            }
            else {
                var ctor = (DeclaringType as SyntheticType)!.ActualType.GetConstructors()[0];
                return ctor.Invoke(invokeAttr, binder, parameters, culture);
            }
        }


        private readonly SyntheticParameterInfo[] parameters_;
    }

    // The functions implemented in this partial definition unconditionally throw NotSupportedExceptions, as they are
    // not needed by Kvasir at all but are necessary parts of the full abstract API of the ConstructorInfo base class
    internal sealed partial class SyntheticConstructorInfo : ConstructorInfo {
        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public override RuntimeMethodHandle MethodHandle {
            get {
                throw new NotSupportedException($"{nameof(SyntheticConstructorInfo)}.{nameof(MethodHandle)}");
            }
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public override string Name {
            get {
                throw new NotSupportedException($"{nameof(SyntheticConstructorInfo)}.{nameof(Name)}");
            }
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public override object[] GetCustomAttributes(bool inherit) {
            throw new NotSupportedException($"{nameof(SyntheticConstructorInfo)}.{nameof(GetCustomAttributes)}");
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public override MethodImplAttributes GetMethodImplementationFlags() {
            throw new NotSupportedException($"{nameof(SyntheticConstructorInfo)}.{nameof(GetMethodImplementationFlags)}");
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public override bool IsDefined(Type attributeType, bool inherit) {
            throw new NotSupportedException($"{nameof(SyntheticConstructorInfo)}.{nameof(IsDefined)}");
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public override object? Invoke(object? obj, BindingFlags invokeAttr, Binder? binder, object?[]? parameters, CultureInfo? culture) {
            throw new NotImplementedException();
        }
    }
}
