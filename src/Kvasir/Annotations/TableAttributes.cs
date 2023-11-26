using Kvasir.Schema;
using System;

namespace Kvasir.Annotations {
    /// <summary>
    ///   An annotation that specifies the name of the Table backing a particular class.
    /// </summary>
    /// <remarks>
    ///   The Kvasir framework will automatically determine the name of backing Tables based on the name of the POCO
    ///   class. The <see cref="TableAttribute"/> can be used to override the default deduction when that deduction
    ///   would be incorrect or undesirable.
    /// </remarks>
    /// <seealso cref="FieldName"/>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class TableAttribute : Attribute {
        /// <summary>
        ///   The Table name specified by the annotation.
        /// </summary>
        internal string Name { get; }

        /// <summary>
        ///   Constructs a new instance of the <see cref="TableAttribute"/>.
        /// </summary>
        /// <param name="name">
        ///   The name of the Table.
        /// </param>
        public TableAttribute(string name) {
            Name = name;
        }
    }

    /// <summary>
    ///   An annotation that specifies the name of the Relation Table backing a particular relation-type property.
    /// </summary>
    /// <remarks>
    ///   The Kvasir framework will automatically determine the name of backing Relation Tables based on the name of the
    ///   in-code property and the owning POCO class. The <see cref="RelationTableAttribute"/> can be used to override
    ///   the default deduction when that deduction would be incorrect or undesirable.
    /// </remarks>
    /// <seealso cref="FieldName"/>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class RelationTableAttribute : Attribute {
        /// <summary>
        ///   The Table name specified by the annotation.
        /// </summary>
        internal string Name { get; }

        /// <summary>
        ///   Constructs a new instance of the <see cref="RelationTableAttribute"/>.
        /// </summary>
        /// <param name="name">
        ///   The name of the Table.
        /// </param>
        public RelationTableAttribute(string name) {
            Name = name;
        }
    }
}
