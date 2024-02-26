using Kvasir.Schema;
using System.Collections.Generic;

namespace Kvasir.Translation2 {
    /// <summary>
    ///   The translated schema components of a single Field.
    /// </summary>
    /// 
    /// <param name="Descriptor">The <see cref="FieldDescriptor"/> that produced the schema.</param>
    /// <param name="Field">The <see cref="IField">Field</see> schema.</param>
    /// <param name="CHECKs">The <c>CHECK</c> constraint clauses that apply to <paramref name="Field"/>.</param>
    internal readonly record struct FieldSchema(
        FieldDescriptor Descriptor,
        IField Field,
        IEnumerable<CheckConstraint> CHECKs
    );
}
