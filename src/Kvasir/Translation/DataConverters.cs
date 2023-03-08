using Cybele.Core;
using Cybele.Extensions;
using Kvasir.Annotations;
using Kvasir.Exceptions;
using Kvasir.Schema;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Kvasir.Translation {
    internal sealed partial class Translator {
        /// <summary>
        ///   Determine the <see cref="DataConverter"/> with which extrinsic transformations for a Field backing a
        ///   scalar property are to be performed.
        /// </summary>
        /// <param name="property">
        ///   The source <see cref="PropertyInfo">property</see>.
        /// </param>
        /// <exception cref="KvasirException">
        ///   if <paramref name="property"/> is annotated with multiple <c>[DataConverter]</c> attributes
        ///     --or--
        ///   if the <c>[DataConverter]</c> annotation applied to <paramref name="property"/> has a non-empty
        ///   <see cref="DataConverterAttribute.UserError">user error</see>
        ///     --or--
        ///   if the value of the <c>[DataConverter]</c> annotation applied to <paramref name="property"/> has a
        ///   <see cref="DataConverter.SourceType">SourceType</see> that is not the same as the CLR type of
        ///   <paramref name="property"/>, with nullability stripped
        ///     --or--
        ///   if the value of <c>[DataConverter]</c> annotation applied to <paramref name="property"/> has a
        ///   <see cref="DataConverter.ResultType">ResultType</see> that is not supported.
        /// </exception>
        /// <returns>
        ///   The <see cref="DataConverter"/> to be used for extrinsic transformations for the Field backing
        ///   <paramref name="property"/>, which may be the
        ///   <see cref="DataConverter.Identity">identity conversion</see>.
        /// </returns>
        private static DataConverter ConverterFor(PropertyInfo property) {
            // It is an error for a property to be annotated with multiple [Name] attributes
            var annotations = property.GetCustomAttributes<DataConverterAttribute>();
            if (annotations.Count() > 1) {
                throw new KvasirException(
                    $"Error translating property {property.Name} of type {property.ReflectedType!.Name}: " +
                    "multiple [DataConverter] annotations encountered"
                );
            }
            var annotation = annotations.FirstOrDefault();

            // If there is no [DataConverter] annotation, then an identity conversion is used
            if (annotation is null) {
                return DataConverter.Identity(property.PropertyType);
            }

            // It is an error for the [DataConverter] attribute of a scalar property to have a populated <UserError>
            if (annotation.UserError is not null) {
                throw new KvasirException(
                    $"Error translating property {property.Name} of type {property.ReflectedType!.Name}: " +
                    $"data provided to [DataConverter] is invalid ({annotation.UserError})"
                );
            }

            // The IDataConverter interface required by the [DataConverter] attribute requires that classes implement
            // both the Convert and Revert APIs, producing a bidirectional converter
            var converter = annotation.DataConverter;
            Debug.Assert(converter.IsBidirectional);

            // It is an error for the <SourceType> of the value of a [DataConverter] annotation to be different than the
            // property's CLR type, modulo nullability for primitives and structs; by using IsInstanceOf, we are able
            // to support the desirable argument variance
            var expectedType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
            if (!expectedType.IsInstanceOf(converter.SourceType)) {
                throw new KvasirException(
                    $"Error translating property {property.Name} of type {property.ReflectedType!.Name}: " +
                    $"[DataConverter] annotation operates on {converter.SourceType.Name} for " +
                    $"Field of type {expectedType.Name}"
                );
            }

            // It is an error for the <ResultType> of the value of a [DataConveter] annotation to be an unsupported type
            if (!DBType.IsSupported(converter.ResultType)) {
                throw new KvasirException(
                    $"Error translating property {property.Name} of type {property.ReflectedType!.Name}: " +
                    $"result type {converter.ResultType.Name} of [DataConverter] annotation is not supported"
                );
            }

            // No errors detected
            return converter;
        }
    }
}
