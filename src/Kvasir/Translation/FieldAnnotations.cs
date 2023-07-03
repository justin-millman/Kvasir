using Cybele.Extensions;
using Kvasir.Annotations;
using Kvasir.Core;
using Kvasir.Schema;
using Optional;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

// This code takes a "base translation" - which is intentionally agnostic of the source property's category - and
// applies annotations. In some cases, it is necessary to reverse engineer the initial category; for example, a
// [DataConverter] annotation can only be applied to a Scalar or an Enumeration property. But for the most part, the
// annotations result in the descriptor being updated in some manner. If there are no annotations, then the "base
// translation" is 100% accurate.
//
// The code for producing the original "base translations" can be found in the BaseTranslations.cs file. The code for
// applying constraint annotations can be found in the FieldConstraints.cs file.

namespace Kvasir.Translation {
    internal sealed partial class Translator {
        private FieldsListing ApplyAnnotations(PropertyInfo property, FieldsListing baseTranslation) {
            Debug.Assert(property is not null);
            Debug.Assert(baseTranslation is not null);
            Debug.Assert(!baseTranslation.IsEmpty());

            var result = new Dictionary<string, FieldDescriptor>(baseTranslation);
            ProcessNames(property, result);
            ProcessNullability(property, result);
            ProcessColumn(property, result);
            ProcessConverters(property, result);
            ProcessDefaults(property, result);              // must come after Nullability and Data Converters
            ProcessPrimaryKeyOptIns(property, result);      // must come after Nullability
            ProcessCandidateKeys(property, result);
            ProcessConstraints(property, result);           // must come after Data Converters

            return result;
        }

        private static void ProcessNames(PropertyInfo property, Dictionary<string, FieldDescriptor> state) {
            Debug.Assert(property is not null);
            Debug.Assert(state is not null);
            HashSet<string> processed = new HashSet<string>();

            foreach (var annotation in property.GetCustomAttributes<NameAttribute>()) {
                var context = new PropertyTranslationContext(property, annotation.Path);

                // It is an error for a [Name] attribute to have a null Path
                if (annotation.Path is null) {
                    throw Error.InvalidPath(context, annotation);
                }

                // It is an error for a [Name] attribute to have a non-existent Path
                if (!state.TryGetValue(annotation.Path, out FieldDescriptor target)) {
                    throw Error.InvalidPath(context, annotation);
                }

                // It is an error for the value specified by a [Name] attribute to be invalid; currently, the only
                // concrete restriction is that the value is neither null nor the empty string
                if (annotation.Name is null || annotation.Name == "") {
                    throw Error.InvalidName(context, annotation, annotation.Name);
                }

                // It is an error for multiple [Name] attributes on a single property to apply to the same Path, though
                // it is legal if a [Name] attribute on an outer aggregate to override that of an inner aggregate
                if (!processed.Add(annotation.Path)) {
                    throw Error.DuplicateAnnotation(context, annotation);
                }

                // No errors encountered
                state[annotation.Path] = target with { Name = annotation.Name };
            }
        }

        private static void ProcessNullability(PropertyInfo property, Dictionary<string, FieldDescriptor> state) {
            Debug.Assert(property is not null);
            Debug.Assert(state is not null);

            var nullable = property.HasAttribute<NullableAttribute>();
            var nonNullable = property.HasAttribute<NonNullableAttribute>();

            if (nullable || nonNullable) {
                // It is an error for a property to be annotated as both [Nullable] and [NonNullable]
                if (nullable && nonNullable) {
                    var context = new PropertyTranslationContext(property, "");
                    throw Error.MutuallyExclusive(context, new NullableAttribute(), new NonNullableAttribute());
                }

                // Note: There is some non-trivial work to do here when dealing with non-scalars. For example, a
                // Reference property cannot take a [NonNullable] annotation, and an Aggregate property cannot take a
                // [Nullable] annotation if all of its constituent Fields are already nullable. We will deal with those
                // nuances when we implement translation for those property categories.
                foreach ((var path, var descriptor) in state) {
                    state[path] = descriptor with { Nullability = nullable ? IsNullable.Yes : IsNullable.No };
                }
            }
        }

        private static void ProcessColumn(PropertyInfo property, Dictionary<string, FieldDescriptor> state) {
            Debug.Assert(property is not null);
            Debug.Assert(state is not null);
            var annotation = property.GetCustomAttribute<ColumnAttribute>();

            if (annotation is not null) {
                // It is an error for the index of a [Column] annotation to be negative
                if (annotation.Column < 0) {
                    var context = new PropertyTranslationContext(property, "");
                    var msg = $"column index {annotation.Column} is negative";
                    throw Error.UserError(context, annotation, msg);
                }

                foreach ((var path, var descriptor) in state) {
                    state[path] = descriptor with {
                        Column = descriptor.Column.Match(
                            some: c => Option.Some(c + annotation.Column),
                            none: () => Option.Some(annotation.Column)
                        )
                    };
                }
            }
        }

        private static void ProcessConverters(PropertyInfo property, Dictionary<string, FieldDescriptor> state) {
            Debug.Assert(property is not null);
            Debug.Assert(state is not null);
            Debug.Assert(state.Count == 1 && state.ContainsKey(""));        // will need to be a proper check later

            var dpAnnotation = property.GetCustomAttribute<DataConverterAttribute>();
            var numeric = property.HasAttribute<NumericAttribute>();
            var asString = property.HasAttribute<AsStringAttribute>();

            var context = new PropertyTranslationContext(property, "");
            var expectedType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

            if (dpAnnotation is not null) {
                // It is an error for a property to be annotated with both [DataProvider] and [Numeric]
                if (numeric) {
                    throw Error.MutuallyExclusive(context, dpAnnotation, new NumericAttribute());
                }

                // It is an error for a property to be annotated with both [DataProvider] and [AsString]
                if (asString) {
                    throw Error.MutuallyExclusive(context, dpAnnotation, new AsStringAttribute());
                }

                // It is an error for a [DataConverter] annotation to have a populated <UserError>
                if (dpAnnotation.UserError is not null) {
                    throw Error.UserError(context, dpAnnotation, dpAnnotation.UserError);
                }

                var converter = dpAnnotation.DataConverter;
                Debug.Assert(converter.IsBidirectional);

                // It is an error for the <SourceType> of a [DataConverter] annotation to be different than the
                // annotated property's CLR type, modulo nullability on primitives and structs
                if (!expectedType.IsInstanceOf(converter.SourceType)) {
                    var expected = $"converter with source type {property.PropertyType.Name}";
                    var actual = $"converter with source type {converter.SourceType.Name}";
                    throw Error.UserError(context, dpAnnotation, $"expected {expected}, but got {actual}");
                }

                // It is an error for the <ResultType> of a [DataConverter] annotation to be unsupported
                if (!DBType.IsSupported(converter.ResultType)) {
                    var msg = $"converter result type {converter.ResultType.Name} is not supported";
                    throw Error.UserError(context, dpAnnotation, msg);
                }

                // No errors encountered
                state[""] = state[""] with { Converter = converter };
            }
            else if (numeric) {
                // It is an error for a property whose type is not an enumeration to be annotated with [Numeric]
                if (!expectedType.IsEnum) {
                    var msg = $"expected Field of enumeration type, but got {expectedType.Name}";
                    throw Error.UserError(context, new NumericAttribute(), msg);
                }

                // It is an error for a property to be annotated with both [Numeric] and [AsString]
                if (asString) {
                    throw Error.MutuallyExclusive(context, new NumericAttribute(), new AsStringAttribute());
                }

                state[""] = state[""] with { Converter = new EnumToNumericConverter(expectedType).ConverterImpl };
            }
            else if (asString) {
                // It is an error for a property whose type is not an enumeration to be annotated with [AsString]
                if (!expectedType.IsEnum) {
                    var msg = $"expected Field of enumeration type, but got {expectedType.Name}";
                    throw Error.UserError(context, new AsStringAttribute(), msg);
                }

                state[""] = state[""] with { Converter = new EnumToStringConverter(expectedType).ConverterImpl };
            }

            // Fields whose pre-conversion CLR type is an enumeration have an implicitly restricted domain, which
            // therefore means that they have an implicitly restricted image. The valid enumerators of that type are fed
            // through the Field's converter (which may be the identity converter) to produce the final set of viable
            // values. These values may be augmented or filtered by [Check.IsOneOf] and [Check.IsNotOneOf] constraints
            // later. If the converter's result type is not itself an enumeration, or if the back-end database provider
            // does not actually support enumerations, the restricted image will be realized as a CHECK constraint.
            var updated = new HashSet<object>();
            foreach (var enumerator in state[""].Constraints.RestrictedImage) {
                try {
                    updated.Add(state[""].Converter.Convert(enumerator)!);
                }
                catch (Exception ex) {
                    Debug.Assert(dpAnnotation is not null);
                    var msg = $"error converting {enumerator.ForDisplay()}: {ex.Message}";
                    throw Error.UserError(context, dpAnnotation, msg);
                }
            }
            state[""] = state[""] with { Constraints = state[""].Constraints with { RestrictedImage = updated } };
        }

        private static void ProcessDefaults(PropertyInfo property, Dictionary<string, FieldDescriptor> state) {
            Debug.Assert(property is not null);
            Debug.Assert(state is not null);
            HashSet<string> processed = new HashSet<string>();

            foreach (var annotation in property.GetCustomAttributes<DefaultAttribute>()) {
                var context = new PropertyTranslationContext(property, annotation.Path);

                // It is an error for a [Default] attribute to have a null Path
                if (annotation.Path is null) {
                    throw Error.InvalidPath(context, annotation);
                }

                // It is an error for a [Default] attribute to have a non-existent Path
                if (!state.TryGetValue(annotation.Path, out FieldDescriptor target)) {
                    throw Error.InvalidPath(context, annotation);
                }

                // It is an error for the value of a [Default] attribute to be incompatible with the expected data type,
                // including nullability
                var value = annotation.Value.ParseFor(target.Converter.ResultType, target.Nullability == IsNullable.Yes);
                value.MatchNone(reason => throw Error.BadValue(context, annotation, annotation.Value, reason));

                // It is an error for multiple [Default] attributes on a single property to apply to the same Path,
                // though it is legal if a [Default] attribute on an outer aggregate to override that of an inner
                // aggregate
                if (!processed.Add(annotation.Path)) {
                    throw Error.DuplicateAnnotation(context, annotation);
                }

                // No errors encountered
                state[annotation.Path] = target with { Default = value.WithoutException() };
            }
        }

        private static void ProcessPrimaryKeyOptIns(PropertyInfo property, Dictionary<string, FieldDescriptor> state) {
            Debug.Assert(property is not null);
            Debug.Assert(state is not null);

            foreach (var annotation in property.GetCustomAttributes<PrimaryKeyAttribute>()) {
                var context = new PropertyTranslationContext(property, annotation.Path);

                // It is an error for a [PrimaryKey] attribute to have a null Path
                if (annotation.Path is null) {
                    throw Error.InvalidPath(context, annotation);
                }

                // It is an error for a [PrimaryKey] attribute to have a non-existent Path
                if (!state.TryGetValue(annotation.Path, out FieldDescriptor target)) {
                    throw Error.InvalidPath(context, annotation);
                }

                // It is an error for a [PrimaryKey] attribute to be applied to a nullable Field
                if (target.Nullability == IsNullable.Yes) {
                    var msg = "a nullable Field cannot be part of an Entity's primary key";
                    throw Error.UserError(context, annotation, msg);
                }

                // No errors encountered
                state[annotation.Path] = state[annotation.Path] with { InPrimaryKey = true };
            }
        }

        private static void ProcessCandidateKeys(PropertyInfo property, Dictionary<string, FieldDescriptor> state) {
            Debug.Assert(property is not null);
            Debug.Assert(state is not null);

            foreach (var annotation in property.GetCustomAttributes<UniqueAttribute>()) {
                var context = new PropertyTranslationContext(property, annotation.Path);

                // It is an error for a [Unique] attribute to have a null Path
                if (annotation.Path is null) {
                    throw Error.InvalidPath(context, annotation);
                }

                // It is an error for a [Unique] attribute to have a non-existent Path
                if (!state.TryGetValue(annotation.Path, out FieldDescriptor target)) {
                    throw Error.InvalidPath(context, annotation);
                }

                // It is an error for the value specified by a [Unique] attribute to be invalid; currently, the only
                // concrete restriction is that the value is neither null nor the empty string and that a non-anonymous
                // annotation's name does not begin with the reserved prefix
                if (annotation.Name is null || annotation.Name == "") {
                    throw Error.InvalidName(context, annotation, annotation.Name);
                }
                else if (!annotation.IsAnonymous && annotation.Name.StartsWith(UniqueAttribute.ANONYMOUS_PREFIX)) {
                    var msg = $"begins with reserved character sequence \"{UniqueAttribute.ANONYMOUS_PREFIX}\"";
                    throw Error.InvalidName(context, annotation, annotation.Name, msg);
                }

                // No errors encountered
                state[annotation.Path] = target with {
                    CandidateKeyMemberships = new HashSet<string>(target.CandidateKeyMemberships) { annotation.Name }
                };
            }
        }
    }
}
