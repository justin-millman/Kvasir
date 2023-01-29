using Ardalis.GuardClauses;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Cybele.Core {
    /// <summary>
    ///   A strongly typed piece of text for representing specific domain concepts.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     The <see cref="ConceptString{TConcept}"/> is designed to be a replacement for <see cref="string"/> in APIs
    ///     where the use of a primitive would otherwise belie a specific domain concept. The class is intended for use
    ///     at non-boundary points to encapsulate specific validation rules, increase readability, and avoid subtle
    ///     bugs that can be introduced by mixing up <see cref="string"/> arguments in function calls. At library and
    ///     application boundaries, a <see cref="ConceptString{TConcept}"/> can be explicitly converted back into the
    ///     raw underlying <see cref="string"/>. By using a <see cref="ConceptString{TConcept}"/>, a API signals to the
    ///     caller that the text has a specific semantic meaning and forces the caller to acknowledge any associated
    ///     requirements.
    ///   </para>
    ///   <para>
    ///     The API exposed by <see cref="ConceptString{TConcept}"/> is intentionally minimal, covering only the barest
    ///     functionality of a character sequence. Equality is performed using a case-sensitive byte-wise comparison of
    ///     characters, while ordering comparisons are made lexicographically. The characters of a
    ///     <see cref="ConceptString{TConcept}"/> can be accessed by index and iterated over, while substrings can be
    ///     obtained for particular endpoints. Concrete derived classes are encouraged to provide more
    ///     domain-appropriate versions of equality and comparison (such as supporting specification of a
    ///     <see cref="StringComparer"/>) and to expose additional functions on top of the base API. Additionally,
    ///     derived classes can restrict the contents of a <see cref="ConceptString{TConcept}"/> by performing
    ///     validity checks in the constructor; the base <see cref="ConceptString{TConcept}"/> imposes no such
    ///     restrictions on the raw string.
    ///   </para>
    /// </remarks>
    public abstract class ConceptString<TConcept> : IEnumerable<char>, IEquatable<ConceptString<TConcept>>,
        IComparable<ConceptString<TConcept>> where TConcept : ConceptString<TConcept> {

        /// <summary>
        ///   The number of characters in this <see cref="ConceptString{TConcept}"/>.
        /// </summary>
        public int Length => Contents.Length;

        /// <summary>
        ///   The character at a particular index in this <see cref="ConceptString{TConcept}"/>.
        /// </summary>
        /// <param name="index">
        ///   The <c>0</c>-based index of the target character.
        /// </param>
        /// <exception cref="IndexOutOfRangeException">
        ///   if <paramref name="index"/> is less than <c>0</c>
        ///     --or--
        ///   if <paramref name="index"/> is greater than or equal to <see cref="Length"/>.
        /// </exception>
        public char this[int index] => Contents[index];

        /// <summary>
        ///   An immutable view over a substring of this <see cref="ConceptString{TConcept}"/>.
        /// </summary>
        /// <param name="range">
        ///   The endpoints of the target substring.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   if the beginning index of <paramref name="range"/> is less than <c>0</c>
        ///     --or--
        ///   if the ending index of <paramref name="range"/> is greater than or equal to <see cref="Length"/>.
        /// </exception>
        public ReadOnlySpan<char> this[Range range] => Contents[range];

        /// <summary>
        ///   An immutable view over the contents of this string.
        /// </summary>
        public ReadOnlySpan<char> View => Contents;

        /// <summary>
        ///   Constructs a new <see cref="ConceptString{TConcept}"/>.
        /// </summary>
        /// <param name="contents">
        ///   The raw contents of the new <see cref="ConceptString{TConcept}"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   if <paramref name="contents"/> is <see langword="null"/>.
        /// </exception>
        public ConceptString(string contents) {
            Guard.Against.Null(contents, nameof(contents));
            Contents = contents;
        }

        /// <summary>
        ///   Implicitly converts this <see cref="ConceptString{TConcept}"/> to a
        ///   <see cref="ReadOnlySpan{T}">ReadOnlySpan&lt;char&gt;</see>.
        /// </summary>
        /// <param name="concept">
        ///   The <see cref="ConceptString{TConcept}"/> to convert.
        /// </param>
        /// <returns>
        ///   A read-only view over <paramref name="concept"/> if <paramref name="concept"/> is not
        ///   <see langword="null"/>; otherwise, a read-only view over nothing.
        /// </returns>
        public static implicit operator ReadOnlySpan<char>(ConceptString<TConcept>? concept) {
            if (concept is null) {
                return default;
            }
            return concept.View;
        }

        /// <summary>
        ///   Explicitly converts this <see cref="ConceptString{TConcept}"/> to a raw <see cref="string"/>.
        /// </summary>
        /// <param name="concept">
        ///   The <see cref="ConceptString{TConcept}"/> to convert.
        /// </param>
        /// <returns>
        ///   If <paramref name="concept"/> is not <see langword="null"/>, a <see cref="string"/> whose contents are
        ///   identical to the contents of <paramref name="concept"/>; otherwise, a <see langword="null"/>
        ///   <see cref="string"/>.
        /// </returns>
        public static explicit operator string?(ConceptString<TConcept>? concept) {
            return concept?.Contents;
        }

        /// <summary>
        ///   Determines if this <see cref="ConceptString{TConcept}"/> is equal to another.
        /// </summary>
        /// <param name="rhs">
        ///   The <see cref="ConceptString{TConcept}"/> against which to compare this one.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if <paramref name="rhs"/> is equal to this <see cref="ConceptString{TConcept}"/>;
        ///   otherwise, <see langword="false"/>.
        /// </returns>
        public bool Equals(ConceptString<TConcept>? rhs) {
            return Contents == rhs?.Contents;
        }

        /// <summary>
        ///   Determines if this <see cref="ConceptString{TConcept}"/> is equal to another <see cref="object"/>.
        /// </summary>
        /// <param name="rhs">
        ///   The <see cref="object"/> against which to compare this <see cref="ConceptString{TConcept}"/>.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if <paramref name="rhs"/> is a non-<see langword="null"/>
        ///   <see cref="ConceptString{TConcept}"/> that is equal to this one; otherwise, <see langword="false"/>.
        /// </returns>
        public sealed override bool Equals(object? rhs) {
            return (rhs is ConceptString<TConcept> cs) && Equals(cs);
        }

        /// <summary>
        ///   Performs a three-way comparison between this <see cref="ConceptString{TConcept}"/> and another.
        /// </summary>
        /// <param name="rhs">
        ///   The <see cref="ConceptString{TConcept}"/> against which to compare this one.
        /// </param>
        /// <returns>
        ///   An unspecified negative integer if this <see cref="ConceptString{TConcept}"/> is less than
        ///   <paramref name="rhs"/>, <c>0</c> if this <see cref="ConceptString{TConcept}"/> is equal to
        ///   <paramref name="rhs"/>, and an unspecified positive integer if this <see cref="ConceptString{TConcept}"/>
        ///   is greater than <paramref name="rhs"/>. A <see langword="null"/> <see cref="ConceptString{TConcept}"/>
        ///   compares less than all others.
        /// </returns>
        public int CompareTo(ConceptString<TConcept>? rhs) {
            // We won't use the general-purpose static helper method here because we know that the left-hand operand
            // (this) is not null, so we avoid a tautologically redundant check
            return Contents.CompareTo(rhs?.Contents);
        }

        /// <summary>
        ///   Produces the hash code for this <see cref="ConceptString{TConcept}"/>.
        /// </summary>
        /// <returns>
        ///   The hash code for this <see cref="ConceptString{TConcept}"/>.
        /// </returns>
        public sealed override int GetHashCode() {
            return HashCode.Combine(Contents);
        }

        /// <summary>
        ///   Produces a human-readable string representation of this <see cref="ConceptString{TConcept}"/>.
        /// </summary>
        /// <returns>
        ///   A human-readable string representation of this <see cref="ConceptString{TConcept}"/>.
        /// </returns>
        public sealed override string ToString() {
            return Contents;
        }

        /// <summary>
        ///   Determines if one <see cref="ConceptString{TConcept}"/> is equal to another.
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
        ///   Determines if one <see cref="ConceptString{TConcept}"/> is not equal to another.
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
        ///   Determines if one <see cref="ConceptString{TConcept}"/> is lexicographically less than another.
        /// </summary>
        /// <param name="lhs">
        ///   The first <see cref="ConceptString{TConcept}"/>.
        /// </param>
        /// <param name="rhs">
        ///   The second <see cref="ConceptString{TConcept}"/>.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if <paramref name="lhs"/> is lexicographially less than <paramref name="rhs"/>;
        ///   otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator<(ConceptString<TConcept>? lhs, ConceptString<TConcept>? rhs) {
            return Compare(lhs, rhs) < 0;
        }

        /// <summary>
        ///   Determines if one <see cref="ConceptString{TConcept}"/> is lexicographically greater than another.
        /// </summary>
        /// <param name="lhs">
        ///   The first <see cref="ConceptString{TConcept}"/>.
        /// </param>
        /// <param name="rhs">
        ///   The second <see cref="ConceptString{TConcept}"/>.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if <paramref name="lhs"/> is lexicographially greater than <paramref name="rhs"/>;
        ///   otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator>(ConceptString<TConcept>? lhs, ConceptString<TConcept>? rhs) {
            return Compare(lhs, rhs) > 0;
        }

        /// <summary>
        ///   Determines if one <see cref="ConceptString{TConcept}"/> is lexicographically less than or equal to
        ///   another.
        /// </summary>
        /// <param name="lhs">
        ///   The first <see cref="ConceptString{TConcept}"/>.
        /// </param>
        /// <param name="rhs">
        ///   The second <see cref="ConceptString{TConcept}"/>.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if <paramref name="lhs"/> is lexicographially less than or equal to
        ///   <paramref name="rhs"/>; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator<=(ConceptString<TConcept>? lhs, ConceptString<TConcept>? rhs) {
            return Compare(lhs, rhs) <= 0;
        }

        /// <summary>
        ///   Determines if one <see cref="ConceptString{TConcept}"/> is lexicographically greater than or equal to
        ///   another.
        /// </summary>
        /// <param name="lhs">
        ///   The first <see cref="ConceptString{TConcept}"/>.
        /// </param>
        /// <param name="rhs">
        ///   The second <see cref="ConceptString{TConcept}"/>.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if <paramref name="lhs"/> is lexicographially greater than or equal to
        ///   <paramref name="rhs"/>; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator>=(ConceptString<TConcept>? lhs, ConceptString<TConcept>? rhs) {
            return Compare(lhs, rhs) >= 0;
        }

        /// <summary>
        ///   Produces an enumerator that iterates over the characters in this <see cref="ConceptString{TConcept}"/>.
        /// </summary>
        /// <returns>
        ///   An enumerator that iterates over the characters in this <see cref="ConceptString{TConcept}"/>.
        /// </returns>
        public IEnumerator<char> GetEnumerator() {
            return Contents.GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        /// <summary>
        ///   The raw contents of this <see cref="ConceptString{TConcept}"/>.
        /// </summary>
        /// <remarks>
        ///   This property is exposed to derived classes so that concrete <see cref="ConceptString{TConcept}"/> types
        ///   can provide specialized equality and/or comparison methods, expose additional domain-relevant APIs, or
        ///   perform validation on construction.
        /// </remarks>
        protected string Contents { get; }

        /// <summary>
        ///   Performs a three-way comparison between two <see cref="ConceptString{TConcept}">ConceptStrings</see>.
        /// </summary>
        /// <param name="lhs">
        ///   The first <see cref="ConceptString{TConcept}"/>.
        /// </param>
        /// <param name="rhs">
        ///   The second <see cref="ConceptString{TConcept}"/>.
        /// </param>
        /// <returns>
        ///   An unspecified negative integer if <paramref name="lhs"/> is less than <paramref name="rhs"/>, <c>0</c>
        ///   if <paramref name="lhs"/> is equal to <paramref name="rhs"/>, and an unspecified positive integer if
        ///   <paramref name="lhs"/> is greater than <paramref name="rhs"/>. A <see langword="null"/>
        ///   <see cref="ConceptString{TConcept}"/> compares equal to another <see langword="null"/> instance and less
        ///   than any non-<see langword="null"/> one.
        /// </returns>
        private static int Compare(ConceptString<TConcept>? lhs, ConceptString<TConcept>? rhs) {
            if (lhs is null) {
                return rhs is null ? 0 : -1;
            }
            return lhs.Contents.CompareTo(rhs?.Contents);
        }
    }
}
