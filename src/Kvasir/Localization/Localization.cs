using Ardalis.GuardClauses;
using Kvasir.Relations;
using System;
using System.Collections.Generic;

namespace Kvasir.Localization {
    /// <summary>
    ///   A collection of localized data that tracks the state of its locale-value mappings for interaction with a
    ///   back-end database.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     The <see cref="Localization{TKey, TLocale, TValue}"/> class is a first-class data structure in Kvasir for
    ///     representing semantics that have different values in different circumstances. The canonical example of this
    ///     is text, which changes according to language; other examples may be measurements in imperial versus metric
    ///     or dates according to different calendars. These modes are called "locales," and the value of a semantic in
    ///     a particular locale the "localized value" or "localization." The semantic being localized is known as the
    ///     "localization key" and is simply an identifier by which the localizations are grouped.
    ///   </para>
    ///   <para>
    ///     Every locale-value pair in a <see cref="Localization{TKey, TLocale, TValue}"/> is in one of three states:
    ///     <c>NEW</c>, <c>SAVED</c>, or <c>DELETED</c>. Each state corresponds to the action or actions that should be
    ///     taken with respect to that item to synchronize the back-end database table corresponding to the
    ///     Localization. An item enters the <c>NEW</c> state when it is first added; when the collection is
    ///     canonicalized, each <c>NEW</c> item transitions to the <c>SAVED</c> state, indicating that it does not need
    ///     to be written to the database on the next write. When a <c>SAVED</c> item is removed from the collection, it
    ///     transitions to the <c>DELETED</c> state; <c>NEW</c> items do not transition to <c>DELETED</c>. Note that if
    ///     a <c>SAVED</c> item is deleted and then re-added, it will be re-added in the <c>SAVED</c> state.
    ///   </para>
    ///   <para>
    ///     The <see cref="Localization{TKey, TLocale, TValue}"/> does not support the <c>MODIFIED</c> state; instead,
    ///     each locale-value pair is treated as its own unit. This means that changing the value localized for an
    ///     existing locale results in two separate operations in the back-end database: first a removal of the old
    ///     locale-value pair, then an insertion of the new one. This behavior <i>may</i> change in the future, so
    ///     clients should not rely on it. The guarantee, however, is that Kvasir will properly respond to an overwrite
    ///     by ensuring that the resulting back-end database contains only the new locale-value pair.
    ///   </para>
    ///   <para>
    ///     Items used for the value in a <see cref="Localization{TKey, TLocale, TValue}"/> should be immutable:
    ///     structs, <see cref="string"/>, etc. This is because read access is <i>not</i> tracked: when using mutable
    ///     values, it is possible for the user to access an item (e.g. through <c>operator[]</c>) and mutate that value
    ///     without the collection knowing, preventing the change from being reflected in the back-end database. This
    ///     also means that actions that convert the collection into another form will <i>copy</i> the elements,
    ///     ensuring that the tracing data remains up-to-date.
    ///   </para>
    ///   <para>
    ///     A <see cref="Localization{TKey, TLocale, TValue}"/> does not permit duplicate locales, but it does support
    ///     duplicate values.
    ///   </para>
    /// </remarks>
    /// <typeparam name="TKey">
    ///   The type of the "localization key" for the Localization.
    /// </typeparam>
    /// <typeparam name="TLocale">
    ///   The type of the locale for which values in the Localization are localized.
    /// </typeparam>
    /// <typeparam name="TValue">
    ///   The type of the localized values.
    /// </typeparam>
    public abstract class Localization<TKey, TLocale, TValue> : IEquatable<Localization<TKey, TLocale, TValue>>
        where TKey : notnull
        where TLocale : notnull {

        /// <summary>
        ///   The localization key of the Localization.
        /// </summary>
        public TKey Key { get; }

        /// <summary>
        ///   Gets or sets the localized value for a specified locale.
        /// </summary>
        /// <remarks>
        ///   By default, a <see cref="Localization{TKey, TLocale, TValue}"/> is read-only. This is to facilitate
        ///   read-only Localizations: if the base class's setter were public, then a hidden non-public setter in a
        ///   derived class could be circumvented by casting to the base class. Derived classes are enouraged to enable
        ///   writeability by providing a <c>new</c> implementation of the <c>this[]</c> operator and delegating to the
        ///   base class's protected implementation.
        /// </remarks>
        /// <param name="locale">
        ///   [GET] The locale of the localized value to return.
        ///   [SET] The locale of the new localized value.
        /// </param>
        /// <returns>
        ///   [GET] The value localized for <paramref name="locale"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        ///   [GET] If there is no localization for <paramref name="locale"/>.
        /// </exception>
        public TValue this[TLocale locale] {
            get {
                return Relation[locale];
            }
            protected set {
                Relation[locale] = value;
            }
        }

        /// <summary>
        ///   The change-tracked collection of localizations.
        /// </summary>
        /// <remarks>
        ///   This property is <c>internal</c> so that it is inaccessible to derived classes but accessible to Kvasir.
        ///   This is the property through which change tracking occurs, and therefore the property through which the
        ///   framework implements extraction and reconstitution. The <see cref="Localizations"/> property exposes the
        ///   same data in a more conventional collection interface.
        /// </remarks>
        internal RelationMap<TLocale, TValue> Relation { get; }

        /// <summary>
        ///   The collection of localized values, as a read-only mapping.
        /// </summary>
        public IReadOnlyDictionary<TLocale, TValue> Localizations => Relation;

        /// <summary>
        ///   Constructs a new <see cref="Localization{TKey, TLocale, TValue}"/>.
        /// </summary>
        /// <param name="localizationKey">
        ///   The <see cref="Key">localization key</see> of the new Localization.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   if <paramref name="localizationKey"/> is <see langword="null"/>.
        /// </exception>
        protected Localization(TKey localizationKey) {
            Guard.Against.Null(localizationKey);

            Key = localizationKey;
            Relation = [];
        }

        /// <summary>
        ///   Removes the localization entry for a particular locale
        /// </summary>
        /// <param name="locale">
        ///   The target locale.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if the element was successfully removed; otherwise, <see langword="false"/>. This
        ///   method also returns <see langword="false"/> if there is no localized value for <paramref name="locale"/>.
        /// </returns>
        protected bool RemoveLocalizationFor(TLocale locale) {
            return Relation.Remove(locale);
        }

        /// <inheritdoc/>
        public bool Equals(Localization<TKey, TLocale, TValue>? rhs) {
            if (rhs is null) {
                return false;
            }
            return Key.Equals(rhs.Key);
        }

        /// <inheritdoc/>
        public sealed override bool Equals(object? obj) {
            return (obj is Localization<TKey, TLocale, TValue> loc) && Equals(loc);
        }

        /// <inheritdoc/>
        public sealed override int GetHashCode() {
            return Key.GetHashCode();
        }

        /// <inheritdoc/>
        public sealed override string ToString() {
            return $"LOC[{typeof(TLocale).Name}:{typeof(TValue).Name}].{Key}";
        }

        /// <summary>
        ///   Equality Operator.
        /// </summary>
        /// <param name="lhs">
        ///   The left-hand operand.
        /// </param>
        /// <param name="rhs">
        ///   The right-hand operand.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if <paramref name="lhs"/> is equal to <paramref name="rhs"/>; otherwise,
        ///   <see langword="false"/>.
        /// </returns>
        /// <seealso cref="Equals(Localization{TKey, TLocale, TValue}?)"/>
        /// <seealso cref="Equals(object?)"/>
        /// <seealso cref="operator!=(Localization{TKey, TLocale, TValue}, Localization{TKey, TLocale, TValue})"/>
        public static bool operator==(Localization<TKey, TLocale, TValue>? lhs, Localization<TKey, TLocale, TValue>? rhs) {
            if (lhs is null) {
                return rhs is null;
            }
            return lhs.Equals(rhs);
        }

        /// <summary>
        ///   Non-Equality Operator.
        /// </summary>
        /// <param name="lhs">
        ///   The left-hand operand.
        /// </param>
        /// <param name="rhs">
        ///   The right-hand operand.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if <paramref name="lhs"/> is not equal to <paramref name="rhs"/>; otherwise,
        ///   <see langword="false"/>.
        /// </returns>
        /// <seealso cref="Equals(Localization{TKey, TLocale, TValue}?)"/>
        /// <seealso cref="Equals(object?)"/>
        /// <seealso cref="operator==(Localization{TKey, TLocale, TValue}, Localization{TKey, TLocale, TValue})"/>
        public static bool operator!=(Localization<TKey, TLocale, TValue>? lhs, Localization<TKey, TLocale, TValue>? rhs) {
            return !(lhs == rhs);
        }
    }
}
