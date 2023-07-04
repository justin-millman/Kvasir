using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Cybele.Extensions {
    /// <summary>
    ///   A collection of <see href="https://tinyurl.com/y8q6ojue">extension methods</see> that extend the public API
    ///   of the enumerators.
    /// </summary>
    public static class EnumExtensions {
        /// <summary>
        ///   Determines if an enumerator is "valid."
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     In C#, there are two types of enumerations: "regular" enumerations and "flag" enumerations. The latter
        ///     are distinguished by the presence of a <see cref="FlagsAttribute"/> annotation and afford the user easy
        ///     use of bitwise operations to combine enumerator flags. As in most languages, C# enums are little more
        ///     than strongly typed wrappers around a numeric value: specifically, it is generally possible to cast an
        ///     integer to any enumeration type without error (subject to over- and underflow). This can conceivably
        ///     break APIs that expect an input enumeration to be one of the explicitly defined values.
        ///   </para>
        ///   <para>
        ///     The "validity" of an enumerator is based on this intuitive sense of what values <i>should</i> be
        ///     allowed as arguments to such an API. For "regular" enumerations, only those enumerator values that are
        ///     explicitly defined in the enum's declaration are considered "valid"; this is identical to the behavior
        ///     of the standard library <see cref="Enum.IsDefined(Type, object)"/> method. For "flag" enumerations,
        ///     both the individually defined flags <i>and</i> any bitwise combinations thereof are considered "valid";
        ///     this differs from the behavior of <see cref="Enum.IsDefined(Type, object)"/>, which only recognizes the
        ///     flags.
        ///   </para>
        /// </remarks>
        /// <typeparam name="TEnum">
        ///   [deduced] The type of the enumeration to which the enumerator belongs.
        /// </typeparam>
        /// <param name="self">
        ///   The enumerator on which the extension method is invoked.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if <paramref name="self"/> is an enumerator that either is defined explicitly in
        ///   the declaration of <typeparamref name="TEnum"/> or is a bitwise combination of such values.
        /// </returns>
        public static bool IsValid<TEnum>(this TEnum self) where TEnum : Enum {
            var enumType = typeof(TEnum);
            var underlyingCode = self.GetTypeCode();
            var numeric = self.AsInt64();

            // Checking for the presence of the FlagsAttribute requires reflection, so we memoize the result to reduce
            // the cost of repeated validity checks of enumerators for the same enum type
            if (!isFlagsMemoizer_.TryGetValue(enumType, out bool isFlags)) {
                isFlags = enumType.HasAttribute<FlagsAttribute>();
                isFlagsMemoizer_[enumType] = isFlags;
            }

            // If the enum type is a regular enum, as opposed to a flags enum, then the validity check should behave the
            // same as Enum.IsDefined. We also use the standard library helper when the input enumerator value is 0,
            // because relying on the ensuing bitwise arithmetic would produce a false positive if a zero-flags
            // combination were not explicitly defined for a flags enum.
            if (!isFlags || numeric == 0) {
                return Enum.IsDefined(enumType, self);
            }

            // Producing the aggregate bit pattern for a flags enum requires reflection (to get the collection of
            // enumerators), potentially expensive conversions (enumerator-to-long), and relatively inexpensive bitwise
            // arithmetic in a LINQ context. To reduce the cost of repeated validity checks of enumerators for the same
            // flags enum type, we memoize the aggregate bit pattern.
            if (!bitsMemoizer_.TryGetValue(enumType, out long aggregateBits)) {
                aggregateBits = Enum.GetValues(enumType).Cast<TEnum>().Aggregate(0L, (bits, f) => bits | f.AsInt64());
                bitsMemoizer_[enumType] = aggregateBits;
            }

            // This bitwise arithmetic trick is O(1), rather than some kind of iteration that would be O(N) in the
            // number of bits
            return (aggregateBits | numeric) == aggregateBits;
        }

        /// <summary>
        ///   Produces an array of "valid" enumerators for an <see cref="Enum">enumeration type</see>
        /// </summary>
        /// <seealso cref="IsValid{TEnum}(TEnum)"/>
        /// <param name="self">
        ///   The <see cref="Type"/> on which the extension method is invoked.
        /// </param>
        /// <returns>
        ///   An array of all the "valid" enumerators of <paramref name="self"/>, in no particular order. For a precise
        ///   definition of what it means for an enumerator to be valid, see <see cref="IsValid{TEnum}(TEnum)"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        ///   if <paramref name="self"/> is not an <see cref="Enum">enumeration type</see>.
        /// </exception>
        public static Enum[] ValidValues(this Type self) {
            if (!self.IsEnum) {
                throw new ArgumentException("type must be an enumeration type", nameof(self));
            }

            // There's a lot of reflection and intense computation going on in this function, so we'll try to reuse the
            // results of a previous evaluation if possible.
            if (valuesMemoizer_.TryGetValue(self, out Enum[]? result)) {
                return result!;
            }

            // Checking for the presence of the FlagsAttribute requires reflection, so we memoize the result to reduce
            // the cost of repeated validity checks of enumerators for the same enum type
            if (!isFlagsMemoizer_.TryGetValue(self, out bool isFlags)) {
                isFlags = self.HasAttribute<FlagsAttribute>();
                isFlagsMemoizer_[self] = isFlags;
            }

            // If the enum type is a regular enum, as opposed to a flags enum, then we can just return the result of the
            // standard library's Enum.GetValues() function, as there is no combinatorics involved
            if (!isFlags) {
                return Enum.GetValues(self).Cast<Enum>().ToArray();
            }

            var flags = Enum.GetValues(self)                    // get named enumerators
                .Cast<Enum>()                                   // convert them to Enum
                .Select(e => e.AsInt64())                       // convert them into longs
                .Where(e => e != 0 && (e & (e - 1)) == 0)       // keep only non-zero powers of two (flags)
                .ToList();                                      // enumerate into list

            // This is a combination-generating algorithm that runs in O(N^2) time. First, we figure out how many
            // combinations are possible: this is the size of the power set of the set of flags. Then, we use each
            // combination's index as a binary representation of which flags are turned on, iteratively building up the
            // combination via bitwise-or.
            var results = new HashSet<Enum>();
            var combinations = 1L << flags.Count;
            for (var counter = 1; counter <= combinations; ++counter) {
                var combo = 0L;
                for (int pos = 0; pos < flags.Count; ++pos) {
                    var mask = 1L << pos;
                    if ((counter & mask) != 0) {
                        combo |= flags[pos];
                    }
                }
                results.Add((Enum)Enum.ToObject(self, Convert.ChangeType(combo, Enum.GetUnderlyingType(self))));
            }

            // Because the power set always contains the empty set, the above algorithm will always produce a value of
            // 0 as one of the combinations. However, not all enumerations define a named constant enumerator with value
            // 0, so we have to check for its absence and remove it from the results appropriately.
            var zero = Convert.ChangeType(0, Enum.GetUnderlyingType(self));
            if (!Enum.IsDefined(self, zero)) {
                results.Remove((Enum)Enum.ToObject(self, zero));
            }

            // Memoize the results
            valuesMemoizer_[self] = results.ToArray();
            return valuesMemoizer_[self];
        }

        /// <summary>
        ///   Initializes the <see langword="static"/> state of the <see cref="EnumExtensions"/> class.
        /// </summary>
        static EnumExtensions() {
            // We use ConcurrentDictionary here so that the IsValid check and the ValidValues lookup are thread-safe
            isFlagsMemoizer_ = new ConcurrentDictionary<Type, bool>();
            bitsMemoizer_ = new ConcurrentDictionary<Type, long>();
            valuesMemoizer_ = new ConcurrentDictionary<Type, Enum[]>();
        }

        /// <summary>
        ///   Produces a <c>64</c>-bit integer whose bit pattern is equivalent to the bit pattern of the numeric value
        ///   of an enumerator.
        /// </summary>
        /// <typeparam name="TEnum">
        ///   [deduced] The type of the enumeration to which the enumerator belongs.
        /// </typeparam>
        /// <param name="self">
        ///   The enumerator on which the extension method is invoked.
        /// </param>
        /// <returns>
        ///   A <see cref="long"/> whose bit pattern is equivalent to the bit pattern of the numeric value underlying
        ///   <paramref name="self"/>.
        /// </returns>
        private static long AsInt64<TEnum>(this TEnum self) where TEnum : Enum {
            // Signed and unsigned bytes, shorts, and ints can be converted to a signed long without risk for data
            // loss: the range of unsigned long covers all possible values of these data types. Unsigned longs cannot
            // be safely converted into signed longs, however, due to the potential for overflow. We use the Convert
            // utility class to actually change the enumerator into the integer value.
            var bytes = self.GetTypeCode() == TypeCode.UInt64 ?
                        BitConverter.GetBytes((ulong)Convert.ChangeType(self, typeof(ulong))) :
                        BitConverter.GetBytes((long)Convert.ChangeType(self, typeof(long)));

            return BitConverter.ToInt64(bytes);
        }


        private static readonly IDictionary<Type, bool> isFlagsMemoizer_;
        private static readonly IDictionary<Type, long> bitsMemoizer_;
        private static readonly IDictionary<Type, Enum[]> valuesMemoizer_;
    }
}
