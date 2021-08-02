using Ardalis.GuardClauses;
using Cybele.Extensions;
using Optional;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Cybele.Collections {
    /// <summary>
    ///   An indexable collection in which certain elements are "stuck" in specific slots while others may move from
    ///   slot to slot.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     Elements in a <see cref="StickyList{T}"/> come in two flavors: sticky and non-sticky. A sticky element is
    ///     inserted with a specific index and will not move from that slot. A non-sticky element is added to the
    ///     collection in the first available slot, being appended on the end if there are no gaps; that element may
    ///     move to a new slot if a gap arises later. Because of this, the order of elements within the collection is
    ///     volatile and should not be relied on; only the positions of sticky elements is guaranteed.
    ///   </para>
    ///   <para>
    ///     The nature of sticky elements means that performing operations on a <see cref="StickyList{T}"/> may lead to
    ///     "gaps." For example, inserting a sticky element at index <c>3</c> into an empty list will produce a gap in
    ///     slots <c>0</c>, <c>1</c>, and <c>2</c>. Attempting to access an element at this index for any reason will
    ///     result in an exception, much like attempting to read beyond the end of the list. However, the
    ///     <see cref="StickyList{T}"/> will do what it can to minimize gaps by rearranging non-sticky elements when
    ///     gaps arise; if the gaps cannot be fully eliminated, they will be pushed as far to the back of the list as
    ///     possible. On iteration, gaps are ignored.
    ///   </para>
    /// </remarks>
    /// <typeparam name="T">
    ///   The type of element to be stored in the StickyList.
    /// </typeparam>
    public sealed class StickyList<T> : IReadOnlyList<T>, IList<T> {
        /// <summary>
        ///   Gets or sets the element at the specified index in the <see cref="StickyList{T}"/>.
        /// </summary>
        /// <param name="index">
        ///   [GET] The <c>0</c>-based index of the element to return.
        ///   [SET] The <c>0</c>-based index at which to insert the new element.
        /// </param>
        /// <returns>
        ///   [GET] The element at index <paramref name="index"/>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   [GET] if <paramref name="index"/> is negative or if there is no element at the specified position,
        ///     including if it is larger than <see cref="LargestIndex"/>.
        ///   [SET] if <paramref name="index"/> is negative
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///   [SET] if a sticky element is already in the StickyList at position <paramref name="index"/>.
        /// </exception>
        public T this[int index] {
            get {
                return elements_[index].ValueOr(
                    () => throw new ArgumentOutOfRangeException(nameof(index), "Requested index is a gap")
                ).item;
            }
            set {
                Insert(index, value);
            }
        }

        /// <summary>
        ///   The number of elements contained in the <see cref="StickyList{T}"/>. This count ignores gaps, and
        ///   therefore may be less than <see cref="LargestIndex"/>.
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        ///   The largest <c>0</c>-based index that is <see cref="IsOccupied(int)">occupied</see> by an element in the
        ///   <see cref="StickyList{T}"/>. If the list is empty, this property returns <c>-1</c>.
        /// </summary>
        public int LargestIndex => elements_.Count - 1;

        /// <summary>
        ///   Whether or not there are any gaps in the <see cref="StickyList{T}"/>.
        /// </summary>
        public bool HasGaps => (Count != elements_.Count);

        /// <inheritdoc/>
        bool ICollection<T>.IsReadOnly => false;

        /// <summary>
        ///   Constructs a new, empty <see cref="StickyList{T}"/> that uses the default <see cref="IEqualityComparer"/>
        ///   for <typeparamref name="T"/> for comparison purposes.
        /// </summary>
        public StickyList()
            : this(Enumerable.Empty<T>()) {}

        /// <summary>
        ///   Constructs a new <see cref="StickyList{T}"/> that uses the default <see cref="IEqualityComparer"/> for
        ///   <typeparamref name="T"/> for comparison purposes and is populated with the non-sticky contents of another
        ///   enumerable.
        /// </summary>
        /// <param name="enumerable">
        ///   The enumerable from which to populate the initial state of the new StickyList. All elemenets will be
        ///   initialized as non-sticky.
        /// </param>
        public StickyList(IEnumerable<T> enumerable)
            : this(enumerable, EqualityComparer<T>.Default) {}

        /// <summary>
        ///   Constructs a new, empty <see cref="StickyList{T}"/> that uses a custom <see cref="IEqualityComparer"/>
        ///   for comparison purposes.
        /// </summary>
        /// <param name="comparer">
        ///   The <see cref="IEqualityComparer{T}"/> that the new StickyList should use for comparisons.
        /// </param>
        public StickyList(IEqualityComparer<T> comparer)
            : this(Enumerable.Empty<T>(), comparer) {}

        /// <summary>
        ///   Constructs a new <see cref="StickyList{T}"/> that uses a custom <see cref="IEqualityComparer{T}"/> for
        ///   comparison purposes and is populated with the non-sticky contents of another enumerable.
        /// </summary>
        /// <param name="enumerable">
        ///   The enumerable from which to populate the initial state of the new StickyList. All elemenets will be
        ///   initialized as non-sticky.
        /// </param>
        /// <param name="comparer">
        ///   The <see cref="IEqualityComparer{T}"/> that the new StickyList should use for comparisons.
        /// </param>
        public StickyList(IEnumerable<T> enumerable, IEqualityComparer<T> comparer) {
            Guard.Against.Null(enumerable, nameof(enumerable));
            Guard.Against.Null(comparer, nameof(comparer));

            elements_ = enumerable.Select(i => Option.Some((i, false))).ToList();
            comparer_ = comparer;

            Count = elements_.Count;
            EnsureInvariant();
        }

        /// <summary>
        ///   Checks if a particular index in the <see cref="StickyList{T}"/> is occupied by an element.
        /// </summary>
        /// <param name="index">
        ///   The <c>0</c>-based index at which to check.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if the StickyList contains an element at position <paramref name="index"/>;
        ///   otherwise, <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   if <paramref name="index"/> is negative or not less than <see cref="LargestIndex"/>.
        /// </exception>
        public bool IsOccupied(int index) {
            return elements_[index].HasValue;
        }

        /// <summary>
        ///   Checks if an element at a particular index in the <see cref="StickyList{T}"/> is "sticky."
        /// </summary>
        /// <param name="index">
        ///   The <c>0</c>-based index at which to check.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if the element at position <paramref name="index"/> is "sticky"; otherwise,
        ///   <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   if <paramref name="index"/> is negative or if there is no element at the specified position, including if
        ///   it is larger than <see cref="LargestIndex"/>.
        /// </exception>
        public bool IsSticky(int index) {
            return elements_[index].Match(some: elt => elt.isSticky, none: () => false);
        }

        /// <summary>
        ///   Checks if an elemment exists in the <see cref="StickyList{T}"/>.
        /// </summary>
        /// <param name="element">
        ///   The probe element.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if the StickyList contains an element that compares equal to
        ///   <paramref name="element"/>; otherwise, <see langword="false"/>.
        /// </returns>
        public bool Contains(T element) {
            return this.Any(entry => comparer_.Equals(entry, element));
        }

        /// <summary>
        ///   Determines the <c>0</c>-based index of an element in the <see cref="StickyList{T}"/>.
        /// </summary>
        /// <param name="element">
        ///   The probe element.
        /// </param>
        /// <returns>
        ///   The <c>0</c>-based index of the first element in the StickList that compares equal to
        ///   <paramref name="element"/>, if suh an element exists; otherwise, <c>-1</c>.
        /// </returns>
        public int IndexOf(T element) {
            for (var idx = 0; idx < elements_.Count; ++idx) {
                if (elements_[idx].HasValue && comparer_.Equals(elements_[idx].Unwrap().item, element)) {
                    return idx;
                }
            }
            return -1;
        }

        /// <summary>
        ///   Adds a new non-sticky element into the <see cref="StickyList{T}"/>. If there are no gaps, the element
        ///   will be appended onto the end of the collection. If there are any gaps, the element will be placed into
        ///   the unoccupied slot with the smallest index.
        /// </summary>
        /// <param name="element">
        ///   The new element to add.
        /// </param>
        public void Add(T element) {
            ++Count;
            var entry = Option.Some((element, false));

            for (int i = 0; i < elements_.Count; ++i) {
                if (!elements_[i].HasValue) {
                    elements_[i] = entry;
                    return;
                }
            }
            elements_.Add(entry);

            EnsureInvariant();
        }

        /// <summary>
        ///   Adds a new sticky element into the <see cref="StickyList{T}"/> at a specified position. If that position
        ///   is occupied by a non-sticky element, it will be displaced and <see cref="Add(T)">added back</see>. If
        ///   that position is beyond the current endpoint of the collection, a new gap will be created.
        /// </summary>
        /// <param name="index">
        ///   The <c>0</c>-based index at which to add the new element.
        /// </param>
        /// <param name="element">
        ///   The new element to add.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   if <paramref name="index"/> is negative.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///   if a sticky element already occupies position <paramref name="index"/>.
        /// </exception>
        public void Insert(int index, T element) {
            var entry = Option.Some((element, true));

            // Scenario 1: The target index is beyond the current end of the collection
            if (index >= elements_.Count) {
                var difference = index - elements_.Count + 1;
                elements_.AddRange(Enumerable.Repeat(Option.None<(T, bool)>(), difference));
                elements_[index] = entry;
                ++Count;
                EnsureInvariant();
                return;
            }

            // Scenario 2: The target index is empty
            if (!elements_[index].HasValue) {
                elements_[index] = entry;
                ++Count;
                EnsureInvariant();
                return;
            }

            // Scenario 3: The target index is occupied by a non-sticky entry
            if (elements_[index].Exists(entry => !entry.isSticky)) {
                Add(elements_[index].Unwrap().item);
                elements_[index] = entry;
                EnsureInvariant();
                return;
            }

            // Scenario 4: The target index is occupied by a sticky entry
            throw new InvalidOperationException($"Cannot insert new entry into StickList at index {index}: a sticky " +
                "entry already occupies that position");
        }

        /// <summary>
        ///   Removes the first element in the <see cref="StickyList{T}"/> that compares equal to a probe. If the
        ///   removal creates a gap, non-sticky elements will be rearranged so as to push the gap toward the end.
        ///   Elements will not be rearranged otherwise.
        /// </summary>
        /// <param name="element">
        ///   The probe element.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if an element was removed; otherwise, <see langword="false"/>.
        /// </returns>
        public bool Remove(T element) {
            var idx = IndexOf(element);
            if (idx == -1) {
                return false;
            }

            RemoveAt(idx);
            EnsureInvariant();
            return true;
        }

        /// <summary>
        ///   Removes the element at a specified index from the <see cref="StickyList{T}"/>. If the removal creates a
        ///   gap, non-sticky elements will be rearranged so as to push the gap toward the end. Elements will not be
        ///   rearranged otherwise.
        /// </summary>
        /// <param name="index">
        ///   The <c>0</c>-based index of the element to remove.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   if <paramref name="index"/> is negative or if there is no element at the specified position, including if
        ///   it is larger than <see cref="LargestIndex"/>.
        /// </exception>
        public void RemoveAt(int index) {
            if (!elements_[index].HasValue) {
                throw new ArgumentOutOfRangeException(nameof(index), "Requested removal index is a gap");
            }

            elements_[index] = Option.None<(T, bool)>();
            for (int i = elements_.Count - 1; i > index; --i) {
                if (elements_[i].HasValue) {
                    var (item, isSticky) = elements_[i].Unwrap();
                    if (!isSticky) {
                        elements_[index] = Option.Some((item, false));
                        elements_[i] = Option.None<(T, bool)>();
                        break;
                    }
                }
            }

            while (!elements_.IsEmpty() && !elements_[^1].HasValue) {
                elements_.RemoveAt(elements_.Count - 1);
            }

            --Count;
            EnsureInvariant();
        }

        /// <summary>
        ///   Removes all elements from the <see cref="StickyList{T}"/>.
        /// </summary>
        public void Clear() {
            elements_.Clear();
            Count = 0;
            EnsureInvariant();
        }

        /// <summary>
        ///   Produces an enumerator that iterates over the elements of the <see cref="StickyList{T}"/>, skipping any
        ///   gaps.
        /// </summary>
        /// <remarks>
        ///   Because the StickyList's enumerator skips gaps, the sequence of items (and, by extension, the sequence
        ///   produced by using LINQ to convert a StickyList to an Array or standard List) may not line up with indices
        ///   as reported by other aspects of the API.
        /// </remarks>
        /// <returns>
        ///   An enumerator over the elements of the StickyList in order, with all gaps skipped.
        /// </returns>
        public IEnumerator<T> GetEnumerator() {
            foreach (var entry in elements_) {
                if (entry.HasValue) {
                    yield return entry.Unwrap().item;
                }
            }
        }

        /// <summary>
        ///   Checks that the <see cref="StickyList{T}"/> invariant is maintained. Specifically, there should be no
        ///   non-sticky elements present after the first gap; and, the Count should always reflect the number of
        ///   non-empty entries in the collection.
        /// </summary>
        /// <remarks>
        ///   Violation of a class invariant is always due to programmer error. User input should be sanitized and
        ///   checked to ensure that its use does not violate invariants, with appropriate (documented) exceptions
        ///   raised if necessary.
        /// </remarks>
        /// <exception cref="ApplicationException">
        ///   if the current state of the StickList violates the class's invariant.
        /// </exception>
        [Conditional("DEBUG"), ExcludeFromCodeCoverage]
        private void EnsureInvariant() {
            bool encounteredGap = false;
            int expectedCount = 0;

            for (int i = 0; i < elements_.Count; ++i) {
                if (!elements_[i].HasValue) {
                    encounteredGap = true;
                    continue;
                }

                ++expectedCount;
                if (encounteredGap && !elements_[i].Unwrap().isSticky) {
                    throw new ApplicationException($"Item at index {i} is non-sticky, but appears after a gap");
                }
            }

            if (!elements_.IsEmpty() && !elements_[^1].HasValue) {
                throw new ApplicationException($"There cannot be a gap at the end of the StickyList");
            }
            if (expectedCount != Count) {
                throw new ApplicationException($"Actual count of {Count} does not match expected {expectedCount}");
            }
            if (encounteredGap != HasGaps) {
                throw new ApplicationException($"HasGaps property is reporting incorrect result of {HasGaps}");
            }
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        /// <inheritdoc/>
        void ICollection<T>.CopyTo(T[] array, int arrayIndex) {
            foreach (var item in this) {
                array[arrayIndex++] = item;
            }
        }


        private readonly List<Option<(T item, bool isSticky)>> elements_;
        private readonly IEqualityComparer<T> comparer_;
    }
}
