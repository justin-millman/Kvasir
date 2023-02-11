using Cybele.Extensions;
using Kvasir.Annotations;
using Kvasir.Exceptions;
using Kvasir.Schema;
using Kvasir.Translation.Extensions;
using System;
using System.Reflection;

namespace Kvasir.Translation {
    internal sealed partial class Translator {
        /// <summary>
        ///   Determine the name of a Field backing a scalar property, using a combination of the property's native CLR
        ///   name and user-provided attributes.
        /// </summary>
        /// <param name="property">
        ///   The source <see cref="PropertyInfo">property</see>.
        /// </param>
        /// <exception cref="KvasirException">
        ///   if <paramref name="property"/> is annotated with multiple <c>[Name]</c> attributes
        ///     --or--
        ///   if the <c>[Name]</c> annotation applied to <paramref name="property"/> has a non-empty
        ///   <see cref="NameAttribute.Path">Path</see>
        ///     --or--
        ///   if the value of the <c>[Name]</c> annotation applied to <paramref name="property"/> is invalid
        ///     --or--
        ///   if the value of the <c>[Name]</c> annotation applied to <paramref name="property"/> is the native name of
        ///   the property.
        /// </exception>
        /// <returns>
        ///   The <see cref="FieldName">name</see> of the Field backing <paramref name="property"/>.
        /// </returns>
        private static FieldName NameOf(PropertyInfo property) {
            var annotation = property.Only<NameAttribute>();

            // If there is no [Name] annotation, then the property's native name is used
            if (annotation is null) {
                return new FieldName(property.Name);
            }

            // It is an error for the [Name] attribute of a scalar property to have a non-empty <Path> value
            if (annotation.Path != "") {
                throw new KvasirException(
                    $"Error translating property {property.Name} of type {property.ReflectedType!.Name}: " +
                    $"path \"{annotation.Path}\" of [Name] annotation does not exist"
                );
            }

            // It is an error for the value of a [Name] annotation to be invalid as the name of a Field; currently, the
            // only restriction on this is that the name must have non-zero length (there is no back-end awareness)
            if (annotation.Name == "") {
                throw new KvasirException(
                    $"Error translating property {property.Name} of type {property.ReflectedType!.Name}: " +
                    $"[Name] \"{annotation.Name}\" is not a valid Field name"
                );
            }

            // It is an error for the value of the [Name] annotation to be the property's native name
            if (annotation.Name == property.Name) {
                throw new KvasirException(
                    $"Error translating property {property.Name} of type {property.ReflectedType!.Name}: " +
                    $"[Name] annotation is redundant (Field would have been named '{annotation.Name}' anyway)"
                );
            }

            // No errors detected
            return new FieldName(annotation.Name);
        }

        /// <summary>
        ///   Determine the name of the primary Table backing an Entity type.
        /// </summary>
        /// <param name="entity">
        ///   The source <see cref="Type">Entity type</see>.
        /// </param>
        /// <exception cref="KvasirException">
        ///   if <paramref name="entity"/> is annotated with both <c>[Table]</c> or <c>[ExcludeNamespaceFromName]</c>
        ///     --or--
        ///   if the value of the <c>[Table]</c> annotation applied to <paramref name="entity"/> is invalid
        ///     --or--
        ///   if the value of the <c>[Table]</c> annotation applied to <paramref name="entity"/> is the same as the name
        ///   of the Table that would have otherwise been ascertained.
        /// </exception>
        /// <returns>
        ///   The <see cref="TableName">name</see> of the primary Table backing <paramref name="entity"/>.
        /// </returns>
        private static TableName NameOf(Type entity) {
            var annotation = entity.GetCustomAttribute<TableAttribute>();
            var excludeNamespace = entity.HasAttribute<ExcludeNamespaceFromNameAttribute>();

            // It is an error for a type to be annotated with both [Table] and [ExcludeNamespaceFromName]
            if (annotation is not null && excludeNamespace) {
                throw new KvasirException(
                    $"Error translating Entity Type {entity.Name}: " +
                    "type is annotated as with both [ExcludeNamespaceFromName] and [Table]"
                );
            }

            // If there is no [Table] annotation, the type's native namespace-qualified name is used; the namespace is
            // dropped if an [ExcludeNamespaceFromName] annotation is present (though nestedness identifiers will still
            // be present)
            if (annotation is null) {
                return new TableName((excludeNamespace ? entity.Name : entity.FullName!) + "Table");
            }

            // It is an error for the value of a [Table] annotation to be invalid as the name of a Table; currently, the
            // only restriction on this is that the name must have non-zero length (there is no back-end awareness)
            if (annotation.Name == "") {
                throw new KvasirException(
                    $"Error translating Entity Type {entity.Name}: " +
                    $"[Table] name \"{annotation.Name}\" is not a valid Table name"
                );
            }

            // It is an error for the value of a [Table] annotation to be the same as what the Entity Type's primary
            // table would have been in the absence of the annotation
            if (annotation.Name == entity.FullName! + "Table") {
                throw new KvasirException(
                    $"Error translating Entity Type {entity.Name}: " +
                    $"[Table] annotation is redundant (Table would have been named '{annotation.Name}' anyway)"
                );
            }

            // No errors detected
            return new TableName(annotation.Name);
        }
    }
}
