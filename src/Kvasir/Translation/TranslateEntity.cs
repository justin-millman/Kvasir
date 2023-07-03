using Cybele.Extensions;
using Kvasir.Annotations;
using Kvasir.Core;
using Kvasir.Exceptions;
using Kvasir.Schema;
using Optional;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Kvasir.Translation {
    internal sealed partial class Translator {
        /// <summary>
        ///   Translate an Entity Type.
        /// </summary>
        /// <param name="entity">
        ///   The Entity Type.
        /// </param>
        /// <returns>
        ///   The <see cref="Translation"/> of <paramref name="entity"/>.
        /// </returns>
        /// <exception cref="KvasirException">
        ///   if <paramref name="entity"/> cannot be translated for any reason.
        /// </exception>
        private Translation TranslateEntity(Type entity) {
            Debug.Assert(entity is not null);

            // If the type has already been translated, return the memoized (and already error-checked) result
            if (entityCache_.TryGetValue(entity, out var result)) {
                return result;
            }

            // It is an error for a prospective Entity type to be invalid
            EntityTypeCheck(entity).MatchSome(s => throw Error.UserError(entity, $"{s} cannot be an Entity type"));

            // Get translation of the Entity type; this is the translation without Entity-specific features, such as
            // primary key deduction, key collapse, and constraint flattening
            var typeTranslation = TranslateType(entity);

            // It is an error for the name of an Entity's Principal Table to be the same as that of another Table
            var tableName = GetTableName(entity);
            if (!tableNames_.Add(tableName)) {
                var msg = $"Table name \"{tableName}\" is already in use";
                throw Error.UserError(entity, msg);
            }

            // Create the individual Fields and all the constraints for the Entity; we need the actual Fields first,
            // because the constraint Clauses operate in terms of Fields
            var fields = new List<IField>();
            var constraints = new List<CheckConstraint>();
            var converters = typeTranslation.Fields.Values.Select(d => d.Converter);
            foreach (var descriptor in typeTranslation.Fields.Values) {
                var flattened = FlattenConstraints(descriptor);
                var field = MakeField(flattened);

                // It is an error for an Entity to have two or more Fields with the same name
                if (fields.Any(f => f.Name == field.Name)) {
                    var msg = $"two or more Fields with name \"{field.Name}\"";
                    throw Error.UserError(entity, msg);
                }

                fields.Add(field);
                constraints.AddRange(MakeConstraints(flattened, fields[^1]));
            }
            foreach (var custom in typeTranslation.CHECKs) {
                constraints.Add(new CheckConstraint(custom(fields, converters)));
            }

            // It is an error for an Entity to have fewer than 2 Fields
            if (fields.Count < 2) {
                throw Error.UserError(entity, $"at least 2 Fields required ({fields.Count} found)");
            }

            // Compute the Primary Key and all Candidate Keys. This includes performing Primary Key deduction (and
            // corresponding error checking), collapsing Candidate Keys to account for subset/superset relations, and
            // eliminating Candidate Keys that are redundant with the Primary Key.
            (var primaryKey, var candidateKeys) = ComputeKeys(entity, fields, typeTranslation.Fields.Values);

            // Foreign Keys are part of the Schema Layer, but they are not yet supported by the Translation Layer
            var foreignKeys = Enumerable.Empty<ForeignKey>();

            // Construct the final Translation
            var table = new Table(tableName, fields, primaryKey, candidateKeys, foreignKeys, constraints);
            var principal = new PrincipalTableDef(table, null!, null!);
            var entityTranslation = new Translation(entity, principal, Enumerable.Empty<RelationTableDef>());
            entityCache_.Add(entity, entityTranslation);
            return entityTranslation;
        }

        /// <summary>
        ///   Check if a particular CLR <see cref="Type"/> can serve as the source of an Entity translation.
        /// </summary>
        /// <param name="type">
        ///   The <see cref="Type"/>.
        /// </param>
        /// <returns>
        ///   A <c>NONE</c> instance if <paramref name="type"/> is a valid Entity type; otherwise a <c>SOME</c> instance
        ///   carrying an explanation as to why it is not.
        /// </returns>
        private static Option<string> EntityTypeCheck(Type type) {
            Debug.Assert(type is not null);

            // An Entity type cannot be an enumeration
            if (type.IsEnum) {
                return Option.Some("an enumeration");
            }

            // An Entity type cannot be a struct or a record struct
            if (type.IsValueType) {
                return Option.Some("a struct or a record struct");
            }

            // An Entity type cannot be a delegate
            if (type.IsInstanceOf(typeof(Delegate))) {
                return Option.Some("a delegate");
            }

            // An Entity type cannot be an interface
            if (type.IsInterface) {
                return Option.Some("an interface");
            }

            // An Entity type cannot be an open generic
            if (type.IsGenericTypeDefinition) {
                return Option.Some("an open generic");
            }

            // An Entity type cannot be a closed generic
            if (type.IsGenericType) {
                return Option.Some("a closed generic");
            }

            // An Entity type cannot be abstract
            if (type.IsAbstract) {
                return Option.Some("an abstract class or an abstract record class");
            }

            // Valid Entity type
            return Option.None<string>();
        }

        /// <summary>
        ///   Determine the name of the Principal Table for an Entity Type.
        /// </summary>
        /// <param name="entity">
        ///   The Entity Type.
        /// </param>
        /// <returns>
        ///   The name of the Principal Table for <paramref name="entity"/>.
        /// </returns>
        /// <exception cref="KvasirException">
        ///   if the name of the Principal Table specified for <paramref name="entity"/> via annotations is invalid.
        /// </exception>
        private static TableName GetTableName(Type entity) {
            Debug.Assert(entity is not null);

            var annotation = entity.GetCustomAttribute<TableAttribute>();
            var excludeNS = entity.HasAttribute<ExcludeNamespaceFromNameAttribute>();

            // It is an error for the value specified by a [Table] attribute to be invalid; currently, the only concrete
            // restriction is that the value is neither null nor the empty string
            if (annotation is not null && (annotation.Name is null || annotation.Name == "")) {
                throw Error.InvalidName(entity, annotation, annotation.Name);
            }

            // No errors encountered
            if (annotation is null) {
                return new TableName((excludeNS ? entity.Name : entity.FullName) + "Table");
            }
            else if (!excludeNS) {
                return new TableName(annotation.Name);
            }
            else {
                // [ExcludeNamespaceFromName] also includes the removal of outer class identifiers
                var name = annotation.Name.Replace(entity.Namespace ?? "", "");
                name = name[0] == '.' ? name[1..] : name;
                name = name[(name.IndexOf('+') + 1)..];
                return new TableName(name);
            }
        }

        /// <summary>
        ///   Condense all of the non-arbitrary constraints applied to a Field such that the minimum set of constraints
        ///   is present.
        /// </summary>
        /// <remarks>
        ///   If the Field has a <see cref="Check.IsOneOfAttribute"><c>[Check.IsOneOf]</c></see> annotation applied,
        ///   then all other constraints are redundant, except that the allowed values are reduced to those that pass
        ///   the other constraints. Otherwise, if the Field has a
        ///   <see cref="Check.IsNotOneOfAttribute"><c>[Check.IsNotOneOf]</c></see> annotation, any disallowed value
        ///   that does not pass the other constraints is removed. Furthermore, if the lower and upper bounds create a
        ///   range of size <c>1</c>, that single value is treated as an "allowed value."
        /// </remarks>
        /// <param name="descriptor">
        ///   The source <see cref="FieldDescriptor"/>.
        /// </param>
        /// <returns>
        ///   A new <see cref="FieldDescriptor"/> that is identical to <paramref name="descriptor"/> in all aspects,
        ///   except that the <see cref="FieldDescriptor.Constraints">constraints</see> have been flattened.
        /// </returns>
        private static FieldDescriptor FlattenConstraints(FieldDescriptor descriptor) {
            var orig = descriptor.Constraints;

            // If there is only one value in the minimum/maximum range, that one value becomes an "allowed value." Any
            // other allowed values will be removed in the subsequent check for not passing the range constraint, which
            // will then be cleared out because of the presence of allowed values. Eventually, this will turn into a
            // single EQ constraint.
            if (orig.LowerBound.Exists(bl => orig.UpperBound.Exists(bu => bl.Value.Equals(bu.Value)))) {
                var allowed = new HashSet<object>(orig.AllowedValues) { orig.LowerBound.Unwrap().Value };
                orig = orig with { AllowedValues = allowed };
            }

            // If there is a constraint limiting the values to a discrete subset, then we have to check which ones pass
            // all the other constraints, discarding those that do not. Then, we can clear out all the other
            // constraints, as [IsOneOf] is the most restrictive.
            if (!orig.AllowedValues.IsEmpty() || !orig.RestrictedImage.IsEmpty()) {
                var effective = orig.AllowedValues.IsEmpty() ? orig.RestrictedImage : orig.AllowedValues;

                var stillValid = effective
                    .Where(v => !orig.DisallowedValues.Contains(v))
                    .Where(v => IsWithinInterval(v, orig.LowerBound, orig.UpperBound))
                    .Where(v => v is not string s || IsWithinInterval(s.Length, orig.MinimumLength, orig.MaximumLength))
                    .Where(v => orig.RestrictedImage.IsEmpty() || orig.RestrictedImage.Contains(v));

                Debug.Assert(!stillValid.IsEmpty());
                return descriptor with {
                    Constraints = new ConstraintBucket(
                        RelativeToZero: Option.None<ComparisonOperator>(),
                        LowerBound: Option.None<Bound>(),
                        UpperBound: Option.None<Bound>(),
                        MinimumLength: Option.None<Bound>(),
                        MaximumLength: Option.None<Bound>(),
                        AllowedValues: stillValid.ToHashSet(),
                        DisallowedValues: new HashSet<object>(),
                        RestrictedImage: new HashSet<object>(),
                        CHECKs: orig.CHECKs
                    )
                };
            }

            // If there are any disallowed values, we have to remove those that don't pass the other constraints
            if (!orig.DisallowedValues.IsEmpty()) {
                var remaining = orig.DisallowedValues
                    .Where(v => IsWithinInterval(v, orig.LowerBound, orig.UpperBound))
                    .Where(v => v is not string s || IsWithinInterval(s.Length, orig.MinimumLength, orig.MaximumLength));

                return descriptor with { Constraints = orig with { DisallowedValues = remaining.ToHashSet() } };
            }

            // No flattening to do
            return descriptor;
        }

        /// <summary>
        ///   Create a <see cref="IField">Field</see> from a <see cref="FieldDescriptor"/>.
        /// </summary>
        /// <param name="descriptor">
        ///   The <see cref="FieldDescriptor"/> describing the prospective <see cref="IField">Field</see>.
        /// </param>
        /// <returns>
        ///   A <see cref="IField">Field</see> based on <paramref name="descriptor"/>.
        /// </returns>
        private static IField MakeField(FieldDescriptor descriptor) {
            if (DBType.Lookup(descriptor.Converter.ResultType) != DBType.Enumeration) {
                return new BasicField(
                    name: new FieldName(descriptor.Name),
                    dataType: DBType.Lookup(descriptor.Converter.ResultType),
                    nullability: descriptor.Nullability,
                    defaultValue: descriptor.Default.Map(v => DBValue.Create(v))
                );
            }
            else {
                var enumType = Nullable.GetUnderlyingType(descriptor.Converter.ResultType) ?? descriptor.Converter.ResultType;
                var converter = new EnumToStringConverter(enumType);
                Func<object?, DBValue> convFn = v => DBValue.Create(converter.ConverterImpl.Convert(v));

                return new EnumField(
                    name: new FieldName(descriptor.Name),
                    nullability: descriptor.Nullability,
                    defaultValue: descriptor.Default.Map(convFn),
                    enumerators: descriptor.Constraints.AllowedValues.Select(convFn)
                );
            }
        }

        /// <summary>
        ///   Create the <see cref="CheckConstraint"><c>CHECK</c> constraints</see> for a
        ///   <see cref="IField">Field</see>.
        /// </summary>
        /// <param name="descriptor">
        ///   The <see cref="FieldDescriptor"/> describing the constraints.
        /// </param>
        /// <param name="field">
        ///   The <see cref="IField">Field</see> to which the <c>CHECK</c> constraints apply.
        /// </param>
        /// <returns>
        ///   A (possibly empty) collection of <c>CHECK</c> constraints that apply to <paramref name="field"/>.
        /// </returns>
        private static IEnumerable<CheckConstraint> MakeConstraints(FieldDescriptor descriptor, IField field) {
            Debug.Assert(field is not null);
            var constraints = descriptor.Constraints;

            // Ranged Comparison Constraints
            if (constraints.LowerBound.HasValue) {
                var bound = constraints.LowerBound.Unwrap();
                var op = bound.IsInclusive ? ComparisonOperator.GTE : ComparisonOperator.GT;
                var clause = new ConstantClause(new FieldExpression(field), op, DBValue.Create(bound.Value));
                yield return new CheckConstraint(clause);
            }
            if (constraints.UpperBound.HasValue) {
                var bound = constraints.UpperBound.Unwrap();
                var op = bound.IsInclusive ? ComparisonOperator.LTE : ComparisonOperator.LT;
                var clause = new ConstantClause(new FieldExpression(field), op, DBValue.Create(bound.Value));
                yield return new CheckConstraint(clause);
            }

            // String Length Constraints
            if (constraints.MinimumLength.Exists(min => constraints.MaximumLength.Exists(max => min == max))) {
                var bound = constraints.MinimumLength.Unwrap();
                var expr = new FieldExpression(FieldFunction.LengthOf, field);
                var clause = new ConstantClause(expr, ComparisonOperator.EQ, DBValue.Create(bound.Value));
                yield return new CheckConstraint(clause);
            }
            else {
                if (constraints.MinimumLength.HasValue) {
                    var bound = constraints.MinimumLength.Unwrap();
                    var expr = new FieldExpression(FieldFunction.LengthOf, field);
                    var clause = new ConstantClause(expr, ComparisonOperator.GTE, DBValue.Create(bound.Value));
                    yield return new CheckConstraint(clause);
                }
                if (constraints.MaximumLength.HasValue) {
                    var bound = constraints.MaximumLength.Unwrap();
                    var expr = new FieldExpression(FieldFunction.LengthOf, field);
                    var clause = new ConstantClause(expr, ComparisonOperator.LTE, DBValue.Create(bound.Value));
                    yield return new CheckConstraint(clause);
                }
            }

            // A single allowed value becomes an EQ constraint
            if (field.DataType != DBType.Enumeration && !constraints.AllowedValues.IsEmpty()) {
                if (constraints.AllowedValues.Count == 1) {
                    var value = DBValue.Create(constraints.AllowedValues.First());
                    var clause = new ConstantClause(new FieldExpression(field), ComparisonOperator.EQ, value);
                    yield return new CheckConstraint(clause);
                }
                else {
                    var values = constraints.AllowedValues.Select(v => DBValue.Create(v));
                    var clause = new InclusionClause(new FieldExpression(field), InclusionOperator.In, values);
                    yield return new CheckConstraint(clause);
                }
            }

            // A single disallowed value becomes an NE constraint
            if (field.DataType != DBType.Enumeration && !constraints.DisallowedValues.IsEmpty()) {
                if (constraints.DisallowedValues.Count == 1) {
                    var value = DBValue.Create(constraints.DisallowedValues.First());
                    var clause = new ConstantClause(new FieldExpression(field), ComparisonOperator.NE, value);
                    yield return new CheckConstraint(clause);
                }
                else {
                    var values = constraints.DisallowedValues.Select(v => DBValue.Create(v));
                    var clause = new InclusionClause(new FieldExpression(field), InclusionOperator.NotIn, values);
                    yield return new CheckConstraint(clause);
                }
            }

            // Custom constraints
            foreach (var custom in constraints.CHECKs) {
                yield return new CheckConstraint(custom(field, descriptor.Converter));
            }
        }
    }
}
