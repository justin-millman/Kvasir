using Combinatorics.Collections;
using Cybele.Collections;
using Cybele.Extensions;
using Optional;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Sequence = System.Collections.Generic.IReadOnlyList<Kvasir.Translation.FieldDescriptor>;

namespace Kvasir.Translation {
    internal sealed partial class Translator {
        private static Option<StickyList<FieldDescriptor>, string> SolveColumns(IReadOnlyList<Sequence> fields) {
            var columns = new StickyList<FieldDescriptor>();
            var gaps = new List<int>();
            var fillers = new List<Sequence>();

            // First, we populate the initial state. Any sequences of Fields that have absolute column indices already
            // assigned are placed into the StickyList; collisions are detected at this stage. Any other sequences are
            // placed into the fillers list for later.
            Option<string> PopulateInitialState() {
                foreach (var sequence in fields) {
                    if (!sequence[0].AbsoluteColumn.HasValue) {
                        Debug.Assert(sequence.None(fd => fd.AbsoluteColumn.HasValue));
                        fillers.Add(sequence);
                    }
                    else {
                        foreach (var field in sequence) {
                            var index = field.AbsoluteColumn.Unwrap();
                            Debug.Assert(index >= 0);

                            if (index <= columns.LargestIndex && columns.IsOccupied(index) && columns.IsSticky(index)) {
                                var paths = $"\"{field.AccessPath}\" and \"{columns[index].AccessPath}\"";
                                return Option.Some($"two Fields pinned to column index {index} (paths {paths})");
                            }
                            columns.Insert(index, field);
                        }
                    }
                }
                return Option.None<string>();
            }

            // Second, we identify all the gaps by length; the actually indices are immaterial to the overall algorithm.
            // There may be no gaps at all, which is okay.
            void IdentifyGaps() {
                for (int idx = 0, length = 0; idx <= columns.LargestIndex; ++idx) {
                    if (!columns.IsOccupied(idx)) {
                        ++length;
                    }
                    else if (length > 0) {
                        gaps.Add(length);
                        length = 0;
                    }
                }
            }

            // To determine if a permutation is successful, we have to see if the lengths of the sequences directly
            // coincide with the lengths of the gaps in the gaps' order. We iterate over both lists concurrently,
            // deducting from the gap with the length of the next sequence; if doing so produces a negative value, that
            // indicates an overlap with a sticky element, which is a failure. Likewise, if we run out of sequences and
            // there are still gaps, the permutation fails. A permutation is a success if all the gaps are filled, even
            // if there are still sequences left.
            bool IsSuccessfulPermutation(IReadOnlyList<int> idxPermutation) {
                int nextOpenIdx = 0;
                foreach (var length in gaps) {
                    int remaining = length;
                    while (remaining > 0 && nextOpenIdx < idxPermutation.Count) {
                        if (fillers[idxPermutation[nextOpenIdx]].Count > remaining) {
                            return false;
                        }
                        else {
                            remaining -= fillers[idxPermutation[nextOpenIdx]].Count;
                            ++nextOpenIdx;
                        }
                    }
                    if (remaining != 0) {
                        return false;
                    }
                }
                return true;
            }

            // Once we've identified a successful permutation, we can just add the Fields one by one. The algorithm
            // above guarantees that the Fields within a group will not be separated from one another, and the
            // StickyList ensures that columns are occupied from lowest index to highest.
            void FillColumnsWith(IReadOnlyList<int> idxPermutation) {
                foreach (var index in idxPermutation) {
                    foreach (var field in fillers[index]) {
                        columns.Add(field);
                    }
                }
            }

            // We have to use indices for the permutations rather than the sequences themselves because the sequences
            // are not totally orderable, and it is far more complicated to make them orderable than it is to simply use
            // indices as a proxy

            var duplicatesError = PopulateInitialState();
            if (duplicatesError.HasValue) {
                return Option.None<StickyList<FieldDescriptor>, string>(duplicatesError.Unwrap());
            }

            IdentifyGaps();
            foreach (var idxPermutation in new Permutations<int>(Enumerable.Range(0, fillers.Count))) {
                if (IsSuccessfulPermutation(idxPermutation)) {
                    FillColumnsWith(idxPermutation);
                    return Option.Some<StickyList<FieldDescriptor>, string>(columns);
                }
            }

            var msg = "unable to assign Fields to columns without introducing gaps";
            return Option.None<StickyList<FieldDescriptor>, string>(msg);
        }
    }
}