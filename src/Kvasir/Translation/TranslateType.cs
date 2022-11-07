using Cybele.Collections;
using Cybele.Extensions;
using Kvasir.Annotations;
using Kvasir.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Kvasir.Translation {
    internal sealed partial class Translator {
        /// <summary>
        ///   Translate a single CLR Type, which may or may not be an Entity Type.
        /// </summary>
        /// <remarks>
        ///   Very few assumptions about the semantic of the source Type are made. In particular, this function does not
        ///   create bona fide schema structures (e.g. Candidate Key, Primary Keys, etc.), nor does it enforce any
        ///   requirements about Fields (e.g. minimum of two, all unique names, etc.). This makes the function suitable
        ///   for Translating Entity Types and also for Translating Aggregate Types and Synthetic Types, where the
        ///   result is one piece of a larger Table whose final form will be impacted by other Translations.
        /// </remarks>
        /// <param name="clr">
        ///   The source <see cref="Type">CLR Type</see>.
        /// </param>
        /// <exception cref="KvasirException">
        ///   if the annotations of the Fields in the Translation of <paramref name="clr"/> leave gaps in the assigned
        ///   or implied column indices.
        /// </exception>
        /// <returns>
        ///   The <see cref="TypeDescriptor"/> of the back-end schema for <paramref name="clr"/>.
        /// </returns>
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
            Debug.Assert(!clr.IsInstanceOf(typeof(Delegate)));

            // If we've already translated the Type once, then simply return the memoized TypeDescriptor; the upstream
            // caller may modify the result based on property-level annotations
            if (typeCache_.TryGetValue(clr, out TypeDescriptor result)) {
                return result;
            }

            var fields = new StickyList<FieldDescriptor>();
            var relations = new List<TypeDescriptor>();
            foreach (var property in ModelPropertiesOf(clr)) {
                var translation = TranslateProperty(property);
                RecordFields(translation.Fields, fields);
            }

            // It is an error for any type (Entity Type, Aggregate Type, Synthetic Type, etc.) to have gaps in its
            // ordered Fields
            if (fields.HasGaps) {
                var gaps = Enumerable.Range(0, fields.LargestIndex).Select(idx => !fields.IsOccupied(idx));
                throw new KvasirException(
                    $"Error translating type {clr.Name}: " +
                    "explicitly specified [Column] indices are non-consecutive, leaving gaps"
                );
            }

            var checks = new List<Check.ComplexAttribute>();
            var descriptor = new TypeDescriptor(CLRType: clr, Fields: fields);
            typeCache_.Add(clr, descriptor);
            return descriptor;
        }

        /// <summary>
        ///   Generate the set of top-level properties of a CLR Type that are part of its back-end data model.
        /// </summary>
        /// <param name="clr">
        ///   The source <see cref="Type">CLR Type</see>.
        /// </param>
        /// <exception cref="KvasirException">
        ///   if any property of <paramref name="clr"/> is annotated as both <c>[IncludeInModel]</c> and
        ///   <c>[CodeOnly</c>
        ///     --or--
        ///   if any property of <paramref name="clr"/> that is read-only, an indexer, or is inherited (from either a
        ///   base class or an interface) is annotated as either <c>[IncludeInModel]</c> or <c>[CodeOnly]</c>
        ///     --or--
        ///   if any public, readable, instance property of <paramref name="clr"/> is annotated as
        ///   <c>[IncludeInModel]</c>
        ///     --or--
        ///   if any property that is either non-public, <see langword="static"/> or an indexer is annotated as
        ///   <c>[CodeOnly]</c>.
        /// </exception>
        /// <returns>
        ///   A finite enumerable of <see cref="PropertyInfo">properties</see> that are part of the back-end data model
        ///   for <paramref name="clr"/>, either in its Principal Table or as a Relation. The order of the properties is
        ///   undefined but is guaranteed to be the same on consecutive invocations for the exact same CLR Type.
        /// </returns>
        private static IEnumerable<PropertyInfo> ModelPropertiesOf(Type clr) {
            var flags =
                BindingFlags.Public | BindingFlags.NonPublic |      // include both public and non-public properties
                BindingFlags.Static | BindingFlags.Instance;        // include both static and instance properties

            foreach (var property in clr.GetProperties(flags).OrderBy(p => p.Name)) {
                var userInclude = property.HasAttribute<IncludeInModelAttribute>();
                var userExclude = property.HasAttribute<CodeOnlyAttribute>();

                var getter = property.GetMethod;
                var indexer = property.GetIndexParameters().Length > 0;
                var inherited = getter is not null && (property.DeclaringType != clr || getter.IsInherited());
                var systemInclude = getter is not null && !indexer && !inherited && getter.IsPublic && !getter.IsStatic;
                var systemExclude = !systemInclude;

                // It is an error for a property to be annotated as both [IncludeInModel] and [CodeOnly]
                if (userInclude && userExclude) {
                    throw new KvasirException(
                        $"Error translating property {property.Name} of type {property.ReflectedType!.Name}: " +
                        "property is annotated with both [IncludeInModel] and [CodeOnly]"
                    );
                }

                // It is an error for a property that is annotated as [IncludeInModel] to be write-only
                if (userInclude && getter is null) {
                    throw new KvasirException(
                        $"Error translating property {property.Name} of type {property.ReflectedType!.Name}: " +
                        "a write-only property cannot be annotated as [IncludeInModel]"
                    );
                }

                // It is an error for a property that is annotated as [IncludeInModel] to be an indexer
                if (userInclude && indexer) {
                    throw new KvasirException(
                        $"Error translating property {property.Name} of type {property.ReflectedType!.Name}: " +
                        "an indexer property cannot be annotated as [IncludeInModel]"
                    );
                }

                // It is an error for a property that is annotated as [IncludeInModel] to have been inherited from
                // either a base class or an interface
                if (userInclude && inherited) {
                    throw new KvasirException(
                        $"Error translating property {property.Name} of type {property.ReflectedType!.Name}: " +
                        "an inherited property cannot be annotated as [IncludeInModel] " +
                        "(the property was inherited - declared by a base class or an interface)"
                    );
                }

                // It is an error for a readable property that is annotated as [IncludeInModel] to be both a public
                // property and an instance property, as the annotation is then redundant
                if (userInclude && systemInclude) {
                    throw new KvasirException(
                        $"Error translating property {property.Name} of type {property.ReflectedType!.Name}: " +
                        "[IncludeInModel] annotation is redundant (property is public and non-static)"
                    );
                }

                // It is an error for a property that is annotated as [CodeOnly] to be inherited from either a base
                // class or an interface, as the annotation is then redundant
                if (userExclude && inherited) {
                    throw new KvasirException(
                        $"Error translating property {property.Name} of type {property.ReflectedType!.Name}: " +
                        "[CodeOnly] annotation is redundant " +
                        "(the property was inherited - declared by a base class or an interface)"
                    );
                }

                // It is an error for a property that is annotated as [CodeOnly] to be read-only, non-public, static, or
                // an indexer, as the annotation is then redundant
                if (userExclude && systemExclude) {
                    throw new KvasirException(
                        $"Error translating property {property.Name} of type {property.ReflectedType!.Name}: " +
                        "[CodeOnly] annotation is redundant (property is write-only, an indexer, non-public, or static)"
                    );
                }

                // No errors detected, and the property is either annotated as [IncludeInModel] or is a property that is
                // included by default and is not annotated as [CodeOnly]
                if (userInclude || (systemInclude && !userExclude)) {
                    yield return property;
                }
            }
        }

        /// <summary>
        ///   Place the Field Descriptors generated during the Translation of a property into a running list tracking
        ///   the Fields' ultimate columnar positions.
        /// </summary>
        /// <param name="descriptors">
        ///   The <see cref="FieldDescriptor">FieldDescriptors</see>.
        /// </param>
        /// <param name="into">
        ///   The running list, using stickiness to indicate a position required by an annotation.
        /// </param>
        /// <exception cref="KvasirException">
        ///   if any of the element of <paramref name="descriptors"/> has a specified column index that is already
        ///   occupied in <paramref name="into"/> by a "sticky" item.
        /// </exception>
        private static void RecordFields(IEnumerable<FieldDescriptor> descriptors, StickyList<FieldDescriptor> into) {
            foreach (var descriptor in descriptors) {
                if (!descriptor.Column.HasValue) {
                    into.Add(descriptor);
                    continue;
                }
                var column = descriptor.Column.Unwrap();

                // It is an error for two or more Fields to be explicitly assigned the same column index
                Debug.Assert(column >= 0);
                if (column <= into.LargestIndex && into.IsOccupied(column) && into.IsSticky(column)) {
                    throw new KvasirException(
                        $"Error translating property {descriptor.AccessPath} of type {descriptor.SourceType.Name}: " +
                        $"[Column] index {column} is already occupied"
                    );
                }
                into.Insert(column, descriptor);
            }
        }
    }
}
