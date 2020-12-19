using System;
using System.Collections;
using System.Collections.Generic;

namespace Cybele {
    /// <summary>
    ///   A strongly typed <see cref="string"/> class that represents a specific application or library domain concept.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     The <see cref="ConceptString{TConcept}"/> class is an abstract base class intended to assist in modeling
    ///     application or library domain concepts whose intrinsic data form is a sequence of characters. This utility
    ///     is designed to afford developers an expressive and type-safe way to design APIs (for both internal and/or
    ///     external use) that do not suffer from "primitive obsession." This utility can thusly be used to
    ///     disambiguate APIs that would otherwise expect a sequence of <see cref="string">strings</see>, provide
    ///     return type clarity, and/or encapsulate more intricate domain validation logic.
    ///   </para>
    ///   <para>
    ///     To prevent escaping out of the strong typing afforded by <see cref="ConceptString{TConcept}"/>, there is
    ///     neither an implicit conversion to nor one from <see cref="string"/>. To interoperate with APIs expecting a
    ///     raw <see cref="string"/> instance, a one-way <i>explicit</i> conversion operator is available. It is
    ///     recommended that this conversion be used only at API boundaries rather than within the domain of a single
    ///     application or library. A separate one-way implicit conversion from <see cref="ConceptString{TConcept}"/>
    ///     to <see cref="ReadOnlySpan{T}">ReadOnlySpan&lt;char&gt;</see> is provided for easy interoperation with the
    ///     .NET standard library.
    ///   </para>
    ///   <para>
    ///     As with <see cref="string"/>, <see cref="ConceptString{TConcept}"/> offers native equality operations, both
    ///     in the form of the <see cref="IEquatable{T}"/> interface and standard binary operators. The default
    ///     behavior of the <see cref="IEquatable{T}.Equals(T)"/> method is to evaluate using an ordinal,
    ///     case-sensitive comparison; however, this method is intentionally <see langword="virtual"/>, affording
    ///     derived types the opportunity to provide a more appropriate implementation. This same pattern extends to
    ///     ordering comparisons through the <see cref="IComparable{T}"/> interface. As per usual, a
    ///     <see langword="null"/> <see cref="ConceptString{TConcept}"/> compares equal/equivalent to other
    ///     <see langword="null"/> instances and compares less than any non-<see langword="null"/> ones.
    ///   </para>
    ///   <para>
    ///     Aside from equality and ordering comparisons, the <see cref="ConceptString{TConcept}"/> class does not
    ///     provide much in the way of an API. There are no modification methods, no concatenation operator, no static
    ///     comparison overloads, etc. The minimal API provided by <see cref="ConceptString{TConcept}"/> is intended to
    ///     be the API that is universally desired; concatenation may not make sense for all domain concepts, so it is
    ///     not provided by default. Derived classes are encouraged to implement additional member functions that
    ///     harmonize with the particular domain concept(s) being modeled.
    ///   </para>
    /// </remarks>
    public abstract class ConceptString<TConcept> : IEnumerable<char>, IEquatable<ConceptString<TConcept>>,
        IComparable<ConceptString<TConcept>> where TConcept : ConceptString<TConcept> {

        /// <value>
        ///   The number of characters in this <see cref="ConceptString{TConcept}"/>.
        /// </value>
        public int Length => contents_.Length;

        /// <value>
        ///   The character in this <see cref="ConceptString{TConcept}"/> at a particular position.
        /// </value>
        /// <param name="index">
        ///   The <c>0</c>-based index of the target character.
        /// </param>
        /// <exception cref="IndexOutOfRangeException">
        ///   if <c><paramref name="index"/> &lt; 0</c> or <c><paramref name="index"/> &gt;= <see cref="Length"/></c>.
        /// </exception>
        public char this[int index] => contents_[index];

        /// <value>
        ///   A read-only, non-allocating view into the contents of this <see cref="ConceptString{TConcept}"/>.
        /// </value>
        public ReadOnlySpan<char> View => contents_;

        /// <summary>
        ///   Converts a <see cref="ConceptString{TConcept}"/> into a
        ///   <see cref="ReadOnlySpan{T}">ReadOnlySpan&lt;char&gt;</see>.
        /// </summary>
        /// <param name="concept">
        ///   The <see cref="ConceptString{TConcept}"/> to convert.
        /// </param>
        /// <returns>
        ///   A read-only, non-allocating view into the contents of <paramref name="concept"/>, or a
        ///   default-constructed <see cref="ReadOnlySpan{T}">ReadOnlySpan&lt;char&gt;</see> if
        ///   <paramref name="concept"/> is <see langword="null"/>.
        /// </returns>
        /// <seealso cref="View"/>
        public static implicit operator ReadOnlySpan<char>(ConceptString<TConcept>? concept) {
            if (concept is null) {
                return default;
            }
            return concept.View;
        }

        /// <summary>
        ///   Converts a <see cref="ConceptString{TConcept}"/> into a raw <see cref="string"/>.
        /// </summary>
        /// <param name="concept">
        ///   The <see cref="ConceptString{TConcept}"/> to convert.
        /// </param>
        /// <returns>
        ///   The raw contents of <paramref name="concept"/>, or <see langword="null"/> if <paramref name="concept"/>
        ///   is <see langword="null"/>.
        /// </returns>
        public static explicit operator string?(ConceptString<TConcept>? concept) {
            return concept?.contents_;
        }

        /// <summary>
        ///   Constructs a new <see cref="ConceptString{TConcept}"/>.
        /// </summary>
        /// <param name="contents">
        ///   The raw contents of the new <see cref="ConceptString{TConcept}"/>.
        /// </param>
        public ConceptString(string contents) {
            contents_ = contents;
        }

        /// <summary>
        ///   Determines whether this <see cref="ConceptString{TConcept}"/> is equal to another
        ///   <see cref="ConceptString{TConcept}"/>.
        /// </summary>
        /// <param name="rhs">
        ///   The <see cref="ConceptString{TConcept}"/> against which to compare this one for equality.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if this <see cref="ConceptString{TConcept}"/> is equal to <paramref name="rhs"/>
        ///   using an ordinal, case-sensitive comparison; otherwise, <see langword="false"/>.
        /// </returns>
        public virtual bool Equals(ConceptString<TConcept>? rhs) {
            return contents_ == rhs?.contents_;
        }

        /// <summary>
        ///   Determines whether this <see cref="ConceptString{TConcept}"/> is equal to another <see cref="object"/>.
        /// </summary>
        /// <param name="rhs">
        ///   The <see cref="object"/> against which to compare this one for equality.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if <paramref name="rhs"/> is a <see cref="ConceptString{TConcept}"/> that is equal
        ///   to this <see cref="ConceptString{TConcept}"/>; otherwise, <see langword="false"/>.
        /// </returns>
        public sealed override bool Equals(object? rhs) {
            return (rhs is ConceptString<TConcept> cstr) && Equals(cstr);
        }

        /// <summary>
        ///   Determines the ordering of this <see cref="ConceptString{TConcept}"/> relative to another in the overall
        ///   total order.
        /// </summary>
        /// <param name="rhs">
        ///   The <see cref="ConceptString{TConcept}"/> against which to compare this one for relative ordering.
        /// </param>
        /// <returns>
        ///   A negative integer if this <see cref="ConceptString{TConcept}"/> comes before <paramref name="rhs"/> in
        ///   the overall total order, <c>0</c> if this <see cref="ConceptString{TConcept}"/> comes at the same
        ///   position as <paramref name="rhs"/> in the overall total order, and a positive integer if this
        ///   <see cref="ConceptString{TConcept}"/> comes after <paramref name="rhs"/> in the overall total order.
        /// </returns>
        public virtual int CompareTo(ConceptString<TConcept>? rhs) {
            return contents_.CompareTo(rhs?.contents_);
        }

        /// <summary>
        ///   Produces the hash code for this <see cref="ConceptString{TConcept}"/>.
        /// </summary>
        /// <returns>
        ///   A <c>32</c>-bit signed integer that is the hash code for this <see cref="ConceptString{TConcept}"/>.
        /// </returns>
        public sealed override int GetHashCode() {
            return contents_.GetHashCode();
        }

        /// <summary>
        ///   Produces a string representation of this <see cref="ConceptString{TConcept}"/>.
        /// </summary>
        /// <returns>
        ///   A string representation of this <see cref="ConceptString{TConcept}"/>.
        /// </returns>
        public sealed override string ToString() {
            return contents_;
        }

        /// <summary>
        ///   Determines if two <see cref="ConceptString{TConcept}">ConceptStrings</see> are equal.
        /// </summary>
        /// <param name="lhs">
        ///   The first <see cref="ConceptString{TConcept}"/>.
        /// </param>
        /// <param name="rhs">
        ///   The second <see cref="ConceptString{TConcept}"/>.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if <paramref name="lhs"/> is equal to <paramref name="rhs"/>; otherwise,
        ///   <see langword="false"/>.
        /// </returns>
        public static bool operator==(ConceptString<TConcept>? lhs, ConceptString<TConcept>? rhs) {
            if (lhs is null) {
                return rhs is null;
            }
            return lhs.Equals(rhs);
        }

        /// <summary>
        ///   Determiens if two <see cref="ConceptString{TConcept}">ConceptStrings</see> are not equal.
        /// </summary>
        /// <param name="lhs">
        ///   The first <see cref="ConceptString{TConcept}"/>.
        /// </param>
        /// <param name="rhs">
        ///   The second <see cref="ConceptString{TConcept}"/>.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if <paramref name="lhs"/> is not equal to <paramref name="rhs"/>; otherwise,
        ///   <see langword="false"/>.
        /// </returns>
        public static bool operator!=(ConceptString<TConcept>? lhs, ConceptString<TConcept>? rhs) {
            return !(lhs == rhs);
        }

        /// <summary>
        ///   Determines if one <see cref="ConceptString{TConcept}"/> is strictly less than another.
        /// </summary>
        /// <param name="lhs">
        ///   The left-hand operand of the ordering relation.
        /// </param>
        /// <param name="rhs">
        ///   The right-hand operand of the ordering relation.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if <paramref name="lhs"/> is strictly less than <paramref name="rhs"/>; otherwise,
        ///   <see langword="false"/>.
        /// </returns>
        public static bool operator<(ConceptString<TConcept>? lhs, ConceptString<TConcept>? rhs) {
            if (lhs is null) {
                return !(rhs is null);
            }
            return lhs.CompareTo(rhs) < 0;
        }

        /// <summary>
        ///   Determines if one <see cref="ConceptString{TConcept}"/> is strictly greater than another.
        /// </summary>
        /// <param name="lhs">
        ///   The left-hand operand of the ordering relation.
        /// </param>
        /// <param name="rhs">
        ///   The right-hand operand of the ordering relation.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if <paramref name="lhs"/> is strictly greater than <paramref name="rhs"/>;
        ///   otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator>(ConceptString<TConcept>? lhs, ConceptString<TConcept>? rhs) {
            if (lhs is null) {
                return false;
            }
            return lhs.CompareTo(rhs) > 0;
        }

        /// <summary>
        ///   Determines if one <see cref="ConceptString{TConcept}"/> is less than or equal to another.
        /// </summary>
        /// <param name="lhs">
        ///   The left-hand operand of the ordering relation.
        /// </param>
        /// <param name="rhs">
        ///   The right-hand operand of the ordering relation.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if <paramref name="lhs"/> is less than or equal to <paramref name="rhs"/>;
        ///   otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator<=(ConceptString<TConcept>? lhs, ConceptString<TConcept>? rhs) {
            if (lhs is null) {
                return true;
            }
            return lhs.CompareTo(rhs) <= 0;
        }

        /// <summary>
        ///   Determines if one <see cref="ConceptString{TConcept}"/> is greater than or equal to another.
        /// </summary>
        /// <param name="lhs">
        ///   The left-hand operand of the ordering relation.
        /// </param>
        /// <param name="rhs">
        ///   The right-hand operand of the ordering relation.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if <paramref name="lhs"/> is greater than or equal to <paramref name="rhs"/>;
        ///   otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator>=(ConceptString<TConcept>? lhs, ConceptString<TConcept>? rhs) {
            if (lhs is null) {
                return rhs is null;
            }
            return lhs.CompareTo(rhs) >= 0;
        }

        /// <summary>
        ///   Produces an enumerator that iterates over the characters in this <see cref="ConceptString{TConcept}"/>.
        /// </summary>
        /// <returns>
        ///   An enumerator that iterates over the characters in this <see cref="ConceptString{TConcept}"/>.
        /// </returns>
        public IEnumerator<char> GetEnumerator() {
            return contents_.GetEnumerator();
        }

        /// <summary>
        ///   Produces an enumerator that iterates over the characters in this <see cref="ConceptString{TConcept}"/> in
        ///   a type-erased manner.
        /// </summary>
        /// <returns>
        ///   An enumerator that iterates over the characters in this <see cref="ConceptString{TConcept}"/> in a
        ///   type-erased manner.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }


        private readonly string contents_;
    }
}
