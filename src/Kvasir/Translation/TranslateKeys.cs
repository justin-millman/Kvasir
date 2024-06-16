using Cybele.Extensions;
using Kvasir.Annotations;
using Kvasir.Schema;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Kvasir.Translation {
    internal sealed partial class Translator {
        private static class KeyTranslator {
            /// <summary>
            ///   Determine the <see cref="PrimaryKey"/> and all <see cref="CandidateKey">Candidate Keys</see> of the
            ///   Principal Table for an Entity Type.
            /// </summary>
            /// <param name="context">
            ///   The <see cref="Context"/> in which the key determinations are being made.
            /// </param>
            /// <param name="source">
            ///   The Entity Type.
            /// </param>
            /// <param name="schemas">
            ///   The Field schemas for the Principal Table of <paramref name="source"/>.
            /// </param>
            /// <returns>
            ///   An named tuple whose first element (<c>PrimaryKey</c>) is the Primary Key for the Principal Table of
            ///   <paramref name="source"/> and whose second element (<c>CandidateKeys</c>) is an enumerable of all the
            ///   table's Candidate Keys.
            /// </returns>
            /// <exception cref="ConflictingKeyNameException">
            ///   if the <see cref="PrimaryKey"/> is supposed to be named, but the name is the same as a non-anonymous
            ///   Candidate Key that was not deduced as the Primary Key.
            /// </exception>
            public static (PrimaryKey PrimaryKey, IEnumerable<CandidateKey> CandidateKeys)
            ComputeKeys(Context context, Type source, IEnumerable<FieldSchema> schemas) {
                Debug.Assert(context is not null);
                Debug.Assert(source is not null);
                Debug.Assert(schemas is not null && schemas.Count() >= 2);

                // This is the logic for creating the set of Candidate Keys for the Primary Table. There is a bunch of
                // (potentially unintuitive) LINQ here, so this is the full breakdown:
                //
                //   1. For each schema, create a list where each element consists of a pair (KeyName, IField), where each
                //        KeyName is the name of one of the Candidate Keys to which that Field belongs. Then, flatten the
                //        list-of-list-of-pairs into a single list-of-pairs.
                //   2. Group the pairs in the list by KeyName, creating a list-of-groups indexed by the shared KeyName.
                //   3. Create a Candidate Key out of each group, using the index KeyName as the key's name and the elements
                //        in the group as the constituent Fields.
                //   4. Append on an anonymous key consisting of all the available Fields. This is the "implicit" Candidate
                //        Key for the Table; it will only ever be used if there are no other Candidate Keys and no better
                //        Primary Key deduction, in which case it will be the Primary Key.
                var candidateKeys = schemas
                    .SelectMany(schema => schema.Descriptor.CandidateKeyMemberships.Select(name => (name, schema.Field)))
                    .GroupBy(membership => membership.name)
                    .Select(group => MakeCandidateKey(group.Key, group.Select(element => element.Field)))
                    .Append(new CandidateKey(schemas.Select(schema => schema.Field)))
                    .ToList();
                candidateKeys = candidateKeys.Where(key => candidateKeys.None(other => IsPreferable(other, key))).ToList();

                var primaryKey = DeducePrimaryKey(context, source, schemas, candidateKeys);
                candidateKeys = candidateKeys.Where(ck => !ck.Fields.ToHashSet().IsSupersetOf(primaryKey.Fields)).ToList();

                if (primaryKey.Name.Exists(name => candidateKeys.Any(ck => ck.Name.Contains(name)))) {
                    var conflict = candidateKeys.First(ck => ck.Name.Contains(primaryKey.Name.Unwrap()))!;
                    throw new ConflictingKeyNameException(context, conflict);
                }
                return (primaryKey, candidateKeys);
            }

            /// <summary>
            ///   Determines if one <see cref="CandidateKey"/> is preferable to another, meaning that it should be kept
            ///   in the final schema while the other should be discarded.
            /// </summary>
            /// <remarks>
            ///   A <see cref="CandidateKey"/> is preferable to another if its Fields are a strict subset of the other's
            ///   Fields, regardless of order. If the two keys are partially disjoint, then neither is preferable to the
            ///   other. When two keys contain identical Fields, the "preferable" key is undefined, except that only one
            ///   is preferable and the preference is consistent.
            /// </remarks>
            /// <param name="lhs">
            ///   The left-hand operand.
            /// </param>
            /// <param name="rhs">
            ///   The right-hand operand.
            /// </param>
            /// <returns>
            ///   <see langword="true"/> if <paramref name="lhs"/> is preferable to <paramref name="rhs"/>; otherwise,
            ///   <see langword="false"/>.
            /// </returns>
            private static bool IsPreferable(CandidateKey lhs, CandidateKey rhs) {
                var otherFields = lhs.Fields.ToHashSet();
                var currentFields = rhs.Fields.ToHashSet();

                if (ReferenceEquals(lhs, rhs)) {
                    // If the "other" is the exact same as the "current" then it cannot be preferable to itself
                    return false;
                }
                else if (otherFields.IsProperSubsetOf(currentFields)) {
                    // The "other" is a proper subset of the "current" which makes the latter redundant
                    return true;
                }
                else if (!otherFields.SetEquals(currentFields)) {
                    // There is no overlap between the two, so the "current" can stay
                    return false;
                }
                else if (!lhs.Name.HasValue && !rhs.Name.HasValue) {
                    // The two are equivalent and both anonymous; it doesn't matter which one we pick, as long as we're
                    // consistent accounting for symmetry
                    return lhs.GetHashCode() <= rhs.GetHashCode();
                }
                else if (!lhs.Name.HasValue || !rhs.Name.HasValue) {
                    // The two are equivalent, but one of them is anonymous while the other is not; we prefer to keep
                    // the one that is named
                    return lhs.Name.HasValue;
                }
                else {
                    // The two are equivalent and neither is anonymous; it doesn't matter which one we pick, as long as
                    // we're consistent accounting for symmetry, so we'll pick the one with the name that is
                    // lexicographically first
                    return lhs.Name.Unwrap() < rhs.Name.Unwrap();
                }
            }

            /// <summary>
            ///   Creates a named or unnamed <see cref="CandidateKey"/>.
            /// </summary>
            /// <param name="name">
            ///   The name of the new Candidate Key. If this value begins with the reserved anonymous prefix, the
            ///   ensuing Candidate Key will be unnamed.
            /// </param>
            /// <param name="fields">
            ///   The Fields that comprise the new Candidate Key.
            /// </param>
            /// <returns>
            ///   A new <see cref="CandidateKey"/> consisting of <paramref name="fields"/>.
            /// </returns>
            private static CandidateKey MakeCandidateKey(KeyName name, IEnumerable<IField> fields) {
                if (name.ToString().StartsWith(UniqueAttribute.ANONYMOUS_PREFIX)) {
                    return new CandidateKey(fields);
                }
                else {
                    return new CandidateKey(name, fields);
                }
            }

            /// <summary>
            ///   Deduces the Primary Key for a Principal Table.
            /// </summary>
            /// <param name="context">
            ///   The <see cref="Context"/> in which the Primary Key is being deduced.
            /// </param>
            /// <param name="source">
            ///   The Entity Type for whose Principal Table the Primary Key is being deduced.
            /// </param>
            /// <param name="schemas">
            ///   The set of Field schemas that comprise the Principal Table for <paramref name="source"/>.
            /// </param>
            /// <param name="candidateKeys">
            ///   The <see cref="CandidateKey">Candidate Keys</see> for the Principal Table of
            ///   <paramref name="source"/>.
            /// </param>
            /// <returns>
            ///   The <see cref="PrimaryKey"/> deduced for the Principal Table of <paramref name="source"/> based on
            ///   <paramref name="schemas"/> and <paramref name="candidateKeys"/>.
            /// </returns>
            /// <exception cref="CannotDeducePrimaryKeyException">
            ///   if no Primary Key can be deduced.
            /// </exception>
            /// <exception cref="InvalidNameException">
            ///   if <paramref name="source"/> is annotated with a
            ///   <see cref="NamedPrimaryKeyAttribute"><c>[NamedPrimaryKey]</c></see> annotation that carries an invalid
            ///   key name.
            /// </exception>
            private static PrimaryKey
            DeducePrimaryKey(Context context, Type source, IEnumerable<FieldSchema> schemas, IEnumerable<CandidateKey> candidateKeys) {
                // A helper function that "scores" a Field based on its deduction to being in the Principal Table's
                // Primary Key. The actual value that is returned is irrelevant, other than the fact that a larger value
                // is the better selection. Two Fields may have the same score, in which case they are both (or all) in
                // the Primary Key.
                int PrimaryKeyScore(IField field, bool annotated) {
                    if (annotated) {
                        // Best Match: a Field that is annotated [PrimaryKey]
                        return 100;
                    }
                    else if (field.Nullability == IsNullable.Yes) {
                        // Worst Match: a nullable Field
                        return 0;
                    }
                    else if (field.Name.ToString() == "ID") {
                        // Second-Best Match: a non-nullable Field named "ID"
                        return 75;
                    }
                    else if (field.Name.ToString() == $"{source.Name}ID") {
                        // Third-Best Match: a non-nullable Field named "<Entity>ID"
                        return 50;
                    }
                    else {
                        // Worst Match: a non-nullable Field with any other name
                        return 0;
                    }
                }

                var annotation = source.GetCustomAttribute<NamedPrimaryKeyAttribute>();
                if (annotation is not null && (annotation.Name is null || annotation.Name == "")) {
                    throw new InvalidNameException(context, annotation);
                }
                var pkName = annotation is null ? null : new KeyName(annotation.Name);


                // (1) the set of Fields explicitly annotated as being in the [PrimaryKey]
                // (2) the single non-nullable Field named "ID"
                // (3) the single non-nullable Field named "<Entity>ID"
                // (4) the single Candidate Key consisting of only non-nullable Fields
                // (5) the single non-nullable Field
                // (6) deduction fails


                // This LINQ expression grabs the set of Fields that belong to the Principal Table's Primary Key,
                // assuming that a deduction can be made without falling back to either (4) or (5). Here's a full
                // breakdown of the individual sub-expressions:
                //
                //   1. For each schema, create a pair (Score, Field)
                //   2. Group the pairs in the list by score, creating a list-of-pairs index by score
                //   3. Eliminate all groupings with a score of 0
                //   4. Select the group that has the largest score; there may be no groups, however
                //   5. If there was at least one group, and therefore a max, grab all the Fields in that group;
                //        otherwise, produce an empty list of Fields
                var pkFields = schemas
                    .Select(schema => (PrimaryKeyScore(schema.Field, schema.Descriptor.InPrimaryKey), schema.Field))
                    .GroupBy(pair => pair.Item1)
                    .Where(group => group.Key != 0)
                    .MaxBy(group => group.Key)?
                    .Select(element => element.Field) ?? Enumerable.Empty<IField>();

                // If no deduction has been made yet, continue on to (4)
                if (pkFields.IsEmpty()) {
                    var possibleCKs = candidateKeys.Where(ck => ck.Fields.All(f => f.Nullability == IsNullable.No));
                    if (possibleCKs.Count() == 1) {
                        var deducedCK = possibleCKs.First();
                        pkFields = possibleCKs.First().Fields;

                        if (pkName is null && deducedCK.Name.Exists(n => !n.ToString().StartsWith(UniqueAttribute.ANONYMOUS_PREFIX))) {
                            pkName = deducedCK.Name.Unwrap();
                        }
                    }
                }

                // If no deduction has been made yet, continue on to (5)
                if (pkFields.IsEmpty()) {
                    var nonNullables = schemas.Select(s => s.Field).Where(f => f.Nullability == IsNullable.No);
                    if (nonNullables.Count() == 1) {
                        pkFields = nonNullables;
                    }
                }

                // Deduction is complete, either successfully or unsuccessfully
                if (pkFields.IsEmpty()) {
                    throw new CannotDeducePrimaryKeyException(context);
                }
                return pkName is null ? new PrimaryKey(pkFields) : new PrimaryKey(pkName, pkFields);
            }
        }


        /// <summary>
        ///   Produces the <see cref="ForeignKey">ForeignKeys</see> referenced by a set of Fields.
        /// </summary>
        /// <param name="fields">
        ///   The collection of Fields.
        /// </param>
        /// <returns>
        ///   A collection of <see cref="ForeignKey">ForeignKeys</see> referenced by <paramref name="fields"/>, in no
        ///   particular order.
        /// </returns>
        private IEnumerable<ForeignKey> CreateForeignKeys(IEnumerable<FieldGroup> fields) {
            var results = new List<ForeignKey>();
            foreach (var group in fields.SelectMany(g => g.References())) {
                var refType = group.Source.PropertyType;
                var refTable = principalTableCache_[refType].Table;

                // These Fields won't be the exact same instances as are in the referencing Table, but that's okay: we
                // only require that there are the correct number of Fields and in the correct order
                var keyFields = group.Select(fd => fd.MakeSchema(settings_).Field);
                results.Add(new ForeignKey(refTable, keyFields, OnDelete.Cascade, OnUpdate.Cascade));
            }
            return results;
        }
    }
}
