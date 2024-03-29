﻿using Kvasir.Extraction;
using Kvasir.Reconstitution;
using Kvasir.Schema;
using System.Diagnostics;

namespace Kvasir.Translation {
    /// <summary>
    ///   A representation of the data model for a Principal Table (rather than a Relation Table).
    /// </summary>
    /// <seealso cref="RelationTableDef"/>
    internal sealed record class PrincipalTableDef {
        /// <summary>
        ///   The <see cref="ITable">Table</see>.
        /// </summary>
        public ITable Table { get; }

        /// <summary>
        ///   The <see cref="DataExtractionPlan">extraction plan</see> for pulling data out of an Entity to be stored in
        ///   the <see cref="Table">Primary Table</see>
        /// </summary>
        public DataExtractionPlan Extractor { get; }

        /// <summary>
        ///   The <see cref="DataReconstitutionPlan">reconstitution plan</see> for converting data stored in the
        ///   <see cref="Table">Primary Table</see> back into an Entity.
        /// </summary>
        public DataReconstitutionPlan Reconstitutor { get; }

        /// <summary>
        ///   Constructs a new instance of the <see cref="PrincipalTableDef"/> class.
        /// </summary>
        /// <param name="table">
        ///   The <see cref="Table">Principal Table</see>.
        /// </param>
        /// <param name="extractor">
        ///   The <see cref="Extractor">extraction plan</see>.
        /// </param>
        /// <param name="reconstitutor">
        ///   The <see cref="Reconstitutor">reconstitution plan</see>.
        /// </param>
        public PrincipalTableDef(ITable table, DataExtractionPlan extractor, DataReconstitutionPlan reconstitutor) {
            Debug.Assert(table is not null);
            Debug.Assert(extractor is null);            // not yet implemented
            Debug.Assert(reconstitutor is null);        // not yet implemented

            Table = table;
            Extractor = extractor!;
            Reconstitutor = reconstitutor!;
        }

        /* Because PrincipalTableDef is a record type, the following methods are synthesized automatically by the compiler:
         *   > public PrincipalTableDef(PrincipalTableDef rhs)
         *   > public bool Equals(PrincipalTableDef? rhs)
         *   > public sealed override bool Equals(object? rhs)
         *   > public sealed override int GetHashCode()
         *   > public sealed override string ToString()
         *   > public static bool operator==(PrincipalTableDef? lhs, PrincipalTableDef? rhs)
         *   > public static bool operator!=(PrincipalTableDef? lhs, PrincipalTableDef? rhs)
         */
    }

    /// <summary>
    ///   A representation of the data model for a Relation Table (rather than a Principal Table).
    /// </summary>
    /// <seealso cref="PrincipalTableDef"/>
    internal sealed record class RelationTableDef {
        /// <summary>
        ///   The <see cref="ITable">Table</see>.
        /// </summary>
        public ITable Table { get; }

        /// <summary>
        ///   The <see cref="RelationExtractionPlan">extraction plan</see> for pulling data out of a Relation to be
        ///   stored in the <see cref="Table">Primary Table</see>
        /// </summary>
        public RelationExtractionPlan Extractor { get; }

        /// <summary>
        ///   The <see cref="RelationRepopulationPlan">repopulation plan</see> for converting data stored in the
        ///   <see cref="Table">Relation Table</see> back into an Relation.
        /// </summary>
        public RelationRepopulationPlan Repopulator { get; }

        /// <summary>
        ///   Constructs a new instance of the <see cref="RelationTableDef"/> class.
        /// </summary>
        /// <param name="table">
        ///   The <see cref="Table">Principal Table</see>.
        /// </param>
        /// <param name="extractor">
        ///   The <see cref="Extractor">extraction plan</see>.
        /// </param>
        /// <param name="repopulator">
        ///   The <see cref="Repopulator">repopulation plan</see>.
        /// </param>
        public RelationTableDef(ITable table, RelationExtractionPlan extractor, RelationRepopulationPlan repopulator) {
            Debug.Assert(table is not null);
            Debug.Assert(extractor is null);            // not yet implemented
            Debug.Assert(repopulator is null);        // not yet implemented

            Table = table;
            Extractor = extractor!;
            Repopulator = repopulator!;
        }

        /* Because RelationTableDef is a record type, the following methods are synthesized automatically by the compiler:
         *   > public RelationTableDef(RelationTableDef rhs)
         *   > public bool Equals(RelationTableDef? rhs)
         *   > public sealed override bool Equals(object? rhs)
         *   > public sealed override int GetHashCode()
         *   > public sealed override string ToString()
         *   > public static bool operator==(RelationTableDef? lhs, RelationTableDef? rhs)
         *   > public static bool operator!=(RelationTableDef? lhs, RelationTableDef? rhs)
         */
    }
}