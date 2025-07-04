using Ardalis.GuardClauses;
using Kvasir.Relations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;

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
    ///     or dates according to different calendars. These modes are called "locales" and the value of a semantic in
    ///     a particular locale the "localized value" or "localization." The semantic being localized is known as the
    ///     "localization key" and is simply an identifier by which the localizations are grouped.
    ///   </para>
    ///   <para>
    ///     A <see cref="Localization{TKey, TLocale, TValue}"/> implements the same interfaces as and behaves
    ///     identically to a standard <see cref="Dictionary{TKey, TValue}"/> collection. Operations that mutate the
    ///     collection are used to track changes that can then be reflected in a back-end database, while view- or
    ///     read-only operations have no additional side effects. Converting a
    ///     <see cref="Localization{TKey, TLocale, TValue}"/> into another collection, such as through a member API or
    ///     LINQ, drops the change tracking capabilities.
    ///   </para>
    ///   <para>
    ///     Every locale-value pair in a <see cref="Localization{TKey, TLocale, TValue}"/> is in one of three states:
    ///     <c>NEW</c>, <c>SAVED</c>, or <c>DELETED</c>. Each state corresponds to the action or actions that should be
    ///     taken with respect to that item to synchronize the back-end database table corresponding to the relation. An
    ///     item enters the <c>NEW</c> state when it is first added; when the collection is canonicalized, each
    ///     <c>NEW</c> item transitions to the <c>SAVED</c> state, indicating that it does not need to be written to the
    ///     database on the next write. When a <c>SAVED</c> item is removed from the collection, it transitions to the
    ///     <c>DELETED</c> state; <c>NEW</c> items do not transition to <c>DELETED</c>. Note that if a <c>SAVED</c>
    ///     item is deleted and then re-added, it will be re-added in the <c>SAVED</c> state.
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
    public sealed class Localization<TKey, TLocale, TValue> : IDictionary, IDictionary<TLocale, TValue>,
        ILocalization, IReadOnlyDictionary<TLocale, TValue>, IReadOnlyLocalization<TKey, TLocale, TValue>,
        IRelation where TKey : notnull where TLocale : notnull {

        // *************************************** PROPERTIES ***************************************

        /// <inheritdoc/>
        public int Count => impl_.Count;

        /// <inheritdoc/>
        public TValue this[TLocale locale] {
            get {
                return impl_[locale];
            }
            set {
                impl_[locale] = value;
            }
        }

        // ************************************** CONSTRUCTORS **************************************

        /// <summary>
        ///   Initializes a new instance of the <see cref="Localization{TKey, TLocale, TValue}"/> class that is empty,
        ///   has the default initial capacity, and uses the default equality comparer for the locale type.
        /// </summary>
        /// <param name="key">
        ///   The "localization key" for the new instance.
        /// </param>
        public Localization(TKey key) {
            Guard.Against.Null(key);

            impl_ = new RelationMap<TLocale, TValue>();
            key_ = key;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="Localization{TKey, TLocale, TValue}"/> class that is empty,
        ///   has the default initial capacity, and uses the specified <see cref="IEqualityComparer{TLocale}"/>.
        /// </summary>
        /// <param name="key">
        ///   The "localization key" for the new instance.
        /// </param>
        /// <param name="comparer">
        ///   The <see cref="IEqualityComparer{TLocale}"/> implementation to use when comparing locale, or
        ///   <see langword="null"/> to use the default <see cref="EqualityComparer{TLocale}"/> for the type of the
        ///   locale.
        /// </param>
        public Localization(TKey key, IEqualityComparer<TLocale>? comparer) {
            Guard.Against.Null(key);

            impl_ = new RelationMap<TLocale, TValue>(comparer);
            key_ = key;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="Localization{TKey, TLocale, TValue}"/> class that contains
        ///   elements copied from the specified <see cref="IDictionary{TLocale, TValue}"/> and use the default
        ///   equality comparer for the locale type.
        /// </summary>
        /// <param name="key">
        ///   The "localization key" for the new instance.
        /// </param>
        /// <param name="dictionary">
        ///   The <see cref="IDictionary{TLocale, TValue}"/> whose elements are copied to the new
        ///   <see cref="Localization{TKey, TLocale, TValue}"/>.
        /// </param>
        /// <exception cref="ArgumentException">
        ///   if <paramref name="dictionary"/> contains one or more duplicate keys.
        /// </exception>
        public Localization(TKey key, IDictionary<TLocale, TValue> dictionary)
            : this(key, dictionary, null) {}

        /// <summary>
        ///   Initializes a new instance of the <see cref="Localization{TKey, TLocale, TValue}"/> class that contains
        ///   elements copied from the specified <see cref="IDictionary{TLocale, TValue}"/> and uses the specified
        ///   <see cref="IEqualityComparer{TLocale}"/>.
        /// </summary>
        /// <param name="key">
        ///   The "localization key" for the new instance.
        /// </param>
        /// <param name="dictionary">
        ///   The <see cref="IDictionary{TLocale, TValue}"/> whose elements are copied to the new
        ///   <see cref="Dictionary{TLocale, TValue}"/>.
        /// </param>
        /// <param name="comparer">
        ///   The <see cref="IEqualityComparer{TLocale}"/> implementation to use when comparing locales, or
        ///   <see langword="null"/> to use the default <see cref="EqualityComparer{TLocale}"/> for the type of the
        ///   locale.
        /// </param>
        /// <exception cref="ArgumentException">
        ///   if <paramref name="dictionary"/> contains one or more duplicate keys.
        /// </exception>
        public Localization(TKey key, IDictionary<TLocale, TValue> dictionary, IEqualityComparer<TLocale>? comparer) {
            Guard.Against.Null(key);

            impl_ = new RelationMap<TLocale, TValue>(dictionary, comparer);
            key_ = key;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="Localization{TKey, TLocale, TValue}"/> class that contains
        ///   elements copied from the specified <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <param name="key">
        ///   The "localization key" for the new instance.
        /// </param>
        /// <param name="collection">
        ///   The <see cref="IEnumerable{T}"/> whose elements are copied to the new
        ///   <see cref="Localization{TKey, TLocale, TValue}"/>.
        /// </param>
        /// <exception cref="ArgumentException">
        ///   if <paramref name="collection"/> contains one or more duplicate keys.
        /// </exception>
        public Localization(TKey key, IEnumerable<KeyValuePair<TLocale, TValue>> collection)
            : this(key, collection, null) {}

        /// <summary>
        ///   Initializes a new instance of the <see cref="Localization{TKey, TLocale, TValue}"/> class that contains
        ///   elements copied from the specified <see cref="IEnumerable{T}"/> and uses the specified
        ///   <see cref="IEqualityComparer{TLocale}"/>.
        /// </summary>
        /// <param name="key">
        ///   The "localization key" for the new instance.
        /// </param>
        /// <param name="collection">
        ///   The <see cref="IEnumerable{T}"/> whose elements are copied to the new
        ///   <see cref="Localization{TKey, TLocale, TValue}"/>.
        /// </param>
        /// <param name="comparer">
        ///   The <see cref="IEqualityComparer{TLocale}"/> implementation to use when comparing locales, or
        ///   <see langword="null"/> to use the default <see cref="EqualityComparer{TLocale}"/> for the type of the
        ///   locale.
        /// </param>
        /// <exception cref="ArgumentException">
        ///   if <paramref name="collection"/> contains one or more duplicate keys.
        /// </exception>
        public Localization(TKey key, IEnumerable<KeyValuePair<TLocale, TValue>> collection, IEqualityComparer<TLocale>? comparer) {
            Guard.Against.Null(key);

            impl_ = new RelationMap<TLocale, TValue>(collection, comparer);
            key_ = key;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="Localization{TKey, TLocale, TValue}"/> class that is empty,
        ///   has the specified initial capacity, and uses the default equality comparer for the locale type.
        /// </summary>
        /// <param name="localizationKey">
        ///   The "localization key" for the new instance.
        /// </param>
        /// <param name="capacity">
        ///   The initial number of elements that the <see cref="Localization{TKey, TLocale, TValue}"/> can contain.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   if <paramref name="capacity"/> is less than <c>0</c>.
        /// </exception>
        public Localization(TKey localizationKey, int capacity) {
            Guard.Against.Null(localizationKey);

            impl_ = new RelationMap<TLocale, TValue>(capacity);
            key_ = localizationKey;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="Localization{TKey, TLocale, TValue}"/> class that is empty,
        ///   has the specified initial capacity, and uses the specified <see cref="IEqualityComparer{TLocale}"/>.
        /// </summary>
        /// <param name="key">
        ///   The "localization key" for the new instance.
        /// </param>
        /// <param name="capacity">
        ///   The initial number of elements that the <see cref="Localization{TKey, TLocale, TValue}"/> can contain.
        /// </param>
        /// <param name="comparer">
        ///   The <see cref="IEqualityComparer{TLocale}"/> implementation to use when comparing locales, or
        ///   <see langword="null"/> to use the default <see cref="EqualityComparer{TLocale}"/> for the type of the
        ///   locale.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   if <paramref name="capacity"/> is less than <c>0</c>.
        /// </exception>
        public Localization(TKey key, int capacity, IEqualityComparer<TLocale>? comparer) {
            Guard.Against.Null(key);

            impl_ = new RelationMap<TLocale, TValue>(capacity, comparer);
            key_ = key;
        }

        // **************************************** METHODS *****************************************

        /// <inheritdoc/>
        public void Add(TLocale locale, TValue value) {
            impl_.Add(locale, value);
        }

        /// <inheritdoc/>
        public void Clear() {
            impl_.Clear();
        }

        /// <summary>
        ///   Determines whether the <see cref="Localization{TKey, TLocale, TValue}"/> contains a localization for a
        ///   specified locale.
        /// </summary>
        /// <param name="locale">
        ///   The locale locate in the <see cref="Localization{TKey, TLocale, TValue}"/>.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if the <see cref="Localization{TKey, TLocale, TValue}"/> contains a localized
        ///   value for the specified locale; otherwise, <see langword="false"/>.
        /// </returns>
        [ExcludeFromCodeCoverage]
        public bool ContainsLocalizationFor(TLocale locale) {
            return impl_.ContainsKey(locale);
        }

        /// <summary>
        ///   Determines whether the <see cref="Localization{TKey, TLocale, TValue}"/> contains a specific localized
        ///   value.
        /// </summary>
        /// <param name="value">
        ///   The localized value to locate in the <see cref="Localization{TKey, TLocale, TValue}"/>. The value can be
        ///   <see langword="null"/> for nullable reference types.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if the <see cref="Localization{TKey, TLocale, TValue}"/> contains a localization
        ///   with the specified value; otherwise, <see langword="false"/>.
        /// </returns>
        [ExcludeFromCodeCoverage]
        public bool ContainsValue(TValue value) {
            return impl_.ContainsValue(value);
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public sealed override bool Equals(object? obj) {
            return (obj is Localization<TKey, TLocale, TValue> local) &&
                   impl_.Equals(local.impl_);
        }

        /// <summary>
        ///   Returns an enumerator that iterates through a <see cref="Localization{TKey, TLocale, TValue}"/>.
        /// </summary>
        /// <returns>
        ///   A <see cref="Dictionary{TLocale, TValue}.Enumerator"/> object for the
        ///   <see cref="Localization{TKey, TLocale, TValue}"/>.
        /// </returns>
        [ExcludeFromCodeCoverage]
        public Dictionary<TLocale, TValue>.Enumerator GetEnumerator() {
            return impl_.GetEnumerator();
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public sealed override int GetHashCode() {
            return impl_.GetHashCode();
        }

        /// <inheritdoc/>
        public bool Remove(TLocale locale) {
            return impl_.Remove(locale);
        }

        /// <summary>
        ///   Removes the localized value for the specified locale from the
        ///   <see cref="Localization{TKey, TLocale, TValue}"/>, and copies the element to the <paramref name="value"/>
        ///   parameter.
        /// </summary>
        /// <param name="locale">
        ///   The locale of the localized value to remove.
        /// </param>
        /// <param name="value">
        ///   The removed element.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if the element is successfully found and removed; otherwise,
        ///   <see langword="false"/>.
        /// </returns>
        public bool Remove(TLocale locale, out TValue value) {
            return impl_.Remove(locale, out value);
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public sealed override string ToString() {
            var builder = new StringBuilder();
            builder.AppendLine($"{{Key = {key_}}}");
            builder.Append(impl_.ToString());
            return builder.ToString();
        }

        /// <summary>
        ///   Sets the capacity of this <see cref="Localization{TKey, TLocale, TValue}"/> to what it would be if it had
        ///   originally been initialized with all its entries.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public void TrimExcess() {
            impl_.TrimExcess();
        }

        /// <summary>
        ///   Sets the capacity of this <see cref="Localization{TKey, TLocale, TValue}"/> to hold up to a specified
        ///   number of entries without further expansion of its backing storage.
        /// </summary>
        /// <param name="capacity">
        ///   The new capacity.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="capacity"/> is less than the size of the
        ///   <see cref="Localization{TKey, TLocale, TValue}"/>.
        /// </exception>
        [ExcludeFromCodeCoverage]
        public void TrimExcess(int capacity) {
            impl_.TrimExcess(capacity);
        }

        /// <summary>
        ///   Attempts to add the specific locale and localized value to the
        ///   <see cref="Localization{TKey, TLocale, TValue}"/>.
        /// </summary>
        /// <param name="locale">
        ///   The locale of the localized value to add.
        /// </param>
        /// <param name="value">
        ///   The localized value to add. It can be <see langword="null"/> for nullable reference types.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if the localization was added to the
        ///   <see cref="Localization{TKey, TLocale, TValue}"/>; otherwise, <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   if <paramref name="locale"/> is <see langword="null"/>.
        /// </exception>
        public bool TryAdd(TLocale locale, TValue value) {
            return impl_.TryAdd(locale, value);
        }

        /// <summary>
        ///   Gets the localized value for the specified locale.
        /// </summary>
        /// <param name="locale">
        ///   The locale of the localized value to get.
        /// </param>
        /// <param name="value">
        ///   When this method returns, contains the localized value for the specified locale, if the locale is found;
        ///   otherwise, the default value for the type of the <paramref name="value"/> parameter. This parameter is
        ///   passed uninitialized.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if the <see cref="Localization{TKey, TLocale, TValue}"/> contains a localized
        ///   value with for specified locale; otherwise, <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   if <paramref name="locale"/> is <see langword="null"/>.
        /// </exception>
        [ExcludeFromCodeCoverage]
        public bool TryGetValue(TLocale locale, out TValue value) {
            return impl_.TryGetValue(locale, out value!);
        }

        // ******************************** EXPLICIT INTERFACE IMPLS ********************************

        /// <inheritdoc/>
        static Type IRelation.ConnectionType => typeof(Tuple<TKey, TLocale, TValue>);

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        bool ICollection.IsSynchronized => (impl_ as ICollection).IsSynchronized;

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        object ICollection.SyncRoot => (impl_ as ICollection).SyncRoot;

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        bool ICollection<KeyValuePair<TLocale, TValue>>.IsReadOnly =>
            (impl_ as ICollection<KeyValuePair<TLocale, TValue>>).IsReadOnly;

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        bool IDictionary.IsFixedSize => (impl_ as IDictionary).IsFixedSize;

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        bool IDictionary.IsReadOnly => (impl_ as IDictionary).IsReadOnly;

        /// <inheritdoc/>
        object? IDictionary.this[object key] {
            get {
                return (impl_ as IDictionary)[key];
            }
            set {
                (impl_ as IDictionary)[key] = value;
            }
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        ICollection IDictionary.Keys => impl_.Keys;

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        ICollection IDictionary.Values => impl_.Values;

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        IEnumerable<TLocale> IReadOnlyDictionary<TLocale, TValue>.Keys => impl_.Keys;

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        IEnumerable<TValue> IReadOnlyDictionary<TLocale, TValue>.Values => impl_.Values;

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        ICollection<TLocale> IDictionary<TLocale, TValue>.Keys => impl_.Keys;

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        ICollection<TValue> IDictionary<TLocale, TValue>.Values => impl_.Values;

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        void ICollection.CopyTo(Array array, int index) {
            (impl_ as ICollection).CopyTo(array, index);
        }

        /// <inheritdoc/>
        void ICollection<KeyValuePair<TLocale, TValue>>.Add(KeyValuePair<TLocale, TValue> item) {
            (impl_ as ICollection<KeyValuePair<TLocale, TValue>>).Add(item);
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        bool ICollection<KeyValuePair<TLocale, TValue>>.Contains(KeyValuePair<TLocale, TValue> item) {
            return (impl_ as ICollection<KeyValuePair<TLocale, TValue>>).Contains(item);
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        void ICollection<KeyValuePair<TLocale, TValue>>.CopyTo(KeyValuePair<TLocale, TValue>[] array, int arrayIndex) {
            (impl_ as ICollection<KeyValuePair<TLocale, TValue>>).CopyTo(array, arrayIndex);
        }

        /// <inheritdoc/>
        bool ICollection<KeyValuePair<TLocale, TValue>>.Remove(KeyValuePair<TLocale, TValue> item) {
            return (impl_ as ICollection<KeyValuePair<TLocale, TValue>>).Remove(item);
        }

        /// <inheritdoc/>
        void IDictionary.Add(object key, object? value) {
            (impl_ as IDictionary).Add(key, value);
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        bool IDictionary.Contains(object key) {
            return (impl_ as IDictionary).Contains(key);
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        IDictionaryEnumerator IDictionary.GetEnumerator() {
            return GetEnumerator();
        }

        /// <inheritdoc/>
        void IDictionary.Remove(object key) {
            (impl_ as IDictionary).Remove(key);
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        bool IDictionary<TLocale, TValue>.ContainsKey(TLocale locale) {
            return impl_.ContainsKey(locale);
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        IEnumerator<KeyValuePair<TLocale, TValue>> IEnumerable<KeyValuePair<TLocale, TValue>>.GetEnumerator() {
            return (impl_ as IEnumerable<KeyValuePair<TLocale, TValue>>).GetEnumerator();
        }

        /// <inheritdoc/>
        object ILocalization.LocalizationKey => key_;

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        bool IReadOnlyDictionary<TLocale, TValue>.ContainsKey(TLocale locale) {
            return impl_.ContainsKey(locale);
        }

        /// <inheritdoc/>
        void IRelation.Canonicalize() {
            (impl_ as IRelation).Canonicalize();
        }

        /// <inheritdoc/>
        IEnumerator<(object Item, Status Status)> IRelation.GetEnumerator() {
            // It's annoying that we can just use a standard foreach loop, but that is inhibited by the fact that the
            // GetEnumerator() function on the IRelation interface is internal; even though we're in the same assembly,
            // the language doesn't recognize it properly. We don't want to change the access modifier, so we have to do
            // a bit of manual iteration.
            var iter = (impl_ as IRelation).GetEnumerator();
            while (iter.MoveNext()) {
                var castedItem = (KeyValuePair<TLocale, TValue>)iter.Current.Item;
                var element = new Tuple<TKey, TLocale, TValue>(key_, castedItem.Key, castedItem.Value);
                yield return (element, iter.Current.Status);
            }
        }

        /// <inheritdoc/>
        void IRelation.Repopulate(object item) {
            Debug.Assert(item.GetType() == typeof(Tuple<TKey, TLocale, TValue>));

            var castedItem = (Tuple<TKey, TLocale, TValue>)item;
            Debug.Assert(key_.Equals(castedItem.Item1));

            (impl_ as IRelation).Repopulate(new KeyValuePair<TLocale, TValue>(castedItem.Item2, castedItem.Item3));
        }

        // ************************************ MEMBER VARIABLES ************************************

        private readonly TKey key_;
        private readonly RelationMap<TLocale, TValue> impl_;
    }

    /// <summary>
    ///   The internal framework interface denoting a collection of localized values.
    /// </summary>
    public interface ILocalization {
        /// <summary>
        ///   Gets the "localization key" that identifies the Localization.
        /// </summary>
        /// <value>
        ///   The "localization key" for the localization.
        /// </value>
        internal object LocalizationKey { get; }
    }

    /// <summary>
    ///   An interface denoting a read-only view over a <see cref="Localization{TKey, TLocale, TValue}"/>.
    /// </summary>
    /// <remarks>
    ///   This interface is intended to allow class authors to expose a Localization through a read-only property while
    ///   controlling mutating operations on the underlying collection via member functions. Users would call, e.g., an
    ///   insertion function that essentially "intercepts" the call to the underlying collection's mutator, permitting
    ///   validation or ordering or logging or any other custom behavior.
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
    public interface IReadOnlyLocalization<TKey, TLocale, TValue> : ILocalization,
        IReadOnlyDictionary<TLocale, TValue>, IRelation where TKey : notnull where TLocale : notnull {

        /// <inheritdoc/>
        static Type IRelation.ConnectionType => typeof(Tuple<TKey, TLocale, TValue>);
    }
}
