using Kvasir.Core;
using Kvasir.Schema;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Kvasir.Translation {
    /// <summary>
    ///   The component responsible for reflecting over CLR Types and properties to create a data model.
    /// </summary>
    internal sealed partial class Translator {
        /// <summary>
        ///   Translate a CLR Type.
        /// </summary>
        /// <param name="clr">
        ///   The source CLR <see cref="Type"/>.
        /// </param>
        /// <returns>
        ///   [GET] The translation associated with <paramref name="clr"/>, creating it if necessary. Note that if
        ///   <paramref name="clr"/> has any relation-type properties, additional Entity Types may be translated as
        ///   well.
        /// </returns>
        public Translation this[Type clr] => TranslateEntity(clr);

        /// <summary>
        ///   Create a new <see cref="Translator"/> with default settings.
        /// </summary>
        public Translator()
            : this(Settings.Default) {

            callingAssembly_ = Assembly.GetCallingAssembly();
        }

        /// <summary>
        ///   Create a new <see cref="Translator"/> with custom settings.
        /// </summary>
        /// <param name="settings">
        ///   The custom <see cref="Settings">settings</see>
        /// </param>
        /// <remarks>
        ///   Note that the <paramref name="settings"/> are not currently used: they exist as a forward-compatible
        ///   customization point to provide control over Translation behaviors.
        /// </remarks>
        public Translator(Settings settings) {
            settings_ = settings;
            callingAssembly_ = Assembly.GetCallingAssembly();
            typeCache_ = new Dictionary<Type, TypeDescriptor>();
            entityCache_ = new Dictionary<Type, Translation>();
            tableNames_ = new HashSet<TableName>();
        }


        private readonly Settings settings_;
        private readonly Assembly callingAssembly_;
        private readonly Dictionary<Type, TypeDescriptor> typeCache_;
        private readonly Dictionary<Type, Translation> entityCache_;
        private readonly HashSet<TableName> tableNames_;
    }
}
