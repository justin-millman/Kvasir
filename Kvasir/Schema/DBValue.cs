using System;

namespace Kvasir.Schema {
    /// <summary>
    ///   A thin wrapper around a CLR object that can be stored in a back-end relational database.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     The <see cref="DBValue"/> class is intended to provide an abstraction over the generalized CLR
    ///     <see  cref="object"/> for use in contexts where compile-time type information is not available. The
    ///     advantage to using a <see cref="DBValue"/> in APIs rather than <see cref="object"/> is the guarantee that a
    ///     <see cref="DBValue"/> only ever wraps an objec that can validly be stored into a database, whereas an
    ///     <see cref="object"/> could feasibly hold an object of any type. Additionally, the wrapped value is never
    ///     a <see langword="null"/> reference: a sentinel value is used to indicate <see langword="null"/>, mitigating
    ///     the need for verbose checks.
    ///   </para>
    ///   <para>
    ///     The strong typing afforded by CLR <see cref="Enum">enums</see> is lost when the enumerator values are
    ///     stored in a back-end relational database. Some providers afford value checking, either through an explicit
    ///     <c>enum</c> data type or via <c>CHECK</c> constraints; however, even in these cases, the actual storage is
    ///     generally an integer or a string. For this reason, an instance of <see cref="DBValue"/> will never wrap a
    ///     CLR enumerator: it is the responsibility of the user to convert the CLR object into the corresponding
    ///     database object, be it an integer or string or other supported type.
    ///   </para>
    ///   <para>
    ///     The same general logic applies to any transformations applied between the CLR domain and the database
    ///     domain. The <see cref="DBValue"/> class is intended to exactly wrap the object that is stored, and as such
    ///     it is the responsibility of the user to apply any necessary transforms prior to creating the
    ///     <see cref="DBValue"/> object.
    ///   </para>
    ///   <para>
    ///     Because of the possibility that a <see cref="DBValue"/> instance wraps a sentinel indicating
    ///     <see langword="null"/>, it is not strictly possible to determine the <see cref="DBType"/> of a particular
    ///     instance. To check if a <see cref="DBValue"/> is valid for a Field of a particular <see cref="DBType"/>,
    ///     use the <see cref="DBType.IsValidValue(DBValue)"/> query method.
    ///   </para>
    /// </remarks>
    public readonly struct DBValue : IEquatable<DBValue> {
        /// <value>
        ///   The <see cref="object"/> wrapped by this <see cref="DBValue"/>.
        /// </value>
        public readonly object Datum { get; }

        /// <value>
        ///   A sentinel <see cref="DBValue"/> instance representing the CLR concept of <see langword="null"/>.
        /// </value>
        public static DBValue NULL { get; } = new DBValue(SENTINEL!);

        /// <summary>
        ///   Constructs a new <see cref="DBValue"/> that wraps a <see cref="bool"/>.
        /// </summary>
        /// <param name="value">
        ///   The <see cref="bool"/> to be wrapped by the new <see cref="DBValue"/>.
        /// </param>
        public DBValue(bool value) {
            Datum = value;
        }

        /// <summary>
        ///   Constructs a new <see cref="DBValue"/> that wraps a <see cref="byte"/>.
        /// </summary>
        /// <param name="value">
        ///   The <see cref="byte"/> to be wrapped by the new <see cref="DBValue"/>.
        /// </param>
        public DBValue(byte value) {
            Datum = value;
        }

        /// <summary>
        ///   Constructs a new <see cref="DBValue"/> that wraps a <see cref="sbyte"/>.
        /// </summary>
        /// <param name="value">
        ///   The <see cref="sbyte"/> to be wrapped by the new <see cref="DBValue"/>.
        /// </param>
        public DBValue(sbyte value) {
            Datum = value;
        }

        /// <summary>
        ///   Constructs a new <see cref="DBValue"/> that wraps a <see cref="ushort"/>.
        /// </summary>
        /// <param name="value">
        ///   The <see cref="ushort"/> to be wrapped by the new <see cref="DBValue"/>.
        /// </param>
        public DBValue(ushort value) {
            Datum = value;
        }

        /// <summary>
        ///   Constructs a new <see cref="DBValue"/> that wraps a <see cref="short"/>.
        /// </summary>
        /// <param name="value">
        ///   The <see cref="short"/> to be wrapped by the new <see cref="DBValue"/>.
        /// </param>
        public DBValue(short value) {
            Datum = value;
        }

        /// <summary>
        ///   Constructs a new <see cref="DBValue"/> that wraps a <see cref="uint"/>.
        /// </summary>
        /// <param name="value">
        ///   The <see cref="uint"/> to be wrapped by the new <see cref="DBValue"/>.
        /// </param>
        public DBValue(uint value) {
            Datum = value;
        }

        /// <summary>
        ///   Constructs a new <see cref="DBValue"/> that wraps a <see cref="int"/>.
        /// </summary>
        /// <param name="value">
        ///   The <see cref="int"/> to be wrapped by the new <see cref="DBValue"/>.
        /// </param>
        public DBValue(int value) {
            Datum = value;
        }

        /// <summary>
        ///   Constructs a new <see cref="DBValue"/> that wraps a <see cref="ulong"/>.
        /// </summary>
        /// <param name="value">
        ///   The <see cref="ulong"/> to be wrapped by the new <see cref="DBValue"/>.
        /// </param>
        public DBValue(ulong value) {
            Datum = value;
        }

        /// <summary>
        ///   Constructs a new <see cref="DBValue"/> that wraps a <see cref="long"/>.
        /// </summary>
        /// <param name="value">
        ///   The <see cref="long"/> to be wrapped by the new <see cref="DBValue"/>.
        /// </param>
        public DBValue(long value) {
            Datum = value;
        }

        /// <summary>
        ///   Constructs a new <see cref="DBValue"/> that wraps a <see cref="float"/>.
        /// </summary>
        /// <param name="value">
        ///   The <see cref="float"/> to be wrapped by the new <see cref="DBValue"/>.
        /// </param>
        public DBValue(float value) {
            Datum = value;
        }

        /// <summary>
        ///   Constructs a new <see cref="DBValue"/> that wraps a <see cref="double"/>.
        /// </summary>
        /// <param name="value">
        ///   The <see cref="double"/> to be wrapped by the new <see cref="DBValue"/>.
        /// </param>
        public DBValue(double value) {
            Datum = value;
        }

        /// <summary>
        ///   Constructs a new <see cref="DBValue"/> that wraps a <see cref="decimal"/>.
        /// </summary>
        /// <param name="value">
        ///   The <see cref="decimal"/> to be wrapped by the new <see cref="DBValue"/>.
        /// </param>
        public DBValue(decimal value) {
            Datum = value;
        }

        /// <summary>
        ///   Constructs a new <see cref="DBValue"/> that wraps a <see cref="DateTime"/>.
        /// </summary>
        /// <param name="value">
        ///   The <see cref="DateTime"/> to be wrapped by the new <see cref="DBValue"/>.
        /// </param>
        public DBValue(DateTime value) {
            Datum = value;
        }

        /// <summary>
        ///   Constructs a new <see cref="DBValue"/> that wraps a <see cref="char"/>.
        /// </summary>
        /// <param name="value">
        ///   The <see cref="char"/> to be wrapped by the new <see cref="DBValue"/>.
        /// </param>
        public DBValue(char value) {
            Datum = value;
        }

        /// <summary>
        ///   Constructs a new <see cref="DBValue"/> that wraps a <see cref="string"/>.
        /// </summary>
        /// <param name="value">
        ///   The <see cref="string"/> to be wrapped by the new <see cref="DBValue"/>.
        /// </param>
        public DBValue(string value) {
            Datum = value;
        }

        /// <summary>
        ///   Constructs a new <see cref="DBValue"/> that wraps a <see cref="Guid"/>.
        /// </summary>
        /// <param name="value">
        ///   The <see cref="Guid"/> to be wrapped by the new <see cref="DBValue"/>.
        /// </param>
        public DBValue(Guid value) {
            Datum = value;
        }

        /// <summary>
        ///   Creates a new <see cref="DBValue"/> that wraps an <see cref="object"/> whose compile-time type is not
        ///   known.
        /// </summary>
        /// <param name="value">
        ///   The <see cref="object"/> to be wrapped by the new <see cref="DBValue"/>.
        /// </param>
        /// <exception cref="ArgumentException">
        ///   if the CLR type of <paramref name="value"/> is not one for which there is an available strongly typed
        ///   constructor, and <paramref name="value"/> cannot be implicitly converted to such a type.
        /// </exception>
        public static DBValue Create(object? value) {
            // We'll use dynamic here so that the compiler is responsible for doing the type checking. This is both
            // easier than doing it manually and more extensible if/when support for additional types is added in the
            // future. The downside is a slight performance penalty, which we've decided to accept.
            try {
                dynamic d = value ?? SENTINEL;
                return new DBValue(d);
            }
            catch (Exception ex) {
                var msg = $"Cannot create {nameof(DBValue)} from instance of unsupported type {value!.GetType()}";
                throw new ArgumentException(msg, ex);
            }
        }

        /// <summary>
        ///   Determines whether this <see cref="DBValue"/> is equal to another <see cref="DBValue"/>.
        /// </summary>
        /// <param name="rhs">
        ///   The <see cref="DBValue"/> against which to compare this one for equality.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if this <see cref="DBValue"/> is equal to <paramref name="rhs"/>; otherwise,
        ///   <see langword="false"/>.
        /// </returns>
        public readonly bool Equals(DBValue rhs) {
            return Datum.Equals(rhs.Datum);
        }

        /// <summary>
        ///   Determines whether this <see cref="DBValue"/> is equal to another <see cref="object"/>.
        /// </summary>
        /// <param name="rhs">
        ///   The <see cref="object"/> against which to compare this one for equality.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if <paramref name="rhs"/> is a <see cref="DBValue"/> that is equal to this
        ///   <see cref="DBValue"/>; otherwise, <see langword="false"/>.
        /// </returns>
        public readonly override bool Equals(object? rhs) {
            return (rhs is DBValue dbv) && Equals(dbv);
        }

        /// <summary>
        ///   Produces the hash code for this <see cref="DBValue"/>.
        /// </summary>
        /// <returns>
        ///   A <c>32</c>-bit signed integer that is the hash code for this <see cref="DBValue"/>.
        /// </returns>
        public readonly override int GetHashCode() {
            return Datum.GetHashCode();
        }

        /// <summary>
        ///   Produces a string representation of this <see cref="DBValue"/>.
        /// </summary>
        /// <returns>
        ///   A string representation of this <see cref="DBValue"/>.
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
        ///   Determines if two <see cref="DBValue">DBValues</see> are equal.
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
        ///   Determiens if two <see cref="DBValue">DBValues</see> are not equal.
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
        ///   Constructs a new <see cref="DBValue"/> that wraps the <c>NULL</c> sentinel.
        /// </summary>
        private DBValue(DBNull _) {
            Datum = DBNull.Value;
        }


        private static readonly DBNull SENTINEL = DBNull.Value;
    }
}
