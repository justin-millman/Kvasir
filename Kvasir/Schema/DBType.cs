using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Kvasir.Schema {
    /// <summary>
    ///   An implementation-agnostic representation of a data type for a Field in a back-end database.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     A <see cref="DBType"/> instance represents the semantics of a data type without specifying the actual
    ///     storage mechanism or data type metadata. These semantics are intentionally generalized, allowing for a
    ///     single <see cref="DBType"/> to represent data types across a variety of back-end database providers.
    ///     Because different providers may offer different restrictions on data types (such as precision for floating
    ///     point numbers or maximum lengths for strings), these auxiliaries are excluded from the content of a
    ///     <see cref="DBType"/>.
    ///   </para>
    ///   <para>
    ///     Functionally, the <see cref="DBType"/> struct operates much like an enumeration, in that users are limited
    ///     to the set of predefined instances that are accessed through the <c>DBType</c> pseduo-namespace. Unlike
    ///     native enumerations, however, <see cref="DBType"/> instances cannot interoperate with numeric primitives
    ///     and cannot be used directly in a <c>switch</c> statement (though they <i>can</i> be used in pattern
    ///     matching expressions). Additionally, the <see cref="DBType"/> struct provides a series of member functions
    ///     for executing various queries.
    ///   </para>
    ///   <para>
    ///     One thing that is notably absent from the specification carried by a <see cref="DBType"/> is any indication
    ///     of nullability. Within Kvasir, nullability is a feature of Fields, and is thus fully omitted from the
    ///     <see cref="DBType"/> struct.
    ///   </para>
    /// </remarks>
    public readonly struct DBType : IEquatable<DBType> {
        /// <value>
        ///   The representation of a Boolean value (i.e. <c>true</c>/<c>false</c>).
        /// </value>
        public static DBType Boolean { get; } = new DBType(default(sbyte) + 1);

        /// <value>
        ///   The representation of a single UTF-16 character point.
        /// </value>
        public static DBType Character { get; } = new DBType(default(sbyte) + 2);

        /// <value>
        ///   The representation of an <c>8</c>-bit signed integer.
        /// </value>
        public static DBType Int8 { get; } = new DBType(default(sbyte) + 3);

        /// <value>
        ///   The representation of a <c>16</c>-bit signed integer.
        /// </value>
        public static DBType Int16 { get; } = new DBType(default(sbyte) + 4);

        /// <value>
        ///   The representation of a <c>32</c>-bit signed integer.
        /// </value>
        /// <remarks>
        ///   <see cref="Int32"/> is guaranteed to be equal to a default constructed <see cref="DBType"/> instance.
        /// </remarks>
        public static DBType Int32 { get; } = new DBType(default);

        /// <value>
        ///   The representation of a <c>64</c>-bit signed integer.
        /// </value>
        public static DBType Int64 { get; } = new DBType(default(sbyte) + 5);

        /// <value>
        ///   The representation of an <c>8</c>-bit unsigned integer.
        /// </value>
        public static DBType UInt8 { get; } = new DBType(default(sbyte) + 6);

        /// <value>
        ///   The representation of a <c>16</c>-bit unsigned integer.
        /// </value>
        public static DBType UInt16 { get; } = new DBType(default(sbyte) + 7);

        /// <value>
        ///   The representation of a <c>32</c>-bit unsigned integer.
        /// </value>
        public static DBType UInt32 { get; } = new DBType(default(sbyte) + 8);

        /// <value>
        ///   The representation of a <c>64</c>-bit unsigned integer.
        /// </value>
        public static DBType UInt64 { get; } = new DBType(default(sbyte) + 9);

        /// <value>
        ///   The representation of a single-precision floating point number.
        /// </value>
        public static DBType Single { get; } = new DBType(default(sbyte) + 10);

        /// <value>
        ///   The representation of a double-precision floating point number.
        /// </value>
        public static DBType Double { get; } = new DBType(default(sbyte) + 11);

        /// <value>
        ///   The representation of a decimal floating point number.
        /// </value>
        public static DBType Decimal { get; } = new DBType(default(sbyte) + 12);

        /// <value>
        ///   The representation of a timestamp consisting of a calendar date and a time.
        /// </value>
        public static DBType DateTime { get; } = new DBType(default(sbyte) + 13);

        /// <value>
        ///   The representation of a variable-length string of characters.
        /// </value>
        public static DBType Text { get; } = new DBType(default(sbyte) + 14);

        /// <value>
        ///   The representation of a globally unique identifier.
        /// </value>
        public static DBType Guid { get; } = new DBType(default(sbyte) + 15);

        /// <value>
        ///   The representation of an enumeration, i.e. a restricted set of integral or string options.
        /// </value>
        public static DBType Enumeration { get; } = new DBType(default(sbyte) + 16);

        /// <summary>
        ///   Determines if a particular CLR <see cref="Type"/> is supported by Kvasir.
        /// </summary>
        /// <param name="clrType">
        ///   The CLR <see cref="Type"/>.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if there is a <see cref="DBType"/> instance that matches the semantics of
        ///   <paramref name="clrType"/>; otherwise, <see langword="false"/>.
        /// </returns>
        /// <seealso cref="Lookup(Type)"/>
        public static bool IsSupported(Type clrType) {
            return lookup_.ContainsKey(Normalize(clrType));
        }

        /// <summary>
        ///   Looks up the <see cref="DBType"/> instance that matches the semantics of a particular CLR
        ///   <see cref="Type"/>.
        /// </summary>
        /// <param name="clrType">
        ///   The CLR <see cref="Type"/>.
        /// </param>
        /// <returns>
        ///   The <see cref="DBType"/> instance that matches the semantics of <paramref name="clrType"/>. This is the
        ///   <see cref="DBType"/> of a Field whose .NET source values are instances of <paramref name="clrType"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        ///   If there is no <see cref="DBType"/> instance that matches the semantics of <paramref name="clrType"/>.
        /// </exception>
        /// <seealso cref="IsSupported(Type)"/>
        public static DBType Lookup(Type clrType) {
            if (!lookup_.TryGetValue(Normalize(clrType), out DBType result)) {
                Debug.Assert(!IsSupported(clrType));
                var msg = $"CLR Type {clrType} is not supported by Kvasir";
                throw new ArgumentException(msg);
            }
            Debug.Assert(IsSupported(clrType));
            return result;
        }

        /// <summary>
        ///   Determines if a particular <see cref="DBValue"/> would be a valid value for a Field whose data type is
        ///   this <see cref="DBType"/>.
        /// </summary>
        /// <param name="value">
        ///   The <see cref="DBValue"/>.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if <paramref name="value"/> is <see cref="DBValue.NULL"/> or if this
        ///   <see cref="DBType"/> matches the semantics of the type of datum wrapped by <paramref name="value"/>;
        ///   otherwise, <see langword="false"/>.
        /// </returns>
        /// <remarks>
        ///   <para>
        ///     Within Kvasir, enums are represented as string values; this is the format by which the data is actually
        ///     transacted with the back-end database. For this reason, a <see cref="DBValue"/> cannot actually wrap a
        ///     CLR enumerator. As such, <see cref="IsValidValue(DBValue)"/> will return <see langword="true"/> if
        ///     <paramref name="value"/> wraps a <see cref="string"/> and the current <see cref="DBType"/> is
        ///     <i>either</i> <see cref="Text"/> or <see cref="Enumeration"/>. However, the <see cref="Lookup(Type)"/>
        ///     method will <i>always</i> return <see cref="Text"/> when the input <see cref="Type"/> is
        ///     <see cref="string"/>.
        ///   </para>
        ///   <para>
        ///     Additionally, <see cref="IsValidValue(DBValue)"/> does not account for any restrictions on a particular
        ///     realization of an abstract data type, such as range limits, valid enumerators, or nullability: just
        ///     because <see cref="IsValidValue(DBValue)"/> returns <see langword="true"/> does not mean that the value
        ///     can actually be stored in a Field whose data type is the current <see cref="DBType"/>.
        ///   </para>
        /// </remarks>
        public readonly bool IsValidValue(DBValue value) {
            if (value == DBValue.NULL) {
                return true;
            }
            else if (this == Enumeration) {
                return value.Datum.GetType() == typeof(string);
            }
            else {
                return Equals(Lookup(value.Datum.GetType()));
            }
        }

        /// <summary>
        ///   Determines whether this <see cref="DBType"/> is equal to another <see cref="DBType"/>.
        /// </summary>
        /// <param name="rhs">
        ///   The <see cref="DBType"/> against which to compare this one for equality.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if this <see cref="DBType"/> is equal to <paramref name="rhs"/>; otherwise,
        ///   <see langword="false"/>.
        /// </returns>
        public readonly bool Equals(DBType rhs) {
            return id_ == rhs.id_;
        }

        /// <summary>
        ///   Determines whether this <see cref="DBType"/> is equal to another <see cref="object"/>.
        /// </summary>
        /// <param name="rhs">
        ///   The <see cref="object"/> against which to compare this one for equality.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if <paramref name="rhs"/> is a <see cref="DBType"/> that is equal to this
        ///   <see cref="DBType"/>; otherwise, <see langword="false"/>.
        /// </returns>
        public readonly override bool Equals(object? rhs) {
            return (rhs is DBType dbt) && Equals(dbt);
        }

        /// <summary>
        ///   Produces the hash code for this <see cref="DBType"/>.
        /// </summary>
        /// <returns>
        ///   A <c>32</c>-bit signed integer that is the hash code for this <see cref="DBType"/>.
        /// </returns>
        public readonly override int GetHashCode() {
            return id_.GetHashCode();
        }

        /// <summary>
        ///   Determines if two <see cref="DBType">DBTypes</see> are equal.
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
        ///   Determiens if two <see cref="DBType">DBTypes</see> are not equal.
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
        ///   Initializes the static state of the <see cref="DBType"/> struct.
        /// </summary>
        static DBType() {
            lookup_ = new Dictionary<Type, DBType>() {
                [typeof(bool)] = Boolean,
                [typeof(byte)] = UInt8,
                [typeof(char)] = Character,
                [typeof(DateTime)] = DateTime,
                [typeof(decimal)] = Decimal,
                [typeof(double)] = Double,
                [ENUM] = Enumeration,
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
        ///   The identifier for the new <see cref="DBType"/>.
        /// </param>
        /// <pre>
        ///   <paramref name="id"/> is in the range <c>[0, 16]</c>.
        /// </pre>
        private DBType(byte id) {
            Debug.Assert(id >= 0 && id <= 16, "DBType id is out of range");
            id_ = id;
        }

        /// <summary>
        ///   Nomralizes a a CLR <see cref="Type"/> for internal lookup.
        /// </summary>
        /// <param name="clrType">
        ///   The CLR <see cref="Type"/> to normalize.
        /// </param>
        /// <returns>
        ///   The normalization of <paramref name="clrType"/>.
        /// </returns>
        private static Type Normalize(Type clrType) {
            // If the CLR type is an instantiation of the Nullable<T> generic, extracting the argument T and move
            // forward with that instead
            var normal = Nullable.GetUnderlyingType(clrType) ?? clrType;

            // Collapse all enumerations into a single sentinel
            return normal.IsEnum ? ENUM : normal;
        }


        private readonly byte id_;
        private static readonly IReadOnlyDictionary<Type, DBType> lookup_;
        private static readonly Type ENUM = typeof(Enum);
    }
}
