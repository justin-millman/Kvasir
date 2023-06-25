using Cybele.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Kvasir.Translation {
    /// <summary>
    ///   A translation of a single Entity Type.
    /// </summary>
    internal sealed record class Translation {
        /// <summary>
        ///   The source CLR <see cref="Type"/>.
        /// </summary>
        public Type CLRSource { get; }

        /// <summary>
        ///   The definition of the Principal Table of the <see cref="CLRSource">source Entity type</see>.
        /// </summary>
        public PrincipalTableDef Principal { get; }

        /// <summary>
        ///   The definitions, no particular order, of the Relation Tables for the
        ///   <see cref="CLRSource">source Entity type</see>.
        /// </summary>
        public IReadOnlyList<RelationTableDef> Relations { get; }

        /// <summary>
        ///   Constructs a new instance of the <see cref="Translation"/> class.
        /// </summary>
        /// <param name="clr">
        ///   The <see cref="CLRSource">CLR source type</see>.
        /// </param>
        /// <param name="principal">
        ///   The <see cref="Principal">definition of the Principal Table</see>.
        /// </param>
        /// <param name="relations">
        ///   The <see cref="Relations">definitions of the Relation Tables</see>, if any.
        /// </param>
        public Translation(Type clr, PrincipalTableDef principal, IEnumerable<RelationTableDef> relations) {
            Debug.Assert(clr is not null);
            Debug.Assert(principal is not null);
            Debug.Assert(relations is not null);
            Debug.Assert(relations.IsEmpty());

            CLRSource = clr;
            Principal = principal;
            Relations = new List<RelationTableDef>(relations);
        }
    }
}
