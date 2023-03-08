using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Kvasir.Schema {
    /// <summary>
    ///   The semantic data type for a Field.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     The <see cref="DBType"/> is strictly a semantic specification, incorporating nothing about the actual
    ///     mechanism by which the type is represented in a back-end database. Different RDBMS providers realize
    ///     semantic types differently; for example, SQL Server does not distinguish between signed and unsigned
    ///     integers whereas MySQL does. <see cref="DBType"/> is intended to partition the data type space along these
    ///     universal semantic fault lines, leaving the actual storage mechanics up to the back-end provider being
    ///     used.
    ///   </para>
    ///   <para>
    ///     Because of this abstract angle, certain aspects of a data type are not represented directly by the
    ///     <see cref="DBType"/> class. This includes precisions for floating point types, ranges for all numeric and
    ///     date-like types, encodings for text types, and others. This extends to enumeration types, where the
    ///     discrete set of allowed values is specified at the Field level rather than by the <see cref="DBType"/>.
    ///     This overall abstraction allows for a single Schema translation to be used for any back-end RDBMS.
    ///   </para>
    ///   <para>
    ///     The various semantic type categories are represented as distinct <see cref="DBType"/> instances exposed as
    ///     <see langword="static"/> pseudo-enumerators: it is not possible to create custom <see cref="DBType"/>
    ///     instances.
    ///   </para>
    /// </remarks>
    public readonly struct DBType : IEquatable<DBType> {
        /// <summary>
        ///   The <see cref="DBType"/> representing a Boolean value.
        /// </summary>
        public static DBType Boolean { get; }

        /// <summary>
        ///   The <see cref="DBValue"/> representing a single UTF-16 character.
        /// </summary>
        public static DBType Character { get; }

        /// <summary>
        ///   The <see cref="DBValue"/> representing an <c>8</c>-bit signed integer.
        /// </summary>
        public static DBType Int8 { get; }

        /// <summary>
        ///   The <see cref="DBValue"/> representing a <c>16</c>-bit signed integer.
        /// </summary>
        public static DBType Int16 { get; }

        /// <summary>
        ///   The <see cref="DBValue"/> representing a <c>32</c>-bit signed integer.
        /// </summary>
        public static DBType Int32 { get; }

        /// <summary>
        ///   The <see cref="DBValue"/> representing a <c>64</c>-bit signed integer.
        /// </summary>
        public static DBType Int64 { get; }

        /// <summary>
        ///   The <see cref="DBValue"/> representing an <c>8</c>-bit unsigned integer.
        /// </summary>
        public static DBType UInt8 { get; }

        /// <summary>
        ///   The <see cref="DBValue"/> representing a <c>16</c>-bit unsigned integer.
        /// </summary>
        public static DBType UInt16 { get; }

        /// <summary>
        ///   The <see cref="DBType"/> representing a <c>32</c>-bit unsigned integer.
        /// </summary>
        public static DBType UInt32 { get; }

        /// <summary>
        ///   The <see cref="DBType"/> representing a <c>64</c>-bit unsigned integer.
        /// </summary>
        public static DBType UInt64 { get; }

        /// <summary>
        ///   The <see cref="DBType"/> representing a single-precision floating point number.
        /// </summary>
        public static DBType Single { get; }

        /// <summary>
        ///   The <see cref="DBType"/> representing a double-precision floating point number.
        /// </summary>
        public static DBType Double { get; }

        /// <summary>
        ///   The <see cref="DBType"/> representing a decimal number.
        /// </summary>
        public static DBType Decimal { get; }

        /// <summary>
        ///   The <see cref="DBType"/> representing a calendar date and time.
        /// </summary>
        public static DBType DateTime { get; }

        /// <summary>
        ///   The <see cref="DBType"/> representing freeform text.
        /// </summary>
        public static DBType Text { get; }

        /// <summary>
        ///   The <see cref="DBType"/> representing a globally unique identifier.
        /// </summary>
        public static DBType Guid { get; }

        /// <summary>
        ///   The <see cref="DBType"/> representing an enumeration.
        /// </summary>
        /// <remarks>
        ///   Note that all flavors of enumeration are collapsed into this single <see cref="DBType"/>. The specifics
        ///   of the enumeration--namely, what the discrete set of allowed values is--is specified at the Field level,
        ///   as different back-end providers have different levels of native support for enumerations.
        /// </remarks>
        public static DBType Enumeration { get; }

        /// <summary>
        ///   Determines if a CLR <see cref="Type"/> is supported by Kvasir.
        /// </summary>
        /// <param name="clrType">
        ///   The CLR <see cref="Type"/> for which to check for support.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if <paramref name="clrType"/> is supported by Kvasir; otherwise,
        ///   <see langword="false"/>.
        /// </returns>
        public static bool IsSupported(Type clrType) {
            return lookup_.ContainsKey(Normalize(clrType));
        }

        /// <summary>
        ///   Looks up the <see cref="DBType"/> instance that corresponds to a CLR <see cref="Type"/>.
        /// </summary>
        /// <remarks>
        ///   The mapping of CLR <see cref="Type"/> to <see cref="DBType"/> instance is not bijective. Specifically,
        ///   all <see cref="Enum"/> types are mapped to the same value (<see cref="Enumeration"/>) and the generic
        ///   argument is used when the <see cref="Type"/> is an instantiation of the <see cref="Nullable{T}"/>
        ///   generic.
        /// </remarks>
        /// <param name="clrType">
        ///   The CLR <see cref="Type"/> for which to look up the corresponding <see cref="DBType"/> instance.
        /// </param>
        /// <returns>
        ///   The <see cref="DBType"/> instance that corresponds to <paramref name="clrType"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        ///   if <paramref name="clrType"/> is not supported by Kvasir, and therefore has no corresponding
        ///   <see cref="DBType"/>.
        /// </exception>
        public static DBType Lookup(Type clrType) {
            if (!lookup_.TryGetValue(Normalize(clrType), out DBType result)) {
                Debug.Assert(!IsSupported(clrType));
                var msg = $"CLR type {clrType.Name} is not supported by {nameof(Kvasir)}";
                throw new ArgumentException(msg, nameof(clrType));
            }

            Debug.Assert(IsSupported(clrType));
            return result;
        }

        /// <summary>
        ///   Determines if this <see cref="DBType"/> is equal to another.
        /// </summary>
        /// <param name="rhs">
        ///   The <see cref="DBType"/> against which to compare this one.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if <paramref name="rhs"/> is equal to this <see cref="DBType"/>; otherwise,
        ///   <see langword="false"/>.
        /// </returns>
        public readonly bool Equals(DBType rhs) {
            return id_ == rhs.id_;
        }

        /// <summary>
        ///   Determines if this <see cref="DBType"/> is equal to another <see cref="object"/>.
        /// </summary>
        /// <param name="rhs">
        ///   The <see cref="object"/> against which to compare this <see cref="DBType"/>.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if <paramref name="rhs"/> is a non-<see langword="null"/> <see cref="DBType"/>
        ///   that is equal to this one; otherwise, <see langword="false"/>.
        /// </returns>
        public readonly override bool Equals(object? rhs) {
            return (rhs is DBType dbt) && Equals(dbt);
        }

        /// <summary>
        ///   Produces the hash code for this <see cref="DBType"/>.
        /// </summary>
        /// <returns>
        ///   The hash code for this <see cref="DBType"/>.
        /// </returns>
        public readonly override int GetHashCode() {
            return HashCode.Combine(id_);
        }

        /// <summary>
        ///   Produces a human-readable string representation of this <see cref="DBType"/>.
        /// </summary>
        /// <returns>
        ///   A human-readable string representation of this <see cref="DBType"/>.
        /// </returns>
        public readonly override string ToString() {
            return STRING_FORMS[id_];
        }

        /// <summary>
        ///   Determines if one <see cref="DBType"/> is equal to another.
        /// </summary>
        /// <param name="lhs">
        ///   The first <see cref="DBType"/>.
        /// </param>
        /// <param name="rhs">
        ///   The second <see cref="DBType"/>.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if <paramref name="lhs"/> is equal to <paramref name="rhs"/>; otherwise,
        ///   <see langword="false"/>.
        /// </returns>
        public static bool operator==(DBType lhs, DBType rhs) {
            return lhs.Equals(rhs);
        }

        /// <summary>
        ///   Determines if one <see cref="DBType"/> is not equal to another.
        /// </summary>
        /// <param name="lhs">
        ///   The first <see cref="DBType"/>.
        /// </param>
        /// <param name="rhs">
        ///   The second <see cref="DBType"/>.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if <paramref name="lhs"/> is not equal to <paramref name="rhs"/>; otherwise,
        ///   <see langword="false"/>.
        /// </returns>
        public static bool operator!=(DBType lhs, DBType rhs) {
            return !(lhs == rhs);
        }

        /// <summary>
        ///   Initializes the <see langword="static"/> state of the <see cref="DBType"/> struct.
        /// </summary>
        static DBType() {
            ENUM_SENTINEL = typeof(Enum);

            STRING_FORMS = new string[] {
                "Int32", "Boolean", "Character", "Date/Time", "Decimal", "Double", "Enumeration", "GUID", "Int8",
                "Int16", "Int64", "Single", "Text", "UInt8", "UInt16", "UInt32", "UInt64"
            };

            Debug.Assert(default(byte) == 0, $"Sequence of IDs for {nameof(DBType)} is not starting at 0");
            Int32 = new DBType(default);
            Boolean = new DBType(default(byte) + 1);
            Character = new DBType(default(byte) + 2);
            DateTime = new DBType(default(byte) + 3);
            Decimal = new DBType(default(byte) + 4);
            Double = new DBType(default(byte) + 5);
            Enumeration = new DBType(default(byte) + 6);
            Guid = new DBType(default(byte) + 7);
            Int8 = new DBType(default(byte) + 8);
            Int16 = new DBType(default(byte) + 9);
            Int64 = new DBType(default(byte) + 10);
            Single = new DBType(default(byte) + 11);
            Text = new DBType(default(byte) + 12);
            UInt8 = new DBType(default(byte) + 13);
            UInt16 = new DBType(default(byte) + 14);
            UInt32 = new DBType(default(byte) + 15);
            UInt64 = new DBType(default(byte) + 16);

            // We don't need to use a ConcurrentDictionary<Type, DBType> because we will only be reading from the
            // lookup map, and Dictionary itself is threadsafe when reading only
            lookup_ = new Dictionary<Type, DBType>() {
                [typeof(bool)] = Boolean,
                [typeof(byte)] = UInt8,
                [typeof(char)] = Character,
                [typeof(DateTime)] = DateTime,
                [typeof(decimal)] = Decimal,
                [typeof(double)] = Double,
                [ENUM_SENTINEL] = Enumeration,
                [typeof(float)] = Single,
                [typeof(Guid)] = Guid,
                [typeof(int)] = Int32,
                [typeof(long)] = Int64,
                [typeof(sbyte)] = Int8,
                [typeof(short)] = Int16,
                [typeof(string)] = Text,
                [typeof(uint)] = UInt32,
                [typeof(ulong)] = UInt64,
                [typeof(ushort)] = UInt16
            };
        }

        /// <summary>
        ///   Constructs a new <see cref="DBType"/>.
        /// </summary>
        /// <param name="id">
        ///   The ID of the new <see cref="DBType"/>. This also serves as the indexer for the various traits of the new
        ///   instance, including its string representation.
        /// </param>
        private DBType(byte id) {
            Debug.Assert(id >= 0 && id < STRING_FORMS.Length, $"{nameof(DBType)} {nameof(id)} is out of range");
            id_ = id;
        }

        /// <summary>
        ///   Normalizes a CLR <see cref="Type"/> by stripping any <see cref="Nullable{T}"/> wrapper and collapsing all
        ///   <see cref="Enum"/> types into a single sentinel.
        /// </summary>
        /// <param name="clrType">
        ///   The CLR <see cref="Type"/> to normalize.
        /// </param>
        /// <returns>
        ///   The normalization of <paramref name="clrType"/>.
        /// </returns>
        private static Type Normalize(Type clrType) {
            // If the CLR type is an instantiation of the Nullable<> generic, we'll deal with the generic argument
            // instead, because DBType does not account for nullability. The generic argument cannot itself be an
            // instantiation of Nullable<>, so a maximum of one unwrapping is needed.
            var unwrapped = Nullable.GetUnderlyingType(clrType) ?? clrType;

            // All CLR enumerations get collapsed into a single sentinel
            return unwrapped.IsEnum ? ENUM_SENTINEL : unwrapped;
        }


        private readonly byte id_;
        private static readonly IReadOnlyDictionary<Type, DBType> lookup_;
        private static readonly Type ENUM_SENTINEL;
        private static readonly string[] STRING_FORMS;
    }
}
