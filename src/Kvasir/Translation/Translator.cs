using Cybele.Extensions;
using Kvasir.Core;
using Kvasir.Exceptions;
using Kvasir.Schema;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Kvasir.Translation {
    /// <summary>
    ///   The component responsible for reflecting over CLR Types and properties to create a data model.
    /// </summary>
    internal sealed partial class Translator {
        /// <summary>
        ///   Translates a CLR Type.
        /// </summary>
        /// <param name="clr">
        ///   The source CLR <see cref="Type"/>.
        /// </param>
        /// <returns>
        ///   [GET] The translation associated with <paramref name="clr"/>, creating it if necessary. Note that if
        ///   <paramref name="clr"/> has any relation-type properties, additional Entity Types may be translated as
        ///   well.
        /// </returns>
        /// <exception cref="KvasirException">
        ///   if <paramref name="clr"/> contains a property that cannot be translated into Kvasir's data model, has any
        ///   invalid annotations, encompasses a cyclic relation, or otherwise cannot be made into a Translation.
        /// </exception>
        public Translation this[Type clr] {
            get {
                if (entityCache_.TryGetValue(clr, out Translation? cached)) {
                    return cached;
                }

                Translation? result = null;
                _ = TranslateEntity(clr);

                // We cannot iterate via for-each, because the list of intermediates may grow as Relations are
                // translated, since each Relation itself may refer to other Entities for the first time. The growth
                // will always be append-to-back (and everything is single-threaded), so removal from anywhere in the
                // list at any time is legal.
                while (intermediates_.Any(intermediate => !intermediate.BeingCompleted)) {
                    // Here's why we have to use this "BeingCompleted" flag. When we translate a Relation, there is a
                    // Field whose type is the owning Entity. This will trigger a Reference Base Translation, which will
                    // try to re-translate the owning Entity. Since the owning Entity's translation is not fully
                    // complete (we're still processing its Relations), there's no corresponding cache entry and we have
                    // to perform a lookup in the intermediates. However, in doing that, we've kept the owning Entity
                    // in the intermediates list, which means it would get re-processed when we finish translating the
                    // Synthetic Entity Type for the Relation. We therefore have to somehow flag it as being "in
                    // progress" so that we don't try to double-translate, which would lead to all sorts of problems.
                    var currentIdx = intermediates_.FindIndex(intermediate => !intermediate.BeingCompleted);
                    var current = intermediates_[currentIdx];
                    intermediates_[currentIdx] = current with { BeingCompleted = true };

                    var relations = current.Relations.Select(r => TranslateRelation(r, current.CLR));
                    var translation = new Translation(current.CLR, current.Principal, relations);
                    entityCache_.Add(current.CLR, translation);

                    if (current.CLR == clr) {
                        Debug.Assert(result is null);
                        result = translation;
                    }

                    intermediates_.RemoveAt(currentIdx);
                }

                // The result placeholder will still be `null` if the initial Entity has a reference to another Entity,
                // and that Entity has at least one Relation. In that case, the intermediate translation for the
                // referenced Entity will be in the intermediate listing first (because it was finished during the
                // translation of the original entity), so it will be post-processed first. When the translation of the
                // Synthetic Entity is completed, the original Entity will be the next "not-being-completed" item in the
                // intermediates list, and it will be finished. By the time the stack unwinds to the original call, the
                // intermediates list will be empty.
                if (result is null) {
                    return entityCache_[clr];
                }
                return result;
            }
        }

        /// <summary>
        ///   Creates a new instance of the <see cref="Translator"/> class that uses default <see cref="Settings"/>.
        /// </summary>
        public Translator()
            : this(Settings.Default) {

            sourceAssembly_ = Assembly.GetCallingAssembly();
        }

        /// <summary>
        ///   Creates a new instance of the <see cref="Translator"/> class that uses specific <see cref="Settings"/>.
        /// </summary>
        /// <param name="settings">
        ///   The <see cref="Settings"/> according to which to perform all translation.
        /// </param>
        /// <remarks>
        ///   Note that the <paramref name="settings"/> are not currently used: they exist as a forward-compatible
        ///   customization point to provide control over translation behaviors.
        /// </remarks>
        public Translator(Settings settings) {
            Debug.Assert(settings is not null);

            settings_ = settings;
            sourceAssembly_ = Assembly.GetCallingAssembly();
            inProgress_ = new Stack<Type>();
            typeCache_ = new Dictionary<Type, TypeDescriptor>();
            entityCache_ = new Dictionary<Type, Translation>();
            primaryKeyCache_ = new Dictionary<Type, FieldsListing>();
            intermediates_ = new List<IntermediateTranslation>();
            tableNames_ = new HashSet<TableName>();
        }


        private readonly Settings settings_;
        private readonly Assembly sourceAssembly_;
        private readonly Stack<Type> inProgress_;
        private readonly Dictionary<Type, TypeDescriptor> typeCache_;
        private readonly Dictionary<Type, Translation> entityCache_;
        private readonly Dictionary<Type, FieldsListing> primaryKeyCache_;
        private readonly List<IntermediateTranslation> intermediates_;
        private readonly HashSet<TableName> tableNames_;
        private const char NAME_SEPARATOR = '.';
        private const char PATH_SEPARATOR = '.';
    }
}
