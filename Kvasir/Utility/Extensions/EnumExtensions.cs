using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Cybele.Extensions {
    /// <summary>
    ///   A collection of methods that extend user-defined <see cref="Enum"/> types.
    /// </summary>
    public static class EnumExtensions {
        /// <summary>
        ///   Determines if an enumerator is valid.
        /// </summary>
        /// <typeparam name="TEnum">
        ///   [deduced] The type of <paramref name="enumerator"/>.
        /// </typeparam>
        /// <param name="enumerator">
        ///   The enumerator whose validity to determine.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if <paramref name="enumerator"/> is a valid enumerator; otherwise,
        ///   <see langword="false"/>.
        /// </returns>
        /// <remarks>
        ///   <para>
        ///     This method is intended primarily as a defensive helper to ensure that enumerators provided to an API
        ///     are among those that the author of the enumerator intended to be used. Because enums are simply type
        ///     system wrappers around integers, it is legal to convert an arbitrary integer into a enumeration even
        ///     if no name was assigned to that value in the enumeration's declaration. Passing such a value to an API
        ///     often implicitly violates preconditions, especially when <c>switch</c> statements are employed that
        ///     assume an enumerator is one of the explicitly defined constants.
        ///   </para>
        ///   <para>
        ///     When dealing with "regular" enums (as opposed to <see cref="FlagsAttribute">"flag"</see> enums), this
        ///     method works identically to <see cref="Enum.IsDefined(Type, object)"/>. Specifically, an enumerator of
        ///     a "regular" enum is valid if and only if its value is that of one of the named constants in the enum's
        ///     declaration. This is not affected by enumeration aliasing (i.e. having more than one named constant
        ///     for the same value).
        ///   </para>
        ///   <para>
        ///     When dealing with a <see cref="FlagsAttribute">"flag"</see> enum, the behavior of this method diverges
        ///     from that of <see cref="Enum.IsDefined(Type, object)"/> to provide a response that is more in line with
        ///     most users' expectations. An enumerator of a <see cref="FlagsAttribute">"flag"</see> enum is valid if
        ///     and only if its value is a combination of one or more of the named constants in the enum's declaration.
        ///     This means that un-aliased combinations will be treated as valid by this method, whereas
        ///     <see cref="Enum.IsDefined(Type, object)"/> recognizes only those values specified in the enum's
        ///     declaration. Note that <see cref="FlagsAttribute">"flag"</see> enums do not receive a <c>None</c>
        ///     enumerator automatically; as such, an enumerator with numeric value <c>0</c> is only valid if such a
        ///     constant is explicitly defined by the enumeration.
        ///   </para>
        /// </remarks>
        public static bool IsValid<TEnum>(this TEnum enumerator) where TEnum : Enum {
            var enumType = typeof(TEnum);
            var underlyingCode = enumerator.GetTypeCode();
            var numeric = enumerator.AsInt64();
            
            // Memoize the results of the flag check so that we only perform the reflection once per enum type
            if (!isFlagsMemoizer_.TryGetValue(enumType, out bool isFlags)) {
                isFlags = !(enumType.GetCustomAttribute<FlagsAttribute>() is null);
                isFlagsMemoizer_[enumType] = isFlags;
            }

            // If the enum type is a regular enum, as opposed to a flags enum, then this function should behave the
            // same as Enum.IsDefined. We also use this check for when the input enumerator value is 0, because relying
            // on the ensuing bitwise arithmetic would produce potential false positives if a zero-flags combination
            // were not explicitly defined.
            if (!isFlags || numeric == 0L) {
                return Enum.IsDefined(enumType, enumerator);
            }

            // Memoize the results of the bit aggregation so that we only perform the reflection and bitwise arithetic
            // once per enum type
            if (!bitsMemoizer_.TryGetValue(enumType, out long aggregateBits)) {
                aggregateBits = Enum.GetValues(enumType).Cast<TEnum>().Aggregate(0L, (i, f) => i | f.AsInt64());
                bitsMemoizer_[enumType] = aggregateBits;
            }

            // We use a bitwise arithmetic trick to determine if the provided enumerator is valid by ensuring that all
            // of the "on" bits are also "on" in the aggregation
            return (aggregateBits | numeric) == aggregateBits;
        }

        /// <summary>
        ///   Reinterprets the bit pattern of the numeric value of an enumerator as the bit pattern for a <c>64</c>-bit
        ///   signed integer (i.e. a <see cref="long"/>).
        /// </summary>
        /// <typeparam name="TEnum">
        ///   [deduced] The type of <paramref name="enumerator"/>.
        /// </typeparam>
        /// <param name="enumerator">
        ///   The enumerator whose bit pattern to reinterpret.
        /// </param>
        /// <returns>
        ///   A <see cref="long"/> whose bit pattern is the same as the bit pattern of <paramref name="enumerator"/>,
        ///   possibly with some extra leading <c>0</c>s.
        /// </returns>
        private static long AsInt64<TEnum>(this TEnum enumerator) where TEnum : Enum {
            // Signed and unsigned bytes, shorts, and ints can be converted to a signed long without risk for data
            // loss: the range of unsigned long covers all possible values. This is not the case for signed long,
            // however, due to the potential for overflow. We have to box the enumerator into a ValueType first because
            // we can't cast the generic TEnum instance directly to a numeric type.
            var bytes = enumerator.GetTypeCode() == TypeCode.UInt64 ?
                        BitConverter.GetBytes((ulong)Convert.ChangeType(enumerator, typeof(ulong))) :
                        BitConverter.GetBytes((long)Convert.ChangeType(enumerator, typeof(long)));

            return BitConverter.ToInt64(bytes);
        }


        private static readonly Dictionary<Type, bool> isFlagsMemoizer_ = new Dictionary<Type, bool>();
        private static readonly Dictionary<Type, long> bitsMemoizer_ = new Dictionary<Type, long>();
    }
}
