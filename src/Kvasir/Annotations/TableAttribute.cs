using Kvasir.Schema;
using System;

namespace Kvasir.Annotations {
    /// <summary>
    ///   An annotation that specifies the name of the Table backing a particula class.
    /// </summary>
    /// <remarks>
    ///   The Kvasir framework will automatically determine the name of backing Tables based on the name of the POCO
    ///   class. The <see cref="TableAttribute"/> can be used to override the default deduction when that deduction
    ///   would be incorrect or undesirable.
    /// </remarks>
    /// <seealso cref="FieldName"/>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class TableAttribute : Attribute {
        /// <summary>
        ///   The <see cref="TableName"/> specified by the annotation.
        /// </summary>
        internal TableName Name { get; }

        /// <summary>
        ///   Constructs a new instance of the <see cref="TableAttribute"/>.
        /// </summary>
        /// <param name="name">
        ///   The name of the Table.
        /// </param>
        /// <exception cref="ArgumentException">
        ///   if <paramref name="name"/> is not a valid name for a database Table.
        /// </exception>
        public TableAttribute(string name) {
            Name = new TableName(name);
        }
    }
}
