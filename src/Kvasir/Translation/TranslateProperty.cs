using Cybele.Extensions;
using Kvasir.Exceptions;
using Kvasir.Relations;
using Kvasir.Schema;
using Kvasir.Translation.Synthetic;
using Optional;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Kvasir.Translation {
    internal sealed partial class Translator {
        /// <summary>
        ///   The five distinct categories of CLR property.
        /// </summary>
        private enum PropertyCategory {
            /// <summary>A scalar property, corresponding to exactly one backing Field</summary>
            Scalar,

            /// <summary>An enumeration property, corresponding to exactly one backing Field whose value is implicitly
            /// restricted to one of a discrete set of options</summary>
            Enumeration,

            /// <summary>An aggregate property, corresponding to one or more backing Fields</summary>
            Aggregate,

            /// <summary>A reference property, corresponding to a Foreign Key against another Entity</summary>
            Reference,

            /// <summary>A relation property, corresponding to another Table altogether</summary>
            Relation
        }

        /// <summary>
        ///   Translate a single property into one or more intermediate Field descriptions. The translation takes into
        ///   account only the property itself and any annotations thereupon; it does not consider annotations on the
        ///   wrapping type.
        /// </summary>
        /// <param name="property">
        ///   The <see cref="PropertyInfo">property</see> to translate.
        /// </param>
        /// <returns>
        ///   A mapping of "access paths" to the <see cref="FieldDescriptor"/> of the property at that path. An "access
        ///   path" is a dot-separated sequence of property names used to access a particular value. For example: an
        ///   "access path" key of <c>A.B</c> means that the property is some aggregate type that itself has a property
        ///   named <c>A</c>, and <i>that</i> property itself has a property named <c>B</c> that is the ultimate source
        ///   of the Field. An empty string for the "access path" indicates a scalar property.
        /// </returns>
        /// <exception cref="KvasirException">
        ///   if <paramref name="property"/> cannot be translated (e.g. its CLR type is not supported, it has an invalid
        ///   annotation, etc.).
        /// </exception>
        private TranslationState TranslateProperty(PropertyInfo property) {
            Debug.Assert(property is not null);

            return CategoryOf(property).Match(
                none: s => throw Error.UnsupportedType(new PropertyTranslationContext(property, ""), s, false),
                some: c => c switch {
                    PropertyCategory.Scalar => ApplyAnnotations(property, ScalarBaseTranslation(property)),
                    PropertyCategory.Enumeration => ApplyAnnotations(property, EnumBaseTranslation(property)),
                    PropertyCategory.Aggregate => ApplyAnnotations(property, AggregateBaseTranslation(property)),
                    PropertyCategory.Reference => ApplyAnnotations(property, ReferenceBaseTranslation(property)),
                    PropertyCategory.Relation => ApplyAnnotations(property, RelationBaseTranslation(property)),

                    _ => throw new ApplicationException("switch statement exhausted")
                }
            );
        }

        /// <summary>
        ///   Determine the <see cref="PropertyCategory"/> of a <see cref="PropertyInfo">property</see>, which informs
        ///   the manner in which the property is translated.
        /// </summary>
        /// <param name="property">
        ///   The <see cref="PropertyInfo">property</see>.
        /// </param>
        /// <returns>
        ///   The <see cref="PropertyCategory"/> according to which <paramref name="property"/> is to be translated.
        /// </returns>
        /// <returns>
        ///   A <c>SOME</c> instance carrying the category of <paramref name="property"/> if its type is supported by
        ///   Kvasir; otherwise, a <c>NONE</c> instance carrying an explanation as to why it is not.
        /// </returns>
        private Option<PropertyCategory, string> CategoryOf(PropertyInfo property) {
            Debug.Assert(property is not null);
            var propertyType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

            // It is an error for a property's CLR type to be `object` or `dynamic`
            if (propertyType == typeof(object)) {
                var msg = $"{nameof(Object)} (or possibly dynamic)";
                return Option.None<PropertyCategory, string>(msg);
            }

            // It is an error for a property's CLR type to be `System.ValueType` or `System.Enum`
            if (propertyType == typeof(ValueType) || propertyType == typeof(Enum)) {
                var msg = "a universal base class";
                return Option.None<PropertyCategory, string>(msg);
            }

            // Implementations of the Kvasir.Relations.IRelation interface that aren't the interface itself are valid.
            // This check must come before the "external assembly," "interface," and "closed generic" checks.
            if (propertyType.IsInstanceOf(typeof(IRelation))) {
                if (propertyType == typeof(IRelation)) {
                    var msg = "the IRelation interface";
                    return Option.None<PropertyCategory, string>(msg);
                }

                var mostRecent = inProgress_.First(t => t.IsClass);
                if (mostRecent.GetType() == typeof(SyntheticType)) {
                    var msg = $"nested within {mostRecent.Name}";
                    return Option.None<PropertyCategory, string>(msg);
                }
                return Option.Some<PropertyCategory, string>(PropertyCategory.Relation);
            }

            // It is an error for a property's CLR type to come from an external assembly; the only exceptions are the
            // types from the C# standard library that are supported as Scalars or Enumerations
            if (!DBType.IsSupported(propertyType) && propertyType.Assembly != sourceAssembly_) {
                var externalAssembly = propertyType.Assembly.FullName!;
                var msg = $"from an external assembly \"{externalAssembly}\"; expected \"{sourceAssembly_.FullName!}\"";
                return Option.None<PropertyCategory, string>(msg);
            }

            // It is an error for a property's CLR type to be an Interface
            if (propertyType.IsInterface) {
                var msg = "an interface";
                return Option.None<PropertyCategory, string>(msg);
            }

            // It is an error for a property's CLR type to be a Delegate
            if (propertyType.IsInstanceOf(typeof(Delegate))) {
                var msg = "a delegate";
                return Option.None<PropertyCategory, string>(msg);
            }

            // It is an error for a property's CLR type to be abstract
            if (propertyType.IsAbstract) {
                var msg = "an abstract class or an abstract record class";
                return Option.None<PropertyCategory, string>(msg);
            }

            // It is an error for a property's CLR type to be a closed generic class
            if (propertyType.IsClass && propertyType.IsGenericType) {
                var msg = "a generic class or a generic record class";
                return Option.None<PropertyCategory, string>(msg);
            }

            // No errors detected (Relation-type properties already detected)
            if (propertyType.IsEnum) {
                return Option.Some<PropertyCategory, string>(PropertyCategory.Enumeration);
            }
            else if (DBType.IsSupported(propertyType)) {
                return Option.Some<PropertyCategory, string>(PropertyCategory.Scalar);
            }
            else if (propertyType.IsClass) {
                var context = new PropertyTranslationContext(property, "");
                EntityTypeCheck(property.PropertyType).MatchSome(s => throw Error.UnsupportedType(context, s, false));
                return Option.Some<PropertyCategory, string>(PropertyCategory.Reference);
            }
            else {
                return Option.Some<PropertyCategory, string>(PropertyCategory.Aggregate);
            }
        }
    }
}
