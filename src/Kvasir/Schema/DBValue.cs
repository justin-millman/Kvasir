using Ardalis.GuardClauses;
using System;

namespace Kvasir.Schema {
    /// <summary>
    ///   A thin, type-controlled wrapper around a CLR object that can be stored in a back-end relational database.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     The <see cref="DBValue"/> struct is intended to provide an abstraction over the generalized CLR
    ///     <see cref="object"/> for use in contexts where compile-time type information is not available. The
    ///     advantage in using a <see cref="DBValue"/> over raw a <see cref="object"/> is that the former is guaranteed
    ///     to only ever hold an object that can actually be stored in a back-end RDBMS, whereas the latter could
    ///     feasibly hold anything. Additionally, a <see cref="DBValue"/> never actually wraps a <see langword="null"/>
    ///     value directly: a sentinel is used such that accessing the raw value is always valid.
    ///   </para>
    ///   <para>
    ///     The strong typing afforded by a CLR <see cref="Enum">enum</see> is lost when the enumerator values are
    ///     stored in a back-end relational database. Some providers afford value checking, either through an explicit
    ///     <c>enum</c> data type or via <c>CHECK</c> constraints; however, even in these cases, the actual storage is
    ///     generally either an integer or a string. For this reason, a <see cref="DBValue"/> cannot wrap a CLR
    ///     enumerator: is the client's responsibility to convert the CLR enumerator into the corresponding database
    ///     object, be it an integer or another supported type.
    ///   </para>
    ///   <para>
    ///     The same general logic applies to conversions from other CLR types, including between CLR types that are
    ///     otherwise supported by Kvasir. The <see cref="DBValue"/> struct is intended to wrap exactly the value that
    ///     is to be stored in the database, with the burden of performing any transforms falling on the client.
    ///   </para>
    ///   <para>
    ///     Because a <see cref="DBValue"/> might hold the <c>NULL</c> sentinel value, which is valid for any data
    ///     type, it is not strictly possible to determine the <see cref="DBType"/> of a particular
    ///     <see cref="DBValue"/>. To check if a particular <see cref="DBValue"/> instance can be safely used for a
    ///     Field of a particular <see cref="DBType"/>, use the <see cref="IsInstanceOf(DBType)">dedicated API</see>.
    ///   </para>
    /// </remarks>
    public readonly struct DBValue : IEquatable<DBValue> {
        /// <summary>
        ///   The raw value of this <see cref="DBValue"/>.
        /// </summary>
        public readonly object Datum { get; }

        /// <summary>
        ///   The sentinel <see cref="DBValue"/> representing <c>NULL</c>.
        /// </summary>
        public static DBValue NULL { get; }

        /// <summary>
        ///   Constructs a new <see cref="DBValue"/> from a <see cref="bool"/>.
        /// </summary>
        /// <param name="value">
        ///   The <see cref="bool"/> value of the new <see cref="DBValue"/>.
        /// </param>
        public DBValue(bool value) {
            Datum = value;
        }

        /// <summary>
        ///   Constructs a new <see cref="DBValue"/> from a <see cref="byte"/>.
        /// </summary>
        /// <param name="value">
        ///   The <see cref="byte"/> value of the new <see cref="DBValue"/>.
        /// </param>
        public DBValue(byte value) {
            Datum = value;
        }

        /// <summary>
        ///   Constructs a new <see cref="DBValue"/> from a <see cref="sbyte"/>.
        /// </summary>
        /// <param name="value">
        ///   The <see cref="sbyte"/> value of the new <see cref="DBValue"/>.
        /// </param>
        public DBValue(sbyte value) {
            Datum = value;
        }

        /// <summary>
        ///   Constructs a new <see cref="DBValue"/> from a <see cref="ushort"/>.
        /// </summary>
        /// <param name="value">
        ///   The <see cref="ushort"/> value of the new <see cref="DBValue"/>.
        /// </param>
        public DBValue(ushort value) {
            Datum = value;
        }

        /// <summary>
        ///   Constructs a new <see cref="DBValue"/> from a <see cref="short"/>.
        /// </summary>
        /// <param name="value">
        ///   The <see cref="short"/> value of the new <see cref="DBValue"/>.
        /// </param>
        public DBValue(short value) {
            Datum = value;
        }

        /// <summary>
        ///   Constructs a new <see cref="DBValue"/> from a <see cref="uint"/>.
        /// </summary>
        /// <param name="value">
        ///   The <see cref="uint"/> value of the new <see cref="DBValue"/>.
        /// </param>
        public DBValue(uint value) {
            Datum = value;
        }

        /// <summary>
        ///   Constructs a new <see cref="DBValue"/> from a <see cref="int"/>.
        /// </summary>
        /// <param name="value">
        ///   The <see cref="int"/> value of the new <see cref="DBValue"/>.
        /// </param>
        public DBValue(int value) {
            Datum = value;
        }

        /// <summary>
        ///   Constructs a new <see cref="DBValue"/> from a <see cref="ulong"/>.
        /// </summary>
        /// <param name="value">
        ///   The <see cref="ulong"/> value of the new <see cref="DBValue"/>.
        /// </param>
        public DBValue(ulong value) {
            Datum = value;
        }

        /// <summary>
        ///   Constructs a new <see cref="DBValue"/> from a <see cref="long"/>.
        /// </summary>
        /// <param name="value">
        ///   The <see cref="long"/> value of the new <see cref="DBValue"/>.
        /// </param>
        public DBValue(long value) {
            Datum = value;
        }

        /// <summary>
        ///   Constructs a new <see cref="DBValue"/> from a <see cref="float"/>.
        /// </summary>
        /// <param name="value">
        ///   The <see cref="float"/> value of the new <see cref="DBValue"/>.
        /// </param>
        public DBValue(float value) {
            Datum = value;
        }

        /// <summary>
        ///   Constructs a new <see cref="DBValue"/> from a <see cref="double"/>.
        /// </summary>
        /// <param name="value">
        ///   The <see cref="double"/> value of the new <see cref="DBValue"/>.
        /// </param>
        public DBValue(double value) {
            Datum = value;
        }

        /// <summary>
        ///   Constructs a new <see cref="DBValue"/> from a <see cref="decimal"/>.
        /// </summary>
        /// <param name="value">
        ///   The <see cref="decimal"/> value of the new <see cref="DBValue"/>.
        /// </param>
        public DBValue(decimal value) {
            Datum = value;
        }

        /// <summary>
        ///   Constructs a new <see cref="DBValue"/> from a <see cref="DateOnly"/>.
        /// </summary>
        /// <param name="value">
        ///   The <see cref="DateOnly"/> value of the new <see cref="DBValue"/>.
        /// </param>
        public DBValue(DateOnly value) {
            Datum = value;
        }

        /// <summary>
        ///   Constructs a new <see cref="DBValue"/> from a <see cref="DateTime"/>.
        /// </summary>
        /// <param name="value">
        ///   The <see cref="DateTime"/> value of the new <see cref="DBValue"/>.
        /// </param>
        public DBValue(DateTime value) {
            Datum = value;
        }

        /// <summary>
        ///   Constructs a new <see cref="DBValue"/> from a <see cref="char"/>.
        /// </summary>
        /// <param name="value">
        ///   The <see cref="char"/> value of the new <see cref="DBValue"/>.
        /// </param>
        public DBValue(char value) {
            Datum = value;
        }

        /// <summary>
        ///   Constructs a new <see cref="DBValue"/> from a <see cref="string"/>.
        /// </summary>
        /// <param name="value">
        ///   The <see cref="string"/> value of the new <see cref="DBValue"/>.
        /// </param>
        public DBValue(string value) {
            Guard.Against.Null(value, nameof(value));
            Datum = value;
        }

        /// <summary>
        ///   Constructs a new <see cref="DBValue"/> from a <see cref="Guid"/>.
        /// </summary>
        /// <param name="value">
        ///   The <see cref="Guid"/> value of the new <see cref="DBValue"/>.
        /// </param>
        public DBValue(Guid value) {
            Datum = value;
        }

        /// <summary>
        ///   Creates a new <see cref="DBValue"/> from an arbitrary value.
        /// </summary>
        /// <param name="value">
        ///   The value of the new <see cref="DBValue"/>.
        /// </param>
        /// <exception cref="ArgumentException">
        ///   if <paramref name="value"/> is non-<see langword="null"/> and of a type that is not supported by Kvasir.
        /// </exception>
        public static DBValue Create(object? value) {
            // We'll use dynamic here so that the compiler is responsible for deducing the runtime type of the argument
            // and finding the appropriate constructor overload. This is not only easier (and less error-prone) than
            // doing it by hand, but it is also forward-compatible with respect to the addition of constructors for
            // new supported types. The performance penalty incurred by using dynamic over traditional switch-on-type
            // is unmeasured.

            try {
                dynamic d = value ?? SENTINEL;
                return new DBValue(d);
            }
            catch (Exception ex) {
                var msg = $"Cannot create {nameof(DBValue)} from instance of unsupported type {value!.GetType()}";
                throw new ArgumentException(msg, nameof(value), ex);
            }
        }

        /// <summary>
        ///   Determines if the current <see cref="DBValue"/> would be valid for a Field with a specific data type,
        ///   considering only the data type and not the actual value.
        /// </summary>
        /// <param name="type">
        ///   The target <see cref="DBType"/>.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if this is <see cref="NULL"/> or if the type of this <see cref="DBValue"/> is
        ///   compatible with <paramref name="type"/>.
        /// </returns>
        public readonly bool IsInstanceOf(DBType type) {
            bool isNumeric =
                type == DBType.Int8 || type == DBType.Int16 || type == DBType.Int32 || type == DBType.Int64 ||
                type == DBType.UInt8 || type == DBType.UInt16 || type == DBType.UInt32 || type == DBType.UInt64 ||
                type == DBType.Single || type == DBType.Double;

            if (Equals(NULL)) {
                return true;
            }
            else if (type == DBType.Enumeration) {
                return Datum.GetType() == typeof(string);
            }
            else if (isNumeric && Equals(Datum, 0)) {
                return true;
            }
            else {
                return type == DBType.Lookup(Datum.GetType());
            }
        }

        /// <summary>
        ///   Determines if this <see cref="DBValue"/> is equal to another.
        /// </summary>
        /// <param name="rhs">
        ///   The <see cref="DBValue"/> against which to compare this one.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if <paramref name="rhs"/> is equal to this <see cref="DBValue"/>; otherwise,
        ///   <see langword="false"/>.
        /// </returns>
        public readonly bool Equals(DBValue rhs) {
            return Datum.Equals(rhs.Datum);
        }

        /// <summary>
        ///   Determines if this <see cref="DBValue"/> is equal to another <see cref="object"/>.
        /// </summary>
        /// <param name="rhs">
        ///   The <see cref="object"/> against which to compare this <see cref="DBValue"/>.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if <paramref name="rhs"/> is a non-<see langword="null"/> <see cref="DBValue"/>
        ///   that is equal to this one; otherwise, <see langword="false"/>.
        /// </returns>
        public readonly override bool Equals(object? rhs) {
            return (rhs is DBValue dbv) && Equals(dbv);
        }

        /// <summary>
        ///   Produces the hash code for this <see cref="DBValue"/>.
        /// </summary>
        /// <returns>
        ///   The hash code for this <see cref="DBValue"/>.
        /// </returns>
        public readonly override int GetHashCode() {
            return HashCode.Combine(Datum);
        }

        /// <summary>
        ///   Produces a human-readable string representation of this <see cref="DBValue"/>.
        /// </summary>
        /// <returns>
        ///   A human-readable string representation of this <see cref="DBValue"/>.
        /// </returns>
        public readonly override string ToString() {
            if (Equals(NULL)) {
                return "NULL";
            }
            else if (Datum.GetType() == typeof(char)) {
                return $"'{Datum}'";
            }
            else if (Datum.GetType() == typeof(string)) {
                return $"\"{Datum}\"";
            }
            else {
                return Datum.ToString()!;
            }
        }

        /// <summary>
        ///   Determines if one <see cref="DBValue"/> is equal to another.
        /// </summary>
        /// <param name="lhs">
        ///   The first <see cref="DBValue"/>.
        /// </param>
        /// <param name="rhs">
        ///   The second <see cref="DBValue"/>.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if <paramref name="lhs"/> is equal to <paramref name="rhs"/>; otherwise,
        ///   <see langword="false"/>.
        /// </returns>
        public static bool operator==(DBValue lhs, DBValue rhs) {
            return lhs.Equals(rhs);
        }

        /// <summary>
        ///   Determines if one <see cref="DBValue"/> is not equal to another.
        /// </summary>
        /// <param name="lhs">
        ///   The first <see cref="DBValue"/>.
        /// </param>
        /// <param name="rhs">
        ///   The second <see cref="DBValue"/>.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if <paramref name="lhs"/> is not equal to <paramref name="rhs"/>; otherwise,
        ///   <see langword="false"/>.
        /// </returns>
        public static bool operator!=(DBValue lhs, DBValue rhs) {
            return !(lhs == rhs);
        }

        /// <summary>
        ///   Initializes the <see langword="static"/> state of the <see cref="DBValue"/> struct.
        /// </summary>
        static DBValue() {
            SENTINEL = DBNull.Value;
            NULL = new DBValue(SENTINEL);
        }

        /// <summary>
        ///   Constructs a new <see cref="DBValue"/> that represents <c>NULL</c>.
        /// </summary>
        private DBValue(DBNull _) {
            Datum = _;
        }


        private static readonly DBNull SENTINEL;
    }
}
