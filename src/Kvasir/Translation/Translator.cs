using Kvasir.Core;
using Kvasir.Localization;
using Kvasir.Reconstitution;
using Kvasir.Schema;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Kvasir.Translation {
    /// <summary>
    ///   The Translator, which converts CLR Entity Types into schema models and data manipulation (e.g. extraction,
    ///   reconstitution, repopulation) plans.
    /// </summary>
    internal sealed partial class Translator {
        /// <summary>
        ///   A tag dispatch constant for overloading `this[]` to translate Localizations.
        /// </summary>
        public static readonly LocalizationTag AsLocalzation = new LocalizationTag();

        /// <summary>
        ///   Translates a non-Localization CLR type, and any other type referenced by it.
        /// </summary>
        /// <param name="source">
        ///   The CLR <see cref="Type"/> to be translated.
        /// </param>
        /// <returns>
        ///   [GET] The translation of <paramref name="source"/>.
        /// </returns>
        /// <exception cref="Kvasir.Exceptions.KvasirException">
        ///   if <paramref name="source"/> cannot be translated (for example, it contains an invalid annotation or
        ///   has a reference cycle); note that the translation error may occur on a different type that is being
        ///   translated due to a reference from <paramref name="source"/>.
        /// </exception>
        public EntityTranslation this[Type source] {
            get {
                Debug.Assert(!IsLocalizationType(source));

                if (translationCache_.TryGetValue(source, out var translation)) {
                    logger_.LogDebug("{} already translated; returning memoization from `operator[]`", source.Name);
                    return translation;
                }

                var context = new Context(source);
                var principal = TranslatePrincipalTable(context, source);
                var relations = TranslateRelationTables(source);

                var traits = IsPreDefined(source) ? TranslationTraits.RequirePreDefined : TranslationTraits.None;
                var sourceTypes = Enumerable.Repeat(source, 1).Concat(relationTypesFromEntity_[source]);
                foreach (var type in sourceTypes) {
                    foreach (var tracker in localizationTrackersCache_[type]) {
                        var localizationContext = tracker.AsContextOn(type);
                        localizationContext.ResetReferences();
                        TranslateType(localizationContext, tracker.Property.PropertyType, traits);
                    }
                }

                var result = new EntityTranslation(CLRSource: source, Principal: principal, Relations: relations);
                translationCache_[source] = result;
                return result;
            }
        }

        /// <summary>
        ///   Translates a Localization CLR type, and any other type referenced by it.
        /// </summary>
        /// <param name="source">
        ///   The CLR <see cref="Type"/> to be translated.
        /// </param>
        /// <param name="_">
        ///   <i>tag dispatch argument</i>
        /// </param>
        /// <returns>
        ///   [GET] The translation of <paramref name="source"/>.
        /// </returns>
        /// <exception cref="Kvasir.Exceptions.KvasirException">
        ///   if <paramref name="source"/> cannot be translated (for example, it contains an invalid Localization Key
        ///   type); note that the translation error may occur on a different type that is being translated due to a
        ///   reference from <paramref name="source"/>.
        /// </exception>
        public LocalizationTranslation this[Type source, LocalizationTag _] {
            get {
                Debug.Assert(IsLocalizationType(source));

                if (localizationCache_.TryGetValue(source, out var translation)) {
                    logger_.LogDebug("{} already translated; returning memoization from `operator[]`", source.Name);
                    return translation;
                }

                var context = new Context(source);
                var principal = TranslateLocalizationTable(context, source);

                var result = new LocalizationTranslation(CLRSource: source, Principal: principal);
                localizationCache_[source] = result;
                return result;
            }
        }

        /// <summary>
        ///   Determines if a CLR type is a Localization Type.
        /// </summary>
        /// <param name="type">
        ///   The CLR <see cref="Type"/> probe.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if <paramref name="type"/> is a Localization Type; otherwise,
        ///   <see langword="false"/>.
        /// </returns>
        public static bool IsLocalizationType(Type? type) {
            if (type is null) {
                return false;
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Localization<,,>)) {
                return true;
            }
            else {
                return IsLocalizationType(type.BaseType);
            }
        }

        /// <summary>
        ///   Constructs a new <see cref="Translator"/> that uses default <see cref="Settings"/>.
        /// </summary>
        /// <param name="entityLookup">
        ///   The function used to look up the collection of existing Entities for a given <see cref="Type"/>.
        /// </param>
        /// <param name="logger">
        ///   The<see cref="ILogger">logger</see> with which to issue diagnostics.
        /// </param>
        public Translator(Func<Type, IEnumerable<object>> entityLookup, ILogger logger)
            : this(entityLookup, Settings.Default, logger) {

            // Need to reset this because the constructor delegation causes the "calling assembly" to actually be the
            // one of the Translator itself, but we need it to be the assembly that initially called into the Translator
            callingAssembly_ = Assembly.GetCallingAssembly();
        }

        /// <summary>
        ///   Constructs a new <see cref="Translator"/> that uses custom <see cref="Settings"/>.
        /// </summary>
        /// <param name="entityLookup">
        ///   The function used to look up the collection of existing Entities for a given <see cref="Type"/>.
        /// </param>
        /// <param name="settings">
        ///   The <see cref="Settings"/> according to which to perform the translation.
        /// </param>
        /// <param name="logger">
        ///   The <see cref="ILogger">logger</see> with which to issue diagnostics.
        /// </param>
        /// <remarks>
        ///   Note that the settings are not currently used for anything; in fact, there are no traits available in the
        ///   <see cref="Settings"/> class. Instead, the settings serve as a forward compatibility mechanism that allows
        ///   us to provide customization of behaviors in the future without necessitating a significant redesign.
        /// </remarks>
        public Translator(Func<Type, IEnumerable<object>> entityLookup, Settings settings, ILogger logger) {
            Debug.Assert(entityLookup is not null);
            Debug.Assert(settings is not null);
            Debug.Assert(logger is not null);

            settings_ = settings;
            callingAssembly_ = Assembly.GetCallingAssembly();
            entityLookup_ = entityLookup;
            typeCache_ = [];
            translationCache_ = [];
            localizationCache_ = [];
            principalTableCache_ = [];
            localizationTableCache_ = [];
            tableNameCache_ = [];
            pkCache_ = [];
            relationTrackersCache_ = [];
            localizationTrackersCache_ = [];
            keyMatchers_ = [];
            relationTypesFromEntity_ = [];
            logger_ = logger;
        }


        private readonly Settings settings_;
        private readonly Assembly callingAssembly_;
        private readonly Func<Type, IEnumerable<object>> entityLookup_;
        private readonly Dictionary<Type, IReadOnlyList<FieldGroup>> typeCache_;
        private readonly Dictionary<Type, EntityTranslation> translationCache_;
        private readonly Dictionary<Type, LocalizationTranslation> localizationCache_;
        private readonly Dictionary<Type, PrincipalTableDef> principalTableCache_;
        private readonly Dictionary<Type, LocalizationTableDef> localizationTableCache_;
        private readonly Dictionary<TableName, Type> tableNameCache_;
        private readonly Dictionary<Type, IReadOnlyList<FieldGroup>> pkCache_;
        private readonly Dictionary<Type, IReadOnlyList<RelationTracker>> relationTrackersCache_;
        private readonly Dictionary<Type, IReadOnlyList<LocalizationTracker>> localizationTrackersCache_;
        private readonly Dictionary<Type, KeyMatcher> keyMatchers_;
        private readonly Dictionary<Type, IReadOnlyList<Type>> relationTypesFromEntity_;
        private readonly ILogger logger_;
    }


    /// A tag type for overloading `Translator.this[]` between Localizations and non-Localizations.
    internal readonly struct LocalizationTag {}
}