using Ardalis.GuardClauses;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Kvasir.Relations {
    /// <summary>
    ///   An ordered collection that tracks the state of its elements for interaction with a back-end database.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     A <see cref="RelationList{T}"/> implements the same interfaces as and behaves identically to a standard
    ///     <see cref="List{T}"/> collection. Operations that mutate the collection are used to track changes that can
    ///     then be reflected in a back-end database, while view- or read-only operations have no additional side
    ///     effects. Converting a <see cref="RelationList{T}"/> into another collection type, such as through a member
    ///     API (e.g. <see cref="CopyTo(T[])"/>) or LINQ, drops the change tracking capabilities.
    ///   </para>
    ///   <para>
    ///     Every item in a <see cref="RelationList{T}"/> is in one of three state: <c>NEW</c>, <c>SAVED</c>, or
    ///     <c>DELETED</c>. Each state corresponds to the action or actions that should be taken with respect to that
    ///     item to synchronize the back-end database table corresponding to the relation. An item enters the
    ///     <c>NEW</c> state when it is first added; when the collection is canonicalized, each <c>NEW</c> item
    ///     transitions to the <c>SAVED</c> state, indicating that it does not need to be written to the database on
    ///     the next write. When a <c>SAVED</c> item is removed from the collection, it transitions to the
    ///     <c>DELETED</c> state; <c>NEW</c> items do not transition to <c>DELETED</c>. Note that if a <c>SAVED</c>
    ///     item is deleted and then re-added, it will be re-added in the <c>SAVED</c> state.
    ///   </para>
    ///   <para>
    ///     Items used in a <see cref="RelationList{T}"/> should be immutable: structs, <see cref="string"/>, etc. This
    ///     is because read access is <i>not</i> tracked: when using mutable elements, it is possible for the user to
    ///     access an item (e.g. through <c>[]</c>) and mutate that element without the collection knowing, preventing
    ///     that change from being reflected in the back-end database. This also means that actions that convert the
    ///     collection into another form will <i>copy</i> the elements, ensuring that the tracking data remains
    ///     up-to-date.
    ///   </para>
    ///   <para>
    ///     A <see cref="RelationList{T}"/> technically permits duplicate elements, though it is strongly advised that
    ///     users treat the collection as more of an ordered set, as the back-end relational database table will
    ///     not permit duplicates. For example, it is possible for the collection to expose a single item in multiple
    ///     seemingly incompatible states (e.g. <c>SAVED</c> and <c>DELETED</c>). Though different search APIs enable
    ///     the use of custom comparators, the internal comparison logic always uses the default comparison.
    ///   </para>
    /// </remarks>
    /// <typeparam name="T">
    ///   The type of element to be stored in the collection.
    /// </typeparam>
    /// <seealso cref="RelationSet{T}"/>
    /// <seealso cref="RelationMap{TKey, TValue}"/>
    /// <seealso cref="RelationOrderedList{T}"/>
    public sealed class RelationList<T> : IList, IList<T>, IReadOnlyList<T>, IReadOnlyRelationList<T>, IRelation
        where T : notnull {

        // *************************************** PROPERTIES ***************************************

        /// <summary>
        ///   Gets or sets the total number of elements the internal data structure can hold without resizing.
        /// </summary>
        /// <value>
        ///   The number of elements that the <see cref="RelationList{T}"/> can contain before resizing is required.
        /// </value>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   if <see cref="Capacity"/> is set to a value that is less than <see cref="Count"/>.
        /// </exception>
        /// <exception cref="OutOfMemoryException">
        ///   if there is not enough memory available on the system.
        /// </exception>
        public int Capacity {
            get {
                return impl_.Capacity;
            }
            set {
                impl_.Capacity = value;
            }
        }

        /// <inheritdoc/>
        public int Count => impl_.Count;

        /// <inheritdoc/>
        public T this[int index] {
            get {
                return impl_[index];
            }
            set {
                BookkeepRemoval(index);
                statuses_[index] = BookkeepAddition(value);
                impl_[index] = value;
            }
        }

        // ************************************** CONSTRUCTORS **************************************

        /// <summary>
        ///   Initializes a new instance of the <see cref="RelationList{T}"/> class that is empty and has the default
        ///   initial capacity.
        /// </summary>
        public RelationList() {
            impl_ = new List<T>();
            statuses_ = new List<Status>();
            deletions_ = new List<T>();
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="RelationList{T}"/> class that contains elements copied from
        ///   the specified collection and has sufficient capacity to accommodate the number of elements copied.
        /// </summary>
        /// <param name="collection">
        ///   The element whose names are copied to the new list.
        /// </param>
        public RelationList(IEnumerable<T> collection) {
            impl_ = new List<T>(collection);
            statuses_ = Enumerable.Repeat(Status.New, impl_.Count).ToList();
            deletions_ = new List<T>();
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="RelationList{T}"/> class that is empty and has the specified
        ///   initial capacity.
        /// </summary>
        /// <param name="capacity">
        ///   The number of elements that the new list can initially store.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   if <paramref name="capacity"/> is less than 0.
        /// </exception>
        public RelationList(int capacity) {
            impl_ = new List<T>(capacity);
            statuses_ = new List<Status>(capacity);
            deletions_ = new List<T>();
        }

        // **************************************** METHODS *****************************************

        /// <inheritdoc/>
        public void Add(T item) {
            statuses_.Add(BookkeepAddition(item));
            impl_.Add(item);
        }

        /// <summary>
        ///   Adds the elements of the specified collection to the end of the <see cref="RelationList{T}"/>.
        /// </summary>
        /// <param name="collection">
        ///   The collection whose elements should be added to the end of the <see cref="RelationList{T}"/>. The
        ///   collection itself cannot be <see langword="null"/>, but it can contain elements that are
        ///   <see langword="null"/>, if type <typeparamref name="T"/> is a reference type.
        /// </param>
        public void AddRange(IEnumerable<T> collection) {
            foreach (var item in collection) {
                statuses_.Add(BookkeepAddition(item));
                impl_.Add(item);
            }
        }

        /// <summary>
        ///   Returns a read-only <see cref="ReadOnlyCollection{T}"/> wrapper for the current collection.
        /// </summary>
        /// <returns>
        ///   An object that acts as a read-only wrapper around the current <see cref="RelationList{T}"/>.
        /// </returns>
        [ExcludeFromCodeCoverage]
        public ReadOnlyCollection<T> AsReadOnly() {
            return impl_.AsReadOnly();
        }

        /// <summary>
        ///   Searches the entire sorted <see cref="RelationList{T}"/> for an element using the default comparer and
        ///   returns the zero-based index of the element.
        /// </summary>
        /// <param name="item">
        ///   The object to locate. The value can be <see langword="null"/> for reference types.
        /// </param>
        /// <returns>
        ///   The zero-based index of <paramref name="item"/> in the sorted <see cref="RelationList{T}"/>, if
        ///   <paramref name="item"/> is found; otherwise, a negative number that is the bitwise complement of the
        ///   index of the new element that is larger than <paramref name="item"/> or, if there is no larger element,
        ///   the bitwise complement of <see cref="Count"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        ///   if the default comparer <see cref="Comparer{T}.Default"/> cannot find an implementation of the
        ///   <see cref="IComparable{T}"/> generic interface or the <see cref="IComparable"/> interface for type
        ///   <typeparamref name="T"/>.
        /// </exception>
        [ExcludeFromCodeCoverage]
        public int BinarySearch(T item) {
            return impl_.BinarySearch(item);
        }

        /// <summary>
        ///   Searches the entire sorted <see cref="RelationList{T}"/> for an element using the specified comparer and
        ///   returns the zero-based index of the element.
        /// </summary>
        /// <param name="item">
        ///   The object to locate. The value can be <see langword="null"/> for reference types.
        /// </param>
        /// <param name="comparer">
        ///   The <see cref="IComparer{T}"/> implementation to use when comparing elements; or, <see langword="null"/>
        ///   to use the default comparer <see cref="Comparer{T}.Default"/>.
        /// </param>
        /// <returns>
        ///   The zero-based index of <paramref name="item"/> in the sorted <see cref="RelationList{T}"/>, if
        ///   <paramref name="item"/> is found; otherwise, a negative number that is the bitwise complement of the
        ///   index of the next element that is larger than <paramref name="item"/> or, if there is no larger element,
        ///   the bitwise complement of <see cref="Count"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        ///   if <paramref name="comparer"/> is <see langword="null"/>, and the default comparer
        ///   <see cref="Comparer{T}.Default"/> cannot find an implementation of the <see cref="IComparable{T}"/>
        ///   generic interface or the <see cref="IComparable"/> interface for the type <typeparamref name="T"/>.
        /// </exception>
        [ExcludeFromCodeCoverage]
        public int BinarySearch(T item, Comparer<T>? comparer) {
            return impl_.BinarySearch(item, comparer);
        }

        /// <summary>
        ///   Searches a range of elements in the sorted <see cref="RelationList{T}"/> for an element using the
        ///   specified comparer and returns the zero-based index of the element.
        /// </summary>
        /// <param name="index">
        ///   The zero-based starting index of the range to search.
        /// </param>
        /// <param name="count">
        ///   The length of the range to search.
        /// </param>
        /// <param name="item">
        ///   The object to locate. The value can be <see langword="null"/> for reference types.
        /// </param>
        /// <param name="comparer">
        ///   The <see cref="IComparer{T}"/> implementation to use when comparing elements, or <see langword="null"/>
        ///   to use the default comparer <see cref="Comparer{T}.Default"/>.
        /// </param>
        /// <returns>
        ///   The zero-based index of <paramref name="item"/> in the sorted <see cref="RelationList{T}"/>, if
        ///   <paramref name="item"/> is found; otherwise, a negative number that is the bitwise complement of the
        ///   index of the next element that is larger than <paramref name="item"/> or, if there is no larger element,
        ///   the bitwise complement of <see cref="Count"/>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   if <paramref name="item"/> is less than <c>0</c>
        ///     --or--
        ///   if <paramref name="count"/> is less than <c>0</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   if <paramref name="index"/> and <paramref name="count"/> do not denote a valid range in the
        ///   <see cref="RelationList{T}"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///   <paramref name="comparer"/> is <see langword="null"/>, and the default comparer
        ///   <see cref="Comparer{T}.Default"/> cannot find an implementation of the <see cref="IComparable{T}"/>
        ///   generic interface or the <see cref="IComparable"/> interface for type <typeparamref name="T"/>.
        /// </exception>
        [ExcludeFromCodeCoverage]
        public int BinarySearch(int index, int count, T item, Comparer<T>? comparer) {
            return impl_.BinarySearch(index, count, item, comparer);
        }

        /// <inheritdoc/>
        public void Clear() {
            for (int idx = 0; idx < impl_.Count; ++idx) {
                BookkeepRemoval(idx);
            }
            impl_.Clear();
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public bool Contains(T item) {
            return impl_.Contains(item);
        }

        /// <summary>
        ///   Converts the elements in the current <see cref="RelationList{T}"/> to another type, and returns a list
        ///   containing the converted elements.
        /// </summary>
        /// <typeparam name="TOutput">
        ///   The type of the elements of the target array.
        /// </typeparam>
        /// <param name="converter">
        ///   A <see cref="Converter{TInput, TOutput}"/> delegate that converts each element from one type to another
        ///   type.
        /// </param>
        /// <returns>
        ///   A <see cref="List{T}"/> of the target type containing the converted elements from the current
        ///   <see cref="RelationList{T}"/>.
        /// </returns>
        [ExcludeFromCodeCoverage]
        public List<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter) {
            return impl_.ConvertAll(converter);
        }

        /// <summary>
        ///   Copies the entire <see cref="RelationList{T}"/> to a compatible one-dimensional array, starting at the
        ///   beginning of the target array.
        /// </summary>
        /// <param name="array">
        ///   The one-dimensional <see cref="Array"/> that is the destination of the elements copied from the
        ///   <see cref="RelationList{T}"/>. The <see cref="Array"/> must have zero-based indexing.
        /// </param>
        /// <exception cref="ArgumentException">
        ///   if the number of elements in the source <see cref="RelationList{T}"/> is greater than the number of
        ///   elements that the destination <paramref name="array"/> can contain.
        /// </exception>
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
        ///   Copies a range of elements from the <see cref="RelationList{T}"/> to a compatible one-dimensional array,
        ///   starting at the specified index of the target array.
        /// </summary>
        /// <param name="index">
        ///   The zero-based index in the source <see cref="RelationList{T}"/> at which copying begins.
        /// </param>
        /// <param name="array">
        ///   The one-dimensional <see cref="Array"/> that is the destination of the elements copied from the
        ///   <see cref="RelationList{T}"/>. The <see cref="Array"/> must have zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">
        ///   The zero-based index in <paramref name="array"/> at which copying begins.
        /// </param>
        /// <param name="count">
        ///   The number of elements to copy.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   if <paramref name="index"/> is less than <c>0</c>
        ///     --or--
        ///   if <paramref name="arrayIndex"/> is less than <c>0</c>
        ///     --or--
        ///   if <paramref name="count"/> is less than <c>0</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   if <paramref name="index"/> is equal to or greater than the <see cref="Count"/> of the source
        ///   <see cref="RelationList{T}"/>
        ///     --or--
        ///   if the number of elements from <paramref name="index"/> to the end of the source
        ///   <see cref="RelationList{T}"/> is greater than the available space from <paramref name="arrayIndex"/> to
        ///   the end of the destination <paramref name="array"/>.
        /// </exception>
        [ExcludeFromCodeCoverage]
        public void CopyTo(int index, T[] array, int arrayIndex, int count) {
            impl_.CopyTo(index, array, arrayIndex, count);
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public sealed override bool Equals(object? obj) {
            return (obj is RelationList<T> list) &&
                   impl_.Equals(list.impl_) &&
                   statuses_.Equals(list.statuses_) &&
                   deletions_.Equals(list.deletions_);
        }

        /// <summary>
        ///   Determines whether the <see cref="RelationList{T}"/> contains elements that match the conditions defined
        ///   by the specified predicate.
        /// </summary>
        /// <param name="match">
        ///   The <see cref="Predicate{T}"/> delegate that defines the conditions of the elements to search for.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if the <see cref="RelationList{T}"/> contains one or more elements that match the
        ///   conditions defined by the specified predicate; otherwise, <see langword="false"/>.
        /// </returns>
        [ExcludeFromCodeCoverage]
        public bool Exists(Predicate<T> match) {
            return impl_.Exists(match);
        }

        /// <summary>
        ///   Searches for an element that matches the conditions defined by the specified predicate, and returns the
        ///   first occurrence within the entire <see cref="RelationList{T}"/>.
        /// </summary>
        /// <param name="match">
        ///   The <see cref="Predicate{T}"/> delegate that defines the conditions of the element to search for.
        /// </param>
        /// <returns>
        ///   The first element that matches the condition defined by the specified predicate, if found; otherwise, the
        ///   default value for type <typeparamref name="T"/>.
        /// </returns>
        [ExcludeFromCodeCoverage]
        public T? Find(Predicate<T> match) {
            return impl_.Find(match);
        }

        /// <summary>
        ///   Retrieves all the elements that match the conditions defined by the specified predicate.
        /// </summary>
        /// <param name="match">
        ///   The <see cref="Predicate{T}"/> delegate that defines the conditions of the elements to search for.
        /// </param>
        /// <returns>
        ///   A <see cref="List{T}"/> containing all the elements that match the conditions defined by the specified
        ///   predicate, if found; otherwise, an empty <see cref="List{T}"/>.
        /// </returns>
        [ExcludeFromCodeCoverage]
        public List<T> FindAll(Predicate<T> match) {
            return impl_.FindAll(match);
        }

        /// <summary>
        ///   Searches for an element that matches the conditions defined by the specified predicate, and returns the
        ///   zero-based index of the first occurrence within the entire <see cref="RelationList{T}"/>.
        /// </summary>
        /// <param name="match">
        ///   The <see cref="Predicate{T}"/> delegate that defines the conditions of the element to search for.
        /// </param>
        /// <returns>
        ///   The zero-based index of the first occurrence of an element that matches the conditions defined by
        ///   <paramref name="match"/>, if found; otherwise, <c>-1</c>.
        /// </returns> 
        [ExcludeFromCodeCoverage]
        public int FindIndex(Predicate<T> match) {
            return impl_.FindIndex(match);
        }

        /// <summary>
        ///   Searches for an element that matches the conditions defined by the specified predicate, and returns the
        ///   zero-based index of the first occurrence within the range of elements in the
        ///   <see cref="RelationList{T}"/> that extends from the specified index to the last element.
        /// </summary>
        /// <param name="startIndex">
        ///   The zero-based starting index of the search.
        /// </param>
        /// <param name="match">
        ///   The <see cref="Predicate{T}"/> delegate that defines the conditions of the element to search for.
        /// </param>
        /// <returns>
        ///   The zero-based index of the first occurrence of an element that matches the conditions defined by
        ///   <paramref name="match"/>, if found; otherwise, <c>-1</c>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///    if <paramref name="startIndex"/> is outside the range of valid indexes for the
        ///    <see cref="RelationList{T}"/>.
        /// </exception>
        [ExcludeFromCodeCoverage]
        public int FindIndex(int startIndex, Predicate<T> match) {
            return impl_.FindIndex(startIndex, match);
        }

        /// <summary>
        ///   Searches for an element that matches the conditions defined by the specified predicate, and returns the
        ///   zero-based index of the first occurrence within the range of elements in the
        ///   <see cref="RelationList{T}"/> that starts at the specified index and contains the specified number of
        ///   elements.
        /// </summary>
        /// <param name="startIndex">
        ///    The zero-based starting index of the search.
        /// </param>
        /// <param name="count">
        ///   The number of elements in the section to search.
        /// </param>
        /// <param name="match">
        ///   The <see cref="Predicate{T}"/> delegate that defines the conditions of the elements to search for.
        /// </param>
        /// <returns>
        ///   The zero-based index of the first occurrence of an element that matches the conditions defined by
        ///   <paramref name="match"/>, if found; otherwise, <c>-1</c>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///    if <paramref name="startIndex"/> is outside the range of valid indexes for the
        ///    <see cref="RelationList{T}"/>
        ///      --or--
        ///    if <paramref name="count"/> is less than <c>0</c>
        ///      --or--
        ///    if <paramref name="startIndex"/> and <paramref name="count"/> do not specify a valid section in the
        ///    <see cref="RelationList{T}"/>.
        /// </exception>
        [ExcludeFromCodeCoverage]
        public int FindIndex(int startIndex, int count, Predicate<T> match) {
            return impl_.FindIndex(startIndex, count, match);
        }

        /// <summary>
        ///   Searches for an element that matches the conditions defined by the specified predicate, and returns the
        ///   last occurrence within the entire <see cref="RelationList{T}"/>.
        /// </summary>
        /// <param name="match">
        ///   The <see cref="Predicate{T}"/> delegate that defines the conditions of the element to search for.
        /// </param>
        /// <returns>
        ///   The last element that matches the conditions defined by the specified predicate, if found; otherwise, the
        ///   default value for type <typeparamref name="T"/>.
        /// </returns>
        [ExcludeFromCodeCoverage]
        public T? FindLast(Predicate<T> match) {
            return impl_.FindLast(match);
        }

        /// <summary>
        ///   Searches for an element that matches the conditions defined by the specified predicate, and returns the
        ///   zero-based index of the last occurrence within the entire <see cref="RelationList{T}"/>.
        /// </summary>
        /// <param name="match">
        ///   The <see cref="Predicate{T}"/> delegate that defines the conditions of the element to search for.
        /// </param>
        /// <returns>
        ///   The zero-based index of the last occurrence of an element that matches the conditions defined by
        ///   <paramref name="match"/>, if found; otherwise, <c>-1</c>.
        /// </returns>
        [ExcludeFromCodeCoverage]
        public int FindLastIndex(Predicate<T> match) {
            return impl_.FindLastIndex(match);
        }

        /// <summary>
        ///   Searches for an element that matches the conditions defined by the specified predicate, and returns the
        ///   zero-based index of the last occurrence within the range of elements in the <see cref="RelationList{T}"/>
        ///   that extends from the first element to the specified index.
        /// </summary>
        /// <param name="startIndex">
        ///   The zero-based starting index of the backward search.
        /// </param>
        /// <param name="match">
        ///   The <see cref="Predicate{T}"/> delegate that defines the conditions of the element to search for.
        /// </param>
        /// <returns>
        ///   The zero-based index of the last occurrence of an element that matches the conditions defined by
        ///   <paramref name="match"/>, if found; otherwise, <c>-1</c>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   if <paramref name="startIndex"/> is outside the range of valid indexes for the
        ///   <see cref="RelationList{T}"/>.
        /// </exception>
        [ExcludeFromCodeCoverage]
        public int FindLastIndex(int startIndex, Predicate<T> match) {
            return impl_.FindLastIndex(startIndex, match);
        }

        /// <summary>
        ///   Searches for an element that matches the conditions defined by the specified predicate, and returns the
        ///   zero-based index of the last occurrence within the range of elements in the <see cref="RelationList{T}"/>
        ///   that contains the specified number of elements and ends at the specified index.
        /// </summary>
        /// <param name="startIndex">
        ///   The zero-based starting index of the backward search.
        /// </param>
        /// <param name="count">
        ///   The number of elements in the section to search.
        /// </param>
        /// <param name="match">
        ///   The <see cref="Predicate{T}"/> delegate that defines the conditions of the element to search for.
        /// </param>
        /// <returns>
        ///   The zero-based index of the last occurrence of an element that matches the conditions defined by
        ///   <paramref name="match"/>, if found; otherwise, <c>-1</c>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   if <paramref name="startIndex"/> is outside the range of valid indexes for the
        ///   <see cref="RelationList{T}"/>
        ///     --or--
        ///   if <paramref name="count"/> is less than <c>0</c>
        ///     --or--
        ///   if <paramref name="startIndex"/> and <paramref name="count"/> do not specify a valid section in the
        ///   <see cref="RelationList{T}"/>.
        /// </exception>
        [ExcludeFromCodeCoverage]
        public int FindLastIndex(int startIndex, int count, Predicate<T> match) {
            return impl_.FindLastIndex(startIndex, count, match);
        }

        /// <summary>
        ///   Performs the specified action on each element of the <see cref="RelationList{T}"/>.
        /// </summary>
        /// <param name="action">
        ///   The <see cref="Action{T}"/> delegate to perform on each element of the <see cref="RelationList{T}"/>.
        /// </param>
        /// <exception cref="InvalidOperationException">
        ///   if an element in the collection has been modified.
        /// </exception>
        [ExcludeFromCodeCoverage]
        public void ForEach(Action<T> action) {
            impl_.ForEach(action);
        }

        /// <summary>
        ///   Returns an enumerator that iterates through the <see cref="RelationList{T}"/>.
        /// </summary>
        /// <returns>
        ///   A <see cref="List{T}.Enumerator"/> for the <see cref="RelationList{T}"/>.
        /// </returns>
        [ExcludeFromCodeCoverage]
        public List<T>.Enumerator GetEnumerator() {
            return impl_.GetEnumerator();
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public sealed override int GetHashCode() {
            return HashCode.Combine(impl_.GetHashCode(), statuses_.GetHashCode(), deletions_.GetHashCode());
        }

        /// <summary>
        ///   Creates a shallow copy of a range of elements in the source <see cref="RelationList{T}"/>.
        /// </summary>
        /// <param name="index">
        ///   The zero-based <see cref="RelationList{T}"/> index at which the range stats.
        /// </param>
        /// <param name="count">
        ///   The number of elements in the range.
        /// </param>
        /// <returns>
        ///   A shallow copy of a range of elements in the source <see cref="RelationList{T}"/>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   if <paramref name="index"/> is less than <c>0</c>
        ///     --or--
        ///   if <paramref name="count"/> is less than <c>0</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   if <paramref name="index"/> and <paramref name="count"/> do not denote a valid range of elements in the
        ///   <see cref="RelationList{T}"/>.
        /// </exception>
        [ExcludeFromCodeCoverage]
        public List<T> GetRange(int index, int count) {
            return impl_.GetRange(index, count);
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public int IndexOf(T item) {
            return impl_.IndexOf(item);
        }

        /// <summary>
        ///   Searches for the specified object and returns the zero-based index of the first occurrence within the
        ///   range of elements in the <see cref="RelationList{T}"/> that extends from the specified index to the last
        ///   element.
        /// </summary>
        /// <param name="item">
        ///   The object to locate in the <see cref="RelationList{T}"/>. The value can be <see langword="null"/> for
        ///   reference types.
        /// </param>
        /// <param name="index">
        ///   The zero-based starting index of the search. <c>0</c> (zero) is valid in an empty list.
        /// </param>
        /// <returns>
        ///   The zero-based index of the first occurrence of <paramref name="item"/> within the range of elements in
        ///   the <see cref="RelationList{T}"/> that extends from <paramref name="index"/> to the last element, if
        ///   found; otherwise, <c>-1</c>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="index"/> is outside the range of valid indexes for the <see cref="RelationList{T}"/>.
        /// </exception>
        [ExcludeFromCodeCoverage]
        public int IndexOf(T item, int index) {
            return impl_.IndexOf(item, index);
        }

        /// <summary>
        ///   Searches for the specified object and returns the zero-based index of the first occurrence within the
        ///   range of elements in the <see cref="RelationList{T}"/> that starts at the specified index and contains
        ///   the specified number of elements.
        /// </summary>
        /// <param name="item">
        ///   The object to locate in the <see cref="RelationList{T}"/>. The value can be <see langword="null"/> for
        ///   reference types.
        /// </param>
        /// <param name="index">
        ///   The zero-based starting index of the search. <c>0</c> (zero) is valid in an empty list.
        /// </param>
        /// <param name="count">
        ///   The number of elements in the section to search.
        /// </param>
        /// <returns>
        ///   The zero-based index of the first occurrence of <paramref name="item"/> within the range of elements in
        ///   the <see cref="RelationList{T}"/> that starts at <paramref name="index"/> and contains
        ///   <paramref name="count"/> number of elements, if found; otherwise, <c>-1</c>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   if <paramref name="index"/> is outside the range of valid indexes of the <see cref="RelationList{T}"/>
        ///     --or--
        ///   if <paramref name="count"/> is less than <c>0</c>
        ///     --or--
        ///   if <paramref name="index"/> and <paramref name="count"/> do not specify a valid section in the
        ///   <see cref="RelationList{T}"/>.
        /// </exception>
        [ExcludeFromCodeCoverage]
        public int IndexOf(T item, int index, int count) {
            return impl_.IndexOf(item, index, count);
        }

        /// <inheritdoc/>
        public void Insert(int index, T item) {
            statuses_.Insert(index, Status.New);                // do this first to trigger exception checking
            statuses_[index] = BookkeepAddition(item);
            impl_.Insert(index, item);
        }

        /// <summary>
        ///   Inserts the elements of a collection into the <see cref="RelationList{T}"/> at the specified index.
        /// </summary>
        /// <param name="index">
        ///   The zero-based index at which the new elements should be inserted.
        /// </param>
        /// <param name="collection">
        ///   The collection whose elements should be inserted into the <see cref="RelationList{T}"/>. The collection
        ///   itself cannot be <see langword="null"/>, but it can contain elements that are <see langword="null"/> if
        ///   type <typeparamref name="T"/> is a reference type.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   if <paramref name="index"/> is less than <c>0</c>
        ///     --or--
        ///   if <paramref name="index"/> is greater than <see cref="Count"/>.
        /// </exception>
        public void InsertRange(int index, IEnumerable<T> collection) {
            Guard.Against.Null(collection, nameof(collection));

            foreach (var item in collection) {
                Insert(index++, item);
            }
        }

        /// <summary>
        ///   Searches for the specified object and returns the zero-based index of the last occurrence within the
        ///   entire <see cref="RelationList{T}"/>.
        /// </summary>
        /// <param name="item">
        ///   The object to locate in the <see cref="RelationList{T}"/>. The value can be <see langword="null"/> for
        ///   reference types.
        /// </param>
        /// <returns>
        ///   The zero-based index of the last occurrence of <paramref name="item"/> within the entire
        ///   <see cref="RelationList{T}"/>, if found; otherwise, <c>-1</c>.
        /// </returns>
        [ExcludeFromCodeCoverage]
        public int LastIndexOf(T item) {
            return impl_.LastIndexOf(item);
        }

        /// <summary>
        ///   Searches for the specified object and returns the zero-based index of the last occurrence within the
        ///   range of elements in the <see cref="RelationList{T}"/> that extends from the first element to the
        ///   specified index.
        /// </summary>
        /// <param name="item">
        ///   The object to locate in the <see cref="RelationList{T}"/>. The value can be <see langword="null"/> for
        ///   reference types.
        /// </param>
        /// <param name="index">
        ///   The zero-based starting index of the backward search.
        /// </param>
        /// <returns>
        ///   The zero-based index of the last occurrence of <paramref name="item"/> within the range of elements in
        ///   the <see cref="RelationList{T}"/> that extends from the first element to <paramref name="index"/>, if
        ///   found; otherwise, <c>-1</c>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   if <paramref name="index"/> is outside the range of valid indexes for the <see cref="RelationList{T}"/>.
        /// </exception>
        [ExcludeFromCodeCoverage]
        public int LastIndexOf(T item, int index) {
            return impl_.LastIndexOf(item, index);
        }

        /// <summary>
        ///   Searches for the specified object and returns the zero-based index of the last occurrence within the
        ///   range of elements in the <see cref="RelationList{T}"/> that contains the specified number of elements and
        ///   ends at the specified index.
        /// </summary>
        /// <param name="item">
        ///   The object to locate in the <see cref="RelationList{T}"/>. The value can be <see langword="null"/> for
        ///   reference types.
        /// </param>
        /// <param name="index">
        ///   The zero-based starting index of the backward search.
        /// </param>
        /// <param name="count">
        ///   The number of elements in the section to search.
        /// </param>
        /// <returns>
        ///   The zero-based index of the last occurrence of <paramref name="item"/> within the range of elements in
        ///   the <see cref="RelationList{T}"/> that contains <paramref name="count"/> number of elements and ends at
        ///   <paramref name="index"/>, if found; otherwise, <c>-1</c>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   if <paramref name="index"/> is outside the range of valid indexes for the <see cref="RelationList{T}"/>
        ///     --or--
        ///   if <paramref name="count"/> is less than <c>0</c>
        ///     --or--
        ///   if <paramref name="index"/> and <paramref name="count"/> do not specify a valid section in the
        ///   <see cref="RelationList{T}"/>.
        /// </exception>
        [ExcludeFromCodeCoverage]
        public int LastIndexOf(T item, int index, int count) {
            return impl_.LastIndexOf(item, index, count);
        }

        /// <inheritdoc/>
        public bool Remove(T item) {
            var idx = IndexOf(item);
            if (idx == -1) {
                return false;
            }

            RemoveAt(idx);
            return true;
        }

        /// <summary>
        ///   Removes all the elements that match the conditions defined by the specified predicate.
        /// </summary>
        /// <param name="match">
        ///   The <see cref="Predicate{T}"/> delegate that defines the conditions of the elements to remove.
        /// </param>
        /// <returns>
        ///   The number of elements removed from the <see cref="RelationList{T}"/>.
        /// </returns>
        public int RemoveAll(Predicate<T> match) {
            int count = 0;
            for (int idx = impl_.Count - 1; idx >= 0; --idx) {
                if (match(impl_[idx])) {
                    ++count;
                    RemoveAt(idx);
                }
            }
            return count;
        }

        /// <inheritdoc/>
        public void RemoveAt(int index) {
            BookkeepRemoval(index);
            impl_.RemoveAt(index);
            statuses_.RemoveAt(index);
        }

        /// <summary>
        ///   Removes a range of elements from the <see cref="RelationList{T}"/>.
        /// </summary>
        /// <param name="index">
        ///   The zero-based starting index of the range of elements to remove.
        /// </param>
        /// <param name="count">
        ///   The number of elements to remove.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   if <paramref name="index"/> is less than <c>0</c>
        ///     --or--
        ///   if <paramref name="count"/> is less than <c>0</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   if <paramref name="index"/> and <paramref name="count"/> do not denote a valid range of elements in the
        ///   <see cref="RelationList{T}"/>.
        /// </exception>
        public int RemoveRange(int index, int count) {
            Guard.Against.OutOfRange(count, nameof(count), 0, int.MaxValue);
            Guard.Against.OutOfRange(index, nameof(index), 0, impl_.Count - 1);
            Guard.Against.InvalidInput(count, nameof(count), c => index + count < impl_.Count);

            for (int c = 1; c <= count; ++c) {
                RemoveAt(index);
            }
            return count;
        }

        /// <summary>
        ///   Reverses the order of the elements in the entire <see cref="RelationList{T}"/>.
        /// </summary>
        public void Reverse() {
            impl_.Reverse();
            statuses_.Reverse();
        }

        /// <summary>
        ///   Reverses the order of the elements in the specified range.
        /// </summary>
        /// <param name="index">
        ///   The zero-based staring index of the range to reverse.
        /// </param>
        /// <param name="count">
        ///   The number of elements in the range to reverse.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   if <paramref name="index"/> is less than <c>0</c>
        ///     --or--
        ///   if <paramref name="count"/> is less than <c>0</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   if <paramref name="index"/> and <paramref name="count"/> do not denote a valid range of elements in the
        ///   <see cref="RelationList{T}"/>.
        /// </exception>
        public void Reverse(int index, int count) {
            impl_.Reverse(index, count);
            statuses_.Reverse(index, count);
        }

        /// <summary>
        ///   Sorts the elements in the entire <see cref="RelationList{T}"/> using the default comparer.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        ///   if the default comparer <see cref="Comparer.Default"/> cannot find an implementation of the
        ///   <see cref="IComparable{T}"/> generic interface or the <see cref="IComparable"/> interface for type
        ///   <typeparamref name="T"/>.
        /// </exception>
        public void Sort() {
            Sort(Comparer<T>.Default);
        }

        /// <summary>
        ///   Sorts the elements in the entire <see cref="RelationList{T}"/> using the specified
        ///   <see cref="Comparison{T}"/>.
        /// </summary>
        /// <param name="comparison">
        ///   The <see cref="Comparison{T}"/> to use when comparing elements.
        /// </param>
        /// <exception cref="ArgumentException">
        ///   The implementation of <paramref name="comparison"/> caused an error during the sort. For example,
        ///   <paramref name="comparison"/> might not return <c>0</c> when comparing an item with itself.
        /// </exception>
        public void Sort(Comparison<T> comparison) {
            var tmp = new List<(T item, Status status)>(impl_.Count);
            for (int idx = 0; idx < impl_.Count; ++idx) {
                tmp.Add((impl_[idx], statuses_[idx]));
            }

            tmp.Sort((lhs, rhs) => comparison(lhs.item, rhs.item));
            for (int idx = 0; idx < impl_.Count; ++idx) {
                impl_[idx] = tmp[idx].item;
                statuses_[idx] = tmp[idx].status;
            }
        }

        /// <summary>
        ///   Sorts the elements in the entire <see cref="RelationList{T}"/> using the specified comparer.
        /// </summary>
        /// <param name="comparer">
        ///   The <see cref="IComparer{T}"/> implementation to use when comparing elements, or <see langword="null"/>
        ///   to use the default comparer <see cref="Comparer{T}.Default"/>.
        /// </param>
        /// <exception cref="InvalidOperationException">
        ///   if <paramref name="comparer"/> is <see langword="null"/>, and the default comparer
        ///   <see cref="Comparer{T}.Default"/> cannot find an implementation of the <see cref="IComparable{T}"/>
        ///   generic interface or the <see cref="IComparable"/> interface for type <typeparamref name="T"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   if the implementation of <paramref name="comparer"/> caused an error during the sort. For example,
        ///   <paramref name="comparer"/> might not return <c>0</c> when comparing an item with itself.
        /// </exception>
        public void Sort(IComparer<T>? comparer) {
            Sort((lhs, rhs) => comparer!.Compare(lhs, rhs));
        }

        /// <summary>
        ///   Sorts the elements in a range of elements in the <see cref="RelationList{T}"/> using the specified
        ///   comparer.
        /// </summary>
        /// <param name="index">
        ///   The zero-based starting index of the range to sort.
        /// </param>
        /// <param name="count">
        ///   The length of the range to sort.
        /// </param>
        /// <param name="comparer">
        ///   The <see cref="IComparer{T}"/> implementation to use when comparing elements, or <see langword="null"/>
        ///   to use the default comparer <see cref="Comparer{T}.Default"/>.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   if <paramref name="index"/> is less than <c>0</c>
        ///     --or--
        ///   if <paramref name="count"/> is less than <c>0</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   if <paramref name="index"/> and <paramref name="count"/> do not specify a valid range in the
        ///   <see cref="RelationList{T}"/>
        ///     --or--
        ///   if the implementation of <paramref name="comparer"/> caused an error during the sort. For example,
        ///   <paramref name="comparer"/> might not return <c>0</c> when comparing an item with itself.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///   if <paramref name="comparer"/> is <see langword="null"/>, and the default comparer
        ///   <see cref="Comparer{T}.Default"/> cannot find an implementation of the <see cref="IComparable{T}"/>
        ///   generic interface or the <see cref="IComparable"/> interface for type <typeparamref name="T"/>.
        /// </exception>
        public void Sort(int index, int count, IComparer<T>? comparer) {
            Guard.Against.Null(comparer, nameof(comparer));

            var tmp = new List<(T item, Status status)>(count);
            for (int idx = index; idx < index + count; ++idx) {
                tmp.Add((impl_[idx], statuses_[idx]));
            }

            tmp.Sort((lhs, rhs) => comparer.Compare(lhs.item, rhs.item));
            for (int offset = 0; offset < count; ++offset) {
                impl_[index + offset] = tmp[offset].item;
                statuses_[index + offset] = tmp[offset].status;
            }
        }

        /// <summary>
        ///   Copies the elements of the <see cref="RelationList{T}"/> to a new array.
        /// </summary>
        /// <returns>
        ///   An array containing copies of the elements of the <see cref="RelationList{T}"/>.
        /// </returns>
        [ExcludeFromCodeCoverage]
        public T[] ToArray() {
            return impl_.ToArray();
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public sealed override string? ToString() {
            var builder = new StringBuilder();
            for (int idx = 0; idx < impl_.Count; ++idx) {
                builder.AppendLine($"{impl_[idx]} [{statuses_[idx]}]");
            }

            return builder.ToString();
        }

        /// <summary>
        ///   Sets the capacity to the actual number of elements in the <see cref="RelationList{T}"/>, if that number
        ///   is less than a threshold value.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public void TrimExcess() {
            impl_.TrimExcess();
            statuses_.TrimExcess();
            deletions_.TrimExcess();
        }

        /// <summary>
        ///   Determines whether every element in the <see cref="RelationList{T}"/> matches the conditions defined by
        ///   the specified predicate.
        /// </summary>
        /// <param name="match">
        ///   The <see cref="Predicate{T}"/> delegate that defines the conditions to check against the elements.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if every element in the <see cref="RelationList{T}"/> matches the conditions
        ///   defined by the specified predicate; otherwise, <see langword="false"/>. If the list contains no elements,
        ///   the return value is <see langword="true"/>.
        /// </returns>
        [ExcludeFromCodeCoverage]
        public bool TrueForAll(Predicate<T> match) {
            return impl_.TrueForAll(match);
        }

        // ******************************** EXPLICIT INTERFACE IMPLS ********************************

        /// <inheritdoc/>
        static Type IRelation.ConnectionType => typeof(T);

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        bool IList.IsFixedSize => (impl_ as IList).IsFixedSize;

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        bool ICollection<T>.IsReadOnly => (impl_ as ICollection<T>).IsReadOnly;

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        bool IList.IsReadOnly => (impl_ as IList).IsReadOnly;

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        bool ICollection.IsSynchronized => (impl_ as ICollection).IsSynchronized;

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        object ICollection.SyncRoot => (impl_ as ICollection).SyncRoot;

        /// <inheritdoc/>
        object? IList.this[int index] {
            get {
                return this[index];
            }
            set {
                Guard.Against.Null(value, nameof(value));
                this[index] = (T)value;
            }
        }

        /// <inheritdoc/>
        int IList.Add(object? value) {
            Guard.Against.Null(value, nameof(value));
            Add((T)value);
            return impl_.Count - 1;
        }

        /// <inheritdoc/>
        void IRelation.Canonicalize() {
            deletions_.Clear();
            for (int idx = 0; idx < statuses_.Count; ++idx) {
                statuses_[idx] = Status.Saved;
            }
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        bool IList.Contains(object? value) {
            return (impl_ as IList).Contains(value);
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        void ICollection.CopyTo(Array array, int index) {
            (impl_ as ICollection).CopyTo(array, index);
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        IEnumerator<T> IEnumerable<T>.GetEnumerator() {
            return GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator<(object Item, Status Status)> IRelation.GetEnumerator() {
            for (int idx = 0; idx < deletions_.Count; ++idx) {
                yield return (deletions_[idx]!, Status.Deleted);
            }
            for (int idx = 0; idx < impl_.Count; ++idx) {
                yield return (impl_[idx]!, statuses_[idx]);
            }
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        int IList.IndexOf(object? value) {
            return (impl_ as IList).IndexOf(value);
        }

        /// <inheritdoc/>
        void IList.Insert(int index, object? value) {
            Guard.Against.Null(value, nameof(value));
            Insert(index, (T)value);
        }

        /// <inheritdoc/>
        void IList.Remove(object? value) {
            Guard.Against.Null(value, nameof(value));
            Remove((T)value);
        }

        /// <inheritdoc/>
        void IRelation.Repopulate(object item) {
            Debug.Assert(item.GetType() == typeof(T));
            Debug.Assert(deletions_.Count == 0);
            Debug.Assert(statuses_.All(s => s == Status.Saved));

            impl_.Add((T)item);
            statuses_.Add(Status.Saved);
        }

        // ************************************ HELPER FUNCTIONS ************************************

        /// <summary>
        ///   Removes an item from the deletions of the current <see cref="RelationList{T}"/> if it's there, and
        ///   determines the status of a to-be-added item.
        /// </summary>
        /// <remarks>
        ///   Note that this method does <i>NOT</i> add put the item into the collection. This method is viable for any
        ///   form of addition: insertions, overwrites, and appends. If the deletions contains multiple copies of the
        ///   item, only one instance is removed.
        /// </remarks>
        /// <param name="addition">
        ///   The item that is being added to the <see cref="RelationList{T}"/>.
        /// </param>
        /// <returns>
        ///   The <see cref="Status"/> of the item being added: either <see cref="Status.New"/> if the item was not
        ///   deleted from the <see cref="RelationList{T}"/> since the last canonicalization, or
        ///   <see cref="Status.Deleted"/> if the item is currently marked for deletion.
        /// </returns>
        private Status BookkeepAddition(T addition) {
            var addedStatus = deletions_.Contains(addition) ? Status.Saved : Status.New;
            if (addedStatus == Status.Saved) {
                deletions_.Remove(addition);
            }
            return addedStatus;
        }

        /// <summary>
        ///   Updates the internal set of deletions with the item at a particular index if the item at that index is
        ///   currently in the <see cref="Status.Saved"/> state. Otherwise, this method has no impact.
        /// </summary>
        /// <param name="removalIndex">
        ///   The index of the item in the <see cref="RelationList{T}"/> to be removed.
        /// </param>
        private void BookkeepRemoval(int removalIndex) {
            if (statuses_[removalIndex] == Status.Saved) {
                deletions_.Add(impl_[removalIndex]);
            }
        }

        // ************************************ MEMBER VARIABLES ************************************

        private readonly List<T> impl_;
        private readonly List<Status> statuses_;
        private readonly List<T> deletions_;
    }


    /// <summary>
    ///   An interface denoting a read-only view over a <see cref="RelationList{T}"/>.
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
    public interface IReadOnlyRelationList<out T> : IReadOnlyList<T>, IRelation where T : notnull {
        /// <inheritdoc/>
        static Type IRelation.ConnectionType => typeof(T);
    }
}
