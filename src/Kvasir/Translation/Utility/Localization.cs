using Cybele.Extensions;
using Kvasir.Localization;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Kvasir.Translation {
    /// <summary>
    ///   The metadata describing the translation of a Localization type.
    /// </summary>
    ///
    /// <param name="KeyProperty">The property defining the Localization Key.</param>
    /// <param name="KeyType">The type of the Localization Key.</param>
    /// <param name="LocaleType">The type of the Localization Locale.</param>
    /// <param name="ValueType">The type of the Localization Value.</param>
    /// <param name="IsKeyNullable"><see langword="true"/> if the Localization Key is nullable; otherwise, <see langword="false"/>.</param>
    /// <param name="IsLocaleNullable"><see langword="true"/> if the Localization Locale is nullable; otherwise, <see langword="false"/>./param>
    /// <param name="IsKeyNullable"><see langword="true"/> if the Localization Value is nullable; otherwise, <see langword="false"/>./param>
    /// <param name="FirstDerivedProperty">The first derived property that is included in the data model, if any.</param>
    internal readonly record struct LocalizationMetadata(
        PropertyInfo KeyProperty,
        Type KeyType,
        Type LocaleType,
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
        ///   Reflects over a Localization type, extracting the important properties, types, and nullability statuses.
        /// </summary>
        /// <param name="localization">
        ///   The Localization type.
        /// </param>
        /// <returns>
        ///   A <see cref="LocalizationMetadata"/> for <paramref name="localization"/>.
        /// </returns>
        public static LocalizationMetadata Reflect(Type localization) {
            Debug.Assert(localization.IsInstanceOf(typeof(ILocalization)) && localization != typeof(ILocalization));

            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy;
            var properties = localization.GetProperties(flags);
            var baseProperties = properties.Where(p => p.DeclaringType!.IsGenericType && p.DeclaringType.GetGenericTypeDefinition() == typeof(Localization<,,>));
            var derivedProperties = properties.Where(p => !baseProperties.Contains(p) && p.TranslationCategory().Equals(PropertyCategory.InDataModel));

            // The `Key` and `Relation` properties are directly on the base Localization<> type, so we need to make sure
            // to pull them from there to avoid picking up derived properties. The `Relation` type has a bunch of
            // properties inherited from the base collection interfaces; the one at index [5] is the `ConnectionType`,
            // which holds the key-value pair from which we can extract the `Locale` and the `Value`.
            var keyProperty = baseProperties.Where(p => p.Name == "Key").First();
            var relationProperty = baseProperties.Where(p => p.Name == "Relation").First();
            var localeProperty = ((Type)relationProperty.PropertyType.GetProperties(flags)[5]!.GetValue(null)!).GetProperty("Key", flags)!;
            var valueProperty = ((Type)relationProperty.PropertyType.GetProperties(flags)[5]!.GetValue(null)!).GetProperty("Value", flags)!;

            var keyNullabilityInfo = new NullabilityInfoContext().Create(keyProperty);
            var relationNullabilityInfo = new NullabilityInfoContext().Create(relationProperty);

            return new LocalizationMetadata() {
                KeyProperty = keyProperty,
                KeyType = keyProperty.PropertyType,
                LocaleType = localeProperty.PropertyType,
                ValueType = valueProperty.PropertyType,
                IsKeyNullable = keyNullabilityInfo.ReadState == NullabilityState.Nullable,
                IsLocaleNullable = relationNullabilityInfo.GenericTypeArguments[0].ReadState == NullabilityState.Nullable,
                IsValueNullable = relationNullabilityInfo.GenericTypeArguments[1].ReadState == NullabilityState.Nullable,
                FirstDerivedProperty = derivedProperties.OrderBy(p => p.Name).FirstOrDefault()
            };
        }
    }
}
