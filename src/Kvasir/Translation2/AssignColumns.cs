using Combinatorics.Collections;
using Cybele.Collections;
using Cybele.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kvasir.Translation2 {
    internal sealed partial class Translator {
        /// <summary>
        ///   Assign columns to a collection of <see cref="FieldGroup">FieldGroups</see>, accounting for any
        ///   <c>[Column]</c> annotations that have already been processed.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the type encapsulating <paramref name="groups"/> is being translated.
        /// </param>
        /// <param name="groups">
        ///   The collection of <paramref name="groups"/>.  These groups will be <i>modified</i> in-place.
        /// </param>
        /// <exception cref="CannotAssignColumnsException">
        ///   if any of the <paramref name="groups"/> are required to overlap
        ///     --or--
        ///   if the <paramref name="groups"/> cannot be assigned consecutive, non-overlapping column indices without
        ///   leaving one or more gaps.
        /// </exception>
        private static void AssignColumns(Context context, IReadOnlyList<FieldGroup> groups) {
            var columns = new StickyList<int>();
            var gaps = new List<int>();
            var fillers = new List<FieldGroup>();

            // First, we populate the initial state. Any FieldGroups that have a column index assigned are inserted into
            // the StickyList. We use the group indices to track which groups are where, primarily for easy retrieval of
            // information for contextualized error messages when collisions are detected. FieldGroups that are not yet
            // assigned a column are placed into the "fillers" collection for later.
            void PopulateInitialState() {
                for (int groupIdx = 0; groupIdx < groups.Count; ++groupIdx) {
                    var group = groups[groupIdx];

                    if (!group.Column.HasValue) {
                        fillers.Add(group);
                    }
                    else {
                        var groupColumn = group.Column.Unwrap();
                        for (int fieldColumn = groupColumn; fieldColumn < groupColumn + group.Size; ++fieldColumn) {
                            if (fieldColumn <= columns.LargestIndex && columns.IsOccupied(fieldColumn)) {
                                var existingGroup = groups[columns[fieldColumn]];
                                var existingPath = existingGroup[fieldColumn - existingGroup.Column.Unwrap()];
                                var newPath = group[fieldColumn - group.Column.Unwrap()];

                                var msg = $"two Fields pinned to column index {fieldColumn} (\"{existingPath}\" and \"{newPath}\")";
                                throw new CannotAssignColumnsException(context, msg);
                            }

                            columns.Insert(fieldColumn, groupIdx);
                        }
                    }
                }
            }

            // Next, we identify all the gaps in assigned columns by length; the actual indices at which the gaps exist
            // are immaterial to the overall assignment algorithm. There may be no gaps at all, which is okay.
            void IdentifyGaps() {
                for (int idx = 0, length = 0; idx <= columns.LargestIndex; ++idx) {
                    if (!columns.IsOccupied(idx)) {
                        ++length;
                    }
                    else {
                        gaps.Add(length);
                        length = 0;
                    }
                }
            }

            // To determine if an assignment is valid, we have to see if the lengths of the groups exactly match the
            // length of the gaps in the gaps' order. We iterate over both collection concurrently, attempting to place
            // the next group in the permutation to the current gap. It may take multiple groups to fill a gap
            // completely, so we track how much of the gap has been filled in. If a group would overrun the gap length,
            // or if we run out of groups before the gap is filled, then the permutation fails. We may not use all of
            // the groups to fill the gaps, which just means that some groups will get tacked on to the end.
            bool IsSuccessfulPermutation(IReadOnlyList<int> permutation) {
                int nextIdx = 0;
                foreach (var gapLength in gaps) {
                    int remaining = gapLength;
                    while (remaining > 0 && nextIdx < permutation.Count) {
                        if (fillers[permutation[nextIdx]].Size > remaining) {
                            return false;
                        }
                        else {
                            remaining -= fillers[permutation[nextIdx]].Size;
                            ++nextIdx;
                        }
                    }
                    if (remaining != 0) {
                        return false;
                    }
                }
                return true;
            }

            // Once we have a successful permutation, we have to assign the column indices to the "filler" groups. We
            // don't really need to use the StickyList anymore, since we know the order of the groups and we know where
            // the gaps are.
            void SetGroupColumns(IReadOnlyList<int> permutation) {
                var numFields = groups.Sum(g => g.Size);
                foreach (var groupIdx in permutation) {
                    var column = Enumerable.Range(0, numFields).First(idx => idx > columns.LargestIndex || !columns.IsOccupied(idx));
                    fillers[groupIdx].SetColumn(context, column);
                    columns.Insert(column, groupIdx);
                }
            }


            // We have to use indices for the permutations rather than the sequences themselves because the sequences
            // are not totally orderable, and it is far more complicated to make them orderable than it is to simply use
            // indices as a proxy
            PopulateInitialState();
            IdentifyGaps();
            foreach (var permutation in new Permutations<int>(Enumerable.Range(0, fillers.Count))) {
                if (IsSuccessfulPermutation(permutation)) {
                    SetGroupColumns(permutation);
                    return;
                }
            }
            throw new CannotAssignColumnsException(context, "unable to assign Fields to columns without introducing gaps");
        }
    }
}
