using Cybele.Extensions;
using Kvasir.Exceptions;
using Kvasir.Schema;
using System;
using System.Diagnostics;
using System.Reflection;

namespace Kvasir.Translation.Extensions {
    internal static partial class TranslationExtensions {
        /// <summary>
        ///   Treat untyped, possibly <see langword="null"/> value as if it were valid value for a Field with a specific
        ///   set of traits.
        /// </summary>
        /// <param name="self">
        ///   The <see cref="object"/> on which the extension method is invoked.
        /// </param>
        /// <param name="property">
        ///   The <see cref="PropertyInfo">source property</see> of the backing Field. This argument is used only to
        ///   produce a contextualized error message and does not contribute to type analysis.
        /// </param>
        /// <param name="into">
        ///   The <see cref="Type"/> into which <paramref name="self"/> is to be coerced.
        /// </param>
        /// <param name="nullability">
        ///   The nullability of the target Field, which determines whether or not <paramref name="self"/> can validly
        ///   be <see langword="null"/>.
        /// </param>
        /// <param name="flavor">
        ///   A brief description of what the value is for, used in the contextualized error message.
        /// </param>
        /// <returns>
        ///   <paramref name="self"/>, having been translated and/or parsed as necessary.
        /// </returns>
        /// <exception cref="KvasirException">
        ///   if <paramref name="self"/> is <see langword="null"/> (or <see cref="DBNull"/>) but
        ///   <paramref name="nullability"/> is <see cref="IsNullable.No"/>
        ///     --or--
        ///   if <paramref name="self"/> is an array, even if its single value would otherwise be valid
        ///     --or--
        ///   if <paramref name="into"/> is either <see cref="DateTime"/> or <see cref="Guid"/> but
        ///   <paramref name="self"/> is neither <see langword="null"/> (nor <see cref="DBNull"/>) nor a string
        ///     --or--
        ///   if <paramref name="into"/> is neither <see cref="DateTime"/> nor <see cref="Guid"/> and
        ///   <paramref name="self"/> is neither <see langword="null"/> (nor <see cref="DBNull"/>) nor an instance of
        ///   <paramref name="into"/>; note that implicit conversions and numeric promotions are not permitted
        ///     --or--
        ///   if <paramref name="into"/> is <see cref="DateTime"/> and <paramref name="self"/> is a string that cannot
        ///   be parsed thereas
        ///     --or--
        ///   if <paramref name="into"/> is <see cref="Guid"/> and <paramref name="self"/> is a string that cannot be
        ///   parsed thereas.
        /// </exception>
        public static object? ParseFor(this object? self, PropertyInfo property, Type into, IsNullable nullability,
            string flavor) {

            into = Nullable.GetUnderlyingType(into) ?? into;
            Debug.Assert(DBType.IsSupported(into));

            // It is an error for an annotation value of 'null' to be provided for a non-nullable Field
            if ((self is null || self == DBNull.Value) && nullability == IsNullable.No) {
                throw new KvasirException(
                    $"Error translating property {property.Name} of type {property.ReflectedType!.Name}: " +
                    $"{flavor} of 'null' is not valid for a non-nullable Field"
                );
            }

            // If the value is 'null' then the remaining checks are not necessary
            if (self is null || self == DBNull.Value) {
                return null;
            }

            // It is an error for an annotation value to be an array
            if (self.GetType().IsArray) {
                throw new KvasirException(
                    $"Error translating property {property.Name} of type {property.ReflectedType!.Name}: " +
                    $"{flavor} cannot be an array"
                );
            }

            // It is an error for the type of an annotation value to be different than the pre-conversion CLR type of
            // the annotated property; conversions are performed on such values when necessary. For a property whose
            // pre-conversion CLR type is either DateTime or Guid, annotation values must be strings (which will be
            // parsed later).
            if (into == typeof(DateTime) || into == typeof(Guid)) {
                if (self.GetType() != typeof(string)) {
                    throw new KvasirException(
                        $"Error translating property {property.Name} of type {property.ReflectedType!.Name}: " +
                        $"{flavor} of {self.ForDisplay()} (of type {self.GetType().Name}) " +
                        $"is not valid on a property of type {into.Name} " +
                        "(a string is required, which will then be parsed)"
                    );
                }
            }
            else if (!into.IsInstanceOfType(self)) {
                throw new KvasirException(
                    $"Error translating property {property.Name} of type {property.ReflectedType!.Name}: " +
                    $"{flavor} of {self.ForDisplay()} (of type {self.GetType().Name}) " +
                    $"is not valid on a property of type {into.Name} "
                );
            }

            // Parse value if necessary
            if (into == typeof(DateTime)) {
                if (!DateTime.TryParse((string)self, out DateTime result)) {
                    throw new KvasirException(
                        $"Error translating property {property.Name} of type {property.ReflectedType!.Name}: " +
                        $"could not parse {flavor} \"{self}\" into {nameof(DateTime)}"
                    );
                }
                return result;
            }
            else if (into == typeof(Guid)) {
                if (!Guid.TryParse((string)self, out Guid result)) {
                    throw new KvasirException(
                        $"Error translating property {property.Name} of type {property.ReflectedType!.Name}: " +
                        $"could not parse {flavor} \"{self}\" into {nameof(Guid)}"
                    );
                }
                return result;
            }
            else {
                return self;
            }
        }
    }
}
