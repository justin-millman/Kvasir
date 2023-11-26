using System;
using Kvasir.Translation.Synthetic;

namespace Kvasir.Translation {
    internal sealed partial class Translator {
        /// <summary>
        ///   Translates a Relation.
        /// </summary>
        /// <param name="descriptor">
        ///   The <see cref="IRelationDescriptor"/> defining the Relation.
        /// </param>
        /// <param name="owningEntity">
        ///   The Entity <see cref="Type"/> against which the Relation exists.
        /// </param>
        /// <returns>
        ///   The <see cref="RelationTableDef"/> that corresponds to <paramref name="descriptor"/>.
        /// </returns>
        private RelationTableDef TranslateRelation(IRelationDescriptor descriptor, Type owningEntity) {
            Type synthetic = new SyntheticType(descriptor, owningEntity);
            var translation = this[synthetic];
            return new RelationTableDef(translation.Principal.Table, null!, null!);
        }
    }
}
