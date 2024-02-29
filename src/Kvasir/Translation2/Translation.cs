using System;
using System.Collections.Generic;

namespace Kvasir.Translation2 {
    /// <summary>
    ///   A translation of a single CLR type.
    /// </summary>
    /// 
    /// <param name="CLRSource">The CLR <see cref="Type"/> that produced this <see cref="Translation"/></param>
    /// <param name="Principal">The definition of the Principal Table</param>
    /// <param name="Relations">The definitions of any Relation Tables</param>
    internal sealed record class Translation(
        Type CLRSource,
        PrincipalTableDef Principal,
        IReadOnlyList<RelationTableDef> Relations
    );
}
