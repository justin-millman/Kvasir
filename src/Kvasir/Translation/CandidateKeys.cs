using Kvasir.Annotations;
using Kvasir.Exceptions;
using Kvasir.Schema;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Kvasir.Translation {
    internal sealed partial class Translator {
        /// <summary>
        ///   Determine the names of the Candidate Keys to which the Field backing a scalar property belongs.
        /// </summary>
        /// <param name="property">
        ///   the source <see cref="PropertyInfo">property</see>.
        /// </param>
        /// <exception cref="KvasirException">
        ///   if any of the <c>[Unique]</c> annotations applied to <paramref name="property"/> has a non-empty
        ///   <see cref="UniqueAttribute.Path">Path</see>.
        ///     --or--
        ///   if the value of any of the <c>[Unique]</c> annotations applied to <paramref name="property"/> is invalid
        ///   or reserved
        ///     --or--
        ///   if two or more of the <c>[Unique]</c> annotations applied to <paramref name="property"/> have the same
        ///   value
        ///     --or--
        ///   if two or more of the <c>[Unique]</c> annotations applied to <paramref name="property"/> are anonymous.
        /// </exception>
        /// <returns>
        ///   A finite, possibly empty enumerable of the <see cref="KeyName">key names</see> of the Candidate Keys to
        ///   which <paramref name="property"/> belong.
        /// </returns>
        private static IEnumerable<KeyName> CandidateKeysContaining(PropertyInfo property) {
            bool anonymousSeen = false;
            var namesSeen = new HashSet<string>();
            foreach (var annotation in property.GetCustomAttributes<UniqueAttribute>()) {
                // It is an error for the [Unique] attribute of a scalar property to have a non-empty <Path> value
                if (annotation.Path != "") {
                    throw new KvasirException(
                        $"Error translating property {property.Name} of type {property.ReflectedType!.Name}: " +
                        $"path \"{annotation.Path}\" of [Unique] annotation does not exist"
                    );
                }

                // It is an error for the value of a [Unique] annotation to be invalid as the name of a Key; currently,
                // the only restrictions on this is that the name must have non-zero length and cannot begin with @@@
                // (the latter is reserved, and there is no back-end awareness)
                if (annotation.Name == "") {
                    throw new KvasirException(
                        $"Error translating property {property.Name} of type {property.ReflectedType!.Name}: " +
                        $"[Unique] name argument \"{annotation.Name}\" is not a valid Candidate Key name"
                    );
                }
                if (!annotation.IsAnonymous && annotation.Name.StartsWith(UniqueAttribute.ANONYMOUS_PREFIX)) {
                    throw new KvasirException(
                        $"Error translating property {property.Name} of type {property.ReflectedType!.Name}: " +
                        $"[Unique] name argument \"{annotation.Name}\" begins with reserved character sequence " +
                        UniqueAttribute.ANONYMOUS_PREFIX
                    );
                }

                // It is an error for the value of two or more [Unique] annotations applied to the same scalar property
                // to have the same name; note that this doesn't cover repeated anonymous [Unique] annotations
                if (!namesSeen.Add(annotation.Name)) {
                    throw new KvasirException(
                        $"Error translating property {property.Name} of type {property.ReflectedType!.Name}: " +
                        $"two or more [Unique] annotations have the same name ('{annotation.Name}')"
                    );
                }

                // It is an error for a single property to be annotated with multiple [Unique] attributes that do not
                // specify a name (i.e. are "anonymous")
                if (anonymousSeen && annotation.IsAnonymous) {
                    throw new KvasirException(
                        $"Error translating property {property.Name} of type {property.ReflectedType!.Name}: " +
                        "two or more [Unique] annotations are anonymous (i.e. no name is provided)"
                    );
                }
                anonymousSeen |= annotation.IsAnonymous;

                // No errors detected
                yield return new KeyName(annotation.Name);
            }
        }

        /// <summary>
        ///   Create the set of <see cref="CandidateKey">CandidateKeys</see> from a Table's constituent Fields.
        /// </summary>
        /// <param name="descriptors">
        ///   The <see cref="FieldDescriptor">FieldDescriptors</see> from which <paramref name="fields"/> were created.
        /// </param>
        /// <param name="fields">
        ///   The actual <see cref="IField">Field</see> objects based on <paramref name="descriptors"/>. The <c>Nth</c>
        ///   Field was created from the <c>Nth</c> FieldDescriptor.
        /// </param>
        /// <exception cref="KvasirException">
        ///   if any two of the Candidate Keys that would be produced consist of the same set of Fields.
        /// </exception>
        /// <returns>
        ///   A finite, possibly empty enumerable of the <see cref="CandidateKey">Candidate Keys</see> defined by the
        ///   <paramref name="descriptors"/>, populated with elements of <paramref name="fields"/>.
        /// </returns>
        private static IEnumerable<CandidateKey> MakeCandidateKeysFrom(IEnumerable<FieldDescriptor> descriptors,
            FieldSeq fields) {

            var keys = new Dictionary<KeyName, IList<IField>>();
            foreach (var (descriptor, field) in descriptors.Zip(fields)) {
                foreach (var candidate in descriptor.KeyMemberships) {
                    keys.TryAdd(candidate, new List<IField>());
                    Debug.Assert(!keys[candidate].Contains(field));
                    keys[candidate].Add(field);
                }
            }

            var reprs = new Dictionary<string, KeyName>();
            foreach (var (name, members) in keys) {
                // Using a string character here is technically dangerous, as the vertical bar is not a reserved
                // character and therefore could appear naturally in a Field name via [Name], but this is unlikely and
                // the alternative (using a collection with a custom aggregating hash function) is overkill
                //
                // No need to sort the Fields because the orders will be consistent with the iteration from the above
                // loop, so e.g. exactly one of (A, B, C) and (A, C, B) is possible
                var repr = string.Join('|', members.Select(f => f.Name));
                if (reprs.TryGetValue(repr, out KeyName? existing)) {
                    throw new KvasirException(
                        $"Error translating type {descriptors.First().SourceType.Name}: " +
                        $"Candidate Key '{name}' and '{existing}' are comprised of the same Fields " +
                        $"({repr.Replace("|", ", ")})"
                    );
                }

                reprs[repr] = name;
                yield return new CandidateKey(name, members);
            }
        }
    }
}
