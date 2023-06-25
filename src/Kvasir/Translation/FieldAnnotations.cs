﻿using Cybele.Extensions;
using Kvasir.Annotations;
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
            var annotation = property.GetCustomAttribute<DataConverterAttribute>();

            if (annotation is not null) {
                var context = new PropertyTranslationContext(property, "");

                // Right now, this is just a Debug.Assert because we only support translation of scalars, but this will
                // have to turn into a proper error check
                Debug.Assert(state.Count == 1 && state.ContainsKey(""));

                // It is an error for a [DataConverter] annotation to have a populated <UserError>
                if (annotation.UserError is not null) {
                    throw Error.UserError(context, annotation, annotation.UserError);
                }

                var converter = annotation.DataConverter;
                Debug.Assert(converter.IsBidirectional);

                // It is an error for the <SourceType> of a [DataConverter] annotation to be different than the
                // annotated property's CLR type, modulo nullability on primitives and structs
                var expectedType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                if (!expectedType.IsInstanceOf(converter.SourceType)) {
                    var expected = $"converter with source type {property.PropertyType.Name}";
                    var actual = $"converter with source type {converter.SourceType.Name}";
                    throw Error.UserError(context, annotation, $"expected {expected}, but got {actual}");
                }

                // It is an error for the <ResultType> of a [DataConverter] annotation to be unsupported
                if (!DBType.IsSupported(converter.ResultType)) {
                    var msg = $"converter result type {converter.ResultType.Name} is not supported";
                    throw Error.UserError(context, annotation, msg);
                }

                // No errors encountered
                state[""] = state[""] with { Converter = converter };
            }
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