using Kvasir.Core;
using Kvasir.Exceptions;
using Kvasir.Schema;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public Translation this[Type clr] => TranslateEntity(clr);

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
            typeCache_ = new Dictionary<Type, TypeDescriptor>();
            entityCache_ = new Dictionary<Type, Translation>();
            tableNames_ = new HashSet<TableName>();
        }


        private readonly Settings settings_;
        private readonly Assembly sourceAssembly_;
        private readonly Dictionary<Type, TypeDescriptor> typeCache_;
        private readonly Dictionary<Type, Translation> entityCache_;
        private readonly HashSet<TableName> tableNames_;
        private const char NAME_SEPARATOR = '.';
        private const char PATH_SEPARATOR = '.';
    }
}
