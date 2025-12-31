using System;
using System.Collections.Generic;

namespace Kvasir.Translation {
    /// <summary>
    ///   A translation of a single non-Localization CLR type.
    /// </summary>
    /// 
    /// <param name="CLRSource">The CLR <see cref="Type"/> that produced this <see cref="EntityTranslation"/></param>
    /// <param name="Principal">The definition of the Principal Table</param>
    /// <param name="Relations">The definitions of any Relation Tables</param>
    internal sealed record class EntityTranslation(
        Type CLRSource,
        PrincipalTableDef Principal,
        IReadOnlyList<RelationTableDef> Relations
    );

    /// <summary>
    ///   A translation of a single Localization CLR type.
    /// </summary>
    internal sealed record class LocalizationTranslation(
        Type CLRSource,
        LocalizationTableDef Principal
    );
}
