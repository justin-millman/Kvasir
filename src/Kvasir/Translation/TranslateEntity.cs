using Cybele.Extensions;
using Kvasir.Exceptions;
using Kvasir.Schema;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kvasir.Translation {
    internal sealed partial class Translator {
        /// <summary>
        ///   Translate a single Entity Type.
        /// </summary>
        /// <param name="clr">
        ///   The Entity Type.
        /// </param>
        /// <exception cref="KvasirException">
        ///   if <paramref name="clr"/> is not a valid Entity Type
        ///     --or--
        ///   if the Translation of <paramref name="clr"/> contains any Tables with exactly 0 or 1 back-end Fields
        ///     --or--
        ///   if the name of any Tables in the Translation of <paramref name="clr"/> has a name that is not globally
        ///   unique
        ///     --or--
        ///   if the Translation of <paramref name="clr"/> contains any Table with two or more Field sharing a single
        ///   name
        ///     --or--
        ///   if the Translation of <paramref name="clr"/> contains any Table in which one or more Candidate Keys is a
        ///   superset (not necessarily proper) of the Table's Primary Key
        ///     --or--
        ///   if the Translation of <paramref name="clr"/> contains any Table in which the name of the Primary Key is
        ///   the same as the name of any of the Table's Candidate Keys.
        /// </exception>
        /// <returns>
        ///   The Translation of <paramref name="clr"/>.
        /// </returns>
        private Translation TranslateEntity(Type clr) {
            // If we've already translated the Type once, then simply return the memoized TypeDescriptor; the upstream
            // caller may modify the result based on property-level annotations
            if (entityCache_.TryGetValue(clr, out Translation? result)) {
                return result;
            }

            // Make sure that the Type can actually be used as an Entity Type
            CheckEntityType(clr);

            // Get the TypeDescriptor for the Type; this will perform all of the property-level error checking and put
            // the FieldDescriptors in the correct order
            var descriptor = TranslateType(clr);

            // It is an error for an Entity to have fewer than two Fields
            if (descriptor.Fields.Count < 2) {
                throw new KvasirException(
                    $"{clr.Name} cannot be an Entity Type: " +
                    $"at least 2 Fields are required (only {descriptor.Fields.Count} Fields found)"
                );
            }

            // It is an error for the name of a Table match that of another Table
            var tableName = NameOf(clr);
            if (!tableNames_.Add(tableName)) {
                throw new KvasirException(
                    $"Error translating Entity Type {clr.Name}: " +
                    $"Primary Table name \"{tableName}\" is already in use"
                );
            }

            // It is an error for a Table to contain two or more Fields with the same name
            var fields = new List<IField>(MakeFieldsFrom(descriptor.Fields)).ToList();
            var fieldNames = new HashSet<FieldName>();
            foreach (var field in fields) {
                if (!fieldNames.Add(field.Name)) {
                    throw new KvasirException(
                        $"Error translating Entity Type {clr.Name}: duplicate Field name \"{field.Name}\" encountered"
                    );
                }
            }

            // Create the Candidate Keys, then deduce the Primary Key
            var candidateKeys = MakeCandidateKeysFrom(descriptor.Fields, fields).ToList();
            var primaryKey = CreatePrimaryKeyFor(clr, descriptor.Fields, fields, candidateKeys);

            // It is an error for the Primary Key of a Table to be a superset of any of the Table's Candidate Keys
            var pkSet = new HashSet<FieldName>(primaryKey.Fields.Select(f => f.Name));
            foreach (var candidate in candidateKeys) {
                var ckSet = new HashSet<FieldName>(candidate.Fields.Select(f => f.Name));
                if (ckSet.IsSupersetOf(pkSet)) {
                    throw new KvasirException(
                        $"Error translating Entity Type {clr.Name}: " +
                        $"Candidate Key \"{candidate.Name.Unwrap()}\" ({string.Join(", ", ckSet)}) " +
                        $"is a superset of the Primary Key ({string.Join(", ", pkSet)})"
                    );
                }
            }

            // It is an error for the name of the Primary Key of a Table to be the same as the name of any of the
            // Table's Candidate Keys
            if (primaryKey.Name.HasValue && candidateKeys.Any(ck => ck.Name == primaryKey.Name)) {
                throw new KvasirException(
                    $"Error translating Entity Type {clr.Name}: " +
                    $"[NamedPrimaryKey] \"{primaryKey.Name.Unwrap()}\" clashes with name of unrelated Candidate Key"
                );
            }

            // Invoke the "generators" to create the CHECK clauses now that we have concrete IField instances
            var converters = descriptor.Fields.Select(f => f.Converter);
            var checks = descriptor.CHECKs.Select(g => new CheckConstraint(g(fields, converters))).ToList();
            foreach (var (field, desc) in fields.Zip(descriptor.Fields)) {
                foreach (var generator in desc.CHECKs) {
                    checks.Add(new CheckConstraint(generator(field, desc.Converter)));
                }
            }

            // These are not currently supported at the Translation Layer, but they are present in the Schema Layer
            var foreignKeys = new List<ForeignKey>();

            // No errors detected
            var table = new Table(tableName, fields, primaryKey, candidateKeys, foreignKeys, checks);
            var def = new PrincipalTableDef(table);
            var translation = new Translation(clr, def);
            entityCache_.Add(clr, translation);
            return translation;
        }

        /// <summary>
        ///   Converts one or more <see cref="FieldDescriptor">FieldDescriptors</see> into their corresponding
        ///   <see cref="IField">Fields</see>.
        /// </summary>
        /// <param name="descriptors">
        ///   The <see cref="FieldDescriptor">FieldDescriptors</see> describing the schema model.
        /// </param>
        /// <returns>
        ///   A finite enumerable of <see cref="IField">Fields</see>, one for each source descriptor in the same order
        ///   thereof.
        /// </returns>
        private static FieldSeq MakeFieldsFrom(IEnumerable<FieldDescriptor> descriptors) {
            foreach (var descriptor in descriptors) {
                var dbType = DBType.Lookup(descriptor.Converter.ResultType);
                var defaultValue = descriptor.RawDefault.Map(obj => DBValue.Create(descriptor.Converter.Convert(obj)));
                yield return new BasicField(descriptor.Name, dbType, descriptor.Nullability, defaultValue);
            }
        }
    }
}
