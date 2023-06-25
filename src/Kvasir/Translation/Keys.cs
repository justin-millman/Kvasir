using Cybele.Extensions;
using Kvasir.Annotations;
using Kvasir.Schema;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

using DescriptorSeq = System.Collections.Generic.IEnumerable<Kvasir.Translation.FieldDescriptor>;

// This code is responsible for deducing the Primary Key for an Entity and then removing any Candidate Keys that are
// made obsolete by that deduction. The deduction depends on a combination of annotations and Field properties,
// leveraging information already extracted (e.g. properties annotated with [PrimaryKey]) where appropriate. Any
// Candidate Key that is a superset of the Primary Key is redundant, but Candidate Keys that are subsets of the Primary
// Key are not.

namespace Kvasir.Translation {
    internal sealed partial class Translator {
        private static (PrimaryKey, IList<CandidateKey>) ComputeKeys(Type entity, FieldSeq fields, DescriptorSeq descriptors) {
            Debug.Assert(entity is not null);
            Debug.Assert(!EntityTypeCheck(entity).HasValue);
            Debug.Assert(descriptors is not null && !descriptors.IsEmpty());
            Debug.Assert(fields.Count() == descriptors.Count());

            // First, map-reduce the Candidate Key membership listings into a dictionary
            var candidateMapping = new Dictionary<string, HashSet<IField>>();
            foreach ((var field, var descriptor) in fields.Zip(descriptors)) {
                foreach (var key in descriptor.CandidateKeyMemberships) {
                    candidateMapping.TryAdd(key, new HashSet<IField>());
                    candidateMapping[key].Add(field);
                }
            }

            // Remove any Candidate Keys that are proper supersets of some other Candidate Key; if two Candidate Keys
            // are identical, take the one that is lexicographically last (which means that a non-anonymous one is
            // preferred to an anonymous one)
            var candidates = candidateMapping.Where(
                potential => candidateMapping.None(
                    other => other.Value.IsProperSubsetOf(potential.Value) ||
                             other.Value.SetEquals(potential.Value) && potential.Key.CompareTo(other.Key) < 0
                )
            ).Select(kvp => new CandidateKey(new KeyName(kvp.Key), kvp.Value));

            // Deduce the Primary Key; this also accounts for the Primary Key's name and any error detection/reporting
            // (e.g. an invalid Primary Key name, or a failed deduction)
            var primaryKey = DeducePrimaryKey(entity, fields, candidates, descriptors);

            // Remove any Candidate Keys that are a superset of the Primary Key (proper or improper)
            candidates = candidates.Where(key => !key.Fields.ToHashSet().IsSupersetOf(primaryKey.Fields));

            // It is an error for the name of the Primary Key to be the same as the name of any Candidate Key; if a
            // Candidate Key was deduced as the Primary Key and its name carried over, it will have been removed from
            // the Candidate Key set
            if (primaryKey.Name.HasValue && candidates.Any(k => k.Name.Contains(primaryKey.Name.Unwrap()))) {
                var msg = "conflicts with name of non-deduced Candidate Key";
                var annotation = entity.GetCustomAttribute<NamedPrimaryKeyAttribute>()!;
                throw Error.InvalidName(entity, annotation, annotation.Name, msg);
            }

            // No errors detected
            return (primaryKey, candidates.ToList());
        }

        private static PrimaryKey DeducePrimaryKey(Type entity, FieldSeq fields, IEnumerable<CandidateKey> candidates,
            DescriptorSeq descriptors) {

            Debug.Assert(entity is not null);
            Debug.Assert(!EntityTypeCheck(entity).HasValue);
            Debug.Assert(fields is not null && !fields.IsEmpty());
            Debug.Assert(candidates is not null);
            Debug.Assert(descriptors is not null && !descriptors.IsEmpty());
            Debug.Assert(fields.Count() == descriptors.Count());

            // It is an error for the value specified by a [NamedPrimaryKey] attribute to be invalid; currently, the
            // only concrete restriction is that the value is neither null nor the empty string
            var annotation = entity.GetCustomAttribute<NamedPrimaryKeyAttribute>();
            if (annotation is not null && (annotation.Name is null || annotation.Name == "")) {
                throw Error.InvalidName(entity, annotation, annotation.Name);
            }
            var name = annotation is null ? null : new KeyName(annotation.Name);

            // These are all the categories into which a Field could fall to be deduced as part of the Primary Key,
            // except for the "single Candidate Key of non-nullable Fields" which is covered by the candidates argument
            var annotated = new List<IField>();
            IField? namedID = null;
            IField? namedEntityID = null;
            IField? nonNullable = null;
            int nonNullableCount = 0;

            // Deduction rules are:
            //   (1) the set of Fields explicitly annotated as being in the Entity's Primary Key
            //   (2) the single non-nullable Field whose (post-annotation) name is "ID"
            //   (3) the single non-nullable Field whose (post-annotation) name is "<EntityType>ID"
            //   (4) the single Candidate Key consisting solely of non-nullable Fields
            //   (5) the single non-nullable Field
            //   (6) deduction fails
            foreach ((var field, var descriptor) in fields.Zip(descriptors)) {
                if (descriptor.InPrimaryKey) {
                    annotated.Add(field);
                }
                else if (field.Nullability == IsNullable.No && field.Name == new FieldName("ID")) {
                    namedID = field;
                }
                else if (field.Nullability == IsNullable.No && field.Name == new FieldName($"{entity.Name}ID")) {
                    namedEntityID = field;
                }
                else if (field.Nullability == IsNullable.No) {
                    nonNullable = field;
                    ++nonNullableCount;
                }
            }

            if (!annotated.IsEmpty()) {
                return name is null ? new PrimaryKey(annotated) : new PrimaryKey(name, annotated);
            }
            else if (namedID is not null) {
                var constituents = Enumerable.Repeat(namedID, 1);
                return name is null ? new PrimaryKey(constituents) : new PrimaryKey(name, constituents);
            }
            else if (namedEntityID is not null) {
                var constituents = Enumerable.Repeat(namedEntityID, 1);
                return name is null ? new PrimaryKey(constituents) : new PrimaryKey(name, constituents);
            }

            var deducedCK = candidates.Where(ck => ck.Fields.All(f => f.Nullability == IsNullable.No));
            if (deducedCK.Count() == 1) {
                var deduction = deducedCK.First();
                if (name is null && !deduction.Name.Exists(n => n.ToString().StartsWith(UniqueAttribute.ANONYMOUS_PREFIX))) {
                    name = deduction.Name.Unwrap();
                }
                return name is null ? new PrimaryKey(deduction.Fields) : new PrimaryKey(name, deduction.Fields);
            }

            if (nonNullable is not null && nonNullableCount == 1) {
                var constituents = Enumerable.Repeat(nonNullable, 1);
                return name is null ? new PrimaryKey(constituents) : new PrimaryKey(name, constituents);
            }

            // It is an error for an Entity to have no Primary Key
            throw Error.UserError(entity, "could not deduce Primary Key");
        }
    }
}
