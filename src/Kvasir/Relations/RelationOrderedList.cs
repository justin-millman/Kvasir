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
    ///   An ordered collection that tracks the state of its elements, including positional movements, for interaction
    ///   with a back-end database.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     A <see cref="RelationOrderedList{T}"/> implements the same interfaces and behave identically to a standard
    ///     <see cref="List{T}"/> collection. Operations that mutate the collection are used to track change that can
    ///     then be reflected in a back-end database, while view- or read-only operations have no additional side
    ///     effects. Converting a <see cref="RelationOrderedList{T}"/> into another collection type, such as through a
    ///     member API (e.g. <see cref="CopyTo(T[])"/> or LINQ, drops the change tracking capabilities.
    ///   </para>
    ///   <para>
    ///     Every item in a <see cref="RelationOrderedList{T}"/> is one of four states: <c>NEW</c>, <c>SAVED</c>,
    ///     <c>MODIFIED</c>, or <c>DELETED</c>. Each state corresponds to the action or actions that should be taken
    ///     with respect to that item to synchronize the back-end database table corresponding to the relation. Items in
    ///     a <see cref="RelationOrderedList{T}"/> are identified by their positional index, unlike in a
    ///     <see cref="RelationList{T}"/> where the items are self-identified. An item enters the <c>NEW</c> state when
    ///     it is first added (i.e. at a brand new position); when the collection is canonicalized, each <c>NEW</c> item
    ///     transitions to the <c>SAVED</c> state, indicating that it does not need to be written to the database on the
    ///     next write. When a <c>SAVED</c> item is changed, the item transitions to the <c>MODIFIED</c> state, which
    ///     goes through the same transitions on write. When a <c>SAVED</c> or <c>MODIFIED</c> item is removed from the
    ///     collection, it causes everything at larger indices to shift downward, forcing them all into the
    ///     <c>MODIFIED</c> state; items for indices that are vacated then transition to the <c>DELETED</c> state.
    ///     (<c>NEW</c> items never transition to <c>MODIFIED</c> or to <c>DELETED</c>.) Note that if a <c>SAVED</c>
    ///     item is changed and then changed back, it will revert to the <c>SAVED</c> state; the same goes if a
    ///     <c>SAVED</c> item is deleted and then re-added at the same index.
    ///   </para>
    ///   <para>
    ///     Items used in a <see cref="RelationOrderedList{T}"/> should be immutable: structs, <see cref="string"/>,
    ///     etc. This is because read access is <i>not</i> tracked: when using mutable elements it is possible for the
    ///     user to access an item (e.g. through <c>[]</c>) and mutate that element without the collection knowing,
    ///     preventing that change from being reflected in the back-end database. This also means that actions that
    ///     convert the collection into another form will <c>copy</c> the elements, ensuring that the tracking data
    ///     remains up-to-date.
    ///   </para>
    ///   <para>
    ///     A <see cref="IReadOnlyRelationOrderedList{T}"/> allows duplicate values, since items are uniquely identified
    ///     by their index, which cannot be occupied by more than one element.
    ///   </para>
    /// </remarks>
    /// <typeparam name="T">
    ///   The type of element to be stored in the collection.
    /// </typeparam>
    /// <seealso cref="RelationList{T}"/>
    /// <seealso cref="RelationSet{T}"/>
    /// <seealso cref="RelationMap{TKey, TValue}"/>
    public sealed class RelationOrderedList<T> : IList, IList<T>, IReadOnlyList<T>, IReadOnlyRelationOrderedList<T>,
        IRelation where T : notnull {

        // *************************************** PROPERTIES ***************************************

        /// <summary>
        ///   Gets or sets the total number of elements the internal data structure can hold without resizing.
        /// </summary>
        /// <value>
        ///   The number of elements that the <see cref="RelationOrderedList{T}"/> can contain before resizing is
        ///   required.
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
                impl_[index] = value;
            }
        }

        // ************************************** CONSTRUCTORS **************************************

        /// <summary>
        ///   Initializes a new instance of the <see cref="RelationOrderedList{T}"/> class that is empty and has the
        ///   default initial capacity.
        /// </summary>
        public RelationOrderedList() {
            impl_ = new List<T>();
            lastSaved_ = new List<T>();
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="RelationOrderedList{T}"/> class that contains elements copied
        ///   from the specified collection and has sufficient capacity to accommodate the number of elements copied.
        /// </summary>
        /// <param name="collection">
        ///   The element whose names are copied to the new list.
        /// </param>
        public RelationOrderedList(IEnumerable<T> collection) {
            impl_ = new List<T>(collection);
            lastSaved_ = new List<T>();
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="RelationOrderedList{T}"/> class that is empty and has the
        ///   specified initial capacity.
        /// </summary>
        /// <param name="capacity">
        ///   The number of elements that the new list can initially store.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   if <paramref name="capacity"/> is less than 0.
        /// </exception>
        public RelationOrderedList(int capacity) {
            impl_ = new List<T>(capacity);
            lastSaved_ = new List<T>();
        }

        // **************************************** METHODS *****************************************

        /// <inheritdoc/>
        public void Add(T item) {
            impl_.Add(item);
        }

        /// <summary>
        ///   Adds the elements of the specified collection to the end of the <see cref="RelationOrderedList{T}"/>.
        /// </summary>
        /// <param name="collection">
        ///   The collection whose elements should be added to the end of the <see cref="RelationOrderedList{T}"/>. The
        ///   collection itself cannot be <see langword="null"/>, but it can contain elements that are
        ///   <see langword="null"/>, if type <typeparamref name="T"/> is a reference type.
        /// </param>
        public void AddRange(IEnumerable<T> collection) {
            impl_.AddRange(collection);
        }

        /// <summary>
        ///   Returns a read-only <see cref="ReadOnlyCollection{T}"/> wrapper for the current collection.
        /// </summary>
        /// <returns>
        ///   An object that acts as a read-only wrapper around the current <see cref="RelationOrderedList{T}"/>.
        /// </returns>
        [ExcludeFromCodeCoverage]
        public ReadOnlyCollection<T> AsReadOnly() {
            return impl_.AsReadOnly();
        }

        /// <summary>
        ///   Searches the entire sorted <see cref="RelationOrderedList{T}"/> for an element using the default comparer
        ///   and returns the zero-based index of the element.
        /// </summary>
        /// <param name="item">
        ///   The object to locate. The value can be <see langword="null"/> for reference types.
        /// </param>
        /// <returns>
        ///   The zero-based index of <paramref name="item"/> in the sorted <see cref="RelationOrderedList{T}"/>, if
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
        ///   Searches the entire sorted <see cref="RelationOrderedList{T}"/> for an element using the specified
        ///   comparer and returns the zero-based index of the element.
        /// </summary>
        /// <param name="item">
        ///   The object to locate. The value can be <see langword="null"/> for reference types.
        /// </param>
        /// <param name="comparer">
        ///   The <see cref="IComparer{T}"/> implementation to use when comparing elements; or, <see langword="null"/>
        ///   to use the default comparer <see cref="Comparer{T}.Default"/>.
        /// </param>
        /// <returns>
        ///   The zero-based index of <paramref name="item"/> in the sorted <see cref="RelationOrderedList{T}"/>, if
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
        ///   Searches a range of elements in the sorted <see cref="RelationOrderedList{T}"/> for an element using the
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
        ///   The zero-based index of <paramref name="item"/> in the sorted <see cref="RelationOrderedList{T}"/>, if
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
        ///   <see cref="RelationOrderedList{T}"/>.
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
            impl_.Clear();
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public bool Contains(T item) {
            return impl_.Contains(item);
        }

        /// <summary>
        ///   Converts the elements in the current <see cref="RelationOrderedList{T}"/> to another type, and returns a
        ///   list containing the converted elements.
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
        ///   <see cref="RelationOrderedList{T}"/>.
        /// </returns>
        [ExcludeFromCodeCoverage]
        public List<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter) {
            return impl_.ConvertAll(converter);
        }

        /// <summary>
        ///   Copies the entire <see cref="RelationOrderedList{T}"/> to a compatible one-dimensional array, starting at
        ///   the beginning of the target array.
        /// </summary>
        /// <param name="array">
        ///   The one-dimensional <see cref="Array"/> that is the destination of the elements copied from the
        ///   <see cref="RelationOrderedList{T}"/>. The <see cref="Array"/> must have zero-based indexing.
        /// </param>
        /// <exception cref="ArgumentException">
        ///   if the number of elements in the source <see cref="RelationOrderedList{T}"/> is greater than the number of
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
        ///   Copies a range of elements from the <see cref="RelationOrderedList{T}"/> to a compatible one-dimensional
        ///   array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="index">
        ///   The zero-based index in the source <see cref="RelationOrderedList{T}"/> at which copying begins.
        /// </param>
        /// <param name="array">
        ///   The one-dimensional <see cref="Array"/> that is the destination of the elements copied from the
        ///   <see cref="RelationOrderedList{T}"/>. The <see cref="Array"/> must have zero-based indexing.
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
        ///   <see cref="RelationOrderedList{T}"/>
        ///     --or--
        ///   if the number of elements from <paramref name="index"/> to the end of the source
        ///   <see cref="RelationOrderedList{T}"/> is greater than the available space from
        ///   <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.
        /// </exception>
        [ExcludeFromCodeCoverage]
        public void CopyTo(int index, T[] array, int arrayIndex, int count) {
            impl_.CopyTo(index, array, arrayIndex, count);
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public sealed override bool Equals(object? obj) {
            return (obj is RelationOrderedList<T> orderedList) &&
                   impl_.Equals(orderedList);
        }

        /// <summary>
        ///   Determines whether the <see cref="RelationOrderedList{T}"/> contains elements that match the conditions
        ///   defined by the specified predicate.
        /// </summary>
        /// <param name="match">
        ///   The <see cref="Predicate{T}"/> delegate that defines the conditions of the elements to search for.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if the <see cref="RelationOrderedList{T}"/> contains one or more elements that
        ///   match the conditions defined by the specified predicate; otherwise, <see langword="false"/>.
        /// </returns>
        [ExcludeFromCodeCoverage]
        public bool Exists(Predicate<T> match) {
            return impl_.Exists(match);
        }

        /// <summary>
        ///   Searches for an element that matches the conditions defined by the specified predicate, and returns the
        ///   first occurrence within the entire <see cref="RelationOrderedList{T}"/>.
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
        ///   zero-based index of the first occurrence within the entire <see cref="RelationOrderedList{T}"/>.
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
        ///   <see cref="RelationOrderedList{T}"/> that extends from the specified index to the last element.
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
        ///    <see cref="RelationOrderedList{T}"/>.
        /// </exception>
        [ExcludeFromCodeCoverage]
        public int FindIndex(int startIndex, Predicate<T> match) {
            return impl_.FindIndex(startIndex, match);
        }

        /// <summary>
        ///   Searches for an element that matches the conditions defined by the specified predicate, and returns the
        ///   zero-based index of the first occurrence within the range of elements in the
        ///   <see cref="RelationOrderedList{T}"/> that starts at the specified index and contains the specified number
        ///   of elements.
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
        ///    <see cref="RelationOrderedList{T}"/>
        ///      --or--
        ///    if <paramref name="count"/> is less than <c>0</c>
        ///      --or--
        ///    if <paramref name="startIndex"/> and <paramref name="count"/> do not specify a valid section in the
        ///    <see cref="RelationOrderedList{T}"/>.
        /// </exception>
        [ExcludeFromCodeCoverage]
        public int FindIndex(int startIndex, int count, Predicate<T> match) {
            return impl_.FindIndex(startIndex, count, match);
        }

        /// <summary>
        ///   Searches for an element that matches the conditions defined by the specified predicate, and returns the
        ///   last occurrence within the entire <see cref="RelationOrderedList{T}"/>.
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
        ///   zero-based index of the last occurrence within the entire <see cref="RelationOrderedList{T}"/>.
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
        ///   zero-based index of the last occurrence within the range of elements in the
        ///   <see cref="RelationOrderedList{T}"/> that extends from the first element to the specified index.
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
        ///   <see cref="RelationOrderedList{T}"/>.
        /// </exception>
        [ExcludeFromCodeCoverage]
        public int FindLastIndex(int startIndex, Predicate<T> match) {
            return impl_.FindLastIndex(startIndex, match);
        }

        /// <summary>
        ///   Searches for an element that matches the conditions defined by the specified predicate, and returns the
        ///   zero-based index of the last occurrence within the range of elements in the
        ///   <see cref="RelationOrderedList{T}"/> that contains the specified number of elements and ends at the
        ///   specified index.
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
        ///   <see cref="RelationOrderedList{T}"/>
        ///     --or--
        ///   if <paramref name="count"/> is less than <c>0</c>
        ///     --or--
        ///   if <paramref name="startIndex"/> and <paramref name="count"/> do not specify a valid section in the
        ///   <see cref="RelationOrderedList{T}"/>.
        /// </exception>
        [ExcludeFromCodeCoverage]
        public int FindLastIndex(int startIndex, int count, Predicate<T> match) {
            return FindLastIndex(startIndex, count, match);
        }

        /// <summary>
        ///   Performs the specified action on each element of the <see cref="RelationOrderedList{T}"/>.
        /// </summary>
        /// <param name="action">
        ///   The <see cref="Action{T}"/> delegate to perform on each element of the
        ///   <see cref="RelationOrderedList{T}"/>.
        /// </param>
        /// <exception cref="InvalidOperationException">
        ///   if an element in the collection has been modified.
        /// </exception>
        [ExcludeFromCodeCoverage]
        public void ForEach(Action<T> action) {
            impl_.ForEach(action);
        }

        /// <summary>
        ///   Returns an enumerator that iterates through the <see cref="RelationOrderedList{T}"/>.
        /// </summary>
        /// <returns>
        ///   A <see cref="List{T}.Enumerator"/> for the <see cref="RelationOrderedList{T}"/>.
        /// </returns>
        [ExcludeFromCodeCoverage]
        public List<T>.Enumerator GetEnumerator() {
            return impl_.GetEnumerator();
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public sealed override int GetHashCode() {
            return impl_.GetHashCode();
        }

        /// <summary>
        ///   Creates a shallow copy of a range of elements in the source <see cref="RelationOrderedList{T}"/>.
        /// </summary>
        /// <param name="index">
        ///   The zero-based <see cref="RelationOrderedList{T}"/> index at which the range stats.
        /// </param>
        /// <param name="count">
        ///   The number of elements in the range.
        /// </param>
        /// <returns>
        ///   A shallow copy of a range of elements in the source <see cref="RelationOrderedList{T}"/>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   if <paramref name="index"/> is less than <c>0</c>
        ///     --or--
        ///   if <paramref name="count"/> is less than <c>0</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   if <paramref name="index"/> and <paramref name="count"/> do not denote a valid range of elements in the
        ///   <see cref="RelationOrderedList{T}"/>.
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
        ///   range of elements in the <see cref="RelationOrderedList{T}"/> that extends from the specified index to the
        ///   last element.
        /// </summary>
        /// <param name="item">
        ///   The object to locate in the <see cref="RelationOrderedList{T}"/>. The value can be <see langword="null"/>
        ///   for reference types.
        /// </param>
        /// <param name="index">
        ///   The zero-based starting index of the search. <c>0</c> (zero) is valid in an empty list.
        /// </param>
        /// <returns>
        ///   The zero-based index of the first occurrence of <paramref name="item"/> within the range of elements in
        ///   the <see cref="RelationOrderedList{T}"/> that extends from <paramref name="index"/> to the last element,
        ///   if found; otherwise, <c>-1</c>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="index"/> is outside the range of valid indexes for the
        ///   <see cref="RelationOrderedList{T}"/>.
        /// </exception>
        [ExcludeFromCodeCoverage]
        public int IndexOf(T item, int index) {
            return impl_.IndexOf(item, index);
        }

        /// <summary>
        ///   Searches for the specified object and returns the zero-based index of the first occurrence within the
        ///   range of elements in the <see cref="RelationOrderedList{T}"/> that starts at the specified index and
        ///   contains the specified number of elements.
        /// </summary>
        /// <param name="item">
        ///   The object to locate in the <see cref="RelationOrderedList{T}"/>. The value can be <see langword="null"/>
        ///   for reference types.
        /// </param>
        /// <param name="index">
        ///   The zero-based starting index of the search. <c>0</c> (zero) is valid in an empty list.
        /// </param>
        /// <param name="count">
        ///   The number of elements in the section to search.
        /// </param>
        /// <returns>
        ///   The zero-based index of the first occurrence of <paramref name="item"/> within the range of elements in
        ///   the <see cref="RelationOrderedList{T}"/> that starts at <paramref name="index"/> and contains
        ///   <paramref name="count"/> number of elements, if found; otherwise, <c>-1</c>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   if <paramref name="index"/> is outside the range of valid indexes of the
        ///   <see cref="RelationOrderedList{T}"/>
        ///     --or--
        ///   if <paramref name="count"/> is less than <c>0</c>
        ///     --or--
        ///   if <paramref name="index"/> and <paramref name="count"/> do not specify a valid section in the
        ///   <see cref="RelationOrderedList{T}"/>.
        /// </exception>
        [ExcludeFromCodeCoverage]
        public int IndexOf(T item, int index, int count) {
            return impl_.IndexOf(item, index, count);
        }

        /// <inheritdoc/>
        public void Insert(int index, T item) {
            impl_.Insert(index, item);
        }

        /// <summary>
        ///   Inserts the elements of a collection into the <see cref="RelationOrderedList{T}"/> at the specified index.
        /// </summary>
        /// <param name="index">
        ///   The zero-based index at which the new elements should be inserted.
        /// </param>
        /// <param name="collection">
        ///   The collection whose elements should be inserted into the <see cref="RelationOrderedList{T}"/>. The
        ///   collection itself cannot be <see langword="null"/>, but it can contain elements that are
        ///   <see langword="null"/> if type <typeparamref name="T"/> is a reference type.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   if <paramref name="index"/> is less than <c>0</c>
        ///     --or--
        ///   if <paramref name="index"/> is greater than <see cref="Count"/>.
        /// </exception>
        public void InsertRange(int index, IEnumerable<T> collection) {
            impl_.InsertRange(index, collection);
        }

        /// <summary>
        ///   Searches for the specified object and returns the zero-based index of the last occurrence within the
        ///   entire <see cref="RelationOrderedList{T}"/>.
        /// </summary>
        /// <param name="item">
        ///   The object to locate in the <see cref="RelationOrderedList{T}"/>. The value can be <see langword="null"/>
        ///   for reference types.
        /// </param>
        /// <returns>
        ///   The zero-based index of the last occurrence of <paramref name="item"/> within the entire
        ///   <see cref="RelationOrderedList{T}"/>, if found; otherwise, <c>-1</c>.
        /// </returns>
        [ExcludeFromCodeCoverage]
        public int LastIndexOf(T item) {
            return impl_.LastIndexOf(item);
        }

        /// <summary>
        ///   Searches for the specified object and returns the zero-based index of the last occurrence within the
        ///   range of elements in the <see cref="RelationOrderedList{T}"/> that extends from the first element to the
        ///   specified index.
        /// </summary>
        /// <param name="item">
        ///   The object to locate in the <see cref="RelationOrderedList{T}"/>. The value can be <see langword="null"/>
        ///   for reference types.
        /// </param>
        /// <param name="index">
        ///   The zero-based starting index of the backward search.
        /// </param>
        /// <returns>
        ///   The zero-based index of the last occurrence of <paramref name="item"/> within the range of elements in
        ///   the <see cref="RelationOrderedList{T}"/> that extends from the first element to <paramref name="index"/>,
        ///   if found; otherwise, <c>-1</c>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   if <paramref name="index"/> is outside the range of valid indexes for the
        ///   <see cref="RelationOrderedList{T}"/>.
        /// </exception>
        [ExcludeFromCodeCoverage]
        public int LastIndexOf(T item, int index) {
            return impl_.LastIndexOf(item, index);
        }

        /// <summary>
        ///   Searches for the specified object and returns the zero-based index of the last occurrence within the
        ///   range of elements in the <see cref="RelationOrderedList{T}"/> that contains the specified number of
        ///   elements and ends at the specified index.
        /// </summary>
        /// <param name="item">
        ///   The object to locate in the <see cref="RelationOrderedList{T}"/>. The value can be <see langword="null"/>
        ///   for reference types.
        /// </param>
        /// <param name="index">
        ///   The zero-based starting index of the backward search.
        /// </param>
        /// <param name="count">
        ///   The number of elements in the section to search.
        /// </param>
        /// <returns>
        ///   The zero-based index of the last occurrence of <paramref name="item"/> within the range of elements in
        ///   the <see cref="RelationOrderedList{T}"/> that contains <paramref name="count"/> number of elements and
        ///   ends at <paramref name="index"/>, if found; otherwise, <c>-1</c>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   if <paramref name="index"/> is outside the range of valid indexes for the
        ///   <see cref="RelationOrderedList{T}"/>
        ///     --or--
        ///   if <paramref name="count"/> is less than <c>0</c>
        ///     --or--
        ///   if <paramref name="index"/> and <paramref name="count"/> do not specify a valid section in the
        ///   <see cref="RelationOrderedList{T}"/>.
        /// </exception>
        [ExcludeFromCodeCoverage]
        public int LastIndexOf(T item, int index, int count) {
            return impl_.LastIndexOf(item, index, count);
        }

        /// <inheritdoc/>
        public bool Remove(T item) {
            return impl_.Remove(item);
        }

        /// <summary>
        ///   Removes all the elements that match the conditions defined by the specified predicate.
        /// </summary>
        /// <param name="match">
        ///   The <see cref="Predicate{T}"/> delegate that defines the conditions of the elements to remove.
        /// </param>
        /// <returns>
        ///   The number of elements removed from the <see cref="RelationOrderedList{T}"/>.
        /// </returns>
        public int RemoveAll(Predicate<T> match) {
            return impl_.RemoveAll(match);
        }

        /// <inheritdoc/>
        public void RemoveAt(int index) {
            impl_.RemoveAt(index);
        }

        /// <summary>
        ///   Removes a range of elements from the <see cref="RelationOrderedList{T}"/>.
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
        ///   <see cref="RelationOrderedList{T}"/>.
        /// </exception>
        public int RemoveRange(int index, int count) {
            impl_.RemoveRange(index, count);
            return count;
        }

        /// <summary>
        ///   Reverses the order of the elements in the entire <see cref="RelationOrderedList{T}"/>.
        /// </summary>
        public void Reverse() {
            impl_.Reverse();
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
        ///   <see cref="RelationOrderedList{T}"/>.
        /// </exception>
        public void Reverse(int index, int count) {
            impl_.Reverse(index, count);
        }

        /// <summary>
        ///   Sorts the elements in the entire <see cref="RelationOrderedList{T}"/> using the default comparer.
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
        ///   Sorts the elements in the entire <see cref="RelationOrderedList{T}"/> using the specified
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
            impl_.Sort(comparison);
        }

        /// <summary>
        ///   Sorts the elements in the entire <see cref="RelationOrderedList{T}"/> using the specified comparer.
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
        ///   Sorts the elements in a range of elements in the <see cref="RelationOrderedList{T}"/> using the specified
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
        ///   <see cref="RelationOrderedList{T}"/>
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
            impl_.Sort(index, count, comparer);
        }

        /// <summary>
        ///   Copies the elements of the <see cref="RelationOrderedList{T}"/> to a new array.
        /// </summary>
        /// <returns>
        ///   An array containing copies of the elements of the <see cref="RelationOrderedList{T}"/>.
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
                builder.AppendLine($"{impl_[idx]} [{StatusOf(idx)}]");
            }

            return builder.ToString();
        }

        /// <summary>
        ///   Sets the capacity to the actual number of elements in the <see cref="RelationOrderedList{T}"/>, if that
        ///   number is less than a threshold value.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public void TrimExcess() {
            impl_.TrimExcess();
            lastSaved_.TrimExcess();
        }

        /// <summary>
        ///   Determines whether every element in the <see cref="RelationOrderedList{T}"/> matches the conditions
        ///   defined by the specified predicate.
        /// </summary>
        /// <param name="match">
        ///   The <see cref="Predicate{T}"/> delegate that defines the conditions to check against the elements.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if every element in the <see cref="RelationOrderedList{T}"/> matches the conditions
        ///   defined by the specified predicate; otherwise, <see langword="false"/>. If the list contains no elements,
        ///   the return value is <see langword="true"/>.
        /// </returns>
        [ExcludeFromCodeCoverage]
        public bool TrueForAll(Predicate<T> match) {
            return impl_.TrueForAll(match);
        }

        // ******************************** EXPLICIT INTERFACE IMPLS ********************************

        /// <inheritdoc/>
        static Type IRelation.ConnectionType => typeof(KeyValuePair<uint, T>);

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
            lastSaved_.Clear();
            lastSaved_.AddRange(impl_);
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
            return (impl_ as IEnumerable).GetEnumerator();
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        IEnumerator<T> IEnumerable<T>.GetEnumerator() {
            return (impl_ as IEnumerable<T>).GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator<(object Item, Status Status)> IRelation.GetEnumerator() {
            for (int i = impl_.Count; i < lastSaved_.Count; ++i) {
                yield return (new KeyValuePair<uint, T>((uint)i, lastSaved_[i]), Status.Deleted);
            }
            for (int i = 0; i < impl_.Count; ++i) {
                yield return (new KeyValuePair<uint, T>((uint)i, impl_[i]), StatusOf(i));
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
            Debug.Assert(item.GetType() == typeof(KeyValuePair<uint, T>));
            Debug.Assert(lastSaved_.Count ==  impl_.Count);
            Debug.Assert(impl_.Select((_, idx) => idx).All(idx => StatusOf(idx) == Status.Saved));

            var element = (KeyValuePair<uint, T>)item;
            Debug.Assert(element.Key == impl_.Count);
            impl_.Add(element.Value);
            lastSaved_.Add(element.Value);
        }

        // ************************************ HELPER FUNCTIONS ************************************

        /// <summary>
        ///   Determine the <see cref="Status"/> of the element at a given index.
        /// </summary>
        /// <param name="index">
        ///   The target index.
        /// </param>
        /// <returns>
        ///   The <see cref="Status"/> of the element at position <paramref name="index"/>, which is guaranteed to be
        ///   one of <see cref="Status.New"><c>NEW</c></see>, <see cref="Status.Saved"><c>SAVED</c></see>, or
        ///   <see cref="Status.Modified"><c>MODIIFED</c></see>.
        /// </returns>
        Status StatusOf(int index) {
            Debug.Assert(index >= 0 && index < impl_.Count);

            if (index >= lastSaved_.Count) {
                return Status.New;
            }
            else if (Equals(impl_[index], lastSaved_[index])) {
                return Status.Saved;
            }
            else {
                return Status.Modified;
            }
        }

        // ************************************ MEMBER VARIABLES ************************************

        private readonly List<T> impl_;
        private readonly List<T> lastSaved_;
    }


    /// <summary>
    ///   An interface denoting a read-only view over a <see cref="RelationOrderedList{T}"/>.
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
    public interface IReadOnlyRelationOrderedList<out T> : IReadOnlyList<T>, IRelation where T : notnull {
        /// <inheritdoc/>
        static Type IRelation.ConnectionType => typeof(KeyValuePair<uint, T>);
    }
}
