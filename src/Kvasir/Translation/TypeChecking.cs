using Cybele.Extensions;
using Kvasir.Exceptions;
using Kvasir.Relations;
using Kvasir.Schema;
using System;
using System.Reflection;

namespace Kvasir.Translation {
    internal sealed partial class Translator {
        /// <summary>
        ///   A representation of the categories of property.
        /// </summary>
        private enum PropertyCategory {
            /// A scalar property, corresponding to exactly one backing Field
            Scalar,

            /// An enumeration property, corresponding to exactly one backing Field with automatic support for an
            /// inclusion <c>CHECK</c> constraint
            Enumeration,

            /// An aggregate property, corresponding to one or more backing Fields
            Aggregate,

            /// A reference property, corresponding to a Foreign Key against another Entity
            Reference,

            /// A relation property, corresponding to another Table altogether
            Relation
        }

        /// <summary>
        ///   Determine the categorization of the Field or Fields backing a property.
        /// </summary>
        /// <exception cref="KvasirException">
        ///   if the <see cref="PropertyInfo.PropertyType">property type</see> of <paramref name="property"/> is
        ///   <see cref="object"/>, <see cref="ValueType"/>, or <see cref="Enum"/>
        ///     --or--
        ///   if the <see cref="PropertyInfo.PropertyType"> property type</see> of<paramref name= "property" /> is an
        ///   interface
        ///     --or--
        ///   if the <see cref="PropertyInfo.PropertyType"> property type</see> of<paramref name= "property" /> is a
        ///   <see cref="Delegate">delegate</see>
        ///     --or--
        ///   if the <see cref="PropertyInfo.PropertyType"> property type</see> of<paramref name= "property" /> is
        ///   <c>dynamic</c>
        ///     --or--
        ///   if the <see cref="PropertyInfo.PropertyType"> property type</see> of<paramref name= "property" /> is from
        ///   an assembly other than that in which the <see cref="Translator"/> was constructed
        ///     --or--
        ///   if the <see cref="PropertyInfo.PropertyType"> property type</see> of<paramref name= "property" /> is
        ///   a closed generic class (open generics are syntactically invalid, and closed generic structs are allowed)
        ///     --or--
        ///   if the <see cref="PropertyInfo.PropertyType"> property type</see> of<paramref name= "property" /> is
        ///   <see langword="abstract"/>
        /// </exception>
        /// <returns>
        ///   The <see cref="PropertyCategory">category</see> of the Field or Fields backing
        ///   <paramref name="property"/>.
        /// </returns>
        private PropertyCategory CategoryOf(PropertyInfo property) {
            // It is an error for a property's type to be 'object' or 'dynamic'
            if (property.PropertyType == typeof(object)) {
                throw new KvasirException(
                    $"Error translating property {property.Name} of type {property.ReflectedType!.Name}: " +
                    $"property has invalid type {property.PropertyType.Name} (or could be dynamic)"
                );
            }

            // It is an error for a property's type to be 'System.ValueType'
            if (property.PropertyType == typeof(ValueType)) {
                throw new KvasirException(
                    $"Error translating property {property.Name} of type {property.ReflectedType!.Name}: " +
                    $"property has invalid type {property.PropertyType.Name}"
                );
            }

            // It is an error for a property's type to be 'System.Enum'
            if (property.PropertyType == typeof(Enum)) {
                throw new KvasirException(
                    $"Error translating property {property.Name} of type {property.ReflectedType!.Name}: " +
                    $"property has invalid type {property.PropertyType.Name}"
                );
            }

            // It is an error for a property's type to come from an external assembly
            if (!DBType.IsSupported(property.PropertyType) && property.PropertyType.Assembly != callingAssembly_) {
                throw new KvasirException(
                    $"Error translating property {property.Name} of type {property.ReflectedType!.Name}: " +
                    $"property has invalid type {property.PropertyType.Name} " +
                    $"(not from assembly '{callingAssembly_.FullName}')"
                );
            }

            // It is an error for a property's type to be an Interface
            if (property.PropertyType.IsInterface) {
                throw new KvasirException(
                    $"Error translating property {property.Name} of type {property.ReflectedType!.Name}: " +
                    $"property has invalid type {property.PropertyType.Name} (an interface)"
                );
            }

            // It is an error for a property's type to be a Delegate
            if (property.PropertyType.IsInstanceOf(typeof(Delegate))) {
                throw new KvasirException(
                    $"Error translating property {property.Name} of type {property.ReflectedType!.Name}: " +
                    $"property has invalid type {property.PropertyType.Name} (a delegate)"
                );
            }

            // It is an error for a property's type to be an abstract class
            if (property.PropertyType.IsClass && property.PropertyType.IsAbstract) {
                throw new KvasirException(
                    $"Error translating property {property.Name} of type {property.ReflectedType!.Name}: " +
                    $"property has invalid type {property.PropertyType.Name} (an abstract class or record class)"
                );
            }

            // It is an error for a property's type to be a closed generic class
            if (property.PropertyType.IsClass && property.PropertyType.IsGenericType) {
                throw new KvasirException(
                    $"Error translating property {property.Name} of type {property.ReflectedType!.Name}: " +
                    $"property has invalid type {property.PropertyType.Name} (a closed generic class or record class)"
                );
            }

            // No errors detected
            if (property.PropertyType.IsEnum) {
                return PropertyCategory.Enumeration;
            }
            else if (property.PropertyType.IsInstanceOf(typeof(IRelation))) {
                return PropertyCategory.Relation;
            }
            else if (DBType.IsSupported(property.PropertyType)) {
                return PropertyCategory.Scalar;
            }
            else if (property.PropertyType.IsClass) {
                #if DEBUG
                CheckEntityType(property.PropertyType);
                #endif
                return PropertyCategory.Reference;
            }
            else {
                return PropertyCategory.Aggregate;
            }
        }

        /// <summary>
        ///   Perform error checking to ensure that a particular Type can be an Entity Type.
        /// </summary>
        /// <param name="entity">
        ///   The source <see cref="Type"/>.
        /// </param>
        /// <exception cref="KvasirException">
        ///   if <paramref name="entity"/> is a primitive type
        ///     --or--
        ///   if <paramref name="entity"/> is an enumeration type
        ///     --or--
        ///   if <paramref name="entity"/> is a struct or record struct type
        ///     --or--
        ///   if <paramref name="entity"/> is an interface
        ///     --or--
        ///   if <paramref name="entity"/> is an open or closed generic type
        ///     --or--
        ///   if <paramref name="entity"/> is an <see langword="abstract"/> type
        /// </exception>
        private static void CheckEntityType(Type entity) {
            // It is an error for an Entity's type to be a primitive
            if (entity.IsPrimitive) {
                throw new KvasirException(
                    $"{entity.Name} cannot be an Entity Type: type is a primitive"
                );
            }

            // It is an error for an Entity's type to be an enumeration
            if (entity.IsEnum) {
                throw new KvasirException(
                    $"{entity.Name} cannot be an Entity Type: type is an enumeration"
                );
            }

            // It is an error for an Entity's type to be a struct or a record struct
            if (entity.IsValueType) {
                throw new KvasirException(
                    $"{entity.Name} cannot be an Entity Type: type is a struct or a record struct"
                );
            }

            // It is an error for an Entity's type to be an Interface
            if (entity.IsInterface) {
                throw new KvasirException(
                    $"{entity.Name} cannot be an Entity Type: type is an interface"
                );
            }

            // It is an error for an Entity's type to be an open generic
            if (entity.IsGenericTypeDefinition) {
                throw new KvasirException(
                    $"{entity.Name} cannot be an Entity Type: type is an open generic"
                );
            }

            // It is an error for an Entity's type to be a closed generic
            if (entity.IsGenericType) {
                throw new KvasirException(
                    $"{entity.Name} cannot be an Entity Type: type is a closed generic"
                );
            }

            // It is an error for an Entity's type to be abstract
            if (entity.IsAbstract) {
                throw new KvasirException(
                    $"{entity.Name} cannot be an Entity Type: type is an abstract class or record class"
                );
            }
        }
    }
}
