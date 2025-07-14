using System;
using System.Collections.Generic;

namespace Kvasir.Translation {
    /// <summary>
    ///   A translation of a single CLR type.
    /// </summary>
    /// 
    /// <param name="CLRSource">The CLR <see cref="Type"/> that produced this <see cref="EntityTranslation"/>.</param>
    /// <param name="Principal">The definition of the Principal Table.</param>
    /// <param name="Relations">The definitions of any Relation Tables.</param>
    /// <param name="Localizations">The definition of any Localizations.</param>
    internal sealed record class EntityTranslation(
        Type CLRSource,
        PrincipalTableDef Principal,
        IReadOnlyList<RelationTableDef> Relations,
        IReadOnlyList<LocalizationDef> Localizations
    );
}
