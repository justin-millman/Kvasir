using Ardalis.GuardClauses;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Kvasir.Relations {
    /// <summary>
    ///   A collection that trakcs the state of its key-value mappings for interaction with a back-end database.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     A <see cref="RelationMap{TKey, TValue}"/> implements the same interfaces as and behaves identically to a
    ///     standard <see cref="Dictionary{TKey, TValue}"/> collection. Operations that mutate the collection are used
    ///     to track changes that can then be reflected in a back-end database, while view- or read-only operationsh
    ///     have no additional side effects. Converting a <see cref="RelationMap{TKey, TValue}"/> into another
    ///     collection, such as through a member API or LINQ, drops the change tracking capabilities.
    ///   </para>
    ///   <para>
    ///     Every key-value pair in a <see cref="RelationMap{TKey, TValue}"/> is in one of three states: <c>NEW</c>,
    ///     <c>SAVED</c>, or <c>DELETED</c>. Each state corresponds to the action or actions that should be taken with
    ///     respect to that item to synchronize the back-end database table corresponding to the relation. An item
    ///     enters the <c>NEW</c> state when it is first added; when the collection is canonicalized, each <c>NEW</c>
    ///     item transitions to the <c>SAVED</c> state, indicating that it does not need to be written to the database
    ///     on the next write. When a <c>SAVED</c> item is removed from the collection, it transitions to the
    ///     <c>DELETED</c> state; <c>NEW</c> items do not transition to <c>DELETED</c>. Note that if a <c>SAVED</c>
    ///     item is deleted and then re-added, it will be re-added in the <c>SAVED</c> state.
    ///   </para>
    ///   <para>
    ///     The <see cref="RelationMap{TKey, TValue}"/> does not support the <c>MODIFIED</c> state; instead, each
    ///     key-value pair is treated as its own unit. This means that changing the value mapped to by an existing key
    ///     results in two separate operations in the back-end database: first a removal of the old key-value pair,
    ///     then an insertion of the new one. This behavior <i>may</i> change in the future, so clients should not rely
    ///     on it. The guarantee, however, is that Kvasir will properly respond to an overwrite by ensuring that the
    ///     resulting back-end database contains only the new key-value pair.
    ///   </para>
    ///   <para>
    ///     Items used for the value in a <see cref="RelationMap{TKey, TValue}"/> should be immutable: structs,
    ///     <see cref="string"/>, etc. This is because read access is <i>not</i> tracked: when usiing mutable values,
    ///     it is possible for the user to access an item (e.g. through <c>operator[]</c>) and mutate that value
    ///     without the collection knowing, preventing the change from being reflected in the back-end database. This
    ///     also means that actions that convert the collection into another form will <i>copy</i> the elements,
    ///     ensuring that the tracing data remains up-to-date.
    ///   </para>
    ///   <para>
    ///     A <see cref="RelationMap{TKey, TValue}"/> does not permit duplicate keys, but it does support duplicate
    ///     values.
    ///   </para>
    /// </remarks>
    /// <typeparam name="TKey">
    ///   The type of the key of the collection's key-value pairs.
    /// </typeparam>
    /// <typeparam name="TValue">
    ///   The type of the value of the collection's key-value pairs.
    /// </typeparam>
    public sealed class RelationMap<TKey, TValue> : ICollection<KeyValuePair<TKey, TValue>>, IDictionary,
        IDictionary<TKey, TValue>, IEnumerable<KeyValuePair<TKey, TValue>>,
        IReadOnlyCollection<KeyValuePair<TKey, TValue>>, IReadOnlyDictionary<TKey, TValue>,
        IReadOnlyRelationMap<TKey, TValue>, IRelation where TKey : notnull where TValue : notnull {

        // *************************************** PROPERTIES ***************************************

        /// <summary>
        ///   Gets the <see cref="IEqualityComparer{T}"/> object that is used to determine equality of keys for the
        ///   <see cref="RelationMap{TKey, TValue}"/>.
        /// </summary>
        /// <value>
        ///   The <see cref="IEqualityComparer{T}"/> object that is used to determine equality of keys for the
        ///   <see cref="RelationMap{TKey, TValue}"/>.
        /// </value>
        public IEqualityComparer<TKey> Comparer => impl_.Comparer;

        /// <inheritdoc/>
        public int Count => impl_.Count;

        /// <inheritdoc/>
        public TValue this[TKey key] {
            get {
                return impl_[key];
            }
            set {
                Remove(key);
                Add(key, value);
            }
        }

        /// <summary>
        ///   Gets a collection containing the keys in the <see cref="RelationMap{TKey, TValue}"/>.
        /// </summary>
        /// <value>
        ///   A <see cref="Dictionary{TKey, TValue}.KeyCollection"/> containing the keys in the
        ///   <see cref="RelationMap{TKey, TValue}"/>.
        /// </value>
        public Dictionary<TKey, TValue>.KeyCollection Keys => impl_.Keys;

        /// <summary>
        ///   Gets a collection containing the keys in the <see cref="RelationMap{TKey, TValue}"/>.
        /// </summary>
        /// <value>
        ///   A <see cref="Dictionary{TKey, TValue}.ValueCollection"/> containing the values in the
        ///   <see cref="RelationMap{TKey, TValue}"/>.
        /// </value>
        public Dictionary<TKey, TValue>.ValueCollection Values => impl_.Values;

        // ************************************** CONSTRUCTORS **************************************

        /// <summary>
        ///   Initializes a new instance of the <see cref="RelationMap{TKey, TValue}"/> class that is empty, has the
        ///   default initial capacity, and uses the default equality comparer for the key type.
        /// </summary>
        public RelationMap() {
            impl_ = new Dictionary<TKey, TValue>();
            statuses_ = new Dictionary<TKey, Status>();
            deletions_ = new Dictionary<TKey, TValue>();
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="RelationMap{TKey, TValue}"/> class that is empty, has the
        ///   default initial capacity, and uses the specified <see cref="IEqualityComparer{T}"/>.
        /// </summary>
        /// <param name="comparer">
        ///   The <see cref="IEqualityComparer{T}"/> implementation to use when comparing keys, or
        ///   <see langword="null"/> to use the default <see cref="EqualityComparer{T}"/> for the type of the key.
        /// </param>
        public RelationMap(IEqualityComparer<TKey>? comparer) {
            impl_ = new Dictionary<TKey, TValue>(comparer);
            statuses_ = new Dictionary<TKey, Status>(comparer);
            deletions_ = new Dictionary<TKey, TValue>(comparer);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="RelationMap{TKey, TValue}"/> class that contains elements
        ///   copied from the specified <see cref="IDictionary{TKey, TValue}"/> and use the default equality comparer
        ///   for the key type.
        /// </summary>
        /// <param name="dictionary">
        ///   The <see cref="IDictionary{TKey, TValue}"/> whose elements are copied to the new
        ///   <see cref="RelationMap{TKey, TValue}"/>.
        /// </param>
        /// <exception cref="ArgumentException">
        ///   if <paramref name="dictionary"/> contains one or more duplicate keys.
        /// </exception>
        public RelationMap(IDictionary<TKey, TValue> dictionary)
            : this(dictionary, null) {}

        /// <summary>
        ///   Initializes a new instance of the <see cref="RelationMap{TKey, TValue}"/> class that contains elements
        ///   copied from the specified <see cref="IDictionary{TKey, TValue}"/> and uses the specified
        ///   <see cref="IEqualityComparer{T}"/>.
        /// </summary>
        /// <param name="dictionary">
        ///   The <see cref="IDictionary{TKey, TValue}"/> whose elements are copied to the new
        ///   <see cref="Dictionary{TKey, TValue}"/>.
        /// </param>
        /// <param name="comparer">
        ///   The <see cref="IEqualityComparer{T}"/> implementation to use when comparing keys, or
        ///   <see langword="null"/> to use the default <see cref="EqualityComparer{T}"/> for the type of the key.
        /// </param>
        /// <exception cref="ArgumentException">
        ///   if <paramref name="dictionary"/> contains one or more duplicate keys.
        /// </exception>
        public RelationMap(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey>? comparer) {
            impl_ = new Dictionary<TKey, TValue>(dictionary, comparer);
            statuses_ = new Dictionary<TKey, Status>(
                dictionary.Keys.Select(k => new KeyValuePair<TKey, Status>(k, Status.New)), comparer);
            deletions_ = new Dictionary<TKey, TValue>(comparer);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="RelationMap{TKey, TValue}"/> class that contains elements
        ///   copied from the specified <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <param name="collection">
        ///   The <see cref="IEnumerable{T}"/> whose elements are copied to the new
        ///   <see cref="RelationMap{TKey, TValue}"/>.
        /// </param>
        /// <exception cref="ArgumentException">
        ///   if <paramref name="collection"/> contains one or more duplicate keys.
        /// </exception>
        public RelationMap(IEnumerable<KeyValuePair<TKey, TValue>> collection)
            : this(collection, null) {}

        /// <summary>
        ///   Initializes a new instance of the <see cref="RelationMap{TKey, TValue}"/> class that contains elements
        ///   copied from the specified <see cref="IEnumerable{T}"/> and uses the specified
        ///   <see cref="IEqualityComparer{T}"/>.
        /// </summary>
        /// <param name="collection">
        ///   The <see cref="IEnumerable{T}"/> whose elements are copied to the new
        ///   <see cref="RelationMap{TKey, TValue}"/>.
        /// </param>
        /// <param name="comparer">
        ///   The <see cref="IEqualityComparer{T}"/> implementation to ue when comparing keys, or
        ///   <see langword="null"/> to use the default <see cref="EqualityComparer{T}"/> for the type of the key.
        /// </param>
        /// <exception cref="ArgumentException">
        ///   if <paramref name="collection"/> contains one or more duplicate keys.
        /// </exception>
        public RelationMap(IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey>? comparer) {
            impl_ = new Dictionary<TKey, TValue>(collection, comparer);
            statuses_ = new Dictionary<TKey, Status>(
                collection.Select(kvp => new KeyValuePair<TKey, Status>(kvp.Key, Status.New)), comparer);
            deletions_ = new Dictionary<TKey, TValue>(comparer);
        }
         
        /// <summary>
        ///   Initializes a new instance of the <see cref="RelationMap{TKey, TValue}"/> class that is empty, has the
        ///   specified initial capacity, and uses the default equality comparer for the key type.
        /// </summary>
        /// <param name="capacity">
        ///   The initial number of elements that the <see cref="RelationMap{TKey, TValue}"/> can contain.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   if <paramref name="capacity"/> is less than <c>0</c>.
        /// </exception>
        public RelationMap(int capacity)
            : this(capacity, null) {}

        /// <summary>
        ///   Initializes a new instance of the <see cref="RelationMap{TKey, TValue}"/> class that is empty, has the
        ///   specified initial capacity, and uses the specified <see cref="IEqualityComparer{T}"/>.
        /// </summary>
        /// <param name="capacity">
        ///   The initial number of elements that the <see cref="RelationMap{TKey, TValue}"/> can contain.
        /// </param>
        /// <param name="comparer">
        ///   The <see cref="IEqualityComparer{T}"/> implementation to use when comparing keys, or
        ///   <see langword="null"/> to use the default <see cref="EqualityComparer{T}"/> for the type of the key.
        /// </param>
        public RelationMap(int capacity, IEqualityComparer<TKey>? comparer) {
            impl_ = new Dictionary<TKey, TValue>(capacity, comparer);
            statuses_ = new Dictionary<TKey, Status>(capacity, comparer);
            deletions_ = new Dictionary<TKey, TValue>(comparer);
        }

        // **************************************** METHODS *****************************************

        /// <inheritdoc/>
        public void Add(TKey key, TValue value) {
            impl_.Add(key, value);
            statuses_[key] = BookkeepAddition(key, value);
        }

        /// <inheritdoc/>
        public void Clear() {
            foreach (var key in impl_.Keys) {
                BookkeepRemoval(key);
            }
            impl_.Clear();
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public bool ContainsKey(TKey key) {
            return impl_.ContainsKey(key);
        }

        /// <summary>
        ///   Determines whether the <see cref="RelationMap{TKey, TValue}"/> contains a specific value.
        /// </summary>
        /// <param name="value">
        ///   The value to locate in the <see cref="RelationMap{TKey, TValue}"/>. The value can be
        ///   <see langword="null"/> for nullable reference types.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if the <see cref="RelationMap{TKey, TValue}"/> contains an element with the
        ///   specified value; otherwise, <see langword="false"/>.
        /// </returns>
        [ExcludeFromCodeCoverage]
        public bool ContainsValue(TValue value) {
            return impl_.ContainsValue(value);
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public sealed override bool Equals(object? obj) {
            return (obj is RelationMap<TKey, TValue> map) &&
                    impl_.Equals(map.impl_) &&
                    statuses_.Equals(map.statuses_) &&
                    deletions_.Equals(map.deletions_);
        }

        /// <summary>
        ///   Returns an enumerator that iterates through a <see cref="RelationMap{TKey, TValue}"/>.
        /// </summary>
        /// <returns>
        ///   A <see cref="Dictionary{TKey, TValue}.Enumerator"/> object for the <see cref="RelationMap{TKey, TValue}"/>.
        /// </returns>
        [ExcludeFromCodeCoverage]
        public Dictionary<TKey, TValue>.Enumerator GetEnumerator() {
            return impl_.GetEnumerator();
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public sealed override int GetHashCode() {
            return HashCode.Combine(impl_.GetHashCode(), statuses_.GetHashCode(), deletions_.GetHashCode());
        }

        /// <inheritdoc/>
        public bool Remove(TKey key) {
            return Remove(key, out TValue _);
        }

        /// <summary>
        ///   Removes the value with the specified key from the <see cref="RelationMap{TKey, TValue}"/>, and copies
        ///   the element to the <paramref name="value"/> parameter.
        /// </summary>
        /// <param name="key">
        ///   The key of the element to remove.
        /// </param>
        /// <param name="value">
        ///   The removed element.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if the element is successfully found and removed; otherwise,
        ///   <see langword="false"/>.
        /// </returns>
        public bool Remove(TKey key, out TValue value) {
            if (impl_.ContainsKey(key)) {
                BookkeepRemoval(key);
            }
            return impl_.Remove(key, out value!);
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public sealed override string ToString() {
            var builder = new StringBuilder();
            foreach (var kvp in impl_) {
                builder.AppendLine($"{kvp.Key} => {kvp.Value} [{statuses_[kvp.Key]}]");
            }

            return builder.ToString();
        }

        /// <summary>
        ///   Sets the capacity of this <see cref="RelationMap{TKey, TValue}"/> to what it would be if it had
        ///   originally been initialized with all its entries.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public void TrimExcess() {
            impl_.TrimExcess();
            statuses_.TrimExcess();
            deletions_.TrimExcess();
        }

        /// <summary>
        ///   Sets the capacity of this <see cref="RelationMap{TKey, TValue}"/> to hold up to a specified number of
        ///   entries without further expansion of its backing storage.
        /// </summary>
        /// <param name="capacity">
        ///   The new capacity.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="capacity"/> is less than the size of the <see cref="RelationMap{TKey, TValue}"/>.
        /// </exception>
        [ExcludeFromCodeCoverage]
        public void TrimExcess(int capacity) {
            impl_.TrimExcess(capacity);
            statuses_.TrimExcess(capacity);
            deletions_.TrimExcess();
        }

        /// <summary>
        ///   Attempts to add the specific key and value to the <see cref="RelationMap{TKey, TValue}"/>.
        /// </summary>
        /// <param name="key">
        ///   The key of the element to add.
        /// </param>
        /// <param name="value">
        ///   The value of the element to add. It can be <see langword="null"/> for nullable reference types.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if the key/value pair was added to the <see cref="RelationMap{TKey, TValue}"/>;
        ///   otherwise, <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   if <paramref name="key"/> is <see langword="null"/>.
        /// </exception>
        public bool TryAdd(TKey key, TValue value) {
            if (ContainsKey(key)) {
                return false;
            }

            Add(key, value);
            return true;
        }

        /// <summary>
        ///   Gets the alue associated with the specified key.
        /// </summary>
        /// <param name="key">
        ///   The key of the value to get.
        /// </param>
        /// <param name="value">
        ///   When this method returns, contains the value associated with the specified key, if the key is found;
        ///   otherwise, the default value for the type of the <paramref name="value"/> parameter. This parameter is
        ///   passed uninitialized.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if the <see cref="RelationMap{TKey, TValue}"/> contains an element with the
        ///   specified key; otherwise, <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   if <paramref name="key"/> is <see langword="null"/>.
        /// </exception>
        [ExcludeFromCodeCoverage]
        public bool TryGetValue(TKey key, out TValue value) {
            return impl_.TryGetValue(key, out value!);
        }

        // ******************************** EXPLICIT INTERFACE IMPLS ********************************

        /// <inheritdoc/>
        Type IRelation.ConnectionType => typeof(KeyValuePair<TKey, TValue>);

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        bool ICollection.IsSynchronized => (impl_ as ICollection).IsSynchronized;

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        object ICollection.SyncRoot => (impl_ as ICollection).SyncRoot;

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly =>
            (impl_ as ICollection<KeyValuePair<TKey, TValue>>).IsReadOnly;

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        bool IDictionary.IsFixedSize => (impl_ as IDictionary).IsFixedSize;

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        bool IDictionary.IsReadOnly => (impl_ as IDictionary).IsReadOnly;

        /// <inheritdoc/>
        object? IDictionary.this[object key] {
            get {
                Guard.Against.NullOrInvalidInput(key, nameof(key), k => k.GetType() == typeof(TKey));
                return this[(TKey)key];
            }
            set {
                Guard.Against.NullOrInvalidInput(key, nameof(key), k => k.GetType() == typeof(TKey));
                Guard.Against.NullOrInvalidInput(value, nameof(value), v => v!.GetType() == typeof(TKey));
                this[(TKey)key] = (TValue)value!;
            }
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        ICollection IDictionary.Keys => this.Keys;

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        ICollection IDictionary.Values => this.Values;

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => this.Keys;

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => this.Values;

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        ICollection<TKey> IDictionary<TKey, TValue>.Keys => this.Keys;

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        ICollection<TValue> IDictionary<TKey, TValue>.Values => this.Values;

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        void ICollection.CopyTo(Array array, int index) {
            (impl_ as ICollection).CopyTo(array, index);
        }

        /// <inheritdoc/>
        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item) {
            Add(item.Key, item.Value);
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item) {
            return (impl_ as ICollection<KeyValuePair<TKey, TValue>>).Contains(item);
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
            (impl_ as ICollection<KeyValuePair<TKey, TValue>>).CopyTo(array, arrayIndex);
        }

        /// <inheritdoc/>
        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item) {
            if (impl_.ContainsKey(item.Key) && Equals(impl_[item.Key], item.Value)) {
                Remove(item.Key);
                return true;
            }
            return false;
        }

        /// <inheritdoc/>
        void IDictionary.Add(object key, object? value) {
            Guard.Against.NullOrInvalidInput(key, nameof(key), k => k.GetType() == typeof(TKey));
            Guard.Against.NullOrInvalidInput(value, nameof(value), v => v!.GetType() == typeof(TKey));
            Add((TKey)key, (TValue)value!);
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        bool IDictionary.Contains(object key) {
            Guard.Against.InvalidInput(key, nameof(key), k => k.GetType() == typeof(TKey));
            return ContainsKey((TKey)key);
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        IDictionaryEnumerator IDictionary.GetEnumerator() {
            return GetEnumerator();
        }

        /// <inheritdoc/>
        void IDictionary.Remove(object key) {
            Guard.Against.NullOrInvalidInput(key, nameof(key), k => k.GetType() == typeof(TKey));
            Remove((TKey)key);
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() {
            return GetEnumerator();
        }

        /// <inheritdoc/>
        void IRelation.Canonicalize() {
            deletions_.Clear();
            foreach (var key in statuses_.Keys) {
                statuses_[key] = Status.Saved;
            }
        }

        /// <inheritdoc/>
        IEnumerator<(object Item, Status Status)> IRelation.GetEnumerator() {
            foreach (var kvp in deletions_) {
                yield return (kvp, Status.Deleted);
            }
            foreach (var kvp in impl_) {
                yield return (kvp, statuses_[kvp.Key]);
            }
        }

        /// <inheritdoc/>
        void IRelation.Repopulate(object item) {
            Debug.Assert(item.GetType() == typeof(KeyValuePair<TKey, TValue>));
            Debug.Assert(deletions_.Count == 0);
            Debug.Assert(statuses_.Values.All(v => v == Status.Saved));

            var castedItem = (KeyValuePair<TKey, TValue>)item;
            Debug.Assert(!impl_.ContainsKey(castedItem.Key));

            impl_[castedItem.Key] = castedItem.Value;
            statuses_[castedItem.Key] = Status.Saved;
        }

        // ************************************ HELPER FUNCTIONS ************************************

        private Status BookkeepAddition(TKey keyAddition, TValue valueAddition) {
            var addedStatus = deletions_.ContainsKey(keyAddition) && deletions_[keyAddition]!.Equals(valueAddition)
                ? Status.Saved : Status.New;

            if (addedStatus == Status.Saved) {
                deletions_.Remove(keyAddition);
            }
            return addedStatus;
        }

        private void BookkeepRemoval(TKey keyRemoval) {
            if (statuses_[keyRemoval] == Status.Saved) {
                deletions_.Add(keyRemoval, impl_[keyRemoval]);
            }
        }

        // ************************************ MEMBER VARIABLES ************************************

        private readonly Dictionary<TKey, TValue> impl_;
        private readonly Dictionary<TKey, Status> statuses_;
        private readonly Dictionary<TKey, TValue> deletions_;
    }

    /// <summary>
    ///   An interface denoting a read-only view over a <see cref="RelationMap{TKey, TValue}"/>.
    /// </summary>
    /// <remarks>
    ///   This interface is intended to allow class authors to expose a relation through a read-only property while
    ///   controlling mutating operations on the underlying relation via member functions. Users would call, e.g., an
    ///   insertion function that essentially "intercepts" the call to the underlying collection's mutator, permitting
    ///   validation or ordering or logging or any other custom behavior.
    /// </remarks>
    /// <typeparam name="TKey">
    ///   The type of the key of the collection's key-value pairs.
    /// </typeparam>
    /// <typeparam name="TValue">
    ///   The type of the value of the collection's key-value pairs.
    /// </typeparam>
    public interface IReadOnlyRelationMap<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>, IRelation
        where TKey : notnull where TValue : notnull {}
}
