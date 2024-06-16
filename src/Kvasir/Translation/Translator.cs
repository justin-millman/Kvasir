using Kvasir.Core;
using Kvasir.Schema;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace Kvasir.Translation {
    /// <summary>
    ///   The Translator, which converts CLR Entity Types into schema models and data manipulation (e.g. extraction,
    ///   reconstitution, repopulation) plans.
    /// </summary>
    internal sealed partial class Translator {
        /// <summary>
        ///   Translates a CLR type, and any other type reference by it.
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
        public Translation this[Type source] {
            get {
                var context = new Context(source);
                var principal = TranslatePrincipalTable(context, source);
                var relations = TranslateRelationTables(source);
                return new Translation(CLRSource: source, Principal: principal, Relations: relations);
            }
        }

        /// <summary>
        ///   Constructs a new <see cref="Translator"/> that uses default <see cref="Settings"/>.
        /// </summary>
        public Translator()
            : this(Settings.Default) {

            // Need to reset this because the constructor delegation causes the "calling assembly" to actually be the
            // one of the Translator itself, but we need it to be the assembly that initially called into the Translator
            callingAssembly_ = Assembly.GetCallingAssembly();
        }

        /// <summary>
        ///   Constructs a new <see cref="Translator"/> that uses custom <see cref="Settings"/>.
        /// </summary>
        /// <param name="settings">
        ///   The <see cref="Settings"/> according to which to perform the translation.
        /// </param>
        /// <remarks>
        ///   Note that the settings are not currently used for anything; in fact, there are no traits available in the
        ///   <see cref="Settings"/> class. Instead, the settings serve as a forward compatibility mechanism that allows
        ///   us to provide customization of behaviors in the future without necessitating a significant redesign.
        /// </remarks>
        public Translator(Settings settings) {
            Debug.Assert(settings is not null);

            settings_ = settings;
            callingAssembly_ = Assembly.GetCallingAssembly();
            typeCache_ = new Dictionary<Type, IReadOnlyList<FieldGroup>>();
            principalTableCache_ = new Dictionary<Type, PrincipalTableDef>();
            tableNameCache_ = new Dictionary<TableName, Type>();
            pkCache_ = new Dictionary<Type, IReadOnlyList<FieldGroup>>();
            relationTrackersCache_ = new Dictionary<Type, IReadOnlyList<RelationTracker>>();
        }


        private readonly Settings settings_;
        private readonly Assembly callingAssembly_;
        private readonly Dictionary<Type, IReadOnlyList<FieldGroup>> typeCache_;
        private readonly Dictionary<Type, PrincipalTableDef> principalTableCache_;
        private readonly Dictionary<TableName, Type> tableNameCache_;
        private readonly Dictionary<Type, IReadOnlyList<FieldGroup>> pkCache_;
        private readonly Dictionary<Type, IReadOnlyList<RelationTracker>> relationTrackersCache_;
    }
}
