using Cybele.Core;
using Cybele.Extensions;
using Kvasir.Annotations;
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
        ///   Translate a single type into a <see cref="TypeDescriptor"/>. The translation takes into account only the
        ///   properties of the type itself and any annotations thereupon; it does not consider annotations on any
        ///   wrapping types.
        /// </summary>
        /// <param name="clr">
        ///   The <see cref="Type"/> to translate.
        /// </param>
        /// <returns>
        ///   A <see cref="TypeDescriptor"/> containing the translation of <paramref name="clr"/>. This translation is
        ///   context-independent, meaning that it is the same regardless of where the type is encountered. For example,
        ///   an Aggregate that has two properties of the same type will only need to translate that type once: any
        ///   annotations on those properties will modify the descriptor after the fact.
        /// </returns>
        /// <exception cref="KvasirException">
        ///   if <paramref name="clr"/> cannot be translated (e.g. there are gaps in the column indices of the type's
        ///   Fields, if any of the properties of the type cannot be translated, etc.).
        /// </exception>
        private TypeDescriptor TranslateType(Type clr) {
            Debug.Assert(clr != typeof(object));
            Debug.Assert(clr != typeof(string));
            Debug.Assert(clr != typeof(DateTime));
            Debug.Assert(clr != typeof(Guid));
            Debug.Assert(!clr.IsAbstract);
            Debug.Assert(!clr.IsEnum);
            Debug.Assert(!clr.IsGenericType);
            Debug.Assert(!clr.IsGenericTypeDefinition);
            Debug.Assert(!clr.IsInterface);
            Debug.Assert(!clr.IsPrimitive);
            Debug.Assert(!clr.IsInstanceOfType(typeof(Delegate)));

            // If the type has already been translated, return the memoized (and already error-checked) result
            if (typeCache_.TryGetValue(clr, out TypeDescriptor result)) {
                return result;
            }

            // If translation of the type is already in progress, then we've detected an impermissible cycle
            if (inProgress_.Contains(clr)) {
                var path = string.Join(" → ", inProgress_.Reverse().Select(t => t.Name)) + $" → {clr.Name}";
                throw Error.UserError(clr, $"reference cycle detected ({path})");
            }
            inProgress_.Push(clr);

            // Generate the "sequences" of Fields, which must appear consecutively in the final column assignment.
            var sequences = new List<IReadOnlyList<FieldDescriptor>>();
            var relations = new Dictionary<string, IRelationDescriptor>();
            foreach (var property in ConstituentPropertiesOf(clr)) {
                var translation = TranslateProperty(property);
                var members = new List<FieldDescriptor>();
                foreach ((var path, var propertyDecriptor) in translation.Fields) {
                    // We have to build up a new path to reflect the property's access mechanics from the
                    // perspective of the type being translated. At a minimum, we need to do this because we may
                    // have multiple scalars with the empty-string path, and they would overwrite each other in the
                    // running dictionary.
                    var nestedPath = path == "" ? property.Name : $"{property.Name}{PATH_SEPARATOR}{path}";
                    members.Add(propertyDecriptor with { AccessPath = nestedPath });
                }
                foreach ((var path, var relationDescriptor) in translation.Relations) {
                    var nestedPath = path == "" ? property.Name : $"{property.Name}{PATH_SEPARATOR}{path}";
                    relations[nestedPath] = relationDescriptor.WithAccessPath(nestedPath);
                }
                if (!members.IsEmpty()) {
                    sequences.Add(members.OrderBy(fd => fd.RelativeColumn).ToList());
                }
            }

            // Solve the columns. This can fail for two reasons: two or more Fields are required to occupy the same
            // column index, or it's not possible to assign all of the Fields to columns without creating gaps while
            // keeping Aggregate groups sequential.
            var ordering = SolveColumns(sequences);
            ordering.MatchNone(reason => throw Error.UserError(clr, reason));
            var columns = ordering.WithoutException().Unwrap();

            // We can't expose a StickyList to the upstream caller, because we might be processing an Aggregate that
            // will be further annotated. Instead, we must present the canonical mapping of paths to FieldDescriptors.
            // Once a type has been translated, every Field has a column, which is fine because the [Column] annotation
            // cannot be applied to a specific nested path. This LINQ query decomposes the StickyList back into a
            // dictionary with new FieldDescriptors containing the appropriate relative column index.
            var fields = columns
                .Select((desc, column) => desc with { AbsoluteColumn = Option.None<int>(), RelativeColumn = column })
                .ToDictionary(desc => desc.AccessPath, desc => desc);

            // It is an error for an Aggregate type to have fewer than 1 Field
            if (clr.IsValueType && fields.Count == 0 && relations.IsEmpty()) {
                throw Error.UserError(clr, $"at least 1 Field required ({fields.Count} found)");
            }

            // Translate all of the [Check.Complex] constraints, which may result in further errors
            var checks = ComplexConstraintsOf(clr).ToList();

            // No errors encountered
            var descriptor = new TypeDescriptor(fields, relations, checks);
            typeCache_.Add(clr, descriptor);
            Debug.Assert(inProgress_.Peek() == clr);
            inProgress_.Pop();
            return descriptor;
        }

        /// <summary>
        ///   Produce a lazily evaluated collection of the properties that make up the data model of an Entity Type.
        /// </summary>
        /// <param name="clr">
        ///   The Entity Type.
        /// </param>
        /// <returns>
        ///   The <see cref="PropertyInfo">properties</see> that comprise the data model of <paramref name="clr"/>, each
        ///   of which will be translated into at least one Field. By default, only public readable instance properties
        ///   that are not inherited are included; however, non-public, static, inherited properties can be included via
        ///   an annotation. Properties annotated as <see cref="CodeOnlyAttribute"><c>[CodeOnly]</c></see> are excluded.
        /// </returns>
        private IEnumerable<PropertyInfo> ConstituentPropertiesOf(Type clr) {
            Debug.Assert(clr is not null);

            var flags =
                BindingFlags.Public | BindingFlags.NonPublic |          // include both public and non-public properties
                BindingFlags.Instance | BindingFlags.Static;            // include both instance and static properties

            // We sort by name, which is guaranteed to be unique, so that there is a guaranteed order in which the
            // properties are processed. This is important for assigning consistent indices to non-annotated properties
            // on repeated calls, which is in turn necessary to properly identify shift.
            foreach (var property in clr.GetProperties(flags).OrderBy(p => p.Name)) {
                var context = new PropertyTranslationContext(property, "");
                var userInclude = property.HasAttribute<IncludeInModelAttribute>();
                var userExclude = property.HasAttribute<CodeOnlyAttribute>();

                var getter = property.GetMethod;        // will be null for write-only properties
                var isIndexer = property.GetIndexParameters().Length > 0;
                var isInherited = getter?.IsInherited() ?? false;
                var systemInclude = getter is not null && !isIndexer && !isInherited && getter.IsPublic && !getter.IsStatic;
                var systemExclude = !systemInclude;

                // It is an error for a property to be annotated with both [IncludeInModel] and [CodeOnly]
                if (userInclude && userExclude) {
                    throw Error.MutuallyExclusive(context, new IncludeInModelAttribute(), new CodeOnlyAttribute());
                }

                // It is an error for a property that is annotated as [IncludeInModel] to be write-only
                if (userInclude && getter is null) {
                    var msg = "write-only properties cannot be included in the data model";
                    throw Error.CannotIncludeInModel(context, msg);
                }

                // It is an error for a property that is annotated as [IncludeInModel] to be an indexer
                if (userInclude && isIndexer) {
                    var msg = "indexers cannot be included in the data model";
                    throw Error.CannotIncludeInModel(context, msg);
                }

                // It is an error for a property whose type is not supported to be annotated as [IncludeInModel]
                CategoryOf(property).MatchNone(s => Error.UnsupportedType(context, s, true));

                // No errors detected
                if (userInclude || (systemInclude && !userExclude)) {
                    yield return property;
                }
            }
        }

        /// <summary>
        ///   Extract the complex constraint generators applied to a single <see cref="Type"/>.
        /// </summary>
        /// <param name="clr">
        ///   The <see cref="Type"/>.
        /// </param>
        /// <returns>
        ///   A collection of generator functions that, when fed with an ordered collection of
        ///   <see cref="IField">Fields</see> and a corresponding ordered collection of
        ///   <see cref="DataConverter">DataConverters</see>, produces a
        ///   <see cref="CheckConstraint"><c>CHECK</c> constraint</see>.
        /// </returns>
        /// <exception cref="KvasirException">
        ///   if any of the <see cref="Check.ComplexAttribute"><c>[Check.Complex]</c></see> annotations applied to
        ///   <paramref name="clr"/> are invalid.
        /// </exception>
        private IEnumerable<ComplexCheckGen> ComplexConstraintsOf(Type clr) {
            Debug.Assert(clr is not null);

            foreach (var annotation in clr.GetCustomAttributes<Check.ComplexAttribute>()) {
                // It is an error for a [Check.Complex] annotation to have a populated <UserError>
                if (annotation.UserError is not null) {
                    throw Error.UserError(clr, annotation, annotation.UserError);
                }

                // It is an error for a [Check.Complex] annotation to have zero constituent Fields
                if (annotation.FieldNames.IsEmpty()) {
                    throw Error.UserError(clr, annotation, "at least 1 Field name required (0 provided)");
                }

                // No errors encountered
                yield return (fs, cs) => {
                    var zip = fs.Zip(cs);
                    var resultFields = new List<IField>();
                    var resultConverters = new List<DataConverter>();

                    foreach (var name in annotation.FieldNames) {
                        var found = zip.FirstOrDefault(pair => pair.First.Name == name);

                        // It is an error for any Field specified by a [Check.Complex] annotation to not exist
                        if (found == default) {
                            var msg = $"Field named \"{name}\" does not exist";
                            var available = $"(available: {string.Join(", ", fs.Select(fs => fs.Name))}";
                            throw Error.UserError(clr, annotation, $"{msg} {available}");
                        }

                        resultFields.Add(found.First);
                        resultConverters.Add(found.Second);
                    }

                    try {
                        var generator = annotation.ConstraintGenerator;
                        return generator.MakeConstraint(resultFields, resultConverters, settings_);
                    }
                    catch (Exception ex) {
                        // It is an error for the creation of the custom constraint to throw an exception
                        var msg = $"unable to create custom constraint ({ex.Message})";
                        throw Error.UserError(clr, annotation, msg);
                    }
                };
            }
        }
    }
}
