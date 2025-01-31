using Cybele.Extensions;
using Kvasir.Extraction;
using Kvasir.Schema;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Kvasir.Reconstitution {
    /// <summary>
    ///   An <see cref="ICreator"/> that treats its collection of <see cref="DBValue">database values</see> as the
    ///   Primary Key of an Entity and looks that Entity up.
    /// </summary>
    internal sealed class KeyLookupCreator : ICreator {
        /// <inheritdoc/>
        public Type ResultType { get; }

        /// <summary>
        ///   Create a new <see cref="KeyLookupCreator"/>.
        /// </summary>
        /// <param name="lookup">
        ///   The <see cref="KeyMatcher"/> with which to look up Entities.
        /// </param>
        public KeyLookupCreator(KeyMatcher lookup) {
            Debug.Assert(lookup is not null);

            lookup_ = lookup;
            ResultType = lookup_.ResultType;
        }

        /// <inheritdoc/>
        public object? CreateFrom(IReadOnlyList<DBValue> dbValues) {
            Debug.Assert(dbValues is not null && !dbValues.IsEmpty());

            if (dbValues.Any(v => v == DBValue.NULL)) {
                return null;
            }
            return lookup_.Lookup(dbValues);
        }


        private readonly KeyMatcher lookup_;
    }

    /// <summary>
    ///   A simple component that enables efficient, cached lookup of Entities by their Primary Key.
    /// </summary>
    internal sealed class KeyMatcher {
        /// <summary>
        ///   The <see cref="Type"/> of Entity produced by look-ups on this <see cref="KeyMatcher"/>.
        /// </summary>
        public Type ResultType { get; }

        /// <summary>
        ///   Construct a new <see cref="KeyMatcher"/>.
        /// </summary>
        /// <param name="domainGenerator">
        ///   A generator function that produces the collection of possible Entities.
        /// </param>
        /// <param name="keyExtractor">
        ///   The <see cref="DataExtractionPlan"/> describing how to determine the Primary Key of a candidate Entity
        ///   produced by <paramref name="domainGenerator"/>.
        /// </param>
        public KeyMatcher(Func<IEnumerable<object>> domainGenerator, DataExtractionPlan keyExtractor) {
            Debug.Assert(domainGenerator is not null);
            Debug.Assert(keyExtractor is not null);

            keyExtractor_ = keyExtractor;
            domainGenerator_ = domainGenerator;
            keyCache_ = new(new ListEqualityComparer());
            cachedPossibleMatches_ = new HashSet<object>();
            ResultType = keyExtractor_.SourceType;
        }

        /// <summary>
        ///   Find the Entity whose Primary Key matches a collection of <see cref="DBValue">database values</see>.
        /// </summary>
        /// <param name="dbValues">
        ///   The target Primary Key.
        /// </param>
        /// <returns>
        ///   The Entity (of type <see cref="ResultType"/>) whose Primary Key is <paramref name="dbValues"/>.
        /// </returns>
        public object Lookup(IReadOnlyList<DBValue> dbValues) {
            Debug.Assert(dbValues is not null && !dbValues.IsEmpty());

            if (keyCache_.TryGetValue(dbValues, out object? match)) {
                return match;
            }

            foreach (var possibleMatch in domainGenerator_()) {
                if (!cachedPossibleMatches_.Contains(possibleMatch)) {
                    var possibleMatchKey = keyExtractor_.ExtractFrom(possibleMatch);
                    cachedPossibleMatches_.Add(possibleMatch);
                    keyCache_[possibleMatchKey] = possibleMatch;

                    if (dbValues.SequenceEqual(possibleMatchKey)) {
                        return possibleMatch;
                    }
                }
            }

            // This code should be unreachable - it means that the Foreign Key values do not match any known Entity, or
            // that the data for a Pre-Defined Entity do not match the hard-coded source values
            throw new UnreachableException($"No entity found to match key: ({string.Join(", ", dbValues)})");
        }


        /// <summary>
        ///   An <see cref="IEqualityComparer{T}"/> for lists of <see cref="DBValue">database values</see> that supports
        ///   ordered, member-wise equality.
        /// </summary>
        private readonly struct ListEqualityComparer : IEqualityComparer<IReadOnlyList<DBValue>> {
            public bool Equals(IReadOnlyList<DBValue>? lhs, IReadOnlyList<DBValue>? rhs) {
                Debug.Assert(lhs is not null && rhs is not null);
                return lhs.SequenceEqual(rhs);
            }

            public int GetHashCode(IReadOnlyList<DBValue> obj) {
                // Copied shamelessly from https://stackoverflow.com/questions/7278136/create-hash-value-on-a-list
                return obj.Aggregate(487, (current, item) => (current * 31) + item.GetHashCode());
            }
        }


        private readonly DataExtractionPlan keyExtractor_;
        private readonly Func<IEnumerable<object>> domainGenerator_;
        private readonly Dictionary<IReadOnlyList<DBValue>, object> keyCache_;
        private readonly HashSet<object> cachedPossibleMatches_;
    }
}
