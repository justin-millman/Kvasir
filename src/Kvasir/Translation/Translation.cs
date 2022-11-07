using System;
using System.Diagnostics;

namespace Kvasir.Translation {
    /// <summary>
    ///   A translation of a single CLR Type.
    /// </summary>
    internal sealed record class Translation {
        /// <summary>
        ///   The source CLR <see cref="Type"/>.
        /// </summary>
        public Type CLRSource { get; }

        /// <summary>
        ///   The definition of the principal Table of the <see cref="CLRSource">source Type</see>.
        /// </summary>
        public PrincipalTableDef Principal { get; }

        /// <summary>
        ///   Construct a new <see cref="Translation"/>.
        /// </summary>
        /// <param name="clr">
        ///   The <see cref="CLRSource">source CLR Type</see>.
        /// </param>
        /// <param name="principal">
        ///   The <see cref="Principal">principal Table definition</see>.
        /// </param>
        public Translation(Type clr, PrincipalTableDef principal) {
            Debug.Assert(clr is not null);
            Debug.Assert(principal is not null);

            CLRSource = clr;
            Principal = principal;
        }

        /* Because Translation is a record type, the following methods are synthesized automatically by the compiler:
         *   > public Translation(Translation rhs)
         *   > public bool Equals(Translation? rhs)
         *   > public sealed override bool Equals(object? rhs)
         *   > public sealed override int GetHashCode()
         *   > public sealed override string ToString()
         *   > public static bool operator==(Translation? lhs, Translation? rhs)
         *   > public static bool oeprator!=(Translation? lhs, Translation? rhs)
         */
    }
}
