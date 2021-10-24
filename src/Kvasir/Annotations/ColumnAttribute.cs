using Ardalis.GuardClauses;
using System;

namespace Kvasir.Annotations {
    /// <summary>
    ///   An annotation that specifies the column index for the Field backing a particular property.
    /// </summary>
    /// <remarks>
    ///   A <see cref="ColumnAttribute"/> that is applied to a property of aggregate type defines the offset of the
    ///   nested properties thereof. When applied to a property within an aggregate, the absolute position of the
    ///   backing Field will depend on the offset defined by the owning Entity (and, therefore, can be different from
    ///   Entity to Entity).
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class ColumnAttribute : Attribute {
        /// <summary>
        ///   The <c>0</c>-based column index.
        /// </summary>
        internal int Column { get; }

        /// <summary>
        ///   Constructs a new instance of the <see cref="ColumnAttribute"/> class.
        /// </summary>
        /// <param name="column">
        ///   The <c>0</c>-based column index.
        /// </param>
        /// <exception cref="ArgumentException">
        ///   [on evaluation] if <paramref name="column"/> is less than <c>0</c>.
        /// </exception>
        public ColumnAttribute(int column) {
            Column = Guard.Against.Negative(column, nameof(column));
        }
    }
}
