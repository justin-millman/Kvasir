using Kvasir.Extraction;
using Kvasir.Reconstitution;
using Kvasir.Schema;
using System.Collections.Generic;

namespace Kvasir.Translation {
    /// <summary>
    ///   The definition of a Principal Table.
    /// </summary>
    /// 
    /// <param name="Table">The schema model for the Principal Table.</param>
    /// <param name="Extractor">The plan that can extract a row of data to be stored into the Principal Table.</param>
    /// <param name="Reconstitutor">The plan that can recreate a CLR object from a row of data stored in the Principal Table.</param>
    /// <param name="KeyExtractor">The plan that can extract the subset of a row of data that constitute's the Primary Key in the Principal Table.</param>
    /// <param name="PreDefinedInstances">The pre-defined instances to be populated into the Principal Table; empty for regular Entities.</param>
    internal sealed record class PrincipalTableDef(
        ITable Table,
        DataExtractionPlan Extractor,
        DataReconstitutionPlan Reconstitutor,
        DataExtractionPlan KeyExtractor,
        IReadOnlyList<object> PreDefinedInstances
    );

    /// <summary>
    ///   The definition of a Relation Table.
    /// </summary>
    /// 
    /// <param name="Table">The schema model for the Relation Table.</param>
    /// <param name="Extractor">The plan that can extract the element-specific data to be stored in the Relation Table.</param>
    /// <param name="Repopulator">The plan that can populate elements into a CLR Relation from a row of data stored in the Relation Table.</param>
    internal sealed record class RelationTableDef(
        ITable Table,
        RelationExtractionPlan Extractor,
        RelationRepopulationPlan Repopulator
    );
}
