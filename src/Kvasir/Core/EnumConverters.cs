using Cybele.Core;
using System;
using System.Diagnostics;
using System.Reflection;

namespace Kvasir.Core {
    /// <summary>
    ///   A built-in <see cref="IDataConverter"/> that bidirectionally converts between enumerations and their
    ///   underlying numeric value.
    /// </summary>
    internal sealed class EnumToNumericConverter : IDataConverter {
        /// <inheritdoc/>
        public DataConverter ConverterImpl => impl_;

        /// <summary>
        ///   Initialize the static state of the <see cref="EnumToNumericConverter"/> class.
        /// </summary>
        static EnumToNumericConverter() {
            var flags = BindingFlags.Static | BindingFlags.NonPublic;
            MAKE_IMPL_FN = typeof(EnumToNumericConverter).GetMethod(nameof(MakeImpl), flags)!;
        }

        /// <summary>
        ///   Constructs a new <see cref="EnumToNumericConverter"/>.
        /// </summary>
        /// <param name="enumType">
        ///   The type of the enumeration to convert.
        /// </param>
        public EnumToNumericConverter(Type enumType) {
            Debug.Assert(enumType is not null);
            Debug.Assert(enumType.IsEnum);

            var underlyingType = Enum.GetUnderlyingType(enumType);
            var maker = MAKE_IMPL_FN.MakeGenericMethod(enumType, underlyingType);
            impl_ = (DataConverter)maker.Invoke(null, Array.Empty<object?>())!;
        }

        /// <summary>
        ///   Creates a <see cref="DataConverter"/> that bidirectionally converts between a specific enumeration type
        ///   and its underlying numeric type.
        /// </summary>
        /// <typeparam name="TEnum">
        ///   The enumeration type.
        /// </typeparam>
        /// <typeparam name="TNumeric">
        ///   The underlying numeric type of <typeparamref name="TEnum"/>.
        /// </typeparam>
        /// <returns>
        ///   A <see cref="DataConverter"/> that converts instances of <typeparamref name="TEnum"/> into the equivalent
        ///   <typeparamref name="TNumeric"/>, and vice-versa.
        /// </returns>
        private static DataConverter MakeImpl<TEnum, TNumeric>() where TEnum : Enum where TNumeric : struct {
            Debug.Assert(Enum.GetUnderlyingType(typeof(TEnum)) == typeof(TNumeric));
            Converter<TEnum, TNumeric> fwd = e => (TNumeric)Convert.ChangeType(e, typeof(TNumeric));
            Converter<TNumeric, TEnum> bwd = n => (TEnum)Enum.ToObject(typeof(TEnum), n);
            return DataConverter.Create(fwd, bwd);
        }


        private readonly DataConverter impl_;
        private static readonly MethodInfo MAKE_IMPL_FN;
    }

    /// <summary>
    ///   A built-in <see cref="IDataConverter"/> that bidirectionally converts between enumerations and their string
    ///   representation.
    /// </summary>
    /// <remarks>
    ///   The string representation of an enumeration is usually the result of invoking <c>ToString()</c> on the
    ///   enumeration. For unnamed combinations of <see cref="FlagsAttribute">flag enumerators</see>, the string
    ///   representation is the <c>ToString()</c> result with the "comma-space" delimiter replaced by a vertical bar
    ///   (<c>|</c>).
    /// </remarks>
    internal sealed class EnumToStringConverter : IDataConverter {
        /// <inheritdoc/>
        public DataConverter ConverterImpl => impl_;

        /// <summary>
        ///   Initialize the static state of the <see cref="EnumToStringConverter"/> class.
        /// </summary>
        static EnumToStringConverter() {
            var flags = BindingFlags.Static | BindingFlags.NonPublic;
            MAKE_IMPL_FN = typeof(EnumToStringConverter).GetMethod(nameof(MakeImpl), flags)!;
        }

        /// <summary>
        ///   Constructs a new <see cref="EnumToStringConverter"/>.
        /// </summary>
        /// <param name="enumType">
        ///   The type of the enumeration to convert.
        /// </param>
        public EnumToStringConverter(Type enumType) {
            Debug.Assert(enumType is not null);
            Debug.Assert(enumType.IsEnum);

            var maker = MAKE_IMPL_FN.MakeGenericMethod(enumType);
            impl_ = (DataConverter)maker.Invoke(null, Array.Empty<object?>())!;
        }

        /// <summary>
        ///   Creates a <see cref="DataConverter"/> that bidirectionally converts between a specific enumeration type
        ///   and its Kvasir-defined string representation.
        /// </summary>
        /// <typeparam name="TEnum">
        ///   The enumeration type.
        /// </typeparam>
        /// <returns>
        ///   A <see cref="DataConverter"/> that converts instances of <typeparamref name="TEnum"/> into the equivalent
        ///   <see cref="string"/>, and vice-versa.
        /// </returns>
        private static DataConverter MakeImpl<TEnum>() where TEnum : Enum {
            Converter<TEnum, string> fwd = e => e.ToString()!.Replace(", ", "|");
            Converter<string, TEnum> bwd = s => (TEnum)Enum.Parse(typeof(TEnum), s.Replace("|", ", "));
            return DataConverter.Create(fwd, bwd);
        }


        private readonly DataConverter impl_;
        private static readonly MethodInfo MAKE_IMPL_FN;
    }
}
