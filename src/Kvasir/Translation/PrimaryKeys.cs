using Cybele.Extensions;
using Kvasir.Annotations;
using Kvasir.Exceptions;
using Kvasir.Schema;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Kvasir.Translation {
    internal sealed partial class Translator {
        /// <summary>
        ///   Determines if a Field backing a scalar property is explicitly proscribed as being part of the Primary Key
        ///   of the owning Table.
        /// </summary>
        /// <param name="property">
        ///   The source <see cref="PropertyInfo">property</see>.
        /// </param>
        /// <param name="nullability">
        ///   The nullability of the Field backing <paramref name="property"/>.
        /// </param>
        /// <exception cref="KvasirException">
        ///   if <paramref name="property"/> is annotated with multiple <c>[PrimaryKey]</c> attributes
        ///     --or--
        ///   if the <c>[PrimaryKey]</c> annotation applied to <paramref name="property"/> has a non-empty
        ///   <see cref="PrimaryKeyAttribute.Path">Path</see>
        ///     --or--
        ///   if <paramref name="nullability"/> is <see cref="IsNullable.Yes"/> but the <c>[PrimaryKey]</c> annotation
        ///   is present on <paramref name="property"/>.
        /// </exception>
        /// <returns>
        ///   <see langword="true"/> if <paramref name="property"/> is annotated with a <c>[PrimaryKey]</c> attribute;
        ///   otherwise, <see langword="false"/>.
        /// </returns>
        private static bool IsInPrimaryKey(PropertyInfo property, IsNullable nullability) {
            // It is an error for a property to be annotated with multiple [PrimaryKey] attributes
            var annotations = property.GetCustomAttributes<PrimaryKeyAttribute>();
            if (annotations.Count() > 1) {
                throw new KvasirException(
                    $"Error translating property {property.Name} of type {property.ReflectedType!.Name}: " +
                    "multiple [PrimaryKey] annotations encountered"
                );
            }
            var annotation = annotations.FirstOrDefault();

            // It is an error for the [PrimaryKey] attribute of a scalar property to have a non-empty <Path> value
            if (annotation is not null && annotation.Path != "") {
                throw new KvasirException(
                    $"Error translating property {property.Name} of type {property.ReflectedType!.Name}: " +
                    $"path \"{annotation.Path}\" of [PrimaryKey] annotation does not exist"
                );
            }

            // It is an error for a nullable Field to be annotated as being part of the [PrimaryKey]
            if (annotation is not null && nullability == IsNullable.Yes) {
                throw new KvasirException(
                    $"Error translating property {property.Name} of type {property.ReflectedType!.Name}: " +
                    "nullable Field cannot be annotated as [PrimaryKey]"
                );
            }

            // No errors detected
            return annotation is not null;
        }

        /// <summary>
        ///   Deduce the <see cref="PrimaryKey">Primary Key</see> for a the Table backing an Entity Type.
        /// </summary>
        /// <param name="entity">
        ///   The source Entity Type.
        /// </param>
        /// <param name="descriptors">
        ///   The <see cref="FieldDescriptor">Field Descriptors</see> for <paramref name="entity"/>.
        /// </param>
        /// <param name="fields">
        ///   The actual <see cref="IField">Field</see> objects based on <paramref name="descriptors"/>. The <c>Nth</c>
        ///   Field was created from the <c>Nth</c> FieldDescriptor.
        /// </param>
        /// <param name="candidates">
        ///   The <see cref="CandidateKey">Candidate Keys</see> for <paramref name="entity"/>. If a Candidate Key is
        ///   deduced as the Primary Key, it will be <i>REMOVED</i> from this list.
        /// </param>
        /// <exception cref="KvasirException">
        ///   if <paramref name="entity"/> is annotated with a <c>[NamedPrimaryKey]</c> whose value is invalid
        ///     --or--
        ///   if <paramref name="entity"/> is annotated with a <c>[NamedPrimaryKey]</c> and the deduced Primary Key is a
        ///   named Candidate Key
        ///     --or--
        ///   if the Primary Key of <paramref name="entity"/> cannot be deduced.
        /// </exception>
        /// <returns>
        ///   The <see cref="PrimaryKey">Primary Key</see> of the Table backing <paramref name="entity"/>.
        /// </returns>
        private static PrimaryKey CreatePrimaryKeyFor(Type entity, IEnumerable<FieldDescriptor> descriptors,
            FieldSeq fields, IList<CandidateKey> candidates) {

            var annotated = new List<IField>();
            IField? namedID = null;
            IField? namedTableID = null;
            IField? nonNullable = null;
            var nonNullableCount = 0;
            var name = entity.GetCustomAttribute<NamedPrimaryKeyAttribute>()?.Name;

            // It is an error for the value of a [NamedPrimaryKey] annotation to be invalid as the name of a Key;
            // currently, the only restrictions on this is that the name must have non-zero length (there is no back-end
            // awareness)
            if (name == "") {
                throw new KvasirException(
                    $"Error translating type {entity.Name}: " +
                    $"[NamedPrimaryKey] name argument \"{name}\" is not a valid Primary Key name"
                );
            }

            // #1) Any Fields that are annotated as [PrimaryKey]
            // #2) The single non-nullable Field named "ID"
            // #3) The single non-nullable Field named "<EntityType>ID"
            // #4) The single Candidate Key that consists of only non-nullable Fields
            // #5) The single non-nullable Field
            // #6) deduction fails with error
            foreach (var (descriptor, field) in descriptors.Zip(fields)) {
                if (descriptor.IsInPrimaryKey) {
                    Debug.Assert(descriptor.Nullability == IsNullable.No);
                    annotated.Add(field);
                }
                else if (field.Nullability == IsNullable.No && descriptor.Name.ToString() == "ID") {
                    namedID = field;
                }
                else if (field.Nullability == IsNullable.No && descriptor.Name.ToString() == $"{entity.Name}ID") {
                    namedTableID = field;
                }
                else if (field.Nullability == IsNullable.No) {
                    ++nonNullableCount;
                    nonNullable = field;
                }
            }

            if (!annotated.IsEmpty()) {
                return name is null ? new PrimaryKey(annotated) : new PrimaryKey(new KeyName(name), annotated);
            }
            if (namedID is not null || namedTableID is not null) {
                var key = Enumerable.Repeat(namedID ?? namedTableID, 1);
                return name is null ? new PrimaryKey(key!) : new PrimaryKey(new KeyName(name), key!);
            }

            var viableCandidates = candidates.Where(ck => ck.Fields.All(f => f.Nullability == IsNullable.No));
            if (viableCandidates.Count() == 1) {
                var deduction = viableCandidates.First();
                bool isAnonymous = deduction.Name.Exists(n => n.ToString().StartsWith(UniqueAttribute.ANONYMOUS_PREFIX));
                
                // It is an error for the value of a [NamedPrimaryKey] to be different than that of the Candidate Key
                // deduced as the Primary Key, unless the Candidate Key is anonymous
                if (name is not null && !isAnonymous && deduction.Name.Exists(n => n.ToString() != name)) {
                    throw new KvasirException(
                        $"Error translating type {entity.Name}: " +
                        $"a Candidate Key with name \"{deduction.Name.Unwrap()}\" was deduced as the Primary Key, " +
                        $"but the [NamedPrimaryKey] of \"{name}\" is conflicting"
                    );
                }

                // It is an error for the value of a [NamedPrimaryKey] to be different the same as that of the Candidate
                // Key deduced as the Primary Key, as the former annotation is then redundant
                if (deduction.Name.Exists(n => n.ToString() == name)) {
                    throw new KvasirException(
                        $"Error translating type {entity.Name}: " +
                        $"a Candidate Key with name \"{deduction.Name.Unwrap()}\" was deduced as the Primary Key, " +
                        "rendering the [NamedPrimaryKey] annotation redundant"
                    );
                }

                // No errors detected; need to remove the deduced Candidate Key to avoid inducing a subset/superset
                // error downstream
                candidates.Remove(deduction);
                return new PrimaryKey(name is null ? deduction.Name.Unwrap() : new KeyName(name), deduction.Fields);
            }

            if (nonNullable is not null && nonNullableCount == 1) {
                var key = Enumerable.Repeat(nonNullable, 1);
                return name is null ? new PrimaryKey(key) : new PrimaryKey(new KeyName(name), key);
            }

            // It is an error for the deduction of a Table's Primary Key to fail
            throw new KvasirException($"Error translating type {entity.Name}: unable to deduce Primary Key");
        }
    }
}
