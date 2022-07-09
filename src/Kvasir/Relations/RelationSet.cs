using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Kvasir.Relations {
    /// <summary>
    ///   An unordered collection that tracks the state of its unique elements for interaction with a back-end
    ///   database.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     A <see cref="RelationSet{T}"/> implements the same interfaces and behaves identically to a standard
    ///     <see cref="HashSet{T}"/> collection. Operations that mutate the collection are used to track changes that
    ///     can then be reflected in a back-end database, while view- or read-only operations have no additional side
    ///     effect. Converting a <see cref="RelationSet{T}"/> into another collection type, such as through a member
    ///     API (e.g. <see cref="CopyTo(T[])"/> or LINQ, drops the change tracking capabilities.
    ///   </para>
    ///   <para>
    ///     Every item in a <see cref="RelationSet{T}"/> is in one of three states: <c>NEW</c>, <c>SAVED</c>, or
    ///     <c>DELETED</c>. Each state corresponds to the action or actions that should be taken with respect to that
    ///     item to synchronize the back-end database table corresponding to the relation. An item enters the
    ///     <c>NEW</c> state when it is first added; when the collection is canonicalized, each <c>NEW</c> item
    ///     transitions to the <c>SAVED</c> state, indicating that it does not need to be written to the database on
    ///     the next write. When a <c>SAVED</c> item is removed from the collection, it transitions to the
    ///     <c>DELETED</c> state; <c>NEW</c> items do not transition to <c>DELETED</c>. Note that if a <c>SAVED</c>
    ///     item is deleted and then re-added, it will be re-added in the <c>SAVED</c> state.
    ///   </para>
    ///   <para>
    ///     Items used in a <see cref="RelationSet{T}"/> should be immutable: structs, <see cref="string"/>, etc. This
    ///     is because read access is <i>not</i> tracked: when using mutable elements, it is possible for the uer to
    ///     access an item (e.g. through <c>TryGetValue</c>) and mutate that element without the collection knowing,
    ///     preventing that change from being reflected in the back-end database. This also means that actions that
    ///     convert the collection into another form will <i>copy</i> the elements, ensuring that the tracking data
    ///     remains up-to-date.
    ///   </para>
    ///   <para>
    ///     A <see cref="RelationSet{T}"/> does not permit duplicate elements.
    ///   </para>
    /// </remarks>
    /// <typeparam name="T">
    ///   The type of element to be stored in the collection.
    /// </typeparam>
    public sealed class RelationSet<T> : ICollection<T>, IEnumerable<T>, IReadOnlyCollection<T>,
        IReadOnlyRelationSet<T>, IReadOnlySet<T>, IRelation, ISet<T> where T : notnull {

        // *************************************** PROPERTIES ***************************************

        /// <summary>
        ///   Gets the <see cref="IEqualityComparer{T}"/> object that is used to determine equality for the values in
        ///   the set.
        /// </summary>
        /// <value>
        ///   The <see cref="IEqualityComparer{T}"/> object that is used to determine equality for hte values in the
        ///   set.
        /// </value>
        public IEqualityComparer<T> Comparer => impl_.Comparer;

        /// <inheritdoc/>
        public int Count => impl_.Count;

        // ************************************** CONSTRUCTORS **************************************

        /// <summary>
        ///   Initializes a new instance of the <see cref="RelationSet{T}"/> class that is empty and uses the default
        ///   equality comparer for the set type.
        /// </summary>
        public RelationSet() {
            impl_ = new HashSet<T>();
            statuses_ = new Dictionary<T, Status>();
            deletions_ = new List<T>();
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="RelationSet{T}"/> class that is empty, but has reserved
        ///   space for <paramref name="capacity"/> items and uses the default equality comparer for the set type.
        /// </summary>
        /// <param name="capacity">
        ///   The initial size of the <see cref="RelationSet{T}"/>.
        /// </param>
        public RelationSet(int capacity) {
            impl_ = new HashSet<T>(capacity);
            statuses_ = new Dictionary<T, Status>(capacity);
            deletions_ = new List<T>();
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="RelationSet{T}"/> class that uses the default equality
        ///   comparer for the set type, contains elements copied from the specified colletion, and has sufficient
        ///   capacity to accommodate the number of elements copied.
        /// </summary>
        /// <param name="collection">
        ///   The collection whose elements are copied to the new set.
        /// </param>
        public RelationSet(IEnumerable<T> collection) {
            impl_ = new HashSet<T>(collection);
            statuses_ = new Dictionary<T, Status>(impl_.Select(v => new KeyValuePair<T, Status>(v, Status.New)));
            deletions_ = new List<T>();
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="RelationSet{T}"/> class that is empty and uses the specified
        ///   equality comparer for the set <typeparamref name="T"/>.
        /// </summary>
        /// <param name="comparer">
        ///   The <see cref="IEqualityComparer{T}"/> implementation to use when comparing values in the set, or
        ///   <see langword="null"/> to use the default <see cref="EqualityComparer{T}"/> implementation for the set
        ///   type.
        /// </param>
        public RelationSet(IEqualityComparer<T>? comparer) {
            impl_ = new HashSet<T>(comparer);
            statuses_ = new Dictionary<T, Status>(comparer);
            deletions_ = new List<T>();
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="RelationSet{T}"/> class that uses the specified equality
        ///   comparer for the set type, and has sufficient capacity to accommodate <paramref name="capacity"/>
        ///   elements.
        /// </summary>
        /// <param name="capacity">
        ///   The initial size of the <see cref="RelationSet{T}"/>.
        /// </param>
        /// <param name="comparer">
        ///   The <see cref="IEqualityComparer{T}"/> implementation to use when comparing values in the set, or
        ///   <see langword="null"/> to use the default <see cref="IEqualityComparer{T}"/> implementation for the set
        ///   type.
        /// </param>
        public RelationSet(int capacity, IEqualityComparer<T>? comparer) {
            impl_ = new HashSet<T>(capacity, comparer);
            statuses_ = new Dictionary<T, Status>(capacity, comparer);
            deletions_ = new List<T>();
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="RelationSet{T}"/> class that uses the specfied equality
        ///   comparer for the set type, contains elements copied for the specified collection, and has sufficient
        ///   capacity to accommodate the number of elements copied.
        /// </summary>
        /// <param name="collection">
        ///   The collection whose elements are copied to the new set.
        /// </param>
        /// <param name="comparer">
        ///   The <see cref="IEqualityComparer{T}"/> implementation to use when comparing values in the set, or
        ///   <see langword="null"/> to use the default <see cref="EqualityComparer{T}"/> implementation for the set
        ///   type.
        /// </param>
        public RelationSet(IEnumerable<T> collection, IEqualityComparer<T>? comparer) {
            impl_ = new HashSet<T>(collection, comparer);
            statuses_ = new Dictionary<T, Status>(
                impl_.Select(v => new KeyValuePair<T, Status>(v, Status.New)), comparer
            );
            deletions_ = new List<T>();
        }

        // **************************************** METHODS *****************************************

        /// <inheritdoc/>
        public bool Add(T item) {
            if (deletions_.Contains(item)) {
                Debug.Assert(!impl_.Contains(item));
                deletions_.Remove(item);
                impl_.Add(item);
                statuses_[item] = Status.Saved;
                return true;
            }

            if (impl_.Add(item)) {
                Debug.Assert(!statuses_.ContainsKey(item));
                statuses_[item] = Status.New;
                return true;
            }

            return false;
        }

        /// <inheritdoc/>
        public void Clear() {
            foreach (var item in impl_) {
                Remove(item);
            }
            statuses_.Clear();
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public bool Contains(T item) {
            return impl_.Contains(item);
        }

        /// <summary>
        ///   Copies the elements of a <see cref="RelationSet{T}"/> object to an array.
        /// </summary>
        /// <param name="array">
        ///   The one-dimensional array that is the destination of the elements copied from the
        ///   <see cref="RelationSet{T}"/> object. The array must have zero-based indexing.
        /// </param>
        [ExcludeFromCodeCoverage]
        public void CopyTo(T[] array) {
            impl_.CopyTo(array);
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public void CopyTo(T[] array, int arrayIndex) {
            impl_.CopyTo(array, arrayIndex);
        }

        /// <summary>
        ///   Copies the specified number of elements of a <see cref="RelationSet{T}"/> object to an array, starting at
        ///   the specified array index.
        /// </summary>
        /// <param name="array">
        ///   The one-dimensional array that is the destination of the elements copied from the
        ///   <see cref="RelationSet{T}"/> object. The array must have zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">
        ///   The zero-based index in <paramref name="array"/> at which copying begins.
        /// </param>
        /// <param name="count">
        ///   The number of elements to copy to <paramref name="array"/>.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="arrayIndex"/> is less than <c>0</c>
        ///     --or--
        ///   <paramref name="count"/> is less than <c>0</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   <paramref name="arrayIndex"/> is greater than the length of the destination <paramref name="array"/>
        ///     --or--
        ///   <paramref name="count"/> is greater than the available space from the <paramref name="arrayIndex"/> to
        ///   the end of the destination <paramref name="array"/>.
        /// </exception>
        [ExcludeFromCodeCoverage]
        public void CopyTo(T[] array, int arrayIndex, int count) {
            impl_.CopyTo(array, arrayIndex, count);
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public sealed override bool Equals(object? obj) {
            return (obj is RelationSet<T> set) &&
                   impl_.Equals(set.impl_) &&
                   statuses_.Equals(set.statuses_) &&
                   deletions_.Equals(set.deletions_);
        }

        /// <inheritdoc/>
        public void ExceptWith(IEnumerable<T> other) {
            foreach (var item in other) {
                // No need for a Contains(...) check here for two reasons. Mathematically, there is nothing wrong with
                // removing an item that doe not exist - it will be a functional no-op. Programmatically, the Remove
                // function will already do the Contains(...) check, so doing it here would be duplicating work.
                Remove(item);
            }
        }

        /// <summary>
        ///   Returns an enumerator that iterates through a <see cref="RelationSet{T}"/> object.
        /// </summary>
        /// <returns>
        ///   A <see cref="HashSet{T}.Enumerator"/> object for the <see cref="RelationSet{T}"/> object.
        /// </returns>
        [ExcludeFromCodeCoverage]
        public HashSet<T>.Enumerator GetEnumerator() {
            return impl_.GetEnumerator();
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public sealed override int GetHashCode() {
            return HashCode.Combine(impl_.GetHashCode(), statuses_.GetHashCode(), deletions_.GetHashCode());
        }
                
        /// <inheritdoc/>
        public void IntersectWith(IEnumerable<T> other) {
            foreach (var item in impl_) {
                if (!other.Contains(item)) {
                    Remove(item);
                }
            }
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public bool IsProperSubsetOf(IEnumerable<T> other) {
            return impl_.IsProperSubsetOf(other);
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public bool IsProperSupersetOf(IEnumerable<T> other) {
            return impl_.IsProperSupersetOf(other);
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public bool IsSubsetOf(IEnumerable<T> other) {
            return impl_.IsSubsetOf(other);
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public bool IsSupersetOf(IEnumerable<T> other) {
            return impl_.IsSupersetOf(other);
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public bool Overlaps(IEnumerable<T> other) {
            return impl_.Overlaps(other);
        }

        /// <inheritdoc/>
        public bool Remove(T item) {
            if (impl_.Contains(item)) {
                Debug.Assert(!deletions_.Contains(item));
                if (statuses_[item] == Status.Saved) {
                    deletions_.Add(item);
                }

                impl_.Remove(item);
                statuses_.Remove(item);
                return true;
            }
            return false;
        }

        /// <summary>
        ///   Removes all elements that match the conditions defined by the specified predicate from a
        ///   <see cref="RelationSet{T}"/> collection.
        /// </summary>
        /// <param name="match">
        ///   The <see cref="Predicate{T}"/> delegate that defines the conditions of the elements to remove.
        /// </param>
        /// <returns>
        ///   The number of elements that were removed from the <see cref="RelationSet{T}"/> collection.
        /// </returns>
        public int RemoveWhere(Predicate<T> match) {
            var oldCount = impl_.Count;
            foreach (var item in impl_.Where(v => match(v))) {
                Remove(item);
            }
            return oldCount - impl_.Count;
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public bool SetEquals(IEnumerable<T> other) {
            return impl_.SetEquals(other);
        }

        /// <inheritdoc/>
        public void SymmetricExceptWith(IEnumerable<T> other) {
            var carryovers = new List<T>();

            foreach (var item in other) {
                if (!impl_.Contains(item)) {
                    carryovers.Add(item);
                }
            }
            foreach (var item in impl_) {
                if (other.Contains(item)) {
                    Remove(item);
                }
            }
            foreach (var item in carryovers) {
                Add(item);
            }
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public sealed override string ToString() {
            var builder = new StringBuilder();
            foreach (var item in impl_) {
                builder.AppendLine($"{item} [{statuses_[item]}]");
            }

            return builder.ToString();
        }

        /// <summary>
        ///   Sets the capacity of the <see cref="RelationSet{T}"/> object to the actual number of elements it
        ///   contains, rounded up to a nearby, immplementation-specific value.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public void TrimExcess() {
            impl_.TrimExcess();
            statuses_.TrimExcess();
            deletions_.TrimExcess();
        }

        /// <summary>
        ///   Searches the set for a given value and returns the equal value it finds, if any.
        /// </summary>
        /// <param name="equalValue">
        ///   The value to search for.
        /// </param>
        /// <param name="actualValue">
        ///   The value from the set that the search found, or the default value of <typeparamref name="T"/> when the
        ///   search yielded no match.
        /// </param>
        /// <returns>
        ///   A value indicating whether the search was successful.
        /// </returns>
        [ExcludeFromCodeCoverage]
        public bool TryGetValue(T equalValue, out T actualValue) {
            return impl_.TryGetValue(equalValue, out actualValue!);
        }

        /// <inheritdoc/>
        public void UnionWith(IEnumerable<T> other) {
            foreach (var item in other) {
                Add(item);
            }
        }

        // ******************************** EXPLICIT INTERFACE IMPLS ********************************

        /// <inheritdoc/>
        Type IRelation.ConnectionType => typeof(T);

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        bool ICollection<T>.IsReadOnly => (impl_ as ICollection<T>).IsReadOnly;

        /// <inheritdoc/>
        void ICollection<T>.Add(T item) {
            Add(item);
        }

        /// <inheritdoc/>
        void IRelation.Canonicalize() {
            deletions_.Clear();
            foreach (var item in impl_) {
                Debug.Assert(statuses_.ContainsKey(item));
                statuses_[item] = Status.Saved;
            }
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        IEnumerator<T> IEnumerable<T>.GetEnumerator() {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        IEnumerator<(object Item, Status Status)> IRelation.GetEnumerator() {
            foreach (var item in deletions_) {
                yield return (item, Status.Deleted);
            }
            foreach (var item in impl_) {
                yield return (item, statuses_[item]);
            }
        }

        /// <inheritdoc/>
        void IRelation.Repopulate(object item) {
            Debug.Assert(item.GetType() == typeof(T));
            Debug.Assert(deletions_.Count == 0);
            Debug.Assert(statuses_.Values.All(v => v == Status.Saved));
            Debug.Assert(!impl_.Contains((T)item));

            var castedItem = (T)item;
            impl_.Add(castedItem);
            statuses_[castedItem] = Status.Saved;
        }

        // ************************************ MEMBER VARIABLES ************************************

        private readonly HashSet<T> impl_;
        private readonly Dictionary<T, Status> statuses_;
        private readonly List<T> deletions_;
    }

    /// <summary>
    ///   An interface denoting a read-only view over a <see cref="RelationSet{T}"/>.
    /// </summary>
    /// <remarks>
    ///   This interface is intended to allow class authors to expose a relation through a read-only property while
    ///   controlling mutating operations on the underlying relation via member functions. Users would call, e.g., an
    ///   insertion function that essentially "intercepts" the call to the underlying collection's mutator, permitting
    ///   validation or ordering or logging or any other custom behavior.
    /// </remarks>
    /// <typeparam name="T">
    ///   The type of element to be stored in the collection.
    /// </typeparam>
    public interface IReadOnlyRelationSet<T> : IReadOnlySet<T>, IRelation where T : notnull {}
}
