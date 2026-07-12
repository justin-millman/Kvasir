using Kvasir.Annotations;
using Kvasir.Localization;
using Kvasir.Schema;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Kvasir.Translation {
    /// <summary>
    ///   The metadata describing the translation of a Localization type.
    /// </summary>
    ///
    /// <param name="RepresentativeProperty">The property representing the Localization in a Principal Table.</param>
    /// <param name="KeyProperty">The property defining the Localization Key.</param>
    /// <param name="LocaleProperty">The property defining the Locale.</param>
    /// <param name="ValueProperty">The property defining the Value.</param>
    /// <param name="ValueType">The type of the Localization Value.</param>
    /// <param name="IsKeyNullable"><see langword="true"/> if the Localization Key is nullable; otherwise, <see langword="false"/>.</param>
    /// <param name="IsLocaleNullable"><see langword="true"/> if the Localization Locale is nullable; otherwise, <see langword="false"/>.</param>
    /// <param name="IsValueNullable"><see langword="true"/> if the Localization Value is nullable; otherwise, <see langword="false"/>.</param>
    /// <param name="FirstDerivedProperty">The first derived property that is included in the data model, if any.</param>
    internal readonly record struct LocalizationMetadata(
        PropertyInfo RepresentativeProperty,
        PropertyInfo KeyProperty,
        PropertyInfo LocaleProperty,
        PropertyInfo ValueProperty,
        Type ValueType,
        bool IsKeyNullable,
        bool IsLocaleNullable,
        bool IsValueNullable,
        PropertyInfo? FirstDerivedProperty
    );

    /// <summary>
    ///   A collection of helper functions for performing translation operations on Localization types.
    /// </summary>
    internal static class LocalizationHelper {
        /// <summary>
        ///   Reflects over a Localization Type, extracting the important properties, types, and nullability statuses
        ///   statuses of its Localization descriptors.
        /// </summary>
        /// <param name="source">
        ///   The Localization Type.
        /// </param>
        /// <returns>
        ///   The <see cref="LocalizationMetadata"/> for <paramref name="source"/>.
        /// </returns>
        public static LocalizationMetadata GetMetadataFor(Type source) {
            Debug.Assert(Translator.IsLocalizationType(source));

            var declaringType = source.IsGenericType ? source.GetGenericTypeDefinition() : source;
            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy;
            var properties = source.GetProperties(flags);
            var baseProperties = properties.Where(p => p.DeclaringType!.IsGenericType && p.DeclaringType.GetGenericTypeDefinition() == typeof(Localization<,,>));
            var derivedProperties = properties.Where(p => !baseProperties.Contains(p) && p.TranslationCategory().Equals(PropertyCategory.InDataModel));

            // The `Key` and `Relation` properties are directly on the base Localization<> type, so we need to make sure
            // to pull them from there to avoid picking up derived properties. The `Relation` type has a bunch of
            // properties inherited from the base collection interfaces; the one at index [5] is the `ConnectionType`,
            // which holds the key-value pair from which we can extract the `Locale` and the `Value`.
            var keyProperty = baseProperties.Where(p => p.Name == "Key").First();
            var relationProperty = baseProperties.Where(p => p.Name == "Relation").First();
            var relationType = relationProperty.PropertyType;
            var connectionType = (Type)relationType.GetProperties(flags)[5]!.GetValue(null)!;
            var localeProperty = connectionType.GetProperty("Key", flags)!;
            var valueProperty = connectionType.GetProperty("Value", flags)!;

            var keyNullabilityInfo = new NullabilityInfoContext().Create(keyProperty);
            var relationNullabilityInfo = new NullabilityInfoContext().Create(relationProperty);

            var isKeyNullable = keyNullabilityInfo.ReadState == NullabilityState.Nullable;
            var isLocaleNullable = relationNullabilityInfo.GenericTypeArguments[0].ReadState == NullabilityState.Nullable;
            var isValueNullable = relationNullabilityInfo.GenericTypeArguments[1].ReadState == NullabilityState.Nullable;

            var keyLocaleAttributes = new Attribute[] { new PrimaryKeyAttribute(), new NonNullableAttribute() };
            var valueAttributes = new Attribute[] { isValueNullable ? new NullableAttribute() : new NonNullableAttribute() };

            return new LocalizationMetadata() {
                RepresentativeProperty = keyProperty,
                KeyProperty = new SyntheticPropertyInfo("Key", declaringType!, keyProperty.PropertyType, keyLocaleAttributes),
                LocaleProperty = new SyntheticPropertyInfo("Locale", declaringType!, localeProperty.PropertyType, keyLocaleAttributes),
                ValueProperty = new SyntheticPropertyInfo("Value", declaringType!, valueProperty.PropertyType, valueAttributes),
                ValueType = valueProperty.PropertyType,
                IsKeyNullable = keyNullabilityInfo.ReadState == NullabilityState.Nullable,
                IsLocaleNullable = relationNullabilityInfo.GenericTypeArguments[0].ReadState == NullabilityState.Nullable,
                IsValueNullable = relationNullabilityInfo.GenericTypeArguments[1].ReadState == NullabilityState.Nullable,
                FirstDerivedProperty = derivedProperties.OrderBy(p => p.Name).FirstOrDefault()
            };
        }

        /// <summary>
        ///   Performs error checking on a Localization, as represented by its
        ///   <see cref="LocalizationMetadata">metadata</see>.
        /// </summary>
        /// <param name="metadata">
        ///   The <see cref="LocalizationMetadata"/> to error check.
        /// </param>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the error checking is being performed, for reporting purposes.
        /// </param>
        /// <exception cref="InvalidNativeNullabilityException">
        ///   if the <see cref="LocalizationMetadata.IsKeyNullable"/> flag or the
        ///   <see cref="LocalizationMetadata.IsLocaleNullable"/> of <paramref name="metadata"/> is
        ///   <see langword="true"/>.
        /// </exception>
        /// <exception cref="InvalidLocalizationKeyException">
        ///   if the type of the <see cref="LocalizationMetadata.KeyProperty"/> of <paramref name="metadata"/> is not
        ///   a type that is natively supported by Kvasir.
        /// </exception>
        /// <exception cref="InvalidPropertyInDataModelException">
        ///   if the <see cref="LocalizationMetadata.FirstDerivedProperty"/> of <paramref name="metadata"/> is not
        ///   <see langword="null"/>.
        /// </exception>
        public static void ErrorCheck(LocalizationMetadata metadata, Context context) {
            // Deal with any extra properties defined on the derived type: none are permitted.
            if (metadata.FirstDerivedProperty is not null) {
                using var guard = context.Push(metadata.FirstDerivedProperty);
                throw new InvalidPropertyInDataModelException(context, new DerivedLocalizationTag());
            }

            // Deal with the Key. It cannot be nullable, and it must be of a type natively supported by Kvasir.
            {
                using var guard = context.Push(metadata.KeyProperty);

                if (metadata.IsKeyNullable) {
                    throw new InvalidNativeNullabilityException(context, "the Localization Key type of a Localization");
                }
                else if (!DBType.IsSupported(metadata.KeyProperty.PropertyType)) {
                    var category = metadata.KeyProperty.PropertyType.TranslationCategory();
                    throw new InvalidLocalizationKeyException(context, metadata.KeyProperty.PropertyType, category);
                }
            }

            // Deal with the Locale. It cannot be nullable, but there are no restrictions on its type category.
            {
                using var guard = context.Push(metadata.LocaleProperty);

                if (metadata.IsLocaleNullable) {
                    throw new InvalidNativeNullabilityException(context, "the Locale type of a Localization");
                }
            }
        }
    }
}